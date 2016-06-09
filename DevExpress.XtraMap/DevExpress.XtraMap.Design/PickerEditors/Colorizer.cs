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
using System.ComponentModel;
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.XtraMap;
namespace DevExpress.XtraMap.Design {
	public class ColorizerPickerEditor : GenericTypePickerEditor<MapColorizer> {
		protected override GenericTypePickerControl<MapColorizer> CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new ColorizerPickerControl(this, value);
		}
	}
	public class ColorizerPickerControl : GenericTypePickerControl<MapColorizer> {
		public ColorizerPickerControl(GenericTypePickerEditor<MapColorizer> editor, object editValue)
			: base(editor, editValue) {
		}
	}
	public class ColorizerValueProviderPickerEditor : GenericTypePickerEditor<IColorizerValueProvider> {
		protected override GenericTypePickerControl<IColorizerValueProvider> CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new ColorizerValueProviderPickerControl(this, value);
		}
	}
	public class ColorizerValueProviderPickerControl : GenericTypePickerControl<IColorizerValueProvider> {
		public ColorizerValueProviderPickerControl(GenericTypePickerEditor<IColorizerValueProvider> editor, object editValue)
			: base(editor, editValue) {
		}
	}
	public class ColorizerItemKeyProviderPickerEditor : GenericTypePickerEditor<IColorizerItemKeyProvider> {
		protected override GenericTypePickerControl<IColorizerItemKeyProvider> CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new ColorizerItemKeyProviderPickerControl(this, value);
		}
	}
	public class ColorizerItemKeyProviderPickerControl : GenericTypePickerControl<IColorizerItemKeyProvider> {
		public ColorizerItemKeyProviderPickerControl(GenericTypePickerEditor<IColorizerItemKeyProvider> editor, object editValue)
			: base(editor, editValue) {
		}
	}
}
