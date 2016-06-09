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
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionEOMonth
	public class FunctionEOMonth : FunctionEDate {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "EOMONTH"; } }
		public override int Code { get { return 0x01C2; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue GetSerialNumberResult(int startDateSerialNumber, int countMonths, WorkbookDataContext context, bool dateIsText) {
			if (startDateSerialNumber == 0)
				startDateSerialNumber = 1;
			if (context.DateSystem == DateSystem.Date1900 && startDateSerialNumber == NumberDay29Feb1900)
				startDateSerialNumber = 59;
			bool temp = (context.DateSystem == DateSystem.Date1904 && dateIsText) ? !dateIsText : dateIsText;
			VariantValue endDateSerialNumber = base.GetSerialNumberResult(startDateSerialNumber, countMonths, context, temp);
			if (endDateSerialNumber.IsError)
				return endDateSerialNumber;
			DateTime dateTime = endDateSerialNumber.ToDateTime(context);
			int year = dateTime.Year;
			int month = dateTime.Month;
			int day = DateTime.DaysInMonth(year, month);
			VariantValue result = context.FromDateTime(new DateTime(year, month, day));
			if (context.DateSystem == DateSystem.Date1904 && dateIsText)
				return result.NumericValue - 1461;
			else
				return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			return collection;
		}
	}
	#endregion
}
