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

namespace DevExpress.XtraScheduler.Forms {
	partial class RemindersForm {
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (components != null) {
						components.Dispose();
					}
					if (control != null) {
						UnsubscribeControlEvents();
						control = null;
					}
					if (storage != null) {
						UnsubscribeStorageEvents();
						storage = null;
					}
					optionsCustomization = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemindersForm));
			this.btnDismissAll = new DevExpress.XtraEditors.SimpleButton();
			this.btnDismiss = new DevExpress.XtraEditors.SimpleButton();
			this.btnOpenItem = new DevExpress.XtraEditors.SimpleButton();
			this.lblSnooze = new DevExpress.XtraEditors.LabelControl();
			this.cbSnooze = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.btnSnooze = new DevExpress.XtraEditors.SimpleButton();
			this.lbItems = new DevExpress.XtraEditors.ImageListBoxControl();
			this.lblSubject = new DevExpress.XtraEditors.LabelControl();
			this.lblStart = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.cbSnooze.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbItems)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnDismissAll, "btnDismissAll");
			this.btnDismissAll.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnDismissAll.Name = "btnDismissAll";
			this.btnDismissAll.Click += new System.EventHandler(this.btnDismissAll_Click);
			resources.ApplyResources(this.btnDismiss, "btnDismiss");
			this.btnDismiss.Name = "btnDismiss";
			this.btnDismiss.Click += new System.EventHandler(this.btnDismiss_Click);
			resources.ApplyResources(this.btnOpenItem, "btnOpenItem");
			this.btnOpenItem.Name = "btnOpenItem";
			this.btnOpenItem.Click += new System.EventHandler(this.btnOpenItem_Click);
			resources.ApplyResources(this.lblSnooze, "lblSnooze");
			this.lblSnooze.Name = "lblSnooze";
			resources.ApplyResources(this.cbSnooze, "cbSnooze");
			this.cbSnooze.Name = "cbSnooze";
			this.cbSnooze.Properties.AccessibleName = resources.GetString("cbSnooze.Properties.AccessibleName");
			this.cbSnooze.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbSnooze.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbSnooze.Properties.Buttons"))))});
			resources.ApplyResources(this.btnSnooze, "btnSnooze");
			this.btnSnooze.Name = "btnSnooze";
			this.btnSnooze.Click += new System.EventHandler(this.btnSnooze_Click);
			this.lbItems.AccessibleRole = System.Windows.Forms.AccessibleRole.Client;
			resources.ApplyResources(this.lbItems, "lbItems");
			this.lbItems.ItemHeight = 16;
			this.lbItems.Name = "lbItems";
			this.lbItems.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbItems.DoubleClick += new System.EventHandler(this.lbItems_DoubleClick);
			this.lbItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbItems_KeyDown);
			resources.ApplyResources(this.lblSubject, "lblSubject");
			this.lblSubject.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.lblSubject.Appearance.Options.UseFont = true;
			this.lblSubject.Name = "lblSubject";
			resources.ApplyResources(this.lblStart, "lblStart");
			this.lblStart.Name = "lblStart";
			this.AcceptButton = this.btnOpenItem;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnDismissAll;
			this.Controls.Add(this.lblStart);
			this.Controls.Add(this.lblSubject);
			this.Controls.Add(this.lbItems);
			this.Controls.Add(this.btnSnooze);
			this.Controls.Add(this.cbSnooze);
			this.Controls.Add(this.lblSnooze);
			this.Controls.Add(this.btnDismissAll);
			this.Controls.Add(this.btnDismiss);
			this.Controls.Add(this.btnOpenItem);
			this.Name = "RemindersForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.RemindersForm_Closing);
			((System.ComponentModel.ISupportInitialize)(this.cbSnooze.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbItems)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraEditors.SimpleButton btnDismissAll;
		protected DevExpress.XtraEditors.SimpleButton btnDismiss;
		protected DevExpress.XtraEditors.SimpleButton btnOpenItem;
		protected DevExpress.XtraEditors.LabelControl lblSnooze;
		protected DevExpress.XtraEditors.ImageComboBoxEdit cbSnooze;
		protected DevExpress.XtraEditors.SimpleButton btnSnooze;
		protected DevExpress.XtraEditors.ImageListBoxControl lbItems;
		protected DevExpress.XtraEditors.LabelControl lblSubject;
		protected DevExpress.XtraEditors.LabelControl lblStart;
		private System.ComponentModel.Container components = null;
	}
}
