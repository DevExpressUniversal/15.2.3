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
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Data.Filtering;
#if !SL
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Collections.Specialized;
using System.Windows.Controls;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Core;
using System.Windows.Media;
#else
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using System.Collections.Specialized;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
#endif
namespace DevExpress.Xpf.Editors.Services {
	public class RangeEditorService : BaseEditBaseService {
		public virtual bool ShouldRoundToBounds { get { return false; } }
		protected BaseEditingSettingsService EditingSettings { get { return PropertyProvider.GetService<BaseEditingSettingsService>(); } }
		public RangeEditorService(BaseEdit editor) : base(editor) {
		}
		public virtual object CorrentToBounds(object maskValue) {
			return maskValue;
		}
		public virtual bool SpinUp(object value, IMaskManagerProvider maskProvider) {
			return EditingSettings.AllowSpin && maskProvider.Instance.SpinUp();
		}
		public virtual bool SpinDown(object value, IMaskManagerProvider maskProvider) {
			return EditingSettings.AllowSpin && maskProvider.Instance.SpinDown();
		}
	}
	public class SpinEditRangeService : RangeEditorService {
		public override bool ShouldRoundToBounds { get { return EditStrategy.ShouldRoundToBounds; } }
		new SpinEditStrategy EditStrategy { get { return (SpinEditStrategy)base.EditStrategy; } }
		new SpinEdit OwnerEdit { get { return (SpinEdit)base.OwnerEdit; } }
		public SpinEditRangeService(BaseEdit editor)
			: base(editor) {
		}
		public override object CorrentToBounds(object maskValue) {
			return EditStrategy.Correct(maskValue);
		}
		public override bool SpinUp(object value, IMaskManagerProvider maskProvider) {
			if (!EditingSettings.AllowSpin)
				return false;
			try {
				decimal tempValue = EditStrategy.CreateValueConverter(value).Value + OwnerEdit.Increment;
				tempValue = EditStrategy.CreateValueConverter(EditStrategy.Correct(EditStrategy.ToRange(tempValue))).Value;
				maskProvider.SetMaskManagerValue(tempValue);
				return true;
			}
			catch {
				maskProvider.SetMaskManagerValue(EditStrategy.GetMaxValue());
				return true;
			}
		}
		public override bool SpinDown(object value, IMaskManagerProvider maskProvider) {
			if (!EditingSettings.AllowSpin)
				return false;
			try {
				decimal tempValue = EditStrategy.CreateValueConverter(value).Value - OwnerEdit.Increment;
				tempValue = EditStrategy.CreateValueConverter(EditStrategy.Correct(EditStrategy.ToRange(tempValue))).Value;
				maskProvider.SetMaskManagerValue(tempValue);
				return true;
			}
			catch {
				maskProvider.SetMaskManagerValue(EditStrategy.GetMinValue());
				return true;
			}
		}
	}
	public class DateEditRangeService : RangeEditorService {
		public override bool ShouldRoundToBounds { get { return EditStrategy.ShouldRoundToBounds; } }
		new DateEditStrategy EditStrategy { get { return (DateEditStrategy)base.EditStrategy; } }
		public DateEditRangeService(BaseEdit editor)
			: base(editor) {
		}
		public override object CorrentToBounds(object maskValue) {
			return EditStrategy.Correct(maskValue);
		}
	}
}
