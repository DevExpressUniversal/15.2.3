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
	public class SplitterImages : ImagesBase {
		static string[] verticalImageNames = new string[] {
			"splVSeparator",
			"splVCollapseBackwardButton",
			"splVCollapseForwardButton"
		};
		static string[] horizontalImageNames = new string[] {
			"splHSeparator",
			"splHCollapseBackwardButton",
			"splHCollapseForwardButton"
		};
		protected internal const string ResizingPointerBackImageName = "splResizingPointer";
		public SplitterImages(ASPxSplitter splitter)
			: base(splitter) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(GetImageInfo(SplitterButtons.Separator, Orientation.Vertical));
			list.Add(GetImageInfo(SplitterButtons.Separator, Orientation.Horizontal));
			list.Add(GetImageInfo(SplitterButtons.Backward, Orientation.Vertical));
			list.Add(GetImageInfo(SplitterButtons.Backward, Orientation.Horizontal));
			list.Add(GetImageInfo(SplitterButtons.Forward, Orientation.Vertical));
			list.Add(GetImageInfo(SplitterButtons.Forward, Orientation.Horizontal));
		}
		protected internal static string GetImageName(SplitterButtons buttonType, Orientation orientation) {
			return (orientation == Orientation.Vertical) ? verticalImageNames[(int)buttonType] : horizontalImageNames[(int)buttonType];
		}
		protected ImageInfo GetImageInfo(SplitterButtons buttonType, Orientation orientation) {
			return new ImageInfo(GetImageName(buttonType, orientation), ImageFlags.HasHottrackState, 
				typeof(HottrackedImageProperties), GetImageName(buttonType, orientation));
		}
		ImagePropertiesBase GetImageBase(SplitterButtons buttonType, Orientation orientation) {
			return GetImageBase(GetImageName(buttonType, orientation));
		}
		ImageProperties GetImage(SplitterButtons buttonType, Orientation orientation) {
			return (ImageProperties)GetImageBase(buttonType, orientation);
		}
		HottrackedImageProperties GetHottrackedImage(SplitterButtons buttonType, Orientation orientation) {
			return (HottrackedImageProperties)GetImageBase(buttonType, orientation);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterImagesVerticalSeparatorButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HottrackedImageProperties VerticalSeparatorButton {
			get { return GetHottrackedImage(SplitterButtons.Separator, Orientation.Vertical); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterImagesHorizontalSeparatorButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HottrackedImageProperties HorizontalSeparatorButton {
			get { return GetHottrackedImage(SplitterButtons.Separator, Orientation.Horizontal); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterImagesVerticalCollapseBackwardButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HottrackedImageProperties VerticalCollapseBackwardButton {
			get { return GetHottrackedImage(SplitterButtons.Backward, Orientation.Vertical); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterImagesVerticalCollapseForwardButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HottrackedImageProperties VerticalCollapseForwardButton {
			get { return GetHottrackedImage(SplitterButtons.Forward, Orientation.Vertical); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterImagesHorizontalCollapseBackwardButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HottrackedImageProperties HorizontalCollapseBackwardButton {
			get { return GetHottrackedImage(SplitterButtons.Backward, Orientation.Horizontal); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterImagesHorizontalCollapseForwardButton"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HottrackedImageProperties HorizontalCollapseForwardButton {
			get { return GetHottrackedImage(SplitterButtons.Forward, Orientation.Horizontal); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageProperties LoadingPanel { get { return base.LoadingPanel; } }
	}
}
