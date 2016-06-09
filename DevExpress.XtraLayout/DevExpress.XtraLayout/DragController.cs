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
using DevExpress.Utils.Win;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout;
using System.Runtime.InteropServices;	
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout.Dragging {
	public class DragCursor : LayoutMessageForwardingWindow, IToolFrame {
		bool drawCrossCore = false;
		Cursor cursor = DevExpress.Utils.DragDrop.DragManager.DragRemoveCursor;
		Rectangle cursorRect = Rectangle.Empty;
		Rectangle dragHeaderRect = Rectangle.Empty;
		LayoutItemDragController dragControllerCore;
		DragHeaderPainter painter;
		DraggingVisualizer visualizer;
		public DragCursor(LayoutControl owner, DraggingVisualizer visualizer)
			: base(owner) {
			SetOpacity();
			this.visualizer = visualizer;
			try {
				if(controlOwner == null) throw new Exception("owner == null");
				if(controlOwner.LookAndFeel == null) throw new Exception("controlOwner.LookAndFeel == null");
				if(controlOwner.LookAndFeel.ActiveLookAndFeel == null) throw new Exception("controlOwner.LookAndFeel.ActiveLookAndFeel == null");
			}
			catch {
				painter = new DragHeaderPainter(UserLookAndFeel.Default.ActiveLookAndFeel, UserLookAndFeel.Default.ActiveStyle);
			}
			finally {
				if(painter == null)
					painter = new DragHeaderPainter(controlOwner.LookAndFeel.ActiveLookAndFeel, controlOwner.LookAndFeel.ActiveStyle);
			}
		}
		protected void SetOpacity() { Opacity = 0.7; }
		public Rectangle DragHeaderScreenRect {
			get { return dragHeaderRect; }
			set { dragHeaderRect = value; UpdateRectangles(); }
		}
		Rectangle PlaceInCenter(Rectangle rectToPlace, Rectangle rect) {
			return new Rectangle(rect.X + rect.Width / 2 - rectToPlace.Width / 2, rect.Y + rect.Height / 2 - rectToPlace.Height / 2, rectToPlace.Width, rectToPlace.Height);
		}
		void UpdateRectangles() {
			Width = Math.Max(dragHeaderRect.Width, cursor.Size.Width);
			Height = Math.Max(dragHeaderRect.Height, cursor.Size.Height);
			cursorRect = PlaceInCenter(new Rectangle(Point.Empty, cursor.Size), new Rectangle(Point.Empty, Size));
			Point screenPoint = dragHeaderRect.Location;
			dragHeaderRect = PlaceInCenter(new Rectangle(Point.Empty, dragHeaderRect.Size), new Rectangle(Point.Empty, Size));
			Location = screenPoint;
			UpdateWindows();
		}
		void UpdateWindows() {
			if(controlOwner.CustomizationForm != null && controlOwner.CustomizationForm.Visible) {
				Point cursorLocationInCustomizationWindow = PointToScreen(Location);
				cursorLocationInCustomizationWindow = controlOwner.CustomizationForm.PointToClient(cursorLocationInCustomizationWindow);
				controlOwner.CustomizationForm.Invalidate(new Rectangle(cursorLocationInCustomizationWindow, Size));
				controlOwner.CustomizationForm.Update();
			}
			Point cursorLocationInDragWindow = PointToScreen(Location);
			cursorLocationInDragWindow = visualizer.DragWindow.PointToClient(cursorLocationInDragWindow);
			visualizer.DragWindow.Invalidate(new Rectangle(cursorLocationInDragWindow, Size));
			visualizer.DragWindow.Update();
			Invalidate();
			Update();
		}
		public bool DrawCross {
			get { return drawCrossCore; }
			set { drawCrossCore = value; }
		}
		public LayoutItemDragController DragController {
			get { return dragControllerCore; }
			set { dragControllerCore = value; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(dragControllerCore != null && e.Graphics != null) {
				painter.Paint(e.Graphics, DragController.DragItem, ObjectState.Normal, dragHeaderRect, null);
				if(DrawCross)
					cursor.Draw(e.Graphics, cursorRect);
			}
		}
	}
	public class DraggingVisualizer : IDisposable{
		ILayoutControl ownerCore;
		DragFrameWindow dragWindowCore;
		BaseLayoutItem fakeDragItem;
		public DraggingVisualizer(ILayoutControl owner) {
			this.ownerCore = owner;
		}
		public void Dispose() {
			if(dragWindowCore!=null) {
				dragWindowCore.IsDisposing = true;
				HideDragBounds();
				dragWindowCore.Dispose();
				dragWindowCore=null;
			}
			if(fakeDragItem!=null) {
				fakeDragItem.Dispose();
				fakeDragItem = null;
			}
			ownerCore = null;
		}
		protected ILayoutControl Owner { get { return ownerCore; } }
		public void Reset() {
			if(dragWindowCore!=null) HideDragBounds();
		}
		public bool IsDragBoundsVisible {
			get { return DragWindow.Visible; }
		}
		LayoutGroupHandler RootHandlerHelper {
			get { return Owner.RootGroup.Handler as LayoutGroupHandler; }
		}
		protected internal DragFrameWindow DragWindow {
			get {
				if (dragWindowCore == null) {
					dragWindowCore = CreateDragFrameWindow();
					if(dragWindowCore != null) dragWindowCore.AllowShowWindow = Owner.OptionsView.UseDefaultDragAndDropRendering || Owner.DesignMode;
				}
				return dragWindowCore;
			}
		}
		protected virtual DragFrameWindow CreateDragFrameWindow(){
			return new DragFrameWindow(Owner);
		}
		public virtual void HideDragBounds() {
			if(!Owner.DesignMode && Owner.OptionsView.DrawAdornerLayer == DefaultBoolean.True && Owner is LayoutControl && (Owner as LayoutControl).layoutAdornerWindowHandler != null) {
				(Owner as LayoutControl).layoutAdornerWindowHandler.dragController = null;
				(Owner as LayoutControl).InvalidateAdornerWindowHandler();
			}
			DragWindow.Visible = false;
			DragWindow.Reset();
		}
		public virtual void ShowDragBounds(LayoutItemDragController dragController) {
			if(!RootHandlerHelper.CheckCustomizationConstraints(dragController)) dragController = null;
			if(dragController != null && dragController.HitInfo != null && dragController.HitInfo.Item !=null) {
				BaseLayoutItem dropItem = dragController.HitInfo.Item;
				if(!dropItem.OptionsCustomization.CanDrop()) dragController = null;
			}
			if(!Owner.DesignMode && Owner.OptionsView.DrawAdornerLayer == DefaultBoolean.True && Owner is LayoutControl && (Owner as LayoutControl).layoutAdornerWindowHandler != null) {
				(Owner as LayoutControl).layoutAdornerWindowHandler.dragController = dragController;
				(Owner as LayoutControl).InvalidateAdornerWindowHandler();
			} 
			DragWindow.DragController = dragController;
		}
		protected void ProcessMouseMove(MouseEventArgs ea) {
			Point mouseMovePoint = ea.Location;
			Rectangle controlRect = new Rectangle(Point.Empty, Owner.Control.Size);
			Rectangle customizationFormScreenBounds = Rectangle.Empty;
			Point screenPoint = Owner.Control.PointToScreen(mouseMovePoint);
			if(Owner.CustomizationForm != null && Owner.CustomizationForm.Visible) {
				customizationFormScreenBounds = Owner.CustomizationForm.Bounds;
			}
			if(controlRect.Contains(mouseMovePoint) && !customizationFormScreenBounds.Contains(screenPoint)) {
				LayoutItemDragController controller = new LayoutItemDragController(FakeDragItem, Owner.RootGroup, mouseMovePoint);
				ShowDragBounds(controller);
			}
		}
		protected BaseLayoutItem FakeDragItem {
			get {
				if(fakeDragItem==null) fakeDragItem = new EmptySpaceItem();
				return fakeDragItem;
			}
		}
		public void ProcessMessageFromDesigner(EventType eventType, MouseEventArgs ea) {
			switch(eventType) {
				case EventType.MouseUp:
				case EventType.MouseEnter:
				case EventType.MouseLeave:
					Reset();
					break;
				case EventType.MouseMove:
					ProcessMouseMove(ea);
					break;
			}
		}
	}
}
