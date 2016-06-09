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

using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class SearchPanelProperties : BaseProperties, ISearchPanelProperties {
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new SearchPanelProperties();
		}
		public SearchPanelProperties()
			: base() {
			SetDefaultValueCore<ClearButtonShowMode>("ClearButtonShowMode", ClearButtonShowMode.ReplaceSearchButton);
			SetDefaultValueCore<bool>("ShowSearchButton", true);
			SetDefaultValueCore<bool>("ShowClearButton", true);
			SetDefaultValueCore<string>("Caption", "Search");
			SetDefaultValueCore<int>("FindDelay", 1000);
			SetDefaultValueCore<KeyShortcut>("Shortcut", new KeyShortcut(System.Windows.Forms.Shortcut.CtrlF));
			SetDefaultValueCore<SearchPanelAnchor>("Anchor", SearchPanelAnchor.Right);
			SetDefaultValueCore<int>("Width", 320);
			SetDefaultValueCore<int>("VisibleRowCount", 5);
			SetDefaultValueCore<string>("NullValuePrompt", "Enter text to search...");
			SetDefaultValueCore<bool>("NullValuePromptShowForEmptyValue", true);
			SetDefaultValueCore<bool>("Enabled", true);
			SetDefaultValueCore<string>("HeaderCategoryActive", "Active Page");
			SetDefaultValueCore<string>("HeaderCategoryChildren", "Child Pages");
			SetDefaultValueCore<string>("HeaderCategoryOther", "Other");
		}
		[DefaultValue(ClearButtonShowMode.ReplaceSearchButton), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public ClearButtonShowMode ClearButtonShowMode {
			get { return GetValueCore<ClearButtonShowMode>("ClearButtonShowMode"); }
			set { SetValueCore<ClearButtonShowMode>("ClearButtonShowMode", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool ShowSearchButton {
			get { return GetValueCore<bool>("ShowSearchButton"); }
			set { SetValueCore<bool>("ShowSearchButton", value); }
		}
		[DefaultValue("Search"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Localizable(true)]
		public string Caption {
			get { return GetValueCore<string>("Caption"); }
			set { SetValueCore<string>("Caption", value); }
		}
		[DefaultValue(1000), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int FindDelay {
			get { return GetValueCore<int>("FindDelay"); }
			set { SetValueCore<int>("FindDelay", value); }
		}
		void ResetShortcut() { Reset("Shortcut"); }
		bool ShouldSerializeShortcut() { return Shortcut.Key != (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F); }
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Editor("DevExpress.XtraEditors.Design.EditorButtonShortcutEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public KeyShortcut Shortcut {
			get { return GetValueCore<KeyShortcut>("Shortcut"); }
			set { SetValueCore<KeyShortcut>("Shortcut", value); }
		}
		[DefaultValue(SearchPanelAnchor.Right), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public SearchPanelAnchor AnchorType {
			get { return GetValueCore<SearchPanelAnchor>("Anchor"); }
			set { SetValueCore<SearchPanelAnchor>("Anchor", value); }
		}
		[DefaultValue(320), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int Width {
			get { return GetValueCore<int>("Width"); }
			set { SetValueCore<int>("Width", value); }
		}
		[DefaultValue(5), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int VisibleRowCount {
			get { return GetValueCore<int>("VisibleRowCount"); }
			set { SetValueCore<int>("VisibleRowCount", value); }
		}
		[DefaultValue("Enter text to search..."), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Localizable(true)]
		public string NullValuePrompt {
			get { return GetValueCore<string>("NullValuePrompt"); }
			set { SetValueCore<string>("NullValuePrompt", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool NullValuePromptShowForEmptyValue {
			get { return GetValueCore<bool>("NullValuePromptShowForEmptyValue"); }
			set { SetValueCore<bool>("NullValuePromptShowForEmptyValue", value); }
		}
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool ShowNullValuePromptWhenFocused {
			get { return GetValueCore<bool>("ShowNullValuePromptWhenFocused"); }
			set { SetValueCore<bool>("ShowNullValuePromptWhenFocused", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool Enabled {
			get { return GetValueCore<bool>("Enabled"); }
			set { SetValueCore<bool>("Enabled", value); }
		}
		[DefaultValue("Active Page"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Localizable(true)]
		public string HeaderCategoryActive {
			get { return GetValueCore<string>("HeaderCategoryActive"); }
			set { SetValueCore<string>("HeaderCategoryActive", value); }
		}
		[DefaultValue("Child Pages"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Localizable(true)]
		public string HeaderCategoryChildren {
			get { return GetValueCore<string>("HeaderCategoryChildren"); }
			set { SetValueCore<string>("HeaderCategoryChildren", value); }
		}
		[DefaultValue("Other"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Localizable(true)]
		public string HeaderCategoryOther {
			get { return GetValueCore<string>("HeaderCategoryOther"); }
			set { SetValueCore<string>("HeaderCategoryOther", value); }
		}
	}
	class CustomSearchObjectProperties : ICustomSearchObjectProperties {
		ISearchContainer searchContainerCore;
		ISearchObject searchObjectCore;
		string subTitleCore;
		string titleCore;
		public CustomSearchObjectProperties(ISearchContainer searchContainer, ISearchObject searchObject) {
			this.searchContainerCore = searchContainer;
			this.searchObjectCore = searchObject;
			this.subTitleCore = searchContainerCore.SearchText;
			this.titleCore = searchObjectCore.SearchText;
		}
		public Views.WindowsUI.IContentContainer SourceContainer { get { return searchContainerCore as Views.WindowsUI.IContentContainer; } }
		public Base.BaseComponent Source { get { return searchObjectCore as Base.BaseComponent; } }
		public TileControlImageToTextAlignment ImageToTextAlignment { get { return TileControlImageToTextAlignment.Left; } }
		public int ImageToTextIndent { get { return 10; } }
		public TileItemImageScaleMode ImageScaleMode { get { return TileItemImageScaleMode.Stretch; } }
		public object Tag { get { return searchObjectCore.SearchTag; } }
		public Image Image { get; set; }
		public Size ImageSize { get { return new Size(24, 24); } }
		public string Title { 
			get { return titleCore; }
			set {
				if(string.IsNullOrEmpty(value)) return;
				titleCore = value;
			}
		}
		public string SubTitle {
			get { return subTitleCore; }
			set {
				if(string.IsNullOrEmpty(value)) return;
				subTitleCore = value;
			}
		}
		public DefaultBoolean AllowGlyphSkinning { get; set; }
		public DefaultBoolean AllowHtmlText { get { return DefaultBoolean.True; } }
		public IEnumerable<string> Content { get; set; }
		public int SubTitleFontSizeDelta { get { return -1; } }
		public Color SubTitleForeColor { get; set; }
		public bool Visible { get; set; }
		public string GetContent() {
			if(Content == null) return null;
			string content = null;
			foreach(string str in Content) {
				if(string.IsNullOrEmpty(str)) continue;
				content += str;
			}
			return content;
		}
		void System.IDisposable.Dispose() {
			this.searchObjectCore = null;
			this.searchContainerCore = null;
			Image = null;
		}
	}
}
