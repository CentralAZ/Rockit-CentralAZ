
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Text;


using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.Web.UI;
using Rock.Web;

namespace RockWeb.Plugins.com_centralaz.Finance
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "SecureGiveImport" )]
    [Category( "com_centralaz > Finance" )]
    [Description( "Finance block to import contribution data from a SecureGive XML file." )]

    [DefinedValueField( "4F02B41E-AB7D-4345-8A97-3904DDD89B01", "Transaction Source", "The transaction source", true )]
    [DefinedValueField( "FFF62A4B-5D88-4DEB-AF8F-8E6178E41FE5", "TransactionType", "The means the transaction was submitted by", true )]
    [IntegerField( "Anonymous Giver PersonID", "PersonId to use in case of anonymous giver", true )]
    [BooleanField( "Use Negative Foreign Keys", "Indicates whether Rock uses the negative of the SecureGive reference ID for the contribution record's foreign key", false )]
    [TextField( "Batch Name", "The name that should be used for the batches created", true, "SecureGive Import" )]
    [LinkedPage( "Batch Detail Page", "The page used to display the contributions for a specific batch", true, "", "", 0 )]
    [LinkedPage( "Contribution Detail Page", "The page used to display the contribution details", true, "", "", 1 )]
    public partial class SecureGiveImport : Rock.Web.UI.RockBlock
    {
        #region Fields

        private FinancialBatch _financialBatch;
        private List<string> errors = new List<string>();

        #endregion

        #region Properties

        // used for public / protected properties

        #endregion

        #region Base Control Methods

        //  overrides of the base RockBlock methods (i.e. OnInit, OnLoad)

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
            gContributions.GridRebind += gList_GridRebind;

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );

            ScriptManager scriptManager = ScriptManager.GetCurrent( Page );
            scriptManager.RegisterPostBackControl( lbImport );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

        }

        #endregion

        #region Events

        // handlers called by the controls on your block
        protected void lbImport_Click( object sender, EventArgs e )
        {
            if ( fuImport.HasFile )
            {
                _financialBatch = new FinancialBatch();

                _financialBatch.Name = GetAttributeValue( "BatchName" );
                _financialBatch.BatchStartDateTime = Rock.RockDateTime.Now;
                RockContext rockContext = new RockContext();
                FinancialBatchService financialBatchService = new FinancialBatchService( rockContext );
                DefinedValueService definedValueService = new DefinedValueService( rockContext );

                var transactionSource = definedValueService.GetIdByGuid( GetAttributeValue( "TransactionSource" ).AsGuid() );
                var transactionType = definedValueService.GetIdByGuid( GetAttributeValue( "TransactionType" ).AsGuid() );
                financialBatchService.Add( _financialBatch );
                rockContext.SaveChanges();

                Dictionary<string, string> dictionaryInfo = new Dictionary<string, string>();
                dictionaryInfo.Add( "batchId", _financialBatch.Id.ToString() );
                string url = LinkedPageUrl( "BatchDetailPage", dictionaryInfo );
                //                PageService pageService = new PageService( rockContext );
                //                var pageNumber = pageService.Get( "606BDA31-A8FE-473A-B3F8-A00ECF7E06EC".AsGuid() ).Id;
                //                string url = ResolveUrl( string.Format( "~/page/{0}?batchId={1}", pageNumber, _financialBatch.Id ) );
                String theString = String.Format( "Batch <a href=\"{0}\">{1}</a> was created.", url, _financialBatch.Id.ToString() );
                nbBatch.Text = theString;
                nbBatch.Visible = true;

                var xdoc = XDocument.Load( System.Xml.XmlReader.Create( fuImport.FileContent ) );
                var elemDonations = xdoc.Element( "Donation" );
                foreach ( var elemGift in elemDonations.Elements( "Gift" ) )
                {
                    ProcessGift( elemGift, rockContext );
                }

                BindGrid();

                if ( errors.Count > 0 )
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append( "The following records need to be addressed:" );
                    foreach ( string error in errors )
                    {
                        stringBuilder.AppendLine( string.Format( "{0}", error ) );
                    }
                    nbMessage.Text = stringBuilder.ToString();
                    nbMessage.Visible = true;
                }

            }
        }

        protected void gContributions_View( object sender, RowEventArgs e )
        {
            FinancialTransactionDetailService financialTransactionDetailService = new FinancialTransactionDetailService( new RockContext() );
            var ftd = financialTransactionDetailService.Get( e.RowKeyId );
            NavigateToLinkedPage( "ContributionDetailPage", "transactionId", ftd.TransactionId, "batchId", _financialBatch.Id );
        }

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {

        }

        /// <summary>
        /// Handles the GridRebind event of the gPledges control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void gList_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        #endregion

        #region Methods

        private void ProcessGift( XElement elemGift, RockContext rockContext )
        {
            PersonAliasService personAliasService = new PersonAliasService( rockContext );
            FinancialTransactionService financialTransactionService = new FinancialTransactionService( rockContext );
            FinancialAccountService financialAccountService = new FinancialAccountService( rockContext );
            FinancialTransactionDetailService financialTransactionDetailService = new FinancialTransactionDetailService( rockContext );
            DefinedValueService definedValueService = new DefinedValueService( rockContext );
            DefinedTypeService definedTypeService = new DefinedTypeService( rockContext );
            PersonService personService = new PersonService( rockContext );
            var transactionSource = definedValueService.GetIdByGuid( GetAttributeValue( "TransactionSource" ).AsGuid() );
            var transactionType = definedValueService.GetIdByGuid( GetAttributeValue( "TransactionType" ).AsGuid() );
            var tenderType = definedTypeService.Get( "1D1304DE-E83A-44AF-B11D-0C66DD600B81".AsGuid() );
            try
            {
                FinancialTransaction financialTransaction = new FinancialTransaction()
                {
                    TransactionTypeValueId = transactionType.Value,
                    SourceTypeValueId = transactionSource.Value
                };

                financialTransactionService.Add( financialTransaction );
                financialTransaction.BatchId = _financialBatch.Id;

                if ( elemGift.Element( "ReceivedDate" ) != null )
                {
                    financialTransaction.ProcessedDateTime = elemGift.Element( "ReceivedDate" ).Value.AsDateTime();
                }

                if ( elemGift.Element( "ContributionType" ) != null )
                {
                    string elemValue = elemGift.Element( "ContributionType" ).Value.ToString();
                    var contribution = definedValueService.Queryable()
                        .Where( d => d.DefinedTypeId == tenderType.Id && d.Value == elemValue )
                        .FirstOrDefault();
                    financialTransaction.CurrencyTypeValue = contribution;
                }

                if ( elemGift.Element( "TransactionID" ) != null )
                {
                    financialTransaction.TransactionCode = elemGift.Element( "TransactionID" ).Value.ToString();
                }

                if ( elemGift.Element( "ReferenceNumber" ) != null )
                {
                    if ( !GetAttributeValue( "UseNegativeForeignKeys" ).AsBoolean() )
                    {
                        financialTransaction.ForeignId = elemGift.Element( "ReferenceNumber" ).Value.ToString();
                    }
                    else
                    {
                        financialTransaction.ForeignId = ( elemGift.Element( "ReferenceNumber" ).Value.AsInteger() * -1 ).ToString();
                    }
                }

                var person = personService.Get( GetAttributeValue( "AnonymousGiverPersonID" ).AsInteger() );
                if ( elemGift.Element( "ContributorName" ) != null )
                {
                    if ( elemGift.Element( "ContributorName" ).Value != "" )
                    {
                        person = personService.GetByFullName( elemGift.Element( "ContributorName" ).Value.ToString(), false, true, false ).FirstOrDefault();
                    }
                }
                financialTransaction.AuthorizedPersonAliasId = personAliasService.GetPrimaryAliasId( person.Id );

                FinancialAccount account = new FinancialAccount();

                if ( elemGift.Element( "FundName" ) != null )
                {
                    String accountName = elemGift.Element( "FundName" ).Value.ToString();
                    account = financialAccountService.Queryable()
                    .Where( fa => fa.Name == accountName )
                    .FirstOrDefault();
                }
                if ( account == null )
                {
                    account = financialAccountService.Queryable()
                    .Where( fa => fa.Name == "General Fund" )
                    .FirstOrDefault();
                }

                rockContext.SaveChanges();

                FinancialTransactionDetail financialTransactionDetail = new FinancialTransactionDetail()
                {
                    TransactionId = financialTransaction.Id,
                    AccountId = account.Id
                };

                financialTransactionDetailService.Add( financialTransactionDetail );
                if ( elemGift.Element( "Amount" ) != null )
                {
                    financialTransactionDetail.Amount = elemGift.Element( "Amount" ).Value.AsDecimal();
                }
                rockContext.SaveChanges();
            }
            catch ( Exception e )
            {
                errors.Add( e.Message );
                return;
            }
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            RockContext rockContext = new RockContext();
            FinancialTransactionDetailService financialTransactionDetailService = new FinancialTransactionDetailService( rockContext );

            // sample query to display a few people
            var qry = financialTransactionDetailService.Queryable()
                        .Where( ftd => ftd.Transaction.BatchId == _financialBatch.Id )
                       .ToList();

            gContributions.DataSource = qry.ToList();
            gContributions.DataBind();
        }

        #endregion
    }
}