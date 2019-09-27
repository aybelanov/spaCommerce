using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Components.UI
{
    public class LabelText<TProperty> : ComponentBase
    {
        /// <summary>
        /// Gets or sets a collection of additional attributes that will be applied to the created element.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The end of label content
        /// </summary>
        [Parameter] public string Postfix { get; set; }

        /// <summary>
        /// Name of the property which an attribute need to apply
        /// </summary>
        [Parameter] public Expression<Func<TProperty>> For { get; set; }

        private string _displayName;
        
        protected override Task OnInitializedAsync()
        {
            _displayName = GetDisplayName();
            return base.OnInitializedAsync();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "label");
            builder.AddMultipleAttributes(1, AdditionalAttributes);

            if (ChildContent != null)
                builder.AddContent(2, ChildContent);
            else
                builder.AddContent(2, _displayName);

            builder.CloseElement();
            base.BuildRenderTree(builder);
        }

        // todo need to make test
        // https://stackoverflow.com/questions/671968/retrieving-property-name-from-lambda-expression
        private string GetDisplayName()
        {
            if (For != null)
            {
                MemberExpression body = For.Body as MemberExpression;

                if (body == null)
                {
                    UnaryExpression ubody = (UnaryExpression)For.Body;
                    body = ubody.Operand as MemberExpression;
                }

                var attribute = body.Member.GetCustomAttribute<DisplayNameAttribute>();
                if (attribute != null)
                {
                    return attribute.DisplayName + Postfix;
                }
            }

            return string.Empty;
        }
    }
}
