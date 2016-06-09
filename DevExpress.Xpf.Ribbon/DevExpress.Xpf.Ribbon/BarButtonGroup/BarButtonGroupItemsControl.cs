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
using DevExpress.Xpf.Bars;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace DevExpress.Xpf.Ribbon {
	public class BarButtonGroupItemsControl : LinksControl {
		#region static
		static BarButtonGroupItemsControl() {			 
		}
		#endregion
		public BarButtonGroupItemsControl() {
			ContainerType = LinkContainerType.BarButtonGroup;
			DefaultStyleKey = typeof(BarButtonGroupItemsControl);
		}
		BarButtonGroupLinkControl BarButtonGroup { get { return LayoutHelper.FindParentObject<BarButtonGroupLinkControl>(this); } }
		protected override bool OpenPopupsAsMenu { get { return false; } }
		protected override NavigationManager CreateNavigationManager() {
			return null;
		}
		BarButtonGroupLinkControl linkControl;
		protected internal BarButtonGroupLinkControl LinkControl {
			get { return linkControl; }
			set {
				if(LinkControl == value)
					return;
				linkControl = value;
				OnLinkControlChanged();
			}
		}
		public BarButtonGroup ButtonGroup { get { return LinkControl == null ? null : LinkControl.ButtonGroup; } }
		internal ItemsPresenter ItemsPresenterCore { get { return ItemsPresenter; } }
		protected virtual void OnLinkControlChanged() {			
			if(ItemsSource is BarItemLinkInfoCollection) {
				((BarItemLinkInfoCollection)ItemsSource).Source.Clear();
			}
			UpdateItemsSource();
			if (LinkControl == null)
				this.ClearValue(SpacingModeProperty);
			else
				BindingOperations.SetBinding(this, SpacingModeProperty, new Binding("SpacingMode") { Source = LinkControl});
		}
		public override BarItemLinkCollection ItemLinks {
			get { return ButtonGroup == null? null: ((ILinksHolder)ButtonGroup).ActualLinks; }
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateBarButtonGroupBorderVisibility();
			UpdateBarButtonGroupIsEmptyProperty();
		}
		protected internal void UpdateItemsSource() {
			BarItemLinkInfoCollection oldValue = ItemsSource as BarItemLinkInfoCollection;
			ItemsSource = new BarItemLinkInfoCollection(ItemLinks);
			if(oldValue != null)
				oldValue.Source = null;
		}
		void UpdateBarButtonGroupBorderVisibility() {
			if(BarButtonGroup != null) {
				if(Items.Count == 0) {
					BarButtonGroup.IsBorderVisible = true;
					return;
				}
				foreach(BarItemLinkInfo linkInfo in Items) {
					if(linkInfo.Link is BarEditItemLink) {
						BarButtonGroup.IsBorderVisible = false;
						return;
					}
				}
				BarButtonGroup.IsBorderVisible = true;
			}
		}
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			UpdateBarButtonGroupBorderVisibility();
			UpdateBarButtonGroupIsEmptyProperty();
		}
		protected virtual void UpdateBarButtonGroupIsEmptyProperty() {
			if(BarButtonGroup != null) {
				BarButtonGroup.IsEmpty = Items.Count == 0;
			}
		}
		protected override void OnItemClick(BarItemLinkControl linkControl) {
			base.OnItemClick(linkControl);
			RibbonPageGroupControl pg = LayoutHelper.FindParentObject<RibbonPageGroupControl>(this);
			if(pg != null) {
				pg.OnContainerItemClick(this, linkControl);
			}			
		}
		protected override int GetNavigationID() { return GetHashCode(); }
		protected override bool GetCanEnterMenuMode() { return false; }
		protected override IBarsNavigationSupport GetNavigationParent() { return null; }
		protected override Orientation GetNavigationOrientation() { return Orientation.Horizontal; }
		protected override NavigationKeys GetNavigationKeys() { return NavigationKeys.Tab | NavigationKeys.Arrows; }
		protected override KeyboardNavigationMode GetNavigationMode() { return KeyboardNavigationMode.Continue; }
		protected override IList<INavigationElement> GetNavigationElements() { return new List<INavigationElement>(); }
	}
}
