#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardCommon.Native {
	public abstract class ChartSeriesConverter {
		string BaseImagePath = "ChartSeriesTypes";
		public abstract string Caption { get; }
		public abstract string ImagePath { get; }
		public virtual string DragAreaImagePath { get { return ImagePath; } }
		public abstract Type SupportedSeries { get; }
		public string SmallGalleryImagePath { get { return string.Format("{0}.{1}_16x16", BaseImagePath, ImagePath); } }
		public string GalleryImagePath { get { return string.Format("{0}.{1}_32x32", BaseImagePath, ImagePath); } }
		public string SmallGalleryImagePathBlack { get { return string.Format("{0}.GrayScale.{1}_16x16", BaseImagePath, ImagePath); } }
		public string GalleryImagePathBlack { get { return string.Format("{0}.GrayScale.{1}_32x32", BaseImagePath, ImagePath); } }
		public string DragAreaSmallImagePath { get { return string.Format("Groups.{0}", string.Format(ChartSeries.BaseImagePath, DragAreaImagePath)); } }
		protected ChartSeriesConverter() {
		}
		protected IList<Measure> FillExistingMeasures(ChartSeries series, ICollection<DataItemDescription> measureDescriptions) {
			List<Measure> remainMeasures = new List<Measure>();
			if(measureDescriptions.Count > 0) {
				List<string> measureNames = new List<string>(series.DataItemKeys);
				foreach(DataItemDescription description in measureDescriptions) {
					string name = description.Name;
					Measure measure = (Measure)description.DataItem;
					if(measureNames.Contains(name)) {
						series.SetDataItem(name, measure);
						measureNames.Remove(name);
					}
					else
						remainMeasures.Add(measure);
				}
				int remainCount = Math.Min(measureNames.Count, remainMeasures.Count);
				for(int i = 0; i < remainCount; i++) {
					series.SetDataItem(measureNames[0], remainMeasures[0]);
					measureNames.RemoveAt(0);
					remainMeasures.RemoveAt(0);
				}
			}
			return remainMeasures;
		}
		public abstract bool IsMatch(ChartSeries series);
		public abstract IList<ChartSeries> PrepareSeries(ICollection<DataItemDescription> measureDescriptions);
		public abstract ChartSeries CreateSeries();
		public IList<ChartSeries> PrepareSeries(ChartSeries oldSeries) {
			List<DataItemDescription> measureDescriptions = new List<DataItemDescription>();
			foreach (string measureKey in oldSeries.DataItemKeys) {
				Measure oldMeasure = oldSeries.GetMeasure(measureKey);
				if (oldMeasure != null) {
					measureDescriptions.Add(new DataItemDescription(measureKey, measureKey, oldMeasure.Clone()));
				}
			}
			List<ChartSeries> newSeries = (List<ChartSeries>)PrepareSeries(measureDescriptions);
			foreach(ChartSeries series in newSeries){
				series.AssignOptions(oldSeries);
			}
			return newSeries;
		}
	}
	public class ChartSimpleSeriesConverter : ChartSeriesConverter {
		readonly SimpleSeriesType seriesType;
		public override string Caption { get { return SimpleSeriesTypeRepository.GetCaption(seriesType); } }
		public override string ImagePath { get { return seriesType.ToString(); } }
		public override Type SupportedSeries { get { return typeof(SimpleSeries); } }
		public SimpleSeriesType SeriesType { get { return seriesType; } }
		public ChartSimpleSeriesConverter(SimpleSeriesType seriesType) {
			this.seriesType = seriesType;
		}
		public override bool IsMatch(ChartSeries series) {
			SimpleSeries simpleSeries = series as SimpleSeries;
			return simpleSeries != null && simpleSeries.SeriesType == seriesType;
		}
		public override IList<ChartSeries> PrepareSeries(ICollection<DataItemDescription> measureDescriptions) {
			List<ChartSeries> series = new List<ChartSeries>();
			if (measureDescriptions.Count > 0)
				foreach(DataItemDescription description in measureDescriptions)
					series.Add(new SimpleSeries((Measure)description.DataItem, seriesType));
			else
				series.Add(CreateSeries());
			return series;
		}
		public override ChartSeries CreateSeries() {
			return  new SimpleSeries(seriesType);
		}	  
	}
	public class ChartRangeSeriesConverter : ChartSeriesConverter {
		readonly RangeSeriesType seriesType;
		public override string Caption { get { return RangeSeriesTypeRepository.GetCaption(seriesType); } }
		public override string ImagePath { get { return seriesType.ToString(); } }
		public override Type SupportedSeries { get { return typeof(RangeSeries); } }
		public RangeSeriesType SeriesType { get { return seriesType; } }
		public ChartRangeSeriesConverter(RangeSeriesType seriesType) {
			this.seriesType = seriesType;
		}
		public override bool IsMatch(ChartSeries series) {
			RangeSeries rangeSeries = series as RangeSeries;
			return rangeSeries != null && rangeSeries.SeriesType == seriesType;
		}
		public override IList<ChartSeries> PrepareSeries(ICollection<DataItemDescription> measureDescriptions) {
			List<ChartSeries> series = new List<ChartSeries>();
			RangeSeries rangeSeries = new RangeSeries(seriesType);
			IList<Measure> remainMeasures = FillExistingMeasures(rangeSeries, measureDescriptions);
			series.Add(rangeSeries);
			int measuresCount = remainMeasures.Count;
			int count = measuresCount / 2 + measuresCount % 2;
			if (count > 0)
				for (int i = 0, measureIndex = 0; i < count; i++) {
					Measure measure1 = remainMeasures[measureIndex++];
					Measure measure2 = measureIndex == measuresCount ? null : remainMeasures[measureIndex++];
					series.Add(new RangeSeries(measure1, measure2, seriesType));
				}
			return series;
		}
		public override ChartSeries CreateSeries() {
			return new RangeSeries(seriesType);
		}
	}
	public class ChartWeightedSeriesConverter : ChartSeriesConverter {
		public override string Caption { get { return DashboardLocalizer.GetString(DashboardStringId.SeriesTypeBubble); } }
		public override string ImagePath { get { return WeightedSeries.ImagePath; } }
		public override Type SupportedSeries { get { return typeof(WeightedSeries); } }
		public ChartWeightedSeriesConverter() {
		}
		public override bool IsMatch(ChartSeries series) { 
			return series is WeightedSeries; 
		}
		public override IList<ChartSeries> PrepareSeries(ICollection<DataItemDescription> measureDescriptions) {
			List<ChartSeries> series = new List<ChartSeries>();
			WeightedSeries weightedSeries = new WeightedSeries();
			IList<Measure> remainMeasures = FillExistingMeasures(weightedSeries, measureDescriptions);
			series.Add(weightedSeries);
			int measuresCount = remainMeasures.Count;
			int count = measuresCount / 2 + measuresCount % 2;
			if (count > 0)
				for (int i = 0, measureIndex = 0; i < count; i++) {
					Measure measure1 = remainMeasures[measureIndex++];
					Measure measure2 = measureIndex == measuresCount ? null : remainMeasures[measureIndex++];
					series.Add(new WeightedSeries(measure1, measure2));
				}
			return series;
		}
		public override ChartSeries CreateSeries() {
			return new WeightedSeries();
	   }
	}
	public class ChartHighLowCloseSeriesConverter : ChartSeriesConverter {
		public override string Caption { get { return DashboardLocalizer.GetString(DashboardStringId.SeriesTypeHighLowClose); } }
		public override string ImagePath { get { return HighLowCloseSeries.ImagePath; } }
		public override Type SupportedSeries { get { return typeof(HighLowCloseSeries); } }
		public ChartHighLowCloseSeriesConverter() {
		}
		public override bool IsMatch(ChartSeries series) {
			return series is HighLowCloseSeries; 
		}
		public override IList<ChartSeries> PrepareSeries(ICollection<DataItemDescription> measureDescriptions) {
			List<ChartSeries> series = new List<ChartSeries>();
			HighLowCloseSeries highLowCloseSeries = new HighLowCloseSeries();
			IList<Measure> remainMeasures = FillExistingMeasures(highLowCloseSeries, measureDescriptions);
			series.Add(highLowCloseSeries);
			int measuresCount = remainMeasures.Count;
			int count = measuresCount / 3;
			if (measuresCount % 3 > 0)
				count++;
			if (count > 0)
				for (int i = 0, measureIndex = 0; i < count; i++) {
					Measure measure1 = remainMeasures[measureIndex++];
					Measure measure2 = measureIndex == measuresCount ? null : remainMeasures[measureIndex++];
					Measure measure3 = measureIndex == measuresCount ? null : remainMeasures[measureIndex++];
					series.Add(new HighLowCloseSeries(measure1, measure2, measure3));
				}
			return series;
		}
		public override ChartSeries CreateSeries() {
			return new HighLowCloseSeries();
		}
	}
	public class ChartOpenHighLowCloseSeriesConverter : ChartSeriesConverter {
		readonly OpenHighLowCloseSeriesType seriesType;
		public override string Caption { 
			get { 
				return String.Format("{0} ({1})", DashboardLocalizer.GetString(DashboardStringId.SeriesTypeOpenHighLowClose), OpenHighLowCloseSeriesTypeRepository.GetCaption(seriesType)); 
			} 
		}
		public override string DragAreaImagePath { get { return seriesType.ToString(); } }
		public override string ImagePath { get { return "OpenHighLowClose" + seriesType.ToString(); } }
		public override Type SupportedSeries { get { return typeof(OpenHighLowCloseSeries); } }
		public OpenHighLowCloseSeriesType SeriesType { get { return seriesType; } }
		public ChartOpenHighLowCloseSeriesConverter(OpenHighLowCloseSeriesType seriesType) {
			this.seriesType = seriesType;
		}
		public override bool IsMatch(ChartSeries series) {
			OpenHighLowCloseSeries openHighLowCloseSeries = series as OpenHighLowCloseSeries;
			return openHighLowCloseSeries != null && openHighLowCloseSeries.SeriesType == seriesType;
		}
		public override IList<ChartSeries> PrepareSeries(ICollection<DataItemDescription> measureDescriptions) {
			List<ChartSeries> series = new List<ChartSeries>();
			OpenHighLowCloseSeries openHighLowCloseSeries = new OpenHighLowCloseSeries(seriesType);
			IList<Measure> remainMeasures = FillExistingMeasures(openHighLowCloseSeries, measureDescriptions);
			series.Add(openHighLowCloseSeries);
			int measuresCount = remainMeasures.Count;
			int count = measuresCount / 4;
			if (measuresCount % 4 > 0)
				count++;
			if (count > 0)
				for (int i = 0, measureIndex = 0; i < count; i++) {
					Measure measure1 = remainMeasures[measureIndex++];
					Measure measure2 = measureIndex == measuresCount ? null : remainMeasures[measureIndex++];
					Measure measure3 = measureIndex == measuresCount ? null : remainMeasures[measureIndex++];
					Measure measure4 = measureIndex == measuresCount ? null : remainMeasures[measureIndex++];
					series.Add(new OpenHighLowCloseSeries(measure1, measure2, measure3, measure4, seriesType));
				}
			return series;
		}
		public override ChartSeries CreateSeries() {
			return new OpenHighLowCloseSeries(seriesType);
		}
	}
}
