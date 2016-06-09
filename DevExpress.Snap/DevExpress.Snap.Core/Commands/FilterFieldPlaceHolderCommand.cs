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
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native.Data;
namespace DevExpress.Snap.Core.Commands {
	[CommandLocalization(Localization.SnapStringId.FilterFieldCommand_MenuCaption, Localization.SnapStringId.FilterFieldCommand_Description)]
	public class FilterFieldPlaceHolderCommand : DropDownCommandBase {
		public FilterFieldPlaceHolderCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId[] GetChildCommandIds() {
			return new RichEditCommandId[] { SnapCommandId.SnapFilterField };
		}
		public override string ImageName { get { return "QuickFilter"; } }
		protected override bool IsEnabled() {
			return IsInDetailList() ? false : base.IsEnabled();
		}
		internal bool IsInDetailList() {
			ListFieldSelectionController controller = new ListFieldSelectionController(DocumentModel);
			SnapFieldInfo fieldInfo = controller.FindDataField();
			if (fieldInfo == null)
				return false;
			SnapFieldCalculatorService fieldCalculator = new SnapFieldCalculatorService();
			IFieldPathService fieldPathService = GetFieldPathService();
			if (fieldPathService == null)
				return false;
			Field snListField = GetParentSNList(fieldInfo.PieceTable, fieldInfo.Field);
			if (snListField != null && HasParentSnListField(fieldInfo.PieceTable, snListField)) {
				SNListField calculatedField = fieldCalculator.ParseField(fieldInfo.PieceTable, snListField) as SNListField;
				FieldPathInfo fieldPathInfo = fieldPathService.FromString(calculatedField.DataSourceName);
				if (fieldPathInfo.DataSourceInfo.FieldDataSourceType == FieldDataSourceType.Relative)
					return true;
			}
			return false;
		}
		IFieldPathService GetFieldPathService() {
			IFieldDataAccessService fieldDataAccessService = DocumentModel.GetService<IFieldDataAccessService>();
			return fieldDataAccessService != null ? fieldDataAccessService.FieldPathService : null;
		}
		bool HasParentSnListField(PieceTable pieceTable, Field field) {
			return GetParentSNList(pieceTable, field) != null;
		}
		Field GetParentSNList(PieceTable pieceTable, Field field) {
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			for (; field.Parent != null; field = field.Parent) {
				Field parentField = field.Parent;
				SNListField calculatedField = calculator.ParseField(pieceTable, parentField) as SNListField;
				if (calculatedField != null)
					return parentField;
			}
			return null;
		}
	}
}
