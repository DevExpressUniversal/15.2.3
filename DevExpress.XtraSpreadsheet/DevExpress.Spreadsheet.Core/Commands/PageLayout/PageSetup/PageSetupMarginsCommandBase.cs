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
using DevExpress.Office;
using DevExpress.Office.Design.Internal;
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PageSetupMarginsCommandBase (abstract class)
	public abstract class PageSetupMarginsCommandBase : SpreadsheetMenuItemSimpleCommand {
		#region Fields
		readonly MarginsInfo predefinedValue;
		#endregion
		protected PageSetupMarginsCommandBase(ISpreadsheetControl control)
			: base(control) {
			this.predefinedValue = CreatePredefinedValue();
		}
		#region Properties
		public MarginsInfo PredefinedValue { get { return predefinedValue; } }
		public override string MenuCaption {
			get {
				DocumentUnit unit = DocumentServer.UIUnit;
				string format = OfficeLocalizer.GetString(OfficeMenuCaptionStringId);
				UIUnitConverter unitConverter = new UIUnitConverter(UnitPrecisionDictionary.DefaultPrecisions);
				return String.Format(format,
					unitConverter.ToUIUnit(predefinedValue.Left, unit).ToString(),
					unitConverter.ToUIUnit(predefinedValue.Top, unit).ToString(),
					unitConverter.ToUIUnit(predefinedValue.Right, unit).ToString(),
					unitConverter.ToUIUnit(predefinedValue.Bottom, unit).ToString());
			}
		}
		#endregion
		protected abstract MarginsInfo CreatePredefinedValue();
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				Margins margins = ActiveSheet.Margins;
				margins.BeginUpdate();
				try {
					margins.Left = predefinedValue.Left;
					margins.Right = predefinedValue.Right;
					margins.Top = predefinedValue.Top;
					margins.Bottom = predefinedValue.Bottom;
					margins.Header = predefinedValue.Header;
					margins.Footer = predefinedValue.Footer;
				}
				finally {
					margins.EndUpdate();
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable && !InnerControl.IsAnyInplaceEditorActive;
			state.Visible = true;
			state.Checked = ActiveSheet.Margins.Info.Equals(predefinedValue);
		}
	}
	#endregion
}
