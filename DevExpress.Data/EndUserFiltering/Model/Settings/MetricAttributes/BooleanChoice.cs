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
	using System.Collections.Generic;
	using System.Linq.Expressions;
	partial class MetricAttributes {
		internal static bool IsBooleanChoice(Type type) {
			if(TypeHelper.IsNullable(type))
				return IsBooleanChoice(Nullable.GetUnderlyingType(type));
			return type == typeof(bool);
		}
		static IDictionary<Type, Func<bool?, string, string, string, BooleanUIEditorType, string[], IMetricAttributes>> booleanChoiceInitializers =
			new Dictionary<Type, Func<bool?, string, string, string, BooleanUIEditorType, string[], IMetricAttributes>>();
		internal static IMetricAttributes CreateBooleanChoice(Type type, bool? defaultValue, string trueName, string falseName, string defaultName, BooleanUIEditorType editorType, string[] members) {
			if(!IsBooleanChoice(type))
				return null;
			Func<bool?, string, string, string, BooleanUIEditorType, string[], IMetricAttributes> initializer;
			if(!booleanChoiceInitializers.TryGetValue(type, out initializer)) {
				var aType = typeof(BooleanChoiceMetricAttributes);
				var pDefaultValue = Expression.Parameter(typeof(bool?), "defaultValue");
				var pTrueName = Expression.Parameter(typeof(string), "trueName");
				var pFalseName = Expression.Parameter(typeof(string), "falseName");
				var pDefaultName = Expression.Parameter(typeof(string), "defaultName");
				var pEditorType = Expression.Parameter(typeof(BooleanUIEditorType), "editorType");
				var pMembers = Expression.Parameter(typeof(string[]), "members");
				var ctorExpression = Expression.New(
							aType.GetConstructor(new Type[] { typeof(bool?), typeof(string), typeof(string), typeof(string), typeof(BooleanUIEditorType), typeof(string[]) }),
							pDefaultValue,
							pTrueName,
							pFalseName,
							pDefaultName,
							pEditorType,
							pMembers);
				initializer = Expression.Lambda<Func<bool?, string, string, string, BooleanUIEditorType, string[], IMetricAttributes>>(
					ctorExpression, pDefaultValue, pTrueName, pFalseName, pDefaultName, pEditorType, pMembers).Compile();
				booleanChoiceInitializers.Add(type, initializer);
			}
			return initializer(defaultValue, trueName, falseName, defaultName, editorType, members);
		}
		class BooleanChoiceMetricAttributes : MetricAttributes, IBooleanChoiceMetricAttributes {
			readonly BooleanUIEditorType editorType;
			readonly string trueName, falseName, defaultName;
			readonly MemberNullableValueBox<bool> defaultValue;
			public BooleanChoiceMetricAttributes(bool? defaultValue, string trueName, string falseName, string defaultName, BooleanUIEditorType editorType, string[] members) :
				base(members) {
				this.defaultValue = new MemberNullableValueBox<bool>(defaultValue, 0, this, () => DefaultValue);
				this.trueName = trueName;
				this.falseName = falseName;
				this.defaultName = defaultName;
				this.editorType = editorType;
			}
			public BooleanUIEditorType EditorType {
				get { return editorType; }
			}
			public bool? DefaultValue {
				get { return defaultValue.Value; }
			}
			public string TrueName {
				get { return trueName ?? FilteringLocalizer.GetString(FilteringLocalizerStringId.TrueName); }
			}
			public string FalseName {
				get { return falseName ?? FilteringLocalizer.GetString(FilteringLocalizerStringId.FalseName); }
			}
			public string DefaultName {
				get { return defaultName ?? FilteringLocalizer.GetString(FilteringLocalizerStringId.DefaultName); }
			}
		}
	}
}
