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
using System.Linq;
using System.Text;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.XtraGrid.Design {
	public abstract class BaseControlGlyph : Glyph {
		Control control;
		ControlDesigner designer;
		BehaviorService service;
		public BaseControlGlyph(ControlDesigner designer, Control control, BehaviorService service, Behavior behavior)
			: base(behavior) {
				this.designer = designer;
				this.control = control;
				this.service = service;
		}
		public BehaviorService Service { get { return service; } }
		public ControlDesigner Designer { get { return designer; } }
		public Control Control { get { return control; } }
		public override System.Drawing.Rectangle Bounds {
			get {
				return ToAdorner(new Rectangle(Point.Empty, Control.Size));
			}
		}
		public Point ToAdorner(Point point) {
			if(Service == null) return point;
			Point p = Service.ControlToAdornerWindow(control);
			p.Offset(point);
			return p;
		}
		public Rectangle ToAdorner(Rectangle rectangle) {
			rectangle.Location = ToControl(rectangle.Location);
			return rectangle;
		}
		public Rectangle ToControl(Rectangle rectangle) {
			rectangle.Location = ToControl(rectangle.Location);
			return rectangle;
		}
		public Point ToScreen(Point point) {
			if(Service == null) return point;
			return Service.AdornerWindowPointToScreen(point);
		}
		public Point ToControl(Point point) {
			if(Service == null || !control.IsHandleCreated) return point;
			Point p = Service.AdornerWindowPointToScreen(point);
			return control.PointToClient(p);
		}
		protected Point ControlLocation {
			get {
				if(Service == null) return Point.Empty;
				return Service.ControlToAdornerWindow(Control);
			}
		}
	}
	public class GridControlGlyph : BaseControlGlyph {
		public GridControlGlyph(GridControlDesigner designer, GridControl grid, BehaviorService service)
			: base(designer, grid, service, new GridControlGlyphBehavior()) {
		}
		public new GridControlDesigner Designer { get { return (GridControlDesigner)base.Designer; } }
		public override Cursor GetHitTest(Point p) {
			return Designer.GetHitTestCore(ToControl(p));
		}
		public override void Paint(System.Windows.Forms.PaintEventArgs pe) {
		}
	}
	public abstract class BaseControlDesignerBehavior : Behavior {
		protected abstract BaseControlDesigner Designer { get; }
		protected virtual bool IsHandleMouse(Point location) { return false; }
		Point mouseLast = Point.Empty;
		Control mouseLastTarget;
		protected virtual Control ControlFromPosition(Point point) {
			if(Designer == null) return null;
			return FindChild(Designer.Control, point);
		}
		Control FindChild(Control control, Point point) {
			if(control == null) return null;
			if(!control.HasChildren) return control;
			Control res = control.GetChildAtPoint(point, GetChildAtPointSkip.Disabled | GetChildAtPointSkip.Invisible | GetChildAtPointSkip.Transparent);
			if(res == null || !res.IsHandleCreated) return control;
			return FindChild(res, res.PointToClient(control.PointToScreen(point)));
		}
		public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc) {
			UpdateOwner(g);
			this.mouseLast = mouseLoc = ToControl(g, mouseLoc);
			if(IsHandleMouse(mouseLoc)) {
				Control control = ControlFromPosition(mouseLast);
				mouseLoc = TranslatePoint(control, mouseLoc);
				ControlMouseHelper.InvokeMouseDown(control, new MouseEventArgs(button, 1, mouseLoc.X, mouseLoc.Y, 0));
				return true;
			}
			return base.OnMouseDown(g, button, mouseLoc);
		}
		public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc) {
			UpdateOwner(g);
			this.mouseLast = mouseLoc = ToControl(g, mouseLoc);
			if(IsHandleMouse(mouseLoc)) {
				Control control = ControlFromPosition(mouseLast);
				mouseLoc = TranslatePoint(control, mouseLoc);
				CheckMouseEnterLeave(control);
				ControlMouseHelper.InvokeMouseMove(control, new MouseEventArgs(button, 1, mouseLoc.X, mouseLoc.Y, 0));
				return true;
			}
			return base.OnMouseMove(g, button, mouseLoc);
		}
		void CheckMouseEnterLeave(Control control) {
			Control prev = this.mouseLastTarget;
			this.mouseLastTarget = control;
			if(prev == null) return;
			if(prev == control) return;
			ControlMouseHelper.InvokeMouseLeave(prev);
		}
		public override bool OnMouseUp(Glyph g, MouseButtons button) {
			UpdateOwner(g);
			if(IsHandleMouse(mouseLast)) {
				Control control = ControlFromPosition(mouseLast);
				Point mouseLoc = TranslatePoint(control, mouseLast);
				ControlMouseHelper.InvokeMouseUp(control, new MouseEventArgs(button, 1, mouseLoc.X, mouseLoc.Y, 0));
				return true;
			}
			return base.OnMouseUp(g, button);
		}
		public override bool OnMouseLeave(Glyph g) {
			UpdateOwner(g);
			if(IsHandleMouse(Point.Empty)) {
				ControlMouseHelper.InvokeMouseLeave(ControlFromPosition(Point.Empty));
				return true;
			}
			return base.OnMouseLeave(g);
		}
		public override bool OnMouseEnter(Glyph g) {
			UpdateOwner(g);
			if(IsHandleMouse(Point.Empty)) {
				ControlMouseHelper.InvokeMouseEnter(ControlFromPosition(Point.Empty));
				return true;
			}
			return base.OnMouseEnter(g);
		}
		Point TranslatePoint(Control control, Point point) {
			if(Designer == null) return point;
			if(Designer.Control != control) {
				return control.PointToClient(Designer.Control.PointToScreen(point));
			}
			return point;
		}
		protected Point ToControl(Glyph g, Point point) {
			if(Designer == null) return point;
			return ToControl(g, Designer.Control, point);
		}
		protected Point ToControl(Glyph g, Control control, Point point) {
			BaseControlGlyph bg = g as BaseControlGlyph;
			if(bg != null && control != null && control.IsHandleCreated) return control.PointToClient(bg.ToScreen(point));
			return point;
		}
		protected abstract void UpdateOwner(Glyph g);
	}
	public class GridControlGlyphBehavior : BaseControlDesignerBehavior {
		GridControlGlyph owner;
		public GridControlGlyphBehavior() {
		}
		public GridControlGlyph Owner { get { return owner; } }
		protected override BaseControlDesigner Designer { get { return Owner == null ? null : Owner.Designer; } }
		protected GridControlDesigner GridDesigner { get { return Designer as GridControlDesigner; } }
		protected override Control ControlFromPosition(Point point) {
			if(GridDesigner == null || GridDesigner.Grid == null) return base.ControlFromPosition(point);
			GridView view = GridDesigner.Grid.DefaultView as GridView;
			if(view != null && (view.IsDraggingState || view.IsSizingState)) return Designer.Control;
			return base.ControlFromPosition(point);
		}
		protected override bool IsHandleMouse(Point location) { return Designer != null && ControlFromPosition(location) != null; }
		protected override void UpdateOwner(Glyph g) {
			BaseControlGlyph glyph = g as BaseControlGlyph;
			if(glyph != null) owner = glyph as GridControlGlyph;
		}
	}
}
