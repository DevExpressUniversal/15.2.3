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
	public class EAN8Generator : EAN13Generator {
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("EAN8GeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get {
				return BarCodeSymbology.EAN8;
			}
		}
		public EAN8Generator() {
		}
		protected EAN8Generator(EAN8Generator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new EAN8Generator(this);
		}
		protected override float GetLeftSpacing(IBarCodeData data, IGraphicsBase gr) {
			return 0.0f;
		}
		protected override void DrawText(IGraphicsBase gr, RectangleF bounds, IBarCodeData data) {
			string leftPart = FinalText.Substring(0, 4);
			string rightPart = FinalText.Substring(4, 3) + CalcCheckDigit(FinalText);
			RectangleF leftBounds = bounds;
			leftBounds.Width /= 2;
			RectangleF rightBounds = leftBounds;
			rightBounds.X = leftBounds.Right;
			gr.DrawString(leftPart, data.Style.Font, gr.GetBrush(data.Style.ForeColor), leftBounds, sfCenter);
			gr.DrawString(rightPart, data.Style.Font, gr.GetBrush(data.Style.ForeColor), rightBounds, sfCenter);
		}
		protected override int GetMiddleIndex() {
			return 5;
		}
		protected override int[] GetGuardBarsBounds() {
			return new int[] { 3, 19, 24, 40 };
		}
		protected override string FormatText(string text) {
			return FormatText(text, 7);
		}
		protected override char[] PrepareText(string text) {
			return base.PrepareText('0' + text);
		}
	}
}
