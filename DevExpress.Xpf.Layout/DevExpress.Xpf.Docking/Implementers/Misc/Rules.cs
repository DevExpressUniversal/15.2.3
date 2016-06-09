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
namespace DevExpress.Xpf.Docking {
	public enum DockLayoutManagerRule {
		WrongLayoutRoot,
		WrongContent,
		InconsistentLayout,
		ItemCanNotBeHidden,
		ItemCanNotBeHosted,
		WrongDocument,
		WrongPanel,
		FloatGroupsCollection,
		AutoHideGroupsCollection,
		ItemsSourceInUse,
		ItemCollectionMustBeEmpty,
		WidthIsNotSupported,
		HeightIsNotSupported
	}
	public static class DockLayoutManagerHelper {
		static Dictionary<DockLayoutManagerRule, string> errors;
		static DockLayoutManagerHelper() {
			errors = new Dictionary<DockLayoutManagerRule, string>();
			errors.Add(DockLayoutManagerRule.WrongLayoutRoot, "Only LayoutGroup can be root of layout");
			errors.Add(DockLayoutManagerRule.WrongContent, "Only LayoutGroup or UIElement can be used as content");
			errors.Add(DockLayoutManagerRule.InconsistentLayout, "Only LayoutControlItem or LayoutGroup can be hosted within layout");
			errors.Add(DockLayoutManagerRule.ItemCanNotBeHidden, "Only LayoutControlItem or LayoutGroup can be hidden from layout to customization window");
			errors.Add(DockLayoutManagerRule.ItemCanNotBeHosted, "LayoutControlItem can be hosted only in LayoutGroup");
			errors.Add(DockLayoutManagerRule.WrongDocument, "Only DocumentPanel can be hosted within DocumentGroup");
			errors.Add(DockLayoutManagerRule.WrongPanel, "TabbedGroup's elements must be LayoutPanel objects");
			errors.Add(DockLayoutManagerRule.FloatGroupsCollection, "FloatGroup can be added only to DockLayoutManager.FloatGroups collection");
			errors.Add(DockLayoutManagerRule.AutoHideGroupsCollection, "AutoHideGroup can be added only to DockLayoutManager.AutoHideGroups collection");
			errors.Add(DockLayoutManagerRule.ItemsSourceInUse, "Operation is not valid while ItemsSource is in use. Access and modify elements with LayoutGroup.ItemsSource instead.");
			errors.Add(DockLayoutManagerRule.ItemCollectionMustBeEmpty, "Items collection must be empty before using ItemsSource.");
			errors.Add(DockLayoutManagerRule.WidthIsNotSupported, "The BaseLayoutItem.Width property is not supported. Use the BaseLayoutItem.ItemWidth property instead.");
			errors.Add(DockLayoutManagerRule.HeightIsNotSupported, "The BaseLayoutItem.Height property is not supported. Use the BaseLayoutItem.ItemHeight property instead.");
		}
		public static string GetRule(DockLayoutManagerRule rule) {
			string error;
			return errors.TryGetValue(rule, out error) ? error : string.Empty;
		}
		public static bool IsPopupRoot(object obj) {
			return (obj != null) && obj.GetType().Name.EndsWith("PopupRoot");
		}
		public static bool IsPopup(object obj) {
			return (obj != null) && obj is System.Windows.Controls.Primitives.Popup;
		}
	}
}
