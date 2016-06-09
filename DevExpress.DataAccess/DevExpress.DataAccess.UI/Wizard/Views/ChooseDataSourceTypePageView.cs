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
using System.Linq;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public partial class ChooseDataSourceTypePageView : WizardViewBase, IChooseDataSourceTypePageView {
		readonly Dictionary<DataSourceType, GalleryItem> itemsRepository;
		readonly DataSourceTypes dataSourceTypes;
		public ChooseDataSourceTypePageView() {
			InitializeComponent();
		}
		public ChooseDataSourceTypePageView(DataSourceTypes dataSourceTypes)
			: this() {
			this.dataSourceTypes = dataSourceTypes;
			GalleryItemGroup galleryItemGroup = new GalleryItemGroup();
			InitializeGallery(galleryItemGroup);
			galleryDataSourceType.Gallery.Groups.Add(galleryItemGroup);
			itemsRepository = new Dictionary<DataSourceType, GalleryItem>();
			foreach (var item in galleryDataSourceType.Gallery.GetAllItems()) {
				itemsRepository[(DataSourceType)item.Tag] = item;
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			galleryDataSourceType.LookAndFeel.SetSkinStyle(this.LookAndFeel.ActiveSkinName);
		}
		#region IWizardPageView Members
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseDSType); }
		}
		#endregion
		public DataSourceType DataSourceType {
			get {
				var checkedItem = galleryDataSourceType.Gallery.GetCheckedItems().FirstOrDefault();
				return checkedItem != null ? (DataSourceType) checkedItem.Tag : DataSourceType.Xpo;
			}
			set {
				GalleryItem galleryItem;
				if(itemsRepository.TryGetValue(value, out galleryItem))
					galleryItem.Checked = true;
			}
		}
		protected virtual void InitializeGallery(GalleryItemGroup galleryItemGroup) {
			if(dataSourceTypes.Contains(DataSourceType.Xpo)) {
				GalleryItem galleryItem = new GalleryItem {
					Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.DSTypeSql),
					Image = GetImage("Database.png"),
					Tag = DataSourceType.Xpo
				};
				galleryItemGroup.Items.Add(galleryItem);
			}
			if(dataSourceTypes.Contains(DataSourceType.Entity)) {
				GalleryItem galleryItem = new GalleryItem {
					Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.DSTypeEF),
					Image = GetImage("EntityFramework.png"),
					Tag = DataSourceType.Entity
				};
				galleryItemGroup.Items.Add(galleryItem);
			}
			if(dataSourceTypes.Contains(DataSourceType.Object)) {
				GalleryItem galleryItem = new GalleryItem {
					Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.DSTypeObject),
					Image = GetImage("ObjectBinding.png"),
					Tag = DataSourceType.Object
				};
				galleryItemGroup.Items.Add(galleryItem);
			}
			if (dataSourceTypes.Contains(DataSourceType.Excel)) {
				GalleryItem galleryItem = new GalleryItem {
					Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.DSTypeExcel),
					Image = GetImage("FileBinding.png"),
					Tag = DataSourceType.Excel
				};
				galleryItemGroup.Items.Add(galleryItem);
			}
			galleryItemGroup.CaptionAlignment = GalleryItemGroupCaptionAlignment.Center;
		}
		Image GetImage(string name) {
			return ResourceImageHelper.CreateImageFromResources("DevExpress.DataAccess.UI.Wizard.Images." + name, typeof(ChooseDataSourceTypePageView).Assembly);
		}
		void galleryControlGallery1_ItemDoubleClick(object sender, GalleryItemClickEventArgs e) {
			MoveForward();
		}
	}
}
