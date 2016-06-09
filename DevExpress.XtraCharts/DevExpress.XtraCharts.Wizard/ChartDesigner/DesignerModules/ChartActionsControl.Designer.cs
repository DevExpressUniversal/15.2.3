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
namespace DevExpress.XtraCharts.Designer
{
	public partial class ChartActionsControl 
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartActionsControl));
			this.galleryControlGallery1 = new DevExpress.XtraBars.Ribbon.Gallery.GalleryControlGallery();
			this.galleryControlGallery2 = new DevExpress.XtraBars.Ribbon.Gallery.GalleryControlGallery();
			this.btnUndo = new DevExpress.XtraCharts.Designer.Native.DropDownButtonFlat();
			this.btnRedo = new DevExpress.XtraCharts.Designer.Native.DropDownButtonFlat();
			this.btnChangeType = new DevExpress.XtraCharts.Designer.Native.DropDownButtonFlat();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.btnAddElement = new DevExpress.XtraCharts.Designer.Native.DropDownButtonFlat();
			this.ppmAddElements = new DevExpress.XtraBars.PopupMenu(this.components);
			this.bhiChartElements = new DevExpress.XtraBars.BarHeaderItem();
			this.bbiAddSeries = new DevExpress.XtraBars.BarButtonItem();
			this.bbiAddPane = new DevExpress.XtraBars.BarButtonItem();
			this.bbiAddAxisX = new DevExpress.XtraBars.BarButtonItem();
			this.bbiAddAxisY = new DevExpress.XtraBars.BarButtonItem();
			this.bbiAddChartTitle = new DevExpress.XtraBars.BarButtonItem();
			this.bbiAddAnnotation = new DevExpress.XtraBars.BarButtonItem();
			this.bhiSeriesElements = new DevExpress.XtraBars.BarHeaderItem();
			this.bbiAddIndicator = new DevExpress.XtraBars.BarButtonItem();
			this.bbiAddSeriesTitle = new DevExpress.XtraBars.BarButtonItem();
			this.bhiAxisElements = new DevExpress.XtraBars.BarHeaderItem();
			this.bbiAddConstantLine = new DevExpress.XtraBars.BarButtonItem();
			this.bbiAddStrip = new DevExpress.XtraBars.BarButtonItem();
			this.bbiAddScaleBreak = new DevExpress.XtraBars.BarButtonItem();
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
			this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
			this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
			this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
			this.barButtonItem5 = new DevExpress.XtraBars.BarButtonItem();
			this.barSubItem1 = new DevExpress.XtraBars.BarSubItem();
			this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
			this.barStaticItem3 = new DevExpress.XtraBars.BarStaticItem();
			this.barStaticItem4 = new DevExpress.XtraBars.BarStaticItem();
			this.bbiSmartBinding = new DevExpress.XtraBars.BarButtonItem();
			this.bbiSeriesBinding = new DevExpress.XtraBars.BarButtonItem();
			this.btnData = new DevExpress.XtraCharts.Designer.Native.DropDownButtonFlat();
			this.ppmDataBinding = new DevExpress.XtraBars.PopupMenu(this.components);
			((System.ComponentModel.ISupportInitialize)(this.ppmAddElements)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ppmDataBinding)).BeginInit();
			this.SuspendLayout();
			this.btnUndo.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
			this.btnUndo.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.Image")));
			this.btnUndo.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnUndo, "btnUndo");
			this.btnUndo.Name = "btnUndo";
			this.btnUndo.Click += new System.EventHandler(this.OnUndoClick);
			this.btnRedo.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
			this.btnRedo.Image = ((System.Drawing.Image)(resources.GetObject("btnRedo.Image")));
			this.btnRedo.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnRedo, "btnRedo");
			this.btnRedo.Name = "btnRedo";
			this.btnRedo.Click += new System.EventHandler(this.OnRedoClick);
			resources.ApplyResources(this.btnChangeType, "btnChangeType");
			this.btnChangeType.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
			this.btnChangeType.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
			this.btnChangeType.Image = ((System.Drawing.Image)(resources.GetObject("btnChangeType.Image")));
			this.btnChangeType.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnChangeType.Name = "btnChangeType";
			this.btnChangeType.Click += new System.EventHandler(this.OnChangeTypeClick);
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			this.btnAddElement.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
			this.btnAddElement.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
			this.btnAddElement.DropDownControl = this.ppmAddElements;
			this.btnAddElement.Image = ((System.Drawing.Image)(resources.GetObject("btnAddElement.Image")));
			this.btnAddElement.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnAddElement, "btnAddElement");
			this.btnAddElement.Name = "btnAddElement";
			this.ppmAddElements.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.bhiChartElements),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.bbiAddSeries, DevExpress.XtraBars.BarItemPaintStyle.Standard),
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiAddPane),
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiAddAxisX),
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiAddAxisY),
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiAddChartTitle),
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiAddAnnotation),
			new DevExpress.XtraBars.LinkPersistInfo(this.bhiSeriesElements),
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiAddIndicator),
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiAddSeriesTitle),
			new DevExpress.XtraBars.LinkPersistInfo(this.bhiAxisElements),
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiAddConstantLine),
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiAddStrip),
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiAddScaleBreak)});
			this.ppmAddElements.Manager = this.barManager1;
			this.ppmAddElements.MinWidth = 170;
			this.ppmAddElements.Name = "ppmAddElements";
			resources.ApplyResources(this.bhiChartElements, "bhiChartElements");
			this.bhiChartElements.Id = 16;
			this.bhiChartElements.Name = "bhiChartElements";
			resources.ApplyResources(this.bbiAddSeries, "bbiAddSeries");
			this.bbiAddSeries.Id = 10;
			this.bbiAddSeries.Name = "bbiAddSeries";
			this.bbiAddSeries.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddSeriesItemClick);
			resources.ApplyResources(this.bbiAddPane, "bbiAddPane");
			this.bbiAddPane.Id = 17;
			this.bbiAddPane.Name = "bbiAddPane";
			this.bbiAddPane.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddPaneItemClick);
			resources.ApplyResources(this.bbiAddAxisX, "bbiAddAxisX");
			this.bbiAddAxisX.Id = 18;
			this.bbiAddAxisX.Name = "bbiAddAxisX";
			this.bbiAddAxisX.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddAxisXItemClick);
			resources.ApplyResources(this.bbiAddAxisY, "bbiAddAxisY");
			this.bbiAddAxisY.Id = 19;
			this.bbiAddAxisY.Name = "bbiAddAxisY";
			this.bbiAddAxisY.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddAxisYItemClick);
			resources.ApplyResources(this.bbiAddChartTitle, "bbiAddChartTitle");
			this.bbiAddChartTitle.Id = 11;
			this.bbiAddChartTitle.Name = "bbiAddChartTitle";
			this.bbiAddChartTitle.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddChartTitleItemClick);
			resources.ApplyResources(this.bbiAddAnnotation, "bbiAddAnnotation");
			this.bbiAddAnnotation.Id = 12;
			this.bbiAddAnnotation.Name = "bbiAddAnnotation";
			this.bbiAddAnnotation.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddAnnotationItemClick);
			resources.ApplyResources(this.bhiSeriesElements, "bhiSeriesElements");
			this.bhiSeriesElements.Id = 13;
			this.bhiSeriesElements.Name = "bhiSeriesElements";
			resources.ApplyResources(this.bbiAddIndicator, "bbiAddIndicator");
			this.bbiAddIndicator.Id = 14;
			this.bbiAddIndicator.Name = "bbiAddIndicator";
			this.bbiAddIndicator.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddIndicatorItemClick);
			resources.ApplyResources(this.bbiAddSeriesTitle, "bbiAddSeriesTitle");
			this.bbiAddSeriesTitle.Id = 15;
			this.bbiAddSeriesTitle.Name = "bbiAddSeriesTitle";
			this.bbiAddSeriesTitle.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddSeriesTitleItemClick);
			resources.ApplyResources(this.bhiAxisElements, "bhiAxisElements");
			this.bhiAxisElements.Id = 20;
			this.bhiAxisElements.Name = "bhiAxisElements";
			resources.ApplyResources(this.bbiAddConstantLine, "bbiAddConstantLine");
			this.bbiAddConstantLine.Id = 21;
			this.bbiAddConstantLine.Name = "bbiAddConstantLine";
			this.bbiAddConstantLine.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddConstantLineItemClick);
			resources.ApplyResources(this.bbiAddStrip, "bbiAddStrip");
			this.bbiAddStrip.Id = 22;
			this.bbiAddStrip.Name = "bbiAddStrip";
			this.bbiAddStrip.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddStripItemClick);
			resources.ApplyResources(this.bbiAddScaleBreak, "bbiAddScaleBreak");
			this.bbiAddScaleBreak.Id = 23;
			this.bbiAddScaleBreak.Name = "bbiAddScaleBreak";
			this.bbiAddScaleBreak.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnAddScaleBreakItemClick);
			this.barManager1.DockControls.Add(this.barDockControlTop);
			this.barManager1.DockControls.Add(this.barDockControlBottom);
			this.barManager1.DockControls.Add(this.barDockControlLeft);
			this.barManager1.DockControls.Add(this.barDockControlRight);
			this.barManager1.Form = this;
			this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.barButtonItem1,
			this.barButtonItem2,
			this.barStaticItem1,
			this.barButtonItem3,
			this.barButtonItem4,
			this.barButtonItem5,
			this.barSubItem1,
			this.barStaticItem2,
			this.barStaticItem3,
			this.barStaticItem4,
			this.bbiAddSeries,
			this.bbiAddChartTitle,
			this.bbiAddAnnotation,
			this.bhiSeriesElements,
			this.bbiAddIndicator,
			this.bbiAddSeriesTitle,
			this.bhiChartElements,
			this.bbiAddPane,
			this.bbiAddAxisX,
			this.bbiAddAxisY,
			this.bhiAxisElements,
			this.bbiAddConstantLine,
			this.bbiAddStrip,
			this.bbiAddScaleBreak,
			this.bbiSmartBinding,
			this.bbiSeriesBinding});
			this.barManager1.MaxItemId = 26;
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.barButtonItem1.Glyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.Glyph")));
			this.barButtonItem1.Id = 0;
			this.barButtonItem1.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.LargeGlyph")));
			this.barButtonItem1.Name = "barButtonItem1";
			this.barButtonItem2.Glyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.Glyph")));
			this.barButtonItem2.Id = 1;
			this.barButtonItem2.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.LargeGlyph")));
			this.barButtonItem2.Name = "barButtonItem2";
			this.barStaticItem1.Id = 2;
			this.barStaticItem1.Name = "barStaticItem1";
			this.barStaticItem1.TextAlignment = System.Drawing.StringAlignment.Near;
			this.barButtonItem3.Glyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem3.Glyph")));
			this.barButtonItem3.Id = 3;
			this.barButtonItem3.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem3.LargeGlyph")));
			this.barButtonItem3.Name = "barButtonItem3";
			this.barButtonItem4.Glyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem4.Glyph")));
			this.barButtonItem4.Id = 4;
			this.barButtonItem4.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem4.LargeGlyph")));
			this.barButtonItem4.Name = "barButtonItem4";
			this.barButtonItem5.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
			this.barButtonItem5.Glyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem5.Glyph")));
			this.barButtonItem5.Id = 5;
			this.barButtonItem5.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem5.LargeGlyph")));
			this.barButtonItem5.Name = "barButtonItem5";
			resources.ApplyResources(this.barSubItem1, "barSubItem1");
			this.barSubItem1.Id = 6;
			this.barSubItem1.Name = "barSubItem1";
			resources.ApplyResources(this.barStaticItem2, "barStaticItem2");
			this.barStaticItem2.Id = 7;
			this.barStaticItem2.Name = "barStaticItem2";
			this.barStaticItem2.TextAlignment = System.Drawing.StringAlignment.Near;
			resources.ApplyResources(this.barStaticItem3, "barStaticItem3");
			this.barStaticItem3.Id = 8;
			this.barStaticItem3.Name = "barStaticItem3";
			this.barStaticItem3.TextAlignment = System.Drawing.StringAlignment.Near;
			resources.ApplyResources(this.barStaticItem4, "barStaticItem4");
			this.barStaticItem4.Id = 9;
			this.barStaticItem4.Name = "barStaticItem4";
			this.barStaticItem4.TextAlignment = System.Drawing.StringAlignment.Near;
			resources.ApplyResources(this.bbiSmartBinding, "bbiSmartBinding");
			this.bbiSmartBinding.Id = 24;
			this.bbiSmartBinding.Name = "bbiSmartBinding";
			this.bbiSmartBinding.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnSmartBindingItemClick);
			resources.ApplyResources(this.bbiSeriesBinding, "bbiSeriesBinding");
			this.bbiSeriesBinding.Id = 25;
			this.bbiSeriesBinding.Name = "bbiSeriesBinding";
			this.bbiSeriesBinding.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnSeriesBindingItemClick);
			this.btnData.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
			this.btnData.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
			this.btnData.DropDownControl = this.ppmDataBinding;
			this.btnData.Image = ((System.Drawing.Image)(resources.GetObject("btnData.Image")));
			this.btnData.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnData, "btnData");
			this.btnData.Name = "btnData";
			this.ppmDataBinding.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiSmartBinding),
			new DevExpress.XtraBars.LinkPersistInfo(this.bbiSeriesBinding)});
			this.ppmDataBinding.Manager = this.barManager1;
			this.ppmDataBinding.MinWidth = 170;
			this.ppmDataBinding.Name = "ppmDataBinding";
			this.Controls.Add(this.btnData);
			this.Controls.Add(this.btnAddElement);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.btnChangeType);
			this.Controls.Add(this.btnRedo);
			this.Controls.Add(this.btnUndo);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "ChartActionsControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.ppmAddElements)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ppmDataBinding)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion        
		private XtraBars.Ribbon.Gallery.GalleryControlGallery galleryControlGallery1;
		private XtraBars.Ribbon.Gallery.GalleryControlGallery galleryControlGallery2;
		private DevExpress.XtraCharts.Designer.Native.DropDownButtonFlat btnUndo;
		private DevExpress.XtraCharts.Designer.Native.DropDownButtonFlat btnRedo;
		private DevExpress.XtraCharts.Designer.Native.DropDownButtonFlat btnChangeType;
		private XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraCharts.Designer.Native.DropDownButtonFlat btnAddElement;
		private DevExpress.XtraCharts.Designer.Native.DropDownButtonFlat btnData;
		private XtraBars.PopupMenu ppmAddElements;
		private XtraBars.BarManager barManager1;
		private XtraBars.BarButtonItem barButtonItem1;
		private XtraBars.BarButtonItem barButtonItem2;
		private XtraBars.BarStaticItem barStaticItem1;
		private XtraBars.BarButtonItem barButtonItem3;
		private XtraBars.BarDockControl barDockControlTop;
		private XtraBars.BarDockControl barDockControlBottom;
		private XtraBars.BarDockControl barDockControlLeft;
		private XtraBars.BarDockControl barDockControlRight;
		private XtraBars.BarButtonItem barButtonItem4;
		private XtraBars.BarButtonItem barButtonItem5;
		private XtraBars.BarStaticItem barStaticItem2;
		private XtraBars.BarStaticItem barStaticItem3;
		private XtraBars.BarStaticItem barStaticItem4;
		private XtraBars.BarSubItem barSubItem1;
		private XtraBars.BarButtonItem bbiAddSeries;
		private XtraBars.BarButtonItem bbiAddChartTitle;
		private XtraBars.BarButtonItem bbiAddAnnotation;
		private XtraBars.BarHeaderItem bhiSeriesElements;
		private XtraBars.BarButtonItem bbiAddIndicator;
		private XtraBars.BarButtonItem bbiAddSeriesTitle;
		private XtraBars.BarHeaderItem bhiChartElements;
		private XtraBars.BarButtonItem bbiAddPane;
		private XtraBars.BarButtonItem bbiAddAxisX;
		private XtraBars.BarButtonItem bbiAddAxisY;
		private XtraBars.BarHeaderItem bhiAxisElements;
		private XtraBars.BarButtonItem bbiAddConstantLine;
		private XtraBars.BarButtonItem bbiAddStrip;
		private XtraBars.BarButtonItem bbiAddScaleBreak;
		private XtraBars.BarButtonItem bbiSmartBinding;
		private XtraBars.BarButtonItem bbiSeriesBinding;
		private XtraBars.PopupMenu ppmDataBinding;
	}
}
