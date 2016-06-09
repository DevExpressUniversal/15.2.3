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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public class MapScalePanel : Control {
		internal const double MaxScaleBarWidth = 100.0;
		public static readonly DependencyProperty KilometersScaleBarWidthProperty = DependencyPropertyManager.Register("KilometersScaleBarWidth",
			typeof(double), typeof(MapScalePanel), new PropertyMetadata(100.0));
		public static readonly DependencyProperty KilometersScaleTextProperty = DependencyPropertyManager.Register("KilometersScaleText",
			typeof(string), typeof(MapScalePanel), new PropertyMetadata("0 km"));
		public static readonly DependencyProperty MilesScaleBarWidthProperty = DependencyPropertyManager.Register("MilesScaleBarWidth",
			typeof(double), typeof(MapScalePanel), new PropertyMetadata(100.0));
		public static readonly DependencyProperty MilesScaleTextProperty = DependencyPropertyManager.Register("MilesScaleText",
			typeof(string), typeof(MapScalePanel), new PropertyMetadata("0 mi"));
		public static readonly DependencyProperty ShowKilometersScaleProperty = DependencyPropertyManager.Register("ShowKilometersScale",
			typeof(bool), typeof(MapScalePanel), new PropertyMetadata(true));
		public static readonly DependencyProperty ShowMilesScaleProperty = DependencyPropertyManager.Register("ShowMilesScale",
			typeof(bool), typeof(MapScalePanel), new PropertyMetadata(true));
		public static readonly DependencyProperty KilometersScaleProperty = DependencyPropertyManager.Register("KilometersScale",
			typeof(double), typeof(MapScalePanel), new PropertyMetadata(0.0, KilometersScalePropertyChanged));
		static void KilometersScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapScalePanel panel = d as MapScalePanel;
			if (panel != null)
				panel.Update((double)e.NewValue);
		}
		public double KilometersScaleBarWidth {
			get { return (double)GetValue(KilometersScaleBarWidthProperty); }
			set { SetValue(KilometersScaleBarWidthProperty, value); }
		}
		public string KilometersScaleText {
			get { return (string)GetValue(KilometersScaleTextProperty); }
			set { SetValue(KilometersScaleTextProperty, value); }
		}
		public double MilesScaleBarWidth {
			get { return (double)GetValue(MilesScaleBarWidthProperty); }
			set { SetValue(MilesScaleBarWidthProperty, value); }
		}
		public string MilesScaleText {
			get { return (string)GetValue(MilesScaleTextProperty); }
			set { SetValue(MilesScaleTextProperty, value); }
		}
		public bool ShowKilometersScale {
			get { return (bool)GetValue(ShowKilometersScaleProperty); }
			set { SetValue(ShowKilometersScaleProperty, value); }
		}
		public bool ShowMilesScale {
			get { return (bool)GetValue(ShowMilesScaleProperty); }
			set { SetValue(ShowMilesScaleProperty, value); }
		}
		public double KilometersScale {
			get { return (double)GetValue(KilometersScaleProperty); }
			set { SetValue(KilometersScaleProperty, value); }
		}
		double[] kilometersScaleRanges = new double[] { 10000.0, 5000.0, 2500.0, 1000.0, 500.0, 250.0, 200.0, 100.0, 50.0, 25.0, 20.0, 10.0, 5.0, 2.5, 1.0, 0.5, 0.25, 0.2, 0.1, 0.05, 0.025, 0.010, 0.005, 0.002, 0.001, 0.0005, 0.0002, 0.0001, 0.00005, 0.00001, 0.000005, 0.000001 };
		double[] milesScaleRanges = new double[] { 10000.0, 5000.0, 2500.0, 1000.0, 500.0, 250.0, 200.0, 100.0, 50.0, 25.0, 20.0, 10.0, 5.0, 2.5, 1.0, 0.5, 0.25, 0.2, 0.1, 0.05, 0.025, 0.010, 0.005 };
		double[] footsScaleRanges = new double[] { 5000.0, 2500.0, 1000.0, 500.0, 250.0, 200.0, 100.0, 50.0, 25.0, 20.0, 10.0, 5.0, 2.0, 1.0 };
		double[] inchesScaleRanges = new double[] { 6.0, 3.0, 2.0, 1.0 };
		public MapScalePanel() {
			DefaultStyleKey = typeof(MapScalePanel);
		}
		internal void Update(double kilometersScale) {
			CalculateMetersScale(kilometersScale);
			double milesScale = kilometersScale * 0.621371192;
			if (milesScale >= 1) {
				foreach (double scaleRange in milesScaleRanges)
					if (scaleRange <= milesScale) {
						MilesScaleBarWidth = scaleRange / milesScale * MaxScaleBarWidth;
						MilesScaleText = scaleRange.ToString() + " mi";
						break;
					}
			} else if (milesScale >= 1.0 / 5280.0) {
				double footsScale = milesScale * 5280.0;
				foreach (double scaleRange in footsScaleRanges)
					if (scaleRange <= footsScale) {
						MilesScaleBarWidth = scaleRange / footsScale * MaxScaleBarWidth;
						MilesScaleText = scaleRange.ToString() + " ft";
						break;
					}
			} else {
				double inchScale = milesScale * 5280.0 * 12.0;
				foreach (double scaleRange in inchesScaleRanges)
					if (scaleRange <= inchScale) {
						MilesScaleBarWidth = scaleRange / inchScale * MaxScaleBarWidth;
						MilesScaleText = scaleRange.ToString() + " in";
						break;
					}
			}
		}
		void CalculateMetersScale(double kilometersScale) {
			foreach (double scaleRange in kilometersScaleRanges)
				if (scaleRange <= kilometersScale) {
					KilometersScaleBarWidth = scaleRange / kilometersScale * MaxScaleBarWidth;
					KilometersScaleText = CalculateMetersText(scaleRange);
					return;
				}
			KilometersScaleBarWidth = MaxScaleBarWidth;
			KilometersScaleText = "0 mm";
		}
		string CalculateMetersText(double scaleRange) {
			if (scaleRange >= 1)
				return scaleRange.ToString() + " km";
			return scaleRange >= 0.001 ? (scaleRange * 1000.0).ToString() + " m" : (scaleRange * 1000000.0).ToString() + " mm";
		}
	}
	public class MapScalePanelLayoutControl : Control {
		public MapScalePanelLayoutControl() {
			DefaultStyleKey = typeof(MapScalePanelLayoutControl);
		}
	}
}
namespace DevExpress.Xpf.Map.Native {
	public class ScalePanelInfo : OverlayInfoBase {
		double kilometersScale;
		public double KilometersScale {
			get { return kilometersScale; }
			set {
				if (kilometersScale != value) {
					kilometersScale = value;
					RaisePropertyChanged("KilometersScale");
				}
			}
		}
		public ScalePanelInfo(MapControl map) : base(map) {
		}
		protected internal override Control CreatePresentationControl() {
			return new MapScalePanelLayoutControl();
		}
	}
}
