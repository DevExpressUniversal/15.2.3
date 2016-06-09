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
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands.Internal;
namespace DevExpress.XtraDiagram.Bars {
	public abstract class DiagramCommandCheckDropDownButtonItem : DiagramCommandDropDownButtonItem {
		public DiagramCommandCheckDropDownButtonItem() {
			RememberLastCommand = true;
			ButtonStyle = BarButtonStyle.CheckDropDown;
		}
		protected override void InvokeCommand() {
			try {
				if(Control == null) return;
				Command command = CreateCommand();
				if(command != null) {
					ICommandUIState state = CreateCommandUIState(command);
					if(command.CanExecute()) command.ForceExecute(state);
				}
				UpdateItemVisibility();
				Control.Focus();
			}
			catch(Exception e) {
				if(!HandleException(e)) throw;
			}
		}
		protected override BarItemLink GetDefaultLastClickedLink() {
			if(PopupMenu != null) {
				return PopupMenu.ItemLinks.FirstOrDefault();
			}
			return null;
		}
		protected virtual ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<bool> value = new DefaultValueBasedCommandUIState<bool>();
			value.Value = Down;
			return value;
		}
		protected override ICommandUIState CreateButtonItemUIState() {
			return new CheckDropDownButtonItemUIState(this);
		}
	}
	public class CheckDropDownButtonItemUIState : BarButtonItemUIState {
		readonly BarButtonItem item;
		public CheckDropDownButtonItemUIState(BarButtonItem item) : base(item) {
			this.item = item;
		}
		public virtual bool Down {
			get { return item.Down; }
			set { item.Down = value; }
		}
	}
}
