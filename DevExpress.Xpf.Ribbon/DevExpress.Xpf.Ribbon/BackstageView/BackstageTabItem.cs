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
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.ComponentModel;
using DevExpress.Mvvm.Native;
using System.Collections;
using DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Ribbon {
	public class BackstageTabItem : BackstageItem {
		#region static
		public static readonly DependencyProperty ControlPaneProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		protected internal static readonly DependencyPropertyKey IsSelectedPropertyKey;
		public static readonly DependencyProperty SelectedTextStyleProperty;
		static BackstageTabItem() {
			ControlPaneProperty = DependencyPropertyManager.Register("ControlPane", typeof(object), typeof(BackstageTabItem),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnControlPanePropertyChanged)));
			IsSelectedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsSelected", typeof(bool), typeof(BackstageTabItem),
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedPropertyChanged)));
			IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;
			SelectedTextStyleProperty = DependencyPropertyManager.Register("SelectedTextStyle", typeof(Style), typeof(BackstageTabItem),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedTextStylePropertyChanged)));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BackstageTabItem), typeof(BackstageTabItemControlAutomationPeer), owner => new BackstageTabItemControlAutomationPeer((BackstageTabItem)owner));			
		}
		protected static void OnControlPanePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageTabItem)d).OnControlPaneChanged(e.OldValue);
		}
		protected static void OnSelectedTextStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageTabItem)d).OnSelectedTextStyleChanged(e.OldValue as Style);
		}
		protected static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageTabItem)d).OnIsSelectedChanged((bool)e.OldValue);
		}		
		#endregion
		#region dep props
		public object ControlPane {
			get { return (object)GetValue(ControlPaneProperty); }
			set { SetValue(ControlPaneProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			protected internal set { this.SetValue(IsSelectedPropertyKey, value); }
		}
		public Style SelectedTextStyle {
			get { return (Style)GetValue(SelectedTextStyleProperty); }
			set { SetValue(SelectedTextStyleProperty, value); }
		}
		#endregion
		#region events
		#endregion
		public BackstageTabItem() {
			DefaultStyleKey = typeof(BackstageTabItem);
			MergingProperties.SetElementMergingBehavior(this, ElementMergingBehavior.InternalWithInternal);
			if(this.IsInDesignTool()) {
				Content = "Tab";
			}
		}		
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			if (Backstage != null)
				Backstage.UpdateSelectedTabAndIndex();
		}
		protected internal FrameworkElement ContentContainer { get; set; }
		protected virtual void OnControlPaneChanged(object oldValue) {
			oldValue.Do(RemoveLogicalChild);
			ControlPane.IfNot(HasParent).Do(AddLogicalChild);
			if(Backstage == null || !IsSelected) return;
			Backstage.UpdateActualControlPane();
		}
		bool HasParent(object obj) {
			if (obj is FrameworkElement)
				return ((FrameworkElement)obj).Parent != null;
			if (obj is FrameworkContentElement)
				return ((FrameworkContentElement)obj).Parent != null;
			return false;
		}
		protected override IEnumerator LogicalChildren {
			get {
				return new MergedEnumerator(base.LogicalChildren, new SingleLogicalChildEnumerator(ControlPane));
			}
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override void UpdateActualTextStyle() {
			if(IsSelected) {
				ActualTextStyle = SelectedTextStyle;
				return;
			}
			base.UpdateActualTextStyle();
		}
		protected internal override void OnClick() {
			if(Backstage != null) {
				Backstage.SetCurrentValue(BackstageViewControl.SelectedTabProperty, this);
			}
			base.OnClick();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ContentContainer = GetTemplateChild("PART_Content") as FrameworkElement;
		}
		protected virtual void OnSelectedTextStyleChanged(Style oldValue) {
			UpdateActualTextStyle();
		}
		protected virtual void OnIsSelectedChanged(bool oldValue) {
			UpdateActualTextStyle();
		}
	}
}
