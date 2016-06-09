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
	#region XlsHyperlinkMonikerBase
	public abstract class XlsHyperlinkMonikerBase {
		const int classIdSize = 16;
		Guid classId;
		public Guid ClassId { get { return classId; } }
		protected XlsHyperlinkMonikerBase(Guid classId) {
			Guard.ArgumentNotNull(classId, "classId");
			this.classId = classId;
		}
		public virtual void Write(BinaryWriter writer) {
			writer.Write(classId.ToByteArray());
		}
		public virtual int GetSize() {
			return classIdSize;
		}
	}
	#endregion
	#region XlsHyperLinkURLMoniker
	public class XlsHyperlinkURLMoniker : XlsHyperlinkMonikerBase {
		#region Fields
		const uint maskAllowRelative = 0x00000001;
		const uint maskAllowImplicitWildcardScheme = 0x00000002;
		const uint maskAllowImplicitFileScheme = 0x00000004;
		const uint maskNoFrag = 0x00000008;
		const uint maskNoCanonicalize = 0x00000010;
		const uint maskCanonicalize = 0x00000020;
		const uint maskFileUseDosPath = 0x00000040;
		const uint maskDecodeExtraInfo = 0x00000080;
		const uint maskNoDecodeExtraInfo = 0x00000100;
		const uint maskCrackUnknownSchemes = 0x00000200;
		const uint maskNoCrackUnknownSchemes = 0x00000400;
		const uint maskPreProcessHtmlUri = 0x00000800;
		const uint maskNoPreProcessHtmlUri = 0x00001000;
		const uint maskIESettings = 0x00002000;
		const uint maskNoIESettings = 0x00004000;
		const uint maskNoEncodeForbiddenChars = 0x00008000;
		const uint maskAllFlags = 0x0000ffff;
		static readonly Guid serialGUID = new Guid("{0xF4815879, 0x1D3B, 0x487F, {0xAF, 0x2C, 0x82, 0x5D, 0xC4, 0x85, 0x27, 0x63}}");
		NullTerminatedUnicodeString url = new NullTerminatedUnicodeString();
		uint uriCreateFlags = maskNoCanonicalize | maskNoDecodeExtraInfo | maskNoCrackUnknownSchemes | maskNoPreProcessHtmlUri | maskNoIESettings;
		#endregion
		#region Properties
		public string Url {
			get { return url.Value; }
			set { url.Value = value; }
		}
		public bool HasOptionalData { get; set; }
		public bool AllowRelative { get { return GetFlag(maskAllowRelative); } set { SetFlag(maskAllowRelative, value); } }
		public bool AllowImplicitWildcardScheme { get { return GetFlag(maskAllowImplicitWildcardScheme); } set { SetFlag(maskAllowImplicitWildcardScheme, value); } }
		public bool AllowImplicitFileScheme { get { return GetFlag(maskAllowImplicitFileScheme); } set { SetFlag(maskAllowImplicitFileScheme, value); } }
		public bool NoFrag { get { return GetFlag(maskNoFrag); } set { SetFlag(maskNoFrag, value); } }
		public bool Canonicalize {
			get { return GetFlag(maskCanonicalize); }
			set {
				SetFlag(maskNoCanonicalize, !value);
				SetFlag(maskCanonicalize, value);
			}
		}
		public bool FileUseDosPath { get { return GetFlag(maskFileUseDosPath); } set { SetFlag(maskFileUseDosPath, value); } }
		public bool DecodeExtraInfo {
			get { return GetFlag(maskDecodeExtraInfo); }
			set {
				SetFlag(maskDecodeExtraInfo, value);
				SetFlag(maskNoDecodeExtraInfo, !value);
			}
		}
		public bool CrackUnknownSchemes {
			get { return GetFlag(maskCrackUnknownSchemes); }
			set {
				SetFlag(maskCrackUnknownSchemes, value);
				SetFlag(maskNoCrackUnknownSchemes, !value);
			}
		}
		public bool PreProcessHtmlUri {
			get { return GetFlag(maskPreProcessHtmlUri); }
			set {
				SetFlag(maskPreProcessHtmlUri, value);
				SetFlag(maskNoPreProcessHtmlUri, !value);
			}
		}
		public bool IESettings {
			get { return GetFlag(maskIESettings); }
			set {
				SetFlag(maskIESettings, value);
				SetFlag(maskNoIESettings, !value);
			}
		}
		public bool NoEncodeForbiddenChars { get { return GetFlag(maskNoEncodeForbiddenChars); } set { SetFlag(maskNoEncodeForbiddenChars, value); } }
		#endregion
		public static XlsHyperlinkURLMoniker FromStream(XlsReader reader) {
			XlsHyperlinkURLMoniker result = new XlsHyperlinkURLMoniker();
			result.Read(reader);
			return result;
		}
		public XlsHyperlinkURLMoniker()
			: base(XlsHyperlinkMonikerFactory.CLSID_URLMoniker) {
		}
		protected void Read(XlsReader reader) {
			int length = reader.ReadInt32();
			this.url = NullTerminatedUnicodeString.FromStream(reader);
			if(length > this.url.Length) {
				HasOptionalData = true;
				Guid guid = new Guid(reader.ReadBytes(16));
				if(guid != serialGUID)
					throw new Exception("Invalid XLS file: Unknown serial GUID in hyperlink URL moniker");
				reader.ReadInt32(); 
				this.uriCreateFlags = reader.ReadUInt32() & maskAllFlags;
			}
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			int length = this.url.Length;
			if(HasOptionalData)
				length += 24;
			writer.Write(length);
			this.url.Write(writer);
			if(HasOptionalData) {
				writer.Write(serialGUID.ToByteArray());
				writer.Write((int)0); 
				writer.Write(this.uriCreateFlags);
			}
		}
		public override int GetSize() {
			int length = this.url.Length;
			if(HasOptionalData)
				length += 24;
			return base.GetSize() + 4 + length;
		}
		bool GetFlag(uint mask) {
			return (this.uriCreateFlags & mask) != 0;
		}
		void SetFlag(uint mask, bool value) {
			if(value)
				this.uriCreateFlags |= mask;
			else
				this.uriCreateFlags &= ~mask;
		}
	}
	#endregion
	#region XlsHyperlinkFileMoniker
	public class XlsHyperlinkFileMoniker : XlsHyperlinkMonikerBase {
		const ushort serializationVersionNumber = 0xdead;
		string path = string.Empty;
		public string Path {
			get { return path; }
			set {
				if(value == null)
					value = string.Empty;
				path = value;
			}
		}
		public static XlsHyperlinkFileMoniker FromStream(XlsReader reader) {
			XlsHyperlinkFileMoniker result = new XlsHyperlinkFileMoniker();
			result.Read(reader);
			return result;
		}
		public XlsHyperlinkFileMoniker()
			: base(XlsHyperlinkMonikerFactory.CLSID_FileMoniker) {
		}
		protected void Read(XlsReader reader) {
			reader.ReadUInt16(); 
			int ansiLength = reader.ReadInt32();
			if(ansiLength > 0) {
				byte[] buf = reader.ReadBytes(ansiLength);
				Path = XLStringEncoder.GetEncoding(false).GetString(buf, 0, ansiLength - 1);
			}
			reader.ReadUInt16(); 
			ushort versionNumber = reader.ReadUInt16();
			if(versionNumber != serializationVersionNumber)
				throw new Exception("Invalid XLS file: Wrong file moniker serialization version number");
			reader.ReadBytes(16); 
			reader.ReadBytes(4); 
			int cbUnicodePathSize = reader.ReadInt32();
			if(cbUnicodePathSize > 0) {
				int cbUnicodePathBytes = reader.ReadInt32();
				reader.ReadUInt16(); 
				byte[] buf = reader.ReadBytes(cbUnicodePathBytes);
				Path = XLStringEncoder.GetEncoding(true).GetString(buf, 0, buf.Length);
			}
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			ushort cbAnti = (ushort)GetParentDirectoryIndicatorsNumber();
			writer.Write(cbAnti);
			byte[] buf = XLStringEncoder.GetBytes(Path + '\0', false);
			writer.Write(buf.Length);
			writer.Write(buf);
			writer.Write((ushort)GetUNCServerPartCharCount());
			writer.Write(serializationVersionNumber);
			writer.Write((long)0); 
			writer.Write((long)0); 
			writer.Write((int)0); 
			if(XLStringEncoder.StringHasHighBytes(Path)) {
				buf = XLStringEncoder.GetBytes(Path, true);
				writer.Write(buf.Length + 6);
				writer.Write(buf.Length);
				writer.Write((ushort)3); 
				writer.Write(buf);
			}
			else
				writer.Write((int)0);
		}
		public override int GetSize() {
			int size = 6; 
			size += Path.Length + 1; 
			size += 24; 
			size += 4; 
			if(XLStringEncoder.StringHasHighBytes(Path))
				size += Path.Length * 2 + 6; 
			return base.GetSize() + size;
		}
		#region Internals
		protected internal int GetParentDirectoryIndicatorsNumber() {
			int result = 0;
			int startIndex = 0;
			while((startIndex < Path.Length) && (Path.IndexOf(@"..\", startIndex) == startIndex)) {
				result++;
				startIndex += 3;
			}
			return result;
		}
		protected internal int GetUNCServerPartCharCount() {
			int result = -1;
			if(Path.IndexOf(@"\\") == 0) {
				int pos = Path.IndexOf(@"\", 2);
				if(pos == -1)
					result = Path.Length;
				else
					result = pos;
			}
			return result;
		}
		#endregion
	}
	#endregion
	#region XlsHyperlinkCompositeMoniker
	public class XlsHyperlinkCompositeMoniker : XlsHyperlinkMonikerBase {
		readonly List<XlsHyperlinkMonikerBase> items = new List<XlsHyperlinkMonikerBase>();
		public List<XlsHyperlinkMonikerBase> Items { get { return items; } }
		public static XlsHyperlinkCompositeMoniker FromStream(XlsReader reader) {
			XlsHyperlinkCompositeMoniker result = new XlsHyperlinkCompositeMoniker();
			result.Read(reader);
			return result;
		}
		public XlsHyperlinkCompositeMoniker()
			: base(XlsHyperlinkMonikerFactory.CLSID_CompositeMoniker) {
		}
		protected void Read(XlsReader reader) {
			int count = reader.ReadInt32();
			for(int i = 0; i < count; i++) {
				XlsHyperlinkMonikerBase item = XlsHyperlinkMonikerFactory.Create(reader);
				items.Add(item);
			}
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			int count = items.Count;
			writer.Write(count);
			for(int i = 0; i < count; i++)
				items[i].Write(writer);
		}
		public override int GetSize() {
			int size = 0;
			for(int i = 0; i < items.Count; i++)
				size += items[i].GetSize();
			return base.GetSize() + 4 + size;
		}
	}
	#endregion
	#region XlsHyperlinkAntiMoniker
	public class XlsHyperlinkAntiMoniker : XlsHyperlinkMonikerBase {
		int count;
		public int Count {
			get { return count; }
			set {
				if (value < 0 || value > 1048576)
					throw new ArgumentOutOfRangeException("Count value out of range 0...1048576");
				count = value;
			}
		}
		public static XlsHyperlinkAntiMoniker FromStream(XlsReader reader) {
			XlsHyperlinkAntiMoniker result = new XlsHyperlinkAntiMoniker();
			result.Read(reader);
			return result;
		}
		public XlsHyperlinkAntiMoniker()
			: base(XlsHyperlinkMonikerFactory.CLSID_AntiMoniker) {
		}
		protected void Read(XlsReader reader) {
			Count = reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write(count);
		}
		public override int GetSize() {
			return base.GetSize() + 4;
		}
	}
	#endregion
	#region XlsHyperlinkItemMoniker
	public class XlsHyperlinkItemMoniker : XlsHyperlinkMonikerBase {
		string delimiter = string.Empty;
		string item = string.Empty;
		#region Properties
		public string Delimiter {
			get { return delimiter; }
			set {
				if(value == null)
					value = string.Empty;
				delimiter = value;
			}
		}
		public string Item {
			get { return item; }
			set {
				if(value == null)
					value = string.Empty;
				item = value;
			}
		}
		#endregion
		public static XlsHyperlinkItemMoniker FromStream(XlsReader reader) {
			XlsHyperlinkItemMoniker result = new XlsHyperlinkItemMoniker();
			result.Read(reader);
			return result;
		}
		public XlsHyperlinkItemMoniker()
			: base(XlsHyperlinkMonikerFactory.CLSID_ItemMoniker) {
			Delimiter = string.Empty;
			Item = string.Empty;
		}
		protected void Read(XlsReader reader) {
			Delimiter = ReadComplexString(reader);
			Item = ReadComplexString(reader);
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			WriteComplexString(writer, Delimiter);
			WriteComplexString(writer, Item);
		}
		public override int GetSize() {
			return base.GetSize() + GetComplexStringSize(Delimiter) + GetComplexStringSize(Item);
		}
		#region Internals
		string ReadComplexString(XlsReader reader) {
			int length = reader.ReadInt32();
			byte[] buf = reader.ReadBytes(length);
			int pos = GetNullTerminatingPosition(buf);
			if(pos == -1)
				return XLStringEncoder.GetEncoding(false).GetString(buf, 0, length);
			if(pos < (length - 1))
				return XLStringEncoder.GetEncoding(true).GetString(buf, pos + 1, length - pos - 1);
			return XLStringEncoder.GetEncoding(false).GetString(buf, 0, pos);
		}
		void WriteComplexString(BinaryWriter writer, string value) {
			int length = GetComplexStringBytesCount(value);
			writer.Write(length);
			writer.Write(XLStringEncoder.GetBytes(value, false));
			writer.Write((byte)0); 
			if(XLStringEncoder.StringHasHighBytes(value))
				writer.Write(XLStringEncoder.GetBytes(value, true));
		}
		int GetComplexStringSize(string value) {
			return GetComplexStringBytesCount(value) + 4;
		}
		int GetComplexStringBytesCount(string value) {
			int result = value.Length + 1;
			if(XLStringEncoder.StringHasHighBytes(value))
				result += value.Length * 2;
			return result;
		}
		int GetNullTerminatingPosition(byte[] buf) {
			for(int i = 0; i < buf.Length; i++)
				if(buf[i] == 0) return i;
			return -1;
		}
		#endregion
	}
	#endregion
	#region XlsHyperlinkMonikerFactory
	public static class XlsHyperlinkMonikerFactory {
		public static readonly Guid CLSID_URLMoniker = new Guid("{0x79EAC9E0, 0xBAF9, 0x11CE, {0x8C, 0x82, 0x00, 0xAA, 0x00, 0x4B, 0xA9, 0x0B}}");
		public static readonly Guid CLSID_FileMoniker = new Guid("{0x00000303, 0x0000, 0x0000, {0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46}}");
		public static readonly Guid CLSID_CompositeMoniker = new Guid("{0x00000309, 0x0000, 0x0000, {0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46}}");
		public static readonly Guid CLSID_AntiMoniker = new Guid("{0x00000305, 0x0000, 0x0000, {0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46}}");
		public static readonly Guid CLSID_ItemMoniker = new Guid("{0x00000304, 0x0000, 0x0000, {0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46}}");
		public static XlsHyperlinkMonikerBase Create(XlsReader reader) {
			Guid classId = new Guid(reader.ReadBytes(16));
			if(classId == CLSID_URLMoniker)
				return XlsHyperlinkURLMoniker.FromStream(reader);
			if(classId == CLSID_FileMoniker)
				return XlsHyperlinkFileMoniker.FromStream(reader);
			if(classId == CLSID_CompositeMoniker)
				return XlsHyperlinkCompositeMoniker.FromStream(reader);
			if(classId == CLSID_AntiMoniker)
				return XlsHyperlinkAntiMoniker.FromStream(reader);
			if(classId == CLSID_ItemMoniker)
				return XlsHyperlinkItemMoniker.FromStream(reader);
			throw new Exception("Invalid XLS file: Unknown hyperlink moniker CLSID");
		}
	}
	#endregion
	#region XlsHyperlinkObject
	public class XlsHyperlinkObject {
		#region Fields
		const int fixedPartSize = 8;
		const int predefinedStreamVersion = 2;
		public static readonly Guid CLSID_StdHyperlink = new Guid("{0x79EAC9D0, 0xBAF9, 0x11CE, {0x8C, 0x82, 0x00, 0xAA, 0x00, 0x4B, 0xA9, 0x0B}}");
		HyperlinkString displayName = new HyperlinkString();
		HyperlinkString frameName = new HyperlinkString();
		HyperlinkString moniker = new HyperlinkString();
		HyperlinkString location = new HyperlinkString();
		Guid optionalGUID = new Guid();
		#endregion
		public static XlsHyperlinkObject FromStream(XlsReader reader) {
			XlsHyperlinkObject result = new XlsHyperlinkObject();
			result.Read(reader);
			return result;
		}
		public static XlsHyperlinkObject FromData(byte[] data) {
			using(MemoryStream stream = new MemoryStream(data, false)) {
				using(BinaryReader baseReader = new BinaryReader(stream)) {
					using(XlsReader reader = new XlsReader(baseReader)) {
						Guid classId = new Guid(reader.ReadBytes(16));
						if(classId != CLSID_StdHyperlink)
							throw new Exception("Invalid XLS file: Wrong hyperlink class id");
						return FromStream(reader);
					}
				}
			}
		}
		#region Properties
		public bool HasMoniker { get; set; }
		public bool IsAbsolute { get; set; }
		public bool SiteGaveDisplayName { get; set; }
		public bool HasLocationString { get; set; }
		public bool HasDisplayName { get; set; }
		public bool HasGUID { get; set; }
		public bool HasCreationTime { get; set; }
		public bool HasFrameName { get; set; }
		public bool IsMonkerSavedAsString { get; set; }
		public bool IsAbsoluteFromRelative { get; set; }
		public string DisplayName {
			get { return displayName.Value; }
			set { displayName.Value = value; }
		}
		public string FrameName {
			get { return frameName.Value; }
			set { frameName.Value = value; }
		}
		public string Moniker {
			get { return moniker.Value; }
			set { moniker.Value = value; }
		}
		public XlsHyperlinkMonikerBase OleMoniker { get; set; }
		public string Location {
			get { return location.Value; }
			set { location.Value = value; }
		}
		public Guid OptionalGUID {
			get { return optionalGUID; }
			set {
				Guard.ArgumentNotNull(value, "OptionalGUID");
				optionalGUID = value;
			}
		}
		public Int64 CreationTime { get; set; }
		#endregion
		protected void Read(XlsReader reader) {
			int streamVersion = reader.ReadInt32();
			if(streamVersion != predefinedStreamVersion)
				throw new Exception("Invalid XLS file: Wrong hyperlink stream version");
			uint bitwiseField = reader.ReadUInt32();
			HasMoniker = Convert.ToBoolean(bitwiseField & 0x0001);
			IsAbsolute = Convert.ToBoolean(bitwiseField & 0x0002);
			SiteGaveDisplayName = Convert.ToBoolean(bitwiseField & 0x0004);
			HasLocationString = Convert.ToBoolean(bitwiseField & 0x0008);
			HasDisplayName = Convert.ToBoolean(bitwiseField & 0x0010);
			HasGUID = Convert.ToBoolean(bitwiseField & 0x0020);
			HasCreationTime = Convert.ToBoolean(bitwiseField & 0x0040);
			HasFrameName = Convert.ToBoolean(bitwiseField & 0x0080);
			IsMonkerSavedAsString = Convert.ToBoolean(bitwiseField & 0x0100);
			IsAbsoluteFromRelative = Convert.ToBoolean(bitwiseField & 0x0200);
			if(HasDisplayName)
				this.displayName = HyperlinkString.FromStream(reader);
			if(HasFrameName)
				this.frameName = HyperlinkString.FromStream(reader);
			if(HasMoniker) {
				if(IsMonkerSavedAsString)
					this.moniker = HyperlinkString.FromStream(reader);
				else
					OleMoniker = XlsHyperlinkMonikerFactory.Create(reader);
			}
			if(HasLocationString)
				this.location = HyperlinkString.FromStream(reader);
			if(HasGUID)
				this.optionalGUID = new Guid(reader.ReadBytes(16));
			if(HasCreationTime)
				CreationTime = reader.ReadInt64();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((int)predefinedStreamVersion);
			uint bitwiseField = 0;
			if(HasMoniker)
				bitwiseField |= 0x0001;
			if(IsAbsolute)
				bitwiseField |= 0x0002;
			if(SiteGaveDisplayName)
				bitwiseField |= 0x0004;
			if(HasLocationString)
				bitwiseField |= 0x0008;
			if(HasDisplayName)
				bitwiseField |= 0x0010;
			if(HasGUID)
				bitwiseField |= 0x0020;
			if(HasCreationTime)
				bitwiseField |= 0x0040;
			if(HasFrameName)
				bitwiseField |= 0x0080;
			if(IsMonkerSavedAsString)
				bitwiseField |= 0x0100;
			if(IsAbsoluteFromRelative)
				bitwiseField |= 0x0200;
			writer.Write(bitwiseField);
			if(HasDisplayName)
				this.displayName.Write(writer);
			if(HasFrameName)
				this.frameName.Write(writer);
			if(HasMoniker) {
				if(IsMonkerSavedAsString)
					this.moniker.Write(writer);
				else
					OleMoniker.Write(writer);
			}
			if(HasLocationString)
				this.location.Write(writer);
			if(HasGUID)
				writer.Write(this.optionalGUID.ToByteArray());
			if(HasCreationTime)
				writer.Write(CreationTime);
		}
		public int GetSize() {
			int result = fixedPartSize;
			if(HasDisplayName)
				result += this.displayName.Length;
			if(HasFrameName)
				result += this.frameName.Length;
			if(HasMoniker) {
				if(IsMonkerSavedAsString)
					result += this.moniker.Length;
				else
					result += OleMoniker.GetSize();
			}
			if(HasLocationString)
				result += this.location.Length;
			if(HasGUID)
				result += 16;
			if(HasCreationTime)
				result += 8;
			return result;
		}
		public byte[] GetHyperlinkData() {
			using(MemoryStream stream = new MemoryStream()) {
				using(BinaryWriter writer = new BinaryWriter(stream)) {
					writer.Write(CLSID_StdHyperlink.ToByteArray());
					Write(writer);
				}
				return stream.ToArray();
			}
		}
	}
	#endregion
	#region XlsDataAwareExporter
	public partial class XlsDataAwareExporter {
		void WriteHyperlinks() {
			int count = currentSheet.Hyperlinks.Count;
			for(int i = 0; i < count; i++)
				WriteHyperlink(currentSheet.Hyperlinks[i]);
		}
		void WriteHyperlink(XlHyperlink hyperlink) {
			if(hyperlink.Reference == null || string.IsNullOrEmpty(hyperlink.TargetUri))
				return;
			XlsRef8 range = XlsRef8.FromRange(hyperlink.Reference);
			if(range == null)
				return;
			XlsContentHyperlink content = new XlsContentHyperlink();
			content.Range = range;
			string targetUri = hyperlink.TargetUri;
			string location = string.Empty;
			int pos = targetUri.IndexOf('#');
			if(pos != -1) {
				location = targetUri.Substring(pos + 1);
				targetUri = targetUri.Substring(0, pos);
			}
			if(!string.IsNullOrEmpty(targetUri)) {
				Uri uri;
				if(!Uri.TryCreate(targetUri, UriKind.RelativeOrAbsolute, out uri))
					return;
				content.HasMoniker = true;
				content.IsAbsolute = uri.IsAbsoluteUri;
#if DXRESTRICTED
				if (uri.IsAbsoluteUri && uri.Scheme != "file") {
#else
				if (uri.IsAbsoluteUri && uri.Scheme != Uri.UriSchemeFile) {
#endif
					XlsHyperlinkURLMoniker urlMoniker = new XlsHyperlinkURLMoniker();
					urlMoniker.Url = targetUri;
					urlMoniker.HasOptionalData = true;
					urlMoniker.AllowImplicitFileScheme = true;
					urlMoniker.AllowRelative = true;
					urlMoniker.Canonicalize = true;
					urlMoniker.CrackUnknownSchemes = true;
					urlMoniker.DecodeExtraInfo = true;
					urlMoniker.IESettings = true;
					urlMoniker.NoEncodeForbiddenChars = true;
					urlMoniker.PreProcessHtmlUri = true;
					content.OleMoniker = urlMoniker;
				}
				else {
					XlsHyperlinkFileMoniker fileMoniker = new XlsHyperlinkFileMoniker();
					fileMoniker.Path = targetUri;
					content.OleMoniker = fileMoniker;
				}
			}
			if(!string.IsNullOrEmpty(location)) {
				content.Location = location;
				content.HasLocationString = true;
			}
			if(!string.IsNullOrEmpty(hyperlink.DisplayText)) {
				content.DisplayName = hyperlink.DisplayText;
				content.HasDisplayName = true;
				content.SiteGaveDisplayName = true;
			}
			WriteContent(XlsRecordType.HLink, content);
			if(!string.IsNullOrEmpty(hyperlink.Tooltip))
				WriteHyperlinkTooltip(hyperlink, range);
		}
		void WriteHyperlinkTooltip(XlHyperlink hyperlink, XlsRef8 range) {
			XlsContentHyperlinkTooltip content = new XlsContentHyperlinkTooltip();
			content.Range = range;
			content.Tooltip = hyperlink.Tooltip;
			WriteContent(XlsRecordType.HLinkTooltip, content);
		}
	}
#endregion
}
