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
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.DocumentViewer {
	public class DocumentMapSettings : FrameworkElement {
		public static readonly DependencyProperty WrapLongLinesProperty;
		public static readonly DependencyProperty HideAfterUseProperty;
		public static readonly DependencyProperty ActualHideAfterUseProperty;
		static readonly DependencyPropertyKey ActualHideAfterUsePropertyKey;
		static DocumentMapSettings() {
			WrapLongLinesProperty = DependencyPropertyRegistrator.Register<DocumentMapSettings, bool?>(owner => owner.WrapLongLines, null, (settings, value, newValue) => settings.RaiseInvalidate());
			HideAfterUseProperty = DependencyPropertyRegistrator.Register<DocumentMapSettings, bool?>(owner => owner.HideAfterUse, null, (settings, value, newValue) => settings.RaiseInvalidate());
			ActualHideAfterUsePropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<DocumentMapSettings, bool>(owner => owner.ActualHideAfterUse, false);
			ActualHideAfterUseProperty = ActualHideAfterUsePropertyKey.DependencyProperty;
		}
		public bool ActualHideAfterUse {
			get { return (bool)GetValue(ActualHideAfterUseProperty); }
			internal set { SetValue(ActualHideAfterUsePropertyKey, value); }
		}
		public bool? WrapLongLines {
			get { return (bool?)GetValue(WrapLongLinesProperty); }
			set { SetValue(WrapLongLinesProperty, value); }
		}
		public bool? HideAfterUse {
			get { return (bool?)GetValue(HideAfterUseProperty); }
			set { SetValue(HideAfterUseProperty, value); }
		}
		DocumentViewerControl owner;
		protected DocumentViewerControl Owner { get { return owner; } }
		public IEnumerable<object> Source { get; internal set; }
		public ICommand GoToCommand { get; internal set; }
		public event EventHandler Invalidate;
		protected internal void RaiseInvalidate() {
			Invalidate.Do(x => x(this, EventArgs.Empty));
		}
		protected internal virtual void Initialize(DocumentViewerControl owner) {
			this.owner = owner;
		}
		protected internal virtual void Release() {
			this.owner = null;
		}
		public virtual void UpdateProperties() {
			if (this.owner == null)
				return;
			UpdatePropertiesInternal();
		}
		protected virtual void UpdatePropertiesInternal() {
			if (HideAfterUse != null)
				ActualHideAfterUse = HideAfterUse.Value;
		}
	}
}
