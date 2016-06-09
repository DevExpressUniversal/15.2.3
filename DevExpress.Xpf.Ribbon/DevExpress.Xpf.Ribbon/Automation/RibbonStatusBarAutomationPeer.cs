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
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Bars;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.Ribbon.Automation {
	public class RibbonStatusBarAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer {
		public RibbonStatusBarAutomationPeer(RibbonStatusBarControl control) : base(control) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.ToolBar;
		}		
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return "StatusBar";
		}
	}
	public class RibbonStatusBarPartControlAutomationPeer : DevExpress.Xpf.Bars.Automation.BaseNavigationAutomationPeer {
		public RibbonStatusBarPartControlAutomationPeer(RibbonStatusBarPartControlBase control) : base(control) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.ToolBar;
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			if(Owner is RibbonStatusBarLeftPartControl)
				return DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.CreateAutomationID(Owner, this, useAttachedValue ? ("Left part" + (string)value) : null);
			if(Owner is RibbonStatusBarRightPartControl)
				return DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.CreateAutomationID(Owner, this, useAttachedValue ? ("Right part" + (string)value) : null);
			return "" + DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.CreateAutomationID(Owner, this, null);
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[]{
				()=>Owner,
				()=>DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<RibbonStatusBarControl>(Owner as RibbonStatusBarPartControlBase)
			};
		}
	}
}
