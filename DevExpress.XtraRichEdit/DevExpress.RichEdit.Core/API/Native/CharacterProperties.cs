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
using DevExpress.XtraRichEdit.Model;
using System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.API.Native {
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217")]
	[Flags]
	public enum CharacterPropertiesMask {
			None = 0x00000000,
			FontName = 0x00000001,
			FontSize = 0x00000002,
			Bold = 0x00000004,
			Italic = 0x00000008,
			StrikeoutType = 0x00000010,
			UnderlineType = 0x00000020,
			AllCaps = 0x00000040,
			ForeColor = 0x00000080,
			BackColor = 0x00000100,
			UnderlineColor = 0x00000200,
			Script = 0x00002000,
			Hidden = 0x00004000,
			Language = 0x00008000,
			NoProof = 0x00010000,
			All = 0x7FFFFFFF
	}
	#region UnderlineType
	[ComVisible(true)]
	public enum UnderlineType {
		None = 0,
		Single = 1,
		Dotted = 2,
		Dashed = 3,
		DashDotted = 4,
		DashDotDotted = 5,
		Double = 6,
		HeavyWave = 7,
		LongDashed = 8,
		ThickSingle = 9,
		ThickDotted = 10,
		ThickDashed = 11,
		ThickDashDotted = 12,
		ThickDashDotDotted = 13,
		ThickLongDashed = 14,
		DoubleWave = 15,
		Wave = 16,
	}
	#endregion
	#region StrikeoutType
	[ComVisible(true)]
	public enum StrikeoutType {
		None = 0,
		Single = 1,
		Double = 2
	}
	#endregion
	#region CharacterPropertiesBase
	[ComVisible(true)]
	public interface CharacterPropertiesBase {
		string FontName { get; set; }
		float? FontSize { get; set; }
		bool? Bold { get; set; }
		bool? Italic { get; set; }
		bool? Superscript { get; set; }
		bool? Subscript { get; set; }
		bool? AllCaps { get; set; }
		UnderlineType? Underline { get; set; }
		StrikeoutType? Strikeout { get; set; }
		Color? UnderlineColor { get; set; }
		Color? ForeColor { get; set; }
		Color? BackColor { get; set; }
		bool? Hidden { get; set; }
		LangInfo? Language { get; set; }
		bool? NoProof { get; set; }
		void Reset();
		void Reset(CharacterPropertiesMask mask);
	}
	#endregion
	#region CharacterProperties
	[ComVisible(true)]
	public interface CharacterProperties : CharacterPropertiesBase {
		CharacterStyle Style { get; set; }
	}
	#endregion
	#region SyntaxHighlightToken
	[ComVisible(true)]
	public class SyntaxHighlightToken {
		int start;
		int length;
		SyntaxHighlightProperties properties;
		public SyntaxHighlightToken() {
		}
		public SyntaxHighlightToken(int start, int length, SyntaxHighlightProperties properties) {
			this.start = start;
			this.length = length;
			this.properties = properties;
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SyntaxHighlightTokenStart")]
#endif
		public int Start { get { return start; } set { start = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SyntaxHighlightTokenEnd")]
#endif
		public int End { get { return start + length; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SyntaxHighlightTokenLength")]
#endif
		public int Length { get { return length; } set { length = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SyntaxHighlightTokenProperties")]
#endif
		public SyntaxHighlightProperties Properties { get { return properties; } set { properties = value; } }
	}
	#endregion
	#region SyntaxHighlightProperties
	[ComVisible(true)]
	public class SyntaxHighlightProperties {
		UnderlineType? underline;
		StrikeoutType? strikeout;
		Color? underlineColor;
		Color? foreColor;
		Color? backColor;
		public SyntaxHighlightProperties() {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SyntaxHighlightPropertiesUnderline")]
#endif
		public UnderlineType? Underline { get { return underline; } set { underline = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SyntaxHighlightPropertiesStrikeout")]
#endif
		public StrikeoutType? Strikeout { get { return strikeout; } set { strikeout = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SyntaxHighlightPropertiesUnderlineColor")]
#endif
		public Color? UnderlineColor { get { return underlineColor; } set { underlineColor = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SyntaxHighlightPropertiesForeColor")]
#endif
		public Color? ForeColor { get { return foreColor; } set { foreColor = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SyntaxHighlightPropertiesBackColor")]
#endif
		public Color? BackColor { get { return backColor; } set { backColor = value; } }
		internal void ApplyTo(DevExpress.XtraRichEdit.Model.TextRunBase textRunBase) {
			if (Underline.HasValue)
				textRunBase.FontUnderlineType = (DevExpress.XtraRichEdit.Model.UnderlineType)Underline.Value;
			if (Strikeout.HasValue)
				textRunBase.FontStrikeoutType = (DevExpress.XtraRichEdit.Model.StrikeoutType)Strikeout.Value;
			if (UnderlineColor.HasValue)
				textRunBase.UnderlineColor = UnderlineColor.Value;
			if (ForeColor.HasValue)
				textRunBase.ForeColor = ForeColor.Value;
			if (BackColor.HasValue)
				textRunBase.BackColor = BackColor.Value;
		}
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(obj, this))
				return true;
			SyntaxHighlightProperties other = obj as SyntaxHighlightProperties;
			if (Object.ReferenceEquals(other, null))
				return false;
			return ForeColor == other.ForeColor && BackColor == other.BackColor && Underline == other.Underline && Strikeout == other.Strikeout && UnderlineColor == other.UnderlineColor;
		}
		public override int GetHashCode() {
			return ForeColor.HasValue ? ForeColor.GetHashCode() : (BackColor.HasValue ? BackColor.GetHashCode() : 0);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using DevExpress.XtraRichEdit.API.Internal;
	using DevExpress.XtraRichEdit.Localization;
	using DevExpress.XtraRichEdit.Utils;
	using DevExpress.Utils;
	using ModelRunIndex = DevExpress.XtraRichEdit.Model.RunIndex;
	using ModelRun = DevExpress.XtraRichEdit.Model.TextRunBase;
	using ModelCharacterScript = DevExpress.Office.CharacterFormattingScript;
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using ModelLogPosition = DevExpress.XtraRichEdit.Model.DocumentLogPosition;
	using ModelUnderlineType = DevExpress.XtraRichEdit.Model.UnderlineType;
	using ModelStrikeoutType = DevExpress.XtraRichEdit.Model.StrikeoutType;
	using ModelRangePropertyModifierBase = DevExpress.XtraRichEdit.Model.RunPropertyModifierBase;
	#region NativeCharacterPropertiesBase (abstract class)
	public abstract class NativeCharacterPropertiesBase : CharacterPropertiesBase {
		#region Fields
		bool isValid = true;
		PropertyAccessor<string> fontName;
		PropertyAccessor<int?> fontSize;
		PropertyAccessor<bool?> fontBold;
		PropertyAccessor<bool?> fontItalic;
		PropertyAccessor<ModelUnderlineType?> fontUnderline;
		PropertyAccessor<ModelStrikeoutType?> fontStrikeout;
		PropertyAccessor<Color?> foreColor;
		PropertyAccessor<Color?> underlineColor;
		PropertyAccessor<Color?> backColor;
		PropertyAccessor<bool?> allCaps;
		PropertyAccessor<bool?> hidden;
		PropertyAccessor<ModelCharacterScript?> fontScript;
		PropertyAccessor<LangInfo?> language;
		PropertyAccessor<bool?> resetAccessor;
		PropertyAccessor<CharacterFormattingOptions.Mask?> resetMaskAccessor;
		PropertyAccessor<bool?> noProof;
		#endregion
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		#region CharacterProperties Members
		#region FontName
		public string FontName {
			get {
				CheckValid();
				return fontName.GetValue();
			}
			set {
				CheckValid();
				if (String.IsNullOrEmpty(value))
					return;
				if (fontName.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region FontSize
		public float? FontSize {
			get {
				CheckValid();
				return fontSize.GetValue() / 2f;
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (fontSize.SetValue((int)Math.Round(value.Value * 2)))
					RaiseChanged();
			}
		}
		#endregion
		#region Bold
		public bool? Bold {
			get {
				CheckValid();
				return fontBold.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (fontBold.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region Italic
		public bool? Italic {
			get {
				CheckValid();
				return fontItalic.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (fontItalic.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region AllCaps
		public bool? AllCaps {
			get {
				CheckValid();
				return allCaps.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (allCaps.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region Hidden
		public bool? Hidden {
			get {
				CheckValid();
				return hidden.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (hidden.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region Superscript
		public bool? Superscript {
			get {
				CheckValid();
				CheckValid();
				ModelCharacterScript? value = fontScript.GetValue();
				if (value.HasValue)
					return value == ModelCharacterScript.Superscript;
				else
					return null;
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				ModelCharacterScript val = value.Value ? ModelCharacterScript.Superscript : ModelCharacterScript.Normal;
				if (fontScript.SetValue(val))
					RaiseChanged();
			}
		}
		#endregion
		#region Subscript
		public bool? Subscript {
			get {
				CheckValid();
				ModelCharacterScript? value = fontScript.GetValue();
				if (value.HasValue)
					return value == ModelCharacterScript.Subscript;
				else
					return null;
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				ModelCharacterScript val = value.Value ? ModelCharacterScript.Subscript : ModelCharacterScript.Normal;
				if (fontScript.SetValue(val))
					RaiseChanged();
			}
		}
		#endregion
		#region Underline
		public UnderlineType? Underline {
			get {
				CheckValid();
				ModelUnderlineType? value = fontUnderline.GetValue();
				if (value.HasValue)
					return (UnderlineType)value.Value;
				else
					return null;
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (fontUnderline.SetValue((ModelUnderlineType)value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region Strikeout
		public StrikeoutType? Strikeout {
			get {
				CheckValid();
				ModelStrikeoutType? value = fontStrikeout.GetValue();
				if (value.HasValue)
					return (StrikeoutType)value.Value;
				else
					return null;
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (fontStrikeout.SetValue((ModelStrikeoutType)value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region ForeColor
		public Color? ForeColor {
			get {
				CheckValid();
				return foreColor.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (foreColor.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region UnderlineColor
		public Color? UnderlineColor {
			get {
				CheckValid();
				return underlineColor.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (underlineColor.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region BackColor
		public Color? BackColor {
			get {
				CheckValid();
				return backColor.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (backColor.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region Language
		public LangInfo? Language {
			get {
				CheckValid();
				return language.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (language.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region NoProof
		public bool? NoProof {
			get {
				CheckValid();
				return noProof.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (noProof.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#endregion
		protected void CheckValid() {
			if (!isValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseInvalidCharacterProperties);
		}
		protected internal virtual void CreateAccessors() {
			this.fontName = CreateFontNameAccessor();
			this.fontSize = CreateFontSizeAccessor();
			this.fontBold = CreateFontBoldAccessor();
			this.fontItalic = CreateFontItalicAccessor();
			this.fontUnderline = CreateFontUnderlineAccessor();
			this.fontStrikeout = CreateFontStrikeoutAccessor();
			this.foreColor = CreateForeColorAccessor();
			this.underlineColor = CreateUnderlineColorAccessor();
			this.backColor = CreateBackColorAccessor();
			this.allCaps = CreateAllCapsAccessor();
			this.hidden = CreateHiddenAccessor();
			this.fontScript = CreateFontScriptAccessor();
			this.resetAccessor = CreateResetAccessor();
			this.resetMaskAccessor = CreateResetMaskAccessor();
			this.language = CreateLanguageAccessor();
			this.noProof = CreateNoProofAccessor();
		}
		public virtual void Reset() {
			CheckValid();
			if (resetAccessor.SetValue(true))
				RaiseChanged();
		}
		public virtual void Reset(CharacterPropertiesMask mask) {
			CheckValid();
			if (resetMaskAccessor.SetValue((CharacterFormattingOptions.Mask)mask))
				RaiseChanged();
		}
		protected internal abstract void RaiseChanged();
		protected internal abstract PropertyAccessor<string> CreateFontNameAccessor();
		protected internal abstract PropertyAccessor<int?> CreateFontSizeAccessor();
		protected internal abstract PropertyAccessor<bool?> CreateFontBoldAccessor();
		protected internal abstract PropertyAccessor<bool?> CreateFontItalicAccessor();
		protected internal abstract PropertyAccessor<ModelUnderlineType?> CreateFontUnderlineAccessor();
		protected internal abstract PropertyAccessor<ModelStrikeoutType?> CreateFontStrikeoutAccessor();
		protected internal abstract PropertyAccessor<Color?> CreateForeColorAccessor();
		protected internal abstract PropertyAccessor<Color?> CreateUnderlineColorAccessor();
		protected internal abstract PropertyAccessor<Color?> CreateBackColorAccessor();
		protected internal abstract PropertyAccessor<bool?> CreateAllCapsAccessor();
		protected internal abstract PropertyAccessor<bool?> CreateHiddenAccessor();
		protected internal abstract PropertyAccessor<ModelCharacterScript?> CreateFontScriptAccessor();
		protected internal abstract PropertyAccessor<bool?> CreateResetAccessor();
		protected internal abstract PropertyAccessor<CharacterFormattingOptions.Mask?> CreateResetMaskAccessor();
		protected internal abstract PropertyAccessor<LangInfo?> CreateLanguageAccessor();
		protected internal abstract PropertyAccessor<bool?> CreateNoProofAccessor(); 
	}
	#endregion
	#region NativeCharacterProperties
	public class NativeCharacterProperties : NativeCharacterPropertiesBase, CharacterProperties, IDocumentModelModifier {
		#region Fields
		readonly ModelLogPosition from;
		readonly int length;
		PropertyAccessor<int?> styleIndex;
		NativeSubDocument document;
		#endregion
		internal NativeCharacterProperties(NativeSubDocument document, ModelLogPosition from, int length) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
			this.from = from;
			this.length = length;
			CreateAccessors();
		}
		ModelPieceTable PieceTable { get { return document.PieceTable; } }
		DevExpress.XtraRichEdit.Model.DocumentModel DocumentModel { get { return document.DocumentModel; } }
		#region Style
		public CharacterStyle Style {
			get {
				CheckValid();
				int? index = styleIndex.GetValue();
				if (!index.HasValue)
					return null;
				return ((NativeCharacterStyleCollection)document.MainDocument.CharacterStyles).GetStyle(DocumentModel.CharacterStyles[index.Value]);
			}
			set {
				CheckValid();
				if (value == null)
					return;
				int index = DocumentModel.CharacterStyles.IndexOf(((NativeCharacterStyle)value).InnerStyle);
				if (styleIndex.SetValue(index))
					RaiseChanged();
			}
		}
		#endregion
		#region IDocumentModelModifier Members
		#region Changed
		EventHandler onChanged;
		public event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal override void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		public void ResetCachedValues() {
			CreateAccessors();
		}
		#endregion
		protected internal override void CreateAccessors() {
			base.CreateAccessors();
			this.styleIndex = new CharacterStyleIndexPropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<string> CreateFontNameAccessor() {
			return new FontNamePropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<int?> CreateFontSizeAccessor() {
			return new FontSizePropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<bool?> CreateFontBoldAccessor() {
			return new FontBoldPropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<bool?> CreateFontItalicAccessor() {
			return new FontItalicPropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<ModelUnderlineType?> CreateFontUnderlineAccessor() {
			return new FontUnderlineTypePropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<ModelStrikeoutType?> CreateFontStrikeoutAccessor() {
			return new FontStrikeoutTypePropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<Color?> CreateForeColorAccessor() {
			return new FontForeColorPropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<Color?> CreateUnderlineColorAccessor() {
			return new FontUnderlineColorPropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<Color?> CreateBackColorAccessor() {
			return new FontBackColorPropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<bool?> CreateAllCapsAccessor() {
			return new AllCapsPropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<bool?> CreateHiddenAccessor() {
			return new HiddenPropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<LangInfo?> CreateLanguageAccessor() {
			return new LanguagePropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<ModelCharacterScript?> CreateFontScriptAccessor() {
			return new FontScriptPropertyAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<bool?> CreateResetAccessor() {
			return new ResetUseAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<CharacterFormattingOptions.Mask?> CreateResetMaskAccessor() {
			return new ResetUseMaskAccessor(PieceTable, from, length);
		}
		protected internal override PropertyAccessor<bool?> CreateNoProofAccessor() {
			return new NoProofPropertyAccessor(PieceTable, from, length);
		}
	}
	#endregion
	#region CharactersPropertyAccessor<T> (abstract class)
	public abstract class CharactersPropertyAccessor<T> : SmartPropertyAccessor<T> {
		#region Fields
		readonly ModelPieceTable pieceTable;
		readonly ModelLogPosition from;
		readonly int length;
		#endregion
		protected CharactersPropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.from = from;
			this.length = length;
		}
		protected internal override T CalculateValueCore() {
			if (length <= 0)
				return default(T); 
			DevExpress.XtraRichEdit.Model.RunInfo rinfo = pieceTable.FindRunInfo(from, length);
			if (rinfo.Start.RunIndex >= new ModelRunIndex(0) && rinfo.End.RunIndex >= rinfo.Start.RunIndex) {
				ModelRangePropertyModifierBase modifier = CreateModifier(default(T));
				T result = CalculateValueCore(pieceTable.Runs[rinfo.Start.RunIndex], modifier);
				for (ModelRunIndex i = rinfo.Start.RunIndex + 1; i <= rinfo.End.RunIndex; i++) {
					T value = CalculateValueCore(pieceTable.Runs[i], modifier);
					if (!Compare(value, result))
						return default(T);
				}
				return result;
			}
			else
				return default(T);
		}
		protected internal virtual bool Compare(T value, T result) {
			return value.Equals(result);
		}
		protected internal override bool SetValueCore(T value) {
			if (length <= 0)
				return false;
			ModelRangePropertyModifierBase modifier = CreateModifier(value);
			pieceTable.ApplyCharacterFormatting(from, length, modifier);
			return true;
		}
		protected internal abstract T CalculateValueCore(ModelRun run, ModelRangePropertyModifierBase modifier);
		protected internal abstract ModelRangePropertyModifierBase CreateModifier(T newValue);
	}
	#endregion
	#region CharactersValuePropertyAccessor<T> (abstract class)
	public abstract class CharactersValuePropertyAccessor<T> : CharactersPropertyAccessor<Nullable<T>> where T : struct {
		protected CharactersValuePropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override bool SetValueCore(T? value) {
			if (!value.HasValue)
				return false;
			return base.SetValueCore(value.Value);
		}
		protected internal override T? CalculateValueCore(ModelRun run, ModelRangePropertyModifierBase modifier) {
			DevExpress.XtraRichEdit.Model.RunPropertyModifier<T> typedModifier = (DevExpress.XtraRichEdit.Model.RunPropertyModifier<T>)modifier;
			return typedModifier.GetRunPropertyValue(run);
		}
		protected internal override ModelRangePropertyModifierBase CreateModifier(T? newValue) {
			if (newValue == null)
				newValue = default(T);
			return CreateModifierCore(newValue.Value);
		}
		protected internal abstract DevExpress.XtraRichEdit.Model.RunPropertyModifier<T> CreateModifierCore(T newValue);
	}
	#endregion
	#region FontNamePropertyAccessor
	public class FontNamePropertyAccessor : CharactersPropertyAccessor<string> {
		public FontNamePropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override ModelRangePropertyModifierBase CreateModifier(string newValue) {
			return new DevExpress.XtraRichEdit.Model.RunFontNamePropertyModifier(newValue);
		}
		protected internal override string CalculateValueCore(ModelRun run, ModelRangePropertyModifierBase modifier) {
			DevExpress.XtraRichEdit.Model.RunFontNamePropertyModifier typedModifier = (DevExpress.XtraRichEdit.Model.RunFontNamePropertyModifier)modifier;
			return typedModifier.GetRunPropertyValue(run);
		}
	}
	#endregion
	#region FontSizePropertyAccessor
	public class FontSizePropertyAccessor : CharactersValuePropertyAccessor<int> {
		public FontSizePropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<int> CreateModifierCore(int newValue) {
			return new DevExpress.XtraRichEdit.Model.RunDoubleFontSizePropertyModifier(newValue);
		}
	}
	#endregion
	#region FontBoldPropertyAccessor
	public class FontBoldPropertyAccessor : CharactersValuePropertyAccessor<bool> {
		public FontBoldPropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new DevExpress.XtraRichEdit.Model.RunFontBoldModifier(newValue);
		}
	}
	#endregion
	#region FontItalicPropertyAccessor
	public class FontItalicPropertyAccessor : CharactersValuePropertyAccessor<bool> {
		public FontItalicPropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new DevExpress.XtraRichEdit.Model.RunFontItalicModifier(newValue);
		}
	}
	#endregion
	#region AllCapsPropertyAccessor
	public class AllCapsPropertyAccessor : CharactersValuePropertyAccessor<bool> {
		public AllCapsPropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new DevExpress.XtraRichEdit.Model.RunAllCapsModifier(newValue);
		}
	}
	#endregion
	#region HiddenPropertyAccessor
	public class HiddenPropertyAccessor : CharactersValuePropertyAccessor<bool> {
		public HiddenPropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new DevExpress.XtraRichEdit.Model.RunHiddenModifier(newValue);
		}
	}
	#endregion
	#region FontForeColorPropertyAccessor
	public class FontForeColorPropertyAccessor : CharactersValuePropertyAccessor<Color> {
		public FontForeColorPropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<Color> CreateModifierCore(Color newValue) {
			return new DevExpress.XtraRichEdit.Model.RunForeColorModifier(newValue);
		}
	}
	#endregion
	#region FontUnderlineColorPropertyAccessor
	public class FontUnderlineColorPropertyAccessor : CharactersValuePropertyAccessor<Color> {
		public FontUnderlineColorPropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<Color> CreateModifierCore(Color newValue) {
			return new DevExpress.XtraRichEdit.Model.RunUnderlineColorModifier(newValue);
		}
	}
	#endregion
	#region FontBackColorPropertyAccessor
	public class FontBackColorPropertyAccessor : CharactersValuePropertyAccessor<Color> {
		public FontBackColorPropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<Color> CreateModifierCore(Color newValue) {
			return new DevExpress.XtraRichEdit.Model.RunBackColorModifier(newValue);
		}
	}
	#endregion
	#region FontUnderlineTypePropertyAccessor
	public class FontUnderlineTypePropertyAccessor : CharactersValuePropertyAccessor<ModelUnderlineType> {
		public FontUnderlineTypePropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<ModelUnderlineType> CreateModifierCore(ModelUnderlineType newValue) {
			return new DevExpress.XtraRichEdit.Model.RunFontUnderlineTypeModifier(newValue);
		}
	}
	#endregion
	#region FontStrikeoutTypePropertyAccessor
	public class FontStrikeoutTypePropertyAccessor : CharactersValuePropertyAccessor<ModelStrikeoutType> {
		public FontStrikeoutTypePropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<ModelStrikeoutType> CreateModifierCore(ModelStrikeoutType newValue) {
			return new DevExpress.XtraRichEdit.Model.RunFontStrikeoutTypeModifier(newValue);
		}
	}
	#endregion
	#region FontScriptPropertyAccessor
	public class FontScriptPropertyAccessor : CharactersValuePropertyAccessor<ModelCharacterScript> {
		public FontScriptPropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<ModelCharacterScript> CreateModifierCore(ModelCharacterScript newValue) {
			return new DevExpress.XtraRichEdit.Model.RunScriptModifier(newValue);
		}
	}
	#endregion
	#region CharacterStyleIndexPropertyAccessor
	public class CharacterStyleIndexPropertyAccessor : CharactersValuePropertyAccessor<int> {
		public CharacterStyleIndexPropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<int> CreateModifierCore(int newValue) {
			return new DevExpress.XtraRichEdit.Model.RunCharacterStyleModifier(newValue);
		}
	}
	#endregion
	#region LanguagePropertyAccessor
	public class LanguagePropertyAccessor : CharactersValuePropertyAccessor<LangInfo> {
		public LanguagePropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<LangInfo> CreateModifierCore(LangInfo newValue) {
			return new DevExpress.XtraRichEdit.Model.RunLanguageTypeModifier(newValue);
		}
	}
	#endregion
	#region NoProofPropertyAccessor
	public class NoProofPropertyAccessor : CharactersValuePropertyAccessor<bool> {
		public NoProofPropertyAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new DevExpress.XtraRichEdit.Model.RunNoProofModifier(newValue);
		}
	}
	#endregion
	#region ResetUseAccessor
	public class ResetUseAccessor : CharactersValuePropertyAccessor<bool> {
		public ResetUseAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new DevExpress.XtraRichEdit.Model.RunResetUseModifier(newValue);
		}
	}
	#endregion
	#region ResetUseMaskAccessor
	public class ResetUseMaskAccessor : CharactersValuePropertyAccessor<CharacterFormattingOptions.Mask> {
		public ResetUseMaskAccessor(ModelPieceTable pieceTable, ModelLogPosition from, int length)
			: base(pieceTable, from, length) {
		}
		protected internal override DevExpress.XtraRichEdit.Model.RunPropertyModifier<CharacterFormattingOptions.Mask> CreateModifierCore(CharacterFormattingOptions.Mask newValue) {
			return new DevExpress.XtraRichEdit.Model.RunResetUseMaskModifier(newValue);
		}
	}
	#endregion
}
