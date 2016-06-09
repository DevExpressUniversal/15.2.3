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
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.TextRotation;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class LabelBrickExporter : TextBrickExporter {
		#region static
		static StringAlignment GetRotatedAlignment(TextAlignment alignment, float angle) {
			angle = angle % 360;
			if(angle < 0) angle += 360;
			switch (alignment) {
				case TextAlignment.TopLeft:
					return (angle >= 45 && angle < 225) ? StringAlignment.Far : StringAlignment.Near;
				case TextAlignment.BottomLeft:
					return (angle >= 135 && angle < 315) ? StringAlignment.Far : StringAlignment.Near;
				case TextAlignment.TopCenter:
				case TextAlignment.MiddleRight:
				case TextAlignment.MiddleLeft:
				case TextAlignment.MiddleCenter:
				case TextAlignment.BottomCenter:
				case TextAlignment.TopJustify:
				case TextAlignment.MiddleJustify:
				case TextAlignment.BottomJustify:
					return StringAlignment.Center;
				case TextAlignment.TopRight:
					return (angle >= 135 && angle < 315) ? StringAlignment.Near : StringAlignment.Far;
				case TextAlignment.BottomRight:
					return (angle >= 45 && angle < 225) ? StringAlignment.Near : StringAlignment.Far;
				default:
					return StringAlignment.Center;
			}
		}
		static float GetAngleInRadians(float angleInDegrees) {
			return (float)Math.PI * angleInDegrees / 180;
		}
		#endregion
		LabelBrick LabelBrick { get { return Brick as LabelBrick; } }
		bool HasAngle { get { return LabelBrick.Angle % 360 != 0; } }
		protected override void DrawText(IGraphics gr, RectangleF clientRectangle, StringFormat sf, Brush brush) {
			if(HasAngle)
				DrawRotatedText(gr, LabelBrick.Text, Rectangle.Round(clientRectangle), brush, sf);
			else
				base.DrawText(gr, clientRectangle, sf, brush);
		}
		private void DrawRotatedText(IGraphics gr, string text, Rectangle bounds, Brush brush, StringFormat format) {
			using(StringFormat sf = format.Clone() as StringFormat) {
				sf.Alignment = GetRotatedAlignment(Style.TextAlignment, LabelBrick.Angle);
				sf.LineAlignment = StringAlignment.Center;
				int width = HasAngle ? LabelBrick.IsVerticalTextMode ? bounds.Height : CalculateWidth(bounds.Width, bounds.Height, LabelBrick.Angle) : bounds.Width ;
				RotatedTextPainter.DrawRotatedString(gr, text, LabelBrick.Font, brush, bounds, sf, LabelBrick.Angle, false, width, Style.TextAlignment);
			}
		}
		int CalculateWidth(float width, float height, float angle) {
			angle = angle % 360;
			if(angle < 0) angle += 360;
			if(angle > 180) angle -= 2 * (angle - 180);
			if(angle > 90) angle = 180 - angle;
			return Math.Tan(GetAngleInRadians(angle)) > (width / height) ? Calculatehypotenuse(height, 90 - angle) : Calculatehypotenuse(width, angle);
		}
		int Calculatehypotenuse(float cathetus, float angle) {
			return (int)Math.Sqrt(Math.Pow(cathetus, 2) + Math.Pow(cathetus * (float)Math.Tan(GetAngleInRadians(angle)), 2));
		}
	}
}
