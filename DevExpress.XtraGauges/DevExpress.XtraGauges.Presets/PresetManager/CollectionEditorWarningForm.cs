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
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors;
namespace DevExpress.XtraGauges.Presets.PresetManager {
	public partial class CollectionEditorWarningForm : XtraMessageBoxForm {
		protected override string GetButtonText(DialogResult target) {
			switch(target) {
				case DialogResult.Yes:
					return "Open Visual Designer...";
				case DialogResult.No:
					return "Open Collection Editor...";
				case DialogResult.Cancel:
					return "Cancel";
			}
			return base.GetButtonText(target);
		}
	}
	public static class CollectionEditorWarning {
		static string text = "We suggest that you use the Visual Designer to edit this collection.\r\n" +
			"Editing the collection via the Collection Editor is strongly not recommended.\r\n" +
			"Please choose one of the following:";
		public static DialogResult Show(string caption) {
			using(CollectionEditorWarningForm form = new CollectionEditorWarningForm()) {
				return form.ShowMessageBoxDialog(
					new XtraMessageBoxArgs(null, text, caption,
					new DialogResult[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel },
					System.Drawing.SystemIcons.Information, 0));
			}
		}
	}
}
