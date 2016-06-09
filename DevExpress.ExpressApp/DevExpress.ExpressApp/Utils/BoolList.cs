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
	public class BoolValueChangedEventArgs : EventArgs {
		private bool newValue;
		private bool oldValue;
		public BoolValueChangedEventArgs(bool oldValue, bool newValue) {
			this.newValue = newValue;
			this.oldValue = oldValue;
		}
		public bool NewValue {
			get { return newValue; }
		}
		public bool OldValue {
			get { return oldValue; }
		}
	}
	public enum BoolListOperatorType { And, Or }
	public class BoolList : ISupportUpdate {
		private bool oldResultValue;
		private bool resultValue;
		private bool emptyListValue;
		private int lockCount;
		private LightDictionary<string, bool> list;
		private BoolListOperatorType operatorType;
		public BoolList(bool emptyListValue, BoolListOperatorType operatorType, ReadByNonExistentKeyMode readByNonExistentKeyMode) {
			this.emptyListValue = emptyListValue;
			this.resultValue = emptyListValue;
			this.operatorType = operatorType;
			list = new LightDictionary<string, bool>(readByNonExistentKeyMode);
		}
		public BoolList(bool emptyListValue, BoolListOperatorType operatorType) : this(emptyListValue, operatorType, ReadByNonExistentKeyMode.ReturnDefault) { }
		public BoolList(bool emptyListValue) : this(emptyListValue, BoolListOperatorType.And) { }
		public BoolList() : this(true, BoolListOperatorType.And) { }
		public bool ResultValue {
			get {
				return resultValue;
			}
		}
		public int Count {
			get { return list.Count; }
		}
		public bool this[string key] {
			get {
				return list[key];
			}
			set {
				BeginUpdate();
				try {
					list[key] = value;
				}
				finally {
					EndUpdate();
				}
			}
		}
		protected void OnResultValueChanged(bool oldResultValue, bool resultValue) {
			if(ResultValueChanged != null) {
				ResultValueChanged(this, new BoolValueChangedEventArgs(oldResultValue, resultValue));
			}
		}
		public void Clear() {
			BeginUpdate();
			try {
				list.Clear();
			}
			finally {
				EndUpdate();
			}
		}
		public void RemoveItem(string key) {
			BeginUpdate();
			try {
				list.Remove(key);
			}
			finally {
				EndUpdate();
			}
		}
		public void SetItemValue(string key, bool value) {
			this[key] = value;
		}
		public IEnumerable<string> GetKeys() {
			return list.GetKeys();
		}
		public bool Contains(string key) {
			return list.ContainsKey(key);
		}
		public void BeginUpdate() {
			if(lockCount == 0) {
				oldResultValue = ResultValue;
			}
			lockCount++;
		}
		public void EndUpdate() {
			if(lockCount > 0) {
				lockCount--;
				if(lockCount == 0) {
					Nullable<bool> newResultValue = new Nullable<bool>();
					foreach(bool currentValue in list.GetValues()) {
						if(operatorType == BoolListOperatorType.And) {
							if(newResultValue.HasValue) {
								newResultValue = newResultValue.Value && currentValue;
							}
							else {
								newResultValue = currentValue;
							}
							if(!newResultValue.Value) {
								break;
							}
						}
						else if(operatorType == BoolListOperatorType.Or) {
							if(newResultValue.HasValue) {
								newResultValue = newResultValue.Value || currentValue;
							}
							else {
								newResultValue = currentValue;
							}
							if(newResultValue.Value) {
								break;
							}
						}
						else {
							throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnknownValue, operatorType.ToString()), "operatorType");
						}
					}
					if(newResultValue.HasValue) {
						resultValue = newResultValue.Value;
					}
					else {
						resultValue = emptyListValue;
					}
					if(oldResultValue != resultValue) {
						OnResultValueChanged(oldResultValue, ResultValue);
					}
				}
				if(Changed != null) {
					Changed(this, EventArgs.Empty);
				}
			}
		}
		public override string ToString() {
			if(!ResultValue) {
				List<string> reasons = new List<string>(Count);
				foreach(string key in GetKeys()) {
					reasons.Add(key + "=" + this[key].ToString());
				}
				if(reasons.Count > 0) {
					return ResultValue.ToString() + "(" + string.Join(", ", reasons.ToArray()) + ")";
				}
			}
			return ResultValue.ToString();
		}
		public override bool Equals(object obj) {
			if(obj is bool) {
				return resultValue == (bool)obj;
			}
			if(obj is BoolList) {
				return resultValue == ((BoolList)obj).resultValue;
			}
			return false;
		}
		public override int GetHashCode() {
			int result = 0;
			foreach(string key in list.Keys) {
				if(this[key]) {
					result += key.GetHashCode();
				}
				else {
					result -= key.GetHashCode();
				}
			}
			return result;
		}
		public static implicit operator bool(BoolList boolList) {
			return boolList.resultValue;
		}
		public static bool operator !(BoolList boolList) {
			return !boolList.resultValue;
		}
		public static bool operator ==(BoolList boolList1, BoolList boolList2) {
			if(object.ReferenceEquals(boolList1, boolList2)) {
				return true;
			}
			if(!object.ReferenceEquals(boolList1, null) && !object.ReferenceEquals(boolList2, null)) {
				return boolList1.resultValue == boolList2.resultValue;
			}
			return false;
		}
		public static bool operator !=(BoolList boolList1, BoolList boolList2) {
			return !(boolList1 == boolList2);
		}
		public static bool operator ==(BoolList boolList, bool b) {
			if(!object.ReferenceEquals(boolList, null)) {
				return boolList.resultValue == b;
			}
			return false;
		}
		public static bool operator !=(BoolList boolList, bool b) {
			return !(boolList == b);
		}
		public static bool operator ==(bool b, BoolList boolList) {
			return boolList == b;
		}
		public static bool operator !=(bool b, BoolList boolList) {
			return !(boolList == b);
		}
		public event EventHandler<BoolValueChangedEventArgs> ResultValueChanged;
		public event EventHandler<EventArgs> Changed;
	}
}
