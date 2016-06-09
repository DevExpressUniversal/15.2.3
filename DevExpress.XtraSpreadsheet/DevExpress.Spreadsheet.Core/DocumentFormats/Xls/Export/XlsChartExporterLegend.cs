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
using DevExpress.Export.Xl;
using DevExpress.Office.DrawingML;
#if !SL
using System.Drawing;
using DevExpress.Office.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public partial class XlsChartExporter {
		#region Field
		XlsCommandChartLegend commandLegend = null;
		XlsCommandChartPos commandLegendPos = null;
		XlsCommandChartFrame commandLegendFrame = null;
		#endregion
		protected void WriteLegend() {
			Legend legend = chart.Legend;
			if (!legend.Visible || viewIndex > 0)
				return;
			WriteChartLegend(legend);
			WriteBegin();
			WriteLegendPos(legend);
			WriteLegendAttachedLabel(legend);
			WriteLegendFrame(legend);
			WriteLegendLayout12(legend);
			WriteLegendOverlay(legend);
			WriteLegendEndBlock();
			WriteEnd();
		}
		protected void WriteChartLegend(Legend legend) {
			LayoutOptions layout = legend.Layout;
			commandLegend = new XlsCommandChartLegend();
			if (layout.Auto) {
				commandLegend.AutoPosition = true;
				commandLegend.AutoXPos = true;
				commandLegend.AutoYPos = true;
				commandLegend.Left = 0;
				commandLegend.Top = 0;
				commandLegend.Width = 0;
				commandLegend.Height = 0;
			}
			else {
				commandLegend.AutoPosition = false;
				commandLegend.AutoXPos = false;
				commandLegend.AutoYPos = false;
				commandLegend.Left = (int)(layout.Left.Value * 4000);
				commandLegend.Top = (int)(layout.Top.Value * 4000);
				commandLegend.Width = (int)(layout.Width.Value * 4000);
				commandLegend.Height = (int)(layout.Height.Value * 4000);
			}
			commandLegend.IsVertical = legend.Position != LegendPosition.Bottom && legend.Position != LegendPosition.Top;
			commandLegend.WasDataTable = false;
			commandLegend.Write(StreamWriter);
		}
		protected void WriteLegendPos(Legend legend) {
			XlsCommandChartPos command = new XlsCommandChartPos();
			command.TopLeftMode = XlsChartPosMode.RelativeChart;
			command.BottomRightMode = XlsChartPosMode.Parent;
			command.Left = 0;
			command.Top = 0;
			command.Width = 0;
			command.Height = 0;
			command.Write(StreamWriter);
			this.commandLegendPos = command;
		}
		protected void WriteLegendAttachedLabel(Legend legend) {
			WriteLegendText(legend);
			WriteBegin();
			WriteLegendTextPos(legend);
			WriteFontX(legend.TextProperties.Paragraphs, ChartElement.Legend);
			WriteLegendRef(legend);
			WriteEnd();
		}
		protected void WriteLegendText(Legend legend) {
			XlsCommandChartText command = new XlsCommandChartText();
			DrawingTextParagraphProperties paragraphProperties = GetFirstParagraphProperties(legend.TextProperties.Paragraphs);
			IDrawingFill fill = paragraphProperties.DefaultCharacterProperties.Fill;
			if (fill.FillType == DrawingFillType.Solid)
				PrepareSolidFill(command, fill as DrawingSolidFill, false);
			else {
				command.TextColor = DXColor.Black;
				command.TextColorIndex = chart.DocumentModel.StyleSheet.Palette.GetPaletteNearestColorIndex(DXColor.Black);
			}
			command.AutoText = true;
			command.IsGenerated = true;
			command.HorizontalAlignment = DrawingTextAlignmentType.Center;
			command.VerticalAlignment = DrawingTextAnchoringType.Center;
			command.IsTransparent = true;
			command.IsManualDataLabelPos = false;
			command.DataLabelPos = DataLabelPosition.Default;
			command.TextReadingOrder = XlReadingOrder.Context;
			command.Write(StreamWriter);
		}
		protected void WriteLegendTextPos(Legend legend) {
			XlsCommandChartPos command = new XlsCommandChartPos();
			command.TopLeftMode = XlsChartPosMode.Parent;
			command.BottomRightMode = XlsChartPosMode.Parent;
			command.Write(StreamWriter);
		}
		protected void WriteLegendRef(Legend legend) {
			XlsCommandChartDataRef command = new XlsCommandChartDataRef();
			command.DataRef.Id = XlsChartDataRefId.Name;
			command.DataRef.DataType = XlsChartDataRefType.Literal;
			command.DataRef.IsSourceLinked = true;
			command.Write(StreamWriter);
		}
		protected void WriteLegendLayout12(Legend legend) {
			XlsCommandChartLayout12 command = new XlsCommandChartLayout12();
			command.CheckSum = CalcLegendLayout12CheckSum();
			command.LegendPos = legend.Position;
			LayoutOptions layout = legend.Layout;
			command.X = layout.Left.Value;
			command.XMode = layout.Left.Mode;
			command.Y = layout.Top.Value;
			command.YMode = layout.Top.Mode;
			command.Width = layout.Width.Value;
			command.WidthMode = layout.Width.Mode;
			command.Height = layout.Height.Value;
			command.HeightMode = layout.Height.Mode;
			command.Write(StreamWriter);
		}
		protected void WriteLegendOverlay(Legend legend) {
			if (!legend.Overlay)
				return;
			XlsChartExtProperties extProperties = new XlsChartExtProperties();
			extProperties.Parent = XlsChartExtPropParent.Legend;
			extProperties.Items.Add(new XlsChartExtPropOverlay() { Value = true });
			WriteExtProperties(extProperties);
		}
		protected void WriteLegendFrame(Legend legend) {
			this.commandLegendFrame = WriteFrame(true, true);
			WriteBegin();
			ResetCrc32();
			WriteLineFormat(legend.ShapeProperties, ChartElement.Legend);
			WriteAreaFormat(legend.ShapeProperties, ChartElement.Legend);
			WriteLegendStartBlock(XlsChartDefs.LegendOfChartContext);
			WriteStartBlock(XlsChartBlockObjectKind.Frame, XlsChartDefs.FrameContext, 0, 0);
			WriteShapeFormat(legend.ShapeProperties, XlsChartDefs.DataContext);
			WriteEndBlock(XlsChartBlockObjectKind.Frame);
			WriteEnd();
		}
		int CalcLegendLayout12CheckSum() {
			int result = 0;
			result ^= commandLegendPos.Left;
			result ^= commandLegendPos.Top;
			int chartWidth = (int)((commandChart.Width - 8) * DocumentModel.DpiX / 72);
			int chartHeight = (int)((commandChart.Height - 8) * DocumentModel.DpiY / 72);
			if (!IsEmbedded && commandChartSpaceFrame != null && commandChartSpaceFrame.FrameType == XlsChartFrameType.FrameWithShadow) {
				chartWidth -= 2;
				chartHeight -= 2;
			}
			int legendWidth = chartWidth * commandLegend.Width / 4000;
			int legendHeight = chartHeight * commandLegend.Height / 4000;
			result ^= legendWidth;
			result ^= legendHeight;
			result ^= commandLegend.AutoXPos ? 1 : 0;
			result ^= commandLegend.AutoYPos ? 1 : 0;
			if (commandLegendFrame != null)
				result ^= commandLegendFrame.AutoSize ? 1 : 0;
			return result;
		}
	}
}
