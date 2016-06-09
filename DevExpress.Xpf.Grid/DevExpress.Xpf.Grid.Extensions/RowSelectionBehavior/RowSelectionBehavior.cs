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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Data;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid.Native;
namespace DevExpress.Xpf.Grid {
	public class SelectAllColumn : GridColumn {
		public static readonly string ColumnFieldName = "_SelectAllColumn";
		public const string CheckHeaderTemplate = @"
         <DataTemplate 
xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
xmlns:dxe=""http://schemas.devexpress.com/winfx/2008/xaml/editors""
xmlns:dxg=""http://schemas.devexpress.com/winfx/2008/xaml/grid""
>
            <dxe:CheckEdit IsChecked=""{Binding 
Path=DataContext.RowSelectionBehavior.IsAllRowsSelected, Mode=TwoWay, 
RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type dxg:GridColumnHeader}}}"" />
        </DataTemplate>
";
		public const string CheckCellTemplate = @"
                        <DataTemplate
xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
xmlns:dxe=""http://schemas.devexpress.com/winfx/2008/xaml/editors""
xmlns:dxg=""http://schemas.devexpress.com/winfx/2008/xaml/grid""
>
                            <dxe:CheckEdit x:Name=""PART_Editor""
                                      HorizontalAlignment=""Center""
                                      VerticalAlignment=""Center"" />
                        </DataTemplate>
";
		public static readonly DependencyProperty RowSelectionBehaviorProperty =
			DependencyProperty.Register("RowSelectionBehavior", typeof(RowSelectionBehavior), typeof(SelectAllColumn), new PropertyMetadata(null));
		public RowSelectionBehavior RowSelectionBehavior {
			get { return (RowSelectionBehavior)GetValue(RowSelectionBehaviorProperty); }
			set { SetValue(RowSelectionBehaviorProperty, value); }
		}
		static SelectAllColumn() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectAllColumn), new FrameworkPropertyMetadata(typeof(SelectAllColumn)));
		}
		public SelectAllColumn() {
			FieldName = SelectAllColumn.ColumnFieldName;
			UnboundType = UnboundColumnType.Boolean;
			AllowSorting = DefaultBoolean.False;
			AllowAutoFilter = false;
			AllowBestFit = DefaultBoolean.False;
			AllowColumnFiltering = DefaultBoolean.False;
			AllowGrouping = DefaultBoolean.False;
			HeaderTemplate = ParseTemplateString(SelectAllColumn.CheckHeaderTemplate);
			CellTemplate = ParseTemplateString(SelectAllColumn.CheckCellTemplate);
		}
		DataTemplate ParseTemplateString(string template) {
#if !SL
			return (DataTemplate)XamlReader.Parse(template);
#else
			return (DataTemplate)XamlReader.Load(template);
#endif
		}
	}
	[SelectedItemsSourceBrowsable]
	public class RowSelectionBehavior : Behavior<GridViewBase>, IRowSelectionBehavior {
#if DEBUGTEST
		internal static int BehaviorsCount = 0;
#endif
		public RowSelectionBehavior()
			: base() {
#if DEBUGTEST
			BehaviorsCount++;
#endif
			SelectedItems = new SelectedItemsCollection(this);
		}
		public static readonly DependencyProperty IsAllRowsSelectedProperty =
			DependencyProperty.Register("IsAllRowsSelected", typeof(bool?), typeof(RowSelectionBehavior),
			new PropertyMetadata(false, (d, e) => ((RowSelectionBehavior)d).OnIsAllRowsSelectedChanged()));
		public bool? IsAllRowsSelected {
			get { return (bool?)GetValue(IsAllRowsSelectedProperty); }
			set { SetValue(IsAllRowsSelectedProperty, value); }
		}
		GridControl GridControl { get { return AssociatedObject.Grid; } }
		ItemsSourceChangedHelper ItemsSourceChangedHelper { get; set; }
		object IRowSelectionBehavior.Source { get { return Source; } }
		IList Source {
			get {
#if !SL
				if(GridControl.ItemsSource is DataTable)
					return ((DataTable)GridControl.ItemsSource).DefaultView;
#endif
				return GridControl.ItemsSource as IList;
			}
		}
		public virtual int ItemsSourceCount { get { return Source == null ? 0 : Source.Count; } }
		protected bool AllowSelectionSynchronize {
			get {
				if(AssociatedObject is SelectionView) {
					return (AssociatedObject as SelectionView).AllowSelectionSynchronize;
				}
				return false;
			}
		}
		bool lockUpdateIsAllRowsSelected = false;
		bool lockSelectionChanged = false;
		internal SelectedItemsCollection SelectedItems;
		void OnIsAllRowsSelectedChanged() {
			if(IsAllRowsSelected == null)
				return;
			if(AllowSelectionSynchronize) {
				if(IsAllRowsSelected.Value)
					GridControl.SelectAll();
				else
					GridControl.UnselectAll();
			} else {
				lockUpdateIsAllRowsSelected = true;
				SetSelectionColumnCellsValue(false, new Predicate<int>(delegate(int handle) { return IsAllRowsSelected.Value; }));
				GridControl.RefreshData();
				lockUpdateIsAllRowsSelected = false;
			}
		}
		void SetSelectionColumnCellsValue(bool allowSetCellValue, Predicate<int> predicate) {
			for(int i = 0; i < ItemsSourceCount; i++) {
				int handle = GridControl.GetRowHandleByListIndex(i);
				if(allowSetCellValue) {
					GridControl.SetCellValue(handle, SelectAllColumn.ColumnFieldName, predicate(handle));
				} else {
					if(predicate(handle)) {
						if(!SelectedItems.Contains(i)) {
							SelectedItems.Add(i);
						}
					} else {
						SelectedItems.Remove(i);
					}
				}
			}
		}
		protected override void OnAttached() {
			base.OnAttached();
			GridControl.CustomUnboundColumnData += new GridColumnDataEventHandler(OnCustomUnboundColumnData);
			GridControl.FilterChanged += new RoutedEventHandler(OnFilterChanged);
			GridControl.Columns.CollectionChanged += new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
			AssociatedObject.CellValueChanging += new CellValueChangedEventHandler(OnCellValueChanging);
			GridControl.SelectionChanged += new GridSelectionChangedEventHandler(OnSelectionChanged);
			UpdateSelectAllColumn();
			if(AssociatedObject is SelectionView) {
				ItemsSourceChangedHelper = new ItemsSourceChangedHelper(this);
				ItemsSourceChangedHelper.CollectionChanged += new ListChangedEventHandler(OnItemsSourceCollectionChanged);
				ItemsSourceChangedHelper.SubscribeOnItemsSourceChanged(AssociatedObject as SelectionView);
			}
		}
		void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateSelectAllColumn();
		}
		void UpdateSelectAllColumn() {
			foreach(GridColumn column in GridControl.Columns) {
				if(column is SelectAllColumn) {
					(column as SelectAllColumn).RowSelectionBehavior = this;
				}
			}
		}
		void OnItemsSourceCollectionChanged(object sender, ListChangedEventArgs e) {
			switch(e.ListChangedType) {
				case ListChangedType.ItemAdded:
					SelectedItems.GrowIndexes(e.NewIndex);
					UpdateIsAllRowsSelectedProperty();
					break;
				case ListChangedType.ItemDeleted:
					SelectedItems.ShrinkIndexes(e.OldIndex);
					UpdateIsAllRowsSelectedProperty();
					break;
				case ListChangedType.Reset:
					SelectedItems.Clear();
					UpdateIsAllRowsSelectedProperty();
					break;
			}
		}
		void OnSelectionChanged(object sender, GridSelectionChangedEventArgs e) {
			if(lockSelectionChanged || !AllowSelectionSynchronize)
				return;
			List<int> selectedHandles = new List<int>(GridControl.GetSelectedRowHandles());
			bool hasGroupping = false;
			for(int i = 0; i < selectedHandles.Count; i++) {
				if(selectedHandles[i] < 0) {
					hasGroupping = true;
					break;
				}
			}
			if((GridControl.VisibleRowCount == selectedHandles.Count) && hasGroupping) {
				GridControl.BeginSelection();
				AssociatedObject.DataControl.UnselectAll();
				for(int i = 0; i < ItemsSourceCount; i++)
					GridControl.SelectItem(i);
				GridControl.EndSelection();
				return;
			}
			SelectedItems.Clear();
			for(int i = selectedHandles.Count - 1; i >= 0; i--) {
				if(GridControl.IsGroupRowHandle(selectedHandles[i])) {
					selectedHandles.RemoveAt(i);
				} else {
					int visibleIndex = GridControl.GetListIndexByRowHandle(selectedHandles[i]);
					if(!SelectedItems.Contains(visibleIndex))
						SelectedItems.Add(visibleIndex);
				}
			}
			SetSelectionColumnCellsValue(true, new Predicate<int>(delegate(int handle) { return selectedHandles.Contains(handle); }));
		}
		protected override void OnDetaching() {
			if(AssociatedObject is SelectionView) {
				ItemsSourceChangedHelper.CollectionChanged -= new ListChangedEventHandler(OnItemsSourceCollectionChanged);
				ItemsSourceChangedHelper.UnsubscribeFromItemsSourceChanged(AssociatedObject as SelectionView);
			}
			GridControl.Columns.CollectionChanged -= new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
			GridControl.CustomUnboundColumnData -= new GridColumnDataEventHandler(OnCustomUnboundColumnData);
			GridControl.FilterChanged -= new RoutedEventHandler(OnFilterChanged);
			AssociatedObject.CellValueChanging -= new CellValueChangedEventHandler(OnCellValueChanging);
			GridControl.SelectionChanged -= new GridSelectionChangedEventHandler(OnSelectionChanged);
			base.OnDetaching();
		}
		void OnCellValueChanging(object sender, CellValueChangedEventArgs e) {
			if(e.Column.FieldName == SelectAllColumn.ColumnFieldName)
				AssociatedObject.PostEditor();
		}
		void OnFilterChanged(object sender, RoutedEventArgs e) {
			UpdateIsAllRowsSelectedProperty();
		}
		void OnCustomUnboundColumnData(object sender, GridColumnDataEventArgs e) {
			if(e.Column != null && e.Column.FieldName == SelectAllColumn.ColumnFieldName) {
				if(e.IsGetData) {
					e.Value = GetIsSelected(e.ListSourceRowIndex);
				}
				if(e.IsSetData) {
					SetIsSelected(e.ListSourceRowIndex, (bool)e.Value);
				}
			}
		}
		void UpdateIsAllRowsSelectedProperty() {
			if(lockUpdateIsAllRowsSelected)
				return;
			bool allSelected = true;
			bool allUnselected = true;
			allSelected = SelectedItems.Count >= ItemsSourceCount;
			allUnselected = SelectedItems.Count == 0;
			if(!allSelected && !allUnselected)
				IsAllRowsSelected = null;
			if(allSelected)
				IsAllRowsSelected = true;
			if(allUnselected)
				IsAllRowsSelected = false;
		}
		bool GetIsSelected(int listIndex) {
			return SelectedItems.Contains(listIndex);
		}
		void SetIsSelected(int listIndex, bool value) {
			lockSelectionChanged = true;
			int rowHandle = GridControl.GetRowHandleByListIndex(listIndex);
			if(value) {
				if(!SelectedItems.Contains(listIndex)) {
					SelectedItems.Add(listIndex);
					if(AllowSelectionSynchronize)
						GridControl.SelectItem(rowHandle);
					GridControl.SetCellValue(rowHandle, SelectAllColumn.ColumnFieldName, true);
				}
			} else {
				if(SelectedItems.Contains(listIndex)) {
					SelectedItems.Remove(listIndex);
					if(AllowSelectionSynchronize)
						GridControl.UnselectItem(rowHandle);
					GridControl.SetCellValue(rowHandle, SelectAllColumn.ColumnFieldName, false);
				}
			}
			lockSelectionChanged = false;
			UpdateIsAllRowsSelectedProperty();
		}
		#region SelectedItemsSource
		Action<IList, IList> onSelectionChanged;
		static RowSelectionBehavior() {
			SelectionControlWrapper.Wrappers.Add(typeof(RowSelectionBehavior), typeof(GridRowSelectionBehaviorSelectionControlWrapper));
		}
		class GridRowSelectionBehaviorSelectionControlWrapper : SelectionControlWrapper {
			RowSelectionBehavior behavior;
			public GridRowSelectionBehaviorSelectionControlWrapper(RowSelectionBehavior behavior) {
				this.behavior = behavior;
			}
			public override void SubscribeSelectionChanged(Action<IList, IList> a) {
				behavior.onSelectionChanged = a;
			}
			public override void UnsubscribeSelectionChanged() {
				behavior.onSelectionChanged = null;
			}
			public override IList GetSelectedItems() { return behavior.SelectedItems.SelectedItemValues; }
			public override void ClearSelection() { behavior.IsAllRowsSelected = false; }
			public override void SelectItem(object item) { behavior.SetIsSelected(behavior.Source.IndexOf(item), true); }
			public override void UnselectItem(object item) { behavior.SetIsSelected(behavior.Source.IndexOf(item), false); }
		}
		internal class SelectedItemsCollection : Collection<int> {
			ArrayList itemValues = new ArrayList();
			RowSelectionBehavior behavior;
			public SelectedItemsCollection(RowSelectionBehavior behavior) {
				this.behavior = behavior;
			}
			protected override void ClearItems() {
				IList oldItemValues = itemValues;
				itemValues = new ArrayList();
				base.ClearItems();
				if(behavior.onSelectionChanged != null)
					behavior.onSelectionChanged(new object[] { }, oldItemValues);
			}
			protected override void InsertItem(int index, int item) {
				object newItemValue = behavior.Source[item];
				itemValues.Insert(index, newItemValue);
				base.InsertItem(index, item);
				if(behavior.onSelectionChanged != null)
					behavior.onSelectionChanged(new object[] { newItemValue }, new object[] { });
			}
			protected override void RemoveItem(int index) {
				object oldItemValue = itemValues[index];
				itemValues.RemoveAt(index);
				base.RemoveItem(index);
				if(behavior.onSelectionChanged != null)
					behavior.onSelectionChanged(new object[] { }, new object[] { oldItemValue });
			}
			protected override void SetItem(int index, int item) {
				object oldItemValue = itemValues[index];
				object newItemValue = behavior.Source[item];
				itemValues[index] = newItemValue;
				base.SetItem(index, item);
				if(behavior.onSelectionChanged != null)
					behavior.onSelectionChanged(new object[] { newItemValue }, new object[] { oldItemValue });
			}
			public void GrowIndexes(int newItemIndex) {
				for(int i = 0; i < Count; ++i) {
					if(this[i] >= newItemIndex)
						base.SetItem(i, this[i] + 1);
				}
			}
			public void ShrinkIndexes(int oldItemIndex) {
				int oldIndex = -1;
				for(int i = 0; i < Count; ++i) {
					if(this[i] == oldItemIndex) {
						oldIndex = i;
						base.SetItem(i, -1);
					} else if(this[i] > oldItemIndex) {
						base.SetItem(i, this[i] - 1);
					}
				}
				if(oldIndex >= 0)
					RemoveAt(oldIndex);
			}
			public IList SelectedItemValues { get { return itemValues; } }
		}
		#endregion
	}
	public class ItemsSourceChangedHelper {
		IRowSelectionBehavior behavior;
		public ItemsSourceChangedHelper(IRowSelectionBehavior behavior) {
			this.behavior = behavior;
		}
		SelectionView View { get; set; }
		GridControl Grid { get; set; }
		object ItemsSource { get; set; }
		public event ListChangedEventHandler CollectionChanged;
		void RaiseCollectionChanged(ListChangedEventArgs e) {
			if(CollectionChanged != null) {
				CollectionChanged(this, e);
			}
		}
		public void SubscribeOnItemsSourceChanged(SelectionView view) {
			View = view;
			view.GridChanged += new EventHandler(view_OnGridChanged);
			SubscribeOnNewGrid();
		}
		public void UnsubscribeFromItemsSourceChanged(SelectionView view) {
			view.GridChanged -= new EventHandler(view_OnGridChanged);
			UnsubscribeFromOldGrid();
			View = null;
		}
		void view_OnGridChanged(object sender, EventArgs e) {
			UnsubscribeFromOldGrid();
			SubscribeOnNewGrid();
		}
		void SubscribeOnNewGrid() {
			if(View.Grid != null) {
				View.Grid.ItemsSourceChanged += new ItemsSourceChangedEventHandler(Grid_ItemsSourceChanged);
				Grid = View.Grid;
				SubscribeOnGridItemsSourceChanged();
			}
		}
		void UnsubscribeFromOldGrid() {
			if(Grid != null) {
				Grid.ItemsSourceChanged -= new ItemsSourceChangedEventHandler(Grid_ItemsSourceChanged);
				UnsubscribeFromGridItemsSourceChanged();
				Grid = null;
			}
		}
		void SubscribeOnGridItemsSourceChanged() {
			ItemsSource = behavior.Source;
			SubscribeOnCollectionChanged(ItemsSource);
		}
		void UnsubscribeFromGridItemsSourceChanged() {
			if(ItemsSource != null) {
				UnsubscribeFromCollectionChanged(ItemsSource);
				ItemsSource = null;
			}
		}
		void SubscribeOnCollectionChanged(object itemsSource) {
			if(itemsSource is INotifyCollectionChanged) {
				(itemsSource as INotifyCollectionChanged).CollectionChanged += new NotifyCollectionChangedEventHandler(ItemsSourceChangedHelper_CollectionChanged);
				return;
			}
			if(itemsSource is IBindingList) {
				(itemsSource as IBindingList).ListChanged += new ListChangedEventHandler(ItemsSourceChangedHelper_ListChanged);
				return;
			}
		}
		void UnsubscribeFromCollectionChanged(object itemsSource) {
			if(itemsSource is INotifyCollectionChanged) {
				(itemsSource as INotifyCollectionChanged).CollectionChanged -= new NotifyCollectionChangedEventHandler(ItemsSourceChangedHelper_CollectionChanged);
				return;
			}
			if(itemsSource is IBindingList) {
				(itemsSource as IBindingList).ListChanged -= new ListChangedEventHandler(ItemsSourceChangedHelper_ListChanged);
				return;
			}
		}
		void ItemsSourceChangedHelper_ListChanged(object sender, ListChangedEventArgs e) {
			RaiseCollectionChanged(e);
		}
		void ItemsSourceChangedHelper_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ListChangedType listChangedType = ListChangedType.Reset;
			if(e.Action == NotifyCollectionChangedAction.Add) {
				listChangedType = ListChangedType.ItemAdded;
			}
			if(e.Action == NotifyCollectionChangedAction.Move) {
				listChangedType = ListChangedType.ItemMoved;
			}
			if(e.Action == NotifyCollectionChangedAction.Remove) {
				listChangedType = ListChangedType.ItemDeleted;
			}
			RaiseCollectionChanged(new ListChangedEventArgs(listChangedType, e.NewStartingIndex, e.OldStartingIndex));
		}
		void Grid_ItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e) {
			UnsubscribeFromGridItemsSourceChanged();
			SubscribeOnGridItemsSourceChanged();
		}
	}
	public class InvertSelectionAction : SelectionActionBase {
		public InvertSelectionAction(GridViewBase view) : base(view) { }
		public override void Execute() {
			if(view.IsRowSelected(view.FocusedRowHandle)) {
				GridControl.UnselectItem(view.FocusedRowHandle);
			} else {
				GridControl.SelectItem(view.FocusedRowHandle);
			}
		}
	}
	public class RowSelectionStrategy : SelectionStrategyRow {
		public RowSelectionStrategy(GridViewBase view) : base(view) { }
		bool isMouseLeftButtonDownEventRasing = false;
		SelectionActionBase tempSelectionAction = null;
		bool selectionActionWasChanged = false;
		SelectionView SelectionView { get { return view as SelectionView; } }
		public override void OnBeforeProcessKeyDown(KeyEventArgs e) {
			if(SelectionView.DisableStandardSelection) return;
			base.OnBeforeProcessKeyDown(e);
		}
		public override void OnBeforeMouseLeftButtonDown(DependencyObject originalSource) {
			if(SelectionView.DisableStandardSelection) return;
			if(Mouse.RightButton != MouseButtonState.Pressed)
				isMouseLeftButtonDownEventRasing = true;
			var column = view.GetColumnByTreeElement(originalSource);
			if((SelectionView.NavigationStyle == GridViewNavigationStyle.Cell) && column != null && column.FieldName == SelectAllColumn.ColumnFieldName) {
				SelectionView.SetEditorSetInactiveAfterClick(false);
				return;
			}
			base.OnBeforeMouseLeftButtonDown(originalSource);
		}
		public override void OnAssignedToGrid() {
			if(SelectionView.DisableStandardSelection)
				SelectionView.DataControl.UnselectAll();
			else
				base.OnAssignedToGrid();
		}
		public override void OnDataSourceReset() {
			if(SelectionView.DisableStandardSelection)
				SelectionView.DataControl.UnselectAll();
			else
				base.OnDataSourceReset();
		}
		public override void OnAfterMouseLeftButtonDown(IDataViewHitInfo hitInfo) {
			base.OnAfterMouseLeftButtonDown(hitInfo);
			if(isMouseLeftButtonDownEventRasing) {
				if(tempSelectionAction != null) {
					if(SelectionAction == null) {
						SelectionAction = tempSelectionAction;
					}
					tempSelectionAction = null;
				}
				if(SelectionAction != null && selectionActionWasChanged) {
					ExecuteSelectionAction();
					selectionActionWasChanged = false;
				}
				isMouseLeftButtonDownEventRasing = false;
			}
		}
		public override void CreateMouseSelectionActions(int rowHandle, bool isIndicator) {
			base.CreateMouseSelectionActions(rowHandle, isIndicator);
			if((SelectionAction is OnlyThisSelectionAction) && (GridView.DataControl.GetSelectedRowHandles().Length > 1) && isMouseLeftButtonDownEventRasing) {
				tempSelectionAction = SelectionAction;
				SelectionAction = null;
				selectionActionWasChanged = true;
			}
		}
		public override void OnFocusedColumnChanged() {
			base.OnFocusedColumnChanged();
			if(selectionActionWasChanged && view.DataControl.CurrentColumn.FieldName == SelectAllColumn.ColumnFieldName) {
				SelectionAction = new InvertSelectionAction(GridView);
			}
		}
	}
	public class SelectionView : TableView {
		public static readonly DependencyProperty SelectedItemsSourceProperty =
			DependencyProperty.Register("SelectedItemsSource", typeof(IList), typeof(SelectionView), new PropertyMetadata(null,
				(d, e) => ((SelectionView)d).OnSelectedItemsSourceChanged(e)));
		public static readonly DependencyProperty AllowSelectionSynchronizeProperty =
			DependencyProperty.Register("AllowSelectionSynchronize", typeof(bool), typeof(SelectionView),
			new PropertyMetadata(true, (d, e) => ((SelectionView)d).OnAllowSelectionSynchronizeChanged()));
		public static readonly DependencyProperty DisableStandardSelectionProperty =
			DependencyProperty.Register("DisableStandardSelection", typeof(bool), typeof(SelectionView),
			new PropertyMetadata(false, (d, e) => ((SelectionView)d).OnAllowSelectionSynchronizeChanged()));
		public IList SelectedItemsSource {
			get { return (IList)GetValue(SelectedItemsSourceProperty); }
			set { SetValue(SelectedItemsSourceProperty, value); }
		}
		public bool AllowSelectionSynchronize {
			get { return (bool)GetValue(AllowSelectionSynchronizeProperty); }
			set { SetValue(AllowSelectionSynchronizeProperty, value); }
		}
		public bool DisableStandardSelection {
			get { return (bool)GetValue(DisableStandardSelectionProperty); }
			set { SetValue(DisableStandardSelectionProperty, value); }
		}
		public event EventHandler GridChanged;
		FrameworkElement pressedCell = null;
		bool AllowInternalSelection { get { return (!AllowSelectionSynchronize) && (NavigationStyle == GridViewNavigationStyle.Row); } }
		protected virtual void OnSelectedItemsSourceChanged(DependencyPropertyChangedEventArgs e) {
			RowSelectionBehavior rowSelectionBehavior = GetRowSelectionBehavior();
			if(rowSelectionBehavior == null) return;
			SelectionAttachedBehavior.SetSelectedItemsSource(rowSelectionBehavior, SelectedItemsSource);
		}
		RowSelectionBehavior GetRowSelectionBehavior() { return Interaction.GetBehaviors(this).OfType<RowSelectionBehavior>().FirstOrDefault(); }
		protected override SelectionStrategyBase CreateSelectionStrategy() {
			return new RowSelectionStrategy(this);
		}
		void RaiseGridChanged() {
			if(GridChanged != null) {
				GridChanged(this, EventArgs.Empty);
			}
		}
		protected override void SetDataControl(DataControlBase newValue) {
			if(DataControl == newValue)
				return;
			Behavior rowSelectionBehavior = GetRowSelectionBehavior();
			if(rowSelectionBehavior != null) {
				SelectionAttachedBehavior.SetSelectedItemsSource(rowSelectionBehavior, null);
				Interaction.GetBehaviors(this).Remove(rowSelectionBehavior);
			}
			base.SetDataControl(newValue);
			if(DataControl != null) {
				rowSelectionBehavior = new RowSelectionBehavior();
				Interaction.GetBehaviors(this).Add(rowSelectionBehavior);
				SelectionAttachedBehavior.SetSelectedItemsSource(rowSelectionBehavior, SelectedItemsSource);
			}
			RaiseGridChanged();
		}
		void OnAllowSelectionSynchronizeChanged() {
			if(Grid == null) return;
			Grid.UnselectAll();
		}
		internal void SetEditorSetInactiveAfterClick(bool value) {
			EditorSetInactiveAfterClick = value;
		}
		protected override void OnPreviewMouseUp(MouseButtonEventArgs e) {
			base.OnPreviewMouseUp(e);
			if(!AllowInternalSelection)
				return;
			FrameworkElement element = GetCellElementByMouseEventArgs(e);
			if((element == null) || (pressedCell != element)) {
				pressedCell = null;
				return;
			}
			EditGridCellData cellData = element.DataContext as EditGridCellData;
			if((cellData == null) || (cellData.Column.FieldName != SelectAllColumn.ColumnFieldName))
				return;
			Grid.SetCellValue(cellData.RowData.RowHandle.Value, cellData.Column as GridColumn, !Convert.ToBoolean(Grid.GetCellValue(cellData.RowData.RowHandle.Value, cellData.Column as GridColumn)));
		}
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseDown(e);
			if(AllowInternalSelection) {
				pressedCell = GetCellElementByMouseEventArgs(e);
			}
		}
	}
	public class OneClickRowSelectionStrategy : SelectionStrategyRow {
		public OneClickRowSelectionStrategy(GridViewBase view) : base(view) { }
		public override void CreateMouseSelectionActions(int rowHandle, bool isIndicator) {
			if(rowHandle == GridControl.AutoFilterRowHandle || rowHandle == GridControl.NewItemRowHandle) {
				return;
			}
			SelectionAction = new InvertSelectionAction(GridView);
		}
		public override void OnBeforeProcessKeyDown(KeyEventArgs e) {
		}
		public override void OnBeforeMouseLeftButtonDown(DependencyObject originalSource) {
			SelectionAction = new InvertSelectionAction(GridView);
			if(GridView.FocusedRowHandle == GridView.GetRowHandleByTreeElement(originalSource))
				ExecuteSelectionAction();
		}
	}
	public class OneClickSelectionView : TableView {
		protected override SelectionStrategyBase CreateSelectionStrategy() {
			return new OneClickRowSelectionStrategy(this);
		}
	}
}
namespace DevExpress.Xpf.Grid.Native {
	public interface IRowSelectionBehavior {
		object Source { get; }
	}
}
