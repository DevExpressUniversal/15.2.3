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

using System.Xml;
using System.Xml.Linq;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public class DataItemXmlSerializationContext {
		public string UniqueName { get; set; }
	}
	public class DataItemXmlSerializer<TDataItem> : ClassXmlSerializer<TDataItem, DataItem> where TDataItem : DataItem, new() {
		const string xmlUniqueName = "UniqueName";
		public DataItemXmlSerializer(string name)
			: base(name) {
		}
		protected override void SaveToXmlInternal(XElement element, DataItem dataItem, object context) {
			dataItem.SaveToXml(element);
			if(context != null)
				element.Add(new XAttribute(xmlUniqueName, ((DataItemXmlSerializationContext)context).UniqueName));
		}
		protected override void LoadFromXmlInternal(XElement element, DataItem dataItem, object context) {
			if (context != null) {
				string uniqueName = element.GetAttributeValue(xmlUniqueName);
				string name = dataItem.LoadNameFromXml(element);
				if (!string.IsNullOrEmpty(uniqueName))
					dataItem.Name = name;
				else if (!string.IsNullOrEmpty(name))
					uniqueName = name;
				else
					throw new XmlException();
				((DataItemXmlSerializationContext)context).UniqueName = uniqueName;
			}
			dataItem.LoadFromXml(element);			   
		}
	}
}
