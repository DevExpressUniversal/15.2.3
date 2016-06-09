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
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	#region PropertyAccessor<T> (abstract class)
	public abstract class PropertyAccessor<T> {
		public abstract T GetValue();
		public abstract bool SetValue(T value);
	}
	#endregion
	#region CachedPropertyAccessor<T>
	public class CachedPropertyAccessor<T> : PropertyAccessor<T> {
		T value;
		public T Value { get { return value; } set { this.value = value; } }
		public override T GetValue() {
			return value;
		}
		public override bool SetValue(T value) {
			if (Object.Equals(this.Value, value))
				return false;
			this.value = value;
			return true;
		}
	}
	#endregion
	#region CalculatedPropertyAccessor<T> (abstract class)
	public abstract class CalculatedPropertyAccessor<T> : PropertyAccessor<T> {
		public override T GetValue() {
			return CalculateValue();
		}
		protected internal abstract T CalculateValue();
	}
	#endregion
	#region SmartPropertyAccessor<T> (abstract class)
	public abstract class SmartPropertyAccessor<T> : CalculatedPropertyAccessor<T> {
		PropertyAccessor<T> currentAccessor;
		protected SmartPropertyAccessor() {
			this.currentAccessor = this;
		}
		public override T GetValue() {
			if (currentAccessor == this)
				return CalculateValue();
			else
				return currentAccessor.GetValue();
		}
		public override bool SetValue(T value) {
			bool result = SetValueCore(value);
			if (currentAccessor != this)
				result = currentAccessor.SetValue(value) || result;
			return result;
		}
		protected internal override T CalculateValue() {
			T result = CalculateValueCore();
			CachedPropertyAccessor<T> newAccessor = new CachedPropertyAccessor<T>();
			newAccessor.Value = result;
			currentAccessor = newAccessor;
			return result;
		}
		protected internal abstract T CalculateValueCore();
		protected internal abstract bool SetValueCore(T value);
	}
	#endregion
}
