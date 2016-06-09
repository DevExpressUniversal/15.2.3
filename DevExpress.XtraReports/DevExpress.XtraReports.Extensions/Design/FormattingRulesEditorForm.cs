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
using System.Text;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using System.Drawing;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils.Controls;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Design {
	public class FormattingRulesEditorForm : ReportsEditorFormBase {
		#region fields
		private DevExpress.XtraEditors.SimpleButton cancelButton;
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraEditors.SimpleButton downButton;
		private DevExpress.XtraEditors.SimpleButton upButton;
		private DevExpress.XtraEditors.SimpleButton removeRangeButton;
		private DevExpress.XtraEditors.SimpleButton removeButton;
		private DevExpress.XtraEditors.SimpleButton addRangeButton;
		private DevExpress.XtraEditors.SimpleButton addButton;
		private DevExpress.XtraEditors.SimpleButton editSheetButton;
		private DevExpress.XtraEditors.LabelControl appliedRulesLabel;
		private DevExpress.XtraEditors.LabelControl availableRulesLabel;
		private DevExpress.XtraEditors.ListBoxControl formattingRulesListBox;
		private DevExpress.XtraEditors.ListBoxControl ruleSheetListBox;
		private DevExpress.XtraEditors.SimpleButton okButton;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.LayoutControlGroup grpButtons;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup4;
		private DevExpress.XtraEditors.LabelControl emptySpaceLabelControl;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
		private DevExpress.XtraEditors.LabelControl separatorLabel;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
		IServiceProvider provider;
		XtraReport report;
		ImageCollection icons;
		object[] controls;
		FormattingRuleCollection editValue;
		#endregion
		public FormattingRuleCollection EditValue { get { return editValue; } }
		FormattingRulesEditorForm() : this(null, new object[] { new XRControl() }) { }
		public FormattingRulesEditorForm(IServiceProvider servProvider, object[] controls)
			: base(servProvider) {
			this.controls = controls;
			editValue = ((XRControl)controls[0]).FormattingRules;
			InitializeComponent();
			InitializeIcons();
			this.provider = servProvider;
			this.report = servProvider!= null ? ((IDesignerHost)servProvider.GetService(typeof(IDesignerHost))).RootComponent as XtraReport : new XtraReport();
			FillListBoxes(true);
			UpdateButtons();
		}
		void InitializeIcons() {
			this.icons = ImageHelper.CreateImageCollectionFromResources(LocalResFinder.GetFullName("Images.ArrowsIcons.png"), LocalResFinder.Assembly, new Size(15, 15));
			this.addButton.Image = icons.Images[0];
			this.addRangeButton.Image = icons.Images[1];
			this.removeButton.Image = icons.Images[2];
			this.removeRangeButton.Image = icons.Images[3];	   
			this.upButton.Image = icons.Images[4];
			this.downButton.Image = icons.Images[5];
		}
		void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormattingRulesEditorForm));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition9 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition10 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition11 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition21 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition22 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition23 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition7 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition8 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition9 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition10 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition11 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition12 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition8 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition13 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition14 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition15 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition16 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition17 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition18 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition19 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition20 = new DevExpress.XtraLayout.RowDefinition();
			this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.separatorLabel = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl = new DevExpress.XtraEditors.LabelControl();
			this.downButton = new DevExpress.XtraEditors.SimpleButton();
			this.upButton = new DevExpress.XtraEditors.SimpleButton();
			this.removeRangeButton = new DevExpress.XtraEditors.SimpleButton();
			this.removeButton = new DevExpress.XtraEditors.SimpleButton();
			this.addRangeButton = new DevExpress.XtraEditors.SimpleButton();
			this.addButton = new DevExpress.XtraEditors.SimpleButton();
			this.editSheetButton = new DevExpress.XtraEditors.SimpleButton();
			this.appliedRulesLabel = new DevExpress.XtraEditors.LabelControl();
			this.availableRulesLabel = new DevExpress.XtraEditors.LabelControl();
			this.formattingRulesListBox = new DevExpress.XtraEditors.ListBoxControl();
			this.ruleSheetListBox = new DevExpress.XtraEditors.ListBoxControl();
			this.okButton = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.formattingRulesListBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ruleSheetListBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.StyleController = this.layoutControl1;
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.separatorLabel);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl);
			this.layoutControl1.Controls.Add(this.downButton);
			this.layoutControl1.Controls.Add(this.upButton);
			this.layoutControl1.Controls.Add(this.removeRangeButton);
			this.layoutControl1.Controls.Add(this.removeButton);
			this.layoutControl1.Controls.Add(this.addRangeButton);
			this.layoutControl1.Controls.Add(this.addButton);
			this.layoutControl1.Controls.Add(this.editSheetButton);
			this.layoutControl1.Controls.Add(this.appliedRulesLabel);
			this.layoutControl1.Controls.Add(this.availableRulesLabel);
			this.layoutControl1.Controls.Add(this.formattingRulesListBox);
			this.layoutControl1.Controls.Add(this.ruleSheetListBox);
			this.layoutControl1.Controls.Add(this.okButton);
			this.layoutControl1.Controls.Add(this.cancelButton);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(610, 56, 972, 778);
			this.layoutControl1.Root = this.layoutControlGroup1;
			resources.ApplyResources(this.separatorLabel, "separatorLabel");
			this.separatorLabel.Name = "separatorLabel";
			this.separatorLabel.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl, "emptySpaceLabelControl");
			this.emptySpaceLabelControl.Name = "emptySpaceLabelControl";
			this.emptySpaceLabelControl.StyleController = this.layoutControl1;
			resources.ApplyResources(this.downButton, "downButton");
			this.downButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.downButton.Name = "downButton";
			this.downButton.StyleController = this.layoutControl1;
			this.downButton.Click += new System.EventHandler(this.downButton_Click);
			resources.ApplyResources(this.upButton, "upButton");
			this.upButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.upButton.Name = "upButton";
			this.upButton.StyleController = this.layoutControl1;
			this.upButton.Click += new System.EventHandler(this.upButton_Click);
			resources.ApplyResources(this.removeRangeButton, "removeRangeButton");
			this.removeRangeButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.removeRangeButton.Name = "removeRangeButton";
			this.removeRangeButton.StyleController = this.layoutControl1;
			this.removeRangeButton.Click += new System.EventHandler(this.removeRangeButton_Click);
			resources.ApplyResources(this.removeButton, "removeButton");
			this.removeButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.removeButton.Name = "removeButton";
			this.removeButton.StyleController = this.layoutControl1;
			this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
			resources.ApplyResources(this.addRangeButton, "addRangeButton");
			this.addRangeButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.addRangeButton.Name = "addRangeButton";
			this.addRangeButton.StyleController = this.layoutControl1;
			this.addRangeButton.Click += new System.EventHandler(this.addRangeButton_Click);
			resources.ApplyResources(this.addButton, "addButton");
			this.addButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.addButton.Name = "addButton";
			this.addButton.StyleController = this.layoutControl1;
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			resources.ApplyResources(this.editSheetButton, "editSheetButton");
			this.editSheetButton.Name = "editSheetButton";
			this.editSheetButton.StyleController = this.layoutControl1;
			this.editSheetButton.Click += new System.EventHandler(this.editSheetButton_Click);
			resources.ApplyResources(this.appliedRulesLabel, "appliedRulesLabel");
			this.appliedRulesLabel.Name = "appliedRulesLabel";
			this.appliedRulesLabel.StyleController = this.layoutControl1;
			resources.ApplyResources(this.availableRulesLabel, "availableRulesLabel");
			this.availableRulesLabel.Name = "availableRulesLabel";
			this.availableRulesLabel.StyleController = this.layoutControl1;
			resources.ApplyResources(this.formattingRulesListBox, "formattingRulesListBox");
			this.formattingRulesListBox.Name = "formattingRulesListBox";
			this.formattingRulesListBox.StyleController = this.layoutControl1;
			this.formattingRulesListBox.SelectedIndexChanged += new System.EventHandler(this.formattingRulesListBox_SelectedIndexChanged);
			resources.ApplyResources(this.ruleSheetListBox, "ruleSheetListBox");
			this.ruleSheetListBox.Name = "ruleSheetListBox";
			this.ruleSheetListBox.StyleController = this.layoutControl1;
			this.ruleSheetListBox.SelectedIndexChanged += new System.EventHandler(this.ruleSheetListBox_SelectedIndexChanged);
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.Name = "okButton";
			this.okButton.StyleController = this.layoutControl1;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.grpButtons,
			this.layoutControlGroup2,
			this.layoutControlGroup3,
			this.layoutControlGroup4,
			this.layoutControlItem6,
			this.layoutControlItem7});
			this.layoutControlGroup1.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			columnDefinition9.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition9.Width = 50D;
			columnDefinition10.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition10.Width = 30D;
			columnDefinition11.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition11.Width = 50D;
			this.layoutControlGroup1.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition9,
			columnDefinition10,
			columnDefinition11});
			rowDefinition21.Height = 17D;
			rowDefinition21.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition22.Height = 100D;
			rowDefinition22.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition23.Height = 37D;
			rowDefinition23.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup1.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition21,
			rowDefinition22,
			rowDefinition23});
			this.layoutControlGroup1.Size = new System.Drawing.Size(531, 387);
			this.layoutControlGroup1.TextVisible = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem2,
			this.layoutControlItem3});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 330);
			this.grpButtons.Name = "grpButtons";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition1.Width = 100D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition2.Width = 80D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition3.Width = 1D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition4.Width = 80D;
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition5.Width = 2D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4,
			columnDefinition5});
			rowDefinition1.Height = 11D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition2.Height = 26D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2});
			this.grpButtons.OptionsTableLayoutItem.ColumnSpan = 3;
			this.grpButtons.OptionsTableLayoutItem.RowIndex = 2;
			this.grpButtons.Size = new System.Drawing.Size(511, 37);
			this.layoutControlItem2.Control = this.cancelButton;
			this.layoutControlItem2.Location = new System.Drawing.Point(429, 11);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem2.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem2.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem3.Control = this.okButton;
			this.layoutControlItem3.Location = new System.Drawing.Point(348, 11);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem3.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem3.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlGroup2.GroupBordersVisible = false;
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem5});
			this.layoutControlGroup2.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup2.Location = new System.Drawing.Point(270, 17);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition6.Width = 100D;
			this.layoutControlGroup2.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition6});
			rowDefinition3.Height = 100D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
			this.layoutControlGroup2.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition3});
			this.layoutControlGroup2.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlGroup2.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlGroup2.Size = new System.Drawing.Size(241, 313);
			this.layoutControlItem5.Control = this.formattingRulesListBox;
			this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(241, 313);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlGroup3.GroupBordersVisible = false;
			this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem4,
			this.layoutControlItem8});
			this.layoutControlGroup3.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup3.Location = new System.Drawing.Point(0, 17);
			this.layoutControlGroup3.Name = "layoutControlGroup3";
			columnDefinition7.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition7.Width = 100D;
			this.layoutControlGroup3.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition7});
			rowDefinition4.Height = 27D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition5.Height = 27D;
			rowDefinition5.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition6.Height = 27D;
			rowDefinition6.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition7.Height = 27D;
			rowDefinition7.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition8.Height = 5D;
			rowDefinition8.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition9.Height = 27D;
			rowDefinition9.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition10.Height = 27D;
			rowDefinition10.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition11.Height = double.NaN;
			rowDefinition11.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition12.Height = 26D;
			rowDefinition12.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup3.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition4,
			rowDefinition5,
			rowDefinition6,
			rowDefinition7,
			rowDefinition8,
			rowDefinition9,
			rowDefinition10,
			rowDefinition11,
			rowDefinition12});
			this.layoutControlGroup3.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlGroup3.Size = new System.Drawing.Size(240, 313);
			this.layoutControlItem4.Control = this.ruleSheetListBox;
			this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.RowSpan = 8;
			this.layoutControlItem4.Size = new System.Drawing.Size(240, 287);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem8.Control = this.editSheetButton;
			this.layoutControlItem8.Location = new System.Drawing.Point(0, 287);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.OptionsTableLayoutItem.RowIndex = 8;
			this.layoutControlItem8.Size = new System.Drawing.Size(240, 26);
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.layoutControlGroup4.GroupBordersVisible = false;
			this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem9,
			this.layoutControlItem10,
			this.layoutControlItem11,
			this.layoutControlItem12,
			this.layoutControlItem13,
			this.layoutControlItem14,
			this.layoutControlItem15,
			this.layoutControlItem16});
			this.layoutControlGroup4.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup4.Location = new System.Drawing.Point(240, 17);
			this.layoutControlGroup4.Name = "layoutControlGroup4";
			columnDefinition8.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition8.Width = 100D;
			this.layoutControlGroup4.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition8});
			rowDefinition13.Height = 24D;
			rowDefinition13.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition14.Height = 24D;
			rowDefinition14.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition15.Height = 24D;
			rowDefinition15.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition16.Height = 24D;
			rowDefinition16.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition17.Height = 13D;
			rowDefinition17.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition18.Height = 24D;
			rowDefinition18.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition19.Height = 24D;
			rowDefinition19.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition20.Height = 100D;
			rowDefinition20.SizeType = System.Windows.Forms.SizeType.Percent;
			this.layoutControlGroup4.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition13,
			rowDefinition14,
			rowDefinition15,
			rowDefinition16,
			rowDefinition17,
			rowDefinition18,
			rowDefinition19,
			rowDefinition20});
			this.layoutControlGroup4.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlGroup4.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlGroup4.Size = new System.Drawing.Size(30, 313);
			this.layoutControlItem9.Control = this.addButton;
			this.layoutControlItem9.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem9.Name = "layoutControlItem9";
			this.layoutControlItem9.Size = new System.Drawing.Size(30, 24);
			this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem9.TextVisible = false;
			this.layoutControlItem10.Control = this.addRangeButton;
			this.layoutControlItem10.Location = new System.Drawing.Point(0, 24);
			this.layoutControlItem10.Name = "layoutControlItem10";
			this.layoutControlItem10.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem10.Size = new System.Drawing.Size(30, 24);
			this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem10.TextVisible = false;
			this.layoutControlItem11.Control = this.removeButton;
			this.layoutControlItem11.Location = new System.Drawing.Point(0, 48);
			this.layoutControlItem11.Name = "layoutControlItem11";
			this.layoutControlItem11.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem11.Size = new System.Drawing.Size(30, 24);
			this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem11.TextVisible = false;
			this.layoutControlItem12.Control = this.removeRangeButton;
			this.layoutControlItem12.Location = new System.Drawing.Point(0, 72);
			this.layoutControlItem12.Name = "layoutControlItem12";
			this.layoutControlItem12.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem12.Size = new System.Drawing.Size(30, 24);
			this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem12.TextVisible = false;
			this.layoutControlItem13.Control = this.upButton;
			this.layoutControlItem13.Location = new System.Drawing.Point(0, 109);
			this.layoutControlItem13.Name = "layoutControlItem13";
			this.layoutControlItem13.OptionsTableLayoutItem.RowIndex = 5;
			this.layoutControlItem13.Size = new System.Drawing.Size(30, 24);
			this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem13.TextVisible = false;
			this.layoutControlItem14.Control = this.downButton;
			this.layoutControlItem14.Location = new System.Drawing.Point(0, 133);
			this.layoutControlItem14.Name = "layoutControlItem14";
			this.layoutControlItem14.OptionsTableLayoutItem.RowIndex = 6;
			this.layoutControlItem14.Size = new System.Drawing.Size(30, 24);
			this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem14.TextVisible = false;
			this.layoutControlItem15.Control = this.emptySpaceLabelControl;
			this.layoutControlItem15.Location = new System.Drawing.Point(0, 157);
			this.layoutControlItem15.Name = "layoutControlItem15";
			this.layoutControlItem15.OptionsTableLayoutItem.RowIndex = 7;
			this.layoutControlItem15.Size = new System.Drawing.Size(30, 156);
			this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem15.TextVisible = false;
			this.layoutControlItem16.Control = this.separatorLabel;
			this.layoutControlItem16.Location = new System.Drawing.Point(0, 96);
			this.layoutControlItem16.Name = "layoutControlItem16";
			this.layoutControlItem16.OptionsTableLayoutItem.RowIndex = 4;
			this.layoutControlItem16.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem16.Size = new System.Drawing.Size(30, 13);
			this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem16.TextVisible = false;
			this.layoutControlItem6.Control = this.availableRulesLabel;
			this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Size = new System.Drawing.Size(240, 17);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem7.Control = this.appliedRulesLabel;
			this.layoutControlItem7.Location = new System.Drawing.Point(270, 0);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem7.Size = new System.Drawing.Size(241, 17);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ControlBox = false;
			this.Controls.Add(this.layoutControl1);
			this.Name = "FormattingRulesEditorForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Load += new System.EventHandler(this.FormattingRulesEditorForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.formattingRulesListBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ruleSheetListBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			this.ResumeLayout(false);
		}
		void FillListBoxes(bool fillFormattingRules) {
			formattingRulesListBox.BeginUpdate();
			if(fillFormattingRules) {
				formattingRulesListBox.Items.Clear();
				foreach(FormattingRule rule in editValue)
					formattingRulesListBox.Items.Add(rule);
			}
			ValidateFormattingRulesListBox();
			formattingRulesListBox.EndUpdate();
			ruleSheetListBox.Items.Clear();
			List<FormattingRule> rules = new List<FormattingRule>(report.FormattingRuleSheet.Count);
			foreach(FormattingRule rule in report.FormattingRuleSheet)
				if(!formattingRulesListBox.Items.Contains(rule))
					rules.Add(rule);
			using(var comparer = new DevExpress.Utils.NaturalStringComparer(rules.Count))
				rules.Sort((a, b) => comparer.Compare(a.Name, b.Name));
			ruleSheetListBox.Items.AddRange(rules.ToArray());
		}
		void ValidateFormattingRulesListBox() {
			for(int i = 0; i < formattingRulesListBox.ItemCount; )
				if(!report.FormattingRuleSheet.Contains(formattingRulesListBox.Items[i] as FormattingRule))
					formattingRulesListBox.Items.Remove(formattingRulesListBox.Items[i]);
				else i++;
		}
		void addButton_Click(object sender, EventArgs e) {
			if(ruleSheetListBox.SelectedItem != null) {
				formattingRulesListBox.Items.Add(ruleSheetListBox.SelectedItem);
				ruleSheetListBox.Items.Remove(ruleSheetListBox.SelectedItem);
			}
		}
		void removeButton_Click(object sender, EventArgs e) {
			if(formattingRulesListBox.SelectedItem != null) {
				ruleSheetListBox.Items.Add(formattingRulesListBox.SelectedItem);
				formattingRulesListBox.Items.Remove(formattingRulesListBox.SelectedItem);
			}
		}
		void addRangeButton_Click(object sender, EventArgs e) {
			foreach(FormattingRule item in ruleSheetListBox.Items)
				formattingRulesListBox.Items.Add(item);
			ruleSheetListBox.Items.Clear();
		}
		void removeRangeButton_Click(object sender, EventArgs e) {
			foreach(FormattingRule item in formattingRulesListBox.Items)
				ruleSheetListBox.Items.Add(item);
			formattingRulesListBox.Items.Clear();
		}
		void okButton_Click(object sender, EventArgs e) {
			IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
			IComponentChangeService changeService = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
			string description = controls.Length == 1 ?
				String.Format(DesignSR.TransFmt_ChangeFormattingRules, ((XRControl)controls[0]).Site.Name) :
				String.Format(DesignSR.Trans_ChangeProp, XRComponentPropertyNames.FormattingRules);
			DesignerTransaction transaction = host.CreateTransaction(description);
			try {
				foreach(XRControl control in controls) {
					changeService.OnComponentChanging(control, XRAccessor.GetPropertyDescriptor(control, XRComponentPropertyNames.FormattingRules));
					control.FormattingRules.Clear();
					foreach(FormattingRule item in formattingRulesListBox.Items)
						control.FormattingRules.Add(item);
					changeService.OnComponentChanged(control, XRAccessor.GetPropertyDescriptor(control, XRComponentPropertyNames.FormattingRules), null, null);
				}
				transaction.Commit();
			}
			catch {
				transaction.Cancel();
			}
			Close();
		}
		void upButton_Click(object sender, EventArgs e) {
			int insertedIndex = formattingRulesListBox.SelectedIndex - 1;
			formattingRulesListBox.Items.Insert(insertedIndex, formattingRulesListBox.SelectedItem);
			formattingRulesListBox.Items.RemoveAt(formattingRulesListBox.SelectedIndex);
			formattingRulesListBox.SelectedIndex = insertedIndex;
		}
		void downButton_Click(object sender, EventArgs e) {
			formattingRulesListBox.Items.Insert(formattingRulesListBox.SelectedIndex + 2, formattingRulesListBox.SelectedItem);
			formattingRulesListBox.Items.RemoveAt(formattingRulesListBox.SelectedIndex);
			formattingRulesListBox.SelectedIndex++;
		}
		void formattingRulesListBox_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateButtons();
		}
		void Items_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
			UpdateButtons();
		}
		void ruleSheetListBox_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateButtons();
		}
		void editSheetButton_Click(object sender, EventArgs e) {
			if(report != null) {
				try {
					IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					if(edSvc != null) {
						using(FormattingRuleSheetEditorForm form = new FormattingRuleSheetEditorForm(report, provider)) {
							edSvc.ShowDialog(form);
						}
					}
				}
				catch { }
			}
			FillListBoxes(false);
		}
		void UpdateButtons() {
			upButton.Enabled = formattingRulesListBox.SelectedIndex - 1 >= 0;
			downButton.Enabled = formattingRulesListBox.SelectedIndex != formattingRulesListBox.ItemCount - 1;
			addButton.Enabled = ruleSheetListBox.ItemCount > 0;
			addRangeButton.Enabled = addButton.Enabled;
			removeButton.Enabled = formattingRulesListBox.ItemCount > 0;
			removeRangeButton.Enabled = removeButton.Enabled;
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			DevExpress.XtraPrinting.Native.RTLHelper.ConvertGroupControlAlignments(layoutControlGroup1);
		}
		private void FormattingRulesEditorForm_Load(object sender, EventArgs e) {
			InitializeLayout();
		}
		void InitializeLayout() {
			InitializeButtonsLayout();
			Size minLayoutControlSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height)
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), Math.Max(minLayoutControlSize.Height, ClientSize.Height));
		}
		void InitializeButtonsLayout() {
			int delta = addButton.Width + 2 + 2 - (int)layoutControlGroup1.OptionsTableLayoutGroup.ColumnDefinitions[1].Width;
			if(delta > 0) {
				layoutControlGroup1.OptionsTableLayoutGroup.ColumnDefinitions[1].Width += delta;
				layoutControl1.Width += delta;
			}
			int okButtonBestWidth = okButton.CalcBestSize().Width;
			int cancelButtonBestWidth = cancelButton.CalcBestSize().Width;
			if(okButtonBestWidth <= okButton.Width && cancelButtonBestWidth <= cancelButton.Width)
				return;
			int btnPrintOKActualSize = Math.Max(okButtonBestWidth, cancelButtonBestWidth);
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[1].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width = btnPrintOKActualSize + 2 + 2;
		}
	}
}
