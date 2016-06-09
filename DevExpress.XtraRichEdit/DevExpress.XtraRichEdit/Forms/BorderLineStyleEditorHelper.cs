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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraRichEdit.Forms {
	public class BorderLineStyleEditorHelper {
		readonly IBorderLineStyleEditor editor;
		public BorderLineStyleEditorHelper(IBorderLineStyleEditor editor) {
			Guard.ArgumentNotNull(editor, "editor");
			this.editor = editor;
		}
		public DocumentModel DocumentModel { get { return editor.DocumentModel; } }
		public IBorderLineStyleEditor Editor { get { return editor; } }
		protected internal virtual void PopulateItemsCore(bool skipNone) {
			List<BorderInfo> borderInfos = DocumentModel.TableBorderInfoRepository.Items;
			int count = borderInfos.Count;
			for (int i = 0; i < count; i++)
				if (!ShouldSkipBorderStyle(borderInfos[i].Style, skipNone))
					AddItem(borderInfos[i]);
		}
		public void OnBeginInit() {
			Editor.Clear();
		}
		public void OnEndInit(bool skipNone) {
			if (Editor.ItemsCount <= 0)
				PopulateItems(skipNone);
		}
		public void PopulateItems(bool skipNone) {
			Editor.BeginUpdate();
			try {
				Editor.Clear();
				if (DocumentModel != null)
					PopulateItemsCore(skipNone);
			}
			finally {
				Editor.EndUpdate();
			}
		}
		protected virtual bool ShouldSkipBorderStyle(BorderLineStyle style, bool skipNone) {
			if (skipNone && (style == BorderLineStyle.None || style == BorderLineStyle.Nil))
				return true;
			return style == BorderLineStyle.Disabled || style == BorderLineStyle.DashDotStroked ||
				style == BorderLineStyle.ThreeDEmboss || style == BorderLineStyle.ThreeDEngrave ||
				style == BorderLineStyle.Inset || style == BorderLineStyle.Outset;
		}
		void AddItem(BorderInfo borderInfo) {
			string description = borderInfo.Style.ToString();
			if (borderInfo.Style == BorderLineStyle.None)
				description = XtraRichEditLocalizer.GetString(XtraRichEditStringId.BorderLineStyleNone);
			Editor.AddItem(description, borderInfo);
		}
	}
	public interface IBorderLineStyleEditor {
		DocumentModel DocumentModel { get; }
		void BeginUpdate();
		void EndUpdate();
		void Clear();
		int ItemsCount { get; }
		void AddItem(string description, BorderInfo borderInfo);
	}
}
