#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Editors {
	public enum EditMaskType { [Browsable(false)]Default, Simple, RegEx, DateTime }
	public class FormattingProvider {
		public const String TimeSpanDefaultEditMask = "([0-9]{1,5}\\.)??(([0-9])|([0-1][0-9])|([2][0123])):[0-5][0-9]:[0-5][0-9](\\.[0-9]{1,3})??";
		public const string DateTimeDefaultEditMask = "d";
		public const string DecimalDefaultEditMask = "C";
		public const string FloatDefaultEditMask = "#,###,##0.##############################";
		public const string IntegerDefaultEditMask = "N0";
		private static FormattingProvider instance = new FormattingProvider();
		public static void SetInstance(FormattingProvider newInstance) {
			instance = newInstance;
		}
		public static string GetEditMask(Type type) {
			return instance.GetEditMaskCore(type);
		}
		public static string GetDisplayFormat(Type type) {
			return instance.GetDisplayFormatCore(type);
		}
		public static EditMaskType GetEditMaskType(Type type) {
			return instance.GetEditMaskTypeCore(type);
		}
		protected virtual string GetEditMaskCore(Type type) {
			if(type == null) {
				return string.Empty;
			}
			Type fixedType = Nullable.GetUnderlyingType(type) ?? type;
			if(fixedType == typeof(TimeSpan)) {
				return TimeSpanDefaultEditMask;
			}
			else if(fixedType == typeof(DateTime)) {
				return DateTimeDefaultEditMask;
			}
			else if(fixedType == typeof(Decimal)) {
				return DecimalDefaultEditMask;
			}
			else if(fixedType == typeof(Single) || fixedType == typeof(Double)) {
				return FloatDefaultEditMask;
			}
			else if(fixedType == typeof(Int16) || fixedType == typeof(Int32) || fixedType == typeof(Int64) || fixedType == typeof(UInt16) || fixedType == typeof(UInt32) || fixedType == typeof(UInt64)) {
				return IntegerDefaultEditMask;
			}
			return string.Empty;
		}
		protected virtual string GetDisplayFormatCore(Type type) {
			if(type == null) {
				return string.Empty;
			}
			Type fixedType = Nullable.GetUnderlyingType(type) ?? type;
			if(fixedType == typeof(TimeSpan)) {
				return string.Empty;
			}
			string editMask = GetEditMask(fixedType);
			if(!string.IsNullOrEmpty(editMask)) {
				return "{0:" + editMask + "}";
			}
			return string.Empty;
		}
		protected virtual EditMaskType GetEditMaskTypeCore(Type type) {
			if(type == null) {
				return EditMaskType.Default;
			}
			Type fixedType = Nullable.GetUnderlyingType(type) ?? type;
			if(fixedType == typeof(TimeSpan)) {
				return EditMaskType.RegEx;
			}
			else if(fixedType == typeof(string)) {
				return EditMaskType.Simple;
			}
			return EditMaskType.Default;
		}
	}
}
