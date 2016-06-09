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

using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Xpf.Spreadsheet.Internal;
using System.Windows;
using DevExpress.Utils;
using System.Windows.Media;
using DevExpress.Office.Internal;
using DevExpress.Office.Utils;
using DevExpress.Office.Drawing;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using DevExpress.Data.Utils;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DrawingFont = System.Drawing.Font;
using DrawingColor = System.Drawing.Color;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public enum ManipulationTypes { None, HorizontalPan, VerticalPan, Zoom, Rotate }
	public interface IGestureClient {
		void OnManipulationStarting(ManipulationStartingEventArgs e);
		void OnManipulationStarted(ManipulationStartedEventArgs e);
		void OnManipulationDelta(ManipulationDeltaEventArgs e, ManipulationTypes type);
		bool ShouldRotate(ManipulationDeltaEventArgs e);
		bool ShouldProcessGesture(TouchEventArgs e);
		void OnTap(TouchEventArgs e);
	}
	public class GestureHelper {
		public GestureHelper(IGestureClient client, UIElement manipulationOwner) {
			Client = client;
			ManipulationOwner = manipulationOwner;
		}
		IGestureClient Client { get; set; }
		UIElement ManipulationOwner { get; set; }
		int TouchCount { get; set; }
		bool ManipulationStarted { get; set; }
		bool RotationStarted { get; set; }
		ManipulationTypes Type { get; set; }
		public void Start() {
			SubscribeEvents();
		}
		public void Stop() {
			UnsubscribeEvents();
		}
		protected internal virtual void SubscribeEvents() {
			ManipulationOwner.TouchDown += OnTouchDown;
			ManipulationOwner.TouchUp += OnTouchUp;
			ManipulationOwner.ManipulationStarting += OnManipulationStarting;
			ManipulationOwner.ManipulationStarted += OnManipulationStarted;
			ManipulationOwner.ManipulationInertiaStarting += OnManipulationInertiaStarting;
			ManipulationOwner.ManipulationDelta += OnManipulationDelta;
			ManipulationOwner.ManipulationCompleted += OnManipulationCompleted;
		}
		void OnManipulationStarted(object sender, ManipulationStartedEventArgs e) {
			Client.OnManipulationStarted(e);
		}
		private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e) {
			ManipulationStarted = false;
			RotationStarted = false;
			TouchCount = 0;
			Reset();
		}
		private void Reset() {
			stopManipulation = false;
			Type = ManipulationTypes.None;
			skipFirstManipulationDelta = 0;
		}
		int skipFirstManipulationDelta = 0;
		private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e) {
			int count = 3;
			if (skipFirstManipulationDelta < count) {
				skipFirstManipulationDelta++;
				return;
			}
			ManipulationStarted = true;
			if (ShouldDeterminateState()) DeterminateState(e);
			ProcessManipulationDelta(e);
		}
		private void ProcessManipulationDelta(ManipulationDeltaEventArgs e) {
			if ((Type == ManipulationTypes.Zoom || Type == ManipulationTypes.Rotate) && e.IsInertial) {
				e.Complete();
				e.Handled = true;
				return;
			}
			Client.OnManipulationDelta(e, Type);
		}
		private bool StopIfInertia(ManipulationDeltaEventArgs e) {
			if (e.IsInertial) {
				e.Complete();
				e.Handled = true;
			}
			return e.Handled;
		}
		private void DeterminateState(ManipulationDeltaEventArgs e) {
			var delta = e.DeltaManipulation;
			var cum = e.CumulativeManipulation;
			if (IsMultiTouch()) {
				double rotation = delta.Rotation;
				if (!RotationStarted) RotationStarted = Client.ShouldRotate(e);
				if (RotationStarted && rotation != 0)
					Type = ManipulationTypes.Rotate;
				else if (IsScaleChanged(delta))
					Type = ManipulationTypes.Zoom;
				return;
			}
			double deltaX = Math.Abs(delta.Translation.X);
			double deltaY = Math.Abs(delta.Translation.Y);
			if (deltaX != 0 && deltaX > deltaY)
				Type = ManipulationTypes.HorizontalPan;
			else if (deltaY != 0 && deltaY > deltaX)
				Type = ManipulationTypes.VerticalPan;
		}
		private static bool IsScaleChanged(ManipulationDelta delta) {
			return (delta.Scale.X != 1 || delta.Scale.Y != 1);
		}
		private bool IsMultiTouch() {
			return TouchCount > 1;
		}
		private bool ShouldDeterminateState() {
			return Type == ManipulationTypes.None;
		}
		private void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e) {
			e.TranslationBehavior.DesiredDeceleration = 0.001; 
		}
		private void OnManipulationStarting(object sender, ManipulationStartingEventArgs e) {
			if (stopManipulation) {
				e.Handled = true;
				e.Cancel();
				Reset();
			}
			Client.OnManipulationStarting(e);
		}
		private void OnTouchUp(object sender, TouchEventArgs e) {
			if (!IsMultiTouch()) {
				TryProcessTap(e);
			}
			Reset();
			TouchCount--;
		}
		private void TryProcessTap(TouchEventArgs e) {
			if ((DateTime.Now - touchDownTime) < MinTapTime && !ManipulationStarted) {
				Client.OnTap(e);
			}
		}
		bool stopManipulation = false;
		TimeSpan MinTapTime = new TimeSpan(0, 0, 0, 0, 500);
		DateTime touchDownTime;
		private void OnTouchDown(object sender, TouchEventArgs e) {
			if (!Client.ShouldProcessGesture(e)) {
				stopManipulation = true;
				return;
			}
			Reset();
			TouchCount++;
			touchDownTime = DateTime.Now;
		}
		protected internal virtual void UnsubscribeEvents() {
			ManipulationOwner.TouchDown -= OnTouchDown;
			ManipulationOwner.TouchUp -= OnTouchUp;
			ManipulationOwner.ManipulationStarting -= OnManipulationStarting;
			ManipulationOwner.ManipulationInertiaStarting -= OnManipulationInertiaStarting;
			ManipulationOwner.ManipulationDelta -= OnManipulationDelta;
			ManipulationOwner.ManipulationCompleted -= OnManipulationCompleted;
		}
	}
	class PostponeAction {
		public PostponeAction(Delegate action, object[] parameters) {
			Action = action;
			Parameters = parameters;
		}
		public Delegate Action { get; private set; }
		public object[] Parameters { get; private set; }
	}
	public class PostponedActionHelper {
		Queue<PostponeAction> queue;
		public PostponedActionHelper() {
			queue = new Queue<PostponeAction>();
		}
		public void EnqueuePostponed(Delegate action, object[] parameters) {
			queue.Enqueue(new PostponeAction(action, parameters));
		}
		public void EnqueuePostponed(Delegate action) {
			queue.Enqueue(new PostponeAction(action, null));
		}
		public void Execute() {
			while (queue.Count > 0) {
				PostponeAction postpone = queue.Dequeue();
				postpone.Action.DynamicInvoke(postpone.Parameters);
			}
		}
	}
	public static class TextInfoCalculator {
		public static TextSettings PrepareTextInfo(CellForegroundDisplayFormat displayFormat, System.Drawing.StringFormat stringFormat, Brush defalutForeground) {
			TextSettings info = SetText(displayFormat.Text, displayFormat.FontInfo, displayFormat.ForeColor, defalutForeground);
			info = SetTextAlignment(stringFormat, info);
			return info;
		}
		public static TextSettings SetText(string text, FontInfo fontInfo, System.Drawing.Color foreColor, Brush defalutForeground) {
			WpfFontInfo wpfFontInfo = (WpfFontInfo)fontInfo;
			TextSettings info = new TextSettings();
			info.Text = text;
			info.FontFamily = wpfFontInfo.FontFamily;
			info.FontStyle = wpfFontInfo.Typeface.Style;
			info.FontWeight = wpfFontInfo.Typeface.Weight;
			info.FontSize = CalculateFontSize(wpfFontInfo.SizeInPoints);
			info.TextDecorations = GetTextDecorations(fontInfo.Font);
			info.Foreground = !DXColor.IsEmpty(foreColor) ? XpfTypeConverter.ToPlatformColor(foreColor) : ((SolidColorBrush)defalutForeground).Color;
			return info;
		}
		static double CalculateFontSize(float sizeInPoints) {
			return Units.DocumentsToPixelsF(Units.PointsToDocumentsF(sizeInPoints), DevExpress.XtraSpreadsheet.Model.DocumentModel.DpiY);
		}
		public static CommentTextSettings CalculateCommentTextSettings(DrawingFont font, DrawingColor foreColor) {
			CommentTextSettings result = new CommentTextSettings();
			TypefaceResolver typefaceResolver = new TypefaceResolver();
			Typeface typeface = typefaceResolver.GetTypeface(font.FontFamily.Name, font.Bold, font.Italic);
			result.FontFamily = typeface.FontFamily;
			result.FontSize = CalculateFontSize(font.SizeInPoints);
			result.FontStyle = typeface.Style;
			result.FontWeight = typeface.Weight;
			result.Foreground = new SolidColorBrush(foreColor.ToWpfColor());
			result.TextWrapping = TextWrapping.Wrap;
			return result;
		}
		static TextDecorationCollection GetTextDecorations(DrawingFont font) {
			if (font.Underline) return TextDecorations.Underline;
			if (font.Strikeout) return TextDecorations.Strikethrough;
			return null;
		}
		public static TextSettings SetTextAlignment(System.Drawing.StringFormat stringFormat, TextSettings info) {
			info.TextAlignment = GetTextAlignment(stringFormat.Alignment);
			info.VerticalAlignment = GetVerticalAlignment(stringFormat.LineAlignment);
			info.TextTrimming = GetTextTrimming(stringFormat.Trimming);
			info.TextWrapping = GetTextWrapping(stringFormat.FormatFlags);
			return info;
		}
		private static TextWrapping GetTextWrapping(System.Drawing.StringFormatFlags stringFormatFlags) {
			return (stringFormatFlags & System.Drawing.StringFormatFlags.NoWrap) == 0 ? TextWrapping.Wrap : TextWrapping.NoWrap;
		}
		private static TextTrimming GetTextTrimming(System.Drawing.StringTrimming stringTrimming) {
			switch (stringTrimming) {
				case System.Drawing.StringTrimming.EllipsisCharacter:
					return TextTrimming.CharacterEllipsis;
				case System.Drawing.StringTrimming.EllipsisWord:
					return TextTrimming.WordEllipsis;
				default:
					return TextTrimming.None;
			}
		}
		private static System.Windows.VerticalAlignment GetVerticalAlignment(System.Drawing.StringAlignment stringAlignment) {
			switch (stringAlignment) {
				case System.Drawing.StringAlignment.Center:
					return VerticalAlignment.Center;
				case System.Drawing.StringAlignment.Far:
					return VerticalAlignment.Bottom;
				case System.Drawing.StringAlignment.Near:
					return VerticalAlignment.Top;
				default:
					return VerticalAlignment.Top;
			}
		}
		private static TextAlignment GetTextAlignment(System.Drawing.StringAlignment stringAlignment) {
			switch (stringAlignment) {
				case System.Drawing.StringAlignment.Center:
					return TextAlignment.Center;
				case System.Drawing.StringAlignment.Far:
					return TextAlignment.Right;
				case System.Drawing.StringAlignment.Near:
					return TextAlignment.Left;
				default:
					return TextAlignment.Left;
			}
		}
	}
#if !SL
	public class ThreadIdleWeakEventHandler<TOwner> : WeakEventHandler<TOwner, EventArgs, EventHandler> where TOwner : class {
		public ThreadIdleWeakEventHandler(TOwner owner, Action<TOwner, Object, EventArgs> onEventAction)
			: base(owner, onEventAction, OnDetach, CreateHandler) {
		}
		static EventHandler CreateHandler(WeakEventHandler<TOwner, EventArgs, EventHandler> handler) {
			return new EventHandler(handler.OnEvent);
		}
		static void OnDetach(WeakEventHandler<TOwner, EventArgs, EventHandler> handler, object obj) {
			System.Windows.Interop.ComponentDispatcher.ThreadIdle -= handler.Handler;
		}
	}
#endif
}
