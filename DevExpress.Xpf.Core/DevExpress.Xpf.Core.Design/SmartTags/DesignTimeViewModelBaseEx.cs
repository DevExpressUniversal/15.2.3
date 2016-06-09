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

using DevExpress.Design.UI;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Linq.Expressions;
namespace DevExpress.Xpf.Core.Design {
	public class DesignTimeViewModelBaseEx : DesignTimeViewModelBase {
		public DesignTimeViewModelBaseEx(IModelItem selectedItem)
			: base(selectedItem) {
		}
		protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> expression, Action<T, T> changedCallback = null) {
			string propertyName = WpfBindableBase.GetPropertyName<T>(expression);
			return SetProperty(ref storage, value, propertyName, changedCallback);
		}
		protected bool SetProperty<T>(ref T storage, T value, string propertyName = null, Action<T, T> changedCallback = null) {
			if(object.Equals(storage, value)) return false;
			T oldValue = storage;
			storage = value;
			if(changedCallback != null)
				changedCallback(oldValue, value);
			RaisePropertyChanged(propertyName);
			return true;
		}
	}
}
