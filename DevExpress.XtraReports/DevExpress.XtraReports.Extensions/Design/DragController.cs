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
using System.Drawing;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Design {
	public class DragDataObject {
		RectangleF primaryRect;
		XRControl[] controls;
		RectangleF[] controlRects; 
		Point startPos;
		XRControl primaryControl;
		public XRControl PrimaryControl { get { return primaryControl; } 
		}
		public XRControl[] Controls { get { return controls; }
		}
		public RectangleF PrimaryRect { get { return primaryRect; } 
		}
		public RectangleF[] ControlRects { get { return controlRects; }
		}
		public DragDataObject(XRControl[] controls, XRControl primaryControl, RectangleF[] controlRects, RectangleF primaryRect) : this(controls, primaryControl, controlRects, primaryRect, Control.MousePosition) { }
		public DragDataObject(XRControl[] controls, XRControl primaryControl, RectangleF[] controlRects, RectangleF primaryRect, Point startPoint) {
			this.primaryControl = primaryControl;
			this.controls = controls;
			this.controlRects = controlRects;
			this.primaryRect = primaryRect;
			this.startPos = startPoint;
		}
		public virtual PointF EvalBasePoint(float x, float y) {
			PointF pt = primaryRect.Location;
			pt.X += x - startPos.X;
			pt.Y += y - startPos.Y;
			return pt;
		}
		public PointF GetDiffPoint(float x, float y) {
			return new PointF(x - startPos.X, y - startPos.Y);
		}
		public bool ControlsContainRotatable(IDesignerHost host) {
			foreach(XRControl control in controls) {
				if(XRControlDesigner.IsControlRotatable(control, host))
					return true;
			}
			return false;
		}
	}
	public class DragController : IDisposable {
		#region static
		static Rectangle GetDragRectangle(Point pt) {
			Size dragSize = SystemInformation.DragSize;
			return new Rectangle(new Point(pt.X - dragSize.Width / 2,
				pt.Y - dragSize.Height / 2), dragSize);
		}
		static bool CanStartDrag() {
			return XRControlDesignerBase.CursorEquals(Cursors.SizeAll) &&
				Control.MouseButtons.IsLeft() &&
				(Control.ModifierKeys == Keys.None || Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt);
		}
		#endregion
		IDesignerHost host;
		Control control;
		Rectangle dragBox;
		ISelectionService selSvc;
		Point startDragPoint;
		protected ISelectionService SelectionSvc {
			get {
				if(selSvc == null) {
					selSvc = (ISelectionService)host.GetService( typeof(ISelectionService) );
				}
				return selSvc;
			}
		}
		public DragController(IDesignerHost host, Control control) {
			this.host = host;
			this.control = control;
			control.MouseDown += new MouseEventHandler(OnMouseDown);
			control.MouseMove += new MouseEventHandler(OnMouseMove);
			control.MouseUp += new MouseEventHandler(OnMouseUp);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected void Dispose(bool disposing) {
			if(disposing) {
				control.MouseDown -= new MouseEventHandler(OnMouseDown);
				control.MouseMove -= new MouseEventHandler(OnMouseMove);
				control.MouseUp -= new MouseEventHandler(OnMouseUp);
			}
		}
		~DragController() {
			Dispose(false);
		}
		private void OnMouseUp(object sender, MouseEventArgs e) {
			ResetDragBox();
		}
		private void OnMouseDown(object sender, MouseEventArgs e) {
			if(CanStartDrag()) {
				startDragPoint = control.PointToScreen(new Point(e.X, e.Y));
				SetDragBox(new Point(e.X, e.Y));				
			} else 
				ResetDragBox();
		}
		private void SetDragBox(Point pos) {
			System.Diagnostics.Debug.Assert( !pos.IsEmpty );
			dragBox = GetDragRectangle(pos);
		}
		private void ResetDragBox() {
			dragBox = Rectangle.Empty;
		}		
		private void OnMouseMove(object sender, MouseEventArgs e) { 
			if(!e.Button.IsLeft() || dragBox.IsEmpty || dragBox.Contains(e.X, e.Y))
				return;
			ResetDragBox();
			DoStartDrag();
		}
		private void DoStartDrag() {
			DragDataObject dragData = CreateDragDataObject();
			if(CanDragDrop(dragData))
				control.DoDragDrop(new DataObject(dragData), DragDropEffects.Move | DragDropEffects.Copy);
		}
		bool CanDragDrop(DragDataObject dragData) {
			if (dragData == null)
				return false;
			foreach (XRControl control in dragData.Controls) {
				XRControlDesigner designer = this.host.GetDesigner(control) as XRControlDesigner;
				if(designer != null && !designer.CanDrag)
					return false;
			}
			return true;
		}
		private DragDataObject CreateDragDataObject() {
			object selection = SelectionSvc.PrimarySelection;
			FakeComponent fakeComponent = selection as FakeComponent;
			if (fakeComponent != null)
				selection = fakeComponent.Parent;
			XRControl primaryControl = FrameSelectionUIService.GetAsXRControl(host, selection);
			if(primaryControl is XRTableCell || primaryControl is XRTableRow)
				return null;
			if(primaryControl == null || (XRControlDesigner.IsControlRotatable(primaryControl, host) && Control.ModifierKeys == Keys.Control) || primaryControl is Band) 
				return null;
			RectangleF primaryRect = GetScreenBounds(primaryControl);
			XRControl[] selectControls = XRControlDesigner.GetControlsFrom(host, SelectionSvc.GetSelectedComponents());
			XRControl[] controls = ArrayHelper.Filter<XRControl>(selectControls, control => { return control.Parent == primaryControl.Parent; });
			RectangleF[] controlBounds = GetRelativeBounds(primaryControl, controls);
			return new DragDataObject(controls, primaryControl, controlBounds, primaryRect, startDragPoint);
		}
		protected RectangleF GetScreenBounds(XRControl xrControl) {
			IBandViewInfoService bandViewSvc = (IBandViewInfoService)host.GetService(typeof(IBandViewInfoService)); 
			System.Diagnostics.Debug.Assert(bandViewSvc != null);
			return bandViewSvc.GetControlScreenBounds(xrControl);
		}
		private RectangleF[] GetRelativeBounds(XRControl baseControl, XRControl[] controls) {
			List<RectangleF> list = new List<RectangleF>();
			RectangleF baseRect = GetScreenBounds(baseControl);
			foreach(XRControl item in controls) {
				RectangleF rect = GetScreenBounds(item);
				rect.Offset(-baseRect.X, -baseRect.Y);
				list.Add(rect);
			}
			return list.ToArray();
		}
	}
}
