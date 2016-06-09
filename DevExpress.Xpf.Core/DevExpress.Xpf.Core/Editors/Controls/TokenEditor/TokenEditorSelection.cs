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
using System.Collections.Generic;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Editors.Internal {
	public class TokenEditorSelection {
		public TokenEditorSelection(TokenEditor owner) {
			Owner = owner;
			SelectedTokensIndexes = new List<int>();
		}
		public List<int> SelectedTokensIndexes { get; private set; }
		public int StartSelectionIndex { get; private set; }
		public bool HasSelectedTokens { get { return SelectedTokensIndexes.Count > 0; } }
		TokenEditor Owner { get; set; }
		public void SetSelectionFromFocusedToken() {
			if (!resetSelectionLocker.IsLocked) {
				SelectedTokensIndexes = new List<int>();
				if (Owner.FocusedToken != null && !Owner.IsDefaultTokenFocused()) {
				  var index = Owner.EditableIndexOfToken(Owner.FocusedToken);
				  SelectTokenByIndex(index);
				  StartSelectionIndex = index;
				}
			}
			UpdateSelectedTokens();
		}
		public void SelectTokenByIndex(int index) {
			if (!SelectedTokensIndexes.Contains(index))
				SelectedTokensIndexes.Add(index);
		}
		public void UpdateSelectedTokens() {
			var visibleTokens = Owner.GetVisibleTokens();
			if (visibleTokens != null) {
				foreach (int index in visibleTokens.Keys) {
					var token = visibleTokens[index] as TokenEditorPresenter;
					token.IsSelected = SelectedTokensIndexes.Contains(index);
				}
			}
		}
		public void SelectToken(TokenEditorPresenter token) {
			int index = Owner.EditableIndexOfToken(token);
			if (SelectedTokensIndexes.Contains(index))
				SelectedTokensIndexes.Remove(index);
			else
				SelectTokenByIndex(index);
			UpdateSelectedTokens();
		}
		Locker resetSelectionLocker = new Locker();
		public void LockSelection(Action action) {
			resetSelectionLocker.DoLockedAction(action);
		}
		public void ResetSelection() {
			SelectedTokensIndexes = new List<int>();
			UpdateSelectedTokens();
		}
		public void SelectTokenByIndexWithUpdate(int index) {
			SelectTokenByIndex(index);
			UpdateSelectedTokens();
		}
		public void RemoveTokenFromSelection(int index) {
			if (SelectedTokensIndexes.Contains(index)) SelectedTokensIndexes.Remove(index);
			UpdateSelectedTokens();
		}
	}
}
