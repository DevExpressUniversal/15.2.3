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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraReports.Wizards3.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Wizards3.Views {
	[ToolboxItem(false)]
	partial class SelectColumnsPageView : WizardViewBase, ISelectColumnsPageView {
		public SelectColumnsPageView() {
			InitializeComponent();
			InitializeImages();
		}
		void InitializeImages() {
			this.addButton.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.ArrowRight.png"), LocalResFinder.Assembly);
			this.addAllButton.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.DoubleArrowRight.png"), LocalResFinder.Assembly);
			this.removeButton.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.ArrowLeft.png"), LocalResFinder.Assembly);
			this.removeAllButton.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.DoubleArrowLeft.png"), LocalResFinder.Assembly);
		}
		#region ISelectColumnsPageView Members
		public event EventHandler ActiveColumnsChanged;
		public event EventHandler AddColumnsClicked;
		public event EventHandler AddAllColumnsClicked;
		public event EventHandler RemoveColumnsClicked;
		public event EventHandler RemoveAllColumnsClicked;
		public void EnableAddColumnsButton(bool enable) {
			addButton.Enabled = enable;
		}
		public void EnableAddAllColumnsButton(bool enable) {
			addAllButton.Enabled = enable;
		}
		public void EnableRemoveColumnsButton(bool enable) {
			removeButton.Enabled = enable;
		}
		public void EnableRemoveAllColumnsButton(bool enable) {
			removeAllButton.Enabled = enable;
		}
		public void FillAvailableColumns(ColumnInfo[] columns) {
			availableColumnsListBox.Items.Clear();
			availableColumnsListBox.DataSource = columns;
		}
		public void FillSelectedColumns(ColumnInfo[] columns) {
			selectedColumnsListBox.Items.Clear();
			selectedColumnsListBox.DataSource = columns;
		}
		public ColumnInfo[] GetActiveAvailableColumns() {
			return ConvertToArray(availableColumnsListBox.SelectedItems.Cast<object>());
		}
		public ColumnInfo[] GetActiveSelectedColumns() {
			return ConvertToArray(selectedColumnsListBox.SelectedItems.Cast<object>());
		}
		static ColumnInfo[] ConvertToArray(IEnumerable<object> items) {
			return items.Select(x => (ColumnInfo)x).ToArray();
		}
		public void ShowWaitIndicator(bool visibility) {
		}
		#endregion
		public override string HeaderDescription { get { return ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_ChooseColumns_Description); } }
		private void addButton_Click(object sender, EventArgs e) {
			if(AddColumnsClicked != null) {
				AddColumnsClicked(this, EventArgs.Empty);
			}
		}
		private void addAllButton_Click(object sender, EventArgs e) {
			if(AddAllColumnsClicked != null) {
				AddAllColumnsClicked(this, EventArgs.Empty);
			}
		}
		private void removeButton_Click(object sender, EventArgs e) {
			if(RemoveColumnsClicked != null) {
				RemoveColumnsClicked(this, EventArgs.Empty);
			}
		}
		private void removeAllButton_Click(object sender, EventArgs e) {
			if(RemoveAllColumnsClicked != null) {
				RemoveAllColumnsClicked(this, EventArgs.Empty);
			}
		}
		private void availableColumnsListBox_SelectedIndexChanged(object sender, EventArgs e) {
			RaiseActiveColumnsChanged();
		}
		private void selectedColumnsListBox_SelectedIndexChanged(object sender, EventArgs e) {
			RaiseActiveColumnsChanged();
		}
		void RaiseActiveColumnsChanged() {
			if(ActiveColumnsChanged != null) {
				ActiveColumnsChanged(this, EventArgs.Empty);
			}
		}
		private void availableColumnsListBox_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(availableColumnsListBox.IndexFromPoint(e.Location)!=-1)
				addButton_Click(sender, EventArgs.Empty);
		}
		private void selectedColumnsListBox_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(selectedColumnsListBox.IndexFromPoint(e.Location) != -1)
				removeButton_Click(sender, EventArgs.Empty);
		}
	}
}
