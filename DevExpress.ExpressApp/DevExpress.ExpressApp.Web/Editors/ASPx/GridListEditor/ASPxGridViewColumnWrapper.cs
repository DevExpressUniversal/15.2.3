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
using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxGridViewColumnWrapper : WebColumnBaseColumnWrapper {
		private GridViewDataColumn column;
		private IGridViewDataColumnInfo gridViewDataColumnInfo;
		private ASPxGridView gridView;
		public ASPxGridViewColumnWrapper(ASPxGridView gridView, GridViewDataColumn column, IGridViewDataColumnInfo gridViewDataColumnInfo)
			: base(column) {
			Guard.ArgumentNotNull(gridView, "gridView");
			Guard.ArgumentNotNull(column, "column");
			Guard.ArgumentNotNull(gridViewDataColumnInfo, "gridViewDataColumnInfo");
			this.column = column;
			this.gridViewDataColumnInfo = gridViewDataColumnInfo;
			this.gridView = gridView;
		}
		public new GridViewDataColumn Column {
			get {
				return column;
			}
		}
		public override string Id {
			get {
				return gridViewDataColumnInfo.Model.Id;
			}
		}
		public override string PropertyName {
			get {
				return gridViewDataColumnInfo.Model.PropertyName;
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
				IList<SummaryType> summary = new List<SummaryType>();
				foreach(ASPxSummaryItem summaryItem in FindTotalSummaryItems()) {
					if(summaryItem.Visible) {
						summary.Add((SummaryType)Enum.Parse(typeof(SummaryType), summaryItem.SummaryType.ToString()));
					}
				}
				return summary;
			}
			set {
				foreach(ASPxSummaryItem summaryItem in new List<ASPxSummaryItem>(FindTotalSummaryItems()))
					gridView.TotalSummary.Remove(summaryItem);
				foreach(ASPxSummaryItem summaryItem in new List<ASPxSummaryItem>(FindGroupSummaryItems()))
					gridView.GroupSummary.Remove(summaryItem);
				foreach(SummaryType summaryType in value) {
					gridView.Settings.ShowFooter = true;
					SummaryItemType summaryItemType = (SummaryItemType)Enum.Parse(typeof(SummaryItemType), summaryType.ToString());
					gridView.TotalSummary.Add(summaryItemType, column.FieldName);
					gridView.GroupSummary.Add(summaryItemType, column.FieldName);
				}
			}
		}
		public override DateTimeGroupInterval GroupInterval {
			get {
				return DateTimeGroupIntervalConverter.Convert(column.Settings.GroupInterval);
			}
			set {
				column.Settings.GroupInterval = DateTimeGroupIntervalConverter.Convert(value);
			}
		}
		public override bool AllowGroupingChange {
			get {
				return DefaultBooleanConverter.ConvertToBoolean(column.Settings.AllowGroup);
			}
			set {
				column.Settings.AllowGroup = Convert(value, column.Settings.AllowGroup);
			}
		}
		public override bool AllowSortingChange {
			get {
				return DefaultBooleanConverter.ConvertToBoolean(column.Settings.AllowSort);
			}
			set {
				column.Settings.AllowSort = Convert(value, column.Settings.AllowSort);
			}
		}
		public override bool AllowSummaryChange {
			get {
				return gridViewDataColumnInfo.AllowSummaryChange;
			}
			set {
				gridViewDataColumnInfo.AllowSummaryChange = value;
			}
		}
		public override void DisableFeaturesForProtectedContentColumn() {
			base.DisableFeaturesForProtectedContentColumn();
			column.EditFormSettings.Visible = DevExpress.Utils.DefaultBoolean.False;
			column.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.False;
			column.Settings.ShowInFilterControl = DevExpress.Utils.DefaultBoolean.False;
			column.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False;
			column.Settings.AllowAutoFilter = DevExpress.Utils.DefaultBoolean.False;
		}
		public override void ApplyModel(IModelColumn columnInfo) {
			gridViewDataColumnInfo.ApplyModel(Column);
		}
		public override void SynchronizeModel() {
			gridViewDataColumnInfo.SynchronizeModel(Column);
		}
		public override string Caption {
			get {
				return column.Caption;
			}
			set {
				column.Caption = value;
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
		public override bool AllowVisibleIndexStore {
			get {
				return true;
			}
		}
		public override bool ShowInCustomizationForm {
			get { return column.ShowInCustomizationForm; }
			set { column.ShowInCustomizationForm = value; }
		}
		private static DevExpress.Utils.DefaultBoolean Convert(bool val, DevExpress.Utils.DefaultBoolean defaultValue) {
			if(!val)
				return DevExpress.Utils.DefaultBoolean.False;
			return defaultValue;
		}
		private List<ASPxSummaryItem> FindTotalSummaryItems() {
			Guard.ArgumentNotNull(gridView, "gridView");
			List<ASPxSummaryItem> summaryItems = new List<ASPxSummaryItem>();
			foreach(ASPxSummaryItem currentItem in gridView.TotalSummary)
				if(currentItem.FieldName == column.FieldName)
					summaryItems.Add(currentItem);
			return summaryItems;
		}
		private List<ASPxSummaryItem> FindGroupSummaryItems() {
			Guard.ArgumentNotNull(gridView, "gridView");
			List<ASPxSummaryItem> summaryItems = new List<ASPxSummaryItem>();
			foreach(ASPxSummaryItem currentItem in gridView.GroupSummary)
				if(currentItem.FieldName == column.FieldName)
					summaryItems.Add(currentItem);
			return summaryItems;
		}
#if DebugTest
		public IGridViewDataColumnInfo GridViewDataColumnInfo_ForTest {
			get {
				return gridViewDataColumnInfo;
			}
		}
#endif
	}
}
