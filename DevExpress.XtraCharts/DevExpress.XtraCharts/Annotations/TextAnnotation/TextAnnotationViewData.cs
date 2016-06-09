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

using DevExpress.Charts.Native;
using System.Drawing;
namespace DevExpress.XtraCharts.Native {
	public class TextAnnotationViewData : AnnotationViewData {
		readonly TextMeasurer textMeasurer;
		readonly RotatedTextPainterOnCircleTangent painter;
		internal RotatedTextPainterOnCircleTangent TextPainter { get { return painter; } }
		public TextAnnotationViewData(TextAnnotation annotation, AnnotationShape shape, RefinedPoint connectedPoint, DiagramPoint anchorPoint, int indexInRepository, DiagramPoint location, TextMeasurer textMeasurer)
			: base(annotation, shape,connectedPoint, anchorPoint, indexInRepository, location) {
			this.textMeasurer = textMeasurer;
			this.painter = CreateTextPainter();
		}
		public TextAnnotationViewData(TextAnnotation annotation, AnnotationShape shape, RefinedPoint connectedPoint, DiagramPoint anchorPoint, int indexInRepository, DiagramPoint location, Rectangle allowedBoundsForAnnotationPlacing, TextMeasurer textMeasurer)
			: base(annotation, shape, connectedPoint, anchorPoint, indexInRepository, location) {
			ZPlaneRectangle allowedBounds = (ZPlaneRectangle)allowedBoundsForAnnotationPlacing;
			ZPlaneRectangle annotationBoundsIntoAllowedBounds = ZPlaneRectangle.Intersect(allowedBounds, Bounds);
			if (annotationBoundsIntoAllowedBounds == null)
				return;
			Bounds = annotationBoundsIntoAllowedBounds;
			InnerBounds = CalculateInnerBounds();
			this.textMeasurer = textMeasurer;
			this.painter = CreateTextPainter();
		}
		RotatedTextPainterOnCircleTangent CreateTextPainter() {
			Size size = InnerBounds.Size.ToSize();
			if (size.Width > 0 && size.Height > 0) {
				TextAnnotation annotation = (TextAnnotation)Annotation;
				Point basePoint = new Point(MathUtils.StrongRound(InnerBounds.Center.X), MathUtils.StrongRound(InnerBounds.Center.Y));
				if (InnerBounds.Width != Annotation.Width || InnerBounds.Height != Annotation.Height)
					return new RotatedTextPainterOnCircleTangent(annotation.Angle, basePoint, annotation.ActualText, size, annotation, false, false, textMeasurer, size.Width, size.Height);
				return new RotatedTextPainterOnCircleTangent(annotation.Angle, basePoint, annotation.ActualText, size, annotation, false, false, true, textMeasurer);
			}
			else
				return null;
		}
		protected override void RenderContent(IRenderer renderer) {
			if (painter != null)
				painter.Render(renderer, HitTestController, Shape.CreateHitRegion(HitTestBounds, AnchorPoint), ConnectedPoint, Color.Black);
		}
	}
}
