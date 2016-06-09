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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design {
	[ToolboxItem(false)]
	public class WizPageLayoutBase : DevExpress.Utils.InteriorWizardPage {
		private System.Windows.Forms.PictureBox picPreview;
		protected GroupControl grpLayout;
		private GroupControl grpOrientation;
		private System.Windows.Forms.PictureBox picOrientation;
		private DevExpress.XtraEditors.CheckEdit chkAdjustFields;
		private System.ComponentModel.IContainer components = null;
		ImageList orientationImageList = new ImageList();
		protected ImageList layoutImageList = new ImageList();
		private DevExpress.XtraEditors.Internal.RadioGroupLocalizable rgrpOrientation;
		protected StandardReportWizard fWizard;
		protected CheckEdit[] layoutEdits;
		protected Dictionary<CheckEdit, ReportLayout> layoutEditValues;
		public WizPageLayoutBase() {
			InitializeComponent();
			orientationImageList.ImageSize = new Size(32, 32);
			orientationImageList.TransparentColor = Color.Magenta;
			ResourceImageHelper.FillImageListFromResources(orientationImageList, "Images.Orientation.bmp", typeof(LocalResFinder));
			picOrientation.Image = orientationImageList.Images[0];
			layoutEditValues = new System.Collections.Generic.Dictionary<XtraEditors.CheckEdit, ReportLayout>();
		}
		public WizPageLayoutBase(XRWizardRunnerBase runner)
			: this() {
			this.fWizard = (StandardReportWizard)runner.Wizard;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageLayoutBase));
			this.picPreview = new System.Windows.Forms.PictureBox();
			this.grpLayout = new DevExpress.XtraEditors.GroupControl();
			this.grpOrientation = new DevExpress.XtraEditors.GroupControl();
			this.picOrientation = new System.Windows.Forms.PictureBox();
			this.rgrpOrientation = new DevExpress.XtraEditors.Internal.RadioGroupLocalizable();
			this.chkAdjustFields = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpLayout)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpOrientation)).BeginInit();
			this.grpOrientation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picOrientation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rgrpOrientation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAdjustFields.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.titleLabel, "titleLabel");
			resources.ApplyResources(this.subtitleLabel, "subtitleLabel");
			this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			resources.ApplyResources(this.picPreview, "picPreview");
			this.picPreview.Name = "picPreview";
			this.picPreview.TabStop = false;
			resources.ApplyResources(this.grpLayout, "grpLayout");
			this.grpLayout.Name = "grpLayout";
			this.grpOrientation.Controls.Add(this.picOrientation);
			this.grpOrientation.Controls.Add(this.rgrpOrientation);
			resources.ApplyResources(this.grpOrientation, "grpOrientation");
			this.grpOrientation.Name = "grpOrientation";
			resources.ApplyResources(this.picOrientation, "picOrientation");
			this.picOrientation.Name = "picOrientation";
			this.picOrientation.TabStop = false;
			resources.ApplyResources(this.rgrpOrientation, "rgrpOrientation");
			this.rgrpOrientation.Name = "rgrpOrientation";
			this.rgrpOrientation.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.rgrpOrientation.Properties.Appearance.Options.UseBackColor = true;
			this.rgrpOrientation.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgrpOrientation.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(0, "&Portrait"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(1, "&Landscape")});
			this.rgrpOrientation.Properties.EditValueChanged += new System.EventHandler(this.rgrpOrientation_Properties_EditValueChanged);
			resources.ApplyResources(this.chkAdjustFields, "chkAdjustFields");
			this.chkAdjustFields.Name = "chkAdjustFields";
			this.chkAdjustFields.Properties.Appearance.Options.UseTextOptions = true;
			this.chkAdjustFields.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chkAdjustFields.Properties.AutoHeight = ((bool)(resources.GetObject("chkAdjustFields.Properties.AutoHeight")));
			this.chkAdjustFields.Properties.Caption = resources.GetString("chkAdjustFields.Properties.Caption");
			this.Controls.Add(this.chkAdjustFields);
			this.Controls.Add(this.grpOrientation);
			this.Controls.Add(this.grpLayout);
			this.Controls.Add(this.picPreview);
			this.Name = "WizPageLayoutBase";
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			this.Controls.SetChildIndex(this.picPreview, 0);
			this.Controls.SetChildIndex(this.grpLayout, 0);
			this.Controls.SetChildIndex(this.grpOrientation, 0);
			this.Controls.SetChildIndex(this.chkAdjustFields, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpLayout)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpOrientation)).EndInit();
			this.grpOrientation.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picOrientation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rgrpOrientation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAdjustFields.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected void UpdateLayoutImage() {
			int index = GetCheckedLayoutEditIndex();
			if(index < 0)
				return;
			if(index + 1 > layoutImageList.Images.Count)
				return;
			picPreview.Image = layoutImageList.Images[index];
		}
		void UpdateOrientationImage() {
			int index = Convert.ToInt32(rgrpOrientation.EditValue);
			if(index < 0)
				return;
			if(index + 1 > orientationImageList.Images.Count)
				return;
			picOrientation.Image = orientationImageList.Images[index];
		}
		protected override bool OnSetActive() {
			rgrpOrientation.EditValue = fWizard.Orientation == PageOrientation.Portrait ? 0 : 1;
			chkAdjustFields.Checked = fWizard.FitFieldsToPage;
			layoutEdits[0].Checked = true;
			ReportLayout val = fWizard.Layout;
			if(val == ReportLayout.Default)
				val = layoutEditValues[layoutEdits[0]];
			FindLayoutEditByValue(val).Checked = true;
			UpdateLayoutImage();
			return true;
		}
		protected override void UpdateWizardButtons() {
			Wizard.WizardButtons = WizardButton.Back | WizardButton.Next | WizardButton.Finish;
		}
		protected void ApplyChanges() {
			fWizard.Layout = layoutEditValues[GetCheckedLayoutEdit()];
			int index = Convert.ToInt32(rgrpOrientation.EditValue);
			if(index == 0)
				fWizard.Orientation = PageOrientation.Portrait;
			else
				fWizard.Orientation = PageOrientation.Landscape;
			fWizard.FitFieldsToPage = chkAdjustFields.Checked;
		}
		protected void RollbackChanges() {
			fWizard.Orientation = PageOrientation.Portrait;
			fWizard.Layout = ReportLayout.Default;
			fWizard.FitFieldsToPage = true;
		}
		protected override bool OnWizardFinish() {
			ApplyChanges();
			return true;
		}
		private void rgrpOrientation_Properties_EditValueChanged(object sender, EventArgs e) {
			UpdateOrientationImage();
		}
		int GetCheckedLayoutEditIndex() {
			for(int index = 0; index < layoutEdits.Length; index++)
				if(layoutEdits[index].Checked)
					return index;
			return -1;
		}
		CheckEdit GetCheckedLayoutEdit() {
			int index = GetCheckedLayoutEditIndex();
			if(index == -1)
				return null;
			else
				return layoutEdits[index];
		}
		CheckEdit FindLayoutEditByValue(ReportLayout reportLayout) {
			foreach(CheckEdit edit in layoutEdits) {
				if(layoutEditValues[edit] == reportLayout)
					return edit;
			}
			return null;
		}
	}
}
