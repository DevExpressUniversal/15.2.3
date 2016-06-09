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

using System.Diagnostics;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using DevExpress.XtraNavBar;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraNavBar {
	[ToolboxItem(false)]
	public class NavBarLocalizer : XtraLocalizer<NavBarStringId> {
		static NavBarLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<NavBarStringId>(CreateDefaultLocalizer()));
		}
		public new static XtraLocalizer<NavBarStringId> Active { 
			get { return XtraLocalizer<NavBarStringId>.Active; }
			set { XtraLocalizer<NavBarStringId>.Active = value; }
		}
		public static XtraLocalizer<NavBarStringId> CreateDefaultLocalizer() { return new NavBarResLocalizer(); }
		public override XtraLocalizer<NavBarStringId> CreateResXLocalizer() { return new NavBarResLocalizer(); }
		#region string PopulateStringTable()
		protected override void PopulateStringTable() {
			AddString(NavBarStringId.NavPaneMenuShowMoreButtons,"Show &More Buttons"); 
			AddString(NavBarStringId.NavPaneMenuShowFewerButtons, "Show &Fewer Buttons"); 
			AddString(NavBarStringId.NavPaneMenuAddRemoveButtons, "&Add or Remove Buttons"); 
			AddString(NavBarStringId.NavPaneChevronHint, "Configure buttons");
			AddString(NavBarStringId.NavPaneMenuPaneOptions, "Na&vigation Pane Options..."); 
			AddString(NavBarStringId.NavPaneOptionsFormMoveUp, "Move Up");
			AddString(NavBarStringId.NavPaneOptionsFormMoveDown, "Move Down");
			AddString(NavBarStringId.NavPaneOptionsFormFont, "Font");
			AddString(NavBarStringId.NavPaneOptionsFormReset, "Reset");
			AddString(NavBarStringId.NavPaneOptionsFormOk, "OK");
			AddString(NavBarStringId.NavPaneOptionsFormCancel, "Cancel");
			AddString(NavBarStringId.NavPaneOptionsFormDescription, "Display buttons in this order");
		}
		#endregion
	}
	public class NavBarResLocalizer : XtraResXLocalizer<NavBarStringId> {
		public NavBarResLocalizer() : base(new NavBarLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraNavBar.LocalizationRes", typeof(NavBarResLocalizer).Assembly);
		}
	}
	#region enum NavBarStringId
	public enum NavBarStringId {
		NavPaneMenuShowMoreButtons,
		NavPaneMenuShowFewerButtons,
		NavPaneMenuAddRemoveButtons,
		NavPaneChevronHint,
		NavPaneMenuPaneOptions,
		NavPaneOptionsFormMoveUp,
		NavPaneOptionsFormMoveDown,
		NavPaneOptionsFormFont,
		NavPaneOptionsFormReset,
		NavPaneOptionsFormOk,
		NavPaneOptionsFormCancel,
		NavPaneOptionsFormDescription
	}
	#endregion
}
