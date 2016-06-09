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

using System.Linq;
using DevExpress.Xpf.PropertyGrid;
using DevExpress.Xpf.PropertyGrid.Internal;
using System.ComponentModel;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
namespace DevExpress.XtraVerticalGrid.Data {
	public class NewListItemDescriptorContext : ListItemDescriptorContext {
		public NewListItemDescriptorContext(DescriptorContext parentContext)
			: base(parentContext.Value, ListConverter.CreateNewListItemPropertyDescriptor(parentContext), parentContext.ServiceProvider, parentContext.IsMultiSource) {
				ParentContext = parentContext;
		}
		public override IInstanceInitializer GetInstanceInitializer() {
			IInstanceInitializer initializer = ParentContext.EdmPropertyInfo.Return(e => e.Attributes.NewItemInstanceInitializer(), () => null);
			if (initializer != null)
				return initializer;
			return DataController.VisualClient.GetListItemInitializer(ParentContext.RowHandle);
		}
		protected override TypeConverter GetConverterInternal() {
			return DataController.NewInstanceConverter;
		}
		internal protected override RowHandle SetValueInternal(object value) {
			base.SetValueInternal(value);
			return ParentContext.RowHandle;
		}
		protected override void ParentContextChanged() {
		}
		protected override void BeforeSetValue(object originalValue, object convertedValue) {
			if (convertedValue != null)
				ParentContext.NewListItemValue = originalValue as TypeInfo;
		}
		protected override object GetValue() {
			if (ParentContext.NewListItemValue == null)
				return StandardValues.Cast<object>().FirstOrDefault();
			return ParentContext.NewListItemValue;
		}
		public override RowHandle ResetValue() {
			base.ResetValue();
			return ParentContext.RowHandle;
		}
		public override bool CanResetValue() {
			return false;
		}
		protected override void SubscribeNotifications() {
		}
	}
}
