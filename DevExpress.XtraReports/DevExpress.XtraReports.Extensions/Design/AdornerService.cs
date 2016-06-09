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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Native;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.SnapLines;
using DevExpress.Utils.Drawing;
using DevExpress.XtraReports.Design.Adapters;
namespace DevExpress.XtraReports.Design {
	public class AdornerService {
		#region internal classes
		class InvalidationHelper {
			AdornerService adornerService;
			RectangleF rect1;
			public InvalidationHelper(AdornerService adornerService) {
				this.adornerService = adornerService;
			}
			public void BeginUpdate() {
				rect1 = GetUnionRect(GetAllRects());
			}
			public void EndUpdate() {
				RectangleF rect2 = GetUnionRect(GetAllRects());
				adornerService.BandViewInfoService.Invalidate(RectangleF.Inflate(SmartUnionRects(rect1, rect2), 1 , 1));
			}
			static RectangleF SmartUnionRects(RectangleF rect1, RectangleF rect2) {
				if(rect1 == RectangleF.Empty)
					return rect2;
				if(rect2 == RectangleF.Empty)
					return rect1;
				return RectangleF.Union(rect1, rect2);
			}
			RectangleF[] GetAllRects() {
				List<RectangleF> rects = new List<RectangleF>();
				if(adornerService.drawRects != null) {
					rects.AddRange(adornerService.drawRects);
				}
				if(adornerService.snapLines != null) {
					foreach(SnapLine snapLine in adornerService.snapLines) {
						rects.Add(snapLine.Bounds);
					}
				}
				return rects.ToArray();
			}
			static RectangleF GetUnionRect(RectangleF[] rects) {
				RectangleF rect = RectangleF.Empty;
				if(rects != null) {
					for(int i = 0; i < rects.Length; i++) {
						rect = i != 0 ? RectangleF.Union(rect, rects[i]) : rects[i];
					}
				}
				return rect;
			}
		}
		#endregion
		SnapLine[] snapLines;
		RectangleF[] drawRects;
		IServiceProvider serviceProvider;
		IBandViewInfoService bandViewInfoService;
		ISelectionService selectionService;
		IDesignerHost designerHost;
		ZoomService zoomService;
		InvalidationHelper invalidationHelper;
		public RectangleF[] DrawRects {
			get { return drawRects; }
		}
		public AdornerService(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			invalidationHelper = new InvalidationHelper(this);
		}
		IBandViewInfoService BandViewInfoService {
			get {
				if(bandViewInfoService == null)
					bandViewInfoService = (IBandViewInfoService)this.serviceProvider.GetService(typeof(IBandViewInfoService));
				return bandViewInfoService;
			}
		}
		ISelectionService SelectionService {
			get {
				if(selectionService == null)
					selectionService = (ISelectionService)this.serviceProvider.GetService(typeof(ISelectionService));
				return selectionService;
			}
		}
		XRControl PrimarySelection {
			get {
				if(SelectionService.PrimarySelection is XRControl)
					return (XRControl)SelectionService.PrimarySelection;
				ResizeService resizeService = (ResizeService)serviceProvider.GetService(typeof(ResizeService));
				XRControl[] selectedControls = resizeService.GetSelectedControls();
				return selectedControls[0];
			}
		}
		IDesignerHost DesignerHost {
			get {
				if(designerHost == null)
					designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
				return designerHost;
			}
		}
		ZoomService ZoomService {
			get {
				if(zoomService == null)
					zoomService = (ZoomService)serviceProvider.GetService(typeof(ZoomService));
				return zoomService;
			}
		}
		public void OnPaint(GraphicsCache cache) {
			if(drawRects != null) {
				foreach(RectangleF drawRect in drawRects) {
					ControlPaintHelper.DrawRectangle(cache.Graphics, Brushes.Black, 1, drawRect);
				}
			}
			if(snapLines == null)
				return;
			foreach(SnapLine snapLine in snapLines) {
				RectangleF bounds = snapLine.Bounds;
				if(RectHelper.RectangleFIsEmptySize(bounds, 0.001))
					continue;
				if(snapLine.Kind == SnapLineKind.RightSide)
					bounds.Offset(-1, 0);
				else if(snapLine.Kind == SnapLineKind.BottomSide)
					bounds.Offset(0, -1);
				Brush brush = cache.GetSolidBrush(GetSnapLineColor(snapLine.Kind));
				cache.Graphics.FillRectangle(brush, bounds.X, bounds.Y, Math.Max(1, bounds.Width), Math.Max(1, bounds.Height));
			}
		}
		static Color GetSnapLineColor(SnapLineKind snapLineKind) {
			return snapLineKind == SnapLineKind.Margin ? Color.FromArgb(0x00, 0xcc, 0xff) : Color.FromArgb(0xff, 0x00, 0xf6);
		}
		public void DrawSnapLines(XRControl control) {
			DrawSnapLines(control, control.BoundsRelativeToBand, control.Band, new XRControl[] { });
		}
		public void DrawSnapLines(XRControl control, RectangleF controlRect, Band parentBand, XRControl[] controlsToExclude) { 
			DrawSnapLines(control, controlRect, parentBand, controlsToExclude, control.RootReport);
		}
		public void DrawSnapLines(XRControl control, RectangleF controlRect, Band parentBand, XRControl[] controlsToExclude, XtraReport rootReport) {
			if(control == null)
				return;
			invalidationHelper.BeginUpdate(); 
			snapLines = control.SupportSnapLines ?
				SnapLineHelper.GetSnapLines(control, controlRect, parentBand, serviceProvider, controlsToExclude, rootReport) :
				null;
			invalidationHelper.EndUpdate();
		}
		public void DrawScreenRects(RectangleF[] rects) {
			invalidationHelper.BeginUpdate();
			this.drawRects = ToClientRects(rects);
			invalidationHelper.EndUpdate();
		}
		RectangleF[] ToClientRects(RectangleF[] rects) {
			return Array.ConvertAll<RectangleF, RectangleF>(rects, delegate(RectangleF value) { return BandViewInfoService.RectangleFToClient(value); });
		}
		public void DrawControlsRects(SizeF deltaInPixels, SelectionRules selectionRules) {
			ResizeService resizeService = (ResizeService)serviceProvider.GetService(typeof(ResizeService));
			XRControl[] selectedControls = resizeService.GetSelectedControls();
			List<RectangleF> rects = new List<RectangleF>();
			SizeF delta = ZoomService.FromScaledPixels(deltaInPixels, PrimarySelection.Dpi);
			PointF pt = GetSnapOffset(PrimarySelection, delta, selectionRules);
			PointF ptInPixels = ZoomService.ToScaledPixels(pt, PrimarySelection.Dpi);
			invalidationHelper.BeginUpdate();
			foreach(XRControl selectedControl in selectedControls) {
				IBoundsAdapter adapter = BoundsAdapterService.GetAdapter(selectedControl, DesignerHost);
				RectangleF bounds = adapter.GetScreenBounds();
				bounds = BandViewInfoService.RectangleFToClient(bounds);
				RectangleF controlRect = GetControlRectangleForSnapLines(selectedControl, bounds, ptInPixels, deltaInPixels, selectionRules);
				rects.Add(controlRect);
			}
			this.drawRects = rects.ToArray();
			invalidationHelper.EndUpdate();
		}
		RectangleF GetControlRectangleForSnapLines(XRControl xrControl, RectangleF xrControlRect, PointF pt, SizeF delta, SelectionRules selectionRules) {
			SelectionRules selRules = ValidateSelectionRules(xrControl, selectionRules);
			Size minSize = ZoomService.ToScaledPixels(xrControl.GetMinSize(), xrControl.Dpi);
			RectangleF rect = ResizeService.ResizeRectangle(xrControlRect, selRules, delta, minSize);
			return CalculateRectangleForSnapLines(rect, pt, selRules);
		}
		SelectionRules ValidateSelectionRules(XRControl xrControl, SelectionRules selectionRules) {
			XRControlDesignerBase controlDesigner = (XRControlDesignerBase)DesignerHost.GetDesigner(xrControl);
			System.Diagnostics.Debug.Assert(controlDesigner != null);
			return selectionRules & controlDesigner.GetSelectionRules();
			}
		static RectangleF CalculateRectangleForSnapLines(RectangleF rect, PointF pt, SelectionRules selectionRules) {
			if(pt != Point.Empty) {
				float left = rect.Left, top = rect.Top, right = rect.Right, bottom = rect.Bottom;
				if((selectionRules & SelectionRules.LeftSizeable) != 0) {
					left += pt.X;
				}
				if((selectionRules & SelectionRules.TopSizeable) != 0) {
					top += pt.Y;
				}
				if((selectionRules & SelectionRules.RightSizeable) != 0) {
					right += pt.X;
				}
				if((selectionRules & SelectionRules.BottomSizeable) != 0) {
					bottom += pt.Y;
				}
				rect = RectangleF.FromLTRB(left, top, right, bottom);
			}
			return rect;
		}
		PointF GetSnapOffset(XRControl xrControl, SizeF delta, SelectionRules selectionRules) {
			XRControlDesignerBase controlDesigner = (XRControlDesignerBase)DesignerHost.GetDesigner(xrControl);
			System.Diagnostics.Debug.Assert(controlDesigner != null);
			SelectionRules selRules = selectionRules & controlDesigner.GetSelectionRules();
			RectangleF rect = ResizeService.ResizeRectangle(xrControl.BoundsRelativeToBand, selRules, delta, xrControl.GetMinSize());
			ResizeService resizeService = (ResizeService)serviceProvider.GetService(typeof(ResizeService));
			return SnapLineHelper.GetSnapOffset(xrControl, rect, xrControl.Band, resizeService.GetSelectedControls(), selRules);
		}
		public void ResetSnapping() {
			invalidationHelper.BeginUpdate();
			this.drawRects = null;
			this.snapLines = null;
			invalidationHelper.EndUpdate();
		}
		public void DrawSnapLinesForPrimarySelection(SizeF deltaInPixels, SelectionRules selectionRules) {
			if(PrimarySelection is XRCrossBandControl)
				return;
			ResizeService resizeService = (ResizeService)serviceProvider.GetService(typeof(ResizeService));
			XRControl[] selectedControls = resizeService.GetSelectedControls();
			XRControl primaryControl = PrimarySelection;
			SizeF delta = ZoomService.FromScaledPixels(deltaInPixels, primaryControl.Dpi);
			PointF pt = GetSnapOffset(primaryControl, delta, selectionRules);
			XRControlDesignerBase designer = DesignerHost.GetDesigner(primaryControl) as XRControlDesignerBase;
			RectangleF primaryControlRect = GetControlRectangleForSnapLines(primaryControl, designer.ControlBoundsRelativeToBand, pt, delta, selectionRules);
			DrawSnapLines(primaryControl, primaryControlRect, primaryControl.Band, selectedControls);
		}
	}
}
