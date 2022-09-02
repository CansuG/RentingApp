using Renting.Models.Advert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Repository;

public interface IAdvertRepository
{
    public Task<Advert> UpsertAsync(AdvertCreate advertCreate, int applicationUserId);

    public Task<Advert> GetAsync(int advertId);

    public Task<List<Advert>> GetAllByUserIdAsync(int applicationUserId);
    
    public Task<List<Advert>> GetByCityAsync(string city);

    public Task<List<Advert>> GetByDistrictAsync(string city, string district);

    public Task<List<Advert>> GetByNeighbourhoodAsync(string city, string district, string neighbourhood);

    public Task<int> DeleteAsync(int advertId);


}
