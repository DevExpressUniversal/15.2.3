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
namespace DevExpress.XtraPrinting.Native {
	public class CommandSetItem {
		EventHandler onCommandVisibilityChanged;
		PrintingSystemCommand command;
		bool enabled;
		CommandVisibility visibility;
		Priority visPriority;
		public PrintingSystemCommand Command {
			get { return command; }
		}
		public virtual bool Enabled {
			get { return enabled; }
			set { enabled = value; }
		}
		public CommandVisibility Visibility {
			get { return visibility; }
		}
		internal bool IsLowPriority { get { return visPriority == Priority.Low; } }
		internal void SetVisibility(CommandVisibility visibility, Priority visPriority) {
			if(this.visPriority <= visPriority) {
				if(this.visibility != visibility) OnCommandVisibilityChanged();
				this.visibility = visibility;
				this.visPriority = visPriority;
			}
		}
		public CommandSetItem(PrintingSystemCommand command, EventHandler onCommandVisibilityChanged) {
			this.onCommandVisibilityChanged = onCommandVisibilityChanged;
			this.command = command;
			this.visibility = CommandVisibility.All;
			this.visPriority = Priority.Low;
		}
		protected CommandSetItem(CommandSetItem source, EventHandler onCommandVisibilityChanged) {
			this.onCommandVisibilityChanged = onCommandVisibilityChanged;
			this.command = source.command;
			this.visibility = source.visibility;
			this.visPriority = source.visPriority;
			this.enabled = source.enabled;
		}
		public virtual CommandSetItem Clone(EventHandler onCommandVisibilityChanged) {
			return new CommandSetItem(this, onCommandVisibilityChanged);
		}
		void OnCommandVisibilityChanged() {
			if(onCommandVisibilityChanged != null) onCommandVisibilityChanged(this, EventArgs.Empty);
		}
	}
}
