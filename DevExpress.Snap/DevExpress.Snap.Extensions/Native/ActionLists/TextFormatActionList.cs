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
using System.ComponentModel;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Utils.Design;
namespace DevExpress.Snap.Extensions.Native.ActionLists {
	#region TextFormatActionList
	public class TextFormatActionList : FieldActionList<SNTextField> {
		Dictionary<TextFormat, string> TextFormatTypeToString = new Dictionary<TextFormat, string>(){
			{ TextFormat.DOC, "doc" },
			{ TextFormat.HTML, "html" },
			{ TextFormat.MHT, "mht" },
			{ TextFormat.OpenDocument, "opendocument" },
			{ TextFormat.OpenXML, "openxml" },
			{ TextFormat.RTF, "rtf" },
			{ TextFormat.WordML, "wordml" },
			{ TextFormat.PlainText, String.Empty }
		};
		Dictionary<string, TextFormat> StringToTextFormatType = new Dictionary<string, TextFormat>() {
			{ "doc", TextFormat.DOC },
			{ "html", TextFormat.HTML },
			{ "mht", TextFormat.MHT },
			{ "opendocument", TextFormat.OpenDocument },
			{ "openxml", TextFormat.OpenXML },
			{ "rtf", TextFormat.RTF },
			{ "wordml", TextFormat.WordML },
			{ String.Empty, TextFormat.PlainText }
		};
		public TextFormatActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SNTextFieldTagItem_TextFormat")]
		public TextFormat TextFormat {
			get {
				string textFormat = !String.IsNullOrEmpty(ParsedInfo.TextFormat) ? ParsedInfo.TextFormat : String.Empty;
				return StringToTextFormatType[textFormat];
			}
			set {
				if (TextFormat == value)
					return;
				ApplyNewValue(ChangeTextFormat, value);
				SNSmartTagService smartTagService = (SNSmartTagService)this.Component.Site.GetService(typeof(SNSmartTagService));
				smartTagService.UpdatePopup();
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SNTextFieldTagItem_KeepLastParagraph")]
		public bool KeepLastParagraph {
			get { return ParsedInfo.KeepLastParagraph; }
			set {
				if (KeepLastParagraph == value)
					return;
				ApplyNewValue(ChangeKeepLastParagraph, value);
			}
		}
		void ChangeKeepLastParagraph(InstructionController controller, bool keepLastParagraph) {
			if (keepLastParagraph)
				controller.SetSwitch(SNTextField.KeepLastParagraphSwitch);
			else
				controller.RemoveSwitch(SNTextField.KeepLastParagraphSwitch);
		}
		void ChangeTextFormat(InstructionController controller, TextFormat type) {
			string textFormat = TextFormatTypeToString[type];
			if (!String.IsNullOrEmpty(textFormat))
				controller.SetSwitch(SNTextField.TextFormatSwitch, TextFormatTypeToString[type]);
			else
				controller.RemoveSwitch(SNTextField.TextFormatSwitch);
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "TextFormat", "TextFormat");
			AddPropertyItem(actionItems, "KeepLastParagraph", "KeepLastParagraph");
		}
	}
	#endregion
	[TypeConverter(typeof(EnumTypeConverter)), ResourceFinder(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum TextFormat {
		PlainText,
		RTF,
		DOC,
		OpenXML,
		HTML,
		MHT,
		WordML,
		OpenDocument
	}
}
