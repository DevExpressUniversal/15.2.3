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
using DevExpress.Data;
using DevExpress.Utils.Design;
#if !SILVERLIGHT && !DXPORTABLE
#else
using System.Collections.Generic;
using System.Drawing;
#endif
namespace DevExpress.Utils {
#if !SL && !DXPORTABLE
	[TypeConverter(typeof(DefaultBooleanConverter))]
#endif
	[ResourceFinder(typeof(ResFinder))]
	public enum DefaultBoolean {
		True,
		False,
		Default
	};
#if SILVERLIGHT
	public class DataObject : System.Windows.IDataObject {
		public Dictionary<string, object> Data = new Dictionary<string, object>();
		public DataObject() {
		}
		public virtual object GetData(Type type) {
			return GetData(type.FullName);
		}
		public virtual bool GetDataPresent(Type type) {
			return GetDataPresent(type.FullName);
		}
		public virtual object GetData(string format) {
			return GetData(format, false);
		}
		public virtual bool GetDataPresent(string format) {
			return GetDataPresent(format, false);
		}
		public virtual object GetData(string format, bool autoConvert) {
			object result;
			Data.TryGetValue(format, out result);
			return result;
		}
		public virtual bool GetDataPresent(string format, bool autoConvert) {
			return Data.ContainsKey(format);
		}
		public string[] GetFormats(bool autoConvert) {
			List<string> result = new List<string>(Data.Keys);
			return result.ToArray();
		}
		public string[] GetFormats() {
			return GetFormats(true);
		}
		public void SetData(string format, object data, bool autoConvert) {
			Data.Add(format, data);
		}
		public virtual void SetData(string format, object data) {
			SetData(format, data, true);
		}
		public void SetData(Type format, object data) {
			SetData(format.FullName, data, true);
		}
		public void SetData(object data) {
			SetData(data.GetType(), data);
		}
	}
	public class SystemInformation {
		public static TimeSpan DoubleClickTime { get { return TimeSpan.FromMilliseconds(400); } }
		public static Size DoubleClickSize { get { return new Size(4, 4); } }
		public static Size DragSize { get { return new Size(4, 4); } }
		public static int MouseWheelScrollDelta { get { return 120; } }
	}
#endif
}
