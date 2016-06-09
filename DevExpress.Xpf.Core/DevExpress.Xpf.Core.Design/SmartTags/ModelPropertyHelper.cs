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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core.Design.SmartTags;
using Platform::DevExpress.Data.Utils;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Xpf.Core.Design {
	public static class ModelPropertyHelper {
		public static readonly object UnsetValue = new object();
		public static void SetPropertyValue(IModelItem item, string propertyName, object value, Type propertyOwnerType) {
			SetPropertyValue(item, propertyName, () => value, propertyOwnerType);
		}
		public static void SetPropertyValue(IModelItem item, string propertyName, Func<object> valueCreateCallback, Type propertyOwnerType) {
			if(item == null) return;
#if !SL
			ICreateXamlMarkupService2010 createXamlMarkupService = item.Context.Services.GetService<ICreateXamlMarkupService>() as ICreateXamlMarkupService2010;
#endif
			IModelProperty property = GetPropertyByName(item, propertyName, propertyOwnerType);
			if(property == null)
				throw new InvalidOperationException("SetPropertyValue(): property == null");
			using(var scope = property.Parent.BeginEdit(string.Format("Change to {0} property.", property.Name))) {
				object value = valueCreateCallback();
				if(property.IsSet)
					property.ClearValue();
				string text = value as string;
				bool isText = text != null;
				if(value != UnsetValue && (!isText && value != null || isText && text.Length != 0 || property.PropertyType == typeof(string))) {
#if !SL
					if(createXamlMarkupService != null)
						value = MarkupHelper.ParseMarkupExtension(value, createXamlMarkupService);
#endif
					property.SetValue(value);
				}
				scope.Complete();
			}
		}
		public static void SetPropertyValue(IModelItem item, string propertyName, object value) {
			SetPropertyValue(item, propertyName, value, null);
		}
		public static object GetPropertyValue(IModelItem item, string propertyName, Type propertyOwnerType) {
			if(item == null) return null;
			IModelProperty property = GetPropertyByName(item, propertyName, propertyOwnerType);
			if(property == null)
				throw new InvalidOperationException("GetPropertyValue(): property == null");
			return GetComputedValue(property);
		}
		public static object GetComputedValue(IModelProperty property) {
			object computedValue = property.ComputedValue;
#if SL
			return computedValue;
#else
			try {
				BitmapImage imageSource = computedValue as BitmapImage;
				if(imageSource == null) return computedValue;
				if(imageSource.UriSource == null || imageSource.UriSource.Scheme != "file") return computedValue;
				string valueSourceText = MarkupHelper.GetCurrentValueSourceText(property);
				return string.IsNullOrEmpty(valueSourceText) ? computedValue : valueSourceText;
			} catch(Exception e) {
				DebugHelper.Assert(e);
				return computedValue;
			}
#endif
		}
		public static Type GetPropertyValueType(IModelItem item, string propertyName, Type propertyOwnerType) {
			if(item == null) return null;
			IModelProperty property = GetPropertyByName(item, propertyName, propertyOwnerType);
			if(property == null) return null;
			IModelItem value = property.Value;
			return value == null ? null : value.ItemType;
		}
		public static object GetPropertyValue(IModelItem item, string propertyName) {
			return GetPropertyValue(item, propertyName, null);
		}
		public static IModelProperty GetPropertyByName(IModelItem item, string propertyName, Type propertyOwnerType) {
			if(item == null) return null;
			string[] properties = propertyName.Split('.');
			if(properties.Length == 1)
				return item.Properties.FindProperty(propertyName, propertyOwnerType);
			else {
				IModelProperty lastProperty = null;
				for(int i = 0; i < properties.Length; i++)
					lastProperty = lastProperty == null ? item.Properties[properties[i]] : lastProperty.Value.Properties[properties[i]];
				return lastProperty;
			}
		}
	}
}
