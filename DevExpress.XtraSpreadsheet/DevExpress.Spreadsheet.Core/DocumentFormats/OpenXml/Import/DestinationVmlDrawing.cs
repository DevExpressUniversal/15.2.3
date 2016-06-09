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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using System.Globalization;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region VmlDrawingRelationDestination
	public class VmlDrawingRelationDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public VmlDrawingRelationDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		internal virtual new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string relationId = reader.GetAttribute("id", Importer.RelationsNamespace);
			string relationPath = Importer.LookupRelationTargetById(Importer.DocumentRelations, relationId, Importer.DocumentRootFolder, string.Empty);
			Importer.VmlDrawingRelations.Add(relationPath, Importer.CurrentWorksheet);
		}
	}
	#endregion
	#region VmlDrawingDestination
	public class VmlDrawingDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("shapelayout", OnShapeLayout);
			result.Add("shapetype", OnShapeType);
			result.Add("shape", OnShape);
			return result;
		}
		static VmlDrawingDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (VmlDrawingDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Worksheet sheet;
		readonly VmlDrawing drawing;
		#endregion
		public VmlDrawingDestination(SpreadsheetMLBaseImporter importer, Worksheet sheet)
			: base(importer) {
			this.sheet = sheet;
			sheet.VmlDrawing = null;
			drawing = new VmlDrawing();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public VmlDrawing Drawing { get { return drawing; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			sheet.VmlDrawing = drawing;
		}
		static Destination OnShapeLayout(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlDrawing vmlDrawing = GetThis(importer).Drawing;
			return new VmlDrawingShapeLayoutDestination(importer, vmlDrawing.ShapeLayout);
		}
		static Destination OnShapeType(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlDrawing vmlDrawing = GetThis(importer).Drawing;
			return new VmlDrawingShapeTypeDestination(importer, vmlDrawing.ShapeTypes);
		}
		static Destination OnShape(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShapeCollection shapes = GetThis(importer).Drawing.Shapes;
			return new VmlShapeDestination(importer, shapes);
		}
	}
	#endregion
	#region VmlDrawingShapeLayoutDestination
	public class VmlDrawingShapeLayoutDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("idmap", OnIdmap);
			return result;
		}
		static Dictionary<VmlExtensionHandlingBehavior, string> vmlExtensionHandlingBehaviorTable = OpenXmlExporter.VmlExtensionHandlingBehaviorTable;
		static VmlDrawingShapeLayoutDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (VmlDrawingShapeLayoutDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly VmlShapeLayout shapeLayout;
		#endregion
		public VmlDrawingShapeLayoutDestination(SpreadsheetMLBaseImporter importer, VmlShapeLayout shapeLayout)
			: base(importer) {
			this.shapeLayout = shapeLayout;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public VmlShapeLayout ShapeLayout { get { return shapeLayout; } }
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			ShapeLayout.ExtensionHandlingBehavior = Importer.GetWpEnumValue<VmlExtensionHandlingBehavior>(reader, "ext", vmlExtensionHandlingBehaviorTable, VmlExtensionHandlingBehavior.View, Importer.VmlDrawingNamespace);
		}
		static Destination OnIdmap(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShapeLayout shapeLayout = GetThis(importer).ShapeLayout;
			shapeLayout.Idmap = new VmlShapeIdMap();
			return new VmlDrawingShapeIdMapDestination(importer, shapeLayout.Idmap);
		}
	}
	#endregion
	#region VmlDrawingShapeIdMapDestination
	public class VmlDrawingShapeIdMapDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static Dictionary<VmlExtensionHandlingBehavior, string> vmlExtensionHandlingBehaviorTable = OpenXmlExporter.VmlExtensionHandlingBehaviorTable;
		#endregion
		#region Fields
		readonly VmlShapeIdMap shapeIdMap;
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		public VmlDrawingShapeIdMapDestination(SpreadsheetMLBaseImporter importer, VmlShapeIdMap shapeIdMap)
			: base(importer) {
			this.shapeIdMap = shapeIdMap;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			shapeIdMap.ExtensionHandlingBehavior = Importer.GetWpEnumValue<VmlExtensionHandlingBehavior>(reader, "ext", vmlExtensionHandlingBehaviorTable, VmlExtensionHandlingBehavior.View, Importer.VmlDrawingNamespace);
			shapeIdMap.Data = Importer.ReadAttribute(reader, "data");
		}
	}
	#endregion
	#region VmlDrawingShapeTypeDestination
	public class VmlDrawingShapeTypeDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("stroke", OnStroke);
			result.Add("path", OnPath);
			result.Add("formulas", OnFormulas);
			result.Add("lock",OnShapeProtections);
			return result;
		}
		static VmlDrawingShapeTypeDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (VmlDrawingShapeTypeDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		VmlShapeType shapeType;
		VmlSingleFormulasCollection shapeFormulas;
		readonly VmlShapeTypeCollection shapeTypes;
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public VmlShapeType ShapeType { get { return shapeType; } }
		public VmlSingleFormulasCollection ShapeFormulas { get { return shapeFormulas; } }
		#endregion
		public VmlDrawingShapeTypeDestination(SpreadsheetMLBaseImporter importer, VmlShapeTypeCollection shapeTypes)
			: base(importer) {
			this.shapeTypes = shapeTypes;
			shapeFormulas = new VmlSingleFormulasCollection();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string typeId = Importer.ReadAttribute(reader, "id");
			if (string.IsNullOrEmpty(typeId))
				shapeType = shapeTypes.DefaultItem;
			else {
				typeId = typeId.TrimStart('#');
				shapeType = shapeTypes.GetByID(typeId);
				if (shapeType == null) {
					shapeType = new VmlShapeType();
					shapeType.Id = typeId;
					shapeTypes.Add(shapeType);
				}
			}
			shapeType.Spt = Importer.GetWpSTFloatValue(reader, "spt", 0, Importer.VmlDrawingOfficeNamespace);
			shapeType.Path = Importer.ReadAttribute(reader, "path");
			PrepareCoordsize(Importer.ReadAttribute(reader, "coordsize"));
		}
		void PrepareCoordsize(string s) {
			if (string.IsNullOrEmpty(s))
				return;
			string[] coordsizes = s.Split(',');
			if (coordsizes.Length != 2)
				Importer.ThrowInvalidFile();
			int coordsizevalue;
			if (int.TryParse(coordsizes[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out coordsizevalue))
				shapeType.CoordsizeX = coordsizevalue;
			else
				Importer.ThrowInvalidFile();
			if (int.TryParse(coordsizes[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out coordsizevalue))
				shapeType.CoordsizeY = coordsizevalue;
			else
				Importer.ThrowInvalidFile();
		}
		static Destination OnStroke(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShapeType shapeType = GetThis(importer).ShapeType;
			shapeType.Stroke = new VmlLineStrokeSettings();
			return new VmlLineStrokeSettingsDestination(importer, shapeType.Stroke);
		}
		static Destination OnPath(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShapeType shapeType = GetThis(importer).ShapeType;
			shapeType.ShapePath = new VmlShapePath();
			return new VmlShapePathDestination(importer, shapeType.ShapePath);
		}
		static Destination OnFormulas(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShapeType shapeType = GetThis(importer).ShapeType;
			shapeType.Formulas = new VmlSingleFormulasCollection();
			return new VmlShapeFormulasDestination(importer, shapeType.Formulas);
		}
		static Destination OnShapeProtections(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShapeType shapeType = GetThis(importer).ShapeType;
			shapeType.ShapeProtections = new VmlShapeProtections();
			return new VmlShapeProtectionsDestination(importer, shapeType.ShapeProtections);
		}
	}
	#endregion
	#region VmlShapeFormulasDestination
	public class VmlShapeFormulasDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly VmlSingleFormulasCollection formulas;
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("f", OnFormula);
			return result;
		}
		static VmlShapeFormulasDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (VmlShapeFormulasDestination)importer.PeekDestination();
		}
		#endregion
		public VmlShapeFormulasDestination(SpreadsheetMLBaseImporter importer, VmlSingleFormulasCollection formulas)
			: base(importer) {
			this.formulas = formulas;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlSingleFormulasCollection formulas = GetThis(importer).formulas;
			VmlSingleFormula formula = new VmlSingleFormula();
			formulas.Add(formula);
			return new VmlShapeFormulaDestination(importer, formula);
		}
	}
	#endregion
	#region VmlShapeFormulaDestination
	public class VmlShapeFormulaDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly VmlSingleFormula formula;
		public VmlShapeFormulaDestination(SpreadsheetMLBaseImporter importer, VmlSingleFormula formula)
			: base(importer) {
			this.formula = formula;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			formula.Equation = Importer.ReadAttribute(reader, "eqn");
		}
	}
	#endregion
	#region VmlShapeProtectionsDestination
	public class VmlShapeProtectionsDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		static Dictionary<VmlExtensionHandlingBehavior, string> vmlExtensionHandlingBehaviorTable = OpenXmlExporter.VmlExtensionHandlingBehaviorTable;
		readonly VmlShapeProtections shapeProtections;
		public VmlShapeProtectionsDestination(SpreadsheetMLBaseImporter importer, VmlShapeProtections shapeProtections)
			: base(importer) {
			this.shapeProtections = shapeProtections;
		}
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public override void ProcessElementOpen(XmlReader reader) {
			shapeProtections.Ext = Importer.GetWpEnumValue<VmlExtensionHandlingBehavior>(reader, "ext", vmlExtensionHandlingBehaviorTable, VmlExtensionHandlingBehavior.View, Importer.VmlDrawingNamespace);
			shapeProtections.Adjusthandles = Importer.GetWpSTOnOffNullValue(reader, "adjusthandles");
			shapeProtections.Aspectratio = Importer.GetWpSTOnOffNullValue(reader, "aspectratio");
			shapeProtections.Cropping = Importer.GetWpSTOnOffNullValue(reader, "cropping");
			shapeProtections.Grouping = Importer.GetWpSTOnOffNullValue(reader, "grouping");
			shapeProtections.Position = Importer.GetWpSTOnOffNullValue(reader, "position");
			shapeProtections.Rotation = Importer.GetWpSTOnOffNullValue(reader, "rotation");
			shapeProtections.Selection = Importer.GetWpSTOnOffNullValue(reader, "selection");
			shapeProtections.Shapetype = Importer.GetWpSTOnOffNullValue(reader, "shapetype");
			shapeProtections.Text = Importer.GetWpSTOnOffNullValue(reader, "text");
			shapeProtections.Ungrouping = Importer.GetWpSTOnOffNullValue(reader, "ungrouping");
			shapeProtections.Verticies = Importer.GetWpSTOnOffNullValue(reader, "verticies");
		}
	}
	#endregion
	#region VmlLineStrokeSettingsDestination
	public class VmlLineStrokeSettingsDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static Dictionary<VmlStrokeJoinStyle, string> vmlStrokeJoinStyleBehaviorTable = OpenXmlExporter.VmlStrokeJoinStyleTable;
		static Dictionary<VmlFillType, string> vmlStrokeFillTypeTable = OpenXmlExporter.VmlFillTypeTable;
		#endregion
		#region Fields
		readonly VmlLineStrokeSettings stroke;
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		public VmlLineStrokeSettingsDestination(SpreadsheetMLBaseImporter importer, VmlLineStrokeSettings stroke)
			: base(importer) {
			this.stroke = stroke;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			stroke.Joinstyle = Importer.GetWpEnumValue<VmlStrokeJoinStyle>(reader, "joinstyle", vmlStrokeJoinStyleBehaviorTable, VmlStrokeJoinStyle.Round);
			stroke.Filltype = Importer.GetWpEnumValue<VmlFillType>(reader, "filltype", vmlStrokeFillTypeTable, VmlFillType.Solid);
			stroke.Title = Importer.ReadAttribute(reader, "title", Importer.VmlDrawingOfficeNamespace);
		}
	}
	#endregion
	#region VmlShapePathDestination
	public class VmlShapePathDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static Dictionary<VmlConnectType, string> vmlConnectTypeTable = OpenXmlExporter.VmlConnectTypeTable;
		#endregion
		#region Fields
		readonly VmlShapePath shapePath;
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		public VmlShapePathDestination(SpreadsheetMLBaseImporter importer, VmlShapePath shapePath)
			: base(importer) {
			this.shapePath = shapePath;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			shapePath.Gradientshapeok = Importer.GetOnOffValue(reader, "gradientshapeok", false);
			shapePath.Connecttype = Importer.GetWpEnumValue<VmlConnectType>(reader, "connecttype", vmlConnectTypeTable, VmlConnectType.None, Importer.VmlDrawingOfficeNamespace);
		}
	}
	#endregion
	#region VmlShapeDestination
	public class VmlShapeDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("fill", OnFill);
			result.Add("shadow", OnShadow);
			result.Add("stroke", OnStroke);
			result.Add("path", OnPath);
			result.Add("textbox", OnTextBox);
			result.Add("ClientData", OnClientData);
			result.Add("lock", OnLock);
			result.Add("imagedata", OnImageData);
			return result;
		}
		static Dictionary<VmlInsetMode, string> vmlInsetModeTable = OpenXmlExporter.VmlInsetModeTable;
		static VmlShapeDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (VmlShapeDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly VmlShape shape;
		readonly VmlShapeCollection shapes;
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public VmlShape Shape { get { return shape; } }
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		public VmlShapeDestination(SpreadsheetMLBaseImporter importer, VmlShapeCollection shapes)
			: base(importer) {
			this.shape = new VmlShape(importer.CurrentWorksheet);
			this.shapes = shapes;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			shape.BeginUpdate();
			string shapeType = Importer.ReadAttribute(reader, "type");
			if (!string.IsNullOrEmpty(shapeType)) {
				shapeType = shapeType.TrimStart('#');
				shape.Type = shapeType;
			}
			shape.Style = Importer.ReadAttribute(reader, "style");
			shape.Filled = Importer.GetWpSTOnOffNullValue(reader, "filled");
			shape.Fillcolor = Importer.GetVmlSTColorValue(reader, "fillcolor", DXColor.White);
			shape.InsetMode = Importer.GetWpEnumValue<VmlInsetMode>(reader, "insetmode", vmlInsetModeTable, VmlInsetMode.Custom, Importer.VmlDrawingOfficeNamespace);
			shape.Alt = Importer.ReadAttribute(reader, "alt");
			shape.Strokecolor = Importer.GetVmlSTColorValue(reader, "strokecolor", DXColor.Black);
			shape.Strokeweight = ConvertStrokeWeight(Importer.ReadAttribute(reader, "strokeweight"));
			shape.Stroked = Importer.GetOnOffValue(reader, "stroked", true);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			shape.EndUpdate();
			shapes.RegisterShape(shape);
		}
		static Destination OnFill(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShape shape = GetThis(importer).Shape;
			shape.Fill = new VmlShapeFillProperties();
			return new VmlShapeFillPropertiesDestination(importer, shape.Fill);
		}
		static Destination OnShadow(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShape shape = GetThis(importer).Shape;
			shape.Shadow.IsDefault = false;
			return new VmlShadowEffectDestination(importer, shape.Shadow);
		}
		static Destination OnStroke(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShape shape = GetThis(importer).Shape;
			shape.Stroke = new VmlLineStrokeSettings();
			return new VmlLineStrokeSettingsDestination(importer, shape.Stroke);
		}
		static Destination OnPath(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShape shape = GetThis(importer).Shape;
			shape.Path = new VmlShapePath();
			return new VmlShapePathDestination(importer, shape.Path);
		}
		static Destination OnTextBox(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShape shape = GetThis(importer).Shape;
			shape.Textbox = new VmlTextBox();
			return new VmlTextBoxDestination(importer, shape.Textbox);
		}
		static Destination OnClientData(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShape shape = GetThis(importer).Shape;
			shape.ClientData = new VmlClientData(shape.Sheet);
			return new VmlClientDataDestination(importer, shape.ClientData);
		}
		static Destination OnLock(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShape shape = GetThis(importer).Shape;
			return new VmlLockShapeDestination(importer, shape);
		}
		static Destination OnImageData(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlShape shape = GetThis(importer).Shape;
			shape.ImageData = new VmlShapeImageData();
			return new VmlShapeImageDataDestination(importer, shape.ImageData);
		}
		int ConvertStrokeWeight(string value) {
			if(!string.IsNullOrEmpty(value)) {
				DXVmlUnit unit = new DXVmlUnit(value);
				VmlUnitConverter converter = new VmlUnitConverter(Importer.DocumentModel.UnitConverter);
				return converter.ToModelUnits(unit);
			}
			return Importer.DocumentModel.UnitConverter.PointsToModelUnits(1);
		}
	}
	#endregion
	#region VmlShapeFillPropertiesDestination
	public class VmlShapeFillPropertiesDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static Dictionary<VmlFillType, string> vmlFillTypeTable = OpenXmlExporter.VmlFillTypeTable;
		static Dictionary<VmlFillMethod, string> vmlFillMethodTable = OpenXmlExporter.VmlFillMethodTable;
		static Dictionary<VmlImageAspect, string> vmlImageAspectTable = OpenXmlExporter.VmlImageAspectTable;
		#endregion
		#region Fields
		readonly VmlShapeFillProperties shapeFillProperties;
		#endregion
		public VmlShapeFillPropertiesDestination(SpreadsheetMLBaseImporter importer, VmlShapeFillProperties shapeFillProperties)
			: base(importer) {
			this.shapeFillProperties = shapeFillProperties;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			shapeFillProperties.Color = Importer.GetWpSTColorValue(reader, "color", DXColor.White);
			shapeFillProperties.Color2 = Importer.GetWpSTColorValue(reader, "color2", DXColor.White);
			shapeFillProperties.Opacity = Importer.GetWpSTFloatOrVulgarFractionValue(reader, "opacity", 1, 65536);
			shapeFillProperties.Opacity2 = Importer.GetWpSTFloatOrVulgarFractionValue(reader, "opacity2", 1, 65536);
			shapeFillProperties.Focus = Importer.ReadPercentageAttribute(reader, "focus");
			shapeFillProperties.Method = Importer.GetWpEnumValue<VmlFillMethod>(reader, "method", vmlFillMethodTable, VmlFillMethod.Sigma);
			shapeFillProperties.Type = Importer.GetWpEnumValue<VmlFillType>(reader, "type", vmlFillTypeTable, VmlFillType.Solid);
			shapeFillProperties.Aspect = Importer.GetWpEnumValue<VmlImageAspect>(reader, "aspect", vmlImageAspectTable, VmlImageAspect.Ignore);
			shapeFillProperties.Recolor = Importer.GetOnOffValue(reader, "recolor", false);
			shapeFillProperties.Rotate = Importer.GetOnOffValue(reader, "rotate", false);
			shapeFillProperties.Title = Importer.ReadAttribute(reader, "title", Importer.OfficeNamespace);
			string temp = Importer.ReadAttribute(reader, "size");
			if (!string.IsNullOrEmpty(temp)) {
				string[] values = SplitCoordinates(temp);
				shapeFillProperties.SizeX = Importer.GetFloatOrVulgarFractionValue(values[0], 0, 65536);
				shapeFillProperties.SizeY = Importer.GetFloatOrVulgarFractionValue(values[1], 0, 65536);
			}
			temp = Importer.ReadAttribute(reader, "origin");
			if (!string.IsNullOrEmpty(temp)) {
				string[] values = SplitCoordinates(temp);
				shapeFillProperties.OriginX = Importer.GetFloatOrVulgarFractionValue(values[0], 0, 65536);
				shapeFillProperties.OriginY = Importer.GetFloatOrVulgarFractionValue(values[1], 0, 65536);
			}
			temp = Importer.ReadAttribute(reader, "position");
			if (!string.IsNullOrEmpty(temp)) {
				string[] values = SplitCoordinates(temp);
				shapeFillProperties.PositionX = Importer.GetFloatOrVulgarFractionValue(values[0], 0, 65536);
				shapeFillProperties.PositionY = Importer.GetFloatOrVulgarFractionValue(values[1], 0, 65536);
			}
		}
		string[] SplitCoordinates(string temp) {
			string[] values = temp.Split(',');
			if (values.Length != 2)
				Importer.ThrowInvalidFile();
			return values;
		}
	}
	#endregion
	#region VmlShadowEffectDestination
	public class VmlShadowEffectDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly VmlShadowEffect shadowEffect;
		#endregion
		public VmlShadowEffectDestination(SpreadsheetMLBaseImporter importer, VmlShadowEffect shadowEffect)
			: base(importer) {
			this.shadowEffect = shadowEffect;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			shadowEffect.Color = Importer.GetWpSTColorValue(reader, "color", DXColor.Gray);
			shadowEffect.Obscured = Importer.GetOnOffValue(reader, "obscured", false);
			shadowEffect.On = Importer.GetOnOffValue(reader, "on", true);
		}
	}
	#endregion
	#region VmlLockShapeDestination
	public class VmlLockShapeDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly VmlShape shape;
		#endregion
		public VmlLockShapeDestination(SpreadsheetMLBaseImporter importer, VmlShape shape) : base(importer) {
			this.shape = shape;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			shape.LockAspectRatio = Importer.GetOnOffValue(reader, "aspectratio", false);
		}
	}
	#endregion
	#region VmlTextBoxDestination
	public class VmlTextBoxDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("div", OnDiv);
			return result;
		}
		static VmlTextBoxDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (VmlTextBoxDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly VmlTextBox textBox;
		#endregion
		public VmlTextBoxDestination(SpreadsheetMLBaseImporter importer, VmlTextBox textBox)
			: base(importer) {
			this.textBox = textBox;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			textBox.Style = Importer.ReadAttribute(reader, "style");
			textBox.Inset = VmlInsetHelper.Parse(Importer.ReadAttribute(reader, "inset"), Importer.DocumentModel);
		}
		static Destination OnDiv(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlTextBox textBox = GetThis(importer).textBox;
			XmlReader subReader = reader.ReadSubtree();
			subReader.ReadToFollowing("div");
			textBox.HtmlContent = subReader.ReadOuterXml();
			return null;
		}
	}
	#endregion
	#region VmlClientDataDestination
	public class VmlClientDataDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("Anchor", OnAnchor);
			result.Add("AutoFill", OnAutoFill);
			result.Add("Row", OnRow);
			result.Add("Column", OnColumn);
			result.Add("Visible", OnVisible);
			result.Add("Locked", OnLocked);
			result.Add("LockText", OnLockText);
			result.Add("MoveWithCells", OnMoveWithCells);
			result.Add("SizeWithCells", OnSizeWithCells);
			result.Add("TextHAlign", OnTextHAlign);
			result.Add("TextVAlign", OnTextVAlign);
			result.Add("FmlaPict", OnCameraSourceRange);
			result.Add("CF", OnClipboardFormat);
			result.Add("Camera", OnCameraTool);
			return result;
		}
		static VmlClientDataDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (VmlClientDataDestination)importer.PeekDestination();
		}
		static Dictionary<VmlObjectType, string> vmlObjectTypeTable = OpenXmlExporter.VmlObjectTypeTable;
		#endregion
		#region Fields
		readonly VmlClientData clientData;
		#endregion
		public VmlClientDataDestination(SpreadsheetMLBaseImporter importer, VmlClientData clientData)
			: base(importer) {
			this.clientData = clientData;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public VmlClientData ClientData { get { return clientData; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			string str = Importer.ReadAttribute(reader, "ObjectType");
			if (string.IsNullOrEmpty(str))
				Importer.ThrowInvalidFile();
			clientData.ObjectType = Importer.GetWpEnumValueCore<VmlObjectType>(str, vmlObjectTypeTable, VmlObjectType.Button);
		}
		static Destination OnAnchor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			return new VmlClientDataAnchorDestination(importer, clientData);
		}
		static Destination OnAutoFill(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			return new OnOffValueTagDestination(importer,
				delegate(bool value) { clientData.AutoFill = value; return true; },
				true);
		}
		static Destination OnRow(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			return new IntegerValueTagDestination(importer, delegate(int value) { clientData.Row = value; return true; });
		}
		static Destination OnColumn(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			return new IntegerValueTagDestination(importer, delegate(int value) { clientData.Column = value; return true; });
		}
		static Destination OnVisible(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			clientData.Visible = true;
			return new OnOffValueTagDestination(importer,
				delegate(bool value) { clientData.Visible = value; return true; },
				false);
		}
		static Destination OnLocked(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			return new OnOffValueTagDestination(importer,
				delegate(bool value) { clientData.Locked = value; return true; },
				true);
		}
		static Destination OnLockText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			return new OnOffValueTagDestination(importer,
				delegate(bool value) { clientData.LockText = value; return true; },
				true);
		}
		static Destination OnMoveWithCells(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			clientData.MoveWithCells = false;
			return new OnOffValueTagDestination(importer,
				delegate(bool value) { clientData.MoveWithCells = !value; return true; },
				true);
		}
		static Destination OnSizeWithCells(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			clientData.SizeWithCells = false;
			return new OnOffValueTagDestination(importer,
				delegate(bool value) { clientData.SizeWithCells = !value; return true; },
				true);
		}
		static Destination OnTextHAlign(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			return new VmlClientDataTextHAlignDestination(importer, clientData);
		}
		static Destination OnTextVAlign(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			return new VmlClientDataTextVAlignDestination(importer, clientData);
		}
		static Destination OnCameraSourceRange(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			return new VmlClientDataCameraSourceRangeDestination(importer, clientData);
		}
		static Destination OnClipboardFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			return new VmlClientDataClipboardFormatDestination(importer, clientData);
		}
		static Destination OnCameraTool(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VmlClientData clientData = GetThis(importer).ClientData;
			clientData.CameraTool = true;
			return new OnOffValueTagDestination(importer,
				delegate(bool value) { clientData.CameraTool = value; return true; },
				true);
		}
	}
	#endregion
	#region VmlShapeImageDataDestination
	public class VmlShapeImageDataDestination: LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly VmlShapeImageData imageData;
		public VmlShapeImageDataDestination(SpreadsheetMLBaseImporter importer, VmlShapeImageData imageData)
			: base(importer) {
				this.imageData = imageData;
		}
		internal virtual new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public override void ProcessElementOpen(XmlReader reader) {
			imageData.Althref = Importer.ReadAttribute(reader, "althref");
			imageData.Bilevel = Importer.GetWpSTOnOffNullValue(reader, "bilevel");
			imageData.Blacklevel = Importer.ReadAttribute(reader, "blacklevel");
			imageData.Chromakey = Importer.GetVmlSTColorValue(reader, "chromakey", Color.Empty);
			imageData.Cropbottom = Importer.ReadAttribute(reader, "cropbottom");
			imageData.Cropleft = Importer.ReadAttribute(reader, "cropleft");
			imageData.Cropright = Importer.ReadAttribute(reader, "cropright");
			imageData.Croptop = Importer.ReadAttribute(reader, "croptop");
			imageData.Detectmouseclick = Importer.GetWpSTOnOffNullValue(reader, "detectmouseclick");
			imageData.Embosscolor = Importer.GetVmlSTColorValue(reader, "embosscolor", Color.Empty);
			imageData.Gain = Importer.ReadAttribute(reader, "gain");
			imageData.Gamma = Importer.ReadAttribute(reader, "gamma");
			imageData.Grayscale = Importer.GetWpSTOnOffNullValue(reader, "grayscale");
			imageData.Href = Importer.ReadAttribute(reader, "href", OpenXmlExporter.VmlDrawingOfficeNamespace);
			imageData.Id = Importer.ReadAttribute(reader, "id");
			string movieString = Importer.ReadAttribute(reader, "movie", OpenXmlExporter.VmlDrawingOfficeNamespace);
			if (!String.IsNullOrEmpty(movieString))
				imageData.Movie = Importer.GetWpSTFloatValue(movieString, NumberStyles.Float, 0);
			string oleidString = Importer.ReadAttribute(reader, "oleid", OpenXmlExporter.VmlDrawingOfficeNamespace);
			if (!String.IsNullOrEmpty(oleidString))
				imageData.Oleid = Importer.GetWpSTFloatValue(oleidString, NumberStyles.Float, 0);
			imageData.Recolortarget = Importer.GetVmlSTColorValue(reader, "recolortarget", Color.Empty);
			imageData.Src = Importer.ReadAttribute(reader, "src");
			imageData.Title = Importer.ReadAttribute(reader, "title", OpenXmlExporter.VmlDrawingOfficeNamespace);
			string relationId = Importer.ReadAttribute(reader, "relid", OpenXmlExporter.VmlDrawingOfficeNamespace);
			if(!String.IsNullOrEmpty(relationId))
				imageData.Image = Importer.LookupImageByRelationId(Importer.DocumentModel, relationId, Importer.DocumentRootFolder);
		}
	}
	#endregion
	#region VmlClientDataTextHAlignDestination
	public class VmlClientDataTextHAlignDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly VmlClientData clientData;
		static Dictionary<string, XlHorizontalAlignment> table = CreateTextHAlignTable();
		static Dictionary<string, XlHorizontalAlignment> CreateTextHAlignTable() {
			Dictionary<string, XlHorizontalAlignment> result = new Dictionary<string, XlHorizontalAlignment>();
			result.Add("Left", XlHorizontalAlignment.Left);
			result.Add("Justify", XlHorizontalAlignment.Justify);
			result.Add("Center", XlHorizontalAlignment.Center);
			result.Add("Right", XlHorizontalAlignment.Right);
			result.Add("Distributed", XlHorizontalAlignment.Distributed);
			return result;
		}
		#endregion
		public VmlClientDataTextHAlignDestination(SpreadsheetMLBaseImporter importer, VmlClientData clientData)
			: base(importer) {
			this.clientData = clientData;
		}
		public override bool ProcessText(XmlReader reader) {
			clientData.TextHAlign = Importer.GetWpEnumValueCore<XlHorizontalAlignment>(reader.Value, table, XlHorizontalAlignment.Left);
			return true;
		}
	}
	#endregion
	#region VmlClientDataTextVAlignDestination
	public class VmlClientDataTextVAlignDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly VmlClientData clientData;
		static Dictionary<string, XlVerticalAlignment> table = CreateTextVAlignTable();
		static Dictionary<string, XlVerticalAlignment> CreateTextVAlignTable() {
			Dictionary<string, XlVerticalAlignment> result = new Dictionary<string, XlVerticalAlignment>();
			result.Add("Top", XlVerticalAlignment.Top);
			result.Add("Justify", XlVerticalAlignment.Justify);
			result.Add("Center", XlVerticalAlignment.Center);
			result.Add("Bottom", XlVerticalAlignment.Bottom);
			result.Add("Distributed", XlVerticalAlignment.Distributed);
			return result;
		}
		#endregion
		public VmlClientDataTextVAlignDestination(SpreadsheetMLBaseImporter importer, VmlClientData clientData)
			: base(importer) {
			this.clientData = clientData;
		}
		public override bool ProcessText(XmlReader reader) {
			clientData.TextVAlign = Importer.GetWpEnumValueCore<XlVerticalAlignment>(reader.Value, table, XlVerticalAlignment.Top);
			return true;
		}
	}
	#endregion
	#region VmlClientDataAnchorDestination
	public class VmlClientDataAnchorDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly VmlClientData clientData;
		#endregion
		public VmlClientDataAnchorDestination(SpreadsheetMLBaseImporter importer, VmlClientData clientData)
			: base(importer) {
			this.clientData = clientData;
		}
		public override bool ProcessText(XmlReader reader) {
			clientData.Anchor = VmlAnchorData.FromString(reader.Value, clientData.Worksheet);
			return true;
		}
	}
	#endregion
	#region VmlClientDataRowDestination
	public class VmlClientDataRowDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly VmlClientData clientData;
		#endregion
		public VmlClientDataRowDestination(SpreadsheetMLBaseImporter importer, VmlClientData clientData)
			: base(importer) {
			this.clientData = clientData;
		}
		public override bool ProcessText(XmlReader reader) {
			clientData.Row = Importer.GetIntegerValue(reader.Value, NumberStyles.Integer, -1);
			if (clientData.Row < 0)
				Importer.ThrowInvalidFile();
			return true;
		}
	}
	#endregion
	#region VmlClientDataColumnDestination
	public class VmlClientDataColumnDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly VmlClientData clientData;
		#endregion
		public VmlClientDataColumnDestination(SpreadsheetMLBaseImporter importer, VmlClientData clientData)
			: base(importer) {
			this.clientData = clientData;
		}
		public override bool ProcessText(XmlReader reader) {
			clientData.Column = Importer.GetIntegerValue(reader.Value, NumberStyles.Integer, -1);
			if (clientData.Column < 0)
				Importer.ThrowInvalidFile();
			return true;
		}
	}
	#endregion
	#region VmlClientDataCameraSourceRangeDestination
	public class VmlClientDataCameraSourceRangeDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly VmlClientData clientData;
		public VmlClientDataCameraSourceRangeDestination(SpreadsheetMLBaseImporter importer, VmlClientData clientData)
			: base(importer) {
			this.clientData = clientData;
		}
		public override bool ProcessText(XmlReader reader) {
			string reference = reader.Value;
			if (!String.IsNullOrEmpty(reference))
				clientData.CameraSourceReference = reference;
			return true;
		}
	}
	#endregion
	#region VmlClientDataClipboardFormatDestination
	public class VmlClientDataClipboardFormatDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static Dictionary<string, ClipboardFormat> table = CreateClipboardFormatTable();
		static Dictionary<string, ClipboardFormat> CreateClipboardFormatTable() {
			Dictionary<string, ClipboardFormat> result = new Dictionary<string, ClipboardFormat>();
			result.Add("Bitmap", ClipboardFormat.Bitmap);
			result.Add("Pict", ClipboardFormat.Pict);
			result.Add("PictOld", ClipboardFormat.PictOld);
			result.Add("PictPrint", ClipboardFormat.PictPrint);
			result.Add("PictScreen", ClipboardFormat.PictScreen);
			return result;
		}
		#endregion
		readonly VmlClientData clientData;
		public VmlClientDataClipboardFormatDestination(SpreadsheetMLBaseImporter importer, VmlClientData clientData)
			: base(importer) {
			this.clientData = clientData;
		}
		public override bool ProcessText(XmlReader reader) {
			clientData.ClipboardFormat = Importer.GetWpEnumValueCore<ClipboardFormat>(reader.Value, table, ClipboardFormat.Empty);
			return true;
		}
	}
	#endregion
}
