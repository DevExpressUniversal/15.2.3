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

namespace DevExpress.XtraDiagram.Designer {
	partial class DiagramDesignerPropertyGridControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.biAlphabeticalView = new DevExpress.XtraBars.BarButtonItem();
			this.propertyGrid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.commandBar = new DevExpress.XtraBars.Bar();
			this.biCategorizedView = new DevExpress.XtraBars.BarButtonItem();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			((System.ComponentModel.ISupportInitialize)(this.propertyGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			this.SuspendLayout();
			this.biAlphabeticalView.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
			this.biAlphabeticalView.Caption = "Alphabetical";
			this.biAlphabeticalView.GroupIndex = 1;
			this.biAlphabeticalView.Id = 1;
			this.biAlphabeticalView.Name = "biAlphabeticalView";
			this.biAlphabeticalView.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnViewTypeChanged);
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Location = new System.Drawing.Point(0, 29);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(312, 525);
			this.propertyGrid.TabIndex = 6;
			this.propertyGrid.ShownEditor += new System.EventHandler(this.OnShownEditor);
			this.propertyGrid.HiddenEditor += new System.EventHandler(this.OnHiddenEditor);
			this.barManager.AllowCustomization = false;
			this.barManager.AllowQuickCustomization = false;
			this.barManager.AllowShowToolbarsPopup = false;
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.commandBar});
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.biCategorizedView,
			this.biAlphabeticalView});
			this.barManager.MaxItemId = 2;
			this.commandBar.BarName = "Tools";
			this.commandBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
			this.commandBar.DockCol = 0;
			this.commandBar.DockRow = 0;
			this.commandBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.commandBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.biCategorizedView),
			new DevExpress.XtraBars.LinkPersistInfo(this.biAlphabeticalView)});
			this.commandBar.OptionsBar.AllowQuickCustomization = false;
			this.commandBar.OptionsBar.DisableCustomization = true;
			this.commandBar.OptionsBar.DrawDragBorder = false;
			this.commandBar.OptionsBar.UseWholeRow = true;
			this.commandBar.Text = "Tools";
			this.biCategorizedView.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
			this.biCategorizedView.Caption = "Categorized";
			this.biCategorizedView.Down = true;
			this.biCategorizedView.GroupIndex = 1;
			this.biCategorizedView.Id = 0;
			this.biCategorizedView.Name = "biCategorizedView";
			this.biCategorizedView.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnViewTypeChanged);
			this.barDockControlTop.CausesValidation = false;
			this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
			this.barDockControlTop.Size = new System.Drawing.Size(312, 29);
			this.barDockControlBottom.CausesValidation = false;
			this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControlBottom.Location = new System.Drawing.Point(0, 554);
			this.barDockControlBottom.Size = new System.Drawing.Size(312, 0);
			this.barDockControlLeft.CausesValidation = false;
			this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
			this.barDockControlLeft.Size = new System.Drawing.Size(0, 525);
			this.barDockControlRight.CausesValidation = false;
			this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControlRight.Location = new System.Drawing.Point(312, 29);
			this.barDockControlRight.Size = new System.Drawing.Size(0, 525);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.propertyGrid);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "DiagramDesignerPropertyGridControl";
			this.Size = new System.Drawing.Size(312, 554);
			((System.ComponentModel.ISupportInitialize)(this.propertyGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraBars.BarButtonItem biAlphabeticalView;
		private XtraVerticalGrid.PropertyGridControl propertyGrid;
		private XtraBars.BarManager barManager;
		private XtraBars.Bar commandBar;
		private XtraBars.BarButtonItem biCategorizedView;
		private XtraBars.BarDockControl barDockControlTop;
		private XtraBars.BarDockControl barDockControlBottom;
		private XtraBars.BarDockControl barDockControlLeft;
		private XtraBars.BarDockControl barDockControlRight;
	}
}
