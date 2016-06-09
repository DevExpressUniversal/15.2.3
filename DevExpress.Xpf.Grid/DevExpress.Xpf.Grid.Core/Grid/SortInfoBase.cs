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
using System.Collections;
using DevExpress.Xpf.GridData;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public class GridSortInfo : DependencyObject, IColumnInfo {
		public static readonly DependencyProperty FieldNameProperty;
		public static readonly DependencyProperty SortOrderProperty;
		public static readonly DependencyProperty IsGroupedProperty;
		static readonly DependencyPropertyKey IsGroupedPropertyKey;
		public static readonly DependencyProperty GroupIndexProperty;
		public static readonly DependencyProperty SortIndexProperty;
		static GridSortInfo() {
			Type ownerType = typeof(GridSortInfo);
			FieldNameProperty = DependencyPropertyManager.Register("FieldName", typeof(string), ownerType, new PropertyMetadata(string.Empty, (d, e) => ((GridSortInfo)d).OnChanged(), OnCoerceName));
			SortOrderProperty = DependencyPropertyManager.Register("SortOrder", typeof(ListSortDirection), ownerType, new PropertyMetadata(ListSortDirection.Ascending, (d, e) => ((GridSortInfo)d).OnChanged()));
			IsGroupedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsGrouped", typeof(bool), ownerType, new PropertyMetadata(false));
			IsGroupedProperty = IsGroupedPropertyKey.DependencyProperty;
			GroupIndexProperty = DependencyPropertyManager.Register("GroupIndex", typeof(int), ownerType, new PropertyMetadata(-1));
			SortIndexProperty = DependencyPropertyManager.Register("SortIndex", typeof(int), ownerType, new PropertyMetadata(-1));
		}
		static string OnCoerceName(DependencyObject d, object baseValue) {
			return baseValue == null ? string.Empty : baseValue as string;
		}
		internal static ColumnSortOrder GetColumnSortOrder(ListSortDirection sortDirection) {
			return sortDirection == ListSortDirection.Ascending ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
		}
		internal static ListSortDirection GetSortDirectionBySortOrder(ColumnSortOrder sortOrder) {
			return sortOrder == ColumnSortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending;
		}
		internal static GridSortInfo GetSortInfoByFieldName(IList sortInfoList, string fieldName) {
			if(string.IsNullOrEmpty(fieldName))
				return null;
			foreach(GridSortInfo info in sortInfoList) {
				if(info.FieldName == fieldName)
					return info;
			}
			return null;
		}
		internal SortInfoCollectionBase Owner;
		public GridSortInfo() : this(string.Empty, ListSortDirection.Ascending) { }
		public GridSortInfo(string fieldName) : this(fieldName, ListSortDirection.Ascending) { }
		public GridSortInfo(string fieldName, ListSortDirection sortOrder) {
			FieldName = fieldName;
			SortOrder = sortOrder;
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridSortInfoIsGrouped")]
#endif
		public bool IsGrouped {
			get { return (bool)GetValue(IsGroupedProperty); }
			internal set { this.SetValue(IsGroupedPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridSortInfoGroupIndex")]
#endif
		public int GroupIndex {
			get { return (int)GetValue(GroupIndexProperty); }
			set { SetValue(GroupIndexProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridSortInfoSortIndex")]
#endif
		public int SortIndex {
			get { return (int)GetValue(SortIndexProperty); }
			set { SetValue(SortIndexProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("GridSortInfoFieldName"),
#endif
 XtraSerializableProperty]
		public string FieldName { get { return (string)GetValue(FieldNameProperty); } set { SetValue(FieldNameProperty, value); } }
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("GridSortInfoSortOrder"),
#endif
 XtraSerializableProperty]
		public ListSortDirection SortOrder { get { return (ListSortDirection)GetValue(SortOrderProperty); } set { SetValue(SortOrderProperty, value); } }
		public void ChangeSortOrder() {
			SortOrder = SortOrder == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
		}
		void OnChanged() {
			if(Owner != null)
				Owner.OnChanged();
		}
		#region IColumnInfo Members
		string IColumnInfo.FieldName { get { return FieldName; } }
		ColumnSortOrder IColumnInfo.SortOrder { get { return GetSortOrder(); } }
		UnboundColumnType IColumnInfo.UnboundType { get { return UnboundColumnType.Bound; } }
		string IColumnInfo.UnboundExpression { get { return string.Empty; } }
		bool IColumnInfo.ReadOnly { get { return false; } }
		#endregion
		internal ColumnSortOrder GetSortOrder() {
			return GetColumnSortOrder(SortOrder);
		}
		internal void SetGroupSortIndexes(int sortIndex, int groupIndex) {
			GroupIndex = groupIndex;
			SortIndex = sortIndex;
			IsGrouped = groupIndex >= 0;
		}
	}
}
