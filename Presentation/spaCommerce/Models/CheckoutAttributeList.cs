using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Catalog;
using spaCommerce.Areas.Admin.Models.Orders;
using spaCommerce.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spaCommerce.Models
{
    public class CheckoutAttributeList : List<ShoppingCartModel.CheckoutAttributeModel>
    {
        public IDictionary<string, string> AttributeFormCollection { get; set; }

        public IFormCollection Form
            => new FormCollection(AttributeFormCollection.ToDictionary(x => x.Key, x => new StringValues(x.Value?.Split(','))));

        public string Name { get; set; }

        public string DefaultValue { get; set; }

        public string TextPrompt { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Selected day value for datepicker
        /// </summary>
        public int? SelectedDay { get; set; }
        /// <summary>
        /// Selected month value for datepicker
        /// </summary>
        public int? SelectedMonth { get; set; }
        /// <summary>
        /// Selected year value for datepicker
        /// </summary>
        public int? SelectedYear { get; set; }

        /// <summary>
        /// Allowed file extensions for customer uploaded files
        /// </summary>
        public IList<string> AllowedFileExtensions { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<CheckoutAttributeValueModel> Values { get; set; }

        public IEnumerable<int> EnabledAttributeMappingIds { get; set; }
        public IEnumerable<int> DisabledAttributeMappingIds { get; set; }

        public CheckoutAttributeList()
        {
            AllowedFileExtensions = new List<string>();
            Values = new List<CheckoutAttributeValueModel>();
            AttributeFormCollection = new Dictionary<string, string>();
            EnabledAttributeMappingIds = new List<int>();
            DisabledAttributeMappingIds = new List<int>();
        }

    }
}
