using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using com.centralaz.SexualOffendersMatch.Model;
using com.centralaz.SexualOffendersMatch.Data;
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
            //For each row in excel file
            while ( true )
            {
                SexualOffender excelRowSexualOffender = new SexualOffender();
                if ( offenderList.ContainsKey( excelRowSexualOffender.KeyString ) )
                {
                    offenderList[excelRowSexualOffender.KeyString] = excelRowSexualOffender;
                }
                else
                {
                    offenderList.Add( excelRowSexualOffender.KeyString, excelRowSexualOffender );
                }
            }


            return true;
        }
    }
}
