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

using System.Collections;
using System.Drawing;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public class GaugeRangeModel {
		public double MinRangeValue { get; set; }
		public double MaxRangeValue { get; set; }
		public int MajorTickCount { get; set; }
		public int MinorTickCount { get; set; }
	}
	public class GaugeModel : ISizable, IValuesProvider {
		readonly string[] seriesLabel;
		readonly string[] valueLabel;
		readonly GaugeRangeModel range;
		readonly float gaugeValue;
		readonly float? target;
		readonly IList selectionValues;
		readonly IndicatorType deltaIndicatorType;
		readonly bool deltaIsGood;
		readonly Color valueColor;
		bool visible;
		int top;
		int left;
		int width;
		int height;
		Point offset;
		string measureId;
		IList IValuesProvider.SelectionValues { get { return selectionValues; } }
		string IValuesProvider.MeasureID { get { return measureId; } }
		public string[] SeriesLabel { get { return seriesLabel; } }
		public string[] ValueLabel { get { return valueLabel; } }
		public GaugeRangeModel Range { get { return range; } }
		public float Value { get { return gaugeValue; } }
		public float? Target { get { return target; } }
		public IndicatorType DeltaIndicatorType { get { return deltaIndicatorType; } }
		public bool DeltaIsGood { get { return deltaIsGood; } }
		public Color ValueColor { get { return valueColor; } }
		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}
		public int Top {
			get { return top - offset.Y; }
			set { top = value; }
		}
		public int Left {
			get { return left - offset.X; }
			set { left = value; }
		}
		public int Width {
			get { return width; }
			set { width = value; }
		}
		public int Height {
			get { return height; }
			set { height = value; }
		}
		public Rectangle Bounds {
			get {
				int padding = (int)(width * DashboardGaugeControlViewer.DefaultBorderProportion / (1 + DashboardGaugeControlViewer.DefaultBorderProportion));
				return new Rectangle(Left + padding / 2, Top + padding / 2, width - padding, height - padding);
			}
		}
		public GaugeModel(string[] valueLabel, string[] seriesLabel, float value, float? target, GaugeRangeModel range, IList selectionValues, IndicatorType deltaIndicatorType, bool deltaIsGood, Color valueColor, string measureId) {
			this.valueLabel = valueLabel;
			this.seriesLabel = seriesLabel;
			this.gaugeValue = value;
			this.target = target;
			this.range = range;
			this.selectionValues = selectionValues;
			this.deltaIndicatorType = deltaIndicatorType;
			this.deltaIsGood = deltaIsGood;
			this.valueColor = valueColor;
			this.measureId = measureId;
		}
		public bool IsVisible(Rectangle clientBounds) {
			Rectangle bounds = Bounds;
			return clientBounds.Contains(bounds) || clientBounds.IntersectsWith(bounds);
		}
		void ISizable.SetOffset(Point offset) {
			this.offset = offset;
		}
	}
}
