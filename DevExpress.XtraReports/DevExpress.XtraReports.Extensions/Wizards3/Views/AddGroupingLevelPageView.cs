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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraReports.Wizards3.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Wizards3.Views {
	[ToolboxItem(false)]
	partial class AddGroupingLevelPageView : WizardViewBase, IAddGroupingLevelPageView {
		public AddGroupingLevelPageView() {
			InitializeComponent();
			InitializeImages();
		}
		#region IAddGroupingLevelPageView Members
		public event EventHandler ActiveAvailableColumnsChanged;
		public event EventHandler ActiveGroupingLevelChanged;
		public event EventHandler AddGroupingLevelClicked;
		public event EventHandler RemoveGroupingLevelClicked;
		public event EventHandler CombineGroupingLevelClicked;
		public event EventHandler GroupingLevelDownClicked;
		public event EventHandler GroupingLevelUpClicked;
		public void FillAvailableColumns(ColumnInfo[] columns) {
			availableColumnsListBox.Items.Clear();
			availableColumnsListBox.DataSource = columns;
		}
		public void FillGroupingLevels(GroupingLevelInfo[] groupingLevels) {
			groupingColumnsTreeView.Nodes.Clear();
			TreeListNodes itemCollection = groupingColumnsTreeView.Nodes;
			TreeListNode lastNode = null;
			for(int i = 0; i < groupingLevels.Length; i++) {
				TreeListNode treeViewItem = groupingColumnsTreeView.AppendNode(new object[] { groupingLevels[i].DisplayName }, lastNode);
				treeViewItem.Tag = groupingLevels[i];
				treeViewItem.ExpandAll();
				lastNode = treeViewItem;
			}
			groupingColumnsTreeView.ExpandAll();
		}
		public ColumnInfo[] GetActiveAvailableColumns() {
			return availableColumnsListBox.SelectedItems.Cast<object>().Select(x => (ColumnInfo)x).ToArray();
		}
		public GroupingLevelInfo GetActiveGroupingLevel() {
			var item = groupingColumnsTreeView.FocusedNode;
			return item != null ? (GroupingLevelInfo)item.Tag : null;
		}
		public void SetActiveGroupingLevel(GroupingLevelInfo groupingLevel) {
			var activeItem = groupingColumnsTreeView.FindItem(x => x.Tag == groupingLevel);
			if(activeItem != null) {
				activeItem.Selected = true;
				groupingColumnsTreeView.FocusedNode = activeItem;
			}
		}
		public void EnableAddGroupingLevelButton(bool enable) {
			addGroupingButton.Enabled = enable;
		}
		public void EnableCombineGroupingLevelButton(bool enable) {
			combineGroupingButton.Enabled = enable;
		}
		public void EnableRemoveGroupingLevelButton(bool enable) {
			removeGroupingButton.Enabled = enable;
		}
		public void EnableGroupingLevelDown(bool enable) {
			groupingLevelDownButton.Enabled = enable;
		}
		public void EnableGroupingLevelUp(bool enable) {
			groupingLevelUpButton.Enabled = enable;
		}
		public void ShowWaitIndicator(bool show) {
		}
		#endregion
		public override string HeaderDescription {
			get {
				return ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_AddGroupingLvl_Description);
			}
		}
		void InitializeImages() {
			this.addGroupingButton.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.ArrowRight.png"), LocalResFinder.Assembly);
			this.combineGroupingButton.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.ArrowPlus.png"), LocalResFinder.Assembly);
			this.removeGroupingButton.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.ArrowLeft.png"), LocalResFinder.Assembly);
			this.groupingLevelUpButton.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.ArrowUp.png"), LocalResFinder.Assembly);
			this.groupingLevelDownButton.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.ArrowDown.png"), LocalResFinder.Assembly);
			this.tableLayoutPanel1.BackgroundImage = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.GroupingPage.png"), LocalResFinder.Assembly);
		}
		private void addGroupingButton_Click(object sender, EventArgs e) {
			if(AddGroupingLevelClicked != null) {
				AddGroupingLevelClicked(this, EventArgs.Empty);
			}
			groupingColumnsTreeView.ExpandAll();
		}
		private void removeGroupingButton_Click(object sender, EventArgs e) {
			if(RemoveGroupingLevelClicked != null) {
				RemoveGroupingLevelClicked(this, EventArgs.Empty);
			}
			groupingColumnsTreeView.ExpandAll();
		}
		private void combineGroupingButton_Click(object sender, EventArgs e) {
			if(CombineGroupingLevelClicked != null) {
				CombineGroupingLevelClicked(this, EventArgs.Empty);
			}
			groupingColumnsTreeView.ExpandAll();
		}
		private void availableColumnsListBox_SelectedIndexChanged(object sender, EventArgs e) {
			if(ActiveAvailableColumnsChanged != null) {
				ActiveAvailableColumnsChanged(this, EventArgs.Empty);
			}
		}
		private void groupingLevelUpButton_Click(object sender, EventArgs e) {
			if(GroupingLevelUpClicked != null) {
				GroupingLevelUpClicked(this, EventArgs.Empty);
			}
			groupingColumnsTreeView.ExpandAll();
		}
		private void groupingLevelDownButton_Click(object sender, EventArgs e) {
			if(GroupingLevelDownClicked != null) {
				GroupingLevelDownClicked(this, EventArgs.Empty);
			}
			groupingColumnsTreeView.ExpandAll();
		}
		private void availableColumnsListBox_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(availableColumnsListBox.IndexFromPoint(e.Location)!=-1)
				addGroupingButton_Click(null, EventArgs.Empty);
		}
		private void groupingColumnsTreeView_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(groupingColumnsTreeView.CalcHitInfo(e.Location).Node != null)
				removeGroupingButton_Click(null, EventArgs.Empty);
		}
		private void groupingColumnsTreeView_FocusedNodeChanged(object sender, XtraTreeList.FocusedNodeChangedEventArgs e) {
			if(ActiveGroupingLevelChanged != null) {
				ActiveGroupingLevelChanged(this, EventArgs.Empty);
			}
		}
	}
}
