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
using DevExpress.Xpf.Charts;
using System;
using System.Windows.Media;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartSeriesPointModel : ChartModelElement {
		public override IEnumerable<ChartModelElement> Children {
			get {
				return null;
			}
		}
		public Series Series {
			get {
				return SeriesPoint.Series;
			}
		}
		public SeriesPoint SeriesPoint {
			get {
				return (SeriesPoint)ChartElement;
			}
		}
		public WpfChartSeriesModel SeriesModel {
			get {
				return (WpfChartSeriesModel)Parent.Parent;
			}
		}
		public object Argument {
			get {
				if (Series != null && Series.ArgumentScaleType == ScaleType.DateTime)
					return SeriesPoint.DateTimeArgument;
				return SeriesPoint.Argument;
			}
			set {
				if (!object.Equals(value, SeriesPoint.Argument)) {
					DataEditorUtils.SetSeriesPointArgumentSafely(SeriesPoint, value);
					OnPropertyChanged("Argument");
				}
			}
		}
		public object Value {
			get {
				if (Series != null && Series.ValueScaleType == ScaleType.DateTime)
					return SeriesPoint.DateTimeValue;
				else
					return SeriesPoint.Value;
			}
			set {
				if (value != null) {
					if (Series != null && Series.ValueScaleType == ScaleType.DateTime || value is DateTime)
						SeriesPoint.DateTimeValue = (DateTime)value;
					else
						SeriesPoint.Value = (double)value;
				}
				else
					SeriesPoint.Value = 0;
				OnPropertyChanged("Value");
			}
		}
		public double Value2 {
			get {
				return DataEditorUtils.GetAttachedValue2(SeriesPoint);
			}
			set {
				DataEditorUtils.SetAttachedValue2(SeriesPoint, value);
				OnPropertyChanged("Value2");
			}
		}
		public double Weight {
			get {
				return DataEditorUtils.GetAttachedWeight(SeriesPoint);
			}
			set {
				DataEditorUtils.SetAttachedWeight(SeriesPoint, value);
				OnPropertyChanged("Weight");
			}
		}
		public double Open {
			get {
				return DataEditorUtils.GetAttachedOpen(SeriesPoint);
			}
			set {
				DataEditorUtils.SetAttachedOpen(SeriesPoint, value);
				OnPropertyChanged("Open");
			}
		}
		public double Close {
			get {
				return DataEditorUtils.GetAttachedClose(SeriesPoint);
			}
			set {
				DataEditorUtils.SetAttachedClose(SeriesPoint, value);
				OnPropertyChanged("Close");
			}
		}
		public double High {
			get {
				return DataEditorUtils.GetAttachedHigh(SeriesPoint);
			}
			set {
				DataEditorUtils.SetAttachedHigh(SeriesPoint, value);
				OnPropertyChanged("High");
			}
		}
		public double Low {
			get {
				return DataEditorUtils.GetAttachedLow(SeriesPoint);
			}
			set {
				DataEditorUtils.SetAttachedLow(SeriesPoint, value);
				OnPropertyChanged("Low");
			}
		}
		public SolidColorBrush Brush {
			get {
				return SeriesPoint.Brush;
			}
			set {
				if (SeriesPoint.Brush != value) {
					SeriesPoint.Brush = value;
					OnPropertyChanged("Brush");
				}
			}
		}
		public WpfChartSeriesPointModel(ChartModelElement parent, SeriesPoint seriesPoint) : base(parent, seriesPoint) {
			PropertyGridModel = WpfChartSeriesPointPropertyGridModel.CreatePropertyGridModelForSeriesPoint((WpfChartModel)GetParent<WpfChartModel>(), this);
		}
		public int GetSelfIndex() {
			return ((WpfChartSeriesPointCollectionModel)Parent).IndexOf(this);
		}
	}
}
