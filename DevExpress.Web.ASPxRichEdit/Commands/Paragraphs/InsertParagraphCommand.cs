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
using System.Collections;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class InsertParagraphCommand : WebRichEditUpdateModelCommandBase {
		public InsertParagraphCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.InsertParagraph; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModel() {
			DocumentLogPosition logPosition = new DocumentLogPosition((int)Parameters["position"]);
			Paragraph paragraph = PieceTable.InsertParagraph(logPosition);
			ApplyParagraphFormatting(paragraph);
			ApplyParagraphStyle(paragraph);
			ApplyParagraphNumberingProperties(paragraph);
		}
		protected override bool IsValidOperation() {
			return Manager.Client.DocumentCapabilitiesOptions.ParagraphsAllowed;
		}
		private void ApplyParagraphNumberingProperties(Paragraph paragraph) {
			int numberingListIndex = Convert.ToInt32(Parameters["numberingListIndex"]);
			int listLevelIndex = Convert.ToInt32(Parameters["listLevelIndex"]);
			paragraph.NumberingListIndex = new NumberingListIndex(numberingListIndex);
			paragraph.SetListLevelIndex(listLevelIndex);
		}
	}
}
