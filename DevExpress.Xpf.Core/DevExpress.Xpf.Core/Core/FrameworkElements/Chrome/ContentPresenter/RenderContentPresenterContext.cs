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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Native {
	public class RenderRealContentPresenterContext : RenderControlBaseContext {
		public object Content {
			get { return ContentPresenter.Content; }
			set { ContentPresenter.Content = value; }
		}
		public DataTemplate ContentTemplate {
			get { return ContentPresenter.ContentTemplate; }
			set { ContentPresenter.ContentTemplate = value; }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return ContentPresenter.ContentTemplateSelector; }
			set { ContentPresenter.ContentTemplateSelector = value; }
		}
		public string ContentStringFormat {
			get { return ContentPresenter.ContentStringFormat; }
			set { ContentPresenter.ContentStringFormat = value; }
		}		
		public bool RecognizesAccessKey {
			get { return ContentPresenter.RecognizesAccessKey; }
			set { ContentPresenter.RecognizesAccessKey = value; }
		}
		ContentPresenter ContentPresenter { get { return Control as ContentPresenter; } }
		public RenderRealContentPresenterContext(RenderRealContentPresenter factory)
			: base(factory) {
		}
		protected internal override void AttachToVisualTree(FrameworkElement root) {
			base.AttachToVisualTree(root);
			ContentControl cc = root.TemplatedParent as ContentControl;
			if (cc == null)
				return;
			ContentPresenter.Content = cc.Content;
			ContentPresenter.ContentTemplate = cc.ContentTemplate;
			ContentPresenter.ContentTemplateSelector = cc.ContentTemplateSelector;
			ContentPresenter.ContentStringFormat = cc.ContentStringFormat;
			ContentPresenter.HorizontalAlignment = cc.HorizontalContentAlignment;
			ContentPresenter.VerticalAlignment = cc.VerticalContentAlignment;
		}
		protected internal override void DetachFromVisualTree(FrameworkElement root) {
			base.DetachFromVisualTree(root);
		}
		protected override bool IsContextProperty(string propertyName) {
			if(string.Equals("Foreground", propertyName) ||
			   string.Equals("Visibility", propertyName))
				return true;
			return base.IsContextProperty(propertyName);
		}
	}
}
