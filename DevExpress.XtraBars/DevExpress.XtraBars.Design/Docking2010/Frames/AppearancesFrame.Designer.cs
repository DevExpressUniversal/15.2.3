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

using System.Windows.Forms;
namespace DevExpress.XtraBars.Design.Frames {
	partial class AppearancesFrame {
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			this.label2 = new System.Windows.Forms.Label();
			this.pcDocumentManager = new ContainerControl();
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).BeginInit();
			this.gcAppearances.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).BeginInit();
			this.gcPreview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.pcDocumentManager.SuspendLayout();
			this.SuspendLayout();
			this.gcPreview.Controls.Add(this.pcDocumentManager);
			this.gcPreview.Location = new System.Drawing.Point(166, 0);
			this.gcPreview.Size = new System.Drawing.Size(302, 312);
			this.lbcAppearances.Location = new System.Drawing.Point(2, 45);
			this.lbcAppearances.Size = new System.Drawing.Size(156, 265);
			this.splMain.Location = new System.Drawing.Point(468, 60);
			this.pgMain.Location = new System.Drawing.Point(474, 60);
			this.pgMain.Size = new System.Drawing.Size(286, 312);
			this.pnlMain.Size = new System.Drawing.Size(468, 312);
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(8, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(84, 20);
			this.label2.TabIndex = 5;
			this.label2.Text = "Paint Style:";
			this.pcDocumentManager.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pcDocumentManager.Location = new System.Drawing.Point(2, 20);
			this.pcDocumentManager.Name = "pcGridView";
			this.pcDocumentManager.TabIndex = 5;
			this.Name = "AppearancesDesigner";
			this.Controls.SetChildIndex(this.lbCaption, 0);
			this.Controls.SetChildIndex(this.pnlControl, 0);
			this.Controls.SetChildIndex(this.horzSplitter, 0);
			this.Controls.SetChildIndex(this.pnlMain, 0);
			this.Controls.SetChildIndex(this.splMain, 0);
			this.Controls.SetChildIndex(this.pgMain, 0);
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).EndInit();
			this.gcAppearances.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).EndInit();
			this.gcPreview.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.pcDocumentManager.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion Component Designer generated code
	}
}
