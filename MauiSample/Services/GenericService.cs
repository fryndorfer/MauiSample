using Microsoft.Extensions.Logging;
using MauiSample.Services.Interfaces;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MauiSample.Services
{
    public class GenericService<TModel, TDto> : RestServiceBase, IGenericService<TModel, TDto>
        where TModel : IGenericBase, new()
        where TDto : new()
    {
        IRestServiceBase _restService;
        ILogger<RestServiceBase> _logger;
        string _columns;
        string _classBaseName;
        string _classTypeName;
        TModel defaultModel = new TModel();

        public GenericService(IRestServiceBase restServiceBase, IHttpClientFactory httpClientFactory, ILogger<RestServiceBase> logger) : base(httpClientFactory, logger)
        {
            _logger = logger;
            _restService = restServiceBase;
        }

        public async Task<TModel?> GetByExpressionObjectId(Guid id)
        {
            var items = await Get(id);
            return items.FirstOrDefault();
        }

        public async Task<List<TModel>> Get(Guid? id = null, string query = "")
        {
            var whereclause = string.Empty;
            if (id.HasValue)
                whereclause = $"[expression-objectid]='{id}'";

            if (!string.IsNullOrEmpty(query))
                whereclause = query;
            
            var result = await _restService.GetAsync<List<TModel>?>($"{_restService.M42ApiPrefix}/data/fragments/{defaultModel.ClassBaseName}?columns={defaultModel.Columns}&where={whereclause}");

            return result ?? new List<TModel>();
        }

        public async Task<List<TModel>> GetAll()
        {
            var result = await _restService.GetAsync<List<TModel>?>($"{_restService.M42ApiPrefix}/data/fragments/{defaultModel.ClassBaseName}?columns={defaultModel.Columns}");

            return result ?? new List<TModel>();
        }

        public async Task<bool> Save(TModel item)
        {
            var result = await _restService.PutAsync<TModel>($"{_restService.M42ApiPrefix}/data/fragments/{defaultModel.ClassBaseName}", item);
            return true;
        }

        public async Task<bool> Save(TDto item, string type)
        {
            var result = await _restService.PutAsync<TDto>($"{_restService.M42ApiPrefix}/data/objects/{type}", item);
            return true;
        }

        public async Task<TModel?> Create(TDto item, string type)
        {
            var eoid = await _restService.PostAsync<object, string>($"{_restService.M42ApiPrefix}/data/objects/{type}", item);
            return await GetByExpressionObjectId(new Guid(eoid));
        }

        public async Task<TModel?> Create(TModel item, string dd)
        {
            var eoid = await _restService.PostAsync<object, string>($"{_restService.M42ApiPrefix}/data/fragments/{dd}", item);
            return await GetByExpressionObjectId(new Guid(eoid));
        }

        public async Task<bool> Delete(TModel item)
        {
            var result = await _restService.DeleteAsync($"{_restService.M42ApiPrefix}/data/objects/{defaultModel.ClassTypeName}/{item.ExpressionObjectId.Value}");
            return result;
        }

        public async Task<Dictionary<TModel?, bool>> CreateBulk(List<TDto> items, string type)
        {
            var results = new Dictionary<TModel?, bool>();

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 2 };
            await Parallel.ForEachAsync(items, parallelOptions, async (item, canceltoken) =>
            {
                var result = await Create(item, type);
                results.Add(result, true);
            });

            return results;
        }

        /* fragments api
            var parameters = new { EntityClassName = "SPSAssetClassBase", WhereExpression = whereclause, Columns = Asset.Columns };
            var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");                        
            var result = await _restService.PostAsync<object, List<Asset>?>($"{_restService.M42ApiPrefix}/dataQuery/relationItems", parameters);
        */  
    }
}
