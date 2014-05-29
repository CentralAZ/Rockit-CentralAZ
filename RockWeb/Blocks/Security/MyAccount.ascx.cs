// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rock;
using Rock.Attribute;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI;

namespace RockWeb.Blocks.Security
{
    /// <summary>
    /// The main Person Profile block the main information about a peron 
    /// </summary>
    [DisplayName( "My Account" )]
    [Category( "Security" )]
    [Description( "Public block for user to manager their account" )]

    [LinkedPage("Detail Page", "Page to edit account details")]
    public partial class MyAccount : RockBlock
    {
        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            RockPage.AddCSSLink( ResolveRockUrl( "~/Styles/fluidbox.css" ) );
            RockPage.AddScriptLink( ResolveRockUrl( "~/Scripts/imagesloaded.min.js" ) );
            RockPage.AddScriptLink( ResolveRockUrl( "~/Scripts/jquery.fluidbox.min.js" ) );

            if ( CurrentPerson != null )
            {
                lName.Text = CurrentPerson.FullName;

                // Setup Image
                var imgTag = new LiteralControl( Rock.Model.Person.GetPhotoImageTag( CurrentPerson.PhotoId, CurrentPerson.Gender, 188, 188 ) );
                if ( CurrentPerson.PhotoId.HasValue )
                {
                    var imgLink = new HyperLink();
                    imgLink.Attributes.Add( "href", CurrentPerson.PhotoUrl );
                    phImage.Controls.Add( imgLink );
                    imgLink.Controls.Add( imgTag );
                }
                else
                {
                    phImage.Controls.Add( imgTag );
                }

                if ( CurrentPerson.BirthDate.HasValue )
                {
                    string ageText = ( CurrentPerson.BirthYear.HasValue && CurrentPerson.BirthYear != DateTime.MinValue.Year ) ?
                        string.Format( "{0} yrs old ", CurrentPerson.BirthDate.Value.Age() ) : string.Empty;
                    lAge.Text = string.Format( "{0}<small>({1})</small><br/>", ageText, CurrentPerson.BirthDate.Value.ToString( "MM/dd" ) );
                }

                lGender.Text = CurrentPerson.Gender != Gender.Unknown ? CurrentPerson.Gender.ToString() : string.Empty;

                if ( CurrentPerson.PhoneNumbers != null )
                {
                    rptPhones.DataSource = CurrentPerson.PhoneNumbers.ToList();
                    rptPhones.DataBind();
                }

                lEmail.Text = CurrentPerson.Email;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the lbEditPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbEditPerson_Click( object sender, EventArgs e )
        {
            if ( CurrentPerson != null )
            {
                NavigateToLinkedPage( "DetailPage" );
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the phone number.
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        protected string FormatPhoneNumber( object countryCode, object number )
        {
            string cc = countryCode as string ?? string.Empty;
            string n = number as string ?? string.Empty;
            return PhoneNumber.FormattedNumber( cc, n );
        }

        #endregion

    }
}