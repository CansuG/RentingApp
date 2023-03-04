using Renting.Models.SavedAdvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Repository
{
    public interface ISavedAdvertRepository
    {
        public Task<SavedAdvert?> SaveAdvertAsync(SavedAdvertCreate savedAdvertCreate, int applicationUserId, int advertId);

        public Task<SavedAdvert> GetSavedAdvertAsync(int savedAdvertId);

        public Task<SavedAdvert> GetSavedAdvertsAsync(int applicationUserId);

        public Task<int> UnsaveAdvertAsync(int savedAdvertId);
    }
}
