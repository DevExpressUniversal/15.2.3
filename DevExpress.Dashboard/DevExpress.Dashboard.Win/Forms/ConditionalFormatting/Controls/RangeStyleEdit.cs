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

using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.DashboardWin.Native {
	class RangeStyleEdit : ButtonEdit {
		const int OffsetX = 6;
		readonly PopupMenu popupMenu;
		readonly RangeSetFormatRuleMenuCreatorBase menuCreator;
		public event FormatRuleRangeSetPredefinedStyleChangedEventHandler PredefinedStyleChanged;
		public FormatConditionRangeSetPredefinedType PredefinedStyleType {
			get { return this.menuCreator.SelectedType; }
			set { 
				this.menuCreator.SelectedType = value;
				UpdateText(value);
				Refresh();
			}
		}
		public RangeStyleEdit(BarManager barManager, StyleMode styleMode) {
			Properties.Buttons[0].Kind = ButtonPredefines.Down;
			Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			MinimumSize = new Size(100, 26);
			this.popupMenu = new PopupMenu(barManager);
			if(styleMode == StyleMode.Icon) {
				this.menuCreator = new RangeIconSetFormatRuleMenuCreator(LookAndFeel);
			} else {
				this.menuCreator = new RangeColorSetFormatRuleMenuCreator(LookAndFeel);
			}
			this.popupMenu.ItemLinks.AddRange(this.menuCreator.Initialize((BarItem barItem) => {
				if(PredefinedStyleChanged != null)
					PredefinedStyleChanged(this, new FormatRuleRangeSetPredefinedStyleChangedEventArgs(PredefinedStyleType));
				UpdateText(PredefinedStyleType);
				Refresh();
			}));
			UpdateText(PredefinedStyleType);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			BarCheckItem selectedItem = this.menuCreator.SelectedItem;
			if(selectedItem != null)
				e.Graphics.DrawImage(selectedItem.Glyph, OffsetX, (Height - selectedItem.Glyph.Height) / 2);
		}
		protected override void OnClick(EventArgs e) {
			popupMenu.ShowPopup(PointToScreen(new Point(0, Height)));
			base.OnClick(e);
		}
		void UpdateText(FormatConditionRangeSetPredefinedType predefinedStyleType) {
			Text = (predefinedStyleType == FormatConditionRangeSetPredefinedType.Custom || predefinedStyleType == FormatConditionRangeSetPredefinedType.None) ? DashboardWinLocalizer.GetString(DashboardWinStringId.EditorCustomValue) : string.Empty;
		}
	}
}
