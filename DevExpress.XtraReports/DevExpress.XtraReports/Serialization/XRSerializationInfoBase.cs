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
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace DevExpress.XtraReports.Serialization {
	public abstract class XRSerializationInfoBase {
		FormatterConverter converter;
		protected XRSerializationInfoBase() {
			converter = new FormatterConverter();
		}
		protected abstract bool ContainsKey(string name);
		protected abstract object GetValue(string name);
		protected abstract void AddValueInternal(string name, object value);
		public object GetValue(string name, Type type, object defaultValue) {
			if (!ContainsKey(name))
				return defaultValue;
			object item = GetValue(name);
			return item == null || type.IsAssignableFrom(item.GetType()) ? item : converter.Convert(item, type);
		}
		public string GetString(string name, string defaultValue) {
			if (!ContainsKey(name))
				return defaultValue;
			object item = GetValue(name);
			return item is string ? (string)item : converter.ToString(item);
		}
		public int GetInt32(string name, int defaultValue) {
			if (!ContainsKey(name))
				return defaultValue;
			object item = GetValue(name);
			return item is int ? (int)item : converter.ToInt32(item);
		}
		public Single GetSingle(string name, float defaultValue) {
			if (!ContainsKey(name))
				return defaultValue;
			object item = GetValue(name);
			return item is float ? (float)item : converter.ToSingle(item);
		}
		public bool GetBoolean(string name, bool defaultValue) {
			if (!ContainsKey(name))
				return defaultValue;
			object item = GetValue(name);
			return item is bool ? (bool)item : converter.ToBoolean(item);
		}
		public void AddValue(string name, bool value) {
			AddValueInternal(name, value);
		}
		public void AddValue(string name, byte value) {
			AddValueInternal(name, value);
		}
		public void AddValue(string name, char value) {
			AddValueInternal(name, value);
		}
		public void AddValue(string name, DateTime value) {
			AddValueInternal(name, value);
		}
		public void AddValue(string name, decimal value) {
			AddValueInternal(name, value);
		}
		public void AddValue(string name, double value) {
			AddValueInternal(name, value);
		}
		public void AddValue(string name, short value) {
			AddValueInternal(name, value);
		}
		public void AddValue(string name, int value) {
			AddValueInternal(name, value);
		}
		public void AddValue(string name, long value) {
			AddValueInternal(name, value);
		}
		public void AddValue(string name, object value) {
			AddValueInternal(name, value);
		}
		public abstract void AddValue(string name, object value, Type type);
	}
}
