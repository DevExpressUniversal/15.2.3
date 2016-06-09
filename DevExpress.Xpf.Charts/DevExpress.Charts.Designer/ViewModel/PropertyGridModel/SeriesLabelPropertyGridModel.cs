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

using System.ComponentModel;
using System.Windows.Input;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartSeriesLabelPropertyGridModel : PropertyGridModelBase {
		protected const string propertyPath = "Label.";
		readonly SetSeriesPropertyCommand setPropertyCommand;
		readonly SetSeriesLabelAttachedPropertyCommand setSeriesLabelAttachedPropertyCommand;
		readonly SeriesLabel label;
		protected internal SeriesLabel Label { get { return label; } }
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		protected override ICommand SetObjectAttachedPropertyCommand { get { return setSeriesLabelAttachedPropertyCommand; } }
		[Category(Categories.Presentation)]
		public int Indent {
			get { return label.Indent; }
			set {
				SetProperty(propertyPath + "Indent", value);
			}
		}
		[Category(Categories.Presentation)]
		public bool ConnectorVisible {
			get { return label.ConnectorVisible; }
			set {
				SetProperty(propertyPath + "ConnectorVisible", value);
			}
		}
		[Category(Categories.Presentation)]
		public int ConnectorThickness {
			get { return label.ConnectorThickness; }
			set {
				SetProperty(propertyPath + "ConnectorThickness", value);
			}
		}
		[Category(Categories.Behavior)]
		public ResolveOverlappingMode ResolveOverlappingMode {
			get { return label.ResolveOverlappingMode; }
			set {
				SetProperty(propertyPath + "ResolveOverlappingMode", value);
			}
		}
		[Category(Categories.Common)]
		public string TextPattern {
			get { return label.TextPattern; }
			set { SetProperty(propertyPath + "TextPattern", value); }
		}
		public WpfChartSeriesLabelPropertyGridModel() : this(null, null) { }
		public WpfChartSeriesLabelPropertyGridModel(WpfChartModel chartModel, SeriesLabel label) : base(chartModel) {
			this.label = label;
			setPropertyCommand = new SetSeriesPropertyCommand(ChartModel);
			setSeriesLabelAttachedPropertyCommand = new SetSeriesLabelAttachedPropertyCommand(ChartModel);
		}
	}
	public class WpfChartSeriesLabelMarkerSeries2DPropertyGridModel : WpfChartSeriesLabelPropertyGridModel {
		[Category(Categories.Layout)]
		public double Angle {
			get { return MarkerSeries2D.GetAngle(Label); }
			set {
				SetAttachedProperty("Angle", value, typeof(MarkerSeries2D),
					delegate(object targetObject, object newValue) {
						MarkerSeries2D.SetAngle((SeriesLabel)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return MarkerSeries2D.GetAngle((SeriesLabel)targetObject);
					});
			}
		}
		public WpfChartSeriesLabelMarkerSeries2DPropertyGridModel(WpfChartModel chartModel, SeriesLabel label)
			: base(chartModel, label) {
		}
	}
	public class WpfChartSeriesLabelMarkerSeries3DPropertyGridModel : WpfChartSeriesLabelPropertyGridModel {
		[Category(Categories.Layout)]
		public Marker3DLabelPosition LabelPosition {
			get { return MarkerSeries3D.GetLabelPosition(Label); }
			set {
				SetAttachedProperty("LabelPosition", value, typeof(MarkerSeries3D),
					delegate(object targetObject, object newValue) {
						MarkerSeries3D.SetLabelPosition((SeriesLabel)targetObject, (Marker3DLabelPosition)newValue);
					},
					delegate(object targetObject) {
						return MarkerSeries3D.GetLabelPosition((SeriesLabel)targetObject);
					});
			}
		}
		public WpfChartSeriesLabelMarkerSeries3DPropertyGridModel(WpfChartModel chartModel, SeriesLabel label)
			: base(chartModel, label) {
		}
	}
	public class WpfChartSeriesLabelBubbleSeries2DPropertyGridModel : WpfChartSeriesLabelMarkerSeries2DPropertyGridModel {
		[Category(Categories.Layout)]
		public Bubble2DLabelPosition LabelPosition {
			get { 
				return BubbleSeries2D.GetLabelPosition(Label); 
			}
			set {
				SetAttachedProperty("LabelPosition", value, typeof(BubbleSeries2D),
					delegate(object targetObject, object newValue) {
						BubbleSeries2D.SetLabelPosition((SeriesLabel)targetObject, (Bubble2DLabelPosition)newValue);
					},
					delegate(object targetObject) {
						return BubbleSeries2D.GetLabelPosition((SeriesLabel)targetObject);
					});
			}
		}
		public WpfChartSeriesLabelBubbleSeries2DPropertyGridModel(WpfChartModel chartModel, SeriesLabel label)
			: base(chartModel, label) {
		}
	}
	public class WpfChartSeriesLabelCircularSeries2DPropertyGridModel : WpfChartSeriesLabelPropertyGridModel {
		[Category(Categories.Layout)]
		public double Angle {
			get { return CircularSeries2D.GetAngle(Label); }
			set {
				SetAttachedProperty("Angle", value, typeof(CircularSeries2D),
					delegate(object targetObject, object newValue) {
						CircularSeries2D.SetAngle((SeriesLabel)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return CircularSeries2D.GetAngle((SeriesLabel)targetObject);
					});
			}
		}
		public WpfChartSeriesLabelCircularSeries2DPropertyGridModel(WpfChartModel chartModel, SeriesLabel label) : base(chartModel, label) {
		}
	}
	public class WpfChartSeriesLabelPieSeriesPropertyGridModel : WpfChartSeriesLabelPropertyGridModel {
		[Category(Categories.Layout)]
		public PieLabelPosition LabelPosition {
			get { return PieSeries.GetLabelPosition(Label); }
			set {
				SetAttachedProperty("LabelPosition", value, typeof(PieSeries),
					delegate(object targetObject, object newValue) {
						PieSeries.SetLabelPosition((SeriesLabel)targetObject, (PieLabelPosition)newValue);
					},
					delegate(object targetObject) {
						return PieSeries.GetLabelPosition((SeriesLabel)targetObject);
					});
			}
		}
		public WpfChartSeriesLabelPieSeriesPropertyGridModel(WpfChartModel chartModel, SeriesLabel label)
			: base(chartModel, label) {
		}
	}
	public class WpfChartSeriesLabelRangeBarSeries2DPropertyGridModel : WpfChartSeriesLabelPropertyGridModel {
		[Category(Categories.Layout)]
		public RangeBarLabelKind LabelKind {
			get { return RangeBarSeries2D.GetLabelKind(Label); }
			set {
				SetAttachedProperty("LabelKind", value, typeof(RangeBarSeries2D),
					delegate(object targetObject, object newValue) {
						RangeBarSeries2D.SetLabelKind((SeriesLabel)targetObject, (RangeBarLabelKind)newValue);
					},
					delegate(object targetObject) {
						return RangeBarSeries2D.GetLabelKind((SeriesLabel)targetObject);
					});
			}
		}
		public WpfChartSeriesLabelRangeBarSeries2DPropertyGridModel(WpfChartModel chartModel, SeriesLabel label)
			: base(chartModel, label) {
		}
	}
	public class WpfChartSeriesLabelRangeAreaSeries2DPropertyGridModel : WpfChartSeriesLabelPropertyGridModel {
		[Category(Categories.Layout)]
		public double MinValueAngle {
			get { return RangeAreaSeries2D.GetMinValueAngle(Label); }
			set {
				SetAttachedProperty("MinValueAngle", value, typeof(RangeAreaSeries2D),
					delegate(object targetObject, object newValue) {
						RangeAreaSeries2D.SetMinValueAngle((SeriesLabel)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return RangeAreaSeries2D.GetMinValueAngle((SeriesLabel)targetObject);
					});
			}
		}
		[Category(Categories.Layout)]
		public double MaxValueAngle {
			get { return RangeAreaSeries2D.GetMaxValueAngle(Label); }
			set {
				SetAttachedProperty("MaxValueAngle", value, typeof(RangeAreaSeries2D),
					delegate(object targetObject, object newValue) {
						RangeAreaSeries2D.SetMaxValueAngle((SeriesLabel)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return RangeAreaSeries2D.GetMaxValueAngle((SeriesLabel)targetObject);
					});
			}
		}
		[Category(Categories.Layout)]
		public RangeAreaLabelKind LabelKind {
			get { return RangeAreaSeries2D.GetLabelKind(Label); }
			set {
				SetAttachedProperty("LabelKind", value, typeof(RangeAreaSeries2D),
					delegate(object targetObject, object newValue) {
						RangeAreaSeries2D.SetLabelKind((SeriesLabel)targetObject, (RangeAreaLabelKind)newValue);
					},
					delegate(object targetObject) {
						return RangeAreaSeries2D.GetLabelKind((SeriesLabel)targetObject);
					});
			}
		}
		public WpfChartSeriesLabelRangeAreaSeries2DPropertyGridModel(WpfChartModel chartModel, SeriesLabel label)
			: base(chartModel, label) {
		}
	}
	public class WpfChartSeriesLabelBarSideBySideSeries2DPropertyGridModel : WpfChartSeriesLabelPropertyGridModel {
		[Category(Categories.Layout)]
		public Bar2DLabelPosition LabelPosition {
			get { return BarSideBySideSeries2D.GetLabelPosition(Label); }
			set {
				SetAttachedProperty("LabelPosition", value, typeof(BarSideBySideSeries2D),
					delegate(object targetObject, object newValue) {
						BarSideBySideSeries2D.SetLabelPosition((SeriesLabel)targetObject, (Bar2DLabelPosition)newValue);
					},
					delegate(object targetObject) {
						return BarSideBySideSeries2D.GetLabelPosition((SeriesLabel)targetObject);
					});
			}
		}
		public WpfChartSeriesLabelBarSideBySideSeries2DPropertyGridModel(WpfChartModel chartModel, SeriesLabel label)
			: base(chartModel, label) {
		}
	}
}
