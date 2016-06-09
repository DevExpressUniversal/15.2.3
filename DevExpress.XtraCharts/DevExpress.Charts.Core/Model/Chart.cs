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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace DevExpress.Charts.Model {
	public abstract class Chart : ModelElement {
		ChartTitleCollection titles;
		Legend legend;
		SeriesCollection series;
		object dataSource;
		IModelListener listener;
		Palette palette;
		ChartAppearanceOptions appearance;
		public SeriesCollection Series { get { return series; } }
		public ChartTitleCollection Titles { get { return titles; } }
		public object DataSource {
			get { return dataSource; }
			set {
				if(Object.Equals(dataSource, value))
					return;
				dataSource = value;
				NotifyParent(this, "DataSource", dataSource);
			}
		}
		public Legend Legend {
			get { return legend; }
			set {
				if(this.legend == value)
					return;
				UpdateElementParent(this.legend, null);
				this.legend = value;
				UpdateElementParent(this.legend, this);
				NotifyParent(this, "Legend", legend);
			}
		}
		public Palette Palette {
			get { return palette; }
			set {
				if (palette != value) {
					UpdateElementParent(palette, null);
					palette = value;
					UpdateElementParent(palette, this);
					NotifyParent(this, "Palette", value);
				}
			}
		}
		public ChartAppearanceOptions Appearance {
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
		protected Chart() {
			this.series = new SeriesCollection(this);
			this.titles = new ChartTitleCollection(this);
		}
		public virtual void SetListener(IModelListener listener) {
			this.listener = listener;
		}
		protected internal override void NotifyParent(ModelElement element, string propertyName, object value) {
			UpdateInfo update = new UpdateInfo(element, propertyName, value);
			if(listener != null) listener.OnModelUpdated(update);
		}
		protected internal abstract bool IsSeriesValid(SeriesModel value);
	}
	public class CartesianChart : Chart {
		bool rotated;
		double barDistance;
		int barDistanceFixed;
		Axis argumentAxis;
		Axis valueAxis;
		AxisCollection secondaryArgumentAxes;
		AxisCollection secondaryValueAxes;
		public bool Rotated {
			get { 
				return rotated; 
			}
			set {
				if (rotated == value)
					return;
				rotated = value;
				NotifyParent(this, "Rotated", rotated);
			}
		}
		public Axis ArgumentAxis {
			get { return argumentAxis; }
			set {
				if(argumentAxis == value)
					return;
				UpdateElementParent(argumentAxis, null);
				argumentAxis = value;
				UpdateElementParent(argumentAxis, this);
				NotifyParent(this, "ArgumentAxis", argumentAxis);
			}
		}
		public Axis ValueAxis {
			get { return valueAxis; }
			set {
				if(valueAxis == value)
					return;
				UpdateElementParent(valueAxis, null);
				valueAxis = value;
				UpdateElementParent(valueAxis, this);
				NotifyParent(this, "ValueAxis", valueAxis);
			}
		}
		public double BarDistance {
			get { return barDistance; }
			set {
				if (value == barDistance)
					return;
				barDistance = value;
				NotifyParent(this, "BarDistance", barDistance);
			}
		}
		public int BarDistanceFixed {
			get { return barDistanceFixed; }
			set {
				if (value == barDistanceFixed)
					return;
				barDistanceFixed = value;
				NotifyParent(this, "BarDistanceFixed", barDistanceFixed);
			}
		}
		public AxisCollection SecondaryArgumentAxes { get { return secondaryArgumentAxes; } }
		public AxisCollection SecondaryValueAxes { get { return secondaryValueAxes; } }
		public CartesianChart() {
			this.secondaryArgumentAxes = new AxisCollection(this);
			this.secondaryValueAxes = new AxisCollection(this);
			this.barDistanceFixed = 1;
		}
		protected internal override bool IsSeriesValid(SeriesModel value) {
			return typeof(CartesianSeriesBase).IsAssignableFrom(value.GetType());
		}
	}
	public class Cartesian3DChart : CartesianChart, IChart3D {
		Options3D options3D;
		public IOptions3D Options3D { get { return options3D; } }
		public Cartesian3DChart() {
			options3D = new Options3D(this);
		}
		protected internal override bool IsSeriesValid(SeriesModel value) {
			return typeof(SeriesModel).IsAssignableFrom(value.GetType());
		}
	}
	public class PieChart : Chart {
		protected internal override bool IsSeriesValid(SeriesModel value) {
			return typeof(PieSeriesBase).IsAssignableFrom(value.GetType());
		}
	}
	public class Pie3DChart : PieChart, IChart3D {
		Options3D options3D;
		public IOptions3D Options3D { get { return options3D; } }
		public Pie3DChart() { 
			options3D = new Options3D(this);
		}
	}
	public abstract class CircularChart : Chart {
		DirectionMode direction = DirectionMode.Counterclockwise;
		CircularAxisY valueAxis;
		CircularDiagramStyle style = CircularDiagramStyle.Circle;
		public DirectionMode Direction {
			get { return direction; }
			set {
				if (direction != value) {
					direction = value;
					NotifyParent(this, "Direction", value);
				}
			}
		}
		public CircularAxisY ValueAxis {
			get { return valueAxis; }
			set {
				if (valueAxis == value)
					return;
				UpdateElementParent(valueAxis, null);
				valueAxis = value;
				UpdateElementParent(valueAxis, this);
				NotifyParent(this, "ValueAxis", valueAxis);
			}
		}
		public CircularDiagramStyle Style {
			get { return style; }
			set {
				if (style != value) {
					style = value;
					NotifyParent(this, "Style", value);
				}
			}
		}
	}
	public class RadarChart : CircularChart {
		RadarAxisX argumentAxis;
		public RadarAxisX ArgumentAxis {
			get { return argumentAxis; }
			set {
				if (argumentAxis == value)
					return;
				UpdateElementParent(argumentAxis, null);
				argumentAxis = value;
				UpdateElementParent(argumentAxis, this);
				NotifyParent(this, "ArgumentAxis", argumentAxis);
			}
		}
		protected internal override bool IsSeriesValid(SeriesModel value) {
			return typeof(RadarSeriesBase).IsAssignableFrom(value.GetType());
		}
	}
	public class PolarChart : CircularChart {
		PolarAxisX argumentAxis;
		public PolarAxisX ArgumentAxis {
			get { return argumentAxis; }
			set {
				if (argumentAxis == value)
					return;
				UpdateElementParent(argumentAxis, null);
				argumentAxis = value;
				UpdateElementParent(argumentAxis, this);
				NotifyParent(this, "ArgumentAxis", argumentAxis);
			}
		}
		protected internal override bool IsSeriesValid(SeriesModel value) {
			return typeof(PolarSeriesBase).IsAssignableFrom(value.GetType());
		}
	}
	public interface IChart3D {
		IOptions3D Options3D { get; }
	}
	public interface IOptions3D {
		int RotationAngleX { get; set; }
		int RotationAngleY  { get; set; }
		int RotationAngleZ  { get; set; }
		int PerspectiveAngle  { get; set; }
		bool EnableAntialiasing { get; set; }
	}
	public class Options3D : ModelElement, IOptions3D {
		bool enableAntialiasing;
		int rotationAngleX = 20;
		int rotationAngleY = -40;
		int rotationAngleZ = 0;
		int perspectiveAngle = 50;
		public Options3D(ModelElement parent) : base(parent) {
		}
		public int RotationAngleX { 
			get { return rotationAngleX; }
			set {
				if(rotationAngleX != value) {
					rotationAngleX  = value;
					NotifyParent(this, "RotationAngleX", value);
				}
			}
		}
		public int RotationAngleY { 
			get { return rotationAngleY; }
			set {
				if(rotationAngleY != value) {
					rotationAngleY  = value;
					NotifyParent(this, "RotationAngleY", value);
				}
			}
		}
		public int RotationAngleZ { 
			get { return rotationAngleZ; }
			set {
				if(rotationAngleZ != value) {
					rotationAngleZ  = value;
					NotifyParent(this, "RotationAngleZ", value);
				}
			}
		}
		public int PerspectiveAngle { 
			get { return perspectiveAngle; }
			set {
				if(perspectiveAngle != value) {
					perspectiveAngle  = value;
					NotifyParent(this, "PerspectiveAngle", value);
				}
			}
		}
		public bool EnableAntialiasing {
			get { return enableAntialiasing; }
			set {
				if (enableAntialiasing != value) {
					enableAntialiasing = value;
					NotifyParent(this, "EnableAntialiasing", value);
				}
			}
		}
	}
}
