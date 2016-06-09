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
namespace DevExpress.Data.PivotGrid {
	public enum ExpressionExceptionType { OrdinaryFieldAccessDataField, ColumnNotExists };
	public class ExpressionException : Exception {
#if DEBUGTEST
		public static int ExceptionCounter;
#endif
		ExpressionExceptionType type;
		public ExpressionException(ExpressionExceptionType type) {
			this.type = type;
#if DEBUGTEST
			ExceptionCounter++;
#endif
		}
		public ExpressionExceptionType Type { get { return type; } }
		public bool AppliesToAllRows {
			get {
				switch(Type) {
					case ExpressionExceptionType.OrdinaryFieldAccessDataField:
					case ExpressionExceptionType.ColumnNotExists:
						return true;
					default:
						return false;
				}
			}
		}
	}
}
namespace DevExpress.XtraPivotGrid.Data {
	public class IncorrectFilterItemValueTypeException : Exception {
		public const string DefaultMessage = "Incorrect filter item value type due to data column type";
		public IncorrectFilterItemValueTypeException() : base() { }
		public IncorrectFilterItemValueTypeException(string message) : base(message) { }
		public IncorrectFilterItemValueTypeException(string message, Exception innerException) : base(message, innerException) { }
	}
	public class NotLoadedValuesException : Exception {
		public const string DefaultMessage = "The values are incorrect, because they weren't loaded from the stream.";
		public NotLoadedValuesException() : base() { }
		public NotLoadedValuesException(string message) : base(message) { }
	}
	public class IncorrectAsyncOperationCallException : Exception {
		public const string DefaultMessage = "Incorrect async operation call.";
		public IncorrectAsyncOperationCallException() : base() { }
		public IncorrectAsyncOperationCallException(string message) : base(message) { }
	}
	public class InvalidOperationOnLockedUpdateException : Exception {
		public const string DefaultMessage = @"This operation cannot be executed when pivot grid updates are blocked. The control's visual presentation is created after the EndUpdate method is called, so that you cannot perform actions that affect it within the BeginUpdate and EndUpdate method calls.";
		public InvalidOperationOnLockedUpdateException() : this(DefaultMessage) { }
		public InvalidOperationOnLockedUpdateException(string message) : base(message) { }
	}
}
