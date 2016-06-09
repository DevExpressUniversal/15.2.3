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
using System;
using System.Windows.Media;
namespace DevExpress.Xpf.NavBar.Automation {
	public class NavBarImageTextAutomationPeerBase : NavBarAutomationPeerBase {
		public NavBarImageTextAutomationPeerBase(FrameworkElement element)
			: base(element) {
		}
		protected virtual List<AutomationPeer> CreateChildren(){
			TextBlock text = FindTextBlock();
			if(text == null)
				return base.GetChildrenCore();
			List<AutomationPeer> children = new List<AutomationPeer>();
			Image image = FindObjectInVisualTree(Owner, "PART_Image") as Image;
			if(image != null)
				children.Add(new ImageAutomationPeer(image));			
			children.Add(new ControlElementTextBlockAutomationPeer(text));
			return children;
		}
		TextBlock FindTextBlock() {
			FrameworkElement element = FindObjectInVisualTree(Owner, "PART_Content") as FrameworkElement;
			if(element == null || VisualTreeHelper.GetChildrenCount(element) == 0)
				return null;
			element = VisualTreeHelper.GetChild(element, 0) as FrameworkElement;
			if(element == null || VisualTreeHelper.GetChildrenCount(element) == 0)
				return null;
			return VisualTreeHelper.GetChild(element, 0) as TextBlock;
		}
	}
	class ControlElementTextBlockAutomationPeer : TextBlockAutomationPeer {
		public ControlElementTextBlockAutomationPeer(TextBlock block) : base(block) { }
		protected override bool IsControlElementCore() {
			return true;
		}
		protected override string GetNameCore() {
			bool useAttachedValue;
			object value = DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.TryGetAutomationPropertyValue(
				AutomationProperties.NameProperty, 
				out useAttachedValue,
				()=>Owner
				);
			if(useAttachedValue)
				return (string)value;
			return base.GetNameCore();
		}
		protected override string GetAutomationIdCore() {
			bool useAttachedValue;
			object value = DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.TryGetAutomationPropertyValue
				(AutomationProperties.AutomationIdProperty, 
				out useAttachedValue,
				()=>Owner
				);
			return DevExpress.Xpf.Bars.Automation.BarsAutomationHelper.CreateAutomationID(Owner, this, useAttachedValue ? (string)value : base.GetAutomationIdCore());
		}
	}
}
