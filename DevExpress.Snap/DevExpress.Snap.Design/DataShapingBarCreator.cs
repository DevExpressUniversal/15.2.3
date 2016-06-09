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
	public class DataShapingBarCreator : ControlCommandBarCreator {
		readonly SnapControl snapControl;
		public DataShapingBarCreator(SnapControl snapControl) {
			this.snapControl = snapControl;
		}
		public override Type SupportedRibbonPageType { get { return typeof(SNMergeFieldToolsRibbonPage); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(DataToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(DataShapingRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(DataShapingBar); } }
		public override Bar CreateBar() {
			return new DataShapingBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new DataShapingBarItemBuilder(snapControl);
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new DataToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new DataShapingRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new SNMergeFieldToolsRibbonPage();
		}
	}
	public class DataShapingBarItemBuilder : CommandBasedBarItemBuilder {
		readonly SnapControl snapControl;
		public DataShapingBarItemBuilder(SnapControl snapControl) {
			this.snapControl = snapControl;
		}
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContex) {
			CommandBarCheckItem groupByFieldItem = new CommandBarCheckItem() { RibbonStyle = RibbonItemStyles.Large, SnapCommand = SnapCommand.GroupByField };
			items.Add(groupByFieldItem);
			items.Add(new CommandBarCheckItem() { SnapCommand = SnapCommand.SnapSortFieldAscending });
			items.Add(new CommandBarCheckItem() { SnapCommand = SnapCommand.SnapSortFieldDescending });
			CommandBarSubItem summaryByFieldItem = new CommandBarSubItem() { SnapCommand = Extensions.UI.SnapCommand.SummaryByField };
			items.Add(summaryByFieldItem);
			summaryByFieldItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.SummaryCount });
			summaryByFieldItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.SummarySum });
			summaryByFieldItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.SummaryAverage });
			summaryByFieldItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.SummaryMax });
			summaryByFieldItem.AddBarItem(new CommandBarCheckItem() { SnapCommand = SnapCommand.SummaryMin });
			items.Add(new FilterPopupButtonItem() { Control = snapControl, ActAsDropDown = true });
		}
	}
}
