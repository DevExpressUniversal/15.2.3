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
namespace DevExpress.XtraCharts.Native {
	public class TextPainter : TextPainterBase {
		const int MaxLineCountWhenWordWrap = 3;
		PointF location; 
		RectangleF? boundsWithBorder;
		public RectangleF BoundsWithBorder {
			get {
				if(!boundsWithBorder.HasValue) {
					int borderThickness = BorderThickness;
					boundsWithBorder = RectangleF.Inflate(Bounds, borderThickness, borderThickness);
				}
				return boundsWithBorder.Value;
			}
		}
		public TextPainter(string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, TextMeasurer textMeasurer, PointF location, int maxWidth, int maxLineCount, bool wordWrap)
			: base(text, textSize, propertiesProvider, false, false, textMeasurer, maxWidth, maxLineCount, wordWrap) {
			this.location = location;
		}
		public TextPainter(string text, Size textSize, ITextPropertiesProvider propertiesProvider, TextMeasurer textMeasurer, RectangleF allowedBounds, PointF locationWithoutTakingBorderintoAccount)
			: base(text, textSize, propertiesProvider, false, false, textMeasurer, 0, 0, false) {
			this.location = locationWithoutTakingBorderintoAccount;
			int lines;
			int maxWidth = (int)(allowedBounds.Width - 2 * BorderThickness);
			SizeF size = textMeasurer.MeasureString(text, propertiesProvider.Font, maxWidth, propertiesProvider.Alignment, StringAlignment.Center, out lines);
			Width = MathUtils.Ceiling(allowedBounds.Width - 2 * BorderThickness);
			if(lines > MaxLineCountWhenWordWrap)
				Height = RoundHeight(size.Height / lines * MaxLineCountWhenWordWrap);
			else
				Height = RoundHeight(size.Height);
			if(Height >= allowedBounds.Height - 2 * BorderThickness)
				Height = RoundHeight(allowedBounds.Height - 2 * BorderThickness);
		}
		protected override int RoundHeight(float height) {
			return (int)Math.Round(height);
		}
		protected override RectangleF GetBounds() {
			return new RectangleF(location.X, location.Y, Width, Height);
		}
		public void SetLocation(PointF location) {
			this.location = location;
			CalculateBounds();
			boundsWithBorder = null;
		}
		public override void Render(IRenderer renderer, HitTestController hitTestController, IHitRegion hitRegion, object additionalHitObject, Color color) {
			RectangleF bounds = Bounds;
			if(!bounds.AreWidthAndHeightPositive())
				return;
			ProcessHitTesting(renderer, hitTestController, hitRegion, additionalHitObject);
			if(Shadow != null && Shadow.Visible)
				Shadow.Render(renderer, bounds);
			RenderInternal(renderer, hitTestController, bounds, color, false);
		}
		public override void RenderWithClipping(IRenderer renderer, HitTestController hitTestController, object additionalHitObject, Color color, Rectangle clipBounds) {
			renderer.SetClipping(clipBounds);
			RectangleF bounds = Bounds;
			if(!bounds.AreWidthAndHeightPositive())
				return;
			RectangleF hitRegionBouns = RectangleF.Intersect(bounds, clipBounds);
			HitRegion hitRegion = new HitRegion(hitRegionBouns);
			ProcessHitTesting(renderer, hitTestController, hitRegion, additionalHitObject);
			if(Shadow != null && Shadow.Visible)
				Shadow.Render(renderer, bounds);
			RenderInternal(renderer, hitTestController, bounds, color, false);
			renderer.RestoreClipping();
		}
		public override void Offset(double dx, double dy) {
			location.X += (float)dx;
			location.Y += (float)dy;
			SetLocation(location);
		}
		public override void Rotate(double angle) {
		}
	}
}
