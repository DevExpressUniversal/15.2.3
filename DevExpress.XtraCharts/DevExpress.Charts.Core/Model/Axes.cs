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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Charts.Native;
namespace DevExpress.Charts.Model {
	public enum AxisPosition { Left, Right, Top, Bottom }
	public enum DateTimeGridAlignment {
		Millisecond = DateTimeGridAlignmentNative.Millisecond,
		Second = DateTimeGridAlignmentNative.Second,
		Minute = DateTimeGridAlignmentNative.Minute,
		Hour = DateTimeGridAlignmentNative.Hour,
		Day = DateTimeGridAlignmentNative.Day,
		Week = DateTimeGridAlignmentNative.Week,
		Month = DateTimeGridAlignmentNative.Month,
		Quarter = DateTimeGridAlignmentNative.Quarter,
		Year = DateTimeGridAlignmentNative.Year
	}
	public class AxisTitle : TitleBase {
		string text;
		bool visible;
		public string Text {
			get { return text; }
			set {
				if (text != value) {
					text = value;
					NotifyParent(this, "Text", value);
				}
			}
		}
		public bool Visible {
			get { return visible; }
			set {
				if (visible != value) {
					visible = value;
					NotifyParent(this, "Visible", value);
				}
			}
		}
		public AxisTitle(Axis parent) : base(parent) {
		}
	}
	public abstract class AxisBase : ModelElement {
		bool gridLinesVisible;
		bool gridLinesMinorVisible;
		AxisLabel label;
		AxisRange range;
		AxisAppearance appearance;
		bool visible = true;
		public bool GridLinesVisible {
			get { return gridLinesVisible; }
			set {
				if (gridLinesVisible != value) {
					gridLinesVisible = value;
					NotifyParent(this, "GridLinesVisible", value);
				}
			}
		}
		public bool GridLinesMinorVisible {
			get { return gridLinesMinorVisible; }
			set {
				if (gridLinesMinorVisible != value) {
					gridLinesMinorVisible = value;
					NotifyParent(this, "GridLinesMinorVisible", value);
				}
			}
		}
		public AxisLabel Label {
			get { return label; }
			set {
				if (label != value) {
					label = value;
					NotifyParent(this, "Label", value);
				}
			}
		}
		public AxisRange Range {
			get { return range; }
			set {
				if (range != value) {
					range = value;
					NotifyParent(this, "Range", value);
				}
			}
		}
		public AxisAppearance Appearance {
			get { return appearance; }
			set {
				if (appearance != value) {
					UpdateElementParent(appearance, null);
					appearance = value;
					UpdateElementParent(appearance, this);
					NotifyParent(this, "Appearance", value);
				}
			}
		}
		public bool Visible {
			get { return visible; }
			set {
				if (visible != value) {
					visible = value;
					NotifyParent(this, "Visible", value);
				}
			}
		}
		public AxisBase() : this(null) {
		}
		public AxisBase(Chart parent) : base(parent) {
		}
	}
	public abstract class NonPolarAxis : AxisBase {
		double gridSpacing = 1.0;
		DateTimeGridAlignment gridAlignment = DateTimeGridAlignment.Day;
		bool autoGrid = true;
		bool logarithmic;
		double logarithmicBase = 10.0;
		public double GridSpacing {
			get { return gridSpacing; }
			set {
				if (gridSpacing != value) {
					gridSpacing = value;
					NotifyParent(this, "GridSpacing", value);
				}
			}
		}
		public DateTimeGridAlignment GridAlignment {
			get { return gridAlignment; }
			set {
				if (gridAlignment != value) {
					gridAlignment = value;
					NotifyParent(this, "GridAlignment", value);
				}
			}
		}
		public bool AutoGrid {
			get { return autoGrid; }
			set {
				if (autoGrid != value) {
					autoGrid = value;
					NotifyParent(this, "AutoGrid", value);
				}
			}
		}
		public bool Logarithmic {
			get { return logarithmic; }
			set {
				if (logarithmic != value) {
					logarithmic = value;
					NotifyParent(this, "Logarithmic", value);
				}
			}
		}
		public double LogarithmicBase {
			get { return logarithmicBase; }
			set {
				if (logarithmicBase != value) {
					logarithmicBase = value;
					NotifyParent(this, "LogarithmicBase", value);
				}
			}
		}
		public NonPolarAxis() : this(null) {
		}
		public NonPolarAxis(Chart parent): base(parent){
		}
	}
	public class Axis : NonPolarAxis {
		AxisPosition position = AxisPosition.Left;
		bool tickmarksVisible;
		bool tickmarksMinorVisible;
		bool tickmarksCrossAxis;
		AxisTitle title;
		bool reverse;
		public AxisPosition Position {
			get { return position; }
			set {
				if (position == value)
					return;
				position = value;
				NotifyParent(this, "Position", value);
			}
		}
		public bool TickmarksVisible {
			get { return tickmarksVisible; }
			set {
				if (tickmarksVisible != value) {
					tickmarksVisible = value;
					NotifyParent(this, "TickmarksVisible", value);
				}
			}
		}
		public bool TickmarksMinorVisible {
			get { return tickmarksMinorVisible; }
			set {
				if (tickmarksMinorVisible != value) {
					tickmarksMinorVisible = value;
					NotifyParent(this, "TickmarksMinorVisible", value);
				}
			}
		}
		public bool TickmarksCrossAxis {
			get { return tickmarksCrossAxis; }
			set {
				if (tickmarksCrossAxis != value) {
					tickmarksCrossAxis = value;
					NotifyParent(this, "TickmarksCrossAxis", value);
				}
			}
		}
		public AxisTitle Title {
			get { return title; }
			set {
				if (title != value) {
					title = value;
					NotifyParent(this, "Title", value);
				}
			}
		}
		public bool Reverse {
			get { return reverse; }
			set {
				if (reverse != value) {
					reverse = value;
					NotifyParent(this, "Reverse", value);
				}
			}
		}
		public Axis() : this(null) {
		}
		public Axis(CartesianChart parent) : base(parent) {
		}
	}
	public class RadarAxisX : NonPolarAxis {
	}
	public class PolarAxisX : AxisBase {
	}
	public class CircularAxisY : NonPolarAxis {
		bool tickmarksVisible;
		bool tickmarksMinorVisible;
		bool tickmarksCrossAxis;
		public bool TickmarksVisible {
			get { return tickmarksVisible; }
			set {
				if (tickmarksVisible != value) {
					tickmarksVisible = value;
					NotifyParent(this, "TickmarksVisible", value);
				}
			}
		}
		public bool TickmarksMinorVisible {
			get { return tickmarksMinorVisible; }
			set {
				if (tickmarksMinorVisible != value) {
					tickmarksMinorVisible = value;
					NotifyParent(this, "TickmarksMinorVisible", value);
				}
			}
		}
		public bool TickmarksCrossAxis {
			get { return tickmarksCrossAxis; }
			set {
				if (tickmarksCrossAxis != value) {
					tickmarksCrossAxis = value;
					NotifyParent(this, "TickmarksCrossAxis", value);
				}
			}
		}
	}
	public interface IAxisLabelFormatter : IAxisLabelFormatterCore {
	}
	public class AxisLabel : ModelElement {
		bool visible = true;
		DefaultBoolean enableAntialiasing = DefaultBoolean.Default;
		IAxisLabelFormatter formatter;
		public bool Visible {
			get { return visible; }
			set {
				if (visible != value) {
					visible = value;
					NotifyParent(this, "Visible", value);
				}
			}
		}
		public IAxisLabelFormatter Formatter {
			get { return formatter; }
			set {
				if(formatter != value){
					formatter = value;
					NotifyParent(this, "Formatter", value);
				}
			}
		}
		public DefaultBoolean EnableAntialiasing {
			get { return enableAntialiasing; }
			set {
				if (enableAntialiasing != value) {
					enableAntialiasing = value;
					NotifyParent(this, "EnableAntialiasing", value);
				}
			}
		}
		public AxisLabel(AxisBase parent) : base(parent) {
		}
	}
	public class AxisRange : ModelElement {
		object minValue = null;
		object maxValue = null;
		public object MinValue {
			get { return minValue; }
			set {
				if (minValue != value) {
					minValue = value;
					NotifyParent(this, "MinValue", value);
				}
			}
		}
		public object MaxValue {
			get { return maxValue; }
			set {
				if (maxValue != value) {
					maxValue = value;
					NotifyParent(this, "MaxValue", value);
				}
			}
		}
		public AxisRange(AxisBase parent) : base(parent) {
		}
	}
	public class AxisCollection : ModelElementCollection<Axis> {
		public AxisCollection(CartesianChart parent)
			: base(parent) {
		}
	}
}
