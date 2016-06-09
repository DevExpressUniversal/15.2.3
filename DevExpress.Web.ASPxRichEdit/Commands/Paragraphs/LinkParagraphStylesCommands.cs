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

using DevExpress.Web.ASPxRichEdit.Export;
using System;
using System.Collections;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public abstract class StyleLinkCommandBase : WebRichEditUpdateModelCommandBase {
		public StyleLinkCommandBase(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		protected override void PerformModifyModel() {
			string paragraphName = (string)this.Parameters["styleName"];
			ParagraphStyle paragraphStyle = DocumentModel.ParagraphStyles[DocumentModel.ParagraphStyles.GetStyleIndexByName(paragraphName)];
			UpdateParagraphStyle(paragraphStyle);
		}
		protected abstract void UpdateParagraphStyle(ParagraphStyle paragraphStyle);
	}
	public class CreateStyleLinkCommand : StyleLinkCommandBase {
		public CreateStyleLinkCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.CreateStyleLink; } }
		protected override bool IsEnabled() { return true; }
		protected override void UpdateParagraphStyle(ParagraphStyle paragraphStyle) {
			if (!paragraphStyle.HasLinkedStyle) {
				CharacterStyle characterStyle = new CharacterStyle(DocumentModel, DocumentModel.CharacterStyles.DefaultItem, String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.LinkedCharacterStyleFormatString), paragraphStyle.StyleName));
				DocumentModel.CharacterStyles.AddCore(characterStyle);
				characterStyle.CharacterProperties.CopyFrom(paragraphStyle.CharacterProperties);
				DocumentModel.StyleLinkManager.CreateLinkCore(paragraphStyle, characterStyle);
			}
		}
	}
	public class DeleteStyleLinkCommand : StyleLinkCommandBase {
		public DeleteStyleLinkCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.DeleteStyleLink; } }
		protected override bool IsEnabled() { return true; }
		protected override void UpdateParagraphStyle(ParagraphStyle paragraphStyle) {
			if (paragraphStyle.HasLinkedStyle) {
				DocumentModel.StyleLinkManager.DeleteLinkCore(paragraphStyle, paragraphStyle.LinkedStyle);
				DocumentModel.CharacterStyles.RemoveLastStyle();
			}
		}
	}
}
