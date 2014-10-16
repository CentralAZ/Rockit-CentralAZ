using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using com.centralaz.Baptism.Data;
using Rock.Model;

namespace com.centralaz.Baptism.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class BaptizeeService : BaptismService<Baptizee>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaptizeeService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public BaptizeeService( BaptismContext context ) : base( context ) { }


        public List<Baptizee> GetBaptizeesByDateRange( DateTime firstDay, DateTime lastDay )
        {
            List<Baptizee> baptizeeList = Queryable()
                .Where( b => b.BaptismDateTime.Day >= firstDay.Day && b.BaptismDateTime.Day <= lastDay.Day )
                .ToList();
            return baptizeeList;
        }

        public List<Baptizee> GetAllBaptizees()
        {
            List<Baptizee> baptizeeList = Queryable()
                .ToList();
            return baptizeeList;
        }
    }
}
