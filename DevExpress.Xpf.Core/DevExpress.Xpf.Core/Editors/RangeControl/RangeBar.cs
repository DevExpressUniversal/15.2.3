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
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.RangeControl;
using DevExpress.Xpf.Editors.RangeControl.Internal;
namespace DevExpress.Xpf.Editors.RangeControl.Internal {
	public class RangeBar : Control {
		const double MinThumbsSpacing = 10;
		public static readonly DependencyProperty SelectionBrushProperty;
		static RangeBar() {
			SelectionBrushProperty =
				DependencyProperty.Register("SelectionBrush", typeof(Brush), typeof(RangeBar),
				new FrameworkPropertyMetadata((d, e) => ((RangeBar)d).OnSelectionBrushChanged()));
		}
		public RangeBar() {
			this.SizeChanged += OnSizeChanged;
		}
		public Brush SelectionBrush {
			get { return (Brush)GetValue(SelectionBrushProperty); }
			set { SetValue(SelectionBrushProperty, value); }
		}
		public double ViewPortEnd { get; set; }
		public double ViewPortStart { get; set; }
		public double ViewPortWidth { get { return ViewPortEnd - ViewPortStart; } }
		public double Minimum { get; set; }
		public double Maximum { get; set; }
		public RangeControl Owner { get; set; }
		internal TransparencyEffect ShaderEffect {
			get { return (rootContainer != null ? rootContainer.Effect : null) as TransparencyEffect; }
		}
		double RenderWidth { get { return rootContainer != null ? rootContainer.ActualWidth : 0; } }
		bool IsDragged { get; set; }
		public void Invalidate() {
			Render();
		}
		Grid rootContainer;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ContentControl rangeBarContainer = LayoutHelper.FindElementByName(this, "PART_RangeBarContainer") as ContentControl;
			rootContainer = rangeBarContainer.Content as Grid;
			this.MouseMove += RangeBarMouseMove;
			this.MouseLeave += RangeBarMouseLeave;
			InitializeElements();
		}
		private void RangeBarMouseLeave(object sender, MouseEventArgs e) {
			ResetCursor();
		}
		private void RangeBarMouseMove(object sender, MouseEventArgs e) {
			if (IsDragged) return;
			Point point = e.GetPosition(this);
			if (IsSliderArea(point)) {
				ChangeCursorHelper.SetHandCursor();
				return;
			}
			if (IsResizeThumbArea(point)) SetResizeCursor();
			else ResetCursor();
		}		
		private bool IsResizeThumbArea(Point point) {
			return TransformHelper.GetElementBounds(leftResizeThumb, this).Contains(point) || TransformHelper.GetElementBounds(rightResizeThumb, this).Contains(point);
		}
		private bool IsSliderArea(Point point) {
			return TransformHelper.GetElementBounds(slider, this).Contains(point);
		}
		protected double NormalizeByLength(double value) {
			if (ActualWidth == 0 || ViewPortWidth <= 0) return 0;
			return value / (ActualWidth - (GetElementWidth(leftResizeThumb) + GetElementWidth(rightResizeThumb) + MinThumbsSpacing));
		}
		Thumb leftResizeThumb;
		Thumb rightResizeThumb;
		Thumb slider;
		Grid middleContainer, topContainer, thumbContainer;
		private void InitializeElements() {
			if (rootContainer != null) {
				InitializeTransparecyEffect();
				leftResizeThumb = LayoutHelper.FindElementByName(rootContainer, "PART_LeftResizeThumb") as Thumb;
				rightResizeThumb = LayoutHelper.FindElementByName(rootContainer, "PART_RightResizeThumb") as Thumb;
				slider = LayoutHelper.FindElementByName(rootContainer, "PART_Slider") as Thumb;
				middleContainer = LayoutHelper.FindElementByName(rootContainer, "PART_MiddleLayerContainer") as Grid;
				thumbContainer = LayoutHelper.FindElementByName(rootContainer, "PART_ThumbContainer") as Grid;
				topContainer = LayoutHelper.FindElementByName(rootContainer, "PART_TopLayerContainer") as Grid;
				slider.DragStarted += OnSliderDragStarted;
				slider.DragDelta += SliderDragDelta;
				slider.DragCompleted += OnSliderDragCompleted;
				leftResizeThumb.DragStarted += OnLeftResizeThumbDragStarted;
				leftResizeThumb.DragCompleted += OnLeftResizeThumbDragCompleted;
				leftResizeThumb.DragDelta += leftResizeThumb_DragDelta;
				rightResizeThumb.DragStarted += OnRightResizeThumbDragStarted;
				rightResizeThumb.DragCompleted += OnRightResizeThumbDragCompleted;
				rightResizeThumb.DragDelta += leftResizeThumb_DragDelta;
				this.MouseLeftButtonUp += RangeBarMouseLeftButtonUp;
				this.TouchUp += RangeBarTouchUp;
				InitializeTransparecyEffect();
				Invalidate();
			}
		}
		void RangeBarTouchUp(object sender, TouchEventArgs e) {
			ScrollByClick(e.GetTouchPoint(this).Position.X);
		}
		void RangeBarMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			ScrollByClick(e.GetPosition(this).X);
		}
		private void ScrollByClick(double position) {
			double length = ViewPortWidth * this.ActualWidth;
			position = position - length / 2;
			position = Math.Min(Math.Max(position, 0), this.ActualWidth - length);
			double normalOffset = Math.Min(Math.Max(position / this.ActualWidth, 0), 1);
			Owner.ScrollRangeBar(normalOffset);
		}
		void leftResizeThumb_DragDelta(object sender, DragDeltaEventArgs e) {
			double currentPosition = Mouse.GetPosition(this).X;
			if (double.IsNaN(prevResizeThumbPosition)) {
				prevResizeThumbPosition = currentPosition;
				return;
			}
			Thumb thumb = isViewPortStartResizing ? leftResizeThumb : rightResizeThumb;
			double thumbCenter = TransformHelper.GetElementCenter(thumb, this);
			if (thumbCenter < 0 || thumbCenter > ActualWidth) return;
			double horizontalChange = currentPosition - thumbCenter;
			double positionDelta = currentPosition - prevResizeThumbPosition;
			prevResizeThumbPosition = currentPosition;
			if (Math.Sign(positionDelta) == Math.Sign(horizontalChange))
				UpdateViewPort(NormalizeByLength(horizontalChange));
		}
		void SliderDragDelta(object sender, DragDeltaEventArgs e) {
			ScrollToPosition(Mouse.GetPosition(this).X);
		}
		private void InitializeTransparecyEffect() {
		}
		private void OnSelectionBrushChanged() {
		}
		private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			Invalidate();
		}
		private void OnRightResizeThumbDragCompleted(object sender, DragCompletedEventArgs e) {
			ResetIsDragged();
			ResetCursor();
			Owner.OnRangeBarViewPortChanged();
		}
		private void OnLeftResizeThumbDragCompleted(object sender, DragCompletedEventArgs e) {
			ResetIsDragged();
			ResetCursor();
			Owner.OnRangeBarViewPortChanged();
		}
		private void OnRightResizeThumbDragStarted(object sender, DragStartedEventArgs e) {
			SetIsDragged();
			SetResizeCursor();
			isViewPortStartResizing = false;
			prevResizeThumbPosition = double.NaN;
		}
		double prevResizeThumbPosition;
		bool isViewPortStartResizing = false;
		private void OnLeftResizeThumbDragStarted(object sender, DragStartedEventArgs e) {
			SetIsDragged();
			SetResizeCursor();
			isViewPortStartResizing = true;
			prevResizeThumbPosition = double.NaN;
		}
		private void SetResizeCursor() {
			ChangeCursorHelper.SetResizeCursor();
		}
		private void UpdateViewPort(double delta) {
			double newViewPortStart = ViewPortStart;
			double newViewPortEnd = ViewPortEnd;
			if (isViewPortStartResizing)
				newViewPortStart = Math.Min(GetCorrectNormalValue(ViewPortStart + delta), ViewPortEnd);
			else
				newViewPortEnd = Math.Max(GetCorrectNormalValue(ViewPortEnd + delta), ViewPortStart);
			if (Owner != null)
				Owner.OnRangeBarViewPortResizing(newViewPortStart, newViewPortEnd, isViewPortStartResizing);
		}
		private void OnSliderDragCompleted(object sender, DragCompletedEventArgs e) {
			ResetIsDragged();
			ResetCursor();
			Owner.OnRangeBarViewPortChanged();
		}
		private void ResetCursor() {
			ChangeCursorHelper.ResetCursorToDefault();
		}
		private void OnSliderDragStarted(object sender, DragStartedEventArgs e) {
			SetIsDragged();
			prevSliderPostion = double.NaN;
			ChangeCursorHelper.SetHandCursor();
		}
		private void SetIsDragged() {
			IsDragged = true;
		}
		private void ResetIsDragged() {
			IsDragged = false;
		}
		double prevSliderPostion;
		private void ScrollToPosition(double newPosition) {
			if (double.IsNaN(prevSliderPostion)) {
				prevSliderPostion = newPosition;
				return;
			}
			double horizontalChange = newPosition - prevSliderPostion;
			prevSliderPostion = newPosition;
			if (Owner != null)
				Owner.OnRangeBarSliderMoved(NormalizeByLength(horizontalChange));
		}
		double leftThumbPosition;
		double rightThumbPosition;
		private void Render() {
			if (rootContainer != null && ViewPortWidth > 0 && ActualWidth > 0) {
				RenderThumbs();
				RenderMiddleContainer();
				RenderTopContainer();
			} else RenderDefault();
		}
		private void RenderThumbs() {
			double width = ActualWidth - (GetElementWidth(leftResizeThumb) + GetElementWidth(rightResizeThumb) + MinThumbsSpacing);
			leftThumbPosition = ViewPortStart * width;
			double thumbsSpacing = (ViewPortWidth * width) + MinThumbsSpacing;
			rightThumbPosition = leftThumbPosition + GetElementWidth(leftResizeThumb) + thumbsSpacing;
			double width1 = leftThumbPosition;
			double width3 = RenderWidth - (rightThumbPosition + GetElementWidth(rightResizeThumb));
			double width2 = RenderWidth - (width1 + width3 + GetElementWidth(leftResizeThumb) + GetElementWidth(rightResizeThumb));
			RenderThumbContainer(width1, width2, width3);
		}
		private void RenderTopContainer() {
			double width1 = leftThumbPosition + GetElementWidth(leftResizeThumb);
			double width3 = RenderWidth - rightThumbPosition;
			double width2 = RenderWidth - (width1 + width3);
			RenderTopContainer(width1, width2, width3);
		}
		private void RenderMiddleContainer() {
			if (CanRenderSelectionSide()) {
				double width1 = Minimum * RenderWidth;
				double width3 =(1 - Maximum) * RenderWidth;
				double width2 = RenderWidth - (width1 + width3);
				RenderMiddleContainer(width1, width2, width3);
			}
		}
		private void RenderDefault() {
			if (topContainer != null)
				RenderTopContainer(RenderWidth, 0, 0);
		}
		private double ConstrainZero(double value) {
			return double.IsNaN(value) ? 0 : Math.Max(value, 0);
		}
		private void RenderThumbContainer(double width1, double width2, double width3) {
			double width = ViewPortWidth / 2 * RenderWidth < 1 ? 0 : ViewPortWidth / 2;
			thumbContainer.ColumnDefinitions[0].Width = new GridLength(ViewPortStart, GridUnitType.Star);
			thumbContainer.ColumnDefinitions[2].Width = new GridLength(width, GridUnitType.Star);
			thumbContainer.ColumnDefinitions[4].Width = new GridLength(width, GridUnitType.Star);
			thumbContainer.ColumnDefinitions[6].Width = new GridLength(1 - ViewPortEnd, GridUnitType.Star);
			thumbContainer.InvalidateArrange();
		}
		private void RenderTopContainer(double width1, double width2, double width3) {
			topContainer.ColumnDefinitions[0].Width = new GridLength(ConstrainZero(width1));
			topContainer.ColumnDefinitions[1].Width = new GridLength(ConstrainZero(width2));
			topContainer.ColumnDefinitions[2].Width = new GridLength(ConstrainZero(width3));
			topContainer.InvalidateArrange();
		}
		private void RenderMiddleContainer(double width1, double width2, double width3) {
			middleContainer.ColumnDefinitions[0].Width = new GridLength(ConstrainZero(width1));
			middleContainer.ColumnDefinitions[1].Width = new GridLength(ConstrainZero(width2));
			middleContainer.ColumnDefinitions[2].Width = new GridLength(ConstrainZero(width3));
			middleContainer.InvalidateArrange();
		}
		private double GetElementWidth(FrameworkElement element) {
			return element == null ? 0d : element.ActualWidth;
		}
		private double GetCorrectNormalValue(double value) {
			return Math.Min(Math.Max(value, 0), 1);
		}
		private bool CanRenderSelectionSide() {
			return rootContainer != null && Maximum - Minimum >= 0;
		}
	}
}
