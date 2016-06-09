#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.Native.RichText;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.Text)
	]
	public class TextBoxDashboardItem : DashboardItem {
		readonly DocumentModel documentModel = new DocumentModel(DevExpress.XtraRichEdit.Internal.RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies());
		string innerRtf;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("TextBoxDashboardItemRtf"),
#endif
		Category(CategoryNames.Data),
		TypeConverter(TypeNames.DisplayNameNoneObjectConverter),
		Editor(TypeNames.RtfEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string Rtf {
			get { return innerRtf ?? GetDocumentRtf(); }
			set {
				if (String.IsNullOrEmpty(value)) {
					if (!String.IsNullOrEmpty(innerRtf))
						SetDefault();
				}
				else if (RtfTags.IsRtfContent(value)) {
					XtraRichTextEditHelper.ImportRtfTextToDocManager(value, documentModel);
					SetInnerRtf(value);
				}
				else
					SetText(value);
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("TextBoxDashboardItemText"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string Text { 
			get { return XtraRichTextEditHelper.GetTextFromDocManager(documentModel); }
			set {
				if (String.IsNullOrEmpty(value)) {
					if (!String.IsNullOrEmpty(innerRtf))
						SetDefault();
				}
				else
					SetText(value);
			} 
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null),
		Localizable(true)
		]
		public string InnerRtf { get { return innerRtf; } set { Rtf = value; } }
		internal DocumentModel DocumentModel { get { return documentModel; } }
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameTextBoxItem); } }
		void ImportPlainText(string text) {
			DevExpress.XtraRichEdit.Utils.TextManipulatorHelper.SetText(documentModel.MainPieceTable, text);
		}
		void SetDefault() {
			ImportPlainText(String.Empty);
			SetInnerRtf(null);
		}
		string GetDocumentRtf() {
			return XtraRichTextEditHelper.GetRtfFromDocManager(documentModel);
		}
		void SetInnerRtf(string newRtf) {
			if(newRtf != innerRtf) {
				innerRtf = newRtf;
				OnChanged(ChangeReason.View);
			}
		}
		void SetText(string text) {
			ImportPlainText(text);
			SetInnerRtf(GetDocumentRtf());
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			return new TextBoxDashboardItemViewModel(this);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			InnerRtf = element.Value.Replace("\n", "\r\n"); 
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(!string.IsNullOrEmpty(innerRtf))
				element.Value = InnerRtf;
		}
	}
}
