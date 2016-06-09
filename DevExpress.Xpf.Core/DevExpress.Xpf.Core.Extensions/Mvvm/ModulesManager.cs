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
using System.Reflection;
using System.Threading;
using System.ComponentModel;
using DevExpress.Xpf.Core.MvvmSample.Helpers;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core.MvvmSample {
	public class ModulesManagerInternalData { }
	public interface IModule : IDisposable, ISupportInitialize {
		object View { get; }
		void SetView(object v);
		bool IsPersistentModule { get; }
		List<IModule> GetSubmodules();
		bool IsVisible { get; }
		void SetIsVisible(bool v);
		event EventHandler BeforeDisappear;
		void RaiseBeforeDisappear();
		event EventHandler BeforeAppearAsync;
		void RaiseBeforeAppearAsync();
		event EventHandler BeforeAppear;
		void RaiseBeforeAppear();
		object InitParam { get; set; }
		ModulesManagerInternalData ModulesManagerInternalData { get; set; }
		IModulesManager ModulesManager { get; }
		void SetModulesManager(IModulesManager v);
		IModule Parent { get; }
		IModule Main { get; }
		void SetParent(IModule v);
	}
	public interface IModulesListProvider {
		Type GetNavigationParentModuleType(Type moduleType);
	}
	public interface IViewsManager {
		void CreateView(IModule module);
		void ShowView(IModule module);
		IModule GetModule(object view);
	}
	public class ModulesManager : IModulesManager {
		class InternalData : ModulesManagerInternalData {
			public static InternalData Get(IModule module) {
				return (InternalData)module.ModulesManagerInternalData;
			}
			public InternalData() {
				Loaded = new AutoResetEvent(false);
			}
			public AutoResetEvent Loaded;
		}
		class ModuleCreator {
			IModule module;
			object parameter;
			IViewsManager viewsManager;
			List<IModule> submodules;
			SplashScreenWrapper splashScreenWrapper;
			public ModuleCreator(SplashScreenWrapper splashScreenWrapper, IModule module, object parameter, IViewsManager viewsManager) {
				this.splashScreenWrapper = splashScreenWrapper;
				this.module = module;
				this.parameter = parameter;
				this.viewsManager = viewsManager;
			}
			public void Load() {
				this.module.InitParam = this.parameter;
				this.module.RaiseBeforeAppearAsync();
			}
			public void InitModule() {
				this.module.RaiseBeforeAppear();
				this.submodules = module.GetSubmodules();
				BackgroundHelper.DoInBackground(WaitSubmodules, ShowModule);
			}
			void WaitSubmodules() {
				foreach(IModule submodule in this.submodules) {
					if(submodule == null) continue;
					InternalData.Get(submodule).Loaded.WaitOne();
				}
				this.submodules = null;
			}
			void ShowModule() {
				this.splashScreenWrapper.HideSplashScreen();
				viewsManager.ShowView(module);
				InternalData.Get(module).Loaded.Set();
			}
		}
		IModulesListProvider modulesListProvider;
		Dictionary<Type, IModule> persistentModules = new Dictionary<Type, IModule>();
		public ModulesManager(IViewsManager viewsManager, IModulesListProvider modulesListProvider) {
			this.modulesListProvider = modulesListProvider;
			SplashScreenWrapper = new SplashScreenWrapper();
			ViewsManager = viewsManager;
		}
		public SplashScreenWrapper SplashScreenWrapper { get; private set; }
		public ISplashScreenProvider SplashScreenProvider {
			get { return SplashScreenWrapper.SplashScreenProvider; }
			set { SplashScreenWrapper.SplashScreenProvider = value; }
		}
		public IViewsManager ViewsManager { get; private set; }
		public IModule CreateModule(Type moduleType, IModule module, IModule parent, object parameter = null) {
			MethodInfo createModuleCoreMethod = GetType().GetMethod("CreateModuleCore", BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo createModuleMethod = createModuleCoreMethod.MakeGenericMethod(moduleType);
			return (Module)createModuleMethod.Invoke(this, new object[] { module, parent, parameter });
		}
		public TModule CreateModule<TModule>(TModule module, IModule parent, object parameter = null) where TModule : class, IModule, new() {
			return CreateModuleCore<TModule>(module, parent, parameter);
		}
		public Type GetNavigationParentModuleType(Type moduleType) {
			return modulesListProvider.GetNavigationParentModuleType(moduleType);
		}
		public bool IsNavigationParent(Type moduleType, Type navigationParentModuleType) {
			for(Type t = moduleType; t != null; t = modulesListProvider.GetNavigationParentModuleType(t)) {
				if(t == navigationParentModuleType) return true;
			}
			return false;
		}
		protected internal TModule CreateModuleCore<TModule>(TModule module, IModule parent, object parameter) where TModule : class, IModule, new() {
			SplashScreenWrapper.ShowSplashScreen();
			if(module == null)
				module = GetPersistentModule<TModule>();
			if(module == null) {
				module = new TModule();
				module.BeginInit();
				module.ModulesManagerInternalData = new InternalData();
				module.SetModulesManager(this);
				module.SetParent(parent);
				ViewsManager.CreateView(module);
				module.EndInit();
				if(module.IsPersistentModule)
					SavePersistentModule(module);
			} else {
				ViewsManager.CreateView(module);
			}
			ModuleCreator creator = new ModuleCreator(SplashScreenWrapper, module, parameter, ViewsManager);
			BackgroundHelper.DoInBackground(creator.Load, creator.InitModule);
			return module;
		}
		protected TModule GetPersistentModule<TModule>() where TModule : class, IModule, new() {
			IModule persistentModule;
			return persistentModules.TryGetValue(typeof(TModule), out persistentModule) ? (TModule)persistentModule : null;
		}
		protected void SavePersistentModule<TModule>(TModule module) where TModule : class, IModule, new() {
			if(persistentModules.ContainsKey(typeof(TModule)))
				persistentModules[typeof(TModule)] = module;
			else
				persistentModules.Add(typeof(TModule), module);
		}
		void IModulesManager.RemovePersistentModule(Type moduleType) {
			if(persistentModules.ContainsKey(moduleType))
				persistentModules.Remove(moduleType);
		}
	}
	public interface ISplashScreenProvider {
		bool ShowSplashScreen { get; set; }
	}
	public class SplashScreenWrapper {
		ISplashScreenProvider splashScreenProvider;
		int waitsCount = 0;
		public void DoInBackground(Action backgroundAction, Action mainThreadAction) {
			ShowSplashScreen();
			BackgroundHelper.DoInBackground(backgroundAction, () => {
				if(mainThreadAction != null)
					mainThreadAction();
				HideSplashScreen();
			});
		}
		public ISplashScreenProvider SplashScreenProvider {
			get { return splashScreenProvider; }
			set {
				if(splashScreenProvider == value) return;
				SetShowSplashScreenCore(false);
				splashScreenProvider = value;
				SetShowSplashScreenCore(waitsCount != 0);
			}
		}
		public void ShowSplashScreen() {
			if(++waitsCount == 1)
				SetShowSplashScreenCore(true);
		}
		public void HideSplashScreen() {
			if(--waitsCount <= 0) {
				waitsCount = 0;
				SetShowSplashScreenCore(false);
			}
		}
		void SetShowSplashScreenCore(bool v) {
			if(splashScreenProvider != null)
				splashScreenProvider.ShowSplashScreen = v;
		}
	}
}
