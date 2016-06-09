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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Win.Layout {
	[System.ComponentModel.ToolboxItem(false)]
	public class SplitContainer : SplitContainerControl, IXtraResizableControl {
		private Size GetPanelMinimumSize(SplitGroupPanel panel) {
			Size controlSize = panel.Controls.Count == 1 ? panel.Controls[0].MinimumSize : panel.MinimumSize;
			if(Horizontal)
				return new Size(Math.Max(controlSize.Width, panel.MinSize), controlSize.Height);
			else
				return new Size(controlSize.Width, Math.Max(controlSize.Height, panel.MinSize));
		}
		private Size GetClientMinimumSize() {
			Size panel1MinSize = GetPanelMinimumSize(Panel1);
			Size panel2MinSize = GetPanelMinimumSize(Panel2);
			if(Horizontal) {
				return new Size(SplitterBounds.Width + panel1MinSize.Width + panel2MinSize.Width, Math.Max(panel1MinSize.Height, panel2MinSize.Height));
			}
			else {
				return new Size(Math.Max(panel1MinSize.Width, panel2MinSize.Width), SplitterBounds.Height + panel1MinSize.Height + panel2MinSize.Height);
			}
		}
		protected void RaiseSizeChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		public override Size MinimumSize {
			get {
				Size clientMinSize = GetClientMinimumSize();
				Size baseMinSize = base.MinimumSize;
				return new Size(Math.Max(clientMinSize.Width, baseMinSize.Width), Math.Max(clientMinSize.Height, baseMinSize.Height));
			}
			set { base.MinimumSize = value; }
		}
		#region IXtraResizableControl Members
		public event EventHandler Changed;
		public bool IsCaptionVisible {
			get { return false; }
		}
		public Size MaxSize {
			get { return new Size(); }
		}
		public Size MinSize {
			get { return MinimumSize; }
		}
		#endregion
	}
	public class WinSimpleLayoutManager : LayoutManager {
		private Control singleControl;
		private SplitContainer splitContainer;
		private IModelSplitLayout layoutInfo;
		private Nullable<int> defaultSplitterPosition;
		private bool isSplitterPositionSet;
		private Hashtable panels = new Hashtable();
		private void splitContainer_Paint(object sender, PaintEventArgs e) {
			if (!defaultSplitterPosition.HasValue) {
				int size = splitContainer.Horizontal ? splitContainer.Width : splitContainer.Height;
				splitContainer.SplitterPosition = size /3;
			}
			else {
				splitContainer.SplitterPosition = defaultSplitterPosition.Value;
			}
			isSplitterPositionSet = true;
			splitContainer.Paint -= new PaintEventHandler(splitContainer_Paint);
		}
		private int GetMinSize(Control control) {
			int result;
			if(splitContainer.Horizontal) {
				result = control.MinimumSize.Width;
			}
			else {
				result = control.MinimumSize.Height;
			}
			if(result < 50) {
				result = 50;
			}
			return result;
		}
		protected override object GetContainerCore() {
			if(singleControl != null) {
				return singleControl;
			}
			else {
				return splitContainer;
			}
		}
		public WinSimpleLayoutManager() : base() {
		}
		public override object LayoutControls(IModelNode layoutInfo, ViewItemsCollection repositoryControls) {
			if(!(layoutInfo is IModelSplitLayout)) {
				throw new ArgumentException("The layoutInfo isn't IModelSplitLayout");
			}
			if(repositoryControls.Count == 2) {
				this.LayoutControls((IModelSplitLayout)layoutInfo, repositoryControls[0], repositoryControls[1]);
			}
			else if(repositoryControls.Count == 1) {
				this.LayoutControls((IModelSplitLayout)layoutInfo, repositoryControls[0], null);
			}
			else {
				throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.LayoutControlInvalidCount, repositoryControls.Count), "repositoryControls");
			}
			return Container;
		}
		public void LayoutControls(IModelSplitLayout layoutInfo, ViewItem firstItem, ViewItem secondItem) {
			if(firstItem == null)
				throw new ArgumentNullException("firstItem");
			if(secondItem != null) {
				splitContainer = new SplitContainer();
				splitContainer.Dock = DockStyle.Fill;
				splitContainer.Panel1.MinSize = 50;
				splitContainer.Panel1.Padding = new Padding(0);
				splitContainer.Panel2.MinSize = 50;
				splitContainer.Panel2.Padding = new Padding(0);
				splitContainer.MinimumSize = new System.Drawing.Size(50, 50);
				isSplitterPositionSet = false;
				splitContainer.Visible = true;
				singleControl = null;
				this.layoutInfo = layoutInfo;
				splitContainer.SuspendLayout();
				splitContainer.BeginInit();
				Control firstControlToPlace = (Control)firstItem.Control;
				Control secondControlToPlace = (Control)secondItem.Control;
				splitContainer.Panel1.Controls.Add(firstControlToPlace);
				splitContainer.Panel2.Controls.Add(secondControlToPlace);
				firstControlToPlace.Dock = DockStyle.Fill;
				secondControlToPlace.Dock = DockStyle.Fill;
				splitContainer.EndInit();
				splitContainer.ResumeLayout();
				panels.Clear();
				panels.Add(firstItem.Id, splitContainer.Panel1);
				panels.Add(secondItem.Id, splitContainer.Panel2);
				if(layoutInfo != null) {
					splitContainer.Horizontal = layoutInfo.Direction == DevExpress.ExpressApp.Layout.FlowDirection.Horizontal;
						defaultSplitterPosition = layoutInfo.SplitterPosition;
				}
				splitContainer.Panel1.MinSize = GetMinSize(firstControlToPlace);
				splitContainer.Panel2.MinSize = GetMinSize(secondControlToPlace);
				splitContainer.Paint += new PaintEventHandler(splitContainer_Paint);
			}
			else {
				singleControl = (Control)firstItem.Control;
				singleControl.Dock = DockStyle.Fill;
			}
		}
		public override void BreakLinksToControls() {
			if(singleControl != null) {
				singleControl = null;
			}
			if (splitContainer != null) {
				splitContainer.Paint -= new PaintEventHandler(splitContainer_Paint);
				splitContainer = null;
			}
			base.BreakLinksToControls();
		}
		public override void SaveModel() {
			base.SaveModel();
			if(singleControl == null) {
				if(layoutInfo != null && isSplitterPositionSet) {
					layoutInfo.SplitterPosition = splitContainer.SplitterPosition;
				}
			}
		}
		public override void Dispose() {
			try {
				BreakLinksToControls();
			}
			finally {
				base.Dispose();
			}
		}
		public override void ReplaceControl(string controlID, object control) {
			SplitGroupPanel panel = panels[controlID] as SplitGroupPanel;
			if(panel != null) {
				panel.Controls.Clear();
				panel.Controls.Add((Control)control);
			}
		}
	}
}
