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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if !SL
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Html {
	#region HtmlParagraphAlignment
	public class HtmlParagraphAlignment {
		#region Fields
		ParagraphAlignment alignmentValue;
		bool useAlignment;
		#endregion
		public HtmlParagraphAlignment() {
			this.useAlignment = false;
			this.alignmentValue = ParagraphAlignment.Left;
		}
		#region Priperties
		public ParagraphAlignment AlignmentValue {
			get { return alignmentValue; }
			set {
				alignmentValue = value;
				useAlignment = true;
			}
		}
		public bool UseAlignment { get { return useAlignment; } }
		#endregion
		public void ResetDefaultAlignment() {
			alignmentValue = ParagraphAlignment.Left;
			useAlignment = false;
		}
		public void CopyFrom(HtmlParagraphAlignment value) {
			this.useAlignment = value.useAlignment;
			this.alignmentValue = value.alignmentValue;
		}
	}
	#endregion
	#region HtmlListLevelProperties
	public class HtmlListLevelProperties {
		#region Field
		int start;
		bool levelPositionIsOutside;
		NumberingFormat format;
		string bulletFontName;
		bool useStart;
		bool useFormat;
		bool useLevelPositionIsOutside;
		bool useBulletFontName;
		#endregion
		public HtmlListLevelProperties() {
			this.start = 1;
			this.levelPositionIsOutside = false;
			this.format = NumberingFormat.Decimal;
			this.bulletFontName = String.Empty;
		}
		#region Proprties
		#region UseBulletFontName
		public bool UseBulletFontName { get { return useBulletFontName; } set { useBulletFontName = value; } }
		#endregion
		#region Start
		public bool UseStart { get { return useStart; } }
		public int Start {
			get { return start; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Start", value);
				start = value;
				useStart = true;
			}
		}
		#endregion
		#region Format
		public bool UseFormat { get { return useFormat; } set { useFormat = value; } }
		public NumberingFormat Format {
			get { return format; }
			set {
				format = value;
				useFormat = true;
			}
		}
		#endregion
		#region LevelPositionIsOutside
		public bool LevelPositionIsOutside {
			get { return levelPositionIsOutside; }
			set {
				levelPositionIsOutside = value;
				useLevelPositionIsOutside = true;
			}
		}
		#endregion
		#region BulletFontName
		public string BulletFontName {
			get {
				return bulletFontName;
			}
			set {
				bulletFontName = value;
				useBulletFontName = true;
			}
		}
		#endregion
		#endregion
		protected internal void CopyFrom(HtmlListLevelProperties value) {
			this.start = value.Start;
			this.levelPositionIsOutside = value.LevelPositionIsOutside;
			this.format = value.Format;
			this.bulletFontName = value.bulletFontName;
			this.useFormat = value.useFormat;
			this.useLevelPositionIsOutside = value.useLevelPositionIsOutside;
			this.useStart = value.useStart;
			this.useBulletFontName = value.useBulletFontName;
		}
		protected internal HtmlListLevelProperties MergeWith(HtmlListLevelProperties properties) {
			HtmlListLevelProperties mergePreoperties = new HtmlListLevelProperties();
			if (this.useFormat)
				mergePreoperties.Format = this.Format;
			else if (properties.useFormat)
				mergePreoperties.Format = properties.Format;
			if (this.useLevelPositionIsOutside)
				mergePreoperties.LevelPositionIsOutside = this.LevelPositionIsOutside;
			else if (properties.useLevelPositionIsOutside)
				mergePreoperties.LevelPositionIsOutside = properties.LevelPositionIsOutside;
			if (this.useStart)
				mergePreoperties.Start = this.Start;
			else if (properties.useStart)
				mergePreoperties.Start = properties.Start;
			if (this.useBulletFontName)
				mergePreoperties.BulletFontName = this.BulletFontName;
			else if (properties.useBulletFontName)
				mergePreoperties.BulletFontName = properties.BulletFontName;
			return mergePreoperties;
		}
		protected internal ListLevel ApplayPropertiesToListLevel(ListLevel level) {
			if (this.useBulletFontName)
				level.CharacterProperties.FontName = BulletFontName;
			if (this.useFormat)
				level.ListLevelProperties.Format = Format;
			if (this.useStart)
				level.ListLevelProperties.Start = Start;
			return level;
		}
		protected internal bool IsDefaultProperties() {
			if (!useFormat && !useFormat && !useLevelPositionIsOutside)
				return true;
			return false;
		}
	}
	#endregion
	#region HtmlBorderProperty
	public class HtmlBorderProperty {
		int width;
		Color color;
		BorderLineStyle lineStyle;
		bool useWidth;
		bool useColor;
		bool useLineStyle;
		public HtmlBorderProperty() {
			this.color = DXColor.Empty;
		}
		public int Width { get { return width; } set { width = value; useWidth = true; } }
		public Color Color { get { return color; } set { color = value; useColor = true; } }
		public BorderLineStyle LineStyle { get { return lineStyle; } set { lineStyle = value; useLineStyle = true; } }
		public bool UseColor { get { return useColor; } }
		public bool UseLineStyle { get { return useLineStyle; } }
		public bool UseWidth { get { return useWidth; } }
		public void CopyFrom(HtmlBorderProperty source) {
			this.width = source.width;
			this.color = source.color;
			this.lineStyle = source.lineStyle;
			this.useWidth = source.useWidth;
			this.useColor = source.useColor;
			this.useLineStyle = source.useLineStyle;
		}
		public void Apply(BorderBase border) {
			if (UseWidth)
				border.Width = this.Width;
			if (UseColor)
				border.Color = this.Color;
			if (UseLineStyle) {
				if (!UseWidth || Width > 0)
					border.Style = this.LineStyle;
				else
					border.Style = BorderLineStyle.None;
			}
		}
		public bool IsDefaultProperties() {
			return !UseWidth && !UseColor && !UseLineStyle;
		}
		public HtmlBorderProperty MergeWith(HtmlBorderProperty other) {
			HtmlBorderProperty result = new HtmlBorderProperty();
			if (UseLineStyle)
				result.LineStyle = this.LineStyle;
			else if (other.UseLineStyle)
				result.LineStyle = other.LineStyle;
			if (UseColor)
				result.Color = this.Color;
			else if (other.UseColor)
				result.Color = other.Color;
			if (UseWidth)
				result.Width = this.Width;
			else if (other.UseWidth)
				result.Width = other.Width;
			return result;
		}
	}
	#endregion
	#region HtmlBordersProperties
	public class HtmlBordersProperties {
		HtmlBorderProperty topBorder;
		HtmlBorderProperty leftBorder;
		HtmlBorderProperty bottomBorder;
		HtmlBorderProperty rightBorder;
		public HtmlBordersProperties() {
			this.topBorder = new HtmlBorderProperty();
			this.leftBorder = new HtmlBorderProperty();
			this.bottomBorder = new HtmlBorderProperty();
			this.rightBorder = new HtmlBorderProperty();
		}
		public HtmlBorderProperty TopBorder { get { return topBorder; } set { topBorder = value; } }
		public HtmlBorderProperty LeftBorder { get { return leftBorder; } set { leftBorder = value; } }
		public HtmlBorderProperty BottomBorder { get { return bottomBorder; } set { bottomBorder = value; } }
		public HtmlBorderProperty RightBorder { get { return rightBorder; } set { rightBorder = value; } }
		public void CopyFrom(HtmlBordersProperties source) {
			TopBorder.CopyFrom(source.TopBorder);
			LeftBorder.CopyFrom(source.LeftBorder);
			RightBorder.CopyFrom(source.RightBorder);
			BottomBorder.CopyFrom(source.BottomBorder);
		}
		internal bool IsDefaultProperties() {
			return TopBorder.IsDefaultProperties() && LeftBorder.IsDefaultProperties() && RightBorder.IsDefaultProperties() && BottomBorder.IsDefaultProperties();
		}
		public HtmlBordersProperties MergeWith(HtmlBordersProperties other) {
			HtmlBordersProperties result = new HtmlBordersProperties();
			result.TopBorder = TopBorder.MergeWith(other.TopBorder);
			result.LeftBorder = LeftBorder.MergeWith(other.LeftBorder);
			result.RightBorder = RightBorder.MergeWith(other.RightBorder);
			result.BottomBorder = BottomBorder.MergeWith(other.BottomBorder);
			return result;
		}
	}
	#endregion
	public enum BorderCollapse {
		Separate,
		Collapse,
	}
	#region HtmlTableProperties
	public class HtmlTableProperties {
		#region Field
		WidthUnitInfo width;
		WidthUnitInfo cellMargings;
		WidthUnitInfo cellSpacing;
		Color backgroundColor;
		bool setInnerBorders;
		bool useWidth;
		bool useCellMargings;
		bool useCellSpacing;
		bool useBackgroundColor;
		HtmlBordersProperties bordersProperties;
		TableRowAlignment tableAlignment;
		bool useTableAlignment;
		WidthUnitInfo indent;
		bool useIndent;
		BorderCollapse borderCollapse;
		bool useBorderCollapse;
		#endregion
		public HtmlTableProperties() {
			width = new WidthUnitInfo();
			cellMargings = new WidthUnitInfo();
			cellSpacing = new WidthUnitInfo();
			indent = new WidthUnitInfo();
			backgroundColor = DXColor.Empty;
			setInnerBorders = false;
			this.bordersProperties = new HtmlBordersProperties();
			this.tableAlignment = TableRowAlignment.Left;
		}
		#region Proprties
		#region Width
		public WidthUnitInfo Width {
			get { return width; }
			set {
				if (value.Value < 0)
					Exceptions.ThrowArgumentException("Width", value.Value);
				width = value;
				useWidth = true;
			}
		}
		#endregion
		#region CellMargings
		public WidthUnitInfo CellMarging {
			get { return cellMargings; }
			set {
				if (value.Value < 0)
					Exceptions.ThrowArgumentException("CellMargings", value.Value);
				cellMargings = value;
				useCellMargings = true;
			}
		}
		#endregion
		#region CellSpacing
		public WidthUnitInfo CellSpacing {
			get { return cellSpacing; }
			set {
				if (value.Value < 0)
					Exceptions.ThrowArgumentException("CellSpacing", value.Value);
				cellSpacing = value;
				useCellSpacing = true;
			}
		}
		#endregion
		public WidthUnitInfo Indent {
			get { return indent; }
			set {
				indent = value;
				useIndent = true;
			}
		}
		#region BordersProperties
		public HtmlBordersProperties BordersProperties { get { return bordersProperties; } set { bordersProperties = value; } }
		#endregion
		#region BackgroundColor
		public Color BackgroundColor {
			get { return backgroundColor; }
			set {
				backgroundColor = value;
				useBackgroundColor = true;
			}
		}
		public bool UseBackgroundColor { get { return useBackgroundColor; } }
		#endregion
		#region TableAlignment
		public TableRowAlignment TableAlignment {
			get { return tableAlignment; }
			set {
				tableAlignment = value;
				useTableAlignment = true;
			}
		}
		#endregion
		#region BorderCollapse
		public BorderCollapse BorderCollapse {
			get { return borderCollapse; }
			set {
				borderCollapse = value;
				useBorderCollapse = true;
			}
		}
		#endregion
		protected internal bool UseCellSpacing { get { return useCellSpacing; } set { useCellSpacing = value; } }
		#endregion
		public bool SetInnerBorders {
			get { return setInnerBorders; }
			set { setInnerBorders = value; }
		}
		protected internal void CopyFrom(HtmlTableProperties value) {
			this.width = value.Width;
			this.cellMargings = value.CellMarging;
			this.cellSpacing = value.CellSpacing;
			this.borderCollapse = value.BorderCollapse;
			this.bordersProperties.CopyFrom(value.BordersProperties);
			this.backgroundColor = value.backgroundColor;
			this.tableAlignment = value.TableAlignment;
			this.useWidth = value.useWidth;
			this.useCellMargings = value.useCellMargings;
			this.useCellSpacing = value.useCellSpacing & (value.cellSpacing.Value > 0 || value.cellSpacing.Type == WidthUnitType.ModelUnits);
			this.useBorderCollapse = value.useBorderCollapse;
			this.useBackgroundColor = value.useBackgroundColor;
			this.setInnerBorders = value.setInnerBorders;
			this.useTableAlignment = value.useTableAlignment;
			this.indent = value.indent;
			this.useIndent = value.useIndent;
		}
		protected internal HtmlTableProperties MergeWith(HtmlTableProperties properties) {
			HtmlTableProperties mergeProperties = new HtmlTableProperties();
			if (this.useWidth)
				mergeProperties.Width = this.Width;
			else if (properties.useWidth)
				mergeProperties.Width = properties.Width;
			if (this.useCellMargings)
				mergeProperties.CellMarging = this.CellMarging;
			else if (properties.useCellMargings)
				mergeProperties.CellMarging = properties.CellMarging;
			if (this.useCellSpacing)
				mergeProperties.CellSpacing = this.CellSpacing;
			else if (properties.useCellSpacing)
				mergeProperties.CellSpacing = properties.cellSpacing;
			if (this.useBorderCollapse)
				mergeProperties.BorderCollapse = this.BorderCollapse;
			else if (properties.useCellSpacing)
				mergeProperties.BorderCollapse = properties.BorderCollapse;
			mergeProperties.BordersProperties = BordersProperties.MergeWith(properties.BordersProperties);
			if (this.useBackgroundColor)
				mergeProperties.BackgroundColor = this.BackgroundColor;
			else if (properties.useBackgroundColor)
				mergeProperties.BackgroundColor = properties.BackgroundColor;
			if (this.setInnerBorders || properties.SetInnerBorders)
				mergeProperties.SetInnerBorders = true;
			if (this.useTableAlignment)
				mergeProperties.TableAlignment = this.TableAlignment;
			else if (properties.useTableAlignment)
				mergeProperties.TableAlignment = properties.tableAlignment;
			if (this.useIndent)
				mergeProperties.Indent = this.Indent;
			else if (properties.useIndent)
				mergeProperties.Indent = properties.Indent;
			return mergeProperties;
		}
		protected internal void ApplyPropertiesToTable(TableProperties properties) {
			if (this.useWidth)
				properties.PreferredWidth.CopyFrom(Width);
			else {
				WidthUnitInfo autoWidth = new WidthUnitInfo();
				autoWidth.Type = WidthUnitType.Auto;
				autoWidth.Value = 0;
				properties.PreferredWidth.CopyFrom(autoWidth);
			}
			if (this.useCellMargings) {
				properties.CellMargins.Top.CopyFrom(CellMarging);
				properties.CellMargins.Left.CopyFrom(CellMarging);
				properties.CellMargins.Bottom.CopyFrom(CellMarging);
				properties.CellMargins.Right.CopyFrom(CellMarging);
			}
			bool spacingBetweenCellsIsForbidden = this.useBorderCollapse && BorderCollapse == Html.BorderCollapse.Collapse;
			if (this.useCellSpacing && !spacingBetweenCellsIsForbidden)
				properties.CellSpacing.CopyFrom(CellSpacing);
			TableBorders borders = properties.Borders;
			bordersProperties.TopBorder.Apply(borders.TopBorder);
			bordersProperties.LeftBorder.Apply(borders.LeftBorder);
			bordersProperties.RightBorder.Apply(borders.RightBorder);
			bordersProperties.BottomBorder.Apply(borders.BottomBorder);
			bordersProperties.LeftBorder.Apply(borders.InsideVerticalBorder);
			bordersProperties.TopBorder.Apply(borders.InsideHorizontalBorder);
			if (this.useBackgroundColor) {
				properties.BackgroundColor = BackgroundColor;
			}
			if (!SetInnerBorders) {
				BorderInfo inners = new BorderInfo();
				inners.Width = 0;
				inners.Style = BorderLineStyle.None;
				properties.Borders.InsideHorizontalBorder.CopyFrom(inners);
				properties.Borders.InsideVerticalBorder.CopyFrom(inners);
			}
			if (useTableAlignment)
				properties.TableAlignment = TableAlignment;
			if (useIndent)
				properties.TableIndent.CopyFrom(Indent);
		}
		public void ApplyBackgroundColorToCell(TableCellProperties properties) {
			if (this.useBackgroundColor)
				properties.BackgroundColor = BackgroundColor;
		}
		public void ApplyPropertiesToCharacter(CharacterFormattingBase properties) {
			if (this.useBackgroundColor)
				properties.BackColor = BackgroundColor;
		}
		protected internal bool IsDefaultProperties() {
			if (!useWidth && !useCellMargings && !useCellSpacing
				&& bordersProperties.IsDefaultProperties()
				&& !useTableAlignment && !useIndent && !useBorderCollapse)
				return true;
			return false;
		}
	}
	#endregion
	#region HtmlTableRowProperties
	public class HtmlTableRowProperties {
		#region Field
		HeightUnitInfo height;
		bool useHeight;
		Color backgroundColor;
		bool useBackgroundColor;
		VerticalAlignment verticalAlignment;
		bool useVerticalAlignment;
		#endregion
		public HtmlTableRowProperties() {
			this.height = new HeightUnitInfo();
		}
		#region Proprties
		#region Height
		public HeightUnitInfo Height {
			get { return height; }
			set {
				if (value.Value < 0)
					Exceptions.ThrowArgumentException("Height", value.Value);
				height = value;
				useHeight = true;
			}
		}
		public bool UseHeight { get { return useHeight; } }
		#endregion
		#region BackgroundColor
		public Color BackgroundColor {
			get { return backgroundColor; }
			set {
				backgroundColor = value;
				useBackgroundColor = true;
			}
		}
		#endregion
		public VerticalAlignment VerticalAlignment {
			get { return verticalAlignment; }
			set {
				verticalAlignment = value;
				useVerticalAlignment = true;
			}
		}
		#endregion
		protected internal void CopyFrom(HtmlTableRowProperties value) {
			this.height = value.Height;
			this.useHeight = value.useHeight;
			this.backgroundColor = value.BackgroundColor;
			this.useBackgroundColor = value.useBackgroundColor;
			this.verticalAlignment = value.VerticalAlignment;
			this.useVerticalAlignment = value.useVerticalAlignment;
		}
		protected internal HtmlTableRowProperties MergeWith(HtmlTableRowProperties properties) {
			HtmlTableRowProperties mergeProperties = new HtmlTableRowProperties();
			if (this.useHeight)
				mergeProperties.Height = this.Height;
			else if (properties.useHeight)
				mergeProperties.Height = properties.Height;
			if (this.useBackgroundColor)
				mergeProperties.BackgroundColor = this.BackgroundColor;
			else if (properties.useBackgroundColor)
				mergeProperties.BackgroundColor = properties.BackgroundColor;
			if (this.useVerticalAlignment)
				mergeProperties.VerticalAlignment = this.VerticalAlignment;
			else if (properties.useVerticalAlignment)
				mergeProperties.VerticalAlignment = properties.VerticalAlignment;
			return mergeProperties;
		}
		protected internal void ApplyPropertiesToRow(TableRowProperties properties) {
			if (this.useHeight)
				properties.Height.CopyFrom(Height);
		}
		protected internal void ApplyBackgroundColorToCell(TableCellProperties properties) {
			if (this.useBackgroundColor)
				properties.BackgroundColor = BackgroundColor;
		}
		protected internal void ApplyVerticalAlignmentToCell(TableCellProperties properties) {
			if (this.useVerticalAlignment)
				properties.VerticalAlignment = VerticalAlignment;
		}
		protected internal bool IsDefaultProperties() {
			if (!useHeight && !useBackgroundColor && !useVerticalAlignment)
				return true;
			return false;
		}
		public void ApplyPropertiesToCharacter(CharacterFormattingBase characterFormattingBase) {
			if (useBackgroundColor)
				characterFormattingBase.BackColor = BackgroundColor;
		}
	}
	#endregion
	public class HtmlImageProperties {
		WidthUnitInfo width;
		WidthUnitInfo height;
		bool useWidth;
		bool useHeight;
		public HtmlImageProperties() {
			this.height = new WidthUnitInfo();
			this.width = new WidthUnitInfo();
			Alignment = HtmlImageAlignment.None;
		}
		#region Width
		public WidthUnitInfo Width {
			get { return width; }
			set {
				if (value.Value < 0)
					Exceptions.ThrowArgumentException("Width", value.Value);
				width = value;
				useWidth = true;
			}
		}
		public bool UseWidth { get { return useWidth; } }
		#endregion
		#region Height
		public WidthUnitInfo Height {
			get { return height; }
			set {
				if (value.Value < 0)
					Exceptions.ThrowArgumentException("Height", value.Value);
				height = value;
				useHeight = true;
			}
		}
		public bool UseHeight { get { return useHeight; } }
		#endregion
		public HtmlImageAlignment Alignment { get; set; }
		protected internal void CopyFrom(HtmlImageProperties value) {
			this.width = value.Width;
			this.useWidth = value.useWidth;
			this.height = value.Height;
			this.useHeight = value.useHeight;
			this.Alignment = value.Alignment;
		}
		protected internal HtmlImageProperties MergeWith(HtmlImageProperties properties) {
			HtmlImageProperties mergeProperties = new HtmlImageProperties();
			if (this.useHeight)
				mergeProperties.Height = this.Height;
			else if (properties.useHeight)
				mergeProperties.Height = properties.Height;
			if (this.useWidth)
				mergeProperties.Width = this.Width;
			else if (properties.useWidth)
				mergeProperties.Width = properties.Width;
			mergeProperties.Alignment = this.Alignment;
			return mergeProperties;
		}
		protected internal bool IsDefaultProperties() {
			if (!useHeight && !useWidth && Alignment == HtmlImageAlignment.None)
				return true;
			return false;
		}
	}
	public enum HtmlCssFloat {
		NotSet,
		Left,
		Right,
		None
	}
	#region HtmlInputPosition
	public class HtmlInputPosition : InputPosition, ICellPropertiesOwner {
		#region Fields
		readonly ParagraphFormattingBase paragraphFormatting;
		readonly HtmlListLevelProperties listLevelProperties;
		int levelIndex;
		int additionalIndent;
		HtmlParagraphAlignment defaultAlignment;
		HtmlTableProperties tableProperties;
		TableCellProperties cellProperties;
		HtmlTableRowProperties rowProperties;
		TabFormattingInfo paragraphTabs;
		int tableCellRowSpan;
		int nextTableRowHeight;
		HtmlImageProperties imageProperties;
		HtmlCssFloat cssFloat;
		#endregion
		public HtmlInputPosition(PieceTable pieceTable)
			: base(pieceTable) {
			this.CharacterFormatting.BeginUpdate();
			this.CharacterFormatting.FontName = "Times New Roman";
			this.CharacterFormatting.DoubleFontSize = 24;
			this.CharacterFormatting.ResetUse(CharacterFormattingOptions.Mask.UseAll);
			this.CharacterFormatting.EndUpdate();
			this.paragraphFormatting = new ParagraphFormattingBase(pieceTable, pieceTable.DocumentModel, ParagraphFormattingInfoCache.DefaultItemIndex, ParagraphFormattingOptionsCache.EmptyParagraphFormattingOptionIndex);
			this.defaultAlignment = new HtmlParagraphAlignment();
			this.listLevelProperties = new HtmlListLevelProperties();
			this.levelIndex = -1;
			this.additionalIndent = 0;
			this.tableProperties = new HtmlTableProperties();
			this.cellProperties = new TableCellProperties(this.PieceTable, this);
			this.rowProperties = new HtmlTableRowProperties();
			this.paragraphTabs = new TabFormattingInfo();
			this.tableCellRowSpan = 1;
			nextTableRowHeight = 0;
			imageProperties = new HtmlImageProperties();
			cssFloat = HtmlCssFloat.NotSet;
		}
		#region Properties
		public ParagraphFormattingBase ParagraphFormatting { get { return paragraphFormatting; } }
		public HtmlParagraphAlignment DefaultAlignment { get { return defaultAlignment; } set { defaultAlignment = value; } }
		public HtmlListLevelProperties ListLevelProperties { get { return listLevelProperties; } }
		public int AdditionalIndent { get { return additionalIndent; } set { additionalIndent = value; } }
		public int NextTableRowHeight { get { return nextTableRowHeight; } set { nextTableRowHeight = value; } }
		public HtmlImageProperties ImageProperties { get { return imageProperties; } set { imageProperties = value; } }
		public HtmlCssFloat CssFloat { get { return cssFloat; } set { cssFloat = value; } }
		public int LevelIndex {
			get { return levelIndex; }
			set {
				if (value >= 9)
					return;
				levelIndex = value;
			}
		}
		public int TableCellRowSpan {
			get { return tableCellRowSpan; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("RowSpan", value);
				tableCellRowSpan = value;
			}
		}
		public HtmlTableProperties TableProperties { get { return tableProperties; } }
		public TableCellProperties CellProperties { get { return cellProperties; } }
		public HtmlTableRowProperties RowProperties { get { return rowProperties; } }
		public TabFormattingInfo ParagraphTabs { get { return paragraphTabs; } }
		#endregion
		protected internal void CopyFrom(HtmlInputPosition value) {
			this.CharacterFormatting.CopyFrom(value.CharacterFormatting);
			this.ParagraphFormatting.CopyFrom(value.ParagraphFormatting);
			this.ListLevelProperties.CopyFrom(value.ListLevelProperties);
			this.TableProperties.CopyFrom(value.TableProperties);
			this.CellProperties.CopyFrom(value.CellProperties);
			this.RowProperties.CopyFrom(value.RowProperties);
			this.ParagraphTabs.CopyFrom(value.ParagraphTabs);
			this.CharacterStyleIndex = value.CharacterStyleIndex;
			this.DefaultAlignment.CopyFrom(value.DefaultAlignment);
			this.AdditionalIndent = value.AdditionalIndent;
			this.levelIndex = value.levelIndex;
			this.tableCellRowSpan = value.tableCellRowSpan;
			this.imageProperties = value.ImageProperties;
			this.cssFloat = value.CssFloat;
		}
		protected internal void CopyParagraphFormattingFrom(HtmlInputPosition value) {
			this.ParagraphFormatting.CopyFrom(value.ParagraphFormatting);
		}
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == CellProperties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
	}
	#endregion
	#region HtmlKeywordTranslator
	public delegate TagBase HtmlTranslateKeywordHandler(HtmlImporter importer);
	public class HtmlKeywordTranslatorTable : Dictionary<HtmlTagNameID, HtmlTranslateKeywordHandler> {
	}
	#endregion
	#region HtmlTagNameIDTable
	public class HtmlTagNameIDTable : Dictionary<String, HtmlTagNameID> {
	}
	#endregion
	#region AttributeKeywordTranslator
	public delegate void AttributeTranslateKeywordHandler(HtmlImporter importer, string value, TagBase tag);
	public class AttributeKeywordTranslatorTable : Dictionary<string, AttributeTranslateKeywordHandler> {
	}
	#endregion
	#region OpenHtmlTag
	public class OpenHtmlTag {
		readonly TagBase tag;
		readonly HtmlInputPosition oldPosition;
		public OpenHtmlTag(TagBase tag, PieceTable pieceTable) {
			this.tag = tag;
			this.oldPosition = new HtmlInputPosition(pieceTable);
		}
		public TagBase Tag { get { return tag; } }
		public HtmlInputPosition OldPosition { get { return oldPosition; } }
		public override string ToString() {
			return String.Format("Tag: {0}", Tag.ToString());
		}
	}
	#endregion
	#region HtmlFontSize
	public class HtmlFontSize {
		const int defaultHtmlFontSize = 3;
		Dictionary<int, float> htmlToDocumentFontSizeTable;
		int fontSize;
		public HtmlFontSize() {
			this.htmlToDocumentFontSizeTable = CreateHtmlToDocumentFontSizeTable();
			this.fontSize = defaultHtmlFontSize;
		}
		public virtual Dictionary<int, float> CreateHtmlToDocumentFontSizeTable() {
			Dictionary<int, float> result = new Dictionary<int, float>();
			result.Add(1, 7.5f);
			result.Add(2, 10);
			result.Add(3, 12);
			result.Add(4, 13.5f);
			result.Add(5, 18);
			result.Add(6, 24);
			result.Add(7, 36);
			return result;
		}
		protected internal float GetFontSize(int htmlFontSize) {
			if (htmlFontSize >= 7)
				htmlFontSize = 7;
			else if (htmlFontSize <= 1)
				htmlFontSize = 1;
			return htmlToDocumentFontSizeTable[htmlFontSize];
		}
		protected internal int GetDoubleFontSize(int htmlFontSize) {
			return (int)Math.Ceiling(GetFontSize(htmlFontSize) * 2.0f);
		}
		protected internal float GetLargerFontSize(int doubleFontSize) {
			float documentFontSize = (float)doubleFontSize / 2.0f;
			int htmlFontSize = 1;
			foreach (KeyValuePair<int, float> fontSizeItem in this.htmlToDocumentFontSizeTable) {
				htmlFontSize = fontSizeItem.Key;
				if (documentFontSize < fontSizeItem.Value)
					break;
			}
			return GetFontSize(htmlFontSize);
		}
		protected internal float GetLargerDoubleFontSize(int doubleFontSize) {
			return (int)Math.Ceiling(GetLargerFontSize(doubleFontSize) * 2.0f);
		}
		protected internal float GetLargerFontSize() {
			if (fontSize < 7)
				fontSize += 1;
			return GetFontSize(fontSize);
		}
		protected internal int GetLargerDoubleFontSize() {
			return (int)Math.Ceiling(GetLargerFontSize() * 2.0f);
		}
		protected internal float GetSmallerFontSize(int doubleFontSize) {
			float documentFontSize = (float)doubleFontSize / 2.0f;
			int htmlFontSize = 1;
			foreach (KeyValuePair<int, float> fontSizeItem in this.htmlToDocumentFontSizeTable) {
				if (documentFontSize > fontSizeItem.Value)
					break;
				htmlFontSize = fontSizeItem.Key;
			}
			return GetFontSize(htmlFontSize);
		}
		protected internal int GetSmallerDoubleFontSize(int doubleFontSize) {
			return (int)Math.Ceiling(GetSmallerFontSize(doubleFontSize) * 2.0f);
		}
		protected internal float GetSmallerFontSize() {
			if (fontSize > 1)
				fontSize -= 1;
			return GetFontSize(fontSize);
		}
		protected internal int GetSmallerDoubleFontSize() {
			return (int)Math.Ceiling(GetSmallerFontSize() * 2.0f);
		}
	}
	#endregion
	#region HtmlCodePageDecoder
	public static class HtmlCodePageDecoder {
		public static string ApplyEncoding(string content, Encoding encoding) {
			int count = content.Length;
			byte[] bytes = new byte[count];
			for (int i = 0; i < count; i++)
				bytes[i] = (byte)content[i];
			return encoding.GetString(bytes, 0, bytes.Length);
		}
	}
	#endregion
	public class HtmlImportedTableInfo : ImportedTableInfo {
		List<TableCell> tableCaption;
		int captionColSpan;
		public HtmlImportedTableInfo(Table table)
			: base(table) {
			this.tableCaption = new List<TableCell>();
			captionColSpan = 0;
		}
		public List<TableCell> TableCaption { get { return tableCaption; } }
		public int CaptionColSpan {
			get { return captionColSpan; }
			set {
				if (captionColSpan == value)
					return;
				if (captionColSpan == 0 && value == 1) {
					captionColSpan = value;
					return;
				}
				captionColSpan = value;
				foreach (TableCell cell in TableCaption)
					cell.ColumnSpan = value;
			}
		}
	}
	public class HtmlTablesImportHelper : TablesImportHelper {
		readonly HtmlImporter importer;
		public HtmlTablesImportHelper(PieceTable pieceTable, HtmlImporter importer)
			: base(pieceTable) {
			this.importer = importer;
		}
		public HtmlImporter Importer { get { return importer; } }
		public new HtmlImportedTableInfo TableInfo { get { return (base.TableInfo == null) ? null : (HtmlImportedTableInfo)(base.TableInfo); } }
		protected override ImportedTableInfo CreateTableInfo(Table newTable) {
			return new HtmlImportedTableInfo(newTable);
		}
		protected internal override TableCell CreateCoveredCellWithEmptyParagraph(TableRow row) {
			TableCell coveredCell = new TableCell(row);
			coveredCell.Row.Cells.AddInternal(coveredCell);
			InsertEmptyParagraph();
			ParagraphIndex start = Importer.Position.ParagraphIndex;
			ParagraphIndex end = Importer.Position.ParagraphIndex;
			Debug.Assert(Object.ReferenceEquals(row, Table.LastRow));
			InitializeTableCell(coveredCell, start, end);
			return coveredCell;
		}
		void InsertEmptyParagraph() {
			if (Importer.IsEmptyParagraph) {
				Importer.IsEmptyParagraph = false;
			}
			else {
				Importer.TagsStack[0].Tag.ParagraphFunctionalProcess();
				Importer.IsEmptyParagraph = false;
			}
		}
		public override void FixBrokenCells(Table currentTable) {
			DocumentLogPosition oldEnd = Importer.PieceTable.Paragraphs.Last.EndLogPosition;
			int oldParagraphsCount = Importer.PieceTable.Paragraphs.Count;
			base.FixBrokenCells(currentTable);
			currentTable.NormalizeRows();
			currentTable.NormalizeCellColumnSpans();
			DocumentLogPosition currentLogPosition = importer.PieceTable.Paragraphs.Last.EndLogPosition;
			int currentParagraphsCount = Importer.PieceTable.Paragraphs.Count;
			int diffLogPosition = Math.Max(0, oldEnd - currentLogPosition);
			int diffParagraphIndex = Math.Max(0, oldParagraphsCount - currentParagraphsCount);
			Importer.Position.LogPosition -= diffLogPosition;
			Importer.Position.ParagraphIndex -= diffParagraphIndex;
		}
	}
	#region HtmlImporter
	public class HtmlImporter : RichEditDocumentModelImporter, IHtmlImporter {
		#region Fields
		readonly static HtmlKeywordTranslatorTable htmlKeywordTable = CreateHtmlKeywordTable();
		readonly static HtmlSpecialSymbolTable specialSymbolTable = CreateHtmlSpecialSymbolTable();
		readonly List<OpenHtmlTag> tagsStack;
		bool hasPreformattedTagInStack;
		HtmlInputPosition position;
		HtmlElement element;
		TagBase tag;
		HtmlFontSize htmlFontSize;
		bool isEmptyParagraph;
		bool isEmptyLine;
		bool isEmptyListItem;
		bool canInsertSpace;
		bool styleTagIsOpen;
		CssElementCollection styleTagCollection;
		string baseUri = String.Empty;
		Encoding encoding;
		bool suppressEncoding;
		ImportFieldInfo processHyperlink;
		readonly Stack<HtmlBookmarkInfo> processBookmarks;
		List<Field> emptyHyperlinks;
		readonly HtmlTablesImportHelper tablesImportHelper;
		SortedList<NumberingListIndex> usedNumberingLists;
		HtmlParser parser;
		DevExpress.Office.Services.Implementation.DataStringUriStreamProvider dataStringUriStreamProvider;
		Dictionary<string, Bookmark> markTable;
		#endregion
		#region CreateHtmlSpecialSymbolTable
		static internal HtmlSpecialSymbolTable CreateHtmlSpecialSymbolTable() {
			HtmlSpecialSymbolTable specialSymbolTable = new HtmlSpecialSymbolTable();
			specialSymbolTable.Add("quot", '\"');
			specialSymbolTable.Add("amp", '&');
			specialSymbolTable.Add("apos", '\'');
			specialSymbolTable.Add("lt", '<');
			specialSymbolTable.Add("gt", '>');
			specialSymbolTable.Add("nbsp", Characters.NonBreakingSpace);
			specialSymbolTable.Add("iexcl", '¡');
			specialSymbolTable.Add("cent", '¢');
			specialSymbolTable.Add("pound", '£');
			specialSymbolTable.Add("curren", '¤');
			specialSymbolTable.Add("yen", '¥');
			specialSymbolTable.Add("brvbar", '¦');
			specialSymbolTable.Add("sect", '§');
			specialSymbolTable.Add("uml", '¨');
			specialSymbolTable.Add("copy", '©');
			specialSymbolTable.Add("ordf", 'ª');
			specialSymbolTable.Add("laquo", '«');
			specialSymbolTable.Add("not", '¬');
			specialSymbolTable.Add("shy", '­');
			specialSymbolTable.Add("reg", '®');
			specialSymbolTable.Add("macr", '¯');
			specialSymbolTable.Add("deg", '°');
			specialSymbolTable.Add("plusmn", '±');
			specialSymbolTable.Add("sup2", '²');
			specialSymbolTable.Add("sup3", '³');
			specialSymbolTable.Add("acute", '´');
			specialSymbolTable.Add("micro", 'µ');
			specialSymbolTable.Add("para", '¶');
			specialSymbolTable.Add("middot", '·');
			specialSymbolTable.Add("cedil", '¸');
			specialSymbolTable.Add("sup1", '¹');
			specialSymbolTable.Add("ordm", 'º');
			specialSymbolTable.Add("raquo", '»');
			specialSymbolTable.Add("frac14", '¼');
			specialSymbolTable.Add("frac12", '½');
			specialSymbolTable.Add("frac34", '¾');
			specialSymbolTable.Add("iquest", '¿');
			specialSymbolTable.Add("Agrave", 'À');
			specialSymbolTable.Add("Aacute", 'Á');
			specialSymbolTable.Add("Acirc", 'Â');
			specialSymbolTable.Add("Atilde", 'Ã');
			specialSymbolTable.Add("Auml", 'Ä');
			specialSymbolTable.Add("Aring", 'Å');
			specialSymbolTable.Add("AElig", 'Æ');
			specialSymbolTable.Add("Ccedil", 'Ç');
			specialSymbolTable.Add("Egrave", 'È');
			specialSymbolTable.Add("Eacute", 'É');
			specialSymbolTable.Add("Ecirc", 'Ê');
			specialSymbolTable.Add("Euml", 'Ë');
			specialSymbolTable.Add("Igrave", 'Ì');
			specialSymbolTable.Add("Iacute", 'Í');
			specialSymbolTable.Add("Icirc", 'Î');
			specialSymbolTable.Add("Iuml", 'Ï');
			specialSymbolTable.Add("ETH", 'Ð');
			specialSymbolTable.Add("Ntilde", 'Ñ');
			specialSymbolTable.Add("Ograve", 'Ò');
			specialSymbolTable.Add("Oacute", 'Ó');
			specialSymbolTable.Add("Ocirc", 'Ô');
			specialSymbolTable.Add("Otilde", 'Õ');
			specialSymbolTable.Add("Ouml", 'Ö');
			specialSymbolTable.Add("times", '×');
			specialSymbolTable.Add("Oslash", 'Ø');
			specialSymbolTable.Add("Ugrave", 'Ù');
			specialSymbolTable.Add("Uacute", 'Ú');
			specialSymbolTable.Add("Ucirc", 'Û');
			specialSymbolTable.Add("Uuml", 'Ü');
			specialSymbolTable.Add("Yacute", 'Ý');
			specialSymbolTable.Add("Yuml", 'Ÿ');
			specialSymbolTable.Add("THORN", 'Þ');
			specialSymbolTable.Add("szlig", 'ß');
			specialSymbolTable.Add("agrave", 'à');
			specialSymbolTable.Add("aacute", 'á');
			specialSymbolTable.Add("acirc", 'â');
			specialSymbolTable.Add("atilde", 'ã');
			specialSymbolTable.Add("auml", 'ä');
			specialSymbolTable.Add("aring", 'å');
			specialSymbolTable.Add("aelig", 'æ');
			specialSymbolTable.Add("ccedil", 'ç');
			specialSymbolTable.Add("egrave", 'è');
			specialSymbolTable.Add("eacute", 'é');
			specialSymbolTable.Add("ecirc", 'ê');
			specialSymbolTable.Add("euml", 'ë');
			specialSymbolTable.Add("igrave", 'ì');
			specialSymbolTable.Add("iacute", 'í');
			specialSymbolTable.Add("icirc", 'î');
			specialSymbolTable.Add("iuml", 'ï');
			specialSymbolTable.Add("eth", 'ð');
			specialSymbolTable.Add("ntilde", 'ñ');
			specialSymbolTable.Add("ograve", 'ò');
			specialSymbolTable.Add("oacute", 'ó');
			specialSymbolTable.Add("ocirc", 'ô');
			specialSymbolTable.Add("otilde", 'õ');
			specialSymbolTable.Add("ouml", 'ö');
			specialSymbolTable.Add("divide", '÷');
			specialSymbolTable.Add("oslash", 'ø');
			specialSymbolTable.Add("ugrave", 'ù');
			specialSymbolTable.Add("uacute", 'ú');
			specialSymbolTable.Add("ucirc", 'û');
			specialSymbolTable.Add("uuml", 'ü');
			specialSymbolTable.Add("yacute", 'ý');
			specialSymbolTable.Add("thorn", 'þ');
			specialSymbolTable.Add("yuml", 'ÿ');
			specialSymbolTable.Add("oelig", 'œ');
			specialSymbolTable.Add("OElig", 'Œ');
			specialSymbolTable.Add("Scaron", 'Š');
			specialSymbolTable.Add("scaron", 'š');
			specialSymbolTable.Add("fnof", 'ƒ');
			specialSymbolTable.Add("circ", 'ˆ');
			specialSymbolTable.Add("tilde", '˜');
			specialSymbolTable.Add("trade", '™');
			specialSymbolTable.Add("hellip", '…');
			specialSymbolTable.Add("prime", '′');
			specialSymbolTable.Add("Prime", '″');
			specialSymbolTable.Add("oline", '‾');
			specialSymbolTable.Add("frasl", '⁄');
			specialSymbolTable.Add("infin", '∞');
			specialSymbolTable.Add("asymp", '≈');
			specialSymbolTable.Add("ne", '≠');
			specialSymbolTable.Add("le", '≤');
			specialSymbolTable.Add("ge", '≥');
			specialSymbolTable.Add("lsquo", '‘');
			specialSymbolTable.Add("rsquo", '’');
			specialSymbolTable.Add("ldquo", '“');
			specialSymbolTable.Add("rdquo", '”');
			specialSymbolTable.Add("bdquo", '\u201E');
			specialSymbolTable.Add("dagger", '†');
			specialSymbolTable.Add("Dagger", '‡');
			specialSymbolTable.Add("permil", '‰');
			specialSymbolTable.Add("lsaquo", '‹');
			specialSymbolTable.Add("rsaquo", '›');
			specialSymbolTable.Add("euro", '€');
			specialSymbolTable.Add("ndash", '–');
			specialSymbolTable.Add("mdash", '—');
			specialSymbolTable.Add("emsp", Characters.EmSpace);
			specialSymbolTable.Add("ensp", Characters.EnSpace);
			specialSymbolTable.Add("thinsp", Characters.QmSpace);
			specialSymbolTable.Add("image", 'ℑ');
			specialSymbolTable.Add("weierp", '℘');
			specialSymbolTable.Add("real", 'ℜ');
			specialSymbolTable.Add("alefsym", 'ℵ');
			specialSymbolTable.Add("larr", '←');
			specialSymbolTable.Add("uarr", '↑');
			specialSymbolTable.Add("rarr", '→');
			specialSymbolTable.Add("darr", '↓');
			specialSymbolTable.Add("harr", '↔');
			specialSymbolTable.Add("crarr", '↵');
			specialSymbolTable.Add("lArr", '⇐');
			specialSymbolTable.Add("uArr", '⇑');
			specialSymbolTable.Add("rArr", '⇒');
			specialSymbolTable.Add("dArr", '⇓');
			specialSymbolTable.Add("hArr", '⇔');
			specialSymbolTable.Add("forall", '∀');
			specialSymbolTable.Add("part", '∂');
			specialSymbolTable.Add("exist", '∃');
			specialSymbolTable.Add("empty", '∅');
			specialSymbolTable.Add("nabla", '∇');
			specialSymbolTable.Add("isin", '∈');
			specialSymbolTable.Add("notin", '∉');
			specialSymbolTable.Add("ni", '∋');
			specialSymbolTable.Add("prod", '∏');
			specialSymbolTable.Add("sum", '∑');
			specialSymbolTable.Add("minus", '−');
			specialSymbolTable.Add("lowast", '∗');
			specialSymbolTable.Add("radic", '√');
			specialSymbolTable.Add("prop", '∝');
			specialSymbolTable.Add("ang", '∠');
			specialSymbolTable.Add("and", '∧');
			specialSymbolTable.Add("or", '∨');
			specialSymbolTable.Add("cap", '∩');
			specialSymbolTable.Add("cup", '∪');
			specialSymbolTable.Add("int", '∫');
			specialSymbolTable.Add("there4", '∴');
			specialSymbolTable.Add("sim", '∼');
			specialSymbolTable.Add("cong", '≅');
			specialSymbolTable.Add("equiv", '≡');
			specialSymbolTable.Add("sub", '⊂');
			specialSymbolTable.Add("sup", '⊃');
			specialSymbolTable.Add("nsub", '⊄');
			specialSymbolTable.Add("sube", '⊆');
			specialSymbolTable.Add("supe", '⊇');
			specialSymbolTable.Add("oplus", '⊕');
			specialSymbolTable.Add("otimes", '⊗');
			specialSymbolTable.Add("perp", '⊥');
			specialSymbolTable.Add("sdot", '⋅');
			specialSymbolTable.Add("lceil", '⌈');
			specialSymbolTable.Add("rceil", '⌉');
			specialSymbolTable.Add("lfloor", '⌊');
			specialSymbolTable.Add("rfloor", '⌋');
			specialSymbolTable.Add("lang", '〈');
			specialSymbolTable.Add("rang", '〉');
			specialSymbolTable.Add("loz", '◊');
			specialSymbolTable.Add("spades", '♠');
			specialSymbolTable.Add("clubs", '♣');
			specialSymbolTable.Add("hearts", '♥');
			specialSymbolTable.Add("diams", '♦');
			specialSymbolTable.Add("Alpha", 'Α');
			specialSymbolTable.Add("Beta", 'Β');
			specialSymbolTable.Add("Gamma", 'Γ');
			specialSymbolTable.Add("Delta", 'Δ');
			specialSymbolTable.Add("Epsilon", 'Ε');
			specialSymbolTable.Add("Zeta", 'Ζ');
			specialSymbolTable.Add("Eta", 'Η');
			specialSymbolTable.Add("Theta", 'Θ');
			specialSymbolTable.Add("Iota", 'Ι');
			specialSymbolTable.Add("Kappa", 'Κ');
			specialSymbolTable.Add("Lambda", 'Λ');
			specialSymbolTable.Add("Mu", 'Μ');
			specialSymbolTable.Add("Nu", 'Ν');
			specialSymbolTable.Add("Xi", 'Ξ');
			specialSymbolTable.Add("Omicron", 'Ο');
			specialSymbolTable.Add("Pi", 'Π');
			specialSymbolTable.Add("Rho", 'Ρ');
			specialSymbolTable.Add("Sigma", 'Σ');
			specialSymbolTable.Add("Tau", 'Τ');
			specialSymbolTable.Add("Upsilon", 'Υ');
			specialSymbolTable.Add("Phi", 'Φ');
			specialSymbolTable.Add("Chi", 'Χ');
			specialSymbolTable.Add("Psi", 'Ψ');
			specialSymbolTable.Add("Omega", 'Ω');
			specialSymbolTable.Add("alpha", 'α');
			specialSymbolTable.Add("beta", 'β');
			specialSymbolTable.Add("gamma", 'γ');
			specialSymbolTable.Add("delta", 'δ');
			specialSymbolTable.Add("epsilon", 'ε');
			specialSymbolTable.Add("zeta", 'ζ');
			specialSymbolTable.Add("eta", 'η');
			specialSymbolTable.Add("theta", 'θ');
			specialSymbolTable.Add("iota", 'ι');
			specialSymbolTable.Add("kappa", 'κ');
			specialSymbolTable.Add("lambda", 'λ');
			specialSymbolTable.Add("mu", 'μ');
			specialSymbolTable.Add("nu", 'ν');
			specialSymbolTable.Add("xi", 'ξ');
			specialSymbolTable.Add("omicron", 'ο');
			specialSymbolTable.Add("pi", 'π');
			specialSymbolTable.Add("rho", 'ρ');
			specialSymbolTable.Add("sigmaf", 'ς');
			specialSymbolTable.Add("sigma", 'σ');
			specialSymbolTable.Add("tau", 'τ');
			specialSymbolTable.Add("upsilon", 'υ');
			specialSymbolTable.Add("phi", 'φ');
			specialSymbolTable.Add("chi", 'χ');
			specialSymbolTable.Add("psi", 'ψ');
			specialSymbolTable.Add("omega", 'ω');
			specialSymbolTable.Add("thetasy", 'ϑ');
			specialSymbolTable.Add("upsih", 'ϒ');
			specialSymbolTable.Add("piv", 'ϖ');
			specialSymbolTable.Add("bull", '•');
			return specialSymbolTable;
		}
		#endregion
		#region CreateHtmlKeywordTable
		protected static HtmlKeywordTranslatorTable CreateHtmlKeywordTable() {
			HtmlKeywordTranslatorTable htmlKeywordTable = new HtmlKeywordTranslatorTable();
			htmlKeywordTable.Add(HtmlTagNameID.Abbr, AbbrKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Acronym, AcronymKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Address, AddressKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Area, AreaKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.BaseFont, BaseFontKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Bdo, BdoKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.BgSound, BgsoundKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Button, ButtonKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Cite, CiteKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Dd, DdKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Del, DelKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Dfn, DfnKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Dl, DlKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Dt, DtKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Embed, EmbedKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Fieldset, FieldsetKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Form, FormKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Frame, FrameKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.FrameSet, FrameSetKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Hr, HrKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Iframe, IframeKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Input, InputKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Ins, InsKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Kbd, KbdKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Label, LabelKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Legend, LegendKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Map, MapKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Nobr, NobrKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Noembed, NoembedKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.NoFrames, NoFramesKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.NoScript, NoScriptKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Object, ObjectKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.OptGroup, OptGroupKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Option, OptionKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Param, ParamKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Q, QKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Samp, SampKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Select, SelectKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.TextArea, TextAreaKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.TT, TtKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Var, VarKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Wbr, WbrKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Xmp, XmpKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Html, HtmlKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Head, HeadKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Base, BaseKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Meta, MetaKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Title, TitleKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Link, LinkKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Anchor, AnchorKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Body, BodyKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Bold, BoldKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Italic, ItalicKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Underline, UnderlineKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Paragraph, ParagraphKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Strong, StrongKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Big, BigKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Small, SmallKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Preformatted, PreformattedKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Font, FontKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.LineBreak, LineBreakKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Emphasized, EmphasizedKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Img, ImageKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Heading1, Heading1KeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Heading2, Heading2KeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Heading3, Heading3KeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Heading4, Heading4KeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Heading5, Heading5KeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Heading6, Heading6KeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.SuperScript, SuperScriptKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.SubScript, SubScriptKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Center, CenterKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Table, TableKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Tbody, TBodyKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Tfoot, TFootKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Thead, THeadKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.TR, TrKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.TH, ThKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.TD, TdKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.LI, LevelKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.NumberingList, NumberingListKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.BulletList, BulletListKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.S, StrikeoutKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Strike, StrikeoutKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Code, CodeKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Span, SpanKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Div, DivisionKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Script, ScriptKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Blockquote, BlockquoteKeywordTag);
			htmlKeywordTable.Add(HtmlTagNameID.Caption, CaptionKeywordTag);
			return htmlKeywordTable;
		}
		#endregion
		#region StaticKeywordTagMetods
		static internal TagBase AbbrKeywordTag(HtmlImporter importer) {
			return new AbberTag(importer);
		}
		static internal TagBase AcronymKeywordTag(HtmlImporter importer) {
			return new AcronymTag(importer);
		}
		static internal TagBase AddressKeywordTag(HtmlImporter importer) {
			return new AddressTag(importer);
		}
		static internal TagBase AreaKeywordTag(HtmlImporter importer) {
			return new AreaTag(importer);
		}
		static internal TagBase BaseFontKeywordTag(HtmlImporter importer) {
			return new BaseFontTag(importer);
		}
		static internal TagBase BdoKeywordTag(HtmlImporter importer) {
			return new BdoTag(importer);
		}
		static internal TagBase BgsoundKeywordTag(HtmlImporter importer) {
			return new BgsoundTag(importer);
		}
		static internal TagBase ButtonKeywordTag(HtmlImporter importer) {
			return new ButtonTag(importer);
		}
		static internal TagBase CiteKeywordTag(HtmlImporter importer) {
			return new CiteTag(importer);
		}
		static internal TagBase DdKeywordTag(HtmlImporter importer) {
			return new DdTag(importer);
		}
		static internal TagBase DelKeywordTag(HtmlImporter importer) {
			return new DelTag(importer);
		}
		static internal TagBase DfnKeywordTag(HtmlImporter importer) {
			return new DfnTag(importer);
		}
		static internal TagBase DlKeywordTag(HtmlImporter importer) {
			return new DlTag(importer);
		}
		static internal TagBase DtKeywordTag(HtmlImporter importer) {
			return new DtTag(importer);
		}
		static internal TagBase EmbedKeywordTag(HtmlImporter importer) {
			return new EmbedTag(importer);
		}
		static internal TagBase FieldsetKeywordTag(HtmlImporter importer) {
			return new FieldsetTag(importer);
		}
		static internal TagBase FormKeywordTag(HtmlImporter importer) {
			return new FormTag(importer);
		}
		static internal TagBase FrameKeywordTag(HtmlImporter importer) {
			return new FrameTag(importer);
		}
		static internal TagBase FrameSetKeywordTag(HtmlImporter importer) {
			return new FrameSetTag(importer);
		}
		static internal TagBase HrKeywordTag(HtmlImporter importer) {
			return new HrTag(importer);
		}
		static internal TagBase IframeKeywordTag(HtmlImporter importer) {
			return new IframeTag(importer);
		}
		static internal TagBase InputKeywordTag(HtmlImporter importer) {
			return new InputTag(importer);
		}
		static internal TagBase InsKeywordTag(HtmlImporter importer) {
			return new InsTag(importer);
		}
		static internal TagBase KbdKeywordTag(HtmlImporter importer) {
			return new KbdTag(importer);
		}
		static internal TagBase LabelKeywordTag(HtmlImporter importer) {
			return new LabelTag(importer);
		}
		static internal TagBase LegendKeywordTag(HtmlImporter importer) {
			return new LegendTag(importer);
		}
		static internal TagBase MapKeywordTag(HtmlImporter importer) {
			return new MapTag(importer);
		}
		static internal TagBase MarqueeKeywordTag(HtmlImporter importer) {
			return new MarqueeTag(importer);
		}
		static internal TagBase NobrKeywordTag(HtmlImporter importer) {
			return new NobrTag(importer);
		}
		static internal TagBase NoembedKeywordTag(HtmlImporter importer) {
			return new NoembedTag(importer);
		}
		static internal TagBase NoFramesKeywordTag(HtmlImporter importer) {
			return new NoFramesTag(importer);
		}
		static internal TagBase NoScriptKeywordTag(HtmlImporter importer) {
			return new NoScriptTag(importer);
		}
		static internal TagBase ObjectKeywordTag(HtmlImporter importer) {
			return new ObjectTag(importer);
		}
		static internal TagBase OptGroupKeywordTag(HtmlImporter importer) {
			return new OptGroupTag(importer);
		}
		static internal TagBase OptionKeywordTag(HtmlImporter importer) {
			return new OptionTag(importer);
		}
		static internal TagBase ParamKeywordTag(HtmlImporter importer) {
			return new ParamTag(importer);
		}
		static internal TagBase QKeywordTag(HtmlImporter importer) {
			return new QTag(importer);
		}
		static internal TagBase SampKeywordTag(HtmlImporter importer) {
			return new SampTag(importer);
		}
		static internal TagBase SelectKeywordTag(HtmlImporter importer) {
			return new SelectTag(importer);
		}
		static internal TagBase TableKeywordTag(HtmlImporter importer) {
			return new TableTag(importer);
		}
		static internal TagBase TdKeywordTag(HtmlImporter importer) {
			return new TdTag(importer);
		}
		static internal TagBase TextAreaKeywordTag(HtmlImporter importer) {
			return new TextAreaTag(importer);
		}
		static internal TagBase ThKeywordTag(HtmlImporter importer) {
			return new ThTag(importer);
		}
		static internal TagBase TFootKeywordTag(HtmlImporter importer) {
			return new TFootTag(importer);
		}
		static internal TagBase CaptionKeywordTag(HtmlImporter importer) {
			return new CaptionTag(importer);
		}
		static internal TagBase TBodyKeywordTag(HtmlImporter importer) {
			return new TBodyTag(importer);
		}
		static internal TagBase THeadKeywordTag(HtmlImporter importer) {
			return new THeadTag(importer);
		}
		static internal TagBase TrKeywordTag(HtmlImporter importer) {
			return new TrTag(importer);
		}
		static internal TagBase TtKeywordTag(HtmlImporter importer) {
			return new TtTag(importer);
		}
		static internal TagBase VarKeywordTag(HtmlImporter importer) {
			return new VarTag(importer);
		}
		static internal TagBase WbrKeywordTag(HtmlImporter importer) {
			return new WbrTag(importer);
		}
		static internal TagBase XmpKeywordTag(HtmlImporter importer) {
			return new XmpTag(importer);
		}
		static internal TagBase HtmlKeywordTag(HtmlImporter importer) {
			return new HtmlTag(importer);
		}
		static internal TagBase HeadKeywordTag(HtmlImporter importer) {
			return new HeadTag(importer);
		}
		static internal TagBase BaseKeywordTag(HtmlImporter importer) {
			return new BaseTag(importer);
		}
		static internal TagBase MetaKeywordTag(HtmlImporter importer) {
			return new MetaTag(importer);
		}
		static internal TagBase BodyKeywordTag(HtmlImporter importer) {
			return new BodyTag(importer);
		}
		static internal TagBase TitleKeywordTag(HtmlImporter importer) {
			return new TitleTag(importer);
		}
		static internal TagBase LinkKeywordTag(HtmlImporter importer) {
			return new LinkTag(importer);
		}
		static internal TagBase AnchorKeywordTag(HtmlImporter importer) {
			return new AnchorTag(importer);
		}
		static internal TagBase BoldKeywordTag(HtmlImporter importer) {
			return new BoldTag(importer);
		}
		static internal TagBase ItalicKeywordTag(HtmlImporter importer) {
			return new ItalicTag(importer);
		}
		static internal TagBase UnderlineKeywordTag(HtmlImporter importer) {
			return new UnderlineTag(importer);
		}
		static internal TagBase ParagraphKeywordTag(HtmlImporter importer) {
			return new ParagraphTag(importer);
		}
		static internal TagBase StrongKeywordTag(HtmlImporter importer) {
			return new StrongTag(importer);
		}
		static internal TagBase BigKeywordTag(HtmlImporter importer) {
			return new BigTag(importer);
		}
		static internal TagBase SmallKeywordTag(HtmlImporter importer) {
			return new SmallTag(importer);
		}
		static internal TagBase PreformattedKeywordTag(HtmlImporter importer) {
			return new PreformattedTag(importer);
		}
		static internal TagBase FontKeywordTag(HtmlImporter importer) {
			return new FontTag(importer);
		}
		static internal TagBase LineBreakKeywordTag(HtmlImporter importer) {
			return new LineBreakTag(importer);
		}
		static internal TagBase EmphasizedKeywordTag(HtmlImporter importer) {
			return new EmphasizedTag(importer);
		}
		static internal TagBase ImageKeywordTag(HtmlImporter importer) {
			return new ImageTag(importer);
		}
		static internal TagBase SubScriptKeywordTag(HtmlImporter importer) {
			return new SubScriptTag(importer);
		}
		static internal TagBase SuperScriptKeywordTag(HtmlImporter importer) {
			return new SuperScriptTag(importer);
		}
		static internal TagBase CenterKeywordTag(HtmlImporter importer) {
			return new CenterTag(importer);
		}
		static internal TagBase StrikeoutKeywordTag(HtmlImporter importer) {
			return new StrikeoutTag(importer);
		}
		static internal TagBase Heading1KeywordTag(HtmlImporter importer) {
			return new HeadingTag(importer, 6, 1);
		}
		static internal TagBase Heading2KeywordTag(HtmlImporter importer) {
			return new HeadingTag(importer, 5, 2);
		}
		static internal TagBase Heading3KeywordTag(HtmlImporter importer) {
			return new HeadingTag(importer, 4, 3);
		}
		static internal TagBase Heading4KeywordTag(HtmlImporter importer) {
			return new HeadingTag(importer, 3, 4);
		}
		static internal TagBase Heading5KeywordTag(HtmlImporter importer) {
			return new HeadingTag(importer, 2, 5);
		}
		static internal TagBase Heading6KeywordTag(HtmlImporter importer) {
			return new HeadingTag(importer, 1, 6);
		}
		static internal TagBase NumberingListKeywordTag(HtmlImporter importer) {
			return new NumberingListTag(importer);
		}
		static internal TagBase BulletListKeywordTag(HtmlImporter importer) {
			return new BulletListTag(importer);
		}
		static internal TagBase LevelKeywordTag(HtmlImporter importer) {
			return new LevelTag(importer);
		}
		static internal TagBase CodeKeywordTag(HtmlImporter importer) {
			return new CodeTag(importer);
		}
		static internal TagBase SpanKeywordTag(HtmlImporter importer) {
			return new SpanTag(importer);
		}
		static internal TagBase DivisionKeywordTag(HtmlImporter importer) {
			return new DivisionTag(importer);
		}
		static internal TagBase ScriptKeywordTag(HtmlImporter importer) {
			return new ScriptTag(importer);
		}
		static internal TagBase BlockquoteKeywordTag(HtmlImporter importer) {
			return new BlockquoteTag(importer);
		}
		#endregion;
		public HtmlImporter(DocumentModel documentModel, HtmlDocumentImporterOptions options)
			: base(documentModel, options) {
			this.position = new HtmlInputPosition(PieceTable);
			this.tagsStack = new List<OpenHtmlTag>();
			this.htmlFontSize = new HtmlFontSize();
			this.styleTagCollection = new CssElementCollection();
			this.isEmptyParagraph = true;
			this.isEmptyLine = false;
			this.canInsertSpace = true;
			this.LastOpenParagraphTagIndex = -1;
			this.LastOpenAnchorTagIndex = -1;
			this.BaseUri = options.SourceUri;
			this.AbsoluteBaseUri = GetAbsoluteBaseUri(options.SourceUri);
			this.encoding = options.Encoding;
			this.processBookmarks = new Stack<HtmlBookmarkInfo>();
			this.tablesImportHelper = new HtmlTablesImportHelper(PieceTable, this);
			this.usedNumberingLists = new SortedList<NumberingListIndex>();
			this.parser = CreateHtmlParser();
			this.markTable = new Dictionary<string, Bookmark>();
			RootDoubleFontSize = 24;
		}
		#region Properties
		public virtual HtmlKeywordTranslatorTable HtmlKeywordTable { get { return htmlKeywordTable; } }
		public virtual HtmlSpecialSymbolTable SpecialSymbolTable { get { return specialSymbolTable; } }
		public List<OpenHtmlTag> TagsStack { get { return tagsStack; } }
		public HtmlInputPosition Position { get { return position; } }
		protected internal PieceTable PieceTable { get { return DocumentModel.MainPieceTable; } }
		protected internal bool IsEmptyParagraph {
			get { return isEmptyParagraph; }
			set {
				isEmptyParagraph = value;
				if (!value)
					IsEmptyListItem = false;
			}
		}
		protected internal bool IsEmptyLine { get { return isEmptyLine; } set { isEmptyLine = value; } }
		protected internal bool IsEmptyListItem { get { return isEmptyListItem; } set { isEmptyListItem = value; } }
		public HtmlElement Element { get { return element; } }
		public CssElementCollection StyleTagCollection { get { return styleTagCollection; } }
		public HtmlFontSize HtmlFontSize { get { return htmlFontSize; } }
		public int RootDoubleFontSize { get; set; }
		public string BaseUri {
			get { return baseUri; }
			set {
				if (value == null)
					value = String.Empty;
				baseUri = Uri.UnescapeDataString(value);
			}
		}
		public int CodePage {
			get { return DXEncoding.GetEncodingCodePage(Encoding); }
			set {
				if (CodePage == value)
					return;
				try {
					this.encoding = DXEncoding.GetEncodingFromCodePage(value);
				}
				catch {
				}
			}
		}
		public Encoding Encoding { get { return encoding; } set { encoding = value; } }
		protected internal bool SuppressEncoding { get { return suppressEncoding; } }
		public HtmlDocumentImporterOptions Options { get { return (HtmlDocumentImporterOptions)InnerOptions; } }
		public int LastOpenParagraphTagIndex { get; set; }
		public int LastOpenAnchorTagIndex { get; set; }
		public ImportFieldInfo ProcessHyperlink { get { return processHyperlink; } set { processHyperlink = value; } }
		public Stack<HtmlBookmarkInfo> ProcessBookmarks { get { return processBookmarks; } }
		public HtmlTablesImportHelper TablesImportHelper { get { return tablesImportHelper; } }
		public SortedList<NumberingListIndex> UsedNumberingLists { get { return usedNumberingLists; } }
		public HtmlParser Parser { get { return parser; } }
		public string AbsoluteBaseUri { get; private set; }
		internal DevExpress.Office.Services.Implementation.DataStringUriStreamProvider DataStringUriStreamProvider { get { return dataStringUriStreamProvider; } }
		protected bool IgnoredTagIsOpen { get { return TagsStack.Count > 0 && TagsStack[TagsStack.Count - 1].Tag.ShouldBeIgnored; } }
		#endregion
		public virtual void Import(Stream stream) {
			dataStringUriStreamProvider = new DevExpress.Office.Services.Implementation.DataStringUriStreamProvider();
			DevExpress.Office.Services.IUriStreamService service = DocumentModel.GetService<DevExpress.Office.Services.IUriStreamService>();
			DocumentModel.BeginSetContent();
			if (service != null)
				service.RegisterProvider(dataStringUriStreamProvider);
			try {
				ImportCore(stream, new HtmlInputPosition(PieceTable));
				PieceTable.CheckIntegrity();
			}
			catch (Exception e) {
				DocumentModel.ClearDocumentCore(true, true);
				throw e;
			}
			finally {
				DocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument, true, Options.UpdateField.GetNativeOptions());
				if (service != null)
					service.UnregisterProvider(dataStringUriStreamProvider);
				dataStringUriStreamProvider = null;
			}
		}
		public virtual string GetAbsoluteBaseUri(string baseUri) {
#if !SL
			Uri result;
			if (Uri.TryCreate(baseUri, UriKind.Absolute, out result))
				return baseUri;
			else {
				try {
					if (!String.IsNullOrEmpty(baseUri))
						return Path.GetFullPath(baseUri);
					else
						return baseUri;
				}
				catch {
					return baseUri;
				}
			}
#else
			return baseUri;
#endif
		}
		public void ImportCore(Stream stream, HtmlInputPosition pos) {
			DocumentModel.DefaultCharacterProperties.DoubleFontSize = 24;
			DocumentModel.DefaultCharacterProperties.FontName = "Times New Roman";
			styleTagCollection.Clear();
			emptyHyperlinks = new List<Field>();
			List<HtmlElement> htmlElements = new List<HtmlElement>();
			this.position = pos;
			ClearTagStack();
			StreamReader streamReader = new StreamReader(stream, EmptyEncoding.Instance, true, 65536);
			streamReader.Peek();
			if (streamReader.CurrentEncoding != EmptyEncoding.Instance)
				this.suppressEncoding = true;
			ParseHtmlContent(htmlElements, streamReader);
			SortSelectors();
			PieceTable.InsertParagraph(PieceTable.DocumentEndLogPosition);
			ElementsImport(htmlElements);
			ProcessRemainTags();
			FixLastParagraph();
			List<PieceTable> pieceTables = DocumentModel.GetPieceTables(false);
			foreach (PieceTable pieceTable in pieceTables)
				FixBordersAndParagraphBetweenTables(pieceTable);
			DocumentModel.NormalizeZOrder();
		}
		void RemoveEmptyHyperlinks() {
			for (int i = emptyHyperlinks.Count - 1; i >= 0; i--) {
				Field emptyHyperlink = emptyHyperlinks[i];
				DocumentLogPosition start = PieceTable.GetRunLogPosition(emptyHyperlink.FirstRunIndex);
				DocumentLogPosition end = PieceTable.GetRunLogPosition(emptyHyperlink.LastRunIndex);
				PieceTable.DeleteContent(start, end - start + 1, false);
			}
		}
		protected virtual void SortSelectors() {
			int count = styleTagCollection.Count;
			for (int i = 0; i < count; i++)
				styleTagCollection[i].Selector.InvalidateSpecifity();
			Comparison<CssElement> cssElementComparsion = delegate(CssElement element1, CssElement element2) {
				int specifity1 = element1.Selector.Specifity;
				int specifity2 = element2.Selector.Specifity;
				if (specifity1 != specifity2)
					return specifity1 - specifity2;
				else
					return element1.Index - element2.Index;
			};
			styleTagCollection.Sort(cssElementComparsion);
		}
		protected internal virtual void ParseHtmlContent(List<HtmlElement> htmlElements, StreamReader streamReader) {
			Guard.ArgumentNotNull(streamReader, "streamReader");
			if (!streamReader.BaseStream.CanRead)
				Exceptions.ThrowArgumentException("Stream doesn't supports reading data", null);
			string rawText = string.Empty;
			element = parser.ParseNext(streamReader);
			while (element != null) {
				if (styleTagIsOpen)
					rawText = StyleTagIsOpen(rawText);
				else
					StyleTagIsNotOpen(streamReader, htmlElements);
				element = parser.ParseNext(streamReader);
			}
			if (Options.AutoDetectEncoding && !Options.ShouldSerializeEncoding()) {
				Encoding detectedEncoding = parser.DetectEncoding();
				if (detectedEncoding != null)
					this.encoding = detectedEncoding;
			}
		}
		protected virtual void FixBordersAndParagraphBetweenTables(PieceTable pieceTable) {
			for (int nestedLevel = 0; ; nestedLevel++) {
				List<Table> tables = GetTablesByLevel(pieceTable, nestedLevel);
				if (tables.Count <= 0)
					return;
				tables.Sort(TableComparer);
				FixBordersAndParagraphBetweenTablesCore(tables);
			}
		}
		protected virtual void FixBordersAndParagraphBetweenTablesCore(List<Table> tables) {
			int count = tables.Count;
			for (int i = 0; i < count - 1; i++) {
				if (tables[i].EndParagraphIndex + 1 == tables[i + 1].StartParagraphIndex)
					InsertParagraphBeforeTable(tables[i + 1]);
				ResolveBorderConflicts(tables[i]);
			}
			ResolveBorderConflicts(tables[count - 1]);
		}
		protected virtual void ResolveBorderConflicts(Table table) {
			if (table.CellSpacing.Value > 0)
				return;
			int rowCount = table.Rows.Count;
			TableBorders tableBorders = table.TableProperties.Borders;
			for (int i = 0; i < rowCount; i++) {
				TableRow row = table.Rows[i];
				TableCellCollection cells = row.Cells;
				TableCell firstCell = cells.First;
				TableCellBorders cellBorders = firstCell.Properties.Borders;
				if (ShouldResetTableCellBorder(tableBorders.UseLeftBorder, cellBorders.UseLeftBorder, tableBorders.LeftBorder, cellBorders.LeftBorder)) {
					cellBorders.LeftBorder.ResetBorder();
				}
				TableCell lastCell = cells.Last;
				cellBorders = lastCell.Properties.Borders;
				if (ShouldResetTableCellBorder(tableBorders.UseRightBorder, cellBorders.UseRightBorder, tableBorders.RightBorder, cellBorders.RightBorder)) {
					cellBorders.RightBorder.ResetBorder();
				}
				if (i == 0) {
					int cellCount = cells.Count;
					for (int j = 0; j < cellCount; j++) {
						cellBorders = cells[j].Properties.Borders;
						if (ShouldResetTableCellBorder(tableBorders.UseTopBorder, cellBorders.UseTopBorder, tableBorders.TopBorder, cellBorders.TopBorder)) {
							cellBorders.TopBorder.ResetBorder();
						}
					}
				}
				if (i == rowCount - 1) {
					int cellCount = cells.Count;
					for (int j = 0; j < cellCount; j++) {
						cellBorders = cells[j].Properties.Borders;
						if (ShouldResetTableCellBorder(tableBorders.UseBottomBorder, cellBorders.UseBottomBorder, tableBorders.BottomBorder, cellBorders.BottomBorder)) {
							cellBorders.BottomBorder.ResetBorder();
						}
					}
				}
			}
		}
		protected virtual bool ShouldResetTableCellBorder(bool useTableBorder, bool useCellBorder, BorderBase tableBorder, BorderBase cellBorder) {
			if (!useTableBorder || !useCellBorder)
				return false;
			bool hasTableBorder = tableBorder.Style != BorderLineStyle.None && tableBorder.Style != BorderLineStyle.Nil;
			bool hasCellBorder = cellBorder.Style != BorderLineStyle.None && cellBorder.Style != BorderLineStyle.Nil;
			return !hasCellBorder && hasTableBorder;
		}
		void InsertParagraphBeforeTable(Table table) {
			PieceTable pieceTable = table.PieceTable;
			Paragraph paragraph = pieceTable.Paragraphs[table.StartParagraphIndex];
			TableCell paragraphCell = paragraph.GetCell();
			Paragraph newParagraph = pieceTable.InsertParagraph(paragraph.LogPosition);
			ShiftBookmarks(newParagraph.LogPosition);
			while (paragraphCell != null && paragraphCell.Table.NestedLevel >= table.NestedLevel) {
				pieceTable.ChangeCellStartParagraphIndex(paragraphCell, newParagraph.Index + 1);
				paragraphCell = paragraphCell.Table.ParentCell;
			}
			if (newParagraph.IsInList())
				pieceTable.RemoveNumberingFromParagraph(newParagraph);
			newParagraph.ParagraphProperties.ResetAllUse();
			DocumentModelPosition paragraphMarkPos = DocumentModelPosition.FromParagraphStart(PieceTable, newParagraph.Index);
			pieceTable.Runs[paragraphMarkPos.RunIndex].CharacterProperties.ResetAllUse();
			pieceTable.Runs[paragraphMarkPos.RunIndex].Hidden = true;
		}
		void ShiftBookmarks(DocumentLogPosition logPosition) {
			int count = PieceTable.Bookmarks.Count;
			for (int i = 0; i < count; i++) {
				Bookmark bookmark = PieceTable.Bookmarks[i];
				if (logPosition <= bookmark.Start)
					bookmark.Interval.Start.LogPosition++;
				if (logPosition <= bookmark.End)
					bookmark.Interval.End.LogPosition++;
			}
		}
		int TableComparer(Table t1, Table t2) {
			return t1.StartParagraphIndex - t2.StartParagraphIndex;
		}
		protected virtual List<Table> GetTablesByLevel(PieceTable pieceTable, int nestedLevel) {
			List<Table> result = new List<Table>();
			TableCollection tables = pieceTable.Tables;
			int count = tables.Count;
			for (int i = 0; i < count; i++) {
				if (tables[i].NestedLevel == nestedLevel)
					result.Add(tables[i]);
			}
			return result;
		}
		protected virtual void ProcessRemainTags() {
			int count = TagsStack.Count;
			if (count == 0)
				return;
			bool tablesAllowed = DocumentModel.DocumentCapabilities.TablesAllowed;
			for (int i = count - 1; i >= 0; i--) {
				ProcessAnchorTag(TagsStack[i].Tag);
				if (tablesAllowed)
					ProcessUnclosedTableTags(i);
			}
		}
		void ProcessUnclosedTableTags(int index) {
			TagBase tag = TagsStack[index].Tag;
			if (tag is TrTag || tag is TdTag || tag is ThTag)
				tag.BeforeDeleteTagFromStack(index);
			else if (tag is TableTag) {
				tag.BeforeDeleteTagFromStack(index);
				SetAppendObjectProperty();
				tag.ParagraphFunctionalProcess();
			}
		}
		void ProcessAnchorTag(TagBase htmlTag) {
			if (htmlTag is AnchorTag)
				htmlTag.BeforeDeleteTagFromStack(-1);
		}
		protected internal void ClearTagStack() {
			TagsStack.Clear();
		}
		protected internal void AddTagToStack(OpenHtmlTag tag) {
			if (tag.Tag is PreformattedTag)
				hasPreformattedTagInStack = true;
			if (tag.Tag.CanAppendToTagStack)
				TagsStack.Add(tag);
		}
		protected internal void RemoveTagFromStack(int index) {
			bool recalculateHasPreformattedTagInStack = (hasPreformattedTagInStack && TagsStack[index].Tag is PreformattedTag);
			TagsStack.RemoveAt(index);
			if (recalculateHasPreformattedTagInStack) {
				hasPreformattedTagInStack = false;
				for (int i = TagsStack.Count - 1; i >= 0; i--) {
					if (TagsStack[i].Tag is PreformattedTag) {
						hasPreformattedTagInStack = true;
						break;
					}
				}
			}
		}
		protected internal void StyleTagIsNotOpen(StreamReader streamReader, List<HtmlElement> htmlElements) {
			if (element.ElementType == HtmlElementType.OpenTag || element.ElementType == HtmlElementType.EmptyTag) {
				Tag htmlTag = (Tag)element;
				if (htmlTag.NameID == HtmlTagNameID.Style) {
					styleTagIsOpen = true;
					return;
				}
				else if (element.ElementType == HtmlElementType.OpenTag && htmlTag.NameID == HtmlTagNameID.Script) {
					HtmlElement scriptContent;
					HtmlElement closeScriptTag;
					parser.ParseNextScript(streamReader, out scriptContent, out closeScriptTag);
					if (closeScriptTag != null)
						htmlElements.Add(closeScriptTag);
					return;
				}
			}
			htmlElements.Add(element);
		}
		protected internal string StyleTagIsOpen(string rawText) {
			if (element.ElementType == HtmlElementType.CloseTag) {
				Tag htmlTag = (Tag)element;
				if (htmlTag.NameID == HtmlTagNameID.Style) {
					styleTagIsOpen = false;
					ParseCssElementCollection(rawText);
					return String.Empty;
				}
			}
			if (element.ElementType == HtmlElementType.Comment)
				rawText += ((Comment)element).CommentText;
			else
				rawText += element.RawText;
			return DecodeStringContent(rawText);
		}
		protected internal virtual void ParseCssElementCollection(string rawText) {
			if (String.IsNullOrEmpty(rawText))
				return;
			using (StringReader reader = new StringReader(rawText)) {
				ParseCssElementCollection(reader);
			}
		}
		protected internal void ParseCssElementCollection(TextReader reader) {
			if (reader == null)
				return;
			CssParser cssParser = new CssParser(DocumentModel);
			try {
				styleTagCollection.AddRange(cssParser.Parse(reader));
			}
			catch {
			}
		}
		protected internal virtual void ElementsImport(List<HtmlElement> htmlElements) {
			bool htmlTagPresent;
			HtmlCorrector htmlCorrector = new HtmlCorrector();
			htmlElements = htmlCorrector.GetCorrectedHtmlElements(htmlElements, out htmlTagPresent);
			int count = htmlElements.Count;
			ProgressIndication.Begin(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_Loading), 0, count, 0);
			try {
				if (!htmlTagPresent) {
					element = new Tag(HtmlElementType.OpenTag) { NameID = HtmlTagNameID.Html };
					ElementsImportCore();						
				}
				for (int i = 0; i < count; i++) {
					element = htmlElements[i];
					ElementsImportCore();
					ProgressIndication.SetProgress(i);
				}
			}
			finally {
				ProgressIndication.End();
			}
		}
		protected internal virtual void ElementsImportCore() {
			switch (element.ElementType) {
				case HtmlElementType.Content:
					ProcessContentText();
					break;
				case HtmlElementType.Comment:
					string commentText = ((Comment)element).CommentText;
					if (this.markTable.ContainsKey(commentText)) {
						DocumentLogPosition logPosition = Position.LogPosition;
						if (IsEmptyParagraph) {
							ParagraphIndex prevParIndex = Position.ParagraphIndex - 1;
							if (prevParIndex >= ParagraphIndex.Zero && PieceTable.Paragraphs[prevParIndex].GetCell() == null)
								logPosition--;
						}
						Bookmark bm = new Bookmark(Position.PieceTable, logPosition, logPosition);
						bm.Name = "_dx_frag_" + commentText;
						PieceTable.Bookmarks.Add(bm);
						markTable[commentText] = bm;
					}
					break;
				default:
					FindKeywordInTagTable();
					break;
			}
		}
		protected internal virtual void FixLastParagraph() {
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			TextRunCollection runs = PieceTable.Runs;
			SectionCollection sections = DocumentModel.Sections;
			for (int i = 0; i < 2; i++) {
				Paragraph lastParagraph = paragraphs.Last;
				if (lastParagraph.IsEmpty && lastParagraph.Index > new ParagraphIndex(0)
					&& paragraphs[lastParagraph.Index - 1].GetCell() == null) {
					ParagraphIndex lastParagraphIndex = lastParagraph.Index;
					paragraphs.RemoveAt(lastParagraphIndex);
					runs.RemoveAt(new RunIndex(runs.Count - 1));
					Section lastSection = sections.Last;
					if (lastSection.FirstParagraphIndex == lastParagraphIndex) {
						SectionIndex sectionIndex = new SectionIndex(sections.Count - 1);
						sections[sectionIndex].UnsubscribeHeadersFootersEvents();
						sections.RemoveAt(sectionIndex);
					}
					else
						lastSection.LastParagraphIndex--;
				}
				else
					runs.Last.CharacterProperties.CopyFrom(Position.CharacterFormatting);
			}
			BookmarkCollection bookmarks = PieceTable.Bookmarks;
			DocumentLogPosition endLogPosition = PieceTable.DocumentEndLogPosition;
			int count = bookmarks.Count;
			DocumentModelPosition lastPosition = PositionConverter.ToDocumentModelPosition(PieceTable, endLogPosition);
			for (int i = 0; i < count; i++) {
				Bookmark bookmark = bookmarks[i];
				if (bookmark.Start > endLogPosition) 
					bookmark.SetStartCore(lastPosition);
				if (bookmark.End > endLogPosition)
					bookmark.SetEndCore(lastPosition);
			}
		}
		protected internal virtual void FindKeywordInTagTable() {
			Tag htmlTag = (Tag)element;
			HtmlTranslateKeywordHandler translator = null;
			if (htmlTag.NameID != HtmlTagNameID.Unknown)
				HtmlKeywordTable.TryGetValue(htmlTag.NameID, out translator);
			if (translator != null) {
				tag = translator(this);
				ProcessTag(tag);
			}
		}
		protected internal virtual void ProcessContentText() {
			string contentText = GetContentText();
			if (String.IsNullOrEmpty(contentText))
				return;
			AppendContentText(contentText, UseRawContent());
		}
		public virtual void AppendContentText(string text, bool useRawText) {
			string insertedText = String.Empty;
			if (useRawText) {
				insertedText = Options.ReplaceSpaceWithNonBreakingSpaceInsidePre ? text.Replace(Characters.Space, Characters.NonBreakingSpace) : text;
				AppendPlainText(insertedText);
			}
			else {
				insertedText = tag is LevelTag ? text.TrimStart(Characters.Space) : text;
				AppendText(insertedText);
			}
			this.canInsertSpace = !text.EndsWith(" ");
		}
		protected internal virtual string GetContentText() {
			if (IgnoredTagIsOpen)
				return String.Empty;
			Content content = (Content)element;
			string contentText;
			if (UseRawContent())
				contentText = content.RawText;
			else {
				contentText = content.ContentText;
				if (!String.IsNullOrEmpty(contentText) && WhiteSpaceAtStartCanRemove(contentText))
					contentText = contentText.Remove(0, 1);
			}
			return DecodeStringContent(contentText);
		}
		protected internal string DecodeStringContent(string contentText) {
			if (String.IsNullOrEmpty(contentText))
				return String.Empty;
			if (!suppressEncoding)
				contentText = HtmlCodePageDecoder.ApplyEncoding(contentText, Encoding);
			return ReplaceSpecialSymbols(contentText);
		}
		protected internal virtual bool UseRawContent() {
			return hasPreformattedTagInStack;
		}
		protected internal string ReplaceSpecialSymbols(string contentText) {
			string[] contentTexts = contentText.Split('&');
			contentText = contentTexts[0];
			for (int i = 1; i < contentTexts.Length; i++) {
				if (!String.IsNullOrEmpty(contentTexts[i])) {
					string replacedString = ReplaceSpecialSymbolCore(contentTexts[i]);
					if (contentTexts[i] == replacedString)
						contentText += '&';
					contentText += replacedString;
				}
			}
			return contentText.Replace(new String(Characters.OptionalHyphen, 1), String.Empty);
		}
		protected internal string ReplaceSpecialSymbolCore(string rawText) {
			string specialSymbol = String.Empty;
			bool isNumeric = false;
			if (rawText[0] == '#') {
				isNumeric = true;
				rawText = rawText.Remove(0, 1);
			}
			int i = 0;
			while (i < rawText.Length && !IsSeparator(rawText[i])) {
				specialSymbol += rawText[i];
				i++;
			}
			if (i < rawText.Length && rawText[i] == ';')
				rawText = rawText.Remove(i, 1);
			char specialValue = GetSpecialValue(specialSymbol, isNumeric);
			if (specialValue != Char.MinValue) {
				rawText = rawText.Remove(0, i);
				rawText = rawText.Insert(0, specialValue.ToString());
			}
			return rawText;
		}
		protected internal virtual char GetSpecialValue(string specialSymbol, bool isNumeric) {
			char specialValue = Char.MinValue;
			if (isNumeric)
				specialValue = GetUnicodeSymbol(specialSymbol);
			else
				SpecialSymbolTable.TryGetValue(specialSymbol, out specialValue);
			return specialValue;
		}
		protected internal char GetUnicodeSymbol(string specialSymbol) {
			int code;
			if (!TryParseSymbol(specialSymbol, out code))
				return Char.MinValue;
#if !SL
			if (code <= byte.MaxValue) {
				Encoding encoding = DXEncoding.GetEncoding(1252);
				string result = encoding.GetString(new byte[] { (byte)code });
				if (!String.IsNullOrEmpty(result))
					return result[0];
			}
#endif
			return (char)code;
		}
		bool TryParseSymbol(string specialSymbol, out int code) {
			if (specialSymbol.StartsWith("x", StringComparison.CurrentCultureIgnoreCase))
				return Int32.TryParse(specialSymbol.Remove(0, 1), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out code);
			else
				return Int32.TryParse(specialSymbol, out code);
		}
		bool IsSeparator(char ch) {
			return Char.IsWhiteSpace(ch) || Char.IsPunctuation(ch) || Char.IsSymbol(ch);
		}
		private bool WhiteSpaceAtStartCanRemove(string contentText) {
			return Char.IsWhiteSpace(contentText[0]) && (IsEmptyParagraph || IsEmptyLine || !canInsertSpace);
		}
		protected internal virtual void ProcessTag(TagBase tag) {
			tag.DeleteOldOpenTag();
			if (element.ElementType == HtmlElementType.OpenTag)
				OpenProcess(tag);
			else if (element.ElementType == HtmlElementType.CloseTag)
				CloseProcess(tag);
			else
				EmptyProcess(tag);
		}
		protected internal virtual void OpenProcess(TagBase tag) {
			if (IgnoredTagIsOpen)
				return;
			OpenHtmlTag openTag = new OpenHtmlTag(tag, PieceTable);
			if (tag is ParagraphTag && LastOpenParagraphTagIndex >= 0)
				OpenTagIsFoundAndRemoved(new ParagraphTag(this));
			else if(tag is AnchorTag && LastOpenAnchorTagIndex >= 0)
				OpenTagIsFoundAndRemoved(new AnchorTag(this));
			openTag.OldPosition.CopyFrom(Position);
			AddTagToStack(openTag);
			tag.OpenTagProcess();
		}
		protected internal virtual void CloseProcess(TagBase tag) {
			if (OpenTagIsFoundAndRemoved(tag))
				return;
			tag.FunctionalTagProcess();
		}
		protected internal bool OpenTagIsFoundAndRemoved(TagBase tag) {
			int startIndex = tag.GetStartIndexAllowedSearchScope();
			int count = TagsStack.Count;
			for (int i = count - 1; i >= startIndex; i--) {
				if (TagsStack[i].Tag.Name == tag.Name) {
					CloseUnClosedTag(tag, i);
					return true;
				}
			}
			return false;
		}
		protected internal virtual void CloseUnClosedTag(TagBase tag, int index) {
			tag.BeforeDeleteTagFromStack(index);
			Position.CopyFrom(TagsStack[index].OldPosition);
			DeleteOpenTagFromStack(index);
			tag.FunctionalTagProcess();
			ApplyProperties(index);
		}
		protected internal void DeleteOpenTagFromStack(int index) {
			if (index < LastOpenParagraphTagIndex)
				LastOpenParagraphTagIndex--;
			if (index < LastOpenAnchorTagIndex)
				LastOpenAnchorTagIndex--;					
			RemoveTagFromStack(index);
		}
		protected internal virtual void ApplyProperties(int index) {
			for (int i = index; i < TagsStack.Count; i++) {
				if (!TagsStack[i].Tag.ApplyStylesToInnerHtml)
					continue;
				TagsStack[i].OldPosition.CopyFrom(Position);
				TagsStack[i].Tag.ApplyProperties();
			}
		}
		protected internal virtual void EmptyProcess(TagBase tag) {
			if (IgnoredTagIsOpen)
				return;
			tag.EmptyTagProcess();
		}
		protected internal virtual void ApplyPreviousCharacterProperties(RunIndex index) {
			int count = TagsStack.Count;
			TextRunBase run = PieceTable.Runs[index];
			if (count > 0)
				run.CharacterProperties.CopyFrom(TagsStack[count - 1].OldPosition.CharacterFormatting);
		}
		Dictionary<string, UriBasedOfficeImage> uniqueUriBasedImages = new Dictionary<string, UriBasedOfficeImage>();
		protected internal UriBasedOfficeImageBase CreateUriBasedRichEditImage(string uri, int pixelTargetWidth, int pixelTargetHeight) {
			UriBasedOfficeImage result;
			if (uniqueUriBasedImages.TryGetValue(uri, out result))
				return new UriBasedOfficeReferenceImage(result, pixelTargetWidth, pixelTargetHeight);
			return CreateUriBasedRichEditImageCore(uri, pixelTargetWidth, pixelTargetHeight);
		}
		UriBasedOfficeImageBase CreateUriBasedRichEditImageCore(string uri, int pixelTargetWidth, int pixelTargetHeight) {
			bool asyncImageLoading = Options.AsyncImageLoading;
			if (asyncImageLoading && DataStringUriStreamProvider != null && DataStringUriStreamProvider.IsUriSupported(uri))
				asyncImageLoading = false;
			UriBasedOfficeImage image = new UriBasedOfficeImage(uri, pixelTargetWidth, pixelTargetHeight, DocumentModel, asyncImageLoading);
			uniqueUriBasedImages.Add(uri, image);
			return image;
		}
		public virtual void AppendText(string text) {
			if (String.IsNullOrEmpty(text))
				return;
			AppendText(PieceTable, text);
		}
		protected internal virtual void AppendText(PieceTable pieceTable, string text) {
			FontSizeInfo info = ApplyPositionScriptFontSize();
			try {
				pieceTable.InsertTextCore(Position, text);
			}
			finally {
				RestorePositionScriptFontSize(info);
			}
			SetAppendObjectProperty();
		}
		protected internal virtual void AppendPlainText(string text) {
			FontSizeInfo info = ApplyPositionScriptFontSize();
			try {
				PieceTable.InsertPlainText(Position, text);
			}
			finally {
				RestorePositionScriptFontSize(info);
			}
			SetAppendObjectProperty();
		}
		protected internal virtual void AppendInlineImage(OfficeImage image, float scaleX, float scaleY, Size desiredSize) {
			FontSizeInfo info = ApplyPositionScriptFontSize();
			try {
				PieceTable.AppendImage(Position, image, scaleX, scaleY, true);
			}
			finally {
				RestorePositionScriptFontSize(info);
			}
			SetAppendObjectProperty();
		}
		protected internal virtual void AppendParagraph() {
			FontSizeInfo info = ApplyPositionScriptFontSize();
			try {
				PieceTable.InsertParagraphCore(Position);
			}
			finally {
				RestorePositionScriptFontSize(info);
			}
		}
		protected internal void SetAppendObjectProperty() {
			IsEmptyParagraph = false;
			IsEmptyLine = false;
		}
		protected internal virtual FontSizeInfo ApplyPositionScriptFontSize() {
			CharacterFormattingBase formatting = Position.CharacterFormatting;
			FontSizeInfo info = new FontSizeInfo(formatting.DoubleFontSize, formatting.Options.UseDoubleFontSize);
			return info;
		}
		protected internal virtual void RestorePositionScriptFontSize(FontSizeInfo fontSizeInfo) {
			CharacterFormattingBase formatting = Position.CharacterFormatting;
			if (formatting.DoubleFontSize != fontSizeInfo.DoubleFontSize)
				formatting.DoubleFontSize = fontSizeInfo.DoubleFontSize;
			if (!fontSizeInfo.UseFontSize)
				formatting.ResetUse(CharacterFormattingOptions.Mask.UseDoubleFontSize);
		}
		public virtual HtmlTagNameID GetTagNameID(string name) {
			return Parser.GetTagNameID(name);
		}
		protected internal virtual HtmlParser CreateHtmlParser() {
			return new HtmlParser();
		}
		protected internal virtual void ValidateHyperlinkInfo(HyperlinkInfo hyperlinkInfo) {
		}
		protected internal virtual void ProcessHyperlinkStart(HyperlinkInfo hyperlinkInfo) {
			if (!DocumentFormatsHelper.ShouldInsertHyperlink(DocumentModel))
				return;
			ValidateHyperlinkInfo(hyperlinkInfo);
			ImportFieldInfo info = new ImportFieldInfo(PieceTable);
			ImportFieldHelper importFieldHelper = new ImportFieldHelper(PieceTable);
			importFieldHelper.ProcessFieldBegin(info, Position);
			importFieldHelper.InsertHyperlinkInstruction(hyperlinkInfo, Position);
			importFieldHelper.ProcessFieldSeparator(info, Position);
			ProcessHyperlink = info;
		}
		protected internal virtual void ProcessHyperlinkEnd() {
			if (!DocumentFormatsHelper.ShouldInsertHyperlink(DocumentModel))
				return;
			ImportFieldInfo currentInfo = ProcessHyperlink;
			ImportFieldHelper importFieldHelper = new ImportFieldHelper(PieceTable);
			Field field = importFieldHelper.ProcessFieldEnd(currentInfo, Position);
			if (currentInfo.CodeEndIndex + 1 == currentInfo.ResultEndIndex)
				emptyHyperlinks.Add(field);
			ProcessHyperlink = null;
		}
		protected virtual string ValidateBookmarkName(string anchorName) {
			return anchorName;
		}
		protected internal virtual void ProcessBookmarkStart(string anchorName) {
			HtmlBookmarkInfo bookmarkInfo = new HtmlBookmarkInfo();
			bookmarkInfo.Name = ValidateBookmarkName(anchorName);
			bookmarkInfo.Start = Position.LogPosition;
			ProcessBookmarks.Push(bookmarkInfo);
		}
		protected internal virtual void ProcessBookmarkEnd() {
			HtmlBookmarkInfo bookmarkInfo = ProcessBookmarks.Pop();
			bookmarkInfo.End = Position.LogPosition;
			if (DocumentModel.DocumentCapabilities.BookmarksAllowed)
				CreateBookmark(bookmarkInfo);
		}
		void CreateBookmark(HtmlBookmarkInfo bookmarkInfo) {
			if (bookmarkInfo.Validate(PieceTable)) {
				int length = bookmarkInfo.End - bookmarkInfo.Start;
				PieceTable.CreateBookmarkCore(bookmarkInfo.Start, length, bookmarkInfo.Name);
			}
		}
		public override void ThrowInvalidFile() {
			throw new ArgumentException("Invalid HTML file");
		}
		public void RegisterCommentMarksToCollectPositions(params string[] marks) {
			foreach (string mark in marks)
				this.markTable.Add(mark, null);
		}
		public DocumentLogPosition GetMarkPosition(string mark) {
			Bookmark result;
			if (this.markTable.TryGetValue(mark, out result))
				return result != null ? result.Start : new DocumentLogPosition(-1);
			return new DocumentLogPosition(-1);
		}
	}
	#endregion
	#region FontSizeInfo
	public class FontSizeInfo {
		int doubleFontSize;
		bool useFontSize;
		public FontSizeInfo(int doubleFontSize, bool useFontSize) {
			this.doubleFontSize = doubleFontSize;
			this.useFontSize = useFontSize;
		}
		public int DoubleFontSize { get { return doubleFontSize; } }
		public bool UseFontSize { get { return useFontSize; } }
	}
	#endregion
	#region HtmlCorrector
	public class HtmlCorrector {
		List<HtmlCorrectorStateBase> states;
		HtmlCorrectorTableStateBase lastOpenedTableState;
		public HtmlCorrector() {
		}
		public static string GetText(List<HtmlElement> elements) {
			String result = "";
			foreach (HtmlElement element in elements) {
				result += element.RawText.Trim();
			}
			return result;
		}
		protected internal HtmlCorrectorStateBase State { get { return states[states.Count - 1]; } }
		public List<HtmlElement> GetCorrectedHtmlElements(List<HtmlElement> htmlElements, out bool htmlTagPresent) {
			this.states = new List<HtmlCorrectorStateBase>();
			List<HtmlElement> output = new List<HtmlElement>(htmlElements.Count);
			states.Add(new HtmlCorrectorDefaultState(this, output));
			Comment startComment = null;
			Comment endComment = null;
			HtmlElement beforeStartComment = null;
			HtmlElement afterEndComment = null;
			int count = htmlElements.Count;
			htmlTagPresent = false;
			for (int i = 0; i < count; i++) {
				HtmlElement element = htmlElements[i];
				Comment comment = element as Comment;
				if (comment != null) {
					if (comment.CommentText == "StartFragment") {
						startComment = comment;
						beforeStartComment = CalculateHtmlElementBeforeComment(comment, htmlElements, i);
					}
					if (comment.CommentText == "EndFragment") {
						endComment = comment;
						afterEndComment = CalculateHtmlElementAfterComment(comment, htmlElements, i);
					}
				}
				else {
					if(!htmlTagPresent && element.ElementType == HtmlElementType.OpenTag) {
					Tag tag = element as Tag;
					if (tag != null && tag.NameID == HtmlTagNameID.Html)
						htmlTagPresent = true;
					}
					Process(element);
				}
			}
			ProcessEnd();
			if (startComment != null)
				InsertStartComment(startComment, output, beforeStartComment);
			if (endComment != null)
				InsertEndComment(endComment, output, afterEndComment);
			return output;
		}
		HtmlElement CalculateHtmlElementBeforeComment(Comment comment, List<HtmlElement> list, int index) {
			for (int i = index - 1 ; i >= 0; i--)
					if (!HtmlCorrectorStateBase.IsWhiteSpaceElement(list[i]))
						return list[i];
			return null;
		}
		HtmlElement CalculateHtmlElementAfterComment(Comment comment, List<HtmlElement> list, int index) {
			int count = list.Count;
			for (int i = index + 1; i < count; i++)
					if (!HtmlCorrectorStateBase.IsWhiteSpaceElement(list[i]))
						return list[i];
			return null;
		}
		void InsertStartComment(Comment newComment, List<HtmlElement> output, HtmlElement beforeComment) {
			if ((beforeComment != null)) {
				output.Insert(output.IndexOf(beforeComment) + 1, newComment);
			}
			else
				output.Insert(0, newComment);
		}
		void InsertEndComment(Comment newComment, List<HtmlElement> output, HtmlElement afterComment) {
			if ((afterComment != null)) {
				output.Insert(output.IndexOf(afterComment), newComment);
			}
			else
				output.Add(newComment);
		} 
		public void StartNewTable(List<HtmlElement> output) {
			HtmlCorrectorTableState newState = new HtmlCorrectorTableState(this, output);
			states.Add(newState);
			lastOpenedTableState = newState;
		}
		public void StartNewRow(List<HtmlElement> output, bool firstRow) {
			HtmlCorrectorTableRowState newState = new HtmlCorrectorTableRowState(this, output, firstRow);
			states.Add(newState);
		}
		public void StartNewCell(List<HtmlElement> output) {
			HtmlCorrectorTableCellState newState = new HtmlCorrectorTableCellState(this, output);
			states.Add(newState);
			lastOpenedTableState.OnStartCell(output);
		}
		public void StartCaption(List<HtmlElement> output) {
			HtmlCorrectorTableCaptionState newState = new HtmlCorrectorTableCaptionState(this, output);
			states.Add(newState);
		}
		public void StartTbody(List<HtmlElement> output) {
			HtmlCorrectorTableBodyState newState = new HtmlCorrectorTableBodyState(this, output);
			states.Add(newState);
			lastOpenedTableState = newState;
		}
		public void StartThead(List<HtmlElement> output) {
			HtmlCorrectorTableHeaderState newState = new HtmlCorrectorTableHeaderState(this, output);
			states.Add(newState);
			lastOpenedTableState = newState;
		}
		public void StartTfoot(List<HtmlElement> output) {
			HtmlCorrectorTableFooterState newState = new HtmlCorrectorTableFooterState(this, output);
			states.Add(newState);
			lastOpenedTableState = newState;
		}
		protected internal virtual void Process(HtmlElement element) {
			State.Process(element);
		}
		public void ProcessEnd() {
			State.ProcessEnd();
		}
		public void AddMisplacedCellContent(List<HtmlElement> elements) {
			lastOpenedTableState.AddMisplacedCellContent(elements);
		}
		public void AddMisplacedCellContent(HtmlElement element) {
			lastOpenedTableState.AddMisplacedCellContent(element);
		}
		public void ForceOpenTableCell() {
			Tag tag = new Tag(HtmlElementType.OpenTag);
			tag.NameID = HtmlTagNameID.TD;
			Process(tag);
		}
		public void ForceCloseTableCell() {
			Tag tag = new Tag(HtmlElementType.CloseTag);
			tag.NameID = HtmlTagNameID.TD;
			Process(tag);
		}
		public void ForceOpenTableRow() {
			Tag tag = new Tag(HtmlElementType.OpenTag);
			tag.NameID = HtmlTagNameID.TR;
			Process(tag);
		}
		public void ForceCloseTableRow() {
			Tag tag = new Tag(HtmlElementType.CloseTag);
			tag.NameID = HtmlTagNameID.TR;
			Process(tag);
		}
		internal void ForceCloseTable() {
			Tag tag = new Tag(HtmlElementType.CloseTag);
			tag.NameID = HtmlTagNameID.Table;
			Process(tag);
		}
		internal void ForceOpenCaption() {
			Tag tag = new Tag(HtmlElementType.OpenTag);
			tag.NameID = HtmlTagNameID.Caption;
			Process(tag);
		}
		protected internal virtual void ForceOpenTag(HtmlTagNameID nameID) {
			Tag tag = new Tag(HtmlElementType.OpenTag);
			tag.NameID = nameID;
			Process(tag);
		}
		protected internal virtual void ForceCloseTag(HtmlTagNameID nameID) {
			Tag tag = new Tag(HtmlElementType.CloseTag);
			tag.NameID = nameID;
			Process(tag);
		}
		internal void ForceCloseCaption() {
			Tag tag = new Tag(HtmlElementType.CloseTag);
			tag.NameID = HtmlTagNameID.Caption;
			Process(tag);
		}
		public void ReturnToPrevStateFromCellState(int lastCellPosition) {
			Debug.Assert(State is HtmlCorrectorTableCellState);
			lastOpenedTableState.OnEndCell(lastCellPosition);
			ReturnToPrevState();
		}
		public void ReturnToPrevState() {
			states.RemoveAt(states.Count - 1);
			if (states.Count == 1)
				 lastOpenedTableState = null;
			else {
				if (State is HtmlCorrectorTableCellState) {
					Debug.Assert(states.Count > 3 && states[states.Count - 3] is HtmlCorrectorTableStateBase);
					lastOpenedTableState = (HtmlCorrectorTableStateBase)states[states.Count - 3];
				}
				else if (State is HtmlCorrectorTableRowState) {
					Debug.Assert(states.Count > 2 && states[states.Count - 2] is HtmlCorrectorTableStateBase);
					lastOpenedTableState = (HtmlCorrectorTableStateBase)states[states.Count - 2];
				}
				else if (State is HtmlCorrectorTableState) {
					Debug.Assert(states.Count > 1 && states[states.Count - 1] is HtmlCorrectorTableStateBase);
					lastOpenedTableState = (HtmlCorrectorTableStateBase)states[states.Count - 1];
				}
			}
		}
	}
	#endregion
	#region HtmlCorrectorStateBase (abstract class)
	public abstract class HtmlCorrectorStateBase {
		readonly HtmlCorrector corrector;
		readonly List<HtmlElement> output;
		protected HtmlCorrectorStateBase(HtmlCorrector corrector, List<HtmlElement> output) {
			this.corrector = corrector;
			this.output = output;
		}
		public HtmlCorrector Corrector { get { return corrector; } }
		public List<HtmlElement> CurrentOutput { get { return output; } }
		protected abstract void ProcessTableOpenTag(HtmlElement element);
		protected abstract void ProcessTableCloseTag(HtmlElement element);
		protected abstract void ProcessTableRowOpenTag(HtmlElement element);
		protected abstract void ProcessTableRowCloseTag(HtmlElement element);
		protected abstract void ProcessTableCellOpenTag(HtmlElement element);
		protected abstract void ProcessTableCellCloseTag(HtmlElement element);
		protected abstract void ProcessNonTableElement(HtmlElement element);
		protected abstract void ProcessCaptionOpenTag(HtmlElement element);
		protected abstract void ProcessCaptionCloseTag(HtmlElement element);
		public abstract void ProcessEnd();
		protected virtual void ProcessTableHeaderCellOpenTag(HtmlElement element) {
			ProcessTableCellOpenTag(element);
		}
		protected virtual void ProcessTableHeaderCellCloseTag(HtmlElement element) {
			ProcessTableCellCloseTag(element);
		}
		protected virtual void ProcessTheadOpenTag(HtmlElement element) {
		}
		protected virtual void ProcessTheadCloseTag(HtmlElement element) {
		}
		protected virtual void ProcessTfootOpenTag(HtmlElement element) {
		}
		protected virtual void ProcessTfootCloseTag(HtmlElement element) {
		}
		protected virtual void ProcessTbodyOpenTag(HtmlElement element) {
		}
		protected virtual void ProcessTbodyCloseTag(HtmlElement element) {
		}
		protected virtual void ProcessColOpenTag(HtmlElement element) {
		}
		protected virtual void ProcessColCloseTag(HtmlElement element) {
		}
		protected virtual void ProcessColgroupOpenTag(HtmlElement element) {
		}
		protected virtual void ProcessColgroupCloseTag(HtmlElement element) {
		}
		protected virtual void ProcessWhiteSpaceElement(HtmlElement element) {
			CurrentOutput.Add(element);
		}
		public virtual void Process(HtmlElement htmlElement) {
			Tag tag = htmlElement as Tag;
			if (tag != null && ProcessTableTag(tag))
				return;
			else
				if (IsWhiteSpaceElement(htmlElement))
					ProcessWhiteSpaceElement(htmlElement);
				else
					ProcessNonTableElement(htmlElement);
		}
		protected virtual bool ProcessTableTag(Tag tag) {
			HtmlElementType elementType = tag.ElementType;
			bool isEmptyTag = elementType == HtmlElementType.EmptyTag;
			bool isOpenTag = elementType == HtmlElementType.OpenTag || isEmptyTag;
			bool isCloseTag = elementType == HtmlElementType.CloseTag || isEmptyTag;
			if (tag.NameID == HtmlTagNameID.Table) {
				if (isOpenTag)
					ProcessTableOpenTag(GetOpenTag(tag));
				if (isCloseTag)
					ProcessTableCloseTag(GetCloseTag(tag));
				return true;
			}
			if (tag.NameID == HtmlTagNameID.TR) {
				if (isOpenTag)
					ProcessTableRowOpenTag(GetOpenTag(tag));
				if (isCloseTag)
					ProcessTableRowCloseTag(GetCloseTag(tag));
				return true;
			}
			if (tag.NameID == HtmlTagNameID.TD) {
				if (isOpenTag)
					ProcessTableCellOpenTag(GetOpenTag(tag));
				if (isCloseTag)
					ProcessTableCellCloseTag(GetCloseTag(tag));
				return true;
			}
			if (tag.NameID == HtmlTagNameID.TH) {
				if (isOpenTag)
					ProcessTableHeaderCellOpenTag(GetOpenTag(tag));
				if (isCloseTag)
					ProcessTableHeaderCellCloseTag(GetCloseTag(tag));
				return true;
			}
			if (tag.NameID == HtmlTagNameID.Caption) {
				if (isOpenTag)
					ProcessCaptionOpenTag(GetOpenTag(tag));
				if (isCloseTag)
					ProcessCaptionCloseTag(GetCloseTag(tag));
				return true;
			}
			if (tag.NameID == HtmlTagNameID.Thead) {
				if (isOpenTag)
					ProcessTheadOpenTag(GetOpenTag(tag));
				if (isCloseTag)
					ProcessTheadCloseTag(GetCloseTag(tag));
				return true;
			}
			if (tag.NameID == HtmlTagNameID.Tfoot) {
				if (isOpenTag)
					ProcessTfootOpenTag(GetOpenTag(tag));
				if (isCloseTag)
					ProcessTfootCloseTag(GetCloseTag(tag));
				return true;
			}
			if (tag.NameID == HtmlTagNameID.Tbody) {
				if (isOpenTag)
					ProcessTbodyOpenTag(GetOpenTag(tag));
				if (isCloseTag)
					ProcessTbodyCloseTag(GetCloseTag(tag));
				return true;
			}
			if (tag.NameID == HtmlTagNameID.Col) {
				if (isOpenTag)
					ProcessColOpenTag(GetOpenTag(tag));
				if (isCloseTag)
					ProcessColCloseTag(GetCloseTag(tag));
				return true;
			}
			if (tag.NameID == HtmlTagNameID.ColGroup) {
				if (isOpenTag)
					ProcessColgroupOpenTag(GetOpenTag(tag));
				if (isCloseTag)
					ProcessColgroupCloseTag(GetCloseTag(tag));
				return true;
			}
			return false;
		}
		HtmlElement GetOpenTag(Tag tag) {
			if (tag.ElementType == HtmlElementType.EmptyTag) {
				Tag result = new Tag(HtmlElementType.OpenTag);
				result.CopyFrom(tag);
				return result;
			}
			else
				return tag;
		}
		HtmlElement GetCloseTag(Tag tag) {
			if (tag.ElementType == HtmlElementType.EmptyTag) {
				Tag result = new Tag(HtmlElementType.CloseTag);
				result.CopyFrom(tag);
				return result;
			}
			else
				return tag;
		}
		public static bool IsWhiteSpaceElement(HtmlElement element) {
			if (element.ElementType == HtmlElementType.Comment)
				return true;
			if (element.ElementType != HtmlElementType.Content)
				return false;
			string rawText = element.RawText;
			int count = rawText.Length;
			for (int i = 0; i < count; i++)
				if (!Char.IsWhiteSpace(rawText, i))
					return false;
			return true;
		}
	}
	#endregion
	#region HtmlCorrectorNoTableState
	public class HtmlCorrectorDefaultState : HtmlCorrectorStateBase {
		public HtmlCorrectorDefaultState(HtmlCorrector corrector, List<HtmlElement> output)
			: base(corrector, output) {
		}
		protected override void ProcessTableOpenTag(HtmlElement element) {
			CurrentOutput.Add(element);
			Corrector.StartNewTable(CurrentOutput);
		}
		protected override void ProcessTableCloseTag(HtmlElement element) {
		}
		protected override void ProcessNonTableElement(HtmlElement element) {
			CurrentOutput.Add(element);
		}
		protected override void ProcessTableRowOpenTag(HtmlElement element) {
		}
		protected override void ProcessTableRowCloseTag(HtmlElement element) {
		}
		protected override void ProcessTableCellOpenTag(HtmlElement element) {
		}
		protected override void ProcessTableCellCloseTag(HtmlElement element) {
		}
		protected override void ProcessCaptionOpenTag(HtmlElement element) {
		}
		protected override void ProcessCaptionCloseTag(HtmlElement element) {
		}
		public override void ProcessEnd() {
		}
	}
	#endregion
	#region HtmlCorrectorTableStateBase
	public abstract class HtmlCorrectorTableStateBase : HtmlCorrectorStateBase {
		List<HtmlElement> deferredCellContent;
		List<HtmlElement> innerOutput;
		int lastCellOutputPosition = -1;
		bool hasRows;
		protected HtmlCorrectorTableStateBase(HtmlCorrector corrector, List<HtmlElement> output)
			: base(corrector, output) {
			this.innerOutput = new List<HtmlElement>();
		}
		protected bool HasRows { get { return hasRows; } set { hasRows = value; } }
		protected List<HtmlElement> InnerOutput { get { return innerOutput; } }
		protected virtual void EnsureTableHasRows() {
			if (!HasRows) {
				Corrector.ForceOpenTableRow();
				Corrector.ForceCloseTableRow();
			}
		}
		protected override void ProcessTableOpenTag(HtmlElement element) {
			Corrector.ForceCloseTable();
			CurrentOutput.Add(element);
			Corrector.StartNewTable(CurrentOutput);
		}
		protected override void ProcessNonTableElement(HtmlElement element) {
			AddMisplacedCellContent(element);
		}
		protected virtual void AddInnerElementsToCurrentOutput() {
			CurrentOutput.AddRange(InnerOutput);
		}
		protected override void ProcessTableRowOpenTag(HtmlElement element) {
			innerOutput.Add(element);
			Corrector.StartNewRow(innerOutput, !HasRows);
			this.hasRows = true;
		}
		protected override void ProcessTableRowCloseTag(HtmlElement element) {
			if (HasRows)
				return;
			Corrector.ForceOpenTableRow();
			Corrector.Process(element);
		}
		protected override void ProcessTableCellOpenTag(HtmlElement element) {
			Corrector.ForceOpenTableRow();
			Corrector.Process(element);
		}
		protected override void ProcessTableCellCloseTag(HtmlElement element) {
			if (HasRows)
				return;
			Corrector.ForceOpenTableRow();
			Corrector.Process(element);
		}
		public void AddMisplacedCellContent(HtmlElement element) {
			if (lastCellOutputPosition == -1) {
				if (deferredCellContent == null)
					deferredCellContent = new List<HtmlElement>();
				deferredCellContent.Add(element);
			}
			else {
				innerOutput.Insert(lastCellOutputPosition, element);
				lastCellOutputPosition++;
			}
		}
		public void AddMisplacedCellContent(List<HtmlElement> elements) {
			int count = elements.Count;
			for (int i = 0; i < count; i++)
				AddMisplacedCellContent(elements[i]);
		}
		public void OnStartCell(List<HtmlElement> elements) {
			int outputPosition = 0;
			if (deferredCellContent != null) {
				for (int i = 0; i < deferredCellContent.Count; i++) {
					elements.Add(deferredCellContent[i]);
					outputPosition++;
				}
				deferredCellContent = null;
			}
			lastCellOutputPosition = outputPosition;
		}
		public void OnEndCell(int outputPosition) {
			lastCellOutputPosition = outputPosition;
		}
		public override void ProcessEnd() {
			Corrector.ForceCloseTable();
			Corrector.ProcessEnd();
		}
	}
	#endregion
	#region HtmlCorrectorTableState
	public class HtmlCorrectorTableState : HtmlCorrectorTableStateBase {
		List<HtmlElement> headerOutput;
		List<HtmlElement> footerOutput;
		List<HtmlElement> captionOutput;
		public HtmlCorrectorTableState(HtmlCorrector corrector, List<HtmlElement> output)
			: base(corrector, output) {
			this.captionOutput = new List<HtmlElement>();
			this.headerOutput = new List<HtmlElement>();
			this.footerOutput = new List<HtmlElement>();
		}
		protected override void ProcessTableCloseTag(HtmlElement element) {
			EnsureTableHasRows();
			CurrentOutput.AddRange(captionOutput);
			CurrentOutput.AddRange(this.headerOutput);
			CurrentOutput.AddRange(InnerOutput);
			CurrentOutput.AddRange(this.footerOutput);
			CurrentOutput.Add(element);
			Corrector.ReturnToPrevState();
			if (Corrector.State.CurrentOutput != CurrentOutput)
				Corrector.AddMisplacedCellContent(CurrentOutput);
		}
		protected override void ProcessCaptionOpenTag(HtmlElement element) {
			captionOutput.Add(element);
			Corrector.StartCaption(captionOutput);
			HasRows = true;
		}
		protected override void ProcessCaptionCloseTag(HtmlElement element) {
			Corrector.ForceOpenCaption();
			Corrector.Process(element);
		}
		protected override void ProcessTbodyOpenTag(HtmlElement element) {
			InnerOutput.Add(element);
			Corrector.StartTbody(InnerOutput);
			HasRows = true;
		}
		protected override void ProcessTheadOpenTag(HtmlElement element) {
			this.headerOutput.Add(element);
			Corrector.StartThead(this.headerOutput);
			HasRows = true;
		}
		protected override void ProcessTfootOpenTag(HtmlElement element) {
			this.footerOutput.Add(element);
			Corrector.StartTfoot(this.footerOutput);
			HasRows = true;
		}
	}
	#endregion
	#region HtmlCorrectorTableRowState
	public class HtmlCorrectorTableRowState : HtmlCorrectorStateBase {
		bool firstRow;
		bool hasCells;
		public HtmlCorrectorTableRowState(HtmlCorrector corrector, List<HtmlElement> output, bool firstRow)
			: base(corrector, output) {
			this.firstRow = firstRow;
		}
		protected override void ProcessTableOpenTag(HtmlElement element) {
			List<HtmlElement> output = new List<HtmlElement>();
			output.Add(element);
			Corrector.StartNewTable(output);
		}
		protected override void ProcessNonTableElement(HtmlElement element) {
			Corrector.AddMisplacedCellContent(element);
		}
		protected override void ProcessTableCloseTag(HtmlElement element) {
			Corrector.ForceCloseTableRow();
			Corrector.Process(element);
		}
		protected override void ProcessTableRowOpenTag(HtmlElement element) {
			Corrector.ForceCloseTableRow();
			Corrector.Process(element);
		}
		protected override void ProcessTableRowCloseTag(HtmlElement element) {
			if (!hasCells && firstRow) {
				Corrector.ForceOpenTableCell();
				Corrector.ForceCloseTableCell();
			}
			CurrentOutput.Add(element);
			Corrector.ReturnToPrevState();
		}
		protected override void ProcessTableCellOpenTag(HtmlElement element) {
			hasCells = true;
			CurrentOutput.Add(element);
			Corrector.StartNewCell(CurrentOutput);
		}
		protected override void ProcessTableCellCloseTag(HtmlElement element) {
			if (hasCells)
				return;
			Corrector.ForceOpenTableCell();
			Corrector.Process(element);
		}
		protected override void ProcessCaptionOpenTag(HtmlElement element) {
			Corrector.ForceCloseTableRow();
			Corrector.Process(element);
		}
		protected override void ProcessCaptionCloseTag(HtmlElement element) {
			Corrector.ForceCloseTableRow();
			Corrector.Process(element);
		}
		public override void ProcessEnd() {
			Corrector.ForceCloseTableRow();
			Corrector.ProcessEnd();
		}
	}
	#endregion
	#region HtmlCorrectorTableCellState
	public class HtmlCorrectorTableCellState : HtmlCorrectorStateBase {
		public HtmlCorrectorTableCellState(HtmlCorrector corrector, List<HtmlElement> output)
			: base(corrector, output) {
		}
		protected override void ProcessNonTableElement(HtmlElement element) {
			CurrentOutput.Add(element);
		}
		protected override void ProcessTableOpenTag(HtmlElement element) {
			CurrentOutput.Add(element);
			Corrector.StartNewTable(CurrentOutput);
		}
		protected override void ProcessTableCloseTag(HtmlElement element) {
			Corrector.ForceCloseTableCell();
			Corrector.Process(element);
		}
		protected override void ProcessTableRowOpenTag(HtmlElement element) {
			Corrector.ForceCloseTableCell();
			Corrector.Process(element);
		}
		protected override void ProcessTableRowCloseTag(HtmlElement element) {
			Corrector.ForceCloseTableCell();
			Corrector.Process(element);
		}
		protected override void ProcessTableCellOpenTag(HtmlElement element) {
			Corrector.ForceCloseTableCell();
			Corrector.Process(element);
		}
		protected override void ProcessTableCellCloseTag(HtmlElement element) {
			Corrector.ReturnToPrevStateFromCellState(CurrentOutput.Count);
			CurrentOutput.Add(element);
		}
		protected override void ProcessCaptionOpenTag(HtmlElement element) {
			Corrector.ForceCloseTableCell();
			Corrector.Process(element);
		}
		protected override void ProcessCaptionCloseTag(HtmlElement element) {
			Corrector.ForceCloseTableCell();
			Corrector.Process(element);
		}
		public override void ProcessEnd() {
			Corrector.ForceCloseTableCell();
			Corrector.ProcessEnd();
		}
	}
	#endregion
	#region HtmlCorrectorTableCaptionState
	public class HtmlCorrectorTableCaptionState : HtmlCorrectorStateBase {
		bool empty;
		List<HtmlElement> parentOutput;
		public HtmlCorrectorTableCaptionState(HtmlCorrector corrector, List<HtmlElement> output)
			: base(corrector, new List<HtmlElement>()) {
			this.parentOutput = output;
			empty = true;
		}
		protected override void ProcessNonTableElement(HtmlElement element) {
			CurrentOutput.Add(element);
			empty = false;
		}
		protected override void ProcessTableOpenTag(HtmlElement element) {
			CurrentOutput.Add(element);
			Corrector.StartNewTable(CurrentOutput);
		}
		protected override void ProcessTableCloseTag(HtmlElement element) {
			Corrector.ForceCloseCaption();
			Corrector.Process(element);
		}
		protected override void ProcessTableRowOpenTag(HtmlElement element) {
			Corrector.ForceCloseCaption();
			Corrector.Process(element);
		}
		protected override void ProcessTableRowCloseTag(HtmlElement element) {
			Corrector.ForceCloseCaption();
			Corrector.Process(element);
		}
		protected override void ProcessTableCellOpenTag(HtmlElement element) {
			Corrector.ForceCloseCaption();
			Corrector.Process(element);
		}
		protected override void ProcessTableCellCloseTag(HtmlElement element) {
			Corrector.ForceCloseCaption();
			CurrentOutput.Add(element);
		}
		protected override void ProcessCaptionOpenTag(HtmlElement element) {
		}
		protected override void ProcessCaptionCloseTag(HtmlElement element) {
			if (!empty) {
				CurrentOutput.Add(element);
				this.parentOutput.AddRange(CurrentOutput);
			}
			else
				this.parentOutput.RemoveAt(this.parentOutput.Count - 1);
			Corrector.ReturnToPrevState();
		}
		public override void ProcessEnd() {
			Corrector.ForceCloseCaption();
			Corrector.ProcessEnd();
		}
	}
	#endregion
	#region HtmlCorrectorTableBodyState
	public class HtmlCorrectorTableBodyState : HtmlCorrectorTableStateBase {
		public HtmlCorrectorTableBodyState(HtmlCorrector corrector, List<HtmlElement> output)
			: base(corrector, output) {
		}
		protected override void ProcessTbodyCloseTag(HtmlElement element) {
			EnsureTableHasRows();
			CurrentOutput.AddRange(InnerOutput);
			CurrentOutput.Add(element);
			Corrector.ReturnToPrevState();
		}
		protected override void ProcessTbodyOpenTag(HtmlElement element) {
		}
		protected override void ProcessTableCloseTag(HtmlElement element) {
			Corrector.ForceCloseTag(HtmlTagNameID.Tbody);
			Corrector.Process(element);
		}
		public override void ProcessEnd() {
			Corrector.ForceCloseTag(HtmlTagNameID.Tbody);
			base.ProcessEnd();
		}
		protected override void ProcessTfootOpenTag(HtmlElement element) {
			Corrector.ForceCloseTag(HtmlTagNameID.Tbody);
			Corrector.Process(element);
		}
		protected override void ProcessTheadOpenTag(HtmlElement element) {
			Corrector.ForceCloseTag(HtmlTagNameID.Tbody);
			Corrector.Process(element);
		}
		protected override void ProcessCaptionOpenTag(HtmlElement element) {
			Corrector.ForceCloseTag(HtmlTagNameID.Tbody);
			Corrector.Process(element);
		}
		protected override void ProcessCaptionCloseTag(HtmlElement element) {
		}
	}
	#endregion
	#region HtmlCorrectorTableHeaderState
	public class HtmlCorrectorTableHeaderState : HtmlCorrectorTableStateBase {
		public HtmlCorrectorTableHeaderState(HtmlCorrector corrector, List<HtmlElement> output)
			: base(corrector, output) {
		}
		protected override void ProcessTheadOpenTag(HtmlElement element) {
		}
		protected override void ProcessTheadCloseTag(HtmlElement element) {
			EnsureTableHasRows();
			CurrentOutput.AddRange(InnerOutput);
			CurrentOutput.Add(element);
			Corrector.ReturnToPrevState();
		}
		protected override void ProcessTbodyOpenTag(HtmlElement element) {
			Corrector.ForceCloseTag(HtmlTagNameID.Thead);
			Corrector.Process(element);
		}
		protected override void ProcessTableCloseTag(HtmlElement element) {
			Corrector.ForceCloseTag(HtmlTagNameID.Thead);
			Corrector.Process(element);
		}
		public override void ProcessEnd() {
			Corrector.ForceCloseTag(HtmlTagNameID.Thead);
			base.ProcessEnd();
		}
		protected override void ProcessCaptionOpenTag(HtmlElement element) {
			Corrector.ForceCloseTag(HtmlTagNameID.Thead);
			Corrector.Process(element);
		}
		protected override void ProcessCaptionCloseTag(HtmlElement element) {
		}
	}
	#endregion
	#region HtmlCorrectorTableFooterState
	public class HtmlCorrectorTableFooterState : HtmlCorrectorTableStateBase {
		public HtmlCorrectorTableFooterState(HtmlCorrector corrector, List<HtmlElement> output)
			: base(corrector, output) {
		}
		protected override void ProcessTbodyOpenTag(HtmlElement element) {
			Corrector.ForceCloseTag(HtmlTagNameID.Tfoot);
			Corrector.Process(element);
		}
		protected override void ProcessTfootOpenTag(HtmlElement element) {
		}
		protected override void ProcessTfootCloseTag(HtmlElement element) {
			EnsureTableHasRows();
			CurrentOutput.AddRange(InnerOutput);
			CurrentOutput.Add(element);
			Corrector.ReturnToPrevState();
		}
		protected override void ProcessTableCloseTag(HtmlElement element) {
			Corrector.ForceCloseTag(HtmlTagNameID.Tfoot);
			Corrector.Process(element);
		}
		public override void ProcessEnd() {
			Corrector.ForceCloseTag(HtmlTagNameID.Tfoot);
			base.ProcessEnd();
		}
		protected override void ProcessCaptionOpenTag(HtmlElement element) {
			Corrector.ForceCloseTag(HtmlTagNameID.Tfoot);
			Corrector.Process(element);
		}
		protected override void ProcessCaptionCloseTag(HtmlElement element) {
		}
	}
	#endregion
}
