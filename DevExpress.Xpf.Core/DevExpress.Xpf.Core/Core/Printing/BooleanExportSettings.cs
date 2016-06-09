﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using System.Windows;
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Printing {
	public static class BooleanExportSettings {
		public static readonly DependencyProperty BooleanValueProperty =
			DependencyPropertyManager.RegisterAttached("BooleanValue", typeof(bool?), typeof(BooleanExportSettings), new PropertyMetadata(ExportSettingDefaultValue.BooleanValue));
		public static readonly DependencyProperty CheckTextProperty =
			DependencyPropertyManager.RegisterAttached("CheckText", typeof(string), typeof(BooleanExportSettings), new PropertyMetadata(ExportSettingDefaultValue.CheckText));
		public static bool? GetBooleanValue(DependencyObject obj) {
			return (bool?)obj.GetValue(BooleanValueProperty);
		}
		public static void SetBooleanValue(DependencyObject obj, bool? value) {
			obj.SetValue(BooleanValueProperty, value);
		}
		public static string GetCheckText(DependencyObject obj) {
			return (string)obj.GetValue(CheckTextProperty);
		}
		public static void SetCheckText(DependencyObject obj, string value) {
			obj.SetValue(CheckTextProperty, value);
		}
	}
}
