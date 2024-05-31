namespace matrix42.mobile.app.ui.Services.Interfaces
{
    public interface IGenericService<TModel, TDto>
    {
        Task<TModel?> GetByExpressionObjectId(Guid id);

        Task<List<TModel>> Get(Guid? id = null, string query = "");

        Task<List<TModel>> GetAll();

        Task<bool> Save(TModel item);

        Task<bool> Save(TDto item, string type);
        
        Task<bool> Delete(TModel item);

        Task<TModel?> Create(TDto item, string type);

        Task<TModel?> Create(TModel item, string dd);

        Task<Dictionary<TModel?, bool>> CreateBulk(List<TDto> items, string type);
    }
}
