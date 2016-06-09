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

#if DXWhidbey
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
namespace DevExpress.XtraLayout.DesignTime {
	class MyBehaviorOld : Behavior {
		bool isDragging = false;
		Point hitPoint;
		Rectangle startRect;
		LayoutControlDesigner designer;
		public MyBehaviorOld(LayoutControlDesigner designer) {
			this.designer = designer;
		}
		public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc) {
			if(button == MouseButtons.Left && !isDragging) {
				StartDragging(mouseLoc);
				return true;
			}
			return false;
		}
		public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc) {
			if(isDragging && button == MouseButtons.Left) {
				DoDragging(g, mouseLoc);
				return true;
			}
			return false;
		}
		public override bool OnMouseLeave(Glyph g) {
			if(isDragging) {
				FinishDragging();
				return true;
			}
			return false;
		}
		public override bool OnMouseUp(Glyph g, MouseButtons button) {
			if(isDragging && button == MouseButtons.Left) {
				FinishDragging();
				return true;
			}
			return false;
		}
		void StartDragging(Point mouseLoc) {
			isDragging = true;
			hitPoint = mouseLoc;
			startRect = designer.Component.Bounds;
		}
		void DoDragging(Glyph g, Point mouseLoc) {
			LayoutGrabHandleGlyph glyph = g as LayoutGrabHandleGlyph;
			if(glyph != null) {
				int difx = mouseLoc.X - hitPoint.X;
				int dify = mouseLoc.Y - hitPoint.Y;
				bool sx = false, sy = false, mx = false, my = false;
				switch(glyph.type) {
					case LayoutGrabHandleGlyphType.LowerLeft:
						sx = sy = mx = true;
						break;
					case LayoutGrabHandleGlyphType.LowerRight:
						sx = sy = true;
						break;
					case LayoutGrabHandleGlyphType.MiddleBottom:
						sy = true;
						break;
					case LayoutGrabHandleGlyphType.MiddleLeft:
						sx = mx = true;
						break;
					case LayoutGrabHandleGlyphType.MiddleRight:
						sx = true;
						break;
					case LayoutGrabHandleGlyphType.MiddleTop:
						sy = my = true;
						break;
					case LayoutGrabHandleGlyphType.UpperLeft:
						sx = sy = mx = my = true;
						break;
					case LayoutGrabHandleGlyphType.UpperRight:
						sx = sy = my = true;
						break;
				}
				if(sx) designer.Component.Width = startRect.Width + difx;
				if(mx) designer.Component.Left = startRect.X - difx;
				if(sy) designer.Component.Height = startRect.Height + dify;
				if(my) designer.Component.Top = startRect.Y - dify;
			}
		}
		void FinishDragging() {
			isDragging = false;
		}
	}
	public class UnsafeNativeMethods {
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int ClientToScreen(HandleRef hWnd, [In, Out] NativeMethods.POINT pt);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hwnd, int msg, bool wparam, int lparam);
	}
	public class NativeMethods {
		public class POINT {
			public int x;
			public int y;
			public POINT(int x, int y) {
				this.x = x;
				this.y = y;
			}
			public POINT() {
			}
		}
	}
	public class StatusCommandUI {
		private IServiceProvider serviceProvider;
		public StatusCommandUI(IServiceProvider provider) {
			this.serviceProvider = provider;
		}
		public void SetStatusInformation(Component selectedComponent) {
			if(selectedComponent != null) {
				Rectangle rectangle1 = Rectangle.Empty;
				Control control1 = selectedComponent as Control;
				if(control1 != null) {
					rectangle1 = control1.Bounds;
				}
				else {
					PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(selectedComponent)["Bounds"];
					if((descriptor1 != null) && typeof(Rectangle).IsAssignableFrom(descriptor1.PropertyType)) {
						rectangle1 = (Rectangle)descriptor1.GetValue(selectedComponent);
					}
				}
			}
		}
		public void SetStatusInformation(Rectangle bounds) {
		}
		public void SetStatusInformation(Component selectedComponent, Point location) {
			if(selectedComponent != null) {
				Rectangle rectangle1 = Rectangle.Empty;
				Control control1 = selectedComponent as Control;
				if(control1 != null) {
					rectangle1 = control1.Bounds;
				}
				else {
					PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(selectedComponent)["Bounds"];
					if((descriptor1 != null) && typeof(Rectangle).IsAssignableFrom(descriptor1.PropertyType)) {
						rectangle1 = (Rectangle)descriptor1.GetValue(selectedComponent);
					}
				}
				if(location != Point.Empty) {
					rectangle1.X = location.X;
					rectangle1.Y = location.Y;
				}
			}
		}
	}
	static class DesignerUtils {
		private static Size minDragSize;
		public static int HANDLEOVERLAP = 3;
		public static int HANDLESIZE = 6;
		public static Size MinDragSize {
			get {
				if(DesignerUtils.minDragSize == Size.Empty) {
					Size size1 = SystemInformation.DragSize;
					Size size2 = SystemInformation.DoubleClickSize;
					DesignerUtils.minDragSize.Width = Math.Max(size1.Width, size2.Width);
					DesignerUtils.minDragSize.Height = Math.Max(size1.Height, size2.Height);
				}
				return DesignerUtils.minDragSize;
			}
		}
	}
	public class FakeDragAssistanceManager {
		public FakeDragAssistanceManager(LayoutControlDesigner provider, ArrayList list, bool flag) {
		}
		public void OnMouseUp() { }
		public Point OnMouseMove() {
			return Point.Empty;
		}
		public void OnMouseMove(Rectangle rect) { }
		public void EraseSnapLines() { }
	}
	public class SelectionGlyphBase : Glyph {
		protected Rectangle bounds;
		protected Rectangle hitBounds;
		protected Cursor hitTestCursor;
		protected SelectionRules rules;
		public SelectionGlyphBase(System.Windows.Forms.Design.Behavior.Behavior behavior)
			: base(behavior) {
		}
		public override Cursor GetHitTest(Point p) {
			if(this.hitBounds.Contains(p)) {
				return this.hitTestCursor;
			}
			return null;
		}
		public override void Paint(PaintEventArgs pe) { }
		public override Rectangle Bounds {
			get {
				return this.bounds;
			}
		}
		public Cursor HitTestCursor {
			get {
				return this.hitTestCursor;
			}
		}
		public SelectionRules SelectionRules {
			get {
				return this.rules;
			}
		}
	}
	public class DesignTimeCursorHelperGlyph : Glyph {
		Control control;
		BehaviorService behaviorSvc;
		Rectangle controlBounds;
		Rectangle Translate(Rectangle rect) {
			Point edge = behaviorSvc.ControlToAdornerWindow(control);
			return new Rectangle(new Point(edge.X + rect.X, edge.Y + rect.Y), rect.Size);
		}
		public DesignTimeCursorHelperGlyph(BehaviorService behaviorSvc, Behavior behavior, Control control)
			: base(behavior) {
			this.control = control;
			this.behaviorSvc = behaviorSvc;
			this.controlBounds = Translate(new Rectangle(Point.Empty, control.Size));
		}
		public override Cursor GetHitTest(Point p) {
			return null;
		}
		public override void Paint(PaintEventArgs pe) {  }
		public override Rectangle Bounds {
			get { return controlBounds; }
		}
	}
	public class LayoutGrabHandleGlyph : SelectionGlyphBase {
		BehaviorService behaviorSvc;
		public LayoutGrabHandleGlyphType type;
		Control control;
		Rectangle controlBounds;
		public LayoutGrabHandleGlyph(BehaviorService behaviorSvc, Control control, LayoutGrabHandleGlyphType type, Behavior behavior, bool primarySelection)
			: base(behavior) {
			this.control = control;
			this.type = type;
			this.behaviorSvc = behaviorSvc;
			this.controlBounds = Translate(new Rectangle(Point.Empty, control.Size));
			Rectangle controlBounds = new Rectangle(Point.Empty, control.Size);
			this.isPrimary = primarySelection;
			hitTestCursor = control.Cursor;
			rules = SelectionRules.AllSizeable;
			switch(type) {
				case LayoutGrabHandleGlyphType.UpperLeft: {
						hitTestCursor = Cursors.SizeNWSE;
						rules = SelectionRules.LeftSizeable | SelectionRules.TopSizeable;
						break;
					}
				case LayoutGrabHandleGlyphType.UpperRight: {
						hitTestCursor = Cursors.SizeNESW;
						rules = SelectionRules.RightSizeable | SelectionRules.TopSizeable;
						break;
					}
				case LayoutGrabHandleGlyphType.LowerLeft: {
						hitTestCursor = Cursors.SizeNESW;
						rules = SelectionRules.LeftSizeable | SelectionRules.BottomSizeable;
						break;
					}
				case LayoutGrabHandleGlyphType.LowerRight: {
						hitTestCursor = Cursors.SizeNWSE;
						rules = SelectionRules.RightSizeable | SelectionRules.BottomSizeable;
						break;
					}
				case LayoutGrabHandleGlyphType.MiddleTop: {
						hitTestCursor = Cursors.SizeNS;
						rules = SelectionRules.TopSizeable;
						break;
					}
				case LayoutGrabHandleGlyphType.MiddleBottom: {
						hitTestCursor = Cursors.SizeNS;
						rules = SelectionRules.BottomSizeable;
						break;
					}
				case LayoutGrabHandleGlyphType.MiddleLeft: {
						hitTestCursor = Cursors.SizeWE;
						rules = SelectionRules.LeftSizeable;
						break;
					}
				case LayoutGrabHandleGlyphType.MiddleRight: {
						hitTestCursor = Cursors.SizeWE;
						rules = SelectionRules.RightSizeable;
						break;
					}
			}
		}
		public Rectangle ControlBounds {
			get {
				return controlBounds;
			}
		}
		public override Rectangle Bounds {
			get {
				Rectangle bounds = Rectangle.Empty;
				switch(type) {
					case LayoutGrabHandleGlyphType.UpperLeft:
						bounds = new Rectangle((ControlBounds.X + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, (ControlBounds.Y + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
						break;
					case LayoutGrabHandleGlyphType.UpperRight:
						bounds = new Rectangle(ControlBounds.Right - DesignerUtils.HANDLEOVERLAP - 1, (ControlBounds.Y + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
						break;
					case LayoutGrabHandleGlyphType.LowerLeft:
						bounds = new Rectangle((ControlBounds.X + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, ControlBounds.Bottom - DesignerUtils.HANDLEOVERLAP - 1, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
						break;
					case LayoutGrabHandleGlyphType.LowerRight:
						bounds = new Rectangle(ControlBounds.Right - DesignerUtils.HANDLEOVERLAP - 1, ControlBounds.Bottom - DesignerUtils.HANDLEOVERLAP - 1, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
						break;
					case LayoutGrabHandleGlyphType.MiddleTop:
						if(ControlBounds.Width >= ((2 * DesignerUtils.HANDLEOVERLAP) + (2 * DesignerUtils.HANDLESIZE)))
							bounds = new Rectangle((ControlBounds.X + (ControlBounds.Width / 2)) - (DesignerUtils.HANDLESIZE / 2), (ControlBounds.Y + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
						break;
					case LayoutGrabHandleGlyphType.MiddleBottom:
						if(ControlBounds.Width >= ((2 * DesignerUtils.HANDLEOVERLAP) + (2 * DesignerUtils.HANDLESIZE)))
							bounds = new Rectangle((ControlBounds.X + (ControlBounds.Width / 2)) - (DesignerUtils.HANDLESIZE / 2), ControlBounds.Bottom - DesignerUtils.HANDLEOVERLAP - 1, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
						break;
					case LayoutGrabHandleGlyphType.MiddleLeft:
						if(ControlBounds.Height >= ((2 * DesignerUtils.HANDLEOVERLAP) + (2 * DesignerUtils.HANDLESIZE)))
							bounds = new Rectangle((ControlBounds.X + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, (ControlBounds.Y + (ControlBounds.Height / 2)) - (DesignerUtils.HANDLESIZE / 2), DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
						break;
					case LayoutGrabHandleGlyphType.MiddleRight:
						if(ControlBounds.Height >= ((2 * DesignerUtils.HANDLEOVERLAP) + (2 * DesignerUtils.HANDLESIZE)))
							bounds = new Rectangle(ControlBounds.Right - DesignerUtils.HANDLEOVERLAP - 1, (ControlBounds.Y + (ControlBounds.Height / 2)) - (DesignerUtils.HANDLESIZE / 2), DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
						break;
				}
				return bounds;
			}
		}
		public override Cursor GetHitTest(Point p) {
			if(Bounds.Contains(p)) {
				return hitTestCursor;
			}
			return null;
		}
		Rectangle Translate(Rectangle rect) {
			Point edge = behaviorSvc.ControlToAdornerWindow(control);
			return new Rectangle(new Point(edge.X + rect.X, edge.Y + rect.Y), rect.Size);
		}
		public override void Paint(PaintEventArgs pe) {
			Rectangle boundsCore = Bounds;
			boundsCore.Inflate(-1, -1);
			pe.Graphics.FillRectangle(new SolidBrush(Color.White), boundsCore);
			pe.Graphics.DrawRectangle(new Pen(Color.Black), boundsCore);
		}
		private bool isPrimary;
	}
	public enum LayoutGrabHandleGlyphType {
		UpperLeft,
		UpperRight,
		LowerLeft,
		LowerRight,
		MiddleTop,
		MiddleBottom,
		MiddleLeft,
		MiddleRight
	}
	public class ResizeBehavior : System.Windows.Forms.Design.Behavior.Behavior {
		private BehaviorService behaviorService;
		private const int borderSize = 2;
		private Cursor cursor;
		private bool didSnap;
		private bool dragging;
		private FakeDragAssistanceManager dragManager;
		private Graphics graphics;
		private Point initialPoint;
		private bool initialResize;
		private NativeMethods.POINT lastMouseAbs;
		private Point lastMouseLoc;
		private Region lastResizeRegion;
		private Point lastSnapOffset;
		private const int MINSIZE = 10;
		private Size parentGridSize;
		private Point parentLocation;
		private Control primaryControl;
		private bool pushedBehavior;
		private ResizeComponent[] resizeComponents;
		private DesignerTransaction resizeTransaction;
		private LayoutControlDesigner serviceProvider;
		private SelectionRules targetResizeRules;
		public ResizeBehavior(LayoutControlDesigner designer) {
			this.cursor = Cursors.Default;
			this.serviceProvider = designer;
			this.dragging = false;
			this.pushedBehavior = false;
			this.lastSnapOffset = Point.Empty;
			this.didSnap = false;
		}
		internal static int AdjustPixelsForIntegralHeight(Control control, int pixelsMoved) {
			PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(control)["IntegralHeight"];
			if(descriptor1 == null) {
				return pixelsMoved;
			}
			object obj1 = descriptor1.GetValue(control);
			if(!(obj1 is bool) || !((bool)obj1)) {
				return pixelsMoved;
			}
			PropertyDescriptor descriptor2 = TypeDescriptor.GetProperties(control)["ItemHeight"];
			if(descriptor2 == null) {
				return pixelsMoved;
			}
			if(pixelsMoved >= 0) {
				return (pixelsMoved - (pixelsMoved % ((int)descriptor2.GetValue(control))));
			}
			int num1 = (int)descriptor2.GetValue(control);
			return (pixelsMoved - (num1 - (Math.Abs(pixelsMoved) % num1)));
		}
		private Rectangle AdjustToGrid(Rectangle controlBounds, SelectionRules rules) {
			Rectangle rectangle1 = controlBounds;
			if((rules & SelectionRules.RightSizeable) != SelectionRules.None) {
				int num1 = controlBounds.Right % this.parentGridSize.Width;
				if(num1 > (this.parentGridSize.Width / 2)) {
					rectangle1.Width += this.parentGridSize.Width - num1;
				}
				else {
					rectangle1.Width -= num1;
				}
			}
			else if((rules & SelectionRules.LeftSizeable) != SelectionRules.None) {
				int num2 = controlBounds.Left % this.parentGridSize.Width;
				if(num2 > (this.parentGridSize.Width / 2)) {
					rectangle1.X += this.parentGridSize.Width - num2;
					rectangle1.Width -= this.parentGridSize.Width - num2;
				}
				else {
					rectangle1.X -= num2;
					rectangle1.Width += num2;
				}
			}
			if((rules & SelectionRules.BottomSizeable) != SelectionRules.None) {
				int num3 = controlBounds.Bottom % this.parentGridSize.Height;
				if(num3 > (this.parentGridSize.Height / 2)) {
					rectangle1.Height += this.parentGridSize.Height - num3;
				}
				else {
					rectangle1.Height -= num3;
				}
			}
			else if((rules & SelectionRules.TopSizeable) != SelectionRules.None) {
				int num4 = controlBounds.Top % this.parentGridSize.Height;
				if(num4 > (this.parentGridSize.Height / 2)) {
					rectangle1.Y += this.parentGridSize.Height - num4;
					rectangle1.Height -= this.parentGridSize.Height - num4;
				}
				else {
					rectangle1.Y -= num4;
					rectangle1.Height += num4;
				}
			}
			rectangle1.Width = Math.Max(rectangle1.Width, this.parentGridSize.Width);
			rectangle1.Height = Math.Max(rectangle1.Height, this.parentGridSize.Height);
			return rectangle1;
		}
		private void InitiateResize() {
			bool flag1 = true;
			ArrayList list1 = new ArrayList();
			IDesignerHost host1 = this.serviceProvider.DesignerHost;
			for(int num1 = 0; num1 < this.resizeComponents.Length; num1++) {
				this.resizeComponents[num1].resizeBounds = ((Control)this.resizeComponents[num1].resizeControl).Bounds;
				if(host1 != null) {
					ControlDesigner designer1 = host1.GetDesigner(this.resizeComponents[num1].resizeControl as Component) as ControlDesigner;
					if(designer1 != null) {
						this.resizeComponents[num1].resizeRules = designer1.SelectionRules;
					}
					else {
						this.resizeComponents[num1].resizeRules = SelectionRules.None;
					}
				}
			}
			BehaviorServiceAdornerCollectionEnumerator enumerator1 = this.BehaviorService.Adorners.GetEnumerator();
			try {
				while(enumerator1.MoveNext()) {
					enumerator1.Current.Enabled = false;
				}
			}
			finally {
				IDisposable disposable1 = enumerator1 as IDisposable;
				if(disposable1 != null) {
					disposable1.Dispose();
				}
			}
			IDesignerHost host2 = serviceProvider.DesignerHost;
			if(host2 != null) {
				string text1;
				if(this.resizeComponents.Length == 1) {
					string text2 = TypeDescriptor.GetComponentName(this.resizeComponents[0].resizeControl);
					if((text2 == null) || (text2.Length == 0)) {
						text2 = this.resizeComponents[0].resizeControl.GetType().Name;
					}
					text1 = "BehaviorServiceResizeControl";
				}
				else {
					text1 = "BehaviorServiceResizeControls";
				}
				this.resizeTransaction = host2.CreateTransaction(text1);
				serviceProvider.ResizeStarted();
			}
			this.initialResize = true;
			if(flag1) {
				this.dragManager = new FakeDragAssistanceManager(this.serviceProvider, list1, true);
			}
			else if(this.resizeComponents.Length > 0) {
				Control control1 = this.resizeComponents[0].resizeControl as Control;
				if((control1 != null) && (control1.Parent != null)) {
					PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(control1.Parent)["SnapToGrid"];
					if((descriptor1 != null) && ((bool)descriptor1.GetValue(control1.Parent))) {
						PropertyDescriptor descriptor2 = TypeDescriptor.GetProperties(control1.Parent)["GridSize"];
						if(descriptor2 != null) {
							this.parentGridSize = (Size)descriptor2.GetValue(control1.Parent);
							this.parentLocation = this.behaviorService.ControlToAdornerWindow(control1);
							this.parentLocation.X -= control1.Location.X;
							this.parentLocation.Y -= control1.Location.Y;
						}
					}
				}
			}
			this.graphics = this.BehaviorService.AdornerWindowGraphics;
		}
		public override void OnLoseCapture(Glyph g, EventArgs e) {
			if(this.pushedBehavior) {
				this.pushedBehavior = false;
				if(this.BehaviorService != null) {
					if(this.dragging) {
						this.dragging = false;
						for(int num1 = 0; (this.graphics != null) && (num1 < this.resizeComponents.Length); num1++) {
							Control control1 = this.resizeComponents[num1].resizeControl as Control;
							Rectangle rectangle1 = this.BehaviorService.ControlRectInAdornerWindow(control1);
							if(!rectangle1.IsEmpty) {
								this.graphics.SetClip(rectangle1);
								using(Region region1 = new Region(rectangle1)) {
									region1.Exclude(Rectangle.Inflate(rectangle1, -2, -2));
									this.BehaviorService.Invalidate(region1);
								}
								this.graphics.ResetClip();
							}
						}
						BehaviorServiceAdornerCollectionEnumerator enumerator1 = this.BehaviorService.Adorners.GetEnumerator();
						try {
							while(enumerator1.MoveNext()) {
								enumerator1.Current.Enabled = true;
							}
						}
						finally {
							IDisposable disposable1 = enumerator1 as IDisposable;
							if(disposable1 != null) {
								disposable1.Dispose();
							}
						}
					}
					this.BehaviorService.PopBehavior(this);
					if(this.lastResizeRegion != null) {
						this.BehaviorService.Invalidate(this.lastResizeRegion);
						this.lastResizeRegion.Dispose();
						this.lastResizeRegion = null;
					}
				}
				if(this.graphics != null) {
					this.graphics.Dispose();
					this.graphics = null;
				}
			}
			if(primaryControl != null) {
				ArrayList list = new ArrayList();
				for(int i = 0; i < resizeComponents.Length; i++) {
					list.Add(((ResizeComponent)resizeComponents[i]).resizeControl);
				}
				serviceProvider.SelectionService.SetSelectedComponents(list);
			}
			if(this.resizeTransaction != null) {
				serviceProvider.ResizeFinished();
				DesignerTransaction transaction1 = this.resizeTransaction;
				this.resizeTransaction = null;
				using(DesignerTransaction transaction2 = transaction1) {
					transaction1.Cancel();
				}
			}
		}
		public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc) {
			if(button != MouseButtons.Left) {
				return this.pushedBehavior;
			}
			this.targetResizeRules = SelectionRules.None;
			SelectionGlyphBase base1 = g as SelectionGlyphBase;
			if(base1 != null) {
				this.targetResizeRules = base1.SelectionRules;
				this.cursor = base1.HitTestCursor;
			}
			if(this.targetResizeRules != SelectionRules.None) {
				ISelectionService service1 = serviceProvider.SelectionService;
				if(service1 == null) {
					return false;
				}
				this.initialPoint = mouseLoc;
				this.lastMouseLoc = mouseLoc;
				this.primaryControl = service1.PrimarySelection as Control;
				ArrayList list1 = new ArrayList();
				foreach(object obj1 in service1.GetSelectedComponents()) {
					if(!(obj1 is Control)) {
						continue;
					}
					PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(obj1)["Locked"];
					if((descriptor1 == null) || !((bool)descriptor1.GetValue(obj1))) {
						list1.Add(obj1);
					}
				}
				if(list1.Count == 0) {
					return false;
				}
				this.resizeComponents = new ResizeBehavior.ResizeComponent[list1.Count];
				for(int num1 = 0; num1 < list1.Count; num1++) {
					this.resizeComponents[num1].resizeControl = list1[num1];
				}
				this.pushedBehavior = true;
				this.BehaviorService.PushCaptureBehavior(this);
			}
			return false;
		}
		private void GetDescriptors(out PropertyDescriptor descriptorWidth, out PropertyDescriptor descriptorHeight, out PropertyDescriptor descriptorTop, out PropertyDescriptor descriptorLeft) {
			descriptorWidth = null;
			descriptorHeight = null;
			descriptorTop = null;
			descriptorLeft = null;
			if(this.initialResize) {
				descriptorWidth = TypeDescriptor.GetProperties(this.resizeComponents[0].resizeControl)["Width"];
				descriptorHeight = TypeDescriptor.GetProperties(this.resizeComponents[0].resizeControl)["Height"];
				descriptorTop = TypeDescriptor.GetProperties(this.resizeComponents[0].resizeControl)["Top"];
				descriptorLeft = TypeDescriptor.GetProperties(this.resizeComponents[0].resizeControl)["Left"];
				if((descriptorWidth != null) && !typeof(int).IsAssignableFrom(descriptorWidth.PropertyType)) {
					descriptorWidth = null;
				}
				if((descriptorHeight != null) && !typeof(int).IsAssignableFrom(descriptorHeight.PropertyType)) {
					descriptorHeight = null;
				}
				if((descriptorTop != null) && !typeof(int).IsAssignableFrom(descriptorTop.PropertyType)) {
					descriptorTop = null;
				}
				if((descriptorLeft != null) && !typeof(int).IsAssignableFrom(descriptorLeft.PropertyType)) {
					descriptorLeft = null;
				}
			}
		}
		private void ApplyBoundsToControlViaPropertyDescriptors(PropertyDescriptor descriptorWidth, PropertyDescriptor descriptorHeight, PropertyDescriptor descriptorTop, PropertyDescriptor descriptorLeft, int num3, Rectangle bounds, BoundsSpecified boundsSpecified) {
			return;
		}
		private void FinalizeMove(Rectangle rectangle2, Control control2, Rectangle rectangle5, Rectangle rectangle7, bool flag5) {
			UnsafeNativeMethods.SendMessage(control2.Handle, 11, true, 0);
			if(flag5) {
				Control control3 = control2.Parent;
				if(control3 != null) {
					control2.Invalidate(true);
					control3.Invalidate(rectangle5, true);
					control3.Update();
				}
				else {
					control2.Refresh();
				}
			}
			if(!rectangle2.IsEmpty) {
				using(Region region1 = new Region(rectangle2)) {
					region1.Exclude(Rectangle.Inflate(rectangle2, -2, -2));
					if(flag5) {
						using(Region region2 = new Region(rectangle7)) {
							region2.Exclude(Rectangle.Inflate(rectangle7, -2, -2));
							this.BehaviorService.Invalidate(region2);
						}
					}
					if(this.graphics != null) {
						if((this.lastResizeRegion != null) && !this.lastResizeRegion.Equals(region1, this.graphics)) {
							this.lastResizeRegion.Exclude(region1);
							this.BehaviorService.Invalidate(this.lastResizeRegion);
							this.lastResizeRegion.Dispose();
							this.lastResizeRegion = null;
						}
						if(this.lastResizeRegion == null) {
							this.lastResizeRegion = region1.Clone();
						}
					}
				}
			}
		}
		public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLocation) {
			if(!this.pushedBehavior) {
				return false;
			}
			bool isAltKeyPressed = Control.ModifierKeys == Keys.Alt;
			if(isAltKeyPressed && (this.dragManager != null)) {
				this.dragManager.EraseSnapLines();
			}
			if(isAltKeyPressed || !mouseLocation.Equals(this.lastMouseLoc)) {
				if(this.lastMouseAbs != null) {
					NativeMethods.POINT currentMouseLocation = GetCurrentMouseLocation(ref mouseLocation);
					if((currentMouseLocation.x == this.lastMouseAbs.x) && (currentMouseLocation.y == this.lastMouseAbs.y)) {
						return true;
					}
				}
				if(!this.dragging) {
					if((Math.Abs((int)(this.initialPoint.X - mouseLocation.X)) > (DesignerUtils.MinDragSize.Width / 2)) || (Math.Abs((int)(this.initialPoint.Y - mouseLocation.Y)) > (DesignerUtils.MinDragSize.Height / 2))) {
						this.InitiateResize();
						this.dragging = true;
					}
					else {
						return false;
					}
				}
				if((this.resizeComponents == null) || (this.resizeComponents.Length == 0)) {
					return false;
				}
				PropertyDescriptor descriptorWidth;
				PropertyDescriptor descriptorHeight;
				PropertyDescriptor descriptorTop;
				PropertyDescriptor descriptorLeft;
				GetDescriptors(out descriptorWidth, out descriptorHeight, out descriptorTop, out descriptorLeft);
				Control control1 = this.resizeComponents[0].resizeControl as Control;
				this.lastMouseLoc = mouseLocation;
				this.lastMouseAbs = new NativeMethods.POINT(mouseLocation.X, mouseLocation.Y);
				Point resultPoint = this.behaviorService.AdornerWindowPointToScreen(new Point(lastMouseAbs.x, lastMouseAbs.y));
				lastMouseAbs.x = resultPoint.X;
				lastMouseAbs.y = resultPoint.Y;
				int num1 = Math.Max(control1.MinimumSize.Height, 10);
				int num2 = Math.Max(control1.MinimumSize.Width, 10);
				if(this.dragManager != null) {
					mouseLocation = GetMouseLocationFromDragManager(mouseLocation, isAltKeyPressed, control1, num1, num2);
				}
				Rectangle rectangle1 = new Rectangle(this.resizeComponents[0].resizeBounds.X, this.resizeComponents[0].resizeBounds.Y, this.resizeComponents[0].resizeBounds.Width, this.resizeComponents[0].resizeBounds.Height);
				if(this.didSnap && (control1.Parent != null)) {
					rectangle1.Location = this.behaviorService.MapAdornerWindowPoint(control1.Parent.Handle, rectangle1.Location);
					if(control1.Parent.IsMirrored) {
						rectangle1.Offset(-rectangle1.Width, 0);
					}
				}
				Rectangle rectangle2 = Rectangle.Empty;
				Rectangle rectangle3 = Rectangle.Empty;
				Color color1 = (control1.Parent != null) ? control1.Parent.BackColor : Color.Empty;
				for(int i = 0; i < this.resizeComponents.Length; i++) {
					Control currentResizedControl = this.resizeComponents[i].resizeControl as Control;
					Rectangle currentResizedControlBounds = currentResizedControl.Bounds;
					Rectangle currentResizedControlBoundsDup = currentResizedControlBounds;
					Rectangle currentResizeBounds = this.resizeComponents[i].resizeBounds;
					Rectangle controlrectInAdronerWindow = this.BehaviorService.ControlRectInAdornerWindow(currentResizedControl);
					bool flag5 = true;
					UnsafeNativeMethods.SendMessage(currentResizedControl.Handle, 11, false, 0);
					try {
						bool IsParentMirrored = false;
						if((currentResizedControl.Parent != null) && currentResizedControl.Parent.IsMirrored) {
							IsParentMirrored = true;
						}
						BoundsSpecified boundsSpectifirdTemp = BoundsSpecified.None;
						SelectionRules currentResizedControlRules = this.resizeComponents[i].resizeRules;
						if(((this.targetResizeRules & SelectionRules.BottomSizeable) != SelectionRules.None) && ((currentResizedControlRules & SelectionRules.BottomSizeable) != SelectionRules.None)) {
							int num4;
							if(this.didSnap) {
								num4 = mouseLocation.Y - rectangle1.Bottom;
							}
							else {
								num4 = ResizeBehavior.AdjustPixelsForIntegralHeight(currentResizedControl, mouseLocation.Y - this.initialPoint.Y);
							}
							currentResizedControlBounds.Height = Math.Max(num1, currentResizeBounds.Height + num4);
							boundsSpectifirdTemp |= BoundsSpecified.Height;
						}
						if(((this.targetResizeRules & SelectionRules.TopSizeable) != SelectionRules.None) && ((currentResizedControlRules & SelectionRules.TopSizeable) != SelectionRules.None)) {
							int diffY;
							if(this.didSnap) {
								diffY = rectangle1.Y - mouseLocation.Y;
							}
							else {
								diffY = ResizeBehavior.AdjustPixelsForIntegralHeight(currentResizedControl, this.initialPoint.Y - mouseLocation.Y);
							}
							boundsSpectifirdTemp |= BoundsSpecified.Height;
							currentResizedControlBounds.Height = Math.Max(num1, currentResizeBounds.Height + diffY);
							if((currentResizedControlBounds.Height != num1) || ((currentResizedControlBounds.Height == num1) && (currentResizedControlBoundsDup.Height != num1))) {
								boundsSpectifirdTemp |= BoundsSpecified.Y;
								currentResizedControlBounds.Y = Math.Min((int)(currentResizeBounds.Bottom - num1), (int)(currentResizeBounds.Y - diffY));
							}
						}
						if(((((this.targetResizeRules & SelectionRules.RightSizeable) != SelectionRules.None) && ((currentResizedControlRules & SelectionRules.RightSizeable) != SelectionRules.None)) && !IsParentMirrored) || ((((this.targetResizeRules & SelectionRules.LeftSizeable) != SelectionRules.None) && ((currentResizedControlRules & SelectionRules.LeftSizeable) != SelectionRules.None)) && IsParentMirrored)) {
							boundsSpectifirdTemp |= BoundsSpecified.Width;
							int num6 = this.initialPoint.X;
							if(this.didSnap) {
								num6 = !IsParentMirrored ? rectangle1.Right : rectangle1.Left;
							}
							currentResizedControlBounds.Width = Math.Max(num2, currentResizeBounds.Width + (!IsParentMirrored ? (mouseLocation.X - num6) : (num6 - mouseLocation.X)));
						}
						if(((((this.targetResizeRules & SelectionRules.RightSizeable) != SelectionRules.None) && ((currentResizedControlRules & SelectionRules.RightSizeable) != SelectionRules.None)) && IsParentMirrored) || ((((this.targetResizeRules & SelectionRules.LeftSizeable) != SelectionRules.None) && ((currentResizedControlRules & SelectionRules.LeftSizeable) != SelectionRules.None)) && !IsParentMirrored)) {
							boundsSpectifirdTemp |= BoundsSpecified.Width;
							int num7 = this.initialPoint.X;
							if(this.didSnap) {
								num7 = !IsParentMirrored ? rectangle1.Left : rectangle1.Right;
							}
							int num8 = !IsParentMirrored ? (num7 - mouseLocation.X) : (mouseLocation.X - num7);
							currentResizedControlBounds.Width = Math.Max(num2, currentResizeBounds.Width + num8);
							if((currentResizedControlBounds.Width != num2) || ((currentResizedControlBounds.Width == num2) && (currentResizedControlBoundsDup.Width != num2))) {
								boundsSpectifirdTemp |= BoundsSpecified.X;
								currentResizedControlBounds.X = Math.Min((int)(currentResizeBounds.Right - num2), (int)(currentResizeBounds.X - num8));
							}
						}
						if(!this.parentGridSize.IsEmpty) {
							currentResizedControlBounds = this.AdjustToGrid(currentResizedControlBounds, this.targetResizeRules);
						}
						ApplyBoundsToControlViaPropertyDescriptors(descriptorWidth, descriptorHeight, descriptorTop, descriptorLeft, i, currentResizedControlBounds, boundsSpectifirdTemp);
						if(this.dragging) {
							currentResizedControl.SetBounds(currentResizedControlBounds.X, currentResizedControlBounds.Y, currentResizedControlBounds.Width, currentResizedControlBounds.Height, boundsSpectifirdTemp);
							rectangle2 = this.BehaviorService.ControlRectInAdornerWindow(currentResizedControl);
							if(currentResizedControl.Equals(control1)) {
								rectangle3 = rectangle2;
							}
							if(currentResizedControl.Bounds == currentResizedControlBoundsDup) {
								flag5 = false;
							}
							if(currentResizedControl.Bounds != currentResizedControlBounds) {
							}
						}
					}
					finally {
						FinalizeMove(rectangle2, currentResizedControl, currentResizedControlBoundsDup, controlrectInAdronerWindow, flag5);
					}
				}
				this.initialResize = false;
			}
			return true;
		}
		private Point GetMouseLocationFromDragManager(Point mouseLocation, bool isAltKeyPressed, Control control1, int num1, int num2) {
			bool flag2 = true;
			if((((this.targetResizeRules & SelectionRules.BottomSizeable) != SelectionRules.None) || ((this.targetResizeRules & SelectionRules.TopSizeable) != SelectionRules.None)) && (control1.Height == num1)) {
				flag2 = false;
			} else if((((this.targetResizeRules & SelectionRules.RightSizeable) != SelectionRules.None) || ((this.targetResizeRules & SelectionRules.LeftSizeable) != SelectionRules.None)) && (control1.Width == num2)) {
				flag2 = false;
			}
			if(!isAltKeyPressed && flag2) {
				this.lastSnapOffset = this.dragManager.OnMouseMove();
			} else {
				this.dragManager.OnMouseMove(new Rectangle(-100, -100, 0, 0));
			}
			mouseLocation.X += this.lastSnapOffset.X;
			mouseLocation.Y += this.lastSnapOffset.Y;
			return mouseLocation;
		}
		private NativeMethods.POINT GetCurrentMouseLocation(ref Point mouseLocation) {
			NativeMethods.POINT currentMouseLocation = new NativeMethods.POINT(mouseLocation.X, mouseLocation.Y);
			Point result = this.behaviorService.AdornerWindowPointToScreen(new Point(currentMouseLocation.x, currentMouseLocation.y));
			currentMouseLocation.x = result.X;
			currentMouseLocation.y = result.Y;
			return currentMouseLocation;
		}
		public override bool OnMouseUp(Glyph g, MouseButtons button) {
			try {
				if(this.dragging) {
					if(this.dragManager != null) {
						this.dragManager.OnMouseUp();
						this.dragManager = null;
						this.lastSnapOffset = Point.Empty;
						this.didSnap = false;
					}
					if((this.resizeComponents != null) && (this.resizeComponents.Length > 0)) {
						PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(this.resizeComponents[0].resizeControl)["Width"];
						PropertyDescriptor descriptor2 = TypeDescriptor.GetProperties(this.resizeComponents[0].resizeControl)["Height"];
						PropertyDescriptor descriptor3 = TypeDescriptor.GetProperties(this.resizeComponents[0].resizeControl)["Top"];
						PropertyDescriptor descriptor4 = TypeDescriptor.GetProperties(this.resizeComponents[0].resizeControl)["Left"];
						for(int num1 = 0; num1 < this.resizeComponents.Length; num1++) {
							if((descriptor1 != null) && (((Control)this.resizeComponents[num1].resizeControl).Width != this.resizeComponents[num1].resizeBounds.Width)) {
								descriptor1.SetValue(this.resizeComponents[num1].resizeControl, ((Control)this.resizeComponents[num1].resizeControl).Width);
							}
							if((descriptor2 != null) && (((Control)this.resizeComponents[num1].resizeControl).Height != this.resizeComponents[num1].resizeBounds.Height)) {
								descriptor2.SetValue(this.resizeComponents[num1].resizeControl, ((Control)this.resizeComponents[num1].resizeControl).Height);
							}
							if((descriptor3 != null) && (((Control)this.resizeComponents[num1].resizeControl).Top != this.resizeComponents[num1].resizeBounds.Y)) {
								descriptor3.SetValue(this.resizeComponents[num1].resizeControl, ((Control)this.resizeComponents[num1].resizeControl).Top);
							}
							if((descriptor4 != null) && (((Control)this.resizeComponents[num1].resizeControl).Left != this.resizeComponents[num1].resizeBounds.X)) {
								descriptor4.SetValue(this.resizeComponents[num1].resizeControl, ((Control)this.resizeComponents[num1].resizeControl).Left);
							}
						}
					}
				}
				if(this.resizeTransaction != null) {
					serviceProvider.ResizeFinished();
					DesignerTransaction transaction1 = this.resizeTransaction;
					this.resizeTransaction = null;
					using(DesignerTransaction transaction2 = transaction1) {
						transaction1.Commit();
					}
				}
			}
			finally {
				this.OnLoseCapture(g, EventArgs.Empty);
			}
			return false;
		}
		private BehaviorService BehaviorService {
			get {
				if(this.behaviorService == null) {
					this.behaviorService = serviceProvider.BehaviorServiceCore;
				}
				return this.behaviorService;
			}
		}
		public override Cursor Cursor {
			get {
				return this.cursor;
			}
		}
		private struct ResizeComponent {
			public object resizeControl;
			public Rectangle resizeBounds;
			public SelectionRules resizeRules;
		}
	}
}
#endif
