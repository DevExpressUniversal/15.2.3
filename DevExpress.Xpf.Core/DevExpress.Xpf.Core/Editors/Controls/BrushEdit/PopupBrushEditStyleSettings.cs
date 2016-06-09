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
using System.Drawing;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public abstract class PopupBrushEditStyleSettingsBase : PopupBaseEditStyleSettings {
		public abstract BaseEditStyleSettings CreateBrushEditStyleSettings();
		public override PopupFooterButtons GetPopupFooterButtons(PopupBaseEdit editor) {
			return PopupFooterButtons.OkCancel;
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			PopupBrushEditBase brushEditBase = editor as PopupBrushEditBase;
			if (brushEditBase == null)
				return;
			brushEditBase.IsTextEditable = false;
			if (!brushEditBase.IsPropertySet(PopupBrushEditBase.PopupMinWidthProperty))
				brushEditBase.PopupMinWidth = 300;
		}
		protected internal override bool GetShowSizeGrip(PopupBaseEdit editor) {
			return true;
		}
	}
	public class PopupBrushEditStyleSettings : PopupBrushEditStyleSettingsBase {
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			PopupBrushEditBase brushEditBase = editor as PopupBrushEditBase;
			if (brushEditBase == null)
				return;
			brushEditBase.BrushType = BrushType.AutoDetect;
		}
		public override BaseEditStyleSettings CreateBrushEditStyleSettings() {
			return null;
		}
	}
	public class PopupSolidColorBrushEditStyleSettings : PopupBrushEditStyleSettingsBase {
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			PopupBrushEditBase brushEditBase = editor as PopupBrushEditBase;
			if (brushEditBase == null)
				return;
			brushEditBase.BrushType = BrushType.SolidColorBrush;
		}
		public override BaseEditStyleSettings CreateBrushEditStyleSettings() {
			return new SolidColorBrushEditStyleSettings();
		}
	}
	public class PopupLinearGradientBrushEditStyleSettings : PopupBrushEditStyleSettingsBase {
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			PopupBrushEditBase brushEditBase = editor as PopupBrushEditBase;
			if (brushEditBase == null)
				return;
			brushEditBase.BrushType = BrushType.LinearGradientBrush;
			brushEditBase.IsTextEditable = false;
		}
		public override BaseEditStyleSettings CreateBrushEditStyleSettings() {
			return null;
		}
	}
	public class PopupRadialGradientBrushEditStyleSettings : PopupBrushEditStyleSettingsBase {
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			PopupBrushEditBase brushEditBase = editor as PopupBrushEditBase;
			if (brushEditBase == null)
				return;
			brushEditBase.BrushType = BrushType.RadialGradientBrush;
			brushEditBase.IsTextEditable = false;
		}
		public override BaseEditStyleSettings CreateBrushEditStyleSettings() {
			return null;
		}
	}
}
