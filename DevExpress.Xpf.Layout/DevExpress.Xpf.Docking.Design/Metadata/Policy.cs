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
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using DTSelection = Microsoft.Windows.Design.Interaction.Selection;
namespace DevExpress.Xpf.Docking.Design {
	class DockLayoutManagerPolicy : SelectionPolicy {
		public DockLayoutManagerPolicy() { }
		protected override IEnumerable<ModelItem> GetPolicyItems(DTSelection selection) {
			ModelItem item = selection.PrimarySelection;
			if(item != null && (item.Is<BaseLayoutItem>() || item.Is<DockLayoutManager>())) {
				item = item.GetDockLayoutManager();
			}
			if(item != null) {
				yield return item;
			}
		}
	}
	abstract class LayoutGroupParentSelectionPolicy : ConditionalSelectionPolicy {
		protected override bool Condition(ModelItem item) {
			return item.Parent.Is<LayoutGroup>();
		}
	}
	abstract class LayoutGroupParentWithInplaceLayoutBuilderSelectionPolicyBase : LayoutGroupParentSelectionPolicy {
		protected override IEnumerable<ModelItem> GetPolicyItems(Microsoft.Windows.Design.Interaction.Selection selection) {
			var items = base.GetPolicyItems(selection);
			foreach(var item in items) {
				if(item != null && item.Is<BaseLayoutItem>()) {
					yield return item;
				}
			}
		}
	}
	class LayoutGroupParentWithInplaceLayoutBuilderSelectionPolicy : LayoutGroupParentWithInplaceLayoutBuilderSelectionPolicyBase {
		protected override bool Condition(ModelItem item) {
			return base.Condition(item) && !(bool)item.Parent.Properties["IsControlItemsHost"].ComputedValue;
		}
	}
	class ControlItemsHostParentWithInplaceLayoutBuilderSelectionPolicy : ConditionalSelectionPolicy {
		protected override bool Condition(ModelItem item) {
			if(item.Is<LayoutPanel>()) return false;
			return (item.Is<BaseLayoutItem>() && ((bool)item.Properties["IsControlItemsHost"].ComputedValue) ||
				(item.Parent.Is<BaseLayoutItem>() && (bool)item.Parent.Properties["IsControlItemsHost"].ComputedValue));
		}
	}
	class InplaceLayoutBuilderSelectionPolicy : PrimarySelectionPolicy {
		protected override IEnumerable<ModelItem> GetPolicyItems(Microsoft.Windows.Design.Interaction.Selection selection) {
			ModelItem item = selection.PrimarySelection;
			if(item != null && item.Is<LayoutGroup>() && !(bool)item.Properties["IsControlItemsHost"].ComputedValue) {
				yield return item;
			}
		}
	}
}
