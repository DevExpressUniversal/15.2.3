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

using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet {
	public class DocumentsMailmergeProcessor :MailMergeProcessor {
		bool unlockFinishList;
		public DocumentsMailmergeProcessor(Worksheet templateSheet, MailMergeOptions options)
			: base(templateSheet, options) {
			unlockFinishList = true;
		}
		protected override bool NeedFinishList {
			get { return RootInfo == null && unlockFinishList; }
		}
		public override List<DocumentModel> Process() {
			DocumentModel targetBook;
			List<DocumentModel> result = new List<DocumentModel>();
			while (!DataAdapter.LastRowProcessed) {
				targetBook = new DocumentModel();
				targetBook.MailMergeProcessor = this;
				DetectedTrackedPositions();
				targetBook.Sheets.Add(new Worksheet(targetBook));
				GenerateContent(targetBook.ActiveSheet);
				result.Add(targetBook);
			}
			return result;
		}
		protected override void ProcessDetailRange(Worksheet targetSheet, CellRange sourceRange) {
			if (DataAdapter.IsSorted) {
				while (RootInfo != null)
					base.ProcessDetailRange(targetSheet, sourceRange);
			}
			else {
				unlockFinishList = false;
				base.ProcessDetailRange(targetSheet, sourceRange);
			}
		}
	}
}
