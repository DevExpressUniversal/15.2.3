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
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.Xpf.Editors {
	public class ValueEditingProcessor {
		NullableContainer displayText = new NullableContainer();
		List<ValueEditingActionBase> EditingActions { get; set; }
		ValueEditingActionBase CurrentAction { get; set; }
		ValueEditingData EditingData { get; set; }
		public string DisplayText { get { return displayText.HasValue ? (string)displayText.Value : string.Empty; } }
		public void SetDisplayText(string text) {
			displayText.SetValue(text);
		}
		public ValueEditingProcessor() {
			EditingActions = new List<ValueEditingActionBase>();
			EditingData = new ValueEditingData();
		}
		public void Register(ValueEditingActionBase valueEditingAction) {
			EditingActions.Add(valueEditingAction);
		}
	}
	public class ValueEditingData {
		public string Text { get; set; }
	}
	public abstract class ValueEditingActionBase {
		Func<string, string> UpdateTextAction { get; set; }
		public ValueEditingActionBase(Func<string, string> updateTextAction) {
			Guard.ArgumentNotNull(updateTextAction, "updateTextAction");
			UpdateTextAction = updateTextAction;
		}
		public abstract string Process(string text, ValueEditingData data);
	}
	public class ValueEditingAction : ValueEditingActionBase {
		public ValueEditingAction(Func<string, string> updateTextAction)
			: base(updateTextAction) {
		}
		public override string Process(string text, ValueEditingData data) {
			throw new NotImplementedException();
		}
	}
}
