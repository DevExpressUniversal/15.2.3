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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Windows.Input;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Data;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Editors;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public enum RowDataUpdateKind {
		Properties,
		Children
	}
	public class NotifyCollectionChangedAggregateAction : IAggregateAction {
		readonly NotifyCollectionChangedEventArgs args;
		readonly RowsCollectionView owner;
		readonly List<NotifyCollectionChangedAggregateAction> aggregated;
		public NotifyCollectionChangedAggregateAction(RowsCollectionView owner, NotifyCollectionChangedEventArgs args) {
			this.args = args;
			this.owner = owner;
			this.aggregated = new List<NotifyCollectionChangedAggregateAction>();
			this.owner.asyncUpdateLocker.Lock();
		}
		public bool CanAggregate(IAction action) {
			var second = action as NotifyCollectionChangedAggregateAction;
			if (second == null)
				return false;
			owner.asyncUpdateLocker.Unlock();
			aggregated.Add(second);
			return true;
		}
		public void Execute() {
			owner.asyncUpdateLocker.Unlock();
			owner.ProcessPendingActions(GetExecutionList().Select(x => x.args));
		}
		protected IEnumerable<NotifyCollectionChangedAggregateAction> GetExecutionList() {
			foreach (var element in aggregated) {
				foreach (var nested in element.GetExecutionList())
					yield return nested;
			}
			yield return this;
		}
	}
	public class RowsCollectionView : CollectionView {
		#region current        
		public override object CurrentItem { get { return (base.CurrentItem as RowHandle).With(generator.RowDataFromHandle); } }
		protected override void OnCurrentChanged() {
			base.OnCurrentChanged();
		}	 
		#endregion //current
		public override bool CanFilter { get { return false; } }
		public override bool CanGroup { get { return base.CanGroup; } }
		public override bool CanSort { get { return base.CanSort; } }
		public override ObservableCollection<GroupDescription> GroupDescriptions { get { return null; } }
		public override ReadOnlyObservableCollection<object> Groups { get { return null; } }
		public override IComparer Comparer { get { return null; } }				
		public override Predicate<object> Filter { get { return null; } set { } }
		public override bool NeedsRefresh { get { return false; } }
		protected override void OnBeginChangeLogging(NotifyCollectionChangedEventArgs args) { }		
		protected override void OnPropertyChanged(PropertyChangedEventArgs e) { }
		public override bool PassesFilter(object item) { return true; }		
		public override SortDescriptionCollection SortDescriptions { get { return null; } }		
		protected override void RefreshOverride() { }
		IList<RowHandle> visibleHandles = new List<RowHandle>();
		protected IList<RowHandle> VisibleHandles { get { return visibleHandles; } }
		protected void UpdateVisibleHandles() { visibleHandles = new List<RowHandle>(dataView.VisibleHandles); }
		public override IEnumerable SourceCollection { get { return VisibleHandles; } }
		readonly RowDataGenerator generator;
		readonly ImmediateActionsManager actionsManager;
		DataViewBase dataView;
		readonly Locker collectionChangedLocker = new Locker();
		internal readonly Locker asyncUpdateLocker = new Locker();
		static readonly Action<CollectionView> invalidateEnumerableWrapper = ReflectionHelper.CreateInstanceMethodHandler<CollectionView, Action<CollectionView>>(null, "InvalidateEnumerableWrapper", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		protected internal bool AsyncUpdatePending { get { return asyncUpdateLocker.IsLocked; } }		
		protected RowDataGenerator Generator { get { return generator; } }
		protected ImmediateActionsManager ActionsManager { get { return actionsManager; } }
		protected DataViewBase DataView {
			get { return dataView; }
			private set { if (dataView == value) return;
				var oldView = dataView;
				dataView = value;
				DataViewChanged(oldView, dataView);
			}
		}
		protected virtual void DataViewChanged(DataViewBase oldView, DataViewBase newView) {
			if (oldView != null)
				oldView.VisibleHandlesChanged -= OnCollectionChangedAsync;
			if (newView != null)
				newView.VisibleHandlesChanged += OnCollectionChangedAsync;
			ProcessCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		protected void OnCollectionChangedAsync(object sender, NotifyCollectionChangedEventArgs args) {
			if (AsyncUpdatePending || Generator.ShouldProcessCollectionChangedAsynchronously()) {
				ActionsManager.EnqueueAction(new NotifyCollectionChangedAggregateAction(this, args));
			} else
				OnCollectionChanged(sender, args);
		}		
		public void ProcessPendingActions(IEnumerable<NotifyCollectionChangedEventArgs> actions) {
			if (actions.Any(x => x.Action == NotifyCollectionChangedAction.Reset)) {
				ProcessCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				return;
			}
			Dictionary<RowHandle, NotifyCollectionChangedEventArgs> result = new Dictionary<RowHandle, NotifyCollectionChangedEventArgs>();
			foreach(var action in actions) {
				switch (action.Action) {
					case NotifyCollectionChangedAction.Add:
						ProcessAdd(action, true);
						break;
					case NotifyCollectionChangedAction.Remove:
						ProcessRemove(action, true);
						break;
					case NotifyCollectionChangedAction.Replace:
						ProcessReplace(action, true);
						break;					
					default:
						ProcessCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
						return;
				}
			}
			invalidateEnumerableWrapper(this);
		}
		public RowsCollectionView(RowDataGenerator generator, DataViewBase dataView, ImmediateActionsManager actionsManager) : base(Enumerable.Empty<object>()) {
			using (collectionChangedLocker.Lock()) {
				this.generator = generator;
				this.actionsManager = actionsManager;
				DataView = dataView;
			}				
		}
		public void Invalidate() {
			if (DataView != Generator.DataView) {
				DataView = Generator.DataView;
			}
		}
		protected override void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args) {
			if (collectionChangedLocker.IsLocked)
				return;
			if(args.Action!= NotifyCollectionChangedAction.Reset) {
				UpdateVisibleHandles();
			}
			switch (args.Action) {
				case NotifyCollectionChangedAction.Add:
					ProcessAdd(args, false);
					break;
				case NotifyCollectionChangedAction.Remove:
					ProcessRemove(args, false);
					break;
				case NotifyCollectionChangedAction.Replace:
					ProcessReplace(args, false);
					break;
				default:
					ProcessReplace(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new List<RowHandle>(dataView.VisibleHandles), new List<RowHandle>(VisibleHandles), 0), true);
					break;
			}
			invalidateEnumerableWrapper(this);
		}
		protected void ProcessReplace(NotifyCollectionChangedEventArgs args, bool updateCache) {
			int index = args.OldStartingIndex;
			bool createOld = false;
			bool createNew = false;
			var oldItemsEnumerator = args.OldItems.GetEnumerator();
			var newItemsEnumerator = args.NewItems.GetEnumerator();
			while ((createOld = oldItemsEnumerator.MoveNext()) | (createNew = newItemsEnumerator.MoveNext())) {
				var currentOld = (createOld ? oldItemsEnumerator.Current : null) as RowHandle;
				var currentNew = (createNew ? newItemsEnumerator.Current : null) as RowHandle;				
				var data = generator.RowDataFromHandle(currentOld);
				try {
					if (currentOld == currentNew) {
						if (data == null)
							continue;
						Generator.UpdateRowDataForHandle(currentNew, RowDataGenerator.RowDataProperties.All);
						continue;
					}
					var oldValue = (object)data ?? currentOld;
					var newValue = (data != null && currentNew != null) ? (object)ForceCreateRowData(index, dataView.VisibleHandles) : currentNew;
					if (currentOld != null && currentNew != null) {
						VisibleHandles[index] = currentNew;
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index));
					} else if (currentOld != null) {
						VisibleHandles.RemoveAt(index);
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldValue, index));
						index--;
					} else if (currentNew != null) {
						VisibleHandles.Insert(index, currentNew);
						OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newValue, index));
					}
				}
				finally {
					index++;
				}				
				generator.ClearRowDataForHandle(data, currentOld);
			}
		}
		protected void ProcessRemove(NotifyCollectionChangedEventArgs args, bool updateCache) {
			int index = args.OldStartingIndex + args.OldItems.Count - 1;
			foreach (RowHandle handle in args.OldItems.OfType<RowHandle>().Reverse()) {
				var data = generator.RowDataFromHandle(handle);
				if (updateCache)
					VisibleHandles.RemoveAt(index);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, data, index--));
				generator.ClearRowDataForHandle(data, handle);
			}			
		}
		protected void ProcessAdd(NotifyCollectionChangedEventArgs args, bool updateCache) {
			int index = args.NewStartingIndex;
			foreach (RowHandle handle in args.NewItems) {
				if (updateCache)
					VisibleHandles.Insert(index, handle);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, fakeData, index++));
			}
		}
		static readonly RowDataBase fakeData = new RowDataBase();
		protected virtual void NotifyAdd(IList newItems, int newIndex) {
			if (newItems == null)
				return;			
		}
		protected virtual void NotifyRemove(IList oldItems, int oldIndex) {
			if (oldItems == null)
				return;			
		}
		protected virtual RowDataBase GenerateRowData(RowHandle handle) {
			var data = generator.GetRowDataForHandle(handle);
			data.Handle = handle;
			return data;
		}
		protected virtual IList CreateAddedItems(IList newItems) {
			if (newItems == null)
				return null;
			List<RowDataBase> rdList = new List<RowDataBase>();
			foreach(RowHandle handle in newItems) {
				rdList.Add(generator.GetRowDataForHandle(handle));
			}
			return rdList;
		}
		public void ImmediateInvalidate() {
			DataView = Generator.DataView;
		}
		public override int Count { get { return VisibleHandles.Count; } }
		public override bool IsEmpty { get { return !VisibleHandles.Any(); } }
		public override bool Contains(object item) { return base.Contains(item); }
		public override object GetItemAt(int index) {
			if (AsyncUpdatePending)
				ActionsManager.ExecuteActions();
			return ForceCreateRowData(index, VisibleHandles);
		}
		protected RowDataBase ForceCreateRowData(int index, IList<RowHandle> handleSource) {
			var handle = handleSource[index];
			if (handle.IsInvalid)
				return null;
			var data = generator.GetRowDataForHandle(handle);
			generator.PrepareRowDataForHandle(data, handle);
			return data;
		}
		public override int IndexOf(object item) {
			RowDataBase data = item as RowDataBase;
			if (data == null)
				return -1;
			return VisibleHandles.IndexOf(data.Handle);
		}
		protected override IEnumerator GetEnumerator() {
			return EnumerateChildren().GetEnumerator();
		}
		protected IEnumerable EnumerateChildren() {
			foreach (var handle in VisibleHandles) {
				yield return generator.RowDataFromHandle(handle);
			}
		}
	}	  
	public class RowDataGenerator : IFilterClient, ISortClient, ISupportInitialize {
		readonly Locker initializationLocker;
		const string RootFieldName = "";
		PropertyGridView view;
		PropertyBuilder propertyBuilder;
		DataViewBase dataView;
		public DataViewBase DataView {
			get { return dataView; }
			set {
				if (value == dataView)
					return;
				DataViewBase oldValue = dataView;
				dataView = value;
				OnDataViewChanged(oldValue);
			}
		}
		protected bool Initializing {
			get { return initializationLocker.IsLocked; }
		}
		public PropertyBuilder PropertyBuilder {
			get { return propertyBuilder; }
			set {
				if (Equals(value, propertyBuilder))
					return;
				PropertyBuilder oldValue = propertyBuilder;
				propertyBuilder = value;
				OnPropertyBuilderChanged(oldValue);
			}
		}
		public PropertyGridView View {
			get { return view; }
			set {
				if (Equals(value, view))
					return;
				PropertyGridView oldValue = view;
				view = value;
				OnViewChanged(oldValue);
			}
		}
		Dictionary<RowHandle, RowDataBase> CreatedItems { get; set; }
		readonly WeakList<FieldInvalidatedEventHandler> fieldInvalidated = new WeakList<FieldInvalidatedEventHandler>();
		public event FieldInvalidatedEventHandler FieldInvalidated {
			add { fieldInvalidated.Add(value); }
			remove { fieldInvalidated.Remove(value); }
		}
		protected void RaiseFieldInvalidated(FieldInvalidatedEventArgs args) {
			foreach (FieldInvalidatedEventHandler element in fieldInvalidated) {
				element(this, args);
			}
		}		
		public RowDataGenerator() {			
			CreatedItems = new Dictionary<RowHandle, RowDataBase>();
			initializationLocker = new Locker();
			initializationLocker.Unlocked += (o, e) => OnInitialized();
		}
		protected internal RowDataBase ForceGetRowDataForHandle(RowHandle handle) {
			if (handle == null)
				return null;
			RowDataBase data = RowDataFromHandle(handle);
			if (data != null)
				return data;
			RowDataBase parentData = ForceGetRowDataForHandle(DataView.GetParent(handle));
			RowDataBase currentData = GetRowDataForHandle(handle).Do(x => PrepareRowDataForHandle(x, handle));
			if (!handle.IsRoot)
				parentData.ForcedChildren.Add(currentData);
			return currentData;
		}
		public RowDataBase GetRowDataForHandle(RowHandle handle) {
			RowDataBase item = RowDataFromHandle(handle) ?? GetRowDataForHandleOverride(handle);
			if (!handle.IsRoot)
				CreatedItems[handle] = item;
			return item;
		}
		public void PrepareRowDataForHandle(RowDataBase data, RowHandle handle) {
			if (data == null || data.IsReady)
				return;
			PrepareRowDataForHandleOverride(data, handle, RowDataProperties.All);
			data.IsDirty = false;
			data.IsReady = true;
		}
		protected void IsSelectedChanged(RowHandle handle, bool isSelected) {
			if (isSelected)
				ItemsSource.MoveCurrentTo(handle);
		}
		public void ClearRowDataForHandle(RowDataBase data, RowHandle handle) {
			if (data == null)
				return;
			data.IsDirty = true;
			if (data.IsReady) {
				ClearRowDataForHandleOverride(data, handle);
				data.IsReady = false;
			}				
			CreatedItems.Remove(handle);
		}
		public void UpdateRowDataForHandle(RowHandle handle, RowDataProperties updateKind) {
			RowDataBase data = RowDataFromHandle(handle);
			if (data == null)
				return;
			UpdateRowDataForHandleOverride(data, handle, updateKind);
			RaiseFieldInvalidated(new FieldInvalidatedEventArgs(DataView.GetFieldNameByHandle(handle)));
		}
		protected virtual RowDataBase GetRowDataForHandleOverride(RowHandle handle) {
			return new RowDataBase() { IsCategory = DataView.IsGroupRowHandle(handle) };
		}
		[Flags]
		public enum RowDataProperties {
			None = 0x0,
			Defining = 0x1,
			Value = 0x2,
			ExpandSettings = 0x4,
			Definition = 0x8,
			Miscellaneous = 0x10,
			Actions = 0x20,
			Children = 0x40,
			SelfWithoutDefinition = Defining | Value | ExpandSettings | Miscellaneous | Actions,
			Self = SelfWithoutDefinition | Definition,
			All = Self | Children
		}
		protected virtual void PrepareRowDataForHandleOverride(RowDataBase data, RowHandle handle, RowDataProperties properties) {
			data.BeginInit();
			if ((properties & RowDataProperties.Defining) != 0) {
				data.Handle = handle;
				data.RowDataGenerator = this;
				data.FullPath = DataView.GetFieldNameByHandle(handle);
				data.ValueType = DataView.GetPropertyType(handle);
			}
			if ((properties & RowDataProperties.Value) != 0) {
				data.SetValueInternal(DataView.GetValue(handle));
			}
			if ((properties & RowDataProperties.Definition) != 0) {
				PropertyDefinitionBase definition = PropertyBuilder.GetDefinition(DataView, handle, View.PropertyGrid.ShowCategories);
				data.Definition = definition;
			}
			if ((properties & RowDataProperties.Miscellaneous) != 0) {
				data.Level = GetLevel(handle);
				data.ValidationError = CreateValidationError(DataView.GetValidationError(handle).Return(x => x.FirstOrDefault(), () => null));
				data.IsReadOnly = View.PropertyGrid.ReadOnly || DataView.IsReadOnly(handle);
				data.RenderReadOnly = View.PropertyGrid.ReadOnly || DataView.ShouldRenderReadOnly(handle);
				data.ShouldHighlightValue = (View.PropertyGrid == null || !View.PropertyGrid.HighlightNonDefaultValues) ? null : (bool?)DataView.ShouldSerializeValue(handle);
				var description = DataView.GetDescription(handle);
				if (!String.IsNullOrEmpty(description)) {
					data.Description = description;
				}
				data.IsCollectionRow = DataView.IsCollectionHandle(handle);
				if (data.IsCollectionRow) {
					data.ActualTypeInfo = DataView.GetSelectedCollectionNewItem(handle) as TypeInfo;
				}
				data.SetEditableObject(data.Value == null ? null : new EditableObjectWrapper(data.Value, this, handle));
				var standardValuesSupported = DataView.GetIsStandardValuesSupported(handle);
				if (standardValuesSupported)
					data.StandardValues = DataView.GetStandardValues(handle);
			}
			if ((properties & RowDataProperties.Definition) != 0) {
				data.IsModifiableCollectionItem = CalcIsModifiableCollectionItem(data, handle, data.Definition);
			}
			if ((properties & RowDataProperties.ExpandSettings) != 0) {
				data.CanExpand = DataView.CanExpand(handle); 
				if (data.CanExpand) {
					var customExpanded = View.PropertyGrid.RaiseCustomExpand(handle);
					if (customExpanded.HasValue)
						SetExpanded(handle, customExpanded.Value);
					data.IsExpanded = GetExpanded(handle); 
				}
			}
			if ((properties & RowDataProperties.Actions) != 0) {
				data.Actions.Clear();
				data.Actions.Add(View.BarItems.CreateRefreshItem());
				data.Actions.Add(View.BarItems.CreateResetItem());
				data.Actions.Add(new BarItemLinkSeparator());
				if (DataView.IsNewInstanceInitializer(handle)) {
					data.Actions.Add(new BarInstanceInitializerSplitButtonItem(this, handle));
				}
			}
			if (data.Definition != null) {
				data.Definition.Apply(data, (properties & RowDataProperties.Definition) != 0);
			}
			data.EndInit();
		}
		protected virtual int GetLevel(RowHandle handle) {
			var parent = DataView.GetParent(handle);
			if (parent == null || parent.IsRoot || DataView.IsGroupRowHandle(parent))
				return 0;
			else
				return GetLevel(parent) + 1;
		}
		bool CalcIsModifiableCollectionItem(RowDataBase data, RowHandle handle, PropertyDefinitionBase definition) {
			if (View.PropertyGrid.ReadOnly)
				return false;
			if (data.IsCollectionRow && (definition == null || definition.isStandardDefinition && !definition.isResourceGeneratedDefinition || definition is CollectionDefinition))
				return true;
			return DataView.CanRemoveCollectionItem(handle);
		}
		public void SetViewValidationError(RowHandle handle, BaseValidationError error) {
			if (View == null || handle == null)
				return;
			View.SetValidationError(handle, error);
		}
		protected virtual BaseValidationError CreateValidationError(string errorText) {
			if (string.IsNullOrEmpty(errorText))
				return null;
			BaseValidationError error = new BaseValidationError(errorText);
			return error;
		}
		protected internal PropertyDefinitionBase GetStandardDefinition(RowHandle handle) {
			return PropertyBuilder.GetStandardDefinition(DataView, handle);
		}
		protected internal virtual BaseEditSettings GetStandardEditSettings(RowHandle handle) {
			return PropertyBuilder.GetStandardSettings(DataView, handle);
		}
		protected virtual void ClearRowDataForHandleOverride(RowDataBase data, RowHandle handle) {
			if (data.Handle == null)
				return;
			if (View.GetValidationError(handle) != null) {
				View.SetValidationError(handle, null);
			}
			foreach(var childData in data.ForcedChildren) {
				if (DataView.VisibleHandles.Contains(childData.Handle))
					continue;
				ClearRowDataForHandle(childData, handle);
			}
			data.IsSelected = false;
			data.Definition = null;
		}
		protected virtual void UpdateRowDataForHandleOverride(RowDataBase data, RowHandle handle, RowDataProperties updateKind) {
			var skipDefinitionUpdate = Equals(data.Handle, handle) && Equals(data.FullPath, DataView.GetFieldNameByHandle(handle)) && data.ValueType == DataView.GetPropertyType(handle) &&
									   data.Definition != null;
			PrepareRowDataForHandleOverride(data, handle, skipDefinitionUpdate ? RowDataProperties.SelfWithoutDefinition : RowDataProperties.Self);			
		}
		protected internal virtual List<RowDataBase> GetMatchingRowDataList(Func<RowDataBase, bool> predicate) {
			return CreatedItems.Values.Where(predicate).ToList();
		}
		public bool HasRowDataForHandle(RowHandle handle) {
			return CreatedItems.ContainsKey(handle);
		}
		public RowDataBase RowDataFromHandle(RowHandle handle) {
			if (handle == null || handle.IsInvalid || !CreatedItems.ContainsKey(handle))
				return null;
			return CreatedItems[handle];
		}		
		protected internal bool GetExpanded(RowHandle handle) {
			return DataView.IsExpanded(handle);
		}
		public void SetExpanded(RowHandle handle, bool value) {
			if (DataView == null)
				return;
			DataView.SetIsExpanded(handle, value);
		}
		public void SetValue(RowHandle handle, object value) {
			if (View == null)
				return;
			Exception exception = DataView.SetValue(handle, value);
			if (exception == null)
				return;
			View.SetValidationError(handle, new CellValidationError(exception.Message, exception, ErrorType.Default));
		}
		protected void Invalidate() {
			Invalidate(RootFieldName);
		}
		protected internal void ImmediateInvalidate(string fieldName) {
			RowHandle handle = DataView.GetHandleByFieldName(fieldName);
			if (!handle.If(x => !x.IsInvalid).ReturnSuccess()) {
				return;
			}
			PropertyBuilder.Invalidate(DataView, handle);
			DoActionAndSaveFocus(() => {
				if (handle == RowHandle.Root) {
					ItemsSource.Invalidate();
				}
				UpdateRowDataForHandle(handle, RowDataProperties.All);
			});
			View.ImmediateActionsManager.EnqueueAction(PropertyBuilder.ClearUnlinkedDefinitions);
		}
		public void Invalidate(string fieldName) {
			View.Do(x => x.Invalidate(fieldName));
		}
		void DoActionAndSaveFocus(Action action) {
			if (action == null)
				return;
			SelectionStrategy selectionStrategy = View.SelectionStrategy;
			var selectedPath = View.PropertyGrid.SelectedPropertyPath;
			selectionStrategy.Lock();
			action();
			selectionStrategy.Unlock();
			selectionStrategy.SelectViaPath(selectedPath);
		}
		protected virtual void OnViewChanged(PropertyGridView oldValue) {
			if (oldValue != null)
				oldValue.ItemsSource = null;
			UpdateViewItemsSource();
			initializationLocker.DoIfNotLocked(Invalidate);
		}
		protected virtual void OnDataViewChanged(DataViewBase oldValue) {
			if (oldValue != null) {
				oldValue.Dispose();
			}
			if (DataView != null) {
				DataView.FilterClient = this;
				DataView.SortClient = this;
			}
			ItemsSource.Do(x=>x.ImmediateInvalidate());
			initializationLocker.DoIfNotLocked(Invalidate);
		}
		protected virtual void OnPropertyBuilderChanged(PropertyBuilder oldValue) {
			if (oldValue != null) {
				oldValue.Changed -= OnPropertyBuilderDescriptionsChanged;
			}
			if (PropertyBuilder != null) {
				PropertyBuilder.Changed += OnPropertyBuilderDescriptionsChanged;
			}
			initializationLocker.DoIfNotLocked(Invalidate);
		}
		void OnPropertyBuilderDescriptionsChanged(object sender, PropertyBuilderChangedEventArgs args) {
			if (args.Definition == null || args.ChangeKind == PropertyBuilderChangeKind.Reset) {
				((IVisualClient)View).Invalidate(RowHandle.Root);
				return;
			}
			View.ImmediateActionsManager.EnqueueAction(() => OnPropertyBuilderDescriptionsChangedImpl(sender, args));
		}
		void OnPropertyBuilderDescriptionsChangedImpl(object sender, PropertyBuilderChangedEventArgs args) {
			if (Equals(null, args.Definition.Builder))
				return;
			switch (args.ChangeKind) {
				case PropertyBuilderChangeKind.VisualClientProperties:
					GetMatchingRowDataList(x => x.Definition != null && x.Definition == args.Definition).ForEach(x => DataView.Invalidate(x.Handle));
					break;
				case PropertyBuilderChangeKind.MenuProperties:
					break;
				case PropertyBuilderChangeKind.CoreProperties:
					DataView.Invalidate(RowHandle.Root);
					break;
			}
		}
		public RowsCollectionView ItemsSource { get; private set; }
		IEnumerable<RowHandle> ISortClient.GetMappings(RowHandle parent, Func<string, string, RowHandle> addMappingSourceToTarget) {
			return EmptyEnumerable<RowHandle>.Instance;
		}
		IEnumerable<RowHandle> ISortClient.GetSortedChildren(RowHandle parent, IEnumerable<RowHandle> children) {
			PropertyGridSortingEventArgs args = new PropertyGridSortingEventArgs(children, DataView, this);
			View.PropertyGrid.Do(x => x.RaiseSortEvent(args, PropertyBuilder.GetActualSortMode(parent, this)));
			foreach (RowInfo element in args.ResultCollection ?? args.SourceCollection) {
				yield return element.Handle;
			}
		}
		object IFilterClient.GetObject(RowHandle handle) {
			return new RowInfo(DataView, handle, this);
		}
		object IFilterClient.GetDefaultObject() {
			return new RowInfo();
		}
		public void SetSelection(string fieldName, bool immediate, Action endAction) {
			if (immediate) {
				VerifySelection(fieldName, endAction, true);
				return;
			}
			View.ImmediateActionsManager.EnqueueAction(() => VerifySelectionAsync(fieldName, endAction));
		}		
		void VerifySelectionAsync(String fieldName, Action endAction) {
			VerifySelection(fieldName, endAction, false);
		}
		void VerifySelection(string fieldName, Action endAction, bool immediate) {
			try {
				if (string.IsNullOrEmpty(fieldName))
					return;
				RowDataBase currentSelection = (View.SelectedItem as RowDataBase).If(x=>!x.IsDirty);
				var currentSelectionFullPath = currentSelection.With(x => x.FullPath);
				if (string.Equals(fieldName, currentSelectionFullPath)) {
					UpdateFocus(currentSelection);
					return;
				}
				if (!immediate) {
					if (UpdateExpand(fieldName, false)) {
						View.ImmediateActionsManager.EnqueueAction(() => VerifySelectionAsync(fieldName, endAction));
						return;
					}
					View.SelectionStrategy.UpdateSelectedValue();
				}
				RowHandle rowHandle = DataView.GetHandleByFieldName(fieldName);
				RowDataBase rowData = RowDataFromHandle(rowHandle);
				currentSelection.If(x => x.IsSelected).Do(x => x.IsSelected = false);
				rowData.If(x => !x.IsSelected).Do(x => x.IsSelected = true);
				View.SelectedItem = rowData;
				UpdateFocus(rowData);
			}
			finally {
				endAction();
			}
		}
		void UpdateFocus(RowDataBase currentSelection) {
			if (currentSelection == null)
				return;
			if (View.IsKeyboardFocusWithin && currentSelection.IsSelected) {
				RowControlBase element = View.GetRowControl(currentSelection);
				if (element != null && !element.IsKeyboardFocusWithin) {
					CellEditor editor =
						LayoutHelper.FindElement(element, x => x.GetType() == typeof(CellEditor) && PropertyGridHelper.GetPropertyGrid(x) == View.PropertyGrid) as CellEditor;
					var focusedPropertyGrid = (Keyboard.FocusedElement as DependencyObject).With(PropertyGridHelper.GetPropertyGrid);
					var ownerPropertyGrid = View.PropertyGrid;
					if (focusedPropertyGrid!=null && focusedPropertyGrid != ownerPropertyGrid) {
						if(TreeHelper.GetParents<PropertyGridControl>(focusedPropertyGrid).Contains(ownerPropertyGrid)) {
							return;
						}
					}
					editor.Do(x => x.Edit.SetKeyboardFocus());
					if (editor == null)
						View.Focus();
				}
			}
		}
		bool UpdateExpand(string fieldName, bool force) {
			RowHandle handle = DataView.GetHandleByFieldName(fieldName);
			var shouldContinueAsync = false;
			foreach (RowHandle childHandle in IterateBranch(handle).Reverse()) {
				if (childHandle == handle && !force)
					continue;
				shouldContinueAsync |= !DataView.IsExpanded(childHandle);
				DataView.SetIsExpanded(childHandle, true);
			}
			return shouldContinueAsync;
		}
		IEnumerable<RowHandle> IterateBranch(RowHandle rowHandle) {
			RowHandle handle = rowHandle;
			while (IsValidHandle(handle)) {
				yield return handle;
				handle = DataView.GetParent(handle);
			}
		}
		static bool IsValidHandle(RowHandle handle) {
			return handle != null && (!handle.IsInvalid && !handle.IsRoot);
		}
		public void UpdateExpandAsync(RowHandle handle) {
			UpdateExpandAsync(DataView.GetFieldNameByHandle(handle));
		}
		public void UpdateExpandAsync(string path) {
			View.ImmediateActionsManager.EnqueueAction(() => UpdateExpand(path, true));
		}
		public void UpdateCollapseAsync(RowHandle handle) {
			UpdateCollapseAsync(DataView.GetFieldNameByHandle(handle));
		}
		public void UpdateCollapseAsync(string path) {
			View.ImmediateActionsManager.EnqueueAction(() => UpdateCollapse(path));
		}
		void UpdateCollapse(string path) {
			RowDataBase rowData = RowDataFromHandle(DataView.GetHandleByFieldName(path));
			rowData.Do(x => x.IsExpanded = false);
		}
		void ScrollTo(string path) {
			RowHandle handle = DataView.GetHandleByFieldName(path);
			if (View.ItemContainerGenerator.Status == GeneratorStatus.NotStarted) {
				View.ImmediateActionsManager.EnqueueAction(() => ScrollTo(path));
				return;
			}
			ScrollToDeferred(new Queue<RowHandle>(GetHandlesForDeferredScrolling(handle)));
		}
		void ScrollToDeferred(Queue<RowHandle> scrollQueue) {
			if (scrollQueue.Count == 0)
				return;
			RowHandle handle = scrollQueue.Peek();
			RowDataBase rowData = RowDataFromHandle(handle);
			if (rowData == null)
				return;
			SelectItemResult result = View.SelectItem(rowData);
			if (result == SelectItemResult.Select) {
				scrollQueue.Dequeue();
				rowData.IsExpanded = scrollQueue.Count != 0 || rowData.IsExpanded;
			}
			View.ImmediateActionsManager.EnqueueAction(() => ScrollToDeferred(scrollQueue));
		}
		IEnumerable<RowHandle> GetHandlesForDeferredScrolling(RowHandle handle) {
			return IterateBranch(handle).Reverse();
		}
		public void ScrollToAsync(string path) {
			View.ImmediateActionsManager.EnqueueAction(() => ScrollTo(path));
		}
		#region ISupportInitialize Members
		public void BeginInit() {
			initializationLocker.Lock();
		}
		public void EndInit() {
			initializationLocker.Unlock();
		}
		protected virtual void OnInitialized() {
			ItemsSource = new RowsCollectionView(this, DataView, View.ImmediateActionsManager);
			UpdateViewItemsSource();
			Invalidate();
		}
		protected virtual void UpdateViewItemsSource() {
			if (View != null && ItemsSource != null)
				View.ItemsSource = ItemsSource;
		}
		public virtual bool ShouldProcessCollectionChangedAsynchronously() {
			if (View == null)
				return false;
			return View.ShouldProcessCollectionChangedAsynchronously();
		}
		#endregion
	}
	public class FieldInvalidatedEventArgs : EventArgs {
		public string FullPath { get; private set; }
		public FieldInvalidatedEventArgs(string fullPath) {
			this.FullPath = fullPath;
		}
	}
	public delegate void FieldInvalidatedEventHandler(object source, FieldInvalidatedEventArgs args);
	public enum SelectItemResult {
		No,
		Scroll,
		Select
	}
	public static class TreeViewHelper {
		public static SelectItemResult SelectItem(this PropertyGridView treeView, object item) {
			return ExpandAndSelectItem(treeView, item);
		}
		static SelectItemResult ExpandAndSelectItem(ItemsControl parentContainer, object itemToSelect) {
			if (parentContainer.Items.Count == 0)
				return SelectItemResult.No;
			var index = parentContainer.Items.IndexOf(itemToSelect);
			if (index > -1) {
				TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(itemToSelect) as TreeViewItem;
				if (currentContainer != null) {
					currentContainer.IsSelected = true;
					return SelectItemResult.Select;
				}
				ScrollInto(parentContainer, itemToSelect);
				return SelectItemResult.Scroll;
			}
			foreach (object item in parentContainer.Items) {
				TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
				if (currentContainer != null) {
					SelectItemResult result = ExpandAndSelectItem(currentContainer, itemToSelect);
					if (result == SelectItemResult.Scroll || result == SelectItemResult.Select)
						return result;
				}
			}
			return SelectItemResult.No;
		}
		static void ScrollInto(ItemsControl parentContainer, object itemToSelect) {
			PGVirtualizingStackPanel panel = LayoutHelper.FindElementByType(parentContainer, typeof(PGVirtualizingStackPanel)) as PGVirtualizingStackPanel;
			panel.Do(x => x.BringIndexIntoView(parentContainer.Items.IndexOf(itemToSelect)));
		}
	}
}
