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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.BarCode.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public class EAN13Generator : BarCodeGeneratorBase {
		#region static
		static Hashtable parityTable = new Hashtable();
		static string validCharSet = charSetDigits;
		internal static Hashtable charPattern = new Hashtable();
		protected static StringFormat sfCenter = StringFormat.GenericTypographic.Clone() as StringFormat;
		protected static StringFormat sfRight = StringFormat.GenericTypographic.Clone() as StringFormat;
		const int spacing = 10;
		static protected char CalcCheckDigit(string text) {
			return Industrial2of5Generator.CalcCheckDigit(text);
		}
		static protected float MeasureCharWidth(char ch, IBarCodeData data, StringFormat sf, IGraphicsBase gr) {
			SizeF size = gr.MeasureString(Convert.ToString(ch), data.Style.Font, PointF.Empty, sf, gr.PageUnit);
			return size.Width;
		}
		#endregion
		#region Fields & Properties
		[
		DefaultValue(true),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CalcCheckSum {
			get { return true; }
			set { }
		}
		internal bool ConvertSpacingUnits { get; set; }
		#endregion
		#region static XREAN13Generator()
		static EAN13Generator() {
			sfCenter.Alignment = StringAlignment.Center;
			sfCenter.LineAlignment = StringAlignment.Center;
			sfRight.Alignment = StringAlignment.Far;
			sfRight.LineAlignment = StringAlignment.Center;
			charPattern['0'] = "3211"; 
			charPattern['1'] = "2221"; 
			charPattern['2'] = "2122"; 
			charPattern['3'] = "1411"; 
			charPattern['4'] = "1132"; 
			charPattern['5'] = "1231"; 
			charPattern['6'] = "1114"; 
			charPattern['7'] = "1312"; 
			charPattern['8'] = "1213"; 
			charPattern['9'] = "3112"; 
			charPattern['a'] = "1123"; 
			charPattern['b'] = "1222"; 
			charPattern['c'] = "2212"; 
			charPattern['d'] = "1141"; 
			charPattern['e'] = "2311"; 
			charPattern['f'] = "1321"; 
			charPattern['g'] = "4111"; 
			charPattern['h'] = "2131"; 
			charPattern['i'] = "3121"; 
			charPattern['j'] = "2113"; 
			charPattern['*'] = "111"; 
			charPattern['|'] = "11111"; 
			charPattern['-'] = "11"; 
			charPattern['>'] = "112"; 
			charPattern['<'] = "111111"; 
			parityTable['0'] = " 000000"; 
			parityTable['1'] = " 00a0aa"; 
			parityTable['2'] = " 00aa0a"; 
			parityTable['3'] = " 00aaa0"; 
			parityTable['4'] = " 0a00aa"; 
			parityTable['5'] = " 0aa00a"; 
			parityTable['6'] = " 0aaa00"; 
			parityTable['7'] = " 0a0a0a"; 
			parityTable['8'] = " 0a0aa0"; 
			parityTable['9'] = " 0aa0a0"; 
		}
		#endregion
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("EAN13GeneratorSymbologyCode")]
#endif
public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.EAN13; }
		}
		public EAN13Generator() {
		}
		protected EAN13Generator(EAN13Generator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new EAN13Generator(this);
		}
		protected internal static string FormatText(string text, int fixedWidth) {
			int count = text.Length;
			if(count < fixedWidth)
				return new String('0', fixedWidth - count) + text;
			else
				return text.Substring(0, fixedWidth);
		}
		protected override string FormatText(string text) {
			return FormatText(text, 12);
		}
		protected virtual int GetMiddleIndex() {
			return 7;
		}
		protected virtual string GetParityString(char numberSystem) {
			return (string)parityTable[numberSystem];
		}
		protected override char[] PrepareText(string text) {
			if(CalcCheckSum)
				text += CalcCheckDigit(text);
			int count = text.Length;
			char[] chars = new char[count + 2];
			chars[0] = '*';
			string parity = GetParityString(text[0]);
			int middleIdnex = GetMiddleIndex();
			for(int i = 1, idx = 1; i < count; i++, idx++) {
				if(i == middleIdnex) {
					chars[idx] = '|';
					idx++;
				}
				char ch = (i < middleIdnex) ? (char)(Char2Int(text[i]) + (int)parity[i]) : text[i];
				chars[idx] = ch;
			}
			chars[count + 1] = '*';
			return chars;
		}
		protected override Hashtable GetPatternTable() {
			return charPattern;
		}
		protected override string GetValidCharSet() { return validCharSet; }
		protected float GetSpacing(IGraphicsBase graphics) {
			if (ConvertSpacingUnits)
				return GraphicsUnitConverter.Convert(spacing, GraphicsUnit.Document, graphics.PageUnit);
			else
				return spacing;
		}
		protected override float GetLeftSpacing(IBarCodeData data, IGraphicsBase gr) {
			return MeasureCharWidth(FinalText[0], data, sfRight, gr) + GetSpacing(gr);
		}
		protected override void DrawText(IGraphicsBase gr, RectangleF bounds, IBarCodeData data) {
			string firstDigit = new string(FinalText[0], 1);
			string leftPart = FinalText.Substring(1, 6);
			string rightPart = FinalText.Substring(7, 5) + CalcCheckDigit(FinalText);
			RectangleF leftBounds = bounds;
			leftBounds.Width /= 2;
			RectangleF rightBounds = leftBounds;
			rightBounds.X = leftBounds.Right;
			RectangleF firstDigitBounds = bounds;
			firstDigitBounds.Width = GetLeftSpacing(data, gr);
			firstDigitBounds.X -= firstDigitBounds.Width;
			gr.DrawString(leftPart, data.Style.Font, gr.GetBrush(data.Style.ForeColor), leftBounds, sfCenter);
			gr.DrawString(rightPart, data.Style.Font, gr.GetBrush(data.Style.ForeColor), rightBounds, sfCenter);
			gr.DrawString(firstDigit, data.Style.Font, gr.GetBrush(data.Style.ForeColor), firstDigitBounds, sfRight);
		}
		protected virtual int[] GetGuardBarsBounds() {
			return new int[] { 3, 27, 32, 56 };
		}
		protected override void DrawBarCode(IGraphicsBase gr, RectangleF barBounds, RectangleF textBounds, IBarCodeData data, float xModule, float yModule) {
			float x = barBounds.Left;
			int[] guardBounds = GetGuardBarsBounds();
			for(int i = 0; i < Pattern.Count; i++) {
				float w = xModule * (float)Pattern[i];
				if(i % 2 == 0) {
					float height = barBounds.Height;
					if(i < guardBounds[0] || (i >= guardBounds[1] && i < guardBounds[2]) || i >= guardBounds[3])
						height += textBounds.Height / 2;
					gr.FillRectangle(gr.GetBrush(data.Style.ForeColor), x, barBounds.Top, w, height);
				}
				x += w;
			}
			if(data.ShowText)
				DrawText(gr, textBounds, data);
		}
	}
}
