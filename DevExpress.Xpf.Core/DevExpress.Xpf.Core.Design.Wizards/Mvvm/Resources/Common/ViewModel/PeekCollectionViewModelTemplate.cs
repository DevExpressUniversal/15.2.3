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
	public partial class PeekCollectionViewModelTemplate : PeekCollectionViewModelTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write(@"using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataAnnotations;
");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// A POCO view model exposing a read-only collection " +
					"of entities of a given type. It is designed for quick navigation between collect" +
					"ion views.\r\n    /// This is a partial class that provides an extension point to " +
					"add custom properties, commands and override methods without modifying the auto-" +
					"generated code.\r\n    /// </summary>\r\n    /// <typeparam name=\"TNavigationToken\">" +
					"A navigation token type.</typeparam>\r\n    /// <typeparam name=\"TEntity\">An entit" +
					"y type.</typeparam>\r\n    /// <typeparam name=\"TPrimaryKey\">A primary key value t" +
					"ype.</typeparam>\r\n    /// <typeparam name=\"TUnitOfWork\">A unit of work type.</ty" +
					"peparam>\r\n    public partial class PeekCollectionViewModel<TNavigationToken, TEn" +
					"tity, TPrimaryKey, TUnitOfWork> : CollectionViewModelBase<TEntity, TEntity, TPri" +
					"maryKey, TUnitOfWork>\r\n        where TEntity : class\r\n        where TUnitOfWork " +
					": IUnitOfWork {\r\n\r\n        /// <summary>\r\n        /// Creates a new instance of " +
					"PeekCollectionViewModel as a POCO view model.\r\n        /// </summary>\r\n        /" +
					"// <param name=\"navigationToken\">Identifies the module that is the navigation ta" +
					"rget.</param>\r\n        /// <param name=\"unitOfWorkFactory\">A factory that is use" +
					"d to create a unit of work instance.</param>\r\n        /// <param name=\"getReposi" +
					"toryFunc\">A function that returns a repository representing entities of a given " +
					"type.</param>\r\n        /// <param name=\"projection\">An optional parameter that p" +
					"rovides a LINQ function used to customize a query for entities. The parameter, f" +
					"or example, can be used for sorting data.</param>\r\n        public static PeekCol" +
					"lectionViewModel<TNavigationToken, TEntity, TPrimaryKey, TUnitOfWork> Create(\r\n " +
					"           TNavigationToken navigationToken,\r\n            IUnitOfWorkFactory<TUn" +
					"itOfWork> unitOfWorkFactory,\r\n            Func<TUnitOfWork, IRepository<TEntity," +
					" TPrimaryKey>> getRepositoryFunc,\r\n            Func<IRepositoryQuery<TEntity>, I" +
					"Queryable<TEntity>> projection = null) {\r\n            return ViewModelSource.Cre" +
					"ate(() => new PeekCollectionViewModel<TNavigationToken, TEntity, TPrimaryKey, TU" +
					"nitOfWork>(navigationToken, unitOfWorkFactory, getRepositoryFunc, projection));\r" +
					"\n        }\r\n\r\n        TNavigationToken navigationToken;\r\n\t\tTEntity pickedEntity;" +
					"\r\n\r\n        /// <summary>\r\n        /// Initializes a new instance of the PeekCol" +
					"lectionViewModel class.\r\n        /// This constructor is declared protected to a" +
					"void an undesired instantiation of the PeekCollectionViewModel type without the " +
					"POCO proxy factory.\r\n        /// </summary>\r\n        /// <param name=\"navigation" +
					"Token\">Identifies the module that is the navigation target.</param>\r\n        ///" +
					" <param name=\"unitOfWorkFactory\">A factory that is used to create a unit of work" +
					" instance.</param>\r\n        /// <param name=\"getRepositoryFunc\">A function that " +
					"returns a repository representing entities of a given type.</param>\r\n        ///" +
					" <param name=\"projection\">An optional parameter that provides a LINQ function us" +
					"ed to customize a query for entities. The parameter, for example, can be used fo" +
					"r sorting data.</param>\r\n        protected PeekCollectionViewModel(\r\n           " +
					" TNavigationToken navigationToken,\r\n            IUnitOfWorkFactory<TUnitOfWork> " +
					"unitOfWorkFactory,\r\n            Func<TUnitOfWork, IRepository<TEntity, TPrimaryK" +
					"ey>> getRepositoryFunc,\r\n            Func<IRepositoryQuery<TEntity>, IQueryable<" +
					"TEntity>> projection = null\r\n            ) : base(unitOfWorkFactory, getReposito" +
					"ryFunc, projection, null, null, true) {\r\n            this.navigationToken = navi" +
					"gationToken;\r\n        }\r\n\r\n        /// <summary>\r\n        /// Navigates to the c" +
					"orresponding collection view and selects the given entity.\r\n        /// Since Pe" +
					"ekCollectionViewModel is a POCO view model, an instance of this class will also " +
					"expose the NavigateCommand property that can be used as a binding source in view" +
					"s.\r\n        /// </summary>\r\n        /// <param name=\"projectionEntity\">An entity" +
					" to select within the collection view.</param>\r\n        [Display(AutoGenerateFie" +
					"ld = false)]\r\n        public void Navigate(TEntity projectionEntity) {\r\n\t\t\tpicke" +
					"dEntity = projectionEntity;\r\n\t\t\tSendSelectEntityMessage();\r\n            Messenge" +
					"r.Default.Send(new NavigateMessage<TNavigationToken>(navigationToken), navigatio" +
					"nToken);\r\n        }\r\n\r\n        /// <summary>\r\n        /// Determines if a naviga" +
					"tion to corresponding collection view can be performed.\r\n        /// Since PeekC" +
					"ollectionViewModel is a POCO view model, this method will be used as a CanExecut" +
					"e callback for NavigateCommand.\r\n        /// </summary>\r\n        /// <param name" +
					"=\"projectionEntity\">An entity to select in the collection view.</param>\r\n       " +
					" public bool CanNavigate(TEntity projectionEntity) {\r\n            return project" +
					"ionEntity != null;\r\n        }\r\n\r\n        protected override void OnInitializeInR" +
					"untime() {\r\n            base.OnInitializeInRuntime();\r\n            Messenger.Def" +
					"ault.Register<SelectedEntityRequest>(this, x => SendSelectEntityMessage());\r\n   " +
					"     }\r\n\r\n        void SendSelectEntityMessage() {\r\n            if(IsLoaded && p" +
					"ickedEntity != null)\r\n                Messenger.Default.Send(new SelectEntityMes" +
					"sage(CreateRepository().GetProjectionPrimaryKey(pickedEntity)));\r\n\t\t}\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class PeekCollectionViewModelTemplateBase
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
