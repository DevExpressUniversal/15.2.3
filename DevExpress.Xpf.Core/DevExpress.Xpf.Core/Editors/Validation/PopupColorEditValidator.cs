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
using System.Windows.Media;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
#if !SL
using ColorConverter = System.Windows.Media.ColorConverter;
#endif
namespace DevExpress.Xpf.Editors.Validation.Native {
	public class PopupColorEditValidator : StrategyValidatorBase {
		public PopupColorEditValidator(PopupColorEdit editor)
			: base(editor) {
		}
		public override object ProcessConversion(object value) {
			object result = base.ProcessConversion(value);
			if (result == null)
				return null;
			string resultString = result.ToString();
			try {
				return ColorConverter.ConvertFromString(resultString);
			}
			catch {
				return null;
			}
		}
	}
#if SL
	static class ColorConverter {
		public static Color ConvertFromString(string value) {
			return (Color)new ComponentModel.ColorConverter().ConvertFrom(null, CultureInfo.CurrentCulture, value);
		}
	}
#endif
#if !SL
	public class PopupBrushEditValidator : StrategyValidatorBase {
		public PopupBrushEditValidator(PopupBrushEditBase editor)
			: base(editor) {
		}
		public override object ProcessConversion(object value) {
			object result = base.ProcessConversion(value);
			var lookUpItem = result as LookUpEditableItem;
			if (lookUpItem == null)
				return result;
			Color resultColor;
			if (Text2ColorHelper.TryConvert(lookUpItem.DisplayValue, out resultColor))
				return new SolidColorBrush(resultColor);
			return lookUpItem.EditValue;
		}
	}
#endif
}
