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
using System.Windows.Controls;
using System.Windows.Media.Animation;
namespace DevExpress.Xpf.RichEdit.Controls.Internal {
	public class XpfRichEditCaret : Control {
		public XpfRichEditCaret() {
			DefaultStyleKey = typeof(XpfRichEditCaret);
			Unloaded += OnUnloaded;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Storyboard sb = GetTemplateChild("Animate") as Storyboard;
			if (sb != null) {
#if !SL
				System.Windows.FrameworkElement caret= (GetTemplateChild("Caret") as System.Windows.FrameworkElement);
				System.Windows.FrameworkElement caretParent = caret != null ? caret.Parent as System.Windows.FrameworkElement : null;
				if(caretParent == null) {
					sb.Stop();
					sb.Begin();
				}
				else {
					sb.Begin(caretParent, true);
				}
#else
				sb.Stop();
				sb.Begin();
#endif
			}
		}
		void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			Storyboard sb = GetTemplateChild("Animate") as Storyboard;
			if (sb != null)
			{
#if !SL
				System.Windows.FrameworkElement caret= (GetTemplateChild("Caret") as System.Windows.FrameworkElement);
				System.Windows.FrameworkElement caretParent = caret != null ? caret.Parent as System.Windows.FrameworkElement : null;
				if (caretParent == null)
					sb.Stop();
				else
					sb.Stop(caretParent);
#else
				sb.Stop();
#endif
			}
		}
	}
	public class XpfRichEditDragCaret : XpfRichEditCaret {
		public XpfRichEditDragCaret() {
			DefaultStyleKey = typeof(XpfRichEditDragCaret);
		}
	}
}
