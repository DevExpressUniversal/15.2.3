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
using System.Resources;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.Xpf.NavBar {
	public enum NavBarStringId {
		NavPaneMenuShowMoreButtons,
		NavPaneMenuShowFewerButtons,
		NavPaneMenuAddRemoveButtons,
		GroupIsAlreadyAddedToAnotherNavBarException,
		ItemIsAlreadyAddedToAnotherGroupException
	}
	public class NavBarLocalizer : DXLocalizer<NavBarStringId> {
		static NavBarLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<NavBarStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(NavBarStringId.NavPaneMenuShowMoreButtons, "Show More Buttons");
			AddString(NavBarStringId.NavPaneMenuShowFewerButtons, "Show Fewer Buttons");
			AddString(NavBarStringId.NavPaneMenuAddRemoveButtons, "Add or Remove Buttons");
			AddString(NavBarStringId.GroupIsAlreadyAddedToAnotherNavBarException, "The group is already added to another NavBar.");
			AddString(NavBarStringId.ItemIsAlreadyAddedToAnotherGroupException, "The item is already added to another group.");
		}
		#endregion
		public static XtraLocalizer<NavBarStringId> CreateDefaultLocalizer() {
			return new NavBarResXLocalizer();
		}
		public static string GetString(NavBarStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<NavBarStringId> CreateResXLocalizer() {
			return new NavBarResXLocalizer();
		}
	}
	public class NavBarResXLocalizer : DXResXLocalizer<NavBarStringId> {
		public NavBarResXLocalizer()
			: base(new NavBarLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.NavBar.LocalizationRes", typeof(NavBarResXLocalizer).Assembly);
		}
	}
}
