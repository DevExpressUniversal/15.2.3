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

using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.Editors.Internal {
	public interface IBrushPicker {
		bool HasFocus { get; }
		object Brush { get; set; }
		event EventHandler BrushChanged;
		BrushType BrushType { get; set; }
		void PerformSync(object value);
		bool NeedsKey(Key key, ModifierKeys modifiers);
	}
	public class BrushPicker : Control, IBrushPicker {
		public static readonly DependencyProperty BrushProperty;
		public static readonly DependencyProperty BrushTypeProperty;
		public static readonly DependencyProperty EditModeProperty;
		static BrushPicker() {
			Type ownerType = typeof(BrushPicker);
			BrushProperty = DependencyPropertyManager.Register("Brush", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (o, args) => ((BrushPicker)o).BrushPropertyChanged(args.NewValue)));
			BrushTypeProperty = DependencyPropertyManager.Register("BrushType", typeof(BrushType), ownerType,
				new FrameworkPropertyMetadata(BrushType.None, FrameworkPropertyMetadataOptions.AffectsMeasure, (o, args) => ((BrushPicker)o).BrushTypeChanged((BrushType)args.NewValue)));
			EditModeProperty = DependencyPropertyManager.Register("EditMode", typeof(EditMode), ownerType, new FrameworkPropertyMetadata(EditMode.Standalone));
		}
		public event EventHandler BrushChanged;
		public BrushEditingWrapper BrushEditing {
			get { return editingWrapper; }
			set {
				if (editingWrapper == value)
					return;
				editingWrapper.Do(x => x.Unsubscribe());
				editingWrapper = value;
				editingWrapper.Do(x => x.Subscribe());
			}
		}
		BrushEditingWrapper editingWrapper;
		public bool HasFocus { get { return IsKeyboardFocusWithin; } }
		public EditMode EditMode {
			get { return (EditMode)GetValue(EditModeProperty); }
			set { SetValue(EditModeProperty, value); }
		}
		public object Brush {
			get { return GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		public BrushType BrushType {
			get { return (BrushType)GetValue(BrushTypeProperty); }
			set { SetValue(BrushTypeProperty, value); }
		}
		public BrushPicker() {
			this.SetDefaultStyleKey(typeof(BrushPicker));
			BrushEditing = BrushEditingWrapper.Create(this, BrushType.None);
		}
		public void PerformSync(object value) {
			Brush = BrushEditing.GetBrush(value);
			RaiseBrushChanged();
		}
		public bool NeedsKey(Key key, ModifierKeys modifiers) {
			return BrushEditing.Return(x => x.NeedsKey(key, modifiers), () => true);
		}
		protected virtual void RaiseBrushChanged() {
			if (BrushChanged != null)
				BrushChanged(this, EventArgs.Empty);
		}
		protected virtual void BrushTypeChanged(BrushType newValue) {
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (HasFocus)
				Focus();
			BrushEditing = BrushEditingWrapper.Create(this, BrushType);
			BrushEditing.FocusPickerIfNeeded();
			BrushEditing.SetBrush(Brush);
		}
		protected virtual void BrushPropertyChanged(object newValue) {
			BrushEditing.Do(x => x.SetBrush(newValue));
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			BrushEditing.Do(x => x.FocusPickerIfNeeded());
		}
	}
}
