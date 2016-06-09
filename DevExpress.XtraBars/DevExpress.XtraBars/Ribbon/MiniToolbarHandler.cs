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
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon.ViewInfo;
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class RibbonMiniToolbarHandler : BaseRibbonHandler { 
		RibbonMiniToolbarControl control;
		public RibbonMiniToolbarHandler(RibbonMiniToolbarControl control) {
			this.control = control;
		}
		public RibbonMiniToolbarControl Control { get { return control; } }
		public RibbonMiniToolbar Toolbar { get { return Control.Toolbar; } }
		public RibbonControl Ribbon { get { return Control.Ribbon; } }
		protected override bool IsKeyboardActive { get { return Ribbon.IsKeyboardActive; } }
		protected override bool IsEditorActive { get { return Ribbon.Manager.ActiveEditor != null; } }
		protected override bool IsDesignTime { get { return Ribbon.IsDesignMode; } }
		protected override DevExpress.XtraBars.ViewInfo.BarSelectionInfo SelectionInfo {
			get { return Ribbon.Manager.SelectionInfo; }
		}
		protected override DevExpress.XtraBars.Ribbon.ViewInfo.BaseRibbonViewInfo ViewInfo {
			get { return Control.ViewInfo; }
		}
		protected override LinksNavigation CreateLinksNavigator() {
			return new RibbonMiniToolbarNavigation(Toolbar);
		}
		public override Control OwnerControl {
			get { return Control; }
		}
		protected override bool CanHotTrack(RibbonHitInfo hitInfo) {
			if(!ViewInfo.PressedObject.IsEmpty) return false;
			if(IsDesignTime) return false;
			return true;
		}
		protected internal override void UpdateImageGalleries() {
			RibbonMiniToolbarViewInfo ti = ViewInfo as RibbonMiniToolbarViewInfo;
			if(ti == null)
				return;
			for(int i = 0; i < ti.Items.Count; i++) {
				UpdateImageGallery(ti.Items[i] as InRibbonGalleryRibbonItemViewInfo);
			}
			if(ViewInfo.HotObject.HitTest == RibbonHitTest.GalleryImage && ViewInfo.HotObject.GalleryInfo.Gallery.AllowHoverImages) {
				if(!ViewInfo.HotObject.GalleryInfo.AllowPartitalItems && ViewInfo.HotObject.GalleryItemInfo.IsPartiallyVisible) return;
				if(!ShouldShowImageForm) return;
				ViewInfo.HotObject.GalleryInfo.Gallery.ShowImageForm(ViewInfo.HotObject.GalleryInfo, ViewInfo.HotObject.GalleryItemInfo);
			}
		}
		protected override bool ShouldShowImageForm {
			get {
				return true;
			}
		}
	}
}
