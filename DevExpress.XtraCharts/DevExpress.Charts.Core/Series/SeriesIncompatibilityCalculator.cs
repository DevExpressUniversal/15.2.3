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

using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public enum SeriesIncompatibilityType {
		ByViewType,
		ByArgumentScaleType,
		ByValueScaleType,
		Invisible
	}
	public class SeriesIncompatibilityInfo : Dictionary<SeriesIncompatibilityType, ISeries> {
		public void AddIncompatibility(SeriesIncompatibilityType type, ISeries masterSeries) {
			if (!ContainsKey(type))
				Add(type, masterSeries);
		}
	}
	public class SeriesIncompatibilityStatistics {
		readonly Dictionary<ISeries, SeriesIncompatibilityInfo> dictionary = new Dictionary<ISeries, SeriesIncompatibilityInfo>();
		public SeriesIncompatibilityInfo this[ISeries series] {
			get {
				SeriesIncompatibilityInfo info;
				return dictionary.TryGetValue(series, out info) ? info : null;
			}
		}
		public bool IsSeriesIncompatible(ISeries series) {
			return dictionary.ContainsKey(series);
		}
		public void Add(ISeries series, SeriesIncompatibilityType type, ISeries masterSeries) {
			ChartDebug.Assert(series != null);
			SeriesIncompatibilityInfo info = this[series];
			if (info == null) {
				info = new SeriesIncompatibilityInfo();
				dictionary.Add(series, info);
			}
			info.AddIncompatibility(type, masterSeries);
		}
		public void Clear() {
			dictionary.Clear();
		}
	}
	public abstract class SeriesIncompatibilityCalculatorBase<VIEW, AXISKEY> {
		class MasterSeriesInfo {
			readonly ISeries series;
			readonly Scale scaleType;
			public ISeries Series { get { return series; } }
			public Scale ScaleType { get { return scaleType; } }
			public MasterSeriesInfo(ISeries series, Scale scaleType) {
				this.series = series;
				this.scaleType = scaleType;
			}
			public MasterSeriesInfo(Scale scaleType)
				: this(null, scaleType) {
			}
		}
		readonly SeriesIncompatibilityStatistics statistics;
		readonly Dictionary<AXISKEY, MasterSeriesInfo> masterSeriesForAxisXDictionary = new Dictionary<AXISKEY, MasterSeriesInfo>();
		readonly Dictionary<AXISKEY, MasterSeriesInfo> masterSeriesForAxisYDictionary = new Dictionary<AXISKEY, MasterSeriesInfo>();
		public SeriesIncompatibilityCalculatorBase()
			: this(new SeriesIncompatibilityStatistics()) {
		}
		public SeriesIncompatibilityCalculatorBase(SeriesIncompatibilityStatistics statistics) {
			this.statistics = statistics;
		}
		public void AddTemplateView(VIEW view, Scale argumentScaleType, Scale valueScaleType) {
			AXISKEY AxisXkey = GetAxisXkey(view);
			AXISKEY AxisYkey = GetAxisYkey(view);
			if (AxisXkey != null && AxisYkey != null) {
				masterSeriesForAxisXDictionary.Add(AxisXkey, new MasterSeriesInfo(argumentScaleType));
				masterSeriesForAxisYDictionary.Add(AxisYkey, new MasterSeriesInfo(valueScaleType));
			}
		}
		protected abstract VIEW GetView(ISeries series);
		protected abstract AXISKEY GetAxisXkey(VIEW view);
		protected abstract AXISKEY GetAxisYkey(VIEW view);
		protected abstract bool CheckIncompatibleViewType(ISeries series, bool is3DView, CompatibleViewType diagramClass);
		public bool IsVisibleAndCompatible(RefinedSeries refinedSeries, bool is3DView, CompatibleViewType diagramClass, ISeries baseSeries) {
			bool isVisible = IsVisible(refinedSeries.Series);
			if (!isVisible)
				return false;
			bool isCompatible = IsCompatible(refinedSeries, is3DView, diagramClass, baseSeries);
			return isCompatible;
		}
		public bool IsVisible(ISeries series) {
			if (!series.Visible) {
				statistics.Add(series, SeriesIncompatibilityType.Invisible, null);
				return false;
			}
			return true;
		}
		public bool IsCompatible(RefinedSeries refinedSeries, bool is3DView, CompatibleViewType diagramClass, ISeries baseSeries) {
			ISeries series = refinedSeries.Series;
			if (!CheckIncompatibleViewType(series, is3DView, diagramClass)) {
				statistics.Add(series, SeriesIncompatibilityType.ByViewType, baseSeries);
				return false;
			}
			VIEW view = GetView(series);
			if (view == null)
				return true;
			MasterSeriesInfo masterSeriesInfo;
			AXISKEY axisX = GetAxisXkey(view);
			AXISKEY axisY = GetAxisYkey(view);
			if (axisX == null || axisY == null) {
				return false;
			}
			bool isCompatible = true;
			if (!refinedSeries.NoArgumentScaleType) {
				if (masterSeriesForAxisXDictionary.TryGetValue(axisX, out masterSeriesInfo)) {
					if (masterSeriesInfo.ScaleType != series.ArgumentScaleType) {
						statistics.Add(series, SeriesIncompatibilityType.ByArgumentScaleType, masterSeriesInfo.Series);
						isCompatible = false;
					}
				} else
					masterSeriesForAxisXDictionary.Add(axisX, new MasterSeriesInfo(series, series.ArgumentScaleType));
			}
			if (masterSeriesForAxisYDictionary.TryGetValue(axisY, out masterSeriesInfo)) {
				if (masterSeriesInfo.ScaleType != series.ValueScaleType) {
					statistics.Add(series, SeriesIncompatibilityType.ByValueScaleType, masterSeriesInfo.Series);
					isCompatible = false;
				}
			} else
				masterSeriesForAxisYDictionary.Add(axisY, new MasterSeriesInfo(series, series.ValueScaleType));
			return isCompatible;
		}
		public Scale GetAxisXMasterScaleType(RefinedSeries refinedSeries) {
			VIEW view = GetView(refinedSeries.Series);
			if (view != null) {
				AXISKEY axisX = GetAxisXkey(view);
				if (axisX != null) {
					MasterSeriesInfo masterSeriesInfo;
					if (masterSeriesForAxisXDictionary.TryGetValue(axisX, out masterSeriesInfo))
						return masterSeriesInfo.ScaleType;
				}
			}
			return Scale.Numerical;
		}
	}
	public class RefinedSeriesIncompatibilityCalculator : SeriesIncompatibilityCalculatorBase<IXYSeriesView, IAxisData> {
		ISeriesView baseView;
		ISeries baseSeries;
		public bool CanCalculate { get { return baseView != null; } }
		public RefinedSeriesIncompatibilityCalculator(SeriesIncompatibilityStatistics statistics) : base(statistics) { }
		protected override IXYSeriesView GetView(ISeries series) {
			return series.SeriesView as IXYSeriesView;
		}
		protected override IAxisData GetAxisXkey(IXYSeriesView view) {
			return view.AxisXData;
		}
		protected override IAxisData GetAxisYkey(IXYSeriesView view) {
			return view.AxisYData;
		}
		protected override bool CheckIncompatibleViewType(ISeries series, bool is3DView, CompatibleViewType diagramClass) {
			return series.SeriesView != null && series.SeriesView.Is3DView == is3DView && series.SeriesView.CompatibleViewType == diagramClass;
		}
		public void Initialize(ISeries baseSeries, ISeriesView baseView) {
			this.baseSeries = baseSeries;
			this.baseView = baseView;
		}
		public bool IsVisibleAndCompatible(RefinedSeries refinedSeries) {
			return IsVisibleAndCompatible(refinedSeries, baseView.Is3DView, baseView.CompatibleViewType, baseSeries);
		}
		public bool IsCompatible(RefinedSeries refinedSeries) {
			return IsCompatible(refinedSeries, baseView.Is3DView, baseView.CompatibleViewType, baseSeries);
		}
	}
}
