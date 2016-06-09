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
using System.Windows;
using System.Windows.Media;
using DevExpress.Map.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class ChoroplethColorizer : MapColorizer, IWeakEventListener, ILegendDataProvider {
		public static readonly DependencyProperty RangeStopsProperty = DependencyPropertyManager.Register("RangeStops",
			typeof(DoubleCollection), typeof(ChoroplethColorizer), new PropertyMetadata(null, InvalidateColorsAndLegend));
		public static readonly DependencyProperty ValueProviderProperty = DependencyPropertyManager.Register("ValueProvider",
			typeof(IColorizerValueProvider), typeof(ChoroplethColorizer), new PropertyMetadata(null, ColorizerOptionsChanged));
		public static readonly DependencyProperty RangeDistributionProperty = DependencyPropertyManager.Register("RangeDistribution",
			typeof(RangeDistributionBase), typeof(ChoroplethColorizer), new PropertyMetadata(null, ColorizerOptionsChanged));
		public static readonly DependencyProperty ApproximateColorsProperty = DependencyPropertyManager.Register("ApproximateColors",
			typeof(bool), typeof(ChoroplethColorizer), new PropertyMetadata(false, InvalidateColorsAndLegend));
		[Category(Categories.Appearance), TypeConverter(typeof(ExpandableObjectConverter))]
		public IColorizerValueProvider ValueProvider {
			get { return (IColorizerValueProvider)GetValue(ValueProviderProperty); }
			set { SetValue(ValueProviderProperty, value); }
		}
		[Category(Categories.Appearance)]
		public RangeDistributionBase RangeDistribution {
			get { return (RangeDistributionBase)GetValue(RangeDistributionProperty); }
			set { SetValue(RangeDistributionProperty, value); }
		}
		[Category(Categories.Appearance)]
		public bool ApproximateColors {
			get { return (bool)GetValue(ApproximateColorsProperty); }
			set { SetValue(ApproximateColorsProperty, value); }
		}
		[Category(Categories.Appearance)]
		public DoubleCollection RangeStops {
			get { return (DoubleCollection)GetValue(RangeStopsProperty); }
			set { SetValue(RangeStopsProperty, value); }
		}
		static void ColorizerOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChoroplethColorizer colorizer = d as ChoroplethColorizer;
			if (colorizer != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as INotifyPropertyChanged, e.NewValue as INotifyPropertyChanged, colorizer);
				InvalidateColorsAndLegend(d, e);
			}
		}
		protected RangeDistributionBase ActualRangeDistribution {
			get {
				return RangeDistribution != null ? RangeDistribution : LinearRangeDistribution.Default;
			}
		}
		public ChoroplethColorizer() {
		}
		#region IWeakEventListener Implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		#region ILegendDataProvider Members
		IList<MapLegendItemBase> ILegendDataProvider.CreateItems(MapLegendBase legend) {
			return CreateLegendItems(legend);
		}
		#endregion
		bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if ((sender is ShapeAttributeValueProvider || sender is RangeDistributionBase)) {
					Invalidate();
					success = true;
				}
			}
			return success;
		}
		Color GetColor(IList<double> sortedRangeStops, double value) {
			return WpfColorizerColorHelper.CalculateValue(ActualColors, sortedRangeStops, ApproximateColors, ActualRangeDistribution, value);
		}
		protected virtual IList<MapLegendItemBase> CreateLegendItems(MapLegendBase legend) {
			ColorizerLegendItemsBuilderBase builder = CreateLegendItemBuilder(legend);
			return builder != null ? builder.CreateItems() : new List<MapLegendItemBase>();
		}
		protected virtual ColorizerLegendItemsBuilderBase CreateLegendItemBuilder(MapLegendBase legend) {
			return new ChoroplethColorizerLegendItemsBuilder(legend, this);
		}
		protected override MapDependencyObject CreateObject() {
			return new ChoroplethColorizer();
		}
		public Color GetColor(double value) {
			return GetColor(CoreUtils.SortDoubleCollection(RangeStops), value);
		}
		public override Color GetItemColor(IColorizerElement item) {
			if (item == null)
				return DefaultColor;
			Color actualItemColorizerColor = item.ColorizerColor;
			if (ValueProvider == null ||
				ActualColors == null ||
				RangeStops == null)
				return actualItemColorizerColor;
			double value = ValueProvider.GetValue(item);
			if (double.IsNaN(value) || double.IsInfinity(value))
				return actualItemColorizerColor;
			return GetColor(CoreUtils.SortDoubleCollection(RangeStops), value);
		}
	}
}
