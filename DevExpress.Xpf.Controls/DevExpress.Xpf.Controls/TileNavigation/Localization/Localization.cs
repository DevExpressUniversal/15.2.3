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

using DevExpress.Xpf.Core;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Utils.Localization;
using System.Resources;
using System;
using System.Windows.Markup;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows;
namespace DevExpress.Xpf.Navigation {
	public enum NavigationStringId {
		CustomizationMenu_NavigationOptions,
		CustomizationMenu_IsCompact,
		CustomizationMenu_MaxItemsCount,
		CustomizationForm_Caption,
		CustomizationForm_CompactModeCheckLabel,
		CustomizationForm_MaximumItemsCountLabel,
		CustomizationForm_DisplayOrderLabel,
		CustomizationForm_OKButton,
		CustomizationForm_CancelButton,
		CustomizationForm_ResetButton,
	}
	public class NavigationLocalizer : DXLocalizer<NavigationStringId> {
		public new static XtraLocalizer<NavigationStringId> Active {
			get { return XtraLocalizer<NavigationStringId>.Active; }
			set { XtraLocalizer<NavigationStringId>.Active = value; }
		}
		static NavigationLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<NavigationStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(NavigationStringId.CustomizationMenu_NavigationOptions, "Navigation Options...");
			AddString(NavigationStringId.CustomizationMenu_IsCompact, "Compact Navigation");
			AddString(NavigationStringId.CustomizationMenu_MaxItemsCount, "Maximum number of visible items:");
			AddString(NavigationStringId.CustomizationForm_Caption, "Navigation Options");
			AddString(NavigationStringId.CustomizationForm_CompactModeCheckLabel, "Compact Navigation");
			AddString(NavigationStringId.CustomizationForm_MaximumItemsCountLabel, "Maximum number of visible items:");
			AddString(NavigationStringId.CustomizationForm_DisplayOrderLabel, "Display in this order");
			AddString(NavigationStringId.CustomizationForm_OKButton, "OK");
			AddString(NavigationStringId.CustomizationForm_CancelButton, "Cancel");
			AddString(NavigationStringId.CustomizationForm_ResetButton, "Reset");
		}
		#endregion
		public static XtraLocalizer<NavigationStringId> CreateDefaultLocalizer() {
			return new NavigationResXLocalizer();
		}
		public static string GetString(NavigationStringId id) {
			return Active.GetLocalizedString(id);
		}
		internal static string GetString(string stringId) {
			return GetString((NavigationStringId)Enum.Parse(typeof(NavigationStringId), stringId, false));
		}
		public override XtraLocalizer<NavigationStringId> CreateResXLocalizer() {
			return new NavigationResXLocalizer();
		}
		public static string GetStringFromEnumItem(Enum enumItem) {
			return Active.GetLocalizedString((NavigationStringId)Enum.Parse(typeof(NavigationStringId), enumItem.GetType().Name + "_" + enumItem.ToString()));
		}
		public static T CreateEnumItemFromIndex<T>(int index) {
			return (T)Enum.ToObject(typeof(T), (int)index);
		}
	}
	public class NavigationResXLocalizer : DXResXLocalizer<NavigationStringId> {
		public NavigationResXLocalizer()
			: base(new NavigationLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Controls.LocalizationRes", typeof(NavigationResXLocalizer).Assembly);
		}
	}
}
