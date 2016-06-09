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
	public class RotatedTextPainterNearRotatedLine : RotatedTextPainterBase {
		const string errorMessage = "RotatedTextPainterNearRotatedLine error: Unsupported NearTextPosition";
		float angleOfLineDegree;
		NearTextPosition nearPosition;
		bool TopHalfCircle { 
			get { return IsTopHalfCircle(angleOfLineDegree); } 
		}
		public RotatedTextPainterNearRotatedLine(float angleOfLineDegree, NearTextPosition nearPosition, Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState)
			: base(basePoint, text, textSize, propertiesProvider, mixColorByHitTestState) {
			if (nearPosition != NearTextPosition.Left && nearPosition != NearTextPosition.Right)
				throw new InternalException(errorMessage);
			this.angleOfLineDegree = CancelAngle(angleOfLineDegree);
			this.nearPosition = CalculateActualNearTextPosition(nearPosition);
			SetTextAngle(CalculateActualTextAngle());
		}
		float CalculateActualTextAngle() {
			return TopHalfCircle ? angleOfLineDegree : angleOfLineDegree - 180;
		}
		NearTextPosition OppositeNearPosition(NearTextPosition nearPosition) {
			switch (nearPosition) {
				case NearTextPosition.Left:
					return NearTextPosition.Right;
				case NearTextPosition.Right:
					return NearTextPosition.Left;
				default:
					throw new InternalException(errorMessage);
			}
		}
		NearTextPosition CalculateActualNearTextPosition(NearTextPosition nearPosition) {
			return TopHalfCircle ? nearPosition : OppositeNearPosition(nearPosition);
		}
		protected override Point CalculateLeftTopPoint() {
			switch (nearPosition) {
				case NearTextPosition.Left:
					return new Point(BasePoint.X - Width, BasePoint.Y - Height / 2);
				case NearTextPosition.Right:
					return new Point(BasePoint.X, BasePoint.Y - Height / 2);
				default:
					throw new InternalException(errorMessage);
			}
		}
		protected override TextRotation CalculateRotation() {
			switch (nearPosition) {
				case NearTextPosition.Left:
					return TextRotation.RightCenter;
				case NearTextPosition.Right:
					return TextRotation.LeftCenter;
				default:
					throw new InternalException(errorMessage);
			}
		}
	}
}
