﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.Commands {
	public class SnapToggleTableFixedColumnWidthCommand : ToggleTableFixedColumnWidthCommand {
		public SnapToggleTableFixedColumnWidthCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override XtraRichEdit.Model.Table Table {
			get {
				TableCell cell = DocumentModel.Selection.SelectedCells.FirstSelectedCell;
				if (cell == null)
					if (InnerControl.ActiveView.CaretPosition.LayoutPosition != null && InnerControl.ActiveView.CaretPosition.LayoutPosition.TableCell != null)
						cell = InnerControl.ActiveView.CaretPosition.LayoutPosition.TableCell.Cell;
					else
						cell = new SnapObjectModelController((SnapPieceTable)DocumentModel.ActivePieceTable).FindTableCellBySeparator();
				return cell.Table;
			}
		}
	}
}
