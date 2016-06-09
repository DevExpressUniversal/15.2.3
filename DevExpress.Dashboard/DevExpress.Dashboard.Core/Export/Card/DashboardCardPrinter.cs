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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DashboardExport {
	public class DashboardCardPrinter : DashboardExportPrinter {
		internal const int SelectedIndent = 4;
		static Color CardBorderColor = Color.FromArgb(255, 191, 191, 191);
		static int CardBorderWidth = 1;
		internal static void RecalculateSelectedBrickBounds(IPanelBrick selectedBrick, Rectangle cardBounds) {
			Point childLocation = new Point();
			Size cardSize = cardBounds.Size;
			cardBounds.Width += SelectedIndent;
			cardBounds.Height += SelectedIndent;
			if(cardBounds.X > SelectedIndent) {
				childLocation.X = SelectedIndent;
				cardBounds.Width += SelectedIndent;
				cardBounds.X -= SelectedIndent;
			}
			else {
				childLocation.X = cardBounds.X;
				cardBounds.Width += cardBounds.X;
				cardBounds.X = 0;
			}
			if(cardBounds.Y > SelectedIndent) {
				childLocation.Y = SelectedIndent;
				cardBounds.Height += SelectedIndent;
				cardBounds.Y -= SelectedIndent;
			}
			else {
				childLocation.Y = cardBounds.Y;
				cardBounds.Height += cardBounds.Y;
				cardBounds.Y = 0;
			}
			IPanelBrick child = ((IPanelBrick)selectedBrick.Bricks[0]);
			child.Rect = new Rectangle(childLocation, cardSize);
			selectedBrick.Rect = cardBounds;
		}
		readonly CardDashboardItemViewModel viewModel;
		readonly MultiDimensionalData data;
		readonly CardContainerControlModel cardContainer;
		readonly ItemViewerClientState clientState;
		readonly bool useClientScrollState;
		readonly IList selectedValues;
		readonly bool increaseClientHeightToVirtualHeight;
		readonly DashboardSparklinePainterCore sparklinePainter = new DashboardSparklinePainterCore();
		readonly DashboardFontInfo fontInfo = new DashboardFontInfo();
		readonly bool drilledDown;
		Point clientOffset;
		Size cardMargin;
		Size cardAdditionalMargin;
		public DashboardCardPrinter(CardContainerControlModel cardContainerControlModel, CardDashboardItemViewModel viewModel, MultiDimensionalData data, ItemViewerClientState clientState, bool useClientScrollState, IList selectedValues, Size cardMargin, Size cardAdditionalMargin, bool increaseClientHeightToVirtualHeight, DashboardFontInfo fontInfo, bool drilledDown) {
			this.viewModel = viewModel;
			this.data = data;
			this.cardContainer = cardContainerControlModel;
			this.clientState = clientState;
			this.useClientScrollState = useClientScrollState;
			this.selectedValues = selectedValues;
			this.cardMargin = cardMargin;
			this.cardAdditionalMargin = cardAdditionalMargin;
			this.increaseClientHeightToVirtualHeight = increaseClientHeightToVirtualHeight;
			this.fontInfo = fontInfo;
			this.drilledDown = drilledDown;
		}
		void PrepareCardContainer(BrickGraphics graphics, CardStyleProperties styleProperties, bool drilledDown) {
			cardContainer.UpdateVisualState(styleProperties, GraphicsDpi.UnitToDpi(graphics.PageUnit));
			ExportContentScrollableControl exportContentScrollableControl = new ExportContentScrollableControl(cardContainer, viewModel.ContentDescription, clientState, useClientScrollState);
			exportContentScrollableControl.ContentScrollableControlModel.ContentArrangementOptions = viewModel.ShowCaption ? ContentArrangementOptions.Default : ContentArrangementOptions.IgnoreMargins;
			cardContainer.Update(viewModel, data, drilledDown);
			if(increaseClientHeightToVirtualHeight) {
				clientState.ViewerArea.Height = exportContentScrollableControl.ContentScrollableControlModel.VirtualSize.Height;
				exportContentScrollableControl.UpdateClientState(clientState);
			}
			if(useClientScrollState) {
				ScrollingState vScrollState = clientState.VScrollingState;
				ScrollingState hScrollState = clientState.HScrollingState;
				int vOffset = vScrollState != null ? (int)(vScrollState.PositionRatio * exportContentScrollableControl.ContentScrollableControlModel.VirtualSize.Height) : 0;
				int hOffset = hScrollState != null ? (int)(hScrollState.PositionRatio * exportContentScrollableControl.ContentScrollableControlModel.VirtualSize.Width) : 0;
				clientOffset = new Point(hOffset, vOffset);
			}
		}
		CardStyleProperties CreateCardStyleProperties() {
			CardStyleProperties styleProperties = new CardStyleProperties() { Margin = cardMargin };
			styleProperties.UpdateCardProportions(viewModel);
			return styleProperties;
		}
		protected override void CreateDetail(IBrickGraphics graph) {
			BrickGraphics graphics = (BrickGraphics)graph;
			CardStyleProperties styleProperties = CreateCardStyleProperties();
			PrepareCardContainer(graphics, styleProperties, drilledDown);
			CardPresenter cardPresenter = new CardPresenter();
			cardPresenter.Initialize(styleProperties);
			Rectangle clientBounds = new Rectangle(clientOffset, cardContainer.Size);
			try {
				foreach(CardModel card in cardContainer.Items) {
					if(card.CheckBounds(clientBounds)) {
						Rectangle panelBrickBounds = FullBoundsToVisible(card.Bounds);
						IPanelBrick panelBrick = PS.CreatePanelBrick();
						panelBrick.BorderWidth = CardBorderWidth;
						panelBrick.BorderColor = CardBorderColor;
						CardViewModel cardViewModel = card.ViewModel;
						bool isSparklineMode = cardContainer.HasSeriesDimensions ? cardContainer.IsSparklineMode && cardViewModel.ShowSparkline : cardContainer.IsSparklineMode && cardContainer.HasAtLeastOneSparkline;
						Rectangle bounds = isSparklineMode ? card.CardPanelBounds : card.Bounds;
						Rectangle cardPanelBounds = RecalculateContentBounds(bounds);
						IDrawPropertiesContainer drawProperties = cardPresenter.GetDrawPropertiesCollection(new ExportCardStringMeasurer(graphics), cardPanelBounds, card, ExportHelper.DefaultFontFamily);
						foreach(DrawProperties draw in drawProperties.GetDrawProperties) {
							TextDrawProperties text = draw as TextDrawProperties;
							if(text != null)
								panelBrick.Bricks.Add(GetTextBrick(text));
							ImageDrawProperties image = draw as ImageDrawProperties;
							if(image != null)
								panelBrick.Bricks.Add(GetImageBrick(image));
						}
						if(cardContainer.IsSparklineMode && cardViewModel.ShowSparkline) {
							Padding padding = DefaultCardMeasurements.DefaultContentPadding;
							Rectangle sparklineBounds = new Rectangle(padding.Left, cardPanelBounds.Bottom,
								panelBrickBounds.Width - padding.Left - padding.Right, panelBrickBounds.Height - cardPanelBounds.Height - padding.Bottom);
							IImageBrick sparklineBrick = PS.CreateImageBrick();
							sparklineBrick.Rect = sparklineBounds;
							sparklineBrick.BackColor = Color.Empty;
							sparklineBrick.BorderWidth = 0;
							Bitmap image = new Bitmap(sparklineBounds.Width, sparklineBounds.Height);
							using(DashboardSparklineInfo info = new DashboardSparklineInfo(cardViewModel.SparklineOptions, card.Data.SparklineValues,
								new Rectangle(0, 0, sparklineBounds.Width, sparklineBounds.Height), new GraphicsCache(Graphics.FromImage(image)), null)) {
								sparklinePainter.Draw(info);
							}
							sparklineBrick.Image = new Bitmap(image);
							panelBrick.Bricks.Add(sparklineBrick);
						}
						if(DataUtils.ListContains(selectedValues, ((IValuesProvider)card).SelectionValues)) {
							IPanelBrick selectedBrick = PS.CreatePanelBrick();
							selectedBrick.BorderWidth = CardBorderWidth;
							selectedBrick.Bricks.Add(panelBrick);
							selectedBrick.BackColor = ExportContentScrollableControl.SelectedBackColor;
							panelBrick.BackColor = ExportContentScrollableControl.SelectedBackColor;
							foreach(IVisualBrick brick in panelBrick.Bricks)
								brick.BackColor = ExportContentScrollableControl.SelectedBackColor;
							RecalculateSelectedBrickBounds(selectedBrick, panelBrickBounds);
							graph.DrawBrick(selectedBrick, selectedBrick.Rect);
						}
						else
							graph.DrawBrick(panelBrick, panelBrickBounds);
					}
				}
			}
			catch { }
		}
		Rectangle RecalculateContentBounds(Rectangle bound) {
			Padding padding = DefaultCardMeasurements.DefaultContentPadding;
			return new Rectangle(padding.Left, padding.Top, bound.Width - padding.Left - padding.Right, bound.Height - padding.Top - padding.Bottom);
		}
		Rectangle FullBoundsToVisible(Rectangle fullBounds) {
			Rectangle rect = Rectangle.Inflate(fullBounds, -cardAdditionalMargin.Width, -cardAdditionalMargin.Height);
			return new Rectangle(rect.Location, new Size(rect.Width - CardBorderWidth, rect.Height - CardBorderWidth));
		}
		Rectangle VisibleBoundsToFull(Rectangle visibleBounds) {
			return new Rectangle(Point.Subtract(visibleBounds.Location, cardAdditionalMargin), visibleBounds.Size);
		}
		ITextBrick GetTextBrick(TextDrawProperties text) {
			ITextBrick brick = PS.CreateTextBrick();
			brick.Font = FontHelper.GetFont(text.Font, fontInfo);
			brick.StringFormat = new BrickStringFormat(text.Format);
			brick.StringFormat.PrototypeKind = BrickStringFormatPrototypeKind.GenericTypographic;
			brick.Text = text.Text;
			brick.BorderWidth = 0;
			brick.Rect = VisibleBoundsToFull(text.Bounds);
			brick.ForeColor = text.Color;
			return brick;
		}
		IImageBrick GetImageBrick(ImageDrawProperties image) {
			IImageBrick brick = PS.CreateImageBrick();
			brick.Image = image.Image;
			brick.BorderWidth = 0;
			brick.Rect = VisibleBoundsToFull(image.Bounds);
			return brick;
		}
	}
	public class ExportCardStringMeasurer : ICardStringMeasurer {
		readonly BrickGraphics graph;
		public ExportCardStringMeasurer(BrickGraphics graph) {
			this.graph = graph;
		}
		public int GetStringHeight(string text, Font font, int width, StringFormat format) {
			SizeF size = Measurement.MeasureString(text, font, Int32.MaxValue, format, graph.PageUnit);
			return (int)size.Height;
		}
		public bool IsStringFit(string text, Font font, int width, StringFormat format) {
			SizeF size = Measurement.MeasureString(text, font, Int32.MaxValue, format, graph.PageUnit);
			return ((int)size.Width) <= width;
		}
	}
}
