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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraSpreadsheet.Commands;
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetCommandBarSubItem
	public class SpreadsheetCommandBarSubItem : ControlCommandBarSubItem<SpreadsheetControl, SpreadsheetCommandId>, IBarButtonGroupMember {
		#region Fields
		string commandName;
		SpreadsheetCommandId id;
		string buttonGroupTag = String.Empty;
		#endregion
		public static SpreadsheetCommandBarSubItem Create(SpreadsheetCommandId command) {
			return Create(command, RibbonItemStyles.All);
		}
		public static SpreadsheetCommandBarSubItem Create(SpreadsheetCommandId command, RibbonItemStyles ribbonStyle) {
			return Create(command, String.Empty, ribbonStyle);
		}
		public static SpreadsheetCommandBarSubItem Create(SpreadsheetCommandId command, string buttonGroupTag, RibbonItemStyles ribbonStyle) {
			SpreadsheetCommandBarSubItem item = new SpreadsheetCommandBarSubItem();
			item.CommandName = SpreadsheetCommandId.GetCommandName(command);
			item.ButtonGroupTag = buttonGroupTag;
			item.RibbonStyle = ribbonStyle;
			return item;
		}
		public SpreadsheetCommandBarSubItem()
			: base() {
		}
		public SpreadsheetCommandBarSubItem(string caption)
			: base(caption) {
		}
		public SpreadsheetCommandBarSubItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		public SpreadsheetCommandBarSubItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		protected override SpreadsheetCommandId CommandId { get { return id; } }
		[DefaultValue(""), Localizable(false)]
		public string CommandName {
			get { return commandName; }
			set {
				if (commandName == value)
					return;
				commandName = value;
				id = SpreadsheetCommandId.GetCommandId(commandName);
			}
		}
		[DefaultValue(""), Browsable(false), Localizable(false)]
		public string ButtonGroupTag { get { return buttonGroupTag; } set { buttonGroupTag = value; } }
		#endregion
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return String.IsNullOrEmpty(this.ButtonGroupTag) ? null : this.ButtonGroupTag; } }
		#endregion
	}
	#endregion
}
