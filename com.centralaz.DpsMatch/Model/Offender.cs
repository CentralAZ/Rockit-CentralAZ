

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using com.centralaz.DpsMatch.Data;
using Rock;
using Rock.Data;
using Rock.Model;
namespace com.centralaz.DpsMatch.Model
{
    /// <summary>
    /// A Offender
    /// </summary>
    [Table( "_com_centralaz_DpsMatch_Offender" )]
    [DataContract]
    public class Offender : Rock.Data.Model<Offender>
    {

        #region Entity Properties

        /// <summary>
        /// The KeyString, the unique identifier. LastName+FirstName+Race+Sex+Hair+Eyes+PostalCode
        /// </summary>
        [DataMember]
        public String KeyString { get; set; }

        [DataMember]
        public String LastName { get; set; }

        [DataMember]
        public String FirstName { get; set; }

        [DataMember]
        public Char? MiddleInitial { get; set; }

        [DataMember]
        public int? Age { get; set; }

        [DataMember]
        public int? Height { get; set; }

        [DataMember]
        public int? Weight { get; set; }

        [DataMember]
        public String Race { get; set; }

        [DataMember]
        public String Sex { get; set; }

        [DataMember]
        public String Hair { get; set; }

        [DataMember]
        public String Eyes { get; set; }

        [DataMember]
        public String ResidentialAddress { get; set; }

        [DataMember]
        public String ResidentialCity { get; set; }

        [DataMember]
        public String ResidentialState { get; set; }

        [DataMember]
        public int? ResidentialZip { get; set; }

        [DataMember]
        public DateTime? VerificationDate { get; set; }

        [DataMember]
        public String Offense { get; set; }

        [DataMember]
        public int? OffenseLevel { get; set; }

        [DataMember]
        public bool? Absconder { get; set; }

        [DataMember]
        public String ConvictingJurisdiction { get; set; }

        [DataMember]
        public bool? Unverified { get; set; }

        #endregion

    }

    #region Entity Configuration


    public partial class OffenderConfiguration : EntityTypeConfiguration<Offender>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OffenderConfiguration"/> class.
        /// </summary>
        public OffenderConfiguration()
        {
        }
    }

    #endregion

}
