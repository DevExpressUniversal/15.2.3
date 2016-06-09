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
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
namespace DevExpress.DashboardExport {
	public class EntireDashboardReportHolder : DashboardReportHolder {
		const int TitleHeight = 60;
		static readonly IEnumerable<string> scrollableItemTypes = 
			new List<string> { DashboardItemType.Card, DashboardItemType.Gauge, DashboardItemType.Pie };
		static Color BorderColor = Color.FromArgb(255, 191, 191, 191);
		readonly List<DashboardItemExporter> exporters = new List<DashboardItemExporter>();
		DashboardExportData data;
		static ClientArea ShiftArea(ClientArea area, Size shift) {
			return new ClientArea { Left = area.Left - shift.Width, Width = area.Width, Height = area.Height, Top = area.Top - shift.Height };
		}
		static ClientArea Join(ClientArea area1, ClientArea area2) {
			int area1Right = area1.Left + area1.Width;
			int area2Right = area2.Left + area2.Width;
			int area1Bottom = area1.Top + area1.Height;
			int area2Bottom = area2.Top + area2.Height;
			int left = area1.Left < area2.Left ? area1.Left : area2.Left;
			int top = area1.Top < area2.Top ? area1.Top : area2.Top;
			int right = area2Right > area1Right ? area2Right : area1Right;
			int bottom = area2Bottom > area1Bottom ? area2Bottom : area1Bottom;
			return new ClientArea {
				Left = left,
				Top = top,
				Width = right - left,
				Height = bottom - top
			};
		}
		internal EntireDashboardReportHolder(DashboardExportData data, DashboardReportOptions opts) :
			base(VerticalContentSplitting.Exact) {
			this.data = data;
			UpdateReport(opts);
		}
		void AddCaptionToDetailBand(DetailBand band, Rectangle captionBounds, string caption, IEnumerable<FormattableValue> filterValues, DashboardFontInfo fontInfo) {
			ExportItemCaption itemCaption = new ExportItemCaption(band, captionBounds, fontInfo);
			ItemCaptionViewControl itemCaptionViewControl = new ItemCaptionViewControl(itemCaption);
			itemCaptionViewControl.Update(caption, filterValues);
		}
		void AddScrollToDetailBand(DetailBand band, Rectangle scrollParentBounds, Rectangle scrollChildBounds, BorderSide borders) {
			XRPanel child = new XRPanel() {
				BackColor = BorderColor,
				BorderWidth = 0,
				KeepTogether = false,
				CanGrow = false
			};
			XRPanel parent = new XRPanel() {
				BorderWidth = 1,
				BorderColor = BorderColor,
				Borders = borders,
				KeepTogether = false,
				CanGrow = false
			};
			parent.Controls.Add(child);
			band.Controls.Add(parent);
			parent.BoundsF = scrollParentBounds;
			child.BoundsF = scrollChildBounds;
		}
		Rectangle CalculateHScroll(ScrollingState scrollingState, int clientSize, Size size) {
			return new Rectangle((int)(size.Width * scrollingState.PositionRatio), 0, (int)(size.Width * ((float)clientSize / (float)scrollingState.VirtualSize)), size.Height);
		}
		Rectangle CalculateVScroll(ScrollingState scrollingState, int clientSize, Size vScrollRect) {
			return new Rectangle(0, (int)(vScrollRect.Height * scrollingState.PositionRatio), vScrollRect.Width, (int)(vScrollRect.Height * ((float)scrollingState.ScrollableAreaSize / (float)scrollingState.VirtualSize)));
		}
		PrintableComponentContainer AddComponentContainerToDetailBand(IPrintable printableComponent, Rectangle bounds, BorderSide borders) {
			PrintableComponentContainer container = new PrintableComponentContainer {
				PrintableComponent = printableComponent,
				Borders = borders,
				BorderWidth = 1,
				BorderColor = BorderColor,
				ClipContent = true,
				KeepTogether = false
			};
			Band.Controls.Add(container);
			container.Bounds = bounds;
			return container;
		}
		void PatchClientSizes(DashboardExportData data, bool showTitle, int titleHeight) {
			if (data.ItemsData.Count == 0)
				return;
			ClientArea size = null;
			foreach (DashboardItemExportData itemData in data.ItemsData) {
				if (size == null)
					size = itemData.ViewerClientState.ViewerArea;
				else
					size = Join(size, itemData.ViewerClientState.ViewerArea);
				if (itemData.ViewerClientState.CaptionArea != null)
					size = Join(size, itemData.ViewerClientState.CaptionArea);
			}
			Size sizeShift = new Size { Width = size.Left, Height = size.Top };
			Size shitWithTitle = new Size { Width = sizeShift.Width, Height = sizeShift.Height - titleHeight };
			Size shift = showTitle ? shitWithTitle : sizeShift;
			foreach (DashboardItemExportData itemData in data.ItemsData) {
				itemData.ViewerClientState.ViewerArea = ShiftArea(itemData.ViewerClientState.ViewerArea, shift);
				if (itemData.ViewerClientState.CaptionArea != null) {
					itemData.ViewerClientState.CaptionArea = ShiftArea(itemData.ViewerClientState.CaptionArea, shift);
				}
			}
			data.ClientSize = new Size { Width = size.Width, Height = size.Height };
			if (showTitle)
				data.ClientSize = new Size { Width = data.ClientSize.Width, Height = data.ClientSize.Height + titleHeight };
		}
		void UpdateReport(DashboardReportOptions opts) {
			Band.Controls.Clear();
			DisposeExporters();
			PaperOptions paperOptions = opts.PageOptions.PaperOptions;
			DocumentContentOptions contentOptions = opts.DocumentContentOptions;
			if (paperOptions.UseCustomMargins)
				Report.Margins = paperOptions.CustomMargins;
			PatchClientSizes(data, contentOptions.ShowTitle, TitleHeight);
			if (contentOptions.ShowTitle) {
				Rectangle titleBounds = new Rectangle { Location = new Point(0, 0), Width = data.ClientSize.Width, Height = TitleHeight };
				ExportDashboardTitle title = new ExportDashboardTitle(Band, FontHelper.GetFont(ExportHelper.DefaultTitleFont, data.FontInfo), TextAlignment.BottomLeft, titleBounds);
				DashboardTitleViewModel viewModel = new DashboardTitleViewModel {
					Text = contentOptions.Title,
					IncludeMasterFilterValues = false,
					LayoutModel = data.TitleLayoutModel
				};
				title.Initialize(viewModel);
			}
			CreateItemExporters();
			PointF blockLocation = new PointF(0, data.ClientSize.Height + (contentOptions.ShowTitle ? TitleHeight : 0));
			List<DimensionFilterValues> masterFilterValues = new List<DimensionFilterValues>();
			if (data.MasterFilterValues != null)
				masterFilterValues.AddRange(data.MasterFilterValues);
			AddSeparateFilterBlock(Band, blockLocation, data.ClientSize.Width, contentOptions.FilterStatePresentation, masterFilterValues, data.FontInfo);
			PrepareReport(Report, DashboardExportMode.EntireDashboard, data.ClientSize, opts);
		}
		void CreateItemExporters() {
			foreach (DashboardItemExportData itemData in data.ItemsData) {
				ItemViewerClientState clientState = itemData.ViewerClientState;
				ClientArea viewerArea = clientState.ViewerArea;
				ClientArea captionArea = clientState.CaptionArea;
				itemData.FontInfo = data.FontInfo;
				DashboardItemExporter exporter = DashboardItemExporter.CreateExporter(DashboardExportMode.EntireDashboard, itemData, null);
				exporter.CalculateShowScrollbars();
				Rectangle viewerBounds = exporter.GetViewerBounds();
				DashboardItemViewModel viewModel = itemData.ServerData.ViewModel;
				exporters.Add(exporter);
				if (viewModel.ShowCaption) {
					Rectangle captionRect = new Rectangle(captionArea.Left, captionArea.Top, viewerBounds.Width, captionArea.Height);
					if(viewModel.ShowCaption) {
						IEnumerable<DimensionFilterValues> filterValues = itemData.ServerData.DrillDownValues;
						IList<FormattableValue> formattableValues = filterValues != null ? filterValues.SelectMany(values => values.Values).ToList() : null;
						AddCaptionToDetailBand(Band, captionRect, viewModel.Caption, formattableValues, data.FontInfo);
					}
				}
				if (exporter.ShowVScroll) {
					Rectangle vScrollRect = new Rectangle(viewerBounds.Right, viewerBounds.Y, ExportScrollBar.PrintSize, viewerBounds.Height);
					Rectangle vScrollBounds = CalculateVScroll(clientState.VScrollingState, clientState.ViewerArea.Height, vScrollRect.Size);
					AddScrollToDetailBand(Band, vScrollRect, vScrollBounds, BorderSide.Top | BorderSide.Right | BorderSide.Bottom);
				}
				IDashboardItemFooterProvider footerProvider = exporter.DashboardItemFooterProvider;
				bool showFooter = footerProvider != null && footerProvider.ShowFooter;
				if (showFooter) {
					PointF footerLocation = new Point(viewerArea.Left, viewerArea.Top + viewerArea.Height);
					footerProvider.AddFooterToDetailBand(Band, footerLocation, viewerArea.Top);
					viewerBounds.Height += footerProvider.FooterHeight;
				}
				if (exporter.ShowHScroll) {
					int hScrollWidth = showFooter ? viewerBounds.Width + ExportScrollBar.PrintSize : viewerBounds.Width;
					Rectangle hScrollRect = new Rectangle(viewerBounds.X, viewerBounds.Bottom, hScrollWidth, ExportScrollBar.PrintSize);
					Rectangle hScrollBounds = CalculateHScroll(clientState.HScrollingState, clientState.ViewerArea.Width, hScrollRect.Size);
					AddScrollToDetailBand(Band, hScrollRect, hScrollBounds, BorderSide.Left | BorderSide.Bottom | BorderSide.Right);
				}
				PrintableComponentContainer container = AddComponentContainerToDetailBand(exporter.PrintableComponent, viewerBounds, exporter.GetBorders());
				if (scrollableItemTypes.Contains(itemData.ServerData.Type)) {
					ScrollingState hScrollState = itemData.ViewerClientState.HScrollingState;
					ScrollingState vScrollState = itemData.ViewerClientState.VScrollingState;
					double scrollLeft = hScrollState != null ? (-1) * hScrollState.PositionRatio * hScrollState.VirtualSize : 0;
					double scrollTop = vScrollState != null ? (-1) * vScrollState.PositionRatio * vScrollState.VirtualSize : 0;
					container.PrintableContentOffset = new SizeF((float)scrollLeft, (float)scrollTop);
				}
			}
		}
		protected override void UpdateReportState(ExportInfo exportInfo) {
			DashboardServiceOperationHelper.UpdateViewerState(data, exportInfo.ViewerState);
			UpdateReport(exportInfo.ExportOptions);
		}
		protected override void DisposeExporters() {
			foreach (DashboardItemExporter exporter in exporters)
				exporter.Dispose();
			exporters.Clear();
		}
	}
}
