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
using System.Windows;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public class ButtonEditPropertyProvider : TextEditPropertyProvider {
		public static readonly DependencyProperty NullValueButtonPlacementProperty;
		public static readonly DependencyProperty ShowLeftButtonsProperty;
		public static readonly DependencyProperty ShowRightButtonsProperty;
		public static readonly DependencyProperty IsTextEditableProperty;
		new ButtonEditStyleSettings StyleSettings { get { return base.StyleSettings as ButtonEditStyleSettings; } }
		public ButtonEditPropertyProvider(TextEdit edit) : base(edit) {
		}
		static ButtonEditPropertyProvider() {
			Type ownerType = typeof(ButtonEditPropertyProvider);
			NullValueButtonPlacementProperty = DependencyPropertyManager.Register("NullValueButtonPlacement", typeof(EditorPlacement), ownerType, new PropertyMetadata(null));
			ShowLeftButtonsProperty = DependencyPropertyManager.Register("ShowLeftButtons", typeof(bool), ownerType);
			ShowRightButtonsProperty = DependencyPropertyManager.Register("ShowRightButtons", typeof(bool), ownerType);
			IsTextEditableProperty = DependencyProperty.Register("IsTextEditable", typeof(bool), ownerType, new PropertyMetadata(true));
		}
		new ButtonEdit Editor { get { return (ButtonEdit)base.Editor; } }
		public bool IsTextEditable {
			get { return (bool)GetValue(IsTextEditableProperty); }
			set { SetValue(IsTextEditableProperty, value); }
		}
		public EditorPlacement NullValueButtonPlacement {
			get { return (EditorPlacement)GetValue(NullValueButtonPlacementProperty); }
			set { SetValue(NullValueButtonPlacementProperty, value); }
		}
		public bool ShowLeftButtons {
			get { return (bool)GetValue(ShowLeftButtonsProperty); }
			set { SetValue(ShowLeftButtonsProperty, value); }
		}
		public bool ShowRightButtons {
			get { return (bool)GetValue(ShowRightButtonsProperty); }
			set { SetValue(ShowRightButtonsProperty, value); }
		}
		public override EditorPlacement GetNullValueButtonPlacement() {
			return Editor.NullValueButtonPlacement ?? EditorPlacement.None;
		}
		public virtual bool GetActualAllowDefaultButton(ButtonEdit editor) {
			return editor.AllowDefaultButton ?? StyleSettings.GetActualAllowDefaultButton(editor);
		}
		public virtual void SetIsTextEditable(ButtonEdit editor) {
			IsTextEditable = Editor.IsTextEditable ?? StyleSettings.GetIsTextEditable(editor);
		}
	}
}
