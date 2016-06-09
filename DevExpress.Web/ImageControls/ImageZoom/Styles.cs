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
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class ImageZoomStyles : StylesBase {
		internal const string CssPrefix = "dxiz";
		internal const string HintClassName = CssPrefix + "-hint",
							InsideClipPanelClassName = CssPrefix + "-inside",
							ClipPanelClassName = CssPrefix + "-clipPanel",
							WrapperClassName = CssPrefix + "-wrapper",
							ExpandWindowClassName = CssPrefix + "-expandWindow",
							ZoomWindowImageClassName = CssPrefix + "-zwImage",
							ExpandWindowImageClassName = CssPrefix + "-ewImage",
							DesignTimeEmptyImageClassName = CssPrefix + "-emptyImage";
		public ImageZoomStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected internal override string GetCssClassNamePrefix() {
			return CssPrefix;
		}
		protected internal T GetDefaultStyle<T>(string cssClassName) where T :
			AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = string.Format("{0}-{1}", GetCssClassNamePrefix(), cssClassName);
			return style;
		}
	}
	public abstract class ImageZoomWindowStylesBase : PopupControlStyles {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupWindowButtonStyle PinButton {
			get { return (PopupWindowButtonStyle)GetStyle(PinButtonStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupWindowButtonStyle RefreshButton {
			get { return (PopupWindowButtonStyle)GetStyle(RefreshButtonStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupWindowButtonStyle CollapseButton {
			get { return (PopupWindowButtonStyle)GetStyle(CollapseButtonStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupWindowButtonStyle MaximizeButton {
			get { return (PopupWindowButtonStyle)GetStyle(MaximizeButtonStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupWindowStyle Header {
			get { return (PopupWindowStyle)GetStyle(HeaderStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LinkStyle Link {
			get { return base.LinkInternal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LoadingDivStyle LoadingDiv {
			get { return base.LoadingDivInternal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LoadingPanelStyle LoadingPanel {
			get { return base.LoadingPanelInternal; }
		}
		public ImageZoomWindowStylesBase(ISkinOwner owner)
			: base(owner) {
		}
	}
	public class ImageZoomZoomWindowStyles : ImageZoomWindowStylesBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControlModalBackgroundStyle ModalBackground {
			get { return base.ModalBackground; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PopupWindowButtonStyle CloseButton {
			get { return (PopupWindowButtonStyle)GetStyle(CloseButtonStyleName); }
		}
		public ImageZoomZoomWindowStyles(ISkinOwner owner)
			: base(owner) {
		}
	}
	public class ImageZoomExpandWindowStyles : ImageZoomWindowStylesBase {
		public const string ExpandWindowCloseButtonStyleName = "EWCloseButton";
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomExpandWindowStylesCloseButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ButtonStyle CloseButton {
			get { return (ButtonStyle)GetStyle(ExpandWindowCloseButtonStyleName); }
		}
		public ImageZoomExpandWindowStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected new internal ButtonStyle GetDefaultCloseButtonStyle() {
			return GetDefaultStyle<ButtonStyle>("dxiz-" + ExpandWindowCloseButtonStyleName);
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ExpandWindowCloseButtonStyleName, delegate() { return new ButtonStyle(); }));
		}
	}
	public class ImageZoomImages : ImageControlsImagesBase {
		public const string HintImageName = "izHint";
		public ImageZoomImages(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomImagesHint"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Hint {
			get { return (ImageProperties)GetImageBase(HintImageName); }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(HintImageName, string.Empty, typeof(ImageProperties), HintImageName));
		}
	}
	public class ImageZoomWindowImagesBase : PopupControlImages {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HeaderButtonCheckedImageProperties PinButton {
			get { return (HeaderButtonCheckedImageProperties)GetImageBase(PinButtonImageName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HeaderButtonImageProperties RefreshButton {
			get { return (HeaderButtonImageProperties)GetImageBase(RefreshButtonImageName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HeaderButtonCheckedImageProperties CollapseButton {
			get { return (HeaderButtonCheckedImageProperties)GetImageBase(CollapseButtonImageName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HeaderButtonCheckedImageProperties MaximizeButton {
			get { return (HeaderButtonCheckedImageProperties)GetImageBase(MaximizeButtonImageName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties SizeGrip {
			get { return (ImageProperties)GetImageBase(SizeGripImageName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties SizeGripRtl {
			get { return (ImageProperties)GetImageBase(SizeGripRtlImageName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageProperties Header {
			get { return (ImageProperties)GetImageBase(HeaderImageName); }
		}
		public ImageZoomWindowImagesBase(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
	}
	public class ImageZoomZoomWindowImages : ImageZoomWindowImagesBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HeaderButtonImageProperties CloseButton {
			get { return (HeaderButtonImageProperties)GetImageBase(CloseButtonImageName); }
		}
		public ImageZoomZoomWindowImages(ISkinOwner owner)
			: base(owner) {
		}
	}
	public class ImageZoomExpandWindowImages : ImageZoomWindowImagesBase {
		public const string ExpandWindowCloseButtonImageName = "izEWCloseButton";
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomExpandWindowImagesCloseButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ButtonImageProperties CloseButton {
			get { return (ButtonImageProperties)GetImageBase(ExpandWindowCloseButtonImageName); }
		}
		public ImageZoomExpandWindowImages(ISkinOwner owner)
			: base(owner) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(ExpandWindowCloseButtonImageName, ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState, string.Empty, typeof(ButtonImageProperties), ExpandWindowCloseButtonImageName));
		}
	}
}
