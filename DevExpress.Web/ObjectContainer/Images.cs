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
	public class ObjectContainerImages : ImagesBase {
		public const string ErrorImageName = "ocErrorObjectProperties",
			ImageImageName = "ocImageObjectProperties",
			FlashImageName = "ocFlashObjectProperties",
			VideoImageName = "ocVideoObjectProperties",
			AudioImageName = "ocAudioObjectProperties",
			QuickTimeImageName = "ocQuickTimeObjectProperties",
			Html5VideoImageName = "ocHtml5VideoObjectProperties",
			Html5AudioImageName = "ocHtml5AudioObjectProperties";
		public ObjectContainerImages(ASPxObjectContainer objectContainer)
			: base(objectContainer) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			string[] names = new string[] { ErrorImageName, ImageImageName, FlashImageName,
				VideoImageName, AudioImageName, QuickTimeImageName, Html5VideoImageName, Html5AudioImageName };
			foreach(string name in names)
				list.Add(new ImageInfo(name));
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ObjectContainerImagesError"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Error {
			get { return GetImage(ErrorImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ObjectContainerImagesImage"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Image {
			get { return GetImage(ImageImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ObjectContainerImagesFlash"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Flash {
			get { return GetImage(FlashImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ObjectContainerImagesVideo"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Video {
			get { return GetImage(VideoImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ObjectContainerImagesAudio"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Audio {
			get { return GetImage(AudioImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ObjectContainerImagesQuickTime"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties QuickTime {
			get { return GetImage(QuickTimeImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ObjectContainerImagesHtml5Video"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Html5Video {
			get { return GetImage(Html5VideoImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ObjectContainerImagesHtml5Audio"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Html5Audio {
			get { return GetImage(Html5AudioImageName); }
		}
	}
}
