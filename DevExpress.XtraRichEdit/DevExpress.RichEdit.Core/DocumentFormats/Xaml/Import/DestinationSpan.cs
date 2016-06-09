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
using System.Xml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Xaml {
	#region SpanDestination
	public class SpanDestination : InlineElementDestination {
		public SpanDestination(XamlImporter importer)
			: base(importer) {
		}
		public override bool ProcessText(XmlReader reader) {
			RunDestination runDestination = new RunDestination(Importer);
			runDestination.ProcessText(reader);
			runDestination.ProcessElementClose(reader);
			return true;
		}
	}
	#endregion
	#region BoldDestination
	public class BoldDestination : SpanDestination {
		public BoldDestination(XamlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyAttributes(XmlReader reader) {
			base.ApplyAttributes(reader);
			Importer.Position.CharacterFormatting.FontBold = true;
		}
	}
	#endregion
	#region ItalicDestination
	public class ItalicDestination : SpanDestination {
		public ItalicDestination(XamlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyAttributes(XmlReader reader) {
			base.ApplyAttributes(reader);
			Importer.Position.CharacterFormatting.FontItalic = true;
		}
	}
	#endregion
	#region UnderlineDestination
	public class UnderlineDestination : SpanDestination {
		public UnderlineDestination(XamlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyAttributes(XmlReader reader) {
			base.ApplyAttributes(reader);
			Importer.Position.CharacterFormatting.FontUnderlineType = UnderlineType.Single;
		}
	}
	#endregion
	#region HyperlinkDestination
	public class HyperlinkDestination : SpanDestination {
		public HyperlinkDestination(XamlImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			if (!DocumentFormatsHelper.ShouldInsertHyperlinks(Importer.DocumentModel))
				return;
			ImportFieldHelper importFieldHelper = new ImportFieldHelper(Importer.PieceTable);
			ImportFieldInfo fieldInfo = new ImportFieldInfo(Importer.PieceTable);
			Importer.FieldInfoStack.Push(fieldInfo);
			importFieldHelper.ProcessFieldBegin(fieldInfo, Importer.Position);
			HyperlinkInfo hyperlinkInfo = new HyperlinkInfo();
			hyperlinkInfo.NavigateUri = Importer.ReadAttribute(reader, "NavigateUri");
			hyperlinkInfo.Target = Importer.ReadAttribute(reader, "TargetName");
			importFieldHelper.InsertHyperlinkInstruction(hyperlinkInfo, Importer.Position);
			importFieldHelper.ProcessFieldSeparator(fieldInfo, Importer.Position);
		}
		public override void ProcessElementClose(XmlReader reader) {
			ImportFieldInfo fieldInfo = Importer.FieldInfoStack.Pop();
			ImportFieldHelper importFieldHelper = new ImportFieldHelper(Importer.PieceTable);
			importFieldHelper.ProcessFieldEnd(fieldInfo, Importer.Position);
			if (Importer.FieldInfoStack.Count > 0)
				fieldInfo.Field.Parent = Importer.FieldInfoStack.Peek().Field;
			base.ProcessElementClose(reader);
		}
	}
	#endregion
}
