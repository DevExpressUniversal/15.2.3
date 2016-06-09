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
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.Design {
	public class XRMenuCommandService : ReplaceableService<IMenuCommandService>, IXRMenuCommandService {
		readonly Control panel;
		FieldDropMenu fieldDropMenu;
		SelectionMenu selectionMenu, reportExplorerMenu;
		public IMenuCommandService VSMenuCommandService {
			get { return OriginalService; }
		}
		public XRMenuCommandService(IMenuCommandService serv, Control panel, IDesignerHost host) :
			base(serv, host) {
			this.panel = panel;
		}
		#region IMenuCommandServiceEx implementation
		DesignerVerbCollection IMenuCommandService.Verbs {
			get { return VSMenuCommandService.Verbs; }
		}
		void IMenuCommandService.AddCommand(MenuCommand command) {
			if(VSMenuCommandService.FindCommand(command.CommandID) == null)
				VSMenuCommandService.AddCommand(command);
		}
		void IMenuCommandService.AddVerb(DesignerVerb designerVerb) {
			VSMenuCommandService.AddVerb(designerVerb);
		}
		MenuCommand IMenuCommandService.FindCommand(CommandID commandID) {
			return VSMenuCommandService.FindCommand(commandID);
		}
		bool IMenuCommandService.GlobalInvoke(CommandID commandID) {
			return VSMenuCommandService.GlobalInvoke(commandID);
		}
		bool IMenuCommandServiceEx.GlobalInvoke(CommandID commandID, object[] args) {
			MenuCommand command = VSMenuCommandService.FindCommand(commandID);
			if(command != null) {
				MenuCommandHandler.InvokeCommandEx(command, args);
				return true;
			}
			return VSMenuCommandService.GlobalInvoke(commandID);
		}
		event EventHandler IMenuCommandServiceEx.MenuCommandChanged {
			add { throw new NotSupportedException(); }
			remove { throw new NotSupportedException(); }
		}
		void IMenuCommandService.RemoveCommand(MenuCommand menuCommand) {
			VSMenuCommandService.RemoveCommand(menuCommand);
		}
		void IMenuCommandService.RemoveVerb(DesignerVerb designerVerb) {
			VSMenuCommandService.RemoveVerb(designerVerb);
		}
		void IMenuCommandService.ShowContextMenu(CommandID commandID, int x, int y) {
			 if(commandID == MenuCommandServiceCommands.SelectionMenu) {
				if(selectionMenu == null)
					selectionMenu = new SelectionMenu(DesignerHost, MenuKind.Selection);
				ShowContextMenu(selectionMenu, x, y);
			} else if(commandID == MenuCommandServiceCommands.ReportExplorerMenu) {
				if(reportExplorerMenu == null)
					reportExplorerMenu = new SelectionMenu(DesignerHost, MenuKind.ReportExplorer);
				ShowContextMenu(reportExplorerMenu, x, y);
			} else if(commandID == MenuCommandServiceCommands.FieldDropMenu) {
				if(fieldDropMenu == null)
					fieldDropMenu = new FieldDropMenu(DesignerHost);
				ShowContextMenu(fieldDropMenu, x, y);
			}
			else
				VSMenuCommandService.ShowContextMenu(commandID, x, y);
		}
		#endregion
		public void ShowContextMenu(XtraContextMenuBase menu, int x, int y) {
			if(menu != null) {
				menu.Show(panel, new Point(x, y), DesignerHost);
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(selectionMenu != null)
					selectionMenu = null;
				if(reportExplorerMenu != null)
					reportExplorerMenu = null;
				if(fieldDropMenu != null)
					fieldDropMenu = null;
			}
			base.Dispose(disposing);
		}
	}
}
