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

namespace DevExpress.XtraScheduler.Design {
	partial class CustomAppointmentFormWizardForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomAppointmentFormWizardForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.lblDescription = new System.Windows.Forms.Label();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.cbAppointmentForm = new DevExpress.XtraEditors.ComboBoxEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciPictureBox = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciAppointmentForm = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciDescription = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciOk = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciCancel = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbAppointmentForm.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAppointmentForm)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDescription)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOk)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(292, 190);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(88, 22);
			this.btnCancel.StyleController = this.layoutControl1;
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "Cancel";
			this.layoutControl1.Controls.Add(this.lblDescription);
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.btnOk);
			this.layoutControl1.Controls.Add(this.pictureBox1);
			this.layoutControl1.Controls.Add(this.cbAppointmentForm);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(700, 183, 730, 350);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(482, 224);
			this.layoutControl1.TabIndex = 5;
			this.layoutControl1.Text = "layoutControl1";
			this.lblDescription.Location = new System.Drawing.Point(222, 36);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(248, 150);
			this.lblDescription.TabIndex = 4;
			this.lblDescription.Text = resources.GetString("lblDescription.Text");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(384, 190);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(86, 22);
			this.btnOk.StyleController = this.layoutControl1;
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "OK";
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(206, 200);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			this.cbAppointmentForm.Location = new System.Drawing.Point(262, 12);
			this.cbAppointmentForm.Name = "cbAppointmentForm";
			this.cbAppointmentForm.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbAppointmentForm.Size = new System.Drawing.Size(208, 20);
			this.cbAppointmentForm.StyleController = this.layoutControl1;
			this.cbAppointmentForm.TabIndex = 1;
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciPictureBox,
			this.lciAppointmentForm,
			this.lciDescription,
			this.lciOk,
			this.lciCancel,
			this.emptySpaceItem1});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(482, 224);
			this.layoutControlGroup1.TextVisible = false;
			this.lciPictureBox.Control = this.pictureBox1;
			this.lciPictureBox.Location = new System.Drawing.Point(0, 0);
			this.lciPictureBox.MaxSize = new System.Drawing.Size(210, 204);
			this.lciPictureBox.MinSize = new System.Drawing.Size(210, 204);
			this.lciPictureBox.Name = "lciPictureBox";
			this.lciPictureBox.Size = new System.Drawing.Size(210, 204);
			this.lciPictureBox.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.lciPictureBox.TextSize = new System.Drawing.Size(0, 0);
			this.lciPictureBox.TextVisible = false;
			this.lciAppointmentForm.Control = this.cbAppointmentForm;
			this.lciAppointmentForm.Location = new System.Drawing.Point(210, 0);
			this.lciAppointmentForm.Name = "lciAppointmentForm";
			this.lciAppointmentForm.Size = new System.Drawing.Size(252, 24);
			this.lciAppointmentForm.Text = "Layout:";
			this.lciAppointmentForm.TextSize = new System.Drawing.Size(37, 13);
			this.lciDescription.Control = this.lblDescription;
			this.lciDescription.Location = new System.Drawing.Point(210, 24);
			this.lciDescription.Name = "lciDescription";
			this.lciDescription.Size = new System.Drawing.Size(252, 154);
			this.lciDescription.TextSize = new System.Drawing.Size(0, 0);
			this.lciDescription.TextVisible = false;
			this.lciOk.Control = this.btnOk;
			this.lciOk.Location = new System.Drawing.Point(372, 178);
			this.lciOk.MaxSize = new System.Drawing.Size(90, 0);
			this.lciOk.MinSize = new System.Drawing.Size(90, 26);
			this.lciOk.Name = "lciOk";
			this.lciOk.Size = new System.Drawing.Size(90, 26);
			this.lciOk.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.lciOk.TextSize = new System.Drawing.Size(0, 0);
			this.lciOk.TextVisible = false;
			this.lciCancel.Control = this.btnCancel;
			this.lciCancel.Location = new System.Drawing.Point(280, 178);
			this.lciCancel.MaxSize = new System.Drawing.Size(92, 0);
			this.lciCancel.MinSize = new System.Drawing.Size(92, 26);
			this.lciCancel.Name = "lciCancel";
			this.lciCancel.Size = new System.Drawing.Size(92, 26);
			this.lciCancel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.lciCancel.TextSize = new System.Drawing.Size(0, 0);
			this.lciCancel.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(210, 178);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(70, 26);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(482, 224);
			this.Controls.Add(this.layoutControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "CustomAppointmentFormWizardForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Customize Appointment Form";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbAppointmentForm.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAppointmentForm)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDescription)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOk)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.ComboBoxEdit cbAppointmentForm;
		private XtraEditors.SimpleButton btnOk;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label lblDescription;
		private XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.LayoutControlItem lciPictureBox;
		private XtraLayout.LayoutControlItem lciAppointmentForm;
		private XtraLayout.LayoutControlItem lciDescription;
		private XtraLayout.LayoutControlItem lciOk;
		private XtraLayout.LayoutControlItem lciCancel;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
	}
}
