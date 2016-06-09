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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class CrosshairPaneViewDataFactory {
		TextMeasurer textMeasurer;
		CrosshairOptions crosshairOptions;
		List<CrosshairPaneViewData> crosshairPaneViewDataList;
		CustomDrawCrosshairEventArgs customDrawCrosshairEventArgs;
		CrosshairSeriesLabel crosshairSeriesLabel;
		public List<CrosshairPaneViewData> CrosshairPaneViewDataList {
			get {
				return crosshairPaneViewDataList;
			}
		}
		public CustomDrawCrosshairEventArgs CustomDrawCrosshairEventArgs {
			get {
				return customDrawCrosshairEventArgs;
			}
		}
		public CrosshairPaneViewDataFactory(TextMeasurer textMeasurer, TextAnnotationAppearance textAppearance, CrosshairOptions crosshairOptions) {
			this.textMeasurer = textMeasurer;
			this.crosshairOptions = crosshairOptions;
			this.crosshairSeriesLabel = new CrosshairSeriesLabel(textAppearance, ((ICrosshairOptions)crosshairOptions).ShowTail);
		}
		public void ProcessCrosshairInfoEx(CrosshairInfoEx crosshairInfos, List<CrosshairHighlightedPointInfo> highlightedPointsInfo, bool useCommonSeriesLabel) {
			crosshairPaneViewDataList = new List<CrosshairPaneViewData>();
			List<CrosshairElementGroup> elementGroups = new List<CrosshairElementGroup>();
			CrosshairLineElement lineElement = null;
			List<CrosshairAxisLabelElement> axisLabelElements = new List<CrosshairAxisLabelElement>();
			foreach (CrosshairPaneInfoEx crosshairInfo in crosshairInfos) {
				CrosshairPaneViewData crosshairPaneViewData = new CrosshairPaneViewData(crosshairInfo, crosshairSeriesLabel, textMeasurer, crosshairOptions, highlightedPointsInfo);
				if (crosshairPaneViewData != null) {
					crosshairPaneViewDataList.Add(crosshairPaneViewData);
					if (lineElement == null)
						lineElement = crosshairPaneViewData.CrosshairLineElement;
					else
						crosshairPaneViewData.CrosshairLineElement = lineElement;
					if (useCommonSeriesLabel) {
						if (crosshairPaneViewData.CrosshairElementGroups.Count > 0)
							elementGroups = crosshairPaneViewData.CrosshairElementGroups;
					}
					else
						elementGroups.AddRange(crosshairPaneViewData.CrosshairElementGroups);
					axisLabelElements.AddRange(crosshairPaneViewData.CrosshairAxisLabelElements);
				}
			}
			customDrawCrosshairEventArgs = new CustomDrawCrosshairEventArgs(elementGroups, lineElement, axisLabelElements);
		}
		public void ProcessAfterCustomDraw() {
			foreach (CrosshairPaneViewData paneViewData in crosshairPaneViewDataList) {
				paneViewData.CalculatePointsHighlightingVisibility();
			}
		}
	}
	public class CrosshairHighlightedPointsDictionary : Hashtable {
		public List<CrosshairHighlightedPointInfo> this[ISeriesPoint key] { get { return base[key] as List<CrosshairHighlightedPointInfo>; } }
	}
	public class CrosshairHighlightedPointInfo {
		readonly HighlightedPointLayout layout;
		readonly Region clipRegion;
		readonly Series series;
		bool visible;
		public HighlightedPointLayout Layout { get { return layout; } }
		public Region ClipRegion { get { return clipRegion; } }
		public Series Series { get { return series; } }
		public bool Visible { get { return visible; } }
		public CrosshairHighlightedPointInfo(HighlightedPointLayout layout, Series series, Region clipRegion) {
			this.layout = layout;
			this.clipRegion = clipRegion;
			this.series = series;
			visible = true;
		}
		public void Hide() {
			visible = false;
		}
	}
}
