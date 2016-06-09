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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
#if !SL
using PopupFooterButtonsType = DevExpress.Xpf.Editors.PopupFooterButtons;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.WPFToSLUtils;
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PopupFooterButtonsType = DevExpress.Xpf.Editors.PopupFooterButtons;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	public abstract class PopupResizingStrategyBase {
		protected abstract double HorizontalIndent { get; }
		protected abstract double VerticalIndent { get; }
		protected FrameworkElement PopupChild { get { return Editor.Popup.Return(popup => popup.Child as FrameworkElement, null); } }
		public PopupResizingStrategyBase(PopupBaseEdit editor) {
			Editor = editor;
		}
		internal protected bool DropOpposite { get; set; }
		protected PopupBaseEdit Editor { get; private set; }
		protected virtual Rect EditorRect { get { return Rect.Empty; } }
		double GetPopupHeightInternal(double offset) {
			if (DropOpposite)
				return Math.Max(Editor.PopupHeight - offset, 0d);
			return Math.Max(Editor.PopupHeight + offset, 0d);
		}
		double GetPopupWidthInternal(double offset) {
			return Math.Max(Editor.PopupWidth + offset, 0d);
		}
		protected internal virtual Rect RootRect { get { return Rect.Empty; } }
		protected virtual Rect PopupRect { get { return Rect.Empty; } }
		protected FrameworkElement Root {
			get {
#if SL
				return (FrameworkElement)Application.Current.RootVisual;
#else
				return VisualTreeHelper.GetChild(LayoutHelper.GetRoot(Editor), 0) as FrameworkElement;
#endif
			}
		}
		public double GetPopupWidth(double offset) {
			if (ActualAvailableSize.Width < PopupRect.Width + offset)
				return ActualAvailableSize.Width;
			return GetPopupWidthInternal(offset);
		}
		public double GetPopupHeight(double offset) {
			if (!DropOpposite) {
				if (RootRect.Bottom > PopupRect.Bottom + offset)
					return GetPopupHeightInternal(offset);
				return RootRect.Bottom - (PopupRect.Top + VerticalIndent);
			}
			if (RootRect.Top < PopupRect.Top + offset)
				return GetPopupHeightInternal(offset);
			return PopupRect.Bottom - (RootRect.Top + VerticalIndent);
		}
		protected virtual bool IsRootRTL { get { return false; } }
		internal bool IsRTL { get { return Editor.FlowDirection == FlowDirection.RightToLeft; } }
		bool SameFlowDirection { get { return IsRTL && IsRootRTL || !IsRTL && !IsRootRTL; } }
		Size AvailableUpSize {
			get {
				if (!SameFlowDirection)
					return new Size(Math.Max(EditorRect.Right - RootRect.Left - HorizontalIndent, 0d), Math.Max(EditorRect.Top - RootRect.Top - VerticalIndent, 0d));
				return new Size(Math.Max(RootRect.Right - EditorRect.Left - HorizontalIndent, 0d), Math.Max(EditorRect.Top - RootRect.Top - VerticalIndent, 0d));
			}
		}
		Size AvailableDownSize {
			get {
				if (!SameFlowDirection)
					return new Size(Math.Max(EditorRect.Right - RootRect.Left - HorizontalIndent, 0d), Math.Max(RootRect.Bottom - EditorRect.Bottom - VerticalIndent, 0d));
				return new Size(Math.Max(RootRect.Right - EditorRect.Left - HorizontalIndent, 0d), Math.Max(RootRect.Bottom - EditorRect.Bottom - VerticalIndent, 0d));
			}
		}
		public Size ActualAvailableSize {
			get {
				if (DropOpposite)
					return AvailableUpSize;
				return AvailableDownSize;
			}
		}
		public void UpdateDropOpposite() {
			DropOpposite = GetDropOpposite();
		}
		bool GetDropOpposite() {
			if (AvailableDownSize.Height - PopupChild.ActualHeight > 0d)
				return false;
			return AvailableDownSize.Height < AvailableUpSize.Height;
		}
		public PlacementMode GetPlacement() {
			if (DropOpposite)
				return PlacementMode.Top;
			return PlacementMode.Bottom;
		}
	}
	public class BrowserPopupResizingStrategy : PopupResizingStrategyBase {
		protected override double HorizontalIndent {
			get { return 1d; }
		}
		protected override double VerticalIndent {
			get { return 1d; }
		}
		protected internal override Rect RootRect { get { return new Rect(new Point(), new Size(Root.ActualWidth, Root.ActualHeight)); } }
		protected override Rect EditorRect { get { return LayoutHelper.GetRelativeElementRect(Editor, Root); } }
		protected override Rect PopupRect {
			get {
				if (!DropOpposite)
					return new Rect(EditorRect.BottomLeft(), new Size(PopupChild.ActualWidth, PopupChild.ActualHeight));
				return new Rect(new Point(EditorRect.Left, EditorRect.Top - PopupChild.ActualHeight), new Size(PopupChild.ActualWidth, PopupChild.ActualHeight));
			}
		}
		public BrowserPopupResizingStrategy(PopupBaseEdit editor)
			: base(editor) {
		}
	}
#if !SL
	public class DesktopPopupResizingStrategy : PopupResizingStrategyBase {
		protected override double HorizontalIndent {
			get { return 7d; }
		}
		protected override double VerticalIndent {
			get { return 7d; }
		}
		public DesktopPopupResizingStrategy(PopupBaseEdit editor)
			: base(editor) {
		}
		protected override Rect PopupRect {
			get {
				Point editorPoint = PopupScreenHelper.GetPopupScreenPoint(Editor);
				return new Rect(editorPoint, new Size(PopupChild.ActualWidth, PopupChild.ActualHeight));
			}
		}
		protected internal override Rect RootRect { get { return PopupScreenHelper.GetScreenRect(PopupChild); } }
		protected override Rect EditorRect {
			get {
				Point editorPoint = PopupScreenHelper.GetPopupScreenPoint(Editor);
				return new Rect(editorPoint, new Size(Editor.ActualWidth, Editor.ActualHeight));
			}
		}
	}
	public class PopupScreenHelper : ScreenHelper {
		public static Point GetPopupScreenPoint(PopupBaseEdit edit) {
			if (edit.PopupSettings.PopupResizingStrategy.IsRTL)
				return GetScaledPoint(edit.PointToScreen(new Point(edit.ActualWidth, 0)));
			return GetScaledPoint(edit.PointToScreen(new Point()));
		}
	}
#else
		public class SLPopupResizingStrategy : BrowserPopupResizingStrategy {
		public SLPopupResizingStrategy(PopupBaseEdit editor)
			: base(editor) {
		}
		protected override double HorizontalIndent {
			get { return 7d; }
		}
		protected override double VerticalIndent {
			get { return 7d; }
		}
		protected override bool IsRootRTL { get { return Root.FlowDirection == FlowDirection.RightToLeft; } }
		protected internal override Rect RootRect { get { return new Rect(new Point(), new Size(AppHelper.HostWidth, AppHelper.HostHeight)); } }
	}
#endif
}
