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
	public class RotatedTextPainterNearLine : RotatedTextPainterBase {
		NearTextPosition nearPosition;
		public RotatedTextPainterNearLine(Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, NearTextPosition nearPosition, float angleDegree, bool mixColorByHitTestState)
			: this(basePoint, text, textSize, propertiesProvider, nearPosition, angleDegree, mixColorByHitTestState, false, null, 0, 0, false) {
		}
		public RotatedTextPainterNearLine(Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, NearTextPosition nearPosition, float angleDegree, bool mixColorByHitTestState, bool parseText, TextMeasurer textMeasurer)
			: this(basePoint, text, textSize, propertiesProvider, nearPosition, angleDegree, mixColorByHitTestState, parseText, textMeasurer, 0, 0, false) {
		}
		public RotatedTextPainterNearLine(Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, NearTextPosition nearPosition, float angleDegree, bool mixColorByHitTestState, bool parseText, TextMeasurer textMeasurer, int maxWidth, int maxLineCount, bool wordWrap)
			: base(basePoint, text, textSize, propertiesProvider, mixColorByHitTestState, parseText, textMeasurer, maxWidth, maxLineCount, wordWrap) {
			this.nearPosition = nearPosition;
			SetTextAngle(CancelAngle(angleDegree));
		}
		protected override Point CalculateLeftTopPoint() {
			GRealPoint2D leftTopPoint;
			GRealRect2D bounds = new GRealRect2D(BasePoint.X, BasePoint.Y, Width, Height);
			switch (nearPosition) {
				case NearTextPosition.Bottom:
					leftTopPoint = AxisLabelRotationHelper.CalculateLeftTopPointForBottomPosition(bounds, TextAngleDegree, TextAngleRadian, true);
					break;
				case NearTextPosition.Left:
					leftTopPoint = AxisLabelRotationHelper.CalculateLeftTopPointForLeftPosition(bounds, TextAngleDegree, TextAngleRadian, true);
					break;
				case NearTextPosition.Right:
					leftTopPoint = AxisLabelRotationHelper.CalculateLeftTopPointForRightPosition(bounds, TextAngleDegree, TextAngleRadian, true);
					break;
				case NearTextPosition.Top:
					leftTopPoint = AxisLabelRotationHelper.CalculateLeftTopPointForTopPosition(bounds, TextAngleDegree, TextAngleRadian, true);
					break;
				default:
					throw new DefaultSwitchException();
			}
			return new Point((int)leftTopPoint.X, (int)leftTopPoint.Y);
		}
		protected override TextRotation CalculateRotation() {
			switch (nearPosition) {
				case NearTextPosition.Bottom:
					return AxisLabelRotationHelper.CalculateRotationForBottomNearPosition(TextAngleDegree);
				case NearTextPosition.Left:
					return AxisLabelRotationHelper.CalculateRotationForLeftNearPosition(TextAngleDegree);
				case NearTextPosition.Right:
					return AxisLabelRotationHelper.CalculateRotationForRightNearPosition(TextAngleDegree);
				case NearTextPosition.Top:
					return AxisLabelRotationHelper.CalculateRotationForTopNearPosition(TextAngleDegree);
				default:
					throw new DefaultSwitchException();
			}
		}
	}
}
