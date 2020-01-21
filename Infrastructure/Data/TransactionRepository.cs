using System;
using ApplicationCore.Interfaces;
namespace Infrastructure.Data
{
    public class TransactionRepository:ITransactionRepository
    {
        protected readonly BaseContext _dbContext;

        public TransactionRepository(BaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Transaction(Action ops)
        {
            using (var tran=_dbContext.Database.BeginTransaction())
            {
                ops.Invoke();
                tran.Commit();
            }
        }
    }
}