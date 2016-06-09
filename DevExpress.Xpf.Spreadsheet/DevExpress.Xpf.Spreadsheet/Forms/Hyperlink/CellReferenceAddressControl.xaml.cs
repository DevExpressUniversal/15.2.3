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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class CellReferenceAddressControl : UserControl {
		public CellReferenceAddressControl() {
			InitializeComponent();
		}
		#region Properties
		public TreeViewItem TreeListFocusedNode { 
			get { return treeView.SelectedItem as TreeViewItem; }
			set {
				SelectTreeViewItem(value);
			}
		}
		private void SelectTreeViewItem(TreeViewItem value) {
			int level = 0;
			TreeViewItem item = GetItemLevelCore(value, treeView.Items, ref level);
			item.IsSelected = true;
		}
		public string Text { get { return teCellReference.EditValue != null ? teCellReference.EditValue.ToString() : ""; } set { teCellReference.EditValue = value; } }
		public bool TextEditEnabled { get { return teCellReference.IsEnabled; } set { teCellReference.IsEnabled = value; } }
		public TextEdit TextEdit { get { return teCellReference; } }
		public int TextEditSelectionStart { get { return teCellReference.SelectionStart; } set { teCellReference.SelectionStart = value; } }
		public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged { add { treeView.SelectedItemChanged += value; } remove { treeView.SelectedItemChanged -= value; }}
		#endregion
		internal TreeViewItem AddRootNode(string caption) {
			TreeViewItem item = new TreeViewItem() { Header = caption };
			treeView.Items.Add(item);
			return item;
		}
		void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			throw new NotImplementedException();
		}
		internal TreeViewItem TreeViewAppendNode(string caption, TreeViewItem parentNode) {
			parentNode.Items.Add(new TreeViewItem() { Header = caption});
			return parentNode;
		}
		internal void TreeViewFocus() {
		}
		internal void TreeViewExpandAll() {
			TreeViewExpandAllCore(treeView.Items);
		}
		private void TreeViewExpandAllCore(ItemCollection itemCollection) {
			foreach (TreeViewItem item in itemCollection) {
				TreeViewExpandAllCore(item.Items);
				item.IsExpanded = true;
			}
		}
		internal int GetItemLevel(TreeViewItem treeViewItem) {
			int level = -1;
			GetItemLevelCore(treeViewItem, treeView.Items, ref level);
			return level;
		}
		private TreeViewItem GetItemLevelCore(TreeViewItem searchItem, ItemCollection itemCollection, ref int level) {
			foreach (TreeViewItem item in itemCollection) {
				if (item == searchItem) {
					level++;
					return item;
				}
				TreeViewItem childItem = GetItemLevelCore(searchItem, item.Items, ref level);
				if (childItem != null) {
					level++;
					return childItem;
				}
			}
			return null;
		}
	}
}
