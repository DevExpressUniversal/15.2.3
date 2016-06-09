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
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class EnumGalleryControl : XtraUserControl {
		static readonly Size DefaultPopupContainerSize = new Size(255, 255);
		object value;
		GalleryElement curentElement;
		GalleryElementsProvider provider;
		List<GalleryElement> elements;
		public event EventHandler SelectedItemChanged;
		GalleryElement CurentElement {
			get { return curentElement; }
			set {
				curentElement = value;
				OnCurentElementChanged();
			}
		}
		public GalleryElementsProvider Provider {
			get { return provider; }
			set {
				if (provider != value) {
					provider = value;
					elements = provider.GetElements();
					FillItemsList(elements);
				}
			}
		}
		public object Value {
			get { return this.value; }
			set {
				if (this.value != value && value != null) {
					this.value = value;
					foreach (GalleryElement element in elements) {
						if (this.value.Equals(element.Value)) {
							CurentElement = element;
							return;
						}
					}
				}
			}
		}
		public EnumGalleryControl() {
			InitializeComponent();
			if (provider != null) {
				elements = provider.GetElements();
				FillItemsList(elements);
			}
		}
		void FillItemsList(List<GalleryElement> elements) {
			GalleryItemGroup galleryGroup = new GalleryItemGroup();
			foreach (GalleryElement element in elements) {
				GalleryItem galleryItem = new GalleryItem(element.Image, null, string.Empty, string.Empty, 0, 0, element, element.Title);
				galleryGroup.Items.Add(galleryItem);
			}
			glrViewTypes.Gallery.Groups.Add(galleryGroup);
		}
		void OnCurentElementChanged() {
			if (CurentElement != null && CurentElement.Image != null) {
				this.dropDownButton.Image = CurentElement.Image;
			}
		}		
		void galleryControlGallery1_ItemClick(object sender, GalleryItemClickEventArgs e) {
			if (e.Item != null && e.Item.Tag != null) {
				CurentElement = ((GalleryElement)e.Item.Tag);
				value = curentElement.Value;
				if (SelectedItemChanged != null)
					SelectedItemChanged(this, new EventArgs());
			}
		}
		void dropDownButton_Click(object sender, EventArgs e) {
			DevExpress.XtraBars.Ribbon.Gallery.GalleryControlGallery gallery = glrViewTypes.Gallery;
			DevExpress.XtraBars.Ribbon.ViewInfo.StandaloneGalleryViewInfo viewinfo = gallery.GetViewInfo();
			popupContainer.Size = viewinfo != null ? viewinfo.CalcGalleryBestSize(gallery.ColumnCount, gallery.RowCount) : DefaultPopupContainerSize;
		}
	}
	public abstract class GalleryElementsProvider {
		public abstract List<GalleryElement> GetElements();
	}
	public class GalleryElement {
		readonly object value;
		readonly string title;
		readonly Image image;
		public object Value {
			get { return value; }
		}
		public string Title {
			get { return title; }
		}
		public Image Image {
			get { return image; }
		}
		public GalleryElement(object value, string title, Image image) {
			this.value = value;
			this.title = title;
			this.image = image;
		}
	}
}
