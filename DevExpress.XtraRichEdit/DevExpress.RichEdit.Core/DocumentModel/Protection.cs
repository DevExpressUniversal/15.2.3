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
using System.Security.Cryptography;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentProtectionType
	public enum DocumentProtectionType {
		None,
		ReadOnly,
	}
	#endregion
	#region HashAlgorithmType
	public enum HashAlgorithmType {
		None = 0,
		Md2 = 1,
		Md4 = 2,
		Md5 = 3,
		Sha1 = 4,
		Mac = 5,
		Ripemd = 6,
		Ripemd160 = 7,
		HMac = 9, 
		Sha256 = 12,
		Sha384 = 13,
		Sha512 = 14,
	}
	#endregion
	#region DocumentProtectionInfo
	public class DocumentProtectionInfo : ICloneable<DocumentProtectionInfo>, ISupportsCopyFrom<DocumentProtectionInfo>, ISupportsSizeOf {
		#region Fields
		bool enforceProtection;
		DocumentProtectionType protectionType;
		HashAlgorithmType hashAlgorithmType;
		int hashIterationCount;
		byte[] passwordHash;
		byte[] passwordPrefix;
		byte[] word2003PasswordHash;
		byte[] openOfficePasswordHash;
		#endregion
		#region Properties
		public bool EnforceProtection { get { return enforceProtection; } set { enforceProtection = value; } }
		public DocumentProtectionType ProtectionType { get { return protectionType; } set { protectionType = value; } }
		public HashAlgorithmType HashAlgorithmType { get { return hashAlgorithmType; } set { hashAlgorithmType = value; } }
		public int HashIterationCount { get { return hashIterationCount; } set { hashIterationCount = value; } }
		public byte[] PasswordHash { get { return passwordHash; } set { passwordHash = value; } }
		public byte[] PasswordPrefix { get { return passwordPrefix; } set { passwordPrefix = value; } }
		public byte[] Word2003PasswordHash { get { return word2003PasswordHash; } set { word2003PasswordHash = value; } }
		public byte[] OpenOfficePasswordHash { get { return openOfficePasswordHash; } set { openOfficePasswordHash = value; } }
		#endregion
		#region ICloneable<DocumentProtectionInfo> Members
		public DocumentProtectionInfo Clone() {
			DocumentProtectionInfo result = new DocumentProtectionInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		public override bool Equals(object obj) {
			DocumentProtectionInfo info = (DocumentProtectionInfo)obj;
			return
				this.EnforceProtection == info.EnforceProtection &&
				this.ProtectionType == info.ProtectionType &&
				this.HashAlgorithmType == info.HashAlgorithmType &&
				this.HashIterationCount == info.HashIterationCount &&
				PasswordHashCodeCalculator.CompareByteArrays(PasswordHash, info.PasswordHash) &&
				PasswordHashCodeCalculator.CompareByteArrays(PasswordPrefix, info.PasswordPrefix) &&
				PasswordHashCodeCalculator.CompareByteArrays(Word2003PasswordHash, info.Word2003PasswordHash) &&
				PasswordHashCodeCalculator.CompareByteArrays(OpenOfficePasswordHash, info.OpenOfficePasswordHash);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public void CopyFrom(DocumentProtectionInfo info) {
			this.EnforceProtection = info.EnforceProtection;
			this.ProtectionType = info.ProtectionType;
			this.HashAlgorithmType = info.HashAlgorithmType;
			this.HashIterationCount = info.HashIterationCount;
			this.PasswordHash = info.PasswordHash;
			this.PasswordPrefix = info.PasswordPrefix;
			this.Word2003PasswordHash = info.Word2003PasswordHash;
			this.OpenOfficePasswordHash = info.OpenOfficePasswordHash;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region DocumentProtectionInfoCache
	public class DocumentProtectionInfoCache : UniqueItemsCache<DocumentProtectionInfo> {
		public DocumentProtectionInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override DocumentProtectionInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DocumentProtectionInfo();
		}
	}
	#endregion
	#region DocumentProtectionProperties
	public class DocumentProtectionProperties : RichEditIndexBasedObject<DocumentProtectionInfo> {
		public DocumentProtectionProperties(DocumentModel documentModel)
			: base(GetMainPieceTable(documentModel)) {
		}
		static PieceTable GetMainPieceTable(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			return documentModel.MainPieceTable;
		}
		#region Properties
		#region EnforceProtection
		public bool EnforceProtection {
			get { return Info.EnforceProtection; }
			set {
				if (EnforceProtection == value)
					return;
				SetPropertyValue(SetEnforceProtectionCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetEnforceProtectionCore(DocumentProtectionInfo info, bool value) {
			info.EnforceProtection = value;
			return GetBatchUpdateChangeActions();
		}
		#endregion
		#region ProtectionType
		public DocumentProtectionType ProtectionType {
			get { return Info.ProtectionType; }
			set {
				if (ProtectionType == value)
					return;
				SetPropertyValue(SetProtectionTypeCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetProtectionTypeCore(DocumentProtectionInfo info, DocumentProtectionType value) {
			info.ProtectionType = value;
			return GetBatchUpdateChangeActions();
		}
		#endregion
		#region HashAlgorithmType
		public HashAlgorithmType HashAlgorithmType {
			get { return Info.HashAlgorithmType; }
			set {
				if (HashAlgorithmType == value)
					return;
				SetPropertyValue(SetHashAlgorithmTypeCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetHashAlgorithmTypeCore(DocumentProtectionInfo info, HashAlgorithmType value) {
			info.HashAlgorithmType = value;
			return GetBatchUpdateChangeActions();
		}
		#endregion
		#region HashIterationCount
		public int HashIterationCount {
			get { return Info.HashIterationCount; }
			set {
				if (HashIterationCount == value)
					return;
				SetPropertyValue(SetHashIterationCountCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetHashIterationCountCore(DocumentProtectionInfo info, int value) {
			info.HashIterationCount = value;
			return GetBatchUpdateChangeActions();
		}
		#endregion
		#region PasswordHash
		public byte[] PasswordHash {
			get { return Info.PasswordHash; }
			set {
				if (PasswordHashCodeCalculator.CompareByteArrays(PasswordHash, value))
					return;
				SetPropertyValue(SetPasswordHashCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetPasswordHashCore(DocumentProtectionInfo info, byte[] value) {
			info.PasswordHash = value;
			return GetBatchUpdateChangeActions();
		}
		#endregion
		#region PasswordPrefix
		public byte[] PasswordPrefix {
			get { return Info.PasswordPrefix; }
			set {
				if (PasswordHashCodeCalculator.CompareByteArrays(PasswordPrefix, value))
					return;
				SetPropertyValue(SetPasswordPrefixCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetPasswordPrefixCore(DocumentProtectionInfo info, byte[] value) {
			info.PasswordPrefix = value;
			return GetBatchUpdateChangeActions();
		}
		#endregion
		#region Word2003PasswordHash
		public byte[] Word2003PasswordHash {
			get { return Info.Word2003PasswordHash; }
			set {
				if (PasswordHashCodeCalculator.CompareByteArrays(Word2003PasswordHash, value))
					return;
				SetPropertyValue(SetWord2003PasswordHashCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetWord2003PasswordHashCore(DocumentProtectionInfo info, byte[] value) {
			info.Word2003PasswordHash = value;
			return GetBatchUpdateChangeActions();
		}
		#endregion
		#region OpenOfficePasswordHash
		public byte[] OpenOfficePasswordHash {
			get { return Info.OpenOfficePasswordHash; }
			set {
				if (PasswordHashCodeCalculator.CompareByteArrays(OpenOfficePasswordHash, value))
					return;
				SetPropertyValue(SetOpenOfficePasswordHashCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetOpenOfficePasswordHashCore(DocumentProtectionInfo info, byte[] value) {
			info.OpenOfficePasswordHash = value;
			return GetBatchUpdateChangeActions();
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<DocumentProtectionInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.DocumentProtectionInfoCache;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.RaiseModifiedChanged | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.RaiseDocumentProtectionChanged;
		}
	}
	#endregion
	#region PasswordHashCodeCalculator
	public class PasswordHashCodeCalculator {
		static readonly int[] initialValues = new int[15] { 0xE1F0, 0x1D0F, 0xCC9C, 0x84C0, 0x110C, 0x0E10, 0xF1CE, 0x313E, 0x1872, 0xE139, 0xD40F, 0x84F9, 0x280C, 0xA96A, 0x4EC3 };
		static readonly int[,] encryptionMatrix = new int[15, 7] {
			{ 0xAEFC, 0x4DD9, 0x9BB2, 0x2745, 0x4E8A, 0x9D14, 0x2A09 },
			{ 0x7B61, 0xF6C2, 0xFDA5, 0xEB6B, 0xC6F7, 0x9DCF, 0x2BBF },
			{ 0x4563, 0x8AC6, 0x05AD, 0x0B5A, 0x16B4, 0x2D68, 0x5AD0 },
			{ 0x0375, 0x06EA, 0x0DD4, 0x1BA8, 0x3750, 0x6EA0, 0xDD40 },
			{ 0xD849, 0xA0B3, 0x5147, 0xA28E, 0x553D, 0xAA7A, 0x44D5 },
			{ 0x6F45, 0xDE8A, 0xAD35, 0x4A4B, 0x9496, 0x390D, 0x721A },
			{ 0xEB23, 0xC667, 0x9CEF, 0x29FF, 0x53FE, 0xA7FC, 0x5FD9 },
			{ 0x47D3, 0x8FA6, 0x0F6D, 0x1EDA, 0x3DB4, 0x7B68, 0xF6D0 },
			{ 0xB861, 0x60E3, 0xC1C6, 0x93AD, 0x377B, 0x6EF6, 0xDDEC },
			{ 0x45A0, 0x8B40, 0x06A1, 0x0D42, 0x1A84, 0x3508, 0x6A10 },
			{ 0xAA51, 0x4483, 0x8906, 0x022D, 0x045A, 0x08B4, 0x1168 },
			{ 0x76B4, 0xED68, 0xCAF1, 0x85C3, 0x1BA7, 0x374E, 0x6E9C },
			{ 0x3730, 0x6E60, 0xDCC0, 0xA9A1, 0x4363, 0x86C6, 0x1DAD },
			{ 0x3331, 0x6662, 0xCCC4, 0x89A9, 0x0373, 0x06E6, 0x0DCC },
			{ 0x1021, 0x2042, 0x4084, 0x8108, 0x1231, 0x2462, 0x48C4 },
		};
		public byte[] CalculateLegacyPasswordHash(string password) {
			if (String.IsNullOrEmpty(password))
				return null;
			return BitConverter.GetBytes(CalculateLegacyPasswordHashInt(password));
		}
		public byte[] CalculateOpenOfficePasswordHash(string password) {
			using (HashAlgorithm hashAlgorithm = SHA1.Create()) {
				hashAlgorithm.Initialize();
				byte[] result = hashAlgorithm.ComputeHash(Encoding.Unicode.GetBytes(password));
				return result;
			}
		}
		protected internal int CalculateLegacyPasswordHashInt(string password) {
			if (String.IsNullOrEmpty(password))
				return 0;
			byte[] bytes = CalculatePasswordBytes(password);
			uint high = (uint)CalculateKeyHighWord(bytes);
			uint low = (uint)CalculateKeyLowWord(bytes);
			return (int)(((low << 24) & 0xFF000000) | ((low << 8) & 0x00FF0000) | ((high << 8) & 0x0000FF00) | ((high >> 8) & 0x000000FF));
		}
		public byte[] CalculatePasswordHash(string password, byte[] prefix, int hashCount, HashAlgorithmType hashAlgorithmType) {
			HashAlgorithm hashAlgorithm = CreateHashAlgorithm(hashAlgorithmType);
			if (hashAlgorithm != null) {
				using (hashAlgorithm) {
					hashAlgorithm.Initialize();
					return CalculatePasswordHash(password, prefix, hashCount, hashAlgorithm);
				}
			}
			else {
				int legacyPasswordHash = CalculateLegacyPasswordHashInt(password);
				return Concatenate(new byte[] { }, legacyPasswordHash);
			}
		}
		public byte[] GeneratePasswordPrefix(int length) {
			byte[] result = new byte[length];
			RandomNumberGenerator provider = RandomNumberGenerator.Create();
#if !SL && !DXPORTABLE
			provider.GetNonZeroBytes(result);
#else
			provider.GetBytes(result);
#endif
			return result;
		}
#if DXPORTABLE
		protected internal virtual HashAlgorithm CreateHashAlgorithm(HashAlgorithmType hashAlgorithmType) {
			switch (hashAlgorithmType) {
				case HashAlgorithmType.None:
				case HashAlgorithmType.Mac:
				case HashAlgorithmType.HMac:
				case HashAlgorithmType.Ripemd:
				case HashAlgorithmType.Md2:
				case HashAlgorithmType.Md4:
				default:
					return null;
				case HashAlgorithmType.Sha1:
					return SHA1.Create();
				case HashAlgorithmType.Sha256:
					return SHA256.Create();
				case HashAlgorithmType.Sha384:
					return SHA384.Create();
				case HashAlgorithmType.Sha512:
					return SHA512.Create();
				case HashAlgorithmType.Md5:
					return MD5.Create();
			}
		}
#else
		protected internal virtual HashAlgorithm CreateHashAlgorithm(HashAlgorithmType hashAlgorithmType) {
			switch (hashAlgorithmType) {
				case HashAlgorithmType.None:
				case HashAlgorithmType.Mac:
				case HashAlgorithmType.HMac:
				case HashAlgorithmType.Ripemd:
				case HashAlgorithmType.Md2:
				case HashAlgorithmType.Md4:
				default:
					return null;
				case HashAlgorithmType.Sha1:
					return new SHA1Managed();
				case HashAlgorithmType.Sha256:
					return new SHA256Managed();
#if !SL
				case HashAlgorithmType.Sha384:
					return new SHA384Managed();
				case HashAlgorithmType.Sha512:
					return new SHA512Managed();
				case HashAlgorithmType.Md5:
					return new MD5CryptoServiceProvider();
				case HashAlgorithmType.Ripemd160:
					return new RIPEMD160Managed();
#endif
			}
		}
#endif
		public byte[] CalculatePasswordHash(string password, byte[] prefix, int hashCount, HashAlgorithm hashAlgorithm) {
			int legacyPasswordHash = CalculateLegacyPasswordHashInt(password);
			return CalculatePasswordHashCore(Encoding.Unicode.GetBytes(String.Format("{0:X8}", legacyPasswordHash)), prefix, hashCount, hashAlgorithm);
		}
		byte[] CalculatePasswordHashCore(byte[] legacyPasswordHash, byte[] prefix, int hashCount, HashAlgorithm hashAlgorithm) {
			byte[] bytes = Concatenate(prefix, legacyPasswordHash);
			int i = 0;
			for (; ; ) {
				bytes = hashAlgorithm.ComputeHash(bytes);
				if (i < hashCount) {
					bytes = Concatenate(bytes, i);
					i++;
				}
				else
					return bytes;
			}
		}
		byte[] Concatenate(byte[] b1, byte[] b2) {
			if (b1 == null)
				return b2;
			if (b2 == null)
				return b1;
			byte[] result = new byte[b1.Length + b2.Length];
			Array.Copy(b1, 0, result, 0, b1.Length);
			Array.Copy(b2, 0, result, b1.Length, b2.Length);
			return result;
		}
		byte[] Concatenate(byte[] bytes, int num) {
			byte[] result = new byte[bytes.Length + 4];
			byte[] countBytes = new byte[4];
			countBytes[3] = (byte)((num & 0xFF000000) >> 24);
			countBytes[2] = (byte)((num & 0x00FF0000) >> 16);
			countBytes[1] = (byte)((num & 0x0000FF00) >> 8);
			countBytes[0] = (byte)((num & 0x000000FF));
			Array.Copy(bytes, 0, result, 0, bytes.Length);
			Array.Copy(countBytes, 0, result, bytes.Length, countBytes.Length);
			return result;
		}
		public static bool CompareByteArrays(byte[] b1, byte[] b2) {
			if (Object.ReferenceEquals(b1, b2))
				return true;
			if (b1 == null || b2 == null)
				return false;
			if (b1.Length != b2.Length)
				return false;
			int count = b1.Length;
			for (int i = 0; i < count; i++)
				if (b1[i] != b2[i])
					return false;
			return true;
		}
		protected internal byte[] CalculatePasswordBytes(string password) {
			if (password.Length > 15)
				password = password.Substring(0, 15);
			int count = password.Length;
			byte[] bytes = new byte[count];
			for (int i = 0; i < count; i++) {
				int ch = (int)password[i];
				if ((ch & 0x00FF) == 0)
					bytes[i] = (byte)(ch >> 8);
				else
					bytes[i] = (byte)(ch & 0x00FF);
			}
			return bytes;
		}
		protected internal int CalculateKeyLowWord(byte[] bytes) {
			int result = 0;
			int count = bytes.Length;
			for (int i = count - 1; i >= 0; i--)
				result = ProcessLowWordByte(result, bytes[i]);
			result = ProcessLowWordByte(result, (byte)count) ^ 0xCE4B;
			return result;
		}
		protected internal int ProcessLowWordByte(int key, byte b) {
			return ((((key >> 14) & 0x0001) | ((key << 1) & 0x7FFF)) ^ b);
		}
		protected internal int CalculateKeyHighWord(byte[] bytes) {
			int count = bytes.Length;
			int result = initialValues[count - 1];
			for (int i = 0; i < count; i++)
				result = ProcessHighWordByte(result, bytes[i], 15 - (count - i));
			return result;
		}
		protected internal int ProcessHighWordByte(int key, byte b, int rowIndex) {
			int mask = 1;
			for (int i = 0; i <= 6; i++, mask <<= 1)
				if ((b & mask) != 0)
					key ^= encryptionMatrix[rowIndex, i];
			return key;
		}
	}
#endregion
#region RangePermissionInfo
	public class RangePermissionInfo : ICloneable<RangePermissionInfo>, ISupportsCopyFrom<RangePermissionInfo>, ISupportsSizeOf {
		string userName = String.Empty;
		string group = String.Empty;
		public string UserName { get { return userName; } set { userName = value; } }
		public string Group { get { return group; } set { group = value; } }
#region ICloneable<RangePermissionInfo> Members
		public RangePermissionInfo Clone() {
			RangePermissionInfo result = new RangePermissionInfo();
			result.CopyFrom(this);
			return result;
		}
#endregion
#region ISupportsCopyFrom<RangePermissionInfo> Members
		public void CopyFrom(RangePermissionInfo value) {
			this.UserName = value.UserName;
			this.Group = value.Group;
		}
#endregion
#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, false);
		}
#endregion
		public override bool Equals(object obj) {
			RangePermissionInfo info = (RangePermissionInfo)obj;
			return info.UserName == UserName && info.Group == Group;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
#endregion
#region RangePermissionProperties
	public class RangePermissionProperties : RichEditIndexBasedObject<RangePermissionInfo> {
		public RangePermissionProperties(PieceTable pieceTable)
			: base(pieceTable) {
		}
#region Properties
#region UserName
		public string UserName {
			get { return Info.UserName; }
			set {
				if (UserName == value)
					return;
				SetPropertyValue(SetUserNameCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetUserNameCore(RangePermissionInfo info, string value) {
			info.UserName = value;
			return GetBatchUpdateChangeActions();
		}
#endregion
#region Group
		public string Group {
			get { return Info.Group; }
			set {
				if (Group == value)
					return;
				SetPropertyValue(SetGroupCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetGroupCore(RangePermissionInfo info, string value) {
			info.Group = value;
			return GetBatchUpdateChangeActions();
		}
#endregion
#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.RaiseModifiedChanged | DocumentModelChangeActions.ResetSecondaryLayout;
		}
		protected internal override UniqueItemsCache<RangePermissionInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.RangePermissionInfoCache;
		}
	}
#endregion
#region RangePermissionInfoCache
	public class RangePermissionInfoCache : UniqueItemsCache<RangePermissionInfo> {
		public RangePermissionInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override RangePermissionInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new RangePermissionInfo();
		}
	}
#endregion
#region RangePermission
	public class RangePermission : BookmarkBase {
#region Fields
		readonly RangePermissionProperties properties;
#endregion
		public RangePermission(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end)
			: base(pieceTable, start, end) {
			this.properties = new RangePermissionProperties(PieceTable);
		}
		RangePermission(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end, int propertiesIndex)
			: this(pieceTable, start, end) {
			this.Properties.SetIndexInitial(propertiesIndex);
		}
#region Properties
		public string UserName { get { return properties.UserName; } set { properties.UserName = value; } }
		public string Group { get { return properties.Group; } set { properties.Group = value; } }
		protected internal RangePermissionProperties Properties { get { return properties; } }
#endregion
		public override void Visit(IDocumentIntervalVisitor visitor) {
			visitor.Visit(this);
		}
		public RangePermissionCollection Subtract(RangePermission interval) {
			RangePermissionCollectionEx result = new RangePermissionCollectionEx(PieceTable);
			if (interval == null || !IntersectsWithExcludingBounds(interval)) {
				result.Add(this);
				return result;
			}
			if (interval.Contains(this))
				return result;
			if (this.Contains(interval)) {
				if (Start < interval.Start)
				result.Add(new RangePermission(PieceTable, Start, interval.Start, Properties.Index));
				if (End > interval.End)
				result.Add(new RangePermission(PieceTable, interval.End, End, Properties.Index));
				return result;
			}
			if (Start >= interval.Start)
				result.Add(new RangePermission(PieceTable, interval.End, End, Properties.Index));
			else
				result.Add(new RangePermission(PieceTable, Start, interval.Start, Properties.Index));
			return result;
		}
		public static RangePermission Union(RangePermission interval1, RangePermission interval2) {
			DocumentLogPosition start = Algorithms.Min(interval1.Start, interval2.Start);
			DocumentLogPosition end = Algorithms.Max(interval1.End, interval2.End);
			return new RangePermission(interval1.PieceTable, start, end, interval1.Properties.Index);
		}
		protected internal override void Delete(int index) {
			PieceTable.DeleteRangePermission(this);
		}
		protected internal bool OnRunInsertedStart(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			DocumentModelPosition pos = Interval.Start;
			DocumentModelPositionAnchor anchor = new DocumentModelPositionAnchor(pos);
			anchor.OnRunInserted(PieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
			OnChanged(anchor.PositionChanged, false);
			return anchor.PositionChanged;
		}
		protected internal bool OnRunMergedStart(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length) {
			DocumentModelPosition pos = Interval.Start;
			DocumentModelPositionAnchor anchor = new DocumentModelPositionAnchor(pos);
			anchor.OnRunMerged(PieceTable, paragraphIndex, newRunIndex, length);
			OnChanged(anchor.PositionChanged, false);
			return anchor.PositionChanged;
		}
		protected internal bool OnRunInsertedEnd(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			DocumentModelPosition pos = Interval.End;
			DocumentModelPositionAnchor anchor = new DocumentModelPositionAnchor(pos);
			anchor.OnRunInserted(PieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
			OnChanged(false, anchor.PositionChanged);
			return anchor.PositionChanged;
		}
		protected internal bool OnRunMergedEnd(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length) {
			DocumentModelPosition pos = Interval.End;
			DocumentModelPositionAnchor anchor = new DocumentModelPositionAnchor(pos);
			anchor.OnRunMerged(PieceTable, paragraphIndex, newRunIndex, length);
			OnChanged(false, anchor.PositionChanged);
			return anchor.PositionChanged;
		}
	}
#endregion
#region RangePermissionCollection
	public class RangePermissionCollection : BookmarkBaseCollection<RangePermission> {
		VisitableDocumentIntervalBoundaryCollection boundaries;
		public RangePermissionCollection(PieceTable pieceTable)
			: base(pieceTable) {
		}
		 VisitableDocumentIntervalBoundaryCollection Boundaries {
			get {
				if (boundaries == null)
					boundaries = CreateBoundaryCollection();
				return boundaries;
			}
		}
		protected override void OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			RangePermissionBoundaryUpdater updater = new RangePermissionBoundaryUpdater(PieceTable, new RunInsertedUpdateStrategy());
			updater.Update(Boundaries, paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		protected override void OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			RangePermissionBoundaryUpdater updater = new RangePermissionBoundaryUpdater(PieceTable, new RunMergedUpdateStrategy());
			updater.Update(Boundaries, paragraphIndex, runIndex, deltaRunLength, NotificationIdGenerator.EmptyId);
		}
		VisitableDocumentIntervalBoundaryCollection CreateBoundaryCollection() {
			VisitableDocumentIntervalBoundaryCollection result = new VisitableDocumentIntervalBoundaryCollection();
			for (int i = 0; i < Count; i++) {
				RangePermission permission = this[i];
				result.Add(new RangePermissionStartBoundary(permission));
				result.Add(new RangePermissionEndBoundary(permission));
			}
			result.Sort(new RangePermissionBoundaryComparer());
			return result;
		}
		protected override void OnDocumentIntervalInserted(int index) {
			base.OnDocumentIntervalInserted(index);
			boundaries = null;
		}
		protected override void OnDocumentIntervalRemoved(int index) {
			base.OnDocumentIntervalRemoved(index);
			boundaries = null;
		}
	}
#endregion
	public interface IRangePermissionBoundaryUpdateStrategy {
		bool IsBoundaryAffected(DocumentModelPosition boundaryPosition, RunIndex newRunIndex);
		void ShiftBoundary(VisitableDocumentIntervalBoundary boundary, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId);
		void RemainBoundary(VisitableDocumentIntervalBoundary boundary, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length);
	}
	public class RunInsertedUpdateStrategy : IRangePermissionBoundaryUpdateStrategy {
#region IRangePermissionBoundaryUpdateStrategy Members
		public bool IsBoundaryAffected(DocumentModelPosition boundaryPosition, RunIndex newRunIndex) {
			return boundaryPosition.RunIndex == newRunIndex && boundaryPosition.RunOffset == 0;
		}
		public void ShiftBoundary(VisitableDocumentIntervalBoundary boundary, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			((RangePermissionBoundary)boundary).OnRunInserted(paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		public void RemainBoundary(VisitableDocumentIntervalBoundary boundary, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length) {
		}
#endregion
	}
	public class RunMergedUpdateStrategy : IRangePermissionBoundaryUpdateStrategy {
#region IRangePermissionBoundaryUpdateStrategy Members
		public bool IsBoundaryAffected(DocumentModelPosition boundaryPosition, RunIndex newRunIndex) {
			return boundaryPosition.RunIndex == newRunIndex + 1 && boundaryPosition.RunOffset == 0;
		}
		public void ShiftBoundary(VisitableDocumentIntervalBoundary boundary, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			((RangePermissionBoundary)boundary).OnRunMerged(paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		public void RemainBoundary(VisitableDocumentIntervalBoundary boundary, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length) {
			DocumentModelPosition pos = boundary.Position;
			pos.RunIndex = newRunIndex;
			TextRunBase run = pos.PieceTable.Runs[pos.RunIndex];
			pos.RunStartLogPosition = pos.LogPosition - run.Length + length;
			pos.ParagraphIndex = run.Paragraph.Index;
		}
#endregion
	}
	public class RangePermissionBoundaryUpdater {
		readonly PieceTable pieceTable;
		readonly IRangePermissionBoundaryUpdateStrategy updateStrategy;
		public RangePermissionBoundaryUpdater(PieceTable pieceTable, IRangePermissionBoundaryUpdateStrategy updateStrategy) {
			this.pieceTable = pieceTable;
			this.updateStrategy = updateStrategy;
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public IRangePermissionBoundaryUpdateStrategy UpdateStrategy { get { return updateStrategy; } }
		public void Update(VisitableDocumentIntervalBoundaryCollection boundaries, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			int count = boundaries.Count;
			for (int index = 0; index < count; ) {
				DocumentModelPosition position = boundaries[index].Position;
				if (updateStrategy.IsBoundaryAffected(position, newRunIndex)) {
					int rangeLength = GetAdjoiningBoundariesCount(boundaries, index);
					ReorderBoundaries(boundaries, index, rangeLength);
					UpdateAdjoiningBoundaries(boundaries.GetRange(index, rangeLength), paragraphIndex, newRunIndex, length, historyNotificationId);
					index += rangeLength;
				}
				else {
					updateStrategy.ShiftBoundary(boundaries[index], paragraphIndex, newRunIndex, length, historyNotificationId);
					index++;
				}
			}
		}
		int GetAdjoiningBoundariesCount(VisitableDocumentIntervalBoundaryCollection boundaries, int index) {
			DocumentLogPosition position = boundaries[index].Position.LogPosition;
			int count = boundaries.Count;
			int result = 1;
			for (int i = index + 1; i < count; i++) {
				if (boundaries[i].Position.LogPosition != position)
					break;
				result++;
			}
			return result;
		}
		void ReorderBoundaries(VisitableDocumentIntervalBoundaryCollection boundaries, int index, int count) {
			boundaries.Sort(index, count, new RangePermissionBoundaryOrderComparer());
		}
		void UpdateAdjoiningBoundaries(List<VisitableDocumentIntervalBoundary> boundaries, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			int count = boundaries.Count;
			int startIndex = 0;
			for (int index = 0; index < count; index++) {
				VisitableDocumentIntervalBoundary boundary = boundaries[index];
				if (index > 0 && boundary.Order != boundaries[index - 1].Order)
					startIndex = index;
				if (ShouldUpdateBoundary(boundary)) {
					ShiftBoundaries(boundaries, startIndex, paragraphIndex, newRunIndex, length, historyNotificationId);
					return;
				}
			}
		}
		void ShiftBoundaries(List<VisitableDocumentIntervalBoundary> boundaries, int index, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			int count = boundaries.Count;
			for (int i = 0; i < count; i++) {
				if (i < index)
					updateStrategy.RemainBoundary(boundaries[i], paragraphIndex, newRunIndex, length);
				else
					updateStrategy.ShiftBoundary(boundaries[i], paragraphIndex, newRunIndex, length, historyNotificationId);
			}
		}
		bool ShouldUpdateBoundary(VisitableDocumentIntervalBoundary boundary) {
			if (boundary.Order == BookmarkBoundaryOrder.Start)
				return false;
			if (PieceTable.DocumentModel.IsDocumentProtectionEnabled) {
				RangePermission rangePermission = (RangePermission)boundary.VisitableInterval;
				return PieceTable.IsPermissionGranted(rangePermission);
			}
			else
				return true;
		}
	}
	public class RangePermissionBoundaryComparer : IComparer<VisitableDocumentIntervalBoundary> {
#region IComparer<BookmarkBoundary> Members
		public int Compare(VisitableDocumentIntervalBoundary x, VisitableDocumentIntervalBoundary y) {
			return x.Position.LogPosition - y.Position.LogPosition;
		}
#endregion
	}
	public class RangePermissionBoundaryOrderComparer : IComparer<VisitableDocumentIntervalBoundary> {
#region IComparer<BookmarkBoundary> Members
		public int Compare(VisitableDocumentIntervalBoundary x, VisitableDocumentIntervalBoundary y) {
			int result = x.VisitableInterval.NormalizedStart - y.VisitableInterval.NormalizedStart;
			if (result != 0)
				return result;
			result = x.VisitableInterval.NormalizedEnd - y.VisitableInterval.NormalizedEnd;
			if (result != 0)
				return result;
			return x.Order - y.Order;
		}
#endregion
	}
#region RangePermissionCollectionEx
	public class RangePermissionCollectionEx : RangePermissionCollection {
		public RangePermissionCollectionEx(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override int AddCore(RangePermission interval) {
			if (Contains(interval))
				return 0;
			List<RangePermission> toRemove = new List<RangePermission>();
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (interval.IntersectsWith(this[i])) {
					interval = RangePermission.Union(this[i], interval);
					toRemove.Add(InnerList[i]);
				}
			}
			RemoveCore(toRemove);
			InnerList.Add(interval);
			return Count - 1;
		}
		public virtual bool Remove(RangePermission interval) {
			if (interval == null)
				return false;
			int index = InnerList.IndexOf(interval);
			if (index >= 0) {
				InnerList.RemoveAt(index);
				return true;
			}
			List<RangePermission> toRemove = new List<RangePermission>();
			List<RangePermission> toAdd = new List<RangePermission>();
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (interval.IntersectsWithExcludingBounds(this[i])) {
					toRemove.Add(this[i]);
					RangePermissionCollection subtractResult = this[i].Subtract(interval);
					int subtractResultCount = subtractResult.Count;
					for (int subtractResultIndex = 0; subtractResultIndex < subtractResultCount; subtractResultIndex++)
						toAdd.Add(subtractResult[subtractResultIndex]);
				}
			}
			RemoveCore(toRemove);
			AddCore(toAdd);
			return true;
		}
		public virtual bool Contains(RangePermission interval) {
			if (InnerList.Contains(interval))
				return true;
			int count = Count;
			for (int i = 0; i < count; i++)
				if (this[i].Contains(interval))
					return true;
			return false;
		}
		void RemoveCore(List<RangePermission> toRemove) {
			int count = toRemove.Count;
			for (int i = 0; i < count; i++)
				InnerList.Remove(toRemove[i]);
		}
		void AddCore(List<RangePermission> toAdd) {
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
				InnerList.Add(toAdd[i]);
		}
	}
#endregion
}
