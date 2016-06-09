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
	public class EnumIntPropertyEditor<TEnum> : EnumPropertyEditor {
		protected override object CreateControlCore() {
			return new EnumIntEdit(typeof(TEnum));
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemEnumIntEdit(typeof(TEnum));
		}
		public EnumIntPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
	}
	public class RepositoryItemEnumIntEdit : RepositoryItemImageComboBox {
		internal const string EditorName = "EnumIntEdit";
		internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(EnumIntEdit),
					typeof(RepositoryItemEnumIntEdit), typeof(ImageComboBoxEditViewInfo),
					 new ImageComboBoxEditPainter(), true, EditImageIndexes.ImageComboBoxEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
			}
		}
		public override string EditorTypeName { get { return EditorName; } }
		static RepositoryItemEnumIntEdit() {
			RepositoryItemEnumIntEdit.Register();
		}
		public void Init(Type type) {
			EnumImagesLoader loader = new EnumIntImagesLoader(type);
			Items.AddRange(loader.GetImageComboBoxItems());
			if(loader.Images.Images.Count > 0) {
				SmallImages = loader.Images;
			}
		}
		public RepositoryItemEnumIntEdit(Type type)
			: this() {
			Init(type);
		}
		public RepositoryItemEnumIntEdit() {
			TextEditStyle = TextEditStyles.DisableTextEditor;
			ShowDropDown = ShowDropDown.SingleClick;
		}
	}
	public class EnumIntImagesLoader : EnumImagesLoader {
		protected override object GetEnumValue(object value) {
			return (int)value;
		}
		public EnumIntImagesLoader(Type enumType) : base(enumType) {
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class EnumIntEdit : ImageComboBoxEdit {
		static EnumIntEdit() {
			RepositoryItemEnumIntEdit.Register();
		}
		public EnumIntEdit() {
			Properties.TextEditStyle = TextEditStyles.Standard;
			Properties.ReadOnly = true;
		}
		public EnumIntEdit(Type type) {
			Height = WinPropertyEditor.TextControlHeight;
			((RepositoryItemEnumIntEdit)this.Properties).Init(type);
		}
		public override string EditorTypeName { get { return RepositoryItemEnumIntEdit.EditorName; } }
	}
}
