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
using DevExpress.Xpf.Layout.Core;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Docking {
	public interface IResizeCalculator {
		void Resize(BaseLayoutItem item1, BaseLayoutItem item2, double change);
		void Init(SplitBehavior splitBehavior);
		Orientation Orientation { get; set; }
	}
	public enum SplitBehavior { Split, Resize1, Resize2, PixelSplit }
	class ResizeData {
		public DefinitionBase Definition1;
		public int Definition1Index;
		public DefinitionBase Definition2;
		public int Definition2Index;
		public Grid Grid;
		public double OriginalDefinition1ActualLength;
		public GridLength OriginalDefinition1Length;
		public double OriginalDefinition2ActualLength;
		public GridLength OriginalDefinition2Length;
		public GridResizeDirection ResizeDirection;
		public SplitBehavior SplitBehavior;
		public int SplitterIndex;
		public double SplitterLength;
	}
	abstract class BaseResizeCalculator : IResizeCalculator {
		#region IResizeCalculator Members
		abstract public void Resize(BaseLayoutItem item1, BaseLayoutItem item2, double change);
		public virtual void Init(SplitBehavior splitBehavior) {
			this.SplitBehavior = splitBehavior;
		}
		#endregion
		public SplitBehavior SplitBehavior { get; set; }
		public Orientation Orientation { get; set; }
		protected virtual double GetMin(BaseLayoutItem item) {
			bool isColumn = Orientation == System.Windows.Controls.Orientation.Horizontal;
			if(item is FixedItem) return 5;
#if DEBUGTEST
			if(item is LayoutPanel) return 12;
#else
			if(item is LayoutPanel) return isColumn ? 12 : 18;
#endif
			if(item is TabbedGroup) return 24;
			if(item is LayoutControlItem) return isColumn ? 24 : 10;
			return 12;
		}
		protected double GetActualLength(DefinitionBase definition) {
			ColumnDefinition column = definition as ColumnDefinition;
			if(column != null) return column.ActualWidth;
			return ((RowDefinition)definition).ActualHeight;
		}
	}
	class RecursiveResizeCalculator : BaseResizeCalculator {
		public class ResizeInfo {
			public double Length { get; set; }
			public GridUnitType GridUnitType { get; set; }
			public BaseLayoutItem Item { get; set; }
		}
		static double AbsMin(double value1, double value2) {
			return Math.Min(Math.Abs(value1), Math.Abs(value2));
		}
		protected List<ResizeInfo> Infos = new List<ResizeInfo>();
		double MeasureChange(BaseLayoutItem def1, BaseLayoutItem def2, double change) {
			double res = change;
			if(def1 is LayoutGroup) {
				LayoutGroup group = (LayoutGroup)def1;
				if(CanMeasureRecursively(group)) {
					var itemToResize = GetItemToResize(group, false);
					if(itemToResize != null) {
						res = AbsMin(res, MeasureChangeRecursive(itemToResize, def2, change));
					}
				}
			}
			if(def2 is LayoutGroup) {
				LayoutGroup group = (LayoutGroup)def2;
				if(CanMeasureRecursively(group)) {
					var itemToResize = GetItemToResize(group, true);
					if(itemToResize != null) {
						res = AbsMin(res, MeasureChangeRecursive(itemToResize, def1, -change, false));
					}
				}
			}
			DefinitionBase d1 = DefinitionsHelper.GetDefinition(def1);
			DefinitionBase d2 = DefinitionsHelper.GetDefinition(def2);
			double actualLength1 = GetActualLength(d1);
			double actualLength2 = GetActualLength(d2);
			double total = actualLength1 + actualLength2;
			double max;
			double min = Math.Min(GetMin(def1) + GetMin(def2), total * 0.5);
			if(change < 0) {
				min = Math.Max(min, DefinitionsHelper.UserMinSizeValueCache(d1));
				max = Math.Max(total - min, DefinitionsHelper.UserMinSizeValueCache(d2));
			}
			else {
				min = Math.Max(min, DefinitionsHelper.UserMinSizeValueCache(d2));
				max = Math.Max(total - min, DefinitionsHelper.UserMinSizeValueCache(d1));
			}
			double l1 = MathHelper.MeasureDimension(min, max, actualLength1 + change);
			double l2 = MathHelper.MeasureDimension(min, max, actualLength2 - change);
			res = AbsMin(l1 - actualLength1, res);
			res = AbsMin(l2 - actualLength2, res);
			return change < 0 ? -res : res;
		}
		double MeasureChangeRecursive(BaseLayoutItem def1, BaseLayoutItem def2, double change, bool first = true) {
			DefinitionBase d1 = DefinitionsHelper.GetDefinition(def1);
			DefinitionBase d2 = DefinitionsHelper.GetDefinition(def2);
			double actualLength1 = GetActualLength(d1);
			double actualLength2 = GetActualLength(d2);
			double total = actualLength1 + actualLength2;
			double min = Math.Min(GetMin(def1) + GetMin(def2), total * 0.5);
			min = Math.Max(min, DefinitionsHelper.UserMinSizeValueCache(d1));
			double max = Math.Min(total - min, DefinitionsHelper.UserMaxSizeValueCache(d1));
			double l1 = MathHelper.MeasureDimension(min, max, actualLength1 + change);
			double res = change;
			if(def1 is LayoutGroup) {
				LayoutGroup group = (LayoutGroup)def1;
				if(CanMeasureRecursively(group)) {
					var itemToResize = GetItemToResize(group, first);
					if(itemToResize != null)
						res = AbsMin(res, MeasureChangeRecursive(itemToResize, def2, change));
				}
			}
			return AbsMin(res, actualLength1 - l1);
		}
		protected virtual bool CanMeasureRecursively(LayoutGroup group) {
			return group.Orientation == Orientation && !group.IgnoreOrientation && !group.IsControlItemsHost;
		}
		BaseLayoutItem GetItemToResize(LayoutGroup group, bool first) {
			var items = first ? group.Items.ToArray() : group.Items.Reverse();
			foreach(BaseLayoutItem item in items) {
				if(LayoutItemsHelper.IsResizable(item, group.Orientation == System.Windows.Controls.Orientation.Horizontal))
					return item;
			}
			return null;
		}
		Dictionary<object, double> StarsHash = new Dictionary<object, double>();
		Dictionary<object, double> PixelHash = new Dictionary<object, double>();
		double GetHashedPixelValue(LayoutGroup parent) {
			double pixels = 0;
			if(!PixelHash.Keys.Contains(parent)) {
				foreach(BaseLayoutItem _item in parent.Items) {
					DefinitionBase def = DefinitionsHelper.GetDefinition(_item);
					if(DefinitionsHelper.IsStar(def)) {
						pixels += DefinitionsHelper.UserActualSizeValueCache(def);
					}
				}
				PixelHash.Add(parent, pixels);
			}
			else
				pixels = PixelHash[parent];
			return pixels;
		}
		double GetHashedStarValue(LayoutGroup parent) {
			double stars = 0;
			if(!StarsHash.Keys.Contains(parent)) {
				foreach(BaseLayoutItem _item in parent.Items) {
					DefinitionBase def = DefinitionsHelper.GetDefinition(_item);
					if(DefinitionsHelper.IsStar(def)) {
						stars += DefinitionsHelper.UserSizeValueCache(def).Value;
					}
				}
				StarsHash.Add(parent, stars);
			}
			else
				stars = StarsHash[parent];
			return stars;
		}
		private void Measure(BaseLayoutItem def1, BaseLayoutItem def2, double change) {
			bool res = true;
			if(def1 is LayoutGroup) {
				LayoutGroup group = (LayoutGroup)def1;
				if(CanMeasureRecursively(group)) {
					var itemToResize = GetItemToResize(group, false);
					if(itemToResize != null)
						res = MeasureRecursive(itemToResize, def2, change);
				}
			}
			if(res && def2 is LayoutGroup) {
				LayoutGroup group = (LayoutGroup)def2;
				if(CanMeasureRecursively(group)) {
					var itemToResize = GetItemToResize(group, true);
					if(itemToResize != null)
						res = MeasureRecursive(itemToResize, def1, -change, false);
				}
			}
			DefinitionBase d1 = DefinitionsHelper.GetDefinition(def1);
			DefinitionBase d2 = DefinitionsHelper.GetDefinition(def2);
			change = GetChange(d1, d2, change);
			double actualLength1 = GetActualLength(d1);
			double actualLength2 = GetActualLength(d2);
			double l1 = actualLength1 + change;
			double l2 = actualLength2 - change;
			if(actualLength1 == l1 || actualLength2 == l2 || !res) {
				Infos.Clear();
				return;
			}
			switch(SplitBehavior) {
				case SplitBehavior.Split: {
						if(!StarsHash.Keys.Contains(d1)) {
							StarsHash.Add(d1, DefinitionsHelper.UserSizeValueCache(d1).Value);
						}
						if(!StarsHash.Keys.Contains(d2)) {
							StarsHash.Add(d2, DefinitionsHelper.UserSizeValueCache(d2).Value);
						}
						double ratio = l1 / (l1 + l2);
						double totalStars = StarsHash[d1] + StarsHash[d2];
						l1 = ratio * totalStars;
						l2 = (1 - ratio) * totalStars;
						Infos.Add(new ResizeInfo() { Length = l1, GridUnitType = GridUnitType.Star, Item = def1 });
						Infos.Add(new ResizeInfo() { Length = l2, GridUnitType = GridUnitType.Star, Item = def2 });
					}
					break;
				case SplitBehavior.Resize1: {
						LayoutGroup parent = def1.Parent;
						double stars = GetHashedStarValue(parent);
						double pixels = GetHashedPixelValue(parent);
						l2 = l2 / pixels * stars;
						Infos.Add(new ResizeInfo() { Length = l1, GridUnitType = GridUnitType.Pixel, Item = def1 });
						Infos.Add(new ResizeInfo() { Length = l2, GridUnitType = GridUnitType.Star, Item = def2 });
					}
					break;
				case SplitBehavior.Resize2: {
						LayoutGroup parent = def1.Parent;
						double stars = GetHashedStarValue(parent);
						double pixels = GetHashedPixelValue(parent);
						l1 = l1 / pixels * stars;
						Infos.Add(new ResizeInfo() { Length = l2, GridUnitType = GridUnitType.Pixel, Item = def2 });
						Infos.Add(new ResizeInfo() { Length = l1, GridUnitType = GridUnitType.Star, Item = def1 });
					}
					break;
				case SplitBehavior.PixelSplit:
					Infos.Add(new ResizeInfo() { Length = l1, GridUnitType = GridUnitType.Pixel, Item = def1 });
					Infos.Add(new ResizeInfo() { Length = l2, GridUnitType = GridUnitType.Pixel, Item = def2 });
					break;
			}
		}
		bool MeasureRecursive(BaseLayoutItem def1, BaseLayoutItem def2, double change, bool first = true) {
			DefinitionBase d1 = DefinitionsHelper.GetDefinition(def1);
			DefinitionBase d2 = DefinitionsHelper.GetDefinition(def2);
			change = GetChange(d1, d2, change);
			double actualLength1 = GetActualLength(d1);
			double actualLength2 = GetActualLength(d2);
			double total = actualLength1 + actualLength2;
			double min = Math.Min(GetMin(def1) + GetMin(def2), total * 0.5);
			double l1 = actualLength1 + change;
			if(actualLength1 + change < min)
				return false;
			if(def1 is LayoutGroup && (((LayoutGroup)def1).Orientation == Orientation && !(((LayoutGroup)def1).IgnoreOrientation))) {
				LayoutGroup gr1 = (LayoutGroup)def1;
				var itemToResize = GetItemToResize(gr1, first);
				if(itemToResize != null) {
					MeasureRecursive(itemToResize, def2, change);
				}
			}
			if(DefinitionsHelper.IsStar(d1)) {
				LayoutGroup parent = def1.Parent;
				double stars = GetHashedStarValue(parent);
				double pixels = GetHashedPixelValue(parent);
				l1 = l1 / pixels * stars;
				Infos.Add(new ResizeInfo() { Length = l1, GridUnitType = GridUnitType.Star, Item = def1 });
			}
			if(DefinitionsHelper.IsAbsolute(d1)) {
				Infos.Add(new ResizeInfo() { Length = l1, GridUnitType = GridUnitType.Pixel, Item = def1 });
			}
			return true;
		}
		protected virtual void ApplyMeasure() {
			foreach(var info in Infos) {
				if(!MathHelper.IsConstraintValid(info.Length)) continue;
				info.Item.ResizeLockHelper.Lock();
				info.Item.SetValue(
					Orientation == Orientation.Horizontal ? BaseLayoutItem.ItemWidthProperty : BaseLayoutItem.ItemHeightProperty,
					new GridLength(info.Length, info.GridUnitType));
			}
			Infos.Clear();
		}
		double GetChange(DefinitionBase def1, DefinitionBase def2, double change) {
			double min, max;
			GetDeltaConstraints(def1, def2, out min, out max);
			return Math.Min(Math.Max(change, min), max);
		}
		void GetDeltaConstraints(DefinitionBase def1, DefinitionBase def2, out double minDelta, out double maxDelta) {
			double actualLength1 = GetActualLength(def1);
			double actualLength2 = GetActualLength(def2);
			double def1MinSize = DefinitionsHelper.UserMinSizeValueCache(def1);
			double def1MaxSize = DefinitionsHelper.UserMaxSizeValueCache(def1);
			double def2MinSize = DefinitionsHelper.UserMinSizeValueCache(def2);
			double def2MaxSize = DefinitionsHelper.UserMaxSizeValueCache(def2);
			minDelta = -Math.Min(actualLength1 - def1MinSize, def2MaxSize - actualLength2);
			maxDelta = Math.Min(def1MaxSize - actualLength1, actualLength2 - def2MinSize);
		}
		public override void Resize(BaseLayoutItem def1, BaseLayoutItem def2, double change) {
			double ch = MeasureChange(def1, def2, change);
			Measure(def1, def2, ch);
			ApplyMeasure();
		}
		public override void Init(SplitBehavior splitBehavior) {
			base.Init(splitBehavior);
			StarsHash.Clear();
			PixelHash.Clear();
			Infos.Clear();
		}
	}
	class LayoutResizeCalculator : RecursiveResizeCalculator {
		protected override bool CanMeasureRecursively(LayoutGroup group) {
			return false;
		}
		protected override void ApplyMeasure() {
			foreach(var info in Infos) {
				if(!MathHelper.IsConstraintValid(info.Length)) continue;
				info.Length = Math.Round(info.Length, 2);
			}
			base.ApplyMeasure();
		}
	}
	public static class DefinitionsHelper {
		internal static GridLength ZeroLength = new GridLength(0);
		internal static GridLength ZeroStarLength = new GridLength(0, GridUnitType.Star);
		internal static bool IsZero(DefinitionBase definition) {
			return UserSizeValueCache(definition).Equals(ZeroLength);
		}
		public static readonly DependencyProperty DefinitionProperty =
			DependencyProperty.RegisterAttached("Definition", typeof(DefinitionBase), typeof(DefinitionsHelper), null);
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public static DefinitionBase GetDefinition(DependencyObject target) {
			return (DefinitionBase)target.GetValue(DefinitionProperty);
		}
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public static void SetDefinition(DependencyObject target, DefinitionBase value) {
			target.SetValue(DefinitionProperty, value);
		}
		static bool IsColumnDefinition(DefinitionBase def) {
			return def is ColumnDefinition;
		}
		public static double UserMinSizeValueCache(DefinitionBase def) {
			return (double)def.GetValue(IsColumnDefinition(def) ? ColumnDefinition.MinWidthProperty : RowDefinition.MinHeightProperty);
		}
		public static double UserMaxSizeValueCache(DefinitionBase def) {
			return (double)def.GetValue(IsColumnDefinition(def) ? ColumnDefinition.MaxWidthProperty : RowDefinition.MaxHeightProperty);
		}
		public static bool IsStar(DefinitionBase def) {
			return UserSizeValueCache(def).IsStar;
		}
		public static bool IsAbsolute(DefinitionBase def) {
			return UserSizeValueCache(def).IsAbsolute;
		}
		public static GridLength UserSizeValueCache(DefinitionBase def) {
			return (GridLength)def.GetValue(IsColumnDefinition(def) ? ColumnDefinition.WidthProperty : RowDefinition.HeightProperty);
		}
		public static double UserActualSizeValueCache(DefinitionBase def) {
			return IsColumnDefinition(def) ? ((ColumnDefinition)def).ActualWidth : ((RowDefinition)def).ActualHeight;
		}
		public static double GetActualLength(this DefinitionBase def) {
			return UserActualSizeValueCache(def);
		}
		public static GridLength GetLength(this DefinitionBase def) {
			return UserSizeValueCache(def);
		}
		internal static double GetMaxLength(this DefinitionBase def) {
			return (double)def.GetValue(IsColumnDefinition(def) ? ColumnDefinition.MaxWidthProperty : RowDefinition.MaxHeightProperty);
		}
		internal static double GetMinLength(this DefinitionBase def) {
			return (double)def.GetValue(IsColumnDefinition(def) ? ColumnDefinition.MinWidthProperty : RowDefinition.MinHeightProperty);
		}
		internal static void SetMaxSize(this DefinitionBase def, Size maxSize) {
			bool isColumn = IsColumnDefinition(def);
			DependencyProperty maxSizeProperty = isColumn ? ColumnDefinition.MaxWidthProperty : RowDefinition.MaxHeightProperty;
			double value = isColumn ? maxSize.Width : maxSize.Height;
			if(MathHelper.IsConstraintValid(value)) def.SetValue(maxSizeProperty, value);
		}
		internal static void SetMinSize(this DefinitionBase def, Size minSize) {
			bool isColumn = IsColumnDefinition(def);
			DependencyProperty minSizeProperty = isColumn ? ColumnDefinition.MinWidthProperty : RowDefinition.MinHeightProperty;
			double value = isColumn ? minSize.Width : minSize.Height;
			if(MathHelper.IsConstraintValid(value)) def.SetValue(minSizeProperty, value);
		}
	}
}
