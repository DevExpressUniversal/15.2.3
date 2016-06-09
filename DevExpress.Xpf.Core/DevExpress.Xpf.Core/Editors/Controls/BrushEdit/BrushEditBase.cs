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
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Editors {
	public interface IBrushEdit : IBaseEdit {
	}
	public interface IPopupBrushEdit : IBrushEdit {
		bool AllowEditBrushType { get; }
		PopupBrushValue GetPopupBrushValue(BrushType brushType);
		BrushType BrushType { get; }
	}
	public enum BrushType {
		None,
		SolidColorBrush,
		LinearGradientBrush,
		RadialGradientBrush,
		AutoDetect,
	}
	public abstract class BrushEditBase : BaseEdit, IBrushEdit {
		static readonly DependencyPropertyKey BrushTypePropertyKey;
		public static readonly DependencyProperty BrushTypeProperty;
		static BrushEditBase() {
			Type ownerType = typeof(BrushEditBase);
			ValidateOnTextInputProperty.OverrideMetadata(typeof(BrushEditBase), new FrameworkPropertyMetadata(true));
			BrushTypePropertyKey = DependencyPropertyManager.RegisterReadOnly("BrushType", typeof(BrushType), ownerType,
				new FrameworkPropertyMetadata(BrushType.SolidColorBrush, FrameworkPropertyMetadataOptions.None, (o, args) => ((BrushEditBase)o).BrushTypeChanged((BrushType)args.NewValue)));
			BrushTypeProperty = BrushTypePropertyKey.DependencyProperty;
		}
		protected internal override Type StyleSettingsType { get { return typeof(BrushEditStyleSettingsBase); } }
		public BrushType BrushType {
			get { return (BrushType)GetValue(BrushTypeProperty); }
			internal set { SetValue(BrushTypePropertyKey, value); }
		}
		internal IBrushPicker BrushPicker { get { return EditCore as IBrushPicker; } }
		protected BrushEditBase() {
			this.SetDefaultStyleKey(typeof(BrushEditBase));
		}
		new BrushEditStrategy EditStrategy { get { return base.EditStrategy as BrushEditStrategy; } }
		protected override EditStrategyBase CreateEditStrategy() {
			return new BrushEditStrategy(this);
		}
		protected internal override BaseEditStyleSettings CreateStyleSettings() {
			return new SolidColorBrushEditStyleSettings();
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new BrushEditPropertyProvider(this);
		}
		protected override void SubscribeEditEventsCore() {
			base.SubscribeEditEventsCore();
			BrushPicker.Do(x => x.BrushChanged += BrushPickerBrushChanged);
		}
		void BrushPickerBrushChanged(object sender, EventArgs eventArgs) {
			EditStrategy.SyncWithEditor();
		}
		protected override void UnsubscribeEditEventsCore() {
			base.UnsubscribeEditEventsCore();
			BrushPicker.Do(x => x.BrushChanged -= BrushPickerBrushChanged);
		}
		protected virtual void BrushTypeChanged(BrushType newValue) {
			BrushPicker.Do(x => x.BrushType = newValue);
		}
		protected internal override bool NeedsKey(Key key, ModifierKeys modifiers) {
			bool result = BrushPicker.Return(x => x.NeedsKey(key, modifiers), () => true);
			return result && base.NeedsKey(key, modifiers);
		}
		protected override bool NeedsTab() {
			return true;
		}
	}
}
