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
using System.Threading;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.MvvmSample.Helpers {
	public class ThePropertyChangedEventArgs<T> : EventArgs {
		T oldValue;
		T newValue;
		public ThePropertyChangedEventArgs(T oldValue, T newValue) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		public T OldValue { get { return oldValue; } }
		public T NewValue { get { return newValue; } }
	}
	public delegate void ThePropertyChangedEventHandler<T>(object sender, ThePropertyChangedEventArgs<T> e);
	public delegate void RaisePropertyChangedDelegate<T>(T oldValue, T newValue);
	public abstract class Disposable : IDisposable {
		bool disposed = false;
		public Disposable() {
			InitializeCommands();
		}
		~Disposable() { Dispose(false); }
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public bool Disposed { get { return disposed; } }
		public event EventHandler AfterDispose;
		protected virtual void DisposeManaged() { }
		protected virtual void DisposeUnmanaged() { }
		void Dispose(bool disposing) {
			if(Disposed) return;
			disposed = true;
			if(disposing)
				DisposeManaged();
			DisposeUnmanaged();
			RaiseAfterDispose();
		}
		void RaiseAfterDispose() {
			if(AfterDispose != null)
				AfterDispose(this, EventArgs.Empty);
			AfterDispose = null;
		}
		#region Commands
		protected virtual void InitializeCommands() {
			DisposeCommand = new SimpleActionCommand(DoDispose);
		}
		public ICommand DisposeCommand { get; private set; }
		void DoDispose(object p) { Dispose(); }
		#endregion
	}
	public abstract class BindableAndDisposable : Disposable, INotifyPropertyChanged {
		bool disposeSignal;
		public bool DisposeSignal {
			get { return disposeSignal; }
			private set { SetValue<bool>("DisposeSignal", ref disposeSignal, value); }
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void SetValue<T>(string propertyName, ref T field, T newValue) {
			SetValue<T>(propertyName, ref field, newValue, false, null);
		}
		protected void SetValue<T>(string propertyName, ref T field, T newValue, bool disposeOldValue) {
			SetValue<T>(propertyName, ref field, newValue, disposeOldValue, null);
		}
		protected void SetValue<T>(string propertyName, ref T field, T newValue, RaisePropertyChangedDelegate<T> raiseChangedDelegate) {
			SetValue<T>(propertyName, ref field, newValue, false, raiseChangedDelegate);
		}
		protected void SetValue<T>(string propertyName, ref T field, T newValue, bool disposeOldValue, RaisePropertyChangedDelegate<T> raiseChangedDelegate) {
			if(object.Equals(field, newValue)) return;
			T oldValue = field;
			field = newValue;
			RaisePropertyChanged(propertyName);
			if(raiseChangedDelegate != null)
				raiseChangedDelegate(oldValue, newValue);
			if(!disposeOldValue) return;
			IDisposable disposableOldValue = oldValue as IDisposable;
			if(disposableOldValue != null)
				disposableOldValue.Dispose();
		}
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		protected override void DisposeManaged() {
			DisposeSignal = true;
			DisposeSignal = false;
			PropertyChanged = null;
			base.DisposeManaged();
		}
	}
}
