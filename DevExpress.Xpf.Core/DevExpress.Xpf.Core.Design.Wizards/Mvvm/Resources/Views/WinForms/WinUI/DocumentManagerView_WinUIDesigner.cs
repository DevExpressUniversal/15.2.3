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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views.WinForms.WinUI
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
	public partial class DocumentManagerView_WinUIDesigner : DocumentManagerView_WinUIDesignerBase
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
			this.Write(@" {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name=""disposing"">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
		 #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
            this.tileBar = new DevExpress.XtraBars.Navigation.TileBar();
            this.navigationFrame = new DevExpress.XtraBars.Navigation.NavigationFrame();
            this.mvvmContext = new DevExpress.Utils.MVVM.MVVMContext(this.components);
			this.tileBarGroupTables = new DevExpress.XtraBars.Navigation.TileBarGroup();
            this.tileBarGroupViews = new DevExpress.XtraBars.Navigation.TileBarGroup();
			");
foreach(var item in viewModelData.Tables){
					string nameForItem = "tileBarItem" + item.ViewName;
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = new DevExpress.XtraBars.Navigation.TileBarItem();\r\n\t\t\t");
}	
			this.Write("\t\t\t");
foreach(var item in viewModelData.Views){
					string nameForItem = "tileBarItem" + item.ViewName;
			this.Write("\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = new DevExpress.XtraBars.Navigation.TileBarItem(); \r\n\t\t\t");
}	
			this.Write("            this.SuspendLayout();\r\n\t\t\t/// \r\n            // tileBar\r\n            /" +
					"/ \r\n            this.tileBar.AllowDrag = false;\r\n\t\t\tthis.tileBar.AllowGlyphSkinn" +
					"ing = true;\r\n\t\t\tthis.tileBar.AllowSelectedItem = true;\r\n            this.tileBar" +
					".Dock = System.Windows.Forms.DockStyle.Top;\r\n            this.tileBar.DropDownOp" +
					"tions.BeakColor = System.Drawing.Color.Empty;\r\n            this.tileBar.Groups.A" +
					"dd(this.tileBarGroupTables);\r\n            this.tileBar.Groups.Add(this.tileBarGr" +
					"oupViews);\r\n\t\t\tthis.tileBar.BackColor = System.Drawing.Color.FromArgb(230, 230, " +
					"230);\r\n            this.tileBar.Location = new System.Drawing.Point(0, 0);\r\n    " +
					"        this.tileBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);\r\n   " +
					"         this.tileBar.Name = \"tileBar\";\r\n            this.tileBar.Padding = new " +
					"System.Windows.Forms.Padding(29, 11, 29, 11);\r\n            this.tileBar.ScrollMo" +
					"de = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;\r\n\t\t\tthis.tileBa" +
					"r.AppearanceGroupText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1" +
					"40)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));\r\n            this.tileB" +
					"ar.AppearanceGroupText.Options.UseForeColor = true;\r\n            this.tileBar.It" +
					"emPadding = new System.Windows.Forms.Padding(8, 6, 12, 6);\r\n            this.til" +
					"eBar.IndentBetweenGroups = 10;\r\n            this.tileBar.IndentBetweenItems = 10" +
					";\r\n            this.tileBar.DropDownButtonWidth = 30;\r\n            this.tileBar." +
					"SelectionBorderWidth = 2;\r\n            this.tileBar.WideTileWidth = 150;\r\n      " +
					"      this.tileBar.MinimumSize = new System.Drawing.Size(100, 110);\r\n\t\t\tthis.til" +
					"eBar.MaximumSize = new System.Drawing.Size(0, 110);\r\n\t\t\tthis.tileBar.SelectionBo" +
					"rderWidth = 2;\r\n            this.tileBar.Text = \"tileBar\";\r\n\t\t\t//\r\n\t\t\t//tileBarG" +
					"roupTables\r\n\t\t\t//\r\n\t\t\tthis.tileBarGroupTables.Name = \"tileBarGroupTables\";\r\n    " +
					"        this.tileBarGroupTables.Text = \"TABLES\";\r\n\t\t\t// \r\n            // tileBar" +
					"GroupViews\r\n            // \r\n            this.tileBarGroupViews.Name = \"tileBarG" +
					"roupViews\";\r\n            this.tileBarGroupViews.Text = \"VIEWS\";\r\n            ");
			int indexer = -1;
			int Red = 0;
			int Green = 0;
			int Blue = 0;
			foreach(var item in viewModelData.Tables){
				string nameForItem = "tileBarItem" + item.ViewName;
				string nameForElement = "tileItemElement" + item.ViewName;
				indexer++;
				if(indexer % 5 == 0){Red = 0; Green = 135; Blue = 156; }
				if(indexer % 5 == 1){Red = 0; Green = 0; Blue = 0; }
				if(indexer % 5 == 2){Red = 204; Green = 109; Blue = 0; }
				if(indexer % 5 == 3){Red = 0; Green = 115; Blue = 196; }
				if(indexer % 5 == 4){Red = 62; Green = 112; Blue = 56; }
			this.Write("\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n\t\t\t//\r\n\t\t\tDevExpress.XtraEditors.TileItemElement ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(" = new DevExpress.XtraEditors.TileItemElement();\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\";\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(".ImageUri.Uri = \"Cube;Size32x32;GrayScaled\";\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Elements.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(");\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Red));
			this.Write(")))), ((int)(((byte)(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Green));
			this.Write(")))), ((int)(((byte)(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Blue));
			this.Write(")))));\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".AppearanceItem.Normal.Options.UseBackColor = true;\r\n            this.tileBarGrou" +
					"pTables.Items.Add(this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(");\r\n\t\t\t");
}
			this.Write("\t\t\t\r\n\t\t\t");
foreach(var item in viewModelData.Views){
				indexer++;
				string nameForItem = "tileBarItem" + item.ViewName;
				string nameForElement = "tileItemElement" + item.ViewName;
				if(indexer % 5 == 0){Red = 0; Green = 135; Blue = 156; }
				if(indexer % 5 == 1){Red = 0; Green = 0; Blue = 0; }
				if(indexer % 5 == 2){Red = 204; Green = 109; Blue = 0; }
				if(indexer % 5 == 3){Red = 0; Green = 115; Blue = 196; }
				if(indexer % 5 == 4){Red = 62; Green = 112; Blue = 56; }
			this.Write("\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n\t\t\t//\r\n\t\t\tDevExpress.XtraEditors.TileItemElement ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(" = new DevExpress.XtraEditors.TileItemElement();\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\";\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(".ImageUri.Uri = \"Cube;Size32x32;GrayScaled\";\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Elements.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(");\r\n\t\t\tthis.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Red));
			this.Write(")))), ((int)(((byte)(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Green));
			this.Write(")))), ((int)(((byte)(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Blue));
			this.Write(")))));\r\n            this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".AppearanceItem.Normal.Options.UseBackColor = true;\r\n\t\t\tthis.tileBarGroupViews.It" +
					"ems.Add(this.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(");\r\n\t\t\t");
}
			this.Write(@"	
			// 
            // navigationFrame
            // 
            this.navigationFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationFrame.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.navigationFrame.Name = ""navigationFrame"";
            this.navigationFrame.SelectedPage = null;
            this.navigationFrame.SelectedPageIndex = -1;
            this.navigationFrame.Text = ""navigationFrame"";
			// 
            // mvvmContext
            // 
            this.mvvmContext.ContainerControl = this;
            this.mvvmContext.ViewModelType = typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(");\r\n\t\t\t// \r\n            // ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
            // 
			this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.navigationFrame);
            this.Controls.Add(this.tileBar);
			this.Size = new System.Drawing.Size(1024, 768);
            this.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("\";\r\n\t\t\tthis.Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@""";
            this.ResumeLayout(false);
		}
		
        #endregion

		private DevExpress.XtraBars.Navigation.TileBar tileBar;
        private DevExpress.XtraBars.Navigation.NavigationFrame navigationFrame;
        private DevExpress.Utils.MVVM.MVVMContext mvvmContext;
        private DevExpress.XtraBars.Navigation.TileBarGroup tileBarGroupTables;
        private DevExpress.XtraBars.Navigation.TileBarGroup tileBarGroupViews;
		");
foreach(var item in viewModelData.Tables){
				string nameForItem = "tileBarItem" + item.ViewName;
			this.Write("\t\tprivate DevExpress.XtraBars.Navigation.TileBarItem ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(";\r\n\t\t");
}	
			this.Write("\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "tileBarItem" + item.ViewName;
			this.Write("\t\tprivate DevExpress.XtraBars.Navigation.TileBarItem ");
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
			Me.tileBar = New DevExpress.XtraBars.Navigation.TileBar()
			Me.navigationFrame = New DevExpress.XtraBars.Navigation.NavigationFrame()
			Me.mvvmContext = New DevExpress.Utils.MVVM.MVVMContext(Me.components)
			Me.tileBarGroupTables = New DevExpress.XtraBars.Navigation.TileBarGroup()
			Me.tileBarGroupViews = New DevExpress.XtraBars.Navigation.TileBarGroup()
			");
foreach(var item in viewModelData.Tables){
					string nameForItem = "tileBarItem" + item.ViewName;
			this.Write("\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = New DevExpress.XtraBars.Navigation.TileBarItem()\r\n\t\t\t");
}	
			this.Write("\t\t\t");
foreach(var item in viewModelData.Views){
					string nameForItem = "tileBarItem" + item.ViewName;
			this.Write("\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" = New DevExpress.XtraBars.Navigation.TileBarItem()\r\n\t\t\t");
}	
			this.Write("            Me.SuspendLayout()\r\n            \'\r\n            \' tileBar\r\n           " +
					" \' \r\n            Me.tileBar.AllowDrag = False\r\n\t\t\tMe.tileBar.AllowGlyphSkinning " +
					"= True\r\n\t\t\tMe.tileBar.AllowSelectedItem = True\r\n\t\t\tMe.tileBar.Dock = System.Wind" +
					"ows.Forms.DockStyle.Top\r\n\t\t\tMe.tileBar.DropDownOptions.BeakColor = System.Drawin" +
					"g.Color.Empty\r\n\t\t\tMe.tileBar.Groups.Add(Me.tileBarGroupTables)\r\n\t\t\tMe.tileBar.Gr" +
					"oups.Add(Me.tileBarGroupViews)\r\n\t\t\tMe.tileBar.BackColor = System.Drawing.Color.F" +
					"romArgb(230, 230, 230)\r\n\t\t\tMe.tileBar.Location = New System.Drawing.Point(0, 0)\r" +
					"\n\t\t\tMe.tileBar.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)\r\n\t\t\tMe.tile" +
					"Bar.Name = \"tileBar\"\r\n\t\t\tMe.tileBar.Padding = New System.Windows.Forms.Padding(2" +
					"9, 11, 29, 11)\r\n\t\t\tMe.tileBar.ScrollMode = DevExpress.XtraEditors.TileControlScr" +
					"ollMode.ScrollButtons\r\n\t\t\tMe.tileBar.AppearanceGroupText.ForeColor = System.Draw" +
					"ing.Color.FromArgb((CInt((CByte(140)))), (CInt((CByte(140)))), (CInt((CByte(140)" +
					"))))\r\n\t\t\tMe.tileBar.AppearanceGroupText.Options.UseForeColor = True\r\n\t\t\tMe.tileB" +
					"ar.ItemPadding = New System.Windows.Forms.Padding(8, 6, 12, 6)\r\n\t\t\tMe.tileBar.In" +
					"dentBetweenGroups = 10\r\n\t\t\tMe.tileBar.IndentBetweenItems = 10\r\n\t\t\tMe.tileBar.Dro" +
					"pDownButtonWidth = 30\r\n\t\t\tMe.tileBar.SelectionBorderWidth = 2\r\n\t\t\tMe.tileBar.Wid" +
					"eTileWidth = 150\r\n\t\t\tMe.tileBar.MinimumSize = New System.Drawing.Size(100, 110)\r" +
					"\n\t\t\tMe.tileBar.MaximumSize = New System.Drawing.Size(0, 110)\r\n\t\t\tMe.tileBar.Sele" +
					"ctionBorderWidth = 2\r\n\t\t\tMe.tileBar.Text = \"tileBar\"\r\n\t\t\t\'\r\n\t\t\t\'tileBarGroupTabl" +
					"es\r\n\t\t\t\'\r\n\t\t\tMe.tileBarGroupTables.Name = \"tileBarGroupTables\"\r\n\t\t\tMe.tileBarGro" +
					"upTables.Text = \"TABLES\"\r\n\t\t\t\' \r\n\t\t\t\' tileBarGroupViews\r\n\t\t\t\' \r\n\t\t\tMe.tileBarGro" +
					"upViews.Name = \"tileBarGroupViews\"\r\n\t\t\tMe.tileBarGroupViews.Text = \"VIEWS\"\r\n\t\t\t");
			int indexer = -1;
			int Red = 0;
			int Green = 0;
			int Blue = 0;
			foreach(var item in viewModelData.Tables){
				string nameForItem = "tileBarItem" + item.ViewName;
				indexer++;
				string nameForElement = "tileItemElement" + item.ViewName;
				if(indexer % 5 == 0){Red = 0; Green = 135; Blue = 156; }
				if(indexer % 5 == 1){Red = 0; Green = 0; Blue = 0; }
				if(indexer % 5 == 2){Red = 204; Green = 109; Blue = 0; }
				if(indexer % 5 == 3){Red = 0; Green = 115; Blue = 196; }
				if(indexer % 5 == 4){Red = 62; Green = 112; Blue = 56; }
			this.Write("\t\t\t\'\r\n\t\t\t\'");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n\t\t\t\'\r\n\t\t\tDim ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(" As New DevExpress.XtraEditors.TileItemElement()\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\"\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(".ImageUri.Uri = \"Cube;Size32x32;GrayScaled\"\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Elements.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(")\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb((CInt((CByte(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Red));
			this.Write(")))), (CInt((CByte(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Green));
			this.Write(")))), (CInt((CByte(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Blue));
			this.Write(")))))\t\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".AppearanceItem.Normal.Options.UseBackColor = true\r\n            Me.tileBarGroupTa" +
					"bles.Items.Add(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(")\r\n\t\t\t");
}
			this.Write("\t\t\t");
foreach(var item in viewModelData.Views){
				indexer++;
				string nameForItem = "tileBarItem" + item.ViewName;
				string nameForElement = "tileItemElement" + item.ViewName;
				if(indexer % 5 == 0){Red = 0; Green = 135; Blue = 156; }
				if(indexer % 5 == 1){Red = 0; Green = 0; Blue = 0; }
				if(indexer % 5 == 2){Red = 204; Green = 109; Blue = 0; }
				if(indexer % 5 == 3){Red = 0; Green = 115; Blue = 196; }
				if(indexer % 5 == 4){Red = 62; Green = 112; Blue = 56; }
			this.Write("\t\t\t\'\r\n\t\t\t\'");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n\t\t\t\'\r\n\t\t\tDim ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(" As New DevExpress.XtraEditors.TileItemElement()\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(".Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\"\r\n            ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(".ImageUri.Uri = \"Cube;Size32x32;GrayScaled\"\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".Elements.Add(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForElement));
			this.Write(")\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb((CInt((CByte(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Red));
			this.Write(")))), (CInt((CByte(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Green));
			this.Write(")))), (CInt((CByte(");
			this.Write(this.ToStringHelper.ToStringWithCulture(Blue));
			this.Write(")))))\t\r\n            Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(".AppearanceItem.Normal.Options.UseBackColor = true\r\n            Me.tileBarGroupVi" +
					"ews.Items.Add(Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(")\r\n\t\t\t");
}
			this.Write(@"	
			' 
			' navigationFrame
			' 
			Me.navigationFrame.Dock = System.Windows.Forms.DockStyle.Fill
			Me.navigationFrame.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
			Me.navigationFrame.Name = ""navigationFrame""
			Me.navigationFrame.SelectedPage = Nothing
			Me.navigationFrame.SelectedPageIndex = -1
			Me.navigationFrame.Text = ""navigationFrame""
			' 
			' mvvmContext
			' 
			Me.mvvmContext.ContainerControl = Me
			Me.mvvmContext.ViewModelType = GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(@")
			' 
			' NorthwindEntitiesView
			' 
			Me.Appearance.BackColor = System.Drawing.Color.White
			Me.Appearance.Options.UseBackColor = True
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.Controls.Add(Me.navigationFrame)
			Me.Controls.Add(Me.tileBar)
			Me.Size = New System.Drawing.Size(1024, 768)
			Me.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("\"\r\n\t\t\tMe.Text = \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"""
			Me.ResumeLayout(False)



		End Sub

		#End Region

		Private tileBar As DevExpress.XtraBars.Navigation.TileBar
		Private navigationFrame As DevExpress.XtraBars.Navigation.NavigationFrame
		Private mvvmContext As DevExpress.Utils.MVVM.MVVMContext
		Private tileBarGroupTables As DevExpress.XtraBars.Navigation.TileBarGroup
		Private tileBarGroupViews As DevExpress.XtraBars.Navigation.TileBarGroup
		");
foreach(var item in viewModelData.Tables){
				string nameForItem = "tileBarItem" + item.ViewName;
			this.Write("\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" As DevExpress.XtraBars.Navigation.TileBarItem\r\n\t\t");
}	
			this.Write("\t\t");
foreach(var item in viewModelData.Views){
				string nameForItem = "tileBarItem" + item.ViewName;
			this.Write("\t\tPrivate ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(" As DevExpress.XtraBars.Navigation.TileBarItem\r\n\t\t");
}	
			this.Write("\tEnd Class\r\nEnd Namespace\r\n");
}
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentManagerView_WinUIDesignerBase
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
