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
using System.Drawing;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.NativeBricks;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class TableBrickExporter : PanelBrickExporter {
#if DEBUGTEST
		public static bool FailOnEmptyAreas = true;
#endif
		protected override BrickViewData GetOuterPanel(ExportContext exportContext, RectangleF boundsF) {
			return null;
		}
		protected override List<Rectangle> GenerateEmptyAreas(RectangleDivider divider, bool allowEmptyAreas) {
#if DEBUGTEST
			if (!allowEmptyAreas && FailOnEmptyAreas) {
				List<Rectangle> areas = base.GenerateEmptyAreas(divider, allowEmptyAreas);
				System.Diagnostics.Debug.Assert(areas.Count == 0);
			}
#endif
			return new List<Rectangle>();
		}
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			BrickViewData[] data = GetViewData(rect, clipRect, exportContext);
			if(data.Length > 0 && !string.IsNullOrEmpty((Brick as TableBrick).AnchorName))
				data[data.Length - 1].TableCell = new AnchorCell(data[data.Length - 1].TableCell, (Brick as TableBrick).AnchorName);
			return data;
		}
	}
}
