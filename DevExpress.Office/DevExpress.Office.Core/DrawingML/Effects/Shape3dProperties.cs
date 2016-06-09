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
using DevExpress.Office.History;
using DevExpress.Utils;
namespace DevExpress.Office.Drawing {
	#region PresetMaterialType
	public enum PresetMaterialType {
		None = 0,
		LegacyMatte = 1,
		LegacyPlastic = 2,
		LegacyMetal = 3,
		LegacyWireframe = 4,
		Matte = 5,
		Plastic = 6,
		Metal = 7,
		WarmMatte = 8,
		TranslucentPowder = 9,
		Powder = 10,
		DarkEdge = 11,
		SoftEdge = 12,
		Clear = 13,
		Flat = 14,
		SoftMetal = 15
	}
	#endregion
	#region PresetBevelType
	public enum PresetBevelType {
		None = 0,
		RelaxedInset = 1,
		Circle = 2,
		Slope = 3,
		Cross = 4,
		Angle = 5,
		SoftRound = 6,
		Convex = 7,
		CoolSlant = 8,
		Divot = 9,
		Riblet = 10,
		HardEdge = 11,
		ArtDeco = 12
	}
	#endregion
	#region Shape3DPropertiesBase (abstract class)
	public abstract class Shape3DPropertiesBase<TPreset> {
		readonly IDocumentModel documentModel;
		long[] coordinates;
		TPreset preset;
		protected Shape3DPropertiesBase(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			Initialize();
		}
		public IDocumentModel DocumentModel { get { return documentModel; } }
		protected long[] Coordinates { get { return coordinates; } set { coordinates = value; } }
		protected TPreset Preset { get { return preset; } set { SetPreset(value); } }
		#region SetCoordinate
		protected internal void SetCoordinateCore(int index, long value) {
			coordinates[index] = value;
		}
		protected void SetCoordinate(int index, long value) {
			if (coordinates[index] != value)
				ApplyHistoryItem(new Shape3DPropertiesCoordinateChangeHistoryItem<TPreset>(this, index, coordinates[index], value));
		}
		#endregion
		#region SetPreset
		public void SetPresetCore(TPreset value){
			preset = value;
		}
		void SetPreset(TPreset value) {
			if (!preset.Equals(value))
				ApplyHistoryItem(new Shape3DPropertiesPresetChangeHistoryItem<TPreset>(this, preset, value));
		}
		#endregion
		protected void ApplyHistoryItem(HistoryItem item) {
			documentModel.History.Add(item);
			item.Execute();
		}
		protected void CopyFromCore(Shape3DPropertiesBase<TPreset> value) {
			int length = coordinates.Length;
			for (int i = 0; i < length; i++)
				coordinates[i] = value.coordinates[i];
			this.preset = value.preset;
		}
		protected abstract void Initialize();
	}
	#endregion
	#region ShapeBevel3DProperties
	public class ShapeBevel3DProperties : Shape3DPropertiesBase<PresetBevelType>, ISupportsCopyFrom<ShapeBevel3DProperties> {
		#region Fields
		const int HeightIndex = 0;
		const int WidthIndex = 1;
		public const int DefaultCoordinate = 76200;
		public const PresetBevelType DefaultPresetType = PresetBevelType.Circle;
		#endregion
		public ShapeBevel3DProperties(IDocumentModel documentModel)
			: base(documentModel) {
		}
		#region Properties
		public long Heigth { get { return Coordinates[HeightIndex]; } set { SetCoordinate(HeightIndex, value); } }
		public long Width { get { return Coordinates[WidthIndex]; } set { SetCoordinate(WidthIndex, value); } }
		public PresetBevelType PresetType { get { return Preset; } set { Preset = value; } }
		public bool IsDefault { get { return PresetType == DefaultPresetType && Heigth == DefaultCoordinate && Width == DefaultCoordinate; } }
		#endregion
		protected override void Initialize() {
			Coordinates = new long[2] { DefaultCoordinate, DefaultCoordinate };
			SetPresetCore(DefaultPresetType);
		}
		public override bool Equals(object obj) {
			ShapeBevel3DProperties other = obj as ShapeBevel3DProperties;
			if (other == null)
				return false;
			return this.Heigth == other.Heigth && this.Width == other.Width && this.PresetType == other.PresetType;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region ISupportsCopyFrom<ShapeBevel3DProperties> Members
		public void CopyFrom(ShapeBevel3DProperties value) {
			CopyFromCore(value);
		}
		#endregion
	}
	#endregion
	#region Shape3DProperties
	public class Shape3DProperties : Shape3DPropertiesBase<PresetMaterialType>, ICloneable<Shape3DProperties>, ISupportsCopyFrom<Shape3DProperties>, IDrawingText3D {
		#region Fields
		const int ExtrusionHeightIndex = 0;
		const int ContourWidthIndex = 1;
		const int ShapeDepthIndex = 2;
		public const long DefaultExtrusionHeight = 0;
		public const long DefaultContourWidth = 0;
		public const long DefaultShapeDepth = 0;
		public const PresetMaterialType DefaultPresetMaterialType = PresetMaterialType.WarmMatte;
		readonly ShapeBevel3DProperties topBevel;
		readonly ShapeBevel3DProperties bottomBevel;
		readonly DrawingColor contourColor;
		readonly DrawingColor extrusionColor;
		#endregion
		public Shape3DProperties(IDocumentModel documentModel)
			: base(documentModel) {
			this.topBevel = new ShapeBevel3DProperties(documentModel);
			this.bottomBevel = new ShapeBevel3DProperties(documentModel);
			this.contourColor = new DrawingColor(documentModel);
			this.extrusionColor = new DrawingColor(documentModel);
		}
		protected override void Initialize() {
			Coordinates = new long[3];
			SetPresetCore(PresetMaterialType.WarmMatte);
		}
		#region Properties
		public long ExtrusionHeight { get { return Coordinates[ExtrusionHeightIndex]; } set { SetCoordinate(ExtrusionHeightIndex, value); } }
		public long ContourWidth { get { return Coordinates[ContourWidthIndex]; } set { SetCoordinate(ContourWidthIndex, value); } }
		public long ShapeDepth { get { return Coordinates[ShapeDepthIndex]; } set { SetCoordinate(ShapeDepthIndex, value); } }
		public PresetMaterialType PresetMaterial { get { return Preset; } set { Preset = value; } }
		public ShapeBevel3DProperties TopBevel { get { return topBevel; } }
		public ShapeBevel3DProperties BottomBevel { get { return bottomBevel; } }
		public DrawingColor ContourColor { get { return contourColor; } }
		public DrawingColor ExtrusionColor { get { return extrusionColor; } }
		public bool IsDefault {
			get {
				return
					ExtrusionColor.IsEmpty && ExtrusionColor.IsEmpty &&
					TopBevel.IsDefault && BottomBevel.IsDefault &&
					PresetMaterial == DefaultPresetMaterialType &&
					ExtrusionHeight == DefaultExtrusionHeight &&
					ContourWidth == DefaultContourWidth &&
					ShapeDepth == DefaultShapeDepth;
			}
		}
		#endregion
		#region IClonable<Shape3DProperties> Members
		public Shape3DProperties Clone() {
			Shape3DProperties result = new Shape3DProperties(DocumentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		public override bool Equals(object obj) {
			Shape3DProperties other = obj as Shape3DProperties;
			if (other == null)
				return false;
			return 
				topBevel.Equals(other.topBevel) && bottomBevel.Equals(other.bottomBevel) && 
				contourColor.Equals(other.contourColor) && extrusionColor.Equals(other.extrusionColor) && 
				this.ExtrusionHeight == other.ExtrusionHeight && this.ContourWidth == other.ContourWidth && 
				this.ShapeDepth == other.ShapeDepth;
		}
		public override int GetHashCode() {
			return 
				base.GetHashCode() ^ topBevel.GetHashCode() ^ bottomBevel.GetHashCode() ^ 
				contourColor.GetHashCode() ^ extrusionColor.GetHashCode();
		}
		#region ISupportsCopyFrom<Shape3DProperties> Members
		public void CopyFrom(Shape3DProperties value) {
			CopyFromCore(value);
			topBevel.CopyFrom(value.topBevel);
			bottomBevel.CopyFrom(value.bottomBevel);
			contourColor.CopyFrom(value.contourColor);
			extrusionColor.CopyFrom(value.extrusionColor);
		}
		#endregion
		#region IDrawingText3D Members
		DrawingText3DType IDrawingText3D.Type { get { return DrawingText3DType.Shape3D; } }
		IDrawingText3D IDrawingText3D.CloneTo(IDocumentModel documentModel) {
			Shape3DProperties result = new Shape3DProperties(documentModel);
			result.CopyFrom(this);
			return result;
		}
		public void Visit(IDrawingText3DVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
	}
	#endregion
}
