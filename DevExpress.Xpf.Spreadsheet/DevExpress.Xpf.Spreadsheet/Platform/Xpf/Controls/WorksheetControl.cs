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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.IO;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Charts;
using DevExpress.XtraSpreadsheet.Services;
using ChartsModel = DevExpress.Charts.Model;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Utils;
using System.Windows.Input;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class DragFloatingObjectFeedbackParams {
		public DragFloatingObjectFeedbackParams(Rect bounds, float angle) {
			Bounds = bounds;
			Angle = angle;
		}
		public Rect Bounds { get; private set; }
		public float Angle { get; private set; }
	}
	public class CellItem {
		public object Content { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
	}
	#region WorksheetControl
	public class WorksheetControl : Control, IDrawingBoxVisitor {
		#region Fields
		const string HorizontalHeadersName = "PART_HorizontalHeadersControl";
		const string VerticalHeadersName = "PART_VerticalHeadersControl";
		const string WorksheetPanelName = "PART_WorksheetPanel";
		public static readonly DependencyProperty GroupWidthProperty;
		public static readonly DependencyProperty GroupHeightProperty;
		public static readonly DependencyProperty HeaderHeightProperty;
		public static readonly DependencyProperty HeaderWidthProperty;
		public static readonly DependencyProperty LayoutInfoProperty;
		public static readonly DependencyProperty CellTemplateSelectorProperty;
		public static readonly DependencyProperty CellTemplateProperty;
		public static readonly DependencyProperty ConditionalFormattingIconTemplateProperty;
		public static readonly DependencyProperty ConditionalFormattingDataBarTemplateProperty;
		WorksheetPanel worksheetPanel;
		WorksheetLineControl lineControl;
		WorksheetBackgroundControl backgroundControl;
		WorksheetSelectionControl selectionControl;
		WorksheetHeadersControl horizontalHeadersControl;
		WorksheetHeadersControl verticalHeadersControl;
		WorksheetDiagonalBorderControl diagonalBorderControl;
		WorksheetGroupControl groupControl;
		VisualFeedbackControl visualFeedbackControl;
		FloatingObjectsContainer floatingObjectsContainer;
		FormulaRangesControl formulaRangesControl;
		XpfCellInplaceEditor cellEditor;
		Border cellEditorContainer;
		XpfCommentInplaceEditor commentInplaceEditor;
		WorksheetIndicatorControl indicatorControl;
		WorksheetCommentPanel commentPanel;
		WorksheetDataValidationPanel dataValidationPanel;
		XpfDataValidationInplaceEditor dataValidationInplaceEditor;
		AutoFilterImageCache autoFilterImageCache;
		#endregion
		static WorksheetControl() {
			Type ownerType = typeof(WorksheetControl);
			GroupWidthProperty = DependencyProperty.Register("GroupWidth", typeof(double), ownerType);
			GroupHeightProperty = DependencyProperty.Register("GroupHeight", typeof(double), ownerType);
			HeaderHeightProperty = DependencyProperty.Register("HeaderHeight", typeof(double), ownerType);
			HeaderWidthProperty = DependencyProperty.Register("HeaderWidth", typeof(double), ownerType);
			LayoutInfoProperty = DependencyProperty.Register("LayoutInfo", typeof(DocumentLayout), ownerType);
			CellTemplateSelectorProperty = DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), ownerType);
			CellTemplateProperty = DependencyProperty.Register("CellTemplate", typeof(DataTemplate), ownerType);
			ConditionalFormattingIconTemplateProperty = DependencyProperty.Register("ConditionalFormattingIconTemplate", typeof(DataTemplate), ownerType);
			ConditionalFormattingDataBarTemplateProperty = DependencyProperty.Register("ConditionalFormattingDataBarTemplate", typeof(DataTemplate), ownerType);
		}
		public WorksheetControl() {
			this.autoFilterImageCache = new AutoFilterImageCache();
			this.DefaultStyleKey = typeof(WorksheetControl);
		}
		#region Properties
		public double GroupWidth {
			get { return (double)GetValue(GroupWidthProperty); }
			set { SetValue(GroupWidthProperty, value); }
		}
		public double GroupHeight {
			get { return (double)GetValue(GroupHeightProperty); }
			set { SetValue(GroupHeightProperty, value); }
		}
		public double HeaderHeight {
			get { return (double)GetValue(HeaderHeightProperty); }
			set { SetValue(HeaderHeightProperty, value); }
		}
		public double HeaderWidth {
			get { return (double)GetValue(HeaderWidthProperty); }
			set { SetValue(HeaderWidthProperty, value); }
		}
		public DocumentLayout LayoutInfo {
			get { return (DocumentLayout)GetValue(LayoutInfoProperty); }
			set { SetValue(LayoutInfoProperty, value); }
		}
		public DataTemplateSelector CellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
			set { SetValue(CellTemplateSelectorProperty, value); }
		}
		public DataTemplate CellTemplate {
			get { return (DataTemplate)GetValue(CellTemplateProperty); }
			set { SetValue(CellTemplateProperty, value); }
		}
		public DataTemplate ConditionalFormattingIconTemplate {
			get { return (DataTemplate)GetValue(ConditionalFormattingIconTemplateProperty); }
			set { SetValue(ConditionalFormattingIconTemplateProperty, value); }
		}
		public DataTemplate ConditionalFormattingDataBarTemplate {
			get { return (DataTemplate)GetValue(ConditionalFormattingDataBarTemplateProperty); }
			set { SetValue(ConditionalFormattingDataBarTemplateProperty, value); }
		}
		SpreadsheetPropertiesProvider SpreadsheetProvider { get { return GetValue(SpreadsheetViewControl.SpreadsheetProviderProperty) as SpreadsheetPropertiesProvider; } }
		internal double ScaleFactor { get { return SpreadsheetProvider != null ? SpreadsheetProvider.ScaleFactor : 1; } }
		internal WorksheetDiagonalBorderControl DiagonalBorderControl { get { return diagonalBorderControl; } }
		internal bool IsMeasureInProcess { get { return worksheetPanel != null ? worksheetPanel.IsMeasureInProcess : false; } }
		internal AutoFilterImageCache AutoFilterImageCache { get { return autoFilterImageCache; } }
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			horizontalHeadersControl = LayoutHelper.FindElementByName(this, HorizontalHeadersName) as WorksheetHeadersControl;
			verticalHeadersControl = LayoutHelper.FindElementByName(this, VerticalHeadersName) as WorksheetHeadersControl;
			worksheetPanel = LayoutHelper.FindElementByName(this, WorksheetPanelName) as WorksheetPanel;
			lineControl = LayoutHelper.FindElementByType(this, typeof(WorksheetLineControl)) as WorksheetLineControl;
			backgroundControl = LayoutHelper.FindElementByType(this, typeof(WorksheetBackgroundControl)) as WorksheetBackgroundControl;
			selectionControl = LayoutHelper.FindElementByType(this, typeof(WorksheetSelectionControl)) as WorksheetSelectionControl;
			diagonalBorderControl = LayoutHelper.FindElementByType(this, typeof(WorksheetDiagonalBorderControl)) as WorksheetDiagonalBorderControl;
			visualFeedbackControl = LayoutHelper.FindElementByType(this, typeof(VisualFeedbackControl)) as VisualFeedbackControl;
			floatingObjectsContainer = LayoutHelper.FindElementByType(this, typeof(FloatingObjectsContainer)) as FloatingObjectsContainer;
			formulaRangesControl = LayoutHelper.FindElementByType(this, typeof(FormulaRangesControl)) as FormulaRangesControl;
			cellEditor = LayoutHelper.FindElementByType(this, typeof(XpfCellInplaceEditor)) as XpfCellInplaceEditor;
			cellEditorContainer = LayoutHelper.FindParentObject<Border>(cellEditor);
			groupControl = LayoutHelper.FindElementByType(this, typeof(WorksheetGroupControl)) as WorksheetGroupControl;
			commentInplaceEditor = LayoutHelper.FindElementByType(this, typeof(XpfCommentInplaceEditor)) as XpfCommentInplaceEditor;
			indicatorControl = LayoutHelper.FindElementByType(this, typeof(WorksheetIndicatorControl)) as WorksheetIndicatorControl;
			commentPanel = LayoutHelper.FindElementByType(this, typeof(WorksheetCommentPanel)) as WorksheetCommentPanel;
			dataValidationPanel = LayoutHelper.FindElementByType(this, typeof(WorksheetDataValidationPanel)) as WorksheetDataValidationPanel;
			dataValidationInplaceEditor = LayoutHelper.FindElementByType(this, typeof(XpfDataValidationInplaceEditor)) as XpfDataValidationInplaceEditor;
			Invalidate();
		}
		internal void Invalidate() {
			SetScale(ScaleFactor);
			SetGroupSize();
			SetHeadersSize();
			horizontalHeadersControl.Invalidate();
			verticalHeadersControl.Invalidate();
			backgroundControl.InvalidateVisual();
			lineControl.InvalidateVisual();
			selectionControl.InvalidateVisual();
			formulaRangesControl.InvalidateVisual();
			if (!worksheetPanel.IsMeasureInProcess)
				worksheetPanel.InvalidateMeasure();
			diagonalBorderControl.InvalidateVisual();
			groupControl.Invalidate();
			floatingObjectsContainer.InvalidateArrange();
			indicatorControl.InvalidateVisual();
			commentPanel.InvalidateVisual();
			dataValidationPanel.InvalidateVisual();
			commentInplaceEditor.InvalidateVisual();
			SetCliping(new Size(this.ActualWidth * (1 / ScaleFactor), this.ActualHeight * (1 / ScaleFactor)));
		}
		void SetGroupSize() {
			if (LayoutInfo == null || LayoutInfo.GroupItemsPage == null)
				return;
			Size offset = LayoutInfo.GroupItemsPage.GroupItemsOffset.ToSize();
			GroupWidth = offset.Width;
			GroupHeight = offset.Height;
		}
		void SetHeadersSize() {
			if (LayoutInfo == null || LayoutInfo.HeaderPage == null) {
				HeaderWidth = 0;
				HeaderHeight = 0;
				return;
			}
			Size offset = LayoutInfo.HeaderPage.HeadersOffset.ToSize();
			HeaderWidth = offset.Width;
			HeaderHeight = offset.Height;
		}
		private void SetScale(double factor) {
			ScaleTransform trans = new ScaleTransform(factor, factor);
			this.RenderTransform = trans;
		}
		private void SetCliping(Size size) {
			this.Clip = new RectangleGeometry(new Rect(new Point(), size));
		}
		internal void ClearMeasureCache() {
			ClearMeasureCache(false);
		}
		internal void ClearMeasureCache(bool deleteChildren) {
			worksheetPanel.ClearCache(deleteChildren);
			floatingObjectsContainer.ClearCache(deleteChildren);
		}
		internal void ShowResizeFeedback(bool canShow) {
			if (canShow)
				visualFeedbackControl.BeginDrawFeedback(new ResizeFeedbackState(false, 0, ScaleFactor));
			else
				visualFeedbackControl.EndDrawFeedback();
		}
		internal void DrawResizeFeedback(int coordinate, bool isVertical) {
			visualFeedbackControl.BeginDrawFeedback(new ResizeFeedbackState(isVertical, coordinate, ScaleFactor));
		}
		int patchOffset = 1;
		internal void ShowInplaceEditor(XtraSpreadsheet.Internal.InplaceEditorBoundsInfo boundsInfo, Size size, TextAlignment alignment) {
			Rect bounds = boundsInfo.EditorBounds.ToRect();
			double left = bounds.X - patchOffset;
			double width = size.Width + 2 * patchOffset;
			double height = size.Height;
			double top = bounds.Top;
			if (alignment == TextAlignment.Right)
				left = left + bounds.Width - width;
			else if (alignment == TextAlignment.Center)
				left = left + bounds.Width / 2 - width / 2;
			double zoomFactor = 1 / (ScaleFactor * DocumentModel.Dpi / 96.0);
			left *= zoomFactor;
			top *= zoomFactor;
			width *= zoomFactor;
			height *= zoomFactor;
			cellEditorContainer.SetValue(Canvas.LeftProperty, left);
			cellEditorContainer.SetValue(Canvas.TopProperty, top);
			cellEditorContainer.Width = width;
			cellEditorContainer.Height = height;
		}
		internal XpfCellInplaceEditor GetEditor() { return cellEditor; }
		internal XpfCommentInplaceEditor CommentInplaceEditor { get { return commentInplaceEditor; } }
		internal XpfDataValidationInplaceEditor DataValidationInplaceEditor { get { return dataValidationInplaceEditor; } }
		internal Brush GetDataValidationBackgroundBrush() {
			return dataValidationPanel.DataValidationBackgroundBrush;
		}
		internal Color GetDataValidationBorderColor() {
			return dataValidationPanel.DataValidationBorderColor;
		}
		internal bool ValidateInplaceEditorPosition(CellPosition cellPosition) {
			if (!SpreadsheetProvider.IsInpaceEditorActive) return true;
			return !cellPosition.Equals(SpreadsheetProvider.InplaceEditorPosition);
		}
		internal void ShowDragRangeFeedback(Rect bounds) {
			DragRangeVisualFeedbackState state = new DragRangeVisualFeedbackState(bounds, selectionControl.SelectionBorderBrush);
			visualFeedbackControl.BeginDrawFeedback(state);
		}
		internal void ShowCommentVisualFeedback(Rect bounds) {
			visualFeedbackControl.BeginDrawFeedback(new CommentVisualFeedbackState(bounds));
		}
		internal void HideVisualFeedback() {
			visualFeedbackControl.EndDrawFeedback();
		}
		DragFloatingObjectFeedbackParams ObjectParams { get; set; }
		DragFloatingObjectVisualFeedbackState FloatingObjectState { get; set; }
		internal void ShowDragFloatingObjectVisualFeedback(bool canShow, DrawingBox box, Rect bounds, float angle) {
			if (box != null && canShow) {
				ObjectParams = new DragFloatingObjectFeedbackParams(bounds, angle);
				box.Visit(this);
			}
			else {
				FloatingObjectState = null;
				visualFeedbackControl.EndDrawFeedback();
			}
		}
		internal DataTemplate GetConditionalFormattingTemplate(ICell cell, IConditionalFormattingPainters condFmts, System.Drawing.Rectangle bounds, ref ConditionalFormattingSettings cfSettings) {
			if (condFmts.IconSet != null) {
				return GetConditionalFormattingIconTemplate(cell, condFmts.IconSet, cfSettings);
			}
			if (condFmts.DataBar != null)
				return GetConditionalFormattingDataBarTemplate(cell, condFmts.DataBar, bounds, ref cfSettings);
			return null;
		}
		private DataTemplate GetConditionalFormattingDataBarTemplate(ICell cell, DataBarConditionalFormatting formatting, System.Drawing.Rectangle bounds, ref ConditionalFormattingSettings cfSettings) {
			ConditionalFormattingDataBarEvaluationResult value = formatting.Evaluate(cell);
			if (value.Length < 0)
				return null;
			ICondFmtDataBarPainterCalculator calculator = CondFmtDataBarPainterCalculator.GetPainterCalculator(formatting, value, bounds, 1);
			if (calculator == null)
				return null;
			calculator.Process();
			System.Drawing.Rectangle barBounds = calculator.Bar;
			bool isPositive = value.Value >= 0;
			System.Drawing.Color fillColor = (isPositive || formatting.IsNegativeColorSameAsPositive) ? formatting.Color : formatting.NegativeValueColor;
			if (barBounds.Width > 0 && barBounds.Height > 0 && !DXColor.IsEmpty(fillColor)) {
				if (formatting.GradientFill) {
					bool gradientFromLeftToRight = (formatting.AxisPosition == ConditionalFormattingDataBarAxisPosition.None) || !(calculator.IsLeftToRight ^ isPositive);
					IGradientColorCalculator gradientColors = DataBarGradientColorCalculatorBase.GetCalculator(gradientFromLeftToRight);
					gradientColors.Prepare(fillColor);
					cfSettings.DataBarFillBrush = new LinearGradientBrush(gradientColors.StartColor.ToWpfColor(), gradientColors.EndColor.ToWpfColor(), 0);
				}
				else
					cfSettings.DataBarFillBrush = new SolidColorBrush(fillColor.ToWpfColor());
			}
			if (formatting.IsBorderColorAssigned) {
				System.Drawing.Color borderColor = isPositive ? formatting.BorderColor : formatting.NegativeValueBorderColor;
				if (!DXColor.IsEmpty(borderColor)) {
					cfSettings.DataBarBorderBrush = new SolidColorBrush(borderColor.ToWpfColor());
				}
			}
			System.Drawing.Point axisTopPos = calculator.AxisTop;
			if (!axisTopPos.IsEmpty && !DXColor.IsEmpty(formatting.AxisColor)) {
				cfSettings.AxisBrush = new SolidColorBrush(formatting.AxisColor.ToWpfColor());
				double offsetX = bounds.X - 1.5;
				double offsetY = bounds.Y;
				cfSettings.AxisStart = new Point(axisTopPos.X - offsetX, axisTopPos.Y - offsetY);
				cfSettings.AxisEnd = new Point(axisTopPos.X - offsetX, bounds.Bottom - offsetY);
			}
			if (barBounds.Width > 0 && barBounds.Height > 0)
				cfSettings.DataBarBounds = new Rect(barBounds.X - bounds.X + 1, barBounds.Y - bounds.Y, barBounds.Width, barBounds.Height);
			return ConditionalFormattingDataBarTemplate;
		}
		private DataTemplate GetConditionalFormattingIconTemplate(ICell cell, IconSetConditionalFormatting formatting, ConditionalFormattingSettings cfSettings) {
			IconSetType iconSet = formatting.IconSet;
			if (iconSet == IconSetType.None)
				return null; 
			int iconIndex = formatting.EvaluateIconIndex(cell);
			if (iconIndex < 0)
				return null;
			if (formatting.IsCustom) {
				ConditionalFormattingCustomIcon customIcon = formatting.GetIcon(iconIndex);
				iconSet = customIcon.IconSet;
				if (iconSet == IconSetType.None)
					return null;  
				iconIndex = customIcon.IconIndex;
			}
			System.Drawing.Image image = ConditionalFormattingIconSet.GetImage(iconSet, iconIndex);
			cfSettings.Image = image.ToImageSource();
			return ConditionalFormattingIconTemplate;
		}
		internal Point GetPointСonsideringScale(Point point) {
			double actualScaleFactor = ScaleFactor * DocumentModel.Dpi / 96.0;
			if (actualScaleFactor == 1.0d)
				return point;
			ScaleTransform transform = new ScaleTransform(actualScaleFactor, actualScaleFactor);
			return transform.Transform(point);
		}
		#region IDrawingBoxVisitor Members
		void IDrawingBoxVisitor.Visit(ChartBox box) {
			if (FloatingObjectState == null) {
				ChartControl control = new ChartControl();
				FloatingObjectState = new DragFloatingObjectVisualFeedbackState(control, ObjectParams);
			}
			BeginShowDragFloatingObjectVisualFeedback();
		}
		void IDrawingBoxVisitor.Visit(PictureBox box) {
			if (FloatingObjectState == null) {
				Image image = new Image() { Source = box.NativeImage.ToImageSource() };
				FloatingObjectState = new DragFloatingObjectVisualFeedbackState(image, ObjectParams);
			}
			BeginShowDragFloatingObjectVisualFeedback();
		}
		void IDrawingBoxVisitor.Visit(ShapeBox box) {
		}
		private void BeginShowDragFloatingObjectVisualFeedback() {
			FloatingObjectState.SetParams(ObjectParams);
			visualFeedbackControl.BeginDrawFeedback(FloatingObjectState);
		}
		#endregion
		internal DevExpress.Spreadsheet.Cell GetApiCell(CellPosition cellPosition) {
			return SpreadsheetProvider.ActiveApiWorksheet[cellPosition.Row, cellPosition.Column];
		}
		internal Color GetGridLinesColor() {
			return lineControl.GridLinesColor;
		}
		public object GetCellToolTip() {
			return SpreadsheetProvider.CellToolTip;
		}
	}
	#endregion
}
