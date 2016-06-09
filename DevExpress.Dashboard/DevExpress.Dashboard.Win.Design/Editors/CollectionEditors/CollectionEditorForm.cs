#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardWin.Native;
using DevExpress.Data.Utils;
using DevExpress.Utils.UI;
namespace DevExpress.DashboardWin.Design {
	abstract class CollectionEditorForm : DashboardCollectionEditorFormBase {
		#region inner
		protected class CustomCollectionEditorContentControl : CollectionEditorContentControl {
			readonly ActionContextMenu actionContextMenu;
			readonly MenuItemController menuItemController;
			protected DashboardDesigner DashboardDesigner {
				get { return GetService<SelectedContextService>().Designer; }
			}
			public CustomCollectionEditorContentControl(IServiceProvider provider, CollectionEditor collectionEditor, MenuItemController menuItemController)
				: base(provider, collectionEditor) {
				this.actionContextMenu = new ActionContextMenu(provider);
				this.menuItemController = menuItemController;
				this.buttonAdd.Image = BitmapStorage.GetBitmap(ActionNames.AddDropDown);
				this.tv.StateImageList = menuItemController.ImageCollection;
				menuItemController.ForEach((name, bitmap, type) => {
					actionContextMenu.AddItem(name, bitmap, type, OnCreateItem);
				});
			}
			protected override int GetItemImageIndex(object item) {
				return menuItemController.GetImageIndex(item);
			}
			protected override void buttonAdd_Click(object sender, EventArgs e) {
				Control control = sender as Control;
				actionContextMenu.ShowPopup(control.PointToScreen(new Point(0, control.Size.Height)));
			}
			protected override void OnAllowGlyphSkinningChanged() {
				base.OnAllowGlyphSkinningChanged();
				this.actionContextMenu.Manager.AllowGlyphSkinning = AllowGlyphSkinning;
			}
			protected virtual void OnCreateItem(object sender, EventArgs e) {
				CreateAndAddInstance(sender as Type);
			}
		}
		#endregion
		protected abstract MenuItemController MenuItemController { get; }
		protected CollectionEditorForm(IServiceProvider provider, CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
			AllowGlyphSkinning = !BitmapStorage.UseColors;
		}
		protected override CollectionEditorContentControl CreateCollectionEditorContentControl(IServiceProvider provider, CollectionEditor collectionEditor) {
			return new CustomCollectionEditorContentControl(provider, collectionEditor, MenuItemController);
		}
	}
}
