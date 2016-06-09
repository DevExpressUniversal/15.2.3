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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Map.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map;
using DevExpress.Xpf.Map.Native;
using System.Globalization;
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public abstract class MapCoordinatesPanel : Control {
		public static readonly DependencyProperty CoordPointProperty = DependencyPropertyManager.Register("CoordPoint",
			typeof(CoordPoint), typeof(MapCoordinatesPanel), new PropertyMetadata(null, CoordPointPropertyChanged));
		public CoordPoint CoordPoint {
			get { return (CoordPoint)GetValue(CoordPointProperty); }
			set { SetValue(CoordPointProperty, value); }
		}
		static void CoordPointPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapCoordinatesPanel coordinatesPanel = d as MapCoordinatesPanel;
			if (coordinatesPanel != null)
				coordinatesPanel.CalculateCoordinates();
		}
		protected abstract void CalculateCoordinates();
	}
	[NonCategorized]
	public class MapGeoCoordinatesPanel : MapCoordinatesPanel {
		public static readonly DependencyProperty LatitudeDegreesProperty = DependencyPropertyManager.Register("LatitudeDegrees",
			typeof(string), typeof(MapGeoCoordinatesPanel), new PropertyMetadata("0"));
		public static readonly DependencyProperty LatitudeMinutesProperty = DependencyPropertyManager.Register("LatitudeMinutes",
			typeof(string), typeof(MapGeoCoordinatesPanel), new PropertyMetadata("0"));
		public static readonly DependencyProperty LatitudeSecondsProperty = DependencyPropertyManager.Register("LatitudeSeconds",
			typeof(string), typeof(MapGeoCoordinatesPanel), new PropertyMetadata("0"));
		public static readonly DependencyProperty LatitudeCardinalPointProperty = DependencyPropertyManager.Register("LatitudeCardinalPoint",
			typeof(string), typeof(MapGeoCoordinatesPanel), new PropertyMetadata("N"));
		public static readonly DependencyProperty LongitudeDegreesProperty = DependencyPropertyManager.Register("LongitudeDegrees",
			typeof(string), typeof(MapGeoCoordinatesPanel), new PropertyMetadata("0"));
		public static readonly DependencyProperty LongitudeMinutesProperty = DependencyPropertyManager.Register("LongitudeMinutes",
			typeof(string), typeof(MapGeoCoordinatesPanel), new PropertyMetadata("0"));
		public static readonly DependencyProperty LongitudeSecondsProperty = DependencyPropertyManager.Register("LongitudeSeconds",
			typeof(string), typeof(MapGeoCoordinatesPanel), new PropertyMetadata("0"));
		public static readonly DependencyProperty LongitudeCardinalPointProperty = DependencyPropertyManager.Register("LongitudeCardinalPoint",
			typeof(string), typeof(MapGeoCoordinatesPanel), new PropertyMetadata("E"));
		public string LatitudeDegrees {
			get { return (string)GetValue(LatitudeDegreesProperty); }
			set { SetValue(LatitudeDegreesProperty, value); }
		}
		public string LatitudeMinutes {
			get { return (string)GetValue(LatitudeMinutesProperty); }
			set { SetValue(LatitudeMinutesProperty, value); }
		}
		public string LatitudeSeconds {
			get { return (string)GetValue(LatitudeSecondsProperty); }
			set { SetValue(LatitudeSecondsProperty, value); }
		}
		public string LatitudeCardinalPoint {
			get { return (string)GetValue(LatitudeCardinalPointProperty); }
			set { SetValue(LatitudeCardinalPointProperty, value); }
		}
		public string LongitudeDegrees {
			get { return (string)GetValue(LongitudeDegreesProperty); }
			set { SetValue(LongitudeDegreesProperty, value); }
		}
		public string LongitudeMinutes {
			get { return (string)GetValue(LongitudeMinutesProperty); }
			set { SetValue(LongitudeMinutesProperty, value); }
		}
		public string LongitudeSeconds {
			get { return (string)GetValue(LongitudeSecondsProperty); }
			set { SetValue(LongitudeSecondsProperty, value); }
		}
		public string LongitudeCardinalPoint {
			get { return (string)GetValue(LongitudeCardinalPointProperty); }
			set { SetValue(LongitudeCardinalPointProperty, value); }
		}
		public MapGeoCoordinatesPanel() {
			DefaultStyleKey = typeof(MapGeoCoordinatesPanel);
		}
		protected override void CalculateCoordinates() {
			double latitude = CoordPoint.GetY();
			double absLatitude = Math.Abs(latitude);
			double longitude = CoordPoint.GetX();
			double absLongitude = Math.Abs(longitude);
			LatitudeDegrees = MathUtils.GetDegrees(absLatitude, 0).ToString() + "°";
			LatitudeMinutes = MathUtils.GetMinutes(absLatitude, 0).ToString() + "'";
			LatitudeSeconds = MathUtils.GetSeconds(absLatitude, 2).ToString() + "''";
			LatitudeCardinalPoint = latitude >= 0 ? "N" : "S";
			LongitudeDegrees = MathUtils.GetDegrees(absLongitude, 0).ToString() + "°";
			LongitudeMinutes = MathUtils.GetMinutes(absLongitude, 0).ToString() + "'";
			LongitudeSeconds = MathUtils.GetSeconds(absLongitude, 2).ToString() + "''";
			LongitudeCardinalPoint = longitude >= 0 ? "E" : "W";
		}
	}
	[NonCategorized]
	public class MapCartesianCoordinatesPanel : MapCoordinatesPanel {
		public static readonly DependencyProperty CoordXProperty = DependencyPropertyManager.Register("CoordX",
			typeof(string), typeof(MapCartesianCoordinatesPanel), new PropertyMetadata(0.ToString("F2")));
		public static readonly DependencyProperty CoordYProperty = DependencyPropertyManager.Register("CoordY",
			typeof(string), typeof(MapCartesianCoordinatesPanel), new PropertyMetadata(0.ToString("F2")));
		public static readonly DependencyProperty MeasureUnitProperty = DependencyPropertyManager.Register("MeasureUnit",
			typeof(string), typeof(MapCartesianCoordinatesPanel), new PropertyMetadata());
		public string CoordX {
			get { return (string)GetValue(CoordXProperty); }
			set { SetValue(CoordXProperty, value); }
		}
		public string CoordY {
			get { return (string)GetValue(CoordYProperty); }
			set { SetValue(CoordYProperty, value); }
		}
		public string MeasureUnit {
			get { return (string)GetValue(MeasureUnitProperty); }
			set { SetValue(MeasureUnitProperty, value); }
		}
		public MapCartesianCoordinatesPanel() {
			DefaultStyleKey = typeof(MapCartesianCoordinatesPanel);
		}
		protected override void CalculateCoordinates() {
			CoordX = CoordPoint.GetX().ToString("F2");
			CoordY = CoordPoint.GetY().ToString("F2");
		}
	}
	[NonCategorized]
	public class CoordinatesPanelTemplateSelector : DataTemplateSelector {
		public DataTemplate GeoCoordinatesPanelTemplate { get; set; }
		public DataTemplate CartesianCoordinatesPanelTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			if (item is GeoCoordinatesPanelInfo)
				return GeoCoordinatesPanelTemplate;
			if (item is CartesianCoordinatesPanelInfo)
				return CartesianCoordinatesPanelTemplate;
			return null;
		}
	}
	public class MapCoordinatesPanelLayoutControl : Control {
		public MapCoordinatesPanelLayoutControl() {
			DefaultStyleKey = typeof(MapCoordinatesPanelLayoutControl);
		}
	}
}
namespace DevExpress.Xpf.Map.Native {
	public class CoordinatesPanelInfo : OverlayInfoBase {
		CoordPoint coordPoint;
		public CoordPoint CoordPoint {
			get { return coordPoint; }
			set {
				if (coordPoint != value) {
					coordPoint = value;
					RaisePropertyChanged("CoordPoint");
				}
			}
		}
		public CoordinatesPanelInfo(MapControl map)
			: base(map) {
		}
		protected internal override Control CreatePresentationControl() {
			return new MapCoordinatesPanelLayoutControl();
		}
	}
	public class GeoCoordinatesPanelInfo : CoordinatesPanelInfo {
		public GeoCoordinatesPanelInfo(MapControl map)
			: base(map) {
		}
	}
	public class CartesianCoordinatesPanelInfo : CoordinatesPanelInfo {
		string measureUnit;
		public CartesianCoordinatesPanelInfo(MapControl map, string measureUnit)
			: base(map) {
			this.measureUnit = measureUnit;
		}
		public string MeasureUnit {
			get { return measureUnit; }
			set {
				if (measureUnit != value) {
					measureUnit = value;
					RaisePropertyChanged("MeasureUnit");
				}
			}
		}
	}
}
