using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.com_centralaz.Accountability
{
    [DisplayName("Accountability Group Member List")]
    [Category("com_centralaz > Accountability")]
    [Description("Lists all members in the accountability group")]

    public partial class AccountabilityGroupMemberList : Rock.Web.UI.RockBlock
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}