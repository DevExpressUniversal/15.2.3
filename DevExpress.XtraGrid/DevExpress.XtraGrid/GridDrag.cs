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
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Utils.Win;
namespace DevExpress.XtraGrid.Dragging {
	public class DragMaster { 
		[ThreadStatic]
		static DragWindow window = null;
		bool dragInProgress;
		Point lastPosition;
		DragDropEffects effects;
		DragDropEffects lastEffect;
		public DragMaster() {
			this.dragInProgress = false;
			this.lastPosition = Point.Empty;
			this.lastEffect = this.effects = DragDropEffects.None;
		}
		internal static DragWindow DragWindow {
			get {
				if(window == null) window = new DragWindow();
				return window;
			}
		}
		public DragDropEffects LastEffect { get { return lastEffect; } }
		public void StartDrag(Bitmap bmp, Point startPoint, DragDropEffects effects) {
			if(DragWindow.IsDisposed) window = new DragWindow();
			StopDrag();
			dragInProgress = true;
			this.effects = effects;
			DragWindow.ShowDrag(startPoint, bmp);
			DragWindow.MakeTopMost();
			this.LastPosition = startPoint;
			CheckDragCursor(effects);
		}
		protected void CheckDragCursor(DragDropEffects e) {
			Cursor cursor = Cursors.Default;
			switch(e) {
				case DragDropEffects.None : cursor = Cursors.Default; break;
				case DragDropEffects.Move : cursor = Cursors.Default; break;
				case DragDropEffects.Link : cursor = DragController.DragRemoveCursor; break;
			}
			Cursor.Current = cursor;
		}
		protected void StopDrag() {
			this.dragInProgress = false;
			this.lastEffect = this.effects = DragDropEffects.None;
			DragWindow.HideDrag();
		}
		public void DoDrag(Point p, DragDropEffects e) {
			if(!dragInProgress) return;
			this.lastEffect = e;
			CheckDragCursor(e);
			DragWindow.MoveDrag(p);
			LastPosition = p;
		}
		public virtual Point LastPosition {
			get { return lastPosition; }
			set { lastPosition = value; }
		}
		public void CancelDrag() {
			if(!dragInProgress) return;
			StopDrag();
		}
		public void EndDrag() {
			if(!dragInProgress) return;
			StopDrag();
		}
	}
	public class DragWindow : TopFormBase {
		bool   dragging;
		Point  hotSpot;
		public DragWindow() {
			SetStyle(ControlStyles.SupportsTransparentBackColor | DevExpress.Utils.ControlConstants.DoubleBuffer, true);
			this.hotSpot = Point.Empty;
			this.dragging = false;
			this.MinimumSize = Size.Empty;
			this.Visible = false;
			this.Size = Size.Empty;
			this.StartPosition = FormStartPosition.Manual;
			this.Location = new Point(-6000, -6000);
			this.Enabled = false;
			this.TabStop = false;
			this.Opacity = DevExpress.Utils.DragDrop.DragWindow.DefaultOpacity;
			this.BackColor = Color.Transparent;
		}
		public void MakeTopMost() {
			UpdateZOrder(InsertAfterWindow);
		}
		protected override void OnLostCapture() {
			HideDrag();
		}
		protected void InternalMoveBitmap(Point p) {
			p.Offset(-hotSpot.X, -hotSpot.Y);
			this.SuspendLayout();
			this.Location = p;
			this.ResumeLayout();
		}
		public void ShowDrag(Point p, Bitmap bitmap) {
			if(IsDisposed) return;
			if(!IsHandleCreated) CreateHandle();
			BackgroundImage = bitmap;
			if(bitmap == null) {
				HideDrag();
				return;
			}
			else {
				Size = bitmap.Size;
				hotSpot = new Point(bitmap.Size.Width / 2, bitmap.Size.Height / 2);
			}
			dragging = true;
			Visible = true;
			Refresh();
			InternalMoveBitmap(p);
		}
		public bool MoveDrag(Point p) {
			if(!dragging) return false;
			InternalMoveBitmap(p);
			return true;
		}
		public bool HideDrag() {
			if(!dragging) return false;
			Visible = false;
			BackgroundImage = null;
			dragging = false;
			Location = new Point(-6000, -6000);
			return true;
		}
		public Point HotSpot {
			get { return hotSpot; }
			set {
				hotSpot = value;
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
		}
	}
}
