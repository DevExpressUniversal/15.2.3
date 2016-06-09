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
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class BaseSplitterItem : Thumb {
		#region static
		public static readonly DependencyProperty DragIncrementProperty;
		public static readonly DependencyProperty KeyboardIncrementProperty;
		public static readonly DependencyProperty OrientationProperty;
		static BaseSplitterItem() {
			var dProp = new DependencyPropertyRegistrator<BaseSplitterItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("DragIncrement", ref DragIncrementProperty, (double)1.0);
			dProp.Register("KeyboardIncrement", ref KeyboardIncrementProperty, (double)10.0);
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal,
				(dObj, e) => ((BaseSplitterItem)dObj).OnOrientationChanged((Orientation)e.NewValue));
		}
		static void OnDragStarted(object sender, DragStartedEventArgs e) {
			(sender as BaseSplitterItem).OnDragStarted(e);
		}
		static void OnDragCompleted(object sender, DragCompletedEventArgs e) {
			(sender as BaseSplitterItem).OnDragCompleted(e);
		}
		static void OnDragDelta(object sender, DragDeltaEventArgs e) {
			(sender as BaseSplitterItem).OnDragDelta(e);
		}
		static protected bool IsResizableItem(BaseLayoutItem item, bool isHorizontal) {
			return LayoutItemsHelper.IsResizable(item, isHorizontal) && item.Visibility != System.Windows.Visibility.Collapsed;
		}
		#endregion
		ResizeData _resizeData;
		public BaseSplitterItem() {
			Focusable = false;
			IsTabStop = false;
		}
		protected bool IsActivated {get;private set;}
		protected bool IsHorizontal {
			get { return Orientation == System.Windows.Controls.Orientation.Horizontal; }
		}
		public virtual void Activate() {
			if(!IsActivated) {
				IsActivated = true;
				SubscribeThumbEvents();
			}
		}
		public virtual void Deactivate() {
			UnsubscribeThumbEvents();
			IsActivated = false;
		}
		void SubscribeThumbEvents() {
			DragStarted += OnDragStarted;
			DragDelta += OnDragDelta;
			DragCompleted += OnDragCompleted;
		}
		void UnsubscribeThumbEvents() {
			DragStarted -= OnDragStarted;
			DragDelta -= OnDragDelta;
			DragCompleted -= OnDragCompleted;
		}
		public DockLayoutManager Manager { get; private set; }
		public LayoutGroup LayoutGroup { get; protected set; }
		void CancelResize() {
			SetDefinitionLength(_resizeData.Definition1, _resizeData.OriginalDefinition1Length);
			SetDefinitionLength(_resizeData.Definition2, _resizeData.OriginalDefinition2Length);
			ResetResizing();
		}
		void ResetResizing() {
			if(!RedrawContent) {
				ResizePreviewHelper.EndResizing();
				resizePreviewHelper = null;
			}
			_resizeData = null;
			ReleaseMouseCapture();
		}
		GridLength GetLength(DefinitionBase definition) {
			return definition is ColumnDefinition ? ((ColumnDefinition)definition).Width : ((RowDefinition)definition).Height;
		}
		double GetActualLength(DefinitionBase definition) {
			ColumnDefinition column = definition as ColumnDefinition;
			if(column != null) return column.ActualWidth;
			return ((RowDefinition)definition).ActualHeight;
		}
		void GetDeltaConstraints(out double minDelta, out double maxDelta) {
			double actualLength1 = GetActualLength(_resizeData.Definition1);
			double actualLength2 = GetActualLength(this._resizeData.Definition2);
			double def1MinSize = DefinitionsHelper.UserMinSizeValueCache(_resizeData.Definition1);
			double def1MaxSize = DefinitionsHelper.UserMaxSizeValueCache(_resizeData.Definition1);
			double def2MinSize = DefinitionsHelper.UserMinSizeValueCache(_resizeData.Definition2);
			double def2MaxSize = DefinitionsHelper.UserMaxSizeValueCache(_resizeData.Definition2);
			if(_resizeData.SplitterIndex == _resizeData.Definition1Index) {
				def1MinSize = Math.Max(def1MinSize, _resizeData.SplitterLength);
			}
			else if(_resizeData.SplitterIndex == _resizeData.Definition2Index) {
				def2MinSize = Math.Max(def2MinSize, this._resizeData.SplitterLength);
			}
			if(_resizeData.SplitBehavior == SplitBehavior.Split) {
				minDelta = -Math.Min(actualLength1 - def1MinSize, def2MaxSize - actualLength2);
				maxDelta = Math.Min(def1MaxSize - actualLength1, actualLength2 - def2MinSize);
			}
			else if(this._resizeData.SplitBehavior == SplitBehavior.Resize1) {
				minDelta = def1MinSize - actualLength1;
				maxDelta = def1MaxSize - actualLength1;
			}
			else {
				minDelta = actualLength2 - def2MaxSize;
				maxDelta = actualLength2 - def2MinSize;
			}
		}
		protected virtual GridResizeDirection GetEffectiveResizeDirection() {
			if(LayoutGroup != null)
				return (LayoutGroup.Orientation == Orientation.Horizontal) ? 
					GridResizeDirection.Columns : GridResizeDirection.Rows;
			return GridResizeDirection.Rows;
		}
		static DefinitionBase GetGridDefinition(Grid grid, int index, GridResizeDirection direction) {
			if(direction != GridResizeDirection.Columns)
				return grid.RowDefinitions[index];
			return grid.ColumnDefinitions[index];
		}
		protected virtual Grid GetParentGrid() {
			return (Grid)LayoutHelper.GetParent(this);
		}
		bool InitializeData() {
			Grid grid = GetParentGrid();
			if(grid != null) {
				this._resizeData = new ResizeData();
				this._resizeData.Grid = grid;
				this._resizeData.ResizeDirection = GetEffectiveResizeDirection();
				this._resizeData.SplitterLength = Math.Min(base.ActualWidth, base.ActualHeight);
				if(!SetupDefinitionsToResize())
					_resizeData = null;
				else {
					ResizeCalculator.Init(_resizeData.SplitBehavior);
					ResizeCalculator.Orientation = Orientation;
					if(!RedrawContent)
						ResizePreviewHelper.InitResizing(new Point(), Manager.GetViewElement(this as IUIElement));
				}
			}
			return _resizeData != null;
		}
		static bool IsStar(DefinitionBase definition) {
			return DefinitionsHelper.IsStar(definition);
		}
		internal bool KeyboardMoveSplitter(double horizontalChange, double verticalChange) {
			if(_resizeData != null) return false;
			this.InitializeData();
			if(_resizeData == null) return false;
			MoveSplitter(CorrectChange(IsHorizontal ? horizontalChange : verticalChange));
			ResetResizing();
			return true;
		}
		void MoveSplitter(double change) {
			using(new NotificationBatch(Manager)) {
					var itemsToResize = GetItemsToResize();
					if(itemsToResize != null && itemsToResize.Item1 != null && itemsToResize.Item2 != null) {
						ResizeCalculator.Resize(itemsToResize.Item1, itemsToResize.Item2, change);
					NotifySizingAction(Manager);
				}
			}
		}
		void NotifySizingAction(DockLayoutManager manager) {
			if(LayoutGroup == null || manager == null) return;
			int index = GetCurrent(LayoutGroup.Orientation == Orientation.Horizontal);
			BaseLayoutItem item1 = LayoutGroup.ItemsInternal[index - 1] as BaseLayoutItem;
			BaseLayoutItem item2 = LayoutGroup.ItemsInternal[index + 1] as BaseLayoutItem;
			DependencyProperty property = LayoutGroup.Orientation == Orientation.Horizontal ?
				BaseLayoutItem.ItemWidthProperty : BaseLayoutItem.ItemHeightProperty;
			if(item1 != null)
				NotificationBatch.Action(manager, item1, property);
			if(item2 != null)
				NotificationBatch.Action(manager, item2, property);
		}
		protected virtual double GetMin(DefinitionBase definition) {
			bool isColumn = definition is ColumnDefinition;
			BaseLayoutItem item = GetItem(definition, isColumn);
			if(item is FixedItem) return 5;
			if(item is LayoutPanel) return isColumn ? 12 : 18;
			if(item is TabbedGroup) return 24;
			if(item is LayoutControlItem) return isColumn ? 24 : 10;
			return 12;
		}
		double CorrectChange(double change) {
			if(FlowDirection != _resizeData.Grid.FlowDirection)
				change = -change;
			double min, max;
			GetDeltaConstraints(out min, out max);
			return Math.Min(Math.Max(change, min), max);
		}
		double GetActualChange(double horizontalChange, double verticalChange) {
			double dragIncrement = this.DragIncrement;
			double change = IsHorizontal ? horizontalChange : verticalChange;
			return Math.Round((double)(change / dragIncrement)) * dragIncrement;
		}
		void OnDragCompleted(DragCompletedEventArgs e) {
			if(_resizeData != null) {
				if(!RedrawContent) {
					double change = CorrectChange(GetActualChange(e.HorizontalChange, e.VerticalChange));
					MoveSplitter(change);
				}
				var itemToResize = GetItemsToResize();
				Manager.RaiseDockOperationCompletedEvent(DockOperation.Resize, itemToResize.Item1);
				Manager.RaiseDockOperationCompletedEvent(DockOperation.Resize, itemToResize.Item2);
				ResetResizing();
			}
			((ISupportBatchUpdate)Manager).EndUpdate();
		}
		bool RedrawContent { get { return Manager.RedrawContentWhenResizing; } }
		void OnDragDelta(DragDeltaEventArgs e) {
			if(this._resizeData != null) {
				double change = CorrectChange(GetActualChange(e.HorizontalChange, e.VerticalChange));
				if(RedrawContent)
					MoveSplitter(change);
				else
					ResizePreviewHelper.Resize(IsHorizontal ? new Point(change, 0) : new Point(0, change));
			}
		}
		protected virtual IResizeCalculator ResolveResizeCalculator() {
			return new LayoutResizeCalculator() { Orientation = Orientation, };
		}
		void OnDragStarted(DragStartedEventArgs e) {
			((ISupportBatchUpdate)Manager).BeginUpdate();
			CaptureMouse();
			if(InitializeData()) {
				var itemsToResize = GetItemsToResize();
				if(Manager.RaiseDockOperationStartingEvent(DockOperation.Resize, itemsToResize.Item1 ) || Manager.RaiseDockOperationStartingEvent(DockOperation.Resize, itemsToResize.Item2)) {
					ResetResizing();
					return;
				}
			}
		}
		bool IsDragStarted { get { return _resizeData != null; } }
		Tuple<BaseLayoutItem, BaseLayoutItem> GetItemsToResize() {
			Tuple<BaseLayoutItem, BaseLayoutItem> itemsToResize = new Tuple<BaseLayoutItem, BaseLayoutItem>(null, null);
			if(IsDragStarted) {
				DefinitionBase def1 = _resizeData.Definition1;
				DefinitionBase def2 = _resizeData.Definition2;
				BaseLayoutItem item1 = null;
				BaseLayoutItem item2 = null;
				if((def1 != null) && (def2 != null)) {
					bool isColumn = _resizeData.ResizeDirection == GridResizeDirection.Columns;
					item1 = GetItem(def1, isColumn);
					item2 = GetItem(def2, isColumn);
					itemsToResize = new Tuple<BaseLayoutItem, BaseLayoutItem>(item1, item2);
				}
			}
			return itemsToResize;
		}
		DevExpress.Xpf.Docking.Platform.LayoutResizingPreviewHelper resizePreviewHelper;
		DevExpress.Xpf.Docking.Platform.LayoutResizingPreviewHelper ResizePreviewHelper {
			get {
				if(resizePreviewHelper == null) {
					resizePreviewHelper = new DevExpress.Xpf.Docking.Platform.LayoutResizingPreviewHelper(Manager.GetView(LayoutGroup), LayoutGroup);
				}
				return resizePreviewHelper;
			}
		}
		IResizeCalculator resizeCalculatorCore;
		protected IResizeCalculator ResizeCalculator {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(resizeCalculatorCore == null)
					resizeCalculatorCore = ResolveResizeCalculator();
				return resizeCalculatorCore;
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			switch(e.Key) {
				case Key.Left: e.Handled = KeyboardMoveSplitter(-KeyboardIncrement, 0.0); return;
				case Key.Up: e.Handled = KeyboardMoveSplitter(0.0, -KeyboardIncrement); return;
				case Key.Right: e.Handled = KeyboardMoveSplitter(KeyboardIncrement, 0.0); return;
				case Key.Down: e.Handled = KeyboardMoveSplitter(0.0, KeyboardIncrement); break;
				case Key.Escape:
					if(_resizeData == null) break;
					CancelResize();
					e.Handled = true;
					return;
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Manager = DockLayoutManager.Ensure(this);
			UpdateCursor();
			if(LayoutGroup != null)
				BindingHelper.SetBinding(this, OrientationProperty, LayoutGroup, LayoutGroup.OrientationProperty);
		}
		void UpdateCursor() {
			if(LayoutGroup != null)
				UpdateCursor(LayoutGroup.Orientation == Orientation.Horizontal);
		}
		protected virtual void SetDefinitionLength(DefinitionBase definition, GridLength length) {
			if(length.Value < 0) return;
			bool isColumn = definition is ColumnDefinition;
			BaseLayoutItem item = GetItem(definition, isColumn);
			if(item != null)
				item.SetValue(isColumn ? BaseLayoutItem.ItemWidthProperty : BaseLayoutItem.ItemHeightProperty, length);
		}
		BaseLayoutItem GetItem(DefinitionBase definition, bool isColumn) {
			if(_resizeData == null || _resizeData.Grid == null) return null;
			if(LayoutGroup != null) {
				int index = isColumn ?
					_resizeData.Grid.ColumnDefinitions.IndexOf(definition as ColumnDefinition) :
					_resizeData.Grid.RowDefinitions.IndexOf(definition as RowDefinition);
				if(index != -1)
					return LayoutGroup.ItemsInternal[index] as BaseLayoutItem;
			}
			return null;
		}
		void SetLengths(double definition1Pixels, double definition2Pixels) {
			if(System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) {
				definition1Pixels = Math.Round(definition1Pixels, 2);
				definition2Pixels = Math.Round(definition2Pixels, 2);
			}
			switch(_resizeData.SplitBehavior) {
				case SplitBehavior.Split:
					SetDefinitionLength(_resizeData.Definition1, new GridLength(definition1Pixels, GridUnitType.Star));
					SetDefinitionLength(_resizeData.Definition2, new GridLength(definition2Pixels, GridUnitType.Star));
					break;
				case SplitBehavior.Resize1:
					SetDefinitionLength(_resizeData.Definition1, new GridLength(definition1Pixels));
					SetDefinitionLength(_resizeData.Definition2, new GridLength(definition2Pixels, GridUnitType.Star));
					break;
				case SplitBehavior.Resize2:
					SetDefinitionLength(_resizeData.Definition1, new GridLength(definition1Pixels, GridUnitType.Star));
					SetDefinitionLength(_resizeData.Definition2, new GridLength(definition2Pixels));
					break;
				default:
					SetDefinitionLength(_resizeData.Definition1, new GridLength(definition1Pixels, GridUnitType.Star));
					break;
			}
		}
		protected virtual int GetNext(int current) {
			if(LayoutGroup != null) {
				int indexInternal = LayoutGroup.ItemsInternal.IndexOf(this);
				BaseLayoutItem nextItem = LayoutGroup.ItemsInternal[indexInternal + 1] as BaseLayoutItem;
				int index = LayoutGroup.Items.IndexOf(nextItem);
				for(int i = index; i < LayoutGroup.Items.Count; i++) {
					nextItem = LayoutGroup.Items[i];
					if(IsResizableItem(nextItem, IsHorizontal))
						return LayoutGroup.ItemsInternal.IndexOf(nextItem);
				}
			}
			return current;
		}
		protected virtual int GetPrev(int current) {
			if(LayoutGroup != null) {
				int indexInternal = LayoutGroup.ItemsInternal.IndexOf(this);
				BaseLayoutItem prevItem = LayoutGroup.ItemsInternal[indexInternal - 1] as BaseLayoutItem;
				int index = LayoutGroup.Items.IndexOf(prevItem);
				for(int i = index; i >= 0; i--) {
					prevItem = LayoutGroup.Items[i];
					if(IsResizableItem(prevItem, IsHorizontal))
						return LayoutGroup.ItemsInternal.IndexOf(prevItem);
				}
			}
			return current;
		}
		protected virtual int GetCurrent(bool isColumns) {
			return (int)GetValue(isColumns ? Grid.ColumnProperty : Grid.RowProperty);
		}
		bool SetupDefinitionsToResize() {
			bool isColumns = _resizeData.ResizeDirection == GridResizeDirection.Columns;
			int span = (int)base.GetValue(isColumns ? Grid.ColumnSpanProperty : Grid.RowSpanProperty);
			if(span == 1) {
				int current = GetCurrent(isColumns);
				int prev = GetPrev(current);
				int next = GetNext(current); 
				int defCount = isColumns ? _resizeData.Grid.ColumnDefinitions.Count : _resizeData.Grid.RowDefinitions.Count;
				if(prev >= 0 && prev != current && next < defCount && next != current) {
					_resizeData.SplitterIndex = current;
					_resizeData.Definition1Index = prev;
					_resizeData.Definition1 = GetGridDefinition(_resizeData.Grid, prev, _resizeData.ResizeDirection);
					_resizeData.OriginalDefinition1Length = DefinitionsHelper.UserSizeValueCache(_resizeData.Definition1);
					_resizeData.OriginalDefinition1ActualLength = GetActualLength(_resizeData.Definition1);
					_resizeData.Definition2Index = next;
					_resizeData.Definition2 = GetGridDefinition(_resizeData.Grid, next, _resizeData.ResizeDirection);
					_resizeData.OriginalDefinition2Length = DefinitionsHelper.UserSizeValueCache(_resizeData.Definition2);
					_resizeData.OriginalDefinition2ActualLength = GetActualLength(_resizeData.Definition2);
					bool def1IsStar = IsStar(_resizeData.Definition1);
					bool def2IsStar = IsStar(_resizeData.Definition2);
					if(def1IsStar && def2IsStar) {
						_resizeData.SplitBehavior = SplitBehavior.Split;
					}
					else {
						_resizeData.SplitBehavior = def1IsStar ? SplitBehavior.Resize2 :
							def2IsStar ? SplitBehavior.Resize1 : SplitBehavior.PixelSplit;
					}
					return true;
				}
			}
			return false;
		}
		protected virtual void OnOrientationChanged(Orientation orientation) {
			if(IsActivated) UpdateCursor();
		}
		protected virtual void UpdateCursor(bool horz) {
			if(CanResize()) Cursor = horz ? Cursors.SizeWE : Cursors.SizeNS;
			else ClearValue(CursorProperty);
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateCursor();
		}
		bool CanResize() {
			 bool isColumns = GetEffectiveResizeDirection() == GridResizeDirection.Columns;
			int span = (int)base.GetValue(isColumns ? Grid.ColumnSpanProperty : Grid.RowSpanProperty);
			Grid grid = GetParentGrid();
			if(span == 1) {
				int current = GetCurrent(isColumns);
				int prev = GetPrev(current);
				int next = GetNext(current); 
				int defCount = isColumns ? grid.ColumnDefinitions.Count : grid.RowDefinitions.Count;
				if(prev >= 0 && prev != current && next < defCount && next != current) {
					return true;
				}
			}
			return false;
		}
		public double DragIncrement {
			get { return (double)base.GetValue(DragIncrementProperty); }
			set { base.SetValue(DragIncrementProperty, value); }
		}
		public double KeyboardIncrement {
			get { return (double)base.GetValue(KeyboardIncrementProperty); }
			set { base.SetValue(KeyboardIncrementProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
	}
}
