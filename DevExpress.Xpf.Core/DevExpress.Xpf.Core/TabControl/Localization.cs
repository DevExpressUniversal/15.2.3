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

using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.Xpf.Core.Localization {
	#region TabControlStringId
	public enum TabControlStringId {
		MenuCmd_ScrollPrev,
		MenuCmd_ScrollPrevDescription,
		MenuCmd_ScrollNext,
		MenuCmd_ScrollNextDescription,
		MenuCmd_ScrollToSelectedTabItem,
		MenuCmd_ScrollToSelectedTabItemDescription,
		MenuCmd_ScrollFirst,
		MenuCmd_ScrollFirstDescription,
		MenuCmd_ScrollLast,
		MenuCmd_ScrollLastDescription,
		MenuCmd_SelectPrev,
		MenuCmd_SelectPrevDescription,
		MenuCmd_SelectNext,
		MenuCmd_SelectNextDescription,
		MenuCmd_HideSelectedItem,
		MenuCmd_HideSelectedItemDescription
	}
	#endregion
	#region TabControlLocalizer
	public class TabControlLocalizer : XtraLocalizer<TabControlStringId> {
		static TabControlLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<TabControlStringId>(CreateDefaultLocalizer()));
		}
		public static new XtraLocalizer<TabControlStringId> Active {
			get { return XtraLocalizer<TabControlStringId>.Active; }
			set { XtraLocalizer<TabControlStringId>.Active = value; }
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(TabControlStringId.MenuCmd_ScrollPrev, "Scroll to previos header");
			AddString(TabControlStringId.MenuCmd_ScrollPrevDescription, "Scroll to previos header");
			AddString(TabControlStringId.MenuCmd_ScrollNext, "Scroll to next header");
			AddString(TabControlStringId.MenuCmd_ScrollNextDescription, "Scroll to next header");
			AddString(TabControlStringId.MenuCmd_ScrollToSelectedTabItem, "Scroll to selected header");
			AddString(TabControlStringId.MenuCmd_ScrollToSelectedTabItemDescription, "Scroll to selected header");
			AddString(TabControlStringId.MenuCmd_ScrollFirst, "Scroll to first header");
			AddString(TabControlStringId.MenuCmd_ScrollFirstDescription, "Scroll to first header");
			AddString(TabControlStringId.MenuCmd_ScrollLast, "Scroll to last header");
			AddString(TabControlStringId.MenuCmd_ScrollLastDescription, "Scroll to last header");
			AddString(TabControlStringId.MenuCmd_SelectPrev, "Select previos tab");
			AddString(TabControlStringId.MenuCmd_SelectPrevDescription, "Select previos tab");
			AddString(TabControlStringId.MenuCmd_SelectNext, "Select next tab");
			AddString(TabControlStringId.MenuCmd_SelectNextDescription, "Select next tab");
			AddString(TabControlStringId.MenuCmd_HideSelectedItem, "Hide selected tab");
			AddString(TabControlStringId.MenuCmd_HideSelectedItemDescription, "Hide selected tab");
		}
		#endregion
		public static XtraLocalizer<TabControlStringId> CreateDefaultLocalizer() {
			return new TabControlLocalizer();
		}
		public static string GetString(TabControlStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<TabControlStringId> CreateResXLocalizer() {
			return new TabControlResLocalizer();
		}
	}
	#endregion
	#region TabControlResLocalizer
	public class TabControlResLocalizer : XtraResXLocalizer<TabControlStringId> {
		public TabControlResLocalizer()
			: base(new TabControlLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Core.LocalizationRes", typeof(TabControlResLocalizer).Assembly);
		}
	}
	#endregion
}
