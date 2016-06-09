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
using System.Windows;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm.Native;
using DevExpress.Data.Utils;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public sealed class XRControlRootReportTracker {
		XRControl control;
		readonly XRControlParentTracker parentTracker;
		XRControlRootReportTracker parentRootReportTracker;
		readonly Action onRootReportChanged;
		public XRControlRootReportTracker(XRControl control, Action onRootReportChanged) {
			this.control = control;
			parentTracker = new XRControlParentTracker(control, OnControlParentChanged);
			OnControlParentChanged(null);
			this.onRootReportChanged = onRootReportChanged;
		}
		public void Dispose() {
			parentTracker.Dispose();
			if(parentRootReportTracker != null)
				parentRootReportTracker.Dispose();
			control = null;
		}
		void OnControlParentChanged(XRControl oldBandParent) {
			var parent = parentTracker.ControlParent;
			if(parentRootReportTracker != null)
				parentRootReportTracker.Dispose();
			parentRootReportTracker = parent == null || parent is XtraReport ? null : new XRControlRootReportTracker(parent, OnParentRootReportChanged);
			OnParentRootReportChanged();
		}
		void OnParentRootReportChanged() {
			if(onRootReportChanged != null)
				onRootReportChanged();
		}
		public XtraReport RootReport { get { return control.RootReport; } }
	}
}
