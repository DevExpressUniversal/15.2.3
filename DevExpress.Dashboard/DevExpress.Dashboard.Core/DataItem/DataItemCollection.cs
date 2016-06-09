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

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess;
namespace DevExpress.DashboardCommon {
	public abstract class DataItemCollection<TDataItem> : NotifyingCollection<TDataItem>, IDataItemCollection<TDataItem> where TDataItem : DataItem {
		IList<string> dataItemNames;
		internal void SaveToXml(XElement element, string xmlCollectionName, string xmlItemName) {
			if(Count > 0) {
				XElement collectionElement = new XElement(xmlCollectionName);
				foreach(TDataItem dataItem in this) {
					XElement itemElement = new XElement(xmlItemName);
					itemElement.SetUniqueNameAttribute(dataItem.SerializableUniqueName);
					collectionElement.Add(itemElement);
				}
				element.Add(collectionElement);
			}
		}
		internal void LoadFromXml(XElement element, string xmlCollectionName, string xmlItemName) {
			XElement collectionElement = element.Element(xmlCollectionName);
			if(collectionElement != null) {
				dataItemNames = new List<string>();
				foreach(XElement itemElement in collectionElement.Elements(xmlItemName))
					dataItemNames.Add(itemElement.GetUniqueNameAttribute());
			}
		}
		internal void OnEndLoading(DataItemRepository dataItemRepository, IDataItemContext context) {			
			if(dataItemNames != null) {
				BeginUpdate();
				try {
					foreach(string name in dataItemNames) {
						TDataItem dataItem = dataItemRepository.FindByUniqueName<TDataItem>(name);
						if(dataItem != null)
							Add(dataItem);
					}
				}
				finally {
					EndUpdate();
				}
				dataItemNames = null;
			}
			foreach(TDataItem dataItem in this) {
				dataItem.Context = context;
				dataItem.OnEndLoading();
			}
		}
	}
}
