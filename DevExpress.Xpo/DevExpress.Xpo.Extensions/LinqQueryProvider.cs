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
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.Data.Services.Providers;
using System.Data.Services.Internal;
using System.Data.Services;
namespace DevExpress.Xpo {
	using DevExpress.Xpo.Helpers;
	public class XpoLinqQueryProvider : IQueryProvider {
		readonly XpoDataServiceV3 dataService;
		public XpoDataServiceV3 DataService {
			get { return dataService; }
		}
		readonly ResourceType resourceType;
		public ResourceType ResourceType {
			get { return resourceType; }
		}
		public XpoLinqQueryProvider(ResourceType resourceType, XpoDataServiceV3 dataService) {
			this.resourceType = resourceType;
			this.dataService = dataService;
		}
		#region IQueryProvider Members
		public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
			return new XpoLinqQuery<TElement>(this, expression);
		}
		public IQueryable CreateQuery(Expression expression) {
			try {
				return CreateQuery(expression.Type, expression);
			} catch(TargetInvocationException tie) {
				throw tie.InnerException;
			}
		}
		public IQueryable CreateQuery(Type elementType, Expression expression) {
				Type result = typeof(XpoLinqQuery<>).MakeGenericType(elementType);
				ConstructorInfo ci = result.GetConstructor(new Type[] { this.GetType(), typeof(Expression) });
				return (IQueryable)ci.Invoke(new object[] { this, expression });
		}
		TResult IQueryProvider.Execute<TResult>(Expression expression) {
			return (TResult)this.Execute(expression);
		}
		object IQueryProvider.Execute(Expression expression) {
			return this.Execute(expression);
		}
		static readonly object executeSyncObject = new object();
		public object Execute(Expression expression) {
			UnitOfWork session = dataService.GetSession(false);
			expression = this.ProcessExpression(expression, session);			
			if (expression.NodeType == ExpressionType.Constant) {
				ConstantExpression c = (ConstantExpression)expression;
				return (IQueryable)c.Value;
			}
			bool executeSingle = false;
			if (expression.Type.IsGenericType) {
				Type expressionType = TypeSystem.GetElementType(expression.Type);
				ResourceType resType;
				if (dataService.Metadata.TryResolveResourceTypeByType(expressionType, out resType)) {
					lock(executeSyncObject) {
						IQueryable xpQuery = ((IQueryProvider)dataService.Context.GetXPQueryCreator(expressionType).CreateXPQuery(session)).CreateQuery(expression);
						return xpQuery;
					}
				}
			} else
				executeSingle = true;
			Expression currentExpression = expression;
			Type elementType = null;
			while (elementType == null) {
				if (currentExpression.Type.IsGenericType) {
					Type expressionType = TypeSystem.GetElementType(currentExpression.Type);
					ResourceType resType;
					if(dataService.Metadata.TryResolveResourceTypeByType(expressionType, out resType)) {
						elementType = resType.GetAnnotation().ClassInfo.ClassType;
						break;
					};
				}
				MethodCallExpression call = currentExpression as MethodCallExpression;
				if (call == null)
					throw new InvalidOperationException();
				currentExpression = call.Arguments[0];
			}
			if(executeSingle) {
				lock(executeSyncObject) {
					return ((IQueryProvider)dataService.Context.GetXPQueryCreator(elementType).CreateXPQuery(session)).Execute(expression);
				}
			}
			XPQueryCreatorBase xpCreator = dataService.Context.GetXPQueryCreator(elementType);
			lock(executeSyncObject) {
				return xpCreator.Enumerate(((IQueryProvider)xpCreator.CreateXPQuery(session)).CreateQuery(expression));
			}
		}
		#endregion
		private Expression ProcessExpression(Expression expression, UnitOfWork session) {
			return MethodTranslatingVisitor.TranslateExpression(this, resourceType, expression, session);
		}
	}
}
namespace DevExpress.Xpo.Helpers{
	public abstract class XPQueryCreatorBase {
		public abstract XpoContext GetContext();
		public abstract IQueryable CreateXPQuery(Session session);
		public abstract IEnumerable Enumerate(IQueryable queryable);
	}
	public class XPQueryCreator<T> : XPQueryCreatorBase {
		XpoContext context;
		public XPQueryCreator(XpoContext context) {
			this.context = context;
		}
		public override XpoContext GetContext() {
			return context;
		}
		public override IQueryable CreateXPQuery(Session session) {
			return new XPQuery<T>(session);
		}
		public override IEnumerable Enumerate(IQueryable queryable) {
			IQueryable<T> queryableG = queryable as IQueryable<T>;
			if(queryableG == null) {
				XPQueryExpandHelperBase expandHelper = GetContext().GetXPQueryExpandHelper(queryable.ElementType);
				if(expandHelper == null) {
					List<object> result = new List<object>();
					foreach(object row in queryable) {
						result.Add(row);
					}
					return result;
				} else {
					return expandHelper.ConvertExpandCollection(queryable);
				}
			}
			return queryableG.ToList();
		}
	}
	public abstract class XPQueryExpandHelperBase {
		public abstract Type GetResultCollectionType();
		public abstract Type GetResultType();
		public abstract object ConvertExpandedWrapper(object wrapper);
		public abstract IEnumerable ConvertExpandCollection(IQueryable queryable);
	}
	class XPQueryExpandHelper<EW, T> : XPQueryExpandHelperBase where EW : ExpandedWrapper<T> {
		readonly Type[] inputArgumentTypes;
		readonly Type[] resultArgumentTypes;
		readonly ResourceType elementResType;
		readonly ResourceType[] resultResourceTypes;
		readonly XPQueryExpandHelperBase[] resultExpandHelpers;
		readonly XPQueryExpandHelperBase[] resultExpandCollectionHelpers;
		readonly XPQueryIQueryableToIEnumerableConverterBase[] resultQTOEConverters;
		readonly XPQueryExpandCreatorBase inputExpandCreator;
		readonly XPQueryExpandCreatorBase expandCreator;
		readonly XpoMetadata metaData;
		readonly XpoContext context;
		readonly Type resultExpandWrapperCollectionType;
		readonly Type resultExpandWrapperType;
		public XPQueryExpandHelper(XpoContext context) {
			this.context = context;
			this.metaData = context.Metadata;
			inputArgumentTypes = typeof(EW).GetGenericArguments();
			resultArgumentTypes = new Type[inputArgumentTypes.Length];
			resultResourceTypes = new ResourceType[inputArgumentTypes.Length];
			resultExpandHelpers = new XPQueryExpandHelperBase[inputArgumentTypes.Length];
			resultExpandCollectionHelpers = new XPQueryExpandHelperBase[inputArgumentTypes.Length];
			resultQTOEConverters = new XPQueryIQueryableToIEnumerableConverterBase[inputArgumentTypes.Length];
			for(int i = 0; i < resultArgumentTypes.Length; i++) {
				Type inputType = inputArgumentTypes[i];
				XPQueryExpandHelperBase expandHelper = context.GetXPQueryExpandHelper(inputType);
				if(expandHelper != null) {
					resultExpandHelpers[i] = expandHelper;
					resultArgumentTypes[i] = expandHelper.GetResultType();
					continue;
				}
				if(inputType.IsGenericType) {
					Type elementType = inputType.GetGenericArguments()[0];
					XPQueryExpandHelperBase expandCollectionHelper = context.GetXPQueryExpandHelper(elementType);
					if(expandCollectionHelper != null) {
						resultExpandCollectionHelpers[i] = expandCollectionHelper;
						resultArgumentTypes[i] = expandCollectionHelper.GetResultCollectionType();
						continue;
					}
					ResourceType resType;
					if(metaData.TryResolveResourceTypeByType(elementType, out resType)) {
						resultArgumentTypes[i] = typeof(IEnumerable<>).MakeGenericType(resType.InstanceType);
						resultResourceTypes[i] = resType;
						continue;
					}
					if(TypeSystem.IsQueryableType(inputType)) {
						XPQueryIQueryableToIEnumerableConverterBase converter = context.GetXPQueryQToEConverter(elementType);
						resultQTOEConverters[i] = converter;
						resultArgumentTypes[i] = converter.GetResultCollectionType();
						continue;
					}
				} else {
					ResourceType resType;
					if(metaData.TryResolveResourceTypeByType(inputType, out resType)) {
						resultArgumentTypes[i] = resType.InstanceType;
						resultResourceTypes[i] = resType;
						continue;
					}
				}
				resultArgumentTypes[i] = inputType;
			}
			if(!metaData.TryResolveResourceTypeByType(typeof(T), out elementResType)) {
				elementResType = null;
			}
			Type resultExpandCreatorType = null;
			Type inputExpandCreatorType = null;
			switch(resultArgumentTypes.Length) {
				case 2:
					resultExpandWrapperType = typeof(ExpandedWrapper<,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,>).MakeGenericType(inputArgumentTypes);
					break;
				case 3:
					resultExpandWrapperType = typeof(ExpandedWrapper<,,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,,>).MakeGenericType(inputArgumentTypes);
					break;
				case 4:
					resultExpandWrapperType = typeof(ExpandedWrapper<,,,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,,,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,,,>).MakeGenericType(inputArgumentTypes);
					break;
				case 5:
					resultExpandWrapperType = typeof(ExpandedWrapper<,,,,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,,,,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,,,,>).MakeGenericType(inputArgumentTypes);
					break;
				case 6:
					resultExpandWrapperType = typeof(ExpandedWrapper<,,,,,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,>).MakeGenericType(inputArgumentTypes);
					break;
				case 7:
					resultExpandWrapperType = typeof(ExpandedWrapper<,,,,,,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,>).MakeGenericType(inputArgumentTypes);
					break;
				case 8:
					resultExpandWrapperType = typeof(ExpandedWrapper<,,,,,,,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,>).MakeGenericType(inputArgumentTypes);
					break;
				case 9:
					resultExpandWrapperType = typeof(ExpandedWrapper<,,,,,,,,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,,>).MakeGenericType(inputArgumentTypes);
					break;
				case 10:
					resultExpandWrapperType = typeof(XPQueryExpandCreator<,,,,,,,,,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,,,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,,,>).MakeGenericType(inputArgumentTypes);
					break;
				case 11:
					resultExpandWrapperType = typeof(ExpandedWrapper<,,,,,,,,,,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,,,,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,,,,>).MakeGenericType(inputArgumentTypes);
					break;
				case 12:
					resultExpandWrapperType = typeof(ExpandedWrapper<,,,,,,,,,,,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,,,,,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,,,,,>).MakeGenericType(inputArgumentTypes);
					break;
				case 13:
					resultExpandWrapperType = typeof(ExpandedWrapper<,,,,,,,,,,,,>).MakeGenericType(resultArgumentTypes);
					resultExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,,,,,,>).MakeGenericType(resultArgumentTypes);
					inputExpandCreatorType = typeof(XPQueryExpandCreator<,,,,,,,,,,,,>).MakeGenericType(inputArgumentTypes);
					break;
				default:
					throw new InvalidOperationException();
			}
			resultExpandWrapperCollectionType = typeof(IEnumerable<>).MakeGenericType(resultExpandWrapperType);
			expandCreator = (XPQueryExpandCreatorBase)Activator.CreateInstance(resultExpandCreatorType);
			inputExpandCreator = (XPQueryExpandCreatorBase)Activator.CreateInstance(inputExpandCreatorType);
		}
		public override Type GetResultCollectionType() {
			return resultExpandWrapperCollectionType;
		}
		public override Type GetResultType() {
			return resultExpandWrapperType;
		}
		public override object ConvertExpandedWrapper(object wrapper) {
			return GetResult(new object[resultArgumentTypes.Length - 1], (ExpandedWrapper<T>)wrapper);
		}
		private object GetResult(object[] properties, ExpandedWrapper<T> ew) {
			object expandedElement = null;
			for(int pIndex = 0; pIndex < resultArgumentTypes.Length; pIndex++) {
				object propValue = pIndex == 0 ? ew.ExpandedElement : inputExpandCreator.GetPropertyValue(ew, pIndex - 1);
				ResourceType resType = resultResourceTypes[pIndex];
				if(resType == null) {
					if(resultExpandCollectionHelpers[pIndex] != null) {
						if(pIndex == 0) {
							expandedElement = resultExpandCollectionHelpers[pIndex].ConvertExpandCollection((IQueryable)propValue);
						} else {
							properties[pIndex - 1] = resultExpandCollectionHelpers[pIndex].ConvertExpandCollection((IQueryable)propValue);
						}
					} else if(resultExpandHelpers[pIndex] != null) {
						if(pIndex == 0) {
							expandedElement = resultExpandHelpers[pIndex].ConvertExpandedWrapper(propValue);
						} else {
							properties[pIndex - 1] = resultExpandHelpers[pIndex].ConvertExpandedWrapper(propValue);
						}
					} else if(resultQTOEConverters[pIndex] != null) {
						if(pIndex == 0) {
							expandedElement = resultQTOEConverters[pIndex].Convert(propValue);
						} else {
							properties[pIndex - 1] = resultQTOEConverters[pIndex].Convert(propValue);
						}
					} else {
						if(pIndex == 0) {
							expandedElement = propValue;
						} else {
							properties[pIndex - 1] = propValue;
						}
					}
					continue;
				}
				if(TypeSystem.IsEntity(resultArgumentTypes[pIndex], metaData)) {
					if(pIndex == 0) {
						expandedElement =propValue;
					} else {
						properties[pIndex - 1] = propValue;
					}
					continue;
				}
				if(pIndex == 0) {
					expandedElement = propValue;
				} else {
					properties[pIndex - 1] = propValue;
				}
			}
			return expandCreator.CreateExpandedWrapper(expandedElement, ew.Description, properties);
		}
		public override IEnumerable ConvertExpandCollection(IQueryable queryable) {
			IList resultCollection = (IList)expandCreator.CreateExpandedWrapperList();
			object[] properties = new object[resultArgumentTypes.Length - 1];
			foreach(ExpandedWrapper<T> ew in queryable) {
				resultCollection.Add(GetResult(properties, ew));
			}
			return resultCollection;
		}
	}
	abstract class XPQueryExpandCreatorBase {
		public abstract IList CreateExpandedWrapperList();
		public abstract object CreateExpandedWrapperInstance(object element, string description);
		public abstract object GetPropertyValue(object wrapper, int index);
		public abstract object CreateExpandedWrapper(object element, string description, object[] properties);
	}
	abstract class XPQueryExpandCreator<T> : XPQueryExpandCreatorBase {
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T>)CreateExpandedWrapperInstance(element, description);
			result.Description = description;
			result.ExpandedElement = (T)element;
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			ExpandedWrapper<T, P0> ew = (ExpandedWrapper<T, P0>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0, P1> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0, P1>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0, P1>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			var ew = (ExpandedWrapper<T, P0, P1>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				case 1:
					return ew.ProjectedProperty1;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0, P1>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			result.ProjectedProperty1 = (P1)(properties[1]);
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0, P1, P2> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0, P1, P2>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0, P1, P2>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			var ew = (ExpandedWrapper<T, P0, P1, P2>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				case 1:
					return ew.ProjectedProperty1;
				case 2:
					return ew.ProjectedProperty2;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0, P1, P2>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			result.ProjectedProperty1 = (P1)(properties[1]);
			result.ProjectedProperty2 = (P2)(properties[2]);
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0, P1, P2, P3> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0, P1, P2, P3>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0, P1, P2, P3>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			var ew = (ExpandedWrapper<T, P0, P1, P2, P3>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				case 1:
					return ew.ProjectedProperty1;
				case 2:
					return ew.ProjectedProperty2;
				case 3:
					return ew.ProjectedProperty3;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0, P1, P2, P3>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			result.ProjectedProperty1 = (P1)(properties[1]);
			result.ProjectedProperty2 = (P2)(properties[2]);
			result.ProjectedProperty3 = (P3)(properties[3]);
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0, P1, P2, P3, P4> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0, P1, P2, P3, P4>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0, P1, P2, P3, P4>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			var ew = (ExpandedWrapper<T, P0, P1, P2, P3, P4>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				case 1:
					return ew.ProjectedProperty1;
				case 2:
					return ew.ProjectedProperty2;
				case 3:
					return ew.ProjectedProperty3;
				case 4:
					return ew.ProjectedProperty4;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0, P1, P2, P3, P4>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			result.ProjectedProperty1 = (P1)(properties[1]);
			result.ProjectedProperty2 = (P2)(properties[2]);
			result.ProjectedProperty3 = (P3)(properties[3]);
			result.ProjectedProperty4 = (P4)(properties[4]);
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0, P1, P2, P3, P4, P5> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0, P1, P2, P3, P4, P5>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0, P1, P2, P3, P4, P5>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			var ew = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				case 1:
					return ew.ProjectedProperty1;
				case 2:
					return ew.ProjectedProperty2;
				case 3:
					return ew.ProjectedProperty3;
				case 4:
					return ew.ProjectedProperty4;
				case 5:
					return ew.ProjectedProperty5;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			result.ProjectedProperty1 = (P1)(properties[1]);
			result.ProjectedProperty2 = (P2)(properties[2]);
			result.ProjectedProperty3 = (P3)(properties[3]);
			result.ProjectedProperty4 = (P4)(properties[4]);
			result.ProjectedProperty5 = (P5)(properties[5]);
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0, P1, P2, P3, P4, P5, P6> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			var ew = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				case 1:
					return ew.ProjectedProperty1;
				case 2:
					return ew.ProjectedProperty2;
				case 3:
					return ew.ProjectedProperty3;
				case 4:
					return ew.ProjectedProperty4;
				case 5:
					return ew.ProjectedProperty5;
				case 6:
					return ew.ProjectedProperty6;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			result.ProjectedProperty1 = (P1)(properties[1]);
			result.ProjectedProperty2 = (P2)(properties[2]);
			result.ProjectedProperty3 = (P3)(properties[3]);
			result.ProjectedProperty4 = (P4)(properties[4]);
			result.ProjectedProperty5 = (P5)(properties[5]);
			result.ProjectedProperty6 = (P6)(properties[6]);
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0, P1, P2, P3, P4, P5, P6, P7> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			var ew = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				case 1:
					return ew.ProjectedProperty1;
				case 2:
					return ew.ProjectedProperty2;
				case 3:
					return ew.ProjectedProperty3;
				case 4:
					return ew.ProjectedProperty4;
				case 5:
					return ew.ProjectedProperty5;
				case 6:
					return ew.ProjectedProperty6;
				case 7:
					return ew.ProjectedProperty7;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			result.ProjectedProperty1 = (P1)(properties[1]);
			result.ProjectedProperty2 = (P2)(properties[2]);
			result.ProjectedProperty3 = (P3)(properties[3]);
			result.ProjectedProperty4 = (P4)(properties[4]);
			result.ProjectedProperty5 = (P5)(properties[5]);
			result.ProjectedProperty6 = (P6)(properties[6]);
			result.ProjectedProperty7 = (P7)(properties[7]);
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0, P1, P2, P3, P4, P5, P6, P7, P8> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			var ew = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				case 1:
					return ew.ProjectedProperty1;
				case 2:
					return ew.ProjectedProperty2;
				case 3:
					return ew.ProjectedProperty3;
				case 4:
					return ew.ProjectedProperty4;
				case 5:
					return ew.ProjectedProperty5;
				case 6:
					return ew.ProjectedProperty6;
				case 7:
					return ew.ProjectedProperty7;
				case 8:
					return ew.ProjectedProperty8;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			result.ProjectedProperty1 = (P1)(properties[1]);
			result.ProjectedProperty2 = (P2)(properties[2]);
			result.ProjectedProperty3 = (P3)(properties[3]);
			result.ProjectedProperty4 = (P4)(properties[4]);
			result.ProjectedProperty5 = (P5)(properties[5]);
			result.ProjectedProperty6 = (P6)(properties[6]);
			result.ProjectedProperty7 = (P7)(properties[7]);
			result.ProjectedProperty8 = (P8)(properties[8]);
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			var ew = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				case 1:
					return ew.ProjectedProperty1;
				case 2:
					return ew.ProjectedProperty2;
				case 3:
					return ew.ProjectedProperty3;
				case 4:
					return ew.ProjectedProperty4;
				case 5:
					return ew.ProjectedProperty5;
				case 6:
					return ew.ProjectedProperty6;
				case 7:
					return ew.ProjectedProperty7;
				case 8:
					return ew.ProjectedProperty8;
				case 9:
					return ew.ProjectedProperty9;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			result.ProjectedProperty1 = (P1)(properties[1]);
			result.ProjectedProperty2 = (P2)(properties[2]);
			result.ProjectedProperty3 = (P3)(properties[3]);
			result.ProjectedProperty4 = (P4)(properties[4]);
			result.ProjectedProperty5 = (P5)(properties[5]);
			result.ProjectedProperty6 = (P6)(properties[6]);
			result.ProjectedProperty7 = (P7)(properties[7]);
			result.ProjectedProperty8 = (P8)(properties[8]);
			result.ProjectedProperty9 = (P9)(properties[9]);
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			var ew = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				case 1:
					return ew.ProjectedProperty1;
				case 2:
					return ew.ProjectedProperty2;
				case 3:
					return ew.ProjectedProperty3;
				case 4:
					return ew.ProjectedProperty4;
				case 5:
					return ew.ProjectedProperty5;
				case 6:
					return ew.ProjectedProperty6;
				case 7:
					return ew.ProjectedProperty7;
				case 8:
					return ew.ProjectedProperty8;
				case 9:
					return ew.ProjectedProperty9;
				case 10:
					return ew.ProjectedProperty10;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			result.ProjectedProperty1 = (P1)(properties[1]);
			result.ProjectedProperty2 = (P2)(properties[2]);
			result.ProjectedProperty3 = (P3)(properties[3]);
			result.ProjectedProperty4 = (P4)(properties[4]);
			result.ProjectedProperty5 = (P5)(properties[5]);
			result.ProjectedProperty6 = (P6)(properties[6]);
			result.ProjectedProperty7 = (P7)(properties[7]);
			result.ProjectedProperty8 = (P8)(properties[8]);
			result.ProjectedProperty9 = (P9)(properties[9]);
			result.ProjectedProperty10 = (P10)(properties[10]);
			return result;
		}
	}
	class XPQueryExpandCreator<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11> : XPQueryExpandCreator<T> {
		public override IList CreateExpandedWrapperList() {
			return new List<ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>>();
		}
		public override object CreateExpandedWrapperInstance(object element, string description) {
			return new ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>();
		}
		public override object GetPropertyValue(object wrapper, int index) {
			var ew = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>)wrapper;
			switch(index) {
				case 0:
					return ew.ProjectedProperty0;
				case 1:
					return ew.ProjectedProperty1;
				case 2:
					return ew.ProjectedProperty2;
				case 3:
					return ew.ProjectedProperty3;
				case 4:
					return ew.ProjectedProperty4;
				case 5:
					return ew.ProjectedProperty5;
				case 6:
					return ew.ProjectedProperty6;
				case 7:
					return ew.ProjectedProperty7;
				case 8:
					return ew.ProjectedProperty8;
				case 9:
					return ew.ProjectedProperty9;
				case 10:
					return ew.ProjectedProperty10;
				case 11:
					return ew.ProjectedProperty11;
				default:
					throw new ArgumentException("index");
			}
		}
		public override object CreateExpandedWrapper(object element, string description, object[] properties) {
			var result = (ExpandedWrapper<T, P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>)base.CreateExpandedWrapper(element, description, properties);
			result.ProjectedProperty0 = (P0)(properties[0]);
			result.ProjectedProperty1 = (P1)(properties[1]);
			result.ProjectedProperty2 = (P2)(properties[2]);
			result.ProjectedProperty3 = (P3)(properties[3]);
			result.ProjectedProperty4 = (P4)(properties[4]);
			result.ProjectedProperty5 = (P5)(properties[5]);
			result.ProjectedProperty6 = (P6)(properties[6]);
			result.ProjectedProperty7 = (P7)(properties[7]);
			result.ProjectedProperty8 = (P8)(properties[8]);
			result.ProjectedProperty9 = (P9)(properties[9]);
			result.ProjectedProperty10 = (P10)(properties[10]);
			result.ProjectedProperty11 = (P11)(properties[11]);
			return result;
		}
	}
	public abstract class XPQueryIQueryableToIEnumerableConverterBase {
		public abstract object Convert(object queryable);
		public abstract Type GetResultCollectionType();
	}
	public class XPQueryIQueryableToIEnumerableConverter<T> : XPQueryIQueryableToIEnumerableConverterBase {
		public override object Convert(object queryable) {
			IQueryable<T> queryableGeneric = (IQueryable<T>)queryable;
			return queryableGeneric.ToList();
		}
		public override Type GetResultCollectionType() {
			return typeof(IEnumerable<T>);
		}
	}
}
