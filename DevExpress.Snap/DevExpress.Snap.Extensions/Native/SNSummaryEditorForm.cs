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
using System.Windows.Forms;
using DevExpress.XtraReports.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Fields;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Snap.Extensions.Localization;
using DevExpress.XtraEditors;
namespace DevExpress.Snap.Extensions.Native {
	public class SNSummaryEditorForm : SummaryEditorForm {
		#region inner classes
		class Entity {
			public object Column { get; set; }
		}
		#endregion
		bool isValidFormatString;
		public SNSummaryEditorForm(DesignBinding designBinding, SummaryRunning summaryRunning, SummaryItemType summaryItemType, string stringFormat, bool ignoreNullValues, IServiceProvider provider)
			: base(designBinding, summaryItemType, typeof(SummaryItemType), stringFormat, ignoreNullValues, summaryRunning, provider) {
		}
		protected override FormatStringEditorForm GetFormatStringEditorForm(string formatString, IServiceProvider serviceProvider) {
			SNFormatStringEditorForm form = new SNFormatStringEditorForm(formatString, serviceProvider);
			form.TopMost = true;
			return form;
		}
		protected override void InitializeRunningSummaryRadioGroup(System.ComponentModel.ComponentResourceManager resources) {
			this.radioGroup1.Properties.Items.AddRange(new RadioGroupItem[] {
																				new RadioGroupItem(SummaryRunning.None, (string)(resources.GetObject("radioGroup1.Item0.Description"))),
																				new RadioGroupItem(SummaryRunning.Group, (string)(resources.GetObject("radioGroup1.Item2.Description"))),
																				new RadioGroupItem(SummaryRunning.Report, (string)(resources.GetObject("radioGroup1.Item3.Description")))});
		}
		protected override object CalcSummaryResult(System.Collections.ArrayList values) {
			List<Entity> list = new List<Entity>();
			foreach (var item in values)	
				list.Add(new Entity() { Column = item });
			using (ListSourceDataController listSourceDataController = new ListSourceDataController()) {
				listSourceDataController.ListSource = list;
				DataColumnInfo dataColumnInfo = listSourceDataController.Columns["Column"];
				SummaryItem summaryItem = new SummaryItem(dataColumnInfo, Func);
				listSourceDataController.TotalSummary.Add(summaryItem);
				return summaryItem.SummaryValue;
			}
		}
		protected override void SetDesignBinding(DesignBinding newDesignBinding) {
			this.fDesignBinding = new DesignBinding(fDesignBinding.DataSource, string.IsNullOrEmpty(newDesignBinding.DataMember) ? string.Empty : newDesignBinding.DataMember);
		}
		protected override string GetFormatString() {
			return MailMergeFieldInfo.MakeFormatString(FormatString);
		}
		protected override string GetResultString(object val) {
			isValidFormatString = true;
			if (Object.ReferenceEquals(val, null))
				return "<null>";
			string formatString = GetFormatString();
			if (String.IsNullOrEmpty(formatString))
				return val.ToString();
			string result;
			try {
				result = String.Format(formatString, val);
			}
			catch {
				result = SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.Msg_Error);
				isValidFormatString = false;
			}
			return result;
		}
		protected override void ValidateFormatString() {
			if (!isValidFormatString)
				XtraMessageBox.Show(LookAndFeel, this, SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.Msg_ContainsIllegalSymbols), SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.Msg_Error), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		protected override DialogResult GetAcceptButtonDialogResult() {
			return isValidFormatString ? DialogResult.OK : DialogResult.None;
		}
		public SummaryRunning Running {
			get { return (SummaryRunning)radioGroup1.EditValue; }
		}
		public SummaryItemType Func {
			get {
				TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(SummaryItemType));
				return (SummaryItemType)typeConverter.ConvertFromString((string)cbSummaryFunction.EditValue);
			}
		}
	}
}
