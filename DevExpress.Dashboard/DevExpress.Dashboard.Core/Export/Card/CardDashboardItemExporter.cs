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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraPrinting;
namespace DevExpress.DashboardExport {
	public class CardDashboardItemExporter : DashboardItemExporter, IDashboardCardControl {
		readonly CardContainerControlModel cardContainer;
		readonly DashboardCardPrinter printer;
		readonly bool showCaption;
		public override IPrintable PrintableComponent {
			get { return printer; }
		}
		public CardDashboardItemExporter(DashboardExportMode mode, DashboardItemExportData data, DashboardReportOptions opts)
			: base(mode, data) {
			this.showCaption = data.ServerData.ViewModel.ShowCaption;
			bool increaseClientHeightToVirtualHeight = false;
			if(HasItemContentOptions(opts) && opts.ItemContentOptions.ArrangerOptions != null) {
				increaseClientHeightToVirtualHeight = opts.ItemContentOptions.ArrangerOptions.AutoArrangeContent;
			}
			cardContainer = new CardContainerControlModel(this);
			Size cardMargin = new Size(DefaultCardMeasurements.DefaultMarginX, DefaultCardMeasurements.DefaultMarginY);
			Size cardAdditionalMargin = Size.Empty;
			if(ClientState.SpecificState != null) {
				if(ClientState.SpecificState.ContainsKey("CardMarginX") && ClientState.SpecificState.ContainsKey("CardMarginY"))
					cardMargin = new Size((int)ClientState.SpecificState["CardMarginX"], (int)ClientState.SpecificState["CardMarginY"]);
				if(ClientState.SpecificState.ContainsKey("CardAdditionalMarginX") && ClientState.SpecificState.ContainsKey("CardAdditionalMarginY"))
					cardAdditionalMargin = new Size((int)ClientState.SpecificState["CardAdditionalMarginX"], (int)ClientState.SpecificState["CardAdditionalMarginY"]);
			}
			DashboardFontInfo fontInfo;
			if(FontHelper.HasValue(data.FontInfo))
				fontInfo = data.FontInfo;
			else
				fontInfo = new DashboardFontInfo();
			printer = new DashboardCardPrinter(cardContainer, (CardDashboardItemViewModel)ServerData.ViewModel, CreateMultiDimensionalData(), ClientState, mode == DashboardExportMode.EntireDashboard, SelectedValues, cardMargin, cardAdditionalMargin, increaseClientHeightToVirtualHeight, fontInfo, DrillDownState != null);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				printer.Dispose();
			base.Dispose(disposing);
		}
		void IDashboardCardControl.BeginUpdate() {
		}
		void IDashboardCardControl.EndUpdate() {
		}
		void IDashboardCardControl.SetSize(Size size) {
		}
		protected internal override BorderSide GetBorders() {
			if(showCaption)
				return base.GetBorders();		  
			else
				return (ShowHScroll ? BorderSide.Bottom : BorderSide.None) | (ShowVScroll ? BorderSide.Right : BorderSide.None);
		}
		protected override int GetViewerHCorrection() {
			if(ShowVScroll)
				return showCaption ? base.GetViewerHCorrection() : ClientState.VScrollingState.ScrollBarSize;
			return 0;
		}
		protected override int GetViewerVCorrection() {
			if(ShowHScroll)
				return showCaption ? base.GetViewerVCorrection() : ClientState.HScrollingState.ScrollBarSize;
			return 0;
		}
		protected internal override Rectangle GetViewerBounds() {
			ClientArea viewerArea = ClientState.ViewerArea;
			Rectangle viewerBounds = base.GetViewerBounds();
			if(!showCaption) {
				Padding margin = DefaultCardMeasurements.DefaultDashboardItemMargin;
				Point newLocation = Point.Add(viewerBounds.Location, new Size(margin.Left, margin.Top));
				return new Rectangle(newLocation, viewerBounds.Size);
			}
			return viewerBounds;
		}
		protected override void CalculateScrollBarSize() {
			if(showCaption)
				base.CalculateScrollBarSize();
		}
		protected internal override void CalculateShowScrollbars() {
			ExportScrollController showScrollCalculator = new ExportScrollController(ClientState);
			showScrollCalculator.CalculateShowScrollbars(!showCaption, ClientState.ViewerArea.Width, true);
			ShowHScroll = showScrollCalculator.ShowHScroll;
			ShowVScroll = showScrollCalculator.ShowVScroll;
		}
	}
}
