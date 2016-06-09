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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Data;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxTreeList.Internal;
namespace DevExpress.Web.ASPxTreeList {
	public class TreeListSummaryItem : CollectionItem, IDataSourceViewSchemaAccessor {		
		protected internal ASPxTreeList TreeList { get { return GetTreeList(); } }
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSummaryItemSummaryType"),
#endif
		Category("Data"), NotifyParentProperty(true), DefaultValue(SummaryItemType.None), AutoFormatDisable]
		public SummaryItemType SummaryType {
			get { return (SummaryItemType)GetEnumProperty("SummaryType", SummaryItemType.None); }
			set {
				if(value != SummaryType) {
					SetEnumProperty("SummaryType", SummaryItemType.None, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSummaryItemFieldName"),
#endif
		Category("Data"), NotifyParentProperty(true), DefaultValue(""), AutoFormatDisable, Localizable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string FieldName {
			get { return GetStringProperty("FieldName", ""); }
			set {
				if(value != FieldName) {
					SetStringProperty("FieldName", "", value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSummaryItemRecursive"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool Recursive {
			get { return GetBoolProperty("Recursive", true); }
			set {
				if(value != Recursive) {
					SetBoolProperty("Recursive", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSummaryItemShowInColumn"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatDisable, Localizable(false),
		TypeConverter("DevExpress.Web.ASPxTreeList.Design.TreeListColumnTypeConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string ShowInColumn {
			get { return GetStringProperty("ShowInColumn", ""); }
			set {
				if(value != ShowInColumn) {
					SetStringProperty("ShowInColumn", "", value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSummaryItemDisplayFormat"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string DisplayFormat {
			get { return GetStringProperty("DisplayFormat", ""); }
			set {
				if(value == null) value = "";
				if(value != DisplayFormat) {
					SetStringProperty("DisplayFormat", "", value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListSummaryItemTag"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string Tag {
			get { return GetStringProperty("Tag", ""); }
			set { SetStringProperty("Tag", "", value); }
		}
		public override string ToString() {			
			if(TreeList == null || SummaryType == SummaryItemType.None || String.IsNullOrEmpty(FieldName))
				return base.ToString();
			return String.Format("{0}({1})", SummaryType, FieldName);
		}
		protected internal string GetFormatString() {
			if(!String.IsNullOrEmpty(DisplayFormat))
				return DisplayFormat;
			switch(SummaryType) {
				case SummaryItemType.Average: return "Avg={0:#.##}";
				case SummaryItemType.Count: return "Count={0}";
				case SummaryItemType.Max: return "Max={0}";
				case SummaryItemType.Min: return "Min={0}";
				case SummaryItemType.Sum: return "Sum={0:#.##}";
				default: return "{0}";
			}
		}
		public override void Assign(CollectionItem source) {			
			base.Assign(source);
			TreeListSummaryItem item = source as TreeListSummaryItem;
			if(item == null)
				return;
			SummaryType = item.SummaryType;
			FieldName = item.FieldName;
			Recursive = item.Recursive;
			ShowInColumn = item.ShowInColumn;
			DisplayFormat = item.DisplayFormat;
			Tag = item.Tag;
		}
		void Changed() {
			LayoutChanged();
		}
		ASPxTreeList GetTreeList() {
			if(Collection == null)
				return null;
			return Collection.Owner as ASPxTreeList;
		}
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema {
			get {			
				IDataSourceViewSchemaAccessor accessor = GetTreeList();
				if(accessor != null)
					return accessor.DataSourceViewSchema;
				return null;
			}
			set { }
		}		
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class TreeListSummaryCollection : Collection<TreeListSummaryItem> {
		public TreeListSummaryCollection(IWebControlObject owner) 
			: base(owner) {
		}
		public override string ToString() { 
			return String.Empty; 
		}
	}
}
namespace DevExpress.Web.ASPxTreeList.Internal {
	public abstract class TreeListSummaryValue {
		TreeListSummaryItem summaryItem;
		public TreeListSummaryValue(TreeListSummaryItem summaryItem) {
			this.summaryItem = summaryItem;
		}
		protected TreeListSummaryItem SummaryItem { get { return summaryItem; } }
		protected string FieldName { get { return SummaryItem.FieldName; } }
		public abstract object Value { get; }
		public virtual void Start() {
		}
		public virtual void Finish() {
		}
		public abstract void Calculate(TreeListNode node);
		protected object GetDefaultNodeValue(TreeListNode node) {
			return node.GetValue(FieldName);			
		}
	}
	public class TreeListSummaryCountValue : TreeListSummaryValue {
		int count;
		public TreeListSummaryCountValue(TreeListSummaryItem summaryItem)
			: base(summaryItem) {
		}
		public override object Value { get { return count; } }
		public override void Start() {
			this.count = 0;
		}
		public override void Calculate(TreeListNode node) {
			this.count++;
		}
	}
	public class TreeListSummaryMinValue : TreeListSummaryValue {
		object min;
		public TreeListSummaryMinValue(TreeListSummaryItem summaryItem)
			: base(summaryItem) {
		}
		public override object Value { get { return min; } }
		public override void Calculate(TreeListNode node) {
			object value = GetDefaultNodeValue(node);
			if(Value == null || Comparer.Default.Compare(value, Value) < 0)
				this.min = value;
		}
	}
	public class TreeListSummaryMaxValue : TreeListSummaryValue {
		object max;
		public TreeListSummaryMaxValue(TreeListSummaryItem summaryItem)
			: base(summaryItem) {
		}
		public override object Value { get { return max; } }
		public override void Calculate(TreeListNode node) {
			object value = GetDefaultNodeValue(node);
			if(Value == null || Comparer.Default.Compare(value, Value) > 0)
				this.max = value;
		}
	}
	public class TreeListSummarySumValue : TreeListSummaryValue {
		decimal sum;
		public TreeListSummarySumValue(TreeListSummaryItem summaryItem)
			: base(summaryItem) {
		}
		protected decimal Sum { get { return sum; } set { sum = value; } }
		public override object Value { get { return sum; } }
		public override void Calculate(TreeListNode node) {
			object value = GetDefaultNodeValue(node);
			try {
				this.sum += Convert.ToDecimal(value);
			} catch {
			}
		}
	}
	public class TreeListSummaryAvgValue : TreeListSummarySumValue {
		int count;
		public TreeListSummaryAvgValue(TreeListSummaryItem summaryItem)
			: base(summaryItem) {
		}
		public override void Start() {
			base.Start();
			this.count = 0;
		}
		public override void Calculate(TreeListNode node) {
			base.Calculate(node);
			this.count++;
		}
		public override void Finish() {
			Sum /= count;
		}
	}
	public class TreeListSummaryCustomValue : TreeListSummaryValue {
		ASPxTreeList treeList;
		object value;
		public TreeListSummaryCustomValue(TreeListSummaryItem summaryItem, ASPxTreeList treeList)
			: base(summaryItem) {
			this.treeList = treeList;
		}
		public override object Value { get { return value; } }
		protected ASPxTreeList TreeList { get { return treeList; } }
		public override void Start() { RaiseEvent(null, CustomSummaryProcess.Start); }
		public override void Calculate(TreeListNode node) { RaiseEvent(node, CustomSummaryProcess.Calculate); }
		public override void Finish() { RaiseEvent(null, CustomSummaryProcess.Finalize); }
		void RaiseEvent(TreeListNode node, CustomSummaryProcess process) {
			TreeListCustomSummaryEventArgs e = new TreeListCustomSummaryEventArgs(node, SummaryItem, process);
			e.Value = Value;
			TreeList.RaiseCustomSummaryCalculate(e);
			this.value = e.Value;
		}
	}
	public class TreeListSummaryCachedValue : TreeListSummaryValue {
		object value;
		public TreeListSummaryCachedValue(TreeListSummaryItem item, object value) 
			: base(item) {
			this.value = value;
		}
		public override object Value { get { return value; } }
		public override void Start() { }
		public override void Calculate(TreeListNode node) { }
		public override void Finish() { }
	}
}
