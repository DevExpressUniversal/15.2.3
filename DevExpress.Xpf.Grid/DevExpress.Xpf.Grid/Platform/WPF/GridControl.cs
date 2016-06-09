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
using DevExpress.Data;
using DevExpress.Xpf.Data;
using System.Windows.Markup;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Data.Helpers;
using System.Linq;
using System.ComponentModel;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Grid {
	[SupportDXTheme]
	public partial class GridControl : IAddChild {
		#region IAddChild Members
#if !SL
		void IAddChild.AddChild(object value) { }
		void IAddChild.AddText(string text) { }
#endif
		#endregion
		#region conditional formatting
		static void ProcessServiceSummary(GridControl grid, CustomSummaryEventArgs e, ServiceSummaryItem serviceSummaryItem) {
			if(serviceSummaryItem.CustomServiceSummaryItemType.Value == CustomServiceSummaryItemType.SortedList)
				CustomSortedListSummary(grid, serviceSummaryItem, e);
			if(serviceSummaryItem.CustomServiceSummaryItemType.Value == CustomServiceSummaryItemType.DateTimeAverage)
				CustomDateTimeAverageSummary(grid, serviceSummaryItem, e);
		}
		static void CustomSortedListSummary(GridControl grid, ServiceSummaryItem serviceSummaryItem, CustomSummaryEventArgs e) {
			if(e.SummaryProcess == CustomSummaryProcess.Start) {
				DataColumnInfo column = grid.DataController.Columns[serviceSummaryItem.FieldName];
				if(column != null && (typeof(IComparable).IsAssignableFrom(column.GetDataType()) || column.UnboundWithExpression)) {
					int[] sortedListIndices = GetSortedListIndices(grid, column);
					sortedListIndices = FilterSortedListIndicesForNullValues(sortedListIndices, column, (GridDataProvider)grid.DataProviderBase);
					e.TotalValue = new SortedIndices(sortedListIndices);
				}
				e.TotalValueReady = true;
			}
		}
		static int[] GetSortedListIndices(GridControl grid, DataColumnInfo column) {
			var existingCollection = grid.DataController.GroupInfo.VisibleListSourceRows;
			var newCollection = existingCollection.CloneThatWouldBeForSureModifiedAndOrForgottenBeforeAnythingHappensToOriginal();
			newCollection.SortRows(new[] { new DataColumnSortInfo(column) });
			return newCollection.ToArray();
		}
		static int[] FilterSortedListIndicesForNullValues(int[] sortedListIndices, DataColumnInfo column, GridDataProvider provider) {
			if(sortedListIndices.Length == 0)
				return sortedListIndices;
			Type columnType = column.Type;
			if(columnType.IsValueType && Nullable.GetUnderlyingType(columnType) == null && !column.UnboundWithExpression)
				return sortedListIndices;
			object minimumRowValue = provider.GetRowValueByListIndex(sortedListIndices[0], column);
			if(minimumRowValue == null) {
				int firstNonNullIndex = -1;
				for(int i = 1; i < sortedListIndices.Length; i++) {
					if(provider.GetRowValueByListIndex(sortedListIndices[i], column) != null) {
						firstNonNullIndex = i;
						break;
					}
				}
				if(firstNonNullIndex == -1)
					return new int[0];
				int nonNullElementCount = sortedListIndices.Length - firstNonNullIndex;
				var filteredIndices = new int[nonNullElementCount];
				Array.Copy(sortedListIndices, firstNonNullIndex, filteredIndices, 0, nonNullElementCount);
				return filteredIndices;
			} else {
				if(provider.ValueComparer.Compare(minimumRowValue, null) <= 0)
					return sortedListIndices.Where(x => provider.GetRowValueByListIndex(x, column) != null).ToArray();
			}
			return sortedListIndices;
		}
		static void CustomDateTimeAverageSummary(GridControl grid, ServiceSummaryItem serviceSummaryItem, CustomSummaryEventArgs e) {
			if(e.SummaryProcess == CustomSummaryProcess.Start) {
				e.TotalValue = new Tuple<decimal, int>((decimal)0, 0);
			}
			if(e.SummaryProcess == CustomSummaryProcess.Calculate) {
				var current = (Tuple<decimal, int>)e.TotalValue;
				e.TotalValue = new Tuple<decimal, int>(current.Item1 + (decimal)((DateTime)e.FieldValue).Ticks, current.Item2 + 1);
			}
			if(e.SummaryProcess == CustomSummaryProcess.Finalize) {
				var total = (Tuple<decimal, int>)e.TotalValue;
				e.TotalValue = total.Item1 / total.Item2;
			}
		}
		#endregion
	}
	class GridControlSerializationProvider : SerializationProvider {
		protected override object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			object item = base.OnCreateCollectionItem(e);
			if(item == null && e.Source is IXtraSupportDeserializeCollectionItem)
				item = ((IXtraSupportDeserializeCollectionItem)e.Source).CreateCollectionItem(e.CollectionName, new XtraItemEventArgs(e.Owner, e.Collection, e.Item));
			return item;
		}
	}
}
