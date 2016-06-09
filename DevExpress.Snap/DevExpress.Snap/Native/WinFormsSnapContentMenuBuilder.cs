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
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Menu;
using DevExpress.XtraRichEdit;
using DevExpress.Utils.Menu;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Fields;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Snap.Localization;
namespace DevExpress.Snap.Native {
	public class WinFormsSnapContentMenuBuilder : WinFormsRichEditContentMenuBuilder {
		public WinFormsSnapContentMenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			AddSnapMenu(menu);
			AddSummarySubMenu(menu);
			AddChartMenu(menu);
			base.PopulatePopupMenu(menu);
		}
		protected override void AddCommentMenuItems(IDXPopupMenu<RichEditCommandId> menu, InnerRichEditControl innerControl) {
		}
		void AddChartMenu(IDXPopupMenu<RichEditCommandId> menu) {
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			SnapFieldInfo fieldInfo = FieldsHelper.GetSelectedField((SnapDocumentModel)Control.InnerControl.DocumentModel);
			if (fieldInfo == null)
				return;
			CalculatedFieldBase calculatedField = calculator.ParseField(fieldInfo);
			if (calculatedField != null && calculatedField is SNChartField)
				AddRunWizardMenu(menu);
		}
		void AddRunWizardMenu(IDXPopupMenu<RichEditCommandId> menu) {
			AddMenuItem(menu, new RunChartDesignerCommand(Control));
		}
		void AddSummarySubMenu(IDXPopupMenu<RichEditCommandId> menu) {
			if(IsSelectedFieldSparkline)
				return;
			ICommandUIState commandUIState = new DefaultCommandUIState();
			SummaryCommand summaryCommand = new SummaryCommand(Control);
			summaryCommand.UpdateUIState(commandUIState);
			if (!commandUIState.Enabled)
				return;
			SnapSummarySubmenuBuilder builder = new SnapSummarySubmenuBuilder(Control, UiFactory);
			IDXPopupMenu<RichEditCommandId> subMenu = builder.CreateSubMenu();
			subMenu.Caption = summaryCommand.MenuCaption;
			AppendSubmenu(menu, subMenu, false);
		}
		bool IsSelectedFieldSparkline {
			get {
				SnapFieldInfo fieldInfo = FieldsHelper.GetSelectedField((SnapDocumentModel)Control.InnerControl.DocumentModel);
				if(fieldInfo == null)
					return false;
				SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
				CalculatedFieldBase calculatedField = calculator.ParseField(fieldInfo);
				if(calculatedField != null && calculatedField is SNSparklineField)
					return true;
				return false;
			}
		}
		protected virtual void AddSnapMenu(IDXPopupMenu<RichEditCommandId> menu) {
			AddSortingMenuItems(menu);
			AddGroupingMenuItems(menu);
		}
		protected virtual void AddSortingMenuItems(IDXPopupMenu<RichEditCommandId> menu) {
			AddSortingMenuItem(menu, new SortFieldAscendingCommand(Control));
			AddSortingMenuItem(menu, new SnapSortFieldDescendingCommand(Control));
		}
		void AddSortingMenuItem(IDXPopupMenu<RichEditCommandId> menu, RichEditMenuItemSimpleCommand command) {
			ICommandUIState commandUIState = command.CreateDefaultCommandUIState();
			command.UpdateUIState(commandUIState);
			if (commandUIState.Enabled)
				AddMenuCheckItem(menu, command, "sorting");
		}
		protected virtual void AddGroupingMenuItems(IDXPopupMenu<RichEditCommandId> menu) {
			GroupByFieldCommand groupByFieldCommand = new GroupByFieldCommand(Control);
			ICommandUIState commandUIState = groupByFieldCommand.CreateDefaultCommandUIState();
			groupByFieldCommand.UpdateUIState(commandUIState);
			if(commandUIState.Enabled)
				AddMenuCheckItem(menu, groupByFieldCommand, "grouping");
		}
	}
	public class WinFormsSnapContentRadialMenuBuilder : WinFormsSnapContentMenuBuilder {
		public WinFormsSnapContentRadialMenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		protected override void AddClipboardMenuItems(IDXPopupMenu<RichEditCommandId> menu, InnerRichEditControl innerControl) {
			IDXPopupMenu<RichEditCommandId> subMenu = UiFactory.CreateSubMenu();
			subMenu.Caption = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_ClipboardSubItem);
			base.AddClipboardMenuItems(subMenu, innerControl);
			AppendSubmenu(menu, subMenu, false);
		}
		protected override void AddSortingMenuItems(IDXPopupMenu<RichEditCommandId> menu) {
			IDXPopupMenu<RichEditCommandId> subMenu = UiFactory.CreateSubMenu();
			base.AddSortingMenuItems(subMenu);
			subMenu.Caption = SnapLocalizer.GetString(SnapStringId.Sorting_MenuCaption);
			AppendSubmenu(menu, subMenu, false);
		}
		protected override RichEditTableCellsAlignmentSubmenuBuilder CreateTableCellsAlignmentSubmenuBuilder() {
			return new RichEditTableCellsAlignmentRadialMenuSubmenuBuilder(Control, UiFactory);
		}
	}
	public class SnapSummarySubmenuBuilder : RichEditMenuBuilder {
		public SnapSummarySubmenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			string summary = "summary";
			AddMenuCheckItem(menu, new SummaryAverageCommand(this.Control), summary);
			AddMenuCheckItem(menu, new SummaryCountCommand(this.Control), summary);
			AddMenuCheckItem(menu, new SummaryMinCommand(this.Control), summary);
			AddMenuCheckItem(menu, new SummaryMaxCommand(this.Control), summary);
			AddMenuCheckItem(menu, new SummarySumCommand(this.Control), summary);
		}
	}
}
