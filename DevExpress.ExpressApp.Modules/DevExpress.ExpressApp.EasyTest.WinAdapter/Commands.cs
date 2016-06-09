#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text.RegularExpressions;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter {
	public class DragDropCommand : Command {
		public DragDropCommand() {
			HasMainParameter = true;
		}
		protected override void InternalExecute(ICommandAdapter adapter) {
			ITestControl testControl = adapter.CreateTestControl(TestControlType.Table, Parameters.MainParameter.Value);
			IControlDragDrop testControlDragDrop = testControl.GetInterface<IControlDragDrop>();
			testControlDragDrop.DragDrop(Parameters[0].Name, Parameters[0].Value, Parameters[1].Name, Parameters[1].Value);
		}
	}
	public class WinCheckValidationResultCommand : CheckValidationResultCommand {
		private void CheckMessageField(ICommandAdapter adapter, string message) {
			string actualMessage = CheckFieldValuesCommand.GetFieldValue(adapter, "ValidationResultsText");
			if(!MultiLineComparisionHelper.CompareString(actualMessage, message)) {
				throw new CommandException(String.Format("Incorrect message value (expected value: '{0}', actual value: '{1}')", message, actualMessage), this.StartPosition);
			}
		}
		private void CheckInfo(ICommandAdapter adapter, string[] columns, string[,] info, bool skipUnexpectedErrors) {
			CheckTableCommand checkTableCommand = new CheckTableCommand();
			checkTableCommand.Parameters.Add(new Parameter(" Columns = " + string.Join(", ", columns), this.StartPosition));
			ITestControl tableControl = adapter.CreateTestControl(TestControlType.Table, "");
			int tableRowCount = tableControl.GetInterface<IGridBase>().GetRowCount();
			for(int i = 0; i < info.GetLength(0); i++) {
				string parameterLine = " Row = ";
				if(columns.Length == 1) {
					parameterLine += info[i, 0];
				}
				else {
					string targetInfoValue = info[i, 0];
					if(!info[i, 0].StartsWith("'") && !info[i, 0].EndsWith("'")) {
						targetInfoValue = string.Format(@"'{0}'", targetInfoValue);
					}
					parameterLine += string.Format(@"{0} (*), {1}", targetInfoValue, info[i, 1]);
				}
				checkTableCommand.Parameters.Add(new Parameter(parameterLine, this.StartPosition));
			}
			checkTableCommand.Execute(adapter);
			int validationInfoLength = info.GetLength(0);
			if(tableRowCount > validationInfoLength && !skipUnexpectedErrors) {
				throw new CommandException(string.Format("{0} unexpected validation error{1} occur", tableRowCount - validationInfoLength, tableRowCount - validationInfoLength == 1 ? "" : "s"), this.StartPosition);
			}
		}
		protected override void InternalCheckValidationResult(ICommandAdapter adapter, string message, string[] columns, string[,] info, bool skipUnexpectedErrors) {
			try {
				if(message != null) {
					CheckMessageField(adapter, message);
				}
				if(info != null) {
					CheckInfo(adapter, columns, info, skipUnexpectedErrors);
				}
			}
			catch(EasyTestException ex) {
				ex.PositionInScript = StartPosition;
				throw;
			}
		}
	}
	public abstract class WindowSizeCommand : Command {
		private const string ErrorMessage = "Invalid window size specified in the CheckActiveWindowSize command. Use 'WidthxHeight' pattern, for instance, '800x600'";
		private Size size;
		public WindowSizeCommand() {
			HasMainParameter = true;
		}
		public Size GetPointFromString(string parameterValue) {
			Size size = new Size();
			Regex regexp = new Regex(@"(\d+)x(\d+)", RegexOptions.IgnoreCase);
			Match match = regexp.Match(parameterValue);
			if(match.Success && match.Groups.Count == 3) {
				size = new Size(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
			}
			else {
				throw new ParserException(ErrorMessage, StartPosition);
			}
			return size;
		}
		public override void ParseCommand(CommandCreationParam commandCreationParam) {
			base.ParseCommand(commandCreationParam);
			size = GetPointFromString(Parameters.MainParameter.Value);
		}
		public Size Size {
			get {
				return size;
			}
		}
	}
	public abstract class WindowPositionCommand : Command {
		private const string ErrorMessage = "X and Y integer parameters are required.";
		private Point position;
		public WindowPositionCommand() {
			HasMainParameter = true;
		}
		public override void ParseCommand(CommandCreationParam commandCreationParam) {
			base.ParseCommand(commandCreationParam);
			Parameter xParameter = Parameters["X"];
			Parameter yParameter = Parameters["Y"];
			int x, y;
			if(xParameter == null || yParameter == null || !Int32.TryParse(xParameter.Value, out x) || !Int32.TryParse(xParameter.Value, out y)) {
				throw new ParserException(ErrorMessage);
			}
			position.X = x;
			position.Y = y;
		}
		protected Point Position {
			get {
				return position;
			}
		}
	}
	public class SetActiveWindowPositionCommand : WindowPositionCommand {
		protected override void InternalExecute(ICommandAdapter adapter) {
			ITestControl activeWindowControl = adapter.CreateTestControl(TestControlType.Dialog, null);
			activeWindowControl.GetInterface<ITestWindowEx>().SetWindowPosition(Position.X, Position.Y);
		}
	}
	public class CheckActiveWindowPositionCommand : WindowPositionCommand {
		protected override void InternalExecute(ICommandAdapter adapter) {
			ITestControl activeWindowControl = adapter.CreateTestControl(TestControlType.Dialog, null);
			activeWindowControl.GetInterface<ITestWindowEx>().CheckWindowPosition(Position.X, Position.Y);
		}
	}
	public class CheckActiveWindowSizeCommand : WindowSizeCommand {
		protected override void InternalExecute(ICommandAdapter adapter) {
			ITestControl activeWindowControl = adapter.CreateTestControl(TestControlType.Dialog, null);
			activeWindowControl.GetInterface<ITestWindowEx>().CheckWindowSize(Size.Width, Size.Height);
		}
	}
	public class AutoTestCommand : Command {
		private const string logonCaption = "Log On";
		private string[] navigationControlPossibleNames = new string[] { "ViewsNavigation.Navigation", "Navigation" };
		private ITestControl GetNavigationTestControl(ICommandAdapter adapter) {
			string controlNames = "";
			for(int i = 0; i < navigationControlPossibleNames.Length; i++) {
				if(adapter.IsControlExist(TestControlType.Action, navigationControlPossibleNames[i])) {
					try {
						ITestControl testControl = adapter.CreateTestControl(TestControlType.Action, navigationControlPossibleNames[i]);
						IGridBase gridBaseInterface = testControl.GetInterface<IGridBase>();
						int itemsCount = gridBaseInterface.GetRowCount();
						if(itemsCount > 0) {
							return testControl;
						}
					}
					catch(WarningException) { }
				}
				controlNames += (i <= navigationControlPossibleNames.Length) ? navigationControlPossibleNames[i] + " or " : navigationControlPossibleNames[i];
			}
			throw new WarningException(string.Format("Cannot find the '{0}' control", controlNames));
		}
		protected override void InternalExecute(ICommandAdapter adapter) {
			if(adapter.IsControlExist(TestControlType.Action, logonCaption)) {
				adapter.CreateTestControl(TestControlType.Action, logonCaption).GetInterface<IControlAct>().Act(null);
			}
			int itemsCount = GetNavigationTestControl(adapter).GetInterface<IGridBase>().GetRowCount();
			for(int i = 0; i < itemsCount; i++) {
				ITestControl testControl = GetNavigationTestControl(adapter);
				IControlAct controlAct = testControl.GetInterface<IControlAct>();
				IGridBase gridBase = testControl.GetInterface<IGridBase>();
				string navigationItemName = gridBase.GetCellValue(i, DevExpress.ExpressApp.Utils.Enumerator.GetFirst<IGridColumn>(gridBase.Columns));
				controlAct.Act(navigationItemName);
				if(adapter.IsControlExist(TestControlType.Action, "New")) {
					try {
						adapter.CreateTestControl(TestControlType.Action, "New").FindInterface<IControlAct>().Act("");
					}
					catch(Exception e) {
						throw new CommandException(string.Format("The 'New' action execution failed. Navigation item: {0}\r\nInner Exception: {1}", navigationItemName, e.Message), this.StartPosition);
					}
					if(adapter.IsControlExist(TestControlType.Action, "Cancel")) {
						try {
							ITestControl cancelActionTestControl = adapter.CreateTestControl(TestControlType.Action, "Cancel");
							if(cancelActionTestControl.GetInterface<IControlEnabled>().Enabled) {
								cancelActionTestControl.FindInterface<IControlAct>().Act(null);
							}
						}
						catch(Exception e) {
							throw new CommandException(string.Format("The 'Cancel' action execution failed. Navigation item: {0}\r\nInner Exception: {1}", navigationItemName, e.Message), this.StartPosition);
						}
					}
					OptionalActionCommand command = new OptionalActionCommand();
					command.Parameters.MainParameter = new MainParameter("Yes");
					command.Parameters.ExtraParameter = new MainParameter();
					command.Execute(adapter);
				}
			}
		}
	}
	public class CloseActiveWindowCommand : Command {
		protected override void InternalExecute(ICommandAdapter adapter) {
			ITestControl activeWindowControl = adapter.CreateTestControl(TestControlType.Dialog, null);
			activeWindowControl.GetInterface<ITestWindow>().Close();
		}
	}
}
