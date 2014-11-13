

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using com.centralaz.EventCalendar.Data;
using Rock;
using Rock.Data;
using Rock.Model;
namespace com.centralaz.EventCalendar.Model
{
    /// <summary>
    /// A CalendarEvent
    /// </summary>
    [Table( "_com_centralaz_EventCalendar_CalendarEvent" )]
    [DataContract]
    public class CalendarEvent : Rock.Data.Model<CalendarEvent>
    {

        #region Entity Properties

        /// <summary>
        /// The name
        /// </summary>
        [DataMember]
        public String Name { get; set; }

        /// <summary>
        /// The start date time
        /// </summary>
        [DataMember]
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// The end date time
        /// </summary>
        [DataMember]
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// The campusId
        /// </summary>
        [DataMember]
        public int? CampusId { get; set; }

        /// <summary>
        /// The description
        /// </summary>
        [DataMember]
        public String Description { get; set; }

        /// <summary>
        /// The link to a detail page
        /// </summary>
        [DataMember]
        public String EventLink { get; set; }

        #endregion

        #region Virtual Properties

        /// <summary>
        /// The campus
        /// </summary>
        public virtual Campus Campus { get; set; }       

        #endregion

    }

    #region Entity Configuration


    public partial class CalendarEventConfiguration : EntityTypeConfiguration<CalendarEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarEventConfiguration"/> class.
        /// </summary>
        public CalendarEventConfiguration()
        {
            this.HasRequired( r => r.Campus ).WithMany().HasForeignKey( r => r.CampusId ).WillCascadeOnDelete( false );
        }
    }

    #endregion

}
