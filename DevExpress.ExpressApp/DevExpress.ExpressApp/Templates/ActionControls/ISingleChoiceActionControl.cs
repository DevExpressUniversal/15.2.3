#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Actions;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Templates.ActionControls {
	public interface ISingleChoiceActionControl : IActionControl {
		void SetShowItemsOnClick(bool value);
		void SetChoiceActionItems(ChoiceActionItemCollection choiceActionItems);
		void SetSelectedItem(ChoiceActionItem selectedItem);
		void Update(IDictionary<object, ChoiceActionItemChangesType> itemsChangedInfo);
		event EventHandler<SingleChoiceActionControlExecuteEventArgs> Execute;
	}
	public class SingleChoiceActionControlExecuteEventArgs : EventArgs {
		private bool isDefaultChoiceActionItem;
		private ChoiceActionItem choiceActionItem;
		public SingleChoiceActionControlExecuteEventArgs() {
			isDefaultChoiceActionItem = true;
		}
		public SingleChoiceActionControlExecuteEventArgs(ChoiceActionItem choiceActionItem) {
			Guard.ArgumentNotNull(choiceActionItem, "choiceActionItem");
			this.choiceActionItem = choiceActionItem;
		}
		public bool IsDefaultChoiceActionItem {
			get { return isDefaultChoiceActionItem; }
		}
		public ChoiceActionItem ChoiceActionItem {
			get { return choiceActionItem; }
		}
	}
}
