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
	public class ImageAnnotationViewData : AnnotationViewData {
		ImageAnnotation ImageAnnotation {
			get { return (ImageAnnotation)Annotation; }
		}
		public ImageAnnotationViewData(ImageAnnotation annotation, AnnotationShape shape, RefinedPoint connectedPoint, DiagramPoint anchorPoint, int indexInRepository, DiagramPoint location)
			: base(annotation, shape, connectedPoint, anchorPoint, indexInRepository, location) {
		}
		public ImageAnnotationViewData(ImageAnnotation annotation, AnnotationShape shape, RefinedPoint connectedPoint, DiagramPoint anchorPoint, int indexInRepository, DiagramPoint location, Rectangle allowedBoundsForAnnotationPlacing)
			: base(annotation, shape, connectedPoint, anchorPoint, indexInRepository, location) {
			ZPlaneRectangle allowedBounds = (ZPlaneRectangle)allowedBoundsForAnnotationPlacing;
			ZPlaneRectangle annotationBoundsIntoAllowedBounds = ZPlaneRectangle.Intersect(allowedBounds, Bounds);
			if (annotationBoundsIntoAllowedBounds == null)
				return;
			Bounds = annotationBoundsIntoAllowedBounds;
			InnerBounds = CalculateInnerBounds();
		}
		protected override void RenderContent(IRenderer renderer) {
			renderer.EnableAntialiasing(true);
			renderer.ProcessHitTestRegion(HitTestController, Annotation, ConnectedPoint, Shape.CreateHitRegion(HitTestBounds, AnchorPoint));
			Image image = ImageAnnotation.ActualImage;
			if (image != null) {
				renderer.SaveState();
				Point offset = new Point((int)Bounds.Center.X, (int)Bounds.Center.Y);
				renderer.TranslateModel(offset);
				Rectangle imageBounds = (Rectangle)InnerBounds;
				imageBounds.Offset(-offset.X, -offset.Y);
				renderer.RotateModel(Annotation.Angle);
				renderer.DrawImage(image, imageBounds, ImageAnnotation.SizeMode);
				renderer.RestoreState();
			}
			renderer.RestoreAntialiasing();
		}
	}
}
