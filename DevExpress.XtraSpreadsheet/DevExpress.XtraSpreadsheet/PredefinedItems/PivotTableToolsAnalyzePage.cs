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

using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetPivotTableAnalyzePivotTableItemBuilder
	public class SpreadsheetPivotTableAnalyzePivotTableItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.OptionsPivotTable));
		}
	}
	#endregion
	#region SpreadsheetPivotTableAnalyzeActiveFieldItemBuilder
	public class SpreadsheetPivotTableAnalyzeActiveFieldItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.SelectFieldTypePivotTable));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableExpandField, RibbonItemStyles.SmallWithText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PivotTableCollapseField, RibbonItemStyles.SmallWithText));
		}
	}
	#endregion
	#region SpreadsheetPivotTableAnalyzeDataItemBuilder
	public class SpreadsheetPivotTableAnalyzeDataItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem refreshSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.PivotTableDataRefreshGroup);
			refreshSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.RefreshPivotTable));
			refreshSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.RefreshAllPivotTable));
			items.Add(refreshSubItem);
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ChangeDataSourcePivotTable));
		}
	}
	#endregion
	#region SpreadsheetPivotTableAnalyzeActionsItemBuilder
	public class SpreadsheetPivotTableAnalyzeActionsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem clearSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.PivotTableActionsClearGroup);
			clearSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ClearAllPivotTable));
			clearSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ClearFiltersPivotTable));
			SpreadsheetCommandBarSubItem selectSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.PivotTableActionsSelectGroup);
			selectSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.SelectValuesPivotTable));
			selectSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.SelectLabelsPivotTable));
			selectSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.SelectEntirePivotTable));
			items.Add(clearSubItem);
			items.Add(selectSubItem);
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MovePivotTable));
		}
	}
	#endregion
	#region SpreadsheetPivotTableAnalyzeShowItemBuilder
	public class SpreadsheetPivotTableAnalyzeShowItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FieldListPanelPivotTable));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.ShowPivotTableExpandCollapseButtons));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.ShowPivotTableFieldHeaders));
		}
	}
	#endregion
	#region SpreadsheetPivotTableAnalyzePivotTableBarCreator
	public class SpreadsheetPivotTableAnalyzePivotTableBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PivotTableAnalyzeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PivotTableAnalyzePivotTableRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(PivotTableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(PivotTableAnalyzePivotTableBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new PivotTableAnalyzePivotTableBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPivotTableAnalyzePivotTableItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PivotTableAnalyzeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PivotTableAnalyzePivotTableRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new PivotTableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetPivotTableAnalyzeAcitveFieldBarCreator
	public class SpreadsheetPivotTableAnalyzeAcitveFieldBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PivotTableAnalyzeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PivotTableAnalyzeActiveFieldRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(PivotTableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(PivotTableAnalyzeActiveFieldBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new PivotTableAnalyzePivotTableBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPivotTableAnalyzeActiveFieldItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PivotTableAnalyzeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PivotTableAnalyzeActiveFieldRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new PivotTableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetPivotTableAnalyzeDataBarCreator
	public class SpreadsheetPivotTableAnalyzeDataBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PivotTableAnalyzeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PivotTableAnalyzeDataRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(PivotTableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(PivotTableAnalyzeDataBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new PivotTableAnalyzePivotTableBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPivotTableAnalyzeDataItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PivotTableAnalyzeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PivotTableAnalyzeDataRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new PivotTableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetPivotTableAnalyzeActionsBarCreator
	public class SpreadsheetPivotTableAnalyzeActionsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PivotTableAnalyzeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PivotTableAnalyzeActionsRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(PivotTableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(PivotTableAnalyzeActionsBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new PivotTableAnalyzePivotTableBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPivotTableAnalyzeActionsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PivotTableAnalyzeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PivotTableAnalyzeActionsRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new PivotTableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region SpreadsheetPivotTableAnalyzeShowBarCreator
	public class SpreadsheetPivotTableAnalyzeShowBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PivotTableAnalyzeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PivotTableAnalyzeShowRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(PivotTableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(PivotTableAnalyzeShowBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new PivotTableAnalyzePivotTableBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPivotTableAnalyzeShowItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PivotTableAnalyzeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PivotTableAnalyzeShowRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new PivotTableToolsRibbonPageCategory();
		}
	}
	#endregion
	#region PivotTableAnalyzePivotTableBar
	public class PivotTableAnalyzePivotTableBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PivotTableAnalyzePivotTableBar() {
		}
		public PivotTableAnalyzePivotTableBar(BarManager manager)
			: base(manager) {
		}
		public PivotTableAnalyzePivotTableBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzePivotTable); } }
	}
	#endregion
	#region PivotTableAnalyzeActiveFieldBar
	public class PivotTableAnalyzeActiveFieldBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PivotTableAnalyzeActiveFieldBar() {
		}
		public PivotTableAnalyzeActiveFieldBar(BarManager manager)
			: base(manager) {
		}
		public PivotTableAnalyzeActiveFieldBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeActiveField); } }
	}
	#endregion
	#region PivotTableAnalyzeDataBar
	public class PivotTableAnalyzeDataBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PivotTableAnalyzeDataBar() {
		}
		public PivotTableAnalyzeDataBar(BarManager manager)
			: base(manager) {
		}
		public PivotTableAnalyzeDataBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeData); } }
	}
	#endregion
	#region PivotTableAnalyzeActionsBar
	public class PivotTableAnalyzeActionsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PivotTableAnalyzeActionsBar() {
		}
		public PivotTableAnalyzeActionsBar(BarManager manager)
			: base(manager) {
		}
		public PivotTableAnalyzeActionsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeActions); } }
	}
	#endregion
	#region PivotTableAnalyzeShowBar
	public class PivotTableAnalyzeShowBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PivotTableAnalyzeShowBar() {
		}
		public PivotTableAnalyzeShowBar(BarManager manager)
			: base(manager) {
		}
		public PivotTableAnalyzeShowBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeShow); } }
	}
	#endregion
	#region PivotTableAnalyzeRibbonPage
	public class PivotTableAnalyzeRibbonPage : ControlCommandBasedRibbonPage {
		public PivotTableAnalyzeRibbonPage() {
		}
		public PivotTableAnalyzeRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableAnalyze); } }
	}
	#endregion
	#region PivotTableAnalyzePivotTableRibbonPageGroup
	public class PivotTableAnalyzePivotTableRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PivotTableAnalyzePivotTableRibbonPageGroup() {
		}
		public PivotTableAnalyzePivotTableRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzePivotTable); } }
	}
	#endregion
	#region PivotTableAnalyzeActiveFieldRibbonPageGroup
	public class PivotTableAnalyzeActiveFieldRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PivotTableAnalyzeActiveFieldRibbonPageGroup() {
		}
		public PivotTableAnalyzeActiveFieldRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeActiveField); } }
	}
	#endregion
	#region PivotTableAnalyzeDataRibbonPageGroup
	public class PivotTableAnalyzeDataRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PivotTableAnalyzeDataRibbonPageGroup() {
		}
		public PivotTableAnalyzeDataRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeData); } }
	}
	#endregion
	#region PivotTableAnalyzeActionsRibbonPageGroup
	public class PivotTableAnalyzeActionsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PivotTableAnalyzeActionsRibbonPageGroup() {
		}
		public PivotTableAnalyzeActionsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeActions); } }
	}
	#endregion
	#region PivotTableAnalyzeShowRibbonPageGroup
	public class PivotTableAnalyzeShowRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PivotTableAnalyzeShowRibbonPageGroup() {
		}
		public PivotTableAnalyzeShowRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTableAnalyzeShow); } }
	}
	#endregion
	#region PivotTableToolsRibbonPageCategory
	public class PivotTableToolsRibbonPageCategory : ControlCommandBasedRibbonPageCategory<SpreadsheetControl, SpreadsheetCommandId> {
		public PivotTableToolsRibbonPageCategory() {
			this.Visible = false;
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageCategoryPivotTableTools); } }
		protected override SpreadsheetCommandId EmptyCommandId { get { return SpreadsheetCommandId.None; } }
		public override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.ToolsPivotTableCommandGroup; } }
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new PivotTableToolsRibbonPageCategory();
		}
	}
	#endregion
}
