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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Localization;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
using System.Windows.Media;
namespace DevExpress.Xpf.Charts {
	public class SeriesPoint : DependencyObject, ISeriesPoint, IChartElement, INotifyPropertyChanged, IInteractiveElement {
		internal static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SeriesPoint point = d as SeriesPoint;
			if (point != null)
				point.Update("Values");			
		}
		internal static SeriesPoint GetSeriesPoint(ISeriesPoint point) {
			SeriesPoint seriesPoint = point as SeriesPoint;
			if (seriesPoint != null)
				return seriesPoint;
			AggregatedSeriesPoint aggregatedPoint = point as AggregatedSeriesPoint;
			if (aggregatedPoint != null)
				return new SeriesPoint(aggregatedPoint);
			return null;
		}
		internal static List<ISeriesPoint> GetSourcePoints(ISeriesPoint point) {
			List<ISeriesPoint> points = new List<ISeriesPoint>();
			if (point is SeriesPoint)
				points.Add(point);
			if (point is AggregatedSeriesPoint) {
				AggregatedSeriesPoint aggregatedPoint = point as AggregatedSeriesPoint;
				foreach (RefinedPoint refinedPoint in aggregatedPoint.SourcePoints)
					points.Add(refinedPoint.SeriesPoint);
			}
			return points;
		}
		internal static void SynchronizeProperties(DependencyObject d, DependencyPropertyChangedEventArgs e, DependencyProperty source, DependencyProperty destination) {
			SeriesPoint seriesPoint = d as SeriesPoint;
			if (seriesPoint != null) {
				if (seriesPoint.synchronizePropertiesLocker.IsLocked)
					return;
				seriesPoint.synchronizePropertiesLocker.Lock();
				try {
					seriesPoint.SetValue(destination, seriesPoint.GetValue(source));
				}
				finally {
					seriesPoint.synchronizePropertiesLocker.Unlock();
				}
			}
		}
		readonly Locker synchronizePropertiesLocker = new Locker();
		string argument;
		double? numericalArgument;
		DateTime? dateTimeArgument;
		ScaleType argumentScaleType;
		double numericalValue = double.NaN;
		DateTime dateTimeValue = DateTime.MinValue;
		object tag;
		object toolTipHint;
		bool animationInProcess = false;
		bool isSelected = false;
		bool isHighlighted = false;
		double baseNumericalValue = double.NaN;
		double internalArgument;
		double[] internalValues;
		SolidColorBrush brush;
		IChartElement owner = null;
		public event PropertyChangedEventHandler PropertyChanged;
		internal double[] InternalValues { get { return internalValues; } }
		internal ScaleType ArgumentScaleType { get { return argumentScaleType; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable { get { return this.GetType().Name; } }
		[
		DefaultValue(typeof(string), ""),
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointArgument"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public string Argument {
			get { return argument; }
			set {
				if (argument != value) {
					ScaleType scaleType = argumentScaleType;
					SetQualitativeArgument(value);					
					RaisePropertyChanged("Argument");
					Update("Argument");
				}
			}
		}
		[
		DefaultValue(double.NaN),
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointValue"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public double Value {
			get { return numericalValue; }
			set {
				if (double.IsInfinity(value))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgSeriesPointIncorrectValue));
				if (numericalValue != value) {
					numericalValue = value;
					if (!animationInProcess)
						baseNumericalValue = value;
					Update("Values");
					RaisePropertyChanged("Value");
				}
			}
		}
		[
		DefaultValue(typeof(DateTime), "01/01/0001 00:00 AM"),
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointDateTimeValue"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public DateTime DateTimeValue {
			get { return dateTimeValue; }
			set {
				if (dateTimeValue != value) {
					dateTimeValue = value;
					RaisePropertyChanged("DateTimeValue");
					Update("Values");
				}
			}
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointTag"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public object Tag {
			get { return tag; }
			set {
				if (!object.Equals(tag, value)) {
					tag = value;
					RaisePropertyChanged("Tag");
				}
			}
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointToolTipHint"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty,
		TypeConverter(typeof(StringToObjectConverter))]
		public object ToolTipHint {
			get { return toolTipHint; }
			set {
				if (!object.Equals(toolTipHint, value)) {
					toolTipHint = value;
					RaisePropertyChanged("ToolTipHint");
				}
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SeriesPointActualArgument")]
#endif
		public string ActualArgument {
			get {
				if (!string.IsNullOrEmpty(argument))
					return argument;
				Series series = Series;
				if (series == null)
					return null;
				return series.ActualArgumentScaleType == ScaleType.DateTime ? DateTime.Today.ToString(CultureInfo.InvariantCulture) : "0";
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointDateTimeArgument")
#else
	Description("")
#endif
		]
		public DateTime DateTimeArgument {
			get { return dateTimeArgument.HasValue ? dateTimeArgument.Value : DateTime.Today; }			
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SeriesPointNonAnimatedValue")]
#endif
		public double NonAnimatedValue { get { return baseNumericalValue; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointNumericalArgument")
#else
	Description("")
#endif
		]
		public double NumericalArgument {
			get { return numericalArgument.HasValue ? numericalArgument.Value : 0; }
			set {
				numericalArgument = value;
				argument = numericalArgument.Value.ToString(CultureInfo.InvariantCulture);
				dateTimeArgument = null;
				argumentScaleType = ScaleType.Numerical;
				RaisePropertyChanged("NumericalArgument");
				Update("Argument");
			}
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointBrush"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public SolidColorBrush Brush {
			get { return brush; }
			set {
				if (brush != value) {
					brush = value;
					Update("Brush");
					RaisePropertyChanged("Brush");
				}
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SeriesPointSeries")]
#endif
		public Series Series { get { return ((IOwnedElement)this).Owner as Series; } }
		public SeriesPoint() {
			ClearArguments();
		}
		public SeriesPoint(string argument) {
			SetQualitativeArgument(argument);
		}
		public SeriesPoint(double argument) {
			if (double.IsInfinity(argument)) {
				ClearArguments();
				return;
			}
			this.argument = argument.ToString(CultureInfo.InvariantCulture);
			numericalArgument = argument;
			dateTimeArgument = null;
			argumentScaleType = ScaleType.Numerical;
		}
		public SeriesPoint(DateTime argument) {
			this.argument = argument.ToString(CultureInfo.InvariantCulture);
			numericalArgument = null;
			dateTimeArgument = argument;
			argumentScaleType = ScaleType.DateTime;
		}
		public SeriesPoint(string argument, double value) : this(argument) {
			baseNumericalValue = numericalValue = value;
		}
		public SeriesPoint(double argument, double value) : this(argument) {
			baseNumericalValue = numericalValue = value;
		}
		public SeriesPoint(DateTime argument, double value) : this(argument) {
			baseNumericalValue = numericalValue = value;
		}
		public SeriesPoint(string argument, DateTime value) : this(argument) {
			baseNumericalValue = numericalValue = double.NaN;
			dateTimeValue = value;
		}
		public SeriesPoint(double argument, DateTime value) : this(argument) {
			baseNumericalValue = numericalValue = double.NaN;
			dateTimeValue = value;
		}
		public SeriesPoint(DateTime argument, DateTime value) : this(argument) {
			baseNumericalValue = numericalValue = double.NaN;
			dateTimeValue = value;
		}
		internal SeriesPoint(object argument, double internalArgument, double value, DateTime dateTimeValue, double[] internalValues, object tag) {
			SetArgument(argument);
			baseNumericalValue = numericalValue = value;
			this.dateTimeValue = dateTimeValue;
			this.internalArgument = internalArgument;
			this.internalValues = internalValues;
			Tag = tag;
		}
		internal SeriesPoint(object argument, double internalArgument, double value, DateTime dateTimeValue, double[] internalValues, object tag, object hint)
			: this(argument, internalArgument, value, dateTimeValue, internalValues, tag) {
			ToolTipHint = hint;
		}
		internal SeriesPoint(object argument, double internalArgument, double value, DateTime dateTimeValue, double[] internalValues, object tag, object hint, SolidColorBrush brush)
			: this(argument, internalArgument, value, dateTimeValue, internalValues, tag, hint) {
				Brush = brush;
		}
		SeriesPoint(AggregatedSeriesPoint aggregatedPoint) {
			SetArgument(aggregatedPoint.Argument);
			Series series = (Series)aggregatedPoint.Series;
			series.SetPointValues(this, aggregatedPoint.Values, aggregatedPoint.DateTimeValues);
			if (aggregatedPoint.SourcePoints.Count == 1) {
				SeriesPoint seriesPoint = aggregatedPoint.SourcePoints[0].SeriesPoint as SeriesPoint;
				if (seriesPoint != null) {
					tag = seriesPoint.Tag;
					toolTipHint = seriesPoint.ToolTipHint;
				}
			}
			else {
				List<object> tags = new List<object>();
				foreach (RefinedPoint point in aggregatedPoint.SourcePoints) {
					SeriesPoint seriesPoint = point.SeriesPoint as SeriesPoint;
					if (seriesPoint != null)
						tags.Add(seriesPoint.Tag);
				}
				tag = tags;
			}
			brush = series.ColorizerController.GetBrushForAgregatedPoint(aggregatedPoint);
			owner = series;
		}
		#region ISeriesPoint implementation
		Scale ISeriesPoint.ArgumentScaleType { get { return (Scale)argumentScaleType; } }
		object ISeriesPoint.UserArgument {
			get {
				Series series = Series;
				if (series != null)
					switch (series.ActualArgumentScaleType) {
						case ScaleType.Numerical:
							return NumericalArgument;
						case ScaleType.DateTime:
							return DateTimeArgument;
					}
				return ActualArgument;
			}
		}
		string ISeriesPoint.QualitativeArgument { get { return ActualArgument; } }
		double ISeriesPoint.NumericalArgument { get { return NumericalArgument; } }
		DateTime ISeriesPoint.DateTimeArgument { get { return DateTimeArgument; } }
		double[] ISeriesPoint.UserValues { get { return GetValues(); } }
		double[] ISeriesPoint.AnimatedValues { get { return Series == null ? new double[] { Value } : Series.GetAnimatedPointValues(this); } }
		DateTime[] ISeriesPoint.DateTimeValues { get { return GetDateTimeValues(); } }
		double ISeriesPoint.InternalArgument {
			get { return internalArgument; }
			set { internalArgument = value; }
		}
		double[] ISeriesPoint.InternalValues { 
			get { return internalValues; } 
			set { internalValues = value; }
		}
		bool ISeriesPoint.IsEmpty(Scale valueScaleType) { 
			switch (valueScaleType) {
				case Scale.Numerical:
					double[] values = GetValues();
					foreach (double value in values)
						if (Double.IsNaN(value))
							return true;
					return false;
				case Scale.DateTime:
					DateTime[] dateTimeValues = GetDateTimeValues();
					foreach (DateTime value in dateTimeValues)
						if (value.Equals(DateTime.MinValue))
							return true;
					return false;
				default:
					ChartDebug.Fail("Incorrect scale type for the series point value.");
					return true;
			}
		}
		#endregion        
		#region IChartElement implementation
		IChartElement IOwnedElement.Owner {
			get { return owner; }
			set { owner = value; }
		}
		void IChartElement.AddChild(object child) { 
		}
		void IChartElement.RemoveChild(object child) { 
		}
		bool IChartElement.Changed(ChartUpdate args) {
			if (owner != null)
				return owner.Changed(args);
			return true;
		}
		ViewController INotificationOwner.Controller { get { return owner == null ? null : owner.Controller; } }
		#endregion
		#region IInteractiveElement implementation
		bool IInteractiveElement.IsHighlighted {
			get { return isHighlighted; }
			set {
				if (isHighlighted != value) {
					isHighlighted = value;
					OnIsHighlightedChanged();
				}
			}
		}
		bool IInteractiveElement.IsSelected {
			get { return isSelected; }
			set {
				if (isSelected != value) {
					isSelected = value;
					OnIsSelectedChanged();
				}
			}
		}
		object IInteractiveElement.Content {
			get { return (Tag != null) ? Tag : this; }
		}
		void OnIsHighlightedChanged() {	
		}
		void OnIsSelectedChanged() {
			if (Series != null)
				Series.ChangeSeriesPointSelection(this, isSelected);
		}
		#endregion
		void Update(string propertyName) {
			ChartElementHelper.Update((IChartElement)this, new PropertyUpdateInfo<ISeries, ISeriesPoint>(this, propertyName, null, null, Series), ChartElementChange.ClearDiagramCache);
		}
		void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		void SetArgument(object argument) {
			Type argumentType = argument.GetType();
			if (argumentType == typeof(DateTime)) {
				dateTimeArgument = (DateTime)argument;
				this.argument = dateTimeArgument.Value.ToString(CultureInfo.InvariantCulture);
				numericalArgument = null;
				argumentScaleType = ScaleType.DateTime;
			}
			else {
				if (argumentType == typeof(double) || argumentType == typeof(float) || argumentType == typeof(int) ||
					argumentType == typeof(uint) || argumentType == typeof(long) || argumentType == typeof(ulong) ||
					argumentType == typeof(decimal) || argumentType == typeof(short) || argumentType == typeof(ushort) ||
					argumentType == typeof(byte) || argumentType == typeof(sbyte) || argumentType == typeof(char)) {
					numericalArgument = Convert.ToDouble(argument);
					this.argument = numericalArgument.Value.ToString(CultureInfo.InvariantCulture);
					dateTimeArgument = null;
					argumentScaleType = ScaleType.Numerical;
				}
				else
					SetQualitativeArgument(argument.ToString());
			}
		}
		void SetQualitativeArgument(string argument) {
			if (string.IsNullOrEmpty(argument)) {
				ClearArguments();
				return;
			}
			this.argument = argument;
			double numeric;
			if (double.TryParse(argument, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out numeric)) {
				numericalArgument = numeric;
				dateTimeArgument = null;
				argumentScaleType = ScaleType.Numerical;
			}
			else {
				numericalArgument = null;
				DateTime dateTime;
				if (DateTime.TryParse(argument, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)) {
					dateTimeArgument = dateTime;
					argumentScaleType = ScaleType.DateTime;
				}
				else {
					dateTimeArgument = null;
					argumentScaleType = ScaleType.Qualitative;
				}
			}
		}
		void ClearArguments() {
			argument = string.Empty;
			numericalArgument = null;
			dateTimeArgument = null;
			argumentScaleType = ScaleType.Auto;
		}
		double[] GetValues() {
			return Series == null ? new double[] { baseNumericalValue } : Series.GetPointValues(this);
		}
		DateTime[] GetDateTimeValues() {
			return Series == null ? new DateTime[] { DateTimeValue } : Series.GetPointDateTimeValues(this);
		}
		SeriesPointAnimationBase GetActualAnimation() {
			return Series != null ? Series.GetActualPointAnimation() : null;
		}
		internal void SetAnimatedArgument(string argument) {
			animationInProcess = true;
			Argument = argument;
			animationInProcess = false;
		}
		internal void SetAnimatedValue(double value) {
			animationInProcess = true;
			Value = value;
			animationInProcess = false;
		}
	}
}
