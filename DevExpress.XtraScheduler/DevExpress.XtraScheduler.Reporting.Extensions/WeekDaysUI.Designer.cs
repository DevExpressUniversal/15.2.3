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

namespace DevExpress.XtraScheduler.Reporting.Design {
	partial class WeekDaysUI {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.chkSun = new System.Windows.Forms.CheckBox();
			this.chkMo = new System.Windows.Forms.CheckBox();
			this.chkTue = new System.Windows.Forms.CheckBox();
			this.chWed = new System.Windows.Forms.CheckBox();
			this.chkThu = new System.Windows.Forms.CheckBox();
			this.chkFri = new System.Windows.Forms.CheckBox();
			this.chkSat = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			this.chkSun.AutoSize = true;
			this.chkSun.Location = new System.Drawing.Point(9, 9);
			this.chkSun.Name = "chkSun";
			this.chkSun.Size = new System.Drawing.Size(45, 17);
			this.chkSun.TabIndex = 0;
			this.chkSun.Tag = 1;
			this.chkSun.Text = "Sun";
			this.chkSun.UseVisualStyleBackColor = true;
			this.chkMo.AutoSize = true;
			this.chkMo.Location = new System.Drawing.Point(9, 32);
			this.chkMo.Name = "chkMo";
			this.chkMo.Size = new System.Drawing.Size(41, 17);
			this.chkMo.TabIndex = 1;
			this.chkMo.Tag = 2;
			this.chkMo.Text = "Mo";
			this.chkMo.UseVisualStyleBackColor = true;
			this.chkTue.AutoSize = true;
			this.chkTue.Location = new System.Drawing.Point(9, 55);
			this.chkTue.Name = "chkTue";
			this.chkTue.Size = new System.Drawing.Size(45, 17);
			this.chkTue.TabIndex = 2;
			this.chkTue.Tag = 4;
			this.chkTue.Text = "Tue";
			this.chkTue.UseVisualStyleBackColor = true;
			this.chWed.AutoSize = true;
			this.chWed.Location = new System.Drawing.Point(9, 78);
			this.chWed.Name = "chWed";
			this.chWed.Size = new System.Drawing.Size(49, 17);
			this.chWed.TabIndex = 3;
			this.chWed.Tag = 8;
			this.chWed.Text = "Wed";
			this.chWed.UseVisualStyleBackColor = true;
			this.chkThu.AutoSize = true;
			this.chkThu.Location = new System.Drawing.Point(9, 101);
			this.chkThu.Name = "chkThu";
			this.chkThu.Size = new System.Drawing.Size(45, 17);
			this.chkThu.TabIndex = 4;
			this.chkThu.Tag = 16;
			this.chkThu.Text = "Thu";
			this.chkThu.UseVisualStyleBackColor = true;
			this.chkFri.AutoSize = true;
			this.chkFri.Location = new System.Drawing.Point(9, 124);
			this.chkFri.Name = "chkFri";
			this.chkFri.Size = new System.Drawing.Size(37, 17);
			this.chkFri.TabIndex = 5;
			this.chkFri.Tag = 32;
			this.chkFri.Text = "Fri";
			this.chkFri.UseVisualStyleBackColor = true;
			this.chkSat.AutoSize = true;
			this.chkSat.Location = new System.Drawing.Point(9, 147);
			this.chkSat.Name = "chkSat";
			this.chkSat.Size = new System.Drawing.Size(42, 17);
			this.chkSat.TabIndex = 6;
			this.chkSat.Tag = 64;
			this.chkSat.Text = "Sat";
			this.chkSat.UseVisualStyleBackColor = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.chkSun);
			this.Controls.Add(this.chkSat);
			this.Controls.Add(this.chkFri);
			this.Controls.Add(this.chkThu);
			this.Controls.Add(this.chWed);
			this.Controls.Add(this.chkTue);
			this.Controls.Add(this.chkMo);
			this.MaximumSize = new System.Drawing.Size(68, 173);
			this.Name = "WeekDaysUI";
			this.Size = new System.Drawing.Size(62, 167);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private System.Windows.Forms.CheckBox chkSun;
		private System.Windows.Forms.CheckBox chkMo;
		private System.Windows.Forms.CheckBox chkTue;
		private System.Windows.Forms.CheckBox chWed;
		private System.Windows.Forms.CheckBox chkThu;
		private System.Windows.Forms.CheckBox chkFri;
		private System.Windows.Forms.CheckBox chkSat;
	}
}
