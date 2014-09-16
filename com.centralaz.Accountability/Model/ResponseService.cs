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
        public Response GetResponse(int responseId)
        {
            return Queryable("Question")
                .Where(r => (r.Id == responseId))
                .FirstOrDefault();

        }
        /// <summary>
        /// Returns a percentage of correct answers
        /// </summary>
        /// <param name="personId"> The person</param>
        /// <param name="groupId">The group</param>
        /// <param name="questionId">The question that we're getting the percentage for</param>
        /// <returns>percentage[0] is the amount correct, and percentage[1] is the amount total</returns>
        public int[] ResponsePercentage(int personId, int groupId, int questionId)
        {
            int[] percentage = new int[2];
            var qry = Queryable("ResponseSet")
                .Where(r => (r.ResponseSet.PersonId == personId) && (r.ResponseSet.GroupId == groupId) && (r.QuestionId == questionId));
            percentage[1] = qry.Count();
            qry = qry.Where(r => r.IsResponseYes == true);
            percentage[0] = qry.Count();
            return percentage;
        }

    }
}
