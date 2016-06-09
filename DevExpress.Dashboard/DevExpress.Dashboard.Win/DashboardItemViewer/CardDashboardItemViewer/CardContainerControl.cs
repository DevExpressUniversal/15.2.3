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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Data;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class CardContainerControl : XtraUserControl, IDashboardCardControl, IWinContentProvider, IMouseWheelSupport, IToolTipControlClient {
		readonly CardDrawingContext drawingContext;
		readonly CardPresenter cardPresenter = new CardPresenter();
		readonly DashboardSparklinePainterCore sparklinePainter = new DashboardSparklinePainterCore();
		readonly CardContainerControlModel model;
		readonly ToolTipController tooltipController;
		IContentProvider IWinContentProvider.ContentProvider { get { return model; } }
		Control IWinContentProvider.ContentControl { get { return this; } }
		public ToolTipController ToolTipController { get { return tooltipController; } }
		public CardDrawingContext DrawingContext { get { return drawingContext; } }
		public CardContainerControlModel Model { get { return model; } }
		public bool IsDataReduced { get { return model.IsDataReduced; } }
		event EventHandler<PaintEventArgs> painted;
		public event EventHandler<PaintEventArgs> Painted {
			add { painted = (EventHandler<PaintEventArgs>)Delegate.Combine(painted, value); }
			remove { painted = (EventHandler<PaintEventArgs>)Delegate.Remove(painted, value); }
		}
		public CardContainerControl() {
			model = new CardContainerControlModel(this);
			LookAndFeel.StyleChanged += (sender, e) => {
				OnLookAndFeelChanged();
			};
			tooltipController = new ToolTipController();
			tooltipController.AddClientControl(this);
			drawingContext = new CardDrawingContext(LookAndFeel);
			UpdateVisualStateBase(drawingContext.ItemPainter.StyleProperties);
			SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
		}
		internal void OnLookAndFeelChanged() {
			drawingContext.Update(LookAndFeel);
			model.RaiseChanged(ContentProviderChangeReason.ItemProperties);
		}
		public void Update(CardDashboardItemViewModel viewModel, MultiDimensionalData data, bool drilledDown) {
			Update(viewModel, data, drilledDown, false);
		}
		public void Update(CardDashboardItemViewModel viewModel, MultiDimensionalData data, bool drilledDown, bool isDesignMode) {
			model.Update(viewModel, data, drilledDown, isDesignMode);
		}
		public void UpdateVisualState(CardDashboardItemViewModel viewModel) {
			CardStyleProperties styleProperties = drawingContext.ItemPainter.StyleProperties;
			styleProperties.UpdateCardProportions(viewModel);
			UpdateVisualStateBase(styleProperties);
		}
		void UpdateVisualStateBase(CardStyleProperties styleProperties) {
			using(Graphics graphics = CreateGraphics())
				model.UpdateVisualState(styleProperties, graphics.DpiX);
			cardPresenter.Initialize(styleProperties);
		}
		Rectangle GetElementBounds(DrawProperties properties) {
			TextDrawProperties text = properties as TextDrawProperties;
			Rectangle rect = Rectangle.Empty;
			if(text != null) {
				rect = text.Bounds;
				IDrawPropertiesContainer drawPropertiesContainer = (IDrawPropertiesContainer)text;
				foreach(TextDrawProperties internalProperties in drawPropertiesContainer.GetDrawProperties) {
					rect = Rectangle.Union(rect, internalProperties.Bounds);
				}
			}
			return rect;
		}
		bool GetShowElementTooltip(DrawProperties properties, CardStringMeasurer measurer) {
			TextDrawProperties text = properties as TextDrawProperties;
			if(text != null) {
				IDrawPropertiesContainer drawPropertiesContainer = (IDrawPropertiesContainer)text;
				foreach(TextDrawProperties internalProperties in drawPropertiesContainer.GetDrawProperties) {
					if(!measurer.IsStringFit(internalProperties.Text, internalProperties.Font, internalProperties.Bounds.Width, internalProperties.Format)) {
						return true;
					}
				}
			}
			return false;
		}
		void UpdateCardTooltipInfo(CardDrawProperties drawProperties, CardStringMeasurer measurer, CardModel card) {
			CardViewInfo viewInfo = card.ViewInfo;
			viewInfo.TitleBounds = GetElementBounds(drawProperties.TitleProperties);
			viewInfo.ShowTitleTooltip = GetShowElementTooltip(drawProperties.TitleProperties, measurer);
			viewInfo.SubTitleBounds = GetElementBounds(drawProperties.SubTitleProperties);
			viewInfo.ShowSubTitleTooltip = GetShowElementTooltip(drawProperties.SubTitleProperties, measurer);
			viewInfo.SubValue1Bounds = GetElementBounds(drawProperties.SubValue1Properties);
			viewInfo.ShowSubValue1Tooltip = GetShowElementTooltip(drawProperties.SubValue1Properties, measurer);
			viewInfo.SubValue2Bounds = GetElementBounds(drawProperties.SubValue2Properties);
			viewInfo.ShowSubValue2Tooltip = GetShowElementTooltip(drawProperties.SubValue2Properties, measurer);
			viewInfo.MainValueBounds = GetElementBounds(drawProperties.MainValueProperties);
			viewInfo.ShowMainValueTooltip = GetShowElementTooltip(drawProperties.MainValueProperties, measurer);
		}
		IDrawPropertiesContainer MeasureCard(GraphicsCache cache, CardModel card, Rectangle clientRectangle) {
			CardStringMeasurer measurer = new CardStringMeasurer(cache);
			CardDrawProperties drawProperties = cardPresenter.GetDrawPropertiesCollection(measurer, clientRectangle, card);
			UpdateCardTooltipInfo(drawProperties, measurer, card);
			return drawProperties;
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				Rectangle clientBounds = new Rectangle(new Point(0, 0), Size);
				foreach(CardModel card in model.Items) {
					CardViewModel cardViewModel = card.ViewModel;
					if(card.CheckBounds(clientBounds)) {
						drawingContext.DrawObject(cache, card.Bounds);
						bool isSparklineMode = model.IsSparklineMode && (model.HasSeriesDimensions ? cardViewModel.ShowSparkline : model.HasAtLeastOneSparkline);
						Rectangle bounds = isSparklineMode ? card.CardPanelBounds : card.Bounds;
						Rectangle clientRectangle = drawingContext.GetObjectClientRectangle(cache, bounds);
						IDrawPropertiesContainer drawProperties = MeasureCard(cache, card, clientRectangle);
						CardPainter.Draw(drawingContext.Appearances.ItemAppearance, cache, drawProperties, cardPresenter, clientRectangle, card);
						if(model.IsSparklineMode && cardViewModel.ShowSparkline) {
							Rectangle sparklineBounds = new Rectangle(clientRectangle.X, clientRectangle.Bottom + DefaultCardMeasurements.DefaultDashboardCardSparklineTopIndent,
								clientRectangle.Width, card.CardSparklineBounds.Height - DefaultCardMeasurements.DefaultDashboardCardSparklineTopIndent);
							card.ViewInfo.SparklineBounds = sparklineBounds;
							sparklinePainter.Draw(new DashboardSparklineInfo(cardViewModel.SparklineOptions, card.Data.SparklineValues, sparklineBounds, cache, LookAndFeel));
						}
					}
				}
			}
			if(painted != null)
				painted(this, e);
			base.RaisePaintEvent(this, e);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				drawingContext.Dispose();
				cardPresenter.Dispose();
				tooltipController.Dispose();
			}
		}
		void IDashboardCardControl.BeginUpdate() {
		}
		void IDashboardCardControl.EndUpdate() {
			Invalidate();
		}
		void IDashboardCardControl.SetSize(Size size) {
			Size = size;
		}
		protected sealed override void OnMouseWheel(MouseEventArgs e) {
			if(XtraForm.ProcessSmartMouseWheel(this, e)) return;
			OnMouseWheelCore(e);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		void OnMouseWheelCore(MouseEventArgs e) {
			if(!ControlHelper.IsHMouseWheel(e))
				base.OnMouseWheel(e);
		}
		CardModel GetHitTestInfo(Point point) {
			foreach(CardModel card in model.Items)
				if(card.Bounds.Contains(point))
					return card;
			return null;
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			CardModel card = GetHitTestInfo(point);
			if(card != null) {
				string cardId = card.Bounds.ToString();
				ToolTipControlInfo tooltipInfo = null;
				CardViewInfo cardViewInfo = card.ViewInfo;
				tooltipInfo = cardViewInfo.ShowTitleTooltip ? GetToolTipInfo(point, cardViewInfo.TitleBounds, card.Title, cardId, "Title") : null;
				if(tooltipInfo != null)
					return tooltipInfo;
				tooltipInfo = cardViewInfo.ShowSubTitleTooltip ? GetToolTipInfo(point, cardViewInfo.SubTitleBounds, card.SubTitle, cardId, "SubTitle") : null;
				if(tooltipInfo != null)
					return tooltipInfo;
				tooltipInfo = cardViewInfo.ShowSubValue1Tooltip ? GetToolTipInfo(point, cardViewInfo.SubValue1Bounds, card.SubValue1, cardId, "SubValue1") : null;
				if(tooltipInfo != null)
					return tooltipInfo;
				tooltipInfo = cardViewInfo.ShowSubValue2Tooltip ? GetToolTipInfo(point, cardViewInfo.SubValue2Bounds, card.SubValue2, cardId, "SubValue2") : null;
				if(tooltipInfo != null)
					return tooltipInfo;
				tooltipInfo = cardViewInfo.ShowMainValueTooltip ? GetToolTipInfo(point, cardViewInfo.MainValueBounds, card.MainValue, cardId, "MainValue") : null;
				if(tooltipInfo != null)
					return tooltipInfo;
				if(cardViewInfo.SparklineBounds.Contains(point)) {
					SparklineTooltipValues sparklineTooptipValues = card.Data.SparklineTooltipValues;
					return DashboardWinHelper.GetSparklineTooltip(sparklineTooptipValues.Start, sparklineTooptipValues.End, sparklineTooptipValues.Min, sparklineTooptipValues.Max, cardId + "Sparkline");
				}
			}
			return null;
		}
		bool IToolTipControlClient.ShowToolTips { get { return true; } }
		ToolTipControlInfo GetToolTipInfo(Point point, Rectangle rectangle, string text, string id, string elementId) {
			if(rectangle.Contains(point)) {
				string tooltipValue = text;
				string valueId = id + elementId;
				return new ToolTipControlInfo(valueId, tooltipValue);
			}
			return null;
		}
	}
}
