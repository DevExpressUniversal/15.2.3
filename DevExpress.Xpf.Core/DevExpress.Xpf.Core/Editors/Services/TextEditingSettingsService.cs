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
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Mvvm.Native;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
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
#endif
namespace DevExpress.Xpf.Editors.Services {
	public class BaseEditingSettingsService : BaseEditBaseService {
		public bool AllowEditing { get { return GetAllowEditing(); } }
		public bool AllowKeyHandling { get { return OwnerEdit.EditMode != EditMode.InplaceInactive && OwnerEdit.IsEnabled; } }
		public bool AllowTextInput { get { return EditStrategy.AllowTextInput; } }
		public virtual bool AllowSpin { get { return false; } }
		public virtual bool IsInLookUpMode { get { return false; } }
		protected virtual bool GetAllowEditing() {
			return !OwnerEdit.IsReadOnly && OwnerEdit.IsEnabled && AllowKeyHandling;
		}
		public BaseEditingSettingsService(BaseEdit editor)
			: base(editor) {
		}
	}
	public class TextEditSettingsService : BaseEditingSettingsService {
		new TextEditPropertyProvider PropertyProvider { get { return (TextEditPropertyProvider)base.PropertyProvider; } }
		new TextEditStrategy EditStrategy { get { return (TextEditStrategy)base.EditStrategy; } }
		public override bool AllowSpin { get { return EditStrategy.AllowSpin; } }
		public override bool IsInLookUpMode {
			get {
				return (OwnerEdit as LookUpEditBase)
					.Return(x => PropertyProvider.TextInputSettings.AllowRejectUnknownValues
				  || !string.IsNullOrEmpty(x.ValueMember)
				  || !string.IsNullOrEmpty(x.DisplayMember), () => false);
			}
		}
		public TextEditSettingsService(BaseEdit editor)
			: base(editor) {
		}
	}
	public class ButtonEditSettingsService : TextEditSettingsService {
		new ButtonEdit OwnerEdit { get { return (ButtonEdit)base.OwnerEdit; } }
		new ButtonEditPropertyProvider PropertyProvider { get { return (ButtonEditPropertyProvider)base.PropertyProvider; } }
		public ButtonEditSettingsService(ButtonEdit editor)
			: base(editor) {
		}
		protected override bool GetAllowEditing() {
			return base.GetAllowEditing() && GetIsTextEditableInternal();
		}
		protected virtual bool GetIsTextEditableInternal() {
			return PropertyProvider.IsTextEditable;
		}
	}
	public class PopupBaseEditSettingsService : ButtonEditSettingsService {
		new PopupBaseEdit OwnerEdit { get { return (PopupBaseEdit)base.OwnerEdit; } }
		public PopupBaseEditSettingsService(ButtonEdit editor)
			: base(editor) {
		}
		protected override bool GetAllowEditing() {
			return base.GetAllowEditing() && OwnerEdit.ShowText;
		}
	}
	public class LookUpEditBaseSettingsService : PopupBaseEditSettingsService {
		protected override bool GetIsTextEditableInternal() {
			return ((LookUpEditBasePropertyProvider)PropertyProvider).IsTextEditable;
		}
		public LookUpEditBaseSettingsService(LookUpEditBase editor) : base(editor) {}
	}
	public class CheckEditSettingsService : BaseEditingSettingsService {
		public CheckEditSettingsService(BaseEdit editor)
			: base(editor) {
		}
#if SL
		protected override bool GetAllowEditing() {
			if (!OwnerEdit.IsReadOnly && OwnerEdit.EditMode == EditMode.InplaceInactive && OwnerEdit.InplaceOwner != null)
				OwnerEdit.BeginInplaceEditingOld();
			return base.GetAllowEditing();
		}
#endif
	}
#if !SL
	public class PopupBrushEditSettingsService : PopupBaseEditSettingsService {
		public override bool IsInLookUpMode { get { return true; } }
		new PopupBrushEditBase OwnerEdit { get { return (PopupBrushEditBase)base.OwnerEdit; } }
		public PopupBrushEditSettingsService(PopupBrushEditBase editor)
			: base(editor) {
		}
	}
#endif
}
