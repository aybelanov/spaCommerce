using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Web.Framework.Components.Services
{
    public interface ICompareProductsComponentService
    {
        Task AddProductToCompareList(int productId);
        Task ClearCompareProducts();
        Task<IList<Product>> GetComparedProducts();
        Task RemoveProductFromCompareList(int productId);
    }
}