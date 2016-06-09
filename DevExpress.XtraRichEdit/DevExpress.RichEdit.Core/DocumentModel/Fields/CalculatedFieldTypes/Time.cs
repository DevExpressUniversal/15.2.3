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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Fields {
	public partial class TimeField : CalculatedFieldBase {
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "TIME";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument();
		public static CalculatedFieldBase Create() {
			return new TimeField();
		}
		#endregion
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		#endregion
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			IDateTimeService dateTimeService = sourcePieceTable.DocumentModel.GetService<IDateTimeService>();
			DateTime result = dateTimeService != null ? dateTimeService.GetCurrentDateTime() : DateTime.Now;
			return new CalculatedFieldValue(result);
		}
		protected internal override DateTimeFieldFormatter CreateDateTimeFieldFormatter() {
			return new TimeFieldFormatter() { UseCurrentCultureDateTimeFormat = this.UseCurrentCultureDateTimeFormat };
		}
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			UpdateFieldOperationType result = base.GetAllowedUpdateFieldTypes(options);
			if (options != null && options.UpdateTimeField)
				result |= UpdateFieldOperationType.Load;
			return result;
		}
	}
	public class TimeFieldFormatter : DateTimeFieldFormatter {
		readonly string defaultFormat = "h:mm am/pm";
		protected override string FormatByDefault(DateTime value) {
			if (!UseCurrentCultureDateTimeFormat)
				return Format(value, this.defaultFormat);
			return value.ToShortTimeString();
		}
	}
}
