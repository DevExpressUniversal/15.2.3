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

using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.Snap.Core.Native.Data {
	public class FieldContextController {		
		SnapDataContext currentDataContext;
		public IFieldContext GetDataContextForField(SnapPieceTable pieceTable, Field field) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			IFieldContext dataContext = FindDataContext(pieceTable, field);
			return dataContext;
		}
		IFieldContext FindDataContext(SnapPieceTable pieceTable, Field field) {
			SnapBookmark bookmark = new SnapBookmarkController(pieceTable).FindInnermostTemplateBookmarkByPosition(field.Code.Start);
			if (bookmark != null)
				return bookmark.FieldContext;
			return pieceTable.DocumentModel.GetRootDataContext();
		}		
		private object GetColumnValue(object source, object fieldDataSource, string propertyName) {
			return new SnapFieldDataSource(source).GetFieldValue(propertyName);
		}			   
		public void EndDataIteration() {
			if (currentDataContext != null) {
				currentDataContext.Dispose();
				currentDataContext = null;
			}
		}
	}
}
