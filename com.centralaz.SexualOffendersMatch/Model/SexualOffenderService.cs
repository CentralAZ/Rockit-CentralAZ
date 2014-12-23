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
    public class SexualOffenderService : SexualOffendersMatchService<SexualOffender>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SexualOffenderService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SexualOffenderService( SexualOffendersMatchContext context ) : base( context ) { }

    }
}
