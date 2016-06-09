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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.XtraRichEdit.Drawing;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.RichEdit.Controls.Internal {
	public abstract class RichEditElement : ContentControl { 
		XpfDrawingSurface surface;
		UIElementCollection children;
		protected RichEditElement() {
			DefaultStyleKey = typeof(RichEditElement);
			VerticalAlignment = System.Windows.VerticalAlignment.Top;
		}
		public Panel SuperRoot { get { return GetTemplateChild("SuperRoot") as Panel; } } 
		protected internal Grid Root { get { return GetTemplateChild("Root") as Grid; } }
		public virtual UIElementCollection Children { get { return children; } }
		public bool IsReady { get; protected set; }
		protected internal XpfDrawingSurface DrawingSurface { get { return surface; } }
		public event EventHandler PageReady;
		void RaisePageReady() {
			if (PageReady != null)
				PageReady(this, EventArgs.Empty);
		}
		protected virtual UIElementCollection GetChildren() {
			return Root.Children;
		}
		protected virtual void CreateChildren() {
			UIElementCollection previousChildren = this.children;
			this.children = GetChildren();
			if (previousChildren != null) {
				int count = previousChildren.Count;
				for (int i = 0; i < count; i++) {
					UIElement child = previousChildren[0];
					previousChildren.RemoveAt(0);
					children.Add(child);
				}
			}
			this.surface = new XpfDrawingSurface(children, this.surface != null ? this.surface.CustomPaintedObjectsIndices : null);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CreateChildren();
			RaisePageReady();
			IsReady = true;
		}
	}
	public class RichEditViewPageContentControl : ContentControl {
		public RichEditViewPageContentControl() {
			DefaultStyleKey = typeof(RichEditViewPageContentControl);
		}
		#region Dependency Properties
		public static readonly DependencyProperty CommentsWidthProperty = DependencyPropertyManager.Register("CommentsWidth", typeof(double), typeof(RichEditViewPageContentControl), new FrameworkPropertyMetadata());
		public double CommentsWidth {
			get { return (double)GetValue(CommentsWidthProperty); }
			set { SetValue(CommentsWidthProperty, value); }
		}
		public static readonly DependencyProperty CommentsVisibilityProperty = DependencyPropertyManager.Register("CommentsVisibility", typeof(Visibility), typeof(RichEditViewPageContentControl), new FrameworkPropertyMetadata());
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Visibility CommentsVisibility {
			get { return (Visibility)GetValue(CommentsVisibilityProperty); }
			set { SetValue(CommentsVisibilityProperty, value); }
		}
		#endregion
		protected internal Grid CommentRoot {
			get {
				RichEditViewCommentsPresenter commentsArea = GetTemplateChild("CommentsArea") as RichEditViewCommentsPresenter;
				if (commentsArea != null && commentsArea.Root == null)
					commentsArea.ApplyTemplate();
				return commentsArea != null ? commentsArea.Root : null;
			}
		}
	}
	public class RichEditViewCommentsPresenter : RichEditElement {
		public RichEditViewCommentsPresenter() {
			DefaultStyleKey = typeof(RichEditViewCommentsPresenter);
		}
	}
	public class RichEditViewPage : RichEditElement {
		UIElementCollection commentChildren;
		#region Dependency Properties
		public static readonly DependencyProperty CommentsWidthProperty = DependencyPropertyManager.Register("CommentsWidth", typeof(double), typeof(RichEditViewPage), new FrameworkPropertyMetadata());
		[ Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public double CommentsWidth {
			get { return (double)GetValue(CommentsWidthProperty); }
			set { SetValue(CommentsWidthProperty, value); }
		}
		public static readonly DependencyProperty CommentsHeightProperty = DependencyPropertyManager.Register("CommentsHeight", typeof(double), typeof(RichEditViewPage), new FrameworkPropertyMetadata());
		[ Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public double CommentsHeight {
			get { return (double)GetValue(CommentsHeightProperty); }
			set { SetValue(CommentsHeightProperty, value); }
		}
		public static readonly DependencyProperty CommentsLeftProperty = DependencyPropertyManager.Register("CommentsLeft", typeof(double), typeof(RichEditViewPage), new FrameworkPropertyMetadata());
		[ Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public double CommentsLeft {
			get { return (double)GetValue(CommentsLeftProperty); }
			set { SetValue(CommentsLeftProperty, value); }
		}
		public static readonly DependencyProperty CommentsTopProperty = DependencyPropertyManager.Register("CommentsTop", typeof(double), typeof(RichEditViewPage), new FrameworkPropertyMetadata());
		[ Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public double CommentsTop {
			get { return (double)GetValue(CommentsTopProperty); }
			set { SetValue(CommentsTopProperty, value); }
		}
		public static readonly DependencyProperty CommentsVisibilityProperty = DependencyPropertyManager.Register("CommentsVisibility", typeof(Visibility), typeof(RichEditViewPage), new FrameworkPropertyMetadata());
		[ Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Visibility CommentsVisibility {
			get { return (Visibility)GetValue(CommentsVisibilityProperty); }
			set { SetValue(CommentsVisibilityProperty, value); }
		}
		#endregion
		public RichEditViewPage() {
			DefaultStyleKey = typeof(RichEditViewPage);
			VerticalAlignment = System.Windows.VerticalAlignment.Top;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
		}
		public virtual UIElementCollection CommentChildren { get { return commentChildren; } }
		protected internal Grid CommentRoot {
			get {
				RichEditViewPageContentControl pageContentControl = GetTemplateChild("PageContentRoot") as RichEditViewPageContentControl;
				if (pageContentControl == null)
					return null;
				if (pageContentControl.CommentRoot == null)
					pageContentControl.ApplyTemplate();
				return pageContentControl.CommentRoot;
			}
		}
		protected virtual UIElementCollection GetCommentChildren() {
			return CommentRoot != null ? CommentRoot.Children : null;
		}
		protected void CreateCommentChildren() {
			UIElementCollection previouscommentChildren = this.commentChildren;
			this.commentChildren = GetCommentChildren();
			if (previouscommentChildren != null) {
				int count = previouscommentChildren.Count;
				for (int i = 0; i < count; i++) {
					UIElement commentchild = previouscommentChildren[0];
					previouscommentChildren.RemoveAt(0);
					if(commentChildren != null)
						commentChildren.Add(commentchild);
				}
			}
		}
		protected override void CreateChildren() {
			base.CreateChildren();
			CreateCommentChildren();
		} 
	}
	public class SimpleViewPage : RichEditViewPage {
		public SimpleViewPage() {
			DefaultStyleKey = typeof(SimpleViewPage);
		}
	}
	public class DraftViewPage : RichEditViewPage {
		public DraftViewPage() {
			DefaultStyleKey = typeof(DraftViewPage);
		}
	}
}
