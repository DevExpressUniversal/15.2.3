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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Wizards;
namespace DevExpress.XtraReports.Design
{
	[ToolboxItem(false)]
	public class WizPageLabelOptions : DevExpress.Utils.InteriorWizardPage
	{
		private DevExpress.XtraEditors.SpinEdit tbTopMargin;
		private System.Windows.Forms.Label lblSideMarginStatic;
		private System.Windows.Forms.Label lblTopMarginStatic;
		private DevExpress.XtraEditors.SpinEdit tbSideMargin;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lblPageSize;
		private System.Windows.Forms.Label lblHorizontalPitchStatic;
		private System.Windows.Forms.Label lblVerticalPitchStatic;
		private System.Windows.Forms.Label lblLabelHeightStatic;
		private System.Windows.Forms.Label lblLabelWidthStatic;
		private System.ComponentModel.IContainer components = null;
		private DevExpress.XtraEditors.SpinEdit tbVerticalPitch;
		private DevExpress.XtraEditors.SpinEdit tbHorizontalPitch;
		private DevExpress.XtraEditors.SpinEdit tbLabelWidth;
		private DevExpress.XtraEditors.SpinEdit tbLabelHeight;
		PreviewControl preview;
		XRWizardRunnerBase runner;
		private DevExpress.XtraEditors.LookUpEdit cbPageSize;
		bool contentCreated;
		LabelInfo LabelInfo { get { return preview.LabelInfo; } }
		LabelReportWizard Wiz { get { return runner.Wizard as LabelReportWizard; } }
		PaperKindList PaperKindList {
			get {
				LabelReportWizard wizard = Wiz;
				return wizard != null ? wizard.PaperKindList: null;
			}
		}
		public WizPageLabelOptions(XRWizardRunnerBase runner) : this() {
			this.runner = runner;
			preview.LabelInfo = new LabelInfo();
			SubscribeEvents();
			UpdateEdits(true);
		}
		WizPageLabelOptions() {
			InitializeComponent();
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		SizeF GetPageSize(float dpi) {
			PaperKindItem item = cbPageSize.EditValue as PaperKindItem;
			return item != null ? XRConvert.Convert(item.Size, GraphicsDpi.HundredthsOfAnInch, dpi) :
				XRConvert.Convert(runner.Report.PageSize, runner.Report.Dpi, dpi);
		}
		void SubscribeEvents() {
			this.tbTopMargin.TextChanged += new System.EventHandler(this.any_TextChanged);
			this.tbSideMargin.TextChanged += new System.EventHandler(this.any_TextChanged);
			this.tbVerticalPitch.TextChanged += new System.EventHandler(this.any_TextChanged);
			this.tbHorizontalPitch.TextChanged += new System.EventHandler(this.any_TextChanged);
			this.tbLabelWidth.TextChanged += new System.EventHandler(this.any_TextChanged);
			this.tbLabelHeight.TextChanged += new System.EventHandler(this.any_TextChanged);
		}
		void UnsubscribeEvents() {
			this.tbTopMargin.TextChanged -= new System.EventHandler(this.any_TextChanged);
			this.tbSideMargin.TextChanged -= new System.EventHandler(this.any_TextChanged);
			this.tbVerticalPitch.TextChanged -= new System.EventHandler(this.any_TextChanged);
			this.tbHorizontalPitch.TextChanged -= new System.EventHandler(this.any_TextChanged);
			this.tbLabelWidth.TextChanged -= new System.EventHandler(this.any_TextChanged);
			this.tbLabelHeight.TextChanged -= new System.EventHandler(this.any_TextChanged);
		}
		void UpdateEdits(bool updateValues) {
			UnsubscribeEvents();
			SizeF pageSize = GetPageSize(runner.Report.Dpi);
			tbSideMargin.Properties.MaxValue = Convert.ToDecimal(pageSize.Width);
			tbHorizontalPitch.Properties.MaxValue = Convert.ToDecimal(pageSize.Width);
			tbLabelWidth.Properties.MaxValue = Convert.ToDecimal(pageSize.Width);
			tbVerticalPitch.Properties.MaxValue = Convert.ToDecimal(pageSize.Height);
			tbLabelHeight.Properties.MaxValue = Convert.ToDecimal(pageSize.Height);
			tbTopMargin.Properties.MaxValue = Convert.ToDecimal(pageSize.Height);
			if(updateValues) {
				tbVerticalPitch.Value = Convert.ToDecimal(LabelInfo.VPitch);
				tbHorizontalPitch.Value = Convert.ToDecimal(LabelInfo.HPitch);
				tbLabelWidth.Value = Convert.ToDecimal(LabelInfo.LabelWidth);
				tbLabelHeight.Value = Convert.ToDecimal(LabelInfo.LabelHeight);
				tbTopMargin.Value = Convert.ToDecimal(LabelInfo.TopMargin);
				tbSideMargin.Value = Convert.ToDecimal(LabelInfo.LeftMargin);
			} else {
				LabelInfo.VPitch = Convert.ToSingle(tbVerticalPitch.GetValidValue());
				LabelInfo.HPitch = Convert.ToSingle(tbHorizontalPitch.GetValidValue());
				LabelInfo.LabelWidth = Convert.ToSingle(tbLabelWidth.GetValidValue());
				LabelInfo.LabelHeight = Convert.ToSingle(tbLabelHeight.GetValidValue());
				LabelInfo.LeftMargin = Convert.ToInt32(tbSideMargin.GetValidValue());
				LabelInfo.TopMargin = Convert.ToInt32(tbTopMargin.GetValidValue());
				ValidateLabelInfo(LabelInfo, pageSize);
			}
			SubscribeEvents();
			preview.Invalidate();
		}
		void FillPageSizeCB() {
			this.cbPageSize.Properties.DataSource = PaperKindList;
			this.cbPageSize.Properties.DisplayMember = "FullText";
			this.cbPageSize.Properties.ValueMember = "Value";
		}
		protected override bool OnSetActive() {
			LabelInfo.LabelWidth = XRConvert.Convert(Wiz.LabelInfo.LabelWidth, GraphicsDpi.HundredthsOfAnInch, runner.Report.Dpi);
			LabelInfo.LabelHeight = XRConvert.Convert(Wiz.LabelInfo.LabelHeight, GraphicsDpi.HundredthsOfAnInch, runner.Report.Dpi);
			LabelInfo.HPitch = XRConvert.Convert(Wiz.LabelInfo.HPitch, GraphicsDpi.HundredthsOfAnInch, runner.Report.Dpi);
			LabelInfo.VPitch = XRConvert.Convert(Wiz.LabelInfo.VPitch, GraphicsDpi.HundredthsOfAnInch, runner.Report.Dpi);
			LabelInfo.TopMargin = XRConvert.Convert(Wiz.LabelInfo.TopMargin, GraphicsDpi.HundredthsOfAnInch, runner.Report.Dpi);
			LabelInfo.LeftMargin = XRConvert.Convert(Wiz.LabelInfo.LeftMargin, GraphicsDpi.HundredthsOfAnInch, runner.Report.Dpi);
			if(!contentCreated) {
				FillPageSizeCB();
				contentCreated = true;
			}
			cbPageSize.EditValue = PaperKindList[PaperKindList.CurrentIndex];
			UpdateEdits(true);
			return true;
		}
		protected override void UpdateWizardButtons() {
			Wizard.WizardButtons = WizardButton.Back |  WizardButton.Finish;
		}
		#region Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageLabelOptions));
			this.tbTopMargin = new DevExpress.XtraEditors.SpinEdit();
			this.lblSideMarginStatic = new System.Windows.Forms.Label();
			this.lblTopMarginStatic = new System.Windows.Forms.Label();
			this.tbSideMargin = new DevExpress.XtraEditors.SpinEdit();
			this.tbVerticalPitch = new DevExpress.XtraEditors.SpinEdit();
			this.lblHorizontalPitchStatic = new System.Windows.Forms.Label();
			this.lblVerticalPitchStatic = new System.Windows.Forms.Label();
			this.tbHorizontalPitch = new DevExpress.XtraEditors.SpinEdit();
			this.tbLabelWidth = new DevExpress.XtraEditors.SpinEdit();
			this.lblLabelHeightStatic = new System.Windows.Forms.Label();
			this.lblLabelWidthStatic = new System.Windows.Forms.Label();
			this.tbLabelHeight = new DevExpress.XtraEditors.SpinEdit();
			this.panel1 = new System.Windows.Forms.Panel();
			this.preview = new DevExpress.XtraReports.Design.PreviewControl();
			this.lblPageSize = new System.Windows.Forms.Label();
			this.cbPageSize = new DevExpress.XtraEditors.LookUpEdit();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbTopMargin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbSideMargin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbVerticalPitch.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbHorizontalPitch.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLabelWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLabelHeight.Properties)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPageSize.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.titleLabel, "titleLabel");
			resources.ApplyResources(this.subtitleLabel, "subtitleLabel");
			resources.ApplyResources(this.tbTopMargin, "tbTopMargin");
			this.tbTopMargin.Name = "tbTopMargin";
			this.tbTopMargin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.lblSideMarginStatic, "lblSideMarginStatic");
			this.lblSideMarginStatic.Name = "lblSideMarginStatic";
			resources.ApplyResources(this.lblTopMarginStatic, "lblTopMarginStatic");
			this.lblTopMarginStatic.Name = "lblTopMarginStatic";
			resources.ApplyResources(this.tbSideMargin, "tbSideMargin");
			this.tbSideMargin.Name = "tbSideMargin";
			this.tbSideMargin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.tbVerticalPitch, "tbVerticalPitch");
			this.tbVerticalPitch.Name = "tbVerticalPitch";
			this.tbVerticalPitch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.lblHorizontalPitchStatic, "lblHorizontalPitchStatic");
			this.lblHorizontalPitchStatic.Name = "lblHorizontalPitchStatic";
			resources.ApplyResources(this.lblVerticalPitchStatic, "lblVerticalPitchStatic");
			this.lblVerticalPitchStatic.Name = "lblVerticalPitchStatic";
			resources.ApplyResources(this.tbHorizontalPitch, "tbHorizontalPitch");
			this.tbHorizontalPitch.Name = "tbHorizontalPitch";
			this.tbHorizontalPitch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.tbLabelWidth, "tbLabelWidth");
			this.tbLabelWidth.Name = "tbLabelWidth";
			this.tbLabelWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.lblLabelHeightStatic, "lblLabelHeightStatic");
			this.lblLabelHeightStatic.Name = "lblLabelHeightStatic";
			resources.ApplyResources(this.lblLabelWidthStatic, "lblLabelWidthStatic");
			this.lblLabelWidthStatic.Name = "lblLabelWidthStatic";
			resources.ApplyResources(this.tbLabelHeight, "tbLabelHeight");
			this.tbLabelHeight.Name = "tbLabelHeight";
			this.tbLabelHeight.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.preview);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			this.preview.BackColor = System.Drawing.SystemColors.Window;
			resources.ApplyResources(this.preview, "preview");
			this.preview.Name = "preview";
			resources.ApplyResources(this.lblPageSize, "lblPageSize");
			this.lblPageSize.Name = "lblPageSize";
			resources.ApplyResources(this.cbPageSize, "cbPageSize");
			this.cbPageSize.Name = "cbPageSize";
			this.cbPageSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPageSize.Properties.Buttons"))))});
			this.cbPageSize.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FullText", "FullText", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None)});
			this.cbPageSize.Properties.ShowFooter = false;
			this.cbPageSize.Properties.ShowHeader = false;
			this.cbPageSize.Properties.ShowLines = false;
			this.Controls.Add(this.tbLabelWidth);
			this.Controls.Add(this.tbLabelHeight);
			this.Controls.Add(this.tbVerticalPitch);
			this.Controls.Add(this.tbHorizontalPitch);
			this.Controls.Add(this.tbTopMargin);
			this.Controls.Add(this.tbSideMargin);
			this.Controls.Add(this.cbPageSize);
			this.Controls.Add(this.lblPageSize);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.lblLabelHeightStatic);
			this.Controls.Add(this.lblLabelWidthStatic);
			this.Controls.Add(this.lblHorizontalPitchStatic);
			this.Controls.Add(this.lblVerticalPitchStatic);
			this.Controls.Add(this.lblSideMarginStatic);
			this.Controls.Add(this.lblTopMarginStatic);
			this.Name = "WizPageLabelOptions";
			this.Controls.SetChildIndex(this.lblTopMarginStatic, 0);
			this.Controls.SetChildIndex(this.lblSideMarginStatic, 0);
			this.Controls.SetChildIndex(this.lblVerticalPitchStatic, 0);
			this.Controls.SetChildIndex(this.lblHorizontalPitchStatic, 0);
			this.Controls.SetChildIndex(this.lblLabelWidthStatic, 0);
			this.Controls.SetChildIndex(this.lblLabelHeightStatic, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			this.Controls.SetChildIndex(this.panel1, 0);
			this.Controls.SetChildIndex(this.lblPageSize, 0);
			this.Controls.SetChildIndex(this.cbPageSize, 0);
			this.Controls.SetChildIndex(this.tbSideMargin, 0);
			this.Controls.SetChildIndex(this.tbTopMargin, 0);
			this.Controls.SetChildIndex(this.tbHorizontalPitch, 0);
			this.Controls.SetChildIndex(this.tbVerticalPitch, 0);
			this.Controls.SetChildIndex(this.tbLabelHeight, 0);
			this.Controls.SetChildIndex(this.tbLabelWidth, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbTopMargin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbSideMargin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbVerticalPitch.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbHorizontalPitch.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLabelWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLabelHeight.Properties)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPageSize.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private void any_TextChanged(object sender, System.EventArgs e) {
			UpdateEdits(false);
		}
		void ApplyChanges() {
			Wiz.LabelInfo.LabelWidth = XRConvert.Convert(LabelInfo.LabelWidth, runner.Report.Dpi, GraphicsDpi.HundredthsOfAnInch);
			Wiz.LabelInfo.LabelHeight = XRConvert.Convert(LabelInfo.LabelHeight, runner.Report.Dpi, GraphicsDpi.HundredthsOfAnInch);
			Wiz.LabelInfo.HPitch = XRConvert.Convert(LabelInfo.HPitch, runner.Report.Dpi, GraphicsDpi.HundredthsOfAnInch);
			Wiz.LabelInfo.VPitch = XRConvert.Convert(LabelInfo.VPitch, runner.Report.Dpi, GraphicsDpi.HundredthsOfAnInch);
			Wiz.LabelInfo.TopMargin = XRConvert.Convert(LabelInfo.TopMargin, runner.Report.Dpi, GraphicsDpi.HundredthsOfAnInch);
			Wiz.LabelInfo.LeftMargin = XRConvert.Convert(LabelInfo.LeftMargin, runner.Report.Dpi, GraphicsDpi.HundredthsOfAnInch);
			PaperKindList.CurrentIndex = PaperKindList.IndexOf(cbPageSize.EditValue);
		}
		static void ValidateLabelInfo(LabelInfo value, SizeF pageSize) {
			value.HPitch = Math.Min(value.HPitch, pageSize.Width);
			value.LabelWidth = Math.Min(value.LabelWidth, value.HPitch);
			value.LeftMargin = (int)Math.Min(value.LeftMargin, Math.Max(0, pageSize.Width - value.HPitch));
			value.VPitch = Math.Min(value.VPitch, pageSize.Height);
			value.LabelHeight = Math.Min(value.LabelHeight, value.VPitch);
			value.TopMargin = (int)Math.Min(value.TopMargin, Math.Max(0, pageSize.Height - value.VPitch));
		}
		protected override string OnWizardNext() {
			ApplyChanges();
			return WizardForm.NextPage;
		}
		protected override bool OnWizardFinish() {
			ApplyChanges();
			return true;
		}
	}
	public static class EditorExtensions {
		public static decimal GetValidValue(this DevExpress.XtraEditors.SpinEdit edit) {
			return Math.Min(edit.Properties.MaxValue, edit.Value);
		}
	}
}
