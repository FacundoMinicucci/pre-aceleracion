using ChallengeDisney.Context;
using ChallengeDisney.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeDisney.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApiRepository _repository;
        private readonly ChallengeDisneyContext _context;
        private bool disposed = false;

        public UnitOfWork(ChallengeDisneyContext context)
        {
            _context = context;
        }

        public ApiRepository ApiRepository
        {
            get
            {
                if (_repository == null)
                {
                    _repository = new ApiRepository(_context);
                }
                return _repository;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task SaveChangesAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
