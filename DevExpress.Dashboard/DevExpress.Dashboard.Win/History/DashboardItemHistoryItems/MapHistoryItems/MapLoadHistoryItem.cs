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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class MapLoadHistoryItem : DashboardItemHistoryItem<MapDashboardItem> {
		class LoadMapState {
			public ShapefileArea Area { get; private set; }
			public CustomShapefile Custom { get; private set; }
			public LoadMapState(ShapefileArea area, CustomShapefile custom) {
				Area = area;
				Custom = custom;
			}
		}
		readonly LoadMapState prevLoadState;
		readonly LoadMapState nextLoadState;
		readonly MapViewport prevViewport;
		readonly string prevAttributeName;
		readonly string prevTooltipAttributeName;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemMapOpen; } }
		public MapLoadHistoryItem(MapDashboardItem dashboardItem)
			: base(dashboardItem) {
			prevViewport = dashboardItem.Viewport.Clone();
			ChoroplethMapDashboardItem choroplethMap = dashboardItem as ChoroplethMapDashboardItem;
			if(choroplethMap != null) {
				prevAttributeName = choroplethMap.AttributeName;
				prevTooltipAttributeName = choroplethMap.TooltipAttributeName;
			}
		}
		public MapLoadHistoryItem(MapDashboardItem dashboardItem, ShapefileArea area)
			: this(dashboardItem) {
			prevLoadState = new LoadMapState(dashboardItem.Area, dashboardItem.CustomShapefile);
			nextLoadState = new LoadMapState(area, new CustomShapefile(null));
		}
		public MapLoadHistoryItem(MapDashboardItem dashboardItem, string url)
			: this(dashboardItem) {
			prevLoadState = new LoadMapState(dashboardItem.Area, dashboardItem.CustomShapefile);
			nextLoadState = new LoadMapState(ShapefileArea.Custom, new CustomShapefile(null) { Url = url });
		}
		public MapLoadHistoryItem(MapDashboardItem dashboardItem, byte[] shapeData, byte[] attributeData)
			: this(dashboardItem) {
			prevLoadState = new LoadMapState(dashboardItem.Area, dashboardItem.CustomShapefile);
			nextLoadState = new LoadMapState(ShapefileArea.Custom, new CustomShapefile(null) { Data = new CustomShapefileData(shapeData, attributeData) });
		}
		void ApplyFileUrl(LoadMapState state, MapViewport viewport, string attributeName, string tooltipAttributeName) {
			MapDashboardItem dashboardItem = DashboardItem;
			dashboardItem.LockChanging();
			dashboardItem.Area = state.Area;
			dashboardItem.CustomShapefile.Url = state.Custom.Url;
			dashboardItem.CustomShapefile.Data = state.Custom.Data;
			ChoroplethMapDashboardItem choroplethMap = dashboardItem as ChoroplethMapDashboardItem;
			if(choroplethMap != null) {
				choroplethMap.AttributeName = attributeName;
				choroplethMap.TooltipAttributeName = tooltipAttributeName;
				choroplethMap.InitializeAttributeName();
			}
			if(viewport != null)
				dashboardItem.Viewport.Apply(viewport);
			dashboardItem.UnlockChanging();
			dashboardItem.OnChanged(ChangeReason.MapFile);
		}
		protected override void PerformUndo() {
			ApplyFileUrl(prevLoadState, prevViewport, prevAttributeName, prevTooltipAttributeName);
		}
		protected override void PerformRedo() {
			ApplyFileUrl(nextLoadState, null, null, null);
		}
	}
}
