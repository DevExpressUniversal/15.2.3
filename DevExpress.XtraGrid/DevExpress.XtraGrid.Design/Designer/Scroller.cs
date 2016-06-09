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
using System.Data;
using System.Windows.Forms;
namespace DevExpress.XtraGrid.Design {
	[ToolboxItem(false)]
	public class Scroller : System.Windows.Forms.Panel {
		private System.ComponentModel.Container components = null;
		ViewTree control;
		public Scroller()
		{
			this.control = null;
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ContainerControl | ControlStyles.ResizeRedraw, true);
			InitializeComponent();
		}
		public ViewTree Control {
			get { return control; }
			set {
				if(Control == value) return;
				if(Control != null) {
					Control.Resize -= new EventHandler(OnControl_Resize);
				}
				control = value;
				if(Control != null) {
					Control.Resize += new EventHandler(OnControl_Resize);
					if(!Controls.Contains(Control)) Controls.Add(Control);
					Control.Location = Point.Empty;
					OnControl_Resize(this, EventArgs.Empty);
				}
			}
		}
		bool internalResizing = false;
		public bool IsInternalSizing { get { return internalResizing; } }
		protected void OnControl_Resize(object sender, EventArgs e) {
			UpdateClientControlSize();
		}
		public virtual void UpdateClientControlSize() {
			if(this.internalResizing) return;
			if(Control == null) return;
			Size size = Control.CalcBestSize(), clientSize = ClientSize;
			if(size.Height < clientSize.Height || size.Width < clientSize.Width) {
				this.internalResizing = true;
				try {
					Control.Size = new Size(Math.Max(size.Width, clientSize.Width), Math.Max(size.Height, clientSize.Height));
					AdjustFormScrollbars(true);
				}
				finally {
					this.internalResizing = false;
				}
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
		}
		protected override void OnResize(EventArgs e) {
			OnControl_Resize(this, EventArgs.Empty);
			base.OnResize(e);
			OnControl_Resize(this, EventArgs.Empty);
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				Control = null;
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.AutoScroll = true;
			this.Name = "Scroller";
			this.Size = new System.Drawing.Size(376, 336);
		}
		#endregion
	}
}
