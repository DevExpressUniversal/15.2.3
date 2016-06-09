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

using System.Xml;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.OpenXml.Export;
namespace DevExpress.Office.Import.OpenXml {
	#region Shape3DPropertiesDestination
	public class Shape3DPropertiesDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("bevelT", OnTopBevel);
			result.Add("bevelB", OnBottomBevel);
			result.Add("extrusionClr", OnExtrusionColor);
			result.Add("contourClr", OnContourColor);
			return result;
		}
		static Shape3DPropertiesDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (Shape3DPropertiesDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		Shape3DProperties shape3d;
		#endregion
		public Shape3DPropertiesDestination(DestinationAndXmlBasedImporter importer, Shape3DProperties shape3d)
			: base(importer) {
			Guard.ArgumentNotNull(shape3d, "shape3d");
			this.shape3d = shape3d;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnTopBevel(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ShapeBevel3DPropertiesDestination(importer, GetThis(importer).shape3d.TopBevel);
		}
		static Destination OnBottomBevel(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ShapeBevel3DPropertiesDestination(importer, GetThis(importer).shape3d.BottomBevel);
		}
		static Destination OnExtrusionColor(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingColorDestination(importer, GetThis(importer).shape3d.ExtrusionColor);
		}
		static Destination OnContourColor(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingColorDestination(importer, GetThis(importer).shape3d.ContourColor);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.BeginUpdate();
			long shapeDepth = Importer.GetLongValue(reader, "z", 0);
			DrawingValueChecker.CheckCoordinate(shapeDepth, "shape3DCoordinate");
			shape3d.ShapeDepth = Importer.DocumentModel.UnitConverter.EmuToModelUnitsL(shapeDepth);
			shape3d.ExtrusionHeight = GetCoordinate(reader, "extrusionH");
			shape3d.ContourWidth = GetCoordinate(reader, "contourW");
			shape3d.PresetMaterial = Importer.GetWpEnumValue(reader, "prstMaterial", OpenXmlExporterBase.PresetMaterialTypeTable, PresetMaterialType.WarmMatte);
		}
		long GetCoordinate(XmlReader reader, string attributeName) {
			long result = Importer.GetLongValue(reader, attributeName, 0);
			DrawingValueChecker.CheckPositiveCoordinate(result, "shape3DCoordinate");
			return Importer.DocumentModel.UnitConverter.EmuToModelUnitsL(result);
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.DocumentModel.EndUpdate();
		}
	}
	#endregion
	#region ShapeBevel3DPropertiesDestination
	public class ShapeBevel3DPropertiesDestination : LeafElementDestination<DestinationAndXmlBasedImporter> {
		#region Fields
		ShapeBevel3DProperties bevel;
		#endregion
		public ShapeBevel3DPropertiesDestination(DestinationAndXmlBasedImporter importer, ShapeBevel3DProperties bevel)
			: base(importer) {
			this.bevel = bevel;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			bevel.Width = GetCoordinate(reader, "w");
			bevel.Heigth = GetCoordinate(reader, "h");
			bevel.PresetType = Importer.GetWpEnumValue(reader, "prst", OpenXmlExporterBase.PresetBevelTypeTable, PresetBevelType.Circle);
		}
		long GetCoordinate(XmlReader reader, string attributeName) {
			long result = Importer.GetLongValue(reader, attributeName, ShapeBevel3DProperties.DefaultCoordinate);
			DrawingValueChecker.CheckPositiveCoordinate(result, "shape3DCoordinate");
			return Importer.DocumentModel.UnitConverter.EmuToModelUnitsL(result);
		}
	}
	#endregion
}
