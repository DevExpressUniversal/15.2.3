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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.Html {
	#region ButtonTag
	public class ButtonTag : TagBase {
		public ButtonTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return new ParagraphFormattingOptions();
		}
		protected internal override void OpenTagProcessCore() {
			base.OpenTagProcessCore();
			Importer.Position.CopyFrom(Importer.TagsStack[Importer.TagsStack.Count - 1].OldPosition);
		}
	}
	#endregion
	#region FormTag
	public class FormTag : TagBase {
		public FormTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return new ParagraphFormattingOptions();
		}
		protected internal override void OpenTagProcessCore() {
			base.OpenTagProcessCore();
			Importer.Position.CopyFrom(Importer.TagsStack[Importer.TagsStack.Count - 1].OldPosition);
		}
	}
	#endregion
	#region FieldsetTag
	public class FieldsetTag : TagBase {
		public FieldsetTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return new ParagraphFormattingOptions();
		}
		protected internal override void OpenTagProcessCore() {
			base.OpenTagProcessCore();
			Importer.Position.CopyFrom(Importer.TagsStack[Importer.TagsStack.Count - 1].OldPosition);
		}
	}
	#endregion
	#region InputTag
	public class InputTag : TagBase {
		public InputTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return new ParagraphFormattingOptions();
		}
		protected internal override void OpenTagProcessCore() {
			base.OpenTagProcessCore();
			Importer.Position.CopyFrom(Importer.TagsStack[Importer.TagsStack.Count - 1].OldPosition);
		}
	}
	#endregion
	#region LegendTag
	public class LegendTag : TagBase {
		public LegendTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return new ParagraphFormattingOptions();
		}
		protected internal override void OpenTagProcessCore() {
			base.OpenTagProcessCore();
			Importer.Position.CopyFrom(Importer.TagsStack[Importer.TagsStack.Count - 1].OldPosition);
		}
	}
	#endregion
	#region LabelTag
	public class LabelTag : TagBase {
		public LabelTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void OpenTagProcessCore() {
			base.OpenTagProcessCore();
			Importer.Position.CopyFrom(Importer.TagsStack[Importer.TagsStack.Count - 1].OldPosition);
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return new ParagraphFormattingOptions();
		}
	}
	#endregion
	#region TextAreaTag
	public class TextAreaTag : TagBase {
		public TextAreaTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void OpenTagProcessCore() {
			base.OpenTagProcessCore();
			Importer.Position.CopyFrom(Importer.TagsStack[Importer.TagsStack.Count - 1].OldPosition);
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return new ParagraphFormattingOptions();
		}
	}
	#endregion
	#region SelectTag
	public class SelectTag : TagBase {
		public SelectTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void OpenTagProcessCore() {
			base.OpenTagProcessCore();
			Importer.Position.CopyFrom(Importer.TagsStack[Importer.TagsStack.Count - 1].OldPosition);
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return new ParagraphFormattingOptions();
		}
	}
	#endregion
	#region OptGroupTag
	public class OptGroupTag : TagBase {
		public OptGroupTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void OpenTagProcessCore() {
			base.OpenTagProcessCore();
			Importer.Position.CopyFrom(Importer.TagsStack[Importer.TagsStack.Count - 1].OldPosition);
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return new ParagraphFormattingOptions();
		}
	}
	#endregion
	#region OptionTag
	public class OptionTag : TagBase {
		public OptionTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return new ParagraphFormattingOptions();
		}
		protected internal override void OpenTagProcessCore() {
			base.OpenTagProcessCore();
			Importer.Position.CopyFrom(Importer.TagsStack[Importer.TagsStack.Count - 1].OldPosition);
		}
	}
	#endregion
}
