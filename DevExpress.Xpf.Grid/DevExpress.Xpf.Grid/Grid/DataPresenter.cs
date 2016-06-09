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
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.Automation;
using DevExpress.Xpf.Grid.Hierarchy;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using System.Windows.Input;
using System.Windows.Shapes;
using DevExpress.Mvvm.UI.Interactivity;
using System.Linq;
namespace DevExpress.Xpf.Grid {
	public abstract class DataPresenterManipulation : GridDataPresenterBase {
		internal Point accumulator;
		internal bool deltaOccured;
		internal void StartManipulation() {
			deltaOccured = false;
		}
		internal virtual void DoManipulation(Point translation, Point totalTranslation) {
			if(!deltaOccured && Math.Abs(totalTranslation.X) < View.TouchScrollThreshold && Math.Abs(totalTranslation.Y) < View.TouchScrollThreshold) {
				PointHelper.Offset(ref accumulator, translation.X, translation.Y);
				return;
			}
			deltaOccured = true;
			View.ScrollAnimationLocker.Lock();
			try {
				DataViewBehavior behavior = View.ViewBehavior as DataViewBehavior;
				if(behavior == null) return;
				Size firstElementSize = GetFirstElementSize();
				if(Size.Empty == firstElementSize || firstElementSize.Height < 1)
					return;
				PointHelper.Offset(ref translation, accumulator.X, accumulator.Y);
				double delta = DefineDelta(translation, firstElementSize);
				if(!View.ViewBehavior.AllowPerPixelScrolling) {
					double trunc = Math.Truncate(delta);
					ChangeOffset(behavior, trunc, translation);
					accumulator = GetAccumulator(GetTranslation(delta - trunc, firstElementSize));
				}
				else {
					ChangeOffset(behavior, delta, translation);
					accumulator = GetAccumulator(0);
				}
			}
			finally {
				View.ScrollAnimationLocker.Unlock();
			}
		}
		protected virtual void ChangeOffset(DataViewBehavior behavior, double delta, Point translation) {
			behavior.ChangeVerticalOffsetBy(-delta);
			behavior.ChangeHorizontalOffsetBy(-translation.X);
		}
		protected virtual double DefineDelta(Point translation, Size firstElementSize) {
			return translation.Y / firstElementSize.Height;
		}
		protected virtual Size GetFirstElementSize() {
			FrameworkElement row = GetFirstVisibleRow();
			return row != null ? new Size(row.ActualWidth, row.ActualHeight) : Size.Empty;
		}
		protected virtual double GetTranslation(double delta, Size firstElementSize) {
			return delta * firstElementSize.Height;
		}
		protected virtual Point GetAccumulator(double translation) {
			return new Point(0, translation);
		}
		public DataPresenterManipulation() {
			IsManipulationEnabled = true;
		}
		protected internal override void SetManipulation(bool isColumnFilterOpened) {
			IsManipulationEnabled = !isColumnFilterOpened;
		}
		protected override void OnManipulationDelta(ManipulationDeltaEventArgs e) {
			DoManipulation(new Point(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y),
				new Point(e.CumulativeManipulation.Translation.X, e.CumulativeManipulation.Translation.Y));
			e.Handled = true;
		}
		protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e) {
		}
		protected override void OnStylusDown(StylusDownEventArgs e) {
			StartManipulation();
		}
		protected override void OnStylusUp(StylusEventArgs e) {
			if(!deltaOccured)
				View.ProcessStylusUp((DependencyObject)e.OriginalSource);
		}
		protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e) {
			e.TranslationBehavior.DesiredDeceleration = 0.001;
			e.Handled = true;
			base.OnManipulationInertiaStarting(e);
			if(!deltaOccured)
				e.Cancel();
		}
	}
	public class DataPresenter : DataPresenterManipulation {
		public static readonly DependencyProperty AnimationOffsetProperty;
		static DataPresenter() {
			AnimationOffsetProperty = DependencyPropertyManager.Register("AnimationOffset", typeof(double), typeof(DataPresenter), new UIPropertyMetadata(0d, (d, e) => ((DataPresenter)d).OnAnimationOffsetChanged((double)e.OldValue)));
		}
		public double AnimationOffset {
			get { return (double)GetValue(AnimationOffsetProperty); }
			set { SetValue(AnimationOffsetProperty, value); }
		}
		public override double ScrollItemOffset { get { return ActualAnimationOffset % 1; } }
		void OnAnimationOffsetChanged(double oldValue) {
			InvalidatePanel(Math.Floor(oldValue) == Math.Floor(AnimationOffset));
		}
		void InvalidatePanel(bool updateCellMergingPanels) {
#if DEBUGTEST
			EventLog.Default.AddEvent(new DependencyPropertyValueSnapshot<DataPresenter, double>(AnimationOffsetProperty, this, AnimationOffset));
#endif
			InvalidateMeasure();
			HierarchyPanel panel = Content as HierarchyPanel;
			if(panel != null)
				panel.InvalidateMeasure();
			if(updateCellMergingPanels) {
				View.UpdateCellMergingPanels();
				View.RowsStateDirty = true;
			}
		}
		Storyboard scrollAnimationStoryboard;
		double oldOffset = 0;
		protected override void OnDefineScrollInfoChangedCore() {
			if(View != null && oldOffset != ScrollInfoCore.DefineSizeScrollInfo.Offset) {
				oldOffset = ScrollInfoCore.DefineSizeScrollInfo.Offset;
				if(scrollAnimationStoryboard != null) {
					AnimationOffset = AnimationOffset;
#if SL
					scrollAnimationStoryboard.Stop();
#else
					scrollAnimationStoryboard.Remove();
#endif
					scrollAnimationStoryboard = null;
				}
				if(CanScrollWithAnimation) {
					scrollAnimationStoryboard = GetStoryboard();
					Storyboard.SetTargetProperty(scrollAnimationStoryboard, new PropertyPath(DataPresenter.AnimationOffsetProperty.GetName()));
					Storyboard.SetTarget(scrollAnimationStoryboard, this);
					scrollAnimationStoryboard.Begin();
				}
				else {
					AnimationOffset = ScrollInfoCore.DefineSizeScrollInfo.Offset;
					InvalidatePanel(false);
				}
			}
			base.OnDefineScrollInfoChangedCore();
		}
		protected internal override bool CanScrollWithAnimation {
			get {
				if(View == null || !View.ViewBehavior.AllowPerPixelScrolling || AdjustmentInProgress || View.ScrollAnimationLocker.IsLocked)
					return false;
				return View.ViewBehavior.AllowScrollAnimation && (View.ViewBehavior.ScrollAnimationDuration != 0 || View.ViewBehavior.ScrollAnimationMode == ScrollAnimationMode.Custom);
			}
		}
		protected internal override bool IsAnimationInProgress {
			get {
				if(!CanScrollWithAnimation) return false;
#if !SL
				return AnimationOffset != ActualScrollOffset;
#else
				return (Int64)(AnimationOffset * 1000) != (Int64)(ActualScrollOffset * 1000);
#endif
			}
		}
		Storyboard GetStoryboard() {
			if(View.ViewBehavior.ScrollAnimationMode == ScrollAnimationMode.Custom) {
				CustomScrollAnimationEventArgs e = new CustomScrollAnimationEventArgs(AnimationOffset, oldOffset);
				View.RaiseCustomScrollAnimation(e);
				return e.Storyboard;
			}
			Storyboard storyboard = new Storyboard();
			DoubleAnimation animation = new DoubleAnimation();
			animation.From = AnimationOffset;
			animation.To = oldOffset;
			animation.Duration = new Duration(TimeSpan.FromMilliseconds(View.ViewBehavior.ScrollAnimationDuration));
			animation.EasingFunction = GetEasingFunction();
			storyboard.Children.Add(animation);
			return storyboard;
		}
		IEasingFunction GetEasingFunction() {
			switch(View.ViewBehavior.ScrollAnimationMode) {
				case ScrollAnimationMode.EaseOut:
					return new ExponentialEase() { Exponent = 3 };
				case ScrollAnimationMode.Linear:
					return null;
				case ScrollAnimationMode.EaseInOut:
					return new ExponentialEase() { EasingMode = EasingMode.EaseInOut, Exponent = 3 };
				default:
					throw new NotImplementedException();
			}
		}
		double ActualAnimationOffset {
			get {
#if !SL
				if(scrollAnimationStoryboard == null)
					return (double)GetAnimationBaseValue(AnimationOffsetProperty);
#endif
				return AnimationOffset;
			}
		}
		protected internal override int GenerateItemsOffset { get { return (int)ActualAnimationOffset; } }
		protected internal override double CurrentOffset { get { return ActualAnimationOffset; } }
		protected override double GetOffset() {
			FrameworkElement firstVisibleRowElement = GetFirstVisibleRow();
			return firstVisibleRowElement != null ? HierarchyPanel.GetScrollElementOffset(firstVisibleRowElement, ScrollItemOffset) : 0;
		}
		protected override FrameworkElement CreateContent() {
			return new HierarchyPanel() { VerticalAlignment = System.Windows.VerticalAlignment.Top, DataPresenter = this };
		}
		protected override void UpdateViewCore() {
			HierarchyPanel hierarchyPanel = Content as HierarchyPanel;
			if(hierarchyPanel != null) {
				hierarchyPanel.ItemsContainer = View.MasterRootRowsContainer;
			}
		}
		void InvalidateElements() {
			FrameworkElement child = Content as HierarchyPanel;
			do {
				child = (FrameworkElement)VisualTreeHelper.GetParent(child);
				child.InvalidateMeasure();
			} while(child != this);
		}
		protected override void PregenerateItems(Size constraint) {
			if(LayoutUpdatedHelper.GlobalLocker.IsLocked)
				return;
			if(!rendered && View.RootNodeContainer.ItemCount == 0 && View.DataControl != null && !(View.IsDesignTime && double.IsInfinity(constraint.Height)) && View.DataControl.VisibleRowCount > 0) {
				View.DataControl.ForceLoad();
				View.ViewBehavior.EnsureSurroundingsActualSize(LastConstraint);
				OnDefineScrollInfoChangedCore();
				View.RootNodeContainer.OnDataChangedCore();
				while(View.RootNodeContainer.StartScrollIndex + View.RootNodeContainer.ItemCount < DataControl.VisibleRowCount) {
					GenerateItems(GenerateItemsCount, constraint.Height);
					BaseMeasureOverride(constraint);
					InvalidateElements();
					if(GetChildHeight() >= constraint.Height)
						break;
					if(!double.IsPositiveInfinity(constraint.Height) && View.RootNodeContainer.ItemCount > 70) {
#if DEBUGTEST
						System.Diagnostics.Debug.Assert(false, "too many items pregenerated");
#endif
						break;
					}
				}
				UpdateScrollInfo();
			}
		}
		ITableView TableView { get{ return View as ITableView; }}
		protected override void GenerateItems(int count, double availableHeight) {
			if(!IsInAction) {
				UIElement firstRow = GetFirstVisibleRow();
				double rowHeight = firstRow != null ? firstRow.DesiredSize.Height : TableView.RowMinHeight;
				if(rowHeight != 0 && !double.IsPositiveInfinity(availableHeight))
					count = Math.Max(Math.Min(count, (int)Math.Ceiling((availableHeight - GetChildHeight()) / rowHeight) + 1), 1);
			}
			base.GenerateItems(count, availableHeight);
		}
		protected override void CancelAllGetRows() {
			base.CancelAllGetRows();
			if(DataControl != null && DataControl.DataProviderBase != null) {
				DataControl.DataProviderBase.CancelAllGetRows();
			}
		}
		protected override void EnsureAllRowsLoaded(int firstRowIndex, int rowsCount) {
			base.EnsureAllRowsLoaded(firstRowIndex, rowsCount);
			DataControl.DataProviderBase.EnsureAllRowsLoaded(firstRowIndex, rowsCount);
		}
	}
	public class CardDataPresenterScrollBehavior : DataPresenterScrollBehavior {
		public override void ScrollToHorizontalOffset(DependencyObject source, double offset) {
			CardDataPresenter cardDataPresenter = GetDataPresenter(source) as CardDataPresenter;
			if(cardDataPresenter != null) {
				switch(((CardView)cardDataPresenter.View).Orientation) {
					case Orientation.Vertical: base.ScrollToHorizontalOffset(source, offset);
						return;
					case Orientation.Horizontal: ScrollToHorizontalOffsetCore(source, offset, !cardDataPresenter.View.ViewBehavior.AllowPerPixelScrolling);
						return;
				}
			}
			base.ScrollToHorizontalOffset(source, offset);
		}
		public override void ScrollToVerticalOffset(DependencyObject source, double offset) {
			CardDataPresenter cardDataPresenter = GetDataPresenter(source) as CardDataPresenter;
			if(cardDataPresenter == null) {
				base.ScrollToVerticalOffset(source, offset);
				return;
			}
			if(((CardView)cardDataPresenter.View).Orientation == Orientation.Horizontal) {
				if((offset > 0 && !cardDataPresenter.CanScrollUp()) || (offset < 0 && !cardDataPresenter.CanScrollDown()))
					ScrollToHorizontalOffset(source, -offset);
				else
					ScrollToVerticalOffsetCore(source, -offset, false);
				return;
			}
			if(cardDataPresenter.View.ViewBehavior.AllowPerPixelScrolling) 
				ScrollToVerticalOffsetCore(source, offset, true);
			else
				ScrollToVerticalOffsetCore(source, offset, true);
		}
		public override bool CanScrollDown(DependencyObject source) {
			if(base.CanScrollDown(source)) return true;
			CardDataPresenter cardDataPresenter = GetDataPresenter(source) as CardDataPresenter;
			if(cardDataPresenter != null && ((CardView)cardDataPresenter.View).Orientation == Orientation.Horizontal)
				return CanScrollRight(source);
			return false;
		}
		public override bool CanScrollUp(DependencyObject source) {
			if(base.CanScrollUp(source)) return true;
			CardDataPresenter cardDataPresenter = GetDataPresenter(source) as CardDataPresenter;
			if(cardDataPresenter != null && ((CardView)cardDataPresenter.View).Orientation == Orientation.Horizontal)
				return CanScrollLeft(source);
			return false;
		}
	}
	public abstract class GridDataPresenterBase : DataPresenterBase {
		public GridDataPresenterBase() {
		}
		protected override void UpdateViewCore() {
			ContentControl itemsContainer = Content as ContentControl;
			if(itemsContainer != null) {
				itemsContainer.Content = View.RootRowsContainer;
			}
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			GridDataViewBase view = GridControl.GetCurrentView(this) as GridDataViewBase;
			if(view == null)
				return null;
			return new DataPanelAutomationPeer(view.DataControl);
		}
		internal override bool IsEnoughExpandingItems() {
			VirtualItemsEnumerator en = new VirtualItemsEnumerator(View.RootNodeContainer);
			while(en.MoveNext()) {
				GroupNode groupNode = en.Current as GroupNode;
				if(groupNode != null && !groupNode.IsFinished) {
					bool allElementsVisible = IsElementVisible(groupNode);
					if(groupNode.IsExpanding) {
						groupNode.CanGenerateItems = allElementsVisible;
					}
					if(allElementsVisible)
						return false;
				}
			}
			return true;
		}
		protected override bool IsElementVisible(RowNode node) {
			if(node.NodesContainer != null) {
				if(node.IsCollapsing) return true;
				if(!AreAllElementsVisible(node.NodesContainer)) return false;
			}
			return base.IsElementVisible(node);
		}
	}
	public class GridScrollBarHelper {
		public static readonly DependencyProperty HasFixedRightColumnsProperty = DependencyProperty.RegisterAttached("HasFixedRightColumns", typeof(bool), typeof(GridScrollBarHelper), new PropertyMetadata(false));
		public static readonly DependencyProperty ExtendScrollBarToFixedColumnsProperty = DependencyProperty.RegisterAttached("ExtendScrollBarToFixedColumns", typeof(bool), typeof(GridScrollBarHelper), new PropertyMetadata(false));
		public static bool GetHasFixedRightColumns(DependencyObject obj) {
			return (bool)obj.GetValue(HasFixedRightColumnsProperty);
		}
		public static void SetHasFixedRightColumns(DependencyObject obj, bool value) {
			obj.SetValue(HasFixedRightColumnsProperty, value);
		}
		public static bool GetExtendScrollBarToFixedColumns(DependencyObject obj) {
			return (bool)obj.GetValue(ExtendScrollBarToFixedColumnsProperty);
		}
		public static void SetExtendScrollBarToFixedColumns(DependencyObject obj, bool value) {
			obj.SetValue(ExtendScrollBarToFixedColumnsProperty, value);
		}
	}
	public class GridBorderHelper {
		public static bool GetShowBorder(DependencyObject obj) {
			return (bool)obj.GetValue(ShowBorderProperty);
		}
		public static void SetShowBorder(DependencyObject obj, bool value) {
			obj.SetValue(ShowBorderProperty, value);
		}
		public static readonly DependencyProperty ShowBorderProperty =
			DependencyPropertyManager.RegisterAttached("ShowBorder", typeof(bool), typeof(GridBorderHelper), new FrameworkPropertyMetadata(true));
	}
	public class GridVerticalLinesHelper {
		public static SelectionState GetSelectionState(DependencyObject obj) {
			return (SelectionState)obj.GetValue(SelectionStateProperty);
		}
		public static void SetSelectionState(DependencyObject obj, SelectionState value) {
			obj.SetValue(SelectionStateProperty, value);
		}
		public static readonly DependencyProperty SelectionStateProperty =
			DependencyPropertyManager.RegisterAttached("SelectionState", typeof(SelectionState), typeof(GridVerticalLinesHelper), new FrameworkPropertyMetadata(SelectionState.None, (d, e) => OnShowVerticalLinesChanged(d, e)));
		public static bool GetShowVerticalLines(DependencyObject obj) {
			return (bool)obj.GetValue(ShowVerticalLinesProperty);
		}
		public static void SetShowVerticalLines(DependencyObject obj, bool value) {
			obj.SetValue(ShowVerticalLinesProperty, value);
		}
		public static readonly DependencyProperty ShowVerticalLinesProperty =
			DependencyPropertyManager.RegisterAttached("ShowVerticalLines", typeof(bool), typeof(GridVerticalLinesHelper), new FrameworkPropertyMetadata(false, (d, e) => OnShowVerticalLinesChanged(d, e)));
		public static Brush GetVerticalLinesBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(VerticalLinesBrushProperty);
		}
		public static void SetVerticalLinesBrush(DependencyObject obj, Brush value) {
			obj.SetValue(VerticalLinesBrushProperty, value);
		}
		public static readonly DependencyProperty VerticalLinesBrushProperty =
			DependencyPropertyManager.RegisterAttached("VerticalLinesBrush", typeof(Brush), typeof(GridVerticalLinesHelper), new FrameworkPropertyMetadata());
		static void OnShowVerticalLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Border border = (Border)d;
			bool showLines = (bool)border.GetValue(ShowVerticalLinesProperty);
			SelectionState selectionState = (SelectionState)border.GetValue(SelectionStateProperty);
			Brush brush = (Brush)border.GetValue(VerticalLinesBrushProperty);
			bool show = showLines && selectionState != SelectionState.Focused;
			border.BorderBrush = show ? brush : null;
		}
	}
	public class FadeSelectionHelper {
		public static readonly DependencyProperty IsKeyboardFocusWithinViewProperty = DependencyPropertyManager.RegisterAttached("IsKeyboardFocusWithinView", typeof(bool), typeof(FadeSelectionHelper), new FrameworkPropertyMetadata(false, OnFadeSelectionHelperPropertyChanged));
		public static readonly DependencyProperty FadeSelectionOnLostFocusProperty = DependencyPropertyManager.RegisterAttached("FadeSelectionOnLostFocus", typeof(bool), typeof(FadeSelectionHelper), new FrameworkPropertyMetadata(false, OnFadeSelectionHelperPropertyChanged));
		public static readonly DependencyProperty OpacityProperty = DependencyPropertyManager.RegisterAttached("Opacity", typeof(double), typeof(FadeSelectionHelper), new FrameworkPropertyMetadata(0.5d, OnFadeSelectionHelperPropertyChanged));
		public static readonly DependencyProperty IsSelectedProperty = DependencyPropertyManager.RegisterAttached("IsSelected", typeof(bool?), typeof(FadeSelectionHelper), new FrameworkPropertyMetadata(null, OnFadeSelectionHelperPropertyChanged));
		public static bool GetIsKeyboardFocusWithinView(DependencyObject obj) {
			return (bool)obj.GetValue(IsKeyboardFocusWithinViewProperty);
		}
		public static void SetIsKeyboardFocusWithinView(DependencyObject obj, bool value) {
			obj.SetValue(IsKeyboardFocusWithinViewProperty, value);
		}
		public static bool GetFadeSelectionOnLostFocus(DependencyObject obj) {
			return (bool)obj.GetValue(FadeSelectionOnLostFocusProperty);
		}
		public static void SetFadeSelectionOnLostFocus(DependencyObject obj, bool value) {
			obj.SetValue(FadeSelectionOnLostFocusProperty, value);
		}
		public static double GetOpacity(DependencyObject obj) {
			return (double)obj.GetValue(OpacityProperty);
		}
		public static void SetOpacity(DependencyObject obj, double value) {
			obj.SetValue(OpacityProperty, value);
		}
		public static bool? GetIsSelected(DependencyObject obj) {
			return (bool?)obj.GetValue(IsSelectedProperty);
		}
		public static void SetIsSelected(DependencyObject obj, bool? value) {
			obj.SetValue(IsSelectedProperty, value);
		}
		static void UpdateElementOpacity(DependencyObject element, bool fadeSelectionOnLostFocus, bool isKeyboardFocusWithin) {
			bool isFadeNeeded = IsFadeNeeded(fadeSelectionOnLostFocus, isKeyboardFocusWithin);
			if(isFadeNeeded)
				element.SetValue(UIElement.OpacityProperty, GetOpacity(element));
			else
				element.ClearValue(UIElement.OpacityProperty);
			OnSelectionChanged(element, isFadeNeeded);
		}
		internal static bool IsFadeNeeded(bool fadeSelectionOnLostFocus, bool isKeyboardFocusWithin) {
			return fadeSelectionOnLostFocus && !isKeyboardFocusWithin;
		}
		static void OnFadeSelectionHelperPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UpdateElementOpacity(d, GetFadeSelectionOnLostFocus(d), GetIsKeyboardFocusWithinView(d));
		}
		static void OnSelectionChanged(DependencyObject d, bool isFadeNeeded) {
			if(GetIsSelected(d) == null) return;
			if(isFadeNeeded && GetIsSelected(d) == true)
				SetVisibility(d, Visibility.Visible);
			else
				SetVisibility(d, Visibility.Collapsed);
		}
		static bool IsFadeNeeded(DependencyObject d) {
			return GetFadeSelectionOnLostFocus(d) && !GetIsKeyboardFocusWithinView(d);
		}
		static void SetVisibility(DependencyObject d, Visibility value) {
			UIElement obj = d as UIElement;
			if(obj == null) return;
			obj.Visibility = value;
		}
	}
}
