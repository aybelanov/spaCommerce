using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using spaCommerce.Models.Catalog;
using Microsoft.AspNetCore.Components;
using Nop.Services.Catalog;
using spaCommerce.Services;
using Nop.Services.Security;
using Nop.Services.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core;
using Nop.Services.Tax;
using Nop.Services.Directory;
using spaCommerce.Infrastructure.Cache;
using Nop.Services.Media;
using Nop.Core.Caching;
using spaCommerce.Models.Media;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;

namespace spaCommerce.Models
{
    // added instead a native one
    public class ProductAttributeList : List<ProductDetailsModel.ProductAttributeModel>
    {
        public IDictionary<string, string> AttributeFormCollection { get; set; }

        public IFormCollection Form
            => new FormCollection(AttributeFormCollection.ToDictionary(x => x.Key, x => new StringValues(x.Value?.Split(','))));

        public string Gtin { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public string Sku { get; set; }
        public decimal BasePrice { get; set; }
        public string Price { get; set; }
        public string BasePricePAngV { get; set; }
        public string StockAvailability { get; set; }
        public IEnumerable<int> EnabledAttributeMappingIds { get; set; }
        public IEnumerable<int> DisabledAttributeMappingIds { get; set; }
        public string FullSizeImageUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool IsFreeShipping { get; set; }
        public List<string> Message { get; set; }

        public ProductAttributeList() : base()
        {
            Message = new List<string>();
            AttributeFormCollection = new Dictionary<string, string>();
            DisabledAttributeMappingIds = new List<int>();
            EnabledAttributeMappingIds = new List<int>();
;        }
    }
}
