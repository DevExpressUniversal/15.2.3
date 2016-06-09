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
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.XtraCharts.Native {
	public class LegendItemGroup : IEnumerable {
		LegendItemViewData[] items;
		IEnumerator IEnumerable.GetEnumerator() { return items.GetEnumerator(); }
		public int Count { get { return items.Length; } }
		public LegendItemViewData this[int index] { get { return items[index]; } }
		public LegendItemGroup(LegendItemViewData[] items) {
			this.items = items;
		}
		public void Render(IRenderer renderer, Point location) {
			foreach (LegendItemViewData item in items)
				item.Render(renderer, location);
		}
	}
	public abstract class LegendItemGroupingAlgorithm {
		static LegendItemGroupingAlgorithm CreateInstance(Legend legend, List<LegendItemViewData> items, Size maxSize) {
			switch (legend.Direction) {
				case LegendDirection.TopToBottom:
				case LegendDirection.BottomToTop:
					return new VerticalLegendItemGroupingAlgorithm(legend, items, maxSize);
				case LegendDirection.LeftToRight:
				case LegendDirection.RightToLeft:
					if (legend.EquallySpacedItems)
						return new HorizontalGroupingAlgorithmWithVerticalAllignment(legend, items, maxSize);
					else
						return new HorizontalLegendItemGroupingAlgorithm(legend, items, maxSize);
				default:
					throw new DefaultSwitchException();
			}
		}
		public static LegendItemGroup[] Calculate(Legend legend, List<LegendItemViewData> items, Size maxSize) {
			return CreateInstance(legend, items, maxSize).CalculateGroups();
		}
		readonly Legend legend;
		readonly IList<LegendItemViewData> items;
		readonly Size maxSize;
		readonly RectangleIndents padding;
		protected Legend Legend { get { return legend; } }
		protected IList<LegendItemViewData> Items { get { return items; } }
		protected Size MaxSize { get { return maxSize; } }
		protected RectangleIndents Padding { get { return padding; } }
		protected abstract int Indent { get; }
		public LegendItemGroupingAlgorithm(Legend legend, List<LegendItemViewData> items, Size maxSize) {
			this.legend = legend;
			if (legend.Direction == LegendDirection.BottomToTop || legend.Direction == LegendDirection.RightToLeft)
				items.Reverse();
			this.items = items;
			this.maxSize = maxSize;
			padding = legend.ActualPadding;
		}
		protected internal virtual LegendItemGroup[] CalculateGroups() {
			List<LegendItemGroup> groups = new List<LegendItemGroup>();
			for (int index = 0; index < items.Count;) {
				LegendItemGroup group = GetNextGroup(index);
				groups.Add(group);
				index += group.Count;
			}
			return groups.ToArray();
		}
		protected virtual LegendItemGroup GetNextGroup(int index) {
			int size = 0;
			int maxAvailableSize = GetCharacteristicLength(maxSize) - GetPadding();
			List<LegendItemViewData> result = new List<LegendItemViewData>();
			for (int i = index; i < Items.Count; i++) {
				size += GetCharacteristicLength(Items[i].Size);
				if (i > index && size > maxAvailableSize)
					break;
				size += Indent;
				result.Add(Items[i]);
			}
			return new LegendItemGroup(result.ToArray());
		}
		protected abstract int GetPadding();
		protected abstract int GetCharacteristicLength(Size size);
	}
	public class VerticalLegendItemGroupingAlgorithm : LegendItemGroupingAlgorithm {
		protected override int Indent { get { return Legend.VerticalIndent; } }
		public VerticalLegendItemGroupingAlgorithm(Legend legend, List<LegendItemViewData> itemData, Size maxSize) : base(legend, itemData, maxSize) {
		}
		protected override int GetPadding() {
			return Padding.Top + Padding.Bottom;
		}
		protected override int GetCharacteristicLength(Size size) {
			return size.Height;
		}
	}
	public class HorizontalLegendItemGroupingAlgorithm : LegendItemGroupingAlgorithm {
		protected override int Indent { get { return Legend.HorizontalIndent; } }
		public HorizontalLegendItemGroupingAlgorithm(Legend legend, List<LegendItemViewData> itemData, Size maxSize) : base(legend, itemData, maxSize) {
		}
		protected override int GetPadding() {
			return Padding.Left + Padding.Right;
		}
		protected override int GetCharacteristicLength(Size size) {
			return size.Width;
		}
	}
	public class HorizontalGroupingAlgorithmWithVerticalAllignment : HorizontalLegendItemGroupingAlgorithm {
		int actualDimension;
		public HorizontalGroupingAlgorithmWithVerticalAllignment(Legend legend, List<LegendItemViewData> itemData, Size maxSize) : base(legend, itemData, maxSize) {
		}
		bool IsLegendFitInLimits(LegendItemGroup[] groups) {
			if (groups.Length == 0 || groups[0].Count < 2)
				return true;
			int[] widths = new int[groups[0].Count];
			foreach (LegendItemGroup group in groups)
				for (int i = 0; i < group.Count; i++)
					widths[i] = Math.Max(widths[i], GetCharacteristicLength(group[i].Size)); 
			int length = 0;
			foreach (int currentWidth in widths)
				length += currentWidth;
			if (widths.Length > 1)
				length += (widths.Length - 1) * Legend.HorizontalIndent;
			return length <= GetCharacteristicLength(MaxSize) - GetPadding();
		}
		int CalcInitialDimension() {
			if (Items.Count <= 1)
				return 1;
			int maxAvailableSize = GetCharacteristicLength(MaxSize) - GetPadding();
			int size = GetCharacteristicLength(Items[0].Size) + Indent;
			for (int i = 1; i < Items.Count; i++) {
				size += GetCharacteristicLength(Items[i].Size);
				if (size > maxAvailableSize)
					return i;
				size += Indent;
			}
			return Items.Count;
		}
		protected internal override LegendItemGroup[] CalculateGroups() {
			actualDimension = CalcInitialDimension();
			LegendItemGroup[] groups = base.CalculateGroups();
			while (!IsLegendFitInLimits(groups)) {
				actualDimension--;
				groups = base.CalculateGroups();
			}
			return groups;
		}
		protected override LegendItemGroup GetNextGroup(int index) {
			int length = Math.Min(actualDimension, Items.Count - index);
			LegendItemViewData[] result = new LegendItemViewData[length];
			for (int i = 0; i < length; i++, index++)
				result[i] = Items[index];
			return new LegendItemGroup(result);
		}
	}
}
