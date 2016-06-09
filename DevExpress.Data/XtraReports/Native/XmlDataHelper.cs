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
using System.Data;
using System.IO;
namespace DevExpress.Utils {
	public static class XmlDataHelper {
		public static string GetXmlSchema(DataSet ds) {
			try {
				using(MemoryStream s = new MemoryStream()) {
					ds.WriteXmlSchema(s);
					using(StreamReader sr = new StreamReader(s)) {
						s.Seek(0, SeekOrigin.Begin);
						return sr.ReadToEnd(); 
					}
				}
			} catch {
				return String.Empty;
			}
		}
		public static DataSet CreateDataSetByXmlUrl(string xmlUrl, bool schemaOnly) {
			if(String.IsNullOrEmpty(xmlUrl))
				return null;
			DataSet ds = new DataSet();	
			if(schemaOnly)
				ds.ReadXmlSchema(xmlUrl);
			else
				ds.ReadXml(xmlUrl);
			return ds;
		}
		public static DataSet CreateDataSetBySchema(string xmlSchema) {
			if(!String.IsNullOrEmpty(xmlSchema)) {
				try {
					DataSet ds = new DataSet();		
					TextReader reader = new StringReader(xmlSchema);
					try {
						ds.ReadXmlSchema(reader); 
					} finally {
						reader.Close();
					}
					return ds;
				} catch { }
			}
			return null;
		}
	}
}
