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

namespace DevExpress.XtraVerticalGrid {
	using System;
	using System.Windows.Forms;
	using System.Collections;
	using System.Drawing;
	using DevExpress.Utils.Win;
	public enum RowDragEffect {None, InsertBefore, MoveChild, MoveToEnd, InsertAfter }
	public class DragMaster {
		[ThreadStatic]
		static DragWindow window;
		internal static DragWindow DragWindow {
			get {
				if(window == null) window = new DragWindow();
				return window;
			}
		}
		bool dragInProgress;
		Hashtable cursors;
		RowDragEffect effects;
		RowDragEffect lastEffect;
		public DragMaster() {
			dragInProgress = false;
			lastEffect = effects = RowDragEffect.None;
			cursors = new Hashtable();
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			cursors[RowDragEffect.None] = Cursors.No;
			cursors[RowDragEffect.InsertBefore] = new Cursor(asm.GetManifestResourceStream("DevExpress.XtraVerticalGrid.Cursors.insBefore.cur"));
			cursors[RowDragEffect.MoveChild] = new Cursor(asm.GetManifestResourceStream("DevExpress.XtraVerticalGrid.Cursors.movChild.cur"));
			cursors[RowDragEffect.MoveToEnd] = new Cursor(asm.GetManifestResourceStream("DevExpress.XtraVerticalGrid.Cursors.movToEnd.cur"));
			cursors[RowDragEffect.InsertAfter] = new Cursor(asm.GetManifestResourceStream("DevExpress.XtraVerticalGrid.Cursors.insAfter.cur"));
		}
		public void StartDrag(Bitmap bmp, Point startPoint, RowDragEffect effects) {
			StopDrag();
			dragInProgress = true;
			this.effects = effects;
			lastEffect = effects;
			DragWindow.ShowDrag(startPoint, bmp);
			DragWindow.MakeTopMost();
			SetDragCursor(effects);
		}
		void SetDragCursor(RowDragEffect e) {
			Cursor.Current = (Cursor)cursors[e];
		}
		protected void StopDrag() {
			dragInProgress = false;
			lastEffect = effects = RowDragEffect.None;
			DragWindow.HideDrag();
		}
		public void DoDrag(Point p, RowDragEffect e, bool setCursor) {
			if(!dragInProgress) return;
			lastEffect = e;
			if(setCursor) SetDragCursor(e);
			DragWindow.MoveDrag(p);
		}
		public void EndDrag() {
			if(!dragInProgress) return;
			StopDrag();
		}
	}
	public class DragWindow : TopFormBase {
		private Bitmap dragBitmap;
		private bool   dragging;
		private Point  hotSpot;
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
			this.Size = BackgroundImage.Size;
			this.ResumeLayout();
		}
		public bool ShowDrag(Point p, Bitmap bmp) {
			if(bmp == null) return false;
			if(!IsHandleCreated) CreateHandle();
			dragging = true;
			DragBitmap = bmp;
			Visible = true;
			Refresh();
			InternalMoveBitmap(p);
			return true;
		}
		public bool MoveDrag(Point p) {
			if(!dragging) return false;
			InternalMoveBitmap(p);
			return true;
		}
		public bool HideDrag() {
			if(!dragging) return false;
			BackgroundImage = null;
			Visible = false;
			dragging = false;
			this.SuspendLayout();
			this.Size = Size.Empty;
			this.Location = new Point(-100, -100);
			this.ResumeLayout();
			return true;
		}
		public Point HotSpot {
			get { return hotSpot; }
			set {
				hotSpot = value;
			}
		}
		public Bitmap DragBitmap { 
			get { return dragBitmap; }
			set {
				this.BackgroundImage = value;
				if(value == null) {
					HideDrag();
				}
				else
					hotSpot = new Point(value.Size.Width / 2, value.Size.Height / 2);
				dragBitmap = value;
			}
		}
	}
}
