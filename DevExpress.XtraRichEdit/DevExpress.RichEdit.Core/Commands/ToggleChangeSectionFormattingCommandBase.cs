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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleChangeSectionFormattingCommandBase (abstract class)
	public abstract class ToggleChangeSectionFormattingCommandBase<T> : ChangeSectionFormattingCommandBase<T> where T : struct {
		protected ToggleChangeSectionFormattingCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (ActivePieceTable.IsMain)
				state.Checked = IsChecked();
		}
		protected internal virtual bool IsChecked() {
			List<SelectionItem> items =  DocumentModel.Selection.Items;
			int count = items.Count;
			bool result = true;
			for (int i = 0; i < count; i++) {
				SelectionItem item = items[i];
				DocumentModelPosition start = CalculateStartPosition(item, false);
				DocumentModelPosition end = CalculateEndPosition(item, false);
				SectionPropertyModifier<T> modifier = CreateModifier(CreateDefaultCommandUIState());
				result &= IsCheckedCore(start.LogPosition, end.LogPosition, modifier);
			}
			return result;
		}
		protected internal virtual bool IsCheckedCore(DocumentLogPosition logPositionFrom, DocumentLogPosition logPositionTo, SectionPropertyModifier<T> modifier) {
			int length = Math.Max(1, logPositionTo - logPositionFrom);
			Nullable<T> value = ActivePieceTable.ObtainSectionsPropertyValue(logPositionFrom, length, modifier);
			if (!value.HasValue)
				return false;
			else
				return IsCheckedValue(value.Value);
		}
		protected internal abstract bool IsCheckedValue(T value);
	}
	#endregion
}
