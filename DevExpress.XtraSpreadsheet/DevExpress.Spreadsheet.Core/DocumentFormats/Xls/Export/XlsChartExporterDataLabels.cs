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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Utils.Zip;
using System.Xml;
using DevExpress.Office;
#if !SL
using System.Drawing;
using DevExpress.Office.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public partial class XlsChartExporter {
		protected void WriteChartAttachedLabels() {
			WriteAttachedLabelForTitle(chart.Title, XlsChartTextObjectLinkType.Chart);
			for (seriesIndex = 0; seriesIndex < seriesList.Count; seriesIndex++) {
				ISeries series = seriesList[seriesIndex];
				viewIndex = viewIndexTable[series.View];
				WriteAttachedLabel(series as SeriesWithDataLabelsAndPoints, WriteAttachedLabelForDataLabels);
			}
		}
		delegate void WriteDataLabelsAction(DataLabels dataLabels, ChartViewType viewType);
		void WriteAttachedLabel(SeriesWithDataLabelsAndPoints series, WriteDataLabelsAction action) {
			if (series == null)
				return;
			ChartViewWithDataLabels view = series.View as ChartViewWithDataLabels;
			DataLabels viewDataLabels = view.DataLabels;
			DataLabels seriesDataLabels = series.DataLabels;
			if (!seriesDataLabels.Apply && viewDataLabels.IsVisible)
				action(viewDataLabels, view.ViewType);
			if (seriesDataLabels.Apply)
				action(seriesDataLabels, view.ViewType);
		}
		#region DataFormat attached label
		void WriteDataFormatAttachedLabel(DataLabels dataLabels, ChartViewType viewType) {
			bool isAreaOrFilledRadar = IsAreaChartGroup(viewType) || IsFilledRadar();
			if (isAreaOrFilledRadar && !dataLabels.ShowSeriesName && !dataLabels.ShowValue)
				return;
			XlsCommandChartAttachedLabel command = PrepareDataFormatAttachedLabel(dataLabels, isAreaOrFilledRadar);
			command.Write(StreamWriter);
		}
		protected internal XlsCommandChartAttachedLabel PrepareDataFormatAttachedLabel(DataLabelBase dataLabel, bool isAreaOrFilledRadar) {
			XlsCommandChartAttachedLabel command = new XlsCommandChartAttachedLabel();
			command.ShowValue = dataLabel.ShowValue;
			command.ShowPercent = dataLabel.ShowPercent;
			command.ShowLabelAndPercent = dataLabel.ShowCategoryName && dataLabel.ShowPercent;
			if (isAreaOrFilledRadar)
				command.ShowLabel = dataLabel.ShowSeriesName;
			else {
				command.ShowLabel = dataLabel.ShowCategoryName;
				command.ShowSeriesName = dataLabel.ShowSeriesName;
			}
			command.ShowBubbleSizes = dataLabel.ShowBubbleSize;
			return command;
		}
		#endregion
		#region DataLabels attached label
		void WriteAttachedLabelForDataLabels(DataLabels dataLabels, ChartViewType viewType) {
			DataLabelCollection labelCollection = dataLabels.Labels;
			for (int pointIndex = 0; pointIndex < labelCollection.Count; pointIndex++) {
				DataLabel dataLabel = labelCollection[pointIndex];
				if (IsWrapped(dataLabel, viewType))
					WriteWrappedDataLabels(dataLabel, viewType, dataLabel.ItemIndex);
				else
					WriteDataPoint(dataLabel, viewType);
			}
			if (IsWrapped(dataLabels, viewType))
				WriteWrappedDataLabels(dataLabels, viewType, XlsChartDefs.PointIndexOfSeries);
			else
				WriteDataLabels(dataLabels, viewType);
		}
		void WriteDataPoint(DataLabel dataLabel, ChartViewType viewType) {
			WriteDataLabelBase(dataLabel, viewType);
			WriteObjectLinkForDataLabels(dataLabel.ItemIndex);
			if (WithExtendedContent(dataLabel, viewType))
				WriteDataLabExtContents(dataLabel, viewType, false);
			if (!dataLabel.Delete)
				WriteCrtLayout12(dataLabel.Layout);
			WriteEnd();
		}
		void WriteDataLabels(DataLabels dataLabels, ChartViewType viewType) {
			WriteDataLabelBase(dataLabels, viewType);
			WriteObjectLinkForDataLabels(XlsChartDefs.PointIndexOfSeries);
			if (WithExtendedContent(dataLabels, viewType))
				WriteDataLabExtContents(dataLabels, viewType, false);
			WriteEnd();
		}
		void WriteDataLabelBase(DataLabelBase dataLabel, ChartViewType viewType) {
			ChartElement element = ChartElement.DataLabels;
			WriteText(dataLabel, viewType);
			WriteBegin();
			DataLabel label = dataLabel as DataLabel;
			if (label != null) {
				WritePosition(label.Layout);
				ChartRichText richText = label.Text as ChartRichText;
				if (RichTextWithoutRuns(richText))
					WriteFontX(richText.Paragraphs, element);
				else
					WriteFontX(label.TextProperties.Paragraphs, element);
				if (label.Text.TextType != ChartTextType.None)
					WriteChartText(label.Text, element);
			}
			else {
				WritePosition(null);
				WriteFontX(dataLabel.TextProperties.Paragraphs, element);
				WriteBRAI(dataLabel.NumberFormat);
			}
			WriteFrame(dataLabel.ShapeProperties, element);
		}
		void WriteWrappedDataLabels(DataLabelBase dataLabel, ChartViewType viewType, int pointIndex) {
			ChartElement element = ChartElement.DataLabels;
			WriteDataLabExt();
			WriteStartObject(XlsChartObjectKind.ExtendedDataLabel);
			WriteFrtWrapper(PrepareTextCommand(dataLabel, viewType));
			WriteFrtWrapper(commandBegin);
			DataLabel label = dataLabel as DataLabel;
			if (label != null) {
				WriteFrtWrapper(PreparePositonCommand(label.Layout));
				IChartText text = label.Text;
				WriteWrappedFontX(text as ChartRichText, label.TextProperties, element);
				WriteWrappedChartText(text, element);
			}
			else {
				WriteFrtWrapper(PrepareEmptyPositonCommand());
				WriteFrtWrapper(PrepareFontXCommand(dataLabel.TextProperties.Paragraphs, element));
				WriteFrtWrapper(PrepareBRAICommand(dataLabel.NumberFormat));
			}
			WriteWrappedFrame(dataLabel.ShapeProperties, element);
			WriteFrtWrapper(PrepareObjectLinkCommand(pointIndex));
			WriteDataLabExtContents(dataLabel, viewType, pointIndex == XlsChartDefs.PointIndexOfSeries);
			if (label != null && !label.Delete)
				WriteCrtLayout12(label.Layout);
			WriteFrtWrapper(commandEnd);
			WriteEndObject(XlsChartObjectKind.ExtendedDataLabel);
		}
		void WriteDataLabExt() {
			XlsCommandChartDataLabExt command = new XlsCommandChartDataLabExt();
			command.Write(StreamWriter);
		}
		void WriteWrappedFontX(ChartRichText richText, TextProperties textProperties, ChartElement element) {
			if (RichTextWithoutRuns(richText))
				WriteFrtWrapper(PrepareFontXCommand(richText.Paragraphs, element));
			else
				WriteFrtWrapper(PrepareFontXCommand(textProperties.Paragraphs, element));
		}
		void WriteWrappedChartText(IChartText text, ChartElement element) {
			ChartTextType textType = text.TextType;
			if (textType == ChartTextType.None)
				return;
			else if (textType == ChartTextType.Auto)
				WriteFrtWrapper(PrepareBRAI());
			else if (textType == ChartTextType.Ref) {
				ChartTextRef refText = text as ChartTextRef;
				WriteFrtWrapper(PrepareReferenceBRAI(refText.CachedValue.InlineTextValue, refText.Expression));
				WriteFrtWrapper(PrepareSeriesText(refText.PlainText));
			}
			else if (textType == ChartTextType.Rich) {
				ChartRichText richText = text as ChartRichText;
				if (!RichTextWithoutRuns(richText))
					WriteFrtWrapper(PrepareAIRuns(richText.Paragraphs, element));
				WriteFrtWrapper(PrepareBRAI());
				WriteFrtWrapper(PrepareSeriesText(richText.PlainText));
			}
		}
		#endregion
		#region Helper Methods
		DrawingTextAlignmentType GetHorizontalAlignment(DrawingTextAlignmentType alignment, bool rightToLeft) {
			if (alignment == DrawingTextAlignmentType.Left && rightToLeft)
				return DrawingTextAlignmentType.Right;
			if (alignment == DrawingTextAlignmentType.Right && rightToLeft)
				return DrawingTextAlignmentType.Left;
			return alignment;
		}
		bool GetShowLabel(DataLabelBase dataLabel, ChartViewType viewType) {
			if (IsPieChartGroup(viewType) && dataLabel.ShowPercent && dataLabel.ShowCategoryName && !dataLabel.ShowValue && !dataLabel.ShowSeriesName)
				return true;
			if (dataLabel.ShowValue || dataLabel.ShowPercent)
				return false;
			return IsAreaChartGroup(viewType) || IsFilledRadar() ? dataLabel.ShowSeriesName : dataLabel.ShowCategoryName;
		}
		bool GetShowLabelAndPercent(DataLabelBase dataLabel, ChartViewType viewType) {
			bool showLabelAndPercent = dataLabel.ShowCategoryName && dataLabel.ShowPercent;
			bool options = showLabelAndPercent && !dataLabel.ShowValue && !dataLabel.ShowBubbleSize && !dataLabel.ShowSeriesName;
			return options && IsPieChartGroup(viewType);
		}
		bool IsPieChartGroup(ChartViewType viewType) {
			return viewType == ChartViewType.Pie || viewType == ChartViewType.Pie3D ||
				   viewType == ChartViewType.OfPie || viewType == ChartViewType.Doughnut;
		}
		bool IsAreaChartGroup(ChartViewType viewType) {
			return viewType == ChartViewType.Area || viewType == ChartViewType.Area3D;
		}
		bool IsFilledRadar() {
			RadarChartView radarView = Chart.Views[0] as RadarChartView;
			if (radarView == null)
				return false;
			return radarView.RadarStyle == RadarChartStyle.Filled;
		}
		protected internal bool WithExtendedContent(DataLabelBase dataLabel, ChartViewType viewType) {
			if (IsPieChartGroup(viewType) && dataLabel.ShowPercent && dataLabel.ShowCategoryName && !dataLabel.ShowSeriesName
			   && !dataLabel.ShowValue && dataLabel.Separator == XlsChartTextBuilder.NewLine)
				return false;
			int numberOfFlags = BoolToByte(dataLabel.ShowValue) + BoolToByte(dataLabel.ShowCategoryName) + BoolToByte(dataLabel.ShowSeriesName);
			return numberOfFlags + BoolToByte(dataLabel.ShowPercent) > 1 || numberOfFlags + BoolToByte(dataLabel.ShowBubbleSize) > 1;
		}
		byte BoolToByte(bool value) {
			return value ? (byte)1 : (byte)0;
		}
		protected internal bool IsWrapped(DataLabelBase dataLabel, ChartViewType viewType) {
			bool options = !dataLabel.ShowValue && !dataLabel.ShowPercent && !dataLabel.ShowBubbleSize;
			if (IsAreaChartGroup(viewType) || IsFilledRadar())
				return options && !dataLabel.ShowSeriesName && dataLabel.ShowCategoryName;
			return options && dataLabel.ShowSeriesName && !dataLabel.ShowCategoryName;
		}
		#endregion
		#region Write Methods
		#region WriteText
		void WriteText(DataLabelBase dataLabel, ChartViewType viewType) {
			XlsCommandChartText command = PrepareTextCommand(dataLabel, viewType);
			command.Write(StreamWriter);
		}
		protected internal XlsCommandChartText PrepareTextCommand(DataLabelBase dataLabel, ChartViewType viewType) {
			DataLabel label = dataLabel as DataLabel;
			bool richText = false;
			if (label != null)
				richText = label.Text.TextType == ChartTextType.Rich;
			XlsCommandChartText command;
			if (richText)
				command = PrepareTextProperties(label.Text as ChartRichText);
			else
				command = PrepareTextProperties(dataLabel.TextProperties);
			command.ShowKey = dataLabel.ShowLegendKey;
			command.ShowValue = dataLabel.ShowPercent ? false : dataLabel.ShowValue;
			command.Deleted = dataLabel.Delete;
			command.ShowLabelAndPercent = GetShowLabelAndPercent(dataLabel, viewType);
			command.ShowPercent = !IsPieChartGroup(viewType) ? false : dataLabel.ShowPercent;
			command.ShowBubbleSize = viewType != ChartViewType.Bubble || dataLabel.ShowValue || dataLabel.ShowCategoryName ? false : dataLabel.ShowBubbleSize;
			command.ShowLabel = GetShowLabel(dataLabel, viewType);
			if (label != null) {
				command.IsManualDataLabelPos = !label.Layout.Auto;
				ChartTextType textType = label.Text.TextType;
				command.AutoText = textType == ChartTextType.Auto || textType == ChartTextType.None;
			}
			else
				command.AutoText = true;
			command.DataLabelPos = dataLabel.LabelPosition;
			return command;
		}
		#endregion
		#region WritePosition
		void WritePosition(LayoutOptions layout) {
			XlsCommandChartPos command = layout == null ? PrepareEmptyPositonCommand() : PreparePositonCommand(layout);
			command.Write(StreamWriter);
		}
		XlsCommandChartPos PreparePositonCommand(LayoutOptions layout) {
			XlsCommandChartPos command = PrepareEmptyPositonCommand();
			return command;
		}
		XlsCommandChartPos PrepareEmptyPositonCommand() {
			XlsCommandChartPos command = new XlsCommandChartPos();
			command.TopLeftMode = XlsChartPosMode.Parent;
			command.BottomRightMode = XlsChartPosMode.Parent;
			return command;
		}
		#endregion
		#region WriteBRAI
		void WriteBRAI(NumberFormatOptions numberFormat) {
			XlsCommandChartDataRef command = PrepareBRAICommand(numberFormat);
			command.Write(StreamWriter);
		}
		XlsCommandChartDataRef PrepareBRAICommand(NumberFormatOptions numberFormat) {
			XlsCommandChartDataRef command = new XlsCommandChartDataRef();
			command.DataRef.Id = XlsChartDataRefId.Name;
			command.DataRef.DataType = XlsChartDataRefType.Literal;
			command.DataRef.IsSourceLinked = numberFormat.SourceLinked;
			command.DataRef.NumberFormatId = ExportStyleSheet.GetNumberFormatId(numberFormat.NumberFormatId);
			return command;
		}
		#endregion
		#region WriteObjectLink
		void WriteObjectLinkForDataLabels(int pointIndex) {
			XlsCommandChartTextObjectLink command = PrepareObjectLinkCommand(pointIndex);
			command.Write(StreamWriter);
		}
		XlsCommandChartTextObjectLink PrepareObjectLinkCommand(int pointIndex) {
			XlsCommandChartTextObjectLink command = new XlsCommandChartTextObjectLink();
			command.LinkType = XlsChartTextObjectLinkType.SeriesOrDataPoint;
			command.SeriesIndex = seriesIndex;
			command.PointIndex = pointIndex;
			return command;
		}
		void WriteObjectlink(XlsChartTextObjectLinkType linkType) {
			XlsCommandChartTextObjectLink command = new XlsCommandChartTextObjectLink();
			command.LinkType = linkType;
			command.Write(StreamWriter);
		}
		#endregion
		#region WriteDataLabExtContents
		void WriteDataLabExtContents(DataLabelBase dataLabel, ChartViewType viewType, bool isSeries) {
			XlsCommandChartDataLabelExtContents command;
			if ((IsAreaChartGroup(viewType) || IsFilledRadar()) && isSeries)
				command = PrepareDataLabExtContentsForAreaOrFilledRadar();
			else
				command = PrepareDataLabExtContents(dataLabel, viewType);
			command.Write(StreamWriter);
		}
		protected internal XlsCommandChartDataLabelExtContents PrepareDataLabExtContents(DataLabelBase dataLabel, ChartViewType viewType) {
			XlsCommandChartDataLabelExtContents command = new XlsCommandChartDataLabelExtContents();
			command.ShowSeriesName = dataLabel.ShowSeriesName;
			command.ShowCategoryName = dataLabel.ShowCategoryName;
			command.ShowValue = dataLabel.ShowValue;
			command.ShowPercent = !IsPieChartGroup(viewType) ? false : dataLabel.ShowPercent;
			command.ShowBubbleSizes = viewType != ChartViewType.Bubble ? false : dataLabel.ShowBubbleSize;
			command.Separator = dataLabel.Separator == DataLabelBase.DefaultSeparator ? String.Empty : dataLabel.Separator;
			return command;
		}
		protected internal XlsCommandChartDataLabelExtContents PrepareDataLabExtContentsForAreaOrFilledRadar() {
			XlsCommandChartDataLabelExtContents command = new XlsCommandChartDataLabelExtContents();
			command.ShowCategoryName = true;
			return command;
		}
		#endregion
		#endregion
	}
}
