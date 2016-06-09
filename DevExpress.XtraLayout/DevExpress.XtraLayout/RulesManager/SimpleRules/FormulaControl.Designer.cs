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

namespace DevExpress.XtraEditors.Frames {
	partial class FormulaControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.lcFormula = new DevExpress.XtraLayout.LayoutControl();
			this.lcgFormula = new DevExpress.XtraLayout.LayoutControlGroup();
			((System.ComponentModel.ISupportInitialize)(this.lcFormula)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgFormula)).BeginInit();
			this.SuspendLayout();
			this.lcFormula.AllowCustomization = false;
			this.lcFormula.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lcFormula.Location = new System.Drawing.Point(0, 0);
			this.lcFormula.Name = "lcFormula";
			this.lcFormula.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(800, 214, 847, 619);
			this.lcFormula.Root = this.lcgFormula;
			this.lcFormula.Size = new System.Drawing.Size(470, 265);
			this.lcFormula.TabIndex = 0;
			this.lcFormula.Text = "layoutControl1";
			this.lcgFormula.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgFormula.GroupBordersVisible = false;
			this.lcgFormula.Location = new System.Drawing.Point(0, 0);
			this.lcgFormula.Name = "Root";
			this.lcgFormula.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 6, 0, 0);
			this.lcgFormula.Size = new System.Drawing.Size(470, 265);
			this.lcgFormula.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lcFormula);
			this.Name = "FormulaControl";
			this.Size = new System.Drawing.Size(470, 265);
			((System.ComponentModel.ISupportInitialize)(this.lcFormula)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgFormula)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraLayout.LayoutControl lcFormula;
		private DevExpress.XtraLayout.LayoutControlGroup lcgFormula;
	}
}
