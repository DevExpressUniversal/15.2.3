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

using DevExpress.Web;
using System;
namespace DevExpress.Web.Mvc {
	public class ImageZoomSettings: SettingsBase {
		public ImageZoomSettings() {
			ShowHint = true;
			ShowHintText = true;
			HintText = string.Empty;
			EnableZoomMode = true;
			EnableExpandMode = true;
			SettingsZoomMode = new ImageZoomZoomModeSettings(null);
			StylesZoomWindow = new ImageZoomZoomWindowStyles(null);
			StylesExpandWindow = new ImageZoomExpandWindowStyles(null);
			LoadingPanelStyle = new LoadingPanelStyle();
			ImagesZoomWindow = new ImageZoomZoomWindowImages(null);
			ImagesExpandWindow = new ImageZoomExpandWindowImages(null);
		}
		public string AssociatedImageZoomNavigatorName { get; set; }
		public string ImageUrl { get; set; }
		public string LargeImageUrl { get; set; }
		public LargeImageLoadMode LargeImageLoadMode { get; set; }
		public string ZoomWindowText { get; set; }
		public string ExpandWindowText { get; set; }
		public bool ShowHint { get; set; }
		public bool ShowHintText { get; set; }
		public string HintText { get; set; }
		public bool EnableZoomMode { get; set; }
		public bool EnableExpandMode { get; set; }
		public ImageZoomZoomModeSettings SettingsZoomMode { get; private set; }
		public ImageZoomZoomWindowStyles StylesZoomWindow { get; private set; }
		public ImageZoomExpandWindowStyles StylesExpandWindow { get; private set; }
		public LoadingPanelStyle LoadingPanelStyle { get; private set; }
		public ImageZoomStyles Styles { get { return (ImageZoomStyles)StylesInternal; } }
		public ImageZoomZoomWindowImages ImagesZoomWindow { get; private set; }
		public ImageZoomExpandWindowImages ImagesExpandWindow { get; private set; }
		public ImageZoomImages Images { get { return (ImageZoomImages)ImagesInternal; } }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return null;
		}
		protected override ImagesBase CreateImages() {
			return new ImageZoomImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new ImageZoomStyles(null);
		}
	}
}
