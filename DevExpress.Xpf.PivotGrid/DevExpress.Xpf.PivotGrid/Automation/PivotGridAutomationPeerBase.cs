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
using System.Windows.Media;
namespace DevExpress.Xpf.PivotGrid.Automation {
	public interface IAutomationPeerCreator {
		AutomationPeer CreatePeer(DependencyObject obj);
	}
	public class PivotGridAutomationPeerBase : FrameworkElementAutomationPeer, IAutomationPeerCreator {
		public PivotGridAutomationPeerBase(PivotGridControl pivotGrid, FrameworkElement element)
			: base(element) {
			PivotGrid = pivotGrid;
		}
		protected PivotGridControl PivotGrid { get; set; }
		public static List<AutomationPeer> GetUIChildrenCore(DependencyObject obj, IAutomationPeerCreator owner) {
			List<AutomationPeer> children = null;
			GetUIChildrenCore(obj, owner, ref children);
			return children;
		}
		public static void GetUIChildrenCore(DependencyObject obj, IAutomationPeerCreator owner, ref List<AutomationPeer> children) {
			if(obj == null) return;
			AutomationPeer peer = null;
			for(int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++) {
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				peer = null;
				if(child != null && (!(child is UIElement) || ((UIElement)child).Visibility == Visibility.Visible)) {
					peer = owner.CreatePeer(child);
					if(peer != null) {
						if(children == null)
							children = new List<AutomationPeer>();
						children.Add(peer);
					}
				}
				if(peer == null)
					GetUIChildrenCore(child, owner, ref children);
			}
			return;
		}
		#region IAutomationPeerCreator Members
		AutomationPeer IAutomationPeerCreator.CreatePeer(DependencyObject obj) {
			AutomationPeer peer = PivotGrid.PeerCache.GetPeer(obj);
			if(peer != null) return peer;
			peer = CreatePeerCore(obj);
			PivotGrid.PeerCache.AddPeer(obj, peer, true);
			return peer;
		}
		protected virtual AutomationPeer CreatePeerCore(DependencyObject obj) {
			return CreatePeerDefault(obj);
		}
		public static AutomationPeer CreatePeerDefault(DependencyObject obj) {
#if !SL
			if(obj is UIElement3D)
				return UIElement3DAutomationPeer.CreatePeerForElement(obj as UIElement3D);
			else if(obj is UIElement)
				return UIElementAutomationPeer.CreatePeerForElement(obj as UIElement);
#else
			if(obj is UIElement)
				return FrameworkElementAutomationPeer.CreatePeerForElement(obj as UIElement);
#endif
			return null;
		}
		#endregion
	}
	public class PivotGridElementAutomationPeer : PivotGridAutomationPeerBase {
		public PivotGridElementAutomationPeer(PivotGridControl pivotGridControl, FrameworkElement element)
			: base(pivotGridControl, element) {
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return GetUIChildrenCore(Owner, Owner as IAutomationPeerCreator);
		}
	}
	public abstract class PivotGridControlVirtualElementAutomationPeerBase : AutomationPeer {
		protected PivotGridControl PivotGrid { get; set; }
		protected PivotGridControlVirtualElementAutomationPeerBase(PivotGridControl pivotGridControl) {
			PivotGrid = pivotGridControl;
		}
		protected AutomationPeer UIAutomationPeer {
			get {
				FrameworkElement element = GetFrameworkElement();
				if(element == null)
					return null;
#if !SL
				AutomationPeer peer = UIElementAutomationPeer.FromElement(element);
#else
				AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(element);
#endif
				return peer != null ? peer : new PivotGridAutomationPeer(PivotGrid, element);
			}
		}
		protected abstract FrameworkElement GetFrameworkElement();
		protected override Rect GetBoundingRectangleCore() {
			AutomationPeer peer = UIAutomationPeer;
			if(peer == null)
				return new Rect();
			return peer.GetBoundingRectangle();
		}
		protected override bool IsOffscreenCore() {
			AutomationPeer peer = UIAutomationPeer;
			if(peer == null)
				return true;
			return peer.IsOffscreen();
		}
		protected override AutomationOrientation GetOrientationCore() {
			return AutomationOrientation.None;
		}
		protected override string GetItemTypeCore() {
			return string.Empty;
		}
		protected override string GetClassNameCore() {
			return string.Empty;
		}
		protected override string GetItemStatusCore() {
			return string.Empty;
		}
		protected override bool IsRequiredForFormCore() {
			return false;
		}
		protected override bool IsKeyboardFocusableCore() {
			return false;
		}
		protected override bool HasKeyboardFocusCore() {
			return false;
		}
		protected override bool IsEnabledCore() {
			return true;
		}
		protected override bool IsPasswordCore() {
			return false;
		}
		protected override string GetAutomationIdCore() {
			AutomationPeer peer = UIAutomationPeer;
			if(peer == null)
				return string.Empty;
			return peer.GetAutomationId();
		}
		protected override bool IsContentElementCore() {
			return true;
		}
		protected override bool IsControlElementCore() {
			return true;
		}
		protected override AutomationPeer GetLabeledByCore() {
			return null;
		}
		protected override string GetHelpTextCore() {
			return string.Empty;
		}
		protected override string GetAcceleratorKeyCore() {
			return string.Empty;
		}
		protected override string GetAccessKeyCore() {
			return string.Empty;
		}
		protected override Point GetClickablePointCore() {
			AutomationPeer peer = UIAutomationPeer;
			if(peer == null)
				return new Point(double.NaN, double.NaN);
			return peer.GetClickablePoint();
		}
		protected override void SetFocusCore() {
		}
	}
}
