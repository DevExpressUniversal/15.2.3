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
using System.Linq;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Web.Internal;
using DevExpress.XtraEditors.Design;
namespace DevExpress.Web.Design {
	public class GridViewDesignerEditForm : LayoutViewDesignerEditorForm {
		public GridViewDesignerEditForm(ASPxGridView gridView)
			: base(new GridViewCommonFormDesigner(gridView, gridView.Site)) {
		}
	}
	public class GridViewCommonFormDesigner : CommonFormDesigner {
		public const string EditFormLayoutItems_NavBarItemCaption = "EditForm Layout Items";
		GridViewColumnsOwner columnsOwner;
		public GridViewCommonFormDesigner(object gridView, IServiceProvider provider)
			: base((ASPxWebControl)gridView, provider) {
			ItemsImageIndex = ColumnsItemImageIndex;
		}
		public override ItemsEditorOwner ItemsOwner {
			get {
				if(columnsOwner == null)
					columnsOwner = new GridViewColumnsOwner(GridView, Provider, GridView.Columns);
				return columnsOwner;
			}
		}
		protected virtual ASPxGridView GridView { get { return (ASPxGridView)Control; } }
		DesignerGroup SummaryGroup { get { return Groups[SummaryGroupCaption]; } }
		protected override void CreateMainGroupItems() {
			AddFeatureBrowserItem();
			CreateItemsItem();
			AddFormLayoutItemsItem();
			CreateClientSideEventsItem();
		}
		void AddFeatureBrowserItem() {
			MainGroup.Add(CreateDesignerItem("FeatureBrowser", "Feature Browser", typeof(GridViewFeatureBrowserFrame), GridView, FeatureBrowserItemImageIndex, null));
		}
		void AddFormLayoutItemsItem() {
			ItemsEditorOwner owner = new GridViewFormLayoutItemsOwner(GridView, GridView.EditFormLayoutProperties, ItemsOwner);
			MainGroup.Add(CreateDesignerItem("EditFormLayoutProperties", EditFormLayoutItems_NavBarItemCaption, typeof(FormLayoutItemsEditorFrame), Control , ItemsImageIndex, owner));
			owner = new GridViewAdaptiveDetailsFormLayoutItemsOwner(GridView, ItemsOwner);
			MainGroup.Add(CreateDesignerItem("AdaptiveDetailLayoutProperties", "AdaptiveDetail Layout Items", typeof(FormLayoutItemsEditorFrame), Control, ItemsImageIndex, owner));
		}
		void CreateSummaryGroupItems() {
			Groups.Add(SummaryGroupCaption, SummaryGroupCaption, GetDefaultGroupImage(SummaryGroupImageIndex), false);
			SummaryGroup.Add(CreateDesignerItem(new SummaryItemsOwner(GridView, Provider, "Total Summary", GridView.TotalSummary), typeof(ItemsEditorFrame), TotalSummaryItemImageIndex));
			SummaryGroup.Add(CreateDesignerItem(new SummaryItemsOwner(GridView, Provider, "Group Summary", GridView.GroupSummary), typeof(ItemsEditorFrame), GroupSummaryItemImageIndex));
		}
	}
	public class ColumnsPropertiesCommonFormDesigner : CommonFormDesigner {
		EditPropertiesBase editProperties;
		public ColumnsPropertiesCommonFormDesigner(object component, IServiceProvider provider, EditPropertiesBase properties)
			: base((ASPxWebControl)component, provider) {
			editProperties = properties;
		}
		protected override void CreateMainGroupItems() {
			AddItemsItem();
			AddButtonsItem();
			AddColumnsItem();
		}
		protected void AddItemsItem() {
			if(editProperties is AutoCompleteBoxPropertiesBase)
				MainGroup.Add(CreateDesignerItem(new ListEditItemsOwner(Control, Provider, ((AutoCompleteBoxPropertiesBase)editProperties).Items), typeof(ItemsEditorFrame), ItemsItemImageIndex));
		}
		protected void AddButtonsItem() {
			if(editProperties is ButtonEditPropertiesBase)
				MainGroup.Add(CreateDesignerItem(new ButtonEditButtonsOwner(Control, Provider, ((ButtonEditPropertiesBase)editProperties).Buttons), typeof(ItemsEditorFrame), ButtonsItemImageIndex));
		}
		protected void AddColumnsItem() {
			if(editProperties is ComboBoxProperties)
				MainGroup.Add(CreateDesignerItem(new ListBoxColumnsOwner(Control, Provider, ((ComboBoxProperties)editProperties).Columns), typeof(ItemsEditorFrame), ColumnsItemImageIndex));
		}
	}
	public class GridViewColumnEditPropertiesEditor : ObjectSelectorEditor {
		public GridViewColumnEditPropertiesEditor() : base(true) { }
		List<string> GetEditors() {
			List<string> res = new List<string>(EditRegistrationInfo.Editors.Keys);
			res.Sort();
			res.Insert(0, GridViewColumnEditPropertiesConverter.DefaultName);
			return res;
		}
		protected override void FillTreeWithData(ObjectSelectorEditor.Selector selector, ITypeDescriptorContext context, IServiceProvider provider) {
			selector.ShowLines = false;
			selector.ShowRootLines = false;
			selector.ShowPlusMinus = false;
			selector.Width = 100;
			selector.Height = 80;
			selector.FullRowSelect = true;
			selector.Clear();
			EditPropertiesBase value = GetCurrentValue(context);
			List<string> list = GetEditors();
			foreach(string s in list) {
				selector.AddNode(s, s, null);
			}
			string valueText = value == null ? GridViewColumnEditPropertiesConverter.DefaultName : EditRegistrationInfo.GetEditName(value);
			foreach(System.Windows.Forms.TreeNode node in selector.Nodes) {
				if(node.Text == valueText) {
					selector.SelectedNode = node;
					break;
				}
			}
			selector.Select();
		}
		EditPropertiesBase GetCurrentValue(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			return context.PropertyDescriptor.GetValue(context.Instance) as EditPropertiesBase;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null) return null;
			value = base.EditValue(context, provider, value);
			if(value is string) {
				value = EditRegistrationInfo.CreateProperties(value.ToString());
			}
			return value;
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
		}
	}
	public class GridFormatRuleExpressionEditorForm: ExpressionEditorFormEx {
		public GridFormatRuleExpressionEditorForm(object contextInstance, IDesignerHost designerHost,  object value)
			: base(contextInstance, designerHost) {
				if(value != null) {
					OldExpression = value.ToString();
					InitializeEditorLogic();
				}
		}
		protected string OldExpression { get; private set; }
		protected override DevExpress.Data.ExpressionEditor.ExpressionEditorLogic CreateExpressionEditorLogic() {
			var condition = ContextInstance as GridFormatConditionBase;
			var grid = condition.Collection.Owner as ASPxGridBase;
			return new GridExpressionEditorLogic(this, CreateDataColumnInfoByCondition(), OldExpression);
		}
		IDataColumnInfo CreateDataColumnInfoByCondition() {
			var condition = ContextInstance as GridFormatConditionBase;
			if(condition == null || condition.Collection == null || condition.Collection.Owner == null)
				return null;
			var grid = condition.Collection.Owner as ASPxGridBase;
			var column = grid.Columns[condition.FieldName] as IWebGridDataColumn;
			return column != null ? new DataColumnInfoWrapper(column) : null;
		}
		class GridExpressionEditorLogic: DevExpress.Data.ExpressionEditor.ExpressionEditorLogicEx {
			internal GridExpressionEditorLogic(DevExpress.Data.ExpressionEditor.IExpressionEditor editor, IDataColumnInfo columnInfo, string expression)
				: base(editor, columnInfo) {
				Expression = expression;
			}
			protected string Expression {get; private set; }
			protected override string GetExpressionMemoEditText() { return Expression; }
		}
	}
	public class DataColumnInfoWrapper : IDataColumnInfo {
		IWebGridDataColumn dataColumn;
		public DataColumnInfoWrapper(IWebGridDataColumn column) {
			dataColumn = column;
		}
		protected ASPxGridBase Grid { get { return dataColumn.Adapter.Grid; } }
		DataControllerBase IDataColumnInfo.Controller { get { return null; } }
		string IDataColumnInfo.Caption { get { return dataColumn.FieldName; } }
		List<IDataColumnInfo> IDataColumnInfo.Columns {
			get {
				var dataColumns = Grid.Columns.OfType<IWebGridDataColumn>().Where(i => i != this.dataColumn).ToList();
				return dataColumns.Select(i => new DataColumnInfoWrapper(i)).OfType<IDataColumnInfo>().ToList();
			}
		}
		string IDataColumnInfo.FieldName { get { return dataColumn.FieldName; } }
		Type IDataColumnInfo.FieldType { get { return dataColumn.Adapter.DataType; } }
		string IDataColumnInfo.Name { get { return dataColumn.Name; } }
		string IDataColumnInfo.UnboundExpression { get { return dataColumn.UnboundExpression; } }
	}
}
