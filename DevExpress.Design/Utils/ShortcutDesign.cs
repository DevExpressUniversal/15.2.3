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

using System.Drawing.Design;
using System.CodeDom;
using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.Utils.Design.Serialization {
	public class KeyShortcutCodeDomSerializer : CodeDomSerializer {
		protected CodeExpression SerializeModifier(string name) {
			return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Keys)), name);
		}
		static Hashtable fieldsHash = null;
		static Hashtable FieldsHash {
			get {
				if(fieldsHash == null) CreateFieldsHash();
				return fieldsHash;
			}
		}
		static void CreateFieldsHash() {
			fieldsHash = new Hashtable();
			FieldInfo[] fis = typeof(Keys).GetFields();
			foreach(FieldInfo fi in fis) {
				if(!fi.IsStatic || fi.IsSpecialName) continue;
				fieldsHash[fi.GetValue(null)] = fi.Name;
			}
		}
		protected CodeExpression SerializeKey(IDesignerSerializationManager manager, Keys key) {
			CodeExpressionCollection list = new CodeExpressionCollection();
			if((key & Keys.Modifiers) != 0) {
				if((key & Keys.Control) != 0) list.Add(SerializeModifier("Control"));
				if((key & Keys.Shift) != 0) list.Add(SerializeModifier("Shift"));
				if((key & Keys.Alt) != 0) list.Add(SerializeModifier("Alt"));
			}
			key = key & (~Keys.Modifiers);
			if(FieldsHash[key] == null) return null;
			list.Add(SerializeModifier(FieldsHash[key].ToString()));
			if(list.Count == 1) return list[0] as CodeExpression;
			CodeBinaryOperatorExpression bexp = new CodeBinaryOperatorExpression(list[0], CodeBinaryOperatorType.BitwiseOr, list[1]);
			for(int n = 2; n < list.Count; n++) {
				bexp = new CodeBinaryOperatorExpression(bexp, CodeBinaryOperatorType.BitwiseOr, list[n]);
			}
			return bexp;
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			KeyShortcut sh = value as KeyShortcut;
			object stackVal = manager.Context.Current;
			if(stackVal == null) return null;
			CodeObjectCreateExpression create = new CodeObjectCreateExpression(value.GetType());
			create.Parameters.Add(SerializeKey(manager, sh.Key));
			return create;
		}
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			return null;
		}
	}
	public class KeyShortcutTypeConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return KeysList;
		}
		static StandardValuesCollection keysList = null;
		public static StandardValuesCollection KeysList {
			get {
				if(keysList != null) return keysList;
				keysList = CreateList();
				return keysList;
			}
		}
		static StandardValuesCollection CreateList() {
			ArrayList list = new ArrayList();
			list.Add(KeyShortcut.Empty);
			list.Add("(custom)");
			FieldInfo[] fis = typeof(Shortcut).GetFields();
			foreach(FieldInfo fi in fis) {
				if(fi.IsSpecialName || !fi.IsStatic) continue;
				Shortcut sh = (Shortcut)fi.GetValue(null);
				if(sh == Shortcut.None) continue;
				list.Add(new KeyShortcut(sh));
			}
			return new StandardValuesCollection(list);
		}
	}
}
