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
	public abstract class RotatedTextPainterOnCircle : RotatedTextPainterBase {
		readonly bool shouldCorrectAngle;
		readonly float angleOnCircleDegree;
		protected float AngleOnCircleDegree { 
			get { return angleOnCircleDegree; } 
		}
		protected bool ShouldCorrectAngle { 
			get { return shouldCorrectAngle; } 
		}
		public RotatedTextPainterOnCircle(float angleOnCircleDegree, Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, bool shouldCorrectAngle)
			: this(angleOnCircleDegree, basePoint, text, textSize, propertiesProvider, mixColorByHitTestState, shouldCorrectAngle, false, null) {
		}
		public RotatedTextPainterOnCircle(float angleOnCircleDegree, Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, bool shouldCorrectAngle, bool parseText, TextMeasurer textMeasurer)
			: base(basePoint, text, textSize, propertiesProvider, mixColorByHitTestState, parseText, textMeasurer, 0, 0, false) {
			this.shouldCorrectAngle = shouldCorrectAngle;
			this.angleOnCircleDegree = CancelAngle(angleOnCircleDegree);
			SetTextAngle(CalculateActualTextAngle());
		}
		public RotatedTextPainterOnCircle(float angleOnCircleDegree, Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, bool shouldCorrectAngle, TextMeasurer textMeasurer, int maxWidth, int maxHeight)
			: base(basePoint, text, textSize, propertiesProvider, mixColorByHitTestState, textMeasurer, maxWidth, maxHeight) {
			this.shouldCorrectAngle = shouldCorrectAngle;
			this.angleOnCircleDegree = CancelAngle(angleOnCircleDegree);
			SetTextAngle(CalculateActualTextAngle());
		}
		protected override Point CalculateLeftTopPoint() {
			return new Point(BasePoint.X - Width / 2, BasePoint.Y - Height / 2);
		}
		protected override TextRotation CalculateRotation() {
			return TextRotation.CenterCenter;
		}
		protected abstract float CalculateActualTextAngle();
	}
}
