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
using DevExpress.Xpf.Editors;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.DateNavigator;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Editors.DateNavigator.Controls;
namespace DevExpress.Xpf.Editors.DateNavigator {
	public class SingleSelectionNavigationStrategy : NavigationStrategyBase {
		public SingleSelectionNavigationStrategy(DateNavigator navigator)
			: base(navigator) {
				SetSelectedDates();
		}
		 void SetSelectedDates() {
			Navigator.SetSelectedDates(new List<DateTime>() { Navigator.FocusedDate });			
		}
		public override void CheckSelectedDates() {
			SetSelectedDates();
			throw new Exception(EditorLocalizer.GetString(EditorStringId.CantModifySelectedDates));
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			base.ProcessKeyDown(e);
			if (e.Handled)
				return;
			switch (e.Key) {
				case Key.Left:
					e.Handled = MoveLeftRight(true);
					break;
				case Key.Right:
					e.Handled = MoveLeftRight(false);
					break;
				case Key.Up:
					e.Handled = MoveUp();
					break;
				case Key.Down:
					e.Handled = MoveDown();
					break;
			}
		}
		public override bool MoveLeft() {
			return Move(ViewSpecific.MoveLeft(SelectionManager.FocusedDate));
		}
		public override bool MoveRight() {
			return Move(ViewSpecific.MoveRight(SelectionManager.FocusedDate));
		}
		public override bool MoveDown() {
			return Move(ViewSpecific.MoveDown(SelectionManager.FocusedDate));
		}
		public override bool MoveUp() {
			return Move(ViewSpecific.MoveUp(SelectionManager.FocusedDate));
		}
		public override bool Move(DateTime dateTime) {
			bool result = base.Move(dateTime);
			result &= Select(dateTime, true);
			return result;
		}
		public override void ProcessMouseDown(DateTime buttonDate, DateNavigatorCalendarButtonKind buttonKind) {
			if (buttonKind == DateNavigatorCalendarButtonKind.WeekNumber) return;
			if (Navigator.CalendarView == DateNavigatorCalendarView.Month) {
				DateTime focused = buttonDate;
				Move(focused);
				Select(focused, true);
			}
			else
				base.ProcessMouseDown(buttonDate, buttonKind);
		}
	}
}
