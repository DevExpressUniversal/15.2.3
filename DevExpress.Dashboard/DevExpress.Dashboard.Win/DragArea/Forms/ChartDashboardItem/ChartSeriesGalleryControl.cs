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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Native;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DashboardWin.Native {
	[ToolboxItem(false)]
	public partial class ChartSeriesGalleryControl : DashboardUserControl {
		static bool useColors = true;
		public static bool UseColors { get { return useColors; } set { useColors = value; } }
		public GalleryControl GalleryControl { get { return galleryControl; } }
		public event EventHandler<GalleryItemEventArgs> SelectedItemChanged;
		public ChartSeriesGalleryControl() {
			InitializeComponent();
		}
		public void Initialize(IChartSeriesOptionsCommandFactory factory, IEnumerable<SeriesViewGroup> seriesViewGroups) {
			galleryControlClient.TabStop = false;
			GalleryControlGallery gallery = galleryControl.Gallery;
			gallery.CustomDrawItemImage += new GalleryItemCustomDrawEventHandler(gallery_CustomDrawItemImage);
			gallery.BeginUpdate();
			try {
				foreach(SeriesViewGroup seriesViewGroup in seriesViewGroups) {
					GalleryItemGroup group = new GalleryItemGroup();
					group.Caption = seriesViewGroup.Name;
					gallery.Groups.Add(group);
					foreach(ChartSeriesConverter converter in seriesViewGroup.Converters) {
						ChartSeriesOptionsCommandBase command = factory.CreateCommand(converter);
						GalleryItem item = new GalleryItem();
						item.Tag = command;
						item.Hint = converter.Caption;
						item.Checked = command.Selected;
						item.Image = ImageHelper.GetImage(UseColors ? converter.GalleryImagePath : converter.GalleryImagePathBlack);
						group.Items.Add(item);
					}
				}
			} finally {
				gallery.EndUpdate();
			}
		}
		public Size CalcBestSize() {
			Size size = galleryControl.CalcBestSize();
			return new Size(size.Width + galleryControl.Margin.Left + galleryControl.Margin.Right + (galleryControl.Gallery.Groups.Count > 1 ? SystemInformation.VerticalScrollBarWidth : 0),
							size.Height + galleryControl.Margin.Top + galleryControl.Margin.Bottom);
		}
		public IList<GalleryItem> GetCheckedItems() {
			return galleryControl.Gallery.GetCheckedItems();
		}
		void gallery_CustomDrawItemImage(object sender, GalleryItemCustomDrawEventArgs e) {
			if(!UseColors) {
				e.Cache.Graphics.DrawImage(e.Item.Image, e.Bounds, 0, 0, e.Item.Image.Width, e.Item.Image.Height, GraphicsUnit.Pixel, DevExpress.Utils.Drawing.ImageColorizer.GetColoredAttributes(((DevExpress.XtraBars.Ribbon.ViewInfo.GalleryItemViewInfo)e.ItemInfo).PaintAppearance.GroupCaption.GetForeColor()));
				e.Handled = true;
			}
		}
		void OnItemCheckedChanged(object sender, GalleryItemEventArgs e) {
			if(SelectedItemChanged != null)
				SelectedItemChanged.Invoke(sender, e);
		}
	}
}
