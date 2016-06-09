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
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Core.Native {
	public class RenderRealTextBlockContext : RenderControlBaseContext {
		TextBlock TextBlock { get { return (TextBlock)Control; } }
		string text;
		string highlightedText;
		HighlightedTextCriteria criteria;
		bool shouldUpdateTextBlock;
		public string Text {
			get { return text; }
			set { SetProperty(ref text, value, FREInvalidateOptions.UpdateLayout, UpdateTextBlock); }
		}
		public string HighlightedText {
			get { return highlightedText; }
			set { SetProperty(ref highlightedText, value, FREInvalidateOptions.UpdateLayout, UpdateTextBlock); }
		}
		public HighlightedTextCriteria HighlightedTextCriteria {
			get { return criteria; }
			set { SetProperty(ref criteria, value, FREInvalidateOptions.UpdateLayout, UpdateTextBlock); }
		}
		public TextWrapping TextWrapping {
			get { return TextBlock.TextWrapping; }
			set { TextBlock.TextWrapping = value; }
		}
		public TextTrimming TextTrimming {
			get { return TextBlock.TextTrimming; }
			set { TextBlock.TextTrimming = value; }
		}
		public TextAlignment TextAlignment {
			get { return TextBlock.TextAlignment; }
			set { TextBlock.TextAlignment = value; }
		}
		public TextDecorationCollection TextDecorations {
			get { return TextBlock.TextDecorations; }
			set { TextBlock.TextDecorations = value; }
		}
		public RenderRealTextBlockContext(RenderRealTextBlock factory)
			: base(factory) {
		}
		protected virtual void UpdateTextBlock() {
			if (IsInSupportInitialize) {
				this.shouldUpdateTextBlock = true;
				return;
			}
			if (shouldUpdateTextBlock)
				TextBlockService.UpdateTextBlock(TextBlock, text, highlightedText, criteria);
			shouldUpdateTextBlock = false;
		}
		protected override void EndInitInternal() {
			base.EndInitInternal();
			UpdateTextBlock();
		}
	}
}
