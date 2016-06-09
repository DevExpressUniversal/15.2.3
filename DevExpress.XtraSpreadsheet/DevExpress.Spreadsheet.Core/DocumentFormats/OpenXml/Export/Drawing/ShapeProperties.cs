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
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Export Shape Properties
		protected internal virtual void ExportShapeProperties(ShapeProperties shapeProperties) {
			WriteStartElement("spPr", SpreadsheetDrawingNamespace);
			try {
				if (shapeProperties.BlackAndWhiteMode != OpenXmlBlackWhiteMode.Empty) {
					string bwMode;
					if (Import.OpenXml.ShapePropertiesDestination.BlackWhiteModeTable.TryGetValue(shapeProperties.BlackAndWhiteMode, out bwMode))
						WriteStringValue("bwMode", bwMode);
				}
				if (!shapeProperties.Transform2D.IsEmpty)
					ExportXfrm(shapeProperties.Transform2D);
				if(shapeProperties.ShapeType != ShapeType.None) {
					ExportPresetGeometry(shapeProperties);
				}
				else {
					ExportCustomGeometry(shapeProperties.CustomGeometry);
				}
				GenerateDrawingFillContent(shapeProperties.Fill);
				GenerateOutlineContent(shapeProperties.Outline);
				ExportDrawingEffectStyle(shapeProperties.EffectStyle);
			}
			finally {
				WriteEndElement();
			}
		}
		protected void ExportDrawingEffectStyle(DrawingEffectStyle style) {
			ContainerEffect containerEffect = style.ContainerEffect;
			if (containerEffect.Effects.Count != 0)
				GenerateContainerEffectContent(containerEffect);
			GenerateScene3DContent(style.Scene3DProperties);
			GenerateShape3DContent(style.Shape3DProperties);
		}
		void ExportPresetGeometry(ShapeProperties shapeProperties) {
			WriteStartElement("prstGeom", DrawingMLNamespace);
			try {
				string prstGeom;
				if(Import.OpenXml.PresetGeometryDestination.shapeTypeTable.TryGetValue(shapeProperties.ShapeType, out prstGeom))
					WriteStringValue("prst", prstGeom);
				if(shapeProperties.PresetAdjustList.Count != 0)
					ExportAdjustValues(shapeProperties.PresetAdjustList);
			}
			finally {
				WriteEndElement();
			}
		}
		public void ExportXfrm(Transform2D xfrm) {
			WriteStartElement("xfrm", DrawingMLNamespace);
			try {
				ExportXfrmCore(xfrm);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region CustomGeometry
		internal void ExportCustomGeometry(ModelShapeCustomGeometry customGeometry) {
			WriteStartElement("custGeom", DrawingMLNamespace);
			try {
				if(customGeometry.AdjustValues.Count > 0)
					ExportAdjustValues(customGeometry.AdjustValues);
				if(customGeometry.Guides.Count > 0)
					ExportGuides(customGeometry.Guides);
				if(customGeometry.AdjustHandles.Count > 0)
					ExportAdjustHandles(customGeometry.AdjustHandles);
				if(customGeometry.ConnectionSites.Count > 0)
					ExportShapeConnectionSites(customGeometry.ConnectionSites);
				if(!customGeometry.ShapeTextRectangle.IsEmpty())
					ExportShapeTextRectangle(customGeometry.ShapeTextRectangle);
				ExportShapePaths(customGeometry.Paths);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportShapePaths(ModelShapePathsList paths) {
			WriteStartElement("pathLst", DrawingMLNamespace);
			try {
				foreach(ModelShapePath path in paths) {
					ExportPath(path);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		#region PathFillModeTable
		internal static Dictionary<PathFillMode, string> PathFillModeTable = CreatePathFillModeTable();
		static Dictionary<PathFillMode, string> CreatePathFillModeTable() {
			Dictionary<PathFillMode, string> result = new Dictionary<PathFillMode, string>();
			result.Add(PathFillMode.None, "none");
			result.Add(PathFillMode.Norm, "norm");
			result.Add(PathFillMode.Lighten, "lighten");
			result.Add(PathFillMode.LightenLess, "lightenLess");
			result.Add(PathFillMode.Darken, "darken");
			result.Add(PathFillMode.DarkenLess, "darkenLess");
			return result;
		}
		#endregion
		class PathInsctructionExportWalker : IPathInstructionWalker {
			#region Fields
			ModelShapePath path;
			OpenXmlExporter exporter;
			#endregion
			public PathInsctructionExportWalker(ModelShapePath path, OpenXmlExporter exporter) {
				this.path = path;
				this.exporter = exporter;
			}
			public void Walk() {
				foreach(IPathInstruction instruction in path.Instructions) {
					instruction.Visit(this);
				}
			}
			#region Implementation of IPathIncstructionWalker
			public void Visit(PathArc pathArc) {
				exporter.ExportShapePathArc(pathArc);
			}
			public void Visit(PathClose value) {
				exporter.ExportShapePathClose(value);
			}
			public void Visit(PathCubicBezier value) {
				exporter.ExportShapePathCubicBezier(value);
			}
			public void Visit(PathLine pathLine) {
				exporter.ExportShapePathLine(pathLine);
			}
			public void Visit(PathMove pathMove) {
				exporter.ExportShapePathMove(pathMove);
			}
			public void Visit(PathQuadraticBezier value) {
				exporter.ExportShapePathQuadraticBezier(value);
			}
			#endregion
		}
		internal void ExportShapePathQuadraticBezier(PathQuadraticBezier value) {
			WriteStartElement("quadBezTo", DrawingMLNamespace);
			try {
				foreach(AdjustablePoint point in value.Points) {
					ExportShapePathPosition(point);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportShapePathMove(PathMove value) {
			WriteStartElement("moveTo", DrawingMLNamespace);
			try {
				ExportShapePathPosition(value.Point);
			} finally {
				WriteEndElement();
			}
		}
		internal void ExportShapePathLine(PathLine value) {
			WriteStartElement("lnTo", DrawingMLNamespace);
			try {
				ExportShapePathPosition(value.Point);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportShapePathCubicBezier(PathCubicBezier value) {
			WriteStartElement("cubicBezTo", DrawingMLNamespace);
			try {
				foreach(AdjustablePoint adjustablePoint in value.Points) {
					ExportShapePathPosition(adjustablePoint);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportShapePathClose(PathClose value) {
			WriteStartElement("close", DrawingMLNamespace);
			WriteEndElement();
		}
		internal void ExportShapePathArc(PathArc value) {
			WriteStartElement("arcTo", DrawingMLNamespace);
			try {
				WriteStringValue("wR", value.WidthRadius.ToString());
				WriteStringValue("hR", value.HeightRadius.ToString());
				WriteStringValue("stAng", value.StartAngle.ToString());
				WriteStringValue("swAng", value.SwingAngle.ToString());
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportPath(ModelShapePath path) {
			WriteStartElement("path", DrawingMLNamespace);
			try {
				WriteLongValue("w", path.Width, 0);
				WriteLongValue("h", path.Height, 0);
				WriteEnumValue("fill", path.FillMode, PathFillModeTable, PathFillMode.Norm);
				WriteBoolValue("stroke", path.Stroke, true);
				WriteBoolValue("extrusionOk", path.ExtrusionOK, true);
				PathInsctructionExportWalker walker = new PathInsctructionExportWalker(path, this);
				walker.Walk();
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportShapeTextRectangle(AdjustableRect shapeTextRectangle) {
			WriteStartElement("rect", DrawingMLNamespace);
			try {
				WriteStringValue("l", shapeTextRectangle.Left.ToString());
				WriteStringValue("t", shapeTextRectangle.Top.ToString());
				WriteStringValue("r", shapeTextRectangle.Right.ToString());
				WriteStringValue("b", shapeTextRectangle.Bottom.ToString());
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportGuides(ModelShapeGuideList guides) {
			WriteStartElement("gdLst", DrawingMLNamespace);
			try {
				ExportGuideList(guides);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportShapeConnectionSites(ModelShapeConnectionList connectionSites) {
			WriteStartElement("cxnLst", DrawingMLNamespace);
			try {
				foreach(ModelShapeConnection connectionSite in connectionSites) {
					ExportShapeConnectionSite(connectionSite);
				}
			} finally {
				WriteEndElement();
			}			
		}
		internal void ExportShapeConnectionSite(ModelShapeConnection connectionSite) {
			WriteStartElement("cxn", DrawingMLNamespace);
			try {
				WriteStringValue("ang", connectionSite.Angle.ToString());
				ExportShapePositionCoordinate(connectionSite);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportAdjustValues(ModelShapeGuideList adjustValues) {
			WriteStartElement("avLst", DrawingMLNamespace);
			try {
				ExportGuideList(adjustValues);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportGuideList(ModelShapeGuideList guideList) {
			foreach(ModelShapeGuide modelShapeGuide in guideList) {
				ExportShapeGuide(modelShapeGuide);
			}
		}
		internal void ExportShapeGuide(ModelShapeGuide modelShapeGuide) {
			WriteStartElement("gd", DrawingMLNamespace);
			try {
				WriteStringValue("name", modelShapeGuide.Name);
				WriteStringValue("fmla", modelShapeGuide.Formula);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportAdjustHandles(ModelAdjustHandlesList adjustHandles) {
			WriteStartElement("ahLst", DrawingMLNamespace);
			try {
				foreach(AdjustablePoint adjustHandle in adjustHandles) {
					XYAdjustHandle xyAdjustHandle = adjustHandle as XYAdjustHandle;
					if(xyAdjustHandle != null) {
						ExportXYAdjustHandle(xyAdjustHandle);
					}
					else
						ExportPolarAdjustHandle((PolarAdjustHandle) adjustHandle);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportXYAdjustHandle(XYAdjustHandle xyAdjustHandle) {
			WriteStartElement("ahXY", DrawingMLNamespace);
			try {
				if(!String.IsNullOrEmpty(xyAdjustHandle.HorizontalGuide))
					WriteStringValue("gdRefX", xyAdjustHandle.HorizontalGuide);
				if(xyAdjustHandle.MinX != null)
					WriteStringValue("minX", xyAdjustHandle.MinX.ToString());
				if(xyAdjustHandle.MaxX != null)
					WriteStringValue("maxX", xyAdjustHandle.MaxX.ToString());
				if(!String.IsNullOrEmpty(xyAdjustHandle.VerticalGuide))
					WriteStringValue("gdRefY", xyAdjustHandle.VerticalGuide);
				if(xyAdjustHandle.MinY != null)
					WriteStringValue("minY", xyAdjustHandle.MinY.ToString());
				if(xyAdjustHandle.MaxY != null)
					WriteStringValue("maxY", xyAdjustHandle.MaxY.ToString());
				ExportShapePositionCoordinate(xyAdjustHandle);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportPolarAdjustHandle(PolarAdjustHandle polarAdjustHandle) {
			WriteStartElement("ahPolar", DrawingMLNamespace);
			try {
				if(!String.IsNullOrEmpty(polarAdjustHandle.RadialGuide))
					WriteStringValue("gdRefR", polarAdjustHandle.RadialGuide);
				if(polarAdjustHandle.MinimumRadial != null)
					WriteStringValue("minR", polarAdjustHandle.MinimumRadial.ToString());
				if(polarAdjustHandle.MaximumRadial != null)
					WriteStringValue("maxR", polarAdjustHandle.MaximumRadial.ToString());
				if(!String.IsNullOrEmpty(polarAdjustHandle.AngleGuide))
					WriteStringValue("gdRefAng", polarAdjustHandle.AngleGuide);
				if(polarAdjustHandle.MinimumAngle != null)
					WriteStringValue("minAng", polarAdjustHandle.MinimumAngle.ToString());
				if(polarAdjustHandle.MaximumAngle != null)
					WriteStringValue("maxAng", polarAdjustHandle.MaximumAngle.ToString());
				ExportShapePositionCoordinate(polarAdjustHandle);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportShapePathPosition(AdjustablePoint adjustablePoint) {
			WriteStartElement("pt", DrawingMLNamespace);
			try {
				ExportShapePositionCore(adjustablePoint);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportShapePositionCoordinate(AdjustablePoint adjustablePoint) {
			WriteStartElement("pos", DrawingMLNamespace);
			try {
				ExportShapePositionCore(adjustablePoint);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportShapePositionCore(AdjustablePoint adjustablePoint) {
			WriteStringValue("x", adjustablePoint.X.ToString());
			WriteStringValue("y", adjustablePoint.Y.ToString());
		}
		#endregion
	}
}
