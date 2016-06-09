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
	partial class BookmarkForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BookmarkForm));
			this.lblBookmarkName = new DevExpress.XtraEditors.LabelControl();
			this.rgSortBy = new DevExpress.XtraEditors.RadioGroup();
			this.lblSortBy = new DevExpress.XtraEditors.LabelControl();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btnGoTo = new DevExpress.XtraEditors.SimpleButton();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.lblSplit = new DevExpress.XtraEditors.LabelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.edtBookmarkName = new DevExpress.XtraEditors.TextEdit();
			this.lbBookmarkName = new DevExpress.XtraEditors.ListBoxControl();
			((System.ComponentModel.ISupportInitialize)(this.rgSortBy.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtBookmarkName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbBookmarkName)).BeginInit();
			this.SuspendLayout();
			this.lblBookmarkName.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblBookmarkName, "lblBookmarkName");
			this.lblBookmarkName.Name = "lblBookmarkName";
			resources.ApplyResources(this.rgSortBy, "rgSortBy");
			this.rgSortBy.Name = "rgSortBy";
			this.rgSortBy.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgSortBy.Properties.Appearance.BackColor")));
			this.rgSortBy.Properties.Appearance.Options.UseBackColor = true;
			this.rgSortBy.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgSortBy.Properties.Columns = 2;
			this.rgSortBy.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgSortBy.Properties.Items")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgSortBy.Properties.Items1"))});
			this.rgSortBy.SelectedIndexChanged += new System.EventHandler(this.OnSortBySelectedIndexChanged);
			this.lblSortBy.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblSortBy, "lblSortBy");
			this.lblSortBy.Name = "lblSortBy";
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Click += new System.EventHandler(this.OnDeleteClick);
			resources.ApplyResources(this.btnGoTo, "btnGoTo");
			this.btnGoTo.Name = "btnGoTo";
			this.btnGoTo.Click += new System.EventHandler(this.OnGoToClick);
			this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.OnAddClick);
			resources.ApplyResources(this.lblSplit, "lblSplit");
			this.lblSplit.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lblSplit.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblSplit.LineVisible = true;
			this.lblSplit.Name = "lblSplit";
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.edtBookmarkName, "edtBookmarkName");
			this.edtBookmarkName.Name = "edtBookmarkName";
			this.edtBookmarkName.LocationChanged += new System.EventHandler(this.edtBookmarkName_LocationChanged);
			this.edtBookmarkName.SizeChanged += new System.EventHandler(this.edtBookmarkName_SizeChanged);
			resources.ApplyResources(this.lbBookmarkName, "lbBookmarkName");
			this.lbBookmarkName.Name = "lbBookmarkName";
			this.lbBookmarkName.TabStop = false;
			this.lbBookmarkName.SizeChanged += new System.EventHandler(this.lbBookmarkName_SizeChanged);
			this.AcceptButton = this.btnAdd;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.lbBookmarkName);
			this.Controls.Add(this.edtBookmarkName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lblSplit);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnGoTo);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.lblSortBy);
			this.Controls.Add(this.rgSortBy);
			this.Controls.Add(this.lblBookmarkName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BookmarkForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.rgSortBy.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtBookmarkName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbBookmarkName)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl lblBookmarkName;
		protected DevExpress.XtraEditors.RadioGroup rgSortBy;
		protected DevExpress.XtraEditors.LabelControl lblSortBy;
		protected DevExpress.XtraEditors.SimpleButton btnDelete;
		protected DevExpress.XtraEditors.SimpleButton btnGoTo;
		protected DevExpress.XtraEditors.SimpleButton btnAdd;
		protected DevExpress.XtraEditors.LabelControl lblSplit;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.TextEdit edtBookmarkName;
		protected DevExpress.XtraEditors.ListBoxControl lbBookmarkName;
	}
}
