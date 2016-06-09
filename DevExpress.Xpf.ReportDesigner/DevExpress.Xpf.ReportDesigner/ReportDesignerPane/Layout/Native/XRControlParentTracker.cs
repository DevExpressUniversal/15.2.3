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
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public sealed class XRControlParentTracker : IDisposable {
		readonly Action<XRControl> onControlParentChanged;
		XRControl control;
		XRControlInvalidater invalidater;
		XRControlParentInvalidatedWeakEventHandler<XRControlParentTracker> onParentInvalidated;
		XRControlParentChangedWeakEventHandler<XRControlParentTracker> onXRObjectParentChanged;
		public XRControlParentTracker(XRControl control, Action<XRControl> onControlParentChanged) {
			this.control = control;
			onXRObjectParentChanged = new XRControlParentChangedWeakEventHandler<XRControlParentTracker>(this, (tracker, sender, e) => OnXRObjectParentChanged(sender, e));
			control.ParentChanged += onXRObjectParentChanged.Handler;
			XRControlInvalidater.GetInvalidater(control, out invalidater);
			onParentInvalidated = new XRControlParentInvalidatedWeakEventHandler<XRControlParentTracker>(this, (layout, sender, EventArgs) => layout.OnXRObjectParentChanged(layout.control.Parent));
			invalidater.ParentInvalidated += onParentInvalidated.Handler;
			OnXRObjectParentChanged(control, new ChangeEventArgs(null, control.Parent));
			this.onControlParentChanged = onControlParentChanged;
		}
		public void Dispose() {
			invalidater.ParentInvalidated -= onParentInvalidated.Handler;
			invalidater = null;
			control.ParentChanged -= onXRObjectParentChanged.Handler;
			ControlParent = null;
			control = null;
		}
		void OnXRObjectParentChanged(object sender, ChangeEventArgs e) {
			OnXRObjectParentChanged((XRControl)e.NewValue);
		}
		void OnXRObjectParentChanged(XRControl newParent) {
			if(ControlParent == newParent) {
				return;
			}
			var oldParent = ControlParent;
			ControlParent = newParent;
			if(onControlParentChanged != null)
				onControlParentChanged(oldParent);
		}
		public XRControl ControlParent { get; private set; }
	}
}
