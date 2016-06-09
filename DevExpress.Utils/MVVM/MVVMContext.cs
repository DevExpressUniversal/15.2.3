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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.Design;
	using System.Drawing;
	using System.Linq.Expressions;
	using System.Windows.Forms;
	[Designer("DevExpress.Utils.MVVM.Design.MVVMContextDesigner, " + AssemblyInfo.SRAssemblyDesignFull, typeof(IDesigner))]
	[Description("A component that generates a ViewModel and manages its lifecycle.")]
	[ToolboxTabName(AssemblyInfo.DXTabComponents), DXToolboxItem(DXToolboxItemKind.Free)]
	[ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "MVVMContext")]
	public class MVVMContext : Component, IBindingSourceProvider, IRegistrationSourceProvider, IViewModelProvider, ISupportInitialize {
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinMVVM));
		}
		#region ctor
		static MVVMContext() {
			if(!DevExpress.Utils.Design.DesignTimeTools.IsDesignMode) {
				DisableDefaultUseCommandManager();
				RegisterXtraMessageBoxService();
				RegisterXtraDialogService();
				RegisterXtraFormWindowedDocumentManagerService();
				RegisterSplashScreenService();
			}
		}
		public MVVMContext()
			: this(null, System.Reflection.Assembly.GetCallingAssembly()) {
		}
		public MVVMContext(IContainer container)
			: this(container, System.Reflection.Assembly.GetCallingAssembly()) {
		}
		protected MVVMContext(IContainer container, System.Reflection.Assembly uiAssembly) {
			if(container != null)
				container.Add(this);
			lock(syncObj)
				contexts.Add(this);
			RegisterUIAssembly(uiAssembly);
			RegisterDispatcherService();
			this.bindingExpressionsCore = CreateBindingExpressions();
			BindingExpressions.CollectionChanged += BindingExpressions_CollectionChanged;
			this.registrationExpressionsCore = CreateRegistrationExpressions();
			RegistrationExpressions.CollectionChanged += RegistrationExpressions_CollectionChanged;
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if(!isDisposing) {
				isDisposing = true;
				OnDisposing();
				lock(syncObj)
					contexts.Remove(this);
			}
			base.Dispose(disposing);
		}
		bool isDisposing;
		protected virtual void OnDisposing() {
			if(BindingExpressions != null)
				BindingExpressions.CollectionChanged -= BindingExpressions_CollectionChanged;
			if(RegistrationExpressions != null)
				RegistrationExpressions.CollectionChanged -= RegistrationExpressions_CollectionChanged;
			SetParentViewModelCore(null);
			Ref.Dispose(ref bindingExpressionsCore);
			Ref.Dispose(ref registrationExpressionsCore);
			Ref.Dispose(ref disposableObjects);
			this.parentViewModelRefCore = null;
			this.parameterRefCore = null;
			this.viewModelTypeCore = null;
			this.viewModelConstructorParametersCore = null;
			this.viewModelConstructorParameterCore = null;
			this.viewModelCore = null;
		}
		DisposableObjectsContainer disposableObjects = new DisposableObjectsContainer();
		protected internal TDisposable Register<TDisposable>(TDisposable obj) where TDisposable : IDisposable {
			return (disposableObjects != null) ? disposableObjects.Register(obj) : obj;
		}
		#region Properties
		ContainerControl containerControlCore;
		[DefaultValue(null), Category(XtraEditors.CategoryName.Behavior), RefreshProperties(RefreshProperties.All)]
		public ContainerControl ContainerControl {
			[System.Diagnostics.DebuggerStepThrough]
			get { return containerControlCore; }
			set {
				if(value == containerControlCore) return;
				containerControlCore = value;
				ResetViewModel();
			}
		}
		Type viewModelTypeCore;
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Data), RefreshProperties(RefreshProperties.All)]
		[TypeConverter("DevExpress.Utils.MVVM.Design.ViewModelSourceObjectTypeConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
		public Type ViewModelType {
			[System.Diagnostics.DebuggerStepThrough]
			get { return viewModelTypeCore; }
			set {
				if(viewModelTypeCore == value) return;
				viewModelTypeCore = value;
				ResetViewModel();
			}
		}
		object viewModelConstructorParameterCore;
		[DefaultValue(null), Browsable(false), Category(DevExpress.XtraEditors.CategoryName.Data)]
		public object ViewModelConstructorParameter {
			[System.Diagnostics.DebuggerStepThrough]
			get { return viewModelConstructorParameterCore; }
			set {
				if(viewModelConstructorParameterCore == value) return;
				viewModelConstructorParameterCore = value;
				ResetViewModel();
			}
		}
		object[] viewModelConstructorParametersCore;
		[DefaultValue(null), Browsable(false), Category(DevExpress.XtraEditors.CategoryName.Data)]
		public object[] ViewModelConstructorParameters {
			[System.Diagnostics.DebuggerStepThrough]
			get { return viewModelConstructorParametersCore; }
			set {
				if(viewModelConstructorParametersCore == value) return;
				viewModelConstructorParametersCore = value;
				ResetViewModel();
			}
		}
		object viewModelCore;
		protected internal object ViewModel {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(viewModelCore == null)
					EnsureViewModelCreated();
				return viewModelCore;
			}
		}
		protected virtual void OnViewModelCreated() {
			RaiseSourceChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsViewModelCreated {
			[System.Diagnostics.DebuggerStepThrough]
			get { return viewModelCore != null; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		public void SetViewModel(Type viewModelType, object viewModel) {
			if(viewModelType == null || viewModel == null || !viewModelType.IsAssignableFrom(viewModel.GetType())) return;
			this.viewModelTypeCore = viewModelType;
			this.viewModelCore = viewModel;
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior), Browsable(false)]
		public object ParentViewModel {
			get { return GetParentViewModel(); }
			set { SetParentViewModel(value); }
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Data), Browsable(false)]
		public object Parameter {
			get { return GetParameter(); }
			set { SetParameter(value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDesignMode {
			get { return DesignMode || DevExpress.Utils.Design.DesignTimeTools.IsDesignMode; }
		}
		#endregion Properties
		WeakReference parentViewModelRefCore;
		[System.Diagnostics.DebuggerStepThrough]
		protected object GetParentViewModel() {
			return (parentViewModelRefCore != null) ? parentViewModelRefCore.Target : null;
		}
		protected void SetParentViewModel(object parentViewModel) {
			this.parentViewModelRefCore = (parentViewModel != null) ? new WeakReference(parentViewModel) : null;
			SetParentViewModelCore(parentViewModel);
		}
		void SetParentViewModelCore(object parentViewModel) {
			if(viewModelCore != null) {
				var pocoInterfaces = GetPOCOInterfaces();
				if(pocoInterfaces != null)
					pocoInterfaces.SetParentViewModel(viewModelCore, parentViewModel);
			}
		}
		void CheckParentViewModel() {
			if(parentViewModelRefCore == null) return;
			SetParentViewModelCore(GetParentViewModel());
		}
		WeakReference parameterRefCore;
		[System.Diagnostics.DebuggerStepThrough]
		protected object GetParameter() {
			return (parameterRefCore != null) ? parameterRefCore.Target : null;
		}
		protected void SetParameter(object parameter) {
			this.parameterRefCore = (parameter != null) ? new WeakReference(parameter) : null;
			SetParameterCore(parameter);
		}
		void SetParameterCore(object parameter) {
			if(viewModelCore != null) {
				var pocoInterfaces = GetPOCOInterfaces();
				if(pocoInterfaces != null)
					pocoInterfaces.SetParameter(viewModelCore, parameter);
			}
		}
		void CheckParameter() {
			if(parameterRefCore == null) return;
			SetParameterCore(GetParameter());
		}
		void ResetViewModel() {
			if(viewModelCore != null) {
				this.viewModelCore = null;
				RaiseSourceChanged();
			}
		}
		[System.Diagnostics.DebuggerStepThrough]
		protected internal virtual IViewModelSource GetViewModelSource() {
			return POCOViewModelSourceProxy.Instance;
		}
		[System.Diagnostics.DebuggerStepThrough]
		protected internal virtual IPOCOInterfaces GetPOCOInterfaces() {
			return GetDefaultPOCOInterfaces();
		}
		static IPOCOInterfaces GetDefaultPOCOInterfaces() {
			return POCOInterfacesProxy.Instance;
		}
		[System.Diagnostics.DebuggerStepThrough]
		public TViewModel GetViewModel<TViewModel>() {
			return (TViewModel)ViewModel;
		}
		[System.Diagnostics.DebuggerStepThrough]
		public TViewModel GetParentViewModel<TViewModel>() {
			return (TViewModel)GetPOCOInterfaces().GetParentViewModel(ViewModel);
		}
		public TService GetService<TService>() where TService : class {
			return GetServiceFromViewModel<TService>(GetPOCOInterfaces(), ViewModel);
		}
		public TService GetService<TService>(string key) where TService : class {
			return GetServiceFromViewModel<TService>(GetPOCOInterfaces(), ViewModel, key);
		}
		static TService GetServiceFromViewModel<TService>(IPOCOInterfaces pocoInterfaces, object viewModel) where TService : class {
			return (viewModel != null) ? GetServiceCore<TService>(pocoInterfaces, pocoInterfaces.GetServiceContainer(viewModel)) : null;
		}
		static TService GetServiceFromViewModel<TService>(IPOCOInterfaces pocoInterfaces, object viewModel, string key) where TService : class {
			return (viewModel != null) ? GetServiceCore<TService>(pocoInterfaces, pocoInterfaces.GetServiceContainer(viewModel), key) : null;
		}
		static TService GetServiceCore<TService>(IPOCOInterfaces pocoInterfaces, object serviceContainer, params object[] parameters) where TService : class {
			return (serviceContainer != null) ? pocoInterfaces.GetService<TService>(serviceContainer, parameters) : null;
		}
		void SetDocumentOwner(object documentOwner) {
			SetDocumentOwnerCore(GetPOCOInterfaces(), ViewModel, documentOwner);
		}
		void OnClose(System.ComponentModel.CancelEventArgs e) {
			OnCloseCore(GetPOCOInterfaces(), ViewModel, e);
		}
		void OnDestroy() {
			OnDestroyCore(GetPOCOInterfaces(), ViewModel);
		}
		object GetTitle() {
			return GetTitleCore(GetPOCOInterfaces(), ViewModel);
		}
		static void SetDocumentOwnerCore(IPOCOInterfaces pocoInterfaces, object viewModel, object documentOwner) {
			if(viewModel != null) pocoInterfaces.SetDocumentOwner(viewModel, documentOwner);
		}
		static void OnCloseCore(IPOCOInterfaces pocoInterfaces, object viewModel, System.ComponentModel.CancelEventArgs e) {
			if(viewModel != null) pocoInterfaces.OnClose(viewModel, e);
		}
		static void OnDestroyCore(IPOCOInterfaces pocoInterfaces, object viewModel) {
			if(viewModel != null) pocoInterfaces.OnDestroy(viewModel);
		}
		static object GetTitleCore(IPOCOInterfaces pocoInterfaces, object viewModel) {
			return (viewModel != null) ? pocoInterfaces.GetTitle(viewModel) : null;
		}
		public IPropertyBinding SetBinding<TDestination, TValue>(TDestination dest, Expression<Func<TDestination, TValue>> selectorExpression, string propertyName)
			where TDestination : class {
			return Register(BindingHelper.SetBinding<TDestination, TValue>(dest, selectorExpression, ViewModel, ViewModelType, propertyName));
		}
		public IPropertyBinding SetParentBinding<TDestination, TValue>(TDestination dest, Expression<Func<TDestination, TValue>> selectorExpression, string propertyName)
			where TDestination : class {
			object parentViewModel = GetPOCOInterfaces().GetParentViewModel(ViewModel);
			return Register(BindingHelper.SetBinding<TDestination, TValue>(dest, selectorExpression, parentViewModel, parentViewModel.GetType(), propertyName));
		}
		public IPropertyBinding SetBinding<TDestination, TViewModel, TValue>(TDestination dest, Expression<Func<TDestination, TValue>> destSelectorExpression, Expression<Func<TViewModel, TValue>> sourceSelectorExpression)
			where TDestination : class {
			string propertyName = ExpressionHelper.GetPath(sourceSelectorExpression);
			return Register(BindingHelper.SetBinding<TDestination, TValue>(dest, destSelectorExpression, GetViewModel<TViewModel>(), typeof(TViewModel), propertyName));
		}
		public IPropertyBinding SetBinding<TSourceEventArgs, TViewModel, TValue>(Expression<Func<TViewModel, TValue>> selectorExpression,
			object source, string sourceEventName, Func<TSourceEventArgs, TValue> sourceEventArgsConverter)
			where TViewModel : class
			where TSourceEventArgs : EventArgs {
			return Register(BindingHelper.SetBinding<TSourceEventArgs, TViewModel, TValue>(GetViewModel<TViewModel>(), selectorExpression, source, sourceEventName, sourceEventArgsConverter));
		}
		public IPropertyBinding SetBinding<TSourceEventArgs, TSource, TViewModel, TValue>(Expression<Func<TViewModel, TValue>> selectorExpression,
			TSource source, string sourceEventName, Func<TSourceEventArgs, TValue> sourceEventArgsConverter, Action<TSource, TValue> updateSourceAction)
			where TViewModel : class
			where TSourceEventArgs : EventArgs {
			return Register(BindingHelper.SetBinding<TSourceEventArgs, TSource, TViewModel, TValue>(GetViewModel<TViewModel>(), selectorExpression, source, sourceEventName, sourceEventArgsConverter, updateSourceAction));
		}
		public IPropertyBinding SetBinding<TDestination, TDestValue, TViewModel, TValue>(TDestination dest, Expression<Func<TDestination, TDestValue>> destSelectorExpression,
			Expression<Func<TViewModel, TValue>> sourceSelectorExpression, Func<TValue, TDestValue> convert = null, Func<TDestValue, TValue> convertBack = null)
			where TDestination : class
			where TViewModel : class {
			string propertyName = ExpressionHelper.GetPath(sourceSelectorExpression);
			return Register(BindingHelper.SetBinding<TDestination, TDestValue, TViewModel, TValue>(dest, destSelectorExpression, GetViewModel<TViewModel>(), typeof(TViewModel), propertyName, convert, convertBack));
		}
		public IDisposable SetTrigger<TViewModel, TValue>(Expression<Func<TViewModel, TValue>> selectorExpression, Action<TValue> triggerAction)
			where TViewModel : class {
			return Register(BindingHelper.SetNPCTrigger<TViewModel, TValue>(GetViewModel<TViewModel>(), selectorExpression, triggerAction));
		}
		public IDisposable AttachBehavior<TBehavior>(object source)
			where TBehavior : BehaviorBase {
			return Register((source != null) ? BehaviorHelper.AttachCore<TBehavior>(source, ViewModel, GetViewModelSource(), GetPOCOInterfaces()) : null);
		}
		public IDisposable AttachBehavior<TBehavior>(object source, Action<TBehavior> behaviorSettings, params object[] parameters)
			where TBehavior : BehaviorBase {
			return Register((source != null) ? BehaviorHelper.AttachCore<TBehavior>(source, ViewModel, GetViewModelSource(), GetPOCOInterfaces(), behaviorSettings, parameters) : null);
		}
		public void DetachBehavior<TBehavior>(object source)
			where TBehavior : BehaviorBase {
			BehaviorHelper.Detach<TBehavior>(source);
		}
		public void DetachBehavior(object source) {
			BehaviorHelper.Detach(source);
		}
		public void RegisterService(object service) {
			if(service == null) return;
			var pocoInterfaces = GetPOCOInterfaces();
			object serviceContainer = pocoInterfaces.GetServiceContainer(ViewModel);
			if(serviceContainer != null)
				pocoInterfaces.RegisterService(serviceContainer, service);
		}
		public void RegisterService(string key, object service) {
			if(service == null) return;
			var pocoInterfaces = GetPOCOInterfaces();
			object serviceContainer = pocoInterfaces.GetServiceContainer(ViewModel);
			if(serviceContainer != null)
				pocoInterfaces.RegisterService(serviceContainer, key, service);
		}
		public void RegisterDefaultService(object service) {
			RegisterDefaultServiceCore(service, GetPOCOInterfaces());
		}
		public void RegisterDefaultService(string key, object service) {
			RegisterDefaultServiceCore(key, service, GetPOCOInterfaces());
		}
		static void RegisterDefaultServiceCore(object service, IPOCOInterfaces pocoInterfaces) {
			if(service == null) return;
			object serviceContainer = pocoInterfaces.GetDefaultServiceContainer();
			pocoInterfaces.RegisterService(serviceContainer, service);
		}
		static void RegisterDefaultServiceCore(string key, object service, IPOCOInterfaces pocoInterfaces) {
			if(service == null) return;
			object serviceContainer = pocoInterfaces.GetDefaultServiceContainer();
			pocoInterfaces.RegisterService(serviceContainer, key, service);
		}
		void EnsureViewModelCreated() {
			if(IsViewModelCreated) return;
			if(viewModelTypeCore != null) {
				viewModelCore = CreateViewModel();
				CheckParentViewModel();
				CheckParameter();
				OnViewModelCreated();
			}
		}
		object CreateViewModel() {
			IViewModelSource viewModelSource = GetViewModelSource();
			return viewModelSource.Create(ViewModelType, GetViewModelConstructorParameters());
		}
		object[] GetViewModelConstructorParameters() {
			if(ViewModelConstructorParameters != null)
				return ViewModelConstructorParameters;
			if(ViewModelConstructorParameter != null)
				return new object[] { ViewModelConstructorParameter };
			return new object[] { };
		}
		public void BindCommand<TViewModel, T>(ISupportCommandBinding control, Expression<Action<TViewModel, T>> commandSelector, Expression<Func<TViewModel, T>> commandParameterSelector = null) {
			TViewModel viewModel = GetViewModel<TViewModel>();
			if(control != null)
				Register(control.BindCommand<T>(ExpressionHelper.Reduce(commandSelector, viewModel), viewModel, ExpressionHelper.ReduceAndCompile(commandParameterSelector, viewModel)));
		}
		public void BindCommand<TViewModel>(ISupportCommandBinding control, Expression<Action<TViewModel>> commandSelector, Expression<Func<TViewModel, object>> commandParameterSelector = null) {
			TViewModel viewModel = GetViewModel<TViewModel>();
			if(control != null)
				Register(control.BindCommand(ExpressionHelper.Reduce(commandSelector, viewModel), viewModel, ExpressionHelper.ReduceAndCompile(commandParameterSelector, viewModel)));
		}
		public void BindCancelCommand<TViewModel, T>(ISupportCommandBinding control, Expression<Action<TViewModel, T>> asyncCommandSelector) {
			TViewModel viewModel = GetViewModel<TViewModel>();
			if(control != null)
				Register(control.BindCommand(GetCancelCommand(ExpressionHelper.Reduce(asyncCommandSelector, viewModel), viewModel)));
		}
		public void BindCancelCommand<TViewModel>(ISupportCommandBinding control, Expression<Action<TViewModel>> asyncCommandSelector) {
			TViewModel viewModel = GetViewModel<TViewModel>();
			if(control != null)
				Register(control.BindCommand(GetCancelCommand(asyncCommandSelector, viewModel)));
		}
		static object GetCancelCommand(Expression<Action> asyncCommandSelector, object source) {
			Type commandType;
			return GetCancelCommandCore(CommandHelper.GetCommand(asyncCommandSelector, source, out commandType));
		}
		static object GetCancelCommand<T>(Expression<Action<T>> asyncCommandSelector, object source) {
			Type commandType;
			return GetCancelCommandCore(CommandHelper.GetCommand(asyncCommandSelector, source, out commandType));
		}
		internal static object GetCancelCommandCore(object command) {
			return CommandHelper.GetCancelCommandCore(MVVMTypesResolver.Instance, command);
		}
		#region static
		public static void DisableDefaultUseCommandManager() {
			var commandBaseType = MVVMTypesResolver.Instance.GetCommandBaseType();
			InterfacesProxy.SetDefaultUseCommandManager(commandBaseType, false);
		}
		static void RegisterUIAssembly(System.Reflection.Assembly uiAssembly) {
			UI.ViewActivator.RegisterContextAssembly(uiAssembly);
		}
		#region Default Services
		void RegisterDispatcherService() {
			Register(Services.DispatcherService.Register(GetPOCOInterfaces()));
		}
		public static void RegisterMessageBoxService() {
			Services.MessageBoxServiceProxy.RegisterMessageBoxService();
		}
		public static void RegisterXtraMessageBoxService() {
			Services.MessageBoxServiceProxy.RegisterXtraMessageBoxService();
		}
		public static void RegisterFlyoutMessageBoxService() {
			Services.MessageBoxServiceProxy.RegisterFlyoutMessageBoxService();
		}
		public static void RegisterXtraDialogService() {
			Services.DialogService.RegisterXtraDialogService();
		}
		public static void RegisterFlyoutDialogService() {
			Services.DialogService.RegisterFlyoutDialogService();
		}
		public static void RegisterRibbonDialogService() {
			Services.DialogService.RegisterRibbonDialogService();
		}
		public static void RegisterFormWindowedDocumentManagerService() {
			Services.WindowedDocumentManagerService.RegisterFormService();
		}
		public static void RegisterXtraFormWindowedDocumentManagerService() {
			Services.WindowedDocumentManagerService.RegisterXtraFormService();
		}
		public static void RegisterSplashScreenService() {
			Services.SplashScreenService.Register();
		}
		public static void RegisterRibbonFormWindowedDocumentManagerService() {
			Services.WindowedDocumentManagerService.RegisterRibbonFormService();
		}
		public static void RegisterFlyoutFormWindowedDocumentManagerService() {
			Services.WindowedDocumentManagerService.RegisterFlyoutFormService();
		}
		#endregion Default Services
		readonly static List<MVVMContext> contexts = new List<MVVMContext>();
		readonly static object syncObj = new object();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static MVVMContext FromControl(Control control) {
			MVVMContext context = null;
			while(control != null) {
				context = FromControlCore(control);
				if(context != null)
					return context;
				control = control.Parent;
			}
			return null;
		}
		static MVVMContext FromControlCore(Control control) {
			lock(syncObj) {
				foreach(MVVMContext context in contexts) {
					if(context.ContainerControl == control)
						return context;
				}
				return null;
			}
		}
		public static object GetViewModel(Control container) {
			var context = FromControl(container);
			return (context != null) ? context.ViewModel : null;
		}
		public static TViewModel GetViewModel<TViewModel>(Control container) {
			var context = FromControl(container);
			if(context != null)
				return context.GetViewModel<TViewModel>();
			return default(TViewModel);
		}
		public static void SetParentViewModel(MVVMContext context, object parentViewModel) {
			if(context != null) context.SetParentViewModel(parentViewModel);
		}
		public static void SetParameter(MVVMContext context, object parameter) {
			if(context != null) context.SetParameter(parameter);
		}
		public static void SetParentViewModel(Control container, object parentViewModel) {
			SetParentViewModel(FromControl(container), parentViewModel);
		}
		public static void SetParameter(Control container, object parameter) {
			SetParameter(FromControl(container), parameter);
		}
		public static TService GetService<TService>(object viewModel) where TService : class {
			return GetServiceFromViewModel<TService>(GetDefaultPOCOInterfaces(), viewModel);
		}
		public static TService GetService<TService>(object viewModel, string key) where TService : class {
			return GetServiceFromViewModel<TService>(GetDefaultPOCOInterfaces(), viewModel, key);
		}
		public static TService GetDefaultService<TService>() where TService : class {
			var pocoInterfaces = GetDefaultPOCOInterfaces();
			return GetServiceCore<TService>(pocoInterfaces, pocoInterfaces.GetDefaultServiceContainer());
		}
		public static TService GetDefaultService<TService>(string key) where TService : class {
			var pocoInterfaces = GetDefaultPOCOInterfaces();
			return GetServiceCore<TService>(pocoInterfaces, pocoInterfaces.GetDefaultServiceContainer(), key);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetDocumentOwner(Control container, object documentOwner) {
			SetDocumentOwner(FromControl(container), documentOwner);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetDocumentOwner(MVVMContext context, object documentOwner) {
			if(context != null) context.SetDocumentOwner(documentOwner);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void OnClose(Control container, System.ComponentModel.CancelEventArgs e) {
			OnClose(FromControl(container), e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void OnClose(MVVMContext context, System.ComponentModel.CancelEventArgs e) {
			if(context != null) context.OnClose(e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void OnDestroy(Control container) {
			OnDestroy(FromControl(container));
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void OnDestroy(MVVMContext context) {
			if(context != null) context.OnDestroy();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static object GetTitle(Control container) {
			return GetTitle(FromControl(container));
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static object GetTitle(MVVMContext context) {
			return (context != null) ? context.GetTitle() : null;
		}
		#endregion static
		#region Extensions
		public IDisposable SetObjectDataSourceBinding<TViewModel, TModel>(BindingSource bindingSource,
			Expression<Func<TViewModel, TModel>> entitySelector, Expression<Action<TViewModel>> updateCommandSelector = null) where TViewModel : class {
			return Register(BindingSourceExtension.SetDataSourceBinding(bindingSource, this, entitySelector, updateCommandSelector));
		}
		public IDisposable SetItemsSourceBinding<TTarget, TItem, TViewModel, TModel>(TTarget target,
			Expression<Func<TTarget, IEnumerable<TItem>>> itemsSelector,
			Expression<Func<TViewModel, IEnumerable<TModel>>> collectionSelector,
			Func<TItem, TModel, bool> match, Func<TModel, TItem> create, Action<TItem, TModel> clear = null, Action<TItem, TModel> prepare = null)
			where TTarget : class, System.ComponentModel.IComponent
			where TViewModel : class {
			return Register(ItemsSourceExtension.SetItemsSourceBinding(target, itemsSelector, this, collectionSelector, match, create, clear, prepare));
		}
		public IDisposable SetItemsSourceBinding<TTarget, TItem, TViewModel, TModel>(TTarget target,
			Expression<Func<TViewModel, IEnumerable<TModel>>> collectionSelector,
			Func<TItem, TModel, bool> match, Func<TModel, TItem> create, Action<TItem, TModel> clear = null, Action<TItem, TModel> prepare = null)
			where TTarget : class, ISupportItemsSource<TItem>
			where TViewModel : class {
			var selector = ItemsSourceExtension.GetItemsSourceSelector<TTarget, TItem>(target);
			return Register(ItemsSourceExtension.SetItemsSourceBinding(target, selector, this, collectionSelector, match, create, clear, prepare));
		}
		public IDisposable SetItemsSourceBinding<TTarget, TContainer, TViewModel, TModel>(TTarget target,
			Expression<Func<TViewModel, IEnumerable<TModel>>> collectionSelector,
			Action<TContainer, TModel> prepare = null)
			where TTarget : class, IItemsControl<TContainer>
			where TViewModel : class {
			var selector = ItemsSourceExtension.GetItemsSourceSelector<TTarget, TContainer>(target);
			return Register(ItemsSourceExtension.SetItemsSourceBinding(target, selector, this, collectionSelector,
						(c, m) => target.IsContainerForItem(c, m),
						(m) => target.CreateContainerForItem(m),
						(c, m) => target.ClearContainerForItem(c, m),
						(c, m) =>
						{
							target.PrepareContainerForItem(c, m);
							if(prepare != null) prepare(c, m);
						}));
		}
		#endregion Extensions
		static readonly object sourceChanged = new object();
		protected void RaiseSourceChanged() {
			EventHandler handler = Events[sourceChanged] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		#region IViewModelProvider
		object IViewModelProvider.ViewModel {
			get { return viewModelCore; }
		}
		bool IViewModelProvider.IsViewModelCreated {
			get { return IsViewModelCreated; }
		}
		event EventHandler IViewModelProvider.ViewModelChanged {
			add { Events.AddHandler(sourceChanged, value); }
			remove { Events.RemoveHandler(sourceChanged, value); }
		}
		#endregion
		#region Binding Expressions
		object IBindingSourceProvider.Source {
			get { return viewModelCore; }
		}
		event EventHandler IBindingSourceProvider.SourceChanged {
			add { Events.AddHandler(sourceChanged, value); }
			remove { Events.RemoveHandler(sourceChanged, value); }
		}
		BindingExpressionCollection bindingExpressionsCore;
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category(DevExpress.XtraEditors.CategoryName.Data), RefreshProperties(RefreshProperties.All)]
		public BindingExpressionCollection BindingExpressions {
			[System.Diagnostics.DebuggerStepThrough]
			get { return bindingExpressionsCore; }
		}
		void BindingExpressions_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			ObjectChanged("BindingExpressions");
		}
		protected virtual BindingExpressionCollection CreateBindingExpressions() {
			return new BindingExpressionCollection(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void PopulateCommandBindingExpressions() {
			BindingExpressions.AddRange(GetCommandBindingExpressions());
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public BindingExpression[] GetCommandBindingExpressions() {
			if(ViewModelType == null)
				return new BindingExpression[0];
			var commandMethods = MemberInfoHelper.GetCommandMethods(MVVMTypesResolver.Instance, ViewModelType);
			BindingExpression[] expressions = new BindingExpression[commandMethods.Length];
			for(int i = 0; i < expressions.Length; i++) {
				var parameters = commandMethods[i].GetParameters();
				if(parameters.Length == 0)
					expressions[i] = CreateCommandBindingExpression(ViewModelType, commandMethods[i]);
				if(parameters.Length == 1)
					expressions[i] = CreateParameterizedCommandBindingExpression(ViewModelType, commandMethods[i]);
			}
			var asyncCommandMethods = Array.FindAll(commandMethods, m => m.ReturnType == typeof(System.Threading.Tasks.Task));
			BindingExpression[] asyncExpressions = new BindingExpression[asyncCommandMethods.Length];
			for(int i = 0; i < asyncExpressions.Length; i++)
				asyncExpressions[i] = CreateCancelCommandBindingExpression(ViewModelType, asyncCommandMethods[i]);
			BindingExpression[] result = new BindingExpression[commandMethods.Length + asyncCommandMethods.Length];
			Array.Copy(expressions, 0, result, 0, expressions.Length);
			Array.Copy(asyncExpressions, 0, result, expressions.Length, asyncExpressions.Length);
			return Array.FindAll(result, e => !e.IsHidden);
		}
		protected virtual CommandBindingExpression CreateCommandBindingExpression(Type viewModelType, System.Reflection.MethodInfo mInfo) {
			return new CommandBindingExpression(viewModelType, mInfo);
		}
		protected virtual CommandBindingExpression CreateCancelCommandBindingExpression(Type viewModelType, System.Reflection.MethodInfo mInfo) {
			return new CancelCommandBindingExpression(viewModelType, mInfo);
		}
		protected virtual CommandBindingExpression CreateParameterizedCommandBindingExpression(Type viewModelType, System.Reflection.MethodInfo mInfo) {
			return new ParameterizedCommandBindingExpression(viewModelType, mInfo);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void PopulatePropertyBindingExpressions() {
			BindingExpressions.AddRange(GetPropertyBindingExpressions());
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public BindingExpression[] GetPropertyBindingExpressions() {
			if(ViewModelType == null)
				return new BindingExpression[0];
			var bindableProperties = MemberInfoHelper.GetBindableProperties(MVVMTypesResolver.Instance, ViewModelType);
			BindingExpression[] expressions = new BindingExpression[bindableProperties.Length];
			for(int i = 0; i < expressions.Length; i++) {
				expressions[i] = CreatePropertyBindingExpression(ViewModelType, bindableProperties[i]);
			}
			return Array.FindAll(expressions, e => !e.IsHidden);
		}
		protected virtual PropertyBindingExpression CreatePropertyBindingExpression(Type viewModelType, System.Reflection.PropertyInfo pInfo) {
			return new PropertyBindingExpression(viewModelType, pInfo);
		}
		#endregion Binding Expressions
		#region Registration Expressions
		object IRegistrationSourceProvider.Source {
			get { return viewModelCore; }
		}
		event EventHandler IRegistrationSourceProvider.SourceChanged {
			add { Events.AddHandler(sourceChanged, value); }
			remove { Events.RemoveHandler(sourceChanged, value); }
		}
		RegistrationExpressionCollection registrationExpressionsCore;
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category(DevExpress.XtraEditors.CategoryName.Data), RefreshProperties(RefreshProperties.All)]
		public RegistrationExpressionCollection RegistrationExpressions {
			[System.Diagnostics.DebuggerStepThrough]
			get { return registrationExpressionsCore; }
		}
		void RegistrationExpressions_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			ObjectChanged("RegistrationExpressions");
		}
		protected virtual RegistrationExpressionCollection CreateRegistrationExpressions() {
			return new RegistrationExpressionCollection(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public RegistrationExpression[] GetServiceRegistrationExpressions() {
			if(ViewModelType == null) return new RegistrationExpression[0];
			return new RegistrationExpression[] { 
				new DispatcherServiceRegistrationExpression(null, false),
				new DialogServiceRegistrationExpression(null, false),
				new MessageBoxServiceRegistrationExpression(null, false),
				new DocumentManagerServiceRegistrationExpression(null, false),
				new WindowedDocumentManagerServiceRegistrationExpression(null, false),
				new LayoutSerializationServiceRegistrationExpression(null, false, DefaultBoolean.Default, this.ContainerControl),
				new SplashScreenServiceRegistrationExpression(null, false),
				new NotificationServiceRegistrationExpression(null, false),
				new ConfirmationBehaviorRegistrationExpression(),
				new EventToCommandBehaviorRegistrationExpression(ViewModelType),
				new EventToCommandBehaviorParameterizedRegistrationExpression(ViewModelType),
			};
		}
		#endregion
		#region Fluent API
		public MVVMContextFluentAPI<TViewModel> OfType<TViewModel>()
			where TViewModel : class {
			return MVVMContextFluentAPI<TViewModel>.OfType(this);
		}
		public MVVMContextFluentAPI<TViewModel, TEventArgs> WithEvent<TViewModel, TEventArgs>(object source, string eventName)
			where TViewModel : class
			where TEventArgs : EventArgs {
			return MVVMContextFluentAPI<TViewModel, TEventArgs>.WithEvent(this, source, eventName);
		}
		public MVVMContextFluentAPI<TViewModel, TSource, TSourceEventArgs> WithEvent<TViewModel, TSource, TSourceEventArgs>(TSource source, string eventName)
			where TViewModel : class
			where TSourceEventArgs : EventArgs {
			return MVVMContextFluentAPI<TViewModel, TSource, TSourceEventArgs>.WithEvent(this, source, eventName);
		}
		public MVVMContextFluentAPI<TViewModel, TSource, EventArgs> WithEvent<TViewModel, TSource>(TSource source, string eventName)
			where TViewModel : class {
			return MVVMContextFluentAPI<TViewModel, TSource, EventArgs>.WithEvent(this, source, eventName);
		}
		public MVVMContextConfirmationFluentAPI<TCancelEventArgs> WithEvent<TCancelEventArgs>(object source, string eventName)
			where TCancelEventArgs : CancelEventArgs {
			return MVVMContextConfirmationFluentAPI<TCancelEventArgs>.WithEvent(this, source, eventName);
		}
		#endregion Fluent API
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
			if(!DesignMode && (BindingExpressions.Count > 0 || RegistrationExpressions.Count > 0))
				EnsureViewModelCreated();
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
	}
	#region Fluent API
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class MVVMContextFluentAPI<TViewModel>
		where TViewModel : class {
		MVVMContext context;
		MVVMContextFluentAPI(MVVMContext context) {
			this.context = context;
		}
		internal static MVVMContextFluentAPI<TViewModel> OfType(MVVMContext context) {
			return new MVVMContextFluentAPI<TViewModel>(context);
		}
		public TViewModel ViewModel {
			get { return context.GetViewModel<TViewModel>(); }
		}
		public IDisposable SetTrigger<TValue>(Expression<Func<TViewModel, TValue>> selectorExpression, Action<TValue> triggerAction) {
			return context.SetTrigger<TViewModel, TValue>(selectorExpression, triggerAction);
		}
		public IPropertyBinding SetBinding<TDestination, TValue>(TDestination dest, Expression<Func<TDestination, TValue>> destSelectorExpression, Expression<Func<TViewModel, TValue>> sourceSelectorExpression)
			where TDestination : class {
			return context.SetBinding<TDestination, TViewModel, TValue>(dest, destSelectorExpression, sourceSelectorExpression);
		}
		public IPropertyBinding SetBinding<TSourceEventArgs, TValue>(Expression<Func<TViewModel, TValue>> selectorExpression,
			object source, string sourceEventName, Func<TSourceEventArgs, TValue> sourceEventArgsConverter)
			where TSourceEventArgs : EventArgs {
			return context.SetBinding<TSourceEventArgs, TViewModel, TValue>(selectorExpression, source, sourceEventName, sourceEventArgsConverter);
		}
		public IPropertyBinding SetBinding<TSourceEventArgs, TSource, TValue>(Expression<Func<TViewModel, TValue>> selectorExpression,
			TSource source, string sourceEventName, Func<TSourceEventArgs, TValue> sourceEventArgsConverter, Action<TSource, TValue> updateSourceAction)
			where TSourceEventArgs : EventArgs {
			return context.SetBinding<TSourceEventArgs, TSource, TViewModel, TValue>(selectorExpression, source, sourceEventName, sourceEventArgsConverter, updateSourceAction);
		}
		public IPropertyBinding SetBinding<TDestination, TDestValue, TValue>(TDestination destination, Expression<Func<TDestination, TDestValue>> destSelectorExpression,
			Expression<Func<TViewModel, TValue>> sourceSelectorExpression, Func<TValue, TDestValue> convert = null, Func<TDestValue, TValue> convertBack = null)
			where TDestination : class {
			return context.SetBinding<TDestination, TDestValue, TViewModel, TValue>(destination, destSelectorExpression, sourceSelectorExpression, convert, convertBack);
		}
		public IDisposable SetObjectDataSourceBinding<TModel>(BindingSource bindingSource,
			Expression<Func<TViewModel, TModel>> entitySelector, Expression<Action<TViewModel>> updateCommandSelector = null) {
			return context.SetObjectDataSourceBinding(bindingSource, entitySelector, updateCommandSelector);
		}
		public IDisposable SetItemsSourceBinding<TTarget, TItem, TModel>(TTarget target,
			Expression<Func<TTarget, IEnumerable<TItem>>> itemsSelector,
			Expression<Func<TViewModel, IEnumerable<TModel>>> collectionSelector,
			Func<TItem, TModel, bool> match, Func<TModel, TItem> create, Action<TItem, TModel> clear = null, Action<TItem, TModel> prepare = null)
			where TTarget : class, System.ComponentModel.IComponent {
			return context.SetItemsSourceBinding(target, itemsSelector, collectionSelector, match, create, clear, prepare);
		}
		public IDisposable SetItemsSourceBinding<TTarget, TItem, TModel>(TTarget target,
			Expression<Func<TViewModel, IEnumerable<TModel>>> collectionSelector,
			Func<TItem, TModel, bool> match, Func<TModel, TItem> create, Action<TItem, TModel> clear = null, Action<TItem, TModel> prepare = null)
			where TTarget : class, ISupportItemsSource<TItem> {
			return context.SetItemsSourceBinding(target, collectionSelector, match, create, clear, prepare);
		}
		public IDisposable SetItemsSourceBinding<TTarget, TContainer, TModel>(TTarget target,
			Expression<Func<TViewModel, IEnumerable<TModel>>> collectionSelector, Action<TContainer, TModel> prepare = null)
			where TTarget : class, IItemsControl<TContainer> {
			return context.SetItemsSourceBinding<TTarget, TContainer, TViewModel, TModel>(target, collectionSelector, prepare);
		}
		public void BindCommand<T>(ISupportCommandBinding control, Expression<Action<TViewModel, T>> commandSelector, Expression<Func<TViewModel, T>> commandParameterSelector = null) {
			context.BindCommand<TViewModel, T>(control, commandSelector, commandParameterSelector);
		}
		public void BindCommand(ISupportCommandBinding control, Expression<Action<TViewModel>> commandSelector, Expression<Func<TViewModel, object>> commandParameterSelector = null) {
			context.BindCommand<TViewModel>(control, commandSelector, commandParameterSelector);
		}
		public void BindCancelCommand<T>(ISupportCommandBinding control, Expression<Action<TViewModel, T>> asyncCommandSelector) {
			context.BindCancelCommand<TViewModel, T>(control, asyncCommandSelector);
		}
		public void BindCancelCommand(ISupportCommandBinding control, Expression<Action<TViewModel>> asyncCommandSelector) {
			context.BindCancelCommand<TViewModel>(control, asyncCommandSelector);
		}
		public IDisposable EventToCommand<TEventArgs>(object source, string eventName, Expression<Action<TViewModel>> commandSelector)
			where TEventArgs : EventArgs {
			return context.AttachBehavior<EventToCommandBehavior<TViewModel, TEventArgs>>(source, null, eventName, commandSelector);
		}
		public IDisposable EventToCommand<TEventArgs>(object source, string eventName, Expression<Action<TViewModel>> commandSelector, Func<TEventArgs, object> eventArgsToCommandParameterConverter)
			where TEventArgs : EventArgs {
			return context.AttachBehavior<EventToCommandBehavior<TViewModel, TEventArgs>>(source, null, eventName, commandSelector, eventArgsToCommandParameterConverter);
		}
		public IDisposable EventToCommand<TEventArgs>(object source, string eventName, Expression<Action<TViewModel>> commandSelector, Predicate<TEventArgs> eventFilter)
			where TEventArgs : EventArgs {
			return context.AttachBehavior<EventToCommandBehavior<TViewModel, TEventArgs>>(source, null, eventName, commandSelector, eventFilter);
		}
		public IDisposable EventToCommand<TEventArgs, T>(object source, string eventName, Expression<Action<TViewModel>> commandSelector, Expression<Func<TViewModel, T>> commandParameterSelector)
			where TEventArgs : EventArgs {
			return context.AttachBehavior<EventToCommandBehavior<TViewModel, T, TEventArgs>>(source, null, eventName, commandSelector, commandParameterSelector);
		}
		public IDisposable EventToCommand<TEventArgs, T>(object source, string eventName, Expression<Action<TViewModel>> commandSelector, Expression<Func<TViewModel, T>> commandParameterSelector, Predicate<TEventArgs> eventFilter)
			where TEventArgs : EventArgs {
			return context.AttachBehavior<EventToCommandBehavior<TViewModel, T, TEventArgs>>(source, null, eventName, commandSelector, commandParameterSelector, eventFilter);
		}
		public MVVMContextFluentAPI<TViewModel, EventArgs> WithEvent(object source, string eventName) {
			return MVVMContextFluentAPI<TViewModel, EventArgs>.WithEvent(context, source, eventName);
		}
		public MVVMContextFluentAPI<TViewModel, TEventArgs> WithEvent<TEventArgs>(object source, string eventName)
			where TEventArgs : EventArgs {
			return MVVMContextFluentAPI<TViewModel, TEventArgs>.WithEvent(context, source, eventName);
		}
		public MVVMContextFluentAPI<TViewModel, TSource, TSourceEventArgs> WithEvent<TSource, TSourceEventArgs>(TSource source, string eventName)
			where TSourceEventArgs : EventArgs {
			return MVVMContextFluentAPI<TViewModel, TSource, TSourceEventArgs>.WithEvent(context, source, eventName);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class MVVMContextFluentAPI<TViewModel, TEventArgs>
		where TViewModel : class
		where TEventArgs : EventArgs {
		MVVMContext context;
		string eventName;
		object source;
		MVVMContextFluentAPI(MVVMContext context, object source, string eventName) {
			this.context = context;
			this.eventName = eventName;
			this.source = source;
		}
		internal static MVVMContextFluentAPI<TViewModel, TEventArgs> WithEvent(MVVMContext context, object source, string eventName) {
			return new MVVMContextFluentAPI<TViewModel, TEventArgs>(context, source, eventName);
		}
		public IPropertyBinding SetBinding<TValue>(Expression<Func<TViewModel, TValue>> selectorExpression, Func<TEventArgs, TValue> sourceEventArgsConverter) {
			return context.SetBinding<TEventArgs, TViewModel, TValue>(selectorExpression, source, eventName, sourceEventArgsConverter);
		}
		public IDisposable EventToCommand(Expression<Action<TViewModel>> commandSelector) {
			return context.AttachBehavior<EventToCommandBehavior<TViewModel, TEventArgs>>(source, null, eventName, commandSelector);
		}
		public IDisposable EventToCommand(Expression<Action<TViewModel>> commandSelector, Func<TEventArgs, object> eventArgsToCommandParameterConverter) {
			return context.AttachBehavior<EventToCommandBehavior<TViewModel, TEventArgs>>(source, null, eventName, commandSelector, eventArgsToCommandParameterConverter);
		}
		public IDisposable EventToCommand(Expression<Action<TViewModel>> commandSelector, Predicate<TEventArgs> eventFilter) {
			return context.AttachBehavior<EventToCommandBehavior<TViewModel, TEventArgs>>(source, null, eventName, commandSelector, eventFilter);
		}
		public IDisposable EventToCommand<T>(Expression<Action<TViewModel>> commandSelector, Expression<Func<TViewModel, T>> commandParameterSelector) {
			return context.AttachBehavior<EventToCommandBehavior<TViewModel, T, TEventArgs>>(source, null, eventName, commandSelector, commandParameterSelector);
		}
		public IDisposable EventToCommand<T>(Expression<Action<TViewModel>> commandSelector, Expression<Func<TViewModel, T>> commandParameterSelector, Predicate<TEventArgs> eventFilter) {
			return context.AttachBehavior<EventToCommandBehavior<TViewModel, T, TEventArgs>>(source, null, eventName, commandSelector, commandParameterSelector, eventFilter);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class MVVMContextFluentAPI<TViewModel, TSource, TSourceEventArgs>
		where TViewModel : class
		where TSourceEventArgs : EventArgs {
		MVVMContext context;
		string eventName;
		TSource source;
		MVVMContextFluentAPI(MVVMContext context, TSource source, string eventName) {
			this.context = context;
			this.eventName = eventName;
			this.source = source;
		}
		internal static MVVMContextFluentAPI<TViewModel, TSource, TSourceEventArgs> WithEvent(MVVMContext context, TSource source, string eventName) {
			return new MVVMContextFluentAPI<TViewModel, TSource, TSourceEventArgs>(context, source, eventName);
		}
		public IPropertyBinding SetBinding<TValue>(Expression<Func<TViewModel, TValue>> selectorExpression, Func<TSourceEventArgs, TValue> sourceEventArgsConverter, Action<TSource, TValue> updateSourceAction) {
			return context.SetBinding<TSourceEventArgs, TSource, TViewModel, TValue>(selectorExpression, source, eventName, sourceEventArgsConverter, updateSourceAction);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class MVVMContextConfirmationFluentAPI<TEventArgs>
		where TEventArgs : CancelEventArgs {
		MVVMContext context;
		string eventName;
		object source;
		MVVMContextConfirmationFluentAPI(MVVMContext context, object source, string eventName) {
			this.context = context;
			this.eventName = eventName;
			this.source = source;
		}
		internal static MVVMContextConfirmationFluentAPI<TEventArgs> WithEvent(MVVMContext context, object source, string eventName) {
			return new MVVMContextConfirmationFluentAPI<TEventArgs>(context, source, eventName);
		}
		public IDisposable Confirmation(Action<ConfirmationBehavior<TEventArgs>> behaviorSettings = null) {
			return context.AttachBehavior<ConfirmationBehavior<TEventArgs>>(source, behaviorSettings, eventName);
		}
	}
	#endregion Fluent API
}
