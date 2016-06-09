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
using System.IO;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout;
using DevExpress.Utils.Win;
using System.Runtime.InteropServices;
using DevExpress.XtraLayout.Utils;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraLayout.Customization.Behaviours;
namespace DevExpress.XtraLayout.Dragging {
	public class LayoutMessageForwardingWindow : BaseDragHelperForm {
		protected ILayoutControl controlOwner;
		public LayoutMessageForwardingWindow(ILayoutControl owner)
			: base(owner.DesignMode ? 0 : owner.OptionsView.DragFadeAnimationSpeed, owner.DesignMode ? 0 : owner.OptionsView.DragFadeAnimationFrameCount) {
			controlOwner = owner;
		}
		MouseEventArgs TranslateMouseEventArgs(MouseEventArgs e) {
			Point point = e.Location;
			point = PointToScreen(point);
			point = controlOwner.Control.PointToClient(point);
			MouseEventArgs newEventArgs = new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta);
			return newEventArgs;
		}
		DragEventArgs TranslateDragEventArgs(DragEventArgs e) {
			Point point = new Point(e.X, e.Y);
			point = PointToScreen(point);
			point = controlOwner.Control.PointToClient(point);
			DragEventArgs newEventArgs = new DragEventArgs(e.Data, e.KeyState, point.X, point.Y, e.Effect, e.AllowedEffect);
			return newEventArgs;
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			Update();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			controlOwner.ActiveHandler.ProcessMessage(EventType.MouseMove, TranslateMouseEventArgs(e));
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			controlOwner.ActiveHandler.ProcessMessage(EventType.MouseDown, e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			controlOwner.ActiveHandler.ProcessMessage(EventType.MouseUp, e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			controlOwner.ActiveHandler.OnKeyDown(EventType.KeyDown, e);
		}
		protected override void OnDragEnter(DragEventArgs drgevent) {
			(controlOwner as LayoutControl).DoDragEnter(TranslateDragEventArgs(drgevent));
		}
		protected override void OnDragLeave(EventArgs e) {
			(controlOwner as LayoutControl).DoDragLeave(e);
		}
		protected override void OnDragOver(DragEventArgs drgevent) {
			(controlOwner as LayoutControl).DoDragOver(TranslateDragEventArgs(drgevent));
		}
	}
	public class DragFrameWindow : LayoutMessageForwardingWindow, IToolFrame {
		protected Rectangle dragBounds = Rectangle.Empty;
		Customization.LayoutItemDragController dragControllerCore;
		bool allowShowWindowCore = true;
		bool isDisposing = false;
		public DragFrameWindow(ILayoutControl control)
			: base(control) {
			base.SetStyle(ControlStyles.Selectable, false);
			base.SetStyle(ControlStyles.DoubleBuffer, true);
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.StartPosition = FormStartPosition.Manual;
			this.BackColor = Color.Transparent;
			base.MinimumSize = Size.Empty;
			base.Size = Size.Empty;
			base.Visible = false;
			base.TabStop = false;
			base.Opacity = 0.5;
			base.Size = Size.Empty;
			base.ShowInTaskbar = false;
			TopLevel = true;
		}
		public Customization.LayoutItemDragController DragController {
			get { return dragControllerCore; }
			set {
				dragControllerCore = value;
				if(!IsDisposing) UpdateWindow();
			}
		}
		internal bool IsDisposing { get { return isDisposing; } set { isDisposing = value; } }
		Size Offset { 
			get { return new Size(1, 1); } 
		}
		protected virtual Size CalcSize(Control control) {
			if (control.Parent == null) return control.ClientSize;
			else {
				Size sz = CalcSize(control.Parent);
				return new Size(Math.Min(sz.Width, control.ClientSize.Width), Math.Min(sz.Height, control.ClientSize.Height));
			}
		}
		protected virtual Point CalcLocation(Control control) {
			if (control.Parent == null) return control.PointToScreen(Point.Empty);
			else {
				Point pointParent = CalcLocation(control.Parent);
				Point pointMe = control.PointToScreen(Point.Empty);
				return new Point(Math.Max(pointMe.X, pointParent.X), Math.Max(pointMe.Y, pointParent.Y));
			}
		}
		void CheckWindowPositionAndSize() {
			if (controlOwner.Bounds != Bounds ) {
				Point location = CalcLocation(controlOwner.Control) - Offset;
				Location = location;
				Size = CalcSize(controlOwner.Control) + Offset + Offset; 
			}
		}
		public bool AllowShowWindow {
			get { return CalcAllowShowWindow(); }
			set { allowShowWindowCore = value; }
		}
		public bool IsVista {
			get { return (Environment.OSVersion.Version.Major == 6); }
		}
		protected virtual bool CalcAllowShowWindow() {
			return allowShowWindowCore ;
		}
		public void Reset() {
			dragBounds = Rectangle.Empty;
			if(!AllowShowWindow) {
				DragController = null;
			}
		}
		Rectangle prevFrame = Rectangle.Empty;
		protected virtual void UpdateReversibleFrame() {
			if(controlOwner != null && controlOwner.OptionsView.DrawAdornerLayer == DefaultBoolean.True && !controlOwner.DesignMode) base.Opacity = 0;
			else base.Opacity = 0.5;
			if(prevFrame != Rectangle.Empty) SplitterLineHelper.Default.DrawReversibleFrame(controlOwner.Control.Handle, prevFrame);
			Invalidate();
			(controlOwner as LayoutControl).Update();
			Rectangle bounds = dragBounds;
			Point location = controlOwner.Control.PointToScreen(Point.Empty);
			bounds.X += Location.X;
			bounds.Y += Location.Y;
			Point temp = controlOwner.Control.PointToClient(new Point(bounds.X, bounds.Y));
			bounds.X = temp.X;
			bounds.Y = temp.Y;
			if(bounds != Rectangle.Empty) SplitterLineHelper.Default.DrawReversibleFrame(controlOwner.Control.Handle, bounds);
			prevFrame = bounds;
		}
		void UpdateWindow() {
			if(controlOwner != null && controlOwner.OptionsView.DrawAdornerLayer == DefaultBoolean.True && !controlOwner.DesignMode) return;
			CheckWindowPositionAndSize();
			Rectangle oldDdragBounds = dragBounds;
			if(DragController == null) {
				dragBounds = Rectangle.Empty;
			} else {
				if(DragController.DragBounds.Size == Size.Empty) {
					dragBounds = Rectangle.Empty;
				}
				if(DragController.DragBounds != dragBounds) {
					dragBounds = DragController.DragBounds;
				}
			}
			dragBounds = TranslateToRealWindowRect(dragBounds);
			if(dragBounds != oldDdragBounds) {
				if(AllowShowWindow) {
					Visible= true;
					Invalidate();
					Update();
				} else {
					UpdateReversibleFrame();
				}
			}
		}
		static Pen dragBoundsPen;
		Brush dragBoundsBrush;
		public static Pen DragBoundsPen {
			get {
				if(dragBoundsPen == null) dragBoundsPen = new Pen(Color.FromArgb(255, DragDropGlyph.DragBoundsPen.Color), 1);
				return dragBoundsPen;
			}
		}
		protected Brush DragBoundsBrush {
			get {
				if(dragBoundsBrush == null) dragBoundsBrush = new SolidBrush(DragBoundsPen.Color);
				return dragBoundsBrush;
			}
		}
		Rectangle TranslateToRealWindowRect(Rectangle rect) {
			return new Rectangle(rect.Location + Offset, rect.Size);
		}
		static void DrawRectangleShape(PaintEventArgs e, Rectangle rect, Pen DragBoundsPen) {
			e.Graphics.DrawRectangle(DragBoundsPen,rect);
			e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(38, DragBoundsPen.Color)), rect);
		}
		bool ShouldDrawEmptyDragBoundsInTabPage {
			get {
				LayoutGroup group = DragController.Item as LayoutGroup;
				if(group != null && group.ParentTabbedGroup != null && group.Items.Count == 0) {
					return true;
				}
				return false;
			}
		}
		bool ShouldShowTabPagesDragBounds {
			get {
				TabbedGroupHitInfo tgh = DragController.HitInfo as TabbedGroupHitInfo;
				if(tgh != null && tgh.TabbedGroupHotPageIndex >= 0 && DragController.DragItem is LayoutGroup) {
					return true;
				}
				return false;
			}
		}
		protected internal static void DrawDragBounds(PaintEventArgs e, Rectangle dragBounds , LayoutItemDragController DragController, bool ShouldShowTabPagesDragBounds, bool ShouldDrawEmptyDragBoundsInTabPage, Pen DragBoundsPen) {
			if(DragController != null) {
				if(DragController.Item != null) {
					LayoutGroup moveToGroup = DragController.Item as LayoutGroup;
					LayoutItem baseItem = DragController.Item as LayoutItem;
					TabbedGroup moveToTabbedGroup = DragController.Item as TabbedGroup;
					LayoutItem dragItem = DragController.DragItem as LayoutItem;
					LayoutGroup dragGroup = DragController.DragItem as LayoutGroup;
					TabbedGroup dragTabbedGroup = DragController.DragItem as TabbedGroup;
					if(moveToGroup != null && moveToGroup.Parent == null && moveToGroup.Items.Count == 0) {
						dragBounds = PatchDragBounds(dragBounds, DragController.Item.Y == 0, DragController.Item.X == 0, DragController.Item.Owner.OptionsView.DrawAdornerLayer == DefaultBoolean.True);
						DrawRectangleShape(e, dragBounds, DragBoundsPen);
						return;
					}
					if(DragController.MoveType == MoveType.Outside && !(ShouldShowTabPagesDragBounds || ShouldDrawEmptyDragBoundsInTabPage)) {
						DrawOutsideDragBounds(e, dragBounds, dragBounds, DragBoundsPen);
						return;
					} else {
						dragBounds = PatchDragBounds(dragBounds, DragController.Item.Y == 0, DragController.Item.X == 0, DragController.Item.Owner.OptionsView.DrawAdornerLayer== DefaultBoolean.True);
						DrawRectangleShape(e, dragBounds, DragBoundsPen);
						return;
					}
				}
			}
			return;
		}
		static Rectangle PatchDragBounds(Rectangle dragBounds, bool yIsEmpty, bool xIsEmpty, bool isAdornerDrawing) {
			if(isAdornerDrawing)
				dragBounds = new Rectangle(dragBounds.Location, new Size(dragBounds.Size.Width - 1, dragBounds.Size.Height - 1));
			else
				dragBounds = new Rectangle(xIsEmpty ? dragBounds.Location.X : dragBounds.Location.X - 1, yIsEmpty ? dragBounds.Location.Y : dragBounds.Location.Y - 1, xIsEmpty ? dragBounds.Width - 1 : dragBounds.Width, yIsEmpty ? dragBounds.Height - 1 : dragBounds.Height);
			return dragBounds;
		}
		protected static void DrawArrow(PaintEventArgs e, Point arrowStartPoint, Locations location, Pen DragBoundsPen) {
			Point leftSideArrowEnd = arrowStartPoint, rightSideArrowEnd = arrowStartPoint;
			switch(location) {
				case Locations.Left:
					leftSideArrowEnd.X -= triangleSize.Width;
					leftSideArrowEnd.Y -= triangleSize.Height/2;
					rightSideArrowEnd.X -= triangleSize.Width;
					rightSideArrowEnd.Y += triangleSize.Height / 2;
					break;
				case Locations.Right:
					leftSideArrowEnd.X += triangleSize.Width;
					leftSideArrowEnd.Y -= triangleSize.Height / 2;
					rightSideArrowEnd.X += triangleSize.Width;
					rightSideArrowEnd.Y += triangleSize.Height / 2;
					break;
				case Locations.Top:
					leftSideArrowEnd.X += triangleSize.Height / 2;
					leftSideArrowEnd.Y -= triangleSize.Width;
					rightSideArrowEnd.X -= triangleSize.Height / 2;
					rightSideArrowEnd.Y -= triangleSize.Width;
					break;
				case Locations.Bottom:
					leftSideArrowEnd.X -= triangleSize.Height / 2;
					leftSideArrowEnd.Y += triangleSize.Width;
					rightSideArrowEnd.X += triangleSize.Height / 2;
					rightSideArrowEnd.Y += triangleSize.Width;
					break;
			}
			e.Graphics.DrawLine(DragBoundsPen, arrowStartPoint, leftSideArrowEnd);
			e.Graphics.DrawLine(DragBoundsPen, arrowStartPoint, rightSideArrowEnd);
			switch(location) {
				case Locations.Left:
				case Locations.Right:
					arrowStartPoint.Y += 1;
					leftSideArrowEnd.Y += 1;
					rightSideArrowEnd.Y += 1;
					break;
				case Locations.Top:
				case Locations.Bottom:
					arrowStartPoint.X += 1;
					leftSideArrowEnd.X += 1;
					rightSideArrowEnd.X += 1;
					break;
			}
			e.Graphics.DrawLine(DragBoundsPen, arrowStartPoint, leftSideArrowEnd);
			e.Graphics.DrawLine(DragBoundsPen, arrowStartPoint, rightSideArrowEnd);
		}
		static Size triangleSize = new Size(4, 8);
		protected static void DrawOutsideDragBounds(PaintEventArgs e, Rectangle rect, Rectangle dragBounds, Pen DragBoundsPen) {
			if(rect.Size == Size.Empty) return;
			if(dragBounds.Width > dragBounds.Height) {
				int hdiff = dragBounds.Height / 2 - triangleSize.Height / 2;
				e.Graphics.DrawLine(DragBoundsPen, rect.Left, rect.Top + rect.Height / 2, rect.Right, rect.Top + rect.Height / 2);
				e.Graphics.DrawLine(DragBoundsPen, rect.Left, rect.Top + rect.Height / 2 + 1, rect.Right, rect.Top + rect.Height / 2 + 1);
				DrawArrow(e, new Point(rect.Left, rect.Top + rect.Height / 2), Locations.Left, DragBoundsPen);
				DrawArrow(e, new Point(rect.Right, rect.Top + rect.Height / 2), Locations.Right, DragBoundsPen);
			} else {
				int wdiff = dragBounds.Width / 2 - triangleSize.Height / 2;
				e.Graphics.DrawLine(DragBoundsPen, rect.Left + rect.Width / 2, rect.Top, rect.Left + rect.Width / 2, rect.Bottom);
				e.Graphics.DrawLine(DragBoundsPen, rect.Left + rect.Width / 2 + 1, rect.Top, rect.Left + rect.Width / 2 + 1, rect.Bottom);
				DrawArrow(e, new Point(rect.Left + rect.Width / 2, rect.Top), Locations.Top, DragBoundsPen);
				DrawArrow(e, new Point(rect.Left + rect.Width / 2, rect.Bottom), Locations.Bottom, DragBoundsPen);
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(!IsHandleCreated || !controlOwner.Control.IsHandleCreated || (controlOwner.OptionsView.DrawAdornerLayer == DefaultBoolean.True && !controlOwner.DesignMode)) return;
			Point lcScreenLocation = controlOwner.Control.PointToScreen(Point.Empty);
			Point windowLocation = this.Location;
			e.Graphics.Transform = new System.Drawing.Drawing2D.Matrix(
				1, 0, 0, 1,
				lcScreenLocation.X - (windowLocation.X + Offset.Width),
				lcScreenLocation.Y - (windowLocation.Y + Offset.Height));
			if(DragController != null) DrawDragBounds(e,dragBounds, DragController, ShouldShowTabPagesDragBounds, ShouldDrawEmptyDragBoundsInTabPage, DragBoundsPen);
		}
		#region IToolFrame
		#endregion
	}
}
