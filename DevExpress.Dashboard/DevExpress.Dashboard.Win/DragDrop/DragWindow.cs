#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.Utils.Internal;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.DashboardWin.DragDrop {
	public class DragWindow : TopFormBase {
		bool isDragging;
		Size offset;
		LayeredDragWindow layeredWindow;
		public bool IsDragging { get { return isDragging; } }
		public Point DragLocation { get { return Location + offset; } }
		public bool DragWindowVisible { get { return layeredWindow != null ? layeredWindow.IsVisible : false; } }
		public Size DragWindowSize { get { return layeredWindow != null ? layeredWindow.Size : Size.Empty; } }
		public Image DragWindowImage { get { return layeredWindow != null ? layeredWindow.Image : null; } }
		public Size Offset { get { return offset; } }
		public event EventHandler DragStarted;
		public event EventHandler<DragCompleteEventArgs> DragEnded;
		public DragWindow() {
			StartPosition = FormStartPosition.Manual;
			TabStop = false;
			Enabled = false;
			Visible = false;
		}
		public void StartDrag(Point startPosition, Bitmap dragBitmap, Size offset) {
			if(!IsDisposed) {
				if(!IsHandleCreated)
					CreateHandle();
				layeredWindow = new LayeredDragWindow(dragBitmap, this);
				Location = new Point(-10000, -10000);
				isDragging = true;
				Capture = true;
				this.offset = offset;
				Location = startPosition - offset;
				layeredWindow.Show(Location);
				if(DragStarted != null)
					DragStarted(this, EventArgs.Empty);
			}
		}
		public void StopDrag(bool isCancelled) {
			if(isDragging) {
				isDragging = false;
				Visible = false;
				Capture = false;
				layeredWindow.Hide();
				if(DragEnded != null)
					DragEnded(this, new DragCompleteEventArgs(isCancelled));
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				StopDrag(true);
			base.Dispose(disposing);
		}
		protected override void OnLostCapture() {
			base.OnLostCapture();
			StopDrag(true);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button == MouseButtons.Left)
				StopDrag(false);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Location += new Size(e.X - offset.Width, e.Y - offset.Height);
			layeredWindow.Show(Location);
		}
	}
	public class LayeredDragWindow : DXLayeredWindowEx {
		Image image;
		Control parent;
		public Image Image { get { return image; } }
		public LayeredDragWindow(Image image, Control parent) {
			base.Alpha = 191;
			base.Size = image.Size;
			this.parent = parent;
			this.image = image;
		}
		protected override void DrawCore(GraphicsCache cache) {
			if(image != null) {
				cache.Graphics.Clear(Color.Transparent);
				cache.Graphics.DrawImage(image, Point.Empty);
			}
		}
		public new void Show(Point location) {
			base.Create(parent);
			base.Show(location);
			Update();
		}
	}
	public class DragCompleteEventArgs : EventArgs {
		readonly bool isDragCancelled;
		public bool IsDragCancelled { get { return isDragCancelled; } }
		public DragCompleteEventArgs(bool isDragCancelled) {
			this.isDragCancelled = isDragCancelled;
		}
	}
}
