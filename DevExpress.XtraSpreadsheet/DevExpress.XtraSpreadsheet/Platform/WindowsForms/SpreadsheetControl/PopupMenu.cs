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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraSpreadsheet.Menu;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Forms;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl {
		[System.Security.SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		protected internal virtual bool OnWmContextMenu(ref Message m) {
			this.Focus();
			Point screenMousePosition = new Point((short)((int)m.LParam), ((int)m.LParam) >> 0x10);
			bool isKeyboardContextMenu = screenMousePosition == new Point(-1, -1);
			Point point = CalculatePopupMenuPosition(screenMousePosition);
			if (isKeyboardContextMenu)
				return OnPopupMenu(point); ;
			return HandleMouseContextMenu(point);
		}
		protected internal bool HandleMouseContextMenu(Point point) {
			if (!ClientBounds.Contains(point))
				return false;
			bool allowShowMenu = !InnerControl.MouseHandler.OnPopupMenu(new MouseEventArgs(MouseButtons.Right, 1, point.X, point.Y, 0));
			if (allowShowMenu)
				return OnPopupMenu(point);
			return true;
		}
		protected internal virtual Point CalculatePopupMenuPosition(Point screenMousePosition) {
			if (screenMousePosition.X == -1) {
				CellPosition cellPosition = DocumentModel.ActiveSheet.Selection.ActiveCell;
				int modelColumnIndex = cellPosition.Column;
				int modelRowIndex = cellPosition.Row;
				Page page = InnerControl.DesignDocumentLayout.GetExactOrNearPageByModelIndexes(modelColumnIndex, modelRowIndex);
				if (page == null)
					return screenMousePosition;
				int columnIndex = GetActualIndex(page.GridColumns, modelColumnIndex);
				int x = page.GridColumns[columnIndex].Far + 1;
				float zoomFactor = ActiveView.ZoomFactor;
				x = (int)Math.Round(x * zoomFactor);
				int rowIndex = GetActualIndex(page.GridRows, modelRowIndex);
				int y = page.GridRows[rowIndex].Far + 1;
				y = (int)Math.Round(y * zoomFactor);
				return new Point(x, y);
			}
			else
				return PointToClient(screenMousePosition);
		}
		int GetActualIndex(PageGrid grid, int modelIndex) {
			int index = grid.TryCalculateIndex(modelIndex);
			if (index != -1)
				return index;
			PageGridItem actualFirst = grid.ActualFirst;
			if (modelIndex < actualFirst.ModelIndex)
				return actualFirst.Index;
			else
				return grid.ActualLast.Index;
		}
		protected internal virtual bool OnPopupMenu(Point point) {
			SpreadsheetHitTestResult hitTestResult = InnerControl.ActiveView.CalculatePageHitTest(point);
			if (hitTestResult == null || !hitTestResult.IsValid(DocumentLayoutDetailsLevel.PageArea))
				return false;
			if (hitTestResult.CommentBox != null) 
				return false;
			return ShowPopupMenu(GetMenuType(hitTestResult), point);
		}
		internal bool ShowPopupMenu(SpreadsheetMenuType menuType, Point location) {
			if (!Options.Behavior.ShowPopupMenuAllowed)
				return false;
			SpreadsheetMenuBuilder builder = CreateMenuBuilder(menuType);
			SpreadsheetPopupMenu menu = (SpreadsheetPopupMenu)builder.CreatePopupMenu();
			menu = RaisePopupMenuShowing(menu, menuType);
			if (menu == null || menu.Items.Count <= 0)
				return false;
			MenuManagerHelper.ShowMenu(menu, LookAndFeel, MenuManager, this, location);
			return true;
		}
		SpreadsheetMenuBuilder CreateMenuBuilder(SpreadsheetMenuType menuType) {
			switch (menuType) {
				case SpreadsheetMenuType.AutoFilter:
					return new SpreadsheetAutoFilterMenuBuilder(this, new WinFormsSpreadsheetMenuBuilderUIFactory());
				case SpreadsheetMenuType.PivotTable:
					return new WinFormsSpreadsheetPivotTableMenuBuilder(this, new WinFormsSpreadsheetMenuBuilderUIFactory());
				case SpreadsheetMenuType.PivotTableAutoFilter:
					return new SpreadsheetPivotTableAutoFilterMenuBuilder(this, new WinFormsSpreadsheetMenuBuilderUIFactory());
				case SpreadsheetMenuType.SheetTab:
					return new WinFormsSpreadsheetTabSelectorMenuBuilder(this, new WinFormsSpreadsheetMenuBuilderUIFactory());
				default:
					return new WinFormsSpreadsheetContentMenuBuilder(this, new WinFormsSpreadsheetMenuBuilderUIFactory());
			}
		}
		protected internal virtual bool ShowAutoFilterPopupMenu(AutoFilterViewModel viewModel) {
			return ShowPopupMenu(SpreadsheetMenuType.AutoFilter, PointToClient(Control.MousePosition));
		}
		protected internal virtual bool ShowPivotTableAutoFilterPopupMenu() {
			PivotTableStaticInfo info = this.DocumentModel.ActiveSheet.PivotTableStaticInfo;
			return ShowPopupMenu(SpreadsheetMenuType.PivotTableAutoFilter, info.PointActivateFilter);
		}
	}
}
namespace DevExpress.XtraSpreadsheet.Menu {
	#region WinFormsSpreadsheetMenuBuilderUIFactory
	public class WinFormsSpreadsheetMenuBuilderUIFactory : IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> {
		#region IMenuBuilderUIFactory<SpreadsheetCommand,SpreadsheetCommandId> Members
		public IDXMenuCheckItemCommandAdapter<SpreadsheetCommandId> CreateMenuCheckItemAdapter(SpreadsheetCommand command) {
			return new SpreadsheetMenuCheckItemCommandWinAdapter(command);
		}
		public IDXMenuItemCommandAdapter<SpreadsheetCommandId> CreateMenuItemAdapter(SpreadsheetCommand command) {
			return new SpreadsheetMenuItemCommandWinAdapter(command);
		}
		public IDXPopupMenu<SpreadsheetCommandId> CreatePopupMenu() {
			return new SpreadsheetPopupMenu();
		}
		public IDXPopupMenu<SpreadsheetCommandId> CreateSubMenu() {
			return new SpreadsheetPopupMenu();
		}
		#endregion
	}
	#endregion
	#region WinFormsSpreadsheetContentMenuBuilder
	public class WinFormsSpreadsheetContentMenuBuilder : SpreadsheetContentMenuBuilder {
		public WinFormsSpreadsheetContentMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
	}
	#endregion
	#region WinFormsSpreadsheetPivotTableMenuBuilder
	public class WinFormsSpreadsheetPivotTableMenuBuilder : SpreadsheetPivotTableMenuBuilder {
		public WinFormsSpreadsheetPivotTableMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
	}
	#endregion
	#region SpreadsheetMenuItem
	public class SpreadsheetMenuItem : CommandMenuItem<SpreadsheetCommandId> {
		public SpreadsheetMenuItem() : this(String.Empty) { }
		public SpreadsheetMenuItem(string caption) : this(caption, null) { }
		public SpreadsheetMenuItem(string caption, EventHandler click) : this(caption, click, null) { }
		public SpreadsheetMenuItem(string caption, EventHandler click, Image image) : this(caption, click, image, null) { }
		public SpreadsheetMenuItem(string caption, EventHandler click, Image image, EventHandler update)
			: base(caption, click, image, update) {
			this.Id = SpreadsheetCommandId.None;
		}
	}
	#endregion
	#region SpreadsheetMenuCheckItem
	public class SpreadsheetMenuCheckItem : CommandMenuCheckItem<SpreadsheetCommandId> {
		public SpreadsheetMenuCheckItem() : this(String.Empty) { }
		public SpreadsheetMenuCheckItem(string caption) : this(caption, false) { }
		public SpreadsheetMenuCheckItem(string caption, bool check, Image image, EventHandler checkedChanged) : this(caption, check, image, checkedChanged, null) { }
		public SpreadsheetMenuCheckItem(string caption, bool check) : this(caption, check, null) { }
		public SpreadsheetMenuCheckItem(string caption, bool check, Image image, EventHandler checkedChanged, EventHandler update)
			: base(caption, check, image, checkedChanged, update) {
			this.Id = SpreadsheetCommandId.None;
		}
		public SpreadsheetMenuCheckItem(string caption, bool check, EventHandler update)
			: base(caption, check, update) {
			this.Id = SpreadsheetCommandId.None;
		}
	}
	#endregion
	#region SpreadsheetPopupMenu
	public class SpreadsheetPopupMenu : CommandPopupMenu<SpreadsheetCommandId> {
		public SpreadsheetPopupMenu(EventHandler beforePopup)
			: base(beforePopup) {
			this.Id = SpreadsheetCommandId.None;
		}
		public SpreadsheetPopupMenu()
			: base() {
			this.Id = SpreadsheetCommandId.None;
		}
	}
	#endregion
	#region SpreadsheetMenuItemCommandWinAdapter
	public class SpreadsheetMenuItemCommandWinAdapter : DXMenuItemCommandAdapter<SpreadsheetCommandId> {
		public SpreadsheetMenuItemCommandWinAdapter(SpreadsheetCommand command)
			: base(command) {
		}
		public override IDXMenuItem<SpreadsheetCommandId> CreateMenuItem(DXMenuItemPriority priority) {
			SpreadsheetMenuItem item = new SpreadsheetMenuItem(Command.MenuCaption, new EventHandler(this.OnClick), Command.Image, new EventHandler(this.OnUpdate));
			SpreadsheetCommand command = (SpreadsheetCommand)Command;
			item.Id = command.Id;
			item.Priority = priority;
			return item;
		}
		public override void OnClick(object sender, EventArgs e) {
			try {
				base.OnClick(sender, e);
			}
			catch (Exception exception) {
				SpreadsheetCommand spreadsheetCommand = (SpreadsheetCommand)Command;
				SpreadsheetControl control = spreadsheetCommand.Control as SpreadsheetControl;
				if (control == null || !control.HandleException(exception))
					throw;
			}
		}
	}
	#endregion
	#region SpreadsheetMenuCheckItemCommandWinAdapter
	public class SpreadsheetMenuCheckItemCommandWinAdapter : DXMenuCheckItemCommandAdapter<SpreadsheetCommandId> {
		public SpreadsheetMenuCheckItemCommandWinAdapter(SpreadsheetCommand command)
			: base(command) {
		}
		public override void OnCheckedChanged(object sender, EventArgs e) {
			try {
				base.OnCheckedChanged(sender, e);
			}
			catch (Exception exception) {
				SpreadsheetCommand spreadsheetCommand = (SpreadsheetCommand)Command;
				SpreadsheetControl control = spreadsheetCommand.Control as SpreadsheetControl;
				if (control == null || !control.HandleException(exception))
					throw;
			}
		}
		public override IDXMenuCheckItem<SpreadsheetCommandId> CreateMenuItem(string groupId) {
			SpreadsheetMenuCheckItem item = new SpreadsheetMenuCheckItem(Command.MenuCaption, false, Command.Image, new EventHandler(this.OnCheckedChanged), new EventHandler(this.OnUpdate));
			SpreadsheetCommand command = (SpreadsheetCommand)Command;
			item.Id = command.Id;
			return item;
		}
	}
	#endregion
}
