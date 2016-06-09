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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.ViewModel
{
	using System;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class ReadOnlyCollectionViewModelTemplate : ReadOnlyCollectionViewModelTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write(@"using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataAnnotations;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// The base class for POCO view models exposing a rea" +
					"d-only collection of entities of a given type. \r\n    /// This is a partial class" +
					" that provides the extension point to add custom properties, commands and overri" +
					"de methods without modifying the auto-generated code.\r\n    /// </summary>\r\n    /" +
					"// <typeparam name=\"TEntity\">An entity type.</typeparam>\r\n    /// <typeparam nam" +
					"e=\"TUnitOfWork\">A unit of work type.</typeparam>\r\n    public partial class ReadO" +
					"nlyCollectionViewModel<TEntity, TUnitOfWork> : ReadOnlyCollectionViewModel<TEnti" +
					"ty, TEntity, TUnitOfWork>\r\n        where TEntity : class\r\n        where TUnitOfW" +
					"ork : IUnitOfWork {\r\n\r\n        /// <summary>\r\n        /// Creates a new instance" +
					" of ReadOnlyCollectionViewModel as a POCO view model.\r\n        /// </summary>\r\n " +
					"       /// <param name=\"unitOfWorkFactory\">A factory used to create a unit of wo" +
					"rk instance.</param>\r\n        /// <param name=\"getRepositoryFunc\">A function tha" +
					"t returns a repository representing entities of the given type.</param>\r\n       " +
					" /// <param name=\"projection\">An optional parameter that provides a LINQ functio" +
					"n used to customize a query for entities. The parameter, for example, can be use" +
					"d for sorting data.</param>\r\n        public static ReadOnlyCollectionViewModel<T" +
					"Entity, TUnitOfWork> CreateReadOnlyCollectionViewModel(\r\n            IUnitOfWork" +
					"Factory<TUnitOfWork> unitOfWorkFactory,\r\n            Func<TUnitOfWork, IReadOnly" +
					"Repository<TEntity>> getRepositoryFunc,\r\n            Func<IRepositoryQuery<TEnti" +
					"ty>, IQueryable<TEntity>> projection = null) {\r\n            return ViewModelSour" +
					"ce.Create(() => new ReadOnlyCollectionViewModel<TEntity, TUnitOfWork>(unitOfWork" +
					"Factory, getRepositoryFunc, projection));\r\n        }\r\n\r\n        /// <summary>\r\n " +
					"       /// Initializes a new instance of the ReadOnlyCollectionViewModel class.\r" +
					"\n        /// This constructor is declared protected to avoid an undesired instan" +
					"tiation of the PeekCollectionViewModel type without the POCO proxy factory.\r\n   " +
					"     /// </summary>\r\n        /// <param name=\"unitOfWorkFactory\">A factory used " +
					"to create a unit of work instance.</param>\r\n        /// <param name=\"getReposito" +
					"ryFunc\">A function that returns a repository representing entities of the given " +
					"type.</param>\r\n        /// <param name=\"projection\">An optional parameter that p" +
					"rovides a LINQ function used to customize a query for entities. The parameter, f" +
					"or example, can be used for sorting data.</param>\r\n        protected ReadOnlyCol" +
					"lectionViewModel(\r\n            IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory" +
					",\r\n            Func<TUnitOfWork, IReadOnlyRepository<TEntity>> getRepositoryFunc" +
					",\r\n            Func<IRepositoryQuery<TEntity>, IQueryable<TEntity>> projection =" +
					" null)\r\n            : base(unitOfWorkFactory, getRepositoryFunc, projection) {\r\n" +
					"        }\r\n    }\r\n\r\n    /// <summary>\r\n    /// The base class for POCO view mode" +
					"ls exposing a read-only collection of entities of a given type. \r\n    /// This i" +
					"s a partial class that provides the extension point to add custom properties, co" +
					"mmands and override methods without modifying the auto-generated code.\r\n    /// " +
					"</summary>\r\n    /// <typeparam name=\"TEntity\">A repository entity type.</typepar" +
					"am>\r\n    /// <typeparam name=\"TProjection\">A projection entity type.</typeparam>" +
					"\r\n    /// <typeparam name=\"TUnitOfWork\">A unit of work type.</typeparam>\r\n    pu" +
					"blic partial class ReadOnlyCollectionViewModel<TEntity, TProjection, TUnitOfWork" +
					"> : ReadOnlyCollectionViewModelBase<TEntity, TProjection, TUnitOfWork>\r\n        " +
					"where TEntity : class\r\n        where TProjection : class\r\n        where TUnitOfW" +
					"ork : IUnitOfWork {\r\n\r\n        /// <summary>\r\n        /// Creates a new instance" +
					" of ReadOnlyCollectionViewModel as a POCO view model.\r\n        /// </summary>\r\n " +
					"       /// <param name=\"unitOfWorkFactory\">A factory used to create a unit of wo" +
					"rk instance.</param>\r\n        /// <param name=\"getRepositoryFunc\">A function tha" +
					"t returns the repository representing entities of a given type.</param>\r\n       " +
					" /// <param name=\"projection\">A LINQ function used to customize a query for enti" +
					"ties. The parameter, for example, can be used for sorting data and/or for projec" +
					"ting data to a custom type that does not match the repository entity type.</para" +
					"m>\r\n        public static ReadOnlyCollectionViewModel<TEntity, TProjection, TUni" +
					"tOfWork> CreateReadOnlyProjectionCollectionViewModel(\r\n            IUnitOfWorkFa" +
					"ctory<TUnitOfWork> unitOfWorkFactory,\r\n            Func<TUnitOfWork, IReadOnlyRe" +
					"pository<TEntity>> getRepositoryFunc,\r\n            Func<IRepositoryQuery<TEntity" +
					">, IQueryable<TProjection>> projection) {\r\n            return ViewModelSource.Cr" +
					"eate(() => new ReadOnlyCollectionViewModel<TEntity, TProjection, TUnitOfWork>(un" +
					"itOfWorkFactory, getRepositoryFunc, projection));\r\n        }\r\n\r\n        /// <sum" +
					"mary>\r\n        /// Initializes a new instance of the ReadOnlyCollectionViewModel" +
					" class.\r\n        /// This constructor is declared protected to avoid an undesire" +
					"d instantiation of the PeekCollectionViewModel type without the POCO proxy facto" +
					"ry.\r\n        /// </summary>\r\n        /// <param name=\"unitOfWorkFactory\">A facto" +
					"ry used to create a unit of work instance.</param>\r\n        /// <param name=\"get" +
					"RepositoryFunc\">A function that returns the repository representing entities of " +
					"a given type.</param>\r\n        /// <param name=\"projection\">A LINQ function used" +
					" to customize a query for entities. The parameter, for example, can be used for " +
					"sorting data and/or for projecting data to a custom type that does not match the" +
					" repository entity type.</param>\r\n        protected ReadOnlyCollectionViewModel(" +
					"\r\n            IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory, \r\n            F" +
					"unc<TUnitOfWork, IReadOnlyRepository<TEntity>> getRepositoryFunc,\r\n            F" +
					"unc<IRepositoryQuery<TEntity>, IQueryable<TProjection>> projection)\r\n           " +
					" : base(unitOfWorkFactory, getRepositoryFunc, projection) {\r\n        }\r\n    }\r\n\r" +
					"\n    /// <summary>\r\n    /// The base class for POCO view models exposing a read-" +
					"only collection of entities of a given type. \r\n    /// It is not recommended to " +
					"inherit directly from this class. Use the ReadOnlyCollectionViewModel class inst" +
					"ead.\r\n    /// </summary>\r\n    /// <typeparam name=\"TEntity\">A repository entity " +
					"type.</typeparam>\r\n    /// <typeparam name=\"TProjection\">A projection entity typ" +
					"e.</typeparam>\r\n    /// <typeparam name=\"TUnitOfWork\">A unit of work type.</type" +
					"param>\r\n    [POCOViewModel]\r\n    public abstract class ReadOnlyCollectionViewMod" +
					"elBase<TEntity, TProjection, TUnitOfWork> : EntitiesViewModel<TEntity, TProjecti" +
					"on, TUnitOfWork>\r\n        where TEntity : class\r\n        where TProjection : cla" +
					"ss\r\n        where TUnitOfWork : IUnitOfWork {\r\n\r\n        /// <summary>\r\n        " +
					"/// Initializes a new instance of the ReadOnlyCollectionViewModelBase class.\r\n  " +
					"      /// </summary>\r\n        /// <param name=\"unitOfWorkFactory\">A factory used" +
					" to create a unit of work instance.</param>\r\n        /// <param name=\"getReposit" +
					"oryFunc\">A function that returns the repository representing entities of a given" +
					" type.</param>\r\n        /// <param name=\"projection\">A LINQ function used to cus" +
					"tomize a query for entities. The parameter, for example, can be used for sorting" +
					" data and/or for projecting data to a custom type that does not match the reposi" +
					"tory entity type.</param>\r\n        protected ReadOnlyCollectionViewModelBase(\r\n " +
					"           IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory, \r\n            Func" +
					"<TUnitOfWork, IReadOnlyRepository<TEntity>> getRepositoryFunc,\r\n            Func" +
					"<IRepositoryQuery<TEntity>, IQueryable<TProjection>> projection\r\n            ) :" +
					" base(unitOfWorkFactory, getRepositoryFunc, projection) {\r\n\t\t\tMessenger.Default." +
					"Register<CloseAllMessage>(this, m => {\r\n                if(m.ShouldProcess(this)" +
					") {\r\n                    SaveLayout();\r\n                }\r\n            });\r\n    " +
					"    }\r\n\r\n        /// <summary>\r\n        /// The selected enity.\r\n        /// Sin" +
					"ce ReadOnlyCollectionViewModelBase is a POCO view model, this property will rais" +
					"e INotifyPropertyChanged.PropertyEvent when modified so it can be used as a bind" +
					"ing source in views.\r\n        /// </summary>\r\n        public virtual TProjection" +
					" SelectedEntity { get; set; }\r\n\r\n        /// <summary>\r\n        /// The lambda e" +
					"xpression used to filter which entities will be loaded locally from the unit of " +
					"work.\r\n        /// Since ReadOnlyCollectionViewModelBase is a POCO view model, t" +
					"his property will raise INotifyPropertyChanged.PropertyEvent when modified so it" +
					" can be used as a binding source in views.\r\n        /// </summary>\r\n        publ" +
					"ic virtual Expression<Func<TEntity, bool>> FilterExpression { get; set; }\r\n\r\n   " +
					"     /// <summary>\r\n        /// Reloads entities.\r\n        /// Since CollectionV" +
					"iewModelBase is a POCO view model, an instance of this class will also expose th" +
					"e RefreshCommand property that can be used as a binding source in views.\r\n      " +
					"  /// </summary>\r\n        public virtual void Refresh() {\r\n            LoadEntit" +
					"ies(false);\r\n        }\r\n\r\n\t\tprotected ILayoutSerializationService LayoutSerializ" +
					"ationService { get { return this.GetService<ILayoutSerializationService>(); } }\r" +
					"\n\r\n        protected virtual string ViewName { get { return typeof(TEntity).Name" +
					" + \"ReadonlyCollectionView\"; } }\r\n\r\n\t\tbool isLoaded = false;\r\n\r\n        [Display" +
					"(AutoGenerateField = false)]\r\n        public virtual void OnLoaded() {\r\n\t\t\tisLoa" +
					"ded = true;\r\n            PersistentLayoutHelper.TryDeserializeLayout(LayoutSeria" +
					"lizationService, ViewName);\r\n        }\r\n\r\n        [Display(AutoGenerateField = f" +
					"alse)]\r\n        public virtual void OnUnloaded() {\r\n\t\t\tif(isLoaded) {\r\n\t\t\t\tSaveL" +
					"ayout();\r\n\t\t\t}\r\n        }\r\n\r\n        void SaveLayout() {\r\n            Persistent" +
					"LayoutHelper.TrySerializeLayout(LayoutSerializationService, ViewName);\r\n        " +
					"}\r\n\r\n        protected override void OnClose(CancelEventArgs e) {\r\n            S" +
					"aveLayout();\r\n\t\t\tMessenger.Default.Send(new DestroyOrphanedDocumentsMessage());\r" +
					"\n            base.OnClose(e);\r\n        }\r\n\r\n        /// <summary>\r\n        /// D" +
					"etermines whether entities can be reloaded.\r\n        /// Since CollectionViewMod" +
					"elBase is a POCO view model, this method will be used as a CanExecute callback f" +
					"or RefreshCommand.\r\n        /// </summary>\r\n        public bool CanRefresh() {\r\n" +
					"            return !IsLoading;\r\n        }\r\n\r\n        protected override void OnE" +
					"ntitiesAssigned(Func<TProjection> getSelectedEntityCallback) {\r\n            base" +
					".OnEntitiesAssigned(getSelectedEntityCallback);\r\n            SelectedEntity = ge" +
					"tSelectedEntityCallback() ?? Entities.FirstOrDefault();\r\n        }\r\n\r\n        pr" +
					"otected override Func<TProjection> GetSelectedEntityCallback() {\r\n            in" +
					"t selectedItemIndex = Entities.IndexOf(SelectedEntity);\r\n            return () =" +
					"> (selectedItemIndex >= 0 && selectedItemIndex < Entities.Count) ? Entities[sele" +
					"ctedItemIndex] : null;\r\n        }\r\n\r\n        protected override void OnIsLoading" +
					"Changed() {\r\n            base.OnIsLoadingChanged();\r\n            this.RaiseCanEx" +
					"ecuteChanged(x => x.Refresh());\r\n        }\r\n\r\n        protected virtual void OnS" +
					"electedEntityChanged() { }\r\n\r\n        protected virtual void OnFilterExpressionC" +
					"hanged() {\r\n            if(IsLoaded || IsLoading)\r\n                LoadEntities(" +
					"true);\r\n        }\r\n\r\n        protected override Expression<Func<TEntity, bool>> " +
					"GetFilterExpression() {\r\n            return FilterExpression;\r\n        }\r\n    }\r" +
					"\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class ReadOnlyCollectionViewModelTemplateBase
	{
		#region Fields
		private global::System.Text.StringBuilder generationEnvironmentField;
		private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
		private global::System.Collections.Generic.List<int> indentLengthsField;
		private string currentIndentField = "";
		private bool endsWithNewline;
		private global::System.Collections.Generic.IDictionary<string, object> sessionField;
		#endregion
		#region Properties
		protected System.Text.StringBuilder GenerationEnvironment
		{
			get
			{
				if ((this.generationEnvironmentField == null))
				{
					this.generationEnvironmentField = new global::System.Text.StringBuilder();
				}
				return this.generationEnvironmentField;
			}
			set
			{
				this.generationEnvironmentField = value;
			}
		}
		public System.CodeDom.Compiler.CompilerErrorCollection Errors
		{
			get
			{
				if ((this.errorsField == null))
				{
					this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
				}
				return this.errorsField;
			}
		}
		private System.Collections.Generic.List<int> indentLengths
		{
			get
			{
				if ((this.indentLengthsField == null))
				{
					this.indentLengthsField = new global::System.Collections.Generic.List<int>();
				}
				return this.indentLengthsField;
			}
		}
		public string CurrentIndent
		{
			get
			{
				return this.currentIndentField;
			}
		}
		public virtual global::System.Collections.Generic.IDictionary<string, object> Session
		{
			get
			{
				return this.sessionField;
			}
			set
			{
				this.sessionField = value;
			}
		}
		#endregion
		#region Transform-time helpers
		public void Write(string textToAppend)
		{
			if (string.IsNullOrEmpty(textToAppend))
			{
				return;
			}
			if (((this.GenerationEnvironment.Length == 0) 
						|| this.endsWithNewline))
			{
				this.GenerationEnvironment.Append(this.currentIndentField);
				this.endsWithNewline = false;
			}
			if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
			{
				this.endsWithNewline = true;
			}
			if ((this.currentIndentField.Length == 0))
			{
				this.GenerationEnvironment.Append(textToAppend);
				return;
			}
			textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
			if (this.endsWithNewline)
			{
				this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
			}
			else
			{
				this.GenerationEnvironment.Append(textToAppend);
			}
		}
		public void WriteLine(string textToAppend)
		{
			this.Write(textToAppend);
			this.GenerationEnvironment.AppendLine();
			this.endsWithNewline = true;
		}
		public void Write(string format, params object[] args)
		{
			this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
		}
		public void WriteLine(string format, params object[] args)
		{
			this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
		}
		public void Error(string message)
		{
			System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
			error.ErrorText = message;
			this.Errors.Add(error);
		}
		public void Warning(string message)
		{
			System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
			error.ErrorText = message;
			error.IsWarning = true;
			this.Errors.Add(error);
		}
		public void PushIndent(string indent)
		{
			if ((indent == null))
			{
				throw new global::System.ArgumentNullException("indent");
			}
			this.currentIndentField = (this.currentIndentField + indent);
			this.indentLengths.Add(indent.Length);
		}
		public string PopIndent()
		{
			string returnValue = "";
			if ((this.indentLengths.Count > 0))
			{
				int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
				this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
				if ((indentLength > 0))
				{
					returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
					this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
				}
			}
			return returnValue;
		}
		public void ClearIndent()
		{
			this.indentLengths.Clear();
			this.currentIndentField = "";
		}
		#endregion
		#region ToString Helpers
		public class ToStringInstanceHelper
		{
			private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
			public System.IFormatProvider FormatProvider
			{
				get
				{
					return this.formatProviderField ;
				}
				set
				{
					if ((value != null))
					{
						this.formatProviderField  = value;
					}
				}
			}
			public string ToStringWithCulture(object objectToConvert)
			{
				if ((objectToConvert == null))
				{
					throw new global::System.ArgumentNullException("objectToConvert");
				}
				System.Type t = objectToConvert.GetType();
				System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
							typeof(System.IFormatProvider)});
				if ((method == null))
				{
					return objectToConvert.ToString();
				}
				else
				{
					return ((string)(method.Invoke(objectToConvert, new object[] {
								this.formatProviderField })));
				}
			}
		}
		private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
		public ToStringInstanceHelper ToStringHelper
		{
			get
			{
				return this.toStringHelperField;
			}
		}
		#endregion
	}
	#endregion
}
