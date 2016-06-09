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
using System.Collections;
namespace DevExpress.ExpressApp.Model.Core {
	public interface IModelValue : ICloneable {
		void Move(int[] indices);
		bool HasValue(int aspectIndex);
		object GetObjectValue(int aspectIndex);
		void SetObjectValue(int aspectIndex, object value);
		void MergeWith(IModelValue mergedWith);
		void CombineWith(IModelValue combineWith);
		bool IsLocalizable { get; }
	}
	public interface IModelLocalizableValue {
		int AspectCount { get; }
	}
	public interface IModelValue<T> : IModelValue {
		T GetValue(int aspectIndex);
		void SetValue(int aspectIndex, T value);
	}
	sealed class ModelValue<T> : IModelValue<T> {
		T value;
		bool hasValue;
		public bool HasValue { get { return hasValue; } }
		public T Value {
			get { return this.value; }
			set {
				this.hasValue = true;
				this.value = value;
			}
		}
		public T GetValue(int aspectIndex) {
			return Value;
		}
		public void SetValue(int aspectIndex, T value) {
			Value = value;
		}
		public object GetObjectValue(int aspectIndex) {
			return GetValue(aspectIndex);
		}
		public void SetObjectValue(int aspectIndex, object value) {
			SetValue(aspectIndex, (T)value);
		}
		public ModelValue<T> Clone() {
			ModelValue<T> clone = new ModelValue<T>();
			clone.value = value;
			clone.hasValue = hasValue;
			return clone;
		}
		public override bool Equals(object obj) {
			ModelValue<T> other = obj as ModelValue<T>;
			if(other != null && HasValue == other.HasValue) {
				if(HasValue) {
					return Value != null ? Value.Equals(other.Value) : other.Value == null;
				}
				return true;
			}
			return false;
		}
		public override int GetHashCode() {
			return Value != null ? Value.GetHashCode() : 0;
		}
		public override string ToString() {
			return HasValue ? Value.ToString() : "n/a";
		}
		bool IModelValue.IsLocalizable { get { return false; } }
		void IModelValue.Move(int[] indices) { }
		bool IModelValue.HasValue(int aspectIndex) {
			return HasValue;
		}
		void IModelValue.MergeWith(IModelValue target) {
			ModelValue<T> modelValue = (ModelValue<T>)target;
			hasValue = modelValue.hasValue;
			value = modelValue.value;
		}
		void IModelValue.CombineWith(IModelValue target) {
			if(!HasValue) {
				((IModelValue)this).MergeWith(target);
			}
		}
		object ICloneable.Clone() {
			return Clone();
		}
	}
	sealed class ModelLocalizableValue<T> : IModelValue<T>, IModelLocalizableValue {
		private BitArray usedValues;
		private T[] values;
		private int ValuesCount { get { return values == null ? 0 : values.Length; } }
		public T this[int aspectIndex] {
			get { return GetValue(aspectIndex); }
			set { SetValue(aspectIndex, value); }
		}
		private void ChangeArraySizeAndCopy(int newSize) {
			ChangeArraySizeAndCopy(newSize, -1);
		}
		private void ChangeArraySizeAndCopy(int newSize, int removedAspectIndex) {
			BitArray newUsedValues = new BitArray(newSize);
			T[] newValues = new T[newSize];
			for(int i = 0, j = 0; i < ValuesCount && j < newValues.Length; i++) {
				if(i != removedAspectIndex) {
					if(HasValue(i)) {
						newUsedValues.Set(j, true);
						newValues[j] = GetValue(i);
					}
					j++;
				}
			}
			usedValues = newUsedValues;
			values = newValues;
		}
		private void FillMovedArray(int[] indices) {
			int newSize = GetMovedArrayLength(indices);
			BitArray newUsedValues = new BitArray(newSize);
			T[] newValues = new T[newSize];
			for(int i = 0; i < indices.Length; i++) {
				if(HasValue(i)) {
					int index = indices[i];
					newUsedValues.Set(index, true);
					newValues[index] = GetValue(i);
				}
			}
			usedValues = newUsedValues;
			values = newValues;
		}
		private int GetMovedArrayLength(int[] indices) {
			int maxLength = ValuesCount;
			for(int i = 0; i < indices.Length; ++i) {
				if(HasValue(i) && indices[i] >= maxLength) {
					maxLength = indices[i] + 1;
				}
			}
			return maxLength;
		}
		public void Remove(int index) {
			if(ValuesCount > 0) {
				ChangeArraySizeAndCopy(ValuesCount - 1, index);
			}
		}
		public void Move(int[] indices) {
			if(indices != null && indices.Length > 0 && ValuesCount > 0) {
				FillMovedArray(indices);
			}
		}
		public bool HasValue(int aspectIndex) {
			return aspectIndex >= 0 && aspectIndex < ValuesCount && usedValues.Get(aspectIndex);
		}
		public T GetValue(int aspectIndex) {
			if(HasValue(aspectIndex)) {
				return values[aspectIndex];
			}
			return default(T);
		}
		public void SetValue(int aspectIndex, T value) {
			if(aspectIndex >= ValuesCount) {
				ChangeArraySizeAndCopy(aspectIndex + 1);
			}
			usedValues.Set(aspectIndex, true);
			values[aspectIndex] = value;
		}
		public object GetObjectValue(int aspectIndex) {
			return GetValue(aspectIndex);
		}
		public void SetObjectValue(int aspectIndex, object value) {
			SetValue(aspectIndex, (T)value);
		}
		public ModelLocalizableValue<T> Clone() {
			ModelLocalizableValue<T> clone = new ModelLocalizableValue<T>();
			if(ValuesCount > 0) {
				clone.usedValues = (BitArray)usedValues.Clone();
				clone.values = new T[values.Length];
				values.CopyTo(clone.values, 0);
			}
			return clone;
		}
		public override bool Equals(object obj) {
			ModelLocalizableValue<T> other = obj as ModelLocalizableValue<T>;
			if(other != null && other.ValuesCount == ValuesCount) {
				for(int i = 0; i < ValuesCount; i++) {
					if(HasValue(i)) {
						if(other.HasValue(i)) {
							T value = GetValue(i);
							T otherValue = other.GetValue(i);
							if(value != null) {
								if(!value.Equals(otherValue)) {
									return false;
								}
							}
							else if(otherValue != null) {
								return false;
							}
						}
						else {
							return false;
						}
					}
					else if(other.HasValue(i)) {
						return false;
					}
				}
				return true;
			}
			return false;
		}
		public override int GetHashCode() {
			int hashCode = 0;
			for(int i = 0; i < ValuesCount; ++i) {
				T value = GetValue(i);
				if(value != null) {
					hashCode ^= value.GetHashCode();
				}
			}
			return hashCode;
		}
		public override string ToString() {
			if(ValuesCount > 0) {
				string[] list = new string[ValuesCount];
				for(int i = 0; i < ValuesCount; i++) {
					list[i] = HasValue(i) ? GetValue(i).ToString() : "n/a";
				}
				return string.Join(", ", list);
			}
			return "n/a";
		}
		bool IModelValue.IsLocalizable { get { return true; } }
		void IModelValue.MergeWith(IModelValue target) {
			ModelLocalizableValue<T> modelValue = (ModelLocalizableValue<T>)target;
			for(int i = modelValue.ValuesCount - 1; i >= 0; i--) {
				if(modelValue.HasValue(i)) {
					SetValue(i, modelValue.GetValue(i));
				}
			}
		}
		void IModelValue.CombineWith(IModelValue target) {
			ModelLocalizableValue<T> modelValue = (ModelLocalizableValue<T>)target;
			for(int i = modelValue.ValuesCount - 1; i >= 0; i--) {
				if(!HasValue(i) && modelValue.HasValue(i)) {
					SetValue(i, modelValue.GetValue(i));
				}
			}
		}
		int IModelLocalizableValue.AspectCount {
			get { return ValuesCount; }
		}
		object ICloneable.Clone() {
			return Clone();
		}
	}
}
