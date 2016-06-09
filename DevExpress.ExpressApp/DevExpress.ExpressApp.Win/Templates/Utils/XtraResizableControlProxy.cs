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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers.Docking;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Templates.Utils {
	public class XtraResizableControlProxy : IXtraResizableControl {
		private Size minSize = new Size(1, 1);
		private BarDockControls dockControls;
		private PanelControl viewPanel;
		private Size GetMinSize(Control control) {
			if(control == null) return new Size(1, 1);
			IXtraResizableControl resizableControl = control as IXtraResizableControl;
			return resizableControl != null ? resizableControl.MinSize : control.MinimumSize;
		}
		private Size CalculateBorderSize() {
			int width = viewPanel.Bounds.Width - viewPanel.DisplayRectangle.Width;
			int height = viewPanel.DisplayRectangle.Height < 0 ? width : viewPanel.Bounds.Height - viewPanel.DisplayRectangle.Height;
			return new Size(width, height);
		}
		private Size CalculateAggregateMinSize(Size controlMinSize, Size borderSize) {
			return new Size(dockControls[BarDockStyle.Left].Width + controlMinSize.Width + dockControls[BarDockStyle.Right].Width + borderSize.Width,
							dockControls[BarDockStyle.Top].Height + controlMinSize.Height + dockControls[BarDockStyle.Bottom].Height + borderSize.Height);
		}
		private Size CalculateMinSize(Control control) {
			Size controlMinSize = GetMinSize(control);
			Size borderSize = CalculateBorderSize();
			return CalculateAggregateMinSize(controlMinSize, borderSize);
		}
		private void UpdateMinSize() {
			if(Changed != null) {
				Control control = viewPanel.Controls.Count == 1 ? viewPanel.Controls[0] : null;
				Size newSize = CalculateMinSize(control);
				if(minSize != newSize) {
					minSize = newSize;
					Changed(this, EventArgs.Empty);
				}
			}
		}
		private void ViewPanel_ControlAdded(object sender, ControlEventArgs e) {
			if(e.Control is IXtraResizableControl) {
				((IXtraResizableControl)e.Control).Changed += ViewResizableControl_Changed;
			}
			UpdateMinSize();
		}
		private void ViewPanel_ControlRemoved(object sender, ControlEventArgs e) {
			if(e.Control is IXtraResizableControl) {
				((IXtraResizableControl)e.Control).Changed -= ViewResizableControl_Changed;
			}
			UpdateMinSize();
		}
		private void ViewResizableControl_Changed(object sender, EventArgs e) {
			UpdateMinSize();
		}
		private void BarDockControl_SizeChanged(object sender, EventArgs e) {
			UpdateMinSize();
		}
		public XtraResizableControlProxy(PanelControl viewPanel, BarDockControls barDockControls) {
			this.viewPanel = viewPanel;
			this.dockControls = barDockControls;
			viewPanel.ControlAdded += ViewPanel_ControlAdded;
			viewPanel.ControlRemoved += ViewPanel_ControlRemoved;
			foreach(BarDockControl dockControl in barDockControls) {
				dockControl.SizeChanged += BarDockControl_SizeChanged;
			}
		}
		public bool IsCaptionVisible {
			get { return false; }
		}
		public Size MaxSize {
			get { return new Size(0, 0); }
		}
		public Size MinSize {
			get { return minSize; }
		}
		public event EventHandler Changed;
	}
}
