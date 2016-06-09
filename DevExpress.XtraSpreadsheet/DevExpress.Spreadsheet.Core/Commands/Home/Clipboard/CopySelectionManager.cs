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
using System.IO;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
#if !DXPORTABLE
using PlatformIndependentDataObject = System.Windows.Forms.DataObject;
using System.Windows.Forms;
#else
using PlatformIndependentDataObject = DevExpress.Compatibility.System.Windows.Forms.DataObject;
using DevExpress.Compatibility.System.Windows.Forms;
#endif
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	public class CopySelectionManager : DevExpress.Spreadsheet.IClipboardManager {
		DocumentModel documentModel;
		public CopySelectionManager(DocumentModel documentModel, bool a) {
			this.documentModel = documentModel;
		}
		public DocumentModel DocumentModel {[System.Diagnostics.DebuggerStepThrough] get { return documentModel; } }
		#region IClipboardManager members
		void DevExpress.Spreadsheet.IClipboardManager.Copy() {
			if (DocumentModel.ActiveSheet.Selection.SelectedRanges.Count == 0)
				return;
			CellRange range = DocumentModel.ActiveSheet.Selection.SelectedRanges[0];
			DocumentModel.CopyCutRangeToClipboard(range, false);
		}
		#endregion
	}
}
