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

using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils.Commands;
namespace DevExpress.DashboardWin.Commands {
	public class OpenDashboardCommand : DashboardCommand { 
		public override DashboardCommandId Id { get { return DashboardCommandId.OpenDashboard; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandOpenDashboardCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandOpenDashboardDescription; } }
		public override string ImageName { get { return "OpenDashboard"; } }
		public OpenDashboardCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			OpenDashboardInternal(null, null);
		}
		protected void OpenDashboardInternal(string filePath, string directoryPath) {
			DashboardDesigner designer = Control;
			if(designer.HandleDashboardClosing()) {
				DashboardOpeningEventArgs args = new DashboardOpeningEventArgs();
				designer.RaiseDashboardOpening(args);
				if(args.Handled) {
					if(args.Dashboard != null)
						designer.Dashboard = args.Dashboard;
				}
				else
					designer.OpenDashboard(filePath, directoryPath);
			}
		}
	}
	public class OpenDashboardPathCommand : OpenDashboardCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.OpenDashboardPath; } }
		public override string ImageName { get { return ""; } }
		public OpenDashboardPathCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			OpenDashboardPathCommandUIState uiState = state as OpenDashboardPathCommandUIState;
			if(uiState != null)
				OpenDashboardInternal(uiState.FilePath, uiState.DirectoryPath);
		}
	}
}
namespace DevExpress.DashboardWin.Native {
	public class OpenDashboardPathCommandUIState : ICommandUIState {
		public bool Enabled { get { return true; } set { } }
		public bool Checked { get { return false; } set { } }
		public object EditValue { get { return null; } set { } }
		public bool Visible { get { return true; } set { } }
		public string FilePath { get; private set; }
		public string DirectoryPath { get; private set; }
		public OpenDashboardPathCommandUIState(string filePath, string directoryPath) {
			FilePath = filePath;
			DirectoryPath = directoryPath;
		}
	}
}
