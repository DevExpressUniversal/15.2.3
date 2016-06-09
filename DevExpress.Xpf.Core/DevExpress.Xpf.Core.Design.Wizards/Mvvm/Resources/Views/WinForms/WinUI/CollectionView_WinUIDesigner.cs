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
	using DevExpress.Mvvm.UI.Native.ViewGenerator;
	using DevExpress.Entity.Model;
	using DevExpress.Mvvm.Native;
	using System;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class CollectionView_WinUIDesigner : CollectionView_WinUIDesignerBase
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
					"nts);\r\n\t\t\tthis.labelControl = new DevExpress.XtraEditors.LabelControl();\r\n\t\t\tthi" +
					"s.windowsUIButtonPanel = new DevExpress.XtraBars.Docking2010.WindowsUIButtonPane" +
					"l();\r\n\t\t\tthis.layoutControl = new DevExpress.XtraLayout.LayoutControl();\r\n      " +
					"      this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();\r" +
					"\n            this.itemLabel = new DevExpress.XtraLayout.LayoutControlItem();\r\n  " +
					"          this.itemGrid = new DevExpress.XtraLayout.LayoutControlItem();\r\n      " +
					"      ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit" +
					"();\r\n            this.layoutControl.SuspendLayout();\r\n            ((System.Compo" +
					"nentModel.ISupportInitialize)(this.gridControl)).BeginInit();\r\n            ((Sys" +
					"tem.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();\r\n           " +
					" ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).BeginInit();\r\n  " +
					"          this.SuspendLayout();\r\n\t\t\t// \r\n            // windowsUIButtonPanel\r\n  " +
					"          // \r\n            this.windowsUIButtonPanel.AppearanceButton.Hovered.Ba" +
					"ckColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(1" +
					"30)))), ((int)(((byte)(130)))));\r\n            this.windowsUIButtonPanel.Appearan" +
					"ceButton.Hovered.FontSizeDelta = -1;\r\n            this.windowsUIButtonPanel.Appe" +
					"aranceButton.Hovered.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13" +
					"0)))), ((int)(((byte)(130)))), ((int)(((byte)(130)))));\r\n            this.window" +
					"sUIButtonPanel.AppearanceButton.Hovered.Options.UseBackColor = true;\r\n          " +
					"  this.windowsUIButtonPanel.AppearanceButton.Hovered.Options.UseFont = true;\r\n  " +
					"          this.windowsUIButtonPanel.AppearanceButton.Hovered.Options.UseForeColo" +
					"r = true;\r\n            this.windowsUIButtonPanel.AppearanceButton.Normal.FontSiz" +
					"eDelta = -1;\r\n            this.windowsUIButtonPanel.AppearanceButton.Normal.Opti" +
					"ons.UseFont = true;\r\n            this.windowsUIButtonPanel.AppearanceButton.Pres" +
					"sed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((b" +
					"yte)(159)))), ((int)(((byte)(159)))));\r\n            this.windowsUIButtonPanel.Ap" +
					"pearanceButton.Pressed.FontSizeDelta = -1;\r\n            this.windowsUIButtonPane" +
					"l.AppearanceButton.Pressed.ForeColor = System.Drawing.Color.FromArgb(((int)(((by" +
					"te)(159)))), ((int)(((byte)(159)))), ((int)(((byte)(159)))));\r\n            this." +
					"windowsUIButtonPanel.AppearanceButton.Pressed.Options.UseBackColor = true;\r\n    " +
					"        this.windowsUIButtonPanel.AppearanceButton.Pressed.Options.UseFont = tru" +
					"e;\r\n            this.windowsUIButtonPanel.AppearanceButton.Pressed.Options.UseFo" +
					"reColor = true;\r\n            this.windowsUIButtonPanel.BackColor = System.Drawin" +
					"g.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(63" +
					")))));\r\n            this.windowsUIButtonPanel.Dock = System.Windows.Forms.DockSt" +
					"yle.Bottom;\r\n            this.windowsUIButtonPanel.EnableImageTransparency = tru" +
					"e;\r\n            this.windowsUIButtonPanel.ForeColor = System.Drawing.Color.White" +
					";\r\n            this.windowsUIButtonPanel.Margin = new System.Windows.Forms.Paddi" +
					"ng(4, 5, 4, 5);\r\n            this.windowsUIButtonPanel.Name = \"windowsUIButtonPa" +
					"nel\";\r\n            this.windowsUIButtonPanel.Text = \"windowsUIButtonPanel\";\r\n   " +
					"         this.windowsUIButtonPanel.UseButtonBackgroundImages = false;\r\n\t\t\tthis.w" +
					"indowsUIButtonPanel.MinimumSize = new System.Drawing.Size(60, 60);\r\n            " +
					"this.windowsUIButtonPanel.MaximumSize = new System.Drawing.Size(0, 60);\r\n\t\t\tthis" +
					".windowsUIButtonPanel.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IB" +
					"aseButton[] {\r\n\t\t\t");
			foreach(var item in viewModelData.Commands){
			string nameForItem = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			if(nameForItem == "Delete") nameForItem = "Edit/Delete";
			this.Write("\t\t\tnew DevExpress.XtraBars.Docking2010.WindowsUIButton(\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\", null, \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(";Size32x32;GrayScaled\"),\r\n\t\t\t");
}
			this.Write(@"			new DevExpress.XtraBars.Docking2010.WindowsUISeparator(),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton(""Print"", null, ""Preview;Size32x32;GrayScaled"")
			});
            // 
            // labelControl
            // 
            this.labelControl.AllowHtmlString = true;
            this.labelControl.Appearance.Font = new System.Drawing.Font(""Segoe UI"", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelControl.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.labelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.labelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl.Name = ""labelControl"";
            this.labelControl.Padding = new System.Windows.Forms.Padding(0, 3, 13, 6);
            this.labelControl.Text = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeName));
			this.Write(@""";
			// 
            // gridControl
            // 
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.Location = new System.Drawing.Point(5, 116);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = ""gridControl"";
            this.gridControl.Size = new System.Drawing.Size(779, 311);
            this.gridControl.TabIndex = 2;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // gridView
            // 
            this.gridView.GridControl = this.gridControl;
            this.gridView.Name = ""gridView"";
            this.gridView.OptionsBehavior.Editable = false;
            this.gridView.OptionsCustomization.AllowColumnMoving = false;
            this.gridView.OptionsCustomization.AllowGroup = false;
            this.gridView.OptionsCustomization.AllowQuickHideColumns = false;
            this.gridView.OptionsMenu.EnableColumnMenu = false;
            this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.OptionsView.ShowIndicator = false;
			this.");
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
			this.Write("),parameters);\r\n\t\t    // \r\n            // mvvmContext\r\n            // \r\n         " +
					"   this.mvvmContext.ContainerControl = this;\r\n            this.mvvmContext.ViewM" +
					"odelType = typeof(");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(");\r\n\t\t\t// \r\n            // layoutControl\r\n            // \r\n            layoutCont" +
					"rol.Controls.AddRange(new System.Windows.Forms.Control[] { this.labelControl, th" +
					"is.gridControl });\r\n            this.layoutControl.AllowCustomization = false;\r\n" +
					"            this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;\r\n    " +
					"        this.layoutControl.Root = this.layoutControlGroup;\r\n            //\r\n    " +
					"        // itemLabel\r\n            //\r\n            this.itemLabel.Control = this." +
					"labelControl;\r\n            this.itemLabel.TextVisible = false;\r\n            this" +
					".itemLabel.Name = \"itemLabel\";\r\n\t\t\tthis.itemLabel.Padding = new DevExpress.XtraL" +
					"ayout.Utils.Padding(0);\r\n            //\r\n            // itemGrid\r\n            //" +
					"\r\n            this.itemGrid.Control = this.gridControl;\r\n            this.itemGr" +
					"id.TextVisible = false;\r\n            this.itemGrid.Name = \"itemGrid\";\r\n\t\t\tthis.i" +
					"temGrid.Padding = new DevExpress.XtraLayout.Utils.Padding(0);\r\n            //\r\n " +
					"           // layoutControlGroup\r\n            //\r\n            this.layoutControl" +
					"Group.GroupBordersVisible = false;\r\n            this.layoutControlGroup.Add(item" +
					"Label);\r\n            this.layoutControlGroup.Add(itemGrid);\r\n            this.la" +
					"youtControlGroup.Name = \"layoutControlGroup\";\r\n            this.layoutControlGro" +
					"up.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;\r\n        " +
					"    this.layoutControlGroup.GroupBordersVisible = false;\r\n            this.layou" +
					"tControlGroup.TextVisible = false;\r\n\t\t\tthis.layoutControlGroup.Padding = new Dev" +
					"Express.XtraLayout.Utils.Padding(40, 40, 0, 0);\r\n\t\t\t//\r\n\t\t\t//");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
			//
			this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl);
            this.Controls.Add(this.windowsUIButtonPanel);
			this.Size = new System.Drawing.Size(1024, 768);
            this.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@""";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mvvmContext)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
		}
		
        #endregion

		private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
		private DevExpress.Utils.MVVM.MVVMContext mvvmContext;
		private System.Windows.Forms.BindingSource ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(@";
		private	DevExpress.XtraEditors.LabelControl labelControl;
		private	DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel windowsUIButtonPanel;
		private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
        private DevExpress.XtraLayout.LayoutControlItem itemLabel;
        private DevExpress.XtraLayout.LayoutControlItem itemGrid;
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
					"ary>\r\n\t\tPrivate Sub InitializeComponent()\r\n\t\t\t\t\tMe.components = New System.Compo" +
					"nentModel.Container()\r\n\t\t\tMe.gridControl = New DevExpress.XtraGrid.GridControl()" +
					"\r\n\t\t\tMe.gridView = New DevExpress.XtraGrid.Views.Grid.GridView()\r\n\t\t\tMe.mvvmCont" +
					"ext = New DevExpress.Utils.MVVM.MVVMContext(Me.components)\r\n\t\t\tMe.labelControl =" +
					" New DevExpress.XtraEditors.LabelControl()\r\n\t\t\tMe.windowsUIButtonPanel = New Dev" +
					"Express.XtraBars.Docking2010.WindowsUIButtonPanel()\r\n\t\t\tMe.layoutControl = New D" +
					"evExpress.XtraLayout.LayoutControl()\r\n\t\t\tMe.layoutControlGroup = New DevExpress." +
					"XtraLayout.LayoutControlGroup()\r\n\t\t\tMe.itemLabel = New DevExpress.XtraLayout.Lay" +
					"outControlItem()\r\n\t\t\tMe.itemGrid = New DevExpress.XtraLayout.LayoutControlItem()" +
					"\r\n\t\t\tCType(Me.layoutControl, System.ComponentModel.ISupportInitialize).BeginInit" +
					"()\r\n\t\t\tMe.layoutControl.SuspendLayout()\r\n\t\t\tCType(Me.gridControl, System.Compone" +
					"ntModel.ISupportInitialize).BeginInit()\r\n\t\t\tCType(Me.gridView, System.ComponentM" +
					"odel.ISupportInitialize).BeginInit()\r\n\t\t\tCType(Me.mvvmContext, System.ComponentM" +
					"odel.ISupportInitialize).BeginInit()\r\n\t\t\tMe.SuspendLayout()\r\n\t\t\t\' \r\n\t\t\t\' windows" +
					"UIButtonPanel\r\n\t\t\t\' \r\n\t\t\tMe.windowsUIButtonPanel.AppearanceButton.Hovered.BackCo" +
					"lor = System.Drawing.Color.FromArgb((CInt((CByte(130)))), (CInt((CByte(130)))), " +
					"(CInt((CByte(130)))))\r\n\t\t\tMe.windowsUIButtonPanel.AppearanceButton.Hovered.FontS" +
					"izeDelta = -1\r\n\t\t\tMe.windowsUIButtonPanel.AppearanceButton.Hovered.ForeColor = S" +
					"ystem.Drawing.Color.FromArgb((CInt((CByte(130)))), (CInt((CByte(130)))), (CInt((" +
					"CByte(130)))))\r\n\t\t\tMe.windowsUIButtonPanel.AppearanceButton.Hovered.Options.UseB" +
					"ackColor = True\r\n\t\t\tMe.windowsUIButtonPanel.AppearanceButton.Hovered.Options.Use" +
					"Font = True\r\n\t\t\tMe.windowsUIButtonPanel.AppearanceButton.Hovered.Options.UseFore" +
					"Color = True\r\n\t\t\tMe.windowsUIButtonPanel.AppearanceButton.Normal.FontSizeDelta =" +
					" -1\r\n\t\t\tMe.windowsUIButtonPanel.AppearanceButton.Normal.Options.UseFont = True\r\n" +
					"\t\t\tMe.windowsUIButtonPanel.AppearanceButton.Pressed.BackColor = System.Drawing.C" +
					"olor.FromArgb((CInt((CByte(159)))), (CInt((CByte(159)))), (CInt((CByte(159)))))\r" +
					"\n\t\t\tMe.windowsUIButtonPanel.AppearanceButton.Pressed.FontSizeDelta = -1\r\n\t\t\tMe.w" +
					"indowsUIButtonPanel.AppearanceButton.Pressed.ForeColor = System.Drawing.Color.Fr" +
					"omArgb((CInt((CByte(159)))), (CInt((CByte(159)))), (CInt((CByte(159)))))\r\n\t\t\tMe." +
					"windowsUIButtonPanel.AppearanceButton.Pressed.Options.UseBackColor = True\r\n\t\t\tMe" +
					".windowsUIButtonPanel.AppearanceButton.Pressed.Options.UseFont = True\r\n\t\t\tMe.win" +
					"dowsUIButtonPanel.AppearanceButton.Pressed.Options.UseForeColor = True\r\n\t\t\tMe.wi" +
					"ndowsUIButtonPanel.BackColor = System.Drawing.Color.FromArgb((CInt((CByte(63))))" +
					", (CInt((CByte(63)))), (CInt((CByte(63)))))\r\n\t\t\tMe.windowsUIButtonPanel.Dock = S" +
					"ystem.Windows.Forms.DockStyle.Bottom\r\n\t\t\tMe.windowsUIButtonPanel.EnableImageTran" +
					"sparency = True\r\n\t\t\tMe.windowsUIButtonPanel.ForeColor = System.Drawing.Color.Whi" +
					"te\r\n\t\t\tMe.windowsUIButtonPanel.Margin = New System.Windows.Forms.Padding(4, 5, 4" +
					", 5)\r\n\t\t\tMe.windowsUIButtonPanel.Name = \"windowsUIButtonPanel\"\r\n\t\t\tMe.windowsUIB" +
					"uttonPanel.Text = \"windowsUIButtonPanel\"\r\n\t\t\tMe.windowsUIButtonPanel.UseButtonBa" +
					"ckgroundImages = False\r\n\t\t\tMe.windowsUIButtonPanel.MinimumSize = New System.Draw" +
					"ing.Size(60, 60)\r\n\t\t\tMe.windowsUIButtonPanel.MaximumSize = New System.Drawing.Si" +
					"ze(0, 60)\r\n\t\t\t");
			foreach(var item in viewModelData.Commands){
			string nameForItem = item.CommandPropertyName.Remove(item.CommandPropertyName.Length -7,7);
			if(nameForItem == "Delete") nameForItem = "Edit/Delete";
			this.Write("\t\t\tMe.windowsUIButtonPanel.Buttons.Add(New DevExpress.XtraBars.Docking2010.Window" +
					"sUIButton(\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(item.Caption));
			this.Write("\", Nothing, \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(";Size32x32;GrayScaled\"))\r\n\t\t\t");
}
			this.Write(@"			Me.windowsUIButtonPanel.Buttons.Add(New DevExpress.XtraBars.Docking2010.WindowsUISeparator())
			Me.windowsUIButtonPanel.Buttons.Add(New DevExpress.XtraBars.Docking2010.WindowsUIButton(""Print"", Nothing, ""Preview;Size32x32;GrayScaled""))
			'
			' labelControl
            ' 
            Me.labelControl.AllowHtmlString = True
			Me.labelControl.Appearance.Font = New System.Drawing.Font(""Segoe UI"", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte(204)))
			Me.labelControl.Appearance.ForeColor = System.Drawing.Color.FromArgb((CInt((CByte(140)))), (CInt((CByte(140)))), (CInt((CByte(140)))))
			Me.labelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
			Me.labelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical
			Me.labelControl.Dock = System.Windows.Forms.DockStyle.Top
			Me.labelControl.Name = ""labelControl""
			Me.labelControl.Padding = New System.Windows.Forms.Padding(0, 3, 13, 6)
			Me.labelControl.Text = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeName));
			this.Write(@"""
			' 
			' gridControl
			' 
			Me.gridControl.Dock = System.Windows.Forms.DockStyle.Fill
			Me.gridControl.Location = New System.Drawing.Point(5, 116)
			Me.gridControl.MainView = Me.gridView
			Me.gridControl.Name = ""gridControl""
			Me.gridControl.Size = New System.Drawing.Size(779, 311)
			Me.gridControl.TabIndex = 2
			Me.gridControl.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() { Me.gridView})
			' 
			' gridView
			' 
			Me.gridView.GridControl = Me.gridControl
			Me.gridView.Name = ""gridView""
			Me.gridView.OptionsBehavior.Editable = False
			Me.gridView.OptionsCustomization.AllowColumnMoving = False
			Me.gridView.OptionsCustomization.AllowGroup = False
			Me.gridView.OptionsCustomization.AllowQuickHideColumns = False
			Me.gridView.OptionsMenu.EnableColumnMenu = False
			Me.gridView.OptionsSelection.EnableAppearanceFocusedCell = False
			Me.gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never
			Me.gridView.OptionsView.ShowGroupPanel = False
			Me.gridView.OptionsView.ShowIndicator = False
			Me.");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(" = New System.Windows.Forms.BindingSource(Me.components)\r\n\t\t\tMe.");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(".DataSource = GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeFullName));
			this.Write(")\r\n\t\t\tMe.gridControl.DataSource = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write("\r\n\t\t\t\r\n\t\t\tDim parameters As New DevExpress.XtraGrid.Extensions.PopulateColumnsPar" +
					"ameters()\r\n\t\t\t");
			foreach(var realLookUpInfo in listLookUpInfo){
			string nameForLookUp = realLookUpInfo.Property.Name;
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
			this.Write("), parameters)\r\n\t\t\t\' \r\n\t\t\t\' mvvmContext\r\n\t\t\t\' \r\n\t\t\tMe.mvvmContext.ContainerContro" +
					"l = Me\r\n\t\t\tMe.mvvmContext.ViewModelType = GetType(Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(@")
			' 
			' layoutControl
			' 
			layoutControl.Controls.AddRange(New System.Windows.Forms.Control() { Me.labelControl, Me.gridControl })
			Me.layoutControl.AllowCustomization = False
			Me.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill
			Me.layoutControl.Root = Me.layoutControlGroup
			'
			' itemLabel
			'
			Me.itemLabel.Control = Me.labelControl
			Me.itemLabel.TextVisible = False
			Me.itemLabel.Name = ""itemLabel""
			Me.itemLabel.Padding = New DevExpress.XtraLayout.Utils.Padding(0)
			'
			' itemGrid
			'
			Me.itemGrid.Control = Me.gridControl
			Me.itemGrid.TextVisible = False
			Me.itemGrid.Name = ""itemGrid""
			Me.itemGrid.Padding = New DevExpress.XtraLayout.Utils.Padding(0)
			'
			' layoutControlGroup
			'
			Me.layoutControlGroup.GroupBordersVisible = False
			Me.layoutControlGroup.Add(itemLabel)
			Me.layoutControlGroup.Add(itemGrid)
			Me.layoutControlGroup.Name = ""layoutControlGroup""
			Me.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True
			Me.layoutControlGroup.GroupBordersVisible = False
			Me.layoutControlGroup.TextVisible = False
			Me.layoutControlGroup.Padding = New DevExpress.XtraLayout.Utils.Padding(40, 40, 0, 0)
			'
			'");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
			'
			Me.Appearance.BackColor = System.Drawing.Color.White
			Me.Appearance.Options.UseBackColor = True
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.Controls.Add(Me.layoutControl)
			Me.Controls.Add(Me.windowsUIButtonPanel)
			Me.Size = New System.Drawing.Size(1024, 768)
			Me.Name = """);
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"""
			CType(Me.layoutControl, System.ComponentModel.ISupportInitialize).EndInit()
			Me.layoutControl.ResumeLayout(False)
			CType(Me.gridControl, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.gridView, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.mvvmContext, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)
			Me.PerformLayout()
		End Sub

		#End Region

		Private gridControl As DevExpress.XtraGrid.GridControl
		Private gridView As DevExpress.XtraGrid.Views.Grid.GridView
		Private mvvmContext As DevExpress.Utils.MVVM.MVVMContext
		Private ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(@" As System.Windows.Forms.BindingSource
		Private labelControl As DevExpress.XtraEditors.LabelControl
		Private windowsUIButtonPanel As DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel
		Private layoutControl As DevExpress.XtraLayout.LayoutControl
		Private layoutControlGroup As DevExpress.XtraLayout.LayoutControlGroup
		Private itemLabel As DevExpress.XtraLayout.LayoutControlItem
		Private itemGrid As DevExpress.XtraLayout.LayoutControlItem
	End Class
End Namespace
");
}
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class CollectionView_WinUIDesignerBase
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
