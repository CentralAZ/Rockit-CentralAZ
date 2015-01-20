using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock;
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
    public class PopulatePotentialMatchesTable : Rock.Workflow.ActionComponent
    {
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();
            SexualOffendersMatchContext sexualOffendersMatchContext = new SexualOffendersMatchContext();
            SexualOffenderPotentialMatchService potentialMatchService = new SexualOffenderPotentialMatchService( sexualOffendersMatchContext );
            GroupMemberService memberService = new GroupMemberService( rockContext );
            PersonAliasService personAliasService = new PersonAliasService( rockContext );
            PersonService personService = new PersonService( rockContext );

            List<SexualOffender> sexualOffenderList = new SexualOffenderService( sexualOffendersMatchContext ).Queryable().ToList();
            List<SexualOffenderPotentialMatch> potentialMatchList = potentialMatchService.Queryable().ToList();
            List<PersonAlias> personList = new PersonAliasService( rockContext ).Queryable().ToList();
            List<String> nameList = new List<string>();
            List<PersonAlias> similarList;

            Guid familyGroupGuid = new Guid( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY );
            SexualOffenderPotentialMatch potentialMatch = null;
            bool isNewPotentialMatch = false;

            //For each offender get a list of possible matches
            foreach ( SexualOffender sexualOffender in sexualOffenderList )
            {
                String fullname = String.Format( "{0} {1}", sexualOffender.FirstName, sexualOffender.LastName );
                nameList = personService.GetSimiliarNames( fullname, new List<int>() );
                similarList = new List<PersonAlias>();

                foreach ( string name in nameList )
                {
                    var names = name.SplitDelimitedValues();
                    String firstName = names.Length >= 1 ? names[0].Trim() : string.Empty;
                    String lastName = names.Length >= 2 ? names[1].Trim() : string.Empty;
                    var list = personList.Where( p => p.Person.FullName == name/*p.Person.FirstName == firstName && p.Person.LastName == lastName*/ ).ToList();
                    similarList.AddRange(list );
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
                        var location = memberService
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
                            potentialMatch.VerifiedDate = DateTime.Now;
                            isNewPotentialMatch = true;
                        }
                        //Computing the match score
                        if ( personAlias.Person.LastName == sexualOffender.LastName && int.Parse( location.PostalCode ) == sexualOffender.ResidentialZip )
                        {
                            if ( personAlias.Person.FirstName == sexualOffender.FirstName || personAlias.Person.NickName == sexualOffender.FirstName)
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
                        if ( personAlias.Person.Age.HasValue && sexualOffender.Age.HasValue )
                        {
                            if ( Math.Abs( personAlias.Person.Age.Value - sexualOffender.Age.Value ) < 2 )
                            {
                                potentialMatch.MatchPercentage += 5;
                            }
                        }

                        // Adding the new match is percent likelyhood is at or over 50%
                        if ( potentialMatch.MatchPercentage >= 50 && isNewPotentialMatch )
                        {
                            potentialMatchService.Add( potentialMatch );
                            isNewPotentialMatch = false;
                        }
                    }

                    // Update database with new / existing entry
                    sexualOffendersMatchContext.SaveChanges();
                }
            }

            return true;
        }

    }
}
