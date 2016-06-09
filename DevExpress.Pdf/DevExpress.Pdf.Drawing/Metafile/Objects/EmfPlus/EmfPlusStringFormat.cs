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

namespace DevExpress.Pdf.Drawing {
	public class EmfPlusStringFormat {
		static PdfHotkeyPrefix ConvertHotkeyPrefix(int value) {
			EmfPlusHotkeyPrefix hotkeyPrefix = (EmfPlusHotkeyPrefix)value;
			switch (hotkeyPrefix) {
				case EmfPlusHotkeyPrefix.Hide:
					return PdfHotkeyPrefix.Hide;
				default:
					return PdfHotkeyPrefix.None;
			}
		}
		static PdfStringAlignment ConvertAligment(int value) {
			EmfPlusStringAlignment aligment = (EmfPlusStringAlignment)value;
			switch (aligment) {
				case EmfPlusStringAlignment.Center:
					return PdfStringAlignment.Center;
				case EmfPlusStringAlignment.Far:
					return PdfStringAlignment.Far;
				default:
					return PdfStringAlignment.Near;
			}
		}
		static PdfStringTrimming ConvertTrimming(int value) {
			EmfPlusStringTrimming trimming = (EmfPlusStringTrimming)value;
			switch (trimming) {
				case EmfPlusStringTrimming.Character:
					return PdfStringTrimming.Character;
				case EmfPlusStringTrimming.EllipsisCharacter:
					return PdfStringTrimming.EllipsisCharacter;
				case EmfPlusStringTrimming.EllipsisWord:
					return PdfStringTrimming.EllipsisWord;
				case EmfPlusStringTrimming.Word:
					return PdfStringTrimming.Word;
				default:
					return PdfStringTrimming.None;
			}
		}
		static PdfStringFormatFlags ConvertStringFormatFlags(int value) {
			EmfPlusStringFormatFlags flags = (EmfPlusStringFormatFlags)value;
			PdfStringFormatFlags result = (PdfStringFormatFlags)0;
			if (flags.HasFlag(EmfPlusStringFormatFlags.StringFormatLineLimit))
				result |= PdfStringFormatFlags.LineLimit;
			if (flags.HasFlag(EmfPlusStringFormatFlags.StringFormatNoClip))
				result |= PdfStringFormatFlags.NoClip;
			if (flags.HasFlag(EmfPlusStringFormatFlags.StringFormatNoWrap))
				result |= PdfStringFormatFlags.NoWrap;
			if (flags.HasFlag(EmfPlusStringFormatFlags.StringFormatMeasureTrailingSpaces))
				result |= PdfStringFormatFlags.MeasureTrailingSpaces;
			return result;
		}
		readonly PdfStringFormat format;
		public PdfStringFormat Format { get { return format; } }
		public EmfPlusStringFormat(EmfPlusReader reader) {
			reader.ReadInt32();
			PdfStringFormatFlags formatFlags = ConvertStringFormatFlags(reader.ReadInt32());
			format = new PdfStringFormat(formatFlags);
			reader.ReadInt32();
			format.Alignment = ConvertAligment(reader.ReadInt32());
			format.LineAlignment = ConvertAligment(reader.ReadInt32());
			reader.ReadBytes(12);
			format.HotkeyPrefix = ConvertHotkeyPrefix(reader.ReadInt32());
			format.LeadingMarginFactor = reader.ReadSingle();
			format.TrailingMarginFactor = reader.ReadSingle();
			reader.ReadSingle();
			format.Trimming = ConvertTrimming(reader.ReadInt32());
		}
	}
	enum EmfPlusStringAlignment {
		Near = 0,
		Center = 1,
		Far = 2
	}
	enum EmfPlusStringTrimming {
		None = 0,
		Character = 1,
		Word = 2,
		EllipsisCharacter = 3,
		EllipsisWord = 4,
		EllipsisPath = 5
	}
	enum EmfPlusHotkeyPrefix {
		None = 0,
		Show = 1,
		Hide = 2
	}
}
