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

namespace DevExpress.Design.Filtering.Services {
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using DevExpress.Design.Filtering.UI;
	using DevExpress.Utils.Filtering;
	using DevExpress.Utils.Filtering.Internal;
	public interface IFilteringModelMetricAttributesCollectionFactory {
		IEnumerable<IEndUserFilteringMetricAttributes> Create(IFilteringModelConfiguratorContext context);
	}
	sealed class FilteringModelMetricAttributesCollectionFactory : IFilteringModelMetricAttributesCollectionFactory {
		public IEnumerable<IEndUserFilteringMetricAttributes> Create(IFilteringModelConfiguratorContext context) {
			var collection = GetValue(context.Component, context.Metadata.CustomAttributesProperty) as IEnumerable<CustomMetricsAttributeExpression>;
			return (collection == null) ? Enumerable.Empty<IEndUserFilteringMetricAttributes>() :
				collection.Select(e => context.MetricAttributesFactory.Create(e.Path, e.Type, e.Attributes.Select(a => a.Attribute).ToArray()));
		}
		static object GetValue(object component, string property) {
			if(component == null || string.IsNullOrEmpty(property))
				return null;
			var pd = TypeDescriptor.GetProperties(component)[property];
			return (pd != null) ? pd.GetValue(component) : null;
		}
	}
}
