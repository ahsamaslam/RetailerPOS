using Microsoft.EntityFrameworkCore;
using Retailer.Api.Entities.Ledger;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using CustomerPayment = Retailer.Api.Entities.CustomerPayment;

namespace Retailer.Api.Services
{
    public class CustomerLedgerService
    {
        private readonly RetailerDbContext _context;

        public CustomerLedgerService(RetailerDbContext context)
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
                case Customer customer:
                    entityId = customer.Id  ;
                    decimal openbal = (decimal)customer.openingBalance;
                    date = customer.openDate??DateTime.Now;
                    debit = openbal > 0? openbal:0;   // Sale increases customer balance
                    credit = openbal < 0 ? openbal*-1 : 0;   //
                    referenceId = customer.Id;
                    companyId = customer.CompanyId;
                    Type = "Opening Balance";
                    remarks = "Opening Balance";
                    break;
                case SalesMaster sale:
                    entityId = sale.CustomerCode??0;
                    date = sale.Date;
                    debit = sale.totalAmount;   // Sale increases customer balance
                    credit = 0;
                    referenceId = sale.Id;
                    companyId = sale.CompanyId;
                    Type = "Sale Invoice";
                    remarks = sale.remarks??"";
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

                case CustomerPayment payment:
                    entityId = payment.CustomerId;
                    var totalAmount= payment.Amount + payment.whtAmount+ payment.taxAmount;
                    date = payment.PaymentDate;
                    debit = 0;
                    credit = totalAmount;     // Payment reduces customer balance
                    referenceId = payment.Id;
                    companyId = payment.companyId;
                    Type = "Customer Payment";
                    remarks = payment.remarks ?? "";
                    break;

                default:
                    throw new InvalidOperationException("Unsupported entity type");
            }

            // Get last balance
            var lastBalance = await _context.CustomerLedger
                .Where(x => x.CustomerId == entityId)
                .OrderByDescending(x => x.Id)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync();

            // Create ledger entry
            var ledgerEntry = new CustomerLedger
            {
                CustomerId = entityId,
                Date = date,
                Debit = debit,
                Credit = credit,
                Balance = lastBalance + debit - credit,
                ReferenceType = entity.GetType().Name,
                ReferenceId = referenceId,
                CompanyId = companyId
            };

            _context.CustomerLedger.Add(ledgerEntry);
            await _context.SaveChangesAsync();

        


            }
        private (string referenceType, int referenceId, int customerId, DateTime date, decimal debit, decimal credit, Guid companyId) GetLedgerInfo(object entity)
        {
            int customerId;
            DateTime date;
            decimal debit = 0, credit = 0;
            int referenceId;
            Guid companyId;
            string referenceType;

            switch (entity)
            {
                case Customer customer:
                    customerId = customer.Id;
                    decimal openbal = (decimal)customer.openingBalance;
                    date = customer.openDate ?? DateTime.Now;
                    debit = openbal > 0 ? openbal : 0;
                    credit = openbal < 0 ? Math.Abs(openbal) : 0;
                    referenceId = customer.Id;
                    companyId = customer.CompanyId;
                    referenceType = nameof(Customer);
                    break;

                case SalesMaster sale:
                    customerId = sale.CustomerCode ?? 0;
                    date = sale.Date;
                    debit = sale.totalAmount;
                    credit = 0;
                    referenceId = sale.Id;
                    companyId = sale.CompanyId;
                    referenceType = nameof(SalesMaster);
                    break;

                case CustomerPayment payment:
                    customerId = payment.CustomerId;
                    date = payment.PaymentDate;
                    debit = 0;
                    credit = payment.Amount;
                    referenceId = payment.Id;
                    companyId = payment.companyId;
                    referenceType = nameof(CustomerPayment);
                    break;

                default:
                    throw new InvalidOperationException("Unsupported entity type");
            }

            return (referenceType, referenceId, customerId, date, debit, credit, companyId);
        }
        public async Task UpdateCustomerBalanceAsync(object entity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var info = GetLedgerInfo(entity);
                var ledgerEntry = await _context.Set<CustomerLedger>()
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
                await UpdateCustomerLedgerBalancesAsync(ledgerEntry.CustomerId, ledgerEntry.Id, diff);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task UpdateCustomerLedgerBalancesAsync(int customerId, int startingLedgerId, decimal balanceDiff)
        {
            // Get all ledger entries **after the updated ledger**
            var subsequentLedgers = _context.Set<CustomerLedger>()
                .Where(x => x.CustomerId == customerId && x.Id > startingLedgerId)
                .OrderBy(x => x.Id);

            await subsequentLedgers.ForEachAsync(x => x.Balance += balanceDiff);

          //  await _context.SaveChangesAsync();
        }

        public async Task ReverseCustomerLedgerEntryAsync(string referenceType, int referenceId, string cancelReason)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Find the original ledger entry
                var orig = await _context.CustomerLedger
                    .FirstOrDefaultAsync(x => x.ReferenceType == referenceType && x.ReferenceId == referenceId);

                if (orig == null)
                    throw new InvalidOperationException("Original ledger entry not found.");

                // 2️⃣ Prepare reversal amounts
                decimal debit = orig.Credit;  // reversed
                decimal credit = orig.Debit;  // reversed

                // 3️⃣ Get last balance before reversal
                var lastBalance = await _context.CustomerLedger
                    .Where(x => x.CustomerId == orig.CustomerId)
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.Balance)
                    .FirstOrDefaultAsync();

                // 4️⃣ Create the reversal entry
                var reversal = new CustomerLedger
                {
                    CustomerId = orig.CustomerId,
                    Date = DateTime.Now,
                    Debit = debit,
                    Credit = credit,
                    ReferenceType = referenceType + "Cancel",  // e.g. "SalesMasterCancel"
                    ReferenceId = orig.ReferenceId,
                    CompanyId = orig.CompanyId,
                    Balance = lastBalance + debit - credit,
                    remarks = cancelReason
                };

                _context.CustomerLedger.Add(reversal);
                await _context.SaveChangesAsync();

                // 5️⃣ Recalculate subsequent balances
                await UpdateCustomerLedgerBalancesAsync(orig.CustomerId, reversal.Id, (debit - credit));

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

                switch (entity)
                {
                    case Customer customer:
                        referenceId = customer.Id;
                        break;
                    case SalesMaster sale:
                        referenceId = sale.Id;
                        break;
                    case SalesReturnMaster sale:
                        referenceId = sale.Id;
                        break;
                    case CustomerPayment payment:
                        referenceId = payment.Id;
                        break;
                }
                        string ReferenceType = entity.GetType().Name;
                var ledgerEntry = await _context.CustomerLedger
                    .FirstOrDefaultAsync(x => x.ReferenceType == ReferenceType && x.ReferenceId == referenceId);

                if (ledgerEntry == null)
                    throw new InvalidOperationException("Ledger entry not found");
await ReverseCustomerLedgerEntryAsync(ReferenceType,referenceId, "Reversal Requested");   
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
        public async Task<decimal> GetCustomerClosingBalanceAsync(DateTime edate,  int customerId)
        {
            // Get the last ledger entry for the customer
            var lastEntry = await _context.CustomerLedger
                .Where(x => x.CustomerId == customerId  && x.Date<=edate.Date)
                .OrderByDescending(x => x.Id) // latest entry by Id
                .FirstOrDefaultAsync();

            // If no ledger exists, balance is zero
            return lastEntry?.Balance ?? 0;
        }
		public async Task<List<CustomerLedger>> GetCustomerLedgerAsync(
	int customerId,
	DateTime sdate,
	DateTime edate)
		{

            List<CustomerLedger> lst = 
			 await _context.CustomerLedger
				.Where(x =>
					x.CustomerId == customerId &&
					x.Date.Date >= sdate.Date &&
					x.Date.Date <= edate.Date)
				.OrderBy(x => x.Date)
				.ThenBy(x => x.Id)
				.ToListAsync();
            if (lst.Count == 0)
            {

              
                CustomerLedger ledger =   await _context.CustomerLedger
               .Where(x =>
                   x.CustomerId == customerId 
                 )  
               .OrderBy(x=>x.Id)
               .LastAsync();
    
                if(ledger!=null)
                    return new List<CustomerLedger> { ledger }; 


            }
          

                return lst;

        }
		public async Task UpdateLedgerAsync(object entity)
        {   using var transaction = await _context.Database.BeginTransactionAsync();
            try
            { 
                decimal diff = 0;
                int referenceId= 0;
                decimal updatedDebit= 0;
                decimal updatedCredit = 0;
                string ReferenceType = entity.GetType().Name;
                var ledgerEntry = await _context.CustomerLedger
                    .FirstOrDefaultAsync(x => x.ReferenceType == ReferenceType && x.ReferenceId == referenceId);

                if (ledgerEntry == null)
                    throw new InvalidOperationException("Ledger entry not found");
                switch (entity)
                {
                    case Customer customer:
                        referenceId = customer.Id;
                        decimal openbal = (decimal)customer.openingBalance;
                        updatedDebit = openbal > 0 ? openbal : 0;   // Sale increases customer balance
                        updatedCredit = openbal < 0 ? openbal * -1 : 0;   //
                        break;
                    case SalesMaster sale:  
                        referenceId = sale.Id;  
                        updatedDebit = sale.totalAmount;   // Sale increases customer balance    
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
                _context.CustomerLedger.Update(ledgerEntry);


                // Update subsequent balances
                await UpdateCustomerLedgerBalancesAsync(ledgerEntry.CustomerId, ledgerEntry.Id, diff);

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
