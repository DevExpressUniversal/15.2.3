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
using System.Windows.Input;
using DevExpress.Mvvm.Native;
#if !NETFX_CORE
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MouseEventArgs = Windows.UI.Xaml.RoutedEventArgs;
#endif
namespace DevExpress.Mvvm.UI {
	public class ItemsControlMouseEventArgsConverter : EventArgsConverterBase<MouseEventArgs> {
		protected static Dictionary<Type, Type> itemsTypes = new Dictionary<Type, Type>() { 
			{ typeof(ListBox), typeof(ListBoxItem) },			
#if !SILVERLIGHT
			{ typeof(ListView), typeof(ListViewItem) },
#if !NETFX_CORE
			{ typeof(TreeView), typeof(TreeViewItem) },
			{ typeof(TreeViewItem), typeof(TreeViewItem) },
			{ typeof(TabControl), typeof(TabItem) },
			{ typeof(StatusBar), typeof(StatusBarItem) },
			{ typeof(Menu), typeof(MenuItem) },
#else
			{ typeof(GridView), typeof(GridViewItem) },
			{ typeof(FlipView), typeof(FlipViewItem) },
#endif
#endif
		};
#if !NETFX_CORE
		public static readonly DependencyProperty ItemTypeProperty =
			DependencyProperty.Register("ItemType", typeof(Type), typeof(ItemsControlMouseEventArgsConverter), new PropertyMetadata(null));
		public Type ItemType {
			get { return (Type)GetValue(ItemTypeProperty); }
			set { SetValue(ItemTypeProperty, value); }
		}
#else
		public static readonly DependencyProperty ItemTypeProperty =
				DependencyProperty.Register("ItemType", typeof(String), typeof(ItemsControlMouseEventArgsConverter), new PropertyMetadata(null));
		public String ItemType {
			get { return (String)GetValue(ItemTypeProperty); }
			set { SetValue(ItemTypeProperty, value); }
		}
#endif
		protected override object Convert(object sender, MouseEventArgs args) {
			return ConvertCore(sender, (DependencyObject)args.OriginalSource);
		}
		protected object ConvertCore(object sender, DependencyObject originalSource) {
			var element = FindParent(sender, originalSource);
#if !NETFX_CORE
			return element != null ? element.DataContext : null;
#else
			if(element == null)
				return null;
			if(element.DataContext != null)
				return element.DataContext;
			return (sender as IItemContainerMapping).With(x => x.ItemFromContainer(element));
#endif
		}
		protected virtual FrameworkElement FindParent(object sender, DependencyObject originalSource) {
			return LayoutTreeHelper.GetVisualParents(originalSource, (DependencyObject)sender).Where(CheckItemType(sender)).FirstOrDefault() as FrameworkElement;
		}
		protected Func<DependencyObject, bool> CheckItemType(object sender) {
			return d => d.GetType() == GetItemType(sender);
		}
		protected virtual Type GetItemType(object sender) {
#if !NETFX_CORE
			Type itemType = ItemType;
#else
			Type itemType = null; 
#endif
			if(itemType == typeof(FrameworkElement) || (ItemType != null && itemType.IsSubclassOf(typeof(FrameworkElement))))
				return itemType;
			Type result = null;
			itemsTypes.TryGetValue(sender.GetType(), out result);
			return result;
		}
	}
}
