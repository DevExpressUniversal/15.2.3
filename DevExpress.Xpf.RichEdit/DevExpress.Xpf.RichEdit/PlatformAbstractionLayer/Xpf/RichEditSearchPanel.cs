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
using System.Windows;
using DevExpress.XtraRichEdit.Menu;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.RichEdit.UI;
using DevExpress.Xpf.RichEdit.Menu;
using DevExpress.Xpf.Core.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#else
using DevExpress.Xpf.Utils;
using System.Windows.Input;
#endif
namespace DevExpress.Xpf.RichEdit.UI {
	public class RichEditSearchPanel : SearchPanel {
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(RichEditSearchPanel), new PropertyMetadata(OnRichEditControlChanged));
		#region Constructors
		public RichEditSearchPanel() {
			DefaultStyleKey = typeof(SearchPanel);
		}
		public RichEditSearchPanel(RichEditControl richEditControl) {
			RichEditControl = richEditControl;
		}
		#endregion
		#region Properties
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		#endregion
		#region Methods
		protected static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditSearchPanel instance = d as RichEditSearchPanel;
			if(instance != null)
				instance.OnRichEditControlChanged((RichEditControl)e.OldValue, (RichEditControl)e.NewValue);
		}
		protected internal virtual void OnRichEditControlChanged(RichEditControl oldValue, RichEditControl newValue) {
			OnControlChanged();
			CommandParameter = RichEditControl;
		}
		protected virtual void OnControlChanged() {
		}
		void SearchStringEdit_ButtonClick(object sender, RoutedEventArgs e) {
			RichEditSearchPanelPopupMenu menu = new RichEditSearchPanelPopupMenu(RichEditControl);
			SearchRegexPopupMenuBuilder builder = new SearchRegexPopupMenuBuilder(RichEditControl, new XpfRichEditMenuBuilderUIFactory(), new ButtonEditAdapter(SearchStringEdit));
			RichEditMenuBuilderInfo builderInfo = (RichEditMenuBuilderInfo)builder.CreatePopupMenu();
			menu.MenuBuilderInfo = builderInfo;
			ShowPopupMenu(sender as UIElement, menu);
		}
		void ReplaceStringEdit_ButtonClick(object sender, RoutedEventArgs e) {
			RichEditSearchPanelPopupMenu menu = new RichEditSearchPanelPopupMenu(RichEditControl);
			ReplaceRegexPopupMenuBuilder builder = new ReplaceRegexPopupMenuBuilder(RichEditControl, new XpfRichEditMenuBuilderUIFactory(), new ButtonEditAdapter(ReplaceStringEdit));
			RichEditMenuBuilderInfo builderInfo = (RichEditMenuBuilderInfo)builder.CreatePopupMenu();
			menu.MenuBuilderInfo = builderInfo;
			ShowPopupMenu(sender as UIElement, menu);
		}
		protected override void OnRegularExpressionCheckedChanged(bool? IsChecked) {
			if(IsChecked.HasValue && IsChecked.Value) {
				if(ViewModel != null)
					ViewModel.UseRegularExpression = true;
				ButtonInfo searchButtonInfo = new ButtonInfo() { GlyphKind = GlyphKind.Right };
				searchButtonInfo.Click += SearchStringEdit_ButtonClick;
				SearchStringEdit.Buttons.Add(searchButtonInfo);
				ButtonInfo replaceButtonInfo = new ButtonInfo() { GlyphKind = GlyphKind.Right };
				replaceButtonInfo.Click += ReplaceStringEdit_ButtonClick;
				ReplaceStringEdit.Buttons.Add(replaceButtonInfo);
			} else {
				if(ViewModel != null)
					ViewModel.UseRegularExpression = false;
				if(SearchStringEdit.Buttons.Count != 0) {
					((ButtonInfo)SearchStringEdit.Buttons[0]).Click -= SearchStringEdit_ButtonClick;
					SearchStringEdit.Buttons.Clear();
				}
				if(ReplaceStringEdit.Buttons.Count != 0) {
					((ButtonInfo)ReplaceStringEdit.Buttons[0]).Click -= ReplaceStringEdit_ButtonClick;
					ReplaceStringEdit.Buttons.Clear();
				}
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(ViewModel == null)
				return;
			if(ViewModel.ShowUseRegularExpressionOption && ViewModel.UseRegularExpression) {
				if(SearchStringEdit.Buttons.Count == 0) {
					ButtonInfo searchButtonInfo = new ButtonInfo() { GlyphKind = GlyphKind.Right };
					searchButtonInfo.Click += SearchStringEdit_ButtonClick;
					SearchStringEdit.Buttons.Add(searchButtonInfo);
				}
				if(ReplaceStringEdit.Buttons.Count == 0) {
					ButtonInfo replaceButtonInfo = new ButtonInfo() { GlyphKind = GlyphKind.Right };
					replaceButtonInfo.Click += ReplaceStringEdit_ButtonClick;
					ReplaceStringEdit.Buttons.Add(replaceButtonInfo);
				}
			}
		}
		public void SetFocus() {
			if (SearchStringEdit == null)
				return;
#if !SL
			FocusManager.SetFocusedElement(this, SearchStringEdit);
#endif
			SearchStringEdit.Focus();
		}
		#endregion
	}
	class RichEditSearchPanelPopupMenu : RichEditPopupMenu {
		public RichEditSearchPanelPopupMenu(RichEditControl control)
			: base(control) {
		}
		protected override bool RaiseShowMenu() {
			return true;
		}
	}
}
