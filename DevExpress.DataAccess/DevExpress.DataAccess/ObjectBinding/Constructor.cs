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
using System.ComponentModel;
using System.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native.ObjectBinding;
namespace DevExpress.DataAccess.ObjectBinding {
	public class ObjectConstructorInfo {
		public class EqualityComparer : IEqualityComparer<ObjectConstructorInfo> {
			public static bool Equals(ObjectConstructorInfo x, ObjectConstructorInfo y) {
				if(ReferenceEquals(x, y))
					return true;
				if(x == null || y == null)
					return false;
				if(x.Parameters.Count != y.Parameters.Count)
					return false;
				IEnumerator<Parameter> xEnumerator = x.Parameters.GetEnumerator();
				IEnumerator<Parameter> yEnumerator = y.Parameters.GetEnumerator();
				while(xEnumerator.MoveNext() && yEnumerator.MoveNext())
					if(!Parameter.EqualityComparer.Equals(xEnumerator.Current, yEnumerator.Current))
						return false;
				return true;
			}
			#region Implementation of IEqualityComparer<in ObjectConstructorInfo>
			bool IEqualityComparer<ObjectConstructorInfo>.Equals(ObjectConstructorInfo x, ObjectConstructorInfo y) { return Equals(x, y); }
			int IEqualityComparer<ObjectConstructorInfo>.GetHashCode(ObjectConstructorInfo obj) { return 0; }
			#endregion
		}
		static readonly ObjectConstructorInfo @default = new ObjectConstructorInfo();
		public static ObjectConstructorInfo Default { get { return @default; } }
		public ObjectConstructorInfo() { Parameters = new ParameterList(); }
		public ObjectConstructorInfo(IEnumerable<Parameter> parameters) { Parameters = new ParameterList(parameters); }
		public ObjectConstructorInfo(params Parameter[] parameters) : this(parameters.AsEnumerable()) { }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ParameterList Parameters { get; private set; }
		#region Overrides of Object
		public override string ToString() {
			return Parameters.Any()
				? string.Format("({0})",
					string.Join(", ",
						Parameters.Select(
							parameter =>
								parameter.Value == null
									? string.Format("{0} {1}", TypeNamesHelper.ShortName(parameter.Type), parameter.Name)
									: string.Format("{0} {1} = {2}",
										TypeNamesHelper.ShortName(TargetType(parameter.Type, parameter.Value as Expression)), parameter.Name,
										parameter.Value))))
				: DataAccessLocalizer.GetString(DataAccessStringId.ParameterlessConstructor);
		}
		static Type TargetType(Type type, Expression expr) {
			if(type != typeof(Expression) || expr == null)
				return type;
			return expr.ResultType ?? type;
		}
		#endregion
	}
}
