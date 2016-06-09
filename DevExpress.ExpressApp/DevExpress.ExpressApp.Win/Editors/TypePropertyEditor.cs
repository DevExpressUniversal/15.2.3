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
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Editors {
	public class TypePropertyEditor : DXPropertyEditor {
		protected virtual bool IsSuitableType(Type type) {
			return true;
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemImageComboBox();
		}
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			RepositoryItemImageComboBox repositoryItem = (RepositoryItemImageComboBox)item;
			TypeConverter typeConverter;
			TypeConverterAttribute typeConverterAttribute = MemberInfo.FindAttribute<TypeConverterAttribute>();
			if (typeConverterAttribute != null) {
				typeConverter =
					(TypeConverter)TypeHelper.CreateInstance(Type.GetType(typeConverterAttribute.ConverterTypeName));
			} else {
				typeConverter = new LocalizedClassInfoTypeConverter();
			}
			foreach(Type type in typeConverter.GetStandardValues()) {
				if(IsSuitableType(type)) {
					repositoryItem.Items.Add(new ImageComboBoxItem(typeConverter.ConvertToString(type), type));
				}
			}
		}
		protected override object CreateControlCore() {
			ImageComboBoxEdit control = new ImageComboBoxEdit();
			control.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			control.Properties.ShowDropDown = ShowDropDown.SingleClick;
			return control;
		}
		public TypePropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
				ImmediatePostData = model.ImmediatePostData;
		}
		public new ImageComboBoxEdit Control {
			get { return (ImageComboBoxEdit)base.Control; }
		}
	}
}
