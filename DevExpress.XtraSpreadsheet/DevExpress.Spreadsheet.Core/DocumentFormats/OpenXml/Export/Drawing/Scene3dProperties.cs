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
using DevExpress.Office.DrawingML;
using DevExpress.XtraSpreadsheet.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal void GenerateScene3DContent(Scene3DProperties scene3d) {
			if (scene3d.IsDefault)
				return;
			WriteStartElement("scene3d", DrawingMLNamespace);
			try {
				GenerateCameraContent(scene3d.Camera);
				GenerateLightRigContent(scene3d.LightRig);
				GenerateBackdropContent(scene3d.BackdropPlane);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateCameraContent(IScene3DCamera camera) {
			WriteStartElement("camera", DrawingMLNamespace);
			try {
				WriteEnumValue("prst", camera.Preset, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.PresetCameraTypeTable);
				WriteIntValue("fov", camera.Fov, Scene3DPropertiesInfo.DefaultInfo.Fov);
				WriteIntValue("zoom", camera.Zoom, Scene3DPropertiesInfo.DefaultInfo.Zoom);
				if (camera.HasRotation)
					GenerateRotationContent(camera.Lat, camera.Lon, camera.Rev);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateLightRigContent(IScene3DLightRig lightRig) {
			WriteStartElement("lightRig", DrawingMLNamespace);
			try {
				WriteEnumValue("rig", lightRig.Preset, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.LightRigPresetTable);
				WriteEnumValue("dir", lightRig.Direction, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.LightRigDirectionTable);
				if (lightRig.HasRotation)
					GenerateRotationContent(lightRig.Lat, lightRig.Lon, lightRig.Rev);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateRotationContent(int lat, int lon, int rev) {
			WriteStartElement("rot", DrawingMLNamespace);
			try {
				WriteIntValue("lat", lat);
				WriteIntValue("lon", lon);
				WriteIntValue("rev", rev);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateBackdropContent(BackdropPlane backdropPlane) {
			if (backdropPlane.IsDefault)
				return;
			WriteStartElement("backdrop", DrawingMLNamespace);
			try {
				GenerateVectorContent("anchor", new string[] { "x", "y", "z" }, backdropPlane.AnchorPoint);
				string[] attrNames = new string[] { "dx", "dy", "dz" };
				GenerateVectorContent("norm", attrNames, backdropPlane.NormalVector);
				GenerateVectorContent("up", attrNames, backdropPlane.UpVector);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateVectorContent(string tagName, string[] attrNames, Scene3DVector vector) {
			WriteStartElement(tagName, DrawingMLNamespace);
			try {
				WriteDrawingCoordinate(attrNames[0], vector.X);
				WriteDrawingCoordinate(attrNames[1], vector.Y);
				WriteDrawingCoordinate(attrNames[2], vector.Z);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteDrawingCoordinate(string attr, long coordinate) {
			WriteLongValue(attr, Workbook.UnitConverter.ModelUnitsToEmuL(coordinate));
		}
	}
}
