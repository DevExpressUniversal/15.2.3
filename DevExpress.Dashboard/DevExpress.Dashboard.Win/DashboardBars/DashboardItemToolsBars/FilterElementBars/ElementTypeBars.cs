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
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
namespace DevExpress.DashboardWin.Bars {
	public class FilterElementTypeRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupFilterElementTypeCaption); } }
		protected override void OnDashboardItemSelected(DashboardItem dashboardItem) {
			base.OnDashboardItemSelected(dashboardItem);
			Visible = dashboardItem is ComboBoxDashboardItem || dashboardItem is ListBoxDashboardItem;
		}
	}
	public class TreeViewLayoutRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupTreeViewLayoutCaption); } }
		protected override void OnDashboardItemSelected(DashboardItem dashboardItem) {
			base.OnDashboardItemSelected(dashboardItem);
			Visible = dashboardItem is TreeViewDashboardItem;
		}
	}
	public class FilterElementItemOptionsRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupElementItemPropertiesCaption); } }
		protected override void OnDashboardItemSelected(DashboardItem dashboardItem) {
			base.OnDashboardItemSelected(dashboardItem);
			Visible = (dashboardItem as ListBoxDashboardItem != null || dashboardItem as ComboBoxDashboardItem != null);
		}
	}
	public class ComboBoxCheckedTypeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ComboBoxTypeChecked; } }
	}
	public class ComboBoxStandardTypeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ComboBoxTypeStandard; } }
	}
	public class ListBoxCheckedTypeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ListBoxTypeChecked; } }
	}
	public class ListBoxRadioTypeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ListBoxTypeRadio; } }
	}
	public class TreeViewStandardBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.TreeViewTypeStandard; } }
	}
	public class TreeViewCheckedBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.TreeViewTypeChecked; } }
	}
	public class TreeViewAutoExpandBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.TreeViewAutoExpand; } }
	}
	public class FilterElementShowAllValueBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.FilterElementShowAllValue; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class ComboBoxTypeBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ComboBoxStandardTypeBarItem());
			items.Add(new ComboBoxCheckedTypeBarItem());
		}
	}
	public class ListBoxTypeBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContex) {
			items.Add(new ListBoxCheckedTypeBarItem());
			items.Add(new ListBoxRadioTypeBarItem());
		}
	}
	public class TreeViewTypeBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContex) {
			items.Add(new TreeViewStandardBarItem());
			items.Add(new TreeViewCheckedBarItem());
		}
	}
	public class TreeViewLayoutBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContex) {
			items.Add(new TreeViewAutoExpandBarItem());
		}
	}
	public class FilterElementItemOptionsBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContex) {
			items.Add(new FilterElementShowAllValueBarItem());
		}
	}
	public class FilterElementTypeBarCreator<T> : DashboardItemDesignBarCreator where T: CommandBasedBarItemBuilder, new()  {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(FilterElementToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(FilterElementTypeRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(FilterElementToolsBar); } }
		public override Bar CreateBar() {
			return new FilterElementToolsBar();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new FilterElementToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new FilterElementTypeRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new T();
		}
	}
	public class TreeViewLayoutBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(FilterElementToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TreeViewLayoutRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(FilterElementToolsBar); } }
		public override Bar CreateBar() {
			return new FilterElementToolsBar();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new FilterElementToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TreeViewLayoutRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new TreeViewLayoutBarItemBuilder();
		}
	}
	public class FilterElementItemOptionsBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(FilterElementToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(FilterElementItemOptionsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(FilterElementToolsBar); } }
		public override Bar CreateBar() {
			return new FilterElementToolsBar();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new FilterElementToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new FilterElementItemOptionsRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new FilterElementItemOptionsBarItemBuilder();
		}
	}
}
