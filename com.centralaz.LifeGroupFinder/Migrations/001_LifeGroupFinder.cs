using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace com.centralaz.LifeGroupFinder.Migrations
{
    [MigrationNumber( 1, "1.0.14" )]
    public class LifeGroupFinder : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            //Add Children's age groups defined type
            RockMigrationHelper.AddDefinedType( "Global", "Child Age Groups", "", "512F355E-9441-4C47-BE47-7FFE19209496", @"" );
            RockMigrationHelper.AddDefinedValue( "512F355E-9441-4C47-BE47-7FFE19209496", "Children (PreK-5th)", "", "B1A13599-85E1-428B-AA99-7F041D05E62A", false );
            RockMigrationHelper.AddDefinedValue( "512F355E-9441-4C47-BE47-7FFE19209496", "Early Childhood (0-3yrs)", "", "7DF06A41-4AD0-47EB-9352-D9AFA1AB3CFC", false );
            RockMigrationHelper.AddDefinedValue( "512F355E-9441-4C47-BE47-7FFE19209496", "High School", "", "EBB29BCA-531D-439E-AB45-C6D6A54FF50A", false );
            RockMigrationHelper.AddDefinedValue( "512F355E-9441-4C47-BE47-7FFE19209496", "Junior High", "", "C08626C9-6516-424D-B09A-C43010BC65CC", false );

            //Life group attributes
            RockMigrationHelper.AddGroupTypeGroupAttribute( "50FCFB30-F51A-49DF-86F4-2B176EA1820B", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Has Pets", "Whether the group has pets.", 1, "False", "504D75BC-5735-437D-BC30-5981B614B92B" );
            RockMigrationHelper.AddGroupTypeGroupAttribute( "50FCFB30-F51A-49DF-86F4-2B176EA1820B", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Has Children", "Whether the group has children.", 2, "", "D6E4B310-FFDC-4166-8803-04C084277F68" );
            RockMigrationHelper.AddGroupTypeGroupAttribute( "50FCFB30-F51A-49DF-86F4-2B176EA1820B", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Crossroads", "The closest crossroads of the group", 3, "", "F246F843-1F7B-4FFB-9252-635668F0002B" );


        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            RockMigrationHelper.DeleteAttribute( "504D75BC-5735-437D-BC30-5981B614B92B" );    // GroupType - Small Group: Has Pets
            RockMigrationHelper.DeleteAttribute( "D6E4B310-FFDC-4166-8803-04C084277F68" );    // GroupType - Small Group: Has Children
            RockMigrationHelper.DeleteAttribute( "F246F843-1F7B-4FFB-9252-635668F0002B" );    // GroupType - Small Group: Crossroads

            RockMigrationHelper.DeleteDefinedValue( "7DF06A41-4AD0-47EB-9352-D9AFA1AB3CFC" );
            RockMigrationHelper.DeleteDefinedValue( "B1A13599-85E1-428B-AA99-7F041D05E62A" );
            RockMigrationHelper.DeleteDefinedValue( "C08626C9-6516-424D-B09A-C43010BC65CC" );
            RockMigrationHelper.DeleteDefinedValue( "EBB29BCA-531D-439E-AB45-C6D6A54FF50A" );
            RockMigrationHelper.DeleteDefinedType( "512F355E-9441-4C47-BE47-7FFE19209496" );
        }
    }
}
