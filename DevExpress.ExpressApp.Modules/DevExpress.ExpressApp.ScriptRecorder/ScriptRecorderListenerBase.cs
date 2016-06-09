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
using System.Text;
using DevExpress.Persistent.Base;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.ScriptRecorder {
	public interface IScriptRecorderControlListener : IDisposable {
		void SetActive(bool value);
		void RegisterControl(object obj);
		void UnRegisterControl(object obj);
	}
	public abstract class ScriptRecorderListenerBase<T> : IScriptRecorderControlListener {
		protected T obj;
		private bool isWriteLocked = false;
		protected virtual string ConvertValueToString(object value) {
			if(!IsEmptyValue(value)) {
				string result = ReflectionHelper.GetObjectDisplayText(value);
				return string.IsNullOrEmpty(result) ? EmptyValue : result;
			}
			return EmptyValue;
		}
		protected bool IsEmptyValue(object value) {
			return value == null || string.IsNullOrEmpty(value.ToString());
		}
		public bool IsWriteLocked {
			get { return isWriteLocked; }
			protected set { isWriteLocked = value; }
		}
		public abstract void RegisterControl(T obj);
		public abstract void UnRegisterControl(T obj);
		public void SetActive(bool value) {
			if(obj != null && IsWriteLocked != value) {
				if(value) {
					UnRegisterControl(obj);
				} else {
					RegisterControl(obj);
				}
			}
			IsWriteLocked = value;
		}
		public virtual string EmptyValue {
			get {
				return "''";
			}
		}
		void IDisposable.Dispose() {
			if(obj != null) {
				UnRegisterControl(obj);
				obj = default(T);
			}
		}
		#region IScriptRecorderControlListener Members
		void IScriptRecorderControlListener.RegisterControl(object obj) {
			RegisterControl((T)obj);
		}
		void IScriptRecorderControlListener.UnRegisterControl(object obj) {
			UnRegisterControl((T)obj);
		}
		#endregion
	}
}
