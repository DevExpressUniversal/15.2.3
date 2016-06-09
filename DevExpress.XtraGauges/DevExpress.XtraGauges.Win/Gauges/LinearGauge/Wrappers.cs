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
namespace DevExpress.XtraGauges.Win.Gauges.Linear {
	[TypeConverter(typeof(ScaleComponentWrapperTypeConverter))]
	public abstract class LinearGaugeScaleComponentWraper : BasePropertyGridObjectWrapper { }
	[TypeConverter(typeof(LinearLogarithmicScaleObjectTypeConverter))]
	public class LinearScaleComponentWrapper : BasePropertyGridObjectWrapper, ILogarithmicBase {
		protected LinearScaleComponent Component {
			get { return WrappedObject as LinearScaleComponent; }
		}
		bool ShouldSerializeAppearanceScale() { return Component.AppearanceScale.ShouldSerialize(); }
		void ResetAppearanceScale() { Component.AppearanceScale.Reset(); }
		[Category("Appearance")]
		public BaseScaleAppearance AppearanceScale {
			get { return Component.Appearance; }
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
			get { return Component.MajorTickmark.TextShape.Appearance; }
		}
		bool ShouldSerializeAppearanceTickmarkText() { return Component.AppearanceTickmarkText.ShouldSerialize(); }
		void ResetAppearanceTickmarkText() { Component.AppearanceTickmarkText.Reset(); }
		[Category("Appearance")]
		public BaseTextAppearance AppearanceTickmarkText {
			get { return Component.MajorTickmark.TextShape.AppearanceText; }
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
		public LogarithmicBase LogarithmicBase { get { return Component.LogarithmicBase; } set { Component.LogarithmicBase = value; } }
		[Category("Scale"), DefaultValue(2)]
		public float CustomLogarithmicBase { get { return Component.CustomLogarithmicBase; } set { Component.CustomLogarithmicBase = value; } }
		[Category("Scale")]
		public RangeCollection Ranges { get { return Component.Ranges; } }
		[Category("Geometry")]
		public PointF2D StartPoint { get { return Component.StartPoint; } set { Component.StartPoint = value; } }
		[Category("Geometry")]
		public PointF2D EndPoint { get { return Component.EndPoint; } set { Component.EndPoint = value; } }
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		[Category("Scale"), DefaultValue(1f)]
		public float MaxValue { get { return Component.MaxValue; } set { Component.MaxValue = value; } }
		[Category("Scale"), DefaultValue(0f)]
		public float MinValue { get { return Component.MinValue; } set { Component.MinValue = value; } }
		[Category("Scale"), DefaultValue(0f)]
		public float Value { get { return Component.Value; } set { Component.Value = value; } }
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
		[Category("Tickmarks"), DefaultValue(21)]
		public int MinorTickCount { get { return Component.MinorTickCount; } set { Component.MinorTickCount = value; } }
		[Category("Tickmarks"), DefaultValue(11)]
		public int MajorTickCount { get { return Component.MajorTickCount; } set { Component.MajorTickCount = value; } }
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class LinearScaleBackgroundLayerComponentWrapper : LinearGaugeScaleComponentWraper {
		protected LinearScaleBackgroundLayerComponent Component {
			get { return WrappedObject as LinearScaleBackgroundLayerComponent; }
		}
		[Category("Scale")]
		public ILinearScale LinearScale { get { return Component.LinearScale; } set { Component.LinearScale = value; } }
		[Category("Geometry")]
		public PointF2D ScaleStartPos { get { return Component.ScaleStartPos; } set { Component.ScaleStartPos = value; } }
		[Category("Geometry")]
		public PointF2D ScaleEndPos { get { return Component.ScaleEndPos; } set { Component.ScaleEndPos = value; } }
		[Category("Appearance")]
		[DefaultValue(BackgroundLayerShapeType.Linear_Default)]
		public BackgroundLayerShapeType ShapeType { get { return Component.ShapeType; } set { Component.ShapeType = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		internal bool ShouldSerializeScaleStartPos() {
			return ScaleStartPos != new PointF2D(0.5f, 0.86f);
		}
		internal bool ShouldSerializeScaleEndPos() {
			return ScaleEndPos != new PointF2D(0.5f, 0.14f);
		}
		internal void ResetScaleStartPos() {
			ScaleStartPos = new PointF2D(0.5f, 0.86f);
		}
		internal void ResetScaleEndPos() {
			ScaleEndPos = new PointF2D(0.5f, 0.14f);
		}
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class LinearScaleEffectLayerComponentWrapper : LinearGaugeScaleComponentWraper {
		protected LinearScaleEffectLayerComponent Component {
			get { return WrappedObject as LinearScaleEffectLayerComponent; }
		}
		[Category("Scale")]
		public ILinearScale LinearScale { get { return Component.LinearScale; } set { Component.LinearScale = value; } }
		[Category("Geometry")]
		public PointF2D ScaleStartPos { get { return Component.ScaleStartPos; } set { Component.ScaleStartPos = value; } }
		[Category("Geometry")]
		public PointF2D ScaleEndPos { get { return Component.ScaleEndPos; } set { Component.ScaleEndPos = value; } }
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		[Category("Appearance")]
		[DefaultValue(EffectLayerShapeType.Circular_Default)]
		public EffectLayerShapeType ShapeType { get { return Component.ShapeType; } set { Component.ShapeType = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		internal bool ShouldSerializeScaleStartPos() {
			return ScaleStartPos != new PointF2D(0.5f, 0.1f);
		}
		internal bool ShouldSerializeScaleEndPos() {
			return ScaleEndPos != new PointF2D(0.5f, 0.9f);
		}
		internal void ResetScaleStartPos() {
			ScaleStartPos = new PointF2D(0.5f, 0.1f);
		}
		internal void ResetScaleEndPos() {
			ScaleEndPos = new PointF2D(0.5f, 0.9f);
		}
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class LinearScaleLevelComponentWrapper : LinearGaugeScaleComponentWraper {
		protected LinearScaleLevelComponent Component {
			get { return WrappedObject as LinearScaleLevelComponent; }
		}
		[Category("Value"), DefaultValue(null)]
		public float? Value { get { return Component.Value; } set { Component.Value = value; } }
		[Category("Scale")]
		public ILinearScale LinearScale { get { return Component.LinearScale; } set { Component.LinearScale = value; } }
		[Category("Appearance")]
		[DefaultValue(LevelShapeSetType.Default)]
		public LevelShapeSetType ShapeType { get { return Component.ShapeType; } set { Component.ShapeType = value; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class LinearScaleRangeBarComponentWrapper : LinearGaugeScaleComponentWraper {
		protected LinearScaleRangeBarComponent Component {
			get { return WrappedObject as LinearScaleRangeBarComponent; }
		}
		[Category("Value"), DefaultValue(null)]
		public float? Value { get { return Component.Value; } set { Component.Value = value; } }
		bool ShouldSerializeAppearanceRangeBar() { return Component.AppearanceRangeBar.ShouldSerialize(); }
		void ResetAppearanceRangeBar() { Component.AppearanceRangeBar.Reset(); }
		[Category("Appearance")]
		public BaseShapeAppearance AppearanceRangeBar { get { return Component.AppearanceRangeBar; } }
		[Category("Appearance")]
		public BaseColorShader Shader { get { return Component.Shader; } set { Component.Shader = value; } }
		[Category("Scale")]
		public ILinearScale LinearScale { get { return Component.LinearScale; } set { Component.LinearScale = value; } }
		[Category("Geometry")]
		[DefaultValue(0f)]
		public float StartOffset { get { return Component.StartOffset; } set { Component.StartOffset = value; } }
		[Category("Geometry")]
		[DefaultValue(10f)]
		public float EndOffset { get { return Component.EndOffset; } set { Component.EndOffset = value; } }
		[Category("Geometry")]
		[DefaultValue(0f)]
		public float AnchorValue { get { return Component.AnchorValue; } set { Component.AnchorValue = value; } }
		[Category("Geometry")]
		public int ZOrder { get { return Component.ZOrder; } set { Component.ZOrder = value; } }
		internal bool ShouldSerializeShader() {
			return !Shader.IsEmpty;
		}
		internal void ResetShader() {
			Shader = BaseColorShader.Empty;
		}
	}
	public class LinearScaleMarkerComponentWrapper : LinearGaugeScaleComponentWraper {
		protected LinearScaleMarkerComponent Component {
			get { return WrappedObject as LinearScaleMarkerComponent; }
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
		public ILinearScale LinearScale { get { return Component.LinearScale; } set { Component.LinearScale = value; } }
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
	public class LinearScaleStateIndicatorComponentWrapper : LinearGaugeScaleComponentWraper {
		protected LinearScaleStateIndicatorComponent Component {
			get { return WrappedObject as LinearScaleStateIndicatorComponent; }
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
}
