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

using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public enum ClearButtonShowMode { Hidden, AlwaysVisible, Auto, ReplaceSearchButton }
	public enum SearchPanelAnchor { Left, Right }
	public interface ISearchObject {
		string SearchText { get; }
		string SearchTag { get; }
		bool EnabledInSearch { get; }
	}
	public interface ISearchContainer : ISearchObject {
		IEnumerable<ISearchObject> SearchObjectList { get; }
		IEnumerable<ISearchContainer> SearchChildList { get; }
	}
	public interface ISearchProvider {
		ISearchContainer SearchActiveContainer { get; }
		IEnumerable<ISearchContainer> SearchOtherList { get; }
		ISearchPanelProperties SearchPanelProperties { get; }
		void CustomizeSearchItems(Views.WindowsUI.CustomizeSearchItemsEventArgs args);
		void Activate(ISearchObjectContext context);
		bool CanShowSearchPanel { get; }
	}
	public interface ISearchPanel {
		bool AttachProvider(ISearchProvider provider);
		void DetachProvider();
	}
	public interface ISearchObjectContext {
		Views.WindowsUI.IContentContainer SourceContainer { get; }
		Base.BaseComponent Source { get; }
		string Title { get; }
	}
	public interface ISearchControlProperties {
		ClearButtonShowMode ClearButtonShowMode { get; set; }
		bool ShowSearchButton { get; set; }
		int FindDelay { get; set; }
		string NullValuePrompt { get; set; }
		bool NullValuePromptShowForEmptyValue { get; set; }
		bool ShowNullValuePromptWhenFocused { get; set; }
	}
	public interface ISearchTileBarProperties : ISearchControlProperties {
		int VisibleRowCount { get; set; }
		string Caption { get; set; }
		string HeaderCategoryActive { get; set; }
		string HeaderCategoryChildren { get; set; }
		string HeaderCategoryOther { get; set; }
	}
	public interface ISearchPanelProperties : DevExpress.Utils.Base.IBaseProperties, ISearchTileBarProperties {
		DevExpress.Utils.KeyShortcut Shortcut { get; set; }
		SearchPanelAnchor AnchorType { get; set; }
		int Width { get; set; }
		bool Enabled { get; set; }
	}
	interface ICustomSearchObjectProperties : System.IDisposable, ISearchObjectContext {
		DevExpress.XtraEditors.TileControlImageToTextAlignment ImageToTextAlignment { get; }
		int ImageToTextIndent { get; }
		DevExpress.XtraEditors.TileItemImageScaleMode ImageScaleMode { get; }
		object Tag { get; }
		Image Image { get; set; }
		Size ImageSize { get; }
		new string Title { get; set; }
		string SubTitle { get; set; }
		DevExpress.Utils.DefaultBoolean AllowGlyphSkinning { get; set; }
		DevExpress.Utils.DefaultBoolean AllowHtmlText { get; }
		IEnumerable<string> Content { get; set; }
		int SubTitleFontSizeDelta { get; }
		Color SubTitleForeColor { get; set; }
		string GetContent();
		bool Visible { get; set; }
	}
}
