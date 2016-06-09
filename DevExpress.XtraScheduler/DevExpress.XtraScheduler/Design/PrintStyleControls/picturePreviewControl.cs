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

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraScheduler.Design.PrintStyleControls {
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class PicturePreviewControl : DevExpress.XtraEditors.XtraUserControl {
		#region Fields
		IContainer components = null;
		int paperWidth;
		int paperHeight;
		int pageWidth;
		int pageHeight;
		Brush whiteBrush = new SolidBrush(Color.White);
		Brush lightGrayBrush = new SolidBrush(Color.Gray);
		Pen blackPen = new Pen(Color.Black);
		#endregion
		public PicturePreviewControl() {
			InitializeComponent();
		}
		#region Properties
		public int PaperWidth { get { return paperWidth; } set { paperWidth = value; } }
		public int PaperHeight { get { return paperHeight; } set { paperHeight = value; } }
		public int PageWidth { get { return pageWidth; } set { pageWidth = value; } }
		public int PageHeight { get { return pageHeight; } set { pageHeight = value; } }
		#endregion
		#region Dispose
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
					components = null;
				}
				if(whiteBrush != null) {
					whiteBrush.Dispose();
					whiteBrush = null;
				}
				if(lightGrayBrush != null) {
					lightGrayBrush.Dispose();
					lightGrayBrush = null;
				}
				if(blackPen != null) {
					blackPen.Dispose();
					blackPen = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion
		#region Designer generated code
		private void InitializeComponent() {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PicturePreviewControl));
			this.AccessibleDescription = ((string)(resources.GetObject("$this.AccessibleDescription")));
			this.AccessibleName = ((string)(resources.GetObject("$this.AccessibleName")));
			this.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("$this.Anchor")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.Name = "PicturePreviewControl";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
			this.TabIndex = ((int)(resources.GetObject("$this.TabIndex")));
			this.Visible = ((bool)(resources.GetObject("$this.Visible")));
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
		}
		#endregion
		void OnPaint(object sender, System.Windows.Forms.PaintEventArgs e) {
			if(PaperWidth == 0 || PaperHeight == 0 || PageWidth == 0 || PageHeight == 0)
				return;
			int maxSize = Math.Max(paperWidth, paperHeight);
			int previewPaperWidth = Width * paperWidth / maxSize - 1;
			int previewPaperHeight = Height * paperHeight / maxSize - 1;
			Rectangle paperRect = new Rectangle(0, 0, previewPaperWidth, previewPaperHeight);
			e.Graphics.FillRectangle(whiteBrush, paperRect);
			e.Graphics.DrawRectangle(blackPen, paperRect);
			int xCount = PaperWidth / PageWidth;
			int yCount = PaperHeight / PageHeight;
			int previewPageWidth = previewPaperWidth * pageWidth / paperWidth;
			int previewPageHeight = previewPaperHeight * pageHeight / paperHeight;
			for(int i = 0; i < xCount; i++) {
				for(int j = 0; j < yCount; j++) {
					Rectangle pageRect = new Rectangle(i * previewPageWidth, j * previewPageHeight, previewPageWidth, previewPageHeight);
					pageRect.Inflate(-4, -4);
					e.Graphics.FillRectangle(lightGrayBrush, pageRect);
					e.Graphics.DrawRectangle(blackPen, pageRect);
				}
			}
		}
	}
}
