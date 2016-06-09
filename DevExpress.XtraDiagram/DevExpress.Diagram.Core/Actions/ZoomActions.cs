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
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Diagram.Core {
	public static class ZoomActions {
		class ZoomRangeData {
			internal readonly double ZoomFactorFrom;
			internal readonly double ZoomFactorTo;
			internal readonly double Delta;
			public ZoomRangeData(double zoomFactorFrom, double zoomFactorTo, double delta) {
				ZoomFactorFrom = zoomFactorFrom;
				ZoomFactorTo = zoomFactorTo;
				Delta = delta;
			}
		}
		readonly static ComparerHelper<ZoomRangeData> comparer = new ComparerHelper<ZoomRangeData>((x, y) => {
			if(y.ZoomFactorFrom >= x.ZoomFactorFrom && y.ZoomFactorFrom < x.ZoomFactorTo)
				return 0;
			return x.ZoomFactorFrom > y.ZoomFactorFrom ? 1 : -1;
		});
		readonly static List<ZoomRangeData> zoomRangeData = new List<ZoomRangeData>();
		static ZoomActions() {
			double delta = 0.1;
			for(double i = 0; i <= DiagramController.MaxZoomFactor; ++i) {
				zoomRangeData.Add(new ZoomRangeData(i, i + 1.0, delta));
				delta += 0.05;
			}
		}
		public static void ZoomIn(this IDiagramControl diagram) {
			diagram.ZoomFactor = Math.Round(diagram.ZoomFactor + zoomRangeData[GetZoomDataIndex(diagram.ZoomFactor)].Delta, 2);
		}
		public static void ZoomOut(this IDiagramControl diagram) {
			var index = GetZoomDataIndex(diagram.ZoomFactor);
			var zoomDelta = (index == 0 || (diagram.ZoomFactor >= zoomRangeData[index - 1].Delta + zoomRangeData[index].ZoomFactorFrom)) ? zoomRangeData[index].Delta : zoomRangeData[index - 1].Delta;
			diagram.ZoomFactor = Math.Round(diagram.ZoomFactor - zoomDelta, 2);
		}
		public static void SetZoom(this IDiagramControl diagram, double zoomFactor) {
			diagram.ZoomFactor = zoomFactor;
		}
		static int GetZoomDataIndex(double currentValue) {
			return zoomRangeData.BinarySearch(new ZoomRangeData(currentValue, double.MinValue, double.MinValue), comparer);
		}
	}
}
