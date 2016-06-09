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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region CustomGeometryDestination
	public class CustomGeometryDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("avLst", OnAdjustValuesList);
			result.Add("gdLst", OnGuidesList);
			result.Add("ahLst", OnAdjustHandlesList);
			result.Add("cxnLst", OnConnectionSitesList);
			result.Add("rect", OnShapeTextRectangle);
			result.Add("pathLst", OnShapePathsList);
			return result;
		}
		#endregion
		readonly ModelShapeCustomGeometry shapeCustomGeometry;
		public CustomGeometryDestination(SpreadsheetMLBaseImporter importer, ModelShapeCustomGeometry shapeCustomGeometry)
			: base(importer) {
			this.shapeCustomGeometry = shapeCustomGeometry;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnAdjustValuesList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ModelShapeGuideListDestination(importer, GetThis(importer).shapeCustomGeometry.AdjustValues);
		}
		static Destination OnGuidesList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ModelShapeGuideListDestination(importer, GetThis(importer).shapeCustomGeometry.Guides);
		}
		static Destination OnAdjustHandlesList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new AdjustHandlesListDestination(importer, GetThis(importer).shapeCustomGeometry.AdjustHandles);
		}
		static Destination OnConnectionSitesList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ConnectionSitesDestination(importer, GetThis(importer).shapeCustomGeometry.ConnectionSites);
		}
		static Destination OnShapeTextRectangle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ShapeTextRectangleDestination(importer, GetThis(importer).shapeCustomGeometry.ShapeTextRectangle);
		}
		static Destination OnShapePathsList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ModelShapePathsDestination(importer, GetThis(importer).shapeCustomGeometry.Paths);
		}
		#endregion
		static CustomGeometryDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (CustomGeometryDestination) importer.PeekDestination();
		}
	}
	#endregion
	#region AdjustHandlesListDestination
	public class AdjustHandlesListDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("ahXY", OnXYAdjustableHandle);
			result.Add("ahPolar", OnPolarAdjustableHandle);
			return result;
		}
		#endregion
		readonly ModelAdjustHandlesList adjustHandlesList;
		public AdjustHandlesListDestination(SpreadsheetMLBaseImporter importer, ModelAdjustHandlesList adjustHandlesList)
			: base(importer) {
			this.adjustHandlesList = adjustHandlesList;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnXYAdjustableHandle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			XYAdjustHandle adjustHandle = new XYAdjustHandle();
			GetThis(importer).adjustHandlesList.Add(adjustHandle);
			return new XYAdjustableHandleDestination(importer, adjustHandle);
		}
		static Destination OnPolarAdjustableHandle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PolarAdjustHandle polarAdjustHandle = new PolarAdjustHandle();
			GetThis(importer).adjustHandlesList.Add(polarAdjustHandle);
			return new PolarAdjustableHandleDestination(importer, polarAdjustHandle);
		}
		#endregion
		static AdjustHandlesListDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (AdjustHandlesListDestination) importer.PeekDestination();
		}
	}
	#endregion
	#region PolarAdjustableHandleDestination
	public class PolarAdjustableHandleDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pos", OnAdjustPoint2D);
			return result;
		}
		#endregion
		readonly PolarAdjustHandle polarAdjustHandle;
		public PolarAdjustableHandleDestination(SpreadsheetMLBaseImporter importer, PolarAdjustHandle polarAdjustHandle)
			: base(importer) {
			this.polarAdjustHandle = polarAdjustHandle;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnAdjustPoint2D(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new AdjustPoint2DDestination(importer, GetThis(importer).polarAdjustHandle);
		}
		#endregion
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			polarAdjustHandle.RadialGuide = reader.GetAttribute("gdRefR");
			polarAdjustHandle.AngleGuide = reader.GetAttribute("gdRefAng");
			polarAdjustHandle.MinimumRadial = AdjustableCoordinate.FromString(reader.GetAttribute("minR"));
			polarAdjustHandle.MaximumRadial = AdjustableCoordinate.FromString(reader.GetAttribute("maxR"));
			polarAdjustHandle.MinimumAngle = AdjustableAngle.FromString(reader.GetAttribute("minAng"));
			polarAdjustHandle.MaximumAngle = AdjustableAngle.FromString(reader.GetAttribute("maxAng"));
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementClose(XmlReader reader) {
			if(polarAdjustHandle.X == null || polarAdjustHandle.Y == null)
				Importer.ThrowInvalidFile("Invalid polar adjust handle");
		}
		#endregion
		#endregion
		static PolarAdjustableHandleDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PolarAdjustableHandleDestination) importer.PeekDestination();
		}
	}
	#endregion
	#region AdjustPoint2DDestination
	public class AdjustPoint2DDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly AdjustablePoint adjustablePoint;
		public AdjustPoint2DDestination(SpreadsheetMLBaseImporter importer, AdjustablePoint adjustablePoint)
			: base(importer) {
			this.adjustablePoint = adjustablePoint;
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			adjustablePoint.X = AdjustableCoordinate.FromString(reader.GetAttribute("x"));
			adjustablePoint.Y = AdjustableCoordinate.FromString(reader.GetAttribute("y"));
			if(adjustablePoint.X == null || adjustablePoint.Y == null)
				Importer.ThrowInvalidFile("Invalid x/y position");
		}
		#endregion
	}
	#endregion
	#region XYAdjustableHandleDestination
	public class XYAdjustableHandleDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pos", OnAdjustPoint2D);
			return result;
		}
		#endregion
		readonly XYAdjustHandle xyAdjustHandle;
		public XYAdjustableHandleDestination(SpreadsheetMLBaseImporter importer, XYAdjustHandle xyAdjustHandle)
			: base(importer) {
			this.xyAdjustHandle = xyAdjustHandle;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnAdjustPoint2D(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new AdjustPoint2DDestination(importer, GetThis(importer).xyAdjustHandle);
		}
		#endregion
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			xyAdjustHandle.HorizontalGuide = reader.GetAttribute("gdRefX");
			xyAdjustHandle.VerticalGuide = reader.GetAttribute("gdRefY");
			xyAdjustHandle.MinX = AdjustableCoordinate.FromString(reader.GetAttribute("minX"));
			xyAdjustHandle.MaxX = AdjustableCoordinate.FromString(reader.GetAttribute("maxX"));
			xyAdjustHandle.MinY = AdjustableCoordinate.FromString(reader.GetAttribute("minY"));
			xyAdjustHandle.MaxY = AdjustableCoordinate.FromString(reader.GetAttribute("maxY"));
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementClose(XmlReader reader) {
			if(xyAdjustHandle.X == null || xyAdjustHandle.Y == null) {
				Importer.ThrowInvalidFile("Invalid XY adjust handle");
			}
		}
		#endregion
		#endregion
		static XYAdjustableHandleDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (XYAdjustableHandleDestination) importer.PeekDestination();
		}
	}
	#endregion
	#region ModelShapeGuideListDestination
	public class ModelShapeGuideListDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("gd", OnShapeGuide);
			return result;
		}
		#endregion
		readonly ModelShapeGuideList modelShapeGuideList;
		public ModelShapeGuideListDestination(SpreadsheetMLBaseImporter importer, ModelShapeGuideList modelShapeGuideList)
			: base(importer) {
			this.modelShapeGuideList = modelShapeGuideList;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnShapeGuide(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ModelShapeGuide modelShapeGuide = new ModelShapeGuide();
			GetThis(importer).modelShapeGuideList.Add(modelShapeGuide);
			return new ModelShapeGuideDestination(importer, modelShapeGuide);
		}
		#endregion
		static ModelShapeGuideListDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ModelShapeGuideListDestination) importer.PeekDestination();
		}
	}
	#endregion
	#region ModelShapeGuideDestination
	public class ModelShapeGuideDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly ModelShapeGuide modelShapeGuide;
		public ModelShapeGuideDestination(SpreadsheetMLBaseImporter importer, ModelShapeGuide modelShapeGuide)
			: base(importer) {
			this.modelShapeGuide = modelShapeGuide;
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			modelShapeGuide.Formula = reader.GetAttribute("fmla");
			modelShapeGuide.Name = reader.GetAttribute("name");
			if(String.IsNullOrEmpty(modelShapeGuide.Formula) || String.IsNullOrEmpty(modelShapeGuide.Name))
				Importer.ThrowInvalidFile("Invalid Shape Guide");
		}
		#endregion
	}
	#endregion
	#region ConnectionSitesDestination
	public class ConnectionSitesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cxn", OnShapeConnection);
			return result;
		}
		#endregion
		readonly ModelShapeConnectionList modelShapeConnectionList;
		public ConnectionSitesDestination(SpreadsheetMLBaseImporter importer, ModelShapeConnectionList modelShapeConnectionList)
			: base(importer) {
			this.modelShapeConnectionList = modelShapeConnectionList;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnShapeConnection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ModelShapeConnection modelShapeConnection = new ModelShapeConnection();
			GetThis(importer).modelShapeConnectionList.Add(modelShapeConnection);
			return new ModelShapeConnectionDestination(importer, modelShapeConnection);
		}
		#endregion
		static ConnectionSitesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConnectionSitesDestination) importer.PeekDestination();
		}
	}
	#endregion
	#region ModelShapeConnectionDestination
	public class ModelShapeConnectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pos", OnAdjustPoint2D);
			return result;
		}
		#endregion
		readonly ModelShapeConnection modelShapeConnection;
		public ModelShapeConnectionDestination(SpreadsheetMLBaseImporter importer, ModelShapeConnection modelShapeConnection)
			: base(importer) {
			this.modelShapeConnection = modelShapeConnection;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnAdjustPoint2D(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new AdjustPoint2DDestination(importer, GetThis(importer).modelShapeConnection);
		}
		#endregion
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			modelShapeConnection.Angle = AdjustableAngle.FromString(reader.GetAttribute("ang"));
		}
		public override void ProcessElementClose(XmlReader reader) {
			if(modelShapeConnection.Angle == null || modelShapeConnection.X == null || modelShapeConnection.Y == null)
				Importer.ThrowInvalidFile("Invalid shape connection");
		}
		#endregion
		static ModelShapeConnectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ModelShapeConnectionDestination) importer.PeekDestination();
		}
	}
	#endregion
	#region ModelShapePathsDestination
	public class ModelShapePathsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("path", OnModelShapePath);
			return result;
		}
		#endregion
		readonly ModelShapePathsList modelShapePathsList;
		public ModelShapePathsDestination(SpreadsheetMLBaseImporter importer, ModelShapePathsList modelShapePathsList)
			: base(importer) {
			this.modelShapePathsList = modelShapePathsList;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnModelShapePath(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ModelShapePath modelShapePath = new ModelShapePath(importer.DocumentModel);
			GetThis(importer).modelShapePathsList.Add(modelShapePath);
			return new ModelShapePathDestination(importer, modelShapePath);
		}
		#endregion
		static ModelShapePathsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ModelShapePathsDestination) importer.PeekDestination();
		}
	}
	#endregion
	#region ModelShapePathDestination
	public class ModelShapePathDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly Dictionary<PathFillMode, string> pathFillModeTable = GetPathFillModeTable();
		static Dictionary<PathFillMode, string> GetPathFillModeTable() {
			Dictionary<PathFillMode, string> result = new Dictionary<PathFillMode, string>();
			result.Add(PathFillMode.None, "none");
			result.Add(PathFillMode.Norm, "norm");
			result.Add(PathFillMode.Lighten, "lighten");
			result.Add(PathFillMode.LightenLess, "lightenLess");
			result.Add(PathFillMode.Darken, "darken");
			result.Add(PathFillMode.DarkenLess, "darkenLess");
			return result;
		}
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("close", OnClose);
			result.Add("moveTo", OnMoveTo);
			result.Add("lnTo", OnLineTo);
			result.Add("arcTo", OnArcTo);
			result.Add("quadBezTo", OnQuadraticBezierTo);
			result.Add("cubicBezTo", OnCubicBezierTo);
			return result;
		}
		#endregion
		readonly ModelShapePath modelShapePath;
		public ModelShapePathDestination(SpreadsheetMLBaseImporter importer, ModelShapePath modelShapePath)
			: base(importer) {
			this.modelShapePath = modelShapePath;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnClose(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PathClose pathClose = new PathClose();
			GetThis(importer).modelShapePath.Instructions.Add(pathClose);
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnMoveTo(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PathMove pathMove = new PathMove();
			GetThis(importer).modelShapePath.Instructions.Add(pathMove);
			return new PathMoveDestination(importer, pathMove);
		}
		static Destination OnLineTo(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PathLine pathLine = new PathLine();
			GetThis(importer).modelShapePath.Instructions.Add(pathLine);
			return new PathLineDestination(importer, pathLine);
		}
		static Destination OnArcTo(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PathArc pathArc = new PathArc();
			GetThis(importer).modelShapePath.Instructions.Add(pathArc);
			return new PathArcDestination(importer, pathArc);
		}
		static Destination OnQuadraticBezierTo(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PathQuadraticBezier pathQuadraticBezier = new PathQuadraticBezier(importer.DocumentModel);
			GetThis(importer).modelShapePath.Instructions.Add(pathQuadraticBezier);
			return new PathQuadraticBezierDestination(importer, pathQuadraticBezier);
		}
		static Destination OnCubicBezierTo(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PathCubicBezier pathCubicBezier = new PathCubicBezier(importer.DocumentModel);
			GetThis(importer).modelShapePath.Instructions.Add(pathCubicBezier);
			return new PathCubicBezierDestination(importer, pathCubicBezier);
		}
		#endregion
		static ModelShapePathDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ModelShapePathDestination) importer.PeekDestination();
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			modelShapePath.Width = Importer.GetLongValue(reader, "w", 0);
			if(modelShapePath.Width < 0)
				Importer.ThrowInvalidFile("Invalid Path Width");
			modelShapePath.Height = Importer.GetLongValue(reader, "h", 0);
			if(modelShapePath.Height < 0)
				Importer.ThrowInvalidFile("Invalid Path Height");
			modelShapePath.FillMode = Importer.GetWpEnumValue(reader, "fill", pathFillModeTable, PathFillMode.Norm);
			modelShapePath.Stroke = Importer.GetWpSTOnOffValue(reader, "stroke", true);
			modelShapePath.ExtrusionOK = Importer.GetWpSTOnOffValue(reader, "extrusionOk", true);
		}
		#endregion
	}
	#endregion
	#region PathArcDestination
	public class PathArcDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PathArc pathArc;
		public PathArcDestination(SpreadsheetMLBaseImporter importer, PathArc pathArc)
			: base(importer) {
			this.pathArc = pathArc;
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			pathArc.HeightRadius = AdjustableCoordinate.FromString(reader.GetAttribute("hR"));
			pathArc.WidthRadius = AdjustableCoordinate.FromString(reader.GetAttribute("wR"));
			pathArc.StartAngle = AdjustableAngle.FromString(reader.GetAttribute("stAng"));
			pathArc.SwingAngle = AdjustableAngle.FromString(reader.GetAttribute("swAng"));
			if(pathArc.HeightRadius == null | pathArc.WidthRadius == null || pathArc.StartAngle == null || pathArc.SwingAngle == null)
				Importer.ThrowInvalidFile("Invalid Path element ArcTo");
		}
		#endregion
	}
	#endregion
	#region PathCubicBezierDestination
	public class PathCubicBezierDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pt", OnPoint);
			return result;
		}
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		readonly PathCubicBezier pathCubicBezier;
		public PathCubicBezierDestination(SpreadsheetMLBaseImporter importer, PathCubicBezier pathCubicBezier)
			: base(importer) {
			this.pathCubicBezier = pathCubicBezier;
		}
		#region Handler
		static Destination OnPoint(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AdjustablePoint point = new AdjustablePoint();
			GetThis(importer).pathCubicBezier.Points.Add(point);
			return new AdjustPoint2DDestination(importer, point);
		}
		#endregion
		static PathCubicBezierDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PathCubicBezierDestination) importer.PeekDestination();
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementClose(XmlReader reader) {
			if(pathCubicBezier.Points.Count != 3)
				Importer.ThrowInvalidFile("Invalid Path element Cubic Bezier");
		}
		#endregion
	}
	#endregion
	#region PathLineDestination
	public class PathLineDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pt", OnPoint);
			return result;
		}
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		readonly PathLine pathLine;
		public PathLineDestination(SpreadsheetMLBaseImporter importer, PathLine pathLine)
			: base(importer) {
			this.pathLine = pathLine;
		}
		#region Handler
		static Destination OnPoint(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).pathLine.Point = new AdjustablePoint();
			return new AdjustPoint2DDestination(importer, GetThis(importer).pathLine.Point);
		}
		#endregion
		static PathLineDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PathLineDestination) importer.PeekDestination();
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementClose(XmlReader reader) {
			if(pathLine.Point == null)
				Importer.ThrowInvalidFile("Invalid Path element Line");
		}
		#endregion
	}
	#endregion
	#region PathMoveDestination
	public class PathMoveDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pt", OnPoint);
			return result;
		}
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		readonly PathMove pathMove;
		public PathMoveDestination(SpreadsheetMLBaseImporter importer, PathMove pathMove)
			: base(importer) {
			this.pathMove = pathMove;
		}
		#region Handler
		static Destination OnPoint(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).pathMove.Point = new AdjustablePoint();
			return new AdjustPoint2DDestination(importer, GetThis(importer).pathMove.Point);
		}
		#endregion
		static PathMoveDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PathMoveDestination) importer.PeekDestination();
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementClose(XmlReader reader) {
			if(pathMove.Point == null)
				Importer.ThrowInvalidFile("Invalid Path element Move");
		}
		#endregion
	}
	#endregion
	#region PathQuadraticBezierDestination
	public class PathQuadraticBezierDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pt", OnPoint);
			return result;
		}
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		readonly PathQuadraticBezier pathQuadraticBezier;
		public PathQuadraticBezierDestination(SpreadsheetMLBaseImporter importer, PathQuadraticBezier pathQuadraticBezier)
			: base(importer) {
			this.pathQuadraticBezier = pathQuadraticBezier;
		}
		#region Handler
		static Destination OnPoint(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AdjustablePoint point = new AdjustablePoint();
			GetThis(importer).pathQuadraticBezier.Points.Add(point);
			return new AdjustPoint2DDestination(importer, point);
		}
		#endregion
		static PathQuadraticBezierDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PathQuadraticBezierDestination) importer.PeekDestination();
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementClose(XmlReader reader) {
			if(pathQuadraticBezier.Points.Count != 2)
				Importer.ThrowInvalidFile("Invalid Path element Quadratic Bezier");
		}
		#endregion
	}
	#endregion
	#region ShapeTextRectangleDestination
	public class ShapeTextRectangleDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly AdjustableRect textBoxRectangle;
		public ShapeTextRectangleDestination(SpreadsheetMLBaseImporter importer, AdjustableRect textBoxRectangle)
			: base(importer) {
			this.textBoxRectangle = textBoxRectangle;
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			AdjustableCoordinate l = AdjustableCoordinate.FromString(Importer.ReadAttribute(reader, "l"));
			AdjustableCoordinate r = AdjustableCoordinate.FromString(Importer.ReadAttribute(reader, "r"));
			AdjustableCoordinate t = AdjustableCoordinate.FromString(Importer.ReadAttribute(reader, "t"));
			AdjustableCoordinate b = AdjustableCoordinate.FromString(Importer.ReadAttribute(reader, "b"));
			if(l == null || r == null || t == null || b == null) {
				Importer.ThrowInvalidFile("Shape text rectangle invalid");
			}
			textBoxRectangle.Bottom = b;
			textBoxRectangle.Top = t;
			textBoxRectangle.Left = l;
			textBoxRectangle.Right = r;
		}
		#endregion
	}
	#endregion
}
