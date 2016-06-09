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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocProtectionInfo
	public class DocProtectionInfo {
		#region static
		public static DocProtectionInfo FromStream(BinaryReader reader) {
			DocProtectionInfo result = new DocProtectionInfo();
			result.Read(reader);
			return result;
		}
		#endregion
		const int ignoredDataSize = 4;
		ushort uid;
		protected DocProtectionInfo() { }
		public DocProtectionInfo(int uid, DocumentProtectionType protectionType) {
			this.uid = (ushort)uid;
			ProtectionType = protectionType;
		}
		protected internal int Uid { get { return uid; } set { uid = (ushort)value; } } 
		public DocumentProtectionType ProtectionType { get; protected internal set; }
		protected internal virtual void Read(BinaryReader reader) {
			reader.BaseStream.Seek(ignoredDataSize, SeekOrigin.Current);
			uid = reader.ReadUInt16();
			ProtectionType = DocumentProtectionTypeCalculator.CalcRangePermissionProtectionType(reader.ReadInt16());
			reader.BaseStream.Seek(ignoredDataSize, SeekOrigin.Current);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(uid);
			writer.Write(DocumentProtectionTypeCalculator.CalcRangePermissionProtectionTypeCode(ProtectionType));
			writer.BaseStream.Seek(ignoredDataSize, SeekOrigin.Current);
		}
	}
	#endregion
	#region DocRangeEditPermissionIterator
	public class DocRangeEditPermissionIterator : DocBookmarkIteratorBase {
		#region Fields
		static readonly string defaultGroupName = "everyone";
		const int bookmarkIndexSize = 4;
		RangeEditPermissionsStringTable protectionInfos;
		UserProtectionStringTable users;
		List<DocImportRangePermissionInfo> permissions;
		Dictionary<int, int> permissionStartPositions;
		#endregion
		public DocRangeEditPermissionIterator() {
			permissionStartPositions = new Dictionary<int, int>();
			protectionInfos = new RangeEditPermissionsStringTable();
			users = new UserProtectionStringTable();
		}
		public DocRangeEditPermissionIterator(FileInformationBlock fib, BinaryReader reader)
			: base(fib, reader) {
		}
		public void InsertRangeEditPermissions(PieceTable pieceTable) {
			int count = this.permissions.Count;
			List<DocImportRangePermissionInfo> processedPermissions = new List<DocImportRangePermissionInfo>();
			DocumentLogPosition start;
			DocumentLogPosition end;
			for (int i = 0; i < count; i++) {
				bool startPositionObtainable = PositionConverter.TryConvert(permissions[i].OriginalStartPosition, out start);
				bool endPositionObtainable = PositionConverter.TryConvert(permissions[i].OriginalEndPosition, out end);
				if (startPositionObtainable && endPositionObtainable && permissions[i].Validate()) {
					pieceTable.ApplyDocumentPermission(start, end, permissions[i].PermissionInfo);
					processedPermissions.Add(permissions[i]);
				}
			}
		}
		void RemoveProcessedPermissions(List<DocImportRangePermissionInfo> processedPermissions) {
			int count = processedPermissions.Count;
			for (int i = 0; i < count; i++)
				this.permissions.Remove(processedPermissions[i]);
			PositionConverter.Clear();
		}
		protected override void Read(FileInformationBlock fib, BinaryReader reader) {
			this.protectionInfos = RangeEditPermissionsStringTable.FromStream(reader, fib.RangeEditPermissionsInformationOffset, fib.RangeEditPermissionsInformationSize);
			FirstTable = DocBookmarkFirstTable.FromStream(reader, fib.RangeEditPermissionsStartInfoOffset, fib.RangeEditPermissionsStartInfoSize, bookmarkIndexSize);
			LimTable = DocBookmarkLimTable.FromStream(reader, fib.RangeEditPermissionsEndInfoOffset, fib.RangeEditPermissionsEndInfoSize);
			this.users = UserProtectionStringTable.FromStream(reader, fib.RangeEditPermissionsUsernamesOffset, fib.RangeEditPermissionsUsernamesSize);
			InitRangeEditPermissions();
			InitConverter();
		}
		void InitRangeEditPermissions() {
			int count = protectionInfos.Count;
			this.permissions = new List<DocImportRangePermissionInfo>();
			for (int i = 0; i < count; i++) {
				if (FirstTable.BookmarkFirstDescriptors[i].Column)
					continue;
				List<string> grantedUsers = users.GetUsersById(protectionInfos.ProtectionInfos[i].Uid);
				int start  = FirstTable.CharacterPositions[i];
				int end = LimTable.CharacterPositions[i];
				this.permissions.AddRange(CreatePermissionInfos(start, end, protectionInfos.ProtectionInfos[i].ProtectionType, grantedUsers));
			}
		}
		List<DocImportRangePermissionInfo> CreatePermissionInfos(int start, int end, DocumentProtectionType type, List<string> users) {
			if (users == null) {
				DocImportRangePermissionInfo info = new DocImportRangePermissionInfo(start, end);
				info.PermissionInfo.Group = defaultGroupName;
				info.ProtectionType = type;
				return new List<DocImportRangePermissionInfo>() { info };
			}
			List<DocImportRangePermissionInfo> result = new List<DocImportRangePermissionInfo>();
			int count = users.Count;
			for (int i = 0; i < count; i++) {
				DocImportRangePermissionInfo info = new DocImportRangePermissionInfo(start, end);
				info.PermissionInfo.UserName = users[i];
				info.ProtectionType = type;
			}
			return result;
		}
		public override void Write(FileInformationBlock fib, BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			fib.RangeEditPermissionsInformationOffset = (int)writer.BaseStream.Position;
			this.protectionInfos.Write(writer);
			fib.RangeEditPermissionsInformationSize = (int)(writer.BaseStream.Position - fib.RangeEditPermissionsInformationOffset);
			fib.RangeEditPermissionsStartInfoOffset = (int)writer.BaseStream.Position;
			FirstTable.Write(writer, bookmarkIndexSize);
			fib.RangeEditPermissionsStartInfoSize = (int)(writer.BaseStream.Position - fib.RangeEditPermissionsStartInfoOffset);
			fib.RangeEditPermissionsEndInfoOffset = (int)writer.BaseStream.Position;
			LimTable.Write(writer);
			fib.RangeEditPermissionsEndInfoSize = (int)(writer.BaseStream.Position - fib.RangeEditPermissionsEndInfoOffset);
			fib.RangeEditPermissionsUsernamesOffset = (int)writer.BaseStream.Position;
			this.users.Write(writer);
			fib.RangeEditPermissionsUsernamesSize = (int)(writer.BaseStream.Position - fib.RangeEditPermissionsUsernamesOffset);
		}
		public void AddPermissionStart(RangePermission permission, int startPosition) {
			int id = permission.GetHashCode();
			if (!this.permissionStartPositions.ContainsKey(id))
				this.permissionStartPositions.Add(id, startPosition);
		}
		public void AddPermissionEnd(RangePermission permission, int endPosition) {
			int id = permission.GetHashCode();
			int startPosition;
			if (!this.permissionStartPositions.TryGetValue(id, out startPosition))
				return;
			int userId = users.GetUserIdByName(permission.UserName);
			protectionInfos.ProtectionInfos.Add(new DocProtectionInfo(userId, DocumentProtectionType.None));
			FirstTable.AddEntry(startPosition, new DocBookmarkFirstDescriptor(CurrentIndex));
			LimTable.CharacterPositions.Add(endPosition);
			CurrentIndex++;
		}
	}
	#endregion
	#region DocUserRole
	public enum DocUserRole {
		None = 0x0000,
		Owner = 0xfffc,
		Editor = 0xfffb
	}
	#endregion
	#region DocImportRangePermissionInfo
	public class DocImportRangePermissionInfo {
		#region Fields
		RangePermissionInfo permissionInfo = new RangePermissionInfo();
		int originalStartPosition;
		int originalEndPosition;
		#endregion
		public DocImportRangePermissionInfo(int originalStartPosition, int originalEndPosition) {
			this.originalStartPosition = originalStartPosition;
			this.originalEndPosition = originalEndPosition;
		}
		#region Properties
		public int OriginalStartPosition { get { return this.originalStartPosition; } }
		public int OriginalEndPosition { get { return this.originalEndPosition; } } 
		public RangePermissionInfo PermissionInfo { get { return permissionInfo; } }
		public DocumentProtectionType ProtectionType { get; set; }
		#endregion
		public bool Validate() {
			if (String.IsNullOrEmpty(PermissionInfo.UserName) && String.IsNullOrEmpty(PermissionInfo.Group))
				return false;
			return OriginalStartPosition <= OriginalEndPosition;
		}
	}
	#endregion
	#region RangeEditPermissionsStringTable
	public class RangeEditPermissionsStringTable : DocStringTableBase {
		#region static
		public static RangeEditPermissionsStringTable FromStream(BinaryReader reader, int offset, int size) {
			RangeEditPermissionsStringTable result = new RangeEditPermissionsStringTable();
			result.Read(reader, offset, size);
			return result;
		}
		#endregion
		const short protectionInfoSize = 0x8;
		public RangeEditPermissionsStringTable() {
			ProtectionInfos = new List<DocProtectionInfo>();
		}
		public List<DocProtectionInfo> ProtectionInfos { get; private set; }
		protected internal override int CalcExtraDataSize(BinaryReader reader) {
			if (reader.ReadInt16() != protectionInfoSize)
				DocImporter.ThrowInvalidDocFile();
			return protectionInfoSize;
		}
		protected internal override int CalcRecordsCount(BinaryReader reader) {
			return reader.ReadInt32();
		}
		protected internal override void Write(BinaryWriter writer) {
			Count = ProtectionInfos.Count;
			if (Count > 0)
				base.Write(writer);
		}
		protected override void WriteExtraDataSize(BinaryWriter writer) {
			writer.Write(protectionInfoSize);
		}
		protected override void WriteCount(BinaryWriter writer) {
			writer.Write(Count);
		}
		protected override void ReadString(BinaryReader reader) {
			reader.ReadInt16();
		}
		protected override void ReadExtraData(BinaryReader reader) {
			ProtectionInfos.Add(DocProtectionInfo.FromStream(reader));
		}
		protected override void WriteString(BinaryWriter writer, int index) {
			writer.Write((short)0x00);
		}
		protected override void WriteExtraData(BinaryWriter writer, int index) {
			ProtectionInfos[index].Write(writer);
		}
	}
	#endregion
	#region UserProtectionStringTable
	public class UserProtectionStringTable : DocStringTableBase {
		#region static
		public static UserProtectionStringTable FromStream(BinaryReader reader, int offset, int size) {
			UserProtectionStringTable result = new UserProtectionStringTable();
			result.Read(reader, offset, size);
			return result;
		}
		#endregion
		const short extraDataSize = 0x02;
		const int ownersId = 0xfffc;
		const int editorsId = 0xfffb;
		const int everyoneId = 0xffff;
		List<string> editors;
		List<string> owners;
		public UserProtectionStringTable() {
			Users = new List<string>();
			Roles = new List<DocUserRole>();
		}
		public List<string> Users { get; private set; }
		public List<DocUserRole> Roles { get; private set; }
		public List<string> GetUsersById(int id) {
			if (id == editorsId)
				return editors;
			if (id == ownersId)
				return owners;
			if (id > 0 && id <= Users.Count)
				return new List<string>() { Users[id - 1] };
			return null;
		}
		public int GetUserIdByName(string userName) {
			if (String.IsNullOrEmpty(userName))
				return everyoneId;
			int count = Users.Count;
			for (int i = 0; i < count; i++) {
				if (Users[i] == userName)
					return i + 1;
			}
			Users.Add(userName);
			Roles.Add(DocUserRole.None);
			return Users.Count;
		}
		protected internal override int CalcExtraDataSize(BinaryReader reader) {
			if (reader.ReadInt16() != extraDataSize)
				DocImporter.ThrowInvalidDocFile();
			return extraDataSize;
		}
		protected internal override int CalcRecordsCount(BinaryReader reader) {
			return reader.ReadInt16();
		}
		protected override void WriteExtraDataSize(BinaryWriter writer) {
			writer.Write(extraDataSize);
		}
		protected override void WriteCount(BinaryWriter writer) {
			writer.Write((short)Count);
		}
		protected internal override void Read(BinaryReader reader, int offset, int size) {
			base.Read(reader, offset, size);
			GroupUsersByRole();
		}
		protected override void ReadString(BinaryReader reader) {
			int length = reader.ReadInt16() * 2;
			byte[] buffer = reader.ReadBytes(length);
			Users.Add(StringHelper.RemoveSpecialSymbols(Encoding.GetString(buffer, 0, buffer.Length)));
		}
		protected override void ReadExtraData(BinaryReader reader) {
			Roles.Add((DocUserRole)reader.ReadUInt16());
		}
		void GroupUsersByRole() {
			Debug.Assert(Users.Count == Roles.Count);
			editors = new List<string>();
			owners = new List<string>();
			int count = Users.Count;
			for (int i = 0; i < count; i++) {
				if (Roles[i] == DocUserRole.Editor)
					editors.Add(Users[i]);
				else if (Roles[i] == DocUserRole.Owner)
					owners.Add(Users[i]);
			}
		}
		protected internal override void Write(BinaryWriter writer) {
			if (Users.Count == 0)
				return;
			Count = Users.Count;
			base.Write(writer);
		}
		protected override void WriteString(BinaryWriter writer, int index) {
			string user = Users[index];
			writer.Write((short)user.Length);
			writer.Write(GetEncoding().GetBytes(user));
		}
		protected override void WriteExtraData(BinaryWriter writer, int index) {
			writer.Write((ushort)Roles[index]);
		}
	}
	#endregion
}
