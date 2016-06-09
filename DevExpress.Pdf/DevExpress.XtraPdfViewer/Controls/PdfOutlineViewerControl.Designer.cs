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

namespace DevExpress.XtraPdfViewer.Controls {
	partial class PdfOutlineViewerControl {
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PdfOutlineViewerControl));
			this.outlineViewerImageCollection = new DevExpress.Utils.ImageCollection(this.components);
			this.outlineViewerBarManager = new DevExpress.XtraBars.BarManager(this.components);
			this.outlineViewerBar = new DevExpress.XtraBars.Bar();
			this.outlineViewerMenu = new DevExpress.XtraPdfViewer.Native.PdfOutlineViewerSubItem();
			this.expandCurrentNodeButton = new DevExpress.XtraBars.BarButtonItem();
			this.outlineViewerBarAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.popupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
			this.outlineViewerSeparator = new DevExpress.XtraPdfViewer.Native.PdfSeparatorControl();
			this.outlineViewerTreeList = new DevExpress.XtraPdfViewer.Native.PdfOutlineViewerTreeList();
			this.titleColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.titleColumnMemoEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
			((System.ComponentModel.ISupportInitialize)(this.outlineViewerImageCollection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.outlineViewerBarManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.outlineViewerBarAndDockingController)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.outlineViewerTreeList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.titleColumnMemoEdit)).BeginInit();
			this.SuspendLayout();
			this.outlineViewerImageCollection.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("outlineViewerImageCollection.ImageStream")));
			this.outlineViewerImageCollection.Images.SetKeyName(0, "Menu_16x16.png");
			this.outlineViewerImageCollection.Images.SetKeyName(1, "Expand_bookmark_16x16.png");
			this.outlineViewerBarManager.AllowShowToolbarsPopup = false;
			this.outlineViewerBarManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.outlineViewerBar});
			this.outlineViewerBarManager.Controller = this.outlineViewerBarAndDockingController;
			this.outlineViewerBarManager.DockControls.Add(this.barDockControlTop);
			this.outlineViewerBarManager.DockControls.Add(this.barDockControlBottom);
			this.outlineViewerBarManager.DockControls.Add(this.barDockControlLeft);
			this.outlineViewerBarManager.DockControls.Add(this.barDockControlRight);
			this.outlineViewerBarManager.Form = this;
			this.outlineViewerBarManager.Images = this.outlineViewerImageCollection;
			this.outlineViewerBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.expandCurrentNodeButton,
			this.outlineViewerMenu});
			this.outlineViewerBarManager.MaxItemId = 5;
			this.outlineViewerBar.BarName = "outlineViewerBar";
			this.outlineViewerBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
			this.outlineViewerBar.DockCol = 0;
			this.outlineViewerBar.DockRow = 0;
			this.outlineViewerBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.outlineViewerBar.FloatLocation = new System.Drawing.Point(1904, 711);
			this.outlineViewerBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.outlineViewerMenu),
			new DevExpress.XtraBars.LinkPersistInfo(this.expandCurrentNodeButton)});
			this.outlineViewerBar.OptionsBar.AllowQuickCustomization = false;
			this.outlineViewerBar.OptionsBar.DrawBorder = false;
			this.outlineViewerBar.OptionsBar.UseWholeRow = true;
			resources.ApplyResources(this.outlineViewerBar, "outlineViewerBar");
			this.outlineViewerMenu.AllowRightClickInMenu = false;
			resources.ApplyResources(this.outlineViewerMenu, "outlineViewerMenu");
			this.outlineViewerMenu.Id = 1;
			this.outlineViewerMenu.ImageIndex = 0;
			this.outlineViewerMenu.Name = "outlineViewerMenu";
			this.outlineViewerMenu.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
			this.expandCurrentNodeButton.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
			resources.ApplyResources(this.expandCurrentNodeButton, "expandCurrentNodeButton");
			this.expandCurrentNodeButton.Id = 0;
			this.expandCurrentNodeButton.ImageIndex = 1;
			this.expandCurrentNodeButton.Name = "expandCurrentNodeButton";
			this.expandCurrentNodeButton.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
			this.outlineViewerBarAndDockingController.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.outlineViewerBarAndDockingController.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.popupMenu.Manager = this.outlineViewerBarManager;
			this.popupMenu.Name = "popupMenu";
			resources.ApplyResources(this.outlineViewerSeparator, "outlineViewerSeparator");
			this.outlineViewerSeparator.Name = "outlineViewerSeparator";
			this.outlineViewerSeparator.TabStop = false;
			this.outlineViewerTreeList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.outlineViewerTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.titleColumn});
			resources.ApplyResources(this.outlineViewerTreeList, "outlineViewerTreeList");
			this.outlineViewerTreeList.KeyFieldName = "Id";
			this.outlineViewerTreeList.Name = "outlineViewerTreeList";
			this.outlineViewerTreeList.OptionsClipboard.AllowCopy = Utils.DefaultBoolean.False;
			this.outlineViewerTreeList.OptionsBehavior.AutoNodeHeight = false;
			this.outlineViewerTreeList.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.None;
			this.outlineViewerTreeList.OptionsBehavior.Editable = false;
			this.outlineViewerTreeList.OptionsBehavior.KeepSelectedOnClick = false;
			this.outlineViewerTreeList.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.outlineViewerTreeList.OptionsSelection.MultiSelect = true;
			this.outlineViewerTreeList.OptionsView.ShowColumns = false;
			this.outlineViewerTreeList.OptionsView.ShowHorzLines = false;
			this.outlineViewerTreeList.OptionsView.ShowIndicator = false;
			this.outlineViewerTreeList.OptionsView.ShowVertLines = false;
			this.outlineViewerTreeList.ParentFieldName = "ParentId";
			this.outlineViewerTreeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.titleColumnMemoEdit});
			resources.ApplyResources(this.titleColumn, "titleColumn");
			this.titleColumn.ColumnEdit = this.titleColumnMemoEdit;
			this.titleColumn.FieldName = "Title";
			this.titleColumn.Name = "titleColumn";
			this.titleColumnMemoEdit.Name = "titleColumnMemoEdit";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.outlineViewerTreeList);
			this.Controls.Add(this.outlineViewerSeparator);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "PdfOutlineViewerControl";
			((System.ComponentModel.ISupportInitialize)(this.outlineViewerImageCollection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.outlineViewerBarManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.outlineViewerBarAndDockingController)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.outlineViewerTreeList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.titleColumnMemoEdit)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraBars.BarManager outlineViewerBarManager;
		private XtraBars.Bar outlineViewerBar;
		private XtraBars.BarDockControl barDockControlTop;
		private XtraBars.BarDockControl barDockControlBottom;
		private XtraBars.BarDockControl barDockControlLeft;
		private XtraBars.BarDockControl barDockControlRight;
		private XtraBars.BarButtonItem expandCurrentNodeButton;
		private Utils.ImageCollection outlineViewerImageCollection;
		private XtraBars.BarAndDockingController outlineViewerBarAndDockingController;
		private XtraBars.PopupMenu popupMenu;
		private Native.PdfSeparatorControl outlineViewerSeparator;
		private Native.PdfOutlineViewerTreeList outlineViewerTreeList;
		private XtraTreeList.Columns.TreeListColumn titleColumn;
		private XtraEditors.Repository.RepositoryItemMemoEdit titleColumnMemoEdit;
		private Native.PdfOutlineViewerSubItem outlineViewerMenu;
	}
}
