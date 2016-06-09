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
	using System;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class DocumentManagerView_OutlookDesigner : DocumentManagerView_OutlookDesignerBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	DocumentManagerViewModelInfo viewModelData = templateInfo.Properties["IViewModelInfo"] as DocumentManagerViewModelInfo;
	UIType uiType = (UIType)templateInfo.Properties["UIType"];
	string viewName = templateInfo.Properties["ViewName"].ToString();	
	string localNamespace = templateInfo.Properties["Namespace"].ToString();
	string viewFullName = localNamespace +"." + viewName;
	string mvvmContextFullName = viewModelData.Namespace+"."+viewModelData.Name;
	string bindingSourceName = Char.ToLowerInvariant(viewName[0]) + viewName.Substring(1) + "BindingSource";
	bool IsVisualBasic = (bool)templateInfo.Properties["IsVisualBasic"];
	bool CreateSubItemForTableAndView = viewModelData.Tables.Count() > 0 && viewModelData.Views.Count() > 0; 
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
					".ComponentModel.Container();\r\n\t\t\tthis.documentManager = new DevExpress.XtraBars." +
					"Docking2010.DocumentManager();\r\n            this.tabbedView = new DevExpress.Xtr" +
					"aBars.Docking2010.Views.Tabbed.TabbedView();\r\n\t\t\tthis.mvvmContext = new DevExpre" +
					"ss.Utils.MVVM.MVVMContext(this.components);\r\n\t\t\tthis.ribbonControl = new DevExpr" +
					"ess.XtraBars.Ribbon.RibbonControl();\r\n\t\t\tthis.ribbonPage = new DevExpress.XtraBa" +
					"rs.Ribbon.RibbonPage();\r\n            this.ribbonPageGroup = new DevExpress.XtraB" +
					"ars.Ribbon.RibbonPageGroup();\r\n\t\t\tthis.ribbonStatusBar = new DevExpress.XtraBars" +
					".Ribbon.RibbonStatusBar();\r\n\t\t\tthis.ribbonPageGroupNavigation = new DevExpress.X" +
					"traBars.Ribbon.RibbonPageGroup();\r\n\t\t\tthis.barSubItemNavigation = new DevExpress" +
					".XtraBars.BarSubItem();\r\n\t\t\tthis.skinRibbonGalleryBarItem = new DevExpress.XtraB" +
					"ars.SkinRibbonGalleryBarItem();\r\n\t\t\tthis.officeNavigationBar = new DevExpress.Xt" +
					"raBars.Navigation.OfficeNavigationBar();\r\n            this.navigationFrame = new" +
					" DevExpress.XtraBars.Navigation.NavigationFrame();\r\n\t\t\t");
if(CreateSubItemForTableAndView){
			this.Write("\t\t\tthis.barSubItemTables = new DevExpress.XtraBars.BarSubItem();\r\n\t\t\tthis.barSubI" +
					"temViews = new DevExpress.XtraBars.BarSubItem();\r\n\t\t\t");
}
			this.Write("\t\t\t");
foreach(var item in viewModelData.Tables){
					string nameForItem = "navigationBarItem" + item.ViewName;
					string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = new DevExpress.XtraBars.Navigation.NavigationBarItem();\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(" = new DevExpress.XtraBars.BarButtonItem();\r\n\t\t\t");
}	
			this.Write("\t\t\t");
foreach(var item in viewModelData.Views){
					string nameForItem = "navigationBarItem" + item.ViewName;
					string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = new DevExpress.XtraBars.Navigation.NavigationBarItem(); \r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(" = new DevExpress.XtraBars.BarButtonItem();\r\n\t\t\t");
}	
			this.Write(@"            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.officeNavigationBar)).BeginInit();
            this.SuspendLayout();
			// 
            // ribbonControl
            // 
            this.ribbonControl.ExpandCollapseItem.Id = 0;
            this.ribbonControl.MaxItemId = 14;
            this.ribbonControl.Name = ""ribbonControl"";
			this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] {this.skinRibbonGalleryBarItem, this.barSubItemNavigation ");
if(CreateSubItemForTableAndView){
			this.Write(",this.barSubItemTables,this.barSubItemViews");
}
			this.Write("});\r\n\t\t\tthis.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPa" +
					"ge[] {\r\n            this.ribbonPage});\r\n            this.ribbonControl.RibbonSty" +
					"le = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013;\r\n            this" +
					".ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;\r\n\t" +
					"\t\tthis.ribbonControl.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeSt" +
					"yle.Always;\r\n            this.ribbonControl.ToolbarLocation = DevExpress.XtraBar" +
					"s.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;\r\n\t\t\tthis.ribbonControl.StatusB" +
					"ar = this.ribbonStatusBar;\r\n            // \r\n            // ribbonPage\r\n        " +
					"    // \r\n            this.ribbonPage.Groups.AddRange(new DevExpress.XtraBars.Rib" +
					"bon.RibbonPageGroup[] {\r\n\t\t\tthis.ribbonPageGroupNavigation,\r\n            this.ri" +
					"bbonPageGroup});\r\n\t\t\tthis.ribbonPage.MergeOrder = -1;\r\n            this.ribbonPa" +
					"ge.Name = \"ribbonPage\";\r\n            this.ribbonPage.Text = \"View\";\r\n\t\t\t// \r\n   " +
					"         // ribbonPageGroupNavigation\r\n            // \r\n            this.ribbonP" +
					"ageGroupNavigation.ItemLinks.Add(this.barSubItemNavigation);\r\n            this.r" +
					"ibbonPageGroupNavigation.Name = \"ribbonPageGroupNavigation\";\r\n            this.r" +
					"ibbonPageGroupNavigation.Text = \"Module\";\r\n\t\t\t // \r\n            // barSubItemNav" +
					"igation\r\n            // \r\n            this.barSubItemNavigation.Caption = \"Navig" +
					"ation\";\r\n            this.barSubItemNavigation.ImageUri.Uri = \"NavigationBar\";\r\n" +
					"            this.barSubItemNavigation.Name = \"barSubItemNavigation\";\r\n\t\t\t");
if(CreateSubItemForTableAndView){
			this.Write(@"			// 
            // barSubItemTables
            // 
            this.barSubItemTables.Caption = ""Tables"";
            this.barSubItemTables.Name = ""barSubItemTables"";
			// 
            // barSubItemViews
            // 
            this.barSubItemViews.Caption = ""Views"";
            this.barSubItemViews.Name = ""barSubItemViews"";
			this.barSubItemNavigation.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemTables));
			this.barSubItemNavigation.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemViews));
			");
}
			this.Write(@"            // 
            // ribbonPageGroup
            // 
            this.ribbonPageGroup.AllowTextClipping = false;
			this.ribbonPageGroup.ItemLinks.Add(this.skinRibbonGalleryBarItem);
            this.ribbonPageGroup.Name = ""ribbonPageGroup"";
            this.ribbonPageGroup.ShowCaptionButton = false;
            this.ribbonPageGroup.Text = ""Appearance"";
			// 
            // ribbonStatusBar
            // 
			this.ribbonStatusBar.Name = ""ribbonStatusBar"";
            this.ribbonStatusBar.Ribbon = this.ribbonControl;
			// 
            // officeNavigationBar
            //
			this.officeNavigationBar.Dock = System.Windows.Forms.DockStyle.Bottom; 
            this.officeNavigationBar.Items.AddRange(new DevExpress.XtraBars.Navigation.NavigationBarItem[] {
            ");
foreach(var item in viewModelData.Tables){
				string nameForItem = "navigationBarItem" + item.ViewName;
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(",\r\n\t\t\t");
}	
			this.Write("\t\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "navigationBarItem" + item.ViewName;
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(",\r\n\t\t\t");
}	
			this.Write("\t\t\t\r\n\t\t\t});\r\n            this.officeNavigationBar.Name = \"officeNavigationBar\";\r\n" +
					"            this.officeNavigationBar.Text = \"officeNavigationBar\";\r\n\t\t\t");
foreach(var item in viewModelData.Tables){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\t\t// \r\n            // ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n            // \r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\";\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\";\r\n\t\t\t// \r\n            // ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("\r\n            // \r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("\";\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(".Caption = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\";\r\n\t\t\t\t");
if(CreateSubItemForTableAndView){
			this.Write("\t\t\tthis.barSubItemTables.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersist" +
					"Info(this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("));\r\n\t\t\t\t");
}	
			this.Write("\t\t\t\t");
if(!CreateSubItemForTableAndView){
			this.Write("\t\t\tthis.barSubItemNavigation.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPer" +
					"sistInfo(this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("));\r\n\t\t\t\t");
}	
			this.Write("\t\r\n\t\t\t");
}	
			this.Write("\t\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\t\t// \r\n            // ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n            // \r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\";\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\";\r\n\t\t\t// \r\n            // ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("\r\n            // \r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("\";\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(".Caption = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\";\r\n\t\t\t");
if(CreateSubItemForTableAndView){
			this.Write("\t\t\tthis.barSubItemViews.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistI" +
					"nfo(this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("));\r\n\t\t\t");
}
			this.Write("\t\t\t");
if(!CreateSubItemForTableAndView){
			this.Write("\t\t\tthis.barSubItemNavigation.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPer" +
					"sistInfo(this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("));\r\n\t\t\t");
}	
			this.Write("\t\t\t");
}	
			this.Write("\t\r\n\t\t\t// \r\n            // mvvmContext\r\n            // \r\n            this.mvvmCont" +
					"ext.ContainerControl = this;\r\n            this.mvvmContext.ViewModelType = typeo" +
					"f(");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(@");
			 // 
            // navigationFrame
            // 
            this.navigationFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationFrame.Name = ""navigationFrame"";
            this.navigationFrame.RibbonAndBarsMergeStyle = DevExpress.XtraBars.Docking2010.Views.RibbonAndBarsMergeStyle.Always;
            this.navigationFrame.Text = ""navigationFrame"";
			// 
            // ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
            // 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.navigationFrame);
            this.Controls.Add(this.officeNavigationBar);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbonControl);
			this.Size = new System.Drawing.Size(1024, 768);
            this.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@""";
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.officeNavigationBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
		}
		
        #endregion

		private DevExpress.XtraBars.Docking2010.DocumentManager documentManager;
        private DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView tabbedView;
		private DevExpress.Utils.MVVM.MVVMContext mvvmContext;
		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup;
		private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroupNavigation;
		private DevExpress.XtraBars.BarSubItem barSubItemNavigation;
		private DevExpress.XtraBars.SkinRibbonGalleryBarItem skinRibbonGalleryBarItem;
		private DevExpress.XtraBars.Navigation.OfficeNavigationBar officeNavigationBar;
        private DevExpress.XtraBars.Navigation.NavigationFrame navigationFrame; 
		");
if(CreateSubItemForTableAndView){
			this.Write("\t\tprivate DevExpress.XtraBars.BarSubItem barSubItemTables;\r\n\t\tprivate DevExpress." +
					"XtraBars.BarSubItem barSubItemViews;\r\n\t\t");
}
			this.Write("\t\t");
foreach(var item in viewModelData.Tables){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\tprivate DevExpress.XtraBars.Navigation.NavigationBarItem ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(";\r\n\t\tprivate DevExpress.XtraBars.BarButtonItem  ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(";\r\n\t\t");
}	
			this.Write("\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\tprivate DevExpress.XtraBars.Navigation.NavigationBarItem ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(";\r\n\t\tprivate DevExpress.XtraBars.BarButtonItem  ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(";\r\n\t\t");
}	
			this.Write("\t}\r\n}\r\n");
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
					" code\"\r\n\r\n\t\t\'\'\' <summary> \r\n\t\t\'\'\' Required method for Designer support - do not " +
					"modify \r\n\t\t\'\'\' the contents of this method with the code editor.\r\n\t\t\'\'\' </summar" +
					"y>\r\n\t\tPrivate Sub InitializeComponent()\r\n\t\t\tMe.components = New System.Component" +
					"Model.Container()\r\n\t\t\tMe.documentManager = New DevExpress.XtraBars.Docking2010.D" +
					"ocumentManager()\r\n\t\t\tMe.tabbedView = New DevExpress.XtraBars.Docking2010.Views.T" +
					"abbed.TabbedView()\r\n\t\t\tMe.mvvmContext = New DevExpress.Utils.MVVM.MVVMContext(Me" +
					".components)\r\n\t\t\tMe.ribbonControl = New DevExpress.XtraBars.Ribbon.RibbonControl" +
					"()\r\n\t\t\tMe.ribbonPage = New DevExpress.XtraBars.Ribbon.RibbonPage()\r\n\t\t\tMe.ribbon" +
					"PageGroup = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()\r\n\t\t\tMe.ribbonStatus" +
					"Bar = New DevExpress.XtraBars.Ribbon.RibbonStatusBar()\r\n\t\t\tMe.ribbonPageGroupNav" +
					"igation = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()\r\n\t\t\tMe.barSubItemNavi" +
					"gation = New DevExpress.XtraBars.BarSubItem()\r\n\t\t\tMe.skinRibbonGalleryBarItem = " +
					"New DevExpress.XtraBars.SkinRibbonGalleryBarItem()\r\n\t\t\tMe.officeNavigationBar = " +
					"New DevExpress.XtraBars.Navigation.OfficeNavigationBar()\r\n\t\t\tMe.navigationFrame " +
					"= New DevExpress.XtraBars.Navigation.NavigationFrame()\r\n\t\t\t");
if(CreateSubItemForTableAndView){
			this.Write("\t\t\tMe.barSubItemTables = New DevExpress.XtraBars.BarSubItem()\r\n\t\t\tMe.barSubItemVi" +
					"ews = New DevExpress.XtraBars.BarSubItem()\r\n\t\t\t");
}
			this.Write("\t\t\t");
foreach(var item in viewModelData.Tables){
					string nameForItem = "navigationBarItem" + item.ViewName;
					string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = New DevExpress.XtraBars.Navigation.NavigationBarItem()\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(" = New DevExpress.XtraBars.BarButtonItem()\r\n\t\t\t");
}	
			this.Write("\t\t\t");
foreach(var item in viewModelData.Views){
					string nameForItem = "navigationBarItem" + item.ViewName;
					string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = New DevExpress.XtraBars.Navigation.NavigationBarItem()\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(" = New DevExpress.XtraBars.BarButtonItem()\r\n\t\t\t");
}	
			this.Write(@"			CType(Me.mvvmContext, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me.ribbonControl, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me.officeNavigationBar, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			' 
			' ribbonControl
			' 
			Me.ribbonControl.ExpandCollapseItem.Id = 0
			Me.ribbonControl.MaxItemId = 14
			Me.ribbonControl.Name = ""ribbonControl""
			Me.ribbonControl.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.skinRibbonGalleryBarItem, Me.barSubItemNavigation");
if(CreateSubItemForTableAndView){	
			this.Write(",Me.barSubItemTables,Me.barSubItemViews");
}
			this.Write(@"})
			Me.ribbonControl.Pages.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPage() {Me.ribbonPage})
			Me.ribbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013
			Me.ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False
			Me.ribbonControl.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeStyle.Always
			Me.ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden
			Me.ribbonControl.StatusBar = Me.ribbonStatusBar
			' 
			' ribbonPage
			' 
			Me.ribbonPage.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() { Me.ribbonPageGroupNavigation, Me.ribbonPageGroup})
			Me.ribbonPage.MergeOrder = -1
			Me.ribbonPage.Name = ""ribbonPage""
			Me.ribbonPage.Text = ""View""
			' 
			' ribbonPageGroupNavigation
			' 
			Me.ribbonPageGroupNavigation.ItemLinks.Add(Me.barSubItemNavigation)
			Me.ribbonPageGroupNavigation.Name = ""ribbonPageGroupNavigation""
			Me.ribbonPageGroupNavigation.Text = ""Module""
			' 
			' barSubItemNavigation
			' 
			Me.barSubItemNavigation.Caption = ""Navigation""
			Me.barSubItemNavigation.ImageUri.Uri = ""NavigationBar""
			Me.barSubItemNavigation.Name = ""barSubItemNavigation""
			");
if(CreateSubItemForTableAndView){
			this.Write(@"			' 
			' barSubItemTables
			' 
			Me.barSubItemTables.Caption = ""Tables""
			Me.barSubItemTables.Name = ""barSubItemTables""
			' 
			' barSubItemViews
			' 
			Me.barSubItemViews.Caption = ""Views""
			Me.barSubItemViews.Name = ""barSubItemViews""
			Me.barSubItemNavigation.LinksPersistInfo.Add(New DevExpress.XtraBars.LinkPersistInfo(Me.barSubItemTables))
			Me.barSubItemNavigation.LinksPersistInfo.Add(New DevExpress.XtraBars.LinkPersistInfo(Me.barSubItemViews))
			");
}
			this.Write(@"			' 
			' ribbonPageGroup
			' 
			Me.ribbonPageGroup.AllowTextClipping = False
			Me.ribbonPageGroup.ItemLinks.Add(Me.skinRibbonGalleryBarItem)
			Me.ribbonPageGroup.Name = ""ribbonPageGroup""
			Me.ribbonPageGroup.ShowCaptionButton = False
			Me.ribbonPageGroup.Text = ""Appearance""
			' 
			' ribbonStatusBar
			' 
			Me.ribbonStatusBar.Name = ""ribbonStatusBar""
			Me.ribbonStatusBar.Ribbon = Me.ribbonControl
			' 
			' officeNavigationBar
			'
			Me.officeNavigationBar.Dock = System.Windows.Forms.DockStyle.Bottom
            ");
foreach(var item in viewModelData.Tables){
				string nameForItem = "navigationBarItem" + item.ViewName;
			this.Write("\t\t\tMe.officeNavigationBar.Items.Add(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(")\r\n\t\t\t");
}	
			this.Write("\t\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "navigationBarItem" + item.ViewName;
			this.Write("\t\t\tMe.officeNavigationBar.Items.Add(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(")\r\n\t\t\t");
}
			this.Write("\t\t\tMe.officeNavigationBar.Name = \"officeNavigationBar\"\r\n\t\t\tMe.officeNavigationBar" +
					".Text = \"officeNavigationBar\"\r\n\t\t\t");
foreach(var item in viewModelData.Tables){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\t\t\' \r\n            \' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n            \' \r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\"\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\"\r\n\t\t\t\' \r\n            \' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("\r\n            \' \r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("\"\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(".Caption = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\"\r\n\t\t\t\t");
if(CreateSubItemForTableAndView){
			this.Write("\t\t\tMe.barSubItemTables.LinksPersistInfo.Add(New DevExpress.XtraBars.LinkPersistIn" +
					"fo(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("))\r\n\t\t\t\t");
}	
			this.Write("\t\t\t\t");
if(!CreateSubItemForTableAndView){
			this.Write("\t\t\tMe.barSubItemNavigation.LinksPersistInfo.Add(New DevExpress.XtraBars.LinkPersi" +
					"stInfo(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("))\r\n\t\t\t\t");
}	
			this.Write("\t\r\n\t\t\t");
}	
			this.Write("\t\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\t\t\' \r\n            \' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n            \' \r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\"\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\"\r\n\t\t\t\' \r\n            \' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("\r\n            \' \r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(".Name = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("\"\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(".Caption = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\"\r\n\t\t\t\t");
if(CreateSubItemForTableAndView){
			this.Write("\t\t\tMe.barSubItemViews.LinksPersistInfo.Add(New DevExpress.XtraBars.LinkPersistInf" +
					"o(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("))\r\n\t\t\t\t");
}	
			this.Write("\t\t\t\t");
if(!CreateSubItemForTableAndView){
			this.Write("\t\t\tMe.barSubItemNavigation.LinksPersistInfo.Add(New DevExpress.XtraBars.LinkPersi" +
					"stInfo(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write("))\r\n\t\t\t\t");
}	
			this.Write("\t\r\n\t\t\t");
}	
			this.Write("\t\t\t\' \r\n\t\t\t\' mvvmContext\r\n\t\t\t\' \r\n\t\t\tMe.mvvmContext.ContainerControl = Me\r\n\t\t\tMe.mv" +
					"vmContext.ViewModelType = GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(@")
			' 
			' navigationFrame
			' 
			Me.navigationFrame.Dock = System.Windows.Forms.DockStyle.Fill
			Me.navigationFrame.Name = ""navigationFrame""
			Me.navigationFrame.RibbonAndBarsMergeStyle = DevExpress.XtraBars.Docking2010.Views.RibbonAndBarsMergeStyle.Always
			Me.navigationFrame.Text = ""navigationFrame""
			' 
            ' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
            '
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.Controls.Add(Me.navigationFrame)
			Me.Controls.Add(Me.officeNavigationBar)
			Me.Controls.Add(Me.ribbonStatusBar)
			Me.Controls.Add(Me.ribbonControl)
			Me.Size = New System.Drawing.Size(1024, 768) 
			Me.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"""
			CType(Me.mvvmContext, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.ribbonControl, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.officeNavigationBar, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)
			Me.PerformLayout()
		End Sub

		#End Region

		Private documentManager As DevExpress.XtraBars.Docking2010.DocumentManager
		Private tabbedView As DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView
		Private mvvmContext As DevExpress.Utils.MVVM.MVVMContext
		Private ribbonControl As DevExpress.XtraBars.Ribbon.RibbonControl
		Private ribbonPage As DevExpress.XtraBars.Ribbon.RibbonPage
		Private ribbonPageGroup As DevExpress.XtraBars.Ribbon.RibbonPageGroup
		Private ribbonStatusBar As DevExpress.XtraBars.Ribbon.RibbonStatusBar
		Private ribbonPageGroupNavigation As DevExpress.XtraBars.Ribbon.RibbonPageGroup
		Private barSubItemNavigation As DevExpress.XtraBars.BarSubItem
		Private skinRibbonGalleryBarItem As DevExpress.XtraBars.SkinRibbonGalleryBarItem
		Private officeNavigationBar As DevExpress.XtraBars.Navigation.OfficeNavigationBar
		Private navigationFrame As DevExpress.XtraBars.Navigation.NavigationFrame
		");
if(CreateSubItemForTableAndView){
			this.Write("\t\tPrivate barSubItemTables As DevExpress.XtraBars.BarSubItem\r\n\t\tPrivate barSubIte" +
					"mViews As DevExpress.XtraBars.BarSubItem\r\n\t\t");
}
			this.Write("\t\t");
foreach(var item in viewModelData.Tables){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" As DevExpress.XtraBars.Navigation.NavigationBarItem\r\n\t\tPrivate  ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(" As DevExpress.XtraBars.BarButtonItem\r\n\t\t");
}	
			this.Write("\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" As DevExpress.XtraBars.Navigation.NavigationBarItem\r\n\t\tPrivate  ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(" As DevExpress.XtraBars.BarButtonItem\r\n\t\t");
}	
			this.Write("\tEnd Class\r\nEnd Namespace\r\n");
}
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentManagerView_OutlookDesignerBase
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
