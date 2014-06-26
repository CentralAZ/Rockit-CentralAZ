﻿// <copyright>
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
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.UI;

using DotLiquid;

using Rock;
using Rock.Attribute;
using Rock.Model;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Rock.Data;

namespace RockWeb.Blocks.Cms
{
    [DisplayName("Ad List")]
    [Category("CMS")]
    [Description("Renders a filtered list of ads for use on public sites.")]
    [IntegerField( "Max Items", "", true, int.MinValue, "", 0 )]
    [LinkedPage( "Detail Page", "", false, "", "", 1 )]
    [CustomCheckboxListField( "Image Types", "The types of images to display",
        "SELECT A.[name] AS [Text], A.[key] AS [Value] FROM [EntityType] E INNER JOIN [attribute] a ON A.[EntityTypeId] = E.[Id] INNER JOIN [FieldType] F ON F.Id = A.[FieldTypeId]	AND F.Guid = '" +
        Rock.SystemGuid.FieldType.IMAGE + "' WHERE E.Name = 'Rock.Model.MarketingCampaignAd' ORDER BY [Key]", false, "", "", 2 )]

    [CodeEditorField( "Template", "The liquid template to use for rendering", CodeEditorMode.Liquid, CodeEditorTheme.Rock, 200, true, @"
    {% include 'AdList' %}
", "", 3 )]

    [CampusesField( "Campuses", "Display Ads for selected campus", false, "", "Filter", 4 )]
    [CustomCheckboxListField( "Ad Types", "Types of Ads to display",
        "SELECT [Name] AS [Text], [Id] AS [Value] FROM [MarketingCampaignAdType] ORDER BY [Name]", true, "", "Filter", 5 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.MARKETING_CAMPAIGN_AUDIENCE_TYPE, "Audience", "The Audience", false, true, "", "Filter", 6 )]
    [CustomCheckboxListField( "Audience Primary Secondary", "Primary or Secondary Audience", "1:Primary,2:Secondary", false, "1,2", "Filter", 7 )]

    [IntegerField( "Image Width", "Width that the image should be resized to. Leave height/width blank to get original size.", false, int.MinValue, "", 8 )]
    [IntegerField( "Image Height", "Height that the image should be resized to. Leave height/width blank to get original size.", false, int.MinValue, "", 9 )]
    [BooleanField("Enable Debug", "Flag indicating that the control should output the ad data that will be passed to Liquid for parsing.", false)]

    [ContextAware( typeof( Campus ) )]
    public partial class AdList : RockBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender( e );
            Render();
        }

        /// <summary>
        /// Renders the Ads using Liquid.
        /// </summary>
        private void Render()
        {
            var rockContext = new RockContext();

            MarketingCampaignAdService marketingCampaignAdService = new MarketingCampaignAdService( rockContext );
            var qry = marketingCampaignAdService.Queryable();

            // limit to date range
            DateTime currentDateTime = RockDateTime.Now.Date;
            qry = qry.Where( a => ( a.StartDate <= currentDateTime ) && ( currentDateTime <= a.EndDate ) );

            // limit to approved
            qry = qry.Where( a => a.MarketingCampaignAdStatus == MarketingCampaignAdStatus.Approved );

            /* Block Attributes */

            // Audience
            string audience = GetAttributeValue( "Audience" );
            if ( !string.IsNullOrWhiteSpace( audience ) )
            {
                var idList = new List<int>();
                foreach ( string guid in audience.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ) )
                {
                    var definedValue = DefinedValueCache.Read( new Guid( guid ) );
                    if ( definedValue != null )
                    {
                        idList.Add( definedValue.Id );
                    }
                }
                qry = qry.Where( a => a.MarketingCampaign.MarketingCampaignAudiences.Any( x => idList.Contains( x.AudienceTypeValueId ) ) );
            }

            // AudiencePrimarySecondary
            string audiencePrimarySecondary = GetAttributeValue( "AudiencePrimarySecondary" );
            if ( !string.IsNullOrWhiteSpace( audiencePrimarySecondary ) )
            {
                // 1 = Primary, 2 = Secondary
                List<int> idlist = audiencePrimarySecondary.SplitDelimitedValues().Select( a => int.Parse( a ) ).ToList();

                if ( idlist.Contains( 1 ) && !idlist.Contains( 2 ) )
                {
                    // only show to Primary Audiences
                    qry = qry.Where( a => a.MarketingCampaign.MarketingCampaignAudiences.Any( x => x.IsPrimary == true ) );
                }
                else if ( idlist.Contains( 2 ) && !idlist.Contains( 1 ) )
                {
                    // only show to Secondary Audiences
                    qry = qry.Where( a => a.MarketingCampaign.MarketingCampaignAudiences.Any( x => x.IsPrimary == false ) );
                }
            }

            // Campuses
            string campuses = GetAttributeValue( "Campuses" );
            if ( !string.IsNullOrWhiteSpace( campuses ) )
            {
                List<int> idlist = campuses.SplitDelimitedValues().Select( a => int.Parse( a ) ).ToList();
                qry = qry.Where( a => a.MarketingCampaign.MarketingCampaignCampuses.Any( x => idlist.Contains( x.CampusId ) ) );
            }

            // Ad Types
            string adtypes = GetAttributeValue( "AdTypes" );
            if ( !string.IsNullOrWhiteSpace( adtypes ) )
            {
                List<int> idlist = adtypes.SplitDelimitedValues().Select( a => int.Parse( a ) ).ToList();
                qry = qry.Where( a => idlist.Contains( a.MarketingCampaignAdTypeId ) );
            }

            // Image Types
            string imageTypes = GetAttributeValue( "ImageTypes" );
            List<string> imageTypeFilter = null;
            if ( !string.IsNullOrWhiteSpace( imageTypes ) )
            {
                imageTypeFilter = imageTypes.SplitDelimitedValues().ToList();
            }

            // Campus Context
            Campus campusContext = this.ContextEntity<Campus>();
            if ( campusContext != null )
            {
                // limit to ads that are targeted to the current campus context
                qry = qry.Where( a => a.MarketingCampaign.MarketingCampaignCampuses.Any( x => x.Id.Equals( campusContext.Id ) ) );
            }


            // Max Items
            string maxItems = GetAttributeValue( "MaxItems" );
            int? maxAdCount = null;
            if ( !string.IsNullOrWhiteSpace( maxItems ) )
            {
                int parsedCount = 0;
                if ( int.TryParse( maxItems, out parsedCount ) )
                {
                    maxAdCount = parsedCount;
                }
            }

            List<MarketingCampaignAd> marketingCampaignAdList;
            qry = qry.OrderBy( a => a.Priority ).ThenBy( a => a.StartDate ).ThenBy( a => a.MarketingCampaign.Title );
            if ( maxAdCount == null )
            {
                marketingCampaignAdList = qry.ToList();
            }
            else
            {
                marketingCampaignAdList = qry.Take( maxAdCount.Value ).ToList();
            }

            // build dictionary for liquid
            var ads = new List<Dictionary<string, object>>();

            foreach ( var marketingCampaignAd in marketingCampaignAdList )
            {
                var ad = new Dictionary<string, object>();

                // DetailPage
                string detailPageUrl = string.Empty;
                string detailPageGuid = GetAttributeValue( "DetailPage" );
                if ( !string.IsNullOrWhiteSpace( detailPageGuid ) )
                {
                    Rock.Model.Page detailPage = new PageService( rockContext ).Get( new Guid( detailPageGuid ) );
                    if ( detailPage != null )
                    {
                        Dictionary<string, string> queryString = new Dictionary<string, string>();
                        queryString.Add( "ad", marketingCampaignAd.Id.ToString() );
                        detailPageUrl = new PageReference( detailPage.Id, 0, queryString ).BuildUrl();
                    }
                }

                string eventGroupName = marketingCampaignAd.MarketingCampaign.EventGroup != null ? marketingCampaignAd.MarketingCampaign.EventGroup.Name : string.Empty;

                // Marketing Campaign Fields
                ad.Add( "Title", marketingCampaignAd.MarketingCampaign.Title );
                ad.Add( "ContactEmail", marketingCampaignAd.MarketingCampaign.ContactEmail );
                ad.Add( "ContactFullName", marketingCampaignAd.MarketingCampaign.ContactFullName );
                ad.Add( "ContactPhoneNumber", marketingCampaignAd.MarketingCampaign.ContactPhoneNumber );
                ad.Add( "LinkedEvent", eventGroupName );

                // Specific Ad Fields
                ad.Add( "AdType", marketingCampaignAd.MarketingCampaignAdType.Name );
                ad.Add( "StartDate", marketingCampaignAd.StartDate.ToString() );
                ad.Add( "EndDate", marketingCampaignAd.EndDate.ToString() );
                ad.Add( "Priority", marketingCampaignAd.Priority );
                ad.Add( "Url", marketingCampaignAd.Url );
                ad.Add( "DetailPageUrl", detailPageUrl );

                // Ad Attributes
                var attributes = new List<Dictionary<string, object>>();
                ad.Add( "Attributes", attributes );

                marketingCampaignAd.LoadAttributes();
                Rock.Attribute.Helper.AddDisplayControls( marketingCampaignAd, phContent );

                // create image resize width/height from block settings
                Dictionary<string, Rock.Field.ConfigurationValue> imageConfig = new Dictionary<string, Rock.Field.ConfigurationValue>();
                if ( !string.IsNullOrWhiteSpace( GetAttributeValue( "ImageWidth" ) )
                    && Int32.Parse( GetAttributeValue( "ImageWidth" ) ) != Int16.MinValue )
                    imageConfig.Add( "width", new Rock.Field.ConfigurationValue( GetAttributeValue( "ImageWidth" ) ) );

                if ( !string.IsNullOrWhiteSpace( GetAttributeValue( "ImageHeight" ) )
                    && Int32.Parse( GetAttributeValue( "ImageHeight" ) ) != Int16.MinValue )
                    imageConfig.Add( "height", new Rock.Field.ConfigurationValue( GetAttributeValue( "ImageHeight" ) ) );

                foreach ( var item in marketingCampaignAd.Attributes )
                {
                    AttributeCache attribute = item.Value;
                    List<AttributeValue> attributeValues = marketingCampaignAd.AttributeValues[attribute.Key];
                    foreach ( AttributeValue attributeValue in attributeValues )
                    {
                        string valueHtml = string.Empty;

                        // if block attributes limit image types, limit images 
                        if ( attribute.FieldType.Guid.Equals( new Guid( Rock.SystemGuid.FieldType.IMAGE ) ) )
                        {
                            if ( imageTypeFilter != null )
                            {
                                if ( !imageTypeFilter.Contains( attribute.Key ) )
                                {
                                    // skip to next attribute if this is an image attribute and it doesn't match the image key filter
                                    continue;
                                }
                                else
                                {
                                    valueHtml = attribute.FieldType.Field.FormatValue( this, attributeValue.Value, imageConfig, false );
                                }
                            }
                        }
                        else
                        {
                            valueHtml = attribute.FieldType.Field.FormatValue( this, attributeValue.Value, attribute.QualifierValues, false );
                        }

                        var valueNode = new Dictionary<string, object>();
                        valueNode.Add( "Key", attribute.Key );
                        valueNode.Add( "Name", attribute.Name );
                        valueNode.Add( "Value", valueHtml );
                        attributes.Add( valueNode );
                    }
                }

                ads.Add( ad );
            }

            var data = new Dictionary<string, object>();
            data.Add( "Ads", ads );
            data.Add( "ApplicationPath", HttpRuntime.AppDomainAppVirtualPath );

            string content;
            try
            {
                content = GetTemplate().Render( Hash.FromDictionary( data ) );
            }
            catch ( Exception ex )
            {
                // liquid compile error
                string exMessage = "An excception occurred while compiling the Liquid template.";

                if ( ex.InnerException != null )
                    exMessage += "<br /><em>" + ex.InnerException.Message + "</em>";

                content = "<div class='alert warning' style='margin: 24px auto 0 auto; max-width: 500px;' ><strong>Liquid Compile Error</strong><p>" + exMessage + "</p></div>";
            }

            // check for errors
            if (content.Contains("No such template"))
            {
                // get template name
                Match match = Regex.Match(GetAttributeValue("Template"), @"'([^']*)");
                if (match.Success)
                {
                    content = String.Format("<div class='alert alert-warning'><h4>Warning</h4>Could not find the template _{1}.liquid in {0}.</div>", ResolveRockUrl("~~/Assets/Liquid"), match.Groups[1].Value);
                }
                else
                {
                    content = "<div class='alert alert-warning'><h4>Warning</h4>Unable to parse the template name from settings.</div>";
                }
            }

            if (content.Contains("error"))
            {
                content = "<div class='alert alert-warning'><h4>Warning</h4>" + content + "</div>";
            }

            phContent.Controls.Clear();
            phContent.Controls.Add( new LiteralControl( content ) );

            // add debug info
            if (GetAttributeValue("EnableDebug").AsBoolean())
            {
                StringBuilder debugInfo = new StringBuilder();
                debugInfo.Append("<p /><div class='alert alert-info'><h4>Debug Info</h4>");

                debugInfo.Append("<pre>");

                debugInfo.Append("<p /><strong>Ad Data</strong> (referenced as 'Ads.' in Liquid)<br>");
                debugInfo.Append(data.LiquidHelpText() + "</pre>");

                debugInfo.Append("</div>");
                phContent.Controls.Add(new LiteralControl(debugInfo.ToString()));
            }
        }

        private string CacheKey()
        {
            return string.Format( "Rock:MarketingCampaignAds:{0}", BlockId );
        }

        private Template GetTemplate()
        {
            string liquidFolder = System.Web.HttpContext.Current.Server.MapPath( ResolveRockUrl( "~~/Assets/Liquid" ) );
            Template.NamingConvention = new DotLiquid.NamingConventions.CSharpNamingConvention();
            Template.FileSystem = new DotLiquid.FileSystems.LocalFileSystem( liquidFolder );

            string cacheKey = CacheKey();

            ObjectCache cache = MemoryCache.Default;
            Template template = cache[cacheKey] as Template;

            if ( template != null )
            {
                return template;
            }
            else
            {
                template = Template.Parse( GetAttributeValue( "Template" ) );

                var cachePolicy = new CacheItemPolicy();
                cache.Set( cacheKey, template, cachePolicy );

                return template;
            }
        }
    }
}