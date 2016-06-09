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

namespace DevExpress.DataAccess.UI.Wizard.Views {
	partial class PasswordRequestForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
			this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
			this.editFileName = new DevExpress.XtraEditors.ButtonEdit();
			this.ScrollableJustification = new DevExpress.XtraEditors.XtraScrollableControl();
			this.labelJustification = new DevExpress.XtraEditors.LabelControl();
			this.buttonOk = new DevExpress.XtraEditors.SimpleButton();
			this.checkSavePassword = new DevExpress.XtraEditors.CheckEdit();
			this.textPassword = new DevExpress.XtraEditors.TextEdit();
			this.layoutGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemPassword = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemSavePassword = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemOk = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlScrollableJustification = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlFileName = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemCancel = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceButtons = new DevExpress.XtraLayout.EmptySpaceItem();
			this.openDialog = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
			this.layoutControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.editFileName.Properties)).BeginInit();
			this.ScrollableJustification.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.checkSavePassword.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.textPassword.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPassword)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSavePassword)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemOk)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlScrollableJustification)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlFileName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceButtons)).BeginInit();
			this.SuspendLayout();
			this.layoutControl.AllowCustomization = false;
			this.layoutControl.Controls.Add(this.buttonCancel);
			this.layoutControl.Controls.Add(this.editFileName);
			this.layoutControl.Controls.Add(this.ScrollableJustification);
			this.layoutControl.Controls.Add(this.buttonOk);
			this.layoutControl.Controls.Add(this.checkSavePassword);
			this.layoutControl.Controls.Add(this.textPassword);
			this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl.Location = new System.Drawing.Point(0, 0);
			this.layoutControl.Name = "layoutControl";
			this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(159, 394, 1064, 727);
			this.layoutControl.Root = this.layoutGroup;
			this.layoutControl.Size = new System.Drawing.Size(401, 161);
			this.layoutControl.TabIndex = 0;
			this.layoutControl.Text = "layoutControl1";
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(293, 127);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(96, 22);
			this.buttonCancel.StyleController = this.layoutControl;
			this.buttonCancel.TabIndex = 11;
			this.buttonCancel.Text = "Cancel";
			this.editFileName.Location = new System.Drawing.Point(65, 56);
			this.editFileName.Name = "editFileName";
			this.editFileName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.editFileName.Size = new System.Drawing.Size(324, 20);
			this.editFileName.StyleController = this.layoutControl;
			this.editFileName.TabIndex = 10;
			this.editFileName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.editFileName_ButtonClick);
			this.ScrollableJustification.Controls.Add(this.labelJustification);
			this.ScrollableJustification.Location = new System.Drawing.Point(12, 12);
			this.ScrollableJustification.Name = "ScrollableJustification";
			this.ScrollableJustification.Size = new System.Drawing.Size(377, 40);
			this.ScrollableJustification.TabIndex = 9;
			this.labelJustification.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.labelJustification.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelJustification.Location = new System.Drawing.Point(0, 0);
			this.labelJustification.Name = "labelJustification";
			this.labelJustification.Size = new System.Drawing.Size(377, 0);
			this.labelJustification.TabIndex = 0;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(193, 127);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(96, 22);
			this.buttonOk.StyleController = this.layoutControl;
			this.buttonOk.TabIndex = 7;
			this.buttonOk.Text = "&OK";
			this.checkSavePassword.Location = new System.Drawing.Point(12, 104);
			this.checkSavePassword.Name = "checkSavePassword";
			this.checkSavePassword.Properties.Caption = "&Save password";
			this.checkSavePassword.Size = new System.Drawing.Size(377, 19);
			this.checkSavePassword.StyleController = this.layoutControl;
			this.checkSavePassword.TabIndex = 5;
			this.textPassword.Location = new System.Drawing.Point(65, 80);
			this.textPassword.Name = "textPassword";
			this.textPassword.Properties.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(324, 20);
			this.textPassword.StyleController = this.layoutControl;
			this.textPassword.TabIndex = 4;
			this.layoutGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroup.GroupBordersVisible = false;
			this.layoutGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemPassword,
			this.layoutItemSavePassword,
			this.layoutItemOk,
			this.layoutControlScrollableJustification,
			this.layoutControlFileName,
			this.layoutItemCancel,
			this.emptySpaceButtons});
			this.layoutGroup.Location = new System.Drawing.Point(0, 0);
			this.layoutGroup.Name = "Root";
			this.layoutGroup.Size = new System.Drawing.Size(401, 161);
			this.layoutGroup.TextVisible = false;
			this.layoutItemPassword.Control = this.textPassword;
			this.layoutItemPassword.Location = new System.Drawing.Point(0, 68);
			this.layoutItemPassword.Name = "layoutItemPassword";
			this.layoutItemPassword.Size = new System.Drawing.Size(381, 24);
			this.layoutItemPassword.Text = "&Password:";
			this.layoutItemPassword.TextSize = new System.Drawing.Size(50, 13);
			this.layoutItemSavePassword.Control = this.checkSavePassword;
			this.layoutItemSavePassword.Location = new System.Drawing.Point(0, 92);
			this.layoutItemSavePassword.Name = "layoutItemSavePassword";
			this.layoutItemSavePassword.Size = new System.Drawing.Size(381, 23);
			this.layoutItemSavePassword.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSavePassword.TextVisible = false;
			this.layoutItemOk.Control = this.buttonOk;
			this.layoutItemOk.ControlAlignment = System.Drawing.ContentAlignment.BottomRight;
			this.layoutItemOk.FillControlToClientArea = false;
			this.layoutItemOk.Location = new System.Drawing.Point(181, 115);
			this.layoutItemOk.MaxSize = new System.Drawing.Size(100, 26);
			this.layoutItemOk.MinSize = new System.Drawing.Size(28, 26);
			this.layoutItemOk.Name = "layoutItemOk";
			this.layoutItemOk.Size = new System.Drawing.Size(100, 26);
			this.layoutItemOk.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemOk.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemOk.TextVisible = false;
			this.layoutItemOk.TrimClientAreaToControl = false;
			this.layoutControlScrollableJustification.Control = this.ScrollableJustification;
			this.layoutControlScrollableJustification.Location = new System.Drawing.Point(0, 0);
			this.layoutControlScrollableJustification.MinSize = new System.Drawing.Size(5, 5);
			this.layoutControlScrollableJustification.Name = "layoutControlScrollableJustification";
			this.layoutControlScrollableJustification.Size = new System.Drawing.Size(381, 44);
			this.layoutControlScrollableJustification.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlScrollableJustification.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlScrollableJustification.TextVisible = false;
			this.layoutControlFileName.Control = this.editFileName;
			this.layoutControlFileName.Location = new System.Drawing.Point(0, 44);
			this.layoutControlFileName.Name = "layoutControlFileName";
			this.layoutControlFileName.Size = new System.Drawing.Size(381, 24);
			this.layoutControlFileName.Text = "&File name:";
			this.layoutControlFileName.TextSize = new System.Drawing.Size(50, 13);
			this.layoutItemCancel.Control = this.buttonCancel;
			this.layoutItemCancel.Location = new System.Drawing.Point(281, 115);
			this.layoutItemCancel.Name = "layoutItemCancel";
			this.layoutItemCancel.Size = new System.Drawing.Size(100, 26);
			this.layoutItemCancel.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemCancel.TextVisible = false;
			this.emptySpaceButtons.AllowHotTrack = false;
			this.emptySpaceButtons.Location = new System.Drawing.Point(0, 115);
			this.emptySpaceButtons.Name = "emptySpaceButtons";
			this.emptySpaceButtons.Size = new System.Drawing.Size(181, 26);
			this.emptySpaceButtons.TextSize = new System.Drawing.Size(0, 0);
			this.openDialog.FileName = "openFileDialog1";
			this.AcceptButton = this.buttonOk;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(401, 161);
			this.Controls.Add(this.layoutControl);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(417, 200);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(417, 199);
			this.Name = "PasswordRequestForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "PasswordRequestForm";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
			this.layoutControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.editFileName.Properties)).EndInit();
			this.ScrollableJustification.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.checkSavePassword.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.textPassword.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPassword)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSavePassword)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemOk)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlScrollableJustification)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlFileName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceButtons)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraLayout.LayoutControl layoutControl;
		private XtraLayout.LayoutControlGroup layoutGroup;
		private XtraEditors.CheckEdit checkSavePassword;
		private XtraEditors.TextEdit textPassword;
		private XtraLayout.LayoutControlItem layoutItemPassword;
		private XtraLayout.LayoutControlItem layoutItemSavePassword;
		private XtraEditors.SimpleButton buttonOk;
		private XtraLayout.LayoutControlItem layoutItemOk;
		private XtraEditors.XtraScrollableControl ScrollableJustification;
		private XtraEditors.LabelControl labelJustification;
		private XtraLayout.LayoutControlItem layoutControlScrollableJustification;
		private XtraEditors.ButtonEdit editFileName;
		private XtraLayout.LayoutControlItem layoutControlFileName;
		private System.Windows.Forms.OpenFileDialog openDialog;
		private XtraEditors.SimpleButton buttonCancel;
		private XtraLayout.LayoutControlItem layoutItemCancel;
		private XtraLayout.EmptySpaceItem emptySpaceButtons;
	}
}
