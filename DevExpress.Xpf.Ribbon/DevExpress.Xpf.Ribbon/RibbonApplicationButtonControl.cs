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
using System.Windows;
using DevExpress.Xpf.Bars;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Ribbon.Themes;
using System.ComponentModel;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonApplicationButtonControl : ContentControl {
		#region static
		public static readonly DependencyProperty ActualBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualBorderTemplatePropertyKey;
		public static readonly DependencyProperty IsPopupProperty;
		public static readonly DependencyProperty IsCheckedProperty;
		public static readonly DependencyProperty RibbonStyleProperty;
		public static readonly DependencyProperty RibbonProperty;
		protected internal static readonly DependencyPropertyKey RibbonPropertyKey;
		public static readonly DependencyProperty BorderTemplateForOffice2007RibbonStyleProperty;
		public static readonly DependencyProperty BorderTemplateInPopupForOffice2007RibbonStyleProperty;
		public static readonly DependencyProperty BorderTemplateForOffice2010RibbonStyleProperty;
		public static readonly DependencyProperty BorderTemplateInPopupForOffice2010RibbonStyleProperty;
		public static readonly DependencyProperty BorderTemplateInPopupForTabletOfficeRibbonStyleProperty;
		public static readonly DependencyProperty BorderTemplateForTabletOfficeRibbonStyleProperty;
		public static readonly DependencyProperty BorderTemplateForOfficeSlimRibbonStyleProperty;
		public static readonly DependencyProperty BorderTemplateInPopupForOfficeSlimRibbonStyleProperty;
		static RibbonApplicationButtonControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonApplicationButtonControl), new FrameworkPropertyMetadata(typeof(RibbonApplicationButtonControl)));
			ActualBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorderTemplate", typeof(ControlTemplate), typeof(RibbonApplicationButtonControl),
				new FrameworkPropertyMetadata(null));
			ActualBorderTemplateProperty = ActualBorderTemplatePropertyKey.DependencyProperty;
			IsPopupProperty = DependencyPropertyManager.Register("IsPopup", typeof(bool), typeof(RibbonApplicationButtonControl),
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsPopupPropertyChanged)));
			IsCheckedProperty = DependencyPropertyManager.Register("IsChecked", typeof(bool), typeof(RibbonApplicationButtonControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsCheckedPropertyChanged)));
			RibbonStyleProperty = RibbonControl.RibbonStyleProperty.AddOwner(typeof(RibbonApplicationButtonControl), new PropertyMetadata(OnRibbonStylePropertyChanged));
			RibbonPropertyKey = DependencyPropertyManager.RegisterReadOnly("Ribbon", typeof(RibbonControl), typeof(RibbonApplicationButtonControl), new FrameworkPropertyMetadata(null));
			RibbonProperty = RibbonPropertyKey.DependencyProperty;
			BorderTemplateForOffice2007RibbonStyleProperty =
				DependencyPropertyManager.Register("BorderTemplateForOffice2007RibbonStyle", typeof(ControlTemplate), typeof(RibbonApplicationButtonControl), new FrameworkPropertyMetadata(null));
			BorderTemplateInPopupForOffice2007RibbonStyleProperty =
				DependencyPropertyManager.Register("BorderTemplateInPopupForOffice2007RibbonStyle", typeof(ControlTemplate), typeof(RibbonApplicationButtonControl), new FrameworkPropertyMetadata(null));
			BorderTemplateForOffice2010RibbonStyleProperty =
				DependencyPropertyManager.Register("BorderTemplateForOffice2010RibbonStyle", typeof(ControlTemplate), typeof(RibbonApplicationButtonControl), new FrameworkPropertyMetadata(null));
			BorderTemplateInPopupForOffice2010RibbonStyleProperty =
				DependencyPropertyManager.Register("BorderTemplateInPopupForOffice2010RibbonStyle", typeof(ControlTemplate), typeof(RibbonApplicationButtonControl), new FrameworkPropertyMetadata(null));
			BorderTemplateForTabletOfficeRibbonStyleProperty =
				DependencyProperty.Register("BorderTemplateForTabletOfficeRibbonStyle", typeof(ControlTemplate), typeof(RibbonApplicationButtonControl), new FrameworkPropertyMetadata(null));
			BorderTemplateInPopupForTabletOfficeRibbonStyleProperty = 
				DependencyPropertyManager.Register("BorderTemplateInPopupForTabletOfficeRibbonStyle", typeof(ControlTemplate), typeof(RibbonApplicationButtonControl), new FrameworkPropertyMetadata(null));
			BorderTemplateForOfficeSlimRibbonStyleProperty = DependencyProperty.Register("BorderTemplateForOfficeSlimRibbonStyle", typeof(ControlTemplate), typeof(RibbonApplicationButtonControl), new FrameworkPropertyMetadata(null));
			BorderTemplateInPopupForOfficeSlimRibbonStyleProperty = DependencyProperty.Register("BorderTemplateInPopupForOfficeSlimRibbonStyle", typeof(ControlTemplate), typeof(RibbonApplicationButtonControl), new FrameworkPropertyMetadata(null));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(RibbonApplicationButtonControl), typeof(RibbonApplicationButtonControlAutomationPeer), owner => new RibbonApplicationButtonControlAutomationPeer((RibbonApplicationButtonControl)owner));
		}
		protected static void OnIsPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonApplicationButtonControl)d).UpdateActualBorderTemplate();
		}
		protected static void OnIsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonApplicationButtonControl)d).OnIsCheckedChanged((bool)e.OldValue);
		}
		protected static void OnRibbonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonApplicationButtonControl)d).OnRibbonStyleChanged((RibbonStyle)e.OldValue);
		}
		#endregion
		#region dep props
		public ControlTemplate ActualBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualBorderTemplateProperty); }
			protected set { this.SetValue(ActualBorderTemplatePropertyKey, value); }
		}
		public bool IsPopup {
			get { return (bool)GetValue(IsPopupProperty); }
			set { SetValue(IsPopupProperty, value); }
		}
		public bool IsChecked {
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		public RibbonStyle RibbonStyle {
			get { return (RibbonStyle)GetValue(RibbonStyleProperty); }
			set { SetValue(RibbonStyleProperty, value); }
		}
		public RibbonControl Ribbon {
			get { return (RibbonControl)GetValue(RibbonProperty); }
			protected internal set { this.SetValue(RibbonPropertyKey, value); }
		}
		public ControlTemplate BorderTemplateForOffice2007RibbonStyle {
			get { return (ControlTemplate)GetValue(BorderTemplateForOffice2007RibbonStyleProperty); }
			set { SetValue(BorderTemplateForOffice2007RibbonStyleProperty, value); }
		}
		public ControlTemplate BorderTemplateInPopupForOffice2007RibbonStyle {
			get { return (ControlTemplate)GetValue(BorderTemplateInPopupForOffice2007RibbonStyleProperty); }
			set { SetValue(BorderTemplateInPopupForOffice2007RibbonStyleProperty, value); }
		}
		public ControlTemplate BorderTemplateForOffice2010RibbonStyle {
			get { return (ControlTemplate)GetValue(BorderTemplateForOffice2010RibbonStyleProperty); }
			set { SetValue(BorderTemplateForOffice2010RibbonStyleProperty, value); }
		}
		public ControlTemplate BorderTemplateInPopupForOffice2010RibbonStyle {
			get { return (ControlTemplate)GetValue(BorderTemplateInPopupForOffice2010RibbonStyleProperty); }
			set { SetValue(BorderTemplateInPopupForOffice2010RibbonStyleProperty, value); }
		}
		public ControlTemplate BorderTemplateForTabletOfficeRibbonStyle {
			get { return (ControlTemplate)GetValue(BorderTemplateForTabletOfficeRibbonStyleProperty); }
			set { SetValue(BorderTemplateForTabletOfficeRibbonStyleProperty, value); }
		}
		public ControlTemplate BorderTemplateInPopupForTabletOfficeRibbonStyle {
			get { return (ControlTemplate)GetValue(BorderTemplateInPopupForTabletOfficeRibbonStyleProperty); }
			set { SetValue(BorderTemplateInPopupForTabletOfficeRibbonStyleProperty, value); }
		}
		public ControlTemplate BorderTemplateForOfficeSlimRibbonStyle {
			get { return (ControlTemplate)GetValue(BorderTemplateForOfficeSlimRibbonStyleProperty); }
			set { SetValue(BorderTemplateForOfficeSlimRibbonStyleProperty, value); }
		}
		public ControlTemplate BorderTemplateInPopupForOfficeSlimRibbonStyle {
			get { return (ControlTemplate)GetValue(BorderTemplateInPopupForOfficeSlimRibbonStyleProperty); }
			set { SetValue(BorderTemplateInPopupForOfficeSlimRibbonStyleProperty, value); }
		}
		#endregion
		public RibbonApplicationButtonControl() { }
		protected internal bool ToggleOnClick { get; set; }
		public event EventHandler Checked;
		public event EventHandler Unchecked;
		public event EventHandler Click;
		protected virtual void OnIsCheckedChanged(bool oldValue) {
			if(IsChecked) {
			   if(Checked != null)
				   Checked(this, new EventArgs());
			} else {
				if(Unchecked != null)
					Unchecked(this, new EventArgs());
			}
		}
		protected virtual void UpdateActualBorderTemplate() {
			switch(RibbonStyle) {
				case RibbonStyle.TabletOffice:
					ActualBorderTemplate = IsPopup ? BorderTemplateInPopupForTabletOfficeRibbonStyle : BorderTemplateForTabletOfficeRibbonStyle;
					break;
				case RibbonStyle.Office2007:
					ActualBorderTemplate = IsPopup ? BorderTemplateInPopupForOffice2007RibbonStyle : BorderTemplateForOffice2007RibbonStyle;
					break;
				case RibbonStyle.Office2010:
					ActualBorderTemplate = IsPopup ? BorderTemplateInPopupForOffice2010RibbonStyle : BorderTemplateForOffice2010RibbonStyle;
					break;
				case RibbonStyle.OfficeSlim:
					ActualBorderTemplate = IsPopup ? BorderTemplateInPopupForOfficeSlimRibbonStyle : BorderTemplateForOfficeSlimRibbonStyle;
					break;
				default:
					ActualBorderTemplate = null;
					break;
			}
		}
		protected virtual void OnRibbonStyleChanged(RibbonStyle oldValue) {
			UpdateActualBorderTemplate();
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateActualBorderTemplate();
			if(Ribbon != null && Ribbon.IsBackStageViewOpen)
				IsChecked = true;
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			Mouse.Capture(this);
			NavigationTree.Lock();
			base.OnMouseLeftButtonDown(e);			
			OnClick();
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			NavigationTree.Unlock();
			ReleaseMouseCapture();
		}
		protected internal virtual void OnClick() {
			if(ToggleOnClick)
				IsChecked = !IsChecked;
			if(Click != null)
				Click(this, new EventArgs());
		}
	}
}
