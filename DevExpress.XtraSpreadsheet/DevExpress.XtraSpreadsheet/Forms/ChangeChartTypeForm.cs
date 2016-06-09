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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.UI;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region ChangeChartTypeForm
	public partial class ChangeChartTypeForm : XtraForm {
		readonly ChangeChartTypeViewModel viewModel;
		ChangeChartTypeForm() {
			InitializeComponent();
		}
		public ChangeChartTypeForm(ChangeChartTypeViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			InitializeGalleryControl();
			InitializeListBoxControl();
			galleryControl.Gallery.ItemClick += OnGalleryItemClick;
			galleryControl.Gallery.ItemDoubleClick += OnGalleryItemDoubleClick;
			galleryControl.Gallery.ItemCheckedChanged += OnGalleryItemCheckedChanged;
			lbGroups.SelectedIndexChanged += OnGroupSelectedIndexChanged;
		}
		public ChangeChartTypeViewModel ViewModel { get { return viewModel; } }
		public SpreadsheetControl Control { get { return (SpreadsheetControl)viewModel.Control; } }
		void InitializeGalleryControl() {
			StandaloneGallery gallery = galleryControl.Gallery;
			gallery.BeginUpdate();
			try {
				gallery.ImageSize = new Size(32, 32);
				PopulateGallery(gallery);
				ControlCommandBarGalleryWalker<SpreadsheetControl, SpreadsheetCommandId> walker = new ControlCommandBarGalleryWalker<SpreadsheetControl, SpreadsheetCommandId>();
				walker.SetGroupsControl(gallery, Control);
				walker.SetItemsControl(gallery, Control);
				walker.UpdateGroupsCaption(gallery);
				walker.UpdateItemsCaption(gallery);
				walker.UpdateItemsImages(gallery);
			}
			finally {
				gallery.EndUpdate();
			}
		}
		class GroupListBoxItem {
			ControlCommandGalleryItemGroup<SpreadsheetControl, SpreadsheetCommandId> group;
			public GroupListBoxItem(ControlCommandGalleryItemGroup<SpreadsheetControl, SpreadsheetCommandId> group) {
				this.group = group;
			}
			public ControlCommandGalleryItemGroup<SpreadsheetControl, SpreadsheetCommandId> Group { get { return group; } }
			public override string ToString() {
				return group.Caption;
			}
		}
		void InitializeListBoxControl() {
			lbGroups.BeginUpdate();
			try {
				ImageCollection images = new ImageCollection();
				lbGroups.ImageList = images;
				StandaloneGallery gallery = galleryControl.Gallery;
				GalleryItemGroupCollection groups = gallery.Groups;
				int index = 0;
				int count = groups.Count;
				for (int i = 0; i < count; i++) {
					ControlCommandGalleryItemGroup<SpreadsheetControl, SpreadsheetCommandId> group = groups[i] as ControlCommandGalleryItemGroup<SpreadsheetControl, SpreadsheetCommandId>;
					if (group != null) {
						lbGroups.Items.Add(new GroupListBoxItem(group), index);
						images.AddImage(Control.CreateCommand(group.GetCommandId()).Image);
						index++;
					}
				}
			}
			finally {
				lbGroups.EndUpdate();
			}
		}
		#region PopulateGallery
		void PopulateGallery(StandaloneGallery gallery) {
			GalleryItemGroupCollection groups = gallery.Groups;
			groups.Add(PopulateColumnsGalleryGroup());
			groups.Add(PopulateLineGalleryGroup());
			groups.Add(PopulatePieGalleryGroup());
			groups.Add(PopulateBarGalleryGroup());
			groups.Add(PopulateAreaGalleryGroup());
			groups.Add(PopulateScatterGalleryGroup());
			groups.Add(PopulateStockGalleryGroup());
			groups.Add(PopulateDoughnutGalleryGroup());
			groups.Add(PopulateBubbleGalleryGroup());
			groups.Add(PopulateRadarGalleryGroup());
		}
		SpreadsheetCommandGalleryItemGroup PopulateColumnsGalleryGroup() {
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartColumnCommandGroup);
			GalleryItemCollection items = group.Items;
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnClustered2D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnStacked2D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnPercentStacked2D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnClustered3D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnStacked3D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumnPercentStacked3D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartColumn3D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartCylinderClustered));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartCylinderStacked));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartCylinderPercentStacked));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartCylinder));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartConeClustered));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartConeStacked));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartConePercentStacked));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartCone));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPyramidClustered));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPyramidStacked));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPyramidPercentStacked));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPyramid));
			return group;
		}
		SpreadsheetCommandGalleryItemGroup PopulateLineGalleryGroup() {
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartLineCommandGroup);
			GalleryItemCollection items = group.Items;
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartLine));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStackedLine));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPercentStackedLine));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartLineWithMarkers));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStackedLineWithMarkers));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPercentStackedLineWithMarkers));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartLine3D));
			return group;
		}
		SpreadsheetCommandGalleryItemGroup PopulatePieGalleryGroup() {
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartPieCommandGroup);
			GalleryItemCollection items = group.Items;
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPie2D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPieExploded2D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPie3D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPieExploded3D));
			return group;
		}
		SpreadsheetCommandGalleryItemGroup PopulateBarGalleryGroup() {
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartBarCommandGroup);
			GalleryItemCollection items = group.Items;
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarClustered2D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarStacked2D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarPercentStacked2D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarClustered3D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarStacked3D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBarPercentStacked3D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalCylinderClustered));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalCylinderStacked));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalCylinderPercentStacked));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalConeClustered));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalConeStacked));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalConePercentStacked));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalPyramidClustered));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalPyramidStacked));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartHorizontalPyramidPercentStacked));
			return group;
		}
		SpreadsheetCommandGalleryItemGroup PopulateAreaGalleryGroup() {
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartAreaCommandGroup);
			GalleryItemCollection items = group.Items;
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartArea));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStackedArea));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPercentStackedArea));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartArea3D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStackedArea3D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartPercentStackedArea3D));
			return group;
		}
		SpreadsheetCommandGalleryItemGroup PopulateScatterGalleryGroup() {
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartScatterCommandGroup);
			GalleryItemCollection items = group.Items;
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartScatterMarkers));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartScatterSmoothLinesAndMarkers));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartScatterSmoothLines));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartScatterLinesAndMarkers));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartScatterLines));
			return group;
		}
		SpreadsheetCommandGalleryItemGroup PopulateStockGalleryGroup() {
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartStockCommandGroup);
			GalleryItemCollection items = group.Items;
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStockHighLowClose));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartStockOpenHighLowClose));
			return group;
		}
		SpreadsheetCommandGalleryItemGroup PopulateDoughnutGalleryGroup() {
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartDoughnut2DCommandGroup);
			GalleryItemCollection items = group.Items;
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartDoughnut2D));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartDoughnutExploded2D));
			return group;
		}
		SpreadsheetCommandGalleryItemGroup PopulateBubbleGalleryGroup() {
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartBubbleCommandGroup);
			GalleryItemCollection items = group.Items;
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBubble));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartBubble3D));
			return group;
		}
		SpreadsheetCommandGalleryItemGroup PopulateRadarGalleryGroup() {
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.InsertChartRadarCommandGroup);
			GalleryItemCollection items = group.Items;
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartRadar));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartRadarWithMarkers));
			items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.InsertChartRadarFilled));
			return group;
		}
		#endregion
		void OnGalleryItemClick(object sender, GalleryItemClickEventArgs e) {
			AssignCommandId(e.Item);
		}
		void OnGalleryItemDoubleClick(object sender, GalleryItemClickEventArgs e) {
			if (AssignCommandId(e.Item))
				Commit();
		}
		void OnGalleryItemCheckedChanged(object sender, GalleryItemEventArgs e) {
			if (e.Item.Checked)
				AssignCommandId(e.Item);
		}
		bool AssignCommandId(GalleryItem galleryItem) {
			ControlCommandGalleryItem<SpreadsheetControl, SpreadsheetCommandId> item = galleryItem as ControlCommandGalleryItem<SpreadsheetControl, SpreadsheetCommandId>;
			if (item == null)
				return false;
			viewModel.CommandId = item.GetCommandId();
			return true;
		}
		void OnGroupSelectedIndexChanged(object sender, EventArgs e) {
			ImageListBoxItem item = lbGroups.SelectedItem as ImageListBoxItem;
			if (item == null)
				return;
			GroupListBoxItem groupItem = item.Value as GroupListBoxItem;
			if (groupItem == null)
				return;
			ControlCommandGalleryItemGroup<SpreadsheetControl, SpreadsheetCommandId> group = groupItem.Group;
			if (group.Items.Count > 0)
				group.Items[0].Checked = true;
			galleryControl.Gallery.ScrollTo(group, false);
		}
		void btnOk_Click(object sender, EventArgs e) {
			Commit();
		}
		void Commit() {
			if (ViewModel != null)
				ViewModel.ApplyChanges();
			DialogResult = DialogResult.OK;
		}
	}
	#endregion
}
