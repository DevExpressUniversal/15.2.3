﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Editors;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
#if SILVERLIGHT
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpf.Printing.Native.Lines {
	class EditorPropertyLineBase : PropertyLine {
		Label label;
		protected BaseEdit editor;
		public override Label Header {
			get { return label; }
		}
		public override Control Content {
			get { return editor; }
		}
		protected EditorPropertyLineBase(BaseEdit editor, PropertyDescriptor property, object obj)
			: base(property, obj) {
			label = new Label() { Padding = new Thickness(0), VerticalAlignment = VerticalAlignment.Center };
			this.editor = editor;
		}
		public override void SetText(string text) {
#if !SILVERLIGHT
			label.Content = new TextBlock() { Text = text, TextTrimming = TextTrimming.CharacterEllipsis };
#else 
			label.Content = text;
#endif
		}
		public override void RefreshContent() {
			base.RefreshContent();
			editor.ClearError();
		}
	}
}
