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

using DevExpress.Utils;
using DevExpress.XtraTreeList.Localization;
using System;
using System.Windows.Forms;
namespace DevExpress.XtraTreeList {
	public class TreeListFindPanelOwner : FindPanelOwnerBase {
		protected TreeList TreeList { get; private set; }
		public TreeListFindPanelOwner(TreeList treeList) {
			TreeList = treeList;
		}
		public override void FocusOwner() {
			TreeList.FocusedNode = TreeList.Nodes.FirstNodeEx;
			TreeList.Focus();
		}
		public override void InitButtons() {
			FindPanel.FindEdit.Properties.NullValuePrompt = TreeList.OptionsFind.FindNullPrompt;
			FindPanel.FindEdit.Properties.NullValuePromptShowForEmptyValue = true;
			FindPanel.FindEdit.Properties.ShowNullValuePromptWhenFocused = true;
			FindPanel.FindButton.Text = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.FindControlFindButton);
			FindPanel.ClearButton.Text = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.FindControlClearButton);
			FindPanel.AdjustButtonSize(FindPanel.FindButton);
			FindPanel.AdjustButtonSize(FindPanel.ClearButton);
			FindPanel.AdjustCloseButtonSize();
			FindPanel.FindButton.Visible = TreeList.OptionsFind.ShowFindButton;
			FindPanel.CloseButton.Visible = TreeList.OptionsFind.ShowCloseButton && !TreeList.OptionsFind.AlwaysVisible;
			FindPanel.ClearButton.Visible = TreeList.OptionsFind.ShowClearButton;
			if(TreeList.IsDesignMode) {
				FindPanel.FindEdit.Enabled = false;
				FindPanel.ClearButton.Enabled = false;
				FindPanel.FindButton.Enabled = false;
			}
		}
		public override void FocusFindEditor() {
			if(TreeList.TryCloseActiveEditor())
				FindPanel.FindEdit.Focus();
			else
				TreeList.Focus();
		}
		public override void HideFindPanel() {
			if(TreeList.OptionsFind.AlwaysVisible)
				return;
			TreeList.HideFindPanel();
		}
		public override void UpdateTimer(Timer updateTimer) {
			updateTimer.Interval = TreeList.OptionsFind.FindDelay;
		}
		public override bool ApplyFindFilter(string text) {
			return TreeList.ApplyFindFilter(text);
		}
		protected override void FindPanelChanged() {
			FindPanel.LookAndFeel.ParentLookAndFeel = TreeList.ElementsLookAndFeel;
			InitButtons();
		}
	}
}
