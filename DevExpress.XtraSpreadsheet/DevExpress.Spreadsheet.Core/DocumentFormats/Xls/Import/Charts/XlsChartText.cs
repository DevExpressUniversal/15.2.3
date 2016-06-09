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
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.DrawingML;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsChartTextObjectLinkType
	public enum XlsChartTextObjectLinkType {
		Chart = 0x01,
		ValueAxis = 0x02,
		ArgumentAxis = 0x03,
		SeriesOrDataPoint = 0x04,
		SeriesAxis = 0x07,
		DispUnitLabel = 0x0c
	}
	#endregion
	#region XlsChartTextObjectLink
	public class XlsChartTextObjectLink {
		public bool Apply { get; set; }
		public XlsChartTextObjectLinkType LinkType { get; set; }
		public int SeriesIndex { get; set; }
		public int PointIndex { get; set; }
		public bool IsChartTitle { get { return Apply && (LinkType == XlsChartTextObjectLinkType.Chart); } }
		public bool IsArgumentAxisTitle { get { return Apply && (LinkType == XlsChartTextObjectLinkType.ArgumentAxis); } }
		public bool IsValueAxisTitle { get { return Apply && (LinkType == XlsChartTextObjectLinkType.ValueAxis); } }
		public bool IsSeriesOrDataPoint { get { return Apply && (LinkType == XlsChartTextObjectLinkType.SeriesOrDataPoint); } }
		public bool IsSeriesAxisTitle { get { return Apply && (LinkType == XlsChartTextObjectLinkType.SeriesAxis); } }
	}
	#endregion
	#region XlsChartTextBuilderBase
	public abstract class XlsChartTextBuilderBase : IXlsChartBuilder, IXlsChartPosition, IXlsChartLayout12, IXlsChartFrameContainer, IXlsChartExtPropertyVisitor, IXlsChartFontX, IXlsChartTextFormatContainer {
		#region Fields
		XlsChartPosition position;
		XlsChartLayout12 layout12;
		XlsChartFrame frame;
		XlsChartFontX fontX;
		XlsChartTextFormat textFormat;
		#endregion
		#region Properties
		public int Left { get; set; }
		public int Top { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public XlsChartPosition Position { get { return position; } }
		public XlsChartLayout12 Layout12 { get { return layout12; } }
		public XlsChartFrame Frame { get { return frame; } }
		public XlsChartFontX FontX { get { return fontX; } }
		public bool Overlay { get; set; }
		public XlsChartTextFormat TextFormat { get { return textFormat; } }
		#endregion
		protected XlsChartTextBuilderBase() {
			this.position = new XlsChartPosition();
			this.layout12 = new XlsChartLayout12();
			this.fontX = new XlsChartFontX();
			this.textFormat = null;
		}
		#region IXlsChartBuilder Members
		public abstract void Execute(XlsContentBuilder contentBuilder);
		#endregion
		#region IXlsChartPosition Members
		bool IXlsChartPosition.Apply {
			get { return position.Apply; }
			set { position.Apply = value; }
		}
		XlsChartPosMode IXlsChartPosition.TopLeftMode {
			get { return position.TopLeftMode; }
			set { position.TopLeftMode = value; }
		}
		XlsChartPosMode IXlsChartPosition.BottomRightMode {
			get { return position.BottomRightMode; }
			set { position.BottomRightMode = value; }
		}
		int IXlsChartPosition.Left {
			get { return position.Left; }
			set { position.Left = value; }
		}
		int IXlsChartPosition.Top {
			get { return position.Top; }
			set { position.Top = value; }
		}
		int IXlsChartPosition.Width {
			get { return position.Width; }
			set { position.Width = value; }
		}
		int IXlsChartPosition.Height {
			get { return position.Height; }
			set { position.Height = value; }
		}
		#endregion
		#region IXlsChartLayout12 Members
		bool IXlsChartLayout12.Apply {
			get { return layout12.Apply; }
			set { layout12.Apply = value; }
		}
		LegendPosition IXlsChartLayout12.LegendPos {
			get { return layout12.LegendPos; }
			set { layout12.LegendPos = value; }
		}
		LayoutMode IXlsChartLayout12.XMode {
			get { return layout12.XMode; }
			set { layout12.XMode = value; }
		}
		LayoutMode IXlsChartLayout12.YMode {
			get { return layout12.YMode; }
			set { layout12.YMode = value; }
		}
		LayoutMode IXlsChartLayout12.WidthMode {
			get { return layout12.WidthMode; }
			set { layout12.WidthMode = value; }
		}
		LayoutMode IXlsChartLayout12.HeightMode {
			get { return layout12.HeightMode; }
			set { layout12.HeightMode = value; }
		}
		double IXlsChartLayout12.X {
			get { return layout12.X; }
			set { layout12.X = value; }
		}
		double IXlsChartLayout12.Y {
			get { return layout12.Y; }
			set { layout12.Y = value; }
		}
		double IXlsChartLayout12.Width {
			get { return layout12.Width; }
			set { layout12.Width = value; }
		}
		double IXlsChartLayout12.Height {
			get { return layout12.Height; }
			set { layout12.Height = value; }
		}
		#endregion
		#region IXlsChartFrameContainer Members
		void IXlsChartFrameContainer.Add(XlsChartFrame frame) {
			this.frame = frame;
		}
		#endregion
		#region IXlsChartExtPropertyVisitor Members
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropScaleMax item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropScaleMin item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropLogBase item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropStyle item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropThemeOverride item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropColorMapOverride item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropNoMultiLvlLbl item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickLabelSkip item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickMarkSkip item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMajorUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMinorUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickLabelPos item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropBaseTimeUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropFormatCode item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMajorTimeUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMinorTimeUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropShowDLblsOverMax item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropBackWallThickness item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropFloorThickness item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropDispBlankAs item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropStartSurface item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropEndSurface item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropShapeProps item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTextProps item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropOverlay item) {
			Overlay = item.Value;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropPieCombo item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRightAngAxOff item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropPerspective item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRotationX item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRotationY item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropHeightPercent item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropCultureCode item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropSymbol item) { }
		#endregion
		#region IXlsChartFontX Members
		bool IXlsChartFontX.Apply {
			get { return fontX.Apply; }
			set { fontX.Apply = value; }
		}
		int IXlsChartFontX.Index {
			get { return fontX.Index; }
			set { fontX.Index = value; }
		}
		#endregion
		#region IXlsChartTextFormatContainer Members
		void IXlsChartTextFormatContainer.Add(XlsChartTextFormat properties) {
			textFormat = properties;
		}
		#endregion
	}
	#endregion
	#region IXlsChartTextContainer
	public interface IXlsChartTextContainer {
		void Add(XlsChartTextBuilder item);
	}
	#endregion
	#region XlsChartTextBuilder
	public class XlsChartTextBuilder : XlsChartTextBuilderBase, IXlsChartDataRefContainer {
		#region Fields
		public const string NewLine = "\n";
		XlsChartDataRef dataRef;
		readonly List<XlsFormatRun> formatRuns = new List<XlsFormatRun>();
		readonly XlsChartTextObjectLink objectLink = new XlsChartTextObjectLink();
		readonly XlsChartDataLabelExtContent dataLabelExtContent = new XlsChartDataLabelExtContent();
		DrawingTextParagraph paragraph;
		#endregion
		#region Properties
		public DrawingTextAlignmentType HorizontalAlignment { get; set; }
		public DrawingTextAnchoringType VerticalAlignment { get; set; }
		public bool IsTransparent { get; set; }
		public Color TextColor { get; set; }
		public bool AutoColor { get; set; }
		public bool ShowKey { get; set; }
		public bool ShowValue { get; set; }
		public bool AutoText { get; set; }
		public bool IsGenerated { get; set; }
		public bool Deleted { get; set; }
		public bool AutoMode { get; set; }
		public bool ShowLabelAndPercent { get; set; }
		public bool ShowPercent { get; set; }
		public bool ShowBubbleSize { get; set; }
		public bool ShowLabel { get; set; }
		public DataLabelPosition DataLabelPos { get; set; }
		public bool IsManualDataLabelPos { get; set; }
		public XlReadingOrder TextReadingOrder { get; set; }
		public DrawingTextVerticalTextType VerticalText { get; set; }
		public int RotationAngle { get; set; }
		public IXlsChartTextContainer Container { get; set; }
		public XlsChartDataRef DataRef { get { return dataRef; } }
		public List<XlsFormatRun> FormatRuns { get { return formatRuns; } }
		public XlsChartTextObjectLink ObjectLink { get { return objectLink; } }
		public XlsChartDataLabelExtContent DataLabelExtContent { get { return dataLabelExtContent; } }
		protected internal int TextRotation { get; set; }
		#endregion
		public XlsChartTextBuilder()
			: base() {
		}
		#region IXlsChartBuilder Members
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (Container != null)
				Container.Add(this);
		}
		#endregion
		#region IXlsChartDataRefContainer Members
		void IXlsChartDataRefContainer.Add(XlsChartDataRef dataRef) {
			this.dataRef = dataRef;
		}
		void IXlsChartDataRefContainer.SetSeriesText(string value) {
			if (this.dataRef != null)
				this.dataRef.SeriesText = value;
		}
		#endregion
		public void SetupTitle(XlsContentBuilder contentBuilder, TitleOptions title) {
			if (title == null || Deleted || dataRef == null)
				return;
			title.Overlay = Overlay;
			title.Text = SetupChartText(contentBuilder, title.Parent);
			if (title.Text.TextType != ChartTextType.Rich)
				SetupTextProperties(contentBuilder, title.TextProperties);
			SetupShapeProperties(title.ShapeProperties);
			SetupLayout(title.Layout);
		}
		public void SetupDisplayUnitOptions(XlsContentBuilder contentBuilder, DisplayUnitOptions displayUnit) {
			if (displayUnit == null || Deleted || dataRef == null)
				return;
			displayUnit.Text = SetupChartText(contentBuilder, displayUnit.Parent);
			if (displayUnit.Text.TextType != ChartTextType.Rich)
				SetupTextProperties(contentBuilder, displayUnit.TextProperties);
			SetupShapeProperties(displayUnit.ShapeProperties);
			SetupLayout(displayUnit.Layout);
		}
		IChartText SetupChartText(XlsContentBuilder contentBuilder, IChart parent) {
			if (AutoText)
				return ChartText.Auto;
			else if (dataRef.DataType == XlsChartDataRefType.Reference)
				return SetupRefText(contentBuilder, parent);
			else if (dataRef.DataType == XlsChartDataRefType.Literal)
				return SetupRichText(contentBuilder, parent);
			return ChartText.Empty;
		}
		ChartTextRef SetupRefText(XlsContentBuilder contentBuilder, IChart parent) {
			ChartTextRef textRef = new ChartTextRef(parent);
			textRef.FormulaData.SetExpressionCore(XlsParsedThingConverter.ToModelExpression(dataRef.Expression, contentBuilder.RPNContext));
			textRef.CachedValue = dataRef.SeriesText;
			return textRef;
		}
		ChartRichText SetupRichText(XlsContentBuilder contentBuilder, IChart parent) {
			ChartRichText richText = new ChartRichText(parent);
			if (FormatRuns.Count > 0)
				SetupTextRuns(contentBuilder, richText, dataRef.SeriesText);
			else {
				string seriesText = dataRef.SeriesText;
				if (!string.IsNullOrEmpty(seriesText)) {
					if (FontX.Apply)
						AddTextRun(richText, seriesText, contentBuilder.StyleSheet.GetFontInfoIndex(FontX.Index));
					richText.PlainText = seriesText;
				}
				else {
					DocumentModel documentModel = richText.DocumentModel;
					DrawingTextParagraph paragraph = new DrawingTextParagraph(documentModel);
					paragraph.ApplyParagraphProperties = true;
					paragraph.ParagraphProperties.ApplyDefaultCharacterProperties = true;
					richText.Paragraphs.Add(paragraph);
				}
			}
			SetupBodyProperties(richText.BodyProperties);
			SetupParagraphProperties(contentBuilder, richText.Paragraphs[0]);
			return richText;
		}
		void SetupTextRuns(XlsContentBuilder contentBuilder, ChartRichText richText, string text) {
			int lastCharIndex = 0;
			int lastFontIndex = 0;
			int length = text.Length;
			for (int i = 0; i < FormatRuns.Count; i++) {
				XlsFormatRun formatRun = FormatRuns[i];
				if (formatRun.CharIndex >= length) break; 
				int runLength = formatRun.CharIndex - lastCharIndex;
				if (runLength > 0)
					AddTextRun(richText, text.Substring(lastCharIndex, runLength), 
						contentBuilder.StyleSheet.GetFontInfoIndex(lastFontIndex));
				lastCharIndex = formatRun.CharIndex;
				lastFontIndex = formatRun.FontIndex;
				if (lastFontIndex == XlsDefs.UnusedFontIndex)
					contentBuilder.ThrowInvalidFile("Invalid font index in format run");
				else if (lastFontIndex > XlsDefs.UnusedFontIndex)
					lastFontIndex--;
			}
			if ((length - lastCharIndex) > 0)
				AddTextRun(richText, text.Substring(lastCharIndex, length - lastCharIndex), contentBuilder.StyleSheet.GetFontInfoIndex(lastFontIndex));
			if (text.Substring(length - 1, 1) == NewLine)
				AddTextRun(richText, NewLine, contentBuilder.StyleSheet.GetFontInfoIndex(lastFontIndex));
		}
		void AddTextRun(ChartRichText richText, string runText, int fontIndex) {
			if (string.IsNullOrEmpty(runText))
				return;
			DocumentModel documentModel = richText.DocumentModel;
			int runTextLength = runText.Length;
			bool emptyLine = runText == NewLine;
			bool lineBreak = !emptyLine && runText.Substring(runTextLength - 1, 1) == NewLine;
			if (this.paragraph == null) {
				this.paragraph = new DrawingTextParagraph(documentModel);
				richText.Paragraphs.Add(this.paragraph);
			}
			DrawingTextRunCollection runs = this.paragraph.Runs;
			RunFontInfo fontInfo = documentModel.Cache.FontInfoCache[fontIndex];
			if (emptyLine) {
				XlsCharacterPropertiesHelper.SetupCharacterProperties(this.paragraph.EndRunProperties, fontInfo, documentModel);
				this.paragraph.ApplyEndRunProperties = true;
			}
			else {
				if (lineBreak)
					runText = runText.Substring(0, runTextLength - 1);
				runs.Add(new DrawingTextRun(documentModel, runText));
				XlsCharacterPropertiesHelper.SetupCharacterProperties((runs[runs.Count - 1] as DrawingTextRunBase).RunProperties, fontInfo, documentModel);
			}
			if (emptyLine || lineBreak)
				this.paragraph = null;
		}
		public void SetupSeriesDataLabels(XlsContentBuilder contentBuilder, SeriesWithDataLabelsAndPoints series) {
			bool isSeries = objectLink.PointIndex == XlsChartDefs.PointIndexOfSeries;
			bool isDataPoint = objectLink.PointIndex >= 0 && objectLink.PointIndex <= XlsChartDefs.MaxPointIndex;
			if (isSeries) {
				bool hasDefaultText = WithDefaultText(contentBuilder, objectLink.SeriesIndex);
				DataLabelBase dataLabel = series.DataLabels;
				if (hasDefaultText)
					SetupSeriesDataLabelsCore(contentBuilder, dataLabel, false);
				else {
					SetupSeriesDataLabelsCore(contentBuilder, dataLabel, IsAreaOrFilledRadar(series.View));
					if (IsPieChartGroup(series.View.ViewType) && !dataLabelExtContent.Apply)
						dataLabel.Separator = NewLine;
				}
			}
			else if (isDataPoint) {
				DataLabel label = new DataLabel(series.Parent, objectLink.PointIndex);
				label.Text = SetupChartText(contentBuilder, label.Parent);
				SetupPointDataLabel(contentBuilder, label);
				SetupLayout(label.Layout);
				series.DataLabels.Labels.Add(label);
			}
			series.DataLabels.Apply = true;
		}
		bool WithDefaultText(XlsContentBuilder contentBuilder, int seriesIndex) {
			int viewIndex = contentBuilder.SeriesFormats[objectLink.SeriesIndex].ViewIndex;
			XlsChartViewBuilder viewBuilder = FindViewBuilderByIndex(contentBuilder, viewIndex);
			return viewBuilder == null ? false : viewBuilder.DefaultText.Count > 0;
		}
		XlsChartViewBuilder FindViewBuilderByIndex(XlsContentBuilder contentBuilder, int viewIndex) {
			foreach (XlsChartAxisGroupBuilder axisBuilder in contentBuilder.AxisGroups)
				foreach (XlsChartViewBuilder viewBuilder in axisBuilder.ViewBuilders)
					if (viewBuilder.ViewIndex == viewIndex)
						return viewBuilder;
			return null;
		}
		void SetupSeriesDataLabelsCore(XlsContentBuilder contentBuilder, DataLabelBase dataLabel, bool isAreaOrFilledRadar) {
			if (isAreaOrFilledRadar)
				dataLabelExtContent.SetupDataLabelsForAreaOrFilledRadar(dataLabel);
			else {
				SetupDataLabelsFromText(dataLabel);
				dataLabelExtContent.SetupDataLabelsFromExtContent(dataLabel);
			}
			SetupNumberFormat(contentBuilder, dataLabel.NumberFormat);
			SetupShapeProperties(dataLabel.ShapeProperties);
			SetupTextProperties(contentBuilder, dataLabel.TextProperties);
		}
		void SetupPointDataLabel(XlsContentBuilder contentBuilder, DataLabel dataLabel) {
			SetupDataLabelsFromText(dataLabel);
			dataLabelExtContent.SetupDataLabelsFromExtContent(dataLabel);
			SetupNumberFormat(contentBuilder, dataLabel.NumberFormat);
			SetupShapeProperties(dataLabel.ShapeProperties);
			if (dataLabel.Text.TextType != ChartTextType.Rich)
				SetupTextProperties(contentBuilder, dataLabel.TextProperties);
		}
		void SetupLayout(LayoutOptions layout) {
			Layout12.SetupLayout(layout);
		}
		void SetupDataLabelsFromText(DataLabelBase dataLabel) {
			dataLabel.BeginUpdate();
			try {
				dataLabel.Delete = Deleted;
				dataLabel.ShowLegendKey = ShowKey;
				dataLabel.ShowValue = ShowValue;
				dataLabel.ShowCategoryName = ShowLabel || ShowLabelAndPercent;
				dataLabel.ShowPercent = ShowPercent || ShowLabelAndPercent;
				dataLabel.ShowBubbleSize = ShowBubbleSize;
				dataLabel.LabelPosition = DataLabelPos;
			}
			finally {
				dataLabel.EndUpdate();
			}
		}
		void SetupNumberFormat(XlsContentBuilder contentBuilder, NumberFormatOptions numberFormat) {
			if (dataRef == null)
				return;
			numberFormat.SourceLinked = dataRef.IsSourceLinked;
			int index = contentBuilder.StyleSheet.GetNumberFormatIndex(dataRef.NumberFormatId);
			numberFormat.NumberFormatCode = contentBuilder.DocumentModel.Cache.NumberFormatCache[index].FormatCode;
		}
		void SetupShapeProperties(ShapeProperties shapeProperties) {
			if (Frame != null)
				Frame.SetupShapeProperties(shapeProperties);
		}
		void SetupTextProperties(XlsContentBuilder contentBuilder, TextProperties properties) {
			if (TextFormat != null && TextFormat.IsValidCheckSum(contentBuilder, this)) {
				TextFormat.SetupTextProperties(properties);
				return;
			}
			if (IsGenerated)
				return;
			SetupTextPropertiesBase(contentBuilder, properties);
		}
		protected internal void SetupTextPropertiesBase(XlsContentBuilder contentBuilder, TextProperties properties) {
			SetupBodyProperties(properties.BodyProperties);
			DrawingTextParagraph paragraph = SetupParagraphProperties(contentBuilder, new DrawingTextParagraph(contentBuilder.DocumentModel));
			properties.Paragraphs.Add(paragraph);
		}
		void SetupBodyProperties(DrawingTextBodyProperties bodyProperties) {
			bodyProperties.VerticalText = VerticalText;
			bodyProperties.Anchor = VerticalAlignment;
			bodyProperties.Rotation = RotationAngle;
		}
		DrawingTextParagraph SetupParagraphProperties(XlsContentBuilder contentBuilder, DrawingTextParagraph paragraph) {
			paragraph.ApplyParagraphProperties = true;
			DrawingTextParagraphProperties paragraphProperties = paragraph.ParagraphProperties;
			paragraphProperties.ApplyDefaultCharacterProperties = true;
			paragraphProperties.TextAlignment = HorizontalAlignment;
			paragraphProperties.RightToLeft = TextReadingOrder == XlReadingOrder.RightToLeft;
			DrawingTextCharacterProperties defaultProperties = paragraphProperties.DefaultCharacterProperties;
			if (FontX.Apply)
				FontX.SetupParagraphDefaults(contentBuilder, defaultProperties);
			DrawingSolidFill fill = new DrawingSolidFill(contentBuilder.DocumentModel);
			fill.Color.OriginalColor.Rgb = TextColor;
			defaultProperties.Fill = fill;
			return paragraph;
		}
		bool IsAreaOrFilledRadar(IChartView view) {
			ChartViewType viewType = view.ViewType;
			if (viewType == ChartViewType.Radar) {
				RadarChartView radarView = view as RadarChartView;
				return radarView.RadarStyle == RadarChartStyle.Filled;
			}
			return viewType == ChartViewType.Area || viewType == ChartViewType.Area3D;
		}
		bool IsPieChartGroup(ChartViewType viewType) {
			return viewType == ChartViewType.Pie || viewType == ChartViewType.Pie3D ||
				   viewType == ChartViewType.OfPie || viewType == ChartViewType.Doughnut;
		}
	}
	#endregion
	#region XlsCharacterPropertiesHelper
	public static class XlsCharacterPropertiesHelper {
		public static RunFontInfo DefaultFont { get { return CreateDefaultFont(); } }
		public static RunFontInfo DefaultChartFont { get { return CreateDefaultChartFont(); } }
		public static RunFontInfo DefaultAxisTitleFont { get { return CreateDefaultAxisTitleFont(); } }
		public static RunFontInfo DefaultChartTitleFont { get { return CreateDefaultChartTitleFont(); } }
		#region CreateDefaultInfos
		static RunFontInfo CreateDefaultFont() {
			RunFontInfo fontInfo = new RunFontInfo();
			fontInfo.Name = "Calibri";
			fontInfo.Size = 11;
			fontInfo.SchemeStyle = XlFontSchemeStyles.Minor;
			fontInfo.FontFamily = 2;
			fontInfo.ColorIndex = Palette.FontAutomaticColorIndex;
			return fontInfo;
		}
		static RunFontInfo CreateDefaultChartFont() {
			RunFontInfo fontInfo = new RunFontInfo();
			fontInfo.Name = "Calibri";
			fontInfo.Size = 10;
			fontInfo.SchemeStyle = XlFontSchemeStyles.Minor;
			fontInfo.FontFamily = 2;
			return fontInfo;
		}
		static RunFontInfo CreateDefaultAxisTitleFont() {
			RunFontInfo fontInfo = new RunFontInfo();
			fontInfo.Name = "Calibri";
			fontInfo.Size = 10;
			fontInfo.Bold = true;
			fontInfo.SchemeStyle = XlFontSchemeStyles.Minor;
			fontInfo.FontFamily = 2;
			return fontInfo;
		}
		static RunFontInfo CreateDefaultChartTitleFont() {
			RunFontInfo fontInfo = new RunFontInfo();
			fontInfo.Name = "Calibri";
			fontInfo.Size = 18;
			fontInfo.Bold = true;
			fontInfo.SchemeStyle = XlFontSchemeStyles.Minor;
			fontInfo.FontFamily = 2;
			return fontInfo;
		}
		#endregion
		public static RunFontInfo GetDefaultFontInfoForChartElement(ChartElement element) {
			switch (element) {
				case ChartElement.ChartTitle:
					return DefaultChartTitleFont;
				case ChartElement.AxisTitle:
					return DefaultAxisTitleFont;
				default:
					return DefaultChartFont;
			}
		}
		public static void SetupCharacterProperties(DrawingTextCharacterProperties characterProperties, RunFontInfo info, DocumentModel documentModel) {
			SetupCharacterPropertiesWithoutFill(characterProperties, info);
			DrawingSolidFill fill = new DrawingSolidFill(documentModel);
			ColorModelInfo colorInfo = documentModel.Cache.ColorModelInfoCache[info.ColorIndex];
			fill.Color.OriginalColor.Rgb = colorInfo.ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors);
			characterProperties.Fill = fill;
		}
		public static void SetupCharacterPropertiesWithoutFill(DrawingTextCharacterProperties characterProperties, RunFontInfo info) {
			characterProperties.BeginUpdate();
			try {
				characterProperties.FontSize = (int)info.Size * 100;
				characterProperties.Italic = info.Italic;
				characterProperties.Bold = info.Bold;
				characterProperties.Strikethrough = info.StrikeThrough ? DrawingTextStrikeType.Single : DrawingTextStrikeType.None;
				characterProperties.Underline = ConvertModelUnderlineToDrawingText(info.Underline);
				characterProperties.Baseline = ConvertModelScriptTypeToBaseline(info.Script);
				characterProperties.Latin.Typeface = info.Name;
			}
			finally {
				characterProperties.EndUpdate();
			}
		}
		public static RunFontInfo GetRunFontInfo(DrawingTextCharacterProperties runProperties, ChartElement element, Palette palette) {
			RunFontInfo defaultInfo = GetDefaultFontInfoForChartElement(element);
			if (runProperties.IsDefault)
				return defaultInfo;
			return UpdateRunFontInfo(defaultInfo, runProperties, palette);
		}
		public static RunFontInfo GetRunFontInfo(DrawingTextCharacterProperties runProperties, DrawingTextParagraphProperties paragraphProperties, ChartElement element, Palette palette) {
			RunFontInfo defaultInfo = GetDefaultFontInfoForChartElement(element);
			if (paragraphProperties.ApplyDefaultCharacterProperties) {
				if (paragraphProperties.IsDefault)
					return runProperties.IsDefault ? defaultInfo : UpdateRunFontInfo(defaultInfo, runProperties, palette);
				else {
					DrawingTextCharacterProperties paragraphDefaults = paragraphProperties.DefaultCharacterProperties;
					if (runProperties.IsDefault)
						return UpdateRunFontInfo(defaultInfo, paragraphDefaults, palette);
					else {
						RunFontInfo updatedInfo = UpdateRunFontInfo(defaultInfo, paragraphDefaults, palette);
						return UpdateRunFontInfo(updatedInfo, runProperties, palette);
					}
				}
			}
			return runProperties.IsDefault ? defaultInfo : UpdateRunFontInfo(new RunFontInfo(), runProperties, palette);
		}
		static RunFontInfo UpdateRunFontInfo(RunFontInfo info, DrawingTextCharacterProperties properties, Palette palette) {
			ITextCharacterOptions options = properties.Options;
			if (options.HasBold)
				info.Bold = properties.Bold;
			if (options.HasItalic)
				info.Italic = properties.Italic;
			if (options.HasStrikethrough)
				info.StrikeThrough = properties.Strikethrough != DrawingTextStrikeType.None;
			if (!properties.Latin.IsDefault)
				info.Name = properties.Latin.Typeface;
			if (options.HasFontSize)
				info.Size = properties.FontSize / 100;
			if (options.HasBaseline)
				info.Script = ConvertBaselineToModelScriptType(properties.Baseline);
			if (options.HasUnderline)
				info.Underline = ConvertDrawingTextUnderlineToModel(properties.Underline);
			IDrawingFill fill = properties.Fill;
			DrawingFillType fillType = fill.FillType;
			if (fillType == DrawingFillType.Solid) {
				DrawingSolidFill solidFill = fill as DrawingSolidFill;
				DrawingColor color = solidFill.Color;
				if (color.ColorType == DrawingColorType.Rgb)
					info.ColorIndex = palette.GetPaletteNearestColorIndex(color.OriginalColor.Rgb);
				else
					info.ColorIndex = palette.GetPaletteNearestColorIndex(color.FinalColor);
			}
			else
				info.ColorIndex = fillType == DrawingFillType.Automatic ? 63 : Palette.FontAutomaticColorIndex;
			return info;
		}
		static DrawingTextUnderlineType ConvertModelUnderlineToDrawingText(XlUnderlineType underline) {
			switch (underline) {
				case XlUnderlineType.Single:
				case XlUnderlineType.SingleAccounting:
					return DrawingTextUnderlineType.Single;
				case XlUnderlineType.Double:
				case XlUnderlineType.DoubleAccounting:
					return DrawingTextUnderlineType.Double;
				default:
					return DrawingTextUnderlineType.None;
			}
		}
		static XlUnderlineType ConvertDrawingTextUnderlineToModel(DrawingTextUnderlineType underline) {
			switch (underline) {
				case DrawingTextUnderlineType.Single:
					return XlUnderlineType.Single;
				case DrawingTextUnderlineType.Double:
					return XlUnderlineType.Double;
				default:
					return XlUnderlineType.None;
			}
		}
		internal static int ConvertModelScriptTypeToBaseline(XlScriptType script) {
			switch (script) {
				case XlScriptType.Superscript:
					return 30000;
				case XlScriptType.Subscript:
					return -25000;
				default:
					return 0;
			}
		}
		internal static XlScriptType ConvertBaselineToModelScriptType(int baseline) {
			if (baseline >= 30000)
				return XlScriptType.Superscript;
			if (baseline <= -25000)
				return XlScriptType.Subscript;
			return XlScriptType.Baseline;
		}
	}
	#endregion
	#region XlsChartTextRotationHelper
	public static class XlsChartTextRotationHelper {
		public const int DefaultRotation = -60000000;
		public static DrawingTextVerticalTextType GetVerticalTextType(int trot) {
			return (trot == 255) ? DrawingTextVerticalTextType.WordArtVertical : DrawingTextVerticalTextType.Horizontal;
		}
		public static int GetRotationAngle(int trot) {
			if (trot >= 0 && trot <= 90)
				return -trot * 60000;
			if (trot >= 91 && trot <= 180)
				return (trot - 90) * 60000;
			if (trot == 254)
				return -90 * 60000;
			return 0;
		}
		public static int GetTrot(DrawingTextVerticalTextType verticalText, int rotationAngle) {
			if (rotationAngle == DefaultRotation)
				return 0;
			if (verticalText == DrawingTextVerticalTextType.WordArtVertical)
				return 255;
			if (rotationAngle > 0)
				return rotationAngle / 60000 + 90;
			return -rotationAngle / 60000;
		}
	}
	#endregion
	#region IXlsChartDefaultTextContainer
	public interface IXlsChartDefaultTextContainer {
		int DefaultTextId { get; set; }
	}
	#endregion
	#region XlsChartDefaultText
	public class XlsChartDefaultText {
		public XlsChartDefaultText(int id, XlsChartTextBuilder text) {
			Guard.ArgumentNotNull(text, "text");
			Id = id;
			Text = text;
		}
		public int Id { get; private set; }
		public XlsChartTextBuilder Text { get; private set; }
	}
	#endregion
	#region XlsChartDefaultTextCollection
	public class XlsChartDefaultTextCollection : List<XlsChartDefaultText> {
		public XlsChartDefaultTextCollection()
			: base() {
		}
		public XlsChartTextBuilder FindBy(int id) {
			foreach (XlsChartDefaultText item in this)
				if (item.Id == id)
					return item.Text;
			return null;
		}
	}
	#endregion
}
