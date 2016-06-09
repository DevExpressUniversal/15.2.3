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
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class UploadControlImages : ImagesBase {
		public const string ClearButtonImageName = "ucClearButton";
		public const string FileListUploadingItemImageName = "ucFileListUploading";
		public const string FileListPendingItemImageName = "ucFileListPending";
		public const string FileListCompleteItemImageName = "ucFileListComplete";
		public UploadControlImages(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlImagesClearButtonImage"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ImagePropertiesEx ClearButtonImage
		{
			get { return (ImagePropertiesEx)GetImageBase(ClearButtonImageName); }
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(ClearButtonImageName, ImageFlags.IsPng | ImageFlags.HasDisabledState, 16, 16,
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_ClearFileSelectionButtonToolTip),
				typeof(ImagePropertiesEx), ClearButtonImageName));
		}
		protected internal ImagePropertiesEx FileListItemUploadingImage {
			get { return (ImagePropertiesEx)GetImageBase(FileListUploadingItemImageName); }
		}
		protected internal ImagePropertiesEx FileListItemPendingImage {
			get { return (ImagePropertiesEx)GetImageBase(FileListPendingItemImageName); }
		}
		protected internal ImagePropertiesEx FileListItemCompleteImage {
			get { return (ImagePropertiesEx)GetImageBase(FileListCompleteItemImageName); }
		}
	}
}
