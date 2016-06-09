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
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using System.Text;
using System.Diagnostics;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotHierarchyCollection
	public class PivotHierarchyCollection : UndoableCollection<PivotHierarchy> {
		public PivotHierarchyCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		public void CopyFromNoHistory(PivotHierarchyCollection source) {
			ClearCore();
			Capacity = source.Count;
			DocumentModel documentModel = (DocumentModel)DocumentModel;
			foreach (PivotHierarchy item in source) {
				PivotHierarchy newItem = new PivotHierarchy(documentModel);
				newItem.CopyFromNoHistory(item);
				AddCore(newItem);
			}
		}
	}
	#endregion
	#region PivotHierarchy
	public class PivotHierarchy {
		#region Fields
		readonly DocumentModel documentModel;
		UndoableCollection<string> members;
		MemberPropertiesCollection memberProperties;
		string caption;
		uint packedValues;
		int? levelMembers;
		const uint offsetShowInFieldList = 1;
		const uint offsetDragToRow = 2;
		const uint offsetDragToCol = 4;
		const uint offsetDragToPage = 8;
		const uint offsetDragOff = 0x10;
		const uint offsetOutline = 0x20;
		const uint offsetMultipleItemSelectionAllowed = 0x40;
		const uint offsetSubtotalTop = 0x80;
		const uint offsetDragToData = 0x100;
		const uint offsetIncludeNewItemsInFilter = 0x200;
		#endregion
		#region Constructors
		public PivotHierarchy(DocumentModel documentModel){
			this.documentModel = documentModel;
			this.members = new UndoableCollection<string>(documentModel);
			this.memberProperties = new MemberPropertiesCollection(documentModel);
			packedValues = 31;
		}
		#endregion
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public UndoableCollection<string> Members { get { return members; } }
		public MemberPropertiesCollection MemberProperties { get { return memberProperties;} }
		public string Caption { get { return caption; } set { SetCaption(value); } }
		public int? LevelMembers { get { return levelMembers; } set { SetLevelMembers(value); } }
		public bool ShowInFieldList { 
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetShowInFieldList); }
			set {
				if (ShowInFieldList != value)
					SetHistory(offsetShowInFieldList, value); 
			}
		}
		public bool DragToRow {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetDragToRow); }
			set {
				if (DragToRow != value)
					SetHistory(offsetDragToRow, value);
			}
		}
		public bool DragToColumn {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetDragToCol); }
			set {
				if (DragToColumn != value)
					SetHistory(offsetDragToCol, value);
			}
		}
		public bool DragToPage {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetDragToPage); }
			set {
				if (DragToPage != value)
					SetHistory(offsetDragToPage, value);
			}
		}
		public bool Outline {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetOutline); }
			set {
				if (Outline != value)
					SetHistory(offsetOutline, value);
			}
		}
		public bool MultipleItemSelectionAllowed {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetMultipleItemSelectionAllowed); }
			set {
				if (MultipleItemSelectionAllowed != value)
					SetHistory(offsetMultipleItemSelectionAllowed, value);
			}
		}
		public bool SubtotalTop {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetSubtotalTop); }
			set {
				if (SubtotalTop != value)
					SetHistory(offsetSubtotalTop, value);
			}
		}
		public bool DragToData {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetDragToData); }
			set {
				if (DragToData != value)
					SetHistory(offsetDragToData, value);
			}
		}
		public bool DragOff {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetDragOff); }
			set {
				if (DragOff != value)
					SetHistory(offsetDragOff, value);
			}
		}
		public bool IncludeNewItemsInFilter {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetIncludeNewItemsInFilter); }
			set {
				if (IncludeNewItemsInFilter != value)
					SetHistory(offsetIncludeNewItemsInFilter, value);
			}
		}
		#endregion
		void SetLevelMembers(int? value) {
			if (levelMembers != value) {
				ActionHistoryItem<int?> historyItem = new ActionHistoryItem<int?>(DocumentModel, levelMembers, value, SetLevelMembersCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetCaption(string value) {
			if (String.Compare(Caption, value) != 0) {
				ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(DocumentModel, caption, value, SetCaptionCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetHistory(uint offset, bool value) {
			ActionHistoryItem<uint> historyItem =
				new ActionHistoryItem<uint>(DocumentModel, packedValues, CreateNewPackedValue(offset, value), SetPackageNewValueCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		uint CreateNewPackedValue(uint offset, bool value) {
			uint nPack = this.packedValues;
			PackedValues.SetBoolBitValue(ref nPack, offset, value);
			return nPack;
		}
		void SetPackageNewValueCore(uint value) {
			packedValues = value;
		}
		protected internal void SetLevelMembersCore(int? value){
			levelMembers = value;
		}
		protected internal void SetCaptionCore(string value) {
			caption = value;
		}
		protected internal void SetShowInFieldListCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetShowInFieldList, value);
		}
		protected internal void SetDragToRowCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetDragToRow, value);
		}
		protected internal void SetDragToColCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetDragToCol, value);
		}
		protected internal void SetDragToPageCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetDragToPage, value);
		}
		protected internal void SetOutlineCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetOutline, value);
		}
		protected internal void SetMultipleItemSelectionAllowedCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetMultipleItemSelectionAllowed, value);
		}
		protected internal void SetSubtotalTopCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetSubtotalTop, value);
		}
		protected internal void SetDragToDataCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetDragToData, value);
		}
		protected internal void SetDragOffCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetDragOff, value);
		}
		protected internal void SetIncludeNewItemsInFilterCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetIncludeNewItemsInFilter, value);
		}
		public void CopyFrom(PivotHierarchy sourceItem) {
			this.caption = sourceItem.caption;
			this.levelMembers = sourceItem.levelMembers;
			this.packedValues = sourceItem.packedValues;
			MemberPropertiesCollection sourceMemberProperties = sourceItem.memberProperties;
			MemberPropertiesCollection targetMemberProperties = this.memberProperties;
			targetMemberProperties.Capacity = sourceMemberProperties.Count;
			foreach(MemberProperty sourceMemberProperty in sourceMemberProperties) {
				MemberProperty targetMemberProperty = new MemberProperty(this.documentModel);
				targetMemberProperty.CopyFrom(sourceMemberProperty);
				targetMemberProperties.Add(targetMemberProperty);
			}
			Debug.Assert(this.members.Count == 0, "this.members.Count == 0", "");
			this.members.InnerList.AddRange(sourceItem.members.InnerList);
		}
		public void CopyFromNoHistory(PivotHierarchy sourceItem) {
			members.InnerList.Clear();
			members.InnerList.AddRange(sourceItem.members);
			memberProperties.CopyFromNoHistory(sourceItem.memberProperties);
			caption = sourceItem.caption;
			packedValues = sourceItem.packedValues;
			levelMembers = sourceItem.levelMembers;
		}
	}
	#endregion
	#region MemberPropertieCollection
	public class MemberPropertiesCollection : UndoableCollection<MemberProperty> {
		public MemberPropertiesCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		public void CopyFromNoHistory(MemberPropertiesCollection source) {
			ClearCore();
			Capacity = source.Count;
			DocumentModel documentModel = (DocumentModel)DocumentModel;
			foreach (MemberProperty item in source) {
				MemberProperty newItem = new MemberProperty(documentModel);
				newItem.CopyFromNoHistory(item);
				AddCore(newItem);
			}
		}
	}
	#endregion
	#region MemberPropertie
	public class MemberProperty {
		#region Fields
		readonly DocumentModel documentModel;
		string name;
		long? nameLen;
		long? pPos;
		long? pLen;
		long? level;
		long field;
		uint packedValues;
		const uint offsetShowCell = 1;
		const uint offsetShowTip = 2;
		const uint offsetShowAsCaption = 4;
		#endregion
		#region Constructors
		public MemberProperty(DocumentModel documentModel) {
			this.documentModel = documentModel;
			packedValues = 0;
		}
		#endregion
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public string Name { get { return name; } set { SetName(value); } }
		public long? NameLen { get { return nameLen; } set { SetNameLen(value); } }
		public long? PropertyNameLength { get { return pLen; } set { SetPropertyNameLength(value); } }
		public long? PropertyNameCharacterIndex { get { return pPos; } set { SetPropertyNameCharacterIndex(value); } }
		public long? LevelIndex { get { return level; } set { SetLevelIndex(value); } }
		public long FieldIndex { get { return field; } set { SetFieldIndex(value); } }
		public bool ShowCell {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetShowCell); }
			set {
				if (ShowCell != value)
					SetHistory(offsetShowCell, value);
			}
		}
		public bool ShowTip {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetShowTip); }
			set {
				if (ShowTip != value)
					SetHistory(offsetShowTip, value);
			}
		}
		public bool ShowAsCaption {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetShowAsCaption); }
			set {
				if (ShowAsCaption != value)
					SetHistory(offsetShowAsCaption, value);
			}
		}
		#endregion
		void SetName(string value) {
			if (String.Compare(Name, value) != 0) {
				ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(DocumentModel, name, value, SetNameCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetNameLen(long? value) {
			if (NameLen != value) {
				ActionHistoryItem<long?> historyItem = new ActionHistoryItem<long?>(DocumentModel, nameLen, value, SetNameLenCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetPropertyNameLength(long? value){
			if (PropertyNameLength != value) {
				ActionHistoryItem<long?> historyItem = new ActionHistoryItem<long?>(DocumentModel, pLen, value, SetPropertyNameLengthCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetPropertyNameCharacterIndex(long? value) {
			if (PropertyNameCharacterIndex != value) {
				ActionHistoryItem<long?> historyItem = new ActionHistoryItem<long?>(DocumentModel, pPos, value, SetPropertyNameCharacterIndexCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetLevelIndex(long? value) {
			if (LevelIndex != value) {
				ActionHistoryItem<long?> historyItem = new ActionHistoryItem<long?>(DocumentModel, level, value, SetLevelIndexCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetFieldIndex(long value) {
			if (FieldIndex != value) {
				ActionHistoryItem<long> historyItem = new ActionHistoryItem<long>(DocumentModel, field, value, SetFieldIndexCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetHistory(uint offset, bool value) {
			ActionHistoryItem<uint> historyItem =
				new ActionHistoryItem<uint>(DocumentModel, packedValues, CreateNewPackedValue(offset, value), SetPackageNewValueCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		uint CreateNewPackedValue(uint offset, bool value) {
			uint nPack = this.packedValues;
			PackedValues.SetBoolBitValue(ref nPack, offset, value);
			return nPack;
		}
		void SetPackageNewValueCore(uint value) {
			packedValues = value;
		}
		protected internal void SetNameCore(string value) {
			name = value;
		}
		protected internal void SetNameLenCore(long? value) {
			nameLen = (uint?)value;
		}
		protected internal void SetPropertyNameLengthCore(long? value) {
			pLen = (uint?)value;
		}
		protected internal void SetPropertyNameCharacterIndexCore(long? value) {
			pPos = (uint?)value;
		}
		protected internal void SetLevelIndexCore(long? value) {
			level = (uint?)value;
		}
		protected internal void SetFieldIndexCore(long value) {
			field = (uint)value;
		}
		protected internal void SetShowCellCore(bool value){
			PackedValues.SetBoolBitValue(ref packedValues, offsetShowCell, value);
		}
		protected internal void SetShowTipCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetShowTip, value);
		}
		protected internal void SetShowAsCaptionCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetShowAsCaption, value);
		}
		public void CopyFrom(MemberProperty source) {
			this.field = source.field;
			this.level = source.level;
			this.name = source.name;
			this.nameLen = source.nameLen;
			this.packedValues = source.packedValues;
			this.pLen = source.pLen;
			this.pPos = source.pPos;
		}
		public void CopyFromNoHistory(MemberProperty source) {
			name = source.name;
			nameLen = source.nameLen;
			pPos = source.pPos;
			pLen = source.pLen;
			level = source.level;
			field = source.field;
			packedValues = source.packedValues;
		}
	}
	#endregion
}
