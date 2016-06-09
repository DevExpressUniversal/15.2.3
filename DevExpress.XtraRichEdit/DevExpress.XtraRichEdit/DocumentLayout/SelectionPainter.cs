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

using DevExpress.Office.Layout;
using DevExpress.Office.PInvoke;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Office.Utils;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraRichEdit.Layout {
	#region RichEditSelectionPainter (abstract class)
	public abstract class RichEditSelectionPainter : OfficeSelectionPainter, ISelectionPainter, IRectangularObjectHotZoneVisitor {
		#region Fields
		protected static readonly Color selectionColor = Color.FromArgb(128, 105, 147, 211);
		static readonly Image anchorImage = LoadAnchorImage();
		static Image LoadAnchorImage() {
			System.IO.Stream stream = typeof(RichEditSelectionPainter).Assembly.GetManifestResourceStream("DevExpress.XtraRichEdit.Images.anchor.png");
			return Bitmap.FromStream(stream);
		}
		#endregion
		protected RichEditSelectionPainter(GraphicsCache cache)
			: base(cache) {
		}
		#region Properties
		public new GraphicsCache Cache { get { return (GraphicsCache)base.Cache; } }
		#endregion
		public virtual void Draw(RowSelectionLayoutBase viewInfo) {
			FillRectangle(viewInfo.Bounds);
		}
		public virtual void Draw(RectangularObjectSelectionLayout viewInfo) {
			Rectangle bounds = viewInfo.Box.ActualSizeBounds;
			Point center = RectangleUtils.CenterPoint(bounds);
			bool transformApplied = TryPushRotationTransform(center, viewInfo.View.DocumentModel.GetBoxEffectiveRotationAngleInDegrees(viewInfo.Box));
			try {
				if(viewInfo.Resizeable)
					DrawRectangle(bounds);
				else
					FillRectangle(bounds);
				HotZoneCollection hotZones = viewInfo.View.SelectionLayout.LastDocumentSelectionLayout.HotZones;
				hotZones.ForEach(DrawHotZone);
			}
			finally {
				if(transformApplied)
					PopTransform();
			}
		}
		public virtual void Draw(FloatingObjectAnchorSelectionLayout viewInfo) {
			Point location = viewInfo.AnchorBounds.Location;
			DocumentLayoutUnitConverter unitConverter = viewInfo.View.DocumentLayout.UnitConverter;
			Point pixelPoint = unitConverter.LayoutUnitsToPixels(location);
			Rectangle bounds = new Rectangle(
				unitConverter.PixelsToLayoutUnits(pixelPoint.X),
				unitConverter.PixelsToLayoutUnits(pixelPoint.Y),
				unitConverter.PixelsToLayoutUnits(anchorImage.Width),
				unitConverter.PixelsToLayoutUnits(anchorImage.Height));
			bounds.X -= bounds.Width;
			Cache.Graphics.DrawImage(anchorImage, bounds);
		}
		protected internal virtual void DrawHotZone(IHotZone hotZone) {
			hotZone.Accept(this);
		}
		#region IHotZoneVisitor Members
		void IRectangularObjectHotZoneVisitor.Visit(RectangularObjectBottomRightHotZone hotZone) {
			DrawEllipticHotZone(hotZone.Bounds, HotZoneGradientColor);
		}
		void IRectangularObjectHotZoneVisitor.Visit(RectangularObjectBottomMiddleHotZone hotZone) {
			DrawRectangularHotZone(hotZone.Bounds, HotZoneGradientColor);
		}
		void IRectangularObjectHotZoneVisitor.Visit(RectangularObjectBottomLeftHotZone hotZone) {
			DrawEllipticHotZone(hotZone.Bounds, HotZoneGradientColor);
		}
		void IRectangularObjectHotZoneVisitor.Visit(RectangularObjectMiddleRightHotZone hotZone) {
			DrawRectangularHotZone(hotZone.Bounds, HotZoneGradientColor);
		}
		void IRectangularObjectHotZoneVisitor.Visit(RectangularObjectMiddleLeftHotZone hotZone) {
			DrawRectangularHotZone(hotZone.Bounds, HotZoneGradientColor);
		}
		void IRectangularObjectHotZoneVisitor.Visit(RectangularObjectTopRightHotZone hotZone) {
			DrawEllipticHotZone(hotZone.Bounds, HotZoneGradientColor);
		}
		void IRectangularObjectHotZoneVisitor.Visit(RectangularObjectTopMiddleHotZone hotZone) {
			DrawRectangularHotZone(hotZone.Bounds, HotZoneGradientColor);
		}
		void IRectangularObjectHotZoneVisitor.Visit(RectangularObjectTopLeftHotZone hotZone) {
			DrawEllipticHotZone(hotZone.Bounds, HotZoneGradientColor);
		}
		void IRectangularObjectHotZoneVisitor.Visit(RectangularObjectRotationHotZone hotZone) {
			DrawLine(RectangleUtils.CenterPoint(hotZone.Bounds), hotZone.LineEnd);
			DrawEllipticHotZone(hotZone.Bounds, HotZoneRotationGradientColor);
		}
		#endregion
	}
	#endregion
	#region XorSelectionPainter
	public class XorSelectionPainter : RichEditSelectionPainter {
		static readonly float[] penPattern = new float[] { 5, 5 };
		public XorSelectionPainter(GraphicsCache cache)
			: base(cache) {
		}
		[System.Security.SecuritySafeCritical]
		protected override void FillRectangle(Rectangle bounds) {
			IntPtr hdc = Cache.Graphics.GetHdc();
			try {
				FillRectangleCore(hdc, bounds);
			}
			finally {
				Cache.Graphics.ReleaseHdc(hdc);
			}
		}
		protected internal virtual void FillRectangleCore(IntPtr hdc, Rectangle bounds) {
			IntPtr brush = Win32.GetStockObject(Win32.StockObject.WHITE_BRUSH);
			IntPtr oldBrush = Win32.SelectObject(hdc, brush);
			try {
				Win32.PatBlt(hdc, bounds.X, bounds.Y, bounds.Width, bounds.Height, Win32.TernaryRasterOperation.PATINVERT);
			}
			finally {
				Win32.SelectObject(hdc, oldBrush);
			}
		}
		protected override void DrawRectangle(Rectangle bounds) {
			using (Pen pen = new Pen(Color.Black)) {
				pen.DashStyle = DashStyle.Custom;
				pen.DashPattern = penPattern;
				Cache.DrawRectangle(pen, bounds);
			}
		}
		protected override void DrawLine(Point from, Point to) {
			using (Pen pen = new Pen(Color.Black)) {
				pen.DashStyle = DashStyle.Custom;
				pen.DashPattern = penPattern;
				Cache.Graphics.DrawLine(pen, from, to);
			}
		}
	}
	#endregion
	#region SemitransparentSelectionPainter
	public class SemitransparentSelectionPainter : RichEditSelectionPainter {
		public SemitransparentSelectionPainter(GraphicsCache cache)
			: base(cache) {
		}
		protected override void FillRectangle(Rectangle bounds) {
			Cache.FillRectangle(selectionColor, bounds);
		}
		protected override void DrawRectangle(Rectangle bounds) {
			using (Pen pen = new Pen(selectionColor)) {
				Cache.DrawRectangle(pen, bounds);
			}
		}
		protected override void DrawLine(Point from, Point to) {
			using (Pen pen = new Pen(selectionColor)) {
				Cache.Graphics.DrawLine(pen, from, to);
			}
		}
	}
	#endregion
	#region RectangularObjectSelectionPainter
	public class RectangularObjectSelectionPainter : RichEditSelectionPainter {
		public RectangularObjectSelectionPainter(GraphicsCache cache)
			: base(cache) {
		}
		protected override void DrawLine(Point from, Point to) {
			Graphics.DrawLine(Cache.GetPen(ShapeBorderColor, ShapeBorderWidth), from, to);
		}
		protected override void FillRectangle(Rectangle bounds) {
			Cache.FillRectangle(selectionColor, bounds);
		}
		protected override void DrawRectangle(Rectangle bounds) {
			Cache.DrawRectangle(Cache.GetPen(ShapeBorderColor, ShapeBorderWidth), bounds);
		}
	}
	#endregion
}
