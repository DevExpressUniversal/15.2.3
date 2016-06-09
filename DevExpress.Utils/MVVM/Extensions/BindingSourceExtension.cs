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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Linq.Expressions;
	using System.Windows.Forms;
	public static class BindingSourceExtension {
		public static IDisposable SetDataSourceBinding<TViewModel, TModel>(
			this BindingSource bindingSource, Control container,
			Expression<Func<TViewModel, TModel>> entitySelector, Expression<Action<TViewModel>> updateCommandSelector = null) where TViewModel : class {
			return new EntityBindingSource<TViewModel, TModel>(bindingSource, MVVMContext.FromControl(container), entitySelector, updateCommandSelector);
		}
		public static IDisposable SetDataSourceBinding<TViewModel, TModel>(
			this BindingSource bindingSource, MVVMContext mvvmContext,
			Expression<Func<TViewModel, TModel>> entitySelector, Expression<Action<TViewModel>> updateCommandSelector = null) where TViewModel : class {
			return new EntityBindingSource<TViewModel, TModel>(bindingSource, mvvmContext, entitySelector, updateCommandSelector);
		}
		sealed class EntityBindingSource<TViewModel, TModel> : IDisposable
			where TViewModel : class {
			IDisposable triggerRef;
			BindingSource bindingSource;
			MVVMContext mvvmContext;
			public EntityBindingSource(BindingSource bindingSource, MVVMContext mvvmContext,
				Expression<Func<TViewModel, TModel>> entitySelector, Expression<Action<TViewModel>> updateCommandSelector = null) {
				if(mvvmContext != null && bindingSource != null) {
					this.bindingSource = bindingSource;
					this.mvvmContext = mvvmContext;
					this.mvvmContext.Disposed += mvvmContext_Disposed;
					this.bindingSource.Disposed += bindingSource_Disposed;
					TViewModel viewModel = mvvmContext.GetViewModel<TViewModel>();
					ITriggerAction triggerAction;
					triggerRef = this.mvvmContext.Register(
						BindingHelper.SetNPCTrigger<TViewModel, TModel>(viewModel,
						entitySelector, (entity) =>
						{
							if(object.Equals(bindingSource.DataSource, entity))
								bindingSource.ResetBindings(false);
							else {
								if(!object.ReferenceEquals(entity, null))
									bindingSource.DataSource = entity;
								else
									bindingSource.DataSource = typeof(TModel);
							}
						}, out triggerAction));
					if(updateCommandSelector != null) {
						this.syncContext = System.Threading.SynchronizationContext.Current;
						mvvmContext.Register(DevExpress.Utils.MVVM.CommandHelper.Bind(this,
							(@this, execute) => update = () => execute(),
							(@this, canExecute) => canUpdate = canExecute(),
							updateCommandSelector, viewModel));
						this.bindingSource.ListChanged += bindingSource_ListChanged;
					}
					UpdateBindingSource(entitySelector, triggerAction);
				}
			}
			void IDisposable.Dispose() {
				OnDisposing();
				GC.SuppressFinalize(this);
			}
			void OnDisposing() {
				this.updateQueued = -1;
				this.canUpdate = false;
				if(mvvmContext != null)
					mvvmContext.Disposed -= mvvmContext_Disposed;
				if(bindingSource != null) {
					bindingSource.Disposed -= bindingSource_Disposed;
					bindingSource.ListChanged -= bindingSource_ListChanged;
				}
				Ref.Dispose(ref triggerRef);
				this.update = null;
				this.syncContext = null;
				this.mvvmContext = null;
				this.bindingSource = null;
			}
			void mvvmContext_Disposed(object sender, EventArgs e) {
				((IDisposable)this).Dispose();
			}
			void bindingSource_Disposed(object sender, EventArgs e) {
				((IDisposable)this).Dispose();
			}
			void bindingSource_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
				if(e.ListChangedType == System.ComponentModel.ListChangedType.ItemChanged)
					QueueViewModelUpdate();
			}
			void UpdateBindingSource(Expression<Func<TViewModel, TModel>> entitySelector, ITriggerAction triggerAction) {
				INotifyPropertyChangedTrigger trigger = BindingHelper.GetNPCTrigger(triggerRef);
				trigger.Execute(ExpressionHelper.GetPropertyName(entitySelector), triggerAction);
			}
			volatile int updateQueued = 0;
			System.Threading.SynchronizationContext syncContext;
			void QueueViewModelUpdate() {
				if(updateQueued < 0) return;
				if(0 == updateQueued) {
					updateQueued++;
					syncContext.Post(UpdateViewModel, mvvmContext.GetViewModel<TViewModel>());
				}
				else updateQueued++;
			}
			volatile bool canUpdate;
			Action update;
			void UpdateViewModel(object state) {
				if(canUpdate) {
					if(update != null)
						update();
				}
				if(updateQueued > 0)
					updateQueued = 0;
			}
		}
	}
}
