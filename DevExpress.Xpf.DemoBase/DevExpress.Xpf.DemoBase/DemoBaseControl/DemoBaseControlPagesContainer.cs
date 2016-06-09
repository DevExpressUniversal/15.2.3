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
using System.Linq;
using System.Text;
using DevExpress.DemoData.Helpers;
using System.Windows.Input;
using System.Windows.Data;
using DevExpress.DemoData.Utils;
using System.Diagnostics;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.DemoData;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.DemoBase.DemoTesting;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI;
using DevExpress.DemoData.Model;
namespace DevExpress.Xpf.DemoBase.Internal {
	class DemoBaseControlPagesContainer : DemoBaseControlPart {
		WeakEventHandler<ModuleAppearEventArgs> onMainPageModuleAppear;
		WeakEventHandler<ThePropertyChangedEventArgs<bool>> onMainPageLoadingInProgressChanged;
		WeakEventHandler<DepPropertyChangedEventArgs> onDemoBaseControlAllowRunAnotherDemoChanged;
		DemoBaseControlProductsPage productsPage;
		DemoBaseControlModulesPage modulesPage;
		DemoBaseControlMainPage mainPage;
		StartupBase startup;
		string loadedProductName;
		Assembly demoAssembly;
		Action onMainPageLoadingCompleted = () => { };
		public DemoBaseControlPagesContainer(DemoBaseControl demoBaseControl, StartupBase startup)
			: base(demoBaseControl)
		{
			this.startup = startup;
			loadedProductName = DemoBaseControl.InitiallyLoadedProductName;
			demoAssembly = DemoBaseControl.InitiallyLoadedDemoAssembly;
			onDemoBaseControlAllowRunAnotherDemoChanged = new WeakEventHandler<DepPropertyChangedEventArgs>(OnDemoBaseControlAllowRunAnotherDemoChanged);
			DemoBaseControl.AllowRunAnotherDemoChanged += onDemoBaseControlAllowRunAnotherDemoChanged.Handler;
			OnDemoBaseControlAllowRunAnotherDemoChanged(DemoBaseControl, new DepPropertyChangedEventArgs(DemoBaseControl.AllowRunAnotherDemoProperty, false, DemoBaseControl.AllowRunAnotherDemo));
			ShowBuyNowButton = IsBuyNowButtonVisible();
			Initialized();
		}
		[Command]
		public void GetStarted() {
			DocumentPresenter.OpenTabLink(DemoDataSettings.GetStartedLink, OpenLinkType.Smart);
		}
		[Command]
		public void GetSupport() {
			DocumentPresenter.OpenTabLink(DemoDataSettings.GetSupportLink, OpenLinkType.Smart);
		}
		bool IsBuyNowButtonVisible() {
			if(CompleteState == null)
				return false;
			if(CompleteState.Page == DemoBaseControlPage.Products) {
				return !DataLoader.DefaultPlatform.IsLicensed;
			}
			return CompleteState.Product == null || !CompleteState.Product.IsLicensed;
		}
		public DemoBaseControlPart CurrentPage {
			get { return GetProperty(() => CurrentPage); }
			set { SetProperty(() => CurrentPage, value); }
		}
		public LString Copyright {
			get { return GetProperty(() => Copyright); }
			set { SetProperty(() => Copyright, value); }
		}
		public string AnimationDirection {
			get { return GetProperty(() => AnimationDirection); }
			set { SetProperty(() => AnimationDirection, value); }
		}
		public bool AllowRunAnotherDemo {
			get { return GetProperty(() => AllowRunAnotherDemo); }
			set { SetProperty(() => AllowRunAnotherDemo, value); }
		}
		public bool ShowBuyNowButton {
			get { return GetProperty(() => ShowBuyNowButton); }
			set { SetProperty(() => ShowBuyNowButton, value); }
		}
		public string HeaderCategory {
			get { return GetProperty(() => HeaderCategory); }
			set { SetProperty(() => HeaderCategory, value); }
		}
		public string HeaderSubcategory {
			get { return GetProperty(() => HeaderSubcategory); }
			set { SetProperty(() => HeaderSubcategory, value); }
		}
		public event EventHandler<ModuleAppearEventArgs> ModuleAppear;
		public event EventHandler BuyButtonClick;
		public event ThePropertyChangedEventHandler<DemoBaseControlPage> ActualPageChanged;
		public event ThePropertyChangedEventHandler<ProductDescription> ActualProductChanged;
		public void NavigateToState(CompletePageState state) {
			switch(state.Page) {
				case DemoBaseControlPage.Main:
					NavigateToPage(MainPage, state, true);
					break;
				case DemoBaseControlPage.Modules:
					NavigateToPage(ModulesPage, state, true);
					break;
				case DemoBaseControlPage.Products:
					NavigateToPage(ProductsPage, state, true);
					break;
				default:
					Debug.Assert(false);
					break;
			}
		}
		public ICommand BuyNowCommand { get { return new DelegateCommand(BuyNow); } }
		public void BuyNow() {
			if(BuyButtonClick != null)
				BuyButtonClick(this, EventArgs.Empty);
		}
		public DemoBaseControlProductsPage ProductsPage {
			get {
				if(productsPage == null)
					productsPage = new DemoBaseControlProductsPage(DemoBaseControl);
				return productsPage;
			}
		}
		public DemoBaseControlModulesPage ModulesPage {
			get {
				if(modulesPage == null) {
					modulesPage = new DemoBaseControlModulesPage(DemoBaseControl);
					modulesPage.ActualProductChanged += new ThePropertyChangedEventHandler<ProductDescription>(OnActualProductChanged);
				}
				return modulesPage;
			}
		}
		void OnActualProductChanged(object sender, ThePropertyChangedEventArgs<ProductDescription> e) {
			if(ActualProductChanged != null)
				ActualProductChanged(sender, e);
		}
		public DemoBaseControlMainPage MainPage {
			get {
				if(mainPage == null) {
					mainPage = new DemoBaseControlMainPage(DemoBaseControl);
					onMainPageModuleAppear = new WeakEventHandler<ModuleAppearEventArgs>(OnMainPageModuleAppear);
					onMainPageLoadingInProgressChanged = new WeakEventHandler<ThePropertyChangedEventArgs<bool>>(OnMainPageLoadingInProgressChanged);
					mainPage.ModuleAppear += onMainPageModuleAppear.Handler;
					mainPage.LoadingInProgressChanged += onMainPageLoadingInProgressChanged.Handler;
				}
				return mainPage;
			}
		}
		public bool ActualShowBackButton {
			get { return GetProperty(() => ActualShowBackButton); }
			set { SetProperty(() => ActualShowBackButton, value); }
		}
		public bool LoadingInProgress {
			get { return GetProperty(() => LoadingInProgress); }
			set { SetProperty(() => LoadingInProgress, value); }
		}
		public bool FullScreenLoadingInProgress {
			get { return GetProperty(() => FullScreenLoadingInProgress); }
			set { SetProperty(() => FullScreenLoadingInProgress, value); }
		}
		public ICommand BackCommand { get { return new DelegateCommand(Back); } }
		public void Back() {
			if(stateHistory.Any()) {
				NavigateToState(stateHistory.Pop());
			}
		}
		void OnDemoBaseControlAllowRunAnotherDemoChanged(object sender, DepPropertyChangedEventArgs e) {
			AllowRunAnotherDemo = (bool)e.NewValue;
		}
		bool IsInitialNavigation() {
			return previousState == null;
		}
		void OnMainPageLoadingInProgressChanged(object sender, ThePropertyChangedEventArgs<bool> e) {
			LoadingInProgress = e.NewValue && CurrentPage != MainPage;
			FullScreenLoadingInProgress = CompleteState != null && LoadingInProgress && IsInitialNavigation() && CompleteState.Page != DemoBaseControlPage.Main;
			if(!LoadingInProgress) {
				onMainPageLoadingCompleted();
				onMainPageLoadingCompleted = () => { };
			}
		}
		void OnMainPageModuleAppear(object sender, ModuleAppearEventArgs e) {
			if(ModuleAppear != null)
				ModuleAppear(this, e);
		}
		public ICommand RaiseActualPageChangedCommand { get { return new DelegateCommand<object>(RaiseActualPageChanged); } }
		public void RaiseActualPageChanged(object pageView) {
			DemoBaseControlPage actualPage = DemoBaseControlPage.None;
			if(pageView == null)
				actualPage = DemoBaseControlPage.None;
			else if(pageView == ProductsPage.View)
				actualPage = DemoBaseControlPage.Products;
			else if(pageView == ModulesPage.View)
				actualPage = DemoBaseControlPage.Modules;
			else if(pageView == MainPage.View)
				actualPage = DemoBaseControlPage.Main;
			if(ActualPageChanged != null)
				ActualPageChanged(this, new ThePropertyChangedEventArgs<DemoBaseControlPage>(DemoBaseControlPage.None, actualPage));
		}
		CompletePageState previousState;
		public CompletePageState CompleteState {
			get { return GetProperty(() => CompleteState); }
			private set { SetProperty(() => CompleteState, value); }
		}
		Stack<CompletePageState> stateHistory = new Stack<CompletePageState>();
		private void RememberCurrentState() {
			if (CompleteState != null)
				stateHistory.Push(CompleteState);
		}
		void UpdateBackButtonVisibility() {
			ActualShowBackButton = stateHistory.Count > 0;
		}
		void NavigateToPage(DemoBaseControlPart page, CompletePageState state, bool back, bool rememberHistory = false) {
			AnimationDirection = back ? "Back" : "Forward";
			if (CurrentPage != null) 
				CurrentPage.OnNavigatingFrom(CompleteState);
			if(CompleteState != null) {
				if(CompleteState.Page == DemoBaseControlPage.Main &&
					state.Page == DemoBaseControlPage.Main &&
					CompleteState.Product == state.Product)
				{
					rememberHistory = false;
				}
			}
			if(rememberHistory)
				RememberCurrentState();
			previousState = CompleteState;
			ShowBuyNowButton = IsBuyNowButtonVisible();
			Action tail = () => {
				CompleteState = state;
				CurrentPage = page;
				UpdateNavigationHeader();
			};
			bool shouldWaitAnimation = state.Page == DemoBaseControlPage.Main && mainPage.ModuleWrapper.Module != state.Module;
			if (shouldWaitAnimation)
				onMainPageLoadingCompleted = tail;
			page.OnNavigatedTo(previousState, state);
			if(!shouldWaitAnimation)
				tail();
		}
		void UpdateNavigationHeader() {
			UpdateBackButtonVisibility();
			switch(CompleteState.Page) {
				case DemoBaseControlPage.Modules:
					HeaderCategory = CompleteState.Product.DisplayName;
					HeaderSubcategory = "";
					break;
				case DemoBaseControlPage.Main:
					HeaderCategory = CompleteState.Product.DisplayName;
					HeaderSubcategory = CompleteState.Module.Title.Text;
					break;
			}
		}
		public void UpdateCurrentModule(ModuleDescription newModule) {
			if(CompleteState == null)
				return;
			CompleteState = CompleteState.WithModule(newModule);
			UpdateNavigationHeader();
		}
		public void NavigateToModulesPage(ProductDescription product, string filter) {
			NavigateToPage(ModulesPage, new CompletePageState(DemoBaseControlPage.Modules, null, product, filter), false, true);
		}
		public void NavigateToMainPage(ModuleDescription module) {
			LoadDemoAssemblyAsync(module).ContinueWith(t => {
				BackgroundHelper.DoInMainThread(() => {
					NavigateToPage(MainPage, new CompletePageState(DemoBaseControlPage.Main, module), false, true);
				});
			});
		}
		public void NavigateToProductsPage() {
			if(CompleteState == null && loadedProductName != null) { 
				var product = DemoBaseControl.Data.Products.First(p => p.Name == loadedProductName);
				if(!string.IsNullOrEmpty(DemoBaseControl.InitiallyLoadedModuleName)) {
					ModuleDescription module = product.Modules.FirstOrDefault(m => m.Name == DemoBaseControl.InitiallyLoadedModuleName);
					if (module == null) {
						module = product.Modules.FirstOrDefault(m => m.Title.Text == DemoBaseControl.InitiallyLoadedModuleName);
					}
					stateHistory.Push(new CompletePageState(DemoBaseControlPage.Products, null));
					stateHistory.Push(new CompletePageState(DemoBaseControlPage.Modules, null, product));
					NavigateToMainPage(module);
				} else if(product.Modules.Count == 1) {
					NavigateToMainPage(product.Modules.First());
				} else {
					NavigateToModulesPage(product, "");
				}
				return;
			}
			NavigateToPage(ProductsPage, new CompletePageState(DemoBaseControlPage.Products, null), false, true);
		}
		public Task<bool> LoadDemoAssemblyAsync(ModuleDescription module) {
			if(module.Product.Name == loadedProductName) {
				return Task.Factory.StartNew(() => false);
			}
			DemoHelper.ClearDemo(demoAssembly);
			return startup.DemoLauncherLoader.LoadDemoModuleAsync(module.DemoAssemblyName, module.Name).ContinueWith(t => {
				Assembly assembly = t.Result;
				ViewLocator.Default = new ViewLocator(assembly);
				foreach(ModuleDescription m in module.Product.Modules) {
#if DEBUG
					try {
#endif
						m.UpdateModuleType(assembly);
#if DEBUG
					} catch { } 
#endif
				}
				BackgroundHelper.DoInMainThread(() => DemoHelper.InitDemo(assembly));
				demoAssembly = assembly;
				loadedProductName = module.Product.Name;
				BackgroundHelper.DoInMainThread(startup.DemoLauncherLoader.ShowDemo);
				return false;
			});
		}
	}
}
