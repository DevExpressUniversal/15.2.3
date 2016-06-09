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

using System.Windows.Media;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Validation.Native;
namespace DevExpress.Xpf.Editors {
	public class TextInputColorEditAutoCompleteSettings : TextInputAutoCompleteSettingsBase {
		public TextInputColorEditAutoCompleteSettings(TextEditBase editor) : base(editor) {
		}
		protected internal override object ProvideEditValue(object editValue, UpdateEditorSource updateSource) {
			object displayValue = GetDisplayValue(editValue);
			Color result;
			if (Text2ColorHelper.TryConvert(displayValue, out result))
				return new SolidColorBrush(result);
			return base.ProvideEditValue(editValue, updateSource);
		}
		protected override object GetEditValueForSyncWithEditor() {
			var valueCandidate = base.GetEditValueForSyncWithEditor();
			LookUpEditableItem item = new LookUpEditableItem() {
				DisplayValue = valueCandidate,
				EditValue = OwnerEdit.EditValue,
			};
			return item;
		}
	}
}
