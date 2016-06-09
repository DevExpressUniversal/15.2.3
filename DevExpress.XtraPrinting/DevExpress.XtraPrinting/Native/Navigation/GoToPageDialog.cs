﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraLayout;
namespace DevExpress.XtraPrinting.Native {
	public class GoToPageDialog : XtraForm {
		private System.ComponentModel.IContainer components = null;
		private LayoutControl layoutControl1;
		private LayoutControlGroup Root;
		private LabelControl lbCaption;
		private SimpleButton btnOk;
		private TextEdit txtPageNumber;
		private SimpleButton btnCancel;
		private LayoutControlItem layoutControlItem1;
		private LayoutControlItem layoutControlItem3;
		private LayoutControlGroup grpButtons;
		private LayoutControlItem layoutControlItem2;
		private LayoutControlItem layoutControlItem4;
		private LabelControl emptySpaceLabel1;
		private LayoutControlItem emptySpaceLabelControl1;
		private LabelControl emptySpaceLabel2;
		private LayoutControlItem emptySpaceLabelControl2;
		int countPages = 0;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GoToPageDialog));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.emptySpaceLabel2 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabel1 = new DevExpress.XtraEditors.LabelControl();
			this.lbCaption = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.txtPageNumber = new DevExpress.XtraEditors.TextEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceLabelControl1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceLabelControl2 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtPageNumber.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceLabelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceLabelControl2)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.emptySpaceLabel2);
			this.layoutControl1.Controls.Add(this.emptySpaceLabel1);
			this.layoutControl1.Controls.Add(this.lbCaption);
			this.layoutControl1.Controls.Add(this.btnOk);
			this.layoutControl1.Controls.Add(this.txtPageNumber);
			this.layoutControl1.Controls.Add(this.btnCancel);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(173, 292, 1116, 652);
			this.layoutControl1.Root = this.Root;
			resources.ApplyResources(this.emptySpaceLabel2, "emptySpaceLabel2");
			this.emptySpaceLabel2.Name = "emptySpaceLabel2";
			this.emptySpaceLabel2.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabel1, "emptySpaceLabel1");
			this.emptySpaceLabel1.Name = "emptySpaceLabel1";
			this.emptySpaceLabel1.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.lbCaption.Name = "lbCaption";
			this.lbCaption.StyleController = this.layoutControl1;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.StyleController = this.layoutControl1;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.txtPageNumber, "txtPageNumber");
			this.txtPageNumber.Name = "txtPageNumber";
			this.txtPageNumber.Properties.Mask.EditMask = resources.GetString("txtPageNumber.Properties.Mask.EditMask");
			this.txtPageNumber.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("txtPageNumber.Properties.Mask.MaskType")));
			this.txtPageNumber.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtPageNumber.Properties.Mask.ShowPlaceHolders")));
			this.txtPageNumber.StyleController = this.layoutControl1;
			this.txtPageNumber.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtPageNumber_InvalidValue);
			this.txtPageNumber.Validating += new System.ComponentModel.CancelEventHandler(this.txtPageNumber_Validating);
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl1;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.Root.GroupBordersVisible = false;
			this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem3,
			this.grpButtons,
			this.emptySpaceLabelControl1,
			this.emptySpaceLabelControl2});
			this.Root.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.Root.Location = new System.Drawing.Point(0, 0);
			this.Root.Name = "Root";
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition5.Width = 244D;
			this.Root.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition5});
			rowDefinition2.Height = 15D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition3.Height = 17D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition4.Height = 24D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition5.Height = 15D;
			rowDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition6.Height = 26D;
			rowDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.Root.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition2,
			rowDefinition3,
			rowDefinition4,
			rowDefinition5,
			rowDefinition6});
			this.Root.Size = new System.Drawing.Size(264, 117);
			this.layoutControlItem1.Control = this.lbCaption;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 15);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem1.Size = new System.Drawing.Size(244, 17);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem3.Control = this.txtPageNumber;
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 32);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem3.Size = new System.Drawing.Size(244, 24);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem2,
			this.layoutControlItem4});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 71);
			this.grpButtons.Name = "grpButtons";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition1.Width = 100D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition2.Width = 80D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition3.Width = 2D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition4.Width = 80D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4});
			rowDefinition1.Height = 26D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1});
			this.grpButtons.OptionsTableLayoutItem.RowIndex = 4;
			this.grpButtons.Size = new System.Drawing.Size(244, 26);
			this.layoutControlItem2.Control = this.btnOk;
			this.layoutControlItem2.Location = new System.Drawing.Point(82, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem2.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem4.Control = this.btnCancel;
			this.layoutControlItem4.Location = new System.Drawing.Point(164, 0);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem4.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.emptySpaceLabelControl1.Control = this.emptySpaceLabel1;
			this.emptySpaceLabelControl1.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceLabelControl1.Name = "emptySpaceLabelControl1";
			this.emptySpaceLabelControl1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 1, 1);
			this.emptySpaceLabelControl1.Size = new System.Drawing.Size(244, 15);
			this.emptySpaceLabelControl1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceLabelControl1.TextVisible = false;
			this.emptySpaceLabelControl2.Control = this.emptySpaceLabel2;
			this.emptySpaceLabelControl2.Location = new System.Drawing.Point(0, 56);
			this.emptySpaceLabelControl2.Name = "emptySpaceLabelControl2";
			this.emptySpaceLabelControl2.OptionsTableLayoutItem.RowIndex = 3;
			this.emptySpaceLabelControl2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 1, 1);
			this.emptySpaceLabelControl2.Size = new System.Drawing.Size(244, 15);
			this.emptySpaceLabelControl2.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceLabelControl2.TextVisible = false;
			this.AcceptButton = this.btnOk;
			this.CancelButton = this.btnCancel;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GoToPageDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.GoToPageDialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtPageNumber.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceLabelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceLabelControl2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public GoToPageDialog(int countPages) {
			InitializeComponent();
			EditorContextMenuLookAndFeelHelper.InitBarManager(ref this.components, this);
			this.countPages = countPages;
			this.lbCaption.Text = string.Format(this.lbCaption.Text, countPages);
		}
		GoToPageDialog() : this(1) { }
		public int PageNumber { get; set; }
		string PageNumberText { get { return PageNumber == -1 ? txtPageNumber.Text : PageNumber.ToString(); } }
		private void txtPageNumber_Validating(object sender, CancelEventArgs e) {
			int result;
			PageNumber = Int32.TryParse(txtPageNumber.Text, out result) ? result : -1;
			e.Cancel = PageNumber <= 0 || PageNumber > countPages;
		}
		private void txtPageNumber_InvalidValue(object sender, XtraEditors.Controls.InvalidValueExceptionEventArgs e) {
			e.ErrorText = string.Format(PreviewLocalizer.GetString(PreviewStringId.Msg_GoToNonExistentPage), PageNumberText);
		}
		private void btnOk_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
		}
		private void btnCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Escape) {
				DialogResult = DialogResult.Cancel;
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		private void GoToPageDialog_Load(object sender, EventArgs e) {
			InitializeLayout();
		}
		void InitializeLayout() {
			InitializeGroupButtonsLayout();
			Size minLayoutControlSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height) {
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), Math.Max(minLayoutControlSize.Height, ClientSize.Height));
			}
		}
		void InitializeGroupButtonsLayout() {
			int btnOkBestWidth = btnOk.CalcBestSize().Width;
			int btnCancelBestWidth = btnCancel.CalcBestSize().Width;
			if(btnOkBestWidth <= btnOk.Width && btnCancelBestWidth <= btnCancel.Width)
				return;
			int btnCancelOKActualSize = Math.Max(btnOkBestWidth, btnCancelBestWidth);
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[1].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width = btnCancelOKActualSize + 2 + 2;
			if(grpButtons.Width < 2 * (btnCancelOKActualSize + 2 + 2))
				grpButtons.Width = 2 * (btnCancelOKActualSize + 2 + 2);
		}
	}
}
