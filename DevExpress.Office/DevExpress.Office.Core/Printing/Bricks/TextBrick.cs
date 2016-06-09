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

using System;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.Office.Layout;
using DevExpress.XtraPrinting.Native;
#if !SL
using DevExpress.XtraPrinting.BrickExporters;
#endif
namespace DevExpress.Office.Printing {
	#region OfficeTextBrick
#if !SL
	[BrickExporter(typeof(OfficeTextBrickExporter))]
#endif
	public class OfficeTextBrick : TextBrick {
		internal readonly DocumentLayoutUnitConverter unitConverter;
		public OfficeTextBrick(DocumentLayoutUnitConverter unitConverter)
			: this(unitConverter, NullBrickOwner.Instance) {
		}
		public OfficeTextBrick(DocumentLayoutUnitConverter unitConverter, IBrickOwner brickOwner) 
			: base(brickOwner) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
		}
		public void SetBrickOwner(IBrickOwner brickOwner) {
			this.BrickOwner = brickOwner;
		}
	}
	#endregion
#if !SL
	#region OfficeTextBrickExporter
	public class OfficeTextBrickExporter : TextBrickExporter {
		OfficeTextBrick OfficeTextBrick { get { return (OfficeTextBrick)Brick; } }
		protected override void DrawBackground(IGraphics gr, RectangleF rect) {
			if (gr is DevExpress.XtraPrinting.Export.Pdf.PdfGraphics)
				base.DrawBackground(gr, rect);
			else {
				Rectangle bounds = Rectangle.Round(OfficeTextBrick.unitConverter.DocumentsToLayoutUnits(rect));
				base.DrawBackground(gr, bounds);
			}
		}
	}
	#endregion
#endif
}
