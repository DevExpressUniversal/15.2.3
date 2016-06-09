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
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office.Utils;
#if SL
using TransformMatrix = DevExpress.Xpf.Core.Native.Matrix;
#else
using TransformMatrix = System.Drawing.Drawing2D.Matrix;
#endif
namespace DevExpress.Xpf.RichEdit.Controls.Internal {
	public class XpfSelectionPainter : ISelectionPainter {
		#region Fields
		readonly Panel surface;
		readonly DocumentLayoutUnitConverter unitConverter;
		readonly DocumentModel documentModel;
		readonly double zoom;
		readonly double hOffset;
		XpfRichEditSelection selection;
		PageViewInfo info;
		#endregion
		public XpfSelectionPainter(Panel surface, DocumentModel documentModel, double zoom, int hOffset) {
			Guard.ArgumentNotNull(surface, "surface");
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.surface = surface;
			this.documentModel = documentModel;
			this.unitConverter = documentModel.LayoutUnitConverter;
			this.zoom = zoom;
			this.hOffset = hOffset;
		}
		#region Properties
		public PageViewInfo Info { get { return info; } set { info = value; } }
		protected Panel Surface { get { return surface; } }
		public XpfRichEditSelection Selection {
			get {
				if (selection == null) {
					selection = CreateSelection();
					Surface.Children.Add(selection);
				}
				return selection;
			}
		}
		public double Zoom { get { return zoom; } }
		public double HOffset { get { return hOffset; } }
		public DocumentLayoutUnitConverter UnitConverter { get { return unitConverter; } }
		#endregion
		#region CreateSelection
		protected virtual XpfRichEditSelection CreateSelection() {
			return new XpfRichEditSelection();
		}
		#endregion
		#region ISelectionPainter Members
		public virtual void Draw(RowSelectionLayoutBase viewInfo) {
			Selection.Type = SelectionType.Flow;
			Rectangle actualBounds = Rectangle.Intersect(viewInfo.Page.Bounds, viewInfo.Bounds);
			Selection.AddRect(GetSelectionBounds(actualBounds), null);
		}
		public void Draw(TableCellSelectionLayout viewInfo) {
			Selection.Type = SelectionType.Flow;
			Rectangle actualBounds = Rectangle.Intersect(viewInfo.Page.Bounds, viewInfo.Bounds);
			Selection.AddRect(GetSelectionBounds(actualBounds), null);
		}
		public void Draw(RectangularObjectSelectionLayout viewInfo) {
			Selection.Type = SelectionType.Image;
			Box box = viewInfo.Box;
			TransformMatrix transform = CreateTransform(box);
			Selection.AddFloatingObjectRect(GetSelectionBounds(box.ActualSizeBounds), transform);
			XpfHotZonePainter hotZonePainter = new XpfHotZonePainter(this, transform);
			DocumentSelectionLayout selectionLayout = viewInfo.View.SelectionLayout.LastDocumentSelectionLayout;
			HotZoneCollection hotZones = selectionLayout.HotZones;
			foreach (IHotZone zone in hotZones) {
				zone.Accept(hotZonePainter);
			}
		}
		TransformMatrix CreateTransform(Box box) {
			Rect rect = GetSelectionBounds(box.ActualSizeBounds);
			return CreateTransform(box, rect, documentModel);
		}
		public static TransformMatrix CreateTransform(float angle, Rect rect, DocumentModel documentModel) {
			TransformMatrix transform = new TransformMatrix();
			if ((angle % 360f) != 0) {
				Rectangle translatedBounds = new Rectangle((int)Math.Floor(rect.X), (int)Math.Floor(rect.Y), (int)Math.Ceiling(rect.Width), (int)Math.Ceiling(rect.Height));
				transform.RotateAt(angle, RectangleUtils.CenterPoint(translatedBounds));
			}
			return transform;
		}
		public static TransformMatrix CreateTransform(Box box, Rect rect, DocumentModel documentModel) {
			float angle = documentModel.GetBoxEffectiveRotationAngleInDegrees(box);
			return CreateTransform(angle, rect, documentModel);
		}
		public void Draw(FloatingObjectAnchorSelectionLayout viewInfo) {
		}
		#endregion
		#region GetSelectionBounds
		protected internal Rect GetSelectionBounds(Rectangle viewInfoBounds) {
			Rectangle pageBounds = info.ClientBounds;
			return new Rect(Math.Floor(unitConverter.LayoutUnitsToPixelsF((float)(pageBounds.Left + viewInfoBounds.Left * Zoom - HOffset))),
							Math.Floor(unitConverter.LayoutUnitsToPixelsF((float)(pageBounds.Top + viewInfoBounds.Top * Zoom))),
							Math.Ceiling(unitConverter.LayoutUnitsToPixelsF((float)(viewInfoBounds.Width * Zoom))),
							Math.Ceiling(unitConverter.LayoutUnitsToPixelsF((float)(viewInfoBounds.Height * Zoom))));
		}
		#endregion
	}
	public class XpfHotZonePainter : IRectangularObjectHotZoneVisitor {
		readonly XpfSelectionPainter painter;
		readonly TransformMatrix transform;
		public XpfHotZonePainter(XpfSelectionPainter painter, TransformMatrix transform) {
			Guard.ArgumentNotNull(painter, "painter");
			this.painter = painter;
			this.transform = transform;
		}
		public XpfRichEditSelection Selection { get { return painter.Selection; } }
		protected internal Rect GetSelectionBounds(Rectangle viewInfoBounds) {
			return SnapToPixel(painter.GetSelectionBounds(viewInfoBounds));
		}
		void DrawRectangularHotZone(HotZone hotZone) {
			Selection.AddRect(GetSelectionBounds(hotZone.Bounds), transform);
		}
		void DrawEllipticHotZone(HotZone hotZone) {
			Selection.AddEllipse(GetSelectionBounds(hotZone.Bounds), transform);
		}
		Rect SnapToPixel(Rect rect) {
			if ((rect.Width % 2) == 0)
				rect.Width++;
			if ((rect.Height % 2) == 0)
				rect.Height++;
			return rect;
		}
		#region IHotZoneVisitor
		public void Visit(RectangularObjectRotationHotZone hotZone) {
			Rect zoneBounds = GetSelectionBounds(hotZone.Bounds);
			System.Drawing.Point startPoint = RectangleUtils.CenterPoint(hotZone.Bounds);
			Rectangle lineBounds = new Rectangle(startPoint.X, startPoint.Y, Math.Max(1, hotZone.LineEnd.X - startPoint.X), hotZone.LineEnd.Y - startPoint.Y);
			Rect lineRect = GetSelectionBounds(lineBounds);
			lineRect.X = Math.Floor((zoneBounds.Right + zoneBounds.Left) / 2f);
			lineRect.Width = 1;
			Selection.AddRect(lineRect, transform);
			Selection.AddEllipse(zoneBounds, transform);
		}
		public void Visit(RectangularObjectBottomRightHotZone hotZone) {
			DrawEllipticHotZone(hotZone);
		}
		public void Visit(RectangularObjectBottomMiddleHotZone hotZone) {
			DrawRectangularHotZone(hotZone);
		}
		public void Visit(RectangularObjectBottomLeftHotZone hotZone) {
			DrawEllipticHotZone(hotZone);
		}
		public void Visit(RectangularObjectMiddleRightHotZone hotZone) {
			DrawRectangularHotZone(hotZone);
		}
		public void Visit(RectangularObjectMiddleLeftHotZone hotZone) {
			DrawRectangularHotZone(hotZone);
		}
		public void Visit(RectangularObjectTopRightHotZone hotZone) {
			DrawEllipticHotZone(hotZone);
		}
		public void Visit(RectangularObjectTopMiddleHotZone hotZone) {
			DrawRectangularHotZone(hotZone);
		}
		public void Visit(RectangularObjectTopLeftHotZone hotZone) {
			DrawEllipticHotZone(hotZone);
		}
		#endregion
	}
	public class XpfCommentSelectionPainter : XpfSelectionPainter {
		double offsetX;
		double offsetY; 
		public XpfCommentSelectionPainter(Panel surface, DocumentModel documentModel, double zoom, int hOffset, double offsetX, double offsetY) :
			base(surface, documentModel, zoom, hOffset) {
			this.offsetX = offsetX;
			this.offsetY = offsetY; 
		}
		protected override XpfRichEditSelection CreateSelection() {
			return new XpfRichEditCommentSelection();
		}
		public override void Draw(RowSelectionLayoutBase viewInfo) {
			Selection.Type = SelectionType.Flow;
			Rect actualBounds = GetSelectionBounds( viewInfo.Bounds);
			actualBounds = new Rect(actualBounds.X + offsetX * Zoom, actualBounds.Y + offsetY * Zoom, actualBounds.Width, actualBounds.Height);
			Selection.AddRect(actualBounds, null);
		}
	}
}
