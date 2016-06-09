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
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using System.IO;
using System.Xml;
using DevExpress.Utils;
using System.Globalization;
using DevExpress.Utils.Zip;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region ExportWorkSheetDrawing
		#region Fields
		int drawingCounter;
		#endregion
		#region Translation Tables
		#endregion
		protected internal virtual void GenerateDrawingContent() {
			if (!ShouldExportDrawing(ActiveSheetCore))
				return;
			WriteShStartElement("drawing");
			try {
				string id = PopulateDrawingsTable(ActiveSheet);
				WriteStringAttr(RelsPrefix, "id", null, id);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportDrawing(IDrawingObjectsContainer drawingObjectsContainer) {
			int pictureCount = 0;
			int chartCount = 0;
			int shapeCount = 0;
			int groupShapeCount = 0;
			foreach(IDrawingObject drawingObject in drawingObjectsContainer.DrawingObjects) {
				if(drawingObject is Picture)
					pictureCount++;
				else if(drawingObject is Chart)
					chartCount++;
				else if(drawingObject is ModelShape)
					shapeCount++;
				else {
					GroupShape groupShape = drawingObject as GroupShape;
					if(groupShape == null)
						continue;
					if(ShouldExportDrawing(groupShape))
						groupShapeCount++;
				}
			}
			return (pictureCount > 0 && ShouldExportPictures) || (chartCount > 0 && ShouldExportCharts) || (shapeCount > 0 && ShouldExportShapes) || groupShapeCount > 0;
		}
		protected internal virtual string PopulateDrawingsTable(InternalSheetBase sheet) {
			this.drawingCounter++;
			string fileName = String.Format("drawing{0}.xml", this.drawingCounter);
			string relationTarget = "../drawings/" + fileName;
			OpenXmlRelationCollection relations = SheetRelationsTable[ActiveSheet.Name];
			string id = GenerateIdByCollection(relations);
			OpenXmlRelation relation = new OpenXmlRelation(id, relationTarget, RelsDrawingNamepace);
			relations.Add(relation);
			DrawingPathsTable.Add(sheet, fileName);
			Builder.OverriddenContentTypes.Add("/xl/drawings/" + fileName, "application/vnd.openxmlformats-officedocument.drawing+xml");
			return id;
		}
		protected internal virtual void AddDrawingPackageContent() {
			foreach (KeyValuePair<InternalSheetBase, string> pair in DrawingPathsTable) {
				DrawingRelationPathTable.Add(pair.Value, "xl\\drawings\\_rels\\" + pair.Value + ".rels");
				currentRelations = new OpenXmlRelationCollection();
				DrawingRelationsTable.Add(pair.Value, currentRelations);
				AddPackageContent(@"xl\drawings\" + pair.Value, ExportDrawing(pair.Key));
			}
		}
		protected internal virtual CompressedStream ExportDrawing(InternalSheetBase worksheet) {
			activeSheet = (Worksheet)worksheet;
			return CreateXmlContent(GenerateDrawingXmlContent);
		}
		protected internal virtual void GenerateDrawingXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			exportedImageTable.Clear();
			exportedExternalImageTable.Clear();
			exportedPictureHyperlinkTable.Clear();
			ExportDrawings(ActiveSheet);
		}
		#region Export drawing relations
		protected internal virtual void AddDrawingRelationsPackageContent() {
			foreach (KeyValuePair<string, string> valuePair in DrawingRelationPathTable) {
				OpenXmlRelationCollection currentRelations = DrawingRelationsTable[valuePair.Key];
					AddRelationsPackageContent(valuePair.Value, currentRelations);
			}
		}
		#endregion
		#region ExportCommonDrawing
		protected internal virtual void ExportDrawings(InternalSheetBase sheet) {
			WriteStartElement("xdr", "wsDr", SpreadsheetDrawingNamespace);
			try {
				GenerateDrawingNamespaceAttributes();
				DrawingObjectExportWalker walker = new DrawingObjectExportWalker(sheet, this);
				walker.Walk();
			}
			finally {
				WriteEndElement();
			}
		}
		public void GenerateDrawingNamespaceAttributes() {
			WriteStringAttr("xmlns", "xdr", null, SpreadsheetDrawingNamespace);
			WriteStringAttr("xmlns", "a", null, DrawingMLNamespace);
		}
		public void ExportClientData(IDrawingObject currentDrawingObjectInfo) {
			WriteStartElement("clientData", SpreadsheetDrawingNamespace);
			try {
				bool locksWithSheet = currentDrawingObjectInfo.LocksWithSheet;
				bool printsWithSheet = currentDrawingObjectInfo.PrintsWithSheet;
				if (!locksWithSheet)
					WriteBoolValue("fLocksWithSheet", locksWithSheet);
				if (!printsWithSheet)
					WriteBoolValue("fPrintsWithSheet", printsWithSheet);
			}
			finally {
				WriteEndElement();
			}
		}
		public void ExportShapePosition(IDrawingObject drawingObj) {
			WriteStartElement("pos", SpreadsheetDrawingNamespace);
			try {
				WriteIntValue("x", Workbook.UnitConverter.ModelUnitsToEmu((int)drawingObj.CoordinateX));
				WriteIntValue("y", Workbook.UnitConverter.ModelUnitsToEmu((int)drawingObj.CoordinateY));
			}
			finally {
				WriteEndElement();
			}
		}
		public void ExportShapeExtent(IDrawingObject drawing) {
			int cx = Workbook.UnitConverter.ModelUnitsToEmu((int)drawing.Width);
			int cy = Workbook.UnitConverter.ModelUnitsToEmu((int)drawing.Height);
			if (cy < 0)
				throw new Exception(string.Format("Invalid cy={0}, drawing.Height={1}", cy, drawing.Height));
			WriteStartElement("ext", SpreadsheetDrawingNamespace);
			try {
				WriteStringValue("cx", cx.ToString());
				WriteStringValue("cy", cy.ToString());
			}
			finally {
				WriteEndElement();
			}
		}
		public void ExportStartingAnchorPoint(AnchorPoint anchorPoint) {
			WriteStartElement("from", SpreadsheetDrawingNamespace);
			try {
				ExportAnchorPoint(anchorPoint);
			}
			finally {
				WriteEndElement();
			}
		}
		public void ExportEndingAnchorPoint(AnchorPoint anchorPoint) {
			WriteStartElement("to", SpreadsheetDrawingNamespace);
			try {
				ExportAnchorPoint(anchorPoint);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteAnchorTag(string tag, string ns, string value) {
			WriteStartElement(tag, ns);
			try {
				WriteShString(value);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportAnchorPoint(AnchorPoint anchorPoint) {
			WriteAnchorTag("col", SpreadsheetDrawingNamespace, anchorPoint.Col.ToString());
			WriteAnchorTag("colOff", SpreadsheetDrawingNamespace, Workbook.UnitConverter.ModelUnitsToEmu((int)Math.Round(anchorPoint.ColOffset)).ToString());
			WriteAnchorTag("row", SpreadsheetDrawingNamespace, anchorPoint.Row.ToString());
			WriteAnchorTag("rowOff", SpreadsheetDrawingNamespace, Workbook.UnitConverter.ModelUnitsToEmu((int)Math.Round(anchorPoint.RowOffset)).ToString());
		}
		public void ExportXfrmCore(Transform2D xfrm) {
			if (xfrm.FlipV != false)
				WriteBoolValue("flipV", xfrm.FlipV);
			if (xfrm.FlipH != false)
				WriteBoolValue("flipH", xfrm.FlipH);
			if (xfrm.Rotation != 0)
				WriteIntValue("rot", Workbook.UnitConverter.ModelUnitsToAdjAngle(xfrm.Rotation));
			ExportXfrmOffset(xfrm);
			ExportXfrmExtents(xfrm);
		}
		void ExportXfrmExtents(Transform2D xfrm) {
			WriteStartElement("ext", DrawingMLNamespace);
			try {
				ExportXfrmExtentsCore(xfrm);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportXfrmExtentsCore(Transform2D xfrm) {
			WriteIntValue("cx", Workbook.UnitConverter.ModelUnitsToEmu(xfrm.Cx));
			WriteIntValue("cy", Workbook.UnitConverter.ModelUnitsToEmu(xfrm.Cy));
		}
		void ExportXfrmOffset(Transform2D xfrm) {
			WriteStartElement("off", DrawingMLNamespace);
			try {
				ExportXfrmOffsetCore(xfrm);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportXfrmOffsetCore(Transform2D xfrm) {
			WriteIntValue("x", Workbook.UnitConverter.ModelUnitsToEmu(xfrm.OffsetX));
			WriteIntValue("y", Workbook.UnitConverter.ModelUnitsToEmu(xfrm.OffsetY));
		}
		#endregion
		#endregion
	}
	public class DrawingObjectExportWalker : IDrawingObjectVisitor {
		#region Fields
		OpenXmlExporter exporter;
		InternalSheetBase sheet;
		#endregion
		public DrawingObjectExportWalker(InternalSheetBase sheet, OpenXmlExporter exporter) {
			this.exporter = exporter;
			this.sheet = sheet;
		}
		public void Walk() {
			foreach (IDrawingObject drawing in sheet.DrawingObjectsByZOrderCollections)
				drawing.Visit(this);
		}
		public void Visit(Picture picture) {
			if (!exporter.ShouldExportPictures)
				return;
			WriteStartElement(picture);
			try {
				picture.CheckRotationAndSwapBox();
				ExportAnchorData(picture);
				picture.CheckRotationAndSwapBox();
				exporter.ExportPictureCore(picture);
				exporter.ExportClientData(picture);
			}
			finally {
				exporter.WriteEndElement();
			}
		}
		public void Visit(Chart chart) {
			if (!exporter.ShouldExportCharts)
				return;
			WriteStartElement(chart);
			try {
				ExportAnchorData(chart);
				exporter.CreateDrawingChartContent(chart);
				exporter.ExportClientData(chart);
			}
			finally {
				exporter.WriteEndElement();
			}
		}
		public void Visit(ModelShape shape) {
			if(!exporter.ShouldExportShapes)
				return;
			WriteStartElement(shape);
			try {
				shape.CheckRotationAndSwapBox();
				ExportAnchorData(shape);
				shape.CheckRotationAndSwapBox();
				exporter.ExportShapeCore(shape);
				exporter.ExportClientData(shape);
			} finally {
				exporter.WriteEndElement();
			}
		}
		public void Visit(GroupShape groupShape) {
			if(!exporter.ShouldExportDrawing(groupShape))
				return;
			WriteStartElement(groupShape);
			try {
				ExportAnchorData(groupShape);
				exporter.ExportGroupShapeCore(groupShape);
				exporter.ExportClientData(groupShape);
			}
			finally {
				exporter.WriteEndElement();
			}
		}
		public void Visit(ConnectionShape connectionShape) {
			if(!exporter.ShouldExportShapes)
				return;
			WriteStartElement(connectionShape);
			try {
				connectionShape.CheckRotationAndSwapBox();
				ExportAnchorData(connectionShape);
				connectionShape.CheckRotationAndSwapBox();
				exporter.ExportConnectionShapeCore(connectionShape);
				exporter.ExportClientData(connectionShape);
			}
			finally {
				exporter.WriteEndElement();
			}
		}
		void WriteStartElement(IDrawingObject drawing) {
			switch (drawing.AnchorType) {
				case AnchorType.TwoCell:
					exporter.WriteStartElement("twoCellAnchor", OpenXmlExporter.SpreadsheetDrawingNamespace);
					break;
				case AnchorType.OneCell:
					exporter.WriteStartElement("oneCellAnchor", OpenXmlExporter.SpreadsheetDrawingNamespace);
					break;
				case AnchorType.Absolute:
					exporter.WriteStartElement("absoluteAnchor", OpenXmlExporter.SpreadsheetDrawingNamespace);
					break;
			}
		}
		void ExportAnchorData(IDrawingObject drawing) {
			switch (drawing.AnchorType) {
				case AnchorType.TwoCell:
					AnchorType enumEditAs = drawing.ResizingBehavior;
					if (enumEditAs != AnchorType.TwoCell) {
						string editAs;
						if (Import.OpenXml.TwoCellAnchorDestination.ResizingBehaviorTable.TryGetValue(enumEditAs, out editAs))
							exporter.WriteStringValue("editAs", editAs);
					}
					exporter.ExportStartingAnchorPoint(drawing.From);
					exporter.ExportEndingAnchorPoint(drawing.To);
					break;
				case AnchorType.OneCell:
					exporter.ExportStartingAnchorPoint(drawing.From);
					exporter.ExportShapeExtent(drawing);
					break;
				case AnchorType.Absolute:
					exporter.ExportShapePosition(drawing);
					exporter.ExportShapeExtent(drawing);
					break;
			}
		}
	}
}
