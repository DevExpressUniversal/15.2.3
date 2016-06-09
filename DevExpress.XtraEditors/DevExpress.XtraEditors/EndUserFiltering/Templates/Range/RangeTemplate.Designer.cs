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

namespace DevExpress.XtraEditors.Filtering.Templates.Range {
	partial class RangeTemplate {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			DevExpress.XtraEditors.Repository.TrackBarLabel trackBarLabel1 = new DevExpress.XtraEditors.Repository.TrackBarLabel();
			DevExpress.XtraEditors.Repository.TrackBarLabel trackBarLabel2 = new DevExpress.XtraEditors.Repository.TrackBarLabel();
			DevExpress.XtraEditors.Repository.TrackBarLabel trackBarLabel3 = new DevExpress.XtraEditors.Repository.TrackBarLabel();
			this.Part_TrackBar = new DevExpress.XtraEditors.RangeTrackBarControl();
			((System.ComponentModel.ISupportInitialize)(this.Part_TrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_TrackBar.Properties)).BeginInit();
			this.SuspendLayout();
			this.Part_TrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Part_TrackBar.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
			this.Part_TrackBar.Location = new System.Drawing.Point(9, 5);
			this.Part_TrackBar.Name = "Part_TrackBar";
			this.Part_TrackBar.Properties.EditValueChangedFiringMode = XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
			this.Part_TrackBar.Properties.EditValueChangedDelay = 250;
			this.Part_TrackBar.Properties.LabelAppearance.Options.UseTextOptions = true;
			this.Part_TrackBar.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			trackBarLabel1.Label = "From";
			trackBarLabel2.Label = "To";
			trackBarLabel2.Value = 100;
			trackBarLabel3.Label = "Avg";
			trackBarLabel3.Value = 50;
			this.Part_TrackBar.Properties.Labels.AddRange(new DevExpress.XtraEditors.Repository.TrackBarLabel[] {
			trackBarLabel1,
			trackBarLabel2,
			trackBarLabel3});
			this.Part_TrackBar.Properties.Maximum = 100;
			this.Part_TrackBar.Properties.ShowLabels = true;
			this.Part_TrackBar.Properties.ShowValueToolTip = true;
			this.Part_TrackBar.Properties.TickFrequency = 10;
			this.Part_TrackBar.Properties.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.Part_TrackBar.Size = new System.Drawing.Size(222, 54);
			this.Part_TrackBar.TabIndex = 1;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.Part_TrackBar);
			this.Name = "RangeTemplate";
			this.Padding = new System.Windows.Forms.Padding(9, 5, 9, 1);
			this.Size = new System.Drawing.Size(240, 60);
			((System.ComponentModel.ISupportInitialize)(this.Part_TrackBar.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_TrackBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private RangeTrackBarControl Part_TrackBar;
	}
}
