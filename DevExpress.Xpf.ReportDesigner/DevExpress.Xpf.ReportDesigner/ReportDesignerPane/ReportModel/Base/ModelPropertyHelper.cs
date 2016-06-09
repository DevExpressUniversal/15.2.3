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
using System.Text;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public static class ModelPropertyHelper {
		public static IEnumerable<PropertyDescriptor> InjectProperty<TModel, T>(this IEnumerable<PropertyDescriptor> properties, TModel model, Expression<Func<TModel, T>> property) where TModel : XRModelBase {
			return properties.Concat(
				ProxyPropertyDescriptor.Create(TypeDescriptor.GetProperties(model)[ExpressionHelper.GetPropertyName(property)], (DiagramItem x) => XRModelBase.GetXRModel(x)).Yield()
			);
		}
		public static IEnumerable<PropertyDescriptor> InjectProperties<TModel, TPropertiesContainer>(this IEnumerable<PropertyDescriptor> properties, TModel model, Func<TModel, TPropertiesContainer> propertiesContainer) where TModel : XRModelBase {
			return properties.Concat(
				ProxyPropertyDescriptor.GetProxyDescriptors(model.DiagramItem, x => propertiesContainer((TModel)XRModelBase.GetXRModel(x)))
			);
		}
		public static IEnumerable<PropertyDescriptor> InjectPropertyModel<TModel, T>(this IEnumerable<PropertyDescriptor> properties, TModel model, Func<TModel, XRModelBase.XRPropertyModel<T>> propertyModel) where TModel : XRModelBase {
			var propertyModelInstance = propertyModel(model);
			string propertyName = propertyModelInstance.SourcePropertyName;
			PropertyDescriptor propertyDescriptor = propertyModelInstance.GetValuePropertyDescriptor();
			return properties.Select(property => !string.Equals(property.Name, propertyName, StringComparison.Ordinal) ? property : ProxyPropertyDescriptor.Create(propertyDescriptor, (DiagramItem x) => propertyModel((TModel)XRModelBase.GetXRModel(x))));
		}
	}
}
