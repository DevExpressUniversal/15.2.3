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

namespace DevExpress.ExpressApp.Win.Templates {
	partial class PopupForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new XafComponentResourceManager(typeof(PopupForm));
			this.xafBarManager = new DevExpress.ExpressApp.Win.Templates.Controls.XafBarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.cObjectsCreation = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cRecordEdit = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cView = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cPrint = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cEdit = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cOpenObject = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cUndoRedo = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cExport = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.actionContainersManager = new DevExpress.ExpressApp.Win.Templates.ActionContainersManager(this.components);
			this.diagnosticContainer = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ButtonsContainer();
			this.buttonsContainer = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ButtonsContainer();
			this.diagnosticContainerLayoutGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.buttonsContainerLayoutGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.viewSitePanel = new DevExpress.XtraEditors.PanelControl();
			this.bottomPanel = new DevExpress.ExpressApp.Win.Layout.XafLayoutControl();
			this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
			this.simpleSeparator = new DevExpress.XtraLayout.SimpleSeparator();
			this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
			this.diagnosticContainerLayoutItem = new DevExpress.XtraLayout.LayoutControlItem();
			this.buttonsContainerLayoutItem = new DevExpress.XtraLayout.LayoutControlItem();
			this.formStateModelSynchronizer = new DevExpress.ExpressApp.Win.Core.FormStateModelSynchronizer(this.components);
			this.viewSiteManager = new DevExpress.ExpressApp.Win.Templates.ViewSiteManager(this.components);
			((System.ComponentModel.ISupportInitialize)(this.xafBarManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.diagnosticContainer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsContainer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.diagnosticContainerLayoutGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsContainerLayoutGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bottomPanel)).BeginInit();
			this.bottomPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.diagnosticContainerLayoutItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsContainerLayoutItem)).BeginInit();
			this.SuspendLayout();
			this.xafBarManager.DockControls.Add(this.barDockControlTop);
			this.xafBarManager.DockControls.Add(this.barDockControlBottom);
			this.xafBarManager.DockControls.Add(this.barDockControlLeft);
			this.xafBarManager.DockControls.Add(this.barDockControlRight);
			this.xafBarManager.Form = this;
			this.xafBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.cObjectsCreation,
			this.cRecordEdit,
			this.cView,
			this.cPrint,
			this.cEdit,
			this.cOpenObject,
			this.cUndoRedo,
			this.cExport});
			this.xafBarManager.MaxItemId = 8;
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			resources.ApplyResources(this.cObjectsCreation, "cObjectsCreation");
			this.cObjectsCreation.ContainerId = "ObjectsCreation";
			this.cObjectsCreation.Id = 0;
			this.cObjectsCreation.Name = "cObjectsCreation";
			this.cObjectsCreation.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cObjectsCreation.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cRecordEdit, "cRecordEdit");
			this.cRecordEdit.ContainerId = "RecordEdit";
			this.cRecordEdit.Id = 1;
			this.cRecordEdit.Name = "cRecordEdit";
			this.cRecordEdit.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cRecordEdit.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cView, "cView");
			this.cView.ContainerId = "View";
			this.cView.Id = 2;
			this.cView.Name = "cView";
			this.cView.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cView.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cPrint, "cPrint");
			this.cPrint.ContainerId = "Print";
			this.cPrint.Id = 3;
			this.cPrint.Name = "cPrint";
			this.cPrint.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cPrint.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cEdit, "cEdit");
			this.cEdit.ContainerId = "Edit";
			this.cEdit.Id = 4;
			this.cEdit.Name = "cEdit";
			this.cEdit.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cEdit.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cOpenObject, "cOpenObject");
			this.cOpenObject.ContainerId = "OpenObject";
			this.cOpenObject.Id = 5;
			this.cOpenObject.Name = "cOpenObject";
			this.cOpenObject.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cOpenObject.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cUndoRedo, "cUndoRedo");
			this.cUndoRedo.ContainerId = "UndoRedo";
			this.cUndoRedo.Id = 6;
			this.cUndoRedo.Name = "cUndoRedo";
			this.cUndoRedo.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cUndoRedo.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cExport, "cExport");
			this.cExport.ContainerId = "Export";
			this.cExport.Id = 7;
			this.cExport.Name = "cExport";
			this.cExport.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cExport.TargetPageGroupCaption = null;
			this.actionContainersManager.ActionContainerComponents.Add(this.cObjectsCreation);
			this.actionContainersManager.ActionContainerComponents.Add(this.cRecordEdit);
			this.actionContainersManager.ActionContainerComponents.Add(this.cView);
			this.actionContainersManager.ActionContainerComponents.Add(this.cPrint);
			this.actionContainersManager.ActionContainerComponents.Add(this.cEdit);
			this.actionContainersManager.ActionContainerComponents.Add(this.cOpenObject);
			this.actionContainersManager.ActionContainerComponents.Add(this.cUndoRedo);
			this.actionContainersManager.ActionContainerComponents.Add(this.cExport);
			this.actionContainersManager.ActionContainerComponents.Add(this.diagnosticContainer);
			this.actionContainersManager.ActionContainerComponents.Add(this.buttonsContainer);
			this.actionContainersManager.ContextMenuContainers.Add(this.cObjectsCreation);
			this.actionContainersManager.ContextMenuContainers.Add(this.cEdit);
			this.actionContainersManager.ContextMenuContainers.Add(this.cRecordEdit);
			this.actionContainersManager.ContextMenuContainers.Add(this.cUndoRedo);
			this.actionContainersManager.ContextMenuContainers.Add(this.cOpenObject);
			this.actionContainersManager.ContextMenuContainers.Add(this.cView);
			this.actionContainersManager.ContextMenuContainers.Add(this.cExport);
			this.actionContainersManager.ContextMenuContainers.Add(this.cPrint);
			this.actionContainersManager.DefaultContainer = this.cObjectsCreation;
			this.actionContainersManager.Template = this;
			this.diagnosticContainer.ActionId = null;
			this.diagnosticContainer.AllowCustomization = false;
			resources.ApplyResources(this.diagnosticContainer, "diagnosticContainer");
			this.diagnosticContainer.ContainerId = "Diagnostic";
			this.diagnosticContainer.HideItemsCompletely = true;
			this.diagnosticContainer.Name = "diagnosticContainer";
			this.diagnosticContainer.OptionsView.UseSkinIndents = false;
			this.diagnosticContainer.Orientation = DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			this.diagnosticContainer.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage;
			this.diagnosticContainer.BackColor = System.Drawing.Color.Transparent;
			this.diagnosticContainer.Root = this.diagnosticContainerLayoutGroup;
			this.buttonsContainer.ActionId = null;
			this.buttonsContainer.AllowCustomization = false;
			resources.ApplyResources(this.buttonsContainer, "buttonsContainer");
			this.buttonsContainer.ContainerId = "PopupActions";
			this.buttonsContainer.HideItemsCompletely = false;
			this.buttonsContainer.Name = "buttonsContainer";
			this.buttonsContainer.OptionsView.UseSkinIndents = false;
			this.buttonsContainer.Orientation = DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			this.buttonsContainer.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage;
			this.buttonsContainer.BackColor = System.Drawing.Color.Transparent;
			this.buttonsContainer.Root = this.buttonsContainerLayoutGroup;
			resources.ApplyResources(this.diagnosticContainerLayoutGroup, "diagnosticContainerLayoutGroup");
			this.diagnosticContainerLayoutGroup.DefaultLayoutType = DevExpress.XtraLayout.Utils.LayoutType.Horizontal;
			this.diagnosticContainerLayoutGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.diagnosticContainerLayoutGroup.GroupBordersVisible = false;
			this.diagnosticContainerLayoutGroup.Location = new System.Drawing.Point(0, 0);
			this.diagnosticContainerLayoutGroup.Name = "diagnosticContainerLayoutGroup";
			this.diagnosticContainerLayoutGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 7, 2, 0);
			this.diagnosticContainerLayoutGroup.Size = new System.Drawing.Size(7, 2);
			this.diagnosticContainerLayoutGroup.TextVisible = false;
			resources.ApplyResources(this.buttonsContainerLayoutGroup, "buttonsContainerLayoutGroup");
			this.buttonsContainerLayoutGroup.DefaultLayoutType = DevExpress.XtraLayout.Utils.LayoutType.Horizontal;
			this.buttonsContainerLayoutGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.buttonsContainerLayoutGroup.GroupBordersVisible = false;
			this.buttonsContainerLayoutGroup.Location = new System.Drawing.Point(0, 0);
			this.buttonsContainerLayoutGroup.Name = "buttonsContainerLayoutGroup";
			this.buttonsContainerLayoutGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 7, 2, 0);
			this.buttonsContainerLayoutGroup.Size = new System.Drawing.Size(7, 2);
			this.buttonsContainerLayoutGroup.TextVisible = false;
			this.viewSitePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.viewSitePanel, "viewSitePanel");
			this.viewSitePanel.Name = "viewSitePanel";
			this.bottomPanel.AllowCustomization = false;
			resources.ApplyResources(this.bottomPanel, "bottomPanel");
			this.bottomPanel.Controls.Add(this.diagnosticContainer);
			this.bottomPanel.Controls.Add(this.buttonsContainer);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.OptionsCustomizationForm.ShowLoadButton = false;
			this.bottomPanel.OptionsCustomizationForm.ShowSaveButton = false;
			this.bottomPanel.OptionsView.AllowItemSkinning = false;
			this.bottomPanel.OptionsView.EnableIndentsInGroupsWithoutBorders = true;
			this.bottomPanel.OptionsView.UseSkinIndents = false;
			this.bottomPanel.BackColor = System.Drawing.Color.Transparent;
			this.bottomPanel.Root = this.Root;
			resources.ApplyResources(this.Root, "Root");
			this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.Root.GroupBordersVisible = false;
			this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.simpleSeparator,
			this.emptySpaceItem,
			this.diagnosticContainerLayoutItem,
			this.buttonsContainerLayoutItem});
			this.Root.Location = new System.Drawing.Point(0, 0);
			this.Root.Name = "Root";
			this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.Root.Size = new System.Drawing.Size(552, 38);
			this.Root.TextVisible = false;
			this.simpleSeparator.AllowHotTrack = false;
			resources.ApplyResources(this.simpleSeparator, "simpleSeparator");
			this.simpleSeparator.Location = new System.Drawing.Point(0, 0);
			this.simpleSeparator.Name = "simpleSeparator";
			this.simpleSeparator.Size = new System.Drawing.Size(552, 2);
			this.emptySpaceItem.AllowHotTrack = false;
			resources.ApplyResources(this.emptySpaceItem, "emptySpaceItem");
			this.emptySpaceItem.Location = new System.Drawing.Point(0, 2);
			this.emptySpaceItem.Name = "emptySpaceItem";
			this.emptySpaceItem.Size = new System.Drawing.Size(518, 36);
			this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
			this.diagnosticContainerLayoutItem.Control = this.diagnosticContainer;
			resources.ApplyResources(this.diagnosticContainerLayoutItem, "diagnosticContainerLayoutItem");
			this.diagnosticContainerLayoutItem.Location = new System.Drawing.Point(518, 2);
			this.diagnosticContainerLayoutItem.Name = "diagnosticContainerLayoutItem";
			this.diagnosticContainerLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 10, 0);
			this.diagnosticContainerLayoutItem.Size = new System.Drawing.Size(17, 36);
			this.diagnosticContainerLayoutItem.TextSize = new System.Drawing.Size(0, 0);
			this.diagnosticContainerLayoutItem.TextToControlDistance = 0;
			this.diagnosticContainerLayoutItem.TextVisible = false;
			this.buttonsContainerLayoutItem.Control = this.buttonsContainer;
			resources.ApplyResources(this.buttonsContainerLayoutItem, "buttonsContainerLayoutItem");
			this.buttonsContainerLayoutItem.Location = new System.Drawing.Point(535, 2);
			this.buttonsContainerLayoutItem.Name = "buttonsContainerLayoutItem";
			this.buttonsContainerLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 10, 0);
			this.buttonsContainerLayoutItem.Size = new System.Drawing.Size(17, 36);
			this.buttonsContainerLayoutItem.TextSize = new System.Drawing.Size(0, 0);
			this.buttonsContainerLayoutItem.TextToControlDistance = 0;
			this.buttonsContainerLayoutItem.TextVisible = false;
			this.formStateModelSynchronizer.Form = this;
			this.formStateModelSynchronizer.Model = null;
			this.viewSiteManager.UseDeferredLoading = false;
			this.viewSiteManager.ViewSiteControl = this.viewSitePanel;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.viewSitePanel);
			this.Controls.Add(this.bottomPanel);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "PopupForm";
			((System.ComponentModel.ISupportInitialize)(this.xafBarManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.diagnosticContainer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsContainer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsContainerLayoutGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.diagnosticContainerLayoutGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bottomPanel)).EndInit();
			this.bottomPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.diagnosticContainerLayoutItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsContainerLayoutItem)).EndInit();
			this.ResumeLayout(false);
		}
		private DevExpress.ExpressApp.Win.Templates.Controls.XafBarManager xafBarManager;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cObjectsCreation;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cRecordEdit;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cView;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cPrint;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cEdit;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cOpenObject;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private ActionContainersManager actionContainersManager;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cUndoRedo;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cExport;
		private DevExpress.XtraEditors.PanelControl viewSitePanel;
		private DevExpress.ExpressApp.Win.Layout.XafLayoutControl bottomPanel;
		private DevExpress.XtraLayout.LayoutControlGroup Root;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ButtonsContainer diagnosticContainer;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ButtonsContainer buttonsContainer;
		private DevExpress.XtraLayout.LayoutControlGroup diagnosticContainerLayoutGroup;
		private DevExpress.XtraLayout.LayoutControlGroup buttonsContainerLayoutGroup;
		private DevExpress.XtraLayout.SimpleSeparator simpleSeparator;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
		private DevExpress.XtraLayout.LayoutControlItem diagnosticContainerLayoutItem;
		private DevExpress.XtraLayout.LayoutControlItem buttonsContainerLayoutItem;
		private DevExpress.ExpressApp.Win.Core.FormStateModelSynchronizer formStateModelSynchronizer;
		private ViewSiteManager viewSiteManager;
	}
}
