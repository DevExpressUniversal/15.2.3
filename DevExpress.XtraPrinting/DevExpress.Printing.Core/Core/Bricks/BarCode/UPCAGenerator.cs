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

using DevExpress.XtraPrinting;
using System;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.BarCode.Native;
using System.ComponentModel;
namespace DevExpress.XtraPrinting.BarCode {
	public class UPCAGenerator : EAN13Generator {
		protected static StringFormat sfLeft = StringFormat.GenericTypographic.Clone() as StringFormat;
		#region static XRUPCAGenerator()
		static UPCAGenerator() {
			sfLeft.Alignment = StringAlignment.Near;
			sfLeft.LineAlignment = StringAlignment.Center;
		}
		#endregion
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("UPCAGeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get {
				return BarCodeSymbology.UPCA;
			}
		}
		public UPCAGenerator() {
		}
		protected UPCAGenerator(UPCAGenerator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new UPCAGenerator(this);
		}
		protected override float GetLeftSpacing(IBarCodeData data, IGraphicsBase gr) {
			return MeasureCharWidth(FinalText[1], data, sfRight, gr) + GetSpacing(gr);
		}
		protected override float GetRightSpacing(IBarCodeData data, IGraphicsBase gr) {
			return MeasureCharWidth(CalcCheckDigit(FinalText), data, sfLeft, gr) + GetSpacing(gr);
		}
		protected override void DrawText(IGraphicsBase gr, RectangleF bounds, IBarCodeData data) {
			string firstDigit = new string(FinalText[1], 1);
			string leftPart = FinalText.Substring(2, 5);
			string rightPart = FinalText.Substring(7, 5);
			string checkDigit = new string(CalcCheckDigit(FinalText), 1);
			RectangleF leftBounds = bounds;
			leftBounds.Width /= 2;
			RectangleF rightBounds = leftBounds;
			rightBounds.X = leftBounds.Right;
			RectangleF firstDigitBounds = bounds;
			firstDigitBounds.Width = GetLeftSpacing(data, gr);
			firstDigitBounds.X -= firstDigitBounds.Width;
			RectangleF checkDigitBounds = bounds;
			checkDigitBounds.Width = GetRightSpacing(data, gr) - GetSpacing(gr);
			checkDigitBounds.X = bounds.Right + GetSpacing(gr);
			gr.DrawString(leftPart, data.Style.Font, gr.GetBrush(data.Style.ForeColor), leftBounds, sfCenter);
			gr.DrawString(rightPart, data.Style.Font, gr.GetBrush(data.Style.ForeColor), rightBounds, sfCenter);
			gr.DrawString(firstDigit, data.Style.Font, gr.GetBrush(data.Style.ForeColor), firstDigitBounds, sfRight);
			gr.DrawString(checkDigit, data.Style.Font, gr.GetBrush(data.Style.ForeColor), checkDigitBounds, sfLeft);
		}
		protected override string FormatText(string text) {
			if(text.Length >= 11)
				return FormatText(text.Substring(0, 11), 12);
			else
				return FormatText(text, 12);
		}
	}
}
