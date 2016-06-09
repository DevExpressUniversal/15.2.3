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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
namespace DevExpress.XtraDiagram.Utils {
	public class DiagramSelectionUtils {
		public static DiagramItemSelection CalcSelection(Rectangle rect, Size gripSize, RotationGrip rotationGrip) {
			return CalcSelection(new DiagramElementBounds(rect), gripSize, rotationGrip);
		}
		public static DiagramItemSelection CalcSelection(DiagramElementBounds rect, Size gripSize, RotationGrip rotationGrip) {
			DiagramItemSelection selection = new DiagramItemSelection(rect, CalcRotationGrip(rect, rotationGrip));
			selection[SizeGripKind.TopLeft] = rect.GetTopLeftPt().CreateElementBounds(gripSize);
			selection[SizeGripKind.Top] = rect.GetTopPt().CreateElementBounds(gripSize);
			selection[SizeGripKind.TopRight] = rect.GetTopRightPt().CreateElementBounds(gripSize);
			selection[SizeGripKind.Left] = rect.GetLeftPt().CreateElementBounds(gripSize);
			selection[SizeGripKind.Right] = rect.GetRightPt().CreateElementBounds(gripSize);
			selection[SizeGripKind.BottomLeft] = rect.GetBottomLeftPt().CreateElementBounds(gripSize);
			selection[SizeGripKind.Bottom] = rect.GetBottomPt().CreateElementBounds(gripSize);
			selection[SizeGripKind.BottomRight] = rect.GetBottomRightPt().CreateElementBounds(gripSize);
			return selection;
		}
		static DiagramElementBounds CalcRotationGrip(DiagramElementBounds rect, RotationGrip rotationGrip) {
			DiagramElementPoint point = rect.GetTopPt();
			int vertIndent = rotationGrip.VertOffset + rotationGrip.GripSize.Height / 2;
			point.Offset(0, -vertIndent);
			return point.CreateElementBounds(rotationGrip.GripSize);
		}
	}
}
