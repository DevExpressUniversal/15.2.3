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

using System.Collections.Generic;
using DevExpress.Export.Xl;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		public bool ShouldExportShapes { get { return Workbook.DocumentCapabilities.ShapesAllowed; } }
		public void ExportShapeCore(ModelShape shape) {
			WriteStartElement("sp", SpreadsheetDrawingNamespace);
			try {
				if(!string.IsNullOrEmpty(shape.Macro))
					WriteStringValue("macro", shape.Macro);
				ExportNonVisualShapeProperties(shape);
				ExportShapeProperties(shape.ShapeProperties);
				ExportShapeStyle(shape.ShapeStyle);
				ExportTextBody(shape.TextProperties);
			} finally {
				WriteEndElement();
			}
		}
		void ExportTextBody(TextProperties textProperties) {
			if(textProperties.IsDefault)
				return;
			WriteStartElement("txBody", SpreadsheetDrawingNamespace);
			try {
				GenerateTextPropertiesContentCore(textProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportShapeStyle(ShapeStyle shapeStyle) {
			WriteStartElement("style", SpreadsheetDrawingNamespace);
			try {
				ExportLineReference(shapeStyle);
				ExportFillReference(shapeStyle);
				ExportEffectReference(shapeStyle);
				ExportFontReference(shapeStyle);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportLineReference(ShapeStyle shapeStyle) {
			WriteStartElement("lnRef", DrawingMLNamespace);
			try {
				ExportStyleMatrixReference(shapeStyle.LineReferenceIdx, shapeStyle.LineColor);
			}
			finally {
				WriteEndElement();
			}
		}
		#region FontCollectionIndexTable
		public static Dictionary<XlFontSchemeStyles, string> FontCollectionIndexTable = GetFontCollectionIndexTable();
		static Dictionary<XlFontSchemeStyles, string> GetFontCollectionIndexTable() {
			Dictionary<XlFontSchemeStyles, string> result = new Dictionary<XlFontSchemeStyles, string>();
			result.Add(XlFontSchemeStyles.Major, "major");
			result.Add(XlFontSchemeStyles.Minor, "minor");
			result.Add(XlFontSchemeStyles.None, "none");
			return result;
		}
		#endregion
		void ExportFontReference(ShapeStyle shapeStyle) {
			WriteStartElement("fontRef", DrawingMLNamespace);
			try {
				WriteEnumValue("idx", shapeStyle.FontReferenceIdx, FontCollectionIndexTable);
				if(shapeStyle.FontColor != null)
					GenerateDrawingColorContent(shapeStyle.FontColor);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportFillReference(ShapeStyle shapeStyle) {
			WriteStartElement("fillRef", DrawingMLNamespace);
			try {
				ExportStyleMatrixReference(shapeStyle.FillReferenceIdx, shapeStyle.FillColor);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportEffectReference(ShapeStyle shapeStyle) {
			WriteStartElement("effectRef", DrawingMLNamespace);
			try {
				ExportStyleMatrixReference(shapeStyle.EffectReferenceIdx, shapeStyle.EffectColor);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportStyleMatrixReference(int idx, DrawingColor effectColor) {
			WriteIntValue("idx", idx);
			if(effectColor != null)
				GenerateDrawingColorContent(effectColor);
		}
		public void ExportNonVisualShapeProperties(ModelShape shape) {
			WriteStartElement("nvSpPr", SpreadsheetDrawingNamespace);
			try {
				ExportNonVisualDrawingProperties(shape.DrawingObject);
				ExportConnectionNonVisualShapeProperties(shape);
			} finally {
				WriteEndElement();
			}
		}
		public void ExportConnectionNonVisualShapeProperties(ModelShape shape) {
			WriteStartElement("cNvSpPr", SpreadsheetDrawingNamespace);
			try {
				WriteBoolValue("txBox", shape.TextBoxMode, false);
				if(!shape.Locks.IsEmpty)
					ExportShapeLocks(shape.Locks);
			}
			finally {
				WriteEndElement();
			}
		}
		public void ExportShapeLocks(ShapeLocks shapeLocks) {
			WriteStartElement("spLocks", DrawingMLNamespace);
			try {
				ExportDrawingLocks(shapeLocks);
				WriteBoolValue("noTextEdit", shapeLocks.NoTextEdit, false);
			}
			finally {
				WriteEndElement();
			}
		}
		public void ExportConnectionShapeCore(ConnectionShape connectionShape) {
			WriteStartElement("cxnSp", SpreadsheetDrawingNamespace);
			try {
				if(!string.IsNullOrEmpty(connectionShape.Macro))
					WriteStringValue("macro", connectionShape.Macro);
				ExportNonVisualConnectionShapeProperties(connectionShape);
				ExportShapeProperties(connectionShape.ShapeProperties);
				ExportShapeStyle(connectionShape.ShapeStyle);
			} finally {
				WriteEndElement();
			}			
		}
		internal void ExportNonVisualConnectionShapeProperties(ConnectionShape connectionShape) {
			WriteStartElement("nvCxnSpPr", SpreadsheetDrawingNamespace);
			try {
				ExportNonVisualDrawingProperties(connectionShape.DrawingObject.Properties);
				ExportNonVisualConnectorShapeDrawingProperties(connectionShape);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportNonVisualConnectorShapeDrawingProperties(ConnectionShape connectionShape) {
			WriteStartElement("cNvCxnSpPr", SpreadsheetDrawingNamespace);
			try {
				if(!connectionShape.Locks.IsEmpty)
					ExportConnectionShapeLocks(connectionShape.Locks);
				ExportConnectionStart(connectionShape);
				ExportConnectionEnd(connectionShape);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportConnectionShapeLocks(ConnectionShapeLocks locks) {
			WriteStartElement("cxnSpLocks", DrawingMLNamespace);
			try {
				ExportDrawingLocks(locks);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportConnectionEnd(ConnectionShape connectionShape) {
			WriteStartElement("endCxn", DrawingMLNamespace);
			try {
				ExportConnectionShapeConnectionCore(connectionShape.EndConnectionId, connectionShape.EndConnectionIdx);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportConnectionStart(ConnectionShape connectionShape) {
			WriteStartElement("stCxn", DrawingMLNamespace);
			try {
				ExportConnectionShapeConnectionCore(connectionShape.StartConnectionId, connectionShape.StartConnectionIdx);
			} finally {
				WriteEndElement();
			}
		}
		internal void ExportConnectionShapeConnectionCore(int id, int idx) {
			WriteIntValue("id", id);
			WriteIntValue("idx", idx);
		}
	}
}
