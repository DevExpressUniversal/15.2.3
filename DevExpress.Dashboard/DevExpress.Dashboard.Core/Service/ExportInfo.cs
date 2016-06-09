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
using System.Drawing;
using DevExpress.DashboardCommon.Printing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.Service {
	public enum ClientDataSortOrderState { Default, Reversed };
	public enum ExportPivotColumnTotalsLocation { Near = 0, Far = 1 }
	public enum ExportPivotRowTotalsLocation { Near = 0, Far = 1, Tree = 2 }
	public class ExportInfo {
		public string GroupName { get; set; }
		public DashboardExportMode Mode { get; set; }
		public DashboardReportOptions ExportOptions { get; set; }
		public ViewerState ViewerState { get; set; }
		public DashboardFontInfo FontInfo { get; set; }
	}
	public class ViewerState {
		public int TitleHeight { get; set; }
		public Size Size { get; set; }
		public Dictionary<string, ItemViewerClientState> ItemsState { get; set; }
	}
	public class ScrollingState {
		public double PositionRatio { get; set; }
		public int VirtualSize { get; set; }
		public int ScrollableAreaSize { get; set; }
		public object[] PositionListSourceRow { get; set; }
		public int ScrollBarSize { get; set; }
	}
	public class MapViewportState {
		public double TopLatitude { get; set; }
		public double BottomLatitude { get; set; }
		public double LeftLongitude { get; set; }
		public double RightLongitude { get; set; }
		public double CenterPointLatitude { get; set; }
		public double CenterPointLongitude { get; set; }
	}
	public static class MapViewPortStateExtension {
		public static bool IsCoordinatesInViewPort(this MapViewportState currentViewPort, double latitude, double longitude) {
			return latitude > currentViewPort.BottomLatitude && latitude < currentViewPort.TopLatitude && longitude > currentViewPort.LeftLongitude && longitude < currentViewPort.RightLongitude;
		}
	}
	public class ClientArea {
		public int Left { get; set; }
		public int Top { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
	}
	public class ItemViewerClientState {
		public Dictionary<string, ClientDataSortOrderState> SortOrderState { get; set; }
		public Dictionary<string, object> SpecificState { get; set; }
		public ClientArea CaptionArea { get; set; }
		public ClientArea ViewerArea { get; set; }
		public ScrollingState VScrollingState { get; set; }
		public ScrollingState HScrollingState { get; set; }
	}
}
