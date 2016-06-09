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
	public class RotatedTextPainterOnCircleTangent : RotatedTextPainterOnCircle {
		bool TopHalfCircle { get { return IsTopHalfCircle(AngleOnCircleDegree); } }
		public RotatedTextPainterOnCircleTangent(float angleOnCircleDegree, Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, bool shouldCorrectAngle)
			: this(angleOnCircleDegree, basePoint, text, textSize, propertiesProvider, mixColorByHitTestState, shouldCorrectAngle, false, null) {
		}
		public RotatedTextPainterOnCircleTangent(float angleOnCircleDegree, Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, bool shouldCorrectAngle, bool parseText, TextMeasurer textMeasurer)
			: base(angleOnCircleDegree, basePoint, text, textSize, propertiesProvider, mixColorByHitTestState, shouldCorrectAngle, parseText, textMeasurer) {
		}
		public RotatedTextPainterOnCircleTangent(float angleOnCircleDegree, Point basePoint, string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, bool shouldCorrectAngle, TextMeasurer textMeasurer, int maxWidth, int maxHeight)
			: base(angleOnCircleDegree, basePoint, text, textSize, propertiesProvider, mixColorByHitTestState, shouldCorrectAngle, textMeasurer, maxWidth, maxHeight) {
		}
		protected override float CalculateActualTextAngle() {
			return ShouldCorrectAngle && !TopHalfCircle ? AngleOnCircleDegree - 180 : AngleOnCircleDegree;
		}
	}
}
