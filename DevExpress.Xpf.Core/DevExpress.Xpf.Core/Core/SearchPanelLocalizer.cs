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
namespace DevExpress.Xpf.Core {
	public enum SearchPanelStringId {
		ButtonTooltip_FindNext,
		ButtonTooltip_FindPrev,
		ButtonTooltip_Close,
		ButtonTooltip_SearchOptions,
		ButtonText_Replace,
		ButtonText_ReplaceAll,
		LabelText_Find,
		LabelText_Replace,
		MenuCheckItem_CaseSensative,
		MenuCheckItem_WholeWord,
		MenuCheckItem_UseRegularExpression
	};
	public class SearchPanelResXLocalizer : DXResXLocalizer<SearchPanelStringId> {
		public SearchPanelResXLocalizer()
			: base(new SearchPanelLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Core.Core.SearchPanelRes", typeof(SearchPanelResXLocalizer).Assembly);
		}
	}
	public class SearchPanelLocalizer : DXLocalizer<SearchPanelStringId> {
		static SearchPanelLocalizer() {
			XtraLocalizer<SearchPanelStringId>.SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<SearchPanelStringId>(CreateDefaultLocalizer()));
		}
		public override XtraLocalizer<SearchPanelStringId> CreateResXLocalizer() {
			return new SearchPanelResXLocalizer();
		}
		public static XtraLocalizer<SearchPanelStringId> CreateDefaultLocalizer() {
			return new SearchPanelResXLocalizer();
		}
		protected override void PopulateStringTable() {
			AddString(SearchPanelStringId.ButtonTooltip_FindPrev, "Find Previous");
			AddString(SearchPanelStringId.ButtonTooltip_FindNext, "Find Next");
			AddString(SearchPanelStringId.ButtonTooltip_Close, "Close");
			AddString(SearchPanelStringId.ButtonTooltip_SearchOptions, "Search Options");
			AddString(SearchPanelStringId.ButtonText_Replace, "Replace");
			AddString(SearchPanelStringId.ButtonText_ReplaceAll, "Replace All");
			AddString(SearchPanelStringId.LabelText_Find, "Find");
			AddString(SearchPanelStringId.LabelText_Replace, "replace with");
			AddString(SearchPanelStringId.MenuCheckItem_CaseSensative, "Case sensitive");
			AddString(SearchPanelStringId.MenuCheckItem_UseRegularExpression, "Regular expression");
			AddString(SearchPanelStringId.MenuCheckItem_WholeWord, "Whole word");
		}
		public static string GetString(SearchPanelStringId id) {
			return XtraLocalizer<SearchPanelStringId>.Active.GetLocalizedString(id);
		}
	}
}
