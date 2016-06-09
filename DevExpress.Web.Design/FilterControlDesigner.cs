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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.About;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxFilterControlDesigner : ASPxWebControlDesigner {
		private ASPxFilterControl filterControl = null;
		public ASPxFilterControl FilterControl {
			get { return filterControl; }
		}
		public override void Initialize(IComponent component) {			
			base.Initialize(component);
			this.filterControl = (ASPxFilterControl)component;
			EnsureReferences("DevExpress.Web" + AssemblyInfo.VSuffix);
		}
		protected override string GetDesignTimeHtmlInternal() {
			ASPxFilterControlBase control = (ASPxFilterControlBase)ViewControl;
			string oldExpr = null, expr = control.FilterExpression;
			if(string.IsNullOrEmpty(expr)) {
				oldExpr = expr;
				control.FilterExpression = "Property1='value1' and Property2='value2' or Property3='value3'";
			}
			string result = base.GetDesignTimeHtmlInternal();
			if(oldExpr != null) control.FilterExpression = oldExpr;
			return result;
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ASPxWebControlDesignerActionList(this);
		}
		public override void ShowAbout() {
			FilterControlAboutDialogHelper.ShowAbout(Component.Site);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new FilterControlCommonDesigner(filterControl, DesignerHost)));
		}
	}
	public class FilterControlAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxFilterControl), ProductKind.DXperienceASP);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if (ShouldShowTrialAbout(typeof(ASPxFilterControl)))
				ShowAbout(provider);
		}
	}
	public class FilterControlCommonDesigner : CommonFormDesigner {
		public FilterControlCommonDesigner(ASPxFilterControl filterControl, IServiceProvider provider) 
			: base(new FilterControlColumnsOwner(filterControl, provider)) {
		}
		protected override void CreateItemsItem() {
			MainGroup.Add(CreateDesignerItem(ItemsOwner, typeof(FilterControlEditorFrame), ColumnsItemImageIndex));
		}
	}
	public class FilterControlChooseDataTypeForm : ChooseDataTypeForm {
		const string ChooseDataTypeDescription = "The ASPxFilterControl control can automatically generate columns for objects of a particular type.\nSpecify the type of objects whose fields should be edited.";
		const string AllowHierarchyCheckBoxText = "Allow hierarchy columns";
		CheckBox AllowHierarchyCheckBox { get; set; }
		public bool AllowHierarchy { get { return AllowHierarchyCheckBox.Checked; } }
		public FilterControlChooseDataTypeForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue, ChooseDataTypeDescription) { 
		}
		protected override void AddControlsToMainPanel(Panel mainPanel) {
			AllowHierarchyCheckBox = new CheckBox();
			AllowHierarchyCheckBox.Location = new Point(0, 90);
			AllowHierarchyCheckBox.Width = 240;
			AllowHierarchyCheckBox.Text = AllowHierarchyCheckBoxText;
			mainPanel.Controls.Add(AllowHierarchyCheckBox);
		}
	}
	public class FilterControlEditorFrame : ItemsEditorFrame {
		protected override void MenuItemClick_RetriveFields() {
			((FilterControlColumnsOwner)ItemsOwner).RetrieveFields();
		}
		protected override void CreateRetrieveFieldsPopup() { }
	}
	public class FilterControlColumnsOwner : ItemsEditorOwner {
		ASPxFilterControl FilterControl { get { return (ASPxFilterControl)Component; } }
		public FilterControlColumnsOwner(ASPxFilterControl filterControl, IServiceProvider provider)
			: base(filterControl, "Columns", provider, filterControl.Columns) {
		}
		public void RetrieveFields() {
			BeginUpdate();
			var dataTypeContainer = new DataTypeContainer();
			var dialogForm = new FilterControlChooseDataTypeForm(FilterControl, null, ServiceProvider, dataTypeContainer);
			DesignUtils.ShowDialog(ServiceProvider, dialogForm);
			if(dataTypeContainer.DataType != null)
				FilterControl.BindToSource(dataTypeContainer.DataType, dialogForm.AllowHierarchy);
			EndUpdate();
		}
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.AddItem,
				DesignEditorMenuRootItemActionType.InsertBefore, 
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp, 
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.MoveLeft,
				DesignEditorMenuRootItemActionType.MoveRight,
				DesignEditorMenuRootItemActionType.RetriveFields
			};
		}
		protected override List<DesignEditorMenuRootItemActionType> GetContextMenuActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp,
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.MoveLeft,
				DesignEditorMenuRootItemActionType.MoveRight,
				DesignEditorMenuRootItemActionType.SelectAll
			};
		}
		protected override void FillItemTypes() {			
			AddItemType(typeof(FilterControlTextColumn), "Text Column", "Text", TextImageResource);
			AddItemType(typeof(FilterControlButtonEditColumn), "Button Edit Column", "Button Edit", ButtonEditColumnImageResource);
			AddItemType(typeof(FilterControlMemoColumn), "Memo Column", "Memo", MemoImageResource);
			AddItemType(typeof(FilterControlHyperLinkColumn), "Hyperlink Column", "Hyperlink", HyperlinkImageResource);
			AddItemType(typeof(FilterControlCheckColumn), "Check Column", "Check", CheckColumnImageResource);
			AddItemType(typeof(FilterControlDateColumn), "Date Column", "Date", DateEditImageResource);
			AddItemType(typeof(FilterControlSpinEditColumn), "Spin Edit Column", "Spin Edit", SpinEditImageResource);
			AddItemType(typeof(FilterControlComboBoxColumn), "Combo Box Column", "Combo Box", ComboboxImageResource);
			AddItemType(typeof(FilterControlComplexTypeColumn), "Complex Type Column", string.Empty, BandColumnImageResource);
		}
		public override Type GetDefaultItemType() {
			return typeof(FilterControlTextColumn);
		}
		protected override internal string GetItemPropertiesTabCaption() {
			return "Columns Properties";
		}
		protected override internal string GetNavBarItemsGroupName() {
			return "Columns";
		}
		protected override DesignEditorDescriptorItem CreateEditorDescriptorItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			if(actionType == DesignEditorMenuRootItemActionType.RetriveFields)
				return CreateRetrieveFieldsMenuItem();
			return base.CreateEditorDescriptorItem(actionType, isToolbarMenu);
		}
		protected DesignEditorDescriptorItem CreateRetrieveFieldsMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.RetriveFields };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Caption = "Retrieve fields";
			item.Enabled = true;
			item.ImageIndex = GetResourceImageIndex(RetrieveFieldsItemImageResource);
			return item;
		}
		protected override DesignEditorDescriptorItem CreateAddItemMenuItem() {
			var item = base.CreateAddItemMenuItem();
			item.EditorType = DesignEditorDescriptorItemType.DropDownButton;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateInsertBeforeMenuItem() {
			var item = base.CreateInsertBeforeMenuItem();
			item.EditorType = DesignEditorDescriptorItemType.DropDownButton;
			return item;
		}
		public override List<string> GetViewDependedProperties() {
			var result = base.GetViewDependedProperties();
			result.Add("PropertyName");
			return result;
		}
	}
}
