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

using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class ImageZoomNavigatorStyles : ImageSliderStylesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorStylesPrevPageButtonHorizontalOutside"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle PrevPageButtonHorizontalOutside {
			get { return PrevPageButtonHorizontalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorStylesNextPageButtonHorizontalOutside"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle NextPageButtonHorizontalOutside {
			get { return NextPageButtonHorizontalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorStylesPrevPageButtonVerticalOutside"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle PrevPageButtonVerticalOutside {
			get { return PrevPageButtonVerticalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorStylesNextPageButtonVerticalOutside"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationButtonStyle NextPageButtonVerticalOutside {
			get { return NextPageButtonVerticalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorStylesNavigatorHorizontal"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarStyle NavigatorHorizontal {
			get { return NavigationBarThumbnailsModeBottomInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorStylesNavigatorVertical"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarStyle NavigatorVertical {
			get { return NavigationBarThumbnailsModeLeftInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorStylesThumbnail"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public ImageSliderNavigationBarThumbnailStyle Thumbnail {
			get { return ThumbnailInternal; }
		}
		public ImageZoomNavigatorStyles(ISkinOwner owner)
			: base(owner) {
		}
	}
	public class ImageZoomNavigatorImages : ImageSliderImagesBase {
		public ImageZoomNavigatorImages(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorImagesPrevPageButtonVerticalOutside"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties PrevPageButtonVerticalOutside {
			get { return PrevPageButtonVerticalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorImagesNextPageButtonVerticalOutside"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties NextPageButtonVerticalOutside {
			get { return NextPageButtonVerticalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorImagesPrevPageButtonHorizontalOutside"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties PrevPageButtonHorizontalOutside {
			get { return PrevPageButtonHorizontalOutsideInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorImagesNextPageButtonHorizontalOutside"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties NextPageButtonHorizontalOutside {
			get { return NextPageButtonHorizontalOutsideInternal; }
		}
	}
}
