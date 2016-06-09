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
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	[ToolboxItem(false)]
	public class MVCxFormLayout : ASPxFormLayout {
		protected internal new void BindLayoutItem(LayoutItem layoutItem, object dataItem) {
			base.BindLayoutItem(layoutItem, dataItem);
		}
		protected override bool LayoutItemHasValidFieldName(LayoutItem layoutItem, object dataItem) {
			var item = layoutItem as MVCxFormLayoutItem;
			return base.LayoutItemHasValidFieldName(layoutItem, dataItem) || item != null && item.NestedExtensionInfo.Metadata != null;
		}
		protected override object GetControlValue(LayoutItem layoutItem, object dataItem) {
			var item = layoutItem as MVCxFormLayoutItem;
			if(item != null && dataItem == null)
				return GetModelValue(item);
			return base.GetControlValue(layoutItem, dataItem);
		}
		object GetModelValue(MVCxFormLayoutItem layoutItem) {
			var value = layoutItem.NestedExtensionInfo.Model;
			if(layoutItem.NestedExtensionType == FormLayoutNestedExtensionItemType.TokenBox)
				value = ((TokenBoxExtension)layoutItem.NestedExtensionInst).ConvertCollectionToString(value);
			return value;
		}
		protected override void EnsureLayoutItemNestedControl(LayoutItem item, object dataItem) {
			base.EnsureLayoutItemNestedControl(item, dataItem);
			var layoutItem = item as MVCxFormLayoutItem;
			if(layoutItem != null && layoutItem.DataType == null && layoutItem.NestedExtensionInfo.Metadata != null)
				layoutItem.DataType = layoutItem.NestedExtensionInfo.Metadata.ModelType;
		}
		public override bool IsClientSideAPIEnabled() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientFormLayout";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxFormLayout), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxFormLayout), Utils.FormLayoutScriptResourceName);
		}
		protected override object[] CreateItemProperties(LayoutItemBase item) {
			object[] itemProperties = base.CreateItemProperties(item);
			var layoutItem = item as MVCxFormLayoutItem;
			if(layoutItem != null){
				int itemPropertiesCount = itemProperties.Length + 1;
				Array.Resize(ref itemProperties, itemPropertiesCount);
				itemProperties[itemPropertiesCount - 1] = layoutItem.CaptionSettings.AssociatedNestedExtensionName;
			}
			return itemProperties;
		}
	}
}
