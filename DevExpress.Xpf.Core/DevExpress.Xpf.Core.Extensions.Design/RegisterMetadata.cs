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

#if SILVERLIGHT
extern alias Platform;
#endif
using System;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Features;
#if SILVERLIGHT
using AssemblyInfo = Platform::AssemblyInfo;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
using System.ComponentModel;
using DevExpress.Xpf.Design;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.Core.ServerMode;
using System.Collections.Generic;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design.Interaction;
using System.ServiceModel.DomainServices.Client;
#else
using DevExpress.Xpf.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
#endif
namespace DevExpress.Xpf.Core.Extensions.Design {
#if !SL
	internal class RegisterMetadata : MetadataProviderBase {
		protected override Assembly RuntimeAssembly { get { return typeof(Class1).Assembly; } }
		protected override string ToolboxCategoryPath { get { return AssemblyInfo.DXTabNameData; } }
	}
#else
	internal class RegisterMetadata : IProvideAttributeTable {
		void RegisterControls(AttributeTableBuilder builder) {
			builder.HideControls(typeof(Platform::DevExpress.Xpf.Core.MvvmSample.Helpers.ViewPresenter));
			builder.HideControls(typeof(Platform::DevExpress.Xpf.Core.MvvmSample.ModuleView));
			builder.HideControls(typeof(Platform::DevExpress.Xpf.Core.MvvmSample.MvvmRoot));
		}
		void RegisterRiaInstantFeedbackDataSourceAttributes(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(RiaInstantFeedbackDataSource),
				DesignHelper.GetPropertyName(RiaInstantFeedbackDataSource.DomainContextProperty),
				new TypeConverterAttribute(typeof(DomainContextConverter)));
			builder.AddCustomAttributes(typeof(RiaInstantFeedbackDataSource),
				DesignHelper.GetPropertyName(RiaInstantFeedbackDataSource.QueryNameProperty),
				new TypeConverterAttribute(typeof(QueryNameConverter)));
		}
		public AttributeTable AttributeTable {
			get {
				var builder = new AttributeTableBuilder();
				RegisterControls(builder);
				RegisterRiaInstantFeedbackDataSourceAttributes(builder);
				return builder.CreateTable();
			}
		}
	}
	public class DomainContextConverter : InheritedTypeTypeConverter {
		public override Type BaseType { get { return typeof(DomainContext); } }
	}
	public class QueryNameConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<string> result = new List<string>();
			ModelService service = context.GetService(typeof(ModelService)) as ModelService;
			Selection selection = service.Root.Context.Items.GetValue<Selection>();
			RiaInstantFeedbackDataSource source = selection.PrimarySelection.View.PlatformObject as RiaInstantFeedbackDataSource;
			if(source != null && source.DomainContext != null) {
				Type type = source.DomainContext.GetType();
				MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
				foreach(MethodInfo mi in methods) {
					if(mi.Name.StartsWith("Get") && mi.Name.EndsWith("Query") && mi.ReturnType.Name.Contains("EntityQuery"))
						result.Add(mi.Name);
				}
			}
			return new TypeConverter.StandardValuesCollection(result);
		}
	}
#endif
}
