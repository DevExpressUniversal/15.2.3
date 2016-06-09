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

using System.Windows.Forms;
using System;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraReports.Wizards3.Localization;
using System.Linq;
using DevExpress.XtraBars.Ribbon;
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Wizards3.Views {
	[System.ComponentModel.ToolboxItem(false)]
	public partial class ChooseReportTypePageView : WizardViewBase, IChooseReportTypePageViewExtended {
		Dictionary<ReportType, GalleryItem> itemsRepository;
		public override string HeaderDescription {
			get { return ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_ChooseReportType_Description); }
		}
		public ReportType ReportType {
			get {
				var checkedItem = reportTypeGallery.Gallery.GetCheckedItems().FirstOrDefault();
				return checkedItem != null ? (ReportType) checkedItem.Tag : ReportType.Empty;
			}
			set {
				DevExpress.XtraBars.Ribbon.GalleryItem galleryItem;
				if(itemsRepository.TryGetValue(value, out galleryItem))
					galleryItem.Checked = true;
			}
		}
		public event EventHandler ReportTypeChanged;
		public ChooseReportTypePageView() {
			InitializeComponent();
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			InitializeGallery(galleryItemGroup);
			this.reportTypeGallery.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
				galleryItemGroup
			});
			itemsRepository = new Dictionary<ReportType, GalleryItem>();
			foreach(var item in reportTypeGallery.Gallery.GetAllItems()) {
				itemsRepository[(ReportType) item.Tag] = item;
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			reportTypeGallery.LookAndFeel.SetSkinStyle(reportTypeGallery.LookAndFeel.ActiveSkinName);
		}
		protected virtual void InitializeGallery(DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup) {
			DevExpress.XtraBars.Ribbon.GalleryItem galleryItem1 = new DevExpress.XtraBars.Ribbon.GalleryItem();
			DevExpress.XtraBars.Ribbon.GalleryItem galleryItem2 = new DevExpress.XtraBars.Ribbon.GalleryItem();
			DevExpress.XtraBars.Ribbon.GalleryItem galleryItem3 = new DevExpress.XtraBars.Ribbon.GalleryItem();
			galleryItemGroup.CaptionAlignment = DevExpress.XtraBars.Ribbon.GalleryItemGroupCaptionAlignment.Center;
			galleryItem1.Description = "Empty Report";
			galleryItem1.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.EmptyReport.png"), LocalResFinder.Assembly);
			galleryItem1.Tag = 2;
			galleryItem2.Description = "Data-bound Report";
			galleryItem2.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.DataboundReport.png"), LocalResFinder.Assembly);
			galleryItem2.Tag = 0;
			galleryItem3.Description = "Label Report";
			galleryItem3.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.labelReport.png"), LocalResFinder.Assembly);
			galleryItem3.Tag = 1;
			galleryItemGroup.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[] {
				galleryItem1,
				galleryItem2,
				galleryItem3
			});
		}
		void galleryControlGallery1_ItemCheckedChanged(object sender, GalleryItemEventArgs e) {
			if(!e.Item.Checked)
				return;
			if(ReportTypeChanged != null)
				ReportTypeChanged(this, EventArgs.Empty);
		}
		void reportTypeGallery_Gallery_ItemDoubleClick(object sender, GalleryItemClickEventArgs e) {
			this.MoveForward();
		}
	}
}
