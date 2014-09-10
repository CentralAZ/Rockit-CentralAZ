using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using com.centralaz.Accountability.Data;
using Rock.Model;

namespace com.centralaz.Accountability.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class ResponseSetService : AccountabilityService<ResponseSet>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseSetService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ResponseSetService(AccountabilityContext context) : base(context) { }

        public ResponseSet GetMostRecentReport(int pId, int groupId){
            var qry = Queryable()
                .Where(r=> (r.PersonId == pId) && (r.GroupId == groupId))
                .OrderBy(r=> r.SubmitForDate)
                .First();
            return qry;

        }

    }
}
