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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class SeriesPointCache : ISeriesPointData {
		readonly RefinedPoint refinedPoint;
		string labelText;
		string legendText;
		double opacity;
		bool shouldUpdateGeometry = true;
		Color color;
		PointGeometry pointGeometry;
		int indexInSeries;
		LegendItem legendItem;		
		public RefinedPoint RefinedPoint { get { return refinedPoint; } }
		public string LabelText { get { return labelText; } }
		public string LegendText { get { return legendText; } set { legendText = value; } }
		public double Opacity { get { return opacity; } set { opacity = value; } }
		public PointGeometry PointGeometry { get { return pointGeometry; } }
		public Color Color { get { return color; } set { color = value; } }
		public bool ShouldUpdateGeometry { get { return shouldUpdateGeometry; } set { shouldUpdateGeometry = value; } }
		public LegendItem LegendItem { get { return legendItem; } set { legendItem = value; } }
		public int IndexInSeries { get { return indexInSeries; } set { indexInSeries = value; } }
		public SeriesPointCache(RefinedPoint refinedPoint, int indexInSeries) {
			this.refinedPoint = refinedPoint;
			this.indexInSeries = indexInSeries;
		}
		#region ISeriesPointItem implementation
		string[] ISeriesPointData.LabelsTexts { get { return new string[] { labelText }; } set { labelText = value[0]; } }
		RefinedPoint ISeriesPointData.RefinedPoint { get { return refinedPoint; } }
		ISeriesPoint ISeriesPointData.SeriesPoint { get { return refinedPoint.SeriesPoint; } }
		#endregion
		public void Clear() {
			if (pointGeometry != null)
				pointGeometry.Clear();
			shouldUpdateGeometry = true;
		}
		public void InitializePointGeometry(PointGeometry pointGeometry) {
			this.pointGeometry = pointGeometry;
			shouldUpdateGeometry = false;
		}
	}
	public class PointGeometry {
		readonly ContentPresenter labelContentPresenter;
		readonly Brush brush;
		public ContentPresenter LabelContentPresenter { get { return labelContentPresenter; } }
		public Brush Brush { get { return brush; } }
		public bool IsLabelPresent {
			get {
				if (labelContentPresenter == null)
					return false;
				SeriesLabelItem labelItem = labelContentPresenter.Content as SeriesLabelItem;
				return labelItem != null && !String.IsNullOrEmpty(labelItem.Text);
			}
		}
		public PointGeometry(SeriesLabel label, ContentPresenter labelContentPresenter) {
			this.labelContentPresenter = labelContentPresenter;
			brush = RenderHelper.CreateBrush(labelContentPresenter, label);
		}
		public void Clear() {
			if (labelContentPresenter != null) {
				labelContentPresenter.Content = null;
				labelContentPresenter.ContentTemplate = null;
			}
		}
	}
	public class Series3DPointGeometry : PointGeometry {
		readonly Model3D labelModel;
		readonly Model3DInfo modelInfo;
		public Model3D LabelModel { get { return labelModel; } }
		public Model3DInfo ModelInfo { get { return modelInfo; } }
		public Series3DPointGeometry(SeriesLabel label, ContentPresenter labelContentPresenter, Model3D labelModel, Model3DInfo modelInfo) : base(label, labelContentPresenter) {
			this.modelInfo = modelInfo;
			this.labelModel = labelModel;
		}
	}
	public class SeriesCache : IEnumerable<SeriesPointCache>, ISeriesItem {
		readonly Dictionary<ISeriesPoint, SeriesPointCache> seriesPointsCache = new Dictionary<ISeriesPoint, SeriesPointCache>();
		readonly Series series;
		DrawOptions drawOptions;
		bool isPointsCacheDefinesFully = false;
		string legendText;
		public bool IsPointsCacheDefinesFully { get { return isPointsCacheDefinesFully; } }
		public DrawOptions DrawOptions { get { return drawOptions; } set { drawOptions = value; } }
		public string LegendText { get { return legendText; } set { legendText = value; } }
		IEnumerator IEnumerable.GetEnumerator() { return seriesPointsCache.Values.GetEnumerator(); }
		IEnumerator<SeriesPointCache> IEnumerable<SeriesPointCache>.GetEnumerator() { return seriesPointsCache.Values.GetEnumerator(); }
		public SeriesCache(Series series) {
			this.series = series;
		}
		#region ISeriesItem implementation
		bool ISeriesItem.ShouldUpdate { get { return !((ISeriesItem)this).HasPoints; } }
		bool ISeriesItem.HasPoints { get { return seriesPointsCache.Count > 0; } }
		IEnumerable<ISeriesPointData> ISeriesItem.PointData {
			get {
				foreach (SeriesPointCache pointCache in this)
					yield return pointCache;
			}
		}
		void ISeriesItem.Update(IRefinedSeries refinedSeries) {
			if (refinedSeries == null)
				return;
			int index = 0;
			foreach (RefinedPoint refinedPoint in refinedSeries.Points) {
				if (!refinedPoint.IsEmpty)
					seriesPointsCache.Add(refinedPoint.SeriesPoint, new SeriesPointCache(refinedPoint, index));
				index++;
			}
		}
		#endregion
		public SeriesPointCache GetSeriesPointCache(ISeriesPoint seriesPoint) {
			if (seriesPoint == null)
				return null;
			SeriesPointCache seriesPointCache;
			if (seriesPointsCache.TryGetValue(seriesPoint, out seriesPointCache))
				return seriesPointCache;
			return TryGetSeriesPointCacheByArgument(seriesPoint);
		}
		SeriesPointCache TryGetSeriesPointCacheByArgument(ISeriesPoint point) {
			foreach (ISeriesPoint cachePoint in seriesPointsCache.Keys)
				if (cachePoint.QualitativeArgument == point.QualitativeArgument)
					return seriesPointsCache[cachePoint];
			return null;
		}
		void ResetPointCacheGeometry(SeriesPointCache seriesPointCache) {
			if (seriesPointCache != null)
				seriesPointCache.ShouldUpdateGeometry = true;
			isPointsCacheDefinesFully = false;
		}
		void SetPointLegendMarkerSelected(SeriesPointCache seriesPointCache, bool isSelected) {
			if (seriesPointCache != null) {
				LegendItem legendItem = seriesPointCache.LegendItem;
				if (legendItem != null)
					legendItem.IsSelected = isSelected;
			}
		}
		public void ValidateSeriesPointCache(RefinedPoint refinedPoint, PointGeometry seriesGeometry) {
			SeriesPointCache seriesPointCache = GetSeriesPointCache(refinedPoint.SeriesPoint);
			if (seriesPointCache != null) {
				if (seriesPointCache.ShouldUpdateGeometry) {
					seriesPointCache.InitializePointGeometry(seriesGeometry);
					isPointsCacheDefinesFully = true;
				}
			} else {
				ChartDebug.Fail("SeriesPointCache cant't be null.");
				isPointsCacheDefinesFully = false;
			}
		}
		public void UpdateSelectionForPointCache(ISeriesPoint seriesPoint, bool isSelected, bool shouldResetCache) {
			SeriesPointCache seriesPointCache;
			seriesPointsCache.TryGetValue(seriesPoint, out seriesPointCache);
			if (shouldResetCache)
				ResetPointCacheGeometry(seriesPointCache);
			SetPointLegendMarkerSelected(seriesPointCache, isSelected);
		}
		public void Clear() {
			foreach (SeriesPointCache seriesPointCache in seriesPointsCache.Values)
				seriesPointCache.Clear();
			seriesPointsCache.Clear();
			isPointsCacheDefinesFully = false;
		}
#if DEBUGTEST
		internal Dictionary<ISeriesPoint, SeriesPointCache> GetSeriesPointCacheDictionaryForTests() {
			return seriesPointsCache;
		}
#endif
	}
}
