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

using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking {
	public abstract class FixedItem : BaseLayoutItem, IUIElement {
		static FixedItem() {
			var dProp = new DependencyPropertyRegistrator<FixedItem>();
			dProp.OverrideMetadata(AllowFloatProperty, false);
			dProp.OverrideMetadata(AllowDockProperty, false);
			dProp.OverrideMetadata(AllowCloseProperty, false);
		}
		public FixedItem() {
		}
		#region IUIElement
		UIChildren IUIElement.Children { get { return null; } }
		#endregion
	}
	[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public static class FixedItemFactory {
		public static FixedItem CreateFixedItem(LayoutItemType type) {
			FixedItem fixedItem = null;
			switch(type) {
				case LayoutItemType.EmptySpaceItem:
					fixedItem = new EmptySpaceItem();
					fixedItem.Caption = DockingLocalizer.GetString(DockingStringId.DefaultEmptySpaceContent);
					break;
				case LayoutItemType.Label:
					fixedItem = new LabelItem();
					fixedItem.Caption = DockingLocalizer.GetString(DockingStringId.DefaultLabelContent);
					break;
				case LayoutItemType.Separator:
					fixedItem = new SeparatorItem();
					fixedItem.Caption = DockingLocalizer.GetString(DockingStringId.DefaultSeparatorContent);
					break;
				case LayoutItemType.LayoutSplitter:
					fixedItem = new LayoutSplitter();
					fixedItem.Caption = DockingLocalizer.GetString(DockingStringId.DefaultSplitterContent);
					break;
				default: return null;
			}
			((ISupportInitialize)fixedItem).BeginInit();
			((ISupportInitialize)fixedItem).EndInit();
			return fixedItem;
		}
		public static FixedItem CreateFixedItem(FixedItem item) {
			return CreateFixedItem(item.ItemType);
		}
	}
}
