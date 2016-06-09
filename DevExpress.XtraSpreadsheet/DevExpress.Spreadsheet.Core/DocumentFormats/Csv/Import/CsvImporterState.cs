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

using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Import.Csv {
	#region CsvImporterState
	public enum CsvImporterStateType {
		Normal,
		Quoted,
	}
	public delegate bool CharProcessor(CsvImporter importer);
	public abstract class CsvImportReaderBase {
		CharProcessor processChar;
		CsvImportStreamAdaptor adaptor;
		protected CsvImportReaderBase(CsvImportStreamAdaptor adaptor) {
			SwitchState(CsvImporterStateType.Normal);
			this.adaptor = adaptor;
		}
		protected CsvImportStreamAdaptor Adaptor { get { return adaptor; } }
		public bool ProcessChar(CsvImporter importer) {
			return processChar(importer);
		}
		public void ProcessChars(CsvImporter importer) {
			while (processChar(importer)) { };
		}
		protected virtual void SwitchState(CsvImporterStateType type) {
			switch (type) {
				case CsvImporterStateType.Normal:
					processChar = ProcessCharNormal;
					break;
				case CsvImporterStateType.Quoted:
					processChar = ProcessCharQuoted;
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		protected abstract bool ProcessCharNormal(CsvImporter importer);
		protected abstract bool ProcessCharQuoted(CsvImporter importer);
	}
	public class CsvImportReader : CsvImportReaderBase {
		int charCount = 0;
		public CsvImportReader(CsvImportStreamAdaptor adaptor)
			: base(adaptor) {
		}
		#region ProcessCharNormal
		bool ProcessNewline(CsvImporter importer) {
			charCount = 0;
			if (!importer.ApplyCellValue())
				return false;
			importer.CellValue.Length = 0; 
			importer.MoveToNextRow();
			importer.MoveToFirstColumnInRow();
			return true;
		}
		void ProcessQuote(CsvImporter importer) {
			if (importer.CellValue.Length > 0)
				Adaptor.AppendToStorage(importer.CellValue, importer.Options.TextQualifier);
			else
				SwitchState(CsvImporterStateType.Quoted);
		}
		bool ProcessSeparator(CsvImporter importer) {
			charCount = 0;
			if (!importer.ApplyCellValue())
				return false;
			importer.CellValue.Length = 0; 
			importer.MoveToNextPositionInRow();
			return true;
		}
		protected override void SwitchState(CsvImporterStateType type) {
			base.SwitchState(type);
			charCount = 0;
		}
		protected override bool ProcessCharNormal(CsvImporter importer) {
			Adaptor.ProcessChar();
			if (Adaptor.IsNewLine())
				return ProcessNewline(importer);
			else {
				if (Adaptor.IsQuote())
					ProcessQuote(importer);
				else if (Adaptor.IsSeparator())
					return ProcessSeparator(importer);
				else if (Adaptor.IsNullSymbol()) {
					ProcessSeparator(importer);
					return false;
				}
				else {
					if (Adaptor.CurrentChar == -1)
						return false;
					Adaptor.AppendToStorage(importer.CellValue, Adaptor.CurrentChar);
				}
			}
			return true;
		}
		#endregion
		#region ProcessCharQuoted
		protected override bool ProcessCharQuoted(CsvImporter importer) {
			if (charCount >= CsvImporter.CharCounterLimit) {
				SwitchState(CsvImporterStateType.Normal);
				return true;
			}
			charCount++;
			Adaptor.ProcessChar();
			if (Adaptor.CurrentChar == -1)
				return false;
			if (Adaptor.IsQuote()) {
				Adaptor.PushPosition();
				Adaptor.ProcessChar();
				if (Adaptor.CurrentChar == -1)
					return false;
				if (Adaptor.IsQuote()) {
					Adaptor.AppendToStorage(importer.CellValue, importer.Options.TextQualifier);
				}
				else {
					Adaptor.PopPosition();
					SwitchState(CsvImporterStateType.Normal);
				}
			}
			else
				Adaptor.AppendToStorage(importer.CellValue, Adaptor.CurrentChar);
			return true;
		}
		#endregion
	}
	public class CsvImportSeparatorDetector : CsvImportReaderBase {
		bool isJustAfterQuoted;
		int commaBeforeQuoteCounter;
		int semicolonBeforeQuoteCounter;
		int tabBeforeQuoteCounter;
		int commaAfterQuoteCounter;
		int semicolonAfterQuoteCounter;
		int tabAfterQuoteCounter;
		int totalCommaCounter;
		int totalSemicolonCounter;
		int totalTabCounter;
		int charCounter;
		public CsvImportSeparatorDetector(CsvImportStreamAdaptor adaptor)
			: base(adaptor) {
		}
		public char DetectSeparator(char defaultDelimeter) {
			long storedPosition = Adaptor.Position;
			while (ProcessChar(null))
				;
			Adaptor.Position = storedPosition;
			return AnalyzeSeparatorStatistics(defaultDelimeter);
		}
		#region ProcessCharNormal
		void ProcessAny(int processedChar) {
			++charCounter;
			switch (processedChar) {
				case ',':
					++totalCommaCounter;
					break;
				case ';':
					++totalSemicolonCounter;
					break;
				case '\t':
					++totalTabCounter;
					break;
			}
		}
		void ProcessBefore(int processedChar) {
			switch (processedChar) {
				case ',':
					++commaBeforeQuoteCounter;
					break;
				case ';':
					++semicolonBeforeQuoteCounter;
					break;
				case '\t':
					++tabBeforeQuoteCounter;
					break;
			}
		}
		void ProcessAfter(int processedChar) {
			isJustAfterQuoted = false;
			switch (processedChar) {
				case ',':
					++commaAfterQuoteCounter;
					break;
				case ';':
					++semicolonAfterQuoteCounter;
					break;
				case '\t':
					++tabAfterQuoteCounter;
					break;
			}
		}
		protected override bool ProcessCharNormal(CsvImporter importer) {
			if (charCounter >= CsvImporter.separatorDetectorCharCounterLimit)
				return false;
			int previousChar = Adaptor.CurrentChar;
			Adaptor.ProcessChar();
			ProcessAny(Adaptor.CurrentChar);
			if (Adaptor.IsQuote()) {
				ProcessBefore(previousChar);
				SwitchState(CsvImporterStateType.Quoted);
			}
			else {
				if (isJustAfterQuoted) {
					ProcessAfter(Adaptor.CurrentChar);
				}
			}
			return Adaptor.CurrentChar != -1;
		}
		#endregion
		#region ProcessCharQuoted
		protected override bool ProcessCharQuoted(CsvImporter importer) {
			if (charCounter >= CsvImporter.separatorDetectorCharCounterLimit)
				return false;
			++charCounter;
			Adaptor.ProcessChar();
			if (Adaptor.IsQuote()) {
				isJustAfterQuoted = true;
				SwitchState(CsvImporterStateType.Normal);
			}
			return Adaptor.CurrentChar != -1;
		}
		#endregion
		#region Analyzers
		char AnalyzeSeparatorSimpleCheckPrimitive(int commaCounter, int semicolonCounter, int tabCounter) {
			if (AnalyzeSeparatorSimpleCheckCountZeros(commaCounter, semicolonCounter, tabCounter) != 2)
				return '\0'; 
			if (commaCounter != 0)
				return ',';
			if (semicolonCounter != 0)
				return ';';
			return '\t';
		}
		int AnalyzeSeparatorSimpleCheckCountZeros(int commaCounter, int semicolonCounter, int tabCounter) {
			int result = commaCounter == 0 ? 1 : 0;
			if (semicolonCounter == 0)
				++result;
			if (tabCounter == 0)
				++result;
			return result;
		}
		char AnalyzeSeparatorSimpleCheck() {
			char result = AnalyzeSeparatorSimpleCheckPrimitive(totalCommaCounter,
															   totalSemicolonCounter,
															   totalTabCounter);
			if (result != '\0')
				return result;
			result = AnalyzeSeparatorSimpleCheckPrimitive(commaBeforeQuoteCounter + commaAfterQuoteCounter,
														  semicolonBeforeQuoteCounter + semicolonAfterQuoteCounter,
														  tabBeforeQuoteCounter + tabAfterQuoteCounter);
			if (result != '\0')
				return result;
			result = AnalyzeSeparatorSimpleCheckPrimitive(commaBeforeQuoteCounter,
														  semicolonBeforeQuoteCounter,
														  tabBeforeQuoteCounter);
			return result;
		}
		char GetMostFrequentCharacter() {
			if (totalTabCounter > totalSemicolonCounter) {
				if (totalTabCounter > totalCommaCounter)
					return '\t';
				if (totalTabCounter == totalCommaCounter)
					return '\0';
				return ',';
			}
			if (totalSemicolonCounter > totalCommaCounter)
				return ';';
			if (totalSemicolonCounter == totalCommaCounter)
				return '\0';
			return ',';
		}
		char AnalyzeSeparatorStatistics(char defaultDelimeter) {
			if (charCounter < 1)
				return defaultDelimeter;
			char result = AnalyzeSeparatorSimpleCheck();
			if (result != '\0')
				return result;
			result = GetMostFrequentCharacter();
			return (result != '\0') ? result : defaultDelimeter;
		}
		#endregion
	}
	#endregion
}
