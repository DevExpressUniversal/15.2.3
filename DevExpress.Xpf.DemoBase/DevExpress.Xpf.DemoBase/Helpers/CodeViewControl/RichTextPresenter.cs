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
using System.Linq;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer.Internal;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using System.Windows.Documents;
namespace DevExpress.Xpf.DemoBase.Helpers.Internal {
	[TemplatePart(Name = "InnerPresenter", Type = typeof(RichTextBox))]
	class RichTextPresenter : Control, IRichTextPresenter {
		public static readonly DependencyProperty TextWrappingProperty =
			DependencyPropertyManager.Register("TextWrapping", typeof(TextWrapping), typeof(RichTextPresenter), new PropertyMetadata(TextWrapping.Wrap));
		List<Block> savedBlocks;
		public RichTextPresenter() {
			InnerPresenter = new RichTextBox() { Document = new FlowDocument() };
			DefaultStyleKey = typeof(RichTextPresenter);
		}
		public RichTextBox InnerPresenter { get; private set; }
		public TextWrapping TextWrapping { get { return (TextWrapping)GetValue(TextWrappingProperty); } set { SetValue(TextWrappingProperty, value); } }
		public TextPointer ContentStart {
			get {
				return InnerPresenter.Document.ContentStart;
			}
		}
		public TextPointer ContentEnd {
			get {
				return InnerPresenter.Document.ContentEnd;
			}
		}
		public void TextWidthMaxSet(double width) {
			InnerPresenter.MaxWidth = width;
		}
		public TextPointer SelectionStart {
			get { return InnerPresenter.Selection.Start; }
		}
		public TextPointer SelectionEnd {
			get { return InnerPresenter.Selection.End; }
		}
		public void Select(TextPointer start, TextPointer end) {
			InnerPresenter.Selection.Select(start, end);
		}
		public ICollection<Block> Blocks {
			get {
				if(savedBlocks != null) return savedBlocks;
				return InnerPresenter.Document.Blocks;
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SaveBlocks();
			InnerPresenter = (RichTextBox)GetTemplateChild("InnerPresenter");
			RestoreBlocks();
		}
		void SaveBlocks() {
			if(InnerPresenter == null) return;
			savedBlocks = new List<Block>();
			foreach(Block block in InnerPresenter.Document.Blocks)
				savedBlocks.Add(block);
			InnerPresenter.Document.Blocks.Clear();
		}
		void RestoreBlocks() {
			if(savedBlocks == null || InnerPresenter == null) return;
			foreach(Block block in savedBlocks)
				InnerPresenter.Document.Blocks.Add(block);
			savedBlocks = null;
		}
	}
}
