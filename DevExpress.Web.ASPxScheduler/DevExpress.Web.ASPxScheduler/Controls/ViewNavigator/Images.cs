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
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Internal;
namespace DevExpress.Web.ASPxScheduler {
	public class ViewNavigatorImages : ImagesBaseNoLoadingPanel {
		#region Fields
		internal const string Category = "ViewNavigator";
		internal const string ForwardName = "Forward";		
		internal const string BackwardName = "Backward";
		internal const string TodayName = "Today";
		internal const string DownName = "Down";
		#endregion
		public ViewNavigatorImages(ISkinOwner owner)
			: base(owner) {
		}
		#region Properties
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties Backward {
			get { return GetButtonImage(BackwardName); }
		}
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties Today {
			get { return GetButtonImage(TodayName); }
		}
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties Forward {
			get { return GetButtonImage(ForwardName); }
		}
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties Down {
			get { return GetButtonImage(DownName); }
		}
		#endregion
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			Type type = typeof(ButtonImageProperties);
			list.Add(new ImageInfo(BackwardName, ImageFlags.IsPng | ImageFlags.HasHottrackState, 4, 7, "<", type, BackwardName));
			list.Add(new ImageInfo(TodayName, type));
			list.Add(new ImageInfo(ForwardName, ImageFlags.IsPng | ImageFlags.HasHottrackState, 4, 7, ">", type, ForwardName));
			list.Add(new ImageInfo(DownName, ImageFlags.IsPng | ImageFlags.HasHottrackState, 7, 7, "V", type, DownName));
		}
		protected override Type GetResourceType() {
			return typeof(ASPxResourceNavigator);
		}
		protected override string GetResourceImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxSchedulerImages.ResourceImagesPath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetImageCategory() {
			return Category;
		}
		protected ButtonImageProperties GetButtonImage(string name) {
			return (ButtonImageProperties)GetImageBase(name);
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxScheduler.SchedulerSpriteCssResourceName;
		}
	}
}
