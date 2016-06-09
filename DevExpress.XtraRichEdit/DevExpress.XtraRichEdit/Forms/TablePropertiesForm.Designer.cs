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

using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Forms {
	partial class TablePropertiesForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TablePropertiesForm));
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tableTab = new DevExpress.XtraTab.XtraTabPage();
			this.btnBorder = new DevExpress.XtraEditors.SimpleButton();
			this.tableSizeControl = new DevExpress.XtraRichEdit.Design.TableSizeControl();
			this.rgTableAlignment = new DevExpress.XtraEditors.RadioGroup();
			this.lblIndentFromLeft = new DevExpress.XtraEditors.LabelControl();
			this.spnIndentFromLeft = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblTableAlignment = new DevExpress.XtraEditors.LabelControl();
			this.btnTableOptions = new DevExpress.XtraEditors.SimpleButton();
			this.lblTableSize = new DevExpress.XtraEditors.LabelControl();
			this.rowTab = new DevExpress.XtraTab.XtraTabPage();
			this.tableRowHeightControl = new DevExpress.XtraRichEdit.Forms.Design.TableRowHeightControl();
			this.btnNextRow = new DevExpress.XtraEditors.SimpleButton();
			this.btnPreviousRow = new DevExpress.XtraEditors.SimpleButton();
			this.chkHeader = new DevExpress.XtraEditors.CheckEdit();
			this.chkCantSplit = new DevExpress.XtraEditors.CheckEdit();
			this.lblRowNumber = new DevExpress.XtraEditors.LabelControl();
			this.lblRowSize = new DevExpress.XtraEditors.LabelControl();
			this.lblRowOptions = new DevExpress.XtraEditors.LabelControl();
			this.columnTab = new DevExpress.XtraTab.XtraTabPage();
			this.columnSizeControl = new DevExpress.XtraRichEdit.Design.TableSizeControl();
			this.btnNextColumn = new DevExpress.XtraEditors.SimpleButton();
			this.btnPreviousColumn = new DevExpress.XtraEditors.SimpleButton();
			this.lblColumn = new DevExpress.XtraEditors.LabelControl();
			this.lblColumnSize = new DevExpress.XtraEditors.LabelControl();
			this.cellTab = new DevExpress.XtraTab.XtraTabPage();
			this.cellSizeControl = new DevExpress.XtraRichEdit.Design.TableSizeControl();
			this.rgCellVerticalAlignment = new DevExpress.XtraEditors.RadioGroup();
			this.btnCellOptions = new DevExpress.XtraEditors.SimpleButton();
			this.lblCellVerticalAlighment = new DevExpress.XtraEditors.LabelControl();
			this.lblCellSize = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.tableTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rgTableAlignment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnIndentFromLeft.Properties)).BeginInit();
			this.rowTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkHeader.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCantSplit.Properties)).BeginInit();
			this.columnTab.SuspendLayout();
			this.cellTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rgCellVerticalAlignment.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.tableTab;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tableTab,
			this.rowTab,
			this.columnTab,
			this.cellTab});
			this.tableTab.Controls.Add(this.btnBorder);
			this.tableTab.Controls.Add(this.tableSizeControl);
			this.tableTab.Controls.Add(this.rgTableAlignment);
			this.tableTab.Controls.Add(this.lblIndentFromLeft);
			this.tableTab.Controls.Add(this.spnIndentFromLeft);
			this.tableTab.Controls.Add(this.lblTableAlignment);
			this.tableTab.Controls.Add(this.btnTableOptions);
			this.tableTab.Controls.Add(this.lblTableSize);
			this.tableTab.Name = "tableTab";
			resources.ApplyResources(this.tableTab, "tableTab");
			resources.ApplyResources(this.btnBorder, "btnBorder");
			this.btnBorder.Name = "btnBorder";
			this.btnBorder.Click += new System.EventHandler(this.btnBorder_Click);
			resources.ApplyResources(this.tableSizeControl, "tableSizeControl");
			this.tableSizeControl.Name = "tableSizeControl";
			resources.ApplyResources(this.rgTableAlignment, "rgTableAlignment");
			this.rgTableAlignment.Name = "rgTableAlignment";
			this.rgTableAlignment.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgTableAlignment.Properties.Appearance.BackColor")));
			this.rgTableAlignment.Properties.Appearance.Options.UseBackColor = true;
			this.rgTableAlignment.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgTableAlignment.Properties.Columns = 3;
			this.rgTableAlignment.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgTableAlignment.Properties.Items")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgTableAlignment.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgTableAlignment.Properties.Items2"))});
			this.lblIndentFromLeft.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblIndentFromLeft, "lblIndentFromLeft");
			this.lblIndentFromLeft.Name = "lblIndentFromLeft";
			resources.ApplyResources(this.spnIndentFromLeft, "spnIndentFromLeft");
			this.spnIndentFromLeft.Name = "spnIndentFromLeft";
			this.spnIndentFromLeft.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnIndentFromLeft.Properties.IsValueInPercent = false;
			this.lblTableAlignment.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblTableAlignment, "lblTableAlignment");
			this.lblTableAlignment.LineVisible = true;
			this.lblTableAlignment.Name = "lblTableAlignment";
			resources.ApplyResources(this.btnTableOptions, "btnTableOptions");
			this.btnTableOptions.CausesValidation = false;
			this.btnTableOptions.Name = "btnTableOptions";
			this.btnTableOptions.Click += new System.EventHandler(this.OnBtnTableOptionsClick);
			this.lblTableSize.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblTableSize, "lblTableSize");
			this.lblTableSize.LineVisible = true;
			this.lblTableSize.Name = "lblTableSize";
			this.rowTab.Controls.Add(this.tableRowHeightControl);
			this.rowTab.Controls.Add(this.btnNextRow);
			this.rowTab.Controls.Add(this.btnPreviousRow);
			this.rowTab.Controls.Add(this.chkHeader);
			this.rowTab.Controls.Add(this.chkCantSplit);
			this.rowTab.Controls.Add(this.lblRowNumber);
			this.rowTab.Controls.Add(this.lblRowSize);
			this.rowTab.Controls.Add(this.lblRowOptions);
			this.rowTab.Name = "rowTab";
			resources.ApplyResources(this.rowTab, "rowTab");
			resources.ApplyResources(this.tableRowHeightControl, "tableRowHeightControl");
			this.tableRowHeightControl.Name = "tableRowHeightControl";
			resources.ApplyResources(this.btnNextRow, "btnNextRow");
			this.btnNextRow.CausesValidation = false;
			this.btnNextRow.Name = "btnNextRow";
			resources.ApplyResources(this.btnPreviousRow, "btnPreviousRow");
			this.btnPreviousRow.CausesValidation = false;
			this.btnPreviousRow.Name = "btnPreviousRow";
			resources.ApplyResources(this.chkHeader, "chkHeader");
			this.chkHeader.Name = "chkHeader";
			this.chkHeader.Properties.AutoWidth = true;
			this.chkHeader.Properties.Caption = resources.GetString("chkHeader.Properties.Caption");
			resources.ApplyResources(this.chkCantSplit, "chkCantSplit");
			this.chkCantSplit.Name = "chkCantSplit";
			this.chkCantSplit.Properties.AutoWidth = true;
			this.chkCantSplit.Properties.Caption = resources.GetString("chkCantSplit.Properties.Caption");
			this.lblRowNumber.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblRowNumber, "lblRowNumber");
			this.lblRowNumber.Name = "lblRowNumber";
			this.lblRowSize.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblRowSize, "lblRowSize");
			this.lblRowSize.LineVisible = true;
			this.lblRowSize.Name = "lblRowSize";
			this.lblRowOptions.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblRowOptions, "lblRowOptions");
			this.lblRowOptions.LineVisible = true;
			this.lblRowOptions.Name = "lblRowOptions";
			this.columnTab.Controls.Add(this.columnSizeControl);
			this.columnTab.Controls.Add(this.btnNextColumn);
			this.columnTab.Controls.Add(this.btnPreviousColumn);
			this.columnTab.Controls.Add(this.lblColumn);
			this.columnTab.Controls.Add(this.lblColumnSize);
			this.columnTab.Name = "columnTab";
			resources.ApplyResources(this.columnTab, "columnTab");
			resources.ApplyResources(this.columnSizeControl, "columnSizeControl");
			this.columnSizeControl.Name = "columnSizeControl";
			resources.ApplyResources(this.btnNextColumn, "btnNextColumn");
			this.btnNextColumn.CausesValidation = false;
			this.btnNextColumn.Name = "btnNextColumn";
			resources.ApplyResources(this.btnPreviousColumn, "btnPreviousColumn");
			this.btnPreviousColumn.CausesValidation = false;
			this.btnPreviousColumn.Name = "btnPreviousColumn";
			this.lblColumn.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblColumn, "lblColumn");
			this.lblColumn.Name = "lblColumn";
			this.lblColumnSize.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblColumnSize, "lblColumnSize");
			this.lblColumnSize.LineVisible = true;
			this.lblColumnSize.Name = "lblColumnSize";
			this.cellTab.Controls.Add(this.cellSizeControl);
			this.cellTab.Controls.Add(this.rgCellVerticalAlignment);
			this.cellTab.Controls.Add(this.btnCellOptions);
			this.cellTab.Controls.Add(this.lblCellVerticalAlighment);
			this.cellTab.Controls.Add(this.lblCellSize);
			this.cellTab.Name = "cellTab";
			resources.ApplyResources(this.cellTab, "cellTab");
			resources.ApplyResources(this.cellSizeControl, "cellSizeControl");
			this.cellSizeControl.Name = "cellSizeControl";
			resources.ApplyResources(this.rgCellVerticalAlignment, "rgCellVerticalAlignment");
			this.rgCellVerticalAlignment.Name = "rgCellVerticalAlignment";
			this.rgCellVerticalAlignment.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgCellVerticalAlignment.Properties.Appearance.BackColor")));
			this.rgCellVerticalAlignment.Properties.Appearance.Options.UseBackColor = true;
			this.rgCellVerticalAlignment.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgCellVerticalAlignment.Properties.Columns = 3;
			this.rgCellVerticalAlignment.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgCellVerticalAlignment.Properties.Items")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgCellVerticalAlignment.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgCellVerticalAlignment.Properties.Items2"))});
			resources.ApplyResources(this.btnCellOptions, "btnCellOptions");
			this.btnCellOptions.CausesValidation = false;
			this.btnCellOptions.Name = "btnCellOptions";
			this.btnCellOptions.Click += new System.EventHandler(this.OnBtnCellOptionsClick);
			this.lblCellVerticalAlighment.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblCellVerticalAlighment, "lblCellVerticalAlighment");
			this.lblCellVerticalAlighment.LineVisible = true;
			this.lblCellVerticalAlighment.Name = "lblCellVerticalAlighment";
			this.lblCellSize.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblCellSize, "lblCellSize");
			this.lblCellSize.LineVisible = true;
			this.lblCellSize.Name = "lblCellSize";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnBtnOkClick);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TablePropertiesForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.tableTab.ResumeLayout(false);
			this.tableTab.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.rgTableAlignment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnIndentFromLeft.Properties)).EndInit();
			this.rowTab.ResumeLayout(false);
			this.rowTab.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkHeader.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCantSplit.Properties)).EndInit();
			this.columnTab.ResumeLayout(false);
			this.columnTab.PerformLayout();
			this.cellTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rgCellVerticalAlignment.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraTab.XtraTabControl tabControl;
		protected DevExpress.XtraTab.XtraTabPage tableTab;
		protected DevExpress.XtraTab.XtraTabPage rowTab;
		protected DevExpress.XtraTab.XtraTabPage columnTab;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.LabelControl lblTableSize;
		protected DevExpress.XtraEditors.SimpleButton btnTableOptions;
		protected DevExpress.XtraEditors.LabelControl lblTableAlignment;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnIndentFromLeft;
		protected DevExpress.XtraEditors.LabelControl lblIndentFromLeft;
		protected DevExpress.XtraEditors.LabelControl lblRowOptions;
		protected DevExpress.XtraEditors.LabelControl lblRowNumber;
		protected DevExpress.XtraEditors.LabelControl lblRowSize;
		protected DevExpress.XtraEditors.CheckEdit chkHeader;
		protected DevExpress.XtraEditors.CheckEdit chkCantSplit;
		protected DevExpress.XtraEditors.SimpleButton btnPreviousRow;
		protected DevExpress.XtraEditors.SimpleButton btnNextRow;
		protected DevExpress.XtraEditors.LabelControl lblColumn;
		protected DevExpress.XtraEditors.LabelControl lblColumnSize;
		protected DevExpress.XtraEditors.SimpleButton btnNextColumn;
		protected DevExpress.XtraEditors.SimpleButton btnPreviousColumn;
		protected DevExpress.XtraTab.XtraTabPage cellTab;
		protected DevExpress.XtraEditors.SimpleButton btnCellOptions;
		protected DevExpress.XtraEditors.LabelControl lblCellVerticalAlighment;
		protected DevExpress.XtraEditors.LabelControl lblCellSize;
		protected DevExpress.XtraEditors.RadioGroup rgTableAlignment;
		protected DevExpress.XtraEditors.RadioGroup rgCellVerticalAlignment;
		protected DevExpress.XtraRichEdit.Design.TableSizeControl tableSizeControl;
		protected DevExpress.XtraRichEdit.Forms.Design.TableRowHeightControl tableRowHeightControl;
		protected DevExpress.XtraRichEdit.Design.TableSizeControl columnSizeControl;
		protected DevExpress.XtraRichEdit.Design.TableSizeControl cellSizeControl;
		private XtraEditors.SimpleButton btnBorder;
	}
}
