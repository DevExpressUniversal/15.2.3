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
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design
{
	public class ASPxTokenBoxDesigner : ASPxEditDesignerBase
	{
		public const string ItemValueTypePropertyName = "ItemValueType";
		protected ASPxTokenBox TokenBox
		{
			get { return Component as ASPxTokenBox; }
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap)
		{
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Items", "Items");
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new TokenBoxItemsOwner(Component, DesignerHost)));
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList()
		{
			return new TokenBoxDesignerActionList(this);
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl)
		{
			ASPxTokenBox edit = dataControl as ASPxTokenBox;
			if (!string.IsNullOrEmpty(edit.DataSourceID) || edit.DataSource != null || edit.Items.Count < 1)
			{
				edit.Items.Clear();
				base.DataBind(edit);
			}
		}
		protected override IEnumerable GetSampleDataSource()
		{
			return new ListEditSampleDataSource(Component);
		}
		protected PropertyDescriptor GetEditorItemValueTypeProprerty()
		{
			return TypeDescriptor.GetProperties(EditBase.GetType())[ItemValueTypePropertyName];
		}
		public string ItemValueType
		{
			get
			{
				PropertyDescriptor itemValueTypeProperty = GetEditorItemValueTypeProprerty();
				return itemValueTypeProperty != null ? itemValueTypeProperty.GetValue(EditBase).ToString() : string.Empty;
			}
			set
			{
				PropertyDescriptor valueTypeProperty = GetEditorItemValueTypeProprerty();
				if (valueTypeProperty != null)
					valueTypeProperty.SetValue(EditBase, Type.GetType(value));
			}
		}
	}
	public class TokenBoxDesignerActionList : EditDesignerActionListBase
	{
		public TokenBoxDesignerActionList(ASPxTokenBoxDesigner tokenBoxDesigner)
			: base(tokenBoxDesigner)
		{
		}
		public ASPxTokenBoxDesigner TokenBoxDesigner {
			get { return (ASPxTokenBoxDesigner)Designer; }
		}
		public enum ItemValueTypeEnum { Boolean, Int16, Int32, String, Char, Byte, Guid }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem(ASPxTokenBoxDesigner.ItemValueTypePropertyName, ASPxTokenBoxDesigner.ItemValueTypePropertyName));
			return collection;
		}
		public ItemValueTypeEnum ItemValueType
		{
			get { return (ItemValueTypeEnum)Enum.Parse(typeof(ItemValueTypeEnum), TokenBoxDesigner.ItemValueType.Replace("System.", string.Empty)); }
			set { TokenBoxDesigner.ItemValueType = string.Format("System.{0}", value); }
		}
	}
	class TokenBoxItemsOwner : FlatCollectionItemsOwner<ListEditItem> {
		public TokenBoxItemsOwner(object component, IServiceProvider provider)
			: base(component, provider, ((ASPxTokenBox)component).Items) {
		}
	}
}
