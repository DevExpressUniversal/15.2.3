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
using System.Threading;
using System.Windows.Input;
using DevExpress.Xpf.Core.MvvmSample.Helpers;
namespace DevExpress.Xpf.Core.MvvmSample {
	public interface IModulesManager {
		TModule CreateModule<TModule>(TModule module, IModule parent, object parameter = null) where TModule : class, IModule, new();
		IModule CreateModule(Type moduleType, IModule module, IModule parent, object parameter = null);
		IViewsManager ViewsManager { get; }
		SplashScreenWrapper SplashScreenWrapper { get; }
		Type GetNavigationParentModuleType(Type moduleType);
		bool IsNavigationParent(Type moduleType, Type navigationParentModuleType);
		void RemovePersistentModule(Type moduleType);
	}
	public abstract class MainModule : Module {
		IModule currentModule;
		Type currentModuleType;
		ICommand showModuleCommand;
		ICommand showNavigationParentCommand;
		public MainModule() {
			IsPersistentModule = true;
			ShowModuleCommand = CreateShowModuleCommand();
			ShowNavigationParentCommand = CreateShowNavigationParentCommand();
		}
		public override void EndInit() {
			base.EndInit();
			CurrentModuleType = GetType();
			CurrentModule = this;
		}
		public void ShowModule<TModule>(object parameter) where TModule : class, IModule, new() {
			CurrentModuleType = typeof(TModule);
			CurrentModule = ModulesManager.CreateModule<TModule>(null, this, parameter);
		}
		public void ShowModule(Type moduleType, object parameter) {
			if(moduleType == null) return;
			CurrentModuleType = moduleType;
			CurrentModule = ModulesManager.CreateModule(moduleType, null, this, parameter);
		}
		public void ShowNavigationParent(IModule module, object parameter) {
			if(module == null) return;
			Type navigationParentType = ModulesManager.GetNavigationParentModuleType(module.GetType());
			CurrentModuleType = navigationParentType;
			CurrentModule = ModulesManager.CreateModule(navigationParentType, null, this, parameter);
		}
		public Type CurrentModuleType {
			get { return currentModuleType; }
			set { SetValue<Type>("CurrentModuleType", ref currentModuleType, value); }
		}
		public IModule CurrentModule {
			get { return currentModule; }
			set { SetValue<IModule>("CurrentModule", ref currentModule, value); }
		}
		public ICommand ShowModuleCommand {
			get { return showModuleCommand; }
			private set { SetValue<ICommand>("ShowModuleCommand", ref showModuleCommand, value); }
		}
		public ICommand ShowNavigationParentCommand {
			get { return showNavigationParentCommand; }
			private set { SetValue<ICommand>("ShowNavigationParentCommand", ref showNavigationParentCommand, value); }
		}
		public Func<object, object, string> StoryboardSelector { get { return SelectStoryboard; } }
		protected virtual string SelectStoryboard(object oldView, object newView) {
			string s = SelectStoryboard(newView);
			if(s != null) return s;
			return SelectStoryboard(ModulesManager.ViewsManager.GetModule(oldView), ModulesManager.ViewsManager.GetModule(newView));
		}
		protected virtual string SelectStoryboard(object newView) { return null; }
		protected virtual string SelectStoryboard(IModule oldModule, IModule newModule) {
			Type newModuleType = newModule == null ? null : newModule.GetType();
			Type oldModuleType = oldModule == null ? null : oldModule.GetType();
			return ModulesManager.IsNavigationParent(oldModuleType, newModuleType) ? "FromLeft" : "FromRight";
		}
		protected virtual ICommand CreateShowModuleCommand<T>() where T : class, IModule, new() {
			return new ExtendedActionCommand(p => ShowModule<T>(p), this, "CurrentModuleType", x => CurrentModuleType != typeof(T), null);
		}
		protected virtual ICommand CreateShowModuleCommand() {
			return new ExtendedActionCommand(t => ShowModule(t as Type, null), this, "CurrentModuleType", t => CurrentModuleType != t as Type, null);
		}
		protected virtual ICommand CreateShowNavigationParentCommand() {
			return new ExtendedActionCommand(p => ShowNavigationParent(p as IModule, null), this, "CurrentModuleType", x => CurrentModuleType != GetType(), null);
		}
	}
	public class Module : BindableAndDisposable, IModule {
		object view;
		IModule parent;
		IModule main;
		bool isVisible;
		object initParam;
		ModulesManagerInternalData internalData;
		public bool IsPersistentModule { get; protected set; }
		public IModulesManager ModulesManager { get; private set; }
		public object View {
			get { return view; }
			private set { SetValue<object>("View", ref view, value); }
		}
		public IModule Parent {
			get { return parent; }
			private set { SetValue<IModule>("Parent", ref parent, value); }
		}
		public bool IsVisible {
			get { return isVisible; }
			private set { SetValue<bool>("IsVisible", ref isVisible, value); }
		}
		public IModule Main {
			get { return main; }
			private set { SetValue<IModule>("Main", ref main, value); }
		}
		public event EventHandler BeforeDisappear;
		public event EventHandler BeforeAppearAsync;
		public event EventHandler BeforeAppear;
		public virtual List<IModule> GetSubmodules() {
			return new List<IModule>();
		}
		public virtual void BeginInit() { }
		public virtual void EndInit() { }
		protected virtual void LoadData(object parameter) { }
		protected virtual void InitData(object parameter) { }
		protected virtual void SaveData() { }
		protected override void DisposeManaged() {
			Main = null;
			Parent = null;
			base.DisposeManaged();
		}
		void IModule.SetView(object v) {
			View = v;
		}
		void IModule.SetIsVisible(bool v) {
			IsVisible = v;
		}
		void IModule.RaiseBeforeDisappear() {
			SaveData();
			if(BeforeDisappear != null)
				BeforeDisappear(this, EventArgs.Empty);
		}
		object IModule.InitParam {
			get { return initParam; }
			set { initParam = value; }
		}
		void IModule.RaiseBeforeAppearAsync() {
			LoadData(initParam);
			if(BeforeAppearAsync != null)
				BeforeAppearAsync(this, EventArgs.Empty);
		}
		void IModule.RaiseBeforeAppear() {
			InitData(initParam);
			if(BeforeAppear != null)
				BeforeAppear(this, EventArgs.Empty);
		}
		void IModule.SetModulesManager(IModulesManager v) {
			ModulesManager = v;
		}
		ModulesManagerInternalData IModule.ModulesManagerInternalData {
			get { return internalData; }
			set { internalData = value; }
		}
		void IModule.SetParent(IModule v) {
			Parent = v;
			Main = v == null ? this : v.Main;
		}
	}
}
