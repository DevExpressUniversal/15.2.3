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
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Diagram.Core {
	public interface ILayersHost {
		LayersHostController Controller { get; }
		void SetCursor(DiagramCursor cursor);
		void FocusSurface();
		IInputElement FindInputElement(Point displayPosition);
		IMouseArgs CreatePlatformMouseArgs();
		Size ViewportSize { get; }
		void InvalidateScrollInfo();
		void UpdateLayers(double zoom);
	}
	public class LayersHostController {
		public Rect Extent { get; private set; }
		public Point Offset { get; private set; }
		public Size Viewport { get; private set; }
		double zoom;
		public readonly ILayersHost LayersHost;
		public IDiagramControl Diagram { get; private set; }
		public Point DisplayOffset { get { return Offset.OffsetPoint(Diagram.ScrollMargin.GetOffset().InvertPoint()).RoundPoint(); } }
		public Rect ClientArea { get { return new Rect(DisplayOffset, Viewport); } }
		public LayersHostController(ILayersHost layersHost) {
			zoom = 1;
			LayersHost = layersHost;
		}
		public void SetOwner(IDiagramControl diagram) {
			this.Diagram = diagram;
		}
		public void Update(OffsetCorrection offsetCorrection) {
			var newZoom = Diagram.ZoomFactor;
			var pageBounds = Diagram.Controller.ActualPageBounds.ScaleRect(newZoom);
			var newExtent = pageBounds.SetSize(pageBounds.Size.InflateSize(Diagram.ScrollMargin));
			var newViewport = LayersHost.ViewportSize;
			Point changedOffset = offsetCorrection(
				oldOffset: Offset,
				oldExtent: Extent,
				newExtent: newExtent,
				oldViewport: Viewport,
				newViewport: newViewport,
				oldZoom: zoom,
				newZoom: newZoom
			);
			Extent = newExtent;
			Viewport = newViewport;
			zoom = newZoom;
			SetOffset(Offset.OffsetPoint(changedOffset.RoundPoint()));
			LayersHost.InvalidateScrollInfo();
		}
		public void UpdateOnInit() {
			Update(Diagram.Controller.GetOffsetCorrection(OffsetCorrections.Init));
		}
		public void UpdateOnChangeViewport() {
			Update(OffsetCorrections.Viewport);
		}
		protected void SetOffset(Point offset) {
			this.Offset = ValidatePoint(offset);
			LayersHost.UpdateLayers(zoom);
			LayersHost.InvalidateScrollInfo();
		}
		protected virtual Point ValidatePoint(Point offset) {
			return offset.ValidatePoint(Extent.Location, GetDifference().OffsetPoint(Extent.Location));
		}
		Point GetDifference() {
			return MathHelper.GetDifference(Viewport, Extent.Size);
		}
		public Matrix CreateDisplayToLogicTransform() {
			var matrix = CreateLogicToDisplayTransform();
			matrix.Invert();
			return matrix;
		}
		public Matrix CreateLogicToDisplayTransform() {
			return CreateLogicToDisplayTransform(DisplayOffset, Diagram.ZoomFactor);
		}
		static Matrix CreateLogicToDisplayOffsetTransform(Point offset) {
			var matrix = Matrix.Identity;
			matrix.Translate(-offset.X, -offset.Y);
			return matrix;
		}
		public static Matrix CreateLogicToDisplayTransform(Point offset, double zoomFactor) {
			var scaleMatrix = Matrix.Identity;
			scaleMatrix.Scale(zoomFactor, zoomFactor);
			return Matrix.Multiply(scaleMatrix, CreateLogicToDisplayOffsetTransform(offset));
		}
		public void ChangeHorizontalOffset(double delta) {
			if(GetDifference().X > 0)
				SetHorizontalOffset(Offset.X + delta);
		}
		public void ChangeVerticalOffset(double delta) {
			if(GetDifference().Y > 0)
				SetVerticalOffset(Offset.Y + delta);
		}
		public void SetVerticalOffset(double offset) {
			SetOffset(new Point(this.Offset.X, offset));
		}
		public void SetHorizontalOffset(double offset) {
			SetOffset(new Point(offset, this.Offset.Y));
		}
		public void BringRectIntoView(Rect rect) {
			var viewportRect = new Rect(DisplayOffset, Viewport);
			rect = rect.ScaleRect(zoom).InflateRect(Diagram.BringIntoViewMargin);
			Update((oldOffset, oldExtent, newExtent, oldViewport, newViewport, oldZoom, newZoom) => {
				var correction = default(Point);
				if(rect.Top > viewportRect.Bottom)
					correction = correction.SetY(rect.Bottom - viewportRect.Bottom);
				if(rect.Left > viewportRect.Right)
					correction = correction.SetX(rect.Right - viewportRect.Right);
				if(rect.Bottom < viewportRect.Top)
					correction = correction.SetY(rect.Top - viewportRect.Top);
				if(rect.Right < viewportRect.Left)
					correction = correction.SetX(rect.Left - viewportRect.Left);
				return correction;
			});
		}
		public Point TransformToLogicPoint(Point p) {
			return p.TransformPoint(CreateDisplayToLogicTransform());
		}
		public Point TransformToDisplayPoint(Point p) {
			return p.TransformPoint(CreateLogicToDisplayTransform());
		}
	}
}
