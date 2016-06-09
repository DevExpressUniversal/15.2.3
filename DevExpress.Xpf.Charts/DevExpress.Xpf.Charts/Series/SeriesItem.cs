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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class SeriesItem : NotifyPropertyChangedObject, ISeriesItem {
		#region internal class 
		class PointDataCollection : Collection<SeriesPointData> {
			readonly Series series;
			readonly List<SeriesPointItem> allItems = new List<SeriesPointItem>();
			readonly ObservableCollection<SeriesPointItem> visibleItems;
			public List<SeriesPointItem> AllPointItems { get { return allItems; } }
			int GetStartIndexInItemCollection(int pointDataIndex, int itemCount) {
				return pointDataIndex * itemCount;
			}
			public PointDataCollection(Series series, ObservableCollection<SeriesPointItem> visibleItems) {
				this.series = series;
				this.visibleItems = visibleItems;
			}
			protected override void ClearItems() {
				allItems.Clear();
				visibleItems.Clear();
				base.ClearItems();
			}
			protected override void InsertItem(int index, SeriesPointData item) {
				base.InsertItem(index, item);
				int startIndex = GetStartIndexInItemCollection(index, item.PointItems.Count);
				for (int i = 0; i < item.PointItems.Count; i++) {
					allItems.Insert(startIndex + i, item.PointItems[i]);
					SubscribeToPropertyChangedEvent(item.PointItems[i]);
					if (item.PointItems[i].ShowBehindAxes)
						visibleItems.Add(item.PointItems[i]);
				}
			}
			protected override void RemoveItem(int index) {
				int startIndex = GetStartIndexInItemCollection(index, this[index].PointItems.Count);
				for (int i = 0; i < this[index].PointItems.Count; i++) {
					UnsubscribeToPropertyChangedEvent(this[index].PointItems[i]);
					if (allItems[startIndex].ShowBehindAxes)
						visibleItems.Remove(allItems[startIndex]);
					allItems.RemoveAt(startIndex);
				}
				base.RemoveItem(index);
			}
			protected override void SetItem(int index, SeriesPointData item) {
				base.SetItem(index, item);
				int startIndex = GetStartIndexInItemCollection(index, item.PointItems.Count);
				for (int i = 0; i < item.PointItems.Count; i++) {
					UnsubscribeToPropertyChangedEvent(allItems[startIndex + i]);
					if (allItems[startIndex + i].ShowBehindAxes)
						visibleItems.Remove(allItems[startIndex + i]);
					allItems[startIndex + i] = item.PointItems[i];
					SubscribeToPropertyChangedEvent(allItems[startIndex + i]);
					if (allItems[startIndex + i].ShowBehindAxes)
						visibleItems.Add(allItems[startIndex + i]);
				}
			}
			public void Update() {
				foreach (SeriesPointItem item in allItems)
					item.Update();
			}
			void PointsData_PropertyChanged(object sender, PropertyChangedEventArgs e) {
				if (e.PropertyName != "ShowBehindOfAxes")
					return;
				SeriesPointItem seriesPointItem = sender as SeriesPointItem;
				if (seriesPointItem.ShowBehindAxes)
					visibleItems.Add(seriesPointItem);
				else
					visibleItems.Remove(seriesPointItem);
			}
			void SubscribeToPropertyChangedEvent(SeriesPointItem item) {
				item.PropertyChanged += new PropertyChangedEventHandler(PointsData_PropertyChanged);
			}
			void UnsubscribeToPropertyChangedEvent(SeriesPointItem item) {
				item.PropertyChanged -= new PropertyChangedEventHandler(PointsData_PropertyChanged);
			}
		}	
		#endregion
		readonly Series series;
		readonly PointDataCollection seriesPointDataList;		
		DrawOptions drawOptions;
		string legendText = string.Empty;
		ObservableCollection<SeriesPointItem> visiblePointItems;
		PointItemsDictionary pointItemsDictionary;
		IRefinedSeries refinedSeries;
		internal IList<SeriesPointData> SeriesPointDataList { get { return seriesPointDataList; } }
		internal IRefinedSeries RefinedSeries { get { return refinedSeries; } }
		internal DrawOptions DrawOptions { get { return drawOptions; } }
		internal bool Visible { get { return refinedSeries != null; } }
		internal IList<SeriesPointItem> AllPointItems { get { return seriesPointDataList.AllPointItems; } }
		public Series Series { get { return series; } }
		[NonTestableProperty]
		public ObservableCollection<SeriesPointItem> VisiblePointItems {
			get { return visiblePointItems; }
			private set { 
				visiblePointItems = value;
				OnPropertyChanged("VisiblePointItems");
			}
		}
		public SeriesItem(Series series) {
			this.series = series;
			VisiblePointItems = new ObservableCollection<SeriesPointItem>();
			seriesPointDataList = new PointDataCollection(series, VisiblePointItems);
		}
		#region ISeriesItem implementation
		DrawOptions ISeriesItem.DrawOptions {
			get { return drawOptions; }
			set { drawOptions = value; }
		}
		string ISeriesItem.LegendText {
			get { return legendText; }
			set { legendText = value; }
		}
		bool ISeriesItem.ShouldUpdate { get { return true; } }
		bool ISeriesItem.HasPoints { get { return series != null && series.Points.Count > 0; } }
		IEnumerable<ISeriesPointData> ISeriesItem.PointData {
			get {
				foreach (SeriesPointData seriesPointData in SeriesPointDataList)
					yield return seriesPointData;
			}
		}
		void ISeriesItem.Update(IRefinedSeries refinedSeries) {
			this.refinedSeries = refinedSeries;
			if (series == null)
				return;
			if (series.ShouldCalculatePointsData) {
				IList<RefinedPoint> actualPoints = refinedSeries != null ? refinedSeries.Points : null;
				int minIndex = refinedSeries != null ? refinedSeries.MinVisiblePointIndex : -1;
				int maxIndex = refinedSeries != null ? refinedSeries.MaxVisiblePointIndex : -1;
				List<RefinedPoint> pointsInfo = GetActualRefinedPoints(actualPoints, minIndex, maxIndex);
				if (seriesPointDataList.Count < pointsInfo.Count) {
					while (seriesPointDataList.Count < pointsInfo.Count)
						seriesPointDataList.Add(new SeriesPointData(series, null));
				}
				else if (seriesPointDataList.Count > pointsInfo.Count) {
					while (seriesPointDataList.Count > pointsInfo.Count)
						seriesPointDataList.RemoveAt(pointsInfo.Count);
				}
				if (refinedSeries != null) {
					int index = 0;
					for (int i = minIndex; i < maxIndex + 1; i++) {
						if (!actualPoints[i].IsEmpty) {
							seriesPointDataList[index].RefinedPoint = actualPoints[i];
							seriesPointDataList[index].IndexInSeries = i;
							index++;
						}
					}
				}
				seriesPointDataList.Update();
				ChartControl chart = series.Diagram.ChartControl;
				if (chart.ActualCrosshairOptions.HighlightPoints || chart.SelectionMode != ElementSelectionMode.None)
					FillPointItemsDictionary();
			}
			else {
				seriesPointDataList.Clear();
				pointItemsDictionary = null;
			}
		}
		#endregion
		List<RefinedPoint> GetActualRefinedPoints(IList<RefinedPoint> actualPoints, int minIndex, int maxIndex) {
			List<RefinedPoint> pointsInfo = new List<RefinedPoint>();
			if (refinedSeries != null) {
				for (int i = minIndex; i < maxIndex + 1; i++) {
					RefinedPoint point = actualPoints[i];
					if (!point.IsEmpty)
						pointsInfo.Add(point);
				}
			}
			return pointsInfo;
		}
		void FillPointItemsDictionary() {
			pointItemsDictionary = new PointItemsDictionary();
			foreach (SeriesPointData pointData in seriesPointDataList) {
				IInteractiveElement element = pointData.SeriesPoint as IInteractiveElement;
				if (element != null) {
					foreach (SeriesPointItem item in pointData.PointItems)
						item.IsSelected = element.IsSelected;
				}
				foreach (ISeriesPoint point in SeriesPoint.GetSourcePoints(pointData.SeriesPoint))
					pointItemsDictionary.Add(point, pointData.PointItems);
			}
			XYSeries2D xySeries2D = Series as XYSeries2D;
			if (xySeries2D != null)
				xySeries2D.ResetHighlightedItems();
		}
		internal void AddPointItems(ISeriesPoint point, List<SeriesPointItem> pointItems) {
			if (pointItemsDictionary == null)
				pointItemsDictionary = new PointItemsDictionary();
			pointItemsDictionary.Add(point, pointItems);
		}
		internal void RemovePointItems(ISeriesPoint point) {
			if (pointItemsDictionary != null)
				pointItemsDictionary.Remove(point);
		}
		internal SeriesPointData CreatePointData(RefinedPoint point) {
			SeriesPointData pointData = new SeriesPointData(series, point);
			((ISeriesPointData)pointData).Color = DrawOptions.Color;
			((ISeriesPointData)pointData).Opacity = series.GetOpacity();
			return pointData;
		}
		internal SeriesPointData GetPointData(IRefinedSeries refinedSeries, RefinedPoint refinedPoint) {
			int pointIndex = Math.Max(refinedSeries.Points.IndexOf(refinedPoint), 0);
			return pointIndex < seriesPointDataList.Count ? seriesPointDataList[pointIndex] : null;
		}
		internal List<SeriesPointItem> GetPointItems(ISeriesPoint pointInfo) {
			if (pointItemsDictionary != null)
				return pointItemsDictionary[pointInfo];
			return null;
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public class SeriesPointData : ISeriesPointData, IAnimatableElement {
		readonly Series series;
		readonly AnimationProgress pointProgress;
		readonly AnimationProgress labelProgress;
		readonly List<SeriesPointItem> pointItems = new List<SeriesPointItem>();
		RefinedPoint refinedPoint;
		int indexInSeries;
		public AnimationProgress PointProgress { get { return pointProgress; } }
		public AnimationProgress LabelProgress { get { return labelProgress; } }
		public List<SeriesPointItem> PointItems { get { return pointItems; } }
		public RefinedPoint RefinedPoint {
			get { return refinedPoint; }
			set {
				refinedPoint = value;
				RefinedPointChanged();
			}
		}
		public ISeriesPoint SeriesPoint { get { return RefinedPoint.SeriesPoint; } }
		public int IndexInSeries {
			get { return indexInSeries; }
			set { indexInSeries = value; }
		}
		public LegendItem LegendItem { get; set; }
		public SeriesPointData(Series series, RefinedPoint refinedPoint) {
			this.series = series;
			this.refinedPoint = refinedPoint;
			foreach (SeriesPointItem item in series.CreateSeriesPointItems(refinedPoint, this))
				pointItems.Add(item);
			pointProgress = new AnimationProgress(this);
			labelProgress = new AnimationProgress(this);
		}
		#region ISeriesPointItem implementation
		Color ISeriesPointData.Color {
			get { return PointItems[0].PresentationData.PointColor; }
			set {
				foreach (SeriesPointItem pointItem in PointItems)
					pointItem.PresentationData.PointColor = value;
			}
		}
		string ISeriesPointData.LegendText {
			get { return PointItems[0].PresentationData.LegendText; }
			set {
				foreach (SeriesPointItem pointItem in PointItems)
					pointItem.PresentationData.LegendText = value;
			}
		}
		string[] ISeriesPointData.LabelsTexts {
			get {
				int count = pointItems.Count;
				string[] labelsTexts = new string[count];
				for (int i = 0; i < count; i++)
					labelsTexts[i] = pointItems[i].PresentationData.LabelText;
				return labelsTexts;
			}
			set {
				foreach (SeriesPointItem pointItem in PointItems)
					pointItem.PresentationData.LabelText = null;
				if (value.Length == 2) {
					foreach (SeriesPointItem pointItem in PointItems) {
						if (pointItem.ValueLevel == RangeValueLevel.Value1)
							pointItem.PresentationData.LabelText = value[0];
						else if(pointItem.ValueLevel == RangeValueLevel.Value2)
							pointItem.PresentationData.LabelText = value[1];
					}
				}
				else {
					foreach (SeriesPointItem pointItem in PointItems)
						if (pointItem.ValueLevel == series.GetValueLevelForLabel(refinedPoint)) {
							pointItem.PresentationData.LabelText = value[0];
							return;
						}
				}
			}
		}
		double ISeriesPointData.Opacity {
			get { return PointItems[0].PresentationData.Opacity; }
			set {
				foreach (SeriesPointItem pointItem in PointItems)
					pointItem.PresentationData.Opacity = value;
			}
		}
		#endregion
		#region IAnimatableElement implementation
		ChartAnimationMode IAnimatableElement.AnimationMode { get { return series != null ? ((IAnimatableElement)series).AnimationMode : ChartAnimationMode.Disabled; } }
		AnimationState IAnimatableElement.AnimationState { get { return series != null ? ((IAnimatableElement)series).AnimationState : AnimationState.Completed; } }
		AnimationAutoStartMode IAnimatableElement.AnimationAutoStartMode { get { return series != null ? series.AnimationAutoStartMode : AnimationAutoStartMode.SetFinalState; } }
		void IAnimatableElement.ProgressChanged(AnimationProgress source) {
			if (source == pointProgress)
				PointProgressChanged();
			else if (source == labelProgress)
				LabelProgressChanged();
		}
		#endregion
		void PointProgressChanged() {
			SeriesPointAnimationBase animation = GetActualAnimation();
			if (animation == null)
				return;
			foreach (SeriesPointItem pointItem in pointItems) {
				if (animation.ShouldAnimateSeriesPointLayout)
					CommonUtils.InvalidateMeasure(pointItem.PointItemPresentation);
				if (pointItem.PointItemPresentation != null && animation.ShouldAnimateSeriesPointOpacity(series))
					pointItem.PointItemPresentation.InvalidateMeasure();
				if (series != null && animation.ShouldAnimateSeriesLabelOpacity(series) && pointItem.LabelItem != null && pointItem.LabelItem.Presentation != null)
					CommonUtils.InvalidateMeasure(pointItem.LabelItem.Presentation);
			}
			IXYSeriesView xyView = series as IXYSeriesView;
		}
		void LabelProgressChanged() {
			SeriesPointAnimationBase animation = GetActualAnimation();
			if (animation == null)
				return;
			foreach (SeriesPointItem pointItem in pointItems)
				if (pointItem.PointItemPresentation != null)
					pointItem.PointItemPresentation.InvalidateArrange();
		}
		void RefinedPointChanged() {
			if (refinedPoint == null)
				return;
			foreach (SeriesPointItem pointItem in pointItems) {
				pointItem.SeriesPoint = DevExpress.Xpf.Charts.SeriesPoint.GetSeriesPoint(this.SeriesPoint);
			}
		}
		SeriesPointAnimationBase GetActualAnimation() {
			return series != null ? series.GetActualPointAnimation() : null;
		}
		public Color? GetCrosshairColor(bool colorEach) {
			if (colorEach)
				return PointItems[0].PresentationData.CrosshairPointColor;
			else if (PointItems[0].SeriesPoint.Brush != null)
				return PointItems[0].SeriesPoint.Brush.Color;
			return null;
		}
		internal double GetLabelOpacity() {
			SeriesPointAnimationBase animation = GetActualAnimation();
			return animation != null && animation.ShouldAnimateSeriesLabelOpacity(series) ? labelProgress.ActualProgress : 1.0;
		}
		internal double GetPointOpacity() {
			SeriesPointAnimationBase animation = GetActualAnimation();
			return animation != null && animation.ShouldAnimateSeriesPointOpacity(series) ? pointProgress.ActualProgress : 1.0;
		}		
	}
}
