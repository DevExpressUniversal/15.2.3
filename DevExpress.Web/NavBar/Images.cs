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
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class NavBarImages : ImagesBase {
		public const string CollapseImageName = "nbCollapse",
							ExpandImageName = "nbExpand",
							GroupHeaderImageName = "nbGroupHeader",
							GroupHeaderImageCollapsedName = "nbGroupHeaderCollapsed",
							ItemImageName = "nbItem";
		public NavBarImages(ASPxNavBar navBar)
			: base(navBar) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarImagesCollapse"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Collapse {
			get { return GetImage(CollapseImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarImagesExpand"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Expand {
			get { return GetImage(ExpandImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarImagesGroupHeader"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties GroupHeader {
			get { return GetImage(GroupHeaderImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarImagesGroupHeaderCollapsed"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties GroupHeaderCollapsed {
			get { return GetImage(GroupHeaderImageCollapsedName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NavBarImagesItem"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ItemImageProperties Item {
			get { return (ItemImageProperties)GetImageBase(ItemImageName); }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(CollapseImageName, ImageFlags.IsPng, 13, 15, CollapseImageName));
			list.Add(new ImageInfo(ExpandImageName, ImageFlags.IsPng, 13, 15, ExpandImageName));
			list.Add(new ImageInfo(GroupHeaderImageName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(GroupHeaderImageCollapsedName, ImageFlags.HasNoResourceImage));
			list.Add(new ImageInfo(ItemImageName, ImageFlags.HasNoResourceImage, typeof(ItemImageProperties)));
		}
	}
}
