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
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Commands {
	public abstract class HistoryCommand : DashboardCommand {
		public override string Description {
			get {
				IEnumerator<IHistoryItem> enumerator = HistoryItems.GetEnumerator();
				if(enumerator.MoveNext())
					return String.Format(base.Description, enumerator.Current.Caption);
				else
					return MenuCaption;
			}
		}
		protected abstract IEnumerable<IHistoryItem> HistoryItems { get; }
		protected HistoryCommand(DashboardDesigner control) : base(control) {
		}
	}
	public static class HistoryCommandExecutor { 
		public static void ExecuteHistoryCommandSet(DashboardDesigner designer, bool isRedo, int operationsCount){
			IDashboardLayoutUpdateService layoutUpdateService = ((IServiceProvider)designer).RequestServiceStrictly<IDashboardLayoutUpdateService>();
			layoutUpdateService.LockLayoutControlUpdate();
			try {
				DashboardCommand command = isRedo ? (DashboardCommand)new RedoCommand(designer) : (DashboardCommand)new UndoCommand(designer);
				for(int i = 0; i <= operationsCount; i++)
					command.Execute();
			} finally {
				layoutUpdateService.UnlockLayoutControlUpdate(false);
			}
		}
	}
}
