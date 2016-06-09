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
using System.Text;
using System.Linq;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Globalization;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.XtraEditors.Registrator;
namespace DevExpress.XtraEditors.Design {
	public class EditorTypeEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			IWindowsFormsEditorService editorService = null;
			BaseEditActionList editList = null;
			BaseEdit edit = null;
			if(provider != null) editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if(context != null) editList = context.Instance as BaseEditActionList;
			if(editList != null) edit = editList.Component as BaseEdit;
			if(provider != null)
				EditorRegistrationInfo.Default.RegisterUserItems(provider.GetService(typeof(IDesignerHost)) as IDesignerHost);
			if(editorService != null && edit != null) {
				using(EditorListBox listBox = new EditorListBox(editorService)) {
					listBox.Populate(edit.EditorTypeName);
					editorService.DropDownControl(listBox);
					DevExpress.XtraEditors.Registrator.EditorClassInfo info = listBox.SelectedValue as DevExpress.XtraEditors.Registrator.EditorClassInfo;
					if(info != null) value = info.Name;
				}
			}
			return base.EditValue(context, provider, value);
		}
		public class EditorListBox : ImageListBoxControl {
			IWindowsFormsEditorService editorService;
			ImageCollection collection;
			public EditorListBox(IWindowsFormsEditorService editorService) {
				this.editorService = editorService;
				this.SortOrder = SortOrder.Ascending;
				this.collection = new ImageCollection();
				Height = 200;
				Width = 200;
			}
			protected override void Dispose(bool disposing) {
				if(disposing) {
					ImageList = null;
					this.collection.Dispose();
				}
				base.Dispose(disposing);
			}
			public void Populate(string typeName) {
				ImageList = collection;
				bool selectedValueSet = false;
				List<ImageListBoxItem> items = new List<ImageListBoxItem>();
				foreach(DevExpress.XtraEditors.Registrator.EditorClassInfo info in Registrator.EditorRegistrationInfo.Default.Editors) {
					if(!info.DesignTimeVisible || info.AllowInplaceEditing != ShowInContainerDesigner.Anywhere) continue;
					if(info.Name == "RichTextEdit") continue;
					Image image = info.Image;
					if(image == null) image = new Bitmap(16, 16);
					int index = collection.Images.Add(image);
					ImageListBoxItem item = new ImageListBoxItem(info, index);
					items.Add(item);
					if(typeName == info.Name) {
						SelectedValue = info;
						selectedValueSet = true;
					}
				}
				Items.AddRange(items.OrderBy(q => ((EditorClassInfo)q.Value).Name).ToArray());
				if(!selectedValueSet) SelectedIndex = -1;
			}
			protected override void OnMouseClick(MouseEventArgs e) {
				base.OnMouseClick(e);
				editorService.CloseDropDown();
			}
		}
	}
}
