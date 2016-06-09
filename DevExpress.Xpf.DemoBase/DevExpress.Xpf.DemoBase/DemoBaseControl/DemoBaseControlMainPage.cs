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
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.DemoBase.Helpers;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.DemoData.Model;
using System.IO;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.DemoBase.Internal {
	sealed class DemoBaseControlMainPage : DemoBaseControlPart {
		WeakEventHandler<ThePropertyChangedEventArgs<bool>> onModuleWrapperLoadingInProgressChanged;
		WeakEventHandler<ThePropertyChangedEventArgs<LString>> onModuleWrapperTitleChanged;
		WeakEventHandler<ThePropertyChangedEventArgs<bool>> onModuleWrapperAllowRtlChanged;
		WeakEventHandler<ModuleAppearEventArgs> onModuleWrapperModuleAppear;
		WeakEventHandler<ThePropertyChangedEventArgs<FlowDirection>> onModuleWrapperDemoFlowDirectionChanged;
		bool isModuleNavigationEnabled = true;
		public DemoBaseControlMainPage(DemoBaseControl demoBaseControl)
			: base(demoBaseControl) {
			onModuleWrapperTitleChanged = new WeakEventHandler<ThePropertyChangedEventArgs<LString>>(OnModuleWrapperTitleChanged);
			onModuleWrapperAllowRtlChanged = new WeakEventHandler<ThePropertyChangedEventArgs<bool>>(OnModuleWrapperAllowRtlChanged);
			onModuleWrapperModuleAppear = new WeakEventHandler<ModuleAppearEventArgs>(OnModuleWrapperModuleAppear);
			onModuleWrapperDemoFlowDirectionChanged = new WeakEventHandler<ThePropertyChangedEventArgs<FlowDirection>>(OnModuleWrapperDemoFlowDirectionChanged);
			onModuleWrapperLoadingInProgressChanged = new WeakEventHandler<ThePropertyChangedEventArgs<bool>>(OnModuleWrapperLoadingInProgressChanged);
			ModuleWrapper = CreateWrapper();
			RaiseDemoFlowDirectionChanged();
			Initialized();
		}
		public bool IsModuleNavigationEnabled {
			get { return isModuleNavigationEnabled; }
			set { SetProperty(ref isModuleNavigationEnabled, value, () => IsModuleNavigationEnabled); }
		}
		public event EventHandler<ModuleAppearEventArgs> ModuleAppear;
		public override void OnNavigatedTo(CompletePageState prevState, CompletePageState newState) {
			IsModuleNavigationEnabled = (prevState == null && (newState.Product != null && newState.Product.Modules.Count > 1)) 
				|| (newState.Product == null || newState.Product.Modules == null || newState.Product.Modules.Count > 1);
			ThemeData.EnsureAllowedTheme(newState.Module);
			ModuleWrapper.Module = newState.Module;
			ModuleWrapper.OnNavigatedTo(prevState, newState);
		}
		public override void OnNavigatingFrom(CompletePageState initialState) {
			ModuleWrapper.OnNavigatingFrom(initialState);
		}
		public LString ModuleTitle {
			get { return GetProperty(() => ModuleTitle); }
			set { SetProperty(() => ModuleTitle, value, RaiseModuleTitleChanged); }
		}
		public bool AllowRtl {
			get { return GetProperty(() => AllowRtl); }
			set { SetProperty(() => AllowRtl, value, RaiseAllowRtlChanged); }
		}
		public bool IsSolutionGroupVisible {
			get { return !EnvironmentHelper.IsClickOnce; }
		}
		public FlowDirection DemoFlowDirection {
			get { return GetProperty(() => DemoFlowDirection); }
			set { SetProperty(() => DemoFlowDirection, value, RaiseDemoFlowDirectionChanged); }
		}
		public bool LoadingInProgress {
			get { return GetProperty(() => LoadingInProgress); }
			set { SetProperty(() => LoadingInProgress, value, RaiseLoadingInProgressChanged); }
		}
		public DemoBaseControlModuleWrapper ModuleWrapper {
			get { return GetProperty(() => ModuleWrapper); }
			set { SetProperty(() => ModuleWrapper, value); }
		}
		public event ThePropertyChangedEventHandler<LString> ModuleTitleChanged;
		public event ThePropertyChangedEventHandler<FlowDirection> DemoFlowDirectionChanged;
		public event ThePropertyChangedEventHandler<bool> LoadingInProgressChanged;
		public event ThePropertyChangedEventHandler<bool> AllowRtlChanged;
		public ICommand NextDemoCommand { get { return new DelegateCommand(NextDemo); } }
		public ICommand PreviousDemoCommand { get { return new DelegateCommand(PrevDemo); } }
		public ICommand OpenCSSolutionCommand { get { return new DelegateCommand(OpenCSSolution); } }
		public ICommand OpenVBSolutionCommand { get { return new DelegateCommand(OpenVBSolution); } }
		void NextDemo() {
			ModuleWrapper.IsNextDemoRequested = true;
			ModuleWrapper.Module = ModuleWrapper.Module.Product.AdvanceModule(ModuleWrapper.Module, 1);
		}
		void PrevDemo() {
			ModuleWrapper.IsNextDemoRequested = false;
			ModuleWrapper.Module = ModuleWrapper.Module.Product.AdvanceModule(ModuleWrapper.Module, -1);
		}
		void OpenCSSolution() {
			ModuleWrapper.Module.Product.OpenCSSolution(GetSolutionFiles(ModuleWrapper.Module.Product.CSSolutionPath));
		}
		void OpenVBSolution() {
			ModuleWrapper.Module.Product.OpenVBSolution(GetSolutionFiles(ModuleWrapper.Module.Product.VBSolutionPath));
		}
		string[] GetSolutionFiles(string relativePath) {
			var rawFileNames = ((DemoModule)ModuleWrapper.CurrentDemoModule).GetCodeFileNames();
			return rawFileNames
				.SelectMany(CodeFileString.GetCodeFileStrings)
				.Select(str => Path.Combine(ModuleWrapper.Module.Product.CSSolutionPath, str.FilePath.Replace("/", "\\")))
				.ToArray();
		}
		DemoBaseControlModuleWrapper CreateWrapper() {
			DemoBaseControlModuleWrapper wrapper = new DemoBaseControlModuleWrapper(this, DemoBaseControl);
			wrapper.TitleChanged += onModuleWrapperTitleChanged.Handler;
			OnModuleWrapperTitleChanged(wrapper, new ThePropertyChangedEventArgs<LString>(null, wrapper.Title));
			wrapper.AllowRtlChanged += onModuleWrapperAllowRtlChanged.Handler;
			OnModuleWrapperAllowRtlChanged(wrapper, new ThePropertyChangedEventArgs<bool>(true, wrapper.AllowRtl));
			wrapper.ModuleAppear += onModuleWrapperModuleAppear.Handler;
			wrapper.DemoFlowDirectionChanged += onModuleWrapperDemoFlowDirectionChanged.Handler;
			wrapper.LoadingInProgressChanged += onModuleWrapperLoadingInProgressChanged.Handler;
			OnModuleWrapperLoadingInProgressChanged(wrapper, new ThePropertyChangedEventArgs<bool>(false, wrapper.LoadingInProgress));
			return wrapper;
		}
		void OnModuleWrapperAllowRtlChanged(object sender, ThePropertyChangedEventArgs<bool> e) {
			AllowRtl = e.NewValue;
		}
		void OnModuleWrapperModuleAppear(object sender, ModuleAppearEventArgs e) {
			if(ModuleAppear != null)
				ModuleAppear(this, e);
		}
		void OnModuleWrapperTitleChanged(object sender, ThePropertyChangedEventArgs<LString> e) {
			ModuleTitle = e.NewValue;
		}
		void RaiseLoadingInProgressChanged() {
			if(LoadingInProgressChanged != null) {
				LoadingInProgressChanged(this, new ThePropertyChangedEventArgs<bool>(false, LoadingInProgress));
			}
		}
		void OnModuleWrapperLoadingInProgressChanged(object sender, ThePropertyChangedEventArgs<bool> e) {
			LoadingInProgress = e.NewValue;
		}
		void OnModuleWrapperDemoFlowDirectionChanged(object sender, ThePropertyChangedEventArgs<FlowDirection> e) {
			DemoFlowDirection = e.NewValue;
		}
		void RaiseModuleTitleChanged() {
			if(ModuleTitleChanged != null)
				ModuleTitleChanged(this, new ThePropertyChangedEventArgs<LString>(null, ModuleTitle));
		}
		void RaiseAllowRtlChanged() {
			if(AllowRtlChanged != null) {
				AllowRtlChanged(this, new ThePropertyChangedEventArgs<bool>(false, AllowRtl));
			}
		}
		void RaiseDemoFlowDirectionChanged() {
			ModuleWrapper.DemoFlowDirection = DemoFlowDirection;
			if(DemoFlowDirectionChanged != null)
				DemoFlowDirectionChanged(this, new ThePropertyChangedEventArgs<FlowDirection>(FlowDirection.LeftToRight, DemoFlowDirection));
		}
	}
}
