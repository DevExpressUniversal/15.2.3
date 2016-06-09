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

namespace DevExpress.XtraRichEdit.Forms.Design {
	partial class BorderShadingTypeLineUserControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BorderShadingTypeLineUserControl));
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.colorEdit1 = new DevExpress.XtraEditors.ColorEdit();
			this.borderLineWeightEdit1 = new DevExpress.XtraRichEdit.Forms.Design.BorderLineWeightEdit();
			this.borderLineStyleListBox1 = new DevExpress.XtraRichEdit.Forms.Design.BorderLineStyleListBox();
			((System.ComponentModel.ISupportInitialize)(this.colorEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.borderLineWeightEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.borderLineStyleListBox1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.labelControl3, "labelControl3");
			this.labelControl3.Name = "labelControl3";
			resources.ApplyResources(this.colorEdit1, "colorEdit1");
			this.colorEdit1.Name = "colorEdit1";
			this.colorEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("colorEdit1.Properties.Buttons"))))});
			this.colorEdit1.EditValueChanged += new System.EventHandler(this.colorEdit1_EditValueChanged);
			this.borderLineWeightEdit1.Control = null;
			resources.ApplyResources(this.borderLineWeightEdit1, "borderLineWeightEdit1");
			this.borderLineWeightEdit1.Name = "borderLineWeightEdit1";
			this.borderLineWeightEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("borderLineWeightEdit1.Properties.Buttons"))))});
			this.borderLineWeightEdit1.Properties.Control = null;
			this.borderLineWeightEdit1.EditValueChanged += new System.EventHandler(this.borderLineWeightEdit1_EditValueChanged);
			this.borderLineStyleListBox1.DocumentModel = null;
			resources.ApplyResources(this.borderLineStyleListBox1, "borderLineStyleListBox1");
			this.borderLineStyleListBox1.Name = "borderLineStyleListBox1";
			this.borderLineStyleListBox1.SelectedValueChanged += new System.EventHandler(this.borderLineStyleListBox1_SelectedValueChanged);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.borderLineWeightEdit1);
			this.Controls.Add(this.borderLineStyleListBox1);
			this.Controls.Add(this.colorEdit1);
			this.Controls.Add(this.labelControl3);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.labelControl1);
			this.Name = "BorderShadingTypeLineUserControl";
			((System.ComponentModel.ISupportInitialize)(this.colorEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.borderLineWeightEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.borderLineStyleListBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.LabelControl labelControl2;
		private XtraEditors.LabelControl labelControl3;
		private XtraEditors.ColorEdit colorEdit1;
		private BorderLineStyleListBox borderLineStyleListBox1;
		private BorderLineWeightEdit borderLineWeightEdit1;
	}
}
