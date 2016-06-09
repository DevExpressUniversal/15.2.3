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
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using DevExpress.XtraGauges.Core.Base;
namespace DevExpress.Web.ASPxGauges.Base {
	public abstract class BaseComponentBuilder<T> : ControlBuilder
		where T : class, IComponent, INamed {
		IDictionary<string, string> aliasesCore;
		protected IDictionary<string, string> Aliases {
			get { return aliasesCore; }
		}
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs) {
			this.aliasesCore = new Dictionary<string, string>();
			BuildAliases(Aliases);
			PreFilterAttributes(attribs);
			base.Init(parser, parentBuilder, type, tagName, id, attribs);
		}
		public override object BuildObject() {
			object obj = null;
			using(new ComponentBuilderInterceptorHelper()) {
				obj = base.BuildObject();
				if(obj is T) 
					OnComponentBuiltSuccess((T)obj);
			}
			return obj;
		}
		public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeTypeDeclaration baseType, CodeTypeDeclaration derivedType, CodeMemberMethod buildMethod, System.CodeDom.CodeMemberMethod dataBindingMethod) {
			if(buildMethod != null) ProcessComponentInitializationCode(buildMethod);
			base.ProcessGeneratedCode(codeCompileUnit, baseType, derivedType, buildMethod, dataBindingMethod);
		}
		protected void ProcessComponentInitializationCode(CodeMemberMethod buildMethod) {
			Type buildedType = Type.GetType(buildMethod.ReturnType.BaseType, false);
			if(typeof(T).IsAssignableFrom(buildedType) && typeof(ISupportLockUpdate).IsAssignableFrom(typeof(T))) {
				CodeExpression beginUpdate = ControlBuilderHelper.CreateBeginMethodInvokeStatement("BeginUpdate");
				CodeExpression endUpdate = ControlBuilderHelper.CreateEndMethodInvokeStatement("EndUpdate");
				if(beginUpdate != null && endUpdate != null) {
					ControlBuilderHelper.RebuildBuildMethod(buildMethod, beginUpdate, endUpdate);
				}
			}
		}
		public override void AppendLiteralString(string s) {
		}
		protected virtual void OnComponentBuiltSuccess(T component) {
			if(InDesigner) {
				if(string.IsNullOrEmpty(component.Name)) { 
					IDesignerHost host = GetDesignerHost();
					if(host != null) {
						host.Container.Add(component);	  
						if(component.Site != null) 
							component.Name = component.Site.Name;
						host.Container.Remove(component);   
					}
				}
			}
		}
		protected virtual bool AllowFilterProperties {
			get { return true; }
		}
		protected virtual void PreFilterAttributes(System.Collections.IDictionary attribs) {
			if(AllowFilterProperties) {
				IDictionary removed = FilterAttributesHelper.PrefilterAttributes(attribs, GetExpectedProperties());
				FilterAttributesHelper.ProcessAliases(attribs, Aliases, removed);
			}
		}
		protected virtual string[] GetExpectedProperties() {
			return new string[0];
		}
		protected virtual void BuildAliases(IDictionary<string, string> aliases) { }
		protected IDesignerHost GetDesignerHost() {
			return (ServiceProvider != null) ? ServiceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost : null;
		}
	}
	public sealed class FilterAttributesHelper {
		public static IDictionary PrefilterAttributes(IDictionary attributes, string[] allowedProps) {
			Hashtable propertiesToRemove = new Hashtable();
			foreach(DictionaryEntry entry in attributes) {
				bool found = false;
				foreach(string name in allowedProps) {
					if((string)entry.Key == name) {
						found = true;
						break;
					}
				}
				if(!found) propertiesToRemove.Add(entry.Key, entry.Value);
			}
			foreach(DictionaryEntry entry in propertiesToRemove) {
				attributes.Remove(entry.Key);
			}
			return propertiesToRemove;
		}
		public static void ProcessAliases(IDictionary attributes, IDictionary<string, string> aliases, IDictionary prefiltered) {
			foreach(KeyValuePair<string, string> alias in aliases) {
				if(prefiltered.Contains(alias.Key) && !attributes.Contains(alias.Value)) {
					attributes.Add(alias.Value, prefiltered[alias.Key]);
					prefiltered.Remove(alias.Key);
				}
			}
		}
	}
	sealed class ComponentBuilderInterceptorHelper : IDisposable {
		ISupportLockUpdate obj;
		BuildEntry entry;
		public ComponentBuilderInterceptorHelper() {
			entry = ComponentBuilderInterceptor.Instance.BeginBuild();
			entry.ObjectCreated += OnComponentCreated;
		}
		public void Dispose() {
			entry.ObjectCreated -= OnComponentCreated;
			if(obj != null) obj.EndUpdate();
			ComponentBuilderInterceptor.Instance.EndBuild();
			obj = null;
			entry = null;
		}
		void OnComponentCreated(object sender, EventArgs e) {
			obj = sender as ISupportLockUpdate;
			if(obj != null) obj.BeginUpdate();
		}
	}
	sealed class BuildEntry {
		public event EventHandler ObjectCreated;
		public void RaiseObjectCreated(object obj) {
			if(ObjectCreated != null) ObjectCreated(obj, EventArgs.Empty);
		}
	}
	sealed class ComponentBuilderInterceptor {
		[ThreadStatic]
		static ComponentBuilderInterceptor instanceCore;
		public static ComponentBuilderInterceptor Instance {
			get {
				if(instanceCore == null) instanceCore = new ComponentBuilderInterceptor();
				return instanceCore;
			}
		}
		Stack<BuildEntry> BuildStack;
		ComponentBuilderInterceptor() {
			BuildStack = new Stack<BuildEntry>();
		}
		public BuildEntry BeginBuild() {
			BuildEntry entry = new BuildEntry();
			BuildStack.Push(entry);
			return entry;
		}
		public void EndBuild() {
			if(BuildStack.Count > 0) BuildStack.Pop();
		}
		public void RaiseObjectCreated(object obj) {
			if(BuildStack.Count > 0) {
				BuildEntry entry = BuildStack.Peek();
				entry.RaiseObjectCreated(obj);
			}
		}
	}
}
