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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public class WrapHighlightTextBlock : Control, INotifyPropertyChanged {
		static WrapHighlightTextBlock() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(WrapHighlightTextBlock), new FrameworkPropertyMetadata(typeof(WrapHighlightTextBlock)));
		}
		#region Dependency Property
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
			typeof(WrapHighlightTextBlock), new UIPropertyMetadata(string.Empty, UpdateWrapHighlightTextBlock));
		public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText", typeof(string),
			typeof(WrapHighlightTextBlock), new UIPropertyMetadata(string.Empty, UpdateWrapHighlightTextBlock));
		#endregion
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public string SearchText {
			get { return (string)GetValue(SearchTextProperty); }
			set { SetValue(SearchTextProperty, value); }
		}
		static void UpdateWrapHighlightTextBlock(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((WrapHighlightTextBlock)d).InvalidateVisual();
		}
		protected override void OnRender(DrawingContext drawingContext) {
			if(Template == null || string.IsNullOrEmpty(this.Text)) {
				base.OnRender(drawingContext);
				return;
			}
			TextBlock displayTextBlock = this.Template.FindName("PART_TEXT", this) as TextBlock;
			if(displayTextBlock == null) {
				throw new NullReferenceException("TextBlock with the PART_TEXT name is not found");
			}
			displayTextBlock.Inlines.Clear();
			string searchstring = ((string)this.SearchText).ToUpper();
			string compareText = this.Text.ToUpper();
			string displayText = this.Text;
			Run run = null;
			while(!string.IsNullOrEmpty(searchstring) && compareText.IndexOf(searchstring) >= 0) {
				int position = compareText.IndexOf(searchstring);
				run = GenerateRun(displayText.Substring(0, position), false);
				if(run != null) {
					displayTextBlock.Inlines.Add(run);
				}
				run = GenerateRun(displayText.Substring(position, searchstring.Length), true);
				if(run != null) {
					displayTextBlock.Inlines.Add(run);
				}
				compareText = compareText.Substring(position + searchstring.Length);
				displayText = displayText.Substring(position + searchstring.Length);
			}
			run = GenerateRun(displayText, false);
			if(run != null) {
				displayTextBlock.Inlines.Add(run);
			}
			base.OnRender(drawingContext);
		}
		Run GenerateRun(string searchedString, bool isHighlight) {
			if(!string.IsNullOrEmpty(searchedString)) {
				Run run = new Run(searchedString) {
					Background = isHighlight ? Brushes.Yellow : this.Background,
					Foreground = isHighlight ? Brushes.Black : this.Foreground,
				};
				return run;
			}
			return null;
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
