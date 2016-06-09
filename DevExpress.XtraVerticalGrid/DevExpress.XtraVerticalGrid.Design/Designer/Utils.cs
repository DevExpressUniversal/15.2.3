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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraTab;
using System.ComponentModel;
namespace DevExpress.XtraVerticalGrid.Design {
	[ToolboxItem(false)]
	public class MyTreeView : DevExpress.Utils.Design.DXTreeView {
		public MyTreeView() {
		}
		protected override CreateParams CreateParams {
			get {
				const int TVS_NOHSCROLL = 0x8000;
				CreateParams cp = base.CreateParams;
				cp.Style |= TVS_NOHSCROLL;
				return cp;
			}
		}
		protected override bool UseThemes {
			get { return true; }
		}
	}
	public class GridAssign {
		public static VGridControl CreateGridControlAssign(VGridControlBase editingGrid) {
			VGridControl grid = (VGridControl)Activator.CreateInstance(editingGrid.GetType());
			grid.Dock = DockStyle.Fill;
			GridControlAssign(editingGrid, grid, true, true);	
			return grid;
		}
		public static void GridControlAssign(VGridControlBase editingGrid, VGridControlBase grid, bool setRepository, bool setStyles) {
			if(setRepository) {
				AssignEditors(editingGrid, grid);
				grid.ExternalRepository = editingGrid.ExternalRepository;
			}
			if(setStyles) grid.Appearance.Assign(editingGrid.Appearance);
			grid.LookAndFeel.Assign(editingGrid.LookAndFeel);
			grid.ImageList = editingGrid.ImageList; 
			System.IO.MemoryStream str = new System.IO.MemoryStream();
			editingGrid.SaveLayoutToStream(str);
			str.Seek(0, System.IO.SeekOrigin.Begin);
			grid.RestoreLayoutFromStream(str);
			str.Close();
		}
		private static void AssignEditors(VGridControlBase sourceGrid, VGridControlBase grid) {
			foreach(DevExpress.XtraEditors.Repository.RepositoryItem item in sourceGrid.RepositoryItems) 
				grid.RepositoryItems.Add(item.Clone() as DevExpress.XtraEditors.Repository.RepositoryItem);
		}
	}
}
namespace DevExpress.XtraVerticalGrid.Frames {	
	internal enum DragDropOperation {AddChild, Insert, Add};
	internal class VGridDragDropItem {
		private TreeNode dragItem;
		private TreeNode destItem;
		private DragDropOperation operation;
		public VGridDragDropItem(TreeNode dragItem, TreeNode destItem, DragDropOperation operation) {
			this.dragItem = dragItem;
			this.destItem = destItem;
			this.operation = operation;
		}
		public TreeNode DragItem { get { return dragItem; } }
		public TreeNode DestItem { get { return destItem; } }
		public DragDropOperation Operation { get { return operation; } }
	} 
}
