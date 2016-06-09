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

using System.Windows.Automation.Provider;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using System.Linq;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Grid.Automation {
	public class FooterPanelAutomationPeer : DataControlAutomationPeerBase {
		public FooterPanelAutomationPeer(DataControlBase dataControl, FrameworkElement element) : base(dataControl, element) { }
		public FooterPanelAutomationPeer(DataControlBase dataControl) : base(dataControl, dataControl) { }
		public override AutomationPeer CreatePeerCore(DependencyObject obj) {
			if(obj is GridTotalSummary) return new TotalSummaryAutomationPeer(DataControl, obj as GridTotalSummary);
			return base.CreatePeerCore(obj);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = GetUIChildrenCore(GridControlAutomationPeerHelper.GetFooterPanelUIElement(DataControl));
#if !SL
			if(DataControl.DataView is CardView && children != null)
				return children.Where(peer => peer is FrameworkElementAutomationPeer && ((FrameworkElementAutomationPeer)peer).Owner.GetVisible()).ToList();
#endif
			return children;
		}
		protected override string GetClassNameCore() {
			return typeof(ItemsControlBase).Name;
		}
		protected override string GetNameCore() {
			return "FooterPanel";
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Pane;
		}
	}
	public static class GridControlAutomationPeerHelper {
		public static FrameworkElement GetFooterPanelUIElement(FrameworkElement root) {
			DependencyObject element = DataControlAutomationPeerBase.FindObjectInVisualTree(root, "PART_FootersPanel") ??
							DataControlAutomationPeerBase.FindObjectInVisualTree(root, "PART_FooterItemsControlBorder");
			return (FrameworkElement)element;
		}
	}
}
