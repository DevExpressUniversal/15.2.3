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
namespace DevExpress.Xpf.Editors.Validation.Native {
	public class StrategyValidatorBase {
		protected BaseEdit Editor { get; private set; }
		protected ActualPropertyProvider PropertyProvider { get { return Editor.PropertyProvider; } }
		public StrategyValidatorBase(BaseEdit editor) {
			Editor = editor;
		}
		public bool DoValidate(object value, object convertedValue, UpdateEditorSource updateSource) {
			return DoValidateInternal(value, convertedValue, updateSource);
		}
		protected virtual bool DoValidateInternal(object value, object convertedValue, UpdateEditorSource updateSource) {
			return IsValidValue(value, convertedValue);
		}
		protected virtual bool IsValidValue(object value, object convertedValue) {
			return true;
		}
		public virtual object ProcessConversion(object value) {
			if (value is InvalidInputValue) {
				var iiv = (InvalidInputValue)value;
				return iiv.EditValue;
			}
			return PropertyProvider.ValueTypeConverter.ConvertBack(value);
		}
		public virtual string GetValidationError() {
			return "Invalid value";
		}
	}
}
