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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Office.DrawingML;
namespace DevExpress.Office.Drawing {
	#region DrawingTextUnderlineType
	public enum DrawingTextUnderlineType {
		None = 0,
		Words,
		Single,
		Double,
		Heavy,
		Dotted,
		HeavyDotted,
		Dashed,
		HeavyDashed,
		LongDashed,
		HeavyLongDashed,
		DotDash, 
		HeavyDotDash,
		DotDotDash,
		HeavyDotDotDash,
		Wavy,
		HeavyWavy,
		DoubleWavy
	}
	#endregion
	#region DrawingTextStrikeType
	public enum DrawingTextStrikeType {
		None = 0,
		Single,
		Double
	}
	#endregion
	#region DrawingTextCapsType
	public enum DrawingTextCapsType {
		None = 0,
		Small,
		All
	}
	#endregion
	#region DrawingStrokeUnderlineType
	public enum DrawingStrokeUnderlineType {
		Automatic,
		Outline,
		FollowsText
	}
	#endregion
	#region DrawingTextUnderlineFillType
	public enum DrawingUnderlineFillType {
		Fill,
		FollowsText
	}
	#endregion
	#region DrawingTextCharacterInfo
	public class DrawingTextCharacterInfo : ICloneable<DrawingTextCharacterInfo>, ISupportsCopyFrom<DrawingTextCharacterInfo>, ISupportsSizeOf {
		readonly static DrawingTextCharacterInfo defaultInfo = new DrawingTextCharacterInfo();
		public static DrawingTextCharacterInfo DefaultInfo { get { return defaultInfo; } }
		#region Fields
		#region PackedValues
		const int offsetUnderline = 4;					  
		const int offsetStrikethrough = 9;
		const int offsetCaps = 11;
		const uint maskKumimoji = 0x0001;				   
		const uint maskBold = 0x0002;					   
		const uint maskItalic = 0x0004;					 
		const uint maskApplyFontSize = 0x0008;			  
		const uint maskUnderline = 0x01f0;				  
		const uint maskStrikethrough = 0x0600;			  
		const uint maskCaps = 0x1800;					   
		const uint maskNormalizeHeight = 0x2000;			
		const uint maskNoProofing = 0x4000;				 
		const uint maskDirty = 0x8000;					  
		const uint maskSpellingError = 0x10000;			 
		const uint maskSmartTagClean = 0x20000;			 
		#endregion
		#region PackedOptionsValues
		const uint maskHasKumimoji = 0x0001;				
		const uint maskHasFontSize = 0x0002;				
		const uint maskHasBold = 0x0004;					
		const uint maskHasItalic = 0x0008;				  
		const uint maskHasUnderline = 0x0010;			   
		const uint maskHasStrikethrough = 0x0020;		   
		const uint maskHasKerning = 0x0040;				 
		const uint maskHasCaps = 0x0080;					
		const uint maskHasSpacing = 0x0100;				 
		const uint maskHasNormalizeHeight = 0x0200;		 
		const uint maskHasBaseline = 0x0400;				
		const uint maskHasNoProofing = 0x0800;			  
		#endregion
		uint packedValues = 0x28000;
		uint packedOptionsValues;
		int fontSize = 10000;
		int kerning = 0;
		int spacing = 0;
		int baseline = 0;
		int smartTagId = 0;
		#endregion
		#region Properties
		public bool Kumimoji {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskKumimoji); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskKumimoji, value); }
		}
		public bool Bold {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskBold); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskBold, value); }
		}
		public bool Italic {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskItalic); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskItalic, value); }
		}
		public bool ApplyFontSize {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskApplyFontSize); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskApplyFontSize, value); }
		}
		public int FontSize {
			get { return fontSize; }
			set {
				ValueChecker.CheckValue(value, 100, 400000, "FontSize");
				fontSize = value;
			}
		}
		public DrawingTextUnderlineType Underline {
			get { return (DrawingTextUnderlineType)PackedValues.GetIntBitValue(this.packedValues, maskUnderline, offsetUnderline); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskUnderline, offsetUnderline, (int)value); }
		}
		public DrawingTextStrikeType Strikethrough {
			get { return (DrawingTextStrikeType)PackedValues.GetIntBitValue(this.packedValues, maskStrikethrough, offsetStrikethrough); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskStrikethrough, offsetStrikethrough, (int)value); }
		}
		public int Kerning {
			get { return kerning; }
			set {
				ValueChecker.CheckValue(value, 0, 400000, "Kerning");
				kerning = value;
			}
		}
		public DrawingTextCapsType Caps {
			get { return (DrawingTextCapsType)PackedValues.GetIntBitValue(this.packedValues, maskCaps, offsetCaps); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskCaps, offsetCaps, (int)value); }
		}
		public int Spacing {
			get { return spacing; }
			set {
				ValueChecker.CheckValue(value, -400000, 400000, "Spacing");
				spacing = value;
			}
		}
		public bool NormalizeHeight {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskNormalizeHeight); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskNormalizeHeight, value); }
		}
		public int Baseline { get { return baseline; } set { baseline = value; } }
		public bool NoProofing {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskNoProofing); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskNoProofing, value); }
		}
		public bool Dirty {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskDirty); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskDirty, value); }
		}
		public bool SpellingError {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskSpellingError); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskSpellingError, value); }
		}
		public bool SmartTagClean {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskSmartTagClean); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskSmartTagClean, value); }
		}
		public int SmartTagId {
			get { return smartTagId; }
			set {
				Guard.ArgumentNonNegative(value, "SmartTagId");
				smartTagId = value;
			}
		}
		public bool HasKumimoji {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasKumimoji); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasKumimoji, value); }
		}
		public bool HasFontSize {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasFontSize); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasFontSize, value); }
		}
		public bool HasBold {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasBold); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasBold, value); }
		}
		public bool HasItalic {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasItalic); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasItalic, value); }
		}
		public bool HasUnderline {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasUnderline); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasUnderline, value); }
		}
		public bool HasStrikethrough {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasStrikethrough); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasStrikethrough, value); }
		}
		public bool HasKerning {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasKerning); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasKerning, value); }
		}
		public bool HasCaps {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasCaps); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasCaps, value); }
		}
		public bool HasSpacing {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasSpacing); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasSpacing, value); }
		}
		public bool HasNormalizeHeight {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasNormalizeHeight); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasNormalizeHeight, value); }
		}
		public bool HasBaseline {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasBaseline); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasBaseline, value); }
		}
		public bool HasNoProofing {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasNoProofing); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasNoProofing, value); }
		}
		#endregion
		#region ICloneable<DrawingTextCharacterInfo> Members
		public DrawingTextCharacterInfo Clone() {
			DrawingTextCharacterInfo result = new DrawingTextCharacterInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingTextCharacterInfo> Members
		public void CopyFrom(DrawingTextCharacterInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.packedOptionsValues = value.packedOptionsValues;
			this.fontSize = value.fontSize;
			this.kerning = value.kerning;
			this.spacing = value.spacing;
			this.baseline = value.baseline;
			this.smartTagId = value.smartTagId;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			DrawingTextCharacterInfo other = obj as DrawingTextCharacterInfo;
			if (other == null)
				return false;
			return 
				this.packedValues == other.packedValues &&
				this.packedOptionsValues == other.packedOptionsValues &&
				this.fontSize == other.fontSize &&
				this.kerning == other.kerning &&
				this.spacing == other.spacing &&
				this.baseline == other.baseline &&
				this.smartTagId == other.smartTagId;
		}
		public override int GetHashCode() {
			return 
				(int)this.packedValues ^ (int)this.packedOptionsValues ^ fontSize ^ kerning ^ spacing ^ baseline ^ smartTagId;
		}
	}
	#endregion
	#region DrawingTextCharacterInfoCache
	public class DrawingTextCharacterInfoCache : UniqueItemsCache<DrawingTextCharacterInfo> {
		public const int DefaultItemIndex = 0;
		public DrawingTextCharacterInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DrawingTextCharacterInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DrawingTextCharacterInfo();
		}
	}
	#endregion
	#region IStrokeUnderline
	public interface IStrokeUnderline {
		DrawingStrokeUnderlineType Type { get; }
		IStrokeUnderline CloneTo(IDocumentModel documentModel);
	}
	#endregion
	#region DrawingStrokeUnderline
	public sealed class DrawingStrokeUnderline : IStrokeUnderline {
		public static DrawingStrokeUnderline Automatic = new DrawingStrokeUnderline(DrawingStrokeUnderlineType.Automatic);
		public static DrawingStrokeUnderline FollowsText = new DrawingStrokeUnderline(DrawingStrokeUnderlineType.FollowsText);
		DrawingStrokeUnderlineType type;
		DrawingStrokeUnderline(DrawingStrokeUnderlineType type) {
			this.type = type;
		}
		#region Equals
		public override bool Equals(object obj) {
			DrawingStrokeUnderline other = obj as DrawingStrokeUnderline;
			return other != null && type == other.Type;
		}
		public override int GetHashCode() {
			return (int)type;
		}
		#endregion
		#region IStrokeUnderline Members
		public DrawingStrokeUnderlineType Type { get { return type; } }
		public IStrokeUnderline CloneTo(IDocumentModel documentModel) {
			if (type == DrawingStrokeUnderlineType.FollowsText)
				return DrawingStrokeUnderline.FollowsText;
			return DrawingStrokeUnderline.Automatic;
		}
		#endregion
	}
	#endregion
	#region IUnderlineFill
	public interface IUnderlineFill {
		DrawingUnderlineFillType Type { get; }
		IUnderlineFill CloneTo(IDocumentModel documentModel);
	}
	#endregion
	#region DrawingUnderlineFill
	public sealed class DrawingUnderlineFill : IUnderlineFill {
		public static DrawingUnderlineFill FollowsText = new DrawingUnderlineFill();
		DrawingUnderlineFill() {
		}
		public override bool Equals(object obj) {
			return obj is DrawingUnderlineFill;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode();
		}
		#region IUnderlineFill Members
		public DrawingUnderlineFillType Type { get { return DrawingUnderlineFillType.FollowsText; } }
		public IUnderlineFill CloneTo(IDocumentModel documentModel) {
			return DrawingUnderlineFill.FollowsText;
		}
		#endregion
	}
	#endregion
	#region ITextCharacterOptions
	public interface ITextCharacterOptions {
		bool HasKumimoji { get; }
		bool HasFontSize { get; }
		bool HasBold { get; }
		bool HasItalic { get; }
		bool HasUnderline { get; }
		bool HasStrikethrough { get; }
		bool HasKerning { get; }
		bool HasCaps { get; }
		bool HasSpacing { get; }
		bool HasNormalizeHeight { get; }
		bool HasBaseline { get; }
		bool HasNoProofing { get; }
	}
	#endregion
	#region DrawingTextCharacterProperties
	public class DrawingTextCharacterProperties : DrawingUndoableIndexBasedObject<DrawingTextCharacterInfo>, ICloneable<DrawingTextCharacterProperties>, ISupportsCopyFrom<DrawingTextCharacterProperties>, IFillOwner, ITextCharacterOptions {
		#region Fields
		IDrawingFill fill;
		Outline outline;
		ContainerEffect effects;
		DrawingColor highlight;
		IUnderlineFill underlineFill;
		IStrokeUnderline strokeUnderline;
		DrawingTextFont eastAsian;
		DrawingTextFont latin;
		DrawingTextFont symbol;
		DrawingTextFont complexScript;
		CultureInfo language;
		CultureInfo alternateLanguage;
		string bookmark = string.Empty;
		#endregion
		public DrawingTextCharacterProperties(IDocumentModel documentModel) 
			: base(documentModel.MainPart) {
			this.fill = DrawingFill.Automatic;
			this.outline = new Outline(documentModel) { Parent = InnerParent };
			this.effects = new ContainerEffect(documentModel);
			this.highlight = new DrawingColor(documentModel) { Parent = InnerParent };
			this.underlineFill = DrawingFill.Automatic;
			this.strokeUnderline = DrawingStrokeUnderline.Automatic;
			this.eastAsian = new DrawingTextFont(documentModel) { Parent = InnerParent };
			this.latin = new DrawingTextFont(documentModel) { Parent = InnerParent };
			this.symbol = new DrawingTextFont(documentModel) { Parent = InnerParent };
			this.complexScript = new DrawingTextFont(documentModel) { Parent = InnerParent };
			this.language = CultureInfo.InvariantCulture;
			this.alternateLanguage = CultureInfo.InvariantCulture;
		}
		#region Properties
		#region Kumimoji
		public bool Kumimoji {
			get { return Info.Kumimoji; }
			set {
				if (Kumimoji == value && Options.HasKumimoji)
					return;
				SetPropertyValue(SetKumimojiCore, value);
			}
		}
		DocumentModelChangeActions SetKumimojiCore(DrawingTextCharacterInfo info, bool value) {
			info.Kumimoji = value;
			info.HasKumimoji = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Bold
		public bool Bold {
			get { return Info.Bold; }
			set {
				if (Bold == value && Options.HasBold)
					return;
				SetPropertyValue(SetBoldCore, value);
			}
		}
		DocumentModelChangeActions SetBoldCore(DrawingTextCharacterInfo info, bool value) {
			info.Bold = value;
			info.HasBold = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Italic
		public bool Italic {
			get { return Info.Italic; }
			set {
				if (Italic == value && Options.HasItalic)
					return;
				SetPropertyValue(SetItalicCore, value);
			}
		}
		DocumentModelChangeActions SetItalicCore(DrawingTextCharacterInfo info, bool value) {
			info.Italic = value;
			info.HasItalic = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ApplyFontSize
		public bool ApplyFontSize {
			get { return Info.ApplyFontSize; }
			set {
				if (ApplyFontSize == value)
					return;
				SetPropertyValue(SetApplyFontSizeCore, value);
			}
		}
		DocumentModelChangeActions SetApplyFontSizeCore(DrawingTextCharacterInfo info, bool value) {
			info.ApplyFontSize = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FontSize
		public int FontSize {
			get { return Info.FontSize; }
			set {
				if (FontSize == value && Options.HasFontSize)
					return;
				SetPropertyValue(SetFontSizeCore, value);
			}
		}
		DocumentModelChangeActions SetFontSizeCore(DrawingTextCharacterInfo info, int value) {
			info.FontSize = value;
			info.HasFontSize = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Underline
		public DrawingTextUnderlineType Underline {
			get { return Info.Underline; }
			set {
				if (Underline == value && Options.HasUnderline)
					return;
				SetPropertyValue(SetUnderlineCore, value);
			}
		}
		DocumentModelChangeActions SetUnderlineCore(DrawingTextCharacterInfo info, DrawingTextUnderlineType value) {
			info.Underline = value;
			info.HasUnderline = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Strikethrough
		public DrawingTextStrikeType Strikethrough {
			get { return Info.Strikethrough; }
			set {
				if (Strikethrough == value && Options.HasStrikethrough)
					return;
				SetPropertyValue(SetStrikethroughCore, value);
			}
		}
		DocumentModelChangeActions SetStrikethroughCore(DrawingTextCharacterInfo info, DrawingTextStrikeType value) {
			info.Strikethrough = value;
			info.HasStrikethrough = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Kerning
		public int Kerning {
			get { return Info.Kerning; }
			set {
				if (Kerning == value && Options.HasKerning)
					return;
				SetPropertyValue(SetKerningCore, value);
			}
		}
		DocumentModelChangeActions SetKerningCore(DrawingTextCharacterInfo info, int value) {
			info.Kerning = value;
			info.HasKerning = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Caps
		public DrawingTextCapsType Caps {
			get { return Info.Caps; }
			set {
				if (Caps == value && Options.HasCaps)
					return;
				SetPropertyValue(SetCapsCore, value);
			}
		}
		DocumentModelChangeActions SetCapsCore(DrawingTextCharacterInfo info, DrawingTextCapsType value) {
			info.Caps = value;
			info.HasCaps = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Spacing
		public int Spacing {
			get { return Info.Spacing; }
			set {
				if (Spacing == value && Options.HasSpacing)
					return;
				SetPropertyValue(SetSpacingCore, value);
			}
		}
		DocumentModelChangeActions SetSpacingCore(DrawingTextCharacterInfo info, int value) {
			info.Spacing = value;
			info.HasSpacing = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region NormalizeHeight
		public bool NormalizeHeight {
			get { return Info.NormalizeHeight; }
			set {
				if (NormalizeHeight == value && Options.HasNormalizeHeight)
					return;
				SetPropertyValue(SetNormalizeHeightCore, value);
			}
		}
		DocumentModelChangeActions SetNormalizeHeightCore(DrawingTextCharacterInfo info, bool value) {
			info.NormalizeHeight = value;
			info.HasNormalizeHeight = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Baseline
		public int Baseline {
			get { return Info.Baseline; }
			set {
				if (Baseline == value && Options.HasBaseline)
					return;
				SetPropertyValue(SetBaselineCore, value);
			}
		}
		DocumentModelChangeActions SetBaselineCore(DrawingTextCharacterInfo info, int value) {
			info.Baseline = value;
			info.HasBaseline = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region NoProofing
		public bool NoProofing {
			get { return Info.NoProofing; }
			set {
				if (NoProofing == value && Options.HasNoProofing)
					return;
				SetPropertyValue(SetNoProofingCore, value);
			}
		}
		DocumentModelChangeActions SetNoProofingCore(DrawingTextCharacterInfo info, bool value) {
			info.NoProofing = value;
			info.HasNoProofing = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Dirty
		public bool Dirty {
			get { return Info.Dirty; }
			set {
				if (Dirty == value)
					return;
				SetPropertyValue(SetDirtyCore, value);
			}
		}
		DocumentModelChangeActions SetDirtyCore(DrawingTextCharacterInfo info, bool value) {
			info.Dirty = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SpellingError
		public bool SpellingError {
			get { return Info.SpellingError; }
			set {
				if (SpellingError == value)
					return;
				SetPropertyValue(SetSpellingErrorCore, value);
			}
		}
		DocumentModelChangeActions SetSpellingErrorCore(DrawingTextCharacterInfo info, bool value) {
			info.SpellingError = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SmartTagClean
		public bool SmartTagClean {
			get { return Info.SmartTagClean; }
			set {
				if (SmartTagClean == value)
					return;
				SetPropertyValue(SetSmartTagCleanCore, value);
			}
		}
		DocumentModelChangeActions SetSmartTagCleanCore(DrawingTextCharacterInfo info, bool value) {
			info.SmartTagClean = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SmartTagId
		public int SmartTagId {
			get { return Info.SmartTagId; }
			set {
				if (SmartTagId == value)
					return;
				SetPropertyValue(SetSmartTagIdCore, value);
			}
		}
		DocumentModelChangeActions SetSmartTagIdCore(DrawingTextCharacterInfo info, int value) {
			info.SmartTagId = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AlternateLanguage
		public CultureInfo AlternateLanguage {
			get { return alternateLanguage; }
			set {
				Guard.ArgumentNotNull(value, "AlternateLanguage");
				if (alternateLanguage == value)
					return;
				ApplyHistoryItem(new DrawingLanguageChangedHistoryItem(this, SetAlternateLanguageCore, alternateLanguage, value));
			}
		}
		public void SetAlternateLanguageCore(CultureInfo value) {
			this.alternateLanguage = value;
			InvalidateParent();
		}
		#endregion
		#region Language
		public CultureInfo Language {
			get { return language; }
			set {
				Guard.ArgumentNotNull(value, "Language");
				if (language == value)
					return;
				ApplyHistoryItem(new DrawingLanguageChangedHistoryItem(this, SetLanguageCore, alternateLanguage, value));
			}
		}
		public void SetLanguageCore(CultureInfo value) {
			this.language = value;
			InvalidateParent();
		}
		#endregion
		#region Fill
		public IDrawingFill Fill {
			get { return fill; }
			set {
				if (value == null)
					value = DrawingFill.Automatic;
				if (fill.Equals(value))
					return;
				HistoryItem item = new DrawingFillPropertyChangedHistoryItem(DocumentModel.MainPart, this, fill, value);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		public void SetDrawingFillCore(IDrawingFill value) {
			this.fill.Parent = null;
			this.fill = value;
			this.fill.Parent = InnerParent;
			InvalidateParent();
		}
		#endregion
		public Outline Outline { get { return outline; } }
		public ContainerEffect Effects { get { return effects; } }
		public DrawingColor Highlight { get { return highlight; } }
		#region UnderlineFill
		public IUnderlineFill UnderlineFill {
			get { return underlineFill; }
			set {
				if (value == null)
					value = DrawingFill.Automatic;
				if (underlineFill == value)
					return;
				ApplyHistoryItem(new DrawingUnderlineFillChangedHistoryItem(this, underlineFill, value));
			}
		}
		public void SetUnderlineFillCore(IUnderlineFill value) {
			this.underlineFill = value;
			InvalidateParent();
		}
		#endregion
		#region Bookmark
		public string Bookmark {
			get { return bookmark; }
			set {
				if (value == null)
					value = string.Empty;
				if (bookmark == value)
					return;
				DrawingTextCharacterPropertiesBookmarkChangedHistoryItem item = new DrawingTextCharacterPropertiesBookmarkChangedHistoryItem(this, bookmark, value);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		protected internal void SetBookmarkCore(string value) {
			bookmark = value;
			InvalidateParent();
		}
		#endregion
		public DrawingTextFont Latin { get { return latin; } }
		public DrawingTextFont EastAsian { get { return eastAsian; } }
		public DrawingTextFont ComplexScript { get { return complexScript; } }
		public DrawingTextFont Symbol { get { return symbol; } }
		public ITextCharacterOptions Options { get { return this; } }
		#region StrokeUnderline
		public IStrokeUnderline StrokeUnderline { 
			get { return strokeUnderline; } 
			set {
				if (value == null)
					value = DrawingStrokeUnderline.Automatic;
				if (strokeUnderline == value)
					return;
				ApplyHistoryItem(new DrawingStrokeUnderlineChangedHistoryItem(this, strokeUnderline, value));
			}
		}
		public void SetStrokeUnderlineCore(IStrokeUnderline value) {
			strokeUnderline = value;
			InvalidateParent();
		}
		#endregion
		void ApplyHistoryItem(HistoryItem item) {
			DocumentModel.History.Add(item);
			item.Execute();
		}
		#region ICharacterOptions Members
		bool ITextCharacterOptions.HasKumimoji { get { return Info.HasKumimoji; } }
		bool ITextCharacterOptions.HasFontSize { get { return Info.HasFontSize; } }
		bool ITextCharacterOptions.HasBold { get { return Info.HasBold; } }
		bool ITextCharacterOptions.HasItalic { get { return Info.HasItalic; } }
		bool ITextCharacterOptions.HasUnderline { get { return Info.HasUnderline; } }
		bool ITextCharacterOptions.HasStrikethrough { get { return Info.HasStrikethrough; } }
		bool ITextCharacterOptions.HasKerning { get { return Info.HasKerning; } }
		bool ITextCharacterOptions.HasCaps { get { return Info.HasCaps; } }
		bool ITextCharacterOptions.HasSpacing { get { return Info.HasSpacing; } }
		bool ITextCharacterOptions.HasNormalizeHeight { get { return Info.HasNormalizeHeight; } }
		bool ITextCharacterOptions.HasBaseline { get { return Info.HasBaseline; } }
		bool ITextCharacterOptions.HasNoProofing { get { return Info.HasNoProofing; } }
		#endregion
		public bool IsDefault { 
			get {
				return
					fill == DrawingFill.Automatic && outline.IsDefault &&
					effects.Effects.Count == 0 && highlight.IsEmpty &&
					underlineFill == DrawingFill.Automatic &&
					strokeUnderline == DrawingStrokeUnderline.Automatic 
					&& eastAsian.IsDefault && latin.IsDefault && 
					symbol.IsDefault && complexScript.IsDefault &&
					Index == DrawingTextCharacterInfoCache.DefaultItemIndex &&
					String.IsNullOrEmpty(bookmark) && 
					language.Equals(CultureInfo.InvariantCulture) && 
					alternateLanguage.Equals(CultureInfo.InvariantCulture);
				} 
		}
		#endregion
		#region ISupportsCopyFrom<DrawingTextCharacterProperties> Members
		public void CopyFrom(DrawingTextCharacterProperties value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			this.fill = value.fill.CloneTo(DocumentModel);
			this.outline.CopyFrom(value.outline);
			this.effects.CopyFrom(value.effects);
			this.highlight.CopyFrom(value.highlight);
			this.underlineFill = value.underlineFill.CloneTo(DocumentModel);
			this.strokeUnderline = value.strokeUnderline.CloneTo(DocumentModel);
			this.bookmark = value.bookmark;
			this.complexScript.CopyFrom(value.complexScript);
			this.eastAsian.CopyFrom(value.eastAsian);
			this.latin.CopyFrom(value.latin);
			this.symbol.CopyFrom(value.symbol);
			this.language = (CultureInfo)value.language.Clone();
			this.alternateLanguage = (CultureInfo)value.alternateLanguage.Clone();
		}
		#endregion
		protected internal override UniqueItemsCache<DrawingTextCharacterInfo> GetCache(IDocumentModel documentModel) {
			return DrawingCache.DrawingTextCharacterInfoCache;
		}
		public override bool Equals(object obj) {
			DrawingTextCharacterProperties other = obj as DrawingTextCharacterProperties;
			if (other == null)
				return false;
			return 
				Info.Equals(other.Info) && fill.Equals(other.fill) && outline.Equals(other.outline) && effects.Equals(other.effects) &&
				highlight.Equals(other.highlight) && underlineFill.Equals(other.underlineFill) &&
				strokeUnderline.Equals(other.strokeUnderline) && bookmark == other.bookmark && latin.Equals(other.latin) && 
				eastAsian.Equals(other.eastAsian) && complexScript.Equals(other.complexScript) && symbol.Equals(other.symbol) &&
				language.Equals(other.language) && alternateLanguage.Equals(other.alternateLanguage);
		}
		public override int GetHashCode() {
			return 
				Info.GetHashCode() ^ fill.GetHashCode() ^ outline.GetHashCode() ^ effects.GetHashCode() ^ highlight.GetHashCode() ^
				underlineFill.GetHashCode() ^ strokeUnderline.GetHashCode() ^ bookmark.GetHashCode() ^ latin.GetHashCode() ^
				eastAsian.GetHashCode() ^ complexScript.GetHashCode() ^ symbol.GetHashCode() ^ language.GetHashCode() ^
				alternateLanguage.GetHashCode();
		}
		public DrawingTextCharacterProperties Clone() {
			DrawingTextCharacterProperties result = new DrawingTextCharacterProperties(DocumentModel);
			result.CopyFrom(this);
			return result;
		}
		public void ResetToStyle() {
			if (IsUpdateLocked)
				Info.CopyFrom(DrawingCache.DrawingTextCharacterInfoCache.DefaultItem);
			else
				ChangeIndexCore(DrawingTextCharacterInfoCache.DefaultItemIndex, DocumentModelChangeActions.None);
			Fill = DrawingFill.Automatic;
			this.outline.ResetToStyle();
			this.effects.Effects.Clear();
			this.highlight.Clear();
			UnderlineFill = DrawingFill.Automatic;
			StrokeUnderline = DrawingStrokeUnderline.Automatic;
			this.eastAsian.Clear();
			this.latin.Clear();
			this.symbol.Clear();
			this.complexScript.Clear();
			Bookmark = string.Empty;
			Language = CultureInfo.InvariantCulture;
			AlternateLanguage = CultureInfo.InvariantCulture;
		}
	}
	#endregion
}
