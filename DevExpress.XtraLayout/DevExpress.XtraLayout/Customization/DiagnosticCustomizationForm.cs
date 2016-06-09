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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraLayout.Customization.Controls;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout.Customization {
	[ToolboxItem(false)]
	public class TestCustomizationForm :CustomizationForm {
		public LayoutResizerTreeView HorizontalResizerTreeView;
		public LayoutControlItem lciHorizontalResizerTreeView;
		public LayoutControlItem lciVerticalResizerTreeView;
		public LayoutControlGroup resizeTreesGroup;
		public SplitterItem splitterItem2;
		public LayoutResizerTreeView VerticalResizerTreeView;
		public TestCustomizationForm() {
			InitializeResizerTreeView();
			base.CreatePanelTemplate();
		}
		void InitializeResizerTreeView() {
			Name = "TestCustomizationForm";
			VerticalResizerTreeView = new LayoutResizerTreeView();
			HorizontalResizerTreeView = new LayoutResizerTreeView();
			layoutControl1.Controls.AddRange(new Control[] { VerticalResizerTreeView, HorizontalResizerTreeView });
			resizeTreesGroup = tabbedControlGroup1.AddTabPage();
			resizeTreesGroup.Text = "ResizeTrees";
			lciHorizontalResizerTreeView = resizeTreesGroup.AddItem("HTree", HorizontalResizerTreeView);
			HorizontalResizerTreeView.Role = TreeViewRoles.ResizeTreeV;
			lciHorizontalResizerTreeView.TextVisible = false;
			lciVerticalResizerTreeView = resizeTreesGroup.AddItem("VTree", VerticalResizerTreeView, lciHorizontalResizerTreeView, InsertType.Right);
			VerticalResizerTreeView.Role = TreeViewRoles.ResizeTreeH;
			lciVerticalResizerTreeView.TextVisible = false;
			splitterItem2 = new SplitterItem();
			resizeTreesGroup.AddItem(splitterItem2, lciHorizontalResizerTreeView, InsertType.Right);
			tabbedControlGroup1.SelectedTabPageIndex = 0;
		}
		protected override void CreatePanelTemplate() {
		}
	}
}
