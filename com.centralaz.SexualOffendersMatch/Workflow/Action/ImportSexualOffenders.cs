using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

using LumenWorks.Framework.IO.Csv;

using com.centralaz.SexualOffendersMatch.Model;
using com.centralaz.SexualOffendersMatch.Data;
using System.IO;
namespace com.centralaz.SexualOffendersMatch.Workflow.Action
{
    /// <summary>
    /// Sets the name of the workflow
    /// </summary>
    [Description( "Sets the name of the workflow" )]
    [Export( typeof( Rock.Workflow.ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Set Workflow Name" )]
    [WorkflowAttribute( "DPS Excel File" )]
    public class ImportSexualOffenders : Rock.Workflow.ActionComponent
    {
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            //Get the dictionary of current SOs <string keyword, SO offender>
            Dictionary<string, SexualOffender> offenderList = new SexualOffenderService( new SexualOffendersMatchContext() ).Queryable().OrderBy( k => k.KeyString ).ToDictionary( k => k.KeyString );

            //Get the excel file
            Guid dpsFileGuid = action.Activity.Workflow.GetAttributeValue( "DPSExcelFile" ).AsGuid();
            BinaryFile binaryFile = new BinaryFileService( rockContext ).Get( dpsFileGuid );

            using ( CsvReader csvReader = new CsvReader( new StreamReader( binaryFile.Data.ContentStream ), true ) )
            {
                int fieldCount = csvReader.FieldCount;

                string[] headers = csvReader.GetFieldHeaders();
                //For each row in excel file

                while ( csvReader.ReadNextRecord() )
                {
                    //Build new SO Object
                    SexualOffender excelRowSexualOffender = new SexualOffender();
                    excelRowSexualOffender.LastName = csvReader[csvReader.GetFieldIndex( "Last Name" )];
                    excelRowSexualOffender.FirstName = csvReader[csvReader.GetFieldIndex( "First Name" )];
                    excelRowSexualOffender.MiddleInitial = csvReader[csvReader.GetFieldIndex( "MI" )].ElementAt( 0 );
                    excelRowSexualOffender.Age = csvReader[csvReader.GetFieldIndex( "Age" )].AsInteger();
                    excelRowSexualOffender.Height = csvReader[csvReader.GetFieldIndex( "HT" )].AsInteger();
                    excelRowSexualOffender.Weight = csvReader[csvReader.GetFieldIndex( "WT" )].AsInteger();
                    excelRowSexualOffender.Race = csvReader[csvReader.GetFieldIndex( "Race" )];
                    excelRowSexualOffender.Sex = csvReader[csvReader.GetFieldIndex( "Sex" )];
                    excelRowSexualOffender.Hair = csvReader[csvReader.GetFieldIndex( "Hair" )];
                    excelRowSexualOffender.Eyes = csvReader[csvReader.GetFieldIndex( "Eyes" )];
                    excelRowSexualOffender.ResidentialAddress = csvReader[csvReader.GetFieldIndex( "Res_Add" )];
                    excelRowSexualOffender.ResidentialCity = csvReader[csvReader.GetFieldIndex( "Res_City" )];
                    excelRowSexualOffender.ResidentialState = csvReader[csvReader.GetFieldIndex( "Res_State" )];
                    excelRowSexualOffender.ResidentialZip = csvReader[csvReader.GetFieldIndex( "Res_Zip" )].AsInteger();
                    excelRowSexualOffender.VerificationDate = csvReader[csvReader.GetFieldIndex( "Verification Date" )].AsDateTime().Value;
                    excelRowSexualOffender.Offense = csvReader[csvReader.GetFieldIndex( "Offense" )];
                    excelRowSexualOffender.OffenseLevel = csvReader[csvReader.GetFieldIndex( "Level" )].AsInteger();
                    excelRowSexualOffender.Absconder = csvReader[csvReader.GetFieldIndex( "Absconder" )].AsBoolean();
                    excelRowSexualOffender.ConvictingJurisdiction = csvReader[csvReader.GetFieldIndex( "Convicting Jurisdiction" )];
                    excelRowSexualOffender.Unverified = csvReader[csvReader.GetFieldIndex( "Unverified" )].AsBoolean();

                    if ( offenderList.ContainsKey( excelRowSexualOffender.KeyString ) )
                    {
                        offenderList[excelRowSexualOffender.KeyString] = excelRowSexualOffender;
                    }
                    else
                    {
                        offenderList.Add( excelRowSexualOffender.KeyString, excelRowSexualOffender );
                    }
                }
            }

            return true;
        }
    }
}
