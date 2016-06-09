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
	using System;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class DocumentManagerView_TabbedMDIDesigner : DocumentManagerView_TabbedMDIDesignerBase
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
					".Ribbon.RibbonStatusBar();\r\n\t\t\tthis.skinRibbonGalleryBarItem = new DevExpress.Xt" +
					"raBars.SkinRibbonGalleryBarItem();\r\n\t\t\tthis.dockManager = new DevExpress.XtraBar" +
					"s.Docking.DockManager(this.components);\r\n            this.dockPanel = new DevExp" +
					"ress.XtraBars.Docking.DockPanel();\r\n            this.dockPanel_Container = new D" +
					"evExpress.XtraBars.Docking.ControlContainer();\r\n            this.accordionContro" +
					"l = new DevExpress.XtraBars.Navigation.AccordionControl();\r\n\t\t\tthis.accordionIte" +
					"mTables = new DevExpress.XtraBars.Navigation.AccordionControlElement();\r\n\t\t\tthis" +
					".accordionItemViews = new DevExpress.XtraBars.Navigation.AccordionControlElement" +
					"(); \r\n\t\t\t");
foreach(var item in viewModelData.Tables){
					string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = new DevExpress.XtraBars.Navigation.AccordionControlElement();\r\n\t\t\t");
}	
			this.Write("\t\t\t");
foreach(var item in viewModelData.Views){
					string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = new DevExpress.XtraBars.Navigation.AccordionControlElement(); \r\n\t\t\t");
}	
			this.Write("\t\t\t((System.ComponentModel.ISupportInitialize)(this.documentManager)).BeginInit()" +
					";\r\n            ((System.ComponentModel.ISupportInitialize)(this.tabbedView)).Beg" +
					"inInit();\r\n            ((System.ComponentModel.ISupportInitialize)(this.mvvmCont" +
					"ext)).BeginInit();\r\n            ((System.ComponentModel.ISupportInitialize)(this" +
					".ribbonControl)).BeginInit();\r\n            ((System.ComponentModel.ISupportIniti" +
					"alize)(this.dockManager)).BeginInit();\r\n            this.dockPanel.SuspendLayout" +
					"();\r\n            this.dockPanel_Container.SuspendLayout();\r\n            ((System" +
					".ComponentModel.ISupportInitialize)(this.accordionControl)).BeginInit();\r\n      " +
					"      this.SuspendLayout();\r\n\t\t\t// \r\n            // ribbonControl\r\n            /" +
					"/ \r\n            this.ribbonControl.ExpandCollapseItem.Id = 0;\r\n            this." +
					"ribbonControl.MaxItemId = 14;\r\n            this.ribbonControl.Name = \"ribbonCont" +
					"rol\";\r\n\t\t\tthis.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] {t" +
					"his.skinRibbonGalleryBarItem});\r\n\t\t\tthis.ribbonControl.Pages.AddRange(new DevExp" +
					"ress.XtraBars.Ribbon.RibbonPage[] {\r\n            this.ribbonPage});\r\n           " +
					" this.ribbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle." +
					"Office2013;\r\n            this.ribbonControl.ShowApplicationButton = DevExpress.U" +
					"tils.DefaultBoolean.False;\r\n\t\t\tthis.ribbonControl.MdiMergeStyle = DevExpress.Xtr" +
					"aBars.Ribbon.RibbonMdiMergeStyle.Always;\r\n            this.ribbonControl.Toolbar" +
					"Location = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;\r\n" +
					"\t\t\tthis.ribbonControl.StatusBar = this.ribbonStatusBar;\r\n            // \r\n      " +
					"      // ribbonPage\r\n            // \r\n            this.ribbonPage.Groups.AddRang" +
					"e(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {\r\n            this.ribbonPag" +
					"eGroup});\r\n\t\t\tthis.ribbonPage.MergeOrder = -1;\r\n            this.ribbonPage.Name" +
					" = \"ribbonPage\";\r\n            this.ribbonPage.Text = \"View\";\r\n            // \r\n " +
					"           // ribbonPageGroup\r\n            // \r\n            this.ribbonPageGroup" +
					".AllowTextClipping = false;\r\n\t\t\tthis.ribbonPageGroup.ItemLinks.Add(this.skinRibb" +
					"onGalleryBarItem);\r\n            this.ribbonPageGroup.Name = \"ribbonPageGroup\";\r\n" +
					"            this.ribbonPageGroup.ShowCaptionButton = false;\r\n            this.ri" +
					"bbonPageGroup.Text = \"Appearance\";\r\n\t\t\t// \r\n            // ribbonStatusBar\r\n    " +
					"        // \r\n\t\t\tthis.ribbonStatusBar.Name = \"ribbonStatusBar\";\r\n            this" +
					".ribbonStatusBar.Ribbon = this.ribbonControl;\r\n\t\t\t// \r\n            // documentMa" +
					"nager\r\n            // \r\n            this.documentManager.ContainerControl = this" +
					";\r\n            this.documentManager.RibbonAndBarsMergeStyle = DevExpress.XtraBar" +
					"s.Docking2010.Views.RibbonAndBarsMergeStyle.Always;\r\n            this.documentMa" +
					"nager.View = this.tabbedView;\r\n            this.documentManager.ViewCollection.A" +
					"ddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[] {\r\n            this" +
					".tabbedView});\r\n\t\t\t// \r\n            // dockManager\r\n            // \r\n           " +
					" this.dockManager.Form = this;\r\n            this.dockManager.RootPanels.AddRange" +
					"(new DevExpress.XtraBars.Docking.DockPanel[] {\r\n            this.dockPanel});\r\n\t" +
					"\t\tthis.dockManager.TopZIndexControls.AddRange(new string[] {\r\n            \"DevEx" +
					"press.XtraBars.BarDockControl\",\r\n            \"DevExpress.XtraBars.StandaloneBarD" +
					"ockControl\",\r\n            \"System.Windows.Forms.StatusBar\",\r\n            \"System" +
					".Windows.Forms.MenuStrip\",\r\n            \"System.Windows.Forms.StatusStrip\",\r\n   " +
					"         \"DevExpress.XtraBars.Ribbon.RibbonStatusBar\",\r\n            \"DevExpress." +
					"XtraBars.Ribbon.RibbonControl\",\r\n            \"DevExpress.XtraBars.Navigation.Off" +
					"iceNavigationBar\",\r\n            \"DevExpress.XtraBars.Navigation.TileNavPane\"});\r" +
					"\n\t\t\t// \r\n            // dockPanel\r\n            // \r\n            this.dockPanel.C" +
					"ontrols.Add(this.dockPanel_Container);\r\n            this.dockPanel.Dock = DevExp" +
					"ress.XtraBars.Docking.DockingStyle.Left;\r\n            this.dockPanel.Name = \"doc" +
					"kPanel\";\r\n            this.dockPanel.OriginalSize = new System.Drawing.Size(200," +
					" 200);\r\n            this.dockPanel.Text = \"Navigation\";\r\n\t\t\t// \r\n            // " +
					"dockPanel_Container\r\n            // \r\n            this.dockPanel_Container.Contr" +
					"ols.Add(this.accordionControl);\r\n            this.dockPanel_Container.Name = \"do" +
					"ckPanel_Container\";\r\n\t\t\t// \r\n            // accordionControl\r\n            // \r\n " +
					"           this.accordionControl.Dock = System.Windows.Forms.DockStyle.Fill;\r\n  " +
					"          this.accordionControl.Elements.AddRange(new DevExpress.XtraBars.Naviga" +
					"tion.AccordionControlElement[] {\r\n            this.accordionItemTables,\r\n       " +
					"     this.accordionItemViews});\r\n            this.accordionControl.Name = \"accor" +
					"dionControl\";\r\n            this.accordionControl.TabIndex = 0;\r\n            this" +
					".accordionControl.Text = \"accordionControl\";\r\n\t\t\t// \r\n            // accordionIt" +
					"emTables\r\n            // \r\n            this.accordionItemTables.Elements.AddRang" +
					"e(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {\r\n            ");
foreach(var item in viewModelData.Tables){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(",\r\n\t\t\t");
}	
			this.Write("\t\t\t\r\n\t\t\t});\r\n            this.accordionItemTables.Expanded = true;\r\n            t" +
					"his.accordionItemTables.Text = \"Tables\";\r\n\t\t\t\r\n\t\t\t");
foreach(var item in viewModelData.Tables){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\t// \r\n            // ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n            // \r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\";\r\n\t\t\t");
}	
			this.Write("\t\t\r\n\t\t\t// \r\n            // accordionItemViews\r\n            // \r\n            this." +
					"accordionItemViews.Elements.AddRange(new DevExpress.XtraBars.Navigation.Accordio" +
					"nControlElement[] {\r\n             ");
foreach(var item in viewModelData.Views){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(",\r\n\t\t\t");
}	
			this.Write("\t\t\t\r\n\t\t\t});\r\n            this.accordionItemViews.Expanded = true;\r\n            th" +
					"is.accordionItemViews.Text = \"Views\";\r\n\t\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\t// \r\n            // ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n            // \r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\";\r\n\t\t\t");
}	
			this.Write("\t\r\n\t\t\t// \r\n            // mvvmContext\r\n            // \r\n            this.mvvmCont" +
					"ext.ContainerControl = this;\r\n            this.mvvmContext.ViewModelType = typeo" +
					"f(");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(");\r\n\t\t\t// \r\n            // ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
            // 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbonControl);
			this.Size = new System.Drawing.Size(1024, 768);
            this.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("\";\r\n\t\t\t((System.ComponentModel.ISupportInitialize)(this.documentManager)).EndInit" +
					"();\r\n            ((System.ComponentModel.ISupportInitialize)(this.tabbedView)).E" +
					"ndInit();\r\n            ((System.ComponentModel.ISupportInitialize)(this.mvvmCont" +
					"ext)).EndInit();\r\n            ((System.ComponentModel.ISupportInitialize)(this.r" +
					"ibbonControl)).EndInit();\r\n            ((System.ComponentModel.ISupportInitializ" +
					"e)(this.dockManager)).EndInit();\r\n            this.dockPanel.ResumeLayout(false)" +
					";\r\n            this.dockPanel_Container.ResumeLayout(false);\r\n            ((Syst" +
					"em.ComponentModel.ISupportInitialize)(this.accordionControl)).EndInit();\r\n      " +
					"      this.ResumeLayout(false);\r\n            this.PerformLayout();\r\n\t\t}\r\n\t\t\r\n   " +
					"     #endregion\r\n\r\n\t\tprivate DevExpress.XtraBars.Docking2010.DocumentManager doc" +
					"umentManager;\r\n        private DevExpress.XtraBars.Docking2010.Views.Tabbed.Tabb" +
					"edView tabbedView;\r\n\t\tprivate DevExpress.Utils.MVVM.MVVMContext mvvmContext;\r\n\t\t" +
					"private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl;\r\n\t\tprivate DevEx" +
					"press.XtraBars.Ribbon.RibbonPage ribbonPage;\r\n        private DevExpress.XtraBar" +
					"s.Ribbon.RibbonPageGroup ribbonPageGroup;\r\n\t\tprivate DevExpress.XtraBars.Ribbon." +
					"RibbonStatusBar ribbonStatusBar;\r\n\t\tprivate DevExpress.XtraBars.SkinRibbonGaller" +
					"yBarItem skinRibbonGalleryBarItem;\r\n\t\tprivate DevExpress.XtraBars.Docking.DockMa" +
					"nager dockManager;\r\n\t\tprivate DevExpress.XtraBars.Docking.DockPanel dockPanel;\r\n" +
					"        private DevExpress.XtraBars.Docking.ControlContainer dockPanel_Container" +
					";\r\n        private DevExpress.XtraBars.Navigation.AccordionControl accordionCont" +
					"rol;\r\n\t\tprivate DevExpress.XtraBars.Navigation.AccordionControlElement accordion" +
					"ItemTables; \r\n\t\tprivate DevExpress.XtraBars.Navigation.AccordionControlElement a" +
					"ccordionItemViews; \r\n\r\n\t\t");
foreach(var item in viewModelData.Tables){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\tprivate DevExpress.XtraBars.Navigation.AccordionControlElement ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(";\r\n\t\t");
}	
			this.Write("\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\tprivate DevExpress.XtraBars.Navigation.AccordionControlElement ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
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
					"ocumentManager()\r\n            Me.tabbedView = New DevExpress.XtraBars.Docking201" +
					"0.Views.Tabbed.TabbedView()\r\n\t\t\tMe.mvvmContext = New DevExpress.Utils.MVVM.MVVMC" +
					"ontext(Me.components)\r\n\t\t\tMe.ribbonControl = New DevExpress.XtraBars.Ribbon.Ribb" +
					"onControl()\r\n\t\t\tMe.ribbonPage = New DevExpress.XtraBars.Ribbon.RibbonPage()\r\n   " +
					"         Me.ribbonPageGroup = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()\r\n" +
					"\t\t\tMe.ribbonStatusBar = New DevExpress.XtraBars.Ribbon.RibbonStatusBar()\r\n\t\t\tMe." +
					"skinRibbonGalleryBarItem = New DevExpress.XtraBars.SkinRibbonGalleryBarItem()\r\n\t" +
					"\t\tMe.dockManager = New DevExpress.XtraBars.Docking.DockManager(Me.components)\r\n " +
					"           Me.dockPanel = New DevExpress.XtraBars.Docking.DockPanel()\r\n         " +
					"   Me.dockPanel_Container = New DevExpress.XtraBars.Docking.ControlContainer()\r\n" +
					"            Me.accordionControl = New DevExpress.XtraBars.Navigation.AccordionCo" +
					"ntrol()\r\n\t\t\tMe.accordionItemTables = New DevExpress.XtraBars.Navigation.Accordio" +
					"nControlElement()\r\n\t\t\tMe.accordionItemViews = New DevExpress.XtraBars.Navigation" +
					".AccordionControlElement()\r\n\t\t\t");
foreach(var item in viewModelData.Tables){
					string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = New DevExpress.XtraBars.Navigation.AccordionControlElement()\r\n\t\t\t");
}	
			this.Write("\t\t\t");
foreach(var item in viewModelData.Views){
					string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = New DevExpress.XtraBars.Navigation.AccordionControlElement() \r\n\t\t\t");
}
			this.Write(" \r\n\t\t\tCType(Me.documentManager, System.ComponentModel.ISupportInitialize).BeginIn" +
					"it()\r\n\t\t\tCType(Me.tabbedView, System.ComponentModel.ISupportInitialize).BeginIni" +
					"t()\r\n\t\t\tCType(Me.mvvmContext, System.ComponentModel.ISupportInitialize).BeginIni" +
					"t()\r\n\t\t\tCType(Me.ribbonControl, System.ComponentModel.ISupportInitialize).BeginI" +
					"nit()\r\n\t\t\tCType(Me.dockManager, System.ComponentModel.ISupportInitialize).BeginI" +
					"nit()\r\n\t\t\tMe.dockPanel.SuspendLayout()\r\n            Me.dockPanel_Container.Suspe" +
					"ndLayout()\r\n\t\t\tCType(Me.accordionControl, System.ComponentModel.ISupportInitiali" +
					"ze).BeginInit()\r\n\t\t\tMe.SuspendLayout()\r\n\t\t\t\' \r\n            \' ribbonControl\r\n    " +
					"        \' \r\n            Me.ribbonControl.ExpandCollapseItem.Id = 0\r\n            " +
					"Me.ribbonControl.MaxItemId = 14\r\n            Me.ribbonControl.Name = \"ribbonCont" +
					"rol\"\r\n\t\t\tMe.ribbonControl.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.s" +
					"kinRibbonGalleryBarItem})\r\n\t\t\tMe.ribbonControl.Pages.AddRange(New DevExpress.Xtr" +
					"aBars.Ribbon.RibbonPage() {\r\n            Me.ribbonPage})\r\n            Me.ribbonC" +
					"ontrol.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013\r\n " +
					"           Me.ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBool" +
					"ean.False\r\n\t\t\tMe.ribbonControl.MdiMergeStyle = DevExpress.XtraBars.Ribbon.Ribbon" +
					"MdiMergeStyle.Always\r\n            Me.ribbonControl.ToolbarLocation = DevExpress." +
					"XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden\r\n\t\t\tMe.ribbonControl.Sta" +
					"tusBar = Me.ribbonStatusBar\r\n            \' \r\n            \' ribbonPage\r\n         " +
					"   \' \r\n            Me.ribbonPage.Groups.AddRange(New DevExpress.XtraBars.Ribbon." +
					"RibbonPageGroup() {\r\n            Me.ribbonPageGroup})\r\n\t\t\tMe.ribbonPage.MergeOrd" +
					"er = -1\r\n            Me.ribbonPage.Name = \"ribbonPage\"\r\n            Me.ribbonPag" +
					"e.Text = \"View\"\r\n            \' \r\n            \' ribbonPageGroup\r\n            \' \r\n" +
					"            Me.ribbonPageGroup.AllowTextClipping = false\r\n\t\t\tMe.ribbonPageGroup." +
					"ItemLinks.Add(Me.skinRibbonGalleryBarItem)\r\n            Me.ribbonPageGroup.Name " +
					"= \"ribbonPageGroup\"\r\n            Me.ribbonPageGroup.ShowCaptionButton = false\r\n " +
					"           Me.ribbonPageGroup.Text = \"Appearance\"\r\n\t\t\t\' \r\n            \' ribbonSt" +
					"atusBar\r\n            \' \r\n\t\t\tMe.ribbonStatusBar.Name = \"ribbonStatusBar\"\r\n       " +
					"     Me.ribbonStatusBar.Ribbon = Me.ribbonControl\r\n\t\t\t\' \r\n            \' document" +
					"Manager\r\n            \' \r\n            Me.documentManager.ContainerControl = Me\r\n " +
					"           Me.documentManager.RibbonAndBarsMergeStyle = DevExpress.XtraBars.Dock" +
					"ing2010.Views.RibbonAndBarsMergeStyle.Always\r\n            Me.documentManager.Vie" +
					"w = Me.tabbedView\r\n            Me.documentManager.ViewCollection.AddRange(New De" +
					"vExpress.XtraBars.Docking2010.Views.BaseView() {\r\n            Me.tabbedView})\r\n\t" +
					"\t\t\' \r\n            \' dockManager\r\n            \' \r\n            Me.dockManager.Form" +
					" = Me\r\n            Me.dockManager.RootPanels.AddRange(New DevExpress.XtraBars.Do" +
					"cking.DockPanel() {\r\n            Me.dockPanel})\r\n\t\t\tMe.dockManager.TopZIndexCont" +
					"rols.AddRange(New String() {\r\n            \"DevExpress.XtraBars.BarDockControl\",\r" +
					"\n            \"DevExpress.XtraBars.StandaloneBarDockControl\",\r\n            \"Syste" +
					"m.Windows.Forms.StatusBar\",\r\n            \"System.Windows.Forms.MenuStrip\",\r\n    " +
					"        \"System.Windows.Forms.StatusStrip\",\r\n            \"DevExpress.XtraBars.Ri" +
					"bbon.RibbonStatusBar\",\r\n            \"DevExpress.XtraBars.Ribbon.RibbonControl\",\r" +
					"\n            \"DevExpress.XtraBars.Navigation.OfficeNavigationBar\",\r\n            " +
					"\"DevExpress.XtraBars.Navigation.TileNavPane\"})\r\n\t\t\t\' \r\n            \' dockPanel\r\n" +
					"            \' \r\n            Me.dockPanel.Controls.Add(Me.dockPanel_Container)\r\n " +
					"           Me.dockPanel.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left\r\n  " +
					"          Me.dockPanel.Name = \"dockPanel\"\r\n            Me.dockPanel.OriginalSize" +
					" = New System.Drawing.Size(200, 200)\r\n            Me.dockPanel.Text = \"Navigatio" +
					"n\"\r\n\t\t\t\' \r\n            \' dockPanel_Container\r\n            \' \r\n            Me.doc" +
					"kPanel_Container.Controls.Add(Me.accordionControl)\r\n            Me.dockPanel_Con" +
					"tainer.Name = \"dockPanel_Container\"\r\n\t\t\t\' \r\n            \' accordionControl\r\n    " +
					"        \' \r\n            Me.accordionControl.Dock = System.Windows.Forms.DockStyl" +
					"e.Fill\r\n            Me.accordionControl.Elements.AddRange(New DevExpress.XtraBar" +
					"s.Navigation.AccordionControlElement() {\r\n            Me.accordionItemTables,\r\n " +
					"           Me.accordionItemViews})\r\n            Me.accordionControl.Name = \"acco" +
					"rdionControl\"\r\n            Me.accordionControl.TabIndex = 0\r\n            Me.acco" +
					"rdionControl.Text = \"accordionControl\"\r\n\t\t\t\' \r\n            \' accordionItemTables" +
					"\r\n            \' \r\n            ");
foreach(var item in viewModelData.Tables){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\tMe.accordionItemTables.Elements.Add(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(")\r\n\t\t\t");
}	
			this.Write("\t\t\t\r\n            Me.accordionItemTables.Expanded = True\r\n            Me.accordion" +
					"ItemTables.Text = \"Tables\"\r\n\t\t\t");
foreach(var item in viewModelData.Tables){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\t\' \r\n            \' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n            \' \r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Style = DevExpress.XtraBars.Navigation.ElementStyle.Item\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\"\r\n\t\t\t");
}	
			this.Write("\t\r\n\t\t\t\' \r\n            \' accordionItemViews\r\n            \' \r\n            ");
foreach(var item in viewModelData.Views){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\tMe.accordionItemViews.Elements.Add(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(")\r\n\t\t\t");
}	
			this.Write("\t\t\t\r\n            Me.accordionItemViews.Expanded = True\r\n            Me.accordionI" +
					"temViews.Text = \"Views\"\r\n\t\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\t\t\' \r\n            \' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n            \' \r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Style = DevExpress.XtraBars.Navigation.ElementStyle.Item\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\"\r\n\t\t\t");
}	
			this.Write("\t\r\n\t\t\t\' \r\n            \' mvvmContext\r\n            \' \r\n            Me.mvvmContext.C" +
					"ontainerControl = Me\r\n            Me.mvvmContext.ViewModelType = GetType(Global." +
					"");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(")\r\n\t\t\t\' \r\n            \' ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
            ' 
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.Controls.Add(Me.dockPanel)
            Me.Controls.Add(Me.ribbonStatusBar)
            Me.Controls.Add(Me.ribbonControl)
			Me.Size = New System.Drawing.Size(1024, 768)
            Me.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("\"\r\n\t\t\tCType(Me.documentManager, System.ComponentModel.ISupportInitialize).EndInit" +
					"()\r\n\t\t\tCType(Me.tabbedView, System.ComponentModel.ISupportInitialize).EndInit()\r" +
					"\n\t\t\tCType(Me.mvvmContext, System.ComponentModel.ISupportInitialize).EndInit()\r\n\t" +
					"\t\tCType(Me.ribbonControl, System.ComponentModel.ISupportInitialize).EndInit()\r\n\t" +
					"\t\tCType(Me.dockManager, System.ComponentModel.ISupportInitialize).EndInit()\r\n   " +
					"         Me.dockPanel.ResumeLayout(false)\r\n            Me.dockPanel_Container.Re" +
					"sumeLayout(false)\r\n\t\t\tCType(Me.accordionControl, System.ComponentModel.ISupportI" +
					"nitialize).EndInit()\r\n\t\t\tMe.ResumeLayout(false)\r\n            Me.PerformLayout()\r" +
					"\n\t\tEnd Sub\r\n\r\n\t\t#End Region\r\n\t\tPrivate documentManager As DevExpress.XtraBars.Do" +
					"cking2010.DocumentManager\r\n        Private tabbedView As DevExpress.XtraBars.Doc" +
					"king2010.Views.Tabbed.TabbedView\r\n\t\tPrivate mvvmContext As DevExpress.Utils.MVVM" +
					".MVVMContext \r\n\t\tPrivate ribbonControl As DevExpress.XtraBars.Ribbon.RibbonContr" +
					"ol\r\n\t\tPrivate ribbonPage As DevExpress.XtraBars.Ribbon.RibbonPage\r\n        Priva" +
					"te ribbonPageGroup As DevExpress.XtraBars.Ribbon.RibbonPageGroup\r\n\t\tPrivate ribb" +
					"onStatusBar As DevExpress.XtraBars.Ribbon.RibbonStatusBar\r\n\t\tPrivate skinRibbonG" +
					"alleryBarItem As DevExpress.XtraBars.SkinRibbonGalleryBarItem\r\n\t\tPrivate dockMan" +
					"ager As DevExpress.XtraBars.Docking.DockManager\r\n\t\tPrivate dockPanel As DevExpre" +
					"ss.XtraBars.Docking.DockPanel\r\n        Private dockPanel_Container As DevExpress" +
					".XtraBars.Docking.ControlContainer\r\n        Private accordionControl As DevExpre" +
					"ss.XtraBars.Navigation.AccordionControl\r\n\t\tPrivate accordionItemTables As DevExp" +
					"ress.XtraBars.Navigation.AccordionControlElement\r\n\t\tPrivate accordionItemViews A" +
					"s DevExpress.XtraBars.Navigation.AccordionControlElement\r\n\r\n\t\t");
foreach(var item in viewModelData.Tables){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" As DevExpress.XtraBars.Navigation.AccordionControlElement\r\n\t\t");
}	
			this.Write("\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "accordionItem" + item.ViewName;
			this.Write("\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" As DevExpress.XtraBars.Navigation.AccordionControlElement\r\n\t\t");
}	
			this.Write("\tEnd Class\r\nEnd Namespace\r\n");
}
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentManagerView_TabbedMDIDesignerBase
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
