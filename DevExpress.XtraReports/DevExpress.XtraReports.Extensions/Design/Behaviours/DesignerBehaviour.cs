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
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.SnapLines;
using DevExpress.XtraReports.Native;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Design.Behaviours {
	public interface IDesignerBehaviour {
		void AdornControl(ControlDrawEventArgs e);
		void SetDefaultComponentBounds();
		void ProcessGrabHandle();
	}
	public class DesignerBehaviour : AdapterBase, IDesignerBehaviour {
		XRControlDesigner designer;
		XRControl xrControl;
		static readonly object padlock = new object();
		protected XRControl XRControl {
			get {
				return xrControl;
			}
		}
		protected XRControlDesigner Designer {
			get {
				return designer;
			}
		}
		public DesignerBehaviour(IServiceProvider servProvider)
			: base(servProvider) {
		}
		public virtual void SetCurrentDesigner(XRControlDesigner designer) {
			this.designer = designer;
			this.xrControl = designer.XRControl;
		}
		public void ProcessGrabHandle() {
			try {
				lock(padlock) {
					IBandViewInfoService svc = servProvider.GetService(typeof(IBandViewInfoService)) as IBandViewInfoService;
					if(svc != null) {
						IntPtr handle = DevExpress.Utils.Drawing.Helpers.NativeMethods.WindowFromPoint(Control.MousePosition);
						if(svc.View.Handle != handle) return;
					}
					ProcessGrabHandleCore(Control.MousePosition, Control.MouseButtons);
				}
			} catch { }
		}
		void ProcessGrabHandleCore(Point screenPoint, MouseButtons buttons) {
			FrameSelectionUIService selUISrv = (FrameSelectionUIService)GetService(typeof(FrameSelectionUIService));
			ISelectionService selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if(selectionService.GetComponentSelected(XRControl) || !buttons.IsNone()) {
				selUISrv.HideGrabHandle(XRControl);
				return;
			}
			if(CanShowGrabHandle(screenPoint) && buttons.IsNone()) {
				selUISrv.ShowGrabHandle(XRControl);
			} else if(!selUISrv.GrabContainsPoint(screenPoint)) {
				selUISrv.HideGrabHandle(XRControl);
			}
		}
		protected virtual bool CanShowGrabHandle(Point screenPoint) {
			XRControl control = BandViewSvc.GetControlByScreenPoint(screenPoint);
			return XRControl.IsOwnerOf(control);
		}
		public virtual void AdornControl(ControlDrawEventArgs e) {
		   if(e.ShouldDrawReportExplorerImage)
			   DrawImage(e.Graph, ReportExplorerImage);
		   else if(XRControl.HasBindings())
				DrawImage(e.Graph, ReportExplorerController.BoundedImage);
		}
		Image ReportExplorerImage {
			get {
				int index = ReportExplorerController.GetImageIndex(XRControl, XRControl.HasBindings());
				return ReportExplorerController.ImageCollection.Images[index];
			}
		}
		protected void DrawImage(Graphics gr, Image image) {
			Region savedClipRegion = gr.Clip;
			GraphicsUnit savedPageUnit = gr.PageUnit;
			Matrix savedTransform = gr.Transform;
			RectangleF viewClientRect = BandViewSvc.GetControlViewClientBounds(XRControl);
			using(Region clipRegion = new Region(viewClientRect)) {
				gr.ResetTransform();
				gr.PageUnit = GraphicsUnit.Pixel;
				gr.Clip = clipRegion;
				try {
					gr.DrawImage(image, (int)Math.Round(viewClientRect.Right - image.Width - 2), (int)Math.Round(viewClientRect.Top + 2));
				} finally {
					gr.MultiplyTransform(savedTransform);
					gr.PageUnit = savedPageUnit;
					gr.Clip = savedClipRegion;
				}
			}
		}
		public virtual void SetDefaultComponentBounds() {
			if(xrControl.Parent == null)
				return;
			RectangleF bounds = GetControlBounds(XRControl.Parent, XRControl.BoundsF, CapturePaintService.GetDragBounds(servProvider), false);
			if(bounds == XRControl.BoundsF)
				return;
			IComponentChangeService changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			changeService.OnComponentChanging(XRControl, null);
			XRControl.BoundsF = bounds;
			changeService.OnComponentChanged(XRControl, null, null, null);
		}
		protected RectangleF GetControlBounds(XRControl parent, RectangleF defaultBounds, RectangleF screenBounds, bool limitParentBounds) {
			RectangleF bounds = defaultBounds;
			if(!RectHelper.RectangleFIsEmpty(screenBounds, 0.001)) {
				RectangleF parentBounds = BandViewSvc.GetControlScreenBounds(parent);
				RectangleF rect = limitParentBounds ? RectangleF.Intersect(parentBounds, screenBounds) : screenBounds;
				rect.Offset(-parentBounds.X, -parentBounds.Y);
				bounds = ZoomService.FromScaledPixels(rect, XRControl.Dpi);
				if(bounds.Size.IsEmpty) {
					bounds.Size = defaultBounds.Size;
				}
				if(Designer.ShouldSnapBounds && SnapLineHelper.IsSnappingAllowed)
					bounds = GetSnappedBounds(bounds, RootReport.GridSizeF);
			} else {
				if(Designer.ShouldSnapBounds && SnapLineHelper.IsSnappingAllowed)
					bounds.Location = GetSnappedBounds(bounds, RootReport.GridSizeF).Location;
			}
			return bounds;
		}
		static RectangleF GetSnappedBounds(RectangleF r, SizeF gridSize) {
			r.Location = Divider.GetDivisibleValue(r.Location, gridSize);
			r.Size = Divider.GetDivisibleValue(r.Size, gridSize);
			r.Size = NativeMethods.GetMaxSize(r.Size, gridSize);
			return r;
		}
	}
}
