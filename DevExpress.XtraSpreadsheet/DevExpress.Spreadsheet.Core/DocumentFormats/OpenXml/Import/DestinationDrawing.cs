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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Import.OpenXml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DrawingDestination
	public class DrawingDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		public static Dictionary<ShapeType, string> ShapeTypeDesctiptionTable = GetShapeTypeDesctiptionTable();
		static Dictionary<ShapeType, string> GetShapeTypeDesctiptionTable() {
			var result = new Dictionary<ShapeType, string>();
			result.Add(ShapeType.Line, "Straight Connector");
			result.Add(ShapeType.StraightConnector1, "Straight Arrow Connector");
			result.Add(ShapeType.BentConnector3, "Elbow Connector");
			result.Add(ShapeType.CurvedConnector3, "Curved Connector");
			result.Add(ShapeType.Rect, "Rectangle");
			result.Add(ShapeType.RoundRect, "Rounded Rectangle");
			result.Add(ShapeType.Snip1Rect, "Snip Single Corner Rectangle");
			result.Add(ShapeType.Snip2SameRect, "Snip Same Side Corner Rectangle");
			result.Add(ShapeType.Snip2DiagRect, "Snip Diagonal Corner Rectangle");
			result.Add(ShapeType.SnipRoundRect, "Snip and Round Single Corner Rectangle");
			result.Add(ShapeType.Round1Rect, "Round Single Corner Rectangle");
			result.Add(ShapeType.Round2SameRect, "Round Same Side Corner Rectangle");
			result.Add(ShapeType.Round2DiagRect, "Round Diagonal Corner Rectangle");
			result.Add(ShapeType.Ellipse, "Oval");
			result.Add(ShapeType.Triangle, "Isosceles Triangle");
			result.Add(ShapeType.RtTriangle, "Right Triangle");
			result.Add(ShapeType.Parallelogram, "Parallelogram");
			result.Add(ShapeType.Trapezoid, "Trapezoid");
			result.Add(ShapeType.Diamond, "Diamond");
			result.Add(ShapeType.Pentagon, "Regular Pentagon");
			result.Add(ShapeType.Hexagon, "Hexagon");
			result.Add(ShapeType.Heptagon, "Heptagon");
			result.Add(ShapeType.Octagon, "Octagon");
			result.Add(ShapeType.Decagon, "Decagon");
			result.Add(ShapeType.Dodecagon, "Dodecagon");
			result.Add(ShapeType.Pie, "Pie");
			result.Add(ShapeType.Chord, "Chord");
			result.Add(ShapeType.Teardrop, "Teardrop");
			result.Add(ShapeType.Frame, "Frame");
			result.Add(ShapeType.HalfFrame, "Half Frame");
			result.Add(ShapeType.Corner, "L-Shape");
			result.Add(ShapeType.DiagStripe, "Diagonal Stripe");
			result.Add(ShapeType.Plus, "Cross");
			result.Add(ShapeType.Plaque, "Plaque");
			result.Add(ShapeType.Can, "Can");
			result.Add(ShapeType.Cube, "Cube");
			result.Add(ShapeType.Bevel, "Bevel");
			result.Add(ShapeType.Donut, "Donut");
			result.Add(ShapeType.NoSmoking, "\"No\" Symbol 12");
			result.Add(ShapeType.BlockArc, "Block Arc");
			result.Add(ShapeType.SmileyFace, "Smiley Face");
			result.Add(ShapeType.Heart, "Heart");
			result.Add(ShapeType.LightningBolt, "Lightning Bolt");
			result.Add(ShapeType.Sun, "Sun");
			result.Add(ShapeType.Moon, "Moon");
			result.Add(ShapeType.BracketPair, "Double Bracket");
			result.Add(ShapeType.BracePair, "Double Brace");
			result.Add(ShapeType.LeftBracket, "Left Bracket");
			result.Add(ShapeType.RightBracket, "Right Bracket");
			result.Add(ShapeType.LeftBrace, "Left Brace");
			result.Add(ShapeType.RightBrace, "Right Brace");
			result.Add(ShapeType.RightArrow, "Right Arrow");
			result.Add(ShapeType.LeftArrow, "Left Arrow");
			result.Add(ShapeType.UpArrow, "Up Arrow");
			result.Add(ShapeType.DownArrow, "Down Arrow");
			result.Add(ShapeType.LeftRightArrow, "Left-Right Arrow");
			result.Add(ShapeType.UpDownArrow, "Up-Down Arrow");
			result.Add(ShapeType.QuadArrow, "Quad Arrow");
			result.Add(ShapeType.LeftRightUpArrow, "Left-Right-Up Arrow");
			result.Add(ShapeType.BentArrow, "Bent Arrow");
			result.Add(ShapeType.UturnArrow, "U-Turn Arrow");
			result.Add(ShapeType.LeftUpArrow, "Left-Up Arrow");
			result.Add(ShapeType.BentUpArrow, "Bent-Up Arrow");
			result.Add(ShapeType.CurvedRightArrow, "Curved Right Arrow");
			result.Add(ShapeType.CurvedLeftArrow, "Curved Left Arrow");
			result.Add(ShapeType.CurvedUpArrow, "Curved Up Arrow");
			result.Add(ShapeType.CurvedDownArrow, "Curved Down Arrow");
			result.Add(ShapeType.StripedRightArrow, "Striped Right Arrow");
			result.Add(ShapeType.NotchedRightArrow, "Notched Right Arrow");
			result.Add(ShapeType.HomePlate, "Pentagon");
			result.Add(ShapeType.Chevron, "Chevron");
			result.Add(ShapeType.RightArrowCallout, "Right Arrow Callout");
			result.Add(ShapeType.DownArrowCallout, "Down Arrow Callout");
			result.Add(ShapeType.LeftArrowCallout, "Left Arrow Callout");
			result.Add(ShapeType.UpArrowCallout, "Up Arrow Callout");
			result.Add(ShapeType.LeftRightArrowCallout, "Left-Right Arrow Callout");
			result.Add(ShapeType.QuadArrowCallout, "Quad Arrow Callout");
			result.Add(ShapeType.CircularArrow, "Circular Arrow");
			result.Add(ShapeType.MathPlus, "Plus");
			result.Add(ShapeType.MathMinus, "Minus");
			result.Add(ShapeType.MathMultiply, "Multiply");
			result.Add(ShapeType.MathDivide, "Division");
			result.Add(ShapeType.MathEqual, "Equal");
			result.Add(ShapeType.MathNotEqual, "Not Equal");
			result.Add(ShapeType.FlowChartProcess, "Flowchart: Process");
			result.Add(ShapeType.FlowChartAlternateProcess, "Flowchart: Alternate Process");
			result.Add(ShapeType.FlowChartDecision, "Flowchart: Decision");
			result.Add(ShapeType.FlowChartInputOutput, "Flowchart: Data");
			result.Add(ShapeType.FlowChartPredefinedProcess, "Flowchart: Predefined Process");
			result.Add(ShapeType.FlowChartInternalStorage, "Flowchart: Internal Storage");
			result.Add(ShapeType.FlowChartDocument, "Flowchart: Document");
			result.Add(ShapeType.FlowChartMultidocument, "Flowchart: Multidocument");
			result.Add(ShapeType.FlowChartTerminator, "Flowchart: Terminator");
			result.Add(ShapeType.FlowChartPreparation, "Flowchart: Preparation");
			result.Add(ShapeType.FlowChartManualInput, "Flowchart: Manual Input");
			result.Add(ShapeType.FlowChartManualOperation, "Flowchart: Manual Operation");
			result.Add(ShapeType.FlowChartConnector, "Flowchart: Connector");
			result.Add(ShapeType.FlowChartOffpageConnector, "Flowchart: Off-page Connector");
			result.Add(ShapeType.FlowChartPunchedCard, "Flowchart: Card");
			result.Add(ShapeType.FlowChartPunchedTape, "Flowchart: Punched Tape");
			result.Add(ShapeType.FlowChartSummingJunction, "Flowchart: Summing Junction");
			result.Add(ShapeType.FlowChartOr, "Flowchart: Or");
			result.Add(ShapeType.FlowChartCollate, "Flowchart: Collate");
			result.Add(ShapeType.FlowChartSort, "Flowchart: Sort");
			result.Add(ShapeType.FlowChartExtract, "Flowchart: Extract");
			result.Add(ShapeType.FlowChartMerge, "Flowchart: Merge");
			result.Add(ShapeType.FlowChartOnlineStorage, "Flowchart: Stored Data");
			result.Add(ShapeType.FlowChartMagneticTape, "Flowchart: Sequential Access Storage");
			result.Add(ShapeType.FlowChartMagneticDisk, "Flowchart: Magnetic Disk");
			result.Add(ShapeType.FlowChartMagneticDrum, "Flowchart: Direct Access Storage");
			result.Add(ShapeType.FlowChartDisplay, "Flowchart: Display");
			result.Add(ShapeType.IrregularSeal1, "Explosion 1");
			result.Add(ShapeType.IrregularSeal2, "Explosion 2");
			result.Add(ShapeType.Star4, "4-Point Star");
			result.Add(ShapeType.Star5, "5-Point Star");
			result.Add(ShapeType.Star6, "6-Point Star");
			result.Add(ShapeType.Star7, "7-Point Star");
			result.Add(ShapeType.Star8, "8-Point Star");
			result.Add(ShapeType.Star10, "10-Point Star");
			result.Add(ShapeType.Star12, "12-Point Star");
			result.Add(ShapeType.Star16, "16-Point Star");
			result.Add(ShapeType.Star24, "24-Point Star");
			result.Add(ShapeType.Star32, "32-Point Star");
			result.Add(ShapeType.Ribbon2, "Up Ribbon");
			result.Add(ShapeType.Ribbon, "Down Ribbon");
			result.Add(ShapeType.EllipseRibbon2, "Curved Up Ribbon");
			result.Add(ShapeType.EllipseRibbon, "Curved Down Ribbon");
			result.Add(ShapeType.VerticalScroll, "Vertical Scroll");
			result.Add(ShapeType.HorizontalScroll, "Horizontal Scroll");
			result.Add(ShapeType.Wave, "Wave");
			result.Add(ShapeType.DoubleWave, "Double Wave");
			result.Add(ShapeType.WedgeRectCallout, "Rectangular Callout");
			result.Add(ShapeType.WedgeRoundRectCallout, "Rounded Rectangular Callout");
			result.Add(ShapeType.WedgeEllipseCallout, "Oval Callout");
			result.Add(ShapeType.CloudCallout, "Cloud Callout");
			result.Add(ShapeType.BorderCallout1, "Line Callout 1");
			result.Add(ShapeType.BorderCallout2, "Line Callout 2");
			result.Add(ShapeType.BorderCallout3, "Line Callout 3");
			result.Add(ShapeType.AccentCallout1, "Line Callout 1 (Accent Bar)");
			result.Add(ShapeType.AccentCallout2, "Line Callout 2 (Accent Bar)");
			result.Add(ShapeType.AccentCallout3, "Line Callout 3 (Accent Bar)");
			result.Add(ShapeType.Callout1, "Line Callout 1 (No Border)");
			result.Add(ShapeType.Callout2, "Line Callout 2 (No Border)");
			result.Add(ShapeType.Callout3, "Line Callout 3 (No Border)");
			result.Add(ShapeType.AccentBorderCallout1, "Line Callout 1 (Border and Accent Bar)");
			result.Add(ShapeType.AccentBorderCallout2, "Line Callout 2 (Border and Accent Bar)");
			result.Add(ShapeType.AccentBorderCallout3, "Line Callout 3 (Border and Accent Bar)");
			return result;
		}
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("twoCellAnchor", OnTwoCellAnchor);
			result.Add("absoluteAnchor", OnAbsoluteAnchor);
			result.Add("oneCellAnchor", OnOneCellAnchor);
			return result;
		}
		#endregion
		DrawingObject drawingObjectInfo;
		public DrawingDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal DrawingObject DrawingObjectInfo { get { return drawingObjectInfo; } }
		#endregion
		static DrawingDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DrawingDestination)importer.PeekDestination();
		}
		static DrawingObject CreateDrawingObject(SpreadsheetMLBaseImporter importer, AnchorType anchorType) {
			DrawingObject drawingObjectInfo = new DrawingObject(importer.CurrentSheet);
			GetThis(importer).drawingObjectInfo = drawingObjectInfo;
			drawingObjectInfo.BeginUpdate();
			drawingObjectInfo.AnchorType = anchorType;
			return drawingObjectInfo;
		}
		#region Handlers
		static Destination OnTwoCellAnchor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TwoCellAnchorDestination(importer, CreateDrawingObject(importer, AnchorType.TwoCell));
		}
		static Destination OnOneCellAnchor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new OneCellAnchorDestination(importer, CreateDrawingObject(importer, AnchorType.OneCell));
		}
		static Destination OnAbsoluteAnchor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new AbsoluteCellAnchorDestination(importer, CreateDrawingObject(importer, AnchorType.Absolute));
		}
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			Dictionary<int, DrawingObject> ids = new Dictionary<int, DrawingObject>();
			int maxId = 1;
			foreach (IDrawingObject drawing in Importer.CurrentSheet.DrawingObjects) {
				DrawingObject drawingObject = drawing.DrawingObject;
				if (drawingObject.Properties.Id > 1 && !ids.ContainsKey(drawingObject.Properties.Id)) {
					ids.Add(drawingObject.Properties.Id, drawingObject);
					maxId = Math.Max(maxId, drawingObject.Properties.Id);
				}
			}
			foreach (IDrawingObject drawing in Importer.CurrentSheet.DrawingObjects) {
				DrawingObject drawingObject = drawing.DrawingObject;
				if (!ids.ContainsKey(drawingObject.Properties.Id) || (ids[drawingObject.Properties.Id] != drawingObject)) {
					maxId++;
					drawingObject.Properties.Id = maxId;
					ids.Add(drawingObject.Properties.Id, drawingObject);
				}
			}
			foreach (IDrawingObject drawing in Importer.CurrentSheet.DrawingObjects) {
				DrawingObject drawingObject = drawing.DrawingObject;
				if (string.IsNullOrEmpty(drawingObject.Properties.Name)) {
					switch(drawing.DrawingType) {
						case DrawingObjectType.Picture:
							drawingObject.Properties.Name = string.Format("Picture {0}", drawingObject.Properties.Id - 1);
							break;
						case DrawingObjectType.Chart:
							drawingObject.Properties.Name = string.Format("Chart {0}", drawingObject.Properties.Id - 1);
							break;
						case DrawingObjectType.GroupShape:
							drawingObject.Properties.Name = string.Format("Group {0}", drawingObject.Properties.Id - 1);
							break;
						case DrawingObjectType.Shape:
						case DrawingObjectType.ConnectionShape: {
								var shapeTypeDesctiption = GetShapeTypeDesctiption(drawing as ModelShapeBase);
								drawingObject.Properties.Name = string.Format("{0} {1}", shapeTypeDesctiption, drawingObject.Properties.Id - 1);
								break;
							}
					}
				}
			}
		}
		string GetShapeTypeDesctiption(ModelShapeBase shape) {
			if (shape.ShapeProperties.ShapeType == ShapeType.Rect) {
				ModelShape modelShape = shape as ModelShape;
				if (modelShape != null && modelShape.TextBoxMode) return "TextBox";
			}
			if (!ShapeTypeDesctiptionTable.ContainsKey(shape.ShapeProperties.ShapeType)) return "Freeform";
			return ShapeTypeDesctiptionTable[shape.ShapeProperties.ShapeType];
		}
	}
	#endregion
	#region TwoCellAnchorDestination
	public class TwoCellAnchorDestination : CommonDrawingObjectsDestination {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("clientData", OnClientData);
			result.Add("from", OnFrom);
			result.Add("to", OnTo);
			result.Add("AlternateContent", OnAlternateContent);
			result.Add("Choice", OnChoice);
			result.Add("Fallback", OnFallback);
			AddCommonHandlers(result);
			return result;
		}
		#endregion
		public static Dictionary<AnchorType, string> ResizingBehaviorTable = CreateResizingBehaviorTable();
		static Dictionary<AnchorType, string> CreateResizingBehaviorTable() {
			Dictionary<AnchorType, string> result = new Dictionary<AnchorType, string>();
			result.Add(AnchorType.TwoCell, "twoCell");
			result.Add(AnchorType.OneCell, "oneCell");
			result.Add(AnchorType.Absolute, "absolute");
			return result;
		}
		public TwoCellAnchorDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer, drawingObjectInfo) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static TwoCellAnchorDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (TwoCellAnchorDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnClientData(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ClientDataDestination(importer, GetThis(importer).GetDrawingObject());
		}
		static Destination OnFrom(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new AnchorDestination(importer, false, GetThis(importer).GetDrawingObject());
		}
		static Destination OnTo(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new AnchorDestination(importer, true, GetThis(importer).GetDrawingObject());
		}
		static Destination OnAlternateContent(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer);
		}
		static Destination OnChoice(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer);
		}
		static Destination OnFallback(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			GetDrawingObject().ResizingBehavior = Importer.GetWpEnumValue(reader, "editAs", ResizingBehaviorTable, AnchorType.TwoCell);
		}
	}
	#endregion
	#region OneCellAnchorDestination
	public class OneCellAnchorDestination : CommonDrawingObjectsDestination {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("from", OnFrom);
			result.Add("ext", OnShapeExtention);
			result.Add("clientData", OnClientData);
			AddCommonHandlers(result);
			return result;
		}
		#endregion
		public OneCellAnchorDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer, drawingObjectInfo) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static OneCellAnchorDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (OneCellAnchorDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnClientData(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ClientDataDestination(importer, GetThis(importer).GetDrawingObject());
		}
		static Destination OnShapeExtention(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ShapeExtentionDestination(importer, GetThis(importer).GetDrawingObject());
		}
		static Destination OnFrom(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new AnchorDestination(importer, false, GetThis(importer).GetDrawingObject());
		}
		#endregion
	}
	#endregion
	#region AbsoluteCellAnchorDestination
	public class AbsoluteCellAnchorDestination : CommonDrawingObjectsDestination {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pos", OnPosition);
			result.Add("ext", OnShapeExtention);
			result.Add("clientData", OnClientData);
			AddCommonHandlers(result);
			return result;
		}
		#endregion
		public AbsoluteCellAnchorDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer, drawingObjectInfo) {
		}
		static AbsoluteCellAnchorDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (AbsoluteCellAnchorDestination)importer.PeekDestination();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnClientData(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ClientDataDestination(importer, GetThis(importer).GetDrawingObject());
		}
		static Destination OnShapeExtention(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ShapeExtentionDestination(importer, GetThis(importer).GetDrawingObject());
		}
		static Destination OnPosition(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ShapePositionDestination(importer, GetThis(importer).GetDrawingObject());
		}
		#endregion
	}
	#endregion
	#region CommonDrawingObjectsDestination
	public class CommonDrawingObjectsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddCommonHandlers(result);
			return result;
		}
		protected static void AddCommonHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> result) {
			result.Add("cxnSp", OnConnectionShape);
			result.Add("graphicFrame", OnGraphicFrame);
			result.Add("grpSp", OnGroupShape);
			result.Add("pic", OnPicture);
			result.Add("sp", OnShape);
		}
		#endregion
		DrawingObject DrawingObject { get; set; }
		public CommonDrawingObjectsDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer) {
			this.DrawingObject = drawingObjectInfo;
			importer.CurrentDrawingObjectsContainer = importer.CurrentSheet;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static CommonDrawingObjectsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (CommonDrawingObjectsDestination)importer.PeekDestination();
		}
		protected virtual DrawingObject GetDrawingObject() {
			return DrawingObject;
		}
		#region Handlers
		static Destination OnConnectionShape(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			if(importer.DocumentModel.DocumentCapabilities.ShapesAllowed)
				return new ConnectionShapeDestination(importer, GetThis(importer).GetDrawingObject());
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnGraphicFrame(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new GraphicFrameDestination(importer, GetThis(importer).GetDrawingObject());
		}
		static Destination OnGroupShape(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new GroupShapeDestination(importer, GetThis(importer).GetDrawingObject());
		}
		static Destination OnPicture(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			if(importer.DocumentModel.DocumentCapabilities.PicturesAllowed)
				return new PictureDestination(importer, GetThis(importer).GetDrawingObject());
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnShape(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			if(importer.DocumentModel.DocumentCapabilities.ShapesAllowed)
				return new ShapeDestination(importer, GetThis(importer).GetDrawingObject());
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		#endregion
	}
	#endregion
	#region ShapePositionDestination
	public class ShapePositionDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly DrawingObject drawingObjectInfo;
		public ShapePositionDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer) {
				this.drawingObjectInfo = drawingObjectInfo;
		}
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			int x = Importer.GetIntegerValue(reader, "x");
			int y = Importer.GetIntegerValue(reader, "y");
			if (x < 0 || y < 0)
				Importer.ThrowInvalidFile("Invalid x/y position");
			drawingObjectInfo.CoordinateX = Importer.DocumentModel.UnitConverter.EmuToModelUnitsF(x);
			drawingObjectInfo.CoordinateY = Importer.DocumentModel.UnitConverter.EmuToModelUnitsF(y);
		}
	}
	#endregion
	#region ShapeExtentionDestination
	public class ShapeExtentionDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly DrawingObject drawingObjectInfo;
		public ShapeExtentionDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer) {
				this.drawingObjectInfo = drawingObjectInfo;
		}
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			int width = Importer.GetIntegerValue(reader, "cx");
			int height = Importer.GetIntegerValue(reader, "cy");
			if (height < 0 || width < 0)
				Importer.ThrowInvalidFile(string.Format("Invalid width {0} or height {1}", width, height));
			drawingObjectInfo.AnchorData.SetExtentCore(Importer.DocumentModel.UnitConverter.EmuToModelUnitsF(width),
				Importer.DocumentModel.UnitConverter.EmuToModelUnitsF(height));
		}
	}
	#endregion
	#region ClientDataDestination
	public class ClientDataDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly DrawingObject drawingObjectInfo;
		public ClientDataDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer) {
				this.drawingObjectInfo = drawingObjectInfo;
		}
		#region Properties
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			drawingObjectInfo.LocksWithSheet = Importer.GetWpSTOnOffValue(reader, "fLocksWithSheet", true);
			drawingObjectInfo.PrintsWithSheet = Importer.GetWpSTOnOffValue(reader, "fPrintsWithSheet", true);
		}
	}
	#endregion
	#region GraphicFrameDestination
	public class GraphicFrameDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("graphic", OnGraphic);
			result.Add("nvGraphicFramePr", OnNonVisualGraphicFrameProperities);
			result.Add("xfrm", OnXfrm);
			return result;
		}
		#endregion
		readonly DrawingObject drawingObjectInfo;
		public GraphicFrameDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer) {
				this.drawingObjectInfo = drawingObjectInfo;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static GraphicFrameDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (GraphicFrameDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnGraphic(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new GraphicDestination(importer, GetThis(importer).drawingObjectInfo);
		}
		static Destination OnNonVisualGraphicFrameProperities(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualGraphicFramePropertiesDestination(importer, GetThis(importer).drawingObjectInfo);
		}
		static Destination OnXfrm(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingObject drawingObjectInfo = GetThis(importer).drawingObjectInfo;
			return new XfrmDestination(importer, drawingObjectInfo.Transform2D);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			drawingObjectInfo.GraphicFrameInfo.IsPublished = Importer.GetWpSTOnOffValue(reader, "fPublished", false);
			string macro = Importer.ReadAttribute(reader, "macro");
			if (String.IsNullOrEmpty(macro))
				macro = string.Empty;
			drawingObjectInfo.GraphicFrameInfo.Macro = macro;
		}
	}
	#endregion
	#region NonVisualGraphicFrameProperitiesDestination
	public class NonVisualGraphicFramePropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cNvPr", OnNonVisualDrawingProps);
			return result;
		}
		static NonVisualGraphicFramePropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (NonVisualGraphicFramePropertiesDestination)importer.PeekDestination();
		}
		#endregion
		readonly DrawingObject drawingObjectInfo;
		public NonVisualGraphicFramePropertiesDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer) {
				this.drawingObjectInfo = drawingObjectInfo;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnNonVisualDrawingProps(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualDrawingProps(importer, GetThis(importer).drawingObjectInfo.Properties);
		}
		#endregion
	}
	#endregion
	#region AnchorDestination
	public class AnchorReadingHelper {
		public string RowString { get; set; }
		public string ColString { get; set; }
		public float ColOffset { get; set; }
		public float RowOffset { get; set; }
	}
	public class AnchorDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("col", OnColumn);
			result.Add("colOff", OnColumnOffset);
			result.Add("row", OnRow);
			result.Add("rowOff", OnRowOffset);
			return result;
		}
		#endregion
		static readonly AnchorReadingHelper readingHelper = new AnchorReadingHelper();
		readonly bool endingPoint;
		readonly DrawingObject drawingObjectInfo;
		public AnchorDestination(SpreadsheetMLBaseImporter importer, bool endingPoint, DrawingObject drawingObjectInfo)
			: base(importer) {
			this.drawingObjectInfo = drawingObjectInfo;
			this.endingPoint = endingPoint;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnColumn(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new StringValueTagDestination(importer, delegate(string value) {
				readingHelper.ColString = value;
				return true;
			});
		}
		static Destination OnColumnOffset(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingColumnOffsetDestination(importer, readingHelper);
		}
		static Destination OnRow(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new StringValueTagDestination(importer, delegate(string value) {
				readingHelper.RowString = value;
				return true;
			});
		}
		static Destination OnRowOffset(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingRowOffsetDestination(importer, readingHelper);
		}
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			int row, col;
			Guard.ArgumentIsNotNullOrEmpty(readingHelper.RowString, "DrawingRowValue");
			if(!int.TryParse(readingHelper.RowString, out row))
				Importer.ThrowInvalidFile("Invalid readingHeader.RowString");
			if(!int.TryParse(readingHelper.ColString, out col))
				Importer.ThrowInvalidFile("Invalid readingHeader.ColString");
			if ((col < 0) || (row < 0)) Importer.ThrowInvalidFile("INvalid col/row");
			AnchorPoint point = endingPoint ? drawingObjectInfo.To : drawingObjectInfo.From;
			float colOffset = readingHelper.ColOffset;
			float rowOffset = readingHelper.RowOffset;
			point = new AnchorPoint(drawingObjectInfo.Worksheet.SheetId, col, row, colOffset, rowOffset);
			if (endingPoint)
				drawingObjectInfo.To = point;
			else
				drawingObjectInfo.From = point;
		}
	}
	#endregion
	#region GraphicDestination
	public class GraphicDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("graphicData", OnGraphicData);
			return result;
		}
		#endregion
		readonly DrawingObject drawingObjectInfo;
		public GraphicDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer) {
			this.drawingObjectInfo = drawingObjectInfo;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static GraphicDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (GraphicDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnGraphicData(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new GraphicDataDestination(importer, GetThis(importer).drawingObjectInfo);
		}
		#endregion
	}
	#endregion
	#region GraphicDataDestination
	public class GraphicDataDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("chart", OnChart);
			return result;
		}
		#endregion
		readonly DrawingObject drawingObjectInfo;
		public GraphicDataDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer) {
			this.drawingObjectInfo = drawingObjectInfo;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static GraphicDataDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (GraphicDataDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartRefDestination(importer, GetThis(importer).drawingObjectInfo);
		}
		#endregion
	}
	#endregion
	#region ChartRefDestination
	public class ChartRefDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly DrawingObject drawingObjectInfo;
		public ChartRefDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer) {
			this.drawingObjectInfo = drawingObjectInfo;
		}
		#region Properties
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			if (!Importer.DocumentModel.DocumentCapabilities.ChartsAllowed)
				return;
			string relationId = reader.GetAttribute("id", Importer.RelationsNamespace);
			if (String.IsNullOrEmpty(relationId))
				Importer.ThrowInvalidFile("relationId is null or empty");
			string relationPath = Importer.LookupRelationTargetById(Importer.DocumentRelations, relationId, Importer.DocumentRootFolder, string.Empty);
			Chart chart = new Chart(drawingObjectInfo);
			Importer.CurrentDrawingObjectsContainer.DrawingObjects.Add(chart);
			Importer.ChartRelations.Add(relationPath, chart);
		}
	}
	#endregion
	#region DrawingColumnOffsetDestination
	public class DrawingColumnOffsetDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		AnchorReadingHelper readingHelper;
		public DrawingColumnOffsetDestination(SpreadsheetMLBaseImporter importer, AnchorReadingHelper readingHelper)
			: base(importer) {
			this.readingHelper = readingHelper;
		}
		public override bool ProcessText(XmlReader reader) {
			string result = reader.Value;
			Guard.ArgumentIsNotNullOrEmpty(result, "DrawingColOffsetValue");
			float colOffset = Importer.DocumentModel.UnitConverter.EmuToModelUnitsF(Convert.ToInt32(result));
			readingHelper.ColOffset = colOffset;
			return true;
		}
	}
	#endregion
	#region DrawingRowOffsetDestination
	public class DrawingRowOffsetDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly AnchorReadingHelper readingHelper;
		public DrawingRowOffsetDestination(SpreadsheetMLBaseImporter importer, AnchorReadingHelper readingHelper)
			: base(importer) {
			this.readingHelper = readingHelper;
		}
		public override bool ProcessText(XmlReader reader) {
			string result = reader.Value;
			Guard.ArgumentIsNotNullOrEmpty(result, "DrawingRowOffsetValue");
			float rowOffset = Importer.DocumentModel.UnitConverter.EmuToModelUnitsF(Convert.ToInt32(result));
			readingHelper.RowOffset = rowOffset;
			return true;
		}
	}
	#endregion
	#region XfrmDestination
	public class XfrmDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddCommonHandlers(result);
			return result;
		}
		protected static void AddCommonHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> result) {
			result.Add("ext", OnExtents);
			result.Add("off", OnOffset);
		}
		#endregion
		readonly Transform2D xfrm;
		public XfrmDestination(SpreadsheetMLBaseImporter importer, Transform2D xfrm)
			: base(importer) {
			this.xfrm = xfrm;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static XfrmDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (XfrmDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnExtents(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Transform2D xfrm = GetThis(importer).xfrm;
			return new ExtentsDestination(importer, xfrm);
		}
		static Destination OnOffset(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Transform2D xfrm = GetThis(importer).xfrm;
			return new OffsetDestination(importer, xfrm);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			xfrm.Rotation = Importer.DocumentModel.UnitConverter.AdjAngleToModelUnits(Importer.GetWpSTIntegerValue(reader, "rot", 0));
			xfrm.FlipH = Importer.GetWpSTOnOffValue(reader, "flipH", false);
			xfrm.FlipV = Importer.GetWpSTOnOffValue(reader, "flipV", false);
		}
	}
	#endregion
	#region PresetGeometryDestination
	public class PresetGeometryDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		public static Dictionary<ShapeType, string> shapeTypeTable = ShapeTypeTable();
		static Dictionary<ShapeType, string> ShapeTypeTable() {
			Dictionary<ShapeType, string> result = new Dictionary<ShapeType, string>();
			result.Add(ShapeType.Line, "line");
			result.Add(ShapeType.LineInv, "lineInv");
			result.Add(ShapeType.Triangle, "triangle");
			result.Add(ShapeType.RtTriangle, "rtTriangle");
			result.Add(ShapeType.Rect, "rect");
			result.Add(ShapeType.Diamond, "diamond");
			result.Add(ShapeType.Parallelogram, "parallelogram");
			result.Add(ShapeType.Trapezoid, "trapezoid");
			result.Add(ShapeType.NonIsoscelesTrapezoid, "nonIsoscelesTrapezoid");
			result.Add(ShapeType.Pentagon, "pentagon");
			result.Add(ShapeType.Hexagon, "hexagon");
			result.Add(ShapeType.Heptagon, "heptagon");
			result.Add(ShapeType.Octagon, "octagon");
			result.Add(ShapeType.Decagon, "decagon");
			result.Add(ShapeType.Dodecagon, "dodecagon");
			result.Add(ShapeType.Star4, "star4");
			result.Add(ShapeType.Star5, "star5");
			result.Add(ShapeType.Star6, "star6");
			result.Add(ShapeType.Star7, "star7");
			result.Add(ShapeType.Star8, "star8");
			result.Add(ShapeType.Star10, "star10");
			result.Add(ShapeType.Star12, "star12");
			result.Add(ShapeType.Star16, "star16");
			result.Add(ShapeType.Star24, "star24");
			result.Add(ShapeType.Star32, "star32");
			result.Add(ShapeType.RoundRect, "roundRect");
			result.Add(ShapeType.Round1Rect, "round1Rect");
			result.Add(ShapeType.Round2SameRect, "round2SameRect");
			result.Add(ShapeType.Round2DiagRect, "round2DiagRect");
			result.Add(ShapeType.SnipRoundRect, "snipRoundRect");
			result.Add(ShapeType.Snip1Rect, "snip1Rect");
			result.Add(ShapeType.Snip2SameRect, "snip2SameRect");
			result.Add(ShapeType.Snip2DiagRect, "snip2DiagRect");
			result.Add(ShapeType.Plaque, "plaque");
			result.Add(ShapeType.Ellipse, "ellipse");
			result.Add(ShapeType.Teardrop, "teardrop");
			result.Add(ShapeType.HomePlate, "homePlate");
			result.Add(ShapeType.Chevron, "chevron");
			result.Add(ShapeType.PieWedge, "pieWedge");
			result.Add(ShapeType.Pie, "pie");
			result.Add(ShapeType.BlockArc, "blockArc");
			result.Add(ShapeType.Donut, "donut");
			result.Add(ShapeType.NoSmoking, "noSmoking");
			result.Add(ShapeType.RightArrow, "rightArrow");
			result.Add(ShapeType.LeftArrow, "leftArrow");
			result.Add(ShapeType.UpArrow, "upArrow");
			result.Add(ShapeType.DownArrow, "downArrow");
			result.Add(ShapeType.StripedRightArrow, "stripedRightArrow");
			result.Add(ShapeType.NotchedRightArrow, "notchedRightArrow");
			result.Add(ShapeType.BentUpArrow, "bentUpArrow");
			result.Add(ShapeType.LeftRightArrow, "leftRightArrow");
			result.Add(ShapeType.UpDownArrow, "upDownArrow");
			result.Add(ShapeType.LeftUpArrow, "leftUpArrow");
			result.Add(ShapeType.LeftRightUpArrow, "leftRightUpArrow");
			result.Add(ShapeType.QuadArrow, "quadArrow");
			result.Add(ShapeType.LeftArrowCallout, "leftArrowCallout");
			result.Add(ShapeType.RightArrowCallout, "rightArrowCallout");
			result.Add(ShapeType.UpArrowCallout, "upArrowCallout");
			result.Add(ShapeType.DownArrowCallout, "downArrowCallout");
			result.Add(ShapeType.LeftRightArrowCallout, "leftRightArrowCallout");
			result.Add(ShapeType.UpDownArrowCallout, "upDownArrowCallout");
			result.Add(ShapeType.QuadArrowCallout, "quadArrowCallout");
			result.Add(ShapeType.BentArrow, "bentArrow");
			result.Add(ShapeType.UturnArrow, "uturnArrow");
			result.Add(ShapeType.CircularArrow, "circularArrow");
			result.Add(ShapeType.LeftCircularArrow, "leftCircularArrow");
			result.Add(ShapeType.LeftRightCircularArrow, "leftRightCircularArrow");
			result.Add(ShapeType.CurvedRightArrow, "curvedRightArrow");
			result.Add(ShapeType.CurvedLeftArrow, "curvedLeftArrow");
			result.Add(ShapeType.CurvedUpArrow, "curvedUpArrow");
			result.Add(ShapeType.CurvedDownArrow, "curvedDownArrow");
			result.Add(ShapeType.SwooshArrow, "swooshArrow");
			result.Add(ShapeType.Cube, "cube");
			result.Add(ShapeType.Can, "can");
			result.Add(ShapeType.LightningBolt, "lightningBolt");
			result.Add(ShapeType.Heart, "heart");
			result.Add(ShapeType.Sun, "sun");
			result.Add(ShapeType.Moon, "moon");
			result.Add(ShapeType.SmileyFace, "smileyFace");
			result.Add(ShapeType.IrregularSeal1, "irregularSeal1");
			result.Add(ShapeType.IrregularSeal2, "irregularSeal2");
			result.Add(ShapeType.FoldedCorner, "foldedCorner");
			result.Add(ShapeType.Bevel, "bevel");
			result.Add(ShapeType.Frame, "frame");
			result.Add(ShapeType.HalfFrame, "halfFrame");
			result.Add(ShapeType.Corner, "corner");
			result.Add(ShapeType.DiagStripe, "diagStripe");
			result.Add(ShapeType.Chord, "chord");
			result.Add(ShapeType.Arc, "arc");
			result.Add(ShapeType.LeftBracket, "leftBracket");
			result.Add(ShapeType.RightBracket, "rightBracket");
			result.Add(ShapeType.LeftBrace, "leftBrace");
			result.Add(ShapeType.RightBrace, "rightBrace");
			result.Add(ShapeType.BracketPair, "bracketPair");
			result.Add(ShapeType.BracePair, "bracePair");
			result.Add(ShapeType.StraightConnector1, "straightConnector1");
			result.Add(ShapeType.BentConnector2, "bentConnector2");
			result.Add(ShapeType.BentConnector3, "bentConnector3");
			result.Add(ShapeType.BentConnector4, "bentConnector4");
			result.Add(ShapeType.BentConnector5, "bentConnector5");
			result.Add(ShapeType.CurvedConnector2, "curvedConnector2");
			result.Add(ShapeType.CurvedConnector3, "curvedConnector3");
			result.Add(ShapeType.CurvedConnector4, "curvedConnector4");
			result.Add(ShapeType.CurvedConnector5, "curvedConnector5");
			result.Add(ShapeType.Callout1, "callout1");
			result.Add(ShapeType.Callout2, "callout2");
			result.Add(ShapeType.Callout3, "callout3");
			result.Add(ShapeType.AccentCallout1, "accentCallout1");
			result.Add(ShapeType.AccentCallout2, "accentCallout2");
			result.Add(ShapeType.AccentCallout3, "accentCallout3");
			result.Add(ShapeType.BorderCallout1, "borderCallout1");
			result.Add(ShapeType.BorderCallout2, "borderCallout2");
			result.Add(ShapeType.BorderCallout3, "borderCallout3");
			result.Add(ShapeType.AccentBorderCallout1, "accentBorderCallout1");
			result.Add(ShapeType.AccentBorderCallout2, "accentBorderCallout2");
			result.Add(ShapeType.AccentBorderCallout3, "accentBorderCallout3");
			result.Add(ShapeType.WedgeRectCallout, "wedgeRectCallout");
			result.Add(ShapeType.WedgeRoundRectCallout, "wedgeRoundRectCallout");
			result.Add(ShapeType.WedgeEllipseCallout, "wedgeEllipseCallout");
			result.Add(ShapeType.CloudCallout, "cloudCallout");
			result.Add(ShapeType.Cloud, "cloud");
			result.Add(ShapeType.Ribbon, "ribbon");
			result.Add(ShapeType.Ribbon2, "ribbon2");
			result.Add(ShapeType.EllipseRibbon, "ellipseRibbon");
			result.Add(ShapeType.EllipseRibbon2, "ellipseRibbon2");
			result.Add(ShapeType.LeftRightRibbon, "leftRightRibbon");
			result.Add(ShapeType.VerticalScroll, "verticalScroll");
			result.Add(ShapeType.HorizontalScroll, "horizontalScroll");
			result.Add(ShapeType.Wave, "wave");
			result.Add(ShapeType.DoubleWave, "doubleWave");
			result.Add(ShapeType.Plus, "plus");
			result.Add(ShapeType.FlowChartProcess, "flowChartProcess");
			result.Add(ShapeType.FlowChartDecision, "flowChartDecision");
			result.Add(ShapeType.FlowChartInputOutput, "flowChartInputOutput");
			result.Add(ShapeType.FlowChartPredefinedProcess, "flowChartPredefinedProcess");
			result.Add(ShapeType.FlowChartInternalStorage, "flowChartInternalStorage");
			result.Add(ShapeType.FlowChartDocument, "flowChartDocument");
			result.Add(ShapeType.FlowChartMultidocument, "flowChartMultidocument");
			result.Add(ShapeType.FlowChartTerminator, "flowChartTerminator");
			result.Add(ShapeType.FlowChartPreparation, "flowChartPreparation");
			result.Add(ShapeType.FlowChartManualInput, "flowChartManualInput");
			result.Add(ShapeType.FlowChartManualOperation, "flowChartManualOperation");
			result.Add(ShapeType.FlowChartConnector, "flowChartConnector");
			result.Add(ShapeType.FlowChartPunchedCard, "flowChartPunchedCard");
			result.Add(ShapeType.FlowChartPunchedTape, "flowChartPunchedTape");
			result.Add(ShapeType.FlowChartSummingJunction, "flowChartSummingJunction");
			result.Add(ShapeType.FlowChartOr, "flowChartOr");
			result.Add(ShapeType.FlowChartCollate, "flowChartCollate");
			result.Add(ShapeType.FlowChartSort, "flowChartSort");
			result.Add(ShapeType.FlowChartExtract, "flowChartExtract");
			result.Add(ShapeType.FlowChartMerge, "flowChartMerge");
			result.Add(ShapeType.FlowChartOfflineStorage, "flowChartOfflineStorage");
			result.Add(ShapeType.FlowChartOnlineStorage, "flowChartOnlineStorage");
			result.Add(ShapeType.FlowChartMagneticTape, "flowChartMagneticTape");
			result.Add(ShapeType.FlowChartMagneticDisk, "flowChartMagneticDisk");
			result.Add(ShapeType.FlowChartMagneticDrum, "flowChartMagneticDrum");
			result.Add(ShapeType.FlowChartDisplay, "flowChartDisplay");
			result.Add(ShapeType.FlowChartDelay, "flowChartDelay");
			result.Add(ShapeType.FlowChartAlternateProcess, "flowChartAlternateProcess");
			result.Add(ShapeType.FlowChartOffpageConnector, "flowChartOffpageConnector");
			result.Add(ShapeType.ActionButtonBlank, "actionButtonBlank");
			result.Add(ShapeType.ActionButtonHome, "actionButtonHome");
			result.Add(ShapeType.ActionButtonHelp, "actionButtonHelp");
			result.Add(ShapeType.ActionButtonInformation, "actionButtonInformation");
			result.Add(ShapeType.ActionButtonForwardNext, "actionButtonForwardNext");
			result.Add(ShapeType.ActionButtonBackPrevious, "actionButtonBackPrevious");
			result.Add(ShapeType.ActionButtonEnd, "actionButtonEnd");
			result.Add(ShapeType.ActionButtonBeginning, "actionButtonBeginning");
			result.Add(ShapeType.ActionButtonReturn, "actionButtonReturn");
			result.Add(ShapeType.ActionButtonDocument, "actionButtonDocument");
			result.Add(ShapeType.ActionButtonSound, "actionButtonSound");
			result.Add(ShapeType.ActionButtonMovie, "actionButtonMovie");
			result.Add(ShapeType.Gear6, "gear6");
			result.Add(ShapeType.Gear9, "gear9");
			result.Add(ShapeType.Funnel, "funnel");
			result.Add(ShapeType.MathPlus, "mathPlus");
			result.Add(ShapeType.MathMinus, "mathMinus");
			result.Add(ShapeType.MathMultiply, "mathMultiply");
			result.Add(ShapeType.MathDivide, "mathDivide");
			result.Add(ShapeType.MathEqual, "mathEqual");
			result.Add(ShapeType.MathNotEqual, "mathNotEqual");
			result.Add(ShapeType.CornerTabs, "cornerTabs");
			result.Add(ShapeType.SquareTabs, "squareTabs");
			result.Add(ShapeType.PlaqueTabs, "plaqueTabs");
			result.Add(ShapeType.ChartX, "chartX");
			result.Add(ShapeType.ChartStar, "chartStar");
			result.Add(ShapeType.ChartPlus, "chartPlus");
			return result;
		}
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("avLst", OnShapeAdjust);
			return result;
		}
		#endregion
		readonly ShapeProperties shapeProperties;
		public PresetGeometryDestination(SpreadsheetMLBaseImporter importer, ShapeProperties shapeProperties)
			: base(importer) {
			this.shapeProperties = shapeProperties;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static PresetGeometryDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PresetGeometryDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnShapeAdjust(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ModelShapeGuideListDestination(importer, GetThis(importer).shapeProperties.PresetAdjustList);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			shapeProperties.ShapeType = Importer.GetWpEnumValue<ShapeType>(reader, "prst", shapeTypeTable, ShapeType.Rect);
		}
	}
	#endregion
	#region ExtentsDestination
	public class ExtentsDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		Transform2D xfrm;
		public ExtentsDestination(SpreadsheetMLBaseImporter importer, Transform2D xfrm)
			: base(importer) {
			this.xfrm = xfrm;
		}
		#region Properties
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			int cx = Importer.GetWpSTIntegerValue(reader, "cx", -1);
			int cy = Importer.GetWpSTIntegerValue(reader, "cy", -1);
			if ((cx < 0) || (cy < 0))
				Importer.ThrowInvalidFile("Invalid cx/cy");
			xfrm.Cx = Importer.DocumentModel.UnitConverter.EmuToModelUnits(cx);
			xfrm.Cy = Importer.DocumentModel.UnitConverter.EmuToModelUnits(cy);
		}
	}
	#endregion
	#region OffsetDestination
	public class OffsetDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		Transform2D xfrm;
		public OffsetDestination(SpreadsheetMLBaseImporter importer, Transform2D xfrm)
			: base(importer) {
			this.xfrm = xfrm;
		}
		#region Properties
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string xs = Importer.ReadAttribute(reader, "x");
			string ys = Importer.ReadAttribute(reader, "y");
			long offsetX, offsetY;
			if (!long.TryParse(xs, out offsetX))
				Importer.ThrowInvalidFile("Invalid x offset");
			if(!long.TryParse(ys, out offsetY))
				Importer.ThrowInvalidFile("Invalid y offset");
			xfrm.OffsetX = Importer.DocumentModel.UnitConverter.EmuToModelUnits((int) offsetX);
			xfrm.OffsetY = Importer.DocumentModel.UnitConverter.EmuToModelUnits((int) offsetY);
		}
	}
	#endregion
	#region StyleReferenceDestination
	public class StyleReferenceDestination : DrawingColorDestination {
		readonly Action1<string> setReferenceDelegate;
		public StyleReferenceDestination(SpreadsheetMLBaseImporter importer, Action1<string> setReferenceDelegate, DrawingColor color)
			: base(importer, color) {
			this.setReferenceDelegate = setReferenceDelegate;
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			string idxValue = Importer.ReadAttribute(reader, "idx");
			setReferenceDelegate(idxValue);
		}
		#endregion
	}
	#endregion
}
