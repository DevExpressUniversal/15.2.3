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

using System.Drawing;
namespace DevExpress.XtraCharts.Native {
	public class VerticalRotatedTextPainter : TextPainter {
		RectangleF boundsBeforeRotation;
		bool topToBottom;
		public VerticalRotatedTextPainter(string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, TextMeasurer textMeasurer, PointF location, bool topToBottom, int maxWidth, int maxLineCount, bool wordWrap)
			: base(text, textSize, propertiesProvider, textMeasurer, location, maxWidth, maxLineCount, wordWrap) {
			boundsBeforeRotation = new RectangleF(0, 0, textSize.Height, textSize.Width);
			boundsBeforeRotation = GraphicUtils.RoundRectangle(boundsBeforeRotation);
			this.topToBottom = topToBottom;
		}
		public override void Render(IRenderer renderer, HitTestController hitTestController, IHitRegion hitRegion, object additionalHitObject, Color color) {
			RectangleF initialRectangle = boundsBeforeRotation;
			RectangleF rotatedRectangle = Bounds;
			if (!GraphicUtils.CheckIsSizePositive(initialRectangle.Size) || !GraphicUtils.CheckIsSizePositive(initialRectangle.Size))
				return;
			ProcessHitTesting(renderer, hitTestController, hitRegion, additionalHitObject);
			renderer.SaveState();
			double offsetX, offsetY;
			float rotationAngle;
			if (topToBottom) {
				offsetX = rotatedRectangle.Left + initialRectangle.Height;
				offsetY = rotatedRectangle.Top;
				rotationAngle = 90;
			}
			else {
				offsetX = rotatedRectangle.Left;
				offsetY = rotatedRectangle.Top + initialRectangle.Width;
				rotationAngle = -90;
			}
			renderer.TranslateModel((float)offsetX, (float)offsetY);
			renderer.RotateModel(rotationAngle);
			ZPlaneRectangle rectangle = (ZPlaneRectangle)initialRectangle;
			if (Shadow != null && Shadow.Visible)
				Shadow.Render(renderer, (Rectangle)rectangle);
			RenderInternal(renderer, hitTestController, (Rectangle)rectangle, color, true);
			renderer.RestoreState();
		}
		public override void RenderWithClipping(IRenderer renderer, HitTestController hitTestController, object additionalHitObject, Color color, Rectangle clipBounds) {
			RectangleF initialRectangle = boundsBeforeRotation;
			RectangleF rotatedRectangle = Bounds;
			if (!GraphicUtils.CheckIsSizePositive(initialRectangle.Size) || !GraphicUtils.CheckIsSizePositive(initialRectangle.Size))
				return;
			RectangleF clippedBounds = RectangleF.Intersect(Bounds, clipBounds);
			HitRegion hitRegion = new HitRegion(clippedBounds);
			ProcessHitTesting(renderer, hitTestController, hitRegion, additionalHitObject);
			renderer.SaveState();
			renderer.SetClipping(clipBounds);
			double offsetX, offsetY;
			float rotationAngle;
			if (topToBottom) {
				offsetX = rotatedRectangle.Left + initialRectangle.Height;
				offsetY = rotatedRectangle.Top;
				rotationAngle = 90;
			}
			else {
				offsetX = rotatedRectangle.Left;
				offsetY = rotatedRectangle.Top + initialRectangle.Width;
				rotationAngle = -90;
			}
			renderer.TranslateModel((float)offsetX, (float)offsetY);
			renderer.RotateModel(rotationAngle);
			ZPlaneRectangle rectangle = (ZPlaneRectangle)initialRectangle;
			if (Shadow != null && Shadow.Visible)
				Shadow.Render(renderer, (Rectangle)rectangle);
			RenderInternal(renderer, hitTestController, (Rectangle)rectangle, color, true);
			renderer.RestoreState();
		}
	}
}
