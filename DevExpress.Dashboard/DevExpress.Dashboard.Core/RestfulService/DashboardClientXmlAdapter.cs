#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Xml.Linq;
namespace DevExpress.DashboardCommon.Native.DashboardRestfulService {
	public static class DashboardClientXmlAdapter {
		#region Constants
		class XmlDashboardNames {
			public const string Items = "Items";
			public const string Dashboard = "Dashboard";
			public const string ItemTypeNew = "Item";
			public const string ItemTypeAttribute = "ItemType";
			public const string QueryArray = "Queries";
			public const string LayoutItemsArray = "LayoutItems";
			public const string ComponentNameAttribute = "ComponentName";
			public const string SqlDataSource = "SqlDataSource";
			public const string OLAPDataSource = "OLAPDataSource";
			public const string ObjectDataSource = "ObjectDataSource";
			public const string EFDataSource = "EFDataSource";
		}
		static string[] XmlDashboardCollectionNames = new string[] { 
			XmlDashboardNames.Items, "Groups", "Parameters", "CalculatedFields", "DataItems", "HiddenDimensions", "FormatRules",
			XmlDashboardNames.QueryArray, XmlDashboardNames.LayoutItemsArray,
			"DataSources", "Query", "Table", "View", "DataSet",
			"GridColumns",
			"Values", "Columns", "Rows",
			"Panes", "Arguments", "Values", "Series", "SeriesDimensions",
			"FilterDimensions",
			"Cards",
			"TooltipDimensions", "TooltipMeasures", "Maps",
			"Gauges",
			"Dimensions"
		};
		static List<ArrayElementInfo> XmlDashboardArrayElementNames = new List<ArrayElementInfo>() {
			new ArrayElementInfo("Query", XmlDashboardNames.QueryArray),
			new ArrayElementInfo("LayoutGroup", XmlDashboardNames.LayoutItemsArray),
			new ArrayElementInfo("LayoutItem", XmlDashboardNames.LayoutItemsArray),
			new ArrayElementInfo("Card", "Cards", "Card"),
			new ArrayElementInfo("GaugeElement", "Gauges", "Gauge")
		};
		#endregion
		public static XElement GetDashboardRootElement(XDocument doc) {
			return doc.Element(XmlDashboardNames.Dashboard);
		}
		public static void PatchDashboardXML(XElement element, Func<string, IDictionary<string,object>, string> urlResolverCallback) {
			PatchToArrays(element);
			PatchCollections(element);
			AddUrls(element, urlResolverCallback);
		}
		public static void UnpatchDashboardXML(XElement element) {
			RemoveUrls(element);
			UnpatchCollections(element);
			UnpatchFromArrays(element);			
		}
		static void PatchToArrays(XElement itemsElement) {
			Dictionary<string, IList<XElement>> arrays = new Dictionary<string, IList<XElement>>();
			foreach(XElement element in itemsElement.Elements()) {
				ArrayElementInfo elementInfo = XmlDashboardArrayElementNames.Find(info => info.IsElementValid(element));
				if (elementInfo != null) {
					IList<XElement> array;
					if(!arrays.TryGetValue(elementInfo.ArrayName, out array)) {
						array = new List<XElement>();
						arrays.Add(elementInfo.ArrayName, array);
					}
					array.Add(element);
				}
				PatchToArrays(element);
			}
			foreach(KeyValuePair<string, IList<XElement>> pair in arrays) {
				foreach(XElement element in pair.Value) {
					XElement parent = element.Parent;
					XElement array = parent.Element(pair.Key);
					if(array == null) {
						array = new XElement(pair.Key, element);
						element.ReplaceWith(array);
					} else {
						element.Remove();
						array.Add(element);
					}
				}
			}
		}
		static void UnpatchFromArrays(XElement itemsElement) {
			foreach(XElement element in itemsElement.Elements()) {
				UnpatchFromArrays(element);
				if (XmlDashboardArrayElementNames.Find(item => item.ArrayName == element.Name.LocalName) != null) {
					element.ReplaceWith(element.Elements());
				}
			}
		}
		static void PatchCollections(XElement itemsElement) {
			foreach(XElement element in itemsElement.Elements()) {
				PatchCollections(element);
			}
			if(Array.IndexOf(XmlDashboardCollectionNames, itemsElement.Name.LocalName) >= 0) {
				int index = 1;
				foreach(XElement element in itemsElement.Elements()) {
					string oldItemTypeName = element.Name.LocalName;
					element.Name = XName.Get(string.Format("{0}{1}", XmlDashboardNames.ItemTypeNew, index++));
					element.Add(new XAttribute(XmlDashboardNames.ItemTypeAttribute, oldItemTypeName));
				}
			}
		}
		static void UnpatchCollections(XElement itemsElement) {
			foreach(XElement patchedElement in itemsElement.Descendants()) {
				XAttribute itemTypeAttribute = patchedElement.Attribute(XmlDashboardNames.ItemTypeAttribute);
				if(itemTypeAttribute != null) {
					patchedElement.Name = XName.Get(itemTypeAttribute.Value);
					itemTypeAttribute.Remove();
				}
			}
		}
		static void AddUrls(XElement element, Func<string, IDictionary<string, object>, string> urlResolverCallback) {
			if (element.Name == "Dashboard") {
				element.Add(new XElement("SelfUrl", urlResolverCallback("dashboardModel", new Dictionary<string, object>())));
			}
			foreach(var item in element.Descendants(XmlDashboardNames.Items).Elements()) { 
				item.Add(new XElement("Url", urlResolverCallback("dashboardItemViewModel", new Dictionary<string, object>() {
					{ "itemId", (string)item.Attribute(XmlDashboardNames.ComponentNameAttribute) }
				})));
			}
			foreach (var item in element.Descendants("Groups").Elements()) {
				item.Add(new XElement("Url", urlResolverCallback("dashboardItemViewModel", new Dictionary<string, object>() {
					{ "itemId", (string)item.Attribute(XmlDashboardNames.ComponentNameAttribute) }
				})));
			}
			string[] dataSources = new string[] {
				XmlDashboardNames.SqlDataSource,
				XmlDashboardNames.OLAPDataSource,
				XmlDashboardNames.ObjectDataSource,
				XmlDashboardNames.EFDataSource
			};
			foreach (var item in element.Descendants("DataSources").Elements().Where(el => dataSources.Contains((string)el.Attribute(XmlDashboardNames.ItemTypeAttribute)))) {
				item.Add(new XElement("ParameterValuesUrl", urlResolverCallback("parameterValues", new Dictionary<string, object>() {
					{ "dataSource", (string)item.Attribute(XmlDashboardNames.ComponentNameAttribute) }
				})));
			}
			string[] notQueryDataSources = new string[] {
				XmlDashboardNames.OLAPDataSource,
				XmlDashboardNames.ObjectDataSource
			};
			foreach (var item in element.Descendants("DataSources").Elements().Where(el => notQueryDataSources.Contains((string)el.Attribute(XmlDashboardNames.ItemTypeAttribute)))) {
				item.Add(new XElement("Url", urlResolverCallback("getFieldList", new Dictionary<string, object>() {
					{ "dataSource", (string)item.Attribute(XmlDashboardNames.ComponentNameAttribute) }
				})));
			}
			foreach (var item in element.Descendants(XmlDashboardNames.QueryArray).Elements()) {
				item.Add(new XElement("Url", urlResolverCallback("getFieldList", new Dictionary<string, object>() {
					{ "dataSource", (string)item.Parent.Parent.Attribute(XmlDashboardNames.ComponentNameAttribute) },
					{ "dataMember", (string)item.Attribute("Name") }
				})));
			}
		}
		static void RemoveUrls(XElement element) {
			element.Descendants("Url").Remove();
			element.Descendants("SelfUrl").Remove();
			element.Descendants("ParameterValuesUrl").Remove();
		}
	}
	public class ArrayElementInfo {
		string elementName;
		string arrayName;
		string elementParentName;
		public string ArrayName { get { return arrayName; } }
		public ArrayElementInfo(string elementName, string arrayName): this(elementName, arrayName, null) {
		}
		public ArrayElementInfo(string elementName, string arrayName, string elementParentName) {
			this.elementName = elementName;
			this.arrayName = arrayName;
			this.elementParentName = elementParentName;
		}
		public bool IsElementValid(XElement element) {
			if (element.Name.LocalName == elementName) {
				if (!string.IsNullOrEmpty(elementParentName) && element.Parent.Name.LocalName != elementParentName) {
					return false;
				}
				return true;
			}
			return false;
		}
	}
}
