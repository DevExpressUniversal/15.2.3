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

namespace DevExpress.XtraReports.Native.Templates {
	partial class ProgressUserControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressUserControl));
			this.gcProgress = new DevExpress.XtraEditors.GroupControl();
			this.marqueeProgressBarControl1 = new DevExpress.XtraEditors.MarqueeProgressBarControl();
			this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.gcProgress)).BeginInit();
			this.gcProgress.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.marqueeProgressBarControl1.Properties)).BeginInit();
			this.SuspendLayout();
			this.gcProgress.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("gcProgress.Appearance.BackColor")));
			this.gcProgress.Appearance.Options.UseBackColor = true;
			this.gcProgress.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gcProgress.Controls.Add(this.marqueeProgressBarControl1);
			this.gcProgress.Controls.Add(this.labelControl6);
			resources.ApplyResources(this.gcProgress, "gcProgress");
			this.gcProgress.Name = "gcProgress";
			this.gcProgress.ShowCaption = false;
			resources.ApplyResources(this.marqueeProgressBarControl1, "marqueeProgressBarControl1");
			this.marqueeProgressBarControl1.Name = "marqueeProgressBarControl1";
			resources.ApplyResources(this.labelControl6, "labelControl6");
			this.labelControl6.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.labelControl6.Name = "labelControl6";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gcProgress);
			this.Name = "ProgressUserControl";
			((System.ComponentModel.ISupportInitialize)(this.gcProgress)).EndInit();
			this.gcProgress.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.marqueeProgressBarControl1.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl gcProgress;
		private DevExpress.XtraEditors.MarqueeProgressBarControl marqueeProgressBarControl1;
		private DevExpress.XtraEditors.LabelControl labelControl6;
	}
}
