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
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.Snap.Extensions.UI;
namespace DevExpress.Snap.Design {
	public class GroupingBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(GroupToolsRibbonPage); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(DataToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(GroupingRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(GroupingBar); } }
		public override Bar CreateBar() {
			return new GroupingBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new GroupingBarItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new DataToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new GroupingRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new GroupToolsRibbonPage();
		}
	}
	public class GroupingBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContex) {
			CommandBarSubItem headerItem = new CommandBarSubItem() { SnapCommand = Extensions.UI.SnapCommand.GroupHeader, RibbonStyle = RibbonItemStyles.Large };
			items.Add(headerItem);
			headerItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.InsertGroupHeader });
			headerItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.RemoveGroupHeader });
			CommandBarSubItem footerItem = new CommandBarSubItem() { SnapCommand = Extensions.UI.SnapCommand.GroupFooter};
			items.Add(footerItem);
			footerItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.InsertGroupFooter });
			footerItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.RemoveGroupFooter });
			items.Add(new CommandBarItem() { SnapCommand = SnapCommand.GroupFieldsCollection });
			items.Add(new CommandBarItem() { SnapCommand = SnapCommand.ShowReportStructureEditorForm });
			CommandBarSubItem separatorItem = new CommandBarSubItem() { SnapCommand = SnapCommand.InsertGroupSeparator };
			separatorItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.InsertPageBreakGroupSeparator });
			separatorItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.InsertSectionBreakNextPageGroupSeparator });
			separatorItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.InsertSectionBreakEvenPageGroupSeparator });
			separatorItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.InsertSectionBreakOddPageGroupSeparator });
			separatorItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.InsertEmptyParagraphGroupSeparator });
			separatorItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.InsertEmptyRowGroupSeparator });
			separatorItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.InsertNoneGroupSeparator });
			items.Add(separatorItem);
		}
	}
}
