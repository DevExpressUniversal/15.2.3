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
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace DevExpress.Design.SmartTags {
	public static class TextBlockService {
		public static bool GetIsTextTrimmed(DependencyObject obj) {
			return (bool)obj.GetValue(IsTextTrimmedProperty);
		}
		public static void SetIsTextTrimmed(DependencyObject obj, bool value) {
			obj.SetValue(IsTextTrimmedProperty, value);
		}
		public static readonly DependencyProperty IsTextTrimmedProperty;
		static TextBlockService() {
			IsTextTrimmedProperty = DependencyProperty.RegisterAttached("IsTextTrimmed", typeof(bool), typeof(TextBlockService), new PropertyMetadata(false));
		}
		public static bool GetIsActualTextBlockTrimmed(TextBlock txtBlock) {
			if(txtBlock == null || txtBlock.TextTrimming == TextTrimming.None) {
				return false;
			} else {
				var typeface = new Typeface(txtBlock.FontFamily, txtBlock.FontStyle, txtBlock.FontWeight, txtBlock.FontStretch);
				var frmdText = new FormattedText(txtBlock.Text, Thread.CurrentThread.CurrentCulture, txtBlock.FlowDirection, typeface, txtBlock.FontSize, txtBlock.Foreground);
				return frmdText.Width > txtBlock.ActualWidth;
			}
		}
	}
}
