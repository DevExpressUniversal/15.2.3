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
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public interface IMeasuredItemValueProvider {
		double GetValue(object item);
	}
	public class ItemAttributeValueProvider : AttributeValueProviderBase, IMeasuredItemValueProvider {
		double IMeasuredItemValueProvider.GetValue(object item) {
			return GetValue(item as MapItem);
		}
		protected override MapDependencyObject CreateObject() {
			return new ItemAttributeValueProvider();
		}
	}
	public class MeasureRules : MapDependencyObject, IOwnedElement, IWeakEventListener, IMeasureRules, ILegendDataProvider {
		public static readonly DependencyProperty RangeStopsProperty = DependencyPropertyManager.Register("RangeStops",
			typeof(DoubleCollection), typeof(MeasureRules), new PropertyMetadata(null, UpdateData));
		public static readonly DependencyProperty ValueProviderProperty = DependencyPropertyManager.Register("ValueProvider",
			typeof(IMeasuredItemValueProvider), typeof(MeasureRules), new PropertyMetadata(null, MeasureRulesOptionsChanged));
		public static readonly DependencyProperty RangeDistributionProperty = DependencyPropertyManager.Register("RangeDistribution",
			typeof(RangeDistributionBase), typeof(MeasureRules), new PropertyMetadata(null, MeasureRulesOptionsChanged));
		public static readonly DependencyProperty ApproximateValuesProperty = DependencyPropertyManager.Register("ApproximateValues",
			typeof(bool), typeof(MeasureRules), new PropertyMetadata(false, UpdateData));
		static void MeasureRulesOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MeasureRules rules = d as MeasureRules;
			if (rules != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as INotifyPropertyChanged, e.NewValue as INotifyPropertyChanged, rules);
				UpdateData(d, e);
			}
		}
		static void UpdateData(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MeasureRules rules = d as MeasureRules;
			if (rules != null) {
				rules.UpdateDataAdapter();
			}
		}
		[Category(Categories.Appearance), TypeConverter(typeof(ExpandableObjectConverter))]
		public IMeasuredItemValueProvider ValueProvider {
			get { return (IMeasuredItemValueProvider)GetValue(ValueProviderProperty); }
			set { SetValue(ValueProviderProperty, value); }
		}
		[Category(Categories.Appearance)]
		public RangeDistributionBase RangeDistribution {
			get { return (RangeDistributionBase)GetValue(RangeDistributionProperty); }
			set { SetValue(RangeDistributionProperty, value); }
		}
		[Category(Categories.Appearance)]
		public bool ApproximateValues {
			get { return (bool)GetValue(ApproximateValuesProperty); }
			set { SetValue(ApproximateValuesProperty, value); }
		}
		[Category(Categories.Appearance)]
		public DoubleCollection RangeStops {
			get { return (DoubleCollection)GetValue(RangeStopsProperty); }
			set { SetValue(RangeStopsProperty, value); }
		}
		object owner;
		protected internal ChartDataSourceAdapter DataAdapter { get { return owner as ChartDataSourceAdapter; } }
		RangeDistributionBase ActualRangeDistribution {
			get {
				return RangeDistribution != null ? RangeDistribution : LinearRangeDistribution.Default;
			}
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if (value != owner) {
					owner = value;
				}
			}
		}
		#endregion
		#region IWeakEventListener Implementation
		bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if ((sender is ItemAttributeValueProvider || sender is RangeDistributionBase)) {
					UpdateDataAdapter();
					success = true;
				}
			}
			return success;
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		#region IMeasureRules Implementation
		IList<double> IMeasureRules.RangeStops { get { return RangeStops; } }
		IRangeDistribution IMeasureRules.RangeDistribution { get { return ActualRangeDistribution; } }
		double IMeasureRules.GetValue(IMapChartItem item) {
			return ValueProvider != null ? ValueProvider.GetValue(item) : item.Value;
		}
		#endregion
		#region ILegendDataProvider implementation
		IList<MapLegendItemBase> ILegendDataProvider.CreateItems(MapLegendBase legend) {
			return CreateLegendItems(legend);
		}
		#endregion
		void UpdateDataAdapter() {
			if (DataAdapter != null)
				DataAdapter.ItemSizeChanged();
		}
		protected virtual IList<MapLegendItemBase> CreateLegendItems(MapLegendBase legend) {
			MeasureRulesLegendItemsBuilder builder = CreateLegendItemBuilder(legend);
			return builder != null ? builder.CreateItems() : new List<MapLegendItemBase>();
		}
		protected virtual MeasureRulesLegendItemsBuilder CreateLegendItemBuilder(MapLegendBase legend) {
			return new MeasureRulesLegendItemsBuilder(legend, this);
		}
		protected override MapDependencyObject CreateObject() {
			return new MeasureRules();
		}
	}
}
