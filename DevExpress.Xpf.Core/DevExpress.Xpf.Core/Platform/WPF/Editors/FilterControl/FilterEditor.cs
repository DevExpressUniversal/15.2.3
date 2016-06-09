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

using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Editors.Filtering {
	public partial class InplaceFilterEditor : InplaceEditorBase {
		protected override IBaseEdit CreateEditor(BaseEditSettings settings) {
			IBaseEdit editor = base.CreateEditor(settings);
			ClearEditorValidation(editor);
			if(editor is TextEdit)
				(editor as TextEdit).ShowTooltipForTrimmedText = false;
			ShowValueEditorEventArgs arg = new ShowValueEditorEventArgs((BaseEdit)editor, Node, new OperandValue(this.Data.Value));
			((FilterControl)Owner.OwnerElement).RaiseBeforeShowValueEditor(arg);
			if(arg.CustomEditSettings != null)
				editor = base.CreateEditor(arg.CustomEditSettings);
			editor.ShouldDisableExcessiveUpdatesInInplaceInactiveMode = false;
			return editor;
		}
		void SetContentAlignment() { }
	}
}
