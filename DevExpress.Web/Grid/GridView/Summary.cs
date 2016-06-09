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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Data.IO;
using DevExpress.Data.Summary;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class ASPxSummaryItem : ASPxSummaryItemBase {
		public ASPxSummaryItem() : base() { }
		public ASPxSummaryItem(string fieldName, SummaryItemType summaryType) : base(fieldName, summaryType) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSummaryItemSummaryType"),
#endif
 DefaultValue(SummaryItemType.None), NotifyParentProperty(true)]
		public new SummaryItemType SummaryType { get { return base.SummaryType; } set { base.SummaryType = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSummaryItemFieldName"),
#endif
 DefaultValue(""), Localizable(false), TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), NotifyParentProperty(true)]
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSummaryItemDisplayFormat"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string DisplayFormat { get { return base.DisplayFormat; } set { base.DisplayFormat = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSummaryItemValueDisplayFormat"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ValueDisplayFormat { get { return base.ValueDisplayFormat; } set { base.ValueDisplayFormat = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSummaryItemVisible"),
#endif
 DefaultValue(true), Localizable(false), NotifyParentProperty(true)]
		public new bool Visible { get { return base.Visible; } set { base.Visible = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSummaryItemTag"),
#endif
 DefaultValue(""), Localizable(false), NotifyParentProperty(true)]
		public new string Tag { get { return base.Tag; } set { base.Tag = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSummaryItemShowInColumn"),
#endif
 DefaultValue(""), Localizable(false), TypeConverter("DevExpress.Web.Design.GridViewColumnsConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), NotifyParentProperty(true)]
		public virtual string ShowInColumn {
			get { return GetStringProperty("ShowInColumn", string.Empty); }
			set {
				if(value == null)
					value = string.Empty;
				SetStringProperty("ShowInColumn", string.Empty, value);
				OnSummaryChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSummaryItemShowInGroupFooterColumn"),
#endif
 DefaultValue(""), Localizable(false), TypeConverter("DevExpress.Web.Design.GridViewColumnsConverter, " + AssemblyInfo.SRAssemblyWebDesignFull),  NotifyParentProperty(true)]
		public virtual string ShowInGroupFooterColumn {
			get { return GetStringProperty("ShowInGroupFooterColumn", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetStringProperty("ShowInGroupFooterColumn", string.Empty, value);
				OnSummaryChanged();
			}
		}
		public string GetGroupRowDisplayText(GridViewColumn column, object value) {
			return GenerateDisplayText(GetDefaultDisplayFormatForSameColumn(), value, column);
		}
		public string GetTotalFooterDisplayText(GridViewColumn column, object value) {
			return GenerateDisplayText(GetDefaultDisplayFormat(ShowInColumn), value, column);
		}
		public string GetGroupFooterDisplayText(GridViewColumn column, object value) {
			return GenerateDisplayText(GetDefaultDisplayFormat(ShowInGroupFooterColumn), value, column);
		}
		protected string GetDefaultDisplayFormat(string columnName) {
			return string.IsNullOrEmpty(columnName) || FieldName == columnName ? GetDefaultDisplayFormatForSameColumn() : GetDefaultDisplayFormatForOtherColumn();
		}
		protected internal bool IsShowInGroupRow { get { return string.IsNullOrEmpty(ShowInGroupFooterColumn); } }
		protected internal bool CreatedByClient { get { return GetBoolProperty("CreatedByClient", false); } set { SetBoolProperty("CreatedByClient", false, value); } }
		protected override void OnSummaryChangedCore() {
			var summaryCollection = Collection as ASPxSummaryItemCollection;
			if(summaryCollection != null)
				summaryCollection.OnSummaryChanged(this);
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var item = source as ASPxSummaryItem;
			if(item != null) {
				ShowInColumn = item.ShowInColumn;
				ShowInGroupFooterColumn = item.ShowInGroupFooterColumn;
			}
		}
		protected internal override void Load(TypedBinaryReader reader) {
			base.Load(reader);
			ShowInColumn = reader.ReadObject<string>();
			ShowInGroupFooterColumn = reader.ReadObject<string>();
			CreatedByClient = reader.ReadObject<bool>();
		}
		protected internal override void Save(TypedBinaryWriter writer) {
			base.Save(writer);
			writer.WriteObject(ShowInColumn);
			writer.WriteObject(ShowInGroupFooterColumn);
			writer.WriteObject(CreatedByClient);
		}
	}
	public class ASPxSummaryItemCollection : ASPxGridSummaryItemCollectionBase<ASPxSummaryItem> {
		public ASPxSummaryItemCollection(IWebControlObject webControlObject)
			: base(webControlObject) {}
		[Browsable(false)]
		public new ASPxSummaryItem this[string fieldName] { get { return base[fieldName]; } }
		[Browsable(false)]
		public new ASPxSummaryItem this[string fieldName, SummaryItemType summaryType] { get { return base[fieldName, summaryType]; } }
		protected internal List<ASPxSummaryItem> GetGroupRowItems() {
			return GetActiveItems().Where(i => i.IsShowInGroupRow).ToList();
		}
		public new ASPxSummaryItem Add(SummaryItemType summaryType, string fieldName) {
			return base.Add(summaryType, fieldName);
		}
	}
	public class ASPxGroupSummarySortInfo : CollectionItem {
		ASPxSummaryItem summaryItem;
		ColumnSortOrder sortOrder = ColumnSortOrder.None;
		string groupColumn = string.Empty;
		public ASPxGroupSummarySortInfo() {}		
		public ASPxGroupSummarySortInfo(string groupColumn, ASPxSummaryItem groupSummary) : this(groupColumn, groupSummary, ColumnSortOrder.Ascending) {}
		public ASPxGroupSummarySortInfo(string groupColumn, ASPxSummaryItem groupSummary, ColumnSortOrder sortOrder) {
			this.groupColumn = groupColumn;
			this.summaryItem = groupSummary;
			this.sortOrder = sortOrder;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGroupSummarySortInfoGroupColumn"),
#endif
		DefaultValue("")]
		public string GroupColumn {
			get { return groupColumn; }
			set {
				if(value == null) value = string.Empty;
				if(value == GroupColumn) return;
				groupColumn = value;
				OnSummaryChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGroupSummarySortInfoSummaryItem"),
#endif
		DefaultValue(null)]
		public ASPxSummaryItem SummaryItem {
			get { return summaryItem; }
			set {
				if(SummaryItem == value) return;
				summaryItem = value;
				OnSummaryChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGroupSummarySortInfoSortOrder"),
#endif
		DefaultValue(ColumnSortOrder.None)]
		public ColumnSortOrder SortOrder {
			get { return sortOrder; }
			set {
				if(SortOrder == value) return;
				sortOrder = value;
				OnSummaryChanged();
			}
		}
		public void Remove() {
			if(Collection == null) return;
			((ASPxGroupSummarySortInfoCollection)Collection).Remove(this);
		}
		protected void OnSummaryChanged() {
			if(IsLoading() || Collection == null) return;
			((ASPxGroupSummarySortInfoCollection)Collection).OnSummaryChanged(this);
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var sortInfo = source as ASPxGroupSummarySortInfo;
			if(sortInfo != null) {
				GroupColumn = sortInfo.GroupColumn;
				SortOrder = sortInfo.SortOrder;
			}
		}
	}
	public class ASPxGroupSummarySortInfoCollection : DevExpress.Web.Collection<ASPxGroupSummarySortInfo> {
#if !SL
	[DevExpressWebLocalizedDescription("ASPxGroupSummarySortInfoCollectionSummaryChanged")]
#endif
		public event CollectionChangeEventHandler SummaryChanged;
		public ASPxGroupSummarySortInfoCollection(IWebControlObject webControlObject)
			: base(webControlObject) {
		}
		public override string ToString() {
			return string.Empty;
		}
		protected internal void OnSummaryChanged(ASPxGroupSummarySortInfo item) {
			if(IsLoading || Owner == null) return;
			if(SummaryChanged != null) SummaryChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, item));
		}
		protected override void OnChanged() {
			base.OnChanged();
			OnSummaryChanged(null);
		}
		public void ClearAndAddRange(params ASPxGroupSummarySortInfo[] sortInfos) {
			BeginUpdate();
			try {
				Clear();
				AddRange(sortInfos);
			}
			finally {
				EndUpdate();
			}
		}
		public void Remove(ASPxSummaryItem summary) {
			for(int n = Count - 1; n >= 0; n--) {
				ASPxGroupSummarySortInfo info = this[n];
				if(info.SummaryItem == summary) RemoveAt(n);
			}
		}
		public void AddRange(params ASPxGroupSummarySortInfo[] sortInfos) {
			BeginUpdate();
			try {
				foreach(ASPxGroupSummarySortInfo info in sortInfos) { Add(info); }
			}
			finally {
				EndUpdate();
			}
		}
		public override void Assign(Utils.IAssignableCollection source) {
			var grid = Owner as ASPxGridView;
			var sourceCollection = source as ASPxGroupSummarySortInfoCollection;
			if(sourceCollection == null || grid == null)
				return;
			BeginUpdate();
			try {
				foreach(ASPxGroupSummarySortInfo sortInfo in sourceCollection) {
					var newSortInfo = (ASPxGroupSummarySortInfo)CloneItem(sortInfo);
					if(sortInfo.SummaryItem != null && grid.GroupSummary.Count > sortInfo.SummaryItem.Index)
						newSortInfo.SummaryItem = grid.GroupSummary[sortInfo.SummaryItem.Index];
					Add(newSortInfo);
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
}
