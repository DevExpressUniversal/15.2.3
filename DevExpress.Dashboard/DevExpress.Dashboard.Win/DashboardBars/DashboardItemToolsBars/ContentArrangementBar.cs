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
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Bars {
	public class ContentArrangementRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupContentArrangementCaption); } }
	}
	public class ContentAutoArrangeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ContentAutoArrange; } }
	}
	public class ContentArrangeInColumnsBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ContentArrangeInColumns; } }
	}
	public class ContentArrangeInRowsBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ContentArrangeInRows; } }
	}
	public class ContentArrangementCountBarItem : CommandBarEditItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ContentArrangementCount; } }
		protected override ICommandUIState CreateCommandUIState(Command command) {
			int value;
			object editValue = EditValue;
			if (editValue == null)
				value = 0;
			else
				Int32.TryParse(editValue.ToString(), out value);
			DefaultValueBasedCommandUIState<int?> result = new DefaultValueBasedCommandUIState<int?>();
			result.Value = value;
			return result;
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItemSpinEdit edit = new RepositoryItemSpinEdit();
			edit.IsFloatValue = false;
			edit.Mask.EditMask = "N00";
			edit.MinValue = 1;
			edit.MaxValue = 10000;
			edit.EditValueChanged += (sender, e) => {
				BaseEdit activeEditor = GetActiveEditor();
				if(activeEditor != null)
					EditValue = activeEditor.EditValue;
			};
			return edit;
		}
		protected override string GetDefaultCaption() {
			return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandContentArrangementCountCaption);
		}
	}
}
namespace DevExpress.DashboardWin.Native {
	public class ContentArrangementBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ContentAutoArrangeBarItem());
			items.Add(new ContentArrangeInColumnsBarItem());
			items.Add(new ContentArrangeInRowsBarItem());
			items.Add(new ContentArrangementCountBarItem());
		}
	}
	public class ContentArrangementBarCreator<TPageCategory, TBar> : DashboardItemDesignBarCreator where TPageCategory : DashboardRibbonPageCategory, new() where TBar : Bar, new() {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ContentArrangementRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ContentArrangementRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ContentArrangementBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new TBar();
		}
	}
}
