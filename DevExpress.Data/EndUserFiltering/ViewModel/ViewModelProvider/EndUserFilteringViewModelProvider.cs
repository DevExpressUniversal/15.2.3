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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using DevExpress.Data.Filtering;
	using DevExpress.Utils.MVVM;
	public class EndUserFilteringViewModelProvider : IEndUserFilteringViewModelProvider, INotifyPropertyChanged, IMetricAttributesQueryOwner {
		readonly IServiceProvider serviceProvider;
		Lazy<IEndUserFilteringSettings> settingsCore;
		Lazy<Type> viewModelTypeCore;
		Lazy<object> viewModelCore;
		Lazy<IStorage<IEndUserFilteringMetricViewModel>> storageCore;
		Lazy<IEndUserFilteringViewModelProperties> propertiesCore;
		Lazy<IEndUserFilteringViewModelPropertyValues> propertyValuesCore;
		Lazy<CriteriaOperator> filterCriteriaCore;
		public EndUserFilteringViewModelProvider(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			settingsCore = new Lazy<IEndUserFilteringSettings>(CreateSettings);
			storageCore = new Lazy<IStorage<IEndUserFilteringMetricViewModel>>(CreateStorage);
			propertiesCore = new Lazy<IEndUserFilteringViewModelProperties>(CreateProperties);
			propertyValuesCore = new Lazy<IEndUserFilteringViewModelPropertyValues>(CreatePropertyValues);
			viewModelTypeCore = new Lazy<Type>(CreateAndInitializeViewModelType);
			viewModelCore = new Lazy<object>(CreateAndInitializeViewModel);
			filterCriteriaCore = new Lazy<CriteriaOperator>(CreateFilterCriteria);
		}
		public void Reset() {
			ResetSettings();
			ResetFilterCriteria();
			ResetViewModelType();
		}
		protected IEnumerable<IEndUserFilteringMetricViewModel> Children {
			get { return storageCore.Value; }
		}
		public IEndUserFilteringMetricViewModel this[string path] {
			get { return propertyValuesCore.Value[path]; }
		}
		IEndUserFilteringViewModelPropertyValues IEndUserFilteringViewModelPropertyValues.GetNestedValues(string rootPath) {
			return propertyValuesCore.Value.GetNestedValues(rootPath);
		}
		protected void ResetSettings() {
			if(settingsCore.IsValueCreated)
				settingsCore = new Lazy<IEndUserFilteringSettings>(CreateSettings);
			if(storageCore.IsValueCreated) {
				foreach(IEndUserFilteringMetricViewModel metricViewModel in storageCore.Value) {
					IDisposable disposable = metricViewModel as IDisposable;
					if(disposable != null) disposable.Dispose();
				}
				storageCore = new Lazy<IStorage<IEndUserFilteringMetricViewModel>>(CreateStorage);
			}
			if(propertiesCore.IsValueCreated)
				propertiesCore = new Lazy<IEndUserFilteringViewModelProperties>(CreateProperties);
			if(propertyValuesCore.IsValueCreated)
				propertyValuesCore = new Lazy<IEndUserFilteringViewModelPropertyValues>(CreatePropertyValues);
		}
		protected void ResetFilterCriteria() {
			if(filterCriteriaCore.IsValueCreated)
				filterCriteriaCore = new Lazy<CriteriaOperator>(CreateFilterCriteria);
		}
		protected void ResetViewModelType() {
			if(viewModelTypeCore.IsValueCreated)
				viewModelTypeCore = new Lazy<Type>(CreateAndInitializeViewModelType);
			if(viewModelCore.IsValueCreated) {
				UnsubscribeViewModel(viewModelCore.Value);
				viewModelCore = new Lazy<object>(CreateAndInitializeViewModel);
			}
		}
		void SubscribeViewModel(object viewModel) {
			(viewModel as INotifyPropertyChanged).@Do(npc =>
				npc.PropertyChanged += ViewModel_PropertyChanged);
		}
		void UnsubscribeViewModel(object viewModel) {
			(viewModel as INotifyPropertyChanged).@Do(npc =>
				npc.PropertyChanged -= ViewModel_PropertyChanged);
		}
		void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName.EndsWith(EndUserFilteringMetricViewModel.FilterCriteriaNotify)) {
				ResetFilterCriteria();
				this.RaisePropertyChanged(() => FilterCriteria);
			}
			else UpdateMemberBindings(sender, e.PropertyName);
		}
		Type sourceTypeCore;
		public Type SourceType {
			get { return sourceTypeCore; }
			set {
				if(value == sourceTypeCore) return;
				sourceTypeCore = value;
				OnSourceTypeChanged();
			}
		}
		protected virtual void OnSourceTypeChanged() {
			ResetSettings();
			ResetViewModelType();
			ResetFilterCriteria();
			OnModelChanged();
		}
		IEnumerable<IEndUserFilteringMetricAttributes> attributesCore;
		public IEnumerable<IEndUserFilteringMetricAttributes> Attributes {
			get { return attributesCore; }
			set {
				if(value == attributesCore) return;
				attributesCore = value;
				OnAttributesChanged();
			}
		}
		protected virtual void OnAttributesChanged() {
			ResetSettings();
			ResetViewModelType();
			ResetFilterCriteria();
			OnModelChanged();
		}
		protected virtual void OnModelChanged() {
			this.RaisePropertyChanged(() => Settings);
			this.RaisePropertyChanged(() => Properties);
			this.RaisePropertyChanged(() => PropertyValues);
			this.RaisePropertyChanged(() => ViewModelType);
			this.RaisePropertyChanged(() => ViewModel);
			this.RaisePropertyChanged(() => FilterCriteria);
		}
		Type viewModelBaseTypeCore;
		public Type ViewModelBaseType {
			get { return viewModelBaseTypeCore; }
			set {
				if(value == viewModelBaseTypeCore) return;
				viewModelBaseTypeCore = value;
				OnViewModelBaseTypeChanged();
			}
		}
		protected virtual void OnViewModelBaseTypeChanged() {
			ResetViewModelType();
			ResetFilterCriteria();
			this.RaisePropertyChanged(() => ViewModelType);
			this.RaisePropertyChanged(() => ViewModel);
			this.RaisePropertyChanged(() => FilterCriteria);
		}
		object parentViewModelCore;
		public object ParentViewModel {
			get { return parentViewModelCore; }
			set {
				if(parentViewModelCore == value) return;
				object oldValue = parentViewModelCore;
				parentViewModelCore = value;
				OnParentViewModelChanged(oldValue, value);
			}
		}
		protected virtual void OnParentViewModelChanged(object oldValue, object newValue) {
			UnsubscribeParentViewModel(oldValue);
			UpdateMemberBindings();
			SubscribeParentViewModel(newValue);
			this.RaisePropertyChanged(() => ParentViewModel);
		}
		void SubscribeParentViewModel(object parentViewModel) {
			(parentViewModel as INotifyPropertyChanged).@Do(npc =>
				npc.PropertyChanged += ParentViewModel_PropertyChanged);
		}
		void UnsubscribeParentViewModel(object parentViewModel) {
			(parentViewModel as INotifyPropertyChanged).@Do(npc =>
				npc.PropertyChanged -= ParentViewModel_PropertyChanged);
		}
		void ParentViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			UpdateMemberBindings(sender, e.PropertyName);
		}
		protected void UpdateMemberBindings() {
			if(!IsViewModelCreated)
				return;
			ViewModel.@Do(vm =>
			{
				MemberReader.ResetAccessors(vm);
				vm.@SetParentViewModel(ParentViewModel);
				UpdateMemberBindings(vm);
				ResetFilterCriteria();
				this.RaisePropertyChanged(() => FilterCriteria);
			});
		}
		void UpdateMemberBindings(object viewModel, string propertyName = null) {
			foreach(IEndUserFilteringMetricViewModel metricViewModel in this)
				metricViewModel.Metric.Attributes.UpdateMemberBindings(viewModel, propertyName, metricViewModel.Query);
		}
		IViewModelProvider parentViewModelProviderCore;
		public IViewModelProvider ParentViewModelProvider {
			get { return parentViewModelProviderCore; }
			set {
				if(parentViewModelProviderCore == value) return;
				var oldValue = parentViewModelProviderCore;
				parentViewModelProviderCore = value;
				OnParentViewModelProviderChanged(oldValue, value);
			}
		}
		protected virtual void OnParentViewModelProviderChanged(IViewModelProvider oldValue, IViewModelProvider newValue) {
			UnsubscribeParentViewModelProvider(oldValue);
			UpdateParentViewModel(newValue);
			SubscribeParentViewModelProvider(newValue);
			this.RaisePropertyChanged(() => ParentViewModelProvider);
		}
		protected void UpdateParentViewModel(IViewModelProvider provider) {
			(provider).@Do(vmp =>
				ParentViewModel = vmp.IsViewModelCreated ? vmp.ViewModel : null);
		}
		void SubscribeParentViewModelProvider(IViewModelProvider viewModelProvider) {
			viewModelProvider.@Do(vmp =>
				vmp.ViewModelChanged += ParentViewModelProvider_ViewModelChanged);
		}
		void UnsubscribeParentViewModelProvider(IViewModelProvider viewModelProvider) {
			viewModelProvider.@Do(vmp =>
				vmp.ViewModelChanged -= ParentViewModelProvider_ViewModelChanged);
		}
		void ParentViewModelProvider_ViewModelChanged(object sender, EventArgs e) {
			UpdateParentViewModel(sender as IViewModelProvider);
		}
		public IEndUserFilteringSettings Settings {
			get { return settingsCore.Value; }
		}
		public IEndUserFilteringViewModelProperties Properties {
			get { return propertiesCore.Value; }
		}
		public IEndUserFilteringViewModelPropertyValues PropertyValues {
			get { return propertyValuesCore.Value; }
		}
		public Type ViewModelType {
			get { return viewModelTypeCore.Value; }
		}
		public object ViewModel {
			get { return viewModelCore.Value; }
		}
		public bool IsViewModelTypeCreated {
			get { return viewModelTypeCore.IsValueCreated; }
		}
		public bool IsViewModelCreated {
			get { return viewModelCore.IsValueCreated; }
		}
		public CriteriaOperator FilterCriteria {
			get { return filterCriteriaCore.Value; }
		}
		public void ClearFilterCriteria() {
			if(!IsViewModelCreated)
				return;
			foreach(IEndUserFilteringMetricViewModel metricViewModel in this.Where(mvm => mvm.HasValue))
				metricViewModel.Value.Reset();
		}
		protected Type CreateAndInitializeViewModelType() {
			Type viewModelType = CreateViewModelType();
			viewModelType.@Do(vmType =>
				this.RaisePropertyChanged(() => IsViewModelTypeCreated));
			return viewModelType;
		}
		protected IEndUserFilteringViewModel CreateAndInitializeViewModel() {
			IEndUserFilteringViewModel viewModel = CreateViewModel();
			viewModel.@Do(vm =>
			{
				vm.@SetParentViewModel(ParentViewModel);
				foreach(IEndUserFilteringMetricViewModel metricViewModel in this)
					metricViewModel.@SetParentViewModel(vm);
				vm.Initialize(this);
				SubscribeViewModel(vm);
				this.RaisePropertyChanged(() => IsViewModelCreated);
			});
			return viewModel;
		}
		protected IEndUserFilteringSettings CreateSettings() {
			return CreateSettings(SourceType, Attributes);
		}
		protected virtual IEndUserFilteringSettings CreateSettings(Type sourceType, IEnumerable<IEndUserFilteringMetricAttributes> attributes) {
			var factory = GetService<IEndUserFilteringSettingsFactory>();
			return factory.@Get(f => f.Create(sourceType, attributes));
		}
		protected IEndUserFilteringViewModelProperties CreateProperties() {
			return CreateProperties(settingsCore.Value);
		}
		protected virtual IEndUserFilteringViewModelProperties CreateProperties(IEndUserFilteringSettings settings) {
			var typeResolver = GetService<IValueTypeResolver>();
			return new EndUserFilteringViewModelProperties(settings, m => GetValueViewModelType(typeResolver, m));
		}
		protected IEndUserFilteringViewModelPropertyValues CreatePropertyValues() {
			return CreatePropertyValues(storageCore.Value);
		}
		protected virtual IEndUserFilteringViewModelPropertyValues CreatePropertyValues(IStorage<IEndUserFilteringMetricViewModel> storage) {
			return new EndUserFilteringViewModelPropertyValues(storage);
		}
		protected IStorage<IEndUserFilteringMetricViewModel> CreateStorage() {
			return CreateStorage(GetChildren(Settings));
		}
		protected virtual IStorage<IEndUserFilteringMetricViewModel> CreateStorage(IEnumerable<IEndUserFilteringMetricViewModel> children) {
			return new Storage<IEndUserFilteringMetricViewModel>(children, m => m.Metric.Order);
		}
		protected IEnumerable<IEndUserFilteringMetricViewModel> GetChildren(IEndUserFilteringSettings settings) {
			var valueTypeResolver = GetService<IValueTypeResolver>();
			var valueViewModelFactory = GetService<IViewModelFactory>();
			var metricViewModelFactory = GetService<IEndUserFilteringMetricViewModelFactory>();
			var viewModelBuilderResolver = GetService<IViewModelBuilderResolver>();
			var metricAttributesQueryFactory = GetService<IMetricAttributesQueryFactory>();
			foreach(IEndUserFilteringMetric metric in settings) {
				IMetricAttributesQuery query = CreateMetricAttributesQuery(metricAttributesQueryFactory, metric);
				Type valueBoxType = GetValueBoxType(valueTypeResolver, metric);
				Type valueType = GetValueViewModelType(valueTypeResolver, metric);
				IValueViewModel value = CreateValueViewModel(valueViewModelFactory, viewModelBuilderResolver, valueBoxType, metric);
				yield return CreateMetricViewModel(metricViewModelFactory, metric, query, value, valueType);
			}
		}
		protected CriteriaOperator CreateFilterCriteria() {
			if(!IsViewModelCreated)
				return null;
			var filterOperands = this
					.Where(mvm => mvm.HasValue)
					.Select(mvm => mvm.FilterCriteria);
			return CriteriaOperator.And(filterOperands);
		}
		protected IValueViewModel CreateValueViewModel(IViewModelFactory viewModelFactory, IViewModelBuilderResolver viewModelBuilderResolver,
			Type valueBoxType, IEndUserFilteringMetric metric) {
			var builder = viewModelBuilderResolver.@Get(resolver => resolver.CreateValueViewModelBuilder(metric));
			return viewModelFactory.@Get(factory =>
					(IValueViewModel)factory.Create(valueBoxType, builder));
		}
		protected Type GetValueViewModelType(IValueTypeResolver typeResolver, IEndUserFilteringMetric metric) {
			return typeResolver.@Get(resolver =>
					resolver.GetValueViewModelType(metric.AttributesTypeDefinition, metric.Type));
		}
		protected Type GetValueBoxType(IValueTypeResolver typeResolver, IEndUserFilteringMetric metric) {
			return typeResolver.@Get(resolver =>
					resolver.GetValueBoxType(metric.AttributesTypeDefinition, metric.Type));
		}
		protected IMetricAttributesQuery CreateMetricAttributesQuery(IMetricAttributesQueryFactory metricAttributesQueryFactory, IEndUserFilteringMetric metric) {
			return metricAttributesQueryFactory.@Get(factory =>
					factory.CreateQuery(metric, this));
		}
		protected IEndUserFilteringMetricViewModel CreateMetricViewModel(IEndUserFilteringMetricViewModelFactory viewModelFactory,
			IEndUserFilteringMetric metric, IMetricAttributesQuery query, IValueViewModel value, Type valueType) {
			return viewModelFactory.@Get(factory =>
					factory.Create(metric, query, value, valueType));
		}
		protected virtual Type CreateViewModelType() {
			var typeBuilder = GetService<IEndUserFilteringViewModelTypeBuilder>();
			return typeBuilder.@Get(builder =>
					builder.Create(ViewModelBaseType, Properties, PropertyValues));
		}
		protected virtual IEndUserFilteringViewModel CreateViewModel() {
			var viewModelFactory = GetService<IViewModelFactory>();
			var viewModelBuilderResolver = GetService<IViewModelBuilderResolver>();
			var builder = viewModelBuilderResolver.@Get(resolver =>
					resolver.CreateViewModelBuilder());
			return viewModelFactory.@Get(factory =>
					(IEndUserFilteringViewModel)factory.Create(ViewModelType, builder));
		}
		#region Services
		protected TService GetService<TService>() where TService : class {
			return serviceProvider.@Get(provider =>
				provider.GetService(typeof(TService)) as TService);
		}
		#endregion
		#region IEnumerable
		IEnumerator<IEndUserFilteringMetricViewModel> IEnumerable<IEndUserFilteringMetricViewModel>.GetEnumerator() {
			return PropertyValues.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)PropertyValues).GetEnumerator();
		}
		#endregion
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			var handler = (PropertyChangedEventHandler)PropertyChanged;
			if(handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#region EndUserFilteringViewModelProperties
		class EndUserFilteringViewModelProperties : IEndUserFilteringViewModelProperties {
			readonly IEnumerable<KeyValuePair<string, Type>> pairs;
			EndUserFilteringViewModelProperties(IEnumerable<KeyValuePair<string, Type>> pairs) {
				this.pairs = pairs;
			}
			internal EndUserFilteringViewModelProperties(IEndUserFilteringSettings settings, Func<IEndUserFilteringMetric, Type> getType) {
				this.pairs = settings.GetPairs(getType);
			}
			IEnumerator<KeyValuePair<string, Type>> IEnumerable<KeyValuePair<string, Type>>.GetEnumerator() {
				return pairs.GetEnumerator();
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return ((IEnumerable)pairs).GetEnumerator();
			}
			IEndUserFilteringViewModelProperties IEndUserFilteringViewModelProperties.GetNestedProperties(string rootPath) {
				return new EndUserFilteringViewModelProperties(pairs.Where(p => p.Key.StartsWith(rootPath)));
			}
		}
		class EndUserFilteringViewModelPropertyValues : IEndUserFilteringViewModelPropertyValues {
			readonly IStorage<IEndUserFilteringMetricViewModel> storageCore;
			internal EndUserFilteringViewModelPropertyValues(IEnumerable<IEndUserFilteringMetricViewModel> values) {
				storageCore = new Storage<IEndUserFilteringMetricViewModel>(values, vm => vm.Metric.Order);
			}
			internal EndUserFilteringViewModelPropertyValues(IStorage<IEndUserFilteringMetricViewModel> storage) {
				storageCore = storage;
			}
			public IEndUserFilteringMetricViewModel this[string path] {
				get { return storageCore[path, vm => vm.Metric.Path]; }
			}
			IEnumerator<IEndUserFilteringMetricViewModel> IEnumerable<IEndUserFilteringMetricViewModel>.GetEnumerator() {
				return storageCore.GetEnumerator();
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return ((IEnumerable)storageCore).GetEnumerator();
			}
			IEndUserFilteringViewModelPropertyValues IEndUserFilteringViewModelPropertyValues.GetNestedValues(string rootPath) {
				return new EndUserFilteringViewModelPropertyValues(storageCore.Where(vm => vm.Metric.Path.StartsWith(rootPath)));
			}
		}
		#endregion
		#region IMetricAttributesQueryOwner
		void IMetricAttributesQueryOwner.RaiseMetricAttributesQuery<TEventArgs, TData>(TEventArgs e) {
			(e as QueryRangeDataEventArgs).@Do(RaiseRangeMetricAttributesQuery);
			(e as QueryLookupDataEventArgs).@Do(RaiseLookupMetricAttributesQuery);
			(e as QueryBooleanChoiceDataEventArgs).@Do(RaiseBooleanChoiceMetricAttributesQuery);
		}
		protected virtual void RaiseRangeMetricAttributesQuery(QueryRangeDataEventArgs e) { }
		protected virtual void RaiseLookupMetricAttributesQuery(QueryLookupDataEventArgs e) { }
		protected virtual void RaiseBooleanChoiceMetricAttributesQuery(QueryBooleanChoiceDataEventArgs e) { }
		#endregion
		#region RetrieveFields
		public void RetrieveFields(Action<Type> retrieveFields, Type sourceType, IEnumerable<IEndUserFilteringMetricAttributes> attributes = null, Type viewModelBaseType = null) {
			if(retrieveFields == null)
				return;
			var typeBuilder = GetService<IEndUserFilteringViewModelTypeBuilder>();
			var viewModelType = typeBuilder.@Get(builder =>
			{
				var settings = CreateSettings(sourceType, attributes);
				var properties = CreateProperties(settings);
				var storage = CreateStorage(GetChildren(settings));
				var propertyValues = CreatePropertyValues(storage);
				return builder.Create(viewModelBaseType, properties, propertyValues);
			});
			viewModelType.@Do(retrieveFields);
		}
		#endregion
	}
}
