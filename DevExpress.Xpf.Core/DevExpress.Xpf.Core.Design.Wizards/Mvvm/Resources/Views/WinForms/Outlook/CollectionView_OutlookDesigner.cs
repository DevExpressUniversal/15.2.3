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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views.WinForms.Outlook
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
	public partial class CollectionView_OutlookDesigner : CollectionView_OutlookDesignerBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	CollectionViewModelData viewModelData = templateInfo.Properties["IViewModelInfo"] as CollectionViewModelData;
	UIType uiType = (UIType)templateInfo.Properties["UIType"];
	string viewName = templateInfo.Properties["ViewName"].ToString();	
	string localNamespace = templateInfo.Properties["Namespace"].ToString();
	string viewFullName = localNamespace +"." + viewName;
	string mvvmContextFullName = viewModelData.Namespace+"."+viewModelData.Name;
	string bindingSourceName = Char.ToLowerInvariant(viewName[0]) + viewName.Substring(1) + "BindingSource";
	List<PropertyEditorInfo> listLookUpInfo = templateInfo.Properties["GeneratedLookups"] as List<PropertyEditorInfo>;
	bool IsVisualBasic = (bool)templateInfo.Properties["IsVisualBasic"];
	IEnumerable<IEdmPropertyInfo> scaffoldingProperties = viewModelData.GetScaffoldingProperties();
	System.Reflection.PropertyInfo[] typeProperties = viewModelData.EntityType.GetProperties();
	IEnumerable<string> NonVisibleTypes = typeProperties.Where(e => !scaffoldingProperties.Any(q => q.Name == e.Name)).Select(e => e.Name);
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
					".ComponentModel.Container();\r\n\t\t\tthis.gridControl = new DevExpress.XtraGrid.Grid" +
					"Control();\r\n            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridV" +
					"iew();\r\n\t\t\tthis.mvvmContext = new DevExpress.Utils.MVVM.MVVMContext(this.compone" +
					"nts);\r\n\t\t\tthis.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();\r\n" +
					"\t\t\tthis.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();\r\n            " +
					"this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();\r\n\t\t\tth" +
					"is.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();\r\n       " +
					"     this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();\r\n " +
					"           this.bsiRecordsCount = new DevExpress.XtraBars.BarStaticItem();\r\n\t\t\tt" +
					"his.bbiPrintPreview = new DevExpress.XtraBars.BarButtonItem();\r\n\t\t\tthis.popupMen" +
					"u = new DevExpress.XtraBars.PopupMenu(this.components);\r\n            ((System.Co" +
					"mponentModel.ISupportInitialize)(this.gridControl)).BeginInit();\r\n            ((" +
					"System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();\r\n        " +
					"    ((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).BeginInit()" +
					";\r\n            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).Be" +
					"ginInit();\r\n\t\t\t((System.ComponentModel.ISupportInitialize)(this.popupMenu)).Begi" +
					"nInit();\r\n            this.SuspendLayout();\r\n\t\t\t// \r\n            // ribbonContro" +
					"l\r\n            // \r\n            this.ribbonControl.ExpandCollapseItem.Id = 0;\r\n " +
					"           this.ribbonControl.MaxItemId = 14;\r\n            this.ribbonControl.Na" +
					"me = \"ribbonControl\";\r\n\t\t\tthis.ribbonControl.Pages.AddRange(new DevExpress.XtraB" +
					"ars.Ribbon.RibbonPage[] {\r\n            this.ribbonPage1});\r\n            this.rib" +
					"bonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office201" +
					"3;\r\n            this.ribbonControl.ShowApplicationButton = DevExpress.Utils.Defa" +
					"ultBoolean.False;\r\n            this.ribbonControl.ToolbarLocation = DevExpress.X" +
					"traBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;\r\n\t\t\t this.ribbonControl." +
					"Items.AddRange(new DevExpress.XtraBars.BarItem[] {this.bbiPrintPreview, this.bsi" +
					"RecordsCount});\r\n\t\t\t");
foreach(var item in viewModelData.Commands){
				string nameForItem = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			this.Write("\t\t\tDevExpress.XtraBars.BarButtonItem bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = new DevExpress.XtraBars.BarButtonItem();\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Caption = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\";\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Name = \"bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\";\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".ImageUri.Uri = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\";\r\n\t\t\tthis.ribbonControl.Items.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(");\r\n\t\t\t");
}	
			this.Write(@"            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2});
			this.ribbonPage1.MergeOrder = 0;
            this.ribbonPage1.Name = ""ribbonPage1"";
            this.ribbonPage1.Text = ""Home"";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.AllowTextClipping = false;
			");
foreach(var item in viewModelData.Commands){
				string nameForItem = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			this.Write("\t\t\tthis.ribbonPageGroup1.ItemLinks.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(");\r\n\t\t\t");
}
			this.Write("            this.ribbonPageGroup1.Name = \"ribbonPageGroup1\";\r\n            this.ri" +
					"bbonPageGroup1.ShowCaptionButton = false;\r\n            this.ribbonPageGroup1.Tex" +
					"t = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeName));
			this.Write(" Tasks\";\r\n\t\t\t// \r\n            // ribbonPageGroup2\r\n            // \r\n            t" +
					"his.ribbonPageGroup2.ItemLinks.Add(this.bbiPrintPreview);\r\n            this.ribb" +
					"onPageGroup2.Name = \"ribbonPageGroup2\";\r\n            this.ribbonPageGroup2.Text " +
					"= \"Print and Export\";\r\n\t\t\tthis.ribbonPageGroup2.AllowTextClipping = false;\r\n\t\t\tt" +
					"his.ribbonPageGroup2.ShowCaptionButton = false;\r\n\t\t\t// \r\n            // ribbonSt" +
					"atusBar\r\n            // \r\n            this.ribbonStatusBar.ItemLinks.Add(this.bs" +
					"iRecordsCount);\r\n            this.ribbonStatusBar.Name = \"ribbonStatusBar\";\r\n   " +
					"         this.ribbonStatusBar.Ribbon = this.ribbonControl;\r\n\t\t\t// \r\n            " +
					"// bbiPrintPreview\r\n            // \r\n            this.bbiPrintPreview.Caption = " +
					"\"Print Preview\";\r\n            this.bbiPrintPreview.ImageUri.Uri = \"Preview\";\r\n  " +
					"          this.bbiPrintPreview.Name = \"bbiPrintPreview\";\r\n\t\t\t// \r\n            //" +
					" barStaticItem1\r\n            // \r\n            this.bsiRecordsCount.Caption = \"RE" +
					"CORDS : 2\";\r\n            this.bsiRecordsCount.Name = \"bsiRecordsCount\";\r\n       " +
					"     this.bsiRecordsCount.TextAlignment = System.Drawing.StringAlignment.Near;\r\n" +
					"\t\t\t// \r\n            // gridControl\r\n            // \r\n            this.gridContro" +
					"l.Dock = System.Windows.Forms.DockStyle.Fill;\r\n            this.gridControl.Loca" +
					"tion = new System.Drawing.Point(5, 116);\r\n            this.gridControl.MainView " +
					"= this.gridView;\r\n            this.gridControl.MenuManager = this.ribbonControl;" +
					"\r\n            this.gridControl.Name = \"gridControl\";\r\n            this.gridContr" +
					"ol.Size = new System.Drawing.Size(779, 311);\r\n            this.gridControl.TabIn" +
					"dex = 2;\r\n            this.gridControl.ViewCollection.AddRange(new DevExpress.Xt" +
					"raGrid.Views.Base.BaseView[] {\r\n            this.gridView});\r\n            // \r\n " +
					"           // gridView\r\n            // \r\n            this.gridView.BorderStyle =" +
					" DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;\r\n            this.gridVi" +
					"ew.GridControl = this.gridControl;\r\n            this.gridView.Name = \"gridView\";" +
					"\r\n            this.gridView.OptionsBehavior.Editable = false;\r\n            this." +
					"gridView.OptionsBehavior.ReadOnly = true;\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(" = new System.Windows.Forms.BindingSource(this.components);\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(".DataSource = typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeFullName));
			this.Write(");\r\n\t\t\tthis.gridControl.DataSource = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(";\r\n\r\n\t\t\tDevExpress.XtraGrid.Extensions.PopulateColumnsParameters parameters = new" +
					" DevExpress.XtraGrid.Extensions.PopulateColumnsParameters();\r\n\t\t\t");
			foreach(var realLookUpInfo in listLookUpInfo){
			string nameForLookUp =  realLookUpInfo.Property.Name;
			this.Write("\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("\r\n\t\t\t//\r\n\t\t\tDevExpress.XtraGrid.Extensions.PopulateColumnParameters ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters = new DevExpress.XtraGrid.Extensions.PopulateColumnParam" +
					"eters();\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Property.Name));
			this.Write("\";\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters.Path = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Property.Name));
			this.Write(".");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.DisplayMemberPropertyName));
			this.Write("\";\r\n\t\t\tparameters.CustomColumnParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters);\r\n\t\t\t");
if(realLookUpInfo.Lookup.ForeignKeyInfo != null && realLookUpInfo.Lookup.ForeignKeyInfo.ForeignKeyPropertyName != null){
			this.Write("\t\t\tDevExpress.XtraGrid.Extensions.PopulateColumnParameters ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters_NotGenerate = new DevExpress.XtraGrid.Extensions.Populat" +
					"eColumnParameters();\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters_NotGenerate.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.ForeignKeyInfo.ForeignKeyPropertyName));
			this.Write("\";\r\n\t\t    ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters_NotGenerate.ColumnVisible = false;\r\n\t\t\tparameters.Custom" +
					"ColumnParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters_NotGenerate);\r\n\t\t\t");
}
			this.Write("\t\t\t");
}
			this.Write("\t\t\t");
foreach(var noVisibleColumn in NonVisibleTypes){
			this.Write("\t\r\n\t\t\tDevExpress.XtraGrid.Extensions.PopulateColumnParameters ");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("PopulateColumnParameters_NotVisible = new DevExpress.XtraGrid.Extensions.Populate" +
					"ColumnParameters();\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("PopulateColumnParameters_NotVisible.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("\";\r\n\t\t    ");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("PopulateColumnParameters_NotVisible.ColumnVisible = false;\r\n\t\t\tparameters.CustomC" +
					"olumnParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("PopulateColumnParameters_NotVisible);\r\n\t\t\t");
}
			this.Write(" \r\n\t\t\tthis.gridView.PopulateColumns(typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeFullName));
			this.Write("),parameters);\r\n\t\t\t// \r\n            // popupMenu1\r\n            // \r\n\t\t\t");
foreach(var item in viewModelData.Commands){
				string nameForItem = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			this.Write("\t\t\tthis.popupMenu.ItemLinks.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(");\r\n\t\t\t");
}
			this.Write(@"            this.popupMenu.Name = ""popupMenu"";
            this.popupMenu.Ribbon = this.ribbonControl;
		    // 
            // mvvmContext
            // 
            this.mvvmContext.ContainerControl = this;
            this.mvvmContext.ViewModelType = typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(");\r\n\t\t\t");
foreach(var item in viewModelData.Commands){
				string nameForItem = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			this.Write("\t\t\t");
			if(String.IsNullOrEmpty(item.ParameterPropertyName))
			{
			this.Write("\t\t\tthis.mvvmContext.BindingExpressions.Add(DevExpress.Utils.MVVM.BindingExpressio" +
					"n.CreateCommandBinding(typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write("), \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\", bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("));\r\n\t\t\t");
}
			this.Write("\t\t\t");
			if(!String.IsNullOrEmpty(item.ParameterPropertyName))
			{
			this.Write("\t\t\tthis.mvvmContext.BindingExpressions.Add(DevExpress.Utils.MVVM.BindingExpressio" +
					"n.CreateParameterizedCommandBinding(typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write("), \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\", \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.ParameterPropertyName));
			this.Write("\", bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("));\r\n\t\t\t");
}
			this.Write("\t\t\t");
}
			this.Write(@"			this.mvvmContext.RegistrationExpressions.AddRange(new DevExpress.Utils.MVVM.RegistrationExpression[] {
            DevExpress.Utils.MVVM.RegistrationExpression.RegisterLayoutSerializationService(null, false, DevExpress.Utils.DefaultBoolean.Default, this.gridControl),
            DevExpress.Utils.MVVM.RegistrationExpression.RegisterWindowedDocumentManagerService(null, false, this, DevExpress.Utils.MVVM.Services.DefaultWindowedDocumentManagerServiceType.XtraForm, null)});
			//
			//");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.gridControl);
			this.Controls.Add(this.ribbonControl);
			this.Size = new System.Drawing.Size(1024, 768);
            this.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@""";
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
		}
		
        #endregion

		private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
		private DevExpress.Utils.MVVM.MVVMContext mvvmContext;
		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
		private System.Windows.Forms.BindingSource ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(@";
		private DevExpress.XtraBars.BarButtonItem bbiPrintPreview;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarStaticItem bsiRecordsCount;
		private DevExpress.XtraBars.PopupMenu popupMenu;
	}
}
");
}
if(IsVisualBasic){
			this.Write("Namespace Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("\r\n\tPartial Public Class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("\r\n\t\t\'\'\' <summary> \r\n\t\t\'\'\' Required designer variable.\r\n\t\t\'\'\' </summary>\r\n\t\tPrivat" +
					"e components As System.ComponentModel.IContainer = Nothing\r\n\r\n\t\t\'\'\' <summary> \r\n" +
					"\t\t\'\'\' Clean up any resources being used.\r\n\t\t\'\'\' </summary>\r\n\t\t\'\'\' <param name=\"d" +
					"isposing\">true if managed resources should be disposed; otherwise, false.</param" +
					">\r\n\t\tProtected Overrides Sub Dispose(ByVal disposing As Boolean)\r\n\t\t\tIf disposin" +
					"g AndAlso (components IsNot Nothing) Then\r\n\t\t\t\tcomponents.Dispose()\r\n\t\t\tEnd If\r\n" +
					"\t\t\tMyBase.Dispose(disposing)\r\n\t\tEnd Sub\r\n\t\t#Region \"Component Designer generated" +
					" code\"\r\n\t\t\r\n\t\t\'\'\' <summary> \r\n\t\t\'\'\' Required method for Designer support - do no" +
					"t modify \r\n\t\t\'\'\' the contents of this method with the code editor.\r\n\t\t\'\'\' </summ" +
					"ary>\r\n\t\tPrivate Sub InitializeComponent()\r\n\t\t\tMe.components = New System.Compone" +
					"ntModel.Container()\r\n\t\t\tMe.gridControl = New DevExpress.XtraGrid.GridControl()\r\n" +
					"\t\t\tMe.gridView = New DevExpress.XtraGrid.Views.Grid.GridView()\r\n\t\t\tMe.mvvmContex" +
					"t = New DevExpress.Utils.MVVM.MVVMContext(Me.components)\r\n\t\t\tMe.ribbonControl = " +
					"New DevExpress.XtraBars.Ribbon.RibbonControl()\r\n\t\t\tMe.ribbonPage1 = New DevExpre" +
					"ss.XtraBars.Ribbon.RibbonPage()\r\n\t\t\tMe.ribbonPageGroup1 = New DevExpress.XtraBar" +
					"s.Ribbon.RibbonPageGroup()\r\n\t\t\tMe.ribbonPageGroup2 = New DevExpress.XtraBars.Rib" +
					"bon.RibbonPageGroup()\r\n\t\t\tMe.ribbonStatusBar = New DevExpress.XtraBars.Ribbon.Ri" +
					"bbonStatusBar()\r\n\t\t\tMe.bsiRecordsCount = New DevExpress.XtraBars.BarStaticItem()" +
					"\r\n\t\t\tMe.bbiPrintPreview = New DevExpress.XtraBars.BarButtonItem()\r\n\t\t\tMe.popupMe" +
					"nu = New DevExpress.XtraBars.PopupMenu(Me.components)\r\n\t\t\tCType(Me.gridControl, " +
					"System.ComponentModel.ISupportInitialize).BeginInit()\r\n\t\t\tCType(Me.gridView, Sys" +
					"tem.ComponentModel.ISupportInitialize).BeginInit()\r\n\t\t\tCType(Me.ribbonControl, S" +
					"ystem.ComponentModel.ISupportInitialize).BeginInit()\r\n\t\t\tCType(Me.mvvmContext, S" +
					"ystem.ComponentModel.ISupportInitialize).BeginInit()\r\n\t\t\tCType(Me.popupMenu, Sys" +
					"tem.ComponentModel.ISupportInitialize).BeginInit()\r\n\t\t\tMe.SuspendLayout()\r\n\t\t\t\' " +
					"\r\n\t\t\t\' ribbonControl\r\n\t\t\t\' \r\n\t\t\tMe.ribbonControl.ExpandCollapseItem.Id = 0\r\n\t\t\tM" +
					"e.ribbonControl.MaxItemId = 14\r\n\t\t\tMe.ribbonControl.Name = \"ribbonControl\"\r\n\t\t\tM" +
					"e.ribbonControl.Pages.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPage() { Me." +
					"ribbonPage1})\r\n\t\t\tMe.ribbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.Ribb" +
					"onControlStyle.Office2013\r\n\t\t\tMe.ribbonControl.ShowApplicationButton = DevExpres" +
					"s.Utils.DefaultBoolean.False\r\n\t\t\tMe.ribbonControl.ToolbarLocation = DevExpress.X" +
					"traBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden\r\n\t\t\tMe.ribbonControl.Item" +
					"s.AddRange(New DevExpress.XtraBars.BarItem() { Me.bbiPrintPreview, Me.bsiRecords" +
					"Count })\r\n\t\t\t");
foreach(var item in viewModelData.Commands){
				string nameForItem = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			this.Write("\t\t\tDim bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" As New DevExpress.XtraBars.BarButtonItem()\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Caption = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\"\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Name = \"bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\"\r\n\t\t\tbbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".ImageUri.Uri = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\"\r\n\t\t\tMe.ribbonControl.Items.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(")\r\n\t\t\t");
}	
			this.Write(@"			' 
			' ribbonPage1
			' 
			Me.ribbonPage1.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() { Me.ribbonPageGroup1, Me.ribbonPageGroup2})
			Me.ribbonPage1.MergeOrder = 0
			Me.ribbonPage1.Name = ""ribbonPage1""
			Me.ribbonPage1.Text = ""Home""
			' 
			' ribbonPageGroup1
			' 
			Me.ribbonPageGroup1.AllowTextClipping = False
			");
foreach(var item in viewModelData.Commands){
				string nameForItem = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			this.Write("\t\t\tMe.ribbonPageGroup1.ItemLinks.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(")\r\n\t\t\t");
}
			this.Write("\t\t\tMe.ribbonPageGroup1.Name = \"ribbonPageGroup1\"\r\n\t\t\tMe.ribbonPageGroup1.ShowCapt" +
					"ionButton = False\r\n\t\t\tMe.ribbonPageGroup1.Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeName));
			this.Write(" Tasks\"\r\n\t\t\t\' \r\n\t\t\t\' ribbonPageGroup2\r\n\t\t\t\' \r\n\t\t\tMe.ribbonPageGroup2.ItemLinks.Ad" +
					"d(Me.bbiPrintPreview)\r\n\t\t\tMe.ribbonPageGroup2.Name = \"ribbonPageGroup2\"\r\n\t\t\tMe.r" +
					"ibbonPageGroup2.Text = \"Print and Export\"\r\n\t\t\tMe.ribbonPageGroup2.AllowTextClipp" +
					"ing = False\r\n\t\t\tMe.ribbonPageGroup2.ShowCaptionButton = False\r\n\t\t\t\' \r\n\t\t\t\' ribbo" +
					"nStatusBar\r\n\t\t\t\' \r\n\t\t\tMe.ribbonStatusBar.ItemLinks.Add(Me.bsiRecordsCount)\r\n\t\t\tM" +
					"e.ribbonStatusBar.Name = \"ribbonStatusBar\"\r\n\t\t\tMe.ribbonStatusBar.Ribbon = Me.ri" +
					"bbonControl\r\n\t\t\t\' \r\n\t\t\t\' bbiPrintPreview\r\n\t\t\t\' \r\n\t\t\tMe.bbiPrintPreview.Caption =" +
					" \"Print Preview\"\r\n\t\t\tMe.bbiPrintPreview.ImageUri.Uri = \"Preview\"\r\n\t\t\tMe.bbiPrint" +
					"Preview.Name = \"bbiPrintPreview\"\r\n\t\t\t\' \r\n\t\t\t\' barStaticItem1\r\n\t\t\t\' \r\n\t\t\tMe.bsiRe" +
					"cordsCount.Caption = \"RECORDS : 2\"\r\n\t\t\tMe.bsiRecordsCount.Name = \"bsiRecordsCoun" +
					"t\"\r\n\t\t\tMe.bsiRecordsCount.TextAlignment = System.Drawing.StringAlignment.Near\r\n\t" +
					"\t\t\' \r\n\t\t\t\' gridControl\r\n\t\t\t\' \r\n\t\t\tMe.gridControl.Dock = System.Windows.Forms.Doc" +
					"kStyle.Fill\r\n\t\t\tMe.gridControl.Location = New System.Drawing.Point(5, 116)\r\n\t\t\tM" +
					"e.gridControl.MainView = Me.gridView\r\n\t\t\tMe.gridControl.MenuManager = Me.ribbonC" +
					"ontrol\r\n\t\t\tMe.gridControl.Name = \"gridControl\"\r\n\t\t\tMe.gridControl.Size = New Sys" +
					"tem.Drawing.Size(779, 311)\r\n\t\t\tMe.gridControl.TabIndex = 2\r\n\t\t\tMe.gridControl.Vi" +
					"ewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() { Me.gridVie" +
					"w})\r\n\t\t\t\' \r\n\t\t\t\' gridView\r\n\t\t\t\' \r\n\t\t\tMe.gridView.BorderStyle = DevExpress.XtraEd" +
					"itors.Controls.BorderStyles.NoBorder\r\n\t\t\tMe.gridView.GridControl = Me.gridContro" +
					"l\r\n\t\t\tMe.gridView.Name = \"gridView\"\r\n\t\t\tMe.gridView.OptionsBehavior.Editable = F" +
					"alse\r\n\t\t\tMe.gridView.OptionsBehavior.ReadOnly = True\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(" = New System.Windows.Forms.BindingSource(Me.components)\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(".DataSource = GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeFullName));
			this.Write(")\r\n\t\t\tMe.gridControl.DataSource = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write("\r\n\r\n\t\t\tDim parameters As New DevExpress.XtraGrid.Extensions.PopulateColumnsParame" +
					"ters()\r\n\t\t\t");
			foreach(var realLookUpInfo in listLookUpInfo){
			string nameForLookUp =  realLookUpInfo.Property.Name;
			this.Write("\t\t\t\'\r\n\t\t\t\'");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("\r\n\t\t\t\'\r\n\t\t\tDim  ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters As New DevExpress.XtraGrid.Extensions.PopulateColumnPara" +
					"meters()\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Property.Name));
			this.Write("\"\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters.Path = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Property.Name));
			this.Write(".");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.DisplayMemberPropertyName));
			this.Write("\"\r\n\t\t\tparameters.CustomColumnParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters)\r\n\t\t\t");
if(realLookUpInfo.Lookup.ForeignKeyInfo != null && realLookUpInfo.Lookup.ForeignKeyInfo.ForeignKeyPropertyName != null){
			this.Write("\t\t\tDim ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters_NotGenerate As New DevExpress.XtraGrid.Extensions.Popula" +
					"teColumnParameters()\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters_NotGenerate.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.ForeignKeyInfo.ForeignKeyPropertyName));
			this.Write("\"\r\n\t\t    ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters_NotGenerate.ColumnVisible = False\r\n\t\t\tparameters.CustomC" +
					"olumnParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write("PopulateColumnParameters_NotGenerate)\r\n\t\t\t");
}
			this.Write("\t\t\t");
}
			this.Write("\t\t\t");
foreach(var noVisibleColumn in NonVisibleTypes){
			this.Write("\t\r\n\t\t\tDim ");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("PopulateColumnParameters_NotVisible As New DevExpress.XtraGrid.Extensions.Populat" +
					"eColumnParameters()\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("PopulateColumnParameters_NotVisible.FieldName = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("\"\r\n\t\t    ");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("PopulateColumnParameters_NotVisible.ColumnVisible = False\r\n\t\t\tparameters.CustomCo" +
					"lumnParameters.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(noVisibleColumn));
			this.Write("PopulateColumnParameters_NotVisible)\r\n\t\t\t");
}
			this.Write(" \r\n\t\t\tMe.gridView.PopulateColumns(GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeFullName));
			this.Write("), parameters)\r\n\t\t\t\' \r\n            \' popupMenu1\r\n            \' \r\n\t\t\t");
foreach(var item in viewModelData.Commands){
				string nameForItem = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			this.Write("\t\t\tMe.popupMenu.ItemLinks.Add(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(")\r\n\t\t\t");
}
			this.Write("\t\t\tMe.popupMenu.Name = \"popupMenu\"\r\n\t\t\tMe.popupMenu.Ribbon = Me.ribbonControl\r\n\t\t" +
					"\t\' \r\n            \' mvvmContext\r\n            \' \r\n            Me.mvvmContext.Conta" +
					"inerControl = Me\r\n            Me.mvvmContext.ViewModelType = GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(")\r\n\t\t\t");
foreach(var item in viewModelData.Commands){
				string nameForItem = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			this.Write("\t\t\t");
			if(String.IsNullOrEmpty(item.ParameterPropertyName))
			{
			this.Write("\t\t\tMe.mvvmContext.BindingExpressions.Add(DevExpress.Utils.MVVM.BindingExpression." +
					"CreateCommandBinding(GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write("), \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\", bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("))\r\n\t\t\t");
}
			this.Write("\t\t\t");
			if(!String.IsNullOrEmpty(item.ParameterPropertyName))
			{
			this.Write("\t\t\tMe.mvvmContext.BindingExpressions.Add(DevExpress.Utils.MVVM.BindingExpression." +
					"CreateParameterizedCommandBinding(GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write("), \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\", \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.ParameterPropertyName));
			this.Write("\", bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("))\r\n\t\t\t");
}
			this.Write("\t\t\t");
}
			this.Write(@"            Me.mvvmContext.RegistrationExpressions.AddRange(New DevExpress.Utils.MVVM.RegistrationExpression() {
            DevExpress.Utils.MVVM.RegistrationExpression.RegisterLayoutSerializationService(Nothing, False, DevExpress.Utils.DefaultBoolean.Default, Me.gridControl),
            DevExpress.Utils.MVVM.RegistrationExpression.RegisterWindowedDocumentManagerService(Nothing, False, Me, DevExpress.Utils.MVVM.Services.DefaultWindowedDocumentManagerServiceType.XtraForm, Nothing)})
			'
            '");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.Controls.Add(Me.ribbonStatusBar)
			Me.Controls.Add(Me.gridControl)
			Me.Controls.Add(Me.ribbonControl)
			Me.Size = New System.Drawing.Size(1024, 768)
			Me.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"""
			CType(Me.gridControl, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.gridView, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.ribbonControl, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.mvvmContext, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.popupMenu, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)
			Me.PerformLayout()
		End Sub

		#End Region

		Private gridControl As DevExpress.XtraGrid.GridControl
		Private gridView As DevExpress.XtraGrid.Views.Grid.GridView
		Private mvvmContext As DevExpress.Utils.MVVM.MVVMContext
		Private ribbonControl As DevExpress.XtraBars.Ribbon.RibbonControl
		Private ribbonPage1 As DevExpress.XtraBars.Ribbon.RibbonPage
		Private ribbonPageGroup1 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
		Private ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(@" As System.Windows.Forms.BindingSource
		Private bbiPrintPreview As DevExpress.XtraBars.BarButtonItem
		Private ribbonPageGroup2 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
		Private ribbonStatusBar As DevExpress.XtraBars.Ribbon.RibbonStatusBar
		Private bsiRecordsCount As DevExpress.XtraBars.BarStaticItem
		Private popupMenu As DevExpress.XtraBars.PopupMenu
	End Class
End Namespace
");
}
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class CollectionView_OutlookDesignerBase
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
