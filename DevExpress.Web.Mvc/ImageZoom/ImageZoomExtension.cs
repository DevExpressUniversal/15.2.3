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
using System.Linq;
using System.Text;
using System.Web.Mvc;
namespace DevExpress.Web.Mvc {
	public class ImageZoomExtension: ExtensionBase {
		public ImageZoomExtension(ImageZoomSettings settings)
			: base(settings) {
		}
		public ImageZoomExtension(ImageZoomSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxImageZoom Control {
			get { return (MVCxImageZoom)base.Control; }
		}
		protected internal new ImageZoomSettings Settings {
			get { return (ImageZoomSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AssociatedImageZoomNavigatorID = Settings.AssociatedImageZoomNavigatorName;
			Control.ImageUrl = Settings.ImageUrl;
			Control.LargeImageUrl = Settings.LargeImageUrl;
			Control.LargeImageLoadMode = Settings.LargeImageLoadMode;
			Control.ZoomWindowText = Settings.ZoomWindowText;
			Control.ExpandWindowText = Settings.ExpandWindowText;
			Control.ShowHint = Settings.ShowHint;
			Control.ShowHintText = Settings.ShowHintText;
			Control.HintText = Settings.HintText;
			Control.EnableZoomMode = Settings.EnableZoomMode;
			Control.EnableExpandMode = Settings.EnableExpandMode;
			Control.SettingsZoomMode.Assign(Settings.SettingsZoomMode);
			Control.StylesZoomWindow.CopyFrom(Settings.StylesZoomWindow);
			Control.StylesExpandWindow.CopyFrom(Settings.StylesExpandWindow);
			Control.LoadingPanelStyle.CopyFrom(Settings.LoadingPanelStyle);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.ImagesZoomWindow.CopyFrom(Settings.ImagesZoomWindow);
			Control.ImagesExpandWindow.CopyFrom(Settings.ImagesExpandWindow);
			Control.Images.CopyFrom(Settings.Images);
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxImageZoom();
		}
	}
}
