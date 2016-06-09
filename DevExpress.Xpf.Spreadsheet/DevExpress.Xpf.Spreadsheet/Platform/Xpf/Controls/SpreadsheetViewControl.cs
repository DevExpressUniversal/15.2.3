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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DevExpress.Spreadsheet;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class SpreadsheetViewControl : Control {
		const string VerticalScrollBarName = "PART_VerticalScrollBar";
		const string HorizontalScrollBarName = "PART_HorizontalScrollBar";
		const string WorksheetControlName = "PART_WorkSheetControl";
		internal const string ScrollBarContainerName = "PART_ScrollBarContainer";
		public static readonly DependencyProperty LayoutInfoProperty;
		protected static readonly DependencyPropertyKey SpreadsheetProviderPropertyKey;
		public static readonly DependencyProperty SpreadsheetProviderProperty;
		protected static readonly DependencyPropertyKey ActiveSpreadsheetViewPropertyKey;
		public static readonly DependencyProperty ActiveSpreadsheetViewProperty;
		public static readonly DependencyProperty ScrollBarPositionProperty;
		public static readonly DependencyProperty ShowTabSelectorProperty;
		public static readonly DependencyProperty ShowHorizontalScrollbarProperty;
		public static readonly DependencyProperty ShowVerticalScrollbarProperty;
		static SpreadsheetViewControl() {
			Type ownerType = typeof(SpreadsheetViewControl);
			ActiveSpreadsheetViewPropertyKey =
				DependencyProperty.RegisterAttachedReadOnly("ActiveSpreadsheetView", ownerType, ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			ActiveSpreadsheetViewProperty = ActiveSpreadsheetViewPropertyKey.DependencyProperty;
			LayoutInfoProperty = DependencyProperty.Register("LayoutInfo", typeof(DocumentLayout), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((SpreadsheetViewControl)d).OnLayoutInfoChanged()));
			ScrollBarPositionProperty = DependencyProperty.Register("ScrollBarPosition", typeof(double), ownerType);
			SpreadsheetProviderPropertyKey =
				DependencyProperty.RegisterAttachedReadOnly("SpreadsheetProvider", typeof(SpreadsheetPropertiesProvider), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			SpreadsheetProviderProperty = SpreadsheetProviderPropertyKey.DependencyProperty;
			ShowTabSelectorProperty = DependencyProperty.Register("ShowTabSelector", typeof(bool), ownerType,
			   new FrameworkPropertyMetadata(true, (d, e) => ((SpreadsheetViewControl)d).OnShowTabSelectorChanged((bool)e.NewValue)));
			ShowHorizontalScrollbarProperty = DependencyProperty.Register("ShowHorizontalScrollbar", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			ShowVerticalScrollbarProperty = DependencyProperty.Register("ShowVerticalScrollbar", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
		}
		internal static void SetActiveSpreadsheetView(DependencyObject element, SpreadsheetViewControl value) {
			if (element == null) return;
			element.SetValue(ActiveSpreadsheetViewPropertyKey, value);
		}
		internal static void SetSpreadsheetProvider(DependencyObject element, SpreadsheetPropertiesProvider value) {
			if (element == null) return;
			element.SetValue(SpreadsheetProviderPropertyKey, value);
		}
		public SpreadsheetViewControl() {
			SetActiveSpreadsheetView(this, this);
			this.DefaultStyleKey = typeof(SpreadsheetViewControl);
		}
		public DocumentLayout LayoutInfo {
			get { return (DocumentLayout)GetValue(LayoutInfoProperty); }
			set { SetValue(LayoutInfoProperty, value); }
		}
		public double ScrollBarPosition {
			get { return (double)GetValue(ScrollBarPositionProperty); }
			set { SetValue(ScrollBarPositionProperty, value); }
		}
		public bool ShowTabSelector {
			get { return (bool)GetValue(ShowTabSelectorProperty); }
			set { SetValue(ShowTabSelectorProperty, value); }
		}
		void OnShowTabSelectorChanged(bool newValue) {
			double newWidth = newValue ? ScrollBarPosition : 0;
			SetTabSelectorContainerWidth(newWidth);
		}
		public bool ShowHorizontalScrollbar {
			get { return (bool)GetValue(ShowHorizontalScrollbarProperty); }
			set { SetValue(ShowHorizontalScrollbarProperty, value); }
		}
		public bool ShowVerticalScrollbar {
			get { return (bool)GetValue(ShowVerticalScrollbarProperty); }
			set { SetValue(ShowVerticalScrollbarProperty, value); }
		}
		double HorizontalOffset { get { return horizontalScrollBar.Value; } }
		private void OnLayoutInfoChanged() { }
		void UpdateScrollBarPosition() {
			UpdateScrollBarPositionCore(scrollBarContainer.ColumnDefinitions[0].ActualWidth);
		}
		void UpdateScrollBarPositionCore(double width) {
			ScrollBarPosition = ShowHorizontalScrollbar ? width : 0;
		}
		ScrollBar verticalScrollBar;
		ScrollBar horizontalScrollBar;
		WorksheetControl worksheetControl;
		SpreadsheetControl owner;
		SpreadsheetTabSelector tabselector;
		Thumb resizeThumb;
		Grid scrollBarContainer;
		protected internal WorksheetControl WorksheetControl { get { return worksheetControl; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UnsubscribeEvents();
			worksheetControl = LayoutHelper.FindElementByName(this, WorksheetControlName) as WorksheetControl;
			verticalScrollBar = LayoutHelper.FindElementByName(this, VerticalScrollBarName) as ScrollBar;
			horizontalScrollBar = LayoutHelper.FindElementByName(this, HorizontalScrollBarName) as ScrollBar;
			tabselector = LayoutHelper.FindElementByType(this, typeof(SpreadsheetTabSelector)) as SpreadsheetTabSelector;
			resizeThumb = LayoutHelper.FindElementByType(this, typeof(Thumb)) as Thumb;
			scrollBarContainer = LayoutHelper.FindElementByName(this, ScrollBarContainerName) as Grid;
			owner = LayoutHelper.FindParentObject<SpreadsheetControl>(this);
			SetSpreadsheetProvider(this, new SpreadsheetPropertiesProvider(owner));
			SubscribeEvents();
		}
		void UnsubscribeEvents() {
			if (owner != null) owner.DocumentModel.DocumentCleared -= OnDocumentCleared;
			if (worksheetControl != null) worksheetControl.SizeChanged -= WorksheetControlSizeChanged;
			if (resizeThumb != null) resizeThumb.DragDelta -= ResizeThumbDragDelta;
			if (tabselector != null) tabselector.ActiveTabChanging -= OnActiveSheetChanging;
			if (scrollBarContainer != null) scrollBarContainer.SizeChanged -= OnScrollBarContainerSizeChanged;
		}
		void SubscribeEvents() {
			owner.DocumentModel.DocumentCleared += OnDocumentCleared;
			worksheetControl.SizeChanged += WorksheetControlSizeChanged;
			resizeThumb.DragDelta += ResizeThumbDragDelta;
			tabselector.ActiveTabChanging += OnActiveSheetChanging;
			scrollBarContainer.SizeChanged += OnScrollBarContainerSizeChanged;
		}
		void OnDocumentCleared(object sender, EventArgs e) {
			worksheetControl.ClearMeasureCache(true);
			LayoutInfo = null;
		}
		void OnActiveSheetChanging(object sender, EventArgs e) {
			ClearMeasureCache();
		}
		internal void ClearMeasureCache() {
			worksheetControl.ClearMeasureCache();
		}
		void ResizeThumbDragDelta(object sender, DragDeltaEventArgs e) {
			double wholeWidth = scrollBarContainer.ActualWidth;
			double delta = e.HorizontalChange;
			double leftWidth = scrollBarContainer.ColumnDefinitions[0].ActualWidth;
			double middleWidth = scrollBarContainer.ColumnDefinitions[1].ActualWidth;
			leftWidth += delta;
			leftWidth = Math.Max(Math.Min(leftWidth, wholeWidth - middleWidth), 0);
			SetTabSelectorContainerWidth(leftWidth);
			UpdateScrollBarPosition();
		}
		void OnScrollBarContainerSizeChanged(object sender, SizeChangedEventArgs e) {
			if (ScrollBarPosition == 0)
				UpdateScrollBarPositionCore(0.7 * e.NewSize.Width);
			else
				UpdateScrollBarPosition();
		}
		void SetTabSelectorContainerWidth(double newWidth) {
			if (scrollBarContainer == null)
				return;
			scrollBarContainer.ColumnDefinitions[0].Width = new GridLength(newWidth);
		}
		void WorksheetControlSizeChanged(object sender, System.Windows.SizeChangedEventArgs e) {
			owner.OnViewportChanged(new System.Drawing.Size((int)e.NewSize.Width, (int)e.NewSize.Height));
		}
		private void Invalidate() {
			if (worksheetControl != null) {
				if (owner.CellTemplate != null)
					worksheetControl.CellTemplate = owner.CellTemplate;
				worksheetControl.CellTemplateSelector = owner.CellTemplateSelector;
				worksheetControl.Invalidate();
			}
		}
		internal ScrollBar GetVerticalScrollBar() {
			return verticalScrollBar;
		}
		internal ScrollBar GetHorizontalScrollBar() {
			return horizontalScrollBar;
		}
		int transactionVersion = 0;
		internal void Invalidate(DocumentLayout documentLayout) {
			LayoutInfo = documentLayout;
			int currentVersion = documentLayout.DocumentModel.TransactionVersion;
			if (transactionVersion != currentVersion && !WorksheetControl.IsMeasureInProcess) {
				transactionVersion = currentVersion;
				ClearMeasureCache();
			}
			Invalidate();
		}
		internal System.Drawing.Size GetViewport() {
			return worksheetControl != null ? new System.Drawing.Size((int)worksheetControl.ActualWidth, (int)worksheetControl.ActualHeight)
				: new System.Drawing.Size(0, int.MaxValue);
		}
		internal ITabSelector GetTabSelector() {
			return tabselector as ITabSelector;
		}
		internal SpreadsheetView GetView() {
			return owner.GetCurrentView();
		}
		internal void DrawReversibleLine(int coordinate, bool isVertical) {
			worksheetControl.DrawResizeFeedback(coordinate, isVertical);
		}
		internal void ShowResizeFeedback(bool canShow) {
			worksheetControl.ShowResizeFeedback(canShow);
			if (!canShow) {
				worksheetControl.ClearMeasureCache();
			}
		}
		internal XpfCellInplaceEditor GetEditor() {
			return worksheetControl.GetEditor();
		}
		internal System.Windows.Input.Cursor GetResizeThumbCursor(Point position) {
			if (!ShowHorizontalScrollbar)
				return null;
			Rect bounds = resizeThumb.TransformToVisual(owner).TransformBounds(new Rect(0, 0, resizeThumb.ActualWidth, resizeThumb.ActualHeight));
			return bounds.Contains(position) ? System.Windows.Input.Cursors.SizeWE : null;
		}
		internal void ShowDragRangeFeedback(Rect bounds) {
			worksheetControl.ShowDragRangeFeedback(bounds);
		}
		internal void ShowCommentVisualFeedback(Rect bounds) {
			worksheetControl.ShowCommentVisualFeedback(bounds);
		}
		internal void HideVisualFeedback() {
			worksheetControl.HideVisualFeedback();
		}
		internal void ShowDragFloatingObjectVisualFeedback(bool canShow, DrawingBox box, Rect bounds, float angle) {
			worksheetControl.ShowDragFloatingObjectVisualFeedback(canShow, box, bounds, angle);
			if (!canShow) worksheetControl.ClearMeasureCache();
		}
		internal SpreadsheetHitTestType GetHitTest(Point pointRelativelySpreadsheetControl) {
			Point pointRelativelyWorksheet = this.TransformToVisual(worksheetControl).Transform(pointRelativelySpreadsheetControl);
			pointRelativelyWorksheet = worksheetControl.GetPointСonsideringScale(pointRelativelyWorksheet);
			Rect worksheetBounds = new Rect(0, 0, worksheetControl.ActualWidth * DocumentModel.DpiX / 96.0, worksheetControl.ActualHeight * DocumentModel.DpiY / 96.0);
			if (worksheetBounds.Contains(pointRelativelyWorksheet))
				return SpreadsheetHitTestType.Worksheet;
			Point tabPoint = this.TransformToVisual(tabselector).Transform(pointRelativelySpreadsheetControl);
			if (ShowTabSelector && !string.IsNullOrEmpty(tabselector.GetTabNameByPoint(tabPoint)))
				return SpreadsheetHitTestType.TabSelector;
			return SpreadsheetHitTestType.None;
		}
		internal System.Drawing.Size GetHeaderSize() {
			return new System.Drawing.Size((int)worksheetControl.HeaderWidth, (int)worksheetControl.HeaderHeight);
		}
		internal Color GetGridLinesColor() {
			return worksheetControl != null ? worksheetControl.GetGridLinesColor() : Color.FromArgb(0, 0, 0, 0);
		}
	}
	public class SpreadsheetPropertiesProvider {
		public SpreadsheetPropertiesProvider(SpreadsheetControl control) {
			this.control = new WeakReference(control);
		}
		WeakReference control;
		SpreadsheetControl Control { get { return control.Target as SpreadsheetControl; } }
		public IWorkbook ApiDocument { get { return Control.Document; } }
		public DevExpress.Spreadsheet.Worksheet ActiveApiWorksheet { get { return Control.ActiveWorksheet; } }
		public double ScaleFactor { get { return GetScaleFactor(); } }
		public SpreadsheetView ActiveView { get { return Control.ActiveView; } }
		public bool IsInpaceEditorActive { get { return Control.InnerControl.IsInplaceEditorActive; } }
		public CellPosition InplaceEditorPosition { get { return Control.InnerControl.InplaceEditor.CellPosition; } }
		public ShowCellToolTipMode ShowCellToolTipMode { get { return Control.ShowCellToolTipMode; } }
		public object CellToolTip { get { return Control.CellToolTip; } }
		double GetScaleFactor() {
			return (ActiveView != null ? Control.ActiveView.ZoomFactor : 1) * 96.0 / DocumentModel.Dpi;
		}
	}
}
