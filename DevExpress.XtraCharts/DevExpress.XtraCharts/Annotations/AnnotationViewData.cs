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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class AnnotationViewData : IBoundsProvider {
		readonly Annotation annotation;
		readonly AnnotationShape shape;
		readonly DiagramPoint anchorPoint;
		readonly int indexInRepository;
		readonly RefinedPoint connectedPoint;
		ZPlaneRectangle bounds;
		ZPlaneRectangle innerBounds;
		protected AnnotationShape Shape { 
			get { return shape; } 
		}
		protected DiagramPoint AnchorPoint { 
			get { return anchorPoint; } 
		}
		protected ZPlaneRectangle Bounds { 
			get { return bounds; }
			set { bounds = value;}
		}
		protected ZPlaneRectangle InnerBounds {
			get { return innerBounds; }
			set { innerBounds = value; }
		}
		protected ZPlaneRectangle HitTestBounds {
			get {
				float width = (float)Math.Max(5, bounds.Width);
				float height = (float)Math.Max(5, bounds.Height);
				return MathUtils.MakeRectangle(bounds.Center, new SizeF(width, height));
			}
		}
		protected HitTestController HitTestController { 
			get { return CommonUtils.FindOwnerChart(annotation).HitTestController; } 
		}
		protected RefinedPoint ConnectedPoint {
			get {
				return connectedPoint;
			}
		}
		public Annotation Annotation {
			get {
				return annotation;
			}
		}
		public int IndexInRepository {
			get {
				return indexInRepository;
			}
		}
		public AnnotationViewData(Annotation annotation, AnnotationShape shape, RefinedPoint connectedPoint, DiagramPoint anchorPoint, int indexInRepository, DiagramPoint location) {
			this.annotation = annotation;
			this.shape = shape;
			this.anchorPoint = anchorPoint;
			this.indexInRepository = indexInRepository;
			this.annotation.UpdateSize();
			this.bounds = new ZPlaneRectangle(location, annotation.Width, annotation.Height);
			this.innerBounds = CalculateInnerBounds();
			this.connectedPoint = connectedPoint;
		}
		void RenderNavigation(IRenderer renderer) {
			if (!bounds.AreWidthAndHeightPositive() || !annotation.CanDrag())
				return;
			renderer.ProcessHitTestRegion(HitTestController, annotation, new ChartFocusedArea(annotation), true, shape.CreateHitRegion(HitTestBounds, AnchorPoint), true);
			foreach (AnnotationDragPoint point in AnnotationDragPointsHelper.CalcPoints(annotation, shape, bounds, HitTestBounds, anchorPoint))
				point.Render(renderer);
		}
		protected abstract void RenderContent(IRenderer renderer);
		protected ZPlaneRectangle CalculateInnerBounds() {
			ZPlaneRectangle outerBounds = ZPlaneRectangle.Inflate(bounds, -this.annotation.Border.ActualThickness, -this.annotation.Border.ActualThickness);
			outerBounds = this.annotation.Padding.DecreaseRectangle(outerBounds);
			Size innerSize = this.shape.CalcInnerSize(outerBounds.Size.ToSize());
			DiagramPoint innerLocation = new DiagramPoint(outerBounds.Location.X + (outerBounds.Width - innerSize.Width) / 2,
														  outerBounds.Location.Y + (outerBounds.Height - innerSize.Height) / 2);
			return new ZPlaneRectangle(innerLocation, innerSize.Width, innerSize.Height);
		}
		public void CalculateDiagramBoundsCorrection(RectangleCorrection correction) {
			if (annotation.ActualLabelMode)
				correction.Update((Rectangle)MathUtils.CalcBounds(bounds, annotation.Angle));
		}		
		public void Render(IRenderer renderer) {
			shape.RenderShadow(renderer, bounds, anchorPoint);
			shape.Render(renderer, bounds, anchorPoint);
			RenderContent(renderer);
			RenderNavigation(renderer);
		}
		#region IBoundsProvider
		bool IBoundsProvider.Enable {
			get { return annotation.ActualLabelMode; }
		}
		GRealRect2D IBoundsProvider.GetBounds() {
			ZPlaneRectangle rect = MathUtils.CalcBounds(bounds, annotation.Angle);
			return new GRealRect2D(rect.Left, rect.Top, rect.Width, rect.Height);
		}
		#endregion
	}
}
