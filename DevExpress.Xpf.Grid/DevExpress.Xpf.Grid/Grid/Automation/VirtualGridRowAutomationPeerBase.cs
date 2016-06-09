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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using System;
namespace DevExpress.Xpf.Grid.Automation {
	public abstract class GridControlVirtualElementAutomationPeerBase : AutomationPeer {
		WeakReference dataControl;
		protected DataControlBase DataControl { get { return (DataControlBase)dataControl.Target;} }
		public GridControlVirtualElementAutomationPeerBase(DataControlBase dataControl) {
			this.dataControl = new WeakReference(dataControl);
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
				return peer != null ? peer : new GridElementAutomationPeer(DataControl, element);
			}
		}
		protected abstract FrameworkElement GetFrameworkElement();
		protected override Rect GetBoundingRectangleCore() {
#if !SL
			AutomationPeer peer = UIAutomationPeer;
			if(peer == null)
				return new Rect();
			return peer.GetBoundingRectangle();
#else
			return this.GetBoundingRectangleCore(GetFrameworkElement());
#endif
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
