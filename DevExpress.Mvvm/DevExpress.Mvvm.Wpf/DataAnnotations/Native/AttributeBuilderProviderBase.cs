#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
namespace DevExpress.Mvvm.Native {
	public interface IAttributeBuilderInternal<TBuilder> {
		TBuilder AddOrReplaceAttribute<TAttribute>(TAttribute attribute) where TAttribute : Attribute;
		TBuilder AddOrModifyAttribute<TAttribute>(Action<TAttribute> setAttributeValue = null) where TAttribute : Attribute, new();
	}
	public interface ICustomAttributeBuilderProvider {
		Type AttributeType { get; }
		CustomAttributeBuilder CreateAttributeBuilder(Attribute attribute);
	}
	public abstract class CustomAttributeBuilderProviderBase : ICustomAttributeBuilderProvider {
		Type ICustomAttributeBuilderProvider.AttributeType { get { return AttributeType; } }
		protected abstract Type AttributeType { get; }
		CustomAttributeBuilder ICustomAttributeBuilderProvider.CreateAttributeBuilder(Attribute attribute) {
			var pairs = GetPropertyValuePairsCore(attribute);
			return new CustomAttributeBuilder(ExpressionHelper.GetConstructorCore(GetConstructorExpressionCore()), GetConstructorParametersCore(attribute).ToArray(), pairs.Select(x => x.Item1).ToArray(), pairs.Select(x => x.Item2).ToArray());
		}
		internal virtual IEnumerable<object> GetConstructorParametersCore(Attribute attribute) {
			yield break;
		}
		internal virtual IEnumerable<Tuple<PropertyInfo, object>> GetPropertyValuePairsCore(Attribute attribute) {
			yield break;
		}
		internal abstract LambdaExpression GetConstructorExpressionCore();
	}
	public abstract class CustomAttributeBuilderProviderBase<T> : CustomAttributeBuilderProviderBase where T : Attribute {
		protected sealed override Type AttributeType { get { return typeof(T); } }
		internal sealed override IEnumerable<Tuple<PropertyInfo, object>> GetPropertyValuePairsCore(Attribute attribute) {
			return GetPropertyValuePairs((T)attribute);
		}
		internal sealed override LambdaExpression GetConstructorExpressionCore() {
			return GetConstructorExpression();
		}
		internal sealed override IEnumerable<object> GetConstructorParametersCore(Attribute attribute) {
			return GetConstructorParameters((T)attribute);
		}
		protected Tuple<PropertyInfo, object> GetPropertyValuePair<TAttribute, TProperty>(TAttribute attribute, Expression<Func<TAttribute, TProperty>> propertyExpression) {
			return DataAnnotationsAttributeHelper.GetPropertyValuePair(attribute, propertyExpression);
		}
		internal virtual IEnumerable<object> GetConstructorParameters(T attribute) {
			yield break;
		}
		internal virtual IEnumerable<Tuple<PropertyInfo, object>> GetPropertyValuePairs(T attribute) {
			yield break;
		}
		internal abstract Expression<Func<T>> GetConstructorExpression();
	}
	class BrowsableAttributeBuilderProvider : CustomAttributeBuilderProviderBase<BrowsableAttribute> {
		internal override Expression<Func<BrowsableAttribute>> GetConstructorExpression() {
			Expression<Func<BrowsableAttribute>> expression = () => new BrowsableAttribute(true);
			return expression;
		}
		internal override IEnumerable<object> GetConstructorParameters(BrowsableAttribute attribute) {
			return new List<object>() { attribute.Browsable };
		}
	}
	class DisplayAttributeBuilderProvider : CustomAttributeBuilderProviderBase {
		protected override Type AttributeType {
			get { return DataAnnotationsAttributeHelper.GetDisplayAttributeType(); }
		}
		internal override LambdaExpression GetConstructorExpressionCore() {
			return DataAnnotationsAttributeHelper.GetDisplayAttributeCreateExpression();
		}
		internal override IEnumerable<Tuple<PropertyInfo, object>> GetPropertyValuePairsCore(Attribute attribute) {
			return DataAnnotationsAttributeHelper.GetDisplayAttributePropertyValuePairs(attribute);
		}
	}
#if !SILVERLIGHT
	public class DisplayNameAttributeBuilderProvider : CustomAttributeBuilderProviderBase<DisplayNameAttribute> {
		internal override Expression<Func<DisplayNameAttribute>> GetConstructorExpression() {
			return () => new DisplayNameAttribute(default(string));
		}
		internal override IEnumerable<object> GetConstructorParameters(DisplayNameAttribute attribute) {
			yield return attribute.DisplayName;
		}
	}
	public class ScaffoldColumnAttributeBuilderProvider : CustomAttributeBuilderProviderBase {
		protected override Type AttributeType { get { return DataAnnotationsAttributeHelper.GetScaffoldColumnAttributeType(); } }
		internal override LambdaExpression GetConstructorExpressionCore() {
			return DataAnnotationsAttributeHelper.GetScaffoldColumnAttributeCreateExpression();
		}
		internal override IEnumerable<object> GetConstructorParametersCore(Attribute attribute) {
			return DataAnnotationsAttributeHelper.GetScaffoldColumnAttributeConstructorParameters(attribute);
		}
	}
#endif
}
