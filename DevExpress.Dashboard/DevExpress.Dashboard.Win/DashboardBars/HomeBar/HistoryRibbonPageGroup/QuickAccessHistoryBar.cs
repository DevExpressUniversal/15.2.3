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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Bars {
	public class QuickAccessHistoryRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupHistoryCaption); } }
		public QuickAccessHistoryRibbonPageGroup() { 
			Visible = false; 
		}
	}
	public abstract class QuickAccessHistoryBarItem : DashboardBarButtonItem {
		public override bool AddToQuickAccess { get { return true; } }
		protected QuickAccessHistoryBarItem() {
		}
		protected override void OnControlUpdateUI(object sender, EventArgs e) {
			base.OnControlUpdateUI(sender,e);
			UpdateSuperTipAndShortCut();
		}
	}
	public class QuickAccessUndoBarItem : QuickAccessHistoryBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.Undo; } }
	}
	public class QuickAccessRedoBarItem : QuickAccessHistoryBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.Redo; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class QuickAccessHistoryBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new QuickAccessUndoBarItem());
			items.Add(new QuickAccessRedoBarItem());
		}
	}
	public class QuickAccessHistoryBarCreator : HomeBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(QuickAccessHistoryRibbonPageGroup); } }
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new QuickAccessHistoryRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new QuickAccessHistoryBarItemBuilder();
		}
	}
}
