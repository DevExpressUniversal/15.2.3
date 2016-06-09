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
using System.ComponentModel;
using System.Reflection;
using DevExpress.Utils.About;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Design {
	public sealed class TagNameEditor : DropDownUITypeEditorBase {
		public static string[] TagNames {
			get { return TagUtils.TagNames; }
		}
		protected override void ApplySelectedValue(System.Windows.Forms.ListBox valueList, ITypeDescriptorContext context) {
			ToolbarCustomCssListEditItem item = GetItem(context);
			item.TagName = TagNames[valueList.SelectedIndex];
		}
		protected override void SetInitiallySelectedValue(System.Windows.Forms.ListBox valueList, ITypeDescriptorContext context) {
			ToolbarCustomCssListEditItem item = GetItem(context);
			valueList.SelectedIndex = GetSeletedItemIndex(item);
		}
		protected override void FillValueList(System.Windows.Forms.ListBox valueList, ITypeDescriptorContext context) {
			valueList.Items.AddRange(TagNames);
		}
		protected override PropertyDescriptor GetChangedPropertyDescriptor(object component) {
			return null;
		}
		protected override object GetComponent(ITypeDescriptorContext context) {
			object obj = GetItem(context);
			while(obj != null && !(obj is ASPxHtmlEditor)) {
				if(obj is CollectionItem)
					obj = ReflectionUtils.GetPropertyValue(obj, "Collection");
				else if(obj is PropertiesBase) {
					BindingFlags binding = BindingFlags.Instance | BindingFlags.NonPublic;
					PropertyInfo propInfo = typeof(PropertiesBase).GetProperty("Owner", binding);
					obj = propInfo != null ? propInfo.GetValue(obj, new object[] { }) : null;
				} else {
					if(!ReflectionUtils.TryToGetPropertyValue(obj, "Owner", out obj))
						obj = null;
				}
			}
			return obj as ASPxHtmlEditor;
		}
		private int GetSeletedItemIndex(ToolbarCustomCssListEditItem item) {
			return Array.IndexOf<string>(TagNames, item.TagName.ToLowerInvariant());
		}
		private ToolbarCustomCssListEditItem GetItem(ITypeDescriptorContext context) {
			return context != null ? context.Instance as ToolbarCustomCssListEditItem : null;
		}
	}
}
