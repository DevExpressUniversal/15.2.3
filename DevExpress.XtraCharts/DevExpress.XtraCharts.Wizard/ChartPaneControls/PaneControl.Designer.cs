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

namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	partial class PaneControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaneControl));
			this.tbcTabPages = new DevExpress.XtraTab.XtraTabControl();
			this.tbGeneral = new DevExpress.XtraTab.XtraTabPage();
			this.groupSize = new DevExpress.XtraEditors.GroupControl();
			this.panelSizeInPixels = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtSizeInPixels = new DevExpress.XtraEditors.SpinEdit();
			this.lblSizeInPixels = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelWeight = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtWeight = new DevExpress.XtraEditors.SpinEdit();
			this.lblWeight = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelSizeMode = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbSizeMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblSizeMode = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl5 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelName = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtName = new DevExpress.XtraEditors.TextEdit();
			this.lblName = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelControl11 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.panelVisible = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chVisible = new DevExpress.XtraEditors.CheckEdit();
			this.tbAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.backgroundControl = new DevExpress.XtraCharts.Wizard.BackgroundControl();
			this.tbBorder = new DevExpress.XtraTab.XtraTabPage();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceBorderColor = new DevExpress.XtraEditors.ColorEdit();
			this.lblColor = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.cbBorderVisible = new DevExpress.XtraEditors.CheckEdit();
			this.tbShadow = new DevExpress.XtraTab.XtraTabPage();
			this.shadowControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.ShadowControl();
			this.tbScrollingZooming = new DevExpress.XtraTab.XtraTabPage();
			this.gcAxisY = new DevExpress.XtraEditors.GroupControl();
			this.chEnableAxisYZooming = new DevExpress.XtraEditors.CheckEdit();
			this.chartPanelControl6 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl10 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chEnableAxisYScrolling = new DevExpress.XtraEditors.CheckEdit();
			this.pnlSeparator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.gcAxisX = new DevExpress.XtraEditors.GroupControl();
			this.chEnableAxisXZooming = new DevExpress.XtraEditors.CheckEdit();
			this.chartPanelControl9 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl8 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chEnableAxisXScrolling = new DevExpress.XtraEditors.CheckEdit();
			this.chartPanelControl7 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblNote = new DevExpress.XtraEditors.LabelControl();
			this.tbScrollBarOptions = new DevExpress.XtraTab.XtraTabPage();
			this.scrollBarOptionsControl = new DevExpress.XtraCharts.Wizard.ChartDiagramControls.ScrollBarOptionsControl();
			((System.ComponentModel.ISupportInitialize)(this.tbcTabPages)).BeginInit();
			this.tbcTabPages.SuspendLayout();
			this.tbGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupSize)).BeginInit();
			this.groupSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelSizeInPixels)).BeginInit();
			this.panelSizeInPixels.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtSizeInPixels.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelWeight)).BeginInit();
			this.panelWeight.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtWeight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelSizeMode)).BeginInit();
			this.panelSizeMode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbSizeMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).BeginInit();
			this.chartPanelControl5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelName)).BeginInit();
			this.panelName.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelVisible)).BeginInit();
			this.panelVisible.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).BeginInit();
			this.tbAppearance.SuspendLayout();
			this.tbBorder.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			this.chartPanelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceBorderColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBorderVisible.Properties)).BeginInit();
			this.tbShadow.SuspendLayout();
			this.tbScrollingZooming.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcAxisY)).BeginInit();
			this.gcAxisY.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxisYZooming.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxisYScrolling.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcAxisX)).BeginInit();
			this.gcAxisX.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxisXZooming.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxisXScrolling.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).BeginInit();
			this.tbScrollBarOptions.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.tbcTabPages, "tbcTabPages");
			this.tbcTabPages.Name = "tbcTabPages";
			this.tbcTabPages.SelectedTabPage = this.tbGeneral;
			this.tbcTabPages.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbGeneral,
			this.tbAppearance,
			this.tbBorder,
			this.tbShadow,
			this.tbScrollingZooming,
			this.tbScrollBarOptions});
			this.tbGeneral.Controls.Add(this.groupSize);
			this.tbGeneral.Controls.Add(this.chartPanelControl2);
			this.tbGeneral.Controls.Add(this.chartPanelControl5);
			this.tbGeneral.Controls.Add(this.panelControl11);
			this.tbGeneral.Controls.Add(this.labelControl1);
			this.tbGeneral.Controls.Add(this.panelVisible);
			this.tbGeneral.Name = "tbGeneral";
			resources.ApplyResources(this.tbGeneral, "tbGeneral");
			resources.ApplyResources(this.groupSize, "groupSize");
			this.groupSize.Controls.Add(this.panelSizeInPixels);
			this.groupSize.Controls.Add(this.panelWeight);
			this.groupSize.Controls.Add(this.chartPanelControl1);
			this.groupSize.Controls.Add(this.panelSizeMode);
			this.groupSize.Name = "groupSize";
			resources.ApplyResources(this.panelSizeInPixels, "panelSizeInPixels");
			this.panelSizeInPixels.BackColor = System.Drawing.Color.Transparent;
			this.panelSizeInPixels.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelSizeInPixels.Controls.Add(this.txtSizeInPixels);
			this.panelSizeInPixels.Controls.Add(this.lblSizeInPixels);
			this.panelSizeInPixels.Name = "panelSizeInPixels";
			resources.ApplyResources(this.txtSizeInPixels, "txtSizeInPixels");
			this.txtSizeInPixels.Name = "txtSizeInPixels";
			this.txtSizeInPixels.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtSizeInPixels.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtSizeInPixels.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtSizeInPixels.EditValueChanged += new System.EventHandler(this.txtSizeInPixels_EditValueChanged);
			resources.ApplyResources(this.lblSizeInPixels, "lblSizeInPixels");
			this.lblSizeInPixels.Name = "lblSizeInPixels";
			resources.ApplyResources(this.panelWeight, "panelWeight");
			this.panelWeight.BackColor = System.Drawing.Color.Transparent;
			this.panelWeight.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelWeight.Controls.Add(this.txtWeight);
			this.panelWeight.Controls.Add(this.lblWeight);
			this.panelWeight.Name = "panelWeight";
			resources.ApplyResources(this.txtWeight, "txtWeight");
			this.txtWeight.Name = "txtWeight";
			this.txtWeight.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtWeight.Properties.DisplayFormat.FormatString = "F1";
			this.txtWeight.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
			this.txtWeight.Properties.EditFormat.FormatString = "F1";
			this.txtWeight.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
			this.txtWeight.Properties.Increment = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.txtWeight.Properties.MaxValue = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			this.txtWeight.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.txtWeight.EditValueChanged += new System.EventHandler(this.txtWeight_EditValueChanged);
			resources.ApplyResources(this.lblWeight, "lblWeight");
			this.lblWeight.Name = "lblWeight";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			resources.ApplyResources(this.panelSizeMode, "panelSizeMode");
			this.panelSizeMode.BackColor = System.Drawing.Color.Transparent;
			this.panelSizeMode.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelSizeMode.Controls.Add(this.cbSizeMode);
			this.panelSizeMode.Controls.Add(this.lblSizeMode);
			this.panelSizeMode.Name = "panelSizeMode";
			resources.ApplyResources(this.cbSizeMode, "cbSizeMode");
			this.cbSizeMode.Name = "cbSizeMode";
			this.cbSizeMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbSizeMode.Properties.Buttons"))))});
			this.cbSizeMode.Properties.Items.AddRange(new object[] {
			resources.GetString("cbSizeMode.Properties.Items"),
			resources.GetString("cbSizeMode.Properties.Items1")});
			this.cbSizeMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbSizeMode.SelectedIndexChanged += new System.EventHandler(this.cbSizeMode_SelectedIndexChanged);
			resources.ApplyResources(this.lblSizeMode, "lblSizeMode");
			this.lblSizeMode.Name = "lblSizeMode";
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.chartPanelControl5, "chartPanelControl5");
			this.chartPanelControl5.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl5.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl5.Controls.Add(this.panelName);
			this.chartPanelControl5.Name = "chartPanelControl5";
			resources.ApplyResources(this.panelName, "panelName");
			this.panelName.BackColor = System.Drawing.Color.Transparent;
			this.panelName.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelName.Controls.Add(this.txtName);
			this.panelName.Controls.Add(this.lblName);
			this.panelName.Name = "panelName";
			resources.ApplyResources(this.txtName, "txtName");
			this.txtName.Name = "txtName";
			this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
			resources.ApplyResources(this.lblName, "lblName");
			this.lblName.Name = "lblName";
			this.panelControl11.BackColor = System.Drawing.Color.Transparent;
			this.panelControl11.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl11, "panelControl11");
			this.panelControl11.Name = "panelControl11";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.panelVisible, "panelVisible");
			this.panelVisible.BackColor = System.Drawing.Color.Transparent;
			this.panelVisible.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelVisible.Controls.Add(this.chVisible);
			this.panelVisible.Name = "panelVisible";
			resources.ApplyResources(this.chVisible, "chVisible");
			this.chVisible.Name = "chVisible";
			this.chVisible.Properties.Caption = resources.GetString("chVisible.Properties.Caption");
			this.chVisible.CheckedChanged += new System.EventHandler(this.chVisible_CheckedChanged);
			this.tbAppearance.Controls.Add(this.backgroundControl);
			this.tbAppearance.Name = "tbAppearance";
			resources.ApplyResources(this.tbAppearance, "tbAppearance");
			resources.ApplyResources(this.backgroundControl, "backgroundControl");
			this.backgroundControl.Name = "backgroundControl";
			this.tbBorder.Controls.Add(this.chartPanelControl3);
			this.tbBorder.Controls.Add(this.chartPanelControl4);
			this.tbBorder.Controls.Add(this.labelControl2);
			this.tbBorder.Controls.Add(this.cbBorderVisible);
			this.tbBorder.Name = "tbBorder";
			resources.ApplyResources(this.tbBorder, "tbBorder");
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl3.Controls.Add(this.ceBorderColor);
			this.chartPanelControl3.Controls.Add(this.lblColor);
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.Name = "chartPanelControl3";
			resources.ApplyResources(this.ceBorderColor, "ceBorderColor");
			this.ceBorderColor.Name = "ceBorderColor";
			this.ceBorderColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceBorderColor.Properties.Buttons"))))});
			this.ceBorderColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceBorderColor.EditValueChanged += new System.EventHandler(this.chBorderColor_EditValueChanged);
			resources.ApplyResources(this.lblColor, "lblColor");
			this.lblColor.Name = "lblColor";
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.Name = "chartPanelControl4";
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.LineVisible = true;
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.cbBorderVisible, "cbBorderVisible");
			this.cbBorderVisible.Name = "cbBorderVisible";
			this.cbBorderVisible.Properties.Caption = resources.GetString("cbBorderVisible.Properties.Caption");
			this.cbBorderVisible.CheckedChanged += new System.EventHandler(this.cbBorderVisible_CheckedChanged);
			this.tbShadow.Controls.Add(this.shadowControl);
			this.tbShadow.Name = "tbShadow";
			resources.ApplyResources(this.tbShadow, "tbShadow");
			resources.ApplyResources(this.shadowControl, "shadowControl");
			this.shadowControl.Name = "shadowControl";
			this.tbScrollingZooming.Controls.Add(this.gcAxisY);
			this.tbScrollingZooming.Controls.Add(this.pnlSeparator);
			this.tbScrollingZooming.Controls.Add(this.gcAxisX);
			this.tbScrollingZooming.Controls.Add(this.chartPanelControl7);
			this.tbScrollingZooming.Controls.Add(this.lblNote);
			this.tbScrollingZooming.Name = "tbScrollingZooming";
			resources.ApplyResources(this.tbScrollingZooming, "tbScrollingZooming");
			resources.ApplyResources(this.gcAxisY, "gcAxisY");
			this.gcAxisY.Controls.Add(this.chEnableAxisYZooming);
			this.gcAxisY.Controls.Add(this.chartPanelControl6);
			this.gcAxisY.Controls.Add(this.chartPanelControl10);
			this.gcAxisY.Controls.Add(this.chEnableAxisYScrolling);
			this.gcAxisY.Name = "gcAxisY";
			resources.ApplyResources(this.chEnableAxisYZooming, "chEnableAxisYZooming");
			this.chEnableAxisYZooming.Name = "chEnableAxisYZooming";
			this.chEnableAxisYZooming.Properties.AllowGrayed = true;
			this.chEnableAxisYZooming.Properties.Caption = resources.GetString("chEnableAxisYZooming.Properties.Caption");
			this.chEnableAxisYZooming.CheckStateChanged += new System.EventHandler(this.chEnableAxisYZooming_CheckStateChanged);
			resources.ApplyResources(this.chartPanelControl6, "chartPanelControl6");
			this.chartPanelControl6.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl6.Name = "chartPanelControl6";
			this.chartPanelControl10.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl10.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl10, "chartPanelControl10");
			this.chartPanelControl10.Name = "chartPanelControl10";
			resources.ApplyResources(this.chEnableAxisYScrolling, "chEnableAxisYScrolling");
			this.chEnableAxisYScrolling.Name = "chEnableAxisYScrolling";
			this.chEnableAxisYScrolling.Properties.AllowGrayed = true;
			this.chEnableAxisYScrolling.Properties.Caption = resources.GetString("chEnableAxisYScrolling.Properties.Caption");
			this.chEnableAxisYScrolling.CheckStateChanged += new System.EventHandler(this.chEnableAxisYScrolling_CheckStateChanged);
			this.pnlSeparator.BackColor = System.Drawing.Color.Transparent;
			this.pnlSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlSeparator, "pnlSeparator");
			this.pnlSeparator.Name = "pnlSeparator";
			resources.ApplyResources(this.gcAxisX, "gcAxisX");
			this.gcAxisX.Controls.Add(this.chEnableAxisXZooming);
			this.gcAxisX.Controls.Add(this.chartPanelControl9);
			this.gcAxisX.Controls.Add(this.chartPanelControl8);
			this.gcAxisX.Controls.Add(this.chEnableAxisXScrolling);
			this.gcAxisX.Name = "gcAxisX";
			resources.ApplyResources(this.chEnableAxisXZooming, "chEnableAxisXZooming");
			this.chEnableAxisXZooming.Name = "chEnableAxisXZooming";
			this.chEnableAxisXZooming.Properties.AllowGrayed = true;
			this.chEnableAxisXZooming.Properties.Caption = resources.GetString("chEnableAxisXZooming.Properties.Caption");
			this.chEnableAxisXZooming.CheckStateChanged += new System.EventHandler(this.chEnableAxisXZooming_CheckStateChanged);
			resources.ApplyResources(this.chartPanelControl9, "chartPanelControl9");
			this.chartPanelControl9.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl9.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.chartPanelControl9.Name = "chartPanelControl9";
			this.chartPanelControl8.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl8.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl8, "chartPanelControl8");
			this.chartPanelControl8.Name = "chartPanelControl8";
			resources.ApplyResources(this.chEnableAxisXScrolling, "chEnableAxisXScrolling");
			this.chEnableAxisXScrolling.Name = "chEnableAxisXScrolling";
			this.chEnableAxisXScrolling.Properties.AllowGrayed = true;
			this.chEnableAxisXScrolling.Properties.Caption = resources.GetString("chEnableAxisXScrolling.Properties.Caption");
			this.chEnableAxisXScrolling.CheckStateChanged += new System.EventHandler(this.chEnableAxisXScrolling_CheckStateChanged);
			this.chartPanelControl7.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl7.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl7, "chartPanelControl7");
			this.chartPanelControl7.Name = "chartPanelControl7";
			resources.ApplyResources(this.lblNote, "lblNote");
			this.lblNote.Name = "lblNote";
			this.tbScrollBarOptions.Controls.Add(this.scrollBarOptionsControl);
			this.tbScrollBarOptions.Name = "tbScrollBarOptions";
			resources.ApplyResources(this.tbScrollBarOptions, "tbScrollBarOptions");
			resources.ApplyResources(this.scrollBarOptionsControl, "scrollBarOptionsControl");
			this.scrollBarOptionsControl.Name = "scrollBarOptionsControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tbcTabPages);
			this.Name = "PaneControl";
			((System.ComponentModel.ISupportInitialize)(this.tbcTabPages)).EndInit();
			this.tbcTabPages.ResumeLayout(false);
			this.tbGeneral.ResumeLayout(false);
			this.tbGeneral.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupSize)).EndInit();
			this.groupSize.ResumeLayout(false);
			this.groupSize.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelSizeInPixels)).EndInit();
			this.panelSizeInPixels.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtSizeInPixels.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelWeight)).EndInit();
			this.panelWeight.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtWeight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelSizeMode)).EndInit();
			this.panelSizeMode.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbSizeMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl5)).EndInit();
			this.chartPanelControl5.ResumeLayout(false);
			this.chartPanelControl5.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelName)).EndInit();
			this.panelName.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelVisible)).EndInit();
			this.panelVisible.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chVisible.Properties)).EndInit();
			this.tbAppearance.ResumeLayout(false);
			this.tbAppearance.PerformLayout();
			this.tbBorder.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			this.chartPanelControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceBorderColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBorderVisible.Properties)).EndInit();
			this.tbShadow.ResumeLayout(false);
			this.tbShadow.PerformLayout();
			this.tbScrollingZooming.ResumeLayout(false);
			this.tbScrollingZooming.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcAxisY)).EndInit();
			this.gcAxisY.ResumeLayout(false);
			this.gcAxisY.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxisYZooming.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxisYScrolling.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcAxisX)).EndInit();
			this.gcAxisX.ResumeLayout(false);
			this.gcAxisX.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxisXZooming.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chEnableAxisXScrolling.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl7)).EndInit();
			this.tbScrollBarOptions.ResumeLayout(false);
			this.tbScrollBarOptions.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTab.XtraTabControl tbcTabPages;
		private DevExpress.XtraTab.XtraTabPage tbGeneral;
		private ChartPanelControl panelVisible;
		private ChartPanelControl panelControl11;
		private DevExpress.XtraEditors.CheckEdit chVisible;
		private ChartPanelControl chartPanelControl2;
		private ChartPanelControl panelName;
		private DevExpress.XtraEditors.TextEdit txtName;
		private ChartLabelControl lblName;
		private DevExpress.XtraEditors.GroupControl groupSize;
		private ChartPanelControl panelSizeMode;
		private DevExpress.XtraEditors.ComboBoxEdit cbSizeMode;
		private ChartLabelControl lblSizeMode;
		private ChartPanelControl chartPanelControl1;
		private ChartPanelControl panelWeight;
		private DevExpress.XtraEditors.SpinEdit txtWeight;
		private ChartLabelControl lblWeight;
		private ChartPanelControl panelSizeInPixels;
		private DevExpress.XtraEditors.SpinEdit txtSizeInPixels;
		private ChartLabelControl lblSizeInPixels;
		private DevExpress.XtraTab.XtraTabPage tbAppearance;
		private DevExpress.XtraTab.XtraTabPage tbBorder;
		private DevExpress.XtraTab.XtraTabPage tbShadow;
		private ChartPanelControl chartPanelControl3;
		private DevExpress.XtraEditors.ColorEdit ceBorderColor;
		private ChartLabelControl lblColor;
		private ChartPanelControl chartPanelControl4;
		private DevExpress.XtraEditors.CheckEdit cbBorderVisible;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.ShadowControl shadowControl;
		private ChartPanelControl chartPanelControl5;
		private BackgroundControl backgroundControl;
		private DevExpress.XtraTab.XtraTabPage tbScrollBarOptions;
		private ScrollBarOptionsControl scrollBarOptionsControl;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.LabelControl labelControl2;
		private DevExpress.XtraTab.XtraTabPage tbScrollingZooming;
		private DevExpress.XtraEditors.GroupControl gcAxisY;
		private DevExpress.XtraEditors.CheckEdit chEnableAxisYZooming;
		private ChartPanelControl chartPanelControl6;
		private ChartPanelControl chartPanelControl10;
		private DevExpress.XtraEditors.CheckEdit chEnableAxisYScrolling;
		private ChartPanelControl pnlSeparator;
		private DevExpress.XtraEditors.GroupControl gcAxisX;
		private DevExpress.XtraEditors.CheckEdit chEnableAxisXZooming;
		private ChartPanelControl chartPanelControl9;
		private ChartPanelControl chartPanelControl8;
		private DevExpress.XtraEditors.CheckEdit chEnableAxisXScrolling;
		private ChartPanelControl chartPanelControl7;
		private DevExpress.XtraEditors.LabelControl lblNote;
	}
}
