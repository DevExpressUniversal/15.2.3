﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common
{
	using System.Linq;
	using System.Text;
	using System.Collections.Generic;
	using System;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class CommonResourcesTemplate : CommonResourcesTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<root>\r\n  <!-- \r\n    Microsoft ResX Schem" +
					"a \r\n    \r\n    Version 2.0\r\n    \r\n    The primary goals of this format is to allo" +
					"w a simple XML format \r\n    that is mostly human readable. The generation and pa" +
					"rsing of the \r\n    various data types are done through the TypeConverter classes" +
					" \r\n    associated with the data types.\r\n    \r\n    Example:\r\n    \r\n    ... ado.ne" +
					"t/XML headers & schema ...\r\n    <resheader name=\"resmimetype\">text/microsoft-res" +
					"x</resheader>\r\n    <resheader name=\"version\">2.0</resheader>\r\n    <resheader nam" +
					"e=\"reader\">System.Resources.ResXResourceReader, System.Windows.Forms, ...</reshe" +
					"ader>\r\n    <resheader name=\"writer\">System.Resources.ResXResourceWriter, System." +
					"Windows.Forms, ...</resheader>\r\n    <data name=\"Name1\"><value>this is my long st" +
					"ring</value><comment>this is a comment</comment></data>\r\n    <data name=\"Color1\"" +
					" type=\"System.Drawing.Color, System.Drawing\">Blue</data>\r\n    <data name=\"Bitmap" +
					"1\" mimetype=\"application/x-microsoft.net.object.binary.base64\">\r\n        <value>" +
					"[base64 mime encoded serialized .NET Framework object]</value>\r\n    </data>\r\n   " +
					" <data name=\"Icon1\" type=\"System.Drawing.Icon, System.Drawing\" mimetype=\"applica" +
					"tion/x-microsoft.net.object.bytearray.base64\">\r\n        <value>[base64 mime enco" +
					"ded string representing a byte array form of the .NET Framework object]</value>\r" +
					"\n        <comment>This is a comment</comment>\r\n    </data>\r\n                \r\n  " +
					"  There are any number of \"resheader\" rows that contain simple \r\n    name/value " +
					"pairs.\r\n    \r\n    Each data row contains a name, and value. The row also contain" +
					"s a \r\n    type or mimetype. Type corresponds to a .NET class that support \r\n    " +
					"text/value conversion through the TypeConverter architecture. \r\n    Classes that" +
					" don\'t support this are serialized and stored with the \r\n    mimetype set.\r\n    " +
					"\r\n    The mimetype is used for serialized objects, and tells the \r\n    ResXResou" +
					"rceReader how to depersist the object. This is currently not \r\n    extensible. F" +
					"or a given mimetype the value must be set accordingly:\r\n    \r\n    Note - applica" +
					"tion/x-microsoft.net.object.binary.base64 is the format \r\n    that the ResXResou" +
					"rceWriter will generate, however the reader can \r\n    read any of the formats li" +
					"sted below.\r\n    \r\n    mimetype: application/x-microsoft.net.object.binary.base6" +
					"4\r\n    value   : The object must be serialized with \r\n            : System.Runti" +
					"me.Serialization.Formatters.Binary.BinaryFormatter\r\n            : and then encod" +
					"ed with base64 encoding.\r\n    \r\n    mimetype: application/x-microsoft.net.object" +
					".soap.base64\r\n    value   : The object must be serialized with \r\n            : S" +
					"ystem.Runtime.Serialization.Formatters.Soap.SoapFormatter\r\n            : and the" +
					"n encoded with base64 encoding.\r\n\r\n    mimetype: application/x-microsoft.net.obj" +
					"ect.bytearray.base64\r\n    value   : The object must be serialized into a byte ar" +
					"ray \r\n            : using a System.ComponentModel.TypeConverter\r\n            : a" +
					"nd then encoded with base64 encoding.\r\n    -->\r\n  <xsd:schema id=\"root\" xmlns=\"\"" +
					" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsof" +
					"t-com:xml-msdata\">\r\n    <xsd:import namespace=\"http://www.w3.org/XML/1998/namesp" +
					"ace\" />\r\n    <xsd:element name=\"root\" msdata:IsDataSet=\"true\">\r\n      <xsd:compl" +
					"exType>\r\n        <xsd:choice maxOccurs=\"unbounded\">\r\n          <xsd:element name" +
					"=\"metadata\">\r\n            <xsd:complexType>\r\n              <xsd:sequence>\r\n     " +
					"           <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\" />\r\n       " +
					"       </xsd:sequence>\r\n              <xsd:attribute name=\"name\" use=\"required\" " +
					"type=\"xsd:string\" />\r\n              <xsd:attribute name=\"type\" type=\"xsd:string\"" +
					" />\r\n              <xsd:attribute name=\"mimetype\" type=\"xsd:string\" />\r\n        " +
					"      <xsd:attribute ref=\"xml:space\" />\r\n            </xsd:complexType>\r\n       " +
					"   </xsd:element>\r\n          <xsd:element name=\"assembly\">\r\n            <xsd:com" +
					"plexType>\r\n              <xsd:attribute name=\"alias\" type=\"xsd:string\" />\r\n     " +
					"         <xsd:attribute name=\"name\" type=\"xsd:string\" />\r\n            </xsd:comp" +
					"lexType>\r\n          </xsd:element>\r\n          <xsd:element name=\"data\">\r\n       " +
					"     <xsd:complexType>\r\n              <xsd:sequence>\r\n                <xsd:eleme" +
					"nt name=\"value\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"1\" />\r\n         " +
					"       <xsd:element name=\"comment\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordina" +
					"l=\"2\" />\r\n              </xsd:sequence>\r\n              <xsd:attribute name=\"name" +
					"\" type=\"xsd:string\" use=\"required\" msdata:Ordinal=\"1\" />\r\n              <xsd:att" +
					"ribute name=\"type\" type=\"xsd:string\" msdata:Ordinal=\"3\" />\r\n              <xsd:a" +
					"ttribute name=\"mimetype\" type=\"xsd:string\" msdata:Ordinal=\"4\" />\r\n              " +
					"<xsd:attribute ref=\"xml:space\" />\r\n            </xsd:complexType>\r\n          </x" +
					"sd:element>\r\n          <xsd:element name=\"resheader\">\r\n            <xsd:complexT" +
					"ype>\r\n              <xsd:sequence>\r\n                <xsd:element name=\"value\" ty" +
					"pe=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"1\" />\r\n              </xsd:sequenc" +
					"e>\r\n              <xsd:attribute name=\"name\" type=\"xsd:string\" use=\"required\" />" +
					"\r\n            </xsd:complexType>\r\n          </xsd:element>\r\n        </xsd:choice" +
					">\r\n      </xsd:complexType>\r\n    </xsd:element>\r\n  </xsd:schema>\r\n  <resheader n" +
					"ame=\"resmimetype\">\r\n    <value>text/microsoft-resx</value>\r\n  </resheader>\r\n  <r" +
					"esheader name=\"version\">\r\n    <value>2.0</value>\r\n  </resheader>\r\n  <resheader n" +
					"ame=\"reader\">\r\n    <value>System.Resources.ResXResourceReader, System.Windows.Fo" +
					"rms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>\r\n" +
					"  </resheader>\r\n  <resheader name=\"writer\">\r\n    <value>System.Resources.ResXRes" +
					"ourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyTo" +
					"ken=b77a5c561934e089</value>\r\n  </resheader>\r\n");
this.GenerateResxStrings();
			this.Write("</root>");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class CommonResourcesTemplateBase
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
