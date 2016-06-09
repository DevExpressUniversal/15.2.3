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
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource;
using DevExpress.SpreadsheetSource.Implementation;
namespace DevExpress.SpreadsheetSource.Xls {
	using DevExpress.Office.Utils;
	using DevExpress.XtraExport.Xls;
	using DevExpress.XtraSpreadsheet.Internal;
	#region IXlsSourceCommand
	public interface IXlsSourceCommand {
		bool IsSubstreamBound { get; }
		void Read(XlsReader reader, XlsSpreadsheetSource contentBuilder);
		void Execute(XlsSpreadsheetSource contentBuilder);
		void Execute(XlsSourceDataReader dataReader);
	}
	#endregion
	#region XlsSourceCommandBase (abstract)
	public abstract class XlsSourceCommandBase : IXlsSourceCommand {
		protected int Size { get; private set; }
		public virtual bool IsSubstreamBound { get { return false; } }
		public void Read(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			Size = reader.ReadNotCryptedUInt16();
			if(Size > XlsDefs.MaxRecordDataSize)
				throw new InvalidFileException(string.Format("Record data size greater than {0}", XlsDefs.MaxRecordDataSize));
			long initialPosition = reader.Position;
			long expectedPosition = initialPosition + Size;
			ReadCore(reader, contentBuilder);
			CheckPosition(reader, initialPosition, expectedPosition);
		}
		public virtual void Execute(XlsSpreadsheetSource contentBuilder) {
		}
		public virtual void Execute(XlsSourceDataReader dataReader) {
		}
		protected abstract void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder);
		protected virtual void CheckPosition(XlsReader reader, long initialPosition, long expectedPosition) {
			long actualPosition = reader.Position;
			if(actualPosition < expectedPosition)
				reader.Seek(expectedPosition - actualPosition, SeekOrigin.Current); 
			else if(actualPosition > expectedPosition)
				throw new InvalidFileException(
					string.Format("Read failure: initial/expected/actual positions = {0}/{1}/{2}, command={3}",
					initialPosition, expectedPosition, actualPosition, this.GetType().ToString()));
		}
	}
	#endregion
	#region XlsSourceCommandContentBase (absract)
	public abstract class XlsSourceCommandContentBase : XlsSourceCommandBase {
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			IXlsContent content = GetContent();
			if(content != null)
				content.Read(reader, Size);
		}
		protected abstract IXlsContent GetContent();
	}
	#endregion
	#region XlsSourceCommandRecordBase (abstract class)
	public abstract class XlsSourceCommandRecordBase : XlsSourceCommandBase {
		byte[] data = new byte[0];
		#region Properties
		public byte[] Data {
			get { return data; }
			private set {
				if(value != null)
					data = value;
				else
					data = new byte[0];
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			Data = reader.ReadBytes(Size);
		}
	}
	#endregion
	#region IXlsSourceDataCollector
	public interface IXlsSourceDataCollector {
		void PutData(byte[] data, XlsSpreadsheetSource contentBuilder);
	}
	#endregion
	#region XlsSourceCommandDataCollectorBase (abstract class)
	public abstract class XlsSourceCommandDataCollectorBase : XlsSourceCommandRecordBase, IXlsSourceDataCollector {
		#region IXlsDataCollector Members
		public virtual void PutData(byte[] data, XlsSpreadsheetSource contentBuilder) {
			MemoryStream stream = new MemoryStream(data, false);
			using(BinaryReader baseReader = new BinaryReader(stream)) {
				using(XlsReader reader = new XlsReader(baseReader)) {
					ReadData(reader, contentBuilder);
				}
			}
			if(GetCompleted())
				contentBuilder.PopDataCollector();
		}
		#endregion
		protected virtual void ReadData(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
		}
		protected virtual bool GetCompleted() {
			return false;
		}
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			if(Data.Length > 0) {
				contentBuilder.PushDataCollector(this);
				PutData(Data, contentBuilder);
			}
		}
	}
	#endregion
	#region XlsSourceCommandEmpty
	public class XlsSourceCommandEmpty : XlsSourceCommandBase {
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			reader.Seek(Size, SeekOrigin.Current);
		}
	}
	#endregion
	#region XlsSourceCommandBOF
	public class XlsSourceCommandBOF : XlsSourceCommandContentBase {
		XlsContentBeginOfSubstream content = new XlsContentBeginOfSubstream();
		public override bool IsSubstreamBound { get { return true; } }
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			contentBuilder.StartContent(content.SubstreamType);
		}
		public override void Execute(XlsSourceDataReader dataReader) {
			dataReader.StartContent(content.SubstreamType);
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandEOF
	public class XlsSourceCommandEOF : XlsSourceCommandBase {
		public override bool IsSubstreamBound { get { return true; } }
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
		}
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			contentBuilder.ClearDataCollectors();
			contentBuilder.EndContent();
		}
		public override void Execute(XlsSourceDataReader dataReader) {
			dataReader.EndContent();
		}
	}
	#endregion
	#region XlsSourceCommandDate1904
	public class XlsSourceCommandDate1904 : XlsSourceCommandBase {
		public bool IsDate1904 { get; private set; }
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			IsDate1904 = reader.ReadInt16() == 0x01;
		}
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			contentBuilder.IsDate1904 = IsDate1904;
		}
	}
	#endregion
	#region XlSourceCommandBoundSheet8
	public class XlsSourceCommandBoundSheet8 : XlsSourceCommandBase {
		bool isRegularSheet;
		#region Properties
		public int StartPosition { get; private set; }
		public XlSheetVisibleState VisibleState { get; private set; }
		public string Name { get; private set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			StartPosition = reader.ReadNotCryptedInt32();
			VisibleState = (XlSheetVisibleState)(reader.ReadByte() & 0x03);
			this.isRegularSheet = reader.ReadByte() == 0;
			Name = ShortXLUnicodeString.FromStream(reader).Value;
		}
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			if(this.isRegularSheet)
				contentBuilder.InnerWorksheets.Add(new XlsWorksheet(Name, VisibleState, StartPosition));
			contentBuilder.SheetNames.Add(Name);
		}
	}
	#endregion
	#region XlsSourceCommandFormat
	public class XlsSourceCommandFormat : XlsSourceCommandContentBase {
		XlsContentNumberFormat content = new XlsContentNumberFormat();
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			contentBuilder.NumberFormatCodes[content.FormatId] = content.FormatCode;
		}
		protected override IXlsContent GetContent() {
			return content;			
		}
	}
	#endregion
	#region XlsSourceCommandXF
	public class XlsSourceCommandXF : XlsSourceCommandContentBase {
		XlsContentXF content = new XlsContentXF();
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			contentBuilder.NumberFormatIds.Add(content.NumberFormatId);
		}
		protected override IXlsContent GetContent() {
			content = new XlsContentXF();
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandSharedStrings
	public class XlsSourceCommandSharedStrings : XlsSourceCommandDataCollectorBase {
		#region Fields
		int count;
		int stringsToRead;
		bool readingStrings;
		XLUnicodeRichExtendedString sharedString = new XLUnicodeRichExtendedString();
		#endregion
		protected override void ReadData(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			if(!this.readingStrings) {
				reader.ReadInt32(); 
				this.stringsToRead = reader.ReadInt32(); 
				this.readingStrings = true;
			}
			while(this.count < this.stringsToRead) {
				if(!ReadSharedString(reader)) return;
				contentBuilder.SharedStrings.Add(this.sharedString.Value);
				count++;
			}
		}
		protected override bool GetCompleted() {
			return this.count == this.stringsToRead;
		}
		bool ReadSharedString(XlsReader reader) {
			if(reader.Position == reader.StreamLength)
				return false;
			this.sharedString.ReadData(reader);
			return this.sharedString.Complete;
		}
	}
	#endregion
	#region XlsSourceCommandContinue
	public class XlsSourceCommandContinue : XlsSourceCommandRecordBase {
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			if(contentBuilder.DataCollector != null)
				contentBuilder.DataCollector.PutData(Data, contentBuilder);
		}
	}
	#endregion
	#region XlsSourceCommandSupBook
	public class XlsSourceCommandSupBook : XlsSourceCommandBase {
		public bool IsSelfReferencing { get; private set; }
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			reader.ReadUInt16(); 
			ushort cch = reader.ReadUInt16();
			IsSelfReferencing = cch == 0x0401;
			int bytesToRead = Size - 4;
			if(bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			if(IsSelfReferencing)
				contentBuilder.SelfRefBookIndex = contentBuilder.SupBookCount;
			contentBuilder.SupBookCount++;
		}
	}
	#endregion
	#region XlsSourceCommandExternSheet
	public class XlsSourceCommandExternSheet : XlsSourceCommandRecordBase {
		#region Fields
		short[] typeCodes = new short[] {
			0x003c 
		};
		readonly List<XlsXTI> items = new List<XlsXTI>();
		#endregion
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			items.Clear();
			if(contentBuilder.ContentType == XlsContentType.WorkbookGlobals) {
				using(XlsCommandStream commandStream = new XlsCommandStream(reader, typeCodes, Size)) {
					using(BinaryReader commandReader = new BinaryReader(commandStream)) {
						int count = commandReader.ReadUInt16();
						for(int i = 0; i < count; i++) {
							XlsXTI item = XlsXTI.FromStream(commandReader);
							this.items.Add(item);
						}
					}
				}
			}
			else
				base.ReadCore(reader, contentBuilder);
		}
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			contentBuilder.ExternSheets.AddRange(this.items);
			this.items.Clear();
		}
		protected override void CheckPosition(XlsReader reader, long initialPosition, long expectedPosition) {
		}
	}
	#endregion
	#region XlsSourceCommandDefinedName
	public class XlsSourceCommandDefinedName : XlsSourceCommandContentBase {
		XlsContentDefinedName content = new XlsContentDefinedName();
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			if(this.content.IsMacro || this.content.IsVbaMacro || this.content.IsXlmMacro)
				return;
			XlCellRange range = GetRange(contentBuilder);
			if(range == null)
				return;
			string scopeSheetName = GetScopeSheetName(contentBuilder);
			if(string.IsNullOrEmpty(range.SheetName) && string.IsNullOrEmpty(scopeSheetName))
				return;
			DefinedName definedName = new DefinedName(this.content.Name, scopeSheetName, this.content.IsHidden, range, range.ToString(true));
			contentBuilder.InnerDefinedNames.Add(definedName);
		}
		protected override IXlsContent GetContent() {
			content = new XlsContentDefinedName();
			return content;
		}
		XlCellRange GetRange(XlsSpreadsheetSource contentBuilder) {
			if(this.content.FormulaSize != 0x0b)
				return null;
			byte[] formulaBytes = this.content.FormulaBytes;
			if(formulaBytes[0] != 0x3b) 
				return null;
			int xtiIndex = BitConverter.ToUInt16(formulaBytes, 1);
			if(xtiIndex == 0xffff)
				return null; 
			if(xtiIndex >= contentBuilder.ExternSheets.Count)
				return null; 
			int firstRow = BitConverter.ToUInt16(formulaBytes, 3);
			int lastRow = BitConverter.ToUInt16(formulaBytes, 5);
			int firstColumn = BitConverter.ToUInt16(formulaBytes, 7);
			int lastColumn = BitConverter.ToUInt16(formulaBytes, 9);
			if((firstColumn & 0xc000) != 0 || (lastColumn & 0xc000) != 0)
				return null; 
			XlsXTI xti = contentBuilder.ExternSheets[xtiIndex];
			if(xti.SupBookIndex != contentBuilder.SelfRefBookIndex)
				return null; 
			if(xti.FirstSheetIndex != xti.LastSheetIndex)
				return null; 
			string sheetName = string.Empty;
			if(xti.FirstSheetIndex != -2) {
				if(xti.FirstSheetIndex < 0 || xti.FirstSheetIndex >= contentBuilder.SheetNames.Count)
					return null; 
				sheetName = contentBuilder.SheetNames[xti.FirstSheetIndex];
				if(contentBuilder.Worksheets[sheetName] == null)
					return null; 
			}
			XlCellRange range = new XlCellRange(
				new XlCellPosition(firstColumn, firstRow, XlPositionType.Absolute, XlPositionType.Absolute),
				new XlCellPosition(lastColumn, lastRow, XlPositionType.Absolute, XlPositionType.Absolute));
			range.SheetName = sheetName;
			return range;
		}
		string GetScopeSheetName(XlsSpreadsheetSource contentBuilder) {
			int index = this.content.SheetIndex - 1;
			if(index < 0 || index >= contentBuilder.SheetNames.Count)
				return string.Empty; 
			string sheetName = contentBuilder.SheetNames[index];
			if(contentBuilder.Worksheets[sheetName] == null)
				return string.Empty; 
			return sheetName;
		}
	}
	#endregion
	#region XlsSourceCommandNameComment
	public class XlsSourceCommandNameComment : XlsSourceCommandBase {
		XLUnicodeStringNoCch name;
		XLUnicodeStringNoCch comment;
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			int cchName = reader.ReadUInt16();
			int cchComment = reader.ReadUInt16();
			this.name = XLUnicodeStringNoCch.FromStream(reader, cchName);
			this.comment = XLUnicodeStringNoCch.FromStream(reader, cchComment);
		}
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			if(contentBuilder.DefinedNames.Count == 0)
				return;
			DefinedName definedName = contentBuilder.DefinedNames[contentBuilder.DefinedNames.Count - 1] as DefinedName;
			if(definedName == null)
				return;
			if(IsSameName(definedName.Name, this.name.Value))
				definedName.Comment = this.comment.Value;
		}
		bool IsSameName(string name1, string name2) {
			if(string.IsNullOrEmpty(name1))
				name1 = string.Empty;
			if(name1.StartsWith("_xlnm."))
				name1 = name1.Remove(0, 6);
			return name1 == name2;
		}
	}
	#endregion
	#region XlsSourceCommandFilePassword
	public class XlsSourceCommandFilePassword : XlsSourceCommandBase {
		#region Fields
		public const string MagicMSPassword = "VelvetSweatshop";
		bool rc4Encrypted;
		XlsXORObfuscation xorObfuscation = new XlsXORObfuscation(0, 0);
		XlsRC4EncryptionHeaderBase rc4EncryptionHeader = new XlsRC4EncryptionHeader();
		#endregion
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			rc4Encrypted = Convert.ToBoolean(reader.ReadUInt16());
			if(rc4Encrypted) {
				short versionMajor = reader.ReadInt16();
				short versionMinor = reader.ReadInt16();
				if(versionMajor == 1 && versionMinor == 1)
					this.rc4EncryptionHeader = new XlsRC4EncryptionHeader();
				else if(versionMajor >= 2 && versionMinor == 2)
					this.rc4EncryptionHeader = new XlsRC4CryptoAPIEncryptionHeader();
				else
					throw new InvalidFileException("Unknown FilePass header version");
				reader.Seek(-4, SeekOrigin.Current);
				this.rc4EncryptionHeader.Read(reader);
			}
			else {
				this.xorObfuscation = XlsXORObfuscation.FromStream(reader);
			}
		}
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			if(rc4Encrypted) {
				XlsRC4EncryptionHeader header = this.rc4EncryptionHeader as XlsRC4EncryptionHeader;
				if(header != null) { 
					ARC4PasswordVerifier verifier = new ARC4PasswordVerifier(header.Salt, header.EncryptedVerifier, header.EncryptedVerifierHash);
					string password = contentBuilder.Options.Password;
					bool tryingDefaultPassword = string.IsNullOrEmpty(password);
					if(tryingDefaultPassword)
						password = MagicMSPassword;
					if(!verifier.VerifyPassword(password)) {
						if(tryingDefaultPassword)
							throw new EncryptedFileException(EncryptedFileError.PasswordRequired, "Password required to open this file!");
						else
							throw new EncryptedFileException(EncryptedFileError.WrongPassword, "Wrong password!");
					}
					contentBuilder.SetupRC4Decryptor(password, header.Salt);
				}
				else {
					throw new EncryptedFileException(EncryptedFileError.EncryptionTypeNotSupported,
						"RC4 CryptoAPI encrypted files is not supported!");
				}
			}
			else {
				throw new EncryptedFileException(EncryptedFileError.EncryptionTypeNotSupported,
					"XOR obfuscated files is not supported!");
			}
		}
	}
	#endregion
	#region XlsSourceCommandIndex
	public class XlsSourceCommandIndex : XlsSourceCommandContentBase {
		XlsContentIndex content = new XlsContentIndex();
		public override void Execute(XlsSourceDataReader dataReader) {
			dataReader.FirstRowIndex = content.FirstRowIndex;
			dataReader.LastRowIndex = content.LastRowIndex;
			dataReader.DefaultColumnWidthOffset = content.DefaultColumnWidthOffset;
			dataReader.DbCellPositions.Clear();
			dataReader.DbCellPositions.AddRange(content.DbCellsPositions);
		}
		protected override IXlsContent GetContent() {
			content = new XlsContentIndex();
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandDefaultColumnWidth
	public class XlsSourceCommandDefaultColumnWidth : XlsSourceCommandEmpty {
	}
	#endregion
	#region XlsSourceCommandColumnInfo
	public class XlsSourceCommandColumnInfo : XlsSourceCommandContentBase {
		XlsContentColumnInfo content = new XlsContentColumnInfo();
		public override void Execute(XlsSourceDataReader dataReader) {
			ColumnInfo column = new ColumnInfo(content.FirstColumn, content.LastColumn, content.Hidden, content.FormatIndex);
			dataReader.Columns.Add(column);
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandDbCell
	public class XlsSourceCommandDbCell : XlsSourceCommandContentBase {
		XlsContentDbCell content = new XlsContentDbCell();
		public override void Execute(XlsSourceDataReader dataReader) {
			if(dataReader.Stage != XlsSourceReaderStage.Data)
				return;
			dataReader.CellOffsets.Clear();
			dataReader.FirstRowOffset = content.FirstRowOffset;
			dataReader.CellOffsets.AddRange(content.StreamOffsets);
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandRow
	public class XlsSourceCommandRow : XlsSourceCommandContentBase {
		XlsContentRow content = new XlsContentRow();
		public override void Execute(XlsSourceDataReader dataReader) {
			XlsRow row = new XlsRow(content.Index, content.FirstColumnIndex, content.LastColumnIndex, content.FormatIndex, content.IsHidden);
			dataReader.AddRow(row);
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandCellBase (abstract)
	public abstract class XlsSourceCommandCellBase : XlsSourceCommandContentBase {
		public abstract int RowIndex { get; }
	}
	#endregion
	#region XlsSourceCommandSingleCellBase (abstract)
	public abstract class XlsSourceCommandSingleCellBase : XlsSourceCommandCellBase {
		public override int RowIndex { get { return Content.RowIndex; } }
		XlsContentCellBase Content { get { return GetContent() as XlsContentCellBase; } }
	}
	#endregion
	#region XlsSourceCommandBlank
	public class XlsSourceCommandBlank : XlsSourceCommandSingleCellBase {
		XlsContentBlank content = new XlsContentBlank();
		public override void Execute(XlsSourceDataReader dataReader) {
			if(dataReader.Stage == XlsSourceReaderStage.Index)
				dataReader.RegisterCell(RowIndex);
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandMulBlank
	public class XlsSourceCommandMulBlank : XlsSourceCommandCellBase {
		XlsContentMulBlank content = new XlsContentMulBlank();
		public override int RowIndex { get { return content.RowIndex; } }
		public override void Execute(XlsSourceDataReader dataReader) {
			if(dataReader.Stage == XlsSourceReaderStage.Index)
				dataReader.RegisterCell(RowIndex);
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandBoolErr
	public class XlsSourceCommandBoolErr : XlsSourceCommandSingleCellBase {
		XlsContentBoolErr content = new XlsContentBoolErr();
		#region Properties
		bool BoolValue {
			get { return content.Value != 0; }
		}
		XlVariantValue ErrorValue {
			get {
				if(this.IsError)
					return XlCellErrorFactory.CreateError((XlCellErrorType)content.Value);
				return XlVariantValue.Empty;
			}
		}
		bool IsError { get { return content.IsError; } }
		#endregion
		public override void Execute(XlsSourceDataReader dataReader) {
			if(dataReader.Stage == XlsSourceReaderStage.Index)
				dataReader.RegisterCell(RowIndex);
			else {
				int columnIndex = dataReader.TranslateColumnIndex(content.ColumnIndex);
				if(columnIndex < 0)
					return;
				if(IsError)
					dataReader.AddCell(new Cell(columnIndex, ErrorValue, content.ColumnIndex, content.FormatIndex));
				else
					dataReader.AddCell(new Cell(columnIndex, BoolValue, content.ColumnIndex, content.FormatIndex));
			}
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandNumber
	public class XlsSourceCommandNumber : XlsSourceCommandSingleCellBase {
		XlsContentNumber content = new XlsContentNumber();
		public override void Execute(XlsSourceDataReader dataReader) {
			if(dataReader.Stage == XlsSourceReaderStage.Index)
				dataReader.RegisterCell(RowIndex);
			else {
				int columnIndex = content.ColumnIndex;
				int fieldIndex = dataReader.TranslateColumnIndex(columnIndex);
				if(fieldIndex < 0)
					return;
				int formatIndex = content.FormatIndex;
				dataReader.AddCell(new Cell(fieldIndex, dataReader.GetDateTimeOrNumericValue(content.Value, formatIndex, columnIndex), columnIndex, formatIndex));
			}
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandLabel
	public class XlsSourceCommandLabel : XlsSourceCommandSingleCellBase {
		XlsContentLabel content = new XlsContentLabel();
		public override void Execute(XlsSourceDataReader dataReader) {
			if(dataReader.Stage == XlsSourceReaderStage.Index)
				dataReader.RegisterCell(RowIndex);
			else {
				int columnIndex = dataReader.TranslateColumnIndex(content.ColumnIndex);
				if(columnIndex < 0)
					return;
				dataReader.AddCell(new Cell(columnIndex, content.Value, content.ColumnIndex, content.FormatIndex));
			}
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandLabelSst
	public class XlsSourceCommandLabelSst : XlsSourceCommandSingleCellBase {
		XlsContentLabelSst content = new XlsContentLabelSst();
		public override void Execute(XlsSourceDataReader dataReader) {
			if(dataReader.Stage == XlsSourceReaderStage.Index)
				dataReader.RegisterCell(RowIndex);
			else {
				int columnIndex = dataReader.TranslateColumnIndex(content.ColumnIndex);
				if(columnIndex < 0)
					return;
				string value = dataReader.GetSharedString(content.StringIndex);
				dataReader.AddCell(new Cell(columnIndex, value, content.ColumnIndex, content.FormatIndex));
			}
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandRk
	public class XlsSourceCommandRk : XlsSourceCommandSingleCellBase {
		XlsContentRk content = new XlsContentRk();
		public override void Execute(XlsSourceDataReader dataReader) {
			if(dataReader.Stage == XlsSourceReaderStage.Index)
				dataReader.RegisterCell(RowIndex);
			else {
				int columnIndex = content.ColumnIndex;
				int fieldIndex = dataReader.TranslateColumnIndex(columnIndex);
				if(fieldIndex < 0)
					return;
				int formatIndex = content.FormatIndex;
				dataReader.AddCell(new Cell(fieldIndex, dataReader.GetDateTimeOrNumericValue(content.Value, formatIndex, columnIndex), columnIndex, formatIndex));
			}
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandMulRk
	public class XlsSourceCommandMulRk : XlsSourceCommandCellBase {
		XlsContentMulRk content = new XlsContentMulRk();
		public override int RowIndex { get { return content.RowIndex; } }
		public override void Execute(XlsSourceDataReader dataReader) {
			if(dataReader.Stage == XlsSourceReaderStage.Index)
				dataReader.RegisterCell(RowIndex);
			else {
				int count = content.RkRecords.Count;
				for(int i = 0; i < count; i++) {
					XlsRkRec rec = content.RkRecords[i];
					int columnIndex = content.FirstColumnIndex + i;
					int fieldIndex = dataReader.TranslateColumnIndex(columnIndex);
					if(fieldIndex < 0)
						continue;
					int formatIndex = rec.FormatIndex;
					dataReader.AddCell(new Cell(fieldIndex, dataReader.GetDateTimeOrNumericValue(rec.Rk.Value, formatIndex, columnIndex), columnIndex, formatIndex));
				}
			}
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandFormula
	public class XlsSourceCommandFormula : XlsSourceCommandSingleCellBase {
		XlsContentFormula content = new XlsContentFormula();
		string stringValue;
		bool hasStringValue;
		internal string StringValue { 
			get { return stringValue; }
			set {
				stringValue = value;
				hasStringValue = true;
			}
		}
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			this.stringValue = null;
			this.hasStringValue = false;
			base.ReadCore(reader, contentBuilder);
		}
		public override void Execute(XlsSourceDataReader dataReader) {
			if(dataReader.Stage == XlsSourceReaderStage.Index)
				dataReader.RegisterCell(RowIndex);
			else {
				int columnIndex = content.ColumnIndex;
				int fieldIndex = dataReader.TranslateColumnIndex(columnIndex);
				if(fieldIndex < 0)
					return;
				int formatIndex = content.FormatIndex;
				XlVariantValue value = GetValue(dataReader, formatIndex, columnIndex);
				if(!value.IsEmpty)
					dataReader.AddCell(new Cell(fieldIndex, value, columnIndex, formatIndex));
			}
		}
		XlVariantValue GetValue(XlsSourceDataReader dataReader, int formatIndex, int columnIndex) {
			XlsFormulaValue formulaValue = content.Value;
			if(formulaValue.IsBoolean)
				return formulaValue.BooleanValue;
			else if(formulaValue.IsError)
				return XlCellErrorFactory.CreateError((XlCellErrorType)formulaValue.ErrorCode);
			else if(formulaValue.IsNumeric)
				return dataReader.GetDateTimeOrNumericValue(formulaValue.NumericValue, formatIndex, columnIndex);
			else if(formulaValue.IsString && hasStringValue)
				return stringValue;
			else if(formulaValue.IsBlankString)
				return string.Empty;
			return XlVariantValue.Empty;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsSourceCommandString
	public class XlsSourceCommandString : XlsSourceCommandDataCollectorBase {
		XLUnicodeString value = new XLUnicodeString();
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			this.value = new XLUnicodeString();
			base.ReadCore(reader, contentBuilder);
		}
		protected override void ReadData(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			this.value.ReadData(reader);
			if(this.value.Complete) {
				XlsSourceCommandFormula command = contentBuilder.CommandFactory.CreateCommand(0x0006) as XlsSourceCommandFormula;
				if(command != null) {
					command.StringValue = this.value.Value;
					command.Execute(contentBuilder.DataReader);
				}
			}
		}
		protected override bool GetCompleted() {
			return this.value.Complete;
		}
	}
	#endregion
	#region XlsSourceTableFeature
	public class XlsSourceTableFeature {
		int tableSource;
		uint tableFlags;
		XLUnicodeString tableName = new XLUnicodeString();
		readonly List<TableColumnInfo> columns = new List<TableColumnInfo>();
		bool abortRead;
		public bool HasHeaderRow { get; private set; }
		public bool HasTotalRow { get; private set; }
		public string TableName { get { return this.tableName.Value; } }
		public IList<TableColumnInfo> Columns { get { return columns; } }
		public void Read(BinaryReader reader) {
			this.abortRead = false;
			this.tableSource = reader.ReadInt32(); 
			reader.ReadBytes(4); 
			HasHeaderRow = reader.ReadUInt32() != 0; 
			HasTotalRow = reader.ReadUInt32() != 0; 
			reader.ReadBytes(4); 
			reader.ReadBytes(4); 
			reader.ReadBytes(2); 
			reader.ReadBytes(2); 
			tableFlags = reader.ReadUInt32();
			reader.ReadBytes(4); 
			reader.ReadBytes(4); 
			reader.ReadBytes(4); 
			reader.ReadBytes(4); 
			reader.ReadBytes(16); 
			this.tableName = XLUnicodeString.FromStream(reader);
			int cFieldData = reader.ReadUInt16();
			if((tableFlags & 0x4000) != 0)
				XLUnicodeString.FromStream(reader); 
			if((tableFlags & 0x100000) != 0)
				XLUnicodeString.FromStream(reader); 
			for(int i = 0; i < cFieldData && !abortRead; i++) {
				TableColumnInfo column = new TableColumnInfo();
				ReadFieldDataItem(reader, column);
				this.columns.Add(column);
			}
		}
		void ReadFieldDataItem(BinaryReader reader, TableColumnInfo column) {
			reader.ReadBytes(4); 
			int dataProviderType = reader.ReadInt32(); 
			reader.ReadBytes(4); 
			reader.ReadBytes(4); 
			uint cbFmtAgg = reader.ReadUInt32();
			reader.ReadBytes(4); 
			int flags = reader.ReadInt32();
			uint cbFmtInsertRow = reader.ReadUInt32();
			reader.ReadBytes(4); 
			XLUnicodeString.FromStream(reader); 
			if((tableFlags & 0x0200) == 0)
				column.Name = XLUnicodeString.FromStream(reader).Value; 
			if(cbFmtAgg > 0)
				ReadDXFN12List(reader, (int)cbFmtAgg, delegate(XlNumberFormat format) { column.TotalRowNumberFormat = format; });
			if(cbFmtInsertRow > 0)
				ReadDXFN12List(reader, (int)cbFmtInsertRow, delegate(XlNumberFormat format) { column.NumberFormat = format; });
			if((tableFlags & 0x0002) != 0) {
				uint cbAutoFilter = reader.ReadUInt32();
				reader.ReadBytes(2); 
				if(cbAutoFilter > 0)
					reader.ReadBytes((int)cbAutoFilter); 
			}
			if((flags & 0x0004) != 0) {
				ushort iXmapMac = reader.ReadUInt16();
				for(int i = 0; i < iXmapMac; i++) {
					reader.ReadBytes(4); 
					reader.ReadBytes(4); 
					XLUnicodeString.FromStream(reader); 
				}
			}
			if((flags & 0x0008) != 0) {
				ushort cbFmla = reader.ReadUInt16();
				if(cbFmla > 0)
					reader.ReadBytes(cbFmla); 
			}
			if((flags & 0x0080) != 0) {
				ushort cbFmla = reader.ReadUInt16();
				if(cbFmla > 0) {
					byte[] totalFmla = reader.ReadBytes(cbFmla); 
					if((flags & 0x0100) != 0)
						ReadTotalFormulaExtra(reader, totalFmla);
					if(abortRead)
						return;
				}
			}
			if((flags & 0x0400) != 0)
				XLUnicodeString.FromStream(reader);
			if(tableSource == 0x01)
				ReadWSSInfo(reader, dataProviderType);
			if(tableSource == 0x03)
				reader.ReadBytes(4); 
			if(!HasHeaderRow && ((tableFlags & 0x0200) == 0)) {
				int cbHdrDisk = reader.ReadInt32();
				if(cbHdrDisk > 0)
					reader.ReadBytes(cbHdrDisk);
				if((flags & 0x0200) != 0)
					XLUnicodeString.FromStream(reader);
			}
		}
		void ReadDXFN12List(BinaryReader reader, int size, Action<XlNumberFormat> action) {
			long startPos = reader.BaseStream.Position;
			reader.ReadUInt16();
			ushort flags = reader.ReadUInt16();
			ushort flags2 = reader.ReadUInt16();
			if(Convert.ToBoolean(flags & 0x0200)) {
				if(Convert.ToBoolean(flags2 & 0x0001)) {
					int cb = reader.ReadUInt16();
					if(cb > 2)
						action(XLUnicodeString.FromStream(reader).Value);
				}
				else {
					reader.ReadBytes(1); 
					action(XlNumberFormat.FromId(reader.ReadByte()));
				}
			}
			if(Convert.ToBoolean(flags & 0x0400)) {
				reader.ReadBytes(1); 
				byte[] buf = reader.ReadBytes(63); 
				reader.ReadBytes(16); 
				reader.ReadBytes(4); 
				reader.ReadBytes(4); 
				reader.ReadBytes(4); 
				reader.ReadBytes(4); 
				reader.ReadBytes(4); 
				reader.ReadBytes(4); 
				reader.ReadBytes(4); 
				reader.ReadBytes(4); 
				reader.ReadBytes(4); 
				reader.ReadBytes(2); 
			}
			if(Convert.ToBoolean(flags & 0x0800))
				reader.ReadBytes(8); 
			if(Convert.ToBoolean(flags & 0x1000))
				reader.ReadBytes(8);
			if(Convert.ToBoolean(flags & 0x2000))
				reader.ReadBytes(4);
			if(Convert.ToBoolean(flags & 0x4000))
				reader.ReadBytes(2);
			long endPos = reader.BaseStream.Position;
			int bytesToRead = size - (int)(endPos - startPos);
			if(bytesToRead > 0)
				reader.ReadBytes(bytesToRead); 
		}
		void ReadTotalFormulaExtra(BinaryReader reader, byte[] totalFormula) {
			abortRead = true;
		}
		void ReadWSSInfo(BinaryReader reader, int dataProviderType) {
			reader.ReadBytes(4); 
			reader.ReadBytes(4); 
			reader.ReadBytes(4); 
			bool loadFormula = (reader.ReadUInt32() & 0x0040) != 0;
			switch(dataProviderType) {
				case 0x0001:
				case 0x0008:
				case 0x000b:
					XLUnicodeString.FromStream(reader);
					break;
				case 0x0002:
				case 0x0004:
				case 0x0006:
					reader.ReadBytes(8);
					break;
				case 0x0003:
					reader.ReadBytes(4);
					break;
			}
			if(loadFormula)
				XLUnicodeString.FromStream(reader);
			reader.ReadBytes(4); 
		}
	}
	#endregion
	#region XlsSourceCommandFeature11
	public class XlsSourceCommandFeature11 : XlsSourceCommandBase {
		#region Static Members
		static short[] typeCodes = new short[] {
			0x0875, 
			0x0812  
		};
		#endregion
		enum SharedFeatureType {
			Unknown = 0,
			Protection = 2,
			IgnoredErrors = 3,
			SmartTag = 4,
			Table = 5
		}
		SharedFeatureType featureType;
		XlsRef8 tableRef;
		XlsSourceTableFeature tableFeature;
		protected override void ReadCore(XlsReader reader, XlsSpreadsheetSource contentBuilder) {
			reader.Seek(12, SeekOrigin.Current); 
			featureType = (SharedFeatureType)reader.ReadUInt16();
			if(featureType != SharedFeatureType.Table) {
				SkipRestOfData(reader, 14);
				return;
			}
			reader.Seek(5, SeekOrigin.Current); 
			int refCount = reader.ReadUInt16();
			if(refCount != 1) {
				featureType = SharedFeatureType.Unknown;
				SkipRestOfData(reader, 21);
				return;
			}
			reader.Seek(6, SeekOrigin.Current); 
			tableRef = XlsRef8.FromStream(reader);
			using(XlsCommandStream commandStream = new XlsCommandStream(reader, typeCodes, Size - 35)) {
				using(BinaryReader commandReader = new BinaryReader(commandStream)) {
					this.tableFeature = new XlsSourceTableFeature();
					this.tableFeature.Read(commandReader);
				}
			}
		}
		public override void Execute(XlsSpreadsheetSource contentBuilder) {
			if(featureType != SharedFeatureType.Table || contentBuilder.CurrentSheet == null || this.tableFeature == null)
				return;
			XlCellRange range = XlCellRange.FromLTRB(tableRef.FirstColumnIndex, tableRef.FirstRowIndex, tableRef.LastColumnIndex, tableRef.LastRowIndex);
			range.SheetName = contentBuilder.CurrentSheet.Name;
			Table table = new Table(tableFeature.TableName, range, tableFeature.HasHeaderRow, tableFeature.HasTotalRow);
			table.Columns.AddRange(tableFeature.Columns);
			contentBuilder.InnerTables.Add(table);
			this.tableFeature = null;
		}
		protected override void CheckPosition(XlsReader reader, long initialPosition, long expectedPosition) {
		}
		void SkipRestOfData(XlsReader reader, int count) {
			int bytesToSkip = Size - count;
			if(bytesToSkip > 0)
				reader.Seek(bytesToSkip, SeekOrigin.Current);
		}
	}
	#endregion
}
