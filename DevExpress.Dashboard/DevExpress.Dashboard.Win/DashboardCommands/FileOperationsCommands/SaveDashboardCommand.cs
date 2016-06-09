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
using DevExpress.Utils.Commands;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Commands {
	public class SaveDashboardCommand : DashboardCommand { 
		public override DashboardCommandId Id { get { return DashboardCommandId.SaveDashboard; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandSaveDashboardCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandSaveDashboardDescription; } }
		public override string ImageName { get { return "SaveDashboard"; } }
		protected virtual DashboardSaveCommand CommandType { get { return DashboardSaveCommand.Save; } }
		public SaveDashboardCommand(DashboardDesigner control)
			: base(control) {
		}
		public bool SaveDashboard(string fileName) {
			return SaveDashboardInternal(fileName);
		}
		protected bool SaveDashboardInternal(string fileName) {
			DashboardSavingEventArgs args = new DashboardSavingEventArgs(Control.Dashboard, CommandType);
			Control.RaiseDashboardSaving(args);
			if(args.Handled) {
				Control.History.IsModified = !args.Saved;
				if(!args.Saved)
					return false;
			}
			else {
				string oldFileName = Control.DashboardFileName;
				if(Control.SaveDashboard(fileName)) {
					Control.RaiseDashboardSaved(new DashboardSavedEventArgs(Control.Dashboard, oldFileName, Control.DashboardFileName));
					return true;
				}
				return false;
			}
			return true;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			SaveDashboardInternal(Control.DashboardFileName);
		}
	}
}
