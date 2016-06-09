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
namespace DevExpress.XtraSpreadsheet.Model {
	#region DocumentModelChangeActions
	[Flags]
	public enum DocumentModelChangeActions {
		None = DevExpress.Office.DrawingML.DocumentModelChangeActions.None,
		Redraw = 0x00000001,
		RaiseContentChanged = 0x00000002,
		PerformActionsOnIdle = 0x00000004,
		RaiseEmptyDocumentCreated = 0x00000008,
		RaiseDocumentLoaded = 0x00000010,
		ResetSelectionLayout = 0x00000020,
		ResetAllLayout = 0x00000040,
		ResetSpellingCheck = 0x00000080,
		RaiseSelectionChanged = 0x00000100,
		SuppressBindingsNotifications = 0x00000200,
		RaiseModifiedChanged = 0x00000400,
		RaisePivotTableChanged = 0x00000800,
		ResetHeaderContent = 0x00001000,
		ResetHeaderLayout = 0x00002000,
		RaiseUpdateUI = 0x00004000,
		RaiseSchemaChanged = 0x00008000,
		RaiseContentVersionChanged = 0x00010000,
		RaiseParameterSelectionChanged = 0x00020000,
		ResetCachedContentVersions = 0x00040000,
		ResetCachedTransactionVersions = 0x00080000,
		ResetControlsLayout = 0x00100000,
		ResetInvalidDataCircles = 0x00200000,
		ResetPivotTableFieldsPanelVisibility = 0x00400000,
		UpdateTransactedVersionInCopiedRange = 0x00800000,
		ClearHistory = 0x01000000
	}
	#endregion
	#region DocumentModelChangeType
	public enum DocumentModelChangeType {
		None = 0,
		CreateEmptyDocument,
		LoadNewDocument,
		FreezePanes,
		DrawingAdded,
		DrawingRemoved
	}
	#endregion
	public static class DocumentModelChangeActionsCalculator {
		internal class DocumentModelChangeActionsTable : Dictionary<DocumentModelChangeType, DocumentModelChangeActions> {
		}
		internal static DocumentModelChangeActionsTable documentModelChangeActionsTable = CreateDocumentModelChangeActionsTable();
		static DocumentModelChangeActionsTable CreateDocumentModelChangeActionsTable() {
			DocumentModelChangeActionsTable result = new DocumentModelChangeActionsTable();
			result.Add(DocumentModelChangeType.None, DocumentModelChangeActions.None);
			result.Add(DocumentModelChangeType.CreateEmptyDocument, DocumentModelChangeActions.RaiseEmptyDocumentCreated | DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetSpellingCheck | DocumentModelChangeActions.RaiseSelectionChanged | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetControlsLayout | DocumentModelChangeActions.ResetPivotTableFieldsPanelVisibility);
			result.Add(DocumentModelChangeType.LoadNewDocument, DocumentModelChangeActions.RaiseDocumentLoaded | DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetSpellingCheck | DocumentModelChangeActions.RaiseSelectionChanged | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetControlsLayout | DocumentModelChangeActions.ResetPivotTableFieldsPanelVisibility);
			result.Add(DocumentModelChangeType.FreezePanes, DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.RaiseUpdateUI | DocumentModelChangeActions.Redraw);
			result.Add(DocumentModelChangeType.DrawingAdded, DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.RaiseUpdateUI | DocumentModelChangeActions.Redraw);
			result.Add(DocumentModelChangeType.DrawingRemoved, DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.RaiseUpdateUI | DocumentModelChangeActions.Redraw);
			return result;
		}
		public static DocumentModelChangeActions CalculateChangeActions(DocumentModelChangeType change) {
			return documentModelChangeActionsTable[change];
		}
	}
}
