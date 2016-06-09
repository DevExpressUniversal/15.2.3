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

using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public interface IProtectionInfo {
		bool IsEmpty { get; }
		bool CheckPassword(string password);
	}
	public class ProtectionByPasswordVerifier : IProtectionInfo {
		readonly UInt16 passwordVerifier;
		[CLSCompliant(false)]
		public ProtectionByPasswordVerifier(UInt16 passwordVerifier) {
			this.passwordVerifier = passwordVerifier;
		}
		public bool IsEmpty { get { return passwordVerifier == PasswordNotSetted; } }
		[CLSCompliant(false)]
		public const UInt16 PasswordNotSetted = 0;
		[CLSCompliant(false)]
		public UInt16 Value { get { return passwordVerifier; } }
		public override bool Equals(object obj) {
			ProtectionByPasswordVerifier other = obj as ProtectionByPasswordVerifier;
			if (other == null)
				return false;
			return other.passwordVerifier == this.passwordVerifier;
		}
		public override int GetHashCode() {
			return (int)passwordVerifier;
		}
		public override string ToString() {
			return passwordVerifier.ToString();
		}
		public bool CheckPassword(string testPassword) {
			PasswordVerifierCalculator calculator = new PasswordVerifierCalculator();
			UInt16 test = calculator.Calculate(testPassword);
			return Value == test;
		}
	}
	public class ProtectionByWorkbookRevisions : IProtectionInfo {
		readonly UInt16 revisionsPasswordVerifier;
		[CLSCompliant(false)]
		public ProtectionByWorkbookRevisions(UInt16 revisionsPasswordVerifier) {
			this.revisionsPasswordVerifier = revisionsPasswordVerifier;
		}
		public bool IsEmpty { get { return revisionsPasswordVerifier == RevisionsPasswordNotSetted; } }
		[CLSCompliant(false)]
		public const UInt16 RevisionsPasswordNotSetted = 0;
		[CLSCompliant(false)]
		public UInt16 Value { get { return revisionsPasswordVerifier; } }
		public override bool Equals(object obj) {
			ProtectionByWorkbookRevisions other = obj as ProtectionByWorkbookRevisions;
			if (other == null)
				return false;
			return other.revisionsPasswordVerifier == this.revisionsPasswordVerifier;
		}
		public override int GetHashCode() {
			return (int)revisionsPasswordVerifier;
		}
		public override string ToString() {
			return revisionsPasswordVerifier.ToString();
		}
		public bool CheckPassword(string testPassword) {
			PasswordVerifierCalculator calculator = new PasswordVerifierCalculator();
			UInt16 test = calculator.Calculate(testPassword);
			return Value == test;
		}
	}
	public class CryptographicProtectionInfo : IProtectionInfo {
		readonly byte[] hashValue;
		readonly byte[] saltValue;
		readonly int spinCount;
		readonly HashAlgorithmType algorithmType;
		public CryptographicProtectionInfo(byte[] hashValue, byte[] saltValue, int spinCount, HashAlgorithmType algorithmType) {
			Guard.ArgumentNotNull(hashValue, "hashValue");
			Guard.ArgumentNotNull(saltValue, "saltValue");
			this.hashValue = hashValue;
			this.saltValue = saltValue;
			this.spinCount = spinCount;
			this.algorithmType = algorithmType;
		}
		public bool IsEmpty { get { return hashValue.Length == 0; } }
		public byte[] HashValue { get { return hashValue; } }
		public byte[] SaltValue { get { return saltValue; } }
		public int SpinCount { get { return spinCount; } }
		public HashAlgorithmType AlgorithmType { get { return algorithmType; } }
		public override bool Equals(object obj) {
			CryptographicProtectionInfo other = obj as CryptographicProtectionInfo;
			if (other == null)
				return false;
			return SpreadsheetPasswordHashCalculator.CompareByteArrays(hashValue, other.hashValue)
				&& SpreadsheetPasswordHashCalculator.CompareByteArrays(saltValue, other.saltValue)
				&& spinCount == other.spinCount
				&& algorithmType == other.algorithmType;
		}
		public override int GetHashCode() {
			CombinedHashCode calculator = new CombinedHashCode(CombinedHashCode.Initial);
			calculator.AddByteArray(hashValue);
			calculator.AddByteArray(saltValue);
			return calculator.CombinedHash32;
		}
		public override string ToString() {
			int stringForReadingLength = 5;
			if (hashValue == null || hashValue.Length < stringForReadingLength)
				return base.ToString();
			return Convert.ToBase64String(hashValue, 0, 5);
		}
		public bool CheckPassword(string testPassword) {
			SpreadsheetPasswordHashCalculator hashCalculator = new SpreadsheetPasswordHashCalculator();
			byte[] test = hashCalculator.CalculatePasswordHashSpreadsheet(testPassword, SaltValue, SpinCount, AlgorithmType);
			return SpreadsheetPasswordHashCalculator.CompareByteArrays(test, hashValue);
		}
	}
	public class OpenOfficeProtectionInfo : IProtectionInfo {
		readonly byte[] passwordHash;
		public OpenOfficeProtectionInfo(byte[] passwordHash) {
			this.passwordHash = passwordHash;
		}
		public bool IsEmpty { get { return passwordHash.Length == 0; } }
		public bool CheckPassword(string testPassword) {
			return false;
		}
	}
	public class ProtectionCredentials : ICloneable<ProtectionCredentials> {
		const int defaultSpinCount = 100000;
		static ProtectionCredentials noProtection = new ProtectionCredentials(){ isReadonly = true};
		ProtectionByPasswordVerifier passwordVerifier = null;
		CryptographicProtectionInfo cryptographicProtection = null;
		ProtectionByWorkbookRevisions revisionProtection = null;
		bool isReadonly = false;
		public static ProtectionCredentials NoProtection { get { return noProtection; } }
		bool UsePasswordVerifierFirst { 
			get {
#if DEBUGTEST || DEBUG
				return true;
#else
				return false;
#endif
			}
		}
		public ProtectionCredentials() {
		}
		public ProtectionCredentials(string password, bool addWorkbookRevisionPassword) 
			: this(password, addWorkbookRevisionPassword, true, defaultSpinCount) {
		}
		public ProtectionCredentials(string password, bool addWorkbookRevisionPassword, bool useStrongPasswordVerifier, int spinCount) {
			UInt16 passwordVefirier = (new PasswordVerifierCalculator()).Calculate(password);
			RegisterPasswordVerifier(new ProtectionByPasswordVerifier(passwordVefirier));
			if (!string.IsNullOrEmpty(password) && useStrongPasswordVerifier) {
				int length = 512 / 8; 
				SpreadsheetPasswordHashCalculator hashCalculator = new SpreadsheetPasswordHashCalculator();
				byte[] salt = hashCalculator.GenerateSalt(length);
#if DEBUG || DEBUGTEST
				int defaultIteractions = 10;
#else
				int defaultIteractions = spinCount;
#endif
				HashAlgorithmType algorithm = HashAlgorithmType.Sha512;
				byte[] hash = hashCalculator.CalculatePasswordHashSpreadsheet(password, salt, defaultIteractions, algorithm);
				CryptographicProtectionInfo cryptographicProtectionInfo = new CryptographicProtectionInfo(hash, salt, defaultIteractions, algorithm);
				RegisterCryptographicProtection(cryptographicProtectionInfo);
			}
			if (addWorkbookRevisionPassword) {
				ProtectionByWorkbookRevisions workbookProtection = new ProtectionByWorkbookRevisions(passwordVefirier);
				RegisterWorkbookRevisionsProtection(workbookProtection);
			}
		}
		public ProtectionByPasswordVerifier PasswordVerifier { get { return passwordVerifier; } }
		public CryptographicProtectionInfo CryptographicProtection { get { return cryptographicProtection; } }
		public ProtectionByWorkbookRevisions RevisionProtection { get { return revisionProtection; } }
		public void RegisterPasswordVerifier(ProtectionByPasswordVerifier byPasswordVerifier){
			if(isReadonly)
				throw new InvalidOperationException("changing default item in cache: protectionOptions shoud be updatelocked");
			this.passwordVerifier = byPasswordVerifier;
		}
		public void RegisterCryptographicProtection(CryptographicProtectionInfo byCryptographicHash) {
			if(isReadonly)
				throw new InvalidOperationException("changing default item in cache: protectionOptions shoud be updatelocked");
			this.cryptographicProtection = byCryptographicHash;
		}
		public void RegisterWorkbookRevisionsProtection(ProtectionByWorkbookRevisions revisionProtection) {
			if(isReadonly)
				throw new InvalidOperationException("changing default item in cache: protectionOptions shoud be updatelocked");
			this.revisionProtection = revisionProtection;
		}
		public bool IsEmpty { 
			get {
				bool result = true;
				if (passwordVerifier != null)
					result &= passwordVerifier.IsEmpty;
				if (cryptographicProtection != null)
					result &= cryptographicProtection.IsEmpty;
				if (revisionProtection != null)
					result &= revisionProtection.IsEmpty;
				return result;
			} 
		}
		public bool CheckPassword(string password) {
			if (passwordVerifier == null 
				&& cryptographicProtection == null 
				&& revisionProtection == null)
				return true;
			if (UsePasswordVerifierFirst) {
				if (passwordVerifier != null)
					return passwordVerifier.CheckPassword(password);
				if (cryptographicProtection != null)
					return cryptographicProtection.CheckPassword(password);
			}
			else {
				if (cryptographicProtection != null)
					return cryptographicProtection.CheckPassword(password);
				if (passwordVerifier != null)
					return passwordVerifier.CheckPassword(password);
			}
			if (revisionProtection != null)
				return revisionProtection.CheckPassword(password);
			return false;
		}
		public override int GetHashCode() {
			CombinedHashCode combined = new CombinedHashCode();
			combined.AddObject(passwordVerifier);
			combined.AddObject(revisionProtection);
			combined.AddObject(cryptographicProtection);
			return combined.CombinedHash32;
		}
		public override bool Equals(object obj){
			ProtectionCredentials other = obj as ProtectionCredentials;
			if(other == null) 
				return false;
			bool equals =  passwordVerifier == null && other.passwordVerifier == null;
			if(passwordVerifier != null && other.passwordVerifier != null) 
				equals = passwordVerifier.Equals(other.passwordVerifier);
			if(!equals)
				return equals;
			equals = cryptographicProtection == null && other.cryptographicProtection == null;
			if(cryptographicProtection != null && other.cryptographicProtection != null)
				equals = cryptographicProtection.Equals(other.cryptographicProtection);
			if(!equals)
				return equals;
			equals = revisionProtection == null  && other.revisionProtection == null;
			if(revisionProtection != null  && other.revisionProtection != null)
				equals = revisionProtection.Equals(other.revisionProtection);
			return equals;
		}
		public void CopyFrom(ProtectionCredentials other){
			this.passwordVerifier = other.passwordVerifier;
			this.cryptographicProtection = other.cryptographicProtection;
			this.revisionProtection = other.revisionProtection;
			this.isReadonly = other.isReadonly;
		}
		public ProtectionCredentials Clone() {
			ProtectionCredentials clonned = new ProtectionCredentials();
			clonned.CopyFrom(this);
			clonned.isReadonly = false;
			return clonned;
		}
	}
	public class WorksheetProtectionOptions : SpreadsheetUndoableIndexBasedObject<WorksheetProtectionInfo>, ICloneable<WorksheetProtectionOptions>{
		public WorksheetProtectionOptions(IDocumentModelPartWithApplyChanges part)
			: base(part) {
		}
		#region Properties
		#region AutoFiltersLocked
		public bool AutoFiltersLocked {
			get { return Info.AutoFiltersLocked; }
			set {
				if (AutoFiltersLocked == value)
					return;
				SetPropertyValue(SetAutoFiltersLockedCore, value);
			}
		}
		DocumentModelChangeActions SetAutoFiltersLockedCore(WorksheetProtectionInfo info, bool value) {
			info.AutoFiltersLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DeleteColumnsLocked
		public bool DeleteColumnsLocked {
			get { return Info.DeleteColumnsLocked; }
			set {
				if (DeleteColumnsLocked == value)
					return;
				SetPropertyValue(SetDeleteColumnsLockedCore, value);
			}
		}
		DocumentModelChangeActions SetDeleteColumnsLockedCore(WorksheetProtectionInfo info, bool value) {
			info.DeleteColumnsLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DeleteRowsLocked
		public bool DeleteRowsLocked {
			get { return Info.DeleteRowsLocked; }
			set {
				if (DeleteRowsLocked == value)
					return;
				SetPropertyValue(SetDeleteRowsLockedCore, value);
			}
		}
		DocumentModelChangeActions SetDeleteRowsLockedCore(WorksheetProtectionInfo info, bool value) {
			info.DeleteRowsLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FormatCellsLocked
		public bool FormatCellsLocked {
			get { return Info.FormatCellsLocked; }
			set {
				if (FormatCellsLocked == value)
					return;
				SetPropertyValue(SetFormatCellsLockedCore, value);
			}
		}
		DocumentModelChangeActions SetFormatCellsLockedCore(WorksheetProtectionInfo info, bool value) {
			info.FormatCellsLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FormatColumnsLocked
		public bool FormatColumnsLocked {
			get { return Info.FormatColumnsLocked; }
			set {
				if (FormatColumnsLocked == value)
					return;
				SetPropertyValue(SetFormatColumnsLockedCore, value);
			}
		}
		DocumentModelChangeActions SetFormatColumnsLockedCore(WorksheetProtectionInfo info, bool value) {
			info.FormatColumnsLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FormatRowsLocked
		public bool FormatRowsLocked {
			get { return Info.FormatRowsLocked; }
			set {
				if (FormatRowsLocked == value)
					return;
				SetPropertyValue(SetFormatRowsLockedCore, value);
			}
		}
		DocumentModelChangeActions SetFormatRowsLockedCore(WorksheetProtectionInfo info, bool value) {
			info.FormatRowsLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region InsertColumnsLocked
		public bool InsertColumnsLocked {
			get { return Info.InsertColumnsLocked; }
			set {
				if (InsertColumnsLocked == value)
					return;
				SetPropertyValue(SetInsertColumnsLockedCore, value);
			}
		}
		DocumentModelChangeActions SetInsertColumnsLockedCore(WorksheetProtectionInfo info, bool value) {
			info.InsertColumnsLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region InsertHyperlinksLocked
		public bool InsertHyperlinksLocked {
			get { return Info.InsertHyperlinksLocked; }
			set {
				if (InsertHyperlinksLocked == value)
					return;
				SetPropertyValue(SetInsertHyperlinksLockedCore, value);
			}
		}
		DocumentModelChangeActions SetInsertHyperlinksLockedCore(WorksheetProtectionInfo info, bool value) {
			info.InsertHyperlinksLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region InsertRowsLocked
		public bool InsertRowsLocked {
			get { return Info.InsertRowsLocked; }
			set {
				if (InsertRowsLocked == value)
					return;
				SetPropertyValue(SetInsertRowsLockedCore, value);
			}
		}
		DocumentModelChangeActions SetInsertRowsLockedCore(WorksheetProtectionInfo info, bool value) {
			info.InsertRowsLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ObjectsLocked
		public bool ObjectsLocked {
			get { return Info.ObjectsLocked; }
			set {
				if (ObjectsLocked == value)
					return;
				SetPropertyValue(SetObjectsLockedCore, value);
			}
		}
		DocumentModelChangeActions SetObjectsLockedCore(WorksheetProtectionInfo info, bool value) {
			info.ObjectsLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PivotTablesLocked
		public bool PivotTablesLocked {
			get { return Info.PivotTablesLocked; }
			set {
				if (PivotTablesLocked == value)
					return;
				SetPropertyValue(SetPivotTablesLockedCore, value);
			}
		}
		DocumentModelChangeActions SetPivotTablesLockedCore(WorksheetProtectionInfo info, bool value) {
			info.PivotTablesLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ScenariosLocked
		public bool ScenariosLocked {
			get { return Info.ScenariosLocked; }
			set {
				if (ScenariosLocked == value)
					return;
				SetPropertyValue(SetScenariosLockedCore, value);
			}
		}
		DocumentModelChangeActions SetScenariosLockedCore(WorksheetProtectionInfo info, bool value) {
			info.ScenariosLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SelectLockedCellsLocked
		public bool SelectLockedCellsLocked {
			get { return Info.SelectLockedCellsLocked; }
			set {
				if (SelectLockedCellsLocked == value)
					return;
				SetPropertyValue(SetSelectLockedCellsLockedCore, value);
			}
		}
		DocumentModelChangeActions SetSelectLockedCellsLockedCore(WorksheetProtectionInfo info, bool value) {
			info.SelectLockedCellsLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SelectUnlockedCellsLocked
		public bool SelectUnlockedCellsLocked {
			get { return Info.SelectUnlockedCellsLocked; }
			set {
				if (SelectUnlockedCellsLocked == value)
					return;
				SetPropertyValue(SetShowHorizontalScrollCore, value);
			}
		}
		DocumentModelChangeActions SetShowHorizontalScrollCore(WorksheetProtectionInfo info, bool value) {
			info.SelectUnlockedCellsLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SheetLocked
		public bool SheetLocked {
			get { return Info.SheetLocked; }
			set {
				if(SheetLocked == value)
					return;
				SetPropertyValue(SetSheetLockedCore, value);
			}
		}
		DocumentModelChangeActions SetSheetLockedCore(WorksheetProtectionInfo info, bool value) {
			info.SheetLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SortLocked
		public bool SortLocked {
			get { return Info.SortLocked; }
			set {
				if (SortLocked == value)
					return;
				SetPropertyValue(SetSortLockedCore, value);
			}
		}
		DocumentModelChangeActions SetSortLockedCore(WorksheetProtectionInfo info, bool value) {
			info.SortLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ContentLocked
		public bool ContentLocked  {
			get { return Info.ContentLocked; }
			set {
				if (SortLocked == value)
					return;
				SetPropertyValue(SetContentLocked, value);
			}
		}
		DocumentModelChangeActions SetContentLocked(WorksheetProtectionInfo info, bool value) {
			info.ContentLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public ProtectionCredentials Credentials {
			get { return Info.Credentials; }
			set {
				if (Credentials.Equals(value))
					return;
				SetPropertyValue(SetCredentials, value);
			}
		}
		DocumentModelChangeActions SetCredentials(WorksheetProtectionInfo info, ProtectionCredentials value) {
			info.Credentials = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public WorksheetProtectionOptions Clone() {
			WorksheetProtectionOptions result = new WorksheetProtectionOptions(DocumentModel);
			result.Info.CopyFrom(this.Info);
			return result;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<WorksheetProtectionInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.WorksheetProtectionInfoCache;
		}
		public bool CheckPassword(string password) {
			return Credentials.CheckPassword(password);
		}
	}
	public class WorksheetProtectionInfo : ICloneable<WorksheetProtectionInfo>, ISupportsCopyFrom<WorksheetProtectionInfo>, ISupportsSizeOf {
		#region Constants
		const uint MaskSheetLocked = 0x00000001;				
		const uint MaskAutoFiltersLocked = 0x00000002;		  
		const uint MaskDeleteColumnsLocked = 0x00000004;		
		const uint MaskDeleteRowsLocked = 0x00000008;		   
		const uint MaskFormatCellsLocked = 0x00000010;		  
		const uint MaskFormatColumnsLocked = 0x00000020;		
		const uint MaskFormatRowsLocked = 0x000000040;		  
		const uint MaskInsertColumnsLocked = 0x00000080;		
		const uint MaskInsertHyperlinksLocked = 0x00000100;	 
		const uint MaskInsertRowsLocked = 0x00000200;		   
		const uint MaskObjectsLocked = 0x00000400;			  
		const uint MaskPivotTablesLocked = 0x00000800;		  
		const uint MaskScenariosLocked = 0x00001000;			
		const uint MaskSelectLockedCellsLocked = 0x00002000;	
		const uint MaskSelectUnlockedCellsLocked = 0x00004000;  
		const uint MaskSortLocked = 0x00008000;				 
		const uint MaskContentLocked = 0x00010000;			  
		#endregion
		#region Fields
		uint packedValues;
		ProtectionCredentials credentials = ProtectionCredentials.NoProtection;
		#endregion
		#region Properties
		public bool SheetLocked { get { return GetBooleanVal(MaskSheetLocked); } set { SetBooleanVal(MaskSheetLocked, value); } }
		public bool AutoFiltersLocked { get { return GetBooleanVal(MaskAutoFiltersLocked); } set { SetBooleanVal(MaskAutoFiltersLocked, value); } }
		public bool DeleteColumnsLocked { get { return GetBooleanVal(MaskDeleteColumnsLocked); } set { SetBooleanVal(MaskDeleteColumnsLocked, value); } }
		public bool DeleteRowsLocked { get { return GetBooleanVal(MaskDeleteRowsLocked); } set { SetBooleanVal(MaskDeleteRowsLocked, value); } }
		public bool FormatCellsLocked { get { return GetBooleanVal(MaskFormatCellsLocked); } set { SetBooleanVal(MaskFormatCellsLocked, value); } }
		public bool FormatColumnsLocked { get { return GetBooleanVal(MaskFormatColumnsLocked); } set { SetBooleanVal(MaskFormatColumnsLocked, value); } }
		public bool FormatRowsLocked { get { return GetBooleanVal(MaskFormatRowsLocked); } set { SetBooleanVal(MaskFormatRowsLocked, value); } }
		public bool InsertColumnsLocked { get { return GetBooleanVal(MaskInsertColumnsLocked); } set { SetBooleanVal(MaskInsertColumnsLocked, value); } }
		public bool InsertHyperlinksLocked { get { return GetBooleanVal(MaskInsertHyperlinksLocked); } set { SetBooleanVal(MaskInsertHyperlinksLocked, value); } }
		public bool InsertRowsLocked { get { return GetBooleanVal(MaskInsertRowsLocked); } set { SetBooleanVal(MaskInsertRowsLocked, value); } }
		public bool ObjectsLocked { get { return GetBooleanVal(MaskObjectsLocked); } set { SetBooleanVal(MaskObjectsLocked, value); } }
		public bool PivotTablesLocked { get { return GetBooleanVal(MaskPivotTablesLocked); } set { SetBooleanVal(MaskPivotTablesLocked, value); } }
		public bool ScenariosLocked { get { return GetBooleanVal(MaskScenariosLocked); } set { SetBooleanVal(MaskScenariosLocked, value); } }
		public bool SelectLockedCellsLocked { get { return GetBooleanVal(MaskSelectLockedCellsLocked); } set { SetBooleanVal(MaskSelectLockedCellsLocked, value); } }
		public bool SelectUnlockedCellsLocked { get { return GetBooleanVal(MaskSelectUnlockedCellsLocked); } set { SetBooleanVal(MaskSelectUnlockedCellsLocked, value); } }
		public bool SortLocked { get { return GetBooleanVal(MaskSortLocked); } set { SetBooleanVal(MaskSortLocked, value); } }
		public bool ContentLocked { get { return GetBooleanVal(MaskContentLocked); } set { SetBooleanVal(MaskContentLocked, value); } }
		public ProtectionCredentials Credentials { get { return credentials; } set { credentials = value; } }
		#endregion
		#region GetVal/SetVal helpers
		void SetBooleanVal(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanVal(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		#region ICloneable<WorksheetProtectionInfo> Members
		public WorksheetProtectionInfo Clone() {
			WorksheetProtectionInfo result = new WorksheetProtectionInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<WorksheetProtectionInfo> Members
		public void CopyFrom(WorksheetProtectionInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.credentials = value.credentials.Clone();
		}
		#endregion
		public override bool Equals(object obj) {
			WorksheetProtectionInfo info = obj as WorksheetProtectionInfo;
			if (info == null)
				return false;
			return this.packedValues == info.packedValues && this.credentials.Equals(info.Credentials);
		}
		public override int GetHashCode() {
			CombinedHashCode calculator = new CombinedHashCode((long)packedValues);
			calculator.AddObject(credentials);
			return calculator.CombinedHash32;
		}
	}
	#region WorksheetProtectionInfoCache
	public class WorksheetProtectionInfoCache : UniqueItemsCache<WorksheetProtectionInfo> {
		public WorksheetProtectionInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		public const int DefaultItemIndex = 0;
		public const int SchemaDefaultItemIndex = 1;
		public WorksheetProtectionInfo SchemaDefaultItem { get { return this[SchemaDefaultItemIndex]; } }
		protected override WorksheetProtectionInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			WorksheetProtectionInfo item = new WorksheetProtectionInfo();
			item.AutoFiltersLocked = true;
			item.DeleteColumnsLocked = true;
			item.DeleteRowsLocked = true;
			item.FormatCellsLocked = true;
			item.FormatColumnsLocked = true;
			item.FormatRowsLocked = true;
			item.InsertColumnsLocked = true;
			item.InsertHyperlinksLocked = true;
			item.InsertRowsLocked = true;
			item.ObjectsLocked = true;
			item.PivotTablesLocked = true;
			item.ScenariosLocked = true;
			item.SelectLockedCellsLocked = false;
			item.SelectUnlockedCellsLocked = false;
			item.SheetLocked = false;
			item.SortLocked = true;
			item.ContentLocked = false;
			return item;
		}
		protected override int AddItemCore(WorksheetProtectionInfo item) {
			return base.AddItemCore(item);
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			base.InitItems(unitConverter);
			WorksheetProtectionInfo schemaDefaultItem = CreateDefaultItem(unitConverter);
			schemaDefaultItem.ScenariosLocked = false;
			schemaDefaultItem.ObjectsLocked = false;
			AppendItem(schemaDefaultItem);
		}
	}
	#endregion
}
