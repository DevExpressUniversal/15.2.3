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
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon {
	[
	EditorBrowsable(EditorBrowsableState.Never)
	]
	public class DataItemNameRepository {
		const string xmlNames = "Names";
		const string xmlName = "Name";
		const string xmlValue = "Value";
		readonly Dictionary<string, string> namesToLoad = new Dictionary<string, string>();
		readonly IDataItemRepositoryProvider repositoryProvider;
		internal DataItemNameRepository(IDataItemRepositoryProvider repositoryProvider) {
			Guard.ArgumentNotNull(repositoryProvider, "repositoryProvider");
			this.repositoryProvider = repositoryProvider;
		}
		public void Clear() {
		}
		public void Add(DataItem dataItem, string name) {
			dataItem.Name = name;
		}
		internal void LoadFromXml(XElement element) {
			XElement namesElement = element.Element(xmlNames);
			if(namesElement != null) {
				foreach(XElement nameElement in namesElement.Elements()) {
					string uniqueName = XmlHelper.GetAttributeValue(nameElement, xmlValue);
					if(!string.IsNullOrEmpty(uniqueName)) {
						string name = XmlHelper.GetAttributeValue(nameElement, xmlName);
						if(!string.IsNullOrEmpty(name))
							namesToLoad.Add(uniqueName, name);
					}
				}
			}
		}
		internal void OnEndLoading() {
			DataItemRepository dataItemRepository = repositoryProvider.DataItemRepository;
			foreach(KeyValuePair<string, string> pair in namesToLoad) {
				foreach(DataItem dataItem in dataItemRepository.DataItems) {
					if(dataItemRepository.GetSerializableUniqueName(dataItem) == pair.Key) {
						if(dataItem != null)
							dataItem.Name = pair.Value;
						break;
					}
				}
			}
		}
	}
}
