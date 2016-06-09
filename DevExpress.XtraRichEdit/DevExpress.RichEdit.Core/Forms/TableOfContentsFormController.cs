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

using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Fields;
namespace DevExpress.XtraRichEdit.Forms {
	public class TOCFormControllerParameters : FormControllerParameters {
		readonly Field tocField;
		internal TOCFormControllerParameters(IRichEditControl control, Field tocField)
			: base(control) {
				Guard.ArgumentNotNull(tocField, "tocField");
				this.tocField = tocField;
		}
		internal Field TocField { get { return tocField; } }
	}
	public class TableOfContentsFormController:FormController {
		 PieceTable pieceTable;
		 Field field;
		 TocField parsedInfo;
		public TableOfContentsFormController(TOCFormControllerParameters parameters) {
			pieceTable = parameters.TocField.PieceTable;
			FieldCalculatorService parser = pieceTable.CreateFieldCalculatorService();
			parsedInfo = (TocField)parser.ParseField(pieceTable, parameters.TocField);
			field = parameters.TocField;
			UseHyperlinks = parsedInfo.Options.CreateHyperlinks;
			ShowPageNumbers = parsedInfo.Options.NoPageNumberLevels.Count == 0;
			RightAlignPageNumbers = parsedInfo.Options.PageNumberSeparator == null || parsedInfo.Options.PageNumberSeparator.Length == 0;
			ShowLevels = parsedInfo.Options.HeaderLevels.Count;
		}
		public bool ShowPageNumbers { get; set; }
		public bool RightAlignPageNumbers { get; set; }
		public bool UseHyperlinks { get; set; }
		public int ShowLevels { get; set; }
		public override void ApplyChanges() {
			InstructionController controller = new InstructionController(pieceTable, parsedInfo, field);
			if (ShowPageNumbers) controller.RemoveSwitch("n");
			else controller.SetSwitch("n");
			if (RightAlignPageNumbers) controller.RemoveSwitch("p");
			else controller.SetSwitch("p"," ", false);
			if (UseHyperlinks) controller.SetSwitch("h");
			else controller.RemoveSwitch("h");
			if (ShowLevels != 9)
			controller.SetSwitch("o",String.Format("1-{0}",ShowLevels));
			controller.ApplyDeferredActions();
		}
	}
}
