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

using System.Windows;
using DevExpress.Xpf.Utils;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Browsing.Design;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#else
using System.Windows.Threading;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public static class TreeViewBehavior {
		public static readonly DependencyProperty ExpandRootNodeProperty = DependencyPropertyManager.RegisterAttached(
			"ExpandRootNode", 
			typeof(bool?), 
			typeof(TreeViewBehavior), 
			new PropertyMetadata(OnExpandRootNodeChanged));
		public static bool? GetExpandRootNode(DependencyObject obj) {
			return (bool?)obj.GetValue(ExpandRootNodeProperty);
		}
		public static void SetExpandRootNode(DependencyObject obj, bool? value) {
			obj.SetValue(ExpandRootNodeProperty, value);
		}
		static void OnExpandRootNodeChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(sender == null)
				throw new ArgumentNullException("sender");
			TreeView treeView = sender as TreeView;
			if(treeView == null)
				throw new NotSupportedException("The ExpandRootNode behavior can be attached to a TreeView class instance only.");
			if((bool)e.NewValue) {
#if SILVERLIGHT
				treeView.Dispatcher.BeginInvoke(() => ExpandRootNodes(treeView));
#else
				Dispatcher.CurrentDispatcher.BeginInvoke(new Action<TreeView>(ExpandRootNodes), DispatcherPriority.Loaded, treeView);
#endif
			}
		}
		static void ExpandRootNodes(TreeView treeView) {
			foreach(object item in treeView.Items) {
				TreeViewItem treeViewItem = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
				if(treeViewItem != null)
					treeViewItem.IsExpanded = true;
			}
		}
		#region Field List
		#region Subscribe
		public static bool GetSubscribe(DependencyObject obj) {
			return (bool)obj.GetValue(SelectedItemProperty);
		}
		public static void SetSubscribe(DependencyObject obj, bool value) {
			obj.SetValue(SubscribeProperty, value);
		}
		public static readonly DependencyProperty SubscribeProperty =
			DependencyPropertyManager.RegisterAttached("Subscribe", typeof(bool), typeof(TreeViewBehavior), new PropertyMetadata(false, OnSubscribeChanged));
		static void OnSubscribeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var treeView = (TreeView)d;
			if((bool)e.NewValue) {
				treeView.SelectedItemChanged += treeView_SelectedItemChanged;
			} else {
				treeView.SelectedItemChanged -= treeView_SelectedItemChanged;
			}
		}
		static void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			var treeView = (TreeView)sender;
#if SL
			treeView.SetValueWithoutChangeNotification(SelectedItemProperty, e.NewValue);
#else
			treeView.SetValue(SelectedItemProperty, e.NewValue);
#endif
		}
		#endregion
		#region SelectedItem
		public static object GetSelectedItem(DependencyObject obj) {
			return (object)obj.GetValue(SelectedItemProperty);
		}
		public static void SetSelectedItem(DependencyObject obj, object value) {
			obj.SetValue(SelectedItemProperty, value);
		}
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyPropertyManager.RegisterAttached("SelectedItem", typeof(object), typeof(TreeViewBehavior), new PropertyMetadata(null, OnSelectedItemChanged));
		static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(e.NewValue == null) {
				return;
			}
			var treeView = (TreeView)d;
			treeView.Dispatcher.BeginInvoke(new Action(() => {
				var treeViewItem = FindTreeViewItem(treeView, e.NewValue);
				if(treeViewItem != null)
					treeViewItem.IsSelected = true;
			}));
		}
		static TreeViewItem FindTreeViewItem(TreeView treeView, object item) {
			var node = item as INode;
			if(node != null && !string.IsNullOrEmpty(node.DataMember))
				return FindTreeViewItemByNodeRecursively(treeView, node);
			List<object> scannedItems = new List<object>();
			return FindTreeViewItemRecursive(treeView.ItemContainerGenerator, treeView.Items, item, scannedItems);
		}
		private static TreeViewItem FindTreeViewItemByNodeRecursively(TreeView treeView, INode node) {
			foreach(INode rootNode in treeView.Items) {
				var treeViewItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(rootNode);
				ExpandTreeViewItem(treeViewItem);
				var result = FindTreeViewItemByNodeRecursivelyCore(treeViewItem.ItemContainerGenerator, rootNode.ChildNodes.Cast<INode>(), node);
				if(result != null) {
					ExpandTreeViewItem(treeViewItem);
					return result;
				}
			}
			return null;
		}
		static TreeViewItem FindTreeViewItemRecursive(ItemContainerGenerator generator, ItemCollection itemCollection, object item, List<object> scannedItems) {
			var treeViewItem = generator.ContainerFromItem(item);
			if(treeViewItem != null)
				return (TreeViewItem)treeViewItem;
			foreach(object childItem in itemCollection) {
				if(scannedItems.Contains(childItem))
					continue;
				TreeViewItem childTreeViewItem = (TreeViewItem)generator.ContainerFromItem(childItem);
				if(childTreeViewItem == null)
					continue;
				bool forceExpand = !childTreeViewItem.IsExpanded;
				if(forceExpand) {
					ExpandTreeViewItem(childTreeViewItem);
				}
				scannedItems.Add(childItem);
				TreeViewItem result = FindTreeViewItemRecursive(childTreeViewItem.ItemContainerGenerator, childTreeViewItem.Items, item, scannedItems);
				if(result != null) {
					return result;
				}
				if(forceExpand) {
					childTreeViewItem.IsExpanded = false;
				}
			}
			return null;
		}
		static TreeViewItem FindTreeViewItemByNodeRecursivelyCore(ItemContainerGenerator generator, IEnumerable<INode> nodes, INode node) {
			var items = node.DataMember.Split('.');
			TreeViewItem treeViewItem = null;
			for(var i = 0; i < items.Length; i++) {
				var currentPath = string.Join(".", items, 0, i + 1);
				treeViewItem = FindTreeViewItemByNode(generator, nodes, currentPath);
				if(treeViewItem == null)
					return null;
				if(i < items.Length - 1) {
					ExpandTreeViewItem(treeViewItem);
					generator = treeViewItem.ItemContainerGenerator;
					nodes = treeViewItem.Items.Cast<INode>();
				}
			}
			return treeViewItem;
		}
		static void ExpandTreeViewItem(TreeViewItem item) {
			if(!item.IsExpanded) {
				item.IsExpanded = true;
				item.UpdateLayout();
			}
		}
		static TreeViewItem FindTreeViewItemByNode(ItemContainerGenerator generator, IEnumerable<INode> nodes, string dataMember) {
			foreach(INode node in nodes)
				if(node.DataMember == dataMember)
					return (TreeViewItem)generator.ContainerFromItem(node);
			return null;
		}
		#endregion
		#endregion
	}
}
