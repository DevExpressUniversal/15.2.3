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

namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using DevExpress.Mvvm;
	using DevExpress.Utils;
	using DevExpress.Xpf.Core.Native;
	using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
	public class ModelRegisterCollection : List<IModelRegistry> { } 
}
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using DevExpress.Mvvm;
	using DevExpress.Utils;
	using DevExpress.Xpf.Core.Native;
	using DevExpress.Xpf.Reports.UserDesigner.Native;
	public interface IModelRegistry { }
	public interface IModelRegistry<TModelFactory> : IModelRegistry where TModelFactory : IModelFactory {
		void Register(TModelFactory factory);
	}
	public interface IModelFactory {
		void AssertModelIsCreatingByFactory(object modelCreationToken);
	}
	public abstract class ModelFactory<TOwner, TSourceBase, TTargetBase> : IModelFactory
		where TOwner : class
		where TSourceBase : class
		where TTargetBase : class {
		public interface ISource<out TSource> where TSource : TSourceBase {
			TSource XRObject { get; }
			TOwner Owner { get; }
			ModelFactoryData FactoryData { get; }
		}
		public interface IRegistry<TSource> where TSource : TSourceBase {
			void Register(Func<Source<TSource>, TTargetBase> createModelFunc);
		}
		public class Source<TSource> : ISource<TSource> where TSource : TSourceBase {
			public Source(TSource xrObject, TOwner owner, ModelFactoryData factoryData) {
				Guard.ArgumentNotNull(factoryData, "factoryData");
				XRObject = xrObject;
				Owner = owner;
				FactoryData = factoryData;
			}
			public TSource XRObject { get; private set; }
			public TOwner Owner { get; private set; }
			public ModelFactoryData FactoryData { get; private set; }
		}
		TOwner owner;
		Func<TTargetBase, TSourceBase> getSourceFunc;
		protected ModelFactory(TOwner owner, Func<TTargetBase, TSourceBase> getSourceFunc) {
			this.owner = owner;
			this.getSourceFunc = getSourceFunc;
		}
		protected static void Initialize<TModelFactory>(TModelFactory factory, IEnumerable<IModelRegistry> modelRegistries) where TModelFactory : IModelFactory {
			foreach(var registry in modelRegistries.OfType<IModelRegistry<TModelFactory>>())
				registry.Register(factory);
		}
		readonly TypeTree<Func<TSourceBase, TOwner, ModelFactoryData, TTargetBase>> modelFactories = new TypeTree<Func<TSourceBase, TOwner, ModelFactoryData, TTargetBase>>();
		public void Register(Type xrControlType, Func<TSourceBase, TOwner, ModelFactoryData, TTargetBase> createModelFunc) {
			if(!typeof(TSourceBase).IsAssignableFrom(xrControlType))
				throw new ArgumentException("", "xrControlType");
			modelFactories.TryAdd(xrControlType, createModelFunc);
#if DEBUGTEST
			registeredTypes.Add(xrControlType);
#endif
		}
		public IRegistry<TSource> Registry<TSource>() where TSource : TSourceBase {
			return new RegistryImpl<TSource>(this);
		}
		class RegistryImpl<TSource> : IRegistry<TSource> where TSource : TSourceBase {
			readonly ModelFactory<TOwner, TSourceBase, TTargetBase> factory;
			public RegistryImpl(ModelFactory<TOwner, TSourceBase, TTargetBase> factory) {
				this.factory = factory;
			}
			public void Register(Func<Source<TSource>, TTargetBase> createModelFunc) {
				factory.Register(typeof(TSource), (control, report, token) => createModelFunc(new Source<TSource>((TSource)control, report, token)));
			}
		}
#if DEBUGTEST
		readonly List<Type> registeredTypes = new List<Type>();
		public IEnumerable<Type> RegisteredTypes { get { return registeredTypes.AsReadOnly(); } }
#endif
		readonly WrappersManager<TSourceBase, TTargetBase> modelsManager = new WrappersManager<TSourceBase, TTargetBase>();
		public TTargetBase GetModel(TSourceBase source, bool sourceIsNew = false) {
			return modelsManager.Wrap(source, control => modelFactories.Find(control.GetType()).Where(x => x.IsNearest).Select(x => x.Value).Single()(control, owner, new ModelFactoryData(owner, this, modelCreationToken, sourceIsNew)));
		}
		object modelCreationToken = new object();
		void IModelFactory.AssertModelIsCreatingByFactory(object modelCreationToken) {
			if(this.modelCreationToken != modelCreationToken)
				throw new InvalidOperationException();
		}
	}
}
