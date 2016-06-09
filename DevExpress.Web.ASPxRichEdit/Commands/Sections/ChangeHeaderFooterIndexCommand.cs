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

using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public abstract class ChangeHeaderFooterIndexCommandBase<TObject, TIndex> : WebRichEditUpdateModelCommandBase
		where TObject : SectionHeaderFooterBase
		where TIndex : struct, IConvertToInt<TIndex> {
		public ChangeHeaderFooterIndexCommandBase(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		protected override void PerformModifyModel() {
			var sectionIndex = new SectionIndex((int)Parameters["sectionIndex"]);
			var section = DocumentModel.Sections[sectionIndex];
			var newObjectIndex = (int)Parameters["newObjectIndex"];
			var type = (HeaderFooterType)Parameters["type"];
			var objectIndex = CreateIndex(newObjectIndex);
			GetContainer(section).SetObjectIndex(type, objectIndex);
		}
		protected override bool IsEnabled() {
			return Client.DocumentCapabilitiesOptions.SectionsAllowed && Client.DocumentCapabilitiesOptions.HeadersFootersAllowed;
		}
		protected abstract SectionHeadersFooters<TObject, TIndex> GetContainer(Section section);
		protected abstract TIndex CreateIndex(int index);
	}
	public class ChangeHeaderIndexCommand : ChangeHeaderFooterIndexCommandBase<SectionHeader, HeaderIndex> {
		public ChangeHeaderIndexCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeHeaderIndex; } }
		protected override HeaderIndex CreateIndex(int index) {
			return new HeaderIndex(index);
		}
		protected override SectionHeadersFooters<SectionHeader, HeaderIndex> GetContainer(Section section) {
			return section.Headers;
		}
	}
	public class ChangeFooterIndexCommand : ChangeHeaderFooterIndexCommandBase<SectionFooter, FooterIndex> {
		public ChangeFooterIndexCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeFooterIndex; } }
		protected override FooterIndex CreateIndex(int index) {
			return new FooterIndex(index);
		}
		protected override SectionHeadersFooters<SectionFooter, FooterIndex> GetContainer(Section section) {
			return section.Footers;
		}
	}
}
