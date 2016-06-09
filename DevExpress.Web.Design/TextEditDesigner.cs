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
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Web.Design;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils.Design;
namespace DevExpress.Web.Design {
	public class ASPxTextBoxDesigner : ASPxEditDesigner {
	}
	public class ASPxMemoDesigner : ASPxEditDesigner {
	}
	public class ASPxButtonEditDesigner: ASPxEditDesigner {
		private ASPxButtonEditBase buttonEdit = null;
		private static string buttonTemplateCaption = "ButtonTemplate";
		private static string buttonTemplateName = "ButtonTemplate";
		protected internal ASPxButtonEditBase ButtonEdit {
			get { return buttonEdit; }
		}
		protected internal string ButtonTemplateCaption {
			get { return buttonTemplateCaption; }
		}
		protected internal string ButtonTemplateName {
			get { return buttonTemplateName; }
		}
		public override void Initialize(IComponent component) {
			buttonEdit = (ASPxButtonEditBase)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Buttons", "Buttons");
		}
		protected override string GetBaseProperty() {
			return "Properties.Buttons";
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ButtonEditDesignerActionList(this);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new ButtonEditButtonsCommonFormDesigner(ButtonEdit, DesignerHost)));
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			EditButtonCollection buttons = ButtonEdit.Properties.Buttons;
			TemplateGroup templateGroup = new TemplateGroup(ButtonTemplateCaption);
			TemplateDefinition templateDefinition = new TemplateDefinition(this, ButtonTemplateName,
					buttonEdit.Properties, ButtonTemplateName, GetTemplateStyle());
			templateGroup.AddTemplateDefinition(templateDefinition);
			templateGroups.Add(templateGroup);
			return templateGroups;
		}
		protected Style GetTemplateStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(buttonEdit.GetControlStyle());
			return style;
		}
		public ClearButtonDisplayMode ClearButtonDisplayMode {
			get { return ButtonEdit.ClearButton.DisplayMode; }
			set {
				ButtonEdit.ClearButton.DisplayMode = value;
				InvokeTransactedChange(Component, (arg) => {
					ComponentChanged();
					return true;
				}, null, string.Format("{0} changed", "ClearButton"));
			}
		}
	}
	public class ButtonEditDesignerActionList : EditDesignerActionList {
		public ButtonEditDesignerActionList(ASPxButtonEditDesigner designer)
			: base(designer) { }
		protected ASPxButtonEditDesigner ButtonEditDesigner {
			get { return (ASPxButtonEditDesigner)Designer; }
		}
		public ClearButtonDisplayMode ClearButtonDisplayMode {
			get { return ButtonEditDesigner.ClearButtonDisplayMode; }
			set { ButtonEditDesigner.ClearButtonDisplayMode = value;}
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("ClearButtonDisplayMode", 
				StringResources.ASPxButtonEditActionList_ClearButtonDisplayMode, null,
				GetClearButtonHint()));
			return collection;
		}
		protected virtual string GetClearButtonHint() {
			return StringResources.ASPxButtonEditActionList_ClearButtonDisplayModeHint;
		}
	}
	class ButtonEditButtonsCommonFormDesigner : CommonFormDesigner {
		public ButtonEditButtonsCommonFormDesigner(ASPxButtonEditBase buttonEdit, IServiceProvider provider)
			: base(new ButtonEditButtonsOwner(buttonEdit, provider)) {
			ItemsImageIndex = ButtonsItemImageIndex;
		}
	}
	class ButtonEditButtonsOwner : FlatCollectionItemsOwner<EditButton> {
		public ButtonEditButtonsOwner(ASPxButtonEditBase buttonEdit, IServiceProvider provider)
			: base(buttonEdit, provider, buttonEdit.Buttons, "Buttons") {
		}
		public ButtonEditButtonsOwner(object component, IServiceProvider provider, EditButtonCollection buttons)
			: base(component, provider, buttons, "Buttons") {
		}
	}
}
