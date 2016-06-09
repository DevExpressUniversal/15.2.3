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
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Workflow.StartWorkflowConditions;
namespace DevExpress.ExpressApp.Workflow {
	public interface ISupportIsActive {
		bool IsActive { get; set; }
	}
	public interface IWorkflowDefinition {
		string GetUniqueId();
		string GetActivityTypeName();
		string Name { get; }
		string Xaml { get; }
		bool CanCompile { get; }
		bool CanCompileForDesigner { get; }
		bool CanOpenHost { get; }
		IList<IStartWorkflowCondition> GetConditions();
	}
	public interface IWorkflowDefinitionSettings : IWorkflowDefinition {
		new string Name { get; set; }
		new string Xaml { get; set; }
		bool AutoStartWhenObjectIsCreated { get; set; }
		bool AutoStartWhenObjectFitsCriteria { get; set; }
		bool AllowMultipleRuns { get; set; }
		Type TargetObjectType { get; set; }
		string Criteria { get; set; }
		bool IsActive { get; set; }
	}
	public interface IActivityXamlValidator {
		bool IsValidActivityXaml(string xaml);
	}
	public static class PersistentWorkflowDefinitionCore {
		public const string UniqueIdPrefix = "UserActivity_";
		public const int WorkflowDefinitionNameMaxLength = 255;
		public const string WorkflowDefinitionCriteriaAppearance = "CriteriaAppearance";
		public const string WorkflowDefinitionAllowMultipleRunsAppearance = "AllowMultipleRunsAppearance";
		public const string InitialXaml = @"<Activity mc:Ignorable=""sap"" x:Class=""DevExpress.Workflow.XafWorkflow"" 
    xmlns=""http://schemas.microsoft.com/netfx/2009/xaml/activities"" 
    xmlns:av=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""     
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""     
    xmlns:sap=""http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation"" 
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
	  <x:Members>
		<x:Property Name=""targetObjectId"" Type=""InArgument(x:Object)"" />
	  </x:Members>
</Activity>
";			   
		public static void AfterConstruction(IWorkflowDefinitionSettings definition) {
			definition.Xaml = InitialXaml;
		}
		public static bool Get_IsValidActivity(IWorkflowDefinitionSettings definition) {
			return ActivityXamlValidator.IsValidActivityXaml(definition.Xaml);
		}
		public static IList<IStartWorkflowCondition> GetConditions(IWorkflowDefinitionSettings definition) {
			List<IStartWorkflowCondition> result = new List<IStartWorkflowCondition>();
			if(definition.TargetObjectType != null && !string.IsNullOrEmpty(definition.GetUniqueId())) {
				if(definition.AutoStartWhenObjectIsCreated) {
					result.Add(new ObjectCreatedStartWorkflowCondition(definition.GetUniqueId(), definition.TargetObjectType));
				}
				if(definition.AutoStartWhenObjectFitsCriteria) {
					result.Add(new CriteriaStartWorkflowCondition(definition.GetUniqueId(), definition.TargetObjectType, definition.Criteria));
				}
			}
			return result;
		}
		public static bool Get_CanCompile(IWorkflowDefinitionSettings definition) {
			return !string.IsNullOrEmpty(definition.Name) && definition.IsActive;
		}
		public static bool Get_CanCompileForDesigner(IWorkflowDefinitionSettings definition) {
			return !string.IsNullOrEmpty(definition.Name);
		}
		public static bool Get_CanOpenHost(IWorkflowDefinitionSettings definition) {
			return !string.IsNullOrEmpty(definition.Name) && definition.IsActive;
		}
		public static string GetUniqueId(IWorkflowDefinitionSettings definition, IObjectSpace os) {
			if(os.IsNewObject(definition)) {
				throw new InvalidOperationException(); 
			}
			return UniqueIdPrefix + os.GetKeyValue(definition).ToString().ToUpper().Replace("-", "_");
		}
		public static object GetWorkflowDefinitionKeyByUniqueId(string uniqueId) {
			if(!uniqueId.StartsWith(UniqueIdPrefix)) {
				throw new ArgumentException(string.Format("{0} is wrong workflow UniqueId", uniqueId));
			}
			string keyString = uniqueId.Replace(UniqueIdPrefix, "").Replace("_", "-");
			try {
				return new Guid(keyString);
			}
			catch(Exception) {
				throw new ArgumentException(string.Format("{0} is wrong workflow UniqueId", uniqueId));
			}
		}
		public static IActivityXamlValidator ActivityXamlValidator { get; set; }
	}
}
