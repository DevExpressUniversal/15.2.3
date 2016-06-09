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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views.WinForms.Standart
{
	using System.Linq;
	using System.Text;
	using System.Collections.Generic;
	using DevExpress.Design.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
	using DevExpress.Xpf.Core.Native;
	using DevExpress.Mvvm.UI.Native.ViewGenerator;
	using DevExpress.Entity.Model;
	using DevExpress.Mvvm.Native;
	using System;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class ElementView_TabbedMDIDesigner : ElementView_TabbedMDIDesignerBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	EntityViewModelData viewModelData = templateInfo.Properties["IViewModelInfo"] as EntityViewModelData;
	UIType uiType = (UIType)templateInfo.Properties["UIType"];
	string viewName = templateInfo.Properties["ViewName"].ToString();	
	string localNamespace = templateInfo.Properties["Namespace"].ToString();
	string viewFullName = localNamespace +"." + viewName;
	string mvvmContextFullName = viewModelData.Namespace+"."+viewModelData.Name;
	string bindingSourceName = Char.ToLowerInvariant(viewName[0]) + viewName.Substring(1) + "BindingSource";
	 List<PropertyEditorInfo> listLookUpInfo = templateInfo.Properties["GeneratedLookups"] as List<PropertyEditorInfo>;
	bool IsVisualBasic = (bool)templateInfo.Properties["IsVisualBasic"];
if(!IsVisualBasic){
			this.Write("namespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write(" {\r\n    partial class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(" {\r\n        /// <summary> \r\n        /// Required designer variable.\r\n        /// " +
					"</summary>\r\n        private System.ComponentModel.IContainer components = null;\r" +
					"\n\r\n        /// <summary> \r\n        /// Clean up any resources being used.\r\n     " +
					"   /// </summary>\r\n        /// <param name=\"disposing\">true if managed resources" +
					" should be disposed; otherwise, false.</param>\r\n        protected override void " +
					"Dispose(bool disposing) {\r\n            if(disposing && (components != null)) {\r\n" +
					"                components.Dispose();\r\n            }\r\n            base.Dispose(d" +
					"isposing);\r\n        }\r\n\t\t #region Component Designer generated code\r\n\r\n        /" +
					"// <summary> \r\n        /// Required method for Designer support - do not modify " +
					"\r\n        /// the contents of this method with the code editor.\r\n        /// </s" +
					"ummary>\r\n\t\tprivate void InitializeComponent() {\r\n\t\t\tthis.components = new System" +
					".ComponentModel.Container();\r\n\t\t\tthis.dataLayoutControl1 = new DevExpress.XtraDa" +
					"taLayout.DataLayoutControl();\r\n\t\t\tthis.layoutControlGroup1 = new DevExpress.Xtra" +
					"Layout.LayoutControlGroup();\r\n\t\t\tthis.mvvmContext = new DevExpress.Utils.MVVM.MV" +
					"VMContext(this.components);\r\n\t\t\tthis.mainRibbonControl = new DevExpress.XtraBars" +
					".Ribbon.RibbonControl();\r\n\t\t\tthis.mainRibbonPage = new DevExpress.XtraBars.Ribbo" +
					"n.RibbonPage();\r\n            this.mainRibbonPageGroup = new DevExpress.XtraBars." +
					"Ribbon.RibbonPageGroup();\r\n\t\t\tthis.ribbonPageGroupLayout = new DevExpress.XtraBa" +
					"rs.Ribbon.RibbonPageGroup();\r\n            this.bbiCustomize = new DevExpress.Xtr" +
					"aBars.BarButtonItem();\r\n\t\t\t");
foreach(var lookUpTable in viewModelData.LookUpTables){
				string nameForGridControl = lookUpTable.LookUpCollectionPropertyAssociationName +"GridControl";
				string nameForGridView = lookUpTable.LookUpCollectionPropertyAssociationName +"GridView";
				string nameForBarManager = lookUpTable.LookUpCollectionPropertyAssociationName +"BarManager";
				string nameForBar = lookUpTable.LookUpCollectionPropertyAssociationName +"Bar";
				string nameForUserControl = lookUpTable.LookUpCollectionPropertyAssociationName +"XtraUserControl";
				string nameForPopUpMenu = lookUpTable.LookUpCollectionPropertyAssociationName +"PopUpMenu";
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(" = new DevExpress.XtraGrid.GridControl();\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(" = new DevExpress.XtraGrid.Views.Grid.GridView();\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(" = new DevExpress.XtraBars.BarManager();\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(" = new DevExpress.XtraBars.Bar();\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(" = new DevExpress.XtraEditors.XtraUserControl();\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(" = new DevExpress.XtraBars.PopupMenu(this.components);\r\n\t\t\t");
foreach(var commandsLookUpTable in lookUpTable.Commands){
				string nameForBarItemInLookUpTable = lookUpTable.LookUpCollectionPropertyAssociationName + commandsLookUpTable.CommandPropertyName.Remove(commandsLookUpTable.CommandPropertyName.Length -7,7);
			this.Write("\t\t\tthis.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(" = new DevExpress.XtraBars.BarButtonItem();\r\n\t\t\t");
}
			this.Write("\t\t\t((System.ComponentModel.ISupportInitialize)(this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(")).BeginInit();\r\n\t\t\t");
}
			this.Write("\t\t\t");
			foreach(var realLookUpInfo in listLookUpInfo){
			string nameForLookUp = realLookUpInfo.Property.Name+"LookUpEdit";
			string nameForLookUpBindingSource = realLookUpInfo.Property.Name+ "BindingSource";
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(" = new DevExpress.XtraEditors.GridLookUpEdit();\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUpBindingSource));
			this.Write(" = new System.Windows.Forms.BindingSource(this.components);\r\n\t\t\t");
}
			this.Write(@" 
			this.SuspendLayout();
			// 
            // mainRibbonControl
            // 
            this.mainRibbonControl.ExpandCollapseItem.Id = 0;
			this.mainRibbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] {this.bbiCustomize});
            this.mainRibbonControl.Name = ""mainRibbonControl"";
			this.mainRibbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {this.mainRibbonPage});
            this.mainRibbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013;
            this.mainRibbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            this.mainRibbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			");
foreach(var mainCommand in viewModelData.Commands){
				string nameForBarItemMain = mainCommand.CommandPropertyName.Remove(mainCommand.CommandPropertyName.Length -7,7);
				string imageUri = nameForBarItemMain;
				if(nameForBarItemMain.Contains("Layout")){
					imageUri = nameForBarItemMain.Replace("Layout", string.Empty);
				} 
			this.Write("\t\t\tDevExpress.XtraBars.BarButtonItem bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(" = new DevExpress.XtraBars.BarButtonItem();\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(".Caption = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(mainCommand.Caption));
			this.Write("\";\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(".Name = \"bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write("\";\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(".ImageUri.Uri = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(imageUri));
			this.Write("\";\r\n\t\t\tthis.mainRibbonControl.Items.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(");\r\n\t\t\t");
}
			this.Write(@"            // 
            // mainRibbonPage
            // 
            this.mainRibbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.mainRibbonPageGroup,
            this.ribbonPageGroupLayout});
            this.mainRibbonPage.Name = ""mainRibbonPage"";
            this.mainRibbonPage.Text = ""Home"";
			this.mainRibbonPage.MergeOrder = 0;
            // 
            // mainRibbonPageGroup
            // 
            this.mainRibbonPageGroup.AllowTextClipping = false;
			");
foreach(var mainCommand in viewModelData.Commands){
				string nameForBarItemMain = mainCommand.CommandPropertyName.Remove(mainCommand.CommandPropertyName.Length -7,7);
				if(!nameForBarItemMain.Contains("Layout")){
			this.Write("\t\t\tthis.mainRibbonPageGroup.ItemLinks.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(");\r\n\t\t\t");
}}
			this.Write("            this.mainRibbonPageGroup.Name = \"mainRibbonPageGroup\";\r\n            t" +
					"his.mainRibbonPageGroup.ShowCaptionButton = false;\r\n            this.mainRibbonP" +
					"ageGroup.Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeName));
			this.Write(" Tasks\";\r\n\t\t\t// \r\n            // ribbonPageGroupLayout\r\n            //\r\n\t\t\tthis.r" +
					"ibbonPageGroupLayout.AllowTextClipping = false; \r\n\t\t\tthis.ribbonPageGroupLayout." +
					"ItemLinks.Add(this.bbiCustomize);\r\n\t\t\t");
foreach(var mainCommand in viewModelData.Commands){
				string nameForBarItemMain = mainCommand.CommandPropertyName.Remove(mainCommand.CommandPropertyName.Length -7,7);
				if(nameForBarItemMain.Contains("Layout")){
			this.Write("\t\t\tthis.ribbonPageGroupLayout.ItemLinks.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(");\r\n\t\t\t");
}}
			this.Write(@"            this.ribbonPageGroupLayout.Name = ""ribbonPageGroupLayout"";
            this.ribbonPageGroupLayout.Text = ""Layout"";
			// 
            // bbiCustomize
            // 
            this.bbiCustomize.Caption = ""Customize"";
            this.bbiCustomize.ImageUri.Uri = ""Customization"";
            this.bbiCustomize.Name = ""bbiCustomize"";
			// 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Name = ""Root"";
            this.layoutControlGroup1.TextVisible = false;
			// 
            // dataLayoutControl1
            // 
            this.dataLayoutControl1.AllowCustomization = false;
			this.dataLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataLayoutControl1.Root = this.layoutControlGroup1;
			this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(" = new System.Windows.Forms.BindingSource(this.components);\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(".DataSource = typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeFullName));
			this.Write(");\r\n\t\t\tthis.dataLayoutControl1.DataSource = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(@";
			//
			//Create GridControls
			//
			DevExpress.XtraDataLayout.RetrieveFieldsParameters parameters = new DevExpress.XtraDataLayout.RetrieveFieldsParameters();
           	parameters.DataSourceUpdateMode = System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged;
			");
			foreach(var lookUpTable in viewModelData.LookUpTables){
				string nameForGridControl = lookUpTable.LookUpCollectionPropertyAssociationName +"GridControl";
				string nameForGridView = lookUpTable.LookUpCollectionPropertyAssociationName +"GridView";
				string nameForBindingSource = lookUpTable.LookUpCollectionPropertyAssociationName +"BindingSource";
				string nameForBarManager = lookUpTable.LookUpCollectionPropertyAssociationName +"BarManager";
				string nameForBar = lookUpTable.LookUpCollectionPropertyAssociationName +"Bar";
				string nameForUserControl = lookUpTable.LookUpCollectionPropertyAssociationName +"XtraUserControl";
				string nameForPopUpMenu = lookUpTable.LookUpCollectionPropertyAssociationName +"PopUpMenu";
				string nameForParemeters = lookUpTable.LookUpCollectionPropertyAssociationName +"PopulateColumnsParameters";
				IEnumerable<IEdmPropertyInfo> scaffoldingProperties = lookUpTable.GetScaffoldingProperties();
				List<IEdmPropertyInfo>  navigationProperty = new List<IEdmPropertyInfo>();
				System.Reflection.PropertyInfo[] typeProperties = lookUpTable.EntityType.GetProperties();
				List<string> NonVisibleTypes = typeProperties.Where(e => !scaffoldingProperties.Any(q => q.Name == e.Name)).Select(e => e.Name).ToList();
				foreach(IEdmPropertyInfo property in scaffoldingProperties){
						if(property.IsForeignKey) NonVisibleTypes.Add(property.Name);
						if(property.IsNavigationProperty){
							if(property.PropertyType == viewModelData.EntityType) NonVisibleTypes.Add(property.Name);
							else navigationProperty.Add(property);
						}
				}
			this.Write("\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write("\r\n\t\t\t//\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".MainView = this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(";\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".Dock = System.Windows.Forms.DockStyle.Fill;\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".MenuManager = this.mainRibbonControl;\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write("\";\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {\r\n       " +
					"     this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write("});\r\n\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write("\r\n\t\t\t//\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".GridControl = this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(";\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write("\";\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".OptionsBehavior.Editable = false;\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".OptionsBehavior.ReadOnly = true;\r\n\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForParemeters));
			this.Write("\r\n\t\t\t//\r\n\t\t\tDevExpress.XtraGrid.Extensions.PopulateColumnsParameters ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForParemeters));
			this.Write(" = new DevExpress.XtraGrid.Extensions.PopulateColumnsParameters();\r\n\t\t\t");
foreach(var noVisibleColumn in NonVisibleTypes){
			 string nameForNonVisibleColumn = noVisibleColumn + lookUpTable.LookUpCollectionPropertyAssociationName;
			this.Write("\t\r\n\t\t\tDevExpress.XtraGrid.Extensions.PopulateColumnParameters ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForNonVisibleColumn));
			this.Write("ChildPopulateColumnParameters_NotVisible = new DevExpress.XtraGrid.Extensions.Pop" +
					"ulateColumnParameters();\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForNonVisibleColumn));
			this.Write("ChildPopulateColumnParameters_NotVisible.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("\";\r\n\t\t    ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForNonVisibleColumn));
			this.Write("ChildPopulateColumnParameters_NotVisible.ColumnVisible = false;\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForParemeters));
			this.Write(".CustomColumnParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForNonVisibleColumn));
			this.Write("ChildPopulateColumnParameters_NotVisible);\r\n\t\t\t");
}
			this.Write(" \r\n\t\t\t");
foreach(var navProperty in navigationProperty){
				string displayMember = EditorsSource.GetDisplayMemberPropertyName(navProperty.PropertyType);
				string BindingPath = navProperty.Name + ((string.IsNullOrEmpty(displayMember) ? null : "." + displayMember));
				string name = navProperty.Name + lookUpTable.LookUpCollectionPropertyAssociationName;
			this.Write("\t\r\n\t\t\tDevExpress.XtraGrid.Extensions.PopulateColumnParameters ");
			this.Write(this.ToStringHelper.ToStringWithCulture(name));
			this.Write("ChildPopulateColumnParameters = new DevExpress.XtraGrid.Extensions.PopulateColumn" +
					"Parameters();\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(name));
			this.Write("ChildPopulateColumnParameters.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(name));
			this.Write("\";\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(name));
			this.Write("ChildPopulateColumnParameters.Path = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(BindingPath));
			this.Write("\";\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForParemeters));
			this.Write(".CustomColumnParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(name));
			this.Write("ChildPopulateColumnParameters);\r\n\t\t\t");
}
			this.Write(" \r\n\t\t    this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".PopulateColumns(typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.EntityTypeFullName));
			this.Write("),");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForParemeters));
			this.Write(");\r\n\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBindingSource));
			this.Write("\r\n\t\t\t//\r\n\t\t\tSystem.Windows.Forms.BindingSource ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBindingSource));
			this.Write(" = new System.Windows.Forms.BindingSource(this.components);\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBindingSource));
			this.Write(".DataSource = typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.EntityTypeFullName));
			this.Write(");\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".DataSource = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBindingSource));
			this.Write(";\r\n\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write("\r\n\t\t\t//\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(".Controls.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(");\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write("\";\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(".MinimumSize = new System.Drawing.Size(100, 100); \r\n\t\t\t\t");
foreach(var commandLookUpTable in lookUpTable.Commands){
						string nameForBarItemInLookUpTable = lookUpTable.LookUpCollectionPropertyAssociationName + commandLookUpTable.CommandPropertyName.Remove(commandLookUpTable.CommandPropertyName.Length -7,7);
						string imageUri = commandLookUpTable.CommandPropertyName.Remove(commandLookUpTable.CommandPropertyName.Length -7,7);
			this.Write("\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write("\r\n\t\t\t//\r\n\t\t\tthis.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(".Caption = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(commandLookUpTable.Caption));
			this.Write("\";\r\n\t\t\tthis.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(".Name = \"bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write("\";\r\n\t\t\tthis.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(".ImageUri.Uri = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(imageUri));
			this.Write("\";\r\n\t\t\tthis.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(".PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(".Items.Add(this.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(");\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(this.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write("));\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(".LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(this.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write("));\r\n\t\t\t");
}
			this.Write("\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write("\r\n\t\t\t//\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".BarName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("\";\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".DockCol = 0;\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".DockRow = 0;\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".DockStyle = DevExpress.XtraBars.BarDockStyle.Top;\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".OptionsBar.AllowQuickCustomization = false;\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".OptionsBar.DrawDragBorder = false;\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("\";\r\n\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write("\r\n\t\t\t//\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(".AllowCustomization = false;\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(".Bars.AddRange(new DevExpress.XtraBars.Bar[] {this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write("});\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(".Form = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(";\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(".MainMenu = this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(";\r\n\t\t\t// \r\n            // ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write("\r\n            // \r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(".Manager = this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(";\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write("\";\r\n\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters\r\n\t\t\t//\r\n\t\t\tDevExpress.XtraDataLayout.RetrieveFieldParamete" +
					"rs ");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters = new DevExpress.XtraDataLayout.RetrieveFieldParameters();" +
					"\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("\";\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters.ControlForField = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(";\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters.CreateTabGroupForItem = true;\r\n\t\t\tparameters.CustomListPar" +
					"ameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters);\r\n\t\t\t");
}
			this.Write("\t\t\t");
			foreach(var realLookUpInfo in listLookUpInfo){
			string nameForLookUp = realLookUpInfo.Property.Name+"LookUpEdit";
			string nameForLookUpBindingSource = realLookUpInfo.Property.Name+ "BindingSource";
			this.Write("\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("\r\n\t\t\t//\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUpBindingSource));
			this.Write(".DataSource = typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Property.PropertyType.FullName));
			this.Write(");\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".Properties.ValueMember = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.ForeignKeyInfo.PrimaryKeyPropertyName));
			this.Write("\"; \r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".Properties.DisplayMember = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.DisplayMemberPropertyName));
			this.Write("\";\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".Properties.DataSource = this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUpBindingSource));
			this.Write(";\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("\";\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".DataBindings.Add(\"EditValue\", ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(", \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.ForeignKeyInfo.ForeignKeyPropertyName));
			this.Write("\", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);\r\n\t\t\tDevExp" +
					"ress.XtraDataLayout.RetrieveFieldParameters ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters = new DevExpress.XtraDataLayout.RetrieveFieldParameters()" +
					";\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.ForeignKeyInfo.ForeignKeyPropertyName));
			this.Write("\";\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters.ControlForField = this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(";\r\n\t\t\tparameters.CustomListParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters);\r\n\t\t\tDevExpress.XtraDataLayout.RetrieveFieldParameters ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters_NotGenerate = new DevExpress.XtraDataLayout.RetrieveField" +
					"Parameters();\r\n\t\t    ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters_NotGenerate.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Property.Name));
			this.Write("\";\r\n\t\t    ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters_NotGenerate.GenerateField = false;\r\n\t\t\tparameters.CustomL" +
					"istParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters_NotGenerate);\r\n\t\t\t");
}
			this.Write(@" 
			//
			//call RetrieveFields
			//
            this.dataLayoutControl1.RetrieveFields(parameters);
			// 
            // mvvmContext
            // 
            this.mvvmContext.ContainerControl = this;
            this.mvvmContext.ViewModelType = typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(");\r\n\t\t\t");
foreach(var item in viewModelData.Commands){
				string nameForBarItemMain = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			this.Write("\t\t\tthis.mvvmContext.BindingExpressions.Add(DevExpress.Utils.MVVM.BindingExpressio" +
					"n.CreateCommandBinding(typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write("), \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write("\", bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write("));\r\n\t\t\t");
}
			this.Write(@"		    this.mvvmContext.RegistrationExpressions.AddRange(new DevExpress.Utils.MVVM.RegistrationExpression[] {
            DevExpress.Utils.MVVM.RegistrationExpression.RegisterLayoutSerializationService(null, false, DevExpress.Utils.DefaultBoolean.True, this.dataLayoutControl1)});
			//
			//");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.Controls.Add(this.dataLayoutControl1);
			this.Controls.Add(this.mainRibbonControl);
			this.Size = new System.Drawing.Size(1024, 768);
            this.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("\";\r\n\t\t\t");
foreach(var lookUpTable in viewModelData.LookUpTables){
				string nameForBarManager = lookUpTable.LookUpCollectionPropertyAssociationName +"BarManager";
			this.Write("\t\t\t((System.ComponentModel.ISupportInitialize)(this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(")).EndInit();\r\n\t\t\t");
}
			this.Write(@"			this.ResumeLayout(false);
		}
		
        #endregion

		private DevExpress.XtraDataLayout.DataLayoutControl dataLayoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.Utils.MVVM.MVVMContext mvvmContext;
		private DevExpress.XtraBars.Ribbon.RibbonControl mainRibbonControl;
		private DevExpress.XtraBars.Ribbon.RibbonPage mainRibbonPage;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup mainRibbonPageGroup;
		private DevExpress.XtraBars.BarButtonItem bbiCustomize;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroupLayout;
		private System.Windows.Forms.BindingSource ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(";\r\n\t\t");
foreach(var lookUpTable in viewModelData.LookUpTables){
				string nameForGridControl = lookUpTable.LookUpCollectionPropertyAssociationName +"GridControl";
				string nameForGridView = lookUpTable.LookUpCollectionPropertyAssociationName +"GridView";
				string nameForBarManager = lookUpTable.LookUpCollectionPropertyAssociationName +"BarManager";
				string nameForBar = lookUpTable.LookUpCollectionPropertyAssociationName +"Bar";
				string nameForUserControl = lookUpTable.LookUpCollectionPropertyAssociationName +"XtraUserControl";
				string nameForPopUpMenu = lookUpTable.LookUpCollectionPropertyAssociationName +"PopUpMenu";
			this.Write("\t\tprivate DevExpress.XtraGrid.GridControl ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(";\r\n\t\tprivate DevExpress.XtraGrid.Views.Grid.GridView ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(";\r\n\t\tprivate DevExpress.XtraBars.BarManager ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(";\r\n\t\tprivate DevExpress.XtraBars.Bar ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(";\r\n\t\tprivate DevExpress.XtraEditors.XtraUserControl ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(";\r\n\t\tprivate DevExpress.XtraBars.PopupMenu ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(";\r\n\t\t");
foreach(var commandLookUpTable in lookUpTable.Commands){
		string nameForBarItemInLookUpTable = lookUpTable.LookUpCollectionPropertyAssociationName + commandLookUpTable.CommandPropertyName.Remove(commandLookUpTable.CommandPropertyName.Length -7,7);
			this.Write("\t\tprivate DevExpress.XtraBars.BarButtonItem bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(";\r\n\t\t");
}
			this.Write("\t\t");
}
			this.Write("\t\t");
		foreach(var realLookUpInfo in listLookUpInfo){
		string nameForLookUp = realLookUpInfo.Property.Name+"LookUpEdit";
		string nameForLookUpBindingSource = realLookUpInfo.Property.Name+ "BindingSource";
			this.Write("\t\tprivate DevExpress.XtraEditors.GridLookUpEdit ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(";\r\n\t\tprivate System.Windows.Forms.BindingSource ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUpBindingSource));
			this.Write(";\r\n\t\t");
}
			this.Write(" \r\n\t}\r\n}\r\n");
}
if(IsVisualBasic){
			this.Write("Namespace Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("\r\n\tPartial Public Class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
		''' <summary> 
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		''' <param name=""disposing"">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub
		#Region ""Component Designer generated code""

		''' <summary> 
		''' Required method for Designer support - do not modify 
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container()
			Me.dataLayoutControl1 = New DevExpress.XtraDataLayout.DataLayoutControl()
			Me.layoutControlGroup1 = New DevExpress.XtraLayout.LayoutControlGroup()
			Me.mvvmContext = New DevExpress.Utils.MVVM.MVVMContext(Me.components)
			Me.mainRibbonControl = New DevExpress.XtraBars.Ribbon.RibbonControl()
			Me.mainRibbonPage = New DevExpress.XtraBars.Ribbon.RibbonPage()
			Me.mainRibbonPageGroup = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
			Me.ribbonPageGroupLayout = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
			Me.bbiCustomize = New DevExpress.XtraBars.BarButtonItem()
			");
foreach(var lookUpTable in viewModelData.LookUpTables){
				string nameForGridControl = lookUpTable.LookUpCollectionPropertyAssociationName +"GridControl";
				string nameForGridView = lookUpTable.LookUpCollectionPropertyAssociationName +"GridView";
				string nameForBarManager = lookUpTable.LookUpCollectionPropertyAssociationName +"BarManager";
				string nameForBar = lookUpTable.LookUpCollectionPropertyAssociationName +"Bar";
				string nameForUserControl = lookUpTable.LookUpCollectionPropertyAssociationName +"XtraUserControl";
				string nameForPopUpMenu = lookUpTable.LookUpCollectionPropertyAssociationName +"PopUpMenu";
			this.Write("\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(" = New DevExpress.XtraGrid.GridControl()\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(" = New DevExpress.XtraGrid.Views.Grid.GridView()\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(" = New DevExpress.XtraBars.BarManager()\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(" = New DevExpress.XtraBars.Bar()\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(" = New DevExpress.XtraEditors.XtraUserControl()\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(" = New DevExpress.XtraBars.PopupMenu(Me.components)\r\n\t\t\t");
foreach(var commandsLookUpTable in lookUpTable.Commands){
				string nameForBarItemInLookUpTable = lookUpTable.LookUpCollectionPropertyAssociationName + commandsLookUpTable.CommandPropertyName.Remove(commandsLookUpTable.CommandPropertyName.Length -7,7);
			this.Write("\t\t\tMe.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write("  = New DevExpress.XtraBars.BarButtonItem()\r\n\t\t\t");
}
			this.Write("\t\t\tCType(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(", System.ComponentModel.ISupportInitialize).BeginInit()\r\n\t\t\t");
}
			this.Write("\t\t\t");
			foreach(var realLookUpInfo in listLookUpInfo){
			string nameForLookUp = realLookUpInfo.Property.Name+"LookUpEdit";
			string nameForLookUpBindingSource = realLookUpInfo.Property.Name+ "BindingSource";
			this.Write("\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(" = New DevExpress.XtraEditors.GridLookUpEdit()\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUpBindingSource));
			this.Write(" = New System.Windows.Forms.BindingSource(Me.components)\r\n\t\t\t");
}
			this.Write(@" 
			Me.SuspendLayout()
			' 
			' mainRibbonControl
			' 
			Me.mainRibbonControl.ExpandCollapseItem.Id = 0
			Me.mainRibbonControl.Items.AddRange(New DevExpress.XtraBars.BarItem() { Me.bbiCustomize })
			Me.mainRibbonControl.Name = ""mainRibbonControl""
			Me.mainRibbonControl.Pages.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPage() { Me.mainRibbonPage })
			Me.mainRibbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013
			Me.mainRibbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False
			Me.mainRibbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden
			");
foreach(var mainCommand in viewModelData.Commands){
				string nameForBarItemMain = mainCommand.CommandPropertyName.Remove(mainCommand.CommandPropertyName.Length -7,7);
				string imageUri = nameForBarItemMain;
				if(nameForBarItemMain.Contains("Layout")){
					imageUri = nameForBarItemMain.Replace("Layout", string.Empty);
				} 
			this.Write("\t\t\tDim bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(" As New DevExpress.XtraBars.BarButtonItem()\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(".Caption = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(mainCommand.Caption));
			this.Write("\"\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(".Name = \"bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write("\"\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(".ImageUri.Uri = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(imageUri));
			this.Write("\"\r\n\t\t\tMe.mainRibbonControl.Items.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(")\r\n\t\t\t");
}
			this.Write(@"			' 
			' mainRibbonPage
			' 
			Me.mainRibbonPage.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() { Me.mainRibbonPageGroup, Me.ribbonPageGroupLayout})
			Me.mainRibbonPage.Name = ""mainRibbonPage""
			Me.mainRibbonPage.Text = ""Home""
			Me.mainRibbonPage.MergeOrder = 0
			' 
			' mainRibbonPageGroup
			' 
			Me.mainRibbonPageGroup.AllowTextClipping = False
			");
foreach(var mainCommand in viewModelData.Commands){
				string nameForBarItemMain = mainCommand.CommandPropertyName.Remove(mainCommand.CommandPropertyName.Length -7,7);
				if(!nameForBarItemMain.Contains("Layout")){
			this.Write("\t\t\tMe.mainRibbonPageGroup.ItemLinks.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(")\r\n\t\t\t");
}}
			this.Write("\t\t\tMe.mainRibbonPageGroup.Name = \"mainRibbonPageGroup\"\r\n\t\t\tMe.mainRibbonPageGroup" +
					".ShowCaptionButton = False\r\n\t\t\tMe.mainRibbonPageGroup.Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeName));
			this.Write(" Tasks\"\r\n\t\t\t\'\r\n\t\t\t\' ribbonPageGroupLayout\r\n\t\t\t\'\r\n\t\t\tMe.ribbonPageGroupLayout.Allo" +
					"wTextClipping = False\r\n\t\t\tMe.ribbonPageGroupLayout.ItemLinks.Add(Me.bbiCustomize" +
					")\r\n\t\t\t");
foreach(var mainCommand in viewModelData.Commands){
				string nameForBarItemMain = mainCommand.CommandPropertyName.Remove(mainCommand.CommandPropertyName.Length -7,7);
				if(nameForBarItemMain.Contains("Layout")){
			this.Write("\t\t\tMe.ribbonPageGroupLayout.ItemLinks.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write(")\r\n\t\t\t");
}}
			this.Write(@"			Me.ribbonPageGroupLayout.Name = ""ribbonPageGroupLayout""
			Me.ribbonPageGroupLayout.Text = ""Layout""
			' 
			' bbiCustomize
			' 
			Me.bbiCustomize.Caption = ""Customize""
			Me.bbiCustomize.ImageUri.Uri = ""Customization""
			Me.bbiCustomize.Name = ""bbiCustomize""
			' 
			' layoutControlGroup1
			' 
			Me.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True
			Me.layoutControlGroup1.GroupBordersVisible = False
			Me.layoutControlGroup1.Name = ""Root""
			Me.layoutControlGroup1.TextVisible = False
			' 
			' dataLayoutControl1
			' 
			Me.dataLayoutControl1.AllowCustomization = False
			Me.dataLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill
			Me.dataLayoutControl1.Root = Me.layoutControlGroup1
			Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(" = New System.Windows.Forms.BindingSource(Me.components)\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(".DataSource = GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeFullName));
			this.Write(")\r\n\t\t\tMe.dataLayoutControl1.DataSource = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write("\r\n\t\t\t\'\r\n\t\t\t\'Create GridControls\r\n\t\t\t\'\r\n\t\t\tDim parameters As New DevExpress.XtraDa" +
					"taLayout.RetrieveFieldsParameters()\r\n\t\t\tparameters.DataSourceUpdateMode = System" +
					".Windows.Forms.DataSourceUpdateMode.OnPropertyChanged\r\n          \t");
			foreach(var lookUpTable in viewModelData.LookUpTables){
				string nameForGridControl = lookUpTable.LookUpCollectionPropertyAssociationName +"GridControl";
				string nameForGridView = lookUpTable.LookUpCollectionPropertyAssociationName +"GridView";
				string nameForBindingSource = lookUpTable.LookUpCollectionPropertyAssociationName +"BindingSource";
				string nameForBarManager = lookUpTable.LookUpCollectionPropertyAssociationName +"BarManager";
				string nameForBar = lookUpTable.LookUpCollectionPropertyAssociationName +"Bar";
				string nameForUserControl = lookUpTable.LookUpCollectionPropertyAssociationName +"XtraUserControl";
				string nameForPopUpMenu = lookUpTable.LookUpCollectionPropertyAssociationName +"PopUpMenu";
				string nameForParemeters = lookUpTable.LookUpCollectionPropertyAssociationName +"PopulateColumnsParameters";
				IEnumerable<IEdmPropertyInfo> scaffoldingProperties = lookUpTable.GetScaffoldingProperties();
				List<IEdmPropertyInfo>  navigationProperty = new List<IEdmPropertyInfo>();
				System.Reflection.PropertyInfo[] typeProperties = lookUpTable.EntityType.GetProperties();
				List<string> NonVisibleTypes = typeProperties.Where(e => !scaffoldingProperties.Any(q => q.Name == e.Name)).Select(e => e.Name).ToList();
				foreach(IEdmPropertyInfo property in scaffoldingProperties){
						if(property.IsForeignKey) NonVisibleTypes.Add(property.Name);
						if(property.IsNavigationProperty){
							if(property.PropertyType == viewModelData.EntityType) NonVisibleTypes.Add(property.Name);
							else navigationProperty.Add(property);
						}
				}
			this.Write("\t\t\t\'\r\n\t\t\t\'");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write("\r\n\t\t\t\'\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".MainView = Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write("\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".Dock = System.Windows.Forms.DockStyle.Fill\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".MenuManager = Me.mainRibbonControl\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write("\"\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() { Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write("})\r\n\t\t\t\'\r\n\t\t\t\'");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write("\r\n\t\t\t\'\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".GridControl = Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write("\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write("\"\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".OptionsBehavior.Editable = False\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".OptionsBehavior.ReadOnly = True\r\n\t\t\t\'\r\n\t\t\t\' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForParemeters));
			this.Write("\r\n\t\t\t\'\r\n\t\t\tDim ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForParemeters));
			this.Write(" As New DevExpress.XtraGrid.Extensions.PopulateColumnsParameters()\r\n\t\t\t");
foreach(var noVisibleColumn in NonVisibleTypes){
			 string nameForNonVisibleColumn = noVisibleColumn + lookUpTable.LookUpCollectionPropertyAssociationName;
			this.Write("\t\r\n\t\t\tDim ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForNonVisibleColumn));
			this.Write("ChildPopulateColumnParameters_NotVisible As New DevExpress.XtraGrid.Extensions.Po" +
					"pulateColumnParameters()\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForNonVisibleColumn));
			this.Write("ChildPopulateColumnParameters_NotVisible.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("\"\r\n\t\t    ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForNonVisibleColumn));
			this.Write("ChildPopulateColumnParameters_NotVisible.ColumnVisible = False\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForParemeters));
			this.Write(".CustomColumnParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForNonVisibleColumn));
			this.Write("ChildPopulateColumnParameters_NotVisible)\r\n\t\t\t");
}
			this.Write(" \r\n\t\t\t");
foreach(var navProperty in navigationProperty){
				string displayMember = EditorsSource.GetDisplayMemberPropertyName(navProperty.PropertyType);
				string BindingPath = navProperty.Name + ((string.IsNullOrEmpty(displayMember) ? null : "." + displayMember));
				string name = navProperty.Name + lookUpTable.LookUpCollectionPropertyAssociationName;
			this.Write("\t\r\n\t\t\tDim ");
			this.Write(this.ToStringHelper.ToStringWithCulture(name));
			this.Write("ChildPopulateColumnParameters As New DevExpress.XtraGrid.Extensions.PopulateColum" +
					"nParameters()\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(name));
			this.Write("ChildPopulateColumnParameters.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(name));
			this.Write("\"\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(name));
			this.Write("ChildPopulateColumnParameters.Path = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(BindingPath));
			this.Write("\"\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForParemeters));
			this.Write(".CustomColumnParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(name));
			this.Write("ChildPopulateColumnParameters)\r\n\t\t\t");
}
			this.Write("\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".PopulateColumns(GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.EntityTypeFullName));
			this.Write("), ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForParemeters));
			this.Write(") \r\n\t\t\t\'\r\n\t\t\t\' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBindingSource));
			this.Write("\r\n\t\t\t\'\r\n\t\t\tDim ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBindingSource));
			this.Write("  As New System.Windows.Forms.BindingSource(Me.components)\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBindingSource));
			this.Write(".DataSource =  GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.EntityTypeFullName));
			this.Write(")\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".DataSource = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBindingSource));
			this.Write(" \r\n\t\t\t\'\r\n\t\t\t\'");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write("\r\n\t\t\t\'\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(".Controls.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(")\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write("\"\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(".MinimumSize = new System.Drawing.Size(100, 100) \r\n\t\t\t\t");
foreach(var commandLookUpTable in lookUpTable.Commands){
						string nameForBarItemInLookUpTable = lookUpTable.LookUpCollectionPropertyAssociationName + commandLookUpTable.CommandPropertyName.Remove(commandLookUpTable.CommandPropertyName.Length -7,7);
						string imageUri = commandLookUpTable.CommandPropertyName.Remove(commandLookUpTable.CommandPropertyName.Length -7,7);
			this.Write("\t\t\t\'\r\n\t\t\t\'");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write("\r\n\t\t\t\'\r\n\t\t\tMe.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(".Caption = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(commandLookUpTable.Caption));
			this.Write("\"\r\n\t\t\tMe.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(".Name = \"bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write("\"\r\n\t\t\tMe.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(".ImageUri.Uri = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(imageUri));
			this.Write("\"\r\n\t\t\tMe.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(".PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(".Items.Add(Me.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(")\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".LinksPersistInfo.Add(New DevExpress.XtraBars.LinkPersistInfo(Me.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write("))\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(".LinksPersistInfo.Add(New DevExpress.XtraBars.LinkPersistInfo(Me.bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write("))\r\n\t\t\t");
}
			this.Write("\t\t\t\'\r\n\t\t\t\' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write("\r\n\t\t\t\'\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".BarName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("\"\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".DockCol = 0\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".DockRow = 0\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".DockStyle = DevExpress.XtraBars.BarDockStyle.Top\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".OptionsBar.AllowQuickCustomization = False\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".OptionsBar.DrawDragBorder = False\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("\"\r\n\t\t\t\'\r\n\t\t\t\' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write("\r\n\t\t\t\'\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(".AllowCustomization = False\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(".Bars.AddRange(New DevExpress.XtraBars.Bar() {Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write("})\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(".Form = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write("\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(".MainMenu = Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write("\r\n\t\t\t\' \r\n            \' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write("\r\n            \' \r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(".Manager = Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write("\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write("\"\r\n\t\t\t\'\r\n\t\t\t\' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters\r\n\t\t\t\'\r\n\t\t\tDim ");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters As New DevExpress.XtraDataLayout.RetrieveFieldParameters()" +
					"\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("\"\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters.ControlForField = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write("\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters.CreateTabGroupForItem = True\r\n\t\t\tparameters.CustomListPara" +
					"meters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write("RetriveFieldParameters)\r\n\t\t\t");
}
			this.Write("\t\t\t");
			foreach(var realLookUpInfo in listLookUpInfo){
			string nameForLookUp = realLookUpInfo.Property.Name+"LookUpEdit";
			string nameForLookUpBindingSource = realLookUpInfo.Property.Name+ "BindingSource";
			this.Write("\t\t\t\'\r\n\t\t\t\'");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("\r\n\t\t\t\'\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUpBindingSource));
			this.Write(".DataSource = GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Property.PropertyType.FullName));
			this.Write(")\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".Properties.ValueMember = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.ForeignKeyInfo.PrimaryKeyPropertyName));
			this.Write("\"\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".Properties.DisplayMember = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.DisplayMemberPropertyName));
			this.Write("\"\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".Properties.DataSource = Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUpBindingSource));
			this.Write("\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("\"\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".DataBindings.Add(\"EditValue\", ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(", \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.ForeignKeyInfo.ForeignKeyPropertyName));
			this.Write("\", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged)\r\n\t\t\tDim ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters As New DevExpress.XtraDataLayout.RetrieveFieldParameters(" +
					")\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.ForeignKeyInfo.ForeignKeyPropertyName));
			this.Write("\"\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters.ControlForField = Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("\r\n\t\t\tparameters.CustomListParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters)\r\n\t\t\tDim ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters_NotGenerate As New DevExpress.XtraDataLayout.RetrieveFiel" +
					"dParameters()\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters_NotGenerate.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Property.Name));
			this.Write("\"\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters_NotGenerate.GenerateField = False\r\n\t\t\tparameters.CustomLi" +
					"stParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("RetrieveFieldParameters_NotGenerate)\r\n\t\t\t");
}
			this.Write("\t\t\t\'\r\n\t\t\t\'call RetrieveFields\r\n\t\t\t\'\r\n\t\t\tMe.dataLayoutControl1.RetrieveFields(para" +
					"meters)\r\n\t\t\t\' \r\n\t\t\t\' mvvmContext\r\n\t\t\t\' \r\n\t\t\tMe.mvvmContext.ContainerControl = Me" +
					"\r\n            Me.mvvmContext.ViewModelType = GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(")\r\n\t\t\t");
foreach(var item in viewModelData.Commands){
				string nameForBarItemMain = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			this.Write("\t\t\tMe.mvvmContext.BindingExpressions.Add(DevExpress.Utils.MVVM.BindingExpression." +
					"CreateCommandBinding(GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write("), \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write("\", bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemMain));
			this.Write("))\r\n\t\t\t");
}
			this.Write(@"            Me.mvvmContext.RegistrationExpressions.AddRange(New DevExpress.Utils.MVVM.RegistrationExpression() {
            DevExpress.Utils.MVVM.RegistrationExpression.RegisterLayoutSerializationService(Nothing, False, DevExpress.Utils.DefaultBoolean.True, Me.dataLayoutControl1)})
			'
			'");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
			'
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange
			Me.Controls.Add(Me.dataLayoutControl1)
			Me.Controls.Add(Me.mainRibbonControl)
			Me.Size = New System.Drawing.Size(1024, 768)
			Me.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("\"\r\n\t\t\t");
foreach(var lookUpTable in viewModelData.LookUpTables){
				string nameForBarManager = lookUpTable.LookUpCollectionPropertyAssociationName +"BarManager";
			this.Write("\t\t\tCType(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(", System.ComponentModel.ISupportInitialize).EndInit()\r\n\t\t\t");
}
			this.Write(@"			Me.ResumeLayout(False)
		End Sub

		#End Region

		Private dataLayoutControl1 As DevExpress.XtraDataLayout.DataLayoutControl
		Private layoutControlGroup1 As DevExpress.XtraLayout.LayoutControlGroup
		Private mvvmContext As DevExpress.Utils.MVVM.MVVMContext
		Private mainRibbonControl As DevExpress.XtraBars.Ribbon.RibbonControl
		Private mainRibbonPage As DevExpress.XtraBars.Ribbon.RibbonPage
		Private mainRibbonPageGroup As DevExpress.XtraBars.Ribbon.RibbonPageGroup
		Private bbiCustomize As DevExpress.XtraBars.BarButtonItem
		Private ribbonPageGroupLayout As DevExpress.XtraBars.Ribbon.RibbonPageGroup
		Private ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(" As System.Windows.Forms.BindingSource\r\n\t\t");
foreach(var lookUpTable in viewModelData.LookUpTables){
				string nameForGridControl = lookUpTable.LookUpCollectionPropertyAssociationName +"GridControl";
				string nameForGridView = lookUpTable.LookUpCollectionPropertyAssociationName +"GridView";
				string nameForBarManager = lookUpTable.LookUpCollectionPropertyAssociationName +"BarManager";
				string nameForBar = lookUpTable.LookUpCollectionPropertyAssociationName +"Bar";
				string nameForUserControl = lookUpTable.LookUpCollectionPropertyAssociationName +"XtraUserControl";
				string nameForPopUpMenu = lookUpTable.LookUpCollectionPropertyAssociationName +"PopUpMenu";
			this.Write("\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(" As DevExpress.XtraGrid.GridControl\r\n\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(" As DevExpress.XtraGrid.Views.Grid.GridView\r\n\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarManager));
			this.Write(" As DevExpress.XtraBars.BarManager\r\n\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBar));
			this.Write(" As DevExpress.XtraBars.Bar\r\n\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForUserControl));
			this.Write(" As DevExpress.XtraEditors.XtraUserControl\r\n\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(" As DevExpress.XtraBars.PopupMenu\r\n\t\t\t");
foreach(var commandLookUpTable in lookUpTable.Commands){
			string nameForBarItemInLookUpTable = lookUpTable.LookUpCollectionPropertyAssociationName + commandLookUpTable.CommandPropertyName.Remove(commandLookUpTable.CommandPropertyName.Length -7,7);
			this.Write("\t\tPrivate bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(" As DevExpress.XtraBars.BarButtonItem\r\n\t\t\t");
}
			this.Write("\t\t");
}
			this.Write("\t\t");
		foreach(var realLookUpInfo in listLookUpInfo){
		string nameForLookUp = realLookUpInfo.Property.Name+"LookUpEdit";
		string nameForLookUpBindingSource = realLookUpInfo.Property.Name+ "BindingSource";
			this.Write("\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(" As DevExpress.XtraEditors.GridLookUpEdit\r\n\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUpBindingSource));
			this.Write(" As System.Windows.Forms.BindingSource\r\n\t\t");
}
			this.Write(" \r\n\tEnd Class\r\nEnd Namespace\r\n\r\n");
}
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class ElementView_TabbedMDIDesignerBase
	{
		#region Fields
		private global::System.Text.StringBuilder generationEnvironmentField;
		private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
		private global::System.Collections.Generic.List<int> indentLengthsField;
		private string currentIndentField = "";
		private bool endsWithNewline;
		private global::System.Collections.Generic.IDictionary<string, object> sessionField;
		#endregion
		#region Properties
		protected System.Text.StringBuilder GenerationEnvironment
		{
			get
			{
				if ((this.generationEnvironmentField == null))
				{
					this.generationEnvironmentField = new global::System.Text.StringBuilder();
				}
				return this.generationEnvironmentField;
			}
			set
			{
				this.generationEnvironmentField = value;
			}
		}
		public System.CodeDom.Compiler.CompilerErrorCollection Errors
		{
			get
			{
				if ((this.errorsField == null))
				{
					this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
				}
				return this.errorsField;
			}
		}
		private System.Collections.Generic.List<int> indentLengths
		{
			get
			{
				if ((this.indentLengthsField == null))
				{
					this.indentLengthsField = new global::System.Collections.Generic.List<int>();
				}
				return this.indentLengthsField;
			}
		}
		public string CurrentIndent
		{
			get
			{
				return this.currentIndentField;
			}
		}
		public virtual global::System.Collections.Generic.IDictionary<string, object> Session
		{
			get
			{
				return this.sessionField;
			}
			set
			{
				this.sessionField = value;
			}
		}
		#endregion
		#region Transform-time helpers
		public void Write(string textToAppend)
		{
			if (string.IsNullOrEmpty(textToAppend))
			{
				return;
			}
			if (((this.GenerationEnvironment.Length == 0) 
						|| this.endsWithNewline))
			{
				this.GenerationEnvironment.Append(this.currentIndentField);
				this.endsWithNewline = false;
			}
			if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
			{
				this.endsWithNewline = true;
			}
			if ((this.currentIndentField.Length == 0))
			{
				this.GenerationEnvironment.Append(textToAppend);
				return;
			}
			textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
			if (this.endsWithNewline)
			{
				this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
			}
			else
			{
				this.GenerationEnvironment.Append(textToAppend);
			}
		}
		public void WriteLine(string textToAppend)
		{
			this.Write(textToAppend);
			this.GenerationEnvironment.AppendLine();
			this.endsWithNewline = true;
		}
		public void Write(string format, params object[] args)
		{
			this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
		}
		public void WriteLine(string format, params object[] args)
		{
			this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
		}
		public void Error(string message)
		{
			System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
			error.ErrorText = message;
			this.Errors.Add(error);
		}
		public void Warning(string message)
		{
			System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
			error.ErrorText = message;
			error.IsWarning = true;
			this.Errors.Add(error);
		}
		public void PushIndent(string indent)
		{
			if ((indent == null))
			{
				throw new global::System.ArgumentNullException("indent");
			}
			this.currentIndentField = (this.currentIndentField + indent);
			this.indentLengths.Add(indent.Length);
		}
		public string PopIndent()
		{
			string returnValue = "";
			if ((this.indentLengths.Count > 0))
			{
				int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
				this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
				if ((indentLength > 0))
				{
					returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
					this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
				}
			}
			return returnValue;
		}
		public void ClearIndent()
		{
			this.indentLengths.Clear();
			this.currentIndentField = "";
		}
		#endregion
		#region ToString Helpers
		public class ToStringInstanceHelper
		{
			private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
			public System.IFormatProvider FormatProvider
			{
				get
				{
					return this.formatProviderField ;
				}
				set
				{
					if ((value != null))
					{
						this.formatProviderField  = value;
					}
				}
			}
			public string ToStringWithCulture(object objectToConvert)
			{
				if ((objectToConvert == null))
				{
					throw new global::System.ArgumentNullException("objectToConvert");
				}
				System.Type t = objectToConvert.GetType();
				System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
							typeof(System.IFormatProvider)});
				if ((method == null))
				{
					return objectToConvert.ToString();
				}
				else
				{
					return ((string)(method.Invoke(objectToConvert, new object[] {
								this.formatProviderField })));
				}
			}
		}
		private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
		public ToStringInstanceHelper ToStringHelper
		{
			get
			{
				return this.toStringHelperField;
			}
		}
		#endregion
	}
	#endregion
}
