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
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.Xpf.Spreadsheet.Localization;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class SimpleFilterControl : UserControl {
		#region Fields
		SimpleFilterViewModel viewModel;
		public ObservableCollection<Node> Nodes { get; private set; }
		#endregion
		public SimpleFilterControl(SimpleFilterViewModel viewModel) {
			InitializeComponent();
			DataContext = viewModel;
			this.ViewModel = viewModel;
		}
		#region Properties
		public SimpleFilterViewModel ViewModel {
			get { return viewModel; }
			set {
				if (this.viewModel == value)
					return;
				Guard.ArgumentNotNull(value, "viewModel");
				this.viewModel = value;
				PopulateTreeView(viewModel.Root);
			}
		}
		#endregion
		void PopulateTreeView(FilterValueNode value) {
			Nodes = new ObservableCollection<Node>();
			Nodes.Clear();
			bool isCheckedNode = false;
			bool isNotCheckedNode = false;
			foreach (var child in value.Children) {
				Node startNode = new Node(child) { Text = child.ToString(), IsChecked = child.IsChecked };
				CheckNodeState(child, ref isCheckedNode, ref isNotCheckedNode);
				RecursivePopulateTreeView(startNode, child.Children, ref isCheckedNode, ref isNotCheckedNode);
				Nodes.Add(startNode);
			}
			treeView.ItemsSource = Nodes;
		}
		void RecursivePopulateTreeView(Node previousNode, List<FilterValueNode> list, ref bool isCheckedNode, ref bool isNotCheckedNode) {
			foreach (var child in list) {
				Node nextNode = new Node(child) { Text = child.ToString(), IsChecked = child.IsChecked };
				CheckNodeState(child, ref isCheckedNode, ref isNotCheckedNode);
				nextNode.Parent.Add(previousNode);
				previousNode.Children.Add(nextNode);
				if (nextNode.IsChecked != previousNode.IsChecked)
					nextNode.CheckedTreeChildMiddle(nextNode.Parent, nextNode.Children, nextNode.IsChecked);
				RecursivePopulateTreeView(nextNode, child.Children, ref isCheckedNode, ref isNotCheckedNode);
			}
		}
		void CheckNodeState(FilterValueNode node, ref bool isCheckedNode, ref bool isNotCheckedNode) {
			if (node.IsChecked)
				isCheckedNode = true;
			else
				isNotCheckedNode = true;
		}
		public void UpdateViewModelState() {
			foreach (Node node in Nodes) {
				FindAndSetStateInViewModel(node.ItemModel, node.IsChecked.HasValue ? node.IsChecked.Value : true);
				RecursiveTraversingTreeView(node.Children);
			}
		}
		void RecursiveTraversingTreeView(ObservableCollection<Node> nodes) {
			foreach (Node node in nodes) {
				FindAndSetStateInViewModel(node.ItemModel, node.IsChecked.HasValue ? node.IsChecked.Value : true);
				RecursiveTraversingTreeView(node.Children);
			}
		}
		void FindAndSetStateInViewModel(FilterValueNode itemName, bool isChecked) {
			foreach (var item in viewModel.DataSource) {
				if (item == itemName) {
					item.IsChecked = isChecked;
					if (item.Text == viewModel.BlankValue)
						viewModel.BlankValueChecked = isChecked;
				}
			}
		}
		void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			CheckBox currentCheckBox = (CheckBox)sender;
			CheckBoxId.checkBoxId = currentCheckBox.Uid;
		}
		void buttonCheckAll_Click(object sender, RoutedEventArgs e) {
			CheckedTree(Nodes, true);
		}
		void buttonUncheckAll_Click(object sender, RoutedEventArgs e) {
			CheckedTree(Nodes, false);
		}
		public void CheckedTree(ObservableCollection<Node> items, bool isChecked) {
			foreach (Node item in items) {
				item.IsChecked = isChecked;
				item.ItemModel.IsChecked = isChecked;
				if (item.Children.Count != 0) CheckedTree(item.Children, isChecked);
			}
		}
	}
	public class SimpleFilterDXDialog : CustomDXDialog {
		#region Fields
		SimpleFilterViewModel viewModel;
		SimpleFilterControl control;
		#endregion
		public SimpleFilterDXDialog(string title, SimpleFilterViewModel viewModel, SimpleFilterControl control)
			: base(title) {
			this.viewModel = viewModel;
			this.control = control;
		}
		#region Properties
		Button CheckAllButton { get { return YesButton; } }
		Button UncheckAllButton { get { return NoButton; } }
		Button OkFormButton { get { return OkButton; } }
		Button CancelFormButton { get { return CancelButton; } }
		#endregion
		#region Events
		#region CheckAllButtonClick
		EventHandler onCheckAllButtonClick;
		public event EventHandler CheckAllButtonClick { add { onCheckAllButtonClick += value; } remove { onCheckAllButtonClick -= value; } }
		void RaiseCheckAllButtonClick() {
			if (onCheckAllButtonClick != null) {
				onCheckAllButtonClick(this, EventArgs.Empty);
			}
		}
		#endregion
		#region UncheckAllButtonClick
		EventHandler onUncheckAllButtonClick;
		public event EventHandler UncheckAllButtonClick { add { onUncheckAllButtonClick += value; } remove { onUncheckAllButtonClick -= value; } }
		void RaiseUncheckAllButtonClick() {
			if (onUncheckAllButtonClick != null) {
				onUncheckAllButtonClick(this, EventArgs.Empty);
			}
		}
		#endregion
		#endregion
		protected override void ApplyDialogButtonProperty() {
			base.ApplyDialogButtonProperty();
			SetButtonVisibilities(true, true, true, true, false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetupCheckAllButton(this.CheckAllButton, SpreadsheetControlStringId.Caption_SimpleFilterCheckAll);
			SetupUncheckAllButton(this.UncheckAllButton, SpreadsheetControlStringId.Caption_SimpleFilterUncheckAll);
		}
		protected override void OnButtonClick(bool? result, MessageBoxResult messageBoxResult) {
			if (messageBoxResult == MessageBoxResult.Yes && CheckAllButton != null) {
				CheckAllButton.Focus();
				RaiseCheckAllButtonClick();
				return;
			}
			if (messageBoxResult == MessageBoxResult.No && UncheckAllButton != null) {
				UncheckAllButton.Focus();
				RaiseUncheckAllButtonClick();
				return;
			}
			base.OnButtonClick(result, messageBoxResult);
		}
		void SetupUncheckAllButton(Button button, SpreadsheetControlStringId stringId) {
			if (button == null)
				return;
			button.Content = XpfSpreadsheetLocalizer.GetString(stringId);
			button.IsDefault = false;
			button.IsCancel = false;
		}
		void SetupCheckAllButton(Button button, SpreadsheetControlStringId stringId) {
			if (button == null)
				return;
			button.Content = XpfSpreadsheetLocalizer.GetString(stringId);
			button.IsDefault = false;
			button.IsCancel = false;
		}
	}
	public class Node : INotifyPropertyChanged {
		#region Fields
		private ObservableCollection<Node> children = new ObservableCollection<Node>();
		private ObservableCollection<Node> parent = new ObservableCollection<Node>();
		private string text;
		private string id;
		private bool? isChecked = true;
		private bool isExpanded;
		private FilterValueNode itemModel;
		#endregion
		public Node(FilterValueNode item) {
			this.id = Guid.NewGuid().ToString();
			this.itemModel = item;
		}
		#region Properties
		public ObservableCollection<Node> Children {
			get { return this.children; }
		}
		public ObservableCollection<Node> Parent {
			get { return this.parent; }
		}
		public bool? IsChecked {
			get { return this.isChecked; }
			set {
				this.isChecked = value;
				RaisePropertyChanged("IsChecked");
			}
		}
		public string Text {
			get { return this.text; }
			set {
				this.text = value;
				RaisePropertyChanged("Text");
			}
		}
		public bool IsExpanded {
			get { return isExpanded; }
			set {
				isExpanded = value;
				RaisePropertyChanged("IsExpanded");
			}
		}
		public FilterValueNode ItemModel {
			get { return this.itemModel; }
			set {
				this.itemModel = value;
				RaisePropertyChanged("IsChecked");
			}
		}
		public string Id {
			get { return this.id; }
			set {
				this.id = value;
			}
		}
		#endregion
		public event PropertyChangedEventHandler PropertyChanged;
		private void RaisePropertyChanged(string propertyName) {
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			int countCheck = 0;
			if (propertyName == "IsChecked") {
				if (this.Id == CheckBoxId.checkBoxId && this.Parent.Count == 0 && this.Children.Count != 0) {
					CheckedTreeParent(this.Children, this.IsChecked);
				}
				if (this.Id == CheckBoxId.checkBoxId && this.Parent.Count > 0 && this.Children.Count > 0) {
					CheckedTreeChildMiddle(this.Parent, this.Children, this.IsChecked);
				}
				if (this.Id == CheckBoxId.checkBoxId && this.Parent.Count > 0 && this.Children.Count == 0) {
					CheckedTreeChild(this.Parent, countCheck);
				}
			}
		}
		public void CheckedTreeChildMiddle(ObservableCollection<Node> itemsParent, ObservableCollection<Node> itemsChild, bool? isChecked) {
			int countCheck = 0;
			CheckedTreeParent(itemsChild, isChecked);
			CheckedTreeChild(itemsParent, countCheck);
		}
		private void CheckedTreeParent(ObservableCollection<Node> items, bool? isChecked) {
			foreach (Node item in items) {
				item.IsChecked = isChecked;
				item.ItemModel.IsChecked = isChecked.HasValue ? isChecked.Value : true;
				if (item.Children.Count != 0) CheckedTreeParent(item.Children, isChecked);
			}
		}
		private void CheckedTreeChild(ObservableCollection<Node> items, int countCheck) {
			bool isNull = false;
			foreach (Node paren in items) {
				foreach (Node child in paren.Children) {
					if (child.IsChecked == true || child.IsChecked == null) {
						countCheck++;
						if (child.IsChecked == null)
							isNull = true;
					}
				}
				if (countCheck != paren.Children.Count && countCheck != 0) {
					paren.IsChecked = null;
					paren.ItemModel.IsChecked = true;
				}
				else if (countCheck == 0) {
					paren.IsChecked = false;
					paren.ItemModel.IsChecked = false;
				}
				else if (countCheck == paren.Children.Count && isNull) {
					paren.IsChecked = null;
					paren.ItemModel.IsChecked = true;
				}
				else if (countCheck == paren.Children.Count && !isNull) {
					paren.IsChecked = true;
					paren.ItemModel.IsChecked = true;
				}
				if (paren.Parent.Count != 0) CheckedTreeChild(paren.Parent, 0);
			}
		}
	}
	public struct CheckBoxId {
		public static string checkBoxId;
	}
}
