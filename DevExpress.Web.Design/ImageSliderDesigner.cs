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
using System.ComponentModel.Design;
using System.Web.UI.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxImageSliderDesignerBase : ASPxDataWebControlDesigner {
		public ASPxImageSliderBase ImageSliderInternal { get; private set; }
		public override void Initialize(IComponent component) {
			ImageSliderInternal = component as ASPxImageSliderBase;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		public override bool IsThemableControl() {
			return false;
		}
		public string ImageSourceFolder {
			get { return ImageSliderInternal.ImageSourceFolder; }
			set {
				ImageSliderInternal.ImageSourceFolder = value;
				PropertyChanged("ImageSourceFolder");
			}
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Items", "Items");
		}
	}
	public class ASPxImageSliderDesigner : ASPxImageSliderDesignerBase {
		private static string itemTemplateCaption = "Item[{0}]";
		private static string[] itemTemplateNames = new string[] { "Template", "TextTemplate", "ThumbnailTemplate" };
		private static string[] controlTemplateNames = new string[] { "ItemTemplate", "ItemTextTemplate", "ItemThumbnailTemplate" };
		protected ASPxImageSlider ImageSlider {
			get { return ImageSliderInternal as ASPxImageSlider; }
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ASPxImageSliderDesignerActionList(this);
		}
		public bool SeoFriendly {
			get { return ImageSlider.SeoFriendly; }
			set {
				ImageSlider.SeoFriendly = value;
				PropertyChanged("SeoFriendly");
			}
		}
		public bool EnableLoopNavigation {
			get { return ImageSlider.SettingsImageArea.EnableLoopNavigation; }
			set {
				ImageSlider.SettingsImageArea.EnableLoopNavigation = value;
				PropertyChanged("SettingsImageArea.EnableLoopNavigation");
			}
		}
		public NavigationBarMode Mode {
			get { return ImageSlider.SettingsNavigationBar.Mode; }
			set {
				ImageSlider.SettingsNavigationBar.Mode = value;
				PropertyChanged("SettingsNavigationBar.Mode");
			}
		}
		public NavigationBarPosition Position {
			get { return ImageSlider.SettingsNavigationBar.Position; }
			set {
				ImageSlider.SettingsNavigationBar.Position = value;
				PropertyChanged("SettingsNavigationBar.Position");
			}
		}
		public NavigationDirection NavigationDirection {
			get { return ImageSlider.SettingsImageArea.NavigationDirection; }
			set {
				ImageSlider.SettingsImageArea.NavigationDirection = value;
				PropertyChanged("SettingsImageArea.NavigationDirection");
			}
		}
		public AnimationType AnimationType {
			get { return ImageSlider.SettingsImageArea.AnimationType; }
			set {
				ImageSlider.SettingsImageArea.AnimationType = value;
				PropertyChanged("SettingsImageArea.AnimationType");
			}
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new ImageSliderItemsOwner(ImageSlider, DesignerHost)));
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			for(int i = 0; i < ImageSlider.Items.Count; i++) {
				TemplateGroup templateGroup = new TemplateGroup(string.Format(itemTemplateCaption, i));
				for(int j = 0; j < itemTemplateNames.Length; j++) {
					TemplateDefinition templateDefinition = new TemplateDefinition(this, itemTemplateNames[j],
						ImageSlider.Items[i], itemTemplateNames[j]);
					templateDefinition.SupportsDataBinding = true;
					templateGroup.AddTemplateDefinition(templateDefinition);
				}
				templateGroups.Add(templateGroup);
			}
			for(int i = 0; i < controlTemplateNames.Length; i++) {
				TemplateGroup templateGroup = new TemplateGroup(controlTemplateNames[i]);
				TemplateDefinition templateDefinition = new TemplateDefinition(this, controlTemplateNames[i],
					Component, controlTemplateNames[i]);
				templateDefinition.SupportsDataBinding = true;
				templateGroup.AddTemplateDefinition(templateDefinition);
				templateGroups.Add(templateGroup);
			}
			return templateGroups;
		}
	}
	public class ASPxImageSliderDesignerActionListBase : ASPxWebControlDesignerActionList {
		protected ASPxImageSliderDesignerBase ImageSliderDesignerInternal {
			get { return Designer as ASPxImageSliderDesignerBase; }
		}
		public ASPxImageSliderDesignerActionListBase(ASPxImageSliderDesignerBase designer)
			: base(designer) {
		}
		public string ImageSourceFolder {
			get { return ImageSliderDesignerInternal.ImageSourceFolder; }
			set { ImageSliderDesignerInternal.ImageSourceFolder = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("ImageSourceFolder", "Image Source Folder", "Source Folder", @"Specify a virtual path, e.g., ~\photos\"));
			return collection;
		}
	}
	public class ASPxImageSliderDesignerActionList : ASPxImageSliderDesignerActionListBase {
		protected ASPxImageSliderDesigner ImageSliderDesigner {
			get { return ImageSliderDesignerInternal as ASPxImageSliderDesigner; }
		}
		public ASPxImageSliderDesignerActionList(ASPxImageSliderDesignerBase designer)
			: base(designer) {
		}
		public bool SeoFriendly {
			get { return ImageSliderDesigner.SeoFriendly; }
			set { ImageSliderDesigner.SeoFriendly = value; }
		}
		public bool EnableLoopNavigation {
			get { return ImageSliderDesigner.EnableLoopNavigation; }
			set { ImageSliderDesigner.EnableLoopNavigation = value; }
		}
		public NavigationBarMode Mode {
			get { return ImageSliderDesigner.Mode; }
			set { ImageSliderDesigner.Mode = value; }
		}
		public NavigationBarPosition Position {
			get { return ImageSliderDesigner.Position; }
			set { ImageSliderDesigner.Position = value; }
		}
		public NavigationDirection NavigationDirection {
			get { return ImageSliderDesigner.NavigationDirection; }
			set { ImageSliderDesigner.NavigationDirection = value; }
		}
		public AnimationType AnimationType {
			get { return ImageSliderDesigner.AnimationType; }
			set { ImageSliderDesigner.AnimationType = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("SeoFriendly", "Seo Friendly"));
			collection.Add(new DesignerActionPropertyItem("EnableLoopNavigation", "Enable Loop Navigation"));
			collection.Add(new DesignerActionPropertyItem("AnimationType", "Animation Type"));
			collection.Add(new DesignerActionPropertyItem("NavigationDirection", "Navigation Direction"));
			collection.Add(new DesignerActionPropertyItem("Mode", "Navigation Bar Mode"));
			collection.Add(new DesignerActionPropertyItem("Position", "Navigation Bar Position"));
			return collection;
		}
	}
	public class ImageSliderItemsOwner : FlatCollectionItemsOwner<ImageSliderItem> {
		public ImageSliderItemsOwner(object control, IServiceProvider provider) 
			: base(control, provider, ((ASPxImageSlider)control).Items) {
		}
	}
}
