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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraEditors.Design {
	[ToolboxItem(false)]
	public class PersistentRepositoryEditor : BaseRepositoryEditor {
		protected RepositoryItemCollection itemCollection;
		public PersistentRepositoryEditor() : base(6) {
		}
		protected override int FindElement(object component) {
			RepositoryItem item = component as RepositoryItem;
			if(item != null) return Registrator.EditorRegistrationInfo.Default.Editors.IndexOf(Registrator.EditorRegistrationInfo.Default.Editors[item.EditorTypeName]);
			return -1;
		}
		protected override object GetDefaultElement() {
			if(EditingObject == null) return null;
			if(lastElement == null) lastElement = Registrator.EditorRegistrationInfo.Default.Editors["TextEdit"];
			return lastElement;
		}
		protected override bool GetElementVisible(object element) {
			EditorClassInfo info = element as EditorClassInfo;
			return info != null && info.DesignTimeVisible && info.AllowInplaceEditing == ShowInContainerDesigner.Anywhere;
		}
		protected override object GetElement(int index) {
			if(index < 0 || index >= Registrator.EditorRegistrationInfo.Default.Editors.Count) return null;
			return Registrator.EditorRegistrationInfo.Default.Editors[index];
		}
		protected override string GetElementText(object element) {
			EditorClassInfo info = element as EditorClassInfo;
			if(info == null) return "TextEdit";
			return info.Name;
		}
		protected override ArrayList GetSortElementList() {
			ArrayList list = new ArrayList();
			foreach(EditorClassInfo info in Registrator.EditorRegistrationInfo.Default.Editors)
				list.Add(info.Name);
			list.Sort();
			return list;
		}
		protected override object GetElementByName(string name) {
			return Registrator.EditorRegistrationInfo.Default.Editors[name];
		}
		protected override int GetElementCount() {
			return Registrator.EditorRegistrationInfo.Default.Editors.Count;
		}
		protected override Image GetElementImage(object element) {
			EditorClassInfo info = element as EditorClassInfo;
			if(info == null) return null;
			Bitmap bitmap = info.Image as Bitmap; 
			if(bitmap != null) {
				bitmap.MakeTransparent();
				return bitmap;
			}
			return info.Image;
		}
		protected override void InitializePopupMenu() {
			EditorRegistrationInfo.Default.RegisterUserItems(GetComponentService(typeof(IDesignerHost)) as IDesignerHost);
			base.InitializePopupMenu();
		}
		protected virtual RepositoryItemCollection ItemCollection {
			get {
				if(EditingObject is RepositoryItemCollection) return EditingObject as RepositoryItemCollection;
				if(EditingObject is EditorsRepositoryBase) {
					EditorsRepositoryBase rb = EditingObject as EditorsRepositoryBase;
					return rb.Items;
				}
				EditorContainer edCont = EditingObject as EditorContainer;
				ComponentEditorContainer compEdCont = EditingObject as ComponentEditorContainer;
				if(edCont != null) return edCont.RepositoryItems;
				if(compEdCont != null) return compEdCont.RepositoryItems;
				return itemCollection;
			}
		}
		public override void InitComponent() {
			if(ItemCollection == null)
				throw new WarningException("Can't initialize RepositoryEditor");
		}
		protected override void AddNewItem(object item) {
			EditorClassInfo info = item as EditorClassInfo;
			if(info != null) {
				ItemCollection.Add(info.Name);
				this.lastElement = info;
				UpdateButtonText();
				RefreshListBox();
				listView1.SelectedItem = itemsCore[itemsCore.Count - 1];
			}
		}
		protected override IList GetComponentCollection() { return ItemCollection; } 
		protected override bool CanRemoveComponent(object component) {
			RepositoryItem item = component as RepositoryItem;
			return item != null && item.LinkCount == 0;
		}
	}
}
