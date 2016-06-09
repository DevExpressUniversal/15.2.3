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
using System.Globalization;
using System.Windows;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Editors.Validation.Native;
#if SL
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Editors {
	public class ValidationEventArgs : RoutedEventArgs {
		public ValidationEventArgs(RoutedEvent routedEvent, object source, object value, CultureInfo culture, UpdateEditorSource updateSource)
			: base(routedEvent, source) {
			Value = value;
			Culture = culture;
			UpdateSource = updateSource;
			SetError(null);
		}
		public ValidationEventArgs(RoutedEvent routedEvent, object source, object value, CultureInfo culture)
			: this(routedEvent, source, value, culture, UpdateEditorSource.DoValidate) {
		}
		internal Exception Exception { get; set; }
		public object ErrorContent { get; set; }
		public ErrorType ErrorType { get; set; }
		public bool IsValid { get; set; }
		public object Value { get; private set; }
		public CultureInfo Culture { get; private set; }
		public UpdateEditorSource UpdateSource{ get; private set; }
		public void SetError(object errorContent) {
			SetErrorCore(errorContent);
			ErrorType = IsValid ? ErrorType.None : ErrorType.Default;
		}
		public void SetError(object errorContent, ErrorType errorType) {
			SetErrorCore(errorContent);
			ErrorType = errorType;
		}
		void SetErrorCore(object errorContent) {
			ErrorContent = errorContent;
			IsValid = (ErrorContent == null || string.IsNullOrEmpty(errorContent as string)) ? true : false;
		}
	}
}
