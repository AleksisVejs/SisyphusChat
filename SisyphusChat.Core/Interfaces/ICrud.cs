using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Core.Interfaces
{
    public interface ICrud<TModel>
    where TModel : class
    {
        Task CreateAsync(TModel model);

        Task UpdateAsync(TModel model);

        Task<ICollection<TModel>> GetAllAsync();

        Task<TModel> GetByIdAsync(string id);

        Task DeleteByIdAsync(string id);
    }
}
