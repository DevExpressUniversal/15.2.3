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
namespace DevExpress.XtraCharts.Native {
	public abstract class LegendSizeCalculator {
		static LegendSizeCalculator CreateInstance(Legend legend, LegendItemGroup[] itemGroups) {
			switch (legend.Direction) {
				case LegendDirection.TopToBottom:
				case LegendDirection.BottomToTop:
					return new VerticalLayoutLegendSizeCalculator(legend, itemGroups);
				case LegendDirection.LeftToRight:
				case LegendDirection.RightToLeft:
					return legend.EquallySpacedItems ? 
						new HorizontalLayoutWithVerticalAllignmentLegendSizeCalculator(legend, itemGroups) :
						new HorizontalLayoutLegendSizeCalculator(legend, itemGroups);
				default:
					throw new DefaultSwitchException();
			} 
		}
		public static Size Calculate(Legend legend, LegendItemGroup[] itemGroups) {
			LegendSizeCalculator calulator = CreateInstance(legend, itemGroups);
			return new Size(calulator.CalcWidth(), calulator.CalcHeight());
		}
		readonly Legend legend;
		readonly LegendItemGroup[] itemGroups;
		readonly RectangleIndents padding;
		protected Legend Legend { get { return legend; } }
		protected LegendItemGroup[] ItemGroups { get { return itemGroups; } }
		protected RectangleIndents Padding { get { return padding; } }
		protected LegendSizeCalculator(Legend legend, LegendItemGroup[] itemGroups) {
			this.legend = legend;
			this.itemGroups = itemGroups;
			padding = legend.ActualPadding;
		}
		protected abstract int CalcWidth();
		protected abstract int CalcHeight();
	}
	public class VerticalLayoutLegendSizeCalculator : LegendSizeCalculator {
		internal VerticalLayoutLegendSizeCalculator(Legend legend, LegendItemGroup[] itemGroups) : base(legend, itemGroups) {
		}
		protected override int CalcWidth() {
			int totalWidth = 0;
			foreach (LegendItemGroup group in ItemGroups) {
				int width = 0;
				foreach (LegendItemViewData itemData in group)
					width = Math.Max(width, itemData.Size.Width);
				totalWidth += width;
			}
			if (totalWidth > 0) {
				totalWidth += Padding.Left + Padding.Right;
				if (ItemGroups.Length > 1)
					totalWidth += Legend.HorizontalIndent * (ItemGroups.Length - 1);
			}
			return totalWidth;
		}
		protected override int CalcHeight() {
			int totalHeight = 0;
			foreach (LegendItemGroup group in ItemGroups) {
				int height = group.Count > 1 ? Legend.VerticalIndent * (group.Count - 1) : 0;
				foreach (LegendItemViewData itemData in group)
					height += itemData.Size.Height;
				totalHeight = Math.Max(height, totalHeight);
			}
			if (totalHeight > 0)
				totalHeight += Padding.Top + Padding.Bottom;
			return totalHeight;
		}
	}
	public class HorizontalLayoutLegendSizeCalculator : LegendSizeCalculator {
		internal HorizontalLayoutLegendSizeCalculator(Legend legend, LegendItemGroup[] itemGroups) : base(legend, itemGroups) {
		}
		protected override int CalcWidth() {
			int totalWidth = 0;
			foreach (LegendItemGroup group in ItemGroups) {
				int width = group.Count > 1 ? Legend.HorizontalIndent * (group.Count - 1) : 0;
				foreach (LegendItemViewData itemData in group)
					width += itemData.Size.Width;
				totalWidth = Math.Max(totalWidth, width);
			}
			if (totalWidth > 0)
				totalWidth += Padding.Left + Padding.Right;
			return totalWidth;
		}
		protected override int CalcHeight() {
			int totalHeight = 0;
			foreach (LegendItemGroup group in ItemGroups) {
				int height = 0;
				foreach (LegendItemViewData itemData in group)
					height = Math.Max(itemData.Size.Height, height);
				totalHeight += height;
			}
			if (totalHeight > 0) {
				totalHeight += Padding.Top + Padding.Bottom;
				if (ItemGroups.Length > 1)
					totalHeight += Legend.VerticalIndent * (ItemGroups.Length - 1);
			}
			return totalHeight;
		}
	}
	public class HorizontalLayoutWithVerticalAllignmentLegendSizeCalculator : HorizontalLayoutLegendSizeCalculator {
		internal HorizontalLayoutWithVerticalAllignmentLegendSizeCalculator(Legend legend, LegendItemGroup[] itemGroups) : base(legend, itemGroups) {
		}
		protected override int CalcWidth() {
			int totalWidth = 0;
			if (ItemGroups.Length > 0)
				for (int i = 0; i < ItemGroups[0].Count; i++) {
					totalWidth += ItemGroups[0][i].Size.Width + ItemGroups[0][i].RightIndent;
					if (i < ItemGroups[0].Count - 1)
						totalWidth += Legend.HorizontalIndent;
				}
			if (totalWidth > 0)
				totalWidth += Padding.Left + Padding.Right;
			return totalWidth;
		}
	}
}
