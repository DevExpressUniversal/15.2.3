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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraEditors.Repository;
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Editors;
using System.Drawing;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Win.Editors {
	public class EnumPropertyEditor : DXPropertyEditor {
		protected override object CreateControlCore() {
			return new EnumEdit(MemberInfo.MemberType);
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemEnumEdit(MemberInfo.MemberType);
		}
		public EnumPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			ImmediatePostData = model.ImmediatePostData;
		}
		public new ComboBoxEdit Control {
			get { return (ComboBoxEdit)base.Control; }
		}
	}
	public class EnumImagesLoader {
		private ImageCollection imageCollection;
		private List<ImageComboBoxItem> comboboxItems;
		protected virtual object GetEnumValue(object value) {
			return value;
		}
		public EnumImagesLoader(Type enumType) {
			Initialize(enumType);
		}
		private void Initialize(Type enumType) {
			EnumDescriptor descriptor = new EnumDescriptor(enumType);
			imageCollection = new ImageCollection();
			comboboxItems = new List<ImageComboBoxItem>(descriptor.Values.Length);
			foreach(object value in descriptor.Values) {
				DevExpress.ExpressApp.Utils.ImageInfo imageInfo = descriptor.GetImageInfo(value);
				int imageIndex = -1;
				if(!imageInfo.IsEmpty) {
					if(imageCollection.Images.Count == 0) {
						imageCollection.ImageSize = imageInfo.Image.Size;
					}
					imageCollection.AddImage(imageInfo.Image);
					imageIndex = imageCollection.Images.Count - 1;
				}
				comboboxItems.Add(new ImageComboBoxItem(descriptor.GetCaption(value), GetEnumValue(value), imageIndex));
			}
		}
		public ImageCollection Images { get { return imageCollection; } }
		public ImageComboBoxItem[] GetImageComboBoxItems() {
			return comboboxItems.ToArray();
		}
	}
	public class RepositoryItemEnumEdit : RepositoryItemImageComboBox {
		internal const string EditorName = "EnumEdit";
		internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(EnumEdit),
					typeof(RepositoryItemEnumEdit), typeof(ImageComboBoxEditViewInfo),
					 new ImageComboBoxEditPainter(), true, EditImageIndexes.ImageComboBoxEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
			}
		}
		public override string EditorTypeName { get { return EditorName; } }
		static RepositoryItemEnumEdit() {
			RepositoryItemEnumEdit.Register();
		}
		public void Init(Type type) {
			EnumImagesLoader loader = new EnumImagesLoader(type);
			Items.AddRange(loader.GetImageComboBoxItems());
			if(loader.Images.Images.Count > 0) {
				SmallImages = loader.Images;
			}
		}
		public RepositoryItemEnumEdit(Type type)
			: this() {
			Init(type);
		}
		public RepositoryItemEnumEdit() {
			TextEditStyle = TextEditStyles.DisableTextEditor;
			ShowDropDown = ShowDropDown.SingleClick;
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class EnumEdit : ImageComboBoxEdit {
		static EnumEdit() {
			RepositoryItemEnumEdit.Register();
		}
		public EnumEdit() {
			Properties.TextEditStyle = TextEditStyles.Standard;
			Properties.ReadOnly = true;
		}
		public EnumEdit(Type type) {
			Height = WinPropertyEditor.TextControlHeight;
			((RepositoryItemEnumEdit)this.Properties).Init(type);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter) {
				ClosePopup();
			}
			base.OnKeyDown(e);
		}
		public override string EditorTypeName { get { return RepositoryItemEnumEdit.EditorName; } }
	}
}
