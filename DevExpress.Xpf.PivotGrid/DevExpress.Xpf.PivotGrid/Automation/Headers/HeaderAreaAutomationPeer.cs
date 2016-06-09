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
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.PivotGrid.Internal;
namespace DevExpress.Xpf.PivotGrid.Automation {
	public class HeaderAreaAutomationPeer : PivotGridAutomationPeerBase {
		public HeaderAreaAutomationPeer(PivotGridControl pivotGrid, FieldArea area)
			: base(pivotGrid, LayoutHelper.FindElement(pivotGrid, (d) => (d is FieldHeaders && ((FieldHeaders)d).Area.ToFieldArea() == area))) {
			Area = area;
		}
		protected FieldHeaders Headers {get{return Owner as FieldHeaders;}}
		public FieldArea Area { get; protected set; }
		protected override string GetNameCore() {
			return Area.ToString() + " Headers";
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Pane;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> peers = new List<AutomationPeer>();
			if(Headers == null || Headers.Panel == null)
				return peers;
			UIElementCollection headersCollection = Headers.Panel.Children;
			for(int i = 0; i < headersCollection.Count; i++) {
				UIElement element = headersCollection[i];
				if(element.Visibility == System.Windows.Visibility.Visible) {
					GroupHeader groupHeader = element as GroupHeader;
					FieldHeader header = element as FieldHeader;
					if(groupHeader != null)
						peers.Add(new GroupHeaderAutomationPeer(PivotGrid, groupHeader));
					if(groupHeader == null && header != null)
						peers.Add(new FieldHeaderAutomationPeer(PivotGrid, header));
				}
			}
			return peers;
		}
	}
}
