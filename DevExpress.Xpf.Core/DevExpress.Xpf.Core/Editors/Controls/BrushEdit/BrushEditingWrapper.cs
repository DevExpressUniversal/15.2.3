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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors.Internal {
	public abstract class BrushEditingWrapper {
		FrameworkElement contentElement;
		Brush brush;
		protected FrameworkElement ContentElement { get { return contentElement ?? (contentElement = FindContentElement()); } }
		public static BrushEditingWrapper Create(IBrushPicker picker, BrushType brushType) {
			if (brushType == BrushType.None)
				return new NoneBrushWrapper(picker);
			if (brushType == BrushType.SolidColorBrush)
				return new SolidColorBrushWrapper(picker);
			return null;
		}
		protected IBrushPicker Picker { get; private set; }
		public virtual Brush Brush { get { return brush; } }
		protected BrushEditingWrapper(IBrushPicker editor) {
			Picker = editor;
		}
		public abstract void Subscribe();
		public abstract void Unsubscribe();
		public virtual void SetBrush(object editValue) {
			this.brush = GetBrush(editValue);
		}
		public abstract Brush GetBrush(object baseValue);
		protected virtual FrameworkElement FindContentElement() {
			return LayoutHelper.FindElementByName((FrameworkElement)Picker, "PART_Content");
		}
		public virtual void GotFocus(RoutedEventArgs e) {
			FrameworkElement picker = ContentElement;
			if (picker == null)
				 return;
			if (!LayoutHelper.IsChildElementEx(picker, (DependencyObject)e.OriginalSource))
				picker.Focus();
		}
		public virtual void LostFocus(RoutedEventArgs e) {
		}
		public void FocusPickerIfNeeded() {
			bool rootHasFocus = Picker.HasFocus;
			if (!rootHasFocus)
				return;
			FrameworkElement picker = ContentElement;
			if (picker == null)
				return;
			picker.Focus();
		}
		public virtual bool NeedsKey(Key key, ModifierKeys modifiers) {
			return false;
		}
		public virtual void Sync() {
		}
	}
	public class NoneBrushWrapper : BrushEditingWrapper {
		public NoneBrushWrapper(IBrushPicker editor) : base(editor) {
		}
		public override Brush Brush { get { return null; } }
		public override void Subscribe() {
		}
		public override void Unsubscribe() {
		}
		public override void SetBrush(object editValue) {
		}
		public override Brush GetBrush(object baseValue) {
			return new SolidColorBrush(Text2ColorHelper.DefaultColor);
		}
	}
	public class SolidColorBrushWrapper : BrushEditingWrapper {
		ColorPicker ColorPicker { get { return ContentElement as ColorPicker; } }
		readonly Locker syncBrushLocker = new Locker();
		public SolidColorBrushWrapper(IBrushPicker editor)
			: base(editor) {
		}
		public override Brush Brush { get { return ColorPicker.Return(x => new SolidColorBrush(x.Color), () => new SolidColorBrush(Text2ColorHelper.DefaultColor)); } }
		public override void Subscribe() {
			ColorPicker.Do(x => x.ColorChanged += PickerColorChanged);
		}
		protected virtual void PickerColorChanged(object sender, RoutedEventArgs e) {
			ColorChangedEventArgs colorArgs = (ColorChangedEventArgs)e;
			syncBrushLocker.DoIfNotLocked(() => Picker.Do(x => x.PerformSync(colorArgs.Color)));
		}
		public override void SetBrush(object editValue) {
			base.SetBrush(editValue);
			syncBrushLocker.DoLockedAction(() => ColorPicker.Do(x => x.Color = GetBrush(editValue).ToSolidColorBrush().Color));
		}
		public override void Unsubscribe() {
			ColorPicker.Do(x => x.ColorChanged -= PickerColorChanged);
		}
		public override Brush GetBrush(object baseValue) {
			return baseValue.ToSolidColorBrush();
		}
		public override bool NeedsKey(Key key, ModifierKeys modifiers) {
			return ColorPicker.Return(x => x.NeedsKey(key, modifiers), () => false);
		}
 }
}
