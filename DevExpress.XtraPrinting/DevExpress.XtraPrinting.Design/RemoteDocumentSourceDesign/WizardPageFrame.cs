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

using DevExpress.Utils.Frames;
using DevExpress.XtraEditors.Designer.Utils;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign {
	class WizardPageFrame : XtraFrame {
		string description = string.Empty;
		DescriptionPanel pnlDescription;
		public WizardPageFrame() : base() {
			InitializeComponent();
			InitFrame("fakeText");
		}
		internal void AddContent(UserControl content) {
			pnlMain.Controls.Clear();
			pnlMain.Controls.Add(content);
			var pageView = content as IPageView;
			if(pageView != null) {
				description = pageView.DescriptionText;
				this.lbCaption.Text = pageView.HeaderText;
				this.pnlDescription.Text = pageView.DescriptionText;
			}
		}
		private void InitializeComponent() {
			this.pnlDescription = new DevExpress.Utils.Frames.DescriptionPanel();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.pnlMain.Location = new System.Drawing.Point(0, 84);
			this.pnlMain.Margin = new System.Windows.Forms.Padding(0);
			this.pnlMain.Size = new System.Drawing.Size(400, 192);
			this.horzSplitter.Location = new System.Drawing.Point(0, 80);
			this.pnlDescription.BackColor = System.Drawing.Color.White;
			this.pnlDescription.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlDescription.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.pnlDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.pnlDescription.Location = new System.Drawing.Point(0, 38);
			this.pnlDescription.Name = "pnlDescription";
			this.pnlDescription.Size = new System.Drawing.Size(400, 42);
			this.pnlDescription.TabIndex = 0;
			this.pnlDescription.TabStop = false;
			this.Controls.Add(this.pnlDescription);
			this.Name = "WizardPageFrame";
			this.Controls.SetChildIndex(this.lbCaption, 0);
			this.Controls.SetChildIndex(this.pnlDescription, 0);
			this.Controls.SetChildIndex(this.horzSplitter, 0);
			this.Controls.SetChildIndex(this.pnlMain, 0);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
	}
}
