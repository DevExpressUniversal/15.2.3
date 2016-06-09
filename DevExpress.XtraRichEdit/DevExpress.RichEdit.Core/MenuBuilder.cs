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
using System.Drawing;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils.Menu;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.SpellChecker;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Menu {
	#region RichEditMenuBuilder (abstract class)
	public abstract class RichEditMenuBuilder : CommandBasedPopupMenuBuilder<RichEditCommand, RichEditCommandId> {
		readonly IRichEditControl control;
		protected RichEditMenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(uiFactory) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public IRichEditControl Control { get { return control; } }
	}
	#endregion
	#region RichEditContentMenuBuilder (abstract class)
	public abstract class RichEditContentMenuBuilder : RichEditMenuBuilder {
		protected RichEditContentMenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			InnerRichEditControl innerControl = Control.InnerControl;
			bool hasFieldMenu = AddFieldMenuItems(menu);
			AddSpellCheckerMenuItems(menu);
			AddClipboardMenuItems(menu, innerControl);
			AddTableOptionsMenuItems(menu, innerControl);
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.IncreaseIndent), DXMenuItemPriority.Low).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.DecreaseIndent), DXMenuItemPriority.Low);
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ShowFontForm)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ShowParagraphForm));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ShowNumberingListForm), DXMenuItemPriority.Low);
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.CreateBookmark), DXMenuItemPriority.Low).BeginGroup = true;
			if (!hasFieldMenu)
				AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.CreateHyperlink), DXMenuItemPriority.Low);
#if !SL
			else
				AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ShowTOCForm), DXMenuItemPriority.Low);
#endif
			AddCommentMenuItems(menu, innerControl);
			AddFloatingObjectTextWrapTypeSubMenu(menu);
			AddFloatingObjectBringForwardSubMenu(menu);
			AddFloatingObjectSendBackwardSubMenu(menu);
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ShowFloatingObjectLayoutOptionsForm));
		}
		protected virtual void AddCommentMenuItems(IDXPopupMenu<RichEditCommandId> menu, InnerRichEditControl innerControl) {
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.NewComment), DXMenuItemPriority.Low).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.DeleteOneComment), DXMenuItemPriority.Low);
		}
		protected virtual void AddClipboardMenuItems(IDXPopupMenu<RichEditCommandId> menu, InnerRichEditControl innerControl) {
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.CutSelection)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.CopySelection));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.PasteSelection));
		}
		protected virtual void AddTableOptionsMenuItems(IDXPopupMenu<RichEditCommandId> menu, InnerRichEditControl innerControl) {
			AddTableInsertSubmenu(menu).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.DeleteTableRowsMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.DeleteTableColumnsMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ShowDeleteTableCellsFormMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.MergeTableElement));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ShowSplitTableCellsFormMenuItem));
			AddTableCellsAlignmentSubmenu(menu);
			AddTableAutoFitSubmenu(menu);
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ShowTablePropertiesFormMenuItem));
		}
		protected internal abstract RichEditHitTestResult CalculateCursorHitTestResult();
		protected virtual bool AddFieldMenuItems(IDXPopupMenu<RichEditCommandId> menu) {
			InnerRichEditControl innerControl = Control.InnerControl;
			Field field = GetSelectedField();
			while (field != null && field.HideByParent)
				field = field.Parent;
			if (field == null)
				return TryAddFieldsMenuItems(menu);
			if (innerControl.DocumentModel.ActivePieceTable.IsHyperlinkField(field))
				AddHyperlinkMenuItems(menu, innerControl, field);
			else
				AddFieldMenuItems(menu, field);
			return true;
		}
		bool TryAddFieldsMenuItems(IDXPopupMenu<RichEditCommandId> menu) {
			UpdateFieldsCommand command = new UpdateFieldsCommand(Control);
			if (command.CanExecute()) {
				AddMenuItem(menu, command).BeginGroup = true;
				return true;
			}
			return false;
		}
		Field GetSelectedField() {
			Selection selection = Control.InnerControl.DocumentModel.Selection;
			RunInfo selectionInterval = selection.Interval;
			DocumentModelPosition selectionStart = selectionInterval.NormalizedStart;
			DocumentModelPosition selectionEnd = selectionInterval.NormalizedEnd;
			PieceTable pieceTable = selection.PieceTable;
			int firstFieldIndex = pieceTable.FindFieldIndexByRunIndex(selectionStart.RunIndex);
			int lastFieldIndex = pieceTable.FindFieldIndexByRunIndex(selectionEnd.RunIndex);
			if (firstFieldIndex == lastFieldIndex && firstFieldIndex >= 0)
				return pieceTable.Fields[firstFieldIndex];
			if (firstFieldIndex < 0)
				firstFieldIndex = ~firstFieldIndex;
			if (lastFieldIndex < 0)
				lastFieldIndex = ~lastFieldIndex;
			if (firstFieldIndex == lastFieldIndex)
				return GetInvisibleField(selectionStart);
			if (firstFieldIndex == lastFieldIndex - 1)
				return pieceTable.Fields[firstFieldIndex];
			Field result = pieceTable.Fields[firstFieldIndex].GetTopLevelParent();
			for (int i = firstFieldIndex + 1; i < lastFieldIndex; i++) {
				Field current = pieceTable.Fields[i].GetTopLevelParent();
				if (!Object.ReferenceEquals(result, current))
					return null;
			}
			return result;
		}
		Field GetInvisibleField(DocumentModelPosition selectionStart) {
			RunIndex runIndex = selectionStart.RunIndex;
			if (selectionStart.RunOffset > 0 || runIndex == RunIndex.Zero)
				return null;
			PieceTable pieceTable = selectionStart.PieceTable;
			do {
				runIndex--;
			} while (runIndex > RunIndex.Zero && !pieceTable.VisibleTextFilter.IsRunVisible(runIndex));
			if (pieceTable.VisibleTextFilter.IsRunVisible(runIndex))
				runIndex++;
			if (runIndex == selectionStart.RunIndex)
				return null;
			int hiddenFieldIndex = pieceTable.FindFieldIndexByRunIndex(runIndex);
			if (hiddenFieldIndex >= 0 && pieceTable.Fields[hiddenFieldIndex].FirstRunIndex == runIndex)
				return pieceTable.Fields[hiddenFieldIndex];
			return null;
		}
		protected virtual void AddFieldMenuItems(IDXPopupMenu<RichEditCommandId> menu, Field field) {
			AddMenuItem(menu, new UpdateFieldCommand(Control, field)).BeginGroup = true;
			AddMenuItem(menu, new ToggleFieldCodesCommand(Control, field));
		}
		protected virtual void AddHyperlinkMenuItems(IDXPopupMenu<RichEditCommandId> menu, InnerRichEditControl innerControl, Field field) {
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.EditHyperlink)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.OpenHyperlink));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.RemoveHyperlink));
		}
		protected virtual void AddSpellCheckerMenuItems(IDXPopupMenu<RichEditCommandId> menu) {
			SpellCheckerCommandBuilder builder = new SpellCheckerCommandBuilder(Control, UiFactory);
			builder.PopulatePopupMenu(menu);
		}
		protected internal virtual IDXPopupMenu<RichEditCommandId> AddTableInsertSubmenu(IDXPopupMenu<RichEditCommandId> menu) {
			RichEditTableInsertSubmenuBuilder builder = new RichEditTableInsertSubmenuBuilder(Control, UiFactory);
			IDXPopupMenu<RichEditCommandId> subMenu = builder.CreateSubMenu();
			InsertTableElementMenuCommand command = new InsertTableElementMenuCommand(Control);
			subMenu.Caption = command.MenuCaption;
			AppendSubmenu(menu, subMenu, false);
			return subMenu;
		}
		protected internal virtual IDXPopupMenu<RichEditCommandId> AddTableCellsAlignmentSubmenu(IDXPopupMenu<RichEditCommandId> menu) {
			RichEditTableCellsAlignmentSubmenuBuilder builder = CreateTableCellsAlignmentSubmenuBuilder();
			IDXPopupMenu<RichEditCommandId> subMenu = builder.CreateSubMenu();
			ChangeTableCellsContentAlignmentPlaceholderCommand command = new ChangeTableCellsContentAlignmentPlaceholderCommand(Control);
			subMenu.Caption = command.MenuCaption;
			AppendSubmenu(menu, subMenu, false);
			return subMenu;
		}
		protected virtual RichEditTableCellsAlignmentSubmenuBuilder CreateTableCellsAlignmentSubmenuBuilder() {
			return new RichEditTableCellsAlignmentSubmenuBuilder(Control, UiFactory);
		}
		protected internal virtual IDXPopupMenu<RichEditCommandId> AddTableAutoFitSubmenu(IDXPopupMenu<RichEditCommandId> menu) {
			RichEditTableAutoFitSubmenuBuilder builder = new RichEditTableAutoFitSubmenuBuilder(Control, UiFactory);
			IDXPopupMenu<RichEditCommandId> subMenu = builder.CreateSubMenu();
			ToggleTableAutoFitPlaceholderMenuCommand command = new ToggleTableAutoFitPlaceholderMenuCommand(Control);
			subMenu.Caption = command.MenuCaption;
			AppendSubmenu(menu, subMenu, false);
			return subMenu;
		}
		protected internal virtual IDXPopupMenu<RichEditCommandId> AddFloatingObjectTextWrapTypeSubMenu(IDXPopupMenu<RichEditCommandId> menu) {
			RichEditFloatingObjectTextWrapTypeSubmenuBuilder builder = new RichEditFloatingObjectTextWrapTypeSubmenuBuilder(Control, UiFactory);
			IDXPopupMenu<RichEditCommandId> subMenu = builder.CreateSubMenu();
			ChangeFloatingObjectTextWrapTypeMenuCommand command = new ChangeFloatingObjectTextWrapTypeMenuCommand(Control);
			subMenu.Caption = command.MenuCaption;
			AppendSubmenu(menu, subMenu, false);
			return subMenu;
		}
		protected internal virtual IDXPopupMenu<RichEditCommandId> AddFloatingObjectBringForwardSubMenu(IDXPopupMenu<RichEditCommandId> menu) {
			RichEditFloatingObjectBringForwardSubmenuBuilder builder = new RichEditFloatingObjectBringForwardSubmenuBuilder(Control, UiFactory);
			IDXPopupMenu<RichEditCommandId> subMenu = builder.CreateSubMenu();
			FloatingObjectBringForwardMenuCommand command = new FloatingObjectBringForwardMenuCommand(Control);
			subMenu.Caption = command.MenuCaption;
			AppendSubmenu(menu, subMenu, false);
			return subMenu;
		}
		protected internal virtual IDXPopupMenu<RichEditCommandId> AddFloatingObjectSendBackwardSubMenu(IDXPopupMenu<RichEditCommandId> menu) {
			RichEditFloatingObjectSendBackwardSubmenuBuilder builder = new RichEditFloatingObjectSendBackwardSubmenuBuilder(Control, UiFactory);
			IDXPopupMenu<RichEditCommandId> subMenu = builder.CreateSubMenu();
			FloatingObjectSendBackwardMenuCommand command = new FloatingObjectSendBackwardMenuCommand(Control);
			subMenu.Caption = command.MenuCaption;
			AppendSubmenu(menu, subMenu, false);
			return subMenu;
		}
	}
	#endregion
	#region RichEditTableInsertSubmenuBuilder
	public class RichEditTableInsertSubmenuBuilder : RichEditMenuBuilder {
		public RichEditTableInsertSubmenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<RichEditCommandId> CreatePopupMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreatePopupMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		public override IDXPopupMenu<RichEditCommandId> CreateSubMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreateSubMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		protected internal virtual void UpdateMenuState(IDXPopupMenu<RichEditCommandId> menu) {
			InsertTableElementMenuCommand command = new InsertTableElementMenuCommand(Control);
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			menu.Caption = command.MenuCaption;
			menu.Visible = state.Visible;
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			InnerRichEditControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.InsertTableColumnToTheLeft));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.InsertTableColumnToTheRight));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.InsertTableRowAbove));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.InsertTableRowBelow));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ShowInsertTableCellsForm));
		}
	}
	#endregion
	#region RichEditTableCellsAlignmentSubmenuBuilder
	public class RichEditTableCellsAlignmentSubmenuBuilder : RichEditMenuBuilder {
		public RichEditTableCellsAlignmentSubmenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<RichEditCommandId> CreatePopupMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreatePopupMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		public override IDXPopupMenu<RichEditCommandId> CreateSubMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreateSubMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		protected internal virtual void UpdateMenuState(IDXPopupMenu<RichEditCommandId> menu) {
			ChangeTableCellsContentAlignmentPlaceholderCommand command = new ChangeTableCellsContentAlignmentPlaceholderCommand(Control);
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			menu.Caption = command.MenuCaption;
			menu.Visible = state.Visible;
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			string groupId = "TableCellsAlignment";
			InnerRichEditControl innerControl = Control.InnerControl;
			DoPopulatePopupMenuCore(menu, groupId, innerControl);
		}
		protected virtual void DoPopulatePopupMenuCore(IDXPopupMenu<RichEditCommandId> menu, string groupId, InnerRichEditControl innerControl) {
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsTopLeftAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsTopCenterAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsTopRightAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsMiddleLeftAlignment), groupId).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsMiddleCenterAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsMiddleRightAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsBottomLeftAlignment), groupId).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsBottomCenterAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsBottomRightAlignment), groupId);
		}
	}
	public class RichEditTableCellsAlignmentRadialMenuSubmenuBuilder : RichEditTableCellsAlignmentSubmenuBuilder {
		public RichEditTableCellsAlignmentRadialMenuSubmenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		protected override void DoPopulatePopupMenuCore(IDXPopupMenu<RichEditCommandId> menu, string groupId, InnerRichEditControl innerControl) {
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsTopCenterAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsTopRightAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsMiddleRightAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsBottomRightAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsBottomCenterAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsBottomLeftAlignment), groupId).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsMiddleCenterAlignment), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsMiddleLeftAlignment), groupId).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableCellsTopLeftAlignment), groupId);
		}
	}
	#endregion
	#region RichEditTableAutoFitSubmenuBuilder
	public class RichEditTableAutoFitSubmenuBuilder : RichEditMenuBuilder {
		public RichEditTableAutoFitSubmenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<RichEditCommandId> CreatePopupMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreatePopupMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		public override IDXPopupMenu<RichEditCommandId> CreateSubMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreateSubMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		protected internal virtual void UpdateMenuState(IDXPopupMenu<RichEditCommandId> menu) {
			ToggleTableAutoFitPlaceholderMenuCommand command = new ToggleTableAutoFitPlaceholderMenuCommand(Control);
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			menu.Caption = command.MenuCaption;
			menu.Visible = state.Visible;
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			InnerRichEditControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableAutoFitContents));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableAutoFitWindow));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleTableFixedColumnWidth));
		}
	}
	#endregion
	#region RichEditHoverMenuBuilder
	public class RichEditHoverMenuBuilder : RichEditMenuBuilder {
		public RichEditHoverMenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			InnerRichEditControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.CutSelection)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.CopySelection));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.PasteSelection));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.IncreaseFontSize)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.DecreaseFontSize));
			string fontGroupId = "FontGroup";
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleFontBold), fontGroupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleFontItalic), fontGroupId);
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ShowFontForm));
			string paragraphAlignmentGroupId = "ParagraphAlignmentGroup";
			string listGroupId = "ListGroup";
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleParagraphAlignmentCenter), paragraphAlignmentGroupId).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.DecreaseIndent));
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.IncreaseIndent));
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.ToggleMultilevelListItem), listGroupId);
			AddMenuItem(menu, innerControl.CreateCommand(RichEditCommandId.ShowParagraphForm));
		}
	}
	#endregion
	#region SpellCheckerCommandBuilder
	public class SpellCheckerCommandBuilder : RichEditMenuBuilder {
		class MisspelledIntervalIntervalAndLogPositionComparer : IComparable<MisspelledInterval> {
			readonly DocumentLogPosition position;
			public MisspelledIntervalIntervalAndLogPositionComparer(DocumentLogPosition position) {
				this.position = position;
			}
			#region IComparable<MisspelledInterval> Members
			public int CompareTo(MisspelledInterval other) {
				if (position < other.Start)
					return 1;
				if (position > other.End)
					return -1;
				return 0;
			}
			#endregion
		}
		public SpellCheckerCommandBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			PieceTable pieceTable = Control.InnerControl.DocumentModel.ActivePieceTable;
			DocumentLogPosition logPosition = pieceTable.DocumentModel.Selection.Interval.NormalizedStart.LogPosition;
			MisspelledIntervalCollection intervals = pieceTable.SpellCheckerManager.MisspelledIntervals;
			MisspelledInterval interval = intervals.FindInerval(logPosition);
			if (interval == null)
				return;
			if (interval.ErrorType == SpellingError.Misspelling) {
				DocumentModelPosition start = interval.Interval.Start;
				DocumentModelPosition end = interval.Interval.End;
				string word = GetWord(start, end);
				PopulateSuggestionCommands(menu, word, start, end);
				AddMenuItem(menu, Control.InnerControl.CreateCommand(RichEditCommandId.IgnoreMisspelling)).BeginGroup = true;
				AddMenuItem(menu, Control.InnerControl.CreateCommand(RichEditCommandId.IgnoreAllMisspellings));
				AddMenuItem(menu, Control.InnerControl.CreateCommand(RichEditCommandId.AddWordToDictionary));
			}
			else {
				AddMenuItem(menu, Control.InnerControl.CreateCommand(RichEditCommandId.DeleteRepeatedWord));
				AddMenuItem(menu, Control.InnerControl.CreateCommand(RichEditCommandId.IgnoreMisspelling));
			}
			AddMenuItem(menu, Control.InnerControl.CreateCommand(RichEditCommandId.CheckSpelling)).BeginGroup = true;
		}
		protected virtual void PopulateSuggestionCommands(IDXPopupMenu<RichEditCommandId> menu, string word, DocumentModelPosition start, DocumentModelPosition end) {
			string[] suggestions = GetSpellingSuggestions(word, start, end);
			int count = Math.Min(suggestions.Length, 5);
			if (count > 0) {
				for (int i = 0; i < count; i++) {
					RichEditCommand command = new ReplaceMisspellingCommand(Control, suggestions[i]);
					AddMenuItem(menu, command).BeginGroup = (i == 0);
				}
			}
			else
				AddMenuItem(menu, new ReplaceMisspellingCommand(Control)).BeginGroup = true;
		}
		string[] GetSpellingSuggestions(string word, DocumentModelPosition start, DocumentModelPosition end) {
			ISpellChecker spellChecker = Control.InnerDocumentServer.SpellChecker;
			Debug.Assert(spellChecker != null);
			System.Globalization.CultureInfo culture = spellChecker.Culture;
			if (Control.InnerControl.Options.SpellChecker.AutoDetectDocumentCulture) {
				LangInfo? langInfo = Control.InnerDocumentServer.DocumentModel.ActivePieceTable.GetLanguageInfo(start, end);
				if (langInfo.HasValue)
					culture = langInfo.Value.Latin;
			}
			return spellChecker.GetSuggestions(word, culture);
		}
		string GetWord(DocumentModelPosition start, DocumentModelPosition end) {
			Debug.Assert(Object.ReferenceEquals(start.PieceTable, end.PieceTable));
			IVisibleTextFilter filter = start.PieceTable.VisibleTextFilter;
			return start.PieceTable.GetFilteredPlainText(start, end, filter.IsRunVisible);
		}
	}
	#endregion
	#region RichEditFloatingObjectTextWrapTypeSubmenuBuilder
	public class RichEditFloatingObjectTextWrapTypeSubmenuBuilder : RichEditMenuBuilder {
		public RichEditFloatingObjectTextWrapTypeSubmenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<RichEditCommandId> CreatePopupMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreatePopupMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		public override IDXPopupMenu<RichEditCommandId> CreateSubMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreateSubMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		protected internal virtual void UpdateMenuState(IDXPopupMenu<RichEditCommandId> menu) {
			ChangeFloatingObjectTextWrapTypeMenuCommand command = new ChangeFloatingObjectTextWrapTypeMenuCommand(Control);
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			menu.Caption = command.MenuCaption;
			menu.Visible = state.Visible;
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			InnerRichEditControl innerControl = Control.InnerControl;
			const string groupId = "0";
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.SetFloatingObjectSquareTextWrapType), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.SetFloatingObjectTightTextWrapType), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.SetFloatingObjectThroughTextWrapType), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.SetFloatingObjectTopAndBottomTextWrapType), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.SetFloatingObjectBehindTextWrapType), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.SetFloatingObjectInFrontOfTextWrapType), groupId);
		}
	}
	#endregion
	#region RichEditFloatingObjectBringForwardSubmenuBuilder
	public class RichEditFloatingObjectBringForwardSubmenuBuilder : RichEditMenuBuilder {
		public RichEditFloatingObjectBringForwardSubmenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<RichEditCommandId> CreatePopupMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreatePopupMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		public override IDXPopupMenu<RichEditCommandId> CreateSubMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreateSubMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		protected internal virtual void UpdateMenuState(IDXPopupMenu<RichEditCommandId> menu) {
			FloatingObjectBringForwardMenuCommand command = new FloatingObjectBringForwardMenuCommand(Control);
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			menu.Caption = command.MenuCaption;
			menu.Visible = state.Visible;
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			InnerRichEditControl innerControl = Control.InnerControl;
			const string groupId = "0";
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.FloatingObjectBringToFront), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.FloatingObjectBringForward), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.FloatingObjectBringInFrontOfText), groupId);
		}
	}
	#endregion
	#region RichEditFloatingObjectSendBackwardSubmenuBuilder
	public class RichEditFloatingObjectSendBackwardSubmenuBuilder : RichEditMenuBuilder {
		public RichEditFloatingObjectSendBackwardSubmenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<RichEditCommandId> CreatePopupMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreatePopupMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		public override IDXPopupMenu<RichEditCommandId> CreateSubMenu() {
			IDXPopupMenu<RichEditCommandId> menu = base.CreateSubMenu() as IDXPopupMenu<RichEditCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		protected internal virtual void UpdateMenuState(IDXPopupMenu<RichEditCommandId> menu) {
			FloatingObjectSendBackwardMenuCommand command = new FloatingObjectSendBackwardMenuCommand(Control);
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			menu.Caption = command.MenuCaption;
			menu.Visible = state.Visible;
		}
		public override void PopulatePopupMenu(IDXPopupMenu<RichEditCommandId> menu) {
			InnerRichEditControl innerControl = Control.InnerControl;
			const string groupId = "0";
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.FloatingObjectSendToBack), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.FloatingObjectSendBackward), groupId);
			AddMenuCheckItem(menu, innerControl.CreateCommand(RichEditCommandId.FloatingObjectSendBehindText), groupId);
		}
	}
	#endregion
}
