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
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.Office.Drawing;
namespace DevExpress.Snap.Core.Native.Services {
	public class CustomRunBoxLayoutExporterService : ICustomRunBoxLayoutExporterService {
		#region ICustomRunBoxLayoutExporterService Members
		public ICustomRunObject CustomRunObjectFromString(string stringObj) {
			return Activator.CreateInstance(Type.GetType(stringObj)) as ICustomRunObject;
		}
		public string CustomRunObjectToSting(ICustomRunObject customObject) {
			return customObject.GetText();
		}
		public void ExportCustomRunBox(PieceTable pieceTable, Painter painter, CustomRunBox box) {
			CustomRun run = (CustomRun)box.GetRun(pieceTable);
			run.CustomRunObject.Export(pieceTable, painter, box.Bounds);
		}
		public VisualBrick ExportToPrintingSystem(PieceTable pieceTable, PrintingSystemBase ps, CustomRunBox box) {
			CustomRun run = (CustomRun)box.GetRun(pieceTable);
			return run.CustomRunObject.GetVisualBrick(pieceTable, box.Bounds);
		}
		#endregion
	}
}
