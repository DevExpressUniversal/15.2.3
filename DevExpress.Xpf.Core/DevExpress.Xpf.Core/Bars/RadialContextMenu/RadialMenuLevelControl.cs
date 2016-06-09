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

using DevExpress.Xpf.Utils;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using System.Linq;
using System.Windows.Data;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	public class RadialMenuLevelControl : LinksControl {
		#region static
		public static readonly DependencyProperty FirstSectorIndexProperty = DependencyProperty.Register("FirstSectorIndex", typeof(int), typeof(RadialMenuLevelControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure));
		static RadialMenuLevelControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialMenuLevelControl), new FrameworkPropertyMetadata(typeof(RadialMenuLevelControl)));
		}
		#endregion
		#region dep props
		public int FirstSectorIndex {
			get { return (int)GetValue(FirstSectorIndexProperty); }
			set { SetValue(FirstSectorIndexProperty, value); }
		}
		#endregion
		#region template parts
		FrameworkElement Border { get; set; }
		#endregion
		RadialMenuControl RadialMenuControl { get { return this.VisualParents().OfType<RadialMenuControl>().FirstOrDefault(); } }
		public RadialMenuLevelControl(ILinksHolder holder) {
			ContainerType = LinkContainerType.RadialMenu;
			LinksHolder = holder;
		}
		public override void OnApplyTemplate() {
			UnsubscribeEvents();
			base.OnApplyTemplate();
			Border = GetTemplateChild("PART_Border") as FrameworkElement;
			SubscribeEvents();
		}
		protected virtual void UnsubscribeEvents() {
			Border.Do(x => x.MouseEnter -= Border_MouseEnter);
		}
		protected virtual void SubscribeEvents() {
			Border.Do(x => x.MouseEnter += Border_MouseEnter);
		}
		private void Border_MouseEnter(object sender, MouseEventArgs e) {
			FocusElement(RadialMenuControl);
		}
		protected bool FocusElement(IInputElement element) {
			return element.Return(x=>x.Focus(), ()=>false);
		}
		public override BarItemLinkCollection ItemLinks {
			get { return LinksHolder == null ? null : LinksHolder.ActualLinks; }
		}
		protected override void OnLinksHolderChanged(DependencyPropertyChangedEventArgs e) {
			base.OnLinksHolderChanged(e);
			if(ItemsSource != null) {
				ClearControlItemLinkCollection(ItemsSource as BarItemLinkInfoCollection);
			}
			ILinksHolder holder = e.NewValue as ILinksHolder;
			if(holder != null)
				UpdateItemsSource(holder.ActualLinks);
		}
		protected internal virtual void UpdateItemsSource(BarItemLinkCollection itemLinks) {
			BarItemLinkInfoCollection oldValue = ItemsSource as BarItemLinkInfoCollection;
			ItemsSource = new BarItemLinkInfoCollection(itemLinks);
			if(oldValue != null)
				oldValue.Source = null;
			CalculateMaxGlyphSize();
		}
		protected internal override void OnItemClick(BarItemLinkControl linkControl) {
			PopupMenuManager.CloseAllPopups(linkControl, null);
		}
		protected internal override void OnItemMouseEnter(BarItemLinkControl linkControl) {
			if(!FocusElement(linkControl))
				FocusElement(RadialMenuControl);
		}
		protected internal override void OnItemMouseLeave(BarItemLinkControl linkControl, MouseEventArgs e) {
			if (RadialMenuControl.With(x => x.Popup).If(x => x.IsOpen).ReturnSuccess())
				FocusElement(RadialMenuControl);
		}
		#region INavigationOwner     
		protected override bool GetCanEnterMenuMode() { return false; }
		protected override IBarsNavigationSupport GetNavigationParent() { return RadialMenuControl; }
		protected override Orientation GetNavigationOrientation() { return Orientation.Horizontal; }
		protected override NavigationKeys GetNavigationKeys() { return NavigationKeys.Arrows | NavigationKeys.Tab; }
		protected override KeyboardNavigationMode GetNavigationMode() { return KeyboardNavigationMode.Cycle; }		
		protected override int GetNavigationID() { return GetHashCode(); }
		#endregion
	}
	public class VisualStateSetter : DependencyObject {
		public static readonly DependencyProperty CommonGroupVisualStateProperty = DependencyProperty.RegisterAttached("CommonGroupVisualState", typeof(string), typeof(VisualStateSetter), new PropertyMetadata(null, OnVisualStateChanged));
		public static string GetCommonGroupVisualState(DependencyObject obj) { return (string)obj.GetValue(CommonGroupVisualStateProperty); }
		public static void SetCommonGroupVisualState(DependencyObject obj, string value) { obj.SetValue(CommonGroupVisualStateProperty, value); }
		public static readonly DependencyProperty CheckedGroupVisualStateProperty = DependencyProperty.RegisterAttached("CheckedGroupVisualState", typeof(string), typeof(VisualStateSetter), new PropertyMetadata(null, OnVisualStateChanged));
		public static string GetCheckedGroupVisualState(DependencyObject obj) { return (string)obj.GetValue(CheckedGroupVisualStateProperty); }
		public static void SetCheckedGroupVisualState(DependencyObject obj, string value) { obj.SetValue(CheckedGroupVisualStateProperty, value); }
		public static readonly DependencyProperty FocusedGroupVisualStateProperty = DependencyProperty.RegisterAttached("FocusedGroupVisualState", typeof(string), typeof(VisualStateSetter), new PropertyMetadata(null, OnVisualStateChanged));
		public static string GetFocusedGroupVisualState(DependencyObject obj) { return (string)obj.GetValue(FocusedGroupVisualStateProperty); }
		public static void SetFocusedGroupVisualState(DependencyObject obj, string value) { obj.SetValue(FocusedGroupVisualStateProperty, value); }
		private static void OnVisualStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			(e.NewValue as string).Do(v => VisualStateManager.GoToState(d as FrameworkElement, v, false));
		}
	}
}
