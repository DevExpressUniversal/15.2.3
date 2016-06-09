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
using System.Collections.Generic;
using System.IO;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocConstants
	public static class DocConstants {
		public const int CharacterPositionSize = 4;
		public const int LastByteOffsetInSector = 511;
		public const int MaxXASValue = 31680;
		public static Color[] DefaultMSWordColor = {	 DXColor.Empty,
														 Color.FromArgb(255, 0, 0, 0),
														 Color.FromArgb(255, 0, 0, 255),
														 Color.FromArgb(255, 0, 255, 255),
														 Color.FromArgb(255, 0, 255, 0),
														 Color.FromArgb(255, 255, 0, 255),
														 Color.FromArgb(255, 255, 0, 0),
														 Color.FromArgb(255, 255, 255, 0),
														 Color.FromArgb(255, 255, 255, 255),
														 Color.FromArgb(255, 0, 0, 128),
														 Color.FromArgb(255, 0, 128, 128),
														 Color.FromArgb(255, 0, 128, 0),
														 Color.FromArgb(255, 128, 0, 128),
														 Color.FromArgb(255, 128, 0, 0),
														 Color.FromArgb(255, 128, 128, 0),
														 Color.FromArgb(255, 128, 128, 128),
														 Color.FromArgb(255, 192, 192, 192)
													 };
	}
	#endregion
	#region TextRunStartReasons
	[Flags]
	public enum TextRunStartReasons {
		TextRunMark = 0x0001,
		ParagraphMark = 0x0002,
		SectionMark = 0x0004,
		ColumnBreak = 0x0008,
		TableUnitMark = 0x0010
	}
	#endregion
	#region TextRunBorder
	public class TextRunBorder {
		#region Fields
		int offset;
		TextRunStartReasons reason;
		#endregion
		public TextRunBorder(int offset, TextRunStartReasons reason) {
			this.offset = offset;
			this.reason = reason;
		}
		#region Properties
		public int Offset { get { return this.offset; } }
		public TextRunStartReasons Reason { get { return this.reason; } set { this.reason = value; } }
		#endregion
	}
	#endregion
	#region TextRunBorderComparable
	public class TextRunBorderComparable : IComparable<TextRunBorder> {
		readonly int offset;
		public TextRunBorderComparable(int offset) {
			this.offset = offset;
		}
		public int Offset { get { return offset; } }
		#region IComparable<TextRunBorder> Members
		public int CompareTo(TextRunBorder textRunBorder) {
			return textRunBorder.Offset - Offset;
		}
		#endregion
	}
	#endregion
	#region DocCharacterFormattingHelper
	public static class DocCharacterFormattingHelper {
		public static CharacterFormattingInfo GetMergedCharacterFormattingInfo(DocCharacterFormattingInfo info, CharacterFormattingInfo parentInfo) {
			CharacterFormattingInfo result = parentInfo.Clone();
			result.AllCaps = ConvertFromDocBoolWrapper(result.AllCaps, info.AllCaps);
			result.BackColor = info.BackColor;
			result.FontBold = ConvertFromDocBoolWrapper(result.FontBold, info.FontBold);
			result.FontItalic = ConvertFromDocBoolWrapper(result.FontItalic, info.FontItalic);
			result.FontName = info.FontName;
			result.DoubleFontSize = info.DoubleFontSize;
			result.FontStrikeoutType = CalcStriceoutType(info.Strike, info.DoubleStrike, result.FontStrikeoutType);
			result.FontUnderlineType = info.FontUnderlineType;
			result.ForeColor = info.ForeColor;
			result.Hidden = ConvertFromDocBoolWrapper(result.Hidden, info.Hidden);
			result.Script = info.Script;
			result.StrikeoutColor = info.StrikeoutColor;
			result.StrikeoutWordsOnly = info.StrikeoutWordsOnly;
			result.UnderlineColor = info.UnderlineColor;
			result.UnderlineWordsOnly = info.UnderlineWordsOnly;
			return result;
		}
		static bool ConvertFromDocBoolWrapper(bool value, DocBoolWrapper boolWrapper) {
			switch (boolWrapper) {
				case DocBoolWrapper.False: return false;
				case DocBoolWrapper.True: return true;
				case DocBoolWrapper.Leave: return value;
				case DocBoolWrapper.Inverse: return !value;
			}
			return false;
		}
		static StrikeoutType CalcStriceoutType(DocBoolWrapper strike, DocBoolWrapper doubleStrike, StrikeoutType baseStrikeout) {
			if (doubleStrike == DocBoolWrapper.True)
				return StrikeoutType.Double;
			if (strike == DocBoolWrapper.True)
				return StrikeoutType.Single;
			if (strike == DocBoolWrapper.Leave || doubleStrike == DocBoolWrapper.Leave)
				return baseStrikeout;
			if (doubleStrike == DocBoolWrapper.Inverse && baseStrikeout == StrikeoutType.None)
				return StrikeoutType.Double;
			if (strike == DocBoolWrapper.Inverse && baseStrikeout == StrikeoutType.None)
				return StrikeoutType.Single;
			return StrikeoutType.None;
		}
	}
	#endregion
	#region AlignmentCalculator
	public static class AlignmentCalculator {
		public static ParagraphAlignment CalcParagraphAlignment(byte alignmentCode) {
			switch (alignmentCode) {
				case 0: return ParagraphAlignment.Left;
				case 1: return ParagraphAlignment.Center;
				case 2: return ParagraphAlignment.Right;
				default: return ParagraphAlignment.Justify;
			}
		}
		public static byte CalcParagraphAlignmentCode(ParagraphAlignment alignment) {
			switch (alignment) {
				case ParagraphAlignment.Left: return 0;
				case ParagraphAlignment.Center: return 1;
				case ParagraphAlignment.Right: return 2;
				case ParagraphAlignment.Justify: return 3;
				default: return 0;
			}
		}
		public static ListNumberAlignment CalcListNumberAlignment(byte alignmentCode) {
			switch (alignmentCode) {
				case 0: return ListNumberAlignment.Left;
				case 1: return ListNumberAlignment.Center;
				case 2: return ListNumberAlignment.Right;
				default: return ListNumberAlignment.Left;
			}
		}
		public static byte CalcListNumberAlignmentCode(ListNumberAlignment alignment) {
			switch (alignment) {
				case ListNumberAlignment.Left: return 0;
				case ListNumberAlignment.Center: return 1;
				case ListNumberAlignment.Right: return 2;
				default: return 0;
			}
		}
		public static VerticalAlignment CalcVerticalAlignment(byte alignmentTypeCode) {
			switch (alignmentTypeCode) {
				case 0: return VerticalAlignment.Top;
				case 1: return VerticalAlignment.Center;
				case 2: return VerticalAlignment.Bottom;
				default: return VerticalAlignment.Both;
			}
		}
		public static byte CalcVerticalAlignmentTypeCode(VerticalAlignment alignment) {
			switch (alignment) {
				case VerticalAlignment.Top: return 0;
				case VerticalAlignment.Center: return 1;
				case VerticalAlignment.Bottom: return 2;
				default: return 0;
			}
		}
	}
	#endregion
	#region NumberingFormatCalculator
	public static class NumberingFormatCalculator {
		class NumberingFormatInfo {
			byte formatCode;
			NumberingFormat numberingFormat;
			public NumberingFormatInfo(byte formatCode, NumberingFormat numberingFormat) {
				this.formatCode = formatCode;
				this.numberingFormat = numberingFormat;
			}
			public byte FormatCode { get { return this.formatCode; } }
			public NumberingFormat NumberingFormat { get { return this.numberingFormat; } }
		}
		static List<NumberingFormatInfo> infos;
		static Dictionary<byte, NumberingFormat> numberingFormats;
		static Dictionary<NumberingFormat, byte> numberingFormatCodes;
		static NumberingFormatCalculator() {
			infos = new List<NumberingFormatInfo>(61);
			numberingFormatCodes = new Dictionary<NumberingFormat, byte>(61);
			numberingFormats = new Dictionary<byte, NumberingFormat>(61);
			infos.Add(new NumberingFormatInfo(0x00, NumberingFormat.Decimal));
			infos.Add(new NumberingFormatInfo(0x01, NumberingFormat.UpperRoman));
			infos.Add(new NumberingFormatInfo(0x02, NumberingFormat.LowerRoman));
			infos.Add(new NumberingFormatInfo(0x03, NumberingFormat.UpperLetter));
			infos.Add(new NumberingFormatInfo(0x04, NumberingFormat.LowerLetter));
			infos.Add(new NumberingFormatInfo(0x05, NumberingFormat.Ordinal));
			infos.Add(new NumberingFormatInfo(0x06, NumberingFormat.CardinalText));
			infos.Add(new NumberingFormatInfo(0x07, NumberingFormat.OrdinalText));
			infos.Add(new NumberingFormatInfo(0x08, NumberingFormat.Hex));
			infos.Add(new NumberingFormatInfo(0x09, NumberingFormat.Chicago));
			infos.Add(new NumberingFormatInfo(0x0a, NumberingFormat.IdeographDigital));
			infos.Add(new NumberingFormatInfo(0x0b, NumberingFormat.JapaneseCounting));
			infos.Add(new NumberingFormatInfo(0x0c, NumberingFormat.AIUEOHiragana));
			infos.Add(new NumberingFormatInfo(0x0d, NumberingFormat.Iroha));
			infos.Add(new NumberingFormatInfo(0x0e, NumberingFormat.DecimalFullWidth));
			infos.Add(new NumberingFormatInfo(0x0f, NumberingFormat.DecimalHalfWidth));
			infos.Add(new NumberingFormatInfo(0x10, NumberingFormat.JapaneseLegal));
			infos.Add(new NumberingFormatInfo(0x11, NumberingFormat.JapaneseDigitalTenThousand));
			infos.Add(new NumberingFormatInfo(0x12, NumberingFormat.DecimalEnclosedCircle));
			infos.Add(new NumberingFormatInfo(0x13, NumberingFormat.DecimalFullWidth2));
			infos.Add(new NumberingFormatInfo(0x14, NumberingFormat.AIUEOFullWidthHiragana));
			infos.Add(new NumberingFormatInfo(0x15, NumberingFormat.IrohaFullWidth));
			infos.Add(new NumberingFormatInfo(0x16, NumberingFormat.DecimalZero));
			infos.Add(new NumberingFormatInfo(0x17, NumberingFormat.Bullet));
			infos.Add(new NumberingFormatInfo(0x18, NumberingFormat.Ganada));
			infos.Add(new NumberingFormatInfo(0x19, NumberingFormat.Chosung));
			infos.Add(new NumberingFormatInfo(0x1a, NumberingFormat.DecimalEnclosedFullstop));
			infos.Add(new NumberingFormatInfo(0x1b, NumberingFormat.DecimalEnclosedParenthses));
			infos.Add(new NumberingFormatInfo(0x1c, NumberingFormat.DecimalEnclosedCircleChinese));
			infos.Add(new NumberingFormatInfo(0x1d, NumberingFormat.IdeographEnclosedCircle));
			infos.Add(new NumberingFormatInfo(0x1e, NumberingFormat.IdeographTraditional));
			infos.Add(new NumberingFormatInfo(0x1f, NumberingFormat.IdeographZodiac));
			infos.Add(new NumberingFormatInfo(0x20, NumberingFormat.IdeographZodiacTraditional));
			infos.Add(new NumberingFormatInfo(0x21, NumberingFormat.TaiwaneseCounting));
			infos.Add(new NumberingFormatInfo(0x22, NumberingFormat.IdeographLegalTraditional));
			infos.Add(new NumberingFormatInfo(0x23, NumberingFormat.TaiwaneseCountingThousand));
			infos.Add(new NumberingFormatInfo(0x24, NumberingFormat.TaiwaneseDigital));
			infos.Add(new NumberingFormatInfo(0x25, NumberingFormat.ChineseCounting));
			infos.Add(new NumberingFormatInfo(0x26, NumberingFormat.ChineseLegalSimplified));
			infos.Add(new NumberingFormatInfo(0x27, NumberingFormat.ChineseCountingThousand));
			infos.Add(new NumberingFormatInfo(0x28, NumberingFormat.Decimal));
			infos.Add(new NumberingFormatInfo(0x29, NumberingFormat.KoreanDigital));
			infos.Add(new NumberingFormatInfo(0x2a, NumberingFormat.KoreanCounting));
			infos.Add(new NumberingFormatInfo(0x2b, NumberingFormat.KoreanLegal));
			infos.Add(new NumberingFormatInfo(0x2c, NumberingFormat.KoreanDigital2));
			infos.Add(new NumberingFormatInfo(0x2d, NumberingFormat.Hebrew1));
			infos.Add(new NumberingFormatInfo(0x2e, NumberingFormat.ArabicAlpha));
			infos.Add(new NumberingFormatInfo(0x2f, NumberingFormat.Hebrew2));
			infos.Add(new NumberingFormatInfo(0x30, NumberingFormat.ArabicAbjad));
			infos.Add(new NumberingFormatInfo(0x31, NumberingFormat.HindiVowels));
			infos.Add(new NumberingFormatInfo(0x32, NumberingFormat.HindiConsonants));
			infos.Add(new NumberingFormatInfo(0x33, NumberingFormat.HindiNumbers));
			infos.Add(new NumberingFormatInfo(0x34, NumberingFormat.HindiDescriptive));
			infos.Add(new NumberingFormatInfo(0x35, NumberingFormat.ThaiLetters));
			infos.Add(new NumberingFormatInfo(0x36, NumberingFormat.ThaiNumbers));
			infos.Add(new NumberingFormatInfo(0x37, NumberingFormat.ThaiDescriptive));
			infos.Add(new NumberingFormatInfo(0x38, NumberingFormat.VietnameseDescriptive));
			infos.Add(new NumberingFormatInfo(0x39, NumberingFormat.NumberInDash));
			infos.Add(new NumberingFormatInfo(0x3a, NumberingFormat.RussianLower));
			infos.Add(new NumberingFormatInfo(0x3b, NumberingFormat.RussianUpper));
			infos.Add(new NumberingFormatInfo(0xff, NumberingFormat.None));
			int count = infos.Count;
			for (int i = 0; i < count; i++) {
				numberingFormats.Add(infos[i].FormatCode, infos[i].NumberingFormat);
				if (!numberingFormatCodes.ContainsKey(infos[i].NumberingFormat))
					numberingFormatCodes.Add(infos[i].NumberingFormat, infos[i].FormatCode);
			}
		}
		public static NumberingFormat CalcNumberingFormat(int numberingFormatCode) {
			NumberingFormat numberingFormat;
			if (!numberingFormats.TryGetValue((byte)numberingFormatCode, out numberingFormat))
				numberingFormat = NumberingFormat.Decimal;
			return numberingFormat;
		}
		public static byte CalcNumberingFormatCode(NumberingFormat numberingFormat) {
			return numberingFormatCodes[numberingFormat];
		}
	}
	#endregion
	#region WidthUnitCalculator
	public static class WidthUnitCalculator {
		public static WidthUnitType CalcWidthUnitType(byte typeCode) {
			switch (typeCode) {
				case 0x00: return WidthUnitType.Nil;
				case 0x01: return WidthUnitType.Auto;
				case 0x02: return WidthUnitType.FiftiethsOfPercent;
				case 0x03: return WidthUnitType.ModelUnits;
				case 0x13: return WidthUnitType.ModelUnits;
				default: return WidthUnitType.Nil;
			}
		}
		public static byte CalcWidthUnitTypeCode(WidthUnitType unitType) {
			switch (unitType) {
				case WidthUnitType.Auto: return 1;
				case WidthUnitType.FiftiethsOfPercent: return 2;
				case WidthUnitType.ModelUnits: return 3;
				case WidthUnitType.Nil: return 0;
				default: return 0;
			}
		}
	}
	#endregion
	#region MergingStateCalculator
	public static class MergingStateCalculator {
		public static MergingState CalcHorizontalMergingState(byte typeCode) {
			switch (typeCode) {
				case 1: return MergingState.Restart;
				case 2: return MergingState.Continue;
				case 3: return MergingState.Continue;
				default: return MergingState.None;
			}
		}
		public static byte CalcHorizontalMergingTypeCode(MergingState mergingState) {
			switch (mergingState) {
				case MergingState.None: return 0;
				case MergingState.Continue: return 3;
				case MergingState.Restart: return 1;
				default: return 0;
			}
		}
		public static MergingState CalcVerticalMergingState(byte typeCode) {
			switch (typeCode) {
				case 1: return MergingState.Continue;
				case 2: return MergingState.Restart;
				case 3: return MergingState.Restart;
				default: return MergingState.None;
			}
		}
		public static byte CalcVerticalMergingTypeCode(MergingState mergingState) {
			switch (mergingState) {
				case MergingState.None: return 0;
				case MergingState.Continue: return 1;
				case MergingState.Restart: return 3;
				default: return 0;
			}
		}
	}
	#endregion
	#region TexDirectionCalculator
	public static class TextDirectionCalculator {
		public static TextDirection CalcTextDirection(byte typeCode) {
			switch (typeCode) {
				case 0: return TextDirection.LeftToRightTopToBottom;
				case 1: return TextDirection.TopToBottomRightToLeftRotated;
				case 3: return TextDirection.LeftToRightTopToBottomRotated;
				case 4: return TextDirection.LeftToRightTopToBottomRotated;
				case 5: return TextDirection.LeftToRightTopToBottomRotated;
				default: return TextDirection.LeftToRightTopToBottom;
			}
		}
	}
	#endregion
	#region FootNotePositionCalculator
	public static class FootNotePositionCalculator {
		public static FootNotePosition CalcFootNotePosition(int typeCode) {
			switch (typeCode) {
				case 0: return FootNotePosition.EndOfSection;
				case 1: return FootNotePosition.BottomOfPage;
				case 2: return FootNotePosition.BelowText;
				case 3: return FootNotePosition.EndOfDocument;
				default: return FootNotePosition.BottomOfPage;
			}
		}
		public static byte CalcFootNotePositionTypeCode(FootNotePosition position) {
			switch (position) {
				case FootNotePosition.EndOfSection: return 0;
				case FootNotePosition.BottomOfPage: return 1;
				case FootNotePosition.BelowText: return 2;
				case FootNotePosition.EndOfDocument: return 3;
				default: return 0;
			}
		}
		public static byte CalcFootNotePositionTypeCodeForDocumentProperties(FootNotePosition position) {
			switch (position) {
				case FootNotePosition.EndOfSection: return 0;
				case FootNotePosition.BottomOfPage: return 1;
				case FootNotePosition.BelowText: return 2;
				default: return 0;
			}
		}
	}
	#endregion
	#region FootNoteNumberingRestartCalculator
	public static class FootNoteNumberingRestartCalculator {
		public static LineNumberingRestart CalcFootNoteNumberingRestart(int typeCode) {
			switch (typeCode) {
				case 0: return LineNumberingRestart.Continuous;
				case 1: return LineNumberingRestart.NewSection;
				case 2: return LineNumberingRestart.NewPage;
				default: return LineNumberingRestart.Continuous;
			}
		}
		public static byte CalcFootNoteNumberingRestartTypeCode(LineNumberingRestart numberingRestart) {
			switch (numberingRestart) {
				case LineNumberingRestart.Continuous: return 0;
				case LineNumberingRestart.NewSection: return 1;
				case LineNumberingRestart.NewPage: return 2;
				default: return 0;
			}
		}
	}
	#endregion
	#region DocFloatingObjectHorizontalPositionTypeCalculator
	public static class DocFloatingObjectHorizontalPositionTypeCalculator {
		public static FloatingObjectHorizontalPositionType CalcHorizontalPositionType97(int typeCode) {
			switch (typeCode) {
				case 1: return FloatingObjectHorizontalPositionType.Page;
				case 2: return FloatingObjectHorizontalPositionType.Column;
				default: return FloatingObjectHorizontalPositionType.Margin;
			}
		}
		public static byte CalcHorizontalPositionTypeCode97(FloatingObjectHorizontalPositionType type) {
			switch (type) {
				case FloatingObjectHorizontalPositionType.Page: return 1;
				case FloatingObjectHorizontalPositionType.Column: return 2;
				default: return 0;
			}
		}
	}
	#endregion
	#region DocFloatingObjectVerticalPositionTypeCalculator
	public static class DocFloatingObjectVerticalPositionTypeCalculator {
		public static FloatingObjectVerticalPositionType CalcVerticalPositionType97(int typeCode) {
			switch (typeCode) {
				case 1: return FloatingObjectVerticalPositionType.Page;
				case 2: return FloatingObjectVerticalPositionType.Paragraph;
				default:
					return FloatingObjectVerticalPositionType.Margin;
			}
		}
		public static byte CalcVerticalPositionTypeCode97(FloatingObjectVerticalPositionType type) {
			switch (type) {
				case FloatingObjectVerticalPositionType.Page: return 1;
				case FloatingObjectVerticalPositionType.Paragraph: return 2;
				default: return 0;
			}
		}
	}
	#endregion
	#region DocFloatingObjectTextWrapTypeCalculator
	public static class DocFloatingObjectTextWrapTypeCalculator {
		const FloatingObjectTextWrapType wrapTypeBehindText = (FloatingObjectTextWrapType)(-1);
		public static FloatingObjectTextWrapType WrapTypeBehindText { get { return wrapTypeBehindText; } }
		public static FloatingObjectTextWrapType CalcTextWrapType(int typeCode) {
			switch (typeCode) {
				case 1: return FloatingObjectTextWrapType.TopAndBottom;
				case 2: return FloatingObjectTextWrapType.Square;
				case 3: return wrapTypeBehindText; 
				case 4: return FloatingObjectTextWrapType.Tight;
				case 5: return FloatingObjectTextWrapType.Through;
				default: return FloatingObjectTextWrapType.None;
			}
		}
		public static byte CalcTextWrapTypeCode(FloatingObjectTextWrapType type) {
			switch (type) {
				case FloatingObjectTextWrapType.TopAndBottom: return 1;
				case FloatingObjectTextWrapType.Square: return 2;
				case wrapTypeBehindText: return 3;
				case FloatingObjectTextWrapType.Tight: return 4;
				case FloatingObjectTextWrapType.Through: return 5;
				default: return 0;
			}
		}
	}
	#endregion
	#region DocFloatingObjectTextWrapSideCalculator
	public static class DocFloatingObjectTextWrapSideCalculator {
		public static FloatingObjectTextWrapSide CalcTextWrapSide(int typeCode) {
			switch (typeCode) {
				case 1: return FloatingObjectTextWrapSide.Left;
				case 2: return FloatingObjectTextWrapSide.Right;
				case 3: return FloatingObjectTextWrapSide.Largest;
				default: return FloatingObjectTextWrapSide.Both;
			}
		}
		public static byte CalcTextWrapSideTypeCode(FloatingObjectTextWrapSide wrapSide) {
			switch (wrapSide) {
				case FloatingObjectTextWrapSide.Both: return 0;
				case FloatingObjectTextWrapSide.Left: return 1;
				case FloatingObjectTextWrapSide.Right: return 2;
				case FloatingObjectTextWrapSide.Largest: return 3;
				default: return 0;
			}
		}
	}
	#endregion
	#region DocColorReference
	public class DocColorReference {
		#region static
		public static DocColorReference FromByteArray(byte[] data, int startIndex) {
			DocColorReference result = new DocColorReference();
			result.Read(data, startIndex);
			return result;
		}
		#endregion
		#region Fields
		public const int ColorReferenceSize = 4;
		Color color;
		#endregion
		public DocColorReference() {
			this.color = DXColor.Empty;
		}
		#region Properties
		public Color Color {
			get { return color; }
			set { color = value; }
		}
		#endregion
		protected void Read(byte[] data, int startIndex) {
			int red = data[startIndex];
			int green = data[startIndex + 1];
			int blue = data[startIndex + 2];
			int auto = data[startIndex + 3];
			if (auto != 0xff)
				this.color = DXColor.FromArgb(red, green, blue);
		}
		public byte[] GetBytes() {
			byte auto = (DXColor.IsTransparentOrEmpty(Color)) ? (byte)0xff : (byte)0x00;
			return new byte[] { Color.R, Color.G, Color.B, auto };
		}
	}
	#endregion
	public class DocTableBorderColorReference {
		#region static
		public static DocTableBorderColorReference FromByteArray(byte[] data, int startIndex) {
			DocTableBorderColorReference result = new DocTableBorderColorReference();
			result.Read(data, startIndex);
			return result;
		}
		#endregion
		#region Fields
		public const int ColorReferenceSize = 4;
		Color color;
		#endregion
		#region Properties
		public Color Color {
			get { return color; }
			set { color = value; }
		}
		#endregion
		protected void Read(byte[] data, int startIndex) {
			int red = data[startIndex];
			int green = data[startIndex + 1];
			int blue = data[startIndex + 2];
			int auto = data[startIndex + 3];
			if (auto == 0xff && red == 0xff && green == 0xff && blue == 0xff)
				this.color = DXColor.Empty;
			else
				this.Color = DXColor.FromArgb(red, green, blue);
		}
		public byte[] GetBytes() {
			byte auto = (DXColor.IsTransparentOrEmpty(Color)) ? (byte)0xff : (byte)0x00;
			return new byte[] { Color.R, Color.G, Color.B, auto };
		}
	}
	#region DocShadingDescriptor
	public class DocShadingDescriptor {
		#region static
		public static DocShadingDescriptor FromByteArray(byte[] data, int startIndex) {
			DocShadingDescriptor result = new DocShadingDescriptor();
			result.Read(data, startIndex);
			return result;
		}
		#endregion
		public DocShadingDescriptor() {
			ForeColor = DXColor.Empty;
			BackgroundColor = DXColor.Empty;
		}
		#region Properties
		public const byte Size = 0x0a;
		public Color ForeColor { get; set; }
		public Color BackgroundColor { get; set; }
		public short ShadingPattern { get; set; }
		#endregion
		protected void Read(byte[] data, int startIndex) {
			int red = data[startIndex];
			int green = data[startIndex + 1];
			int blue = data[startIndex + 2];
			int auto = data[startIndex + 3];
			ForeColor = (auto != 0xff) ? DXColor.FromArgb(red, green, blue) : DXColor.Empty;
			red = data[startIndex + 4];
			green = data[startIndex + 5];
			blue = data[startIndex + 6];
			auto = data[startIndex + 7];
			BackgroundColor = (auto != 0xff) ? DXColor.FromArgb(red, green, blue) : DXColor.Empty;
			ShadingPattern = BitConverter.ToInt16(data, startIndex + 8);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			byte auto = (DXColor.IsTransparentOrEmpty(ForeColor)) ? (byte)0xff : (byte)0x00;
			writer.Write(new byte[] { ForeColor.R, ForeColor.G, ForeColor.B, auto });
			auto = (DXColor.IsTransparentOrEmpty(BackgroundColor)) ? (byte)0xff : (byte)0x00;
			writer.Write(new byte[] { BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, auto });
			writer.Write(ShadingPattern);
		}
	}
	#endregion
	#region DocShadingDescriptor80
	public class DocShadingDescriptor80 {
		#region static
		public static DocShadingDescriptor80 FromByteArray(byte[] data) {
			return FromByteArray(data, 0);
		}
		public static DocShadingDescriptor80 FromByteArray(byte[] data, int startIndex) {
			DocShadingDescriptor80 result = new DocShadingDescriptor80();
			result.Read(data, startIndex);
			return result;
		}
		#endregion
		public DocShadingDescriptor80() {
			ForeColor = DXColor.Empty;
			BackgroundColor = DXColor.Empty;
		}
		#region Properties
		public Color ForeColor { get; set; }
		public Color BackgroundColor { get; set; }
		public short ShadingPattern { get; set; }
		#endregion
		protected void Read(byte[] data, int startIndex) {
			short info = BitConverter.ToInt16(data, startIndex);
			byte colorIndex = (byte)(info & 0x1f);
			if (colorIndex < DocConstants.DefaultMSWordColor.Length)
				ForeColor = (colorIndex != 0) ? DocConstants.DefaultMSWordColor[colorIndex] : DXColor.Empty;
			colorIndex = (byte)((info & 0x03e0) >> 5);
			if (colorIndex < DocConstants.DefaultMSWordColor.Length)
				BackgroundColor = (colorIndex != 0) ? DocConstants.DefaultMSWordColor[colorIndex] : DXColor.Empty;
			ShadingPattern = (short)((info & 0xfc00) >> 10);
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			short info;
			byte colorIndex = (byte)Array.BinarySearch(DocConstants.DefaultMSWordColor, ForeColor);
			if (colorIndex < 0)
				colorIndex = 0;
			info = colorIndex;
			colorIndex = (byte)Array.BinarySearch(DocConstants.DefaultMSWordColor, BackgroundColor);
			if (colorIndex < 0)
				colorIndex = 0;
			info = (short)((ushort)info | (colorIndex << 5));
			info = (short)((ushort)info | (ushort)ShadingPattern);
			writer.Write(BitConverter.GetBytes(info));
		}
	}
	#endregion
	#region DocumentProtectionTypeCalculator
	public static class DocumentProtectionTypeCalculator {
		public static DocumentProtectionType CalcDocumentProtectionType(short protectionTypeCode) {
			switch (protectionTypeCode) {
				case 0x7: return DocumentProtectionType.None;
				case 0x3: return DocumentProtectionType.ReadOnly;
				default: return DocumentProtectionType.None;
			}
		}
		public static short CalcDocumentProtectionTypeCode(DocumentProtectionType type) {
			switch (type) {
				case DocumentProtectionType.None: return 0x7;
				case DocumentProtectionType.ReadOnly: return 0x3;
				default: return 0x3;
			}
		}
		public static DocumentProtectionType CalcRangePermissionProtectionType(short protectionTypeCode) {
			switch (protectionTypeCode) {
				case 0x0004: return DocumentProtectionType.ReadOnly;
				default: return DocumentProtectionType.None;
			}
		}
		public static short CalcRangePermissionProtectionTypeCode(DocumentProtectionType type) {
			switch (type) {
				case DocumentProtectionType.None: return 0x0001;
				case DocumentProtectionType.ReadOnly: return 0x0004;
				default: return 0x0000;					
			}
		}
	}
	#endregion
}
