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
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.Office.Model;
using System.Diagnostics;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region ListNumberAlignment
	public enum ListNumberAlignment {
		Left = 0,
		Center = 1,
		Right = 2
	}
	#endregion
	#region IListLevelProperties
	public interface IListLevelProperties : IReadOnlyListLevelProperties {
		new int Start { get; set; }
		new NumberingFormat Format { get; set; }
		new ListNumberAlignment Alignment { get; set; }
		new bool ConvertPreviousLevelNumberingToDecimal { get; set; }
		new char Separator { get; set; }
		new bool SuppressRestart { get; set; }
		new bool SuppressBulletResize { get; set; }
		new string DisplayFormatString { get; set; }
		new int RelativeRestartLevel { get; set; }
		new int TemplateCode { get; set; }
		new bool Legacy { get; set; }
		new int LegacyIndent { get; set; }
		new int LegacySpace { get; set; }
	}
	#endregion
	#region IReadOnlyListLevel
	public interface IReadOnlyListLevelProperties {
		int Start { get; }
		NumberingFormat Format { get; }
		ListNumberAlignment Alignment { get; }
		bool ConvertPreviousLevelNumberingToDecimal { get; }
		char Separator { get; }
		bool SuppressRestart { get; }
		bool SuppressBulletResize { get; }
		string DisplayFormatString { get; }
		int RelativeRestartLevel { get;}
		int TemplateCode { get; }
		int OriginalLeftIndent { get; }
		bool Legacy { get; }
		int LegacyIndent { get; }
		int LegacySpace { get; }
	}
	#endregion
	internal interface IInternalListLevel : IListLevel {
		int FontCacheIndex { get; }
	}
	#region IListLevel
	public interface IListLevel : ICharacterProperties, IParagraphProperties {
		ParagraphProperties ParagraphProperties { get; }
		CharacterProperties CharacterProperties { get; }
		ListLevelProperties ListLevelProperties { get; }
		bool BulletLevel { get; }
		DocumentModel DocumentModel { get; }
		ParagraphStyle ParagraphStyle { get; }
	}
	#endregion
	#region ListLevelInfo
	public class ListLevelInfo : ICloneable<ListLevelInfo>, ISupportsCopyFrom<ListLevelInfo>, IListLevelProperties, IReadOnlyListLevelProperties, ISupportsSizeOf {
		#region Fields
		int start;
		NumberingFormat format;
		ListNumberAlignment alignment;
		bool convertPreviousLevelNumberingToDecimal;
		char separator;
		bool suppressRestart;
		bool suppressBulletResize;
		string displayFormatString = String.Empty; 
		int relativeRestartLevel;
		int templateCode;
		int originalLeftIndent;
		bool legacy;
		int legacySpace;
		int legacyIndent;
		#endregion
		#region Properties
		public int Start { get { return start; } set { start = value; } }
		public NumberingFormat Format { get { return format; } set { format = value; } }
		public ListNumberAlignment Alignment { get { return alignment; } set { alignment = value; } }
		public bool ConvertPreviousLevelNumberingToDecimal { get { return convertPreviousLevelNumberingToDecimal; } set { convertPreviousLevelNumberingToDecimal = value; } }
		public char Separator { get { return separator; } set { separator = value; } }
		public bool SuppressRestart { get { return suppressRestart; } set { suppressRestart = value; } }
		public bool SuppressBulletResize { get { return suppressBulletResize; } set { suppressBulletResize = value; } }
		public string DisplayFormatString { get { return displayFormatString; } set { displayFormatString = value; } }
		public int RelativeRestartLevel { get { return relativeRestartLevel; } set { relativeRestartLevel = value; } }
		public int TemplateCode { get { return templateCode; } set { templateCode = value; } }
		protected internal int OriginalLeftIndent { get { return originalLeftIndent; } set { originalLeftIndent = value; } }
		int IReadOnlyListLevelProperties.OriginalLeftIndent { get { return this.OriginalLeftIndent; } }
		protected internal bool HasTemplateCode { get { return TemplateCode != 0; } }
		public bool Legacy { get { return legacy; } set { legacy = value; } }
		public int LegacySpace { get { return legacySpace; } set { legacySpace = value; } }
		public int LegacyIndent { get { return legacyIndent; } set { legacyIndent = value; } }
		#endregion
		#region ICloneable<ListLevelInfo> Members
		public ListLevelInfo Clone() {
			ListLevelInfo clone = new ListLevelInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		protected internal virtual void InitializeAsDefault() {
			this.Start = 1;
			this.Separator = Characters.TabMark;
			this.DisplayFormatString = "{0}.";
			this.RelativeRestartLevel = 0;
		}
		public override bool Equals(object obj) {
			ListLevelInfo info = (ListLevelInfo)obj;
			return
				this.Start == info.Start &&
				this.Format == info.Format &&
				this.Alignment == info.Alignment &&
				this.ConvertPreviousLevelNumberingToDecimal == info.ConvertPreviousLevelNumberingToDecimal &&
				this.Separator == info.Separator &&
				this.SuppressRestart == info.SuppressRestart &&
				this.SuppressBulletResize == info.SuppressBulletResize &&
				this.DisplayFormatString == info.DisplayFormatString &&
				this.RelativeRestartLevel == info.RelativeRestartLevel &&
				this.HasTemplateCode == info.HasTemplateCode &&
				this.OriginalLeftIndent == info.OriginalLeftIndent &&
				this.Legacy == info.Legacy &&
				this.LegacyIndent == info.LegacyIndent &&
				this.LegacySpace == info.LegacySpace &&
				this.TemplateCode == info.TemplateCode;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public void CopyFrom(ListLevelInfo info) {
			this.Start = info.Start;
			this.Format = info.Format;
			this.Alignment = info.Alignment;
			this.ConvertPreviousLevelNumberingToDecimal = info.ConvertPreviousLevelNumberingToDecimal;
			this.Separator = info.Separator;
			this.SuppressRestart = info.SuppressRestart;
			this.SuppressBulletResize = info.SuppressBulletResize;
			this.DisplayFormatString = info.DisplayFormatString;
			this.RelativeRestartLevel = info.RelativeRestartLevel;
			this.templateCode = info.TemplateCode;
			this.OriginalLeftIndent = info.OriginalLeftIndent;
			this.Legacy = info.Legacy;
			this.LegacySpace = info.LegacySpace;
			this.LegacyIndent = info.LegacyIndent;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region ListLevelInfoCache
	public class ListLevelInfoCache : UniqueItemsCache<ListLevelInfo> {
		public ListLevelInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override ListLevelInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			ListLevelInfo result = new ListLevelInfo();
			result.InitializeAsDefault();
			return result;
		}
	}
	#endregion
	#region ListLevelProperties
	public class ListLevelProperties : RichEditIndexBasedObject<ListLevelInfo>, ICloneable<ListLevelProperties>, IListLevelProperties, IReadOnlyListLevelProperties {
		public ListLevelProperties(DocumentModel documentModel)
			: base(GetMainPieceTable(documentModel)) {
		}
		#region Properties
		#region Start
		[JSONEnum((int)JSONListLevelProperty.Start)]
		public int Start {
			get { return Info.Start; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Start", value);
				if (Start == value)
					return;
				SetPropertyValue(SetStartCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetStartCore(ListLevelInfo listLevel, int value) {
			listLevel.Start = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.Start);
		}
		#endregion
		#region Format
		[JSONEnum((int)JSONListLevelProperty.Format)]
		public NumberingFormat Format {
			get { return Info.Format; }
			set {
				if (Format == value)
					return;
				SetPropertyValue(SetFormatCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetFormatCore(ListLevelInfo listLevel, NumberingFormat value) {
			listLevel.Format = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.Format);
		}
		#endregion
		#region Alignment
		[JSONEnum((int)JSONListLevelProperty.Alignment)]
		public ListNumberAlignment Alignment {
			get { return Info.Alignment; }
			set {
				if (Alignment == value)
					return;
				SetPropertyValue(SetAlignmentCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetAlignmentCore(ListLevelInfo listLevel, ListNumberAlignment value) {
			listLevel.Alignment = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.Alignment);
		}
		#endregion
		#region ConvertPreviousLevelNumberingToDecimal
		[JSONEnum((int)JSONListLevelProperty.ConvertPreviousLevelNumberingToDecimal)]
		public bool ConvertPreviousLevelNumberingToDecimal {
			get { return Info.ConvertPreviousLevelNumberingToDecimal; }
			set {
				if (ConvertPreviousLevelNumberingToDecimal == value)
					return;
				SetPropertyValue(SetConvertPreviousLevelNumberingToDecimalCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetConvertPreviousLevelNumberingToDecimalCore(ListLevelInfo listLevel, bool value) {
			listLevel.ConvertPreviousLevelNumberingToDecimal = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.RelativeRestartLevel);
		}
		#endregion
		#region Separator
		[JSONEnum((int)JSONListLevelProperty.Separator)]
		public char Separator {
			get { return Info.Separator; }
			set {
				if (Separator == value)
					return;
				SetPropertyValue(SetSeparatorCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetSeparatorCore(ListLevelInfo listLevel, char value) {
			listLevel.Separator = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.Separator);
		}
		#endregion
		#region SuppressRestart
		[JSONEnum((int)JSONListLevelProperty.SuppressRestart)]
		public bool SuppressRestart {
			get { return Info.SuppressRestart; }
			set {
				if (SuppressRestart == value)
					return;
				SetPropertyValue(SetSuppressRestartCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetSuppressRestartCore(ListLevelInfo listLevel, bool value) {
			listLevel.SuppressRestart = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.SuppressRestart);
		}
		#endregion
		#region SuppressBulletResize
		[JSONEnum((int)JSONListLevelProperty.SuppressBulletResize)]
		public bool SuppressBulletResize {
			get { return Info.SuppressBulletResize; }
			set {
				if (SuppressBulletResize == value)
					return;
				SetPropertyValue(SetSuppressBulletResizeCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetSuppressBulletResizeCore(ListLevelInfo listLevel, bool value) {
			listLevel.SuppressBulletResize = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.SuppressBulletResize);
		}
		#endregion
		#region DisplayFormatString
		[JSONEnum((int)JSONListLevelProperty.DisplayFormatString)]
		public string DisplayFormatString {
			get { return Info.DisplayFormatString; }
			set {
				if (DisplayFormatString == value)
					return;
				SetPropertyValue(SetDisplayFormatStringCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetDisplayFormatStringCore(ListLevelInfo listLevel, string value) {
			listLevel.DisplayFormatString = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.DisplayFormatString);
		}
		#endregion
		#region RelativeRestartLevel
		[JSONEnum((int)JSONListLevelProperty.RelativeRestartLevel)]
		public int RelativeRestartLevel {
			get { return Info.RelativeRestartLevel; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("RestartLevel", value);
				if (RelativeRestartLevel == value)
					return;
				SetPropertyValue(SetRestartLevelCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetRestartLevelCore(ListLevelInfo listLevel, int value) {
			listLevel.RelativeRestartLevel = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.RelativeRestartLevel);
		}
		#endregion
		#region TemplateCode
		[JSONEnum((int)JSONListLevelProperty.TemplateCode)]
		public int TemplateCode {
			get { return Info.TemplateCode; }
			set {
				if (TemplateCode == value)
					return;
				SetPropertyValue(SetTemplateCode, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetTemplateCode(ListLevelInfo listLevel, int value) {
			listLevel.TemplateCode = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.TemplateCode);
		}
		#endregion
		int IReadOnlyListLevelProperties.OriginalLeftIndent {
			get {
				return this.OriginalLeftIndent;
			}
		}	
		#region OriginalLeftIndent
		[JSONEnum((int)JSONListLevelProperty.OriginalLeftIndent)]
		protected internal int OriginalLeftIndent {
			get { return Info.OriginalLeftIndent; }
			set {
				if (OriginalLeftIndent == value)
					return;
				SetPropertyValue(SetOriginalLeftIndent, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetOriginalLeftIndent(ListLevelInfo listLevel, int value) {
			listLevel.OriginalLeftIndent = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Legacy
		[JSONEnum((int)JSONListLevelProperty.Legacy)]
		public bool Legacy {
			get { return Info.Legacy; }
			set {
				if (Legacy == value)
					return;
				SetPropertyValue(SetLegacyCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetLegacyCore(ListLevelInfo listLevel, bool value) {
			listLevel.Legacy = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.Legacy);
		}
		#endregion
		#region LegacySpace
		[JSONEnum((int)JSONListLevelProperty.LegacySpace)]
		public int LegacySpace {
			get { return Info.LegacySpace; }
			set {
				if (LegacySpace == value)
					return;
				SetPropertyValue(SetLegacySpaceCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetLegacySpaceCore(ListLevelInfo listLevel, int value) {
			listLevel.LegacySpace = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.LegacySpace);
		}
		#endregion
		#region LegacyIndent
		[JSONEnum((int)JSONListLevelProperty.LegacyIndent)]
		public int LegacyIndent {
			get { return Info.LegacyIndent; }
			set {
				if (LegacyIndent == value)
					return;
				SetPropertyValue(SetLegacyIndentCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetLegacyIndentCore(ListLevelInfo listLevel, int value) {
			listLevel.LegacyIndent = value;
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.LegacyIndent);
		}
		#endregion
		#endregion
		internal static PieceTable GetMainPieceTable(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			return documentModel.MainPieceTable;
		}
		protected internal override UniqueItemsCache<ListLevelInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.ListLevelInfoCache;
		}
		#region ICloneable<ListLevel> Members
		public ListLevelProperties Clone() {
			ListLevelProperties clone = new ListLevelProperties(DocumentModel);
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return ListLevelChangeActionsCalculator.CalculateChangeActions(ListLevelChangeType.BatchUpdate);
		}
	}
	public enum JSONListLevelProperty {
		Start = 0,
		Format = 1,
		ConvertPreviousLevelNumberingToDecimal = 2,
		SuppressBulletResize = 3,
		SuppressRestart = 4,
		Alignment = 5,
		DisplayFormatString = 6,
		RelativeRestartLevel = 7,
		Separator = 8,
		TemplateCode = 9,
		OriginalLeftIndent = 10,
		Legacy = 11,
		LegacySpace = 12,
		LegacyIndent = 13
	}
	#endregion
	#region ListLevelDisplayTextHelper
	public static class ListLevelDisplayTextHelper {
		public static string CreateDisplayFormatStringCore(List<int> placeholderIndices, string text) {
			StringBuilder sb = new StringBuilder();
			sb.Append(GetTextRange(placeholderIndices, 0, text));
			int count = placeholderIndices.Count - 1;
			for (int i = 1; i < count; i++) {
				sb.Append('{');
				sb.Append((int)text[placeholderIndices[i]]);
				sb.Append('}');
				sb.Append(GetTextRange(placeholderIndices, i, text));
			}
			return sb.ToString();
		}
		public static string GetTextRange(List<int> placeholderIndices, int startPlaceHolderIndex, string text) {
			int index = placeholderIndices[startPlaceHolderIndex] + 1;
			string result = text.Substring(index, placeholderIndices[startPlaceHolderIndex + 1] - index);
			result = result.Replace("{", "{{");
			result = result.Replace("}", "}}");
			return result;
		}
	}
	#endregion
	#region ListLevel
	public class ListLevel : IInternalListLevel, IParagraphPropertiesContainer, ICharacterPropertiesContainer, ICloneable<ListLevel> {
		const int bulletLevelDisplayFormatStringLength = 1;
		#region Fields
		readonly ParagraphProperties paragraphProperties;
		readonly CharacterProperties characterProperties;
		readonly ListLevelProperties listLevelProperties;
		int paragraphStyleIndex = -1;		
		readonly DocumentModel documentModel;
		int mergedCharacterFormattingCacheIndex = -1;
		int mergedParagraphFormattingCacheIndex = -1;
		int fontCacheIndex = -1;
		#endregion
		public ListLevel(DocumentModel documentModel) {
			this.documentModel = documentModel;
			this.paragraphProperties = new ParagraphProperties(this);
			this.characterProperties = new CharacterProperties(this);
			this.listLevelProperties = new ListLevelProperties(documentModel);
			this.paragraphProperties.ObtainAffectedRange += OnPropertiesObtainAffectedRange;
			this.characterProperties.ObtainAffectedRange += OnPropertiesObtainAffectedRange;
			this.listLevelProperties.ObtainAffectedRange += OnPropertiesObtainAffectedRange;
		}
		#region Properties
		int IInternalListLevel.FontCacheIndex {
			get {
				if (fontCacheIndex < 0)
					fontCacheIndex = documentModel.FontCache.CalcFontIndex(FontName, DoubleFontSize, FontBold, FontItalic, Script, false, false);
				return fontCacheIndex;
			}
		}
		public ParagraphProperties ParagraphProperties { get { return paragraphProperties; }  }
		public CharacterProperties CharacterProperties { get { return characterProperties; }  }
		public ListLevelProperties ListLevelProperties { get { return listLevelProperties; } }
		public int ParagraphStyleIndex { get { return paragraphStyleIndex; } set { paragraphStyleIndex = value; OnParagraphStyleChanged(); } }
		public ParagraphStyle ParagraphStyle { get { return paragraphStyleIndex >= 0 ? DocumentModel.ParagraphStyles[ParagraphStyleIndex] : null; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		public PieceTable PieceTable { get { return documentModel.MainPieceTable; } }
		public bool BulletLevel {
			get { return ListLevelProperties.DisplayFormatString.Length == bulletLevelDisplayFormatStringLength; }
		}
		protected internal int MergedCharacterFormattingCacheIndex {
			get {
				if (mergedCharacterFormattingCacheIndex < 0)
					mergedCharacterFormattingCacheIndex = DocumentModel.Cache.MergedCharacterFormattingInfoCache.GetItemIndex(GetMergedCharacterProperties().Info);
				return mergedCharacterFormattingCacheIndex;
			}
		}
		protected internal int MergedParagraphFormattingCacheIndex {
			get {
				if (mergedParagraphFormattingCacheIndex < 0)
					mergedParagraphFormattingCacheIndex = DocumentModel.Cache.MergedParagraphFormattingInfoCache.GetItemIndex(GetMergedParagraphProperties().Info);
				return mergedParagraphFormattingCacheIndex;
			}
		}
		protected internal CharacterFormattingInfo MergedCharacterFormatting { get { return DocumentModel.Cache.MergedCharacterFormattingInfoCache[MergedCharacterFormattingCacheIndex]; } }
		protected internal ParagraphFormattingInfo MergedParagraphFormatting { get { return DocumentModel.Cache.MergedParagraphFormattingInfoCache[MergedParagraphFormattingCacheIndex]; } }
		#endregion
		#region IListLevel Members
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
		#endregion
		#region IParagraphProperties Members
		public ParagraphAlignment Alignment { get { return MergedParagraphFormatting.Alignment; } set { ParagraphProperties.Alignment = value; } }
		public int LeftIndent { get { return MergedParagraphFormatting.LeftIndent; } set { ParagraphProperties.LeftIndent = value; } }
		public int RightIndent { get { return MergedParagraphFormatting.RightIndent; } set { ParagraphProperties.RightIndent = value; } }
		public int SpacingBefore { get { return MergedParagraphFormatting.SpacingBefore; } set { ParagraphProperties.SpacingBefore = value; } }
		public int SpacingAfter { get { return MergedParagraphFormatting.SpacingBefore; } set { ParagraphProperties.SpacingAfter = value; } }
		public ParagraphLineSpacing LineSpacingType { get { return MergedParagraphFormatting.LineSpacingType; } set { ParagraphProperties.LineSpacingType = value; } }
		public float LineSpacing { get { return MergedParagraphFormatting.LineSpacing; } set { ParagraphProperties.LineSpacing = value; } }
		public ParagraphFirstLineIndent FirstLineIndentType { get { return MergedParagraphFormatting.FirstLineIndentType; } set { ParagraphProperties.FirstLineIndentType = value; } }
		public int FirstLineIndent { get { return MergedParagraphFormatting.FirstLineIndent; } set { ParagraphProperties.FirstLineIndent = value; } }
		public bool SuppressHyphenation { get { return MergedParagraphFormatting.SuppressHyphenation; } set { ParagraphProperties.SuppressHyphenation = value; } }
		public bool SuppressLineNumbers { get { return MergedParagraphFormatting.SuppressLineNumbers; } set { ParagraphProperties.SuppressLineNumbers = value; } }
		public bool ContextualSpacing { get { return MergedParagraphFormatting.ContextualSpacing; } set { ParagraphProperties.ContextualSpacing = value; } }
		public bool PageBreakBefore { get { return MergedParagraphFormatting.PageBreakBefore; } set { ParagraphProperties.PageBreakBefore = value; } }
		public bool BeforeAutoSpacing { get { return MergedParagraphFormatting.BeforeAutoSpacing; } set { ParagraphProperties.BeforeAutoSpacing = value; } }
		public bool AfterAutoSpacing { get { return MergedParagraphFormatting.AfterAutoSpacing; } set { ParagraphProperties.AfterAutoSpacing = value; } }
		public bool KeepWithNext { get { return MergedParagraphFormatting.KeepWithNext; } set { ParagraphProperties.KeepWithNext = value; } }
		public bool KeepLinesTogether { get { return MergedParagraphFormatting.KeepLinesTogether; } set { ParagraphProperties.KeepLinesTogether = value; } }
		public bool WidowOrphanControl { get { return MergedParagraphFormatting.WidowOrphanControl; } set { ParagraphProperties.WidowOrphanControl = value; } }
		public int OutlineLevel { get { return MergedParagraphFormatting.SpacingBefore; } set { ParagraphProperties.OutlineLevel = value; } }
		Color IParagraphProperties.BackColor { get { return MergedParagraphFormatting.BackColor; } set { ParagraphProperties.BackColor = value; } }
		public BorderInfo LeftBorder { get { return MergedParagraphFormatting.LeftBorder; } set { ParagraphProperties.LeftBorder = value; } }
		public BorderInfo RightBorder { get { return MergedParagraphFormatting.RightBorder; } set { ParagraphProperties.RightBorder = value; } }
		public BorderInfo TopBorder { get { return MergedParagraphFormatting.TopBorder; } set { ParagraphProperties.TopBorder = value; } }
		public BorderInfo BottomBorder { get { return MergedParagraphFormatting.BottomBorder; } set { ParagraphProperties.BottomBorder = value; } }
		#endregion
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateCharacterPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, CharacterProperties);
		}
		public void OnCharacterPropertiesChanged() {
			mergedCharacterFormattingCacheIndex = -1;
			fontCacheIndex = -1;
			DocumentModel.ResetDocumentFormattingCaches(ResetFormattingCacheType.Character);
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateParagraphPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, ParagraphProperties);
		}
		public void OnParagraphStyleChanged() {
			mergedParagraphFormattingCacheIndex = -1;
			DocumentModel.ResetDocumentFormattingCaches(ResetFormattingCacheType.All);
		}
		public void OnParagraphPropertiesChanged() {
			mergedParagraphFormattingCacheIndex = -1;
			DocumentModel.ResetDocumentFormattingCaches(ResetFormattingCacheType.Paragraph);
		}
		protected internal virtual void CopyFrom(ListLevel listLevel) {
			ListLevelProperties.CopyFrom(listLevel.ListLevelProperties.Info);
			ParagraphProperties.CopyFrom(listLevel.ParagraphProperties.Info);
			CharacterProperties.CopyFrom(listLevel.CharacterProperties.Info);
			ParagraphStyleIndex = listLevel.ParagraphStyleIndex;
		}
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(obj, this))
				return true;
			ListLevel other = obj as ListLevel;
			if (Object.ReferenceEquals(other, null))
				return false;
			if (
				!this.ListLevelProperties.Info.Equals(other.ListLevelProperties.Info) ||
				!this.CharacterProperties.Info.Equals(other.CharacterProperties.Info) ||
				!this.ParagraphProperties.Equals(other.ParagraphProperties))
				return false;
			if (DocumentModel == other.DocumentModel)
				return this.ParagraphStyleIndex == other.ParagraphStyleIndex;
			else {
				ParagraphStyle thisParagraphStyle = this.ParagraphStyle;
				ParagraphStyle otherParagraphStyle = other.ParagraphStyle;
				if (thisParagraphStyle == null && otherParagraphStyle == null)
					return true;
				if (thisParagraphStyle == null || otherParagraphStyle == null)
					return false;
				return this.ParagraphStyle.StyleName == otherParagraphStyle.StyleName;
			}
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public virtual ListLevel Clone() {
			ListLevel clone = CreateLevel();
			clone.CopyFrom(this);
			return clone;
		}
		protected virtual ListLevel CreateLevel() {
			return new ListLevel(DocumentModel);
		}
		protected internal virtual MergedCharacterProperties GetMergedCharacterProperties() {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(CharacterProperties);
			merger.Merge(DocumentModel.DefaultCharacterProperties);
			return merger.MergedProperties;
		}
		protected internal virtual MergedParagraphProperties GetMergedParagraphProperties() {
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(ParagraphProperties);
			merger.Merge(DocumentModel.DefaultParagraphProperties);
			return merger.MergedProperties;
		}
		protected internal virtual void OnPropertiesObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			e.Start = RunIndex.Zero;
			e.End = RunIndex.MaxValue;
		}
	}
	#endregion
	public class OverrideListLevel : ListLevel, IOverrideListLevel {
		bool overrideStart;
		public OverrideListLevel(DocumentModel documentModel)
			: base(documentModel) {
		}
		public bool OverrideStart { get { return overrideStart; } }
		public int NewStart { get { return ListLevelProperties.Start; } set { ListLevelProperties.Start = value; } }
		protected override ListLevel CreateLevel() {
			return new OverrideListLevel(DocumentModel);
		}
		public virtual void SetOverrideStart(bool value) {
			this.overrideStart = value;
		}
	}
	#region ListLevelChangeType
	public enum ListLevelChangeType {
		None = 0,
		Start,
		Format,
		Alignment,
		ConvertPreviousLevelNumberingToDecimal,
		Separator,
		SuppressRestart,
		SuppressBulletResize,
		DisplayFormatString,
		RelativeRestartLevel,
		TemplateCode,
		Legacy,
		LegacySpace,
		LegacyIndent,
		BatchUpdate
	}
	#endregion
	#region ListLevelChangeActionsCalculator
	public static class ListLevelChangeActionsCalculator {
		internal class ListLevelChangeActionsTable : Dictionary<ListLevelChangeType, DocumentModelChangeActions> {
		}
		internal static readonly ListLevelChangeActionsTable listLevelChangeActionsTable = CreateListLevelChangeActionsTable();
		internal static ListLevelChangeActionsTable CreateListLevelChangeActionsTable() {
			ListLevelChangeActionsTable table = new ListLevelChangeActionsTable();
			table.Add(ListLevelChangeType.None, DocumentModelChangeActions.None);
			table.Add(ListLevelChangeType.Start, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.Format, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.Alignment, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.ConvertPreviousLevelNumberingToDecimal, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.Separator, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.SuppressRestart, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.SuppressBulletResize, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.DisplayFormatString, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.RelativeRestartLevel, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.TemplateCode, DocumentModelChangeActions.None);
			table.Add(ListLevelChangeType.Legacy, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.LegacyIndent, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.LegacySpace, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(ListLevelChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(ListLevelChangeType change) {
			return listLevelChangeActionsTable[change];
		}
	}
	#endregion
	#region NumberingListReferenceLevel
	public class NumberingListReferenceLevel : IInternalListLevel, IOverrideListLevel {
		readonly NumberingList owner;
		int level;
		bool overrideStart;
		int newStart;
		public NumberingListReferenceLevel(NumberingList owner, int level) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNonNegative(level, "level");
			this.owner = owner;
			SetLevelCore(level);
		}
		#region Properties
		public NumberingList Owner { get { return owner; } }
		public int Level { get { return level; } }
		int IInternalListLevel.FontCacheIndex { get { return ((IInternalListLevel)OwnerLevel).FontCacheIndex; } }
		#endregion
		protected internal void SetLevelCore(int level) {
			this.level = level;
		}
		#region IListLevel Members
		protected internal virtual ListLevel OwnerLevel { get { return Owner.AbstractNumberingList.Levels[Level]; } }
		public ParagraphProperties ParagraphProperties { get { return OwnerLevel.ParagraphProperties; } }
		public CharacterProperties CharacterProperties { get { return OwnerLevel.CharacterProperties; } }
		public ListLevelProperties ListLevelProperties { get { return OwnerLevel.ListLevelProperties; } }
		bool IListLevel.BulletLevel { get { return OwnerLevel.BulletLevel; } }
		#endregion
		#region IListLevel Members
		public string FontName { get { return OwnerLevel.FontName; } set { OwnerLevel.FontName = value; } }
		public int DoubleFontSize { get { return OwnerLevel.DoubleFontSize; } set { OwnerLevel.DoubleFontSize = value; } }
		public bool FontBold { get { return OwnerLevel.FontBold; } set { OwnerLevel.FontBold = value; } }
		public bool FontItalic { get { return OwnerLevel.FontItalic; } set { OwnerLevel.FontItalic = value; } }
		public CharacterFormattingScript Script { get { return OwnerLevel.Script; } set { OwnerLevel.Script = value; } }
		public StrikeoutType FontStrikeoutType { get { return OwnerLevel.FontStrikeoutType; } set { OwnerLevel.FontStrikeoutType = value; } }
		public UnderlineType FontUnderlineType { get { return OwnerLevel.FontUnderlineType; } set { OwnerLevel.FontUnderlineType = value; } }
		public bool AllCaps { get { return OwnerLevel.AllCaps; } set { OwnerLevel.AllCaps = value; } }
		public bool UnderlineWordsOnly { get { return OwnerLevel.UnderlineWordsOnly; } set { OwnerLevel.UnderlineWordsOnly = value; } }
		public bool StrikeoutWordsOnly { get { return OwnerLevel.StrikeoutWordsOnly; } set { OwnerLevel.StrikeoutWordsOnly = value; } }
		public Color ForeColor { get { return OwnerLevel.ForeColor; } set { OwnerLevel.ForeColor = value; } }
		public Color BackColor { get { return OwnerLevel.BackColor; } set { OwnerLevel.BackColor = value; } }
		public Color UnderlineColor { get { return OwnerLevel.UnderlineColor; } set { OwnerLevel.UnderlineColor = value; } }
		public Color StrikeoutColor { get { return OwnerLevel.StrikeoutColor; } set { OwnerLevel.StrikeoutColor = value; } }
#if THEMES_EDIT        
		public RichEditFontInfo FontInfo { get { return OwnerLevel.FontInfo; } set { OwnerLevel.FontInfo = value; } }
		public ColorModelInfo ForeColorInfo { get { return OwnerLevel.ForeColorInfo; } set { OwnerLevel.ForeColorInfo = value; } }
		public ColorModelInfo BackColorInfo { get { return OwnerLevel.BackColorInfo; } set { OwnerLevel.BackColorInfo = value; } }
		public ColorModelInfo UnderlineColorInfo { get { return OwnerLevel.UnderlineColorInfo; } set { OwnerLevel.UnderlineColorInfo = value; } }
		public ColorModelInfo StrikeoutColorInfo { get { return OwnerLevel.StrikeoutColorInfo; } set { OwnerLevel.StrikeoutColorInfo = value; } }
		public Shading Shading { get { return OwnerLevel.Shading; } set { OwnerLevel.Shading = value; } }
#endif
		public bool Hidden { get { return OwnerLevel.Hidden; } set { OwnerLevel.Hidden = value; } }
		public LangInfo LangInfo { get { return OwnerLevel.LangInfo; } set { OwnerLevel.LangInfo = value; } }
		public bool NoProof { get { return OwnerLevel.NoProof; } set { OwnerLevel.NoProof = value; } }
		public ParagraphStyle ParagraphStyle { get { return OwnerLevel.ParagraphStyle; } }
		DocumentModel IListLevel.DocumentModel { get { return OwnerLevel.DocumentModel; } }
		#endregion
		#region IParagraphProperties Members
		public ParagraphAlignment Alignment { get { return OwnerLevel.Alignment; } set { OwnerLevel.Alignment = value; } }
		public int LeftIndent { get { return OwnerLevel.LeftIndent; } set { OwnerLevel.LeftIndent = value; } }
		public int RightIndent { get { return OwnerLevel.RightIndent; } set { OwnerLevel.RightIndent = value; } }
		public int SpacingBefore { get { return OwnerLevel.SpacingBefore; } set { OwnerLevel.SpacingBefore = value; } }
		public int SpacingAfter { get { return OwnerLevel.SpacingAfter; } set { OwnerLevel.SpacingAfter = value; } }
		public ParagraphLineSpacing LineSpacingType { get { return OwnerLevel.LineSpacingType; } set { OwnerLevel.LineSpacingType = value; } }
		public float LineSpacing { get { return OwnerLevel.LineSpacing; } set { OwnerLevel.LineSpacing = value; } }
		public ParagraphFirstLineIndent FirstLineIndentType { get { return OwnerLevel.FirstLineIndentType; } set { OwnerLevel.FirstLineIndentType = value; } }
		public int FirstLineIndent { get { return OwnerLevel.FirstLineIndent; } set { OwnerLevel.FirstLineIndent = value; } }
		public bool SuppressHyphenation { get { return OwnerLevel.SuppressHyphenation; } set { OwnerLevel.SuppressHyphenation = value; } }
		public bool SuppressLineNumbers { get { return OwnerLevel.SuppressLineNumbers; } set { OwnerLevel.SuppressLineNumbers = value; } }
		public bool ContextualSpacing { get { return OwnerLevel.ContextualSpacing; } set { OwnerLevel.ContextualSpacing = value; } }
		public bool PageBreakBefore { get { return OwnerLevel.PageBreakBefore; } set { OwnerLevel.PageBreakBefore = value; } }
		public bool BeforeAutoSpacing { get { return OwnerLevel.BeforeAutoSpacing; } set { OwnerLevel.BeforeAutoSpacing = value; } }
		public bool AfterAutoSpacing { get { return OwnerLevel.AfterAutoSpacing; } set { OwnerLevel.AfterAutoSpacing = value; } }
		public bool KeepWithNext { get { return OwnerLevel.KeepWithNext; } set { OwnerLevel.KeepWithNext = value; } }
		public bool KeepLinesTogether { get { return OwnerLevel.KeepLinesTogether; } set { OwnerLevel.KeepLinesTogether = value; } }
		public bool WidowOrphanControl { get { return OwnerLevel.WidowOrphanControl; } set { OwnerLevel.WidowOrphanControl = value; } }
		public int OutlineLevel { get { return OwnerLevel.OutlineLevel; } set { OwnerLevel.OutlineLevel = value; } }
		Color IParagraphProperties.BackColor { get { return OwnerLevel.BackColor; } set { OwnerLevel.BackColor = value; } }
		public BorderInfo LeftBorder { get { return OwnerLevel.LeftBorder; } set { OwnerLevel.LeftBorder = value; } }
		public BorderInfo RightBorder { get { return OwnerLevel.RightBorder; } set { OwnerLevel.RightBorder = value; } }
		public BorderInfo TopBorder { get { return OwnerLevel.TopBorder; } set { OwnerLevel.TopBorder = value; } }
		public BorderInfo BottomBorder { get { return OwnerLevel.BottomBorder; } set { OwnerLevel.BottomBorder = value; } }
		#endregion
		public bool OverrideStart { get { return overrideStart; } }
		public int NewStart { get { return newStart; } set { newStart = value; } }
		public virtual void SetOverrideStart(bool value) {
			this.overrideStart = value;
		}
	}
	#endregion
	#region ListLevelCollection
	public interface IReadOnlyIListLevelCollection {
		int Count { get; }
		IListLevel this[int index] { get; }
	}
	public class ListLevelCollection<T> : IReadOnlyIListLevelCollection where T : IListLevel{
		List<T> innerList;
		public ListLevelCollection() {
			this.innerList = new List<T>();
		}
		public int Count { get { return innerList.Count; } }
		public void Add(T level) {
			this.innerList.Add(level);
		}
		public T this[int index] { get { return innerList[index]; } set { innerList[index] = value; } }
		#region IReadOnlyIListLevelCollection Members
		IListLevel IReadOnlyIListLevelCollection.this[int index] { get { return this[index]; } }		
		#endregion
	}
	#endregion
	#region NumberingType
	public enum NumberingType {
		MultiLevel,
		Simple,
		Bullet
	}
	#endregion
	public interface NumberingListBase {
		IReadOnlyIListLevelCollection Levels { get; }
		DocumentModel DocumentModel { get; }
	}
	#region NumberingListBase
	public abstract class NumberingListBaseImpl<T> : NumberingListBase where T : IListLevel {
		int id;
		readonly ListLevelCollection<T> levels;
		readonly DocumentModel documentModel;
		protected NumberingListBaseImpl(DocumentModel documentModel, int levelCount) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.levels = new ListLevelCollection<T>();
			this.id = -1;
			InitLevels(levelCount);
		}
		#region Properties
		IReadOnlyIListLevelCollection NumberingListBase.Levels { get { return this.Levels; } }
		protected ListLevelCollection<T> Levels { get { return levels; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		#region ID
		public int Id {
			get {
				if (id == -1)
					id = GenerateNewId();
				return id;
			}
		}
		protected internal abstract int GenerateNewId();
		#endregion
		#endregion
		public virtual void SetId(int id) {
			this.id = id;
		}
		public int GetId() {
			return id;
		}
		protected internal virtual void InitLevels(int levelCount) {
			for (int i = 0; i < levelCount; i++) {
				T listLevel = CreateLevel(i);
				levels.Add(listLevel);
			}
		}
		protected internal abstract T CreateLevel(int levelIndex);
		public virtual void CopyFrom(NumberingListBaseImpl<T> list) {
			this.id = list.Id;
			CopyLevelsFrom(list.levels);
		}
		public virtual void CopyFromViaHistory(NumberingListBaseImpl<T> list) {
			this.id = list.Id;
			CopyLevelsFromViaHistory(list.levels);
		}
		public bool IsEqual(NumberingListBaseImpl<T> list) {
			return EqualsLevels(list.levels);
		}
		bool EqualsLevels(ListLevelCollection<T> listLevel) {
			for (int i = 0; i < listLevel.Count; i++) {
				ListLevel level = Levels[i] as ListLevel;
				if (level != null && !level.Equals(listLevel[i]))
					return false;
			}
			return true;
		}
		protected internal abstract void CopyLevelsFrom(ListLevelCollection<T> sourceLevels);
		protected internal abstract void CopyLevelsFromViaHistory(ListLevelCollection<T> sourceLevels);
		public virtual void SetLevel(int levelIndex, T value) {
			Levels[levelIndex] = value;
		}
	}
	#endregion
	public interface IOverrideListLevel : IListLevel {
		bool OverrideStart { get; }
		int NewStart { get; set; }
		void SetOverrideStart(bool value);
	}
	public interface INumberingListBase {
	}
	#region NumberingList
	public class NumberingList : NumberingListBaseImpl<IOverrideListLevel>, INumberingListBase, ICloneable<NumberingList> {
		int referringParagraphsCount;
		AbstractNumberingListIndex abstractNumberingListIndex;
		public NumberingList(DocumentModel documentModel, AbstractNumberingListIndex abstractNumberingListIndex)
			: this(documentModel, abstractNumberingListIndex, 9) {
		}
		public NumberingList(DocumentModel documentModel, AbstractNumberingListIndex abstractNumberingListIndex, int levelCount) 
			: base(documentModel, levelCount) {
			if (abstractNumberingListIndex < AbstractNumberingListIndex.MinValue || abstractNumberingListIndex > new AbstractNumberingListIndex(documentModel.AbstractNumberingLists.Count - 1))
				Exceptions.ThrowArgumentException("abstractNumberingListIndex", abstractNumberingListIndex);
			this.abstractNumberingListIndex = abstractNumberingListIndex;
		}
		public AbstractNumberingListIndex AbstractNumberingListIndex { get { return abstractNumberingListIndex; } }
		public AbstractNumberingList AbstractNumberingList { get { return DocumentModel.AbstractNumberingLists[AbstractNumberingListIndex]; } }
		public new ListLevelCollection<IOverrideListLevel> Levels { get { return base.Levels; } }
		#region ICloneable<NumberingList> Members
		public NumberingList Clone() {
			NumberingList clone = new NumberingList(DocumentModel, abstractNumberingListIndex);
			clone.CopyFrom(this);
			return clone;
		}
		public override void CopyFrom(NumberingListBaseImpl<IOverrideListLevel> list) {
			base.CopyFrom(list);
		}
		public override void CopyFromViaHistory(NumberingListBaseImpl<IOverrideListLevel> list) {
			base.CopyFromViaHistory(list);
		}
		#endregion
		protected internal override IOverrideListLevel CreateLevel(int levelIndex) {
			return new NumberingListReferenceLevel(this, levelIndex);
		}
		protected internal override int GenerateNewId() {
			return DocumentModel.NumberingListIdProvider.GetNextId();
		}
		protected internal override void CopyLevelsFrom(ListLevelCollection<IOverrideListLevel> sourceLevels) {
			int count = sourceLevels.Count;
			for (int i = 0; i < count; i++) {
				OverrideListLevel sourceLevel = sourceLevels[i] as OverrideListLevel;
				if (sourceLevel != null) {
					OverrideListLevel newLevel = new OverrideListLevel(DocumentModel);
					newLevel.CopyFrom(sourceLevel);
					this.Levels[i] = newLevel;
				}
				else {
					IOverrideListLevel newLevel = CreateLevel(i);
					newLevel.SetOverrideStart(sourceLevels[i].OverrideStart);
					newLevel.NewStart = sourceLevels[i].NewStart;
					this.Levels[i] = newLevel;
				}
			}
		}
		protected internal override void CopyLevelsFromViaHistory(ListLevelCollection<IOverrideListLevel> sourceLevels) {
			int count = sourceLevels.Count;
			for (int i = 0; i < count; i++) {
				ChangeListLevelHistoryItem<IOverrideListLevel> item = new ChangeListLevelHistoryItem<IOverrideListLevel>(DocumentModel.ActivePieceTable, this.Levels);
				item.IndexLevel = i;
				item.OldLevel = this.Levels[i];
				OverrideListLevel sourceLevel = sourceLevels[i] as OverrideListLevel;
				if (sourceLevel != null) {
					OverrideListLevel newLevel = new OverrideListLevel(DocumentModel);
					newLevel.CopyFrom(sourceLevel);
					item.NewLevel = newLevel;
				}
				else {
					IOverrideListLevel newLevel = CreateLevel(i);
					newLevel.SetOverrideStart(sourceLevels[i].OverrideStart);
					newLevel.NewStart = sourceLevels[i].NewStart;
					item.NewLevel = newLevel;
				}
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		internal void OnParagraphAdded() {
			this.referringParagraphsCount++;
		}
		internal void OnParagraphRemoved() {
			this.referringParagraphsCount--;
		}
		internal bool CanRemove() {
			return this.referringParagraphsCount <= 0;
		}
	}
	#endregion
	public static class NumberingListHelper {
		static int templateCodeStart = 0x100;
		static int templateCodeEnd = 0x7FFFFFFF;
		static readonly Random randomTemplateCode = new Random();
		public static NumberingType GetLevelType(NumberingListBase numberingList, int levelIndex) {
			IListLevel level = numberingList.Levels[levelIndex];
			if (level.BulletLevel)
				return NumberingType.Bullet;
			if (!IsHybridList(numberingList))
				return NumberingType.MultiLevel;
			else
				return NumberingType.Simple;
		}
		public static NumberingType GetListType(NumberingListBase numberingList) {
			if (!IsHybridList(numberingList))
				return NumberingType.MultiLevel;
			IListLevel level = numberingList.Levels[0];
			if (level.BulletLevel)
				return NumberingType.Bullet;
			else
				return NumberingType.Simple;
		}
		public static bool IsHybridList(NumberingListBase numberingList) {
			IReadOnlyIListLevelCollection levels = numberingList.Levels;
			int count = levels.Count;
			for (int i = 0; i < count; i++)
				if (levels[i].ListLevelProperties.TemplateCode != 0)
					return true;
			return false;
		}
		public static int GenerateNewTemplateCode(DocumentModel documentModel) {
			for (; ; ) {
				int code = randomTemplateCode.Next(templateCodeStart, templateCodeEnd);
				if (IsNewTemplateCode(documentModel, code))
					return code;
			}
		}
		public static bool IsNewTemplateCode(DocumentModel documentModel, int templateCode) {
			ListLevelInfoCache levelInfoCache = documentModel.Cache.ListLevelInfoCache;
			int count = levelInfoCache.Count;
			for (int i = 0; i < count; i++) {
				if (levelInfoCache[i].TemplateCode == templateCode)
					return false;
			}
			return true;
		}
		public static void SetHybridListType(NumberingListBase list) {
			Action<ListLevelProperties> anchorAction = delegate(ListLevelProperties properties) {
				properties.TemplateCode = GenerateNewTemplateCode(list.DocumentModel);
			};
			SetListLevelsProperty(list, delegate(ListLevelProperties properties) { return properties.TemplateCode == 0; }, anchorAction);
		}
		public static void SetSimpleListType(NumberingListBase list) {
			Action<ListLevelProperties> anchorAction = delegate(ListLevelProperties properties) {
				properties.TemplateCode = 0;
			};
			SetListLevelsProperty(list, delegate(ListLevelProperties properties) { return properties.TemplateCode != 0; }, anchorAction);
		}
		public static void SetListLevelsProperty(NumberingListBase list, Predicate<ListLevelProperties> condition, Action<ListLevelProperties> action) {
			int count = list.Levels.Count;
			for (int i = 0; i < count; i++) {
				IListLevel listLevel = list.Levels[i];
				listLevel.ListLevelProperties.BeginInit();
				try {
					if (condition(listLevel.ListLevelProperties)) {
						action(listLevel.ListLevelProperties);
					}
				}
				finally {
					listLevel.ListLevelProperties.EndInit();
				}
			}
		}
		public static void SetListType(NumberingListBase list, NumberingType value) {
			NumberingType currentType = GetListType(list);
			bool wasSimple = currentType != NumberingType.MultiLevel;
			bool wasBullet = currentType == NumberingType.Bullet;
			bool newValueIsMultiLevel = value == NumberingType.MultiLevel;
			bool newValueIsSimple = value == NumberingType.Simple;
			bool newValueIsBullet = value == NumberingType.Bullet;
			if (wasSimple && newValueIsMultiLevel)
				NumberingListHelper.SetSimpleListType(list);
			else if (!newValueIsMultiLevel)
				NumberingListHelper.SetHybridListType(list);
			if (newValueIsBullet) {
				SetListLevelsFormat(list, NumberingFormat.Bullet);
			}
			if (wasBullet && newValueIsSimple) {
				SetListLevelsFormat(list, NumberingFormat.Decimal); 
			}
		}
		public static void SetListLevelsFormat(NumberingListBase list, NumberingFormat format) {
			Action<ListLevelProperties> anchorAction = delegate(ListLevelProperties properties) {
				properties.Format = format;
			};
			SetListLevelsProperty(list, delegate(ListLevelProperties properties) { return true; }, anchorAction);
		}
		public static AbstractNumberingList GetAbstractListByType(AbstractNumberingListCollection listCollection, NumberingType type) {
			AbstractNumberingListIndex index = GetAbstractListIndexByType(listCollection, type);
			return listCollection[index];
		}
		public static AbstractNumberingListIndex GetAbstractListIndexByType(AbstractNumberingListCollection listCollection, NumberingType type) {
			for (AbstractNumberingListIndex i = new AbstractNumberingListIndex(0); i < new AbstractNumberingListIndex(listCollection.Count); i++) {
				if (NumberingListHelper.GetListType(listCollection[i]) == type)
					return i;
			}
			Exceptions.ThrowArgumentException("type", type);
			return new AbstractNumberingListIndex(-1);
		}
	}
	public class AbstractNumberingList : NumberingListBaseImpl<ListLevel>, INumberingListBase {
		bool deleted;
		int numberingStyleReferenceIndex = -1;
		int styleLinkIndex = -1;
		public AbstractNumberingList(DocumentModel documentModel)
			: base(documentModel, 9) {
		}
		public int NumberingStyleReferenceIndex { get { return numberingStyleReferenceIndex; } }
		public int StyleLinkIndex { get { return styleLinkIndex; } }
		public NumberingListStyle StyleLink { get { return styleLinkIndex >= 0 ? DocumentModel.NumberingListStyles[styleLinkIndex] : null; } }
		public NumberingListStyle NumberingStyleReference { get { return numberingStyleReferenceIndex >= 0 ? DocumentModel.NumberingListStyles[numberingStyleReferenceIndex] : null; } }
		public new ListLevelCollection<ListLevel> Levels { get { return GetLevels(); } }
		protected internal int TemplateCode { get { return 0x0409001D; } }
		protected internal bool Deleted { get { return deleted; } set { deleted = value; } }
		protected virtual ListLevelCollection<ListLevel> GetLevels() {
			if (styleLinkIndex < 0 && numberingStyleReferenceIndex >= 0)
				return DocumentModel.NumberingListStyles[numberingStyleReferenceIndex].NumberingList.AbstractNumberingList.Levels;
			else
				return base.Levels;
		}
		protected internal void SetNumberingStyleReferenceIndex(int styleIndex) {
			this.numberingStyleReferenceIndex = styleIndex;
		}
		protected internal void SetStyleLinkIndex(int styleIndex) {
			this.styleLinkIndex = styleIndex;
		}
		protected internal override int GenerateNewId() {
			return DocumentModel.AbstractNumberingListIdProvider.GetNextId();
		}
		protected internal override ListLevel CreateLevel(int levelIndex) {
			return new ListLevel(DocumentModel);
		}
		protected internal override void CopyLevelsFrom(ListLevelCollection<ListLevel> sourceLevels) {
			int count = sourceLevels.Count;
			for (int i = 0; i < count; i++) {
				ListLevel sourceLevel = sourceLevels[i];
				ListLevel newLevel = new ListLevel(DocumentModel);
				newLevel.CopyFrom(sourceLevel);
				this.Levels[i] = newLevel;
			}
		}
		protected internal override void CopyLevelsFromViaHistory(ListLevelCollection<ListLevel> sourceLevels) {
			int count = sourceLevels.Count;
			for (int i = 0; i < count; i++) {
				ChangeListLevelHistoryItem<ListLevel> item = new ChangeListLevelHistoryItem<ListLevel>(DocumentModel.ActivePieceTable, this.Levels);
				item.IndexLevel = i;
				item.OldLevel = this.Levels[i];
				ListLevel sourceLevel = sourceLevels[i];
				ListLevel newLevel = new ListLevel(DocumentModel);
				newLevel.CopyFrom(sourceLevel);
				item.NewLevel = newLevel;
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		#region ICloneable<AbstractNumberingList> Members
		public AbstractNumberingList Clone() {
			AbstractNumberingList clone = CreateNumberingList();
			clone.CopyFrom(this);
			return clone;
		}
		public virtual AbstractNumberingList CreateNumberingList() {
			return new AbstractNumberingList(DocumentModel);
		}
		#endregion
	}	
	#region NumberingListCollection
	public class NumberingListCollection : List<NumberingList, NumberingListIndex> {
	}
	#endregion
	#region AbstractNumberingListCollection
	public class AbstractNumberingListCollection : List<AbstractNumberingList, AbstractNumberingListIndex> {
		public bool HasListOfType(NumberingType listType) {
			int count = Count;
			for (AbstractNumberingListIndex i = new AbstractNumberingListIndex(count - 1); i >= AbstractNumberingListIndex.MinValue; i--) {
				if (NumberingListHelper.GetListType(this[i]) == listType)
					return true;
			}
			return false;
		}
	}
	#endregion
	#region NumberingListIndex
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct NumberingListIndex : IConvertToInt<NumberingListIndex>, IComparable<NumberingListIndex> {
		readonly int m_value;
		public static readonly NumberingListIndex NoNumberingList = new NumberingListIndex(-2);
		public static readonly NumberingListIndex ListIndexNotSetted = new NumberingListIndex(-1);
		public static readonly NumberingListIndex MinValue = new NumberingListIndex(0);
		public static readonly NumberingListIndex MaxValue = new NumberingListIndex(int.MaxValue);
		[System.Diagnostics.DebuggerStepThrough]
		public NumberingListIndex(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is NumberingListIndex) && (this.m_value == ((NumberingListIndex)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static NumberingListIndex operator +(NumberingListIndex index, int value) {
			return new NumberingListIndex(index.m_value + value);
		}
		public static int operator -(NumberingListIndex index1, NumberingListIndex index2) {
			return index1.m_value - index2.m_value;
		}
		public static NumberingListIndex operator -(NumberingListIndex index, int value) {
			return new NumberingListIndex(index.m_value - value);
		}
		public static NumberingListIndex operator ++(NumberingListIndex index) {
			return new NumberingListIndex(index.m_value + 1);
		}
		public static NumberingListIndex operator --(NumberingListIndex index) {
			return new NumberingListIndex(index.m_value - 1);
		}
		public static bool operator <(NumberingListIndex index1, NumberingListIndex index2) {
			return index1.m_value < index2.m_value;
		}
		public static bool operator <=(NumberingListIndex index1, NumberingListIndex index2) {
			return index1.m_value <= index2.m_value;
		}
		public static bool operator >(NumberingListIndex index1, NumberingListIndex index2) {
			return index1.m_value > index2.m_value;
		}
		public static bool operator >=(NumberingListIndex index1, NumberingListIndex index2) {
			return index1.m_value >= index2.m_value;
		}
		public static bool operator ==(NumberingListIndex index1, NumberingListIndex index2) {
			return index1.m_value == index2.m_value;
		}
		public static bool operator !=(NumberingListIndex index1, NumberingListIndex index2) {
			return index1.m_value != index2.m_value;
		}
		#region IConvertToInt<NumberingListIndex> Members
		[System.Diagnostics.DebuggerStepThrough]
		int IConvertToInt<NumberingListIndex>.ToInt() {
			return m_value;
		}
		[System.Diagnostics.DebuggerStepThrough]
		NumberingListIndex IConvertToInt<NumberingListIndex>.FromInt(int value) {
			return new NumberingListIndex(value);
		}
		#endregion
		#region IComparable<NumberingListIndex> Members
		public int CompareTo(NumberingListIndex other) {
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
	#region AbstractNumberingListIndex
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct AbstractNumberingListIndex : IConvertToInt<AbstractNumberingListIndex>, IComparable<AbstractNumberingListIndex> {
		readonly int m_value;
		public static readonly AbstractNumberingListIndex MinValue = new AbstractNumberingListIndex(0);
		public static readonly AbstractNumberingListIndex MaxValue = new AbstractNumberingListIndex(int.MaxValue);
		public static readonly AbstractNumberingListIndex InvalidValue = new AbstractNumberingListIndex(-1);
		[System.Diagnostics.DebuggerStepThrough]
		public AbstractNumberingListIndex(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is AbstractNumberingListIndex) && (this.m_value == ((AbstractNumberingListIndex)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static AbstractNumberingListIndex operator +(AbstractNumberingListIndex index, int value) {
			return new AbstractNumberingListIndex(index.m_value + value);
		}
		public static int operator -(AbstractNumberingListIndex index1, AbstractNumberingListIndex index2) {
			return index1.m_value - index2.m_value;
		}
		public static AbstractNumberingListIndex operator -(AbstractNumberingListIndex index, int value) {
			return new AbstractNumberingListIndex(index.m_value - value);
		}
		public static AbstractNumberingListIndex operator ++(AbstractNumberingListIndex index) {
			return new AbstractNumberingListIndex(index.m_value + 1);
		}
		public static AbstractNumberingListIndex operator --(AbstractNumberingListIndex index) {
			return new AbstractNumberingListIndex(index.m_value - 1);
		}
		public static bool operator <(AbstractNumberingListIndex index1, AbstractNumberingListIndex index2) {
			return index1.m_value < index2.m_value;
		}
		public static bool operator <=(AbstractNumberingListIndex index1, AbstractNumberingListIndex index2) {
			return index1.m_value <= index2.m_value;
		}
		public static bool operator >(AbstractNumberingListIndex index1, AbstractNumberingListIndex index2) {
			return index1.m_value > index2.m_value;
		}
		public static bool operator >=(AbstractNumberingListIndex index1, AbstractNumberingListIndex index2) {
			return index1.m_value >= index2.m_value;
		}
		public static bool operator ==(AbstractNumberingListIndex index1, AbstractNumberingListIndex index2) {
			return index1.m_value == index2.m_value;
		}
		public static bool operator !=(AbstractNumberingListIndex index1, AbstractNumberingListIndex index2) {
			return index1.m_value != index2.m_value;
		}
		#region IConvertToInt<AbstractNumberingListIndex> Members
		[System.Diagnostics.DebuggerStepThrough]
		int IConvertToInt<AbstractNumberingListIndex>.ToInt() {
			return m_value;
		}
		[System.Diagnostics.DebuggerStepThrough]
		AbstractNumberingListIndex IConvertToInt<AbstractNumberingListIndex>.FromInt(int value) {
			return new AbstractNumberingListIndex(value);
		}
		#endregion
		#region IComparable<AbstractNumberingListIndex> Members
		public int CompareTo(AbstractNumberingListIndex other) {
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
	#region NumberingListStyle
	public class NumberingListStyle : StyleBase<NumberingListStyle> {
		#region Fields
		NumberingListIndex numberingListIndex;
		#endregion
		protected internal NumberingListStyle(DocumentModel documentModel, string styleName)
			: this(documentModel, NumberingListIndex.NoNumberingList, styleName) {
		}
		public NumberingListStyle(DocumentModel documentModel, NumberingListIndex numberingListIndex)
			: this(documentModel, numberingListIndex, String.Empty) {
		}
		public NumberingListStyle(DocumentModel documentModel, NumberingListIndex numberingListIndex, string styleName)
			: base(documentModel, null, styleName) {
				if (numberingListIndex < NumberingListIndex.MinValue && numberingListIndex != NumberingListIndex.NoNumberingList)
				Exceptions.ThrowArgumentException("numberingListIndex", numberingListIndex);
			this.numberingListIndex = numberingListIndex;
		}
		public override StyleType Type { get { return StyleType.NumberingListStyle; } }
		public NumberingListIndex NumberingListIndex { get { return numberingListIndex; } }
		public NumberingList NumberingList { get { return DocumentModel.NumberingLists[numberingListIndex]; } }
		internal void SetNumberingListIndex(NumberingListIndex numberingListIndex) {
			this.numberingListIndex = numberingListIndex;
		}
		protected internal virtual NumberingListStyle CopyFrom(DocumentModel targetModel) {
			NumberingListStyle style = new NumberingListStyle(targetModel, numberingListIndex, StyleName);
			Debug.Assert(Parent == null);
			return style;
		}
		public override int Copy(DocumentModel targetModel) {
			for (int i = 0; i < targetModel.NumberingListStyles.Count; i++) {
				if (this.StyleName == targetModel.NumberingListStyles[i].StyleName)
					return i;
			}
			return targetModel.NumberingListStyles.AddNewStyle(CopyFrom(targetModel));
		}
		protected internal override void CopyProperties(NumberingListStyle source) {
			this.numberingListIndex = source.numberingListIndex;
		}
		protected override void MergePropertiesWithParent() {
			Debug.Assert(Parent == null);
		}
		public override void ResetCachedIndices(ResetFormattingCacheType resetFormattingCacheType) {
		}
	}
	#endregion
	#region NumberingListStyleCollection
	public class NumberingListStyleCollection : StyleCollectionBase<NumberingListStyle> {
		public static readonly string DefaultListStyleName = "List";
		public NumberingListStyleCollection(DocumentModel documentModel)
			: base(documentModel) {
		}
		protected internal override NumberingListStyle CreateDefaultItem() {
			return null;
		}
		protected override bool CanDeleteStyle(NumberingListStyle style) {
			return false;
		}
		protected override void NotifyPieceTableStyleDeleting(PieceTable pieceTable, NumberingListStyle style) {
		}
	}
	#endregion
	#region ListIdProviderBase (abstract class)
	public abstract class ListIdProviderBase {
		readonly DocumentModel documentModel;
		protected ListIdProviderBase(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			Reset();
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		public abstract int GetNextId();
		public abstract void Reset();
	}
	#endregion
	#region AbstractNumberingListIdProvider
	public class AbstractNumberingListIdProvider : ListIdProviderBase {
		static readonly Random randomId = new Random();
		public AbstractNumberingListIdProvider(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override void Reset() {
		}
		public override int GetNextId() {
			int id = -1;
			do {
				lock (randomId) {
					id = randomId.Next(0, Int32.MaxValue);
				}
			}
			while (!IsNewId(id));
			return id;
		}
		protected bool IsNewId(int id) {
			AbstractNumberingListCollection lists = DocumentModel.AbstractNumberingLists;
			AbstractNumberingListIndex maxIndex = new AbstractNumberingListIndex(lists.Count - 1);
			for (AbstractNumberingListIndex i = new AbstractNumberingListIndex(0); i <= maxIndex; i++) {
				if (id == lists[i].GetId())
					return false;
			}
			return true;
		}
	}
	#endregion
	#region NumberingListIdProvider
	public class NumberingListIdProvider : ListIdProviderBase {
		int lastId;
		public NumberingListIdProvider(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override void Reset() {
			this.lastId = 0;
		}
		public override int GetNextId() {
			do {
				this.lastId++;
			} while (!IsNewId(this.lastId));
			return lastId;
		}
		bool IsNewId(int id) {
			return id > this.DocumentModel.NumberingLists.Max(list => list.GetId());
		}
	}
	#endregion
	#region NumberingListNotifier
	public static class NumberingListNotifier {
		public static void NotifyParagraphAdded(DocumentModel documentModel, NumberingListIndex index) {
			if (index >= NumberingListIndex.MinValue)
				documentModel.NumberingLists[index].OnParagraphAdded();
		}
		public static void NotifyParagraphRemoved(DocumentModel documentModel, NumberingListIndex index) {
			if (index >= NumberingListIndex.MinValue)
				documentModel.NumberingLists[index].OnParagraphRemoved();
		}
	}
	#endregion
}
