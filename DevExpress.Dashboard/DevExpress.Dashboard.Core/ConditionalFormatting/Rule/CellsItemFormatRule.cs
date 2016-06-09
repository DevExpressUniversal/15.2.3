#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DataItemBox = DevExpress.DashboardCommon.Native.DataItemBox<DevExpress.DashboardCommon.DataItem>;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public abstract class CellsItemFormatRule : DashboardItemFormatRule, IFormatRuleLevel {
		const string XmlDataItem = "DataItem";
		const string XmlDataItemApplyTo = "DataItemApplyTo";
		const string XmlApplyToRow = "ApplyToRow";
		const bool DefaultApplyToRow = false;
		bool applyToRow = DefaultApplyToRow;
		readonly DataItemBox itemBox;
		readonly DataItemBox itemApplyToBox;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CellsItemFormatRuleDataItem"),
#endif
		DefaultValue(null),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor))
		]
		public DataItem DataItem {
			get { return itemBox.DataItem; }
			set {
				if(DataItem != value) {
					this.itemBox.DataItem = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CellsItemFormatRuleDataItemApplyTo"),
#endif
		DefaultValue(null),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor))
		]
		public DataItem DataItemApplyTo {
			get { return itemApplyToBox.DataItem; }
			set {
				if(DataItemApplyTo != value) {
					this.itemApplyToBox.DataItem = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CellsItemFormatRuleApplyToRow"),
#endif
		DefaultValue(DefaultApplyToRow)
		]
		public bool ApplyToRow {
			get { return applyToRow; }
			set {
				if(ApplyToRow == value) return;
				applyToRow = value;
				OnChanged();
			}
		}
		[
		Browsable(false)
		]
		public override bool IsValid { get { return base.IsValid && (IsEvaluatorRequiredCondition && ActualDataItemApplyTo != null || DataItem != null && !string.IsNullOrEmpty(DataItem.ActualId)); } }
		protected internal abstract bool EnableLevel { get; }
		protected abstract Dimension GetAxisItem(bool isSecond);
		protected bool IsEvaluatorRequiredCondition { get { return Condition is IEvaluatorRequired; } }
		protected DataItem ActualDataItemApplyTo { get { return DataItemApplyTo ?? DataItem; } }
		internal override IFormatRuleLevel LevelCore { get { return this; } }
		protected CellsItemFormatRule() : this(null) {
		}
		protected CellsItemFormatRule(DataItem item)
			: this(item, null) {
		}
		protected CellsItemFormatRule(DataItem item, DataItem itemApplyTo) {
			this.itemBox = new DataItemBox(this, XmlDataItem);
			this.itemBox.DataItem = item;
			this.itemBox.Changed += (sender, e) => OnChanged();
			this.itemApplyToBox = new DataItemBox(this, XmlDataItemApplyTo);
			this.itemApplyToBox.DataItem = itemApplyTo;
			this.itemApplyToBox.Changed += (sender, e) => OnChanged();
		}
		protected override void AssignCore(DashboardItemFormatRule source) {
			base.AssignCore(source);
			CellsItemFormatRule obj = source as CellsItemFormatRule;
			if(obj != null) {
				DataItem = obj.DataItem;
				DataItemApplyTo = obj.DataItemApplyTo;
				ApplyToRow = obj.ApplyToRow;
			}
		}
		protected internal override void OnEndLoading() {
			base.OnEndLoading();
			itemBox.OnEndLoading();
			itemApplyToBox.OnEndLoading();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XmlHelper.Save(element, XmlApplyToRow, ApplyToRow, DefaultApplyToRow);
			itemBox.SaveToXml(element);
			itemApplyToBox.SaveToXml(element);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XmlHelper.Load<bool>(element, XmlApplyToRow, x => applyToRow = x);
			itemBox.LoadFromXml(element);
			itemApplyToBox.LoadFromXml(element);
		}
		protected Dimension FilterByEvaluatorRequiredDataMembers(IList<Dimension> axisItems, IEnumerable<string> filterDataMembers) {
			if(axisItems.Select(item => item.ActualId).Intersect(filterDataMembers).Count() > 0)
				return axisItems.Where(item => filterDataMembers.Contains(item.ActualId)).Last();
			return null;
		}
		#region IFormatRuleLevel Members
		bool IFormatRuleLevel.Enabled {
			get { return EnableLevel; } 
		}
		Dimension IFormatRuleLevel.Axis1Item {
			get { return GetAxisItem(false); }
		}
		Dimension IFormatRuleLevel.Axis2Item {
			get { return GetAxisItem(true); }
		}
		DataItem IFormatRuleLevel.Item {
			get { return DataItem; }
		}
		DataItem IFormatRuleLevel.ItemApplyTo {
			get { return ActualDataItemApplyTo; }
		}
		#endregion
	}
}
