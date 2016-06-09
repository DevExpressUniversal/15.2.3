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
using DevExpress.Utils.Design;
using DevExpress.Serialization.CodeDom;
namespace DevExpress.XtraBars.Design.Serialization {
	public class RibbonItemLinksSerializer : CodeDomSerializer {
		public override object Deserialize(IDesignerSerializationManager manager, object value) {
			return null;
		}
		void FilterCollection(BarItemLinkCollection coll, ICollection deltaColl) {
			for (int i = 0; i < coll.Count; i++) {
				bool found = false;
				foreach (BarItemLink link in deltaColl) {
					if (coll[i] == link) {
						found = true;
						break;
					}
				}
				if (!found) coll.RemoveAt(i);
				else i++;
			}
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			if (manager == null) throw new ArgumentNullException("manager");
			if (value == null) throw new ArgumentNullException("value");
			if (!(value is ICollection)) return null;
			CodePropertyReferenceExpression propRef = null;
#if DXWhidbey
			ExpressionContext context = manager.Context[typeof(ExpressionContext)] as ExpressionContext;
			if (context != null) propRef = context.Expression as CodePropertyReferenceExpression;
			PropertyDescriptor descriptor1 = manager.Context[typeof(PropertyDescriptor)] as PropertyDescriptor;
#else
			if(manager.Context.Current is CodePropertyReferenceExpression) {
				propRef = manager.Context.Current as CodePropertyReferenceExpression;
			} else {
				if(manager.Context.Current.GetType().Name == "CodeValueExpression") {
					propRef = manager.Context.Current.GetType().InvokeMember("Expression", System.Reflection.BindingFlags.GetProperty, null, manager.Context.Current, null) as CodePropertyReferenceExpression;
				}
			}
#endif
			if (propRef == null) return null;
			ICollection links = value as ICollection;
			ICollection links2 = null;
			DXCollectionCodeDomSerializer dxCollSerializer = new DXCollectionCodeDomSerializer();
			if (dxCollSerializer.IsInheritedDescriptor(descriptor1)) {
				links2 = dxCollSerializer.GetCollectionDelta((ICollection)dxCollSerializer.GetOriginalValue(descriptor1), links);
				if (links2 != null)
					links = links2;
			}
			CodeStatementCollection res = new CodeStatementCollection();
			CodeMethodReferenceExpression expression1 = new CodeMethodReferenceExpression(propRef, "Add");
			if (descriptor1.Attributes[typeof(HiddenInheritableCollectionAttribute)] != null) {
				FilterCollection(value as BarItemLinkCollection, links);
				return res;
			}
			foreach (BarItemLink link in links) {
				if (link.Item == null) continue;
				CodeExpression param = SerializeToExpression(manager, link.Item);
				if (param != null) {
					CodeMethodInvokeExpression cmAdd = new CodeMethodInvokeExpression();
					cmAdd.Method = expression1;
					cmAdd.Parameters.Add(param);
					if (ShouldAddBeginGroupParam(link)) cmAdd.Parameters.Add(SerializeToExpression(manager, link.BeginGroup));
					bool keyTipAdded = false;
					if (ShouldAddKeyTipParam(link)) {
						cmAdd.Parameters.Add(SerializeToExpression(manager, link.KeyTip));
						keyTipAdded = true;
					}
					if (ShouldAddDropDownKeyTipParam(link)) {
						if (!keyTipAdded)
							cmAdd.Parameters.Add(SerializeToExpression(manager, link.KeyTip));
						BarButtonItemLink buttonLink = link as BarButtonItemLink;
						cmAdd.Parameters.Add(SerializeToExpression(manager, buttonLink == null ? "" : buttonLink.DropDownKeyTip));
					}
					if (ShoulAddButtonGroupParam(link))
						cmAdd.Parameters.Add(SerializeToExpression(manager, link.ActAsButtonGroup));
					res.Add(cmAdd);
				}
			}
			return res;
		}
		bool ShouldAddBeginGroupParam(BarItemLink link) {
			return link.BeginGroup || !link.IsDefaultActAsButtonGroup;
		}
		bool ShouldAddKeyTipParam(BarItemLink link) {
			return link.KeyTip != string.Empty || !link.IsDefaultActAsButtonGroup;
		}
		bool ShouldAddDropDownKeyTipParam(BarItemLink link) {
			if (!link.IsDefaultActAsButtonGroup) return true;
			BarButtonItemLink buttonLink = link as BarButtonItemLink;
			return buttonLink != null && buttonLink.DropDownKeyTip != string.Empty;
		}
		bool ShoulAddButtonGroupParam(BarItemLink link) {
			return !link.IsDefaultActAsButtonGroup;
		}
	}
	public class BarShortcutCodeDomSerializer : CodeDomSerializer {
		protected CodeExpression SerializeModifier(string name) {
			return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Keys)), name);
		}
		static Hashtable fieldsHash = null;
		static Hashtable FieldsHash {
			get {
				if (fieldsHash == null) CreateFieldsHash();
				return fieldsHash;
			}
		}
		static void CreateFieldsHash() {
			fieldsHash = new Hashtable();
			FieldInfo[] fis = typeof(Keys).GetFields();
			foreach (FieldInfo fi in fis) {
				if (!fi.IsStatic || fi.IsSpecialName) continue;
				fieldsHash[fi.GetValue(null)] = fi.Name;
			}
		}
		protected CodeExpression SerializeKey(IDesignerSerializationManager manager, Keys key) {
			if (key == Keys.None) return null;
			CodeExpressionCollection list = new CodeExpressionCollection();
			if ((key & Keys.Modifiers) != 0) {
				if ((key & Keys.Control) != 0) list.Add(SerializeModifier("Control"));
				if ((key & Keys.Shift) != 0) list.Add(SerializeModifier("Shift"));
				if ((key & Keys.Alt) != 0) list.Add(SerializeModifier("Alt"));
			}
			key = key & (~Keys.Modifiers);
			if (FieldsHash[key] == null) return null;
			list.Add(SerializeModifier(FieldsHash[key].ToString()));
			if (list.Count == 1) return list[0] as CodeExpression;
			CodeBinaryOperatorExpression bexp = new CodeBinaryOperatorExpression(list[0], CodeBinaryOperatorType.BitwiseOr, list[1]);
			for (int n = 2; n < list.Count; n++) {
				bexp = new CodeBinaryOperatorExpression(bexp, CodeBinaryOperatorType.BitwiseOr, list[n]);
			}
			return bexp;
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			BarShortcut sh = value as BarShortcut;
			if (sh == null) return null;
			object stackVal = manager.Context.Current;
			if (stackVal == null) return null;
			CodeObjectCreateExpression create = new CodeObjectCreateExpression(typeof(BarShortcut));
			if (sh.Key != Keys.None)
				create.Parameters.Add(SerializeKey(manager, sh.Key));
			if (sh.SecondKey != Keys.None)
				create.Parameters.Add(SerializeKey(manager, sh.SecondKey));
			return create;
		}
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			return null;
		}
	}
	public class RibbonExpandCollapseItemCodeDomSerializer : DXComponentCodeDomSerializer {
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			return base.Serialize(manager, value);
		}
	}
	public class RibbonAutoHiddenPagesMenuItemCodeDomSerializer : DXComponentCodeDomSerializer {
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			return base.Serialize(manager, value);
		}
	}
}
