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

namespace DevExpress.XtraRichEdit.Forms {
	partial class BulletedListForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BulletedListForm));
			this.bulletCharacterControl = new DevExpress.XtraRichEdit.Design.BulletCharacterControl();
			this.lblBulletCharacter = new DevExpress.XtraEditors.LabelControl();
			this.lblBulletPosition = new DevExpress.XtraEditors.LabelControl();
			this.btnCharacter = new DevExpress.XtraEditors.SimpleButton();
			this.lblTextPosition = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.edtAligned.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtIndent.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			resources.ApplyResources(this.btnCancel, "btnCancel");
			resources.ApplyResources(this.btnFont, "btnFont");
			resources.ApplyResources(this.edtAligned, "edtAligned");
			resources.ApplyResources(this.edtIndent, "edtIndent");
			resources.ApplyResources(this.lblAlignedAt, "lblAlignedAt");
			resources.ApplyResources(this.lblIndentAt, "lblIndentAt");
			resources.ApplyResources(this.bulletCharacterControl, "bulletCharacterControl");
			this.bulletCharacterControl.Name = "bulletCharacterControl";
			this.bulletCharacterControl.SelectedIndex = 0;
			this.lblBulletCharacter.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblBulletCharacter, "lblBulletCharacter");
			this.lblBulletCharacter.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lblBulletCharacter.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblBulletCharacter.LineVisible = true;
			this.lblBulletCharacter.Name = "lblBulletCharacter";
			this.lblBulletPosition.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblBulletPosition, "lblBulletPosition");
			this.lblBulletPosition.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lblBulletPosition.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblBulletPosition.LineVisible = true;
			this.lblBulletPosition.Name = "lblBulletPosition";
			resources.ApplyResources(this.btnCharacter, "btnCharacter");
			this.btnCharacter.Name = "btnCharacter";
			this.btnCharacter.Click += new System.EventHandler(this.OnCharacterClick);
			this.lblTextPosition.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblTextPosition, "lblTextPosition");
			this.lblTextPosition.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lblTextPosition.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblTextPosition.LineVisible = true;
			this.lblTextPosition.Name = "lblTextPosition";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.bulletCharacterControl);
			this.Controls.Add(this.lblBulletCharacter);
			this.Controls.Add(this.btnCharacter);
			this.Controls.Add(this.lblTextPosition);
			this.Controls.Add(this.lblBulletPosition);
			this.Name = "BulletedListForm";
			this.Controls.SetChildIndex(this.lblIndentAt, 0);
			this.Controls.SetChildIndex(this.edtIndent, 0);
			this.Controls.SetChildIndex(this.lblBulletPosition, 0);
			this.Controls.SetChildIndex(this.lblAlignedAt, 0);
			this.Controls.SetChildIndex(this.lblTextPosition, 0);
			this.Controls.SetChildIndex(this.btnCharacter, 0);
			this.Controls.SetChildIndex(this.btnOk, 0);
			this.Controls.SetChildIndex(this.btnCancel, 0);
			this.Controls.SetChildIndex(this.lblBulletCharacter, 0);
			this.Controls.SetChildIndex(this.bulletCharacterControl, 0);
			this.Controls.SetChildIndex(this.btnFont, 0);
			this.Controls.SetChildIndex(this.edtAligned, 0);
			((System.ComponentModel.ISupportInitialize)(this.edtAligned.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtIndent.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl lblBulletCharacter;
		protected DevExpress.XtraEditors.SimpleButton btnCharacter;
		protected DevExpress.XtraEditors.LabelControl lblBulletPosition;
		protected DevExpress.XtraRichEdit.Design.BulletCharacterControl bulletCharacterControl;
		protected DevExpress.XtraEditors.LabelControl lblTextPosition;
	}
}
