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
using System.Windows;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class SchedulerControlBorder : DXSchedulerThemesLoader {
		#region VisibleBorderTemplate
		public ControlTemplate VisibleBorderTemplate {
			get { return (ControlTemplate)GetValue(VisibleBorderTemplateProperty); }
			set { SetValue(VisibleBorderTemplateProperty, value); }
		}
		public static readonly DependencyProperty VisibleBorderTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControlBorder, ControlTemplate>("VisibleBorderTemplate", null, (d, e) => d.OnVisibleBorderTemplateChanged(), null);
		void OnVisibleBorderTemplateChanged() {
			SelectTemplate();
		}
		#endregion
		#region InvisibleBorderTemplate
		public ControlTemplate InvisibleBorderTemplate {
			get { return (ControlTemplate)GetValue(InvisibleBorderTemplateProperty); }
			set { SetValue(InvisibleBorderTemplateProperty, value); }
		}
		public static readonly DependencyProperty InvisibleBorderTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControlBorder, ControlTemplate>("InvisibleBorderTemplate", null, (d, e) => d.OnInvisibleBorderTemplateChanged(), null);
		void OnInvisibleBorderTemplateChanged() {
			SelectTemplate();
		}
		#endregion
		#region ShowBorder
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		public static readonly DependencyProperty ShowBorderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControlBorder, bool>("ShowBorder", true, (d, e) => d.OnShowBorderChanged(), null);
		void OnShowBorderChanged() {
			SelectTemplate();
		}
		#endregion
		protected internal virtual void SelectTemplate() {
			ControlTemplate template = ShowBorder ? VisibleBorderTemplate : InvisibleBorderTemplate;
			if(!Object.ReferenceEquals(Template, template))
				Template = template;
		}
	}
}
