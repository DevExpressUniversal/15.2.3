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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class SimpleFilterForm : XtraForm {
		SimpleFilterViewModel viewModel;
		public SimpleFilterForm() {
			InitializeComponent();
		}
		public SimpleFilterForm(SimpleFilterViewModel viewModel) {
			InitializeComponent();
			this.ViewModel = viewModel;
		}
		public SimpleFilterViewModel ViewModel {
			get { return viewModel; }
			set {
				if (value == viewModel)
					return;
				Guard.ArgumentNotNull(value, "viewModel");
				this.viewModel = value;
				SetBindings();
			}
		}
		protected internal virtual void SetBindings() {
			if (viewModel == null)
				return;
			edtValues.DataSource = ViewModel.DataSource;
			edtValues.Columns["IsChecked"].Visible = false;
			edtValues.Columns["DateTime"].Visible = false;
			edtValues.Columns["DateTimeGrouping"].Visible = false;
			edtValues.ForceInitialize();
			UpdateNodeCheckboxes();
			edtValues.NodesIterator.DoOperation(new UpdateNodeInitialExpandOperation());
			if (edtValues.Nodes.Count == 0)
				btnOk.Enabled = false;
		}
		#region UpdateDateNodesCheckboxes
		void UpdateNodeCheckboxes() {
			edtValues.BeginUpdate();
			try {
				edtValues.NodesIterator.DoOperation(new UpdateNodeCheckedOperation());
				UpdateNodeCheckboxes(edtValues.Nodes, CheckState.Unchecked);
			}
			finally {
				edtValues.EndUpdate();
			}
		}
		class UpdateNodeCheckedOperation : TreeListOperation {
			public override void Execute(TreeListNode node) {
				node.Checked = (bool)node.GetValue("IsChecked");
			}
			public override bool NeedsFullIteration { get { return true; } }
		}
		class UpdateNodeInitialExpandOperation : TreeListOperation {
			public override void Execute(TreeListNode node) {
				DateTimeGroupingType dateTimeGrouping = (DateTimeGroupingType)node.GetValue("DateTimeGrouping");
				if (dateTimeGrouping < DateTimeGroupingType.Day)
					node.Expanded = true;
			}
			public override bool NeedsFullIteration { get { return true; } }
		}
		CheckState UpdateNodeCheckboxes(TreeListNodes nodes, CheckState parentCheckState) {
			if (nodes.Count <= 0)
				return parentCheckState;
			CheckState result = UpdateNodeCheckBox(nodes[0]);
			int count = nodes.Count;
			for (int i = 1; i < count; i++) {
				CheckState state = UpdateNodeCheckBox(nodes[i]);
				if (result != state)
					result = CheckState.Indeterminate;
			}
			return result;
		}
		CheckState UpdateNodeCheckBox(TreeListNode node) {
			CheckState state = UpdateNodeCheckboxes(node.Nodes, node.CheckState);
			if (node.CheckState != state) {
				if (node.Nodes.Count > 1)
					state = CheckState.Indeterminate;
				node.CheckState = state;
				UpdateModelIsChecked(node);
			}
			return state;
		}
		void SetNodeCheckRecursively(TreeListNodes nodes, CheckState state) {
			foreach (TreeListNode node in nodes) {
				node.CheckState = state;
				UpdateModelIsChecked(node);
				SetNodeCheckRecursively(node.Nodes, state);
			}
		}
		void edtValues_AfterCheckNode(object sender, XtraTreeList.NodeEventArgs e) {
			edtValues.BeginUpdate();
			try {
				TreeListNode selectedNode = e.Node;
				bool isChecked = selectedNode.CheckState != CheckState.Unchecked;
				CheckState state = isChecked ? CheckState.Checked : CheckState.Unchecked;
				selectedNode.CheckState = state;
				UpdateModelIsChecked(selectedNode);
				SetNodeCheckRecursively(selectedNode.Nodes, state);
				UpdateNodeParentCheckboxes(selectedNode.ParentNode, state);
				btnOk.Enabled = isChecked || viewModel.Validate();
			}
			finally {
				edtValues.EndUpdate();
			}
		}
		void UpdateNodeParentCheckboxes(TreeListNode nodeParent, CheckState state) {
			if (nodeParent == null)
				return;
			nodeParent.CheckState = state;
			foreach (TreeListNode node in nodeParent.Nodes) {
				if (node.CheckState != state) {
					nodeParent.CheckState = CheckState.Indeterminate;
					break;
				}
			}
			UpdateModelIsChecked(nodeParent);
			UpdateNodeParentCheckboxes(nodeParent.ParentNode, state);
		}
		void UpdateModelIsChecked(TreeListNode node) {
			string textValue = (string)node.GetValue("Text");
			bool isChecked = node.CheckState != CheckState.Unchecked;
			if (textValue == viewModel.BlankValue)
				viewModel.BlankValueChecked = isChecked;
			node.SetValue("IsChecked", isChecked);
		}
		#endregion
		protected internal virtual void btnOk_Click(object sender, EventArgs e) {
			ViewModel.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
		void btnCheckAll_Click(object sender, EventArgs e) {
			edtValues.BeginUpdate();
			try {
				SetNodeCheckRecursively(edtValues.Nodes, CheckState.Checked);
				btnOk.Enabled = true;
			}
			finally {
				edtValues.EndUpdate();
			}
		}
		void btnUncheckAll_Click(object sender, EventArgs e) {
			edtValues.BeginUpdate();
			try {
				SetNodeCheckRecursively(edtValues.Nodes, CheckState.Unchecked);
				btnOk.Enabled = false;
			}
			finally {
				edtValues.EndUpdate();
			}
		}
	}
}
