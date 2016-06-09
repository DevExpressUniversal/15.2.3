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
using System.Text;
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using System.Diagnostics;
using DevExpress.XtraReports.Design.Commands;
namespace DevExpress.XtraReports.Design {
	public interface IXRMenuCommandService : IMenuCommandServiceEx {
		void ShowContextMenu(XtraContextMenuBase menu, int x, int y);
	}
	public class XRMenuCommandServiceBase : IXRMenuCommandService {
		Dictionary<CommandID, MenuCommand> commands = new Dictionary<CommandID, MenuCommand>();
		public event EventHandler MenuCommandChanged;
		public void AddCommand(MenuCommand command) {
			Debug.Assert(command != null);
			Debug.Assert(command.CommandID != null);
			Debug.Assert(!commands.ContainsKey(command.CommandID));
			SubscribeEvents(command);
			commands.Add(command.CommandID, command);
			RaiseMenuCommandChanged(command);
		}
		public void RemoveCommand(MenuCommand command) {
			Debug.Assert(command != null);
			UnsubsciberEvents(command);
			commands.Remove(command.CommandID);
			RaiseMenuCommandChanged(command);
		}
		public virtual MenuCommand FindCommand(CommandID commandID) {
			MenuCommand menuCommand;
			return commandID != null && commands.TryGetValue(commandID, out menuCommand) ? menuCommand : null;
		}
		void UnsubsciberEvents(MenuCommand command) {
			command.CommandChanged -= new EventHandler(command_CommandChanged);
		}
		void SubscribeEvents(MenuCommand command) {
			command.CommandChanged += new EventHandler(command_CommandChanged);
		}
		void command_CommandChanged(object sender, EventArgs e) {
			RaiseMenuCommandChanged(sender);
		}
		void RaiseMenuCommandChanged(object sender) {
			if (MenuCommandChanged != null)
				MenuCommandChanged(sender, EventArgs.Empty);
		}
		public bool GlobalInvoke(CommandID commandID) {
			return GlobalInvoke(commandID, null);
		}
		public virtual bool GlobalInvoke(CommandID commandID, object[] args) {
			MenuCommand menuCommand = FindCommand(commandID);
			if (menuCommand != null) {
				MenuCommandHandlerBase.InvokeCommandEx(menuCommand, args);
				return true;
			}
			return false;
		}
		public virtual void ShowContextMenu(CommandID commandID, int x, int y) {
		}
		public virtual void ShowContextMenu(XtraContextMenuBase menu, int x, int y) {
		}
		public virtual DesignerVerbCollection Verbs {
			get { return null; }
		}
		public virtual void AddVerb(DesignerVerb verb) {
		}
		public virtual void RemoveVerb(DesignerVerb verb) {
		}
	}
}
