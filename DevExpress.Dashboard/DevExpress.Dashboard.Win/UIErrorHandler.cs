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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.DashboardWin.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
namespace DevExpress.DashboardWin.Native {
	public class UIErrorHandler : IDataAccessErrorHandler, IErrorInfoVisitor {
		static Dictionary<ErrorType, DashboardWinStringId> errorDescriptionsTable = CreateErrorDescriptionsTable();
		static Dictionary<ErrorType, DashboardWinStringId> CreateErrorDescriptionsTable() {
			Dictionary<ErrorType, DashboardWinStringId> result = new Dictionary<ErrorType, DashboardWinStringId>();
			result.Add(ErrorType.DuplicateName, DashboardWinStringId.DataSourceNameExistsMessage);
			result.Add(ErrorType.EmptyName, DashboardWinStringId.DataSourceEmptyNameMessage);
			return result;
		}
		readonly DashboardDesigner designer;
		public UIErrorHandler(DashboardDesigner designer) {
			this.designer = designer;
		}
		public Dictionary<ErrorType, DashboardWinStringId> ErrorDescriptionsTable { get { return errorDescriptionsTable; } }
		public ErrorHandlingResult HandleError(IErrorInfo errorInfo) {
			if (errorInfo == null)
				return ErrorHandlingResult.Ignore;
			return errorInfo.Visit(this);
		}
		public ErrorHandlingResult Visit(TooltipErrorInfo info) {
			return ErrorHandlingResult.Ignore;
		}
		public ErrorHandlingResult Visit(ErrorInfo info) {
			DashboardWinStringId textDescriptionStringId = GetDescriptionStringId(info.ErrorType);
			DashboardWinHelper.ShowWarningMessage(designer.LookAndFeel, designer, DashboardWinLocalizer.GetString(textDescriptionStringId));
			return ErrorHandlingResult.Ignore;
		}
		DashboardWinStringId GetDescriptionStringId(ErrorType errorType) {
			return errorDescriptionsTable[errorType];
		}
	}
}
