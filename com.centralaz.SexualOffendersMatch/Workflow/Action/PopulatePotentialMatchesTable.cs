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
    public class PopulatePotentialMatchesTable : Rock.Workflow.ActionComponent
    {
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();
            SexualOffendersMatchContext sexualOffendersMatchContext = new SexualOffendersMatchContext();
            List<SexualOffender> sexualOffenderList = new SexualOffenderService( sexualOffendersMatchContext ).Queryable().ToList();
            SexualOffenderPotentialMatchService potentialMatchService = new SexualOffenderPotentialMatchService( sexualOffendersMatchContext );
            List<SexualOffenderPotentialMatch> potentialMatchList = potentialMatchService.Queryable().ToList();
            List<PersonAlias> personList = new PersonAliasService( new RockContext() ).Queryable().ToList();
            Guid familyGroupGuid = new Guid( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY );
            var memberService = new GroupMemberService( rockContext );


            List<PersonAlias> similarList = new List<PersonAlias>();
            List<String> nameList = new List<string>();
            SexualOffenderPotentialMatch potentialMatch = null;
            bool isNewPotentialMatch = false;
            //For each offender get a list of possible matches
            foreach ( SexualOffender sexualOffender in sexualOffenderList )
            {
                String fullname = String.Format( "{0}, {1}", sexualOffender.LastName, sexualOffender.FirstName );
                nameList = new PersonService( new RockContext() ).GetSimiliarNames( fullname, null );
                similarList = null;
                foreach ( string name in nameList )
                {
                    similarList.AddRange( new PersonAliasService( new RockContext() ).Queryable().Where( p => p.Person.FullName == name ).ToList() );
                }
                //For each possible match check if a match
                foreach ( PersonAlias personAlias in similarList )
                {
                    potentialMatch = potentialMatchList.Where( pm =>
                        pm.PersonAliasId == personAlias.Id
                        && pm.SexualOffenderId == sexualOffender.Id ).FirstOrDefault();
                    //If not an existing match, evaluate.
                    if ( potentialMatch == null || !potentialMatch.IsConfirmedAsMatch )
                    {
                        var location = new GroupMemberService( rockContext )
                                .Queryable( "GroupLocations.Location" )
                                .Where( m =>
                                    m.PersonId == personAlias.PersonId &&
                                    m.Group.GroupType.Guid == familyGroupGuid )
                                .SelectMany( m => m.Group.GroupLocations )
                                .Select( gl => gl.Location )
                                .FirstOrDefault();

                        if ( potentialMatch == null )
                        {
                            potentialMatch = new SexualOffenderPotentialMatch();
                            potentialMatch.PersonAliasId = personAlias.Id;
                            potentialMatch.SexualOffenderId = sexualOffender.Id;
                            isNewPotentialMatch = true;
                        }
                        if ( personAlias.Person.LastName == sexualOffender.LastName && int.Parse( location.PostalCode ) == sexualOffender.ResidentialZip )
                        {
                            if ( personAlias.Person.FirstName == sexualOffender.FirstName )
                            {
                                if ( location.GetFullStreetAddress() == sexualOffender.ResidentialAddress )
                                {
                                    potentialMatch.MatchPercentage = 95;
                                }
                                else
                                {
                                    potentialMatch.MatchPercentage = 80;
                                }
                            }
                            else
                            {
                                potentialMatch.MatchPercentage = 70;
                            }
                        }
                        else
                        {
                            potentialMatch.MatchPercentage = 50;
                        }
                        if ( personAlias.Person.Age.HasValue )
                        {
                            if ( Math.Abs( personAlias.Person.Age.Value - sexualOffender.Age ) < 2 )
                            {
                                potentialMatch.MatchPercentage += 5;
                            }
                        }
                        if ( potentialMatch.MatchPercentage >= 50 )
                        {
                            if ( isNewPotentialMatch )
                            {
                                potentialMatchService.Add( potentialMatch );
                                isNewPotentialMatch = false;
                            }
                        }
                    }

                }
            }
            sexualOffendersMatchContext.SaveChanges();

            return true;
        }

    }
}
