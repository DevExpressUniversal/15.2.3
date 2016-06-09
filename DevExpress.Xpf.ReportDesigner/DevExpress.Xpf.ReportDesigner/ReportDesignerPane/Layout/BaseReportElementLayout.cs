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
using System.Linq.Expressions;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout {
	public abstract class BaseReportElementLayout : INotifyPropertyChanged {
		public abstract XRControl Ref1 { get; }
		public abstract XRControl Ref2 { get; }
		public abstract double Param1 { get; set; }
		public abstract double Param2 { get; set; }
		public abstract double Param3 { get; set; }
		public abstract double Param4 { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void RaiseRef1Changed() {
			RaisePropertyChanged(() => Ref1);
		}
		protected virtual void RaiseRef2Changed() {
			RaisePropertyChanged(() => Ref2);
		}
		protected virtual void RaiseParam1Changed() {
			RaisePropertyChanged(() => Param1);
		}
		protected virtual void RaiseParam2Changed() {
			RaisePropertyChanged(() => Param2);
		}
		protected virtual void RaiseParam3Changed() {
			RaisePropertyChanged(() => Param3);
		}
		protected virtual void RaiseParam4Changed() {
			RaisePropertyChanged(() => Param4);
		}
		protected void RaisePropertyChanged<T>(Expression<Func<T>> expression) {
			RaisePropertyChanged(BindableBase.GetPropertyName(expression));
		}
		protected void RaisePropertyChanged(string name) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}
