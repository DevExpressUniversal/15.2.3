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

using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Win.Base;
namespace DevExpress.XtraGauges.Win.Gauges.Circular {
	[TypeConverter(typeof(ScaleComponentWrapperTypeConverter))]
	public abstract class CircularGaugeScaleComponentWrapper : BasePropertyGridObjectWrapper { }
	[TypeConverter(typeof(ArcLogarithmicScaleObjectTypeConverter))]
	public class ArcScaleComponentWrapper : BasePropertyGridObjectWrapper, ILogarithmicBase {
		protected ArcScaleComponent Component {
			get { return WrappedObject as ArcScaleComponent; }
		}		
		bool ShouldSerializeAppearanceScale() { return Component.AppearanceScale.ShouldSerialize(); }
		void ResetAppearanceScale() { Component.AppearanceScale.Reset(); }
		[Category("Appearance")]
		public BaseScaleAppearance AppearanceScale {
			get { return Component.AppearanceScale; }
		}
		bool ShouldSerializeAppearanceMinorTickmark() { return Component.AppearanceMinorTickmark.ShouldSerialize(); }
		void ResetAppearanceMinorTickmark() { Component.AppearanceMinorTickmark.Reset(); }
		[Category("Appearance")]
		public BaseShapeAppearance AppearanceMinorTickmark {
			get { return Component.AppearanceMinorTickmark; }
		}
		bool ShouldSerializeAppearanceMajorTickmark() { return Component.AppearanceMajorTickmark.ShouldSerialize(); }
		void ResetAppearanceMajorTickmark() { Component.AppearanceMajorTickmark.Reset(); }
		[Category("Appearance")]
		public BaseShapeAppearance AppearanceMajorTickmark {
			get { return Component.AppearanceMajorTickmark; }
		}
		bool ShouldSerializeAppearanceTickmarkTextBackground() { return Component.AppearanceTickmarkTextBackground.ShouldSerialize(); }
		void ResetAppearanceTickmarkTextBackground() { Component.AppearanceTickmarkTextBackground.Reset(); }
		[Category("Appearance")]
		public BaseShapeAppearance AppearanceTickmarkTextBackground {
			get { return Component.AppearanceTickmarkTextBackground; }
		}
		bool ShouldSerializeAppearanceTickmarkText() { return Component.AppearanceTickmarkText.ShouldSerialize(); }
		void ResetAppearanceTickmarkText() { Component.AppearanceTickmarkText.Reset(); }
		[Category("Appearance")]
		public BaseTextAppearance AppearanceTickmarkText {
			get { return Component.AppearanceTickmarkText; }
		}
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		[Category("Appearance"), DefaultValue(true)]
		public bool UseColorScheme {
			get { return Component.UseColorScheme; }
			set { Component.UseColorScheme = value; }
		}
		[Category("Scale")]
		public LabelCollection Labels { get { return Component.Labels; } }
		[Category("Scale"), DefaultValue(false),
		RefreshProperties(RefreshProperties.All)]
		public bool Logarithmic { get { return Component.Logarithmic; } set { Component.Logarithmic = value; } }
		[Category("Scale"), DefaultValue(LogarithmicBase.Binary),
		RefreshProperties(RefreshProperties.All)]		
		public LogarithmicBase LogarithmicBase { get { return Component.LogarithmicBase; } set { Component.LogarithmicBase = value; }
 }
		[Category("Scale"), DefaultValue(2)]
		public float CustomLogarithmicBase { get { return Component.CustomLogarithmicBase; } set { Component.CustomLogarithmicBase = value; } }
		[Category("Scale")]
		public RangeCollection Ranges { get { return Component.Ranges; } }
		[Category("Geometry")]
		public PointF2D Center { get { return Component.Center; } set { Component.Center = value; } }
		[Category("Geometry")]
		[DefaultValue(360f)]
		public float EndAngle { get { return Component.EndAngle; } set { Component.EndAngle = value; } }
		[Category("Geometry"), DefaultValue(0f)]
		public float StartAngle { get { return Component.StartAngle; } set { Component.StartAngle = value; } }
		[Category("Geometry"), DefaultValue(100f)]
		public float RadiusX { get { return Component.RadiusX; } set { Component.RadiusX = value; } }
		[Category("Geometry"), DefaultValue(100f)]
		public float RadiusY { get { return Component.RadiusY; } set { Component.RadiusY = value; } }
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		[Category("Scale"), DefaultValue(0f)]
		public float Value { get { return Component.Value; } set { Component.Value = value; } }
		[Category("Scale"), DefaultValue(1f)]
		public float MaxValue { get { return Component.MaxValue; } set { Component.MaxValue = value; } }
		[Category("Scale"), DefaultValue(0f)]
		public float MinValue { get { return Component.MinValue; } set { Component.MinValue = value; } }
		[Category("AutoRescaling"), DefaultValue(false)]
		public bool AutoRescaling { get { return Component.AutoRescaling; } set { Component.AutoRescaling = value; } }
		[Category("AutoRescaling"), DefaultValue(0.05f)]
		public float RescalingThresholdMin { get { return Component.RescalingThresholdMin; } set { Component.RescalingThresholdMin = value; } }
		[Category("AutoRescaling"), DefaultValue(0.05f)]
		public float RescalingThresholdMax { get { return Component.RescalingThresholdMax; } set { Component.RescalingThresholdMax = value; } }
		[Category("AutoRescaling"), DefaultValue(false)]
		public bool RescalingBestValues { get { return Component.RescalingBestValues; } set { Component.RescalingBestValues = value; } }
		[Category("Tickmarks")]
		public IMinorTickmark MinorTickmark { get { return Component.MinorTickmark; } }
		[Category("Tickmarks")]
		public IMajorTickmark MajorTickmark { get { return Component.MajorTickmark; } }
		[Category("Tickmarks"), DefaultValue(1)]
		public int MinorTickCount { get { return Component.MinorTickCount; } set { Component.MinorTickCount = value; } }
		[DefaultValue(11), Category("Tickmarks")]
		public int MajorTickCount { get { return Component.MajorTickCount; } set { Component.MajorTickCount = value; } }
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class ArcScaleBackgroundLayerComponentWrapper : CircularGaugeScaleComponentWrapper {
		protected ArcScaleBackgroundLayerComponent Component {
			get { return WrappedObject as ArcScaleBackgroundLayerComponent; }
		}
		[Category("Scale")]
		public IArcScale ArcScale { get { return Component.ArcScale; } set { Component.ArcScale = value; } }
		[Category("Geometry")]
		public PointF2D ScaleCenterPos { get { return Component.ScaleCenterPos; } set { Component.ScaleCenterPos = value; } }
		[Category("Geometry")]
		public SizeF Size { get { return Component.Size; } set { Component.Size = value; } }
		[Category("Appearance")]
		[DefaultValue(BackgroundLayerShapeType.CircularFull_Default)]
		public BackgroundLayerShapeType ShapeType { get { return Component.ShapeType; } set { Component.ShapeType = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		internal bool ShouldSerializeSize() {
			return Size != new SizeF(250, 250);
		}
		internal bool ShouldSerializeScaleCenterPos() {
			return ScaleCenterPos != new PointF2D(0.5f, 0.5f);
		}
		internal void ResetScaleCenterPos() {
			ScaleCenterPos = new PointF2D(0.5f, 0.5f);
		}
		internal void ResetSize() {
			Size = new SizeF(250, 250);
		}
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class ArcScaleEffectLayerComponentWrapper : CircularGaugeScaleComponentWrapper {
		protected ArcScaleEffectLayerComponent Component {
			get { return WrappedObject as ArcScaleEffectLayerComponent; }
		}
		[Category("Scale")]
		public IArcScale ArcScale { get { return Component.ArcScale; } set { Component.ArcScale = value; } }
		[Category("Geometry")]
		public PointF2D ScaleCenterPos { get { return Component.ScaleCenterPos; } set { Component.ScaleCenterPos = value; } }
		[Category("Geometry")]
		public SizeF Size { get { return Component.Size; } set { Component.Size = value; } }
		[Category("Appearance")]
		[DefaultValue(EffectLayerShapeType.Circular_Default)]
		public EffectLayerShapeType ShapeType { get { return Component.ShapeType; } set { Component.ShapeType = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		internal bool ShouldSerializeSize() {
			return Size != new SizeF(200, 100);
		}
		internal bool ShouldSerializeScaleCenterPos() {
			return ScaleCenterPos != new PointF2D(0.5f, 1.1f);
		}
		internal void ResetScaleCenterPos() {
			ScaleCenterPos = new PointF2D(0.5f, 1.1f);
		}
		internal void ResetSize() {
			Size = new SizeF(200, 100);
		}
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class ArcScaleNeedleComponentWrapper : CircularGaugeScaleComponentWrapper {
		protected ArcScaleNeedleComponent Component {
			get { return WrappedObject as ArcScaleNeedleComponent; }
		}
		[Category("Value"), DefaultValue(null)]
		public float? Value { get { return Component.Value; } set { Component.Value = value; } }
		[Category("Scale")]
		public IArcScale ArcScale { get { return Component.ArcScale; } set { Component.ArcScale = value; } }
		[Category("Geometry"), DefaultValue(0f)]
		public float StartOffset { get { return Component.StartOffset; } set { Component.StartOffset = value; } }
		[Category("Geometry"), DefaultValue(0f)]
		public float EndOffset { get { return Component.EndOffset; } set { Component.EndOffset = value; } }
		[Category("Appearance")]
		[DefaultValue(NeedleShapeType.Circular_Default)]
		public NeedleShapeType ShapeType { get { return Component.ShapeType; } set { Component.ShapeType = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class ArcScaleRangeBarComponentWrapper : CircularGaugeScaleComponentWrapper {
		protected ArcScaleRangeBarComponent Component {
			get { return WrappedObject as ArcScaleRangeBarComponent; }
		}
		[Category("Value"), DefaultValue(null)]
		public float? Value { get { return Component.Value; } set { Component.Value = value; } }
		bool ShouldSerializeAppearanceRangeBar() { return Component.AppearanceRangeBar.ShouldSerialize(); }
		void ResetAppearanceRangeBar() { Component.AppearanceRangeBar.Reset(); }
		[Category("Appearance")]
		public RangeBarAppearance AppearanceRangeBar { get { return Component.AppearanceRangeBar; } }
		[Category("Appearance"), DefaultValue(false)]
		public bool ShowBackground { get { return Component.ShowBackground; } set { Component.ShowBackground = value; } }
		[Category("Scale")]
		public IArcScale ArcScale { get { return Component.ArcScale; } set { Component.ArcScale = value; } }
		[Category("Geometry"), DefaultValue(0f)]
		public float StartOffset { get { return Component.StartOffset; } set { Component.StartOffset = value; } }
		[Category("Geometry"), DefaultValue(0f)]
		public float EndOffset { get { return Component.EndOffset; } set { Component.EndOffset = value; } }
		[Category("Geometry"), DefaultValue(10f)]
		public float AnchorValue { get { return Component.AnchorValue; } set { Component.AnchorValue = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		[Category("Geometry")]
		public float StartAngle { get { return Component.StartAngle; } set { Component.StartAngle = value; } }
		[Category("Geometry")]
		public float EndAngle { get { return Component.EndAngle; } set { Component.EndAngle = value; } }
		[Category("Geometry"), DefaultValue(false)]
		public bool RoundedCaps { get { return Component.RoundedCaps; } set { Component.RoundedCaps = value; } }
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class ArcScaleMarkerComponentWrapper : CircularGaugeScaleComponentWrapper {
		protected ArcScaleMarkerComponent Component {
			get { return WrappedObject as ArcScaleMarkerComponent; }
		}
		[Category("Value"), DefaultValue(null)]
		public float? Value { get { return Component.Value; } set { Component.Value = value; } }
		[Category("Appearance"), DefaultValue(MarkerPointerShapeType.Default)]
		public MarkerPointerShapeType ShapeType { get { return Component.ShapeType; } set { Component.ShapeType = value; } }
		[Category("Appearance"), DefaultValue(0f)]
		public float ShapeOffset { get { return Component.ShapeOffset; } set { Component.ShapeOffset = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		[Category("Scale")]
		public IArcScale ArcScale { get { return Component.ArcScale; } set { Component.ArcScale = value; } }
		[Category("Geometry")]
		public FactorF2D ShapeScale { get { return Component.ShapeScale; } set { Component.ShapeScale = value; } }
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		internal bool ShouldSerializeShapeScale() {
			return ShapeScale != new FactorF2D(1f, 1f);
		}
		internal void ResetShapeScale() {
			ShapeScale = new FactorF2D(1f, 1f);
		}
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class ArcScaleSpindleCapComponentWrapper : CircularGaugeScaleComponentWrapper {
		protected ArcScaleSpindleCapComponent Component {
			get { return WrappedObject as ArcScaleSpindleCapComponent; }
		}
		[Category("Appearance"), DefaultValue(SpindleCapShapeType.CircularFull_Default)]
		public SpindleCapShapeType ShapeType { get { return Component.ShapeType; } set { Component.ShapeType = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		[Category("Scale")]
		public IArcScale ArcScale { get { return Component.ArcScale; } set { Component.ArcScale = value; } }
		[Category("Geometry")]
		public SizeF Size { get { return Component.Size; } set { Component.Size = value; } }
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		internal bool ShouldSerializeSize() {
			return Size != new SizeF(40f, 40f);
		}
		internal void ResetSize() {
			Size = new SizeF(40f, 40f);
		}
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class ArcScaleStateIndicatorComponentWrapper : CircularGaugeScaleComponentWrapper {
		protected ArcScaleStateIndicatorComponent Component {
			get { return WrappedObject as ArcScaleStateIndicatorComponent; }
		}
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		[Category("Scale")]
		public IScale IndicatorScale { get { return Component.IndicatorScale; } set { Component.IndicatorScale = value; } }
		[Category("State")]
		public IndicatorStateCollection States { get { return Component.States; } }
		[Category("Geometry")]
		public PointF2D Center { get { return Component.Center; } set { Component.Center = value; } }
		[Category("Geometry")]
		public SizeF Size { get { return Component.Size; } set { Component.Size = value; } }
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		internal bool ShouldSerializeSize() {
			return Size != new SizeF(25, 25);
		}
		internal bool ShouldSerializeCenter() {
			return Center != PointF2D.Empty;
		}
		internal void ResetCenter() {
			Center = PointF2D.Empty;
		}
		internal void ResetSize() {
			Size = new SizeF(25, 25);
		}
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class StateImageIndicatorComponentWrapper : CircularGaugeScaleComponentWrapper {
		protected StateImageIndicatorComponent Component {
			get { return WrappedObject as StateImageIndicatorComponent; }
		}
		[Category("Appearance")]
		public BaseShapeAppearance AppearanceBackground {
			get { return Component.AppearanceBackground; }
		}
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		[Category("Geometry")]
		public PointF2D Position { get { return Component.Position; } set { Component.Position = value; } }
		[Category("Image"), DefaultValue(ImageLayoutMode.Default)]
		public ImageLayoutMode ImageLayoutMode { get { return Component.ImageLayoutMode; } set { Component.ImageLayoutMode = value; } }
		[Category("Image"), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public int? StateIndex { get { return Component.StateIndex; } set { Component.StateIndex = value; } }
		[Category("Image"), DefaultValue(false)]
		public bool AllowImageSkinning { get { return Component.AllowImageSkinning; } set { Component.AllowImageSkinning = value; } }
		[Category("Image")]
		public Color Color { get { return Component.Color; } set { Component.Color = value; } }
		[Category("States")]
		public ImageIndicatorStateCollection ImageStateCollection { get { return Component.ImageStateCollection; } }
		[Category("Scale")]
		public IScale IndicatorScale { get { return Component.IndicatorScale; } set { Component.IndicatorScale = value; } }
		[Category("Geometry")]
		public SizeF Size { get { return Component.Size; } set { Component.Size = value; } }
		internal bool ShouldSerializeSize() {
			return (Component.Image != null) ? Size != Component.Image.Size : Size != new SizeF(32, 32);
		}
		internal void ResetSize() {
			if(Component.Image != null)
				Size = Component.Image.Size;
			else
				Size = new SizeF(32, 32);
		}
		internal bool ShouldSerializeColor() {
			return Color != Color.Empty;
		}
		internal void ResetColor() {
			Color = Color.Empty;
		}
		internal bool ShouldSerializePosition() {
			return Position != new PointF2D(125, 125);
		}
		internal void ResetPosition() {
			Position = new PointF2D(125, 125);
		}
		internal bool ShouldSerializeAllowImageSkinning() {
			return AllowImageSkinning != false;
		}
		internal void ResetAllowImageSkinning() {
			AllowImageSkinning = false;
		}
	}
}
