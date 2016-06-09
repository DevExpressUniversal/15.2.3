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
using System.Xml;
using System.Xml.Linq;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native {
	public class DataItemBox<TDataItem> where TDataItem : DataItem {
		const string xmlUniqueName = "UniqueName";
		readonly IDataItemContext dataItemContext;
		readonly IDataItemRepositoryProvider repositoryProvider;		
		readonly string xmlName;
		readonly DataItemXmlType xmlType;
		TDataItem dataItem;
		string uniqueName;
		public TDataItem DataItem {
			get { return dataItem; }
			set {
				if(value != dataItem) {
					TDataItem oldDataItem = dataItem;
					dataItem = value;
					if(Changed != null)
						Changed(this, new DataItemBoxChangedEventArgs<TDataItem>(oldDataItem, dataItem));
				}
			}
		}
		public string UniqueName {
			get { return IsDataItemValid ? DataItemRepository.GetSerializableUniqueName(dataItem) : null; }
			set { uniqueName = value; }
		}
		public bool ShouldSerialize { get { return IsDataItemValid; } }
		bool IsDataItemValid { get { return dataItem != null && DataItemRepository != null && DataItemRepository.Contains(dataItem); } }
		DataItemRepository DataItemRepository { get { return repositoryProvider != null ? repositoryProvider.DataItemRepository : null; } }
		public event EventHandler<DataItemBoxChangedEventArgs<TDataItem>> Changed;
		public DataItemBox(IDataItemContext dataItemContext, string xmlName)
			: this(dataItemContext, dataItemContext.DataItemRepositoryProvider, xmlName, DataItemXmlType.Element) {
		}
		public DataItemBox(IDataItemRepositoryProvider repositoryProvider, string xmlName)
			: this(null, repositoryProvider, xmlName, DataItemXmlType.Attribute) {
		}
		public DataItemBox(IDataItemContext dataItemContext, IDataItemRepositoryProvider repositoryProvider, string xmlName, DataItemXmlType xmlType) {
			Guard.ArgumentIsNotNullOrEmpty(xmlName, "xmlName");
			this.dataItemContext = dataItemContext;
			this.repositoryProvider = repositoryProvider;
			this.xmlName = xmlName;
			this.xmlType = xmlType;
		}
		public void SaveToXml(XElement element) {
			if(ShouldSerialize) {
				switch(xmlType) {
					case DataItemXmlType.Element:
						XElement dataItemElement = new XElement(xmlName);
						dataItemElement.Add(new XAttribute(xmlUniqueName, UniqueName));
						element.Add(dataItemElement);
						break;
					case DataItemXmlType.Attribute:
						element.Add(new XAttribute(xmlName, UniqueName));
						break;
					default: 
						goto case DataItemXmlType.Element;
				}
			}
		}
		public void LoadFromXml(XElement element) {
			switch(xmlType) {
				case DataItemXmlType.Element:
					XElement dataItemElement = element.Element(xmlName);
					if(dataItemElement != null) {
						uniqueName = dataItemElement.GetAttributeValue(xmlUniqueName);
						if(String.IsNullOrEmpty(uniqueName))
							throw new XmlException();
					}
					break;
				case DataItemXmlType.Attribute:
					uniqueName = element.GetAttributeValue(xmlName);
					break;
				default:
					goto case DataItemXmlType.Element;
			}
		}
		public void OnEndLoading() {
			if(dataItem == null && !string.IsNullOrEmpty(uniqueName))
				dataItem = DataItemRepository.FindByUniqueName<TDataItem>(uniqueName);
			if(dataItem != null) {
				if(dataItemContext != null)
					dataItem.Context = dataItemContext;
				dataItem.OnEndLoading();
			}
			uniqueName = null;
		}
	}
	public enum DataItemXmlType {
		Element,
		Attribute
	}
	public class DataItemBoxChangedEventArgs<TDataItem> : EventArgs where TDataItem : DataItem {
		public TDataItem OldDataItem { get; private set; }
		public TDataItem NewDataItem { get; private set; }
		public DataItemBoxChangedEventArgs(TDataItem oldDataItem, TDataItem newDataItem) {
			OldDataItem = oldDataItem;
			NewDataItem = newDataItem;
		}
	}
}
