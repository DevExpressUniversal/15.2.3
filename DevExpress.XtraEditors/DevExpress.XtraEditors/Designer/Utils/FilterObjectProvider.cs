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

using System.ComponentModel;
using DevExpress.Utils;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraEditors.Designer.Utils {
	public interface IPropertyGridSearchClient : ISearchControlClient {
		object[] SelectedObjects { get; set; }
		object SelectedObject { get; set; }
	}
	class FilterObjectProvider : SearchControlProviderBase {
		IPropertyGridSearchClient client;
		public FilterObjectProvider(IPropertyGridSearchClient client) {
			this.client = client;
		}
		protected override void DisposeCore() { client = null; }
		protected override SearchInfoBase GetCriteriaInfoCore(SearchControlQueryParamsEventArgs args) {
			string searchText = args.SearchText ?? string.Empty;
			return new FilterObjectsInfo(searchText);
		}
	}
	class FilterObjectsInfo : SearchInfoBase {
		string searchText;
		public FilterObjectsInfo() {
			this.searchText = null;
		}
		public FilterObjectsInfo(string searchText) {
			this.searchText = searchText;
		}
		public override string SearchText { get { return searchText; } }
	}
	static class PropertiesDescriptorSearchHelper {
		static int maxLevel = 4;
		public static PropertyDescriptorCollection GetProperties(GridItem gridItem, object component, string filter, PropertyDescriptorCollection collection) {
			if(gridItem == null || string.IsNullOrEmpty(filter)) return collection;
			string f = filter.ToLower();
			if(IsCheckParentProperty(gridItem, f)) return collection;
			List<PropertyDescriptor> filtered = new List<PropertyDescriptor>();
			int level = GetLevel(gridItem);
			foreach(PropertyDescriptor property in collection) {
				if(!IsTextMatch(property.Name, f)) {
					if(!IsCheckProperty(property, component, gridItem, f, level)) continue;
				}
				filtered.Add(property);
			}
			return new PropertyDescriptorCollection(filtered.ToArray());
		}
		static bool IsProperty(GridItem gridItem) {
			return gridItem.GridItemType != GridItemType.Category && gridItem.GridItemType != GridItemType.Root;
		}
		static bool IsCheckParentProperty(GridItem gridItem, string filter) {
			if(gridItem == null || !IsProperty(gridItem))
				return false;
			if((gridItem.PropertyDescriptor != null) && IsTextMatch(gridItem.PropertyDescriptor.Name, filter))
				return true;
			return IsCheckParentProperty(gridItem.Parent, filter);
		}
		static int GetLevel(GridItem gridItem) {
			int res = 0;
			int loopBreak = 0;
			while(gridItem != null && ++loopBreak < 10) {
				if(IsProperty(gridItem))
					res++;
				gridItem = gridItem.Parent;
			}
			return res;
		}
		static bool IsCheckProperty(PropertyDescriptor property, object instance, GridItem gridItem, string filter, int level) {
			try {
				return IsCheckPropertyCore(property, instance, gridItem, filter, level);
			}
			catch { }
			return false;
		}
		static bool IsNestedPropertyDescriptor(PropertyDescriptor pd) {
			if(pd.SerializationVisibility == DesignerSerializationVisibility.Content) return true;
			XtraSerializableProperty attr = pd.Attributes[typeof(XtraSerializableProperty)] as XtraSerializableProperty;
			return attr != null && attr.Visibility == XtraSerializationVisibility.Content;
		}
		static bool IsCheckPropertyCore(PropertyDescriptor property, object instance, GridItem gridItem, string filter, int level) {
			if(!property.IsBrowsable) return false;
			if(IsTextMatch(property.Name, filter)) return true;
			if(gridItem == null || IsProperty(gridItem))
				if(maxLevel > 4) return false;
			var converter = property.Converter;
			bool isPropertiesSupported = converter != null && converter.GetPropertiesSupported();
			if(!isPropertiesSupported && !IsNestedPropertyDescriptor(property)) return false;
			var propertyValue = property.GetValue(instance);
			if(propertyValue == null) return false;
			PropertyDescriptorCollection properties = null;
			if(isPropertiesSupported)
				properties = converter.GetProperties(propertyValue);
			if(properties == null && !(propertyValue is ICollection))
				properties = TypeDescriptor.GetProperties(propertyValue);
			if(properties != null) {
				foreach(PropertyDescriptor prop in properties) {
					bool test = IsCheckPropertyCore(prop, propertyValue, null, filter, level + 1);
					if(test) return true;
				}
			}
			return false;
		}
		static bool IsTextMatch(string name, string filter) {
			return name.ToLower().Contains(filter);
		}
	}
}
