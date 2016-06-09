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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class ResolveOverlappingCache : List<ResolveOverlappingCacheItem> {
		public ResolveOverlappingCache(IList<SeriesLabelLayoutList> labelLayoutLists) {
			foreach(SeriesLabelLayoutList labelLayoutList in labelLayoutLists) {
				foreach(SeriesLabelLayout labelLayout in labelLayoutList) {
					XYDiagramSeriesLabelLayout xyLabelLayout = labelLayout as XYDiagramSeriesLabelLayout;
					if(xyLabelLayout != null)
						Add(new ResolveOverlappingCacheItem(xyLabelLayout));
					else {
						ChartDebug.Fail("XYDiagramSeriesLabelLayout is expected.");
						return;
					}
				}
			}
		}
		public void Apply(IList<SeriesLabelLayoutList> labelLayoutLists) {
			int index = 0;
			foreach(SeriesLabelLayoutList labelLayoutList in labelLayoutLists) {
				foreach(SeriesLabelLayout labelLayout in labelLayoutList) {
					if(index < Count) {
						XYDiagramSeriesLabelLayout xyLabelLayout = labelLayout as XYDiagramSeriesLabelLayout;
						if(xyLabelLayout != null)
							this[index++].Apply(xyLabelLayout);
						else {
							ChartDebug.Fail("XYDiagramSeriesLabelLayout is expected.");
							return;
						}
					}
					else {
						ChartDebug.Fail("Invalid ResolveOverlappingCacheItem count.");
						return;
					}
				}
			}
		}
	}
	public class ResolveOverlappingCacheItem {
		readonly bool visible;
		readonly GPoint2D anchorPoint;
		readonly GRect2D labelBounds;
		public ResolveOverlappingCacheItem(XYDiagramSeriesLabelLayout labelLayout) {
			visible = labelLayout.Visible;
			anchorPoint = labelLayout.AnchorPoint;
			labelBounds = labelLayout.LabelBounds;
		}
		public void Apply(XYDiagramSeriesLabelLayout labelLayout) {
			labelLayout.Visible = visible;
			int dx = labelLayout.AnchorPoint.X - anchorPoint.X;
			int dy = labelLayout.AnchorPoint.Y - anchorPoint.Y;
			GRect2D labelBoundsModified = this.labelBounds;
			labelBoundsModified.Offset(dx, dy);
			labelLayout.LabelBounds = labelBoundsModified;
		}
	}
}
