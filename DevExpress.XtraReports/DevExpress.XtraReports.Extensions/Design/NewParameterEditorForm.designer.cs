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

namespace DevExpress.XtraReports.Design
{
	partial class NewParameterEditorForm
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewParameterEditorForm));
			this.lbName = new DevExpress.XtraEditors.LabelControl();
			this.teName = new DevExpress.XtraEditors.TextEdit();
			this.teDescription = new DevExpress.XtraEditors.TextEdit();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.pageDynamic = new DevExpress.XtraTab.XtraTabPage();
			this.pageStatic = new DevExpress.XtraTab.XtraTabPage();
			this.dataNavigator1 = new DevExpress.XtraEditors.DataNavigator();
			this.staticItemsTreeList = new DevExpress.XtraTreeList.TreeList();
			this.columnValue = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.columnDescription = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.ceShowAtParametersPanel = new DevExpress.XtraEditors.CheckEdit();
			this.ceStandardValues = new DevExpress.XtraEditors.CheckEdit();
			this.ceMultiValue = new DevExpress.XtraEditors.CheckEdit();
			this.lbDescription = new DevExpress.XtraEditors.LabelControl();
			this.lbType = new DevExpress.XtraEditors.LabelControl();
			this.lbDefaultValue = new DevExpress.XtraEditors.LabelControl();
			this.btCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btOK = new DevExpress.XtraEditors.SimpleButton();
			this.panelLookUpSettings = new System.Windows.Forms.Panel();
			this.cbType = new DevExpress.XtraEditors.ComboBoxEdit();
			this.teDefaultValue = new DevExpress.XtraEditors.TextEdit();
			((System.ComponentModel.ISupportInitialize)(this.teName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teDescription.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.pageStatic.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.staticItemsTreeList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowAtParametersPanel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceStandardValues.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceMultiValue.Properties)).BeginInit();
			this.panelLookUpSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teDefaultValue.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lbName, "lbName");
			this.lbName.Name = "lbName";
			resources.ApplyResources(this.teName, "teName");
			this.teName.Name = "teName";
			resources.ApplyResources(this.teDescription, "teDescription");
			this.teDescription.Name = "teDescription";
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.pageDynamic;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.pageDynamic,
			this.pageStatic});
			this.pageDynamic.Name = "pageDynamic";
			resources.ApplyResources(this.pageDynamic, "pageDynamic");
			this.pageStatic.Controls.Add(this.dataNavigator1);
			this.pageStatic.Controls.Add(this.staticItemsTreeList);
			this.pageStatic.Name = "pageStatic";
			resources.ApplyResources(this.pageStatic, "pageStatic");
			resources.ApplyResources(this.dataNavigator1, "dataNavigator1");
			this.dataNavigator1.Name = "dataNavigator1";
			resources.ApplyResources(this.staticItemsTreeList, "staticItemsTreeList");
			this.staticItemsTreeList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.staticItemsTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.columnValue,
			this.columnDescription});
			this.staticItemsTreeList.Name = "staticItemsTreeList";
			this.staticItemsTreeList.OptionsView.ShowRoot = false;
			resources.ApplyResources(this.columnValue, "columnValue");
			this.columnValue.FieldName = "Value";
			this.columnValue.Name = "columnValue";
			resources.ApplyResources(this.columnDescription, "columnDescription");
			this.columnDescription.FieldName = "Description";
			this.columnDescription.Name = "columnDescription";
			resources.ApplyResources(this.ceShowAtParametersPanel, "ceShowAtParametersPanel");
			this.ceShowAtParametersPanel.Name = "ceShowAtParametersPanel";
			this.ceShowAtParametersPanel.Properties.Caption = resources.GetString("ceShowAtParametersPanel.Properties.Caption");
			resources.ApplyResources(this.ceStandardValues, "ceStandardValues");
			this.ceStandardValues.Name = "ceStandardValues";
			this.ceStandardValues.Properties.Caption = resources.GetString("ceStandardValues.Properties.Caption");
			resources.ApplyResources(this.ceMultiValue, "ceMultiValue");
			this.ceMultiValue.Name = "ceMultiValue";
			this.ceMultiValue.Properties.Caption = resources.GetString("ceMultiValue.Properties.Caption");
			resources.ApplyResources(this.lbDescription, "lbDescription");
			this.lbDescription.Name = "lbDescription";
			resources.ApplyResources(this.lbType, "lbType");
			this.lbType.Name = "lbType";
			resources.ApplyResources(this.lbDefaultValue, "lbDefaultValue");
			this.lbDefaultValue.Name = "lbDefaultValue";
			resources.ApplyResources(this.btCancel, "btCancel");
			this.btCancel.CausesValidation = false;
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Name = "btCancel";
			resources.ApplyResources(this.btOK, "btOK");
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Name = "btOK";
			resources.ApplyResources(this.panelLookUpSettings, "panelLookUpSettings");
			this.panelLookUpSettings.Controls.Add(this.tabControl);
			this.panelLookUpSettings.Name = "panelLookUpSettings";
			resources.ApplyResources(this.cbType, "cbType");
			this.cbType.Name = "cbType";
			this.cbType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbType.Properties.Buttons"))))});
			this.cbType.Properties.DropDownRows = 10;
			resources.ApplyResources(this.teDefaultValue, "teDefaultValue");
			this.teDefaultValue.Name = "teDefaultValue";
			this.AcceptButton = this.btOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btCancel;
			this.Controls.Add(this.teDefaultValue);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.ceMultiValue);
			this.Controls.Add(this.ceShowAtParametersPanel);
			this.Controls.Add(this.ceStandardValues);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.teDescription);
			this.Controls.Add(this.teName);
			this.Controls.Add(this.lbDefaultValue);
			this.Controls.Add(this.lbType);
			this.Controls.Add(this.lbDescription);
			this.Controls.Add(this.lbName);
			this.Controls.Add(this.panelLookUpSettings);
			this.Controls.Add(this.cbType);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewParameterEditorForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.teName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teDescription.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.pageStatic.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.staticItemsTreeList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceMultiValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowAtParametersPanel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceStandardValues.Properties)).EndInit();
			this.panelLookUpSettings.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teDefaultValue.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.LabelControl lbName;
		private DevExpress.XtraEditors.TextEdit teName;
		private DevExpress.XtraEditors.TextEdit teDescription;
		private DevExpress.XtraTab.XtraTabControl tabControl;
		private DevExpress.XtraTab.XtraTabPage pageDynamic;
		private DevExpress.XtraTab.XtraTabPage pageStatic;
		private DevExpress.XtraEditors.CheckEdit ceShowAtParametersPanel;
		private DevExpress.XtraEditors.CheckEdit ceStandardValues;
		private DevExpress.XtraEditors.CheckEdit ceMultiValue;
		private DevExpress.XtraEditors.LabelControl lbDescription;
		private DevExpress.XtraEditors.LabelControl lbType;
		private DevExpress.XtraEditors.LabelControl lbDefaultValue;
		private DevExpress.XtraEditors.SimpleButton btCancel;
		private DevExpress.XtraEditors.SimpleButton btOK;
		private System.Windows.Forms.Panel panelLookUpSettings;
		private DevExpress.XtraEditors.ComboBoxEdit cbType;
		private DevExpress.XtraTreeList.TreeList staticItemsTreeList;
		private DevExpress.XtraTreeList.Columns.TreeListColumn columnValue;
		private DevExpress.XtraTreeList.Columns.TreeListColumn columnDescription;
		private DevExpress.XtraEditors.DataNavigator dataNavigator1;
		private XtraEditors.TextEdit teDefaultValue;
	}
}
