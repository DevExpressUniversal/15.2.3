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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp {
	public class ObjectKeyHelper {
		public const string InvalidObjectKey = "";
		protected virtual string Encode(string str) { return str; }
		protected virtual string Decode(string str) { return str; }
		public string SerializeObjectKey(object objectKey) {
			MemoryStream stream = new MemoryStream();
			XmlSerializer serializer = new XmlSerializer(objectKey.GetType());
			serializer.Serialize(stream, objectKey);
			string result = Encoding.UTF8.GetString(stream.GetBuffer());
			int indexOfZero = result.IndexOf('\0');
			if(indexOfZero >= 0) {
				result = result.Remove(indexOfZero);
			}
			return Encode(result);
		}
		public object DeserializeObjectKey(string objectKey, Type objectType) {
			if(objectKey == InvalidObjectKey) {
				return null;
			}
			XmlSerializer serializer = new XmlSerializer(objectType);
			return serializer.Deserialize(new StringReader(Decode(objectKey)));
		}
		private static ObjectKeyHelper instance;
		public static void SetIntance(ObjectKeyHelper newInstance) {
			instance = newInstance;
		}
		public static ObjectKeyHelper Instance { 
			get
			{
				if (instance == null) {
					instance = new ObjectKeyHelper();
				}
				return instance;
			}
		}
	}
	public class ViewShortcut : LightDictionary<String, String>  {
		static class StringHelper {
			public static string Join(char separator, char escapeCharacter, params string[] values) {
				if(values == null || values.Length == 0) {
					return "";
				}
				string result = GetEncodedString(separator, escapeCharacter, values[0]);
				for(int i = 1; i < values.Length; i++) {
					result += separator + GetEncodedString(separator, escapeCharacter, values[i]);
				}
				return result;
			}
			private static string GetEncodedString(char separator, char escapeCharacter, string value) {
				return value.Replace(escapeCharacter.ToString(), string.Concat(escapeCharacter, escapeCharacter)).Replace(separator.ToString(), string.Concat(escapeCharacter, separator));
			}
			public static string[] Split(char separator, char escapeCharacter, string value) {
				if(string.IsNullOrEmpty(value)) {
					return new string[0];
				}
				List<string> result = new List<string>();
				int itemStartIndex = 0;
				for(int i = 0; i < value.Length; i++) {
					if(value[i] == escapeCharacter) {
						i++;
						if(i == value.Length || (value[i] != escapeCharacter && value[i] != separator)) {
							throw new ArgumentException(string.Format("The {0} value has excessive escape character.", value), "value");
						}
					}
					else if(value[i] == separator) {
						if(itemStartIndex == i) {
							result.Add("");
						}
						else {
							result.Add(GetDecodedString(separator, escapeCharacter, value.Substring(itemStartIndex, i - itemStartIndex)));
						}
						itemStartIndex = i + 1;
					}
				}
				if(itemStartIndex == value.Length) {
					result.Add("");
				}
				else if(itemStartIndex < value.Length) {
					result.Add(GetDecodedString(separator, escapeCharacter, value.Substring(itemStartIndex)));
				}
				return result.ToArray();
			}
			private static string GetDecodedString(char separator, char escapeCharacter, string value) {
				return value.Replace(string.Concat(escapeCharacter, escapeCharacter), escapeCharacter.ToString()).Replace(string.Concat(escapeCharacter, separator), separator.ToString());
			}
		}
		const char EscapeCharacter = '\\';
		const char PairSeparator = '&';
		const char KeyValueSeparator = '=';
		protected override String GetValueTypeDefault() {
			return "";
		}
		public const String scrollXYSeparator = "_";
		public ViewShortcut()
			: this(null, null, null, Point.Empty) {
		}
		public ViewShortcut(String viewId, String objectKey)
			: this(null, objectKey, viewId, Point.Empty) {
		}
		public ViewShortcut(Type objectClass, String objectKey, String viewId)
			: this(objectClass, objectKey, viewId, Point.Empty) {
		}
		public ViewShortcut(Type objectClass, String objectKey, String viewId, Point scrollPosition) {
			ViewId = viewId ?? "";
			ObjectKey = objectKey ?? ObjectKeyHelper.InvalidObjectKey;
			ScrollPosition = scrollPosition;
			ObjectClassName = (objectClass != null) ? objectClass.FullName : "";
		}
		public Type ObjectClass {
			get { return ReflectionHelper.FindType(ObjectClassName); }
		}
		public String ObjectKey {
			get { return this[ObjectKeyParamName]; }
			set { this[ObjectKeyParamName] = value; }
		}
		public String ViewId {
			get { return this[ViewIdParamName]; }
			set { this[ViewIdParamName] = value; }
		}
		public String ObjectClassName {
			get { return this[ObjectClassNameParamName]; }
			set { this[ObjectClassNameParamName] = value; }
		}
		public Point ScrollPosition {
			get { return ParseScrollPosition(this[ScrollPositionParamName]); }
			set { this[ScrollPositionParamName] = GetScrollPosition(value); }
		}
		private String GetScrollPosition(Point scrollPosition) {
			if(scrollPosition.IsEmpty) {
				return string.Empty;
			}
			return scrollPosition.X.ToString() + scrollXYSeparator + scrollPosition.Y.ToString();
		}
		private Point ParseScrollPosition(String scrollPosition) {
			string[] items = scrollPosition.Split(scrollXYSeparator.ToCharArray());
			int x, y;
			if(items.Length != 2 || !int.TryParse(items[0], out x) || !int.TryParse(items[1], out y)) {
				return new Point(0, 0);
			}
			return new Point(x, y);
		}
		public const String ViewIdParamName = "ViewID";
		public const String ObjectClassNameParamName = "ObjectClassName";
		public const String ObjectKeyParamName = "ObjectKey";
		public const String TemporaryObjectKeyParamName = "TemporaryObjectKey";
		public const String ScrollPositionParamName = "ScrollPosition";
		public const String IsNewObject = "NewObject";
		public override String ToString() {
			List<string> values = new List<string>();
			for(int i = 0; i < Count; i++) {
				string value = StringHelper.Join(KeyValueSeparator, EscapeCharacter, GetKey(i), this[i]);
				values.Add(value);
			}
			return StringHelper.Join(PairSeparator, EscapeCharacter, values.ToArray());
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override Boolean Equals(Object obj) {
			return Equals(obj, EqualsDefaultIgnoredParameters);
		}
		public static List<String> EqualsDefaultIgnoredParameters = new List<String>(new String[] { ScrollPositionParamName });
		public Boolean Equals(Object obj, IList<String> ignoredParamNames) {
			if(base.Equals(obj)) {
				return true;
			}
			ViewShortcut shortcut = obj as ViewShortcut;
			if(shortcut == null) {
				return false;
			}
			if((shortcut.ObjectClass == this.ObjectClass)
					&& (shortcut.ObjectKey == this.ObjectKey)
					&& (shortcut.ViewId == this.ViewId)) {
				int maxCount = (Count > shortcut.Count) ? Count : shortcut.Count;
				for(int i = 0; i < maxCount; i++) {
					if(i < shortcut.Count) {
						if(shortcut[i] != this[shortcut.GetKey(i)] && !ignoredParamNames.Contains(shortcut.GetKey(i))) {
							return false;
						}
					}
					if(i < Count) {
						if(this[i] != shortcut[this.GetKey(i)] && !ignoredParamNames.Contains(this.GetKey(i))) {
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}
		public Boolean HasViewParameters {
			get {
				return !string.IsNullOrEmpty(ObjectClassName) || !string.IsNullOrEmpty(ViewId);
			}
		}
		public static Boolean operator ==(ViewShortcut a, ViewShortcut b) {
			if((object)a != null) {
				return a.Equals(b);
			}
			else {
				if((object)b != null) {
					return b.Equals(a);
				}
				else {
					return true;
				}
			}
		}
		public static Boolean operator !=(ViewShortcut a, ViewShortcut b) {
			return !(a == b);
		}
		public static readonly ViewShortcut Empty = new ViewShortcut(null, ObjectKeyHelper.InvalidObjectKey, "");
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ViewShortcut FromString(string shortcutString) {
			ViewShortcut result = ViewShortcut.Empty;
			if(!string.IsNullOrEmpty(shortcutString)) {
				result = new ViewShortcut();
				string[] parts = StringHelper.Split(PairSeparator, EscapeCharacter, shortcutString);
				foreach(string pairString in parts) {
					string[] pair = StringHelper.Split(KeyValueSeparator, EscapeCharacter, pairString);
					string key = pair[0];
					string value = pair.Length > 1 ? pair[1] : "";
					result[key] = value;
				}
			}
			return result;
		}
	}
	public class ViewParameters {
		private String viewId = null;
		private Type objectType;
		private Object objectKey = null;
		private Point scrollPosition;
		private IObjectSpace objectSpace;
		public ViewParameters(String viewId, Type objectType, Object objectKey) {
			this.viewId = viewId;
			this.objectType = objectType;
			this.objectKey = objectKey;
		}
		public ViewParameters(String viewId, Type objectType, Object objectKey, Point scrollPosition)
			: this(viewId, objectType, objectKey) {
			this.scrollPosition = scrollPosition;
		}
		public String ViewId {
			get { return viewId; }
		}
		public Type ObjectType {
			get { return objectType; }
		}
		public Object ObjectKey {
			get { return objectKey; }
		}
		public Point ScrollPosition {
			get { return scrollPosition; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
			protected internal set { objectSpace = value; }
		}
	}
	public class DashboardViewParameters: ViewParameters {
		public DashboardViewParameters(String viewId, Point scrollPosition)
			: base(viewId, null, null, scrollPosition) {
		}
	}
	public class ListViewParameters : ViewParameters {
		public ListViewParameters(String viewId, Type objectType, Object objectKey, Point scrollPosition)
			: base(viewId, objectType, objectKey, scrollPosition) {
		}
	}
	public class DetailViewParameters : ViewParameters {
		public DetailViewParameters(String viewId, Type objectType, Object objectKey, Point scrollPosition)
			: base(viewId, objectType, objectKey, scrollPosition) {
		}
	}
}
