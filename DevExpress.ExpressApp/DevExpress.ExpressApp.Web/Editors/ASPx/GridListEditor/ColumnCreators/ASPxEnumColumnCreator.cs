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
using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal class ASPxEnumColumnCreator : DataColumnCreatorBase {
		public ASPxEnumColumnCreator(IModelColumn columnInfo)
			: base(columnInfo) {
		}
		protected override GridViewDataColumn CreateGridViewColumnCore() {
			GridViewDataComboBoxColumn comboBoxColumn = new GridViewDataComboBoxColumn();
			comboBoxColumn.PropertiesComboBox.EncodeHtml = false;
			comboBoxColumn.PropertiesComboBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
			comboBoxColumn.PropertiesComboBox.ShowImageInEditBox = true;
			comboBoxColumn.PropertiesComboBox.ValueType = ValueType;
			SetupComboBoxItems(comboBoxColumn.PropertiesComboBox);
			return comboBoxColumn;
		}
		private EnumDescriptor _descriptor = null;
		private EnumDescriptor EnumDescriptor {
			get {
				if(_descriptor == null) {
					_descriptor = new EnumDescriptor(GetEnumType());
				}
				return _descriptor;
			}
		}
		protected virtual Type ValueType {
			get {
				return EnumDescriptor.EnumType;
			}
		}
		protected virtual void SetupComboBoxItems(DevExpress.Web.Internal.IListEditItemsRequester control) {
			control.Items.Clear();
			foreach(object enumValue in EnumDescriptor.Values) {
				ImageInfo imageInfo = EnumDescriptor.GetImageInfo(enumValue);
				if(imageInfo.IsUrlEmpty) {
					control.Items.Add(EnumDescriptor.GetCaption(enumValue), enumValue);
				}
				else {
					control.Items.Add(EnumDescriptor.GetCaption(enumValue), enumValue, imageInfo.ImageUrl);
				}
			}
		}
		protected Type GetEnumType() {
			return Model.ModelMember.MemberInfo.MemberType;
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
	}
}
