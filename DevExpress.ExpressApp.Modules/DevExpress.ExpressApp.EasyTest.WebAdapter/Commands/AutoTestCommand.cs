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
using System.Collections.Generic;
using System.Text;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.Commands {
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
		private void TryClosePopupWindow(ICommandAdapter adapter) {
			OptionalActionCommand command = new OptionalActionCommand();
			command.Parameters.MainParameter = new MainParameter("Close");
			command.Parameters.ExtraParameter = new MainParameter();
			command.Execute(adapter);
		}
		protected override void InternalExecute(ICommandAdapter adapter) {
			if(adapter.IsControlExist(TestControlType.Action, logonCaption)) {
				adapter.CreateTestControl(TestControlType.Action, logonCaption).GetInterface<IControlAct>().Act(null);
			}
			int itemsCount = GetNavigationTestControl(adapter).GetInterface<IGridBase>().GetRowCount();
			for(int i = 0; i < itemsCount; i++) {
				ITestControl testControl;
				try {
					testControl = GetNavigationTestControl(adapter);
				}
				catch(WarningException) {
					TryClosePopupWindow(adapter);
					testControl = GetNavigationTestControl(adapter);
				}
				IGridBase gridBase = testControl.GetInterface<IGridBase>();
				string navigationItemName = gridBase.GetCellValue(i, new List<IGridColumn>(gridBase.Columns)[0]); ;
				if(!string.IsNullOrEmpty(navigationItemName)) {
					EasyTestTracer.Tracer.LogText(string.Format("EnumerateNavigationItem '{0}'", navigationItemName));
					IControlActionItems controlActionItems = testControl.FindInterface<IControlActionItems>();
					if(controlActionItems != null && controlActionItems.IsEnabled(navigationItemName)) {
						try {
							testControl.GetInterface<IControlAct>().Act(navigationItemName);
						}
						catch(Exception e) {
							throw new CommandException(string.Format("The 'Navigation' action execution failed. Navigation item: {0}\r\nInner Exception: {1}", navigationItemName, e.Message), this.StartPosition);
						}
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
		}
	}
}
