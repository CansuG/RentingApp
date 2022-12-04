using Renting.Models.AdvertPhoto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Repository
{
    internal interface IAdvertPhotoRepository
    {
        public Task<AdvertPhoto?> AddPhotoAsync(AdvertPhotoCreate advertPhotoCreate, int advertId);

        public Task<AdvertPhoto> GetPhotoByPhotoIdAsync(int photoId);

        public Task<List<AdvertPhoto>> GetPhotoByAdvertIdAsync(int advertId);

        public Task<bool> CheckPhotoAsync(int advertId);

        public Task<int> DeletePhotoAsync(int photoId);
    }
}
