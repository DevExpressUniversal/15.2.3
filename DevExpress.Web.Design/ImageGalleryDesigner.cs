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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Web.UI.Design;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxImageGalleryDesigner : ASPxDataViewControlDesignerBase {
		public ASPxImageGallery ImageGallery { get { return DataView as ASPxImageGallery; } }
		private string itemTemplateCaption = "Item[{0}]";
		private string[] controlTemplateNames = new string[] { 
			"ItemTextTemplate", 
			"FullscreenViewerTextTemplate",
			"FullscreenViewerItemTextTemplate",
		};
		private string[] controlItemTemplatesNames = new string[] {
			"TextTemplate",
			"FullscreenViewerTextTemplate"
		};
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Items", "Items");
		}
		protected override string[] ControlTemplateNames {
			get { return controlTemplateNames; }
		}
		protected override IEnumerable GetDesignTimeDataSource() {
			List<ImageGalleryItem> sampleData = new List<ImageGalleryItem>();
			for(int i = 0; i < 15; i++)
				sampleData.Add(new ImageGalleryItem());
			return sampleData;
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		public string ImageSourceFolder {
			get { return ImageGallery.SettingsFolder.ImageSourceFolder; }
			set {
				ImageGallery.SettingsFolder.ImageSourceFolder = value;
				PropertyChanged("SettingsFolder.ImageSourceFolder");
			}
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new ImageGalleryItemsOwner(ImageGallery, DesignerHost)));
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			for(int i = 0; i < ImageGallery.Items.Count; i++) {
				TemplateGroup templateGroup = new TemplateGroup(string.Format(itemTemplateCaption, i));
				for(int j = 0; j < controlItemTemplatesNames.Length; j++) {
					TemplateDefinition templateDefinition = new TemplateDefinition(this, controlItemTemplatesNames[j],
						ImageGallery.Items[i], controlItemTemplatesNames[j]);
					templateDefinition.SupportsDataBinding = true;
					templateGroup.AddTemplateDefinition(templateDefinition);
				}
				templateGroups.Add(templateGroup);
			}
			return templateGroups;
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ASPxImageGalleryDesignerActionList(this);
		}
	}
	public class ASPxImageGalleryDesignerActionList : ASPxDataViewBaseDesignerActionList {
		protected ASPxImageGalleryDesigner ImageGalleryDesigner { get { return Designer as ASPxImageGalleryDesigner; } }
		public ImageGalleryItemCollection Items {
			get { return ImageGalleryDesigner.ImageGallery.Items; }
			set {
				IComponent component = ImageGalleryDesigner.Component;
				TypeDescriptor.GetProperties(component)["Items"].SetValue(component, value);
			}
		}
		public ASPxImageGalleryDesignerActionList(ASPxImageGalleryDesigner designer)
			: base(designer) {
		}
		public string ImageSourceFolder {
			get { return ImageGalleryDesigner.ImageSourceFolder; }
			set { ImageGalleryDesigner.ImageSourceFolder = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			var runDesignerItem = collection.OfType<DesignerActionItem>().FirstOrDefault(i => i.DisplayName == StringResources.CommonDesigner_RunDesigner);
			int pos = runDesignerItem == null ? 0 : collection.IndexOf(runDesignerItem) + 1;
			collection.Insert(pos, new DesignerActionPropertyItem("ImageSourceFolder", "Image Source Folder", "Source Folder", @"Specify a virtual path, e.g., ~\photos\"));
			return collection;
		}
	}
	public class ImageGalleryItemsOwner : FlatCollectionItemsOwner<ImageGalleryItem> {
		public ImageGalleryItemsOwner(object control, IServiceProvider provider) 
			: base(control, provider, ((ASPxImageGallery)control).Items) {
		}
	}
}
