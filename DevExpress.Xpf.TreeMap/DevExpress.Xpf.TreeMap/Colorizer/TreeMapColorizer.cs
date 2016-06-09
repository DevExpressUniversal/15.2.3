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
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.TreeMap {
	public abstract class TreeMapColorizerBase : TreeMapDependencyObject {
		public abstract Color? GetItemColor(TreeMapItem item, TreeMapItemGroupInfo group);
	}
	[ContentProperty("Palette")]
	public abstract class TreeMapPaletteColorizerBase : TreeMapColorizerBase, IWeakEventListener {
		public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register("Palette", typeof(PaletteBase), typeof(TreeMapColorizerBase), new PropertyMetadata(OnPaletteChanged));
		TreeMapControl TreeMap { get { return Owner as TreeMapControl; } }
		static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapPaletteColorizerBase colorizer = d as TreeMapPaletteColorizerBase;
			if (colorizer != null) {
				if (e.OldValue != null)
					PropertyChangedWeakEventManager.RemoveListener(e.OldValue as PaletteBase, colorizer);
				if (e.NewValue != null)
					PropertyChangedWeakEventManager.AddListener(e.NewValue as PaletteBase, colorizer);
				colorizer.UpdateColors();
			}
		}
		public TreeMapPaletteColorizerBase() {
			SetValue(PaletteProperty, new Office2016Palette());
		}
		public PaletteBase Palette {
			get { return (PaletteBase)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (sender is PaletteBase && e is PropertyChangedEventArgs) {
				UpdateColors();
				return true;
			}
			return false;
		}
		#endregion
		void UpdateColors() {
			if (TreeMap != null)
				TreeMap.ColorizeItems();
		}
	}
	public struct TreeMapItemGroupInfo {
		public int GroupLevel { get; private set; }
		public int GroupIndex { get; private set; }
		public int ItemIndex { get; private set; }
		public double MinValue { get; private set; }
		public double MaxValue { get; private set; }
		public TreeMapItemGroupInfo(int groupLevel, int groupIndex, int itemIndex, double minValue, double maxValue)
			: this() {
			GroupLevel = groupLevel;
			GroupIndex = groupIndex;
			ItemIndex = itemIndex;
			MinValue = minValue;
			MaxValue = maxValue;
		}
	}
}
namespace DevExpress.Xpf.TreeMap.Native {
	public class ColorizerController {
		struct MinMax {
			public static MinMax Empty { get { return new MinMax(Double.NaN, Double.NaN); } }
			double min, max;
			public double Min { get { return min; } }
			public double Max { get { return max; } }
			public MinMax(double min, double max) {
				this.min = max;
				this.max = max;
			}
			public void ApplyValue(double value) {
				if (!double.IsNaN(value)) {
					min = double.IsNaN(min) ? value : Math.Min(value, min);
					max = double.IsNaN(max) ? value : Math.Max(value, max);
				}
			}
		}
		ColorizerBrushCache colorizerBrushCache;
		TreeMapControl treeMap;
		TreeMapColorizerBase Colorizer { get { return treeMap.Colorizer; } }
		TreeMapItemCollection Items { get { return treeMap.ActualItems; } }
		bool CanColorize { get { return Colorizer != null && Items != null && Items.Count > 0; } }
		public ColorizerController(TreeMapControl treeMap) {
			this.treeMap = treeMap;
			this.colorizerBrushCache = new ColorizerBrushCache();
		}
		SolidColorBrush GetItemBrush(TreeMapItem item, TreeMapItemGroupInfo group) {
			Color? itemColor = Colorizer.GetItemColor(item, group);
			if (itemColor.HasValue)
				return colorizerBrushCache.GetBrush(itemColor.Value);
			return null;
		}
		MinMax CalculateMinMax(TreeMapItemCollection items) {
			MinMax minMax = MinMax.Empty;
			foreach (TreeMapItem item in items)
				minMax.ApplyValue(item.GetActualValue());
			return minMax;
		}
		int ColorizeItems(TreeMapItemCollection items, int groupLevel, int groupIndex) {
			int groupCount = 1;
			MinMax minMax = CalculateMinMax(items);
			for (int i = 0; i < items.Count; i++) {
				TreeMapItem item = items[i];
				item.ColorizerBrush = GetItemBrush(item, new TreeMapItemGroupInfo(groupLevel, groupIndex, i, minMax.Min, minMax.Max));
				if (item.IsGroup)
					groupCount += ColorizeItems(item.Children, groupLevel + 1, groupIndex + groupCount);
			}
			return groupCount;
		}
		public void ColorizeItems() {
			if (CanColorize)
				ColorizeItems(Items, 0, 0);
		}
		public void ClearBrushCache() {
			colorizerBrushCache.Clear();
		}
	}
	public class ColorizerBrushCache {
		readonly Dictionary<Color, SolidColorBrush> brushCache = new Dictionary<Color, SolidColorBrush>();
		public SolidColorBrush GetBrush(Color color) {
			SolidColorBrush brush = null;
			if (!brushCache.TryGetValue(color, out brush)) {
				brush = new SolidColorBrush(color);
				brushCache.Add(color, brush);
			}
			return brush;
		}
		public void Clear() {
			brushCache.Clear();
		}
	}
}
