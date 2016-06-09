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

using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotStyleFormatRelativeColumnRowStripeInfoCacheBase (abstract class)
	public abstract class PivotStyleFormatRelativeColumnRowStripeInfoCacheBase : TableStyleFormatRelativeColumnStripeInfoCacheBase<IPivotStyleOwner> {
		protected PivotStyleFormatRelativeColumnRowStripeInfoCacheBase(IPivotStyleOwner owner, int firstAbsoluteIndex, int lastAbsoluteIndex)
			: base(owner, firstAbsoluteIndex, lastAbsoluteIndex) {
		}
		#region ITableStyleFormatStripeInfoCache
		public override void RegisterStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo) {
			int itemIndex = GetItemIndex(absoluteDataIndex);
			SetStripeInfo(itemIndex, stripeInfo);
		}
		public override void ReplaceStripeInfo(int absoluteDataIndex, TableStyleStripeInfo stripeInfo) {
			RegisterStripeInfo(absoluteDataIndex, stripeInfo);
		}
		public override TableStyleStripeInfo GetStripeInfo(int absoluteDataIndex) {
			int itemIndex = GetItemIndex(absoluteDataIndex);
			return GetStripeInfoCore(itemIndex);
		}
		#endregion
		protected abstract int GetItemIndex(int absoluteDataIndex);
		protected abstract void SetStripeInfo(int itemIndex, TableStyleStripeInfo stripeInfo);
		protected abstract TableStyleStripeInfo GetStripeInfoCore(int itemIndex);
	}
	#endregion
	#region PivotStyleFormatRelativeColumnStripeInfoCache
	public class PivotStyleFormatRelativeColumnStripeInfoCache : PivotStyleFormatRelativeColumnRowStripeInfoCacheBase {
		public PivotStyleFormatRelativeColumnStripeInfoCache(IPivotStyleOwner owner, int firstAbsoluteIndex, int lastAbsoluteIndex)
			: base(owner, firstAbsoluteIndex, lastAbsoluteIndex) {
		}
		#region PivotStyleFormatRelativeColumnRowStripeInfoCacheBase Members
		protected override int GetItemIndex(int absoluteDataIndex) {
			return Owner.GetDataColumnItemIndex(absoluteDataIndex); 
		}
		protected override void SetStripeInfo(int itemIndex, TableStyleStripeInfo stripeInfo) {
			Owner.CacheColumnItemStripeInfo(itemIndex, stripeInfo);
		}
		protected override TableStyleStripeInfo GetStripeInfoCore(int itemIndex) {
			return Owner.GetColumnItemStripeInfo(itemIndex);
		}
		#endregion
	}
	#endregion
	#region PivotStyleFormatRelativeRowStripeInfoCache
	public class PivotStyleFormatRelativeRowStripeInfoCache : PivotStyleFormatRelativeColumnRowStripeInfoCacheBase {
		public PivotStyleFormatRelativeRowStripeInfoCache(IPivotStyleOwner owner, int firstAbsoluteIndex, int lastAbsoluteIndex)
			: base(owner, firstAbsoluteIndex, lastAbsoluteIndex) {
		}
		#region PivotStyleFormatRelativeColumnRowStripeInfoCacheBase<PivotTableRowItems> Members
		protected override int GetItemIndex(int absoluteDataIndex) {
			return Owner.GetDataRowItemIndex(absoluteDataIndex);
		}
		protected override void SetStripeInfo(int itemIndex, TableStyleStripeInfo stripeInfo) {
			Owner.CacheRowItemStripeInfo(itemIndex, stripeInfo);
		}
		protected override TableStyleStripeInfo GetStripeInfoCore(int itemIndex) {
			return Owner.GetRowItemStripeInfo(itemIndex);
		}
		#endregion
	}
	#endregion
} 
