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

using DevExpress.Data;
namespace DevExpress.Snap.Core.API {
	using System;
	using System.ComponentModel;
	using SummaryRunning = DevExpress.Snap.Core.Fields.SummaryRunning;
	using SummaryFuction = DevExpress.Data.SummaryItemType;
	public interface SnapText : SnapSingleListItemEntity {
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the DataFieldName property instead.")]
		string FieldName { get; set; }				  
		SummaryRunning SummaryRunning { get; set; }	 
		SummaryFuction SummaryFunc { get; set; }		
		bool SummaryIgnoreNullValues { get; set; }	  
		string TextBeforeIfFieldNotBlank { get; set; }  
		string TextAfterIfFieldNotBlank { get; set; }   
		string FormatString { get; set; }			   
		bool KeepLastParagraph { get; set; }			
		string TextFormat { get; set; }				 
	}
}
namespace DevExpress.Snap.API.Native {
	using System;
	using System.ComponentModel;
	using DevExpress.Snap.Core.API;
	using DevExpress.XtraRichEdit.API.Native;
	using DevExpress.Snap.Core.Fields;
	using DevExpress.XtraRichEdit.Fields;
	public class NativeSnapText : NativeSnapSingleListItemEntity, SnapText {
		SummaryRunning summaryRunning;
		SummaryItemType summaryFunc;
		bool summaryIgnoreNullValues;
		string textB, textF;
		string formatString;
		bool keepLastParagraph;
		string textFormat;
		public NativeSnapText(SnapNativeDocument document, Field field) : base(document, field) { }
		public NativeSnapText(SnapSubDocument subDocument, SnapNativeDocument document, Field field) : base(subDocument, document, field) { }
		protected override void Init() {
			base.Init();
			SNTextField parsedField = GetParsedField<SNTextField>();
			this.summaryRunning = parsedField.SummaryRunning;
			this.summaryFunc = parsedField.SummaryFunc;
			this.summaryIgnoreNullValues = parsedField.SummariesIgnoreNullValues;
			this.textB = parsedField.TextBeforeIfFieldNotBlank;
			this.textF = parsedField.TextAfterIfFieldNotBlank;
			this.formatString = parsedField.NumericFormatting;
			this.keepLastParagraph = parsedField.KeepLastParagraph;
			this.textFormat = parsedField.TextFormat;
		}
		#region SnapText Members
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the DataFieldName property instead.")]
		public string FieldName { get { return DataFieldName; } set { DataFieldName = value; } }
		public SummaryRunning SummaryRunning {
			get {
				return summaryRunning;
			}
			set {
				EnsureUpdateBegan();
				if(summaryRunning == value)
					return;
				Controller.SetSwitch(SNTextField.SummaryRunningSwitch, System.Enum.GetName(typeof(SummaryRunning), value));
				summaryRunning = value;
			}
		}
		public SummaryItemType SummaryFunc {
			get {
				return summaryFunc;
			}
			set {
				EnsureUpdateBegan();
				if(summaryFunc == value)
					return;
				Controller.SetSwitch(SNTextField.SummaryFuncSwitch, System.Enum.GetName(typeof(SummaryItemType), value));
				summaryFunc = value;
			}
		}
		public bool SummaryIgnoreNullValues {
			get {
				return summaryIgnoreNullValues;
			}
			set {
				EnsureUpdateBegan();
				if(summaryIgnoreNullValues == value)
					return;
				if(value)
					Controller.SetSwitch(SNTextField.SummariesIgnoreNullValuesSwitch);
				else
					Controller.RemoveSwitch(SNTextField.SummariesIgnoreNullValuesSwitch);
				Controller.RemoveSwitch(SNTextField.ObsoleteSummariesIgnoreNullValuesSwitch);
				summaryIgnoreNullValues = value;
			}
		}
		public string TextBeforeIfFieldNotBlank {
			get {
				return textB;
			}
			set {
				EnsureUpdateBegan();
				if(String.Equals(textB, value))
					return;
				Controller.SetSwitch(TextBeforeIfFieldNotBlankSwitch, value);
				textB = value;
			}
		}
		public string TextAfterIfFieldNotBlank {
			get {
				return textF;
			}
			set {
				EnsureUpdateBegan();
				if(String.Equals(textF, value))
					return;
				Controller.SetSwitch(TextAfterIfFieldNotBlankSwitch, value);
				textF = value;
			}
		}
		public string FormatString {
			get {
				return formatString;
			}
			set {
				EnsureUpdateBegan();
				if(String.Equals(formatString, value))
					return;
				Controller.SetSwitch(CalculatedFieldBase.FrameworkStringFormatSwitch, value);
				formatString = value;
			}
		}
		public bool KeepLastParagraph {
			get {
				return keepLastParagraph;
			}
			set {
				EnsureUpdateBegan();
				if (keepLastParagraph == value)
					return;
				if (value)
					Controller.SetSwitch(SNTextField.KeepLastParagraphSwitch);
				else
					Controller.RemoveSwitch(SNTextField.KeepLastParagraphSwitch);
				keepLastParagraph = value;
			}
		}
		public string TextFormat {
			get {
				return textFormat;
			}
			set {
				EnsureUpdateBegan();
				if (String.Equals(textFormat, value))
					return;
				Controller.SetSwitch(SNTextField.TextFormatSwitch, value);
				textFormat = value;
			}
		}
		#endregion
	}
}
