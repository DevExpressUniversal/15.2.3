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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.DataModel
{
	using System;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class IRepositoryTemplate : IRepositoryTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.Linq;\r\nusing System.Linq.Expressions;\r\nusing System.C" +
					"ollections.Generic;\r\nusing System.Reflection;\r\nusing System.ComponentModel;\r\n");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n        /// <summary>\r\n    /// The IRepository interface represents the read " +
					"and write implementation of the Repository pattern \r\n    /// such that it can be" +
					" used to query entities of a given type. \r\n    /// </summary>\r\n    /// <typepara" +
					"m name=\"TEntity\">A repository entity type.</typeparam>\r\n    /// <typeparam name=" +
					"\"TPrimaryKey\">An entity primary key type.</typeparam>\r\n    public interface IRep" +
					"ository<TEntity, TPrimaryKey> : IReadOnlyRepository<TEntity> where TEntity : cla" +
					"ss {\r\n\r\n        /// <summary>\r\n        /// Finds an entity with the given primar" +
					"y key value. \r\n        /// If an entity with the given primary key value exists " +
					"in the unit of work, then it is returned immediately without making a request to" +
					" the store. \r\n        /// Otherwise, a request is made to the store for an entit" +
					"y with the given primary key value and this entity, if found, is attached to the" +
					" unit of work and returned. \r\n        /// If no entity is found in the unit of w" +
					"ork or the store, then null is returned.\r\n        /// </summary>\r\n        /// <p" +
					"aram name=\"primaryKey\">The value of the primary key for the entity to be found.<" +
					"/param>\r\n        TEntity Find(TPrimaryKey primaryKey);\r\n\r\n        /// <summary>\r" +
					"\n        /// Marks the given entity as Added such that it will be commited to th" +
					"e store when IUnitOfWork.SaveChanges is called.\r\n        /// </summary>\r\n       " +
					" /// <param name=\"entity\">The entity to add.</param>\r\n\t\tvoid Add(TEntity entity)" +
					";\r\n\r\n        /// <summary>\r\n        /// Marks the given entity as Deleted such t" +
					"hat it will be deleted from the store when IUnitOfWork.SaveChanges is called. \r\n" +
					"        /// Note that the entity must exist in the unit of work in some other st" +
					"ate before this method is called.\r\n        /// </summary>\r\n        /// <param na" +
					"me=\"entity\">The entity to remove.</param>\r\n        void Remove(TEntity entity);\r" +
					"\n\r\n        /// <summary>\r\n        /// Creates a new instance of the entity type." +
					"\r\n        /// </summary>\r\n\t\t/// <param name=\"add\">A flag determining if the newl" +
					"y created entity is added to the repository.</param>\r\n        TEntity Create(boo" +
					"l add = true);\r\n\r\n        /// <summary>\r\n        /// Returns the state of the gi" +
					"ven entity.\r\n        /// </summary>\r\n        /// <param name=\"entity\">An entity " +
					"to get state from</param>\r\n        EntityState GetState(TEntity entity);\r\n\r\n    " +
					"    /// <summary>\r\n        /// Changes the state of the specified entity to Modi" +
					"fied if changes are not automatically tracked by the implementation.\r\n        //" +
					"/ </summary>\r\n        /// <param name=\"entity\">An entity which state should be u" +
					"pdated/</param>\r\n        void Update(TEntity entity);\r\n\r\n        /// <summary>\r\n" +
					"        /// Reloads the entity from the store overwriting any property values wi" +
					"th values from the store and returns a reloaded entity. \r\n        /// This metho" +
					"d returns the same entity instance with updated properties or new one depending " +
					"on the implementation.\r\n        /// The entity will be in the Unchanged state af" +
					"ter calling this method.\r\n        /// </summary>\r\n        /// <param name=\"entit" +
					"y\">An entity to reload.</param>\r\n        TEntity Reload(TEntity entity);\r\n\r\n    " +
					"    /// <summary>\r\n        /// The lambda-expression that returns the entity pri" +
					"mary key.\r\n        /// </summary>\r\n        Expression<Func<TEntity, TPrimaryKey>" +
					"> GetPrimaryKeyExpression { get; }\r\n\r\n        /// <summary>\r\n        /// Returns" +
					" the primary key value for the entity.\r\n        /// </summary>\r\n        /// <par" +
					"am name=\"entity\">An entity for which to obtain a primary key value.</param>\r\n   " +
					"     TPrimaryKey GetPrimaryKey(TEntity entity);\r\n\r\n        /// <summary>\r\n      " +
					"  /// Determines whether the given entity has the primary key assigned (the prim" +
					"ary key is not null). Always returns true if the primary key is a non-nullable v" +
					"alue type.\r\n        /// </summary>\r\n        /// <param name=\"entity\">An entity t" +
					"o test.</param>\r\n        bool HasPrimaryKey(TEntity entity);\r\n\r\n        /// <sum" +
					"mary>\r\n        /// Assigns the given primary key value to a given entity.\r\n     " +
					"   /// </summary>\r\n        /// <param name=\"entity\">An entity to which to assign" +
					" the primary key value.</param>\r\n        /// <param name=\"primaryKey\">A primary " +
					"key value</param>\r\n        void SetPrimaryKey(TEntity entity, TPrimaryKey primar" +
					"yKey);\r\n    }\r\n\r\n    /// <summary>\r\n    /// Provides a set of extension methods " +
					"to perform commonly used operations with IRepository.\r\n    /// </summary>\r\n    p" +
					"ublic static class RepositoryExtensions {\r\n        /// <summary>\r\n        /// Bu" +
					"ilds a lambda expression that compares an entity primary key with the given cons" +
					"tant value.\r\n        /// </summary>\r\n        /// <typeparam name=\"TEntity\">A rep" +
					"ository entity type.</typeparam>\r\n        /// <typeparam name=\"TProjection\">A pr" +
					"ojection entity type.</typeparam>\r\n        /// <typeparam name=\"TPrimaryKey\">An " +
					"entity primary key type.</typeparam>\r\n        /// <param name=\"repository\">A rep" +
					"ository.</param>\r\n        /// <param name=\"primaryKey\">A value to compare with t" +
					"he entity primary key.</param>\r\n        public static Expression<Func<TProjectio" +
					"n, bool>> GetProjectionPrimaryKeyEqualsExpression<TEntity, TProjection, TPrimary" +
					"Key>(this IRepository<TEntity, TPrimaryKey> repository, TPrimaryKey primaryKey) " +
					"where TEntity : class {\r\n            return ExpressionHelper.GetKeyEqualsExpress" +
					"ion<TEntity, TProjection, TPrimaryKey>(repository.GetPrimaryKeyExpression, prima" +
					"ryKey);\r\n        }\r\n\r\n        /// <summary>\r\n        /// Returns a primary key o" +
					"f the given entity.\r\n        /// </summary>\r\n        /// <typeparam name=\"TEntit" +
					"y\">A repository entity type.</typeparam>\r\n        /// <typeparam name=\"TProjecti" +
					"on\">A projection entity type.</typeparam>\r\n        /// <typeparam name=\"TPrimary" +
					"Key\">An entity primary key type.</typeparam>\r\n        /// <param name=\"repositor" +
					"y\">A repository.</param>\r\n        /// <param name=\"projectionEntity\">An entity.<" +
					"/param>\r\n        public static TPrimaryKey GetProjectionPrimaryKey<TEntity, TPro" +
					"jection, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TProjec" +
					"tion projectionEntity) where TEntity : class {\r\n            return GetProjection" +
					"Value(projectionEntity,\r\n                (TEntity x) => {\r\n                    i" +
					"f(repository.HasPrimaryKey(x)) {\r\n                        return repository.GetP" +
					"rimaryKey(x);\r\n                    }\r\n                    return default(TPrimar" +
					"yKey);\r\n                },\r\n                (TProjection x) => GetProjectionKey(" +
					"repository, x));\r\n        }\r\n\r\n        static TPrimaryKey GetProjectionKey<TEnti" +
					"ty, TProjection, TPrimaryKey>(IRepository<TEntity, TPrimaryKey> repository, TPro" +
					"jection projection) where TEntity : class {\r\n            var properties = Expres" +
					"sionHelper.GetKeyProperties(repository.GetPrimaryKeyExpression);\r\n            if" +
					"(ExpressionHelper.IsTuple<TPrimaryKey>()) {\r\n                var objects = prope" +
					"rties.Select(p => p.GetValue(projection, null));\r\n                return Express" +
					"ionHelper.MakeTuple<TPrimaryKey>(objects.ToArray());\r\n            }\r\n           " +
					" var property = properties.Single();\r\n            return (TPrimaryKey)projection" +
					".GetType().GetProperty(property.Name).GetValue(projection, null);\r\n        }\r\n\r\n" +
					"        static void SetProjectionKey<TEntity, TProjection, TPrimaryKey>(IReposit" +
					"ory<TEntity, TPrimaryKey> repository, TProjection projectionEntity, TPrimaryKey " +
					"primaryKey) where TEntity : class {\r\n            var properties = ExpressionHelp" +
					"er.GetKeyProperties(repository.GetPrimaryKeyExpression);\r\n            var values" +
					" = ExpressionHelper.GetKeyPropertyValues(primaryKey);\r\n            if(properties" +
					".Count() != values.Count())\r\n                throw new Exception();\r\n           " +
					" for (int i = 0; i < values.Count(); i++) {\r\n                var projectionPrope" +
					"rty = typeof(TProjection).GetProperty(properties[i].Name);\r\n                proj" +
					"ectionProperty.SetValue(projectionEntity, values[i], null);\r\n            }\r\n    " +
					"    }\r\n\r\n\t\tpublic static Expression<Func<TProjection, TPrimaryKey>> GetSinglePro" +
					"pertyPrimaryKeyProjectionProperty<TEntity, TProjection, TPrimaryKey>(this IRepos" +
					"itory<TEntity, TPrimaryKey> repository) where TEntity : class {\r\n            var" +
					" properties = ExpressionHelper.GetKeyProperties(repository.GetPrimaryKeyExpressi" +
					"on);\r\n            var propertyName = properties.Single().Name;\r\n            var " +
					"parameter = Expression.Parameter(typeof(TProjection));\r\n            return Expre" +
					"ssion.Lambda<Func<TProjection, TPrimaryKey>>(Expression.Property(parameter, prop" +
					"ertyName), parameter);\r\n        }\r\n\r\n        public static void VerifyProjection" +
					"<TEntity, TProjection, TPrimaryKey>(IRepository<TEntity, TPrimaryKey> repository" +
					", Func<IRepositoryQuery<TEntity>, IQueryable<TProjection>> projection) where TEn" +
					"tity : class {\r\n            if(typeof(TProjection) != typeof(TEntity) && project" +
					"ion == null)\r\n                throw new ArgumentException(\"Projection should not" +
					" be null when its type is different from TEntity.\");\r\n            var entityKeyP" +
					"roperties = ExpressionHelper.GetKeyProperties(repository.GetPrimaryKeyExpression" +
					");\r\n            var projectionKeyPropertyCount = entityKeyProperties.Count(p => " +
					"{\r\n\t\t\t\tvar properties = TypeDescriptor.GetProperties(typeof(TProjection));\r\n    " +
					"            var property = properties[p.Name];\r\n\t\t\t\treturn property != null;\r\n  " +
					"          });\r\n            if(projectionKeyPropertyCount != entityKeyProperties." +
					"Count()) {\r\n                string tprojectionName = typeof(TProjection).Name;\r\n" +
					"                string message = string.Format(\"Projection type {0} should have " +
					"the same primary key as its corresponding entity\", tprojectionName);\r\n          " +
					"      throw new ArgumentException(message, tprojectionName);\r\n            }\r\n   " +
					"     }\r\n\r\n        /// <summary>\r\n        /// Sets the primary key of a given pro" +
					"jection.\r\n        /// </summary>\r\n        /// <typeparam name=\"TEntity\">A reposi" +
					"tory entity type.</typeparam>\r\n        /// <typeparam name=\"TProjection\">A proje" +
					"ction entity type.</typeparam>\r\n        /// <typeparam name=\"TPrimaryKey\">An ent" +
					"ity primary key type.</typeparam>\r\n        /// <param name=\"repository\">A reposi" +
					"tory.</param>\r\n        /// <param name=\"projectionEntity\">A projection.</param>\r" +
					"\n        /// <param name=\"primaryKey\">A new primary key value.</param>\r\n        " +
					"public static void SetProjectionPrimaryKey<TEntity, TProjection, TPrimaryKey>(th" +
					"is IRepository<TEntity, TPrimaryKey> repository, TProjection projectionEntity, T" +
					"PrimaryKey primaryKey) where TEntity : class {\r\n            if(IsProjection<TEnt" +
					"ity, TProjection>(projectionEntity)) {\r\n                SetProjectionKey<TEntity" +
					", TProjection, TPrimaryKey>(repository, projectionEntity, primaryKey);\r\n        " +
					"    } else {\r\n                repository.SetPrimaryKey(projectionEntity as TEnti" +
					"ty, primaryKey);\r\n            }\r\n        }\r\n\r\n        /// <summary>\r\n        ///" +
					" Given a projection, this function returns the corresponding entity. \r\n        /" +
					"// If the projection has no corresponding entity, a new entity is created and ad" +
					"ded to the repository.\r\n        /// Before the new entity is returned, the apply" +
					"ProjectionPropertiesToEntity action is used to transfer property values from the" +
					" projection to the entity.\r\n        /// </summary>\r\n        /// <typeparam name=" +
					"\"TEntity\">A repository entity type.</typeparam>\r\n        /// <typeparam name=\"TP" +
					"rojection\">A projection entity type.</typeparam>\r\n        /// <typeparam name=\"T" +
					"PrimaryKey\">An entity primary key type.</typeparam>\r\n        /// <param name=\"re" +
					"pository\">A repository.</param>\r\n        /// <param name=\"projectionEntity\">A pr" +
					"ojection.</param>\r\n        /// <param name=\"applyProjectionPropertiesToEntity\">A" +
					"n action which applies the projection properties to the newly created entity.</p" +
					"aram>\t\t\r\n        public static TEntity FindExistingOrAddNewEntity<TEntity, TProj" +
					"ection, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TProject" +
					"ion projectionEntity, Action<TProjection, TEntity> applyProjectionPropertiesToEn" +
					"tity) where TEntity : class {\r\n            bool projection = IsProjection<TEntit" +
					"y, TProjection>(projectionEntity);\r\n            var entity = repository.Find(rep" +
					"ository.GetProjectionPrimaryKey(projectionEntity));\r\n            if(entity == nu" +
					"ll) {\r\n                if(projection) {\r\n                    entity = repository" +
					".Create();\r\n                } else {\r\n                    entity = projectionEnt" +
					"ity as TEntity;\r\n                    repository.Add(entity);\r\n                }\r" +
					"\n            }\r\n            if(projection) {\r\n                applyProjectionPro" +
					"pertiesToEntity(projectionEntity, entity);\r\n            }\r\n            return en" +
					"tity;\r\n        }\r\n\r\n        /// <summary>\r\n        /// Gets whether the given en" +
					"tity is detached from the unit of work.\r\n        /// </summary>\r\n        /// <ty" +
					"peparam name=\"TEntity\">A repository entity type.</typeparam>\r\n        /// <typep" +
					"aram name=\"TProjection\">A projection entity type.</typeparam>\r\n        /// <type" +
					"param name=\"TPrimaryKey\">An entity primary key type.</typeparam>\r\n        /// <p" +
					"aram name=\"repository\">A repository.</param>\r\n        /// <param name=\"projectio" +
					"nEntity\">An entity.</param>\r\n        public static bool IsDetached<TEntity, TPro" +
					"jection, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TProjec" +
					"tion projectionEntity) where TEntity : class {\r\n            return GetProjection" +
					"Value(projectionEntity,\r\n                (TEntity x) => repository.GetState(x) =" +
					"= EntityState.Detached,\r\n                (TProjection x) => false);\r\n        }\r\n" +
					"\r\n        /// <summary>\r\n        /// Determines whether the given entity has the" +
					" primary key assigned (the primary key is not null). Always returns true if the " +
					"primary key is a non-nullable value type.\r\n        /// </summary>\r\n        /// <" +
					"typeparam name=\"TEntity\">A repository entity type.</typeparam>\r\n        /// <typ" +
					"eparam name=\"TProjection\">A projection entity type.</typeparam>\r\n        /// <ty" +
					"peparam name=\"TPrimaryKey\">An entity primary key type.</typeparam>\r\n        /// " +
					"<param name=\"repository\">A repository.</param>\r\n        /// <param name=\"project" +
					"ionEntity\">An entity.</param>\r\n        public static bool ProjectionHasPrimaryKe" +
					"y<TEntity, TProjection, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repo" +
					"sitory, TProjection projectionEntity) where TEntity : class {\r\n            retur" +
					"n GetProjectionValue(projectionEntity,\r\n                (TEntity x) => repositor" +
					"y.HasPrimaryKey(x),\r\n                (TProjection x) => true);\r\n        }\r\n\r\n   " +
					"     /// <summary>\r\n        /// Loads from the store or updates an entity with t" +
					"he given primary key value. If no entity with the given primary key is found in " +
					"the store, returns null.\r\n        /// </summary>\r\n        /// <typeparam name=\"T" +
					"Entity\">A repository entity type.</typeparam>\r\n        /// <typeparam name=\"TPro" +
					"jection\">A projection entity type.</typeparam>\r\n        /// <typeparam name=\"TPr" +
					"imaryKey\">An entity primary key type.</typeparam>\r\n        /// <param name=\"repo" +
					"sitory\">A repository.</param>\r\n        /// <param name=\"projection\">A LINQ funct" +
					"ion used to transform entities from the repository entity type to the projection" +
					" entity type.</param>\r\n        /// <param name=\"primaryKey\">A value to compare w" +
					"ith the entity primary key.</param>\r\n        public static TProjection FindActua" +
					"lProjectionByKey<TEntity, TProjection, TPrimaryKey>(this IRepository<TEntity, TP" +
					"rimaryKey> repository, Func<IRepositoryQuery<TEntity>, IQueryable<TProjection>> " +
					"projection, TPrimaryKey primaryKey) where TEntity : class {\r\n            var pri" +
					"maryKeyEqualsExpression = GetProjectionPrimaryKeyEqualsExpression<TEntity, TProj" +
					"ection, TPrimaryKey>(repository, primaryKey);\r\n            var result = reposito" +
					"ry.GetFilteredEntities(null, projection).Where(primaryKeyEqualsExpression).Take(" +
					"1).ToArray().FirstOrDefault(); //WCF incorrect FirstOrDefault implementation wor" +
					"karound\r\n            return GetProjectionValue(result,\r\n                (TEntity" +
					" x) => x != null ? repository.Reload(x) : null,\r\n                (TProjection x)" +
					" => x);\r\n        }\r\n        \r\n        static TProjectionResult GetProjectionValu" +
					"e<TEntity, TProjection, TEntityResult, TProjectionResult>(TProjection value, Fun" +
					"c<TEntity, TEntityResult> entityFunc, Func<TProjection, TProjectionResult> proje" +
					"ctionFunc) {\r\n            if(typeof(TEntity) != typeof(TProjection) || typeof(TE" +
					"ntityResult) != typeof(TProjectionResult))\r\n                return projectionFun" +
					"c(value);\r\n            return (TProjectionResult)(object)entityFunc((TEntity)(ob" +
					"ject)value);\r\n        }\r\n\r\n        static bool IsProjection<TEntity, TProjection" +
					">(TProjection projection) {\r\n            return !(projection is TEntity);\r\n     " +
					"   }\r\n    }\r\n}\r\n");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class IRepositoryTemplateBase
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
