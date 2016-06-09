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
using System.Reflection;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region IXlsCommand
	public interface IXlsCommand {
		void Read(XlsReader reader, XlsContentBuilder contentBuilder);
		void Execute(XlsContentBuilder contentBuilder);
		void Write(BinaryWriter writer);
		IXlsCommand GetInstance();
		short GetRecordSize();
	}
	#endregion
	#region XlsCommandBase (abstract class)
	public abstract class XlsCommandBase : IXlsCommand {
		#region Fields
		short size;
		#endregion
		#region Properties
		protected internal short Size {
			get { return this.size; }
			set {
				ValueChecker.CheckValue((int)value, 0, XlsDefs.MaxRecordDataSize, "Record data size");
				this.size = value; 
			}
		}
		#endregion
		#region IXlsCommand Members
		protected virtual short GetSize() {
			return 2;
		}
		public void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort dataSize = reader.ReadNotCryptedUInt16();
			if(dataSize > XlsDefs.MaxRecordDataSize)
				contentBuilder.ThrowInvalidFile(string.Format("Record data size greater than {0}", XlsDefs.MaxRecordDataSize));
			this.size = (short)dataSize;
			long initialPosition = reader.Position;
			long expectedPosition = initialPosition + this.size;
			ReadCore(reader, contentBuilder);
			CheckPosition(reader, contentBuilder, initialPosition, expectedPosition);
		}
		protected virtual void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
			long actualPosition = reader.Position;
			if (actualPosition < expectedPosition)
				reader.Seek(expectedPosition - actualPosition, SeekOrigin.Current); 
			else if (actualPosition > expectedPosition)
				contentBuilder.ThrowInvalidFile(
					string.Format("Read failure: initial/expected/actual positions = {0}/{1}/{2}, command={3}",
					initialPosition, expectedPosition, actualPosition, this.GetType().ToString()));
		}
		protected abstract void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder);
		public virtual void Execute(XlsContentBuilder contentBuilder) {
			switch(contentBuilder.ContentType) {
				case XlsContentType.Sheet:
					ApplySheetContent(contentBuilder);
					break;
				case XlsContentType.WorkbookGlobals:
					ApplyWorkbookGlobalsContent(contentBuilder);
					break;
				case XlsContentType.Chart:
					ApplyChartContent(contentBuilder);
					break;
				case XlsContentType.ChartSheet:
					ApplyChartSheetContent(contentBuilder);
					break;
				case XlsContentType.PivotCache:
					ApplyPivotCacheContent(contentBuilder);
					break;
				case XlsContentType.MacroSheet:
					ApplyMacroSheetContent(contentBuilder);
					break;
				case XlsContentType.VisualBasicModule:
					ApplyVisualBasicModuleContent(contentBuilder);
					break;
				case XlsContentType.Workspace:
					ApplyWorkspaceContent(contentBuilder);
					break;
				case XlsContentType.SheetCustomView:
					ApplySheetCustomViewContent(contentBuilder);
					break;
				case XlsContentType.ChartCustomView:
					ApplyChartCustomViewContent(contentBuilder);
					break;
				case XlsContentType.ChartSheetCustomView:
					ApplyChartSheetCustomViewContent(contentBuilder);
					break;
				case XlsContentType.MacroSheetCustomView:
					ApplyMacroSheetCustomViewContent(contentBuilder);
					break;
				case XlsContentType.VisualBasicModuleCustomView:
					ApplyVisualBasicModuleCustomViewContent(contentBuilder);
					break;
			}
		}
		protected virtual void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplySheetContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplyChartContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplyChartSheetContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplyMacroSheetContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplyVisualBasicModuleContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplyWorkspaceContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplySheetCustomViewContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplyChartCustomViewContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplyChartSheetCustomViewContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplyMacroSheetCustomViewContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplyVisualBasicModuleCustomViewContent(XlsContentBuilder contentBuilder) {
		}
		protected virtual void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
		}
		public virtual void Write(BinaryWriter writer) {
			writer.Write(XlsCommandFactory.GetTypeCodeByType(GetType()));
			writer.Write(GetSize());
			WriteCore(writer);
		}
		protected abstract void WriteCore(BinaryWriter writer);
		public abstract IXlsCommand GetInstance();
		public short GetRecordSize() {
			return (short)(GetSize() + 4);
		}
		#endregion
	}
	#endregion
	#region XlsCommandEncodingBase (abstract class)
	public abstract class XlsCommandEncodingBase : XlsCommandContentBase {
		XlsContentEncoding content = new XlsContentEncoding();
		public Encoding Encoding {
			get { return this.content.Value; }
			set { this.content.Value = value; }
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandBoolPropertyBase
	public abstract class XlsCommandBoolPropertyBase : XlsCommandBase {
		bool value;
		public bool Value { get { return this.value; } set { this.value = value; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.value = reader.ReadInt16() == 0x01;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(Convert.ToInt16(this.value));
		}
	}
	#endregion
	#region XlsCommandShortPropertyValueBase
	public abstract class XlsCommandShortPropertyValueBase : XlsCommandBase {
		short value;
		public short Value { get { return this.value; } set { this.value = value; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.value = reader.ReadInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(this.value);
		}
	}
	#endregion
	#region XlsCommandDoublePropertyValueBase
	public abstract class XlsCommandDoublePropertyValueBase : XlsCommandBase {
		double value;
		public double Value { get { return this.value; } set { this.value = value; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.value = reader.ReadDouble();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(this.value);
		}
		protected override short GetSize() {
			return sizeof(double);
		}
	}
	#endregion
	#region XlsCommandStringValueBase
	public abstract class XlsCommandStringValueBase : XlsCommandBase {
		XLUnicodeString internalString = new XLUnicodeString();
		public string Value {
			get { return internalString.Value; }
			set {
				if(string.IsNullOrEmpty(value))
					internalString.Value = value;
				else {
					if(value.Length > GetMaxLength())
						throw new ArgumentException("String value too long");
					internalString.Value = value;
				}
			}
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if(Size > 0)
				this.internalString = XLUnicodeString.FromStream(reader);
			else
				this.internalString.Value = string.Empty;
		}
		protected override void WriteCore(BinaryWriter writer) {
			if (!string.IsNullOrEmpty(this.internalString.Value))
				this.internalString.Write(writer);
		}
		protected override short GetSize() {
			if(string.IsNullOrEmpty(this.internalString.Value)) return 0;
			return (short)this.internalString.Length;
		}
		protected abstract int GetMaxLength();
	}
	#endregion
	#region XlsCommandRecordBase (abstract class)
	public abstract class XlsCommandRecordBase : XlsCommandBase, IXlsChunk {
		byte[] data = new byte[0];
		#region Properties
		public byte[] Data {
			get { return data; }
			set {
				if(value != null) {
					CheckDataLength(value);
					data = value;
				}
				else
					data = new byte[0];
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Data = reader.ReadBytes(Size);
		}
		protected override void WriteCore(BinaryWriter writer) {
			if (this.data.Length > 0)
				writer.Write(this.data);
		}
		protected override short GetSize() {
			return (short)this.data.Length;
		}
		protected void CheckDataLength(byte[] value) {
			if (value == null) return;
			if(value.Length > GetMaxDataSize())
				throw new ArgumentException("Data exceed maximun record data length");
		}
		public virtual int GetMaxDataSize() {
			return XlsDefs.MaxRecordDataSize;
		}
	}
	#endregion
	#region IXLsDataCollector
	public interface IXlsDataCollector {
		void PutData(byte[] data, XlsContentBuilder contentBuilder);
	}
	#endregion
	#region XlsCommandDataCollectorBase (abstract class)
	public abstract class XlsCommandDataCollectorBase : XlsCommandRecordBase, IXlsDataCollector {
		#region IXlsDataCollector Members
		public virtual void PutData(byte[] data, XlsContentBuilder contentBuilder) {
			MemoryStream stream = new MemoryStream(data, false);
			using (BinaryReader baseReader = new BinaryReader(stream)) {
				using (XlsReader reader = new XlsReader(baseReader)) {
					ReadData(reader, contentBuilder);
				}
			}
			if(GetCompleted())
				contentBuilder.PopDataCollector();
		}
		#endregion
		protected virtual void ReadData(XlsReader reader, XlsContentBuilder contentBuilder) {
		}
		protected virtual bool GetCompleted() {
			return false;
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if(Data.Length > 0) {
				contentBuilder.PushDataCollector(this);
				PutData(Data, contentBuilder);
			}
		}
	}
	#endregion
	#region XlsCommandContinueFrtBase
	public abstract class XlsCommandContinueFrtBase : XlsCommandRecordBase {
		public FutureRecordHeaderBase FrtHeader { get; set; }
		protected XlsCommandContinueFrtBase() {
			FrtHeader = XlsFutureRecordHeaderFactory.Create(XlsCommandFactory.GetTypeCodeByType(GetType()));
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			short typeCode = XlsCommandFactory.GetTypeCodeByType(GetType());
			FrtHeader = XlsFutureRecordHeaderFactory.FromStream(typeCode, reader);
			Data = reader.ReadBytes(Size - GetFrtHeaderSize());
		}
		protected override void WriteCore(BinaryWriter writer) {
			if(FrtHeader != null)
				FrtHeader.Write(writer);
			base.WriteCore(writer);
		}
		protected override short GetSize() {
			return (short)(base.GetSize() + GetFrtHeaderSize());
		}
		public override int GetMaxDataSize() {
			return XlsDefs.MaxRecordDataSize - GetFrtHeaderSize();
		}
		protected short GetFrtHeaderSize() {
			if(FrtHeader == null)
				return 0;
			return FrtHeader.GetSize();
		}
	}
	#endregion
	#region XlsCommandContentBase
	public abstract class XlsCommandContentBase : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			IXlsContent content = GetContent();
			if (content != null)
				content.Read(reader, Size);
		}
		protected override void WriteCore(BinaryWriter writer) {
			IXlsContent content = GetContent();
			if (content != null)
				content.Write(writer);
		}
		protected override short GetSize() {
			IXlsContent content = GetContent();
			return (short)(content != null ? content.GetSize() : 0);
		}
		protected abstract IXlsContent GetContent();
	}
	#endregion
}
