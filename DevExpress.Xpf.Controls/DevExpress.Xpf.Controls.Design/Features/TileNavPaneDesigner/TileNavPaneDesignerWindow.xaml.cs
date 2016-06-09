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
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Controls.Design.Features.TileNavPaneDesigner.ViewModel;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Navigation;
using System.Windows.Input;
using DevExpress.Xpf.Navigation.Internal;
using DevExpress.Xpf.WindowsUI.Base;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Controls.Primitives;
namespace DevExpress.Xpf.Controls.Design.Features.TileNavPaneDesigner {
	public partial class TileNavPaneDesignerWindow : DXWindow {
		public TileNavPaneDesignerWindow() {
			InitializeComponent();
		}
		private void categories_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			SelectPropertyGrid(TileNavPaneElements.Categories);
		}
		private void items_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			SelectPropertyGrid(TileNavPaneElements.Items);
		}
		private void subItems_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			SelectPropertyGrid(TileNavPaneElements.SubItems);
		}
		enum TileNavPaneElements { Categories, Items, SubItems }
		private void SelectPropertyGrid(TileNavPaneElements element) {
			switch(element) {
				case TileNavPaneElements.Categories: propetyGridTabControl.SelectedIndex = 0; break;
				case TileNavPaneElements.Items: propetyGridTabControl.SelectedIndex = 1; break;
				case TileNavPaneElements.SubItems: propetyGridTabControl.SelectedIndex = 2; break;
			}
		}
		private void treeView_SelectionChanged(object sender, RoutedEventArgs e) {
			var treeViewItem = (TreeViewItem)e.OriginalSource;
			treeViewItem.IsExpanded = true;
			var tile = treeViewItem.Header as NavElementBase;
			while(tile != null) {
				tile.IsSelected = true;
				tile = LogicalTreeHelper.GetParent(tile) as NavElementBase;
			}
		}
	}
	internal class DesignTimeTileBar : TileNavPaneBar {
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return false;
		}
		protected override ISelectorItem CreateSelectorItem() {
			return new TileBarItemWrapper(this);
		}
		protected override void PrepareSelectorItem(ISelectorItem selectorItem, object item) {
			base.PrepareSelectorItem(selectorItem, item);
		}
	}
	internal class TileBarItemWrapper : ClickableBase, ISelectorItem {
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(TileBarItemWrapper));
		public TileBarItemWrapper(TileBar owner) {
			SetBinding(IsSelectedProperty, new Binding("Content.IsSelected") { Source = this, Mode = BindingMode.TwoWay });
			Owner = owner;
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public ISelector Owner { get; set; }
		protected override void OnContentChanged(object oldContent, object newContent) {
			var content = newContent as ISelectorItem;
			if(content == null) return;
			content.Owner = Owner;
		}
		protected override void OnClick() {
			IsSelected = true;
		}
	}
}
