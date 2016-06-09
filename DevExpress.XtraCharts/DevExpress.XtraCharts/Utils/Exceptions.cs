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
using System.Text;
namespace DevExpress.XtraCharts {
	public class LayoutStreamException : Exception {
		public LayoutStreamException() : base() {
		}
		public LayoutStreamException(string message) : base(message) {
		}
		public LayoutStreamException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	public class PaletteException : Exception {
		internal PaletteException() : base() {
		}
		internal PaletteException(string message) : base(message) {
		}
		internal PaletteException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	public class InvalidScaleTypeException : Exception {
		public InvalidScaleTypeException(string message) : base(message) {
		}
	}
	public class InvalidAxisAlignmentException : Exception {
		public InvalidAxisAlignmentException(string message) : base(message) {
		}
	}
	public class SeriesPointArgumentChangingException : Exception {
		public SeriesPointArgumentChangingException(string message) : base(message) {
		}
	}
	public class SeriesPointValueChangingException : Exception {
		public SeriesPointValueChangingException(string message) : base(message) {
		}
	}
	public class InvalidSeriesPointIDException : Exception {
		public InvalidSeriesPointIDException(string message) : base(message) {
		}
	}
	public class SeriesPointCollectionChangingException : Exception {
		public SeriesPointCollectionChangingException(string message) : base(message) {
		}
	}
	public class SeriesPointFilterException : Exception {
		public SeriesPointFilterException(string message) : base(message) {
		}
	}
	public class ExplodedSeriesPointCollectionException : Exception {
		public ExplodedSeriesPointCollectionException(string message) : base(message) {
		}
	}
	public class InvalidOwnerException : Exception {
		public InvalidOwnerException(string message) : base(message) {
		}
	}
	public class ChartLoadingException : Exception {
		public ChartLoadingException(string message) : base(message) {
		}
	}
	public class ChartCollectionException : Exception {
		public ChartCollectionException(string message) : base(message) {
		}
	}
	public class InvalidPaneException : Exception {
		public InvalidPaneException(string message) : base(message) {
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class SeriesViewFactoryException : Exception {
		public SeriesViewFactoryException() : base() {
		}
		public SeriesViewFactoryException(string message) : base(message) {
		}
		public SeriesViewFactoryException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	public class DefaultSwitchException : Exception {
		public DefaultSwitchException() : base() {
		}
	}
	public class InternalException : Exception {
		public InternalException(string message) : base("internal error: " + message) {
		}
	}
	public class OpenGLException : Exception {
		public OpenGLException(string message) : base(message) {
		}
	}
}
