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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraEditors;
namespace DevExpress.XtraPrinting.Native.WinControls {
	[System.ComponentModel.ToolboxItem(false)]
	public partial class CertificateSelector : XtraUserControl {
		public event EventHandler SelectionChanged;
		GalleryControl list;
		GalleryControlClient galleryControlClient1;
		public ICertificateItem SelectedItem {
			get { 
				foreach(GalleryItem galleryItem in Gallery.Groups[0].Items) {
					if(galleryItem.Checked)
						return (ICertificateItem)galleryItem.Tag;
				}
				return null;
			}
			set{
				foreach(GalleryItem galleryItem in Gallery.Groups[0].Items) {
					if(galleryItem.Tag == value)
						galleryItem.Checked = true;
				}
			}
		}
		GalleryControlGallery Gallery { get { return this.list.Gallery; } }
		public CertificateSelector() {
			InitializeComponent();
			foreach(AppearanceObject a in GetAppearances(Gallery.Appearance.ItemCaptionAppearance)) {
				a.TextOptions.HAlignment = HorzAlignment.Near;
				a.Font = new Font("Tahoma", 10);
				a.Options.UseTextOptions = true;
				a.Options.UseFont = true;
			}
			foreach(AppearanceObject a in GetAppearances(Gallery.Appearance.ItemDescriptionAppearance)) {
				a.TextOptions.HAlignment = HorzAlignment.Near;
				a.Font = new Font("Tahoma", 8.25f);
				a.Options.UseTextOptions = true;
				a.Options.UseFont = true;
			}
			Gallery.ItemCheckedChanged += (o, e) => {
				if(SelectionChanged != null)
					SelectionChanged(this, EventArgs.Empty);
			};
		}
		static AppearanceObject[] GetAppearances(StateAppearances a) {
			return new AppearanceObject[] { a.Normal, a.Hovered, a.Pressed };
		}
		public void FillItems(IEnumerable<ICertificateItem> items) {
			Gallery.Groups[0].Items.Clear();
			foreach(ICertificateItem cert in items) {
				string decription = cert.Description;
				Gallery.Groups[0].Items.Add(new GalleryItem(null, cert.Subject, decription) { Tag = cert });
			}
		}
		public void SelectItem(ICertificateItem item) {
			foreach(GalleryItem galleryItem in Gallery.Groups[0].Items) {
				if(galleryItem.Tag == item)
					galleryItem.Checked = true;				
			}
		}
	}
}
