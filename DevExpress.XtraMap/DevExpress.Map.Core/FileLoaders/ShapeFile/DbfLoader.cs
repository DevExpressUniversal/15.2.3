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
using System.IO;
using System.Globalization;
using DevExpress.Utils;
namespace DevExpress.Map.Native {
	public enum DbfTypes {
		FoxBase = 0x02,
		WithoutDBT = 0x03,
		VisualFoxPro = 0x30,
		Flagship = 0x43,
		WithDBT = 0x83,
		DBase4 = 0x8B,
		FoxProWithMemo = 0xF5,
		FoxPro = 0xFB
	}
	public class DbfField {
		public string FieldName { get; set; }
		public byte FieldType { get; set; }
		public byte FieldLengthInBytes { get; set; }
		public byte NumberOfDecimalPlaces { get; set; }
		public byte FieldFlags { get; set; }
		public byte AutoIncrementStepValue { get; set; }
		public Type GetFieldType() {
			switch ((char)FieldType) {
				case 'B':
				case 'F': {
					return typeof(double);
				}
				case 'D':
				case 'T': {
					return typeof(DateTime);
				}
				case 'I': {
					return typeof(int);
				}
				case 'N': {
					if (NumberOfDecimalPlaces > 0)
						return typeof(double);
					if(FieldLengthInBytes > 9) {
						return typeof(Int64);
					}
					return typeof(Int32);
				}
				case 'L': {
					return typeof(bool);
				}
				default: {
					return typeof(string);
				}
			}
		}
		public static char GetFieldTypeChar(Type type) {
			if(type.Equals(typeof(Int32))) return 'N';
			if(type.Equals(typeof(double))) return 'N';
			if(type.Equals(typeof(bool))) return 'L';
			return 'C';
		}
		public byte GetSize(object value) {
			if((char)FieldType == 'L')
				return 1;
			return (byte)value.ToString().Length;;
		}
	}
	public class DbfHeader {
		readonly List<DbfField> fields = new List<DbfField>();
		readonly int recordsOffset;
		readonly int recordLength;
		readonly int recordCount;
		public int RecordsOffset { get { return recordsOffset; } }
		public int RecordLength { get { return recordLength; } }
		public int RecordCount { get { return recordCount; } }
		public List<DbfField> Fields { get { return fields; } }
		public DbfHeader(int recordsOffset, int recordLength, int recordCount) {
			this.recordsOffset = recordsOffset;
			this.recordLength = recordLength;
			this.recordCount = recordCount;
		}
	}
	public class DbfMapItemAttribute : IMapItemAttribute {
		public string Name { get; set; }
		public Type Type { get; set; }
		public object Value { get; set; }
	}
	public class DbfRecord {
		readonly Dictionary<string, object> fields = new Dictionary<string, object>();
		public IEnumerable<IMapItemAttribute> Attributes {
			get {
				foreach (string key in fields.Keys) {
					yield return CreateMapItemAttribute(key, fields[key], fields[key].GetType());
				}
			}
		}
		protected IMapItemAttribute CreateMapItemAttribute(string name, object value, Type type) {
			return new DbfMapItemAttribute() { Name = name, Value = value, Type = type };
		}
		public void AddField(string name, object value) {
			if (!fields.ContainsKey(name))
				fields.Add(name, value);
			else
				fields[name] = value;
		}
	}
	public class DbfLoaderCore : IDisposable {
		readonly DbfHeader header;
		readonly BinaryReader reader;
		readonly List<DbfRecord> records = new List<DbfRecord>();
		Encoding defaultEncoding = DXEncoding.Default;
		public DbfHeader Header { get { return header; } }
		public List<DbfRecord> Records { get { return records; } }
		public Encoding DefaultEncoding { 
			get { return defaultEncoding; } 
			set {
				if (Object.Equals(value, defaultEncoding))
					return;
				defaultEncoding = value ?? DXEncoding.Default; 
			} 
		}
		public DbfLoaderCore(Stream stream) : this(stream, DXEncoding.Default) {
		}
		public DbfLoaderCore(Stream stream, Encoding defaultEncoding) {
			this.reader = new BinaryReader(stream);
			DefaultEncoding = defaultEncoding;
			Encoding textEncoding;
			this.header = LoadHeader(out textEncoding);
			for (int i = 0; i < header.RecordCount; i++) {
				DbfRecord record = LoadRecord(textEncoding);
				if (record != null)
					records.Add(record);
			}
		}
		protected DbfRecord CreateEmptyDbfRecord() {
			return new DbfRecord();
		}
		bool CheckFileType(byte fileType) {
			return Enum.IsDefined(typeof(DbfTypes), (DbfTypes)fileType);
		}
		DbfHeader LoadHeader(out Encoding textEncoding) {
			textEncoding = null;
			byte fileType = reader.ReadByte();
			if (!CheckFileType(fileType))
				throw new Exception("Unsupported Dbf FIle Format");
			try {
				reader.BaseStream.Seek(3L, SeekOrigin.Current);
				int recordCount = (int)reader.ReadUInt32();
				int recordsOffset = reader.ReadUInt16();
				int recordLength = reader.ReadUInt16();
				DbfHeader result = new DbfHeader(recordsOffset, recordLength, recordCount);
				reader.ReadUInt16();
				reader.ReadByte();
				reader.ReadByte();
				reader.ReadUInt32();
				reader.ReadBytes(8);
				reader.ReadByte();
				byte languageDriver = reader.ReadByte();
				textEncoding = GetEncoding(languageDriver);
				reader.ReadUInt16();
				while (reader.PeekChar() != 13) {
					DbfField field = new DbfField();
					byte[] bytes = reader.ReadBytes(11);
					field.FieldName = textEncoding.GetString(bytes, 0, 11).Replace("\0", string.Empty);
					field.FieldType = reader.ReadByte();
					reader.ReadUInt32(); 
					field.FieldLengthInBytes = reader.ReadByte();
					field.NumberOfDecimalPlaces = reader.ReadByte();
					field.FieldFlags = reader.ReadByte();
					reader.ReadUInt32(); 
					field.AutoIncrementStepValue = reader.ReadByte();
					reader.ReadBytes(8);
					result.Fields.Add(field);
				}
				reader.ReadByte();
				return result;
			}
			catch {
				throw new Exception("Incorrect Dbf FIle Format");
			}
		}
		Encoding GetEncoding(byte languageDriver) {
			Encoding encoding = DefaultEncoding;
			switch (languageDriver) {
				case 1: {
					encoding = Encoding.GetEncoding("cp437");
					break;
				}
				case 2: {
					encoding = Encoding.GetEncoding("cp850");
					break;
				}
				case 3: {
					encoding = Encoding.GetEncoding("windows-1252");
					break;
				}
				case 4: {
					encoding = Encoding.GetEncoding("macintosh");
					break;
				}
				case 100: {
					encoding = Encoding.GetEncoding("cp852");
					break;
				}
				case 101: {
					encoding = Encoding.GetEncoding("cp865");
					break;
				}
				case 102: {
					encoding = Encoding.GetEncoding("cp866");
					break;
				}
				case 103: {
					encoding = Encoding.GetEncoding("cp861");
					break;
				}
				case 104: {
					encoding = Encoding.GetEncoding("cp852");
					break;
				}
				case 105: {
					encoding = Encoding.GetEncoding("cp437");
					break;
				}
				case 106: {
					encoding = Encoding.GetEncoding("ibm737");
					break;
				}
				case 107: {
					encoding = Encoding.GetEncoding("cp857");
					break;
				}
				case 150: {
					encoding = Encoding.GetEncoding("x-mac-cyrillic");
					break;
				}
				case 151: {
					encoding = Encoding.GetEncoding("x-mac-ce");
					break;
				}
				case 152: {
					encoding = Encoding.GetEncoding("x-mac-greek");
					break;
				}
				case 200: {
					encoding = Encoding.GetEncoding("windows-1250");
					break;
				}
				case 201: {
					encoding = Encoding.GetEncoding("windows-1251");
					break;
				}
				case 202: {
					encoding = Encoding.GetEncoding("windows-1254");
					break;
				}
				case 203: {
					encoding = Encoding.GetEncoding("windows-1253");
					break;
				}
			}
			return encoding;
		}
		DbfRecord LoadRecord(Encoding encoding) {
			if(reader.PeekChar() == 26)
				return null;
			try {
				DbfRecord record = CreateEmptyDbfRecord();
				byte[] recordContent = reader.ReadBytes(header.RecordLength);
				int offset = 1;
				foreach(DbfField field in this.header.Fields) {
					string data = encoding.GetString(recordContent, offset, (int)field.FieldLengthInBytes);
					string correctFieldValueByType = CorrectFieldValueByType(data, field.FieldType);
					if (!string.IsNullOrEmpty(correctFieldValueByType)) {
						object value = CoreUtils.TryConvertValue(field.GetFieldType(), correctFieldValueByType);
						record.AddField(field.FieldName, value);
					}
					offset += (int)field.FieldLengthInBytes;
				}
				return record;
			}
			catch {
				throw new Exception("Incorrect Dbf File Format");
			}
		}
		String CorrectFieldValueByType(string initialValue, byte fieldType) {
			string value;
			switch ((char)fieldType) {
				case 'B':
				case 'F':
				case 'N':
				case 'Y': {
					value = initialValue.TrimStart(new char[] { ' ' });
					break;
				}
				case 'D': {
					if (string.IsNullOrEmpty(initialValue.TrimEnd(new char[] { ' ' }))) 
						value = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
					else {
						value = string.Concat(new string[]
									{
										initialValue.Substring(0, 4), 
										"-", 
										initialValue.Substring(4, 2), 
										"-", 
										initialValue.Substring(6, 2)
									});
					}
					break;
				}
				case 'L': {
					if (initialValue.ToUpper() == "T") {
						value = bool.TrueString;
					}
					else {
						value = bool.FalseString;
					}
					break;
				}
				case 'T': {
					value = string.Concat(new string[]
									{
										initialValue.Substring(0, 4), 
										"-", 
										initialValue.Substring(4, 2), 
										"-", 
										initialValue.Substring(6, 2), 
										" ", 
										initialValue.Substring(8, 2), 
										":", 
										initialValue.Substring(10, 2), 
										":", 
										initialValue.Substring(12, 2)
									});
					break;
				}
				default: {
					value = initialValue.TrimEnd(new char[] { ' ' });
					break;
				}
			}
			return value;
		}
		void IDisposable.Dispose() {
			if (reader != null)
				reader.Dispose();
		}
	}
}
