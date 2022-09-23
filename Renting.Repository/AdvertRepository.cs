using Dapper;
using Microsoft.Extensions.Configuration;
using Renting.Models.Advert;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Repository;

public class AdvertRepository : IAdvertRepository
{
    private readonly IConfiguration _config;

    public AdvertRepository(IConfiguration config)
    {
        _config = config;
    }
    public async Task<int> DeleteAsync(int advertId)
    {
        int affectedRows = 0;

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            affectedRows = await connection.ExecuteAsync(
                "Advert_Delete",
                new { AdvertId = advertId },
                commandType: CommandType.StoredProcedure);
        }

        return affectedRows;
    }

    public async Task<List<Advert>> GetAllByUserIdAsync(int applicationUserId)
    {
        IEnumerable<Advert> adverts;

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            adverts = await connection.QueryAsync<Advert>(
                "Advert_GetByUserId",
                new { ApplicationUserId = applicationUserId },
                commandType: CommandType.StoredProcedure);
        }

        return adverts.ToList();
    }

    public async Task<Advert> GetAsync(int advertId)
    {
        Advert advert;

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            advert = await connection.QueryFirstOrDefaultAsync<Advert>(
                "Advert_Get",
                new { AdvertId = advertId },
                commandType: CommandType.StoredProcedure);
        }

        return advert;
    }


    public async Task<List<Advert>> GetAdvertsWithFiltersAsync(Filtering filter)
    {
        IEnumerable<Advert> adverts;

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            adverts = await connection.QueryAsync<Advert>(
                "Advert_GetAdvertsWithFilters",
                new {
                    City = filter.City,
                    District = filter.District,
                    Neighbourhood = filter.Neighbourhood,
                    Rooms = filter.Rooms,
                    MinPrice = filter.MinPrice,
                    MaxPrice = filter.MaxPrice,
                    MinFloorArea = filter.MinFloorArea,
                    MaxFloorArea = filter.MaxFloorArea,
                    Offset = (filter.Page - 1) * filter.PageSize,
                    PageSize = filter.PageSize,
                    OrderByWith = filter.OrderByWith
                },
                commandType: CommandType.StoredProcedure);
        }

        return adverts.ToList();
    }

    public async Task<Advert> UpsertAsync(AdvertCreate advertCreate, int applicationUserId)
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("AdvertId", typeof(int));
        dataTable.Columns.Add("Title", typeof(string));
        dataTable.Columns.Add("Content", typeof(string));
        dataTable.Columns.Add("City", typeof(string));
        dataTable.Columns.Add("District", typeof(string));
        dataTable.Columns.Add("Neighbourhood", typeof(string));
        dataTable.Columns.Add("Rooms", typeof(string));
        dataTable.Columns.Add("Price", typeof(decimal));
        dataTable.Columns.Add("FloorArea", typeof(string));

        dataTable.Rows.Add(
            advertCreate.AdvertId, 
            advertCreate.Title, 
            advertCreate.Content, 
            advertCreate.City,
            advertCreate.District, 
            advertCreate.Neighbourhood, 
            advertCreate.Rooms, 
            advertCreate.Price, 
            advertCreate.FloorArea);

        int? newAdvertId;

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            newAdvertId = await connection.ExecuteScalarAsync<int?>(
                "Advert_Upsert",
                new { Advert = dataTable.AsTableValuedParameter("dbo.AdvertType"), ApplicationUserId = applicationUserId },
                commandType: CommandType.StoredProcedure
                );
        }

        newAdvertId = newAdvertId ?? advertCreate.AdvertId;

        Advert advert = await GetAsync(newAdvertId.Value);

        return advert;
    }
}
