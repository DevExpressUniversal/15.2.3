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

using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.UI {
	internal static class TableHelper {
		public static void ArrangeRows(XRTable table) {
			foreach(XRTableRow row in table.Rows) {
				row.Dpi = table.Dpi;
				row.SetBoundsCore(row.RealBoundsF);
				ArrangeCells(row);
			}
		}
		public static void ArrangeCells(XRTableRow row) {
			foreach(XRTableCell cell in row.Cells) {
				cell.Dpi = row.Dpi;
				cell.SetBoundsCore(cell.RealBoundsF);
			}
		}
		public static bool IsColumnCellsArray(object[] controlList) {
			if(controlList == null) return false;
			XRTableCell standardCell = controlList[0] as XRTableCell;
			if(standardCell == null) return false;
			XRTable table = standardCell.Row.Table;
			PointF bounds = new PointF(standardCell.LeftF, standardCell.RightF);
			int controlsCount = 0;
			foreach(XRControl control in controlList) {
				XRTableCell cell = control as XRTableCell;
				if(cell == null || cell.Row.Table != table ||
					!FloatsComparer.Default.FirstEqualsSecond(cell.LeftF, bounds.X) ||
					!FloatsComparer.Default.FirstEqualsSecond(cell.RightF, bounds.Y)) return false;
				controlsCount += cell.RowSpan > 1 ? Math.Min(cell.Row.Table.Rows.Count - cell.Row.Index, cell.RowSpan) : 1;
			}
			if(controlsCount != table.Rows.Count) return false;
			return true;
		}
		public static bool IsRowCellsArray(object[] controlList) {
			 if(controlList == null || controlList.Length > 1 || controlList[0].GetType() != typeof(XRTableRow)) return false;
			 return true;
		 }
	}
	internal static class WeightHelper {
		public const double DefaultWeight = 1.0;
		public static float GetMaxAvailableWidth(IWeighty item, float oldPosition, float newPosition, float oldAmount, float newAmount, float minAmount) {
			float diffAmount = newAmount - oldAmount;
			bool positionChanged = Math.Abs(oldPosition - newPosition) > 0.0001f;
			float result;
			if(positionChanged && item.Previous != null)
				result = GetValidNeighbourAmount(item.Previous, diffAmount, minAmount);
			else if(!positionChanged && item.Next != null)
				result = GetValidNeighbourAmount(item.Next, diffAmount, minAmount);
			else result = diffAmount;
			return result + oldAmount;
		}
		public static void ResizeProportionalItem(IWeighty item, IEnumerable<IWeighty> neighbourItems, int itemsCount, float oldPosition, float newPosition, float oldAmount, float newAmount, float minAmount) {
			double minWeight = GetWeight(item.Parent, minAmount);
			double rightItemsWeight = SumUpWeight(neighbourItems);
			double additionalWeight = 0;
			float diffAmount = newAmount - oldAmount;
			double diffWeight = Math.Min(CalculateDiffWeight(item, diffAmount, minAmount), rightItemsWeight - itemsCount * minWeight);
			foreach(IWeighty neighbourItem in neighbourItems) {
				double maxAvailableDiffWeight = neighbourItem.Weight - minWeight;
				double actualDiffWeight = diffWeight * (neighbourItem.Weight / rightItemsWeight);
				if(actualDiffWeight > maxAvailableDiffWeight) additionalWeight += actualDiffWeight - maxAvailableDiffWeight;
				ResizeItem(neighbourItem, -Math.Min(maxAvailableDiffWeight, actualDiffWeight));
			}
			ResizeItem(item, diffWeight - additionalWeight);
		}
		public static void ResizeSpecifiedItem(IWeighty item, float oldPosition, float newPosition, float oldAmount, float newAmount, float minAmount) {
			float diffAmount = newAmount - oldAmount;
			ResizeItemWithoutNeighbour(item, diffAmount, minAmount);
		}
		public static void Resize(IWeighty item, float oldPosition, float newPosition, float oldAmount, float newAmount, float minAmount) {
			float diffAmount = newAmount - oldAmount;
			bool positionChanged = Math.Abs(oldPosition - newPosition) > 0.0001f;
			if(positionChanged && item.Previous != null)
				ValidateAndResizeNeighbour(item, item.Previous, diffAmount, minAmount);
			else if(!positionChanged && item.Next != null)
				ValidateAndResizeNeighbour(item, item.Next, diffAmount, minAmount);
			else {
				ValidateAmount(item, minAmount, ref diffAmount);
				if(positionChanged)
					ResizeExtendBefore(item, diffAmount);
				else
					ResizeExtendAfter(item, diffAmount);
			}
		}
		public static void UpdateWeightBySize(IEnumerable<IWeighty> items, float amount) {
			IList<IWeighty> updatableItems = GetWeightlessItems(items);
			if(updatableItems.Count == 0)
				return;
			float sumUpdatableItemsAmount = SumUpAmount(updatableItems);
			if(sumUpdatableItemsAmount < amount)
				amount -= sumUpdatableItemsAmount;
			double weight = SumUpWeight(items);
			if(weight <= 0.0)
				weight = DefaultWeight;
			double amountOfOneWeight = amount / weight;
			foreach(IWeighty item in updatableItems) {
				double itemAmount = (double)item.Amount;
				if(itemAmount <= 0.0)
					itemAmount = amountOfOneWeight;
				item.Weight = itemAmount / amountOfOneWeight;
			}
			updatableItems.Clear();
		}
		public static double SumUpWeight(IEnumerable<IWeighty> items, int count) {
			double sum = 0.0;
			int index = 0;
			foreach(IWeighty item in items) {
				if(count > -1 && (++index) > count)
					break;
				sum += item.Weight;
			}
			return sum;
		}
		public static double GetWeight(IWeightyContainer container, float amount) {
			return amount / container.AmountOfOneWeight;
		}
		public static double GetAmount(IWeightyContainer container, double weight) {
			return container.AmountOfOneWeight * weight;
		}
		public static double GetAmountOfOneWeight(IEnumerable<IWeighty> items, float amount) {
			double weights = WeightHelper.SumUpWeight(items);
			if(weights <= 0.0)
				return WeightHelper.DefaultWeight;
			return amount / weights;
		}
		static void ValidateAndResizeNeighbour(IWeighty item, IWeighty neighbour, float diffAmount, float minAmount) {
			diffAmount = GetValidNeighbourAmount(neighbour, diffAmount, minAmount);
			double diffWeight = CalculateDiffWeight(item, diffAmount, minAmount);
			ResizeItemWithNeighbour(item, neighbour, diffWeight);
		}
		static void ResizeItemWithoutNeighbour(IWeighty item, float diffAmount, float minAmount) {
			double diffWeight = CalculateDiffWeight(item, diffAmount, minAmount);
			ResizeItem(item, diffWeight);
		}
		static double CalculateDiffWeight(IWeighty item, float diffAmount, float minAmount) {
			double minWeight = GetWeight(item.Parent, minAmount);
			double diffWeight = GetWeight(item.Parent, diffAmount);
			if(item.Weight + diffWeight < minWeight)
				diffWeight = minWeight - item.Weight;
			return diffWeight;
		}
		static float GetValidNeighbourAmount(IWeighty neighbour, float diffAmount, float minAmount) {
			diffAmount = -diffAmount;
			ValidateAmount(neighbour, minAmount, ref diffAmount);
			return -diffAmount;
		}
		static void ValidateAmount(IWeighty item, float minAmount, ref float subtractAmount) {
			double minAmountDouble = (double)minAmount;
			double itemAmount = GetAmount(item.Parent, item.Weight);
			if(itemAmount + subtractAmount < minAmountDouble)
				subtractAmount = (int)Math.Round(minAmountDouble - itemAmount);
		}
		static void ResizeItemWithNeighbour(IWeighty item, IWeighty neighbour, double diffWeight) {
			ResizeItem(item, diffWeight);
			neighbour.Weight -= diffWeight;
		}
		static void ResizeItem(IWeighty item, double diffWeight) {
			item.Weight += diffWeight;
		}
		static void ResizeExtendBefore(IWeighty item, float diffAmount) {
			float initDiffAmount = diffAmount;
			item.Parent.AddAmountToChildren(Side.Before, ref diffAmount);
			item.Parent.PositionAndExtendingAmount = new PointF(item.Parent.Position - initDiffAmount, item.Parent.ExtendingAmount + diffAmount);
		}
		static void ResizeExtendAfter(IWeighty item, float diffAmount) {
			item.Parent.AddAmountToChildren(Side.After, ref diffAmount);
			item.Parent.ExtendingAmount += diffAmount;
		}
		static IList<IWeighty> GetWeightlessItems(IEnumerable<IWeighty> items) {
			List<IWeighty> result = new List<IWeighty>();
			foreach(IWeighty item in items)
				if(item.Weight == 0.0)
					result.Add(item);
			return result;
		}
		static float SumUpAmount(IEnumerable<IWeighty> items) {
			float result = 0;
			foreach(IWeighty item in items)
				result += item.Amount;
			return result;
		}
		static double SumUpWeight(IEnumerable<IWeighty> items) {
			return SumUpWeight(items, -1);
		}
	}
	internal interface IWeighty {
		double Weight { get; set; }
		float Amount { get; }
		IWeightyContainer Parent { get; }
		IWeighty Previous { get; }
		IWeighty Next { get; }
	}
	internal interface IWeightyContainer {
		double AmountOfOneWeight { get; }
		float ExtendingAmount { get; set; }
		float Position { get; set; }
		PointF PositionAndExtendingAmount { set; }
		void AddAmountToChildren(Side side, ref float amount);
	}
	internal enum Side : byte {
		Before,
		After,
	}
}
