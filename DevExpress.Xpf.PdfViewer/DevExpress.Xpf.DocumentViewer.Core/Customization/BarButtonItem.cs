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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.DocumentViewer.Themes;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Bars.Themes;
using System;
namespace DevExpress.Xpf.DocumentViewer {
	public abstract class DocumentViewerBarButtonItem : BarButtonItem {
		protected override void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			base.OnCommandChanged(oldCommand, newCommand);
			var command = newCommand as CommandBase;
			if (command == null) {
				ClearValue(StyleProperty);
				return;
			}
			DataContext = command;
			Style = (Style)FindResource(new DocumentViewerThemeKeyExtension() { ResourceKey = DocumentViewerThemeKeys.BarButtonItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(this) });
		}
	}
	public abstract class DocumentViewerBarSubItem : BarSubItem {
		protected override void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			base.OnCommandChanged(oldCommand, newCommand);
			var command = newCommand as CommandBase;
			if (command == null) {
				ClearValue(StyleProperty);
				return;
			}
			DataContext = command;
			Style = (Style)FindResource(new DocumentViewerThemeKeyExtension() { ResourceKey = DocumentViewerThemeKeys.BarSubItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(this) });
		}
	}
	public abstract class DocumentViewerBarStaticItem : BarStaticItem {
		static DocumentViewerBarStaticItem() {
			ShowBorderProperty.OverrideMetadata(typeof(DocumentViewerBarStaticItem), new FrameworkPropertyMetadata(false));
		}
		protected override void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			base.OnCommandChanged(oldCommand, newCommand);
			var command = newCommand as CommandBase;
			if (command == null) {
				ClearValue(StyleProperty);
				return;
			}
			DataContext = command;
			Style = (Style)FindResource(new DocumentViewerThemeKeyExtension() { ResourceKey = DocumentViewerThemeKeys.BarStaticItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(this) });
		}
	}
	public class SearchBarButtonItem : BarButtonItem {
		static SearchBarButtonItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(SearchBarButtonItem), typeof(SearchBarButtonItemLink), it => new SearchBarButtonItemLink());
		}
	}
	public class SearchBarButtonItemLink : BarButtonItemLink {
		static SearchBarButtonItemLink() {
			BarItemLinkControlCreator.Default.RegisterObject(typeof(SearchBarButtonItemLink), typeof(SearchBarButtonItemLinkControl), li => new SearchBarButtonItemLinkControl());
		}
	}
	public class SearchBarButtonItemLinkControl : BarButtonItemLinkControl {
		protected override Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get { return key => (BarItemLayoutPanelThemeKeyExtension)new SearchBarButtonItemLayoutPanelThemeKeyExtension() { ResourceKey = key, ThemeName = ThemeName }; }
		}
		protected override void UpdateLayoutPanelElementContentProperties() {
			base.UpdateLayoutPanelElementContentProperties();
			if (LayoutPanel != null)
				LayoutPanel.ContentHorizontalAlignment = System.Windows.HorizontalAlignment.Center;
		}
	}
	public class SearchBarButtonItemLayoutPanelThemeKeyExtension : BarItemLayoutPanelThemeKeyExtension {
		public SearchBarButtonItemLayoutPanelThemeKeyExtension() {
		}
	}
	public class SearchBarButtonItemItemBorderThemeKeyExtension : BarItemBorderThemeKeyExtension {
		public SearchBarButtonItemItemBorderThemeKeyExtension() {
		}
	}
	public class SearchBarSubItem : BarSubItem {
		static SearchBarSubItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(SearchBarSubItem), typeof(SearchBarSubItemLink), it => new SearchBarSubItemLink());
		}
	}
	public class SearchBarSubItemLink : BarSubItemLink {
		static SearchBarSubItemLink() {
			BarItemLinkControlCreator.Default.RegisterObject(typeof(SearchBarSubItemLink), typeof(SearchBarSubItemLinkControl), li => new SearchBarSubItemLinkControl());
		}
	}
	public class SearchBarSubItemLinkControl : BarSubItemLinkControl {
		protected override Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get { return key => (BarItemLayoutPanelThemeKeyExtension)new SearchBarSubItemLayoutPanelThemeKeyExtension() { ResourceKey = key, ThemeName = ThemeName }; }
		}
	}
	public class SearchBarSubItemLayoutPanelThemeKeyExtension : BarItemLayoutPanelThemeKeyExtension {
		public SearchBarSubItemLayoutPanelThemeKeyExtension() {
		}
	}
	public class SearchBarSubItemItemBorderThemeKeyExtension : BarItemBorderThemeKeyExtension {
		public SearchBarSubItemItemBorderThemeKeyExtension() {
		}
	}
	public class SearchBarStaticItem : BarStaticItem {
		static SearchBarStaticItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(SearchBarStaticItem), typeof(SearchBarStaticItemLink), it => new SearchBarStaticItemLink());
			ShowBorderProperty.OverrideMetadata(typeof(SearchBarStaticItem), new FrameworkPropertyMetadata(false));
		}
	}
	public class SearchBarStaticItemLink : BarStaticItemLink {
		static SearchBarStaticItemLink() {
			BarItemLinkControlCreator.Default.RegisterObject(typeof(SearchBarStaticItemLink), typeof(SearchBarStaticItemLinkControl), li => new SearchBarStaticItemLinkControl());
		}
	}
	public class SearchBarStaticItemLinkControl : BarStaticItemLinkControl {
		protected override Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get { return key => (BarItemLayoutPanelThemeKeyExtension)new SearchBarStaticItemLayoutPanelThemeKeyExtension() { ResourceKey = key, ThemeName = ThemeName }; }
		}
	}
	public class SearchBarStaticItemLayoutPanelThemeKeyExtension : BarItemLayoutPanelThemeKeyExtension {
		public SearchBarStaticItemLayoutPanelThemeKeyExtension() {
		}
	}
	public class SearchBarStaticItemItemBorderThemeKeyExtension : BarItemBorderThemeKeyExtension {
		public SearchBarStaticItemItemBorderThemeKeyExtension() {
		}
	}
}
