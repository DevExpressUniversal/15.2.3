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
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.UI;
namespace DevExpress.Web.Mvc.Internal {
	[ToolboxItem(false)]
	public class ContentControl<T> : Control where T : Control {
		protected ContentControl(string content, Action contentMethod, Action<T> contentMethodEx, T context, Type contextType, bool isReplaceWriter) {
			Content = content;
			ContentMethod = contentMethod;
			ContentMethodEx = contentMethodEx;
			Context = context;
			ContextType = contextType;
			IsReplaceWriter = isReplaceWriter;
		}
		public string Content { get; protected set; }
		public Action ContentMethod { get; protected set; }
		public Action<T> ContentMethodEx { get; protected set; }
		public new T Context { get; protected set; }
		public Type ContextType { get; protected set; }
		public bool IsReplaceWriter { get; protected set; }
		public static Control Create(string content) {
			return Create(content, null, null, null);
		}
		public static Control Create(Action<T> contentMethod, T context) {
			return Create(string.Empty, contentMethod, context, null);
		}
		public static Control Create(Action<T> contentMethod, Type contextType) {
			return Create(string.Empty, contentMethod, null, contextType);
		}
		public static Control Create(string content, Action<T> contentMethod, T context) {
			return Create(content, contentMethod, context, null);
		}
		public static Control Create(string content, Action<T> contentMethod, Type contextType) {
			return Create(content, contentMethod, null, contextType);
		}
		public static Control Create(string content, Action<T> contentMethod, T context, Type contextType) {
			return Create(content, contentMethod, context, contextType, false);
		}
		public static Control Create(string content, Action<T> contentMethod, T context, Type contextType, bool isReplaceWriter) {
			return new ContentControl<T>(content, null, contentMethod, context, contextType, isReplaceWriter);
		}
		protected internal static Control Create(string content, Action contentMethod, Action<T> contentMethodEx, T context, Type contextType, bool isReplaceWriter) {
			return new ContentControl<T>(content, contentMethod, contentMethodEx, context, contextType, isReplaceWriter);
		}
		protected override void Render(HtmlTextWriter writer) {
			TextWriter writerOfView = null;
			if (IsReplaceWriter) {
				writerOfView = ViewContext.Writer;
				ViewContext.Writer = writer;
			}
			try {
				RenderInternal(writer);
			}
			finally {
				if(IsReplaceWriter)
					ViewContext.Writer = writerOfView;
			}
		}
		void RenderInternal(HtmlTextWriter writer) {
			if (!string.IsNullOrEmpty(Content))
				writer.Write(Content);
			else if (ContentMethod != null)
				ContentMethod();
			else if (ContentMethodEx != null) {
				T content = null;
				if (Context != null)
					content = Context;
				else if (ContextType != null) {
					content = FindContextByType(ContextType);
					if (content == null)
						throw new Exception(string.Format("Cannot find render method context by the '{0}' type.", ContextType));
				}
				if (content == null)
					throw new Exception("Cannot find render method context.");
				ContentMethodEx(content);
			}
		}
		ViewContext viewContext;
		ViewContext ViewContext {
			get {
				if (viewContext == null)
					viewContext = GetViewContextByControl(this);
				return viewContext;
			}
		}
		ViewContext GetViewContextByControl(Control control) {
			if (control == null) return null;
			return control is IViewContext ? ((IViewContext)control).ViewContext : GetViewContextByControl(control.Parent);
		}
		protected T FindContextByType(Type type) {
			Control control = Parent;
			while (control != null) {
				if (control.GetType() == type || control.GetType().IsSubclassOf(type))
					return (T)control;
				control = control.Parent;
			}
			return null;
		}
	}
	public class ContentControlTemplate<T>: ITemplate where T: Control {
		protected ContentControlTemplate(string content, Action contentMethod, Action<T> contentMethodEx, T context, Type contextType, bool isReplaceWriter) {
			Content = content;
			ContentMethod = contentMethod;
			ContentMethodEx = contentMethodEx;
			Context = context;
			ContextType = contextType;
			IsReplaceWriter = isReplaceWriter;
		}
		public string Content { get; protected set; }
		public Action ContentMethod { get; protected set; }
		public Action<T> ContentMethodEx { get; protected set; }
		public T Context { get; protected set; }
		public Type ContextType { get; protected set; }
		public bool IsReplaceWriter { get; protected set; }
		public static ITemplate Create(string content) {
			return Create(content, null, null, null);
		}
		public static ITemplate Create(Action<T> contentMethod, T context) {
			return Create(string.Empty, contentMethod, context, null);
		}
		public static ITemplate Create(Action<T> contentMethod, Type contextType) {
			return Create(string.Empty, contentMethod, null, contextType);
		}
		public static ITemplate Create(string content, Action<T> contentMethod, T context) {
			return Create(content, contentMethod, context, null);
		}
		public static ITemplate Create(string content, Action<T> contentMethod, Type contextType) {
			return Create(content, contentMethod, null, contextType);
		}
		public static ITemplate Create(string content, Action<T> contentMethod, Type contextType, bool isReplaceWriter) {
			return Create(content, contentMethod, null, contextType, isReplaceWriter);
		}
		protected static ITemplate Create(string content, Action<T> contentMethod, T context, Type contextType) {
			return Create(content, null, contentMethod, context, contextType, false);
		}
		protected static ITemplate Create(string content, Action<T> contentMethod, T context, Type contextType, bool isReplaceWriter) {
			return Create(content, null, contentMethod, context, contextType, isReplaceWriter);
		}
		protected internal static ITemplate Create(string content, Action contentMethod, Action<T> contentMethodEx, T context, Type contextType, bool isReplaceWriter) {
			if (!string.IsNullOrEmpty(content) || contentMethod != null || contentMethodEx != null)
				return new ContentControlTemplate<T>(content, contentMethod, contentMethodEx, context, contextType, isReplaceWriter);
			return null;
		}
		void ITemplate.InstantiateIn(Control container) {
			container.Controls.Add(ContentControl<T>.Create(Content, ContentMethod, ContentMethodEx, Context, ContextType, IsReplaceWriter));
		}
	}
	public class ContentControl: ContentControl<Control> {
		protected ContentControl(string content, Action contentMethod) :
			base(content, contentMethod, null, null, null, false) {
		}
		public static Control Create(Action contentMethod) {
			return Create(string.Empty, contentMethod);
		}
		public static new Control Create(string content) {
			return Create(content, null);
		}
		public static Control Create(string content, Action contentMethod) {
			return Create(content, contentMethod, null, null, null, false);
		}
	}
	public class ContentControlTemplate: ContentControlTemplate<Control> {
		protected ContentControlTemplate(string content, Action contentMethod)
			: base(content, contentMethod, null, null, null, false) {
		}
		protected ContentControlTemplate(string content, Action contentMethod, bool isReplaceWriter)
			: base(content, contentMethod, null, null, null, isReplaceWriter) {
		}
		public static new ITemplate Create(string content) {
			return Create(content, null);
		}
		public static ITemplate Create(Action contentMethod) {
			return Create(string.Empty, contentMethod);
		}
		public static ITemplate Create(string content, Action contentMethod) {
			return Create(content, contentMethod, false);
		}
		public static ITemplate Create(string content, Action contentMethod, bool isReplaceWriter) {
			if(!string.IsNullOrEmpty(content) || contentMethod != null)
				return new ContentControlTemplate(content, contentMethod, isReplaceWriter);
			return null;
		}
	}
	public class DummyPage: Page {
		public DummyPage()
			: base() {
		}
		public bool ApplyTheme(string theme, Control control) {
			try {
				StyleSheetTheme = theme;
				FrameworkInitialize();
				control.ApplyStyleSheetSkin(this);
			} catch { return false; }
			return true;
		}
	}
	[ToolboxItem(false)]
	public class MVCxDummyIconControl: DevExpress.Web.ASPxWebControl {
		static Dictionary<ExtensionType, string> extensionIconIDs = new Dictionary<ExtensionType, string>();
		static MVCxDummyIconControl() {
			extensionIconIDs[ExtensionType.Icons16x16] = "actions_add_16x16";
			extensionIconIDs[ExtensionType.Icons16x16gray] = "actions_add_16x16gray";
			extensionIconIDs[ExtensionType.Icons16x16office2013] = "actions_add_16x16office2013";
			extensionIconIDs[ExtensionType.Icons32x32] = "actions_add_32x32";
			extensionIconIDs[ExtensionType.Icons32x32gray] = "actions_add_32x32gray";
			extensionIconIDs[ExtensionType.Icons32x32office2013] = "actions_add_32x32office2013";
			extensionIconIDs[ExtensionType.Icons16x16devav] = "actions_about_16x16devav";
			extensionIconIDs[ExtensionType.Icons32x32devav] = "actions_about_32x32devav";
		}
		public MVCxDummyIconControl(ExtensionType type) {
			if(!extensionIconIDs.ContainsKey(type))
				throw new Exception("Invalid value for ExtensionType");
			IconID = extensionIconIDs[type];
		}
		protected string IconID { get; set; }
		public override void RegisterStyleSheets() {
			IconsHelper.RegisterIconID(IconID);
			base.RegisterStyleSheets();
		}
	}
}
