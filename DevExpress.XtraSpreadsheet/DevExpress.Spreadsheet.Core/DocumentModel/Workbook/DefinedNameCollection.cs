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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class DocumentModel {
		#region CheckSheetCount
		public void CheckSheetCount() {
			if(Sheets.Count == 0)
				throw new InvalidOperationException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorZeroCountSheets));
		}
		#endregion
		#region CreateDefinedName
		public DefinedName CreateDefinedName(string name, string reference) {
			CheckDefinedName(name, -1);
			DefinedNameWorkbookCreateCommand command = new DefinedNameWorkbookCreateCommand(this, name, reference);
			command.Execute();
			return (DefinedName)command.Result;
		}
		public DefinedName CreateDefinedName(string name, ParsedExpression expression) {
			DefinedNameWorkbookCreateCommand command = new DefinedNameWorkbookCreateCommand(this, name, expression);
			command.Execute();
			return (DefinedName)command.Result;
		}
		#endregion
		#region RemoveDefinedName
		public void RemoveDefinedName(string name) {
			if(DefinedNames.Contains(name))
				RemoveDefinedName((DefinedName)DefinedNames[name]);
		}
		public void RemoveDefinedName(DefinedName definedName) {
			if (!DefinedNames.Contains(definedName.Name))
				return;
			DefinedNameWorkbookRemoveCommand command = new DefinedNameWorkbookRemoveCommand(this, definedName);
			command.Execute();
		}
		#endregion
		#region ClearDefinedNames
		public void ClearDefinedNames() {
			DefinedNameWorkbookCollectionClearCommand command = new DefinedNameWorkbookCollectionClearCommand(this);
			command.Execute();
		}
		#endregion
	}
}
