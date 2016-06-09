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
using System.ComponentModel.Design;
using System.ComponentModel;
namespace DevExpress.XtraPivotGrid.Design {
	public class CustomTotalsCollectionEditor : CollectionEditor {
		bool changesCanceled;
		public CustomTotalsCollectionEditor(Type type)
			: base(type) { }
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			PivotGridCustomTotalCollectionBase collection = (PivotGridCustomTotalCollectionBase)value;
			PivotGridCustomTotalBase[] cloneArray = collection.CloneToArray();
			IComponentChangeService changeService = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			changesCanceled = false;
			object result = base.EditValue(new TypeDescriptorContext(this, context), provider, value);
			if(result == value) {
				if(changesCanceled) {
					collection.AssignArray(cloneArray);
					return collection;
				}
				if(collection.Count != cloneArray.Length) FireChanged(context, changeService);
				else {
					for(int i = 0; i < cloneArray.Length; i++)
						if(!cloneArray[i].IsEqual(collection[i])) {
							FireChanged(context, changeService);
							break;
						}
				}
			}
			return result;
		}
		static void FireChanged(ITypeDescriptorContext context, IComponentChangeService changeService) {
			if(changeService == null) return;
			changeService.OnComponentChanging(context.Instance, null);
			changeService.OnComponentChanged(context.Instance, null, null, null);
		}
		protected override void CancelChanges() {
			changesCanceled = true;
		}
		class TypeDescriptorContext : ITypeDescriptorContext {
			readonly CustomTotalsCollectionEditor Owner;
			readonly ITypeDescriptorContext Context;
			public TypeDescriptorContext(CustomTotalsCollectionEditor owner, ITypeDescriptorContext context) {
				Owner = owner;
				Context = context;
			}
			#region ITypeDescriptorContext Members
			public IContainer Container {
				get { return Context.Container; }
			}
			public object Instance {
				get { return Context.Instance; }
			}
			public void OnComponentChanged() {
				Context.OnComponentChanged();
			}
			public bool OnComponentChanging() {
				if(Owner.changesCanceled) return false;
				return Context.OnComponentChanging();
			}
			public PropertyDescriptor PropertyDescriptor {
				get { return Context.PropertyDescriptor; }
			}
			#endregion
			#region IServiceProvider Members
			public object GetService(Type serviceType) {
				return Context.GetService(serviceType);
			}
			#endregion
		}
	}
	public class FieldsCollectionEditor : DevExpress.Utils.UI.CollectionEditor {
		public FieldsCollectionEditor(Type type)
			: base(type) {
		}
		protected override object SetItems(object editValue, object[] value) {
			PivotGridFieldCollectionBase fields = editValue as PivotGridFieldCollectionBase;
			if(fields != null && fields.Data != null)
				fields.Data.SetIsDeserializing(true);
			try {
				return base.SetItems(editValue, value);
			} finally {
				if(fields != null && fields.Data != null)
					fields.Data.SetIsDeserializing(false);
			}
		}
		protected override DevExpress.Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			var form = base.CreateCollectionForm(serviceProvider);
			form.Text = "Field Collection Editor";
			form.ClientSize = new System.Drawing.Size(680, 386);
			return form;
		}
	}
}
