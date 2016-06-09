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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
namespace DevExpress.Map.Native {
	public class DbfExporter : CommonExporterBase {
		const byte DefaultLanguageDriver = 0x57;
		const int MaxFieldNameLength = 11;
		readonly IList<IMapItemCore> items;
		readonly Dictionary<string, DbfField> fieldsHash;
		protected internal Dictionary<string, DbfField> FieldsHash { get { return fieldsHash; } }
		public DbfExporter(IList<IMapItemCore> items) {
			this.items = items;
			this.fieldsHash = new Dictionary<string, DbfField>();
			PrepareAttributes();
		}
		void PrepareAttributes() {
			foreach(IMapItemCore item in items) {
				for(int i = 0; i < item.AttributesCount; i++) {
					IMapItemAttribute attribute = item.GetAttribute(i);
					string key = attribute.Name.Length > MaxFieldNameLength ? attribute.Name.Substring(0, MaxFieldNameLength) : attribute.Name;
					DbfField field = new DbfField() { FieldName = attribute.Name, FieldType = Convert.ToByte(DbfField.GetFieldTypeChar(attribute.Type)) };
					byte size = field.GetSize(attribute.Value);
					field.FieldLengthInBytes = size;
					field.NumberOfDecimalPlaces = 0;
					if(!fieldsHash.ContainsKey(key))
						fieldsHash.Add(field.FieldName, field);
					else {
						field = fieldsHash[field.FieldName];
						field.FieldLengthInBytes = Math.Max(field.FieldLengthInBytes, size);
						field.NumberOfDecimalPlaces = Math.Max(field.NumberOfDecimalPlaces, CalculateDecimalPlacesNumber(attribute.Value));
					}
				}
			}
		}
		byte CalculateDecimalPlacesNumber(object str) {
			double value;
			if(!double.TryParse(str.ToString(), out value))
				return 0;
			string doubleAsString = value.ToString(CultureInfo.InvariantCulture);
			string[] parts = doubleAsString.Split('.');
			int result = parts.Length > 1 ? parts[1].Length : 0;
			return (byte)result;
		}
		void WriteUpdateDate() {
			WriteByte((byte)(DateTime.Now.Year - 1900));
			WriteByte((byte)DateTime.Now.Month);
			WriteByte((byte)DateTime.Now.Day);
		}
		ushort CalculateRecordLength() {
			ushort sum = 1;
			foreach(KeyValuePair<string, DbfField> field in fieldsHash)
				sum += field.Value.FieldLengthInBytes;
			return sum;
		}
		void WriteValue(byte lengthInBytes, byte type, object value) {
			string stringToWrite = value == null ? GetEmptyString(lengthInBytes) : value.ToString();
			stringToWrite = type == 'C' ? stringToWrite.PadRight(lengthInBytes) : stringToWrite.PadLeft(lengthInBytes);
			WriteString(stringToWrite);
		}
		string GetEmptyString(byte length) {
			return new string('\0', length);
		}
		protected override void WriteHeader() {
			WriteByte(0x03);
			WriteUpdateDate();
			WriteInt(items.Count, false);
			WriteUInt16((ushort)(33 + 32 * fieldsHash.Count));
			WriteUInt16(CalculateRecordLength());
			WriteUInt16(0);
			WriteByte(0x00);
			WriteByte(0x00);
			WriteInt(0, false);
			for(int i = 0; i < 8; i++)
				WriteByte(0x00);
			WriteByte(0);
			WriteByte(DefaultLanguageDriver);
			WriteUInt16(0);
			foreach(KeyValuePair<string, DbfField> field in fieldsHash) {
				WriteString(field.Key.PadRight(MaxFieldNameLength, '\0'));
				WriteByte(field.Value.FieldType);
				WriteInt(0, false);
				WriteByte(field.Value.FieldLengthInBytes);
				WriteByte(field.Value.NumberOfDecimalPlaces);
				WriteByte(0);
				WriteInt(0, false);
				WriteByte(0);
				for(int i = 0; i < 8; i++)
					WriteByte(0);
			}
			WriteByte(13);
		}
		protected override void WriteRecords() {
			foreach(IMapItemCore item in items) {
				WriteByte(0x20);
				foreach(KeyValuePair<string, DbfField> field in fieldsHash) {
					IMapItemAttribute attr = item.GetAttribute(field.Value.FieldName);
					WriteValue(field.Value.FieldLengthInBytes, field.Value.FieldType, attr != null ? attr.Value : null);
				}
			}
		}
	}
}
