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

using DevExpress.XtraSpreadsheet.Internal;
using System.IO;
namespace DevExpress.XtraSpreadsheet.Import.Csv {
	#region CsvImportStreamAdaptor and derived classes
	public delegate bool CsvImportNewlineChecker();
	public abstract class CsvImportStreamAdaptor {
		#region Static
		public static CsvImportStreamAdaptor CreateAdaptor(Stream stream, TextDocumentImporterOptionsBase options) {
			switch (options.ActualEncoding.WebName.ToLowerInvariant()) {
				case "utf-8":
					return new CsvImportStreamAdaptorUtf8(stream, options);
				case "utf-16":
				case "utf-16le":
					return new CsvImportStreamAdaptorUtf16LittleEndian(stream, options);
				case "utf-16be": 
				case "unicodefffe": 
					return new CsvImportStreamAdaptorUtf16BigEndian(stream, options);
				case "utf-32":
				case "utf-32le":
					return new CsvImportStreamAdaptorUtf32LittleEndian(stream, options);
				case "utf-32be":
					return new CsvImportStreamAdaptorUtf32BigEndian(stream, options);
				default:
					return new CsvImportStreamAdaptorAscii8(stream, options);
			}
		}
		#endregion
		#region Fields
		int currChar;
		TextDocumentImporterOptionsBase options;
		CsvImportNewlineChecker newlineChecker;
		Stream inputStream;
		long storedPosition;
		#endregion;
		protected CsvImportStreamAdaptor(Stream inputStream, TextDocumentImporterOptionsBase options) {
			this.options = options;
			this.inputStream = inputStream;
			this.currChar = -1;
			this.newlineChecker = SelectNewlineChecker(options.NewlineType);
		}
		#region Properties
		public int CurrentChar { get { return currChar; } }
		public long Position { get { return inputStream.Position; } set { inputStream.Position = value; } }
		protected Stream InputStream { get { return inputStream; } }
		#endregion
		public bool IsQuote() {
			return CurrentChar == options.TextQualifier;
		}
		public bool IsSeparator() {
			return CurrentChar == options.ActualDelimiter;
		}
		public bool IsNewLine() {
			return newlineChecker();
		}
		public bool IsNullSymbol() {
			return CurrentChar == '\0';
		}
		public void PushPosition() {
			this.storedPosition = InputStream.Position;
		}
		public void PopPosition() {
			InputStream.Position = this.storedPosition;
		}
		public void ProcessChar() {
			this.currChar = ReadChar();
		}
		protected virtual int ReadChar() {
			return InputStream.ReadByte();
		}
		public abstract void AppendToStorage(ByteBuilder storage, int value);
		#region Newline helpers
		bool IsNewlineCrLf() {
			bool result = (CurrentChar == '\r');
			if(result) {
				long previousStreamPos = InputStream.Position;
				int secondChar = ReadChar();
				result = (secondChar == '\n');
				if(!result && (secondChar != -1))
					InputStream.Position = previousStreamPos;
			}
			return result;
		}
		bool IsNewlineLf() {
			return CurrentChar == '\n';
		}
		bool IsNewlineCr() {
			return CurrentChar == '\r';
		}
		bool IsNewlineLfCr() {
			bool result = (CurrentChar == '\n');
			if(result) {
				long previousStreamPos = InputStream.Position;
				int secondChar = ReadChar();
				result = (secondChar == '\r');
				if(!result && (secondChar != -1))
					InputStream.Position = previousStreamPos;
			}
			return result;
		}
		bool IsNewlineVerticalTab() {
			return CurrentChar == '\v';
		}
		bool IsNewlineFormFeed() {
			return CurrentChar == '\f';
		}
		bool IsNewlineAnyCrLf() {
			return IsOneOrTwoCharSequence('\r', '\n') || IsOneOrTwoCharSequence('\n', '\r');
		}
		bool IsOneOrTwoCharSequence(char firstChar, char secondChar) {
			if (CurrentChar == firstChar) {
				long previousStreamPos = InputStream.Position;
				int nextChar = ReadChar();
				if ((nextChar != secondChar) && (nextChar != -1)) {
					InputStream.Position = previousStreamPos;
				}
				return true;
			}
			return false;
		}
		bool IsNewlineAuto() {
			if(CurrentChar == -1)
				return false;
			if(IsNewlineCrLf()) {
				newlineChecker = IsNewlineCrLf;
				options.NewlineType = NewlineType.CrLf;
				return true;
			}
			if(IsNewlineLfCr()) {
				newlineChecker = IsNewlineLfCr;
				options.NewlineType = NewlineType.LfCr;
				return true;
			}
			if(IsNewlineCr()) {
				newlineChecker = IsNewlineCr;
				options.NewlineType = NewlineType.Cr;
				return true;
			}
			if(IsNewlineLf()) {
				newlineChecker = IsNewlineLf;
				options.NewlineType = NewlineType.Lf;
				return true;
			}
			return false;
		}
		CsvImportNewlineChecker SelectNewlineChecker(NewlineType newlineType) {
			switch(newlineType) {
				case NewlineType.CrLf:
					return IsNewlineCrLf;
				case NewlineType.Lf:
					return IsNewlineLf;
				case NewlineType.Cr:
					return IsNewlineCr;
				case NewlineType.LfCr:
					return IsNewlineLfCr;
				case NewlineType.FormFeed:
					return IsNewlineFormFeed;
				case NewlineType.VerticalTab:
					return IsNewlineVerticalTab;
				case NewlineType.AnyCrLf:
					return IsNewlineAnyCrLf;
			}
			return IsNewlineAuto;
		}
		#endregion
	}
	#region CsvImportStreamAdaptorAscii8
	public class CsvImportStreamAdaptorAscii8 : CsvImportStreamAdaptor {
		public CsvImportStreamAdaptorAscii8(Stream inputStream, TextDocumentImporterOptionsBase options)
			: base(inputStream, options) {
		}
		public override void AppendToStorage(ByteBuilder storage, int value) {
			storage.Append((byte)value);
		}
	}
	#endregion
	#region CsvImportControlsAdaptorUtf8
	public class CsvImportStreamAdaptorUtf8 : CsvImportStreamAdaptorAscii8 {
		public CsvImportStreamAdaptorUtf8(Stream inputStream, TextDocumentImporterOptionsBase options)
			: base(inputStream, options) {
		}
	}
	#endregion
	#region CsvImportStreamAdaptorUtf16LittleEndian
	public class CsvImportStreamAdaptorUtf16LittleEndian : CsvImportStreamAdaptor {
		public CsvImportStreamAdaptorUtf16LittleEndian(Stream inputStream, TextDocumentImporterOptionsBase options)
			: base(inputStream, options) {
		}
		protected override int ReadChar() {
			int result = -1;
			int part1 = InputStream.ReadByte();
			if(part1 != -1) {
				int part2 = InputStream.ReadByte();
				if(part2 != -1) {
					result = (part2 << 8) | (part1 & 0xff);
				}
			}
			return result;
		}
		public override void AppendToStorage(ByteBuilder storage, int value) {
			storage.Append((char)value);
		}
	}
	#endregion
	#region CsvImportStreamAdaptorUtf16BigEndian
	public class CsvImportStreamAdaptorUtf16BigEndian : CsvImportStreamAdaptor {
		public CsvImportStreamAdaptorUtf16BigEndian(Stream inputStream, TextDocumentImporterOptionsBase options)
			: base(inputStream, options) {
		}
		protected override int ReadChar() {
			int result = -1;
			int part1 = InputStream.ReadByte();
			if(part1 != -1) {
				int part2 = InputStream.ReadByte();
				if(part2 != -1) {
					result = (part1 << 8) | (part2 & 0xff);
				}
			}
			return result;
		}
		public override void AppendToStorage(ByteBuilder storage, int value) {
			storage.AppendBigEndian((char)value);
		}
	}
	#endregion
	#region CsvImportStreamAdaptorUtf32LittleEndian
	public class CsvImportStreamAdaptorUtf32LittleEndian : CsvImportStreamAdaptor {
		public CsvImportStreamAdaptorUtf32LittleEndian(Stream inputStream, TextDocumentImporterOptionsBase options)
			: base(inputStream, options) {
		}
		protected override int ReadChar() {
			int result = -1;
			int part1 = InputStream.ReadByte();
			if(part1 != -1) {
				int part2 = InputStream.ReadByte();
				if(part2 != -1) {
					int part3 = InputStream.ReadByte();
					if(part3 != -1) {
						int part4 = InputStream.ReadByte();
						if(part4 != -1) {
							result = (part4 << 24) | (part3 << 16) | (part2 << 8) | (part1 & 0xff);
						}
					}
				}
			}
			return result;
		}
		public override void AppendToStorage(ByteBuilder storage, int value) {
			storage.Append(value);
		}
	}
	#endregion
	#region CsvImportStreamAdaptorUtf32BigEndian
	public class CsvImportStreamAdaptorUtf32BigEndian : CsvImportStreamAdaptor {
		public CsvImportStreamAdaptorUtf32BigEndian(Stream inputStream, TextDocumentImporterOptionsBase options)
			: base(inputStream, options) {
		}
		protected override int ReadChar() {
			int result = -1;
			int part1 = InputStream.ReadByte();
			if(part1 != -1) {
				int part2 = InputStream.ReadByte();
				if(part2 != -1) {
					int part3 = InputStream.ReadByte();
					if(part3 != -1) {
						int part4 = InputStream.ReadByte();
						if(part4 != -1) {
							result = (part1 << 24) | (part2 << 16) | (part3 << 8) | (part4 & 0xff);
						}
					}
				}
			}
			return result;
		}
		public override void AppendToStorage(ByteBuilder storage, int value) {
			storage.Append(value);
		}
	}
	#endregion
	#endregion
}
