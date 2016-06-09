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
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Model {
	#region ParagraphPropertiesBasedStyle (abstract class)
	public abstract class ParagraphPropertiesBasedStyle<TParagraphPropertiesBasedStyle> : StyleBase<TParagraphPropertiesBasedStyle>, IParagraphPropertiesContainer, ITabPropertiesContainer where TParagraphPropertiesBasedStyle : ParagraphPropertiesBasedStyle<TParagraphPropertiesBasedStyle> {
		#region Fields
		readonly ParagraphProperties paragraphProperties;
		readonly TabProperties tabs;
		FrameProperties frameProperties;
		#endregion
		protected ParagraphPropertiesBasedStyle(DocumentModel documentModel, TParagraphPropertiesBasedStyle parent, string styleName)
			: base(documentModel, parent, styleName) {
			this.paragraphProperties = new ParagraphProperties(this);
			this.tabs = new TabProperties(this);
		}
		public TabProperties Tabs { get { return tabs; } }
		public ParagraphProperties ParagraphProperties { get { return paragraphProperties; } }
		public FrameProperties FrameProperties { get { return frameProperties; } set { frameProperties = value; } }
		public virtual ParagraphProperties GetParagraphProperties(ParagraphFormattingOptions.Mask mask) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			if (ParagraphProperties.UseVal(mask))
				return ParagraphProperties;
			return Parent != null ? Parent.GetParagraphProperties(mask) : null;
		}
		public virtual MergedParagraphProperties GetMergedParagraphProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(ParagraphProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedParagraphProperties());
			return merger.MergedProperties;
		}
		public virtual MergedParagraphProperties GetMergedWithDefaultParagraphProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(GetMergedParagraphProperties());
			merger.Merge(DocumentModel.DefaultParagraphProperties);
			return merger.MergedProperties;
		}
		public TabFormattingInfo GetTabs() {
			if (Parent == null)
				return Tabs.GetTabs();
			return TabFormattingInfo.Merge(Tabs.GetTabs(), Parent.GetTabs());
		}
		public virtual MergedFrameProperties GetMergedFrameProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			FramePropertiesMerger merger = new FramePropertiesMerger(FrameProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedFrameProperties());
			return merger.MergedProperties;
		}
		#region IParagraphPropertiesContainer Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> IParagraphPropertiesContainer.CreateParagraphPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, ParagraphProperties);
		}
		void IParagraphPropertiesContainer.OnParagraphPropertiesChanged() {
			BeginUpdate();
			try {
				NotifyStyleChanged();
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region ITabPropertiesContainer Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ITabPropertiesContainer.CreateTabPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, Tabs);
		}
		void ITabPropertiesContainer.OnTabPropertiesChanged() {
		}
		#endregion
		protected override void MergePropertiesWithParent() {
			ParagraphProperties.Merge(Parent.ParagraphProperties);
			FrameProperties.Merge(Parent.FrameProperties);
		}
		protected internal override void CopyProperties(TParagraphPropertiesBasedStyle source) {
			ParagraphProperties.CopyFrom(source.ParagraphProperties.Info);
			if (FrameProperties != null && source.FrameProperties != null)
				FrameProperties.CopyFrom(source.FrameProperties.Info);
		}
	}
	#endregion
	#region ParagraphStyle
	public class ParagraphStyle : ParagraphPropertiesBasedStyle<ParagraphStyle>, ICharacterPropertiesContainer {
		#region Fields
		bool autoUpdate;
		readonly CharacterProperties characterProperties;
		NumberingListIndex numberingListIndex = NumberingListIndex.ListIndexNotSetted;
		int listLevelIndex;
		ParagraphStyle nextParagraphStyle;
		#endregion
		public ParagraphStyle(DocumentModel documentModel)
			: this(documentModel, null) {
		}
		public ParagraphStyle(DocumentModel documentModel, ParagraphStyle parent)
			: this(documentModel, parent, String.Empty) {
		}
		public ParagraphStyle(DocumentModel documentModel, ParagraphStyle parent, string styleName)
			: base(documentModel, parent, styleName) {
			this.characterProperties = new CharacterProperties(this);
			SubscribeCharacterPropertiesEvents();
			SubscribeParagraphPropertiesEvents();
		}
		#region Properties
		#region AutoUpdate
		public bool AutoUpdate {
			get { return autoUpdate; }
			set {
				if (AutoUpdate == value)
					return;
				ParagraphStyleChangeAutoUpdatePropertyHistoryItem item = new ParagraphStyleChangeAutoUpdatePropertyHistoryItem(this);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		internal void SetAutoUpdateCore(bool autoUpdate) {
			this.autoUpdate = autoUpdate;
		}
		#endregion
		#region NextParagraphStyle
		public ParagraphStyle NextParagraphStyle {
			get { return nextParagraphStyle; }
			set {
				if (NextParagraphStyle == value)
					return;
				ParagraphStyleChangeNextParagraphStylePropertyHistoryItem item = new ParagraphStyleChangeNextParagraphStylePropertyHistoryItem(this, NextParagraphStyle, value);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		#endregion
		public bool ContainsNumberingList() {
			return GetNumberingListIndex() >= NumberingListIndex.MinValue;
		}
		public void SetNextParagraphStyleCore(ParagraphStyle value) {
			nextParagraphStyle = value;
		}
		public bool HasLinkedStyle { get { return DocumentModel.StyleLinkManager.HasLinkedStyle(this); } }
		public CharacterStyle LinkedStyle { get { return DocumentModel.StyleLinkManager.GetLinkedStyle(this); } }
		public override StyleType Type { get { return StyleType.ParagraphStyle; } }
		public CharacterProperties CharacterProperties { get { return characterProperties; } }
		#endregion
		public virtual CharacterProperties GetCharacterProperties(CharacterFormattingOptions.Mask mask) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			if (CharacterProperties.UseVal(mask))
				return CharacterProperties;
			return Parent != null ? Parent.GetCharacterProperties(mask) : null;
		}
		public virtual MergedCharacterProperties GetMergedCharacterProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(CharacterProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedCharacterProperties());
			return merger.MergedProperties;
		}
		public virtual MergedCharacterProperties GetMergedWithDefaultCharacterProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(GetMergedCharacterProperties());
			merger.Merge(DocumentModel.DefaultCharacterProperties);
			return merger.MergedProperties;
		}
		#region ICharacterPropertiesContainer Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICharacterPropertiesContainer.CreateCharacterPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, CharacterProperties);
		}
		void ICharacterPropertiesContainer.OnCharacterPropertiesChanged() {
			BeginUpdate();
			try {
				NotifyStyleChanged();
				if (HasLinkedStyle)
					LinkedStyle.CharacterProperties.CopyFrom(CharacterProperties);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		public virtual NumberingListIndex GetNumberingListIndex() {
			if (numberingListIndex >= NumberingListIndex.MinValue || numberingListIndex == NumberingListIndex.NoNumberingList || Parent == null)
				return numberingListIndex;
			else
				return Parent.GetNumberingListIndex();
		}
		public virtual NumberingListIndex GetOwnNumberingListIndex() {
			return numberingListIndex;
		}
		public virtual int GetListLevelIndex() {
			if (numberingListIndex >= NumberingListIndex.MinValue)
				return GetOwnListLevelIndex();
			if (numberingListIndex == NumberingListIndex.NoNumberingList || Parent == null)
				return 0;
			else
				return Parent.GetListLevelIndex();
		}
		public int GetOwnListLevelIndex() {
			return listLevelIndex;
		}
		public virtual void SetNumberingListIndex(NumberingListIndex numberingListIndex) {
			this.numberingListIndex = numberingListIndex;
		}
		public virtual void SetNumberingListLevelIndex(int listLevelIndex) {
			this.listLevelIndex = listLevelIndex;
		}
		protected override void MergePropertiesWithParent() {
			base.MergePropertiesWithParent();
			CharacterProperties.Merge(Parent.CharacterProperties);
		}
		protected internal virtual void SubscribeCharacterPropertiesEvents() {
			CharacterProperties.ObtainAffectedRange += OnObtainAffectedRange;
		}
		protected internal virtual void SubscribeParagraphPropertiesEvents() {
			ParagraphProperties.ObtainAffectedRange += OnObtainAffectedRange;
		}
		protected internal virtual void OnObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			e.Start = RunIndex.Zero;
			e.End = RunIndex.MaxValue;
		}
		protected internal virtual ParagraphStyle CopyFrom(DocumentModel targetModel) {
			ParagraphStyle style = new ParagraphStyle(targetModel);
			CopyTo(style);
			return style;
		}
		protected internal void CopyTo(ParagraphStyle style) {
			style.ParagraphProperties.CopyFrom(this.ParagraphProperties.Info);
			if (this.FrameProperties != null) {
				if (style.FrameProperties == null)
					style.FrameProperties = new FrameProperties(style);
				style.FrameProperties.CopyFrom(this.FrameProperties.Info);
			}
			style.CharacterProperties.CopyFrom(this.characterProperties.Info);
			style.AutoUpdate = this.AutoUpdate;
			style.Tabs.CopyFrom(this.Tabs.Info);
			style.StyleName = this.StyleName;
			if (GetOwnNumberingListIndex() >= new NumberingListIndex(0))
				style.SetNumberingListIndex(DocumentModel.GetNumberingListIndex(style.DocumentModel, this.GetOwnNumberingListIndex()));
			if (GetOwnListLevelIndex() >= 0)
				style.SetNumberingListLevelIndex(this.GetOwnListLevelIndex());
			if (Parent != null)
				style.Parent = style.DocumentModel.ParagraphStyles[Parent.Copy(style.DocumentModel)];
			ParagraphProperties.ApplyPropertiesDiff(style.ParagraphProperties, style.GetMergedWithDefaultParagraphProperties().Info, this.GetMergedWithDefaultParagraphProperties().Info);
			CharacterProperties.ApplyPropertiesDiff(style.CharacterProperties, style.GetMergedWithDefaultCharacterProperties().Info, this.GetMergedWithDefaultCharacterProperties().Info);
		}
		public override int Copy(DocumentModel targetModel) {
			for (int i = 0; i < targetModel.ParagraphStyles.Count; i++) {
				if (this.StyleName == targetModel.ParagraphStyles[i].StyleName)
					return i;
			}
			ParagraphStyle copy = CopyFrom(targetModel);
			int result = targetModel.ParagraphStyles.AddNewStyle(copy);
			if (NextParagraphStyle != null)
				copy.NextParagraphStyle = targetModel.ParagraphStyles[NextParagraphStyle.Copy(targetModel)];
			return result;
		}
		protected internal override void CopyProperties(ParagraphStyle source) {
			CharacterProperties.CopyFrom(source.CharacterProperties.Info);
			base.CopyProperties(source);
		}
		public override void ResetCachedIndices(ResetFormattingCacheType resetFormattingCacheType) {
		}
	}
	#endregion
	#region ParagraphStyleCollection
	public class ParagraphStyleCollection : StyleCollectionBase<ParagraphStyle> {
		public const int EmptyParagraphStyleIndex = 0;
		public static readonly string DefaultParagraphStyleName = "Normal";
		public ParagraphStyleCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		protected internal override ParagraphStyle CreateDefaultItem() {
			return new ParagraphStyle(DocumentModel, null, DefaultParagraphStyleName);
		}
		protected override void NotifyDocumentStyleDeleting(ParagraphStyle style) {
			base.NotifyDocumentStyleDeleting(style);
			for (int i = 0; i < this.Count; i++) {
				if (this[i].NextParagraphStyle == style)
					this[i].NextParagraphStyle = null;
			}
		}
		protected override void NotifyPieceTableStyleDeleting(PieceTable pieceTable, ParagraphStyle style) {
			ParagraphCollection paragraphs = pieceTable.Paragraphs;
			ParagraphIndex count = new ParagraphIndex(paragraphs.Count);
			for (ParagraphIndex i = ParagraphIndex.Zero; i < count; i++) {
				if (paragraphs[i].ParagraphStyle == style)
					paragraphs[i].ParagraphStyleIndex = this.DefaultItemIndex;
			}
		}
	}
	#endregion
}
