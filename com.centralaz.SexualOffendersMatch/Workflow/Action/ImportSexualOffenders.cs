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
    public class ImportSexualOffenders : Rock.Workflow.ActionComponent
    {
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            //Get the dictionary of current SOs <string keyword, SO offender>
            SexualOffendersMatchContext sexualOffendersMatchContext = new SexualOffendersMatchContext();
            SexualOffenderService sexualOffenderService = new SexualOffenderService( sexualOffendersMatchContext );
            Dictionary<string, SexualOffender> offenderList = sexualOffenderService.Queryable().OrderBy( k => k.KeyString ).ToDictionary( k => k.KeyString );
            sexualOffendersMatchContext.SaveChanges();


            //Get the excel file
            Guid dpsFileGuid = action.Activity.Workflow.GetAttributeValue( "DPSFile" ).AsGuid();
            BinaryFile binaryFile = new BinaryFileService( new RockContext() ).Get( dpsFileGuid );

            //For each row in excel file
            SexualOffender excelRowSexualOffender;
            using ( CsvReader csvReader = new CsvReader( new StreamReader( binaryFile.Data.ContentStream ), true ) )
            {
                while ( csvReader.ReadNextRecord() )
                {
                    if ( !string.IsNullOrWhiteSpace( csvReader[csvReader.GetFieldIndex( "Last Name" )] ) )
                    {
                        //Build new SO Object
                        excelRowSexualOffender = new SexualOffender();
                        excelRowSexualOffender.LastName = csvReader[csvReader.GetFieldIndex( "Last Name" )];
                        excelRowSexualOffender.FirstName = csvReader[csvReader.GetFieldIndex( "First Name" )];
                        if ( !string.IsNullOrWhiteSpace( csvReader[csvReader.GetFieldIndex( "MI" )] ) )
                        {
                            String middleName = csvReader[csvReader.GetFieldIndex( "MI" )];
                            char middleInitial = middleName.ElementAt( 0 );
                            excelRowSexualOffender.MiddleInitial = middleInitial;
                        }

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

                        if ( !string.IsNullOrWhiteSpace( csvReader[csvReader.GetFieldIndex( "Verification Date" )] ) && csvReader[csvReader.GetFieldIndex( "Verification Date" )].AsDateTime().HasValue )
                        {
                            excelRowSexualOffender.VerificationDate = csvReader[csvReader.GetFieldIndex( "Verification Date" )].AsDateTime().Value;
                        }
                        excelRowSexualOffender.Offense = csvReader[csvReader.GetFieldIndex( "Offense" )];
                        excelRowSexualOffender.OffenseLevel = csvReader[csvReader.GetFieldIndex( "Level" )].AsInteger();
                        excelRowSexualOffender.Absconder = csvReader[csvReader.GetFieldIndex( "Absconder" )].AsBoolean();
                        excelRowSexualOffender.ConvictingJurisdiction = csvReader[csvReader.GetFieldIndex( "Convicting Jurisdiction" )];
                        excelRowSexualOffender.Unverified = csvReader[csvReader.GetFieldIndex( "Unverified" )].AsBoolean();
                        if ( excelRowSexualOffender.KeyString == null )
                        {
                            excelRowSexualOffender.KeyString = String.Format( "{0}{1}{2}{3}{4}{5}", excelRowSexualOffender.LastName, excelRowSexualOffender.FirstName, excelRowSexualOffender.Race, excelRowSexualOffender.Sex, excelRowSexualOffender.Hair, excelRowSexualOffender.Eyes );
                        }

                        //Place SO object in existing or new spot in 
                        if ( offenderList.ContainsKey( excelRowSexualOffender.KeyString ) )
                        {
                            offenderList[excelRowSexualOffender.KeyString] = excelRowSexualOffender;
                            sexualOffendersMatchContext.SaveChanges();
                        }
                        else
                        {
                            offenderList.Add( excelRowSexualOffender.KeyString, excelRowSexualOffender );
                            sexualOffenderService.Add( excelRowSexualOffender );
                            sexualOffendersMatchContext.SaveChanges();
                        }
                    }
                }
            }

            return true;
        }
    }
}
