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
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraBars.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout;
namespace DevExpress.XtraPrinting.Preview {
	public class ScaleControlContainer : PrintPreviewPopupControlContainer, DevExpress.Utils.Menu.IDXMenuManagerProvider {
		#region consts
		const int minValue = 1;
		const int maxValue = 1000;
		const int space = 3;
		#endregion
		#region fields
		bool isRTLChanged = false;
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraEditors.SimpleButton buttonCancel;
		private DevExpress.XtraEditors.SimpleButton buttonOk;
		private DevExpress.XtraEditors.LabelControl labelPagesWide;
		private DevExpress.XtraEditors.LabelControl labelAdjustTo;
		private DevExpress.XtraEditors.SpinEdit spinPagesWide;
		private DevExpress.XtraEditors.SpinEdit spinScalePercent;
		private DevExpress.XtraEditors.CheckEdit radioFitToPageWidth;
		private DevExpress.XtraEditors.CheckEdit radioAdjustToPersent;
		private DevExpress.XtraLayout.LayoutControlGroup group;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.LayoutControlGroup grpButtons;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
		private System.Windows.Forms.Label emptySpaceLabel;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
		bool committed;
		#endregion
		public float ScaleFactor { get { return (float)spinScalePercent.Value / 100f; } }
		public bool Committed { get { return committed; } }
		public bool FitToPageWidth { get { return radioFitToPageWidth.Checked; } }
		public int FitToWidthPagesCount { get { return (int)spinPagesWide.Value; } }
		public Document Document { get { return PrintControl.PrintingSystem.Document; } }
		private void InitializeComponent() {
			DevExpress.XtraLayout.ColumnDefinition columnDefinition9 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition7 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition8 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
			this.buttonOk = new DevExpress.XtraEditors.SimpleButton();
			this.labelPagesWide = new DevExpress.XtraEditors.LabelControl();
			this.labelAdjustTo = new DevExpress.XtraEditors.LabelControl();
			this.spinPagesWide = new DevExpress.XtraEditors.SpinEdit();
			this.spinScalePercent = new DevExpress.XtraEditors.SpinEdit();
			this.radioFitToPageWidth = new DevExpress.XtraEditors.CheckEdit();
			this.radioAdjustToPersent = new DevExpress.XtraEditors.CheckEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.group = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceLabel = new System.Windows.Forms.Label();
			this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spinPagesWide.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinScalePercent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.radioFitToPageWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.radioAdjustToPersent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.group)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.emptySpaceLabel);
			this.layoutControl1.Controls.Add(this.buttonCancel);
			this.layoutControl1.Controls.Add(this.buttonOk);
			this.layoutControl1.Controls.Add(this.labelPagesWide);
			this.layoutControl1.Controls.Add(this.labelAdjustTo);
			this.layoutControl1.Controls.Add(this.spinPagesWide);
			this.layoutControl1.Controls.Add(this.spinScalePercent);
			this.layoutControl1.Controls.Add(this.radioFitToPageWidth);
			this.layoutControl1.Controls.Add(this.radioAdjustToPersent);
			this.layoutControl1.Location = new System.Drawing.Point(26, 65);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(306, 150);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonCancel.Location = new System.Drawing.Point(225, 116);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(69, 22);
			this.buttonCancel.StyleController = this.layoutControl1;
			this.buttonCancel.TabIndex = 11;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new EventHandler(button_Click);
			this.buttonOk.Location = new System.Drawing.Point(149, 116);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(69, 22);
			this.buttonOk.StyleController = this.layoutControl1;
			this.buttonOk.TabIndex = 10;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new EventHandler(button_Click);
			this.labelPagesWide.Location = new System.Drawing.Point(196, 59);
			this.labelPagesWide.Name = "labelPagesWide";
			this.labelPagesWide.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
			this.labelPagesWide.Size = new System.Drawing.Size(64, 17);
			this.labelPagesWide.StyleController = this.layoutControl1;
			this.labelPagesWide.TabIndex = 9;
			this.labelPagesWide.Text = "pages wide";
			this.labelPagesWide.Enabled = false;
			this.labelAdjustTo.Location = new System.Drawing.Point(196, 26);
			this.labelAdjustTo.Name = "labelAdjustTo";
			this.labelAdjustTo.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
			this.labelAdjustTo.Size = new System.Drawing.Size(90, 17);
			this.labelAdjustTo.StyleController = this.layoutControl1;
			this.labelAdjustTo.TabIndex = 8;
			this.labelAdjustTo.Text = "% of normal size";
			this.labelAdjustTo.Enabled = false;
			this.spinPagesWide.EditValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.spinPagesWide.Location = new System.Drawing.Point(103, 58);
			this.spinPagesWide.Name = "spinPagesWide";
			this.spinPagesWide.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.spinPagesWide.Size = new System.Drawing.Size(91, 20);
			this.spinPagesWide.StyleController = this.layoutControl1;
			this.spinPagesWide.TabIndex = 7;
			this.spinPagesWide.Properties.EditMask = "###";
			this.spinPagesWide.Properties.MinValue = 1;
			this.spinPagesWide.Properties.MaxValue = 10;
			this.spinPagesWide.Enabled = false;
			this.spinScalePercent.EditValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.spinScalePercent.Location = new System.Drawing.Point(103, 25);
			this.spinScalePercent.Name = "spinScalePercent";
			this.spinScalePercent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.spinScalePercent.Size = new System.Drawing.Size(91, 20);
			this.spinScalePercent.StyleController = this.layoutControl1;
			this.spinScalePercent.TabIndex = 6;
			this.spinScalePercent.Properties.EditMask = "####";
			this.spinScalePercent.Properties.MinValue = minValue;
			this.spinScalePercent.Properties.MaxValue = maxValue;
			this.spinScalePercent.Enabled = false;
			this.radioFitToPageWidth.Location = new System.Drawing.Point(27, 58);
			this.radioFitToPageWidth.Name = "radioFitToPageWidth";
			this.radioFitToPageWidth.Properties.Caption = "Fit to";
			this.radioFitToPageWidth.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.radioFitToPageWidth.Size = new System.Drawing.Size(72, 19);
			this.radioFitToPageWidth.StyleController = this.layoutControl1;
			this.radioFitToPageWidth.TabIndex = 5;
			this.radioFitToPageWidth.Properties.RadioGroupIndex = 0;
			this.radioFitToPageWidth.CheckedChanged += new EventHandler(radioFitToPageWidth_CheckedChanged);
			this.radioAdjustToPersent.Location = new System.Drawing.Point(27, 25);
			this.radioAdjustToPersent.Name = "radioAdjustToPersent";
			this.radioAdjustToPersent.Properties.Caption = "Adjust to";
			this.radioAdjustToPersent.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.radioAdjustToPersent.Size = new System.Drawing.Size(72, 19);
			this.radioAdjustToPersent.StyleController = this.layoutControl1;
			this.radioAdjustToPersent.TabIndex = 4;
			this.radioAdjustToPersent.Properties.RadioGroupIndex = 0;
			this.radioAdjustToPersent.CheckedChanged += new EventHandler(radioAdjustToPersent_CheckedChanged);
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.group,
			this.grpButtons,
			this.layoutControlItem9});
			this.layoutControlGroup1.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			columnDefinition9.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition9.Width = 271D;
			this.layoutControlGroup1.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			this.layoutControlGroup1.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition9});
			rowDefinition5.Height = 57D;
			rowDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition6.Height = 34D;
			rowDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition7.Height = 26D;
			rowDefinition7.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup1.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition5,
			rowDefinition6,
			rowDefinition7});
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 10, 23, 10);
			this.layoutControlGroup1.Size = new System.Drawing.Size(306, 150);
			this.layoutControlGroup1.TextVisible = false;
			this.group.GroupBordersVisible = false;
			this.group.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem6});
			this.group.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.group.Location = new System.Drawing.Point(0, 0);
			this.group.Name = "group";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition1.Width = 76D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition2.Width = 95D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition3.Width = 90D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition4.Width = 100D;
			this.group.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			this.group.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4});
			rowDefinition1.Height = 24D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition2.Height = 9D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition3.Height = 24D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.group.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2,
			rowDefinition3});
			this.group.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.group.Size = new System.Drawing.Size(271, 57);
			this.group.Text = "Scaling";
			this.layoutControlItem1.Control = this.radioAdjustToPersent;
			this.layoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(76, 24);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem1.TrimClientAreaToControl = false;
			this.layoutControlItem2.Control = this.radioFitToPageWidth;
			this.layoutControlItem2.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 33);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem2.Size = new System.Drawing.Size(76, 24);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem2.TrimClientAreaToControl = false;
			this.layoutControlItem3.Control = this.spinScalePercent;
			this.layoutControlItem3.Location = new System.Drawing.Point(76, 0);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem3.Size = new System.Drawing.Size(95, 24);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem4.Control = this.spinPagesWide;
			this.layoutControlItem4.Location = new System.Drawing.Point(76, 33);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem4.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem4.Size = new System.Drawing.Size(95, 24);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem5.Control = this.labelAdjustTo;
			this.layoutControlItem5.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem5.Location = new System.Drawing.Point(171, 0);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem5.Size = new System.Drawing.Size(90, 24);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem5.TrimClientAreaToControl = false;
			this.layoutControlItem6.Control = this.labelPagesWide;
			this.layoutControlItem6.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem6.Location = new System.Drawing.Point(171, 33);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem6.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem6.Size = new System.Drawing.Size(90, 24);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem6.TrimClientAreaToControl = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem7,
			this.layoutControlItem8});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 91);
			this.grpButtons.Name = "grpButtons";
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition5.Width = 100D;
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition6.Width = 73D;
			columnDefinition7.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition7.Width = 3D;
			columnDefinition8.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition8.Width = 73D;
			this.grpButtons.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition5,
			columnDefinition6,
			columnDefinition7,
			columnDefinition8});
			rowDefinition4.Height = 26D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition4});
			this.grpButtons.OptionsTableLayoutItem.RowIndex = 2;
			this.grpButtons.Size = new System.Drawing.Size(271, 26);
			this.layoutControlItem7.Control = this.buttonOk;
			this.layoutControlItem7.Location = new System.Drawing.Point(122, 0);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem7.Size = new System.Drawing.Size(73, 26);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.layoutControlItem8.Control = this.buttonCancel;
			this.layoutControlItem8.Location = new System.Drawing.Point(198, 0);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem8.Size = new System.Drawing.Size(73, 26);
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.emptySpaceLabel.Location = new System.Drawing.Point(25, 80);
			this.emptySpaceLabel.Name = "emptySpaceLabel";
			this.emptySpaceLabel.Padding = new System.Windows.Forms.Padding(5);
			this.emptySpaceLabel.Size = new System.Drawing.Size(271, 34);
			this.emptySpaceLabel.TabIndex = 12;
			this.layoutControlItem9.Control = this.emptySpaceLabel;
			this.layoutControlItem9.Location = new System.Drawing.Point(0, 57);
			this.layoutControlItem9.Name = "layoutControlItem9";
			this.layoutControlItem9.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem9.Size = new System.Drawing.Size(271, 34);
			this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem9.TextVisible = false;
			this.Controls.Add(this.layoutControl1);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spinPagesWide.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinScalePercent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.radioFitToPageWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.radioAdjustToPersent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.group)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
			this.ResumeLayout(false);
		}
		public ScaleControlContainer()
			: base() {
			InitializeComponent();
			LocalizeControls();
			InitialazeLayout();
		}
		void LocalizeControls() {
			radioAdjustToPersent.Text = PreviewLocalizer.GetString(PreviewStringId.ScalePopup_AdjustTo);
			radioFitToPageWidth.Text = PreviewLocalizer.GetString(PreviewStringId.ScalePopup_FitTo);
			labelPagesWide.Text = PreviewLocalizer.GetString(PreviewStringId.ScalePopup_PagesWide);
			labelAdjustTo.Text = PreviewLocalizer.GetString(PreviewStringId.ScalePopup_NormalSize);
			buttonOk.Text = PreviewLocalizer.GetString(PreviewStringId.Button_Ok);
			buttonCancel.Text = PreviewLocalizer.GetString(PreviewStringId.Button_Cancel);
		}
		protected override void OnPopup() {
			base.OnPopup();
			float scaleFactor = Document.ScaleFactor;
			spinScalePercent.Value = (int)System.Math.Round(scaleFactor * 100f);
			committed = false;
			radioFitToPageWidth.Checked = Document.AutoFitToPagesWidth > 0;
			radioAdjustToPersent.Checked = !radioFitToPageWidth.Checked;
			spinPagesWide.Value = radioFitToPageWidth.Checked ? Document.AutoFitToPagesWidth : 1;		   
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			if(FitToPageWidth)
				SetSpinEditFocus(spinPagesWide);
			else 
				SetSpinEditFocus(spinScalePercent);
		}
		protected override bool ProcessDialogKey(System.Windows.Forms.Keys keyData) {
			if(keyData == System.Windows.Forms.Keys.Enter) {
				committed = true;
				buttonOk.Focus();
				HidePopup();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected override void OnRightToLeftChanged() {
			base.OnRightToLeftChanged();
			isRTLChanged = true;
		}
		void InitialazeLayout() {
			InitializeGroupButtonsLayout();
			Size minLayoutControlSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height)
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), Math.Max(minLayoutControlSize.Height, ClientSize.Height));
			if(isRTLChanged)
				DevExpress.XtraPrinting.Native.RTLHelper.ConvertGroupControlAlignments(layoutControlGroup1);
		}
		void InitializeGroupButtonsLayout() {
			int btnOKBestWidth = buttonOk.CalcBestSize().Width;
			int btnCancelBestWidth = buttonCancel.CalcBestSize().Width;
			if(btnOKBestWidth <= buttonOk.Width && btnCancelBestWidth <= buttonCancel.Width)
				return;
			int btnCancelOKActualSize = Math.Max(btnOKBestWidth, btnCancelBestWidth);
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[1].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width = btnCancelOKActualSize + 2 + 2;
		}
		void button_Click(object sender, EventArgs e) {
			committed = (sender == buttonOk);
			HidePopup();
		}
		void radioAdjustToPersent_CheckedChanged(object sender, EventArgs e) {
			spinScalePercent.Enabled = labelAdjustTo.Enabled = radioAdjustToPersent.Checked;
			if(radioAdjustToPersent.Checked == true) 
				SetSpinEditFocus(spinScalePercent);
		}
		void radioFitToPageWidth_CheckedChanged(object sender, EventArgs e) {
			spinPagesWide.Enabled = labelPagesWide.Enabled = radioFitToPageWidth.Checked;
			if(radioFitToPageWidth.Checked == true) 
				SetSpinEditFocus(spinPagesWide);	   
		}
		void SetSpinEditFocus(SpinEdit spinEdit) {
			spinEdit.Focus();
			spinEdit.SelectAll();
		}
		DevExpress.Utils.Menu.IDXMenuManager DevExpress.Utils.Menu.IDXMenuManagerProvider.MenuManager {
			get { return this.Manager; }
		}
	}
}
