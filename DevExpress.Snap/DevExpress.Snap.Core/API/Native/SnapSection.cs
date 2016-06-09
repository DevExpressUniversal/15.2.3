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

using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Snap.Core.API;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Internal;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using ModelSection = DevExpress.XtraRichEdit.Model.Section;
using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
namespace DevExpress.Snap.API.Native {
	#region SnapSection
	[ComVisible(true)]
	public interface SnapSection : Section {
		new SnapSubDocument BeginUpdateHeader();
		new SnapSubDocument BeginUpdateHeader(HeaderFooterType type);
		new SnapSubDocument BeginUpdateFooter();
		new SnapSubDocument BeginUpdateFooter(HeaderFooterType type);
	}
	#endregion
	public class NativeSnapSection : NativeSection, SnapSection {
		internal NativeSnapSection(SnapNativeDocument document, ModelSection innerSection) : base(document, innerSection) { }
		public new SnapSubDocument BeginUpdateHeader() {
			return (SnapSubDocument)base.BeginUpdateHeader();
		}
		public new SnapSubDocument BeginUpdateHeader(HeaderFooterType type) {
			return (SnapSubDocument)base.BeginUpdateHeader(type);
		}
		public new SnapSubDocument BeginUpdateFooter() {
			return (SnapSubDocument)base.BeginUpdateFooter();
		}
		public new SnapSubDocument BeginUpdateFooter(HeaderFooterType type) {
			return (SnapSubDocument)base.BeginUpdateFooter(type);
		}
		protected override SubDocument CreateSubDocument(ModelPieceTable pieceTable, InnerRichEditDocumentServer server) {
			return new SnapNativeSubDocument((SnapPieceTable)pieceTable, server, (SnapNativeDocument)Document);
		}
	}
	[ComVisible(true)]
	public interface SnapSectionCollection : SectionCollection {
		new SnapSection this[int index] { get; }
	}
	public class NativeSnapSectionCollection : NativeSectionCollection, SnapSectionCollection {
		public new SnapSection this[int index] { get { return (SnapSection)base[index]; } }
	}
}
