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
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Data;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Native;
using DependencyPropertyHelper = System.Windows.DependencyPropertyHelper;
#if SL
using DevExpress.Data;
#else
using System.ComponentModel;
#endif
namespace DevExpress.Xpf.Grid.LookUp.Native {
	public class GridControlVisualClientOwner : VisualClientOwner {
		new LookUpEdit Editor { get { return base.Editor as LookUpEdit; } }
		internal GridControl Grid { get { return InnerEditor as GridControl; } }
		DataViewBase View { get { return Grid != null ? Grid.View : null; } }
		SearchControl SearchControl { get { return View != null ? View.SearchControl : null; } }
		LookUpEditStyleSettings StyleSettings { get { return (LookUpEditStyleSettings)ActualPropertyProvider.GetProperties(Editor).StyleSettings; } }
		protected override bool IsLoaded { get { return base.IsLoaded && Grid != null; } }
		LookUpEditBasePropertyProvider PropertyProvider { get { return ActualPropertyProvider.GetProperties(Editor) as LookUpEditBasePropertyProvider; } }
		bool ShouldSyncProperties { get { return IsLoaded && Editor.EditMode != EditMode.InplaceInactive; } }
		bool IsInAutoFilterRowEditing { get { return IsLoaded && View.FocusedRowHandle == DataControlBase.AutoFilterRowHandle && View.ActiveEditor != null; } }
		bool IsInnerPopupOpened { get { return IsLoaded && (View.IsContextMenuOpened || View.IsColumnFilterOpened); } }
		internal bool IsSearchControlFocused { get { return SearchControl != null && SearchControl.IsKeyboardFocusWithin; } }
		internal bool IsSearchTextEmpty {
			get {
				if (SearchControl == null)
					return true;
				SearchControl.DoValidate();
				return string.IsNullOrEmpty(SearchControl.SearchText);
			}
		}
		bool IsServerMode { get { return LookUpEditHelper.GetIsServerMode(Editor); } }
		IList<object> TotalSelectedItems { get; set; }
		protected override void SubscribeEvents() {
			if (Grid == null)
				return;
			Grid.MouseLeftButtonUp += GridMouseUp;
			Grid.View.FocusedRowHandleChanged += FocusedRowChanged;
			Grid.SelectionChanged += GridSelectionChanged;
			base.SubscribeEvents();
		}
		protected override void UnsubscribeEvents() {
			if (Grid == null)
				return;
			Grid.MouseLeftButtonUp -= GridMouseUp;
			Grid.View.FocusedRowHandleChanged -= FocusedRowChanged;
			Grid.SelectionChanged -= GridSelectionChanged;
			base.UnsubscribeEvents();
		}
		void GridSelectionChanged(object sender, GridSelectionChangedEventArgs e) {
			if (LookUpEditHelper.GetIsSingleSelection(Editor))
				return;
			List<object> addedItems = null, removedItems = null;
			if (!Grid.IsGroupRowHandle(e.ControllerRow)) {
				if (e.Action == CollectionChangeAction.Add) {
					addedItems = new List<object> { GetRowKey(e.ControllerRow) };
					TotalSelectedItems = TotalSelectedItems.Union(CustomItem.FilterCustomItems(addedItems)).ToList();
				}
				else if (e.Action == CollectionChangeAction.Remove) {
					removedItems = new List<object> { GetRowKey(e.ControllerRow) };
					TotalSelectedItems = TotalSelectedItems.Except(CustomItem.FilterCustomItems(removedItems)).ToList();
				}
				else if (e.Action == CollectionChangeAction.Refresh) {
					TotalSelectedItems = Grid.GetSelectedRowHandles().Select(GetRowKey).ToList();
				}
			}
			LookUpEditHelper.RaisePopupContentSelectionChangedEvent(Editor, removedItems, addedItems);
		}
		public override bool IsClosePopupWithCancelGesture(Key key, ModifierKeys modifiers) {
			return !IsInnerPopupOpened && base.IsClosePopupWithCancelGesture(key, modifiers);
		}
		public override bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return !IsInAutoFilterRowEditing && !IsInnerPopupOpened && base.IsClosePopupWithAcceptGesture(key, modifiers);
		}
		void FocusedRowChanged(object sender, FocusedRowHandleChangedEventArgs e) {
			LookUpEditHelper.RaisePopupContentSelectionChangedEvent(Editor, new List<object> { null }, new List<object> { Grid.CurrentItem });
		}
		void GridMouseUp(object sender, MouseButtonEventArgs e) {
			if (e.LeftButton != MouseButtonState.Released)
				return;
			int rowHandle = Grid.View.GetRowHandleByMouseEventArgs(e);
			if (IsDataRowRowHandle(rowHandle) && LookUpEditHelper.GetClosePopupOnMouseUp(Editor))
				Editor.ClosePopup();
		}
		bool IsDataRowRowHandle(int rowHandle) {
			if (Grid.IsGroupRowHandle(rowHandle))
				return false;
			return rowHandle != DataControlBase.InvalidRowHandle && rowHandle != DataControlBase.NewItemRowHandle &&
				   rowHandle != DataControlBase.AutoFilterRowHandle;
		}
		public override void InnerEditorMouseMove(object sender, MouseEventArgs e) {
			if (IsDragging(sender) || !CalcAllowItemHighlighting(Editor) || Grid.View.IsColumnChooserVisible)
				return;
			Point mousePosition = e.GetPosition(Grid);
			if (lastMousePosition != mousePosition) {
				int rowHandle = Grid.View.GetRowHandleByMouseEventArgs(e);
				if (IsDataRowRowHandle(rowHandle) && !Grid.View.IsEditing)
					SetFocusedRowHandleInternal(rowHandle);
			}
			lastMousePosition = mousePosition;
			base.InnerEditorMouseMove(sender, e);
		}
		bool CalcAllowItemHighlighting(LookUpEditBase editor) {
			bool allow = LookUpEditHelper.GetIsAllowItemHighlighting(editor);
			if (!allow)
				return false;
			return LookUpEditHelper.GetClosePopupOnMouseUp(Editor);
		}
		bool IsDragging(object sender) {
			DependencyObject topLevel = LayoutHelper.GetTopLevelVisual(sender as FrameworkElement);
			if (topLevel == null)
				return false;
			return DragManager.GetIsDragging(topLevel);
		}
		void SetFocusedRowHandleInternal(int rowHandle) {
			View.ScrollIntoViewLocker.DoLockedAction(() => { Grid.View.SetFocusedRowHandle(rowHandle); });
		}
		public GridControlVisualClientOwner(PopupBaseEdit editor)
			: base(editor) {
		}
		public override void PopupClosed() {
			base.PopupClosed();
			LookUpEditHelper.FocusEditCore(Editor);
		}
		public override void PopupContentLoaded() {
			base.PopupContentLoaded();
			SyncProperties(true);
		}
		public override void SyncProperties(bool syncDataSource) {
			if (!ShouldSyncProperties)
				return;
			if (syncDataSource)
				SetupDataSource();
			if (IsServerMode)
				Grid.ExtraFilter = LookUpEditHelper.GetActualFilterCriteria(Editor);
			SyncValues();
			Dispatcher.BeginInvoke(new Action(() => { if (IsLoaded) Grid.InvalidateMeasure(); }));
			SyncSelectedItems(syncDataSource);
		}
		void SetupDataSource() {
			object dataSource = ((ISelectorEdit)Editor).GetPopupContentItemsSource();
			if (Grid.ItemsSource != dataSource) {
				var valueSource = DependencyPropertyHelper.GetValueSource(Grid, DataControlBase.ItemsSourceProperty).BaseValueSource;
				if (valueSource == BaseValueSource.Default || valueSource == BaseValueSource.Local) {
					Grid.ItemsSource = dataSource;
					((BaseGridController)Grid.DataController).Do(x => x.KeepFocusedRowOnUpdate = false);
				}
			}
			if (Editor.AutoPopulateColumns)
				Grid.PopulateColumns(true, true);
		}
		public override bool ProcessKeyDownInternal(KeyEventArgs e) {
			if (e.Handled)
				return true;
			if (!Editor.IsPopupOpen && !CanClosePopup(e)) {
				if (OpenPopupAndReraiseTextInput(e))
					return true;
			}
			if (View != null && !View.IsContextMenuOpened && !View.IsColumnFilterOpened && IsDataRowRowHandle(View.FocusedRowHandle) && e.Key == Key.Enter && Editor.IsPopupOpen) {
				e.Handled = true;
				Editor.ClosePopup();
				return true;
			}
			if (View != null && !IsNavigationKey(e.Key)) {
				View.ViewBehavior.ProcessPreviewKeyDown(e);
				if (!e.Handled)
					View.ProcessKeyDown(e);
			}
			return true;
		}
		bool CanClosePopup(System.Windows.Input.KeyEventArgs e) {
			return e.Key == Key.Enter || e.Key == Key.Escape || e.Key == Key.F4;
		}
		protected virtual bool OpenPopupAndReraiseTextInput(System.Windows.Input.KeyEventArgs e) {
			if (PropertyProvider.IsTextEditable || !Editor.ImmediatePopup
				|| e.Key == Key.Tab || e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.System || e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.NumLock || e.Key == Key.CapsLock || e.Key == Key.Capital)
				return false;
			Editor.ShowPopup();
			return true;
		}
		protected virtual bool IsNavigationKey(Key key) {
			return (key == Key.Right) || (key == Key.Left) || (key == Key.Tab) ||
				   (key == Key.Home) || (key == Key.End);
		}
		public override object GetSelectedItem() {
			if (!IsLoaded)
				return null;
			return GetRowKey(Grid.View.FocusedRowHandle);
		}
		object GetRowKey(int rowHandle) {
			if (!IsServerMode)
				return Grid.GetRow(rowHandle);
			return string.IsNullOrEmpty(Editor.ValueMember) ? Grid.DataController.GetRowKey(rowHandle) : Grid.DataController.GetValueEx(rowHandle, Editor.ValueMember);
		}
		public override void SyncValues(bool resetTotals = false) {
			base.SyncValues(resetTotals);
			if (!IsLoaded)
				return;
			StyleSettings.SyncValues(Editor, Grid);
			SyncSelectedItems(resetTotals);
		}
		void SyncSelectedItems(bool updateTotals) {
			if (updateTotals) {
				if (!LookUpEditHelper.GetIsSingleSelection(Editor))
					TotalSelectedItems = LookUpEditHelper.GetEditValue(Editor) as IList<object> ?? new List<object>();
				else
					TotalSelectedItems = new List<object>();
			}
		}
		protected override FrameworkElement FindEditor() {
			return LayoutHelper.FindElementByName(LookUpEditHelper.GetPopupContentOwner(Editor).Child, "PART_GridControl");
		}
		protected override void SetupEditor() {
			if (Grid == null)
				return;
			InitializeSearchPanel();
			Grid.ShowBorder = false;
			Grid.View.NavigationStyle = GridViewNavigationStyle.Row;
			UpdateViewProperty(DataViewBase.IsSynchronizedWithCurrentItemProperty, false);
			UpdateViewProperty(DataViewBase.ShowColumnHeadersProperty, StyleSettings.ShowColumnHeaders);
			UpdateViewProperty(DataViewBase.ShowTotalSummaryProperty, StyleSettings.ShowTotalSummary);
			UpdateViewProperty(GridViewBase.ShowGroupPanelProperty, StyleSettings.ShowGroupPanel);
			UpdateViewProperty(DataViewBase.AllowSortingProperty, StyleSettings.AllowSorting);
			UpdateViewProperty(GridViewBase.AllowGroupingProperty, StyleSettings.AllowGrouping);
			UpdateViewProperty(DataViewBase.AllowColumnFilteringProperty, StyleSettings.AllowColumnFiltering);
			UpdateProperty(Grid, DataControlBase.SelectionModeProperty, LookUpEditHelper.GetIsSingleSelection(Editor) ? MultiSelectMode.None : MultiSelectMode.Row);
			SyncSelectedItems(true);
		}
		protected virtual void InitializeSearchPanel() {
			Grid.View.ShowSearchPanelMode = StyleSettings.ShowSearchPanel ? ShowSearchPanelMode.Always : ShowSearchPanelMode.Never;
			CreateColumnProvider();
			Grid.View.SearchPanelFindFilter = PropertyProvider.FilterCondition;
			Grid.View.SearchPanelFindMode = PropertyProvider.FindMode;
			Grid.View.ShowSearchPanelFindButton = PropertyProvider.GetFindButtonPlacement() != EditorPlacement.None;
			Grid.View.SearchPanelImmediateMRUPopup = Grid.View.SearchPanelImmediateMRUPopup.HasValue && Grid.View.SearchPanelImmediateMRUPopup.Value;
			Grid.View.SearchString = null;
		}
		protected void CreateColumnProvider() {
			if (GridControlColumnProviderBase.GetColumnProvider(Grid) != null)
				return;
			GridControlColumnProvider searchPanelColumnProvider = new GridControlColumnProvider { AllowColumnsHighlighting = true, FilterByColumnsMode = PropertyProvider.FilterByColumnsMode, IsSearchLookUpMode = true };
			if (PropertyProvider.FilterByColumnsMode == FilterByColumnsMode.Custom)
				searchPanelColumnProvider.CustomColumns = new ObservableCollection<string>(LookUpEditHelper.GetHighlightedColumns(Editor));
			GridControlColumnProviderBase.SetColumnProvider(Grid, searchPanelColumnProvider);
		}
		protected void UpdateViewProperty(DependencyProperty property, object value) {
			UpdateViewProperty(Grid.View, property, value);
		}
		void UpdateViewProperty(DependencyObject obj, DependencyProperty property, object value) {
			if (obj is GridViewBase) {
				UpdateProperty(obj, property, value);
			}
		}
		void UpdateProperty(DependencyObject obj, DependencyProperty property, object value) {
			ValueSource source = DependencyPropertyHelper.GetValueSource(obj, property);
#if !SL
			if (source.BaseValueSource == BaseValueSource.Unknown || source.BaseValueSource == BaseValueSource.Default) {
#else
				if (source.BaseValueSource == BaseValueSource.Default) {
#endif
				obj.SetValue(property, value);
			}
		}
		public override IEnumerable GetSelectedItems() {
			return TotalSelectedItems ?? new List<object>();
		}
		Point lastMousePosition;
	}
	public class GridControlColumnProvider : GridControlColumnProviderBase {
	}
}
