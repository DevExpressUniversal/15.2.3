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
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using System.Windows.Controls;
namespace DevExpress.Xpf.Ribbon {
	public static class RibbonControlHelper {
		public static FrameworkElement GetRibbonPageGroupControl(RibbonPageGroup ribbonPageGroup) {
			return ribbonPageGroup == null ? null : ribbonPageGroup.FirstPageGroupControl;
		}
		public static BarItem GetRibbonPageGroupCollapsedItem(RibbonPageGroupControl pageGroupControl) {
			return pageGroupControl.CollapseButtonInfo == null ? null : pageGroupControl.CollapseButtonInfo.Item;
		}
		public static RibbonCaptionControl GetRibbonPageCaptionControl(RibbonPage ribbonPage) {
			if(ribbonPage == null) return null;
			RibbonPageCategoryControl categoryControl = GetRibbonPageCategoryControl(ribbonPage.PageCategory);
			var item = ribbonPage;
			RibbonPageHeaderControl pageHeaderControl = categoryControl == null ? null : categoryControl.ItemContainerGenerator.ContainerFromItem(item) as RibbonPageHeaderControl;
			return pageHeaderControl == null ? null : pageHeaderControl.CaptionControl;
		}
		public static FrameworkElement GetRibbonPageCategoryHeaderControl(RibbonPageCategoryBase ribbonPageCategory) {
			var category = GetRibbonPageCategoryControl(ribbonPageCategory);
			return category == null ? null : category.PageCategoryHeaderControl;
		}
		public static RibbonPageCategoryControl GetRibbonPageCategoryControl(RibbonPageCategoryBase ribbonPageCategory) {
			if(ribbonPageCategory == null || ribbonPageCategory.Ribbon == null)
				return null;
			RibbonControl ribbon = ribbonPageCategory.Ribbon;
			return ribbon.GetPageCategoryControl(ribbonPageCategory);
		}
		public static ItemBorderControl GetCollapsedStateBorder(RibbonPageGroupControl pageGroupControl) {
			return pageGroupControl == null ? null : pageGroupControl.CollapsedStateBorder;
		}
		public static void ShowRibbonPageGroupInScrollViewer(RibbonPageGroup pageGroup) {
			if(pageGroup == null) return;
			RibbonSelectedPageControl selectedPageControl = pageGroup.Ribbon == null ? null : pageGroup.Ribbon.SelectedPageControl;
			if(selectedPageControl == null)
				return;
			selectedPageControl.ShowRibbonPageGroupInScrollViewer(pageGroup);
		}
		public static RibbonSelectedPageControl GetRibbonSelectedPageControl(RibbonControl ribbon) {
			if(ribbon == null) return null;
			return ribbon.SelectedPageControl;
		}
		public static void ShowApplicationMenu(RibbonControl ribbon) {
			if(ribbon != null) ribbon.ShowApplicationMenu();
		}
		public static bool GetDisableSharedSizeScope(DependencyObject obj) {
			return (bool)obj.GetValue(DisableSharedSizeScopeProperty);
		}
		public static void SetDisableSharedSizeScope(DependencyObject obj, bool value) {
			obj.SetValue(DisableSharedSizeScopeProperty, value);
		}
		public static readonly DependencyProperty DisableSharedSizeScopeProperty =
			DependencyProperty.RegisterAttached("DisableSharedSizeScope", typeof(bool), typeof(RibbonControlHelper), new PropertyMetadata(false, new PropertyChangedCallback(OnDisableSharedSizeScopePropertyChanged)));
		static void OnDisableSharedSizeScopePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if ((bool)e.NewValue) {				
				d.SetValue(new DevExpress.Xpf.Core.Internal.ReflectionHelper().GetStaticMethodHandler<Func<string, Type, DependencyProperty>>(typeof(DependencyProperty), "FromName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)("PrivateSharedSizeScope", typeof(DefinitionBase)), null);
			}
		}
		public static bool GetIsAutoHide(DependencyObject obj) {
			return (bool)obj.GetValue(IsAutoHideProperty);
		}
		public static void SetIsAutoHide(DependencyObject obj, bool value) {
			obj.SetValue(IsAutoHideProperty, value);
		}
		public static readonly DependencyProperty IsAutoHideProperty =
			DependencyProperty.RegisterAttached("IsAutoHide", typeof(bool), typeof(RibbonControlHelper), new PropertyMetadata(false));
		public static bool GetDisplayShowModeSelector(DependencyObject obj) {
			return (bool)obj.GetValue(DisplayShowModeSelectorProperty);
		}
		public static void SetDisplayShowModeSelector(DependencyObject obj, bool value) {
			obj.SetValue(DisplayShowModeSelectorProperty, value);
		}
		public static readonly DependencyProperty DisplayShowModeSelectorProperty =
			DependencyProperty.RegisterAttached("DisplayShowModeSelector", typeof(bool), typeof(RibbonControlHelper), new PropertyMetadata(false));
	}
	public static class RibbonControlDesignTimeEventHelper {
		public static void SubscribeRibbonSelectedPageChanged(RibbonControl ribbon, RibbonPropertyChangedEventHandler handler) {
			ribbon.SelectedPageChanged += handler;
		}
		public static void UnsubscribeRibbonSelectedPageChanged(RibbonControl ribbon, RibbonPropertyChangedEventHandler handler) {
			ribbon.SelectedPageChanged -= handler;
		}
		public static void SubscribeRibbonPageInserted(RibbonControl ribbon, RibbonPageInsertedEventHandler handler) {
			ribbon.RibbonPageInserted += handler;
		}
		public static void UnsubscribeRibbonPageInserted(RibbonControl ribbon, RibbonPageInsertedEventHandler handler) {
			ribbon.RibbonPageInserted -= handler;
		}
		public static void SubscribeRibbonPageRemoved(RibbonControl ribbon, RibbonPageRemovedEventHandler handler) {
			ribbon.RibbonPageRemoved += handler;
		}
		public static void UnsubscribeRibbonPageRemoved(RibbonControl ribbon, RibbonPageRemovedEventHandler handler) {
			ribbon.RibbonPageRemoved -= handler;
		}
		public static void SubscribeRibbonPageGroupInserted(RibbonControl ribbon, RibbonPageGroupInsertedEventHandler handler) {
			ribbon.RibbonPageGroupInserted += handler;
		}
		public static void UnsubscribeRibbonPageGroupInserted(RibbonControl ribbon, RibbonPageGroupInsertedEventHandler handler) {
			ribbon.RibbonPageGroupInserted -= handler;
		}
		public static void SubscribeRibbonPageGroupRemoved(RibbonControl ribbon, RibbonPageGroupRemovedEventHandler handler) {
			ribbon.RibbonPageGroupRemoved += handler;
		}
		public static void UnsubscribeRibbonPageGroupRemoved(RibbonControl ribbon, RibbonPageGroupRemovedEventHandler handler) {
			ribbon.RibbonPageGroupRemoved -= handler;
		}
		public static void SubscribeComplexLayoutUpdated(RibbonControl ribbon, ComplexLayoutStateChangedEventHandler handler) {
			if(ribbon == null || ribbon.SelectedPageControl == null) return;
			ribbon.SelectedPageControl.ComplexLayoutStateChanged += handler;
		}
		public static void UnsubscribeComplexLayoutUpdated(RibbonControl ribbon, ComplexLayoutStateChangedEventHandler handler) {
			if(ribbon == null || ribbon.SelectedPageControl == null) return; 
			ribbon.SelectedPageControl.ComplexLayoutStateChanged -= handler;
		}
	}
}
namespace DevExpress.Xpf.Ribbon.Native {
	public static class RibbonControlPropertiesHelper {
		public static void SetRibbon(RibbonPageCategoryBase category, RibbonControl ribbon) {
			category.Ribbon = ribbon;
		}
		public static void SetRibbonCategory(RibbonPage page, RibbonPageCategoryBase category) {
			page.PageCategory = category;
		}
		public static void SetRibbonPage(RibbonPageGroup group, RibbonPage page) {
			group.Page = page;
		}
	}
}
