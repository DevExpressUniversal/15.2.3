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
	public static class VisibilityHelper {
		public static bool GetIsVisible(BaseLayoutItem item, bool isVisible) {
			if(isVisible && item.Parent != null)
				return GetIsVisible(item.Parent);
			if(isVisible && item is LayoutGroup && ((LayoutGroup)item).ParentPanel != null)
				return GetIsVisible(item as LayoutGroup);
			return isVisible;
		}
		public static bool GetIsVisible(LayoutGroup group) {
			if(group.ParentPanel != null) return group.ParentPanel.IsVisible;
			return GetIsVisible(group, group.IsVisible);
		}
		public static bool GetIsVisible(UIElement element) {
			return element.IsVisible;
		}
		public static bool ContainsNotCollapsedItems(LayoutGroup group) {
			foreach(BaseLayoutItem item in group.Items) {
				if(item.Visibility != Visibility.Collapsed) {
					if(item is LayoutGroup) {
						LayoutGroup nested = (LayoutGroup)item;
						if(nested.ItemType == LayoutItemType.Group)
							if(nested.GroupBorderStyle == GroupBorderStyle.NoBorder && !nested.HasNotCollapsedItems && !nested.GetIsDocumentHost())
								continue;
						if(nested.ItemType == LayoutItemType.TabPanelGroup)
							if(!nested.HasNotCollapsedItems) continue;
						if(nested.ItemType == LayoutItemType.DocumentPanelGroup) {
							if(!nested.HasNotCollapsedItems) {
								if(nested.GetIsDocumentHost() || !((DocumentGroup)nested).ShowWhenEmpty) continue;
							}
						}
					}
					return true;
				}
			}
			return false;
		}
		public static bool HasVisibleItems(LayoutGroup group) {
			if(group == null) return false;
			foreach(BaseLayoutItem item in group.Items) {
				if(item.IsVisible) return true;
			}
			return false;
		}
		public static Visibility Convert(bool isVisible, Visibility invisibleValue = Visibility.Collapsed) {
			return isVisible ? Visibility.Visible : invisibleValue;
		}
	}
}
