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

using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Collections;
using System;
using System.Windows.Threading;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using DevExpress.Xpf.Grid.DragDrop;
using System.Windows.Input;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Data.Browsing;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class DataViewDragDropManager : DragDropManagerBase {
		public static readonly DependencyProperty AllowScrollingProperty = DependencyPropertyManager.Register("AllowScrolling",
			typeof(bool), typeof(DataViewDragDropManager), new PropertyMetadata(true));
		public static readonly DependencyProperty AllowAutoExpandProperty = DependencyPropertyManager.Register("AllowAutoExpand",
			typeof(bool), typeof(DataViewDragDropManager), new PropertyMetadata(true));
		public static readonly DependencyProperty AutoExpandDelayProperty = DependencyPropertyManager.Register("AutoExpandDelay",
			typeof(int), typeof(DataViewDragDropManager), 
			new PropertyMetadata(1000, (s, e) => {
				((DataViewDragDropManager)s).AutoExpandTimer.Interval = TimeSpan.FromMilliseconds((int)e.NewValue);
			}));
		public DataViewDragDropManager() {
			ScrollSpeed = 1;
			ScrollSpacing = 10;
			AutoExpandTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(AutoExpandDelay) };
		}
		IList itemsSourceCore;
		protected internal override IList ItemsSource { get { return itemsSourceCore; } }
		protected internal DataControlBase DataControl { get { return AssociatedObject as DataControlBase; } }
		public virtual DataViewBase View { get { return null; } }
		public GridViewHitInfoBase MouseDownHitInfo { get; protected internal set; }
		protected internal override bool CustomAllowDrag(IndependentMouseEventArgs e) {
			DraggingRows = CalcDraggingRows(e);
			StartDragEventArgs startDragArgs = RaiseStartDragEvent(e);
			return startDragArgs != null ? startDragArgs.CanDrag : true;
		}
		protected internal override bool CanStartDrag(MouseButtonEventArgs e) {
			MouseDownHitInfo = GetHitInfo(e.OriginalSource as DependencyObject);
			return false;
		}
		protected DispatcherTimer scrollTimer;
		protected DispatcherTimer AutoExpandTimer;
		DependencyObject hitElement;
		public bool AllowScrolling {
			get { return (bool)GetValue(AllowScrollingProperty); }
			set { SetValue(AllowScrollingProperty, value); }
		}
		public bool AllowAutoExpand {
			get { return (bool)GetValue(AllowAutoExpandProperty); }
			set { SetValue(AllowAutoExpandProperty, value); }
		}
		public int AutoExpandDelay {
			get { return (int)GetValue(AutoExpandDelayProperty); }
			set { SetValue(AutoExpandDelayProperty, value); }
		}
		protected internal Point LastPosition { get; set; }
		public double ScrollSpacing { get; set; }
		public double ScrollSpeed { get; set; }
		protected DependencyObject HitElement { get { return this.hitElement; } }
		[Browsable(false)]
		public virtual void InvalidateScrolling(Point point) {
			Rect rect = LayoutHelper.GetRelativeElementRect(View.ScrollContentPresenter, DataControl);
			double position = point.Y - rect.Top;
			if(position > 0 && position < ScrollSpacing) {
				View.ChangeVerticalScrollOffsetBy(-ScrollSpeed);
			}
			if(position > View.ScrollContentPresenter.ActualHeight - ScrollSpacing && position < View.ScrollContentPresenter.ActualHeight) {
				View.ChangeVerticalScrollOffsetBy(ScrollSpeed);
			}
		}
		protected virtual bool AllowMouseMoveSelection(MouseEventArgs args) {
			return false;
		}
		protected override void OnAttached() {
			base.OnAttached();
			DataControlBase dataCtrl = DataControl;
			if(dataCtrl != null) {
				DragManager.SetAllowMouseMoveSelectionFunc(dataCtrl, new Func<MouseEventArgs, bool>((e) => this.AllowMouseMoveSelection(e)));
				DataViewDragDropManager.SetDragDropManager(dataCtrl, this);
				DragDropHelper = new RowDragDropElementHelper(CreateDragSource(this));
				scrollTimer = new DispatcherTimer();
				scrollTimer.Interval = TimeSpan.FromMilliseconds(100);
				scrollTimer.Tick += new EventHandler(scrollTimer_Tick);
				AutoExpandTimer.Tick += new EventHandler(AutoExpandTimer_Tick);
				DragManager.SetDropTargetFactory(dataCtrl, new DragDropManagerDropTargetFactory());
				DataControl.ItemsSourceChanged += new ItemsSourceChangedEventHandler(ItemsSourceChanged);
			}
			UpdateItemsSource();
		}
		protected virtual ISupportDragDrop CreateDragSource(DataViewDragDropManager dataViewDragDropManager) {
			return null;
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			if(DataControl != null) {
				scrollTimer.Tick -= new EventHandler(scrollTimer_Tick);
				AutoExpandTimer.Tick -= new EventHandler(AutoExpandTimer_Tick);
				DataControl.ItemsSourceChanged -= new ItemsSourceChangedEventHandler(ItemsSourceChanged);			 
				DragManager.RemoveAllowMouseMoveSelectionFunc(DataControl);
				DragDropHelper.Destroy();				
			}
			ClearItemsSource();
		}
		void scrollTimer_Tick(object sender, EventArgs e) {
			if(AllowScrolling) {
				this.hitElement = GetVisibleHitTestElement(LastPosition);
				InvalidateScrollingAndPerformDropToView(dragOverSourceManager, LastPosition);
			}
		}
		void AutoExpandTimer_Tick(object sender, EventArgs e) {
			if(AllowAutoExpand)
				DataControl.Dispatcher.BeginInvoke(new Action(() => PerformAutoExpand()));
		}
		protected virtual void PerformAutoExpand() { }
		protected virtual bool InternalBanDrop(int insertRowHandle, DragDropManagerBase sourceManager) {
			return false;
		}
		protected void InvalidateScrollingAndPerformDropToView(DragDropManagerBase sourceManager, Point point) {
			InvalidateScrolling(point);
			DataControl.Dispatcher.BeginInvoke(new Action(() => PerformDropToView(sourceManager)));
		}
		protected int HoverRowHandle;
		[Browsable(false)]
		public override void OnDragOver(DragDropManagerBase sourceManager, UIElement source, Point pt) {
			LastPosition = pt;
			scrollTimer.Start();
			UpdateHoverRowHandle(source, pt);
			if(IsExpandable) {
				AutoExpandTimer.Stop();
				AutoExpandTimer.Start();
			} else
				AutoExpandTimer.Stop();
			this.hitElement = GetVisibleHitTestElement(pt);
			if(AllowScrolling)
				InvalidateScrollingAndPerformDropToView(sourceManager, pt);
			else
				PerformDropToView(sourceManager);
			base.OnDragOver(sourceManager, source, pt);
		}
		protected DropTargetType GetDropTargetTypeByHitElement(DependencyObject hitElement) {
			return GetDropTargetType(GetRowContentElementByHitElement(hitElement));
		}
		protected virtual FrameworkElement GetRowContentElementByHitElement(DependencyObject hitElement) {
			return GetRowElement(hitElement);
		}
		void UpdateHoverRowHandle(UIElement source, Point pt) {
			HoverRowHandle = GetOverRowHandle(source, pt);
		}
		protected virtual int GetOverRowHandle(UIElement source, Point pt) {
			return GridControl.InvalidRowHandle;
		}
		protected virtual bool IsExpandable { 
			get {
				return false;
			} 
		}
		protected internal GridViewHitInfoBase GetHitInfo(IndependentMouseEventArgs e) {
			return GetHitInfo(e.OriginalSource as DependencyObject);
		}
		protected virtual GridViewHitInfoBase GetHitInfo(DependencyObject element) {
			return null;
		}
		protected virtual bool BanDrop(int insertRowHandle, GridViewHitInfoBase hitInfo, DragDropManagerBase sourceManager, DropTargetType dropTargetType) {
			DragDropDragOverEventArgs e = RaiseDragOverEvent(hitInfo, sourceManager, dropTargetType);
			sourceManager.SetDragInfoVisibility(e.ShowDragInfo);
			sourceManager.SetDropMarkerVisibility(e.ShowDropMarker);
			return DropEventIsLocked = e.Handled ? !e.AllowDrop : !AllowDrop || InternalBanDrop(insertRowHandle, sourceManager);
		}
		protected abstract DragDropDragOverEventArgs RaiseDragOverEvent(GridViewHitInfoBase hitInfo, DragDropManagerBase sourceManager, DropTargetType dropTargetType);
		protected void PerformDropToView(DragDropManagerBase sourceManager) {
			if(IsDragLeaved) return;
			sourceManager.SetDropTargetType(GetDropTargetTypeByHitElement(HitElement));
			if(!sourceManager.IsDragging) return;
			PerformDropToViewCore(sourceManager);
		}
		protected virtual void PerformDropToViewCore(DragDropManagerBase sourceManager) { }
		protected internal override void OnDragLeave() {
			StopScrollTimer();
			StopAutoExpandTimer();
			HoverRowHandle = GridControl.InvalidRowHandle;
			MouseDownHitInfo = null;
			base.OnDragLeave();
		}
		private void StopAutoExpandTimer() {
			if(AutoExpandTimer.IsEnabled)
				AutoExpandTimer.Stop();
		}
		void StopScrollTimer() {
			if(scrollTimer.IsEnabled)
				scrollTimer.Stop();
		}
		protected class DragDropHitTestResult : DragDropObjectBase {
			public UIElement Element { get; private set; }
			public DragDropHitTestResult(DragDropManagerBase manager)
				: base(manager) {
			}
			public HitTestResultBehavior CallBack(HitTestResult result) {
				Element = result.VisualHit as UIElement;
				if(Element == null || !UIElementHelper.IsVisibleInTree(Element as FrameworkElement) || !Element.IsHitTestVisible) {
					return HitTestResultBehavior.Continue;
				}
				return HitTestResultBehavior.Stop;
			}
		}
		protected UIElement GetVisibleHitTestElement(Point pt) {
			DragDropHitTestResult result = new DragDropHitTestResult(this);
#if SL
			FrameworkElement root = LayoutHelper.FindRoot(DataControl) as FrameworkElement;
			Rect rect = LayoutHelper.GetRelativeElementRect(DataControl, root);
			Point location;
			if(root.FlowDirection == FlowDirection.LeftToRight) {
				location = rect.TopLeft();
			} else {
				location = rect.TopRight();
			}
			System.Windows.Controls.Primitives.Popup popup = root as System.Windows.Controls.Primitives.Popup;
			if(popup!=null)
				PointHelper.Offset(ref location, popup.HorizontalOffset, popup.VerticalOffset);
			PointHelper.Offset(ref location, pt.X, pt.Y);
			HitTestHelper.HitTest(DataControl, null, new HitTestResultCallback(result.CallBack), new PointHitTestParameters(location), true);
#else
			VisualTreeHelper.HitTest(DataControl, null, new HitTestResultCallback(result.CallBack), new PointHitTestParameters(pt));
#endif
			return result.Element;
		}
		protected FrameworkElement GetRowElement(DependencyObject hitElement) {
			DataViewHitTestVisitorBase visitor = CreateFindRowElementHitTestVisitor(this);
			return GetElementAcceptVisitor(hitElement, visitor);
		}
		protected virtual DataViewHitTestVisitorBase CreateFindRowElementHitTestVisitor(DataViewDragDropManager dataViewDragDropManager) {
			return null;
		}
		protected FrameworkElement GetDataAreaElement(DependencyObject hitElement) {
			DataViewHitTestVisitorBase visitor = CreateFindDataAreaElementHitTestVisitor(this);
			return GetElementAcceptVisitor(hitElement, visitor);
		}
		protected virtual DataViewHitTestVisitorBase CreateFindDataAreaElementHitTestVisitor(DataViewDragDropManager dataViewDragDropManager) {
			return null;
		}
		protected internal virtual FrameworkElement GetElementAcceptVisitor(DependencyObject hitElement, DataViewHitTestVisitorBase visitor) { 
			return null; 
		}
		protected override FrameworkElement CreateLogicalOwner() {
			return View;
		}
		protected override bool CanShowDropMarker() {
			return !View.RenderSize.IsZero();
		}
		protected void SetAddRowsDropInfo(DragDropManagerBase sourceManager, int insertRowHandle, DependencyObject hitElement) {
			sourceManager.SetDropTargetType(DropTargetType.DataArea);
			sourceManager.ShowDropMarker(GetDataAreaElement(hitElement), TableDragIndicatorPosition.None);
		}
		protected void AddRows(DragDropManagerBase sourceManager, int insertRowHandle, DependencyObject hitElement) {
			DataControl.BeginDataUpdate();
			foreach(object row in sourceManager.DraggingRows)
				AddRow(sourceManager, row, insertRowHandle);
			DataControl.EndDataUpdate();
		}
		protected virtual void AddRow(DragDropManagerBase sourceManager, object row, int insertRowHandle) { }
		protected DropTargetType GetDropTargetType(FrameworkElement row) {
			return GetDropTargetType(GetDragIndicatorPositionForRowElement(row));
		}
		protected DropTargetType GetDropTargetType(TableDragIndicatorPosition indicatorPosition) {
			switch(indicatorPosition) {
				case TableDragIndicatorPosition.Top:
					return DropTargetType.InsertRowsBefore;
				case TableDragIndicatorPosition.Bottom:
					return DropTargetType.InsertRowsAfter;
				case TableDragIndicatorPosition.InRow:
					return DropTargetType.InsertRowsIntoNode;
				default:
					return DropTargetType.None;
			}
		}
		public bool RestoreSelection {
			get {
				return DataControl.ItemsSource is System.Collections.Specialized.INotifyCollectionChanged || DataControl.ItemsSource is IBindingList;
			}
		}
		protected virtual TableDragIndicatorPosition GetDragIndicatorPositionForRowElement(FrameworkElement rowElement) {
			return TableDragIndicatorPosition.None;
		}
		protected virtual void ItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e) {
			UpdateItemsSource();
		}
		void UpdateItemsSource() {
			if(DataControl != null) {
				itemsSourceCore = GetItemsSource();
			}
			else {
				ClearItemsSource();
			}
		}
		protected virtual IList GetItemsSource() {
			if(DataControl.ItemsSource is IListSource)
				return ((IListSource)DataControl.ItemsSource).GetList();
			if(DataControl.ItemsSource is ICollectionView)
				return ((ICollectionView)DataControl.ItemsSource).SourceCollection as IList;
			return DataControl.ItemsSource as IList;
		}
		protected virtual void ValidateItemsSource() {
			if(ItemsSource == null && DataControl.ItemsSource != null)
				throw new InvalidOperationException(GetItemsSourceErrorText());
		}
		internal virtual string GetItemsSourceErrorText() { 
			return string.Empty; 
		}
		void ClearItemsSource() {
			itemsSourceCore = null;
		}
	}
}
