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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.TreeMap.Native;
namespace DevExpress.Xpf.TreeMap {
	public class TreeMapItemsControl : ItemsControl {
		public static readonly DependencyProperty ParentItemProperty = DependencyProperty.Register("ParentItem", typeof(object), typeof(TreeMapItemsControl));
		public object ParentItem {
			get { return (object)GetValue(ParentItemProperty); }
			set { SetValue(ParentItemProperty, value); }
		}
		static TreeMapItemsControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeMapItemsControl), new FrameworkPropertyMetadata(typeof(TreeMapItemsControl)));
		}
		internal InteractionData Interaction { get; set; }
		internal ITreeMapLayoutCalculator LayoutCalculator { get { return Interaction != null ? Interaction.LayoutCalculator : null; } }
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			TreeMapItemPresentation container = element as TreeMapItemPresentation;
			if (container != null) {
				TreeMapItem treeMapItem = item as TreeMapItem;
				if (treeMapItem != null) {
					container.TreeMapItem = treeMapItem;   
					container.ItemsSource = treeMapItem.Children;
					treeMapItem.UpdateTemplate();
					TreeMapItem parent = ParentItem as TreeMapItem;
					if (parent != null)
						parent.UpdateTemplate();
				}
				else if (Interaction != null && Interaction.Collector != null)
					container.TreeMapItem = Interaction.Collector.ProcessNativeItem(item, ParentItem);
			}
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new TreeMapItemPresentation();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return false;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (Interaction == null) {
				DependencyObject parent = VisualTreeHelper.GetParent(this);
				if (parent != null) {
					TreeMapItemsControl itemsControl = LayoutHelper.FindAmongParents<TreeMapItemsControl>(parent, null);
					if (itemsControl != null)
						Interaction = itemsControl.Interaction;
				}
			}
		}
	}
}
