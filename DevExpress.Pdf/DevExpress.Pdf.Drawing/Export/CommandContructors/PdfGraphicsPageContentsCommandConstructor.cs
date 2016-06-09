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
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	abstract class PdfGraphicsPageContentsCommandConstructor : PdfGraphicsCommandConstructor {
		readonly PdfPage page;
		readonly IList<PdfGraphicsCommand> exportCommands;
		public PdfPage Page { get { return page; } }
		protected PdfGraphicsPageContentsCommandConstructor(PdfPage page, IList<PdfGraphicsCommand> exportCommands, float factorX, float factorY, PdfEditableFontDataCache fontCache, PdfImageCache imageCache)
				: base(page.CropBox, page.Resources, factorX, factorY, fontCache, imageCache, page.DocumentCatalog) {
			this.page = page;
			this.exportCommands = exportCommands;
		}
		public void Execute() {
			if (exportCommands.Count != 0) {
				PdfCommandList pageCommands = page.Commands;
				int pageCommandsCount = pageCommands.Count;
				if (pageCommandsCount != 0 && (pageCommandsCount < 2 || pageCommands[0] is PdfSaveGraphicsStateCommand || pageCommands[pageCommandsCount - 1] is PdfRestoreGraphicsStateCommand)) {
					pageCommands.Insert(0, new PdfSaveGraphicsStateCommand());
					pageCommands.Add(new PdfRestoreGraphicsStateCommand());
				}
				SaveGraphicsState();
				foreach (PdfGraphicsCommand command in exportCommands)
					command.Execute(this);
				RestoreGraphicsState();
				AddNewCommands(pageCommands, Commands);
				page.ReplaceCommands(pageCommands);
			}
		}
	}
}
