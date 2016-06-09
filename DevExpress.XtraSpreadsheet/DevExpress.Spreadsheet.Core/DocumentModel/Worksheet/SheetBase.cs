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
using System.Globalization;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SheetBase (abstract class)
	public abstract partial class SheetBase {
		#region Fields
		readonly int sheetId;
		readonly IModelWorkbook workbook;
		string name;
		#endregion
		protected SheetBase(IModelWorkbook workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.workbook = workbook;
			this.sheetId = workbook.CreateNextSheetId();
		}
		#region Properties
		public int SheetId { get { return this.sheetId; } }
		public IModelWorkbook Workbook { get { return workbook; } }
		public CultureInfo Culture { get { return Workbook.Culture; } }
		public string Name { 
			get { return name; }
			set {
				if (name == value)
					return;
				Rename(value);
			}
		}
		public abstract SheetType SheetType { get; }
		#endregion
		public override string ToString() {
			return String.Format("Name='{0}', SheetId={1}", Name, SheetId);
		}
		protected internal virtual void Rename(string newName) {
			SetNameCore(newName);
		}
		protected internal virtual void SetNameCore(string newName) {
			this.name = newName;
		}
	}
	#endregion
	#region SheetType
	public enum SheetType {
		RegularWorksheet = 0,
		MacroSheet = 1,
		ChartSheet = 2,
		VBAModule = 6,
		External = 7,
	}
	#endregion
}
