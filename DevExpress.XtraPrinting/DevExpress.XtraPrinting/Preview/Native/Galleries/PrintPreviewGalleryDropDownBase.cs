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
using System.Text;
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Utils;
using System.Drawing.Printing;
namespace DevExpress.XtraPrinting.Preview.Native.Galleries {
	[ToolboxItem(false)]
	public abstract class PrintPreviewGalleryDropDownBase : GalleryDropDown {
		RibbonPreviewItemsLogicBase logic;
		GalleryItemGroup group = new GalleryItemGroup();
		public abstract PrintPreviewGalleryItem[] Items { get; }
		protected virtual int RowCount { get { return group.Items.Count; } }
		protected RibbonPreviewItemsLogicBase Logic { get { return logic; } }
		protected PrintPreviewGalleryDropDownBase(RibbonPreviewItemsLogicBase logic, IContainer container) : base(container) {
			this.logic = logic;
			SetAlignment(Gallery.Appearance.ItemCaptionAppearance.Normal);
			SetAlignment(Gallery.Appearance.ItemCaptionAppearance.Pressed);
			SetAlignment(Gallery.Appearance.ItemCaptionAppearance.Hovered);
			SetAlignment(Gallery.Appearance.ItemDescriptionAppearance.Normal);
			SetAlignment(Gallery.Appearance.ItemDescriptionAppearance.Pressed);
			SetAlignment(Gallery.Appearance.ItemDescriptionAppearance.Hovered);
			Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleCenter;
			Gallery.ItemImageLocation = Locations.Left;
			Gallery.ColumnCount = 1;
			Gallery.ShowGroupCaption = false;
			Gallery.ShowItemText = true;
			Gallery.DrawImageBackground = false;
			Gallery.Groups.Add(group);
			GalleryItemClick += new GalleryItemClickEventHandler(OnGalleryItemClick);
		}
		static void SetAlignment(AppearanceObject appearance) {
			appearance.TextOptions.HAlignment = HorzAlignment.Near;
			appearance.TextOptions.VAlignment = VertAlignment.Center;
			appearance.Options.UseTextOptions = true;
		}
		protected override void OnBeforePopup(CancelEventArgs e) {
			base.OnBeforePopup(e);
			BeginUpdate();
			group.Items.Clear();
			foreach(PrintPreviewGalleryItem item in Items) {
				if(Logic.IsCommandVisible(item.Command) && Logic.IsCommandEnabled(item.Command)) {
					group.Items.Add(item);
					item.Checked = IsItemChecked(item);
				}
			}
			Gallery.RowCount = RowCount;
			EndUpdate();
		}
		protected virtual bool IsItemChecked(PrintPreviewGalleryItem item) {
			return false;
		}
		protected virtual void OnPrintPreviewGalleryItemClick(PrintPreviewGalleryItem commandItem) {
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					GalleryItemClick -= new GalleryItemClickEventHandler(OnGalleryItemClick);
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		void OnGalleryItemClick(object sender, GalleryItemClickEventArgs e) {
			PrintPreviewGalleryItem commandItem = e.Item as PrintPreviewGalleryItem;
			Logic.ExecCommand(commandItem);
			OnPrintPreviewGalleryItemClick(commandItem);
		}
		protected Image GetImage(string alias) {
			foreach(ImageItem imageItem in this.Logic.ImageCollection) {
				if(imageItem.Name == alias)
					return imageItem.Image;
			}
			return PrintRibbonControllerConfigurator.GetImageFromResource(alias);
		}
		protected Image GetLargeImage(string alias) {
			return GetImage(PrintRibbonControllerConfigurator.GetLargeImageName(alias));
		}
	}
}
