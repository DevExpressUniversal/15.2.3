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

namespace DevExpress.XtraReports.Design.BindingMapper {
	partial class BindingMapperForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null)
					components.Dispose();
				if(isCheckBoxColumnBehaviorAttached)
					behavior.Detach();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BindingMapperForm));
			DevExpress.XtraTreeList.FilterCondition filterCondition1 = new DevExpress.XtraTreeList.FilterCondition();
			DevExpress.XtraTreeList.StyleFormatConditions.StyleFormatCondition styleFormatCondition1 = new DevExpress.XtraTreeList.StyleFormatConditions.StyleFormatCondition();
			this.isValidColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.treeList1 = new DevExpress.XtraTreeList.TreeList();
			this.checkColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
			this.controlColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.sourceColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
			this.destinationColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.repositoryItemPopupContainerEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPopupContainerEdit();
			this.propertyColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.btCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btOK = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupContainerEdit1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.isValidColumn, "isValidColumn");
			this.isValidColumn.FieldName = "IsValid";
			this.isValidColumn.Name = "isValidColumn";
			this.isValidColumn.OptionsColumn.AllowEdit = false;
			this.isValidColumn.OptionsColumn.FixedWidth = true;
			this.isValidColumn.OptionsColumn.ShowInCustomizationForm = false;
			this.treeList1.ActiveFilterEnabled = false;
			resources.ApplyResources(this.treeList1, "treeList1");
			this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.checkColumn,
			this.controlColumn,
			this.isValidColumn,
			this.sourceColumn,
			this.destinationColumn,
			this.propertyColumn});
			this.treeList1.CustomizationFormBounds = new System.Drawing.Rectangle(899, 460, 216, 178);
			filterCondition1.Column = this.isValidColumn;
			filterCondition1.Condition = DevExpress.XtraTreeList.FilterConditionEnum.Equals;
			filterCondition1.Value1 = false;
			filterCondition1.Value2 = true;
			filterCondition1.Visible = true;
			this.treeList1.FilterConditions.AddRange(new DevExpress.XtraTreeList.FilterCondition[] {
			filterCondition1});
			styleFormatCondition1.Column = this.isValidColumn;
			styleFormatCondition1.Condition = DevExpress.XtraGrid.FormatConditionEnum.Equal;
			styleFormatCondition1.Value1 = true;
			this.treeList1.FormatConditions.AddRange(new DevExpress.XtraTreeList.StyleFormatConditions.StyleFormatCondition[] {
			styleFormatCondition1});
			this.treeList1.Name = "treeList1";
			this.treeList1.OptionsBehavior.EnableFiltering = true;
			this.treeList1.OptionsFilter.AllowColumnMRUFilterList = false;
			this.treeList1.OptionsFilter.AllowFilterEditor = false;
			this.treeList1.OptionsFilter.AllowMRUFilterList = false;
			this.treeList1.OptionsPrint.UsePrintStyles = true;
			this.treeList1.OptionsView.ShowFilterPanelMode = DevExpress.XtraTreeList.ShowFilterPanelMode.ShowAlways;
			this.treeList1.OptionsView.ShowIndicator = false;
			this.treeList1.OptionsView.ShowRoot = false;
			this.treeList1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemCheckEdit1,
			this.repositoryItemTextEdit1,
			this.repositoryItemPopupContainerEdit1});
			this.treeList1.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.treeList1_CustomDrawNodeCell);
			this.treeList1.CustomFilterDisplayText += new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(this.treeList1_CustomFilterDisplayText);
			this.checkColumn.AllowIncrementalSearch = false;
			this.checkColumn.ColumnEdit = this.repositoryItemCheckEdit1;
			this.checkColumn.FieldName = "IsChecked";
			resources.ApplyResources(this.checkColumn, "checkColumn");
			this.checkColumn.Name = "checkColumn";
			this.checkColumn.OptionsColumn.AllowSize = false;
			this.checkColumn.OptionsColumn.AllowSort = false;
			this.checkColumn.OptionsColumn.FixedWidth = true;
			this.checkColumn.OptionsFilter.AllowFilter = false;
			resources.ApplyResources(this.repositoryItemCheckEdit1, "repositoryItemCheckEdit1");
			this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
			resources.ApplyResources(this.controlColumn, "controlColumn");
			this.controlColumn.FieldName = "ControlName";
			this.controlColumn.Name = "controlColumn";
			this.controlColumn.OptionsFilter.AllowFilter = false;
			resources.ApplyResources(this.sourceColumn, "sourceColumn");
			this.sourceColumn.ColumnEdit = this.repositoryItemTextEdit1;
			this.sourceColumn.FieldName = "Source";
			this.sourceColumn.Name = "sourceColumn";
			this.sourceColumn.OptionsColumn.AllowEdit = false;
			this.sourceColumn.OptionsColumn.AllowSort = true;
			this.sourceColumn.OptionsFilter.AllowFilter = false;
			resources.ApplyResources(this.repositoryItemTextEdit1, "repositoryItemTextEdit1");
			this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
			this.repositoryItemTextEdit1.ReadOnly = true;
			resources.ApplyResources(this.destinationColumn, "destinationColumn");
			this.destinationColumn.ColumnEdit = this.repositoryItemPopupContainerEdit1;
			this.destinationColumn.FieldName = "Destination";
			this.destinationColumn.Name = "destinationColumn";
			this.destinationColumn.OptionsFilter.AllowFilter = false;
			this.destinationColumn.OptionsColumn.AllowSort = true;
			resources.ApplyResources(this.repositoryItemPopupContainerEdit1, "repositoryItemPopupContainerEdit1");
			this.repositoryItemPopupContainerEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemPopupContainerEdit1.Buttons"))))});
			this.repositoryItemPopupContainerEdit1.Name = "repositoryItemPopupContainerEdit1";
			resources.ApplyResources(this.propertyColumn, "propertyColumn");
			this.propertyColumn.FieldName = "PropertyName";
			this.propertyColumn.Name = "propertyColumn";
			this.propertyColumn.OptionsColumn.FixedWidth = true;
			this.propertyColumn.OptionsFilter.AllowFilter = false;
			resources.ApplyResources(this.btCancel, "btCancel");
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Name = "btCancel";
			resources.ApplyResources(this.btOK, "btOK");
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Name = "btOK";
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btCancel;
			this.AcceptButton = this.btOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.treeList1);
			this.Name = "BindingMapperForm";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupContainerEdit1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTreeList.TreeList treeList1;
		private DevExpress.XtraTreeList.Columns.TreeListColumn checkColumn;
		private DevExpress.XtraTreeList.Columns.TreeListColumn sourceColumn;
		private DevExpress.XtraTreeList.Columns.TreeListColumn destinationColumn;
		private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
		private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
		private DevExpress.XtraEditors.Repository.RepositoryItemPopupContainerEdit repositoryItemPopupContainerEdit1;
		private DevExpress.XtraEditors.SimpleButton btCancel;
		private DevExpress.XtraEditors.SimpleButton btOK;
		private DevExpress.XtraTreeList.Columns.TreeListColumn controlColumn;
		private DevExpress.XtraTreeList.Columns.TreeListColumn propertyColumn;
		private DevExpress.XtraTreeList.Columns.TreeListColumn isValidColumn;
	}
}
