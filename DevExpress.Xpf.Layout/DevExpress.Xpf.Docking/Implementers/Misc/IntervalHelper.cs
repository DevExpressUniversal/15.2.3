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
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking {
	public static class IntervalHelper {
		public static DependencyProperty GetTargetProperty(BaseLayoutItem item1, BaseLayoutItem item2) {
			if(item1 != null && item2 != null) {
				if(item1.ItemType == LayoutItemType.Group && item2.ItemType == LayoutItemType.Group) {
					if((HasAccent(item1) && (item1.IsControlItemsHost || ContainsControlItemsHosts(item1 as LayoutGroup))) ||
						(HasAccent(item2) && (item2.IsControlItemsHost || ContainsControlItemsHosts(item2 as LayoutGroup))))
						return LayoutGroup.ActualLayoutGroupIntervalProperty;
					return LayoutGroup.ActualDockItemIntervalProperty;
				}
				if(item1.ItemType == LayoutItemType.Panel || item2.ItemType == LayoutItemType.Panel)
					return LayoutGroup.ActualDockItemIntervalProperty;
				if(item1.ItemType == LayoutItemType.ControlItem && item2.ItemType == LayoutItemType.Group)
					return HasAccent(item2) ? LayoutGroup.ActualLayoutGroupIntervalProperty : LayoutGroup.ActualLayoutItemIntervalProperty;
				if(item2.ItemType == LayoutItemType.ControlItem && item1.ItemType == LayoutItemType.Group)
					return HasAccent(item1) ? LayoutGroup.ActualLayoutGroupIntervalProperty : LayoutGroup.ActualLayoutItemIntervalProperty;
				if(item1.ItemType == LayoutItemType.ControlItem && item2.ItemType == LayoutItemType.ControlItem)
					return LayoutGroup.ActualLayoutItemIntervalProperty;
				if(item1.ItemType == LayoutItemType.ControlItem && IsFixed(item2))
					return LayoutGroup.ActualLayoutItemIntervalProperty;
				if(item2.ItemType == LayoutItemType.ControlItem && IsFixed(item1))
					return LayoutGroup.ActualLayoutItemIntervalProperty;
				if(IsFixed(item1) && IsFixed(item2))
					return LayoutGroup.ActualLayoutItemIntervalProperty;
			}
			return LayoutGroup.ActualDockItemIntervalProperty;
		}
		static bool IsFixed(BaseLayoutItem item) {
			return item is FixedItem && !(item is SeparatorItem || item is LayoutSplitter);
		}
		static bool ContainsControlItemsHosts(LayoutGroup group) {
			return group == null ? false : group.Items.ContainsOnlyControlItemsOrItsHosts();
		}
		static bool HasAccent(BaseLayoutItem group) {
			bool? isLogical = (bool?)group.GetValue(LayoutGroup.HasAccentProperty);
			return isLogical.HasValue && isLogical.Value;
		}
	}
}
