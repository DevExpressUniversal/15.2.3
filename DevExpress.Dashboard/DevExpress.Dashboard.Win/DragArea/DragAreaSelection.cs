#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardWin.Native {
	public class DragAreaSelection {
		DragGroup group;
		DragItem item;
		IDragAreaElementWithButton elementWithButton;
		ImageButton imageButton;
		DragAreaSelectionType type;
		int selectedGroupIndex;
		public DragGroup Group { get { return group; } set { group = value; } }
		public DragItem Item { get { return item; } set { item = value; } }
		public IDragAreaElementWithButton ElementWithButton { get { return elementWithButton; } set { elementWithButton = value; } }
		public ImageButton ImageButton { get { return imageButton; } set { imageButton = value; } }
		public DragAreaSelectionType Type { get { return type; } }
		public int SelectedGroupIndex { get { return selectedGroupIndex; } }
		public DragAreaSelection(DragGroup group, DragItem item, IDragAreaElementWithButton elementWithButton, DragAreaSelectionType type) {
			this.group = group;
			this.selectedGroupIndex = group.Section.IndexOf(group);
			this.item = item;
			this.elementWithButton = elementWithButton;
			this.type = type;
		}
		public DragAreaSelection(DragAreaSelectionType type) {
			this.type = type;
		}
		public bool SameSelection(DragAreaSelection selection) {
			if (selection == null || Type != selection.Type)
				return false;
			switch(Type){
				case DragAreaSelectionType.OptionsButton:
				case DragAreaSelectionType.Group:
					return Group == selection.Group;
				case DragAreaSelectionType.ImageButton:
					return ImageButton == selection.ImageButton;
				case DragAreaSelectionType.DragItemPopupButton:
				case DragAreaSelectionType.NonEmptyDragItem:
					return Item == selection.Item;
				case DragAreaSelectionType.None:
					return selection.Type == DragAreaSelectionType.None;
			}
			return false;
		}
	}
	public enum DragAreaSelectionType { 
		None,
		Group,
		NonEmptyDragItem,
		DragItemPopupButton,
		OptionsButton,
		ImageButton
	}
}
