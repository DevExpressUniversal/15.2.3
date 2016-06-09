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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Globalization;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
using DevExpress.Xpf;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region ICharacterPropertiesContainer
	public interface ICharacterPropertiesContainer {
		PieceTable PieceTable { get; }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateCharacterPropertiesChangedHistoryItem();
		void OnCharacterPropertiesChanged();
	}
	#endregion
	#region ICharacterProperties
	public interface ICharacterProperties  {
		string FontName { get; set; }
		int DoubleFontSize { get; set; }
		bool FontBold { get; set; }
		bool FontItalic { get; set; }
		StrikeoutType FontStrikeoutType { get; set; }
		UnderlineType FontUnderlineType { get; set; }
		bool AllCaps { get; set; }
		bool UnderlineWordsOnly { get; set; }
		bool StrikeoutWordsOnly { get; set; }
		Color ForeColor { get; set; }
		Color BackColor { get; set; }
		Color UnderlineColor { get; set; }
		Color StrikeoutColor { get; set; }
#if THEMES_EDIT
		RichEditFontInfo FontInfo { get; set; }
		ColorModelInfo ForeColorInfo { get; set; }
		ColorModelInfo BackColorInfo { get; set; }
		ColorModelInfo UnderlineColorInfo { get; set; }
		ColorModelInfo StrikeoutColorInfo { get; set; }   
		Shading Shading { get; set; }
#endif
		CharacterFormattingScript Script { get; set; }
		bool Hidden { get; set; }
		LangInfo LangInfo { get; set; }
		bool NoProof { get; set; }
	}
	#endregion
	#region CharacterFormattingInfo
	[UseWebFontInfoCache]
	public class CharacterFormattingInfo : ICloneable<CharacterFormattingInfo>, ISupportsCopyFrom<CharacterFormattingInfo>, ICharacterProperties, ISupportsSizeOf {
		#region Fields
		const uint MaskUnderlineType = 0x0000001F; 
		const uint MaskStrikeoutType = 0x00000060; 
		const uint MaskCharacterFormattingScript = 0x00000180; 
		const uint MaskFontBold = 0x00000200;
		const uint MaskFontItalic = 0x00000400;
		const uint MaskAllCaps = 0x00000800;
		const uint MaskUnderlineWordsOnly = 0x00001000;
		const uint MaskStrikeoutWordsOnly = 0x00002000;
		const uint MaskHidden = 0x00004000;
		const uint MaskNoProof = 0x00008000;
		const uint MaskDoubleFontSize = 0xFFFF0000u; 
		string fontName;
		uint packedValues;
		Color foreColor;
		Color backColor;
		Color underlineColor;
		Color strikeoutColor;
#if THEMES_EDIT
		RichEditFontInfo fontInfo;
		ColorModelInfo foreColorInfo;
		ColorModelInfo backColorInfo;
		ColorModelInfo underlineColorInfo;
		ColorModelInfo strikeoutColorInfo;
		Shading shading;
#endif
		#endregion
		public CharacterFormattingInfo() {
			this.fontName = String.Empty;
			this.foreColor = DXColor.Empty;
			this.backColor = DXColor.Transparent;
			this.underlineColor = DXColor.Empty;
			this.strikeoutColor = DXColor.Empty;
#if THEMES_EDIT
			this.fontInfo = new RichEditFontInfo();
			this.foreColorInfo = ColorModelInfo.Create(DXColor.Empty);
			this.backColorInfo = ColorModelInfo.Create(DXColor.Empty);
			this.underlineColorInfo = ColorModelInfo.Create(DXColor.Empty);
			this.strikeoutColorInfo = ColorModelInfo.Create(DXColor.Empty);
			this.shading = Shading.Create();
#endif
		}
		#region Properties
		[JSONSupports("fontInfoIndex", "this.fontInfoCache.AddItem(info.FontName)", "this.fontInfoCache.GetItem((int)source[((int)JSONCharacterFormattingProperty.FontInfoIndex).ToString()]).Name")]
		[JSONEnum((int)JSONCharacterFormattingProperty.FontInfoIndex)]
		public string FontName { get { return fontName; } set { fontName = value; } }
		[JSONSupports("fontSize", "info.DoubleFontSize / 2.0f", "(int)(Convert.ToDouble(source[((int)JSONCharacterFormattingProperty.FontSize).ToString()]) * 2)")]
		[JSONEnum((int)JSONCharacterFormattingProperty.FontSize)]
		public int DoubleFontSize {
			get { return (int)((packedValues & MaskDoubleFontSize) >> 16); }
			set {
				if (value <= 0)
					Exceptions.ThrowArgumentException("FontSize", value);
				packedValues &= ~MaskDoubleFontSize;
				packedValues |= ((uint)value << 16) & MaskDoubleFontSize;
			}
		}
		[JSONEnum((int)JSONCharacterFormattingProperty.FontBold)]
		public bool FontBold { get { return GetBooleanValue(MaskFontBold); } set { SetBooleanValue(MaskFontBold, value); } }
		[JSONEnum((int)JSONCharacterFormattingProperty.FontItalic)]
		public bool FontItalic { get { return GetBooleanValue(MaskFontItalic); } set { SetBooleanValue(MaskFontItalic, value); } }
		[JSONEnum((int)JSONCharacterFormattingProperty.FontStrikeoutType)]
		public StrikeoutType FontStrikeoutType {
			get { return (StrikeoutType)((packedValues & MaskStrikeoutType) >> 5); }
			set {
				packedValues &= ~MaskStrikeoutType;
				packedValues |= ((uint)value << 5) & MaskStrikeoutType;
			}
		}
		[JSONEnum((int)JSONCharacterFormattingProperty.FontUnderlineType)]
		public UnderlineType FontUnderlineType {
			get { return (UnderlineType)(packedValues & MaskUnderlineType); }
			set {
				packedValues &= ~MaskUnderlineType;
				packedValues |= (uint)value & MaskUnderlineType;
			}
		}
		[JSONEnum((int)JSONCharacterFormattingProperty.AllCaps)]
		public bool AllCaps { get { return GetBooleanValue(MaskAllCaps); } set { SetBooleanValue(MaskAllCaps, value); } }
		[JSONEnum((int)JSONCharacterFormattingProperty.UnderlineWordsOnly)]
		public bool UnderlineWordsOnly { get { return GetBooleanValue(MaskUnderlineWordsOnly); } set { SetBooleanValue(MaskUnderlineWordsOnly, value); } }
		[JSONEnum((int)JSONCharacterFormattingProperty.StrikeoutWordsOnly)]
		public bool StrikeoutWordsOnly { get { return GetBooleanValue(MaskStrikeoutWordsOnly); } set { SetBooleanValue(MaskStrikeoutWordsOnly, value); } }
		[JSONEnum((int)JSONCharacterFormattingProperty.ForeColor)]
		public Color ForeColor { get { return foreColor; } set { foreColor = value; } }
		[JSONEnum((int)JSONCharacterFormattingProperty.BackColor)]
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		[JSONEnum((int)JSONCharacterFormattingProperty.UnderlineColor)]
		public Color UnderlineColor { get { return underlineColor; } set { underlineColor = value; } }
		[JSONEnum((int)JSONCharacterFormattingProperty.StrikeoutColor)]
		public Color StrikeoutColor { get { return strikeoutColor; } set { strikeoutColor = value; } }
#if THEMES_EDIT
		public RichEditFontInfo FontInfo { get { return fontInfo; } set { fontInfo = value; } }
		public ColorModelInfo ForeColorInfo { get { return foreColorInfo; } set { foreColorInfo = value; } }
		public ColorModelInfo BackColorInfo { get { return backColorInfo; } set { backColorInfo = value; } }
		public ColorModelInfo UnderlineColorInfo { get { return underlineColorInfo; } set { underlineColorInfo = value; } }
		public ColorModelInfo StrikeoutColorInfo { get { return strikeoutColorInfo; } set { strikeoutColorInfo = value; } }
		public Shading Shading { get { return shading; } set { shading = value; } }
#endif
		[JSONEnum((int)JSONCharacterFormattingProperty.Script)]
		public CharacterFormattingScript Script {
			get { return (CharacterFormattingScript)((packedValues & MaskCharacterFormattingScript) >> 7); }
			set {
				packedValues &= ~MaskCharacterFormattingScript;
				packedValues |= ((uint)value << 7) & MaskCharacterFormattingScript;
			}
		}
		[JSONEnum((int)JSONCharacterFormattingProperty.Hidden)]
		public bool Hidden { get { return GetBooleanValue(MaskHidden); } set { SetBooleanValue(MaskHidden, value); } }
		[ExcludeFromJSON]
		public LangInfo LangInfo { get ; set; }
		[JSONEnum((int)JSONCharacterFormattingProperty.NoProof)]
		public bool NoProof { get { return GetBooleanValue(MaskNoProof); } set { SetBooleanValue(MaskNoProof, value); } }
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		public CharacterFormattingInfo Clone() {
			CharacterFormattingInfo result = new CharacterFormattingInfo();
			result.CopyFrom(this);
			return result;
		}
		public override bool Equals(object obj) {
			CharacterFormattingInfo info = (CharacterFormattingInfo)obj;
			return
				this.packedValues == info.packedValues &&
				this.ForeColor.ToArgb() == info.ForeColor.ToArgb() &&
				this.BackColor.ToArgb() == info.BackColor.ToArgb() &&
				this.UnderlineColor.ToArgb() == info.UnderlineColor.ToArgb() &&
				this.StrikeoutColor.ToArgb() == info.StrikeoutColor.ToArgb() &&
				this.LangInfo.Equals(info.LangInfo) &&
#if THEMES_EDIT
				this.ForeColorInfo.Equals(info.ForeColorInfo) &&
				this.BackColorInfo.Equals(info.BackColorInfo) &&
				this.UnderlineColorInfo.Equals(info.UnderlineColorInfo) &&
				this.StrikeoutColorInfo.Equals(info.StrikeoutColorInfo) &&
				this.Shading.Equals(info.Shading) &&
				this.FontInfo.Equals(info.FontInfo) &&
#endif
				StringExtensions.CompareInvariantCultureIgnoreCase(this.FontName, info.FontName) == 0;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public void CopyFrom(CharacterFormattingInfo info) {
			this.packedValues = info.packedValues;
			FontName = info.FontName;
			ForeColor = info.ForeColor;
			BackColor = info.BackColor;
			UnderlineColor = info.UnderlineColor;
			StrikeoutColor = info.StrikeoutColor;
			LangInfo = info.LangInfo;
#if THEMES_EDIT            
			FontInfo = info.FontInfo;
			ForeColorInfo = info.ForeColorInfo;
			BackColorInfo = info.BackColorInfo;
			UnderlineColorInfo = info.UnderlineColorInfo;
			StrikeoutColorInfo = info.StrikeoutColorInfo;
			Shading = info.Shading;
#endif
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region CharacterFormattingInfoCache
	public class CharacterFormattingInfoCache : UniqueItemsCache<CharacterFormattingInfo> {
		public const int DefaultItemIndex = 0;
		public CharacterFormattingInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override CharacterFormattingInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			CharacterFormattingInfo defaultItem = new CharacterFormattingInfo();
			defaultItem.FontName = RichEditControlCompatibility.DefaultFontName;
			defaultItem.DoubleFontSize = RichEditControlCompatibility.DefaultDoubleFontSize;
			return defaultItem;
		}
	}
	#endregion
	#region CharacterFormattingOptions
	public class CharacterFormattingOptions : ICloneable<CharacterFormattingOptions>, ISupportsCopyFrom<CharacterFormattingOptions>, ISupportsSizeOf {
		#region Mask enumeration
		public enum Mask {
			UseNone = 0x00000000,
			UseFontName = 0x00000001,
			UseDoubleFontSize = 0x00000002,
			UseFontBold = 0x00000004,
			UseFontItalic = 0x00000008,
			UseFontStrikeoutType = 0x00000010,
			UseFontUnderlineType = 0x00000020,
			UseAllCaps = 0x00000040,
			UseForeColor = 0x00000080,
			UseBackColor = 0x00000100,
			UseUnderlineColor = 0x00000200,
			UseStrikeoutColor = 0x00000400,
			UseUnderlineWordsOnly = 0x00000800,
			UseStrikeoutWordsOnly = 0x00001000,
			UseScript = 0x00002000,
			UseHidden = 0x00004000,
			UseLangInfo = 0x00008000,
			UseNoProof = 0x00010000,
#if THEMES_EDIT
			UseFontInfo = 0x00020000,
#endif
			UseAll = 0x7FFFFFFF
		}
		#endregion
		Mask val = Mask.UseNone;
		#region Properties
		internal Mask Value { get { return val; } set { val = value; } }
		public bool UseFontName { get { return GetVal(Mask.UseFontName); } set { SetVal(Mask.UseFontName, value); } }
		public bool UseDoubleFontSize { get { return GetVal(Mask.UseDoubleFontSize); } set { SetVal(Mask.UseDoubleFontSize, value); } }
		public bool UseFontBold { get { return GetVal(Mask.UseFontBold); } set { SetVal(Mask.UseFontBold, value); } }
		public bool UseFontItalic { get { return GetVal(Mask.UseFontItalic); } set { SetVal(Mask.UseFontItalic, value); } }
		public bool UseFontStrikeoutType { get { return GetVal(Mask.UseFontStrikeoutType); } set { SetVal(Mask.UseFontStrikeoutType, value); } }
		public bool UseFontUnderlineType { get { return GetVal(Mask.UseFontUnderlineType); } set { SetVal(Mask.UseFontUnderlineType, value); } }
		public bool UseAllCaps { get { return GetVal(Mask.UseAllCaps); } set { SetVal(Mask.UseAllCaps, value); } }
		public bool UseStrikeoutWordsOnly { get { return GetVal(Mask.UseStrikeoutWordsOnly); } set { SetVal(Mask.UseStrikeoutWordsOnly, value); } }
		public bool UseUnderlineWordsOnly { get { return GetVal(Mask.UseUnderlineWordsOnly); } set { SetVal(Mask.UseUnderlineWordsOnly, value); } }
		public bool UseForeColor { get { return GetVal(Mask.UseForeColor); } set { SetVal(Mask.UseForeColor, value); } }
		public bool UseBackColor { get { return GetVal(Mask.UseBackColor); } set { SetVal(Mask.UseBackColor, value); } }
		public bool UseUnderlineColor { get { return GetVal(Mask.UseUnderlineColor); } set { SetVal(Mask.UseUnderlineColor, value); } }
		public bool UseStrikeoutColor { get { return GetVal(Mask.UseStrikeoutColor); } set { SetVal(Mask.UseStrikeoutColor, value); } }
		public bool UseScript { get { return GetVal(Mask.UseScript); } set { SetVal(Mask.UseScript, value); } }
		public bool UseHidden { get { return GetVal(Mask.UseHidden); } set { SetVal(Mask.UseHidden, value); } }
		public bool UseLangInfo { get { return GetVal(Mask.UseLangInfo); } set { SetVal(Mask.UseLangInfo, value); } }
		public bool UseNoProof { get { return GetVal(Mask.UseNoProof); } set { SetVal(Mask.UseNoProof, value); } }
#if THEMES_EDIT
		public bool UseFontInfo { get { return GetVal(Mask.UseFontInfo); } set { SetVal(Mask.UseFontInfo, value); } } 
#endif
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
		public CharacterFormattingOptions() {
		}
		internal CharacterFormattingOptions(Mask val) {
			this.val = val;
		}
		public CharacterFormattingOptions Clone() {
			return new CharacterFormattingOptions(this.val);
		}
		public override bool Equals(object obj) {
			CharacterFormattingOptions opts = (CharacterFormattingOptions)obj;
			return opts.Value == this.Value;
		}
		public override int GetHashCode() {
			return (int)this.Value;
		}
		public void CopyFrom(CharacterFormattingOptions options) {
			this.val = options.Value;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region CharacterFormattingOptionsCache
	public class CharacterFormattingOptionsCache : UniqueItemsCache<CharacterFormattingOptions> {
		internal const int EmptyCharacterFormattingOptionIndex = 0;
		internal const int RootCharacterFormattingOptionIndex = 1;
		public CharacterFormattingOptionsCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			base.InitItems(unitConverter);
			AddRootStyleOptions();
		}
		protected override CharacterFormattingOptions CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new CharacterFormattingOptions();
		}
		void AddRootStyleOptions() {
			AppendItem(new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseAll));
		}
	}
	#endregion
	#region CharacterFormattingBase
	[UseWebFontInfoCache]
	public class CharacterFormattingBase : IndexBasedObjectB<CharacterFormattingInfo, CharacterFormattingOptions>, ICloneable<CharacterFormattingBase>, ISupportsCopyFrom<CharacterFormattingBase>, ICharacterProperties {
		internal CharacterFormattingBase(PieceTable pieceTable, DocumentModel documentModel, int formattingInfoIndex, int formattingOptionsIndex)
			: base(pieceTable, documentModel, formattingInfoIndex, formattingOptionsIndex) {
		}
		#region Properties
		internal CharacterFormattingInfo FormattingInfo { get { return Info; } }
		internal CharacterFormattingOptions FormattingOptions { get { return Options; } }
		protected override UniqueItemsCache<CharacterFormattingInfo> InfoCache { get { return ((DocumentModel)DocumentModel).Cache.CharacterFormattingInfoCache; } }
		protected override UniqueItemsCache<CharacterFormattingOptions> OptionsCache { get { return ((DocumentModel)DocumentModel).Cache.CharacterFormattingOptionsCache; } }
		#region AllCaps
		[JSONEnum((int)JSONCharacterFormattingProperty.AllCaps)]
		public bool AllCaps {
			get { return Info.AllCaps; }
			set {
				if (Info.AllCaps == value && Options.UseAllCaps)
					return;
				SetPropertyValue(SetAllCapsCore, value, SetUseAllCapsCore);
			}
		}
		void SetAllCapsCore(CharacterFormattingInfo info, bool value) {
			info.AllCaps = value;
		}
		void SetUseAllCapsCore(CharacterFormattingOptions options, bool value) {
			options.UseAllCaps = value;
		}
		#endregion
		#region BackColor
		[JSONEnum((int)JSONCharacterFormattingProperty.BackColor)]
		public Color BackColor {
			get { return Info.BackColor; }
			set {
				if (Info.BackColor == value && Options.UseBackColor)
					return;
				SetPropertyValue(SetBackColorCore, value, SetUseBackColorCore);
			}
		}
		void SetBackColorCore(CharacterFormattingInfo info, Color value) {
			info.BackColor = value;
		}
		void SetUseBackColorCore(CharacterFormattingOptions options, bool value) {
			options.UseBackColor = value;
		}
		#endregion
#if THEMES_EDIT
		#region BackColorInfo
		public ColorModelInfo BackColorInfo {
			get { return Info.BackColorInfo; }
			set {
				if (Info.BackColorInfo == value && Options.UseBackColor)
					return;
				SetPropertyValue(SetBackColorInfoCore, value, SetUseBackColorCore);
			}
		}
		void SetBackColorInfoCore(CharacterFormattingInfo info, ColorModelInfo value) {
			info.BackColorInfo = value;
		}
		#endregion
		#region Shading
		public Shading Shading {
			get { return Info.Shading; }
			set {
				if (Info.Shading == value && Options.UseBackColor)
					return;
				SetPropertyValue(SetShadingCore, value, SetUseBackColorCore);
			}
		}
		void SetShadingCore(CharacterFormattingInfo info, Shading value) {
			info.Shading = value;
		}
		#endregion
#endif
		#region FontBold
		[JSONEnum((int)JSONCharacterFormattingProperty.FontBold)]
		public bool FontBold {
			get { return Info.FontBold; }
			set {
				if (Info.FontBold == value && Options.UseFontBold)
					return;
				SetPropertyValue(SetFontBoldCore, value, SetUseFontBoldCore);
			}
		}
		void SetFontBoldCore(CharacterFormattingInfo info, bool value) {
			info.FontBold = value;
		}
		void SetUseFontBoldCore(CharacterFormattingOptions options, bool value) {
			options.UseFontBold = value;
		}
		#endregion
		#region FontItalic
		[JSONEnum((int)JSONCharacterFormattingProperty.FontItalic)]
		public bool FontItalic {
			get { return Info.FontItalic; }
			set {
				if (Info.FontItalic == value && Options.UseFontItalic)
					return;
				SetPropertyValue(SetFontItalicCore, value, SetUseFontItalicCore);
			}
		}
		void SetFontItalicCore(CharacterFormattingInfo info, bool value) {
			info.FontItalic = value;
		}
		void SetUseFontItalicCore(CharacterFormattingOptions options, bool value) {
			options.UseFontItalic = value;
		}
		#endregion
		#region FontName
		[JSONSupports("fontInfoIndex", "this.fontInfoCache.AddItem(info.FontName)", "this.fontInfoCache.GetItem((int)source[((int)JSONCharacterFormattingProperty.FontInfoIndex).ToString()]).Name")]
		[JSONEnum((int)JSONCharacterFormattingProperty.FontInfoIndex)]
		public string FontName {
			get { return Info.FontName; }
			set {
				if (Info.FontName == value && Options.UseFontName)
					return;
				SetPropertyValue(SetFontNameCore, value, SetUseFontNameCore);
			}
		}
		void SetFontNameCore(CharacterFormattingInfo info, string value) {
			info.FontName = value;
		}
		void SetUseFontNameCore(CharacterFormattingOptions options, bool value) {
			options.UseFontName = value;
		}
		#endregion
#if THEMES_EDIT 
		#region FontInfo
		public RichEditFontInfo FontInfo {
			get { return Info.FontInfo; }
			set {
				if (Info.FontInfo == value && Options.UseFontName)
					return;
				SetPropertyValue(SetRichEditFontInfoCore, value, SetUseFontNameCore);
			}
		}
		void SetRichEditFontInfoCore(CharacterFormattingInfo info, RichEditFontInfo value) {
			info.FontInfo = value;
		}
		#endregion
#endif
		#region DoubleFontSize
		[JSONSupports("fontSize", "info.DoubleFontSize / 2.0f", "(int)(Convert.ToDouble(source[((int)JSONCharacterFormattingProperty.FontSize).ToString()]) * 2)")]
		[JSONEnum((int)JSONCharacterFormattingProperty.FontSize)]
		public int DoubleFontSize {
			get { return Info.DoubleFontSize; }
			set {
				if (Info.DoubleFontSize == value && Options.UseDoubleFontSize)
					return;
				SetPropertyValue(SetFontSizeCore, value, SetUseFontSizeCore);
			}
		}
		void SetFontSizeCore(CharacterFormattingInfo info, int value) {
			info.DoubleFontSize = value;
		}
		void SetUseFontSizeCore(CharacterFormattingOptions options, bool value) {
			options.UseDoubleFontSize = value;
		}
		void SetFontSizeCore(int value) {
			CharacterFormattingInfo newInfo = GetInfoForModification();
			CharacterFormattingOptions newOptions = GetOptionsForModification();
			newInfo.DoubleFontSize = value;
			newOptions.UseDoubleFontSize = true;
			ReplaceInfo(newInfo, newOptions);
		}
		#endregion
		#region FontStrikeoutType
		[JSONEnum((int)JSONCharacterFormattingProperty.FontStrikeoutType)]
		public StrikeoutType FontStrikeoutType {
			get { return Info.FontStrikeoutType; }
			set {
				if (Info.FontStrikeoutType == value && Options.UseFontStrikeoutType)
					return;
				SetPropertyValue(SetFontStrikeoutTypeCore, value, SetUseFontStrikeoutTypeCore);
			}
		}
		void SetFontStrikeoutTypeCore(CharacterFormattingInfo info, StrikeoutType value) {
			info.FontStrikeoutType = value;
		}
		void SetUseFontStrikeoutTypeCore(CharacterFormattingOptions options, bool value) {
			options.UseFontStrikeoutType = value;
		}
		#endregion
		#region FontUnderlineType
		[JSONEnum((int)JSONCharacterFormattingProperty.FontUnderlineType)]
		public UnderlineType FontUnderlineType {
			get { return Info.FontUnderlineType; }
			set {
				if (Info.FontUnderlineType == value && Options.UseFontUnderlineType)
					return;
				SetPropertyValue(SetFontUnderlineTypeCore, value, SetUseFontUnderlineTypeCore);
			}
		}
		void SetFontUnderlineTypeCore(CharacterFormattingInfo info, UnderlineType value) {
			info.FontUnderlineType = value;
		}
		void SetUseFontUnderlineTypeCore(CharacterFormattingOptions options, bool value) {
			options.UseFontUnderlineType = value;
		}
		#endregion
		#region ForeColor
		[JSONEnum((int)JSONCharacterFormattingProperty.ForeColor)]
		public Color ForeColor {
			get { return Info.ForeColor; }
			set {
				if (Info.ForeColor == value && Options.UseForeColor)
					return;
				SetPropertyValue(SetForeColorCore, value, SetUseForeColorCore);
			}
		}
		void SetForeColorCore(CharacterFormattingInfo info, Color value) {
			info.ForeColor = value;
		}
		void SetUseForeColorCore(CharacterFormattingOptions options, bool value) {
			options.UseForeColor = value;
		}
		#endregion
#if THEMES_EDIT
		#region ForeColorInfo
		public ColorModelInfo ForeColorInfo {
			get { return Info.ForeColorInfo; }
			set {
				if (Info.ForeColorInfo == value && Options.UseForeColor)
					return;
				SetPropertyValue(SetForeColorInfoCore, value, SetUseForeColorCore);
			}
		}
		void SetForeColorInfoCore(CharacterFormattingInfo info, ColorModelInfo value) {
			info.ForeColorInfo = value;
		}
		#endregion
#endif
		#region Script
		[JSONEnum((int)JSONCharacterFormattingProperty.Script)]
		public CharacterFormattingScript Script {
			get { return Info.Script; }
			set {
				if (Info.Script == value && Options.UseScript)
					return;
				SetPropertyValue(SetScriptCore, value, SetUseScriptCore);
			}
		}
		void SetScriptCore(CharacterFormattingInfo info, CharacterFormattingScript value) {
			info.Script = value;
		}
		void SetUseScriptCore(CharacterFormattingOptions options, bool value) {
			options.UseScript = value;
		}
		#endregion
		#region StrikeoutColor
		[JSONEnum((int)JSONCharacterFormattingProperty.StrikeoutColor)]
		public Color StrikeoutColor {
			get { return Info.StrikeoutColor; }
			set {
				if (Info.StrikeoutColor == value && Options.UseStrikeoutColor)
					return;
				SetPropertyValue(SetStrikeoutColorCore, value, SetUseStrikeoutColorCore);
			}
		}
		void SetStrikeoutColorCore(CharacterFormattingInfo info, Color value) {
			info.StrikeoutColor = value;
		}
		void SetUseStrikeoutColorCore(CharacterFormattingOptions options, bool value) {
			options.UseStrikeoutColor = value;
		}
		#endregion
#if THEMES_EDIT
		#region StrikeoutColorInfo
		public ColorModelInfo StrikeoutColorInfo {
			get { return Info.StrikeoutColorInfo; }
			set {
				if (Info.StrikeoutColorInfo == value && Options.UseStrikeoutColor)
					return;
				SetPropertyValue(SetStrikeoutColorInfoCore, value, SetUseStrikeoutColorCore);
			}
		}
		void SetStrikeoutColorInfoCore(CharacterFormattingInfo info, ColorModelInfo value) {
			info.StrikeoutColorInfo = value;
		}
		#endregion
#endif
		#region StrikeoutWordsOnly
		[JSONEnum((int)JSONCharacterFormattingProperty.StrikeoutWordsOnly)]
		public bool StrikeoutWordsOnly {
			get { return Info.StrikeoutWordsOnly; }
			set {
				if (Info.StrikeoutWordsOnly == value && Options.UseStrikeoutWordsOnly)
					return;
				SetPropertyValue(SetStrikeoutWordsOnlyCore, value, SetUseStrikeoutWordsOnlyCore);
			}
		}
		void SetStrikeoutWordsOnlyCore(CharacterFormattingInfo info, bool value) {
			info.StrikeoutWordsOnly = value;
		}
		void SetUseStrikeoutWordsOnlyCore(CharacterFormattingOptions options, bool value) {
			options.UseStrikeoutWordsOnly = value;
		}
		#endregion
		#region UnderlineColor
		[JSONEnum((int)JSONCharacterFormattingProperty.UnderlineColor)]
		public Color UnderlineColor {
			get { return Info.UnderlineColor; }
			set {
				if (Info.UnderlineColor == value && Options.UseUnderlineColor)
					return;
				SetPropertyValue(SetUnderlineColorCore, value, SetUseUnderlineColorCore);
			}
		}
		void SetUnderlineColorCore(CharacterFormattingInfo info, Color value) {
			info.UnderlineColor = value;
		}
		void SetUseUnderlineColorCore(CharacterFormattingOptions options, bool value) {
			options.UseUnderlineColor = value;
		}
		#endregion
#if THEMES_EDIT
		#region UnderlineColorInfo
		public ColorModelInfo UnderlineColorInfo {
			get { return Info.UnderlineColorInfo; }
			set {
				if (Info.UnderlineColorInfo == value && Options.UseUnderlineColor)
					return;
				SetPropertyValue(SetUnderlineColorInfoCore, value, SetUseUnderlineColorCore);
			}
		}
		void SetUnderlineColorInfoCore(CharacterFormattingInfo info, ColorModelInfo value) {
			info.UnderlineColorInfo = value;
		}
		#endregion
#endif
		#region UnderlineWordsOnly
		[JSONEnum((int)JSONCharacterFormattingProperty.UnderlineWordsOnly)]
		public bool UnderlineWordsOnly {
			get { return Info.UnderlineWordsOnly; }
			set {
				if (Info.UnderlineWordsOnly == value && Options.UseUnderlineWordsOnly)
					return;
				SetPropertyValue(SetUnderlineWordsOnlyCore, value, SetUseUnderlineWordsOnlyCore);
			}
		}
		void SetUnderlineWordsOnlyCore(CharacterFormattingInfo info, bool value) {
			info.UnderlineWordsOnly = value;
		}
		void SetUseUnderlineWordsOnlyCore(CharacterFormattingOptions options, bool value) {
			options.UseUnderlineWordsOnly = value;
		}
		#endregion
		#region Hidden
		[JSONEnum((int)JSONCharacterFormattingProperty.Hidden)]
		public bool Hidden {
			get { return Info.Hidden; }
			set {
				if (Info.Hidden == value && Options.UseHidden)
					return;
				SetPropertyValue(SetHiddenCore, value, SetHiddenCore);
			}
		}
		void SetHiddenCore(CharacterFormattingInfo info, bool value) {
			info.Hidden = value;
		}
		void SetHiddenCore(CharacterFormattingOptions options, bool value) {
			options.UseHidden = value;
		}
		#endregion
		#region LangInfo
		[ExcludeFromJSON]
		public LangInfo LangInfo {
			get { return Info.LangInfo; }
			set {
				if (Info.LangInfo.Equals(value) && Options.UseLangInfo)
					return;
				SetPropertyValue(SetLangInfoCore, value, SetUseLangInfoCore);
			}
		}
		void SetLangInfoCore(CharacterFormattingInfo info, LangInfo value) {
			info.LangInfo = value;
		}
		void SetUseLangInfoCore(CharacterFormattingOptions options, bool value) {
			options.UseLangInfo = value;
		}
		#endregion
		#region NoProof
		[JSONEnum((int)JSONCharacterFormattingProperty.NoProof)]
		public bool NoProof {
			get { return Info.NoProof; }
			set {
				if (Info.NoProof == value && Options.UseNoProof)
					return;
				SetPropertyValue(SetNoProofCore, value, SetNoProofCore);
			}
		}
		void SetNoProofCore(CharacterFormattingInfo info, bool value) {
			info.NoProof = value;
		}
		void SetNoProofCore(CharacterFormattingOptions options, bool value) {
			options.UseNoProof = value;
	   }
		#endregion
		#region UseValue
		[JSONEnum((int)JSONCharacterFormattingProperty.UseValue)]
		public CharacterFormattingOptions.Mask UseValue { 
			get { return FormattingOptions.Value; }
			internal set { FormattingOptions.Value = value; }
		}
		#endregion
		#endregion
		#region ICloneable<CharacterFormattingBase> Members
		public CharacterFormattingBase Clone() {
			return new CharacterFormattingBase(PieceTable, (DocumentModel)DocumentModel, InfoIndex, OptionsIndex);
		}
		#endregion
		public void CopyFrom(CharacterFormattingBase characterFormatting) {
			CopyFrom(characterFormatting.Info, characterFormatting.Options);
		}
		public void CopyFrom(CharacterFormattingInfo info, CharacterFormattingOptions options) {
			CopyFromCore(info, options);
		}
		protected override bool PropertyEquals(IndexBasedObject<CharacterFormattingInfo, CharacterFormattingOptions> other) {
			Guard.ArgumentNotNull(other, "other");
			return Options.Value == other.Options.Value &&
				Info.Equals(other.Info);
		}
		protected override void SetPropertyValue<U>(IndexBasedObjectB<CharacterFormattingInfo, CharacterFormattingOptions>.SetPropertyValueDelegate<U> setter, U newValue, IndexBasedObjectB<CharacterFormattingInfo, CharacterFormattingOptions>.SetOptionsValueDelegate optionsSetter) {
			if (((DocumentModel)DocumentModel).DocumentCapabilities.CharacterFormattingAllowed)
				base.SetPropertyValue<U>(setter, newValue, optionsSetter);
		}
		internal void ResetUse(CharacterFormattingOptions.Mask mask) {
			CharacterFormattingOptions options = GetOptionsForModification();
			options.Value &= ~mask;
			ReplaceInfo(GetInfoForModification(), options);
		}
	}
	#endregion
	#region JSONCharacterFormattingProperty
	public enum JSONCharacterFormattingProperty {
		FontInfoIndex = 0,
		FontSize = 1,
		FontBold = 2,
		FontItalic = 3,
		FontStrikeoutType = 4,
		FontUnderlineType = 5,
		AllCaps = 6,
		StrikeoutWordsOnly = 7,
		UnderlineWordsOnly = 8,
		ForeColor = 9,
		BackColor = 10,
		UnderlineColor = 11,
		StrikeoutColor = 12,
		Script = 13,
		Hidden = 14,
		LangInfo = 15,
		NoProof = 16,
		UseValue = 17
	}
	#endregion
	#region CharacterFormattingCache
	public class CharacterFormattingCache : UniqueItemsCache<CharacterFormattingBase> {
		#region Fields
		public const int RootCharacterFormattingIndex = 1;
		public const int EmptyCharacterFormattingIndex = 0;
		readonly DocumentModel documentModel;
		#endregion
		public CharacterFormattingCache(DocumentModel documentModel)
			: base(documentModel.UnitConverter) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			AppendItem(new CharacterFormattingBase(DocumentModel.MainPieceTable, DocumentModel, CharacterFormattingInfoCache.DefaultItemIndex, CharacterFormattingOptionsCache.EmptyCharacterFormattingOptionIndex));
			AppendItem(new CharacterFormattingBase(DocumentModel.MainPieceTable, DocumentModel, CharacterFormattingInfoCache.DefaultItemIndex, CharacterFormattingOptionsCache.RootCharacterFormattingOptionIndex));
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		protected override CharacterFormattingBase CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return null;
		}
	}
	#endregion
	#region CharacterProperties
	public class CharacterProperties : RichEditIndexBasedObject<CharacterFormattingBase>, ICharacterProperties {
		#region static
		public static void ApplyPropertiesDiff(CharacterProperties target, CharacterFormattingInfo targetMergedInfo, CharacterFormattingInfo sourceMergedInfo) {
			if (targetMergedInfo.AllCaps != sourceMergedInfo.AllCaps)
				target.AllCaps = sourceMergedInfo.AllCaps;
			if (targetMergedInfo.BackColor != sourceMergedInfo.BackColor)
				target.BackColor = sourceMergedInfo.BackColor;
#if THEMES_EDIT
			if (!targetMergedInfo.BackColorInfo.Equals(sourceMergedInfo.BackColorInfo))
				target.BackColorInfo = sourceMergedInfo.BackColorInfo;
			if (!targetMergedInfo.Shading.Equals(sourceMergedInfo.Shading))
				target.Shading = sourceMergedInfo.Shading;
#endif
			if (targetMergedInfo.FontBold != sourceMergedInfo.FontBold)
				target.FontBold = sourceMergedInfo.FontBold;
			if (targetMergedInfo.FontItalic != sourceMergedInfo.FontItalic)
				target.FontItalic = sourceMergedInfo.FontItalic;
			if (targetMergedInfo.FontName != sourceMergedInfo.FontName)
				target.FontName = sourceMergedInfo.FontName;
			if (targetMergedInfo.DoubleFontSize != sourceMergedInfo.DoubleFontSize)
				target.DoubleFontSize = sourceMergedInfo.DoubleFontSize;
			if (targetMergedInfo.FontStrikeoutType != sourceMergedInfo.FontStrikeoutType)
				target.FontStrikeoutType = sourceMergedInfo.FontStrikeoutType;
			if (targetMergedInfo.FontUnderlineType != sourceMergedInfo.FontUnderlineType)
				target.FontUnderlineType = sourceMergedInfo.FontUnderlineType;
			if (targetMergedInfo.ForeColor != sourceMergedInfo.ForeColor)
				target.ForeColor = sourceMergedInfo.ForeColor;
#if THEMES_EDIT
			if (!targetMergedInfo.ForeColorInfo.Equals(sourceMergedInfo.ForeColorInfo))
				target.ForeColorInfo = sourceMergedInfo.ForeColorInfo;
#endif
			if (targetMergedInfo.Hidden != sourceMergedInfo.Hidden)
				target.Hidden = sourceMergedInfo.Hidden;
			if (targetMergedInfo.Script != sourceMergedInfo.Script)
				target.Script = sourceMergedInfo.Script;
			if (targetMergedInfo.StrikeoutColor != sourceMergedInfo.StrikeoutColor)
				target.StrikeoutColor = sourceMergedInfo.StrikeoutColor;
#if THEMES_EDIT
			if (!targetMergedInfo.StrikeoutColorInfo.Equals(sourceMergedInfo.StrikeoutColorInfo))
				target.StrikeoutColorInfo = sourceMergedInfo.StrikeoutColorInfo;
#endif
			if (targetMergedInfo.StrikeoutWordsOnly != sourceMergedInfo.StrikeoutWordsOnly)
				target.StrikeoutWordsOnly = sourceMergedInfo.StrikeoutWordsOnly;
			if (targetMergedInfo.UnderlineColor != sourceMergedInfo.UnderlineColor)
				target.UnderlineColor = sourceMergedInfo.UnderlineColor;
#if THEMES_EDIT
			if (!targetMergedInfo.UnderlineColorInfo.Equals(sourceMergedInfo.UnderlineColorInfo))
				target.UnderlineColorInfo = sourceMergedInfo.UnderlineColorInfo;
#endif
			if (targetMergedInfo.UnderlineWordsOnly != sourceMergedInfo.UnderlineWordsOnly)
				target.UnderlineWordsOnly = sourceMergedInfo.UnderlineWordsOnly;
			if (targetMergedInfo.NoProof != sourceMergedInfo.NoProof)
				target.NoProof = sourceMergedInfo.NoProof;
		}
		#endregion
		readonly ICharacterPropertiesContainer owner;
		public CharacterProperties(ICharacterPropertiesContainer owner)
			: base(GetPieceTable(owner)) {
			this.owner = owner;
		}
		static PieceTable GetPieceTable(ICharacterPropertiesContainer owner) {
			Guard.ArgumentNotNull(owner, "owner");
			return owner.PieceTable;
		}
		protected override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateIndexChangedHistoryItem() {
			return owner.CreateCharacterPropertiesChangedHistoryItem();
		}
		#region Properties
		protected internal ICharacterPropertiesContainer Owner { get { return owner; } }
		#region AllCaps
		public bool AllCaps {
			get { return Info.AllCaps; }
			set {
				SetPropertyValue(SetAllCapsCore, value);
			}
		}
		public bool UseAllCaps { get { return Info.Options.UseAllCaps; } }
		protected internal virtual DocumentModelChangeActions SetAllCapsCore(CharacterFormattingBase info, bool value) {
			info.AllCaps = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.AllCaps);
		}
		#endregion
		#region BackColor
		public Color BackColor {
			get { return Info.BackColor; }
			set {
				SetPropertyValue(SetBackColorCore, value);
			}
		}
		public bool UseBackColor { get { return Info.Options.UseBackColor; } }
		protected internal virtual DocumentModelChangeActions SetBackColorCore(CharacterFormattingBase info, Color value) {
			info.BackColor = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.BackColor);
		}
		#endregion
#if THEMES_EDIT
		#region BackColorInfo
		public ColorModelInfo BackColorInfo {
			get { return Info.BackColorInfo; }
			set {
				SetPropertyValue(SetBackColorInfoCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetBackColorInfoCore(CharacterFormattingBase info, ColorModelInfo value) {
			info.BackColorInfo = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.BackColor);
		}
		#endregion
		#region Shading
		public Shading Shading {
			get { return Info.Shading; }
			set {
				SetPropertyValue(SetShadingCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetShadingCore(CharacterFormattingBase info, Shading value) {
			info.Shading = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.BackColor);
		}
		#endregion
#endif
		#region FontBold
		public bool FontBold {
			get { return Info.FontBold; }
			set {
				SetPropertyValue(SetFontBoldCore, value);
			}
		}
		public bool UseFontBold { get { return Info.Options.UseFontBold; } }
		protected internal virtual DocumentModelChangeActions SetFontBoldCore(CharacterFormattingBase info, bool value) {
			info.FontBold = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.FontBold);
		}
		#endregion
		#region FontItalic
		public bool FontItalic {
			get { return Info.FontItalic; }
			set {
				SetPropertyValue(SetFontItalicCore, value);
			}
		}
		public bool UseFontItalic { get { return Info.Options.UseFontItalic; } }
		protected internal virtual DocumentModelChangeActions SetFontItalicCore(CharacterFormattingBase info, bool value) {
			info.FontItalic = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.FontItalic);
		}
		#endregion
		#region FontName
		public string FontName {
			get { return Info.FontName; }
			set {
				SetPropertyValue(SetFontNameCore, value);
			}
		}
		public bool UseFontName { get { return Info.Options.UseFontName; } }
		protected internal virtual DocumentModelChangeActions SetFontNameCore(CharacterFormattingBase info, string value) {
			info.FontName = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.FontName);
		}
		#endregion
#if THEMES_EDIT      
		#region FontInfo
		public RichEditFontInfo FontInfo {
			get { return Info.FontInfo; }
			set {
				SetPropertyValue(SetRichEditFontInfoCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetRichEditFontInfoCore(CharacterFormattingBase info, RichEditFontInfo value) {
			info.FontInfo = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.FontName);
		}
		#endregion
#endif
		#region DoubleFontSize
		public int DoubleFontSize {
			get { return Info.DoubleFontSize; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("DoubleFontSize", value);
				SetPropertyValue(SetFontSizeCore, value);
			}
		}
		public bool UseDoubleFontSize { get { return Info.Options.UseDoubleFontSize; } }
		protected internal virtual DocumentModelChangeActions SetFontSizeCore(CharacterFormattingBase info, int value) {
			info.DoubleFontSize = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.FontSize);
		}
		#endregion
		#region FontStrikeoutType
		public StrikeoutType FontStrikeoutType {
			get { return Info.FontStrikeoutType; }
			set {
				SetPropertyValue(SetFontStrikeoutTypeCore, value);
			}
		}
		public bool UseFontStrikeoutType { get { return Info.Options.UseFontStrikeoutType; } }
		protected internal virtual DocumentModelChangeActions SetFontStrikeoutTypeCore(CharacterFormattingBase info, StrikeoutType value) {
			info.FontStrikeoutType = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.FontStrikeoutType);
		}
		#endregion
		#region FontUnderlineType
		public UnderlineType FontUnderlineType {
			get { return Info.FontUnderlineType; }
			set {
				SetPropertyValue(SetFontUnderlineTypeCore, value);
			}
		}
		public bool UseFontUnderlineType { get { return Info.Options.UseFontUnderlineType; } }
		protected internal virtual DocumentModelChangeActions SetFontUnderlineTypeCore(CharacterFormattingBase info, UnderlineType value) {
			info.FontUnderlineType = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.FontUnderlineType);
		}
		#endregion
		#region ForeColor
		public Color ForeColor {
			get { return Info.ForeColor; }
			set {
				if (!IsUpdateLocked && Info.Options.UseForeColor && Object.Equals(Info.Info.ForeColor, value))
					return;
				SetPropertyValue(SetForeColorCore, value);
			}
		}
		public bool UseForeColor { get { return Info.Options.UseForeColor; } }
		protected internal virtual DocumentModelChangeActions SetForeColorCore(CharacterFormattingBase info, Color value) {
			info.ForeColor = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.ForeColor);
		}
		#endregion
#if THEMES_EDIT
		#region ForeColorInfo
		public ColorModelInfo ForeColorInfo {
			get { return Info.ForeColorInfo; }
			set {
				if (!IsUpdateLocked && Info.Options.UseForeColor && Object.Equals(Info.Info.ForeColorInfo, value))
					return;
				SetPropertyValue(SetForeColorInfoCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetForeColorInfoCore(CharacterFormattingBase info, ColorModelInfo value) {
			info.ForeColorInfo = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.ForeColor);
		}
		#endregion
#endif
		#region Script
		public CharacterFormattingScript Script {
			get { return Info.Script; }
			set {
				SetPropertyValue(SetScriptCore, value);
			}
		}
		public bool UseScript { get { return Info.Options.UseScript; } }
		protected internal virtual DocumentModelChangeActions SetScriptCore(CharacterFormattingBase info, CharacterFormattingScript value) {
			info.Script = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.Script);
		}
		#endregion
		#region StrikeoutColor
		public Color StrikeoutColor {
			get { return Info.StrikeoutColor; }
			set {
				SetPropertyValue(SetStrikeoutColorCore, value);
			}
		}
		public bool UseStrikeoutColor { get { return Info.Options.UseStrikeoutColor; } }
		protected internal virtual DocumentModelChangeActions SetStrikeoutColorCore(CharacterFormattingBase info, Color value) {
			info.StrikeoutColor = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.StrikeoutColor);
		}
		#endregion
#if THEMES_EDIT
		#region StrikeoutColorInfo
		public ColorModelInfo StrikeoutColorInfo {
			get { return Info.StrikeoutColorInfo; }
			set {
				SetPropertyValue(SetStrikeoutColorInfoCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetStrikeoutColorInfoCore(CharacterFormattingBase info, ColorModelInfo value) {
			info.StrikeoutColorInfo = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.StrikeoutColor);
		}
		#endregion
#endif
		#region StrikeoutWordsOnly
		public bool StrikeoutWordsOnly {
			get { return Info.StrikeoutWordsOnly; }
			set {
				SetPropertyValue(SetStrikeoutWordsOnlyCore, value);
			}
		}
		public bool UseStrikeoutWordsOnly { get { return Info.Options.UseStrikeoutWordsOnly; } }
		protected internal virtual DocumentModelChangeActions SetStrikeoutWordsOnlyCore(CharacterFormattingBase info, bool value) {
			info.StrikeoutWordsOnly = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.StrikeoutWordsOnly);
		}
		#endregion
		#region UnderlineColor
		public Color UnderlineColor {
			get { return Info.UnderlineColor; }
			set {
				SetPropertyValue(SetUnderlineColorCore, value);
			}
		}
		public bool UseUnderlineColor { get { return Info.Options.UseUnderlineColor; } }
		protected internal virtual DocumentModelChangeActions SetUnderlineColorCore(CharacterFormattingBase info, Color value) {
			info.UnderlineColor = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.UnderlineColor);
		}
		#endregion
#if THEMES_EDIT
		#region UnderlineColorInfo
		public ColorModelInfo UnderlineColorInfo {
			get { return Info.UnderlineColorInfo; }
			set {
				SetPropertyValue(SetUnderlineColorInfoCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetUnderlineColorInfoCore(CharacterFormattingBase info, ColorModelInfo value) {
			info.UnderlineColorInfo = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.UnderlineColor);
		}
		#endregion
#endif
		#region UnderlineWordsOnly
		public bool UnderlineWordsOnly {
			get { return Info.UnderlineWordsOnly; }
			set {
				SetPropertyValue(SetUnderlineWordsOnlyCore, value);
			}
		}
		public bool UseUnderlineWordsOnly { get { return Info.Options.UseUnderlineWordsOnly; } }
		protected internal virtual DocumentModelChangeActions SetUnderlineWordsOnlyCore(CharacterFormattingBase info, bool value) {
			info.UnderlineWordsOnly = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.UnderlineWordsOnly);
		}
		#endregion
		#region Hidden
		public bool Hidden {
			get { return Info.Hidden; }
			set {
				SetPropertyValue(SetHiddenCore, value);
			}
		}
		public bool UseHidden { get { return Info.Options.UseHidden; } }
		protected internal virtual DocumentModelChangeActions SetHiddenCore(CharacterFormattingBase info, bool value) {
			info.Hidden = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.Hidden);
		}
		#endregion
		#region LangInfo
		public LangInfo LangInfo {
			get { return Info.LangInfo; }
			set {
				SetPropertyValue(SetLangInfoCore, value);
			}
		}
		public bool UseLangInfo { get { return Info.Options.UseLangInfo; } }
		protected internal virtual DocumentModelChangeActions SetLangInfoCore(CharacterFormattingBase info, LangInfo value) {
			info.LangInfo = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.LangInfo);
		}
		#endregion
		#region NoProof
		public bool NoProof {
			get { return Info.NoProof; }
			set {
				SetPropertyValue(SetNoProofCore, value);
			}
		}
		public bool UseNoProof { get { return Info.Options.UseNoProof; } }
		protected internal virtual DocumentModelChangeActions SetNoProofCore(CharacterFormattingBase info, bool value) {
			info.NoProof = value;
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.NoProof);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<CharacterFormattingBase> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.CharacterFormattingCache;
		}
		protected internal bool UseVal(CharacterFormattingOptions.Mask mask) {
			return (Info.Options.Value & mask) != 0;
		}
		public void CopyFrom(MergedProperties<CharacterFormattingInfo, CharacterFormattingOptions> characterProperties) {
			CharacterFormattingBase info = GetInfoForModification();
			info.CopyFromCore(characterProperties.Info, characterProperties.Options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		internal void ResetAllUse() {
			CharacterFormattingBase info = GetInfoForModification();
			CharacterFormattingOptions options = info.GetOptionsForModification();
			options.Value = CharacterFormattingOptions.Mask.UseNone;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		internal void ResetUse(CharacterFormattingOptions.Mask mask) {
			CharacterFormattingBase info = GetInfoForModification();
			CharacterFormattingOptions options = info.GetOptionsForModification();
			options.Value &= ~mask;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		public void Reset() {
			CharacterFormattingBase info = GetInfoForModification();
			CharacterFormattingBase emptyInfo = GetCache(DocumentModel)[CharacterFormattingCache.EmptyCharacterFormattingIndex];
			info.ReplaceInfo(emptyInfo.Info, emptyInfo.Options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		protected internal virtual void Merge(CharacterProperties properties) {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(this);
			merger.Merge(properties);
			CopyFrom(merger.MergedProperties);
		}
		protected override void OnIndexChanged() {
			Owner.OnCharacterPropertiesChanged();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.BatchUpdate);
		}
		protected internal override IObtainAffectedRangeListener GetObtainAffectedRangeListener() {
			return Owner as IObtainAffectedRangeListener;
		}
	}
	#endregion
	#region MergedProperties<TInfo, TOptions>
	public class MergedProperties<TInfo, TOptions>
		where TInfo : ICloneable<TInfo>
		where TOptions : ICloneable<TOptions> {
		readonly TInfo info;
		readonly TOptions options;
		public MergedProperties(TInfo info, TOptions options) {
			this.info = info.Clone();
			this.options = options.Clone();
		}
		public TInfo Info { get { return info; } }
		public TOptions Options { get { return options; } }
	}
	#endregion
	#region PropertiesMergerBase
	public abstract class PropertiesMergerBase<TInfo, TOptions, TMergedProperties>
		where TInfo : ICloneable<TInfo>
		where TOptions : ICloneable<TOptions>
		where TMergedProperties : MergedProperties<TInfo, TOptions> {
		readonly TMergedProperties mergedProperties;
		protected PropertiesMergerBase(TMergedProperties initial) {
			mergedProperties = initial;
		}
		public TMergedProperties MergedProperties { get { return mergedProperties; } }
		protected TInfo OwnInfo { get { return MergedProperties.Info; } }
		protected TOptions OwnOptions { get { return MergedProperties.Options; } }
		public void Merge(MergedProperties<TInfo, TOptions> properties) {
			MergeCore(properties.Info, properties.Options);
		}
		protected internal abstract void MergeCore(TInfo info, TOptions options);
	}
	#endregion
	#region MergedCharacterProperties
	public class MergedCharacterProperties : MergedProperties<CharacterFormattingInfo, CharacterFormattingOptions> {
		public MergedCharacterProperties(CharacterFormattingInfo info, CharacterFormattingOptions options)
			: base(info, options) {
		}
	}
	#endregion
	#region RunMergedCharacterPropertiesCachedResult
	public class RunMergedCharacterPropertiesCachedResult : ParagraphMergedCharacterPropertiesCachedResult {
		int characterStyleIndex = -1;
		int characterPropertiesIndex = -1;
		public int CharacterStyleIndex { get { return characterStyleIndex; } set { characterStyleIndex = value; } }
		public int CharacterPropertiesIndex { get { return characterPropertiesIndex; } set { characterPropertiesIndex = value; } }
	}
	#endregion
	#region ParagraphMergedCharacterPropertiesCachedResult
	public class ParagraphMergedCharacterPropertiesCachedResult {
		int paragraphStyleIndex = -1;
		TableCell tableCell;
		MergedCharacterProperties mergedCharacterProperties;
		public int ParagraphStyleIndex { get { return paragraphStyleIndex; } set { paragraphStyleIndex = value; } }
		public TableCell TableCell { get { return tableCell; } set { tableCell = value; } }
		public MergedCharacterProperties MergedCharacterProperties { get { return mergedCharacterProperties; } set { mergedCharacterProperties = value; } }
	}
	#endregion
	#region CharacterPropertiesMerger
	public class CharacterPropertiesMerger : PropertiesMergerBase<CharacterFormattingInfo, CharacterFormattingOptions, MergedCharacterProperties> {
		public CharacterPropertiesMerger(CharacterProperties initialProperties)
			: base(new MergedCharacterProperties(initialProperties.Info.Info, initialProperties.Info.Options)) {
		}
		public CharacterPropertiesMerger(MergedCharacterProperties initialProperties)
			: base(new MergedCharacterProperties(initialProperties.Info, initialProperties.Options)) {
		}
		public void Merge(CharacterProperties properties) {
			MergeCore(properties.Info.Info, properties.Info.Options);
		}
		protected internal override void MergeCore(CharacterFormattingInfo info, CharacterFormattingOptions options) {
			if (!OwnOptions.UseAllCaps && options.UseAllCaps) {
				OwnInfo.AllCaps = info.AllCaps;
				OwnOptions.UseAllCaps = true;
			}
			if (!OwnOptions.UseBackColor && options.UseBackColor) {
				OwnInfo.BackColor = info.BackColor;
#if THEMES_EDIT
				OwnInfo.BackColorInfo = info.BackColorInfo;
				OwnInfo.Shading = info.Shading;
#endif
				OwnOptions.UseBackColor = true;
			}
			if (!OwnOptions.UseFontBold && options.UseFontBold) {
				OwnInfo.FontBold = info.FontBold;
				OwnOptions.UseFontBold = true;
			}
			if (!OwnOptions.UseFontItalic && options.UseFontItalic) {
				OwnInfo.FontItalic = info.FontItalic;
				OwnOptions.UseFontItalic = true;
			}
			if (!OwnOptions.UseFontName && options.UseFontName) {
				OwnInfo.FontName = info.FontName;
				OwnOptions.UseFontName = true;
			}
#if THEMES_EDIT
			if (!OwnOptions.UseFontInfo && options.UseFontInfo) {
				OwnInfo.FontInfo = info.FontInfo;
				OwnOptions.UseFontInfo = true;
			}
#endif
			if (!OwnOptions.UseDoubleFontSize && options.UseDoubleFontSize) {
				OwnInfo.DoubleFontSize = info.DoubleFontSize;
				OwnOptions.UseDoubleFontSize = true;
			}
			if (!OwnOptions.UseFontStrikeoutType && options.UseFontStrikeoutType) {
				OwnInfo.FontStrikeoutType = info.FontStrikeoutType;
				OwnOptions.UseFontStrikeoutType = true;
			}
			if (!OwnOptions.UseFontUnderlineType && options.UseFontUnderlineType) {
				OwnInfo.FontUnderlineType = info.FontUnderlineType;
				OwnOptions.UseFontUnderlineType = true;
			}
			if (!OwnOptions.UseForeColor && options.UseForeColor) {
				OwnInfo.ForeColor = info.ForeColor;
#if THEMES_EDIT
				OwnInfo.ForeColorInfo = info.ForeColorInfo;
#endif
				OwnOptions.UseForeColor = true;
			}
			if (!OwnOptions.UseScript && options.UseScript) {
				OwnInfo.Script = info.Script;
				OwnOptions.UseScript = true;
			}
			if (!OwnOptions.UseStrikeoutColor && options.UseStrikeoutColor) {
				OwnInfo.StrikeoutColor = info.StrikeoutColor;
#if THEMES_EDIT
				OwnInfo.StrikeoutColorInfo = info.StrikeoutColorInfo;
#endif
				OwnOptions.UseStrikeoutColor = true;
			}
			if (!OwnOptions.UseStrikeoutWordsOnly && options.UseStrikeoutWordsOnly) {
				OwnInfo.StrikeoutWordsOnly = info.StrikeoutWordsOnly;
				OwnOptions.UseStrikeoutWordsOnly = true;
			}
			if (!OwnOptions.UseUnderlineColor && options.UseUnderlineColor) {
				OwnInfo.UnderlineColor = info.UnderlineColor;
#if THEMES_EDIT
				OwnInfo.UnderlineColorInfo = info.UnderlineColorInfo;
#endif
				OwnOptions.UseUnderlineColor = true;
			}
			if (!OwnOptions.UseUnderlineWordsOnly && options.UseUnderlineWordsOnly) {
				OwnInfo.UnderlineWordsOnly = info.UnderlineWordsOnly;
				OwnOptions.UseUnderlineWordsOnly = true;
			}
			if (!OwnOptions.UseHidden && options.UseHidden) {
				OwnInfo.Hidden = info.Hidden;
				OwnOptions.UseHidden = true;
			}
			if (!OwnOptions.UseLangInfo && options.UseLangInfo) {
				OwnInfo.LangInfo = info.LangInfo;
				OwnOptions.UseLangInfo = true;
			}
			if (!OwnOptions.UseNoProof && options.UseNoProof) {
				OwnInfo.NoProof = info.NoProof;
				OwnOptions.UseNoProof = true;
			}
		}
	}
	#endregion
	#region CharacterFormattingChangeType
	public enum CharacterFormattingChangeType {
		None = 0,
		FontName,
		FontSize,
		FontBold,
		FontItalic,
		FontUnderlineType,
		FontStrikeoutType,
		AllCaps,
		ForeColor,
		BackColor,
		UnderlineColor,
		StrikeoutColor,
		UnderlineWordsOnly,
		StrikeoutWordsOnly,
		Script,
		CharacterStyle,
		Hidden,
		LangInfo,
		NoProof,
		BatchUpdate
	}
	#endregion
	#region CharacterFormattingChangeActionsCalculator
	public static class CharacterFormattingChangeActionsCalculator {
		internal class CharacterFormattingChangeActionsTable : Dictionary<CharacterFormattingChangeType, DocumentModelChangeActions> {
		}
		internal static DocumentModelChangeActions[] characterFormattingChangeActionsTable = CreateCharacterFormattingChangeActionsTable();
		internal static DocumentModelChangeActions[] CreateCharacterFormattingChangeActionsTable() {
#if !SL
			DocumentModelChangeActions[] table = new DocumentModelChangeActions[Enum.GetValues(typeof(CharacterFormattingChangeType)).Length];
#else
			DocumentModelChangeActions[] table = new DocumentModelChangeActions[EnumExtensions.GetValues(typeof(CharacterFormattingChangeType)).Length];			
#endif
			table[(int)CharacterFormattingChangeType.None] = DocumentModelChangeActions.None;
			table[(int)CharacterFormattingChangeType.FontName] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.SplitRunByCharset;
			table[(int)CharacterFormattingChangeType.FontSize] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.FontBold] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.FontItalic] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.FontUnderlineType] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.FontStrikeoutType] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.AllCaps] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.ForeColor] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.BackColor] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetSecondaryLayout;
			table[(int)CharacterFormattingChangeType.UnderlineColor] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.StrikeoutColor] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.UnderlineWordsOnly] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.StrikeoutWordsOnly] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.Script] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting;
			table[(int)CharacterFormattingChangeType.Hidden] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ValidateSelectionInterval ;
			table[(int)CharacterFormattingChangeType.CharacterStyle] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.SplitRunByCharset ;
			table[(int)CharacterFormattingChangeType.BatchUpdate] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ValidateSelectionInterval ;
			table[(int)CharacterFormattingChangeType.LangInfo] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetUncheckedIntervals;
			table[(int)CharacterFormattingChangeType.NoProof] = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetUncheckedIntervals | DocumentModelChangeActions.ValidateSelectionInterval;
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(CharacterFormattingChangeType change) {
			return characterFormattingChangeActionsTable[(int)change];
		}
	}
	#endregion
	#region CharacterPropertiesFontAssignmentHelper
	public static class CharacterPropertiesFontAssignmentHelper {
		public static void AssignFont(ICharacterProperties characterProperties, Font font) {
			characterProperties.FontName = font.Name;
			characterProperties.DoubleFontSize = (int)Math.Round(DevExpress.XtraPrinting.Native.FontSizeHelper.GetSizeInPoints(font))*2;
			characterProperties.FontBold = font.Bold;
			characterProperties.FontItalic = font.Italic;
			characterProperties.FontUnderlineType = font.Underline ? DevExpress.XtraRichEdit.Model.UnderlineType.Single : DevExpress.XtraRichEdit.Model.UnderlineType.None;
			characterProperties.FontStrikeoutType = font.Strikeout ? DevExpress.XtraRichEdit.Model.StrikeoutType.Single : DevExpress.XtraRichEdit.Model.StrikeoutType.None;
		}
	}
	#endregion
	public enum JSONLangInfoProperty {
		Latin = 0,
		Bidi = 1,
		EastAsia = 2
	}
	[HashtableConverter("DevExpress.Web.ASPxRichEdit.Export.LangInfoExporter", true)]
	public struct LangInfo {
		readonly CultureInfo latin;
		readonly CultureInfo bidi;
		readonly CultureInfo eastAsia;
		public LangInfo (CultureInfo latin, CultureInfo bidi, CultureInfo eastAsia) {
			this.latin = latin;
			this.bidi = bidi;
			this.eastAsia = eastAsia;
		}
		[JSONEnum((int)JSONLangInfoProperty.Latin)]
		public CultureInfo Latin { get { return latin; } }
		[JSONEnum((int)JSONLangInfoProperty.Bidi)]
		public CultureInfo Bidi { get { return bidi; } }
		[JSONEnum((int)JSONLangInfoProperty.EastAsia)]
		public CultureInfo EastAsia { get { return eastAsia; } }
		public override bool Equals(object obj) {
			if (obj is LangInfo) {
				LangInfo other = (LangInfo)obj;
				return (EqualsField(other.latin, this.latin) && EqualsField(other.bidi, this.bidi) && EqualsField(other.eastAsia, this.eastAsia));				   
			}
			return false;
		}
		bool EqualsField(CultureInfo info1, CultureInfo info2) { 
			if ((info1 != null)&&(info2 !=null)) {
				return info1.Equals(info2);
			}
			else {
				return ((info1 == null) && (info2 == null)) ;
			}
		}
		public override int GetHashCode() {
			if (latin == null)
				return 0;
			else
				return latin.GetHashCode();		  
		}
		public override string ToString() {
			return String.Format("{0} {1} {2}", latin, bidi, eastAsia);
		}
	}
}
