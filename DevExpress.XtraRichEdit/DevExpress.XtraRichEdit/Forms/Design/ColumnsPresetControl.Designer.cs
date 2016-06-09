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
	partial class ColumnsPresetControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.label = new DevExpress.XtraEditors.LabelControl();
			this.checkBox = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.checkBox.Properties)).BeginInit();
			this.SuspendLayout();
			this.label.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.label.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.label.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label.Location = new System.Drawing.Point(0, 152);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(213, 13);
			this.label.TabIndex = 3;
			this.checkBox.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.checkBox.Location = new System.Drawing.Point(0, 0);
			this.checkBox.Name = "checkBox";
			this.checkBox.Properties.Appearance.BackColor = System.Drawing.Color.White;
			this.checkBox.Properties.Appearance.Options.UseBackColor = true;
			this.checkBox.Properties.AutoHeight = false;
			this.checkBox.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.checkBox.Properties.Caption = "";
			this.checkBox.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
			this.checkBox.Properties.FullFocusRect = true;
			this.checkBox.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.checkBox.Size = new System.Drawing.Size(0, 0);
			this.checkBox.TabIndex = 4;
			this.checkBox.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			this.checkBox.Paint += new System.Windows.Forms.PaintEventHandler(this.checkBox_Paint);
			this.Controls.Add(this.checkBox);
			this.Controls.Add(this.label);
			this.Name = "ColumnsPresetControl";
			this.Size = new System.Drawing.Size(213, 165);
			((System.ComponentModel.ISupportInitialize)(this.checkBox.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.LabelControl label;
		private DevExpress.XtraEditors.CheckEdit checkBox;
	}
}
