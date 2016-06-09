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
using DevExpress.Export.Xl;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ShapeDestination
	public class ShapeDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("nvSpPr", OnNonVisualShapeProperties);
			result.Add("spPr", OnShapeProperties);
			result.Add("style", OnShapeStyle);
			result.Add("txBody", OnShapeTextBody);
			return result;
		}
		#endregion
		readonly DrawingObject drawingObject;
		readonly ModelShape shape;
		public ShapeDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObject)
			: base(importer) {
			this.drawingObject = drawingObject;
			this.drawingObject.EndUpdate();
			shape = new ModelShape(this.drawingObject);
			Shape.BeginUpdate();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public ModelShape Shape { get { return shape; } }
		#endregion
		#region Handlers
		static Destination OnNonVisualShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualShapePropertiesDestination(importer, GetThis(importer).Shape);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ShapePropertiesDestination(importer, GetThis(importer).Shape.ShapeProperties);
		}
		static Destination OnShapeStyle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ShapeStyleDestination(importer, GetThis(importer).Shape.ShapeStyle);
		}
		static Destination OnShapeTextBody(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TextPropertiesDestination(importer, GetThis(importer).Shape.TextProperties);
		}
		#endregion
		static ShapeDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ShapeDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Shape.IsPublished = Importer.GetWpSTOnOffValue(reader, "fPublished", false);
			string macro = Importer.ReadAttribute(reader, "macro");
			if(String.IsNullOrEmpty(macro))
				macro = "";
			Shape.Macro = macro;
		}
		public override void ProcessElementClose(XmlReader reader) {
			Shape.EndUpdate();
			Importer.CurrentDrawingObjectsContainer.DrawingObjects.Add(Shape);
			drawingObject.BeginUpdate();
			Shape.CheckRotationAndSwapBox();
		}
	}
	#endregion
	#region NonVisualShapePropertiesDestination
	public class NonVisualShapePropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cNvPr", OnNonVisualDrawingProperties);
			result.Add("cNvSpPr", OnConnectionNonVisualShapeProperties);
			return result;
		}
		#endregion
		readonly ModelShape shape;
		public NonVisualShapePropertiesDestination(SpreadsheetMLBaseImporter importer, ModelShape shape)
			: base(importer) {
			this.shape = shape;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static NonVisualShapePropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (NonVisualShapePropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnConnectionNonVisualShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ConnectionNonVisualShapePropertiesDestination(importer, GetThis(importer).shape);
		}
		static Destination OnNonVisualDrawingProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualDrawingProps(importer, GetThis(importer).shape.DrawingObject.Properties);
		}
		#endregion
	}
	#endregion
	#region ShapePropertiesDestination
	public class ShapePropertiesDestination : ShapePropertiesDestinationBase {
		public static Dictionary<OpenXmlBlackWhiteMode, string> BlackWhiteModeTable = CreateBlackWhiteModeTable();
		static Dictionary<OpenXmlBlackWhiteMode, string> CreateBlackWhiteModeTable() {
			Dictionary<OpenXmlBlackWhiteMode, string> result = new Dictionary<OpenXmlBlackWhiteMode, string>();
			result.Add(OpenXmlBlackWhiteMode.Auto, "auto");
			result.Add(OpenXmlBlackWhiteMode.Black, "black");
			result.Add(OpenXmlBlackWhiteMode.BlackGray, "blackGray");
			result.Add(OpenXmlBlackWhiteMode.BlackWhite, "blackWhite");
			result.Add(OpenXmlBlackWhiteMode.Clr, "clr");
			result.Add(OpenXmlBlackWhiteMode.Gray, "gray");
			result.Add(OpenXmlBlackWhiteMode.GrayWhite, "grayWhite");
			result.Add(OpenXmlBlackWhiteMode.Hidden, "hidden");
			result.Add(OpenXmlBlackWhiteMode.InvGray, "invGray");
			result.Add(OpenXmlBlackWhiteMode.LtGray, "ltGray");
			result.Add(OpenXmlBlackWhiteMode.White, "white");
			return result;
		}
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("xfrm", OnXfrm);
			result.Add("prstGeom", OnPresetGeometry);
			result.Add("custGeom", OnCustomGeometry);
			AddCommonHandlers(result);
			return result;
		}
		#endregion
		static ShapePropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ShapePropertiesDestination)importer.PeekDestination();
		}
		static Destination OnXfrm(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			SpreadsheetMLBaseImporter spreadsheetMlBaseImporter = (SpreadsheetMLBaseImporter)importer;
			return new XfrmDestination(spreadsheetMlBaseImporter, GetThis(spreadsheetMlBaseImporter).ShapeProperties.Transform2D);
		}
		static Destination OnPresetGeometry(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			SpreadsheetMLBaseImporter spreadsheetMlBaseImporter = (SpreadsheetMLBaseImporter)importer;
			return new PresetGeometryDestination(spreadsheetMlBaseImporter, GetThis(spreadsheetMlBaseImporter).ShapeProperties);
		}
		static Destination OnCustomGeometry(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			SpreadsheetMLBaseImporter spreadsheetMlBaseImporter = (SpreadsheetMLBaseImporter)importer;
			ShapeProperties shapeProperties = GetThis(spreadsheetMlBaseImporter).ShapeProperties;
			shapeProperties.ShapeType = ShapeType.None;
			return new CustomGeometryDestination(spreadsheetMlBaseImporter, shapeProperties.CustomGeometry);
		}
		public ShapePropertiesDestination(SpreadsheetMLBaseImporter importer, ShapeProperties shapeProperties)
			: base(importer, shapeProperties) {
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			ShapeProperties.BlackAndWhiteMode = Importer.GetWpEnumValue(reader, "bwMode", BlackWhiteModeTable, OpenXmlBlackWhiteMode.Empty);
		}
	}
	#endregion
	#region ShapeStyleDestination
	public class ShapeStyleDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly Dictionary<string, XlFontSchemeStyles> fontCollectionIndexTable = GetFontCollectionIndexTable();
		static Dictionary<string, XlFontSchemeStyles> GetFontCollectionIndexTable() {
			Dictionary<string, XlFontSchemeStyles> result = new Dictionary<string, XlFontSchemeStyles>();
			result.Add("major", XlFontSchemeStyles.Major);
			result.Add("minor", XlFontSchemeStyles.Minor);
			result.Add("none", XlFontSchemeStyles.None);
			return result;
		}
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("lnRef", OnLineReference);
			result.Add("fillRef", OnFillReference);
			result.Add("effectRef", OnEffectReference);
			result.Add("fontRef", OnFontReference);
			return result;
		}
		#endregion
		readonly ShapeStyle shapeStyle;
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public ShapeStyleDestination(SpreadsheetMLBaseImporter importer, ShapeStyle shapeStyle)
			: base(importer) {
			this.shapeStyle = shapeStyle;
		}
		#region Handlers
		static Destination OnLineReference(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ShapeStyle style = GetThis(importer).shapeStyle;
			return new StyleReferenceDestination(importer, value => {
				int idx = GetIntegerIdx(value);
				style.LineReferenceIdx = idx;
			}, style.LineColor);
		}
		static Destination OnFillReference(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ShapeStyle style = GetThis(importer).shapeStyle;
			return new StyleReferenceDestination(importer, value => {
				int idx = GetIntegerIdx(value);
				style.FillReferenceIdx = idx;
			}, style.FillColor);
		}
		static Destination OnEffectReference(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ShapeStyle style = GetThis(importer).shapeStyle;
			return new StyleReferenceDestination(importer, value => {
				int idx = GetIntegerIdx(value);
				style.EffectReferenceIdx = idx;
			}, style.EffectColor);
		}
		static Destination OnFontReference(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ShapeStyle style = GetThis(importer).shapeStyle;
			return new StyleReferenceDestination(importer, value => {
				XlFontSchemeStyles idx;
				if(!fontCollectionIndexTable.TryGetValue(value, out idx))
					idx = XlFontSchemeStyles.None;
				style.FontReferenceIdx = idx;
			}, style.FontColor);
		}
		#endregion
		static ShapeStyleDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ShapeStyleDestination)importer.PeekDestination();
		}
		static int GetIntegerIdx(string value) {
			int idx;
			if(!Int32.TryParse(value, out idx))
				idx = 1;
			return idx;
		}
	}
	#endregion
	#region ConnectionNonVisualShapePropertiesDestination
	public class ConnectionNonVisualShapePropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("spLocks", OnShapeLocks);
			return result;
		}
		#endregion
		readonly ModelShape shape;
		public ConnectionNonVisualShapePropertiesDestination(SpreadsheetMLBaseImporter importer, ModelShape shape)
			: base(importer) {
			this.shape = shape;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static ConnectionNonVisualShapePropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConnectionNonVisualShapePropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnShapeLocks(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ShapeLocksDestination(importer, GetThis(importer).shape);
		}
		#endregion
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			shape.TextBoxMode = Importer.GetWpSTOnOffValue(reader, "txBox", false);
		}
		#endregion
	}
	#endregion
	#region ShapeLocksDestination
	public class ShapeLocksDestination : DrawingLockingDestination {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			return result;
		}
		#endregion
		readonly ModelShape shape;
		public ShapeLocksDestination(SpreadsheetMLBaseImporter importer, ModelShape shape)
			: base(importer, shape.Locks) {
			this.shape = shape;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			shape.Locks.NoTextEdit = Importer.GetWpSTOnOffValue(reader, "noTextEdit", false);
		}
		#endregion
	}
	#endregion
	#region ConnectionShapeDestination
	public class ConnectionShapeDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("nvCxnSpPr", OnNonVisualPropertiesForConnectionShape);
			result.Add("spPr", OnShapeProperties);
			result.Add("style", OnShapeStyle);
			return result;
		}
		static ConnectionShapeDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConnectionShapeDestination)importer.PeekDestination();
		}
		#endregion
		readonly DrawingObject drawingObject;
		readonly ConnectionShape connectionShape;
		public ConnectionShapeDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObject)
			: base(importer) {
			this.drawingObject = drawingObject;
			this.drawingObject.EndUpdate();
			connectionShape = new ConnectionShape(this.drawingObject);
			connectionShape.BeginUpdate();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnNonVisualPropertiesForConnectionShape(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualConnectionShapePropertiesDestination(importer, GetThis(importer).connectionShape);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ShapePropertiesDestination(importer, GetThis(importer).connectionShape.ShapeProperties);
		}
		static Destination OnShapeStyle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ShapeStyleDestination(importer, GetThis(importer).connectionShape.ShapeStyle);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			connectionShape.IsPublished = Importer.GetWpSTOnOffValue(reader, "fPublished", false);
			string macro = Importer.ReadAttribute(reader, "macro");
			if(String.IsNullOrEmpty(macro))
				macro = "";
			connectionShape.Macro = macro;
		}
		public override void ProcessElementClose(XmlReader reader) {
			connectionShape.EndUpdate();
			Importer.CurrentDrawingObjectsContainer.DrawingObjects.Add(connectionShape);
			drawingObject.BeginUpdate();
			connectionShape.CheckRotationAndSwapBox();
		}
	}
	#endregion
	#region NonVisualConnectionShapePropertiesDestination
	public class NonVisualConnectionShapePropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cNvPr", OnNonVisualDrawingProperties);
			result.Add("cNvCxnSpPr", OnNonVisualConnectorShapeProperties);
			return result;
		}
		#endregion
		readonly ConnectionShape connectionShape;
		public NonVisualConnectionShapePropertiesDestination(SpreadsheetMLBaseImporter importer, ConnectionShape connectionShape)
			: base(importer) {
			this.connectionShape = connectionShape;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static NonVisualConnectionShapePropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (NonVisualConnectionShapePropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnNonVisualConnectorShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualConnectorShapeDrawingPropertiesDestination(importer, GetThis(importer).connectionShape);
		}
		static Destination OnNonVisualDrawingProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualDrawingProps(importer, GetThis(importer).connectionShape.DrawingObject.Properties);
		}
		#endregion
	}
	#endregion
	#region NonVisualConnectorShapeDrawingPropertiesDestination
	public class NonVisualConnectorShapeDrawingPropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cxnSpLocks", OnConnectionShapeLocks);
			result.Add("endCxn", OnConnectionEnd);
			result.Add("stCxn", OnConnectionStart);
			return result;
		}
		#endregion
		readonly ConnectionShape connectionShape;
		public NonVisualConnectorShapeDrawingPropertiesDestination(SpreadsheetMLBaseImporter importer, ConnectionShape connectionShape)
			: base(importer) {
			this.connectionShape = connectionShape;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static NonVisualConnectorShapeDrawingPropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (NonVisualConnectorShapeDrawingPropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnConnectionShapeLocks(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingLockingDestination(importer, GetThis(importer).connectionShape.Locks);
		}
		static Destination OnConnectionEnd(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ConnectionShapeConnectionDestination(importer, GetThis(importer).connectionShape, false);
		}
		static Destination OnConnectionStart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ConnectionShapeConnectionDestination(importer, GetThis(importer).connectionShape, true);
		}
		#endregion
	}
	#endregion
	#region ConnectionShapeConnectionDestination
	public class ConnectionShapeConnectionDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly ConnectionShape connectionShape;
		bool isStart;
		public ConnectionShapeConnectionDestination(SpreadsheetMLBaseImporter importer, ConnectionShape connectionShape, bool isStart)
			: base(importer) {
			this.connectionShape = connectionShape;
			this.isStart = isStart;
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			string idString = reader.GetAttribute("id");
			string idxString = reader.GetAttribute("idx");
			int id;
			int idx;
			if(String.IsNullOrEmpty(idString) || !int.TryParse(idString, out id)) {
				Importer.ThrowInvalidFile("Invalid DrawingElementId");
				return;
			}
			if(String.IsNullOrEmpty(idxString) || !int.TryParse(idxString, out idx)) {
				Importer.ThrowInvalidFile("Invalid idx");
				return;
			}
			if(isStart) {
				connectionShape.StartConnectionId = id;
				connectionShape.StartConnectionIdx = idx;
			} else {
				connectionShape.EndConnectionId = id;
				connectionShape.EndConnectionIdx = idx;
			}
		}
		#endregion
	}
	#endregion
}
