

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using com.centralaz.SexualOffendersMatch.Data;
using Rock;
using Rock.Data;
using Rock.Model;
namespace com.centralaz.SexualOffendersMatch.Model
{
    /// <summary>
    /// A SexualOffenderPotentialMatch
    /// </summary>
    [Table( "_com_centralaz_SexualOffendersMatch_SexualOffenderPotentialMatch" )]
    [DataContract]
    public class SexualOffenderPotentialMatch : Rock.Data.Model<SexualOffenderPotentialMatch>
    {

        #region Entity Properties

        /// <summary>
        /// The PersonAlias of the potential Person Match
        /// </summary>
        [DataMember]
        public int PersonAliasId { get; set; }

        /// <summary>
        /// The Id of the potential Sexual Offender Match
        /// </summary>
        [DataMember]
        public int SexualOffenderId { get; set; }

        /// <summary>
        /// The likelyhood of the match
        /// </summary>
        [DataMember]
        public int MatchPercentage { get; set; }

        /// <summary>
        /// Whether the potential match is confirmed as not a match
        /// </summary>
        [DataMember]
        public bool IsConfirmedAsNotMatch { get; set; }

        /// <summary>
        /// Whether the potential match is confirmed as a match
        /// </summary>
        [DataMember]
        public bool IsConfirmedAsMatch { get; set; }

        /// <summary>
        /// The date the potential match was confirmed as true or false
        /// </summary>
        [DataMember]
        public DateTime VerifiedDate { get; set; }

        #endregion

        #region Virtual Properties

        /// <summary>
        /// The personalias of the potential match
        /// </summary>
        public virtual PersonAlias PersonAlias { get; set; }

        /// <summary>
        /// The sexual offender of the potential match
        /// </summary>
        public virtual SexualOffender SexualOffender { get; set; }

        #endregion
    }

    #region Entity Configuration


    public partial class SexualOffenderPotentialMatchConfiguration : EntityTypeConfiguration<SexualOffenderPotentialMatch>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SexualOffenderPotentialMatchConfiguration"/> class.
        /// </summary>
        public SexualOffenderPotentialMatchConfiguration()
        {
            this.HasRequired( r => r.PersonAlias ).WithMany().HasForeignKey( r => r.PersonAliasId ).WillCascadeOnDelete( false );
            this.HasRequired( r => r.SexualOffender ).WithMany().HasForeignKey( r => r.SexualOffenderId ).WillCascadeOnDelete( false );
        }
    }

    #endregion

}
