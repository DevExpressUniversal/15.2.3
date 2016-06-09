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
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class PanelImages : ImagesBase {
		internal const string ExpandButtonImageName = "pnlExpand";
		internal const string CollapseButtonImageName = "pnlCollapse";
		internal const string ExpandButtonArrowLeftImageName = "pnlExpandArrowLeft";
		internal const string ExpandButtonArrowRightImageName = "pnlExpandArrowRight";
		internal const string ExpandButtonArrowTopImageName = "pnlExpandArrowTop";
		internal const string ExpandButtonArrowBottomImageName = "pnlExpandArrowBottom";
		public PanelImages(ASPxCollapsiblePanel owner)
			:base(owner) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(ExpandButtonImageName, ImageFlags.IsPng, 32, 32, string.Empty, ExpandButtonImageName));
			list.Add(new ImageInfo(CollapseButtonImageName, ImageFlags.IsPng, 32, 32, string.Empty, CollapseButtonImageName));
			list.Add(new ImageInfo(ExpandButtonArrowLeftImageName, ImageFlags.IsPng, 32, 32, string.Empty, ExpandButtonArrowLeftImageName));
			list.Add(new ImageInfo(ExpandButtonArrowRightImageName, ImageFlags.IsPng, 32, 32, string.Empty, ExpandButtonArrowRightImageName));
			list.Add(new ImageInfo(ExpandButtonArrowTopImageName, ImageFlags.IsPng, 32, 32, string.Empty, ExpandButtonArrowTopImageName));
			list.Add(new ImageInfo(ExpandButtonArrowBottomImageName, ImageFlags.IsPng, 32, 32, string.Empty, ExpandButtonArrowBottomImageName));
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelImagesExpandButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ExpandButton {
			get { return (ImageProperties)GetImageBase(ExpandButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelImagesCollapseButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties CollapseButton {
			get { return (ImageProperties)GetImageBase(CollapseButtonImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelImagesExpandButtonArrowLeft"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ExpandButtonArrowLeft {
			get { return (ImageProperties)GetImageBase(ExpandButtonArrowLeftImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelImagesExpandButtonArrowRight"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ExpandButtonArrowRight {
			get { return (ImageProperties)GetImageBase(ExpandButtonArrowRightImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelImagesExpandButtonArrowTop"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ExpandButtonArrowTop {
			get { return (ImageProperties)GetImageBase(ExpandButtonArrowTopImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PanelImagesExpandButtonArrowBottom"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ExpandButtonArrowBottom {
			get { return (ImageProperties)GetImageBase(ExpandButtonArrowBottomImageName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageProperties LoadingPanel { get { return base.LoadingPanel; } }
	}
}
