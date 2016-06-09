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

using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.WindowsUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
namespace DevExpress.Xpf.WindowsUI.UIAutomation {
	public class SlideViewAutomationPeer : HeaderedControlAutomationPeerBase<SlideView>, IScrollProvider {
		protected override object Header {
			get {
				return Owner.Header;
			}
		}
		public SlideViewAutomationPeer(SlideView owner)
			: base(owner) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.List;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Scroll)
				return this;
			return base.GetPattern(patternInterface);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			for(int i = 0; i < Owner.Items.Count; i++) {
				object obj = Owner.ItemContainerGenerator.ContainerFromIndex(i);
				if(obj as FrameworkElement != null) {
					AutomationPeer peer = CreatePeerForElement(obj as FrameworkElement);
					if(peer != null)
						children.Add(peer);
				}
			}
			if(Owner.BackCommand != null) {
				if(Owner.PartNavigationHeaderControl != null)
					children.Add(CreatePeerForElement(Owner.PartNavigationHeaderControl.PartBackButton));
			}
			return children;
		}
		#region IScrollProvider Members
		public double HorizontalScrollPercent {
			get { return (Owner.HorizontalOffset * 100) / (Owner.TotalWidth - Owner.ViewportWidth); }
		}
		public double HorizontalViewSize {
			get { return (Owner.ViewportWidth / Owner.TotalWidth) * 100; }
		}
		public bool HorizontallyScrollable {
			get { return Owner.HasItems; }
		}
		public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount) {
			if(verticalAmount != ScrollAmount.NoAmount)
				throw new InvalidOperationException("Vertical Scrolling is not supported");
			if(horizontalAmount == ScrollAmount.NoAmount)
				return;
			switch(horizontalAmount) {
				case ScrollAmount.LargeDecrement: Owner.SetScrollOffset(-48); break;
				case ScrollAmount.LargeIncrement: Owner.SetScrollOffset(48); break;
				case ScrollAmount.SmallDecrement: Owner.SetScrollOffset(-16); break;
				case ScrollAmount.SmallIncrement: Owner.SetScrollOffset(16); break;
			}
		}
		public void SetScrollPercent(double horizontalPercent, double verticalPercent) {
			if(verticalPercent != -1)
				throw new InvalidOperationException("Vertical Scrolling is not supported");
			if(horizontalPercent < 0 || horizontalPercent > 100)
				throw new ArgumentOutOfRangeException("Horizontal percent may not be " + horizontalPercent);
			Owner.SetScrollOffset(horizontalPercent / 100 * Owner.TotalWidth);
		}
		public double VerticalScrollPercent {
			get { return -1; }
		}
		public double VerticalViewSize {
			get { return 100; }
		}
		public bool VerticallyScrollable {
			get { return false; }
		}
		#endregion
	}
	public class SlideViewItemAutomationPeer : HeaderedControlAutomationPeerBase<SlideViewItem>, IInvokeProvider {
		public SlideViewItemAutomationPeer(SlideViewItem owner)
			: base(owner) {
		}
		protected override object Header {
			get { return Owner.Header; }
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.ListItem;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Invoke)
				return this;
			return base.GetPattern(patternInterface);
		}
		#region IInvokeProvider Members
		public void Invoke() {
			((IClickableControl)Owner).OnClick();
		}
		#endregion
	}
}
