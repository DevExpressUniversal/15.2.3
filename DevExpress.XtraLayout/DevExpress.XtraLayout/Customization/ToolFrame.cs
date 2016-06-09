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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using System.Drawing;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Customization.Controls;
namespace DevExpress.XtraLayout.Customization {
	public interface IToolFrame {
		Rectangle Bounds { get; set; }
		bool Visible { get; set; }
		Point PointToScreen(Point p);
	}
	public interface IToolFrameOwner {
		Point PointToClient(Point p);
	}
	public class ControlToolFrameInfo : IToolFrame, IDisposable {
		Control control;
		public ControlToolFrameInfo(Control control) {
			this.control = control;
		}
		public Rectangle Bounds { get { return control.Bounds; } set { } }
		public bool Visible { get { return control.Visible; } set { } }
		public Point PointToScreen(Point p) { return control.PointToScreen(p); }
		public void Dispose() {
			control = null;
		}
	}
	public class BaseToolFrame : BaseDragHelperForm, IToolFrame {
		public BaseToolFrame() : base(0, 0) { }
		IToolFrameOwner frameOwner;
		public IToolFrameOwner FrameOwner {
			get { return frameOwner; }
			set { frameOwner = value; }
		}
		public MouseEventArgs TranslateMouseEventArgs(MouseEventArgs e) {
			Point point = e.Location;
			point = PointToScreen(point);
			point = FrameOwner.PointToClient(point);
			return new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta);
		}
		public void SetInactive() {
			SetVisibleInactive(this, true);
		}
	}
	public class DragDropItemPreviewCursor : DragDropItemCursor {
		static double CalcResizeRatio(int height, int width) {
			return 1;
		}
		public override int CursorWidth {
			get {
				if(DragItemImage != null)
					return (int)(DragItemImage.Width * CalcResizeRatio(DragItemImage.Height, DragItemImage.Width));
				else return base.CursorWidth;
			}
		}
		public override int CursorHeight {
			get {
				if(DragItemImage != null)
					return (int)(DragItemImage.Height * CalcResizeRatio(DragItemImage.Height, DragItemImage.Width));
				else
					return base.CursorHeight;
			}
		}
		[ThreadStatic]
		static DragDropItemPreviewCursor _default;
		public static new bool DefaultExists {
			get {
				return _default != null;
			}
		}
		public static new DragDropItemPreviewCursor Default {
			get {
				if(_default == null) _default = new DragDropItemPreviewCursor();
				return _default;
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(DragItemImage != null) {
				Rectangle rect = ClientRectangle;
				e.Graphics.FillRectangle(Brushes.Magenta, rect);
				e.Graphics.DrawImage(DragItemImage, rect);
			}
		}
		Image imageCore = null;
		public Image DragItemImage {
			get { return imageCore; }
			set { imageCore = value; }
		}
	}
	public class DragDropItemCursor : BaseToolFrame {
		BaseLayoutItem itemCore = null;
		DragCursorFramePainter dragCursorPainterCore = null;
		internal static int cursorWidth = (int)(80 * Skins.DpiProvider.Default.DpiScaleFactor);
		internal static int cursorHeight = (int)(24 * Skins.DpiProvider.Default.DpiScaleFactor);
		public virtual int CursorWidth { get { return cursorWidth; } }
		public virtual int CursorHeight { get { return cursorHeight; } }
		protected Size CursorSize { get { return new Size(CursorWidth, CursorHeight); } }
		[ThreadStatic]
		static DragDropItemCursor _default;
		public static bool DefaultExists {
			get {
				return _default != null;
			}
		}
		public static DragDropItemCursor Default {
			get {
				if(_default == null) _default = new DragDropItemCursor();
				return _default;
			}
		}
		public static void Reset() {
			if(_default != null) {
				_default.Dispose();
				_default = null;
			}
		}
		protected DragDropItemCursor() {
			Opacity = 1;
			TopMost = true;
			Visible = false;
			Location = Point.Empty;
			Size = CursorSize;
			dragCursorPainterCore = CreateCursorFramePainter();
		}
		protected override bool ShowWithoutActivation {
			get {
				return true;
			}
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams p = base.CreateParams;
				p.Style |= 0x40000000;
				p.ExStyle |= 0x8000000;
				return p;
			}
		}
		protected virtual DragCursorFramePainter CreateCursorFramePainter() {
			return new DragCursorFramePainter();
		}
		public BaseLayoutItem DragItem {
			get { return itemCore; }
			set { itemCore = value; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(DragItem != null) {
				Rectangle rect = ClientRectangle;
				dragCursorPainterCore.DrawItem(e.Graphics, DragItem, rect);
			}
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
		}
		public Rectangle GetItemCursorRect(LayoutTreeViewHitInfo hitInfo) {
			Rectangle cursorRect = Rectangle.Empty;
			if(hitInfo.Node != null) {
				cursorRect = hitInfo.Node.Bounds;
				cursorRect.Offset(-20, (cursorRect.Height - CursorHeight) / 2);
				cursorRect.Size = new Size(cursorRect.Width + 24, CursorHeight);
			}
			switch(hitInfo.HitType) {
				case LayoutTreeViewHitType.Bottom:
					cursorRect.Offset(0, CursorHeight);
					break;
				case LayoutTreeViewHitType.Left:
					cursorRect.Offset(-CursorWidth, 0);
					cursorRect.Size = new Size(CursorWidth, CursorHeight);
					break;
				case LayoutTreeViewHitType.Top:
					cursorRect.Offset(0, -CursorHeight);
					break;
				case LayoutTreeViewHitType.Right:
					cursorRect.Offset(cursorRect.Width, 0);
					cursorRect.Size = new Size(CursorWidth, CursorHeight);
					break;
				default:
					Point pt = hitInfo.HitPoint;
					pt.Offset(-CursorWidth / 2, -CursorHeight / 2);
					cursorRect = new Rectangle(pt, new Size(CursorWidth, CursorHeight));
					break;
			}
			return cursorRect;
		}
		protected void ProcessKeyDownOnDragging(KeyEventArgs e) {
			IDragDropDispatcher dispatcher = DragDropDispatcherFactory.Default;
			switch(e.KeyCode) {
				case Keys.Escape: dispatcher.CancelAllDragOperations();
					break;
				default:
					if(dispatcher.ActiveReceiver != null) dispatcher.ProcessKeyEvent(dispatcher.ActiveReceiver, e);
					break;
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			ProcessKeyDownOnDragging(e);
		}
	}
}
