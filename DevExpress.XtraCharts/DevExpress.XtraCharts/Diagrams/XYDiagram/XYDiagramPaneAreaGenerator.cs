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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class XYDiagramPaneAreaGenerator {
		public static List<XYDiagramPaneArea> GenerateAreas(RaggedEdgeGeometry raggedGeometry, WavedEdgeGeometry wavedGeometry, AxisViewData axisViewData) {
			return new XYDiagramPaneAreaGenerator(raggedGeometry, wavedGeometry, axisViewData).GenerateAreas();
		}
		readonly RaggedEdgeGeometry raggedGeometry;
		readonly WavedEdgeGeometry wavedGeometry;
		readonly AxisViewData axisViewData;
		int IntervalsCount { get { return axisViewData.IntervalsViewData.Count; } }
		ScaleBreakOptions ScaleBreakOptions { get { return axisViewData.Axis.ActualScaleBreakOptions; } }
		Color ScaleBreakColor { get { return ScaleBreakOptions != null ? ScaleBreakOptions.Color : Color.Empty; } }
		ScaleBreakStyle ScaleBreakStyle { get { return ScaleBreakOptions != null ? ScaleBreakOptions.Style : ScaleBreakStyle.Straight; } }
		XYDiagramPaneAreaGenerator(RaggedEdgeGeometry raggedGeometry, WavedEdgeGeometry wavedGeometry, AxisViewData axisViewData) {
			this.raggedGeometry = raggedGeometry;
			this.wavedGeometry = wavedGeometry;
			this.axisViewData = axisViewData;
		}
		List<XYDiagramPaneArea> GenerateAreas() {
			List<XYDiagramPaneArea> paneAreas = new List<XYDiagramPaneArea>();
			if(IntervalsCount > 0) {
				if(IntervalsCount > 1) {
					switch(axisViewData.AxisDirection) {
						case AxisDirection.LeftToRight:
							GenerateAreasLeftToRight(paneAreas);
							break;
						case AxisDirection.RightToLeft:
							GenerateAreasRightToLeft(paneAreas);
							break;
						case AxisDirection.TopToBottom:
							GenerateAreasTopToBottom(paneAreas);
							break;
						case AxisDirection.BottomToTop:
							GenerateAreasBottomToTop(paneAreas);
							break;
						default:
							ChartDebug.Fail("Unknown axis direction.");
							break;
					}
				}
				else
					paneAreas.Add(new XYDiagramPaneArea(raggedGeometry, wavedGeometry, axisViewData.IntervalsViewData[0].PaneIntervalBounds));
			}
			return paneAreas;
		}
		void GenerateAreasLeftToRight(List<XYDiagramPaneArea> paneAreas) {
			for(int i = 0; i < IntervalsCount; i++) {
				Rectangle bounds = axisViewData.IntervalsViewData[i].PaneIntervalBounds;
				XYDiagramPaneArea paneArea;
				if(IsFirstInterval(i))
					paneArea = CreateLeftArea(bounds);
				else if(IsLastInterval(i))
					paneArea = CreateRightArea(bounds);
				else
					paneArea = CreateHorizontalArea(bounds);
				paneAreas.Add(paneArea);
			}
		}
		void GenerateAreasRightToLeft(List<XYDiagramPaneArea> paneAreas) {
			for(int i = 0; i < IntervalsCount; i++) {
				Rectangle bounds = axisViewData.IntervalsViewData[i].PaneIntervalBounds;
				XYDiagramPaneArea paneArea;
				if(IsFirstInterval(i))
					paneArea = CreateRightArea(bounds);
				else if(IsLastInterval(i))
					paneArea = CreateLeftArea(bounds);
				else
					paneArea = CreateHorizontalArea(bounds);
				paneAreas.Add(paneArea);
			}
		}
		void GenerateAreasTopToBottom(List<XYDiagramPaneArea> paneAreas) {
			for(int i = 0; i < IntervalsCount; i++) {
				Rectangle bounds = axisViewData.IntervalsViewData[i].PaneIntervalBounds;
				XYDiagramPaneArea paneArea;
				if(IsFirstInterval(i))
					paneArea = CreateTopArea(bounds);
				else if(IsLastInterval(i))
					paneArea = CreateBottomArea(bounds);
				else
					paneArea = CreateVerticalArea(bounds);
				paneAreas.Add(paneArea);
			}
		}
		void GenerateAreasBottomToTop(List<XYDiagramPaneArea> paneAreas) {
			for(int i = 0; i < IntervalsCount; i++) {
				Rectangle bounds = axisViewData.IntervalsViewData[i].PaneIntervalBounds;
				XYDiagramPaneArea paneArea;
				if(IsFirstInterval(i))
					paneArea = CreateBottomArea(bounds);
				else if(IsLastInterval(i))
					paneArea = CreateTopArea(bounds);
				else
					paneArea = CreateVerticalArea(bounds);
				paneAreas.Add(paneArea);
			}
		}
		static bool IsFirstInterval(int intervalIndex) {
			return intervalIndex == 0;
		}
		bool IsLastInterval(int intervalIndex) {
			return intervalIndex == IntervalsCount - 1;
		}
		PaneAreaSide CreateInnerSide() {
			return PaneAreaSide.CreateInner(ScaleBreakStyle, ScaleBreakColor);
		}
		XYDiagramPaneArea CreateVerticalArea(Rectangle bounds) {
			return new XYDiagramPaneArea(raggedGeometry, wavedGeometry, bounds, PaneAreaSide.CreateOuter(), CreateInnerSide(), PaneAreaSide.CreateOuter(), CreateInnerSide());
		}
		XYDiagramPaneArea CreateHorizontalArea(Rectangle bounds) {
			return new XYDiagramPaneArea(raggedGeometry, wavedGeometry, bounds, CreateInnerSide(), PaneAreaSide.CreateOuter(), CreateInnerSide(), PaneAreaSide.CreateOuter());
		}
		XYDiagramPaneArea CreateLeftArea(Rectangle bounds) {
			return new XYDiagramPaneArea(raggedGeometry, wavedGeometry, bounds, PaneAreaSide.CreateOuter(), PaneAreaSide.CreateOuter(), CreateInnerSide(), PaneAreaSide.CreateOuter());
		}
		XYDiagramPaneArea CreateRightArea(Rectangle bounds) {
			return new XYDiagramPaneArea(raggedGeometry, wavedGeometry, bounds, CreateInnerSide(), PaneAreaSide.CreateOuter(), PaneAreaSide.CreateOuter(), PaneAreaSide.CreateOuter());
		}
		XYDiagramPaneArea CreateTopArea(Rectangle bounds) {
			return new XYDiagramPaneArea(raggedGeometry, wavedGeometry, bounds, PaneAreaSide.CreateOuter(), PaneAreaSide.CreateOuter(), PaneAreaSide.CreateOuter(), CreateInnerSide());
		}
		XYDiagramPaneArea CreateBottomArea(Rectangle bounds) {
			return new XYDiagramPaneArea(raggedGeometry, wavedGeometry, bounds, PaneAreaSide.CreateOuter(), CreateInnerSide(), PaneAreaSide.CreateOuter(), PaneAreaSide.CreateOuter());
		}
	}	
}
