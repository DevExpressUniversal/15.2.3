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
using System.Reflection;
using System.ServiceModel;
using System.Xaml;
using DevExpress.Design.SmartTags;
using DevExpress.MarkupUtils.Design;
using DevExpress.Utils.Design;
using EnvDTE;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using Window = System.Windows.Window;
using DevExpress.Xpf.Core.Design.Utils;
namespace DevExpress.Xpf.Core.Design {
	internal class ConvertBaseClassActionProvider : CommandActionLineProvider {
		public Type TargetType { get; private set; }
		public Type CurrentType { get; private set; }
		public ConvertBaseClassActionProvider(IPropertyLineContext context, Type targetType)
			: base(context) {
			TargetType = targetType;
			UpdateCurrentType();
			this.CanExecuteAction = CanExecute;
		}
		void UpdateCurrentType() {
			CurrentType = Context == null || Context.ModelItem == null ? null : Context.ModelItem.ItemType;
		}
		bool CanExecute(object arg) {
			return Context != null && Context.ModelItem != null && Context.ModelItem.ItemType != TargetType;
		}
		protected override string GetCommandText() {
			return string.Format("Convert to {0}", TargetType.Name);
		}
		protected override void OnCommandExecute(object param) {
			ModelItem item = XpfModelItem.ToModelItem(Context.ModelItem);
			ConvertBaseClass(item, TargetType);
		}
		void ConvertBaseClass(ModelItem item, Type targetType) {
			CurrentType = item.ItemType;
			try {
				DTE dte = DTEHelper.GetCurrentDTE();
				Document document = dte.ActiveDocument;
				ProjectItem projectItem = document.ProjectItem;
				ProjectReferencesHelper.EnsureAssemblyReferenced(XpfModelItem.FromModelItem(item), targetType.Assembly.GetName().Name, false);
				FormTypeConverter.ToType(projectItem, targetType.FullName);
				string markup = ConvertRootElementInMarkup(item.Root, targetType);
				markup = DesignDteHelper.FormatMarkup(markup) ?? markup;
				EnvDTE.TextDocument textDoc = document.Object(string.Empty) as TextDocument;
				if(textDoc == null)
					return;
				textDoc.Selection.SelectAll();
				textDoc.Selection.Insert(markup);
			} catch(Exception e) {
#if DEBUG 
				System.Windows.MessageBox.Show(e.Message);
#endif
				CanExecuteAction = (param) => { return false; };
			}
		}
		string ConvertRootElementInMarkup(ModelItem sourceRoot, Type targetType) {
			string resultMarkup = string.Empty;
			using(var scope = sourceRoot.BeginEdit(GetCommandText())) {
				ClearUnsupportedPropertiesAndEvents(sourceRoot, targetType);
				resultMarkup = XamlMarkupWindowConverter.ConvertRootElement(sourceRoot, sourceRoot.ItemType, targetType);
				scope.Revert();
			}
			return resultMarkup;
		}
		void ClearUnsupportedPropertiesAndEvents(ModelItem sourceRoot, Type targetType) {
			using(ModelEditingScope scope = sourceRoot.BeginEdit()) {
				foreach(ModelProperty property in sourceRoot.Properties) {
					if(property.IsSet && targetType.GetProperty(property.Name) == null)
						property.ClearValue();
				}
				foreach(ModelEvent modelEvent in sourceRoot.Events) {
					if(modelEvent.Handlers.Count > 0 && targetType.GetEvent(modelEvent.Name) == null)
						modelEvent.Handlers.Clear();
				}
				scope.Complete();
			}
		}
	}
	internal static class XamlMarkupWindowConverter {
		public static string GetXamlMarkup(ModelItem source) {
			EditingContext context = source.Context;
			ExternalMarkupService markupService = context.Services.GetService<ExternalMarkupService>();
			IEnumerable<AssemblyName> assemblies = null;
			return markupService.Save(source, out assemblies);
		}
		public static string ConvertRootElement(ModelItem source, Type sourceType, Type targetType) {
			string xamlMarkup = GetXamlMarkup(source);
			return ConvertRootElement(xamlMarkup, sourceType, targetType);
		}
		public static string ConvertRootElement(string xamlMarkup, Type sourceType, Type targetType) {
			string resultMarkup = xamlMarkup;
			XamlSchemaContext schemaContext = System.Windows.Markup.XamlReader.GetWpfSchemaContext();
			string sourceRootDeclaration = GetTypeDeclaration(new XamlType(sourceType, schemaContext));
			string targetRootDeclaration = GetTypeDeclaration(new XamlType(targetType, schemaContext));
			string prefixDeclaration = GetPrefixDeclaration(schemaContext, targetType);
			if(!resultMarkup.Contains(prefixDeclaration))
				resultMarkup = resultMarkup.Insert(resultMarkup.IndexOf("xmlns"), prefixDeclaration + " ");
			resultMarkup = resultMarkup.Replace("<" + sourceRootDeclaration, "<" + targetRootDeclaration);
			resultMarkup = resultMarkup.Replace("</" + sourceRootDeclaration, "</" + targetRootDeclaration);
			return resultMarkup;
		}
		public static string GetPrefixDeclaration(XamlSchemaContext schemaContext, Type type) {
			return GetPrefixDeclaration(new XamlType(type, schemaContext));
		}
		public static string GetPrefixDeclaration(XamlType xamlType) {
			XamlSchemaContext schemaContext = xamlType.SchemaContext;
			if(xamlType.UnderlyingType == typeof(Window))
				return string.Format("xmlns=\"{0}\"", xamlType.PreferredXamlNamespace);
			string prefix = schemaContext.GetPreferredPrefix(xamlType.PreferredXamlNamespace);
			return string.Format("xmlns:{0}=\"{1}\"", prefix, xamlType.PreferredXamlNamespace);
		}
		public static string GetTypeDeclaration(XamlSchemaContext schemaContext, Type type) {
			return GetTypeDeclaration(new XamlType(type, schemaContext));
		}
		public static string GetTypeDeclaration(XamlType xamlType) {
			XamlSchemaContext schemaContext = xamlType.SchemaContext;
			if(xamlType.UnderlyingType == typeof(Window))
				return xamlType.Name;
			string sourceRootPrefix = schemaContext.GetPreferredPrefix(xamlType.PreferredXamlNamespace);
			return string.IsNullOrEmpty(sourceRootPrefix) ? xamlType.Name : string.Format("{0}:{1}", sourceRootPrefix, xamlType.Name);
		}
	}
}
