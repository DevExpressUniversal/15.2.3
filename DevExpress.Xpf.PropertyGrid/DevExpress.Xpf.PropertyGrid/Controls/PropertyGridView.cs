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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.PropertyGrid.Internal;
using DevExpress.Xpf.Editors.Helpers;
using System.ComponentModel;
using DevExpress.Data.Filtering.Helpers;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.PropertyGrid {
	public enum PropertyGridViewColumnMode { Default, Modified, BestFit }
	public class PropertyGridView : ItemsControl, IVisualClient, ISupportInitialize, INavigationSupport {			   
		protected static readonly DependencyPropertyKey PropertyGridPropertyKey; 
		public static readonly DependencyProperty PropertyGridProperty; 
		protected static readonly DependencyPropertyKey HasCellEditorErrorPropertyKey;
		public static readonly DependencyProperty HasCellEditorErrorProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		static PropertyGridView() {
			Type ownerType = typeof(PropertyGridView);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			NavigationManager.NavigationModeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(NavigationMode.Auto));
			SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(RowDataBase), typeof(PropertyGridView), new PropertyMetadata(null));
			PropertyGridPropertyKey = DependencyPropertyManager.RegisterReadOnly("PropertyGrid", typeof(PropertyGridControl), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridView)d).OnPropertyGridChanged((PropertyGridControl)e.OldValue)));
			HasCellEditorErrorPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasCellEditorError", typeof(bool), typeof(PropertyGridView), new FrameworkPropertyMetadata(false, (d, e) => ((PropertyGridView)d).OnHasCellEditorErrorChanged((bool)e.OldValue)));
			ThemeManager.TreeWalkerProperty.OverrideMetadata(typeof(PropertyGridView), new FrameworkPropertyMetadata(new PropertyChangedCallback((d, e) => ((PropertyGridView)d).OnTreeWalkerChanged((ThemeTreeWalker)e.OldValue, (ThemeTreeWalker)e.NewValue))));
			HasCellEditorErrorProperty = HasCellEditorErrorPropertyKey.DependencyProperty;
			PropertyGridProperty = PropertyGridPropertyKey.DependencyProperty;
		}		
		internal PropertyGridRenderResourceProvider ResourceProvider { get; private set; }
		public PropertyGridResizingStrategy ResizingStrategy { get; private set; }	  
		public PropertyGridMenuHelper MenuHelper { get; private set; }
		public ModifyCollectionHelper CollectionHelper { get; private set; }
		public virtual void ShowEditor(bool selectAll) { CellEditorOwner.CurrentCellEditor.Do(x => x.ShowEditorIfNotVisible(selectAll)); }
		public virtual void HideEditor() { CellEditorOwner.CurrentCellEditor.Do(x => x.CancelEditInVisibleEditor()); }
		public virtual void CloseEditor() { CellEditorOwner.CurrentCellEditor.Do(x => x.CommitEditor(true)); }
		public virtual void PostEditor() { CellEditorOwner.CurrentCellEditor.Do(x => x.PostEditor()); }
		public PropertyGridControl PropertyGrid {
			get { return (PropertyGridControl)GetValue(PropertyGridProperty); }
			protected internal set { this.SetValue(PropertyGridPropertyKey, value); }
		}
		protected internal SelectionStrategy SelectionStrategy { get { return PropertyGrid.SelectionStrategy; } }
		protected internal ItemsPresenter ItemsPresenter { get; private set; }
		protected internal ScrollViewer ScrollViewer { get; private set; }
		protected internal PGVirtualizingStackPanel VirtualizingPanel { get; private set; }
		protected internal bool MouseIsOutOfBounds { get; private set; }
		public ImmediateActionsManager ImmediateActionsManager { get; private set; }
		internal BarItems BarItems { get; set; }
		public CellEditorOwner CellEditorOwner { get; private set; }
		protected Dictionary<string, BaseValidationError> ValidationErrors { get; private set; }
		public RowDataBase SelectedItem {
			get { return (RowDataBase)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public bool HasCellEditorError {
			get { return (bool)GetValue(HasCellEditorErrorProperty); }
			protected internal set { this.SetValue(HasCellEditorErrorPropertyKey, value); }
		}
		protected internal bool HasErrorInActiveEditor {
			get { return CellEditorOwner.IsActiveEditorHaveValidationError(); }
		}
		protected internal double ActualRowWidth {
			get { return ItemsPresenter == null ? ActualWidth : ItemsPresenter.ActualWidth; }
		}		
		public PropertyGridView() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			CellEditorOwner = new CellEditorOwner(this);
			ImmediateActionsManager = new ImmediateActionsManager(this);
			ValidationErrors = new Dictionary<string, BaseValidationError>();
			BarItems = new BarItems(this);
			LayoutUpdated += OnLayoutUpdated;
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.Refresh, OnRefreshCommandExecuted, OnRefreshCommandCanExecute));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.Reset, OnResetCommandExecuted, OnResetCommandCanExecute));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.AddItem, OnAddItemCommandExecuted, OnAddItemCommandCanExecute));
			CommandBindings.Add(new CommandBinding(PropertyGridCommands.RemoveItem, OnRemoveItemCommandExecuted, OnRemoveItemCommandCanExecute));
			ResizingStrategy = new PropertyGridResizingStrategy(this);
			UpdateResourceProvider();
			MenuHelper = new PropertyGridMenuHelper(this);
			CollectionHelper = new ModifyCollectionHelper(this);
			PropertyGridHelper.SetView(this, this);
		}
		protected virtual void OnTreeWalkerChanged(ThemeTreeWalker oldValue, ThemeTreeWalker newValue) {
			UpdateResourceProvider();
		}
		protected virtual void UpdateResourceProvider() {
			ResourceProvider = PropertyGridRenderResourceProvider.From(this);
		}
		public void SetValidationError(RowHandle handle, BaseValidationError error) {
			if (handle == null)
				return;
			SetValidationError(PropertyGrid.DataView.GetFieldNameByHandle(handle), error);
		}
		public void SetValidationError(string fieldName, BaseValidationError error) {
			if (fieldName != null) {
				if (error == null) {
					if (ValidationErrors.ContainsKey(fieldName))
						ValidationErrors.Remove(fieldName);
				} else {
					ValidationErrors[fieldName] = error;
				}
			}
			var oldValue = HasCellEditorError;
			HasCellEditorError = ValidationErrors.Count != 0;
			if (oldValue == HasCellEditorError)
				OnHasCellEditorErrorChanged(oldValue);
		}
		public BaseValidationError GetValidationError(RowHandle handle) {
			return GetValidationError(PropertyGrid.DataView.GetFieldNameByHandle(handle));
		}
		public BaseValidationError GetValidationError(string fieldName) {
			return (fieldName != null && ValidationErrors.ContainsKey(fieldName)) ? ValidationErrors[fieldName] : null;
		}
		protected internal virtual void OnRefreshCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
			var handle = (SelectedItem as RowDataBase).If(x=>!x.IsCategory).With(x => x.Handle) ?? GetRowHandleForCommandEventArgs(e.OriginalSource, e.Parameter);
			if (handle != null && PropertyGrid != null) {
				var fieldName = PropertyGrid.DataView.GetFieldNameByHandle(handle);
				PropertyGrid.DataView.Invalidate(handle);
				PropertyGrid.RowDataGenerator.Invalidate(fieldName);
			}
		}
		protected internal virtual void OnResetCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
			var handle = (SelectedItem as RowDataBase).If(x=>!x.IsCategory).With(x => x.Handle) ?? GetRowHandleForCommandEventArgs(e.OriginalSource, e.Parameter);
			if (CellEditorOwner.CanHideEditor && CellEditorOwner.CurrentCellEditor != null)
				CellEditorOwner.CurrentCellEditor.HideEditor(true);
			if (handle != null && PropertyGrid != null) {
				var exception = PropertyGrid.RowDataGenerator.DataView.ResetValue(handle);
				if (exception != null) {
					var error = new CellValidationError(exception.Message, exception, XtraEditors.DXErrorProvider.ErrorType.Default);
					SetValidationError(handle, error);
					(CellEditorOwner.CurrentCellEditor as CellEditor).Do(x => x.SetValidationErrorInternal(error));
				}
			}
		}
		protected internal virtual void OnAddItemCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
			if (!OnAddItemCommandCanExecuteCore())
				return;
			var data = (SelectedItem as RowDataBase).If(x=>!x.IsCategory);
			if (data == null || data.Handle == null || PropertyGrid == null || PropertyGrid.RowDataGenerator == null || PropertyGrid.RowDataGenerator.DataView == null) {
				return;
			}
			PropertyGrid.RowDataGenerator.DataView.SetValue(data.Handle, data.Value);
		}
		protected internal virtual void OnRemoveItemCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
		}
		protected internal virtual void EnqueueShowEditor(string fullPath) {
			ImmediateActionsManager.EnqueueAction(() => ShowEditor(false));
		}
		protected internal void OnAddItemCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}
		bool OnAddItemCommandCanExecuteCore() {
			var data = (SelectedItem as RowDataBase).If(x=>x.IsCategory);
			if (data == null || data.Handle == null || PropertyGrid == null || PropertyGrid.RowDataGenerator == null || PropertyGrid.RowDataGenerator.DataView == null) {
				return false;
			}
			return PropertyGrid.RowDataGenerator.DataView.IsNewInstanceInitializer(data.Handle) && data.Value != null;
		}
		protected internal void OnRemoveItemCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
		}
		protected internal void OnRefreshCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}
		protected internal void OnResetCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
			var handle = (SelectedItem as RowDataBase).If(x=>!x.IsCategory).With(x => x.Handle) ?? GetRowHandleForCommandEventArgs(e.OriginalSource, e.Parameter);
			if (handle != null) {
				e.CanExecute = PropertyGrid.With(x => x.RowDataGenerator).If(x => x.DataView.CanResetValue(handle)).ReturnSuccess();
			}
		}
		private RowHandle GetRowHandleForCommandEventArgs(object sender, object parameter) {
			RowHandle result = null;
			var parametrizedResult = (parameter as string).With(x => PropertyGrid.With(y => y.RowDataGenerator.DataView.GetHandleByFieldName(x)));
			if (sender is FrameworkElement) {
				FrameworkElement item = (FrameworkElement)sender;
				var dataContextResult = (item.DataContext as RowDataBase).With(x => x.Handle);
				result = parametrizedResult ?? dataContextResult;
			}
			else {
				result = parametrizedResult;
			}
			return result;
		}
		protected virtual void OnLayoutUpdated(object sender, EventArgs e) {
			PropertyGrid.Do(x => x.UpdateData());
			ImmediateActionsManager.ExecuteActions();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
		}
		protected virtual void OnPropertyGridChanged(PropertyGridControl oldValue) {
		}
		#region RESIZING        
		protected virtual void OnRowMinHeightChanged(double oldValue) {
		}					
		#endregion        
		protected override Size MeasureOverride(Size constraint) {
			return base.MeasureOverride(constraint);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();			
			ScrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;			
			ItemsPresenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
			ScrollViewer.Do(x => VirtualizingPanel = (PGVirtualizingStackPanel)LayoutHelper.FindElementByType(x, typeof(VirtualizingPanel)));
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return base.IsItemItsOwnContainerOverride(item);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new RowControl();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			RowControlBase rcBase = element as RowControlBase;
			RowDataBase rData = item as RowDataBase;
			if (rcBase != null && rData != null) {
				rcBase.IsCategory = rData.IsCategory;
				rcBase.IsLastElement = Items.IndexOf(rData) == Items.Count - 1;
				rcBase.RowData = rData;
				rcBase.OwnerView = this;
			}
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			RowControlBase rcBase = element as RowControlBase;
			if (rcBase != null) {
				rcBase.RowData = null;
			}
		}
		public bool CommitEditing() {
			return !HasErrorInActiveEditor;
		}
		public bool PerformNavigationOnLeftButtonDown(DependencyObject originalSource) {
			return true;
		}
		protected internal virtual bool CheckCanNavigate(bool shouldCommitEditor = true) {
			if (CellEditorOwner.InternalCanCommitEditing && CellEditorOwner.CurrentCellEditor != null && shouldCommitEditor) 
				CellEditorOwner.CurrentCellEditor.CommitEditor();
			return CellEditorOwner.CurrentCellEditor.Return(x => !x.Edit.IsValueChanged || !HasErrorInActiveEditor, () => true);
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			if (!IsKeyboardFocusWithin && CellEditorOwner.IsActiveEditorHaveValidationError())
				CellEditorOwner.ProcessIsKeyboardFocusWithinChanged();
		}		
#if DEBUGTEST
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseDown(e);
			if (DevExpress.Xpf.PropertyGrid.Tests.TestHelper.TestsRunning)
				ProcessMouseLeftButtonDown(e);
		}		
#endif
		public void ProcessMouseButtonUp(MouseButtonEventArgs e) {			
		}
		public void ProcessMouseLeftButtonDown(MouseButtonEventArgs e) {
			var mouseData = (e.OriginalSource as DependencyObject).With(x => PropertyGridHelper.GetRowData(x));
			var currentData = CellEditorOwner.CurrentCellEditor.With(x => PropertyGridHelper.GetRowData(x));
			if (!CheckCanNavigate(mouseData != currentData)) {
				CellEditorOwner.CurrentCellEditor.With(x => x.Edit).Do(x => x.SetKeyboardFocus());
				if (CellEditorOwner.CurrentCellEditor == null || !TreeHelper.GetParents(e.OriginalSource as DependencyObject).Contains(CellEditorOwner.CurrentCellEditor)) {
					e.Handled = true;
					return;
				}	  
			}
			if (e.ChangedButton != MouseButton.Left)
				return;
			var source = LayoutHelper.FindLayoutOrVisualParentObject(e.OriginalSource as DependencyObject, dobj => PropertyGridHelper.GetView(dobj) == this);
			var presenter = source == null ? null : PropertyGridHelper.GetEditorPresenter(source as DependencyObject);
			var rowControl = source == null ? null : PropertyGridHelper.GetRowControl(source as DependencyObject);
			if (rowControl != null) {
				var handle = rowControl.RowData.Handle;
				var path = PropertyGrid.DataView.GetFieldNameByHandle(handle);
				SelectionStrategy.SelectViaPath(rowControl.RowData.FullPath, path == rowControl.RowData.FullPath);
				if (presenter != null && presenter.RowControl == rowControl)
					rowControl.EnsureSelection(presenter);
			}
			CellEditorOwner.ProcessMouseLeftButtonDown(e);							
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			SelectionStrategy.SelectViaPath(PropertyGrid.SelectedPropertyPath, true);
		}
		protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnPreviewGotKeyboardFocus(e);
		}
		protected override bool HandlesScrolling { get { return true; } }
		protected override void OnKeyDown(KeyEventArgs e) {
			this.CellEditorOwner.ProcessKeyDown(e);
		}
		protected internal virtual bool ShouldPassKeyDownInLookUpMode(KeyEventArgs e) {
			return CellEditorOwner.ProcessKeyForLookUp(e);
		}
		protected internal void ProcessKeyDown(KeyEventArgs e) {
			if (CanProcessNavigation())
				PropertyGrid.NavigationManager.ProcessNavigation(e);
			MenuHelper.ProcessKeyDown(e);
		}
		protected bool CanProcessNavigation() {
			return !MenuHelper.MenuIsOpen;
		}
		public virtual bool ShouldProcessCollectionChangedAsynchronously() { return true; }
		internal RowControlBase GetRowControl(ItemContainerGenerator generator, RowDataBase item) {
			if (item == null)
				return null;
			return generator.ContainerFromItem(item) as RowControlBase;
		}
		protected virtual void OnHasCellEditorErrorChanged(bool oldValue) {
			if (HasErrorInActiveEditor)
				(CellEditorOwner.ActiveEditor as DependencyObject).Do(MouseEventLockHelper.SubscribeMouseEvents);
		}
		DevExpress.Xpf.Bars.Native.WeakList<RowHandle> invalidationList = new Bars.Native.WeakList<RowHandle>();
		bool suppressInvalidations = false;
		bool IVisualClient.AllowCommitOnValidationAttributeError() {
			return PropertyGrid.Return(p => p.AllowCommitOnValidationAttributeError, () => false);
		}
		AllowExpandingMode IVisualClient.GetAllowExpandingMode(RowHandle handle) {
			var definition = PropertyGrid.PropertyBuilder.GetDefinition(PropertyGrid.DataView, handle, PropertyGrid.DataView is CategoryDataView) as PropertyDefinition;
			if (definition == null)
				return AllowExpandingMode.Default;
			return definition.AllowExpanding.HasValue ? definition.AllowExpanding.Value : PropertyGrid.AllowExpanding;
		}
		void IVisualClient.Invalidate(RowHandle handle) {
			if (suppressInvalidations)
				return;
			Invalidate(handle);
		}
		bool IVisualClient.InvokeRequired {
			get { return !Dispatcher.CheckAccess(); }
		}
		void IVisualClient.Invoke(Delegate method, params object[] args) {
			Dispatcher.Invoke(method, args);
		}
		IInstanceInitializer IVisualClient.GetInstanceInitializer(RowHandle handle) {
			if (handle == RowHandle.Root)
				return null;
			return PropertyGrid.With(x => x.RowDataGenerator).RowDataFromHandle(handle).With(x => x.Definition).With(x => x.InstanceInitializer);
		}
		IInstanceInitializer IVisualClient.GetListItemInitializer(RowHandle handle) {
			return (PropertyGrid.With(x => x.RowDataGenerator).RowDataFromHandle(handle).With(x => x.Definition) as CollectionDefinition).With(x => x.NewItemInitializer);
		}
		bool IVisualClient.AllowInstanceInitializer(RowHandle handle) {
			var pd = (PropertyGrid.With(x => x.RowDataGenerator).RowDataFromHandle(handle).With(x => x.Definition) as PropertyDefinition);
			return pd != null && pd.AllowInstanceInitializer.HasValue ? pd.AllowInstanceInitializer.Value : PropertyGrid.If(x => !x.AllowInstanceInitializer.HasValue || x.AllowInstanceInitializer.Value).ReturnSuccess();
		}
		bool IVisualClient.AllowListItemInitializer(RowHandle handle) {
			var cd = (PropertyGrid.With(x => x.RowDataGenerator).RowDataFromHandle(handle).With(x => x.Definition) as CollectionDefinition);
			return cd != null && cd.AllowNewItemInitializer.HasValue ? cd.AllowNewItemInitializer.Value : PropertyGrid.If(x => !x.AllowListItemInitializer.HasValue || x.AllowListItemInitializer.Value).ReturnSuccess();
		}
		bool IVisualClient.AllowCollectionEditor(RowHandle handle) {
			var cd = (PropertyGrid.With(x => x.RowDataGenerator).RowDataFromHandle(handle).With(x => x.Definition) as CollectionDefinition);
			return cd != null && cd.UseCollectionEditor.HasValue ? cd.UseCollectionEditor.Value : PropertyGrid.If(x => !x.UseCollectionEditor.HasValue || x.UseCollectionEditor.Value).ReturnSuccess();
		}
		bool IVisualClient.IsExpanded(RowHandle handle) {
			if (PropertyGrid.DataView.IsGroupRowHandle(handle) && PropertyGrid.ExpandCategoriesWhenSelectedObjectChanged)
				return true;
			return false;
		}
		void Invalidate(RowHandle handle) {
			Invalidate(PropertyGrid.DataView.GetFieldNameByHandle(handle));
		}		
		protected internal virtual void Invalidate(string fieldName) {
			var currentEditorName = GetEditorFieldName((CellEditorOwner.CurrentCellEditor as CellEditor).If(x => x.IsEditorVisible));			
			FieldNameAction action = new FieldNameAction(theAction => {
				if (PropertyGrid != null && PropertyGrid.RowDataGenerator != null) {
					PropertyGrid.RowDataGenerator.ImmediateInvalidate(theAction.FieldName);
				}
			}, fieldName);
			action.CanAggregateFunc = (t, a) => {
				FieldNameAction aggregateCandidate = a as FieldNameAction;
				if (aggregateCandidate == null)
					return false;
				var view = PropertyGrid.DataView;
				var handle = view.Return(x => x.GetHandleByFieldName(fieldName), () => RowHandle.Invalid);
				var result = handle.IsRoot || PropertyBuilder.GetParentHandles(view, aggregateCandidate.FieldName).Select(x => view.GetFieldNameByHandle(x)).Any(x => String.Equals(x, t.FieldName));
				return result;
			};
			ImmediateActionsManager.EnqueueAction(action);
		}
		bool IsRootFieldName(string name) {
			if (name == null)
				return false;
			return PropertyGrid.DataView.GetHandleByFieldName(name).If(x => x.IsRoot).ReturnSuccess();
		}
		public override void BeginInit() {
			suppressInvalidations = true;
			base.BeginInit();
		}
		public override void EndInit() {
			base.EndInit();
			suppressInvalidations = false;
			PropertyGrid.InvalidateData();
		}
		#region INavigationSupport Members
		bool INavigationSupport.ProcessNavigation(NavigationDirection direction) {
			return false;
		}
		IEnumerable<FrameworkElement> INavigationSupport.GetChildren() {
			return Items.Cast<object>().Select((item, index) => ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement);
		}
		FrameworkElement INavigationSupport.GetParent() {
			return PropertyGrid;
		}
		bool INavigationSupport.GetSkipNavigation() {
			return IsKeyboardFocusWithin || SelectedItem == null;
		}
		bool INavigationSupport.GetUseLinearNavigation() {
			return true;
		}
		#endregion
		protected string GetEditorFieldName(CellEditor editor) {
			return editor.With(x => x.RowData).With(x => x.FullPath);
		}
	}
}
