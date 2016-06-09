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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DashboardWin.Bars {
	public class TextBoxToolsBar : DashboardCommandBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryTextBoxToolsName); } }
		public TextBoxToolsBar() {
			Visible = false;
		}
		protected override bool CheckBarVisibility(DashboardItemViewer viewer) {
			return viewer is TextBoxDashboardItemViewer;
		}
	}
	public class TextBoxToolsRibbonPageCategory : DashboardRibbonPageCategory {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryTextBoxToolsName); } }
		protected override DashboardCommandId EmptyCommandId { get { return DashboardCommandId.None; } }
		protected internal override bool CheckCurrentPage(string dashboardItemType) {
			return dashboardItemType == DashboardItemType.Text;
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new TextBoxToolsRibbonPageCategory();
		}
	}
	public class TextBoxSettingsRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupTextBoxSettingsCaption); } }
	}
	public class TextBoxEditTextBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.TextBoxEditText; } }
		protected override string GetDefaultCaption() {
			return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandTextBoxEditTextRibbonCaption);
		}
	}
}
namespace DevExpress.DashboardWin.Native {
	public class TextBoxFormatBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new TextBoxEditTextBarItem());
		}
	}
	public class TextBoxFormatBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(TextBoxSettingsRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TextBoxToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(TextBoxToolsBar); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new TextBoxFormatBarItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TextBoxSettingsRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TextBoxToolsRibbonPageCategory();
		}
		public override Bar CreateBar() {
			return new TextBoxToolsBar();
		}
	}
}
