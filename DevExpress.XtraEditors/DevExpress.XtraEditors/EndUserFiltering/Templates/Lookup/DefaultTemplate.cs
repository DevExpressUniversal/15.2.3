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
using System.Drawing;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraEditors.Filtering.Templates.Lookup {
	[Browsable(false), ToolboxItem(false)]
	public partial class DefaultTemplate : XtraUserControl, IXtraResizableControl {
		ListAutoSizeHelper helper;
		public DefaultTemplate() {
			InitializeComponent();
			helper = new ListAutoSizeHelper(Part_Values, Part_MoreButton, Part_FewerButton);
		}
		protected override void OnParentBackColorChanged(EventArgs e) {
			base.OnParentBackColorChanged(e);
			System.Windows.Forms.Control parent = Parent;
			while(parent != null) {
				if(parent.BackColor != Color.Transparent) {
					Part_Values.BackColor = parent.BackColor;
					break;
				}
				parent = parent.Parent;
			}
		}
		Size IXtraResizableControl.MinSize {
			get { return new Size(ListAutoSizeHelper.DefaultMinWidth, helper.GetBestHeight(helper.CalcAreButtonsAvailable(this), Padding.Vertical)); }
		}
		Size IXtraResizableControl.MaxSize {
			get { return new Size(0, helper.GetBestHeight(helper.CalcAreButtonsAvailable(this), Padding.Vertical)); }
		}
	}
	class ListAutoSizeHelper {
		CheckedListBoxControl listbox;
		SimpleButton moreButton;
		SimpleButton fewerButton;
		public ListAutoSizeHelper(CheckedListBoxControl list, SimpleButton more, SimpleButton fewer) {
			listbox = list;
			fewerButton = fewer;
			moreButton = more;
		}
		public bool CalcAreButtonsAvailable(XtraUserControl template) {
			FilterUIEditorContainerEdit containerEdit = template.Parent as FilterUIEditorContainerEdit; 
			if(containerEdit != null) {
				var collectionViewModel = containerEdit.EditValue as DevExpress.Utils.Filtering.Internal.ICollectionValueViewModel;
				if(collectionViewModel != null)
					return collectionViewModel.IsLoadMoreAvailable || collectionViewModel.IsLoadFewerAvailable;
			}
			return true;
		}
		public int GetBestHeight(bool buttonsAvailable, int paddingVertical) {
			int listHeightWithMargin = buttonsAvailable ? 110 : 226;
			int buttonHeightWithMargin = buttonsAvailable ? 22 : 0;
			if(listbox.IsHandleCreated) {
				var bestSize = listbox.CalcBestSize();
				if(listbox.MultiColumn && bestSize.Width * 2 < DefaultMinWidth) {
					int borderSize = bestSize.Height - listbox.ViewInfo.ItemHeight * listbox.ItemCount;
					int maxRows = MaxRowCountForSingleColumnMode;
					if(listbox.ItemCount < 16 && listbox.ItemCount > 5)
						maxRows = (listbox.ItemCount + 1) / 2;
					bestSize.Height = Math.Max(listbox.ViewInfo.ItemHeight * maxRows + borderSize, bestSize.Height / 2);
				}
				listHeightWithMargin = bestSize.Height + listbox.Margin.Vertical;
			}
			if(moreButton.IsHandleCreated || fewerButton.IsHandleCreated)
				buttonHeightWithMargin = Math.Max(moreButton.Height + moreButton.Margin.Vertical, fewerButton.Height + fewerButton.Margin.Vertical);
			return buttonsAvailable ?
				(listHeightWithMargin + buttonHeightWithMargin + paddingVertical) :
				(listHeightWithMargin + paddingVertical);
		}
		public const int MaxRowCountForSingleColumnMode = 10;
		public const int DefaultMinWidth = 240;
	}
}
