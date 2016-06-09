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
using System.Text;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web.Internal {
	public class SplitterRenderHelper {
		#region Style-cache keys
		static readonly object
			StyleKeyPane = new object(),
			StyleKeyPaneCollapsed = new object(),
			StyleKeyRootPane = new object(),
			StyleKeyRootPaneCollapsed = new object(),
			StyleKeySeparator = new object(),
			StyleKeySeparatorCollapsed = new object(),
			StyleKeyVSeparator = new object(),
			StyleKeyHSeparator = new object(),
			StyleKeyVSeparatorCollapsed = new object(),
			StyleKeyHSeparatorCollapsed = new object(),
			StyleKeySeparatorButton = new object(),
			StyleKeyVSeparatorButton = new object(),
			StyleKeyHSeparatorButton = new object();
		#endregion
		protected static string TableIDPostfix = "T";
		protected static string ContentContainerIDPostfix = "CC";
		protected static string SeparatorIDPostfix = "S";
		protected static string[] ButtonsIDPostfixes = new string[] { "CS", "CB", "CF"};
		protected static string ImagePostfix = "Img";
		protected static string ResizingPointerPostfix = "RP";
		protected static Unit DefaultPaneSize = 200;
		protected static Unit DefaultSeparatorSize = 6;
		ASPxSplitter splitter;
		public SplitterRenderHelper(ASPxSplitter splitter) {
			this.splitter = splitter;
		}
		ASPxSplitter Splitter { get { return splitter; } }
		bool DesignMode { get { return Splitter.DesignMode; } }
		Page Page { get { return Splitter.Page; } }
		SplitterStyles Styles { get { return Splitter.Styles; } }
		SplitterImages Images { get { return Splitter.Images; } }
		bool IsRightToLeft { get { return (Splitter as ISkinOwner).IsRightToLeft(); } }
		protected internal static bool MergeDefaultBoolean(DefaultBoolean value, bool parentValue) {
			if(value == DefaultBoolean.False)
				return false;
			if((value == DefaultBoolean.Default) && !parentValue)
				return false;
			return true;
		}
		public bool IsButtonVisible(SplitterButtons buttonType, SplitterPane pane) {
			if(buttonType == SplitterButtons.Separator)
				return MergeDefaultBoolean(pane.ShowSeparatorImage, Splitter.ShowSeparatorImage);
			else {
				if(pane.VisibleIndex == 0)
					return false;
				if(buttonType == SplitterButtons.Backward)
					return MergeDefaultBoolean(pane.Parent.Panes.GetVisibleItem(pane.VisibleIndex - 1).ShowCollapseBackwardButton, Splitter.ShowCollapseBackwardButton);
				else
					return MergeDefaultBoolean(pane.ShowCollapseForwardButton, Splitter.ShowCollapseForwardButton);
			}
		}
		public bool IsBackwardForwardButtonsVisible(SplitterPane pane) {
			return IsButtonVisible(SplitterButtons.Backward, pane) || IsButtonVisible(SplitterButtons.Forward, pane);
		}
		public bool IsButtonsVisible(SplitterPane pane) {
			return IsButtonVisible(SplitterButtons.Separator, pane) || IsBackwardForwardButtonsVisible(pane);
		}
		string GetImagePostfix() {
			return ImagePostfix;
		}
		string GetButtonPostfix(SplitterButtons buttonType) {
			return ButtonsIDPostfixes[(int)buttonType];
		}
		string GetButtonImagePostfix(SplitterButtons buttonType) {
			return GetPostfixedId(GetButtonPostfix(buttonType), GetImagePostfix());
		}
		protected internal string GetImageFullPostfix() {
			return GetPostfixedId("", GetImagePostfix());
		}
		protected internal string GetButtonFullPostfix(SplitterButtons buttonType) {
			return GetPostfixedId("", GetButtonPostfix(buttonType));
		}
		protected internal string GetButtonImageFullPostfix(SplitterButtons buttonType) {
			return GetPostfixedId("", GetButtonImagePostfix(buttonType));
		}
		string GetPanePathCore(SplitterPane pane, Func<SplitterPane, int> getIndex) {
			if((pane == null) || (pane.Parent == null))
				return "";
			StringBuilder path = new StringBuilder();
			path.Insert(0, getIndex(pane));
			SplitterPane current = pane.Parent;
			while(current.Parent != null) {
				path.Insert(0, RenderUtils.IndexSeparator);
				path.Insert(0, getIndex(current));
				current = current.Parent;
			}
			return path.ToString();
		}
		protected internal string GetPanePath(SplitterPane pane) {
			return GetPanePathCore(pane, p => p.Index);
		}
		protected internal string GetPaneVisiblePath(SplitterPane pane) {
			return GetPanePathCore(pane, p => p.VisibleIndex);
		}
		string GetPostfixedId(string id, string postfix) {
			return String.Concat(id, "_", postfix);
		}
		protected internal string GetPaneID(SplitterPane pane) {
			return GetPaneVisiblePath(pane);
		}
		protected internal string GetPanesTableID(SplitterPane pane) {
			return GetPostfixedId(GetPaneID(pane), TableIDPostfix);
		}
		protected internal string GetContentContainerID(SplitterPane pane) {
			return GetPostfixedId(GetPaneID(pane), ContentContainerIDPostfix);
		}
		protected internal string GetSeparatorID(SplitterPane pane) {
			return GetPostfixedId(GetPaneID(pane), SeparatorIDPostfix);
		}
		protected internal string GetButtonID(SplitterButtons buttonType, SplitterPane pane) {
			return GetPostfixedId(GetSeparatorID(pane), GetButtonPostfix(buttonType));
		}
		protected internal string GetButtonImageID(SplitterButtons buttonType, SplitterPane pane) {
			return GetPostfixedId(GetSeparatorID(pane), GetButtonImagePostfix(buttonType));
		}
		protected internal string GetResizingPointerID() {
			return ResizingPointerPostfix;
		}
		protected internal string GetSeparatorSpacerClassName() {
			return "dxsplS";
		}
		protected internal string GetContentContentContainerClassName(SplitterPane pane) {
			if(pane.HasVisibleChildren)
				return "dxsplCC";
			else if(!pane.HasContentUrl)
				return "dxsplLCC";
			return "";
		}
		protected internal string GetPanesTableClassName() {
			return "dxsplP";
		}
		protected internal static void CheckSizeType(Unit size, bool allowEm, bool allowEx, bool allowPrc, string propertyName) {
			if(size.IsEmpty)
				return;
			if( (!allowEm && (size.Type == UnitType.Em)) ||
				(!allowEx && (size.Type == UnitType.Ex)) ||
				(!allowPrc && (size.Type == UnitType.Percentage)))
					throw new ArgumentException(String.Format("The ASPxSplitter control's {0} property cannot be set to UnitType.{1}.", propertyName, size.Type.ToString()));
		}
		protected internal Unit GetSeparatorSize(SplitterPane pane) {
			Unit size = pane.Separator.Size;
			SplitterPane current = pane.Parent;
			while(size.IsEmpty && (current != null)) {
				size = current.Separators.Size;
				current = current.Parent;
			}
			if(size.IsEmpty)
				size = Splitter.SeparatorSize;
			if(size.IsEmpty)
				return DefaultSeparatorSize;
			else {
				size = UnitUtils.ConvertToPixels(size);
				if(size.Value < 1)
					return Unit.Pixel(1);
				return size;
			}
		}
		Unit GetConvertedSize(SplitterPane pane) {
			Unit convertedSize = UnitUtils.ConvertToPixels(pane.Size);
			if(DesignMode && convertedSize.IsEmpty)
				return DefaultPaneSize;
			return convertedSize;
		}
		static Unit GetPaneDesignTimeSize(Unit size, double pxRate, double prcRate) {
			return Unit.Percentage(Math.Ceiling(size.Value * ((size.Type == UnitType.Pixel) ? pxRate : prcRate)));
		}
		protected internal void PrepareChildrenDesignModeSize(SplitterPane pane) {
			if(pane.Panes.GetVisibleItemCount() == 0)
				return;
			double totalPx = 0, totalPrc = 0;
			foreach(SplitterPane child in pane.Panes.GetVisibleItems()) {
				Unit size = GetConvertedSize(child);
				if(size.Type == UnitType.Pixel)
					totalPx += size.Value;
				else if(size.Type == UnitType.Percentage)
					totalPrc += size.Value;
			}
			foreach(SplitterPane child in pane.Panes.GetVisibleItems()) {
				Unit childSize = GetConvertedSize(child);
				double maxPrc = 100, prcRate = 0, pxRate = 0;
				if(totalPx > 0) {
					maxPrc = 90;
					pxRate = (100.0 - Math.Min(totalPrc, 90)) / totalPx;
				}
				if(totalPrc > 0)
					prcRate = (totalPrc <= maxPrc) ? 1.0 : maxPrc / totalPrc;
				child.DesignModeSize = GetPaneDesignTimeSize(childSize, pxRate, prcRate);
				PrepareChildrenDesignModeSize(child);
			}
		}
		protected internal Hashtable GetStateObject(SplitterPane pane, bool isClientScriptObject) {
			Hashtable result = new Hashtable();
			if(isClientScriptObject)
				result["v"] = pane.Visible ? 1 : 0;
			Unit size = GetConvertedSize(pane);
			if(!size.IsEmpty) {
				result["st"] = (size.Type == UnitType.Pixel) ? "px" : "%";
				result["s"] = size.Value;
			}
			if(pane.Collapsed)
				result["c"] = 1;
			if(pane.Panes.Count == 0) {
				if(pane.ScrollTop > 0)
					result["spt"] = pane.ScrollTop;
				if(pane.ScrollLeft > 0)
					result["spl"] = pane.ScrollLeft;
			}
			if(isClientScriptObject && pane.Visible) {
				if(!string.IsNullOrEmpty(pane.Name))
					result["n"] = pane.Name;
				if(!pane.MinSize.IsEmpty && (pane.MinSize != Splitter.PaneMinSize))
					result["smin"] = UnitUtils.ConvertToPixels(pane.MinSize).Value;
				if(!pane.MaxSize.IsEmpty)
					result["smax"] = UnitUtils.ConvertToPixels(pane.MaxSize).Value;
				if(pane.AutoWidth)
					result["aw"] = 1;
				if(pane.AutoHeight)
					result["ah"] = 1;
				if(pane.AllowResize == DefaultBoolean.False)
					result["nar"] = 0;
				if(IsButtonVisible(SplitterButtons.Backward, pane))
					result["scbb"] = 1;
				if(IsButtonVisible(SplitterButtons.Forward, pane))
					result["scfb"] = 1;
				if(pane.HasContentUrl) {
					ArrayList iframeParams = new ArrayList();
					iframeParams.Add(ResolveContentUrl(pane.ContentUrl));
					iframeParams.Add(GetIFrameScrollingAttributeValue(pane));
					iframeParams.Add(pane.ContentUrlIFrameName);
					iframeParams.Add(pane.ContentUrlIFrameTitle);
					result["iframe"] = iframeParams;
				}
			}
			if(pane.Panes.Count > 0)
				result["i"] = GetStateObject(pane.Panes, isClientScriptObject);
			return result;
		}
		protected internal IEnumerable GetStateObject(SplitterPaneCollection collection, bool isClientScriptObject) {
			for(int i = 0; i < collection.Count; i++)
				yield return GetStateObject(collection[i], isClientScriptObject);
		}
		static T GetMergedStyle<T>(params AppearanceStyleBase[] styles) where T : AppearanceStyleBase, new() {
			T style = new T();
			for(int i = 0; i < styles.Length; i++)
				style.MergeWith(styles[i]);
			return style;
		}		
		SplitterPaneStyle GetPaneStyleInternal(SplitterPane pane) {
			return Splitter.InternalCreateStyle<SplitterPaneStyle>(delegate() {
				if(pane == null)
					return GetMergedStyle<SplitterPaneStyle>(Styles.Pane, Styles.GetDefaultPaneStyle());
				else
					return GetMergedStyle<SplitterPaneStyle>(pane.PaneStyle, GetPaneStyleInternal(pane.Parent));
			}, (pane == null) ? StyleKeyRootPane : pane, StyleKeyPane);
		}
		protected internal SplitterPaneStyle GetPaneStyle(SplitterPane pane) {
			if(pane.HasVisibleChildren)
				return new SplitterPaneStyle();
			if(!pane.PaneStyle.Paddings.IsEmpty && pane.PaneStyle.Paddings.Padding.IsEmpty) {   
				pane.PaneStyle.Paddings.Padding = GetPaneStyleInternal(pane.Parent).Paddings.Padding;
			}
			SplitterPaneStyle paneStyle = GetPaneStyleInternal(pane);
			if(pane.GetVisibleIndex() > 0 && !IsVisibleSeparator(pane)) {
				(pane.Separator.IsVertical
					? paneStyle.BorderLeft
					: paneStyle.BorderTop
				 ).BorderWidth = Unit.Pixel(0); 
			}
			return paneStyle;
		}
		protected internal SplitterPaneCollapsedStyle GetPaneCollapsedStyle(SplitterPane pane) {
			return Splitter.InternalCreateStyle<SplitterPaneCollapsedStyle>(delegate() {
				if(pane == null)
					return GetMergedStyle<SplitterPaneCollapsedStyle>(Styles.PaneCollapsed, Styles.GetDefaultPaneCollapsedStyle());
				else
					return GetMergedStyle<SplitterPaneCollapsedStyle>(pane.CollapsedStyle, GetPaneCollapsedStyle(pane.Parent));
			}, (pane == null) ? StyleKeyRootPaneCollapsed : pane, StyleKeyPaneCollapsed);
		}
		SplitterSeparatorStyle GetRootSeparatorStyle(bool isVertical) {
			return Splitter.InternalCreateStyle<SplitterSeparatorStyle>(delegate() {
				return GetMergedStyle<SplitterSeparatorStyle>(isVertical ? Styles.VerticalSeparator : Styles.HorizontalSeparator, Styles.GetDefaultSeparatorStyle(isVertical), Styles.Separator);
			}, isVertical ? StyleKeyVSeparator : StyleKeyHSeparator);
		}
		SplitterSeparatorStyle GetSeparatorStyleInternal(SplitterPane pane) {
			if(pane == null)
				return null;
			return Splitter.InternalCreateStyle<SplitterSeparatorStyle>(delegate() {
				return GetMergedStyle<SplitterSeparatorStyle>(pane.Separators.SeparatorStyle, GetSeparatorStyleInternal(pane.Parent));
			}, pane, StyleKeySeparator);
		}
		protected internal SplitterSeparatorStyle GetSeparatorStyle(SplitterPane pane) {						
			return GetMergedStyle<SplitterSeparatorStyle>(pane.Separator.SeparatorStyle, GetSeparatorStyleInternal(pane.Parent), GetRootSeparatorStyle(pane.Separator.IsVertical));
		}
		protected internal AppearanceSelectedStyle GetSeparatorHoverStyle(SplitterPane pane) {
			return GetMergedStyle<AppearanceSelectedStyle>(GetSeparatorStyle(pane).HoverStyle, Styles.GetDefaultSeparatorHoverStyle(pane.Separator.IsVertical));
		}
		SplitterSeparatorStyle GetRootSeparatorCollapsedStyle(bool isVertical) {
			return Splitter.InternalCreateStyle<SplitterSeparatorStyle>(delegate() {
				return GetMergedStyle<SplitterSeparatorStyle>(isVertical ? Styles.VerticalSeparatorCollapsed : Styles.HorizontalSeparatorCollapsed, Styles.GetDefaultSeparatorCollapsedStyle(isVertical), Styles.SeparatorCollapsed);
			}, isVertical ? StyleKeyVSeparatorCollapsed : StyleKeyHSeparatorCollapsed);
		}
		SplitterSeparatorStyle GetSeparatorCollapsedStyleInternal(SplitterPane pane) {
			if(pane == null)
				return null;
			return Splitter.InternalCreateStyle<SplitterSeparatorStyle>(delegate() {
				return GetMergedStyle<SplitterSeparatorStyle>(pane.Separators.CollapsedStyle, GetSeparatorCollapsedStyleInternal(pane.Parent));
			}, pane, StyleKeySeparatorCollapsed);
		}
		protected internal SplitterSeparatorStyle GetSeparatorCollapsedStyle(SplitterPane pane) {
			return GetMergedStyle<SplitterSeparatorStyle>(pane.Separator.CollapsedStyle, GetSeparatorCollapsedStyleInternal(pane.Parent), GetRootSeparatorCollapsedStyle(pane.Separator.IsVertical));
		}
		SplitterSeparatorButtonStyle GetRootSeparatorButtonStyle(bool isVertical) {
			return Splitter.InternalCreateStyle<SplitterSeparatorButtonStyle>(delegate() {
				return GetMergedStyle<SplitterSeparatorButtonStyle>(isVertical ? Styles.VerticalSeparatorButton : Styles.HorizontalSeparatorButton, Styles.GetDefaultSeparatorButtonStyle(isVertical), Styles.SeparatorButton);
			}, isVertical ? StyleKeyVSeparatorButton : StyleKeyHSeparatorButton);
		}
		SplitterSeparatorButtonStyle GetSeparatorButtonStyleInternal(SplitterPane pane) {
			if(pane == null)
				return null;
			return Splitter.InternalCreateStyle<SplitterSeparatorButtonStyle>(delegate() {
				return GetMergedStyle<SplitterSeparatorButtonStyle>(pane.Separators.ButtonStyle, GetSeparatorButtonStyleInternal(pane.Parent));
			}, pane, StyleKeySeparatorButton);
		}
		protected internal SplitterSeparatorButtonStyle GetSeparatorButtonStyle(SplitterPane pane) {			
			return GetMergedStyle<SplitterSeparatorButtonStyle>(pane.Separator.ButtonStyle, GetSeparatorButtonStyleInternal(pane.Parent), GetRootSeparatorButtonStyle(pane.Separator.IsVertical));
		}
		protected internal AppearanceSelectedStyle GetSeparatorButtonHoverStyle(SplitterPane pane) {			
			return GetMergedStyle<AppearanceSelectedStyle>(GetSeparatorButtonStyle(pane).HoverStyle, Styles.GetDefaultSeparatorButtonHoverStyle(pane.Separator.IsVertical));
		}
		protected internal AppearanceStyleBase GetResizingPointerStyle() {
			return GetMergedStyle<AppearanceStyleBase>(Styles.ResizingPointer, Styles.GetDefaultResizingPointerStyle());
		}
		protected internal static ScrollBars GetPaneScrollBars(SplitterPane pane) {
			SplitterPane current = pane;
			ScrollBars scrollBars = ScrollBars.None;
			while((scrollBars == ScrollBars.None) && (current != null)) {
				scrollBars = current.ScrollBars;
				current = current.Parent;
			}
			return scrollBars;
		}
		protected internal void ApplyDivScrollBarsAttribute(SplitterPane pane, WebControl control) {
			ScrollBars scrollBars = GetPaneScrollBars(pane);
			RenderUtils.SetScrollBars(control, scrollBars);
		}
		protected internal string GetIFrameScrollingAttributeValue(SplitterPane pane) {
			ScrollBars scrollBars = GetPaneScrollBars(pane);
			if(scrollBars == ScrollBars.Auto)
				return "auto";
			else if(scrollBars == ScrollBars.None)
				return "no";
			else
				return "yes";
		}
		HottrackedImageProperties GetSeparatorButtonImage(SplitterButtons buttonType,  SplitterSeparators separator) {
			if (buttonType == SplitterButtons.Separator)
				return separator.Image;
			else if (buttonType == SplitterButtons.Backward)
				return separator.BackwardCollapseButtonImage;
			else
				return separator.ForwardCollapseButtonImage;
		}
		protected internal HottrackedImageProperties GetButtonImage(SplitterButtons buttonType, SplitterPane pane) {
			if(IsRightToLeft)
				buttonType = CorrectImageButtonTypeForRtl(buttonType, pane.Parent);
			HottrackedImageProperties image = new HottrackedImageProperties();			
			image.MergeWith(GetSeparatorButtonImage(buttonType, pane.Separator));
			SplitterPane current = pane.Parent;
			while(current != null) {
				image.MergeWith(GetSeparatorButtonImage(buttonType, current.Separators));
				current = current.Parent;
			}
			image.MergeWith(Images.GetImageProperties(Page, SplitterImages.GetImageName(buttonType, pane.Separator.Orientation)));
			return image;
		}
		SplitterButtons CorrectImageButtonTypeForRtl(SplitterButtons buttonType, SplitterPane pane) {
			if(pane.Orientation == Orientation.Horizontal)
				return buttonType;
			if(buttonType == SplitterButtons.Forward)
				return SplitterButtons.Backward;
			if(buttonType == SplitterButtons.Backward)
				return SplitterButtons.Forward;
			return buttonType;
		}
		protected internal bool IsEnabled(SplitterPane pane) {
			SplitterPane current = pane;
			while(current != null) {
				if(!current.Enabled)
					return false;
				current = current.Parent;
			}
			if(!pane.Splitter.Enabled)
				return false;
			return true;
		}		
		protected internal bool IsVisibleSeparator(SplitterPane pane) {
			if (pane.GetVisibleIndex() == 0) return false;
			DefaultBoolean separatorVisible = pane.Separator.Visible;
			SplitterPane current = pane.Parent;
			while((current != null) && (separatorVisible == DefaultBoolean.Default)) {
				separatorVisible = current.Separators.Visible;
				current = current.Parent;				
			}
			return MergeDefaultBoolean(separatorVisible, Splitter.SeparatorVisible);
		}
		protected internal string ResolveContentUrl(string contentUrl) {
			return Splitter.ResolveUrl(contentUrl.Replace("'", "%27")); 
		}
	}
}
