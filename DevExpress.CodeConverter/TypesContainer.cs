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

using DevExpress.CodeParser;
using System.Collections.Generic;
namespace DevExpress.CodeConverter {
	public class ElementsContainer {
		public ElementsContainer() {
		}
		public ElementsContainer(LanguageElement element) {
			Init(element);
		}
		protected virtual void Init(LanguageElement element) {
			Element = element.Clone() as LanguageElement;
		}
		public LanguageElement Element { get; private set; }
	}
	public class TypeContainer {
		Dictionary<string, ElementsContainer> fields;
		Dictionary<string, ElementsContainer> methods;
		Dictionary<string, ElementsContainer> properties;
		Dictionary<string, ElementsContainer> aliases;
		public TypeContainer(bool languageIsCaseSensetivity) {
			fields = new Dictionary<string, ElementsContainer>();
			methods = new Dictionary<string, ElementsContainer>();
			properties = new Dictionary<string, ElementsContainer>();
			aliases = new Dictionary<string, ElementsContainer>();
			LanguageIsCaseSensetivity = languageIsCaseSensetivity;
		}
		T Get<T>(string name, Dictionary<string, T> containers) where T: class
		{
			T result;
			if (!containers.TryGetValue(name, out result))
				return null;
			return result;
		}
		public void Add(LanguageElement element) {
			string name = element.Name;
			NamespaceReference nr = element as NamespaceReference;
			if (nr != null) {
				if (!nr.IsAlias)
					return;
				Expression aliasExp = nr.AliasExpression;
				if (aliasExp != null)
					name = aliasExp.Name;
				else
					name = nr.AliasName;
				name = CorrectName(name);
				if (!aliases.ContainsKey(name))
					aliases.Add(name, null);
				return;
			}
			name = CorrectName(name);
			if (string.IsNullOrEmpty(name))
				return;			
			BaseVariable var = element as BaseVariable;
			if (var != null && var.IsField)
			{
				if (!fields.ContainsKey(name))
					fields.Add(name, new ElementsContainer(var));
				return;
			}
			Method method = element as Method;
			if (method != null) {
				if (!methods.ContainsKey(name))
					methods.Add(name, new ElementsContainer(method));
				return;
			}
			Property property = element as Property;
			if (property != null) {
				if (!properties.ContainsKey(name))
				  properties.Add(name, new ElementsContainer(property));
				return;
			}
		}
		public bool HasAlias(string name) {
			if (string.IsNullOrEmpty(name))
				return false;
			return aliases.ContainsKey(CorrectName(name));
		}
		public bool HasNonPublicNonProtectedField(string name) {
			if (string.IsNullOrEmpty(name))
				return false;
			name = CorrectName(name);
			ElementsContainer elementsContainer;
			if (!fields.TryGetValue(name, out elementsContainer))
				return false;
			BaseVariable var = elementsContainer.Element as BaseVariable;
			return var != null && var.Visibility != MemberVisibility.Public && var.Visibility != MemberVisibility.Protected;
		}
		string CorrectName(string name) {
			return FileContainer.CorrectName(name, LanguageIsCaseSensetivity);
		}
		public bool LanguageIsCaseSensetivity { get; set; }
		public bool IsVoidMethod(string name) {
			if (string.IsNullOrEmpty(name))
				return false;
			name = CorrectName(name);
			ElementsContainer elementsContainer;
			if (!methods.TryGetValue(name, out elementsContainer))
				return false;
			Method method = elementsContainer.Element as Method;
			return method != null && method.MethodType == MethodTypeEnum.Void;
		}
		public bool HasProperty(string name) {
			if (string.IsNullOrEmpty(name))
				return false;
			name = CorrectName(name);
			return properties.ContainsKey(name);
		}
		public bool HasReadOnlyProperty(string name) {
			if (string.IsNullOrEmpty(name))
				return false;
			name = CorrectName(name);
			ElementsContainer elementsContainer;
			if (!properties.TryGetValue(name, out elementsContainer))
				return false;
			Property property = elementsContainer.Element as Property;
			return property != null && (!property.HasSetter || property.Setter.Visibility != MemberVisibility.Public);
		}
		public bool NeedReplaceAutoImplementedPropertyReference(string name)
		{
			if (string.IsNullOrEmpty(name))
				return false;
			name = CorrectName(name);
			ElementsContainer elementsContainer;
			if (!properties.TryGetValue(name, out elementsContainer))
				return false;
			Property property = elementsContainer.Element as Property;
			if(property == null || !property.IsAutoImplemented || property.Setter == null || property.Getter == null)
			  return false;
			return property.Setter.Visibility == MemberVisibility.Private && property.Setter.Visibility != property.Getter.Visibility;
		}
		public ElementsContainer GetMember(string name, LanguageElementType languageElementType) {
			switch(languageElementType)
			{
				case LanguageElementType.Property:
				  return Get<ElementsContainer>(name, properties);
				case LanguageElementType.Method:
					return Get<ElementsContainer>(name, methods);
				case LanguageElementType.Variable:
				case LanguageElementType.InitializedVariable:
				   return Get<ElementsContainer>(name, fields);
			}
			return null;
		}
	}
	public class FileContainer {
		Dictionary<string, TypeContainer> classes;
		public bool LanguageIsCaseSensetivity { get; set; }
		public FileContainer(bool languageIsCaseSensetivity) {
			classes = new Dictionary<string, TypeContainer>();
			LanguageIsCaseSensetivity = languageIsCaseSensetivity;
		}
		public void Add(LanguageElement element) {
			GetOrCreateTypeContainer(element);
		}
		public string CorrectName(string name) {
			return CorrectName(name, LanguageIsCaseSensetivity);
		}
		public static string CorrectName(string name, bool languageIsCaseSensetivity) {
			if (string.IsNullOrEmpty(name))
				return name;
			name = name.TrimStart('[');
			name = name.TrimEnd(']');
			if (languageIsCaseSensetivity)
				return name;
			return name.ToLower();
		}
		public void Add(LanguageElement type, LanguageElement element) {
			if (type == null || element == null)
				return;
			TypeContainer memberContainer = GetOrCreateTypeContainer(type);
			if (memberContainer == null)
				return;
			memberContainer.Add(element);
		}
		TypeContainer GetOrCreateTypeContainer(LanguageElement element) {
			if (element == null)
				return null;
			if (element.ElementType == LanguageElementType.Class)
				return GetOrCreateTypeContainer(classes, element as TypeDeclaration);
			return null;
		}
		TypeContainer GetOrCreateTypeContainer(Dictionary<string, TypeContainer> container, TypeDeclaration type) {
			string name = CorrectName(type.Name);
			if (container.ContainsKey(name))
				return container[name];
			TypeContainer result = new TypeContainer(LanguageIsCaseSensetivity);
			Namespace ns = type.GetParentNamespace();
			if (ns != null)
				foreach (LanguageElement element in ns.Nodes)
					if (element.ElementType == LanguageElementType.NamespaceReference)
						result.Add(element);
			container.Add(name, result);
			return result;
		}
		public TypeContainer GetTypeContainer(LanguageElement element)
		{
			if (element == null)
				return null;
			LanguageElement type = element.GetParentClassInterfaceStructOrModule();
			if (type == null)
				return null;
			string typeName = CorrectName(type.Name);
			 if (string.IsNullOrEmpty(typeName))
				return null;
			if (classes.ContainsKey(typeName))
				return classes[typeName];
			return null;
		}
		public bool HasNonPublicNonProtectedField(Expression expression) {
			TypeContainer mc = GetTypeContainer(expression);
			if (mc == null)
				return false;
			return mc.HasNonPublicNonProtectedField(expression.Name);
		}
		public bool HasAlias(TypeReferenceExpression expression) {
			if (expression == null)
				return false;
			TypeContainer mc = GetTypeContainer(expression);
			if (mc == null)
				return false;
			return mc.HasAlias(expression.Name);
		}
		public bool HasMemberInParent(string name, LanguageElement child){
			if (string.IsNullOrEmpty(name))
				return false;
			TypeContainer mc = GetTypeContainer(child);
			if (mc == null)
				return false;
			return mc.GetMember(name, child.ElementType) != null;
		}
		public bool HasProperty(Expression expression) 
		{
			TypeContainer mc = GetTypeContainer(expression);
			if (mc == null)
				return false;
		   return mc.HasProperty(expression.Name);
		}
		public bool HasReadOnlyProperty(Expression expression) {
			TypeContainer mc = GetTypeContainer(expression);
			if (mc == null)
				return false;
			return mc.HasReadOnlyProperty(expression.Name);
		}
		public bool NeedReplaceAutoImplementedPropertyReference(Expression expression)
		{
			TypeContainer mc = GetTypeContainer(expression);
			if (mc == null)
				return false;
			return mc.NeedReplaceAutoImplementedPropertyReference(expression.Name);
		}
		public bool IsVoidMethod(MethodCallExpression methodCall) {
			TypeContainer mc = GetTypeContainer(methodCall);
			if (mc == null)
				return false;
			return mc.IsVoidMethod(methodCall.Name);
		}
	}
}
