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
using DevExpress.XtraPivotGrid.Customization.ViewInfo;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.ViewInfo;
namespace DevExpress.XtraPivotGrid.Customization {
	[ToolboxItem(false)]
	public abstract class PivotCustomizationTreeBoxBase : CustomizationTreeBox {		
		public PivotCustomizationTreeBoxBase(CustomizationForm form)
			: base(form) {
			this.UseDisabledStatePainter = false;
		}
		protected PivotGridViewInfoData Data { get { return CustomizationForm.Data; } }
		protected PivotGridControl PivotGrid { get { return Data != null ? Data.PivotGrid : null; } }
		protected PivotGridViewInfo PivotViewInfo { get { return (PivotGridViewInfo)Data.ViewInfo; } }
		public CustomizationFormFields CustomizationFields { get { return CustomizationForm.CustomizationFields; } }
		protected override bool IsDragging { get { return PivotViewInfo.IsDragging; } }
		protected override void DoDragDrop(object dragItem, Point p) {
			IVisualCustomizationTreeItem node = (IVisualCustomizationTreeItem)dragItem;
			if(node.Field == null || !node.Field.CanDragInCustomizationForm)
				return;
			PivotViewInfo.StartDragging((PivotFieldItem)node.Field, delegate() {
				OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
			});
			Populate();
		}
#if DEBUGTEST
		internal object AccessPressedItem {
			get { return PressedItem; }
			set { PressedItem = value; }
		}
#endif
		protected override object PressedItem {
			get {
				if(!Items.Contains(base.PressedItem))
					return null;
				return base.PressedItem;
			}
			set { base.PressedItem = value; }
		}
	}
	[ToolboxItem(false)]
	public class PivotCustomizationTreeBox : PivotCustomizationTreeBoxBase {
		PivotCustomizationFieldsTreeBase fullTree;
		public PivotCustomizationTreeBox(CustomizationForm form)
			: base(form) {
		}
		public override bool HotTrackItems {
			get { return IsAdvHotTrackEnabled(); }
			set { base.HotTrackItems = value; }
		}
		protected PivotCustomizationFieldsTreeBase FullTree { get { return fullTree ?? (fullTree = CreateFieldsTree()); } }
		protected virtual bool ShowAsTree { get { return Data.ShowCustomizationTree; } }
		protected virtual PivotCustomizationFieldsTreeBase CreateFieldsTree() {
			return new PivotCustomizationFieldsTree(CustomizationFields, Data);
		}
		protected override CustomizationItemViewInfo CreateItemViewInfo(IVisualCustomizationTreeItem item) {
			if(Data.ShowCustomizationTree)
				return base.CreateItemViewInfo(item);
			else
				return new PivotCustomizationListItemViewInfo(this, item, (PivotGridViewInfo)Data.ViewInfo);
		}
		protected override int CalcItemHeight() {
			if(Data.ShowCustomizationTree)
				return base.CalcItemHeight();
			else
				return PivotViewInfo.FieldMeasures.DefaultHeaderHeight;
		}
		protected virtual void MoveFieldToPivotGrid(PivotFieldItem field) {
			if(field == null) return;
			CustomizationForm.BottomPanel.MoveFieldToPivotGrid(field);
		}
		protected override void PopulateCore() {
			FullTree.Update(ShowAsTree);
			foreach(ICustomizationTreeItem node in FullTree) {
				if(node.IsVisible && CustomizationForm.BottomPanel.IsNodeVisible(node))
					Items.Add(node);
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Left:
					OnLeftArrowKeyDown(SelectedItem);
					return;
				case Keys.Enter:
					OnEnterKeyDown();
					return;
			}
			base.OnKeyDown(e);
		}
		protected override void OnNodeDoubleClick(IVisualCustomizationTreeItem item) {
			PivotFieldItem field = (PivotFieldItem)item.Field;
			if(field != null)
				MoveFieldToPivotGrid(field);
			else
				base.OnNodeDoubleClick(item);
		}
		protected internal virtual void OnEnterKeyDown() {
			if(SelectedItem != null)
				MoveFieldToPivotGrid((PivotFieldItem)SelectedItem.Field);
		}
		protected virtual void OnLeftArrowKeyDown(IVisualCustomizationTreeItem node) {
			if(node == null) return;
			if(node.IsExpanded && node.CanExpand) {
				ChangeExpanded(node, false);
				return;
			}
			int visibleIndex = FullTree.GetNodeVisibleIndex(node.Parent);
			SelectedIndex = visibleIndex == -1 ? 0 : visibleIndex;
		}
	}
}
