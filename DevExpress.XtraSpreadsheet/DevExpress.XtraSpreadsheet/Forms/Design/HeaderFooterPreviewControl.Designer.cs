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

namespace DevExpress.XtraSpreadsheet.Forms.Design {
	partial class HeaderFooterPreviewControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeaderFooterPreviewControl));
			this.edtLeftHeaderFooterPreview = new DevExpress.XtraEditors.MemoEdit();
			this.edtCenterHeaderFooterPreview = new DevExpress.XtraEditors.MemoEdit();
			this.edtRightHeaderFooterPreview = new DevExpress.XtraEditors.MemoEdit();
			((System.ComponentModel.ISupportInitialize)(this.edtLeftHeaderFooterPreview.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtCenterHeaderFooterPreview.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRightHeaderFooterPreview.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.edtLeftHeaderFooterPreview, "edtLeftHeaderFooterPreview");
			this.edtLeftHeaderFooterPreview.Name = "edtLeftHeaderFooterPreview";
			this.edtLeftHeaderFooterPreview.Properties.AccessibleName = resources.GetString("edtLeftHeaderFooterPreview.Properties.AccessibleName");
			this.edtLeftHeaderFooterPreview.Properties.Appearance.Options.UseTextOptions = true;
			this.edtLeftHeaderFooterPreview.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.edtLeftHeaderFooterPreview.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.edtLeftHeaderFooterPreview.Properties.ReadOnly = true;
			this.edtLeftHeaderFooterPreview.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			resources.ApplyResources(this.edtCenterHeaderFooterPreview, "edtCenterHeaderFooterPreview");
			this.edtCenterHeaderFooterPreview.Name = "edtCenterHeaderFooterPreview";
			this.edtCenterHeaderFooterPreview.Properties.AccessibleName = resources.GetString("edtCenterHeaderFooterPreview.Properties.AccessibleName");
			this.edtCenterHeaderFooterPreview.Properties.Appearance.Options.UseTextOptions = true;
			this.edtCenterHeaderFooterPreview.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.edtCenterHeaderFooterPreview.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.edtCenterHeaderFooterPreview.Properties.ReadOnly = true;
			this.edtCenterHeaderFooterPreview.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			resources.ApplyResources(this.edtRightHeaderFooterPreview, "edtRightHeaderFooterPreview");
			this.edtRightHeaderFooterPreview.Name = "edtRightHeaderFooterPreview";
			this.edtRightHeaderFooterPreview.Properties.AccessibleName = resources.GetString("edtRightHeaderFooterPreview.Properties.AccessibleName");
			this.edtRightHeaderFooterPreview.Properties.Appearance.Options.UseTextOptions = true;
			this.edtRightHeaderFooterPreview.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.edtRightHeaderFooterPreview.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.edtRightHeaderFooterPreview.Properties.ReadOnly = true;
			this.edtRightHeaderFooterPreview.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.edtRightHeaderFooterPreview);
			this.Controls.Add(this.edtCenterHeaderFooterPreview);
			this.Controls.Add(this.edtLeftHeaderFooterPreview);
			this.Name = "HeaderFooterPreviewControl";
			((System.ComponentModel.ISupportInitialize)(this.edtLeftHeaderFooterPreview.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtCenterHeaderFooterPreview.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRightHeaderFooterPreview.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.MemoEdit edtLeftHeaderFooterPreview;
		private XtraEditors.MemoEdit edtCenterHeaderFooterPreview;
		private XtraEditors.MemoEdit edtRightHeaderFooterPreview;
	}
}
