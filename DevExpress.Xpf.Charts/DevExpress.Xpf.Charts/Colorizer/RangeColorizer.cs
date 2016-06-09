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
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public interface IColorizerValueProvider {
		double GetValue(object obj);
	}
	public class RangeColorizer : LegendSupportColorizerBase {
		public static readonly DependencyProperty ValueProviderProperty = DependencyPropertyManager.Register("ValueProvider", typeof(IColorizerValueProvider), typeof(RangeColorizer));
		public static readonly DependencyProperty RangeStopsProperty = DependencyPropertyManager.Register("RangeStops", typeof(DoubleCollection), typeof(RangeColorizer), new PropertyMetadata(OnRangeStopsPropertyChanged));
		[Category(Categories.Appearance),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true),
		TypeConverter(typeof(ExpandableObjectConverter))]
		public IColorizerValueProvider ValueProvider {
			get { return (IColorizerValueProvider)GetValue(ValueProviderProperty); }
			set { SetValue(ValueProviderProperty, value); }
		}
		[Category(Categories.Appearance),
		XtraSerializableProperty(XtraSerializationVisibility.SimpleCollection)]
		public DoubleCollection RangeStops {
			get { return (DoubleCollection)GetValue(RangeStopsProperty); }
			set { SetValue(RangeStopsProperty, value); }
		}
		static void OnRangeStopsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RangeColorizer colorizer = d as RangeColorizer;
			if (colorizer != null)
				colorizer.OnRangeStopsChanged(e.OldValue as ColorizerKeysCollection, e.NewValue as ColorizerKeysCollection);
		}
		readonly IColorizerValueProvider defaultValueProvider = new NumericColorizerValueProvider();
		List<double> sortedRangeStops;
		IColorizerValueProvider ActualValueProvider { get { return ValueProvider != null ? ValueProvider : defaultValueProvider; } }
		int RangeCount { get { return sortedRangeStops.Count - 1; } }
		public RangeColorizer() {
			SetValue(RangeStopsProperty, new DoubleCollection());
			SetValue(LegendTextPatternProperty, "{V1} - {V2}");
		}
		void OnRangeStopsChanged(ColorizerKeysCollection oldRangeStops, ColorizerKeysCollection newRangeStops) {
			if (oldRangeStops != null)
				oldRangeStops.CollectionChanged -= RangeStopsCollectionChanged;
			if (newRangeStops != null)
				newRangeStops.CollectionChanged += RangeStopsCollectionChanged;
			sortedRangeStops = null;
		}
		void RangeStopsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			sortedRangeStops = null;
			NotifyPropertyChanged("RangeStops");
		}
		int GetRangeIndex(double value) {
			CheckSortedRangeStops();
			if (RangeCount > 0) {
				for (int index = 0; index < RangeCount; index++)
					if (value >= sortedRangeStops[index] && value < sortedRangeStops[index + 1])
						return index;
			}
			return -1;
		}
		void CheckSortedRangeStops() {
			if (sortedRangeStops == null) {
				sortedRangeStops = RangeStops.Distinct().ToList();
				sortedRangeStops.Remove(double.NaN);
				sortedRangeStops.Sort(Comparer<double>.Default);
			}
		}
		Color MixColors(Color fromUnit, Color toUnit, double ratio) {
			return Color.FromArgb(MixChannel(fromUnit.A, toUnit.A, ratio),
								  MixChannel(fromUnit.R, toUnit.R, ratio),
								  MixChannel(fromUnit.G, toUnit.G, ratio),
								  MixChannel(fromUnit.B, toUnit.B, ratio));
		}
		byte MixChannel(byte fromValue, byte toValue, double ratio) {
			return (byte)(fromValue * (1.0 - ratio) + toValue * ratio);
		}
		Color CalculateApproximatedColor(double normalized, Palette palette) {
			double paletteNormalized = normalized * (palette.Count - 1);
			int index = (int)Math.Floor(paletteNormalized);
			double ratio = (paletteNormalized - index);
			return MixColors(palette[index], palette[index + 1], ratio);
		}
		protected override ChartDependencyObject CreateObject() {
			return new RangeColorizer();
		}
		protected override PatternDataProvider GetPatternDataProvider(PatternConstants patternConstant) {
			return new RangeColorizerLegendItemDataProvider(patternConstant);
		}
		protected override void UpdateLegendItems(List<ColorizerLegendItem> legendItems, Palette palette) {
			CheckSortedRangeStops();
			if (palette.Count > 0 && RangeCount > 0) {
				for (int i = 0; i < RangeCount; i++) {
					Color color = (palette.Count < RangeCount) ? CalculateApproximatedColor((double)i / (RangeCount - 1), palette) : palette[i];
					string text = CreateLegendItemText(new double[] { sortedRangeStops[i], sortedRangeStops[i + 1] });
					legendItems.Add(new ColorizerLegendItem(color, text));
				}
			}
		}
		public override Color? GetPointColor(object argument, object[] values, object colorKey, Palette palette) {
			if (colorKey != null && palette != null && palette.Count > 0 && RangeStops != null && RangeStops.Count > 0) {
				double value = ActualValueProvider.GetValue(colorKey);
				if (!double.IsNaN(value)) {
					int index = GetRangeIndex(value);
					if (index >= 0) {
						if (palette.Count < RangeCount) {
							double normalizedValue = (double)index / (RangeCount - 1);
							return CalculateApproximatedColor(normalizedValue, palette);
						}
						else
							return palette[index];
					}
				}
			}
			return null;
		}
	}
	public class NumericColorizerValueProvider : ChartDependencyObject, IColorizerValueProvider {
		double IColorizerValueProvider.GetValue(object obj) {
			return DataItemsHelper.ParseNumerical(obj) ?? double.NaN;
		}
		protected override ChartDependencyObject CreateObject() {
			return new NumericColorizerValueProvider();
		}
	}
}
