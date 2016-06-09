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
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using XpfRoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using XpfRoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using XpfRoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
using System.Collections.Generic;
#else
using XpfRoutedEvent = System.Windows.RoutedEvent;
using XpfRoutedEventArgs = System.Windows.RoutedEventArgs;
using XpfRoutedEventHandler = System.Windows.RoutedEventHandler;
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Core.Native {
	public class SearchPanel : Control, ILogicalOwner {
		#region Fields
		public static readonly DependencyProperty ReplaceButtonTextProperty = DependencyPropertyManager.Register("ReplaceButtonText", typeof(string), typeof(SearchPanel), new FrameworkPropertyMetadata(String.Empty));
		public static readonly DependencyProperty ReplaceAllButtonTextProperty = DependencyPropertyManager.Register("ReplaceAllButtonText", typeof(string), typeof(SearchPanel), new FrameworkPropertyMetadata(String.Empty));
		public static readonly DependencyProperty FindLabelTextProperty = DependencyPropertyManager.Register("FindLabelText", typeof(string), typeof(SearchPanel), new FrameworkPropertyMetadata(String.Empty));
		public static readonly DependencyProperty ReplaceLabelTextProperty = DependencyPropertyManager.Register("ReplaceLabelText", typeof(string), typeof(SearchPanel), new FrameworkPropertyMetadata(String.Empty));
		public static readonly DependencyProperty CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), typeof(SearchPanel), new FrameworkPropertyMetadata(null));
#if !SL
		public static readonly DependencyProperty FindNextButtonTooltipProperty = DependencyPropertyManager.Register("FindNextButtonTooltip", typeof(string), typeof(SearchPanel), new FrameworkPropertyMetadata(String.Empty));
		public static readonly DependencyProperty FindPrevButtonTooltipProperty = DependencyPropertyManager.Register("FindPrevButtonTooltip", typeof(string), typeof(SearchPanel), new FrameworkPropertyMetadata(String.Empty));
		public static readonly DependencyProperty CloseButtonTooltipProperty = DependencyPropertyManager.Register("CloseButtonTooltip", typeof(string), typeof(SearchPanel), new FrameworkPropertyMetadata(String.Empty));
		public static readonly DependencyProperty SearchOptionsButtonTooltipProperty = DependencyPropertyManager.Register("SearchOptionsButtonTooltip", typeof(string), typeof(SearchPanel), new FrameworkPropertyMetadata(String.Empty));
#endif
		public static readonly XpfRoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Direct, typeof(XpfRoutedEventHandler), typeof(SearchPanel));
		#endregion
		#region Constructors
		public SearchPanel()
			: this(null) {
		}
		public SearchPanel(ISearchPanelViewModel viewModel) {
			ViewModel = viewModel;
			DefaultStyleKey = typeof(SearchPanel);
			ReplaceButtonText = SearchPanelLocalizer.GetString(SearchPanelStringId.ButtonText_Replace);
			ReplaceAllButtonText = SearchPanelLocalizer.GetString(SearchPanelStringId.ButtonText_ReplaceAll);
			FindLabelText = SearchPanelLocalizer.GetString(SearchPanelStringId.LabelText_Find);
			ReplaceLabelText = SearchPanelLocalizer.GetString(SearchPanelStringId.LabelText_Replace);
#if !SL
			FindPrevButtonTooltip = SearchPanelLocalizer.GetString(SearchPanelStringId.ButtonTooltip_FindPrev);
			FindNextButtonTooltip = SearchPanelLocalizer.GetString(SearchPanelStringId.ButtonTooltip_FindNext);
			CloseButtonTooltip = SearchPanelLocalizer.GetString(SearchPanelStringId.ButtonTooltip_Close);
			SearchOptionsButtonTooltip = SearchPanelLocalizer.GetString(SearchPanelStringId.ButtonTooltip_SearchOptions);
#endif
		}
		#endregion
		#region Properties
		public ISearchPanelViewModel ViewModel {
			get { return DataContext as ISearchPanelViewModel; }
			set {
				DataContext = value;
				ApplyTemplate();
			}
		}
		public string ReplaceButtonText {
			get { return (string)GetValue(ReplaceButtonTextProperty); }
			set { SetValue(ReplaceButtonTextProperty, value); }
		}
		public string ReplaceAllButtonText {
			get { return (string)GetValue(ReplaceAllButtonTextProperty); }
			set { SetValue(ReplaceAllButtonTextProperty, value); }
		}
		public string FindLabelText {
			get { return (string)GetValue(FindLabelTextProperty); }
			set { SetValue(FindLabelTextProperty, value); }
		}
		public string ReplaceLabelText {
			get { return (string)GetValue(ReplaceLabelTextProperty); }
			set { SetValue(ReplaceLabelTextProperty, value); }
		}
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
#if !SL
		public string FindNextButtonTooltip {
			get { return (string)GetValue(FindNextButtonTooltipProperty); }
			set { SetValue(FindNextButtonTooltipProperty, value); }
		}
		public string FindPrevButtonTooltip {
			get { return (string)GetValue(FindPrevButtonTooltipProperty); }
			set { SetValue(FindPrevButtonTooltipProperty, value); }
		}
		public string CloseButtonTooltip {
			get { return (string)GetValue(CloseButtonTooltipProperty); }
			set { SetValue(CloseButtonTooltipProperty, value); }
		}
		public string SearchOptionsButtonTooltip {
			get { return (string)GetValue(SearchOptionsButtonTooltipProperty); }
			set { SetValue(SearchOptionsButtonTooltipProperty, value); }
		}
#endif
		protected internal ButtonEdit SearchStringEdit { get { return GetTemplateChild("tbSearchString") as ButtonEdit; } }
		protected internal ButtonEdit ReplaceStringEdit { get { return GetTemplateChild("tbReplaceString") as ButtonEdit; } }
		protected internal Button FindNextButton { get { return GetTemplateChild("FindNextButton") as Button; } }
		protected internal Button FindPrevButton { get { return GetTemplateChild("FindPrevButton") as Button; } }
		protected internal Button ReplaceButton { get { return GetTemplateChild("ReplaceButton") as Button; } }
		protected internal Button ReplaceAllButton { get { return GetTemplateChild("ReplaceAllButton") as Button; } }
		protected internal FrameworkElement ReplaceBox { get { return GetTemplateChild("ReplaceBox") as FrameworkElement; } }
		protected internal FrameworkElement ReplaceButtons { get { return GetTemplateChild("ReplaceButtons") as FrameworkElement; } }
		protected internal Button SearchClose { get { ApplyTemplate(); return GetTemplateChild("SearchClose") as Button; } }
		protected internal Button SearchOptionsButton { get { return GetTemplateChild("SearchOptionsButton") as Button; } }
		#endregion
		#region Events
		public event XpfRoutedEventHandler Closed {
			add { this.AddHandler(ClosedEvent, value); }
			remove { this.RemoveHandler(ClosedEvent, value); }
		}
		#endregion
		#region Methods
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(SearchClose != null)
				SearchClose.Click += OnClose;
			if(SearchStringEdit != null) {
				SearchStringEdit.KeyDown += OnSearchStringKeyDown;
			}
			if(ReplaceStringEdit != null)
				ReplaceStringEdit.KeyDown += OnReplaceStringKeyDown;
			if(SearchOptionsButton != null)
				SearchOptionsButton.Click += SearchOptionsButton_Click;
		}
		protected internal virtual void OnClose(object sender, RoutedEventArgs e) {
			RaiseCloseEvent();
		}
		protected internal virtual void RaiseCloseEvent() {
			this.RaiseEvent(new XpfRoutedEventArgs(ClosedEvent));
		}
		protected internal virtual void OnReplaceStringKeyDown(object sender, KeyEventArgs e) {
			if(e.Key == Key.Enter) {
				ExecuteReplaceForwardCommand();
#if SL
				ReplaceStringEdit.Focus();
#else
				FocusManager.SetFocusedElement(this, ReplaceStringEdit);
#endif
				e.Handled = true;
			}
			if(e.Key == Key.Escape) {
				CloseSearchPanel();
				RaiseCloseEvent();
				e.Handled = true;
			}
			if(e.Key == Key.Tab) {
				if(SearchStringEdit != null) {
#if SL
				SearchStringEdit.Focus();
#else
				FocusManager.SetFocusedElement(this, SearchStringEdit);
#endif
				}
				e.Handled = true;
			}
		}
		protected internal virtual void OnSearchStringKeyDown(object sender, KeyEventArgs e) {
			if(e.Key == Key.Enter) {
				ExecuteFindForwardCommand();
				SearchStringEdit.Focus();
				e.Handled = true;
			}
			if(e.Key == Key.Escape) {
				CloseSearchPanel();
				RaiseCloseEvent();
				e.Handled = true;
			}
			if(e.Key == Key.Tab) {
				if(ReplaceStringEdit!=null && ReplaceStringEdit.Visibility == Visibility.Visible)
					ReplaceStringEdit.Focus();
				e.Handled = true;
			}
		}
		void CloseSearchPanel() {
			if(ViewModel == null || ViewModel.CloseCommand == null)
				return;
			if(ViewModel.CloseCommand.CanExecute(null))
				ViewModel.CloseCommand.Execute(null);
		}
		void SearchOptionsButton_Click(object sender, RoutedEventArgs e) {
			OnSearchOptionsButtonClick(sender as UIElement);
		}
		protected virtual void OnSearchOptionsButtonClick(UIElement control) {
			SearchPanelOptionsPopupMenu menu = new SearchPanelOptionsPopupMenu(this);
			ShowPopupMenu(control, menu);
		}
		protected static void ShowPopupMenu(UIElement control, PopupMenu menu) {
			if(control != null)
				menu.ShowPopup(control);
		}
		protected internal void OnCaseSensitiveCheckedChanged(object sender, ItemClickEventArgs e) {
			if(ViewModel == null)
				return;
			BarCheckItem item = (BarCheckItem)e.Item;
			if(item.IsChecked.HasValue && item.IsChecked.Value)
				ViewModel.CaseSensitive = true;
			else
				ViewModel.CaseSensitive = false;
		}
		protected internal void OnWholeWordCheckedChanged(object sender, ItemClickEventArgs e) {
			if(ViewModel == null)
				return;
			BarCheckItem item = (BarCheckItem)e.Item;
			if(item.IsChecked.HasValue && item.IsChecked.Value)
				ViewModel.FindWholeWord = true;
			else
				ViewModel.FindWholeWord = false;
		}
		protected internal void OnRegularExpressionCheckedChanged(object sender, ItemClickEventArgs e) {
			if(ViewModel == null)
				return;
			BarCheckItem item = (BarCheckItem)e.Item;
			if(item.IsChecked.HasValue && item.IsChecked.Value)
				ViewModel.UseRegularExpression = true;
			else
				ViewModel.UseRegularExpression = false;
			OnRegularExpressionCheckedChanged(item.IsChecked);
		}
		protected virtual void OnRegularExpressionCheckedChanged(bool? IsChecked) {
		}
		protected virtual void ExecuteFindForwardCommand() {
			if(ViewModel == null || ViewModel.FindForwardCommand == null)
				return;
			if(ViewModel.FindForwardCommand.CanExecute(CommandParameter))
				ViewModel.FindForwardCommand.Execute(CommandParameter);
		}
		protected virtual void ExecuteReplaceForwardCommand() {
			if(ViewModel == null || ViewModel.ReplaceForwardCommand == null)
				return;
			if(ViewModel.ReplaceForwardCommand.CanExecute(CommandParameter))
				ViewModel.ReplaceForwardCommand.Execute(CommandParameter);
		}
		#endregion
		#region ILogicalOwner Members
#if SL
		readonly List<object> logicalChildren = new List<object>();
		void ILogicalOwner.AddChild(object child) {
			logicalChildren.Add(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			logicalChildren.Remove(child);
		}
		bool ILogicalOwner.IsLoaded { get { return true; } }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewGotKeyboardFocus { add { } remove { } }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewLostKeyboardFocus { add { } remove { } }
#else
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveLogicalChild(child);
		}
#endif
		#endregion
	}
}
