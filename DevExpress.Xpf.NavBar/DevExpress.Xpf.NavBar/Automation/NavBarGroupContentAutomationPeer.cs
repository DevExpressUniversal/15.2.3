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
using System.Windows.Automation;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using System;
namespace DevExpress.Xpf.NavBar.Automation {
	public class NavBarGroupContentAutomationPeer : NavBarAutomationPeerBase {
		public NavBarGroupContentAutomationPeer(FrameworkElement element)
			: base(element) {			
		}		
		protected override List<AutomationPeer> GetChildrenCore() {			
			List<AutomationPeer> children = new List<AutomationPeer>();
			DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.GetChildrenRecursive(children, Owner);
			return children;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}	 
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = TryGetAutomationPropertyValue(AutomationProperties.NameProperty, out useAttachedValue);
			if(useAttachedValue)
				return (string)value;
			return "GroupContent";
		}
		protected override Func<DependencyObject>[] GetAttachedAutomationPropertySource() {
			return new Func<DependencyObject>[] { () => Owner };
		}
	}
}
