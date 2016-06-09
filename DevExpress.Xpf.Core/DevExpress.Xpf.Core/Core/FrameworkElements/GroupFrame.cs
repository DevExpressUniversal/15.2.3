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

using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Utils.Themes;
using System.Collections.Generic;
using System;
namespace DevExpress.Xpf.Core {
#if !SILVERLIGHT
	public class GroupFrame : GroupBox{
		public GroupFrame() {
			DefaultStyleKey = typeof(GroupFrame);
		}
	}
#else
	public class GroupFrame : ContentControl {
		public static readonly DependencyProperty HeaderProperty =
			 DependencyProperty.Register("Header", typeof(object), typeof(GroupFrame), 
			 new PropertyMetadata(null, (d, e) => ((GroupFrame)d).OnHeaderPropertyChanged()));
		public static readonly DependencyProperty HeaderTemplateProperty =
			 DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(GroupFrame), 
			 new PropertyMetadata(null, (d, e) => ((GroupFrame)d).OnHeaderTemplatePropertyChanged()));
		public object Header {
			get { return (object)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		bool hasHeader = false;
		public bool HasHeader {
			get { return hasHeader; }
			protected set {
				if(value == hasHeader)
					return;
				hasHeader = value;
				OnHasHeaderChanged();
			}
		}
		bool hasContent = false;
		public bool HasContent {
			get {
				UpdateHasContent();
				return hasContent; 
			}
			protected set {
				if(value == hasContent)
					return;
				hasContent = value;
				OnHasContentChanged();
			}
		}
		public GroupFrame() {
			this.DefaultStyleKey = typeof(GroupFrame);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateHasHeader();
			UpdateHasContent();
			UpdateHeaderVisiblityVisualState();
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			UpdateHasContent();
		}
		protected virtual void UpdateHasContent() {
			HasContent = Content != null || ContentTemplate != null;
		}
		protected virtual void UpdateHasHeader() {
			HasHeader = Header != null || HeaderTemplate != null;
		}
		protected virtual void OnHeaderPropertyChanged() {
			UpdateHasHeader();
		}
		protected virtual void OnHeaderTemplatePropertyChanged() {
			UpdateHasHeader();
		}
		protected virtual void OnHasHeaderChanged() {
			UpdateHeaderVisiblityVisualState();
		}
		protected virtual void OnHasContentChanged() {
		}
		protected virtual void UpdateHeaderVisiblityVisualState() {
			VisualStateManager.GoToState(this, HasHeader ? "HeaderVisible" : "HeaderCollapsed", false);
		}
	}
#endif
}
