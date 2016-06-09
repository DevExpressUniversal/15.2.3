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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class RotatedTextPainterNearCircleBottomToTop : RotatedTextPainterNearCircle {
		public RotatedTextPainterNearCircleBottomToTop(float angleOnCircleDegree, Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, bool parseText, TextMeasurer textMeasurer, int maxWidth, int maxLineCount)
			: base(angleOnCircleDegree, basePoint, text, textSize, propertiesProvider, mixColorByHitTestState, parseText, textMeasurer, maxWidth, maxLineCount) {
		}
		protected override float CalculateActualTextAngle() {
			return -90;
		}
		protected override Point CalculateLeftTopPoint() {
			if (AngleOnCircleDegree == 0)
				return new Point(BasePoint.X, BasePoint.Y - Height / 2);
			else if (AngleOnCircleDegree > 0 && AngleOnCircleDegree < 90)
				return new Point(BasePoint.X, BasePoint.Y);
			else if (AngleOnCircleDegree == 90)
				return new Point(BasePoint.X - Width / 2, BasePoint.Y);
			else if (AngleOnCircleDegree > 90 && AngleOnCircleDegree < 180)
				return new Point(BasePoint.X - Width, BasePoint.Y);
			else if (AngleOnCircleDegree == 180)
				return new Point(BasePoint.X - Width, BasePoint.Y - Height / 2);
			else if (AngleOnCircleDegree > 180 && AngleOnCircleDegree < 270)
				return new Point(BasePoint.X - Width, BasePoint.Y - Height);
			else if (AngleOnCircleDegree == 270)
				return new Point(BasePoint.X - Width / 2, BasePoint.Y - Height);
			else if (AngleOnCircleDegree > 270 && AngleOnCircleDegree < 360)
				return new Point(BasePoint.X, BasePoint.Y - Height);
			else
				throw new InternalException("RotatedTextPainterNearCircleBottomToTop error");
		}
		protected override TextRotation CalculateRotation() {
			if (AngleOnCircleDegree == 0)
				return TextRotation.LeftCenter;
			else if (AngleOnCircleDegree > 0 && AngleOnCircleDegree < 90)
				return TextRotation.LeftTop;
			else if (AngleOnCircleDegree == 90)
				return TextRotation.CenterTop;
			else if (AngleOnCircleDegree > 90 && AngleOnCircleDegree < 180)
				return TextRotation.RightTop;
			else if (AngleOnCircleDegree == 180)
				return TextRotation.RightCenter;
			else if (AngleOnCircleDegree > 180 && AngleOnCircleDegree < 270)
				return TextRotation.RightBottom;
			else if (AngleOnCircleDegree == 270)
				return TextRotation.CenterBottom;
			else if (AngleOnCircleDegree > 270 && AngleOnCircleDegree < 360)
				return TextRotation.LeftBottom;
			else
				throw new InternalException("RotatedTextPainterNearCircleTopToBottom error");
		}
	}
}
