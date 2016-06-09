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
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Data.Utils;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if !SL
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Html {
	#region CssKeywordTranslator
	public delegate void CssPropertiesTranslateHandler(CssProperties cssProperties, List<string> propertiesValues);
	public class CssKeywordTranslatorTable : Dictionary<string, CssPropertiesTranslateHandler> {
	}
	#endregion
	#region RelativeProperties
	public class RelativeProperties {
		#region Field
		bool hasRelativeProperties;
		string unitRelativeFontSize;
		string unitRelativeFirstLineIndent;
		string unitRelativeLeftIndent;
		string unitRelativeRightIndent;
		string unitRelativeLineSpacing;
		string unitRelativeSpacingAfter;
		string unitRelativeSpacingBefore;
		string unitRelativeWidth;
		string unitRelativeHeight;
		float relativeWidth;
		float relativeHeight;
		#endregion
		public RelativeProperties() {
			this.unitRelativeFirstLineIndent = String.Empty;
			this.unitRelativeFontSize = String.Empty;
			this.unitRelativeLeftIndent = String.Empty;
			this.unitRelativeRightIndent = String.Empty;
			this.unitRelativeLineSpacing = String.Empty;
			this.unitRelativeSpacingAfter = String.Empty;
			this.unitRelativeSpacingBefore = String.Empty;
			this.unitRelativeWidth = String.Empty;
			this.unitRelativeHeight = String.Empty;
		}
		#region Properties
		public bool HasRelativeProperties { get { return hasRelativeProperties; } }
		public string UnitRelativeFontSize {
			get { return unitRelativeFontSize; }
			set {
				unitRelativeFontSize = value;
				OnPropertyChanged(value);
			}
		}
		public string UnitRelativeFirstLineIndent {
			get { return unitRelativeFirstLineIndent; }
			set {
				unitRelativeFirstLineIndent = value;
				OnPropertyChanged(value);
			}
		}
		public string UnitRelativeLeftIndent {
			get { return unitRelativeLeftIndent; }
			set {
				unitRelativeLeftIndent = value;
				OnPropertyChanged(value);
			}
		}
		public string UnitRelativeRightIndent {
			get { return unitRelativeRightIndent; }
			set {
				unitRelativeRightIndent = value;
				OnPropertyChanged(value);
			}
		}
		public string UnitRelativeLineSpacing {
			get { return unitRelativeLineSpacing; }
			set {
				unitRelativeLineSpacing = value;
				OnPropertyChanged(value);
			}
		}
		public string UnitRelativeSpacingAfter {
			get { return unitRelativeSpacingAfter; }
			set {
				unitRelativeSpacingAfter = value;
				OnPropertyChanged(value);
			}
		}
		public string UnitRelativeSpacingBefore {
			get { return unitRelativeSpacingBefore; }
			set {
				unitRelativeSpacingBefore = value;
				OnPropertyChanged(value);
			}
		}
		public string UnitRelativeWidth {
			get { return unitRelativeWidth; }
			set {
				unitRelativeWidth = value;
				OnPropertyChanged(value);
			}
		}
		public string UnitRelativeHeight {
			get { return unitRelativeHeight; }
			set {
				unitRelativeHeight = value;
				OnPropertyChanged(value);
			}
		}
		public float RelativeWidth { get { return relativeWidth; } set { relativeWidth = value; } }
		public float RelativeHeight { get { return relativeHeight; } set { relativeHeight = value; } }
		#endregion
		protected internal void CopyFrom(RelativeProperties value) {
			this.hasRelativeProperties = value.hasRelativeProperties;
			this.unitRelativeFirstLineIndent = value.unitRelativeFirstLineIndent;
			this.unitRelativeFontSize = value.unitRelativeFontSize;
			this.unitRelativeLeftIndent = value.unitRelativeLeftIndent;
			this.unitRelativeRightIndent = value.unitRelativeRightIndent;
			this.unitRelativeLineSpacing = value.unitRelativeLineSpacing;
			this.unitRelativeSpacingAfter = value.unitRelativeSpacingAfter;
			this.unitRelativeSpacingBefore = value.unitRelativeSpacingBefore;
			this.unitRelativeWidth = value.unitRelativeWidth;
			this.unitRelativeHeight = value.unitRelativeHeight;
			this.relativeWidth = value.RelativeWidth;
			this.relativeHeight = value.RelativeHeight;
		}
		void OnPropertyChanged(string value) {
			if (!this.hasRelativeProperties)
				this.hasRelativeProperties = IsRelative(value);
		}
		internal static bool IsRelative(string relativeUnit) {
			if (String.IsNullOrEmpty(relativeUnit))
				return false;
			return relativeUnit == "em" || relativeUnit == "rem" || relativeUnit == "ex" || relativeUnit == "%";
		}
		public bool IsDefaultProperties() {
			return String.IsNullOrEmpty(this.unitRelativeFirstLineIndent) &&
			String.IsNullOrEmpty(this.unitRelativeFontSize) &&
			String.IsNullOrEmpty(this.unitRelativeLeftIndent) &&
			String.IsNullOrEmpty(this.unitRelativeRightIndent) &&
			String.IsNullOrEmpty(this.unitRelativeLineSpacing) &&
			String.IsNullOrEmpty(this.unitRelativeSpacingAfter) &&
			String.IsNullOrEmpty(this.unitRelativeSpacingBefore) &&
			String.IsNullOrEmpty(this.unitRelativeWidth) &&
			String.IsNullOrEmpty(this.unitRelativeHeight);
		}
	}
	#endregion
	#region UnitConversionParameters
	public class UnitConversionParameters {
		int originalValue;
		float em;
		float ex;
		float dpi;
		#region Properties
		public int OriginalValue { get { return originalValue; } set { originalValue = value; } }
		public float Em { get { return em; } set { em = value; } }
		public float Ex { get { return ex; } set { ex = value; } }
		public float Dpi { get { return dpi; } set { dpi = value; } }
		static readonly UnitConversionParameters empty = CreateEmptyUnitConversionParameters();
		public static UnitConversionParameters Empty { get { return empty; } }
		#endregion
		UnitConversionParameters() {
		}
		public static UnitConversionParameters CreateEmptyUnitConversionParameters() {
			UnitConversionParameters parameters = new UnitConversionParameters();
			parameters.dpi = 96;
			parameters.em = 1;
			parameters.ex = 0.5f;
			parameters.originalValue = 0;
			return parameters;
		}
	}
	#endregion
	#region UnitConverter
	public class UnitConverter {
		readonly DocumentModelUnitConverter unitConverter;
		public UnitConverter(DocumentModelUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
		}
		public int ToModelUnits(DXUnit value) {
			return ToModelUnits(value, UnitConversionParameters.Empty);
		}
		public int DegreeToModelUnits(DXRotationUnit value) {
			return (int)unitConverter.DegreeToModelUnits((int)value.Value);
		}
		public int FDToModelUnits(DXRotationUnit value) {
			return (int)unitConverter.FDToModelUnits((int)value.Value);
		}
		public int ToModelUnits(DXUnit value, UnitConversionParameters parameters) {
			return (int)unitConverter.PointsToModelUnitsF(ToPoints(value, parameters));
		}
		public float ToPoints(DXUnit value, UnitConversionParameters parameters) {
			switch (value.Type) {
				case DXUnitType.Em:
					return parameters.OriginalValue * value.Value * parameters.Em;
				case DXUnitType.Ex:
					return parameters.OriginalValue * value.Value * parameters.Ex;
				case DXUnitType.Percentage:
					return parameters.OriginalValue * value.Value / 100;
				case DXUnitType.Point:
					return value.Value;
				case DXUnitType.Pica:
					return value.Value * 12;
				case DXUnitType.Pixel:
					return Units.PixelsToPointsF(value.Value, parameters.Dpi);
				case DXUnitType.Inch:
					return value.Value * 72;
				case DXUnitType.Cm:
					return Units.MillimetersToPointsF(value.Value * 10.0f);
				case DXUnitType.Mm:
					return Units.MillimetersToPointsF(value.Value);
			}
			return value.Value;
		}
	}
	#endregion
	#region UnitValueParser
	public class UnitValueParser {
		DXUnit unit;
		public UnitValueParser() {
			this.unit = new DXUnit();
		}
		public DXUnit TryParse(string inputValue) {
			try {
				unit = new DXUnit(inputValue);
			}
			catch {
			}
			return unit;
		}
	}
	#endregion
	#region LengthValueParser
	public class LengthValueParser {
		#region Field
		float pointsValue;
		bool isRelativeUnit;
		ValueInfo valueInfo;
		string relativeUnit;
		string lengthValue;
		float dpi;
		bool success;
		#endregion
		public LengthValueParser(string lengthValue, float dpi) {
			this.dpi = dpi;
			this.relativeUnit = String.Empty;
			this.lengthValue = lengthValue;
			this.valueInfo = ValueInfo.Empty;
			ParseLengthValue();
		}
		public LengthValueParser(string lengthValue)
			: this(lengthValue, DocumentModelDpi.Dpi) {
		}
		#region Properties
		public float PointsValue { get { return pointsValue; } }
		public float Value { get { return valueInfo.Value; } }
		public string Unit { get { return valueInfo.Unit; } }
		public string RelativeUnit { get { return relativeUnit; } }
		public bool IsRelativeUnit { get { return isRelativeUnit; } }
		public bool IsDigit { get { return valueInfo.IsValidNumber; } }
		public bool Success { get { return success; } }
		#endregion
		protected internal void ParseLengthValue() {
			this.success = Parse();
		}
		bool Parse() {
			if (String.IsNullOrEmpty(lengthValue))
				return false;
			this.valueInfo = StringValueParser.TryParse(lengthValue);
			if (this.valueInfo == ValueInfo.Empty)
				return false;
			if (IsDigit) {
				pointsValue = GetLengthValueInPoints(this.valueInfo);
				if (relativeUnit == "em" || relativeUnit == "rem" || relativeUnit == "ex" || relativeUnit == "%")
					isRelativeUnit = true;
			}
			return true;
		}
		protected internal float GetLengthValueInPoints(ValueInfo valueInfo) {
			return GetLengthValueInPoints(valueInfo, UnitConversionParameters.Empty);
		}
		protected internal float GetLengthValueInPoints(ValueInfo valueInfo, UnitConversionParameters v) {
			float value = valueInfo.Value;
			string unit = valueInfo.Unit.ToUpper(CultureInfo.InvariantCulture);
			if(unit.StartsWith("%")) {
				relativeUnit = "%";
				return value;
			}
			if (unit.Length >= 2) {
				if (unit.StartsWith("REM")) {
					relativeUnit = "rem";
					return value * 100;
				}
				switch (unit.Substring(0, 2)) {
					case "EM":
						relativeUnit = "em";
						return value * 100;					
					case "EX":
						relativeUnit = "ex";
						return value * 100 / 2;
					case "PT":
						return value;
					case "PC":
						return value * 12;
					case "PX":
						return Units.PixelsToPointsF(value, this.dpi);
					case "IN":
						return value * 72;
					case "CM":
						return Units.MillimetersToPointsF(value * 10);
					case "MM":
						return Units.MillimetersToPointsF(value);
				}
			}
			return value;
		}
	}
	#endregion
	#region CssProperties
	public class CssProperties : ICellPropertiesOwner {
		#region Fields
		readonly ParagraphFormattingBase paragraphProperties;
		readonly CharacterFormattingBase characterProperties;
		HtmlListLevelProperties listLevelProperties;
		HtmlTableProperties tableProperties;
		TableCellProperties cellProperties;
		HtmlTableRowProperties rowProperties;
		RelativeProperties relativeProperties;
		HtmlBordersProperties bordersProperties;
		HtmlImageProperties imageProperties;
		HtmlCssFloat cssFloat;
		PieceTable pieceTable;
		#endregion
		public CssProperties(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.paragraphProperties = new ParagraphFormattingBase(pieceTable, pieceTable.DocumentModel, ParagraphFormattingInfoCache.DefaultItemIndex, ParagraphFormattingOptionsCache.EmptyParagraphFormattingOptionIndex);
			this.characterProperties = new CharacterFormattingBase(pieceTable, pieceTable.DocumentModel, CharacterFormattingInfoCache.DefaultItemIndex, CharacterFormattingOptionsCache.EmptyCharacterFormattingOptionIndex);
			this.characterProperties.BeginUpdate();
			this.characterProperties.FontName = "Times New Roman";
			this.characterProperties.DoubleFontSize = 24;
			this.characterProperties.ResetUse(CharacterFormattingOptions.Mask.UseAll);
			this.characterProperties.EndUpdate();
			this.listLevelProperties = new HtmlListLevelProperties();
			this.tableProperties = new HtmlTableProperties();
			this.rowProperties = new HtmlTableRowProperties();
			this.cellProperties = new TableCellProperties(pieceTable, this);
			this.relativeProperties = new RelativeProperties();
			this.bordersProperties = new HtmlBordersProperties();
			this.imageProperties = new HtmlImageProperties();
			this.cssFloat = HtmlCssFloat.NotSet;
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public ParagraphFormattingBase CssParagraphProperties { get { return paragraphProperties; } }
		public CharacterFormattingBase CssCharacterProperties { get { return characterProperties; } }
		public HtmlListLevelProperties ListLevelProperties { get { return listLevelProperties; } }
		public HtmlTableProperties TableProperties { get { return tableProperties; } }
		public TableCellProperties CellProperties { get { return cellProperties; } }
		public HtmlTableRowProperties RowProperties { get { return rowProperties; } }
		public HtmlBordersProperties BordersProperties { get { return bordersProperties; } }
		public HtmlImageProperties ImageProperties { get { return imageProperties; } }
		public HtmlCssFloat CssFloat { get { return cssFloat; } set { cssFloat = value; } }
		public RelativeProperties RelativeProperties { get { return relativeProperties; } }
		protected internal DocumentModelUnitConverter UnitConverter { get { return ((DocumentModel)paragraphProperties.DocumentModel).UnitConverter; } }
		#endregion
		protected internal void CopyFrom(CssProperties value) {
			this.characterProperties.CopyFrom(value.characterProperties);
			this.paragraphProperties.CopyFrom(value.paragraphProperties);
			this.listLevelProperties.CopyFrom(value.listLevelProperties);
			this.tableProperties.CopyFrom(value.tableProperties);
			this.rowProperties.CopyFrom(value.rowProperties);
			this.cellProperties.CopyFrom(value.cellProperties);
			this.relativeProperties.CopyFrom(value.relativeProperties);
			this.bordersProperties.CopyFrom(value.bordersProperties);
			this.imageProperties.CopyFrom(value.imageProperties);
			this.cssFloat = value.cssFloat;
		}
		protected internal bool IsDefaultProperties() {
			return CssCharacterProperties.Options.Value == CharacterFormattingOptions.Mask.UseNone &&
				CssParagraphProperties.Options.Value == ParagraphFormattingOptions.Mask.UseNone &&
				ListLevelProperties.IsDefaultProperties() &&
				TableProperties.IsDefaultProperties() &&
				RowProperties.IsDefaultProperties() &&
				BordersProperties.IsDefaultProperties() &&
				ImageProperties.IsDefaultProperties() &&
				RelativeProperties.IsDefaultProperties() &&
				cssFloat == HtmlCssFloat.NotSet &&
				Object.Equals(CellProperties.GetCache(CellProperties.DocumentModel).DefaultItem, CellProperties.Info);
		}
		protected internal void Reset(DocumentModel documentModel) {
			CssCharacterProperties.ReplaceInfo(documentModel.Cache.CharacterFormattingInfoCache.DefaultItem, new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseNone));
			CssCharacterProperties.DoubleFontSize = 24;
			CssCharacterProperties.FontName = "Times New Roman";
			CssCharacterProperties.ResetUse(CharacterFormattingOptions.Mask.UseAll);
			CssParagraphProperties.ReplaceInfo(documentModel.Cache.ParagraphFormattingInfoCache.DefaultItem, new ParagraphFormattingOptions(ParagraphFormattingOptions.Mask.UseNone));
			listLevelProperties = new HtmlListLevelProperties();
			tableProperties = new HtmlTableProperties();
			rowProperties = new HtmlTableRowProperties();
			cellProperties.Reset();
			relativeProperties = new RelativeProperties();
			bordersProperties = new HtmlBordersProperties();
			imageProperties = new HtmlImageProperties();
			cssFloat = HtmlCssFloat.NotSet;
		}
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == CellProperties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
	}
	#endregion
	#region CssElement
	public class CssElement {
		readonly int index;
		Selector selector;
		readonly CssProperties properties;
		public CssElement(PieceTable pieceTable, int index) {
			this.properties = new CssProperties(pieceTable);
			this.selector = new Selector();
			this.index = index;
		}
		public int Index { get { return index; } }
		public Selector Selector { get { return selector; } set { selector = value; } }
		public CssProperties Properties { get { return properties; } }
	}
	#endregion
	#region CssElementCollection
	public class CssElementCollection : List<CssElement> {
	}
	#endregion
	#region CssPropertiesState
	public enum CssParserState {
		ReadSelector,
		ReadPropertiesName,
		WaitColonSymbol,
		SkipProperties,
		WaitPropertiesValue,
		ReadPropertiesValueInQuotes,
		ReadPropertiesValueInApostrophe,
		ReadPropertiesValueInParentheses,
		ReadPropertiesValue,
		WaitPropertiesKeyword,
		ReadPropertiesKeyword,
		WaitStartComment,
		ReadComment,
		WaitEndComment,
		SkipNestedProperties,
	}
	#endregion
	#region CssParser
	public class CssParser {
		#region Field
		readonly DocumentModel documentModel;
		string selectorsText;
		List<Selector> selectors;
		readonly CssElementCollection cssElementCollection;
		CssParserState state;
		CssParserState parentState;
		readonly StringBuilder properties;
		readonly StringBuilder propertiesValue;
		readonly StringBuilder propertiesKeyword;
		readonly List<string> propertiesValues;
		readonly CssProperties cssProperties;
		int bracketCount = 0;
		#endregion
		readonly static CssKeywordTranslatorTable acssKeywordTable = CreateCssKeywordTable();
		static CssKeywordTranslatorTable CreateCssKeywordTable() {
			CssKeywordTranslatorTable cssKeywordTable = new CssKeywordTranslatorTable();
			cssKeywordTable.Add(ConvertKeyToUpper("font-size"), CssFontSizeKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("background"), CssBackgroundKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("background-attachment"), CssBackgroundAttachmentKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("background-color"), CssBackgroundColorKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("background-image"), CssBackgroundImageKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("background-position"), CssBackgroundPositionKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("background-repeat"), CssBackgroundRepeatKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("before"), CssBeforeKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border"), CssBorderKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-bottom"), CssBorderBottomKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-bottom-color"), CssBorderBottomColorKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-bottom-style"), CssBorderBottomStyleKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-bottom-width"), CssBorderBottomWidthKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-collapse"), CssBorderCollapseKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-color"), CssBorderColorKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-left"), CssBorderLeftKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-left-color"), CssBorderLeftColorKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-left-style"), CssBorderLeftStyleKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-left-width"), CssBorderLeftWidthKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-right"), CssBorderRightKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-right-color"), CssBorderRightColorKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-right-style"), CssBorderRightStyleKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-right-width"), CssBorderRightWidthKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-spacing"), CssBorderSpacingKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-style"), CssBorderStyleKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-top"), CssBorderTopKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-top-color"), CssBorderTopColorKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-top-style"), CssBorderTopStyleKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-top-width"), CssBorderTopWidthKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("border-width"), CssBorderWidthKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("bottom"), CssBottomKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("caption-side"), CssCaptionSideKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("clear"), CssClearKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("clip"), CssClipKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("color"), CssColorKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("content"), CssContentKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("counter-increment"), CssCounterIncrementKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("counter-reset"), CssCounterResetKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("cursor"), CssCursorKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("direction"), CssDirectionKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("display"), CssDisplayKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("empty-cells"), CssEmptyCellsKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("first-child"), CssFirstChildKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("float"), CssFloatKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("focus"), CssFocusKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("font"), CssFontKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("font-family"), CssFontFamilyKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("font-style"), CssFontStyleKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("font-variant"), CssFontVariantKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("font-weight"), CssFontWeightKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("height"), CssHeightKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("hover"), CssHoverKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("left"), CssLeftKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("letter-spacing"), CssLetterSpacingKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("line-height"), CssLineHeightKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("link"), CssLinkKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("list-style"), CssListStyleKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("list-style-image"), CssListStyleImageKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("list-style-position"), CssListStylePositionKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("list-style-type"), CssListStyleTypeKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("margin"), CssMarginKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("margin-bottom"), CssMarginBottomKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("margin-left"), CssMarginLeftKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("margin-right"), CssMarginRightKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("margin-top"), CssMarginTopKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("max-height"), CssMaxHeightKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("max-width"), CssMaxWidthKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("min-height"), CssMinHeightKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("min-width"), CssMinWidthKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-margin-top-alt"), CssBeforeAutoSpacing);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-margin-bottom-alt"), CssAfterAutoSpacing);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-pagination"), CssMsoPagination);
			cssKeywordTable.Add(ConvertKeyToUpper("opacity"), CssOpacityKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("outline"), CssOutlineKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("outline-color"), CssOutlineColorKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("outline-style"), CssOutlineStyleKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("outline-width"), CssOutlineWidthKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("overflow"), CssOverflowKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("padding"), CssPaddingKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("padding-bottom"), CssPaddingBottomKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("padding-left"), CssPaddingLeftKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("padding-right"), CssPaddingRightKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("padding-top"), CssPaddingTopKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("page-break-after"), CssPageBreakAfterKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("page-break-before"), CssPageBreakBeforeKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("page-break-inside"), CssPageBreakInsideKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("position"), CssPositionKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("quotes"), CssQuotesKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("right"), CssRightKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("table-layout"), CssTableLayoutKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("text-align"), CssTextAlignKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("text-decoration"), CssTextDecorationKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("text-indent"), CssTextIndentKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("text-transform"), CssTextTransformKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("top"), CssTopKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("unicode-bidi"), CssUnicodeBidiKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("vertical-align"), CssVerticalAlignKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("visibility"), CssVisibilityKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("visited"), CssVisitedKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("white-space"), CssWhiteSpaceKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("width"), CssWidthKeyword);
			cssKeywordTable.Add(ConvertKeyToUpper("word-spacing"), CssWordSpacingKeyword);
			return cssKeywordTable;
		}
		protected internal static string ConvertKeyToUpper(string key) {
			return key.ToUpper(CultureInfo.InvariantCulture);
		}
		protected internal static bool CompareNoCase(string str1, string str2) {
			return StringExtensions.CompareInvariantCultureIgnoreCase(str1, str2) == 0;
		}
		#region StaticMethods
		static internal void CssBackgroundKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			string[] properties = propertiesValue[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			int count = properties.Length;
			for (int i = 0; i < count; i++) {
				Color color = GetColor(properties[i]);
				if (DXColor.IsEmpty(color))
					continue;
				cssProperties.CssCharacterProperties.BackColor = color;
				cssProperties.CssParagraphProperties.BackColor = color;
				cssProperties.CellProperties.BackgroundColor = color;
				cssProperties.TableProperties.BackgroundColor = color;
			}
		}
		static internal void CssBackgroundAttachmentKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssBackgroundColorKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			Color color = GetColor(propertiesValue[0]);
			cssProperties.CssCharacterProperties.BackColor = color;
			cssProperties.CellProperties.BackgroundColor = color;
			cssProperties.TableProperties.BackgroundColor = color;
		}
		static internal void CssBackgroundImageKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssBackgroundPositionKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssBackgroundRepeatKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssBeforeKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssBorderKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty topBorder = cssProperties.BordersProperties.TopBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, topBorder);
			cssProperties.BordersProperties.LeftBorder.CopyFrom(topBorder);
			cssProperties.BordersProperties.RightBorder.CopyFrom(topBorder);
			cssProperties.BordersProperties.BottomBorder.CopyFrom(topBorder);
		}
		static internal void CssBorderBottomKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty bottomBorder = cssProperties.BordersProperties.BottomBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, bottomBorder);
		}
		static internal void CssBorderKeywordCore(CssProperties cssProperties, List<string> propertiesValue, HtmlBorderProperty border) {
			if (propertiesValue.Count == 0)
				return;
			string normalizedSpaces = propertiesValue[0].Replace("  ", " ");
			string propertyValueWithoutRgbColorRepresentation = normalizedSpaces;
			if (normalizedSpaces.Contains("rgb")) {
				int startIndex = normalizedSpaces.IndexOf("rgb");
				int endIndex2 = normalizedSpaces.IndexOf(")", startIndex);
				int colorRgbValueLength = endIndex2 - startIndex + 1;
				string colorRgbValue = normalizedSpaces.Substring(startIndex, colorRgbValueLength);
				TdTag.ImportBorderColor(cssProperties.UnitConverter, colorRgbValue, border);
				propertyValueWithoutRgbColorRepresentation = normalizedSpaces.Remove(startIndex, colorRgbValueLength);
				if (String.IsNullOrEmpty(propertyValueWithoutRgbColorRepresentation))
					return;
			}
			string[] bordersContent = propertyValueWithoutRgbColorRepresentation.Split(' ');
			for (int i = 0; i < bordersContent.Length; i++) {
				string value = bordersContent[i];
				TdTag.ImportBorderWidth(cssProperties.UnitConverter, value, border);
				TagBase.ImportBorderLineStyle(cssProperties.UnitConverter, value, border);
				TdTag.ImportBorderColor(cssProperties.UnitConverter, value, border);
			}
		}
		static internal void CssBorderBottomColorKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.BottomBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderBottomStyleKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.BottomBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderBottomWidthKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty bottomBorder = cssProperties.BordersProperties.BottomBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, bottomBorder);
		}
		static internal void CssBorderCollapseKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			string value = propertiesValue.Count > 0 ? propertiesValue[0].ToLowerInvariant() : String.Empty;
			if (value == "collapse")
				cssProperties.TableProperties.BorderCollapse = BorderCollapse.Collapse;
			else if (value == "separate")
				cssProperties.TableProperties.BorderCollapse = BorderCollapse.Separate;
		}
		static internal void CssBorderLeftKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.LeftBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderLeftColorKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.LeftBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderLeftStyleKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.LeftBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderLeftWidthKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.LeftBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderRightKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.RightBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderRightColorKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.RightBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderRightStyleKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.RightBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderRightWidthKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.RightBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderSpacingKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssBorderStyleKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count == 0)
				return;
			TagBase.ImportBordersLineStyles(cssProperties.UnitConverter, propertiesValue[0], cssProperties.BordersProperties);
		}
		static internal void CssBorderColorKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count == 0)
				return;
			TagBase.ImportBordersColors(cssProperties.UnitConverter, propertiesValue[0], cssProperties.BordersProperties);
		}
		static internal void CssBorderWidthKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count == 0)
				return;
			TagBase.ImportBordersWidths(cssProperties.UnitConverter, propertiesValue[0], cssProperties.BordersProperties);
		}
		static internal void CssBorderTopKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.TopBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderTopColorKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.TopBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderTopStyleKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.TopBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBorderTopWidthKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			HtmlBorderProperty border = cssProperties.BordersProperties.TopBorder;
			CssBorderKeywordCore(cssProperties, propertiesValue, border);
		}
		static internal void CssBottomKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssCaptionSideKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssClearKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssClipKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssColorKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			cssProperties.CssCharacterProperties.ForeColor = GetColor(propertiesValue[0]);
		}
		static Color GetColor(string value) {
			return MarkupLanguageColorParser.ParseColor(value);
		}
		static internal void CssContentKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssCounterIncrementKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssCounterResetKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssCursorKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssDirectionKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssDisplayKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			string value = propertiesValue[0];
			if (!String.IsNullOrEmpty(value) && CompareNoCase(value, "none"))
				cssProperties.CssCharacterProperties.Hidden = true;
		}
		static internal void CssEmptyCellsKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssFirstChildKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssFloatKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count == 0)
				return;
			string value = propertiesValue[0];
			TableRowAlignment targetAlign = TableRowAlignment.Left;
			if (!String.IsNullOrEmpty(value) && TagBase.ImportAlignment(value, ref targetAlign))
				cssProperties.TableProperties.TableAlignment = targetAlign;
			if (value == "left")
				cssProperties.CssFloat = HtmlCssFloat.Left;
			else if (value == "right")
				cssProperties.CssFloat = HtmlCssFloat.Right;
			else if (value == "none")
				cssProperties.CssFloat = HtmlCssFloat.None;
		}
		static internal void CssFocusKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		enum FontProperties {
			Style = 0,
			Variant,
			Weight,
			Size,
			Family
		}
		static internal void CssFontKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			string propertyValue = propertiesValue[0];
			int tokenStartIndex = 0;
			FontProperties nextFontProperty = FontProperties.Style;
			while (tokenStartIndex < propertyValue.Length && nextFontProperty <= FontProperties.Family) {
				int separatorIndex;
				if (nextFontProperty < FontProperties.Family) {
					separatorIndex = propertyValue.IndexOf(' ', tokenStartIndex);
					if (separatorIndex < 0)
						separatorIndex = propertyValue.Length;
				}
				else
					separatorIndex = propertyValue.Length;
				string token = propertyValue.Substring(tokenStartIndex, separatorIndex - tokenStartIndex);
				if (!String.IsNullOrEmpty(token)) {
					for (FontProperties property = nextFontProperty; property <= FontProperties.Family; property++) {
						if (TrySetFontProperty(cssProperties, property, token)) {
							nextFontProperty = property + 1;
							break;
						}
						else if (property == FontProperties.Size || property == FontProperties.Family)
							return;
					}
				}
				tokenStartIndex = separatorIndex + 1;
			}
		}
		static bool TrySetFontProperty(CssProperties cssProperties, FontProperties property, string value) {
			switch (property) {
				case FontProperties.Style:
					return SetFontStyle(cssProperties, value);
				case FontProperties.Variant:
					return SetFontVariant(cssProperties, value);
				case FontProperties.Weight:
					return SetFontWeight(cssProperties, value);
				case FontProperties.Size:
					string[] sizeParts = value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
					if (sizeParts.Length > 1) {
						string lineHeight = sizeParts[1];
						CssLineHeightKeyword(cssProperties, GetPropertiesList(lineHeight));
					}
					return SetFontSize(cssProperties, value);
				case FontProperties.Family:
					SetFontFamily(cssProperties, value);
					return true;
				default:
					return false;
			}
		}
		static bool ShouldSplitFontSizeLineHeight(String fontSizeProperties) {
			if (string.IsNullOrEmpty(fontSizeProperties)) 
				return false;
			string[] fontsizeLineHeight = fontSizeProperties.Split('/');
			if (fontsizeLineHeight.Length == 2) {
				return true;
			}
			return false;
		}
		static List<string> GetPropertiesList(string propertiy) {
			List<string> list = new List<string>();
			list.Add(propertiy);
			return list;
		}
		static internal void CssFontFamilyKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			SetFontFamily(cssProperties, propertiesValue[0]);
		}
		static void SetFontFamily(CssProperties cssProperties, string value) {
			string fontName = value.Trim(' ', '\'', '"');
			switch (fontName.ToLowerInvariant()) {
				case "serif":
					fontName = "Times New Roman";
					break;
				case "sans-serif":
					fontName = "Arial";
					break;
				case "cursive":
					fontName = "Comic Sans MS";
					break;
				case "fantasy":
					fontName = "Algerian";
					break;
				case "monospace":
					fontName = "Courier New";
					break;
			}
			cssProperties.CssCharacterProperties.FontName = fontName;
		}
		static internal void CssFontSizeKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (String.IsNullOrEmpty(propertiesValue[0]))
				return;
			SetFontSize(cssProperties, propertiesValue[0]);
		}
		static bool SetFontSize(CssProperties cssProperties, string value) {
			CharacterFormattingBase characterFormatting = cssProperties.CssCharacterProperties;
			HtmlFontSize htmlFontSize = new HtmlFontSize();
			switch (value.ToLowerInvariant()) {
				case "xx-small":
					characterFormatting.DoubleFontSize = htmlFontSize.GetDoubleFontSize(1);
					break;
				case "x-small":
					characterFormatting.DoubleFontSize = htmlFontSize.GetDoubleFontSize(2);
					break;
				case "small":
					characterFormatting.DoubleFontSize = htmlFontSize.GetDoubleFontSize(3);
					break;
				case "medium":
					characterFormatting.DoubleFontSize = htmlFontSize.GetDoubleFontSize(4);
					break;
				case "large":
					characterFormatting.DoubleFontSize = htmlFontSize.GetDoubleFontSize(5);
					break;
				case "x-large":
					characterFormatting.DoubleFontSize = htmlFontSize.GetDoubleFontSize(6);
					break;
				case "xx-large":
					characterFormatting.DoubleFontSize = htmlFontSize.GetDoubleFontSize(7);
					break;
				case "larger":
					characterFormatting.DoubleFontSize = htmlFontSize.GetLargerDoubleFontSize();
					break;
				case "smaller":
					characterFormatting.DoubleFontSize = htmlFontSize.GetSmallerDoubleFontSize();
					break;
				case "inherit":
				case "initial":
					break;
				default:
					LengthValueParser par = new LengthValueParser(value, cssProperties.UnitConverter.ScreenDpi);
					if (!par.Success || !par.IsDigit)
						return false;
					if (par.PointsValue > 0) {
						cssProperties.RelativeProperties.UnitRelativeFontSize = par.RelativeUnit;
						characterFormatting.DoubleFontSize = Math.Max(1, (int)Math.Round(par.PointsValue * 2));
					}
					return true;
			}
			return true;
		}
		static internal void CssFontStyleKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			SetFontStyle(cssProperties, propertiesValue[0]);
		}
		static bool SetFontStyle(CssProperties cssProperties, string value) {
			switch (value.ToLowerInvariant()) {
				case "inherit":
				case "initial":
				case "normal":
					cssProperties.CssCharacterProperties.FontItalic = false;
					break;
				case "italic":
				case "oblique":
					cssProperties.CssCharacterProperties.FontItalic = true;
					break;
				default:
					return false;
			}
			return true;
		}
		static internal void CssFontVariantKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			SetFontVariant(cssProperties, propertiesValue[0]);
		}
		static bool SetFontVariant(CssProperties cssProperties, string value) {
			switch (value.ToLowerInvariant()) {
				case "normal":
				case "small-caps":
				case "initial":
				case "inherit":
					break;
				default:
					return false;
			}
			return true;
		}
		static internal void CssFontWeightKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			SetFontWeight(cssProperties, propertiesValue[0]);
		}
		static bool SetFontWeight(CssProperties cssProperties, string value) {
			int fontWeight;
			if (Int32.TryParse(value, out fontWeight)) {
				switch (fontWeight) {
					case 100:
					case 200:
					case 300:
					case 400:
					case 500:
						cssProperties.CssCharacterProperties.FontBold = false;
						break;
					case 600:
					case 700:
					case 800:
					case 900:
						cssProperties.CssCharacterProperties.FontBold = true;
						break;
				}
			}
			else {
				switch (value.ToLowerInvariant()) {
					case "initial":
					case "inherit":
					case "lighter":
					case "normal":
						cssProperties.CssCharacterProperties.FontBold = false;
						break;
					case "bold":
					case "bolder":
						cssProperties.CssCharacterProperties.FontBold = true;
						break;
					default:
						return false;
				}
			}
			return true;
		}
		static internal void CssHeightKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (String.IsNullOrEmpty(propertiesValue[0]))
				return;
			LengthValueParser par = new LengthValueParser(propertiesValue[0], cssProperties.UnitConverter.ScreenDpi);
			if (!par.IsDigit || par.PointsValue < 0)
				return;
			if (!par.IsRelativeUnit) {
				int value = (int)Math.Round(cssProperties.UnitConverter.PointsToModelUnitsF(par.PointsValue));
				HeightUnitInfo newHeight = new HeightUnitInfo();
				newHeight.Value = value;
				newHeight.Type = HeightUnitType.Minimum;
				cssProperties.RowProperties.Height = newHeight.Clone();
				cssProperties.ImageProperties.Height = new WidthUnitInfo(WidthUnitType.ModelUnits, value);
			}
			else {
				cssProperties.RelativeProperties.RelativeHeight = par.Value;
				cssProperties.RelativeProperties.UnitRelativeHeight = par.Unit;
			}
		}
		static internal void CssHoverKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssLeftKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssLetterSpacingKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssLineHeightKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (String.IsNullOrEmpty(propertiesValue[0]))
				return;
			LengthValueParser par = new LengthValueParser(propertiesValue[0], cssProperties.UnitConverter.ScreenDpi);
			if (!par.IsDigit || par.PointsValue <= 0)
				return;
			if (String.IsNullOrEmpty(par.Unit)) {
				cssProperties.CssParagraphProperties.SetMultipleLineSpacing(par.PointsValue);
			}
			else if (par.Unit == "%") {
				float multiple = par.PointsValue / 100f;
				cssProperties.CssParagraphProperties.SetMultipleLineSpacing(multiple);
			}
			else {
				cssProperties.CssParagraphProperties.LineSpacingType = ParagraphLineSpacing.AtLeast;
				if (par.IsRelativeUnit) {
					cssProperties.RelativeProperties.UnitRelativeLineSpacing = par.RelativeUnit;
					cssProperties.CssParagraphProperties.LineSpacing = par.PointsValue;
				}
				else
					cssProperties.CssParagraphProperties.LineSpacing = cssProperties.UnitConverter.PointsToModelUnitsF(par.PointsValue);
			}
		}
		static internal void CssLinkKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssListStyleKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssListStyleImageKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssListStylePositionKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssListStyleTypeKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			switch (propertiesValue[0].ToUpper(CultureInfo.InvariantCulture)) {
				case "DISC":
					cssProperties.ListLevelProperties.Format = NumberingFormat.Bullet;
					cssProperties.ListLevelProperties.BulletFontName = "Symbol";
					break;
				case "CIRCLE":
					cssProperties.ListLevelProperties.Format = NumberingFormat.Bullet;
					cssProperties.ListLevelProperties.BulletFontName = "Courier New";
					break;
				case "SQUARE":
					cssProperties.ListLevelProperties.Format = NumberingFormat.Bullet;
					cssProperties.ListLevelProperties.BulletFontName = "Wingdings";
					break;
				case "DECIMAL":
					cssProperties.ListLevelProperties.Format = NumberingFormat.Decimal;
					break;
				case "DECIMAL-LEADING-ZERO":
					cssProperties.ListLevelProperties.Format = NumberingFormat.DecimalZero;
					break;
				case "UPPER-LATIN":
					cssProperties.ListLevelProperties.Format = NumberingFormat.UpperLetter;
					break;
				case "LOWER-LATIN":
					cssProperties.ListLevelProperties.Format = NumberingFormat.LowerLetter;
					break;
				case "LOWER-ALPHA":
					cssProperties.ListLevelProperties.Format = NumberingFormat.LowerLetter;
					break;
				case "UPPER-ALPHA":
					cssProperties.ListLevelProperties.Format = NumberingFormat.UpperLetter;
					break;
				case "UPPER-ROMAN":
					cssProperties.ListLevelProperties.Format = NumberingFormat.UpperRoman;
					break;
				case "LOWER-ROMAN":
					cssProperties.ListLevelProperties.Format = NumberingFormat.LowerRoman;
					break;
				case "HEBREW":
					cssProperties.ListLevelProperties.Format = NumberingFormat.Hebrew1;
					break;
				case "HIRAGANA":
					cssProperties.ListLevelProperties.Format = NumberingFormat.AIUEOHiragana;
					break;
				case "KATAKANA":
					cssProperties.ListLevelProperties.Format = NumberingFormat.AIUEOFullWidthHiragana;
					break;
				case "HIRAGANA-IROHA":
					cssProperties.ListLevelProperties.Format = NumberingFormat.Iroha;
					break;
				case "KATAKANA-IROHA":
					cssProperties.ListLevelProperties.Format = NumberingFormat.IrohaFullWidth;
					break;
			}
		}
		static internal void CssMarginKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			string[] properties = propertiesValue[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			switch (properties.Length) {
				case 1:
					CssMarginKeywordCore(cssProperties, properties[0], properties[0], properties[0], properties[0]);
					break;
				case 2:
					CssMarginKeywordCore(cssProperties, properties[0], properties[0], properties[1], properties[1]);
					break;
				case 3:
					CssMarginKeywordCore(cssProperties, properties[2], properties[0], properties[1], properties[1]);
					break;
				case 4:
					CssMarginKeywordCore(cssProperties, properties[2], properties[0], properties[3], properties[1]);
					break;
			}
		}
		static void CssMarginKeywordCore(CssProperties cssProperties, string bottom, string top, string left, string right) {
			CssMarginBottomKeyword(cssProperties, CreateStringList(bottom));
			CssMarginTopKeyword(cssProperties, CreateStringList(top));
			CssMarginLeftKeyword(cssProperties, CreateStringList(left));
			CssMarginRightKeyword(cssProperties, CreateStringList(right));
		}
		static List<string> CreateStringList(string item) {
			List<string> list = new List<string>();
			list.Add(item);
			return list;
		}
		static internal void CssMarginTopKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			float spacingBefore = cssProperties.CssParagraphProperties.SpacingBefore;
			cssProperties.RelativeProperties.UnitRelativeSpacingBefore = SetMarginProperties(propertiesValue, ref spacingBefore, cssProperties.UnitConverter);
			int result = (int)Math.Round(spacingBefore);
			cssProperties.CssParagraphProperties.SpacingBefore = (result > 0)? result : 0;
		}
		static internal void CssMarginBottomKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			float spacingAfter = cssProperties.CssParagraphProperties.SpacingAfter;
			cssProperties.RelativeProperties.UnitRelativeSpacingAfter = SetMarginProperties(propertiesValue, ref spacingAfter, cssProperties.UnitConverter);
			int result = (int)Math.Round(spacingAfter);
			cssProperties.CssParagraphProperties.SpacingAfter = (result > 0) ? result : 0;
		}
		static internal void CssMarginLeftKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			float leftIndent = cssProperties.CssParagraphProperties.LeftIndent;
			cssProperties.RelativeProperties.UnitRelativeLeftIndent = SetMarginProperties(propertiesValue, ref leftIndent, cssProperties.UnitConverter);
			cssProperties.CssParagraphProperties.LeftIndent = (int)Math.Round(leftIndent);
			WidthUnitInfo indent = new WidthUnitInfo(WidthUnitType.ModelUnits, (int)Math.Round(leftIndent));
			cssProperties.TableProperties.Indent = indent;
		}
		static internal void CssMarginRightKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			float rightIndent = cssProperties.CssParagraphProperties.RightIndent;
			cssProperties.RelativeProperties.UnitRelativeRightIndent = SetMarginProperties(propertiesValue, ref rightIndent, cssProperties.UnitConverter);
			cssProperties.CssParagraphProperties.RightIndent = (int)Math.Round(rightIndent);
		}
		static internal void CssMaxHeightKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssMaxWidthKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssMinHeightKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssMinWidthKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssBeforeAutoSpacing(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count > 0 && CompareNoCase(propertiesValue[0], "auto"))
				cssProperties.CssParagraphProperties.BeforeAutoSpacing = true;
		}
		static internal void CssAfterAutoSpacing(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count > 0 && CompareNoCase(propertiesValue[0], "auto"))
				cssProperties.CssParagraphProperties.AfterAutoSpacing = true;
		}
		static internal void CssMsoPagination(CssProperties cssProperties, List<string> propertiesValue) {
			int count = propertiesValue.Count;
			for (int i = 0; i < count; i++) {
				if (CompareNoCase(propertiesValue[i], "lines-together"))
					cssProperties.CssParagraphProperties.KeepLinesTogether = true;
			}
		}
		static internal void CssOpacityKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssOutlineKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssOutlineColorKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssOutlineStyleKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssOutlineWidthKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssOverflowKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssPaddingKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			string[] properties = propertiesValue[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			switch (properties.Length) {
				case 1:
					CssPaddingKeywordCore(cssProperties, properties[0], properties[0], properties[0], properties[0]);
					break;
				case 2:
					CssPaddingKeywordCore(cssProperties, properties[0], properties[0], properties[1], properties[1]);
					break;
				case 3:
					CssPaddingKeywordCore(cssProperties, properties[0], properties[2], properties[1], properties[1]);
					break;
				case 4:
					CssPaddingKeywordCore(cssProperties, properties[2], properties[0], properties[3], properties[1]);
					break;
			}
		}
		static void CssPaddingKeywordCore(CssProperties cssProperties, string bottom, string top, string left, string right) {
			CssPaddingBottomKeyword(cssProperties, CreateStringList(bottom));
			CssPaddingTopKeyword(cssProperties, CreateStringList(top));
			CssPaddingLeftKeyword(cssProperties, CreateStringList(left));
			CssPaddingRightKeyword(cssProperties, CreateStringList(right));
		}
		static internal void CssPaddingBottomKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (String.IsNullOrEmpty(propertiesValue[0]))
				return;
			LengthValueParser par = new LengthValueParser(propertiesValue[0], cssProperties.UnitConverter.ScreenDpi);
			if (par.IsDigit && par.PointsValue >= 0) {
				if (!par.IsRelativeUnit) {
					int value = (int)Math.Round(cssProperties.UnitConverter.PointsToModelUnitsF(par.PointsValue));
					cssProperties.CellProperties.CellMargins.Bottom.Value = value;
					cssProperties.CellProperties.CellMargins.Bottom.Type = WidthUnitType.ModelUnits;
				}
			}
		}
		static internal void CssPaddingLeftKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (String.IsNullOrEmpty(propertiesValue[0]))
				return;
			LengthValueParser par = new LengthValueParser(propertiesValue[0], cssProperties.UnitConverter.ScreenDpi);
			if (par.IsDigit && par.PointsValue >= 0) {
				if (!par.IsRelativeUnit) {
					int value = (int)Math.Round(cssProperties.UnitConverter.PointsToModelUnitsF(par.PointsValue));
					cssProperties.CellProperties.CellMargins.Left.Value = value;
					cssProperties.CellProperties.CellMargins.Left.Type = WidthUnitType.ModelUnits;
				}
			}
		}
		static internal void CssPaddingRightKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (String.IsNullOrEmpty(propertiesValue[0]))
				return;
			LengthValueParser par = new LengthValueParser(propertiesValue[0], cssProperties.UnitConverter.ScreenDpi);
			if (par.IsDigit && par.PointsValue >= 0) {
				if (!par.IsRelativeUnit) {
					int value = (int)Math.Round(cssProperties.UnitConverter.PointsToModelUnitsF(par.PointsValue));
					cssProperties.CellProperties.CellMargins.Right.Value = value;
					cssProperties.CellProperties.CellMargins.Right.Type = WidthUnitType.ModelUnits;
				}
			}
		}
		static internal void CssPaddingTopKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (String.IsNullOrEmpty(propertiesValue[0]))
				return;
			LengthValueParser par = new LengthValueParser(propertiesValue[0], cssProperties.UnitConverter.ScreenDpi);
			if (par.IsDigit && par.PointsValue >= 0) {
				if (!par.IsRelativeUnit) {
					int value = (int)Math.Round(cssProperties.UnitConverter.PointsToModelUnitsF(par.PointsValue));
					cssProperties.CellProperties.CellMargins.Top.Value = value;
					cssProperties.CellProperties.CellMargins.Top.Type = WidthUnitType.ModelUnits;
				}
			}
		}
		static internal void CssPageBreakAfterKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count <= 0)
				return;
			if (CompareNoCase(propertiesValue[0], "avoid"))
				cssProperties.CssParagraphProperties.KeepWithNext = true;
		}
		static internal void CssPageBreakBeforeKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count <= 0)
				return;
			if (CompareNoCase(propertiesValue[0], "always"))
				cssProperties.CssParagraphProperties.PageBreakBefore = true;
		}
		static internal void CssPageBreakInsideKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count <= 0)
				return;
			if (CompareNoCase(propertiesValue[0], "avoid"))
				cssProperties.CssParagraphProperties.KeepLinesTogether = true;
		}
		static internal void CssPositionKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssQuotesKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssRightKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssTableLayoutKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssTextAlignKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			switch (propertiesValue[0].ToUpper(CultureInfo.InvariantCulture)) {
				case "LEFT":
					cssProperties.CssParagraphProperties.Alignment = ParagraphAlignment.Left;
					break;
				case "RIGHT":
					cssProperties.CssParagraphProperties.Alignment = ParagraphAlignment.Right;
					break;
				case "CENTER":
					cssProperties.CssParagraphProperties.Alignment = ParagraphAlignment.Center;
					break;
				case "JUSTIFY":
					cssProperties.CssParagraphProperties.Alignment = ParagraphAlignment.Justify;
					break;
			}
		}
		static internal void CssTextDecorationKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			string[] properties = propertiesValue[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			int count = properties.Length;
			for (int i = 0; i < count; i++) {
				switch (properties[i].ToUpper(CultureInfo.InvariantCulture)) {
					case "LINE-THROUGH":
						cssProperties.CssCharacterProperties.FontStrikeoutType = StrikeoutType.Single;
						break;
					case "UNDERLINE":
						cssProperties.CssCharacterProperties.FontUnderlineType = UnderlineType.Single;
						break;
					case "NONE":
						cssProperties.CssCharacterProperties.FontUnderlineType = UnderlineType.None;
						cssProperties.CssCharacterProperties.FontStrikeoutType = StrikeoutType.None;
						break;
				}
			}
		}
		static internal void CssTextIndentKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (String.IsNullOrEmpty(propertiesValue[0]))
				return;
			LengthValueParser par = new LengthValueParser(propertiesValue[0], cssProperties.UnitConverter.ScreenDpi);
			if (!par.IsDigit)
				return;
			float indent = SetLineIndentType(cssProperties, par.PointsValue);
			if (par.IsRelativeUnit) {
				cssProperties.RelativeProperties.UnitRelativeFirstLineIndent = par.RelativeUnit;
				cssProperties.CssParagraphProperties.FirstLineIndent = (int)Math.Round(indent);
			}
			else
				cssProperties.CssParagraphProperties.FirstLineIndent = (int)Math.Round(cssProperties.UnitConverter.PointsToModelUnitsF(indent));
		}
		static string SetMarginProperties(List<string> propertiesValue, ref float marginValue, DocumentModelUnitConverter unitConverter) {
			if (String.IsNullOrEmpty(propertiesValue[0]))
				return String.Empty;
			LengthValueParser par = new LengthValueParser(propertiesValue[0], unitConverter.ScreenDpi);
			if (!par.IsDigit ) 
				return String.Empty;
			if (par.IsRelativeUnit) {
				marginValue = par.PointsValue;
				return par.RelativeUnit;
			}
			else
				marginValue = unitConverter.PointsToModelUnitsF(par.PointsValue);
			return String.Empty;
		}
		private static float SetLineIndentType(CssProperties cssProperties, float indent) {
			if (indent < 0) {
				cssProperties.CssParagraphProperties.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
				indent = -indent;
			}
			else
				cssProperties.CssParagraphProperties.FirstLineIndentType = ParagraphFirstLineIndent.Indented;
			return indent;
		}
		static internal void CssTextTransformKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count == 1 && StringExtensions.CompareInvariantCultureIgnoreCase(propertiesValue[0], "uppercase") == 0)
				cssProperties.CssCharacterProperties.AllCaps = true;
		}
		static internal void CssTopKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssUnicodeBidiKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssVerticalAlignKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count != 1)
				return;
			VerticalAlignment alignment = TagBase.ReadVerticalAlignment(propertiesValue[0]);
			cssProperties.CellProperties.VerticalAlignment = alignment;
			cssProperties.RowProperties.VerticalAlignment = alignment;
		}
		static internal void CssVisibilityKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssVisitedKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		static internal void CssWhiteSpaceKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (propertiesValue.Count != 1)
				return;
			string readedValue = propertiesValue[0];
			if (readedValue == "nowrap")
				cssProperties.CellProperties.NoWrap = true;
			else if (readedValue == "normal")
				cssProperties.CellProperties.NoWrap = false;
		}
		static internal void CssWidthKeyword(CssProperties cssProperties, List<string> propertiesValue) {
			if (String.IsNullOrEmpty(propertiesValue[0]))
				return;
			int separatorPos = propertiesValue[0].LastIndexOf('!');
			if (separatorPos > 1)
				propertiesValue[0] = propertiesValue[0].Substring(1, separatorPos - 1);
			LengthValueParser par = new LengthValueParser(propertiesValue[0], cssProperties.UnitConverter.ScreenDpi);
			if (par.IsDigit && par.PointsValue > 0) {
				if (!par.IsRelativeUnit) {
					int value = (int)Math.Round(cssProperties.UnitConverter.PointsToModelUnitsF(par.PointsValue));
					cssProperties.CellProperties.PreferredWidth.Value = value;
					cssProperties.CellProperties.PreferredWidth.Type = WidthUnitType.ModelUnits;
					cssProperties.TableProperties.Width = cssProperties.CellProperties.PreferredWidth.Info.Clone();
					cssProperties.ImageProperties.Width = new WidthUnitInfo(WidthUnitType.ModelUnits, value);
				}
				else {
					cssProperties.RelativeProperties.RelativeWidth = par.Value;
					cssProperties.RelativeProperties.UnitRelativeWidth = par.Unit;
				}
			}
		}
		static internal void CssWordSpacingKeyword(CssProperties cssProperties, List<string> propertiesValue) {
		}
		#endregion
		public CssParser(DocumentModel documentModel) {
			this.propertiesValue = new StringBuilder();
			this.properties = new StringBuilder();
			this.propertiesKeyword = new StringBuilder();
			this.documentModel = documentModel;
			this.selectors = new List<Selector>();
			this.cssElementCollection = new CssElementCollection();
			this.state = CssParserState.ReadSelector;
			this.parentState = new CssParserState();
			this.propertiesValues = new List<string>();
			this.cssProperties = new CssProperties(documentModel.MainPieceTable);
			this.selectorsText = String.Empty;
		}
		protected internal virtual CssKeywordTranslatorTable CssKeywordTable { get { return acssKeywordTable; } }
		protected internal bool IsUpdateLocked { get { return cssProperties.CssCharacterProperties.IsUpdateLocked && cssProperties.CssParagraphProperties.IsUpdateLocked && cssProperties.CellProperties.IsUpdateLocked; } }
		protected internal virtual CssElementCollection Parse(TextReader reader) {
			int intChar = reader.Peek();
			if (intChar < 0)
				return null;
			ParseCssElements(reader);
			return cssElementCollection;
		}
		void BeginUpdate() {
			cssProperties.CssCharacterProperties.BeginUpdate();
			cssProperties.CssParagraphProperties.BeginUpdate();
			cssProperties.CellProperties.BeginUpdate();
		}
		void EndUpdate() {
			cssProperties.CellProperties.EndUpdate();
			cssProperties.CssParagraphProperties.EndUpdate();
			cssProperties.CssCharacterProperties.EndUpdate();
		}
		protected internal virtual CssElementCollection ParseAttribute(TextReader reader) {
			int intChar = reader.Peek();
			if (intChar < 0)
				return null;
			BeginUpdate();
			SetCssParserState(CssParserState.ReadPropertiesName);
			ParseCssElements(reader);
			EndProperty();
			AddCssElement(new Selector());
			return cssElementCollection;
		}
		protected internal CssElementCollection ParseCssElements(TextReader reader) {
			for (; ; ) {
				int intChar = reader.Read();
				if (intChar < 0)
					break;
				char ch = (char)intChar;
				switch (state) {
					case CssParserState.ReadSelector:
						ParseSelectors(ch);
						break;
					case CssParserState.ReadPropertiesName:
						ReadPropertiesName(ch);
						break;
					case CssParserState.WaitColonSymbol:
						WaitColonSymbol(ch);
						break;
					case CssParserState.WaitPropertiesValue:
						WaitPropertiesValue(ch);
						break;
					case CssParserState.ReadPropertiesValue:
						ReadPropertiesValue(ch);
						break;
					case CssParserState.ReadPropertiesValueInQuotes:
						ReadPropertiesValueInQuotes(ch);
						break;
					case CssParserState.ReadPropertiesValueInApostrophe:
						ReadPropertiesValueInApostrophe(ch);
						break;
					case CssParserState.ReadPropertiesValueInParentheses:
						ReadPropertiesValueInParentheses(ch);
						break;
					case CssParserState.WaitPropertiesKeyword:
						WaitPropertiesKeyword(ch);
						break;
					case CssParserState.ReadPropertiesKeyword:
						ReadPropertiesKeyword(ch);
						break;
					case CssParserState.WaitStartComment:
						WaitStartComment(ch);
						break;
					case CssParserState.ReadComment:
						ReadComment(ch);
						break;
					case CssParserState.WaitEndComment:
						WaitEndComment(ch);
						break;
					case CssParserState.SkipNestedProperties:
						SkipNestedProperties(ch);
						break;
				}
			}
			if (IsUpdateLocked)
				EndUpdate();
			return cssElementCollection;
		}
		protected internal virtual void SetCssParserState(CssParserState newState) {
			parentState = state;
			state = newState;
		}
		protected internal void ParseSelectors(char ch) {
			if (ch == '{') {
				SelectorParser selectorParser = new SelectorParser(selectorsText);
				selectorsText = String.Empty;
				selectors = selectorParser.Parse();
				cssProperties.Reset(documentModel);
				BeginUpdate();
				SetCssParserState(CssParserState.ReadPropertiesName);
				return;
			}
			else if (ch == '/')
				SetCssParserState(CssParserState.WaitStartComment);
			else
				selectorsText += ch;
		}
		protected internal void ReadPropertiesName(char ch) {
			if (CheckGeneralEvent(ch))
				return;
			if (Char.IsWhiteSpace(ch)) {
				if (properties.Length > 0)
					SetCssParserState(CssParserState.WaitColonSymbol);
				return;
			}
			if (ch == ':')
				SetCssParserState(CssParserState.WaitPropertiesValue);
			else
				properties.Append(ch);
		}
		protected internal void WaitPropertiesValue(char ch) {
			if (CheckGeneralEvent(ch) || Char.IsWhiteSpace(ch))
				return;
			switch (ch) {
				case '\'':
					SetCssParserState(CssParserState.ReadPropertiesValueInApostrophe);
					break;
				case '\"':
					SetCssParserState(CssParserState.ReadPropertiesValueInQuotes);
					break;
				default:
					propertiesValue.Append(ch);
					SetCssParserState(CssParserState.ReadPropertiesValue);
					break;
			}
		}
		protected internal void ReadPropertiesValue(char ch) {
			if (CheckGeneralEvent(ch))
				return;
			if (ch == '(') {
				propertiesValue.Append(ch);
				SetCssParserState(CssParserState.ReadPropertiesValueInParentheses);
				return;
			}
			if (ch == ',') {
				propertiesValues.Add(propertiesValue.ToString());
				propertiesValue.Length = 0;
				SetCssParserState(CssParserState.WaitPropertiesValue);
				return;
			}
			if(ch == '!') {
				SetCssParserState(CssParserState.WaitPropertiesKeyword);
				return;
			}
			propertiesValue.Append(ch);
		}
		protected internal void ReadPropertiesValueInQuotes(char ch) {
			if (ch == '\"')
				SetCssParserState(CssParserState.ReadPropertiesValue);
			else
				propertiesValue.Append(ch);
		}
		protected internal void ReadPropertiesValueInApostrophe(char ch) {
			if (ch == '\'')
				SetCssParserState(CssParserState.ReadPropertiesValue);
			else
				propertiesValue.Append(ch);
		}
		protected internal void ReadPropertiesValueInParentheses(char ch) {
			if (ch == ')')
				SetCssParserState(CssParserState.ReadPropertiesValue);
			propertiesValue.Append(ch);
		}
		protected internal void WaitPropertiesKeyword(char ch) {
			if(CheckGeneralEvent(ch) || Char.IsWhiteSpace(ch))
				return;
			propertiesKeyword.Clear();
			SetCssParserState(CssParserState.ReadPropertiesKeyword);
			ReadPropertiesKeyword(ch);
		}
		protected internal void ReadPropertiesKeyword(char ch) {
			if(CheckGeneralEvent(ch))
				return;
			propertiesKeyword.Append(ch);
		}
		protected internal void WaitColonSymbol(char ch) {
			if (ch == ':') {
				SetCssParserState(CssParserState.WaitPropertiesValue);
				return;
			}
			if (CheckGeneralEvent(ch) || Char.IsWhiteSpace(ch))
				return;
		}
		protected internal bool CheckGeneralEvent(char ch) {
			switch (ch) {
				case '/':
					SetCssParserState(CssParserState.WaitStartComment);
					return true;
				case '}':
					EndCssElement();
					return true;
				case '{':
					SetCssParserState(CssParserState.SkipNestedProperties);
					return true;
				case ';':
					EndProperty();
					SetCssParserState(CssParserState.ReadPropertiesName);
					return true;
			}
			return false;
		}
		protected internal void SkipNestedProperties(char ch) {
			if (ch == '}') {
				if (bracketCount == 0) {
					ClearProperties();
					SetCssParserState(CssParserState.ReadPropertiesName);
				}
				else
					bracketCount--;
			}
			if (ch == '{')
				bracketCount++;
		}
		protected internal virtual void EndProperty() {
			if (propertiesValue.Length > 0) {
				propertiesValues.Add(propertiesValue.ToString().Trim());
			}
			BeginUpdate();
			if (propertiesValues.Count > 0)
				FindKeywordInTable();
			EndUpdate();
			ClearProperties();
		}
		protected internal void EndCssElement() {
			EndProperty();
			AddCssElements();
			EndUpdate();
			SetCssParserState(CssParserState.ReadSelector);
		}
		protected internal void ClearProperties() {
			properties.Length = 0;
			propertiesValue.Length = 0;
			propertiesValues.Clear();
			propertiesKeyword.Clear();
		}
		protected internal void WaitStartComment(char ch) {
			if (ch == '*')
				state = CssParserState.ReadComment;
			else {
				CssParserState parentState1 = parentState;
				SetCssParserState(parentState);
				if (parentState1 == CssParserState.ReadPropertiesValue) {
					propertiesValue.Append('/');
					ReadPropertiesValue(ch);
				}
			}
		}
		protected internal void WaitEndComment(char ch) {
			if (ch == '/')
				SetCssParserState(parentState);
			else
				state = CssParserState.ReadComment;
		}
		protected internal void ReadComment(char ch) {
			if (ch == '*')
				state = CssParserState.WaitEndComment;
		}
		protected internal virtual void FindKeywordInTable() {
			CssPropertiesTranslateHandler translator = null;
			string propertiesName = properties.ToString().ToUpper(CultureInfo.InvariantCulture);
			if (propertiesName != null)
				CssKeywordTable.TryGetValue(propertiesName, out translator);
			if (translator != null)
				translator(this.cssProperties, propertiesValues);
		}
		protected internal void AddCssElements() {
			for (int i = 0; i < selectors.Count; i++) {
				if (selectors[i].Elements.Count != 0)
					AddCssElement(selectors[i]);
			}
			selectors.Clear();
		}
		protected internal void AddCssElement(Selector selector) {
			if (cssProperties.IsDefaultProperties())
				return;
			CssElement cssElement = new CssElement(documentModel.MainPieceTable, cssElementCollection.Count);
			cssElement.Selector = selector;
			cssElement.Properties.CopyFrom(cssProperties);
			cssElementCollection.Add(cssElement);
		}
	}
	#endregion
}
