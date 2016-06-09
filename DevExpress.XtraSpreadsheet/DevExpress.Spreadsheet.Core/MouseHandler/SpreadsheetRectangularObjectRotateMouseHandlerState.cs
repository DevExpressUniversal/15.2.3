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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Layout;
using System.Drawing;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region SpreadsheetRectangularObjectRotateMouseHandlerState
	public class SpreadsheetRectangularObjectRotateMouseHandlerState : SpreadsheetRectangularObjectResizeMouseHandlerState {
		float initialRotationAngle;
		public SpreadsheetRectangularObjectRotateMouseHandlerState(SpreadsheetMouseHandler mouseHandler, DrawingObjectHotZone hotZone, SpreadsheetHitTestResult initialHitTestResult)
			: base(mouseHandler, hotZone, initialHitTestResult) {
			this.initialRotationAngle = RotationAngle;
		}
		protected internal float InitialRotationAngle { get { return initialRotationAngle; } set { initialRotationAngle = value; } }
		protected internal override SpreadsheetCursor CalculateMouseCursor() {
			return SpreadsheetCursors.Rotate;
		}
		protected internal override void CommitChanges(Point point) {
			int degreeInModelUnits = DocumentModel.UnitConverter.DegreeToModelUnits(RotationAngle - InitialRotationAngle);
			DocumentModel.ActiveSheet.DrawingObjects[Box.DrawingIndex].Rotate(degreeInModelUnits);
		}
		protected internal override void UpdateObjectProperties() {
			Point logicalPoint = CurrentHitTestResult.LogicalPoint;
			RotationAngle = ObjectRotationAngleCalculator.CalculateAngle(logicalPoint, InitialShapeBounds, InitialRotationAngle, HotZone.HitTestTransform);
		}
	}
	#endregion
}
