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

using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		internal void ExportGroupShapeCore(GroupShape groupShape) {
			if(!ShouldExportDrawing(groupShape))
				return;
			WriteStartElement("grpSp", SpreadsheetDrawingNamespace);
			try {
				ExportNonVisualGroupShapeProperties(groupShape);
				ExportGroupShapeProperties(groupShape.GroupShapeProperties);
				GroupShapeDrawingsWalker walker = new GroupShapeDrawingsWalker(this, groupShape);
				walker.Walk();
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportGroupShapeProperties(GroupShapeProperties groupShapeProperties) {
			WriteStartElement("grpSpPr", SpreadsheetDrawingNamespace);
			try {
				WriteEnumValue("bwMode", groupShapeProperties.BlackAndWhiteMode, Import.OpenXml.ShapePropertiesDestination.BlackWhiteModeTable, OpenXmlBlackWhiteMode.Empty);
				if(!groupShapeProperties.Transform2D.IsEmpty)
					ExportGroupShapeXfrm(groupShapeProperties.Transform2D);
				GenerateDrawingFillContent(groupShapeProperties.Fill);
				ContainerEffect containerEffect = groupShapeProperties.EffectStyle.ContainerEffect;
				if(containerEffect.Effects.Count != 0)
					GenerateContainerEffectContent(containerEffect);
				GenerateScene3DContent(groupShapeProperties.EffectStyle.Scene3DProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportGroupShapeXfrm(GroupTransform2D groupTransform) {
			WriteStartElement("xfrm", DrawingMLNamespace);
			try {
				ExportXfrmCore(groupTransform.MainTransform);
				if(groupTransform.ChildTransform.IsEmpty)
					return;
				ExportChildOffset(groupTransform.ChildTransform);
				ExportChildExtents(groupTransform.ChildTransform);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportChildOffset(Transform2D childTransform) {
			WriteStartElement("chOff", DrawingMLNamespace);
			try {
				ExportXfrmOffsetCore(childTransform);
			} finally {
				WriteEndElement();
			}			
		}
		void ExportChildExtents(Transform2D childTransform) {
			WriteStartElement("chExt", DrawingMLNamespace);
			try {
				ExportXfrmExtentsCore(childTransform);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportNonVisualGroupShapeProperties(GroupShape groupShape) {
			WriteStartElement("nvGrpSpPr", SpreadsheetDrawingNamespace);
			try {
				ExportNonVisualDrawingProperties(groupShape.DrawingObject);
				ExportNonVisualGroupShapeDrawingProperties(groupShape);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportNonVisualGroupShapeDrawingProperties(GroupShape groupShape) {
			WriteStartElement("cNvGrpSpPr", SpreadsheetDrawingNamespace);
			try {
				ExportGroupShapeLocks(groupShape.Locks);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportGroupShapeLocks(GroupShapeLocks groupShapeLocks) {
			WriteStartElement("grpSpLocks", DrawingMLNamespace);
			try {
				ExportCommonDrawingLocks(groupShapeLocks);
				WriteBoolValue("noResize", groupShapeLocks.NoResize, false);
				WriteBoolValue("noRot", groupShapeLocks.NoRotate, false);
				WriteBoolValue("noUngrp", groupShapeLocks.NoUngroup, false);
			}
			finally {
				WriteEndElement();
			}
		}
	}
	internal class GroupShapeDrawingsWalker : IDrawingObjectVisitor {
		#region Fields
		readonly OpenXmlExporter exporter;
		readonly IDrawingObjectsContainer drawingsContainer;
		#endregion
		public GroupShapeDrawingsWalker(OpenXmlExporter exporter, IDrawingObjectsContainer groupShape) {
			this.exporter = exporter;
			this.drawingsContainer = groupShape;
		}
		public void Walk() {
			foreach(IDrawingObject drawingObject in drawingsContainer.DrawingObjects) {
				drawingObject.Visit(this);
			}
		}
		#region Implementation of IDrawingObjectVisitor
		public void Visit(Picture picture) {
			exporter.ExportPictureCore(picture);
		}
		public void Visit(Chart chart) {
			exporter.CreateDrawingChartContent(chart);
		}
		public void Visit(ModelShape shape) {
			exporter.ExportShapeCore(shape);
		}
		public void Visit(GroupShape groupShape) {
			exporter.ExportGroupShapeCore(groupShape);
		}
		public void Visit(ConnectionShape connectionShape) {
			exporter.ExportConnectionShapeCore(connectionShape);
		}
		#endregion
	}
}
