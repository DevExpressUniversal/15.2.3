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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Spreadsheet;
using DevExpress.Office.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	public abstract class PasteContentBase : PasteCommandBase {
		PasteContentWorksheetCommand innerCommand;
		protected PasteContentBase(ISpreadsheetControl control)
			: base(control) {
				this.innerCommand = CreateInnerCommandCore();
		}
		protected internal PasteContentWorksheetCommand InnerCommand { get { return innerCommand; } }
		public override DocumentFormat Format { get { return InnerCommand.Format; } }
		public override PasteSource PasteSource { get { return InnerCommand.PasteSource; } set { InnerCommand.PasteSource = value; } }
		public override ModelPasteSpecialOptions PasteOptions { get { return InnerCommand.PasteOptions; } set { InnerCommand.PasteOptions = value; } }
		protected internal abstract PasteContentWorksheetCommand CreateInnerCommandCore();
		protected internal override void PerformModifyModel() {
			InnerCommand.ShowUnprotectRangeForm = Control.InnerControl.ShowUnprotectRangeForm;
			InnerCommand.Execute();
		}
		protected internal override bool IsDataAvailable() {
			return InnerCommand.IsDataAvailable();
		}
		protected internal override bool ChangeSelection() {
			if (InnerCommand.RangeToSelect == null)
				return false;
			Selection.SetSelection(InnerCommand.RangeToSelect, false);
			return true;
		}
	}
}
