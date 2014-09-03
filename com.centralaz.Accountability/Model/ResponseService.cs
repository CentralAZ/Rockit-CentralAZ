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
    public class ResponseService : AccountabilityService<Response>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ResponseService(AccountabilityContext context) : base(context) { }

        /// <summary>
        /// Returns the question the response is to
        /// </summary>
        public Response GetResponse(int ResponseId)
        {
            return Queryable("Question")
                .Where(r => (r.Id == ResponseId))
                .FirstOrDefault();
            
        }
    }
}
