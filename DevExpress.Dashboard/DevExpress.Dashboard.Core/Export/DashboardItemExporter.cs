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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.XtraPrinting;
namespace DevExpress.DashboardExport {
	public abstract class DashboardItemExporter : IDisposable {
		protected static bool HasItemContentOptions(DashboardReportOptions opts) {
			return opts != null && opts.ItemContentOptions != null;
		}
		protected internal static bool EqualsFieldValues(object fieldValue, object pathSegment) {
			return Helper.EqualsWithConversion(fieldValue, pathSegment);
		}
		public static DashboardItemExporter CreateExporter(DashboardExportMode mode, DashboardItemExportData data, DashboardReportOptions opts) {
			switch(data.ServerData.Type) {
				case DashboardItemType.Grid:
					return new GridDashboardItemExporter(mode, data, opts);
				case DashboardItemType.Pivot:
					return new PivotDashboardItemExporter(mode, data, opts);
				case DashboardItemType.Gauge:
					return new GaugeDashboardItemExporter(mode, data, opts);
				case DashboardItemType.Card:
					return new CardDashboardItemExporter(mode, data, opts);
				case DashboardItemType.Chart:
					return new ChartDashboardItemExporter(mode, data);
				case DashboardItemType.ScatterChart:
					return new ScatterChartDashboardItemExporter(mode, data);
				case DashboardItemType.Pie:
					return new PieDashboardItemExporter(mode, data, opts);
				case DashboardItemType.RangeFilter:
					return new RangeFilterDashboardItemExporter(mode, data);
				case DashboardItemType.Text:
					return new TextDashboardItemExporter(mode, data);
				case DashboardItemType.Image:
					return new ImageDashboardItemExporter(mode, data);
				case DashboardItemType.ChoroplethMap:
					return new ChoroplethMapDashboardItemExporter(mode, data);
				case DashboardItemType.GeoPointMap:
					return new GeoPointMapDashboardItemExporter(mode, data);
				case DashboardItemType.BubbleMap:
					return new BubbleMapDashboardItemExporter(mode, data);
				case DashboardItemType.PieMap:
					return new PieMapDashboardItemExporter(mode, data);
				case DashboardItemType.Group:
					return new DashboardItemGroupExporter(mode, data);
				case DashboardItemType.Combobox:
				case DashboardItemType.ListBox:
				case DashboardItemType.TreeView:
					return new FilterElementDashboardItemExporter(mode, data);
				default:
					throw new NotSupportedException();
			}
		}
		static DashboardItemExporter() {
			DevExpress.DashboardExport.DashboardBrickResolver.RegisterBrickFactory();
		}
		readonly DashboardExportMode mode;
		readonly DashboardItemExportData data;
		public virtual IDashboardItemFooterProvider DashboardItemFooterProvider { get { return null; } }
		protected DashboardExportMode Mode { get { return mode; } }
		protected DashboardItemServerData ServerData { get { return data.ServerData; } }
		protected ItemViewerClientState ClientState { get { return data.ViewerClientState; } }
		protected IList SelectedValues { get { return Mode == DashboardExportMode.EntireDashboard ? ServerData.SelectedValues : null; } }
		public abstract IPrintable PrintableComponent { get; }
		public virtual int ActualPrintWidth { get { return data.ViewerClientState.ViewerArea.Width; } }
		protected MultiDimensionalData CreateMultiDimensionalData() {
			HierarchicalMetadata hMetaData = ServerData.Metadata;
			MultidimensionalDataDTO multiDataDTO = ServerData.MultiDimensionalData;
			ClientHierarchicalMetadata metaData = new ClientHierarchicalMetadata(hMetaData);
			return new MultiDimensionalData(multiDataDTO.HierarchicalDataParams, metaData);
		}
		protected virtual IList DrillDownState { get { return ServerData.DrillDownState; } }
		protected virtual string[] TargetAxisNames { get { return new[] { DashboardDataAxisNames.DefaultAxis }; } }
		protected internal bool ShowHScroll { get; protected set; }
		protected internal bool ShowVScroll { get; protected set; }
		protected DashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data) {
			this.mode = mode;
			this.data = data;
			CalculateScrollBarSize();
			if(ClientState.VScrollingState != null && ClientState.VScrollingState.ScrollableAreaSize == 0)
				ClientState.VScrollingState.ScrollableAreaSize = ClientState.ViewerArea.Height;
			if(ClientState.HScrollingState != null && ClientState.HScrollingState.ScrollableAreaSize == 0)
				ClientState.HScrollingState.ScrollableAreaSize = ClientState.ViewerArea.Width;
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
		protected internal IDictionary<string, IList> GetDrillDownState() {
			if(DrillDownState != null && TargetAxisNames.Length == 1) {
				Dictionary<string, IList> drillDownState = new Dictionary<string, IList>();
				drillDownState.Add(TargetAxisNames[0], DrillDownState);
				return drillDownState;
			}
			return null;
		}
		protected internal virtual BorderSide GetBorders() {
			return ShowHScroll || ShowVScroll ? BorderSide.All : BorderSide.None;
		}
		protected virtual int GetViewerHCorrection() {
			return ShowVScroll ? ExportScrollBar.PrintSize : 0;
		}
		protected virtual int GetViewerVCorrection() {
			return ShowHScroll ? ExportScrollBar.PrintSize : 0;
		}
		protected internal virtual Rectangle GetViewerBounds() {
			ClientArea viewerArea = ClientState.ViewerArea;
			return new Rectangle(viewerArea.Left, viewerArea.Top, viewerArea.Width - GetViewerHCorrection(), viewerArea.Height - GetViewerVCorrection());
		}
		protected internal virtual void CalculateShowScrollbars() {
			ExportScrollController showScrollCalculator = new ExportScrollController(ClientState);
			showScrollCalculator.CalculateShowScrollbars(false, ClientState.ViewerArea.Width, true);
			ShowHScroll = showScrollCalculator.ShowHScroll;
			ShowVScroll = showScrollCalculator.ShowVScroll;
		}
		protected virtual void CalculateScrollBarSize(){
			if(ClientState.HScrollingState != null)
				ClientState.HScrollingState.ScrollBarSize = ExportScrollBar.PrintSize;
			if(ClientState.VScrollingState != null)
				ClientState.VScrollingState.ScrollBarSize = ExportScrollBar.PrintSize;
		}
	}
}
