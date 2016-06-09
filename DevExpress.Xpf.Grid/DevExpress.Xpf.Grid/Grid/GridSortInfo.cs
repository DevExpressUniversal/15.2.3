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
using System.ComponentModel;
using DevExpress.Xpf.Data;
using DevExpress.Data;
using System.Windows;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public class GridGroupSummarySortInfo : IDetailElement<GridGroupSummarySortInfo> {
		protected GridGroupSummarySortInfo() { }
		public GridGroupSummarySortInfo(GridSummaryItem summaryItem, string fieldName) 
			: this(summaryItem, fieldName, ListSortDirection.Ascending) { }
		public GridGroupSummarySortInfo(GridSummaryItem summaryItem, string fieldName, ListSortDirection sortOrder) {
			FieldName = fieldName;
			SummaryItem = summaryItem;
			SortOrder = sortOrder; 
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridGroupSummarySortInfoFieldName"),
#endif
 XtraSerializableProperty]
		public string FieldName { get; internal set; }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridGroupSummarySortInfoSortOrder"),
#endif
 XtraSerializableProperty]
		public ListSortDirection SortOrder { get; internal set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridGroupSummarySortInfoSummaryItem")]
#endif
		public GridSummaryItem SummaryItem { get; private set;  }
		[XtraSerializableProperty, EditorBrowsable(EditorBrowsableState.Never)]
		public int SummaryItemIndex { 
			get { return GroupSummary.IndexOf(SummaryItem); }
			internal set { 
				bool isValidIndex = 0 <= value && value < GroupSummary.Count;
#if DEBUGTEST
				System.Diagnostics.Debug.Assert(isValidIndex);
#endif
				if(isValidIndex)
					SummaryItem = GroupSummary[value]; 
			}
		}
		internal GridGroupSummarySortInfoCollection Owner;
		GridSummaryItemCollection GroupSummary { get { return Owner.Owner.GroupSummary; } }
		internal ColumnSortOrder GetSortOrder() {
			return SortOrder == ListSortDirection.Ascending ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
		}
		#region IDetailElement<GridGroupSummarySortInfo> Members
		GridGroupSummarySortInfo IDetailElement<GridGroupSummarySortInfo>.CreateNewInstance(params object[] args) {
			GridControl targetControl = (GridControl)args[0];
			GridControl originationControl = (GridControl)((GridDataProvider)Owner.Owner).Owner;
			return new GridGroupSummarySortInfo(CloneDetailHelper.SafeGetDependentCollectionItem<GridSummaryItem>(SummaryItem, originationControl.GroupSummary, targetControl.GroupSummary), FieldName, SortOrder);
		}
		#endregion
	}
}
namespace DevExpress.Xpf.Grid.Native {
	public class GridGroupSummarySortInfoDeserializable : GridGroupSummarySortInfo {
		public GridGroupSummarySortInfoDeserializable() { }
		[XtraSerializableProperty]
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		[XtraSerializableProperty]
		public new ListSortDirection SortOrder { get { return base.SortOrder; } set { base.SortOrder = value; } }
		[XtraSerializableProperty]
		public new int SummaryItemIndex { get { return base.SummaryItemIndex; } set { base.SummaryItemIndex = value; } }
	}
}
