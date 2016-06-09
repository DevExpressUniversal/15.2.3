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
using System.Collections;
namespace DevExpress.XtraPrinting.Native {
	public class CommandSet : CommandSetBase {
		protected Hashtable commandHT = new Hashtable();
		public CommandSetItem this[PrintingSystemCommand command] {
			get { return (CommandSetItem)commandHT[command]; }
		}
		public CommandSet() {
			foreach(PrintingSystemCommand command in PSCommandHelper.GetCommands())
				Add(new CommandSetItem(command, OnCommandVisibilityChanged));
		}
		public void CopyFrom(CommandSet source) {
			commandHT.Clear();
			foreach(CommandSetItem item in source.GetAllCommands()) {
				Add(item.Clone(OnCommandVisibilityChanged));
			}
		}
		public CommandSetItem[] GetAllCommands() {
			return (CommandSetItem[])new ArrayList(commandHT.Values).ToArray(typeof(CommandSetItem));
		}
		public void Add(CommandSetItem item) {
			if(!commandHT.Contains(item.Command)) {
				commandHT[item.Command] = item;
			}
		}
		public void Remove(CommandSetItem item) {
			commandHT.Remove(item);
		}
		public void Clear() {
			commandHT.Clear();
		}
		public void EnableAllCommands(bool value) {
			foreach(CommandSetItem item in commandHT.Values)
				EnableCommandCore(value, item);
		}
		protected override void EnableCommand(bool value, PrintingSystemCommand command) {
			EnableCommandCore(value, this[command]);
		}
		private void EnableCommandCore(bool value, CommandSetItem item) {
			if(item != null && item.Enabled != value) {
				item.Enabled = value;
				Dirty = true;
			}
		}
		protected override bool ContainsCommand(PrintingSystemCommand command) {
			return commandHT.Contains(command);
		}
		void OnCommandVisibilityChanged(object sender, EventArgs e) {
			Dirty = true;
			fCommandVisibilityChanged = true;
		}
		protected override void SetCommandVisibilityCore(PrintingSystemCommand command, CommandVisibility visibility, Priority priority, PrintingSystemBase printingSystem) {
			if(ContainsCommand(command))
				this[command].SetVisibility(visibility, priority);
		}
		internal bool GetCommandVisibility(PrintingSystemCommand command, CommandVisibility visibility) {
			return (GetCommandVisibility(command) & visibility) > 0;
		}
		internal CommandVisibility GetCommandVisibility(PrintingSystemCommand command) {
			return ContainsCommand(command) ?
				this[command].Visibility : CommandVisibility.None;
		}
		internal bool IsCommandEnabled(PrintingSystemCommand command) {
			return ContainsCommand(command) ?
				this[command].Enabled : false;			
		}
		public override bool HasEnabledCommand(PrintingSystemCommand[] commands) {
			foreach(PrintingSystemCommand command in commands) {
				CommandSetItem item = this[command];
				if(item != null && item.Enabled && item.Visibility != CommandVisibility.None)
					return true;
			}
			return false;
		}
	}
}
