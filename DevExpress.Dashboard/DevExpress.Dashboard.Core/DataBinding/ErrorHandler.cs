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

using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DataAccess.Native {
	public enum ErrorType {
		None,
		DuplicateName,
		EmptyName,
		WrongCharacterName
	}
	public interface IDataAccessErrorHandler {
		ErrorHandlingResult HandleError(IErrorInfo errorInfo);
	}
	public enum ErrorHandlingResult {
		Ignore,
		Abort,
	}
	public interface IErrorInfo {
		ErrorType ErrorType { get; }
		ErrorHandlingResult Visit(IErrorInfoVisitor visitor);
	}
	public interface IErrorInfoVisitor {
		ErrorHandlingResult Visit(ErrorInfo info);
		ErrorHandlingResult Visit(TooltipErrorInfo info);
	}
	public class ErrorInfo : IErrorInfo {
		readonly ErrorType errorType;
		public ErrorInfo(ErrorType errorType) {
			this.errorType = errorType;
		}
		public ErrorType ErrorType { get { return errorType; } }
		public ErrorHandlingResult Visit(IErrorInfoVisitor visitor) {
			return visitor.Visit(this);
		}
	}
	public class TooltipErrorInfo : IErrorInfo {
		readonly ErrorType errorType;
		readonly Point toolTipLocation;
		public TooltipErrorInfo(ErrorType errorType, Point toolTipLocation) {
			this.errorType = errorType;
			this.toolTipLocation = toolTipLocation;
		}
		public ErrorType ErrorType { get { return errorType; } }
		public Point ToolTipLocation { get { return toolTipLocation; } }
		public ErrorHandlingResult Visit(IErrorInfoVisitor visitor) {
			return visitor.Visit(this);
		}
	}
}
