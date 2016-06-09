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
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsSupportingLinkType
	public enum XlsSupportingLinkType {
		Self,
		SameSheet,
		AddIn,
		ExternalWorkbook,
		DataSource,
		Unused
	}
	#endregion
	#region XlsSupBookInfo
	public class XlsSupBookInfo : IEquatable<XlsSupBookInfo> {
		#region Fields
		const int selfRef = 0x0401;
		const int addInRef = 0x3a01;
		int sheetCount;
		string virtualPath = string.Empty;
		List<string> sheetNames = new List<string>();
		List<XlsDefinedNameInfo> externalNames = new List<XlsDefinedNameInfo>();
		#endregion
		#region Properties
		public XlsSupportingLinkType LinkType { get; set; }
		public int SheetCount {
			get { return sheetCount; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "SheetCount");
				sheetCount = value;
			}
		}
		public string VirtualPath {
			get { return virtualPath; }
			set {
				if(string.IsNullOrEmpty(value))
					virtualPath = string.Empty;
				else if(value.Length > 255)
					virtualPath = value.Substring(0, 255);
				else
					virtualPath = value;
			}
		}
		public List<string> SheetNames { get { return sheetNames; } }
		protected internal int ExternalIndex { get; set; }
		protected internal List<XlsDefinedNameInfo> ExternalNames { get { return externalNames; } }
		#endregion
		public static XlsSupBookInfo FromStream(BinaryReader reader) {
			XlsSupBookInfo result = new XlsSupBookInfo();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			this.sheetCount = reader.ReadUInt16();
			ushort cch = reader.ReadUInt16();
			if(cch == selfRef) {
				LinkType = XlsSupportingLinkType.Self;
			}
			else if(cch == addInRef) {
				LinkType = XlsSupportingLinkType.AddIn;
			}
			else if(cch >= 0x0000 && cch <= 0x00ff) { 
				XLUnicodeStringNoCch virtPath = XLUnicodeStringNoCch.FromStream(reader, cch);
				this.virtualPath = virtPath.Value;
				if(XlsVirtualPath.IsUnusedLink(this.virtualPath))
					LinkType = XlsSupportingLinkType.Unused;
				else if(XlsVirtualPath.IsSameSheetLink(this.virtualPath))
					LinkType = XlsSupportingLinkType.SameSheet;
				else if(XlsVirtualPath.IsOleLink(this.virtualPath))
					LinkType = XlsSupportingLinkType.DataSource;
				else
					LinkType = XlsSupportingLinkType.ExternalWorkbook;
				if(LinkType == XlsSupportingLinkType.Unused || LinkType == XlsSupportingLinkType.ExternalWorkbook) {
					for(int i = 0; i < SheetCount; i++) {
						XLUnicodeString sheetName = XLUnicodeString.FromStream(reader);
						SheetNames.Add(CleanupSheetName(sheetName.Value, i + 1));
					}
					if(IsPreinstalledAddIn())
						LinkType = XlsSupportingLinkType.AddIn;
				}
			}
		}
		static List<string> preinstalledAddIns = CreatePreinstalledAddIns();
		static List<string> CreatePreinstalledAddIns() {
			List<string> result = new List<string>();
			result.Add("ANALYS32.XLL");
			result.Add("ATPVBAEN.XLA");
			result.Add("FUNCRES.XLA");
			result.Add("PROCDB.XLA");
			result.Add("EUROTOOL.XLA");
			result.Add("SOLVER.XLA");
			return result;
		}
		bool IsPreinstalledAddIn() {
			string upperCasePath = this.virtualPath.ToUpper(CultureInfo.InvariantCulture);
			foreach (string addInName in preinstalledAddIns) {
				if (upperCasePath.Contains(addInName))
					return true;
			}
			return false;
		}
		public void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(LinkType == XlsSupportingLinkType.SameSheet || LinkType == XlsSupportingLinkType.DataSource)
				writer.Write((ushort)0);
			else if(LinkType == XlsSupportingLinkType.AddIn)
				writer.Write((ushort)1);
			else
				writer.Write((ushort)this.sheetCount);
			if(LinkType == XlsSupportingLinkType.Self)
				writer.Write((ushort)selfRef);
			else if(LinkType == XlsSupportingLinkType.AddIn)
				writer.Write((ushort)addInRef);
			else {
				XLUnicodeStringNoCch virtPath = new XLUnicodeStringNoCch();
				if(LinkType == XlsSupportingLinkType.Unused)
					virtPath.Value = Char.ToString('\u0020');
				else if(LinkType == XlsSupportingLinkType.SameSheet)
					virtPath.Value = Char.ToString('\u0000');
				else
					virtPath.Value = this.virtualPath;
				writer.Write((ushort)virtPath.Value.Length);
				virtPath.Write(writer);
				if(LinkType == XlsSupportingLinkType.Unused) {
					XLUnicodeString sheetName = new XLUnicodeString();
					sheetName.Value = Char.ToString('\u0020');
					for(int i = 0; i < sheetCount; i++) {
						if(chunkWriter != null) chunkWriter.BeginRecord(sheetName.Length);
						sheetName.Write(writer);
					}
				}
				else if(LinkType == XlsSupportingLinkType.ExternalWorkbook) {
					for(int i = 0; i < SheetNames.Count; i++) {
						XLUnicodeString sheetName = new XLUnicodeString();
						sheetName.Value = LimitLength(SheetNames[i], XlsDefs.MaxSheetNameLength);
						if(chunkWriter != null) chunkWriter.BeginRecord(sheetName.Length);
						sheetName.Write(writer);
					}
				}
			}
		}
		#region IEquatable<XlsSupBookInfo> Members
		public bool Equals(XlsSupBookInfo other) {
			if(other == null) return false;
			if(LinkType != other.LinkType) return false;
			if(LinkType == XlsSupportingLinkType.DataSource || LinkType == XlsSupportingLinkType.ExternalWorkbook)
				return StringExtensions.CompareInvariantCultureIgnoreCase(VirtualPath, other.VirtualPath) == 0;
			return true;
		}
		#endregion
		protected internal string GetSheetNameByIndex(int index) {
			if(index < 0 || index >= SheetNames.Count)
				return string.Empty;
			return SheetNames[index];
		}
		protected internal void RegisterExternalName(string name, int scope) {
			XlsDefinedNameInfo info = new XlsDefinedNameInfo();
			info.Name = name;
			info.Scope = scope;
			ExternalNames.Add(info);
		}
		protected internal bool IsRegisteredExternalName(string name, int scope) {
			return IndexOfExternalName(name, scope) != -1;
		}
		protected internal int IndexOfExternalName(string name, int scope) {
			XlsDefinedNameInfo info = new XlsDefinedNameInfo();
			info.Name = name;
			info.Scope = scope;
			return ExternalNames.IndexOf(info);
		}
		protected internal int IndexOfExternalName(string name, string sheetName) {
			int scope = SheetNames.IndexOf(sheetName);
			if(scope == -1)
				scope = XlsDefs.NoScope;
			XlsDefinedNameInfo info = new XlsDefinedNameInfo();
			info.Name = name;
			info.Scope = scope;
			return ExternalNames.IndexOf(info);
		}
		string CleanupSheetName(string value, int index) {
			int pos = value.LastIndexOf(']');
			if(pos != -1)
				value = value.Remove(0, pos + 1);
			if(string.IsNullOrEmpty(value))
				value = "Sheet" + index.ToString();
			if(value.Length > XlsDefs.MaxSheetNameLength)
				return value.Substring(0, XlsDefs.MaxSheetNameLength);
			return value;
		}
		string LimitLength(string value, int maxLength) {
			if(string.IsNullOrEmpty(value)) 
				return string.Empty;
			if(value.Length > maxLength)
				return value.Substring(0, maxLength);
			return value;
		}
	}
	#endregion
	#region XlsCommandSupBook
	public class XlsCommandSupBook : XlsCommandRecordBase {
		#region Fields
		short[] typeCodes = new short[] {
			0x003c 
		};
		XlsSupBookInfo info;
		#endregion
		public XlsSupBookInfo Info { get { return info; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if(contentBuilder.ContentType == XlsContentType.WorkbookGlobals) {
				using(XlsCommandStream commandStream = new XlsCommandStream(reader, typeCodes, Size)) {
					using(BinaryReader commandReader = new BinaryReader(commandStream)) {
						this.info = XlsSupBookInfo.FromStream(commandReader);
					}
				}
			}
			else
				base.ReadCore(reader, contentBuilder);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			contentBuilder.RegisterSupBook(Info);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandSupBook();
		}
	}
	#endregion
	#region XlsExternInfo
	public class XlsExternInfo : IEquatable<XlsExternInfo> {
		int supBookIndex;
		int firstSheetIndex;
		int lastSheetIndex;
		#region Properties
		public int SupBookIndex {
			get { return supBookIndex; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "SupBookIndex");
				supBookIndex = value;
			}
		}
		public int FirstSheetIndex {
			get { return firstSheetIndex; }
			set {
				ValueChecker.CheckValue(value, -2, short.MaxValue, "FirstSheetIndex");
				firstSheetIndex = value;
			}
		}
		public int LastSheetIndex {
			get { return lastSheetIndex; }
			set {
				ValueChecker.CheckValue(value, -2, short.MaxValue, "LastSheetIndex");
				lastSheetIndex = value;
			}
		}
		public bool EntireWorkbook { get { return firstSheetIndex == -2; } }
		public bool FirstSheetNotFound { get { return firstSheetIndex == -1; } }
		public bool LastSheetNotFound { get { return lastSheetIndex == -1; } }
		#endregion
		public static XlsExternInfo FromStream(BinaryReader reader) {
			XlsExternInfo result = new XlsExternInfo();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			SupBookIndex = reader.ReadUInt16();
			FirstSheetIndex = reader.ReadInt16();
			LastSheetIndex = reader.ReadInt16();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)SupBookIndex);
			writer.Write((short)FirstSheetIndex);
			writer.Write((short)LastSheetIndex);
		}
		#region IEquatable<XlsExternInfo> Members
		public bool Equals(XlsExternInfo other) {
			if(other == null) return false;
			return SupBookIndex == other.SupBookIndex &&
				FirstSheetIndex == other.FirstSheetIndex &&
				LastSheetIndex == other.LastSheetIndex;
		}
		#endregion
	}
	#endregion
	#region XlsExternInfoExtensions
	public static class XlsExternInfoExtensions {
		const int xtiStructSize = 6;
		public static void Read(this List<XlsExternInfo> list, BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			int count = reader.ReadUInt16();
			for(int i = 0; i < count; i++) {
				XlsExternInfo item = XlsExternInfo.FromStream(reader);
				list.Add(item);
			}
		}
		public static void Write(this List<XlsExternInfo> list, BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			int count = list.Count;
			writer.Write((ushort)count);
			for(int i = 0; i < count; i++) {
				if(chunkWriter != null) chunkWriter.BeginRecord(xtiStructSize);
				list[i].Write(writer);
			}
		}
	}
	#endregion
	#region XlsCommandExternSheet
	public class XlsCommandExternSheet : XlsCommandRecordBase {
		#region Fields
		short[] typeCodes = new short[] {
			0x003c 
		};
		List<XlsExternInfo> externInfoList = new List<XlsExternInfo>();
		#endregion
		public List<XlsExternInfo> ExternInfoList { get { return externInfoList; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if(contentBuilder.ContentType == XlsContentType.WorkbookGlobals) {
				using(XlsCommandStream commandStream = new XlsCommandStream(reader, typeCodes, Size)) {
					using(BinaryReader commandReader = new BinaryReader(commandStream)) {
						this.externInfoList.Read(commandReader);
					}
				}
			}
			else
				base.ReadCore(reader, contentBuilder);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			contentBuilder.RPNContext.ExternSheets.AddRange(ExternInfoList);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandExternSheet();
		}
	}
	#endregion
	#region XlsVirtualPath
	public static class XlsVirtualPath {
#if !DXPORTABLE
		static readonly char directorySeparatorChar = Path.DirectorySeparatorChar;
		static readonly char volumeSeparatorChar = Path.VolumeSeparatorChar;
		static string GetDirectoryName(string path) {
			return Path.GetDirectoryName(path);
		}
		static string CombinePath(string path1, string path2) {
			return Path.Combine(path1, path2);
		}
		static bool IsPathRooted(string path) {
			return Path.IsPathRooted(path);
		}
		static string GetRootPath(string path) {
			return Path.GetPathRoot(path);
		}
#else
		const char directorySeparatorChar = '\\';
		const char altDirectorySeparatorChar = '/';
		const char volumeSeparatorChar = ':';
		const string directorySeparatorCharAsString = "\\";
		static string GetDirectoryName(string path) {
			if (!string.IsNullOrEmpty(path)) {
				int index = path.LastIndexOf(altDirectorySeparatorChar);
				if (index == -1)
					index = path.LastIndexOf(directorySeparatorChar);
				if (index != -1)
					return path.Substring(0, index);
			}
			return string.Empty;
		}
		static string CombinePath(string path1, string path2) {
			if(path1 == null || path2 == null)
				throw new ArgumentNullException(path1 == null ? "path1" : "path2");
			if (path2.Length == 0)
				return path1;
			if (path1.Length == 0)
				return path2;
			if (IsPathRooted(path2))
				return path2;
			char ch = path1[path1.Length - 1];
			if (ch != directorySeparatorChar && ch != altDirectorySeparatorChar && ch != volumeSeparatorChar)
				return path1 + directorySeparatorCharAsString + path2;
			return path1 + path2;
		}
		static bool IsPathRooted(string path) {
			if (path != null) {
				int length = path.Length;
				if ((length >= 1 && (path[0] == directorySeparatorChar || path[0] == altDirectorySeparatorChar)) || (length >= 2 && path[1] == volumeSeparatorChar))
					return true;
			}
			return false;
		}
		static string GetRootPath(string path) {
			if(string.IsNullOrEmpty(path))
				return string.Empty;
			return path.Substring(0, GetRootPathLength(path));
		}
		static int GetRootPathLength(string path) {
			int result = 0;
			int len = path.Length;
			if (len >= 1 && (path[0] == directorySeparatorChar || path[0] == altDirectorySeparatorChar)) {
				result = 1;
				if (len >= 2 && (path[1] == directorySeparatorChar || path[1] == altDirectorySeparatorChar)) {
					int n = 2;
					result = 2;
					while (result < len && ((path[result] != directorySeparatorChar && path[result] != altDirectorySeparatorChar) || --n > 0)) 
						result++;
				}
			}
			else if (len >= 2 && path[1] == volumeSeparatorChar) {
				result = 2;
				if (len >= 3 && (path[2] == directorySeparatorChar || path[2] == altDirectorySeparatorChar)) 
					result++;
			}
			return result;
		}
#endif
		public static bool IsUnusedLink(string virtualPath) {
			return virtualPath == Char.ToString('\u0020');
		}
		public static bool IsSameSheetLink(string virtualPath) {
			return virtualPath == Char.ToString('\u0000');
		}
		public static bool IsOleLink(string virtualPath) {
			if(string.IsNullOrEmpty(virtualPath)) return false;
			string[] parts = virtualPath.Split(new char[] { '\u0003' });
			if(parts.Length != 2) return false;
			if (parts[0] == "Package")
				return IsPackagePathString(parts[1]);
			return IsOlePathString(parts[0]) && IsOlePathString(parts[1]);
		}
		public static bool IsSelfReference(string virtualPath) {
			if (string.IsNullOrEmpty(virtualPath)) return false;
			return virtualPath[0] == '\u0002';
		}
		public static string GetDdeServer(string virtualPath) {
			if(string.IsNullOrEmpty(virtualPath))
				return string.Empty;
			string[] parts = virtualPath.Split(new char[] { '\u0003' });
			if(parts.Length < 1) 
				return string.Empty;
			return parts[0];
		}
		public static string GetDdeTopic(string virtualPath) {
			if(string.IsNullOrEmpty(virtualPath))
				return string.Empty;
			string[] parts = virtualPath.Split(new char[] { '\u0003' });
			if(parts.Length < 2)
				return string.Empty;
			return parts[1];
		}
		public static string GetFilePath(string virtualPath) {
			return GetFilePath(virtualPath, new WorkbookSaveOptions());
		}
		public static string GetFilePath(string virtualPath, WorkbookSaveOptions options) {
			Guard.ArgumentNotNull(options, "options");
			if(IsUNCVolume(virtualPath)) {
				return ParseUNCPath(virtualPath.Substring(3));
			}
			else if(IsVolume(virtualPath)) {
				string volume = virtualPath.Substring(2, 1) + volumeSeparatorChar + directorySeparatorChar;
				return volume + ParseFilePath(virtualPath.Substring(3));
			}
			else if(IsRelativeVolume(virtualPath)) {
				return CombinePath(GetDriveVolume(options.CurrentFileName), ParseFilePath(virtualPath.Substring(2)));
			}
			else if(IsStartup(virtualPath)) {
				return CombinePath(options.StartupPath, ParseFilePath(virtualPath.Substring(2)));
			}
			else if(IsAltStartup(virtualPath)) {
				return CombinePath(options.AltStartupPath, ParseFilePath(virtualPath.Substring(2)));
			}
			else if(IsLibrary(virtualPath)) {
				return CombinePath(options.LibraryPath, ParseFilePath(virtualPath.Substring(2)));
			}
			else if(IsTransferProtocol(virtualPath)) {
				return ParseTransferPath(virtualPath.Substring(3));
			}
			else if(IsSimpleFilePath(virtualPath)) {
				string simplePath = IsPathCore(virtualPath) ? virtualPath.Substring(1) : virtualPath;
#if !SL && !DXPORTABLE
				try {
					string result;
					if(string.IsNullOrEmpty(options.CurrentFileName))
						result = ParseSimpleFilePath(simplePath);
					else {
						string oldDirectory = Directory.GetCurrentDirectory();
						Directory.SetCurrentDirectory(GetDirectoryName(options.CurrentFileName));
						try {
							result = ParseSimpleFilePath(simplePath);
						}
						finally {
							Directory.SetCurrentDirectory(oldDirectory);
						}
					}
					return result;
				}
				catch {
					return ParseSimpleFilePath(simplePath);
				}
#else
				return ParseSimpleFilePath(simplePath);
#endif
			}
			return string.Empty;
		}
		public static string GetVirtualPath(string server, string topic) {
			return server + Char.ToString('\u0003') + topic;
		}
		public static string GetVirtualPath(string filePath) {
			return GetVirtualPath(filePath, new WorkbookSaveOptions());
		}
		public static string GetVirtualPath(string filePath, WorkbookSaveOptions options) {
			Guard.ArgumentNotNull(options, "options");
			StringBuilder sb = new StringBuilder();
			sb.Append('\u0001');
			try {
				if(filePath.IndexOf("://") != -1) { 
					string transferPath = CreateTransferPath(filePath);
					int count = transferPath.Length;
					sb.Append('\u0005');
					sb.Append(Convert.ToChar(count));
					sb.Append(transferPath);
				}
				else if(IsUNCVolumePath(filePath)) {
					sb.Append('\u0001');
					sb.Append('\u0040');
					sb.Append(CreateUNCVolumePath(filePath));
				}
				else if(IsEnvironmentPath(filePath, options.StartupPath)) {
					sb.Append('\u0006');
					sb.Append(CreateEnvironmentPath(filePath, options.StartupPath));
				}
				else if(IsEnvironmentPath(filePath, options.AltStartupPath)) {
					sb.Append('\u0007');
					sb.Append(CreateEnvironmentPath(filePath, options.AltStartupPath));
				}
				else if(IsEnvironmentPath(filePath, options.LibraryPath)) {
					sb.Append('\u0008');
					sb.Append(CreateEnvironmentPath(filePath, options.LibraryPath));
				}
				else if(IsRelativeVolumePath(filePath, options.CurrentFileName)) {
					sb.Append('\u0002');
					sb.Append(CreateRelativeVolumePath(filePath, options.CurrentFileName));
				}
				else if(IsVolumePath(filePath, options.CurrentFileName)) {
					sb.Append('\u0001');
					sb.Append(CreateVolumePath(filePath));
				}
				else
					sb.Append(CreateSimpleFilePath(filePath, options.CurrentFileName));
			}
			catch {
				sb.Append(CreateSimpleFilePath(filePath, string.Empty));
			}
			return sb.ToString();
		}
		#region Internals
		#region Common
		static bool IsOlePathString(string path) {
			if (string.IsNullOrEmpty(path)) return false;
			for (int i = 0; i < path.Length; i++) {
				Char ch = path[i];
				if (ch >= '\u0020' && ch <= '\u0021') continue;
				if (ch >= '\u0023' && ch <= '\u0029') continue;
				if (ch >= '\u002b' && ch <= '\u002e') continue;
				if (ch >= '\u0030' && ch <= '\u0039') continue;
				if (ch == '\u003b') continue;
				if (ch == '\u003d') continue;
				if (ch == '\u003f') continue;
				if (ch >= '\u0040' && ch <= '\u005b') continue;
				if (ch >= '\u005d' && ch <= '\u007b') continue;
				if (ch >= '\u007d' && ch <= '\uffff') continue;
				return false;
			}
			return true;
		}
		static bool IsPackagePathString(string path) {
			if (string.IsNullOrEmpty(path)) return false;
			for (int i = 0; i < path.Length; i++) {
				Char ch = path[i];
				if (ch >= '\u0020' && ch <= '\u0021') continue;
				if (ch >= '\u0023' && ch <= '\u0029') continue;
				if (ch >= '\u002b' && ch <= '\u002e') continue;
				if (ch >= '\u0030' && ch <= '\u0039') continue;
				if (ch == '\u003b') continue;
				if (ch == '\u003d') continue;
				if (ch >= '\u0040' && ch <= '\u007b') continue;
				if (ch >= '\u007d' && ch <= '\uffff') continue;
				return false;
			}
			return true;
		}
		static string GetDriveVolume(string path) {
			try {
				if(!IsPathRooted(path))
					return string.Empty;
				string rootPath = GetRootPath(path);
				if(rootPath.IndexOf(volumeSeparatorChar) == -1)
					return string.Empty;
				return rootPath;
			}
			catch { }
			return string.Empty;
		}
		#endregion
		#region GetFilePath
		static bool IsVolume(string path) {
			return (GetStringLength(path) > 2) && IsPathCore(path) && (path[1] == '\u0001');
		}
		static bool IsVolumeCore(string path) {
			return IsPathCore(path) && (path[1] == '\u0001');
		}
		static bool IsRelativeVolume(string path) {
			return (GetStringLength(path) > 2) && IsPathCore(path) && (path[1] == '\u0002');
		}
		static bool IsUNCVolume(string path) {
			return (GetStringLength(path) > 3) && IsVolumeCore(path) && (path[2] == '\u0040');
		}
		static bool IsStartup(string path) {
			return (GetStringLength(path) > 2) && IsPathCore(path) && (path[1] == '\u0006');
		}
		static bool IsAltStartup(string path) {
			return (GetStringLength(path) > 2) && IsPathCore(path) && (path[1] == '\u0007');
		}
		static bool IsLibrary(string path) {
			return (GetStringLength(path) > 2) && IsPathCore(path) && (path[1] == '\u0008');
		}
		static bool IsTransferProtocol(string path) {
			return (GetStringLength(path) > 2) && IsPathCore(path) && (path[1] == '\u0005');
		}
		static bool IsSimpleFilePath(string path) {
			return GetStringLength(path) > 0;
		}
		static bool IsPathCore(string path) {
			if(string.IsNullOrEmpty(path))
				return false;
			return path[0] == '\u0001';
		}
		static int GetStringLength(string text) {
			if(string.IsNullOrEmpty(text)) return 0;
			return text.Length;
		}
		static string ParseFilePath(string path) {
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string relativePath = path.Substring(1, pos - 1);
				string sheetName = path.Substring(pos + 1);
				return "[" + ParseRelativePath(relativePath) + "]" + sheetName;
			}
			return ParseRelativePath(path);
		}
		static string ParseRelativePath(string path) {
			StringBuilder sb = new StringBuilder();
			string[] parts = path.Split(new char[] { '\u0003' });
			foreach(string part in parts) {
				if(sb.Length > 0)
					sb.Append(directorySeparatorChar);
				sb.Append(part);
			}
			return sb.ToString();
		}
		static string ParseSimpleFilePath(string path) {
			if(string.IsNullOrEmpty(path)) 
				return path;
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string relativePath = path.Substring(1, pos - 1);
				string sheetName = path.Substring(pos + 1);
				return "[" + ParseSimpleRelativePath(relativePath) + "]" + sheetName;
			}
			return ParseSimpleRelativePath(path);
		}
		static string ParseSimpleRelativePath(string path) {
			StringBuilder sb = new StringBuilder();
			string[] parts = path.Split(new char[] { '\u0003' });
			foreach(string part in parts) {
				if(sb.Length > 0)
					sb.Append(directorySeparatorChar);
				sb.Append(part);
			}
			string result = sb.ToString();
			if(!string.IsNullOrEmpty(result) && (result[0] == '\u0004')) {
				try {
					result = Path.GetFullPath(result.Replace(Char.ToString('\u0004'), @"..\"));
				}
				catch {
					result = result.Replace(Char.ToString('\u0004'), null);
				}
			}
			return result;
		}
		static string ParseUNCPath(string path) {
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string uncBasePath = path.Substring(1, pos - 1);
				string sheetName = path.Substring(pos + 1);
				return "[" + ParseUNCBasePath(uncBasePath) + "]" + sheetName;
			}
			return ParseUNCBasePath(path);
		}
		static string ParseUNCBasePath(string path) {
			StringBuilder sb = new StringBuilder();
			string[] parts = path.Split(new char[] { '\u0003' });
			sb.Append(directorySeparatorChar);
			foreach(string part in parts) {
				sb.Append(directorySeparatorChar);
				sb.Append(part);
			}
			return sb.ToString();
		}
		static string ParseTransferPath(string path) {
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string uncBasePath = path.Substring(1, pos - 1);
				string sheetName = path.Substring(pos + 1);
				return "[" + ParseTransferBasePath(uncBasePath) + "]" + sheetName;
			}
			return ParseTransferBasePath(path);
		}
		static string ParseTransferBasePath(string path) {
			int pos = path.IndexOf("://");
			if(pos == -1)
				return ParseFilePath(path);
			string transferType = path.Substring(0, pos + 3);
			return transferType + ParseTransferFilePath(path.Substring(pos + 3));
		}
		static string ParseTransferFilePath(string path) {
			StringBuilder sb = new StringBuilder();
			string[] parts = path.Split(new char[] { '\u0003' });
			foreach(string part in parts) {
				if(sb.Length > 0)
					sb.Append(directorySeparatorChar);
				sb.Append(part);
			}
			return sb.ToString();
		}
		#endregion
		#region GetVirtualPath
		static string CreateTransferPath(string path) {
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string basePath = path.Substring(1, pos - 1);
				string sheetName = path.Substring(pos + 1);
				return "[" + CreateTransferBasePath(basePath) + "]" + sheetName;
			}
			return CreateTransferBasePath(path);
		}
		static string CreateTransferBasePath(string path) {
			return path;
		}
		static string CreateFilePath(string path) {
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string basePath = path.Substring(1, pos - 1);
				string sheetName = path.Substring(pos + 1);
				return "[" + CreateRelativePath(basePath) + "]" + sheetName;
			}
			return CreateRelativePath(path);
		}
		static string CreateRelativePath(string path) {
			return path.Replace(directorySeparatorChar, '\u0003');
		}
		static bool IsEnvironmentPath(string path, string envPath) {
			if(string.IsNullOrEmpty(envPath))
				return false;
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				return path.Substring(1, pos - 1).StartsWith(envPath, StringComparison.CurrentCultureIgnoreCase);
			}
			return path.StartsWith(envPath, StringComparison.CurrentCultureIgnoreCase);
		}
		static string CreateEnvironmentPath(string path, string envPath) {
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string basePath = path.Substring(1, pos - 1);
				string sheetName = path.Substring(pos + 1);
				return "[" + CreateEnvironmentBasePath(basePath, envPath) + "]" + sheetName;
			}
			return CreateEnvironmentBasePath(path, envPath);
		}
		static string CreateEnvironmentBasePath(string path, string envPath) {
			string filePath = path.Substring(envPath.Length);
			if(filePath[0] == directorySeparatorChar)
				filePath = filePath.Substring(1);
			return CreateFilePath(filePath);
		}
		static bool IsRelativeVolumePath(string path, string documentPath) {
			if(string.IsNullOrEmpty(path))
				return false;
			string driveVolume = GetDriveVolume(documentPath);
			if(string.IsNullOrEmpty(driveVolume))
				return false;
			string documentDir = GetDirectoryName(documentPath);
			string fileDir;
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string basePath = path.Substring(1, pos - 1);
				try {
					fileDir = GetDirectoryName(basePath);
					if(!string.IsNullOrEmpty(fileDir) && fileDir.StartsWith(documentDir, StringComparison.CurrentCultureIgnoreCase))
						return false;
				}
				catch {
					return false;
				}
				return basePath.StartsWith(driveVolume, StringComparison.CurrentCultureIgnoreCase);
			}
			try {
				fileDir = GetDirectoryName(path);
				if(!string.IsNullOrEmpty(fileDir) && fileDir.StartsWith(documentDir, StringComparison.CurrentCultureIgnoreCase))
					return false;
			}
			catch {
				return false;
			}
			return path.StartsWith(driveVolume, StringComparison.CurrentCultureIgnoreCase);
		}
		static string CreateRelativeVolumePath(string path, string documentPath) {
			string driveVolume = GetDriveVolume(documentPath);
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string basePath = path.Substring(1, pos - 1);
				string sheetName = path.Substring(pos + 1);
				return "[" + CreateEnvironmentBasePath(basePath, driveVolume) + "]" + sheetName;
			}
			return CreateEnvironmentBasePath(path, driveVolume);
		}
		static string CreateSimpleFilePath(string path, string documentPath) {
			if(string.IsNullOrEmpty(path))
				return string.Empty;
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string basePath = path.Substring(1, pos - 1);
				string sheetName = path.Substring(pos + 1);
				return "[" + CreateSimpleBasePath(basePath, documentPath) + "]" + sheetName;
			}
			return CreateSimpleBasePath(path, documentPath);
		}
		static string CreateSimpleBasePath(string path, string documentPath) {
			if(string.IsNullOrEmpty(documentPath))
				return CreateFilePath(path);
			try {
				string fileDir = GetDirectoryName(path);
				string documentDir = GetDirectoryName(documentPath);
				if(string.IsNullOrEmpty(fileDir) || string.IsNullOrEmpty(documentDir))
					return CreateFilePath(path);
				if(!fileDir.StartsWith(documentDir, StringComparison.CurrentCultureIgnoreCase))
					return CreateFilePath(path);
				return CreateEnvironmentBasePath(path, documentDir);
			}
			catch {
				return CreateFilePath(path);
			}
		}
		static bool IsVolumePath(string path, string documentPath) {
			if(string.IsNullOrEmpty(path))
				return false;
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string basePath = path.Substring(1, pos - 1);
				if(!string.IsNullOrEmpty(documentPath)) {
					string documentDir = GetDirectoryName(documentPath);
					try {
						string fileDir = GetDirectoryName(basePath);
						if(!string.IsNullOrEmpty(fileDir) && fileDir.StartsWith(documentDir, StringComparison.CurrentCultureIgnoreCase))
							return false;
					}
					catch {
						return false;
					}
				}
				return !string.IsNullOrEmpty(GetDriveVolume(basePath));
			}
			if(!string.IsNullOrEmpty(documentPath)) {
				string documentDir = GetDirectoryName(documentPath);
				try {
					string fileDir = GetDirectoryName(path);
					if(!string.IsNullOrEmpty(fileDir) && fileDir.StartsWith(documentDir, StringComparison.CurrentCultureIgnoreCase))
						return false;
				}
				catch {
					return false;
				}
			}
			return !string.IsNullOrEmpty(GetDriveVolume(path));
		}
		static string CreateVolumePath(string path) {
			string driveVolume;
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string basePath = path.Substring(1, pos - 1);
				string sheetName = path.Substring(pos + 1);
				driveVolume = GetDriveVolume(basePath);
				return driveVolume.Substring(0, 1) + "[" + CreateEnvironmentBasePath(basePath, driveVolume) + "]" + sheetName;
			}
			driveVolume = GetDriveVolume(path);
			return driveVolume.Substring(0, 1) + CreateEnvironmentBasePath(path, driveVolume);
		}
		static bool IsUNCVolumePath(string path) {
			if(string.IsNullOrEmpty(path))
				return false;
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string basePath = path.Substring(1, pos - 1);
				return IsUNCVolumeBasePath(basePath);
			}
			return IsUNCVolumeBasePath(path);
		}
		static bool IsUNCVolumeBasePath(string path) {
			if(string.IsNullOrEmpty(path))
				return false;
			if(!IsPathRooted(path)) return false;
			return path.IndexOf(@"\\") == 0;
		}
		static string CreateUNCVolumePath(string path) {
			if(path[0] == '[') {
				int pos = path.LastIndexOf(']');
				string basePath = path.Substring(1, pos - 1);
				string sheetName = path.Substring(pos + 1);
				return "[" + CreateFilePath(basePath.Substring(2)) + "]" + sheetName;
			}
			return CreateFilePath(path.Substring(2));
		}
		#endregion
		#endregion
	}
	#endregion
	#region XlsCommandExternCacheStart
	public class XlsCommandExternCacheStart : XlsCommandBase {
		#region Fields
		int itemsCount;
		int sheetIndex;
		#endregion
		#region Properties
		public bool IsLinkValid { get { return itemsCount >= 0; } }
		public int Count {
			get { return Math.Abs(itemsCount); }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "Count");
				itemsCount = value;
			}
		}
		public int SheetIndex {
			get { return sheetIndex; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "SheetIndex");
				sheetIndex = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			itemsCount = reader.ReadInt16();
			if(Size > 2)
				sheetIndex = reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)itemsCount);
			writer.Write((ushort)sheetIndex);
		}
		protected override short GetSize() {
			return 4;
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			ExternalWorksheet worksheet = contentBuilder.GetCurrentExternalWorkSheet();
			if (worksheet != null && !IsLinkValid)
				worksheet.RefreshFailed = true;
			contentBuilder.ExternSheetIndex = SheetIndex;
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandExternCacheStart();
		}
	}
	#endregion
	#region XlsExternCacheData
	public class XlsExternCacheData {
		#region Fields
		int row;
		int firstColumn;
		readonly List<IPtgExtraArrayValue> values = new List<IPtgExtraArrayValue>();
		#endregion
		#region Properties
		public int Row {
			get { return row; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "Row");
				row = value;
			}
		}
		public int FirstColumn {
			get { return firstColumn; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "FirstColumn");
				firstColumn = value;
			}
		}
		public int LastColumn { get { return firstColumn + values.Count - 1; } }
		public List<IPtgExtraArrayValue> Values { get { return values; } }
		public bool IsValidColumnRange { get { return LastColumn >= FirstColumn; } }
		#endregion
		public static XlsExternCacheData FromStream(BinaryReader reader) {
			XlsExternCacheData result = new XlsExternCacheData();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			int lastColumn = reader.ReadByte();
			this.firstColumn = reader.ReadByte();
			if(firstColumn > lastColumn)
				throw new Exception(string.Format("External data cache has wrong column range {0}...{1}!", firstColumn, lastColumn));
			this.row = reader.ReadUInt16();
			int count = lastColumn - firstColumn + 1;
			for(int i = 0; i < count; i++) {
				IPtgExtraArrayValue item = PtgExtraArrayFactory.CreateArrayValue(reader);
				item.Read(reader);
				this.values.Add(item);
			}
		}
		public void Write(BinaryWriter writer) {
			writer.Write((byte)LastColumn);
			writer.Write((byte)FirstColumn);
			writer.Write((ushort)Row);
			int count = Values.Count;
			for(int i = 0; i < count; i++)
				Values[i].Write(writer);
		}
		public int GetSize() {
			return GetValuesSize() + 4;
		}
		int GetValuesSize() {
			int result = 0;
			foreach(IPtgExtraArrayValue item in Values)
				result += item.GetSize();
			return result;
		}
	}
	#endregion
	#region XlsCommandExternCacheItem
	public class XlsCommandExternCacheItem : XlsCommandRecordBase {
		#region Fields
		short[] typeCodes = new short[] {
			0x003c 
		};
		XlsExternCacheData cacheData = new XlsExternCacheData();
		#endregion
		public XlsExternCacheData CacheData { get { return cacheData; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if(contentBuilder.ContentType == XlsContentType.WorkbookGlobals) {
				using(XlsCommandStream commandStream = new XlsCommandStream(reader, typeCodes, Size)) {
					using(BinaryReader commandReader = new BinaryReader(commandStream)) {
						this.cacheData = XlsExternCacheData.FromStream(commandReader);
					}
				}
			}
			else
				base.ReadCore(reader, contentBuilder);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			ExternalWorksheet sheet = contentBuilder.GetCurrentExternalWorkSheet();
			if(sheet != null) {
				int row = CacheData.Row;
				int firstColumn = CacheData.FirstColumn;
				int count = CacheData.Values.Count;
				for(int i = 0; i < count; i++) {
					if(CacheData.Values[i].Value != VariantValue.Empty) {
						ExternalCell cell = sheet.Rows[row].Cells[firstColumn + i];
						cell.Value = CacheData.Values[i].Value;
					}
				}
			}
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsExternClipboardFormat
	public enum XlsExternClipboardFormat {
		Text = 0,
		EnhancedMetafile = 2,
		CSV = 5,
		SymbolicLink = 6,
		RichText = 7,
		BIFF8 = 8,
		Bitmap = 9,
		StdDocumentName = 15,
		ExcelTable = 16,
		BIFF3 = 20,
		BIFF4 = 30,
		MetafilePicture = 36,
		UnicodeText = 44,
		BIFF12 = 63,
		None = 1023
	}
	#endregion
	#region XlsExternNameContent
	public abstract class XlsExternNameContent {
		ShortXLUnicodeString name = new ShortXLUnicodeString();
		public string Name { get { return name.Value; } set { name.Value = value; } }
		public abstract void Read(BinaryReader reader);
		public abstract void Write(BinaryWriter writer);
		protected void ReadName(BinaryReader reader) {
			this.name = ShortXLUnicodeString.FromStream(reader);
		}
		protected void WriteName(BinaryWriter writer) {
			this.name.Write(writer);
		}
	}
	#region XlsExternDocName
	public class XlsExternDocName : XlsExternNameContent {
		#region Fields
		int sheetIndex;
		byte[] formula = new byte[0];
		#endregion
		#region Properties
		public int SheetIndex {
			get { return sheetIndex; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "SheetIndex");
				sheetIndex = value;
			}
		}
		public byte[] Formula {
			get { return formula; }
			set {
				if(value == null)
					formula = new byte[0];
				else
					formula = value;
			}
		}
		#endregion
		public override void Read(BinaryReader reader) {
			SheetIndex = reader.ReadUInt16();
			reader.ReadUInt16(); 
			ReadName(reader);
			int count = reader.ReadUInt16();
			if(count > 0)
				formula = reader.ReadBytes(count);
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)SheetIndex);
			writer.Write((ushort)0); 
			WriteName(writer);
			int count = formula.Length;
			writer.Write((ushort)count);
			if(count > 0)
				writer.Write(formula);
		}
	}
	#endregion
	#region XlsExternOleDdeLink
	public class XlsExternOleDdeLink : XlsExternNameContent {
		#region Fields
		int lastColumn;
		int lastRow;
		readonly List<IPtgExtraArrayValue> values = new List<IPtgExtraArrayValue>();
		#endregion
		#region Properties
		public int Storage { get; set; }
		public string StorageName { get { return string.Format("LNK{0}", Storage.ToString("X8")); } }
		public int LastRow {
			get { return lastRow; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "LastRow");
				lastRow = value;
			}
		}
		public int LastColumn {
			get { return lastColumn; }
			set {
				ValueChecker.CheckValue(value, 0, byte.MaxValue, "LastColumn");
				lastColumn = value;
			}
		}
		public List<IPtgExtraArrayValue> Values { get { return values; } }
		#endregion
		public override void Read(BinaryReader reader) {
			Storage = reader.ReadInt32();
			ReadName(reader);
			if(reader.BaseStream.Position == reader.BaseStream.Length) return;
			LastColumn = reader.ReadByte();
			LastRow = reader.ReadUInt16();
			int count = (LastRow + 1) * (LastColumn + 1);
			for(int i = 0; i < count; i++) {
				IPtgExtraArrayValue item = PtgExtraArrayFactory.CreateArrayValue(reader);
				item.Read(reader);
				this.values.Add(item);
			}
		}
		public override void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			writer.Write(Storage);
			WriteName(writer);
			if(this.values.Count > 0) {
				writer.Write((byte)LastColumn);
				writer.Write((ushort)LastRow);
				int count = Values.Count;
				for(int i = 0; i < count; i++) {
					IPtgExtraArrayValue item = Values[i];
					if(chunkWriter != null) chunkWriter.BeginRecord(item.GetSize());
					item.Write(writer);
				}
			}
		}
	}
	#endregion
	#region XlsExternDdeLink
	public class XlsExternDdeLink : XlsExternNameContent {
		public override void Read(BinaryReader reader) {
			reader.ReadInt32(); 
			ReadName(reader);
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((int)0); 
			WriteName(writer);
		}
	}
	#endregion
	#region XlsAddInUdf
	public class XlsAddInUdf : XlsExternNameContent {
		public override void Read(BinaryReader reader) {
			reader.ReadInt32(); 
			ReadName(reader);
			int count = reader.ReadUInt16();
			reader.ReadBytes(count);
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((int)0); 
			WriteName(writer);
			writer.Write((ushort)0); 
		}
	}
	#endregion
	#endregion
	#region XlsExternNameInfo
	public class XlsExternNameInfo {
		XlsSupportingLinkType linkType = XlsSupportingLinkType.AddIn;
		XlsExternNameContent content = new XlsAddInUdf();
		#region Properties
		public XlsSupportingLinkType LinkType {
			get { return linkType; }
			set {
				linkType = value;
			}
		}
		public bool IsBuiltIn { get; set; }
		public bool WantAdvise { get; set; }
		public bool WantPictureFormat { get; set; }
		public bool IsOle { get; set; }
		public bool IsOleLink { get; set; }
		public XlsExternClipboardFormat ClipboardFormat { get; set; }
		public bool IsIconDisplayed { get; set; }
		public XlsExternNameContent Content {
			get { return content; }
			set {
				Guard.ArgumentNotNull(value, "Content");
				content = value;
			}
		}
		#endregion
		public static XlsExternNameInfo FromStream(BinaryReader reader, XlsSupportingLinkType linkType) {
			XlsExternNameInfo result = new XlsExternNameInfo();
			result.LinkType = linkType;
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			ushort bitwiseField = reader.ReadUInt16();
			IsBuiltIn = Convert.ToBoolean(bitwiseField & 0x0001);
			WantAdvise = Convert.ToBoolean(bitwiseField & 0x0002);
			WantPictureFormat = Convert.ToBoolean(bitwiseField & 0x0004);
			IsOle = Convert.ToBoolean(bitwiseField & 0x0008);
			IsOleLink = Convert.ToBoolean(bitwiseField & 0x0010);
			ClipboardFormat = (XlsExternClipboardFormat)((bitwiseField & 0x7fe0) >> 5);
			IsIconDisplayed = Convert.ToBoolean(bitwiseField & 0x8000);
			if(LinkType == XlsSupportingLinkType.AddIn)
				this.content = new XlsAddInUdf();
			else if(LinkType == XlsSupportingLinkType.DataSource) {
				if(IsOle)
					this.content = new XlsExternDdeLink();
				else
					this.content = new XlsExternOleDdeLink();
			}
			else 
				this.content = new XlsExternDocName();
			this.content.Read(reader);
		}
		public void Write(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(IsBuiltIn)
				bitwiseField |= 0x0001;
			if(WantAdvise)
				bitwiseField |= 0x0002;
			if(WantPictureFormat)
				bitwiseField |= 0x0004;
			if(IsOle)
				bitwiseField |= 0x0008;
			if(IsOleLink)
				bitwiseField |= 0x0010;
			bitwiseField |= (ushort)((ushort)ClipboardFormat << 5);
			if(IsIconDisplayed)
				bitwiseField |= 0x8000;
			writer.Write(bitwiseField);
			this.content.Write(writer);
		}
	}
	#endregion
	#region XlsCommandExternName
	public class XlsCommandExternName : XlsCommandRecordBase {
		#region Fields
		short[] typeCodes = new short[] {
			0x003c 
		};
		XlsExternNameInfo info = new XlsExternNameInfo();
		#endregion
		public XlsExternNameInfo Info { get { return info; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if(contentBuilder.ContentType == XlsContentType.WorkbookGlobals) {
				XlsSupBookInfo supBook = contentBuilder.GetLastSupBook();
				if(supBook == null)
					contentBuilder.ThrowInvalidFile("ExternName record without SupBook record");
				using(XlsCommandStream commandStream = new XlsCommandStream(reader, typeCodes, Size)) {
					using(BinaryReader commandReader = new BinaryReader(commandStream)) {
						this.info = XlsExternNameInfo.FromStream(commandReader, supBook.LinkType);
					}
				}
			}
			else
				base.ReadCore(reader, contentBuilder);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			XlsSupBookInfo supBook = contentBuilder.GetLastSupBook();
			string name = Info.Content.Name;
			int scope = XlsDefs.NoScope;
			if(Info.LinkType == XlsSupportingLinkType.ExternalWorkbook) {
				XlsExternDocName content = Info.Content as XlsExternDocName;
				if(content != null) {
					contentBuilder.RPNContext.WorkbookContext.PushCurrentWorkbook(contentBuilder.GetCurrentExternalWorkbook());
					try {
						ParsedExpression formula = contentBuilder.RPNContext.ExtNameBinaryToExpression(content.Formula);
						if(formula.Count == 0)
							formula.Add(new ParsedThingError(VariantValue.ErrorReference.ErrorValue));
						name = contentBuilder.RegisterExternalDefinedName(name, content.SheetIndex, formula);
						if(content.SheetIndex > 0)
							scope = content.SheetIndex - 1;
					}
					finally {
						contentBuilder.RPNContext.WorkbookContext.PopCurrentWorkbook();
					}
				}
			}
			else if(Info.LinkType == XlsSupportingLinkType.DataSource) {
				DdeExternalWorkbook connection = contentBuilder.GetCurrentDdeExternalWorkbook();
				if(connection != null) {
					DdeExternalWorksheet item = new DdeExternalWorksheet(connection, Info.Content.Name);
					item.Advise = Info.WantAdvise;
					item.IsDataImage = Info.WantPictureFormat;
					item.IsUsesOLE = Info.IsOle;
					item.IsOleLink = Info.IsOleLink;
					connection.Sheets.Add(item);
					if(!Info.IsOle) {
						XlsExternOleDdeLink content = Info.Content as XlsExternOleDdeLink;
						if(content.Values.Count > 0) {
							item.ColumnCount = content.LastColumn + 1;
							item.RowCount = content.LastRow + 1;
							for(int i = 0; i < content.Values.Count; i++) {
								if(content.Values[i].Value != VariantValue.Empty) {
									int rowIndex = i / item.ColumnCount;
									int columnIndex = i % item.ColumnCount;
									ExternalCell cell = item.Rows[rowIndex].Cells[columnIndex];
									cell.Value = content.Values[i].Value;
								}
							}
						}
					}
				}
			}
			else if(Info.LinkType == XlsSupportingLinkType.AddIn) {
			}
			else { 
			}
			supBook.RegisterExternalName(name, scope);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandExternName();
		}
	}
	#endregion
}
