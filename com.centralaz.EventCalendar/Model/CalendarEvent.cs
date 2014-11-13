

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

        [DataMember]
        public String Name { get; set; }

        [DataMember]
        public DateTime StartDateTime { get; set; }

        [DataMember]
        public DateTime EndDateTime { get; set; }

        [DataMember]
        [DefinedValue( SystemGuid.DefinedType.CAMPUS )]
        public int? CampusId { get; set; }

        [DataMember]
        public String Description { get; set; }

        [DataMember]
        public String EventLink { get; set; }

        #endregion

        #region Virtual Properties

       

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
        }
    }

    #endregion

}
