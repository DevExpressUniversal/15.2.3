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
using DevExpress.XtraBars;
using System.Collections.Generic;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetShowItemBuilder
	public class SpreadsheetShowItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarCheckItem checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.ViewShowGridlines);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
			checkItem = SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.ViewShowHeadings);
			checkItem.CheckBoxVisibility = CheckBoxVisibility.BeforeText;
			items.Add(checkItem);
		}
	}
	#endregion
	#region SpreadsheetZoomItemBuilder
	public class SpreadsheetZoomItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ViewZoomOut));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ViewZoomIn));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ViewZoom100Percent));
		}
	}
	#endregion
	#region SpreadsheetWindowItemBuilder
	public class SpreadsheetWindowItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem freezeSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.ViewFreezePanesCommandGroup);
			freezeSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ViewFreezePanes));
			freezeSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ViewUnfreezePanes));
			freezeSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ViewFreezeTopRow));
			freezeSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ViewFreezeFirstColumn));
			items.Add(freezeSubItem);
		}
	}
	#endregion
	#region SpreadsheetShowBarCreator
	public class SpreadsheetShowBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ViewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ShowRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ShowBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 4; } }
		public override Bar CreateBar() {
			return new ShowBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetShowItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ShowRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetZoomBarCreator
	public class SpreadsheetZoomBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ViewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ZoomRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ZoomBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 5; } }
		public override Bar CreateBar() {
			return new ZoomBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetZoomItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ZoomRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetWindowBarCreator
	public class SpreadsheetWindowBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ViewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(WindowRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(WindowBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 6; } }
		public override Bar CreateBar() {
			return new WindowBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetWindowItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new WindowRibbonPageGroup();
		}
	}
	#endregion
	#region ShowBar
	public class ShowBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ShowBar() {
		}
		public ShowBar(BarManager manager)
			: base(manager) {
		}
		public ShowBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupShow); } }
	}
	#endregion
	#region ZoomBar
	public class ZoomBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ZoomBar() {
		}
		public ZoomBar(BarManager manager)
			: base(manager) {
		}
		public ZoomBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupZoom); } }
	}
	#endregion
	#region WindowBar
	public class WindowBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public WindowBar() {
		}
		public WindowBar(BarManager manager)
			: base(manager) {
		}
		public WindowBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupWindow); } }
	}
	#endregion
	#region ViewRibbonPage
	public class ViewRibbonPage : ControlCommandBasedRibbonPage {
		public ViewRibbonPage() {
		}
		public ViewRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageView); } }
	}
	#endregion
	#region ShowRibbonPageGroup
	public class ShowRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ShowRibbonPageGroup() {
		}
		public ShowRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupShow); } }
	}
	#endregion
	#region ZoomRibbonPageGroup
	public class ZoomRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ZoomRibbonPageGroup() {
		}
		public ZoomRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupZoom); } }
	}
	#endregion
	#region WindowRibbonPageGroup
	public class WindowRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public WindowRibbonPageGroup() {
		}
		public WindowRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupWindow); } }
	}
	#endregion
}
