using Microsoft.EntityFrameworkCore;
using Retailer.Api.Entities.Ledger;
using Retailer.Api.Migrations;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using CustomerPayment = Retailer.Api.Entities.CustomerPayment;

namespace Retailer.Api.Services
{
    public class ItemLedgerService
    {
        private readonly RetailerDbContext _context;

        public ItemLedgerService(RetailerDbContext context)
        {
            _context = context;
        }

        public async Task PostLedgerAsync(object entity) 
        {
            int entityId;
            DateTime date;
            decimal debit = 0, credit = 0;
            int referenceId;
            Guid companyId;
            string Type = ""; 
            string remarks = ""; 
            // Detect object type and extract values
            switch (entity)
            {
                //case Customer customer:
                //    entityId = customer.Id  ;
                //    decimal openbal = (decimal)customer.openingBalance;
                //    date = customer.openDate??DateTime.Now;
                //    debit = openbal > 0? openbal:0;   // Sale increases customer balance
                //    credit = openbal < 0 ? openbal*-1 : 0;   //
                //    referenceId = customer.Id;
                //    companyId = customer.CompanyId;
                //    Type = "Opening Balance";
                //    remarks = "Opening Balance";
                //    break;
                case SalesDetail sale:
                    entityId = sale.ItemCode;
                    date = sale.SalesMaster.Date;
                    debit =0 ;   // Sale increases customer balance
                    credit = sale.SalesMaster.Details.Where(r => r.ItemCode == entityId).Sum(x => x.Qty);
                    referenceId = sale.Id;
                    companyId = sale.CompanyId;
                    Type = "Sale Invoice";
                    remarks = sale.SalesMaster.remarks??"";
                    break;
                case SalesReturnMaster sale:
                    entityId = sale.CustomerCode ?? 0;
                    date = sale.Date;
                    credit = sale.totalAmount;   // Sale increases customer balance
                    debit = 0;
                    referenceId = sale.Id;
                    companyId = sale.CompanyId;
                    Type = "Sale Return";
                    remarks = sale.remarks ?? "";
                    break;

                case PurchaseDetail purchase:
                    entityId = purchase.ItemId;
                    date = purchase.Purchase.Date;
                    debit = purchase.Purchase.Details.Where(r => r.ItemId == entityId).Sum(x => x.Qty);   // Sale increases customer balance
                    credit = 0;
                    referenceId = purchase.Purchase.Id;
                    companyId = purchase.Purchase.CompanyId;
                    Type = "Purchase Invoice";
                    remarks = purchase.Purchase.remarks ?? "";
                    break;
                //case SalesReturnMaster sale:
                //    entityId = sale.CustomerCode ?? 0;
                //    date = sale.Date;
                //    credit = sale.totalAmount;   // Sale increases customer balance
                //    debit = 0;
                //    referenceId = sale.Id;
                //    companyId = sale.CompanyId;
                //    Type = "Sale Return";
                //    remarks = sale.remarks ?? "";
                //    break;

                default:
                    throw new InvalidOperationException("Unsupported entity type");
            }

            // Get last balance
            var lastBalance = await _context.ItemLedger
                .Where(x => x.ItemId == entityId)
                .OrderByDescending(x => x.Id)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync();

            // Create ledger entry
            var ledgerEntry = new ItemLedger
            {
                ItemId = entityId,
                Date = date,
                Debit = debit,
                Credit = credit,
                Balance = lastBalance + debit - credit,
                ReferenceType = entity.GetType().Name,
                ReferenceId = referenceId,
                CompanyId = companyId
            };

            _context.ItemLedger.Add(ledgerEntry);
            await _context.SaveChangesAsync();
			await UpdateItemQtyInHandAsync(entityId);



		}
        private (string referenceType, int referenceId, int itemId, DateTime date, decimal debit, decimal credit, Guid companyId) GetLedgerInfo(object entity)
        {
            int itemId;
            DateTime date;
            decimal debit = 0, credit = 0;
            int referenceId;
            Guid companyId;
            string referenceType;

            switch (entity)
            {
                //case Customer customer:
                //    itemId = customer.Id;
                //    decimal openbal = (decimal)customer.openingBalance;
                //    date = customer.openDate ?? DateTime.Now;
                //    debit = openbal > 0 ? openbal : 0;
                //    credit = openbal < 0 ? Math.Abs(openbal) : 0;
                //    referenceId = customer.Id;
                //    companyId = customer.CompanyId;
                //    referenceType = nameof(Customer);
                //    break;

                case SalesDetail sale:
                    itemId = sale.ItemCode;
                    date = sale.SalesMaster.Date;
                    debit =  0;
                    credit = sale.SalesMaster.Details.Where(r=>r.ItemCode== itemId).Sum(x=>x.Qty);
                    referenceId = sale.SalesMaster.Id;
                    companyId = sale.CompanyId;
                    referenceType = nameof(SalesMaster);
                    break;

                
                default:
                    throw new InvalidOperationException("Unsupported entity type");
            }

            return (referenceType, referenceId, itemId, date, debit, credit, companyId);
        }
        public async Task UpdateCustomerBalanceAsync(object entity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var info = GetLedgerInfo(entity);
                var ledgerEntry = await _context.Set<ItemLedger>()
                    .FirstOrDefaultAsync(x => x.ReferenceType == info.referenceType && x.ReferenceId == info.referenceId);
           
                if (ledgerEntry == null)
                    throw new InvalidOperationException("Ledger entry not found");

                var oldDebit = ledgerEntry.Debit;
                var newDebit = ledgerEntry.Balance;
                var diff = newDebit - oldDebit;

                // Update the ledger entry
                ledgerEntry.Debit = newDebit;
                ledgerEntry.Balance += diff;

                // Update subsequent balances
                await UpdateItemLedgerBalancesAsync(ledgerEntry.ItemId, ledgerEntry.Id, diff);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateItemLedgerBalancesAsync(int itemId, int startingLedgerId, decimal balanceDiff)
        {
            // Get all ledger entries **after the updated ledger**
            var subsequentLedgers = _context.ItemLedger
                .Where(x => x.  ItemId == itemId && x.Id > startingLedgerId)
                .OrderBy(x => x.Id);

            await subsequentLedgers.ForEachAsync(x => x.Balance += balanceDiff);

			//  await _context.SaveChangesAsync();
			await UpdateItemQtyInHandAsync(itemId);
		}

        public async Task ReverseItemLedgerEntryAsync(string referenceType, int referenceId, string cancelReason  , int entityID)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Find the original ledger entry
                var orig = await _context.ItemLedger
                    .FirstOrDefaultAsync(x => x.ReferenceType == referenceType && x.ReferenceId == referenceId  && x.ItemId== entityID);

                if (orig == null)
                    throw new InvalidOperationException("Original ledger entry not found.");
				string cancelRefType = referenceType + "Cancel";

				// 1️⃣ Check if reversal already exists
				bool reversalExists = await _context.ItemLedger.AnyAsync(x =>
					x.ReferenceType == cancelRefType &&
					x.ItemId == entityID &&
					x.ReferenceId == referenceId);
                 
				// 2️⃣ Prepare reversal amounts
				decimal debit = orig.Credit;  // reversed
                decimal credit = orig.Debit;  // reversed

                // 3️⃣ Get last balance before reversal
                var lastBalance = await _context.ItemLedger
                    .Where(x => x.ItemId       == orig.ItemId)
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.Balance)
                    .FirstOrDefaultAsync();

                // 4️⃣ Create the reversal entry
                var reversal = new ItemLedger
                {
                    ItemId = orig.ItemId,
                    Date = DateTime.Now,
                    Debit = debit,
                    Credit = credit,
                    ReferenceType = referenceType + "Cancel",  // e.g. "SalesMasterCancel"
                    ReferenceId = orig.ReferenceId,
                    CompanyId = orig.CompanyId,
                    Balance = lastBalance + debit - credit,
                    remarks = cancelReason
                };
				if (!reversalExists)
					_context.ItemLedger.Add(reversal);
                await _context.SaveChangesAsync();

                // 5️⃣ Recalculate subsequent balances
                await UpdateItemLedgerBalancesAsync(orig.ItemId, reversal.Id, (debit - credit));

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task ReverseLedgerAsync(object entity)
        {
            try {
                int referenceId = 0;
                int entityid = 0;

                switch (entity)
                {
                    case PurchaseDetail purchase:
                        referenceId = purchase.PurchaseId;
						entityid = purchase.ItemId;
                        break;
                    case SalesDetail sale:
                        referenceId = sale.SalesMasterId;
						entityid = sale.ItemCode;
						break;
                    case SalesReturnDetail sale:
                        referenceId = sale.SalesReturnMasterId;
						entityid = sale.ItemCode;
                        break;
                 
                }
                        string ReferenceType = entity.GetType().Name;
                var ledgerEntry = await _context.ItemLedger
                    .FirstOrDefaultAsync(x => x.ReferenceType == ReferenceType && x.ReferenceId == referenceId    && x.ItemId== entityid);

                if (ledgerEntry == null)
                    throw new InvalidOperationException("Ledger entry not found");
await ReverseItemLedgerEntryAsync(ReferenceType,referenceId, "Reversal Requested",entityid);   
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
		public async Task UpdateItemQtyInHandAsync(int itemId)
		{
			// Get the latest balance from the ledger
			var latestBalance = await _context.ItemLedger
				.Where(x => x.ItemId == itemId)
				.OrderByDescending(x => x.Id)
				.Select(x => x.Balance)
				.FirstOrDefaultAsync();

			var item = await _context.Items.FindAsync(itemId);
			if (item != null)
			{
				item.QtyInHand = latestBalance;
				_context.Items.Update(item);
				await _context.SaveChangesAsync();
			}
		}
		public async Task<decimal> GetCustomerClosingBalanceAsync(DateTime edate,  int itemId)
        {
            // Get the last ledger entry for the customer
            var lastEntry = await _context.ItemLedger
                .Where(x => x.ItemId == itemId  && x.Date<=edate.Date)
                .OrderByDescending(x => x.Id) // latest entry by Id
                .FirstOrDefaultAsync();

            // If no ledger exists, balance is zero
            return lastEntry?.Balance ?? 0;
        }
        public async Task UpdateLedgerAsync(object entity)
        {   using var transaction = await _context.Database.BeginTransactionAsync();
            try
            { 
                decimal diff = 0;
                int referenceId= 0;
                int entityId= 0;
                decimal updatedDebit= 0;
                decimal updatedCredit = 0;
                string ReferenceType = entity.GetType().Name;
            
                switch (entity)
                {
                    case SalesDetail sale:
                        referenceId = sale.SalesMaster.Id;
						entityId = sale.ItemCode; 
                        updatedDebit = 0;   // Sale increases customer balance
                        updatedCredit = sale.Qty;   //
                        break;
                    case PurchaseDetail purchase:
						entityId = purchase.ItemId;  
                           referenceId = purchase.Purchase.Id;
                        updatedDebit = purchase.Purchase.Details.Where(r=>r.ItemId== entityId).Sum(x=>x.Qty);   // Sale increases customer balance    
                        break;
                    case SalesReturnMaster sale:  
                        referenceId = sale.Id;  
                        updatedCredit = sale.totalAmount;   // Sale increases customer balance    
                        break;
                    case CustomerPayment payment: 
                        referenceId = payment.Id;
                        var totalAmount = payment.Amount + payment.whtAmount + payment.taxAmount;
                        updatedCredit = totalAmount;
                        updatedDebit = 0;
                        break;
                    // Add cases for other entity types as needed
                   
                }

				var ledgerEntry = await _context.ItemLedger
				.FirstOrDefaultAsync(x => x.ReferenceType == ReferenceType && x.ReferenceId == referenceId  && x.ItemId== entityId);

				if (ledgerEntry == null)
					throw new InvalidOperationException("Ledger entry not found");
				var oldDebit = ledgerEntry.Debit;
                var oldCredit = ledgerEntry.Credit;
                var newDebit = updatedDebit;
                var newCredit = updatedCredit;

                // Calculate the net difference
                  diff = (newDebit - oldDebit) - (newCredit - oldCredit);

                // Update the ledger entry
                ledgerEntry.Debit = newDebit;
                ledgerEntry.Credit = newCredit;
                ledgerEntry.Balance += diff;

                // Update in context
                _context.ItemLedger.Update(ledgerEntry);


                // Update subsequent balances
                await UpdateItemLedgerBalancesAsync(ledgerEntry.ItemId, ledgerEntry.Id, diff);

                await _context.SaveChangesAsync();

				await UpdateItemQtyInHandAsync(entityId);
				await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
            
            throw;
            }
        }

    }

}
