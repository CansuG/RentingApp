using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renting.Models.SavedAdvert;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Renting.Repository
{
    public class SavedAdvertRepository
    {
        private readonly IConfiguration _config;

        public SavedAdvertRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<SavedAdvert?> SaveAdvertAsync(SavedAdvertCreate savedAdvertCreate, int applicationUserId, int advertId)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("SavedAdvertId", typeof(int));

            dataTable.Rows.Add(
                savedAdvertCreate.SavedAdvertId);

            int newSavedAdvertId;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                newSavedAdvertId = await connection.ExecuteScalarAsync<int>(
                    "SavedAdvert_SaveAdvert",
                    new
                    {
                        SavedAdvert = dataTable.AsTableValuedParameter("dbo.SavedAdvertType"),
                        ApplicationUserId = applicationUserId,
                        AdvertId = advertId
                    },
                    commandType: CommandType.StoredProcedure);
            }

            SavedAdvert savedAdvert = await GetSavedAdvertAsync(applicationUserId);

            return savedAdvert;
        }

        public async Task<SavedAdvert> GetSavedAdvertAsync(int applicationUserId)
        {
            SavedAdvert savedAdvert;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                savedAdvert = await connection.QueryFirstOrDefaultAsync<SavedAdvert>(
                    "SavedAdvert_GetSavedAdvertByUserId",
                    new { ApplicationUserId = applicationUserId },
                    commandType: CommandType.StoredProcedure);
            }
            return savedAdvert;
        }

        public async Task<int> UnsaveAdvertAsync(int savedAdvertId)
        {
            int affectedRows = 0;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                affectedRows = await connection.ExecuteAsync(
                    "Advert_UnsaveAdvertById",
                    new { SavedAdvertId = savedAdvertId },
                    commandType: CommandType.StoredProcedure);
            }
            return affectedRows;
        }
    }
}
