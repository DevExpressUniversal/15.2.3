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
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
namespace DevExpress.DashboardExport {
	public class SingleDashboardItemReportHolder : DashboardReportHolder {
		const int ItemTitleHeight = 40;
		DashboardItemExportData data;
		DashboardItemExporter exporter;
		static ContentDescriptionViewModel GetContentDescription(DashboardItemServerData data) {
			switch (data.Type) {
				case DashboardItemType.Card:
					return ((CardDashboardItemViewModel)data.ViewModel).ContentDescription;
				case DashboardItemType.Pie:
					return ((PieDashboardItemViewModel)data.ViewModel).ContentDescription;
				case DashboardItemType.Gauge:
					return ((GaugeDashboardItemViewModel)data.ViewModel).ContentDescription;
				default:
					throw new NotSupportedException();
			}
		}
		static Size CorrectPageSizeWithTitleHeight(Size pageSize) {
			return new Size(pageSize.Width, pageSize.Height - ItemTitleHeight);
		}
		static void ApplyAutomaticPageLayout(ClientArea viewerArea, AutomaticPageOptions autoOptions, PaperOptions paperOptions) {
			if (autoOptions.AutoRotate)
				paperOptions.Landscape = CalculateAutoLayoutLandscape(viewerArea);
		}
		static bool CalculateAutoLayoutLandscape(ClientArea area) {
			return (float)area.Width > (float)area.Height;
		}
		static void ApplySizeMode(ItemContentOptions itemOptions, Size clientSize, ItemViewerClientState clientState) {
			switch (itemOptions.SizeMode) {
				case ItemSizeMode.Zoom:
					PerformZoom(clientState.ViewerArea, clientSize);
					break;
				case ItemSizeMode.Stretch:
					PerformStretch(clientState.ViewerArea, clientSize);
					break;
				case ItemSizeMode.FitWidth:
					clientState.ViewerArea.Width = clientSize.Width;
					break;
				default:
					break;
			}
		}
		static void PerformStretch(ClientArea viewerArea, Size clientSize) {
			viewerArea.Width = clientSize.Width;
			viewerArea.Height = clientSize.Height;
		}
		static void PerformZoom(ClientArea area, Size pageSize) {
			float pageWidth = (float)pageSize.Width;
			float pageHeight = (float)pageSize.Height;
			float areaWidth = (float)area.Width;
			float areaHeight = (float)area.Height;
			float pageRatio = pageHeight / pageWidth;
			float areaRatio = areaHeight / areaWidth;
			float zoomRatio = pageRatio > areaRatio ? pageWidth / areaWidth : pageHeight / areaHeight;
			SizeF zoomedArea = new SizeF { Width = areaWidth * zoomRatio, Height = areaHeight * zoomRatio };
			Size correctedArea = Size.Truncate(zoomedArea);
			area.Width = correctedArea.Width;
			area.Height = correctedArea.Height;
		}
		static void ApplyAutoArrangeContent(DashboardItemExportData data, ItemContentOptions itemOptions, Size pageSize, ItemViewerClientState clientState) {
			string type = data.ServerData.Type;
			if (type == DashboardItemType.Gauge || type == DashboardItemType.Card || type == DashboardItemType.Pie) {
				if (itemOptions.ArrangerOptions.AutoArrangeContent) {
					clientState.ViewerArea.Width = pageSize.Width;
					clientState.ViewerArea.Height = pageSize.Height;
					clientState.HScrollingState = null;
					clientState.VScrollingState = null;
					GetContentDescription(data.ServerData).ArrangementMode = ContentArrangementMode.Auto;
				}
			}
		}
		internal SingleDashboardItemReportHolder(DashboardItemExportData data, DashboardReportOptions options): base(VerticalContentSplitting.Smart) {
			this.data = data;   
			UpdateReport(options);
		}
		void UpdateReport(DashboardReportOptions opts) {
			Band.Controls.Clear();
			DisposeExporters();
			PaperOptions paperOptions = opts.PageOptions.PaperOptions;
			DocumentContentOptions contentOptions = opts.DocumentContentOptions;
			bool showTitle = contentOptions.ShowTitle;
			Size pageSize = Size.Empty;
			if (paperOptions.UseCustomMargins)
				Report.Margins = paperOptions.CustomMargins;
			if (opts.FormatOptions.Format == DashboardExportFormat.PDF) {
				pageSize = ExportHelper.GetPageClientSize(paperOptions.PaperKind, Report.Margins, paperOptions.Landscape);
				Size currentSize = pageSize;
				Size rotatedSize = new Size(pageSize.Height, pageSize.Width);
				Size clientSizePortrait = paperOptions.Landscape ? rotatedSize : currentSize;
				Size clientSizeLandscape = paperOptions.Landscape ? currentSize : rotatedSize;
				if (showTitle) {
					clientSizePortrait = CorrectPageSizeWithTitleHeight(clientSizePortrait);
					clientSizeLandscape = CorrectPageSizeWithTitleHeight(clientSizeLandscape);
				}
				pageSize = PrepareSpecificOptions(data, opts, clientSizePortrait, clientSizeLandscape);
			}
			this.exporter = DashboardItemExporter.CreateExporter(DashboardExportMode.SingleItem, data, opts);
			if (opts.FormatOptions.Format == DashboardExportFormat.Image) {
				ClientArea viewerArea = data.ViewerClientState.ViewerArea;
				pageSize = new Size(this.exporter.ActualPrintWidth, viewerArea.Height + (showTitle ? ItemTitleHeight : 0));
			}
			if (showTitle) {
				ExportDashboardTitle title = new ExportDashboardTitle(Band, FontHelper.GetFont(ExportHelper.DefaultCaptionFont, data.FontInfo), TextAlignment.MiddleLeft, new Rectangle(0, 0, pageSize.Width, ItemTitleHeight - 1));
				title.Initialize(new DashboardTitleViewModel { Text = contentOptions.Title });
			}
			PrintableComponentContainer container = new PrintableComponentContainer {
				PrintableComponent = exporter.PrintableComponent,
				ClipContent = false
			};
			container.Location = new Point(0, showTitle ? ItemTitleHeight + 1 : 0);
			container.HeightF = 10;
			Band.Controls.Add(container);
			IDashboardItemFooterProvider footerProvider = exporter.DashboardItemFooterProvider;
			if (footerProvider != null && footerProvider.ShowFooter) {
				RectangleF bounds = container.BoundsF;
				PointF footerLocation = new PointF(bounds.Left, bounds.Bottom);
				footerProvider.AddFooterToDetailBand(Band, footerLocation, bounds.Bottom);
			}
			IList<DimensionFilterValues> filterValues = data.ServerData.FilterValues;
			if(data.ServerData.DrillDownValues != null)
				filterValues = filterValues.Concat(data.ServerData.DrillDownValues).ToList();
			AddSeparateFilterBlock(Band, new PointF(0, container.BoundsF.Bottom + 5), pageSize.Width, contentOptions.FilterStatePresentation, filterValues, data.FontInfo);
			PrepareReport(Report, DashboardExportMode.SingleItem, Size.Empty, opts);
		}
		Size PrepareSpecificOptions(DashboardItemExportData data, DashboardReportOptions opts, Size clientSizePortrait, Size clientSizeLandscape) {
			ItemContentOptions itemOptions = opts.ItemContentOptions;
			PaperOptions paperOptions = opts.PageOptions.PaperOptions;
			AutomaticPageOptions autoOptions = opts.AutoPageOptions;
			ItemViewerClientState clientState = data.ViewerClientState;
			ApplyAutomaticPageLayout(clientState.ViewerArea, autoOptions, paperOptions);
			Size clientSize = paperOptions.Landscape ? clientSizeLandscape : clientSizePortrait;
			ApplySizeMode(itemOptions, clientSize, clientState);
			ApplyAutoArrangeContent(data, itemOptions, clientSize, clientState);
			return clientSize;
		}
		protected override void DisposeExporters() {
			if (exporter != null)
				exporter.Dispose();
		}
		protected override void UpdateReportState(ExportInfo exportInfo) {
			DashboardServiceOperationHelper.UpdateItemViewerState(data, exportInfo.ViewerState.ItemsState[data.Name]);
			UpdateReport(exportInfo.ExportOptions);
		}
	}
}
