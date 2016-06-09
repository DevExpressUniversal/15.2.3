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
using System.Text;
using DevExpress.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xls {
	#region IDocumentPropertiesContainer
	public interface IDocumentPropertiesContainer {
		void SetTitle(string value);
		void SetSubject(string value);
		void SetAuthor(string value);
		void SetKeywords(string value);
		void SetDescription(string value);
		void SetLastModifiedBy(string value);
		void SetCategory(string value);
		void SetCreated(DateTime value);
		void SetModified(DateTime value);
		void SetLastPrinted(DateTime value);
		void SetApplication(string value);
		void SetManager(string value);
		void SetCompany(string value);
		void SetVersion(string value);
		void SetSecurity(int value);
		void SetText(string propName, string propValue);
		void SetNumeric(string propName, double propValue);
		void SetBoolean(string propName, bool propValue);
		void SetDateTime(string propName, DateTime propValue);
	}
	#endregion
	#region OlePropDefs
	public static class OlePropDefs {
		public const int ByteOrderValue = 0xfffe;
		public const int CodePageUnicode = 0x04b0;
		public static readonly Guid FMTID_SummaryInformation = new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9");
		public static readonly Guid FMTID_DocSummaryInformation = new Guid("D5CDD502-2E9C-101B-9397-08002B2CF9AE");
		public static readonly Guid FMTID_UserDefinedProperties = new Guid("D5CDD505-2E9C-101B-9397-08002B2CF9AE");
		public const int PID_DICTIONARY = 0x00000000;
		public const int PID_CODEPAGE = 0x00000001;
		public const int PID_LOCALE = -2147483648; 
		public const int PID_BEHAVIOR = -2147483645; 
		public const int PID_NORMAL_MIN = 0x00000002;
		public const int PID_NORMAL_MAX = 0x7fffffff;
		public const int PIDSI_TITLE = 0x0002;
		public const int PIDSI_SUBJECT = 0x0003;
		public const int PIDSI_AUTHOR = 0x0004;
		public const int PIDSI_KEYWORDS = 0x0005;
		public const int PIDSI_COMMENTS = 0x0006;
		public const int PIDSI_TEMPLATE = 0x007;
		public const int PIDSI_LASTAUTHOR = 0x0008;
		public const int PIDSI_REVNUMBER = 0x0009;
		public const int PIDSI_EDITTIME = 0x000a;
		public const int PIDSI_LASTPRINTED = 0x000b;
		public const int PIDSI_CREATE_DTM = 0x000c;
		public const int PIDSI_LASTSAVE_DTM = 0x000d;
		public const int PIDSI_PAGECOUNT = 0x000e;
		public const int PIDSI_WORDCOUNT = 0x000f;
		public const int PIDSI_CHARCOUNT = 0x0010;
		public const int PIDSI_THUMBNAIL = 0x0011;
		public const int PIDSI_APPNAME = 0x0012;
		public const int PIDSI_DOC_SECURITY = 0x0013;
		public const int PIDDSI_CATEGORY = 0x0002;
		public const int PIDDSI_MANAGER = 0x000e;
		public const int PIDDSI_COMPANY = 0x000f;
		public const int PIDDSI_VERSION = 0x0017;
		public const int VT_EMPTY = 0x0000;
		public const int VT_NULL = 0x0001;
		public const int VT_I2 = 0x0002;
		public const int VT_I4 = 0x0003;
		public const int VT_R4 = 0x0004;
		public const int VT_R8 = 0x0005;
		public const int VT_CY = 0x0006;
		public const int VT_DATE = 0x0007;
		public const int VT_BSTR = 0x0008;
		public const int VT_ERROR = 0x000a;
		public const int VT_BOOL = 0x000b;
		public const int VT_VARIANT = 0x000c;
		public const int VT_DECIMAL = 0x000e;
		public const int VT_I1 = 0x0010;
		public const int VT_UI1 = 0x0011;
		public const int VT_UI2 = 0x0012;
		public const int VT_UI4 = 0x0013;
		public const int VT_I8 = 0x0014;
		public const int VT_UI8 = 0x0015;
		public const int VT_INT = 0x0016;
		public const int VT_UINT = 0x0017;
		public const int VT_LPSTR = 0x001e;
		public const int VT_LPWSTR = 0x001f;
		public const int VT_FILETIME = 0x0040;
		public const int VT_BLOB = 0x0041;
		public const int VT_STREAM = 0x0042;
		public const int VT_STORAGE = 0x0043;
		public const int VT_STREAMED_OBJECT = 0x0044;
		public const int VT_STORED_OBJECT = 0x0045;
		public const int VT_BLOB_OBJECT = 0x0046;
		public const int VT_CF = 0x0047;
		public const int VT_CLSID = 0x0048;
		public const int VT_VERSIONED_STREAM = 0x0049;
		public const int VT_VECTOR = 0x1000;
		public const int VT_ARRAY = 0x2000;
	}
	#endregion
	#region OlePropertyBase
	public abstract class OlePropertyBase {
		#region Properties
		public int PropertyId { get; private set; }
		public int PropertyType { get; private set; }
		#endregion
		protected OlePropertyBase(int propertyId, int propertyType) {
			PropertyId = propertyId;
			PropertyType = propertyType;
		}
		public abstract void Read(BinaryReader reader, OlePropertySetBase propertySet);
		public abstract void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet);
		public abstract void Write(BinaryWriter writer, OlePropertySetBase propertySet);
		public abstract int GetSize(OlePropertySetBase propertySet);
		protected void WritePadding(BinaryWriter writer, int paddingCount) {
			for(int i = 0; i < paddingCount; i++)
				writer.Write((byte)0);
		}
		protected bool IsSingleByteEncoding(Encoding encoding) {
			int codePage = DXEncoding.GetEncodingCodePage(encoding);
			return codePage != OlePropDefs.CodePageUnicode;
		}
	}
	#endregion
	#region Typed properties
	#region OlePropertyIntBase
	public abstract class OlePropertyIntBase : OlePropertyBase {
		public int Value { get; set; }
		protected OlePropertyIntBase(int propertyId, int propertyType)
			: base(propertyId, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			string propertyName = propertySet.GetPropertyName(PropertyId);
			if(!string.IsNullOrEmpty(propertyName))
				propertiesContainer.SetNumeric(propertyName, Value);
		}
		public override int GetSize(OlePropertySetBase propertySet) {
			return 8;
		}
	}
	#endregion
	#region OlePropertyUIntBase
	[CLSCompliant(false)]
	public abstract class OlePropertyUIntBase : OlePropertyBase {
		[CLSCompliant(false)]
		public uint Value { get; set; }
		protected OlePropertyUIntBase(int propertyId, int propertyType)
			: base(propertyId, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			string propertyName = propertySet.GetPropertyName(PropertyId);
			if(!string.IsNullOrEmpty(propertyName))
				propertiesContainer.SetNumeric(propertyName, Value);
		}
		public override int GetSize(OlePropertySetBase propertySet) {
			return 8;
		}
	}
	#endregion
	#region OlePropertyNumericBase
	public abstract class OlePropertyNumericBase : OlePropertyBase {
		public double Value { get; set; }
		protected OlePropertyNumericBase(int propertyId, int propertyType)
			: base(propertyId, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			string propertyName = propertySet.GetPropertyName(PropertyId);
			if(!string.IsNullOrEmpty(propertyName))
				propertiesContainer.SetNumeric(propertyName, Value);
		}
	}
	#endregion
	#region OlePropertyInt16
	public class OlePropertyInt16 : OlePropertyIntBase {
		public OlePropertyInt16(int propertyId)
			: base(propertyId, OlePropDefs.VT_I2) {
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			Value = reader.ReadInt16();
			reader.ReadInt16();
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			writer.Write(PropertyType);
			writer.Write((short)Value);
			writer.Write((short)0);
		}
	}
	#endregion
	#region OlePropertyInt32
	public class OlePropertyInt32 : OlePropertyIntBase {
		public OlePropertyInt32(int propertyId)
			: base(propertyId, OlePropDefs.VT_I4) {
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			Value = reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			writer.Write(PropertyType);
			writer.Write(Value);
		}
	}
	#endregion
	#region OlePropertyUInt16
	[CLSCompliant(false)]
	public class OlePropertyUInt16 : OlePropertyUIntBase {
		public OlePropertyUInt16(int propertyId)
			: base(propertyId, OlePropDefs.VT_UI2) {
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			Value = reader.ReadUInt16();
			reader.ReadInt16();
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			writer.Write(PropertyType);
			writer.Write((ushort)Value);
			writer.Write((short)0);
		}
	}
	#endregion
	#region OlePropertyUInt32
	[CLSCompliant(false)]
	public class OlePropertyUInt32 : OlePropertyUIntBase {
		public OlePropertyUInt32(int propertyId)
			: base(propertyId, OlePropDefs.VT_UI4) {
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			Value = reader.ReadUInt32();
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			writer.Write(PropertyType);
			writer.Write(Value);
		}
	}
	#endregion
	#region OlePropertyFloat
	public class OlePropertyFloat : OlePropertyNumericBase {
		public OlePropertyFloat(int propertyId)
			: base(propertyId, OlePropDefs.VT_R4) {
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			Value = reader.ReadSingle();
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			writer.Write(PropertyType);
			writer.Write((float)Value);
		}
		public override int GetSize(OlePropertySetBase propertySet) {
			return 8;
		}
	}
	#endregion
	#region OlePropertyDouble
	public class OlePropertyDouble : OlePropertyNumericBase {
		public OlePropertyDouble(int propertyId)
			: base(propertyId, OlePropDefs.VT_R8) {
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			Value = reader.ReadDouble();
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			writer.Write(PropertyType);
			writer.Write(Value);
		}
		public override int GetSize(OlePropertySetBase propertySet) {
			return 12;
		}
	}
	#endregion
	#region OlePropertyByte
	public class OlePropertyByte : OlePropertyBase {
		public byte Value { get; set; }
		public OlePropertyByte(int propertyId)
			: base(propertyId, OlePropDefs.VT_UI1) {
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			Value = reader.ReadByte();
			reader.ReadBytes(3);
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			string propertyName = propertySet.GetPropertyName(PropertyId);
			if(!string.IsNullOrEmpty(propertyName))
				propertiesContainer.SetNumeric(propertyName, Value);
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			writer.Write(PropertyType);
			writer.Write((uint)Value);
		}
		public override int GetSize(OlePropertySetBase propertySet) {
			return 8;
		}
	}
	#endregion
	#region OlePropertyString
	public class OlePropertyString : OlePropertyBase {
		string value = string.Empty;
		public string Value {
			get { return value; }
			set {
				if(string.IsNullOrEmpty(value))
					this.value = string.Empty;
				else
					this.value = value;
			}
		}
		public OlePropertyString(int propertyId, int propertyType)
			: base(propertyId, propertyType) {
			if(propertyType != OlePropDefs.VT_LPSTR && propertyType != OlePropDefs.VT_LPWSTR)
				throw new ArgumentException(string.Format("Such property type is not allowed: {0}", propertyType));
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			int cch = reader.ReadInt32();
			if(cch < 0 || cch > 0x0ffff)
				throw new Exception("Invalid property set stream: string char count out of range 0...0xffff");
			Encoding encoding = PropertyType == OlePropDefs.VT_LPWSTR ? DXEncoding.GetEncoding(OlePropDefs.CodePageUnicode) : propertySet.GetEncoding();
			bool isSingleByte = IsSingleByteEncoding(encoding);
			byte[] valueBytes = reader.ReadBytes(isSingleByte ? cch : cch * 2);
			string valueString = encoding.GetString(valueBytes, 0, valueBytes.Length);
			Value = valueString.TrimEnd(new char[] { '\0' });
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			string propertyName = propertySet.GetPropertyName(PropertyId);
			if(!string.IsNullOrEmpty(propertyName))
				propertiesContainer.SetText(propertyName, Value);
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			writer.Write(PropertyType);
			string valueString = Value;
			int cch = valueString.Length;
			if(cch == 0 || (cch > 0 && Value[cch - 1] != '\0')) {
				valueString += "\0";
				cch++;
			}
			writer.Write(cch);
			Encoding encoding = PropertyType == OlePropDefs.VT_LPWSTR ? DXEncoding.GetEncoding(OlePropDefs.CodePageUnicode) : propertySet.GetEncoding();
			bool isSingleByte = IsSingleByteEncoding(encoding);
			byte[] valueBytes = encoding.GetBytes(valueString);
			writer.Write(valueBytes);
			int paddingCount = 4 - (isSingleByte ? cch : cch * 2 + 8) % 4;
			if(paddingCount < 4)
				WritePadding(writer, paddingCount);
		}
		public override int GetSize(OlePropertySetBase propertySet) {
			int cch = Value.Length;
			if(cch == 0 || (cch > 0 && Value[cch - 1] != '\0'))
				cch++;
			Encoding encoding = PropertyType == OlePropDefs.VT_LPWSTR ? DXEncoding.GetEncoding(OlePropDefs.CodePageUnicode) : propertySet.GetEncoding();
			bool isSingleByte = IsSingleByteEncoding(encoding);
			return ((8 + (isSingleByte ? cch : cch * 2) + 3) / 4) * 4;
		}
	}
	#endregion
	#region OlePropertyFileTime
	public class OlePropertyFileTime : OlePropertyBase {
		long fileTime;
		#region Properties
		public DateTime Value {
			get {
				if(fileTime < 0)
					return DateTime.MinValue;
				try {
					return DateTime.FromFileTime(fileTime);
				}
				catch(ArgumentOutOfRangeException) {
					return DateTime.MaxValue;
				}
			}
			set {
				try {
					fileTime = value.ToFileTime();
				}
				catch(ArgumentOutOfRangeException) {
					fileTime = 0;
				}
			}
		}
		public bool IsValidDateTime {
			get {
				try {
					DateTime.FromFileTime(fileTime);
				}
				catch(ArgumentOutOfRangeException) {
					return false;
				}
				return true;
			}
		}
		#endregion
		public OlePropertyFileTime(int propertyId)
			: base(propertyId, OlePropDefs.VT_FILETIME) {
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			fileTime = reader.ReadInt64();
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			string propertyName = propertySet.GetPropertyName(PropertyId);
			if(!string.IsNullOrEmpty(propertyName) && IsValidDateTime)
				propertiesContainer.SetDateTime(propertyName, Value);
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			writer.Write(PropertyType);
			writer.Write(fileTime);
		}
		public override int GetSize(OlePropertySetBase propertySet) {
			return 12;
		}
	}
	#endregion
	#region OlePropertyBool
	public class OlePropertyBool : OlePropertyBase {
		public bool Value { get; set; }
		public OlePropertyBool(int propertyId)
			: base(propertyId, OlePropDefs.VT_BOOL) {
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			Value = (reader.ReadInt32() & 0x0ffff) != 0;
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			string propertyName = propertySet.GetPropertyName(PropertyId);
			if(!string.IsNullOrEmpty(propertyName))
				propertiesContainer.SetBoolean(propertyName, Value);
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			writer.Write(PropertyType);
			writer.Write((int)(Value ? 0x0ffff : 0x0000));
		}
		public override int GetSize(OlePropertySetBase propertySet) {
			return 8;
		}
	}
	#endregion
	#endregion
	#region Special properties
	#region OlePropertyDictionary
	public class OlePropertyDictionary : OlePropertyBase {
		#region Fields
		readonly Dictionary<int, string> entries = new Dictionary<int, string>();
		#endregion
		#region Properties
		public Dictionary<int, string> Entries { get { return entries; } }
		#endregion
		public OlePropertyDictionary()
			: base(OlePropDefs.PID_DICTIONARY, 0) {
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			Encoding encoding = propertySet.GetEncoding();
			bool isSingleByte = IsSingleByteEncoding(encoding);
			int numEntries = reader.ReadInt32();
			for(int i = 0; i < numEntries; i++) {
				int propId = reader.ReadInt32();
				int length = reader.ReadInt32();
				if(!isSingleByte)
					length = ((length * 2 + 3) / 4) * 4;
				byte[] nameBytes = reader.ReadBytes(length);
				string nameString = encoding.GetString(nameBytes, 0, nameBytes.Length);
				Entries.Add(propId, nameString.TrimEnd(new char[] { '\0' }));
			}
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			Encoding encoding = propertySet.GetEncoding();
			bool isSingleByte = IsSingleByteEncoding(encoding);
			writer.Write(Entries.Count);
			int paddingCount;
			int bytesWritten = 4;
			foreach(KeyValuePair<int, string> entry in Entries) {
				int propId = entry.Key;
				string name = entry.Value;
				int length = name.Length;
				if(length > 0 && name[length - 1] != '\0') {
					name += '\0';
					length++;
				}
				writer.Write(propId);
				writer.Write(length);
				byte[] nameBytes = encoding.GetBytes(name);
				writer.Write(nameBytes);
				bytesWritten += nameBytes.Length + 8;
				if(!isSingleByte) {
					paddingCount = 4 - (length * 2 + 8) % 4;
					if(paddingCount < 4) {
						WritePadding(writer, paddingCount);
						bytesWritten += paddingCount;
					}
				}
			}
			paddingCount = 4 - bytesWritten % 4;
			if(paddingCount < 4)
				WritePadding(writer, paddingCount);
		}
		public override int GetSize(OlePropertySetBase propertySet) {
			Encoding encoding = propertySet.GetEncoding();
			bool isSingleByte = IsSingleByteEncoding(encoding);
			int result = 4;
			foreach(string name in Entries.Values) {
				int length = name.Length;
				if(length > 0 && name[length - 1] != '\0')
					length++;
				if(!isSingleByte)
					length = ((length * 2 + 3) / 4) * 4;
				result += length + 8;
			}
			result = ((result + 3) / 4) * 4;
			return result;
		}
	}
	#endregion
	#region OlePropertyCodePage
	public class OlePropertyCodePage : OlePropertyInt16 {
		public OlePropertyCodePage()
			: base(OlePropDefs.PID_CODEPAGE) {
		}
		public override void Read(BinaryReader reader, OlePropertySetBase propertySet) {
			Value = reader.ReadUInt16();
			reader.ReadInt16();
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
		}
		public override void Write(BinaryWriter writer, OlePropertySetBase propertySet) {
			writer.Write(PropertyType);
			writer.Write((ushort)Value);
			writer.Write((short)0);
		}
	}
	#endregion
	#region OlePropertyLocale
	[CLSCompliant(false)]
	public class OlePropertyLocale : OlePropertyUInt32 {
		public OlePropertyLocale()
			: base(OlePropDefs.PID_LOCALE) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
		}
	}
	#endregion
	#endregion
	#region Well-known properties
	#region OlePropertyTitle
	public class OlePropertyTitle : OlePropertyString {
		public OlePropertyTitle(int propertyType)
			: base(OlePropDefs.PIDSI_TITLE, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetTitle(Value);
		}
	}
	#endregion
	#region OlePropertySubject
	public class OlePropertySubject : OlePropertyString {
		public OlePropertySubject(int propertyType)
			: base(OlePropDefs.PIDSI_SUBJECT, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetSubject(Value);
		}
	}
	#endregion
	#region OlePropertyAuthor
	public class OlePropertyAuthor : OlePropertyString {
		public OlePropertyAuthor(int propertyType)
			: base(OlePropDefs.PIDSI_AUTHOR, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetAuthor(Value);
		}
	}
	#endregion
	#region OlePropertyKeywords
	public class OlePropertyKeywords : OlePropertyString {
		public OlePropertyKeywords(int propertyType)
			: base(OlePropDefs.PIDSI_KEYWORDS, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetKeywords(Value);
		}
	}
	#endregion
	#region OlePropertyComments
	public class OlePropertyComments : OlePropertyString {
		public OlePropertyComments(int propertyType)
			: base(OlePropDefs.PIDSI_COMMENTS, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetDescription(Value);
		}
	}
	#endregion
	#region OlePropertyLastAuthor
	public class OlePropertyLastAuthor : OlePropertyString {
		public OlePropertyLastAuthor(int propertyType)
			: base(OlePropDefs.PIDSI_LASTAUTHOR, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetLastModifiedBy(Value);
		}
	}
	#endregion
	#region OlePropertyCreated
	public class OlePropertyCreated : OlePropertyFileTime {
		public OlePropertyCreated()
			: base(OlePropDefs.PIDSI_CREATE_DTM) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetCreated(Value);
		}
	}
	#endregion
	#region OlePropertyModified
	public class OlePropertyModified : OlePropertyFileTime {
		public OlePropertyModified()
			: base(OlePropDefs.PIDSI_LASTSAVE_DTM) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetModified(Value);
		}
	}
	#endregion
	#region OlePropertyLastPrinted
	public class OlePropertyLastPrinted : OlePropertyFileTime {
		public OlePropertyLastPrinted()
			: base(OlePropDefs.PIDSI_LASTPRINTED) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetLastPrinted(Value);
		}
	}
	#endregion
	#region OlePropertyApplication
	public class OlePropertyApplication : OlePropertyString {
		public OlePropertyApplication(int propertyType)
			: base(OlePropDefs.PIDSI_APPNAME, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetApplication(Value);
		}
	}
	#endregion
	#region OlePropertyCategory
	public class OlePropertyCategory : OlePropertyString {
		public OlePropertyCategory(int propertyType)
			: base(OlePropDefs.PIDDSI_CATEGORY, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetCategory(Value);
		}
	}
	#endregion
	#region OlePropertyManager
	public class OlePropertyManager : OlePropertyString {
		public OlePropertyManager(int propertyType)
			: base(OlePropDefs.PIDDSI_MANAGER, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetManager(Value);
		}
	}
	#endregion
	#region OlePropertyCompany
	public class OlePropertyCompany : OlePropertyString {
		public OlePropertyCompany(int propertyType)
			: base(OlePropDefs.PIDDSI_COMPANY, propertyType) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetCompany(Value);
		}
	}
	#endregion
	#region OlePropertyVersion
	public class OlePropertyVersion : OlePropertyInt32 {
		public OlePropertyVersion()
			: base(OlePropDefs.PIDDSI_VERSION) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			ushort minor = (ushort)((uint)Value & 0x0ffff);
			ushort major = (ushort)(((uint)Value & 0xffff0000) >> 16);
			propertiesContainer.SetVersion(string.Format("{0}.{1:D4}", major, minor));
		}
	}
	#endregion
	#region OlePropertyDocSecurity
	public class OlePropertyDocSecurity : OlePropertyInt32 {
		public OlePropertyDocSecurity()
			: base(OlePropDefs.PIDSI_DOC_SECURITY) {
		}
		public override void Execute(IDocumentPropertiesContainer propertiesContainer, OlePropertySetBase propertySet) {
			propertiesContainer.SetSecurity(Value);
		}
	}
	#endregion
	#endregion
	#region Property sets
	#region OlePropertySetBase
	public abstract class OlePropertySetBase {
		#region Diagnostics
		#endregion
		#region OlePropIdAndOffset
		class OlePropIdAndOffset {
			public int PropertyId { get; set; }
			public int Offset { get; set; }
		}
		#endregion
		#region Fields
		List<OlePropertyBase> properties = new List<OlePropertyBase>();
		#endregion
		#region Properties
		public abstract Guid FormatId { get; }
		public List<OlePropertyBase> Properties { get { return properties; } }
		#endregion
		public void Read(BinaryReader reader) {
			int propertySetOffset = (int)reader.BaseStream.Position;
			reader.ReadInt32(); 
			int numProperties = reader.ReadInt32();
			List<OlePropIdAndOffset> propertyTable = new List<OlePropIdAndOffset>(numProperties);
			for(int i = 0; i < numProperties; i++) {
				OlePropIdAndOffset item = new OlePropIdAndOffset();
				item.PropertyId = reader.ReadInt32();
				item.Offset = reader.ReadInt32();
				if(item.PropertyId == OlePropDefs.PID_CODEPAGE)
					propertyTable.Insert(0, item);
				else
					propertyTable.Add(item);
			}
			for(int i = 0; i < numProperties; i++) {
				OlePropertyBase prop = ReadProperty(reader, propertyTable[i].PropertyId, propertyTable[i].Offset + propertySetOffset);
				if(prop != null)
					Properties.Add(prop);
			}
		}
		public void Execute(IDocumentPropertiesContainer propertiesContainer) {
			foreach(OlePropertyBase prop in Properties)
				prop.Execute(propertiesContainer, this);
		}
		public void Write(BinaryWriter writer) {
			int size = GetSize();
			writer.Write(size);
			int numProperties = Properties.Count;
			writer.Write(numProperties);
			int offset = 8 + numProperties * 8;
			foreach(OlePropertyBase prop in Properties) {
				writer.Write(prop.PropertyId);
				writer.Write(offset);
				offset += prop.GetSize(this);
			}
			foreach(OlePropertyBase prop in Properties)
				prop.Write(writer, this);
		}
		public int GetSize() {
			int result = 8 + Properties.Count * 8;
			foreach(OlePropertyBase property in Properties)
				result += property.GetSize(this);
			return result;
		}
		protected abstract OlePropertyBase CreateProperty(BinaryReader reader, int propertyId, int offset);
		protected internal OlePropertyBase ReadProperty(BinaryReader reader, int propertyId, int offset) {
			OlePropertyBase prop = CreateProperty(reader, propertyId, offset);
			if(prop != null)
				prop.Read(reader, this);
			return prop;
		}
		public virtual string GetPropertyName(int propertyId) {
			return string.Empty;
		}
		public Encoding GetEncoding() {
			OlePropertyCodePage codePageProperty = FindById(OlePropDefs.PID_CODEPAGE) as OlePropertyCodePage;
			return DXEncoding.GetEncoding(codePageProperty != null ? codePageProperty.Value : 1252);
		}
		protected OlePropertyBase FindById(int propertyId) {
			foreach(OlePropertyBase prop in Properties)
				if(prop.PropertyId == propertyId)
					return prop;
			return null;
		}
	}
	#endregion
	#region OlePropertySetSummary
	public class OlePropertySetSummary : OlePropertySetBase {
		public override Guid FormatId { get { return OlePropDefs.FMTID_SummaryInformation; } }
		protected override OlePropertyBase CreateProperty(BinaryReader reader, int propertyId, int offset) {
			reader.BaseStream.Position = offset;
			int propertyType = reader.ReadInt32() & 0x0ffff;
			switch(propertyId) {
				case OlePropDefs.PID_CODEPAGE:
					return new OlePropertyCodePage();
				case OlePropDefs.PIDSI_TITLE:
					return new OlePropertyTitle(propertyType);
				case OlePropDefs.PIDSI_SUBJECT:
					return new OlePropertySubject(propertyType);
				case OlePropDefs.PIDSI_AUTHOR:
					return new OlePropertyAuthor(propertyType);
				case OlePropDefs.PIDSI_KEYWORDS:
					return new OlePropertyKeywords(propertyType);
				case OlePropDefs.PIDSI_COMMENTS:
					return new OlePropertyComments(propertyType);
				case OlePropDefs.PIDSI_LASTAUTHOR:
					return new OlePropertyLastAuthor(propertyType);
				case OlePropDefs.PIDSI_CREATE_DTM:
					return new OlePropertyCreated();
				case OlePropDefs.PIDSI_LASTSAVE_DTM:
					return new OlePropertyModified();
				case OlePropDefs.PIDSI_LASTPRINTED:
					return new OlePropertyLastPrinted();
				case OlePropDefs.PIDSI_APPNAME:
					return new OlePropertyApplication(propertyType);
				case OlePropDefs.PIDSI_DOC_SECURITY:
					return new OlePropertyDocSecurity();
			}
			return null;
		}
	}
	#endregion
	#region OlePropertySetDocSummary
	public class OlePropertySetDocSummary : OlePropertySetBase {
		public override Guid FormatId { get { return OlePropDefs.FMTID_DocSummaryInformation; } }
		protected override OlePropertyBase CreateProperty(BinaryReader reader, int propertyId, int offset) {
			reader.BaseStream.Position = offset;
			int propertyType = reader.ReadInt32() & 0x0ffff;
			switch(propertyId) {
				case OlePropDefs.PID_CODEPAGE:
					return new OlePropertyCodePage();
				case OlePropDefs.PIDDSI_CATEGORY:
					return new OlePropertyCategory(propertyType);
				case OlePropDefs.PIDDSI_MANAGER:
					return new OlePropertyManager(propertyType);
				case OlePropDefs.PIDDSI_COMPANY:
					return new OlePropertyCompany(propertyType);
				case OlePropDefs.PIDDSI_VERSION:
					return new OlePropertyVersion();
			}
			return null;
		}
	}
	#endregion
	#region OlePropertySetUserDefined
	public class OlePropertySetUserDefined : OlePropertySetBase {
		public override Guid FormatId { get { return OlePropDefs.FMTID_UserDefinedProperties; } }
		protected override OlePropertyBase CreateProperty(BinaryReader reader, int propertyId, int offset) {
			reader.BaseStream.Position = offset;
			if(propertyId >= OlePropDefs.PID_NORMAL_MIN && propertyId <= OlePropDefs.PID_NORMAL_MAX) {
				int propertyType = reader.ReadInt32() & 0x0ffff;
				switch(propertyType) {
					case OlePropDefs.VT_I2:
						return new OlePropertyInt16(propertyId);
					case OlePropDefs.VT_I4:
						return new OlePropertyInt32(propertyId);
					case OlePropDefs.VT_UI2:
						return new OlePropertyUInt16(propertyId);
					case OlePropDefs.VT_UI4:
						return new OlePropertyUInt32(propertyId);
					case OlePropDefs.VT_R4:
						return new OlePropertyFloat(propertyId);
					case OlePropDefs.VT_R8:
						return new OlePropertyDouble(propertyId);
					case OlePropDefs.VT_LPSTR:
					case OlePropDefs.VT_LPWSTR:
						return new OlePropertyString(propertyId, propertyType);
					case OlePropDefs.VT_FILETIME:
						return new OlePropertyFileTime(propertyId);
					case OlePropDefs.VT_BOOL:
						return new OlePropertyBool(propertyId);
					case OlePropDefs.VT_UI1:
						return new OlePropertyByte(propertyId);
				}
			}
			else if(propertyId == OlePropDefs.PID_DICTIONARY)
				return new OlePropertyDictionary();
			else if(propertyId == OlePropDefs.PID_CODEPAGE) {
				reader.ReadInt32(); 
				return new OlePropertyCodePage();
			}
			else if(propertyId == OlePropDefs.PID_LOCALE) {
				reader.ReadInt32(); 
				return new OlePropertyLocale();
			}
			return null;
		}
		public override string GetPropertyName(int propertyId) {
			OlePropertyDictionary dictionary = FindById(OlePropDefs.PID_DICTIONARY) as OlePropertyDictionary;
			if(dictionary != null && dictionary.Entries.ContainsKey(propertyId))
				return dictionary.Entries[propertyId];
			return string.Empty;
		}
	}
	#endregion
	#endregion
	#region OlePropertyStreamContent
	public class OlePropertyStreamContent {
		#region Fields
		int version = 0;
		int systemId = 0x00020106;
		Guid classId = Guid.Empty;
		List<OlePropertySetBase> propertySets = new List<OlePropertySetBase>();
		#endregion
		#region Properties
		public int Version {
			get { return version; }
			set {
				if(value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("Version out of range 0...1");
				version = value;
			}
		}
		public int SystemId {
			get { return systemId; }
			set { systemId = value; }
		}
		public Guid ClassId {
			get { return classId; }
			set { classId = value; }
		}
		public List<OlePropertySetBase> PropertySets { get { return propertySets; } }
		#endregion
		public static OlePropertyStreamContent FromStream(BinaryReader reader) {
			OlePropertyStreamContent result = new OlePropertyStreamContent();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			if(reader.BaseStream.Length < 28)
				return; 
			int byteOrder = reader.ReadUInt16();
			if(byteOrder != OlePropDefs.ByteOrderValue)
				return; 
			Version = reader.ReadUInt16();
			SystemId = reader.ReadInt32();
			byte[] guidBytes = reader.ReadBytes(16);
			ClassId = new Guid(guidBytes);
			int numPropertySets = reader.ReadInt32();
			if(numPropertySets < 1 || numPropertySets > 2)
				return; 
			Guid fmtId0 = Guid.Empty;
			Guid fmtId1 = Guid.Empty;
			int offset0 = 0;
			int offset1 = 0;
			guidBytes = reader.ReadBytes(16);
			fmtId0 = new Guid(guidBytes);
			offset0 = reader.ReadInt32();
			if(numPropertySets == 2) {
				guidBytes = reader.ReadBytes(16);
				fmtId1 = new Guid(guidBytes);
				offset1 = reader.ReadInt32();
			}
			if(fmtId0 == OlePropDefs.FMTID_SummaryInformation)
				ReadPropertySet(reader, offset0, new OlePropertySetSummary());
			else if(fmtId0 == OlePropDefs.FMTID_DocSummaryInformation) {
				ReadPropertySet(reader, offset0, new OlePropertySetDocSummary());
				if(fmtId1 == OlePropDefs.FMTID_UserDefinedProperties)
					ReadPropertySet(reader, offset1, new OlePropertySetUserDefined());
			}
		}
		public void Execute(IDocumentPropertiesContainer propertiesContainer) {
			foreach(OlePropertySetBase propertySet in propertySets)
				propertySet.Execute(propertiesContainer);
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)OlePropDefs.ByteOrderValue);
			writer.Write((ushort)Version);
			writer.Write(SystemId);
			writer.Write(ClassId.ToByteArray());
			int numPropertySets = PropertySets.Count;
			writer.Write(numPropertySets);
			int offset = (int)writer.BaseStream.Position + numPropertySets * 20;
			foreach(OlePropertySetBase propertySet in PropertySets) {
				writer.Write(propertySet.FormatId.ToByteArray());
				writer.Write(offset);
				offset += propertySet.GetSize();
			}
			foreach(OlePropertySetBase propertySet in PropertySets)
				propertySet.Write(writer);
			int paddingCount = (int)(4 - writer.BaseStream.Length % 4);
			if(paddingCount < 4)
				for(int i = 0; i < paddingCount; i++)
					writer.Write((byte)0);
		}
		void ReadPropertySet(BinaryReader reader, int offset, OlePropertySetBase propertySet) {
			reader.BaseStream.Position = offset;
			propertySet.Read(reader);
			PropertySets.Add(propertySet);
		}
	}
	#endregion
}
