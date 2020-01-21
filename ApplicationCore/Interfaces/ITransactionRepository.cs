using System;

namespace ApplicationCore.Interfaces
{
    public interface ITransactionRepository
    {
        void Transaction(Action ops);
    }
}