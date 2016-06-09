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
using System.Text;
using System.ComponentModel;
using System.Collections;
using DevExpress.Data.Browsing;
using DevExpress.Data.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraReports.Native.Data {
	public class PropertyAggregator {
		public const string ReferenceAsObjectTail = "!";
		public const string ReferenceAsKeyTail = "!Key";
		public static PropertyDescriptor[] Aggregate(ICollection properties) {
			return new PropertyAggregator().Aggregate(properties, null, string.Empty);
		}
		DataContext dataContext;
		public PropertyAggregator(DataContext dataContext) {
			this.dataContext = dataContext;
		}
		public PropertyAggregator() {
		}
		public PropertyDescriptor[] Aggregate(ICollection properties, object dataSource, string dataMember) {
			Dictionary<string, List<PropertyDescriptor>> dictionary = new Dictionary<string, List<PropertyDescriptor>>();
			foreach(PropertyDescriptor property in properties) {
				string name = GetName(property);
				if(string.IsNullOrEmpty(name))
					continue;
				List<PropertyDescriptor> list;
				if(!dictionary.TryGetValue(name, out list)) {
					list = new List<PropertyDescriptor>();
					dictionary.Add(name, list);
				}
				list.Add(property);
			}
			List<PropertyDescriptor> result = new List<PropertyDescriptor>();
			foreach(List<PropertyDescriptor> item in dictionary.Values)
				result.Add(GetProperty(item, dataSource, dataMember));
			return result.ToArray();
		}
		protected string GetName(PropertyDescriptor property) {
			string propertyName = property.Name;
			if(property.GetType().FullName != "DevExpress.Xpo.Helpers.XPPropertyDescriptor")
				return propertyName;
			if(propertyName.EndsWith(ReferenceAsObjectTail))
				return propertyName.Substring(0, propertyName.Length - ReferenceAsObjectTail.Length);
			if(propertyName.EndsWith(ReferenceAsKeyTail))
				return propertyName.Substring(0, propertyName.Length - ReferenceAsKeyTail.Length);
			return propertyName;
		}
		PropertyDescriptor GetProperty(List<PropertyDescriptor> list, object dataSource, string dataMember) {
			if(list.Count == 2 || list.Count == 3) {
				PropertyDescriptor property = list.Find(delegate(PropertyDescriptor prop) { return prop.Name.EndsWith(ReferenceAsObjectTail); });
				PropertyDescriptor keyProperty = list.Find(delegate(PropertyDescriptor prop) { return prop.Name.EndsWith(ReferenceAsKeyTail); });
				if(property != null && keyProperty != null) {
					PropertyDescriptor aggregatedProperty = GetAggregatedProperty(property, list, dataSource, dataMember);
					if(aggregatedProperty != null)
						return aggregatedProperty;
				}
			}
			return list[0];
		}
		protected virtual PropertyDescriptor GetAggregatedProperty(PropertyDescriptor property, List<PropertyDescriptor> list, object dataSource, string dataMember) {
			string propName = property.Name.TrimEnd('!');
			PropertyDescriptor collectionProperty = list.Find(delegate(PropertyDescriptor prop) { return prop.Name == propName; });
			if(collectionProperty == null && dataContext != null) {
				DataBrowser dataBrowser = dataContext.GetDataBrowser(dataSource, dataMember, true);
				if(dataBrowser != null)
					collectionProperty = dataBrowser.FindItemProperty(propName, true);
			}
			if(collectionProperty != null)
				return new AggregatedPropertyDescriptor(collectionProperty, property.PropertyType);
			return null;
		}
	}
	public class AggregatedPropertyDescriptor : PropertyDescriptorWrapper {
		Type propertyType;
		String displayName;
		string name;
		public AggregatedPropertyDescriptor(PropertyDescriptor oldPropertyDescriptor, Type propertyType) : this(oldPropertyDescriptor, propertyType, oldPropertyDescriptor.Name) { 
		}
		public AggregatedPropertyDescriptor(PropertyDescriptor oldPropertyDescriptor, Type propertyType, string name)
			: base(oldPropertyDescriptor) {
			this.propertyType = propertyType;
			displayName = oldPropertyDescriptor.DisplayName;
			this.name = name;
		}
		public override string Name {
			get {
				return name;
			}
		}
		public override string DisplayName {
			get {
				return displayName;
			}
		}
		public override Type PropertyType {
			get {
				return propertyType;
			}
		}
	}
}
