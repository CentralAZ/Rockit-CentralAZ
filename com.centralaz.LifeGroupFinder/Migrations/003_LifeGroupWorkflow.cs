using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.centralaz.LifeGroupFinder.Migrations
{
    [MigrationNumber( 3, "1.0.14" )]
    public class LifeGroupWorkflow : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.UpdateEntityType( "Rock.Model.Workflow", "3540E9A7-FE30-43A9-8B0A-A372B63DFC93", true, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Model.WorkflowActivity", "2CB52ED0-CB06-4D62-9E2C-73B60AFA4C9F", true, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Model.WorkflowActionType", "23E3273A-B137-48A3-9AFF-C8DC832DDCA6", true, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.CompleteWorkflow", "EEDA4318-F014-4A46-9C76-4C052EF81AA1", false, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.SendSystemEmail", "4487702A-BEAF-4E5A-92AD-71A1AD48DFCE", false, true );

            RockMigrationHelper.UpdateEntityType( "Rock.Workflow.Action.SetAttributeFromEntity", "972F19B9-598B-474B-97A4-50E56E7B59D2", false, true );

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "4487702A-BEAF-4E5A-92AD-71A1AD48DFCE", "08F3003B-F3E2-41EC-BDF1-A2B7AC2908CF", "System Email", "SystemEmail", "A system email to send.", 0, @"", "00676307-F278-42ED-8C05-5B5DD43408B1" ); // Rock.Workflow.Action.SendSystemEmail:System Email

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "4487702A-BEAF-4E5A-92AD-71A1AD48DFCE", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "0DF2AEAA-D6A8-45D8-9F27-663FFD151EA1" ); // Rock.Workflow.Action.SendSystemEmail:Active

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "4487702A-BEAF-4E5A-92AD-71A1AD48DFCE", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "Send To Email Address|Attribute Value", "Recipient", "The email address or an attribute that contains the person or email address that email should be sent to. <span class='tip tip-lava'></span>", 1, @"", "2D0E8665-8B1F-4632-88D8-9A9B6C4E9457" ); // Rock.Workflow.Action.SendSystemEmail:Send To Email Address|Attribute Value

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "4487702A-BEAF-4E5A-92AD-71A1AD48DFCE", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "55C27F0A-6397-4452-8A5A-279590A6F680" ); // Rock.Workflow.Action.SendSystemEmail:Order

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Lava Template", "LavaTemplate", "By default this action will set the attribute value equal to the guid (or id) of the entity that was passed in for processing. If you include a lava template here, the action will instead set the attribute value to the output of this template. The mergefield to use for the entity is 'Entity.' For example, use {{ Entity.Name }} if the entity has a Name property. <span class='tip tip-lava'></span>", 4, @"", "A8176BAA-6401-4079-B507-AB094C16C984" ); // Rock.Workflow.Action.SetAttributeFromEntity:Lava Template

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1" ); // Rock.Workflow.Action.SetAttributeFromEntity:Active

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Entity Is Required", "EntityIsRequired", "Should an error be returned if the entity is missing or not a valid entity type?", 2, @"True", "960E2E93-46AA-4CF9-9450-A096418C5555" ); // Rock.Workflow.Action.SetAttributeFromEntity:Entity Is Required

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Use Id instead of Guid", "UseId", "Most entity attribute field types expect the Guid of the entity (which is used by default). Select this option if the entity's Id should be used instead (should be rare).", 3, @"False", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B" ); // Rock.Workflow.Action.SetAttributeFromEntity:Use Id instead of Guid

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Attribute", "Attribute", "The attribute to set the value of.", 1, @"", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7" ); // Rock.Workflow.Action.SetAttributeFromEntity:Attribute

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "972F19B9-598B-474B-97A4-50E56E7B59D2", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB" ); // Rock.Workflow.Action.SetAttributeFromEntity:Order

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EEDA4318-F014-4A46-9C76-4C052EF81AA1", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C" ); // Rock.Workflow.Action.CompleteWorkflow:Active

            RockMigrationHelper.UpdateWorkflowActionEntityAttribute( "EEDA4318-F014-4A46-9C76-4C052EF81AA1", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "25CAD4BE-5A00-409D-9BAB-E32518D89956" ); // Rock.Workflow.Action.CompleteWorkflow:Order

            RockMigrationHelper.UpdateWorkflowType( false, true, "New group member", "", "78E38655-D951-41DB-A0FF-D6474775CFA1", "Work", "fa fa-list-ol", 0, true, 3, "5997D765-55C8-4A1F-BE59-EEC256180F3C" ); // New group member

            RockMigrationHelper.UpdateWorkflowType( false, true, "New Info Seeker", "", "78E38655-D951-41DB-A0FF-D6474775CFA1", "Work", "fa fa-list-ol", 0, true, 3, "4CB877C4-5BD0-4D0E-B03F-99DAD20EDF27" ); // New Info Seeker

            RockMigrationHelper.UpdateWorkflowTypeAttribute( "5997D765-55C8-4A1F-BE59-EEC256180F3C", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "leader", "leader", "", 0, @"", "B56A7BA6-3CD0-43B3-A571-C3ECF0BF5699" ); // New group member:leader

            RockMigrationHelper.UpdateWorkflowTypeAttribute( "5997D765-55C8-4A1F-BE59-EEC256180F3C", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "member", "member", "", 1, @"", "0D4DDA7A-D39A-4840-8D20-D07D562283F6" ); // New group member:member

            RockMigrationHelper.UpdateWorkflowTypeAttribute( "4CB877C4-5BD0-4D0E-B03F-99DAD20EDF27", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "leader", "leader", "", 0, @"", "2A0B9839-B9E7-4E78-A2C9-ECD193DDB774" ); // New Info Seeker:leader

            RockMigrationHelper.UpdateWorkflowTypeAttribute( "4CB877C4-5BD0-4D0E-B03F-99DAD20EDF27", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "member", "member", "", 1, @"", "FF9C1074-6489-41C2-A328-D3E4F83BD191" ); // New Info Seeker:member

            RockMigrationHelper.UpdateWorkflowActivityType( "5997D765-55C8-4A1F-BE59-EEC256180F3C", true, "notify", "", true, 0, "B5A34857-A809-43C2-B8AD-EF2CEDADEC0A" ); // New group member:notify

            RockMigrationHelper.UpdateWorkflowActivityType( "4CB877C4-5BD0-4D0E-B03F-99DAD20EDF27", true, "notify", "", true, 0, "3506A3A0-5A8D-4D43-ADE0-9134129D27FF" ); // New Info Seeker:notify

            RockMigrationHelper.UpdateWorkflowActionType( "B5A34857-A809-43C2-B8AD-EF2CEDADEC0A", "asdfgasdfasfasd", 3, "EEDA4318-F014-4A46-9C76-4C052EF81AA1", true, false, "", "", 1, "", "3EB3BAB5-20CF-4C9D-A37C-B3883FAB43AA" ); // New group member:notify:asdfgasdfasfasd

            RockMigrationHelper.UpdateWorkflowActionType( "3506A3A0-5A8D-4D43-ADE0-9134129D27FF", "asdfgasdfasfasd", 1, "EEDA4318-F014-4A46-9C76-4C052EF81AA1", true, false, "", "", 1, "", "E3CAAF62-23DF-4660-ABC3-ADE5B35CD284" ); // New Info Seeker:notify:asdfgasdfasfasd

            RockMigrationHelper.UpdateWorkflowActionType( "B5A34857-A809-43C2-B8AD-EF2CEDADEC0A", "memberbuild", 1, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "16E73C00-197E-4163-AEA1-B031D1557E56" ); // New group member:notify:memberbuild

            RockMigrationHelper.UpdateWorkflowActionType( "B5A34857-A809-43C2-B8AD-EF2CEDADEC0A", "membermail", 2, "4487702A-BEAF-4E5A-92AD-71A1AD48DFCE", true, false, "", "", 1, "", "F7EC3D6D-4ECD-4C87-A4B4-0E7AF2B47EE2" ); // New group member:notify:membermail

            RockMigrationHelper.UpdateWorkflowActionType( "B5A34857-A809-43C2-B8AD-EF2CEDADEC0A", "leadermail", 0, "4487702A-BEAF-4E5A-92AD-71A1AD48DFCE", true, false, "", "", 1, "", "CDEC0142-71D0-4813-8603-7F49DECBD62B" ); // New group member:notify:leadermail

            RockMigrationHelper.UpdateWorkflowActionType( "3506A3A0-5A8D-4D43-ADE0-9134129D27FF", "leadermail", 0, "4487702A-BEAF-4E5A-92AD-71A1AD48DFCE", true, false, "", "", 1, "", "D585D333-9217-41E3-8497-3CBE8A805AC2" ); // New Info Seeker:notify:leadermail

            RockMigrationHelper.AddActionTypeAttributeValue( "CDEC0142-71D0-4813-8603-7F49DECBD62B", "00676307-F278-42ED-8C05-5B5DD43408B1", @"caafa7d3-b8f2-4fb5-9c57-ebcb91de5a2e" ); // New group member:notify:leadermail:System Email

            RockMigrationHelper.AddActionTypeAttributeValue( "CDEC0142-71D0-4813-8603-7F49DECBD62B", "0DF2AEAA-D6A8-45D8-9F27-663FFD151EA1", @"False" ); // New group member:notify:leadermail:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "CDEC0142-71D0-4813-8603-7F49DECBD62B", "55C27F0A-6397-4452-8A5A-279590A6F680", @"" ); // New group member:notify:leadermail:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "D585D333-9217-41E3-8497-3CBE8A805AC2", "0DF2AEAA-D6A8-45D8-9F27-663FFD151EA1", @"False" ); // New Info Seeker:notify:leadermail:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "D585D333-9217-41E3-8497-3CBE8A805AC2", "55C27F0A-6397-4452-8A5A-279590A6F680", @"" ); // New Info Seeker:notify:leadermail:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "D585D333-9217-41E3-8497-3CBE8A805AC2", "00676307-F278-42ED-8C05-5B5DD43408B1", @"8091e0d6-1f54-4015-a298-f87d36982daf" ); // New Info Seeker:notify:leadermail:System Email

            RockMigrationHelper.AddActionTypeAttributeValue( "D585D333-9217-41E3-8497-3CBE8A805AC2", "2D0E8665-8B1F-4632-88D8-9A9B6C4E9457", @"2a0b9839-b9e7-4e78-a2c9-ecd193ddb774" ); // New Info Seeker:notify:leadermail:Send To Email Address|Attribute Value

            RockMigrationHelper.AddActionTypeAttributeValue( "CDEC0142-71D0-4813-8603-7F49DECBD62B", "2D0E8665-8B1F-4632-88D8-9A9B6C4E9457", @"b56a7ba6-3cd0-43b3-a571-c3ecf0bf5699" ); // New group member:notify:leadermail:Send To Email Address|Attribute Value

            RockMigrationHelper.AddActionTypeAttributeValue( "16E73C00-197E-4163-AEA1-B031D1557E56", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False" ); // New group member:notify:memberbuild:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "16E73C00-197E-4163-AEA1-B031D1557E56", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @"" ); // New group member:notify:memberbuild:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "E3CAAF62-23DF-4660-ABC3-ADE5B35CD284", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C", @"False" ); // New Info Seeker:notify:asdfgasdfasfasd:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "E3CAAF62-23DF-4660-ABC3-ADE5B35CD284", "25CAD4BE-5A00-409D-9BAB-E32518D89956", @"" ); // New Info Seeker:notify:asdfgasdfasfasd:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "16E73C00-197E-4163-AEA1-B031D1557E56", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"0d4dda7a-d39a-4840-8d20-d07d562283f6" ); // New group member:notify:memberbuild:Attribute

            RockMigrationHelper.AddActionTypeAttributeValue( "16E73C00-197E-4163-AEA1-B031D1557E56", "960E2E93-46AA-4CF9-9450-A096418C5555", @"True" ); // New group member:notify:memberbuild:Entity Is Required

            RockMigrationHelper.AddActionTypeAttributeValue( "16E73C00-197E-4163-AEA1-B031D1557E56", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False" ); // New group member:notify:memberbuild:Use Id instead of Guid

            RockMigrationHelper.AddActionTypeAttributeValue( "16E73C00-197E-4163-AEA1-B031D1557E56", "A8176BAA-6401-4079-B507-AB094C16C984", @"" ); // New group member:notify:memberbuild:Lava Template

            RockMigrationHelper.AddActionTypeAttributeValue( "F7EC3D6D-4ECD-4C87-A4B4-0E7AF2B47EE2", "0DF2AEAA-D6A8-45D8-9F27-663FFD151EA1", @"False" ); // New group member:notify:membermail:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "F7EC3D6D-4ECD-4C87-A4B4-0E7AF2B47EE2", "55C27F0A-6397-4452-8A5A-279590A6F680", @"" ); // New group member:notify:membermail:Order

            RockMigrationHelper.AddActionTypeAttributeValue( "F7EC3D6D-4ECD-4C87-A4B4-0E7AF2B47EE2", "00676307-F278-42ED-8C05-5B5DD43408B1", @"b83ef8b4-67df-4824-b38f-b7cf5527a381" ); // New group member:notify:membermail:System Email

            RockMigrationHelper.AddActionTypeAttributeValue( "F7EC3D6D-4ECD-4C87-A4B4-0E7AF2B47EE2", "2D0E8665-8B1F-4632-88D8-9A9B6C4E9457", @"0d4dda7a-d39a-4840-8d20-d07d562283f6" ); // New group member:notify:membermail:Send To Email Address|Attribute Value

            RockMigrationHelper.AddActionTypeAttributeValue( "3EB3BAB5-20CF-4C9D-A37C-B3883FAB43AA", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C", @"False" ); // New group member:notify:asdfgasdfasfasd:Active

            RockMigrationHelper.AddActionTypeAttributeValue( "3EB3BAB5-20CF-4C9D-A37C-B3883FAB43AA", "25CAD4BE-5A00-409D-9BAB-E32518D89956", @"" ); // New group member:notify:asdfgasdfasfasd:Order


            //--------------------------------------------------------------

            RockMigrationHelper.AddBlockAttributeValue( "18DC3025-3791-43C8-9805-113AF17D5942", "42D95717-2494-4E76-B837-C78AC6E8B139", "5997d765-55c8-4a1f-be59-eec256180f3c" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteBlockAttributeValue( "18DC3025-3791-43C8-9805-113AF17D5942", "42D95717-2494-4E76-B837-C78AC6E8B139" );
        }
    }
}
