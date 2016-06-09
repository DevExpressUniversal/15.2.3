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
using System.Globalization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services;
namespace DevExpress.XtraRichEdit.Fields {
	public partial class DateField : CalculatedFieldBase {
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "DATE";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument();
		public static CalculatedFieldBase Create() {
			return new DateField();
		}
		#endregion
		bool useLunarCalendar;
		bool useSakaEraCalendar;
		bool useLastFormat;
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } } 
		public bool UseLunarCalendar { get { return useLunarCalendar; } }
		public bool UseSakaEraCalendar { get { return useSakaEraCalendar; } }
		public bool UseLastFormat { get { return useLastFormat; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			useLunarCalendar = instructions.GetBool("h");
			useSakaEraCalendar = instructions.GetBool("s");
			useLastFormat = instructions.GetBool("l");
		}
		#endregion
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			IDateTimeService dateTimeService = sourcePieceTable.DocumentModel.GetService<IDateTimeService>();
			DateTime result = dateTimeService != null ? dateTimeService.GetCurrentDateTime() : DateTime.Now;
			return new CalculatedFieldValue(result);
		}
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			UpdateFieldOperationType result = base.GetAllowedUpdateFieldTypes(options);
			if (options != null && options.UpdateDateField)
				result |= UpdateFieldOperationType.Load;
			return result;
		}
	}
}
