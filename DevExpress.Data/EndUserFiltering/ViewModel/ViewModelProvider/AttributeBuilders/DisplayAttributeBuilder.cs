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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.Reflection;
	using System.Reflection.Emit;
	static class DisplayAttributeBuilder {
		readonly static ConstructorInfo attributeCtor;
		readonly static PropertyInfo[] attributeProperties;
		static DisplayAttributeBuilder() {
			try {
				DataAnnotationAttributeBuilderHelper.CheckDataAnnotations_ConditionallyAPTCAIssue<DisplayAttribute>();
				var attributeType = typeof(DisplayAttribute);
				attributeCtor = attributeType.GetConstructor(Type.EmptyTypes);
				attributeProperties = new PropertyInfo[] {
					attributeType.GetProperty("Name"),
					attributeType.GetProperty("ShortName"),
					attributeType.GetProperty("Description"),
					attributeType.GetProperty("GroupName"),
					attributeType.GetProperty("Order")
				};
			}
			catch(MethodAccessException) {
				attributeCtor = null;
				attributeProperties = null;
			}
		}
		internal static CustomAttributeBuilder Build(string name) {
			return DataAnnotationAttributeBuilderHelper.Build(
				attributeCtor, attributeProperties,
				new object[] { name });
		}
		internal static CustomAttributeBuilder Build(IEndUserFilteringMetric metric, bool calculateShortName = false) {
			string shortName = GetShortName(metric, calculateShortName);
			if(calculateShortName && shortName == null) {
				var booleanAttributes = metric.Attributes as IBooleanChoiceMetricAttributes;
				if(booleanAttributes != null) {
					if(booleanAttributes.EditorType != BooleanUIEditorType.DropDown && booleanAttributes.EditorType != BooleanUIEditorType.List)
						shortName = string.Empty;
				}
			}
			return DataAnnotationAttributeBuilderHelper.Build(
				attributeCtor, attributeProperties,
				new object[] { 
					metric.Caption, 
					shortName,
					metric.Description, 
					metric.Layout, 
					metric.Order 
				});
		}
		static string GetShortName(IEndUserFilteringMetric metric, bool calculateShortName) {
			if(!calculateShortName)
				return null;
			return GetService<IMetadataProvider>(metric as IServiceProvider)
					.@Get(metadata => metadata.GetShortName(metric.Path));
		}
		static TService GetService<TService>(IServiceProvider serviceProvider)
			where TService : class {
			return serviceProvider.@Get(sp => sp.GetService(typeof(TService)) as TService);
		}
	}
}
