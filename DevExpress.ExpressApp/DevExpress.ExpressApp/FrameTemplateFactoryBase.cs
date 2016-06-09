#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Templates;
namespace DevExpress.ExpressApp {
	public interface IFrameTemplateFactory {
		IFrameTemplate CreateTemplate(TemplateContext context);
	}
	public abstract class FrameTemplateFactoryBase : IFrameTemplateFactory {
		protected abstract IFrameTemplate CreateNestedFrameTemplate();
		protected abstract IFrameTemplate CreatePopupWindowTemplate();
		protected abstract IFrameTemplate CreateLookupControlTemplate();
		protected abstract IFrameTemplate CreateLookupWindowTemplate();
		protected abstract IFrameTemplate CreateApplicationWindowTemplate();
		protected abstract IFrameTemplate CreateViewTemplate();
		protected virtual IFrameTemplate CreateTemplateCore(TemplateContext context) {
			if(context == TemplateContext.NestedFrame) {
				return CreateNestedFrameTemplate();
			}
			else if(context == TemplateContext.PopupWindow) {
				return CreatePopupWindowTemplate();
			}
			else if(context == TemplateContext.LookupControl) {
				return CreateLookupControlTemplate();
			}
			else if(context == TemplateContext.LookupWindow) {
				return CreateLookupWindowTemplate();
			}
			else if(context == TemplateContext.ApplicationWindow.Name) {
				return CreateApplicationWindowTemplate();
			}
			else if(context == TemplateContext.View) {
				return CreateViewTemplate();
			}
			return null;
		}
		public IFrameTemplate CreateTemplate(TemplateContext context) {
			IFrameTemplate template = CreateTemplateCore(context);
			if(template == null) {
				throw new ArgumentException(string.Format("Cannot create a template for the {0} context.", context), "context");
			}
			return template;
		}
	}
}
