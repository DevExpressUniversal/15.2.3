#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.ComponentModel;
namespace DevExpress.DashboardWin.Bars {
	partial class RecentDashboardsControl {
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecentDashboardsControl));
			this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
			this.panelRecentDashboards = new DevExpress.DashboardWin.Bars.NoOverlapScrollableContainer();
			this.labelRecentDashboards = new DevExpress.XtraEditors.LabelControl();
			this.splitterRecentCount = new DevExpress.XtraEditors.LabelControl();
			this.panelRecentCount = new DevExpress.XtraEditors.XtraScrollableControl();
			this.seRecentCount = new DevExpress.XtraEditors.SpinEdit();
			this.ceRecentCount = new DevExpress.XtraEditors.CheckEdit();
			this.panelRecentPlaces = new DevExpress.DashboardWin.Bars.NoOverlapScrollableContainer();
			this.labelRecentPlaces = new DevExpress.XtraEditors.LabelControl();
			this.panelEmptySpace = new DevExpress.XtraEditors.XtraScrollableControl();
			this.popupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.SuspendLayout();
			this.panelRecentCount.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.seRecentCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceRecentCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			this.SuspendLayout();
			this.splitContainer.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("splitContainer.Appearance.BackColor")));
			this.splitContainer.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("splitContainer.Appearance.ForeColor")));
			this.splitContainer.Appearance.Options.UseBackColor = true;
			this.splitContainer.Appearance.Options.UseForeColor = true;
			resources.ApplyResources(this.splitContainer, "splitContainer");
			this.splitContainer.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Panel1.Controls.Add(this.panelRecentDashboards);
			this.splitContainer.Panel1.Controls.Add(this.labelRecentDashboards);
			this.splitContainer.Panel1.Controls.Add(this.splitterRecentCount);
			this.splitContainer.Panel1.Controls.Add(this.panelRecentCount);
			resources.ApplyResources(this.splitContainer.Panel1, "splitContainer.Panel1");
			this.splitContainer.Panel2.Controls.Add(this.panelRecentPlaces);
			this.splitContainer.Panel2.Controls.Add(this.labelRecentPlaces);
			this.splitContainer.Panel2.Controls.Add(this.panelEmptySpace);
			this.splitContainer.SplitterPosition = 400;
			resources.ApplyResources(this.panelRecentDashboards, "panelRecentDashboards");
			this.panelRecentDashboards.FireScrollEventOnMouseWheel = true;
			this.panelRecentDashboards.Name = "panelRecentDashboards";
			this.labelRecentDashboards.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelRecentDashboards.Appearance.Font")));
			resources.ApplyResources(this.labelRecentDashboards, "labelRecentDashboards");
			this.labelRecentDashboards.LineLocation = DevExpress.XtraEditors.LineLocation.Bottom;
			this.labelRecentDashboards.LineVisible = true;
			this.labelRecentDashboards.Name = "labelRecentDashboards";
			this.labelRecentDashboards.ShowLineShadow = false;
			resources.ApplyResources(this.splitterRecentCount, "splitterRecentCount");
			this.splitterRecentCount.LineVisible = true;
			this.splitterRecentCount.Name = "splitterRecentCount";
			this.splitterRecentCount.ShowLineShadow = false;
			this.panelRecentCount.Controls.Add(this.seRecentCount);
			this.panelRecentCount.Controls.Add(this.ceRecentCount);
			resources.ApplyResources(this.panelRecentCount, "panelRecentCount");
			this.panelRecentCount.Name = "panelRecentCount";
			resources.ApplyResources(this.seRecentCount, "seRecentCount");
			this.seRecentCount.Name = "seRecentCount";
			this.seRecentCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seRecentCount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.seRecentCount.Properties.IsFloatValue = false;
			this.seRecentCount.Properties.Mask.EditMask = resources.GetString("seRecentCount.Properties.Mask.EditMask");
			this.seRecentCount.Properties.MaxValue = new decimal(new int[] {
			25,
			0,
			0,
			0});
			this.seRecentCount.EditValueChanged += new System.EventHandler(this.RecentCountEditValueChanged);
			resources.ApplyResources(this.ceRecentCount, "ceRecentCount");
			this.ceRecentCount.Name = "ceRecentCount";
			this.ceRecentCount.Properties.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("ceRecentCount.Properties.Appearance.ForeColor")));
			this.ceRecentCount.Properties.Appearance.Options.UseForeColor = true;
			this.ceRecentCount.Properties.Caption = resources.GetString("ceRecentCount.Properties.Caption");
			this.ceRecentCount.CheckedChanged += new System.EventHandler(this.RecentCountCheckedChanged);
			resources.ApplyResources(this.panelRecentPlaces, "panelRecentPlaces");
			this.panelRecentPlaces.FireScrollEventOnMouseWheel = true;
			this.panelRecentPlaces.Name = "panelRecentPlaces";
			this.labelRecentPlaces.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelRecentPlaces.Appearance.Font")));
			resources.ApplyResources(this.labelRecentPlaces, "labelRecentPlaces");
			this.labelRecentPlaces.LineLocation = DevExpress.XtraEditors.LineLocation.Bottom;
			this.labelRecentPlaces.LineVisible = true;
			this.labelRecentPlaces.Name = "labelRecentPlaces";
			this.labelRecentPlaces.ShowLineShadow = false;
			resources.ApplyResources(this.panelEmptySpace, "panelEmptySpace");
			this.panelEmptySpace.Name = "panelEmptySpace";
			this.popupMenu.Manager = this.barManager;
			this.popupMenu.Name = "popupMenu";
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.MaxItemId = 0;
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "RecentDashboardsControl";
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.panelRecentCount.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.seRecentCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceRecentCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.SplitContainerControl splitContainer;
		private DevExpress.XtraEditors.LabelControl splitterRecentCount;
		private DevExpress.XtraEditors.XtraScrollableControl panelRecentCount;
		private DevExpress.XtraEditors.SpinEdit seRecentCount;
		private DevExpress.XtraEditors.CheckEdit ceRecentCount;
		private NoOverlapScrollableContainer panelRecentDashboards;
		private NoOverlapScrollableContainer panelRecentPlaces;
		private DevExpress.XtraEditors.LabelControl labelRecentDashboards;
		private DevExpress.XtraEditors.LabelControl labelRecentPlaces;
		private DevExpress.XtraEditors.XtraScrollableControl panelEmptySpace;
		private DevExpress.XtraBars.PopupMenu popupMenu;
		private DevExpress.XtraBars.BarManager barManager;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
	}
	[DXToolboxItem(false)]
	class NoOverlapScrollableContainer : DevExpress.XtraEditors.XtraScrollableControl {
		public NoOverlapScrollableContainer() : base() {
		}
		protected override bool IsOverlapHScrollBar { get { return false; } }
		protected override bool IsOverlapVScrollBar { get { return false; } }
	}
}
