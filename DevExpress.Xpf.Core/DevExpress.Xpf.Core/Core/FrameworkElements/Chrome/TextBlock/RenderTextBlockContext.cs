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
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Core.Native {
	public enum RenderTextBlockMode {
		GlyphRun,
		FormattedText,
		TextBlock,
	}
	public class RenderTextBlockContext : FrameworkRenderElementContext {
		protected override int RenderChildrenCount {
			get { return Child != null ? 1 : 0; }
		}
		protected override FrameworkRenderElementContext GetRenderChild(int index) {
			return Child;
		}
		string text;
		string highlightedText;
		HighlightedTextCriteria criteria;
		TextTrimming? textTrimming;
		TextWrapping? textWrapping;
		TextAlignment? textAlignment;
		TextDecorationCollection textDecorations;
		bool forceUseRealTextBlock;
		public bool IsRealTextBlockInitialized { get { return Child != null; } }		
		public string Text {
			get { return text; }
			set {
				SetProperty(ref text, value, FREInvalidateOptions.UpdateLayout, () => Child.Do(x => x.Text = value));
			}
		}
		public bool ForceUseRealTextBlock {
			get { return forceUseRealTextBlock; }
			set { SetProperty(ref forceUseRealTextBlock, value);}
		}
		public HighlightedTextCriteria HighlightedTextCriteria {
			get { return criteria; }
			set { SetProperty(ref criteria, value, FREInvalidateOptions.UpdateLayout, () => Child.Do(x => x.HighlightedTextCriteria = value)); }
		}
		public string HighlightedText {
			get { return highlightedText; }
			set {
				SetProperty(ref highlightedText, value, FREInvalidateOptions.None);
				if (ShouldAdditionalUpdateOnHighlightedTextChanged(value, textDecorations != null))
					UpdateLayout(FREInvalidateOptions.UpdateSubTree);
				Child.Do(x => x.HighlightedText = value);
			}
		}
		bool ShouldAdditionalUpdateOnHighlightedTextChanged(string ht, bool hasDecorations) {
			if (hasDecorations)
				return true;
			if (string.IsNullOrEmpty(ht) && IsRealTextBlockInitialized)
				return true;
			if (!string.IsNullOrEmpty(ht) && !IsRealTextBlockInitialized)
				return true;
			return false;
		}
		public TextTrimming? TextTrimming {
			get { return textTrimming; }
			set { SetProperty(ref textTrimming, value, FREInvalidateOptions.UpdateSubTree); }
		}
		public TextWrapping? TextWrapping {
			get { return textWrapping; }
			set { SetProperty(ref textWrapping, value, FREInvalidateOptions.UpdateSubTree); }
		}
		public TextAlignment? TextAlignment {
			get { return textAlignment; }
			set { SetProperty(ref textAlignment, value); }
		}
		public TextDecorationCollection TextDecorations { 
			get { return textDecorations; }
			set { SetProperty(ref textDecorations, value, FREInvalidateOptions.AffectsRenderCaches, () => Child.Do(x => x.TextDecorations = value)); }
		}
		public RenderTextBlockMode RenderMode { get; internal set; }
		public TextDecorationCollection CachedTextDecorations { get; internal set; }
		public IFormattedTextContainer GlyphRunContainer { get; internal set; }
		public IFormattedTextContainer FormattedTextContainer { get; internal set; }
		public Typeface Typeface { get; internal set; }
		public RenderRealTextBlockContext Child { get; private set; }
		public bool UseRealTextBlock { get { return Child != null; } }
		public GlyphTypeface GlyphTypeface { get; internal set; }
		public GlyphRun GlyphRun { get; internal set; }
		public RenderTextBlockContext(RenderTextBlock factory)
			: base(factory) {
		}
		protected override void ResetTemplatesAndVisualsInternal() {
			base.ResetTemplatesAndVisualsInternal();
			if (Child != null) {
				RemoveChild(Child);
				Child = null;
			}
		}
		protected override void OnForegroundChanged(object oldValue, object newValue) {
			base.OnForegroundChanged(oldValue, newValue);
			UpdateLayout();
		}
		protected override void OnFlowDirectionChanged(FlowDirection oldValue, FlowDirection newValue) {
			UpdateLayout();
		}
		public override bool ShouldUseMirrorTransform() {
			var bValue = base.ShouldUseMirrorTransform();
			if (FlowDirection == System.Windows.FlowDirection.RightToLeft)
				return !bValue;
			return bValue;
		}
		protected override void ResetRenderCachesInternal() {
			base.ResetRenderCachesInternal();
			Typeface = null;
			CachedTextDecorations = null;
			GlyphTypeface = null;
			FormattedTextContainer = null;
			GlyphRunContainer = null;
		}
		public override void AddChild(FrameworkRenderElementContext child) {
			base.AddChild(child);
			Child = (RenderRealTextBlockContext)child;
		}
		public override void RemoveChild(FrameworkRenderElementContext child) {
			base.RemoveChild(child);
			Child = null;
		}
	}
}
