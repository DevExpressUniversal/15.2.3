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
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Drawing;
using System.ComponentModel;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraGauges.Core.Model {	
	public interface ILogarithmicBase {
		LogarithmicBase LogarithmicBase { get; set; }
		float CustomLogarithmicBase { get; set; }
		bool Logarithmic { get; set; }
	}
	public interface ILogarithmicScale : IScale, ILogarithmicBase { }
	public interface IScale : IPrimitive {
		bool IsEmpty { get; }
		bool IsDiscrete { get; }
		bool IsLogarithmic { get; }
		bool IsAnimating { get; }
		float MinValue { get; set; }
		float MaxValue { get; set; }
		float ScaleLength { get; }
		float Value { get; set; }
		event EventHandler ValueChanged;
		event EventHandler MinMaxValueChanged;
		float GetInternalValue();
	}
	public interface ILabelColorProvider {
		Color GetLabelShemeColor();
	}
	public interface IConvertibleScale : ILogarithmicScale {
		float Percent { get;}
		float PercentToValue(float percent);
		float ValueToPercent(float value);
	}
	public interface IConvertibleScaleEx : IConvertibleScale, ISupportLockUpdate {
		PointF PercentToPoint(float percent);
		float PointToPercent(PointF point);
	}
	public interface IDiscreteScale : IPrimitive, ISupportLockUpdate {
		BaseScaleAppearance Appearance { get;}
		int MajorTickCount { get;set;}
		int MinorTickCount { get;set;}
		IMinorTickmark MinorTickmark { get;}
		IMajorTickmark MajorTickmark { get;}
		LabelCollection Labels { get;}
		RangeCollection Ranges { get;}
		int TickCount { get;}
	}
	public interface ISupportAutoRescaling {
		bool AutoRescaling { get;set;}
		float RescalingThresholdMin { get;set;}
		float RescalingThresholdMax { get;set;}
		bool RescalingBestValues { get;set;}
		event CustomRescalingEventHandler CustomRescaling;
	}
	public interface IBaseScale : IConvertibleScaleEx, IDiscreteScale, ISupportAutoRescaling {
		IScaleRange CreateRange();
		IScaleLabel CreateLabel();
		event EventHandler GeometryChanged;
		event EventHandler Animating;
		event EventHandler AnimationCompleted;
	}
	public interface ILinearScale : IBaseScale {
		PointF2D StartPoint { get;set;}
		PointF2D EndPoint { get;set;}
	}
	public interface IArcScale : IBaseScale {
		PointF2D Center { get;set;}
		float RadiusX { get;set;}
		float RadiusY { get;set;}
		float StartAngle { get;set;}
		float EndAngle { get;set;}
		PointF NeedleVector { get;}
	}
	public interface ITickmark : IPrimitive, INamedObject, ICloneable, ISupportLockUpdate, ISupportAcceptOrder {
		PointF2D Origin { get;set;}
		PointF2D Orientation { get;set;}
		BaseShape Shape { get;}
	}
	[TypeConverter(typeof(TickmarkObjectTypeConverter))]
	public interface IMinorTickmark : ITickmark, ISupportAssign<IMinorTickmark> {
		TickmarkShapeType ShapeType { get;set;}
		FactorF2D ShapeScale { get;set;}
		float ShapeOffset { get;set;}
		bool ShowTick { get;set;}
		bool ShowFirst { get;set;}
		bool ShowLast { get;set;}
	}
	[TypeConverter(typeof(TickmarkObjectTypeConverter))]
	public interface IMajorTickmark : ITickmark, ISupportAssign<IMajorTickmark> {
		TickmarkShapeType ShapeType { get;set;}
		FactorF2D ShapeScale { get;set;}
		float ShapeOffset { get;set;}
		bool ShowTick { get;set;}
		string Text { get;set;}
		string FormatString { get;set;}
		float Addend { get;set;}
		float Multiplier { get;set;}
		LabelOrientation TextOrientation { get;set;}
		float TextOffset { get;set;}
		TextShape TextShape { get;}
		bool AllowTickOverlap { get;set;}
		bool ShowText { get;set;}
		bool ShowFirst { get;set;}
		bool ShowLast { get;set;}
	}
	public interface ILabel : IBaseObject, ISupportLockUpdate, ISupportAcceptOrder, ISupportAssign<ILabel>, INamed {
		PointF2D Position { get; set; }
		SizeF Size { get; set; }
		LabelOrientation TextOrientation { get; set; }
		string Text { get; set; }
		string FormatString { get; set; }
		BaseShapeAppearance AppearanceBackground { get; }
		BaseTextAppearance AppearanceText { get; }
		TextShape TextShape { get; }
	}
	public interface IImageIndicator : IBaseObject, ISupportLockUpdate, ISupportAcceptOrder, ISupportAssign<IImageIndicator>, INamed {
		PointF2D Position { get; set; }
		SizeF Size { get; set; }
		BaseShapeAppearance AppearanceBackground { get; }
		Image Image { get; set; }
		Color Color { get; set; }
		object StateImages { get; set; }
		int? StateIndex { get; set; }
		IScale IndicatorScale { get; set; }
		ImageIndicatorStateCollection ImageStateCollection { get; }
		ImageLayoutMode ImageLayoutMode { get; set; }
		ImageIndicatorShape ImageIndicatorShape { get; }
	}
	public interface IStateImageIndicator : IImageIndicator, IScaleComponent { }
	public interface ILabelEx : ILabel {
		bool AllowHTMLString { get;set;}
	}
	public interface IScaleLabel : ILabelEx {
		IScale Scale { get;}
		float Addend { get; set;}
		float Multiplier { get; set;}
	}
	public interface IRange : IDisposable, ISupportLockUpdate, ISupportAcceptOrder, ISupportAssign<IRange>, INamed {
		float StartValue { get;set;}
		float EndValue { get;set;}
		float StartThickness { get;set;}
		float EndThickness { get;set;}
		float ShapeOffset { get;set;}
		BaseShapeAppearance AppearanceRange { get;}
		event EventHandler Enter;
		event EventHandler Leave;
		BaseShape Shape { get;}
	}
	public interface IScaleRange : IRange {
		IScale Scale { get;}
		float? StartPercent { get;set;}
		float? EndPercent { get;set;}
	}
	public interface IScaleComponent : IPrimitive, ISupportLockUpdate {
		IScale Scale { get;}
	}
	public interface IArcScaleComponent : IScaleComponent {
		IArcScale ArcScale { get;set;}
	}
	public interface ILinearScaleComponent : IScaleComponent {
		ILinearScale LinearScale { get;set;}
	}
	public interface IValueIndicator {
		float ActualValue { get; }
		float? Value { get; set; }
	}
	public interface IArcScalePointer : IValueIndicator, IArcScaleComponent {
	}
	public interface ILinearScalePointer : IValueIndicator, ILinearScaleComponent {
	}
	public interface INeedle : IScaleComponent {
		float StartOffset { get;set;}
		float EndOffset { get;set;}
		NeedleShapeType ShapeType { get;set;}
		BaseShape Shape { get;}
	}
	public interface IRangeBar : IScaleComponent {
		float StartOffset { get;set;}
		float EndOffset { get;set;}
		float AnchorValue { get;set;}
		RangeBarAppearance Appearance { get; }
		void LockValue(float value);
		void UnlockValue();
	}
	public interface IMarker : IScaleComponent {
		FactorF2D ShapeScale { get;set;}
		float ShapeOffset { get;set;}
		MarkerPointerShapeType ShapeType { get;set;}
		BaseShape Shape { get;}
	}
	public interface ILayer<T> : IPrimitive{
		T ShapeType { get;set;}
		BaseShape Shape { get;}
	}
	public interface IScaleBackgroundLayer :
		ILayer<BackgroundLayerShapeType>, IScaleComponent {
	}
	public interface IScaleEffectLayer :
		ILayer<EffectLayerShapeType>, IScaleComponent {
	}
	public interface IDigitalLayer<T> : ILayer<T>, ISupportLockUpdate {
		PointF2D TopLeft { get;set;}
		PointF2D BottomRight { get;set;}
	}
	public interface IDigitalBackgroundLayer : IDigitalLayer<DigitalBackgroundShapeSetType> {
		BaseShape ShapeNear { get;}
		BaseShape ShapeFar { get;}
	}
	public interface IDigitalEffectLayer : IDigitalLayer<DigitalEffectShapeType> {
	}
	public interface ISpindleCap : IScaleComponent {
		SizeF Size { get;set;}
		SpindleCapShapeType ShapeType { get;set;}
		BaseShape Shape { get;}
	}
	public interface IArcScaleBackgroundLayer : IScaleBackgroundLayer, IArcScaleComponent{
		PointF2D ScaleCenterPos { get;set;}
		SizeF Size { get;set;}
	}
	public interface ILinearScaleBackgroundLayer : IScaleBackgroundLayer, ILinearScaleComponent{
		PointF2D ScaleStartPos { get;set;}
		PointF2D ScaleEndPos { get;set;}
	}
	public interface IArcScaleEffectLayer : IScaleEffectLayer, IArcScaleComponent {
		PointF2D ScaleCenterPos { get;set;}
		SizeF Size { get;set;}
	}
	public interface ILinearScaleEffectLayer : IScaleEffectLayer, ILinearScaleComponent {
		PointF2D ScaleStartPos { get;set;}
		PointF2D ScaleEndPos { get;set;}
	}
	public interface IScaleLevel : IScaleComponent {
		LevelShapeSetType ShapeType { get;set;}
		BaseShape BarStartShape { get;}
		BaseShape BarPackedShape { get;}
		BaseShape BarEmptyShape { get;}
		BaseShape BarEndShape { get;}
	}
	[TypeConverter(typeof(StateIndicatorObjectTypeConverter))]
	public interface IStateIndicator : IPrimitive, ISupportLockUpdate {
		int StateIndex { get;set;}
		IIndicatorState State { get;}
		IndicatorStateCollection States { get;}
		void AddEnum(Enum states);
		void SetStateByName(string name);
		PointF2D Center { get;set;}
		SizeF Size { get;set;}
	}
	[TypeConverter(typeof(StateIndicatorObjectTypeConverter))]
	public interface IScaleStateIndicator : IStateIndicator, IScaleComponent { 
		IScale IndicatorScale { get;set;}
	}
	[TypeConverter(typeof(IndicatorStateObjectTypeConverter))]
	public interface IIndicatorState : ISupportAcceptOrder, ISupportAssign<IIndicatorState>, INamed {
		bool IsUnknown { get;}
		StateIndicatorShapeType ShapeType { get;set;}
		BaseShape Shape { get;}
	}
	public interface IScaleIndicatorState : IIndicatorState {
		float StartValue { get;set;}
		float IntervalLength { get;set;}
	}
}
