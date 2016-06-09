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
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	public interface IHighlightableTextRun {
		Color BackColor { get; }
	}
	#region TextRunBase (abstract class)
	public abstract class TextRunBase : ICharacterPropertiesContainer, IBatchInit, IBatchUpdateable, ICharacterProperties, IObtainAffectedRangeListener {
		#region Fields
		Paragraph paragraph;
		CharacterProperties characterProperties;
		int startIndex;
		int length = 1;
		int mergedCharacterFormattingCacheIndex = -1;
		int fontCacheIndex = -1;
		int characterStyleIndex;
		RowProcessingFlags rowProcessingFlags;
		#endregion
		protected TextRunBase(Paragraph paragraph)
			: this(paragraph, 0, 1) {
		}
		protected TextRunBase(Paragraph paragraph, int startIndex, int length) {
			Guard.ArgumentNotNull(paragraph, "paragraph");
			this.StartIndex = startIndex;
			this.Length = length;
			this.Paragraph = paragraph;
			this.characterProperties = new CharacterProperties(this);
		}
		#region Properties
		public int FontCacheIndex {
			get {
				if (fontCacheIndex < 0)
					fontCacheIndex = CalculateFontIndexCore(FontName);
				return fontCacheIndex;
			}
		}
		protected internal int CalculateFontIndexCore(string fontName) {
			return Paragraph.DocumentModel.FontCache.CalcFontIndex(fontName, DoubleFontSize, FontBold, FontItalic, Script, false, false);
		}
		protected internal int InnerFontCacheIndex { get { return fontCacheIndex; } set { fontCacheIndex = value; } }
		protected internal int MergedCharacterFormattingCacheIndex {
			get {
				if (mergedCharacterFormattingCacheIndex < 0) {
					mergedCharacterFormattingCacheIndex = Paragraph.DocumentModel.Cache.MergedCharacterFormattingInfoCache.GetItemIndex(GetMergedCharacterProperties().Info);
					rowProcessingFlags = CalculateRowProcessingFlags();
				}
				return mergedCharacterFormattingCacheIndex;
			}
		}
		internal void EnsureMergedCharacterFormattingCacheIndexCalculated(RunMergedCharacterPropertiesCachedResult cachedResult) {
			if (mergedCharacterFormattingCacheIndex < 0) {
				MergedCharacterProperties mergedCharacterProperties = GetMergedCharacterProperties(cachedResult);
				mergedCharacterFormattingCacheIndex = Paragraph.DocumentModel.Cache.MergedCharacterFormattingInfoCache.GetItemIndex(mergedCharacterProperties.Info);
				rowProcessingFlags = CalculateRowProcessingFlags();
			}
		}
		internal int InnerMergedCharacterFormattingCacheIndex { get { return mergedCharacterFormattingCacheIndex; } set { mergedCharacterFormattingCacheIndex = value; } }
		internal CharacterFormattingInfo MergedCharacterFormatting { get { return Paragraph.DocumentModel.Cache.MergedCharacterFormattingInfoCache[MergedCharacterFormattingCacheIndex]; } }
		public int StartIndex {
			get { return startIndex; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("StartIndex", value);
				startIndex = value;
			}
		}
		public virtual int Length {
			get { return length; }
			set {
				if (value <= 0)
					Exceptions.ThrowArgumentException("Length", value);
				length = value;
			}
		}
		public int EndIndex { get { return StartIndex + Length - 1; } }
		public virtual Paragraph Paragraph {
			get { return paragraph; }
			set {
				Guard.ArgumentNotNull(value, "value");
				paragraph = value;
				ResetCachedIndicesCore();
			}
		}
		public abstract bool CanJoinWith(TextRunBase nextRun);
		public virtual int CharacterStyleIndex {
			get { return characterStyleIndex; }
			set {
				Guard.ArgumentNonNegative(value, "value");
				if (characterStyleIndex == value)
					return;
				DocumentModel.BeginUpdate();
				try {
					ChangeCharacterStyleIndexHistoryItem item = new ChangeCharacterStyleIndexHistoryItem(PieceTable, this.GetRunIndex(), characterStyleIndex, value);
					DocumentModel.History.Add(item);
					item.Execute();
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		public CharacterStyle CharacterStyle { get { return DocumentModel.CharacterStyles[CharacterStyleIndex]; } }
		public CharacterProperties CharacterProperties { get { return characterProperties; } }
		protected internal virtual DocumentModel DocumentModel { get { return Paragraph.DocumentModel; } }
		protected internal virtual PieceTable PieceTable { get { return Paragraph.PieceTable; } }
		public string FontName {
			get { return MergedCharacterFormatting.FontName; }
			set {
				CharacterProperties.FontName = value;
			}
		}
#if THEMES_EDIT
		public RichEditFontInfo FontInfo {
			get { return MergedCharacterFormatting.FontInfo; }
			set {
				CharacterProperties.FontInfo = value;
			}
		}
#endif
		public int DoubleFontSize {
			get { return MergedCharacterFormatting.DoubleFontSize; }
			set {
				CharacterProperties.DoubleFontSize = value;
			}
		}
		public bool FontBold {
			get { return MergedCharacterFormatting.FontBold; }
			set {
				CharacterProperties.FontBold = value;
			}
		}
		public bool FontItalic {
			get { return MergedCharacterFormatting.FontItalic; }
			set {
				CharacterProperties.FontItalic = value;
			}
		}
		public CharacterFormattingScript Script {
			get { return MergedCharacterFormatting.Script; }
			set {
				CharacterProperties.Script = value;
			}
		}
		public StrikeoutType FontStrikeoutType {
			get { return MergedCharacterFormatting.FontStrikeoutType; }
			set {
				CharacterProperties.FontStrikeoutType = value;
			}
		}
		public UnderlineType FontUnderlineType {
			get { return MergedCharacterFormatting.FontUnderlineType; }
			set {
				CharacterProperties.FontUnderlineType = value;
			}
		}
		public bool AllCaps {
			get { return MergedCharacterFormatting.AllCaps; }
			set {
				CharacterProperties.AllCaps = value;
			}
		}
		public bool UnderlineWordsOnly {
			get { return MergedCharacterFormatting.UnderlineWordsOnly; }
			set {
				CharacterProperties.UnderlineWordsOnly = value;
			}
		}
		public bool StrikeoutWordsOnly {
			get { return MergedCharacterFormatting.StrikeoutWordsOnly; }
			set {
				CharacterProperties.StrikeoutWordsOnly = value;
			}
		}
		public Color ForeColor {
			get { return MergedCharacterFormatting.ForeColor; }
			set {
				CharacterProperties.ForeColor = value;
			}
		}
		public Color BackColor {
			get { return MergedCharacterFormatting.BackColor; }
			set {
				CharacterProperties.BackColor = value;
			}
		}
		public Color UnderlineColor {
			get { return MergedCharacterFormatting.UnderlineColor; }
			set {
				CharacterProperties.UnderlineColor = value;
			}
		}
		public Color StrikeoutColor {
			get { return MergedCharacterFormatting.StrikeoutColor; }
			set {
				CharacterProperties.StrikeoutColor = value;
			}
		}
#if THEMES_EDIT
		public ColorModelInfo ForeColorInfo {
			get { return MergedCharacterFormatting.ForeColorInfo; }
			set {
				CharacterProperties.ForeColorInfo = value;
			}
		}
		public ColorModelInfo BackColorInfo {
			get { return MergedCharacterFormatting.BackColorInfo; }
			set {
				CharacterProperties.BackColorInfo = value;
			}
		}
		public Shading Shading {
			get { return MergedCharacterFormatting.Shading; }
			set {
				CharacterProperties.Shading = value;
			}
		}
		public ColorModelInfo UnderlineColorInfo {
			get { return MergedCharacterFormatting.UnderlineColorInfo; }
			set {
				CharacterProperties.UnderlineColorInfo = value;
			}
		}
		public ColorModelInfo StrikeoutColorInfo {
			get { return MergedCharacterFormatting.StrikeoutColorInfo; }
			set {
				CharacterProperties.StrikeoutColorInfo = value;
			}
		}
#endif
		public bool Hidden {
			get { return MergedCharacterFormatting.Hidden; }
			set {
				CharacterProperties.Hidden = value;
			}
		}
		public LangInfo LangInfo {
			get { return MergedCharacterFormatting.LangInfo; }
			set {
				CharacterProperties.LangInfo = value;
			}
		}
		public bool NoProof {
			get { return MergedCharacterFormatting.NoProof; }
			set {
				CharacterProperties.NoProof = value;
			}
		}
		protected internal virtual RowProcessingFlags RowProcessingFlags { get { return rowProcessingFlags; } set { rowProcessingFlags = value; } }
		#endregion
		internal void InheritStyleAndFormattingFromCore(TextRunBase run) {
			InheritStyleAndFormattingFromCore(run, false);
		}
		protected internal virtual IRectangularObject GetRectangularObject() {
			return this as IRectangularObject;
		}
		internal void InheritStyleAndFormattingFromCore(TextRunBase run, bool forceVisible) {
			InheritStyleAndFormattingFromCore(run, forceVisible, false);
		}
		internal void InheritStyleAndFormattingFromCore(TextRunBase run, bool forceVisible, bool suppressChangeStyleNotification) {
			Guard.ArgumentNotNull(run, "run");
			this.SetCharacterStyleIndexCore(run.CharacterStyleIndex, suppressChangeStyleNotification);
			this.CharacterProperties.CopyFromCore(run.CharacterProperties);
			this.InnerFontCacheIndex = run.InnerFontCacheIndex;
			this.InnerMergedCharacterFormattingCacheIndex = run.InnerMergedCharacterFormattingCacheIndex;
			InheritRowProcessgFlags(run);
			if (forceVisible && CharacterProperties.UseHidden && CharacterProperties.Hidden) {
				CharacterProperties.ResetUse(CharacterFormattingOptions.Mask.UseHidden);
				this.ResetMergedCharacterFormattingIndex();
			}
		}
		protected virtual void InheritRowProcessgFlags(TextRunBase run) {
			this.RowProcessingFlags = run.RowProcessingFlags & (~RowProcessingFlags.ProcessLayoutDependentText);
		}
		internal void SetCharacterStyleIndexCore(int newStyleIndex) {
			SetCharacterStyleIndexCore(newStyleIndex, false);
		}
		internal void SetCharacterStyleIndexCore(int newStyleIndex, bool suppressChangeStyleNotification) {
			if (this.CharacterStyleIndex == newStyleIndex)
				return;
			this.characterStyleIndex = newStyleIndex;
			ResetCachedIndicesCore();
			if (suppressChangeStyleNotification)
				return;
			RunIndex runIndex = CalculateRunIndex();
			if (runIndex >= RunIndex.Zero)
				PieceTable.ApplyChangesCore(CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.CharacterStyle), runIndex, runIndex);
		}
		RunIndex CalculateRunIndex() {
			for (RunIndex i = Paragraph.FirstRunIndex; i <= Paragraph.LastRunIndex; i++)
				if (PieceTable.Runs[i] == this)
					return i;
			return new RunIndex(-1);
		}
		protected internal virtual MergedCharacterProperties GetMergedCharacterProperties() {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(CharacterProperties);
			merger.Merge(GetParentMergedCharacterProperties());
			return merger.MergedProperties;
		}
		internal bool TryUseMergedCachedResult(RunMergedCharacterPropertiesCachedResult cachedResult) {
			bool shouldUseExistingResult =
				this.Paragraph.TryUseMergedCharacterCachedResult(cachedResult) &&
				cachedResult.CharacterStyleIndex == this.CharacterStyleIndex &&
				cachedResult.CharacterPropertiesIndex == this.CharacterProperties.Index;
			if (shouldUseExistingResult) 
				return true;
			cachedResult.CharacterStyleIndex = this.CharacterStyleIndex;
			cachedResult.CharacterPropertiesIndex = this.CharacterProperties.Index;
			return false;
		}
		protected internal virtual MergedCharacterProperties GetMergedCharacterProperties(RunMergedCharacterPropertiesCachedResult cachedResult) {
			if (TryUseMergedCachedResult(cachedResult))
				return cachedResult.MergedCharacterProperties;
			MergedCharacterProperties result = GetMergedCharacterProperties();
			cachedResult.MergedCharacterProperties = result;
			return result;
		}
		protected internal virtual MergedCharacterProperties GetParentMergedCharacterProperties() {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(CharacterStyle.GetMergedCharacterProperties());
			merger.Merge(Paragraph.GetMergedCharacterProperties());
			return merger.MergedProperties;
		}
		protected internal virtual MergedCharacterProperties GetParentMergedCharacterProperties(bool useSpecialTableStyle, TableStyle tableStyle) {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(CharacterStyle.GetMergedCharacterProperties());
			merger.Merge(Paragraph.GetMergedCharacterProperties(useSpecialTableStyle, tableStyle));
			return merger.MergedProperties;
		}
		internal bool TryUseParentMergedCachedResult(RunMergedCharacterPropertiesCachedResult cachedResult) {
			bool shouldUseExistingResult =
				cachedResult.CharacterStyleIndex == this.CharacterStyleIndex &&
				cachedResult.CharacterPropertiesIndex < 0 &&
				this.Paragraph.TryUseMergedCharacterCachedResult(cachedResult);
			if (shouldUseExistingResult)
				return true;
			cachedResult.CharacterStyleIndex = this.CharacterStyleIndex;
			cachedResult.CharacterPropertiesIndex = -1;
			return false;
		}
		protected internal virtual void ResetFontCacheIndex() {
			fontCacheIndex = -1;
		}
		protected internal virtual void ResetMergedCharacterFormattingIndex() {
			mergedCharacterFormattingCacheIndex = -1;
			rowProcessingFlags = RowProcessingFlags.None;
		}
		protected internal virtual string GetText() {
			return GetTextFast(PieceTable.TextBuffer);
		}
		protected internal virtual string GetTextFast(ChunkedStringBuilder growBuffer) {
			return growBuffer.ToString(StartIndex, Length);
		}
		protected internal virtual string GetRawTextFast(ChunkedStringBuilder growBuffer) {
			return GetTextFast(growBuffer);
		}
		public virtual string GetText(ChunkedStringBuilder growBuffer, int from, int to) {
			return growBuffer.ToString(StartIndex + from, to - from + 1);
		}
		public virtual string GetPlainText(ChunkedStringBuilder growBuffer) {
			return GetTextFast(growBuffer);
		}
		protected internal virtual string GetPlainText(ChunkedStringBuilder growBuffer, int from, int to) {
			return GetText(growBuffer, from, to);
		}
		protected internal string GetNonEmptyText(ChunkedStringBuilder growBuffer) {
			return growBuffer.ToString(StartIndex, Length);
		}
		protected internal abstract void Measure(BoxInfo boxInfo, IObjectMeasurer measurer);
		protected internal abstract bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth, IObjectMeasurer measurer);
		public abstract void Export(IDocumentModelExporter exporter);
		public abstract TextRunBase Copy(DocumentModelCopyManager copyManager);
		protected internal virtual void CopyCharacterPropertiesFrom(DocumentModelCopyManager copyManager, TextRunBase sourceRun) {
			this.CharacterProperties.CopyFrom(sourceRun.CharacterProperties.Info);
			this.CharacterStyleIndex = sourceRun.CharacterStyle.Copy(copyManager.TargetModel);
		}
		public abstract bool CanPlaceCaretBefore { get; }
		#region ICharacterPropertiesContainer Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICharacterPropertiesContainer.CreateCharacterPropertiesChangedHistoryItem() {
			RunIndex runIndex = GetRunIndex();
			return new RunCharacterPropertiesChangedHistoryItem(Paragraph.PieceTable, runIndex);
		}
		public RunIndex GetRunIndex() {
			RunIndex runIndex = Paragraph.GetRunIndex(this);
			if (runIndex >= new RunIndex(0))
				return runIndex;
			runIndex = PieceTable.Runs.IndexOf(this);
			if (runIndex < new RunIndex(0))
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_InvalidSetCharacterProperties);
			return runIndex;
		}
		PieceTable ICharacterPropertiesContainer.PieceTable {
			get { return PieceTable; }
		}
		void ICharacterPropertiesContainer.OnCharacterPropertiesChanged() {
			ResetCachedIndicesCore();
		}
		#endregion
		#region IBatchInit Members
		public void BeginInit() {
			CharacterProperties.BeginInit();
		}
		public void EndInit() {
			CharacterProperties.EndInit();
		}
		public void CancelInit() {
			CharacterProperties.CancelInit();
		}
		#endregion
		#region IBatchUpdateable Members
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper {
			get { return ((IBatchUpdateable)CharacterProperties).BatchUpdateHelper; }
		}
		public void BeginUpdate() {
			CharacterProperties.BeginUpdate();
		}
		public void CancelUpdate() {
			CharacterProperties.CancelUpdate();
		}
		public void EndUpdate() {
			CharacterProperties.EndUpdate();
		}
		public bool IsUpdateLocked {
			get { return ((IBatchUpdateable)CharacterProperties).IsUpdateLocked; }
		}
		#endregion
		public void ResetCharacterProperties() {
			CharacterProperties.Reset();
		}
		#region IObtainAffectedRangeListener Members
		public void NotifyObtainAffectedRange(ObtainAffectedRangeEventArgs args) {
			OnCharacterPropertiesObtainAffectedRange(characterProperties, args);
		}
		#endregion
		protected internal virtual void OnCharacterPropertiesObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			RunIndex thisRunIndex = PieceTable.CalculateRunIndex(this);
			e.Start = thisRunIndex;
			e.End = thisRunIndex;
		}
		internal void ApplyFormatting(InputPosition pos) {
			ApplyFormatting(pos, false);
		}
		internal void ApplyFormatting(InputPosition pos, bool forceVisible) {
			CharacterFormattingBase formatting = pos.CharacterFormatting;
			bool shouldResetUseHidden = forceVisible && formatting.Options.UseHidden && formatting.Info.Hidden;
			if (!shouldResetUseHidden && !IsUpdateLocked) { 
				CharacterProperties.ReplaceInfo(formatting.Clone(), CharacterProperties.GetBatchUpdateChangeActions());
				CharacterStyleIndex = pos.CharacterStyleIndex;
			}
			else {
				BeginUpdate();
				try {
					CharacterProperties.CopyFrom(formatting);
					CharacterStyleIndex = pos.CharacterStyleIndex;
					if (shouldResetUseHidden)
						CharacterProperties.ResetUse(CharacterFormattingOptions.Mask.UseHidden);
				}
				finally {
					EndUpdate();
				}
			}
		}
		internal void ApplyFormatting(CharacterFormattingInfo info, CharacterFormattingOptions options, int styleIndex, bool forceVisible) {
			bool shouldResetUseHidden = forceVisible && options.UseHidden && info.Hidden;
			CharacterFormattingBase formatting = new CharacterFormattingBase(PieceTable, DocumentModel, DocumentModel.Cache.CharacterFormattingInfoCache.GetItemIndex(info), DocumentModel.Cache.CharacterFormattingOptionsCache.GetItemIndex(options));
			if (!shouldResetUseHidden && !IsUpdateLocked) { 
				CharacterProperties.ReplaceInfo(formatting, CharacterProperties.GetBatchUpdateChangeActions());
				CharacterStyleIndex = styleIndex;
			}
			else {
				BeginUpdate();
				try {
					CharacterProperties.CopyFrom(formatting);
					CharacterStyleIndex = styleIndex;
					if (shouldResetUseHidden)
						CharacterProperties.ResetUse(CharacterFormattingOptions.Mask.UseHidden);
				}
				finally {
					EndUpdate();
				}
			}
		}
		protected internal virtual void ResetCachedIndicesCore() {
			ResetFontCacheIndex();
			ResetMergedCharacterFormattingIndex();
		}
		protected internal virtual void ResetCachedIndices(ResetFormattingCacheType resetFromattingCacheType) {
			if ((resetFromattingCacheType & ResetFormattingCacheType.Character) != 0)
				ResetCachedIndicesCore();
		}
		protected internal virtual RowProcessingFlags CalculateRowProcessingFlags() {
			RowProcessingFlags result = RowProcessingFlags.None;
			CharacterFormattingInfo formatting = MergedCharacterFormatting;
			if (formatting.FontUnderlineType != UnderlineType.None || formatting.FontStrikeoutType != StrikeoutType.None)
				result |= RowProcessingFlags.ProcessCharacterLines;
			if (formatting.Hidden)
				result |= RowProcessingFlags.ProcessHiddenText;
			if (!DXColor.IsTransparentOrEmpty(formatting.BackColor))
				result |= RowProcessingFlags.ProcessTextHighlight;
			return result;
		}
		protected internal virtual void BeforeRunRemoved() {
		}
		protected internal virtual void AfterRunInserted() {
		}
	}
	#endregion
	#region TextRunCollection
	public class TextRunCollection : List<TextRunBase, RunIndex> {
	}
	#endregion
	#region RunIndex
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct RunIndex : IConvertToInt<RunIndex>, IComparable<RunIndex> {
		readonly int m_value;
		public static RunIndex DontCare = new RunIndex(-1);
		public static RunIndex MinValue = new RunIndex(0);
		public static RunIndex Zero = new RunIndex(0);
		public static RunIndex MaxValue = new RunIndex(int.MaxValue);
		[System.Diagnostics.DebuggerStepThrough]
		public RunIndex(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is RunIndex) && (this.m_value == ((RunIndex)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static RunIndex operator +(RunIndex index, int value) {
			return new RunIndex(index.m_value + value);
		}
		public static int operator -(RunIndex index1, RunIndex index2) {
			return index1.m_value - index2.m_value;
		}
		public static RunIndex operator -(RunIndex index, int value) {
			return new RunIndex(index.m_value - value);
		}
		public static RunIndex operator ++(RunIndex index) {
			return new RunIndex(index.m_value + 1);
		}
		public static RunIndex operator --(RunIndex index) {
			return new RunIndex(index.m_value - 1);
		}
		public static bool operator <(RunIndex index1, RunIndex index2) {
			return index1.m_value < index2.m_value;
		}
		public static bool operator <=(RunIndex index1, RunIndex index2) {
			return index1.m_value <= index2.m_value;
		}
		public static bool operator >(RunIndex index1, RunIndex index2) {
			return index1.m_value > index2.m_value;
		}
		public static bool operator >=(RunIndex index1, RunIndex index2) {
			return index1.m_value >= index2.m_value;
		}
		public static bool operator ==(RunIndex index1, RunIndex index2) {
			return index1.m_value == index2.m_value;
		}
		public static bool operator !=(RunIndex index1, RunIndex index2) {
			return index1.m_value != index2.m_value;
		}
		#region IConvertToInt<RunIndex> Members
		[System.Diagnostics.DebuggerStepThrough]
		int IConvertToInt<RunIndex>.ToInt() {
			return m_value;
		}
		[System.Diagnostics.DebuggerStepThrough]
		RunIndex IConvertToInt<RunIndex>.FromInt(int value) {
			return new RunIndex(value);
		}
		#endregion
		#region IComparable<RunIndex> Members
		public int CompareTo(RunIndex other) {
			if (m_value < other.m_value)
				return -1;
			if (m_value > other.m_value)
				return 1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
}
