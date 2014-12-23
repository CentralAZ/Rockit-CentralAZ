using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using com.centralaz.SexualOffendersMatch.Data;
using Rock.Model;

namespace com.centralaz.SexualOffendersMatch.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class SexualOffenderPotentialMatchService : SexualOffendersMatchService<SexualOffenderPotentialMatch>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SexualOffenderPotentialMatchService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SexualOffenderPotentialMatchService( SexualOffendersMatchContext context ) : base( context ) { }

    }
}
