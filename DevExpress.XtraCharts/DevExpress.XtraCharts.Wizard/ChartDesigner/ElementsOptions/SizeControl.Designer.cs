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

namespace DevExpress.XtraCharts.Designer.Native {
	partial class SizeControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
			this.lWidth = new System.Windows.Forms.Label();
			this.lHeigth = new System.Windows.Forms.Label();
			this.seWidth = new DevExpress.XtraEditors.SpinEdit();
			this.seHeigth = new DevExpress.XtraEditors.SpinEdit();
			((System.ComponentModel.ISupportInitialize)(this.seWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.seHeigth.Properties)).BeginInit();
			this.SuspendLayout();
			this.lWidth.AutoSize = true;
			this.lWidth.Location = new System.Drawing.Point(112, 3);
			this.lWidth.Name = "lWidth";
			this.lWidth.Size = new System.Drawing.Size(35, 13);
			this.lWidth.TabIndex = 0;
			this.lWidth.Text = "Width";
			this.lHeigth.AutoSize = true;
			this.lHeigth.Location = new System.Drawing.Point(112, 28);
			this.lHeigth.Name = "lHeigth";
			this.lHeigth.Size = new System.Drawing.Size(38, 13);
			this.lHeigth.TabIndex = 1;
			this.lHeigth.Text = "Heigth";
			this.seWidth.EditValue = new decimal(new int[] {
			0,
			0,
			0,
			0});
			this.seWidth.Location = new System.Drawing.Point(0, 0);
			this.seWidth.Name = "seWidth";
			this.seWidth.Properties.Appearance.Options.UseTextOptions = true;
			this.seWidth.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.seWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "0", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, false)});
			this.seWidth.Properties.IsFloatValue = false;
			this.seWidth.Properties.Mask.EditMask = "N00";
			this.seWidth.Size = new System.Drawing.Size(106, 20);
			this.seWidth.TabIndex = 15;
			this.seHeigth.EditValue = new decimal(new int[] {
			0,
			0,
			0,
			0});
			this.seHeigth.Location = new System.Drawing.Point(0, 25);
			this.seHeigth.Name = "seHeigth";
			this.seHeigth.Properties.Appearance.Options.UseTextOptions = true;
			this.seHeigth.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.seHeigth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "0", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, "", null, null, false)});
			this.seHeigth.Properties.IsFloatValue = false;
			this.seHeigth.Properties.Mask.EditMask = "N00";
			this.seHeigth.Size = new System.Drawing.Size(106, 20);
			this.seHeigth.TabIndex = 16;
			this.seHeigth.EditValueChanged += new System.EventHandler(this.seHeigth_EditValueChanged);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.seHeigth);
			this.Controls.Add(this.seWidth);
			this.Controls.Add(this.lHeigth);
			this.Controls.Add(this.lWidth);
			this.Name = "SizeControl";
			this.Size = new System.Drawing.Size(153, 48);
			((System.ComponentModel.ISupportInitialize)(this.seWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.seHeigth.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private System.Windows.Forms.Label lWidth;
		private System.Windows.Forms.Label lHeigth;
		private XtraEditors.SpinEdit seWidth;
		private XtraEditors.SpinEdit seHeigth;
	}
}
