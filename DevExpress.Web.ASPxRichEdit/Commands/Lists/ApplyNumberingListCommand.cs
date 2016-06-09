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

using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class ApplyNumberingListCommand : WebRichEditStateBasedCommand<IntervalCommandState> {
		public ApplyNumberingListCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ApplyNumberingList; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModelCore(IntervalCommandState stateObject) {
			ArrayList value = (ArrayList)stateObject.Value;
			var numberingListIndex = new NumberingListIndex(Convert.ToInt32(value[0]));
			var listLevelIndex = Convert.ToInt32(value[1]);
			var paragraphIndex = PieceTable.FindParagraphIndex(stateObject.Position);
			var endPosition = stateObject.Position + stateObject.Length;
			while(true) {
				var paragraph = PieceTable.Paragraphs[paragraphIndex];
				paragraph.SetNumberingListIndex(numberingListIndex);
				paragraph.SetListLevelIndex(listLevelIndex);
				if(paragraph.LogPosition + paragraph.Length >= endPosition)
					break;
				else
					paragraphIndex++;
			}
		}
		protected override bool IsValidOperation() {
			return Manager.Client.DocumentCapabilitiesOptions.Numbering.BulletedAllowed || Manager.Client.DocumentCapabilitiesOptions.Numbering.MultiLevelAllowed || Manager.Client.DocumentCapabilitiesOptions.Numbering.SimpleAllowed;
		}
	}
}
