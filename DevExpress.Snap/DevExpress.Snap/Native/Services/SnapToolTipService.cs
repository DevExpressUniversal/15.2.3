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
using DevExpress.Utils;
using DevExpress.Services.Implementation;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Data.Browsing;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Fields;
using System.Collections.Generic;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native.Services;
namespace DevExpress.Snap.Native.Services {
	public class SnapToolTipService : ToolTipService {
		public SnapToolTipService(SnapControl control)
			: base(control) {
		}
		public new SnapControl Control { get { return (SnapControl)base.Control; } }
		protected SnapDocumentModel DocumentModel { get { return Control.DocumentModel; } }
		protected override ToolTipControlInfo CalculateToolTipInfo(object activeObject) {
			Field field = activeObject as Field;
			if (field != null && field.DisableUpdate)
				field = field.Parent;
			if (field == null || field.IsCodeView)
				return null;
			ToolTipControlInfo fieldInfo = CreateFieldToolTipInfo(field, DocumentModel.ActivePieceTable);
			if (fieldInfo != null)
				return fieldInfo;
			return base.CalculateToolTipInfo(activeObject);
		}
		ToolTipControlInfo CreateFieldToolTipInfo(Field field, SnapPieceTable pieceTable) {
			DesignBinding binding = FieldsHelper.GetFieldDesignBinding(DocumentModel.DataSourceDispatcher, new SnapFieldInfo(pieceTable, field));
			string fieldDisplayName = binding.GetDisplayName(this.DocumentModel);
			if (String.IsNullOrEmpty(fieldDisplayName))
				return null;
			CalculatedFieldBase calculatedField = FieldsHelper.GetParsedInfoCore(pieceTable, field);
			SNTextField textField = calculatedField as SNTextField;
			string bodyText;
			if (textField != null && textField.SummaryRunning != SummaryRunning.None)
				bodyText = String.Format(SnapLocalizer.GetString(SnapStringId.SummaryTooltip), textField.SummaryFunc, fieldDisplayName, textField.SummaryRunning);
			else
				bodyText = fieldDisplayName;
			string dataSourceName = GetDataSourceDisplayName(binding);
			ToolTipControlInfo result = new ToolTipControlInfo();
			result.Object = field;
			result.ToolTipLocation = ToolTipLocation.RightTop;
			result.ToolTipType = ToolTipType.SuperTip;
			result.SuperTip = CreateSuperTip(dataSourceName, bodyText);
			return result;
		}
		string GetDataSourceDisplayName(DesignBinding binding) {
			IDataSourceDisplayNameProvider displayNameProvider = DocumentModel.GetService<IDataSourceDisplayNameProvider>();
			if (displayNameProvider == null || binding.DataSource == null)
				return null;
			string result = displayNameProvider.GetDataSourceName(binding.DataSource);
			if (!String.IsNullOrEmpty(result))
				return result;
			DataContext dataContext = new SnapDataContext(DocumentModel.DataSourceDispatcher.GetCalculatedFields(binding.DataSource), DocumentModel.Parameters, displayNameProvider);
			return DesignBindingHelper.GetListName(dataContext, binding.DataSource, binding.DataMember);
		}
		SuperToolTip CreateSuperTip(string headerText, string bodyText) {
			SuperToolTip superTip = new SuperToolTip();
			ToolTipTitleItem header = new ToolTipTitleItem();
			header.Text = headerText;
			superTip.Items.Add(header);
			ToolTipItem text = new ToolTipItem();
			text.Text = bodyText;
			superTip.Items.Add(text);
			return superTip;
		}
	}
}
