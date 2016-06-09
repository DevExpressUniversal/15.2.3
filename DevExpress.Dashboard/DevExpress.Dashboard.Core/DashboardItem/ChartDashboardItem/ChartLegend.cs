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
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon {
	public enum ChartLegendInsidePosition {
		TopLeftVertical, TopLeftHorizontal, TopCenterHorizontal, TopCenterVertical, TopRightVertical, TopRightHorizontal,
		BottomLeftVertical, BottomLeftHorizontal, BottomCenterHorizontal, BottomCenterVertical, BottomRightVertical, BottomRightHorizontal
	}
	public enum ChartLegendOutsidePosition {
		TopLeftVertical, TopLeftHorizontal, TopCenterHorizontal, TopRightVertical, TopRightHorizontal,
		BottomLeftVertical, BottomLeftHorizontal, BottomCenterHorizontal, BottomRightVertical, BottomRightHorizontal
	}
	public class ChartLegend {
		const string xmlLegend = "ChartLegend";
		const string xmlVisible = "Visible";
		const string xmlIsInsidePosition = "IsInsidePosition";
		const string xmlInsidePosition = "InsidePosition";
		const string xmlOutsidePosition = "OutsidePosition";
		const bool DefaultVisible = true;
		const bool DefaultIsInsideDiagram = false;
		const ChartLegendOutsidePosition DefaultOutsidePosition = ChartLegendOutsidePosition.TopRightHorizontal;
		const ChartLegendInsidePosition DefaultInsidePosition = ChartLegendInsidePosition.TopRightHorizontal;
		readonly DataDashboardItem dashboardItem;
		ChartLegendOutsidePosition outsidePosition = DefaultOutsidePosition;
		ChartLegendInsidePosition insidePosition = DefaultInsidePosition;
		bool isInsideDiagram = DefaultIsInsideDiagram;
		bool visible = DefaultVisible;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartLegendOutsidePosition"),
#endif
		DefaultValue(DefaultOutsidePosition)
		]
		public ChartLegendOutsidePosition OutsidePosition {
			get { return outsidePosition; }
			set {
				if (value != outsidePosition) {
					outsidePosition = value;					
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartLegendInsidePosition"),
#endif
		DefaultValue(DefaultInsidePosition)
		]
		public ChartLegendInsidePosition InsidePosition {
			get { return insidePosition; }
			set {
				if (value != insidePosition) {
					insidePosition = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartLegendIsInsideDiagram"),
#endif
		DefaultValue(DefaultIsInsideDiagram)
		]
		public bool IsInsideDiagram {
			get { return isInsideDiagram; }
			set {
				if (value != isInsideDiagram) {
					isInsideDiagram = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartLegendVisible"),
#endif
		DefaultValue(DefaultVisible)
		]
		public bool Visible {
			get { return visible; }
			set {
				if (value != visible) {
					visible = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		internal ChartLegend(ChartDashboardItem chartDashboardItem) {
			this.dashboardItem = chartDashboardItem;
		}
		internal ChartLegend(ScatterChartDashboardItem scatterDashboardItem) {
			this.dashboardItem = scatterDashboardItem;
		}
		internal XElement SaveToXml() {
			XElement element = new XElement(xmlLegend);
			if(Visible != DefaultVisible)
				element.Add(new XAttribute(xmlVisible, visible));
			if(IsInsideDiagram != DefaultIsInsideDiagram)
				element.Add(new XAttribute(xmlIsInsidePosition, isInsideDiagram));
			if(InsidePosition != DefaultInsidePosition)
				element.Add(new XAttribute(xmlInsidePosition, insidePosition));
			if(OutsidePosition != DefaultOutsidePosition)
				element.Add(new XAttribute(xmlOutsidePosition, outsidePosition));
			return element;
		}
		internal void LoadFromXml(XElement element) {
			XElement legendElement = element.Element(xmlLegend);
			if (legendElement != null) {
				string argument = XmlHelper.GetAttributeValue(legendElement, xmlVisible);
				if (!String.IsNullOrEmpty(argument))
					visible = XmlHelper.FromString<bool>(argument);
				argument = XmlHelper.GetAttributeValue(legendElement, xmlIsInsidePosition);
				if (!String.IsNullOrEmpty(argument))
					isInsideDiagram = XmlHelper.FromString<bool>(argument);
				argument = XmlHelper.GetAttributeValue(legendElement, xmlInsidePosition);
				if (!String.IsNullOrEmpty(argument))
					insidePosition = XmlHelper.EnumFromString<ChartLegendInsidePosition>(argument);
				argument = XmlHelper.GetAttributeValue(legendElement, xmlOutsidePosition);
				if (!String.IsNullOrEmpty(argument))
					outsidePosition = XmlHelper.EnumFromString<ChartLegendOutsidePosition>(argument);
			}
		}
		internal bool ShouldSerialize() {
			return OutsidePosition != DefaultOutsidePosition || InsidePosition != DefaultInsidePosition || IsInsideDiagram != DefaultIsInsideDiagram || Visible != DefaultVisible;
		}
		void OnChanged(ChangeReason reason) {
			if(dashboardItem != null)
				dashboardItem.OnChanged(reason);
		}
	}
}
