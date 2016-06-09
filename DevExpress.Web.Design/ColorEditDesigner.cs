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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxColorEditDesigner : ASPxButtonEditDesigner {
		protected ASPxColorEdit ColorEdit {
			get { return Component as ASPxColorEdit; }
		}
		protected internal bool EnableCustomColors {
			get { return ColorEdit.EnableCustomColors; }
			set { 
				ColorEdit.EnableCustomColors = value;
				PropertyChanged("EnableCustomColors");
			}
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Items", "Items");
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new ColorEditCommonFormDesigner(ColorEdit, DesignerHost)));
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection groups = base.CreateTemplateGroups();
			foreach(TemplateGroup group in groups) {
				if(group.GroupName == ButtonTemplateName) {
					groups.Remove(group);
					break;
				}
			}
			return groups;
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ColorEditDesignerActionList(this);
		}
	}
	public class ColorEditDesignerActionList : ButtonEditDesignerActionList {
		public ColorEditDesignerActionList(ASPxColorEditDesigner designer)
			: base(designer) {
		}
		protected new ASPxColorEditDesigner EditDesigner {
			get { return (ASPxColorEditDesigner)base.EditDesigner; }
		}
		public bool EnableCustomColors {
			get { return EditDesigner.EnableCustomColors; }
			set { EditDesigner.EnableCustomColors = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("EnableCustomColors", 
				StringResources.ActionList_EnableCustomColors, 
				StringResources.ActionList_MiscCategory, 
				StringResources.ActionList_EnableCustomColorsDescription));
			return collection;
		}
		protected override string GetClearButtonHint() {
			return StringResources.ASPxButtonEditActionList_ClearButtonDisplayModeAllowNullHint;
		}
	}
	public class ColorEditCommonFormDesigner : CommonFormDesigner {
		public ColorEditCommonFormDesigner(ASPxColorEdit colorEdit, IServiceProvider provider)
			: base(colorEdit, provider) {
		}
		ASPxColorEdit ColorEdit { get { return (ASPxColorEdit)Control; } }
		protected override void CreateMainGroupItems() {
			AddItemsItem();
			AddButtonsItem();
			CreateClientSideEventsItem();
		}
		protected void AddItemsItem() {
			MainGroup.Add(CreateDesignerItem(new ColorEditColorItemOwner(ColorEdit, Provider), typeof(ItemsEditorFrame), ItemsItemImageIndex));
		}
		protected void AddButtonsItem() {
			MainGroup.Add(CreateDesignerItem(new ButtonEditButtonsOwner((ASPxButtonEditBase)ColorEdit, Provider), typeof(ItemsEditorFrame), ButtonsItemImageIndex));
		}
	}
	public class ColorEditColorItemOwner : FlatCollectionItemsOwner<ColorEditItem> {
		public ColorEditColorItemOwner(ASPxColorEdit colorEdit, IServiceProvider provider)
			: base(colorEdit, provider, colorEdit.Items, "ColorEdit Items") {
		}
		public override List<string> GetViewDependedProperties() {
			var result = base.GetViewDependedProperties();
			result.Add("Color");
			return result;
		}
	}
}
