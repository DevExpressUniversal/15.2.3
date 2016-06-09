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
using System.Data.Services.Providers;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.Xpo.Metadata;
using System.Collections.ObjectModel;
using DevExpress.Xpo.Helpers;
using ExpressionVisitor = DevExpress.Xpo.Helpers.ExpressionVisitor;
namespace DevExpress.Xpo {
	internal class MethodTranslatingVisitor : ExpressionVisitor {
		readonly XpoLinqQueryProvider provider;
		readonly ResourceType resourceType;
		readonly Session session;
		readonly Dictionary<Type, ParameterExpression> parameterDict = new Dictionary<Type, ParameterExpression>();
		internal readonly Stack<ParameterExpression> ElementParameterStack = new Stack<ParameterExpression>();
		internal readonly Stack<Type> TypeParameterStack = new Stack<Type>();
		internal XpoMetadata Metadata { get { return this.provider.DataService.Metadata; } }
		char parameterName = 'a';
		public MethodTranslatingVisitor(XpoLinqQueryProvider provider, ResourceType resourceType, Session session) {
			this.provider = provider;
			this.resourceType = resourceType;
			this.session = session;
			ElementParameterStack.Push(GetParameter(resourceType.GetAnnotation().ClassInfo.ClassType));
		}
		ParameterExpression GetParameter(Type type) {
			ParameterExpression result;
			if(!parameterDict.TryGetValue(type, out result)) {
				parameterName = (char)((int)parameterName + 1);
				result = Expression.Parameter(type, new string(parameterName, 1));
			}
			return result;
		}
		internal static readonly MethodInfo GetValueMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
			"GetValue",
			BindingFlags.Static | BindingFlags.Public,
			null,
			new Type[] { typeof(object), typeof(ResourceProperty) },
			null);
		internal static readonly MethodInfo GetSequenceValueMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
			"GetSequenceValue",
			BindingFlags.Static | BindingFlags.Public,
			null,
			new Type[] { typeof(object), typeof(ResourceProperty) },
			null);
		internal static readonly MethodInfo ConvertMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
			"Convert",
			BindingFlags.Static | BindingFlags.Public);
		internal static readonly MethodInfo TypeIsMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
			"TypeIs",
			BindingFlags.Static | BindingFlags.Public);
		public static Expression TranslateExpression(XpoLinqQueryProvider provider, ResourceType resource, Expression expression, Session session) {
			MethodTranslatingVisitor visitor = new MethodTranslatingVisitor(provider, resource, session);
			return visitor.Visit(expression);
		}
		internal override Expression VisitParameter(ParameterExpression p) {
			if(TypeSystem.IsEntity(p.Type, Metadata)) {
				if(TypeParameterStack.Count > 0 && TypeParameterStack.Peek() != ElementParameterStack.Peek().Type)
					return Expression.Convert(ElementParameterStack.Peek(), TypeParameterStack.Peek());
				if(ElementParameterStack.Peek().Type == p.Type)
					return ElementParameterStack.Peek();
				else
					return Expression.Convert(ElementParameterStack.Peek(), p.Type);
			}
			return base.VisitParameter(p);
		}
		internal override Expression VisitConstant(ConstantExpression c) {
			if(c.Type.IsGenericType) {
				IQueryable query = c.Value as IQueryable;
				if(query != null && TypeSystem.IsEntity(query.ElementType, Metadata) && query.Provider as XpoLinqQueryProvider != null) {
					XpoLinqQueryProvider provider = query.Provider as XpoLinqQueryProvider;
					Type elementType = provider.ResourceType.GetAnnotation().ClassInfo.ClassType;
					IQueryable xpQuery = (IQueryable)provider.DataService.Context.GetXPQueryCreator(elementType).CreateXPQuery(session);
					return Expression.Constant(xpQuery);
				} else {
					Type[] genTypes = c.Type.GetGenericArguments();
					bool changed = false;
					for(int i = 0; i < genTypes.Length; i++) {
						if(TypeSystem.IsEntity(genTypes[i], Metadata)) {
							if(TypeParameterStack.Count > 0)
								genTypes[i] = TypeParameterStack.Peek();
							else
								genTypes[i] = ElementParameterStack.Peek().Type;
							changed = true;
						}
					}
					Type genericType = c.Type;
					if(changed) {
						if(c.Type.IsGenericTypeDefinition)
							genericType = c.Type;
						else {
							genericType = c.Type.GetGenericTypeDefinition();
							genericType = genericType.MakeGenericType(genTypes);
						}
					}
					return Expression.Constant(c.Value, genericType);
				}
			} else {
				if(TypeSystem.IsEntity(c.Type, Metadata))
					if(TypeParameterStack.Count > 0 && TypeParameterStack.Peek() != ElementParameterStack.Peek().Type)
						return Expression.Convert(Expression.Constant(c.Value, ElementParameterStack.Peek().Type), TypeParameterStack.Peek());
			}
			if(c.Type == typeof(object))
				return Expression.Constant(c.Value, ElementParameterStack.Peek().Type);
			else
				return c;
		}
		static readonly object boxedZero = (int)0;
		internal override Expression VisitBinary(BinaryExpression b) {
			Expression left = this.Visit(b.Left);
			Expression right = this.Visit(b.Right);
			if(left.NodeType == ExpressionType.Call && right.NodeType == ExpressionType.Constant && left.Type == typeof(int) && right.Type == typeof(int)) {
				MethodCallExpression leftCall = (MethodCallExpression)left;
				ConstantExpression rightConst = (ConstantExpression)right;
				if(leftCall.Method.DeclaringType == typeof(DataServiceProviderMethods) && leftCall.Method.Name == "Compare" && object.Equals(rightConst.Value, boxedZero)) {
					if(leftCall.Arguments[0].Type == typeof(string)) {
						return Expression.MakeBinary(b.NodeType, Expression.Call(typeof(string), "Compare", new Type[0], leftCall.Arguments[0], leftCall.Arguments[1]), rightConst);
					} else
						return Expression.MakeBinary(b.NodeType, leftCall.Arguments[0], leftCall.Arguments[1]);
				}
			}
			if(left.Type != right.Type && left.NodeType == ExpressionType.Constant) {
				left = Expression.Constant(((ConstantExpression)left).Value, right.Type);
			}
			if(right.Type != left.Type && right.NodeType == ExpressionType.Constant) {
				right = Expression.Constant(((ConstantExpression)right).Value, left.Type);
			}
			Expression conversion = this.Visit(b.Conversion);
			if(left != b.Left || right != b.Right || conversion != b.Conversion) {
				if(b.NodeType == ExpressionType.Coalesce && b.Conversion != null) {
					return Expression.Coalesce(left, right, conversion as LambdaExpression);
				} else {
					return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
				}
			}
			return b;
		}
		internal override Expression VisitUnary(UnaryExpression u) {
			Expression operand = this.Visit(u.Operand);
			if(operand != u.Operand) {
				if(u.NodeType == ExpressionType.Convert || u.NodeType == ExpressionType.TypeAs) {
					if(TypeSystem.IsEntity(u.Type, Metadata) || TypeSystem.IsStruct(operand.Type, Metadata)) {
						return operand;
					}
					if(u.Type == operand.Type || u.Type != typeof(object) && u.Type.IsAssignableFrom(operand.Type) && operand.Type.IsGenericType)
						return operand;
				}
				MemberExpression memberExpr = operand as MemberExpression;
				ResourceType memberResourseType;
				if(memberExpr != null && this.Metadata.TryResolveResourceTypeByType(memberExpr.Member.DeclaringType, out memberResourseType) && memberResourseType != null) {
					XPMemberInfo prop = memberResourseType.GetAnnotation().ClassInfo.PersistentProperties.OfType<XPMemberInfo>().FirstOrDefault(i => i.Name == memberExpr.Member.Name);
					if(prop != null && prop.Converter != null) {
						if(prop.MemberType.IsEnum) {
							operand = Expression.MakeUnary(ExpressionType.Convert, operand, prop.MemberType.GetEnumUnderlyingType()); 
						} else {
							MethodInfo mi1 = typeof(XpoContext).GetMethod("CallConverter", new Type[] { typeof(XPMemberInfo) }, null);
							Type converterType = prop.Converter.GetType();
							MethodInfo mi2 = converterType.GetMethod("ConvertToStorageType", new Type[] { typeof(object) }, null);
							return Expression.Call(Expression.Convert(Expression.Call(mi1, Expression.Constant(prop)), converterType), mi2,
								prop.MemberType.IsValueType ? Expression.Convert(operand, typeof(object)) : operand);
						}
					}
				}
				return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
			}
			return u;
		}
		internal override Expression VisitLambda(LambdaExpression lambda) {
			Expression body = this.Visit(lambda.Body);
			ParameterExpression[] parameters = new ParameterExpression[lambda.Parameters.Count];
			for(int i = 0; i < parameters.Length; i++) {
				Expression expr = this.Visit(lambda.Parameters[i]);
				if(expr.NodeType == ExpressionType.Convert)
					expr = ((UnaryExpression)expr).Operand;
				parameters[i] = (ParameterExpression)expr;
			}
			return Expression.Lambda(body, parameters);
		}
		internal override Expression VisitMemberInit(MemberInitExpression init) {
			NewExpression n = this.VisitNew(init.NewExpression);
			IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
			if(n != init.NewExpression || bindings != init.Bindings) {
				return Expression.MemberInit(n, bindings);
			}
			return init;
		}
		internal override Expression VisitMemberAccess(MemberExpression m) {
			if(m.Expression == null)
				return m;
			Expression exp = this.Visit(m.Expression);
			if(exp != m.Expression) {
				return Expression.MakeMemberAccess(exp, m.Member);
			}
			if(exp.NodeType == ExpressionType.Constant) {
				FieldInfo fi = m.Member as FieldInfo;
				if(fi != null)
					return Expression.Constant(fi.GetValue(((ConstantExpression)exp).Value));
			}
			return m;
		}
		internal override NewExpression VisitNew(NewExpression nex) {
			IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
			if(nex.Type.IsGenericType) {
				Type memberType = nex.Type;
				Type[] constuctorParameterTypes = nex.Arguments.Select(i => i.GetType()).ToArray();
				return Expression.New(memberType.GetConstructor(constuctorParameterTypes), args);
			}
			if(args != nex.Arguments) {
				if(nex.Members != null) return Expression.New(nex.Constructor, args, nex.Members);
				return Expression.New(nex.Constructor, args);
			}
			return nex;
		}
		internal override MemberAssignment VisitMemberAssignment(MemberAssignment assignment) {
			Expression e = Visit(assignment.Expression);
			Type instanceType = assignment.Member.ReflectedType;
			Type memberType;
			MemberInfo memberInfo = null;
			switch(assignment.Member.MemberType) {
				case MemberTypes.Field:
					memberInfo = instanceType.GetField(assignment.Member.Name);
					memberType = ((FieldInfo)memberInfo).FieldType;
					break;
				case MemberTypes.Property:
					memberInfo = instanceType.GetProperty(assignment.Member.Name);
					memberType = ((PropertyInfo)memberInfo).PropertyType;
					break;
				default:
					throw new NotImplementedException();
			}
			if(memberType != e.Type)
				e = Expression.Convert(e, memberType);
			return Expression.Bind(memberInfo, e);
		}
		internal ReadOnlyCollection<Expression> VisitExpressionListForCall(ReadOnlyCollection<Expression> original) {
			List<Expression> list = null;
			int stackCount = ElementParameterStack.Count;
			try {
				for(int i = 0, n = original.Count; i < n; i++) {
					Expression p = this.Visit(original[i]);
					if(i == 0 && TypeSystem.IsQueryableType(p.Type)) {
						ElementParameterStack.Push(GetParameter(p.Type.GetGenericArguments()[0]));
					} else
						if(p.Type.GetInterface("IEnumerable") != null && p.Type.GetGenericArguments()[0] != ElementParameterStack.Peek().Type) {
							ElementParameterStack.Push(GetParameter(p.Type.GetGenericArguments()[0]));
						}
					if(list != null) {
						list.Add(p);
					} else if(p != original[i]) {
						list = new List<Expression>(n);
						for(int j = 0; j < i; j++) {
							list.Add(original[j]);
						}
						list.Add(p);
					}
				}
				if(list != null) {
					return list.AsReadOnly();
				}
			} finally {
				for(; ElementParameterStack.Count > stackCount; ) {
					ElementParameterStack.Pop();
				}
			}
			return original;
		}
		internal override Expression VisitMethodCall(MethodCallExpression m) {
			switch(m.Method.Name) {
				case "Any": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							if(TypeSystem.IsEnumerableType(args[0].Type)) {
								if(args.Count == 1)
									return Expression.Call(typeof(Enumerable), "Any", new Type[] { sourceType }, GetOperand(args[0]));
								else {
									Expression source = GetOperand(args[0]);
									if(sourceType.IsGenericType) {
										sourceType = sourceType.GetGenericArguments()[0];
									}
									return Expression.Call(typeof(Enumerable), "Any", new Type[] { sourceType }, source, args[1]);
								}
							}
							var result = Expression.Call(typeof(Queryable), "Any", new Type[] { sourceType }, args.ToArray());
							return result;
						}
						return m;
					}
				case "All": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							if(TypeSystem.IsEnumerableType(args[0].Type)) {
								if(args.Count == 1)
									return Expression.Call(typeof(Enumerable), "All", new Type[] { sourceType }, args[0]);
								else {
									Expression source = GetOperand(args[0]);
									if(sourceType.IsGenericType) {
										sourceType = sourceType.GetGenericArguments()[0];
									}
									return Expression.Call(typeof(Enumerable), "All", new Type[] { sourceType }, source, args[1]);
								}
							}
							var result = Expression.Call(typeof(Queryable), "All", new Type[] { sourceType }, args.ToArray());
							return result;
						}
						return m;
					}
				case "LongCount": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							var result = Expression.Call(typeof(Queryable), "LongCount", new Type[] { sourceType }, args.ToArray());
							return result;
						}
						return m;
					}
				case "OrderBy": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							Type keyType = GetGenericArguments(GetGenericArguments(args[1].Type, 0), 1);
							var result = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { sourceType, keyType }, args.ToArray());
							return result;
						}
						return m;
					}
				case "OfType": {
						Type sourceType = null;
						if(m.Arguments.Count > 1 && m.Arguments[1].NodeType == ExpressionType.Constant && m.Arguments[1].Type == typeof(ResourceType)) {
							ConstantExpression constant = (ConstantExpression)m.Arguments[1];
							ResourceType resourceType = (ResourceType)constant.Value;
							sourceType = resourceType.InstanceType;
						}
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							sourceType = sourceType ?? GetGenericArguments(args[0].Type, 0);
							var result = Expression.Call(typeof(Queryable), "OfType", new Type[] { sourceType }, args[0]);
							return result;
						}
						return m;
					}
				case "OrderByDescending": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							Type keyType = GetGenericArguments(GetGenericArguments(args[1].Type, 0), 1);
							var result = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { sourceType, keyType }, args.ToArray());
							return result;
						}
						return m;
					}
				case "Select": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							Type resultType = GetGenericArguments(GetGenericArguments(args[1].Type, 1), 1);
							if(!TypeSystem.IsQueryableType(args[0].Type)) {
								Expression[] args2 = { Expression.Call(typeof(Queryable), "AsQueryable", new Type[] { sourceType }, args[0]), args[1] };
								args = new ReadOnlyCollection<Expression>(args2);
							}
							return Expression.Call(typeof(Queryable), "Select", new Type[] { sourceType, resultType }, args.ToArray());
						}
						return m;
					}
				case "SelectMany": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							Type resultType = GetGenericArguments(GetGenericArguments(GetGenericArguments(args[1].Type, 0), 1), 0);
							return Expression.Call(typeof(Queryable), "SelectMany", new Type[] { sourceType, resultType }, args.ToArray());
						}
						return m;
					}
				case "Skip": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							var result = Expression.Call(typeof(Queryable), "Skip", new Type[] { sourceType }, args.ToArray());
							return result;
						}
						return m;
					}
				case "Take": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							var result = Expression.Call(typeof(Queryable), "Take", new Type[] { sourceType }, args.ToArray());
							return result;
						}
						return m;
					}
				case "ThenBy": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							Type keyType = GetGenericArguments(GetGenericArguments(args[1].Type, 0), 1);
							var result = Expression.Call(typeof(Queryable), "ThenBy", new Type[] { sourceType, keyType }, args.ToArray());
							return result;
						}
						return m;
					}
				case "ThenByDescending": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							Type keyType = GetGenericArguments(GetGenericArguments(args[1].Type, 0), 1);
							var result = Expression.Call(typeof(Queryable), "ThenByDescending", new Type[] { sourceType, keyType }, args.ToArray());
							return result;
						}
						return m;
					}
				case "Where": {
						Expression obj = this.Visit(m.Object);
						ReadOnlyCollection<Expression> args = this.VisitExpressionListForCall(m.Arguments);
						if(obj != m.Object || args != m.Arguments) {
							Type sourceType = GetGenericArguments(args[0].Type, 0);
							return Expression.Call(typeof(Queryable), "Where", new Type[] { sourceType }, args.ToArray());
						}
						return m;
					}
			}
			if(m.Method == GetValueMethodInfo) {
				Expression obj = this.Visit(m.Object);
				ReadOnlyCollection<Expression> args = this.VisitExpressionList(m.Arguments);
				if(obj != m.Object || args != m.Arguments) {
					string memberName;
					if(args[1].NodeType == ExpressionType.Constant) {
						memberName = ((ResourceProperty)((ConstantExpression)args[1]).Value).Name;
					} else {
						Func<object> func = Expression.Lambda<Func<object>>(args[1]).Compile();
						memberName = ((ResourceProperty)func()).Name;
					}
					ResourceType currentResourceType;
					if(!Metadata.TryResolveResourceTypeByType(args[0].Type, out currentResourceType))
						throw new InvalidOperationException();
					XPMemberInfo xpMi = currentResourceType.GetAnnotation().ClassInfo.FindMember(memberName);
					if(xpMi == null) throw new InvalidOperationException(string.Format(DevExpress.Xpo.Extensions.Properties.Resources.CannotFindMember, memberName));
					MemberInfo mi = xpMi.Owner.ClassType.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if(mi == null) mi = xpMi.Owner.ClassType.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if(mi == null) throw new InvalidOperationException(string.Format(DevExpress.Xpo.Extensions.Properties.Resources.CannotFindMember, memberName));
					return Expression.MakeMemberAccess(args[0], mi);
				}
			}
			if(m.Method.IsGenericMethod && m.Method.GetGenericMethodDefinition() == GetSequenceValueMethodInfo) {
				Expression obj = this.Visit(m.Object);
				ReadOnlyCollection<Expression> args = this.VisitExpressionList(m.Arguments);
				if(obj != m.Object || args != m.Arguments) {
					string memberName;
					if(args[1].NodeType == ExpressionType.Constant) {
						memberName = ((ResourceProperty)((ConstantExpression)args[1]).Value).Name;
					} else {
						Func<object> func = Expression.Lambda<Func<object>>(args[1]).Compile();
						memberName = ((ResourceProperty)func()).Name;
					}
					MemberInfo mi = args[0].Type.GetProperty(memberName);
					if(mi == null) mi = args[0].Type.GetField(memberName);
					if(mi == null) throw new InvalidOperationException(string.Format(DevExpress.Xpo.Extensions.Properties.Resources.CannotFindMember, memberName));
					var result = Expression.MakeMemberAccess(args[0], mi);
					Type tempType = result.Type.GetGenericArguments()[0];
					Type resultType = typeof(IEnumerable<>).MakeGenericType(tempType);
					return Expression.Convert(result, resultType);
				}
			}
			if(m.Method == ConvertMethodInfo) {
				if(m.Arguments.Count > 1 && m.Arguments[1].Type == typeof(ResourceType) && m.Arguments[1] is ConstantExpression) {
					ResourceType entityType = ((ConstantExpression)m.Arguments[1]).Value as ResourceType;
					if(entityType != null) {
						TypeParameterStack.Push(entityType.GetAnnotation().ClassInfo.ClassType);
						Expression result = this.Visit(m.Arguments[0]);
						TypeParameterStack.Pop();
						return result;
					}
				}
				return this.Visit(m.Arguments[0]);
			}
			if(m.Method == TypeIsMethodInfo) {
				ReadOnlyCollection<Expression> args = this.VisitExpressionList(m.Arguments);
				ConstantExpression expr = args[1] as ConstantExpression;
				Type argType = ((ResourceType)expr.Value).GetAnnotation().ClassInfo.ClassType;
				return Expression.TypeIs(args[0], argType);
			}
			return base.VisitMethodCall(m);
		}
		Type GetGenericArguments(Type type, int index) {
			if(type == null || !type.IsGenericType) return type;
			Type[] genericArguments = type.GetGenericArguments();
			if(type.BaseType == typeof(LambdaExpression) || type.BaseType == typeof(XPQueryBase) || type.BaseType == typeof(MulticastDelegate) || type.IsInterface) {
				if(index < genericArguments.Length)
					return genericArguments[index];
				else
					return genericArguments[genericArguments.Length - 1];
			} else {
				return type;
			}
		}
		internal override Expression VisitConditional(ConditionalExpression c) {
			Expression test = this.Visit(c.Test);
			Expression iftrue = this.Visit(c.IfTrue);
			Expression iffalse = this.Visit(c.IfFalse);
			if(test != c.Test || iftrue != c.IfTrue || iffalse != c.IfFalse) {
				if(iftrue.Type != c.Type && TypeSystem.AreConvertable(iftrue.Type, c.Type))
					iftrue = Expression.Convert(iftrue, c.Type);
				if(iffalse.Type != c.Type && TypeSystem.AreConvertable(iffalse.Type, c.Type))
					iffalse = Expression.Convert(iffalse, c.Type);
				if(test.NodeType == ExpressionType.Equal && ((ConstantExpression)((BinaryExpression)test).Right).Value == null && TypeSystem.AreConvertable(((BinaryExpression)test).Left.Type, c.Type))
					return iffalse;
				return Expression.Condition(test, iftrue, iffalse);
			}
			return c;
		}
		Expression GetOperand(Expression expression) {
			switch(expression.NodeType) {
				case ExpressionType.UnaryPlus:
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.ArrayLength:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
					return ((UnaryExpression)expression).Operand;
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.Divide:
				case ExpressionType.Modulo:
				case ExpressionType.Power:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
				case ExpressionType.Coalesce:
				case ExpressionType.ArrayIndex:
				case ExpressionType.RightShift:
				case ExpressionType.LeftShift:
				case ExpressionType.ExclusiveOr:
					throw new InvalidOperationException(expression.NodeType.ToString());
				case ExpressionType.TypeIs:
				case ExpressionType.Conditional:
				case ExpressionType.Constant:
				case ExpressionType.Parameter:
					throw new InvalidOperationException(expression.NodeType.ToString());
				case ExpressionType.MemberAccess:
					return expression;
				case ExpressionType.Call:
				case ExpressionType.Lambda:
				case ExpressionType.New:
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
				case ExpressionType.Invoke:
				case ExpressionType.MemberInit:
				case ExpressionType.ListInit:
				default:
					throw new InvalidOperationException(expression.NodeType.ToString());
			}
		}
	}
}
