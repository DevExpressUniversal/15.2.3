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
using DevExpress.Xpf.Utils;
using System.Windows.Input;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using System.Windows.Media;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.RichEdit.Controls.Internal {
	public class RichEditComment : RichEditElement {
		#region Dependency Properties
		public static readonly DependencyProperty RootRenderTransformProperty = DependencyPropertyManager.Register("RootRenderTransform", typeof(Transform), typeof(RichEditComment), new FrameworkPropertyMetadata(Transform.Identity));
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Transform RootRenderTransform {
			get { return (Transform)GetValue(RootRenderTransformProperty); }
			set { SetValue(RootRenderTransformProperty, value); }
		}
		#region MoreButtonVisibility
		public static readonly DependencyProperty MoreButtonVisibilityProperty = DependencyPropertyManager.Register("MoreButtonVisibility", typeof(Visibility), typeof(RichEditComment), new FrameworkPropertyMetadata());
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Visibility MoreButtonVisibility {
			get { return (Visibility)GetValue(MoreButtonVisibilityProperty); }
			set { SetValue(MoreButtonVisibilityProperty, value); }
		}
		#endregion
		#region MoreButtonCommand
#if !SL
		public static  DependencyPropertyKey MoreButtonCommandPropertyKey = DependencyProperty.RegisterReadOnly("MoreButtonCommand", typeof(ICommand), typeof(RichEditComment), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty MoreButtonCommandProperty = MoreButtonCommandPropertyKey.DependencyProperty;
		public ICommand MoreButtonCommand {
			get { return (ICommand)GetValue(MoreButtonCommandProperty); }
			protected internal set { SetValue(MoreButtonCommandPropertyKey, value); }
		}
#else
		public static readonly DependencyProperty MoreButtonCommandProperty = DependencyProperty.Register("MoreButtonCommand", typeof(ICommand), typeof(RichEditComment), new FrameworkPropertyMetadata());
		public ICommand MoreButtonCommand {
			get { return (ICommand)GetValue(MoreButtonCommandProperty); }
			protected internal set { SetValue(MoreButtonCommandProperty, value); }
		}
#endif
		#endregion
		#endregion
		public RichEditComment() {
			DefaultStyleKey = typeof(RichEditComment);			
		}
	}
	public class ShowReviewPaneCommand : ICommand {		
		RichEditControl control;
		CommentViewInfo commentViewInfo;
		public ShowReviewPaneCommand(RichEditControl control, CommentViewInfo commentViewInfo) {
			this.control = control;
			this.commentViewInfo = commentViewInfo;
		}
		public bool CanExecute(object parameter) {
			return true;
		}
		public event EventHandler CanExecuteChanged;
		protected void RaiseCanExecuteChanged() {
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}
		public void Execute(object parameter) {
			control.ShowReviewingPaneForm(control.DocumentModel, commentViewInfo, false, DocumentLogPosition.Zero, DocumentLogPosition.Zero, false);
		}
	}
}
