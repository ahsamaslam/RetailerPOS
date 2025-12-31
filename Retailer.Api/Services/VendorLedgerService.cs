using Microsoft.EntityFrameworkCore;
using Retailer.Api.Entities.Ledger;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using VendorPayment = Retailer.Api.Entities.VendorPayment;

namespace Retailer.Api.Services
{
    public class VendorLedgerService
    {
        private readonly RetailerDbContext _context;

        public VendorLedgerService(RetailerDbContext context)
        {
            _context = context;
        }
		public async Task<List<VendorLedger>> GetVendorLedgerAsync(
	int vendorId,
	DateTime sdate,
	DateTime edate)
		{
			return await _context.VendorLedger
				.Where(x =>
					x.VendorId == vendorId &&
					x.Date.Date >= sdate.Date &&
					x.Date.Date <= edate.Date)
				.OrderBy(x => x.Date)
				.ThenBy(x => x.Id)
				.ToListAsync();
		}
		public async Task PostLedgerAsync(object entity) 
        {
            int entityId;
            DateTime date;
			TimeSpan currentTime = DateTime.Now.TimeOfDay;
			decimal debit = 0, credit = 0;
            int referenceId;
            Guid companyId;
            string Type = ""; 
            string remarks = ""; 
            // Detect object type and extract values
            switch (entity)
            {
                case Vendor Vendor:
                    entityId = Vendor.Id  ;
                    decimal openbal = (decimal)Vendor.openingBalance;  
					date = (DateTime)(Vendor.openDate + currentTime); 
                    credit = openbal > 0? openbal:0;   // purchase increases Vendor balance
                    debit = openbal < 0 ? openbal*-1 : 0;   //
                    referenceId = Vendor.Id;
                    companyId = Vendor.CompanyId;
                    Type = "Opening Balance";
                    remarks = "Opening Balance";
                    break;
                case PurchaseMaster purchase:
                    entityId = purchase.VendorID;
				 
					date = purchase.Date + currentTime; ;
                    credit = purchase.Total;   // purchase increases Vendor balance
                    debit = 0;
                    referenceId = purchase.Id;
                    companyId = purchase.CompanyId;
                    Type = "purchase Invoice";
                    remarks = purchase.remarks??"";
                    break;
                case PurchaseReturnMaster purchase:
                    entityId = purchase.VendorID    ;
                    date = purchase.Date;
                    debit = purchase.Total ;   // purchase increases Vendor balance
                    credit = 0;
                    referenceId = purchase.Id;
                    companyId = purchase.CompanyId;
                    Type = "purchase Return";
                    remarks = purchase.remarks ?? "";
                    break;

                case VendorPayment payment:
                    entityId = payment.VendorId;
                    var totalAmount= payment.Amount + payment.whtAmount+ payment.taxAmount;
                    date = payment.PaymentDate;
                    credit = 0;
                    debit = totalAmount;     // Payment reduces Vendor balance
                    referenceId = payment.Id;
                    companyId = payment.companyId;
                    Type = "Vendor Payment";
                    remarks = payment.remarks ?? "";
                    break;

                default:
                    throw new InvalidOperationException("Unsupported entity type");
            }

            // Get last balance
            var lastBalance = await _context.VendorLedger
                .Where(x => x.VendorId == entityId)
                .OrderByDescending(x => x.Id)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync();

            // Create ledger entry
            var ledgerEntry = new VendorLedger
            {
                VendorId = entityId,
                Date = date,
                Debit = debit,
                Credit = credit,
                Balance = lastBalance + debit - credit,
                ReferenceType = entity.GetType().Name,
                ReferenceId = referenceId,
                CompanyId = companyId
            };

            _context.VendorLedger.Add(ledgerEntry);
            await _context.SaveChangesAsync();

        


            }
        private (string referenceType, int referenceId, int VendorId, DateTime date, decimal debit, decimal credit, Guid companyId) GetLedgerInfo(object entity)
        {
            int VendorId;
            DateTime date;
            decimal debit = 0, credit = 0;
            int referenceId;
            Guid companyId;
            string referenceType;

            switch (entity)
            {
                case Vendor Vendor:
                    VendorId = Vendor.Id;
                    decimal openbal = (decimal)Vendor.openingBalance;
                    date = Vendor.openDate ?? DateTime.Now;
                    credit = openbal > 0 ? openbal : 0;
                    debit = openbal < 0 ? Math.Abs(openbal) : 0;
                    referenceId = Vendor.Id;
                    companyId = Vendor.CompanyId;
                    referenceType = nameof(Vendor);
                    break;

                case PurchaseMaster purchase:
                    VendorId = purchase.VendorID;
                    date = purchase.Date;
                    credit = purchase.Total;
                    debit = 0;
                    referenceId = purchase.Id;
                    companyId = purchase.CompanyId;
                    referenceType = nameof(PurchaseMaster);
                    break;

                case VendorPayment payment:
                    VendorId = payment.VendorId;
                    date = payment.PaymentDate;
                    credit = 0;
                    debit = payment.Amount;
                    referenceId = payment.Id;
                    companyId = payment.companyId;
                    referenceType = nameof(VendorPayment);
                    break;

                default:
                    throw new InvalidOperationException("Unsupported entity type");
            }

            return (referenceType, referenceId, VendorId, date, debit, credit, companyId);
        }
        public async Task UpdateVendorBalanceAsync(object entity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var info = GetLedgerInfo(entity);
                var ledgerEntry = await _context.Set<VendorLedger>()
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
                await UpdateVendorLedgerBalancesAsync(ledgerEntry.VendorId, ledgerEntry.Id, diff);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task UpdateVendorLedgerBalancesAsync(int VendorId, int startingLedgerId, decimal balanceDiff)
        {
            // Get all ledger entries **after the updated ledger**
            var subsequentLedgers = _context.Set<VendorLedger>()
                .Where(x => x.VendorId == VendorId && x.Id > startingLedgerId)
                .OrderBy(x => x.Id);

            await subsequentLedgers.ForEachAsync(x => x.Balance += balanceDiff);

          //  await _context.SaveChangesAsync();
        }

        public async Task ReverseVendorLedgerEntryAsync(string referenceType, int referenceId, string cancelReason, int entityid)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Find the original ledger entry
                var orig = await _context.VendorLedger
                    .FirstOrDefaultAsync(x => x.ReferenceType == referenceType && x.ReferenceId == referenceId  && x.VendorId== entityid);

                if (orig == null)
                    throw new InvalidOperationException("Original ledger entry not found.");
				string cancelRefType = referenceType + "Cancel";

				// 1️⃣ Check if reversal already exists
				bool reversalExists = await _context.VendorLedger.AnyAsync(x =>
					x.ReferenceType == cancelRefType &&
					x.VendorId == entityid &&
					x.ReferenceId == referenceId);

			 
				// 2️⃣ Prepare reversal amounts
				decimal debit = orig.Credit;  // reversed
                decimal credit = orig.Debit;  // reversed

                // 3️⃣ Get last balance before reversal
                var lastBalance = await _context.VendorLedger
                    .Where(x => x.VendorId == orig.VendorId)
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.Balance)
                    .FirstOrDefaultAsync();

                // 4️⃣ Create the reversal entry
                var reversal = new VendorLedger
                {
                    VendorId = orig.VendorId,
                    Date = DateTime.Now,
                    Debit = debit,
                    Credit = credit,
                    ReferenceType = referenceType + "Cancel",  // e.g. "PurchaseMasterCancel"
                    ReferenceId = orig.ReferenceId,
                    CompanyId = orig.CompanyId,
                    Balance = lastBalance + debit - credit,
                    remarks = cancelReason
                };
				if (!reversalExists)
					_context.VendorLedger.Add(reversal);
                await _context.SaveChangesAsync();

                // 5️⃣ Recalculate subsequent balances
                await UpdateVendorLedgerBalancesAsync(orig.VendorId, reversal.Id, (debit - credit));

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
                int entityId = 0;

                switch (entity)
                {
                    case Vendor Vendor:
                        referenceId = Vendor.Id;
						entityId = Vendor.Id;
                        break;
                    case PurchaseMaster purchase:
                        referenceId = purchase.Id;
                        entityId = purchase.VendorID;
                        break;
                    case PurchaseReturnMaster purchase:
                        referenceId = purchase.Id;
						entityId = purchase.VendorID;
						break;
                    case VendorPayment payment:
                        referenceId = payment.Id;
                        entityId = payment.VendorId;
                        break;
                }
                        string ReferenceType = entity.GetType().Name;
                var ledgerEntry = await _context.VendorLedger
                    .FirstOrDefaultAsync(x => x.ReferenceType == ReferenceType && x.ReferenceId == referenceId  && x.VendorId==entityId  );

                if (ledgerEntry == null)
                    throw new InvalidOperationException("Ledger entry not found");
await ReverseVendorLedgerEntryAsync(ReferenceType,referenceId, "Reversal Requested", entityId);   
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
        public async Task<decimal> GetVendorClosingBalanceAsync(DateTime edate,  int VendorId)
        {
            // Get the last ledger entry for the Vendor
            var lastEntry = await _context.VendorLedger
                .Where(x => x.VendorId == VendorId  && x.Date<=edate.Date)
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
                    case Vendor Vendor:
                        referenceId = Vendor.Id;
                        entityId = Vendor.Id;
                        decimal openbal = (decimal)Vendor.openingBalance;
						updatedCredit  = openbal > 0 ? openbal : 0;   // purchase increases Vendor balance
						updatedDebit = openbal < 0 ? openbal * -1 : 0;   //
                        break;
                    case PurchaseMaster purchase:  
                        referenceId = purchase.Id;
						entityId = purchase.VendorID;
						updatedCredit = purchase.Total;   // purchase increases Vendor balance    
                        break;
                    case PurchaseReturnMaster purchase:  
                        referenceId = purchase.Id;
                        entityId = purchase.VendorID;
						updatedDebit = purchase.Total;   // purchase increases Vendor balance    
                        break;
                    case VendorPayment payment: 
                        referenceId = payment.Id;
                        entityId = payment.VendorId;

						var totalAmount = payment.Amount + payment.whtAmount + payment.taxAmount;
                        updatedDebit = totalAmount;
                        updatedCredit = 0;
                        break;
                    // Add cases for other entity types as needed
                   
                }
				var ledgerEntry = await _context.VendorLedger
				  .FirstOrDefaultAsync(x => x.ReferenceType == ReferenceType && x.ReferenceId == referenceId  && x.VendorId== entityId);
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
                _context.VendorLedger.Update(ledgerEntry);


                // Update subsequent balances
                await UpdateVendorLedgerBalancesAsync(ledgerEntry.VendorId, ledgerEntry.Id, diff);

                await _context.SaveChangesAsync();


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
