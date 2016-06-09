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
	partial class FieldSettingsPivotTableForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FieldSettingsPivotTableForm));
			this.lblSourceName = new DevExpress.XtraEditors.LabelControl();
			this.lblSourceNameValue = new DevExpress.XtraEditors.LabelControl();
			this.lblCustomName = new DevExpress.XtraEditors.LabelControl();
			this.edtCustomName = new DevExpress.XtraEditors.TextEdit();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabSubtotalsAndFilters = new DevExpress.XtraTab.XtraTabPage();
			this.chkCustomSubtotal = new DevExpress.XtraEditors.CheckEdit();
			this.chkNoneSubtotal = new DevExpress.XtraEditors.CheckEdit();
			this.chkAutomaticSubtotal = new DevExpress.XtraEditors.CheckEdit();
			this.chkIncludeNewItems = new DevExpress.XtraEditors.CheckEdit();
			this.lblSeperator2 = new DevExpress.XtraEditors.LabelControl();
			this.lblFilter = new DevExpress.XtraEditors.LabelControl();
			this.lbFunctions = new DevExpress.XtraEditors.ListBoxControl();
			this.lblSelectFunctions = new DevExpress.XtraEditors.LabelControl();
			this.lblSeparator = new DevExpress.XtraEditors.LabelControl();
			this.lblSubtotals = new DevExpress.XtraEditors.LabelControl();
			this.xtraTabLayoutAndPrint = new DevExpress.XtraTab.XtraTabPage();
			this.chkInsertPageBreak = new DevExpress.XtraEditors.CheckEdit();
			this.lblSeparator4 = new DevExpress.XtraEditors.LabelControl();
			this.lblPrint = new DevExpress.XtraEditors.LabelControl();
			this.chkShowItemWithNoData = new DevExpress.XtraEditors.CheckEdit();
			this.chkInsertBlankLine = new DevExpress.XtraEditors.CheckEdit();
			this.chkRepeatItemLabels = new DevExpress.XtraEditors.CheckEdit();
			this.chkShowItemLabelsTabularForm = new DevExpress.XtraEditors.CheckEdit();
			this.chkSubtotalTop = new DevExpress.XtraEditors.CheckEdit();
			this.chkCompactForm = new DevExpress.XtraEditors.CheckEdit();
			this.lblSeparator3 = new DevExpress.XtraEditors.LabelControl();
			this.lblLayout = new DevExpress.XtraEditors.LabelControl();
			this.chkShowItemLabelsOutlineForm = new DevExpress.XtraEditors.CheckEdit();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.edtCustomName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.xtraTabSubtotalsAndFilters.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkCustomSubtotal.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkNoneSubtotal.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAutomaticSubtotal.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkIncludeNewItems.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFunctions)).BeginInit();
			this.xtraTabLayoutAndPrint.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkInsertPageBreak.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowItemWithNoData.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkInsertBlankLine.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkRepeatItemLabels.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowItemLabelsTabularForm.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkSubtotalTop.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCompactForm.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowItemLabelsOutlineForm.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblSourceName, "lblSourceName");
			this.lblSourceName.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblSourceName.Name = "lblSourceName";
			resources.ApplyResources(this.lblSourceNameValue, "lblSourceNameValue");
			this.lblSourceNameValue.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this.lblSourceNameValue.Name = "lblSourceNameValue";
			resources.ApplyResources(this.lblCustomName, "lblCustomName");
			this.lblCustomName.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblCustomName.Name = "lblCustomName";
			resources.ApplyResources(this.edtCustomName, "edtCustomName");
			this.edtCustomName.Name = "edtCustomName";
			this.edtCustomName.Properties.AccessibleName = resources.GetString("edtCustomName.Properties.AccessibleName");
			this.edtCustomName.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.xtraTabSubtotalsAndFilters;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.xtraTabSubtotalsAndFilters,
			this.xtraTabLayoutAndPrint});
			this.xtraTabSubtotalsAndFilters.Controls.Add(this.chkCustomSubtotal);
			this.xtraTabSubtotalsAndFilters.Controls.Add(this.chkNoneSubtotal);
			this.xtraTabSubtotalsAndFilters.Controls.Add(this.chkAutomaticSubtotal);
			this.xtraTabSubtotalsAndFilters.Controls.Add(this.chkIncludeNewItems);
			this.xtraTabSubtotalsAndFilters.Controls.Add(this.lblSeperator2);
			this.xtraTabSubtotalsAndFilters.Controls.Add(this.lblFilter);
			this.xtraTabSubtotalsAndFilters.Controls.Add(this.lbFunctions);
			this.xtraTabSubtotalsAndFilters.Controls.Add(this.lblSelectFunctions);
			this.xtraTabSubtotalsAndFilters.Controls.Add(this.lblSeparator);
			this.xtraTabSubtotalsAndFilters.Controls.Add(this.lblSubtotals);
			this.xtraTabSubtotalsAndFilters.Name = "xtraTabSubtotalsAndFilters";
			resources.ApplyResources(this.xtraTabSubtotalsAndFilters, "xtraTabSubtotalsAndFilters");
			resources.ApplyResources(this.chkCustomSubtotal, "chkCustomSubtotal");
			this.chkCustomSubtotal.Name = "chkCustomSubtotal";
			this.chkCustomSubtotal.Properties.AccessibleName = resources.GetString("chkCustomSubtotal.Properties.AccessibleName");
			this.chkCustomSubtotal.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkCustomSubtotal.Properties.Caption = resources.GetString("chkCustomSubtotal.Properties.Caption");
			this.chkCustomSubtotal.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			resources.ApplyResources(this.chkNoneSubtotal, "chkNoneSubtotal");
			this.chkNoneSubtotal.Name = "chkNoneSubtotal";
			this.chkNoneSubtotal.Properties.AccessibleName = resources.GetString("chkNoneSubtotal.Properties.AccessibleName");
			this.chkNoneSubtotal.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkNoneSubtotal.Properties.Caption = resources.GetString("chkNoneSubtotal.Properties.Caption");
			this.chkNoneSubtotal.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			resources.ApplyResources(this.chkAutomaticSubtotal, "chkAutomaticSubtotal");
			this.chkAutomaticSubtotal.Name = "chkAutomaticSubtotal";
			this.chkAutomaticSubtotal.Properties.AccessibleName = resources.GetString("chkAutomaticSubtotal.Properties.AccessibleName");
			this.chkAutomaticSubtotal.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkAutomaticSubtotal.Properties.Caption = resources.GetString("chkAutomaticSubtotal.Properties.Caption");
			this.chkAutomaticSubtotal.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			resources.ApplyResources(this.chkIncludeNewItems, "chkIncludeNewItems");
			this.chkIncludeNewItems.Name = "chkIncludeNewItems";
			this.chkIncludeNewItems.Properties.Caption = resources.GetString("chkIncludeNewItems.Properties.Caption");
			this.lblSeperator2.AccessibleRole = System.Windows.Forms.AccessibleRole.Separator;
			resources.ApplyResources(this.lblSeperator2, "lblSeperator2");
			this.lblSeperator2.LineVisible = true;
			this.lblSeperator2.Name = "lblSeperator2";
			resources.ApplyResources(this.lblFilter, "lblFilter");
			this.lblFilter.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblFilter.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblFilter.Appearance.Font")));
			this.lblFilter.Name = "lblFilter";
			resources.ApplyResources(this.lbFunctions, "lbFunctions");
			this.lbFunctions.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbFunctions.Name = "lbFunctions";
			this.lbFunctions.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
			resources.ApplyResources(this.lblSelectFunctions, "lblSelectFunctions");
			this.lblSelectFunctions.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblSelectFunctions.Name = "lblSelectFunctions";
			this.lblSeparator.AccessibleRole = System.Windows.Forms.AccessibleRole.Separator;
			resources.ApplyResources(this.lblSeparator, "lblSeparator");
			this.lblSeparator.LineVisible = true;
			this.lblSeparator.Name = "lblSeparator";
			resources.ApplyResources(this.lblSubtotals, "lblSubtotals");
			this.lblSubtotals.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblSubtotals.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblSubtotals.Appearance.Font")));
			this.lblSubtotals.Name = "lblSubtotals";
			this.xtraTabLayoutAndPrint.Controls.Add(this.chkInsertPageBreak);
			this.xtraTabLayoutAndPrint.Controls.Add(this.lblSeparator4);
			this.xtraTabLayoutAndPrint.Controls.Add(this.lblPrint);
			this.xtraTabLayoutAndPrint.Controls.Add(this.chkShowItemWithNoData);
			this.xtraTabLayoutAndPrint.Controls.Add(this.chkInsertBlankLine);
			this.xtraTabLayoutAndPrint.Controls.Add(this.chkRepeatItemLabels);
			this.xtraTabLayoutAndPrint.Controls.Add(this.chkShowItemLabelsTabularForm);
			this.xtraTabLayoutAndPrint.Controls.Add(this.chkSubtotalTop);
			this.xtraTabLayoutAndPrint.Controls.Add(this.chkCompactForm);
			this.xtraTabLayoutAndPrint.Controls.Add(this.lblSeparator3);
			this.xtraTabLayoutAndPrint.Controls.Add(this.lblLayout);
			this.xtraTabLayoutAndPrint.Controls.Add(this.chkShowItemLabelsOutlineForm);
			this.xtraTabLayoutAndPrint.Name = "xtraTabLayoutAndPrint";
			resources.ApplyResources(this.xtraTabLayoutAndPrint, "xtraTabLayoutAndPrint");
			resources.ApplyResources(this.chkInsertPageBreak, "chkInsertPageBreak");
			this.chkInsertPageBreak.Name = "chkInsertPageBreak";
			this.chkInsertPageBreak.Properties.Caption = resources.GetString("chkInsertPageBreak.Properties.Caption");
			this.lblSeparator4.AccessibleRole = System.Windows.Forms.AccessibleRole.Separator;
			resources.ApplyResources(this.lblSeparator4, "lblSeparator4");
			this.lblSeparator4.LineVisible = true;
			this.lblSeparator4.Name = "lblSeparator4";
			resources.ApplyResources(this.lblPrint, "lblPrint");
			this.lblPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPrint.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblPrint.Appearance.Font")));
			this.lblPrint.Name = "lblPrint";
			resources.ApplyResources(this.chkShowItemWithNoData, "chkShowItemWithNoData");
			this.chkShowItemWithNoData.Name = "chkShowItemWithNoData";
			this.chkShowItemWithNoData.Properties.Caption = resources.GetString("chkShowItemWithNoData.Properties.Caption");
			resources.ApplyResources(this.chkInsertBlankLine, "chkInsertBlankLine");
			this.chkInsertBlankLine.Name = "chkInsertBlankLine";
			this.chkInsertBlankLine.Properties.AccessibleName = resources.GetString("chkInsertBlankLine.Properties.AccessibleName");
			this.chkInsertBlankLine.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkInsertBlankLine.Properties.Caption = resources.GetString("chkInsertBlankLine.Properties.Caption");
			resources.ApplyResources(this.chkRepeatItemLabels, "chkRepeatItemLabels");
			this.chkRepeatItemLabels.Name = "chkRepeatItemLabels";
			this.chkRepeatItemLabels.Properties.AccessibleName = resources.GetString("chkRepeatItemLabels.Properties.AccessibleName");
			this.chkRepeatItemLabels.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkRepeatItemLabels.Properties.Caption = resources.GetString("chkRepeatItemLabels.Properties.Caption");
			resources.ApplyResources(this.chkShowItemLabelsTabularForm, "chkShowItemLabelsTabularForm");
			this.chkShowItemLabelsTabularForm.Name = "chkShowItemLabelsTabularForm";
			this.chkShowItemLabelsTabularForm.Properties.AccessibleName = resources.GetString("chkShowItemLabelsTabularForm.Properties.AccessibleName");
			this.chkShowItemLabelsTabularForm.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkShowItemLabelsTabularForm.Properties.Caption = resources.GetString("chkShowItemLabelsTabularForm.Properties.Caption");
			this.chkShowItemLabelsTabularForm.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			resources.ApplyResources(this.chkSubtotalTop, "chkSubtotalTop");
			this.chkSubtotalTop.Name = "chkSubtotalTop";
			this.chkSubtotalTop.Properties.AccessibleName = resources.GetString("chkSubtotalTop.Properties.AccessibleName");
			this.chkSubtotalTop.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkSubtotalTop.Properties.Caption = resources.GetString("chkSubtotalTop.Properties.Caption");
			resources.ApplyResources(this.chkCompactForm, "chkCompactForm");
			this.chkCompactForm.Name = "chkCompactForm";
			this.chkCompactForm.Properties.AccessibleName = resources.GetString("chkCompactForm.Properties.AccessibleName");
			this.chkCompactForm.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkCompactForm.Properties.Caption = resources.GetString("chkCompactForm.Properties.Caption");
			this.lblSeparator3.AccessibleRole = System.Windows.Forms.AccessibleRole.Separator;
			resources.ApplyResources(this.lblSeparator3, "lblSeparator3");
			this.lblSeparator3.LineVisible = true;
			this.lblSeparator3.Name = "lblSeparator3";
			resources.ApplyResources(this.lblLayout, "lblLayout");
			this.lblLayout.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblLayout.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblLayout.Appearance.Font")));
			this.lblLayout.Name = "lblLayout";
			resources.ApplyResources(this.chkShowItemLabelsOutlineForm, "chkShowItemLabelsOutlineForm");
			this.chkShowItemLabelsOutlineForm.Name = "chkShowItemLabelsOutlineForm";
			this.chkShowItemLabelsOutlineForm.Properties.AccessibleName = resources.GetString("chkShowItemLabelsOutlineForm.Properties.AccessibleName");
			this.chkShowItemLabelsOutlineForm.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkShowItemLabelsOutlineForm.Properties.Caption = resources.GetString("chkShowItemLabelsOutlineForm.Properties.Caption");
			this.chkShowItemLabelsOutlineForm.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.edtCustomName);
			this.Controls.Add(this.lblCustomName);
			this.Controls.Add(this.lblSourceNameValue);
			this.Controls.Add(this.lblSourceName);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FieldSettingsPivotTableForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.edtCustomName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.xtraTabSubtotalsAndFilters.ResumeLayout(false);
			this.xtraTabSubtotalsAndFilters.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkCustomSubtotal.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkNoneSubtotal.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAutomaticSubtotal.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkIncludeNewItems.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFunctions)).EndInit();
			this.xtraTabLayoutAndPrint.ResumeLayout(false);
			this.xtraTabLayoutAndPrint.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkInsertPageBreak.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowItemWithNoData.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkInsertBlankLine.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkRepeatItemLabels.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowItemLabelsTabularForm.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkSubtotalTop.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCompactForm.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowItemLabelsOutlineForm.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl lblSourceName;
		private XtraEditors.LabelControl lblSourceNameValue;
		private XtraEditors.LabelControl lblCustomName;
		private XtraEditors.TextEdit edtCustomName;
		private XtraTab.XtraTabControl tabControl;
		private XtraTab.XtraTabPage xtraTabSubtotalsAndFilters;
		private XtraTab.XtraTabPage xtraTabLayoutAndPrint;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.CheckEdit chkIncludeNewItems;
		private XtraEditors.LabelControl lblSeperator2;
		private XtraEditors.LabelControl lblFilter;
		private XtraEditors.ListBoxControl lbFunctions;
		private XtraEditors.LabelControl lblSelectFunctions;
		private XtraEditors.LabelControl lblSeparator;
		private XtraEditors.LabelControl lblSubtotals;
		private XtraEditors.LabelControl lblSeparator3;
		private XtraEditors.LabelControl lblLayout;
		private XtraEditors.CheckEdit chkShowItemLabelsOutlineForm;
		private XtraEditors.CheckEdit chkInsertPageBreak;
		private XtraEditors.LabelControl lblSeparator4;
		private XtraEditors.LabelControl lblPrint;
		private XtraEditors.CheckEdit chkShowItemWithNoData;
		private XtraEditors.CheckEdit chkInsertBlankLine;
		private XtraEditors.CheckEdit chkRepeatItemLabels;
		private XtraEditors.CheckEdit chkShowItemLabelsTabularForm;
		private XtraEditors.CheckEdit chkSubtotalTop;
		private XtraEditors.CheckEdit chkCompactForm;
		private XtraEditors.CheckEdit chkCustomSubtotal;
		private XtraEditors.CheckEdit chkNoneSubtotal;
		private XtraEditors.CheckEdit chkAutomaticSubtotal;
	}
}
