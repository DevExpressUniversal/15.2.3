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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Design;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public class GridControlAppearences : GridControlElementsSelector {
		Hashtable appearences;
		string[] selectedAppearanceNames;
		public EventHandler AppearanceCollectionChanged;
		public GridControlAppearences() : this(typeof(GridView))  {}
		public GridControlAppearences(Type viewType) : base(viewType) { 
			this.selectedAppearanceNames = new string[0] {};
		}
		public string[] SelectedAppearanceNames {
			get { return selectedAppearanceNames; }
			set {
				if(value == null) value = new string[0] {};
				if(IsSameArrays(selectedAppearanceNames, value)) return;
				selectedAppearanceNames = value;
				HighlightElements = GetElementsByAppearanceNames(value);
				HideHint();
			}
		}
		bool IsSameArrays(string[] array1, string[] array2) {
			if(array1.Length != array2.Length) return false;
			bool isSame = true;
			for(int i = 0; i < array1.Length; i ++) {
				if(array1[i] != array2[i]) {
					isSame = false;
					break;
				}
			}
			return isSame;
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			HighlightElements = GetElementsByAppearanceNames(SelectedAppearanceNames);
			HideHint();
		}
		protected virtual GridControlElementBase[] GetElementsByAppearanceNames(string[] names) {
			ArrayList list = new ArrayList();
			for(int i = 0; i < names.Length; i ++)
				AddElementsByAppearanceName(names[i], list);
			return (GridControlElementBase[])list.ToArray(typeof(GridControlElementBase));
		}
		protected virtual void AddElementsByAppearanceName(string name, ArrayList list) {
			if(name == string.Empty) return;
			foreach(DictionaryEntry entry in this.appearences) {
				string[] names = entry.Value as string[];
				if(Array.IndexOf(names, name) != -1) {
					GridControlElementBase element = GetElementByType(entry.Key as Type);
					if(element != null && list.IndexOf(element) < 0)
						list.Add(element);
				}
			}
		}
		protected override void AddElements() {
			base.AddElements();
			SetupAppearencesForElements();
		}
		protected virtual void SetupAppearencesForElements() {
			this.appearences = new Hashtable();
			this.appearences[typeof(GridControlColumnViewEmptyElement)] = new string[] {"Empty", "EmptySpace"};
			this.appearences[typeof(GridControlColumnViewFilterPanelElement)] = new string[] {"FilterPanel", "FilterCloseButton"};
			this.appearences[typeof(GridControlGridViewGroupPanelElement)] = new string[] {"GroupPanel"};
			this.appearences[typeof(GridControlGridViewHeadersElement)] = new string[] {"HeaderPanel", "HeaderPanelBackground", "FixedLine"};
			this.appearences[typeof(GridControlGridViewFilterButtonsElement)] = new string[] {"ColumnFilterButton", "ColumnFilterButtonActive"};
			this.appearences[typeof(GridControlGridViewIndicatorElement)] = new string[] {"HeaderPanel"}; 
			this.appearences[typeof(GridControlGridViewAutoFilterRowElement)] = new string[] {"Row"};
			this.appearences[typeof(GridControlGridViewNewItemRowElement)] = new string[] {"TopNewRow"};
			this.appearences[typeof(GridControlGridViewFooterElement)] = new string[] {"FooterPanel"};
			this.appearences[typeof(GridControlGridViewRowFooterElement)] = new string[] {"GroupFooter"};
			this.appearences[typeof(GridControlGridViewRowPreviewElement)] = new string[] {"Preview"};
			this.appearences[typeof(GridControlGridViewGroupRowElement)] = new string[] {"GroupRow", "GroupButton"};
			this.appearences[typeof(GridControlGridViewFocusedRowElement)] = new string[] {"FocusedRow", "FocusedCell", "HideSelectionRow", "SelectedRow"};
			this.appearences[typeof(GridControlGridViewRowElement)] = new string[] {"Row", "EvenRow", "HorzLine", "OddRow", "RowSeparator", "VertLine"};
			this.appearences[typeof(GridControlBandedGridViewBandsElement)] = new string[] {"BandPanel", "BandPanelBackground", "FixedLine"};
			this.appearences[typeof(GridControlCardViewCaptionElement)] = new string[] {"FocusedCardCaption", "CardCaption", "HideSelectionCardCaption", "SelectedCardCaption"};
			this.appearences[typeof(GridControlCardViewFieldCaptionElement)] = new string[] {"FieldCaption"};
			this.appearences[typeof(GridControlCardViewFieldValueElement)] = new string[] {"FieldValue"};
			this.appearences[typeof(GridControlCardViewCaptionExpandButtonElement)] = new string[] {"CardExpandButton"};
			this.appearences[typeof(GridControlCardViewQuickCustomizeButtonElement)] = new string[] {};
			this.appearences[typeof(GridControlCardViewSeparateLineElement)] = new string[] {"SeparatorLine"};
			this.appearences[typeof(GridControlLayoutViewCaptionElement)] = new string[] { "FocusedCardCaption", "CardCaption", "HideSelectionCardCaption", "SelectedCardCaption" };
			this.appearences[typeof(GridControlLayoutViewFieldCaptionElement)] = new string[] { "FieldCaption" };
			this.appearences[typeof(GridControlLayoutViewFieldValueElement)] = new string[] { "FieldValue" };
			this.appearences[typeof(WinExplorerViewGroupElement)] = new string[] { "GroupNormal", "GroupHovered", "GroupPressed" };
			this.appearences[typeof(WinExplorerViewItemTextElement)] = new string[] { "ItemNormal", "ItemHovered", "ItemPressed" };
			this.appearences[typeof(WinExplorerViewItemDescriptionElement)] = new string[] { "ItemDescriptionNormal", "ItemDescriptionHovered", "ItemDescriptionPressed" };
		}
		protected override void OnFocusedElementChanged() {
			base.OnFocusedElementChanged();
			string[] strings = new string[0];
			if(SelectedElement != null) {
				strings = (string[])this.appearences[SelectedElement.GetType()];
				if(strings == null)
					strings = new string[0];
			}
			RaiseAppearanceCollectionChanged(strings);
		}
		void RaiseAppearanceCollectionChanged(string[] appearances) {
			if(AppearanceCollectionChanged != null)
				AppearanceCollectionChanged(appearances, EventArgs.Empty);
		}
	}
}
