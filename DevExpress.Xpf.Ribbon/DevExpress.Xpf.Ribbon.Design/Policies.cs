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

extern alias Platform;
using System;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Xpf.Ribbon;
namespace DevExpress.Xpf.Ribbon.Design {
	class BarManagerInfoSelectioPolicy : PrimarySelectionPolicy {
		protected override System.Collections.Generic.IEnumerable<ModelItem> GetPolicyItems(Selection selection) {
			ModelItem selectedItem = selection.PrimarySelection;
			if(selectedItem == null) return new List<ModelItem>();
			ModelService serv = selectedItem.Context.Services.GetRequiredService<ModelService>();
			return serv.Find(selectedItem.Root, typeof(RibbonControl));
		}
	}
	class RibbonControlHookPanelPolicy : SelectionPolicy {
		protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection) {
			ModelItem primary = selection.PrimarySelection;
			if(primary == null) yield break;
			ModelItem ribbon = RibbonDesignTimeHelper.FindParent<RibbonControl>(primary);
			if(ribbon != null) yield return ribbon;
			if(primary.IsItemOfType(typeof(BarItem))) {
				ModelItem ribbonItem = FindRibbonFromBarItem(primary);
				if(ribbonItem != null) yield return ribbonItem;
			} else if(primary.IsItemOfType(typeof(BarItemLink))) {
				ModelItem ribbonItem = FindRibbonFromBarItemLink(primary);
				if(ribbonItem != null) yield return ribbonItem;
			}
		}
		protected virtual ModelItem FindRibbonFromBarItem(ModelItem primary) {
			ModelItem ribbon = null;
			foreach(ModelItem link in BarManagerDesignTimeHelper.GetBarItemLinks(primary)) {
				ribbon = FindRibbonFromBarItemLink(link);
				if(ribbon != null) break;
			}
			return ribbon;
		}
		protected virtual ModelItem FindRibbonFromBarItemLink(ModelItem primary) {
			ModelItem ribbon = null;
			RibbonControl ribbonControl = null;
			foreach(BarItemLinkControl linkControl in BarManagerDesignTimeHelper.GetBarItemLinkControls(primary)) {
				ribbonControl = LayoutHelper.FindParentObject<RibbonControl>(linkControl);
				if(ribbonControl != null) {
					ribbon = BarManagerDesignTimeHelper.GetModelItem(primary.Root, ribbonControl);
					if(ribbon != null) break;
				}
			}
			return ribbon;
		}
	}
	class RibbonStatusBarSelectionPolicy : SelectionPolicy {
		protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection) {
			ModelItem primary = selection.PrimarySelection;
			if(primary == null)
				return new List<ModelItem>();
			List<ModelItem> list = new List<ModelItem>();
			ModelItem statusBar = null;
			if(primary.Parent != null && primary.Parent.IsItemOfType(typeof(BarButtonGroup))) {
				statusBar = RibbonDesignTimeHelper.GetRibbonStatusBarByBarItemLink(primary);
			} else
				statusBar = RibbonDesignTimeHelper.FindParent<RibbonStatusBarControl>(primary);
			if(statusBar != null) {
				list.Add(statusBar);
			}
			return list;
		}
	}
}
