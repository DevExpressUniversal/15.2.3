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
using System.IO;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public abstract class PdfDocumentObject {
		PdfObject innerObject;
		bool compressed;
#if DEBUGTEST
		bool isWritten;
#endif
		protected bool Compressed { get { return compressed; } }
		public PdfObject InnerObject { get { return innerObject; } }
		protected PdfDocumentObject(PdfObject innerObject, bool compressed) {
			this.innerObject = innerObject;
			this.compressed = compressed;
		}	
		protected void SetInnerObjectIfNull(PdfObject innerObject) {
			if(this.innerObject == null)
				this.innerObject = innerObject;
		}
		protected virtual void RegisterContent(PdfXRef xRef) {
		}
		protected virtual void WriteContent(StreamWriter writer) {
		}
		public virtual void FillUp() {
		}
		public void Register(PdfXRef xRef) {
			xRef.RegisterObject(this.innerObject);
			RegisterContent(xRef);
		}
		public void Write(StreamWriter writer) {
#if DEBUGTEST
			if(isWritten)
				throw new Exception("Object already written");
			isWritten = true;
#endif
			this.innerObject.WriteIndirect(writer);
			WriteContent(writer);
		}
		public virtual void AddToDictionary(PdfDictionary dictionary) {
			throw new NotImplementedException();			
		}
	}
	public abstract class PdfDocumentDictionaryObject : PdfDocumentObject {
		public PdfDictionary Dictionary { get { return (PdfDictionary)base.InnerObject; } }
		protected PdfDocumentDictionaryObject(bool compressed)
			: base(new PdfDictionary(PdfObjectType.Indirect), compressed) {
		}
	}
	public abstract class PdfDocumentStreamObject : PdfDocumentObject {
		static PdfStream CreateInnerStream(bool useFlateEncoding) {
			return 
				useFlateEncoding ?
				new PdfDirectFlateStream() :
				new PdfStream();
		}
		protected virtual bool UseFlateEncoding { get { return true; } }
		public PdfStream Stream { get { return (PdfStream)base.InnerObject; } }
		public PdfDictionary Attributes { get { return Stream.Attributes; } }
		protected PdfDocumentStreamObject(bool compressed)
			: base(null, compressed) {
			SetInnerObjectIfNull(CreateInnerStream(UseFlateEncoding && compressed));
		}
		public virtual void Close() {
			Stream.Close();
		}
	}
	public class PdfObjectCollection<T> where T : PdfDocumentObject {
		List<T> list = new List<T>();
		public int Count { get { return list.Count; } }
		public T this[int index] { get { return list[index]; } }
		protected IList<T> List { get { return list; } }
		public void Add(T pdfDocumentObject) {
			if(pdfDocumentObject != null)
				this.list.Add(pdfDocumentObject);
		}
		public void AddUnique(T pdfDocumentObject) {
			if(pdfDocumentObject != null)
				if(list.IndexOf(pdfDocumentObject) == -1)
			   list.Add(pdfDocumentObject);
		}
		public void FillUp() {
			foreach(T obj in this.list)
				obj.FillUp();
		}
		public void Register(PdfXRef xRef) {
			foreach(T obj in this.list)
				obj.Register(xRef);
		}
		public void Write(StreamWriter writer) {
			foreach(T obj in this.list)
				obj.Write(writer);
		}
		public void Clear() {
			list.Clear();
		}
		public PdfDictionary CreateDictionary() {
			if(this.Count > 0) {
				PdfDictionary dictionary = new PdfDictionary();
				for(int i = 0; i < this.Count; i++)
					this[i].AddToDictionary(dictionary);
				return dictionary;
			} else
				return null;
		}
	}
}
