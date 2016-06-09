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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.TreeMap.Native;
namespace DevExpress.Xpf.TreeMap {
	public class TreeMapRangeColorizer : TreeMapPaletteColorizerBase {
		public static readonly DependencyProperty GroupColorProperty = DependencyProperty.Register("GroupColor", typeof(Color), typeof(TreeMapRangeColorizer), new PropertyMetadata(Colors.Transparent));
		public static readonly DependencyProperty RangeStopsProperty = DependencyProperty.Register("RangeStops", typeof(DoubleCollection), typeof(TreeMapRangeColorizer), new PropertyMetadata(OnRangeStopsPropertyChanged));
		static void OnRangeStopsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		public Color GroupColor {
			get { return (Color)GetValue(GroupColorProperty); }
			set { SetValue(GroupColorProperty, value); }
		}
		[TypeConverter(typeof(DoubleCollectionConverter))]
		public DoubleCollection RangeStops {
			get { return (DoubleCollection)GetValue(RangeStopsProperty); }
			set { SetValue(RangeStopsProperty, value); }
		}
		protected override TreeMapDependencyObject CreateObject() {
			return new TreeMapRangeColorizer();
		}
		public override Color? GetItemColor(TreeMapItem item, TreeMapItemGroupInfo group) {
			if (item.IsGroup)
				return GroupColor;
			return RangeColorizerCalculator.GetItemColor(item, RangeStops, Palette);
		}
	}
}
namespace DevExpress.Xpf.TreeMap.Native {
	public static class RangeColorizerCalculator {
		public static Color? GetItemColor(TreeMapItem item, DoubleCollection rangeStops, PaletteBase colors) {
			if (colors != null && colors.Count > 0 && rangeStops != null && rangeStops.Count > 0) {
				double value = item.GetActualValue();
				int rangeIndex = GetRangeIndex(value, rangeStops);
				if (rangeIndex >= 0) {
					if (colors.Count < rangeStops.Count) {
						double normalizedValue = (double)rangeIndex / (rangeStops.Count - 1);
						return CalculateApproximatedColor(normalizedValue, colors);
					}
					else
						return colors[rangeIndex];
				}
			}
			return null;
		}
		public static Color MixColors(Color fromUnit, Color toUnit, double ratio) {
			return Color.FromArgb(MixChannel(fromUnit.A, toUnit.A, ratio),
								  MixChannel(fromUnit.R, toUnit.R, ratio),
								  MixChannel(fromUnit.G, toUnit.G, ratio),
								  MixChannel(fromUnit.B, toUnit.B, ratio));
		}
		static byte MixChannel(byte fromValue, byte toValue, double ratio) {
			return (byte)(fromValue * (1.0 - ratio) + toValue * ratio);
		}
		static Color CalculateApproximatedColor(double normalized, PaletteBase colors) {
			double paletteNormalized = normalized * (colors.Count - 1);
			int index = (int)Math.Floor(paletteNormalized);
			double ratio = (paletteNormalized - index);
			return MixColors(colors[index], colors[index + 1], ratio);
		}
		static int GetRangeIndex(double value, DoubleCollection rangeStops) {
			List<double> sortedRangeStops = CheckSortedRangeStops(rangeStops);
			if (rangeStops.Count > 0) {
				for (int index = 0; index < rangeStops.Count - 1; index++)
					if (value >= sortedRangeStops[index] && value < sortedRangeStops[index + 1])
						return index;
			}
			return -1;
		}
		static List<double> CheckSortedRangeStops(DoubleCollection rangeStops) {
			List<double> sortedRangeStops = rangeStops.Distinct().ToList();
			sortedRangeStops.Remove(double.NaN);
			sortedRangeStops.Sort(Comparer<double>.Default);
			return sortedRangeStops;
		}
	}
}
