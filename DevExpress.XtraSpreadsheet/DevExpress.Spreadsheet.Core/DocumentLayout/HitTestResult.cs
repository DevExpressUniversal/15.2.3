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
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region SpreadsheetHitTestResult
	public class SpreadsheetHitTestResult : DocumentLayoutPosition, ISupportsCopyFrom<SpreadsheetHitTestResult> {
		#region Fields
		HitTestAccuracy accuracy;
		Point logicalPoint;
		Point physicalPoint;
		HeaderTextBox headerBox;
		OutlineLevelBox groupBox;
		#endregion
		public SpreadsheetHitTestResult(DocumentLayout documentLayout)
			: base(documentLayout, CellPosition.InvalidValue) {
		}
		#region Properties
		public HitTestAccuracy Accuracy { get { return accuracy; } set { accuracy = value; } }
		public Point LogicalPoint { get { return logicalPoint; } set { logicalPoint = value; } }
		public Point PhysicalPoint { get { return physicalPoint; } set { physicalPoint = value; } }
		public HeaderTextBox HeaderBox { get { return headerBox; } set { headerBox = value; } }
		public OutlineLevelBox GroupBox { get { return groupBox; } set { groupBox = value; } }
		#endregion
		#region ISupportsCopyFrom<SpreadsheetHitTestResult> Members
		public void CopyFrom(SpreadsheetHitTestResult value) {
			base.CopyFrom(value);
			this.Accuracy = value.Accuracy;
			this.LogicalPoint = value.LogicalPoint;
			this.PhysicalPoint = value.PhysicalPoint;
			this.HeaderBox = value.HeaderBox;
			this.GroupBox = value.GroupBox;
		}
		#endregion
	}
	#endregion
}
