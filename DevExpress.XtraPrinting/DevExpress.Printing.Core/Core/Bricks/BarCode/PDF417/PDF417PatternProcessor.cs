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
using System.Text;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraPrinting.BarCode;
namespace DevExpress.XtraPrinting.BarCode.Native {
	public abstract class PDF417PatternProcessor: IPatternProcessor {
		public static PDF417PatternProcessor CreateInstance(PDF417CompactionMode mode) {
			if(mode == PDF417CompactionMode.Binary)
				return new PDF417BinaryPatternProcessor();
			return new PDF417TextPatternProcessor();
		}
		#region fields
		int columns = PDF417Constants.MinColumnsCount;
		int rows = PDF417Constants.MinRowsCount;
		bool truncateSymbol = false;
		ErrorCorrectionLevel errorCorrectionLevel = ErrorCorrectionLevel.Level2;
		int[] encodedBuf = new int[PDF417Constants.MaxSizeOfBuffer];
		int[] errorCorrectionCodes = new int[PDF417Constants.MaxSizeOfBuffer];
		int[] encodedData = new int[PDF417Constants.MaxSizeOfBuffer];
		int countAferEncode;
		List<List<bool>> pattern = new List<List<bool>>();
		List<bool> currentRow = new List<bool>();
		#endregion
		protected PDF417PatternProcessor() {
		}
		#region properties
		[DefaultValue(PDF417Constants.MinColumnsCount)]
		public int Columns {
			get {
				return columns;
			}
			set {
				columns = Math.Min(Math.Max(PDF417Constants.MinColumnsCount, value), PDF417Constants.MaxColumnsCount);
				CalculatePattern();
			}
		}
		[DefaultValue(PDF417Constants.MinRowsCount)]
		public int Rows {
			get { return rows; }
			set {
				rows = Math.Min(Math.Max(PDF417Constants.MinRowsCount, value), PDF417Constants.MaxRowsCount);
				CalculatePattern();
			}
		}
		[DefaultValue(ErrorCorrectionLevel.Level2)]
		public ErrorCorrectionLevel ErrorCorrectionLevel {
			get { return errorCorrectionLevel; }
			set {
				errorCorrectionLevel = value;
				CalculatePattern();
			}
		}
		[DefaultValue(false)]
		public bool TruncateSymbol {
			get { return truncateSymbol; }
			set { truncateSymbol = value; }
		}
		#endregion
		protected abstract bool IsValidData(object data);
		protected abstract void SetData(object data);
		protected abstract int GetCountAfterEncode(int index, int correctionWordsCount);
		protected abstract void ErrorCorrection(int countAferEncode, int correctionWordsCount, int index, int[] encodedBuf, int[] encodedData);
		protected abstract int WriteDataInEncodedBuffer(int[] encodedBuf);
		protected void FillPattern() {
			pattern.Clear();
			int index = 0;
			for(int i = 0; i < rows; i++) {
				CreateStartOfRow(i);
				for(int j = 0; j < columns; j++)
					UpdatePattern(PDF417Constants.PDF417Bits[i % 3, encodedData[index++]]);
				CreateEndOfRow(i);
			}
		}
		protected void CalculatePattern() {
			int correctionWordsCount = GetCorrectionWordsCount();
			int index = WriteDataInEncodedBuffer(this.encodedBuf);
			this.countAferEncode = GetRows(GetCountAfterEncode(index, correctionWordsCount)) * columns;
			ErrorCorrection(this.countAferEncode, correctionWordsCount, index, encodedBuf, encodedData);
		}
		protected void CreateErrorCorrection() {
			int correctionWordsCount = GetCorrectionWordsCount();
			int index = 0;
			for(index = 0; index < correctionWordsCount; index++)
				this.errorCorrectionCodes[index] = 0;
			for(index = 0; index <= ((this.countAferEncode - correctionWordsCount) - 1); index++) {
				int t2;
				int t3;
				int t1 = (this.encodedData[index] + this.errorCorrectionCodes[correctionWordsCount - 1]) % PDF417Constants.MaxCodewordsCount;
				for(int i = correctionWordsCount - 1; i >= 1; i--) {
					t2 = (t1 * GetErrorCorrectionCoefficient(i)) % PDF417Constants.MaxCodewordsCount;
					t3 = PDF417Constants.MaxCodewordsCount - t2;
					this.errorCorrectionCodes[i] = (this.errorCorrectionCodes[i - 1] + t3) % PDF417Constants.MaxCodewordsCount;
				}
				t2 = (t1 * GetErrorCorrectionCoefficient(0)) % PDF417Constants.MaxCodewordsCount;
				t3 = PDF417Constants.MaxCodewordsCount - t2;
				this.errorCorrectionCodes[0] = t3 % PDF417Constants.MaxCodewordsCount;
			}
			for(index = correctionWordsCount - 1; index >= 0; index--) {
				if(this.errorCorrectionCodes[index] != 0) {
					this.errorCorrectionCodes[index] = PDF417Constants.MaxCodewordsCount - this.errorCorrectionCodes[index];
				}
			}
			for(index = 0; index < correctionWordsCount; index++) {
				this.encodedData[(this.countAferEncode - correctionWordsCount) + index] = this.errorCorrectionCodes[(correctionWordsCount - index) - 1];
			}
		}
		int GetRows(int countAferEncode) {
			int rows = countAferEncode / this.columns;
			if((countAferEncode % this.columns) != 0)
				rows++;
			rows = Math.Max(PDF417Constants.MinRowsCount, rows);
			if(rows > PDF417Constants.MaxRowsCount) {
				this.columns++;
				if(this.columns > PDF417Constants.MaxColumnsCount)
					return rows;
				rows = GetRows(countAferEncode);
			}
			rows = Math.Max(rows, this.rows);
			this.rows = rows;
			return rows;
		}
		void CreateStartOfRow(int index) {
			UpdatePattern(PDF417Constants.StartPattern);
			UpdatePattern(PDF417Constants.PDF417Bits[index % 3, GetLeftRowIndicator(index)]);
		}
		void CreateEndOfRow(int index) {
			if(!this.truncateSymbol) {
				UpdatePattern(PDF417Constants.PDF417Bits[index % 3, GetRightRowIndicator(index)]);
				UpdatePattern(PDF417Constants.StopPattern);
			}
			else
				UpdatePattern(PDF417Constants.TruncatedStopPattern);
			CreateNewString();
		}
		int GetCorrectionWordsCount() {
			return 2 << (int)this.errorCorrectionLevel;
		}
		int GetErrorCorrectionCoefficient(int index) {
			switch(this.errorCorrectionLevel) {
				case ErrorCorrectionLevel.Level0:
					return ErrorCorrCoefficients.ErrorCorrCoef0[index];
				case ErrorCorrectionLevel.Level1:
					return ErrorCorrCoefficients.ErrorCorrCoef1[index];
				case ErrorCorrectionLevel.Level2:
					return ErrorCorrCoefficients.ErrorCorrCoef2[index];
				case ErrorCorrectionLevel.Level3:
					return ErrorCorrCoefficients.ErrorCorrCoef3[index];
				case ErrorCorrectionLevel.Level4:
					return ErrorCorrCoefficients.ErrorCorrCoef4[index];
				case ErrorCorrectionLevel.Level5:
					return ErrorCorrCoefficients.ErrorCorrCoef5[index];
				case ErrorCorrectionLevel.Level6:
					return ErrorCorrCoefficients.ErrorCorrCoef6[index];
				case ErrorCorrectionLevel.Level7:
					return ErrorCorrCoefficients.ErrorCorrCoef7[index];
				case ErrorCorrectionLevel.Level8:
					return ErrorCorrCoefficients.ErrorCorrCoef8[index];
			}
			return 0;
		}
		int GetRightRowIndicator(int row_index) {
			return GetRowIndicator(row_index, 1, 2, 0);
		}
		int GetLeftRowIndicator(int row_index) {
			return GetRowIndicator(row_index, 0, 1, 2);
		}
		int GetRowIndicator(int row_index, int firstRest, int secondRest, int thirdRest) {
			int rowIndicator = 0;
			if((row_index % 3) == firstRest)
				rowIndicator = ((row_index / 3) * PDF417Constants.MaxCharsCountInRow) + ((this.rows - 1) / 3);
			else if((row_index % 3) == secondRest)
				rowIndicator = (((row_index / 3) * PDF417Constants.MaxCharsCountInRow) + ((this.rows - 1) % 3)) + (3 * (int)this.errorCorrectionLevel);
			else if((row_index % 3) == thirdRest)
				rowIndicator = ((row_index / 3) * PDF417Constants.MaxCharsCountInRow) + (this.columns - 1);
			return rowIndicator;
		}
		void UpdatePattern(int numPattern) {
			List<bool> columnRow = new List<bool>();
			while(numPattern != 0) {
				columnRow.Insert(0, (numPattern & 1) != 0);
				numPattern >>= 1;
			}
			currentRow.AddRange(columnRow);
		}
		void CreateNewString() {
			pattern.Add(currentRow);
			currentRow = new List<bool>();
		}
		#region IPatternProcessor Members
		ArrayList IPatternProcessor.Pattern {
			get { return new ArrayList(pattern); }
		}
		void IPatternProcessor.RefreshPattern(object data) {
			if(IsValidData(data)) {
				SetData(data);
				CalculatePattern();
				FillPattern();
			}
		}
		void IPatternProcessor.Assign(IPatternProcessor source) {
			PDF417PatternProcessor sourceProcessor = source as PDF417PatternProcessor;
			this.columns = sourceProcessor.Columns;
			this.rows = sourceProcessor.Rows;
			this.errorCorrectionLevel = sourceProcessor.ErrorCorrectionLevel;
			this.truncateSymbol = sourceProcessor.TruncateSymbol;
		}
		#endregion
	}
	public class PDF417TextPatternProcessor : PDF417PatternProcessor {
		#region inner classes
		enum TextMode {
			UpperCase,
			LowerCase,
			Digit,
			Symbol
		}
		class TextModeHelper {
			public static int GetPDF417TextMode(TextMode textMode, TextMode previousTextMode) {
				if(textMode == TextMode.Symbol)
					return 0x1d;
				if(textMode == TextMode.Digit)
					return 0x1c;
				if(textMode == TextMode.LowerCase)
					return 0x1b;
				if(textMode == TextMode.UpperCase)
					return previousTextMode == TextMode.LowerCase ? 0x1b : 0x1c;
				return 0x1a;
			}
			public static TextMode GetPreviousLetterTextMode(TextMode textMode, TextMode previousTextMode, TextMode previousLetterTextMode) {
				if(textMode == TextMode.LowerCase)
					return TextMode.LowerCase;
				else if(textMode == TextMode.UpperCase)
					if(previousTextMode != TextMode.LowerCase)
						return TextMode.UpperCase;
				return previousLetterTextMode;
			}
			Dictionary<char, int> digitTable;
			Dictionary<char, int> symbolTable;
			TextMode textMode;
			int textValue;
			char ch;
			public TextMode TextMode { get { return textMode; } }
			public int TextValue { get { return textValue; } }
			public TextModeHelper(char ch) {
				this.ch = ch;
				InitializeTables();
				SetModeAndValue();
			}
			void InitializeTables() {
				digitTable = new Dictionary<char, int>();
				digitTable.Add('&', 10);
				digitTable.Add(',', 13);
				digitTable.Add(':', 14);
				digitTable.Add('#', 15);
				digitTable.Add('-', 16);
				digitTable.Add('.', 17);
				digitTable.Add('$', 18);
				digitTable.Add('/', 19);
				digitTable.Add('+', 20);
				digitTable.Add('%', 21);
				digitTable.Add('*', 22);
				digitTable.Add('=', 23);
				digitTable.Add('^', 24);
				digitTable.Add(' ', 26);
				symbolTable = new Dictionary<char, int>();
				symbolTable.Add(';', 0);
				symbolTable.Add('<', 1);
				symbolTable.Add('>', 2);
				symbolTable.Add('@', 3);
				symbolTable.Add('[', 4);
				symbolTable.Add('\\', 5);
				symbolTable.Add(']', 6);
				symbolTable.Add('_', 7);
				symbolTable.Add('`', 8);
				symbolTable.Add('~', 9);
				symbolTable.Add('!', 10);
				symbolTable.Add('\r', 11);
				symbolTable.Add('\t', 12);
				symbolTable.Add('\n', 15);
				symbolTable.Add('"', 20);
				symbolTable.Add('|', 21);
				symbolTable.Add('(', 23);
				symbolTable.Add(')', 24);
				symbolTable.Add('?', 25);
				symbolTable.Add('{', 26);
				symbolTable.Add('}', 27);
				symbolTable.Add('\'', 28);
			}
			void SetModeAndValue() {
				int textValue = 0;
				if(char.IsUpper(ch)) {
					this.textMode = TextMode.UpperCase;
					this.textValue = GetUpperValue();
				}
				else if(char.IsLower(ch)) {
					this.textMode = TextMode.LowerCase;
					this.textValue = GetLowerValue();
				}
				else if(char.IsDigit(ch)) {
					this.textMode = TextMode.Digit;
					this.textValue = (int)char.GetNumericValue(ch);
				}
				else if(digitTable.TryGetValue(ch, out textValue)) {
					this.textMode = TextMode.Digit;
					this.textValue = textValue;
				}
				else if(symbolTable.TryGetValue(ch, out textValue)) {
					this.textMode = TextMode.Symbol;
					this.textValue = textValue;
				}
			}
			int GetLowerValue() {
				return ch - 'a';
			}
			int GetUpperValue() {
				return ch - 'A';
			}
		}
		#endregion
		string text = string.Empty;
		protected override void SetData(object data) {
			System.Diagnostics.Debug.Assert(data is string, "data must be string type");
			this.text = (string)data;
		}
		protected override bool IsValidData(object data) {
			return ((string)data).Length <= PDF417Constants.MaxTextLength;
		}
		protected override int WriteDataInEncodedBuffer(int[] encodedBuf) {
			int index = 0;
			int charIndex = 0;
			TextModeHelper textModeHelper;
			TextMode previousTextMode = TextMode.UpperCase;
			TextMode previousLetterTextMode = TextMode.UpperCase;
			while(charIndex < text.Length) {
				textModeHelper = new TextModeHelper(text.ToCharArray()[charIndex]);
				if((previousLetterTextMode == TextMode.LowerCase) && (previousTextMode == TextMode.UpperCase))
					previousTextMode = TextMode.LowerCase;
				if(textModeHelper.TextMode != previousTextMode) {
					encodedBuf[index++] = TextModeHelper.GetPDF417TextMode(textModeHelper.TextMode, previousTextMode);
					previousLetterTextMode = TextModeHelper.GetPreviousLetterTextMode(textModeHelper.TextMode, previousTextMode, previousLetterTextMode);
				}
				encodedBuf[index++] = textModeHelper.TextValue;
				if(textModeHelper.TextMode != previousTextMode && textModeHelper.TextMode != TextMode.Symbol)
					previousTextMode = textModeHelper.TextMode;
				charIndex++;
			}
			if((index % 2) == 1)
				encodedBuf[index++] = 0x1d;
			return index;
		}
		protected override int GetCountAfterEncode(int index, int correctionWordsCount) {
			return (1 + (index / 2)) + correctionWordsCount;
		}
		protected override void ErrorCorrection(int countAferEncode, int correctionWordsCount, int index, int[] encodedBuf, int[] encodedData) {
			if(countAferEncode <= PDF417Constants.MaxCodewordsCapacity) {
				while((index / 2) < ((countAferEncode - correctionWordsCount) - 1)) {
					encodedBuf[index++] = PDF417Constants.MaxCharsCountInRow;
					encodedBuf[index++] = 0;
				}
				encodedData[0] = countAferEncode - correctionWordsCount;
				for(int j = 0; j < (index / 2); j++) {
					encodedData[j + 1] = (encodedBuf[2 * j] * PDF417Constants.MaxCharsCountInRow) + encodedBuf[(2 * j) + 1];
				}
				encodedData[(index / 2) + 1] = 0;
				CreateErrorCorrection();
			}
		}
	}
	public class PDF417BinaryPatternProcessor : PDF417PatternProcessor {
		byte[] data = new byte[] { };
		protected override void SetData(object data) {
			System.Diagnostics.Debug.Assert(data is byte[], "data must be byte array type");
			this.data = (byte[])data;
		}
		protected override bool IsValidData(object data) {
			return ((byte[])data).Length <= PDF417Constants.MaxDataLength;
		}
		protected override int WriteDataInEncodedBuffer(int[] encodedBuf) {
			int byteIndex = 0;
			int index = 0;
			ushort[] numArray = new ushort[PDF417Constants.CalculateDegree];
			while((byteIndex + (PDF417Constants.CalculateDegree - 1)) < data.Length) {
					long t = 0L;
					for(int j = 0; (j < PDF417Constants.CalculateDegree) && (byteIndex < data.Length); j++) {
						t = (t * PDF417Constants.ByteCalculationCoefficient) + data[byteIndex++];
					}
					int numIndex = 0;
					while(numIndex < (PDF417Constants.CalculateDegree - 1)) {
						numArray[numIndex++] = (ushort)(t % PDF417Constants.ByteConversionCoefficient);
						t /= PDF417Constants.ByteConversionCoefficient;
					}
					for(int k = numIndex - 1; k >= 0; k--)
						encodedBuf[index++] = numArray[k];
				}
				while(byteIndex <= (data.Length - 1))
					encodedBuf[index++] = data[byteIndex++];
				return index;
		}
		protected override int GetCountAfterEncode(int index, int correctionWordsCount) {
			return 2 + index + correctionWordsCount;
		}
		protected override void ErrorCorrection(int countAferEncode, int correctionWordsCount, int index, int[] encodedBuf, int[] encodedData) {
			if(countAferEncode <= PDF417Constants.MaxCodewordsCapacity) {
				while(index < ((countAferEncode - correctionWordsCount) - 2))
					encodedBuf[index++] = PDF417Constants.ErrorCorrectionPlacebo;
				encodedData[0] = (ushort)(countAferEncode - correctionWordsCount);
				if((data.Length % PDF417Constants.CalculateDegree) != 0)
					encodedData[1] = PDF417Constants.ModeLatchToByteCompaction;
				else
					encodedData[1] = PDF417Constants.EndLatchToByteCompaction;
				for(int m = 0; m < index; m++)
					encodedData[m + 2] = encodedBuf[m];
				CreateErrorCorrection();
			}
		}
	}
}
