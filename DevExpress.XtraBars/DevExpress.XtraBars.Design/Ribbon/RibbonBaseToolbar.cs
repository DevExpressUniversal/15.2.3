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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Ribbon.Design {
	[ToolboxItem(false)]
	public class RibbonBaseToolbar : ItemLinksBaseToolbar {
		Container components = null;
		public RibbonBaseToolbar() {
			InitializeComponent();
		}
		protected override void Dispose( bool disposing ) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			this.SuspendLayout();
			this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
			this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
			this.barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.Name = "RibbonBaseToolbar";
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class RibbonItemsToolbar : RibbonBaseToolbar {
		protected override void FilterButtons() {
			base.FilterButtons();
			MoveUpButton.Visible = MoveDownButton.Visible = RemoveButton.Visible = MoveDeleteSeparator.Visible = false;
		}
	}
	[ToolboxItem(false)]
	public class RibbonLinksToolbar : RibbonBaseToolbar {
		SimpleButton btnAddPage;
		SimpleButton btnAddGroup;
		SimpleButton btnAddPageCategory;
		LabelControl vertSeparator;
		public RibbonLinksToolbar() {
			InitializeAdditionalComponents();
		}
		protected override void FilterButtons() {
			base.FilterButtons();
			AddItemButton.Visible = AddButtonGroup.Visible = RemoveItemButton.Visible = AddDeleteSeparator.Visible = false;
		}
		static Size DefaultButtonSize = new Size(28, 30);
		void InitializeAdditionalComponents() {
			this.btnAddPage = new SimpleButton();
			this.btnAddPage.AllowFocus = false;
			this.btnAddPage.Text = string.Empty;
			this.btnAddPage.Name = "btAddPage";
			this.btnAddPage.Size = DefaultButtonSize;
			this.btnAddPage.Margin = new Padding(3, 0, 3, 3);
			this.btnAddPage.ImageList = SmallImages;
			this.btnAddPage.ImageIndex = 1;
			this.btnAddPage.ImageLocation = ImageLocation.MiddleCenter;
			this.btnAddPage.Tag = "Add Page";
			this.btnAddGroup = new SimpleButton();
			this.btnAddGroup.AllowFocus = false;
			this.btnAddGroup.Text = string.Empty;
			this.btnAddGroup.Name = "btAddGroup";
			this.btnAddGroup.Size = DefaultButtonSize;
			this.btnAddGroup.Margin = new Padding(3, 0, 6, 3);
			this.btnAddGroup.ImageList = SmallImages;
			this.btnAddGroup.ImageIndex = 2;
			this.btnAddGroup.ImageLocation = ImageLocation.MiddleCenter;
			this.btnAddGroup.Tag = "Add Group";
			this.btnAddPageCategory = new SimpleButton();
			this.btnAddPageCategory.AllowFocus = false;
			this.btnAddPageCategory.Text = string.Empty;
			this.btnAddPageCategory.Name = "btAddPageCategory";
			this.btnAddPageCategory.Size = DefaultButtonSize;
			this.btnAddPageCategory.Margin = new Padding(0, 0, 3, 3);
			this.btnAddPageCategory.ImageList = SmallImages;
			this.btnAddPageCategory.ImageIndex = 0;
			this.btnAddPageCategory.ImageLocation = ImageLocation.MiddleCenter;
			this.btnAddPageCategory.Tag = "Add Category";
			this.vertSeparator = new LabelControl();
			this.vertSeparator.AutoSizeMode = LabelAutoSizeMode.None;
			this.vertSeparator.BorderStyle = BorderStyles.NoBorder;
			this.vertSeparator.LineLocation = LineLocation.Left;
			this.vertSeparator.LineOrientation = LabelLineOrientation.Vertical;
			this.vertSeparator.LineVisible = true;
			this.vertSeparator.Location = new Point(67, 0);
			this.vertSeparator.Margin = new Padding(4, 1, 8, 0);
			this.vertSeparator.Name = "vertSeparator";
			this.vertSeparator.Size = new Size(3, 28);
			ButtonContainer.Controls.Add(this.vertSeparator);
			ButtonContainer.Controls.SetChildIndex(this.vertSeparator, 0);
			ButtonContainer.Controls.Add(AddGroupButton);
			ButtonContainer.Controls.SetChildIndex(AddGroupButton, 0);
			ButtonContainer.Controls.Add(AddPageButton);
			ButtonContainer.Controls.SetChildIndex(AddPageButton, 0);
			ButtonContainer.Controls.Add(AddPageCategoryButton);
			ButtonContainer.Controls.SetChildIndex(AddPageCategoryButton, 0);
		}
		public SimpleButton AddPageButton { get { return btnAddPage; } }
		public SimpleButton AddGroupButton { get { return btnAddGroup; } }
		public SimpleButton AddPageCategoryButton { get { return btnAddPageCategory; } }
	}
}
