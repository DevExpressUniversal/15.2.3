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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.Office.UI;
using DevExpress.Utils.Text;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Commands;
using DevExpress.Office.Internal;
using DevExpress.XtraBars.Ribbon;
using System.Collections.Generic;
using DevExpress.XtraBars.Commands;
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Design;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Layout;
namespace DevExpress.XtraSpreadsheet.UI {
	#region MailMergeRibbonPage
	public class MailMergeRibbonPage :ControlCommandBasedRibbonPage {
		public MailMergeRibbonPage() {
		}
		public MailMergeRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageMailMerge); } }
	}
	#endregion
	#region MailMergeDataBarCreator
	public class MailMergeDataBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(MailMergeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(MailMergeDataRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(MailMergeDataBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new MailMergeDataBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new MailMergeDataItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new MailMergeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new MailMergeDataRibbonPageGroup();
		}
	}
	#endregion
	#region MailMergeModeBarCreator
	public class MailMergeModeBarCreator :ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(MailMergeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(MailMergeModeRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(MailMergeModeBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new MailMergeModeBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new MailMergeModeItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new MailMergeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new MailMergeModeRibbonPageGroup();
		}
	}
	#endregion
	#region MailMergeExtendedBarCreator
	public class MailMergeExtendedBarCreator :ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(MailMergeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(MailMergeExtendedRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(MailMergeExtendedBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new MailMergeExtendedBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new MailMergeExtendedItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new MailMergeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new MailMergeExtendedRibbonPageGroup();
		}
	}
	#endregion
	#region MailMergeGroupingBarCreator
	public class MailMergeGroupingBarCreator :ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(MailMergeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(MailMergeGroupingRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(MailMergeGroupingBar); } }
		public override Bar CreateBar() {
			return new MailMergeGroupingBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new MailMergeGroupingItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new MailMergeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new MailMergeGroupingRibbonPageGroup();
		}
	}
	#endregion
	#region MailMergeFilteringBarCreator
	public class MailMergeFilteringBarCreator :ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(MailMergeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(MailMergeFilteringRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(MailMergeFilteringBar); } }
		public override Bar CreateBar() {
			return new MailMergeFilteringBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new MailMergeFilteringItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new MailMergeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new MailMergeFilteringRibbonPageGroup();
		}
	}
	#endregion
	#region MailMergeBindingBarCreator
	public class MailMergeBindingBarCreator :ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(MailMergeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(MailMergeBindingRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(MailMergeBindingBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new MailMergeBindingBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new MailMergeDesignItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new MailMergeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new MailMergeBindingRibbonPageGroup();
		}
	}
	#endregion
	#region MailMergeDataRibbonPageGroup
	public class MailMergeDataRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public MailMergeDataRibbonPageGroup() {
		}
		public MailMergeDataRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeData); } }
	}
	#endregion
	#region MailMergeModeRibbonPageGroup
	public class MailMergeModeRibbonPageGroup :SpreadsheetControlRibbonPageGroup {
		public MailMergeModeRibbonPageGroup() {
		}
		public MailMergeModeRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeMode); } }
	}
	#endregion
	#region MailMergeExtendedRibbonPageGroup
	public class MailMergeExtendedRibbonPageGroup :SpreadsheetControlRibbonPageGroup {
		public MailMergeExtendedRibbonPageGroup() {
		}
		public MailMergeExtendedRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeExtended); } }
	}
	#endregion
	#region MailMergeGroupingRibbonPageGroup
	public class MailMergeGroupingRibbonPageGroup :SpreadsheetControlRibbonPageGroup {
		public MailMergeGroupingRibbonPageGroup() {
		}
		public MailMergeGroupingRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeGrouping); } }
	}
	#endregion
	#region MailMergeFilteringRibbonPageGroup
	public class MailMergeFilteringRibbonPageGroup :SpreadsheetControlRibbonPageGroup {
		public MailMergeFilteringRibbonPageGroup() {
		}
		public MailMergeFilteringRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeFiltering); } }
	}
	#endregion
	#region MailMergeBindingRibbonPageGroup
	public class MailMergeBindingRibbonPageGroup :SpreadsheetControlRibbonPageGroup {
		public MailMergeBindingRibbonPageGroup() {
		}
		public MailMergeBindingRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeBinding); } }
	}
	#endregion
	#region MailMergeDataBar
	public class MailMergeDataBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public MailMergeDataBar() {
		}
		public MailMergeDataBar(BarManager manager)
			: base(manager) {
		}
		public MailMergeDataBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeData); } }
	}
	#endregion
	#region MailMergeModeBar
	public class MailMergeModeBar :ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public MailMergeModeBar() {
		}
		public MailMergeModeBar(BarManager manager)
			: base(manager) {
		}
		public MailMergeModeBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeMode); } }
	}
	#endregion
	#region MailMergeExtendedBar
	public class MailMergeExtendedBar :ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public MailMergeExtendedBar() {
		}
		public MailMergeExtendedBar(BarManager manager)
			: base(manager) {
		}
		public MailMergeExtendedBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeExtended); } }
	}
	#endregion
	#region MailMergeGroupingBar
	public class MailMergeGroupingBar :ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public MailMergeGroupingBar() {
		}
		public MailMergeGroupingBar(BarManager manager)
			: base(manager) {
		}
		public MailMergeGroupingBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeGrouping); } }
	}
	#endregion
	#region MailMergeFilteringBar
	public class MailMergeFilteringBar :ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public MailMergeFilteringBar() {
		}
		public MailMergeFilteringBar(BarManager manager)
			: base(manager) {
		}
		public MailMergeFilteringBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeFiltering); } }
	}
	#endregion
	#region MailMergeBindingBar
	public class MailMergeBindingBar :ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public MailMergeBindingBar() {
		}
		public MailMergeBindingBar(BarManager manager)
			: base(manager) {
		}
		public MailMergeBindingBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupMailMergeBinding); } }
	}
	#endregion
	#region  MailMergeDataItemBuilder
	public class MailMergeDataItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeAddDataSource, RibbonItemStyles.Large));
			SpreadsheetCommandBarSubItem manageRelations = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.MailMergeManageRelationsCommandGroup, RibbonItemStyles.Large);
			manageRelations.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeManageQueriesCommand));
			manageRelations.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeManageRelationsCommand));
			items.Add(manageRelations);
			SpreadsheetCommandBarSubItem manageDataSource = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.MailMergeManageDataSourceCommandGroup, RibbonItemStyles.Large);
			manageDataSource.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeSelectDataSource));
			manageDataSource.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeSelectDataMember));
			manageDataSource.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeManageDataSourcesCommand));
			items.Add(manageDataSource);
		}
	}
	#endregion
	#region  MailMergeModeItemBuilder
	public class MailMergeModeItemBuilder :CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.MailMergeDocumentsMode, RibbonItemStyles.SmallWithText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.MailMergeOneDocumentMode, RibbonItemStyles.SmallWithText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.MailMergeOneSheetMode, RibbonItemStyles.SmallWithText));
			SpreadsheetCommandBarSubItem subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.MailMergeOrientationCommandGroup, RibbonItemStyles.Large);
			subItem.AddBarItem(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.MailMergeHorizontalMode));
			subItem.AddBarItem(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.MailMergeVerticalMode));
			items.Add(subItem);
		}
	}
	#endregion
	#region  MailMergeExtendedItemBuilder
	public class MailMergeExtendedItemBuilder :CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeSetHeaderRange, RibbonItemStyles.SmallWithText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeSetFooterRange, RibbonItemStyles.SmallWithText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeSetDetailRange, RibbonItemStyles.Large));
			SpreadsheetCommandBarSubItem subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.EditingMailMergeMasterDetailCommandGroup, RibbonItemStyles.Large);
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeSetDetailLevel));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeSetDetailDataMember));
			items.Add(subItem);
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeResetRange, RibbonItemStyles.Large));
		}
	}
	#endregion
	#region  MailMergeGroupingItemBuilder
	public class MailMergeGroupingItemBuilder :CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeSetGroup, RibbonItemStyles.Large));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeSetGroupHeader, RibbonItemStyles.SmallWithText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeSetGroupFooter, RibbonItemStyles.SmallWithText));
		}
	}
	#endregion
	#region  MailMergeFilteringItemBuilder
	public class MailMergeFilteringItemBuilder :CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeSetFilter, RibbonItemStyles.Large));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergeResetFilter, RibbonItemStyles.Large));
		}
	}
	#endregion
	#region  MailMergeDesignItemBuilder
	public class MailMergeDesignItemBuilder :CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.MailMergeShowRanges, RibbonItemStyles.Large));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MailMergePreview, RibbonItemStyles.Large));
		}
	}
	#endregion
}
