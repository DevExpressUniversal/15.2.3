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

namespace DevExpress.DashboardWin.Native {
	partial class ChartSeriesOptionsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartSeriesOptionsForm));
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
			this.cbPosition = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbOrientation = new DevExpress.XtraEditors.ComboBoxEdit();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.cbOverlappingMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.ceShowPointLabels = new DevExpress.XtraEditors.CheckEdit();
			this.cbContentType = new DevExpress.XtraEditors.ComboBoxEdit();
			this.ceShowForZeroValues = new DevExpress.XtraEditors.CheckEdit();
			this.seriesGallery = new DevExpress.DashboardWin.Native.ChartSeriesGalleryControl();
			this.cePlotOnSecondaryAxis = new DevExpress.XtraEditors.CheckEdit();
			this.ceIgnoreEmptyPoints = new DevExpress.XtraEditors.CheckEdit();
			this.ceShowPointMarkers = new DevExpress.XtraEditors.CheckEdit();
			this.lcgRoot = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciCancelButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.tabbedControlGroup = new DevExpress.XtraLayout.TabbedControlGroup();
			this.lcgPointLabelOptions = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciShowPointLabels = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lciPointLabelContentType = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciPointLabelOverlappingMode = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciPointLabelOrientation = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciShowForZeroValues = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciPosition = new DevExpress.XtraLayout.LayoutControlItem();
			this.separatorPointLabelOptions = new DevExpress.XtraLayout.SimpleSeparator();
			this.lblBarOptions = new DevExpress.XtraLayout.SimpleLabelItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lblBubbleOptions = new DevExpress.XtraLayout.SimpleLabelItem();
			this.lcgSeriesType = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.lcgCommonOptions = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciShowPointMarkers = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciIgnoreEmptyPoints = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciPlotOnSecondaryAxis = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lciOkButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
			this.layoutControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOrientation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOverlappingMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowPointLabels.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbContentType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowForZeroValues.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cePlotOnSecondaryAxis.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceIgnoreEmptyPoints.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowPointMarkers.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgRoot)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancelButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabbedControlGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgPointLabelOptions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciShowPointLabels)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPointLabelContentType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPointLabelOverlappingMode)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPointLabelOrientation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciShowForZeroValues)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPosition)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separatorPointLabelOptions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lblBarOptions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lblBubbleOptions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgSeriesType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgCommonOptions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciShowPointMarkers)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciIgnoreEmptyPoints)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPlotOnSecondaryAxis)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOkButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).BeginInit();
			this.SuspendLayout();
			this.barManager1.DockControls.Add(this.barDockControlTop);
			this.barManager1.DockControls.Add(this.barDockControlBottom);
			this.barManager1.DockControls.Add(this.barDockControlLeft);
			this.barManager1.DockControls.Add(this.barDockControlRight);
			this.barManager1.Form = this;
			this.barManager1.MaxItemId = 0;
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl;
			this.layoutControl.AllowCustomization = false;
			this.layoutControl.Controls.Add(this.cbPosition);
			this.layoutControl.Controls.Add(this.cbOrientation);
			this.layoutControl.Controls.Add(this.btnCancel);
			this.layoutControl.Controls.Add(this.btnOK);
			this.layoutControl.Controls.Add(this.cbOverlappingMode);
			this.layoutControl.Controls.Add(this.ceShowPointLabels);
			this.layoutControl.Controls.Add(this.cbContentType);
			this.layoutControl.Controls.Add(this.ceShowForZeroValues);
			this.layoutControl.Controls.Add(this.seriesGallery);
			this.layoutControl.Controls.Add(this.cePlotOnSecondaryAxis);
			this.layoutControl.Controls.Add(this.ceIgnoreEmptyPoints);
			this.layoutControl.Controls.Add(this.ceShowPointMarkers);
			resources.ApplyResources(this.layoutControl, "layoutControl");
			this.layoutControl.Name = "layoutControl";
			this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(415, 145, 807, 834);
			this.layoutControl.Root = this.lcgRoot;
			resources.ApplyResources(this.cbPosition, "cbPosition");
			this.cbPosition.Name = "cbPosition";
			this.cbPosition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPosition.Properties.Buttons"))))});
			this.cbPosition.Properties.Items.AddRange(new object[] {
			resources.GetString("cbPosition.Properties.Items"),
			resources.GetString("cbPosition.Properties.Items1")});
			this.cbPosition.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPosition.StyleController = this.layoutControl;
			resources.ApplyResources(this.cbOrientation, "cbOrientation");
			this.cbOrientation.Name = "cbOrientation";
			this.cbOrientation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbOrientation.Properties.Buttons"))))});
			this.cbOrientation.Properties.Items.AddRange(new object[] {
			resources.GetString("cbOrientation.Properties.Items"),
			resources.GetString("cbOrientation.Properties.Items1"),
			resources.GetString("cbOrientation.Properties.Items2")});
			this.cbOrientation.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbOrientation.StyleController = this.layoutControl;
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.StyleController = this.layoutControl;
			resources.ApplyResources(this.cbOverlappingMode, "cbOverlappingMode");
			this.cbOverlappingMode.Name = "cbOverlappingMode";
			this.cbOverlappingMode.Properties.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.False;
			this.cbOverlappingMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbOverlappingMode.Properties.Buttons"))))});
			this.cbOverlappingMode.Properties.Items.AddRange(new object[] {
			resources.GetString("cbOverlappingMode.Properties.Items"),
			resources.GetString("cbOverlappingMode.Properties.Items1"),
			resources.GetString("cbOverlappingMode.Properties.Items2")});
			this.cbOverlappingMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbOverlappingMode.StyleController = this.layoutControl;
			resources.ApplyResources(this.ceShowPointLabels, "ceShowPointLabels");
			this.ceShowPointLabels.Name = "ceShowPointLabels";
			this.ceShowPointLabels.Properties.Caption = resources.GetString("ceShowPointLabels.Properties.Caption");
			this.ceShowPointLabels.StyleController = this.layoutControl;
			resources.ApplyResources(this.cbContentType, "cbContentType");
			this.cbContentType.Name = "cbContentType";
			this.cbContentType.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
			this.cbContentType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbContentType.Properties.Buttons"))))});
			this.cbContentType.Properties.Items.AddRange(new object[] {
			resources.GetString("cbContentType.Properties.Items"),
			resources.GetString("cbContentType.Properties.Items1"),
			resources.GetString("cbContentType.Properties.Items2"),
			resources.GetString("cbContentType.Properties.Items3")});
			this.cbContentType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbContentType.StyleController = this.layoutControl;
			resources.ApplyResources(this.ceShowForZeroValues, "ceShowForZeroValues");
			this.ceShowForZeroValues.Name = "ceShowForZeroValues";
			this.ceShowForZeroValues.Properties.Caption = resources.GetString("ceShowForZeroValues.Properties.Caption");
			this.ceShowForZeroValues.StyleController = this.layoutControl;
			resources.ApplyResources(this.seriesGallery, "seriesGallery");
			this.seriesGallery.Name = "seriesGallery";
			resources.ApplyResources(this.cePlotOnSecondaryAxis, "cePlotOnSecondaryAxis");
			this.cePlotOnSecondaryAxis.Name = "cePlotOnSecondaryAxis";
			this.cePlotOnSecondaryAxis.Properties.Caption = resources.GetString("cePlotOnSecondaryAxis.Properties.Caption");
			this.cePlotOnSecondaryAxis.StyleController = this.layoutControl;
			resources.ApplyResources(this.ceIgnoreEmptyPoints, "ceIgnoreEmptyPoints");
			this.ceIgnoreEmptyPoints.Name = "ceIgnoreEmptyPoints";
			this.ceIgnoreEmptyPoints.Properties.Caption = resources.GetString("ceIgnoreEmptyPoints.Properties.Caption");
			this.ceIgnoreEmptyPoints.StyleController = this.layoutControl;
			resources.ApplyResources(this.ceShowPointMarkers, "ceShowPointMarkers");
			this.ceShowPointMarkers.Name = "ceShowPointMarkers";
			this.ceShowPointMarkers.Properties.Caption = resources.GetString("ceShowPointMarkers.Properties.Caption");
			this.ceShowPointMarkers.StyleController = this.layoutControl;
			this.lcgRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgRoot.GroupBordersVisible = false;
			this.lcgRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciCancelButton,
			this.tabbedControlGroup,
			this.lciOkButton,
			this.emptySpaceItem,
			this.emptySpaceItem4});
			this.lcgRoot.Location = new System.Drawing.Point(0, 0);
			this.lcgRoot.Name = "Root";
			this.lcgRoot.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 12, 12, 12);
			this.lcgRoot.Size = new System.Drawing.Size(440, 521);
			this.lcgRoot.TextVisible = false;
			this.lciCancelButton.Control = this.btnCancel;
			this.lciCancelButton.Location = new System.Drawing.Point(341, 471);
			this.lciCancelButton.MaxSize = new System.Drawing.Size(75, 26);
			this.lciCancelButton.MinSize = new System.Drawing.Size(75, 26);
			this.lciCancelButton.Name = "layoutControlItem7";
			this.lciCancelButton.Size = new System.Drawing.Size(75, 26);
			this.lciCancelButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.lciCancelButton.TextSize = new System.Drawing.Size(0, 0);
			this.lciCancelButton.TextVisible = false;
			this.tabbedControlGroup.Location = new System.Drawing.Point(0, 0);
			this.tabbedControlGroup.Name = "tabbedControlGroup3";
			this.tabbedControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.tabbedControlGroup.SelectedTabPage = this.lcgPointLabelOptions;
			this.tabbedControlGroup.SelectedTabPageIndex = 2;
			this.tabbedControlGroup.Size = new System.Drawing.Size(416, 459);
			this.tabbedControlGroup.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.tabbedControlGroup.TabPages.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lcgSeriesType,
			this.lcgCommonOptions,
			this.lcgPointLabelOptions});
			this.lcgPointLabelOptions.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciShowPointLabels,
			this.emptySpaceItem3,
			this.lciPointLabelContentType,
			this.lciPointLabelOverlappingMode,
			this.lciPointLabelOrientation,
			this.lciShowForZeroValues,
			this.lciPosition,
			this.separatorPointLabelOptions,
			this.lblBarOptions,
			this.emptySpaceItem1,
			this.lblBubbleOptions,
			this.emptySpaceItem5});
			this.lcgPointLabelOptions.Location = new System.Drawing.Point(0, 0);
			this.lcgPointLabelOptions.Name = "lcgPointLabelOptions";
			this.lcgPointLabelOptions.Size = new System.Drawing.Size(410, 431);
			resources.ApplyResources(this.lcgPointLabelOptions, "lcgPointLabelOptions");
			this.lciShowPointLabels.Control = this.ceShowPointLabels;
			this.lciShowPointLabels.Location = new System.Drawing.Point(0, 0);
			this.lciShowPointLabels.Name = "lciShowPointLabels";
			this.lciShowPointLabels.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 20);
			this.lciShowPointLabels.Size = new System.Drawing.Size(410, 64);
			resources.ApplyResources(this.lciShowPointLabels, "lciShowPointLabels");
			this.lciShowPointLabels.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.lciShowPointLabels.TextSize = new System.Drawing.Size(110, 13);
			this.lciShowPointLabels.TextToControlDistance = 10;
			this.emptySpaceItem3.AllowHotTrack = false;
			this.emptySpaceItem3.Location = new System.Drawing.Point(0, 259);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Size = new System.Drawing.Size(410, 172);
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.lciPointLabelContentType.Control = this.cbContentType;
			this.lciPointLabelContentType.Location = new System.Drawing.Point(0, 64);
			this.lciPointLabelContentType.Name = "layoutControlItem9";
			this.lciPointLabelContentType.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.lciPointLabelContentType.Size = new System.Drawing.Size(410, 24);
			resources.ApplyResources(this.lciPointLabelContentType, "lciPointLabelContentType");
			this.lciPointLabelContentType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.lciPointLabelContentType.TextSize = new System.Drawing.Size(110, 13);
			this.lciPointLabelContentType.TextToControlDistance = 10;
			this.lciPointLabelOverlappingMode.Control = this.cbOverlappingMode;
			this.lciPointLabelOverlappingMode.Location = new System.Drawing.Point(0, 88);
			this.lciPointLabelOverlappingMode.Name = "layoutControlItem10";
			this.lciPointLabelOverlappingMode.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.lciPointLabelOverlappingMode.Size = new System.Drawing.Size(410, 24);
			resources.ApplyResources(this.lciPointLabelOverlappingMode, "lciPointLabelOverlappingMode");
			this.lciPointLabelOverlappingMode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.lciPointLabelOverlappingMode.TextSize = new System.Drawing.Size(110, 13);
			this.lciPointLabelOverlappingMode.TextToControlDistance = 10;
			this.lciPointLabelOrientation.Control = this.cbOrientation;
			this.lciPointLabelOrientation.Location = new System.Drawing.Point(0, 112);
			this.lciPointLabelOrientation.Name = "layoutControlItem11";
			this.lciPointLabelOrientation.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.lciPointLabelOrientation.Size = new System.Drawing.Size(410, 24);
			resources.ApplyResources(this.lciPointLabelOrientation, "lciPointLabelOrientation");
			this.lciPointLabelOrientation.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.lciPointLabelOrientation.TextSize = new System.Drawing.Size(110, 13);
			this.lciPointLabelOrientation.TextToControlDistance = 10;
			this.lciShowForZeroValues.Control = this.ceShowForZeroValues;
			this.lciShowForZeroValues.Location = new System.Drawing.Point(0, 212);
			this.lciShowForZeroValues.Name = "layoutControlItem8";
			this.lciShowForZeroValues.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.lciShowForZeroValues.Size = new System.Drawing.Size(410, 23);
			resources.ApplyResources(this.lciShowForZeroValues, "lciShowForZeroValues");
			this.lciShowForZeroValues.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.lciShowForZeroValues.TextSize = new System.Drawing.Size(110, 13);
			this.lciShowForZeroValues.TextToControlDistance = 10;
			this.lciPosition.Control = this.cbPosition;
			this.lciPosition.Location = new System.Drawing.Point(0, 235);
			this.lciPosition.Name = "layoutControlItem12";
			this.lciPosition.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.lciPosition.Size = new System.Drawing.Size(410, 24);
			resources.ApplyResources(this.lciPosition, "lciPosition");
			this.lciPosition.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.lciPosition.TextSize = new System.Drawing.Size(110, 13);
			this.lciPosition.TextToControlDistance = 10;
			this.separatorPointLabelOptions.AllowHotTrack = false;
			this.separatorPointLabelOptions.Location = new System.Drawing.Point(0, 197);
			this.separatorPointLabelOptions.Name = "simpleSeparator1";
			this.separatorPointLabelOptions.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.separatorPointLabelOptions.Size = new System.Drawing.Size(410, 2);
			this.lblBarOptions.AllowHotTrack = false;
			this.lblBarOptions.Location = new System.Drawing.Point(0, 163);
			this.lblBarOptions.Name = "lblBarOptions";
			this.lblBarOptions.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.lblBarOptions.Size = new System.Drawing.Size(410, 17);
			resources.ApplyResources(this.lblBarOptions, "lblBarOptions");
			this.lblBarOptions.TextSize = new System.Drawing.Size(70, 13);
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 136);
			this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 27);
			this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 27);
			this.emptySpaceItem1.Name = "item0";
			this.emptySpaceItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.emptySpaceItem1.Size = new System.Drawing.Size(410, 27);
			this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.lblBubbleOptions.AllowHotTrack = false;
			resources.ApplyResources(this.lblBubbleOptions, "lblBubbleOptions");
			this.lblBubbleOptions.Location = new System.Drawing.Point(0, 180);
			this.lblBubbleOptions.Name = "lblBubbleOptions";
			this.lblBubbleOptions.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.lblBubbleOptions.Size = new System.Drawing.Size(410, 17);
			this.lblBubbleOptions.TextSize = new System.Drawing.Size(70, 13);
			this.lcgSeriesType.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem5});
			this.lcgSeriesType.Location = new System.Drawing.Point(0, 0);
			this.lcgSeriesType.Name = "lcgSeriesType";
			this.lcgSeriesType.Size = new System.Drawing.Size(410, 431);
			this.lcgSeriesType.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			resources.ApplyResources(this.lcgSeriesType, "lcgSeriesType");
			this.layoutControlItem5.Control = this.seriesGallery;
			this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem5.MinSize = new System.Drawing.Size(50, 12);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem5.Size = new System.Drawing.Size(410, 431);
			this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.lcgCommonOptions.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciShowPointMarkers,
			this.lciIgnoreEmptyPoints,
			this.lciPlotOnSecondaryAxis,
			this.emptySpaceItem2});
			this.lcgCommonOptions.Location = new System.Drawing.Point(0, 0);
			this.lcgCommonOptions.Name = "lcgCommonOptions";
			this.lcgCommonOptions.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.lcgCommonOptions.Size = new System.Drawing.Size(410, 431);
			this.lcgCommonOptions.Spacing = new DevExpress.XtraLayout.Utils.Padding(50, 50, 50, 50);
			resources.ApplyResources(this.lcgCommonOptions, "lcgCommonOptions");
			this.lciShowPointMarkers.Control = this.ceShowPointMarkers;
			this.lciShowPointMarkers.Location = new System.Drawing.Point(0, 69);
			this.lciShowPointMarkers.Name = "layoutControlItem3";
			this.lciShowPointMarkers.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.lciShowPointMarkers.Size = new System.Drawing.Size(410, 23);
			this.lciShowPointMarkers.TextSize = new System.Drawing.Size(0, 0);
			this.lciShowPointMarkers.TextVisible = false;
			this.lciIgnoreEmptyPoints.Control = this.ceIgnoreEmptyPoints;
			this.lciIgnoreEmptyPoints.Location = new System.Drawing.Point(0, 46);
			this.lciIgnoreEmptyPoints.Name = "layoutControlItem2";
			this.lciIgnoreEmptyPoints.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.lciIgnoreEmptyPoints.Size = new System.Drawing.Size(410, 23);
			this.lciIgnoreEmptyPoints.TextSize = new System.Drawing.Size(0, 0);
			this.lciIgnoreEmptyPoints.TextVisible = false;
			this.lciPlotOnSecondaryAxis.Control = this.cePlotOnSecondaryAxis;
			this.lciPlotOnSecondaryAxis.Location = new System.Drawing.Point(0, 0);
			this.lciPlotOnSecondaryAxis.Name = "layoutControlItem1";
			this.lciPlotOnSecondaryAxis.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 2);
			this.lciPlotOnSecondaryAxis.Size = new System.Drawing.Size(410, 46);
			this.lciPlotOnSecondaryAxis.TextSize = new System.Drawing.Size(0, 0);
			this.lciPlotOnSecondaryAxis.TextVisible = false;
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(0, 92);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(410, 339);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.lciOkButton.Control = this.btnOK;
			this.lciOkButton.Location = new System.Drawing.Point(266, 471);
			this.lciOkButton.MaxSize = new System.Drawing.Size(75, 26);
			this.lciOkButton.MinSize = new System.Drawing.Size(75, 26);
			this.lciOkButton.Name = "layoutControlItem6";
			this.lciOkButton.Size = new System.Drawing.Size(75, 26);
			this.lciOkButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.lciOkButton.TextSize = new System.Drawing.Size(0, 0);
			this.lciOkButton.TextVisible = false;
			this.emptySpaceItem.AllowHotTrack = false;
			this.emptySpaceItem.Location = new System.Drawing.Point(0, 459);
			this.emptySpaceItem.MaxSize = new System.Drawing.Size(0, 12);
			this.emptySpaceItem.MinSize = new System.Drawing.Size(104, 12);
			this.emptySpaceItem.Name = "emptySpaceItem1";
			this.emptySpaceItem.Size = new System.Drawing.Size(416, 12);
			this.emptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem4.AllowHotTrack = false;
			this.emptySpaceItem4.Location = new System.Drawing.Point(0, 471);
			this.emptySpaceItem4.MaxSize = new System.Drawing.Size(0, 26);
			this.emptySpaceItem4.MinSize = new System.Drawing.Size(10, 26);
			this.emptySpaceItem4.Name = "emptySpaceItem4";
			this.emptySpaceItem4.Size = new System.Drawing.Size(266, 26);
			this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
			this.barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.emptySpaceItem5.AllowHotTrack = false;
			this.emptySpaceItem5.Location = new System.Drawing.Point(0, 199);
			this.emptySpaceItem5.Name = "emptySpaceItem5";
			this.emptySpaceItem5.Size = new System.Drawing.Size(410, 13);
			this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.layoutControl);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChartSeriesOptionsForm";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
			this.layoutControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOrientation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOverlappingMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowPointLabels.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbContentType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowForZeroValues.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cePlotOnSecondaryAxis.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceIgnoreEmptyPoints.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowPointMarkers.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgRoot)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancelButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabbedControlGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgPointLabelOptions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciShowPointLabels)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPointLabelContentType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPointLabelOverlappingMode)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPointLabelOrientation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciShowForZeroValues)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPosition)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separatorPointLabelOptions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lblBarOptions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lblBubbleOptions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgSeriesType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgCommonOptions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciShowPointMarkers)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciIgnoreEmptyPoints)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPlotOnSecondaryAxis)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOkButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private XtraBars.BarAndDockingController barAndDockingController1;
		private XtraEditors.CheckEdit ceShowPointMarkers;
		private XtraEditors.CheckEdit ceIgnoreEmptyPoints;
		private XtraEditors.CheckEdit cePlotOnSecondaryAxis;
		private ChartSeriesGalleryControl seriesGallery;
		private XtraLayout.LayoutControl layoutControl;
		private XtraLayout.LayoutControlGroup lcgRoot;
		private XtraLayout.LayoutControlItem lciOkButton;
		private XtraLayout.LayoutControlItem lciCancelButton;
		private XtraLayout.EmptySpaceItem emptySpaceItem;
		private XtraLayout.TabbedControlGroup tabbedControlGroup;
		private XtraLayout.LayoutControlGroup lcgCommonOptions;
		private XtraLayout.LayoutControlItem lciShowPointMarkers;
		private XtraLayout.LayoutControlItem lciIgnoreEmptyPoints;
		private XtraLayout.LayoutControlItem lciPlotOnSecondaryAxis;
		private XtraLayout.LayoutControlGroup lcgPointLabelOptions;
		private XtraEditors.CheckEdit ceShowPointLabels;
		private XtraLayout.LayoutControlItem lciShowPointLabels;
		private XtraLayout.EmptySpaceItem emptySpaceItem3;
		private XtraEditors.CheckEdit ceShowForZeroValues;
		private XtraLayout.LayoutControlItem lciShowForZeroValues;
		private XtraEditors.ComboBoxEdit cbContentType;
		private XtraLayout.LayoutControlItem lciPointLabelContentType;
		private XtraEditors.ComboBoxEdit cbOverlappingMode;
		private XtraLayout.LayoutControlItem lciPointLabelOverlappingMode;
		private XtraEditors.ComboBoxEdit cbOrientation;
		private XtraLayout.LayoutControlItem lciPointLabelOrientation;
		private XtraEditors.ComboBoxEdit cbPosition;
		private XtraLayout.LayoutControlItem lciPosition;
		private XtraLayout.EmptySpaceItem emptySpaceItem2;
		private XtraLayout.SimpleSeparator separatorPointLabelOptions;
		private XtraLayout.SimpleLabelItem lblBarOptions;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.EmptySpaceItem emptySpaceItem4;
		private XtraLayout.LayoutControlGroup lcgSeriesType;
		private XtraLayout.LayoutControlItem layoutControlItem5;
		private XtraBars.BarManager barManager1;
		private XtraBars.BarDockControl barDockControlTop;
		private XtraBars.BarDockControl barDockControlBottom;
		private XtraBars.BarDockControl barDockControlLeft;
		private XtraBars.BarDockControl barDockControlRight;
		private XtraLayout.SimpleLabelItem lblBubbleOptions;
		private XtraLayout.EmptySpaceItem emptySpaceItem5;
	}
}
