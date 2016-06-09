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

using DevExpress.Charts.Model.Native;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Charts.Model {
	public abstract class SeriesModel : ModelElement {
		readonly Dictionary<DataMemberType, string> dataMembers;
		ArgumentScaleType argumentScaleType = ArgumentScaleType.Auto;
		ValueScaleType valueScaleType = ValueScaleType.Numerical;
		object dataSource;
		SeriesPropertyBagBase properties;
		bool labelsVisibility;
		SeriesLabel label;
		public string DisplayName { get; set; }
		public string LegendPointPattern { get; set; }
		public bool ShowInLegend { get; set; }
		public virtual ArgumentScaleType ArgumentScaleType { get { return argumentScaleType; } set { argumentScaleType = value; } }
		public ValueScaleType ValueScaleType { get { return valueScaleType; } set { valueScaleType = value; } }
		public Dictionary<DataMemberType, string> DataMembers { get { return dataMembers; } }
		public object DataSource {
			get { return dataSource; }
			set { dataSource = value; }
		}
		public bool LabelsVisibility {
			get { return labelsVisibility; }
			set {
				if (labelsVisibility != value) {
					labelsVisibility = value;
					NotifyParent(this, "LabelsVisibility", value);
				}
			}
		}
		public SeriesLabel Label {
			get { return label; }
			set {
				if (label != value) {
					label = value;
					NotifyParent(this, "Label", value);
				}
			}
		}
		protected SeriesPropertyBagBase Properties {
			get {
				if(properties == null)
					properties = CreatePropertyBag();
				return properties;
			} 
		}
		protected virtual SeriesPropertyBagBase CreatePropertyBag() { return EmptyPropertyBag.Instance; } 
		public SeriesModel() {
			this.dataMembers = new Dictionary<DataMemberType, string>();
			ShowInLegend = true;
		}
	}
	public class SeriesCollection : ModelElementCollection<SeriesModel> {
		public SeriesCollection(Chart parent) : base(parent) { 
		}
		protected override void OnValidate(SeriesModel value) {
			base.OnValidate(value);
			Chart chart = Parent as Chart;
			if (chart != null) {
				if(!chart.IsSeriesValid(value))
					throw new ChartModelException("Invalid series type for this type of chart");
			}
		}
	}
	public abstract class CartesianSeriesBase : SeriesModel {
		int secondaryArgumentAxisIndex = -1;
		int secondaryValueAxisIndex = -1;
		public int SecondaryArgumentAxisIndex {
			get { return secondaryArgumentAxisIndex; }
			set {
				if(secondaryArgumentAxisIndex != value) {
					secondaryArgumentAxisIndex = value;
					NotifyParent(this, "SecondaryArgumentAxisIndex", value);
				}
			}
		}
		public int SecondaryValueAxisIndex {
			get { return secondaryValueAxisIndex; }
			set {
				if(secondaryValueAxisIndex != value) {
					secondaryValueAxisIndex = value;
					NotifyParent(this, "SecondaryValueAxisIndex", value);
				}
			}
		}
	}
	public abstract class ColorEachCartesianSeriesBase : CartesianSeriesBase, ISupportColorEachSeries {
		public bool ColorEach {
			get { return Properties.GetSupport<ISupportColorEachSeries>().ColorEach; }
			set { Properties.GetSupport<ISupportColorEachSeries>().ColorEach = value; }
		}
		protected override SeriesPropertyBagBase CreatePropertyBag() {
			return new GenericPropertyBag<ColorEachBagSupport>(this);
		}
	}
	public abstract class ColorEachMarkerCartesianSeriesBase : CartesianSeriesBase, ISupportMarkerSeries {
		public Marker Marker {
			get { return Properties.GetSupport<ISupportMarkerSeries>().Marker; }
			set { Properties.GetSupport<ISupportMarkerSeries>().Marker = value; }
		}
		protected override SeriesPropertyBagBase CreatePropertyBag() {
			return new GenericPropertyBag2<MarkerBagSupport, ColorEachBagSupport>(this);
		}
	}
	public abstract class ColorEachMarkerSeriesBase : SeriesModel, ISupportMarkerSeries, ISupportColorEachSeries {
		public bool ColorEach {
			get { return Properties.GetSupport<ISupportColorEachSeries>().ColorEach; }
			set { Properties.GetSupport<ISupportColorEachSeries>().ColorEach = value; }
		}
		public Marker Marker {
			get { return Properties.GetSupport<ISupportMarkerSeries>().Marker; }
			set { Properties.GetSupport<ISupportMarkerSeries>().Marker = value; }
		}
		protected override SeriesPropertyBagBase CreatePropertyBag() {
			return new GenericPropertyBag2<MarkerBagSupport, ColorEachBagSupport>(this);
		}
	}
	public abstract class RadarSeriesBase : ColorEachMarkerSeriesBase {
	}
	public abstract class PolarSeriesBase : ColorEachMarkerSeriesBase {
		public override ArgumentScaleType ArgumentScaleType { get { return ArgumentScaleType.Numerical; } set { ;  } }
	}
}
