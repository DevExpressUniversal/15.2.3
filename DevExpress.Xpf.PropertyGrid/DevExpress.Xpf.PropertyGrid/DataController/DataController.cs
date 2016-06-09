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

#if SL
using DevExpress.Data.Browsing;
#else
using System.ComponentModel;
#endif
using System.Collections.Generic;
using DevExpress.XtraVerticalGrid.Data;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using System;
using System.Linq;
using System.Collections;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.PropertyGrid.Internal {	
	public interface IVisualClient {
		void Invalidate(RowHandle handle);
		IInstanceInitializer GetInstanceInitializer(RowHandle handle);
		IInstanceInitializer GetListItemInitializer(RowHandle handle);
		bool AllowCommitOnValidationAttributeError();
		bool AllowCollectionEditor(RowHandle handle);
		bool AllowInstanceInitializer(RowHandle handle);
		bool AllowListItemInitializer(RowHandle handle);
		AllowExpandingMode GetAllowExpandingMode(RowHandle handle);
		bool IsExpanded(RowHandle handle);
		bool InvokeRequired { get; }
		void Invoke(Delegate method, params object[] args);
	}
	public class DataController : IServiceProvider {
		static NullVisualClient NullClient = new NullVisualClient();
		const string IncorrectHandleString = "Incorrect handle";
		internal static NewInstanceConverter NewInstanceConverter = new NewInstanceConverter();
		static DescriptorContextFactory DescriptorContextFactory = new DescriptorContextFactory();
		bool isMultiSource = false;
		bool showAttachedProperties = true;
		PGridDataModeHelperContextCache contextCache;
		GetDescriptorContextCommand getContextCommand;
		object source;
		IVisualClient visualClient;
		Attribute[] browsableAttributes = new Attribute[] { new BrowsableAttribute(true) };
		internal event EventHandler SourceChanged;
		internal event EventHandler SourceChanging;
		internal protected PGridDataModeHelperContextCache ContextCache {
			get {
				if (contextCache == null) {
					contextCache = CreateContextCache();
					InitializeContextCache();
				}
				return contextCache;
			}
		}
		protected GetDescriptorContextCommand GetContextCommand {
			get {
				if(getContextCommand == null)
					getContextCommand = new GetDescriptorContextCommand();
				return getContextCommand;
			}
		}
		public object Source {
			get { return source; }
			set {
				if(Source == value)
					return;
				source = value;
				OnSourceChanged();
			}
		}
		public bool ShowAttachedProperties {
			get { return showAttachedProperties; }
			set {
				if (showAttachedProperties == value)
					return;
				showAttachedProperties = value;
				ShowAttachedPropertiesChanged();
			}
		}
		public bool IsMultiSource {
			get { return isMultiSource; }
			set {
				if(IsMultiSource == value)
					return;
				isMultiSource = value;
				OnIsMultiSourceChanged();
			}
		}
		public IVisualClient VisualClient {
			get {
				if(visualClient == null) {
					visualClient = NullClient;
				}
				return visualClient;
			}
			set {
				if(VisualClient == value)
					return;
				visualClient = value;
				InvalidateSource();
			}
		}
		internal Attribute[] BrowsableAttributes { get { return browsableAttributes; } }
		Locker SetValueLocker { get; set; }
		public DataController() {
			SetValueLocker = new Locker();
		}
		public void UpdateContextTree() {
			UpdateHandlesOperation updateChildrenOperation = new UpdateHandlesOperation();
			DescriptorContextIterator iterator = new DescriptorContextIterator(ContextCache) { IsMultiSource = IsMultiSource };
			iterator.DoOperation(updateChildrenOperation);
			CollectExpandedStateOperation collectStateOperation = new CollectExpandedStateOperation();
			iterator.DoLocalOperation(collectStateOperation, updateChildrenOperation.InvalidatedHandles);
			ContextCache.Clear();
			UpdateCache(updateChildrenOperation.contextList, updateChildrenOperation.handleList);
			RestoreExpandedState(collectStateOperation.ExpandedState);
		}
		protected virtual void InitializeContextCache() {
			var context = GetDescriptorContext(PropertyHelper.RootPropertyName);
			ContextCache[IsMultiSource, RowHandle.Invalid] = null;
		}
		void RestoreExpandedState(List<KeyValuePair<string, bool?>> expandedState) {
			expandedState.ForEach(state => GetDescriptorContext(state.Key, true).Do(x => x.SetIsExpandedInternal(state.Value)));
		}
		void UpdateCache(IList<DescriptorContext> contexList, IList<RowHandle> handleList) {
			for (int i = 0; i < contexList.Count; i++) {
				ContextCache[IsMultiSource, contexList[i].FieldName] = contexList[i];
				ContextCache[IsMultiSource, handleList[i]] = contexList[i];
				if (!contexList[i].IsValid) {
					contexList[i].Reset();
				}
			}
		}
		void ResetCache() {
			this.contextCache = null;
		}
		void OnIsMultiSourceChanged() {
			InvalidateSource();
		}
		void InvalidateSource() {
			ResetCache();
			RowHandleChanged(RowHandle.Root, RowHandleChangeType.Reset);
		}
		void RowHandleChanged(RowHandle handle, RowHandleChangeType change) {
			SourceChanged.Do(x => x(this, new RowHandleChangedEventArgs { ChangeType = change, Handle = handle }));
		}
		void RowHandleChanging(RowHandle handle, RowHandleChangeType change) {
			SourceChanging.Do(x => x(this, new RowHandleChangedEventArgs { ChangeType = change, Handle = handle }));
		}
		void OnSourceChanged() {
			InvalidateSource();
		}
		void ShowAttachedPropertiesChanged() {
			InvalidateSource();
		}
		void UseListEditorChanged() {
			InvalidateSource();
		}
		internal object GetValue(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.Value, () => null);
		}
		internal Type GetPropertyType(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.PropertyType, () => null);
		}
		internal bool CanExpand(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.CanExpand, () => false);
		}
		public IEnumerable<RowHandle> GetChildHandles(RowHandle handle) {
			return GetDescriptorContext(handle).If(x => x.IsLoaded).Return(x => x.ChildHandles, () => new List<RowHandle>());
		}
		internal bool ShouldIncludeMultiProperty(DescriptorContext parentContext, PropertyDescriptor descriptor) {
			if(!IsMultiSource)
				return false;
			return GetDescriptorContext(parentContext, descriptor).PropertyDescriptor != null;
		}
		internal bool IsGroup(DescriptorContext context) {
			return context != null && context is GroupDescriptorContext;
		}
		internal bool IsGroupRowHandle(RowHandle handle) {
			return IsGroup(GetDescriptorContext(handle));
		}
		internal bool IsNewInstanceInitializer(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.Converter is NewInstanceConverter, () => false);
		}
		internal IList<RowHandle> GetCategoryHandles() {
			DescriptorContext rootContext = GetDescriptorContext(RowHandle.Root);
			return rootContext.CategoryHandles;
		}
		protected virtual PGridDataModeHelperContextCache CreateContextCache() {
			return new PGridDataModeHelperContextCache();
		}
		internal DescriptorContext GetParent(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.ParentContext, () => null);
		}
		internal bool IsExpanded(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.IsExpanded, () => false);
		}
		internal bool IsLoaded(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.IsLoaded, () => false);
		}
		internal IEnumerable GetStandardValues(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.StandardValues, () => null);
		}
		internal bool GetIsStandardValuesSupported(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.IsStandardValuesSupported, () => false);
		}
		internal bool CanUseCollectionEditor(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.CanUseCollectionEditor(), () => false);
		}
		internal bool IsCollectionHandle(RowHandle handle) {
			return GetDescriptorContext(handle).With(x => x.Converter as ListConverter).ReturnSuccess();
		}
		internal bool GetIsStandardValuesExclusive(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.IsStandardValuesExclusive, () => true);
		}
		internal CollectionHelper GetCollectionHelper(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.CollectionHelper, null);
		}
		internal DescriptorContext GetDescriptorContext(RowHandle handle) {
			if (handle == null)
				return null;
			return ContextCache[IsMultiSource, handle];
		}
		internal DescriptorContext GetDescriptorContext(string fieldName, bool onlyBrowsable = false) {
			return GetDescriptorContextCore(fieldName, IsMultiSource, onlyBrowsable);
		}
		internal DescriptorContext GetDescriptorContext(DescriptorContext parentContext, PropertyDescriptor descriptor) {
			return GetDescriptorContextCore(parentContext, descriptor, IsMultiSource);
		}
		DescriptorContext GetDescriptorContextCore(DescriptorContext parentContext, PropertyDescriptor descriptor, bool isMultiSource) {
			object dataSource = GetSource(isMultiSource);
			GetContextCommand.Initialize(isMultiSource, ContextCache, dataSource, this, BrowsableAttributes);
			DescriptorContext context = GetContextCommand.Execute(parentContext, descriptor);
			GetContextCommand.Release();
			return context;
		}
		DescriptorContext GetDescriptorContextCore(string fieldName, bool isMultiSource, bool onlyBrowsable = false) {
			if (fieldName == null)
				return null;
			object dataSource = GetSource(isMultiSource);
			GetContextCommand.Initialize(isMultiSource, ContextCache, dataSource, this, BrowsableAttributes);
			GetContextCommand.OnlyBrowsable = onlyBrowsable;
			DescriptorContext context = GetContextCommand.Execute(fieldName);
			GetContextCommand.Release();
			return context;
		}
		object GetSource(bool isMultiSource) {
			if(IsMultiSource && !isMultiSource)
				return ((object[])Source)[0];
			return Source;
		}
		internal Exception SetValue(RowHandle handle, object newValue) {
			return SetValueInternal(handle,
				context => context.SetValue(newValue));
		}
		Exception SetValueInternal(RowHandle handle, Func<DescriptorContext, RowHandle> setValueFunction) {
			DescriptorContext context = GetDescriptorContext(handle);
			if (context == null)
				return null;
			Exception exception = null;
			RowHandle changedHandle = null;
			SetValueLocker.DoLockedAction(() => {
				try {
					changedHandle = setValueFunction(context);
				}
				catch (Exception e) {
					exception = PropertyHelper.GetUnwindedException(e);
					return;
				}
			});
			if (exception == null) {
				Invalidate(changedHandle);
			}
			return exception;
		}
		List<RowHandle> invalidateList = new List<RowHandle>();
		internal bool AddToInvalidateList(RowHandle handle) {
			if (!IsValidProperty(handle))
				return false;
			invalidateList.Add(handle);
			return true;
		}
		internal void UpdateHandles() {
			List<RowHandle> filteredList = Filter(invalidateList);
			filteredList.ForEach(handle => {
				DescriptorContext changedContext = GetDescriptorContext(handle);
				if (changedContext == null)
					return;
				changedContext.InvalidateChildren();
			});
			if (filteredList.Count == 1) {
				var handle = filteredList[0];
				RowHandleChanging(handle, handle.IsRoot ? RowHandleChangeType.Reset : RowHandleChangeType.Replace);
				UpdateContextTree();
				RowHandleChanged(handle, handle.IsRoot ? RowHandleChangeType.Reset : RowHandleChangeType.Replace);
			}
			else {
				UpdateContextTree();
				RowHandleChanged(RowHandle.Root, RowHandleChangeType.Reset);
			}
			invalidateList.Clear();
		}
		List<RowHandle> Filter(List<RowHandle> invalidateList) {
			List<RowHandle> list = new List<RowHandle>();
			foreach (var handle in invalidateList) {
				if (ShouldUpdateRoot(handle)) {
					list.Clear();
					list.Add(RowHandle.Root);
					return list;
				}
				if (!list.Contains(handle) && IsValidProperty(handle)) {
					list.Add(handle);
				}
			}
			return list;
		}
		internal IEnumerable<string> GetValidationError(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.ValidationError, () => null);
		}
		internal void Invalidate(RowHandle handle) {
			if (Source == null)
				return;
			if (AddToInvalidateList(handle))
				VisualClient.Invalidate(handle);
		}
		bool IsValid { get { return invalidateList.Count == 0; } }
		public void Update() {
			if (IsValid)
				return;
			if (VisualClient.InvokeRequired) {
				VisualClient.Invoke((Action)UpdateHandles);
			}
			else {
				UpdateHandles();
			}
		}
		bool ShouldUpdateRoot(RowHandle handle) {
			return handle == RowHandle.Root ||
				RefreshPropertiesAttribute.All.Equals(GetDescriptorContext(handle).With(x => x.PropertyAttributes).Return(propertyAttributes => propertyAttributes[typeof(RefreshPropertiesAttribute)], () => null));
		}
		bool IsValidProperty(RowHandle handle) {
			if (handle == null || handle == RowHandle.Invalid)
				return false;
			var context = GetDescriptorContext(handle);
			return context != null;
		}
		internal bool CanResetValue(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.CanResetValue(), () => false);
		}
		internal Exception ResetValue(RowHandle handle) {
			return SetValueInternal(handle,
				context => context.ResetValue());
		}
		internal string GetNameByHandle(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.Name, () => null);
		}
		internal string GetFieldNameByHandle(RowHandle rowHandle) {
			return GetDescriptorContext(rowHandle).Return(x => x.FieldName, () => null);
		}
		internal RowHandle GetHandleByFieldName(string fieldName) {
			return GetDescriptorContext(fieldName).Return(x => x.RowHandle, () => RowHandle.Invalid);
		}
		internal string GetDescription(RowHandle handle) {
			DescriptorContext context = GetDescriptorContext(handle);
			if (context == null)
				return null;
			return context.PropertyDescriptor.Return(x => x.Description, () => null);
		}
		internal bool IsAttachedProperty(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => DescriptorContext.IsAttachedProperty(x.PropertyDescriptor), () => false);
		}
		internal string GetDisplayName(RowHandle rowHandle) {
			return GetDescriptorContext(rowHandle).Return(x => x.DisplayName, () => null);
		}
		internal bool IsReadOnly(RowHandle rowHandle) {
			return !GetDescriptorContext(rowHandle).Return(x => x.IsValueEditable, () => true);
		}
		internal bool ShouldRenderReadOnly(RowHandle rowHandle) {
			return GetDescriptorContext(rowHandle).Return(x => x.ShouldRenderReadOnly, () => true);
		}
		internal void SetIsExpanded(RowHandle handle, bool isExpanded) {
			bool changed = false;
			RowHandleChanging(handle, isExpanded ? RowHandleChangeType.Expand : RowHandleChangeType.Collapse);
			GetDescriptorContext(handle).Do(x => changed = x.SetIsExpanded(isExpanded));
			if (changed) {
				RowHandleChanged(handle, isExpanded ? RowHandleChangeType.Expand : RowHandleChangeType.Collapse);
			}
		}
		internal bool ShouldSerializeValue(RowHandle handle) {
			return GetDescriptorContext(handle).Return(x => x.ShouldSerializeValue, () => false);
		}
		T GetService<T>() {
			return (T)((IServiceProvider)this).GetService(typeof(T));
		}
		object IServiceProvider.GetService(Type serviceType) {
			if (serviceType == typeof(DescriptorContextFactory))
				return DescriptorContextFactory;
			if (serviceType == typeof(DataController))
				return this;
			return null;
		}
		internal bool CanConvertToString(RowHandle handle) {
			DescriptorContext context = GetDescriptorContext(handle);
			if (context == null)
				return false;
			return context.Converter.Return(converter => converter.CanConvertTo(context, typeof(string)), () => false);
		}
		internal bool CanConvertFromString(RowHandle handle) {
			DescriptorContext context = GetDescriptorContext(handle);
			if (context == null)
				return false;
			return context.Converter.Return(converter => converter.CanConvertFrom(context, typeof(string)), () => false);
		}
		internal string ConvertToString(RowHandle handle, object value) {
			DescriptorContext context = GetDescriptorContext(handle);
			if (context == null || context.Converter == null)
				return IncorrectHandleString;
			return context.Converter.ConvertToString(context, value);
		}
	}
	class NullVisualClient : IVisualClient {
		public bool IsExpanded(RowHandle handle) {
			return false;
		}
		public void Invalidate(RowHandle handle) { }
		public IInstanceInitializer GetInstanceInitializer(RowHandle handle) {
			return null;
		}
		public IInstanceInitializer GetListItemInitializer(RowHandle handle) {
			return null;
		}
		public bool AllowCollectionEditor(RowHandle handle) {
			return false;
		}
		bool IVisualClient.AllowInstanceInitializer(RowHandle handle) {
			return false;
		}
		bool IVisualClient.AllowListItemInitializer(RowHandle handle) {
			return true;
		}
		AllowExpandingMode IVisualClient.GetAllowExpandingMode(RowHandle handle) {
			return AllowExpandingMode.Default;
		}
		bool IVisualClient.InvokeRequired {
			get { return false; }
		}
		bool IVisualClient.AllowCommitOnValidationAttributeError() {
			return false;
		}
		void IVisualClient.Invoke(Delegate method, params object[] args) {
			throw new NotImplementedException();
		}
	}
	public class RowHandleChangedEventArgs : EventArgs {
		public RowHandle Handle { get; set; }
		public RowHandleChangeType ChangeType { get; set; }
	}
	public enum RowHandleChangeType {
		Collapse,
		Expand,
		Reset,
		Replace
	}
}
