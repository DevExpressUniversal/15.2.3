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
using DevExpress.Data;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native {
	public static class InplaceEditorHelper {
		public static MailMergeFieldInfo GetMailMergeFieldInfoFromInplaceEditor(XRFieldEmbeddableControl control) {
			if(control == null)
				return null;
			System.ComponentModel.Design.IDesignerHost designerHost = (System.ComponentModel.Design.IDesignerHost)control.Site.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
			DevExpress.XtraReports.Design.XRTextControlDesigner textDesigner = designerHost.GetDesigner(control) as DevExpress.XtraReports.Design.XRTextControlDesigner;
			if(textDesigner == null || !textDesigner.IsInplaceEditingMode)
				return null;
			int position = 0;
			string columnNameStr = GetColumnNameFromTextBoxSelection(textDesigner.Editor, ref position);
			if(columnNameStr == null)
				return null;
			MailMergeFieldInfo fieldInfo = new MailMergeFieldInfo();
			fieldInfo.FieldName = columnNameStr;
			string dataMember = control.Report.GetDataMemberFromDisplayName(fieldInfo.DisplayName);
			if(dataMember != null) {
				textDesigner.Editor.SetSelection(position, fieldInfo.ToString().Length);
				return fieldInfo;
			}
			return null;
		}
		static string GetColumnNameFromTextBoxSelection(InplaceTextEditorBase textBoxBase, ref int position) {
			int selectionStart = textBoxBase.SelectionStart;
			int selectionEnd = selectionStart + textBoxBase.SelectionLength;
			MailMergeFieldInfoCollection bracketPairs = PlainTextMailMergeFieldInfosCalculator.Instance.CalcMailMergeFieldInfos(textBoxBase.Text);
			foreach(MailMergeFieldInfo pair in bracketPairs) {
				if((pair.StartPosition < selectionStart && pair.EndPosition > selectionEnd)
				   || (pair.EndPosition >= selectionStart && pair.StartPosition <= selectionStart)
				   || (pair.StartPosition >= selectionStart && pair.EndPosition <= selectionEnd)
				   || (pair.StartPosition < selectionEnd && pair.EndPosition > selectionEnd)) {
					position = pair.StartPosition;
					return textBoxBase.Text.Substring(pair.StartPosition + 1, pair.EndPosition - pair.StartPosition - 1);
				}
			}
			return null;
		}
	}
}
