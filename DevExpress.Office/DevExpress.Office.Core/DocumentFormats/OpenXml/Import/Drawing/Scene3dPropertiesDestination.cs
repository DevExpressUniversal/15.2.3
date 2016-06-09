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
using System.Xml;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Office.DrawingML;
using DevExpress.Office.Drawing;
using DevExpress.Office.OpenXml.Export;
namespace DevExpress.Office.Import.OpenXml {
	#region Scene3DPropertiesDestination
	public class Scene3DPropertiesDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("backdrop", OnBackdropPlane);
			result.Add("camera", OnCamera);
			result.Add("lightRig", OnLightRig);
			return result;
		}
		static Scene3DPropertiesDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (Scene3DPropertiesDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Scene3DProperties scene3d;
		Scene3DPropertiesInfo info;
		bool hasCamera = false;
		bool hasLightRig = false;
		#endregion
		public Scene3DPropertiesDestination(DestinationAndXmlBasedImporter importer, Scene3DProperties scene3d)
			: base(importer) {
			Guard.ArgumentNotNull(scene3d, "scene3d");
			this.scene3d = scene3d;
			this.info = scene3d.Info.Clone();
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnBackdropPlane(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new Scene3DBackdropPlaneDestination(importer, GetThis(importer).scene3d);
		}
		static Destination OnCamera(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			Scene3DPropertiesDestination destination = GetThis(importer);
			destination.hasCamera = true;
			return new Scene3DCameraDestination(importer, destination.scene3d.DrawingCache, destination.scene3d, destination.info);
		}
		static Destination OnLightRig(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			Scene3DPropertiesDestination destination = GetThis(importer);
			destination.hasLightRig = true;
			return new Scene3DLightRigDestination(importer, destination.scene3d.DrawingCache, destination.scene3d, destination.info);
		}
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			if (!hasCamera || !hasLightRig)
				Importer.ThrowInvalidFile();
			scene3d.AssignInfoIndex(scene3d.DrawingCache.Scene3DPropertiesInfoCache.AddItem(info));
		}
	}
	#endregion
	#region Scene3DBackdropPlaneDestination
	public class Scene3DBackdropPlaneDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("anchor", OnAnchorPoint);
			result.Add("norm", OnNormalVector);
			result.Add("up", OnUpVector);
			return result;
		}
		#endregion
		static Scene3DBackdropPlaneDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (Scene3DBackdropPlaneDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnAnchorPoint(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			Scene3DBackdropPlaneDestination destination = GetThis(importer);
			Scene3DVector vector = new Scene3DVector(destination.scene3d.DocumentModel);
			destination.anchorPoint = vector;
			return new Scene3DAnchorPointDestination(importer, vector);
		}
		static Destination OnNormalVector(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			Scene3DBackdropPlaneDestination destination = GetThis(importer);
			Scene3DVector vector = new Scene3DVector(destination.scene3d.DocumentModel);
			destination.normalVector = vector;
			return new Scene3DVectorDestination(importer, vector);
		}
		static Destination OnUpVector(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			Scene3DBackdropPlaneDestination destination = GetThis(importer);
			Scene3DVector vector = new Scene3DVector(destination.scene3d.DocumentModel);
			destination.upVector = vector;
			return new Scene3DVectorDestination(importer, vector);
		}
		#endregion
		#endregion
		#region Fields
		readonly Scene3DProperties scene3d;
		Scene3DVector anchorPoint;
		Scene3DVector normalVector;
		Scene3DVector upVector;
		#endregion
		public Scene3DBackdropPlaneDestination(DestinationAndXmlBasedImporter importer, Scene3DProperties scene3d)
			: base(importer) {
			this.scene3d = scene3d;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			if (anchorPoint == null || normalVector == null || upVector == null)
				Importer.ThrowInvalidFile();
		}
	}
	#endregion
	#region Scene3DAnchorPointDestination
	public class Scene3DAnchorPointDestination : LeafElementDestination<DestinationAndXmlBasedImporter> {
		readonly Scene3DVector vector;
		public Scene3DAnchorPointDestination(DestinationAndXmlBasedImporter importer, Scene3DVector vector)
			: base(importer) {
			this.vector = vector;
		}
		protected void CreateVector(XmlReader reader, string xName, string yName, string zName) {
			vector.DocumentModel.BeginUpdate();
			try {
				vector.X = GetCoordinate(reader, xName);
				vector.Y = GetCoordinate(reader, yName);
				vector.Z = GetCoordinate(reader, zName);
			}
			finally {
				vector.DocumentModel.EndUpdate();
			}
		}
		long GetCoordinate(XmlReader reader, string coordinateName) {
			long defaultValue = DrawingValueConstants.MinCoordinate - 1;
			long result = Importer.GetLongValue(reader, coordinateName, defaultValue);
			if (result == defaultValue)
				Importer.ThrowInvalidFile();
			DrawingValueChecker.CheckCoordinate(result, coordinateName);
			return Importer.DocumentModel.UnitConverter.EmuToModelUnitsL(result);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CreateVector(reader, "x", "y", "z");
		}
	}
	#endregion
	#region Scene3DVectorDestination
	public class Scene3DVectorDestination : Scene3DAnchorPointDestination {
		public Scene3DVectorDestination(DestinationAndXmlBasedImporter importer, Scene3DVector vector)
			: base(importer, vector) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CreateVector(reader, "dx", "dy", "dz");
		}
	}
	#endregion
	#region Scene3DInfoDestinationBase
	public abstract class Scene3DInfoDestinationBase : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("rot", OnRotation);
			return result;
		}
		#endregion
		static Scene3DInfoDestinationBase GetThis(DestinationAndXmlBasedImporter importer) {
			return (Scene3DInfoDestinationBase)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnRotation(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			Scene3DInfoDestinationBase destination = GetThis(importer);
			destination.hasRotation = true;
			return new Scene3DRotationDestination(importer, destination.rotationInfo);
		}
		#endregion
		#endregion
		#region Fields
		readonly IDrawingCache cache;
		readonly Scene3DProperties properties;
		readonly Scene3DPropertiesInfo info;
		readonly Scene3DRotationInfo rotationInfo;
		bool hasRotation;
		#endregion
		protected Scene3DInfoDestinationBase(DestinationAndXmlBasedImporter importer, IDrawingCache cache, Scene3DProperties properties, Scene3DPropertiesInfo info)
			: base(importer) {
			this.cache = cache;
			this.properties = properties;
			this.info = info;
			this.rotationInfo = cache.Scene3DRotationInfoCache.DefaultItem.Clone();
			this.hasRotation = false;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal IDrawingCache Cache { get { return cache; } }
		protected internal Scene3DProperties Properties { get { return properties; } }
		protected internal Scene3DPropertiesInfo Info { get { return info; } }
		protected internal Scene3DRotationInfo RotationInfo { get { return rotationInfo; } }
		protected internal bool HasRotation { get { return hasRotation; } }
		#endregion
	}
	#endregion
	#region Scene3DCameraDestination
	public class Scene3DCameraDestination : Scene3DInfoDestinationBase {
		public Scene3DCameraDestination(DestinationAndXmlBasedImporter importer, IDrawingCache cache, Scene3DProperties properties, Scene3DPropertiesInfo info)
			: base(importer, cache, properties, info) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int fov = Importer.GetIntegerValue(reader, "fov", Scene3DPropertiesInfo.DefaultInfo.Fov);
			DrawingValueChecker.CheckFOVAngle(fov, "fovAngle");
			Info.Fov = fov;
			int zoom = Importer.GetIntegerValue(reader, "zoom", Scene3DPropertiesInfo.DefaultInfo.Zoom);
			if (zoom < 0)
				Importer.ThrowInvalidFile();
			Info.Zoom = zoom;
			PresetCameraType? preset = Importer.GetWpEnumOnOffNullValue(reader, "prst", OpenXmlExporterBase.PresetCameraTypeTable);
			if (!preset.HasValue)
				Importer.ThrowInvalidFile();
			Info.CameraType = preset.Value;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (HasRotation)
				Info.HasCameraRotation = true;
			Properties.AssignCameraRotationInfoIndex(Cache.Scene3DRotationInfoCache.AddItem(RotationInfo));
		}
	}
	#endregion
	#region Scene3DLightRigDestination
	public class Scene3DLightRigDestination : Scene3DInfoDestinationBase {
		public Scene3DLightRigDestination(DestinationAndXmlBasedImporter importer, IDrawingCache cache, Scene3DProperties properties, Scene3DPropertiesInfo info)
			: base(importer, cache, properties, info) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			LightRigDirection? direction = Importer.GetWpEnumOnOffNullValue(reader, "dir", OpenXmlExporterBase.LightRigDirectionTable);
			if (!direction.HasValue)
				Importer.ThrowInvalidFile();
			Info.LightRigDirection = direction.Value;
			LightRigPreset? preset = Importer.GetWpEnumOnOffNullValue(reader, "rig", OpenXmlExporterBase.LightRigPresetTable);
			if (!preset.HasValue)
				Importer.ThrowInvalidFile();
			Info.LightRigPreset = preset.Value;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (HasRotation)
				Info.HasLightRigRotation = true;
			Properties.AssignLightRigRotationInfoIndex(Cache.Scene3DRotationInfoCache.AddItem(RotationInfo));
		}
	}
	#endregion
	#region Scene3DRotationDestination
	public class Scene3DRotationDestination : LeafElementDestination<DestinationAndXmlBasedImporter> {
		Scene3DRotationInfo info;
		public Scene3DRotationDestination(DestinationAndXmlBasedImporter importer, Scene3DRotationInfo info)
			: base(importer) {
			this.info = info;
		}
		int GetAngle(XmlReader reader, string coordinateName) {
			int defaultValue = Int32.MinValue;
			int result = Importer.GetIntegerValue(reader, coordinateName, defaultValue);
			if (result == defaultValue)
				Importer.ThrowInvalidFile();
			DrawingValueChecker.CheckPositiveFixedAngle(result, coordinateName);
			return result;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			info.Latitude = GetAngle(reader, "lat");
			info.Longitude = GetAngle(reader, "lon");
			info.Revolution = GetAngle(reader, "rev");
		}
	}
	#endregion
}
