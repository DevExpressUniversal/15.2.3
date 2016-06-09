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

using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
namespace DevExpress.XtraSpreadsheet.Model {
public class ComplexNumberParser : IDisposable {
	const int _EOF = 0;
	const int _positiveinumber = 1;
	const int _imaginaryunit = 2;
	const int _fnumber = 3;
	const int _space = 4;
	const int maxT = 9;
	const bool T = true;
	const bool x = false;
	const int minErrDist = 1;
	ComplexNumberScanner scanner;
	ComplexNumberParserErrors  errors;
	SpreadsheetComplex resultComplex;
	WorkbookDataContext context;
	Token t;	
	Token la;   
	int errDist = minErrDist;
bool IsSimpleImaginaryUnit(){
		scanner.ResetPeek();
		if (la.kind == _imaginaryunit)
			return true; 
		if (la.val != "+" && la.val != "-")
			return false; 
		Token next = scanner.Peek();
		return next != null && next.kind == _imaginaryunit;
	} 
	public ComplexNumberParser(WorkbookDataContext context) {
		this.scanner = new ComplexNumberScanner();
		this.context = context;
		errors = new ComplexNumberParserErrors();
	}
	#region Properties
	public ComplexNumberParserErrors Errors { get { return errors; } }
	public SpreadsheetComplex ResultComplex { get { return resultComplex; } }
	#endregion 
	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}
	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }
			la = t;
		}
	}
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811")]
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	void ComplexNumberParserGrammar() {
		double real = 0;
		double imaginary = 0;
		bool wasImaginary = true;
		char suffix = 'i';
		if (IsSimpleImaginaryUnit()) {
			int sign = 1;	
			if (la.kind == 5 || la.kind == 6) {
				if (la.kind == 5) {
					Get();
				} else {
					Get();
					sign = -1;	
				}
			}
			ImaginaryUnitTerm(out suffix);
			imaginary = sign;	
		} else if (StartOf(1)) {
			ComplexValuePart(out real);
			if (la.kind == 2 || la.kind == 5 || la.kind == 6) {
				wasImaginary = false;	
				if (la.kind == 5 || la.kind == 6) {
					int sign = 1;	
					if (la.kind == 5) {
						Get();
					} else {
						Get();
						sign = -1;	
					}
					imaginary = sign;	
					wasImaginary = true;	
					if (StartOf(1)) {
						ComplexValuePart(out imaginary);
						imaginary *= sign; 
					}
				}
				ImaginaryUnitTerm(out suffix);
			}
		} else SynErr(10);
		resultComplex = new SpreadsheetComplex();
		if(wasImaginary)
		resultComplex.Value = new Complex(real, imaginary);
		else
		resultComplex.Value = new Complex(0, real);
		resultComplex.Suffix = suffix;
	}
	void ImaginaryUnitTerm(out char value) {
		value = 'i';	
		Expect(2);
		value = t.val[0]; 
	}
	void ComplexValuePart(out double value) {
		value = 0;	
		if (StartOf(2)) {
			int sign = 1;	
			if (la.kind == 5 || la.kind == 6) {
				if (la.kind == 5) {
					Get();
				} else {
					Get();
					sign = -1;	
				}
			}
			TermNumber(out value);
			value *= sign;	
		} else if (la.kind == 7) {
			Get();
			TermNumber(out value);
			value = value * (-1); 
			Expect(8);
		} else SynErr(11);
	}
	void TermNumber(out double value) {
		if (la.kind == 1) {
			Get();
		} else if (la.kind == 3) {
			Get();
		} else SynErr(12);
		value=-1;
		if(!double.TryParse(t.val, NumberStyles.Float, context.Culture, out value))
		SemErr("Incorrect double value");
	}
	public bool TryParse(string reference, char decimalSymbol, out SpreadsheetComplex value) {
		resultComplex = new SpreadsheetComplex();
		scanner.SetString(reference, decimalSymbol);
		Errors.Clear();
		la = new Token();
		la.val = "";		
		Get();
		ComplexNumberParserGrammar();
		Expect(0);
		value = resultComplex;
		return !(Errors.Count > 0);
	}
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x},
		{x,T,x,T, x,T,T,T, x,x,x},
		{x,T,x,T, x,T,T,x, x,x,x}
	};
	#region IDisposable Members
	public void Dispose() {
		scanner.Dispose();
		scanner = null;
	}
	#endregion
} 
public class ComplexNumberParserErrors
{
	#region Fields
	int count = 0;									
	string errMsgFormat = "-- line {0} col {1}: {2}"; 
	List<ErrorDescription> errorCollection = new List<ErrorDescription>();
	#endregion
	#region Properties
	public int Count { get { return count; } }
	public List<ErrorDescription> ErrorCollection { get { return errorCollection; } }
	#endregion
	public ComplexNumberParserErrors()
	{
	}
	public void SynErr(int line, int col, int n)
	{
		string s;
		switch (n)
		{
			case 0: s = "EOF expected"; break;
			case 1: s = "positiveinumber expected"; break;
			case 2: s = "imaginaryunit expected"; break;
			case 3: s = "fnumber expected"; break;
			case 4: s = "space expected"; break;
			case 5: s = "\"+\" expected"; break;
			case 6: s = "\"-\" expected"; break;
			case 7: s = "\"(\" expected"; break;
			case 8: s = "\")\" expected"; break;
			case 9: s = "??? expected"; break;
			case 10: s = "invalid ComplexNumberParserGrammar"; break;
			case 11: s = "invalid ComplexValuePart"; break;
			case 12: s = "invalid TermNumber"; break;
			default: s = "error " + n; break;
		}
		this.ErrorCollection.Add(new ErrorDescription(
			string.Format(errMsgFormat, line, col, s),
			ErrorType.Syntax,
			line,
			col));
		count++;
	}
	public void SemErr(int line, int col, string s)
	{
		this.ErrorCollection.Add(new ErrorDescription(
			string.Format(errMsgFormat, line, col, s),
			ErrorType.Semantic,
			line,
			col));
		count++;
	}
	public void SemErr(string s)
	{
		this.ErrorCollection.Add(new ErrorDescription(
			s,
			ErrorType.Semantic,
			-1,
			-1));
		count++;
	}
	public void Warning(int line, int col, string s)
	{
		this.ErrorCollection.Add(new ErrorDescription(
			string.Format(errMsgFormat, line, col, s),
			ErrorType.Warning,
			line,
			col));
	}
	public void Warning(string s)
	{
		this.ErrorCollection.Add(new ErrorDescription(
			s,
			ErrorType.Warning,
			-1,
			-1));
	}
	public void Clear() {
		count = 0;
		ErrorCollection.Clear();
	}
} 
}
