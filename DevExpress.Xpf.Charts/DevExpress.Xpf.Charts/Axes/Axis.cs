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
using System.ComponentModel;
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Localization;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class Axis : AxisBase, ILogarithmic {
		[Obsolete(ObsoleteMessages.AxisGridSpacingProperty)]
		public static readonly DependencyProperty GridSpacingProperty = DependencyPropertyManager.Register("GridSpacing", typeof(double),
			typeof(Axis), new PropertyMetadata(Double.NaN, GridSpacingPropertyChanged), new ValidateValueCallback(ValidateGridSpacing));
		[Obsolete(ObsoleteMessages.AxisDateTimeMeasureUnitProperty)]
		public static readonly DependencyProperty DateTimeMeasureUnitProperty = DependencyPropertyManager.Register("DateTimeMeasureUnit", typeof(DateTimeMeasurementUnit),
			typeof(Axis), new FrameworkPropertyMetadata(DateTimeMeasurementUnit.Day, FrameworkPropertyMetadataOptions.AffectsRender, MeasureUnitPropertyChanged));
		[Obsolete(ObsoleteMessages.AxisDateTimeGridAlignmentProperty)]
		public static readonly DependencyProperty DateTimeGridAlignmentProperty = DependencyPropertyManager.Register("DateTimeGridAlignment", typeof(DateTimeMeasurementUnit),
			typeof(Axis), new FrameworkPropertyMetadata(DateTimeMeasurementUnit.Day, FrameworkPropertyMetadataOptions.AffectsRender, GridAlignmentPropertyChanged));
		[Obsolete(ObsoleteMessages.AxisDateTimeOptionsProperty)]
		public static readonly DependencyProperty DateTimeOptionsProperty = DependencyPropertyManager.Register("DateTimeOptions",
			typeof(DateTimeOptions), typeof(Axis), new PropertyMetadata(DateTimeOptionsPropertyChanged));
		[Obsolete(ObsoleteMessages.RangeProperty)]
		public static readonly DependencyProperty RangeProperty;
		public static readonly DependencyProperty TitleProperty = DependencyPropertyManager.Register("Title",
			typeof(AxisTitle), typeof(Axis), new PropertyMetadata(ChartElementHelper.ChangeOwnerAndUpdateXYDiagram2DItems));
		public static readonly DependencyProperty LogarithmicProperty = DependencyPropertyManager.Register("Logarithmic", typeof(bool),
			typeof(Axis), new FrameworkPropertyMetadata(false, LogarithmicPropertyChanged));
		public static readonly DependencyProperty LogarithmicBaseProperty = DependencyPropertyManager.Register("LogarithmicBase", typeof(double),
			typeof(Axis), new FrameworkPropertyMetadata(10.0, LogarithmicPropertyChanged, CoercyLogarithmicBase));
		public static readonly DependencyProperty WholeRangeProperty = DependencyPropertyManager.Register("WholeRange", typeof(Range), typeof(Axis), new PropertyMetadata(WholeRangePropertyChanged));
		static object CoercyLogarithmicBase(DependencyObject d, object value) {
			if ((double)value <= 1)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidLogarithmicBase));
			return value;
		}
		static bool ValidateGridSpacing(object value) {
			double gridSpacing = (double)value;
			return Double.IsNaN(gridSpacing) || gridSpacing > 0;
		}
		static void GridSpacingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Axis axis = d as Axis;
			if (axis != null) {
				axis.gridSpacing = (double)e.NewValue;
				axis.ActualGridSpacing = (double)e.NewValue;
			}
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		static void DateTimeOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Axis axis = d as Axis;
			if (axis != null) {
				axis.dateTimeOptions = e.NewValue as DateTimeOptions;
				axis.UpdateLabelTextPattern();
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as DateTimeOptions, e.NewValue as DateTimeOptions, axis);
			}
			ChartElementHelper.Update(d, e);
		}
		[Obsolete]
		static void RangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Axis axis = d as Axis;
			if (axis != null) {
				AxisRange range = e.NewValue as AxisRange;
				if (range == null) {
					axis.ActualRange = new AxisRange();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, axis.ActualRange);
				}
				else {
					range.UpdateMinMaxValues(axis);
					axis.ActualRange = range;
				}
			}
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.ClearDiagramCache);
		}
		static void LogarithmicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Axis axis = d as Axis;
			if (axis != null)
				axis.ScaleMap.BuildTransformation(axis);
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache);
		}
		static void MeasureUnitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Axis axis = d as Axis;
			if (axis != null)
				axis.ActualDateTimeMeasureUnit = (DateTimeMeasurementUnit)e.NewValue;
			ChartElementHelper.Update((IChartElement)axis, new DataAggregationUpdate(d as IAxisData), ChartElementChange.None);
		}
		static void GridAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Axis axis = d as Axis;
			if (axis != null)
				axis.ActualGridAlignment = (DateTimeMeasurementUnit)e.NewValue;
			ChartElementHelper.Update((IChartElement)axis, new PropertyUpdateInfo(d, "GridAlignment"), ChartElementChange.None);
		}
		static void WholeRangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Axis axis = d as Axis;
			if (axis != null) {
				Range range = e.NewValue as Range;
				if (range == null) {
					range = new Range();
					axis.WholeRangeData.Reset(false);
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, range);
				}
				range.SetRangeData(axis.WholeRangeData);
				axis.ActualWholeRange = range;
				axis.CreateMediator();
			}
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.ClearDiagramCache);
		}
		DateTimeOptions dateTimeOptions;
		[
		Obsolete(ObsoleteMessages.AxisGridSpacingProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public double GridSpacing {
			get { return (double)GetValue(GridSpacingProperty); }
			set { SetValue(GridSpacingProperty, value); }
		}
		[
		Obsolete(ObsoleteMessages.AxisDateTimeMeasureUnitProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DateTimeMeasurementUnit DateTimeMeasureUnit {
			get { return (DateTimeMeasurementUnit)GetValue(DateTimeMeasureUnitProperty); }
			set { SetValue(DateTimeMeasureUnitProperty, value); }
		}
		[
		Obsolete(ObsoleteMessages.AxisDateTimeGridAlignmentProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DateTimeMeasurementUnit DateTimeGridAlignment {
			get { return (DateTimeMeasurementUnit)GetValue(DateTimeGridAlignmentProperty); }
			set { SetValue(DateTimeGridAlignmentProperty, value); }
		}
		[
		Obsolete(ObsoleteMessages.AxisDateTimeOptionsProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DateTimeOptions DateTimeOptions {
			get { return (DateTimeOptions)GetValue(DateTimeOptionsProperty); }
			set { SetValue(DateTimeOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisLogarithmic"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Logarithmic {
			get { return (bool)GetValue(LogarithmicProperty); }
			set { SetValue(LogarithmicProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisLogarithmicBase"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double LogarithmicBase {
			get { return (double)GetValue(LogarithmicBaseProperty); }
			set { SetValue(LogarithmicBaseProperty, value); }
		}
		[
		TypeConverter(typeof(AxisRangeTypeConverter)),
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisRange"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden, true),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		Obsolete(ObsoleteMessages.RangeProperty)
		]
		public AxisRange Range {
			get { return (AxisRange)GetValue(RangeProperty); }
			set { SetValue(RangeProperty, value); }
		}
		[
		Category(Categories.Common),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Range WholeRange {
			get { return (Range)GetValue(WholeRangeProperty); }
			set { SetValue(WholeRangeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisTitle"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public AxisTitle Title {
			get { return (AxisTitle)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		double gridSpacing = Double.NaN;
		internal bool IsTitleVisible {
			get {
				AxisTitle title = Title;
				if (title == null || !title.ActualVisible)
					return false;
				object content = title.Content;
				if (content == null)
					return false;
				string text = content as string;
				return text == null || text != String.Empty;
			}
		}
		protected internal override double GridSpacingImpl { get { return gridSpacing; } }
		protected internal override DateTimeOptions DateTimeOptionsImpl { get { return dateTimeOptions; } }
		protected internal abstract bool ShouldRotateTitle { get; }
		protected internal abstract AxisAlignment ActualAlignment { get; }
		[Obsolete]
		static Axis() {
			RangeProperty = DependencyPropertyManager.Register("Range", typeof(AxisRange), typeof(Axis), new PropertyMetadata(RangePropertyChanged));
		}
		public Axis() {
			BeginInit();
			actualWholeRangeValue = new Range();
			actualWholeRangeValue.SetRangeData(WholeRangeData);
			actualWholeRangeValue.ChangeOwner(null, this);
			EndInit();
		}
		#region ILogarithmic implementation
		bool ILogarithmic.Enabled { get { return Logarithmic; } }
		double ILogarithmic.Base { get { return LogarithmicBase; } }
		#endregion
		protected internal override IAxisMapping CreateMapping(Rect bounds) {
			return new AxisMappingEx(this, IsVertical ? bounds.Height : bounds.Width);
		}
		protected internal override void UpdateUserValues() {
			base.UpdateUserValues();
#pragma warning disable 0612
			DeserializeObsolete();
#pragma warning restore 0612
			if (ActualWholeRange != null)
				ActualWholeRange.UpdateMinMaxValues(this);
		}
		[Obsolete]
		void DeserializeObsolete() {
			if (Range != null)
				Range.UpdateMinMaxValues(this);
		}
	}
}
