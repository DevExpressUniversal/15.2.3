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
namespace DevExpress.ExpressApp {
	public struct TemplateContext {
		private const string FindPopupWindowContextName = "FindPopupWindowContext";
		private const string PopupWindowContextName = "PopupWindowContext";
		private const string LookupControlContextName = "LookupControlContext";
		private const string LookupWindowContextName = "LookupWindowContext";
		private const string ApplicationWindowContextName = "ApplicationWindowContext";
		private const string NestedFrameContextName = "NestedFrameContext";
		private const string ViewContextName = "ViewContext";
		private const string LogonWindowContextName = "LogonWindowContext";
		private static readonly TemplateContext findPopupWindow = new TemplateContext(FindPopupWindowContextName);
		private static readonly TemplateContext popupWindow = new TemplateContext(PopupWindowContextName);
		private static readonly TemplateContext lookupControl = new TemplateContext(LookupControlContextName);
		private static readonly TemplateContext lookupWindow = new TemplateContext(LookupWindowContextName);
		private static readonly TemplateContext applicationWindow = new TemplateContext(ApplicationWindowContextName);
		private static readonly TemplateContext nestedFrame = new TemplateContext(NestedFrameContextName);
		private static readonly TemplateContext view = new TemplateContext(ViewContextName);
		private static readonly TemplateContext logonWindow = new TemplateContext(LogonWindowContextName);
		private string name;
		private TemplateContext(string name) {
			this.name = !string.IsNullOrEmpty(name) ? name : Undefined.name;
		}
		public static TemplateContext FindPopupWindow {
			get { return findPopupWindow; }
		}
		public static TemplateContext PopupWindow {
			get { return popupWindow; }
		}
		public static TemplateContext LookupControl {
			get { return lookupControl; }
		}
		public static TemplateContext LookupWindow {
			get { return lookupWindow; }
		}
		public static TemplateContext ApplicationWindow {
			get { return applicationWindow; }
		}
		public static TemplateContext NestedFrame {
			get { return nestedFrame; }
		}
		public static TemplateContext View {
			get { return view; }
		}
		public static TemplateContext LogonWindow {
			get { return logonWindow; }
		}
		public static readonly TemplateContext Undefined = new TemplateContext("");
		public override string ToString() {
			return name;
		}
		public override int GetHashCode() {
			return name.GetHashCode();
		}
		public override bool Equals(object obj) {
			if(obj is TemplateContext) {
				TemplateContext other = (TemplateContext)obj;
				return this.name == other.name;
			}
			string str = obj as string;
			if(str != null) {
				return this.name == str;
			}
			return false;
		}
		public static bool operator ==(TemplateContext x, TemplateContext y) {
			return Equals(x, y);
		}
		public static bool operator !=(TemplateContext x, TemplateContext y) {
			return !Equals(x, y);
		}
		public static implicit operator TemplateContext(string s) {
			return new TemplateContext(s);
		}
		public static implicit operator string(TemplateContext s) {
			return s.name;
		}
		public string Name { get { return name; } }
	}
}
