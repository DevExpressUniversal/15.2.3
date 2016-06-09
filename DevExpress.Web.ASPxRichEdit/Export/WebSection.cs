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
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class WebSection : IHashtableProvider {
		public WebSection(Section section, WebSectionPropertiesCache cache, WebRichEditLoadModelCommandBase command) {
			Section = section;
			Cache = cache;
			LogPosition = Section.DocumentModel.MainPieceTable.Paragraphs[section.FirstParagraphIndex].LogPosition;
			EndLogPosition = Section.DocumentModel.MainPieceTable.Paragraphs[section.LastParagraphIndex].EndLogPosition;
			SectionPropertiesCacheIndex = cache.AddItem(new WebSectionProperties(section));
			Command = command;
		}
		protected WebSectionPropertiesCache Cache { get; private set; }
		protected Section Section { get; private set; }
		protected WebRichEditLoadModelCommandBase Command { get; private set; }
		public int SectionPropertiesCacheIndex { get; protected internal set; }
		public DocumentLogPosition LogPosition { get; protected internal set; }
		public DocumentLogPosition EndLogPosition { get; protected internal set; }
		public void FillHashtable(Hashtable result) {
			result["start"] = ((IConvertToInt<DocumentLogPosition>)LogPosition).ToInt();
			result["length"] = EndLogPosition - LogPosition + 1;
			result["sectionPropertiesCacheIndex"] = SectionPropertiesCacheIndex;
			result["headers"] = GetHeaders(Section.Headers);
			result["footers"] = GetFooters(Section.Footers);
		}
		protected Hashtable GetHeaders(SectionHeaders headers) {
			var result = new Hashtable();
			foreach(HeaderFooterType type in Enum.GetValues(typeof(HeaderFooterType)))
				result[(int)type] = ((IConvertToInt<HeaderIndex>)headers.GetObjectIndex(type)).ToInt();
			return result;
		}
		protected Hashtable GetFooters(SectionFooters footers) {
			var result = new Hashtable();
			foreach (HeaderFooterType type in Enum.GetValues(typeof(HeaderFooterType)))
				result[(int)type] = ((IConvertToInt<FooterIndex>)footers.GetObjectIndex(type)).ToInt();
			return result;
		}
	}
	public class WebSectionCollection : HashtableCollection<WebSection> { }
}
