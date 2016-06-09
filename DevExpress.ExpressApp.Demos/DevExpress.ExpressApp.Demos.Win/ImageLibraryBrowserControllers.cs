#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.ExpressApp.Demos.Win.PropertyEditors;
namespace DevExpress.ExpressApp.Demos.Win {
	public class CategoryListViewController : ViewController {
		private SimpleAction openCategory;
		private void openCategory_Execute(object sender, SimpleActionExecuteEventArgs e) {
			e.ShowViewParameters.CreatedView = Application.CreateDetailView(Application.CreateObjectSpace(e.CurrentObject.GetType()), e.CurrentObject);
		}
		private void CategoryListViewController_CreateCustomCurrentObjectDetailView(object sender, CreateCustomCurrentObjectDetailViewEventArgs arg) {
			if(arg.ListViewCurrentObject != null) {
				arg.DetailViewId = Application.GetDetailViewId(arg.ListViewCurrentObject.GetType());
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			ListView listView = View as ListView;
			if(listView != null) {
				listView.CreateCustomCurrentObjectDetailView += new EventHandler<CreateCustomCurrentObjectDetailViewEventArgs>(CategoryListViewController_CreateCustomCurrentObjectDetailView);
				if(listView.Editor is TreeListEditor) {
					((TreeListEditor)listView.Editor).ClearFocusedObjectOnMouseClick = false;
				}
			}
		}
		protected override void OnDeactivated() {
			if(View is ListView) {
				((ListView)View).CreateCustomCurrentObjectDetailView -= new EventHandler<CreateCustomCurrentObjectDetailViewEventArgs>(CategoryListViewController_CreateCustomCurrentObjectDetailView);
			}
			base.OnDeactivated();
		}
		public CategoryListViewController()
			: base() {
			TargetObjectType = typeof(ImageBrowserCategory);
			TargetViewType = ViewType.ListView;
			openCategory = new SimpleAction(this, "OpenCategory", PredefinedCategory.Edit);
			openCategory.Caption = "Open";
			openCategory.ImageName = "MenuBar_OpenObject";
			openCategory.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			openCategory.Execute += new SimpleActionExecuteEventHandler(openCategory_Execute);
		}
	}
	public class ImageBrowserDetailViewController : ViewController {
		private SimpleAction fillCategories;
		private SimpleAction categorizeImages;
		private void fillCategories_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ImageSourceBrowserBase browser = e.CurrentObject as ImageSourceBrowserBase;
			if(browser != null && !string.IsNullOrEmpty(browser.ImageSourceName)) {
				browser.AddImageCategories(browser.ImageSource.GetImageNames());
				browser.BuildTreeNodes();
			}
		}
		private void categorizeImages_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ImageSourceBrowserBase browser = e.CurrentObject as ImageSourceBrowserBase;
			if(browser != null && !string.IsNullOrEmpty(browser.ImageSourceName)) {
				browser.BuildTreeNodes();
			}
		}
		public ImageBrowserDetailViewController()
			: base() {
			TargetObjectType = typeof(ImageSourceBrowserBase);
			TargetViewType = ViewType.DetailView;
			fillCategories = new SimpleAction(this, "FillCategories", PredefinedCategory.Edit);
			fillCategories.Execute += new SimpleActionExecuteEventHandler(fillCategories_Execute);
			categorizeImages = new SimpleAction(this, "CategorizeImages", PredefinedCategory.Edit);
			categorizeImages.Execute += new SimpleActionExecuteEventHandler(categorizeImages_Execute);
		}
	}
	public enum ImagePreviewListMode { List, Thumbnails }
	public class ImagePreviewBaseListWinViewController : ViewController<ListView> {
		private SingleChoiceAction listViewMode;
		private void listViewMode_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			if(e.SelectedChoiceActionItem.Data is ImagePreviewListMode && ((ImagePreviewListMode)(e.SelectedChoiceActionItem.Data)) == ImagePreviewListMode.Thumbnails) {
				View.Model.EditorType = typeof(WinThumbnailEditor);
			}
			else {
				View.Model.EditorType = typeof(DevExpress.ExpressApp.Win.Editors.GridListEditor);
			}
			View.LoadModel();
			Frame.Template.SetView(View);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(View.Model.EditorType == typeof(WinThumbnailEditor)) {
				listViewMode.SelectedItem = listViewMode.Items.Find(ImagePreviewListMode.Thumbnails);
			}
			else {
				listViewMode.SelectedItem = listViewMode.Items.Find(ImagePreviewListMode.List);
			}
		}
		public ImagePreviewBaseListWinViewController()
			: base() {
			TargetObjectType = typeof(ImagePreviewObject);
			listViewMode = new SingleChoiceAction(this, "ImagePreviewListMode", PredefinedCategory.View);
			listViewMode.Items.Add(new ChoiceActionItem(ImagePreviewListMode.List.ToString(), ImagePreviewListMode.List));
			listViewMode.Items.Add(new ChoiceActionItem(ImagePreviewListMode.Thumbnails.ToString(), ImagePreviewListMode.Thumbnails));
			listViewMode.Execute += new SingleChoiceActionExecuteEventHandler(listViewMode_Execute);
		}
	}
}
