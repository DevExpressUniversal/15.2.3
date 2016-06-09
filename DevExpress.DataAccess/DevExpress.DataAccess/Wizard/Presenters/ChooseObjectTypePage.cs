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
using System.Linq;
using System.Text;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ChooseObjectTypePage<TModel> : WizardPageBase<IChooseObjectTypePageView, TModel>
		where TModel : IObjectDataSourceModel {
		readonly IWizardRunnerContext context;
		readonly OperationMode operationMode;
		IEnumerable<IDXTypeInfo> dxTypeInfos;
		public ChooseObjectTypePage(IChooseObjectTypePageView view, IWizardRunnerContext context, OperationMode operationMode) : base(view) {
			this.context = context;
			this.operationMode = operationMode;
		}
		#region Overrides of WizardPageBase<IChooseObjectTypePageView,TModel>
		public override bool MoveNextEnabled { get { return View.SelectedItem != null; } }
		public override bool FinishEnabled { get { return View.SelectedItem != null && GetNextPageType() == null; } }
		public override Type GetNextPageType() {
			IDXTypeInfo result = GetResult();
			if(result == null)
				return null;
			Type objType = result.ResolveType();
			return ObjectBindingPagesRouter.AfterChooseObjectTypePage<TModel>(objType, operationMode);
		}
		public override void Begin() {
			dxTypeInfos = Model.Assembly.TypesInfo.Where(ti => TypeIsVisible(ti.ResolveType(), Model.Assembly.IsProjectAssembly));
			View.Changed += ViewOnChanged;
			TypeViewInfo selected;
			View.Initialize(GatherViewInfos(out selected), Model.ShowAllState.HasFlag(ShowAllState.Classes));
			View.SelectedItem = selected;
		}
		static bool TypeIsVisible(Type type, bool projectAssembly) {
			if(type.IsPublic)
				return true;
			if(type.IsNestedPublic)
				return TypeIsVisible(type.DeclaringType, projectAssembly);
			if(!projectAssembly)
				return false;
			if(!type.IsNested)
				return true;
			if(type.IsNestedAssembly || type.IsNestedFamORAssem)
				return TypeIsVisible(type.DeclaringType, true);
			return false;
		}
		List<TypeViewInfo> GatherViewInfos(out TypeViewInfo selected) {
			List<TypeViewInfo> result = new List<TypeViewInfo>();
			selected = null;
			List<IDXTypeInfo> nested = new List<IDXTypeInfo>();
			List<IDXTypeInfo> notNested = new List<IDXTypeInfo>(dxTypeInfos.Count());
			foreach(IDXTypeInfo info in dxTypeInfos) {
				Type type = info.ResolveType();
				if(type.IsGenericTypeDefinition)
					continue;
				if(type.IsEnum)
					continue;
				(type.IsNested ? nested : notNested).Add(info);
			}
			foreach(IDXTypeInfo info in notNested)
				result.Add(CreateTypeViewInfo(info, ref selected, nested));
			return result;
		}
		TypeViewInfo CreateTypeViewInfo(IDXTypeInfo info, ref TypeViewInfo selected, List<IDXTypeInfo> nestedTypes) {
			Type type = info.ResolveType();
			return CreateTypeViewInfo(info, ref selected, nestedTypes, type);
		}
		TypeViewInfo CreateTypeViewInfo(IDXTypeInfo info, ref TypeViewInfo selected, List<IDXTypeInfo> nestedTypes, Type type) {
			bool highlighted = Highlighted(type);
			var classType = ClassType(type);
			IEnumerable<TypeViewInfo> nested = Nested(type, nestedTypes, ref selected);
			TypeViewInfo item = new TypeViewInfo(highlighted, info.NamespaceName, info.Name, classType, nested);
			if(Model.ObjectType != null && string.Equals(info.FullName, Model.ObjectType.FullName))
				selected = item;
			return item;
		}
		static bool Highlighted(Type type) {
			return type.GetCustomAttributes(typeof(HighlightedClassAttribute), false).Length > 0;
		}
		static TypeViewInfo.NodeType ClassType(Type type) {
			TypeViewInfo.NodeType classType = TypeViewInfo.NodeType.Class;
			if(type.IsInterface)
				classType = TypeViewInfo.NodeType.Interface;
			else if(type.IsAbstract)
				classType = type.IsSealed ? TypeViewInfo.NodeType.StaticClass : TypeViewInfo.NodeType.AbstractClass;
			return classType;
		}
		IEnumerable<TypeViewInfo> Nested(Type type, List<IDXTypeInfo> nestedTypes, ref TypeViewInfo selected) {
			List<TypeViewInfo> result = new List<TypeViewInfo>();
			foreach(IDXTypeInfo info in nestedTypes) {
				Type itemType = info.ResolveType();
				if(itemType.DeclaringType != type)
					continue;
				result.Add(CreateTypeViewInfo(info, ref selected, nestedTypes, itemType));
			}
			return result;
		}
		void ViewOnChanged(object sender, EventArgs eventArgs) { RaiseChanged(); }
		public override bool Validate(out string errorMessage) {
			IDXTypeInfo result = GetResult();
			if(result == null) {
				errorMessage = null;
				return false;
			}
			Type type = result.ResolveType();
			errorMessage = ObjectDataSourceFillHelper.ValidateResultType(type);
			if(errorMessage != null) {
				ShowMessage(errorMessage);
				return false;
			}
			object instance = ObjectDataSourceFillHelper.CreateInstanceForSchema(type);
			Model.DataSchema = ObjectDataSourceFillHelper.CreateTypedList(instance, type.Name);
			return true;
		}
		public override void Commit() {
			View.Changed -= ViewOnChanged;
			IDXTypeInfo result = GetResult();
			if(Model.ObjectType != null && result != null && !string.Equals(Model.ObjectType.FullName, result.FullName))
			{
				Model.ObjectMember = null;
				Model.ObjectConstructor = null;
			}
			Model.ObjectType = result;
			if(View.ShowAll)
				Model.ShowAllState |= ShowAllState.Classes;
			else
				Model.ShowAllState &= ~ShowAllState.Classes;
		}
		#endregion
		protected IDXTypeInfo GetResult() {
			if(dxTypeInfos == null)
				return null;
			TypeViewInfo viewResult = View.SelectedItem;
			if(viewResult == null)
				return null;
			StringBuilder sb = new StringBuilder(viewResult.Namespace);
			sb.Append('.');
			if(viewResult.Parent != null) {
				int pos = sb.Length;
				TypeViewInfo node = viewResult.Parent;
				sb.Append(node.TypeName);
				while(node.Parent != null) {
					node = node.Parent;
					sb.Insert(pos, '+');
					sb.Insert(pos, node.TypeName);
				}
				sb.Append('+');
			}
			sb.Append(viewResult.TypeName);
			string fullName = sb.ToString();
			return dxTypeInfos.FirstOrDefault(info => string.Equals(info.FullName, fullName));
		}
		protected virtual void ShowMessage(string message) { context.ShowMessage(message); }
	}
}
