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
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.Xpf.Spreadsheet.Menu;
using DevExpress.Xpf.Spreadsheet.Menu.Internal;
using DevExpress.XtraSpreadsheet.Menu;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DevExpress.XtraSpreadsheet.Layout;
namespace DevExpress.Xpf.Spreadsheet.Services {
	public interface ISpreadsheetContextMenuService {
		bool ShowContextMenu(SpreadsheetControl control);
		Bars.BarManagerMenuController GetMenuController(SpreadsheetControl control);
	}
	public class SpreadsheetContextMenuService : ISpreadsheetContextMenuService {
		#region Fields
		SpreadsheetPopupMenu menu;
		BarManagerMenuController menuController;
		#endregion
		public SpreadsheetContextMenuService(SpreadsheetControl control) {
			this.menu = new SpreadsheetPopupMenu(control);
			this.menu.Init();
			this.menuController = new BarManagerMenuController(Menu);
		}
		#region Properties
		SpreadsheetPopupMenu Menu { get { return menu; } }
		internal BarManagerMenuController MenuController { get { return menuController; } }
		#endregion
		void ShowContentContextMenu(SpreadsheetControl control) {
			Point mousePosition = GetMousePosition(control.ViewControl.WorksheetControl);
			SpreadsheetHitTestResult hitTestResult = control.InnerControl.ActiveView.CalculatePageHitTest(mousePosition.ToDrawingPoint());
			if (hitTestResult == null || !hitTestResult.IsValid(DocumentLayoutDetailsLevel.PageArea))
				return;
			ShowContextMenu(control, control.GetMenuType(hitTestResult));
		}
		Point GetMousePosition(WorksheetControl worksheet) {
			Point position = Mouse.GetPosition(worksheet);
			return worksheet.GetPointСonsideringScale(position);
		}
		void ShowTabSelectorContextMenu(SpreadsheetControl control) {
			ShowContextMenu(control, SpreadsheetMenuType.SheetTab);
		}
		internal void ShowAutoFilterContextMenu(SpreadsheetControl control) {
			ShowContextMenu(control, SpreadsheetMenuType.AutoFilter, true);
		}
		internal void ShowPivotTableAutoFilterContextMenu(SpreadsheetControl control) {
			ShowContextMenu(control, SpreadsheetMenuType.PivotTableAutoFilter, true);
		}
		void ShowContextMenu(SpreadsheetControl control, SpreadsheetMenuType menuType) {
			ShowContextMenu(control, menuType, false);
		}
		void ShowContextMenu(SpreadsheetControl control, SpreadsheetMenuType menuType, bool needLockTree) {
			Menu.MenuType = menuType;
			Menu.MenuBuilderInfo = CreateMenuBuilderInfo(control, menuType);
			Menu.PlacementTarget = control;
			Menu.Placement = PlacementMode.Relative;
			System.Windows.Point point;
			if (menuType != SpreadsheetMenuType.PivotTableAutoFilter)
				point = Mouse.GetPosition(control);
			else 
				point = control.DocumentModel.ActiveSheet.PivotTableStaticInfo.PointActivateFilter.ToPoint();
			Menu.HorizontalOffset = point.X;
			Menu.VerticalOffset = point.Y;
			ShowMenu(needLockTree);
			control.ViewControl.ClearMeasureCache();
		}
		SpreadsheetMenuBuilderInfo CreateMenuBuilderInfo(SpreadsheetControl control, SpreadsheetMenuType menuType) {
			SpreadsheetMenuBuilder builder;
			switch (menuType) {
				case SpreadsheetMenuType.AutoFilter:
					builder = new XpfSpreadsheetAutoFilterMenuBuilder(control, new XpfSpreadsheetMenuBuilderUIFactory());
					break;
				case SpreadsheetMenuType.PivotTable:
					builder = new XpfSpreadsheetPivotTableMenuBuilder(control, new XpfSpreadsheetMenuBuilderUIFactory());
					break;
				case SpreadsheetMenuType.PivotTableAutoFilter:
					builder = new XpfSpreadsheetPivotTableAutoFilterMenuBuilder(control, new XpfSpreadsheetMenuBuilderUIFactory());
					break;
				case SpreadsheetMenuType.SheetTab:
					builder = new XpfSpreadsheetTabSelectorMenuBuilder(control, new XpfSpreadsheetMenuBuilderUIFactory());
					break;
				default:
					builder = new XpfSpreadsheetContentMenuBuilder(control, new XpfSpreadsheetMenuBuilderUIFactory());
					break;
			}
			return (SpreadsheetMenuBuilderInfo)builder.CreatePopupMenu();
		}
		void ShowMenu(bool needLockTree) {
			if (!needLockTree) {
				Menu.IsOpen = true;
				return;
			}
			using (NavigationTree.Lock()) {
				Menu.IsOpen = true;
			}
		}
		#region ISpreadsheetContextMenuService Members
		bool ISpreadsheetContextMenuService.ShowContextMenu(SpreadsheetControl control) {
			SpreadsheetHitTestType hitTest = control.GetHitTest(Mouse.GetPosition(control));
			bool isShowing = false;
			if (hitTest == SpreadsheetHitTestType.Worksheet) {
				ShowContentContextMenu(control);
				isShowing = true;
			}
			else if (hitTest == SpreadsheetHitTestType.TabSelector) {
				ShowTabSelectorContextMenu(control);
				isShowing = true;
			}
			return isShowing;
		}
		Bars.BarManagerMenuController ISpreadsheetContextMenuService.GetMenuController(SpreadsheetControl control) {
			return MenuController;
		}
		#endregion
	}
}
