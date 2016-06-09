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
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.XtraReports.Design;
namespace DevExpress.XtraReports.UserDesigner {
	public enum MenuKind { Selection, FieldDrop, BandCollection, FieldList, ReportExplorer }
	public interface IMenuCreationService {
		void ProcessMenuItems(MenuKind menuKind, MenuItemDescriptionCollection items);
		MenuCommandDescription[] GetCustomMenuCommands();
	}
}
namespace DevExpress.XtraReports.Design {
	using DevExpress.XtraReports.UserDesigner;
	using DevExpress.XtraReports.Design.Commands;
	public class MenuCommandDescription {
		public EventHandler<CommandExecuteEventArgs> OnExecute { get; private set; }
		public EventHandler OnStatusRequire { get; private set; }
		public CommandID CommandID { get; private set; }
		public MenuCommandDescription(CommandID commandID, EventHandler<CommandExecuteEventArgs> onExecute, EventHandler onStatusRequire) {
			OnExecute = onExecute;
			OnStatusRequire = onStatusRequire;
			CommandID = commandID;
		}
	}
	public static class MenuItemDescriptionCollectionExtensions {
		public static void Remove(this MenuItemDescriptionCollection items, ReportCommand reportCommand) {
			CommandID commandID = CommandIDReportCommandConverter.GetCommandID(reportCommand);
			items.Remove(commandID);
		}
		public static bool Contains(this MenuItemDescriptionCollection items, ReportCommand reportCommand) {
			CommandID commandID = CommandIDReportCommandConverter.GetCommandID(reportCommand);
			return items.Contains(commandID);
		}
		public static int IndexOf(this MenuItemDescriptionCollection items, ReportCommand reportCommand) {
			CommandID commandID = CommandIDReportCommandConverter.GetCommandID(reportCommand);
			return items.IndexOf(commandID);
		}
	}
}
