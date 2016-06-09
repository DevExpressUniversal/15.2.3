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
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal void GenerateShape3DContent(Shape3DProperties shape3d) {
			if (shape3d.IsDefault)
				return;
			WriteStartElement("sp3d", DrawingMLNamespace);
			try {
				if (shape3d.ShapeDepth != Shape3DProperties.DefaultShapeDepth)
					WriteDrawingCoordinate("z", shape3d.ShapeDepth);
				if (shape3d.ExtrusionHeight != Shape3DProperties.DefaultExtrusionHeight)
					WriteDrawingCoordinate("extrusionH", shape3d.ExtrusionHeight);
				if (shape3d.ContourWidth != Shape3DProperties.DefaultContourWidth)
					WriteDrawingCoordinate("contourW", shape3d.ContourWidth);
				WriteEnumValue("prstMaterial", shape3d.PresetMaterial, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.PresetMaterialTypeTable, Shape3DProperties.DefaultPresetMaterialType);
				GenerateShapeBevel3DPropertiesContent(shape3d.TopBevel, "bevelT");
				GenerateShapeBevel3DPropertiesContent(shape3d.BottomBevel, "bevelB");
				GenerateShapeColor3DPropertiesContent(shape3d.ExtrusionColor, "extrusionClr");
				GenerateShapeColor3DPropertiesContent(shape3d.ContourColor, "contourClr");
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateShapeBevel3DPropertiesContent(ShapeBevel3DProperties bevel, string tagName) {
			if (bevel.IsDefault)
				return;
			WriteStartElement(tagName, DrawingMLNamespace);
			try {
				if (bevel.Width != ShapeBevel3DProperties.DefaultCoordinate)
					WriteDrawingCoordinate("w", bevel.Width);
				if (bevel.Heigth != ShapeBevel3DProperties.DefaultCoordinate)
					WriteDrawingCoordinate("h", bevel.Heigth);
				WriteEnumValue("prst", bevel.PresetType, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.PresetBevelTypeTable, ShapeBevel3DProperties.DefaultPresetType);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateShapeColor3DPropertiesContent(DrawingColor color, string tagName) {
			if (color.IsEmpty)
				return;
			WriteStartElement(tagName, DrawingMLNamespace);
			try {
				GenerateDrawingColorContent(color);
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
