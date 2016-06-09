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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Wizard.Model {
	public class ObjectDataSourceModel : DataSourceModelBase, IObjectDataSourceModel {
		#region static
		internal static bool ParametersEquals(ObjectBinding.Parameter[] x, ObjectBinding.Parameter[] y) {
			if(x == null)
				return y == null;
			if(y == null)
				return false;
			int n = x.Length;
			if(y.Length != n)
				return false;
			for(int i = 0; i < n; i++)
				if(!ObjectBinding.Parameter.EqualityComparer.Equals(x[i], y[i]))
					return false;
			return true;
		}
		#endregion
		public ObjectDataSourceModel() { }
		public ObjectDataSourceModel(ObjectDataSourceModel other)
			: base(other) {
			Assembly = other.Assembly;
			ObjectType = other.ObjectType;
			ObjectMember = other.ObjectMember;
			MemberParameters = other.MemberParameters == null
				? null
				: other.MemberParameters.Select(p => new ObjectBinding.Parameter(p)).ToArray();
			ObjectConstructor = other.ObjectConstructor;
			CtorParameters = other.CtorParameters == null
				? null
				: other.CtorParameters.Select(p => new ObjectBinding.Parameter(p)).ToArray();
			ShowAllState = other.ShowAllState;
		}
		public ObjectDataSourceModel(ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider, IWaitFormActivator waitFormActivator, IExceptionHandler exceptionHandler) {
			MemberParameters = ods.Parameters.Select(p => new ObjectBinding.Parameter(p)).ToArray();
			CtorParameters = ods.Constructor == null ? null : ods.Constructor.Parameters.Select(p => new ObjectBinding.Parameter(p)).ToArray();
			DataSchema = ods.Result;
			Type type;
			if(ods.DataSource == null)
				type = null;
			else {
				type = ods.DataSource as Type;
				if(type == null) {
					type = ods.DataSource.GetType();
					if(typeof(IEnumerable).IsAssignableFrom(type))
						type = ObjectDataSourceFillHelper.GetItemType(type);
				}
			}
			if(type != null) {
				if(ods.DataMember != null)
					try {
						ObjectMember =
							ObjectDataSourceFillHelper.FindMember(
								ObjectDataSourceFillHelper.CreateInstanceForSchema(ods.DataSource), ods.DataMember,
								ObjectDataSourceFillHelper.EvaluateParametersForSchema(ods.Parameters));
					}
					catch(DataMemberResolveException) {
						ObjectMember = null;
					}
				if(ObjectMember == null || !ObjectMember.IsStatic)
					try {
						ObjectConstructor = ods.Constructor == null ? null : ObjectDataSourceFillHelper.FindConstructor(type,
							ObjectDataSourceFillHelper.EvaluateParametersForSchema(ods.Constructor.Parameters));
					}
					catch(ConstructorResolutionException e) {
						ObjectConstructor = null;
						if(e.CtorParameters == null || e.CtorParameters.Count != 0)
							Debug.Fail(e.Message);
					}
				if(waitFormActivator != null) {
					waitFormActivator.ShowWaitForm(true, false, true);
					waitFormActivator.SetWaitFormDescription(DataAccessLocalizer.GetString(DataAccessStringId.GatheringTypesPanelText));
				}
				Exception exception = null;
				try {
					string assemblyName = type.Assembly.GetName().FullName;
					Assembly = solutionTypesProvider.GetAssembly(assemblyName);
					if(Assembly != null)
						ObjectType = Assembly.GetTypeInfo(type.FullName);
					else
						Debug.Fail(string.Format("Cannot find the {0} assembly containing the following type: {1}.", assemblyName, type.FullName));
				}
				catch(Exception ex) {
					exception = new CannotGetTypesException(ex);
				}
				finally {
					if(waitFormActivator != null)
						waitFormActivator.CloseWaitForm();
				}
				if(exception != null) {
					if(exceptionHandler == null)
						throw exception;
					exceptionHandler.HandleException(exception);
				}
			}
		}
		#region Implementation of IDataComponentModel
		public IDataConnection DataConnection {
			get { return null; }
			set { throw new NotSupportedException(); }
		}
		#endregion
		#region Implementation of IObjectDataSourceModel
		public IDXAssemblyInfo Assembly { get; set; }
		public IDXTypeInfo ObjectType { get; set; }
		public ObjectMember ObjectMember { get; set; }
		public ObjectBinding.Parameter[] MemberParameters { get; set; }
		public ConstructorInfo ObjectConstructor { get; set; }
		public ObjectBinding.Parameter[] CtorParameters { get; set; }
		public ShowAllState ShowAllState { get; set; }
		#endregion
		#region Overrides of DataSourceModelBase
		public override object Clone() { return new ObjectDataSourceModel(this); }
		public override bool Equals(object obj) {
			ObjectDataSourceModel other = obj as ObjectDataSourceModel;
			if(other == null)
				return false;
			return base.Equals(obj) 
				   && ReferenceEquals(Assembly, other.Assembly)
				   && ReferenceEquals(ObjectType, other.ObjectType)
				   && ReferenceEquals(ObjectMember, other.ObjectMember)
				   && ParametersEquals(MemberParameters, other.MemberParameters)
				   && ReferenceEquals(ObjectConstructor, other.ObjectConstructor)
				   && ParametersEquals(CtorParameters, other.CtorParameters);
		}
		public override int GetHashCode() {
			return 0;
		}
		#endregion
	}
	public class CannotGetTypesException : Exception {
		public CannotGetTypesException(Exception innerException)
			: base(string.Format("No suitable object was found in this project to be used as a data source.{0}Before running the wizard again, please make sure that the project contains an appropriate data source class, close the designer and rebuild the solution.", Environment.NewLine), innerException) { }
	}
}
