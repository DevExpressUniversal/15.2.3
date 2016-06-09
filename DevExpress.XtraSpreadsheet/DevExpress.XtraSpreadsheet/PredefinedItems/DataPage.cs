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
using System.Collections.Generic;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SortAndFilter
	#region SpreadsheetSortAndFilterItemBuilder
	public class SpreadsheetSortAndFilterItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.DataSortAscending));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.DataSortDescending));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.DataFilterToggle));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.DataFilterClear));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.DataFilterReApply));
		}
	}
	#endregion
	#region SpreadsheetSortAndFilterBarCreator
	public class SpreadsheetSortAndFilterBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(DataRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(SortAndFilterRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(SortAndFilterBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new SortAndFilterBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetSortAndFilterItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new DataRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new SortAndFilterRibbonPageGroup();
		}
	}
	#endregion
	#region SortAndFilterBar
	public class SortAndFilterBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public SortAndFilterBar() {
		}
		public SortAndFilterBar(BarManager manager)
			: base(manager) {
		}
		public SortAndFilterBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupSortAndFilter); } }
	}
	#endregion
	#region SortAndFilterRibbonPageGroup
	public class SortAndFilterRibbonPageGroup :SpreadsheetControlRibbonPageGroup {
		public SortAndFilterRibbonPageGroup() {
		}
		public SortAndFilterRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return DoubleExistingAmpersands(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupSortAndFilter)); } }
		public static string DoubleExistingAmpersands(string text) {
			if (!String.IsNullOrEmpty(text))
				return text.Replace("&", "&&");
			else
				return text;
		}
	}
	#endregion
	#endregion
	#region DataTools
	#region SpreadsheetDataToolsItemBuilder
	public class SpreadsheetDataToolsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem dataValidationSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.DataToolsDataValidationCommandGroup);
			dataValidationSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.DataToolsDataValidation));
			dataValidationSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.DataToolsCircleInvalidData));
			dataValidationSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.DataToolsClearValidationCircles));
			items.Add(dataValidationSubItem);
		}
	}
	#endregion
	#region SpreadsheetDataToolsBarCreator
	public class SpreadsheetDataToolsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(DataRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(DataToolsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(DataToolsBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new DataToolsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetDataToolsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new DataRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new DataToolsRibbonPageGroup();
		}
	}
	#endregion
	#region DataToolsBar
	public class DataToolsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public DataToolsBar() {
		}
		public DataToolsBar(BarManager manager)
			: base(manager) {
		}
		public DataToolsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupDataTools); } }
	}
	#endregion
	#region DataToolsRibbonPageGroup
	public class DataToolsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public DataToolsRibbonPageGroup() {
		}
		public DataToolsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupDataTools); } }
	}
	#endregion
	#endregion
	#region Outline
	#region SpreadsheetOutlineItemBuilder
	public class SpreadsheetOutlineItemBuilder :CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem groupSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.OutlineGroupCommandGroup, RibbonItemStyles.Large);
			groupSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.GroupOutline));
			groupSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.AutoOutline));
			items.Add(groupSubItem);
			SpreadsheetCommandBarSubItem unGroupSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.OutlineUngroupCommandGroup, RibbonItemStyles.Large);
			unGroupSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.UngroupOutline));
			unGroupSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ClearOutline));
			items.Add(unGroupSubItem);
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.Subtotal, RibbonItemStyles.Large));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ShowDetail, RibbonItemStyles.SmallWithText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.HideDetail, RibbonItemStyles.SmallWithText));
		}
	}
	#endregion
	#region SpreadsheetOutlineBarCreator
	public class SpreadsheetOutlineBarCreator :ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(DataRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(OutlineRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(OutlineBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new OutlineBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetOutlineItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new DataRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new OutlineRibbonPageGroup();
		}
	}
	#endregion
	#region OutlineBar
	public class OutlineBar :ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public OutlineBar() {
		}
		public OutlineBar(BarManager manager)
			: base(manager) {
		}
		public OutlineBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupOutline); } }
	}
	#endregion
	#region OutlineRibbonPageGroup
	public class OutlineRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public OutlineRibbonPageGroup() {
		}
		public OutlineRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupOutline); } }
		public override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.OutlineSettingsCommand; } }
	}
	#endregion
	#endregion
	#region DataRibbonPage
	public class DataRibbonPage : ControlCommandBasedRibbonPage {
		public DataRibbonPage() {
		}
		public DataRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageData); } }
	}
	#endregion
}
