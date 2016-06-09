#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraLayout;
namespace DevExpress.ExpressApp.Win.Layout {
	public class XafLayoutControlItem : LayoutControlItem {
		private static readonly Size defaultMinimalSize = new Size(1, 1);
		private static HashSet<LayoutControl> layoutControlsToRefresh = new HashSet<LayoutControl>();
		private Control RaiseQueryControl() {
			QueryControlEventArgs args = new QueryControlEventArgs();
			if(QueryControl != null) {
				QueryControl(this, args);
			}
			return args.Control;
		}
		private void layoutControl_Invalidated(object sender, InvalidateEventArgs e) {
			LayoutControl layoutControl = (LayoutControl)sender;
			RemoveCacheEntry(layoutControl);
			layoutControl.Refresh();
		}
		private void layoutControl_Disposed(object sender, EventArgs e) {
			RemoveCacheEntry((LayoutControl)sender);
		}
		private void RemoveCacheEntry(LayoutControl layoutControl) {
			layoutControlsToRefresh.Remove(layoutControl);
			layoutControl.Invalidated -= layoutControl_Invalidated;
			layoutControl.Disposed -= layoutControl_Disposed;
		}
		protected override void RaiseShowHide(bool visible) {
			base.RaiseShowHide(visible);
			if(Control == null && visible && !DisposingFlag && !IsUpdateLocked) {
				Control control = RaiseQueryControl();
				if(control != null) {
					LayoutControl layoutControl = Owner as LayoutControl;
					if(layoutControl != null && !layoutControlsToRefresh.Contains(layoutControl)) {
						layoutControlsToRefresh.Add(layoutControl);
						layoutControl.Invalidated += layoutControl_Invalidated;
						layoutControl.Disposed += layoutControl_Disposed;
					}
					BeginInit();
					originalEnabled = control.Enabled;
					originalEnabledInitialized = true;
					Control = control;
					EndInit();
				}
			}
		}
		protected override void Dispose(bool disposing) {
			QueryControl = null;
			base.Dispose(disposing);
		}
		public XafLayoutControlItem() { }
		public XafLayoutControlItem(LayoutControlGroup parent) : base(parent) { }
		public XafLayoutControlItem(LayoutControl layoutControl, Control control) : base(layoutControl, control) { }
		public override Size MinSize {
			get { return Control != null ? base.MinSize : defaultMinimalSize; }
		}
		public event EventHandler<QueryControlEventArgs> QueryControl;
#if DebugTest
		public static HashSet<LayoutControl> DebugTest_LayoutControlsToRefresh {
			get { return layoutControlsToRefresh; }
		}
#endif
	}
	public class QueryControlEventArgs : EventArgs {
		public Control Control { get; set; }
	}
}
