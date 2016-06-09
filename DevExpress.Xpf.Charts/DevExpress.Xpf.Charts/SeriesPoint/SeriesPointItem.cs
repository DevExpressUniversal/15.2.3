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
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class SeriesPointItem: NotifyPropertyChangedObject {
		const RangeValueLevel defaultValueLevel = RangeValueLevel.Value1;
		readonly SeriesPointPresentationData presentationData;
		readonly Series series;
		readonly SeriesPointData seriesPointData;
		readonly RangeValueLevel valueLevel;
		WeakReference pointItemPresentation;
		SeriesLabelItem labelItem;
		PointModel model;
		double opacity = 1;
		bool showInFrontOfAxes;
		bool showBehindOfAxes;
		internal AnimationProgress PointProgress { get { return seriesPointData.PointProgress; } }
		internal AnimationProgress LabelProgress { get { return seriesPointData.LabelProgress; } }
		internal SeriesPointData SeriesPointData { get { return seriesPointData; } }
		internal RefinedPoint RefinedPoint {
			get { return seriesPointData.RefinedPoint; }	
		}
		internal RangeValueLevel ValueLevel { get { return valueLevel; } }
		internal SeriesPointItemPresentation PointItemPresentation {
			get { return pointItemPresentation != null ? pointItemPresentation.Target as SeriesPointItemPresentation : null; }
			set { pointItemPresentation = new WeakReference(value); }
		}
		internal bool IsHighlighted {
			get { return presentationData.IsHighlighted; }
			set {
				presentationData.IsHighlighted = value;
				if (PointItemPresentation != null)
					PointItemPresentation.InvalidateMeasure();
			}
		}
		internal bool IsSelected {
			get { return presentationData.IsSelected; }
			set {
				presentationData.IsSelected = value;
				OnSelectedStateChanged();
			}
		}
		internal SeriesPoint SeriesPoint {
			get { return presentationData.SeriesPoint; }
			set { presentationData.SeriesPoint = value; }
		}
		public PointModel Model {
			get { return model; }
			set {
				if (value != model) {
					model = value;
					OnPropertyChanged("Model");
				}
			}
		}
		public bool ShowInFrontOfAxes {
			get { return showInFrontOfAxes; }
			set {
				if (showInFrontOfAxes != value) {
					showInFrontOfAxes = value;
					OnPropertyChanged("ShowInFrontOfAxes");
				}
			}
		}
		public bool ShowBehindAxes {
			get { return showBehindOfAxes; }
			set {
				if (showBehindOfAxes != value) {
					showBehindOfAxes = value;
					OnPropertyChanged("ShowBehindOfAxes");
				}
			}
		}
		public double Opacity {
			get { return opacity; }
			set {
				opacity = value;
				OnPropertyChanged("Opacity");
			}
		}
		public SeriesPointLayout Layout { 
			get { return presentationData.Layout; }
			set {
				presentationData.Layout = value;
				if (PointItemPresentation != null)
					PointItemPresentation.InvalidateMeasure();
			}
		}
		public SeriesLabelItem LabelItem { get { return labelItem; } set { labelItem = value; } }
		public SeriesPointPresentationData PresentationData { get { return presentationData; } }
		public Series Series { get { return series; } }
		internal SeriesPointItem(Series series, SeriesPointData seriesPointData) : this(series, seriesPointData, defaultValueLevel) {
		}
		internal SeriesPointItem(Series series, SeriesPointData seriesPointData, RangeValueLevel valueLevel) {
			this.series = series;
			this.seriesPointData = seriesPointData;
			this.presentationData = new SeriesPointPresentationData() { SeriesSelectionType = series.SupportedSelectionType };
			this.valueLevel = valueLevel;
			Update();
		}
		void OnSelectedStateChanged() {
			if (seriesPointData != null && seriesPointData.LegendItem != null)
				seriesPointData.LegendItem.IsSelected = IsSelected;
			if (VisualSelectionHelper.SupportsSizeSelection(series.SupportedSelectionType) && PointItemPresentation != null)
				PointItemPresentation.InvalidateMeasure();
		}
		internal void Update() {
			bool visible = series.IsPointValueVisible(valueLevel);
			Model = visible ? series.GetModel(valueLevel) : null;
			ShowInFrontOfAxes = series.InFrontOfAxes && visible;
			ShowBehindAxes = !series.InFrontOfAxes && visible;
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public enum RangeValueLevel {
		Value1,
		Value2,
		TwoValues
	}
}
