using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.centralaz.SexualOffendersMatch.Migrations
{
    [MigrationNumber( 2, "1.0.14" )]
    public class WorkflowConfig : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.UpdateEntityType( "Rock.Model.Workflow", "3540E9A7-FE30-43A9-8B0A-A372B63DFC93", true, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Model.WorkflowActivity", "2CB52ED0-CB06-4D62-9E2C-73B60AFA4C9F", true, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Model.WorkflowActionType", "23E3273A-B137-48A3-9AFF-C8DC832DDCA6", true, true );

            RockMigrationHelper.UpdateEntityType( "com.centralaz.SexualOffendersMatch.Workflow.Action.ImportSexualOffenders", "A95659E5-FF18-4587-8E29-9E4B393E917E", false, true );

            RockMigrationHelper.UpdateEntityType( "com.centralaz.SexualOffendersMatch.Workflow.Action.PopulatePotentialMatchesTable", "D2B6E0A1-B5A7-4BD0-89FF-6BC5BA7E175B", false, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.CompleteWorkflow", "EEDA4318-F014-4A46-9C76-4C052EF81AA1", false, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.PersistWorkflow", "F1A39347-6FE0-43D4-89FB-544195088ECF", false, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.SendEmail", "66197B01-D1F0-4924-A315-47AD54E030DE", false, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.SetAttributeFromPerson", "17962C23-2E94-4E06-8461-0FB8B94E2FEA", false, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.SetAttributeToCurrentPerson", "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", false, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.UserEntryForm", "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", false, true );

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "17962C23-2E94-4E06-8461-0FB8B94E2FEA", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "CE28B79D-FBC2-4894-9198-D923D0217549" ); // Rock.Workflow.Action.SetAttributeFromPerson:Active

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "17962C23-2E94-4E06-8461-0FB8B94E2FEA", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Attribute", "Attribute", "The person attribute to set the value of.", 0, @"", "7AC47975-71AC-4A2F-BF1F-115CF5578D6F" ); // Rock.Workflow.Action.SetAttributeFromPerson:Attribute

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "17962C23-2E94-4E06-8461-0FB8B94E2FEA", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "18EF907D-607E-4891-B034-7AA379D77854" ); // Rock.Workflow.Action.SetAttributeFromPerson:Order

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "17962C23-2E94-4E06-8461-0FB8B94E2FEA", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Person", "Person", "The person to set attribute value to. Leave blank to set person to nobody.", 1, @"", "5C803BD1-40FA-49B1-AE7E-68F43D3687BB" ); // Rock.Workflow.Action.SetAttributeFromPerson:Person

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "DE9CB292-4785-4EA3-976D-3826F91E9E98" ); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Active

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Person Attribute", "PersonAttribute", "The attribute to set to the currently logged in person.", 0, @"", "BBED8A83-8BB2-4D35-BAFB-05F67DCAD112" ); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Person Attribute

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8" ); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Order

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE" ); // Rock.Workflow.Action.UserEntryForm:Active

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "C178113D-7C86-4229-8424-C6D0CF4A7E23" ); // Rock.Workflow.Action.UserEntryForm:Order

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "66197B01-D1F0-4924-A315-47AD54E030DE", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Body", "Body", "The body of the email that should be sent. <span class='tip tip-liquid'></span> <span class='tip tip-html'></span>", 3, @"", "4D245B9E-6B03-46E7-8482-A51FBA190E4D" ); // Rock.Workflow.Action.SendEmail:Body

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "66197B01-D1F0-4924-A315-47AD54E030DE", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "36197160-7D3D-490D-AB42-7E29105AFE91" ); // Rock.Workflow.Action.SendEmail:Active

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "66197B01-D1F0-4924-A315-47AD54E030DE", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "From Email Address|Attribute Value", "From", "The email address or an attribute that contains the person or email address that email should be sent from (will default to organization email). <span class='tip tip-liquid'></span>", 0, @"", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC" ); // Rock.Workflow.Action.SendEmail:From Email Address|Attribute Value

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "66197B01-D1F0-4924-A315-47AD54E030DE", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "Send To Email Address|Attribute Value", "To", "The email address or an attribute that contains the person or email address that email should be sent to", 1, @"", "0C4C13B8-7076-4872-925A-F950886B5E16" ); // Rock.Workflow.Action.SendEmail:Send To Email Address|Attribute Value

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "66197B01-D1F0-4924-A315-47AD54E030DE", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Subject", "Subject", "The subject that should be used when sending email. <span class='tip tip-liquid'></span>", 2, @"", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386" ); // Rock.Workflow.Action.SendEmail:Subject

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "66197B01-D1F0-4924-A315-47AD54E030DE", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "D1269254-C15A-40BD-B784-ADCC231D3950" ); // Rock.Workflow.Action.SendEmail:Order

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "A95659E5-FF18-4587-8E29-9E4B393E917E", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "E4AD47F4-88EB-4A71-8288-A71C62A07F6D" ); // com.centralaz.SexualOffendersMatch.Workflow.Action.ImportSexualOffenders:Active

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "A95659E5-FF18-4587-8E29-9E4B393E917E", "33E6DF69-BDFA-407A-9744-C175B60643AE", "DPS Excel File", "DPSExcelFile", "", 0, @"", "731D48EB-DF39-4B9E-9337-6DFD733053EF" ); // com.centralaz.SexualOffendersMatch.Workflow.Action.ImportSexualOffenders:DPS Excel File

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "A95659E5-FF18-4587-8E29-9E4B393E917E", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "64924A19-DCF9-451B-85EE-99656CE9020E" ); // com.centralaz.SexualOffendersMatch.Workflow.Action.ImportSexualOffenders:Order

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "D2B6E0A1-B5A7-4BD0-89FF-6BC5BA7E175B", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "C3ECEB73-6BA8-4268-9379-2A26C9378034" ); // com.centralaz.SexualOffendersMatch.Workflow.Action.PopulatePotentialMatchesTable:Active

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "D2B6E0A1-B5A7-4BD0-89FF-6BC5BA7E175B", "33E6DF69-BDFA-407A-9744-C175B60643AE", "DPS Excel File", "DPSExcelFile", "", 0, @"", "4F28E746-629D-4D36-A3F1-71CFFE108250" ); // com.centralaz.SexualOffendersMatch.Workflow.Action.PopulatePotentialMatchesTable:DPS Excel File

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "D2B6E0A1-B5A7-4BD0-89FF-6BC5BA7E175B", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "EC4E6399-6CE0-4899-A176-59F2DE562AAC" ); // com.centralaz.SexualOffendersMatch.Workflow.Action.PopulatePotentialMatchesTable:Order

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EEDA4318-F014-4A46-9C76-4C052EF81AA1", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C" ); // Rock.Workflow.Action.CompleteWorkflow:Active

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EEDA4318-F014-4A46-9C76-4C052EF81AA1", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "25CAD4BE-5A00-409D-9BAB-E32518D89956" ); // Rock.Workflow.Action.CompleteWorkflow:Order

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "F1A39347-6FE0-43D4-89FB-544195088ECF", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "50B01639-4938-40D2-A791-AA0EB4F86847" ); // Rock.Workflow.Action.PersistWorkflow:Active

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "F1A39347-6FE0-43D4-89FB-544195088ECF", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "86F795B0-0CB6-4DA4-9CE4-B11D0922F361" ); // Rock.Workflow.Action.PersistWorkflow:Order

            RockMigrationHelper.UpdateWorkflowType( false, true, "DPS Matching Request", "", "78E38655-D951-41DB-A0FF-D6474775CFA1", "Work", "fa fa-list-ol", 0, false, 0, "1531D23C-2662-4CC2-9178-75CCDD332445" ); // DPS Matching Request

            RockMigrationHelper.UpdateWorkflowTypeAttribute( "1531D23C-2662-4CC2-9178-75CCDD332445", "6F9E2DD0-E39E-4602-ADF9-EB710A75304A", "DPS Excel File", "DPSExcelFile", "", 0, @"", "0E790484-E68F-43EB-B717-74D7D332FBCC" ); // DPS Matching Request:DPS Excel File

            RockMigrationHelper.UpdateWorkflowTypeAttribute( "1531D23C-2662-4CC2-9178-75CCDD332445", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Requester", "Requester", "The person who made the request", 1, @"", "567C84A5-E6EE-489A-9993-73EECBFFAA10" ); // DPS Matching Request:Requester

            RockMigrationHelper.UpdateWorkflowTypeAttribute( "1531D23C-2662-4CC2-9178-75CCDD332445", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Worker", "Worker", "", 2, @"", "C2E9EA22-7DBB-4B5F-8ABE-A7A078AB2949" ); // DPS Matching Request:Worker

            RockMigrationHelper.AddAttributeQualifier( "0E790484-E68F-43EB-B717-74D7D332FBCC", "binaryFileType", @"", "AD68C45A-587D-4F7B-AD01-56D4FF96CE38" ); // DPS Matching Request:DPS Excel File:binaryFileType

            RockMigrationHelper.UpdateWorkflowActivityType( "1531D23C-2662-4CC2-9178-75CCDD332445", true, "Request", "Prompt the user for the excel file.", true, 0, "71EA2494-21FB-49B1-A91A-FD22991D7B84" ); // DPS Matching Request:Request

            RockMigrationHelper.UpdateWorkflowActivityType( "1531D23C-2662-4CC2-9178-75CCDD332445", true, "Process Excel File", "", false, 1, "DD721C86-7055-44AC-A315-0648E14E031A" ); // DPS Matching Request:Process Excel File

            RockMigrationHelper.UpdateWorkflowActionForm( @"<h2>DPS Request</h2>
<p>
Attach the Excel file below.
</p>
<br/>", @"", "Submit^fdc397cd-8b4a-436e-bea1-bce2e6717c03^DD721C86-7055-44AC-A315-0648E14E031A^Your information has been submitted successfully.|", "", true, "", "B740D7CE-5CB6-40FB-94DC-60C5608DDCE1" ); // DPS Matching Request:Request:Prompt User

            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "B740D7CE-5CB6-40FB-94DC-60C5608DDCE1", "0E790484-E68F-43EB-B717-74D7D332FBCC", 0, true, false, true, "91871030-36FA-4838-B7FB-F36BA0A12199" ); // DPS Matching Request:Request:Prompt User:DPS Excel File

            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "B740D7CE-5CB6-40FB-94DC-60C5608DDCE1", "567C84A5-E6EE-489A-9993-73EECBFFAA10", 1, false, true, false, "B9660A37-5882-40BA-8CA7-687D79CA68FB" ); // DPS Matching Request:Request:Prompt User:Requester

            RockMigrationHelper.UpdateWorkflowActionFormAttribute( "B740D7CE-5CB6-40FB-94DC-60C5608DDCE1", "C2E9EA22-7DBB-4B5F-8ABE-A7A078AB2949", 2, false, true, false, "FB7FDE38-BE1C-47CB-9DC2-C0D1E4C8EAF9" ); // DPS Matching Request:Request:Prompt User:Worker

            RockMigrationHelper.UpdateWorkflowActionType( "71EA2494-21FB-49B1-A91A-FD22991D7B84", "Prompt User", 0, "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", true, false, "B740D7CE-5CB6-40FB-94DC-60C5608DDCE1", "", 1, "", "26EF3DAD-865C-47D5-86B0-ABA68868C02A" ); // DPS Matching Request:Request:Prompt User

            RockMigrationHelper.UpdateWorkflowActionType( "71EA2494-21FB-49B1-A91A-FD22991D7B84", "Set Requester", 1, "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", true, false, "", "", 1, "", "79E993F9-2B54-4136-A729-E81EBDBAA374" ); // DPS Matching Request:Request:Set Requester

            RockMigrationHelper.UpdateWorkflowActionType( "71EA2494-21FB-49B1-A91A-FD22991D7B84", "Set Worker", 2, "17962C23-2E94-4E06-8461-0FB8B94E2FEA", true, false, "", "", 1, "", "73038A7D-5626-45E0-913B-FA7C5566F7CF" ); // DPS Matching Request:Request:Set Worker

            RockMigrationHelper.UpdateWorkflowActionType( "71EA2494-21FB-49B1-A91A-FD22991D7B84", "Persist the Workflow", 3, "F1A39347-6FE0-43D4-89FB-544195088ECF", true, false, "", "", 1, "", "AD5FBED6-348D-46BE-9D79-1232B6D26656" ); // DPS Matching Request:Request:Persist the Workflow

            RockMigrationHelper.UpdateWorkflowActionType( "DD721C86-7055-44AC-A315-0648E14E031A", "Import Sexual Offenders", 0, "A95659E5-FF18-4587-8E29-9E4B393E917E", true, false, "", "", 1, "", "628A2316-025F-428C-85C6-3EE35E479A92" ); // DPS Matching Request:Process Excel File:Import Sexual Offenders

            RockMigrationHelper.UpdateWorkflowActionType( "DD721C86-7055-44AC-A315-0648E14E031A", "Populate Potential Matches Table", 1, "D2B6E0A1-B5A7-4BD0-89FF-6BC5BA7E175B", true, false, "", "", 1, "", "F78EBACE-4F8D-466F-A311-B17587F1216D" ); // DPS Matching Request:Process Excel File:Populate Potential Matches Table

            RockMigrationHelper.UpdateWorkflowActionType( "DD721C86-7055-44AC-A315-0648E14E031A", "Notify Worker", 2, "66197B01-D1F0-4924-A315-47AD54E030DE", true, false, "", "", 1, "", "8B9EC2CC-E62F-4E6D-A356-3827BEE344E4" ); // DPS Matching Request:Process Excel File:Notify Worker

            RockMigrationHelper.UpdateWorkflowActionType( "DD721C86-7055-44AC-A315-0648E14E031A", "Complete the Workflow", 3, "EEDA4318-F014-4A46-9C76-4C052EF81AA1", true, false, "", "", 1, "", "30C2F3FA-85D6-44CD-9150-75CA40451E09" ); // DPS Matching Request:Process Excel File:Complete the Workflow

            RockMigrationHelper.AddActionTypeAttributeValue( "26EF3DAD-865C-47D5-86B0-ABA68868C02A", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE", @"False" ); // DPS Matching Request:Request:Prompt User:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "26EF3DAD-865C-47D5-86B0-ABA68868C02A", "C178113D-7C86-4229-8424-C6D0CF4A7E23", @"" ); // DPS Matching Request:Request:Prompt User:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "79E993F9-2B54-4136-A729-E81EBDBAA374", "DE9CB292-4785-4EA3-976D-3826F91E9E98", @"False" ); // DPS Matching Request:Request:Set Requester:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "79E993F9-2B54-4136-A729-E81EBDBAA374", "89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8", @"" ); // DPS Matching Request:Request:Set Requester:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "79E993F9-2B54-4136-A729-E81EBDBAA374", "BBED8A83-8BB2-4D35-BAFB-05F67DCAD112", @"567c84a5-e6ee-489a-9993-73eecbffaa10" ); // DPS Matching Request:Request:Set Requester:Person Attribute

            RockMigrationHelper.AddActionTypeAttributeValue( "73038A7D-5626-45E0-913B-FA7C5566F7CF", "CE28B79D-FBC2-4894-9198-D923D0217549", @"False" ); // DPS Matching Request:Request:Set Worker:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "73038A7D-5626-45E0-913B-FA7C5566F7CF", "7AC47975-71AC-4A2F-BF1F-115CF5578D6F", @"c2e9ea22-7dbb-4b5f-8abe-a7a078ab2949" ); // DPS Matching Request:Request:Set Worker:Attribute

            RockMigrationHelper.AddActionTypeAttributeValue( "73038A7D-5626-45E0-913B-FA7C5566F7CF", "18EF907D-607E-4891-B034-7AA379D77854", @"" ); // DPS Matching Request:Request:Set Worker:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "AD5FBED6-348D-46BE-9D79-1232B6D26656", "50B01639-4938-40D2-A791-AA0EB4F86847", @"False" ); // DPS Matching Request:Request:Persist the Workflow:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "AD5FBED6-348D-46BE-9D79-1232B6D26656", "86F795B0-0CB6-4DA4-9CE4-B11D0922F361", @"" ); // DPS Matching Request:Request:Persist the Workflow:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "628A2316-025F-428C-85C6-3EE35E479A92", "E4AD47F4-88EB-4A71-8288-A71C62A07F6D", @"False" ); // DPS Matching Request:Process Excel File:Import Sexual Offenders:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "628A2316-025F-428C-85C6-3EE35E479A92", "731D48EB-DF39-4B9E-9337-6DFD733053EF", @"0e790484-e68f-43eb-b717-74d7d332fbcc" ); // DPS Matching Request:Process Excel File:Import Sexual Offenders:DPS Excel File

            RockMigrationHelper.AddActionTypeAttributeValue( "628A2316-025F-428C-85C6-3EE35E479A92", "64924A19-DCF9-451B-85EE-99656CE9020E", @"" ); // DPS Matching Request:Process Excel File:Import Sexual Offenders:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "F78EBACE-4F8D-466F-A311-B17587F1216D", "C3ECEB73-6BA8-4268-9379-2A26C9378034", @"False" ); // DPS Matching Request:Process Excel File:Populate Potential Matches Table:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "F78EBACE-4F8D-466F-A311-B17587F1216D", "4F28E746-629D-4D36-A3F1-71CFFE108250", @"0e790484-e68f-43eb-b717-74d7d332fbcc" ); // DPS Matching Request:Process Excel File:Populate Potential Matches Table:DPS Excel File

            RockMigrationHelper.AddActionTypeAttributeValue( "F78EBACE-4F8D-466F-A311-B17587F1216D", "EC4E6399-6CE0-4899-A176-59F2DE562AAC", @"" ); // DPS Matching Request:Process Excel File:Populate Potential Matches Table:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "8B9EC2CC-E62F-4E6D-A356-3827BEE344E4", "36197160-7D3D-490D-AB42-7E29105AFE91", @"False" ); // DPS Matching Request:Process Excel File:Notify Worker:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "8B9EC2CC-E62F-4E6D-A356-3827BEE344E4", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC", @"" ); // DPS Matching Request:Process Excel File:Notify Worker:From Email Address|Attribute Value

            RockMigrationHelper.AddActionTypeAttributeValue( "8B9EC2CC-E62F-4E6D-A356-3827BEE344E4", "D1269254-C15A-40BD-B784-ADCC231D3950", @"" ); // DPS Matching Request:Process Excel File:Notify Worker:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "8B9EC2CC-E62F-4E6D-A356-3827BEE344E4", "0C4C13B8-7076-4872-925A-F950886B5E16", @"c2e9ea22-7dbb-4b5f-8abe-a7a078ab2949" ); // DPS Matching Request:Process Excel File:Notify Worker:Send To Email Address|Attribute Value

            RockMigrationHelper.AddActionTypeAttributeValue( "8B9EC2CC-E62F-4E6D-A356-3827BEE344E4", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386", @"" ); // DPS Matching Request:Process Excel File:Notify Worker:Subject

            RockMigrationHelper.AddActionTypeAttributeValue( "8B9EC2CC-E62F-4E6D-A356-3827BEE344E4", "4D245B9E-6B03-46E7-8482-A51FBA190E4D", @"{{ GlobalAttribute.EmailHeader }}

<p>A DPS Match Request has been submitted by {{ Workflow.Requester }}:</p>
<h4><a href='{{ GlobalAttribute.InternalApplicationRoot }}DPSMatching'>Process DPS Request</a></h4>


{{ GlobalAttribute.EmailFooter }}" ); // DPS Matching Request:Process Excel File:Notify Worker:Body

            RockMigrationHelper.AddActionTypeAttributeValue( "30C2F3FA-85D6-44CD-9150-75CA40451E09", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C", @"False" ); // DPS Matching Request:Process Excel File:Complete the Workflow:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "30C2F3FA-85D6-44CD-9150-75CA40451E09", "25CAD4BE-5A00-409D-9BAB-E32518D89956", @"" ); // DPS Matching Request:Process Excel File:Complete the Workflow:Order

        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
        }
    }
}
