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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public enum ToolbarView {
		Demo, Code
	}
	public enum ToolbarSidebarView {
		ClassicThemes,
		TouchThemes,
		Options,
		About,
		Custom,
		Custom2,
		None
	}
	public enum ToolbarThemesMode {
		All, ClassicOnly, None
	}
	public class Toolbar : Control {
		public ICommand PreviousCommand {
			get { return (ICommand)GetValue(PreviousCommandProperty); }
			set { SetValue(PreviousCommandProperty, value); }
		}
		public static readonly DependencyProperty PreviousCommandProperty =
			DependencyProperty.Register("PreviousCommand", typeof(ICommand), typeof(Toolbar), new PropertyMetadata(null));
		public ICommand NextCommand {
			get { return (ICommand)GetValue(NextCommandProperty); }
			set { SetValue(NextCommandProperty, value); }
		}
		public static readonly DependencyProperty NextCommandProperty =
			DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(Toolbar), new PropertyMetadata(null));
		public ICommand OpenCSSolutionCommand {
			get { return (ICommand)GetValue(OpenCSSolutionCommandProperty); }
			set { SetValue(OpenCSSolutionCommandProperty, value); }
		}
		public static readonly DependencyProperty OpenCSSolutionCommandProperty =
			DependencyProperty.Register("OpenCSSolutionCommand", typeof(ICommand), typeof(Toolbar), new PropertyMetadata(null));
		public ToolbarView View {
			get { return (ToolbarView)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public ICommand OpenVBSolutionCommand {
			get { return (ICommand)GetValue(OpenVBSolutionCommandProperty); }
			set { SetValue(OpenVBSolutionCommandProperty, value); }
		}
		public static readonly DependencyProperty OpenVBSolutionCommandProperty =
			DependencyProperty.Register("OpenVBSolutionCommand", typeof(ICommand), typeof(Toolbar), new PropertyMetadata(null));
		public static readonly DependencyProperty ViewProperty =
			DependencyProperty.Register("View", typeof(ToolbarView), typeof(Toolbar), new PropertyMetadata(ToolbarView.Demo,
				(d, e) => ((Toolbar)d).OnViewChanged((ToolbarView)e.OldValue)));
		public ToolbarSidebarView SidebarView {
			get { return (ToolbarSidebarView)GetValue(SidebarViewProperty); }
			set { SetValue(SidebarViewProperty, value); }
		}
		public static readonly DependencyProperty SidebarViewProperty =
			DependencyProperty.Register("SidebarView", typeof(ToolbarSidebarView), typeof(Toolbar), new PropertyMetadata(ToolbarSidebarView.Options,
				(d, e) => ((Toolbar)d).OnSidebarViewChanged((ToolbarSidebarView)e.OldValue)));
		public bool AllowRtl {
			get { return (bool)GetValue(AllowRtlProperty); }
			set { SetValue(AllowRtlProperty, value); }
		}
		public static readonly DependencyProperty AllowRtlProperty =
			DependencyProperty.Register("AllowRtl", typeof(bool), typeof(Toolbar), new PropertyMetadata(true,
				(d, e) => ((Toolbar)d).OnAllowRtlChanged((bool)e.OldValue)));
		public ToolbarThemesMode ThemesMode {
			get { return (ToolbarThemesMode)GetValue(ThemesModeProperty); }
			set { SetValue(ThemesModeProperty, value); }
		}
		public static readonly DependencyProperty ThemesModeProperty =
			DependencyProperty.Register("ThemesMode", typeof(ToolbarThemesMode), typeof(Toolbar), new PropertyMetadata(ToolbarThemesMode.All,
				(d, e) => ((Toolbar)d).OnThemesModeChanged((ToolbarThemesMode)e.OldValue)));
		public bool IsCustomSidebarEnabled {
			get { return (bool)GetValue(IsCustomSidebarEnabledProperty); }
			set { SetValue(IsCustomSidebarEnabledProperty, value); }
		}
		public static readonly DependencyProperty IsCustomSidebarEnabledProperty =
			DependencyProperty.Register("IsCustomSidebarEnabled", typeof(bool), typeof(Toolbar), new PropertyMetadata(default(bool),
				(d, e) => ((Toolbar)d).OnIsCustomSidebarEnabledChanged((bool)e.OldValue)));
		public bool IsCustomSidebar2Enabled {
			get { return (bool)GetValue(IsCustomSidebar2EnabledProperty); }
			set { SetValue(IsCustomSidebar2EnabledProperty, value); }
		}
		public static readonly DependencyProperty IsCustomSidebar2EnabledProperty =
			DependencyProperty.Register("IsCustomSidebar2Enabled", typeof(bool), typeof(Toolbar), new PropertyMetadata(default(bool),
				(d, e) => ((Toolbar)d).OnIsCustomSidebar2EnabledChanged((bool)e.OldValue)));
		public FlowDirection Layout {
			get { return (FlowDirection)GetValue(LayoutProperty); }
			set { SetValue(LayoutProperty, value); }
		}
		public static readonly DependencyProperty LayoutProperty =
			DependencyProperty.Register("Layout", typeof(FlowDirection), typeof(Toolbar), new PropertyMetadata(FlowDirection.LeftToRight));
		public bool IsNavigationEnabled {
			get { return (bool)GetValue(IsNavigationEnabledProperty); }
			set { SetValue(IsNavigationEnabledProperty, value); }
		}
		public static readonly DependencyProperty IsNavigationEnabledProperty =
			DependencyProperty.Register("IsNavigationEnabled", typeof(bool), typeof(Toolbar), new PropertyMetadata(true));
		public bool HasOptions {
			get { return (bool)GetValue(HasOptionsProperty); }
			set { SetValue(HasOptionsProperty, value); }
		}
		public static readonly DependencyProperty HasOptionsProperty =
			DependencyProperty.Register("HasOptions", typeof(bool), typeof(Toolbar), new PropertyMetadata(true,
				(d, e) => ((Toolbar)d).OnHasOptionsChanged((bool)e.OldValue)));
		public bool IsCustomSidebarButtonVisible {
			get { return (bool)GetValue(IsCustomSidebarButtonVisibleProperty); }
			set { SetValue(IsCustomSidebarButtonVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsCustomSidebarButtonVisibleProperty =
			DependencyProperty.Register("IsCustomSidebarButtonVisible", typeof(bool), typeof(Toolbar), new PropertyMetadata(default(bool),
				(d, e) => ((Toolbar)d).OnIsCustomSidebarButtonVisibleChanged((bool)e.OldValue)));
		public bool IsCustomSidebar2ButtonVisible {
			get { return (bool)GetValue(IsCustomSidebar2ButtonVisibleProperty); }
			set { SetValue(IsCustomSidebar2ButtonVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsCustomSidebar2ButtonVisibleProperty =
			DependencyProperty.Register("IsCustomSidebar2ButtonVisible", typeof(bool), typeof(Toolbar), new PropertyMetadata(default(bool),
				(d, e) => ((Toolbar)d).OnIsCustomSidebar2ButtonVisibleChanged((bool)e.OldValue)));
		public ImageSource CustomSidebarButtonIcon {
			get { return (ImageSource)GetValue(CustomSidebarButtonIconProperty); }
			set { SetValue(CustomSidebarButtonIconProperty, value); }
		}
		public static readonly DependencyProperty CustomSidebarButtonIconProperty =
			DependencyProperty.Register("CustomSidebarButtonIcon", typeof(ImageSource), typeof(Toolbar), new PropertyMetadata(default(ImageSource)));
		public ImageSource CustomSidebar2ButtonIcon {
			get { return (ImageSource)GetValue(CustomSidebar2ButtonIconProperty); }
			set { SetValue(CustomSidebar2ButtonIconProperty, value); }
		}
		public static readonly DependencyProperty CustomSidebar2ButtonIconProperty =
			DependencyProperty.Register("CustomSidebar2ButtonIcon", typeof(ImageSource), typeof(Toolbar), new PropertyMetadata(default(ImageSource)));
		public ImageSource CustomSidebarButtonHoveredIcon {
			get { return (ImageSource)GetValue(CustomSidebarButtonHoveredIconProperty); }
			set { SetValue(CustomSidebarButtonHoveredIconProperty, value); }
		}
		public static readonly DependencyProperty CustomSidebarButtonHoveredIconProperty =
			DependencyProperty.Register("CustomSidebarButtonHoveredIcon", typeof(ImageSource), typeof(Toolbar), new PropertyMetadata(default(ImageSource)));
		public ImageSource CustomSidebar2ButtonHoveredIcon {
			get { return (ImageSource)GetValue(CustomSidebar2ButtonHoveredIconProperty); }
			set { SetValue(CustomSidebar2ButtonHoveredIconProperty, value); }
		}
		public static readonly DependencyProperty CustomSidebar2ButtonHoveredIconProperty =
			DependencyProperty.Register("CustomSidebar2ButtonHoveredIcon", typeof(ImageSource), typeof(Toolbar), new PropertyMetadata(default(ImageSource)));
		public string CustomSidebarButtonLabel {
			get { return (string)GetValue(CustomSidebarButtonLabelProperty); }
			set { SetValue(CustomSidebarButtonLabelProperty, value); }
		}
		public static readonly DependencyProperty CustomSidebarButtonLabelProperty =
			DependencyProperty.Register("CustomSidebarButtonLabel", typeof(string), typeof(Toolbar), new PropertyMetadata(default(string)));
		public string CustomSidebar2ButtonLabel {
			get { return (string)GetValue(CustomSidebar2ButtonLabelProperty); }
			set { SetValue(CustomSidebar2ButtonLabelProperty, value); }
		}
		public static readonly DependencyProperty CustomSidebar2ButtonLabelProperty =
			DependencyProperty.Register("CustomSidebar2ButtonLabel", typeof(string), typeof(Toolbar), new PropertyMetadata(default(string)));
		#region internal
		public bool IsDemoButtonChecked {
			get { return (bool)GetValue(IsDemoButtonCheckedProperty); }
			set { SetValue(IsDemoButtonCheckedProperty, value); }
		}
		public static readonly DependencyProperty IsDemoButtonCheckedProperty =
			DependencyProperty.Register("IsDemoButtonChecked", typeof(bool), typeof(Toolbar), new PropertyMetadata(true,
				(d, e) => ((Toolbar)d).OnIsDemoButtonCheckedChanged((bool)e.OldValue)));
		public bool IsCodeButtonChecked {
			get { return (bool)GetValue(IsCodeButtonCheckedProperty); }
			set { SetValue(IsCodeButtonCheckedProperty, value); }
		}
		public static readonly DependencyProperty IsCodeButtonCheckedProperty =
			DependencyProperty.Register("IsCodeButtonChecked", typeof(bool), typeof(Toolbar), new PropertyMetadata(default(bool),
				(d, e) => ((Toolbar)d).OnIsCodeButtonCheckedChanged((bool)e.OldValue)));
		public bool IsLTRButtonChecked {
			get { return (bool)GetValue(IsLTRButtonCheckedProperty); }
			set { SetValue(IsLTRButtonCheckedProperty, value); }
		}
		public static readonly DependencyProperty IsLTRButtonCheckedProperty =
			DependencyProperty.Register("IsLTRButtonChecked", typeof(bool), typeof(Toolbar), new PropertyMetadata(true,
				(d, e) => ((Toolbar)d).OnIsLTRButtonCheckedChanged((bool)e.OldValue)));
		public bool IsRTLButtonChecked {
			get { return (bool)GetValue(IsRTLButtonCheckedProperty); }
			set { SetValue(IsRTLButtonCheckedProperty, value); }
		}
		public static readonly DependencyProperty IsRTLButtonCheckedProperty =
			DependencyProperty.Register("IsRTLButtonChecked", typeof(bool), typeof(Toolbar), new PropertyMetadata(default(bool),
				(d, e) => ((Toolbar)d).OnIsRTLButtonCheckedChanged((bool)e.OldValue)));
		public bool IsThemesButtonChecked {
			get { return (bool)GetValue(IsThemesButtonCheckedProperty); }
			set { SetValue(IsThemesButtonCheckedProperty, value); }
		}
		public static readonly DependencyProperty IsThemesButtonCheckedProperty =
			DependencyProperty.Register("IsThemesButtonChecked", typeof(bool), typeof(Toolbar), new PropertyMetadata(false,
				(d, e) => ((Toolbar)d).OnIsThemesButtonCheckedChanged((bool)e.OldValue)));
		public bool IsTouchThemesButtonChecked {
			get { return (bool)GetValue(IsTouchThemesButtonCheckedProperty); }
			set { SetValue(IsTouchThemesButtonCheckedProperty, value); }
		}
		public static readonly DependencyProperty IsTouchThemesButtonCheckedProperty =
			DependencyProperty.Register("IsTouchThemesButtonChecked", typeof(bool), typeof(Toolbar), new PropertyMetadata(false,
				(d, e) => ((Toolbar)d).OnIsTouchThemesButtonCheckedChanged((bool)e.OldValue)));
		public bool IsOptionsButtonChecked {
			get { return (bool)GetValue(IsOptionsButtonCheckedProperty); }
			set { SetValue(IsOptionsButtonCheckedProperty, value); }
		}
		public static readonly DependencyProperty IsOptionsButtonCheckedProperty =
			DependencyProperty.Register("IsOptionsButtonChecked", typeof(bool), typeof(Toolbar), new PropertyMetadata(true,
				(d, e) => ((Toolbar)d).OnIsOptionsButtonCheckedChanged((bool)e.OldValue)));
		public bool IsSidebarButtonChecked {
			get { return (bool)GetValue(IsSidebarButtonCheckedProperty); }
			set { SetValue(IsSidebarButtonCheckedProperty, value); }
		}
		public static readonly DependencyProperty IsSidebarButtonCheckedProperty =
			DependencyProperty.Register("IsSidebarButtonChecked", typeof(bool), typeof(Toolbar), new PropertyMetadata(default(bool),
				(d, e) => ((Toolbar)d).OnIsSidebarButtonCheckedChanged((bool)e.OldValue)));
		public bool IsAboutButtonChecked {
			get { return (bool)GetValue(IsAboutButtonCheckedProperty); }
			set { SetValue(IsAboutButtonCheckedProperty, value); }
		}
		public bool IsSidebar2ButtonChecked {
			get { return (bool)GetValue(IsSidebar2ButtonCheckedProperty); }
			set { SetValue(IsSidebar2ButtonCheckedProperty, value); }
		}
		public static readonly DependencyProperty IsSidebar2ButtonCheckedProperty =
			DependencyProperty.Register("IsSidebar2ButtonChecked", typeof(bool), typeof(Toolbar), new PropertyMetadata(default(bool),
				(d, e) => ((Toolbar)d).OnIsSidebar2ButtonCheckedChanged((bool)e.OldValue)));
		public static readonly DependencyProperty IsAboutButtonCheckedProperty =
			DependencyProperty.Register("IsAboutButtonChecked", typeof(bool), typeof(Toolbar), new PropertyMetadata(default(bool),
				(d, e) => ((Toolbar)d).OnIsAboutButtonCheckedChanged((bool)e.OldValue)));
		public bool IsThemesButtonEnabled {
			get { return (bool)GetValue(IsThemesButtonEnabledProperty); }
			set { SetValue(IsThemesButtonEnabledProperty, value); }
		}
		public static readonly DependencyProperty IsThemesButtonEnabledProperty =
			DependencyProperty.Register("IsThemesButtonEnabled", typeof(bool), typeof(Toolbar), new PropertyMetadata(default(bool)));
		public bool IsTouchThemesButtonEnabled {
			get { return (bool)GetValue(IsTouchThemesButtonEnabledProperty); }
			set { SetValue(IsTouchThemesButtonEnabledProperty, value); }
		}
		public static readonly DependencyProperty IsTouchThemesButtonEnabledProperty =
			DependencyProperty.Register("IsTouchThemesButtonEnabled", typeof(bool), typeof(Toolbar), new PropertyMetadata(default(bool)));
		public bool IsLayoutGroupVisible {
			get { return (bool)GetValue(IsLayoutGroupVisibleProperty); }
			set { SetValue(IsLayoutGroupVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsLayoutGroupVisibleProperty =
			DependencyProperty.Register("IsLayoutGroupVisible", typeof(bool), typeof(Toolbar), new PropertyMetadata(true));
		public bool IsSolutionGroupVisible {
			get { return (bool)GetValue(IsSolutionGroupVisibleProperty); }
			set { SetValue(IsSolutionGroupVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsSolutionGroupVisibleProperty =
			DependencyProperty.Register("IsSolutionGroupVisible", typeof(bool), typeof(Toolbar), new PropertyMetadata(false));
		#endregion
		void UpdateSidebar(DependencyProperty updated, ToolbarSidebarView sidebar) {
			if(!(bool)GetValue(updated)) {
				updated = null;
				sidebar = ToolbarSidebarView.None;
			}
			var all = new[] {
				IsThemesButtonCheckedProperty,
				IsTouchThemesButtonCheckedProperty,
				IsOptionsButtonCheckedProperty,
				IsSidebarButtonCheckedProperty,
				IsSidebar2ButtonCheckedProperty,
				IsAboutButtonCheckedProperty
			};
			foreach(var p in all.Except(new[] { updated }))
				SetValue(p, false);
			SidebarView = sidebar;
		}
		void OnIsCodeButtonCheckedChanged(bool oldValue) {
			if(IsCodeButtonChecked) {
				View = ToolbarView.Code;
			}
		}
		void OnIsDemoButtonCheckedChanged(bool oldValue) {
			if(IsDemoButtonChecked) {
				View = ToolbarView.Demo;
			}
		}
		void OnIsLTRButtonCheckedChanged(bool oldValue) {
			if(IsLTRButtonChecked) {
				Layout = FlowDirection.LeftToRight;
			}
		}
		void OnIsRTLButtonCheckedChanged(bool oldValue) {
			if(IsRTLButtonChecked) {
				Layout = FlowDirection.RightToLeft;
			}
		}
		void OnIsAboutButtonCheckedChanged(bool oldValue) {
			UpdateSidebar(IsAboutButtonCheckedProperty, ToolbarSidebarView.About);
		}
		void OnIsSidebarButtonCheckedChanged(bool oldValue) {
			UpdateSidebar(IsSidebarButtonCheckedProperty, ToolbarSidebarView.Custom);
		}
		void OnIsSidebar2ButtonCheckedChanged(bool oldValue) {
			UpdateSidebar(IsSidebar2ButtonCheckedProperty, ToolbarSidebarView.Custom2);
		}
		void OnIsOptionsButtonCheckedChanged(bool oldValue) {
			UpdateSidebar(IsOptionsButtonCheckedProperty, ToolbarSidebarView.Options);
		}
		void OnIsTouchThemesButtonCheckedChanged(bool oldValue) {
			UpdateSidebar(IsTouchThemesButtonCheckedProperty, ToolbarSidebarView.TouchThemes);
		}
		void OnIsThemesButtonCheckedChanged(bool oldValue) {
			UpdateSidebar(IsThemesButtonCheckedProperty, ToolbarSidebarView.ClassicThemes);
		}
		void OnThemesModeChanged(ToolbarThemesMode oldValue) {
			if(ThemesMode == ToolbarThemesMode.ClassicOnly) {
				IsTouchThemesButtonChecked = false;
				IsTouchThemesButtonEnabled = false;
			}
			if(ThemesMode == ToolbarThemesMode.None) {
				IsTouchThemesButtonChecked = false;
				IsThemesButtonChecked = false;
				IsTouchThemesButtonEnabled = false;
				IsThemesButtonEnabled = false;
			}
		}
		void OnAllowRtlChanged(bool oldValue) {
			if(!AllowRtl) {
				IsLTRButtonChecked = true;
			}
		}
		void OnHasOptionsChanged(bool oldValue) {
			if(!HasOptions) {
				IsOptionsButtonChecked = false;
			} else if (SidebarView == ToolbarSidebarView.None) {
				IsOptionsButtonChecked = true;
			}
		}
		void OnSidebarViewChanged(ToolbarSidebarView oldValue) {
			switch(SidebarView) {
				case ToolbarSidebarView.About: IsAboutButtonChecked = true; break;
				case ToolbarSidebarView.ClassicThemes: IsThemesButtonChecked = true; break;
				case ToolbarSidebarView.Custom: IsSidebarButtonChecked = true; break;
				case ToolbarSidebarView.Custom2: IsSidebar2ButtonChecked = true; break;
				case ToolbarSidebarView.Options: IsOptionsButtonChecked = true; break;
				case ToolbarSidebarView.TouchThemes: IsTouchThemesButtonChecked = true; break;
			}
		}
		void OnViewChanged(ToolbarView oldValue) {
			switch(View) {
				case ToolbarView.Code: IsCodeButtonChecked = true; break;
				case ToolbarView.Demo: IsDemoButtonChecked = true; break;
			}
		}
		void OnIsCustomSidebarEnabledChanged(bool oldValue) {
			if(!IsCustomSidebarEnabled) {
				DisableCustomSidebarIfNeeded();
			}
		}
		void OnIsCustomSidebarButtonVisibleChanged(bool oldValue) {
			if(!IsCustomSidebarButtonVisible) {
				DisableCustomSidebarIfNeeded();
			}
		}
		void DisableCustomSidebarIfNeeded() {
			if (SidebarView == ToolbarSidebarView.Custom) {
				SidebarView = HasOptions ? ToolbarSidebarView.Options : ToolbarSidebarView.None;
			}
		}
		void OnIsCustomSidebar2EnabledChanged(bool oldValue) {
			if(!IsCustomSidebar2Enabled) {
				DisableCustomSidebar2IfNeeded();
			}
		}
		void OnIsCustomSidebar2ButtonVisibleChanged(bool oldValue) {
			if(!IsCustomSidebar2ButtonVisible) {
				DisableCustomSidebar2IfNeeded();
			}
		}
		void DisableCustomSidebar2IfNeeded() {
			if(SidebarView == ToolbarSidebarView.Custom2) {
				SidebarView = HasOptions ? ToolbarSidebarView.Options : ToolbarSidebarView.None;
			}
		}
		static Toolbar() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Toolbar), new FrameworkPropertyMetadata(typeof(Toolbar)));
		}
	}
}
