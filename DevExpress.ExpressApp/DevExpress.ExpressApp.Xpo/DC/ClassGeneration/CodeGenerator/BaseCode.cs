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
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal class BaseCode : CodeProvider {
		private string name;
		private bool isStatic;
		private Visibility visibility;
		private Virtuality virtuality;
		private readonly List<Attribute> attributes;
		public BaseCode(string name) {
			this.name = name;
			isStatic = false;
			visibility = Visibility.None;
			attributes = new List<Attribute>();
			virtuality = Virtuality.None;
		}
		protected void SetName(string name) {
			this.name = name;
		}
		protected bool RemoveAttribute(Attribute attribute) {
			return attributes.Remove(attribute);
		}
		protected void AttributesClear() {
			attributes.Clear();
		}
		protected override void GetCodeCore(CodeBuilder builder) {
			AttributesCode.Append(builder, Attributes);
			builder.AppendIndent();
			VisibilityHelper.Append(builder, visibility);
			StaticModifierHelper.Append(builder, IsStatic);
			VirtualityHelper.Append(builder, virtuality);
		}
		public virtual void AddAttribute(Attribute attribute) {
			Type newAttributeType = attribute.GetType();
			for(int x = 0; x < attributes.Count; x++) {
				if(attributes[x].GetType() == newAttributeType) {
					attributes.RemoveAt(x);
				}
			}
			attributes.Add(attribute);
		}
		public void AddAttribute(IEnumerable<Attribute> attributeCollection) {
			foreach(Attribute attribute in attributeCollection) {
				AddAttribute(attribute);
			}
		}
		public override string ToString() {
			return string.Format("{0}: {1}", GetType().Name, Name);
		}
		public string Name {
			get { return name; }
		}
		public bool IsStatic {
			get { return isStatic; }
			set { isStatic = value; }
		}
		public Visibility Visibility {
			get { return visibility; }
			set { visibility = value; }
		}
		public Virtuality Virtuality {
			get { return virtuality; }
			set { virtuality = value; }
		}
		public Attribute[] Attributes {
			get { return attributes.ToArray(); }
		}
	}
	internal enum Visibility {
		None,
		Public,
		Private,
		Internal,
		Protected,
		ProtectedInternal
	}
	internal enum Virtuality {
		None,
		Abstract,
		Virtual,
		Override,
		New
	}
	internal static class VisibilityHelper {
		public static void Append(CodeBuilder builder, Visibility visibility) {
			if(visibility != Visibility.None) {
				builder.Append(ToString(visibility)).Append(" ");
			}
		}
		public static string ToString(Visibility visibility) {
			switch(visibility) {
				case Visibility.Public:
					return "public";
				case Visibility.Private:
					return "private";
				case Visibility.Internal:
					return "internal";
				case Visibility.Protected:
					return "protected";
				case Visibility.ProtectedInternal:
					return "protected internal";
				case Visibility.None:
				default:
					return string.Empty;
			}
		}
	}
	internal static class VirtualityHelper {
		public static void Append(CodeBuilder builder, Virtuality virtuality) {
			if(virtuality != Virtuality.None) {
				builder.Append(ToString(virtuality)).Append(" ");
			}
		}
		public static string ToString(Virtuality virtuality) {
			switch(virtuality) {
				case Virtuality.Abstract:
					return "abstract";
				case Virtuality.Virtual:
					return "virtual";
				case Virtuality.Override:
					return "override";
				case Virtuality.New:
					return "new";
				case Virtuality.None:
				default:
					return string.Empty;
			}
		}
	}
	internal static class StaticModifierHelper {
		public static void Append(CodeBuilder builder, bool isStatic) {
			if(isStatic) {
				builder.Append(ToString(isStatic)).Append(" ");
			}
		}
		public static string ToString(bool isStatic) {
			if(isStatic) {
				return "static";
			}
			return string.Empty;
		}
	}
}
