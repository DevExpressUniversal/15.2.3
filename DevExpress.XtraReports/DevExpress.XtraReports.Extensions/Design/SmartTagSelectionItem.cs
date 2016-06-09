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
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Design {
	[Flags]
	public enum SmartTagState { None = 0, Popup = 1, ContainsMouse = 2 }
	public class SmartTagSelectionItem : SelectionItemBase {
		public static SmartTagSelectionItem CreateInstance(XRControl ctl, XRControlDesignerBase designer, IServiceProvider provider) {
			if(!LockService.GetInstance(provider).CanChangeComponent(ctl))
				return null;
			return ctl is Band ? new BandSmartTagSelectionItem((Band)ctl, designer) :
				new SmartTagSelectionItem(ctl, designer);
		}
		static RectangleF GetBaseRectangle() {
			return new RectangleF(0, 0, itemWidth, itemWidth);
		}
		protected static readonly float itemWidth = XRConvert.Convert(11f, GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.Pixel); 
		RectangleF rect = RectangleF.Empty;
		protected XRSmartTagService smartTagSvc;
		protected SmartTagViewInfo fViewInfo;
		SmartTagPainter painter;
		UserLookAndFeel LookAndFeel { get { return DesignLookAndFeelHelper.GetLookAndFeel(smartTagSvc.DesignerHost); }
		}
		public SmartTagSelectionItem(XRControl ctl, XRComponentDesigner designer) : base(ctl, designer) {
			rect = GetRectangleRelativeToView(GetBaseRectangle());
			smartTagSvc = (XRSmartTagService)GetService(typeof(XRSmartTagService));
			fViewInfo = new SmartTagViewInfo();
		}
		protected virtual RectangleF GetRectangleRelativeToView(RectangleF baseRect) {
			RectangleF rect = svcBandViewInfo.GetControlViewClientBounds(fControl);			
			const float yDelta = 4f;
			baseRect.Offset(rect.Right - baseRect.Width, rect.Top - baseRect.Height - yDelta);
			return baseRect;
		}
		public override void DoPaint(Graphics graph, Band band) {
			if(!this.Control.IsDisposed && this.Control.IsInsideBand(band)) {
				rect = GetRectangleRelativeToView(GetBaseRectangle());
				DrawSmartTag(graph, XRConvert.Convert(rect, GraphicsDpi.Pixel, GraphicsDpi.Document));
			}
		}
		public void DrawSmartTag(Graphics graph, RectangleF rect) {
			fViewInfo.SmartTagState = GetSmartTagState();
			fViewInfo.SmartTagRect = rect;
			UpdatePainter();
			using(GraphicsCache cache = new GraphicsCache(graph)) {
				ObjectPainter.DrawObject(cache, painter, fViewInfo);
			}
		}
		void UpdatePainter() {
			painter = ReportPaintStyles.GetPaintStyle(LookAndFeel).CreateSmartTagPainter(LookAndFeel);
		}
		protected SmartTagState GetSmartTagState()  { 
			SmartTagState val = SmartTagState.None;
			if(smartTagSvc.PopupIsVisible) 
				val |= SmartTagState.Popup;
			if(GetScreenRectangle().Contains(System.Windows.Forms.Control.MousePosition))
				val |= SmartTagState.ContainsMouse;
			return val;
		}
		public virtual RectangleF GetScreenRectangle()  {
			return svcBandViewInfo.View.RectangleToScreen(Rectangle.Round(rect));
		}
		public override void HandleMouseMove(object sender, BandMouseEventArgs args) {
			((Control)sender).Cursor = Cursors.Default;
		}
		public override void HandleMouseDown(object sender, BandMouseEventArgs args) {
			if(!smartTagSvc.PopupIsVisible && args.Button.IsLeft()) {
				smartTagSvc.ShowPopup(GetPopupFormLocation(), fDesigner, this);
			} else
				smartTagSvc.HidePopup();
		}
		public virtual Point GetPopupFormLocation() {
			RectangleF screenRect = GetScreenRectangle();
			return Point.Ceiling(new PointF(screenRect.Right, screenRect.Top));
		}
		public virtual void Invalidate() {
			if(fViewInfo.SmartTagState != GetSmartTagState())
				svcBandViewInfo.Invalidate();
		}
		public override bool ContainsPoint(Point pt, BandViewInfo viewInfo) {
			return rect.Contains(pt);
		}
	}
	public class BandSmartTagSelectionItem : SmartTagSelectionItem {
		public BandSmartTagSelectionItem(Band band, XRControlDesignerBase designer) : base(band, designer) {
		}
		protected override RectangleF GetRectangleRelativeToView(RectangleF baseRect) {
			BandViewInfo bvi = svcBandViewInfo.GetViewInfoByBand((Band)fControl);
			if(bvi == null) {
				svcBandViewInfo.InvalidateViewInfo();
				bvi = svcBandViewInfo.GetViewInfoByBand((Band)fControl);
			}
			if(bvi != null) {
				const float xDelta = 2f;
				const float yDelta = -3f;
				baseRect.Offset(bvi.TextBounds.Right + xDelta, bvi.Bounds.Top + yDelta);
			}
			return baseRect;
		}
	}
	public class WinControlSmartTagSelectionItem : SmartTagSelectionItem {
		Control winControl;
		public WinControlSmartTagSelectionItem(XRControl control, XRComponentDesigner designer, Control winControl) : base(control, designer) {
			this.winControl = winControl;
			this.fViewInfo.IsWinControlItem = true;
		}
		protected override RectangleF GetRectangleRelativeToView(RectangleF baseRect) {
			return RectangleF.Empty;
		}
		public override void DoPaint(Graphics graph, Band band) {
		}
		Rectangle TagBounds {
			get {
				Rectangle rect = RectHelper.AlignRectangle(new Rectangle(0, 0, (int)itemWidth, (int)itemWidth), winControl.ClientRectangle, ContentAlignment.MiddleCenter);
				rect.Offset(1, 1);
				return rect;
			}
		}
		public Rectangle TagScreenBounds {
			get {
				return winControl.RectangleToScreen(TagBounds);
			}
		}
		public override RectangleF GetScreenRectangle() {
			return winControl.RectangleToScreen(RectHelper.InflateRect(winControl.ClientRectangle, -1, -1, -1, -1));
		}
		public override void Invalidate() {
			if(fViewInfo.SmartTagState != GetSmartTagState())
				winControl.Invalidate();
		}
		public override Point GetPopupFormLocation() {
			return Point.Ceiling(RectHelper.BottomRight(TagScreenBounds));
		}
	}
	public class SmartTagViewInfo : ObjectInfoArgs {
		SmartTagState smartTagState;
		RectangleF smartTagRect;
		bool drawBorder = true; 
		bool isWinControlItem;
		public SmartTagState SmartTagState { get { return smartTagState; } set { smartTagState = value; }
		}
		public RectangleF SmartTagRect { get { return smartTagRect; } set { smartTagRect = value; }
		}
		public bool DrawBorder { get { return drawBorder; } set { drawBorder = value; }
		}
		public bool IsWinControlItem { get { return isWinControlItem; } set { isWinControlItem = value; }
		}
		public RectangleF GetBackgroundRect(float outDpi) {
			if(drawBorder) return smartTagRect;
			float delta = XRConvert.Convert(1f, GraphicsDpi.Pixel, outDpi);
			return RectangleF.Inflate(smartTagRect, -delta, -delta);
		}
	}
}
