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
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BarCode.Native;
using System.Collections.Generic;
#if WINRT || WP
using DevExpress.Collections;
#endif
namespace DevExpress.XtraPrinting.BarCode {
	public class IntelligentMailPackageGenerator : EAN128Generator {
		const int allowedLengthShort = 22;
		const int allowedLengthShortWithoutCheckDigit = 21;
		const int allowedLengthLong = 26;
		const int allowedLengthLongWithoutCheckDigit = 25;
		static readonly float quietZone = GraphicsUnitConverter.Convert(0.25f, GraphicsUnit.Inch, GraphicsUnit.Document); 
		static readonly float lineWidth = GraphicsUnitConverter.Convert(0.04f, GraphicsUnit.Inch, GraphicsUnit.Document); 
		static readonly float smallClearance = GraphicsUnitConverter.Convert(0.031f, GraphicsUnit.Inch, GraphicsUnit.Document); 
		static readonly float bigClearance = GraphicsUnitConverter.Convert(0.125f, GraphicsUnit.Inch, GraphicsUnit.Document); 
		static readonly float textHeight = GraphicsUnitConverter.Convert(0.125f, GraphicsUnit.Inch, GraphicsUnit.Document); 
		static readonly float barHeight = GraphicsUnitConverter.Convert(0.75f, GraphicsUnit.Inch, GraphicsUnit.Document); 
		static readonly float barCodeHeight = GraphicsUnitConverter.Convert(1.392f, GraphicsUnit.Inch, GraphicsUnit.Document); 
		public override BarCodeSymbology SymbologyCode {
			get {
				return BarCodeSymbology.IntelligentMailPackage;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool HumanReadableText {
			get { return true; }
			set { }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool AddLeadingZero {
			get { return false; }
			set { }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override DevExpress.XtraPrinting.BarCode.Code128Charset CharacterSet {
			get { return DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetC; }
			set { }
		}
		public IntelligentMailPackageGenerator() {
			CharacterSet = Code128Charset.CharsetC;
		}
		protected IntelligentMailPackageGenerator(IntelligentMailPackageGenerator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new IntelligentMailPackageGenerator(this);
		}
		protected override string MakeDisplayText(string text) {
			return text;
		}
		protected override bool IsValidTextFormat(string text) {
			if(!IsValidZIP(GetZIP(text)))
				return false;
			string textWithoutZIP = GetTextWithoutZIP(text);
			int lengthWithoutZIP = textWithoutZIP.Length;
			int ai;
			if(!int.TryParse(textWithoutZIP.Substring(0, 2), out ai))
				return false;
			return IsValidAIWithShortLength(ai, lengthWithoutZIP) || IsValidAIWithLongLength(ai, lengthWithoutZIP);
		}
		static bool IsValidZIP(string zip) {
			return zip.Length == 0 || zip.Length == 9 || zip.Length == 5;
		}
		static bool IsValidAIWithLongLength(int ai, int lengthWithoutZIP) {
			return ai >= 91 && ai <= 93 && (lengthWithoutZIP == allowedLengthLongWithoutCheckDigit || lengthWithoutZIP == allowedLengthLong);
		}
		static bool IsValidAIWithShortLength(int ai, int lengthWithoutZIP) {
			return ai >= 91 && ai <= 95 && (lengthWithoutZIP == allowedLengthShortWithoutCheckDigit || lengthWithoutZIP == allowedLengthShort);
		}
		protected override float CalcBarCodeHeight(ArrayList pattern, double module) {
			return barCodeHeight;
		}
		protected override float CalcBarCodeWidth(ArrayList pattern, double module) {
			return (2 * quietZone) + base.CalcBarCodeWidth(pattern, module);
		}
		protected override void JustifyBarcodeBounds(IBarCodeData data, ref float barCodeWidth, ref float barCodeHeight, ref RectangleF barBounds) {
		}
		protected override double CalcAutoModuleX(IBarCodeData data, RectangleF clientBounds, IGraphicsBase gr) {
			float barCodeWidth = clientBounds.Width;
			SizeF measureString = gr.MeasureString(GetHumanReadableText(FinalText), GetFont(), GraphicsUnit.Document);
			return ((double)(measureString.Width > barCodeWidth ? measureString.Width : barCodeWidth) - quietZone * 2) / ((double)CalcBarCodeWidth(Pattern, 1) - 2 * quietZone);
		}
		protected override bool IsValidText(string text) {
			return string.IsNullOrEmpty(text) ? false : base.IsValidText(text);
		}
		protected override string FormatText(string text) {
			string correctedText;
			if(GetTextWithoutZIP(text).Length % 2 == 0) {
				string textWithoutCheckDigit = text.Length > 0 ? text.Remove(text.Length - 1) : string.Empty;
				int checkDigit = CalculateCheckDigit(textWithoutCheckDigit);
				correctedText = textWithoutCheckDigit + checkDigit;
			} else
				correctedText = text + CalculateCheckDigit(text);
			return base.FormatText(correctedText);
		}
		string GetTextWithoutZIP(string text) {
			var aiElement = GS1Helper.GetAIElements(text, fnc1Char[0], FNC1Substitute).Where(element => element.AI != "420").FirstOrDefault();
			return aiElement.AI + aiElement.Value;
		}
		string GetZIP(string text) {
			var aiElement = GS1Helper.GetAIElements(text, fnc1Char[0], FNC1Substitute).FirstOrDefault();
			return aiElement.AI == "420" ? aiElement.Value : string.Empty;
		}
		int[] GetNumbersWithoutZIP(string text) {
			return Array.ConvertAll(GetTextWithoutZIP(text).ToCharArray(), c => (int)(c - '0'));
		}
		int GetSum(bool even, int[] numbers) {
			int sum = 0;
			for(int i = 0; i < numbers.Length; i++) {
				if((even && i % 2 == 0) || (!even && i % 2 != 0))
					sum += numbers[i];
			}
			return sum;
		}
		int CalculateCheckDigit(string text) {
			int[] numbers = GetNumbersWithoutZIP(text);
			Array.Reverse(numbers);
			int sum = GetSum(true, numbers);
			sum *= 3;
			sum += GetSum(false, numbers);
			int digit = 0;
			while(sum % 10 != 0) {
				sum++;
				digit++;
			}
			return digit;
		}
		string GetHumanReadableText(string text) {
			string textWithoutZIP = GetTextWithoutZIP(text);
			StringBuilder resultString = new StringBuilder();
			for(int i = 0; i < textWithoutZIP.Length; i++) {
				resultString.Append(textWithoutZIP[i]);
				if(i % 4 == 3)
					resultString.Append(" ");
			}
			return resultString.ToString();
		}
		static Font GetFont() {
			return new Font("Arial", 0.125f, FontStyle.Bold, GraphicsUnit.Inch);
		}
		static StringFormat GetStringFormat() {
			StringFormat strFormat = new StringFormat();
			strFormat.Alignment = StringAlignment.Center;
			strFormat.LineAlignment = StringAlignment.Center;
			strFormat.Trimming = StringTrimming.EllipsisCharacter;
			return strFormat;
		}
		float VerticalAlignHeight(float clientHeight, TextAlignment alignment) {
			StringAlignment sa = GraphicsConvertHelper.ToVertStringAlignment(alignment);
			if(sa == StringAlignment.Near)
				return 0;
			if(sa == StringAlignment.Center)
				return (clientHeight - barCodeHeight) / 2;
			return clientHeight - barCodeHeight;
		}
		protected override void DrawBarCode(IGraphicsBase gr, RectangleF barBounds, RectangleF textBounds, IBarCodeData data, float xModule, float yModule) {
			Brush brush = gr.GetBrush(data.Style.ForeColor);
			float verticalAlignHeight = VerticalAlignHeight(barBounds.Height, data.Alignment);
			float barsTop = barBounds.Top + lineWidth + smallClearance + textHeight + bigClearance + verticalAlignHeight;
			gr.FillRectangle(brush, barBounds.Left, barBounds.Top + verticalAlignHeight, barBounds.Width, lineWidth);
			RectangleF upperTextBounds = new RectangleF(barBounds.Left, barBounds.Top + lineWidth + smallClearance + verticalAlignHeight, barBounds.Width, textHeight);
			gr.DrawString("USPS TRACKING #", GetFont(), brush, upperTextBounds, GetStringFormat());
			float x = barBounds.Left + quietZone;
			for(int i = 0; i < Pattern.Count; i++) {
				float w = xModule * (float)Pattern[i];
				if(i % 2 == 0)
					gr.FillRectangle(brush, x, barsTop, w, barHeight);
				x += w;
			}
			RectangleF lowerTextBounds = new RectangleF(barBounds.Left, barsTop + barHeight + bigClearance, barBounds.Width, textHeight);
			gr.DrawString(GetHumanReadableText(FinalText), GetFont(), brush, lowerTextBounds, GetStringFormat());
			gr.FillRectangle(brush, barBounds.Left, lowerTextBounds.Top + textHeight + smallClearance, barBounds.Width, lineWidth);
			if(data.ShowText)
				DrawText(gr, textBounds, data);
		}
#if DEBUGTEST
		public int Test_CalculateCheckDigit(string text) {
			return CalculateCheckDigit(text);
		}
		public string Test_GetHumanReadableText(string text) {
			return GetHumanReadableText(text);
		}
#endif
	}
}
