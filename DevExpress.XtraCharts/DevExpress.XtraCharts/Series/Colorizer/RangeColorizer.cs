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

using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					  "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(RangeColorizerTypeConverter))
	]
	public class RangeColorizer : ChartPaletteColorizerBase, IPatternHolder {
		static byte MixChannel(byte fromValue, byte toValue, double ratio) {
			return (byte)(fromValue * (1.0 - ratio) + toValue * ratio);
		}
		internal static Color MixColors(Color fromUnit, Color toUnit, double ratio) {
			return Color.FromArgb(MixChannel(fromUnit.A, toUnit.A, ratio),
								  MixChannel(fromUnit.R, toUnit.R, ratio),
								  MixChannel(fromUnit.G, toUnit.G, ratio),
								  MixChannel(fromUnit.B, toUnit.B, ratio));
		}
		const string DefaultLegendPattern = "{V1} - {V2}";
		readonly IColorizerValueProvider defaultValueProvider;
		readonly DoubleCollection rangeStops;
		List<double> sortedRangeStops;
		IColorizerValueProvider valueProvider = null;
		bool dirty;
		string legendPattern;
		IColorizerValueProvider ActualValueProvider {
			get {
				if (valueProvider == null)
					return defaultValueProvider;
				return valueProvider;
			}
		}
		int RangeCount { get { return sortedRangeStops.Count - 1; } }
		string ActualLegendPattern { get { return !string.IsNullOrEmpty(legendPattern) ? legendPattern : DefaultLegendPattern; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeColorizerRangeStops"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeColorizer.RangeStops"),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty
		]
		public DoubleCollection RangeStops { get { return rangeStops; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeColorizerLegendItemPattern"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeColorizer.LegendItemPattern"),
		Editor("DevExpress.XtraCharts.Design.RangeColorizerLegendItemPatternEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty]
		public string LegendItemPattern {
			get { return legendPattern; }
			set {
				if (value != legendPattern) {
					RaisePropertyChanging();
					legendPattern = value;
					RaisePropertyChanged("LegendItemPattern");
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeColorizerValueProvider"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeColorizer.ValueProvider"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DefaultValue(null),
		Browsable(false)
		]
		public IColorizerValueProvider ValueProvider {
			get { return valueProvider; }
			set {
				RaisePropertyChanging();
				valueProvider = value;
				RaisePropertyChanged("ValueProvider");
			}
		}
		public RangeColorizer() {
			defaultValueProvider = new NumericColorizerValueProvider();
			rangeStops = new DoubleCollection();
			rangeStops.CollectionChanged += OnRangeStopsCollectionChanged;
			rangeStops.CollectionChanging += OnRangeStopsCollectionChanging;
			sortedRangeStops = new List<double>();
		}
		#region IPatternHolder
		PatternDataProvider IPatternHolder.GetDataProvider(PatternConstants patternConstant) {
			return new RangeColorizerLegendItemDataProvider(patternConstant);
		}
		string IPatternHolder.PointPattern { get { return ActualLegendPattern; } }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeLegendItemPattern() {
			return !String.IsNullOrEmpty(legendPattern);
		}
		void ResetLegendItemPattern() {
			LegendItemPattern = string.Empty;
		}
		bool ShouldSerializeRangeStops() {
			return RangeStops.Count > 0;
		}
		internal bool ShouldSerialize() {
			return ShouldSerializeLegendItemPattern() || ShouldSerializeRangeStops();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "LegendItemPattern":
					return ShouldSerializeLegendItemPattern();
				case "RangeStops":
					return ShouldSerializeRangeStops();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		void OnRangeStopsCollectionChanging(object sender, CollectionChangingEventArgs<double> e) {
			RaisePropertyChanging();
		}
		void OnRangeStopsCollectionChanged(object sender, CollectionChangedEventArgs<double> e) {
			dirty = true;
			RaisePropertyChanged("RangeStops");
		}
		void CheckSortedRangeStops() {
			if (dirty) {
				sortedRangeStops.Clear();
				sortedRangeStops.AddRange(rangeStops);
				sortedRangeStops.Remove(double.NaN);
				sortedRangeStops.Remove(double.MaxValue);
				sortedRangeStops.Remove(double.MinValue);
				sortedRangeStops.Sort(Comparer<double>.Default);
				dirty = false;
			}
		}
		int GetRangeIndex(double value) {
			CheckSortedRangeStops();
			int index = 0;
			for (index = 0; index < RangeCount; index++) {
				if (sortedRangeStops[index] <= value && value < sortedRangeStops[index + 1])
					return index;
			}
			return -1;
		}
		Color CalculateApproximatedColor(double normalized, Palette palette) {
			double paletteNormalized = normalized * (palette.Count - 1);
			int index = (int)Math.Floor(paletteNormalized);
			if (index == palette.Count - 1)
				return palette[index].Color;
			double ratio = (paletteNormalized - index);
			return MixColors(palette[index].Color, palette[index + 1].Color, ratio);
		}
		string GetText(double min, double max) {
			PatternParser patternParser = new PatternParser(ActualLegendPattern, this);
			patternParser.SetContext(new double[] { min, max });
			return patternParser.GetText();
		}
		protected override List<LegendItem> GetLegendItems(Palette palette, bool textVisible, Color textColor, Font legendFont, bool markerVisible, Size markerSize) {
			List<LegendItem> items = new List<LegendItem>();
			if (palette.Count == 0 || RangeCount == 0)
				return items;
			CheckSortedRangeStops();
			for (int index = 0; index < RangeCount; index++) {
				Color color = Color.Empty;
				if (palette.Count < RangeCount)
					color = CalculateApproximatedColor((double)index / (RangeCount - 1), palette);
				else
					color = palette[index].Color;
				string text;
				double min = sortedRangeStops[index];
				double max = sortedRangeStops[index + 1];
				text = GetText(min, max);
				items.Add(new LegendItem(text, color, textColor, markerSize, markerVisible, textVisible));
			}
			return items;
		}
		internal string[] GetAvailablePatternPlaceholders() {
			return new string[2] { ToolTipPatternUtils.Value1Pattern, ToolTipPatternUtils.Value2Pattern };
		}
		public override Color GetPointColor(object argument, object[] values, object colorKey, Palette palette) {
			if (rangeStops.Count < 2 || palette.Count == 0)
				return Color.Empty;
			double value = ActualValueProvider.GetValue(colorKey);
			if (double.IsNaN(value))
				return Color.Empty;
			int index = GetRangeIndex(value);
			if (index == -1)
				return Color.Empty;
			if (palette.Count < RangeCount)
				return CalculateApproximatedColor((double)index / (RangeCount - 1), palette);
			else
				return palette[index].Color;
		}
	}
	public class NumericColorizerValueProvider : IColorizerValueProvider {
		double IColorizerValueProvider.GetValue(object obj) {
			if (obj != null) {
				if (DataItemsHelper.IsNumericalType(obj.GetType()))
					return Convert.ToDouble(obj);
				if (obj is string) {
					double result;
					if (double.TryParse((string)obj, out result))
						return result;
				}
			}
			return double.NaN;
		}
	}
	public class DoubleCollection : NotificationCollection<double> { }
}
