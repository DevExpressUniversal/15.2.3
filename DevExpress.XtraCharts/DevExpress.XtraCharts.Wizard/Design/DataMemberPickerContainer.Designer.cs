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

namespace DevExpress.XtraCharts.Design {
	partial class DataMemberPickerContainer {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				this.dataMemberPicker.Stop();
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.dataMemberPicker = new DevExpress.XtraCharts.Design.DataMemberPicker();
			this.SuspendLayout();
			this.dataMemberPicker.AutoSize = true;
			this.dataMemberPicker.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.dataMemberPicker.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataMemberPicker.Location = new System.Drawing.Point(0, 0);
			this.dataMemberPicker.Name = "dataMemberPicker";
			this.dataMemberPicker.Size = new System.Drawing.Size(254, 276);
			this.dataMemberPicker.TabIndex = 1;
			this.dataMemberPicker.TreeViewDoubleClick += new System.EventHandler(this.dataMemberPicker_TreeViewDoubleClick);
			this.Controls.Add(this.dataMemberPicker);
			this.Size = new System.Drawing.Size(254, 276);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DataMemberPicker dataMemberPicker;
	}
}
