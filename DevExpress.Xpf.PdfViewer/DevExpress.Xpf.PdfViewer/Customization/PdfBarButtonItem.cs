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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.PdfViewer.Internal;
using DevExpress.Xpf.PdfViewer.Themes;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
using DevExpress.Xpf.DocumentViewer;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfMRUSplitItem : BarSplitButtonItem {
		public PdfMRUSplitItem() {
			PopupControl = new PopupMenu();
		}
		PopupMenu PopupMenu { get { return PopupControl as PopupMenu; } }
		protected override void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			base.OnCommandChanged(oldCommand, newCommand);
			BindingOperations.ClearBinding(PopupMenu, PopupMenu.ItemLinksSourceProperty);
			PopupMenu.ClearValue(PopupMenu.ItemTemplateProperty);
			if (newCommand == null) {
				return;
			}
			DataContext = newCommand;
			Binding recentFilesBinding = new Binding("RecentFiles");
			recentFilesBinding.Source = newCommand;
			Style = (Style)FindResource(new PdfViewerThemeKeyExtension() { ResourceKey = PdfViewerThemeKeys.BarSplitButtonItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(this) });
		}
	}
	public class PdfBarSplitItem : BarSplitButtonItem {
		PopupMenu PopupMenu { get { return PopupControl as PopupMenu; } }
		public PdfBarSplitItem() {
			PopupControl = new PopupMenu();
		}
		protected override void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			base.OnCommandChanged(oldCommand, newCommand);
			BindingOperations.ClearBinding(PopupMenu, PopupMenu.ItemLinksSourceProperty); PopupMenu.ClearValue(PopupMenu.ItemTemplateProperty);
			PopupMenu.ClearValue(PopupMenu.ItemTemplateProperty);
			if (newCommand == null) {
				return;
			}
			DataContext = newCommand;
			Binding commandsBinding = new Binding("Commands");
			commandsBinding.Source = newCommand;
			PopupMenu.SetBinding(PopupMenu.ItemLinksSourceProperty, commandsBinding);
			PopupMenu.ItemTemplate = (DataTemplate)FindResource(new PdfViewerThemeKeyExtension() { ResourceKey = PdfViewerThemeKeys.PopupMenuItemTemplate, ThemeName = ThemeHelper.GetEditorThemeName(this) });
			Style = (Style)FindResource(new PdfViewerThemeKeyExtension() { ResourceKey = PdfViewerThemeKeys.BarSplitButtonItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(this) });
		}
	}
	public class PdfBarCheckItem : BarCheckItem {
		public static readonly DependencyProperty PdfBarItemNameProperty = DependencyPropertyManager.Register("PdfBarItemName", typeof(string), typeof(PdfBarCheckItem),
			new PropertyMetadata(string.Empty, (obj, args) => ((PdfBarCheckItem)obj).OnPdfBarItemNameChanged((string)args.NewValue)));
		protected virtual void OnPdfBarItemNameChanged(string newValue) {
			if (!this.IsPropertySet(NameProperty))
				Name = newValue;
		}
		public string PdfBarItemName {
			get { return (string)GetValue(PdfBarItemNameProperty); }
			set { SetValue(PdfBarItemNameProperty, value); }
		}
		protected override void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			base.OnCommandChanged(oldCommand, newCommand);
			var command = newCommand as CommandBase;
			if (command == null) {
				return;
			}
			DataContext = command;
			Style = (Style)FindResource(new PdfViewerThemeKeyExtension() { ResourceKey = PdfViewerThemeKeys.BarCheckItemStyle, ThemeName = ThemeHelper.GetEditorThemeName(this) });
		}
	}
}
