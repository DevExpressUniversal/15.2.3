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
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	[ContentProperty("Menu")]
	public class MenuBarControl : Control {
		#region static
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty MenuInfoProperty;
		public static readonly DependencyProperty ShowGlyphSideBackgroundProperty;
		public static readonly DependencyProperty ShowContentSideBackgroundProperty;
		public static readonly DependencyProperty MenuProperty;
		static MenuBarControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuBarControl), new FrameworkPropertyMetadata(typeof(MenuBarControl)));
			MenuProperty = DependencyProperty.Register("Menu", typeof(PopupMenu), typeof(MenuBarControl), new PropertyMetadata(null, (d, e) => ((MenuBarControl)d).OnMenuChanged((PopupMenu)e.OldValue, (PopupMenu)e.NewValue)));
			ShowGlyphSideBackgroundProperty = DependencyProperty.Register("ShowGlyphSideBackground", typeof(bool), typeof(MenuBarControl), new PropertyMetadata(true, new PropertyChangedCallback(OnShowGlyphSideBackgroundPropertyChanged)));
			ShowContentSideBackgroundProperty = DependencyProperty.Register("ShowContentSideBackground", typeof(bool), typeof(MenuBarControl), new PropertyMetadata(true, new PropertyChangedCallback(OnShowContentSideBackgroundPropertyChanged)));
		}
		protected static void OnShowGlyphSideBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((MenuBarControl)d).OnShowGlyphSideBackgroundChanged(e);
		}
		protected static void OnShowContentSideBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((MenuBarControl)d).OnShowContentSideBackgroundChanged(e);
		}
		protected static void OnMenuInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((MenuBarControl)d).OnMenuInfoChanged(e);
		}
		#endregion
		public MenuBarControl() { 
		}
		[Obsolete("MenuInfo property is no longer needed. Please replace MenuInfo with Menu property.", false)]
		public PopupMenuBarInfo MenuInfo {
			get;
			set;
		}
		public PopupMenu Menu {
			get { return (PopupMenu)GetValue(MenuProperty); }
			set { this.SetValue(MenuProperty, value); }
		}
		protected DXContentPresenter LinksPresenter { get; private set; }
		protected virtual void OnMenuChanged(object oldValue, object newValue) {
			oldValue.Do(RemoveLogicalChild);
			UpdatePopup();
			newValue.Do(AddLogicalChild);
		}
		protected virtual void OnMenuInfoChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void UpdatePopup() {
			if(Menu == null) return;
			if(LinksPresenter != null)
				LinksPresenter.Content = Menu.PopupContent;
			UpdatePopupBackgroundVisibility();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			LinksPresenter = (DXContentPresenter)GetTemplateChild("PART_LinksPresenter");
			UpdatePopup();
		}
		public bool ShowGlyphSideBackground {
			get { return (bool)GetValue(ShowGlyphSideBackgroundProperty); }
			set { SetValue(ShowGlyphSideBackgroundProperty, value); }
		}
		public bool ShowContentSideBackground {
			get { return (bool)GetValue(ShowContentSideBackgroundProperty); }
			set { SetValue(ShowContentSideBackgroundProperty, value); }
		}
		protected virtual void UpdatePopupBackgroundVisibility() {
			if(Menu != null) {
				((SubMenuBarControl)Menu.PopupContent).ContentSideVisibility = ShowContentSideBackground ? Visibility.Visible : Visibility.Collapsed;
				((SubMenuBarControl)Menu.PopupContent).GlyphSideVisibility = ShowGlyphSideBackground ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		protected virtual void OnShowContentSideBackgroundChanged(DependencyPropertyChangedEventArgs e) {
			UpdatePopupBackgroundVisibility();
		}
		protected virtual void OnShowGlyphSideBackgroundChanged(DependencyPropertyChangedEventArgs e) {
			UpdatePopupBackgroundVisibility();
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				return new MergedEnumerator(base.LogicalChildren, new SingleLogicalChildEnumerator(Menu));
			}
		}
	}
}
