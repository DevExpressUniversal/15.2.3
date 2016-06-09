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
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.Web {
	public class ImageZoomZoomModeSettings : PropertiesBase {
		internal const string DefaultZoomAreaSize = "150%";
		internal const int DefaultZoomWindowOffset = 15;
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomZoomModeSettingsZoomWindowWidth"),
#endif
		DefaultValue(typeof(Unit), DefaultZoomAreaSize), AutoFormatEnable, NotifyParentProperty(true)]
		public Unit ZoomWindowWidth {
			get { return GetUnitProperty("ZoomWindowWidth", new Unit(DefaultZoomAreaSize)); }
			set { SetUnitProperty("ZoomWindowWidth", new Unit(DefaultZoomAreaSize), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomZoomModeSettingsZoomWindowHeight"),
#endif
		DefaultValue(typeof(Unit), DefaultZoomAreaSize), AutoFormatEnable, NotifyParentProperty(true)]
		public Unit ZoomWindowHeight {
			get { return GetUnitProperty("ZoomWindowHeight", new Unit(DefaultZoomAreaSize)); }
			set { SetUnitProperty("ZoomWindowHeight", new Unit(DefaultZoomAreaSize), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomZoomModeSettingsZoomWindowOffset"),
#endif
		DefaultValue(DefaultZoomWindowOffset), AutoFormatEnable, NotifyParentProperty(true)]
		public int ZoomWindowOffset {
			get { return (int)GetIntProperty("ZoomWindowOffset", DefaultZoomWindowOffset); }
			set { SetIntProperty("ZoomWindowOffset", DefaultZoomWindowOffset, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomZoomModeSettingsZoomWindowPosition"),
#endif
		DefaultValue(ZoomWindowPosition.Right), AutoFormatEnable, NotifyParentProperty(true)]
		public ZoomWindowPosition ZoomWindowPosition {
			get { return (ZoomWindowPosition)GetEnumProperty("ZoomWindowPosition", ZoomWindowPosition.Right); }
			set { SetEnumProperty("ZoomWindowPosition", ZoomWindowPosition.Right, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomZoomModeSettingsMouseBoxOpacityMode"),
#endif
		DefaultValue(MouseBoxOpacityMode.Inside), AutoFormatEnable, NotifyParentProperty(true)]
		public MouseBoxOpacityMode MouseBoxOpacityMode {
			get { return (MouseBoxOpacityMode)GetEnumProperty("MouseBoxOpacityMode", MouseBoxOpacityMode.Inside); }
			set { SetEnumProperty("MouseBoxOpacityMode", MouseBoxOpacityMode.Inside, value); }
		}
		public ImageZoomZoomModeSettings(ASPxImageZoom imageZoom)
			: base(imageZoom) {
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ImageZoomZoomModeSettings src = source as ImageZoomZoomModeSettings;
				if (src != null) {
					ZoomWindowWidth = src.ZoomWindowWidth;
					ZoomWindowHeight = src.ZoomWindowHeight;
					ZoomWindowOffset = src.ZoomWindowOffset;
					ZoomWindowPosition = src.ZoomWindowPosition;
					MouseBoxOpacityMode = src.MouseBoxOpacityMode;
				}
			} finally {
				EndUpdate();
			}
		}
	}
}
