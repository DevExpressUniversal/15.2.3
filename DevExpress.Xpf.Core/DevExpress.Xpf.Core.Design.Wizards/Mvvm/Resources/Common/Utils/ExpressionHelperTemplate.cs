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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.Utils
{
	using System;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class ExpressionHelperTemplate : ExpressionHelperTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing Syste" +
					"m.Linq.Expressions;\r\nusing System.Reflection;\r\n\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// Provides methods to perform operations with lambda" +
					" expression trees.\r\n    /// </summary>\r\n\tpublic class ExpressionHelper {\r\n      " +
					"  class ValueHolder {\r\n            public readonly object value;\r\n            pu" +
					"blic ValueHolder(object value) {\r\n                this.value = value;\r\n         " +
					"   }\r\n        }\r\n\r\n        static readonly Dictionary<Type, object> TraitsCache " +
					"= new Dictionary<Type, object>();\r\n\r\n        static Expression GetConstExpressio" +
					"n(Type type, object value) {\r\n            return Expression.Convert(Expression.F" +
					"ield(Expression.Constant(new ValueHolder(value)), \"value\"), type);\r\n        }\r\n\r" +
					"\n        internal static bool IsTuple<T>() {\r\n            var type = typeof(T);\r" +
					"\n            return type.IsGenericType && type.GetGenericTypeDefinition().Name.S" +
					"tartsWith(\"Tuple`\");\r\n        }\r\n\r\n\t\t/// <summary>\r\n        /// If value is a Tu" +
					"ple, this method collects its items into an array of objects. For other types it" +
					" creates an array of objects with the value being its only element.\r\n        ///" +
					" </summary>\r\n        /// <typeparam name=\"T\">The type of the property. Possibly " +
					"a Tuple.</typeparam>\r\n        /// <param name=\"value\">An object from which an ar" +
					"ray of objects is created. It it supposed to be a primary key value.</param>\r\n  " +
					"      public static object[] GetKeyPropertyValues<T>(T value) {\r\n            if(" +
					"ExpressionHelper.IsTuple<T>()) {\r\n                return typeof(T).GetProperties" +
					"().Where(p => p.Name.StartsWith(\"Item\")).Select(p => p.GetValue(value, null)).To" +
					"Array();\r\n            }\r\n            return new object[] { value };\r\n        }\r\n" +
					"\r\n        /// <summary>\r\n        /// Builds a lambda expression that compares an" +
					" entity property value with a given constant value.\r\n        /// </summary>\r\n   " +
					"     /// <typeparam name=\"TPropertyOwner\">An owner type of the property.</typepa" +
					"ram>\r\n        /// <typeparam name=\"TPrimaryKey\">A primary key property type.</ty" +
					"peparam>\r\n        /// <param name=\"getKeyExpression\">A lambda expression that re" +
					"turns the property value for a given entity.</param> TODO: rewrite\r\n        /// " +
					"<param name=\"key\">A constant value to compare with entity property value.</param" +
					">\r\n        public static Expression<Func<TPropertyOwner, bool>> GetKeyEqualsExpr" +
					"ession<TGetKeyExpressionOwner, TPropertyOwner, TPrimaryKey>(Expression<Func<TGet" +
					"KeyExpressionOwner, TPrimaryKey>> getKeyExpression, TPrimaryKey key) {\r\n        " +
					"    if(key == null)\r\n                return k => false;\r\n            var entityP" +
					"aram = Expression.Parameter(typeof(TPropertyOwner));\r\n            var keyPropert" +
					"ies = GetKeyProperties(getKeyExpression);\r\n            var keyValues = GetKeyPro" +
					"pertyValues(key);\r\n            if(keyProperties.Count() != keyValues.Count())\r\n " +
					"               throw new Exception();\r\n            var propertyEqualExprs = keyP" +
					"roperties.Zip(keyValues, (p, v) => {\r\n                var constExpr = GetConstEx" +
					"pression(p.PropertyType, v);\r\n                var propertyExpr = Expression.Prop" +
					"erty(entityParam, typeof(TPropertyOwner).GetProperty(p.Name));\r\n                " +
					"return Expression.Equal(propertyExpr, constExpr);\r\n            });\r\n            " +
					"var andExpr = propertyEqualExprs.Aggregate((Expression)Expression.Constant(true)" +
					", (a, e) => Expression.And(a, e));\r\n            return Expression.Lambda<Func<TP" +
					"ropertyOwner, bool>>(andExpr, entityParam);\r\n        }\r\n\r\n        /// <summary>\r" +
					"\n        /// Returns an instance of the EntityTraits class that encapsulates ope" +
					"rations to obtain and set the primary key value of a given entity.\r\n        /// " +
					"</summary>\r\n        /// <typeparam name=\"TOwner\">A type used as a key to cache c" +
					"ompiled lambda expressions.</typeparam>\r\n        /// <typeparam name=\"TPropertyO" +
					"wner\">An owner type of the primary key property.</typeparam>\r\n        /// <typep" +
					"aram name=\"TProperty\">A primary key property type.</typeparam>\r\n        /// <par" +
					"am name=\"owner\">An instance of the TOwner type which type is used as a key to ca" +
					"che compiled lambda expressions.</param>\r\n        /// <param name=\"getPropertyEx" +
					"pression\">A lambda expression that returns the primary key value for a given ent" +
					"ity.</param>\r\n        public static EntityTraits<TPropertyOwner, TProperty> GetE" +
					"ntityTraits<TOwner, TPropertyOwner, TProperty>(TOwner owner, Expression<Func<TPr" +
					"opertyOwner, TProperty>> getPropertyExpression) {\r\n            object traits = n" +
					"ull;\r\n            if(!TraitsCache.TryGetValue(owner.GetType(), out traits)) {\r\n " +
					"               traits = new EntityTraits<TPropertyOwner, TProperty>(getPropertyE" +
					"xpression.Compile(), GetSetKeyAction(getPropertyExpression), GetHasKeyFunction(g" +
					"etPropertyExpression));\r\n                TraitsCache[owner.GetType()] = traits;\r" +
					"\n            }\r\n            return (EntityTraits<TPropertyOwner, TProperty>)trai" +
					"ts;\r\n        }\r\n\r\n        /// <summary>\r\n        /// Determines whether the give" +
					"n entity satisfies the condition represented by a lambda expression.\r\n        //" +
					"/ </summary>\r\n        /// <typeparam name=\"TEntity\">A type of the given object.<" +
					"/typeparam>\r\n        /// <param name=\"entity\">An object to test.</param>\r\n      " +
					"  /// <param name=\"predicate\">A function that determines whether the given objec" +
					"t satisfies the condition.</param>\r\n        public static bool IsFitEntity<TEnti" +
					"ty>(TEntity entity, Expression<Func<TEntity, bool>> predicate) where TEntity : c" +
					"lass {\r\n            return predicate == null || (new TEntity[] { entity }.AsQuer" +
					"yable().Any(predicate));\r\n        }\r\n\r\n\t\t/// <summary>\r\n        /// Creates an i" +
					"nstance of a generic Tuple type from items.\r\n        /// </summary>\r\n        ///" +
					" <typeparam name=\"TupleType\">A tuple type.</typeparam>\r\n        /// <param name=" +
					"\"items\">Objects that will comprise the tuple.</param>\r\n        public static Tup" +
					"leType MakeTuple<TupleType>(object[] items) {\r\n            var args = typeof(Tup" +
					"leType).GetGenericArguments();\r\n            if(args.Count() != items.Count())\r\n " +
					"               throw new Exception();\r\n            var create = typeof(Tuple).Ge" +
					"tMethods(BindingFlags.Static | BindingFlags.Public)\r\n                .First(m =>" +
					" m.Name == \"Create\" && m.GetGenericArguments().Count() == args.Count());\r\n      " +
					"      return (TupleType)create.MakeGenericMethod(args).Invoke(null, items);\r\n   " +
					"     }\r\n\r\n\t\t/// <summary>\r\n        /// Get an expression with incapsulating a la" +
					"mda that given an object of type TOwner returns the value of property propertyNa" +
					"me.\r\n        /// </summary>\r\n        /// <typeparam name=\"TOwner\">The name of ty" +
					"pe that contains the property.</typeparam>\r\n\t\t/// <typeparam name=\"TProperty\">Th" +
					"e type of a property.</typeparam>\r\n        /// <param name=\"propertyName\">The na" +
					"me of a property.</param>\r\n        public static Expression<Func<TOwner, TProper" +
					"ty>> GetPropertyExpression<TOwner, TProperty>(string propertyName) {\r\n          " +
					"  var parameter = Expression.Parameter(typeof(TOwner));\r\n            return Expr" +
					"ession.Lambda<Func<TOwner, TProperty>>(Expression.Property(parameter, propertyNa" +
					"me), parameter);\r\n        }\r\n\r\n        /// <summary>\r\n        /// Converts a pro" +
					"perty reference represented as a lambda expression to a property name.\r\n        " +
					"/// </summary>\r\n        /// <param name=\"expression\">A lambda expression that re" +
					"turns the property value.</param>\r\n        public static string GetPropertyName(" +
					"LambdaExpression expression) {\r\n            Expression body = expression.Body;\r\n" +
					"            if(body is UnaryExpression) {\r\n                body = ((UnaryExpress" +
					"ion)body).Operand;\r\n            }\r\n            var memberExpression = UnpackNull" +
					"ableMemberExpression((MemberExpression)body);\r\n            return memberExpressi" +
					"on.Member.Name;\r\n        }\r\n\r\n        static MemberExpression UnpackNullableMemb" +
					"erExpression(MemberExpression memberExpression) {\r\n            if(memberExpressi" +
					"on != null && IsNullableValueExpression(memberExpression))\r\n                memb" +
					"erExpression = (MemberExpression)memberExpression.Expression;\r\n            retur" +
					"n memberExpression;\r\n        }\r\n\r\n        static bool IsNullableValueExpression(" +
					"MemberExpression memberExpression) {\r\n            var propertyInfo = (PropertyIn" +
					"fo)memberExpression.Member;\r\n            return Nullable.GetUnderlyingType(prope" +
					"rtyInfo.ReflectedType) != null;\r\n        }\r\n\r\n\t\t/// <summary>\r\n        /// Gets " +
					"an array of PropertyInfo objects that describe the properties that comprise the " +
					"primary key of the TPropertyOwner type.\r\n        /// </summary>\r\n        /// <ty" +
					"peparam name=\"TPropertyOwner\">A type with a primary key.</typeparam>\r\n        //" +
					"/ <typeparam name=\"TProperty\">The type of the primary key. Possibly a Tuple type" +
					".</typeparam>\r\n        /// <param name=\"getPropertyExpression\">An expression tha" +
					"t when compiled and evaluated returns the value of the primary key of an TProper" +
					"tyOwner object.</param>\r\n        public static PropertyInfo[] GetKeyProperties<T" +
					"PropertyOwner, TProperty>(Expression<Func<TPropertyOwner, TProperty>> getPropert" +
					"yExpression) {\r\n            var memberExpr = UnpackNullableMemberExpression(getP" +
					"ropertyExpression.Body as MemberExpression);\r\n            var methodCallExpr = g" +
					"etPropertyExpression.Body as MethodCallExpression;\r\n            IEnumerable<stri" +
					"ng> propertyNames;\r\n            if(memberExpr != null) {\r\n                proper" +
					"tyNames = new string[] { memberExpr.Member.Name };\r\n            } else if (metho" +
					"dCallExpr != null) {\r\n                if(methodCallExpr.Method.DeclaringType != " +
					"typeof(Tuple) || methodCallExpr.Method.Name != \"Create\") {\r\n                    " +
					"throw new Exception();\r\n                }\r\n                var args = methodCall" +
					"Expr.Arguments.Cast<MemberExpression>();\r\n                propertyNames = args.S" +
					"elect(a => a.Member.Name);\r\n            } else {\r\n                propertyNames " +
					"= Enumerable.Empty<string>();\r\n            }\r\n            return propertyNames.S" +
					"elect(p => typeof(TPropertyOwner).GetProperty(p)).ToArray();\r\n        }\r\n\r\n     " +
					"   public static Action<TPropertyOwner, TProperty> GetSetKeyAction<TPropertyOwne" +
					"r, TProperty>(Expression<Func<TPropertyOwner, TProperty>> getKeyExpression) {\r\n " +
					"           var properties = GetKeyProperties(getKeyExpression);\r\n            ret" +
					"urn (o, val) => {\r\n                var values = GetKeyPropertyValues(val);\r\n    " +
					"            values.Zip(properties, (v, p) => {\r\n                    p.SetValue(o" +
					", v, null);\r\n                    return \"\";\r\n                }).ToArray();\r\n    " +
					"        };\r\n        }\r\n\r\n        static bool IsNullable(Type type) {\r\n          " +
					"  return Nullable.GetUnderlyingType(type) != null;\r\n        }\r\n\r\n        static " +
					"Func<TPropertyOwner, bool> GetHasKeyFunction<TPropertyOwner, TProperty>(Expressi" +
					"on<Func<TPropertyOwner, TProperty>> getKeyExpression) {\r\n            var propert" +
					"ies = GetKeyProperties(getKeyExpression);\r\n            return o => properties.Al" +
					"l(p => !IsNullable(p.PropertyType) || p.GetValue(o, null) != null);\r\n        }\r\n" +
					"    }\r\n\r\n    /// <summary>\r\n    /// Incapsulates operations to obtain and set th" +
					"e primary key value of a given entity.\r\n    /// </summary>\r\n    /// <typeparam n" +
					"ame=\"TEntity\">An owner type of the primary key property.</typeparam>\r\n    /// <t" +
					"ypeparam name=\"TPrimaryKey\">A primary key property type.</typeparam>\r\n    public" +
					" class EntityTraits<TEntity, TPrimaryKey> {\r\n\r\n        /// <summary>\r\n        //" +
					"/ Initializes a new instance of EntityTraits class.\r\n        /// </summary>\r\n   " +
					"     /// <param name=\"getPrimaryKeyFunction\">A function that returns the primary" +
					" key value of a given entity.</param>\r\n        /// <param name=\"setPrimaryKeyAct" +
					"ion\">An action that assigns the primary key value to a given entity.</param>\r\n  " +
					"      /// <param name=\"hasPrimaryKeyFunction\">A function that determines whether" +
					" given the entity has a primary key assigned.</param>\r\n        public EntityTrai" +
					"ts(Func<TEntity, TPrimaryKey> getPrimaryKeyFunction, Action<TEntity, TPrimaryKey" +
					"> setPrimaryKeyAction, Func<TEntity, bool> hasPrimaryKeyFunction) {\r\n           " +
					" this.GetPrimaryKey = getPrimaryKeyFunction;\r\n            this.SetPrimaryKey = s" +
					"etPrimaryKeyAction;\r\n            this.HasPrimaryKey = hasPrimaryKeyFunction;\r\n  " +
					"      }\r\n\r\n        /// <summary>\r\n        /// The function that returns the prim" +
					"ary key value of a given entity.\r\n        /// </summary>\r\n        public Func<TE" +
					"ntity, TPrimaryKey> GetPrimaryKey { get; private set; }\r\n\r\n        /// <summary>" +
					"\r\n        /// The action that assigns the primary key value to a given entity.\r\n" +
					"        /// </summary>\r\n        public Action<TEntity, TPrimaryKey> SetPrimaryKe" +
					"y { get; private set; }\r\n\r\n        /// <summary>\r\n        /// A function that de" +
					"termines whether the given entity has a primary key assigned (the primary key is" +
					" not null). Always returns true if the primary key is a non-nullable value type." +
					"\r\n        /// </summary>\r\n        /// <returns></returns>\r\n        public Func<T" +
					"Entity, bool> HasPrimaryKey { get; private set; }\r\n    }\r\n}\r\n");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class ExpressionHelperTemplateBase
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
