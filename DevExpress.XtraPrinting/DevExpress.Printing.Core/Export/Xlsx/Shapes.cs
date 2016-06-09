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
using System.Text;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Export.Xl;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraExport.Xlsx {
	using DevExpress.XtraExport.Implementation;
	partial class XlsxDataAwareExporter {
		#region Translation tables
		static Dictionary<XlGeometryPreset, string> geometryPresetTable = CreateGeometryPresetTable();
		static Dictionary<XlGeometryPreset, string> CreateGeometryPresetTable() {
			Dictionary<XlGeometryPreset, string> result = new Dictionary<XlGeometryPreset, string>();
			result.Add(XlGeometryPreset.Line, "line");
			result.Add(XlGeometryPreset.LineInv, "lineInv");
			result.Add(XlGeometryPreset.Triangle, "triangle");
			result.Add(XlGeometryPreset.RtTriangle, "rtTriangle");
			result.Add(XlGeometryPreset.Rect, "rect");
			result.Add(XlGeometryPreset.Diamond, "diamond");
			result.Add(XlGeometryPreset.Parallelogram, "parallelogram");
			result.Add(XlGeometryPreset.Trapezoid, "trapezoid");
			result.Add(XlGeometryPreset.NonIsoscelesTrapezoid, "nonIsoscelesTrapezoid");
			result.Add(XlGeometryPreset.Pentagon, "pentagon");
			result.Add(XlGeometryPreset.Hexagon, "hexagon");
			result.Add(XlGeometryPreset.Heptagon, "heptagon");
			result.Add(XlGeometryPreset.Octagon, "octagon");
			result.Add(XlGeometryPreset.Decagon, "decagon");
			result.Add(XlGeometryPreset.Dodecagon, "dodecagon");
			result.Add(XlGeometryPreset.Star4, "star4");
			result.Add(XlGeometryPreset.Star5, "star5");
			result.Add(XlGeometryPreset.Star6, "star6");
			result.Add(XlGeometryPreset.Star7, "star7");
			result.Add(XlGeometryPreset.Star8, "star8");
			result.Add(XlGeometryPreset.Star10, "star10");
			result.Add(XlGeometryPreset.Star12, "star12");
			result.Add(XlGeometryPreset.Star16, "star16");
			result.Add(XlGeometryPreset.Star24, "star24");
			result.Add(XlGeometryPreset.Star32, "star32");
			result.Add(XlGeometryPreset.RoundRect, "roundRect");
			result.Add(XlGeometryPreset.Round1Rect, "round1Rect");
			result.Add(XlGeometryPreset.Round2SameRect, "round2SameRect");
			result.Add(XlGeometryPreset.Round2DiagRect, "round2DiagRect");
			result.Add(XlGeometryPreset.SnipRoundRect, "snipRoundRect");
			result.Add(XlGeometryPreset.Snip1Rect, "snip1Rect");
			result.Add(XlGeometryPreset.Snip2SameRect, "snip2SameRect");
			result.Add(XlGeometryPreset.Snip2DiagRect, "snip2DiagRect");
			result.Add(XlGeometryPreset.Plaque, "plaque");
			result.Add(XlGeometryPreset.Ellipse, "ellipse");
			result.Add(XlGeometryPreset.Teardrop, "teardrop");
			result.Add(XlGeometryPreset.HomePlate, "homePlate");
			result.Add(XlGeometryPreset.Chevron, "chevron");
			result.Add(XlGeometryPreset.PieWedge, "pieWedge");
			result.Add(XlGeometryPreset.Pie, "pie");
			result.Add(XlGeometryPreset.BlockArc, "blockArc");
			result.Add(XlGeometryPreset.Donut, "donut");
			result.Add(XlGeometryPreset.NoSmoking, "noSmoking");
			result.Add(XlGeometryPreset.RightArrow, "rightArrow");
			result.Add(XlGeometryPreset.LeftArrow, "leftArrow");
			result.Add(XlGeometryPreset.UpArrow, "upArrow");
			result.Add(XlGeometryPreset.DownArrow, "downArrow");
			result.Add(XlGeometryPreset.StripedRightArrow, "stripedRightArrow");
			result.Add(XlGeometryPreset.NotchedRightArrow, "notchedRightArrow");
			result.Add(XlGeometryPreset.BentUpArrow, "bentUpArrow");
			result.Add(XlGeometryPreset.LeftRightArrow, "leftRightArrow");
			result.Add(XlGeometryPreset.UpDownArrow, "upDownArrow");
			result.Add(XlGeometryPreset.LeftUpArrow, "leftUpArrow");
			result.Add(XlGeometryPreset.LeftRightUpArrow, "leftRightUpArrow");
			result.Add(XlGeometryPreset.QuadArrow, "quadArrow");
			result.Add(XlGeometryPreset.LeftArrowCallout, "leftArrowCallout");
			result.Add(XlGeometryPreset.RightArrowCallout, "rightArrowCallout");
			result.Add(XlGeometryPreset.UpArrowCallout, "upArrowCallout");
			result.Add(XlGeometryPreset.DownArrowCallout, "downArrowCallout");
			result.Add(XlGeometryPreset.LeftRightArrowCallout, "leftRightArrowCallout");
			result.Add(XlGeometryPreset.UpDownArrowCallout, "upDownArrowCallout");
			result.Add(XlGeometryPreset.QuadArrowCallout, "quadArrowCallout");
			result.Add(XlGeometryPreset.BentArrow, "bentArrow");
			result.Add(XlGeometryPreset.UturnArrow, "uturnArrow");
			result.Add(XlGeometryPreset.CircularArrow, "circularArrow");
			result.Add(XlGeometryPreset.LeftCircularArrow, "leftCircularArrow");
			result.Add(XlGeometryPreset.LeftRightCircularArrow, "leftRightCircularArrow");
			result.Add(XlGeometryPreset.CurvedRightArrow, "curvedRightArrow");
			result.Add(XlGeometryPreset.CurvedLeftArrow, "curvedLeftArrow");
			result.Add(XlGeometryPreset.CurvedUpArrow, "curvedUpArrow");
			result.Add(XlGeometryPreset.CurvedDownArrow, "curvedDownArrow");
			result.Add(XlGeometryPreset.SwooshArrow, "swooshArrow");
			result.Add(XlGeometryPreset.Cube, "cube");
			result.Add(XlGeometryPreset.Can, "can");
			result.Add(XlGeometryPreset.LightningBolt, "lightningBolt");
			result.Add(XlGeometryPreset.Heart, "heart");
			result.Add(XlGeometryPreset.Sun, "sun");
			result.Add(XlGeometryPreset.Moon, "moon");
			result.Add(XlGeometryPreset.SmileyFace, "smileyFace");
			result.Add(XlGeometryPreset.IrregularSeal1, "irregularSeal1");
			result.Add(XlGeometryPreset.IrregularSeal2, "irregularSeal2");
			result.Add(XlGeometryPreset.FoldedCorner, "foldedCorner");
			result.Add(XlGeometryPreset.Bevel, "bevel");
			result.Add(XlGeometryPreset.Frame, "frame");
			result.Add(XlGeometryPreset.HalfFrame, "halfFrame");
			result.Add(XlGeometryPreset.Corner, "corner");
			result.Add(XlGeometryPreset.DiagStripe, "diagStripe");
			result.Add(XlGeometryPreset.Chord, "chord");
			result.Add(XlGeometryPreset.Arc, "arc");
			result.Add(XlGeometryPreset.LeftBracket, "leftBracket");
			result.Add(XlGeometryPreset.RightBracket, "rightBracket");
			result.Add(XlGeometryPreset.LeftBrace, "leftBrace");
			result.Add(XlGeometryPreset.RightBrace, "rightBrace");
			result.Add(XlGeometryPreset.BracketPair, "bracketPair");
			result.Add(XlGeometryPreset.BracePair, "bracePair");
			result.Add(XlGeometryPreset.StraightConnector1, "straightConnector1");
			result.Add(XlGeometryPreset.BentConnector2, "bentConnector2");
			result.Add(XlGeometryPreset.BentConnector3, "bentConnector3");
			result.Add(XlGeometryPreset.BentConnector4, "bentConnector4");
			result.Add(XlGeometryPreset.BentConnector5, "bentConnector5");
			result.Add(XlGeometryPreset.CurvedConnector2, "curvedConnector2");
			result.Add(XlGeometryPreset.CurvedConnector3, "curvedConnector3");
			result.Add(XlGeometryPreset.CurvedConnector4, "curvedConnector4");
			result.Add(XlGeometryPreset.CurvedConnector5, "curvedConnector5");
			result.Add(XlGeometryPreset.Callout1, "callout1");
			result.Add(XlGeometryPreset.Callout2, "callout2");
			result.Add(XlGeometryPreset.Callout3, "callout3");
			result.Add(XlGeometryPreset.AccentCallout1, "accentCallout1");
			result.Add(XlGeometryPreset.AccentCallout2, "accentCallout2");
			result.Add(XlGeometryPreset.AccentCallout3, "accentCallout3");
			result.Add(XlGeometryPreset.BorderCallout1, "borderCallout1");
			result.Add(XlGeometryPreset.BorderCallout2, "borderCallout2");
			result.Add(XlGeometryPreset.BorderCallout3, "borderCallout3");
			result.Add(XlGeometryPreset.AccentBorderCallout1, "accentBorderCallout1");
			result.Add(XlGeometryPreset.AccentBorderCallout2, "accentBorderCallout2");
			result.Add(XlGeometryPreset.AccentBorderCallout3, "accentBorderCallout3");
			result.Add(XlGeometryPreset.WedgeRectCallout, "wedgeRectCallout");
			result.Add(XlGeometryPreset.WedgeRoundRectCallout, "wedgeRoundRectCallout");
			result.Add(XlGeometryPreset.WedgeEllipseCallout, "wedgeEllipseCallout");
			result.Add(XlGeometryPreset.CloudCallout, "cloudCallout");
			result.Add(XlGeometryPreset.Cloud, "cloud");
			result.Add(XlGeometryPreset.Ribbon, "ribbon");
			result.Add(XlGeometryPreset.Ribbon2, "ribbon2");
			result.Add(XlGeometryPreset.EllipseRibbon, "ellipseRibbon");
			result.Add(XlGeometryPreset.EllipseRibbon2, "ellipseRibbon2");
			result.Add(XlGeometryPreset.LeftRightRibbon, "leftRightRibbon");
			result.Add(XlGeometryPreset.VerticalScroll, "verticalScroll");
			result.Add(XlGeometryPreset.HorizontalScroll, "horizontalScroll");
			result.Add(XlGeometryPreset.Wave, "wave");
			result.Add(XlGeometryPreset.DoubleWave, "doubleWave");
			result.Add(XlGeometryPreset.Plus, "plus");
			result.Add(XlGeometryPreset.FlowChartProcess, "flowChartProcess");
			result.Add(XlGeometryPreset.FlowChartDecision, "flowChartDecision");
			result.Add(XlGeometryPreset.FlowChartInputOutput, "flowChartInputOutput");
			result.Add(XlGeometryPreset.FlowChartPredefinedProcess, "flowChartPredefinedProcess");
			result.Add(XlGeometryPreset.FlowChartInternalStorage, "flowChartInternalStorage");
			result.Add(XlGeometryPreset.FlowChartDocument, "flowChartDocument");
			result.Add(XlGeometryPreset.FlowChartMultidocument, "flowChartMultidocument");
			result.Add(XlGeometryPreset.FlowChartTerminator, "flowChartTerminator");
			result.Add(XlGeometryPreset.FlowChartPreparation, "flowChartPreparation");
			result.Add(XlGeometryPreset.FlowChartManualInput, "flowChartManualInput");
			result.Add(XlGeometryPreset.FlowChartManualOperation, "flowChartManualOperation");
			result.Add(XlGeometryPreset.FlowChartConnector, "flowChartConnector");
			result.Add(XlGeometryPreset.FlowChartPunchedCard, "flowChartPunchedCard");
			result.Add(XlGeometryPreset.FlowChartPunchedTape, "flowChartPunchedTape");
			result.Add(XlGeometryPreset.FlowChartSummingJunction, "flowChartSummingJunction");
			result.Add(XlGeometryPreset.FlowChartOr, "flowChartOr");
			result.Add(XlGeometryPreset.FlowChartCollate, "flowChartCollate");
			result.Add(XlGeometryPreset.FlowChartSort, "flowChartSort");
			result.Add(XlGeometryPreset.FlowChartExtract, "flowChartExtract");
			result.Add(XlGeometryPreset.FlowChartMerge, "flowChartMerge");
			result.Add(XlGeometryPreset.FlowChartOfflineStorage, "flowChartOfflineStorage");
			result.Add(XlGeometryPreset.FlowChartOnlineStorage, "flowChartOnlineStorage");
			result.Add(XlGeometryPreset.FlowChartMagneticTape, "flowChartMagneticTape");
			result.Add(XlGeometryPreset.FlowChartMagneticDisk, "flowChartMagneticDisk");
			result.Add(XlGeometryPreset.FlowChartMagneticDrum, "flowChartMagneticDrum");
			result.Add(XlGeometryPreset.FlowChartDisplay, "flowChartDisplay");
			result.Add(XlGeometryPreset.FlowChartDelay, "flowChartDelay");
			result.Add(XlGeometryPreset.FlowChartAlternateProcess, "flowChartAlternateProcess");
			result.Add(XlGeometryPreset.FlowChartOffpageConnector, "flowChartOffpageConnector");
			result.Add(XlGeometryPreset.ActionButtonBlank, "actionButtonBlank");
			result.Add(XlGeometryPreset.ActionButtonHome, "actionButtonHome");
			result.Add(XlGeometryPreset.ActionButtonHelp, "actionButtonHelp");
			result.Add(XlGeometryPreset.ActionButtonInformation, "actionButtonInformation");
			result.Add(XlGeometryPreset.ActionButtonForwardNext, "actionButtonForwardNext");
			result.Add(XlGeometryPreset.ActionButtonBackPrevious, "actionButtonBackPrevious");
			result.Add(XlGeometryPreset.ActionButtonEnd, "actionButtonEnd");
			result.Add(XlGeometryPreset.ActionButtonBeginning, "actionButtonBeginning");
			result.Add(XlGeometryPreset.ActionButtonReturn, "actionButtonReturn");
			result.Add(XlGeometryPreset.ActionButtonDocument, "actionButtonDocument");
			result.Add(XlGeometryPreset.ActionButtonSound, "actionButtonSound");
			result.Add(XlGeometryPreset.ActionButtonMovie, "actionButtonMovie");
			result.Add(XlGeometryPreset.Gear6, "gear6");
			result.Add(XlGeometryPreset.Gear9, "gear9");
			result.Add(XlGeometryPreset.Funnel, "funnel");
			result.Add(XlGeometryPreset.MathPlus, "mathPlus");
			result.Add(XlGeometryPreset.MathMinus, "mathMinus");
			result.Add(XlGeometryPreset.MathMultiply, "mathMultiply");
			result.Add(XlGeometryPreset.MathDivide, "mathDivide");
			result.Add(XlGeometryPreset.MathEqual, "mathEqual");
			result.Add(XlGeometryPreset.MathNotEqual, "mathNotEqual");
			result.Add(XlGeometryPreset.CornerTabs, "cornerTabs");
			result.Add(XlGeometryPreset.SquareTabs, "squareTabs");
			result.Add(XlGeometryPreset.PlaqueTabs, "plaqueTabs");
			result.Add(XlGeometryPreset.ChartX, "chartX");
			result.Add(XlGeometryPreset.ChartStar, "chartStar");
			result.Add(XlGeometryPreset.ChartPlus, "chartPlus");
			return result;
		}
		static Dictionary<XlThemeColor, string> schemeColorTable = CreateSchemeColorTable();
		static Dictionary<XlThemeColor, string> CreateSchemeColorTable() {
			Dictionary<XlThemeColor, string> result = new Dictionary<XlThemeColor, string>();
			result.Add(XlThemeColor.Accent1, "accent1");
			result.Add(XlThemeColor.Accent2, "accent2");
			result.Add(XlThemeColor.Accent3, "accent3");
			result.Add(XlThemeColor.Accent4, "accent4");
			result.Add(XlThemeColor.Accent5, "accent5");
			result.Add(XlThemeColor.Accent6, "accent6");
			result.Add(XlThemeColor.Dark1, "tx1");
			result.Add(XlThemeColor.Dark2, "tx2");
			result.Add(XlThemeColor.Light1, "bg1");
			result.Add(XlThemeColor.Light2, "bg2");
			result.Add(XlThemeColor.FollowedHyperlink, "folHlink");
			result.Add(XlThemeColor.Hyperlink, "hlink");
			return result;
		}
		static Dictionary<XlOutlineDashing, string> presetDashTable = CreatePresetDashTable();
		static Dictionary<XlOutlineDashing, string> CreatePresetDashTable() {
			Dictionary<XlOutlineDashing, string> result = new Dictionary<XlOutlineDashing, string>();
			result.Add(XlOutlineDashing.Solid, "solid");
			result.Add(XlOutlineDashing.Dash, "dash");
			result.Add(XlOutlineDashing.DashDot, "dashDot");
			result.Add(XlOutlineDashing.Dot, "dot");
			result.Add(XlOutlineDashing.LongDash, "lgDash");
			result.Add(XlOutlineDashing.LongDashDot, "lgDashDot");
			result.Add(XlOutlineDashing.LongDashDotDot, "lgDashDotDot");
			result.Add(XlOutlineDashing.SystemDash, "sysDash");
			result.Add(XlOutlineDashing.SystemDashDot, "sysDashDot");
			result.Add(XlOutlineDashing.SystemDashDotDot, "sysDashDotDot");
			result.Add(XlOutlineDashing.SystemDot, "sysDot");
			return result;
		}
		#endregion
		bool ShouldExportShapes() {
			IXlShapeContainer container = currentSheet as IXlShapeContainer;
			if(container != null) {
				foreach(XlShape shape in container.Shapes) {
					if(shape.GeometryPreset == XlGeometryPreset.Line)
						return true;
				}
			}
			return false;
		}
		void GenerateShape(XlShape shape) {
			if(shape == null || shape.GeometryPreset != XlGeometryPreset.Line)
				return;
			switch(shape.AnchorType) {
				case XlAnchorType.TwoCell:
					WriteTwoCellAnchor(shape, WriteConnectorShape);
					break;
				case XlAnchorType.OneCell:
					WriteOneCellAnchor(shape, WriteConnectorShape);
					break;
				case XlAnchorType.Absolute:
					WriteAbsoluteAnchor(shape, WriteConnectorShape);
					break;
			}
		}
		void WriteConnectorShape(XlDrawingObjectBase drawingObject) {
			XlShape shape = (XlShape)drawingObject;
			WriteStartElement("cxnSp", spreadsheetDrawingNamespace);
			try {
				WriteStringValue("macro", string.Empty);
				WriteConnectorShapeNonVisualProperties(shape);
				WriteShapeProperties(shape);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteConnectorShapeNonVisualProperties(XlShape shape) {
			WriteStartElement("nvCxnSpPr", spreadsheetDrawingNamespace);
			try {
				WriteNonVisualDrawingProperties(shape);
				WriteNonVisualConnectorShapeProperties(shape);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteNonVisualDrawingProperties(XlShape shape) {
			WriteStartElement("cNvPr", spreadsheetDrawingNamespace);
			try {
				shapeId++;
				int id = shapeId + 1;
				WriteIntValue("id", id);
				string name = shape.Name;
				if(string.IsNullOrEmpty(name))
					name = string.Format("Straight Connector {0}", shapeId);
				WriteStringValue("name", name);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteNonVisualConnectorShapeProperties(XlShape shape) {
			WriteStartElement("cNvCxnSpPr", spreadsheetDrawingNamespace);
			try {
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteShapeProperties(XlShape shape) {
			WriteStartElement("spPr", spreadsheetDrawingNamespace);
			try {
				WriteGraphicFrame(shape);
				WritePresetGeometry(shape);
				WriteShapeOutline(shape);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteGraphicFrame(XlShape shape) {
			XlGraphicFrame frame = shape.Frame;
			if(frame.IsDefault())
				return;
			WriteStartElement("xfrm", drawingMLNamespace);
			try {
				if(frame.Rotation != 0.0)
					WriteIntValue("rot", (int)(frame.Rotation * 60000));
				if(frame.FlipHorizontal)
					WriteBoolValue("flipH", true);
				if(frame.FlipVertical)
					WriteBoolValue("flipV", true);
			}
			finally {
				WriteEndElement();
			}
		}
		void WritePresetGeometry(XlShape shape) {
			WriteStartElement("prstGeom", drawingMLNamespace);
			try {
				WriteStringValue("prst", geometryPresetTable[shape.GeometryPreset]);
				WriteShapeAdjustValues(shape);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteShapeAdjustValues(XlShape shape) {
			WriteStartElement("avLst", drawingMLNamespace);
			try {
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteShapeOutline(XlShape shape) {
			XlOutline outline = shape.Outline;
			if(outline.Color.IsEmpty)
				return;
			WriteStartElement("ln", drawingMLNamespace);
			try {
				int widthInEMU = (int)(outline.Width * 12700);
				if(widthInEMU > 0)
					WriteIntValue("w", widthInEMU);
				WriteShapeOutlineFill(outline);
				WriteShapeOutlinePresetDash(outline);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteShapeOutlineFill(XlOutline outline) {
			WriteStartElement("solidFill", drawingMLNamespace);
			try {
				if(outline.Color.ColorType == XlColorType.Rgb)
					WriteShapeColorRGB(outline.Color);
				else if(outline.Color.ColorType == XlColorType.Theme)
					WriteShapeColorScheme(outline.Color);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteShapeColorRGB(XlColor color) {
			WriteStartElement("srgbClr", drawingMLNamespace);
			try {
				Color rgbColor = color.Rgb;
				string rgbHex = string.Format("{0:X2}{1:X2}{2:X2}", (int)rgbColor.R, (int)rgbColor.G, (int)rgbColor.B);
				WriteStringValue("val", rgbHex);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteShapeColorScheme(XlColor color) {
			WriteStartElement("schemeClr", drawingMLNamespace);
			try {
				WriteStringValue("val", schemeColorTable[color.ThemeColor]);
				WriteShapeColorTint(color.Tint);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteShapeOutlinePresetDash(XlOutline outline) {
			if(outline.Dashing == XlOutlineDashing.Solid)
				return;
			WriteStartElement("prstDash", drawingMLNamespace);
			try {
				WriteStringValue("val", presetDashTable[outline.Dashing]);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteShapeColorTint(double tint) {
			int modulation = Math.Min((int)((1.0 - Math.Abs(tint)) * 100000), 100000);
			int offset = Math.Min((int)(Math.Abs(tint) * 100000), 100000);
			if(offset == 0)
				return;
			WriteStartElement("lumMod", drawingMLNamespace);
			try {
				WriteIntValue("val", modulation);
			}
			finally {
				WriteEndElement();
			}
			if(tint > 0.0) {
				WriteStartElement("lumOff", drawingMLNamespace);
				try {
					WriteIntValue("val", offset);
				}
				finally {
					WriteEndElement();
				}
			}
		}
	}
}
