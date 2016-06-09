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
namespace DevExpress.XtraSpreadsheet.Layout {
	#region SpreadsheetHitTestRequest
	public class SpreadsheetHitTestRequest : ICloneable<SpreadsheetHitTestRequest>, ISupportsCopyFrom<SpreadsheetHitTestRequest> {
		#region Fields
		Point physicalPoint;
		Point logicalPoint;
		DocumentLayoutDetailsLevel detailsLevel;
		HitTestAccuracy accuracy;
		#endregion
		public SpreadsheetHitTestRequest() {
		}
		#region Properties
		public Point PhysicalPoint { get { return physicalPoint; } set { physicalPoint = value; } }
		public Point LogicalPoint { get { return logicalPoint; } set { logicalPoint = value; } }
		public DocumentLayoutDetailsLevel DetailsLevel { get { return detailsLevel; } set { detailsLevel = value; } }
		public HitTestAccuracy Accuracy { get { return accuracy; } set { accuracy = value; } }
		#endregion
		#region ICloneable<SpreadsheetHitTestRequest> Members
		public SpreadsheetHitTestRequest Clone() {
			SpreadsheetHitTestRequest clone = new SpreadsheetHitTestRequest();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<SpreadsheetHitTestRequest> Members
		public void CopyFrom(SpreadsheetHitTestRequest value) {
			this.PhysicalPoint = value.PhysicalPoint;
			this.LogicalPoint = value.LogicalPoint;
			this.DetailsLevel = value.DetailsLevel;
			this.Accuracy = value.Accuracy;
		}
		#endregion
	}
	#endregion
}
