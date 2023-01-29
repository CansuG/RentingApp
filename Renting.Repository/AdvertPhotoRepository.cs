using Microsoft.Extensions.Configuration;
using Renting.Models.AdvertPhoto;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Threading;

namespace Renting.Repository
{
    public class AdvertPhotoRepository : IAdvertPhotoRepository
    {
        private readonly IConfiguration _config;

        public AdvertPhotoRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> DeletePhotoAsync(int photoId)
        {
            int affectedRows = 0;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                affectedRows = await connection.ExecuteAsync(
                    "AdvertPhoto_DeletePhotoById",
                    new { PhotoId = photoId },
                    commandType: CommandType.StoredProcedure);
            }
            return affectedRows;
        }

        public async Task<List<AdvertPhoto>> GetPhotoByAdvertIdAsync(int advertId)
        {
            IEnumerable<AdvertPhoto> photos;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                photos = await connection.QueryAsync<AdvertPhoto>(
                "AdvertPhoto_GetPhotoByAdvertId",
                    new { AdvertId = advertId },
                    commandType: CommandType.StoredProcedure);
            }
            return photos.ToList();
        }

        public async Task<AdvertPhoto> GetPhotoByPhotoIdAsync(int photoId)
        {
            AdvertPhoto advertPhoto;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                advertPhoto = await connection.QueryFirstOrDefaultAsync<AdvertPhoto>(
                "AdvertPhoto_GetPhotoByPhotoId",
                    new { PhotoId = photoId },
                    commandType: CommandType.StoredProcedure);
            }
            return advertPhoto;
        }

        public async Task<bool> CheckPhotoAsync(int advertId)
        {
            IEnumerable<AdvertPhoto> photos;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                photos = await connection.QueryAsync<AdvertPhoto>(
                "AdvertPhoto_GetPhotoByAdvertId",
                    new { AdvertId = advertId },
                    commandType: CommandType.StoredProcedure);
            }

            return photos.Count() < 10;
        }

        public async Task<AdvertPhoto?> AddPhotoAsync(AdvertPhotoCreate advertPhotoCreate, int advertId)
        {
            if (await CheckPhotoAsync(advertId))
            {
                var dataTable = new DataTable();
                dataTable.Columns.Add("PhotoId", typeof(int));
                dataTable.Columns.Add("PublicId", typeof(int));
                dataTable.Columns.Add("ImageURL", typeof(string));

                dataTable.Rows.Add(
                advertPhotoCreate.PhotoId,
                advertPhotoCreate.PublicId,
                advertPhotoCreate.ImageUrl);

                int newPhotoId;

                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    newPhotoId = await connection.ExecuteScalarAsync<int>(
                        "AdvertPhoto_AddPhoto",
                        new
                        {
                            AdvertPhoto = dataTable.AsTableValuedParameter("dbo.AdvertPhotoType"),
                            AdvertId = advertId
                        },
                        commandType: CommandType.StoredProcedure);
                }

                AdvertPhoto advertPhoto = await GetPhotoByPhotoIdAsync(newPhotoId);

                return advertPhoto;
            }
            //If there are more than 10 photos, then the method returns null. (Ask if it is right)
            return null;      
        }
    }
}
