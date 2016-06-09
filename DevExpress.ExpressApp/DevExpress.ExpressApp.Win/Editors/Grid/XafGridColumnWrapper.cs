#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
namespace DevExpress.ExpressApp.Win.Editors {
	public class XafGridColumnWrapper : ColumnWrapper {
		private const int defaultColumnWidth = 75;
		static DefaultBoolean Convert(bool val, DefaultBoolean defaultValue) {
			if(!val)
				return DefaultBoolean.False;
			return defaultValue;
		}
		static bool Convert(DefaultBoolean val) {
			if(val == DefaultBoolean.False)
				return false;
			return true;
		}
		private GridColumn column;
		private IGridColumnModelSynchronizer gridColumnInfo;
		protected virtual GridColumnSummaryItem CreateGridColumnSummaryItem() {
			return new GridColumnSummaryItem();
		}
		public XafGridColumnWrapper(GridColumn column, IGridColumnModelSynchronizer gridColumnInfo) {
			this.column = column;
			this.gridColumnInfo = gridColumnInfo;
		}
		public GridColumn Column {
			get {
				return column;
			}
		}
		public IGridColumnModelSynchronizer GridColumnInfo {
			get { return gridColumnInfo; }
		}
		public override string Id {
			get {
				return gridColumnInfo != null ? gridColumnInfo.Model.Id : string.Empty;
			}
		}
		public override string PropertyName {
			get {
				return gridColumnInfo != null ? gridColumnInfo.PropertyName : string.Empty;
			}
		}
		public override int SortIndex {
			get {
				return column.SortIndex;
			}
			set {
				column.SortIndex = value;
			}
		}
		public override ColumnSortOrder SortOrder {
			get {
				return column.SortOrder;
			}
			set {
				column.SortOrder = value;
			}
		}
		public override IList<SummaryType> Summary {
			get {
				IList<SummaryType> list = new List<SummaryType>();
				foreach(GridColumnSummaryItem summaryItem in column.Summary)
					list.Add((SummaryType)Enum.Parse(typeof(SummaryType), summaryItem.SummaryType.ToString()));
				return list;
			}
			set {
				column.Summary.Clear();
				if(value != null)
					foreach(SummaryType summaryType in value) {
						GridColumnSummaryItem summaryItem = CreateGridColumnSummaryItem();
						summaryItem.SummaryType = (SummaryItemType)Enum.Parse(typeof(SummaryItemType), summaryType.ToString());
						column.Summary.Add(summaryItem);
						summaryItem.DisplayFormat = summaryItem.GetDefaultDisplayFormat();
					}
			}
		}
		public override string SummaryFormat {
			get {
				return column.SummaryItem.DisplayFormat;
			}
			set {
				column.SummaryItem.DisplayFormat = value;
			}
		}
		public override int GroupIndex {
			get {
				return column.GroupIndex;
			}
			set {
				column.GroupIndex = value;
			}
		}
		public override DateTimeGroupInterval GroupInterval {
			get {
				return DateTimeGroupIntervalConverter.Convert(column.GroupInterval);
			}
			set {
				column.GroupInterval = DateTimeGroupIntervalConverter.Convert(value);
			}
		}
		public override bool AllowGroupingChange {
			get {
				return Convert(column.OptionsColumn.AllowGroup);
			}
			set {
				column.OptionsColumn.AllowGroup = Convert(value, column.OptionsColumn.AllowGroup);
			}
		}
		public override bool AllowSortingChange {
			get {
				return Convert(column.OptionsColumn.AllowSort);
			}
			set {
				column.OptionsColumn.AllowSort = Convert(value, column.OptionsColumn.AllowSort);
			}
		}
		public override bool AllowSummaryChange {
			get {
				return gridColumnInfo != null ? gridColumnInfo.AllowSummaryChange : false;
			}
			set {
				if(gridColumnInfo != null) {
					gridColumnInfo.AllowSummaryChange = value;
				}
			}
		}
		public override bool Visible {
			get {
				return column.Visible;
			}
		}
		public override int VisibleIndex {
			get {
				return column.VisibleIndex;
			}
			set {
				column.VisibleIndex = value;
			}
		}
		public override string Caption {
			get {
				return column.Caption;
			}
			set {
				column.Caption = value;
				if(string.IsNullOrEmpty(column.Caption))
					column.Caption = column.FieldName;
			}
		}
		public override string ToolTip {
			get {
				return column.ToolTip;
			}
			set {
				column.ToolTip = value;
			}
		}
		public override string DisplayFormat {
			get {
				return column.DisplayFormat.FormatString;
			}
			set {
				column.DisplayFormat.FormatString = value;
				column.DisplayFormat.FormatType = FormatType.Custom;
				column.GroupFormat.FormatString = value;
				column.GroupFormat.FormatType = FormatType.Custom;
				RepositoryItem repositoryItem = column.ColumnEdit;
				if(repositoryItem != null) {
					if(!column.DisplayFormat.IsEquals(repositoryItem.DisplayFormat)) {
						column.DisplayFormat.FormatType = repositoryItem.DisplayFormat.FormatType;
						column.DisplayFormat.Format = repositoryItem.DisplayFormat.Format;
						column.DisplayFormat.FormatString = repositoryItem.DisplayFormat.FormatString;
					}
				}
			}
		}
		public override int Width {
			get {
				if(column.Width == defaultColumnWidth)
					return 0;
				return column.Width;
			}
			set {
				if(value == 0)
					return;
				column.Width = value;
			}
		}
		public override bool ShowInCustomizationForm {
			get { return column.OptionsColumn.ShowInCustomizationForm; }
			set { column.OptionsColumn.ShowInCustomizationForm = value; }
		}
		public override void DisableFeaturesForProtectedContentColumn() {
			base.DisableFeaturesForProtectedContentColumn();
			column.OptionsFilter.AllowFilter = false;
			column.OptionsFilter.AllowAutoFilter = false;
			column.OptionsColumn.AllowIncrementalSearch = false;
			column.SortMode = ColumnSortMode.DisplayText;
		}
		public override void ApplyModel(IModelColumn columnInfo) {
			base.ApplyModel(columnInfo);
			if(gridColumnInfo != null) {
				gridColumnInfo.ApplyModel(column);
			}
		}
		public override void SynchronizeModel() {
			base.SynchronizeModel();
			if(gridColumnInfo != null) {
				gridColumnInfo.SynchronizeModel(column);
			}
		}
	}
}
