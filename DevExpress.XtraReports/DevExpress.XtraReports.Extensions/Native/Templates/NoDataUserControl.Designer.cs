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
	partial class NoDataUserControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.gcNoData = new DevExpress.XtraEditors.GroupControl();
			this.lbNoItemsFound = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.gcNoData)).BeginInit();
			this.gcNoData.SuspendLayout();
			this.SuspendLayout();
			this.gcNoData.Appearance.BackColor = System.Drawing.Color.White;
			this.gcNoData.Appearance.Options.UseBackColor = true;
			this.gcNoData.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gcNoData.Controls.Add(this.lbNoItemsFound);
			this.gcNoData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gcNoData.Location = new System.Drawing.Point(0, 0);
			this.gcNoData.Name = "gcNoData";
			this.gcNoData.ShowCaption = false;
			this.gcNoData.Size = new System.Drawing.Size(292, 498);
			this.gcNoData.TabIndex = 20;
			this.gcNoData.Text = "groupControl3";
			this.lbNoItemsFound.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.lbNoItemsFound.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.lbNoItemsFound.Location = new System.Drawing.Point(3, 4);
			this.lbNoItemsFound.Name = "lbNoItemsFound";
			this.lbNoItemsFound.Size = new System.Drawing.Size(286, 0);
			this.lbNoItemsFound.TabIndex = 0;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gcNoData);
			this.Name = "NoDataUserControl";
			this.Size = new System.Drawing.Size(292, 498);
			((System.ComponentModel.ISupportInitialize)(this.gcNoData)).EndInit();
			this.gcNoData.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl gcNoData;
		public DevExpress.XtraEditors.LabelControl lbNoItemsFound;
	}
}
