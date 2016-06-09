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
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors {
	public class UITypeEditorValue : BindableBase {
		object originalValue;
		object value;
		object content;
		bool isModified;
		bool? forcePost;
		protected internal DependencyObject Owner { get; private set; }
		protected internal IEditorSource Source { get; private set; }
		public UITypeEditorValue(DependencyObject owner, IEditorSource source, object editValue, object content) {
			Owner = owner;
			Source = source;
			originalValue = editValue;
			value = editValue;
			this.content = content;
		}
		public object OriginalValue {
			get { return originalValue; }
			private set { SetProperty(ref originalValue, value, () => OriginalValue); }
		}
		public object Value {
			get { return value; }
			set { SetProperty(ref this.value, value, () => Value, () => isModified = true); }
		}
		public object Content {
			get { return content; }
			set { SetProperty(ref content, value, () => Content); }
		}
		public bool IsModified { get { return isModified; } }
		public bool? ForcePost {
			get { return forcePost; }
			set { SetProperty(ref forcePost, value, () => ForcePost); }
		}
		protected internal virtual bool ShouldPost() {
			if (!ForcePost.HasValue)
				return isModified;
			return ForcePost.Value;
		}
		public override string ToString() {
			if (Content != null)
				return Content.With(x => x.ToString());
			return IsModified ? Value.With(x => x.ToString()) : OriginalValue.With(x => x.ToString());
		}
	}
}
