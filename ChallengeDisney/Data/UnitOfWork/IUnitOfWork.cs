using ChallengeDisney.Entities;
using ChallengeDisney.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeDisney.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ApiRepository ApiRepository { get; }
        
        Task SaveChangesAsync();

    }
}
