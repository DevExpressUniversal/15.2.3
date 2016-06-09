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
using System.Data;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Wizards;
namespace DevExpress.XtraReports.Design
{
	[ToolboxItem(false)]
	public class WizPageLabelType : DevExpress.Utils.InteriorWizardPage
	{
		private System.Windows.Forms.Panel panel1;
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label lblLabelProducts;
		private DevExpress.XtraEditors.ComboBoxEdit cbLabelProducts;
		private System.Windows.Forms.Label lblProductNumber;
		private DevExpress.XtraEditors.ComboBoxEdit cbProductNumber;
		private GroupControl grpLabelInfo;
		private System.Windows.Forms.Label lblWidthStatic;
		private System.Windows.Forms.Label lblHeightStatic;
		private System.Windows.Forms.Label lblPageSizeStatic;
		private System.Windows.Forms.Label lblPageSize;
		private System.Windows.Forms.Label lblHeight;
		private System.Windows.Forms.Label lblWidth;
		PreviewControl preview;
		LabelInfo labelInfo = new LabelInfo();
		ArrayList productNumberIndexList = new ArrayList();
		bool contentCreated;
		private System.Windows.Forms.Label lblPageType;
		private System.Windows.Forms.Label lblPageTypeStatic;
		XRWizardRunnerBase runner;
		LabelReportWizard Wiz { get { return runner.Wizard as LabelReportWizard;} }
		DataTable LabelProducts { 
			get {
				LabelReportWizard wizard = Wiz;
				return wizard != null ? wizard.LabelProducts: null;
			}
		}
		DataTable LabelDetails { 
			get {
				LabelReportWizard wizard = Wiz;
				return wizard != null ? wizard.LabelDetails: null;
			}
		}
		PaperKindList PaperKindList {
			get {
				LabelReportWizard wizard = Wiz;
				return wizard != null ? wizard.PaperKindList: null;
			}
		}
		public WizPageLabelType(XRWizardRunnerBase runner) : this() {
			this.runner = runner;
		}
		public WizPageLabelType()
		{
			InitializeComponent();
			preview.LabelInfo = labelInfo;
		}
		protected override bool OnSetActive() {
			if(!contentCreated) {
				FillLabelProductsCB();
				contentCreated = true;
			}
			return true;
		}
		protected override void UpdateWizardButtons() {
			Wizard.WizardButtons = WizardButton.Back | WizardButton.Next | WizardButton.Finish;
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		void FillLabelProductsCB() {
			cbLabelProducts.Properties.Items.Clear();
			for(int i = 0; i < LabelProducts.Rows.Count; i++)
				cbLabelProducts.Properties.Items.Add(LabelProducts.Rows[i]["Name"]);
			if(cbLabelProducts.Properties.Items.Count > 0) {
				cbLabelProducts.SelectedIndex = 0;
				FillProductNumberCB();
			}
		}
		void FillProductNumberCB() {			
			ClearProductNumbers();
			int labelProductID = Convert.ToInt32(LabelProducts.Rows[cbLabelProducts.SelectedIndex]["LabelProductID"]);
			if(labelProductID <= 0) return;
			ProductNumberList pnList = new ProductNumberList();
			for(int i = 0; i < LabelDetails.Rows.Count; i++) {
				int labelProductID_i = Convert.ToInt32(LabelDetails.Rows[i]["LabelProductID"]);
				if(labelProductID_i == labelProductID)
					pnList.Add((string)LabelDetails.Rows[i]["Name"], i);
			}
			pnList.Sort();
			for(int i = 0; i < pnList.Count; i++) {
				cbProductNumber.Properties.Items.Add(pnList[i].Name);
				productNumberIndexList.Add(pnList[i].Index);
			}
			if(cbProductNumber.Properties.Items.Count > 0) {
				cbProductNumber.SelectedIndex = 0;
				InitializeLabelInfo();
			}
		}
		void ClearProductNumbers() {
			cbProductNumber.Properties.Items.Clear();
			productNumberIndexList.Clear();
		}
		void InitializeLabelInfo() {
			DataRow row = LabelDetails.Rows[(int)productNumberIndexList[cbProductNumber.SelectedIndex]];
			labelInfo.LabelWidth = Convert.ToSingle(row["Width"]);
			labelInfo.LabelHeight = Convert.ToSingle(row["Height"]);
			labelInfo.HPitch = Convert.ToSingle(row["HPitch"]);
			labelInfo.VPitch = Convert.ToSingle(row["VPitch"]);
			labelInfo.TopMargin = Convert.ToInt32(row["TopMargin"]);
			labelInfo.LeftMargin = Convert.ToInt32(row["SideMargin"]);
			PaperKindList.CurrentID = Convert.ToInt32(row["PaperKind"]);
			UpdatePreview();
			preview.Invalidate();
		}
		void UpdatePreview() {
			panel1.Invalidate();
			lblWidth.Text = XRConvert.Convert(labelInfo.LabelWidth, GraphicsDpi.HundredthsOfAnInch, runner.Report.Dpi).ToString();
			lblHeight.Text = XRConvert.Convert(labelInfo.LabelHeight, GraphicsDpi.HundredthsOfAnInch, runner.Report.Dpi).ToString();
			lblPageType.Text = PaperKindList.PaperName;
			lblPageSize.Text = PaperKindList.PaperSizeText;
		}
		#region Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageLabelType));
			this.panel1 = new System.Windows.Forms.Panel();
			this.preview = new DevExpress.XtraReports.Design.PreviewControl();
			this.lblLabelProducts = new System.Windows.Forms.Label();
			this.cbLabelProducts = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblProductNumber = new System.Windows.Forms.Label();
			this.cbProductNumber = new DevExpress.XtraEditors.ComboBoxEdit();
			this.grpLabelInfo = new DevExpress.XtraEditors.GroupControl();
			this.lblPageTypeStatic = new System.Windows.Forms.Label();
			this.lblPageType = new System.Windows.Forms.Label();
			this.lblWidth = new System.Windows.Forms.Label();
			this.lblHeight = new System.Windows.Forms.Label();
			this.lblPageSize = new System.Windows.Forms.Label();
			this.lblPageSizeStatic = new System.Windows.Forms.Label();
			this.lblHeightStatic = new System.Windows.Forms.Label();
			this.lblWidthStatic = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbLabelProducts.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbProductNumber.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpLabelInfo)).BeginInit();
			this.grpLabelInfo.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.titleLabel, "titleLabel");
			resources.ApplyResources(this.subtitleLabel, "subtitleLabel");
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.preview);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			this.preview.BackColor = System.Drawing.SystemColors.Window;
			resources.ApplyResources(this.preview, "preview");
			this.preview.Name = "preview";
			resources.ApplyResources(this.lblLabelProducts, "lblLabelProducts");
			this.lblLabelProducts.Name = "lblLabelProducts";
			resources.ApplyResources(this.cbLabelProducts, "cbLabelProducts");
			this.cbLabelProducts.Name = "cbLabelProducts";
			this.cbLabelProducts.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbLabelProducts.Properties.Buttons"))))});
			this.cbLabelProducts.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbLabelProducts.EditValueChanged += new System.EventHandler(this.cbLabelProducts_EditValueChanged);
			resources.ApplyResources(this.lblProductNumber, "lblProductNumber");
			this.lblProductNumber.Name = "lblProductNumber";
			resources.ApplyResources(this.cbProductNumber, "cbProductNumber");
			this.cbProductNumber.Name = "cbProductNumber";
			this.cbProductNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbProductNumber.Properties.Buttons"))))});
			this.cbProductNumber.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbProductNumber.EditValueChanged += new System.EventHandler(this.cbProductNumber_EditValueChanged);
			resources.ApplyResources(this.grpLabelInfo, "grpLabelInfo");
			this.grpLabelInfo.Controls.Add(this.lblPageTypeStatic);
			this.grpLabelInfo.Controls.Add(this.lblPageType);
			this.grpLabelInfo.Controls.Add(this.lblWidth);
			this.grpLabelInfo.Controls.Add(this.lblHeight);
			this.grpLabelInfo.Controls.Add(this.lblPageSize);
			this.grpLabelInfo.Controls.Add(this.lblPageSizeStatic);
			this.grpLabelInfo.Controls.Add(this.lblHeightStatic);
			this.grpLabelInfo.Controls.Add(this.lblWidthStatic);
			this.grpLabelInfo.Name = "grpLabelInfo";
			resources.ApplyResources(this.lblPageTypeStatic, "lblPageTypeStatic");
			this.lblPageTypeStatic.Name = "lblPageTypeStatic";
			resources.ApplyResources(this.lblPageType, "lblPageType");
			this.lblPageType.Name = "lblPageType";
			resources.ApplyResources(this.lblWidth, "lblWidth");
			this.lblWidth.Name = "lblWidth";
			resources.ApplyResources(this.lblHeight, "lblHeight");
			this.lblHeight.Name = "lblHeight";
			resources.ApplyResources(this.lblPageSize, "lblPageSize");
			this.lblPageSize.Name = "lblPageSize";
			resources.ApplyResources(this.lblPageSizeStatic, "lblPageSizeStatic");
			this.lblPageSizeStatic.Name = "lblPageSizeStatic";
			resources.ApplyResources(this.lblHeightStatic, "lblHeightStatic");
			this.lblHeightStatic.Name = "lblHeightStatic";
			resources.ApplyResources(this.lblWidthStatic, "lblWidthStatic");
			this.lblWidthStatic.Name = "lblWidthStatic";
			this.Controls.Add(this.grpLabelInfo);
			this.Controls.Add(this.cbProductNumber);
			this.Controls.Add(this.lblProductNumber);
			this.Controls.Add(this.cbLabelProducts);
			this.Controls.Add(this.lblLabelProducts);
			this.Controls.Add(this.panel1);
			this.Name = "WizPageLabelType";
			this.Controls.SetChildIndex(this.panel1, 0);
			this.Controls.SetChildIndex(this.lblLabelProducts, 0);
			this.Controls.SetChildIndex(this.cbLabelProducts, 0);
			this.Controls.SetChildIndex(this.lblProductNumber, 0);
			this.Controls.SetChildIndex(this.cbProductNumber, 0);
			this.Controls.SetChildIndex(this.grpLabelInfo, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbLabelProducts.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbProductNumber.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpLabelInfo)).EndInit();
			this.grpLabelInfo.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected override string OnWizardBack() {
			return "WizPageWelcome";
		}
		void ApplyChanges() {
			LabelReportWizard wizard = (LabelReportWizard)runner.Wizard;
			wizard.LabelInfo.Assign(this.labelInfo);
		}
		protected override string OnWizardNext() {
			ApplyChanges();
			return WizardForm.NextPage;
		}
		protected override bool OnWizardFinish() {
			ApplyChanges();
			return true;
		}
		private void cbLabelProducts_EditValueChanged(object sender, System.EventArgs e) {
			FillProductNumberCB();
		}
		private void cbProductNumber_EditValueChanged(object sender, System.EventArgs e) {
			InitializeLabelInfo();
		}
	}
	public class ProductNumber : IComparable{
		string name;
		int index;
		public string Name { get { return name; } }
		public int Index { get { return index; } }
		public ProductNumber(string name, int index) {
			this.name = name;
			this.index = index;
		}
		int IComparable.CompareTo(object obj) {
			return name.CompareTo(((ProductNumber)obj).Name);
		}
	}
	public class ProductNumberList : ArrayList {
		public new ProductNumber this[int index] { get { return (ProductNumber)base[index]; } }
		public int Add(string name, int index) {
			return base.Add(new ProductNumber(name, index));
		}
	}
}
