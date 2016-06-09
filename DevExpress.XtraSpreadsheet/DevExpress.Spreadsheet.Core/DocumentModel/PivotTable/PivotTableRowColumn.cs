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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region IPivotLayoutItem
	public interface IPivotLayoutItem {
		PivotFieldItemType Type { get; }
		int DataFieldIndex { get; }
		int RepeatedItemsCount { get; }
		int[] PivotFieldItemIndices { get; }
		TableStyleStripeInfo CachedStripeInfo { get; set; }
	}
	#endregion
	#region PivotLayoutItemFactory
	public static class PivotLayoutItemFactory {
		public static IPivotLayoutItem CreateInstance(int[] pivotFieldItemIndices, int repeatedItemsCount, int dataFieldIndex, PivotFieldItemType itemType) {
			switch (itemType) {
				case PivotFieldItemType.Data:
					return new PivotLayoutDataItem(pivotFieldItemIndices, repeatedItemsCount, dataFieldIndex);
				case PivotFieldItemType.Blank:
					return new PivotLayoutEmptyItem(pivotFieldItemIndices, repeatedItemsCount);
				default:
					return new PivotLayoutSubtotalItem(pivotFieldItemIndices, repeatedItemsCount, dataFieldIndex, itemType);
			}
		}
	}
	#endregion
	#region PivotLayoutEmptyItem
	public class PivotLayoutEmptyItem : IPivotLayoutItem {
		readonly int[] pivotFieldItemIndices;
		readonly int repeatedItemsCount;
		TableStyleStripeInfo cachedStripeInfo;
		public PivotLayoutEmptyItem(int[] pivotFieldItemIndices, int repeatedItemsCount) {
			this.pivotFieldItemIndices = pivotFieldItemIndices;
			this.repeatedItemsCount = repeatedItemsCount;
		}
		#region Properties
		public virtual PivotFieldItemType Type { get { return PivotFieldItemType.Blank; } }
		public virtual int DataFieldIndex { get { return 0; } }
		public int RepeatedItemsCount { get { return repeatedItemsCount; } }
		public int[] PivotFieldItemIndices { get { return pivotFieldItemIndices; } }
		public TableStyleStripeInfo CachedStripeInfo { get { return cachedStripeInfo; } set { cachedStripeInfo = value; } }
		#endregion
	}
	#endregion
	#region PivotLayoutDataItem
	public class PivotLayoutDataItem : PivotLayoutEmptyItem {
		readonly int dataFieldIndex;
		public PivotLayoutDataItem(int[] pivotFieldItemIndices, int repeatedItemsCount, int dataFieldIndex)
			: base(pivotFieldItemIndices, repeatedItemsCount) {
			this.dataFieldIndex = dataFieldIndex;
		}
		public override PivotFieldItemType Type { get { return PivotFieldItemType.Data; } }
		public override int DataFieldIndex { get { return dataFieldIndex; } }
	}
	#endregion
	#region PivotReportSubtotalsRow
	public class PivotLayoutSubtotalItem : PivotLayoutDataItem {
		readonly PivotFieldItemType itemType;
		public PivotLayoutSubtotalItem(int[] pivotFieldItemIndices, int repeatedItemsCount, int dataFieldIndex, PivotFieldItemType itemType)
			: base(pivotFieldItemIndices, repeatedItemsCount, dataFieldIndex) {
			this.itemType = itemType;
		}
		public override PivotFieldItemType Type { get { return itemType; } }
	}
	#endregion
	public class PivotLayoutItems : SimpleCollection<IPivotLayoutItem> { }
}
