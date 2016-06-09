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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.RangeControl {
	public abstract class CalendarItemBase : Control {
		public static readonly DependencyProperty TextProperty;
		static CalendarItemBase() {
			TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CalendarItemBase));
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
	public class CalendarItem : CalendarItemBase {
		public static readonly DependencyProperty IsSelectedProperty;
		static CalendarItem() {
			Type ownerType = typeof(CalendarItem);
			IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
		}
		public CalendarItem() {
			DefaultStyleKey = typeof(CalendarItem);
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
	}
	public class CalendarGroupItem : CalendarItemBase {
		public CalendarGroupItem() {
			DefaultStyleKey = typeof(CalendarGroupItem);
		}
		TextBlock textContainer;
		Thickness defaultTextMargin = new Thickness();
		public override void OnApplyTemplate() {
			textContainer = LayoutHelper.FindElementByType(this, typeof(TextBlock)) as TextBlock;
			if (textContainer != null)
				defaultTextMargin = textContainer.Margin;
		}
		internal void SetTextOffset(double offset) {
			if (textContainer != null)
				textContainer.Margin = new Thickness(defaultTextMargin.Left + offset, defaultTextMargin.Top, defaultTextMargin.Right, defaultTextMargin.Bottom);
		}
		internal Thickness GetTextMargin() {
			return defaultTextMargin;
		}
	}
}
