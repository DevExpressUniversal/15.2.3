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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Fields {
	public class Base64StringDataContainer : IDataContainer, ISupportsCopyFrom<Base64StringDataContainer>, IDisposable {
		public static Base64StringDataContainer FromBase64String(string str) {
			Guard.ArgumentIsNotNullOrEmpty(str, "str");
			Base64StringDataContainer result = new Base64StringDataContainer();
			result.SetData(Convert.FromBase64String(str));
			return result;
		}
		ChunkedMemoryStream stream;
		public Base64StringDataContainer() {
			stream = new ChunkedMemoryStream();
		}
		public void SetData(string str) {
			if (String.IsNullOrEmpty(str))
				return;
			SetData(Convert.FromBase64String(str));
		}
		#region IDataContainer Members
		DataContainerPropertiesChangedEventHandler propertiesChanged;
		public event DataContainerPropertiesChangedEventHandler PropertiesChanged { add { propertiesChanged += value; } remove { propertiesChanged -= value; } }
		public void SetData(byte[] data) {
			if (stream == null)
				return;
			stream.Write(data, 0, data.Length);
			RaisePropertiesChanged(null);
		}
		public byte[] GetData() {
			if (stream != null)
				return stream.ToArray();
			return new byte[] { };
		}
		void RaisePropertiesChanged(IDataContainerPropertiesChangedInfo info) {
			if (this.propertiesChanged != null) {
				DataContainerPropertiesChangedEventArgs e = new DataContainerPropertiesChangedEventArgs();
				e.ChangedInfo = info;
				propertiesChanged(this, e);
			}
		}
		#endregion
		public override bool Equals(object obj) {
			Base64StringDataContainer dataContainer = obj as Base64StringDataContainer;
			return Equals(dataContainer);
		}
		public bool Equals(Base64StringDataContainer dataContainer) {
			if (dataContainer == null)
				return false;
			return this.stream == dataContainer.stream;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region ICloneable<IDataContainer> Members
		public IDataContainer Clone() {
			Base64StringDataContainer result = new Base64StringDataContainer();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<StringDataContainer> Members
		public void CopyFrom(Base64StringDataContainer value) {
			this.stream.CopyFrom(value.stream);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing){
			if (disposing)
				if (stream != null) {
					stream.Close();
					((IDisposable)stream).Dispose();
					stream = null;
				}
		}
		#endregion
	}
}
