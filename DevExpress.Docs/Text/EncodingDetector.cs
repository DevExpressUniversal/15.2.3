#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using System.Text;
using DevExpress.Utils;
using DevExpress.Internal;
namespace DevExpress.Docs.Text {
	#region EncodingDetector
	public class EncodingDetector {
		readonly InternalEncodingDetector internalDetector = new InternalEncodingDetector();
		public void BeginDetection() {
			internalDetector.BeginDetection();
		}
		public bool AnalyseData(byte[] buffer, int from, int length) {
			return internalDetector.AnalyseData(buffer, from, length);
		}
		public Encoding EndDetection() {
			return internalDetector.EndDetection();
		}
		public Encoding Detect(byte[] buffer, int from, int length) {
			return internalDetector.Detect(buffer, from, length);
		}
		public Encoding Detect(byte[] buffer) {
			return internalDetector.Detect(buffer);
		}
		public Encoding Detect(Stream stream, int maxByteCount, bool keepPosition) {
			return internalDetector.Detect(stream, maxByteCount, keepPosition);
		}
		public Encoding Detect(Stream stream, int maxByteCount) {
			return internalDetector.Detect(stream, maxByteCount);
		}
		public Encoding Detect(Stream stream) {
			return internalDetector.Detect(stream);
		}
	}
	#endregion
}
