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
using System.ComponentModel;
using DevExpress.XtraEditors.DXErrorProvider;
namespace DevExpress.Xpf.Editors.Validation {
	public class BaseValidationError : INotifyPropertyChanged {
		internal bool IsHidden { get; set; }
		public BaseValidationError(object errorContent)
			: this(errorContent, null) {
		}
		public BaseValidationError(object errorContent, Exception exception)
			: this(errorContent, exception, ErrorType.Critical) {
		}
		public BaseValidationError(object errorContent, Exception exception, ErrorType errorType) {
			ErrorContent = errorContent;
			Exception = exception;
			ErrorType = errorType;
		}
		public BaseValidationError() : this(null) {
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseValidationErrorErrorContent")]
#endif
		public object ErrorContent { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseValidationErrorException")]
#endif
		public Exception Exception { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseValidationErrorErrorType")]
#endif
		public ErrorType ErrorType { get; private set; }
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { } remove { } }
	}
}
