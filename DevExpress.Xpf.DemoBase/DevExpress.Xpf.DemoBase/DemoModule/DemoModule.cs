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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.DemoData.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer;
#if !DEMO
using DevExpress.Utils;
#endif
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.DemoBase {
	public class DemoModule : UserControl {
		#region Dependency Properties
		public static readonly RoutedEvent ModuleAppearEvent;
		public static readonly RoutedEvent BeforeModuleDisappearEvent;
		public static readonly DependencyProperty IsPopupContentInvisibleProperty;
		public static readonly DependencyProperty OwnerProperty;
		public static readonly DependencyProperty ThemeProperty;
		static DemoModule() {
			Type ownerType = typeof(DemoModule);
			IsPopupContentInvisibleProperty = DependencyProperty.Register("IsPopupContentInvisible", typeof(bool), ownerType, new PropertyMetadata(false, RaiseIsPopupContentInvisibleChanged));
			ModuleAppearEvent = EventManager.RegisterRoutedEvent("ModuleAppear", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			BeforeModuleDisappearEvent = EventManager.RegisterRoutedEvent("BeforeModuleDisappear", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			OwnerProperty = DependencyProperty.Register("Owner", typeof(object), ownerType, new PropertyMetadata(null));
			ThemeProperty = DependencyProperty.Register("Theme", typeof(Theme), ownerType, new PropertyMetadata(null, RaiseThemeChanged));
			BarNameScope.IsScopeOwnerProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
		}
		static void RaiseIsPopupContentInvisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModule)d).RaiseIsPopupContentInvisibleChanged(e);
			if (!(bool)e.NewValue)
				Bars.BarManagerHelper.ShowFloatingBars(d, true);
			else
				Bars.BarManagerHelper.HideFloatingBars(d, true);
		}
		static void RaiseThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModule)d).RaiseThemeChanged(e);
		}
		#endregion
		string savedThemeName = null;
		public DemoModule() {
			SupportTouchThemes = true;
			SupportDarkThemes = true;
			AllowSwitchingTheme = true;
			Unloaded += DemoModule_Unloaded;
		}
		void DemoModule_Unloaded(object sender, System.Windows.RoutedEventArgs e) {
			themeSelectorHelper.Dispose();
		}
		public DemoModuleControl DemoModuleControl { get { return (DemoModuleControl)Content; } }
		public object Owner { get { return GetValue(OwnerProperty); } set { SetValue(OwnerProperty, value); } }
		public bool IsPopupContentInvisible { get { return (bool)GetValue(IsPopupContentInvisibleProperty); } internal set { SetValue(IsPopupContentInvisibleProperty, value); } }
		public event RoutedEventHandler ModuleAppear { add { this.AddHandler(ModuleAppearEvent, value); } remove { this.RemoveHandler(ModuleAppearEvent, value); } }
		public event RoutedEventHandler BeforeModuleDisappear { add { this.AddHandler(BeforeModuleDisappearEvent, value); } remove { this.RemoveHandler(BeforeModuleDisappearEvent, value); } }
		public virtual int GetDescriptionRowsCount() { return 1; }
		public virtual bool SupportPopupContent() { return false; }
		public virtual object GetPopupContent() { return null; }
		public virtual void UpdatePopupContent(object popupContent) { }
		public virtual bool SupportSidebarContent() { return false; }
		public virtual bool SupportSidebar2Content() { return false; }
		public virtual bool IsSidebarButtonEnabled() { return true; }
		public virtual bool IsSidebar2ButtonEnabled() { return true; }
		public virtual string GetSidebarTag() { return "Sidebar"; }
		public virtual string GetSidebar2Tag() { return "Sidebar2"; }
		public virtual ImageSource GetSidebarIcon() { return null; }
		public virtual ImageSource GetSidebar2Icon() { return null; }
		public virtual ImageSource GetSidebarIconSelected() { return null; }
		public virtual ImageSource GetSidebar2IconSelected() { return null; }
		public virtual object GetSidebarContent() { return null; }
		public virtual object GetSidebar2Content() { return null; }
		public virtual void UpdateSidebarContent(object sidebarContent) { }
		public virtual void UpdateSidebar2Content(object sidebar2Content) { }
		public bool AllowSwitchingTheme { get; set; }
		bool supportTouchThemes;
		bool supportDarkThemes;
		public bool SupportTouchThemes {
			get { return supportTouchThemes; }
			set { supportTouchThemes = value; }
		}
		public bool SupportDarkThemes {
			get { return supportDarkThemes; }
			set { supportDarkThemes = value; }
		}
		public virtual bool AllowRtl { get { return true; } }
		public IList<string> GetCodeFileNames() {
			return CodeFileAttributeParser.GetCodeFiles(GetType(), XamlSuffix, ModulesFolder);
		}
		protected virtual string XamlSuffix { get { return "(.SL).xaml"; } }
		protected virtual string ModulesFolder { get { return "Modules"; } }
		protected internal virtual bool CanLeave() { return true; }
		public virtual void Leave() { }
		public virtual void Show() { }
		ThemeSelectorHelper themeSelectorHelper = new ThemeSelectorHelper();
		public bool BeginDisappear() {
			if(!CanLeave()) return false;
			RaiseBeforeModuleDisappear();
			return true;
		}
		public void EndDisappear() {
			RaiseAfterModuleDisappear();
			if(savedThemeName != null)
				ThemeManager.ApplicationThemeName = savedThemeName;
			Clear();
		}
		public void BeginAppear() {
			RaiseBeforeModuleAppear();
		}
		public void EndAppear() {
			RaiseModuleAppear();
		}
		protected virtual void RaiseIsPopupContentInvisibleChanged(DependencyPropertyChangedEventArgs e) { }
		protected virtual void RaiseBeforeModuleAppear() { }
		protected virtual Theme LocalTheme { get { return null; } }
		protected virtual void RaiseModuleAppear() {
			this.RaiseEvent(new RoutedEventArgs() { RoutedEvent = DemoModule.ModuleAppearEvent });
		}
		protected virtual void RaiseBeforeModuleDisappear() {
			this.RaiseEvent(new RoutedEventArgs() { RoutedEvent = DemoModule.BeforeModuleDisappearEvent });
			DemoModuleControl.RaiseBeforeModuleDisappear();
		}
		protected virtual void RaiseAfterModuleDisappear() { }
		protected internal virtual object GetModuleDataContext() { return this; }
		protected virtual void Clear() {
		}
		protected virtual void RaiseThemeChanged(DependencyPropertyChangedEventArgs e) {
			if(e.NewValue == null) return;
			themeSelectorHelper.ApplicationTheme = (Theme)e.NewValue;
		}
		protected override Size MeasureOverride(Size constraint) {
			constraint = base.MeasureOverride(constraint);
			ClearAutomationEventsHelper.ClearAutomationEvents();
			return constraint;
		}
	}
	public class ThemeOverridePair {
		public Theme Theme { get; set; }
		public Theme ThemeOverride { get; set; }
	}
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class CodeFileAttribute : Attribute {
		public CodeFileAttribute(string path) {
			Path = path;
		}
		public string Path { get; private set; }
	}
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class NoAutogeneratedCodeFilesAttribute : Attribute { }
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CodeFilesAttribute : Attribute {
		public CodeFilesAttribute(string path1, string path2, string path3, string path4, string path5, string path6, string path7) {
			Paths = new[] { path1, path2, path3, path4, path5, path6, path7 };
		}
		public CodeFilesAttribute(string path1, string path2, string path3, string path4, string path5, string path6) {
			Paths = new[] { path1, path2, path3, path4, path5, path6 };
		}
		public CodeFilesAttribute(string path1, string path2, string path3, string path4, string path5) {
			Paths = new[] { path1, path2, path3, path4, path5 };
		}
		public CodeFilesAttribute(string path1, string path2, string path3, string path4) {
			Paths = new[] { path1, path2, path3, path4 };
		}
		public CodeFilesAttribute(string path1, string path2, string path3) {
			Paths = new[] { path1, path2, path3 };
		}
		public CodeFilesAttribute(string path1, string path2) {
			Paths = new[] { path1, path2 };
		}
		public CodeFilesAttribute(string str) {
			Paths = str.Split(';').Select(p => p.Trim()).ToArray();
		}
		public string[] Paths { get; private set; }
	}
	[DefaultProperty("Content"), ContentProperty("Content")]
	[TemplatePart(Name = "OptionsContentContainer", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "ContentContainer", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "DemoContentElement", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "CornerPopup", Type = typeof(CornerPopup))]
	[TemplatePart(Name = "XamlTextViewer", Type = typeof(CodeViewControl))]
	[TemplatePart(Name = "CodeTextViewer", Type = typeof(CodeViewControl))]
	[TemplatePart(Name = "CodeOwner", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "OptionsTopText", Type = typeof(FrameworkElement))]
	public class DemoModuleControl : Control {
		public static DependencyObject FindDemoContent(Type tp, DependencyObject root) {
			VisualTreeEnumerator en = new VisualTreeEnumerator(root);
			while(en.MoveNext()) {
				if(en.Current.GetType() == tp) {
					return en.Current;
				}
			}
			return null;
		}
		public static DemoModuleControl FindParentDemoModuleControl(DependencyObject d) {
			for(DependencyObject parent = d; parent != null; parent = VisualTreeHelper.GetParent(parent)) {
				if(parent is DemoModuleControl) return (DemoModuleControl)parent;
			}
			return null;
		}
		#region Dependency Properties
		public static readonly DependencyProperty OptionsTopTextProperty;
		public static readonly DependencyProperty OptionsContentProperty;
		public static readonly DependencyProperty DemoContentProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly RoutedEvent BeforeModuleDisappearEvent;
		public static readonly DependencyProperty OptionsContentVerticalScrollVisibilityProperty;
		public static readonly DependencyProperty ViewProperty;
		public static readonly DependencyProperty OptionsViewProperty;
		public static readonly DependencyProperty OptionsExpandedProperty;
		public static readonly DependencyProperty SupressGroupFramePaddingInOptionsProperty;
		static DemoModuleControl() {
			Type ownerType = typeof(DemoModuleControl);
			OptionsTopTextProperty = DependencyProperty.Register("OptionsTopText", typeof(string), ownerType, new PropertyMetadata(null));
			ContentProperty = RegisterContentProperty("Content", typeof(FrameworkElement), ownerType, null, null);
			DemoContentProperty = DependencyProperty.Register("DemoContent", typeof(DependencyObject), ownerType, new PropertyMetadata(null));
			OptionsContentProperty = RegisterContentProperty("OptionsContent", typeof(FrameworkElement), ownerType, null, null);
			BeforeModuleDisappearEvent = EventManager.RegisterRoutedEvent("BeforeModuleDisappear", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			OptionsContentVerticalScrollVisibilityProperty = DependencyProperty.Register("OptionsContentVerticalScrollVisibility", typeof(ScrollBarVisibility), ownerType, new PropertyMetadata(ScrollBarVisibility.Auto, RaiseOptionsContentVerticalScrollVisibilityChanged));
			ViewProperty = DependencyProperty.Register("View", typeof(ToolbarView), ownerType, new PropertyMetadata(ToolbarView.Demo, RaiseViewChanged));
			OptionsViewProperty = DependencyProperty.Register("OptionsView", typeof(ToolbarSidebarView), ownerType, new PropertyMetadata(ToolbarSidebarView.Options, RaiseOptionsViewChanged));
			OptionsExpandedProperty = DependencyProperty.Register("OptionsExpanded", typeof(bool), ownerType, new PropertyMetadata(true, RaiseOptionsExpandedChanged));
			SupressGroupFramePaddingInOptionsProperty = DependencyProperty.Register("SupressGroupFramePaddingInOptions", typeof(bool), ownerType, new PropertyMetadata(true, RaiseSupressGroupFramePaddingInOptionsChanged));
		}
		ScrollBarVisibility optionsContentVerticalScrollVisibilityValue = ScrollBarVisibility.Auto;
		ToolbarView viewValue = ToolbarView.Demo;
		ToolbarSidebarView optionsViewValue = ToolbarSidebarView.Options;
		bool optionsExpandedValue = true;
		bool supressGroupFramePaddingInOptionsValue = true;
		static void RaiseOptionsContentVerticalScrollVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleControl)d).optionsContentVerticalScrollVisibilityValue = (ScrollBarVisibility)e.NewValue;
		}
		static void RaiseViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleControl)d).viewValue = (ToolbarView)e.NewValue;
		}
		static void RaiseOptionsViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleControl)d).optionsViewValue = (ToolbarSidebarView)e.NewValue;
		}
		static void RaiseOptionsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleControl)d).optionsExpandedValue = (bool)e.NewValue;
		}
		static void RaiseSupressGroupFramePaddingInOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DemoModuleControl)d).supressGroupFramePaddingInOptionsValue = (bool)e.NewValue;
		}
		public Thickness AboutPanelMargin {
			get { return (Thickness)GetValue(AboutPanelMarginProperty); }
			set { SetValue(AboutPanelMarginProperty, value); }
		}
		public static readonly DependencyProperty AboutPanelMarginProperty =
			DependencyProperty.Register("AboutPanelMargin", typeof(Thickness), typeof(DemoModuleControl), new PropertyMetadata(null));
		#endregion
		public DemoModuleControl() {
			this.SetDefaultStyleKey(typeof(DemoModuleControl));
			Bars.BarNameScope.SetIsScopeOwner(this, true);
			FocusHelper.SetFocusable(this, false);
			Loaded += DemoModuleControl_Loaded;
		}
		Dictionary<object, bool> logicalChildren = new Dictionary<object, bool>();
		protected static DependencyProperty RegisterContentProperty(string name, Type propertyType, Type ownerType, object defaultValue, PropertyChangedCallback changedCallback) {
			PropertyChangedCallback callback = (d, e) => {
				var control = (DemoModuleControl)d;
				object oldChild = e.OldValue;
				object newChild = e.NewValue;
				if(oldChild != null) {
					if(control.logicalChildren.ContainsKey(oldChild))
						control.logicalChildren.Remove(oldChild);
					control.RemoveLogicalChild(oldChild);
				}
				if(newChild != null) {
					control.AddLogicalChild(newChild);
					control.logicalChildren.Add(newChild, true);
				}
				if(changedCallback != null)
					changedCallback(d, e);
			};
			return DependencyProperty.Register(name, propertyType, ownerType, new PropertyMetadata(defaultValue, callback));
		}
		protected override IEnumerator LogicalChildren {
			get {
				var children = new List<object>();
				var baseEnumerator = base.LogicalChildren;
				if(baseEnumerator != null) {
					while(baseEnumerator.MoveNext()) {
						children.Add(baseEnumerator.Current);
					}
				}
				children.AddRange(logicalChildren.Keys);
				return children.GetEnumerator();
			}
		}
		private void DemoModuleControl_Loaded(object sender, RoutedEventArgs e) {
			if(DemoModule.SupportPopupContent()) {
				if(CornerPopup.PopupContent == null) CornerPopup.PopupContent = DemoModule.GetPopupContent();
				else DemoModule.UpdatePopupContent(CornerPopup.PopupContent);
			} else {
				if(CornerPopup != null)
					CornerPopup.PopupContent = null;
			}
			if(DemoModule.SupportSidebarContent()) {
				if(SidebarContent == null)
					SidebarContent = (DependencyObject)DemoModule.GetSidebarContent();
				else DemoModule.UpdateSidebarContent(DemoModule.GetSidebarContent());
			} else {
				SidebarContent = null;
			}
			if(DemoModule.SupportSidebar2Content()) {
				if(Sidebar2Content == null)
					Sidebar2Content = (DependencyObject)DemoModule.GetSidebar2Content();
				else DemoModule.UpdateSidebarContent(DemoModule.GetSidebar2Content());
			} else {
				Sidebar2Content = null;
			}
		}
		public bool OptionsExpanded { get { return optionsExpandedValue; } set { SetValue(OptionsExpandedProperty, value); } }
		public ScrollBarVisibility OptionsContentVerticalScrollVisibility { get { return optionsContentVerticalScrollVisibilityValue; } set { SetValue(OptionsContentVerticalScrollVisibilityProperty, value); } }
		public ToolbarView View { get { return viewValue; } set { SetValue(ViewProperty, value); } }
		public ToolbarSidebarView OptionsView { get { return optionsViewValue; } set { SetValue(OptionsViewProperty, value); } }
		public string OptionsTopText { get { return (string)GetValue(OptionsTopTextProperty); } set { SetValue(OptionsTopTextProperty, value); } }
		public FrameworkElement Content { get { return (FrameworkElement)GetValue(ContentProperty); } set { SetValue(ContentProperty, value); } }
		public DependencyObject DemoContent { get { return (DependencyObject)GetValue(DemoContentProperty); } set { SetValue(DemoContentProperty, value); } }
		public FrameworkElement OptionsContent { get { return (FrameworkElement)GetValue(OptionsContentProperty); } set { SetValue(OptionsContentProperty, value); } }
		public DemoModule DemoModule { get { return (DemoModule)Parent; } }
		public FrameworkElement ContentContainer { get; private set; }
		public FrameworkElement OptionsContentContainer { get; private set; }
		public FrameworkElement DemoContentElement { get; private set; }
		public FrameworkElement DescriptionContentContainer { get; private set; }
		internal CornerPopup CornerPopup { get; private set; }
		internal CodeViewControl CodeTextViewer { get; private set; }
		public FrameworkElement CodeOwner { get; private set; }
		public event RoutedEventHandler BeforeModuleDisappear { add { this.AddHandler(BeforeModuleDisappearEvent, value); } remove { this.RemoveHandler(BeforeModuleDisappearEvent, value); } }
		public bool SupressGroupFramePaddingInOptions { get { return supressGroupFramePaddingInOptionsValue; } set { SetValue(SupressGroupFramePaddingInOptionsProperty, value); } }
		public DependencyObject Sidebar2Content {
			get { return (DependencyObject)GetValue(Sidebar2ContentProperty); }
			set { SetValue(Sidebar2ContentProperty, value); }
		}
		public static readonly DependencyProperty Sidebar2ContentProperty =
			DependencyProperty.Register("Sidebar2Content", typeof(DependencyObject), typeof(DemoModuleControl), new PropertyMetadata(null));
		public DependencyObject SidebarContent {
			get { return (DependencyObject)GetValue(SidebarContentProperty); }
			set { SetValue(SidebarContentProperty, value); }
		}
		public static readonly DependencyProperty SidebarContentProperty =
			DependencyProperty.Register("SidebarContent", typeof(DependencyObject), typeof(DemoModuleControl), new PropertyMetadata(null));
		protected internal virtual void RaiseBeforeModuleDisappear() {
			this.RaiseEvent(new RoutedEventArgs() { RoutedEvent = DemoModuleControl.BeforeModuleDisappearEvent });
		}
		void UpdateOptionsContentDataContext(object dataContext) {
			if(OptionsContent != null) {
				if(dataContext == null)
					OptionsContent.SetBinding(FrameworkElement.DataContextProperty, new Binding("DataContext") { Source = this, Mode = BindingMode.OneWay });
				else
					OptionsContent.DataContext = dataContext;
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateOptionsContentDataContext(DemoModule.GetModuleDataContext());
			ContentContainer = (FrameworkElement)GetTemplateChild("ContentContainer");
			DemoContentElement = (FrameworkElement)GetTemplateChild("DemoContentElement");
			OptionsContentContainer = (FrameworkElement)GetTemplateChild("OptionsContentContainer");
			CornerPopup = (CornerPopup)GetTemplateChild("CornerPopup");
			CodeTextViewer = (CodeViewControl)GetTemplateChild("CodeTextViewer");
			DescriptionContentContainer = (FrameworkElement)GetTemplateChild("DescriptionContentContainer");
			CodeOwner = (FrameworkElement)GetTemplateChild("CodeOwner");
		}
		protected override Size MeasureOverride(Size constraint) {
			constraint = base.MeasureOverride(constraint);
			ClearAutomationEventsHelper.ClearAutomationEvents();
			return constraint;
		}
	}
	class CodeFileAttributeParser {
		public static List<string> GetCodeFiles(Type type, string xamlSuffix, string modulesFolder) {
			List<string> files = type.GetCustomAttributes(typeof(CodeFileAttribute), true).OfType<CodeFileAttribute>().OrderBy(a => a.Path).Select(a => a.Path).ToList();
			files.AddRange(type.GetCustomAttributes(typeof(CodeFilesAttribute), true).OfType<CodeFilesAttribute>().SelectMany(a => a.Paths));
			string xamlName = type.Name + xamlSuffix;
			string csName = type.Name + ".xaml.(cs)";
			var noAutogenerated =
				type.GetCustomAttributes(typeof (NoAutogeneratedCodeFilesAttribute), true)
					.OfType<NoAutogeneratedCodeFilesAttribute>()
					.FirstOrDefault();
			if(noAutogenerated == null) {
				if(!files.Any(f => f.EndsWith(csName))) {
					files.Insert(0, AssemblyHelper.CombineUri(modulesFolder, csName));
				}
				if(!files.Any(f => f.EndsWith(xamlName))) {
					files.Insert(0, AssemblyHelper.CombineUri(modulesFolder, xamlName));
				}
			}
			return files;
		}
	}
	class RightExpanderContentTemplateSelector : DataTemplateSelector {
		public DataTemplate OptionsTemplate { get; set; }
		public DataTemplate SidebarTemplate { get; set; }
		public DataTemplate Sidebar2Template { get; set; }
		public DataTemplate AboutTemplate { get; set; }
		public DataTemplate ThemesTemplate { get; set; }
		public DataTemplate TouchThemesTemplate { get; set; }
		public DataTemplate NoneTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			switch((ToolbarSidebarView)item) {
				case ToolbarSidebarView.Options: return OptionsTemplate;
				case ToolbarSidebarView.Custom: return SidebarTemplate;
				case ToolbarSidebarView.Custom2: return Sidebar2Template;
				case ToolbarSidebarView.About: return AboutTemplate;
				case ToolbarSidebarView.ClassicThemes: return ThemesTemplate;
				case ToolbarSidebarView.TouchThemes: return TouchThemesTemplate;
				default: return NoneTemplate;
			}
		}
	}
}
