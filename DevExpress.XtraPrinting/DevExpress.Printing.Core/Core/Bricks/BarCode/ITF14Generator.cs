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
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BarCode.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public class ITF14Generator : Interleaved2of5Generator {
		const int quietZone = 10;
		[
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.ITF14Generator.WideNarrowRatio"),
		 ]
		public override float WideNarrowRatio {
			get { return base.WideNarrowRatio; }
			set { base.WideNarrowRatio = value; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ITF14GeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get {
				return BarCodeSymbology.ITF14;
			}
		}
		public ITF14Generator() {
		}
		protected ITF14Generator(ITF14Generator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new ITF14Generator(this);
		}
		protected override string FormatText(string text) {
			if(CalcCheckSum)
				text += Industrial2of5Generator.CalcCheckDigit(text);
			const int fixedNumberDigits = 14;
			return text.Length < fixedNumberDigits ? text.PadLeft(fixedNumberDigits, '0') : text.Substring(0, fixedNumberDigits);
		}
		protected override string MakeDisplayText(string text) {
			return string.Join(" ", text.Substring(0, 1), text.Substring(1, 2), text.Substring(3, 5), text.Substring(8, 5), text.Substring(13, 1));
		}
		protected override char[] PrepareText(string text) {
			return text.ToCharArray();
		}
		protected override float CalcBarCodeWidth(ArrayList pattern, double module) {
			return (float)(2 * quietZone * module) + base.CalcBarCodeWidth(pattern, module);
		}
		protected override void DrawBarCode(IGraphicsBase gr, RectangleF barBounds, RectangleF textBounds, IBarCodeData data, float xModule, float yModule) {
			Brush brush = gr.GetBrush(data.Style.ForeColor);
			float x = barBounds.Left;
			x += quietZone * xModule;
			for(int i = 0; i < Pattern.Count; i++) {
				float w = xModule * (float)Pattern[i];
				if(i % 2 == 0)
					gr.FillRectangle(brush, x, barBounds.Top + xModule, w, barBounds.Height - 2 * xModule);
				x += w;
			}
			gr.FillRectangle(brush, barBounds.Left, barBounds.Top, barBounds.Width, 2 * xModule);
			gr.FillRectangle(brush, barBounds.Left, barBounds.Top + barBounds.Height - 2 * xModule, barBounds.Width, 2 * xModule);
			if(data.ShowText)
				DrawText(gr, textBounds, data);
		}
	}
}
