#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Localization;
namespace DevExpress.ExpressApp.Utils {
	public interface IObservableValue<T> : IDisposable {
		T Value { get; set; }
		event EventHandler<EventArgs> ValueChanged;
	}
	public interface IValueConverter<LeftValueDataType, RightValueDataType> {
		LeftValueDataType ConvertRightToLeft(RightValueDataType rightValue);
		RightValueDataType ConvertLeftToRight(LeftValueDataType leftValue);
	}
	public class Binding<LeftValueDataType, RightValueDataType> : IDisposable {
		private IObservableValue<LeftValueDataType> left;
		private IObservableValue<RightValueDataType> right;
		private IValueConverter<LeftValueDataType, RightValueDataType> converter;
		private bool enableLeftValueChangedHandler = true;
		private bool enableRightValueChangedHandler = true;
		private bool isDisposed = false;
		public Binding(IObservableValue<LeftValueDataType> left, IObservableValue<RightValueDataType> right, IValueConverter<LeftValueDataType, RightValueDataType> converter) {
			this.left = left;
			this.left.ValueChanged += left_ValueChanged;
			this.right = right;
			this.right.ValueChanged += right_ValueChanged;
			this.converter = converter;
		}
		public void AssignValueFromLeftToRight() {
			if(isDisposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}
			left_ValueChanged(this, EventArgs.Empty);
		}
		public void Dispose() {
			isDisposed = true;
			if(left != null) {
				left.ValueChanged -= left_ValueChanged;
				left.Dispose();
				left = null;
			}
			if(right != null) {
				right.ValueChanged -= left_ValueChanged;
				right.Dispose();
				right = null;
			}
		}
		private void right_ValueChanged(object sender, EventArgs e) {
			if(isDisposed) {
				return;
			}
			if(enableRightValueChangedHandler) {
				enableLeftValueChangedHandler = false;
				try {
					left.Value = converter.ConvertRightToLeft(right.Value);
				}
				finally {
					enableLeftValueChangedHandler = true;
				}
			}
		}
		private void left_ValueChanged(object sender, EventArgs e) {
			if(isDisposed) {
				return;
			}
			if(enableLeftValueChangedHandler) {
				enableRightValueChangedHandler = false;
				try {
					right.Value = converter.ConvertLeftToRight(left.Value);
				}
				finally {
					enableRightValueChangedHandler = true;
				}
			}
		}
	}
}
