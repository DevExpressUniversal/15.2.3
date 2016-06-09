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
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.DemoData.Helpers;
using System.Windows.Input;
using DevExpress.Xpf.DemoBase.Helpers;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer;
using DevExpress.DemoData.Utils;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using System.Diagnostics;
namespace DevExpress.Xpf.DemoBase.Internal {
	sealed class DemoBaseControlModuleWrapper : DemoBaseControlPart {
		WeakEventHandler<EventArgs> onDemoBaseControlModuleReload;
		WeakEventHandler<ThePropertyChangedEventArgs<FlowDirection>> onParentDemoFlowDirectionChanged;
		WeakEventHandler<ThePropertyChangedEventArgs<DemoBaseControlPage>> onDemoBaseControlStatePageChanged;
		WeakEventHandler<ThePropertyChangedEventArgs<ToolbarSidebarView>> onDemoBaseControlStateOptionsViewChanged;
		WeakEventHandler<ThePropertyChangedEventArgs<bool>> onDemoBaseControlStateOptionsIsExpandedChanged;
		WeakEventHandler<DepPropertyChangedEventArgs> onDemoBaseControlActualPageChanged;
		DemoModuleLoader demoModuleLoader;
		ToolbarView demoModuleView = ToolbarView.Demo;
		ModuleDescription module;
		bool moduleLoadingInProgress = false;
		bool moduleChanged = false;
		ModuleDescription loadModule;
		List<CodeTextDescription> csCodeTexts;
		WeakEventHandler<ThePropertyChangedEventArgs<bool>> onCodeTextLoadingInProgressChanged;
		bool loadingInProgressCore = false;
		ToolbarView nextView = ToolbarView.Demo;
		ICommand onDemoGoneCommand;
		ICommand onDemoCameCommand;
		public DemoBaseControlModuleWrapper(DemoBaseControlMainPage parent, DemoBaseControl demoBaseControl)
			: base(demoBaseControl) {
			onCodeTextLoadingInProgressChanged = new WeakEventHandler<ThePropertyChangedEventArgs<bool>>(OnCodeTextLoadingInProgressChanged);
			onDemoBaseControlModuleReload = new WeakEventHandler<EventArgs>(OnDemoBaseControlModuleReload);
			onDemoBaseControlStatePageChanged = new WeakEventHandler<ThePropertyChangedEventArgs<DemoBaseControlPage>>(OnDemoBaseControlStatePageChanged);
			onDemoBaseControlStateOptionsViewChanged = new WeakEventHandler<ThePropertyChangedEventArgs<ToolbarSidebarView>>(OnDemoBaseControlStateOptionsViewChanged);
			onDemoBaseControlStateOptionsIsExpandedChanged = new WeakEventHandler<ThePropertyChangedEventArgs<bool>>(OnDemoBaseControlStateOptionsIsExpandedChanged);
			onParentDemoFlowDirectionChanged = new WeakEventHandler<ThePropertyChangedEventArgs<FlowDirection>>(OnParentDemoFlowDirectionChanged);
			onDemoBaseControlActualPageChanged = new WeakEventHandler<DepPropertyChangedEventArgs>(OnDemoBaseControlActualPageChanged);
			parent.DemoFlowDirectionChanged += onParentDemoFlowDirectionChanged.Handler;
			OnParentDemoFlowDirectionChanged(parent, new ThePropertyChangedEventArgs<FlowDirection>(FlowDirection.LeftToRight, parent.DemoFlowDirection));
			ContentPresenter cp1 = (ContentPresenter)View.GetObjectFromVisualTree("DemoModule1Presenter");
			ContentPresenter cp2 = (ContentPresenter)View.GetObjectFromVisualTree("DemoModule2Presenter");
			Style dmcs = (Style)View.GetObjectFromResources("DemoModuleControl");
			demoModuleLoader = new DemoModuleLoader(cp1, cp2, dmcs);
			demoModuleLoader.CurrentDemoModuleHelper.Owner = this;
			demoModuleLoader.NextDemoModuleHelper.Owner = this;
			demoModuleLoader.NextDemoModuleLoaded += OnDemoModuleLoaderNextDemoModuleLoaded;
			DemoModuleView = ToolbarView.Demo;
			HasOptions = true;
			DemoBaseControl.ModuleReload += onDemoBaseControlModuleReload.Handler;
			DemoBaseControl.ActualPageChanged += onDemoBaseControlActualPageChanged.Handler;
			LoadingBackground = LoadingSplashBackground.White;
			OptionsView = ToolbarSidebarView.Options;
			Initialized();
		}
		public override void OnNavigatingFrom(CompletePageState initialState) {
			bool canLeave = CanLeave();
			Debug.Assert(canLeave);
			demoModuleLoader.IsPopupContentInvisible = true;
			Leave();
		}
		public override void OnNavigatedTo(CompletePageState prevState, CompletePageState newState) {
			base.OnNavigatedTo(prevState, newState);
			Show(newState.Module.ModuleType);
		}
		public List<CodeTextDescription> CodeTexts {
			get { return csCodeTexts; }
			private set {
				List<CodeTextDescription> oldValue = csCodeTexts;
				if(SetProperty(ref csCodeTexts, value, "CodeTexts"))
					RaiseCodeTextsChanged(oldValue, value);
			}
		}
		public bool IsDemoGone {
			get { return GetProperty(() => IsDemoGone); }
			set { SetProperty(() => IsDemoGone, value); }
		}
		public bool? IsNextDemoRequested {
			get { return GetProperty(() => IsNextDemoRequested); }
			set { SetProperty(() => IsNextDemoRequested, value); }
		}
		public bool? CurrentIsNextDemoRequested {
			get { return GetProperty(() => CurrentIsNextDemoRequested); }
			set { SetProperty(() => CurrentIsNextDemoRequested, value); }
		}
		public ToolbarSidebarView OptionsView {
			get { return GetProperty(() => OptionsView); }
			set { SetProperty(() => OptionsView, value, RaiseOptionsViewChanged); }
		}
		public ToolbarView DemoModuleView {
			get { return GetProperty(() => DemoModuleView); }
			set {
				ToolbarView oldValue = demoModuleView;
				if (SetProperty(() => DemoModuleView, value)) {
					OnDemoModuleViewChanged(oldValue, value);
				}
			}
		}
		public FrameworkElement CurrentDemoModule { get; set; }
		public bool HasOptions {
			get { return GetProperty(() => HasOptions); }
			set { SetProperty(() => HasOptions, value, RaiseHasOptionsChanged); }
		}
		public ToolbarThemesMode ThemesMode {
			get { return GetProperty(() => ThemesMode); }
			set { SetProperty(() => ThemesMode, value); }
		}
		public LString Title {
			get { return GetProperty(() => Title); }
			set { SetProperty(() => Title, value, RaiseTitleChanged); }
		}
		public bool AllowRtl {
			get { return GetProperty(() => AllowRtl); }
			set { SetProperty(() => AllowRtl, value, RaiseAllowRtlChanged); }
		}
		public bool IsLayoutGroupVisible {
			get { return GetProperty(() => IsLayoutGroupVisible); }
			set { SetProperty(() => IsLayoutGroupVisible, value); }
		}
		public bool IsSidebarVisible {
			get { return GetProperty(() => IsSidebarVisible); }
			set { SetProperty(() => IsSidebarVisible, value); }
		}
		public bool IsSidebarButtonEnabled {
			get { return GetProperty(() => IsSidebarButtonEnabled); }
			set { SetProperty(() => IsSidebarButtonEnabled, value); }
		}
		public string SidebarTag {
			get { return GetProperty(() => SidebarTag); }
			set { SetProperty(() => SidebarTag, value, RaiseSidebarTagChanged); }
		}
		public ImageSource SidebarIcon {
			get { return GetProperty(() => SidebarIcon); }
			set { SetProperty(() => SidebarIcon, value, RaiseSidebarIconChanged); }
		}
		public ImageSource SidebarIconSelected {
			get { return GetProperty(() => SidebarIconSelected); }
			set { SetProperty(() => SidebarIconSelected, value, RaiseSidebarIconSelectedChanged); }
		}
		public bool IsSidebar2Visible {
			get { return GetProperty(() => IsSidebar2Visible); }
			set { SetProperty(() => IsSidebar2Visible, value); }
		}
		public bool IsSidebar2ButtonEnabled {
			get { return GetProperty(() => IsSidebar2ButtonEnabled); }
			set { SetProperty(() => IsSidebar2ButtonEnabled, value); }
		}
		public string Sidebar2Tag {
			get { return GetProperty(() => Sidebar2Tag); }
			set { SetProperty(() => Sidebar2Tag, value, RaiseSidebar2TagChanged); }
		}
		public ImageSource Sidebar2Icon {
			get { return GetProperty(() => Sidebar2Icon); }
			set { SetProperty(() => Sidebar2Icon, value, RaiseSidebar2IconChanged); }
		}
		public ImageSource Sidebar2IconSelected {
			get { return GetProperty(() => Sidebar2IconSelected); }
			set { SetProperty(() => Sidebar2IconSelected, value, RaiseSidebar2IconSelectedChanged); }
		}
		public LString ShortDescription {
			get { return GetProperty(() => ShortDescription); }
			set { SetProperty(() => ShortDescription, value); }
		}
		public LString Description {
			get { return GetProperty(() => Description); }
			set { SetProperty(() => Description, value); }
		}
		public FlowDirection DemoFlowDirection {
			get { return GetProperty(() => DemoFlowDirection); }
			set { SetProperty(() => DemoFlowDirection, value, RaiseDemoFlowDirectionChanged); }
		}
		public ModuleDescription Module {
			get { return module; }
			set {
				if(SetProperty(ref module, value, "Module")) {
					RaiseModuleChanged();
				} else {
					(demoModuleLoader.CurrentDemoModuleHelper.DemoModule as DemoModule)
						.Do(m => m.BeginAppear());
				}
			}
		}
		public bool LoadingInProgress {
			get { return GetProperty(() => LoadingInProgress); }
			set { SetProperty(() => LoadingInProgress, value, RaiseLoadingInProgressChanged); }
		}
		public bool IsOptionsExpanded {
			get { return GetProperty(() => IsOptionsExpanded); }
			set { SetProperty(() => IsOptionsExpanded, value, RaiseIsOptionsExpandedChanged); }
		}
		public ReadOnlyCollection<ReadOnlyCollection<ModuleLinkDescription>> Links {
			get { return GetProperty(() => Links); }
			set { SetProperty(() => Links, value, RaiseLinksChanged); }
		}
		public bool ShowLinks {
			get { return GetProperty(() => ShowLinks); }
			set { SetProperty(() => ShowLinks, value); }
		}
		public LoadingSplashBackground LoadingBackground {
			get { return GetProperty(() => LoadingBackground); }
			set { SetProperty(() => LoadingBackground, value); }
		}
		public event EventHandler<ModuleAppearEventArgs> ModuleAppear;
		public event ThePropertyChangedEventHandler<LString> TitleChanged;
		public event ThePropertyChangedEventHandler<ToolbarView> DemoModuleViewChanged;
		public event ThePropertyChangedEventHandler<ToolbarSidebarView> OptionsViewChanged;
		public event ThePropertyChangedEventHandler<FlowDirection> DemoFlowDirectionChanged;
		public event ThePropertyChangedEventHandler<bool> IsOptionsExpandedChanged;
		public event ThePropertyChangedEventHandler<bool> HasOptionsChanged;
		public event ThePropertyChangedEventHandler<bool> LoadingInProgressChanged;
		public event ThePropertyChangedEventHandler<bool> AllowRtlChanged;
		public event ThePropertyChangedEventHandler<string> SidebarTagChanged;
		public event ThePropertyChangedEventHandler<ImageSource> SidebarIconChanged;
		public event ThePropertyChangedEventHandler<ImageSource> SidebarIconSelectedChanged;
		public event ThePropertyChangedEventHandler<string> Sidebar2TagChanged;
		public event ThePropertyChangedEventHandler<ImageSource> Sidebar2IconChanged;
		public event ThePropertyChangedEventHandler<ImageSource> Sidebar2IconSelectedChanged;
		public bool CanLeave() {
			return this.demoModuleLoader == null || this.demoModuleLoader.CurrentDemoModuleHelper.CanLeave();
		}
		public void Leave() {
			if(this.demoModuleLoader != null && this.demoModuleLoader.CurrentDemoModuleHelper != null)
				this.demoModuleLoader.CurrentDemoModuleHelper.Leave();
		}
		public void Show(Type demoModuleType) {
			if(this.demoModuleLoader != null && this.demoModuleLoader.CurrentDemoModuleHelper != null && nextView == ToolbarView.Demo && this.demoModuleLoader.CurrentDemoModuleHelper.DemoModule!= null && this.demoModuleLoader.CurrentDemoModuleHelper.DemoModule.GetType() == demoModuleType)				
				this.demoModuleLoader.CurrentDemoModuleHelper.Show();
		}
		void ShowCurrent() {
			if(this.demoModuleLoader != null && this.demoModuleLoader.CurrentDemoModuleHelper != null)
				this.demoModuleLoader.CurrentDemoModuleHelper.Show();
		}
		public ICommand OnDemoGoneCommand {
			get {
				if(onDemoGoneCommand == null)
					onDemoGoneCommand = new DelegateCommand(OnDemoGone);
				return onDemoGoneCommand;
			}
		}
		public ICommand OnDemoCameCommand {
			get {
				if(onDemoCameCommand == null)
					onDemoCameCommand = new DelegateCommand(OnDemoCame);
				return onDemoCameCommand;
			}
		}
		void RaiseCodeTextsChanged(List<CodeTextDescription> oldValue, List<CodeTextDescription> newValue) {
			if(oldValue != null) {
				foreach(CodeTextDescription ctd in oldValue)
					ctd.CodeText.LoadingInProgressChanged -= onCodeTextLoadingInProgressChanged.Handler;
			}
			if(newValue != null) {
				foreach(CodeTextDescription ctd in newValue)
					ctd.CodeText.LoadingInProgressChanged += onCodeTextLoadingInProgressChanged.Handler;
			}
		}
		void OnCodeTextLoadingInProgressChanged(object sender, ThePropertyChangedEventArgs<bool> e) {
			UpdateLoadingInProgress();
		}
		void UpdateLoadingInProgress() {
			LoadingInProgress = this.loadingInProgressCore;
			if(CodeTexts == null) return;
			foreach(CodeTextDescription ctd in CodeTexts) {
				LoadingInProgress = LoadingInProgress || ctd.CodeText.LoadingInProgress;
			}
		}
		internal void OnThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e) {
			if(e.ThemeName != "Demo" && demoModuleView == ToolbarView.Demo)
				LoadingBackground = ThemeSelectorHelper.ThemeToLoadingSplashBackground(e.ThemeName);
		}
		void OnDemoBaseControlStatePageChanged(object sender, ThePropertyChangedEventArgs<DemoBaseControlPage> e) {
			bool mustBeInvisible = e.NewValue != DemoBaseControlPage.Main;
			if(mustBeInvisible)
				this.demoModuleLoader.IsPopupContentInvisible = true;
		}
		void OnDemoBaseControlActualPageChanged(object sender, DepPropertyChangedEventArgs e) {
			bool mustBeInvisible = (DemoBaseControlPage)e.NewValue != DemoBaseControlPage.Main;
			if(!mustBeInvisible)
				this.demoModuleLoader.IsPopupContentInvisible = false;
		}
		void OnDemoBaseControlStateOptionsViewChanged(object sender, ThePropertyChangedEventArgs<ToolbarSidebarView> e) {
			OptionsView = e.NewValue;
		}
		void OnDemoBaseControlStateOptionsIsExpandedChanged(object sender, ThePropertyChangedEventArgs<bool> e) {
			IsOptionsExpanded = e.NewValue;
		}
		void BeginLoadNextModule() {
			if(this.moduleLoadingInProgress || !this.moduleChanged) return;
			ThemeData.EnsureAllowedTheme(Module);
			this.loadingInProgressCore = true;
			UpdateLoadingInProgress();
			this.moduleLoadingInProgress = true;
			this.moduleChanged = false;
			this.loadModule = Module;
			CurrentIsNextDemoRequested = IsNextDemoRequested;
			IsNextDemoRequested = null;
			this.demoModuleLoader.BeginLoadNextDemoModule(this.loadModule == null ? null : this.loadModule.ModuleType);
		}
		void OnDemoModuleLoaderNextDemoModuleLoaded(object sender, EventArgs e) {
			DemoBaseControl.PagesContainer.UpdateCurrentModule(Module);
			DoInBackgroundThread(null, () => {
				IsDemoGone = true;
				this.loadingInProgressCore = false;
				UpdateLoadingInProgress();
			});
		}
		public void OnDemoGone() {
			try { ReplaceCurrentDemoModuleByNext(); } catch { }
			DoInBackgroundThread(null, () => {
				IsDemoGone = false;
			});
		}
		void ReplaceCurrentDemoModuleByNext() {
			demoModuleLoader.CurrentDemoModuleHelper.ClearValue(DemoModuleHelper.DemoModuleViewProperty);
			demoModuleLoader.CurrentDemoModuleHelper.ClearValue(DemoModuleHelper.DemoModuleOptionsViewProperty);
			demoModuleLoader.CurrentDemoModuleHelper.ClearValue(DemoModuleHelper.OptionsExpandedProperty);
			UpdateLoadingInProgress();
			this.demoModuleLoader.ReplaceCurrentDemoModuleByNext();
			CurrentDemoModule = demoModuleLoader.CurrentDemoModuleHelper.DemoModule;
			IList<string> csCodeFiles = demoModuleLoader.CurrentDemoModuleHelper.GetCodeFileNames();
			CodeTexts = this.loadModule == null ? null : DemoHelper.GetCodeTexts(this.loadModule.ModuleType.Assembly, csCodeFiles);
			ShortDescription = this.loadModule == null ? LString.Empty : this.loadModule.ShortDescription;
			Description = this.loadModule == null ? LString.Empty : this.loadModule.Description;
			Title = this.loadModule == null ? LString.Empty : this.loadModule.Title;
			AllowRtl = this.loadModule == null ? true : this.loadModule.AllowRtl;
			IsLayoutGroupVisible = this.loadModule == null ? true : this.loadModule.Product.Modules.Any(m => m.AllowRtl);
			IsSidebarVisible = this.loadModule == null ? false : demoModuleLoader.CurrentDemoModuleHelper.IsSidebarVisible;
			IsSidebarButtonEnabled = this.loadModule == null ? true : demoModuleLoader.CurrentDemoModuleHelper.IsSidebarButtonEnabled;
			SidebarTag = this.loadModule == null ? "Sidebar" : demoModuleLoader.CurrentDemoModuleHelper.SidebarTag;
			SidebarIcon = this.loadModule == null ? null : demoModuleLoader.CurrentDemoModuleHelper.SidebarIcon;
			SidebarIconSelected = this.loadModule == null ? null : demoModuleLoader.CurrentDemoModuleHelper.SidebarIconSelected;
			IsSidebar2Visible = this.loadModule == null ? false : demoModuleLoader.CurrentDemoModuleHelper.IsSidebar2Visible;
			IsSidebar2ButtonEnabled = this.loadModule == null ? true : demoModuleLoader.CurrentDemoModuleHelper.IsSidebar2ButtonEnabled;
			Sidebar2Tag = this.loadModule == null ? "Sidebar2" : demoModuleLoader.CurrentDemoModuleHelper.Sidebar2Tag;
			Sidebar2Icon = this.loadModule == null ? null : demoModuleLoader.CurrentDemoModuleHelper.Sidebar2Icon;
			Sidebar2IconSelected = this.loadModule == null ? null : demoModuleLoader.CurrentDemoModuleHelper.Sidebar2IconSelected;
			HasOptions = demoModuleLoader.CurrentDemoModuleHelper.HasOptionsContent;
			DemoModuleView = demoModuleLoader.CurrentDemoModuleHelper.DemoModuleView;
			BindingOperations.SetBinding(demoModuleLoader.CurrentDemoModuleHelper, DemoModuleHelper.DemoModuleViewProperty, new Binding("DemoModuleView") { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
			BindingOperations.SetBinding(demoModuleLoader.CurrentDemoModuleHelper, DemoModuleHelper.DemoModuleOptionsViewProperty, new Binding("OptionsView") { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
			IsOptionsExpanded = demoModuleLoader.CurrentDemoModuleHelper.OptionsExpanded;
			BindingOperations.SetBinding(demoModuleLoader.CurrentDemoModuleHelper, DemoModuleHelper.OptionsExpandedProperty, new Binding("IsOptionsExpanded") { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
			Links = this.loadModule == null ? null : this.loadModule.Links;
			ThemesMode = GetThemeMode(loadModule);
			demoModuleLoader.CurrentDemoModuleHelper.AllowRtl = this.loadModule == null ? true : this.loadModule.AllowRtl;
			UpdateDemoFlowDirection(DemoFlowDirection);
		}
		ToolbarThemesMode GetThemeMode(ModuleDescription module) {
			if(module == null)
				return ToolbarThemesMode.None;
			if(!module.AllowSwitchingTheme)
				return ToolbarThemesMode.None;
			if(module.SupportTouchThemes)
				return ToolbarThemesMode.All;
			return ToolbarThemesMode.ClassicOnly;
		}
		public void OnDemoCame() {
			this.moduleLoadingInProgress = false;
			DemoBaseControl.ActualPageChanged += OnDemoBaseControlActualPageChanged2;
			if(DemoBaseControl.ActualPage == DemoBaseControlPage.Main)
				OnDemoBaseControlActualPageChanged2(DemoBaseControl, null);
		}
		void OnDemoBaseControlActualPageChanged2(object sender, DepPropertyChangedEventArgs e) {
			if(DemoBaseControl.ActualPage != DemoBaseControlPage.Main) return;
			DemoBaseControl.ActualPageChanged -= OnDemoBaseControlActualPageChanged2;
			this.demoModuleLoader.AppearCurrentDemoModule();
			if(ModuleAppear != null)
				ModuleAppear(this, new ModuleAppearEventArgs(this.demoModuleLoader.CurrentDemoModuleHelper.DemoModule, this.demoModuleLoader.CurrentDemoModuleHelper.DemoModuleException));
			BeginLoadNextModule();
		}
		void RaiseLoadingInProgressChanged() {
			if(LoadingInProgressChanged != null) {
				LoadingInProgressChanged(this, new ThePropertyChangedEventArgs<bool>(false, LoadingInProgress));
			}
		}
		void RaiseModuleChanged() {
			this.moduleChanged = true;
			BeginLoadNextModule();
		}
		void UpdateDemoFlowDirection(FlowDirection demoFlowDirection) {
			this.demoModuleLoader.CurrentDemoModuleHelper.DemoFlowDirection = demoFlowDirection;
		}
		void OnDemoModuleViewChanged(ToolbarView oldValue, ToolbarView newValue) {
			if(newValue == ToolbarView.Code)
				LoadCode(oldValue);
			else
				LoadDemo(oldValue);
		}
		void LoadCode(ToolbarView oldValue) {
			if(CodeTexts != null) {
				foreach(CodeTextDescription ctd in CodeTexts) {
					ctd.CodeText.Load();
				}
			}
			this.nextView = ToolbarView.Code;
			ChangeCurrentModuleViewAndRaiseChanged(oldValue);
		}
		void LoadDemo(ToolbarView oldValue) {
			this.nextView = ToolbarView.Demo;
			ChangeCurrentModuleViewAndRaiseChanged(oldValue);
		}
		void ChangeCurrentModuleViewAndRaiseChanged(ToolbarView oldView) {
			if (nextView == ToolbarView.Demo) 
				ShowCurrent(); 
			else 
				Leave();
			if(LoadingInProgress || oldView == nextView)
				return;
			this.demoModuleLoader.CurrentDemoModuleHelper.DemoModuleView = nextView;
			if(nextView == ToolbarView.Demo)
				LoadingBackground = ThemeSelectorHelper.ThemeToLoadingSplashBackground(ThemeManager.ApplicationThemeName);
			else
				LoadingBackground = LoadingSplashBackground.White;
			if(DemoModuleViewChanged != null)
				DemoModuleViewChanged(this, new ThePropertyChangedEventArgs<ToolbarView>(oldView, nextView));
		}
		void RaiseDemoFlowDirectionChanged() {
			demoModuleLoader.CurrentDemoModuleHelper.DemoFlowDirection = DemoFlowDirection;
			if(DemoFlowDirectionChanged != null)
				DemoFlowDirectionChanged(this, new ThePropertyChangedEventArgs<FlowDirection>(FlowDirection.LeftToRight, DemoFlowDirection));
		}
		void RaiseOptionsViewChanged() {
			if(OptionsViewChanged != null)
				OptionsViewChanged(this, new ThePropertyChangedEventArgs<ToolbarSidebarView>(ToolbarSidebarView.Options, OptionsView));
		}
		void RaiseTitleChanged() {
			if(TitleChanged != null)
				TitleChanged(this, new ThePropertyChangedEventArgs<LString>(null, Title));
		}
		void RaiseIsOptionsExpandedChanged() {
			if(IsOptionsExpandedChanged != null)
				IsOptionsExpandedChanged(this, new ThePropertyChangedEventArgs<bool>(false, IsOptionsExpanded));
		}
		void RaiseHasOptionsChanged() {
			if(HasOptionsChanged != null)
				HasOptionsChanged(this, new ThePropertyChangedEventArgs<bool>(false, HasOptions));
		}
		void RaiseLinksChanged() {
			ShowLinks = Links != null && Links.Count != 0;
		}
		void RaiseAllowRtlChanged() {
			if(AllowRtlChanged != null) {
				AllowRtlChanged(this, new ThePropertyChangedEventArgs<bool>(false, AllowRtl));
			}
		}
		void RaiseSidebarTagChanged() {
			if(SidebarTagChanged != null)
				SidebarTagChanged(this, new ThePropertyChangedEventArgs<string>(null, SidebarTag));
		}
		void RaiseSidebarIconChanged() {
			if(SidebarIconChanged != null)
				SidebarIconChanged(this, new ThePropertyChangedEventArgs<ImageSource>(null, SidebarIcon));
		}
		void RaiseSidebarIconSelectedChanged() {
			if(SidebarIconSelectedChanged != null)
				SidebarIconSelectedChanged(this, new ThePropertyChangedEventArgs<ImageSource>(null, SidebarIconSelected));
		}
		void RaiseSidebar2TagChanged() {
			if(Sidebar2TagChanged != null)
				Sidebar2TagChanged(this, new ThePropertyChangedEventArgs<string>(null, Sidebar2Tag));
		}
		void RaiseSidebar2IconChanged() {
			if(Sidebar2IconChanged != null)
				Sidebar2IconChanged(this, new ThePropertyChangedEventArgs<ImageSource>(null, Sidebar2Icon));
		}
		void RaiseSidebar2IconSelectedChanged() {
			if(Sidebar2IconSelectedChanged != null)
				Sidebar2IconSelectedChanged(this, new ThePropertyChangedEventArgs<ImageSource>(null, Sidebar2IconSelected));
		}
		void OnDemoBaseControlModuleReload(object sender, EventArgs e) {
			this.moduleChanged = true;
			BeginLoadNextModule();
		}
		void OnParentDemoFlowDirectionChanged(object sender, ThePropertyChangedEventArgs<FlowDirection> e) {
			DemoFlowDirection = e.NewValue;
		}
	}
}
