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
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class MessagesTemplate : MessagesTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.Linq;\r\nusing System.ComponentModel;\r\n\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// Represents the type of an entity state change noti" +
					"fication that is shown when the IUnitOfWork.SaveChanges method has been called.\r" +
					"\n    /// </summary>\r\n    public enum EntityMessageType {\r\n\r\n        /// <summary" +
					">\r\n        /// A new entity has been added to the unit of work. \r\n        /// </" +
					"summary>\r\n        Added,\r\n\r\n        /// <summary>\r\n        /// An entity has bee" +
					"n removed from the unit of work.\r\n        /// </summary>\r\n        Deleted,\r\n\r\n  " +
					"      /// <summary>\r\n        /// One of the entity properties has been modified." +
					" \r\n        /// </summary>\r\n        Changed\r\n    }\r\n\r\n    /// <summary>\r\n    /// " +
					"Provides the information about an entity state change notification that is shown" +
					" when an entity has been added, removed or modified, and the IUnitOfWork.SaveCha" +
					"nges method has been called.\r\n    /// </summary>\r\n    /// <typeparam name=\"TEnti" +
					"ty\">An entity type.</typeparam>\r\n    /// <typeparam name=\"TPrimaryKey\">A primary" +
					" key value type.</typeparam>\r\n    public class EntityMessage<TEntity, TPrimaryKe" +
					"y> {\r\n\r\n        /// <summary>\r\n        /// Initializes a new instance of the Ent" +
					"ityMessage class.\r\n        /// </summary>\r\n        /// <param name=\"primaryKey\">" +
					"A primary key of an entity that has been added, removed or modified.</param>\r\n  " +
					"      /// <param name=\"messageType\">An entity state change notification type.</p" +
					"aram>\r\n        /// /// <param name=\"sender\">The message sender.</param>\r\n       " +
					" public EntityMessage(TPrimaryKey primaryKey, EntityMessageType messageType, obj" +
					"ect sender = null) {\r\n            this.PrimaryKey = primaryKey;\r\n            thi" +
					"s.MessageType = messageType;\r\n            this.Sender = sender;\r\n        }\r\n\r\n  " +
					"      /// <summary>\r\n        /// The primary key of entity that has been added, " +
					"deleted or modified.\r\n        /// </summary>\r\n        public TPrimaryKey Primary" +
					"Key { get; private set; }\r\n\r\n        /// <summary>\r\n        /// The entity state" +
					" change notification type.\r\n        /// </summary>\r\n        public EntityMessage" +
					"Type MessageType { get; private set; }\r\n\r\n        /// <summary>\r\n        /// The" +
					" message sender.\r\n        /// </summary>\r\n        public object Sender { get; pr" +
					"ivate set; }\r\n    }\r\n\r\n    /// <summary>\r\n    /// A message notifying that all v" +
					"iew models should save changes. Usually sent by DocumentsViewModel when the Save" +
					"All command is executed.\r\n    /// </summary>\r\n\tpublic class SaveAllMessage {\r\n  " +
					"  }\r\n\r\n    /// <summary>\r\n    /// A message notifying that all view models shoul" +
					"d close itself. Usually sent by DocumentsViewModel when the CloseAll command is " +
					"executed.\r\n    /// </summary>\r\n    public class CloseAllMessage {\r\n\r\n        rea" +
					"donly CancelEventArgs cancelEventArgs;\r\n\t\tFunc<object, bool> viewModelPredicate;" +
					"\r\n\r\n        /// <summary>\r\n        /// Initializes a new instance of the CloseAl" +
					"lMessage class.\r\n        /// </summary>\r\n        /// <param name=\"cancelEventArg" +
					"s\">An argument of the System.ComponentModel.CancelEventArgs type which can be us" +
					"ed to cancel closing.</param>\r\n        public CloseAllMessage(CancelEventArgs ca" +
					"ncelEventArgs, Func<object, bool> viewModelPredicate) {\r\n            this.cancel" +
					"EventArgs = cancelEventArgs;\r\n\t\t\tthis.viewModelPredicate = viewModelPredicate;\r\n" +
					"        }\r\n\r\n\t\tpublic bool ShouldProcess(object viewModel) {\r\n            return" +
					" viewModelPredicate(viewModel);\r\n        }\r\n\r\n        /// <summary>\r\n        ///" +
					" Used to cancel closing and check whether the closing has already been cancelled" +
					".\r\n        /// </summary>\r\n        public bool Cancel {\r\n            get { retur" +
					"n cancelEventArgs.Cancel; }\r\n            set { cancelEventArgs.Cancel = value; }" +
					"\r\n        }\r\n    }\r\n\r\n\tpublic class DestroyOrphanedDocumentsMessage { }\r\n\r\n    /" +
					"// <summary>\r\n    /// Used by the PeekCollectionViewModel to notify that Documen" +
					"tsViewModel should navigate to the specified module.\r\n    /// </summary>\r\n    //" +
					"/ <typeparam name=\"TNavigationToken\">The navigation token type.</typeparam>\r\n\tpu" +
					"blic class NavigateMessage<TNavigationToken> {\r\n\r\n        /// <summary>\r\n       " +
					" /// Initializes a new instance of the NavigateMessage class.\r\n        /// </sum" +
					"mary>\r\n        /// <param name=\"token\">An object that is used to identify the mo" +
					"dule to which the DocumentsViewModel should navigate.</param>\r\n        public Na" +
					"vigateMessage(TNavigationToken token) {\r\n            Token = token;\r\n        }\r\n" +
					"\r\n        /// <summary>\r\n        /// An object that is used to identify the modu" +
					"le to which the DocumentsViewModel should navigate.\r\n        /// </summary>\r\n   " +
					"     public TNavigationToken Token { get; private set; }\r\n\t}\r\n}\r\n");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class MessagesTemplateBase
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
