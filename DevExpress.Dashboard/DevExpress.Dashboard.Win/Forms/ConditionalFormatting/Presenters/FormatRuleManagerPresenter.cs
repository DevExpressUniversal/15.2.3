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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	class FormatRuleManagerPresenter : FormatRulePresenterBase {
		FilterDataItemWrapperList filterDataItems;
		DataItemWrapperList calculateByDataItems;
		protected override string Caption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandEditRulesCaption); } }
		IFormatRuleManagerView SpecificView {
			get { return (IFormatRuleManagerView)base.View; }
		}
		FilterDataItemWrapperList FilterDataItems {
			get {
				if(filterDataItems == null) {
					IEnumerable<DataItem> dataItems = DashboardItem.FormatRulesInternal
						.Where(rule => rule.LevelCore.Item != null)
						.Select(rule => rule.LevelCore.Item)
						.Distinct();
					filterDataItems = new FilterDataItemWrapperList(dataItems);
				}
				return filterDataItems;
			}
		}
		DataItemWrapperList CalculateByDataItems {
			get {
				if(calculateByDataItems == null) {
					calculateByDataItems = new DataItemWrapperList();
					foreach(DataItem item in DashboardItem.GetInternalDataItems())
						if(DashboardItem.IsConditionalFormattingCalculateByAllowed(item))
							calculateByDataItems.Add(item);
				}
				return calculateByDataItems;
			}
		}
		public FormatRuleManagerPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem) 
			: base(serviceProvider, dashboardItem, dataItem) {
		}
		protected override IFormatRuleBaseViewInitializationContext ViewInitializationContext {
			get { return new FormatRulesManageViewInitializationContext(); }
		}
		protected override bool? InitializeView() {
			if(SpecificView != null) {
				SpecificView.SetFilterDataItems(FilterDataItems);
				SpecificView.SetCalculatedByDataItems(CalculateByDataItems);
				int filterIndex = FilterDataItems.IndexOf(DataItem);
				if(filterIndex < 0)
					filterIndex = FilterDataItems.IndexOf(null);
				SetSpecificViewRules(FilterDataItems[filterIndex]);
				SpecificView.SelectedFilterDataItemIndex = filterIndex;
				int selectedDataItemIndex = CalculateByDataItems.IndexOf(DataItem);
				SpecificView.SelectedCalculatedByDataItemIndex = selectedDataItemIndex > 0 ? selectedDataItemIndex : 0;
				SetViewPopupMenuItems();
			}
			return true;
		}
		protected override void ApplyView() {
		}
		protected override void ApplyHistory() {
		}
		protected override void SubscribeViewEvents() {
			base.SubscribeViewEvents();
			if(SpecificView != null) {
				SpecificView.FilterDataItemChanged += OnFilterDataItemChanged;
				SpecificView.CalculatedByDataItemChanged += OnCalculatedByDataItemChanged;
				SpecificView.Editing += OnRuleEditing;
				SpecificView.Deleting += OnRuleDeleting;
				SpecificView.Moving += OnRuleMoving;
				SpecificView.Enabling += OnRuleEnabling;
			}
		}
		protected override void UnsubscribeViewEvents() {
			base.UnsubscribeViewEvents();
			if(SpecificView != null) {
				SpecificView.FilterDataItemChanged -= OnFilterDataItemChanged;
				SpecificView.CalculatedByDataItemChanged -= OnCalculatedByDataItemChanged;
				SpecificView.Editing -= OnRuleEditing;
				SpecificView.Deleting -= OnRuleDeleting;
				SpecificView.Moving -= OnRuleMoving;
				SpecificView.Enabling -= OnRuleEnabling;
			}
		}
		protected override void OnCreated() {
			base.OnCreated();
			DashboardItem.FormatRulesInternal.Changed += OnFormatRulesChanged;
		}
		protected override void OnDestroyed() {
			base.OnDestroyed();
			DashboardItem.FormatRulesInternal.Changed -= OnFormatRulesChanged;
		}
		void OnFormatRulesChanged(object sender, EventArgs e) {
			SetSpecificViewRules();
		}
		void OnFilterDataItemChanged(object sender, EventArgs e) {
			SetSpecificViewRules();
		}
		void SetSpecificViewRules() {
			DataItem filterDataItem = FilterDataItems[SpecificView.SelectedFilterDataItemIndex];
			SetSpecificViewRules(filterDataItem);
		}
		void OnCalculatedByDataItemChanged(object sender, EventArgs e) {
			SetViewPopupMenuItems();
		}
		void SetViewPopupMenuItems() {
			DataItem dataItem = CalculateByDataItems[SpecificView.SelectedCalculatedByDataItemIndex];
			if(dataItem != null) {
				SpecificView.ClearPopupMenuItems();
				SpecificView.SetPopupMenuItems(FormatRuleControlBarItemsCreator.CreateValueFormatRuleItemContent(null, ServiceProvider, DashboardItem, dataItem), false);
				SpecificView.SetPopupMenuItems(FormatRuleControlBarItemsCreator.CreateRangeFormatRuleItemContent(null, ServiceProvider, DashboardItem, dataItem), true);
				SpecificView.SetPopupMenuItems(FormatRuleControlBarItemsCreator.CreateBarFormatRuleItemContent(null, ServiceProvider, DashboardItem, dataItem), true);
			}
		}
		Form OnRuleEditing(object sender, FormatRuleEditingEventArgs e) {
			return FormatRuleViewFactory.CreateControlViewForm(ServiceProvider, DashboardItem, (DashboardItemFormatRule)e.RuleView.Rule);			
		}
		void OnRuleEnabling(object sender, FormatRuleEnablingEventArgs e) {
			PropertyEnabledFormatRuleHistoryItem historyItem = new PropertyEnabledFormatRuleHistoryItem(DashboardItem, (DashboardItemFormatRule)e.RuleView.Rule, e.Enabled);
			HistoryService.RedoAndAdd(historyItem);
		}
		void OnRuleMoving(object sender, FormatRuleMovingEventArgs e) {
			MoveFormatRuleHistoryItem historyItem = new MoveFormatRuleHistoryItem(DashboardItem, (DashboardItemFormatRule)e.OldRuleView.Rule, (DashboardItemFormatRule)e.NewRuleView.Rule);
			HistoryService.RedoAndAdd(historyItem);
		}
		void OnRuleDeleting(object sender, FormatRuleDeletingEventArgs e) {
			DeleteFormatRuleHistoryItem historyItem = new DeleteFormatRuleHistoryItem(DashboardItem, (DashboardItemFormatRule)e.RuleView.Rule);
			HistoryService.RedoAndAdd(historyItem);
		}
		void SetSpecificViewRules(DataItem filterDataItem) {
			IList<IFormatRuleView> rules = new List<IFormatRuleView>();
			foreach(DashboardItemFormatRule rule in DashboardItem.FormatRulesInternal) {
				if(filterDataItem == null || (filterDataItem != null && filterDataItem == rule.LevelCore.Item))
					rules.Add(new DashboardItemFormatRuleView(rule));
			}
			SpecificView.SetRules(rules);
		}
	}
	class DashboardItemFormatRuleView : IFormatRuleView {
		readonly DashboardItemFormatRule rule;
		public DashboardItemFormatRule Rule {
			get { return rule; }
		}
		public DashboardItemFormatRuleView(DashboardItemFormatRule rule) {
			this.rule = rule;
		}
		#region IFormatRuleView Members
		object IFormatRuleView.Rule {
			get { return rule; }
		}
		bool IFormatRuleView.IsValid {
			get { return rule.IsValid; }
		}
		bool IFormatRuleView.Enabled {
			get { return rule.Enabled; }
			set { }
		}
		string IFormatRuleView.Caption {
			get { return rule.Caption(); }
		}
		string IFormatRuleView.DataItemCaption {
			get { return rule.LevelCore.Item != null ? rule.LevelCore.Item.DisplayName : DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleDataItemIsNotAvailable); }
		}
		string IFormatRuleView.DataItemApplyToCaption {
			get { return rule.LevelCore.ItemApplyTo != null ? rule.LevelCore.ItemApplyTo.DisplayName : DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleDataItemIsNotAvailable); }
		}
		#endregion
	}
}
