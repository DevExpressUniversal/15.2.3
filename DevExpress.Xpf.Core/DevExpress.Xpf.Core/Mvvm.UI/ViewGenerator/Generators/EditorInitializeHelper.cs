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

using DevExpress.Entity.Model;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using System;
namespace DevExpress.Mvvm.UI.Native.ViewGenerator {
	public static class EditorInitializeHelper {
		public static void InitializeMaxLength(IModelItem textEdit, int maxLength) {
			textEdit.SetValueIfNotSet(TextEditSettings.MaxLengthProperty, maxLength, false);
		}
		public static void InitializeEnumItemsSource(IEdmPropertyInfo property, IModelItem comboBox, Type enumType = null) {
			if(enumType == null) enumType = property.GetUnderlyingClrType();
			IModelItem itemsSouce = comboBox.Context.CreateItem(typeof(EnumItemsSource));
			itemsSouce.Properties[BindableBase.GetPropertyName(() => new EnumItemsSource().EnumType)].SetValue(enumType);
			comboBox.SetValue(LookUpEditBase.ItemsSourceProperty, itemsSouce, false);
			comboBox.SetValueIfNotSet(ButtonEditSettings.IsTextEditableProperty, false, false);
			if(!property.HasNullableType())
				comboBox.SetValueIfNotSet(BaseEditSettings.AllowNullInputProperty, false);
			else comboBox.SetValueIfNotSet(ButtonEditSettings.NullValueButtonPlacementProperty, EditorPlacement.EditBox, false);
		}
		public static void InitializeCharEdit(IModelItem textEdit) {
			textEdit.SetValueIfNotSet(TextEdit.MaskTypeProperty, MaskType.Simple, false);
			textEdit.SetValueIfNotSet(TextEdit.MaskProperty, "C", false);
			textEdit.SetValueIfNotSet(TextEdit.MaskPlaceHolderProperty, ' ', false);
		}
		public static void InitializeMask(IModelItem textEdit, MaskType maskType, MaskInfo mask, int maxLength) {
			textEdit.SetValueIfNotSet(TextEdit.MaskTypeProperty, maskType, false);
			InitializeMask(textEdit, mask);
			InitializeMaxLength(textEdit, maxLength);
		}
		public static void InitializeRegExMask(IModelItem textEdit, MaskInfo mask, int maxLength) {
			InitializeMask(textEdit, GetMaskType(mask.RegExMaskType.Value), mask, maxLength);
		}
		static MaskType GetMaskType(RegExMaskType maskType) {
			switch(maskType) {
				case RegExMaskType.RegEx: return MaskType.RegEx;
				case RegExMaskType.Regular: return MaskType.Regular;
				case RegExMaskType.Simple: return MaskType.Simple;
				default: throw new NotSupportedException();
			}
		}
		public static void InitializeDateTimeMask(IModelItem textEdit, MaskInfo mask) {
			InitializeMask(textEdit, mask);
		}
		static void InitializeMask(IModelItem textEdit, MaskInfo mask) {
			AttributesApplier.ApplyMaskAttributesForEditor(mask, () => textEdit);
		}
	}
}
