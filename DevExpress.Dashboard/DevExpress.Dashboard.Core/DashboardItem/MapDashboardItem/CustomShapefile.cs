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

using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public class CustomShapefile {
		const string xmlUrl = "Url";
		const string xmlData = "Data";
		readonly MapDashboardItem dashboardItem;
		string url;
		CustomShapefileData data;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CustomShapefileUrl"),
#endif
		Category(CategoryNames.General),
		Editor("DevExpress.DashboardWin.Design.ShapefileNameEditor," + AssemblyInfo.SRAssemblyDashboardWinDesign, typeof(UITypeEditor)),
		DefaultValue(null),
		Localizable(false)
		]
		public string Url {
			get { return url; }
			set {
				if(url != value) {
					url = value;
					data = null;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CustomShapefileData"),
#endif
		Category(CategoryNames.General),
		Editor("DevExpress.DashboardWin.Design.ShapefileDataEditor," + AssemblyInfo.SRAssemblyDashboardWinDesign, typeof(UITypeEditor)),
		DefaultValue(null)
		]
		public CustomShapefileData Data {
			get { return data; }
			set {
				if(data != value) {
					data = value;
					url = null;
					if(data != null)
						data.DashboardItem = dashboardItem;
					OnChanged();
				}
			}
		}
		internal CustomShapefile(MapDashboardItem dashboardItem) {
			this.dashboardItem = dashboardItem;
		}
		internal void SaveToXml(XElement element) {
			if(!string.IsNullOrEmpty(Url))
				element.Add(new XAttribute(xmlUrl, url));
			if(Data != null) {
				XElement dataElement = new XElement(xmlData);
				Data.SaveToXml(dataElement);
				element.Add(dataElement);
			}
		}
		internal void LoadFromXml(XElement element) {
			string urlAttr = XmlHelper.GetAttributeValue(element, xmlUrl);
			if(!string.IsNullOrEmpty(urlAttr))
				url = urlAttr;
			XElement dataElement = element.Element(xmlData);
			if(dataElement != null) {
				Data = new CustomShapefileData();
				Data.LoadFromXml(dataElement);
			}
		}
		internal bool ShouldSerialize() {
			return !string.IsNullOrEmpty(Url) || Data != null;
		}
		void OnChanged() {
			if(dashboardItem != null && !dashboardItem.Loading) {
				dashboardItem.ResetMapItems();
				dashboardItem.OnChanged(ChangeReason.MapFile);
			}
		}
		internal void CopyTo(CustomShapefile destination) {
			destination.Url = url;
			if (data != null)
				destination.Data = data.Clone();
		}
	}
}
