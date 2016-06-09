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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region EditPageFooterCommand
	public class EditPageFooterCommand : EditPageHeaderFooterCommand<SectionFooter> {
		public EditPageFooterCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EditPageFooterCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_EditPageFooter; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EditPageFooterCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_EditPageFooterDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EditPageFooterCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.EditPageFooter; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EditPageFooterCommandImageName")]
#endif
		public override string ImageName { get { return "Footer"; } }
		protected internal override InsertPageHeaderFooterCoreCommand<SectionFooter> CreateInsertObjectCommand(HeaderFooterType type) {
			return new InsertPageFooterCoreCommand(Control, type);
		}
		protected internal override SectionHeadersFootersBase GetContainer(Section section) {
			return section.Footers;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertPageFooterCoreCommand
	public class InsertPageFooterCoreCommand : InsertPageHeaderFooterCoreCommand<SectionFooter> {
		public InsertPageFooterCoreCommand(IRichEditControl control, HeaderFooterType type)
			: base(control, type) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_EditPageFooter; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_EditPageFooterDescription; } }
		public override string ImageName { get { return "Footer"; } }
		protected internal override SectionHeadersFootersBase GetHeaderFooterContainer(Section section) {
			return section.Footers;
		}
	}
	#endregion
}
