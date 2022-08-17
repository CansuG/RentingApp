using Renting.Models.Advert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Repository
{
    public interface IAdvertRepository
    {
        public Task<Advert> UpsertAsync(AdvertCreate advertCreate, int applicationUserId);

        public Task<Advert> GetAsync(int advertId);

        public Task<List<Advert>> GetAllByUserIdAsync(int applicationUserId);

        public Task<int> DeleteAsync(int advertId);
    }
}
