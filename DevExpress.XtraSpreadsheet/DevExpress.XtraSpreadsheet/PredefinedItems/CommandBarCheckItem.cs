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
using System.ComponentModel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetCommandBarCheckItem
	public class SpreadsheetCommandBarCheckItem : ControlCommandBarCheckItem<SpreadsheetControl, SpreadsheetCommandId>, IBarButtonGroupMember {
		string commandName = String.Empty;
		SpreadsheetCommandId id;
		string buttonGroupTag = String.Empty;
		public SpreadsheetCommandBarCheckItem()
			: base() {
		}
		public SpreadsheetCommandBarCheckItem(string caption)
			: base(caption) {
		}
		public SpreadsheetCommandBarCheckItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		public SpreadsheetCommandBarCheckItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public static SpreadsheetCommandBarCheckItem Create(SpreadsheetCommandId command) {
			return Create(command, String.Empty, RibbonItemStyles.All);
		}
		public static SpreadsheetCommandBarCheckItem Create(SpreadsheetCommandId command, RibbonItemStyles ribbonStyle) {
			return Create(command, String.Empty, ribbonStyle);
		}
		public static SpreadsheetCommandBarCheckItem Create(SpreadsheetCommandId command, string buttonGroupTag) {
			return Create(command, buttonGroupTag, RibbonItemStyles.All);
		}
		public static SpreadsheetCommandBarCheckItem Create(SpreadsheetCommandId command, string buttonGroupTag, RibbonItemStyles ribbonStyle) {
			SpreadsheetCommandBarCheckItem item = new SpreadsheetCommandBarCheckItem();
			item.CommandName = SpreadsheetCommandId.GetCommandName(command);
			item.ButtonGroupTag = buttonGroupTag;
			item.RibbonStyle = ribbonStyle;
			return item;
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
		[DefaultValue(false), Browsable(false), Localizable(false)]
		public bool CaptionDependOnUnits { get; set; }
		#endregion
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return String.IsNullOrEmpty(this.ButtonGroupTag) ? null : this.ButtonGroupTag; } }
		#endregion
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			SpreadsheetControl server = this.Control;
			server.UnitChanging += OnUnitChanging;
			server.UnitChanged += OnUnitChanged;
		}
		protected override void UnsubscribeControlEvents() {
			SpreadsheetControl server = this.Control;
			server.UnitChanging -= OnUnitChanging;
			server.UnitChanged -= OnUnitChanged;
			base.UnsubscribeControlEvents();
		}
		bool wasDefaultCaption;
		void OnUnitChanging(object sender, EventArgs e) {
			if (CaptionDependOnUnits)
				this.wasDefaultCaption = !ShouldSerializeCaption();
		}
		void OnUnitChanged(object sender, EventArgs e) {
			if (CaptionDependOnUnits) {
				if (wasDefaultCaption)
					ResetCaption();
				wasDefaultCaption = false;
			}
		}
	}
	#endregion
}
