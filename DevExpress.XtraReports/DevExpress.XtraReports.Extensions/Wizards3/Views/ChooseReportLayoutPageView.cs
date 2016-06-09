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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraReports.Wizards3.Localization;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Wizards3.Views {
	[ToolboxItem(false)]
	partial class ChooseReportLayoutPageView : WizardViewBase, IChooseReportLayoutPageView  {
		Dictionary<ReportLayout, GalleryItem> reportLayoutItems = new Dictionary<ReportLayout, GalleryItem>();
		GalleryItemGroup groupedReportItems;
		GalleryItemGroup ungroupedReportItems;
		bool isGroupedReport;
		public ChooseReportLayoutPageView() {
			InitializeComponent();
			InitializeGroups();
			UpdateLayoutListItemSource();
		}
		protected override void OnLoad(System.EventArgs e) {
			base.OnLoad(e);
			reportLayoutGallery.LookAndFeel.SetSkinStyle(reportLayoutGallery.LookAndFeel.ActiveSkinName);
		}
		#region IChooseReportLayoutPageView Members
		public bool IsGroupedReport {
			get {
				return isGroupedReport;
			}
			set {
				if(isGroupedReport == value)
					return;
				isGroupedReport = value;
				UpdateLayoutListItemSource();
			}
		}
		public bool AdjustFieldWidth {
			get {
				return adjustFieldWidthButton.Checked == true;
			}
			set {
				adjustFieldWidthButton.Checked = value;
			}
		}
		public ReportLayout ReportLayout {
			get {
				var checkedItem = reportLayoutGallery.Gallery.GetCheckedItems().FirstOrDefault();
				return (ReportLayout)checkedItem.Tag;
			}
			set {
				reportLayoutItems[value].Checked = true;
			}
		}
		public bool Portrait {
			get {
				return (bool)orientationPanel.EditValue;
			}
			set {
				orientationPanel.EditValue = value;
			}
		}
		#endregion
		public override string HeaderDescription {
			get {
				return ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_ReportLayout_Description);
			}
		}
		void InitializeGroups() {
			groupedReportItems = new GalleryItemGroup() { Caption = "GroupedItems" };
			groupedReportItems.Items.Add(CreateGalleryItem(ReportLayout.Stepped));
			groupedReportItems.Items.Add(CreateGalleryItem(ReportLayout.Outline1));
			groupedReportItems.Items.Add(CreateGalleryItem(ReportLayout.Outline2));
			groupedReportItems.Items.Add(CreateGalleryItem(ReportLayout.AlignLeft1));
			groupedReportItems.Items.Add(CreateGalleryItem(ReportLayout.AlignLeft2));
			foreach(GalleryItem item in groupedReportItems.Items)
				reportLayoutItems[(ReportLayout)item.Tag] = item;
			ungroupedReportItems = new GalleryItemGroup() { Caption = "UngroupedItems" };
			ungroupedReportItems.Items.Add(CreateGalleryItem(ReportLayout.Columnar));
			ungroupedReportItems.Items.Add(CreateGalleryItem(ReportLayout.Tabular));
			ungroupedReportItems.Items.Add(CreateGalleryItem(ReportLayout.Justified));
			foreach(GalleryItem item in ungroupedReportItems.Items)
				reportLayoutItems[(ReportLayout)item.Tag] = item;
		}
		private GalleryItem CreateGalleryItem(ReportLayout reportLayout) {
			var item = new GalleryItem();
			switch(reportLayout) {
				case ReportLayout.AlignLeft1:
					item.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.AlignLeft1.png"), LocalResFinder.Assembly);
					item.Caption = ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_Layout_AlignLeft1);
					item.Tag = 4;
					break;
				case ReportLayout.AlignLeft2:
					item.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.AlignLeft2.png"), LocalResFinder.Assembly);
					item.Caption = ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_Layout_AlignLeft2);
					item.Tag = 5;
					break;
				case ReportLayout.Columnar:
					item.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.Columnar.png"), LocalResFinder.Assembly);
					item.Caption = ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_Layout_Columnar);
					item.Tag = 6;
					break;
				case ReportLayout.Justified:
					item.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.Justified.png"), LocalResFinder.Assembly);
					item.Caption = ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_Layout_Justified);
					item.Tag = 8;
					break;
				case ReportLayout.Outline1:
					item.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.Outline1.png"), LocalResFinder.Assembly);
					item.Caption = ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_Layout_Outline1);
					item.Tag = 2;
					break;
				case ReportLayout.Outline2:
					item.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.Outline2.png"), LocalResFinder.Assembly);
					item.Caption = ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_Layout_Outline2);
					item.Tag = 3;
					break;
				case ReportLayout.Stepped:
					item.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.Stepped.png"), LocalResFinder.Assembly);
					item.Caption = ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_Layout_Stepped);
					item.Tag = 0;
					break;
				case ReportLayout.Tabular:
					item.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.Tabular.png"), LocalResFinder.Assembly);
					item.Caption = ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_Layout_Tabular);
					item.Tag = 7;
					break;
				default:
					break;
			}
			return item;
		}
		void UpdateLayoutListItemSource() {
			reportLayoutGallery.Gallery.Groups.Clear();
			reportLayoutGallery.Gallery.DestroyItems();
			if(IsGroupedReport) {
				reportLayoutGallery.Padding = new System.Windows.Forms.Padding(0);
				reportLayoutGallery.Gallery.DistanceBetweenItems = 0;
				reportLayoutGallery.Gallery.Groups.Add(groupedReportItems);
			} else {
				reportLayoutGallery.Padding = new System.Windows.Forms.Padding(36,0,0,0);
				reportLayoutGallery.Gallery.DistanceBetweenItems = 36;
				reportLayoutGallery.Gallery.Groups.Add(ungroupedReportItems);
			}
		}
	}
}
