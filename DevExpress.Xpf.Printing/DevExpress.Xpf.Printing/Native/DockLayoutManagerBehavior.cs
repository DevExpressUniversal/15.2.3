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
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public static class DockLayoutManagerBehavior {
		#region HideItemsOnClosingProperty
		public static readonly DependencyProperty HideItemsOnClosingProperty = DependencyPropertyManager.RegisterAttached(
			"HideItemsOnClosing",
			 typeof(bool?),
			 typeof(DockLayoutManagerBehavior),
			 new PropertyMetadata(OnHideItemsOnClosingChanged));
		public static bool? GetHideItemsOnClosing(DependencyObject obj) {
			return (bool?)obj.GetValue(HideItemsOnClosingProperty);
		}
		public static void SetHideItemsOnClosing(DependencyObject obj, bool? value) {
			obj.SetValue(HideItemsOnClosingProperty, value);
		}
		static void OnHideItemsOnClosingChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(sender == null)
				throw new ArgumentNullException("sender");
			DockLayoutManager manager = sender as DockLayoutManager;
			if(manager == null)
				throw new NotSupportedException("It is impossible to attach HideItemsOnClosing behavior to non-DockLayoutManager elements");
			if((bool?)e.NewValue == true) {
				manager.DockItemClosing += manager_DockItemClosing;
			}
			if((bool?)e.NewValue == false) {
				manager.DockItemClosing -= manager_DockItemClosing;
			}
		}
		static void manager_DockItemClosing(object sender, ItemCancelEventArgs e) {
			e.Item.Visibility = Visibility.Collapsed;
			e.Cancel = true;
		}
		#endregion
		#region ForbidDockingWithItemsProperty
		static string itemName = string.Empty;
		public static readonly DependencyProperty ForbidDockingWithItemsProperty = DependencyPropertyManager.RegisterAttached(
			"ForbidDockingWithItems",
			 typeof(string),
			 typeof(DockLayoutManagerBehavior),
			 new PropertyMetadata(OnForbidDockingWithItemsChanged)
		);
		public static string GetForbidDockingWithItems(DependencyObject obj) {
			return (string)obj.GetValue(ForbidDockingWithItemsProperty);
		}
		public static void SetForbidDockingWithItems(DependencyObject obj, string value) {
			obj.SetValue(ForbidDockingWithItemsProperty, value);
		}
		static void OnForbidDockingWithItemsChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(sender == null)
				throw new ArgumentNullException("sender");
			DockLayoutManager manager = sender as DockLayoutManager;
			if(manager == null)
				throw new NotSupportedException("It is impossible to attach ForbidDockingWithItems behavior to non-DockLayoutManager elements");
			itemName = e.NewValue as string;
			if(string.IsNullOrEmpty(itemName))
				throw new ArgumentException("Cannot assign an empty or null value to the string property");
			if(e.OldValue == null)
				manager.ShowingDockHints += manager_ShowingDockHints;
			else {
				manager.ShowingDockHints -= manager_ShowingDockHints;
				manager.ShowingDockHints += manager_ShowingDockHints;
			}
		}
		static void manager_ShowingDockHints(object sender, ShowingDockHintsEventArgs e) {
			List<string> items = new List<string>(itemName.Split(new char[] { ',' }));
			if(e.DraggingTarget != null && items.Contains(e.DraggingTarget.Name))
				e.Hide(DockGuide.Center);
		}
		#endregion
	}
}
