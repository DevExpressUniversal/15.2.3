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
using System.Xml;
using System.Xml.Linq;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public abstract class ListXmlSerializer<TItem> where TItem : class {
		readonly string xmlListName;
		readonly bool allowUnknownItemsOnLoad;
		public IList<TItem> List { get; set; }
		bool IsListElementEnabled { get { return xmlListName != null; } }
		protected ListXmlSerializer(string xmlListName, bool allowUnknownItemsOnLoad) {
			this.xmlListName = xmlListName;
			this.allowUnknownItemsOnLoad = allowUnknownItemsOnLoad;
		}
		public void SaveToXml(XElement element) {
			if(List.Count > 0) {
				XElement listElement;
				if(IsListElementEnabled) {
					listElement = new XElement(xmlListName);
					element.Add(listElement);
				}
				else
					listElement = element;
				foreach(TItem item in List) {
					XElement itemElement = SaveItemToXml(item);
					if(itemElement != null)
						listElement.Add(itemElement);
				}
			}
		}
		public void LoadFromXml(XElement element) {
			XElement listElement = IsListElementEnabled ? element.Element(xmlListName) : element;
			if(listElement != null) {
				IUpdateLocker updateLocker = List as IUpdateLocker;
				if(updateLocker != null)
					updateLocker.BeginUpdate();
				try {
					List.Clear();
					foreach(XElement itemElement in listElement.Elements()) {
						TItem item = LoadItemFromXml(itemElement);
						if(item != null)
							List.Add(item);
						else if(!allowUnknownItemsOnLoad)
							throw new XmlException();
					}
				}
				finally {
					if(updateLocker != null)
						updateLocker.EndUpdate();
				}
			}
		}
		protected abstract XElement SaveItemToXml(TItem item);
		protected abstract TItem LoadItemFromXml(XElement itemElement);
	}
}
