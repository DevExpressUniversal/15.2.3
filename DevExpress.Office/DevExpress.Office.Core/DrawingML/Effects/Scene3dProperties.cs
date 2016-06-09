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

using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.DrawingML;
using DevExpress.Office.History;
using DevExpress.Utils;
namespace DevExpress.Office.DrawingML {
	#region PresetCameraType
	public enum PresetCameraType {
		None								= 0,
		LegacyObliqueTopLeft				= 1,
		LegacyObliqueTop					= 2,
		LegacyObliqueTopRight			   = 3,
		LegacyObliqueLeft				   = 4,
		LegacyObliqueFront				  = 5,
		LegacyObliqueRight				  = 6,
		LegacyObliqueBottomLeft			 = 7,
		LegacyObliqueBottom				 = 8,
		LegacyObliqueBottomRight			= 9,
		LegacyPerspectiveTopLeft			= 10,
		LegacyPerspectiveTop				= 11,
		LegacyPerspectiveTopRight		   = 12,
		LegacyPerspectiveLeft			   = 13,
		LegacyPerspectiveFront			  = 14,
		LegacyPerspectiveRight			  = 15,
		LegacyPerspectiveBottomLeft		 = 16,
		LegacyPerspectiveBottom			 = 17,
		LegacyPerspectiveBottomRight		= 18,
		OrthographicFront				   = 19,
		IsometricTopUp					  = 20,
		IsometricTopDown					= 21,
		IsometricBottomUp				   = 22,
		IsometricBottomDown				 = 23,
		IsometricLeftUp					 = 24,
		IsometricLeftDown				   = 25,
		IsometricRightUp					= 26, 
		IsometricRightDown				  = 27,
		IsometricOffAxis1Left			   = 28,
		IsometricOffAxis1Right			  = 29,
		IsometricOffAxis1Top				= 30,
		IsometricOffAxis2Left			   = 31,
		IsometricOffAxis2Right			  = 32,
		IsometricOffAxis2Top				= 33,
		IsometricOffAxis3Left			   = 34,
		IsometricOffAxis3Right			  = 35,
		IsometricOffAxis3Bottom			 = 36,
		IsometricOffAxis4Left			   = 37,
		IsometricOffAxis4Right			  = 38,
		IsometricOffAxis4Bottom			 = 39,
		ObliqueTopLeft					  = 40,
		ObliqueTop						  = 41,
		ObliqueTopRight					 = 42,
		ObliqueLeft						 = 43,
		ObliqueRight						= 44,
		ObliqueBottomLeft				   = 45,
		ObliqueBottom					   = 46,
		ObliqueBottomRight				  = 47, 
		PerspectiveFront					= 48,
		PerspectiveLeft					 = 49,
		PerspectiveRight					= 50,
		PerspectiveAbove					= 51,
		PerspectiveBelow					= 52,
		PerspectiveAboveLeftFacing		  = 53,
		PerspectiveAboveRightFacing		 = 54,
		PerspectiveContrastingLeftFacing	= 55,
		PerspectiveContrastingRightFacing   = 56,
		PerspectiveHeroicLeftFacing		 = 57,
		PerspectiveHeroicRightFacing		= 58,
		PerspectiveHeroicExtremeLeftFacing  = 59, 
		PerspectiveHeroicExtremeRightFacing = 60,
		PerspectiveRelaxed				  = 61,
		PerspectiveRelaxedModerately		= 62
	}
	#endregion
	#region LightRigDirection
	public enum LightRigDirection {
		None		= 0,
		Bottom	  = 1,
		BottomLeft  = 2,
		BottomRight = 3,
		Left		= 4,
		Right	   = 5,
		Top		 = 6,
		TopLeft	 = 7,
		TopRight	= 8
	}
	#endregion
	#region LightRigPreset
	public enum LightRigPreset {
		None		  = 0,
		LegacyFlat1   = 1,
		LegacyFlat2   = 2,
		LegacyFlat3   = 3,
		LegacyFlat4   = 4,
		LegacyNormal1 = 5,
		LegacyNormal2 = 6,
		LegacyNormal3 = 7,
		LegacyNormal4 = 8,
		LegacyHarsh1  = 9,
		LegacyHarsh2  = 10,
		LegacyHarsh3  = 11,
		LegacyHarsh4  = 12,
		ThreePt	   = 13,
		Balanced	  = 14,
		Soft		  = 15,
		Harsh		 = 16,
		Flood		 = 17,
		Contrasting   = 18,
		Morning	   = 19,
		Sunrise	   = 20,
		Sunset		= 21,
		Chilly		= 22,
		Freezing	  = 23,
		Flat		  = 24,
		TwoPt		 = 25,
		Glow		  = 26,
		BrightRoom	= 27
	}
	#endregion
	#region Scene3DPropertiesInfo
	public class Scene3DPropertiesInfo : ICloneable<Scene3DPropertiesInfo>, ISupportsCopyFrom<Scene3DPropertiesInfo>, ISupportsSizeOf {
		readonly static Scene3DPropertiesInfo defaultInfo = new Scene3DPropertiesInfo();
		public static Scene3DPropertiesInfo DefaultInfo { get { return defaultInfo; } }
		#region Fields
		const uint MaskPresetCameraType	= 0x0000003F; 
		const uint MaskLightRigDirection   = 0x000003C0; 
		const uint MaskLightRigPreset	  = 0x00007C00; 
		const uint MaskHasCameraRotation   = 0x00008000; 
		const uint MaskHasLightRigRotation = 0x00010000; 
		uint packedValues				  = 0x00000000;
		int fov;
		int zoom = DrawingValueConstants.ThousandthOfPercentage;
		#endregion
		#region Properties
		public PresetCameraType CameraType {
			get { return (PresetCameraType)GetUIntValue(MaskPresetCameraType, 0); }
			set { SetUIntValue(MaskPresetCameraType, 0, (uint)value); }
		}
		public LightRigDirection LightRigDirection {
			get { return (LightRigDirection)GetUIntValue(MaskLightRigDirection, 6); }
			set { SetUIntValue(MaskLightRigDirection, 6, (uint)value); }
		}
		public LightRigPreset LightRigPreset {
			get { return (LightRigPreset)GetUIntValue(MaskLightRigPreset, 10); }
			set { SetUIntValue(MaskLightRigPreset, 10, (uint)value); }
		}
		public bool HasCameraRotation { get { return GetBooleanValue(MaskHasCameraRotation); } set { SetBooleanValue(MaskHasCameraRotation, value); } }
		public bool HasLightRigRotation { get { return GetBooleanValue(MaskHasLightRigRotation); } set { SetBooleanValue(MaskHasLightRigRotation, value); } }
		public int Fov { get { return fov; } set { fov = value; } }
		public int Zoom { get { return zoom; } set { zoom = value; } }
		public bool IsDefault { get { return this.Equals(defaultInfo); } }
		#endregion
		#region GetUIntValue/SetUIntValue helpers
		void SetUIntValue(uint mask, int bits, uint value) {
			packedValues &= ~mask;
			packedValues |= (value << bits) & mask;
		}
		uint GetUIntValue(uint mask, int bits) {
			return (packedValues & mask) >> bits;
		}
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		public override bool Equals(object obj) {
			Scene3DPropertiesInfo info = obj as Scene3DPropertiesInfo;
			if (info == null)
				return false;
			return
				this.packedValues == info.packedValues &&
				this.fov == info.fov &&
				this.zoom == info.zoom;
		}
		public override int GetHashCode() {
			return packedValues.GetHashCode() ^ fov ^ zoom;
		}
		#region ICloneable<Scene3DPropertiesInfo> Members
		public Scene3DPropertiesInfo Clone() {
			Scene3DPropertiesInfo result = new Scene3DPropertiesInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<Scene3DPropertiesInfo> Members
		public void CopyFrom(Scene3DPropertiesInfo value) {
			this.packedValues = value.packedValues;
			this.fov = value.fov;
			this.zoom = value.zoom;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region Scene3DRotationInfo
	public class Scene3DRotationInfo : ICloneable<Scene3DRotationInfo>, ISupportsCopyFrom<Scene3DRotationInfo>, ISupportsSizeOf {
		#region Static Members
		readonly static Scene3DRotationInfo defaultInfo = new Scene3DRotationInfo();
		public static Scene3DRotationInfo DefaultInfo { get { return defaultInfo; } }
		public static Scene3DRotationInfo CreateFromLatitude(int value, Scene3DRotationInfo oldInfo) {
			Scene3DRotationInfo result = oldInfo.Clone();
			result.Latitude = value;
			return result;
		}
		public static Scene3DRotationInfo CreateFromLongitude(int value, Scene3DRotationInfo oldInfo) {
			Scene3DRotationInfo result = oldInfo.Clone();
			result.Longitude = value;
			return result;
		}
		public static Scene3DRotationInfo CreateFromRevolution(int value, Scene3DRotationInfo oldInfo) {
			Scene3DRotationInfo result = oldInfo.Clone();
			result.Revolution = value;
			return result;
		}
		#endregion
		#region Fields
		int latitude = 0;
		int longitude = 0;
		int revolution = 0;
		#endregion
		#region Properties
		public int Latitude { get { return latitude; } set { latitude = value; } }
		public int Longitude { get { return longitude; } set { longitude = value; } }
		public int Revolution { get { return revolution; } set { revolution = value; } }
		#endregion
		public override bool Equals(object obj) {
			Scene3DRotationInfo info = obj as Scene3DRotationInfo;
			if (info == null)
				return false;
			return
				this.latitude == info.latitude &&
				this.longitude == info.longitude &&
				this.revolution == info.revolution;
		}
		public override int GetHashCode() {
			return latitude ^ longitude ^ revolution;
		}
		#region ICloneable<Scene3DRotationInfo> Members
		public Scene3DRotationInfo Clone() {
			Scene3DRotationInfo result = new Scene3DRotationInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<Scene3DRotationInfo> Members
		public void CopyFrom(Scene3DRotationInfo value) {
			this.latitude = value.latitude;
			this.longitude = value.longitude;
			this.revolution = value.revolution;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region Scene3DPropertiesInfoCache
	public class Scene3DPropertiesInfoCache : UniqueItemsCache<Scene3DPropertiesInfo> {
		public const int DefaultItemIndex = 0;
		public Scene3DPropertiesInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override Scene3DPropertiesInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new Scene3DPropertiesInfo();
		}
	}
	#endregion
	#region Scene3DRotationInfoCache
	public class Scene3DRotationInfoCache : UniqueItemsCache<Scene3DRotationInfo> {
		public const int DefaultItemIndex = 0;
		public Scene3DRotationInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override Scene3DRotationInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new Scene3DRotationInfo();
		}
	}
	#endregion
	#region Scene3DPropertiesBatchUpdateHelper
	public class Scene3DPropertiesBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		Scene3DPropertiesInfo info;
		Scene3DRotationInfo cameraRotationInfo;
		Scene3DRotationInfo lightRigRotationInfo;
		int suppressDirectNotificationsCount;
		public Scene3DPropertiesBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public Scene3DPropertiesInfo Info { get { return info; } set { info = value; } }
		public Scene3DRotationInfo CameraRotationInfo { get { return cameraRotationInfo; } set { cameraRotationInfo = value; } }
		public Scene3DRotationInfo LightRigRotationInfo { get { return lightRigRotationInfo; } set { lightRigRotationInfo = value; } }
		public bool IsDirectNotificationsEnabled { get { return suppressDirectNotificationsCount == 0; } }
		public void SuppressDirectNotifications() {
			suppressDirectNotificationsCount++;
		}
		public void ResumeDirectNotifications() {
			suppressDirectNotificationsCount--;
		}
	}
	#endregion
	#region Scene3DPropertiesBatchInitHelper
	public class Scene3DPropertiesBatchInitHelper : MultiIndexBatchUpdateHelper {
		public Scene3DPropertiesBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region Scene3DPropertiesInfoIndexAccessor
	public class Scene3DPropertiesInfoIndexAccessor : IIndexAccessor<Scene3DProperties, Scene3DPropertiesInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<Scene3DProperties, Scene3DPropertiesInfo> Members
		public int GetIndex(Scene3DProperties owner) {
			return owner.InfoIndex;
		}
		public int GetDeferredInfoIndex(Scene3DProperties owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(Scene3DProperties owner, int value) {
			owner.AssignInfoIndex(value);
		}
		public int GetInfoIndex(Scene3DProperties owner, Scene3DPropertiesInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public Scene3DPropertiesInfo GetInfo(Scene3DProperties owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(Scene3DProperties owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<Scene3DPropertiesInfo> GetInfoCache(Scene3DProperties owner) {
			return owner.DrawingCache.Scene3DPropertiesInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Scene3DProperties owner) {
			return new Scene3DPropertiesInfoIndexChangeHistoryItem(owner);
		}
		public Scene3DPropertiesInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((Scene3DPropertiesBatchUpdateHelper)helper).Info;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, Scene3DPropertiesInfo info) {
			((Scene3DPropertiesBatchUpdateHelper)helper).Info = info.Clone();
		}
		public void InitializeDeferredInfo(Scene3DProperties owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Scene3DProperties owner, Scene3DProperties from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(Scene3DProperties owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region Scene3DCameraRotationInfoIndexAccessor
	public class Scene3DCameraRotationInfoIndexAccessor : IIndexAccessor<Scene3DProperties, Scene3DRotationInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<Scene3DProperties, Scene3DRotationInfo> Members
		public int GetIndex(Scene3DProperties owner) {
			return owner.CameraRotationInfoIndex;
		}
		public int GetDeferredInfoIndex(Scene3DProperties owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(Scene3DProperties owner, int value) {
			owner.AssignCameraRotationInfoIndex(value);
		}
		public int GetInfoIndex(Scene3DProperties owner, Scene3DRotationInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public Scene3DRotationInfo GetInfo(Scene3DProperties owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(Scene3DProperties owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<Scene3DRotationInfo> GetInfoCache(Scene3DProperties owner) {
			return owner.DrawingCache.Scene3DRotationInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Scene3DProperties owner) {
			return new Scene3DCameraRotationInfoIndexChangeHistoryItem(owner);
		}
		public Scene3DRotationInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((Scene3DPropertiesBatchUpdateHelper)helper).CameraRotationInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, Scene3DRotationInfo info) {
			((Scene3DPropertiesBatchUpdateHelper)helper).CameraRotationInfo = info.Clone();
		}
		public void InitializeDeferredInfo(Scene3DProperties owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Scene3DProperties owner, Scene3DProperties from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(Scene3DProperties owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region Scene3DLightRigRotationInfoIndexAccessor
	public class Scene3DLightRigRotationInfoIndexAccessor : IIndexAccessor<Scene3DProperties, Scene3DRotationInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<Scene3DProperties, Scene3DRotationInfo> Members
		public int GetIndex(Scene3DProperties owner) {
			return owner.LightRigRotationInfoIndex;
		}
		public int GetDeferredInfoIndex(Scene3DProperties owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(Scene3DProperties owner, int value) {
			owner.AssignLightRigRotationInfoIndex(value);
		}
		public int GetInfoIndex(Scene3DProperties owner, Scene3DRotationInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public Scene3DRotationInfo GetInfo(Scene3DProperties owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(Scene3DProperties owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<Scene3DRotationInfo> GetInfoCache(Scene3DProperties owner) {
			return owner.DrawingCache.Scene3DRotationInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Scene3DProperties owner) {
			return new Scene3DLightRigRotationInfoIndexChangeHistoryItem(owner);
		}
		public Scene3DRotationInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((Scene3DPropertiesBatchUpdateHelper)helper).LightRigRotationInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, Scene3DRotationInfo info) {
			((Scene3DPropertiesBatchUpdateHelper)helper).LightRigRotationInfo = info.Clone();
		}
		public void InitializeDeferredInfo(Scene3DProperties owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Scene3DProperties owner, Scene3DProperties from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(Scene3DProperties owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region IScene3DCamera
	public interface IScene3DCamera {
		PresetCameraType Preset { get; set; }
		int Fov { get; set; }
		int Zoom { get; set; }
		int Lat { get; set; }
		int Lon { get; set; }
		int Rev { get; set; }
		bool HasRotation { get; }
	}
	#endregion
	#region IScene3DPropertiesLightRig
	public interface IScene3DLightRig {
		LightRigDirection Direction { get; set; }
		LightRigPreset Preset { get; set; }
		int Lat { get; set; }
		int Lon { get; set; }
		int Rev { get; set; }
		bool HasRotation { get; }
	}
	#endregion
	#region IScene3DProperties
	public interface IScene3DProperties : IScene3DCamera, IScene3DLightRig {
		IScene3DCamera Camera { get; }
		IScene3DLightRig LightRig { get; }
		BackdropPlane BackdropPlane { get; }
	}
	#endregion
	#region Scene3DVector
	public class Scene3DVector : ISupportsCopyFrom<Scene3DVector> {
		#region Fields
		const int Xindex = 0;
		const int Yindex = 1;
		const int Zindex = 2;
		readonly IDocumentModel documentModel;
		readonly long[] coordinates;
		#endregion
		public Scene3DVector(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.coordinates = new long[3];
		}
		#region Properties
		public IDocumentModel DocumentModel { get { return documentModel; } }
		public long X { get { return coordinates[Xindex]; } set { SetCoordinate(Xindex, value); } }
		public long Y { get { return coordinates[Yindex]; } set { SetCoordinate(Yindex, value); } }
		public long Z { get { return coordinates[Zindex]; } set { SetCoordinate(Zindex, value); } }
		public bool IsDefault { get { return X == 0 && Y == 0 && Z == 0; } }
		#endregion
		public void SetCoordinateCore(int index, long value) {
			coordinates[index] = value;
		}
		void SetCoordinate(int index, long value) {
			if (coordinates[index] == value)
				return;
			Scene3DVectorCoordinateChangeHistoryItem item = new Scene3DVectorCoordinateChangeHistoryItem(this, index, coordinates[index], value);
			documentModel.History.Add(item);
			item.Execute();
		}
		public void CopyFrom(Scene3DVector value) {
			coordinates[Xindex] = value.X;
			coordinates[Yindex] = value.Y;
			coordinates[Zindex] = value.Z;
		}
		public override bool Equals(object obj) {
			Scene3DVector value = obj as Scene3DVector;
			if (value == null)
				return false;
			return
				this.coordinates[Xindex] == value.X && this.coordinates[Yindex] == value.Y && this.coordinates[Zindex] == value.Z;
		}
		public override int GetHashCode() {
			return coordinates[Xindex].GetHashCode() ^ coordinates[Yindex].GetHashCode() ^ coordinates[Zindex].GetHashCode();
		}
	}
	#endregion
	#region BackdropPlane
	public class BackdropPlane : ISupportsCopyFrom<BackdropPlane> {
		#region Fields
		Scene3DVector normalVector;
		Scene3DVector upVector;
		Scene3DVector anchorPoint;
		#endregion
		public BackdropPlane(IDocumentModel documentModel) {
			this.normalVector = new Scene3DVector(documentModel);
			this.upVector = new Scene3DVector(documentModel);
			this.anchorPoint = new Scene3DVector(documentModel);
		}
		#region Properties
		public Scene3DVector NormalVector { get { return normalVector; } }
		public Scene3DVector UpVector { get { return upVector; } }
		public Scene3DVector AnchorPoint { get { return anchorPoint; } }
		public bool IsDefault { get { return anchorPoint.IsDefault && normalVector.IsDefault && upVector.IsDefault; } }
		#endregion
		#region ISupportsCopyFrom<BackdropPlane>
		public void CopyFrom(BackdropPlane value) {
			normalVector.CopyFrom(value.normalVector);
			upVector.CopyFrom(value.upVector);
			anchorPoint.CopyFrom(value.anchorPoint);
		}
		#endregion
		public override bool Equals(object obj) {
			BackdropPlane value = obj as BackdropPlane;
			if (value == null)
				return false;
			return
				this.normalVector.Equals(value.normalVector) && this.anchorPoint.Equals(value.anchorPoint) && this.upVector.Equals(value.upVector);
		}
		public override int GetHashCode() {
			return normalVector.GetHashCode() ^ anchorPoint.GetHashCode() ^ upVector.GetHashCode();
		}
		public void ResetToStyle() {
			NormalVector.X = 0;
			NormalVector.Y = 0;
			NormalVector.Z = 0;
			UpVector.X = 0;
			UpVector.Y = 0;
			UpVector.Z = 0;
			AnchorPoint.X = 0;
			AnchorPoint.Y = 0;
			AnchorPoint.Z = 0;
		}
	}
	#endregion 
	#region Scene3DProperties
	public class Scene3DProperties : DrawingMultiIndexObject<Scene3DProperties, DocumentModelChangeActions>, ICloneable<Scene3DProperties>, ISupportsCopyFrom<Scene3DProperties>, IScene3DProperties {
		#region Static Members
		readonly static Scene3DPropertiesInfoIndexAccessor infoIndexAccessor = new Scene3DPropertiesInfoIndexAccessor();
		readonly static Scene3DCameraRotationInfoIndexAccessor cameraRotationInfoIndexAccessor = new Scene3DCameraRotationInfoIndexAccessor();
		readonly static Scene3DLightRigRotationInfoIndexAccessor lightRigRotationInfoIndexAccessor = new Scene3DLightRigRotationInfoIndexAccessor();
		readonly static IIndexAccessorBase<Scene3DProperties, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<Scene3DProperties, DocumentModelChangeActions>[] {
			infoIndexAccessor,
			cameraRotationInfoIndexAccessor,
			lightRigRotationInfoIndexAccessor,
		};
		public static Scene3DPropertiesInfoIndexAccessor InfoIndexAccessor { get { return infoIndexAccessor; } }
		public static Scene3DCameraRotationInfoIndexAccessor CameraRotationInfoIndexAccessor { get { return cameraRotationInfoIndexAccessor; } }
		public static Scene3DLightRigRotationInfoIndexAccessor LightRigRotationInfoIndexAccessor { get { return lightRigRotationInfoIndexAccessor; } }
		#endregion
		#region Fields
		BackdropPlane backdropPlane;
		int infoIndex;
		int cameraRotationInfoIndex;
		int lightRigRotationInfoIndex;
		#endregion
		public Scene3DProperties(IDocumentModel documentModel)
			: base(documentModel) {
			this.backdropPlane = new BackdropPlane(documentModel);
		}
		#region Properties
		public int InfoIndex { get { return infoIndex; } }
		public int CameraRotationInfoIndex { get { return cameraRotationInfoIndex; } }
		public int LightRigRotationInfoIndex { get { return lightRigRotationInfoIndex; } }
		protected override IIndexAccessorBase<Scene3DProperties, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		internal new Scene3DPropertiesBatchUpdateHelper BatchUpdateHelper { get { return (Scene3DPropertiesBatchUpdateHelper)base.BatchUpdateHelper; } }
		public Scene3DPropertiesInfo Info { get { return IsUpdateLocked ? BatchUpdateHelper.Info : InfoCore; } }
		protected internal Scene3DRotationInfo CameraRotationInfo { get { return IsUpdateLocked ? BatchUpdateHelper.CameraRotationInfo : CameraRotationInfoCore; } }
		protected internal Scene3DRotationInfo LightRigRotationInfo { get { return IsUpdateLocked ? BatchUpdateHelper.LightRigRotationInfo : LightRigRotationInfoCore; } }
		Scene3DPropertiesInfo InfoCore { get { return infoIndexAccessor.GetInfo(this); } }
		Scene3DRotationInfo CameraRotationInfoCore { get { return cameraRotationInfoIndexAccessor.GetInfo(this); } }
		Scene3DRotationInfo LightRigRotationInfoCore { get { return lightRigRotationInfoIndexAccessor.GetInfo(this); } }
		public bool IsDefault { 
			get { 
				return 
					infoIndex == Scene3DPropertiesInfoCache.DefaultItemIndex &&
					cameraRotationInfoIndex == Scene3DRotationInfoCache.DefaultItemIndex &&
					lightRigRotationInfoIndex == Scene3DRotationInfoCache.DefaultItemIndex && 
					backdropPlane.IsDefault; 
			} 
		}
		#region IScene3DProperties Members
		public IScene3DCamera Camera { get { return this; } }
		public IScene3DLightRig LightRig { get { return this; } }
		public BackdropPlane BackdropPlane { get { return backdropPlane; } }
		#region IScene3DPropertiesCamera
		PresetCameraType IScene3DCamera.Preset {
			get { return Info.CameraType; }
			set {
				if (Info.CameraType == value)
					return;
				SetPropertyValue(infoIndexAccessor, SetCameraTypeCore, value);
			}
		}
		DocumentModelChangeActions SetCameraTypeCore(Scene3DPropertiesInfo info, PresetCameraType value) {
			info.CameraType = value;
			return DocumentModelChangeActions.None; 
		}
		int IScene3DCamera.Fov {
			get { return Info.Fov; }
			set {
				if (Info.Fov == value)
					return;
				SetPropertyValue(infoIndexAccessor, SetFovCore, value);
			}
		}
		DocumentModelChangeActions SetFovCore(Scene3DPropertiesInfo info, int value) {
			info.Fov = value;
			return DocumentModelChangeActions.None; 
		}
		int IScene3DCamera.Zoom {
			get { return Info.Zoom; }
			set {
				if (Info.Zoom == value)
					return;
				SetPropertyValue(infoIndexAccessor, SetZoomCore, value);
			}
		}
		DocumentModelChangeActions SetZoomCore(Scene3DPropertiesInfo info, int value) {
			info.Zoom = value;
			return DocumentModelChangeActions.None; 
		}
		int IScene3DCamera.Lat {
			get { return CameraRotationInfo.Latitude; }
			set {
				if (CameraRotationInfo.Latitude == value && Info.HasCameraRotation)
					return;
				SetRotationPropertyValue(cameraRotationInfoIndexAccessor, SetLatitudeCore, SetHasCameraRotationCore, value);
			}
		}
		DocumentModelChangeActions SetLatitudeCore(Scene3DRotationInfo info, int value) {
			info.Latitude = value;
			return DocumentModelChangeActions.None; 
		}
		int IScene3DCamera.Lon {
			get { return CameraRotationInfo.Longitude; }
			set {
				if (CameraRotationInfo.Longitude == value && Info.HasCameraRotation)
					return;
				SetRotationPropertyValue(cameraRotationInfoIndexAccessor, SetLongitudeCore, SetHasCameraRotationCore, value);
			}
		}
		DocumentModelChangeActions SetLongitudeCore(Scene3DRotationInfo info, int value) {
			info.Longitude = value;
			return DocumentModelChangeActions.None; 
		}
		int IScene3DCamera.Rev {
			get { return CameraRotationInfo.Revolution; }
			set {
				if (CameraRotationInfo.Revolution == value && Info.HasCameraRotation)
					return;
				SetRotationPropertyValue(cameraRotationInfoIndexAccessor, SetRevolutionCore, SetHasCameraRotationCore, value);
			}
		}
		DocumentModelChangeActions SetRevolutionCore(Scene3DRotationInfo info, int value) {
			info.Revolution = value;
			return DocumentModelChangeActions.None; 
		}
		bool IScene3DCamera.HasRotation { get { return Info.HasCameraRotation; } }
		DocumentModelChangeActions SetHasCameraRotationCore(Scene3DPropertiesInfo info, bool value) {
			info.HasCameraRotation = true;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IScene3DPropertiesLightRig
		LightRigDirection IScene3DLightRig.Direction {
			get { return Info.LightRigDirection; }
			set {
				if (Info.LightRigDirection == value)
					return;
				SetPropertyValue(infoIndexAccessor, SetDirectionCore, value);
			}
		}
		protected DocumentModelChangeActions SetDirectionCore(Scene3DPropertiesInfo info, LightRigDirection value) {
			info.LightRigDirection = value;
			return DocumentModelChangeActions.None; 
		}
		LightRigPreset IScene3DLightRig.Preset {
			get { return Info.LightRigPreset; }
			set {
				if (Info.LightRigPreset == value)
					return;
				SetPropertyValue(infoIndexAccessor, SetLightRigPresetCore, value);
			}
		}
		protected DocumentModelChangeActions SetLightRigPresetCore(Scene3DPropertiesInfo info, LightRigPreset value) {
			info.LightRigPreset = value;
			return DocumentModelChangeActions.None; 
		}
		int IScene3DLightRig.Lat {
			get { return LightRigRotationInfo.Latitude; }
			set {
				if (LightRigRotationInfo.Latitude == value && Info.HasLightRigRotation)
					return;
				SetRotationPropertyValue(lightRigRotationInfoIndexAccessor, SetLatitudeCore, SetHasLightRigRotationCore, value);
			}
		}
		int IScene3DLightRig.Lon {
			get { return LightRigRotationInfo.Longitude; }
			set {
				if (LightRigRotationInfo.Longitude == value && Info.HasLightRigRotation)
					return;
				SetRotationPropertyValue(lightRigRotationInfoIndexAccessor, SetLongitudeCore, SetHasLightRigRotationCore, value);
			}
		}
		int IScene3DLightRig.Rev {
			get { return LightRigRotationInfo.Revolution; }
			set {
				if (LightRigRotationInfo.Revolution == value && Info.HasLightRigRotation)
					return;
				SetRotationPropertyValue(lightRigRotationInfoIndexAccessor, SetRevolutionCore, SetHasLightRigRotationCore, value);
			}
		}
		bool IScene3DLightRig.HasRotation { get { return Info.HasLightRigRotation; } }
		DocumentModelChangeActions SetHasLightRigRotationCore(Scene3DPropertiesInfo info, bool value) {
			info.HasLightRigRotation = true;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		void SetRotationPropertyValue(IIndexAccessor<Scene3DProperties, Scene3DRotationInfo, DocumentModelChangeActions> indexHolder, SetPropertyValueDelegate<Scene3DRotationInfo, int> setter, SetPropertyValueDelegate<Scene3DPropertiesInfo, bool> hasRotationSetter, int newValue) {
			DocumentModel.BeginUpdate();
			try {
				SetPropertyValueCore(indexHolder, setter, newValue);
				SetPropertyValueCore(infoIndexAccessor, hasRotationSetter, true);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#region DrawingMultiIndexObject members
		public override Scene3DProperties GetOwner() {
			return this;
		}
		public void AssignInfoIndex(int value) {
			this.infoIndex = value;
		}
		public void AssignCameraRotationInfoIndex(int value) {
			this.cameraRotationInfoIndex = value;
		}
		public void AssignLightRigRotationInfoIndex(int value) {
			this.lightRigRotationInfoIndex = value;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new Scene3DPropertiesBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new Scene3DPropertiesBatchInitHelper(this);
		}
		#endregion
		#region IClonable<Scene3DProperties> Members
		public Scene3DProperties Clone() {
			Scene3DProperties result = new Scene3DProperties(DocumentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<Scene3DProperties> Members
		public void CopyFrom(Scene3DProperties value) {
			base.CopyFrom(value);
			backdropPlane.CopyFrom(value.backdropPlane);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			Scene3DProperties other = obj as Scene3DProperties;
			if (other == null)
				return false;
			return base.Equals(other) && backdropPlane.Equals(other.backdropPlane);
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ backdropPlane.GetHashCode();
		}
		#endregion
		public void ResetToStyle() {
			BeginUpdate();
			try {
				Info.CopyFrom(DrawingCache.Scene3DPropertiesInfoCache.DefaultItem);
				CameraRotationInfo.CopyFrom(DrawingCache.Scene3DRotationInfoCache.DefaultItem);
				LightRigRotationInfo.CopyFrom(DrawingCache.Scene3DRotationInfoCache.DefaultItem); 
			}
			finally {
				EndUpdate();
			}
			this.backdropPlane.ResetToStyle();
		}
	}
	#endregion
}
