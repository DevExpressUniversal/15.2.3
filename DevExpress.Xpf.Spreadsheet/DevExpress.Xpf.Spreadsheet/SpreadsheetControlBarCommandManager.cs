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
using System.Windows.Input;
using DevExpress.Office.Internal;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Spreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Xpf.Spreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region SpreadsheetControlBarCommandManager
	public class SpreadsheetControlBarCommandManager : ControlBarCommandManager<SpreadsheetControl, SpreadsheetCommand, SpreadsheetCommandId> {
		public SpreadsheetControlBarCommandManager(SpreadsheetControl control)
			: base(control) {
		}
		protected override object ControlAccessor { get { return Control.Accessor; } }
		protected override BarManager BarManager { get { return Control.BarManager; } }
		protected override RibbonControl Ribbon { get { return Control.Ribbon; } }
		protected override SpreadsheetCommandId EmptyCommandId { get { return SpreadsheetCommandId.None; } }
		protected override SpreadsheetCommandId GetCommandId(ICommand command) {
			SpreadsheetUICommand commandUi = command as SpreadsheetUICommand;
			if (commandUi == null)
				return SpreadsheetCommandId.None;
			return commandUi.CommandId;
		}
		protected override void SetFocus() {
			Control.SetFocus();
		}
		protected override bool IsControlProvider(object value) {
			return value is SpreadsheetControlProvider;
		}
		protected override BarItemCommandUIState CreateBarItemUIState(BarItem item) {
			return new SpreadsheetBarItemCommandUIState(item);
		}
		protected internal override void ExecuteParametrizedCommand(DevExpress.Utils.Commands.Command command, object parameter) {
			SpreadsheetViewControl viewControl = Control.ViewControl;
			if (viewControl != null)
				viewControl.ClearMeasureCache();
			base.ExecuteParametrizedCommand(command, parameter);
		}
	}
	#endregion
}
