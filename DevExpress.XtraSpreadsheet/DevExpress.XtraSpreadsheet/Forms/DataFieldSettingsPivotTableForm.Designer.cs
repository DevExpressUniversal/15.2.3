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

namespace DevExpress.XtraSpreadsheet.Forms {
	partial class DataFieldSettingsPivotTableForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataFieldSettingsPivotTableForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.edtCustomName = new DevExpress.XtraEditors.TextEdit();
			this.lblCustomName = new DevExpress.XtraEditors.LabelControl();
			this.lblSourceNameValue = new DevExpress.XtraEditors.LabelControl();
			this.lblSourceName = new DevExpress.XtraEditors.LabelControl();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabSummarizeValuesBy = new DevExpress.XtraTab.XtraTabPage();
			this.lbFunctions = new DevExpress.XtraEditors.ListBoxControl();
			this.lblTypeOfCalculation = new DevExpress.XtraEditors.LabelControl();
			this.lblSeparator = new DevExpress.XtraEditors.LabelControl();
			this.lblSummarizeValueFieldBy = new DevExpress.XtraEditors.LabelControl();
			this.xtraTabShowValuesAs = new DevExpress.XtraTab.XtraTabPage();
			this.drawPanel = new DevExpress.XtraEditors.PanelControl();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.lblBaseItem = new DevExpress.XtraEditors.LabelControl();
			this.lblBaseField = new DevExpress.XtraEditors.LabelControl();
			this.lbBaseField = new DevExpress.XtraEditors.ListBoxControl();
			this.lbBaseItem = new DevExpress.XtraEditors.ListBoxControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.edtShowValuesAs = new DevExpress.XtraEditors.LookUpEdit();
			this.lblSeparator3 = new DevExpress.XtraEditors.LabelControl();
			this.lblShowValuesAs = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.edtCustomName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.xtraTabSummarizeValuesBy.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbFunctions)).BeginInit();
			this.xtraTabShowValuesAs.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.drawPanel)).BeginInit();
			this.drawPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbBaseField)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbBaseItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtShowValuesAs.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.edtCustomName, "edtCustomName");
			this.edtCustomName.Name = "edtCustomName";
			this.edtCustomName.Properties.AccessibleName = resources.GetString("edtCustomName.Properties.AccessibleName");
			this.edtCustomName.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			resources.ApplyResources(this.lblCustomName, "lblCustomName");
			this.lblCustomName.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblCustomName.Name = "lblCustomName";
			resources.ApplyResources(this.lblSourceNameValue, "lblSourceNameValue");
			this.lblSourceNameValue.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this.lblSourceNameValue.Name = "lblSourceNameValue";
			resources.ApplyResources(this.lblSourceName, "lblSourceName");
			this.lblSourceName.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblSourceName.Name = "lblSourceName";
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.xtraTabSummarizeValuesBy;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.xtraTabSummarizeValuesBy,
			this.xtraTabShowValuesAs});
			this.xtraTabSummarizeValuesBy.Controls.Add(this.lbFunctions);
			this.xtraTabSummarizeValuesBy.Controls.Add(this.lblTypeOfCalculation);
			this.xtraTabSummarizeValuesBy.Controls.Add(this.lblSeparator);
			this.xtraTabSummarizeValuesBy.Controls.Add(this.lblSummarizeValueFieldBy);
			this.xtraTabSummarizeValuesBy.Name = "xtraTabSummarizeValuesBy";
			resources.ApplyResources(this.xtraTabSummarizeValuesBy, "xtraTabSummarizeValuesBy");
			resources.ApplyResources(this.lbFunctions, "lbFunctions");
			this.lbFunctions.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbFunctions.Name = "lbFunctions";
			resources.ApplyResources(this.lblTypeOfCalculation, "lblTypeOfCalculation");
			this.lblTypeOfCalculation.Name = "lblTypeOfCalculation";
			this.lblSeparator.AccessibleRole = System.Windows.Forms.AccessibleRole.Separator;
			resources.ApplyResources(this.lblSeparator, "lblSeparator");
			this.lblSeparator.LineVisible = true;
			this.lblSeparator.Name = "lblSeparator";
			resources.ApplyResources(this.lblSummarizeValueFieldBy, "lblSummarizeValueFieldBy");
			this.lblSummarizeValueFieldBy.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblSummarizeValueFieldBy.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblSummarizeValueFieldBy.Appearance.Font")));
			this.lblSummarizeValueFieldBy.Name = "lblSummarizeValueFieldBy";
			this.xtraTabShowValuesAs.Controls.Add(this.drawPanel);
			this.xtraTabShowValuesAs.Controls.Add(this.edtShowValuesAs);
			this.xtraTabShowValuesAs.Controls.Add(this.lblSeparator3);
			this.xtraTabShowValuesAs.Controls.Add(this.lblShowValuesAs);
			this.xtraTabShowValuesAs.Name = "xtraTabShowValuesAs";
			resources.ApplyResources(this.xtraTabShowValuesAs, "xtraTabShowValuesAs");
			resources.ApplyResources(this.drawPanel, "drawPanel");
			this.drawPanel.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("drawPanel.Appearance.BackColor")));
			this.drawPanel.Appearance.Options.UseBackColor = true;
			this.drawPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.drawPanel.Controls.Add(this.layoutControl1);
			this.drawPanel.Name = "drawPanel";
			this.layoutControl1.Controls.Add(this.lblBaseItem);
			this.layoutControl1.Controls.Add(this.lblBaseField);
			this.layoutControl1.Controls.Add(this.lbBaseField);
			this.layoutControl1.Controls.Add(this.lbBaseItem);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(419, 66, 1059, 732);
			this.layoutControl1.Root = this.layoutControlGroup1;
			resources.ApplyResources(this.lblBaseItem, "lblBaseItem");
			this.lblBaseItem.Name = "lblBaseItem";
			this.lblBaseItem.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lblBaseField, "lblBaseField");
			this.lblBaseField.Name = "lblBaseField";
			this.lblBaseField.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbBaseField, "lbBaseField");
			this.lbBaseField.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbBaseField.Name = "lbBaseField";
			this.lbBaseField.StyleController = this.layoutControl1;
			this.lbBaseField.SelectedValueChanged += new System.EventHandler(this.lbBaseField_SelectedValueChanged);
			resources.ApplyResources(this.lbBaseItem, "lbBaseItem");
			this.lbBaseItem.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbBaseItem.Name = "lbBaseItem";
			this.lbBaseItem.StyleController = this.layoutControl1;
			this.lbBaseItem.SelectedValueChanged += new System.EventHandler(this.lbBaseItem_SelectedValueChanged);
			this.lbBaseItem.EnabledChanged += new System.EventHandler(this.lbBaseItem_EnabledChanged);
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem7,
			this.layoutControlItem8,
			this.layoutControlItem1,
			this.layoutControlItem2,
			this.emptySpaceItem1,
			this.emptySpaceItem2});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(340, 105);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlItem7.Control = this.lbBaseItem;
			this.layoutControlItem7.Location = new System.Drawing.Point(170, 17);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Size = new System.Drawing.Size(170, 88);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.layoutControlItem8.Control = this.lbBaseField;
			this.layoutControlItem8.Location = new System.Drawing.Point(0, 17);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.Size = new System.Drawing.Size(170, 88);
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.layoutControlItem1.Control = this.lblBaseField;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(54, 17);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem2.Control = this.lblBaseItem;
			this.layoutControlItem2.Location = new System.Drawing.Point(170, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(54, 17);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(54, 0);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(116, 17);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(224, 0);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(116, 17);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			resources.ApplyResources(this.edtShowValuesAs, "edtShowValuesAs");
			this.edtShowValuesAs.Name = "edtShowValuesAs";
			this.edtShowValuesAs.Properties.AccessibleName = resources.GetString("edtShowValuesAs.Properties.AccessibleName");
			this.edtShowValuesAs.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtShowValuesAs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtShowValuesAs.Properties.Buttons"))))});
			this.edtShowValuesAs.Properties.NullText = resources.GetString("edtShowValuesAs.Properties.NullText");
			this.edtShowValuesAs.Properties.ShowFooter = false;
			this.edtShowValuesAs.Properties.ShowHeader = false;
			this.lblSeparator3.AccessibleRole = System.Windows.Forms.AccessibleRole.Separator;
			resources.ApplyResources(this.lblSeparator3, "lblSeparator3");
			this.lblSeparator3.LineVisible = true;
			this.lblSeparator3.Name = "lblSeparator3";
			resources.ApplyResources(this.lblShowValuesAs, "lblShowValuesAs");
			this.lblShowValuesAs.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblShowValuesAs.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblShowValuesAs.Appearance.Font")));
			this.lblShowValuesAs.Name = "lblShowValuesAs";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.edtCustomName);
			this.Controls.Add(this.lblCustomName);
			this.Controls.Add(this.lblSourceNameValue);
			this.Controls.Add(this.lblSourceName);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataFieldSettingsPivotTableForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.edtCustomName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.xtraTabSummarizeValuesBy.ResumeLayout(false);
			this.xtraTabSummarizeValuesBy.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbFunctions)).EndInit();
			this.xtraTabShowValuesAs.ResumeLayout(false);
			this.xtraTabShowValuesAs.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.drawPanel)).EndInit();
			this.drawPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lbBaseField)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbBaseItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtShowValuesAs.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.TextEdit edtCustomName;
		private XtraEditors.LabelControl lblCustomName;
		private XtraEditors.LabelControl lblSourceNameValue;
		private XtraEditors.LabelControl lblSourceName;
		private XtraTab.XtraTabControl tabControl;
		private XtraTab.XtraTabPage xtraTabSummarizeValuesBy;
		private XtraEditors.ListBoxControl lbFunctions;
		private XtraEditors.LabelControl lblTypeOfCalculation;
		private XtraEditors.LabelControl lblSeparator;
		private XtraEditors.LabelControl lblSummarizeValueFieldBy;
		private XtraTab.XtraTabPage xtraTabShowValuesAs;
		private XtraEditors.LabelControl lblSeparator3;
		private XtraEditors.LabelControl lblShowValuesAs;
		private XtraEditors.LookUpEdit edtShowValuesAs;
		private XtraEditors.ListBoxControl lbBaseItem;
		private XtraEditors.ListBoxControl lbBaseField;
		private XtraEditors.PanelControl drawPanel;
		private XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.LayoutControlItem layoutControlItem7;
		private XtraLayout.LayoutControlItem layoutControlItem8;
		private XtraEditors.LabelControl lblBaseItem;
		private XtraEditors.LabelControl lblBaseField;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.EmptySpaceItem emptySpaceItem2;
	}
}
