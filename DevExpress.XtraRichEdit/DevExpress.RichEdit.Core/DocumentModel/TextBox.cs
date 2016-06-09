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
using System.Drawing;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
namespace DevExpress.XtraRichEdit.Model {
	#region TextBoxFloatingObjectContent
	public class TextBoxFloatingObjectContent : FloatingObjectContent, ITextBoxPropertiesContainer {
		readonly TextBoxContentType textBox;
		readonly TextBoxProperties textBoxProperties;
		public TextBoxFloatingObjectContent(FloatingObjectAnchorRun anchor, TextBoxContentType content)
			: base(anchor) {
			this.textBox = content;
			this.textBoxProperties = new TextBoxProperties(this);
		}
		public TextBoxContentType TextBox { get { return textBox; } }
		public override Size OriginalSize { get { return new Size(1000, 1000); } }
		public TextBoxProperties TextBoxProperties { get { return textBoxProperties; } }
		protected internal override void SetOriginalSize(Size value) {
		}
		protected internal virtual void SetAnchorRun(FloatingObjectAnchorRun anchor) {
			this.textBox.AnchorRun = anchor;
		}
		public override FloatingObjectContent Clone(FloatingObjectAnchorRun run, DocumentModelCopyManager copyManager) {
			TextBoxContentType contentTextBox = new TextBoxContentType(run.DocumentModel);
			using (DocumentModel tempDocumentModel = run.DocumentModel.CreateNew()) {				
				tempDocumentModel.IntermediateModel = true;
				tempDocumentModel.FieldOptions.CopyFrom(run.DocumentModel.FieldOptions);
				PieceTable textBoxPieceTable = TextBox.PieceTable;
				DocumentModelCopyOptions options = new DocumentModelCopyOptions(textBoxPieceTable.DocumentStartLogPosition, textBoxPieceTable.DocumentEndLogPosition - textBoxPieceTable.DocumentStartLogPosition + 1);
				options.DefaultPropertiesCopyOptions = DefaultPropertiesCopyOptions.Always;
				DocumentModelCopyCommand copyCommand = new DocumentModelCopyCommand(textBoxPieceTable, tempDocumentModel, options);
				copyCommand.Execute();
				API.Native.InsertOptions insertOptions = GetInsertOptions(copyManager.FormattingCopyOptions);
				PieceTableInsertContentConvertedToDocumentModelCommand command = new PieceTableInsertContentConvertedToDocumentModelCommand(contentTextBox.PieceTable, tempDocumentModel, DocumentLogPosition.Zero, insertOptions, false);
				command.SuppressFieldsUpdate = true;
				command.Execute();
				contentTextBox.PieceTable.FixLastParagraph();
			}
			TextBoxFloatingObjectContent result = new TextBoxFloatingObjectContent(run, contentTextBox);
			result.TextBoxProperties.CopyFrom(TextBoxProperties.Info);
			return result;
		}
		API.Native.InsertOptions GetInsertOptions(FormattingCopyOptions formattingCopyOptions) {
			switch(formattingCopyOptions) {
				case FormattingCopyOptions.KeepSourceFormatting:
					return API.Native.InsertOptions.KeepSourceFormatting;
				case FormattingCopyOptions.UseDestinationStyles:
					return API.Native.InsertOptions.MatchDestinationFormatting;
				default:
					Debug.Assert(false);
					return API.Native.InsertOptions.MatchDestinationFormatting;
			}
		}
		#region ITextBoxPropertiesContainer Members
		PieceTable ITextBoxPropertiesContainer.PieceTable { get { return Run.PieceTable; } }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ITextBoxPropertiesContainer.CreateTextBoxChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(Run.PieceTable, TextBoxProperties);
		}
		void ITextBoxPropertiesContainer.OnTextBoxChanged() {
		}
		#endregion
	}
	#endregion    
	#region TextBoxContentType
	public class TextBoxContentType : ContentTypeBase {
		FloatingObjectAnchorRun anchorRun;
		public TextBoxContentType(DocumentModel documentModel)
			: base(documentModel) {
			documentModel.UnsafeEditor.InsertFirstParagraph(PieceTable);
			PieceTable.SpellCheckerManager = documentModel.MainPieceTable.SpellCheckerManager.CreateInstance(PieceTable);
			PieceTable.SpellCheckerManager.Initialize();
		}
		public TextBoxContentType(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override bool IsMain { get { return false; } }
		public override bool IsTextBox { get { return true; } }
		public override bool IsReferenced { get { return AnchorRun != null; } }
		protected internal FloatingObjectAnchorRun AnchorRun { get { return anchorRun; } set { anchorRun = value; } }
		protected internal virtual RunIndex CalculateAnchorPieceTableStartRunIndex(RunIndex runIndex) {
			return AnchorRun.Paragraph.FirstRunIndex;
		}
		protected internal virtual RunIndex CalculateAnchorPieceTableEndRunIndex(RunIndex runIndex) {
			return AnchorRun.Paragraph.LastRunIndex;
		}
		protected internal override SectionIndex LookupSectionIndexByParagraphIndex(ParagraphIndex paragraphIndex) {
			return SectionIndex.DontCare;
		}
		protected internal override void ApplyChanges(DocumentModelChangeType changeType, RunIndex startRunIndex, RunIndex endRunIndex) {
			if (!IsReferenced)
				base.ApplyChanges(changeType, startRunIndex, endRunIndex);
			else {
				AnchorRun.PieceTable.ApplyChanges(changeType, CalculateAnchorPieceTableStartRunIndex(startRunIndex), CalculateAnchorPieceTableEndRunIndex(endRunIndex));
			}
		}
		protected internal override void ApplyChangesCore(DocumentModelChangeActions actions, RunIndex startRunIndex, RunIndex endRunIndex) {
			if (!IsReferenced) {
				base.ApplyChangesCore(actions, startRunIndex, endRunIndex);
				return;
			}
			if ((actions & DocumentModelChangeActions.SplitRunByCharset) != 0) { 
				base.ApplyChangesCore(DocumentModelChangeActions.SplitRunByCharset, startRunIndex, endRunIndex);
				actions &= ~DocumentModelChangeActions.SplitRunByCharset;
			}
			base.ApplyChangesCore(actions, CalculateAnchorPieceTableStartRunIndex(startRunIndex), CalculateAnchorPieceTableEndRunIndex(endRunIndex));
		}
		protected internal override void FixLastParagraphOfLastSection(int originalParagraphCount) {
		}
	}
	#endregion
	#region TextBoxInfo
	public class TextBoxInfo : ICloneable<TextBoxInfo>, ISupportsCopyFrom<TextBoxInfo>, ISupportsSizeOf {
		#region Fields
		int leftMargin;
		int rightMargin;
		int topMargin;
		int bottomMargin;
		bool resizeShapeToFitText;
		bool wrapText;
		bool upright;
		VerticalAlignment verticalAlignment;
		#endregion
		#region Properties
		public int LeftMargin { get { return leftMargin; } set { leftMargin = value; } }
		public int RightMargin { get { return rightMargin; } set { rightMargin = value; } }
		public int TopMargin { get { return topMargin; } set { topMargin = value; } }
		public int BottomMargin { get { return bottomMargin; } set { bottomMargin = value; } }
		public bool ResizeShapeToFitText { get { return resizeShapeToFitText; } set { resizeShapeToFitText = value; } }
		public bool WrapText { get { return wrapText; } set { wrapText = value; } }
		public VerticalAlignment VerticalAlignment { get { return verticalAlignment; } set { verticalAlignment = value; } }
		public bool Upright { get { return upright; } set { upright = value; } }
		#endregion
		public TextBoxInfo Clone() {
			TextBoxInfo result = new TextBoxInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(TextBoxInfo val) {
			this.LeftMargin = val.LeftMargin;
			this.RightMargin = val.RightMargin;
			this.TopMargin = val.TopMargin;
			this.BottomMargin = val.BottomMargin;
			this.ResizeShapeToFitText = val.ResizeShapeToFitText;
			this.WrapText = val.WrapText;
			this.VerticalAlignment = val.VerticalAlignment;
			this.Upright = val.Upright;
		}
		public override bool Equals(object obj) {
			TextBoxInfo info = (TextBoxInfo)obj;
			return
				this.LeftMargin == info.LeftMargin &&
				this.RightMargin == info.RightMargin &&
				this.TopMargin == info.TopMargin &&
				this.BottomMargin == info.BottomMargin &&
				this.ResizeShapeToFitText == info.ResizeShapeToFitText &&
				this.WrapText == info.WrapText &&
				this.Upright == info.Upright &&
				this.VerticalAlignment == info.VerticalAlignment;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region TextBoxInfoCache
	public class TextBoxInfoCache : UniqueItemsCache<TextBoxInfo> {
		internal const int DefaultItemIndex = 0;
		public TextBoxInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override TextBoxInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			TextBoxInfo item = new TextBoxInfo();
			item.WrapText = true;
			item.LeftMargin = unitConverter.DocumentsToModelUnits(30); 
			item.RightMargin = unitConverter.DocumentsToModelUnits(30); 
			item.TopMargin = unitConverter.DocumentsToModelUnits(15); 
			item.BottomMargin = unitConverter.DocumentsToModelUnits(15); 
			return item;
		}
	}
	#endregion
	#region TextBoxOptions
	public class TextBoxOptions : ICloneable<TextBoxOptions>, ISupportsCopyFrom<TextBoxOptions>, ISupportsSizeOf {
		#region Mask enumeration
		public enum Mask {
			UseNone = 0x00000000,
			UseLeftMargin = 0x00000001,
			UseRightMargin = 0x00000002,
			UseTopMargin = 0x00000004,
			UseBottomMargin = 0x00000008,
			UseResizeShapeToFitText = 0x00000010,
			UseWrapText = 0x00000020,
			UseVerticalAlignment = 0x00000040,
			UseUpright = 0x00000080,
			UseAll = 0x7FFFFFFF
		}
		#endregion
		Mask val = Mask.UseNone;
		#region Properties
		internal Mask Value { get { return val; } set { val = value; } }
		public bool UseLeftMargin { get { return GetVal(Mask.UseLeftMargin); } set { SetVal(Mask.UseLeftMargin, value); } }
		public bool UseRightMargin { get { return GetVal(Mask.UseRightMargin); } set { SetVal(Mask.UseRightMargin, value); } }
		public bool UseTopMargin { get { return GetVal(Mask.UseTopMargin); } set { SetVal(Mask.UseTopMargin, value); } }
		public bool UseBottomMargin { get { return GetVal(Mask.UseBottomMargin); } set { SetVal(Mask.UseBottomMargin, value); } }
		public bool UseResizeShapeToFitText { get { return GetVal(Mask.UseResizeShapeToFitText); } set { SetVal(Mask.UseResizeShapeToFitText, value); } }
		public bool UseWrapText { get { return GetVal(Mask.UseWrapText); } set { SetVal(Mask.UseWrapText, value); } }
		public bool UseVerticalAlignment { get { return GetVal(Mask.UseVerticalAlignment); } set { SetVal(Mask.UseVerticalAlignment, value); } }
		public bool UseUpright { get { return GetVal(Mask.UseUpright); } set { SetVal(Mask.UseUpright, value); } }
		#endregion
		#region GetVal/SetVal helpers
		void SetVal(Mask mask, bool bitVal) {
			if (bitVal)
				val |= mask;
			else
				val &= ~mask;
		}
		public bool GetVal(Mask mask) {
			return (val & mask) != 0;
		}
		#endregion
		public TextBoxOptions() {
		}
		internal TextBoxOptions(Mask val) {
			this.val = val;
		}
		public TextBoxOptions Clone() {
			return new TextBoxOptions(this.val);
		}
		public override bool Equals(object obj) {
			TextBoxOptions opts = (TextBoxOptions)obj;
			return opts.Value == this.Value;
		}
		public override int GetHashCode() {
			return (int)this.Value;
		}
		public void CopyFrom(TextBoxOptions options) {
			this.val = options.Value;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region TextBoxOptionsCache
	public class TextBoxOptionsCache : UniqueItemsCache<TextBoxOptions> {
		internal const int EmptyTextBoxOptionIndex = 0;
		public TextBoxOptionsCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override TextBoxOptions CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new TextBoxOptions();
		}
	}
	#endregion
	#region TextBoxFormatting
	public class TextBoxFormatting : IndexBasedObjectB<TextBoxInfo, TextBoxOptions>, ICloneable<TextBoxFormatting>, ISupportsCopyFrom<TextBoxFormatting> {
		internal TextBoxFormatting(PieceTable pieceTable, DocumentModel documentModel, int infoIndex, int optionsIndex)
			: base(pieceTable, documentModel, infoIndex, optionsIndex) {
		}
		protected override UniqueItemsCache<TextBoxInfo> InfoCache { get { return ((DocumentModel)DocumentModel).Cache.TextBoxInfoCache; } }
		protected override UniqueItemsCache<TextBoxOptions> OptionsCache { get { return ((DocumentModel)DocumentModel).Cache.TextBoxOptionsCache; } }
		#region LeftMargin
		public int LeftMargin {
			get { return Info.LeftMargin; }
			set {
				if (Info.LeftMargin == value && Options.UseLeftMargin)
					return;
				SetPropertyValue(SetLeftMarginCore, value, SetUseLeftMarginCore);
			}
		}
		void SetLeftMarginCore(TextBoxInfo info, int value) {
			info.LeftMargin = value;
		}
		void SetUseLeftMarginCore(TextBoxOptions options, bool value) {
			options.UseLeftMargin = value;
		}
		#endregion
		#region RightMargin
		public int RightMargin {
			get { return Info.RightMargin; }
			set {
				if (Info.RightMargin == value && Options.UseRightMargin)
					return;
				SetPropertyValue(SetRightMarginCore, value, SetUseRightMarginCore);
			}
		}
		void SetRightMarginCore(TextBoxInfo info, int value) {
			info.RightMargin = value;
		}
		void SetUseRightMarginCore(TextBoxOptions options, bool value) {
			options.UseRightMargin = value;
		}
		#endregion
		#region TopMargin
		public int TopMargin {
			get { return Info.TopMargin; }
			set {
				if (Info.TopMargin == value && Options.UseTopMargin)
					return;
				SetPropertyValue(SetTopMarginCore, value, SetUseTopMarginCore);
			}
		}
		void SetTopMarginCore(TextBoxInfo info, int value) {
			info.TopMargin = value;
		}
		void SetUseTopMarginCore(TextBoxOptions options, bool value) {
			options.UseTopMargin = value;
		}
		#endregion
		#region BottomMargin
		public int BottomMargin {
			get { return Info.BottomMargin; }
			set {
				if (Info.BottomMargin == value && Options.UseBottomMargin)
					return;
				SetPropertyValue(SetBottomMarginCore, value, SetUseBottomMarginCore);
			}
		}
		void SetBottomMarginCore(TextBoxInfo info, int value) {
			info.BottomMargin = value;
		}
		void SetUseBottomMarginCore(TextBoxOptions options, bool value) {
			options.UseBottomMargin = value;
		}
		#endregion
		#region ResizeShapeToFitText
		public bool ResizeShapeToFitText {
			get { return Info.ResizeShapeToFitText; }
			set {
				if (Info.ResizeShapeToFitText == value && Options.UseResizeShapeToFitText)
					return;
				SetPropertyValue(SetResizeShapeToFitTextCore, value, SetUseResizeShapeToFitTextCore);
			}
		}
		void SetResizeShapeToFitTextCore(TextBoxInfo info, bool value) {
			info.ResizeShapeToFitText = value;
		}
		void SetUseResizeShapeToFitTextCore(TextBoxOptions options, bool value) {
			options.UseResizeShapeToFitText = value;
		}
		#endregion
		#region WrapText
		public bool WrapText {
			get { return Info.WrapText; }
			set {
				if (Info.WrapText == value && Options.UseWrapText)
					return;
				SetPropertyValue(SetWrapTextCore, value, SetUseWrapTextCore);
			}
		}
		void SetWrapTextCore(TextBoxInfo info, bool value) {
			info.WrapText = value;
		}
		void SetUseWrapTextCore(TextBoxOptions options, bool value) {
			options.UseWrapText = value;
		}
		#endregion
		#region VerticalAlignment
		public VerticalAlignment VerticalAlignment {
			get { return Info.VerticalAlignment; }
			set {
				if (Info.VerticalAlignment == value && Options.UseVerticalAlignment)
					return;
				SetPropertyValue(SetVerticalAlignmentCore, value, SetUseVerticalAlignmentCore);
			}
		}
		void SetVerticalAlignmentCore(TextBoxInfo info, VerticalAlignment value) {
			info.VerticalAlignment = value;
		}
		void SetUseVerticalAlignmentCore(TextBoxOptions options, bool value) {
			options.UseVerticalAlignment = value;
		}
		#endregion
		#region Upright
		public bool Upright {
			get { return Info.Upright; }
			set {
				if (Info.Upright == value && Options.UseUpright)
					return;
				SetPropertyValue(SetUprightCore, value, SetUseUprightCore);
			}
		}
		void SetUprightCore(TextBoxInfo info, bool value) {
			info.Upright = value;
		}
		void SetUseUprightCore(TextBoxOptions options, bool value) {
			options.UseUpright = value;
		}
		#endregion
		#region ICloneable<TextBoxFormatting> Members
		public TextBoxFormatting Clone() {
			return new TextBoxFormatting(PieceTable, (DocumentModel)DocumentModel, InfoIndex, OptionsIndex);
		}
		#endregion
		public void CopyFrom(TextBoxFormatting TextBoxFormatting) {
			CopyFrom(TextBoxFormatting.Info, TextBoxFormatting.Options);
		}
		public void CopyFrom(TextBoxInfo info, TextBoxOptions options) {
			CopyFromCore(info, options);
		}
		protected override bool PropertyEquals(IndexBasedObject<TextBoxInfo, TextBoxOptions> other) {
			Guard.ArgumentNotNull(other, "other");
			return Options.Value == other.Options.Value &&
				Info.Equals(other.Info);
		}
		protected override void SetPropertyValue<U>(IndexBasedObjectB<TextBoxInfo, TextBoxOptions>.SetPropertyValueDelegate<U> setter, U newValue, IndexBasedObjectB<TextBoxInfo, TextBoxOptions>.SetOptionsValueDelegate optionsSetter) {
			if (((DocumentModel)DocumentModel).DocumentCapabilities.FloatingObjectsAllowed)
				base.SetPropertyValue<U>(setter, newValue, optionsSetter);
		}
	}
	#endregion
	#region TextBoxFormattingCache
	public class TextBoxFormattingCache : UniqueItemsCache<TextBoxFormatting> {
		#region Fields
		public const int EmptyTextBoxFormattingIndex = 0;
		readonly DocumentModel documentModel;
		#endregion
		public TextBoxFormattingCache(DocumentModel documentModel)
			: base(documentModel.UnitConverter) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			AppendItem(new TextBoxFormatting(DocumentModel.MainPieceTable, DocumentModel, 0, TextBoxOptionsCache.EmptyTextBoxOptionIndex));
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		protected override TextBoxFormatting CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return null;
		}
	}
	#endregion
	#region ITextBoxPropertiesContainer
	public interface ITextBoxPropertiesContainer {
		PieceTable PieceTable { get; }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateTextBoxChangedHistoryItem();
		void OnTextBoxChanged();
	}
	#endregion
	#region TextBoxProperties
	public class TextBoxProperties : RichEditIndexBasedObject<TextBoxFormatting> {
		readonly ITextBoxPropertiesContainer owner;
		public TextBoxProperties(ITextBoxPropertiesContainer owner)
			: base(GetPieceTable(owner)) {
			this.owner = owner;
		}
		static PieceTable GetPieceTable(ITextBoxPropertiesContainer owner) {
			Guard.ArgumentNotNull(owner, "owner");
			return owner.PieceTable;
		}
		#region Properties
		#region LeftMargin
		public int LeftMargin {
			get { return Info.LeftMargin; }
			set {
				if (Info.LeftMargin == value && UseLeftMargin)
					return;
				SetPropertyValue(SetLeftMarginCore, value);
			}
		}
		public bool UseLeftMargin { get { return Info.Options.UseLeftMargin; } }
		protected internal virtual DocumentModelChangeActions SetLeftMarginCore(TextBoxFormatting info, int value) {
			info.LeftMargin = value;
			return TextBoxPropertiesChangeActionsCalculator.CalculateChangeActions(TextBoxPropertiesChangeType.LeftMargin);
		}
		#endregion
		#region RightMargin
		public int RightMargin {
			get { return Info.RightMargin; }
			set {
				if (Info.RightMargin == value && UseRightMargin)
					return;
				SetPropertyValue(SetRightMarginCore, value);
			}
		}
		public bool UseRightMargin { get { return Info.Options.UseRightMargin; } }
		protected internal virtual DocumentModelChangeActions SetRightMarginCore(TextBoxFormatting info, int value) {
			info.RightMargin = value;
			return TextBoxPropertiesChangeActionsCalculator.CalculateChangeActions(TextBoxPropertiesChangeType.RightMargin);
		}
		#endregion
		#region TopMargin
		public int TopMargin {
			get { return Info.TopMargin; }
			set {
				if (Info.TopMargin == value && UseTopMargin)
					return;
				SetPropertyValue(SetTopMarginCore, value);
			}
		}
		public bool UseTopMargin { get { return Info.Options.UseTopMargin; } }
		protected internal virtual DocumentModelChangeActions SetTopMarginCore(TextBoxFormatting info, int value) {
			info.TopMargin = value;
			return TextBoxPropertiesChangeActionsCalculator.CalculateChangeActions(TextBoxPropertiesChangeType.TopMargin);
		}
		#endregion
		#region BottomMargin
		public int BottomMargin {
			get { return Info.BottomMargin; }
			set {
				if (Info.BottomMargin == value && UseBottomMargin)
					return;
				SetPropertyValue(SetBottomMarginCore, value);
			}
		}
		public bool UseBottomMargin { get { return Info.Options.UseBottomMargin; } }
		protected internal virtual DocumentModelChangeActions SetBottomMarginCore(TextBoxFormatting info, int value) {
			info.BottomMargin = value;
			return TextBoxPropertiesChangeActionsCalculator.CalculateChangeActions(TextBoxPropertiesChangeType.BottomMargin);
		}
		#endregion
		#region ResizeShapeToFitText
		public bool ResizeShapeToFitText {
			get { return Info.ResizeShapeToFitText; }
			set {
				if (Info.ResizeShapeToFitText == value && UseResizeShapeToFitText)
					return;
				SetPropertyValue(SetResizeShapeToFitTextCore, value);
			}
		}
		public bool UseResizeShapeToFitText { get { return Info.Options.UseResizeShapeToFitText; } }
		protected internal virtual DocumentModelChangeActions SetResizeShapeToFitTextCore(TextBoxFormatting info, bool value) {
			info.ResizeShapeToFitText = value;
			return TextBoxPropertiesChangeActionsCalculator.CalculateChangeActions(TextBoxPropertiesChangeType.ResizeShapeToFitText);
		}
		#endregion
		#region WrapText
		public bool WrapText {
			get { return Info.WrapText; }
			set {
				if (Info.WrapText == value && UseWrapText)
					return;
				SetPropertyValue(SetWrapTextCore, value);
			}
		}
		public bool UseWrapText { get { return Info.Options.UseWrapText; } }
		protected internal virtual DocumentModelChangeActions SetWrapTextCore(TextBoxFormatting info, bool value) {
			info.WrapText = value;
			return TextBoxPropertiesChangeActionsCalculator.CalculateChangeActions(TextBoxPropertiesChangeType.WrapText);
		}
		#endregion
		#region VerticalAlignment
		public VerticalAlignment VerticalAlignment {
			get { return Info.VerticalAlignment; }
			set {
				if (Info.VerticalAlignment == value && UseVerticalAlignment)
					return;
				SetPropertyValue(SetVerticalAlignmentCore, value);
			}
		}
		public bool UseVerticalAlignment { get { return Info.Options.UseVerticalAlignment; } }
		protected internal virtual DocumentModelChangeActions SetVerticalAlignmentCore(TextBoxFormatting info, VerticalAlignment value) {
			info.VerticalAlignment = value;
			return TextBoxPropertiesChangeActionsCalculator.CalculateChangeActions(TextBoxPropertiesChangeType.VerticalAlignment);
		}
		#endregion
		#region Upright
		public bool Upright {
			get { return Info.Upright; }
			set {
				if (Info.Upright == value && UseUpright)
					return;
				SetPropertyValue(SetUprightCore, value);
			}
		}
		public bool UseUpright { get { return Info.Options.UseUpright; } }
		protected internal virtual DocumentModelChangeActions SetUprightCore(TextBoxFormatting info, bool value) {
			info.Upright = value;
			return TextBoxPropertiesChangeActionsCalculator.CalculateChangeActions(TextBoxPropertiesChangeType.Upright);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<TextBoxFormatting> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.TextBoxFormattingCache;
		}
		protected internal bool UseVal(TextBoxOptions.Mask mask) {
			return (Info.Options.Value & mask) != 0;
		}
		public void Reset() {
			TextBoxFormatting info = GetInfoForModification();
			TextBoxFormatting emptyInfo = GetCache(DocumentModel)[TextBoxFormattingCache.EmptyTextBoxFormattingIndex];
			info.ReplaceInfo(emptyInfo.Info, emptyInfo.Options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		public override bool Equals(object obj) {
			TextBoxProperties other = obj as TextBoxProperties;
			if (ReferenceEquals(obj, null))
				return false;
			if (DocumentModel == other.DocumentModel)
				return Index == other.Index;
			else
				return Info.Equals(other.Info);
		}
		internal void ResetUse(TextBoxOptions.Mask mask) {
			TextBoxFormatting info = GetInfoForModification();
			TextBoxOptions options = info.GetOptionsForModification();
			options.Value &= ~mask;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		internal void ResetAllUse() {
			TextBoxFormatting info = GetInfoForModification();
			TextBoxOptions options = info.GetOptionsForModification();
			options.Value = TextBoxOptions.Mask.UseNone;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		public override int GetHashCode() {
			return Index;
		}
#if DEBUGTEST
		public override string ToString() {
			return String.Format("TextBoxInfoIndex:{0}, InfoIndex:{1}, OptionsIndex:{2}", Index, Info.InfoIndex, Info.OptionsIndex);
		}
#endif
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return TextBoxPropertiesChangeActionsCalculator.CalculateChangeActions(TextBoxPropertiesChangeType.BatchUpdate);
		}
		protected override void OnIndexChanged() {
			base.OnIndexChanged();
			owner.OnTextBoxChanged();
		}
		protected override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateIndexChangedHistoryItem() {
			return owner.CreateTextBoxChangedHistoryItem();
		}
	}
	#endregion
	#region TextBoxPropertiesChangeType
	public enum TextBoxPropertiesChangeType {
		None = 0,
		LeftMargin,
		RightMargin,
		TopMargin,
		BottomMargin,
		ResizeShapeToFitText,
		WrapText,
		VerticalAlignment,
		Upright,
		BatchUpdate
	}
	#endregion
	#region TextBoxPropertiesChangeActionsCalculator
	public static class TextBoxPropertiesChangeActionsCalculator {
		internal class TextBoxPropertiesChangeActionsTable : Dictionary<TextBoxPropertiesChangeType, DocumentModelChangeActions> {
		}
		internal static readonly TextBoxPropertiesChangeActionsTable textBoxPropertiesChangeActionsTable = CreateTextBoxPropertiesChangeActionsTable();
		internal static TextBoxPropertiesChangeActionsTable CreateTextBoxPropertiesChangeActionsTable() {
			TextBoxPropertiesChangeActionsTable table = new TextBoxPropertiesChangeActionsTable();
			table.Add(TextBoxPropertiesChangeType.None, DocumentModelChangeActions.None);
			table.Add(TextBoxPropertiesChangeType.LeftMargin, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TextBoxPropertiesChangeType.RightMargin, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TextBoxPropertiesChangeType.TopMargin, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TextBoxPropertiesChangeType.BottomMargin, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TextBoxPropertiesChangeType.ResizeShapeToFitText, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TextBoxPropertiesChangeType.WrapText, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TextBoxPropertiesChangeType.VerticalAlignment, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TextBoxPropertiesChangeType.Upright, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(TextBoxPropertiesChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(TextBoxPropertiesChangeType change) {
			return textBoxPropertiesChangeActionsTable[change];
		}
	}
	#endregion
}
