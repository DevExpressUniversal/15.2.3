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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Win;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	public class AccordionDragMaster {
		[ThreadStatic]
		static AccordionDragWindow window = null;
		bool dragInProgress;
		Point lastPosition;
		public AccordionDragMaster() {
			this.dragInProgress = false;
			this.lastPosition = Point.Empty;
		}
		internal static AccordionDragWindow DragWindow {
			get {
				if(window == null) window = new AccordionDragWindow();
				return window;
			}
		}
		public void StartDragging(Bitmap bmp, Point startPoint, AccordionDropTargetArgs dt) {
			if(DragWindow.IsDisposed) {
				window = new AccordionDragWindow();
			}
			StopDrag();
			dragInProgress = true;
			DragWindow.DropTargets = dt;
			DragWindow.ShowDrag(startPoint, bmp);
			DragWindow.MakeTopMost();
			LastPosition = startPoint;
			CheckDragCursor(false);
		}
		protected void CheckDragCursor(bool canDrop) {
			if(DragWindow.DropTargets.CanInsertElement() && canDrop)
				Cursor.Current = Cursors.Default;
			else Cursor.Current = Cursors.No;
		}
		protected void StopDrag() {
			this.dragInProgress = false;
			Cursor.Current = Cursors.Default;
			DragWindow.HideDrag();
		}
		public void DoDrag(Point p, bool canDrop) {
			if(!dragInProgress) return;
			CheckDragCursor(canDrop);
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
		public void EndDragging() {
			if(!dragInProgress) return;
			StopDrag();
		}
	}
	public class AccordionDragWindow : TopFormBase {
		bool dragging;
		Point hotSpot;
		public AccordionDragWindow() {
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
		internal AccordionDropTargetArgs DropTargets { get; set; }
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
	}
	public class AccordionDragController {
		AccordionControl control;
		AccordionDragMaster dragMaster;
		public AccordionDragController(AccordionControl control) {
			this.control = control;
			DropTarget = new AccordionDropTargetArgs();
			this.dragMaster = new AccordionDragMaster();
		}
		public AccordionControl AccordionControl { get { return control; } }
		AccordionDragMaster DragMaster { get { return dragMaster; } }
		public virtual bool StartDragging(AccordionControlHitInfo info) {
			if(!info.IsInElement) {
				DropTarget.ElementInfo = null;
				return false;
			}
			if(IsDesignMode) {
				DropTarget.ElementInfo = info.ItemInfo;
				return true;
			}
			StartAccordionElementDraggingEventArgs args = new StartAccordionElementDraggingEventArgs(info.ItemInfo.Element);
			AccordionControl.RaiseStartElementDragging(args);
			if(args.Cancel)
				return false;
			DropTarget.ElementInfo = info.ItemInfo;
			Bitmap bmp = (Bitmap)args.DragImage;
			if(bmp == null) bmp = AccordionBitmapPainter.Draw(DropTarget.ElementInfo);
			DragMaster.StartDragging(bmp, Cursor.Position, DropTarget);
			return true;
		}
		bool IsDesignMode { get { return AccordionControl.IsDesignMode; } }
		public virtual void DragOver(Point p) {
			object prevOwner = DropTarget.TargetOwner;
			AccordionControlHitInfo info = AccordionControl.ControlInfo.CalcHitInfo(p);
			DropTarget.TargetOwner = GetTargetOwner(info);
			if(!IsDesignMode) {
				AccordionElementDragOverEventArgs args = new AccordionElementDragOverEventArgs(DropTarget.ElementInfo.Element, DropTarget.TargetOwner, DropTarget.CanInsertElement(info.ItemInfo));
				AccordionControl.RaiseElementDragOver(args);
				DropTarget.CanDrop = args.CanDrop;
				DragMaster.DoDrag(AccordionControl.PointToScreen(p), args.CanDrop);
			}
			if(info.ItemInfo == null) return;
			AccordionDropTargetArgs dt = DropTarget;
			if(info.ItemInfo == dt.ElementInfo) {
				GroupExpandTimer.Stop();
				dt.PrevElementInfo = null;
				AccordionControl.Invalidate();
				return;
			}
			else if(dt.PrevElementInfo == info.ItemInfo && dt.TargetOwner == prevOwner)
				return;
			UpdatePrevElementInfo(info);
			if(dt.PrevElementInfo != null) {
				if(dt.PrevElementInfo.Element.Style == ElementStyle.Group)
					GroupExpandTimer.Start();
				else GroupExpandTimer.Stop();
			}
			AccordionControl.Invalidate();
		}
		void UpdatePrevElementInfo(AccordionControlHitInfo info) {
			if(ShouldDropIntoElement(info))
				DropTarget.PrevElementInfo = null;
			else DropTarget.PrevElementInfo = info.ItemInfo;
		}
		bool ShouldDropIntoElement(AccordionControlHitInfo info) {
			if(info.ItemInfo != null && info.ItemInfo.Element.Style == ElementStyle.Group) {
				Rectangle elemBounds = info.ItemInfo.HeaderBounds;
				Rectangle upperPartElementBounds = new Rectangle(elemBounds.X, elemBounds.Y, elemBounds.Width, elemBounds.Height / 2);
				if(upperPartElementBounds.Contains(info.HitPoint)) {
					return true;
				}
			}
			return false;
		}
		public object GetTargetOwner(AccordionControlHitInfo hitInfo) {
			Rectangle contentRect = new Rectangle(Point.Empty, AccordionControl.Size);
			if(contentRect.Contains(hitInfo.HitPoint)) {
				if(hitInfo.ItemInfo == null)
					return AccordionControl;
				if(hitInfo.ItemInfo.Equals(DropTarget.ElementInfo))
					return null;
				if(ShouldDropIntoElement(hitInfo))
					return hitInfo.ItemInfo.Element;
				if(hitInfo.ItemInfo.Element.OwnerElement == null)
					return AccordionControl;
				return hitInfo.ItemInfo.Element.OwnerElement;
			}
			return null;
		}
		protected internal AccordionDropTargetArgs DropTarget { get; set; }
		public virtual void EndDragging(Point p) {
			bool cancel = true;
			AccordionDropTargetArgs dt = DropTarget;
			if(!IsDesignMode){
				AccordionControlHitInfo info = AccordionControl.ControlInfo.CalcHitInfo(p);
				object targetOwner = GetTargetOwner(info);
				int insertIndex = GetInsertIndex(targetOwner);
				EndAccordionElementDraggingEventArgs args = new EndAccordionElementDraggingEventArgs(dt.ElementInfo.Element, GetTargetOwner(info), insertIndex);
				AccordionControl.RaiseEndElementDragging(args);
				cancel = !args.Cancel && dt.CanDrop;
			}
			if(CanDrop(cancel)) {
				AccordionControl.ControlInfo.ClearCache();
				if(dt.PrevElementInfo != null) {
					InsertElement(dt.ElementInfo.Element, dt.PrevElementInfo.Element.OwnerElements);
				}
				else if(dt.TargetOwner != null) {
					if(dt.TargetOwner is AccordionControl)
						InsertElement(dt.ElementInfo.Element, AccordionControl.Elements);
					else {
						AccordionControlElement target = dt.TargetOwner as AccordionControlElement;
						if(target != null) InsertElement(dt.ElementInfo.Element, target.Elements);
					}
				}
			}
			ResetDropTarget();
			AccordionControl.Refresh();
		}
		internal void ResetDropTarget() {
			DropTarget.PrevElementInfo = null;
			DropTarget.TargetOwner = null;
			DropTarget.ElementInfo = null;
			if(!IsDesignMode) {
				DragMaster.EndDragging();
			}
			AccordionControl.ControlInfo.ResetPressedInfo();
			AccordionControl.ControlInfo.ResetHoverInfo();
		}
		void InsertElement(AccordionControlElement elem, AccordionControlElementCollection col) {
			elem.OwnerElements.Remove(elem);
			int index = 0;
			if(DropTarget.PrevElementInfo != null)
				index = DropTarget.PrevElementInfo.Element.OwnerElements.IndexOf(DropTarget.PrevElementInfo.Element) + 1;
			col.Insert(index, elem);
			elem.RaiseDragDrop();
		}
		bool CanDrop(bool canDrop) {
			return DropTarget.ElementInfo != null && DropTarget.CanInsertElement() && canDrop;
		}
		int GetInsertIndex(object targetOwner) {
			if(targetOwner == null || !DropTarget.CanDrop || !DropTarget.CanInsertElement())
				return -1;
			if(DropTarget.PrevElementInfo != null) {
				return DropTarget.PrevElementInfo.Element.OwnerElements.IndexOf(DropTarget.PrevElementInfo.Element) + 1;
			}
			return 0;
		}
		System.Windows.Forms.Timer groupExpandTimer;
		System.Windows.Forms.Timer GroupExpandTimer {
			get {
				if(groupExpandTimer == null)
					groupExpandTimer = CreateGroupExpandTimer();
				return groupExpandTimer;
			}
		}
		protected System.Windows.Forms.Timer CreateGroupExpandTimer() {
			System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
			timer.Interval = 2500;
			timer.Tick += OnGroupExpandTimerTick;
			return timer;
		}
		void OnGroupExpandTimerTick(object sender, EventArgs e) {
			AccordionDropTargetArgs dt = DropTarget;
			if(dt.PrevElementInfo != null) {
				if(dt.CanInsertElement() && dt.PrevElementInfo.Element.Style == ElementStyle.Group) {
					dt.PrevElementInfo.Element.InvertExpanded();
					AccordionControl.Invalidate();
				}
			}
			GroupExpandTimer.Stop();
		}
	}
	static class AccordionBitmapPainter {
		public static Bitmap Draw(AccordionElementBaseViewInfo info) {
			return ElementBitmapPainter.Draw(cache => { Paint(cache, info); }, info.HeaderBounds.Size, info.HeaderBounds.Location);
		}
		static void Paint(GraphicsCache cache, AccordionElementBaseViewInfo info) {
			AccordionControlPainter painter = (AccordionControlPainter)info.ControlInfo.Painter;
			cache.Graphics.Clear(info.ControlInfo.GetBackColor());
			painter.DrawHeaderCore(cache, info);
		}
	}
	public static class ElementBitmapPainter {
		[SecuritySafeCritical]
		public static Bitmap Draw(Action<GraphicsCache> paint, Size size, Point offset) {
			Bitmap bmp = new Bitmap(size.Width, size.Height);
			IntPtr dib;
			var memoryHdc = CreateMemoryHdc(IntPtr.Zero, bmp.Width, bmp.Height, out dib);
			try {
				using(Graphics memoryGraphics = Graphics.FromHdc(memoryHdc)) {
					memoryGraphics.TranslateTransform(-offset.X, -offset.Y);
					using(GraphicsCache cache = new GraphicsCache(memoryGraphics)) {
						paint(cache);
					}
				}  
				using(Graphics imageGraphics = Graphics.FromImage(bmp)) {
					var imgHdc = imageGraphics.GetHdc();
					BitBlt(imgHdc, 0, 0, bmp.Width, bmp.Height, memoryHdc, 0, 0, 0x00CC0020);
					imageGraphics.ReleaseHdc(imgHdc);
				}
			}
			finally {   
				DeleteObject(dib);
				DeleteDC(memoryHdc);
			}
			return bmp;
		}
		[SecuritySafeCritical]
		static IntPtr CreateMemoryHdc(IntPtr hdc, int width, int height, out IntPtr dib) {
			IntPtr memoryHdc = CreateCompatibleDC(hdc);
			SetBkMode(memoryHdc, 1);
			var info = new BitMapInfo();
			info.biSize = Marshal.SizeOf(info);
			info.biWidth = width;
			info.biHeight = -height;
			info.biPlanes = 1;
			info.biBitCount = 32;
			info.biCompression = 0;	
			IntPtr ppvBits;
			dib = CreateDIBSection(hdc, ref info, 0, out ppvBits, IntPtr.Zero, 0);
			SelectObject(memoryHdc, dib);
			return memoryHdc;
		}
		[DllImport("gdi32.dll")]
		public static extern int SetBkMode(IntPtr hdc, int mode);
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BitMapInfo pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
		[DllImport("gdi32.dll")]
		public static extern int SelectObject(IntPtr hdc, IntPtr hgdiObj);
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern bool DeleteDC(IntPtr hdc);
		[StructLayout(LayoutKind.Sequential)]
		internal struct BitMapInfo {
			public int biSize;
			public int biWidth;
			public int biHeight;
			public short biPlanes;
			public short biBitCount;
			public int biCompression;
			public int biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public int biClrUsed;
			public int biClrImportant;
			public byte bmiColors_rgbBlue;
			public byte bmiColors_rgbGreen;
			public byte bmiColors_rgbRed;
			public byte bmiColors_rgbReserved;
		}
	}
}
