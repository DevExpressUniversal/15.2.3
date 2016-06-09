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
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Filtering.Internal;
using DevExpress.Utils.MVVM;
using DevExpress.Utils.MVVM.Internal;
namespace DevExpress.Utils.Filtering {
	[Designer("DevExpress.Utils.Design.FilteringUIContextDesigner, " + AssemblyInfo.SRAssemblyDesignFull, typeof(IDesigner))]
	[Description("A component that generates a Filtering UI ViewModel and manages its lifecycle.")]
	[ToolboxTabName(AssemblyInfo.DXTabComponents), DXToolboxItem(DXToolboxItemKind.Free)]
	[ToolboxBitmap(typeof(ToolBoxIcons.ToolboxIconsRootNS), "EndUserFilteringContext")]
	[DevExpress.Utils.Design.Filtering.FilteringModelMetadata(CustomAttributesProperty = "CustomMetricAttributes")]
	public class FilteringUIContext : Component, ISupportInitialize, ITypedList {
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinMVVM));
		}
		#region ctor
		public FilteringUIContext()
			: this(null) {
		}
		public FilteringUIContext(IContainer container) {
			if(container != null)
				container.Add(this);
			providerFactoryCore = new Lazy<BaseEndUserFilteringViewModelProviderFactory>(() => CreateProviderFactory());
			viewModelProviderCore = new Lazy<IEndUserFilteringViewModelProvider>(() => CreateViewModelProvider());
			this.customMetricAttributesCore = CreateCustomMetricAttributes();
			CustomMetricAttributes.CollectionChanged += CustomMetricAttributes_CollectionChanged;
		}
		#endregion ctor
		bool isDisposing;
		protected override void Dispose(bool disposing) {
			if(!isDisposing) {
				isDisposing = true;
				OnDisposing();
			}
			base.Dispose(disposing);
		}
		protected virtual void OnDisposing() {
			UnsubscribeControlEvents();
			ResetTypedListProperties();
			((IDisposable)disposableObjects).Dispose();
			if(CustomMetricAttributes != null)
				CustomMetricAttributes.CollectionChanged -= CustomMetricAttributes_CollectionChanged;
			if(viewModelProviderCore.IsValueCreated) {
				UnsubscribeViewModelProvider(viewModelProviderCore.Value);
				if(ViewModelProvider.IsViewModelCreated)
					POCOTypesFactory.Reset(ViewModelProvider.ViewModel.GetType());
				ViewModelProvider.Reset();
			}
			ModelType = null;
			BaseViewModelType = null;
			ParentViewModel = null;
			ParentViewModelProvider = null;
			providerFactoryCore = null;
			viewModelProviderCore = null;
			controlCore = null;
		}
		DisposableObjectsContainer disposableObjects = new DisposableObjectsContainer();
		protected internal TDisposable Register<TDisposable>(TDisposable obj) where TDisposable : IDisposable {
			return (disposableObjects != null) ? disposableObjects.Register(obj) : obj;
		}
		#region Properties
		protected bool IsDesignMode {
			get { return DesignMode || DevExpress.Utils.Design.DesignTimeTools.IsDesignMode; }
		}
		Type modelTypeCore;
		[DefaultValue(null), Category("Model"), RefreshProperties(RefreshProperties.All)]
		[TypeConverter("DevExpress.Utils.MVVM.Design.ModelObjectTypeConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
		public Type ModelType {
			[System.Diagnostics.DebuggerStepThrough]
			get { return modelTypeCore; }
			set {
				if(modelTypeCore == value) return;
				modelTypeCore = value;
				OnModelTypeChanged();
			}
		}
		Type baseViewModelTypeCore;
		[DefaultValue(null), Category("ViewModel"), RefreshProperties(RefreshProperties.All)]
		[TypeConverter("DevExpress.Utils.MVVM.Design.ViewModelSourceObjectTypeConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
		public Type BaseViewModelType {
			[System.Diagnostics.DebuggerStepThrough]
			get { return baseViewModelTypeCore; }
			set {
				if(baseViewModelTypeCore == value) return;
				baseViewModelTypeCore = value;
				OnBaseViewModelTypeChanged();
			}
		}
		IFilteringUIProvider controlCore;
		[DefaultValue(null), RefreshProperties(RefreshProperties.All)]
		[Category("UI Provider")]
		public IFilteringUIProvider Control {
			get { return controlCore; }
			set {
				if(controlCore == value) return;
				UnsubscribeControlEvents();
				controlCore = value;
				SubscribeControlEvents();
			}
		}
		void SubscribeControlEvents() {
			var disposableComponent = Control as Component;
			if(disposableComponent != null)
				disposableComponent.Disposed += Control_Disposed;
		}
		void UnsubscribeControlEvents() {
			var disposableComponent = Control as Component;
			if(disposableComponent != null)
				disposableComponent.Disposed -= Control_Disposed;
		}
		void Control_Disposed(object sender, EventArgs e) {
			var disposableComponent = sender as Component;
			if(disposableComponent != null)
				disposableComponent.Disposed -= Control_Disposed;
			controlCore = null;
		}
		public void RetrieveFields() {
			if(Control == null || isDisposing)
				return;
			if(!IsDesignMode)
				Control.RetrieveFields(ViewModelProvider.ViewModel, ViewModelProvider.ViewModelType);
			else {
				var attributes = GetCustomMetricAttributes();
				ViewModelProvider.RetrieveFields((t) => Control.RetrieveFields(this, t), ModelType, attributes, BaseViewModelType);
			}
		}
		[Browsable(false)]
		public Type ViewModelType {
			get { return (!isDisposing && !IsDesignMode) ? ViewModelProvider.ViewModelType : BaseViewModelType; }
		}
		[Browsable(false)]
		public object ViewModel {
			get { return (!isDisposing && !IsDesignMode) ? ViewModelProvider.ViewModel : null; }
		}
		[Browsable(false)]
		public CriteriaOperator FilterCriteria {
			get { return (!isDisposing && !IsDesignMode) ? ViewModelProvider.FilterCriteria : null; }
		}
		public void ClearFilterCriteria() {
			if((isDisposing || IsDesignMode)) 
				return;
			ViewModelProvider.ClearFilterCriteria();
		}
		protected virtual void OnModelTypeChanged() {
			if(!isDisposing && !IsDesignMode)
				ViewModelProvider.SourceType = ModelType;
		}
		protected virtual void OnBaseViewModelTypeChanged() {
			if(!isDisposing && !IsDesignMode)
				ViewModelProvider.ViewModelBaseType = BaseViewModelType;
		}
		#endregion
		#region ParentViewModel
		object parentViewModelCore;
		[Browsable(false)]
		[DefaultValue(null), Category("Parent ViewModel"), RefreshProperties(RefreshProperties.All)]
		public object ParentViewModel {
			[System.Diagnostics.DebuggerStepThrough]
			get { return parentViewModelCore; }
			set {
				if(parentViewModelCore == value) return;
				parentViewModelCore = value;
				OnParentViewModelChanged();
			}
		}
		IViewModelProvider parentViewModelProviderCore;
		[DefaultValue(null), Category("Parent ViewModel"), RefreshProperties(RefreshProperties.All)]
		public IViewModelProvider ParentViewModelProvider {
			[System.Diagnostics.DebuggerStepThrough]
			get { return parentViewModelProviderCore; }
			set {
				if(parentViewModelProviderCore == value) return;
				parentViewModelProviderCore = value;
				OnParentViewModelProviderChanged();
			}
		}
		protected virtual void OnParentViewModelChanged() {
			if(!isDisposing && !IsDesignMode)
				ViewModelProvider.ParentViewModel = ParentViewModel;
		}
		protected virtual void OnParentViewModelProviderChanged() {
			if(!isDisposing && !IsDesignMode)
				ViewModelProvider.ParentViewModelProvider = ParentViewModelProvider;
		}
		CustomMetricAttributesCollection customMetricAttributesCore;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Model"), RefreshProperties(RefreshProperties.All)]
		public CustomMetricAttributesCollection CustomMetricAttributes {
			[System.Diagnostics.DebuggerStepThrough]
			get { return customMetricAttributesCore; }
		}
		protected virtual CustomMetricAttributesCollection CreateCustomMetricAttributes() {
			return new CustomMetricAttributesCollection(this);
		}
		void CustomMetricAttributes_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			ObjectChanged("CustomMetricAttributes");
		}
		#endregion
		#region Providers
		Lazy<BaseEndUserFilteringViewModelProviderFactory> providerFactoryCore;
		protected BaseEndUserFilteringViewModelProviderFactory ProviderFactory {
			get { return providerFactoryCore.Value; }
		}
		Lazy<IEndUserFilteringViewModelProvider> viewModelProviderCore;
		protected IEndUserFilteringViewModelProvider ViewModelProvider {
			get { return viewModelProviderCore.Value; }
		}
		protected virtual BaseEndUserFilteringViewModelProviderFactory CreateProviderFactory() {
			return IsDesignMode ? new BaseEndUserFilteringViewModelProviderFactory() : new EndUserFilteringViewModelProviderFactory(this);
		}
		protected virtual IEndUserFilteringViewModelProvider CreateViewModelProvider() {
			var provider = ProviderFactory.CreateViewModelProvider();
			SubscribeViewModelProvider(provider);
			return provider;
		}
		void SubscribeViewModelProvider(IEndUserFilteringViewModelProvider viewModelProvider) {
			INotifyPropertyChanged npc = viewModelProvider as INotifyPropertyChanged;
			if(npc != null)
				npc.PropertyChanged += ViewModelProvider_PropertyChanged;
		}
		void UnsubscribeViewModelProvider(IEndUserFilteringViewModelProvider viewModelProvider) {
			INotifyPropertyChanged npc = viewModelProvider as INotifyPropertyChanged;
			if(npc != null)
				npc.PropertyChanged -= ViewModelProvider_PropertyChanged;
		}
		void ViewModelProvider_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == "ViewModelType")
				ResetTypedListProperties();
			if(e.PropertyName == "ViewModel")
				RaiseViewModelChanged();
			if(e.PropertyName == "FilterCriteria")
				RaiseFilterCriteriaChanged(FilterCriteria);
		}
		#endregion Providers
		#region Events
		readonly static object viewModelChanged = new object();
		readonly static object filterCriteriaChanged = new object();
		readonly static object queryRangeData = new object();
		readonly static object queryLookupData = new object();
		readonly static object queryBooleanChoiceData = new object();
		[Category("ViewModel")]
		public event EventHandler ViewModelChanged {
			add { Events.AddHandler(viewModelChanged, value); }
			remove { Events.RemoveHandler(viewModelChanged, value); }
		}
		void RaiseViewModelChanged() {
			var handler = Events[viewModelChanged] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		[Category("ViewModel")]
		public event FilterCiteriaChangedEventHandler FilterCriteriaChanged {
			add { Events.AddHandler(filterCriteriaChanged, value); }
			remove { Events.RemoveHandler(filterCriteriaChanged, value); }
		}
		void RaiseFilterCriteriaChanged(CriteriaOperator filterCriteria) {
			var handler = Events[filterCriteriaChanged] as FilterCiteriaChangedEventHandler;
			if(handler != null) handler(this, new FilterCiteriaChangedEventArgs(filterCriteria));
		}
		[Category("Data")]
		public event QueryDataEventHandler<QueryRangeDataEventArgs, RangeData> QueryRangeData {
			add { Events.AddHandler(queryRangeData, value); }
			remove { Events.RemoveHandler(queryRangeData, value); }
		}
		internal void RaiseQueryRangeData(QueryRangeDataEventArgs args) {
			var handler = Events[queryRangeData] as QueryDataEventHandler<QueryRangeDataEventArgs, RangeData>;
			if(handler != null) handler(this, args);
		}
		[Category("Data")]
		public event QueryDataEventHandler<QueryLookupDataEventArgs, LookupData> QueryLookupData {
			add { Events.AddHandler(queryLookupData, value); }
			remove { Events.RemoveHandler(queryLookupData, value); }
		}
		internal void RaiseQueryLookupData(QueryLookupDataEventArgs args) {
			var handler = Events[queryLookupData] as QueryDataEventHandler<QueryLookupDataEventArgs, LookupData>;
			if(handler != null) handler(this, args);
		}
		[Category("Data")]
		public event QueryDataEventHandler<QueryBooleanChoiceDataEventArgs, BooleanChoiceData> QueryBooleanChoiceData {
			add { Events.AddHandler(queryBooleanChoiceData, value); }
			remove { Events.RemoveHandler(queryBooleanChoiceData, value); }
		}
		internal void RaiseQueryBooleanChoiceData(QueryBooleanChoiceDataEventArgs args) {
			var handler = Events[queryBooleanChoiceData] as QueryDataEventHandler<QueryBooleanChoiceDataEventArgs, BooleanChoiceData>;
			if(handler != null) handler(this, args);
		}
		#endregion Events
		#region ISupportInitialize
		int initializing = 0;
		void ISupportInitialize.BeginInit() {
			initializing++;
		}
		void ISupportInitialize.EndInit() {
			if(--initializing == 0)
				OnInitialized();
		}
		protected virtual void OnInitialized() {
			if(!isDisposing && !DesignMode)
				EnsureCustomMetricAttributes();
		}
		protected void EnsureCustomMetricAttributes() {
			if(CustomMetricAttributes.Count > 0)
				ViewModelProvider.Attributes = GetCustomMetricAttributes().ToArray();
		}
		IEnumerable<IEndUserFilteringMetricAttributes> GetCustomMetricAttributes() {
			var factory = ProviderFactory.GetService<IEndUserFilteringMetricAttributesFactory>();
			foreach(CustomMetricsAttributeExpression attributes in CustomMetricAttributes)
				yield return factory.Create(attributes.Path, attributes.Type, (from a in attributes.Attributes select a.Attribute).ToArray());
		}
		protected void ObjectChanged(string member = null) {
			FireChanging(member);
			FireChanged(member);
		}
		protected void FireChanging(string member = null) {
			if(!DesignMode || initializing > 0) return;
			var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(changeService != null) {
				MemberDescriptor memberDescriptor = null;
				if(member != null) {
					var properties = TypeDescriptor.GetProperties(this);
					memberDescriptor = properties[member];
				}
				changeService.OnComponentChanging(this, memberDescriptor);
			}
		}
		protected void FireChanged(string member = null) {
			if(!DesignMode || initializing > 0) return;
			var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(changeService != null) {
				MemberDescriptor memberDescriptor = null;
				if(member != null) {
					var properties = TypeDescriptor.GetProperties(this);
					memberDescriptor = properties[member];
				}
				changeService.OnComponentChanged(this, memberDescriptor, null, null);
			}
		}
		#endregion ISupportInitialize
		#region Binding
		public IPropertyBinding SetFilterCriteriaBinding<TDestination>(TDestination dest, Expression<Func<TDestination, CriteriaOperator>> selectorExpression) where TDestination : class {
			if(isDisposing || IsDesignMode) return null;
			return Register(BindingHelper.SetBinding<TDestination, CriteriaOperator>(dest, selectorExpression, ViewModelProvider, typeof(IEndUserFilteringViewModelProvider), "FilterCriteria"));
		}
		PropertyDescriptorCollection typedListProperties;
		void ResetTypedListProperties() {
			if(typedListProperties != null)
				typedListProperties.Clear();
			typedListProperties = null;
		}
		readonly static PropertyDescriptorCollection Empty = new PropertyDescriptorCollection(
			new PropertyDescriptor[] { }, true);
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(isDisposing || IsDesignMode)
				return Empty;
			if(typedListProperties == null) {
				var properties = TypeDescriptor.GetProperties(ViewModelType);
				typedListProperties = new PropertyDescriptorCollection(
					properties.OfType<PropertyDescriptor>()
						.Select(x => new ViewModelPropertyDescriptor(x, () => ViewModel)).ToArray());
			}
			return typedListProperties;
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return null;
		}
		class ViewModelPropertyDescriptor : PropertyDescriptor {
			readonly PropertyDescriptor source;
			Lazy<WeakReference> viewModelRef;
			public ViewModelPropertyDescriptor(PropertyDescriptor source, Func<object> viewModelAccessor)
				: base(source.Name, source.Attributes.OfType<Attribute>().ToArray()) {
				this.source = source;
				this.viewModelRef = new Lazy<WeakReference>(() => new WeakReference(viewModelAccessor()));
			}
			object GetViewModel() {
				return viewModelRef.Value.Target;
			}
			public override Type ComponentType {
				get { return source.ComponentType; }
			}
			public override Type PropertyType {
				get { return source.PropertyType; }
			}
			public override bool IsReadOnly {
				get { return source.IsReadOnly; }
			}
			public override bool SupportsChangeEvents {
				get { return source.SupportsChangeEvents; }
			}
			public override bool CanResetValue(object component) {
				return source.CanResetValue(GetViewModel());
			}
			public override object GetValue(object component) {
				return source.GetValue(GetViewModel());
			}
			public override void ResetValue(object component) {
				source.ResetValue(GetViewModel());
			}
			public override void SetValue(object component, object value) {
				source.SetValue(GetViewModel(), value);
			}
			public override bool ShouldSerializeValue(object component) {
				return source.ShouldSerializeValue(GetViewModel());
			}
			public override void RemoveValueChanged(object component, EventHandler handler) {
				source.RemoveValueChanged(GetViewModel(), handler);
			}
			public override void AddValueChanged(object component, EventHandler handler) {
				source.AddValueChanged(GetViewModel(), handler);
			}
		}
		#endregion Binding
		#region EndUserFilteringViewModelProviderFactory
		class EndUserFilteringViewModelProviderFactory : BaseEndUserFilteringViewModelProviderFactory {
			FilteringUIContext context;
			public EndUserFilteringViewModelProviderFactory(FilteringUIContext context) {
				this.context = context;
			}
			protected override IViewModelFactory GetViewModelFactory() {
				return new POCOViewModelFactory();
			}
			protected override IEndUserFilteringViewModelProvider CreateViewModelProvider(IServiceProvider serviceProvider) {
				return new EndUserFilteringViewModelProvider(serviceProvider, context);
			}
			class POCOViewModelFactory : IViewModelFactory {
				public object Create(Type viewModelType, IViewModelBuilder builder) {
					return POCOTypesFactory.Create(viewModelType, builder.TypeNameModifier, builder.ForceBindableProperty, builder.BuildBindablePropertyAttributes);
				}
			}
			class EndUserFilteringViewModelProvider : Internal.EndUserFilteringViewModelProvider {
				FilteringUIContext context;
				public EndUserFilteringViewModelProvider(IServiceProvider serviceProvider, FilteringUIContext context)
					: base(serviceProvider) {
					this.context = context;
				}
				protected override void RaiseRangeMetricAttributesQuery(QueryRangeDataEventArgs e) {
					context.RaiseQueryRangeData(e);
				}
				protected override void RaiseLookupMetricAttributesQuery(QueryLookupDataEventArgs e) {
					context.RaiseQueryLookupData(e);
				}
				protected override void RaiseBooleanChoiceMetricAttributesQuery(QueryBooleanChoiceDataEventArgs e) {
					context.RaiseQueryBooleanChoiceData(e);
				}
			}
		}
		#endregion
	}
	public interface IFilteringUIProvider {
		void RetrieveFields(object filteringViewModel, Type filteringViewModelType = null);
	}
}
