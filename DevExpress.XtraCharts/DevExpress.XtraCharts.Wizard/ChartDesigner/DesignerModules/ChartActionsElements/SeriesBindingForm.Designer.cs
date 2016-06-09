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

namespace DevExpress.XtraCharts.Designer.Native {
	partial class SeriesBindingForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lvSeries = new DevExpress.XtraEditors.ImageListBoxControl();
			this.cbColor = new DevExpress.XtraEditors.PopupContainerEdit();
			this.beSummaryFunction = new DevExpress.XtraEditors.ButtonEdit();
			this.cbValue4 = new DevExpress.XtraEditors.PopupContainerEdit();
			this.cbValue3 = new DevExpress.XtraEditors.PopupContainerEdit();
			this.cbValue2 = new DevExpress.XtraEditors.PopupContainerEdit();
			this.cbValue1 = new DevExpress.XtraEditors.PopupContainerEdit();
			this.cbBindingMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbValueScaleType = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbArgument = new DevExpress.XtraEditors.PopupContainerEdit();
			this.cbArgumentScaleType = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbDataSource = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblDataSource = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.cbChartDataMember = new DevExpress.XtraEditors.PopupContainerEdit();
			this.lblDataMember = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.beFilters = new DevExpress.XtraEditors.ButtonEdit();
			this.lblDataFilters = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.grColorSettings = new DevExpress.XtraEditors.GroupControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.popupContainerEdit1 = new DevExpress.XtraEditors.PopupContainerEdit();
			this.lblColor = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.grValueSettings = new DevExpress.XtraEditors.GroupControl();
			this.pnlSummaryFunction = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.buttonEdit1 = new DevExpress.XtraEditors.ButtonEdit();
			this.lblSummaryFunction = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlValue4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.popupContainerEdit2 = new DevExpress.XtraEditors.PopupContainerEdit();
			this.lblValue4 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.lblDockOffset4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlValue3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.popupContainerEdit3 = new DevExpress.XtraEditors.PopupContainerEdit();
			this.lblValue3 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.lblDockOffset3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlValue2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.popupContainerEdit4 = new DevExpress.XtraEditors.PopupContainerEdit();
			this.lblValue2 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.lblDockOffset2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlValue1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.popupContainerEdit5 = new DevExpress.XtraEditors.PopupContainerEdit();
			this.lblValue1 = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlBindingMode = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblBindingMode = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.lblDockOffset1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlValueScaleType = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.comboBoxEdit2 = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblValueScaleType = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.grArgumentSettings = new DevExpress.XtraEditors.GroupControl();
			this.pnlArgument = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.popupContainerEdit6 = new DevExpress.XtraEditors.PopupContainerEdit();
			this.lblArgument = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlArgumentSplitter = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlArgumentScaleType = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.comboBoxEdit3 = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblArgumentScaleType = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.lvSeries)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.beSummaryFunction.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValue4.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValue3.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValue2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValue1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBindingMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValueScaleType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbArgument.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbArgumentScaleType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbDataSource.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbChartDataMember.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.beFilters.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grColorSettings)).BeginInit();
			this.grColorSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			this.chartPanelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grValueSettings)).BeginInit();
			this.grValueSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSummaryFunction)).BeginInit();
			this.pnlSummaryFunction.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.buttonEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlValue4)).BeginInit();
			this.pnlValue4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lblDockOffset4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlValue3)).BeginInit();
			this.pnlValue3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit3.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lblDockOffset3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlValue2)).BeginInit();
			this.pnlValue2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit4.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lblDockOffset2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlValue1)).BeginInit();
			this.pnlValue1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit5.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBindingMode)).BeginInit();
			this.pnlBindingMode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lblDockOffset1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlValueScaleType)).BeginInit();
			this.pnlValueScaleType.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grArgumentSettings)).BeginInit();
			this.grArgumentSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlArgument)).BeginInit();
			this.pnlArgument.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit6.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlArgumentSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlArgumentScaleType)).BeginInit();
			this.pnlArgumentScaleType.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit3.Properties)).BeginInit();
			this.SuspendLayout();
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(454, 470);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 82;
			this.btnOK.Text = "OK";
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(535, 470);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 83;
			this.btnCancel.Text = "Cancel";
			this.lvSeries.Location = new System.Drawing.Point(6, 12);
			this.lvSeries.Name = "lvSeries";
			this.lvSeries.Size = new System.Drawing.Size(257, 452);
			this.lvSeries.TabIndex = 90;
			this.lvSeries.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Exclamation;
			this.cbColor.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbColor.EditValue = "";
			this.cbColor.EnterMoveNextControl = true;
			this.cbColor.Location = new System.Drawing.Point(110, 0);
			this.cbColor.Name = "cbColor";
			this.cbColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbColor.Size = new System.Drawing.Size(207, 20);
			this.cbColor.TabIndex = 0;
			this.beSummaryFunction.Dock = System.Windows.Forms.DockStyle.Top;
			this.beSummaryFunction.Location = new System.Drawing.Point(110, 0);
			this.beSummaryFunction.Name = "beSummaryFunction";
			this.beSummaryFunction.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.beSummaryFunction.Properties.ValidateOnEnterKey = true;
			this.beSummaryFunction.Size = new System.Drawing.Size(207, 20);
			this.beSummaryFunction.TabIndex = 0;
			this.cbValue4.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbValue4.EditValue = "";
			this.cbValue4.EnterMoveNextControl = true;
			this.cbValue4.Location = new System.Drawing.Point(110, 0);
			this.cbValue4.Name = "cbValue4";
			this.cbValue4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbValue4.Size = new System.Drawing.Size(207, 20);
			this.cbValue4.TabIndex = 0;
			this.cbValue3.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbValue3.EditValue = "";
			this.cbValue3.EnterMoveNextControl = true;
			this.cbValue3.Location = new System.Drawing.Point(110, 0);
			this.cbValue3.Name = "cbValue3";
			this.cbValue3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbValue3.Size = new System.Drawing.Size(207, 20);
			this.cbValue3.TabIndex = 0;
			this.cbValue2.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbValue2.EditValue = "";
			this.cbValue2.EnterMoveNextControl = true;
			this.cbValue2.Location = new System.Drawing.Point(110, 0);
			this.cbValue2.Name = "cbValue2";
			this.cbValue2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbValue2.Size = new System.Drawing.Size(207, 20);
			this.cbValue2.TabIndex = 0;
			this.cbValue1.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbValue1.EditValue = "";
			this.cbValue1.EnterMoveNextControl = true;
			this.cbValue1.Location = new System.Drawing.Point(110, 0);
			this.cbValue1.Name = "cbValue1";
			this.cbValue1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbValue1.Size = new System.Drawing.Size(207, 20);
			this.cbValue1.TabIndex = 0;
			this.cbBindingMode.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbBindingMode.EditValue = "Values";
			this.cbBindingMode.EnterMoveNextControl = true;
			this.cbBindingMode.Location = new System.Drawing.Point(110, 0);
			this.cbBindingMode.Name = "cbBindingMode";
			this.cbBindingMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbBindingMode.Properties.Items.AddRange(new object[] {
			"Values",
			"Summary Function"});
			this.cbBindingMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbBindingMode.Size = new System.Drawing.Size(207, 20);
			this.cbBindingMode.TabIndex = 0;
			this.cbValueScaleType.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbValueScaleType.EditValue = "Numerical";
			this.cbValueScaleType.EnterMoveNextControl = true;
			this.cbValueScaleType.Location = new System.Drawing.Point(110, 0);
			this.cbValueScaleType.Name = "cbValueScaleType";
			this.cbValueScaleType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbValueScaleType.Properties.Items.AddRange(new object[] {
			"Numerical",
			"DateTime"});
			this.cbValueScaleType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbValueScaleType.Size = new System.Drawing.Size(207, 20);
			this.cbValueScaleType.TabIndex = 0;
			this.cbArgument.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbArgument.EditValue = "";
			this.cbArgument.EnterMoveNextControl = true;
			this.cbArgument.Location = new System.Drawing.Point(110, 0);
			this.cbArgument.Name = "cbArgument";
			this.cbArgument.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbArgument.Size = new System.Drawing.Size(207, 20);
			this.cbArgument.TabIndex = 0;
			this.cbArgumentScaleType.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbArgumentScaleType.EditValue = "Qualitative";
			this.cbArgumentScaleType.EnterMoveNextControl = true;
			this.cbArgumentScaleType.Location = new System.Drawing.Point(110, 0);
			this.cbArgumentScaleType.Name = "cbArgumentScaleType";
			this.cbArgumentScaleType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbArgumentScaleType.Properties.Items.AddRange(new object[] {
			"Qualitative",
			"Numerical",
			"DateTime",
			"Auto"});
			this.cbArgumentScaleType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbArgumentScaleType.Size = new System.Drawing.Size(207, 20);
			this.cbArgumentScaleType.TabIndex = 0;
			this.cbDataSource.EditValue = "";
			this.cbDataSource.Location = new System.Drawing.Point(391, 9);
			this.cbDataSource.Name = "cbDataSource";
			this.cbDataSource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbDataSource.Properties.Items.AddRange(new object[] {
			"Qualitative",
			"Numerical",
			"DateTime"});
			this.cbDataSource.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbDataSource.Size = new System.Drawing.Size(206, 20);
			this.cbDataSource.TabIndex = 91;
			this.lblDataSource.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblDataSource.Location = new System.Drawing.Point(281, 9);
			this.lblDataSource.Name = "lblDataSource";
			this.lblDataSource.Size = new System.Drawing.Size(110, 20);
			this.lblDataSource.TabIndex = 92;
			this.lblDataSource.Text = "Choose a datasource:";
			this.cbChartDataMember.EditValue = "";
			this.cbChartDataMember.EnterMoveNextControl = true;
			this.cbChartDataMember.Location = new System.Drawing.Point(391, 35);
			this.cbChartDataMember.Name = "cbChartDataMember";
			this.cbChartDataMember.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbChartDataMember.Properties.ReadOnly = true;
			this.cbChartDataMember.Size = new System.Drawing.Size(206, 20);
			this.cbChartDataMember.TabIndex = 95;
			this.lblDataMember.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblDataMember.Location = new System.Drawing.Point(281, 35);
			this.lblDataMember.Name = "lblDataMember";
			this.lblDataMember.Size = new System.Drawing.Size(110, 20);
			this.lblDataMember.TabIndex = 93;
			this.lblDataMember.Text = "Data Member:";
			this.beFilters.Location = new System.Drawing.Point(391, 61);
			this.beFilters.Name = "beFilters";
			this.beFilters.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.beFilters.Properties.ReadOnly = true;
			this.beFilters.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.beFilters.Size = new System.Drawing.Size(206, 20);
			this.beFilters.TabIndex = 96;
			this.lblDataFilters.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblDataFilters.Location = new System.Drawing.Point(281, 61);
			this.lblDataFilters.Name = "lblDataFilters";
			this.lblDataFilters.Size = new System.Drawing.Size(110, 20);
			this.lblDataFilters.TabIndex = 94;
			this.lblDataFilters.Text = "Data filters:";
			this.grColorSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.grColorSettings.Controls.Add(this.chartPanelControl2);
			this.grColorSettings.Location = new System.Drawing.Point(269, 377);
			this.grColorSettings.Name = "grColorSettings";
			this.grColorSettings.Padding = new System.Windows.Forms.Padding(10);
			this.grColorSettings.Size = new System.Drawing.Size(341, 63);
			this.grColorSettings.TabIndex = 99;
			this.grColorSettings.Text = "Color Properties";
			this.chartPanelControl2.AutoSize = true;
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl2.Controls.Add(this.popupContainerEdit1);
			this.chartPanelControl2.Controls.Add(this.lblColor);
			this.chartPanelControl2.Dock = System.Windows.Forms.DockStyle.Top;
			this.chartPanelControl2.Location = new System.Drawing.Point(12, 31);
			this.chartPanelControl2.Name = "chartPanelControl2";
			this.chartPanelControl2.Size = new System.Drawing.Size(317, 20);
			this.chartPanelControl2.TabIndex = 3;
			this.popupContainerEdit1.Dock = System.Windows.Forms.DockStyle.Top;
			this.popupContainerEdit1.EditValue = "";
			this.popupContainerEdit1.EnterMoveNextControl = true;
			this.popupContainerEdit1.Location = new System.Drawing.Point(110, 0);
			this.popupContainerEdit1.Name = "popupContainerEdit1";
			this.popupContainerEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.popupContainerEdit1.Size = new System.Drawing.Size(207, 20);
			this.popupContainerEdit1.TabIndex = 0;
			this.lblColor.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblColor.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblColor.Location = new System.Drawing.Point(0, 0);
			this.lblColor.Name = "lblColor";
			this.lblColor.Size = new System.Drawing.Size(110, 20);
			this.lblColor.TabIndex = 0;
			this.lblColor.Text = "Color:";
			this.grValueSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.grValueSettings.Controls.Add(this.pnlSummaryFunction);
			this.grValueSettings.Controls.Add(this.pnlValue4);
			this.grValueSettings.Controls.Add(this.lblDockOffset4);
			this.grValueSettings.Controls.Add(this.pnlValue3);
			this.grValueSettings.Controls.Add(this.lblDockOffset3);
			this.grValueSettings.Controls.Add(this.pnlValue2);
			this.grValueSettings.Controls.Add(this.lblDockOffset2);
			this.grValueSettings.Controls.Add(this.pnlValue1);
			this.grValueSettings.Controls.Add(this.chartPanelControl1);
			this.grValueSettings.Controls.Add(this.pnlBindingMode);
			this.grValueSettings.Controls.Add(this.lblDockOffset1);
			this.grValueSettings.Controls.Add(this.pnlValueScaleType);
			this.grValueSettings.Location = new System.Drawing.Point(269, 174);
			this.grValueSettings.Name = "grValueSettings";
			this.grValueSettings.Padding = new System.Windows.Forms.Padding(10);
			this.grValueSettings.Size = new System.Drawing.Size(341, 203);
			this.grValueSettings.TabIndex = 98;
			this.grValueSettings.Text = "Value Properties";
			this.pnlSummaryFunction.AutoSize = true;
			this.pnlSummaryFunction.BackColor = System.Drawing.Color.Transparent;
			this.pnlSummaryFunction.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSummaryFunction.Controls.Add(this.buttonEdit1);
			this.pnlSummaryFunction.Controls.Add(this.lblSummaryFunction);
			this.pnlSummaryFunction.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlSummaryFunction.Location = new System.Drawing.Point(12, 171);
			this.pnlSummaryFunction.Name = "pnlSummaryFunction";
			this.pnlSummaryFunction.Size = new System.Drawing.Size(317, 20);
			this.pnlSummaryFunction.TabIndex = 7;
			this.buttonEdit1.Dock = System.Windows.Forms.DockStyle.Top;
			this.buttonEdit1.Location = new System.Drawing.Point(110, 0);
			this.buttonEdit1.Name = "buttonEdit1";
			this.buttonEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.buttonEdit1.Properties.ValidateOnEnterKey = true;
			this.buttonEdit1.Size = new System.Drawing.Size(207, 20);
			this.buttonEdit1.TabIndex = 0;
			this.lblSummaryFunction.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblSummaryFunction.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblSummaryFunction.Location = new System.Drawing.Point(0, 0);
			this.lblSummaryFunction.Name = "lblSummaryFunction";
			this.lblSummaryFunction.Size = new System.Drawing.Size(110, 20);
			this.lblSummaryFunction.TabIndex = 0;
			this.lblSummaryFunction.Text = "Summary function:";
			this.pnlValue4.AutoSize = true;
			this.pnlValue4.BackColor = System.Drawing.Color.Transparent;
			this.pnlValue4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlValue4.Controls.Add(this.popupContainerEdit2);
			this.pnlValue4.Controls.Add(this.lblValue4);
			this.pnlValue4.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlValue4.Location = new System.Drawing.Point(12, 151);
			this.pnlValue4.Name = "pnlValue4";
			this.pnlValue4.Size = new System.Drawing.Size(317, 20);
			this.pnlValue4.TabIndex = 6;
			this.popupContainerEdit2.Dock = System.Windows.Forms.DockStyle.Top;
			this.popupContainerEdit2.EditValue = "";
			this.popupContainerEdit2.EnterMoveNextControl = true;
			this.popupContainerEdit2.Location = new System.Drawing.Point(110, 0);
			this.popupContainerEdit2.Name = "popupContainerEdit2";
			this.popupContainerEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.popupContainerEdit2.Size = new System.Drawing.Size(207, 20);
			this.popupContainerEdit2.TabIndex = 0;
			this.lblValue4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblValue4.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblValue4.Location = new System.Drawing.Point(0, 0);
			this.lblValue4.Name = "lblValue4";
			this.lblValue4.Size = new System.Drawing.Size(110, 20);
			this.lblValue4.TabIndex = 0;
			this.lblValue4.Text = "Value:";
			this.lblDockOffset4.BackColor = System.Drawing.Color.Transparent;
			this.lblDockOffset4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.lblDockOffset4.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblDockOffset4.Location = new System.Drawing.Point(12, 147);
			this.lblDockOffset4.Name = "lblDockOffset4";
			this.lblDockOffset4.Size = new System.Drawing.Size(317, 4);
			this.lblDockOffset4.TabIndex = 101;
			this.pnlValue3.AutoSize = true;
			this.pnlValue3.BackColor = System.Drawing.Color.Transparent;
			this.pnlValue3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlValue3.Controls.Add(this.popupContainerEdit3);
			this.pnlValue3.Controls.Add(this.lblValue3);
			this.pnlValue3.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlValue3.Location = new System.Drawing.Point(12, 127);
			this.pnlValue3.Name = "pnlValue3";
			this.pnlValue3.Size = new System.Drawing.Size(317, 20);
			this.pnlValue3.TabIndex = 5;
			this.popupContainerEdit3.Dock = System.Windows.Forms.DockStyle.Top;
			this.popupContainerEdit3.EditValue = "";
			this.popupContainerEdit3.EnterMoveNextControl = true;
			this.popupContainerEdit3.Location = new System.Drawing.Point(110, 0);
			this.popupContainerEdit3.Name = "popupContainerEdit3";
			this.popupContainerEdit3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.popupContainerEdit3.Size = new System.Drawing.Size(207, 20);
			this.popupContainerEdit3.TabIndex = 0;
			this.lblValue3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblValue3.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblValue3.Location = new System.Drawing.Point(0, 0);
			this.lblValue3.Name = "lblValue3";
			this.lblValue3.Size = new System.Drawing.Size(110, 20);
			this.lblValue3.TabIndex = 0;
			this.lblValue3.Text = "Value:";
			this.lblDockOffset3.BackColor = System.Drawing.Color.Transparent;
			this.lblDockOffset3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.lblDockOffset3.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblDockOffset3.Location = new System.Drawing.Point(12, 123);
			this.lblDockOffset3.Name = "lblDockOffset3";
			this.lblDockOffset3.Size = new System.Drawing.Size(317, 4);
			this.lblDockOffset3.TabIndex = 99;
			this.pnlValue2.AutoSize = true;
			this.pnlValue2.BackColor = System.Drawing.Color.Transparent;
			this.pnlValue2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlValue2.Controls.Add(this.popupContainerEdit4);
			this.pnlValue2.Controls.Add(this.lblValue2);
			this.pnlValue2.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlValue2.Location = new System.Drawing.Point(12, 103);
			this.pnlValue2.Name = "pnlValue2";
			this.pnlValue2.Size = new System.Drawing.Size(317, 20);
			this.pnlValue2.TabIndex = 4;
			this.popupContainerEdit4.Dock = System.Windows.Forms.DockStyle.Top;
			this.popupContainerEdit4.EditValue = "";
			this.popupContainerEdit4.EnterMoveNextControl = true;
			this.popupContainerEdit4.Location = new System.Drawing.Point(110, 0);
			this.popupContainerEdit4.Name = "popupContainerEdit4";
			this.popupContainerEdit4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.popupContainerEdit4.Size = new System.Drawing.Size(207, 20);
			this.popupContainerEdit4.TabIndex = 0;
			this.lblValue2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblValue2.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblValue2.Location = new System.Drawing.Point(0, 0);
			this.lblValue2.Name = "lblValue2";
			this.lblValue2.Size = new System.Drawing.Size(110, 20);
			this.lblValue2.TabIndex = 0;
			this.lblValue2.Text = "Value:";
			this.lblDockOffset2.BackColor = System.Drawing.Color.Transparent;
			this.lblDockOffset2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.lblDockOffset2.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblDockOffset2.Location = new System.Drawing.Point(12, 99);
			this.lblDockOffset2.Name = "lblDockOffset2";
			this.lblDockOffset2.Size = new System.Drawing.Size(317, 4);
			this.lblDockOffset2.TabIndex = 97;
			this.pnlValue1.AutoSize = true;
			this.pnlValue1.BackColor = System.Drawing.Color.Transparent;
			this.pnlValue1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlValue1.Controls.Add(this.popupContainerEdit5);
			this.pnlValue1.Controls.Add(this.lblValue1);
			this.pnlValue1.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlValue1.Location = new System.Drawing.Point(12, 79);
			this.pnlValue1.Name = "pnlValue1";
			this.pnlValue1.Size = new System.Drawing.Size(317, 20);
			this.pnlValue1.TabIndex = 3;
			this.popupContainerEdit5.Dock = System.Windows.Forms.DockStyle.Top;
			this.popupContainerEdit5.EditValue = "";
			this.popupContainerEdit5.EnterMoveNextControl = true;
			this.popupContainerEdit5.Location = new System.Drawing.Point(110, 0);
			this.popupContainerEdit5.Name = "popupContainerEdit5";
			this.popupContainerEdit5.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.popupContainerEdit5.Size = new System.Drawing.Size(207, 20);
			this.popupContainerEdit5.TabIndex = 0;
			this.lblValue1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblValue1.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblValue1.Location = new System.Drawing.Point(0, 0);
			this.lblValue1.Name = "lblValue1";
			this.lblValue1.Size = new System.Drawing.Size(110, 20);
			this.lblValue1.TabIndex = 0;
			this.lblValue1.Text = "Value:";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.chartPanelControl1.Location = new System.Drawing.Point(12, 75);
			this.chartPanelControl1.Name = "chartPanelControl1";
			this.chartPanelControl1.Size = new System.Drawing.Size(317, 4);
			this.chartPanelControl1.TabIndex = 103;
			this.pnlBindingMode.AutoSize = true;
			this.pnlBindingMode.BackColor = System.Drawing.Color.Transparent;
			this.pnlBindingMode.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlBindingMode.Controls.Add(this.comboBoxEdit1);
			this.pnlBindingMode.Controls.Add(this.lblBindingMode);
			this.pnlBindingMode.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlBindingMode.Location = new System.Drawing.Point(12, 55);
			this.pnlBindingMode.Name = "pnlBindingMode";
			this.pnlBindingMode.Size = new System.Drawing.Size(317, 20);
			this.pnlBindingMode.TabIndex = 2;
			this.comboBoxEdit1.Dock = System.Windows.Forms.DockStyle.Top;
			this.comboBoxEdit1.EditValue = "Values";
			this.comboBoxEdit1.EnterMoveNextControl = true;
			this.comboBoxEdit1.Location = new System.Drawing.Point(110, 0);
			this.comboBoxEdit1.Name = "comboBoxEdit1";
			this.comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.comboBoxEdit1.Properties.Items.AddRange(new object[] {
			"Values",
			"Summary Function"});
			this.comboBoxEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.comboBoxEdit1.Properties.SelectedIndexChanged += new System.EventHandler(this.cbBindingMode_SelectedIndexChanged);
			this.comboBoxEdit1.Size = new System.Drawing.Size(207, 20);
			this.comboBoxEdit1.TabIndex = 0;
			this.lblBindingMode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblBindingMode.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblBindingMode.Location = new System.Drawing.Point(0, 0);
			this.lblBindingMode.Name = "lblBindingMode";
			this.lblBindingMode.Size = new System.Drawing.Size(110, 20);
			this.lblBindingMode.TabIndex = 0;
			this.lblBindingMode.Text = "Binding mode:";
			this.lblDockOffset1.BackColor = System.Drawing.Color.Transparent;
			this.lblDockOffset1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.lblDockOffset1.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblDockOffset1.Location = new System.Drawing.Point(12, 51);
			this.lblDockOffset1.Name = "lblDockOffset1";
			this.lblDockOffset1.Size = new System.Drawing.Size(317, 4);
			this.lblDockOffset1.TabIndex = 95;
			this.pnlValueScaleType.AutoSize = true;
			this.pnlValueScaleType.BackColor = System.Drawing.Color.Transparent;
			this.pnlValueScaleType.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlValueScaleType.Controls.Add(this.comboBoxEdit2);
			this.pnlValueScaleType.Controls.Add(this.lblValueScaleType);
			this.pnlValueScaleType.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlValueScaleType.Location = new System.Drawing.Point(12, 31);
			this.pnlValueScaleType.Name = "pnlValueScaleType";
			this.pnlValueScaleType.Size = new System.Drawing.Size(317, 20);
			this.pnlValueScaleType.TabIndex = 1;
			this.comboBoxEdit2.Dock = System.Windows.Forms.DockStyle.Top;
			this.comboBoxEdit2.EditValue = "Numerical";
			this.comboBoxEdit2.EnterMoveNextControl = true;
			this.comboBoxEdit2.Location = new System.Drawing.Point(110, 0);
			this.comboBoxEdit2.Name = "comboBoxEdit2";
			this.comboBoxEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.comboBoxEdit2.Properties.Items.AddRange(new object[] {
			"Numerical",
			"DateTime"});
			this.comboBoxEdit2.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.comboBoxEdit2.Size = new System.Drawing.Size(207, 20);
			this.comboBoxEdit2.TabIndex = 0;
			this.lblValueScaleType.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblValueScaleType.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblValueScaleType.Location = new System.Drawing.Point(0, 0);
			this.lblValueScaleType.Name = "lblValueScaleType";
			this.lblValueScaleType.Size = new System.Drawing.Size(110, 20);
			this.lblValueScaleType.TabIndex = 0;
			this.lblValueScaleType.Text = "Value scale type:";
			this.grArgumentSettings.Controls.Add(this.pnlArgument);
			this.grArgumentSettings.Controls.Add(this.pnlArgumentSplitter);
			this.grArgumentSettings.Controls.Add(this.pnlArgumentScaleType);
			this.grArgumentSettings.Location = new System.Drawing.Point(269, 87);
			this.grArgumentSettings.Name = "grArgumentSettings";
			this.grArgumentSettings.Padding = new System.Windows.Forms.Padding(10);
			this.grArgumentSettings.Size = new System.Drawing.Size(341, 87);
			this.grArgumentSettings.TabIndex = 97;
			this.grArgumentSettings.Text = "Argument Properties";
			this.pnlArgument.AutoSize = true;
			this.pnlArgument.BackColor = System.Drawing.Color.Transparent;
			this.pnlArgument.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlArgument.Controls.Add(this.popupContainerEdit6);
			this.pnlArgument.Controls.Add(this.lblArgument);
			this.pnlArgument.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlArgument.Location = new System.Drawing.Point(12, 55);
			this.pnlArgument.Name = "pnlArgument";
			this.pnlArgument.Size = new System.Drawing.Size(317, 20);
			this.pnlArgument.TabIndex = 3;
			this.popupContainerEdit6.Dock = System.Windows.Forms.DockStyle.Top;
			this.popupContainerEdit6.EditValue = "";
			this.popupContainerEdit6.EnterMoveNextControl = true;
			this.popupContainerEdit6.Location = new System.Drawing.Point(110, 0);
			this.popupContainerEdit6.Name = "popupContainerEdit6";
			this.popupContainerEdit6.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.popupContainerEdit6.Size = new System.Drawing.Size(207, 20);
			this.popupContainerEdit6.TabIndex = 0;
			this.lblArgument.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblArgument.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblArgument.Location = new System.Drawing.Point(0, 0);
			this.lblArgument.Name = "lblArgument";
			this.lblArgument.Size = new System.Drawing.Size(110, 20);
			this.lblArgument.TabIndex = 0;
			this.lblArgument.Text = "Argument:";
			this.pnlArgumentSplitter.BackColor = System.Drawing.Color.Transparent;
			this.pnlArgumentSplitter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlArgumentSplitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlArgumentSplitter.Location = new System.Drawing.Point(12, 51);
			this.pnlArgumentSplitter.Name = "pnlArgumentSplitter";
			this.pnlArgumentSplitter.Size = new System.Drawing.Size(317, 4);
			this.pnlArgumentSplitter.TabIndex = 92;
			this.pnlArgumentScaleType.AutoSize = true;
			this.pnlArgumentScaleType.BackColor = System.Drawing.Color.Transparent;
			this.pnlArgumentScaleType.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlArgumentScaleType.Controls.Add(this.comboBoxEdit3);
			this.pnlArgumentScaleType.Controls.Add(this.lblArgumentScaleType);
			this.pnlArgumentScaleType.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlArgumentScaleType.Location = new System.Drawing.Point(12, 31);
			this.pnlArgumentScaleType.Name = "pnlArgumentScaleType";
			this.pnlArgumentScaleType.Size = new System.Drawing.Size(317, 20);
			this.pnlArgumentScaleType.TabIndex = 2;
			this.comboBoxEdit3.Dock = System.Windows.Forms.DockStyle.Top;
			this.comboBoxEdit3.EditValue = "Qualitative";
			this.comboBoxEdit3.EnterMoveNextControl = true;
			this.comboBoxEdit3.Location = new System.Drawing.Point(110, 0);
			this.comboBoxEdit3.Name = "comboBoxEdit3";
			this.comboBoxEdit3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.comboBoxEdit3.Properties.Items.AddRange(new object[] {
			"Qualitative",
			"Numerical",
			"DateTime",
			"Auto"});
			this.comboBoxEdit3.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.comboBoxEdit3.Size = new System.Drawing.Size(207, 20);
			this.comboBoxEdit3.TabIndex = 0;
			this.lblArgumentScaleType.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblArgumentScaleType.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblArgumentScaleType.Location = new System.Drawing.Point(0, 0);
			this.lblArgumentScaleType.Name = "lblArgumentScaleType";
			this.lblArgumentScaleType.Size = new System.Drawing.Size(110, 20);
			this.lblArgumentScaleType.TabIndex = 0;
			this.lblArgumentScaleType.Text = "Argument scale type:";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(620, 504);
			this.Controls.Add(this.grColorSettings);
			this.Controls.Add(this.grValueSettings);
			this.Controls.Add(this.grArgumentSettings);
			this.Controls.Add(this.cbDataSource);
			this.Controls.Add(this.lblDataSource);
			this.Controls.Add(this.cbChartDataMember);
			this.Controls.Add(this.lblDataMember);
			this.Controls.Add(this.beFilters);
			this.Controls.Add(this.lblDataFilters);
			this.Controls.Add(this.lvSeries);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "SeriesBindingForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Series Binding";
			((System.ComponentModel.ISupportInitialize)(this.lvSeries)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.beSummaryFunction.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValue4.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValue3.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValue2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValue1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBindingMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbValueScaleType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbArgument.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbArgumentScaleType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbDataSource.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbChartDataMember.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.beFilters.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grColorSettings)).EndInit();
			this.grColorSettings.ResumeLayout(false);
			this.grColorSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			this.chartPanelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grValueSettings)).EndInit();
			this.grValueSettings.ResumeLayout(false);
			this.grValueSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSummaryFunction)).EndInit();
			this.pnlSummaryFunction.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.buttonEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlValue4)).EndInit();
			this.pnlValue4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lblDockOffset4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlValue3)).EndInit();
			this.pnlValue3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit3.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lblDockOffset3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlValue2)).EndInit();
			this.pnlValue2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit4.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lblDockOffset2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlValue1)).EndInit();
			this.pnlValue1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit5.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBindingMode)).EndInit();
			this.pnlBindingMode.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lblDockOffset1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlValueScaleType)).EndInit();
			this.pnlValueScaleType.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grArgumentSettings)).EndInit();
			this.grArgumentSettings.ResumeLayout(false);
			this.grArgumentSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlArgument)).EndInit();
			this.pnlArgument.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.popupContainerEdit6.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlArgumentSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlArgumentScaleType)).EndInit();
			this.pnlArgumentScaleType.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit3.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.ImageListBoxControl lvSeries;
		private XtraEditors.PopupContainerEdit cbColor;
		private XtraEditors.ButtonEdit beSummaryFunction;
		private XtraEditors.PopupContainerEdit cbValue4;
		private XtraEditors.PopupContainerEdit cbValue3;
		private XtraEditors.PopupContainerEdit cbValue2;
		private XtraEditors.PopupContainerEdit cbValue1;
		private XtraEditors.ComboBoxEdit cbBindingMode;
		private XtraEditors.ComboBoxEdit cbValueScaleType;
		private XtraEditors.PopupContainerEdit cbArgument;
		private XtraEditors.ComboBoxEdit cbArgumentScaleType;
		private XtraEditors.ComboBoxEdit cbDataSource;
		private Wizard.ChartLabelControl lblDataSource;
		private XtraEditors.PopupContainerEdit cbChartDataMember;
		private Wizard.ChartLabelControl lblDataMember;
		private XtraEditors.ButtonEdit beFilters;
		private Wizard.ChartLabelControl lblDataFilters;
		private XtraEditors.GroupControl grColorSettings;
		private Wizard.ChartPanelControl chartPanelControl2;
		private XtraEditors.PopupContainerEdit popupContainerEdit1;
		private Wizard.ChartLabelControl lblColor;
		private XtraEditors.GroupControl grValueSettings;
		private Wizard.ChartPanelControl pnlSummaryFunction;
		private XtraEditors.ButtonEdit buttonEdit1;
		private Wizard.ChartLabelControl lblSummaryFunction;
		private Wizard.ChartPanelControl pnlValue4;
		private XtraEditors.PopupContainerEdit popupContainerEdit2;
		private Wizard.ChartLabelControl lblValue4;
		private Wizard.ChartPanelControl lblDockOffset4;
		private Wizard.ChartPanelControl pnlValue3;
		private XtraEditors.PopupContainerEdit popupContainerEdit3;
		private Wizard.ChartLabelControl lblValue3;
		private Wizard.ChartPanelControl lblDockOffset3;
		private Wizard.ChartPanelControl pnlValue2;
		private XtraEditors.PopupContainerEdit popupContainerEdit4;
		private Wizard.ChartLabelControl lblValue2;
		private Wizard.ChartPanelControl lblDockOffset2;
		private Wizard.ChartPanelControl pnlValue1;
		private XtraEditors.PopupContainerEdit popupContainerEdit5;
		private Wizard.ChartLabelControl lblValue1;
		private Wizard.ChartPanelControl chartPanelControl1;
		private Wizard.ChartPanelControl pnlBindingMode;
		private XtraEditors.ComboBoxEdit comboBoxEdit1;
		private Wizard.ChartLabelControl lblBindingMode;
		private Wizard.ChartPanelControl lblDockOffset1;
		private Wizard.ChartPanelControl pnlValueScaleType;
		private XtraEditors.ComboBoxEdit comboBoxEdit2;
		private Wizard.ChartLabelControl lblValueScaleType;
		private XtraEditors.GroupControl grArgumentSettings;
		private Wizard.ChartPanelControl pnlArgument;
		private XtraEditors.PopupContainerEdit popupContainerEdit6;
		private Wizard.ChartLabelControl lblArgument;
		private Wizard.ChartPanelControl pnlArgumentSplitter;
		private Wizard.ChartPanelControl pnlArgumentScaleType;
		private XtraEditors.ComboBoxEdit comboBoxEdit3;
		private Wizard.ChartLabelControl lblArgumentScaleType;
	}
}
