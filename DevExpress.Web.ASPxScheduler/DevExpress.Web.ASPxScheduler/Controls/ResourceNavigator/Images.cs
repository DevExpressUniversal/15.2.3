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
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxScheduler.Internal;
namespace DevExpress.Web.ASPxScheduler {
	public class ResourceNavigatorImages : ImagesBaseNoLoadingPanel {
		internal const string 
			Category = "ResourceNavigator",
			FirstName = "First",
			PrevPageName = "PrevPage",
			PrevName = "Prev",
			NextName = "Next",
			NextPageName = "NextPage",
			LastName = "Last",
			DecreaseName = "Decrease",
			IncreaseName = "Increase",
			ComboBoxDrowDownName = "ComboBoxDropDown";
		public ResourceNavigatorImages(ISkinOwner owner)
			: base(owner) {
		}
		#region Properties
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties First {
			get { return GetButtonImage(FirstName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties PrevPage {
			get { return GetButtonImage(PrevPageName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties Prev {
			get { return GetButtonImage(PrevName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties Next {
			get { return GetButtonImage(NextName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties NextPage {
			get { return GetButtonImage(NextPageName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties Last {
			get { return GetButtonImage(LastName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties Decrease {
			get { return GetButtonImage(DecreaseName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties Increase {
			get { return GetButtonImage(IncreaseName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageProperties ComboBoxDropDown {
			get { return GetButtonImage(ComboBoxDrowDownName); }
		}
		#endregion
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			Type type = typeof(ButtonImageProperties);
			ImageFlags flags = ImageFlags.IsPng | ImageFlags.HasDisabledState | ImageFlags.HasHottrackState;
			list.Add(new ImageInfo(FirstName, flags, 19, 19, "|<<", type, FirstName));
			list.Add(new ImageInfo(PrevPageName, flags, 19, 19, "<<", type, PrevPageName));
			list.Add(new ImageInfo(PrevName, flags, 19, 19, "<", type, PrevName));
			list.Add(new ImageInfo(NextName, flags, 19, 19, ">", type, NextName));
			list.Add(new ImageInfo(NextPageName, flags, 19, 19, ">>", type, NextPageName));
			list.Add(new ImageInfo(LastName, flags, 19, 19, ">>|", type, LastName));
			list.Add(new ImageInfo(IncreaseName, flags, 19, 19, "+", type, IncreaseName));
			list.Add(new ImageInfo(DecreaseName, flags, 19, 19, "-", type, DecreaseName));
			list.Add(new ImageInfo(ComboBoxDrowDownName, ImageFlags.HasNoResourceImage, type));
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
