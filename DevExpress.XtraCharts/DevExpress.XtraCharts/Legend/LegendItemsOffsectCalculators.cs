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
using System.Collections.Generic;
namespace DevExpress.XtraCharts.Native {
	public abstract class LegendItemsOffsetCalculator {
		static LegendItemsOffsetCalculator CreateInstance(Legend legend, IList<LegendItemGroup> itemGroups) {
			switch (legend.Direction) {
				case LegendDirection.TopToBottom:
				case LegendDirection.BottomToTop:
					return new VerticalLayoutLegendItemsOffsetCalculator(legend, itemGroups);
				case LegendDirection.LeftToRight:
				case LegendDirection.RightToLeft:
					return legend.EquallySpacedItems ?
						new HorizontalLayoutWithVerticalAlignningOffsetCalculator(legend, itemGroups) :
						new HorizontalLayoutLegendItemsOffsetCalculator(legend, itemGroups);
				default:
					throw new DefaultSwitchException();
			} 
		}
		public static void Calculate(Legend legend, IList<LegendItemGroup> itemGroups) {
			if (itemGroups.Count == 0)
				return;
			LegendItemsOffsetCalculator calculator = CreateInstance(legend, itemGroups);
			calculator.CalculateRightIndents();
			calculator.CalculateOffsets();
		}
		Legend legend;
		IList<LegendItemGroup> itemGroups;
		protected Legend Legend { get { return legend; } }
		protected IList<LegendItemGroup> ItemGroups { get { return itemGroups; } }
		public LegendItemsOffsetCalculator(Legend legend, IList<LegendItemGroup> itemGroups) {
			this.legend = legend;
			this.itemGroups = itemGroups;
		}
		int GetMaxCharacteristicLength(LegendItemGroup group) {
			int length = 0;
			foreach (LegendItemViewData item in group)
				length = Math.Max(length, GetCharacteristicLength(item.Size));
			return length;
		}
		protected abstract int GetCharacteristicLength(Size size);
		protected abstract void CalculateOffsets();
		protected virtual void CalculateRightIndents() {
			foreach (LegendItemGroup group in ItemGroups) {
				int maxLength = GetMaxCharacteristicLength(group);
				foreach (LegendItemViewData item in group)
					item.RightIndent = maxLength - GetCharacteristicLength(item.Size);
			}
		}
	}
	public class VerticalLayoutLegendItemsOffsetCalculator : LegendItemsOffsetCalculator {
		public VerticalLayoutLegendItemsOffsetCalculator(Legend legend, IList<LegendItemGroup> itemGroups) : base(legend, itemGroups) {
		}
		protected override int GetCharacteristicLength(Size size) {
			return size.Width;
		}
		protected override void CalculateOffsets() {
			int x = 0;
			for (int i = 0; i < ItemGroups.Count; i++) {
				int y = 0;
				for (int j = 0; j < ItemGroups[i].Count; j++) {
					ItemGroups[i][j].Offset = new Point(x, y);
					y += ItemGroups[i][j].Size.Height + Legend.VerticalIndent;
				}
				x += ItemGroups[i][0].Size.Width + ItemGroups[i][0].RightIndent + Legend.HorizontalIndent;
			}
		}
	}
	public class HorizontalLayoutLegendItemsOffsetCalculator : LegendItemsOffsetCalculator {
		public HorizontalLayoutLegendItemsOffsetCalculator(Legend legend, IList<LegendItemGroup> itemGroups) : base(legend, itemGroups) {
		}
		protected override int GetCharacteristicLength(Size size) {
			return size.Height;
		}
		protected override void CalculateOffsets() {
			int y = 0;
			for (int i = 0; i < ItemGroups.Count; i++) {
				int x = 0;
				for (int j = 0; j < ItemGroups[i].Count; j++) {
					ItemGroups[i][j].Offset = new Point(x, y);
					x += ItemGroups[i][j].Size.Width + ItemGroups[i][j].RightIndent + Legend.HorizontalIndent;
				}
				y += ItemGroups[i][0].Size.Height + Legend.VerticalIndent;
			}
		}
	}
	public class HorizontalLayoutWithVerticalAlignningOffsetCalculator : HorizontalLayoutLegendItemsOffsetCalculator {
		public HorizontalLayoutWithVerticalAlignningOffsetCalculator(Legend legend, IList<LegendItemGroup> itemGroups) : base(legend, itemGroups) {
		}
		int[] CalculateWidths() {
			int[] widths = new int[ItemGroups[0].Count];
			foreach (LegendItemGroup group in ItemGroups)
				for (int i = 0; i < group.Count; i++)
					widths[i] = Math.Max(widths[i], group[i].Size.Width);
			return widths;
		}
		protected override void CalculateRightIndents() {
			int[] widths = CalculateWidths();
			foreach (LegendItemGroup group in ItemGroups)
				for (int i = 0; i < group.Count; i++)
					group[i].RightIndent = widths[i] - group[i].Size.Width;
		}
	}
}
