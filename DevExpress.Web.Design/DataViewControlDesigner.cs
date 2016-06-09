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
using System.Reflection;
using System.Text;
using System.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Design.Forms;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public abstract class ASPxDataViewControlDesignerBase : ASPxDataWebControlDesigner {
		protected ASPxDataViewBase fDataViewControl = null;
		public virtual ASPxDataViewBase DataView {
			get { return fDataViewControl; }
		}
		protected override int SampleRowCount {
			get { return 10; }
		}
		protected virtual string[] ControlTemplateNames {
			get { return new string[0]; }
		}
		public override void Initialize(IComponent component) {
			fDataViewControl = (ASPxDataViewBase)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override string GetBaseProperty() {
			return "DataSourceID";
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			for(int i = 0; i < ControlTemplateNames.Length; i++) {
				TemplateGroup templateGroup = new TemplateGroup(ControlTemplateNames[i]);
				TemplateDefinition templateDefinition = new TemplateDefinition(this, ControlTemplateNames[i],
					Component, ControlTemplateNames[i], GetTemplateStyle(ControlTemplateNames[i]));
				templateDefinition.SupportsDataBinding = true;
				templateGroup.AddTemplateDefinition(templateDefinition);
				templateGroups.Add(templateGroup);
			}
			return templateGroups;
		}
		protected Style GetTemplateStyle(string templateName) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(DataView.GetControlStyle());
			return style;
		}
		public Layout Layout {
			get { return (DataView as ASPxDataViewBase).LayoutInternal; }
			set {
				(DataView as ASPxDataViewBase).LayoutInternal = value;
				PropertyChanged("LayoutInternal");
				UpdateDesignTimeHtml();
			}
		}
		public int ItemsPerPage {
			get { return (DataView as ASPxDataViewBase).SettingsFlowLayoutInternal.ItemsPerPage; }
			set {
				(DataView as ASPxDataViewBase).SettingsFlowLayoutInternal.ItemsPerPage = value;
				PropertyChanged("SettingsFlowLayoutInternal.ItemsPerPage");
			}
		}
		public int RowsPerPage {
			get { return (DataView as ASPxDataViewBase).SettingsTableLayoutInternal.RowsPerPage; }
			set {
				(DataView as ASPxDataViewBase).SettingsTableLayoutInternal.RowsPerPage = value;
				PropertyChanged("SettingsTableLayoutInternal.RowsPerPage");
			}
		}
		public int ColumnCount {
			get { return (DataView as ASPxDataViewBase).SettingsTableLayoutInternal.ColumnCount; }
			set {
				(DataView as ASPxDataViewBase).SettingsTableLayoutInternal.ColumnCount = value;
				PropertyChanged("SettingsTableLayoutInternal.ColumnCount");
			}
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ASPxDataViewBaseDesignerActionList(this);
		}
	}
	public class ASPxDataViewBaseDesignerActionList : ASPxWebControlDesignerActionList {
		protected ASPxDataViewControlDesignerBase DataViewDesignerBase { get { return Designer as ASPxDataViewControlDesignerBase; } }
		public ASPxDataViewBaseDesignerActionList(ASPxDataViewControlDesignerBase designer)
			: base(designer) {
		}
		public Layout Layout {
			get { return DataViewDesignerBase.Layout; }
			set { DataViewDesignerBase.Layout = value; }
		}
		public int ItemsPerPage {
			get { return DataViewDesignerBase.ItemsPerPage; }
			set { DataViewDesignerBase.ItemsPerPage = value; }
		}
		public int RowsPerPage {
			get { return DataViewDesignerBase.RowsPerPage; }
			set { DataViewDesignerBase.RowsPerPage = value; }
		}
		public int ColumnCount {
			get { return DataViewDesignerBase.ColumnCount; }
			set { DataViewDesignerBase.ColumnCount = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("Layout", "Layout", "Layout"));
			if(Layout == Layout.Flow)
				collection.Add(new DesignerActionPropertyItem("ItemsPerPage", "Items Per Page", "FlowLayoutSettings", "The number of items that a page displays in flow mode"));
			else {
				collection.Add(new DesignerActionPropertyItem("RowsPerPage", "Rows Per Page", "TableLayoutSettings", "The number of rows that a page displays in table mode"));
				collection.Add(new DesignerActionPropertyItem("ColumnCount", "Column Count", "TableLayoutSettings", "The number of columns that a page displays in table mode"));
			}
			return collection;
		}
	}
	public class ASPxDataViewControlDesigner : ASPxDataViewControlDesignerBase {
		private string[] fControlTemplateNames = new string[] {
			"ItemTemplate",
			"EmptyItemTemplate",
			"PagerPanelLeftTemplate",
			"PagerPanelRightTemplate",
			"EmptyDataTemplate"
		};
		protected override string[] ControlTemplateNames {
			get { return fControlTemplateNames; }
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxDataView dataViewControl = dataControl as ASPxDataView;
			if (!string.IsNullOrEmpty(dataViewControl.DataSourceID) || (dataViewControl.DataSource != null) ||
				!dataViewControl.HasVisibleItems()) {
				dataViewControl.DataItems.Clear();
				base.DataBind(dataViewControl);
			}
		}
		protected IDataSourceViewSchema GetDataSourceSchema() {
			return (DesignerView != null) ? DesignerView.Schema : null;
		}
		protected override void OnSchemaRefreshed() {
			ControlDesigner.InvokeTransactedChange(Component, new TransactedChangeCallback(this.RefreshSchemaCallback), null, "DataList_RefreshSchemaTransaction");
		}
		protected void CreateDefaultTemplate() {
			StringBuilder sb = new StringBuilder();
			ASPxDataView dataViewControl = (ASPxDataView)Component;
			IDataSourceViewSchema schema = GetDataSourceSchema();
			IDataSourceFieldSchema[] fieldSchemasArray = null;
			if (schema != null)
				fieldSchemasArray = schema.GetFields();
			if (fieldSchemasArray != null && fieldSchemasArray.Length > 0) {
				foreach (IDataSourceFieldSchema fieldSchema in fieldSchemasArray) {
					string fieldSchemaName = fieldSchema.Name;
					char[] fieldSchemaNameChars = new char[fieldSchemaName.Length];
					for (int i = 0; i < fieldSchemaName.Length; i++) {
						char ch = fieldSchemaName[i];
						if (char.IsLetterOrDigit(ch) || (ch == '_'))
							fieldSchemaNameChars[i] = ch;
						else
							fieldSchemaNameChars[i] = '_';
					}
					string updatedFieldSchemaName = new string(fieldSchemaNameChars);
					sb.AppendLine(string.Format("<b>{0}</b>: <asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label\"/><br/>", 
						fieldSchemaName, CreateEvalExpression(fieldSchemaName, ""), updatedFieldSchemaName));
				}
				sb.Append(Environment.NewLine);
				try {
					dataViewControl.ItemTemplate = GetTemplateFromText(sb.ToString(), dataViewControl.ItemTemplate);
				}
				catch { 
				}
			}
		}
		public string CreateEvalExpression(string field, string format) {
			string preparedExpression = field;
			bool isExpressionPrepared = false;
			for (int i = 0; i < field.Length; i++) {
				char ch = field[i];
				if ((!char.IsLetterOrDigit(ch) && (ch != '_')) && !isExpressionPrepared) {
					preparedExpression = "[" + field + "]";
					isExpressionPrepared = true;
				}
			}
			if ((format != null) && (format.Length != 0))
				return string.Format("Eval(\"{0}\", \"{1}\")", preparedExpression, format);
			return string.Format("Eval(\"{0}\")", preparedExpression);
		}
		protected internal ITemplate GetTemplateFromText(string contentText, ITemplate currentTemplate) {
			if (string.IsNullOrEmpty(contentText))
				throw new ArgumentNullException("ContentText");
			IDesignerHost designerHost = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
			try {
				ITemplate generatedTemplate = ControlParser.ParseTemplate(designerHost, contentText);
				if (generatedTemplate != null)
					return generatedTemplate;
			}
			catch { 
			}
			return currentTemplate;
		}
		protected virtual bool RefreshSchemaCallback(object context) {
			ASPxDataView dataView = (ASPxDataView)Component;
			bool noTemplate = (dataView.ItemTemplate == null);
			IDataSourceViewSchema dataSourceViewSchema = this.GetDataSourceSchema();
			if (!string.IsNullOrEmpty(DataSourceID) && dataSourceViewSchema != null) {
				if (noTemplate || (!noTemplate && MessageBoxEx.Show(null, StringResources.DataViewItemTemplateRegenerate,
					StringResources.DataViewItemTemplateResetCaption, MessageBoxButtonsEx.YesNo) == DialogResultEx.Yes)) {
					dataView.ItemTemplate = null;
					CreateDefaultTemplate();
					UpdateDesignTimeHtml();
				}
			}
			else {
				if (noTemplate || (!noTemplate && MessageBoxEx.Show(null, StringResources.DataViewItemTemplateClear,
					StringResources.DataViewItemTemplateResetCaption, MessageBoxButtonsEx.YesNo) == DialogResultEx.Yes)) {
					dataView.ItemTemplate = null;
					UpdateDesignTimeHtml();
				}
			}
			return true;
		}
	}
}
