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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.DemoData.Helpers;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using System.Linq;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer;
using DevExpress.Utils.About;
namespace DevExpress.Xpf.DemoBase {
	class DemoModuleHelper : DependencyObject {
		#region Dependency Properties
		public static readonly DependencyProperty DescriptionRowsCountProperty;
		public static readonly DependencyProperty LinksProperty;
		public static readonly DependencyProperty OptionsTopTextProperty;
		public static readonly DependencyProperty IsPopupContentInvisibleProperty;
		public static readonly DependencyProperty IsRTLSupportedProperty;
		public static readonly DependencyProperty DemoModuleViewProperty;
		public static readonly DependencyProperty DemoModuleOptionsViewProperty;
		public static readonly DependencyProperty OptionsExpandedProperty;
		public static readonly DependencyProperty DemoModuleProperty;
		public static readonly DependencyProperty DemoFlowDirectionProperty;
		public static readonly DependencyProperty OwnerProperty;
		public static readonly DependencyProperty SidebarTagProperty;
		public static readonly DependencyProperty SidebarIconProperty;
		public static readonly DependencyProperty SidebarIconSelectedProperty;
		static DemoModuleHelper() {
			Type ownerType = typeof(DemoModuleHelper);
			DescriptionRowsCountProperty = DependencyProperty.Register("DescriptionRowsCount", typeof(int), ownerType, new PropertyMetadata(1, RaiseDescriptionRowsCountChanged));
			LinksProperty = DependencyProperty.Register("Links", typeof(List<object>), ownerType, new PropertyMetadata(null, RaiseLinksChanged));
			OptionsTopTextProperty = DependencyProperty.Register("OptionsTopText", typeof(string), ownerType, new PropertyMetadata(string.Empty, RaiseOptionsTopTextChanged));
			IsPopupContentInvisibleProperty = DependencyProperty.Register("IsPopupContentInvisible", typeof(bool), ownerType, new PropertyMetadata(true, RaiseIsPopupContentInvisibleChanged));
			IsRTLSupportedProperty = DependencyProperty.Register("IsRTLSupported", typeof(bool), ownerType, new PropertyMetadata(false, RaiseIsRTLSupportedChanged));
			DemoModuleViewProperty = DependencyProperty.Register("DemoModuleView", typeof(ToolbarView), ownerType, new PropertyMetadata(ToolbarView.Demo, RaiseDemoModuleViewChanged));
			DemoModuleOptionsViewProperty = DependencyProperty.Register("DemoModuleOptionsView", typeof(ToolbarSidebarView), ownerType, new PropertyMetadata(ToolbarSidebarView.Options, RaiseDemoModuleOptionsViewChanged));
			OptionsExpandedProperty = DependencyProperty.Register("OptionsExpanded", typeof(bool), ownerType, new PropertyMetadata(true, RaiseOptionsExpandedChanged));
			DemoModuleProperty = DependencyProperty.Register("DemoModule", typeof(FrameworkElement), ownerType, new PropertyMetadata(null, RaiseDemoModuleChanged));
			DemoFlowDirectionProperty = DependencyProperty.Register("DemoFlowDirection", typeof(FlowDirection), ownerType, new PropertyMetadata(FlowDirection.LeftToRight, RaiseDemoFlowDirectionChanged));
			OwnerProperty = DependencyProperty.Register("Owner", typeof(object), ownerType, new PropertyMetadata(null, RaiseOwnerChanged));
			SidebarTagProperty = DependencyProperty.Register("SidebarTag", typeof(string), ownerType, new PropertyMetadata("Sidebar", RaiseSidebarTagChanged));
			SidebarIconProperty = DependencyProperty.Register("SidebarIcon", typeof(ImageSource), ownerType, new PropertyMetadata(null, RaiseSidebarIconChanged));
			SidebarIconSelectedProperty = DependencyProperty.Register("SidebarIconSelected", typeof(ImageSource), ownerType, new PropertyMetadata(null, RaiseSidebarIconSelectedChanged));
		}
		int descriptionRowsCountValue = 1;
		List<object> linksValue = null;
		string optionsTopTextValue = string.Empty;
		bool isPopupContentInvisibleValue = true;
		bool isRTLSupportedValue = false;
		ToolbarView demoModuleViewValue = ToolbarView.Demo;
		ToolbarSidebarView demoModuleOptionsViewValue = ToolbarSidebarView.Options;
		FrameworkElement demoModuleValue = null;
		FlowDirection demoFlowDirectionValue = FlowDirection.LeftToRight;
		bool optionsExpandedValue = true;
		object ownerValue;
		string sidebarTag;
		ImageSource sidebarIcon;
		ImageSource sidebarIconSelected;
		static void RaiseSidebarTagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).sidebarTag = (string)e.NewValue;
		}
		static void RaiseSidebarIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).sidebarIcon = (ImageSource)e.NewValue;
		}
		static void RaiseSidebarIconSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).sidebarIconSelected = (ImageSource)e.NewValue;
		}
		static void RaiseDescriptionRowsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).descriptionRowsCountValue = (int)e.NewValue;
		}
		static void RaiseLinksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).linksValue = (List<object>)e.NewValue;
		}
		static void RaiseOptionsTopTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).optionsTopTextValue = (string)e.NewValue;
		}
		static void RaiseIsRTLSupportedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).isRTLSupportedValue = (bool)e.NewValue;
		}
		static void RaiseDemoModuleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).demoModuleValue = (FrameworkElement)e.NewValue;
		}
		static void RaiseOwnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).ownerValue = e.NewValue;
		}
		static void RaiseDemoModuleViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).demoModuleViewValue = (ToolbarView)e.NewValue;
			((DemoModuleHelper)d).RaiseDemoModuleViewChanged(e);
		}
		static void RaiseDemoModuleOptionsViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).demoModuleOptionsViewValue = (ToolbarSidebarView)e.NewValue;
			((DemoModuleHelper)d).RaiseDemoModuleOptionsViewChanged(e);
		}
		static void RaiseIsPopupContentInvisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).isPopupContentInvisibleValue = (bool)e.NewValue;
			((DemoModuleHelper)d).RaiseIsPopupContentInvisibleChanged(e);
		}
		static void RaiseDemoFlowDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).demoFlowDirectionValue = (FlowDirection)e.NewValue;
			((DemoModuleHelper)d).RaiseDemoFlowDirectionChanged(e);
		}
		static void RaiseOptionsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleHelper)d).optionsExpandedValue = (bool)e.NewValue;
			((DemoModuleHelper)d).RaiseOptionsExpandedChanged(e);
		}
		public bool IsSidebarVisible {
			get { return (bool)GetValue(IsSidebarVisibleProperty); }
			set { SetValue(IsSidebarVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsSidebarVisibleProperty =
			DependencyProperty.Register("IsSidebarVisible", typeof(bool), typeof(DemoModuleHelper), new PropertyMetadata(default(bool)));
		public bool IsSidebarButtonEnabled {
			get { return (bool)GetValue(IsSidebarButtonEnabledProperty); }
			set { SetValue(IsSidebarButtonEnabledProperty, value); }
		}
		public static readonly DependencyProperty IsSidebarButtonEnabledProperty =
			DependencyProperty.Register("IsSidebarButtonEnabled", typeof(bool), typeof(DemoModuleHelper), new PropertyMetadata(true));
		public bool IsSidebar2Visible {
			get { return (bool)GetValue(IsSidebar2VisibleProperty); }
			set { SetValue(IsSidebar2VisibleProperty, value); }
		}
		public static readonly DependencyProperty IsSidebar2VisibleProperty =
			DependencyProperty.Register("IsSidebar2Visible", typeof(bool), typeof(DemoModuleHelper), new PropertyMetadata(default(bool)));
		public bool IsSidebar2ButtonEnabled {
			get { return (bool)GetValue(IsSidebar2ButtonEnabledProperty); }
			set { SetValue(IsSidebar2ButtonEnabledProperty, value); }
		}
		public static readonly DependencyProperty IsSidebar2ButtonEnabledProperty =
			DependencyProperty.Register("IsSidebar2ButtonEnabled", typeof(bool), typeof(DemoModuleHelper), new PropertyMetadata(true));
		#endregion
		Style demoModuleControlStyle;
		Action<DemoModuleHelper> endAppearAction;
		bool isPopupContentInvisibleInternal = true;
		public DemoModuleHelper(ContentPresenter demoModulePresenter, Style demoModuleControlStyle) {
			this.demoModuleControlStyle = demoModuleControlStyle;
			DemoModulePresenter = demoModulePresenter;
			DemoModulePresenter.SetBinding(ContentPresenter.ContentProperty, new Binding("DemoModule") { Source = this, Mode = BindingMode.OneWay });
		}
		public List<object> Links { get { return linksValue; } set { SetValue(LinksProperty, value); } }
		public int DescriptionRowsCount { get { return descriptionRowsCountValue; } private set { SetValue(DescriptionRowsCountProperty, value); } }
		public string OptionsTopText { get { return optionsTopTextValue; } private set { SetValue(OptionsTopTextProperty, value); } }
		public bool IsPopupContentInvisible { get { return isPopupContentInvisibleValue; } set { SetValue(IsPopupContentInvisibleProperty, value); } }
		public bool IsRTLSupported { get { return isRTLSupportedValue; } set { SetValue(IsRTLSupportedProperty, value); } }
		public ToolbarView DemoModuleView { get { return demoModuleViewValue; } set { SetValue(DemoModuleViewProperty, value); } }
		public ToolbarSidebarView DemoModuleOptionsView { get { return demoModuleOptionsViewValue; } set { SetValue(DemoModuleOptionsViewProperty, value); } }
		public bool OptionsExpanded { get { return optionsExpandedValue; } set { SetValue(OptionsExpandedProperty, value); } }
		public FlowDirection DemoFlowDirection { get { return demoFlowDirectionValue; } set { SetValue(DemoFlowDirectionProperty, value); } }
		public object Owner { get { return ownerValue; } set { SetValue(OwnerProperty, value); } }
		public ContentPresenter DemoModulePresenter { get; private set; }
		public FrameworkElement DemoModule { get { return demoModuleValue; } private set { SetValue(DemoModuleProperty, value); } }
		public Exception DemoModuleException { get; private set; }
		public CodeViewControl CodeTextViewer { get; private set; }
		public FrameworkElement DemoContentElement { get; private set; }
		public bool HasOptionsContent { get; private set; }
		public bool AllowThemeChange { get; private set; }
		bool supportTouchThemes;
		bool supportDarkThemes;
		public bool SupportTouchThemes {
			get { return supportTouchThemes; }
			set {
				if(supportTouchThemes == value)
					return;
				supportTouchThemes = value;
				DemoModule demoModule = ((DemoModule)DemoModule);
				if(demoModule == null)
					return;
				demoModule.SupportTouchThemes = value;
			}
		}
		public bool SupportDarkThemes {
			get { return supportDarkThemes; }
			set {
				if(supportDarkThemes == value)
					return;
				supportDarkThemes = value;
				DemoModule demoModule = ((DemoModule)DemoModule);
				if(demoModule == null)
					return;
				demoModule.SupportDarkThemes = value;
			}
		}
		public bool AllowRtl { get; set; }
		public string SidebarTag { get { return sidebarTag; } private set { SetValue(SidebarTagProperty, value); } }
		public ImageSource SidebarIcon { get { return sidebarIcon; } private set { SetValue(SidebarIconProperty, value); } }
		public ImageSource SidebarIconSelected { get { return sidebarIconSelected; } private set { SetValue(SidebarIconSelectedProperty, value); } }
		public string Sidebar2Tag {
			get { return (string)GetValue(Sidebar2TagProperty); }
			set { SetValue(Sidebar2TagProperty, value); }
		}
		public static readonly DependencyProperty Sidebar2TagProperty =
			DependencyProperty.Register("Sidebar2Tag", typeof(string), typeof(DemoModuleHelper), new PropertyMetadata(null));
		public ImageSource Sidebar2Icon {
			get { return (ImageSource)GetValue(Sidebar2IconProperty); }
			set { SetValue(Sidebar2IconProperty, value); }
		}
		public static readonly DependencyProperty Sidebar2IconProperty =
			DependencyProperty.Register("Sidebar2Icon", typeof(ImageSource), typeof(DemoModuleHelper), new PropertyMetadata(null));
		public ImageSource Sidebar2IconSelected {
			get { return (ImageSource)GetValue(Sidebar2IconSelectedProperty); }
			set { SetValue(Sidebar2IconSelectedProperty, value); }
		}
		public static readonly DependencyProperty Sidebar2IconSelectedProperty =
			DependencyProperty.Register("Sidebar2IconSelected", typeof(ImageSource), typeof(DemoModuleHelper), new PropertyMetadata(null));
		public ToolbarSidebarView? InitialOptionsView { get; set; }
		public bool CanLeave() {
			DemoModule demoModule = DemoModule as DemoModule;
			return demoModule == null || demoModule.CanLeave();
		}
		public void Leave() {
			DemoModule demoModule = DemoModule as DemoModule;
			if(demoModule != null)
				demoModule.Leave();
		}
		public void Show() {
			DemoModule demoModule = DemoModule as DemoModule;
			if(demoModule != null)
				demoModule.Show();
		}
		public void BeginAppearDemoModule(Type demoModuleType, Action<DemoModuleHelper> endAppearAction) {
			DevExpress.DemoData.Helpers.BackgroundHelper.DoInBackground(null, () => BeginAppearDemoModuleCore(demoModuleType, endAppearAction));
		}
		void BeginAppearDemoModuleCore(Type demoModuleType, Action<DemoModuleHelper> endAppearAction) {
			if(demoModuleType == null) {
				DemoModule = null;
				endAppearAction(this);
				return;
			}
			try {
				DemoModule demoModule = CreateDemoModule(demoModuleType);
				if(demoModule.Content as DemoModuleControl == null)
					demoModule.Content = new DemoModuleControl();
				SetDemoFlowDirection(demoModule, false);
				if(this.demoModuleControlStyle != null)
					demoModule.DemoModuleControl.Style = this.demoModuleControlStyle;
				demoModule.BeginAppear();
				this.endAppearAction = endAppearAction;
				demoModule.Loaded += OnDemoModuleActualLoaded;
				demoModule.IsPopupContentInvisible = true;
				DescriptionRowsCount = demoModule.GetDescriptionRowsCount();
				IsSidebarVisible = demoModule.SupportSidebarContent();
				SidebarTag = demoModule.GetSidebarTag();
				SidebarIcon = demoModule.GetSidebarIcon();
				SidebarIconSelected = demoModule.GetSidebarIconSelected();
				IsSidebarButtonEnabled = demoModule.IsSidebarButtonEnabled();
				IsSidebar2Visible = demoModule.SupportSidebar2Content();
				Sidebar2Tag = demoModule.GetSidebar2Tag();
				Sidebar2Icon = demoModule.GetSidebar2Icon();
				Sidebar2IconSelected = demoModule.GetSidebar2IconSelected();
				IsSidebar2ButtonEnabled = demoModule.IsSidebar2ButtonEnabled();
				OptionsTopText = demoModule.DemoModuleControl.OptionsTopText;
				SupportTouchThemes = demoModule.SupportTouchThemes;
				SupportDarkThemes = demoModule.SupportDarkThemes;
				HasOptionsContent = demoModule.DemoModuleControl.OptionsContent != null;
				demoModule.SetBinding(DevExpress.Xpf.DemoBase.DemoModule.OwnerProperty, new Binding("Owner") { Source = this, Mode = BindingMode.OneWay });
				DemoModule = demoModule;
			} catch (Exception e) {
				DemoModuleException = e;
				DemoModule = new TextBlock() { Foreground = new SolidColorBrush(Colors.Purple), TextWrapping = TextWrapping.Wrap, Text = ExceptionHelper.GetMessage(e) };
				endAppearAction(this);
			}
		}
		public void EndAppearDemoModule() {
			DemoModule demoModule = DemoModule as DemoModule;
			if(demoModule != null)
				demoModule.EndAppear();
			isPopupContentInvisibleInternal = false;
			UpdateIsPopupContentInvisibleAndView();
		}
		public void BeginDisappearDemoModule(Action<DemoModuleHelper> endDisappearAction) {
			DevExpress.DemoData.Helpers.BackgroundHelper.DoInBackground(null, () => BeginDisappearDemoModuleCore(endDisappearAction));
		}
		void BeginDisappearDemoModuleCore(Action<DemoModuleHelper> endDisappearAction) {
			DemoModule demoModule = DemoModule as DemoModule;
			if(demoModule != null) {
				if(!demoModule.BeginDisappear()) return;
			}
			isPopupContentInvisibleInternal = true;
			UpdateIsPopupContentInvisibleAndView();
			endDisappearAction(this);
		}
		public void EndDispappearDemoModule() {
			DemoModule demoModule = DemoModule as DemoModule;
			DemoContentElement = null;
			CodeTextViewer = null;
			DemoModule = null;
			if(demoModule != null) {
				demoModule.Owner = null;
				demoModule.EndDisappear();
			}
			DescriptionRowsCount = 1;
			OptionsTopText = string.Empty;
			DemoModuleView = ToolbarView.Demo;
			DemoModuleException = null;
			isPopupContentInvisibleInternal = true;
			UpdateIsPopupContentInvisibleAndView();
		}
		void OnDemoModuleActualLoaded(object sender, EventArgs e) {
			DemoModule demoModule = (DemoModule)sender;
			demoModule.Loaded -= OnDemoModuleActualLoaded;
			CodeTextViewer = demoModule.DemoModuleControl.CodeTextViewer;
			DemoContentElement = demoModule.DemoModuleControl.DemoContentElement;
			UpdateDemoFlowDirection();
			if(endAppearAction != null) {
				this.endAppearAction(this);
				this.endAppearAction = null;
			}
		}
		void UpdateDemoFlowDirection() {
			DemoModule demoModule = DemoModule as DemoModule;
			if(demoModule == null) return;
			SetDemoFlowDirection(demoModule, true);
		}
		void SetDemoFlowDirection(DemoModule demoModule, bool correctOptionsPanel) {
			if(correctOptionsPanel) {
				foreach(FrameworkElement child in GetAllChildren(demoModule.DemoModuleControl, demoModule.DemoModuleControl.OptionsContentContainer)) {
					if(child != null)
						child.FlowDirection = FlowDirection.LeftToRight;
				}
			}
			FlowDirectionHelper.SetFlowDirection(demoModule.DemoModuleControl, AllowRtl ? DemoFlowDirection : FlowDirection.LeftToRight);
		}
		void RaiseDemoModuleOptionsViewChanged(DependencyPropertyChangedEventArgs e) {
			UpdateDemoModuleOptionsView();
		}
		void RaiseOptionsExpandedChanged(DependencyPropertyChangedEventArgs e) {
			DemoModule demoModule = DemoModule as DemoModule;
			if(demoModule == null) return;
			demoModule.DemoModuleControl.OptionsExpanded = OptionsExpanded;
		}
		void RaiseIsPopupContentInvisibleChanged(DependencyPropertyChangedEventArgs e) {
			UpdateIsPopupContentInvisibleAndView();
		}
		void RaiseDemoModuleViewChanged(DependencyPropertyChangedEventArgs e) {
			UpdateIsPopupContentInvisibleAndView();
		}
		void UpdateIsPopupContentInvisibleAndView() {
			DemoModule demoModule = DemoModule as DemoModule;
			if(demoModule == null) return;
			demoModule.IsPopupContentInvisible = isPopupContentInvisibleInternal || IsPopupContentInvisible || DemoModuleView != ToolbarView.Demo;
			demoModule.DemoModuleControl.View = DemoModuleView;
		}
		void UpdateDemoModuleOptionsView() {
			DemoModule demoModule = DemoModule as DemoModule;
			if(demoModule == null) return;
			demoModule.DemoModuleControl.OptionsView = DemoModuleOptionsView;
		}
		void RaiseDemoFlowDirectionChanged(DependencyPropertyChangedEventArgs e) {
			FlowDirection newValue = (FlowDirection)e.NewValue;
			UpdateDemoFlowDirection();
		}
		IEnumerable<FrameworkElement> FindLogicalChildren(FrameworkElement root, Func<FrameworkElement, bool> condition) {
			foreach(FrameworkElement child in LogicalTreeHelper.GetChildren(root)) {
				if(condition(child))
					yield return child;
			}
		}
		bool IsVisualParent(FrameworkElement child, FrameworkElement parent, FrameworkElement root) {
			for(FrameworkElement node = child; node != null && node != root;) {
				FrameworkElement nodeParent = VisualTreeHelper.GetParent(node) as FrameworkElement;
				if(nodeParent == parent) return true;
				node = nodeParent;
			}
			return false;
		}
		IEnumerable<FrameworkElement> GetAllChildren(FrameworkElement root, FrameworkElement visualParent) {
			foreach(FrameworkElement child in FindLogicalChildren(root, fe => IsVisualParent(fe, visualParent, root))) {
				yield return child;
			}
			yield return visualParent;
		}
		void ApplyToAllChildren(FrameworkElement root, Action<FrameworkElement> action, int depth) {
			if(depth < 0) return;
			action(root);
			if(depth < 1) return;
			for(int i = VisualTreeHelper.GetChildrenCount(root); --i >= 0;) {
				FrameworkElement child = VisualTreeHelper.GetChild(root, i) as FrameworkElement;
				if(child != null)
					ApplyToAllChildren(child, action, depth - 1);
			}
		}
		DemoModule CreateDemoModule(Type demoModuleType) {
			DemoModule demoModule = (DemoModule)Activator.CreateInstance(demoModuleType);
			demoModule.Width = demoModule.Height = double.NaN;
			demoModule.MaxWidth = demoModule.MaxHeight = double.PositiveInfinity;
			return demoModule;
		}
		public IList<string> GetCodeFileNames() {
			DemoModule demoModule = DemoModule as DemoModule;
			return demoModule == null ? new List<string>() : demoModule.GetCodeFileNames();
		}
	}
}
