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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.API.Native {
	[ComVisible(true)]
	#region TableLookTypes
	[Flags]
	public enum TableLookTypes {
		None = 0x0000,
		ApplyFirstRow = 0x0020,
		ApplyLastRow = 0x0040,
		ApplyFirstColumn = 0x0080,
		ApplyLastColumn = 0x0100,
		DoNotApplyRowBanding = 0x0200,
		DoNotApplyColumnBanding = 0x0400
	}
	#endregion
	[ComVisible(true)]
	#region ConditionalTableStyleFormattingTypes
	[Flags]
	public enum ConditionalTableStyleFormattingTypes {
		WholeTable = 4096,
		FirstRow = 2048, 
		LastRow = 1024, 
		FirstColumn = 512, 
		LastColumn = 256, 
		OddColumnBanding = 128, 
		EvenColumnBanding = 64, 
		OddRowBanding = 32, 
		EvenRowBanding = 16, 
		TopRightCell = 8, 
		TopLeftCell = 4, 
		BottomRightCell = 2, 
		BottomLeftCell = 1, 
	}
	#endregion
	[ComVisible(true)]
	public interface CharacterStyle : CharacterPropertiesBase {
		string Name { get; set; }
		bool IsDeleted { get; }
		CharacterStyle Parent { get; set; }
		ParagraphStyle LinkedStyle { get; set; }
	}
	[ComVisible(true)]
	public interface ParagraphStyle : ParagraphPropertiesWithTabs, CharacterPropertiesBase {
		string Name { get; set; }
		bool IsDeleted { get; }
		ParagraphStyle Parent { get; set; }
		ParagraphStyle NextStyle { get; set; }
		CharacterStyle LinkedStyle { get; set; }
		int NumberingListIndex { get; set; }
		int ListLevelIndex { get; set; }
	}
	[ComVisible(true)]
	public interface TableStyle : TablePropertiesBase, TableCellPropertiesBase, CharacterPropertiesBase, ParagraphPropertiesBase {
		string Name { get; set; }
		bool IsDeleted { get; }
		TableStyle Parent { get; set; }
		TableConditionalStyleProperties ConditionalStyleProperties { get; }
	}
	[ComVisible(true)]
	public interface TableConditionalStyleProperties {
		TableStyle Owner { get; }
		TableConditionalStyle this[ConditionalTableStyleFormattingTypes condition] { get; }
		TableConditionalStyle CreateConditionalStyle(ConditionalTableStyleFormattingTypes condition);
	}
	[ComVisible(true)]
	public interface TableConditionalStyle : TablePropertiesBase, TableCellPropertiesBase, CharacterPropertiesBase, ParagraphPropertiesBase {
	}
	[ComVisible(true)]
	public interface TableCellStyle : TableCellPropertiesBase, CharacterPropertiesBase, ParagraphPropertiesBase {
		string Name { get; set; }
		bool IsDeleted { get; }
		TableCellStyle Parent { get; set; }
	}
	[ComVisible(true)]
	public interface CharacterStyleCollection : ISimpleCollection<CharacterStyle> {
		CharacterStyle this[string name] { get; }
		CharacterStyle CreateNew();
		void Add(CharacterStyle style);
		void Delete(CharacterStyle style);
	}
	[ComVisible(true)]
	public interface ParagraphStyleCollection : ISimpleCollection<ParagraphStyle> {
		ParagraphStyle this[string name] { get; }
		ParagraphStyle CreateNew();
		void Add(ParagraphStyle style);
		void Delete(ParagraphStyle style);
	}
	[ComVisible(true)]
	public interface TableStyleCollection : ISimpleCollection<TableStyle> {
		TableStyle this[string name] { get; }
		TableStyle CreateNew();
		void Add(TableStyle style);
		void Delete(TableStyle style);
	}
	[ComVisible(true)]
	public interface TableCellStyleCollection : ISimpleCollection<TableCellStyle> {
		TableCellStyle this[string name] { get; }
		TableCellStyle CreateNew();
		void Add(TableCellStyle style);
		void Delete(TableCellStyle style);
	}
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ModelCharacterStyle = DevExpress.XtraRichEdit.Model.CharacterStyle;
	using ModelParagraphStyle = DevExpress.XtraRichEdit.Model.ParagraphStyle;
	using ModelTableStyle = DevExpress.XtraRichEdit.Model.TableStyle;
	using ModelTableConditionalStyle = DevExpress.XtraRichEdit.Model.TableConditionalStyle;
	using ModelTableCellStyle = DevExpress.XtraRichEdit.Model.TableCellStyle;
	using ModelCharacterProperties = DevExpress.XtraRichEdit.Model.CharacterProperties;
	using ModelParagraphProperties = DevExpress.XtraRichEdit.Model.ParagraphProperties;
	using ModelCharacterScript = DevExpress.Office.CharacterFormattingScript;
	using ModelUnderlineType = DevExpress.XtraRichEdit.Model.UnderlineType;
	using ModelStrikeoutType = DevExpress.XtraRichEdit.Model.StrikeoutType;
	using ModelCharacterStyleCollection = DevExpress.XtraRichEdit.Model.CharacterStyleCollection;
	using ModelConditionalTableStyleFormattingTypes = DevExpress.XtraRichEdit.Model.ConditionalTableStyleFormattingTypes;
	using DevExpress.Utils;
	using DevExpress.Office;
	using DevExpress.Office.Utils;
	using DevExpress.XtraRichEdit.Model.History;
	using Compatibility.System.Drawing;
	#region NativeCharacterStyle
	public class NativeCharacterStyle : CharacterStyle {
		readonly NativeDocument document;
		readonly ModelCharacterStyle innerStyle;
		readonly CharacterPropertiesBase characterProperties;
		internal NativeCharacterStyle(NativeDocument document, ModelCharacterStyle innerStyle) {
			this.document = document;
			this.innerStyle = innerStyle;
			this.characterProperties = new NativeSimpleCharacterProperties(innerStyle.CharacterProperties);
		}
		NativeDocument Document { get { return document; } }
		protected internal ModelCharacterStyle InnerStyle { get { return innerStyle; } }
		protected internal DevExpress.XtraRichEdit.Model.DocumentModel DocumentModel { get { return innerStyle.DocumentModel; } }
		#region CharacterStyle Members
		public string Name { get { return innerStyle.StyleName; } set { innerStyle.StyleName = value; } }
		public bool IsDeleted { get { return innerStyle.Deleted; } }
		public CharacterStyle Parent {
			get {
				NativeCharacterStyleCollection styles = (NativeCharacterStyleCollection)document.CharacterStyles;
				return styles.GetStyle(innerStyle.Parent);
			}
			set {
				ModelCharacterStyle style = value != null ? ((NativeCharacterStyle)value).InnerStyle : null;
				innerStyle.Parent = style;
			}
		}
		public ParagraphStyle LinkedStyle {
			get {
				NativeParagraphStyleCollection styles = (NativeParagraphStyleCollection)document.ParagraphStyles;
				return styles.GetStyle(innerStyle.LinkedStyle);
			}
			set {
				ModelParagraphStyle style = value != null ? ((NativeParagraphStyle)value).InnerStyle : null;
				if (Object.ReferenceEquals(style, innerStyle.LinkedStyle))
					return;
				if (innerStyle.HasLinkedStyle)
					DocumentModel.StyleLinkManager.DeleteLink(innerStyle);
				if (style != null)
					DocumentModel.StyleLinkManager.CreateLink(style, innerStyle);
			}
		}
		#endregion
		#region CharacterProperties Members
		public string FontName { get { return characterProperties.FontName; } set { characterProperties.FontName = value; } }
		public float? FontSize { get { return characterProperties.FontSize; } set { characterProperties.FontSize = value; } }
		public bool? Bold { get { return characterProperties.Bold; } set { characterProperties.Bold = value; } }
		public bool? Italic { get { return characterProperties.Italic; } set { characterProperties.Italic = value; } }
		public bool? Superscript { get { return characterProperties.Superscript; } set { characterProperties.Superscript = value; } }
		public bool? Subscript { get { return characterProperties.Subscript; } set { characterProperties.Subscript = value; } }
		public bool? AllCaps { get { return characterProperties.AllCaps; } set { characterProperties.AllCaps = value; } }
		public UnderlineType? Underline { get { return characterProperties.Underline; } set { characterProperties.Underline = value; } }
		public StrikeoutType? Strikeout { get { return characterProperties.Strikeout; } set { characterProperties.Strikeout = value; } }
		public Color? UnderlineColor { get { return characterProperties.UnderlineColor; } set { characterProperties.UnderlineColor = value; } }
		public Color? ForeColor { get { return characterProperties.ForeColor; } set { characterProperties.ForeColor = value; } }
		public Color? BackColor { get { return characterProperties.BackColor; } set { characterProperties.BackColor = value; } }
		public bool? Hidden { get { return characterProperties.Hidden; } set { characterProperties.Hidden = value; } }
		public LangInfo? Language { get { return characterProperties.Language; } set { characterProperties.Language = value; } }
		public bool? NoProof { get { return characterProperties.NoProof; } set { characterProperties.NoProof = value; } }
		#endregion
		public virtual void Reset() {
			characterProperties.Reset();
		}
		public virtual void Reset(CharacterPropertiesMask mask) {
			characterProperties.Reset(mask);
		}
	}
	#endregion
	#region NativeReadOnlyCollectionBase
	public abstract class NativeReadOnlyCollectionBase<TItem, TModelItem> : ISimpleCollection<TItem>
		where TModelItem : class
		where TItem : class {
		readonly NativeDocument document;
		readonly Dictionary<TModelItem, TItem> itemTable;
		internal NativeReadOnlyCollectionBase(NativeDocument document) {
			this.document = document;
			this.itemTable = new Dictionary<TModelItem, TItem>();
		}
		protected internal DevExpress.XtraRichEdit.Model.DocumentModel DocumentModel { get { return document.DocumentModel; } }
		protected internal NativeDocument Document { get { return document; } }
		public abstract TItem this[int index] { get; }
		public abstract int Count { get; }
		protected internal TItem GetItem(TModelItem item) {
			if (item == null)
				return null;
			TItem result;
			if (!itemTable.TryGetValue(item, out result)) {
				result = CreateNew(item);
				itemTable.Add(item, result);
			}
			return result;
		}
		internal void Invalidate() {
			itemTable.Clear();
		}
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}
		#endregion
		#region IEnumerable<TStyle> Members
		IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			List<TItem> result = new List<TItem>();
			int count = Count;
			for (int i = 0; i < count; i++)
				result.Add(this[i]);
			Array.Copy(result.ToArray(), 0, array, index, count);
		}
		bool ICollection.IsSynchronized {
			get {
				return false;
			}
		}
		object ICollection.SyncRoot {
			get {
				return this;
			}
		}
		#endregion
		protected internal abstract TItem CreateNew(TModelItem item);
	}
	#endregion
	#region NativeReadOnlyStyleCollectionBase
	public abstract class NativeReadOnlyStyleCollectionBase<TStyle, TModelStyle> : NativeReadOnlyCollectionBase<TStyle, TModelStyle>
		where TModelStyle : StyleBase<TModelStyle>
		where TStyle : class {
		internal NativeReadOnlyStyleCollectionBase(NativeDocument document)
			: base(document) {
		}
		public abstract TStyle this[string name] { get; }
		protected internal TStyle GetStyle(TModelStyle style) {
			return GetItem(style);
		}
	} 
	#endregion
	#region NativeStyleCollectionBase<TStyle, TModelStyle> (abstract class)
	public abstract class NativeStyleCollectionBase<TStyle, TModelStyle> : NativeReadOnlyStyleCollectionBase<TStyle, TModelStyle>
		where TModelStyle : StyleBase<TModelStyle>
		where TStyle : class {
		internal NativeStyleCollectionBase(NativeDocument document)
			: base(document) {
		}
		#region Properties
		public override TStyle this[string name] { get { return GetStyle(InnerStyles.GetStyleByName(name)); } }
		public override TStyle this[int index] { get { return GetStyle(InnerStyles[index]); } }
		public override int Count { get { return InnerStyles.Count; } }
		protected internal abstract StyleCollectionBase<TModelStyle> InnerStyles { get; }
		#endregion
		public TStyle CreateNew() {
			TModelStyle innerStyle = CreateModelStyle();
			return GetStyle(innerStyle);
		}
		public void Add(TStyle style) {
			TModelStyle innerStyle = GetModelStyle(style);
			if (!InnerStyles.Contains(innerStyle)) {
				ValidateStyleProperties(style);
				InnerStyles.Add(innerStyle);
			}
		}
		public virtual void ValidateStyleProperties(TStyle style) {
		}
		public void Delete(TStyle style) {
			TModelStyle innerStyle = GetModelStyle(style);
			InnerStyles.Delete(innerStyle);
		}
		protected internal abstract TModelStyle CreateModelStyle();
		protected internal abstract TModelStyle GetModelStyle(TStyle style);
	}
	#endregion
	#region NativeCharacterStyleCollection
	public class NativeCharacterStyleCollection : NativeStyleCollectionBase<CharacterStyle, ModelCharacterStyle>, CharacterStyleCollection {
		internal NativeCharacterStyleCollection(NativeDocument document)
			: base(document) {
		}
		protected internal override StyleCollectionBase<ModelCharacterStyle> InnerStyles { get { return DocumentModel.CharacterStyles; } }
		protected internal override CharacterStyle CreateNew(ModelCharacterStyle style) {
			return new NativeCharacterStyle(Document, style);
		}
		protected internal override ModelCharacterStyle CreateModelStyle() {
			return new ModelCharacterStyle(DocumentModel);
		}
		protected internal override ModelCharacterStyle GetModelStyle(CharacterStyle style) {
			return ((NativeCharacterStyle)style).InnerStyle;
		}
	}
	#endregion
	#region NativeSimpleCharacterProperties
	public class NativeSimpleCharacterProperties : NativeCharacterPropertiesBase {
		readonly ModelCharacterProperties properties;
		internal NativeSimpleCharacterProperties(ModelCharacterProperties properties) {
			this.properties = properties;
			CreateAccessors();
		}
		protected internal override PropertyAccessor<string> CreateFontNameAccessor() {
			return new FontNamePropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<int?> CreateFontSizeAccessor() {
			return new FontSizePropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<bool?> CreateFontBoldAccessor() {
			return new FontBoldPropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<bool?> CreateFontItalicAccessor() {
			return new FontItalicPropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<ModelUnderlineType?> CreateFontUnderlineAccessor() {
			return new FontUnderlineTypePropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<ModelStrikeoutType?> CreateFontStrikeoutAccessor() {
			return new FontStrikeoutTypePropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<Color?> CreateForeColorAccessor() {
			return new ForeColorPropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<Color?> CreateUnderlineColorAccessor() {
			return new UnderlineColorPropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<Color?> CreateBackColorAccessor() {
			return new BackColorPropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<bool?> CreateAllCapsAccessor() {
			return new AllCapsPropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<bool?> CreateHiddenAccessor() {
			return new HiddenPropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<LangInfo?> CreateLanguageAccessor() {
			return new LanguagePropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<CharacterFormattingScript?> CreateFontScriptAccessor() {
			return new FontScriptPropertyAccessor(properties);
		}
		protected internal override PropertyAccessor<bool?> CreateResetAccessor() {
			return new FontResetUseAccessor(properties);
		}
		protected internal override PropertyAccessor<CharacterFormattingOptions.Mask?> CreateResetMaskAccessor() {
			return new FontResetUseMaskAccessor(properties);
		}
		protected internal override PropertyAccessor<bool?> CreateNoProofAccessor() {
			return new NoProofPropertyAccessor(properties);
		}
		#region Accessor classes
		#region ModelCharacterPropertiesPropertyAccessor<T> (abstract class)
		abstract class ModelCharacterPropertiesPropertyAccessor<T> : PropertyAccessor<T> {
			readonly ModelCharacterProperties properties;
			public ModelCharacterPropertiesPropertyAccessor(ModelCharacterProperties properties) {
				this.properties = properties;
			}
			public ModelCharacterProperties Properties { get { return properties; } }
		}
		#endregion
		#region FontNamePropertyAccessor
		class FontNamePropertyAccessor : ModelCharacterPropertiesPropertyAccessor<string> {
			public FontNamePropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override string GetValue() {
				if (!Properties.UseFontName)
					return null;
				return Properties.FontName;
			}
			public override bool SetValue(string value) {
				if (value == null)
					return false;
				Properties.FontName = value;
				return true;
			}
		}
		#endregion
		#region FontSizePropertyAccessor
		class FontSizePropertyAccessor : ModelCharacterPropertiesPropertyAccessor<int?> {
			public FontSizePropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseDoubleFontSize)
					return null;
				return Properties.DoubleFontSize;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.DoubleFontSize = value.Value;
				return true;
			}
		}
		#endregion
		#region FontBoldPropertyAccessor
		class FontBoldPropertyAccessor : ModelCharacterPropertiesPropertyAccessor<bool?> {
			public FontBoldPropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				if (!Properties.UseFontBold)
					return null;
				return Properties.FontBold;
			}
			public override bool SetValue(bool? value) {
				if (!value.HasValue)
					return false;
				Properties.FontBold = value.Value;
				return true;
			}
		}
		#endregion
		#region FontItalicPropertyAccessor
		class FontItalicPropertyAccessor : ModelCharacterPropertiesPropertyAccessor<bool?> {
			public FontItalicPropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				if (!Properties.UseFontItalic)
					return null;
				return Properties.FontItalic;
			}
			public override bool SetValue(bool? value) {
				if (!value.HasValue)
					return false;
				Properties.FontItalic = value.Value;
				return true;
			}
		}
		#endregion
		#region FontUnderlineTypePropertyAccessor
		class FontUnderlineTypePropertyAccessor : ModelCharacterPropertiesPropertyAccessor<ModelUnderlineType?> {
			public FontUnderlineTypePropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override ModelUnderlineType? GetValue() {
				if (!Properties.UseFontUnderlineType)
					return null;
				return Properties.FontUnderlineType;
			}
			public override bool SetValue(ModelUnderlineType? value) {
				if (!value.HasValue)
					return false;
				Properties.FontUnderlineType = value.Value;
				return true;
			}
		}
		#endregion
		#region FontStrikeoutTypePropertyAccessor
		class FontStrikeoutTypePropertyAccessor : ModelCharacterPropertiesPropertyAccessor<ModelStrikeoutType?> {
			public FontStrikeoutTypePropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override ModelStrikeoutType? GetValue() {
				if (!Properties.UseFontStrikeoutType)
					return null;
				return Properties.FontStrikeoutType;
			}
			public override bool SetValue(ModelStrikeoutType? value) {
				if (!value.HasValue)
					return false;
				Properties.FontStrikeoutType = value.Value;
				return true;
			}
		}
		#endregion
		#region ForeColorPropertyAccessor
		class ForeColorPropertyAccessor : ModelCharacterPropertiesPropertyAccessor<Color?> {
			public ForeColorPropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override Color? GetValue() {
				if (!Properties.UseForeColor)
					return null;
				return Properties.ForeColor;
			}
			public override bool SetValue(Color? value) {
				if (!value.HasValue)
					return false;
				Properties.ForeColor = value.Value;
				return true;
			}
		}
		#endregion
		#region UnderlineColorPropertyAccessor
		class UnderlineColorPropertyAccessor : ModelCharacterPropertiesPropertyAccessor<Color?> {
			public UnderlineColorPropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override Color? GetValue() {
				if (!Properties.UseUnderlineColor)
					return null;
				return Properties.UnderlineColor;
			}
			public override bool SetValue(Color? value) {
				if (!value.HasValue)
					return false;
				Properties.UnderlineColor = value.Value;
				return true;
			}
		}
		#endregion
		#region BackColorPropertyAccessor
		class BackColorPropertyAccessor : ModelCharacterPropertiesPropertyAccessor<Color?> {
			public BackColorPropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override Color? GetValue() {
				if (!Properties.UseBackColor)
					return null;
				return Properties.BackColor;
			}
			public override bool SetValue(Color? value) {
				if (!value.HasValue)
					return false;
				Properties.BackColor = value.Value;
				return true;
			}
		}
		#endregion
		#region AllCapsPropertyAccessor
		class AllCapsPropertyAccessor : ModelCharacterPropertiesPropertyAccessor<bool?> {
			public AllCapsPropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				if (!Properties.UseAllCaps)
					return null;
				return Properties.AllCaps;
			}
			public override bool SetValue(bool? value) {
				if (!value.HasValue)
					return false;
				Properties.AllCaps = value.Value;
				return true;
			}
		}
		#endregion
		#region HiddenPropertyAccessor
		class HiddenPropertyAccessor : ModelCharacterPropertiesPropertyAccessor<bool?> {
			public HiddenPropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				if (!Properties.UseHidden)
					return null;
				return Properties.Hidden;
			}
			public override bool SetValue(bool? value) {
				if (!value.HasValue)
					return false;
				Properties.Hidden = value.Value;
				return true;
			}
		}
		#endregion
		#region LanguagePropertyAccessor
		class LanguagePropertyAccessor : ModelCharacterPropertiesPropertyAccessor<LangInfo?> {
			public LanguagePropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override LangInfo? GetValue() {
				if (!Properties.UseLangInfo)
					return null;
				return Properties.LangInfo;
			}
			public override bool SetValue(LangInfo? value) {
				if (!value.HasValue)
					return false;
				Properties.LangInfo = value.Value;
				return true;
			}
		}
		#endregion
		#region NoProofPropertyAccessor
		class NoProofPropertyAccessor : ModelCharacterPropertiesPropertyAccessor<bool?> {
			public NoProofPropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				if (!Properties.UseNoProof)
					return null;
				return Properties.NoProof;
			}
			public override bool SetValue(bool? value) {
				if (!value.HasValue)
					return false;
				Properties.NoProof = value.Value;
				return true;
			}
		}
		#endregion
		#region FontScriptPropertyAccessor
		class FontScriptPropertyAccessor : ModelCharacterPropertiesPropertyAccessor<ModelCharacterScript?> {
			public FontScriptPropertyAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override ModelCharacterScript? GetValue() {
				if (!Properties.UseScript)
					return null;
				return Properties.Script;
			}
			public override bool SetValue(ModelCharacterScript? value) {
				if (!value.HasValue)
					return false;
				Properties.Script = value.Value;
				return true;
			}
		}
		#endregion
		#region FontResetUseAccessor
		class FontResetUseAccessor : ModelCharacterPropertiesPropertyAccessor<bool?> {
			public FontResetUseAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				return Properties.Info.Options.Value == CharacterFormattingOptions.Mask.UseNone;
			}
			public override bool SetValue(bool? value) {
				Properties.ResetAllUse();
				return true;
			}
		}
		#endregion
		#region FontResetUseAccessor
		class FontResetUseMaskAccessor : ModelCharacterPropertiesPropertyAccessor<CharacterFormattingOptions.Mask?> {
			public FontResetUseMaskAccessor(ModelCharacterProperties properties)
				: base(properties) {
			}
			public override CharacterFormattingOptions.Mask? GetValue() {
				return Properties.Info.Options.Value;
			}
			public override bool SetValue(CharacterFormattingOptions.Mask? value) {
				if (!value.HasValue)
					return false;
				Properties.ResetUse(value.Value);
				return true;
			}
		}
		#endregion
		#endregion
		protected internal override void RaiseChanged() {
		}
	}
	#endregion
	#region NativeDefaultCharacterProperties
	public class NativeDefaultCharacterProperties : NativeSimpleCharacterProperties {
		public NativeDefaultCharacterProperties(ModelCharacterProperties properties)
			: base(properties) {
		}
		public override void Reset() {
			RichEditExceptions.ThrowInvalidOperationException(DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.Msg_CantResetDefaultProperties);
		}
	}
	#endregion
	#region NativeParagraphStyle
	public class NativeParagraphStyle : ParagraphStyle {
		#region Fields
		readonly NativeDocument document;
		readonly ModelParagraphStyle innerStyle;
		readonly ParagraphPropertiesWithTabs paragraphProperties;
		readonly CharacterPropertiesBase characterProperties;
		#endregion
		internal NativeParagraphStyle(NativeDocument document, ModelParagraphStyle innerStyle) {
			this.document = document;
			this.innerStyle = innerStyle;
			this.paragraphProperties = new NativeStyleParagraphProperties(document, innerStyle.Tabs, innerStyle.ParagraphProperties);
			this.characterProperties = new NativeSimpleCharacterProperties(innerStyle.CharacterProperties);
		}
		#region Properties
		NativeDocument Document { get { return document; } }
		public ModelParagraphStyle InnerStyle { get { return innerStyle; } }
		protected internal DevExpress.XtraRichEdit.Model.DocumentModel DocumentModel { get { return innerStyle.DocumentModel; } }
		#endregion
		#region ParagraphStyle Members
		public string Name { get { return innerStyle.StyleName; } set { innerStyle.StyleName = value; } }
		public bool IsDeleted { get { return innerStyle.Deleted; } }
		public ParagraphStyle Parent {
			get {
				NativeParagraphStyleCollection styles = (NativeParagraphStyleCollection)document.ParagraphStyles;
				return styles.GetStyle(innerStyle.Parent);
			}
			set {
				ModelParagraphStyle style = value != null ? ((NativeParagraphStyle)value).InnerStyle : null;
				innerStyle.Parent = style;
			}
		}
		public ParagraphStyle NextStyle {
			get {
				NativeParagraphStyleCollection styles = (NativeParagraphStyleCollection)document.ParagraphStyles;
				return styles.GetStyle(innerStyle.NextParagraphStyle);
			}
			set {
				ModelParagraphStyle style = value != null ? ((NativeParagraphStyle)value).InnerStyle : null;
				innerStyle.NextParagraphStyle = style;
			}
		}
		public CharacterStyle LinkedStyle {
			get {
				NativeCharacterStyleCollection styles = (NativeCharacterStyleCollection)document.CharacterStyles;
				return styles.GetStyle(innerStyle.LinkedStyle);
			}
			set {
				ModelCharacterStyle style = value != null ? ((NativeCharacterStyle)value).InnerStyle : null;
				if (Object.ReferenceEquals(style, innerStyle.LinkedStyle))
					return;
				if (innerStyle.HasLinkedStyle)
					DocumentModel.StyleLinkManager.DeleteLink(innerStyle);
				if (style != null)
					DocumentModel.StyleLinkManager.CreateLink(innerStyle, style);
			}
		}
		public int NumberingListIndex {
			get {
				return ((IConvertToInt<Model.NumberingListIndex>)InnerStyle.GetNumberingListIndex()).ToInt();
			}
			set {
				DevExpress.XtraRichEdit.Model.ParagraphStyle style = innerStyle;
				if (!innerStyle.Deleted && DocumentModel.ParagraphStyles.Contains(innerStyle))
					ValidateNumberingListIndex(value);
				innerStyle.DocumentModel.BeginUpdate();
				try {
					style.SetNumberingListIndex(new Model.NumberingListIndex(value));
				}
				finally {
					innerStyle.DocumentModel.EndUpdate();
				}
			}
		}
		public int ListLevelIndex {
			get {
				return InnerStyle.GetOwnListLevelIndex();
			}
			set {
				DevExpress.XtraRichEdit.Model.ParagraphStyle style = innerStyle;
				innerStyle.DocumentModel.BeginUpdate();
				try {
					style.SetNumberingListLevelIndex(value);
				}
				finally {
					innerStyle.DocumentModel.EndUpdate();
				}
			}
		}
		#endregion
		#region CharacterProperties Members
		public string FontName { get { return characterProperties.FontName; } set { characterProperties.FontName = value; } }
		public float? FontSize { get { return characterProperties.FontSize; } set { characterProperties.FontSize = value; } }
		public bool? Bold { get { return characterProperties.Bold; } set { characterProperties.Bold = value; } }
		public bool? Italic { get { return characterProperties.Italic; } set { characterProperties.Italic = value; } }
		public bool? Superscript { get { return characterProperties.Superscript; } set { characterProperties.Superscript = value; } }
		public bool? Subscript { get { return characterProperties.Subscript; } set { characterProperties.Subscript = value; } }
		public bool? AllCaps { get { return characterProperties.AllCaps; } set { characterProperties.AllCaps = value; } }
		public UnderlineType? Underline { get { return characterProperties.Underline; } set { characterProperties.Underline = value; } }
		public StrikeoutType? Strikeout { get { return characterProperties.Strikeout; } set { characterProperties.Strikeout = value; } }
		public Color? UnderlineColor { get { return characterProperties.UnderlineColor; } set { characterProperties.UnderlineColor = value; } }
		public Color? ForeColor { get { return characterProperties.ForeColor; } set { characterProperties.ForeColor = value; } }
		public Color? BackColor { get { return characterProperties.BackColor; } set { characterProperties.BackColor = value; } }
		public bool? Hidden { get { return characterProperties.Hidden; } set { characterProperties.Hidden = value; } }
		public LangInfo? Language { get { return characterProperties.Language; } set { characterProperties.Language = value; } }
		public bool? NoProof { get { return characterProperties.NoProof; } set { characterProperties.NoProof = value; } }
		void CharacterPropertiesBase.Reset() {
			characterProperties.Reset();
		}
		public void Reset(CharacterPropertiesMask mask) {
			characterProperties.Reset(mask);
		}
		#endregion
		#region ParagraphProperties Members
		public ParagraphAlignment? Alignment { get { return paragraphProperties.Alignment; } set { paragraphProperties.Alignment = value; } }
		public float? LeftIndent { get { return paragraphProperties.LeftIndent; } set { paragraphProperties.LeftIndent = value; } }
		public float? RightIndent { get { return paragraphProperties.RightIndent; } set { paragraphProperties.RightIndent = value; } }
		public float? SpacingBefore { get { return paragraphProperties.SpacingBefore; } set { paragraphProperties.SpacingBefore = value; } }
		public float? SpacingAfter { get { return paragraphProperties.SpacingAfter; } set { paragraphProperties.SpacingAfter = value; } }
		public ParagraphLineSpacing? LineSpacingType { get { return paragraphProperties.LineSpacingType; } set { paragraphProperties.LineSpacingType = value; } }
		public float? LineSpacing { get { return paragraphProperties.LineSpacing; } set { paragraphProperties.LineSpacing = value; } }
		public float? LineSpacingMultiplier { get { return paragraphProperties.LineSpacingMultiplier; } set { paragraphProperties.LineSpacingMultiplier = value; } }
		public ParagraphFirstLineIndent? FirstLineIndentType { get { return paragraphProperties.FirstLineIndentType; } set { paragraphProperties.FirstLineIndentType = value; } }
		public float? FirstLineIndent { get { return paragraphProperties.FirstLineIndent; } set { paragraphProperties.FirstLineIndent = value; } }
		public bool? SuppressHyphenation { get { return paragraphProperties.SuppressHyphenation; } set { paragraphProperties.SuppressHyphenation = value; } }
		public bool? SuppressLineNumbers { get { return paragraphProperties.SuppressLineNumbers; } set { paragraphProperties.SuppressLineNumbers = value; } }
		public int? OutlineLevel { get { return paragraphProperties.OutlineLevel; } set { paragraphProperties.OutlineLevel = value; } }
		public bool? KeepLinesTogether { get { return paragraphProperties.KeepLinesTogether; } set { paragraphProperties.KeepLinesTogether = value; } }
		public bool? PageBreakBefore { get { return paragraphProperties.PageBreakBefore; } set { paragraphProperties.PageBreakBefore = value; } }
		Color? ParagraphPropertiesBase.BackColor { get { return paragraphProperties.BackColor; } set { paragraphProperties.BackColor = value; } }
		public bool? ContextualSpacing { get { return paragraphProperties.ContextualSpacing; } set { paragraphProperties.ContextualSpacing = value; } }
		public TabInfoCollection BeginUpdateTabs(bool onlyOwnTabs) {
			return paragraphProperties.BeginUpdateTabs(onlyOwnTabs);
		}
		public void EndUpdateTabs(TabInfoCollection tabs) {
			paragraphProperties.EndUpdateTabs(tabs);
		}
		void ParagraphPropertiesBase.Reset() {
			paragraphProperties.Reset();
		}
		public void Reset(ParagraphPropertiesMask mask) {
			paragraphProperties.Reset(mask);
		}
		#endregion
		public void ValidateStyleProperties() {
			ValidateNumberingListIndex(NumberingListIndex);
		}
		void ValidateNumberingListIndex(int value) {
			int numberingListsCount = DocumentModel.NumberingLists.Count;
			if (value >= numberingListsCount)
				RichEditExceptions.ThrowInvalidOperationException(Localization.XtraRichEditStringId.Msg_InvalidNumberingListIndex);
		}
	}
	#endregion
	#region NativeParagraphStyleCollection
	public class NativeParagraphStyleCollection : NativeStyleCollectionBase<ParagraphStyle, ModelParagraphStyle>, ParagraphStyleCollection {
		internal NativeParagraphStyleCollection(NativeDocument document)
			: base(document) {
		}
		protected internal override StyleCollectionBase<ModelParagraphStyle> InnerStyles { get { return DocumentModel.ParagraphStyles; } }
		protected internal override ParagraphStyle CreateNew(ModelParagraphStyle style) {
			return new NativeParagraphStyle(Document, style);
		}
		protected internal override ModelParagraphStyle CreateModelStyle() {
			return new ModelParagraphStyle(DocumentModel);
		}
		protected internal override ModelParagraphStyle GetModelStyle(ParagraphStyle style) {
			return ((NativeParagraphStyle)style).InnerStyle;
		}
		public override void ValidateStyleProperties(ParagraphStyle style) {
			base.ValidateStyleProperties(style);
			NativeParagraphStyle nativeStyle = (NativeParagraphStyle)style;
			nativeStyle.ValidateStyleProperties();
		}
	}
	#endregion
	#region NativeSimpleParagraphPropertiesBase
	public abstract class NativeSimpleParagraphPropertiesBase : NativeParagraphPropertiesBase {
		internal NativeSimpleParagraphPropertiesBase(NativeSubDocument document)
			: base(document) {
		}
		protected internal abstract ModelParagraphProperties ParagraphProperties { get; }
		protected internal abstract TabProperties Tabs { get; }
		protected internal override PropertyAccessor<ParagraphAlignment?> CreateAlignmentAccessor() {
			return new AlignmentPropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<int?> CreateLeftIndentAccessor() {
			return new LeftIndentPropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<int?> CreateRightIndentAccessor() {
			return new RightIndentPropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<int?> CreateSpacingBeforeAccessor() {
			return new SpacingBeforePropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<int?> CreateSpacingAfterAccessor() {
			return new SpacingAfterPropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<ParagraphLineSpacing?> CreateLineSpacingTypeAccessor() {
			return new LineSpacingTypePropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<float?> CreateLineSpacingAccessor() {
			return new LineSpacingPropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<ParagraphFirstLineIndent?> CreateFirstLineIndentTypeAccessor() {
			return new FirstLineIndentTypePropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<int?> CreateFirstLineIndentAccessor() {
			return new FirstLineIndentPropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<bool?> CreateSuppressHyphenationAccessor() {
			return new SuppressHyphenationPropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<bool?> CreateSuppressLineNumbersAccessor() {
			return new SuppressLineNumbersPropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<int?> CreateOutlineLevelAccessor() {
			return new OutlineLevelPropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<bool?> CreateKeepLinesTogetherAccessor() {
			return new KeepLinesTogetherPropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<bool?> CreatePageBreakBeforeAccessor() {
			return new PageBreakBeforePropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<Color?> CreateBackColorAccessor() {
			return new BackColorPropertyAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<TabFormattingInfo> CreateTabsAccessor(bool onlyOwnTabs) {
			return new TabsPropertyAccessor(Tabs);
		}
		protected internal override PropertyAccessor<bool?> CreateResetAccessor() {
			return new ResetUseAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<ParagraphFormattingOptions.Mask?> CreateResetMaskAccessor() {
			return new ResetUseMaskAccessor(ParagraphProperties);
		}
		protected internal override PropertyAccessor<bool?> CreateContextualSpacingAccessor() {
			return new ContextualSpacingPropertyAccessor(ParagraphProperties);
		}
		#region Accessor classes
		#region ModelParagraphPropertiesPropertyAccessor<T> (abstract class)
		abstract class ModelParagraphPropertiesPropertyAccessor<T> : PropertyAccessor<T> {
			readonly ModelParagraphProperties properties;
			public ModelParagraphPropertiesPropertyAccessor(ModelParagraphProperties properties) {
				this.properties = properties;
			}
			public ModelParagraphProperties Properties { get { return properties; } }
		}
		#endregion
		#region AlignmentPropertyAccessor
		class AlignmentPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<ParagraphAlignment?> {
			public AlignmentPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override ParagraphAlignment? GetValue() {
				if (!Properties.UseAlignment)
					return null;
				return (ParagraphAlignment)Properties.Alignment;
			}
			public override bool SetValue(ParagraphAlignment? value) {
				if (!value.HasValue)
					return false;
				Properties.Alignment = (DevExpress.XtraRichEdit.Model.ParagraphAlignment)value.Value;
				return true;
			}
		}
		#endregion
		#region LeftIndentPropertyAccessor
		class LeftIndentPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<int?> {
			public LeftIndentPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseLeftIndent)
					return null;
				return Properties.LeftIndent;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.LeftIndent = value.Value;
				return true;
			}
		}
		#endregion
		#region RightIndentPropertyAccessor
		class RightIndentPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<int?> {
			public RightIndentPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseRightIndent)
					return null;
				return Properties.RightIndent;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.RightIndent = value.Value;
				return true;
			}
		}
		#endregion
		#region SpacingBeforePropertyAccessor
		class SpacingBeforePropertyAccessor : ModelParagraphPropertiesPropertyAccessor<int?> {
			public SpacingBeforePropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseSpacingBefore)
					return null;
				return Properties.SpacingBefore;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.SpacingBefore = value.Value;
				return true;
			}
		}
		#endregion
		#region SpacingAfterPropertyAccessor
		class SpacingAfterPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<int?> {
			public SpacingAfterPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseSpacingAfter)
					return null;
				return Properties.SpacingAfter;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.SpacingAfter = value.Value;
				return true;
			}
		}
		#endregion
		#region LineSpacingTypePropertyAccessor
		class LineSpacingTypePropertyAccessor : ModelParagraphPropertiesPropertyAccessor<ParagraphLineSpacing?> {
			public LineSpacingTypePropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override ParagraphLineSpacing? GetValue() {
				if (!Properties.UseLineSpacingType)
					return null;
				return (ParagraphLineSpacing)Properties.LineSpacingType;
			}
			public override bool SetValue(ParagraphLineSpacing? value) {
				if (!value.HasValue)
					return false;
				Properties.LineSpacingType = (DevExpress.XtraRichEdit.Model.ParagraphLineSpacing)value.Value;
				return true;
			}
		}
		#endregion
		#region LineSpacingPropertyAccessor
		class LineSpacingPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<float?> {
			public LineSpacingPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override float? GetValue() {
				if (!Properties.UseLineSpacing)
					return null;
				return Properties.LineSpacing;
			}
			public override bool SetValue(float? value) {
				if (!value.HasValue)
					return false;
				Properties.LineSpacing = value.Value;
				return true;
			}
		}
		#endregion
		#region FirstLineIndentTypePropertyAccessor
		class FirstLineIndentTypePropertyAccessor : ModelParagraphPropertiesPropertyAccessor<ParagraphFirstLineIndent?> {
			public FirstLineIndentTypePropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override ParagraphFirstLineIndent? GetValue() {
				if (!Properties.UseFirstLineIndentType)
					return null;
				return (ParagraphFirstLineIndent)Properties.FirstLineIndentType;
			}
			public override bool SetValue(ParagraphFirstLineIndent? value) {
				if (!value.HasValue)
					return false;
				Properties.FirstLineIndentType = (DevExpress.XtraRichEdit.Model.ParagraphFirstLineIndent)value.Value;
				return true;
			}
		}
		#endregion
		#region FirstLineIndentPropertyAccessor
		class FirstLineIndentPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<int?> {
			public FirstLineIndentPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseFirstLineIndent)
					return null;
				return Properties.FirstLineIndent;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.FirstLineIndent = value.Value;
				return true;
			}
		}
		#endregion
		#region SuppressHyphenationPropertyAccessor
		class SuppressHyphenationPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<bool?> {
			public SuppressHyphenationPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				if (!Properties.UseSuppressHyphenation)
					return null;
				return Properties.SuppressHyphenation;
			}
			public override bool SetValue(bool? value) {
				if (!value.HasValue)
					return false;
				Properties.SuppressHyphenation = value.Value;
				return true;
			}
		}
		#endregion
		#region SuppressLineNumbersPropertyAccessor
		class SuppressLineNumbersPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<bool?> {
			public SuppressLineNumbersPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				if (!Properties.UseSuppressLineNumbers)
					return null;
				return Properties.SuppressLineNumbers;
			}
			public override bool SetValue(bool? value) {
				if (!value.HasValue)
					return false;
				Properties.SuppressLineNumbers = value.Value;
				return true;
			}
		}
		#endregion
		#region OutlineLevelPropertyAccessor
		class OutlineLevelPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<int?> {
			public OutlineLevelPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseOutlineLevel)
					return null;
				return Properties.OutlineLevel;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.OutlineLevel = value.Value;
				return true;
			}
		}
		#endregion
		#region KeepLinesTogetherPropertyAccessor
		class KeepLinesTogetherPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<bool?> {
			public KeepLinesTogetherPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				if (!Properties.UseKeepLinesTogether)
					return null;
				return Properties.KeepLinesTogether;
			}
			public override bool SetValue(bool? value) {
				if (!value.HasValue)
					return false;
				Properties.KeepLinesTogether = value.Value;
				return true;
			}
		}
		#endregion
		#region PageBreakBeforePropertyAccessor
		class PageBreakBeforePropertyAccessor : ModelParagraphPropertiesPropertyAccessor<bool?> {
			public PageBreakBeforePropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				if (!Properties.UsePageBreakBefore)
					return null;
				return Properties.PageBreakBefore;
			}
			public override bool SetValue(bool? value) {
				if (!value.HasValue)
					return false;
				Properties.PageBreakBefore = value.Value;
				return true;
			}
		}
		#endregion
		#region BackColorPropertyAccessor
		class BackColorPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<Color?> {
			public BackColorPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override Color? GetValue() {
				if (!Properties.UseBackColor)
					return null;
				return Properties.BackColor;
			}
			public override bool SetValue(Color? value) {
				if (!value.HasValue)
					return false;
				Properties.BackColor = value.Value;
				return true;
			}
		}
		#endregion
		#region ContextualSpacingPropertyAccessor
		class ContextualSpacingPropertyAccessor : ModelParagraphPropertiesPropertyAccessor<bool?> {
			public ContextualSpacingPropertyAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				if (!Properties.UseContextualSpacing)
					return null;
				return Properties.ContextualSpacing;
			}
			public override bool SetValue(bool? value) {
				if (!value.HasValue)
					return false;
				Properties.ContextualSpacing = value.Value;
				return true;
			}
		}
		#endregion
		#region TabsPropertyAccessor
		class TabsPropertyAccessor : PropertyAccessor<TabFormattingInfo> {
			readonly TabProperties properties;
			public TabsPropertyAccessor(TabProperties properties) {
				this.properties = properties;
			}
			public TabProperties Properties { get { return properties; } }
			public override TabFormattingInfo GetValue() {
				return properties.GetTabs();
			}
			public override bool SetValue(TabFormattingInfo value) {
				if (value == null)
					return false;
				properties.SetTabs(value);
				return true;
			}
		}
		#endregion
		#region ResetUseAccessor
		class ResetUseAccessor : ModelParagraphPropertiesPropertyAccessor<bool?> {
			public ResetUseAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				return Properties.Info.Options.Value == ParagraphFormattingOptions.Mask.UseNone;
			}
			public override bool SetValue(bool? value) {
				Properties.ResetAllUse();
				return true;
			}
		}
		#endregion
		#region ResetUseMaskAccessor
		class ResetUseMaskAccessor : ModelParagraphPropertiesPropertyAccessor<ParagraphFormattingOptions.Mask?> {
			public ResetUseMaskAccessor(ModelParagraphProperties properties)
				: base(properties) {
			}
			public override ParagraphFormattingOptions.Mask? GetValue() {
				return Properties.Info.Options.Value;
			}
			public override bool SetValue(ParagraphFormattingOptions.Mask? value) {
				if (!value.HasValue)
					return false;
				Properties.ResetUse(value.Value);
				return true;
			}
		}
		#endregion
		#endregion
		protected internal override void RaiseChanged() {
		}
	}
	#endregion
	#region NativeDefaultParagraphProperties
	public class NativeDefaultParagraphProperties : NativeSimpleParagraphPropertiesBase {
		readonly ModelParagraphProperties properties;
		internal NativeDefaultParagraphProperties(NativeSubDocument document, ModelParagraphProperties properties)
			: base(document) {
			this.properties = properties;
			CreateAccessors();
		}
		protected internal override ModelParagraphProperties ParagraphProperties { get { return properties; } }
		protected internal override TabProperties Tabs { get { RichEditExceptions.ThrowInvalidOperationException(DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.Msg_NoDefaultTabs); return null; } }
		public override void Reset() {
			RichEditExceptions.ThrowInvalidOperationException(DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.Msg_CantResetDefaultProperties);
		}
	}
	#endregion
	#region NativeStyleParagraphProperties
	public class NativeStyleParagraphProperties : NativeSimpleParagraphPropertiesBase {
		readonly ModelParagraphProperties modelParagraphProperties;
		readonly Model.TabProperties tabs;
		internal NativeStyleParagraphProperties(NativeDocument document, Model.TabProperties tabs, ModelParagraphProperties paragraphProperties)
			: base(document) {
			this.tabs = tabs;
			this.modelParagraphProperties = paragraphProperties;
			CreateAccessors();
		}
		protected internal override ModelParagraphProperties ParagraphProperties { get { return modelParagraphProperties; } }
		protected internal override TabProperties Tabs { get { return tabs; } }
	}
	#endregion
	public class NativeTableStyle : TableStyle {
		readonly NativeDocument document;
		readonly ModelTableStyle innerStyle;
		readonly TablePropertiesBase tableProperties;
		readonly TableCellPropertiesBase tableCellProperties;
		readonly ParagraphPropertiesWithTabs paragraphProperties;
		readonly CharacterPropertiesBase characterProperties;
		TableConditionalStyleProperties conditionalStyleProperties;
		internal NativeTableStyle(NativeDocument document, ModelTableStyle innerStyle) {
			this.document = document;
			this.innerStyle = innerStyle;
			this.tableProperties = new NativeStyleTableProperties(document, innerStyle.TableProperties);
			this.tableCellProperties = new NativeStyleTableCellProperties(document, innerStyle.TableCellProperties);
			this.paragraphProperties = new NativeStyleParagraphProperties(document, innerStyle.Tabs, innerStyle.ParagraphProperties);
			this.characterProperties = new NativeSimpleCharacterProperties(innerStyle.CharacterProperties);
			this.conditionalStyleProperties = new NativeTableConditionalStyleProperties(this);
		}
		#region Properties
		internal NativeDocument Document { get { return document; } }
		public ModelTableStyle InnerStyle { get { return innerStyle; } }
		protected internal DevExpress.XtraRichEdit.Model.DocumentModel DocumentModel { get { return innerStyle.DocumentModel; } }
		#endregion
		public TablePropertiesBase TableProperties { get { return tableProperties; } }
		public TableCellPropertiesBase TableCellProperties { get { return tableCellProperties; } }
		public ParagraphPropertiesWithTabs ParagraphProperties { get { return paragraphProperties; } }
		public CharacterPropertiesBase CharacterProperties { get { return characterProperties; } }
		#region TableStyle Members
		public string Name { get { return innerStyle.StyleName; } set { innerStyle.StyleName = value; } }
		public bool IsDeleted { get { return innerStyle.Deleted; } }
		public TableStyle Parent {
			get {
				NativeTableStyleCollection styles = (NativeTableStyleCollection)document.TableStyles;
				return styles.GetStyle(innerStyle.Parent);
			}
			set {
				ModelTableStyle style = value != null ? ((NativeTableStyle)value).InnerStyle : null;
				innerStyle.Parent = style;
			}
		}
		public TableConditionalStyleProperties ConditionalStyleProperties {
			get {
				if (conditionalStyleProperties == null)
					conditionalStyleProperties = new NativeTableConditionalStyleProperties(this);
				return conditionalStyleProperties;
			}
		}
		#endregion
		#region CharacterProperties Members
		public string FontName { get { return characterProperties.FontName; } set { characterProperties.FontName = value; } }
		public float? FontSize { get { return characterProperties.FontSize; } set { characterProperties.FontSize = value; } }
		public bool? Bold { get { return characterProperties.Bold; } set { characterProperties.Bold = value; } }
		public bool? Italic { get { return characterProperties.Italic; } set { characterProperties.Italic = value; } }
		public bool? Superscript { get { return characterProperties.Superscript; } set { characterProperties.Superscript = value; } }
		public bool? Subscript { get { return characterProperties.Subscript; } set { characterProperties.Subscript = value; } }
		public bool? AllCaps { get { return characterProperties.AllCaps; } set { characterProperties.AllCaps = value; } }
		public UnderlineType? Underline { get { return characterProperties.Underline; } set { characterProperties.Underline = value; } }
		public StrikeoutType? Strikeout { get { return characterProperties.Strikeout; } set { characterProperties.Strikeout = value; } }
		public Color? UnderlineColor { get { return characterProperties.UnderlineColor; } set { characterProperties.UnderlineColor = value; } }
		public Color? ForeColor { get { return characterProperties.ForeColor; } set { characterProperties.ForeColor = value; } }
		Color? CharacterPropertiesBase.BackColor { get { return characterProperties.BackColor; } set { characterProperties.BackColor = value; } }
		public bool? Hidden { get { return characterProperties.Hidden; } set { characterProperties.Hidden = value; } }
		public LangInfo? Language { get { return characterProperties.Language; } set { characterProperties.Language = value; } }
		public bool? NoProof { get { return characterProperties.NoProof; } set { characterProperties.NoProof = value; } }
		void CharacterPropertiesBase.Reset() {
			characterProperties.Reset();
		}
		public void Reset(CharacterPropertiesMask mask) {
			characterProperties.Reset(mask);
		}
		#endregion
		#region ParagraphProperties Members
		public ParagraphAlignment? Alignment { get { return paragraphProperties.Alignment; } set { paragraphProperties.Alignment = value; } }
		public float? LeftIndent { get { return paragraphProperties.LeftIndent; } set { paragraphProperties.LeftIndent = value; } }
		public float? RightIndent { get { return paragraphProperties.RightIndent; } set { paragraphProperties.RightIndent = value; } }
		public float? SpacingBefore { get { return paragraphProperties.SpacingBefore; } set { paragraphProperties.SpacingBefore = value; } }
		public float? SpacingAfter { get { return paragraphProperties.SpacingAfter; } set { paragraphProperties.SpacingAfter = value; } }
		public ParagraphLineSpacing? LineSpacingType { get { return paragraphProperties.LineSpacingType; } set { paragraphProperties.LineSpacingType = value; } }
		public float? LineSpacing { get { return paragraphProperties.LineSpacing; } set { paragraphProperties.LineSpacing = value; } }
		public float? LineSpacingMultiplier { get { return paragraphProperties.LineSpacingMultiplier; } set { paragraphProperties.LineSpacingMultiplier = value; } }
		public ParagraphFirstLineIndent? FirstLineIndentType { get { return paragraphProperties.FirstLineIndentType; } set { paragraphProperties.FirstLineIndentType = value; } }
		public float? FirstLineIndent { get { return paragraphProperties.FirstLineIndent; } set { paragraphProperties.FirstLineIndent = value; } }
		public bool? SuppressHyphenation { get { return paragraphProperties.SuppressHyphenation; } set { paragraphProperties.SuppressHyphenation = value; } }
		public bool? SuppressLineNumbers { get { return paragraphProperties.SuppressLineNumbers; } set { paragraphProperties.SuppressLineNumbers = value; } }
		public int? OutlineLevel { get { return paragraphProperties.OutlineLevel; } set { paragraphProperties.OutlineLevel = value; } }
		public bool? KeepLinesTogether { get { return paragraphProperties.KeepLinesTogether; } set { paragraphProperties.KeepLinesTogether = value; } }
		public bool? PageBreakBefore { get { return paragraphProperties.PageBreakBefore; } set { paragraphProperties.PageBreakBefore = value; } }
		public bool? ContextualSpacing { get { return paragraphProperties.ContextualSpacing; } set { paragraphProperties.ContextualSpacing = value; } }
		Color? ParagraphPropertiesBase.BackColor { get { return paragraphProperties.BackColor; } set { paragraphProperties.BackColor = value; } }
		public TabInfoCollection BeginUpdateTabs(bool onlyOwnTabs) {
			return paragraphProperties.BeginUpdateTabs(onlyOwnTabs);
		}
		public void EndUpdateTabs(TabInfoCollection tabs) {
			paragraphProperties.EndUpdateTabs(tabs);
		}
		void ParagraphPropertiesBase.Reset() {
			paragraphProperties.Reset();
		}
		public void Reset(ParagraphPropertiesMask mask) {
			paragraphProperties.Reset(mask);
		}
		#endregion
		#region TablePropertiesBase Members
		public float? TopPadding { get { return tableProperties.TopPadding; } set { tableProperties.TopPadding = value; } }
		public float? BottomPadding { get { return tableProperties.BottomPadding; } set { tableProperties.BottomPadding = value; } }
		public float? LeftPadding { get { return tableProperties.LeftPadding; } set { tableProperties.LeftPadding = value; } }
		public float? RightPadding { get { return tableProperties.RightPadding; } set { tableProperties.RightPadding = value; } }
		public float? TableCellSpacing { get { return tableProperties.TableCellSpacing; } set { tableProperties.TableCellSpacing = value; } }
		public TableRowAlignment? TableAlignment { get { return tableProperties.TableAlignment; } set { tableProperties.TableAlignment = value; } }
		public TableLayoutType? TableLayout { get { return tableProperties.TableLayout; } set { tableProperties.TableLayout = value; } }
		public TableBorders TableBorders { get { return tableProperties.TableBorders; } }
		public Color? TableBackgroundColor { get { return tableProperties.TableBackgroundColor; } set { tableProperties.TableBackgroundColor = value; } }
		void TablePropertiesBase.Reset() {
			tableProperties.Reset();
		}
		public void Reset(TablePropertiesMask mask) {
			tableProperties.Reset(mask);
		}
		#endregion
		#region TableCellPropertiesBase Members
		float? TableCellPropertiesBase.CellTopPadding { get { return tableCellProperties.CellTopPadding; } set { tableCellProperties.CellTopPadding = value; } }
		float? TableCellPropertiesBase.CellBottomPadding { get { return tableCellProperties.CellBottomPadding; } set { tableCellProperties.CellBottomPadding = value; } }
		float? TableCellPropertiesBase.CellLeftPadding { get { return tableCellProperties.CellLeftPadding; } set { tableCellProperties.CellLeftPadding = value; } }
		float? TableCellPropertiesBase.CellRightPadding { get { return tableCellProperties.CellRightPadding; } set { tableCellProperties.CellRightPadding = value; } }
		public TableCellVerticalAlignment? VerticalAlignment { get { return tableCellProperties.VerticalAlignment; } set { tableCellProperties.VerticalAlignment = value; } }
		public bool? NoWrap { get { return tableCellProperties.NoWrap; } set { tableCellProperties.NoWrap = value; } }
		public TableCellBorders TableCellBorders { get { return tableCellProperties.TableCellBorders; } }
		public Color? CellBackgroundColor { get { return tableCellProperties.CellBackgroundColor; } set { tableCellProperties.CellBackgroundColor = value; } }
		void TableCellPropertiesBase.Reset() {
			tableCellProperties.Reset();
		}
		public void Reset(TableCellPropertiesMask mask) {
			tableCellProperties.Reset(mask);
		}
		#endregion
	}
	#region NativeTableStyleCollection
	public class NativeTableStyleCollection : NativeStyleCollectionBase<TableStyle, ModelTableStyle>, TableStyleCollection {
		internal NativeTableStyleCollection(NativeDocument document)
			: base(document) {
		}
		protected internal override StyleCollectionBase<ModelTableStyle> InnerStyles { get { return DocumentModel.TableStyles; } }
		protected internal override TableStyle CreateNew(ModelTableStyle style) {
			return new NativeTableStyle(Document, style);
		}
		protected internal override ModelTableStyle CreateModelStyle() {
			return new ModelTableStyle(DocumentModel);
		}
		protected internal override ModelTableStyle GetModelStyle(TableStyle style) {
			return ((NativeTableStyle)style).InnerStyle;
		}
	}
	#endregion
	#region NativeTableConditionalStyleProperties
	public class NativeTableConditionalStyleProperties : TableConditionalStyleProperties {
		static readonly ConditionalTableStyleFormattingTypes[] styleTypes;
		public static ConditionalTableStyleFormattingTypes[] StyleTypes { get { return styleTypes; } }
		static NativeTableConditionalStyleProperties() {
			List<ConditionalTableStyleFormattingTypes> styleTypeList = new List<ConditionalTableStyleFormattingTypes>();
			foreach (ConditionalTableStyleFormattingTypes styleType in Enum.GetValues(typeof(ConditionalTableStyleFormattingTypes))) {
				styleTypeList.Add(styleType);
			}
			styleTypes = styleTypeList.ToArray();
		}
		readonly Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle> items;
		readonly TableStyle owner;
		public NativeTableConditionalStyleProperties(TableStyle owner) {
			this.items = new Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle>();
			this.owner = owner;
			InitializeItems();
			foreach (ConditionalTableStyleFormattingTypes condition in StyleTypes) {
				if (((NativeTableStyle)owner).InnerStyle.ConditionalStyleProperties[(ModelConditionalTableStyleFormattingTypes)condition] != null)
				CreateConditionalStyle(condition);
			}
		}
		public TableConditionalStyle this[ConditionalTableStyleFormattingTypes condition] { get { return items[condition]; } }
		internal Dictionary<ConditionalTableStyleFormattingTypes, TableConditionalStyle> Items { get { return items; } }
		public TableStyle Owner { get { return owner; } }
		void InitializeItems() {
			foreach (ConditionalTableStyleFormattingTypes condition in StyleTypes) {
				Items.Add(condition, null);
			}
		}
		public TableConditionalStyle CreateConditionalStyle(ConditionalTableStyleFormattingTypes condition) {
			if (items[condition] == null) {
				NativeTableStyle style = (NativeTableStyle)this.Owner;
				if (style.InnerStyle.ConditionalStyleProperties[(ModelConditionalTableStyleFormattingTypes)condition] == null)
					style.InnerStyle.ConditionalStyleProperties.AddStyle(new ModelTableConditionalStyle(style.InnerStyle, (ModelConditionalTableStyleFormattingTypes)condition));
				items[condition] = new NativeTableConditionalStyle(style.Document, style.InnerStyle.ConditionalStyleProperties[(ModelConditionalTableStyleFormattingTypes)condition]);
			}
			return items[condition];
		}
	}
	#endregion
	public class NativeTableConditionalStyle : TableConditionalStyle {
		readonly NativeDocument document;
		readonly ModelTableConditionalStyle innerStyle;
		readonly TablePropertiesBase tableProperties;
		readonly TableCellPropertiesBase tableCellProperties;
		readonly ParagraphPropertiesWithTabs paragraphProperties;
		readonly CharacterPropertiesBase characterProperties;
		internal NativeTableConditionalStyle(NativeDocument document, ModelTableConditionalStyle innerStyle) {
			this.document = document;
			this.innerStyle = innerStyle;
			this.tableProperties = new NativeStyleTableProperties(document, innerStyle.TableProperties);
			this.tableCellProperties = new NativeStyleTableCellProperties(document, innerStyle.TableCellProperties);
			this.paragraphProperties = new NativeStyleParagraphProperties(document, innerStyle.Tabs, innerStyle.ParagraphProperties);
			this.characterProperties = new NativeSimpleCharacterProperties(innerStyle.CharacterProperties);
		}
		#region Properties
		NativeDocument Document { get { return document; } }
		public ModelTableConditionalStyle InnerStyle { get { return innerStyle; } }
		protected internal DevExpress.XtraRichEdit.Model.DocumentModel DocumentModel { get { return innerStyle.DocumentModel; } }
		#endregion
		public TablePropertiesBase TableProperties { get { return tableProperties; } }
		public TableCellPropertiesBase TableCellProperties { get { return tableCellProperties; } }
		public ParagraphPropertiesWithTabs ParagraphProperties { get { return paragraphProperties; } }
		public CharacterPropertiesBase CharacterProperties { get { return characterProperties; } }
		#region TableStyle Members
		public string Name { get { return String.Empty; } set { return; } }
		public bool IsDeleted { get { return false; } }
		#endregion
		#region CharacterProperties Members
		public string FontName { get { return characterProperties.FontName; } set { characterProperties.FontName = value; } }
		public float? FontSize { get { return characterProperties.FontSize; } set { characterProperties.FontSize = value; } }
		public bool? Bold { get { return characterProperties.Bold; } set { characterProperties.Bold = value; } }
		public bool? Italic { get { return characterProperties.Italic; } set { characterProperties.Italic = value; } }
		public bool? Superscript { get { return characterProperties.Superscript; } set { characterProperties.Superscript = value; } }
		public bool? Subscript { get { return characterProperties.Subscript; } set { characterProperties.Subscript = value; } }
		public bool? AllCaps { get { return characterProperties.AllCaps; } set { characterProperties.AllCaps = value; } }
		public UnderlineType? Underline { get { return characterProperties.Underline; } set { characterProperties.Underline = value; } }
		public StrikeoutType? Strikeout { get { return characterProperties.Strikeout; } set { characterProperties.Strikeout = value; } }
		public Color? UnderlineColor { get { return characterProperties.UnderlineColor; } set { characterProperties.UnderlineColor = value; } }
		public Color? ForeColor { get { return characterProperties.ForeColor; } set { characterProperties.ForeColor = value; } }
		Color? CharacterPropertiesBase.BackColor { get { return characterProperties.BackColor; } set { characterProperties.BackColor = value; } }
		public bool? Hidden { get { return characterProperties.Hidden; } set { characterProperties.Hidden = value; } }
		public LangInfo? Language { get { return characterProperties.Language; } set { characterProperties.Language = value; } }
		public bool? NoProof { get { return characterProperties.NoProof; } set { characterProperties.NoProof = value; } }
		void CharacterPropertiesBase.Reset() {
			characterProperties.Reset();
		}
		public void Reset(CharacterPropertiesMask mask) {
			characterProperties.Reset(mask);
		}
		public void Reset(ParagraphPropertiesMask mask) {
			paragraphProperties.Reset(mask);
		}
		#endregion
		#region ParagraphProperties Members
		public ParagraphAlignment? Alignment { get { return paragraphProperties.Alignment; } set { paragraphProperties.Alignment = value; } }
		public float? LeftIndent { get { return paragraphProperties.LeftIndent; } set { paragraphProperties.LeftIndent = value; } }
		public float? RightIndent { get { return paragraphProperties.RightIndent; } set { paragraphProperties.RightIndent = value; } }
		public float? SpacingBefore { get { return paragraphProperties.SpacingBefore; } set { paragraphProperties.SpacingBefore = value; } }
		public float? SpacingAfter { get { return paragraphProperties.SpacingAfter; } set { paragraphProperties.SpacingAfter = value; } }
		public ParagraphLineSpacing? LineSpacingType { get { return paragraphProperties.LineSpacingType; } set { paragraphProperties.LineSpacingType = value; } }
		public float? LineSpacing { get { return paragraphProperties.LineSpacing; } set { paragraphProperties.LineSpacing = value; } }
		public float? LineSpacingMultiplier { get { return paragraphProperties.LineSpacingMultiplier; } set { paragraphProperties.LineSpacingMultiplier = value; } }
		public ParagraphFirstLineIndent? FirstLineIndentType { get { return paragraphProperties.FirstLineIndentType; } set { paragraphProperties.FirstLineIndentType = value; } }
		public float? FirstLineIndent { get { return paragraphProperties.FirstLineIndent; } set { paragraphProperties.FirstLineIndent = value; } }
		public bool? SuppressHyphenation { get { return paragraphProperties.SuppressHyphenation; } set { paragraphProperties.SuppressHyphenation = value; } }
		public bool? SuppressLineNumbers { get { return paragraphProperties.SuppressLineNumbers; } set { paragraphProperties.SuppressLineNumbers = value; } }
		public int? OutlineLevel { get { return paragraphProperties.OutlineLevel; } set { paragraphProperties.OutlineLevel = value; } }
		public bool? KeepLinesTogether { get { return paragraphProperties.KeepLinesTogether; } set { paragraphProperties.KeepLinesTogether = value; } }
		public bool? PageBreakBefore { get { return paragraphProperties.PageBreakBefore; } set { paragraphProperties.PageBreakBefore = value; } }
		public bool? ContextualSpacing { get { return paragraphProperties.ContextualSpacing; } set { paragraphProperties.ContextualSpacing = value; } }
		Color? ParagraphPropertiesBase.BackColor { get { return paragraphProperties.BackColor; } set { paragraphProperties.BackColor = value; } }
		public TabInfoCollection BeginUpdateTabs(bool onlyOwnTabs) {
			return paragraphProperties.BeginUpdateTabs(onlyOwnTabs);
		}
		public void EndUpdateTabs(TabInfoCollection tabs) {
			paragraphProperties.EndUpdateTabs(tabs);
		}
		void ParagraphPropertiesBase.Reset() {
			paragraphProperties.Reset();
		}
		#endregion
		#region TablePropertiesBase Members
		public float? TopPadding { get { return tableProperties.TopPadding; } set { tableProperties.TopPadding = value; } }
		public float? BottomPadding { get { return tableProperties.BottomPadding; } set { tableProperties.BottomPadding = value; } }
		public float? LeftPadding { get { return tableProperties.LeftPadding; } set { tableProperties.LeftPadding = value; } }
		public float? RightPadding { get { return tableProperties.RightPadding; } set { tableProperties.RightPadding = value; } }
		public float? TableCellSpacing { get { return tableProperties.TableCellSpacing; } set { tableProperties.TableCellSpacing = value; } }
		public TableRowAlignment? TableAlignment { get { return tableProperties.TableAlignment; } set { tableProperties.TableAlignment = value; } }
		public TableLayoutType? TableLayout { get { return tableProperties.TableLayout; } set { tableProperties.TableLayout = value; } }
		public TableBorders TableBorders { get { return tableProperties.TableBorders; } }
		public Color? TableBackgroundColor { get { return tableProperties.TableBackgroundColor; } set { tableProperties.TableBackgroundColor = value; } }
		void TablePropertiesBase.Reset() {
			tableProperties.Reset();
		}
		public void Reset(TablePropertiesMask mask) {
			tableProperties.Reset(mask);
		}
		#endregion
		#region TableCellPropertiesBase Members
		float? TableCellPropertiesBase.CellTopPadding { get { return tableCellProperties.CellTopPadding; } set { tableCellProperties.CellTopPadding = value; } }
		float? TableCellPropertiesBase.CellBottomPadding { get { return tableCellProperties.CellBottomPadding; } set { tableCellProperties.CellBottomPadding = value; } }
		float? TableCellPropertiesBase.CellLeftPadding { get { return tableCellProperties.CellLeftPadding; } set { tableCellProperties.CellLeftPadding = value; } }
		float? TableCellPropertiesBase.CellRightPadding { get { return tableCellProperties.CellRightPadding; } set { tableCellProperties.CellRightPadding = value; } }
		public TableCellVerticalAlignment? VerticalAlignment { get { return tableCellProperties.VerticalAlignment; } set { tableCellProperties.VerticalAlignment = value; } }
		public bool? NoWrap { get { return tableCellProperties.NoWrap; } set { tableCellProperties.NoWrap = value; } }
		public TableCellBorders TableCellBorders { get { return tableCellProperties.TableCellBorders; } }
		public Color? CellBackgroundColor { get { return tableCellProperties.CellBackgroundColor; } set { tableCellProperties.CellBackgroundColor = value; } }
		void TableCellPropertiesBase.Reset() {
			tableCellProperties.Reset();
		}
		public void Reset(TableCellPropertiesMask mask) {
			tableCellProperties.Reset(mask);
		}
		#endregion
	}
}
