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
using System.Xml;
using System.Xml.Linq;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardFlatLayoutItem : IDashboardFlatLayoutItemInfo {
		internal const string XmlLayoutItem = "LayoutItem";
		const string xmlName = "Name";
		const string xmlLeft = "Left";
		const string xmlRight = "Right";
		const string xmlTop = "Top";
		const string xmlBottom = "Bottom";
		public static DashboardFlatLayoutItem LoadFromXml(XElement element, IEnumerable<DashboardItem> dashboardItems) {
			string name = XmlHelper.GetAttributeValue(element, xmlName);
			if (String.IsNullOrEmpty(name))
				throw new XmlException();
			double left = XmlHelper.LoadDoubleFromXml(element, xmlLeft);
			double right = XmlHelper.LoadDoubleFromXml(element, xmlRight);
			double top = XmlHelper.LoadDoubleFromXml(element, xmlTop);
			double bottom = XmlHelper.LoadDoubleFromXml(element, xmlBottom);
			foreach(DashboardItem dashboardItem in dashboardItems) {
				string dashboardItemName = dashboardItem.UniqueName_13_1 ?? dashboardItem.ComponentName;
				if(dashboardItemName == name)
					return new DashboardFlatLayoutItem(dashboardItem, left, right, top, bottom);
			}
			throw new XmlException();
		}
		readonly DashboardItem dashboardItem;
		readonly double left;
		readonly double right;
		readonly double top;
		readonly double bottom;
		public DashboardItem DashboardItem { get { return dashboardItem; } }
		public double Left { get { return left; } }
		public double Right { get { return right; } }
		public double Top { get { return top; } }
		public double Bottom { get { return bottom; } }
		public DashboardFlatLayoutItem(DashboardItem dashboardItem, double left, double right, double top, double bottom) {
			this.dashboardItem = dashboardItem;
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}
		public XElement SaveToXml() {
			XElement element = new XElement(XmlLayoutItem);
			element.Add(new XAttribute(xmlName, dashboardItem.ComponentName));
			element.Add(new XAttribute(xmlLeft, left));
			element.Add(new XAttribute(xmlTop, top));
			element.Add(new XAttribute(xmlRight, right));
			element.Add(new XAttribute(xmlBottom, bottom));
			return element;
		}
	}
}
