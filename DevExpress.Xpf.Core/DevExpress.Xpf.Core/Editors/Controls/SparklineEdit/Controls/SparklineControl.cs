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

using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors {
	public class SparklineRangeChangedEventArgs : EventArgs {
		public object MinValue { get; private set; }
		public object MaxValue { get; private set; }
		public SparklineRangeChangedEventArgs (object minValue, object maxValue)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}
		public SparklineRangeChangedEventArgs(InternalRange range) {
			MinValue = SparklineMathUtils.ConvertToNative(range.Min, range.ScaleTypeMin);
			MaxValue = SparklineMathUtils.ConvertToNative(range.Max, range.ScaleTypeMax);
		}
	}
	public delegate void SparklineArgumentRangeChangedEventHandler(object sender, SparklineRangeChangedEventArgs e);
	public delegate void SparklineValueRangeChangedEventHandler(object sender, SparklineRangeChangedEventArgs e);
	public delegate void SparklinePointsChangedEventHandler(object sender, EventArgs e);
	public interface ISparklineRangeContainer { 
		void RaiseArgumentRangeChanged(SparklineRangeChangedEventArgs e);
		void RaiseValueRangeChanged(SparklineRangeChangedEventArgs e);
	}
	public interface IRangeContainer { 
		void OnRangeChanged();
	}
	[ContentProperty("Points")]
	public abstract class SparklineControl : Control, ISparklineRangeContainer, IRangeContainer {
		#region Dependency Properties
		public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(SparklinePointCollection), typeof(SparklineControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((SparklineControl)d).OnPointsPropertyChanged((SparklinePointCollection)e.NewValue)));
		public static readonly DependencyProperty HighlightMaxPointProperty = DependencyProperty.Register("HighlightMaxPoint", typeof(Boolean), typeof(SparklineControl),
			new FrameworkPropertyMetadata(defaultHighlightMaxPoint, (d, e) => ((SparklineControl)d).OnHighlightMaxPointChanged((bool)e.NewValue)));
		public static readonly DependencyProperty HighlightMinPointProperty = DependencyProperty.Register("HighlightMinPoint", typeof(Boolean), typeof(SparklineControl),
			new FrameworkPropertyMetadata(defaultHighlightMinPoint, (d, e) => ((SparklineControl)d).OnHighlightMinPointChanged((bool)e.NewValue)));
		public static readonly DependencyProperty HighlightStartPointProperty = DependencyProperty.Register("HighlightStartPoint", typeof(Boolean), typeof(SparklineControl),
			new FrameworkPropertyMetadata(defaultHighlightStartPoint, (d, e) => ((SparklineControl)d).OnHighlightStartPointChanged((bool)e.NewValue)));
		public static readonly DependencyProperty HighlightEndPointProperty = DependencyProperty.Register("HighlightEndPoint", typeof(Boolean), typeof(SparklineControl),
			new FrameworkPropertyMetadata(defaultHighlightEndPoint, (d, e) => ((SparklineControl)d).OnHighlightEndPointChanged((bool)e.NewValue)));
		public static readonly DependencyProperty BrushProperty = DependencyProperty.Register("Brush", typeof(SolidColorBrush), typeof(SparklineControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((SparklineControl)d).OnBrushChanged((SolidColorBrush)e.NewValue)));
	   public static readonly DependencyProperty MaxPointBrushProperty = DependencyProperty.Register("MaxPointBrush", typeof(SolidColorBrush), typeof(SparklineControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((SparklineControl)d).OnMaxPointBrushChanged((SolidColorBrush)e.NewValue)));
		public static readonly DependencyProperty MinPointBrushProperty = DependencyProperty.Register("MinPointBrush", typeof(SolidColorBrush), typeof(SparklineControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((SparklineControl)d).OnMinPointBrushChanged((SolidColorBrush)e.NewValue)));
		public static readonly DependencyProperty StartPointBrushProperty = DependencyProperty.Register("StartPointBrush", typeof(SolidColorBrush), typeof(SparklineControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((SparklineControl)d).OnStartPointBrushChanged((SolidColorBrush)e.NewValue)));
		public static readonly DependencyProperty EndPointBrushProperty = DependencyProperty.Register("EndPointBrush", typeof(SolidColorBrush), typeof(SparklineControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((SparklineControl)d).OnEndPointBrushChanged((SolidColorBrush)e.NewValue)));
		public static readonly DependencyProperty NegativePointBrushProperty = DependencyProperty.Register("NegativePointBrush", typeof(SolidColorBrush), typeof(SparklineControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((SparklineControl)d).OnNegativePointBrushChanged((SolidColorBrush)e.NewValue)));
		public static readonly DependencyProperty AutoRangeProperty = DependencyProperty.Register("AutoRange", typeof(bool), typeof(SparklineControl),
			new FrameworkPropertyMetadata(defaultAutoRange, (d, e) => ((SparklineControl)d).OnAutoRangeChanged((bool)e.NewValue)));
		public static readonly DependencyProperty ArgumentRangeProperty = DependencyProperty.Register("ArgumentRange", typeof(Range), typeof(SparklineControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((SparklineControl)d).OnArgumentRangeChanged((Range)e.NewValue)));
		public static readonly DependencyProperty ValueRangeProperty = DependencyProperty.Register("ValueRange", typeof(Range), typeof(SparklineControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((SparklineControl)d).OnValueRangeChanged((Range)e.NewValue)));
		#endregion
		const bool defaultAutoRange = true;
		const bool defaultHighlightMaxPoint = false;
		const bool defaultHighlightMinPoint = false;
		const bool defaultHighlightStartPoint = false;
		const bool defaultHighlightEndPoint = false;
		SparklinePointCollection actualPoints;
		bool highlightMaxPoint = defaultHighlightMaxPoint;
		bool highlightMinPoint = defaultHighlightMinPoint;
		bool highlightStartPoint = defaultHighlightStartPoint;
		bool highlightEndPoint = defaultHighlightEndPoint;
		SolidColorBrush defaultBrush = new SolidColorBrush(Colors.Black);
		SolidColorBrush brush;
		SolidColorBrush maxPointBrush = new SolidColorBrush(Colors.Black);
		SolidColorBrush minPointBrush = new SolidColorBrush(Colors.Black);
		SolidColorBrush startPointBrush = new SolidColorBrush(Colors.Black);
		SolidColorBrush endPointBrush = new SolidColorBrush(Colors.Black);
		SolidColorBrush negativePointBrush = new SolidColorBrush(Colors.Black);
		BaseSparklinePainter painter;
		SparklinePointCollection randomPoints;
		SparklinePointArgumentComparer argumentComparer;
		RangeDirector rangeDirector;
		ExtremePointIndexes extremeIndexes;
		internal SparklinePointCollection ActualPoints { 
			get {
				if ((actualPoints == null || actualPoints.Count == 0) && DesignMode)
					return randomPoints;
				else {
					return actualPoints;
				}
			} 
		}
		internal SolidColorBrush ActualBrush { 
			get { 
				if (brush != null)
					return brush;
				else if (Foreground != null && Foreground is SolidColorBrush)
					return Foreground as SolidColorBrush;
				return defaultBrush;
			} 
		}
		internal SolidColorBrush ActualMaxPointBrush { get { return maxPointBrush; } }
		internal SolidColorBrush ActualMinPointBrush { get { return minPointBrush; } }
		internal SolidColorBrush ActualStartPointBrush { get { return startPointBrush; } }
		internal SolidColorBrush ActualEndPointBrush { get { return endPointBrush; } }
		internal SolidColorBrush ActualNegativePointBrush { get { return negativePointBrush; } }
		internal bool ActualHighlightMaxPoint { get { return highlightMaxPoint; } }
		internal bool ActualHighlightMinPoint { get { return highlightMinPoint; } }
		internal bool ActualHighlightStartPoint { get { return highlightStartPoint; } }
		internal bool ActualHighlightEndPoint { get { return highlightEndPoint; } }
		internal bool DesignMode { get { return DesignerProperties.GetIsInDesignMode(this); } }
		public event SparklineArgumentRangeChangedEventHandler SparklineArgumentRangeChanged;
		public event SparklineValueRangeChangedEventHandler SparklineValueRangeChanged;
		public event SparklinePointsChangedEventHandler SparklinePointsChanged;
		public bool HighlightMaxPoint {
			get { return (bool)GetValue(HighlightMaxPointProperty); }
			set { SetValue(HighlightMaxPointProperty, value); }
		}
		public bool HighlightMinPoint {
			get { return (bool)GetValue(HighlightMinPointProperty); }
			set { SetValue(HighlightMinPointProperty, value); }
		}
		public bool HighlightStartPoint {
			get { return (bool)GetValue(HighlightStartPointProperty); }
			set { SetValue(HighlightStartPointProperty, value); }
		}
		public bool HighlightEndPoint {
			get { return (bool)GetValue(HighlightEndPointProperty); }
			set { SetValue(HighlightEndPointProperty, value); }
		}
		public SolidColorBrush Brush {
			get { return (SolidColorBrush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		public SolidColorBrush MaxPointBrush {
			get { return (SolidColorBrush)GetValue(MaxPointBrushProperty); }
			set { SetValue(MaxPointBrushProperty, value); }
		}
		public SolidColorBrush MinPointBrush {
			get { return (SolidColorBrush)GetValue(MinPointBrushProperty); }
			set { SetValue(MinPointBrushProperty, value); }
		}
		public SolidColorBrush StartPointBrush {
			get { return (SolidColorBrush)GetValue(StartPointBrushProperty); }
			set { SetValue(StartPointBrushProperty, value); }
		}
		public SolidColorBrush EndPointBrush {
			get { return (SolidColorBrush)GetValue(EndPointBrushProperty); }
			set { SetValue(EndPointBrushProperty, value); }
		}
		public SolidColorBrush NegativePointBrush {
			get { return (SolidColorBrush)GetValue(NegativePointBrushProperty); }
			set { SetValue(NegativePointBrushProperty, value); }
		}
		public SparklinePointCollection Points { 
			get { return (SparklinePointCollection)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); } 
		}
		public bool AutoRange {
			get { return (bool)GetValue(AutoRangeProperty); }
			set { SetValue(AutoRangeProperty, value); }
		}
		public Range ArgumentRange {
			get { return (Range)GetValue(ArgumentRangeProperty); }
			set { SetValue(ArgumentRangeProperty, value); }
		}
		public Range ValueRange {
			get { return (Range)GetValue(ValueRangeProperty); }
			set { SetValue(ValueRangeProperty, value); }
		}
		protected internal abstract bool ActualShowNegativePoint { get; }
		[Browsable(false)]
		public abstract SparklineViewType Type { get; }
		public SparklineControl() {
			DefaultStyleKey = typeof(SparklineControl);
			rangeDirector = new RangeDirector(this); 
			painter = CreatePainter();
			SparklinePointCollection collection = new SparklinePointCollection();
			actualPoints = collection;
			SetValue(PointsProperty, collection);
			SetValue(ArgumentRangeProperty, new Range(this) { Limit1 = 0, Limit2 = 0, Auto = true });
			SetValue(ValueRangeProperty, new Range(this) { Limit1 = 0, Limit2 = 0, Auto = true });
			collection.CollectionChanged += OnItemsCollectionChanged;
			randomPoints = GenerateRandomValues();
			argumentComparer = new SparklinePointArgumentComparer();
			ClipToBounds = true;
		}
		#region ISparklineRangeContainer
		void ISparklineRangeContainer.RaiseArgumentRangeChanged(SparklineRangeChangedEventArgs e) {
			if (SparklineArgumentRangeChanged != null)
				SparklineArgumentRangeChanged(this, e);
		}
		void ISparklineRangeContainer.RaiseValueRangeChanged(SparklineRangeChangedEventArgs e) {
			if (SparklineValueRangeChanged != null)
				SparklineValueRangeChanged(this, e);
		}
		#endregion
		List<SparklinePoint> GetSortedByArgumentCollection(IList<SparklinePoint> data) {
			if (data == null)
				return null;
			List<SparklinePoint> sortedByArgument = new List<SparklinePoint>();
			sortedByArgument.AddRange(data);
			sortedByArgument.Sort(argumentComparer);
			return sortedByArgument;
		}
		void OnHighlightMaxPointChanged(bool highlightMaxPoint) {
			this.highlightMaxPoint = highlightMaxPoint;
			PropertyChanged();
		}
		void OnHighlightMinPointChanged(bool highlightMinPoint) {
			this.highlightMinPoint = highlightMinPoint;
			PropertyChanged();
		}
		void OnHighlightStartPointChanged(bool highlightStartPoint) {
			this.highlightStartPoint = highlightStartPoint;
			PropertyChanged();
		}
		void OnHighlightEndPointChanged(bool highlightEndPoint) {
			this.highlightEndPoint = highlightEndPoint;
			PropertyChanged();
		}
		void OnBrushChanged(SolidColorBrush brush) {
			this.brush = brush;
			PropertyChanged();
		}
		void OnMaxPointBrushChanged(SolidColorBrush maxPointBrush) {
			this.maxPointBrush = maxPointBrush;
			PropertyChanged();
		}
		void OnMinPointBrushChanged(SolidColorBrush minPointBrush) {
			this.minPointBrush = minPointBrush;
			PropertyChanged();
		}
		void OnStartPointBrushChanged(SolidColorBrush startPointBrush) {
			this.startPointBrush = startPointBrush;
			PropertyChanged();
		}
		void OnEndPointBrushChanged(SolidColorBrush endPointBrush) {
			this.endPointBrush = endPointBrush;
			PropertyChanged();
		}
		void OnNegativePointBrushChanged(SolidColorBrush negativePointBrush) {
			this.negativePointBrush = negativePointBrush;
			PropertyChanged();
		}
		void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			extremeIndexes = new ExtremePointIndexes(ActualPoints);
			if (SparklinePointsChanged != null)
				SparklinePointsChanged(this, new EventArgs());			
			CalculateRanges();
			PropertyChanged();
		}
		void OnPointsPropertyChanged(SparklinePointCollection sparklineItemCollection) {
			if (actualPoints != null)
				actualPoints.CollectionChanged -= OnItemsCollectionChanged;
			actualPoints = sparklineItemCollection;
			if (sparklineItemCollection == null)
				actualPoints = new SparklinePointCollection();
			actualPoints.CollectionChanged += OnItemsCollectionChanged;
			OnItemsCollectionChanged(this, null);
		}
		void OnAutoRangeChanged(bool autoRange) {
			PropertyChanged();
		}
		void OnArgumentRangeChanged(Range range) {
			((IRangeContainer)this).OnRangeChanged();
		}
		void OnValueRangeChanged(Range range) {
			((IRangeContainer)this).OnRangeChanged();
		}
		void IRangeContainer.OnRangeChanged() {
			CalculateRanges();
			PropertyChanged();
		}
		Bounds GetDrawingBounds(Bounds bounds) {
			var markersPadding = GetMarkersPadding();
			int x = bounds.X + markersPadding.Left;
			int y = bounds.Y + markersPadding.Top;
			int width = bounds.Width - markersPadding.Left - markersPadding.Right;
			int height = bounds.Height - markersPadding.Top - markersPadding.Bottom;
			Bounds drawingBounds = new Bounds(x, y, width, height);
			return drawingBounds;
		}
		SparklinePointCollection GenerateRandomValues() {
			Random random = new Random(0);
			SparklinePointCollection values = new SparklinePointCollection();
			for (int i = 0; i < 10; i++)
				values.Add(new SparklinePoint(i, random.NextDouble() - 0.5));
			return values;
		}
		void CalculateRanges() {
			if (extremeIndexes != null && ArgumentRange != null && ValueRange != null)
				rangeDirector.CalculateRanges(ActualPoints, extremeIndexes, ArgumentRange.InternalRange, ValueRange.InternalRange);
		}
		protected virtual System.Windows.Forms.Padding GetMarkersPadding() {
			return new System.Windows.Forms.Padding();
		}
		protected abstract string GetViewName();
		protected internal abstract BaseSparklinePainter CreatePainter();
		protected void PropertyChanged() {
			InvalidateVisual();
		}
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			Bounds bounds = new Bounds(0, 0, (int)base.ActualWidth, (int)base.ActualHeight);
			Bounds drawingBounds = GetDrawingBounds(bounds);
			if (extremeIndexes != null) {
				SparklineMappingBase mapping = SparklineMappingBase.CreateMapping(this.Type, ActualPoints, drawingBounds, rangeDirector.ArgumentRange, rangeDirector.ValueRange);
				painter.Initialize(ActualPoints, this, mapping, extremeIndexes);
				painter.Draw(drawingContext);
			}
		}
		public virtual void Assign(SparklineControl view) {
			if (view != null) {
				highlightMaxPoint = view.highlightMaxPoint;
				highlightMinPoint = view.highlightMinPoint;
				highlightStartPoint = view.highlightStartPoint;
				highlightEndPoint = view.highlightEndPoint;
				brush = view.brush;
				maxPointBrush = view.maxPointBrush;
				minPointBrush = view.minPointBrush;
				startPointBrush = view.startPointBrush;
				endPointBrush = view.endPointBrush;
				negativePointBrush = view.negativePointBrush;
			}
		}
		public override string ToString() {
			return GetViewName();
		}
	}
	public class RangeDirector {
		SparklinePointCollection pointsSortedByArgument;
		InternalRange userArgumentRange; 
		InternalRange userValueRange;
		ExtremePointIndexes extremeIndexes;
		ISparklineRangeContainer container;
		public InternalRange ArgumentRange { get; private set; }
		public InternalRange ValueRange { get; private set; }
		public RangeDirector(ISparklineRangeContainer container) {
			this.container = container;
		}
		public RangeDirector() { }
		InternalRange CalculateValueAutoRange() {
			if (pointsSortedByArgument.Count == 0)
				return new InternalRange(0, 0);
			SparklinePoint fakeMinPoint = new SparklinePoint(userArgumentRange.Min, 0);
			SparklinePoint fakeMaxPoint = new SparklinePoint(userArgumentRange.Max, 0);
			SortedSparklinePointCollection sortedCollection;
			if (pointsSortedByArgument is SortedSparklinePointCollection)
				sortedCollection = (SortedSparklinePointCollection)pointsSortedByArgument;
			else {
				sortedCollection = new SortedSparklinePointCollection(new SparklinePointArgumentComparer(true));
				foreach (SparklinePoint point in pointsSortedByArgument)
					sortedCollection.Add(point);
				}
			int minIndex = sortedCollection.BinarySearch(fakeMinPoint);
			if (minIndex < 0)
				minIndex = ~minIndex;
			if (minIndex >= pointsSortedByArgument.Count)
				return new InternalRange(0, 0);
			int maxIndex = sortedCollection.BinarySearch(fakeMaxPoint);
			if (maxIndex < 0) {
				maxIndex = ~maxIndex;
				if (maxIndex == pointsSortedByArgument.Count)
					maxIndex = maxIndex - 1;
				if (maxIndex == 0)
					return new InternalRange(0, 0);
			}
			InternalRange range = new InternalRange(pointsSortedByArgument[minIndex].Value, pointsSortedByArgument[maxIndex].Value);
			for (int i = minIndex; i <= maxIndex; i++) {
				range.Min = Math.Min(range.Min, pointsSortedByArgument[i].Value);
				range.Max = Math.Max(range.Max, pointsSortedByArgument[i].Value);
			}
			return range;
		}
		InternalRange CalculateArgumentAutoRange() {
			int minIndex = -1;
			for (int i = 0; i < pointsSortedByArgument.Count; i++)
				if (pointsSortedByArgument[i].Value >= userValueRange.Min && pointsSortedByArgument[i].Value <= userValueRange.Max) {
					minIndex = i;
					break;
				}
			if (minIndex == -1)
				return new InternalRange(0, 0);
			int maxIndex = -1;
			for (int i = pointsSortedByArgument.Count - 1; i >= 0; i--)
				if (pointsSortedByArgument[i].Value >= userValueRange.Min && pointsSortedByArgument[i].Value <= userValueRange.Max) {
					maxIndex = i;
					break;
				}
			return new InternalRange(pointsSortedByArgument[minIndex].Argument, pointsSortedByArgument[maxIndex].Argument);
		}
		public void CalculateRanges(SparklinePointCollection pointsSortedByArgument, ExtremePointIndexes extremePointIndexes, InternalRange userArgumentRange, InternalRange userValueRange) {
			Initialize(pointsSortedByArgument, extremePointIndexes, userArgumentRange, userValueRange);
			InternalRange argumentRange, valueRange;
			CalculateRanges(out argumentRange, out valueRange);
			if (!argumentRange.IsEqual(ArgumentRange))
				RaiseRangeChanged(true, argumentRange);
			if (!valueRange.IsEqual(ValueRange))
				RaiseRangeChanged(false, valueRange);
			ArgumentRange = argumentRange;
			ValueRange = valueRange;
		}
		void RaiseRangeChanged(bool isArgumentRangeChanged, InternalRange newRange) {
			if (container == null)
				return;
			if (isArgumentRangeChanged)
				container.RaiseArgumentRangeChanged(new SparklineRangeChangedEventArgs(newRange));
			else
				container.RaiseValueRangeChanged(new SparklineRangeChangedEventArgs(newRange));
		}
		void CalculateRanges(out InternalRange argumentRange, out InternalRange valueRange) {
			if (extremeIndexes.IsEmpty) {
				argumentRange = new InternalRange(0, 0);
				valueRange = new InternalRange(0, 0);
				argumentRange.SetScaleTypes(SparklineScaleType.Unknown);
				valueRange.SetScaleTypes(SparklineScaleType.Unknown);
			}
			else {
				if (userArgumentRange.Auto && userValueRange.Auto) {
					argumentRange = new InternalRange(pointsSortedByArgument[extremeIndexes.Start].Argument, pointsSortedByArgument[extremeIndexes.End].Argument);
					valueRange = new InternalRange(pointsSortedByArgument[extremeIndexes.Min].Value, pointsSortedByArgument[extremeIndexes.Max].Value);
					argumentRange.SetScaleTypes(pointsSortedByArgument.ArgumentScaleType);
					valueRange.SetScaleTypes(pointsSortedByArgument.ValueScaleType);
				}
				else if (userArgumentRange.Auto) {
					argumentRange = CalculateArgumentAutoRange();
					valueRange = userValueRange;
					argumentRange.SetScaleTypes(pointsSortedByArgument.ArgumentScaleType);
				}
				else if (userValueRange.Auto) {
					argumentRange = userArgumentRange;
					valueRange = CalculateValueAutoRange();
					valueRange.SetScaleTypes(pointsSortedByArgument.ValueScaleType);
				}
				else {
					argumentRange = userArgumentRange;
					valueRange = userValueRange;
				}
			}
			argumentRange.Auto = userArgumentRange.Auto;
			valueRange.Auto = userValueRange.Auto;
		}
		void Initialize(SparklinePointCollection pointsSortedByArgument, ExtremePointIndexes extremePointIndexes, InternalRange userArgumentRange, InternalRange userValueRange) {
			this.pointsSortedByArgument = pointsSortedByArgument;
			this.userArgumentRange = userArgumentRange;
			this.userValueRange = userValueRange;
			this.extremeIndexes = extremePointIndexes;
		}
	}
	public class SparklinePoint  {
		public double Argument { get; set; }
		public double Value { get; set; }
		public SparklineScaleType ArgumentScaleType { get; set; }
		public SparklineScaleType ValueScaleType { get; set; }
		public SparklinePoint() { }
		public SparklinePoint(double argument, double value) {
			this.Argument = argument;
			this.Value = value;
			this.ArgumentScaleType = SparklineScaleType.Unknown;
			this.ValueScaleType = SparklineScaleType.Unknown;
		}
		public SparklinePoint(double argument, double value, SparklineScaleType argumentScaleType, SparklineScaleType valueScaleType) {
			this.Argument = argument;
			this.Value = value;
			this.ArgumentScaleType = argumentScaleType;
			this.ValueScaleType = valueScaleType;
		}
	}
	public class Range : DXFrameworkContentElement{
		public static readonly DependencyProperty Limit1Property;
		public static readonly DependencyProperty Limit2Property;
		public static readonly DependencyProperty AutoProperty;
		static Range() {
			Type ownerType = typeof(Range);
			Limit1Property = DependencyProperty.Register("Limit1", typeof(object), ownerType,
					new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (o, args) => ((Range)o).OnLimit1Changed(args.NewValue)));
			Limit2Property = DependencyProperty.Register("Limit2", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (o, args) => ((Range)o).OnLimit2Changed(args.NewValue)));
			AutoProperty = DependencyProperty.Register("Auto", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, (o, args) => ((Range)o).OnAutoChanged((bool)args.NewValue)));
		}
		IRangeContainer container;
		object limit1;
		object limit2;
		internal InternalRange InternalRange { get; private set; }
		public object Limit1 {
			get { return GetValue(Limit1Property); }
			set { SetValue(Limit1Property, value); }
		}
		public object Limit2 {
			get { return GetValue(Limit2Property); }
			set { SetValue(Limit2Property, value); }
		}
		public bool Auto {
			get { return (bool)GetValue(AutoProperty); }
			set { SetValue(AutoProperty, value); }
		}
		public Range() : this(null) { }
		public Range(IRangeContainer container) {
			this.container = container;
			InternalRange = new InternalRange(0, 0, Auto);
		}
		void OnLimit1Changed(object limit1) {
			this.limit1 = limit1;
			SetInternalRange();
			PropertyChanged();
		}
		void OnLimit2Changed(object limit2) {
			this.limit2 = limit2;
			SetInternalRange();
			PropertyChanged();
		}
		void OnAutoChanged(bool auto) {
			SetInternalRange();
			PropertyChanged();
		}
		void PropertyChanged() {
			if (container != null)
				container.OnRangeChanged();
		}
		void SetInternalRange() {
			InternalRange.Auto = Auto;
			if (limit1 == null || limit2 == null) {
				InternalRange.IsSet = false;
				InternalRange.Min = 0;
				InternalRange.Max = 0;
			}
			else {
				SparklineScaleType limit1Type, limit2Type;
				double? internalLimit1 = SparklineMathUtils.ConvertToDouble(limit1, out limit1Type);
				double? internalLimit2 = SparklineMathUtils.ConvertToDouble(limit2, out limit2Type);
				if (internalLimit1 == null || internalLimit2 == null) {
					InternalRange.IsSet = false;
					InternalRange.Min = 0;
					InternalRange.Max = 0;
					InternalRange.ScaleTypeMin = limit1Type;
					InternalRange.ScaleTypeMax = limit2Type;
				}
				else {
					InternalRange.IsSet = true;
					if ((double)internalLimit1 < (double)internalLimit2) {
						InternalRange.Min = (double)internalLimit1;
						InternalRange.Max = (double)internalLimit2;
						InternalRange.ScaleTypeMin = limit1Type;
						InternalRange.ScaleTypeMax = limit2Type;
					}
					else {
						InternalRange.Min = (double)internalLimit2;
						InternalRange.Max = (double)internalLimit1;
						InternalRange.ScaleTypeMin = limit2Type;
						InternalRange.ScaleTypeMax = limit1Type;
					}
				}
			}
		}
		internal void SetContainer(IRangeContainer container) {
			this.container = container;
		}
	}
	public class InternalRange {
		public bool Auto { get; set; }
		public double Min { get; set; }
		public double Max { get; set; }
		public bool IsSet { get; set; }
		public double CorrectionMin { get; set; }
		public double CorrectionMax { get; set; }
		public double ActualMin { get { return Min + CorrectionMin; } }
		public double ActualMax { get { return Max + CorrectionMax; } }
		public SparklineScaleType ScaleTypeMin { get; set; }
		public SparklineScaleType ScaleTypeMax { get; set; }
		public InternalRange(double min, double max) {
			Min = min;
			Max = max;
			IsSet = false;
			CorrectionMin = 0;
			CorrectionMax = 0;
		}
		public InternalRange(double min, double max, bool auto) : this (min, max) {
			Auto = auto;
		}
		public bool ContainsValue(double value) {
			return value >= Min && value <= Max;
		}
		public bool IsEqual(InternalRange range) {
			if (range != null && this.Min == range.Min && this.Max == range.Max && this.Auto == range.Auto)
				return true;
			return false;
		}
		public void SetScaleTypes(SparklineScaleType scaleType) {
			ScaleTypeMin = ScaleTypeMax = scaleType;
		}
	}
	public class SparklinePointArgumentComparer : Comparer<SparklinePoint> {
		bool isAsceding;
		public SparklinePointArgumentComparer() : this(true) { }
		public SparklinePointArgumentComparer(bool isAsceding) {
			this.isAsceding = isAsceding;
		}
		public override int Compare(SparklinePoint pointInArray, SparklinePoint newРoint) {
			if (newРoint == null)
				throw new ArgumentException();
			if (pointInArray == null)
				return 1;
			if (isAsceding)
				return pointInArray.Argument.CompareTo(newРoint.Argument);
			else
				return newРoint.Argument.CompareTo(pointInArray.Argument);
		}
	}
}
