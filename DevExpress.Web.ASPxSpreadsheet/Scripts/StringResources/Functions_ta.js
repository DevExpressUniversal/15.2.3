ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Returns the absolute value of a number, a number without its sign.",
		arguments: [
			{
				name: "number",
				description: "is the real number for which you want the absolute value"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Returns the accrued interest for a security that pays interest at maturity.",
		arguments: [
			{
				name: "issue",
				description: "is the security's issue date, expressed as a serial date number"
			},
			{
				name: "settlement",
				description: "is the security's maturity date, expressed as a serial date number"
			},
			{
				name: "rate",
				description: "is the security's annual coupon rate"
			},
			{
				name: "par",
				description: "is the security's par value"
			},
			{
				name: "basis",
				description: "is the type of day count basis to use"
			}
		]
	},
	{
		name: "ACOS",
		description: "Returns the arccosine of a number, in radians in the range 0 to Pi. The arccosine is the angle whose cosine is Number.",
		arguments: [
			{
				name: "number",
				description: "is the cosine of the angle you want and must be from -1 to 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Returns the inverse hyperbolic cosine of a number.",
		arguments: [
			{
				name: "number",
				description: "is any real number equal to or greater than 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Returns the arccotangent of a number, in radians in the range 0 to Pi.",
		arguments: [
			{
				name: "number",
				description: "is the cotangent of the angle you want"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Returns the inverse hyperbolic cotangent of a number.",
		arguments: [
			{
				name: "number",
				description: "is the hyperbolic cotangent of the angle that you want"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "Creates a cell reference as text, given specified row and column numbers.",
		arguments: [
			{
				name: "row_num",
				description: "is the row number to use in the cell reference: Row_number = 1 for row 1"
			},
			{
				name: "column_num",
				description: "is the column number to use in the cell reference. For example, Column_number = 4 for column D"
			},
			{
				name: "abs_num",
				description: "specifies the reference type: absolute = 1; absolute row/relative column = 2; relative row/absolute column = 3; relative = 4"
			},
			{
				name: "a1",
				description: "is a logical value that specifies the reference style: A1 style = 1 or TRUE; R1C1 style = 0 or FALSE"
			},
			{
				name: "sheet_text",
				description: "is text specifying the name of the worksheet to be used as the external reference"
			}
		]
	},
	{
		name: "AND",
		description: "Checks whether all arguments are TRUE, and returns TRUE if all arguments are TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "are 1 to 255 conditions you want to test that can be either TRUE or FALSE and can be logical values, arrays, or references"
			},
			{
				name: "logical2",
				description: "are 1 to 255 conditions you want to test that can be either TRUE or FALSE and can be logical values, arrays, or references"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Converts a Roman numeral to Arabic.",
		arguments: [
			{
				name: "text",
				description: "is the Roman numeral you want to convert"
			}
		]
	},
	{
		name: "AREAS",
		description: "Returns the number of areas in a reference. An area is a range of contiguous cells or a single cell.",
		arguments: [
			{
				name: "reference",
				description: "is a reference to a cell or range of cells and can refer to multiple areas"
			}
		]
	},
	{
		name: "ASIN",
		description: "Returns the arcsine of a number in radians, in the range -Pi/2 to Pi/2.",
		arguments: [
			{
				name: "number",
				description: "is the sine of the angle you want and must be from -1 to 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Returns the inverse hyperbolic sine of a number.",
		arguments: [
			{
				name: "number",
				description: "is any real number equal to or greater than 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Returns the arctangent of a number in radians, in the range -Pi/2 to Pi/2.",
		arguments: [
			{
				name: "number",
				description: "is the tangent of the angle you want"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Returns the arctangent of the specified x- and y- coordinates, in radians between -Pi and Pi, excluding -Pi.",
		arguments: [
			{
				name: "x_num",
				description: "is the x-coordinate of the point"
			},
			{
				name: "y_num",
				description: "is the y-coordinate of the point"
			}
		]
	},
	{
		name: "ATANH",
		description: "Returns the inverse hyperbolic tangent of a number.",
		arguments: [
			{
				name: "number",
				description: "is any real number between -1 and 1 excluding -1 and 1"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "Returns the average of the absolute deviations of data points from their mean. Arguments can be numbers or names, arrays, or references that contain numbers.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 arguments for which you want the average of the absolute deviations"
			},
			{
				name: "number2",
				description: "are 1 to 255 arguments for which you want the average of the absolute deviations"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "Returns the average (arithmetic mean) of its arguments, which can be numbers or names, arrays, or references that contain numbers.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numeric arguments for which you want the average"
			},
			{
				name: "number2",
				description: "are 1 to 255 numeric arguments for which you want the average"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Returns the average (arithmetic mean) of its arguments, evaluating text and FALSE in arguments as 0; TRUE evaluates as 1. Arguments can be numbers, names, arrays, or references.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "are 1 to 255 arguments for which you want the average"
			},
			{
				name: "value2",
				description: "are 1 to 255 arguments for which you want the average"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Finds average(arithmetic mean) for the cells specified by a given condition or criteria.",
		arguments: [
			{
				name: "range",
				description: "is the range of cells you want evaluated"
			},
			{
				name: "criteria",
				description: "is the condition or criteria in the form of a number, expression, or text that defines which cells will be used to find the average"
			},
			{
				name: "average_range",
				description: "are the actual cells to be used to find the average. If omitted, the cells in range are used "
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Finds average(arithmetic mean) for the cells specified by a given set of conditions or criteria.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "average_range",
				description: "are the actual cells to be used to find the average."
			},
			{
				name: "criteria_range",
				description: "is the range of cells you want evaluated for the particular condition"
			},
			{
				name: "criteria",
				description: "is the condition or criteria in the form of a number, expression, or text that defines which cells will be used to find the average"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Converts a number to text (baht).",
		arguments: [
			{
				name: "number",
				description: "is a number that you want to convert"
			}
		]
	},
	{
		name: "BASE",
		description: "Converts a number into a text representation with the given radix (base).",
		arguments: [
			{
				name: "number",
				description: "is the number that you want to convert"
			},
			{
				name: "radix",
				description: "is the base Radix that you want to convert the number into"
			},
			{
				name: "min_length",
				description: "is the minimum length of the returned string.  If omitted leading zeros are not added"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Returns the modified Bessel function In(x).",
		arguments: [
			{
				name: "x",
				description: "is the value at which to evaluate the function"
			},
			{
				name: "n",
				description: "is the order of the Bessel function"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Returns the Bessel function Jn(x).",
		arguments: [
			{
				name: "x",
				description: "is the value at which to evaluate the function"
			},
			{
				name: "n",
				description: "is the order of the Bessel function"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Returns the modified Bessel function Kn(x).",
		arguments: [
			{
				name: "x",
				description: "is the value at which to evaluate the function"
			},
			{
				name: "n",
				description: "is the order of the function"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Returns the Bessel function Yn(x).",
		arguments: [
			{
				name: "x",
				description: "is the value at which to evaluate the function"
			},
			{
				name: "n",
				description: "is the order of the function"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Returns the beta probability distribution function.",
		arguments: [
			{
				name: "x",
				description: "is the value between A and B at which to evaluate the function"
			},
			{
				name: "alpha",
				description: "is a parameter to the distribution and must be greater than 0"
			},
			{
				name: "beta",
				description: "is a parameter to the distribution and must be greater than 0"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE"
			},
			{
				name: "A",
				description: "is an optional lower bound to the interval of x. If omitted, A = 0"
			},
			{
				name: "B",
				description: "is an optional upper bound to the interval of x. If omitted, B = 1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Returns the inverse of the cumulative beta probability density function (BETA.DIST).",
		arguments: [
			{
				name: "probability",
				description: "is a probability associated with the beta distribution"
			},
			{
				name: "alpha",
				description: "is a parameter to the distribution and must be greater than 0"
			},
			{
				name: "beta",
				description: "is a parameter to the distribution and must be greater than 0"
			},
			{
				name: "A",
				description: "is an optional lower bound to the interval of x. If omitted, A = 0"
			},
			{
				name: "B",
				description: "is an optional upper bound to the interval of x. If omitted, B = 1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "Returns the cumulative beta probability density function.",
		arguments: [
			{
				name: "x",
				description: "is the value between A and B at which to evaluate the function"
			},
			{
				name: "alpha",
				description: "is a parameter to the distribution and must be greater than 0"
			},
			{
				name: "beta",
				description: "is a parameter to the distribution and must be greater than 0"
			},
			{
				name: "A",
				description: "is an optional lower bound to the interval of x. If omitted, A = 0"
			},
			{
				name: "B",
				description: "is an optional upper bound to the interval of x. If omitted, B = 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Returns the inverse of the cumulative beta probability density function (BETADIST).",
		arguments: [
			{
				name: "probability",
				description: "is a probability associated with the beta distribution"
			},
			{
				name: "alpha",
				description: "is a parameter to the distribution and must be greater than 0"
			},
			{
				name: "beta",
				description: "is a parameter to the distribution and must be greater than 0"
			},
			{
				name: "A",
				description: "is an optional lower bound to the interval of x. If omitted, A = 0"
			},
			{
				name: "B",
				description: "is an optional upper bound to the interval of x. If omitted, B = 1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Converts a binary number to decimal.",
		arguments: [
			{
				name: "number",
				description: "is the binary number you want to convert"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Converts a binary number to hexadecimal.",
		arguments: [
			{
				name: "number",
				description: "is the binary number you want to convert"
			},
			{
				name: "places",
				description: "is the number of characters to use"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Converts a binary number to octal.",
		arguments: [
			{
				name: "number",
				description: "is the binary number you want to convert"
			},
			{
				name: "places",
				description: "is the number of characters to use"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Returns the individual term binomial distribution probability.",
		arguments: [
			{
				name: "number_s",
				description: "is the number of successes in trials"
			},
			{
				name: "trials",
				description: "is the number of independent trials"
			},
			{
				name: "probability_s",
				description: "is the probability of success on each trial"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative distribution function, use TRUE; for the probability mass function, use FALSE"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Returns the probability of a trial result using a binomial distribution.",
		arguments: [
			{
				name: "trials",
				description: "is the number of independent trials"
			},
			{
				name: "probability_s",
				description: "is the probability of success on each trial"
			},
			{
				name: "number_s",
				description: "is the number of successes in trials"
			},
			{
				name: "number_s2",
				description: "if provided this function returns the probability that the number of successful trials shall lie between number_s and number_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Returns the smallest value for which the cumulative binomial distribution is greater than or equal to a criterion value.",
		arguments: [
			{
				name: "trials",
				description: "is the number of Bernoulli trials"
			},
			{
				name: "probability_s",
				description: "is the probability of success on each trial, a number between 0 and 1 inclusive"
			},
			{
				name: "alpha",
				description: "is the criterion value, a number between 0 and 1 inclusive"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Returns the individual term binomial distribution probability.",
		arguments: [
			{
				name: "number_s",
				description: "is the number of successes in trials"
			},
			{
				name: "trials",
				description: "is the number of independent trials"
			},
			{
				name: "probability_s",
				description: "is the probability of success on each trial"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative distribution function, use TRUE; for the probability mass function, use FALSE"
			}
		]
	},
	{
		name: "BITAND",
		description: "Returns a bitwise 'And' of two numbers.",
		arguments: [
			{
				name: "number1",
				description: "is the decimal representation of the binary number you want to evaluate"
			},
			{
				name: "number2",
				description: "is the decimal representation of the binary number you want to evaluate"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Returns a number shifted left by shift_amount bits.",
		arguments: [
			{
				name: "number",
				description: "is the decimal representation of the binary number you want to evaluate"
			},
			{
				name: "shift_amount",
				description: "is the number of bits that you want to shift Number left by"
			}
		]
	},
	{
		name: "BITOR",
		description: "Returns a bitwise 'Or' of two numbers.",
		arguments: [
			{
				name: "number1",
				description: "is the decimal representation of the binary number you want to evaluate"
			},
			{
				name: "number2",
				description: "is the decimal representation of the binary number you want to evaluate"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Returns a number shifted right by shift_amount bits.",
		arguments: [
			{
				name: "number",
				description: "is the decimal representation of the binary number you want to evaluate"
			},
			{
				name: "shift_amount",
				description: "is the number of bits that you want to shift Number right by"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Returns a bitwise 'Exclusive Or' of two numbers.",
		arguments: [
			{
				name: "number1",
				description: "is the decimal representation of the binary number you want to evaluate"
			},
			{
				name: "number2",
				description: "is the decimal representation of the binary number you want to evaluate"
			}
		]
	},
	{
		name: "CEILING",
		description: "Rounds a number up, to the nearest multiple of significance.",
		arguments: [
			{
				name: "number",
				description: "is the value you want to round"
			},
			{
				name: "significance",
				description: "is the multiple to which you want to round"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Rounds a number up, to the nearest integer or to the nearest multiple of significance.",
		arguments: [
			{
				name: "number",
				description: "is the value you want to round"
			},
			{
				name: "significance",
				description: "is the multiple to which you want to round"
			},
			{
				name: "mode",
				description: "when given and nonzero this function will round away from zero"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Rounds a number up, to the nearest integer or to the nearest multiple of significance.",
		arguments: [
			{
				name: "number",
				description: "is the value you want to round"
			},
			{
				name: "significance",
				description: "is the multiple to which you want to round"
			}
		]
	},
	{
		name: "CELL",
		description: "Returns information about the formatting, location, or contents of the first cell, according to the sheet's reading order, in a reference.",
		arguments: [
			{
				name: "info_type",
				description: "is a text value that specifies what type of cell information you want."
			},
			{
				name: "reference",
				description: "is the cell that you want information about"
			}
		]
	},
	{
		name: "CHAR",
		description: "Returns the character specified by the code number from the character set for your computer.",
		arguments: [
			{
				name: "number",
				description: "is a number between 1 and 255 specifying which character you want"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Returns the right-tailed probability of the chi-squared distribution.",
		arguments: [
			{
				name: "x",
				description: "is the value at which you want to evaluate the distribution, a nonnegative number"
			},
			{
				name: "deg_freedom",
				description: "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Returns the inverse of the right-tailed probability of the chi-squared distribution.",
		arguments: [
			{
				name: "probability",
				description: "is a probability associated with the chi-squared distribution, a value between 0 and 1 inclusive"
			},
			{
				name: "deg_freedom",
				description: "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Returns the left-tailed probability of the chi-squared distribution.",
		arguments: [
			{
				name: "x",
				description: "is the value at which you want to evaluate the distribution, a nonnegative number"
			},
			{
				name: "deg_freedom",
				description: "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			},
			{
				name: "cumulative",
				description: "is a logical value for the function to return: the cumulative distribution function = TRUE; the probability density function = FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Returns the right-tailed probability of the chi-squared distribution.",
		arguments: [
			{
				name: "x",
				description: "is the value at which you want to evaluate the distribution, a nonnegative number"
			},
			{
				name: "deg_freedom",
				description: "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Returns the inverse of the left-tailed probability of the chi-squared distribution.",
		arguments: [
			{
				name: "probability",
				description: "is a probability associated with the chi-squared distribution, a value between 0 and 1 inclusive"
			},
			{
				name: "deg_freedom",
				description: "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Returns the inverse of the right-tailed probability of the chi-squared distribution.",
		arguments: [
			{
				name: "probability",
				description: "is a probability associated with the chi-squared distribution, a value between 0 and 1 inclusive"
			},
			{
				name: "deg_freedom",
				description: "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Returns the test for independence: the value from the chi-squared distribution for the statistic and the appropriate degrees of freedom.",
		arguments: [
			{
				name: "actual_range",
				description: "is the range of data that contains observations to test against expected values"
			},
			{
				name: "expected_range",
				description: "is the range of data that contains the ratio of the product of row totals and column totals to the grand total"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Returns the test for independence: the value from the chi-squared distribution for the statistic and the appropriate degrees of freedom.",
		arguments: [
			{
				name: "actual_range",
				description: "is the range of data that contains observations to test against expected values"
			},
			{
				name: "expected_range",
				description: "is the range of data that contains the ratio of the product of row totals and column totals to the grand total"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "Chooses a value or action to perform from a list of values, based on an index number.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_num",
				description: "specifies which value argument is selected. Index_num must be between 1 and 254, or a formula or a reference to a number between 1 and 254"
			},
			{
				name: "value1",
				description: "are 1 to 254 numbers, cell references, defined names, formulas, functions, or text arguments from which CHOOSE selects"
			},
			{
				name: "value2",
				description: "are 1 to 254 numbers, cell references, defined names, formulas, functions, or text arguments from which CHOOSE selects"
			}
		]
	},
	{
		name: "CLEAN",
		description: "Removes all nonprintable characters from text.",
		arguments: [
			{
				name: "text",
				description: "is any worksheet information from which you want to remove nonprintable characters"
			}
		]
	},
	{
		name: "CODE",
		description: "Returns a numeric code for the first character in a text string, in the character set used by your computer.",
		arguments: [
			{
				name: "text",
				description: "is the text for which you want the code of the first character"
			}
		]
	},
	{
		name: "COLUMN",
		description: "Returns the column number of a reference.",
		arguments: [
			{
				name: "reference",
				description: "is the cell or range of contiguous cells for which you want the column number. If omitted, the cell containing the COLUMN function is used"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "Returns the number of columns in an array or reference.",
		arguments: [
			{
				name: "array",
				description: "is an array or array formula, or a reference to a range of cells for which you want the number of columns"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Returns the number of combinations for a given number of items.",
		arguments: [
			{
				name: "number",
				description: "is the total number of items"
			},
			{
				name: "number_chosen",
				description: "is the number of items in each combination"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Returns the number of combinations with repetitions for a given number of items.",
		arguments: [
			{
				name: "number",
				description: "is the total number of items"
			},
			{
				name: "number_chosen",
				description: "is the number of items in each combination"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Converts real and imaginary coefficients into a complex number.",
		arguments: [
			{
				name: "real_num",
				description: "is the real coefficient of the complex number"
			},
			{
				name: "i_num",
				description: "is the imaginary coefficient of the complex number"
			},
			{
				name: "suffix",
				description: "is the suffix for the imaginary component of the complex number"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Joins several text strings into one text string.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "are 1 to 255 text strings to be joined into a single text string and can be text strings, numbers, or single-cell references"
			},
			{
				name: "text2",
				description: "are 1 to 255 text strings to be joined into a single text string and can be text strings, numbers, or single-cell references"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Returns the confidence interval for a population mean, using a normal distribution.",
		arguments: [
			{
				name: "alpha",
				description: "is the significance level used to compute the confidence level, a number greater than 0 and less than 1"
			},
			{
				name: "standard_dev",
				description: "is the population standard deviation for the data range and is assumed to be known. Standard_dev must be greater than 0"
			},
			{
				name: "size",
				description: "is the sample size"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Returns the confidence interval for a population mean, using a normal distribution.",
		arguments: [
			{
				name: "alpha",
				description: "is the significance level used to compute the confidence level, a number greater than 0 and less than 1"
			},
			{
				name: "standard_dev",
				description: "is the population standard deviation for the data range and is assumed to be known. Standard_dev must be greater than 0"
			},
			{
				name: "size",
				description: "is the sample size"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "Returns the confidence interval for a population mean, using a Student's T distribution.",
		arguments: [
			{
				name: "alpha",
				description: "is the significance level used to compute the confidence level, a number greater than 0 and less than 1"
			},
			{
				name: "standard_dev",
				description: "is the population standard deviation for the data range and is assumed to be known. Standard_dev must be greater than 0"
			},
			{
				name: "size",
				description: "is the sample size"
			}
		]
	},
	{
		name: "CONVERT",
		description: "Converts a number from one measurement system to another.",
		arguments: [
			{
				name: "number",
				description: "is the value in from_units to convert"
			},
			{
				name: "from_unit",
				description: "is the units for number"
			},
			{
				name: "to_unit",
				description: "is the units for the result"
			}
		]
	},
	{
		name: "CORREL",
		description: "Returns the correlation coefficient between two data sets.",
		arguments: [
			{
				name: "array1",
				description: "is a cell range of values. The values should be numbers, names, arrays, or references that contain numbers"
			},
			{
				name: "array2",
				description: "is a second cell range of values. The values should be numbers, names, arrays, or references that contain numbers"
			}
		]
	},
	{
		name: "COS",
		description: "Returns the cosine of an angle.",
		arguments: [
			{
				name: "number",
				description: "is the angle in radians for which you want the cosine"
			}
		]
	},
	{
		name: "COSH",
		description: "Returns the hyperbolic cosine of a number.",
		arguments: [
			{
				name: "number",
				description: "is any real number"
			}
		]
	},
	{
		name: "COT",
		description: "Returns the cotangent of an angle.",
		arguments: [
			{
				name: "number",
				description: "is the angle in radians for which you want the cotangent"
			}
		]
	},
	{
		name: "COTH",
		description: "Returns the hyperbolic cotangent of a number.",
		arguments: [
			{
				name: "number",
				description: "is the angle in radians for which you want the hyperbolic cotangent"
			}
		]
	},
	{
		name: "COUNT",
		description: "Counts the number of cells in a range that contain numbers.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "are 1 to 255 arguments that can contain or refer to a variety of different types of data, but only numbers are counted"
			},
			{
				name: "value2",
				description: "are 1 to 255 arguments that can contain or refer to a variety of different types of data, but only numbers are counted"
			}
		]
	},
	{
		name: "COUNTA",
		description: "Counts the number of cells in a range that are not empty.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "are 1 to 255 arguments representing the values and cells you want to count. Values can be any type of information"
			},
			{
				name: "value2",
				description: "are 1 to 255 arguments representing the values and cells you want to count. Values can be any type of information"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Counts the number of empty cells in a specified range of cells.",
		arguments: [
			{
				name: "range",
				description: "is the range from which you want to count the empty cells"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Counts the number of cells within a range that meet the given condition.",
		arguments: [
			{
				name: "range",
				description: "is the range of cells from which you want to count nonblank cells"
			},
			{
				name: "criteria",
				description: "is the condition in the form of a number, expression, or text that defines which cells will be counted"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Counts the number of cells specified by a given set of conditions or criteria.",
		arguments: [
			{
				name: "criteria_range",
				description: "is the range of cells you want evaluated for the particular condition"
			},
			{
				name: "criteria",
				description: "is the condition in the form of a number, expression, or text that defines which cells will be counted"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Returns the number of days from the beginning of the coupon period to the settlement date.",
		arguments: [
			{
				name: "settlement",
				description: "is the security's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the security's maturity date, expressed as a serial date number"
			},
			{
				name: "frequency",
				description: "is the number of coupon payments per year"
			},
			{
				name: "basis",
				description: "is the type of day count basis to use"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Returns the next coupon date after the settlement date.",
		arguments: [
			{
				name: "settlement",
				description: "is the security's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the security's maturity date, expressed as a serial date number"
			},
			{
				name: "frequency",
				description: "is the number of coupon payments per year"
			},
			{
				name: "basis",
				description: "is the type of day count basis to use"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Returns the number of coupons payable between the settlement date and maturity date.",
		arguments: [
			{
				name: "settlement",
				description: "is the security's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the security's maturity date, expressed as a serial date number"
			},
			{
				name: "frequency",
				description: "is the number of coupon payments per year"
			},
			{
				name: "basis",
				description: "is the type of day count basis to use"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Returns the previous coupon date before the settlement date.",
		arguments: [
			{
				name: "settlement",
				description: "is the security's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the security's maturity date, expressed as a serial date number"
			},
			{
				name: "frequency",
				description: "is the number of coupon payments per year"
			},
			{
				name: "basis",
				description: "is the type of day count basis to use"
			}
		]
	},
	{
		name: "COVAR",
		description: "Returns covariance, the average of the products of deviations for each data point pair in two data sets.",
		arguments: [
			{
				name: "array1",
				description: "is the first cell range of integers and must be numbers, arrays, or references that contain numbers"
			},
			{
				name: "array2",
				description: "is the second cell range of integers and must be numbers, arrays, or references that contain numbers"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Returns population covariance, the average of the products of deviations for each data point pair in two data sets.",
		arguments: [
			{
				name: "array1",
				description: "is the first cell range of integers and must be numbers, arrays, or references that contain numbers"
			},
			{
				name: "array2",
				description: "is the second cell range of integers and must be numbers, arrays, or references that contain numbers"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Returns sample covariance, the average of the products of deviations for each data point pair in two data sets.",
		arguments: [
			{
				name: "array1",
				description: "is the first cell range of integers and must be numbers, arrays, or references that contain numbers"
			},
			{
				name: "array2",
				description: "is the second cell range of integers and must be numbers, arrays, or references that contain numbers"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Returns the smallest value for which the cumulative binomial distribution is greater than or equal to a criterion value.",
		arguments: [
			{
				name: "trials",
				description: "is the number of Bernoulli trials"
			},
			{
				name: "probability_s",
				description: "is the probability of success on each trial, a number between 0 and 1 inclusive"
			},
			{
				name: "alpha",
				description: "is the criterion value, a number between 0 and 1 inclusive"
			}
		]
	},
	{
		name: "CSC",
		description: "Returns the cosecant of an angle.",
		arguments: [
			{
				name: "number",
				description: "is the angle in radians for which you want the cosecant"
			}
		]
	},
	{
		name: "CSCH",
		description: "Returns the hyperbolic cosecant of an angle.",
		arguments: [
			{
				name: "number",
				description: "is the angle in radians for which you want the hyperbolic cosecant"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "Returns the cumulative interest paid between two periods.",
		arguments: [
			{
				name: "rate",
				description: "is the interest rate"
			},
			{
				name: "nper",
				description: "is the total number of payment periods"
			},
			{
				name: "pv",
				description: "is the present value"
			},
			{
				name: "start_period",
				description: "is the first period in the calculation"
			},
			{
				name: "end_period",
				description: "is the last period in the calculation"
			},
			{
				name: "type",
				description: "is the timing of the payment"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Returns the cumulative principal paid on a loan between two periods.",
		arguments: [
			{
				name: "rate",
				description: "is the interest rate"
			},
			{
				name: "nper",
				description: "is the total number of payment periods"
			},
			{
				name: "pv",
				description: "is the present value"
			},
			{
				name: "start_period",
				description: "is the first period in the calculation"
			},
			{
				name: "end_period",
				description: "is the last period in the calculation"
			},
			{
				name: "type",
				description: "is the timing of the payment"
			}
		]
	},
	{
		name: "DATE",
		description: "Returns the number that represents the date in Spreadsheet date-time code.",
		arguments: [
			{
				name: "year",
				description: "is a number from 1900 to 9999 in Spreadsheet for Windows or from 1904 to 9999 in Spreadsheet for the Macintosh"
			},
			{
				name: "month",
				description: "is a number from 1 to 12 representing the month of the year"
			},
			{
				name: "day",
				description: "is a number from 1 to 31 representing the day of the month"
			}
		]
	},
	{
		name: "DATEDIF",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATEVALUE",
		description: "Converts a date in the form of text to a number that represents the date in Spreadsheet date-time code.",
		arguments: [
			{
				name: "date_text",
				description: "is text that represents a date in a Spreadsheet date format, between 1/1/1900 (Windows) or 1/1/1904 (Macintosh) and 12/31/9999"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "Averages the values in a column in a list or database that match conditions you specify.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "DAY",
		description: "Returns the day of the month, a number from 1 to 31.",
		arguments: [
			{
				name: "serial_number",
				description: "is a number in the date-time code used by Spreadsheet"
			}
		]
	},
	{
		name: "DAYS",
		description: "Returns the number of days between the two dates.",
		arguments: [
			{
				name: "end_date",
				description: "start_date and end_date are the two dates between which you want to know the number of days"
			},
			{
				name: "start_date",
				description: "start_date and end_date are the two dates between which you want to know the number of days"
			}
		]
	},
	{
		name: "DAYS360",
		description: "Returns the number of days between two dates based on a 360-day year (twelve 30-day months).",
		arguments: [
			{
				name: "start_date",
				description: "start_date and end_date are the two dates between which you want to know the number of days"
			},
			{
				name: "end_date",
				description: "start_date and end_date are the two dates between which you want to know the number of days"
			},
			{
				name: "method",
				description: "is a logical value specifying the calculation method: U.S. (NASD) = FALSE or omitted; European = TRUE."
			}
		]
	},
	{
		name: "DB",
		description: "Returns the depreciation of an asset for a specified period using the fixed-declining balance method.",
		arguments: [
			{
				name: "cost",
				description: "is the initial cost of the asset"
			},
			{
				name: "salvage",
				description: "is the salvage value at the end of the life of the asset"
			},
			{
				name: "life",
				description: "is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset)"
			},
			{
				name: "period",
				description: "is the period for which you want to calculate the depreciation. Period must use the same units as Life"
			},
			{
				name: "month",
				description: "is the number of months in the first year. If month is omitted, it is assumed to be 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "Counts the cells containing numbers in the field (column) of records in the database that match the conditions you specify.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "Counts nonblank cells in the field (column) of records in the database that match the conditions you specify.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "DDB",
		description: "Returns the depreciation of an asset for a specified period using the double-declining balance method or some other method you specify.",
		arguments: [
			{
				name: "cost",
				description: "is the initial cost of the asset"
			},
			{
				name: "salvage",
				description: "is the salvage value at the end of the life of the asset"
			},
			{
				name: "life",
				description: "is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset)"
			},
			{
				name: "period",
				description: "is the period for which you want to calculate the depreciation. Period must use the same units as Life"
			},
			{
				name: "factor",
				description: "is the rate at which the balance declines. If Factor is omitted, it is assumed to be 2 (the double-declining balance method)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Converts a decimal number to binary.",
		arguments: [
			{
				name: "number",
				description: "is the decimal integer you want to convert"
			},
			{
				name: "places",
				description: "is the number of characters to use"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Converts a decimal number to hexadecimal.",
		arguments: [
			{
				name: "number",
				description: "is the decimal integer you want to convert"
			},
			{
				name: "places",
				description: "is the number of characters to use"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Converts a decimal number to octal.",
		arguments: [
			{
				name: "number",
				description: "is the decimal integer you want to convert"
			},
			{
				name: "places",
				description: "is the number of characters to use"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Converts a text representation of a number in a given base into a decimal number.",
		arguments: [
			{
				name: "number",
				description: "is the number that you want to convert"
			},
			{
				name: "radix",
				description: "is the base Radix of the number you are converting"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Converts radians to degrees.",
		arguments: [
			{
				name: "angle",
				description: "is the angle in radians that you want to convert"
			}
		]
	},
	{
		name: "DELTA",
		description: "Tests whether two numbers are equal.",
		arguments: [
			{
				name: "number1",
				description: "is the first number"
			},
			{
				name: "number2",
				description: "is the second number"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "Returns the sum of squares of deviations of data points from their sample mean.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 arguments, or an array or array reference, on which you want DEVSQ to calculate"
			},
			{
				name: "number2",
				description: "are 1 to 255 arguments, or an array or array reference, on which you want DEVSQ to calculate"
			}
		]
	},
	{
		name: "DGET",
		description: "Extracts from a database a single record that matches the conditions you specify.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "DISC",
		description: "Returns the discount rate for a security.",
		arguments: [
			{
				name: "settlement",
				description: "is the security's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the security's maturity date, expressed as a serial date number"
			},
			{
				name: "pr",
				description: "is the security's price per $100 face value"
			},
			{
				name: "redemption",
				description: "is the security's redemption value per $100 face value"
			},
			{
				name: "basis",
				description: "is the type of day count basis to use"
			}
		]
	},
	{
		name: "DMAX",
		description: "Returns the largest number in the field (column) of records in the database that match the conditions you specify.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "DMIN",
		description: "Returns the smallest number in the field (column) of records in the database that match the conditions you specify.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "Converts a number to text, using currency format.",
		arguments: [
			{
				name: "number",
				description: "is a number, a reference to a cell containing a number, or a formula that evaluates to a number"
			},
			{
				name: "decimals",
				description: "is the number of digits to the right of the decimal point. The number is rounded as necessary; if omitted, Decimals = 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Converts a dollar price, expressed as a fraction, into a dollar price, expressed as a decimal number.",
		arguments: [
			{
				name: "fractional_dollar",
				description: "is a number expressed as a fraction"
			},
			{
				name: "fraction",
				description: "is the integer to use in the denominator of the fraction"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "Converts a dollar price, expressed as a decimal number, into a dollar price, expressed as a fraction.",
		arguments: [
			{
				name: "decimal_dollar",
				description: "is a decimal number"
			},
			{
				name: "fraction",
				description: "is the integer to use in the denominator of a fraction"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "Multiplies the values in the field (column) of records in the database that match the conditions you specify.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "Estimates the standard deviation based on a sample from selected database entries.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "Calculates the standard deviation based on the entire population of selected database entries.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "DSUM",
		description: "Adds the numbers in the field (column) of records in the database that match the conditions you specify.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "DVAR",
		description: "Estimates variance based on a sample from selected database entries.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "DVARP",
		description: "Calculates variance based on the entire population of selected database entries.",
		arguments: [
			{
				name: "database",
				description: "is the range of cells that makes up the list or database. A database is a list of related data"
			},
			{
				name: "field",
				description: "is either the label of the column in double quotation marks or a number that represents the column's position in the list"
			},
			{
				name: "criteria",
				description: "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition"
			}
		]
	},
	{
		name: "EDATE",
		description: "Returns the serial number of the date that is the indicated number of months before or after the start date.",
		arguments: [
			{
				name: "start_date",
				description: "is a serial date number that represents the start date"
			},
			{
				name: "months",
				description: "is the number of months before or after start_date"
			}
		]
	},
	{
		name: "EFFECT",
		description: "Returns the effective annual interest rate.",
		arguments: [
			{
				name: "nominal_rate",
				description: "is the nominal interest rate"
			},
			{
				name: "npery",
				description: "is the number of compounding periods per year"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Returns a URL-encoded string.",
		arguments: [
			{
				name: "text",
				description: "is a string to be URL encoded"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Returns the serial number of the last day of the month before or after a specified number of months.",
		arguments: [
			{
				name: "start_date",
				description: "is a serial date number that represents the start date"
			},
			{
				name: "months",
				description: "is the number of months before or after the start_date"
			}
		]
	},
	{
		name: "ERF",
		description: "Returns the error function.",
		arguments: [
			{
				name: "lower_limit",
				description: "is the lower bound for integrating ERF"
			},
			{
				name: "upper_limit",
				description: "is the upper bound for integrating ERF"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Returns the error function.",
		arguments: [
			{
				name: "X",
				description: "is the lower bound for integrating ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC",
		description: "Returns the complementary error function.",
		arguments: [
			{
				name: "x",
				description: "is the lower bound for integrating ERF"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Returns the complementary error function.",
		arguments: [
			{
				name: "X",
				description: "is the lower bound for integrating ERFC.PRECISE"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Returns a number matching an error value.",
		arguments: [
			{
				name: "error_val",
				description: "is the error value for which you want the identifying number, and can be an actual error value or a reference to a cell containing an error value"
			}
		]
	},
	{
		name: "EVEN",
		description: "Rounds a positive number up and negative number down to the nearest even integer.",
		arguments: [
			{
				name: "number",
				description: "is the value to round"
			}
		]
	},
	{
		name: "EXACT",
		description: "Checks whether two text strings are exactly the same, and returns TRUE or FALSE. EXACT is case-sensitive.",
		arguments: [
			{
				name: "text1",
				description: "is the first text string"
			},
			{
				name: "text2",
				description: "is the second text string"
			}
		]
	},
	{
		name: "EXP",
		description: "Returns e raised to the power of a given number.",
		arguments: [
			{
				name: "number",
				description: "is the exponent applied to the base e. The constant e equals 2.71828182845904, the base of the natural logarithm"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Returns the exponential distribution.",
		arguments: [
			{
				name: "x",
				description: "is the value of the function, a nonnegative number"
			},
			{
				name: "lambda",
				description: "is the parameter value, a positive number"
			},
			{
				name: "cumulative",
				description: "is a logical value for the function to return: the cumulative distribution function = TRUE; the probability density function = FALSE"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Returns the exponential distribution.",
		arguments: [
			{
				name: "x",
				description: "is the value of the function, a nonnegative number"
			},
			{
				name: "lambda",
				description: "is the parameter value, a positive number"
			},
			{
				name: "cumulative",
				description: "is a logical value for the function to return: the cumulative distribution function = TRUE; the probability density function = FALSE"
			}
		]
	},
	{
		name: "F.DIST",
		description: "Returns the (left-tailed) F probability distribution (degree of diversity) for two data sets.",
		arguments: [
			{
				name: "x",
				description: "is the value at which to evaluate the function, a nonnegative number"
			},
			{
				name: "deg_freedom1",
				description: "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			},
			{
				name: "deg_freedom2",
				description: "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			},
			{
				name: "cumulative",
				description: "is a logical value for the function to return: the cumulative distribution function = TRUE; the probability density function = FALSE"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "Returns the (right-tailed) F probability distribution (degree of diversity) for two data sets.",
		arguments: [
			{
				name: "x",
				description: "is the value at which to evaluate the function, a nonnegative number"
			},
			{
				name: "deg_freedom1",
				description: "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			},
			{
				name: "deg_freedom2",
				description: "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "Returns the inverse of the (left-tailed) F probability distribution: if p = F.DIST(x,...), then F.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "is a probability associated with the F cumulative distribution, a number between 0 and 1 inclusive"
			},
			{
				name: "deg_freedom1",
				description: "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			},
			{
				name: "deg_freedom2",
				description: "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Returns the inverse of the (right-tailed) F probability distribution: if p = F.DIST.RT(x,...), then F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "is a probability associated with the F cumulative distribution, a number between 0 and 1 inclusive"
			},
			{
				name: "deg_freedom1",
				description: "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			},
			{
				name: "deg_freedom2",
				description: "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Returns the result of an F-test, the two-tailed probability that the variances in Array1 and Array2 are not significantly different.",
		arguments: [
			{
				name: "array1",
				description: "is the first array or range of data and can be numbers or names, arrays, or references that contain numbers (blanks are ignored)"
			},
			{
				name: "array2",
				description: "is the second array or range of data and can be numbers or names, arrays, or references that contain numbers (blanks are ignored)"
			}
		]
	},
	{
		name: "FACT",
		description: "Returns the factorial of a number, equal to 1*2*3*...* Number.",
		arguments: [
			{
				name: "number",
				description: "is the nonnegative number you want the factorial of"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Returns the double factorial of a number.",
		arguments: [
			{
				name: "number",
				description: "is the value for which to return the double factorial"
			}
		]
	},
	{
		name: "FALSE",
		description: "Returns the logical value FALSE.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "Returns the (right-tailed) F probability distribution (degree of diversity) for two data sets.",
		arguments: [
			{
				name: "x",
				description: "is the value at which to evaluate the function, a nonnegative number"
			},
			{
				name: "deg_freedom1",
				description: "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			},
			{
				name: "deg_freedom2",
				description: "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			}
		]
	},
	{
		name: "FIELD",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIELDPICTURE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIND",
		description: "Returns the starting position of one text string within another text string. FIND is case-sensitive.",
		arguments: [
			{
				name: "find_text",
				description: "is the text you want to find. Use double quotes (empty text) to match the first character in Within_text; wildcard characters not allowed"
			},
			{
				name: "within_text",
				description: "is the text containing the text you want to find"
			},
			{
				name: "start_num",
				description: "specifies the character at which to start the search. The first character in Within_text is character number 1. If omitted, Start_num = 1"
			}
		]
	},
	{
		name: "FINV",
		description: "Returns the inverse of the (right-tailed) F probability distribution: if p = FDIST(x,...), then FINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "is a probability associated with the F cumulative distribution, a number between 0 and 1 inclusive"
			},
			{
				name: "deg_freedom1",
				description: "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			},
			{
				name: "deg_freedom2",
				description: "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "Returns the Fisher transformation.",
		arguments: [
			{
				name: "x",
				description: "is the value for which you want the transformation, a number between -1 and 1, excluding -1 and 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Returns the inverse of the Fisher transformation: if y = FISHER(x), then FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "is the value for which you want to perform the inverse of the transformation"
			}
		]
	},
	{
		name: "FIXED",
		description: "Rounds a number to the specified number of decimals and returns the result as text with or without commas.",
		arguments: [
			{
				name: "number",
				description: "is the number you want to round and convert to text"
			},
			{
				name: "decimals",
				description: "is the number of digits to the right of the decimal point. If omitted, Decimals = 2"
			},
			{
				name: "no_commas",
				description: "is a logical value: do not display commas in the returned text = TRUE; do display commas in the returned text = FALSE or omitted"
			}
		]
	},
	{
		name: "FLOOR",
		description: "Rounds a number down to the nearest multiple of significance.",
		arguments: [
			{
				name: "number",
				description: "is the numeric value you want to round"
			},
			{
				name: "significance",
				description: "is the multiple to which you want to round. Number and Significance must either both be positive or both be negative"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Rounds a number down, to the nearest integer or to the nearest multiple of significance.",
		arguments: [
			{
				name: "number",
				description: "is the value you want to round"
			},
			{
				name: "significance",
				description: "is the multiple to which you want to round"
			},
			{
				name: "mode",
				description: "when given and nonzero this function will round towards zero"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Rounds a number down, to the nearest integer or to the nearest multiple of significance.",
		arguments: [
			{
				name: "number",
				description: "is the numeric value you want to round"
			},
			{
				name: "significance",
				description: "is the multiple to which you want to round. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "Calculates, or predicts, a future value along a linear trend by using existing values.",
		arguments: [
			{
				name: "x",
				description: "is the data point for which you want to predict a value and must be a numeric value"
			},
			{
				name: "known_y's",
				description: "is the dependent array or range of numeric data"
			},
			{
				name: "known_x's",
				description: "is the independent array or range of numeric data. The variance of Known_x's must not be zero"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Returns a formula as a string.",
		arguments: [
			{
				name: "reference",
				description: "is a reference to a formula"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "Calculates how often values occur within a range of values and then returns a vertical array of numbers having one more element than Bins_array.",
		arguments: [
			{
				name: "data_array",
				description: "is an array of or reference to a set of values for which you want to count frequencies (blanks and text are ignored)"
			},
			{
				name: "bins_array",
				description: "is an array of or reference to intervals into which you want to group the values in data_array"
			}
		]
	},
	{
		name: "FTEST",
		description: "Returns the result of an F-test, the two-tailed probability that the variances in Array1 and Array2 are not significantly different.",
		arguments: [
			{
				name: "array1",
				description: "is the first array or range of data and can be numbers or names, arrays, or references that contain numbers (blanks are ignored)"
			},
			{
				name: "array2",
				description: "is the second array or range of data and can be numbers or names, arrays, or references that contain numbers (blanks are ignored)"
			}
		]
	},
	{
		name: "FV",
		description: "Returns the future value of an investment based on periodic, constant payments and a constant interest rate.",
		arguments: [
			{
				name: "rate",
				description: "is the interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR"
			},
			{
				name: "nper",
				description: "is the total number of payment periods in the investment"
			},
			{
				name: "pmt",
				description: "is the payment made each period; it cannot change over the life of the investment"
			},
			{
				name: "pv",
				description: "is the present value, or the lump-sum amount that a series of future payments is worth now. If omitted, Pv = 0"
			},
			{
				name: "type",
				description: "is a value representing the timing of payment: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Returns the future value of an initial principal after applying a series of compound interest rates.",
		arguments: [
			{
				name: "principal",
				description: "is the present value"
			},
			{
				name: "schedule",
				description: "is an array of interest rates to apply"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Returns the Gamma function value.",
		arguments: [
			{
				name: "x",
				description: "is the value for which you want to calculate Gamma"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Returns the gamma distribution.",
		arguments: [
			{
				name: "x",
				description: "is the value at which you want to evaluate the distribution, a nonnegative number"
			},
			{
				name: "alpha",
				description: "is a parameter to the distribution, a positive number"
			},
			{
				name: "beta",
				description: "is a parameter to the distribution, a positive number. If beta = 1, GAMMA.DIST returns the standard gamma distribution"
			},
			{
				name: "cumulative",
				description: "is a logical value: return the cumulative distribution function = TRUE; return the probability mass function = FALSE or omitted"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Returns the inverse of the gamma cumulative distribution: if p = GAMMA.DIST(x,...), then GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "is the probability associated with the gamma distribution, a number between 0 and 1, inclusive"
			},
			{
				name: "alpha",
				description: "is a parameter to the distribution, a positive number"
			},
			{
				name: "beta",
				description: "is a parameter to the distribution, a positive number. If beta = 1, GAMMA.INV returns the inverse of the standard gamma distribution"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Returns the gamma distribution.",
		arguments: [
			{
				name: "x",
				description: "is the value at which you want to evaluate the distribution, a nonnegative number"
			},
			{
				name: "alpha",
				description: "is a parameter to the distribution, a positive number"
			},
			{
				name: "beta",
				description: "is a parameter to the distribution, a positive number. If beta = 1, GAMMADIST returns the standard gamma distribution"
			},
			{
				name: "cumulative",
				description: "is a logical value: return the cumulative distribution function = TRUE; return the probability mass function = FALSE or omitted"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Returns the inverse of the gamma cumulative distribution: if p = GAMMADIST(x,...), then GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "is the probability associated with the gamma distribution, a number between 0 and 1, inclusive"
			},
			{
				name: "alpha",
				description: "is a parameter to the distribution, a positive number"
			},
			{
				name: "beta",
				description: "is a parameter to the distribution, a positive number. If beta = 1, GAMMAINV returns the inverse of the standard gamma distribution"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Returns the natural logarithm of the gamma function.",
		arguments: [
			{
				name: "x",
				description: "is the value for which you want to calculate GAMMALN, a positive number"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Returns the natural logarithm of the gamma function.",
		arguments: [
			{
				name: "x",
				description: "is the value for which you want to calculate GAMMALN.PRECISE, a positive number"
			}
		]
	},
	{
		name: "GAUSS",
		description: "",
		arguments: [
		]
	},
	{
		name: "GCD",
		description: "Returns the greatest common divisor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 values"
			},
			{
				name: "number2",
				description: "are 1 to 255 values"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Returns the geometric mean of an array or range of positive numeric data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the mean"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the mean"
			}
		]
	},
	{
		name: "GESTEP",
		description: "Tests whether a number is greater than a threshold value.",
		arguments: [
			{
				name: "number",
				description: "is the value to test against step"
			},
			{
				name: "step",
				description: "is the threshold value"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "Extracts data stored in a PivotTable.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "data_field",
				description: "is the name of the data field to extract data from"
			},
			{
				name: "pivot_table",
				description: "is a reference to a cell or range of cells in the PivotTable that contains the data you want to retrieve"
			},
			{
				name: "field",
				description: "field to refer to"
			},
			{
				name: "item",
				description: "field item to refer to"
			}
		]
	},
	{
		name: "GROWTH",
		description: "Returns numbers in an exponential growth trend matching known data points.",
		arguments: [
			{
				name: "known_y's",
				description: "is the set of y-values you already know in the relationship y = b*m^x, an array or range of positive numbers"
			},
			{
				name: "known_x's",
				description: "is an optional set of x-values that you may already know in the relationship y = b*m^x, an array or range the same size as Known_y's"
			},
			{
				name: "new_x's",
				description: "are new x-values for which you want GROWTH to return corresponding y-values"
			},
			{
				name: "const",
				description: "is a logical value: the constant b is calculated normally if Const = TRUE; b is set equal to 1 if Const = FALSE or omitted"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Returns the harmonic mean of a data set of positive numbers: the reciprocal of the arithmetic mean of reciprocals.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the harmonic mean"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the harmonic mean"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Converts a Hexadecimal number to binary.",
		arguments: [
			{
				name: "number",
				description: "is the hexadecimal number you want to convert"
			},
			{
				name: "places",
				description: "is the number of characters to use"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Converts a hexadecimal number to decimal.",
		arguments: [
			{
				name: "number",
				description: "is the hexadecimal number you want to convert"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Converts a hexadecimal number to octal.",
		arguments: [
			{
				name: "number",
				description: "is the hexadecimal number you want to convert"
			},
			{
				name: "places",
				description: "is the number of characters to use"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "Looks for a value in the top row of a table or array of values and returns the value in the same column from a row you specify.",
		arguments: [
			{
				name: "lookup_value",
				description: "is the value to be found in the first row of the table and can be a value, a reference, or a text string"
			},
			{
				name: "table_array",
				description: "is a table of text, numbers, or logical values in which data is looked up. Table_array can be a reference to a range or a range name"
			},
			{
				name: "row_index_num",
				description: "is the row number in table_array from which the matching value should be returned. The first row of values in the table is row 1"
			},
			{
				name: "range_lookup",
				description: "is a logical value: to find the closest match in the top row (sorted in ascending order) = TRUE or omitted; find an exact match = FALSE"
			}
		]
	},
	{
		name: "HOUR",
		description: "Returns the hour as a number from 0 (12:00 A.M.) to 23 (11:00 P.M.).",
		arguments: [
			{
				name: "serial_number",
				description: "is a number in the date-time code used by Spreadsheet, or text in time format, such as 16:48:00 or 4:48:00 PM"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Creates a shortcut or jump that opens a document stored on your hard drive, a network server, or on the Internet.",
		arguments: [
			{
				name: "link_location",
				description: "is the text giving the path and file name to the document to be opened, a hard drive location, UNC address, or URL path"
			},
			{
				name: "friendly_name",
				description: "is text or a number that is displayed in the cell. If omitted, the cell displays the Link_location text"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Returns the hypergeometric distribution.",
		arguments: [
			{
				name: "sample_s",
				description: "is the number of successes in the sample"
			},
			{
				name: "number_sample",
				description: "is the size of the sample"
			},
			{
				name: "population_s",
				description: "is the number of successes in the population"
			},
			{
				name: "number_pop",
				description: "is the population size"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Returns the hypergeometric distribution.",
		arguments: [
			{
				name: "sample_s",
				description: "is the number of successes in the sample"
			},
			{
				name: "number_sample",
				description: "is the size of the sample"
			},
			{
				name: "population_s",
				description: "is the number of successes in the population"
			},
			{
				name: "number_pop",
				description: "is the population size"
			}
		]
	},
	{
		name: "IF",
		description: "Checks whether a condition is met, and returns one value if TRUE, and another value if FALSE.",
		arguments: [
			{
				name: "logical_test",
				description: "is any value or expression that can be evaluated to TRUE or FALSE"
			},
			{
				name: "value_if_true",
				description: "is the value that is returned if Logical_test is TRUE. If omitted, TRUE is returned. You can nest up to seven IF functions"
			},
			{
				name: "value_if_false",
				description: "is the value that is returned if Logical_test is FALSE. If omitted, FALSE is returned"
			}
		]
	},
	{
		name: "IFERROR",
		description: "Returns value_if_error if expression is an error and the value of the expression itself otherwise.",
		arguments: [
			{
				name: "value",
				description: "is any value or expression or reference"
			},
			{
				name: "value_if_error",
				description: "is any value or expression or reference"
			}
		]
	},
	{
		name: "IFNA",
		description: "Returns the value you specify if the expression resolves to #N/A, otherwise returns the result of the expression.",
		arguments: [
			{
				name: "value",
				description: "is any value or expression or reference"
			},
			{
				name: "value_if_na",
				description: "is any value or expression or reference"
			}
		]
	},
	{
		name: "IMABS",
		description: "Returns the absolute value (modulus) of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the absolute value"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Returns the imaginary coefficient of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the imaginary coefficient"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Returns the argument q, an angle expressed in radians.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the argument"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Returns the complex conjugate of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the conjugate"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Returns the cosine of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the cosine"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Returns the hyperbolic cosine of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the hyperbolic cosine"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Returns the cotangent of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the cotangent"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Returns the cosecant of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the cosecant"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Returns the hyperbolic cosecant of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the hyperbolic cosecant"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Returns the quotient of two complex numbers.",
		arguments: [
			{
				name: "inumber1",
				description: "is the complex numerator or dividend"
			},
			{
				name: "inumber2",
				description: "is the complex denominator or divisor"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Returns the exponential of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the exponential"
			}
		]
	},
	{
		name: "IMLN",
		description: "Returns the natural logarithm of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the natural logarithm"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Returns the base-10 logarithm of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the common logarithm"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Returns the base-2 logarithm of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the base-2 logarithm"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Returns a complex number raised to an integer power.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number you want to raise to a power"
			},
			{
				name: "number",
				description: "is the power to which you want to raise the complex number"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Returns the product of 1 to 255 complex numbers.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "Inumber1, Inumber2,... are from 1 to 255 complex numbers to multiply."
			},
			{
				name: "inumber2",
				description: "Inumber1, Inumber2,... are from 1 to 255 complex numbers to multiply."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Returns the real coefficient of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the real coefficient"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Returns the secant of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the secant"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Returns the hyperbolic secant of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the hyperbolic secant"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Returns the sine of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the sine"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Returns the hyperbolic sine of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the hyperbolic sine"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Returns the square root of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the square root"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Returns the difference of two complex numbers.",
		arguments: [
			{
				name: "inumber1",
				description: "is the complex number from which to subtract inumber2"
			},
			{
				name: "inumber2",
				description: "is the complex number to subtract from inumber1"
			}
		]
	},
	{
		name: "IMSUM",
		description: "Returns the sum of complex numbers.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "are from 1 to 255 complex numbers to add"
			},
			{
				name: "inumber2",
				description: "are from 1 to 255 complex numbers to add"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Returns the tangent of a complex number.",
		arguments: [
			{
				name: "inumber",
				description: "is a complex number for which you want the tangent"
			}
		]
	},
	{
		name: "INDEX",
		description: "Returns a value or reference of the cell at the intersection of a particular row and column, in a given range.",
		arguments: [
			{
				name: "array",
				description: "is a range of cells or an array constant."
			},
			{
				name: "row_num",
				description: "selects the row in Array or Reference from which to return a value. If omitted, Column_num is required"
			},
			{
				name: "column_num",
				description: "selects the column in Array or Reference from which to return a value. If omitted, Row_num is required"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Returns the reference specified by a text string.",
		arguments: [
			{
				name: "ref_text",
				description: "is a reference to a cell that contains an A1- or R1C1-style reference, a name defined as a reference, or a reference to a cell as a text string"
			},
			{
				name: "a1",
				description: "is a logical value that specifies the type of reference in Ref_text: R1C1-style = FALSE; A1-style = TRUE or omitted"
			}
		]
	},
	{
		name: "INFO",
		description: "Returns information about the current operating environment.",
		arguments: [
			{
				name: "type_text",
				description: "is text specifying what type of information you want returned."
			}
		]
	},
	{
		name: "INT",
		description: "Rounds a number down to the nearest integer.",
		arguments: [
			{
				name: "number",
				description: "is the real number you want to round down to an integer"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Calculates the point at which a line will intersect the y-axis by using a best-fit regression line plotted through the known x-values and y-values.",
		arguments: [
			{
				name: "known_y's",
				description: "is the dependent set of observations or data and can be numbers or names, arrays, or references that contain numbers"
			},
			{
				name: "known_x's",
				description: "is the independent set of observations or data and can be numbers or names, arrays, or references that contain numbers"
			}
		]
	},
	{
		name: "INTRATE",
		description: "Returns the interest rate for a fully invested security.",
		arguments: [
			{
				name: "settlement",
				description: "is the security's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the security's maturity date, expressed as a serial date number"
			},
			{
				name: "investment",
				description: "is the amount invested in the security"
			},
			{
				name: "redemption",
				description: "is the amount to be received at maturity"
			},
			{
				name: "basis",
				description: "is the type of day count basis to use"
			}
		]
	},
	{
		name: "IPMT",
		description: "Returns the interest payment for a given period for an investment, based on periodic, constant payments and a constant interest rate.",
		arguments: [
			{
				name: "rate",
				description: "is the interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR"
			},
			{
				name: "per",
				description: "is the period for which you want to find the interest and must be in the range 1 to Nper"
			},
			{
				name: "nper",
				description: "is the total number of payment periods in an investment"
			},
			{
				name: "pv",
				description: "is the present value, or the lump-sum amount that a series of future payments is worth now"
			},
			{
				name: "fv",
				description: "is the future value, or a cash balance you want to attain after the last payment is made. If omitted, Fv = 0"
			},
			{
				name: "type",
				description: "is a logical value representing the timing of payment: at the end of the period = 0 or omitted, at the beginning of the period = 1"
			}
		]
	},
	{
		name: "IRR",
		description: "Returns the internal rate of return for a series of cash flows.",
		arguments: [
			{
				name: "values",
				description: "is an array or a reference to cells that contain numbers for which you want to calculate the internal rate of return"
			},
			{
				name: "guess",
				description: "is a number that you guess is close to the result of IRR; 0.1 (10 percent) if omitted"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "Checks whether a reference is to an empty cell, and returns TRUE or FALSE.",
		arguments: [
			{
				name: "value",
				description: "is the cell or a name that refers to the cell you want to test"
			}
		]
	},
	{
		name: "ISERR",
		description: "Checks whether a value is an error (#VALUE!, #REF!, #DIV/0!, #NUM!, #NAME?, or #NULL!) excluding #N/A, and returns TRUE or FALSE.",
		arguments: [
			{
				name: "value",
				description: "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value"
			}
		]
	},
	{
		name: "ISERROR",
		description: "Checks whether a value is an error (#N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME?, or #NULL!), and returns TRUE or FALSE.",
		arguments: [
			{
				name: "value",
				description: "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Returns TRUE if the number is even.",
		arguments: [
			{
				name: "number",
				description: "is the value to test"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Checks whether a reference is to a cell containing a formula, and returns TRUE or FALSE.",
		arguments: [
			{
				name: "reference",
				description: "is a reference to the cell you want to test.  Reference can be a cell reference, a formula, or name that refers to a cell"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "Checks whether a value is a logical value (TRUE or FALSE), and returns TRUE or FALSE.",
		arguments: [
			{
				name: "value",
				description: "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value"
			}
		]
	},
	{
		name: "ISNA",
		description: "Checks whether a value is #N/A, and returns TRUE or FALSE.",
		arguments: [
			{
				name: "value",
				description: "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "Checks whether a value is not text (blank cells are not text), and returns TRUE or FALSE.",
		arguments: [
			{
				name: "value",
				description: "is the value you want tested: a cell; a formula; or a name referring to a cell, formula, or value"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "Checks whether a value is a number, and returns TRUE or FALSE.",
		arguments: [
			{
				name: "value",
				description: "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Rounds a number up, to the nearest integer or to the nearest multiple of significance.",
		arguments: [
			{
				name: "number",
				description: "is the value you want to round"
			},
			{
				name: "significance",
				description: "is the optional multiple to which you want to round"
			}
		]
	},
	{
		name: "ISODD",
		description: "Returns TRUE if the number is odd.",
		arguments: [
			{
				name: "number",
				description: "is the value to test"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Returns number of the ISO week number of the year for a given date.",
		arguments: [
			{
				name: "date",
				description: "is the date-time code used by Spreadsheet for date and time calculation"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Returns the interest paid during a specific period of an investment.",
		arguments: [
			{
				name: "rate",
				description: "interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR"
			},
			{
				name: "per",
				description: "period for which you want to find the interest"
			},
			{
				name: "nper",
				description: "number of payment periods in an investment"
			},
			{
				name: "pv",
				description: "lump sum amount that a series of future payments is right now"
			}
		]
	},
	{
		name: "ISREF",
		description: "Checks whether a value is a reference, and returns TRUE or FALSE.",
		arguments: [
			{
				name: "value",
				description: "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "Checks whether a value is text, and returns TRUE or FALSE.",
		arguments: [
			{
				name: "value",
				description: "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value"
			}
		]
	},
	{
		name: "KURT",
		description: "Returns the kurtosis of a data set.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the kurtosis"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the kurtosis"
			}
		]
	},
	{
		name: "LARGE",
		description: "Returns the k-th largest value in a data set. For example, the fifth largest number.",
		arguments: [
			{
				name: "array",
				description: "is the array or range of data for which you want to determine the k-th largest value"
			},
			{
				name: "k",
				description: "is the position (from the largest) in the array or cell range of the value to return"
			}
		]
	},
	{
		name: "LCM",
		description: "Returns the least common multiple.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 values for which you want the least common multiple"
			},
			{
				name: "number2",
				description: "are 1 to 255 values for which you want the least common multiple"
			}
		]
	},
	{
		name: "LEFT",
		description: "Returns the specified number of characters from the start of a text string.",
		arguments: [
			{
				name: "text",
				description: "is the text string containing the characters you want to extract"
			},
			{
				name: "num_chars",
				description: "specifies how many characters you want LEFT to extract; 1 if omitted"
			}
		]
	},
	{
		name: "LEN",
		description: "Returns the number of characters in a text string.",
		arguments: [
			{
				name: "text",
				description: "is the text whose length you want to find. Spaces count as characters"
			}
		]
	},
	{
		name: "LINEST",
		description: "Returns statistics that describe a linear trend matching known data points, by fitting a straight line using the least squares method.",
		arguments: [
			{
				name: "known_y's",
				description: "is the set of y-values you already know in the relationship y = mx + b"
			},
			{
				name: "known_x's",
				description: "is an optional set of x-values that you may already know in the relationship y = mx + b"
			},
			{
				name: "const",
				description: "is a logical value: the constant b is calculated normally if Const = TRUE or omitted; b is set equal to 0 if Const = FALSE"
			},
			{
				name: "stats",
				description: "is a logical value: return additional regression statistics = TRUE; return m-coefficients and the constant b = FALSE or omitted"
			}
		]
	},
	{
		name: "LN",
		description: "Returns the natural logarithm of a number.",
		arguments: [
			{
				name: "number",
				description: "is the positive real number for which you want the natural logarithm"
			}
		]
	},
	{
		name: "LOG",
		description: "Returns the logarithm of a number to the base you specify.",
		arguments: [
			{
				name: "number",
				description: "is the positive real number for which you want the logarithm"
			},
			{
				name: "base",
				description: "is the base of the logarithm; 10 if omitted"
			}
		]
	},
	{
		name: "LOG10",
		description: "Returns the base-10 logarithm of a number.",
		arguments: [
			{
				name: "number",
				description: "is the positive real number for which you want the base-10 logarithm"
			}
		]
	},
	{
		name: "LOGEST",
		description: "Returns statistics that describe an exponential curve matching known data points.",
		arguments: [
			{
				name: "known_y's",
				description: "is the set of y-values you already know in the relationship y = b*m^x"
			},
			{
				name: "known_x's",
				description: "is an optional set of x-values that you may already know in the relationship y = b*m^x"
			},
			{
				name: "const",
				description: "is a logical value: the constant b is calculated normally if Const = TRUE or omitted; b is set equal to 1 if Const = FALSE"
			},
			{
				name: "stats",
				description: "is a logical value: return additional regression statistics = TRUE; return m-coefficients and the constant b = FALSE or omitted"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Returns the inverse of the lognormal cumulative distribution function of x, where ln(x) is normally distributed with parameters Mean and Standard_dev.",
		arguments: [
			{
				name: "probability",
				description: "is a probability associated with the lognormal distribution, a number between 0 and 1, inclusive"
			},
			{
				name: "mean",
				description: "is the mean of ln(x)"
			},
			{
				name: "standard_dev",
				description: "is the standard deviation of ln(x), a positive number"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "Returns the lognormal distribution of x, where ln(x) is normally distributed with parameters Mean and Standard_dev.",
		arguments: [
			{
				name: "x",
				description: "is the value at which to evaluate the function, a positive number"
			},
			{
				name: "mean",
				description: "is the mean of ln(x)"
			},
			{
				name: "standard_dev",
				description: "is the standard deviation of ln(x), a positive number"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Returns the inverse of the lognormal cumulative distribution function of x, where ln(x) is normally distributed with parameters Mean and Standard_dev.",
		arguments: [
			{
				name: "probability",
				description: "is a probability associated with the lognormal distribution, a number between 0 and 1, inclusive"
			},
			{
				name: "mean",
				description: "is the mean of ln(x)"
			},
			{
				name: "standard_dev",
				description: "is the standard deviation of ln(x), a positive number"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "Returns the cumulative lognormal distribution of x, where ln(x) is normally distributed with parameters Mean and Standard_dev.",
		arguments: [
			{
				name: "x",
				description: "is the value at which to evaluate the function, a positive number"
			},
			{
				name: "mean",
				description: "is the mean of ln(x)"
			},
			{
				name: "standard_dev",
				description: "is the standard deviation of ln(x), a positive number"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "Looks up a value either from a one-row or one-column range or from an array. Provided for backward compatibility.",
		arguments: [
			{
				name: "lookup_value",
				description: "is a value that LOOKUP searches for in Lookup_vector and can be a number, text, a logical value, or a name or reference to a value"
			},
			{
				name: "lookup_vector",
				description: "is a range that contains only one row or one column of text, numbers, or logical values, placed in ascending order"
			},
			{
				name: "result_vector",
				description: "is a range that contains only one row or column, the same size as Lookup_vector"
			}
		]
	},
	{
		name: "LOWER",
		description: "Converts all letters in a text string to lowercase.",
		arguments: [
			{
				name: "text",
				description: "is the text you want to convert to lowercase. Characters in Text that are not letters are not changed"
			}
		]
	},
	{
		name: "MATCH",
		description: "Returns the relative position of an item in an array that matches a specified value in a specified order.",
		arguments: [
			{
				name: "lookup_value",
				description: "is the value you use to find the value you want in the array, a number, text, or logical value, or a reference to one of these"
			},
			{
				name: "lookup_array",
				description: "is a contiguous range of cells containing possible lookup values, an array of values, or a reference to an array"
			},
			{
				name: "match_type",
				description: "is a number 1, 0, or -1 indicating which value to return."
			}
		]
	},
	{
		name: "MAX",
		description: "Returns the largest value in a set of values. Ignores logical values and text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the maximum"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the maximum"
			}
		]
	},
	{
		name: "MAXA",
		description: "Returns the largest value in a set of values. Does not ignore logical values and text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the maximum"
			},
			{
				name: "value2",
				description: "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the maximum"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Returns the matrix determinant of an array.",
		arguments: [
			{
				name: "array",
				description: "is a numeric array with an equal number of rows and columns, either a cell range or an array constant"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Returns the median, or the number in the middle of the set of given numbers.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the median"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the median"
			}
		]
	},
	{
		name: "MID",
		description: "Returns the characters from the middle of a text string, given a starting position and length.",
		arguments: [
			{
				name: "text",
				description: "is the text string from which you want to extract the characters"
			},
			{
				name: "start_num",
				description: "is the position of the first character you want to extract. The first character in Text is 1"
			},
			{
				name: "num_chars",
				description: "specifies how many characters to return from Text"
			}
		]
	},
	{
		name: "MIN",
		description: "Returns the smallest number in a set of values. Ignores logical values and text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the minimum"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the minimum"
			}
		]
	},
	{
		name: "MINA",
		description: "Returns the smallest value in a set of values. Does not ignore logical values and text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the minimum"
			},
			{
				name: "value2",
				description: "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the minimum"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Returns the minute, a number from 0 to 59.",
		arguments: [
			{
				name: "serial_number",
				description: "is a number in the date-time code used by Spreadsheet or text in time format, such as 16:48:00 or 4:48:00 PM"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "Returns the inverse matrix for the matrix stored in an array.",
		arguments: [
			{
				name: "array",
				description: "is a numeric array with an equal number of rows and columns, either a cell range or an array constant"
			}
		]
	},
	{
		name: "MIRR",
		description: "Returns the internal rate of return for a series of periodic cash flows, considering both cost of investment and interest on reinvestment of cash.",
		arguments: [
			{
				name: "values",
				description: "is an array or a reference to cells that contain numbers that represent a series of payments (negative) and income (positive) at regular periods"
			},
			{
				name: "finance_rate",
				description: "is the interest rate you pay on the money used in the cash flows"
			},
			{
				name: "reinvest_rate",
				description: "is the interest rate you receive on the cash flows as you reinvest them"
			}
		]
	},
	{
		name: "MMULT",
		description: "Returns the matrix product of two arrays, an array with the same number of rows as array1 and columns as array2.",
		arguments: [
			{
				name: "array1",
				description: "is the first array of numbers to multiply and must have the same number of columns as Array2 has rows"
			},
			{
				name: "array2",
				description: "is the first array of numbers to multiply and must have the same number of columns as Array2 has rows"
			}
		]
	},
	{
		name: "MOD",
		description: "Returns the remainder after a number is divided by a divisor.",
		arguments: [
			{
				name: "number",
				description: "is the number for which you want to find the remainder after the division is performed"
			},
			{
				name: "divisor",
				description: "is the number by which you want to divide Number"
			}
		]
	},
	{
		name: "MODE",
		description: "Returns the most frequently occurring, or repetitive, value in an array or range of data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Returns a vertical array of the most frequently occurring, or repetitive, values in an array or range of data.  For a horizontal array, use =TRANSPOSE(MODE.MULT(number1,number2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Returns the most frequently occurring, or repetitive, value in an array or range of data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode"
			}
		]
	},
	{
		name: "MONTH",
		description: "Returns the month, a number from 1 (January) to 12 (December).",
		arguments: [
			{
				name: "serial_number",
				description: "is a number in the date-time code used by Spreadsheet"
			}
		]
	},
	{
		name: "MROUND",
		description: "Returns a number rounded to the desired multiple.",
		arguments: [
			{
				name: "number",
				description: "is the value to round"
			},
			{
				name: "multiple",
				description: "is the multiple to which you want to round number"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Returns the multinomial of a set of numbers.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 values for which you want the multinomial"
			},
			{
				name: "number2",
				description: "are 1 to 255 values for which you want the multinomial"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Returns the unit matrix for the specified dimension.",
		arguments: [
			{
				name: "dimension",
				description: "is an integer specifying the dimension of the unit matrix that you want to return"
			}
		]
	},
	{
		name: "N",
		description: "Converts non-number value to a number, dates to serial numbers, TRUE to 1, anything else to 0 (zero).",
		arguments: [
			{
				name: "value",
				description: "is the value you want converted"
			}
		]
	},
	{
		name: "NA",
		description: "Returns the error value #N/A (value not available).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Returns the negative binomial distribution, the probability that there will be Number_f failures before the Number_s-th success, with Probability_s probability of a success.",
		arguments: [
			{
				name: "number_f",
				description: "is the number of failures"
			},
			{
				name: "number_s",
				description: "is the threshold number of successes"
			},
			{
				name: "probability_s",
				description: "is the probability of a success; a number between 0 and 1"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative distribution function, use TRUE; for the probability mass function, use FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Returns the negative binomial distribution, the probability that there will be Number_f failures before the Number_s-th success, with Probability_s probability of a success.",
		arguments: [
			{
				name: "number_f",
				description: "is the number of failures"
			},
			{
				name: "number_s",
				description: "is the threshold number of successes"
			},
			{
				name: "probability_s",
				description: "is the probability of a success; a number between 0 and 1"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Returns the number of whole workdays between two dates.",
		arguments: [
			{
				name: "start_date",
				description: "is a serial date number that represents the start date"
			},
			{
				name: "end_date",
				description: "is a serial date number that represents the end date"
			},
			{
				name: "holidays",
				description: "is an optional set of one or more serial date numbers to exclude from the working calendar, such as state and federal holidays and floating holidays"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Returns the number of whole workdays between two dates with custom weekend parameters.",
		arguments: [
			{
				name: "start_date",
				description: "is a serial date number that represents the start date"
			},
			{
				name: "end_date",
				description: "is a serial date number that represents the end date"
			},
			{
				name: "weekend",
				description: "is a number or string specifying when weekends occur"
			},
			{
				name: "holidays",
				description: "is an optional set of one or more serial date numbers to exclude from the working calendar, such as state and federal holidays and floating holidays"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Returns the annual nominal interest rate.",
		arguments: [
			{
				name: "effect_rate",
				description: "is the effective interest rate"
			},
			{
				name: "npery",
				description: "is the number of compounding periods per year"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Returns the normal distribution for the specified mean and standard deviation.",
		arguments: [
			{
				name: "x",
				description: "is the value for which you want the distribution"
			},
			{
				name: "mean",
				description: "is the arithmetic mean of the distribution"
			},
			{
				name: "standard_dev",
				description: "is the standard deviation of the distribution, a positive number"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Returns the inverse of the normal cumulative distribution for the specified mean and standard deviation.",
		arguments: [
			{
				name: "probability",
				description: "is a probability corresponding to the normal distribution, a number between 0 and 1 inclusive"
			},
			{
				name: "mean",
				description: "is the arithmetic mean of the distribution"
			},
			{
				name: "standard_dev",
				description: "is the standard deviation of the distribution, a positive number"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Returns the standard normal distribution (has a mean of zero and a standard deviation of one).",
		arguments: [
			{
				name: "z",
				description: "is the value for which you want the distribution"
			},
			{
				name: "cumulative",
				description: "is a logical value for the function to return: the cumulative distribution function = TRUE; the probability density function = FALSE"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Returns the inverse of the standard normal cumulative distribution (has a mean of zero and a standard deviation of one).",
		arguments: [
			{
				name: "probability",
				description: "is a probability corresponding to the normal distribution, a number between 0 and 1 inclusive"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Returns the normal cumulative distribution for the specified mean and standard deviation.",
		arguments: [
			{
				name: "x",
				description: "is the value for which you want the distribution"
			},
			{
				name: "mean",
				description: "is the arithmetic mean of the distribution"
			},
			{
				name: "standard_dev",
				description: "is the standard deviation of the distribution, a positive number"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Returns the inverse of the normal cumulative distribution for the specified mean and standard deviation.",
		arguments: [
			{
				name: "probability",
				description: "is a probability corresponding to the normal distribution, a number between 0 and 1 inclusive"
			},
			{
				name: "mean",
				description: "is the arithmetic mean of the distribution"
			},
			{
				name: "standard_dev",
				description: "is the standard deviation of the distribution, a positive number"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Returns the standard normal cumulative distribution (has a mean of zero and a standard deviation of one).",
		arguments: [
			{
				name: "z",
				description: "is the value for which you want the distribution"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Returns the inverse of the standard normal cumulative distribution (has a mean of zero and a standard deviation of one).",
		arguments: [
			{
				name: "probability",
				description: "is a probability corresponding to the normal distribution, a number between 0 and 1 inclusive"
			}
		]
	},
	{
		name: "NOT",
		description: "Changes FALSE to TRUE, or TRUE to FALSE.",
		arguments: [
			{
				name: "logical",
				description: "is a value or expression that can be evaluated to TRUE or FALSE"
			}
		]
	},
	{
		name: "NOW",
		description: "Returns the current date and time formatted as a date and time.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "Returns the number of periods for an investment based on periodic, constant payments and a constant interest rate.",
		arguments: [
			{
				name: "rate",
				description: "is the interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR"
			},
			{
				name: "pmt",
				description: "is the payment made each period; it cannot change over the life of the investment"
			},
			{
				name: "pv",
				description: "is the present value, or the lump-sum amount that a series of future payments is worth now"
			},
			{
				name: "fv",
				description: "is the future value, or a cash balance you want to attain after the last payment is made. If omitted, zero is used"
			},
			{
				name: "type",
				description: "is a logical value: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted"
			}
		]
	},
	{
		name: "NPV",
		description: "Returns the net present value of an investment based on a discount rate and a series of future payments (negative values) and income (positive values).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rate",
				description: "is the rate of discount over the length of one period"
			},
			{
				name: "value1",
				description: "are 1 to 254 payments and income, equally spaced in time and occurring at the end of each period"
			},
			{
				name: "value2",
				description: "are 1 to 254 payments and income, equally spaced in time and occurring at the end of each period"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Converts text to number in a locale-independent manner.",
		arguments: [
			{
				name: "text",
				description: "is the string representing the number you want to convert"
			},
			{
				name: "decimal_separator",
				description: "is the character used as the decimal separator in the string"
			},
			{
				name: "group_separator",
				description: "is the character used as the group separator in the string"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Converts an octal number to binary.",
		arguments: [
			{
				name: "number",
				description: "is the octal number you want to convert"
			},
			{
				name: "places",
				description: "is the number of characters to use"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Converts an octal number to decimal.",
		arguments: [
			{
				name: "number",
				description: "is the octal number you want to convert"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Converts an octal number to hexadecimal.",
		arguments: [
			{
				name: "number",
				description: "is the octal number you want to convert"
			},
			{
				name: "places",
				description: "is the number of characters to use"
			}
		]
	},
	{
		name: "ODD",
		description: "Rounds a positive number up and negative number down to the nearest odd integer.",
		arguments: [
			{
				name: "number",
				description: "is the value to round"
			}
		]
	},
	{
		name: "OFFSET",
		description: "Returns a reference to a range that is a given number of rows and columns from a given reference.",
		arguments: [
			{
				name: "reference",
				description: "is the reference from which you want to base the offset, a reference to a cell or range of adjacent cells"
			},
			{
				name: "rows",
				description: "is the number of rows, up or down, that you want the upper-left cell of the result to refer to"
			},
			{
				name: "cols",
				description: "is the number of columns, to the left or right, that you want the upper-left cell of the result to refer to"
			},
			{
				name: "height",
				description: "is the height, in number of rows, that you want the result to be, the same height as Reference if omitted"
			},
			{
				name: "width",
				description: "is the width, in number of columns, that you want the result to be, the same width as Reference if omitted"
			}
		]
	},
	{
		name: "OR",
		description: "Checks whether any of the arguments are TRUE, and returns TRUE or FALSE. Returns FALSE only if all arguments are FALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "are 1 to 255 conditions that you want to test that can be either TRUE or FALSE"
			},
			{
				name: "logical2",
				description: "are 1 to 255 conditions that you want to test that can be either TRUE or FALSE"
			}
		]
	},
	{
		name: "PARAMETER",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "PDURATION",
		description: "Returns the number of periods required by an investment to reach a specified value.",
		arguments: [
			{
				name: "rate",
				description: "is the interest rate per period."
			},
			{
				name: "pv",
				description: "is the present value of the investment"
			},
			{
				name: "fv",
				description: "is the desired future value of the investment"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Returns the Pearson product moment correlation coefficient, r.",
		arguments: [
			{
				name: "array1",
				description: "is a set of independent values"
			},
			{
				name: "array2",
				description: "is a set of dependent values"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Returns the k-th percentile of values in a range.",
		arguments: [
			{
				name: "array",
				description: "is the array or range of data that defines relative standing"
			},
			{
				name: "k",
				description: "is the percentile value that is between 0 through 1, inclusive"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "Returns the k-th percentile of values in a range, where k is in the range 0..1, exclusive.",
		arguments: [
			{
				name: "array",
				description: "is the array or range of data that defines relative standing"
			},
			{
				name: "k",
				description: "is the percentile value that is between 0 through 1, inclusive"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "Returns the k-th percentile of values in a range, where k is in the range 0..1, inclusive.",
		arguments: [
			{
				name: "array",
				description: "is the array or range of data that defines relative standing"
			},
			{
				name: "k",
				description: "is the percentile value that is between 0 through 1, inclusive"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Returns the rank of a value in a data set as a percentage of the data set.",
		arguments: [
			{
				name: "array",
				description: "is the array or range of data with numeric values that defines relative standing"
			},
			{
				name: "x",
				description: "is the value for which you want to know the rank"
			},
			{
				name: "significance",
				description: "is an optional value that identifies the number of significant digits for the returned percentage, three digits if omitted (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Returns the rank of a value in a data set as a percentage of the data set as a percentage (0..1, exclusive) of the data set.",
		arguments: [
			{
				name: "array",
				description: "is the array or range of data with numeric values that defines relative standing"
			},
			{
				name: "x",
				description: "is the value for which you want to know the rank"
			},
			{
				name: "significance",
				description: "is an optional value that identifies the number of significant digits for the returned percentage, three digits if omitted (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Returns the rank of a value in a data set as a percentage of the data set as a percentage (0..1, inclusive) of the data set.",
		arguments: [
			{
				name: "array",
				description: "is the array or range of data with numeric values that defines relative standing"
			},
			{
				name: "x",
				description: "is the value for which you want to know the rank"
			},
			{
				name: "significance",
				description: "is an optional value that identifies the number of significant digits for the returned percentage, three digits if omitted (0.xxx%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Returns the number of permutations for a given number of objects that can be selected from the total objects.",
		arguments: [
			{
				name: "number",
				description: "is the total number of objects"
			},
			{
				name: "number_chosen",
				description: "is the number of objects in each permutation"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Returns the number of permutations for a given number of objects (with repetitions) that can be selected from the total objects.",
		arguments: [
			{
				name: "number",
				description: "is the total number of objects"
			},
			{
				name: "number_chosen",
				description: "is the number of objects in each permutation"
			}
		]
	},
	{
		name: "PHI",
		description: "Returns the value of the density function for a standard normal distribution.",
		arguments: [
			{
				name: "x",
				description: "is the number for which you want the density of the standard normal distribution"
			}
		]
	},
	{
		name: "PI",
		description: "Returns the value of Pi, 3.14159265358979, accurate to 15 digits.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "Calculates the payment for a loan based on constant payments and a constant interest rate.",
		arguments: [
			{
				name: "rate",
				description: "is the interest rate per period for the loan. For example, use 6%/4 for quarterly payments at 6% APR"
			},
			{
				name: "nper",
				description: "is the total number of payments for the loan"
			},
			{
				name: "pv",
				description: "is the present value: the total amount that a series of future payments is worth now"
			},
			{
				name: "fv",
				description: "is the future value, or a cash balance you want to attain after the last payment is made, 0 (zero) if omitted"
			},
			{
				name: "type",
				description: "is a logical value: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted"
			}
		]
	},
	{
		name: "POISSON",
		description: "Returns the Poisson distribution.",
		arguments: [
			{
				name: "x",
				description: "is the number of events"
			},
			{
				name: "mean",
				description: "is the expected numeric value, a positive number"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative Poisson probability, use TRUE; for the Poisson probability mass function, use FALSE"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Returns the Poisson distribution.",
		arguments: [
			{
				name: "x",
				description: "is the number of events"
			},
			{
				name: "mean",
				description: "is the expected numeric value, a positive number"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative Poisson probability, use TRUE; for the Poisson probability mass function, use FALSE"
			}
		]
	},
	{
		name: "POWER",
		description: "Returns the result of a number raised to a power.",
		arguments: [
			{
				name: "number",
				description: "is the base number, any real number"
			},
			{
				name: "power",
				description: "is the exponent, to which the base number is raised"
			}
		]
	},
	{
		name: "PPMT",
		description: "Returns the payment on the principal for a given investment based on periodic, constant payments and a constant interest rate.",
		arguments: [
			{
				name: "rate",
				description: "is the interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR"
			},
			{
				name: "per",
				description: "specifies the period and must be in the range 1 to nper"
			},
			{
				name: "nper",
				description: "is the total number of payment periods in an investment"
			},
			{
				name: "pv",
				description: "is the present value: the total amount that a series of future payments is worth now"
			},
			{
				name: "fv",
				description: "is the future value, or cash balance you want to attain after the last payment is made"
			},
			{
				name: "type",
				description: "is a logical value: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "Returns the price per $100 face value of a discounted security.",
		arguments: [
			{
				name: "settlement",
				description: "is the security's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the security's maturity date, expressed as a serial date number"
			},
			{
				name: "discount",
				description: "is the security's discount rate"
			},
			{
				name: "redemption",
				description: "is the security's redemption value per $100 face value"
			},
			{
				name: "basis",
				description: "is the type of day count basis to use"
			}
		]
	},
	{
		name: "PROB",
		description: "Returns the probability that values in a range are between two limits or equal to a lower limit.",
		arguments: [
			{
				name: "x_range",
				description: "is the range of numeric values of x with which there are associated probabilities"
			},
			{
				name: "prob_range",
				description: "is the set of probabilities associated with values in X_range, values between 0 and 1 and excluding 0"
			},
			{
				name: "lower_limit",
				description: "is the lower bound on the value for which you want a probability"
			},
			{
				name: "upper_limit",
				description: "is the optional upper bound on the value. If omitted, PROB returns the probability that X_range values are equal to Lower_limit"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Multiplies all the numbers given as arguments.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers, logical values, or text representations of numbers that you want to multiply"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers, logical values, or text representations of numbers that you want to multiply"
			}
		]
	},
	{
		name: "PROPER",
		description: "Converts a text string to proper case; the first letter in each word in uppercase, and all other letters to lowercase.",
		arguments: [
			{
				name: "text",
				description: "is text enclosed in quotation marks, a formula that returns text, or a reference to a cell containing text to partially capitalize"
			}
		]
	},
	{
		name: "PV",
		description: "Returns the present value of an investment: the total amount that a series of future payments is worth now.",
		arguments: [
			{
				name: "rate",
				description: "is the interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR"
			},
			{
				name: "nper",
				description: "is the total number of payment periods in an investment"
			},
			{
				name: "pmt",
				description: "is the payment made each period and cannot change over the life of the investment"
			},
			{
				name: "fv",
				description: "is the future value, or a cash balance you want to attain after the last payment is made"
			},
			{
				name: "type",
				description: "is a logical value: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Returns the quartile of a data set.",
		arguments: [
			{
				name: "array",
				description: "is the array or cell range of numeric values for which you want the quartile value"
			},
			{
				name: "quart",
				description: "is a number: minimum value = 0; 1st quartile = 1; median value = 2; 3rd quartile = 3; maximum value = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "Returns the quartile of a data set, based on percentile values from 0..1, exclusive.",
		arguments: [
			{
				name: "array",
				description: "is the array or cell range of numeric values for which you want the quartile value"
			},
			{
				name: "quart",
				description: "is a number: minimum value = 0; 1st quartile = 1; median value = 2; 3rd quartile = 3; maximum value = 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "Returns the quartile of a data set, based on percentile values from 0..1, inclusive.",
		arguments: [
			{
				name: "array",
				description: "is the array or cell range of numeric values for which you want the quartile value"
			},
			{
				name: "quart",
				description: "is a number: minimum value = 0; 1st quartile = 1; median value = 2; 3rd quartile = 3; maximum value = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Returns the integer portion of a division.",
		arguments: [
			{
				name: "numerator",
				description: "is the dividend"
			},
			{
				name: "denominator",
				description: "is the divisor"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Converts degrees to radians.",
		arguments: [
			{
				name: "angle",
				description: "is an angle in degrees that you want to convert"
			}
		]
	},
	{
		name: "RAND",
		description: "Returns a random number greater than or equal to 0 and less than 1, evenly distributed (changes on recalculation).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Returns a random number between the numbers you specify.",
		arguments: [
			{
				name: "bottom",
				description: "is the smallest integer RANDBETWEEN will return"
			},
			{
				name: "top",
				description: "is the largest integer RANDBETWEEN will return"
			}
		]
	},
	{
		name: "RANGE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "RANK",
		description: "Returns the rank of a number in a list of numbers: its size relative to other values in the list.",
		arguments: [
			{
				name: "number",
				description: "is the number for which you want to find the rank"
			},
			{
				name: "ref",
				description: "is an array of, or a reference to, a list of numbers. Nonnumeric values are ignored"
			},
			{
				name: "order",
				description: "is a number: rank in the list sorted descending = 0 or omitted; rank in the list sorted ascending = any nonzero value"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Returns the rank of a number in a list of numbers: its size relative to other values in the list; if more than one value has the same rank, the average rank is returned.",
		arguments: [
			{
				name: "number",
				description: "is the number for which you want to find the rank"
			},
			{
				name: "ref",
				description: "is an array of, or a reference to, a list of numbers. Nonnumeric values are ignored"
			},
			{
				name: "order",
				description: "is a number: rank in the list sorted descending = 0 or omitted; rank in the list sorted ascending = any nonzero value"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Returns the rank of a number in a list of numbers: its size relative to other values in the list; if more than one value has the same rank, the top rank of that set of values is returned.",
		arguments: [
			{
				name: "number",
				description: "is the number for which you want to find the rank"
			},
			{
				name: "ref",
				description: "is an array of, or a reference to, a list of numbers. Nonnumeric values are ignored"
			},
			{
				name: "order",
				description: "is a number: rank in the list sorted descending = 0 or omitted; rank in the list sorted ascending = any nonzero value"
			}
		]
	},
	{
		name: "RATE",
		description: "Returns the interest rate per period of a loan or an investment. For example, use 6%/4 for quarterly payments at 6% APR.",
		arguments: [
			{
				name: "nper",
				description: "is the total number of payment periods for the loan or investment"
			},
			{
				name: "pmt",
				description: "is the payment made each period and cannot change over the life of the loan or investment"
			},
			{
				name: "pv",
				description: "is the present value: the total amount that a series of future payments is worth now"
			},
			{
				name: "fv",
				description: "is the future value, or a cash balance you want to attain after the last payment is made. If omitted, uses Fv = 0"
			},
			{
				name: "type",
				description: "is a logical value: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted"
			},
			{
				name: "guess",
				description: "is your guess for what the rate will be; if omitted, Guess = 0.1 (10 percent)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Returns the amount received at maturity for a fully invested security.",
		arguments: [
			{
				name: "settlement",
				description: "is the security's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the security's maturity date, expressed as a serial date number"
			},
			{
				name: "investment",
				description: "is the amount invested in the security"
			},
			{
				name: "discount",
				description: "is the security's discount rate"
			},
			{
				name: "basis",
				description: "is the type of day count basis to use"
			}
		]
	},
	{
		name: "REPLACE",
		description: "Replaces part of a text string with a different text string.",
		arguments: [
			{
				name: "old_text",
				description: "is text in which you want to replace some characters"
			},
			{
				name: "start_num",
				description: "is the position of the character in Old_text that you want to replace with New_text"
			},
			{
				name: "num_chars",
				description: "is the number of characters in Old_text that you want to replace"
			},
			{
				name: "new_text",
				description: "is the text that will replace characters in Old_text"
			}
		]
	},
	{
		name: "REPT",
		description: "Repeats text a given number of times. Use REPT to fill a cell with a number of instances of a text string.",
		arguments: [
			{
				name: "text",
				description: "is the text you want to repeat"
			},
			{
				name: "number_times",
				description: "is a positive number specifying the number of times to repeat text"
			}
		]
	},
	{
		name: "RIGHT",
		description: "Returns the specified number of characters from the end of a text string.",
		arguments: [
			{
				name: "text",
				description: "is the text string that contains the characters you want to extract"
			},
			{
				name: "num_chars",
				description: "specifies how many characters you want to extract, 1 if omitted"
			}
		]
	},
	{
		name: "ROMAN",
		description: "Converts an Arabic numeral to Roman, as text.",
		arguments: [
			{
				name: "number",
				description: "is the Arabic numeral you want to convert"
			},
			{
				name: "form",
				description: "is the number specifying the type of Roman numeral you want."
			}
		]
	},
	{
		name: "ROUND",
		description: "Rounds a number to a specified number of digits.",
		arguments: [
			{
				name: "number",
				description: "is the number you want to round"
			},
			{
				name: "num_digits",
				description: "is the number of digits to which you want to round. Negative rounds to the left of the decimal point; zero to the nearest integer"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Rounds a number down, toward zero.",
		arguments: [
			{
				name: "number",
				description: "is any real number that you want rounded down"
			},
			{
				name: "num_digits",
				description: "is the number of digits to which you want to round. Negative rounds to the left of the decimal point; zero or omitted, to the nearest integer"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Rounds a number up, away from zero.",
		arguments: [
			{
				name: "number",
				description: "is any real number that you want rounded up"
			},
			{
				name: "num_digits",
				description: "is the number of digits to which you want to round. Negative rounds to the left of the decimal point; zero or omitted, to the nearest integer"
			}
		]
	},
	{
		name: "ROW",
		description: "Returns the row number of a reference.",
		arguments: [
			{
				name: "reference",
				description: "is the cell or a single range of cells for which you want the row number; if omitted, returns the cell containing the ROW function"
			}
		]
	},
	{
		name: "ROWS",
		description: "Returns the number of rows in a reference or array.",
		arguments: [
			{
				name: "array",
				description: "is an array, an array formula, or a reference to a range of cells for which you want the number of rows"
			}
		]
	},
	{
		name: "RRI",
		description: "Returns an equivalent interest rate for the growth of an investment.",
		arguments: [
			{
				name: "nper",
				description: "is the number of periods for the investment"
			},
			{
				name: "pv",
				description: "is the present value of the investment"
			},
			{
				name: "fv",
				description: "is the future value of the investment"
			}
		]
	},
	{
		name: "RSQ",
		description: "Returns the square of the Pearson product moment correlation coefficient through the given data points.",
		arguments: [
			{
				name: "known_y's",
				description: "is an array or range of data points and can be numbers or names, arrays, or references that contain numbers"
			},
			{
				name: "known_x's",
				description: "is an array or range of data points and can be numbers or names, arrays, or references that contain numbers"
			}
		]
	},
	{
		name: "RTD",
		description: "Retrieves real-time data from a program that supports COM automation.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "is the name of the ProgID of a registered COM automation add-in. Enclose the name in quotation marks"
			},
			{
				name: "server",
				description: "is the name of the server where the add-in should be run. Enclose the name in quotation marks. If the add-in is run locally, use an empty string"
			},
			{
				name: "topic1",
				description: "are 1 to 38 parameters that specify a piece of data"
			},
			{
				name: "topic2",
				description: "are 1 to 38 parameters that specify a piece of data"
			}
		]
	},
	{
		name: "SEARCH",
		description: "Returns the number of the character at which a specific character or text string is first found, reading left to right (not case-sensitive).",
		arguments: [
			{
				name: "find_text",
				description: "is the text you want to find. You can use the ? and * wildcard characters; use ~? and ~* to find the ? and * characters"
			},
			{
				name: "within_text",
				description: "is the text in which you want to search for Find_text"
			},
			{
				name: "start_num",
				description: "is the character number in Within_text, counting from the left, at which you want to start searching. If omitted, 1 is used"
			}
		]
	},
	{
		name: "SEC",
		description: "Returns the secant of an angle.",
		arguments: [
			{
				name: "number",
				description: "is the angle in radians for which you want the secant"
			}
		]
	},
	{
		name: "SECH",
		description: "Returns the hyperbolic secant of an angle.",
		arguments: [
			{
				name: "number",
				description: "is the angle in radians for which you want the hyperbolic secant"
			}
		]
	},
	{
		name: "SECOND",
		description: "Returns the second, a number from 0 to 59.",
		arguments: [
			{
				name: "serial_number",
				description: "is a number in the date-time code used by Spreadsheet or text in time format, such as 16:48:23 or 4:48:47 PM"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Returns the sum of a power series based on the formula.",
		arguments: [
			{
				name: "x",
				description: "is the input value to the power series"
			},
			{
				name: "n",
				description: "is the initial power to which you want to raise x"
			},
			{
				name: "m",
				description: "is the step by which to increase n for each term in the series"
			},
			{
				name: "coefficients",
				description: "is a set of coefficients by which each successive power of x is multiplied"
			}
		]
	},
	{
		name: "SHEET",
		description: "Returns the sheet number of the referenced sheet.",
		arguments: [
			{
				name: "value",
				description: "is the name of a sheet or a reference that you want the sheet number of.  If omitted the number of the sheet containing the function is returned"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Returns the number of sheets in a reference.",
		arguments: [
			{
				name: "reference",
				description: "is a reference for which you want to know the number of sheets it contains.  If omitted the number of sheets in the workbook containing the function is returned"
			}
		]
	},
	{
		name: "SIGN",
		description: "Returns the sign of a number: 1 if the number is positive, zero if the number is zero, or -1 if the number is negative.",
		arguments: [
			{
				name: "number",
				description: "is any real number"
			}
		]
	},
	{
		name: "SIN",
		description: "Returns the sine of an angle.",
		arguments: [
			{
				name: "number",
				description: "is the angle in radians for which you want the sine. Degrees * PI()/180 = radians"
			}
		]
	},
	{
		name: "SINH",
		description: "Returns the hyperbolic sine of a number.",
		arguments: [
			{
				name: "number",
				description: "is any real number"
			}
		]
	},
	{
		name: "SKEW",
		description: "Returns the skewness of a distribution: a characterization of the degree of asymmetry of a distribution around its mean.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the skewness"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the skewness"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Returns the skewness of a distribution based on a population: a characterization of the degree of asymmetry of a distribution around its mean.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 254 numbers or names, arrays, or references that contain numbers for which you want the population skewness"
			},
			{
				name: "number2",
				description: "are 1 to 254 numbers or names, arrays, or references that contain numbers for which you want the population skewness"
			}
		]
	},
	{
		name: "SLN",
		description: "Returns the straight-line depreciation of an asset for one period.",
		arguments: [
			{
				name: "cost",
				description: "is the initial cost of the asset"
			},
			{
				name: "salvage",
				description: "is the salvage value at the end of the life of the asset"
			},
			{
				name: "life",
				description: "is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "Returns the slope of the linear regression line through the given data points.",
		arguments: [
			{
				name: "known_y's",
				description: "is an array or cell range of numeric dependent data points and can be numbers or names, arrays, or references that contain numbers"
			},
			{
				name: "known_x's",
				description: "is the set of independent data points and can be numbers or names, arrays, or references that contain numbers"
			}
		]
	},
	{
		name: "SMALL",
		description: "Returns the k-th smallest value in a data set. For example, the fifth smallest number.",
		arguments: [
			{
				name: "array",
				description: "is an array or range of numerical data for which you want to determine the k-th smallest value"
			},
			{
				name: "k",
				description: "is the position (from the smallest) in the array or range of the value to return"
			}
		]
	},
	{
		name: "SQRT",
		description: "Returns the square root of a number.",
		arguments: [
			{
				name: "number",
				description: "is the number for which you want the square root"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Returns the square root of (number * Pi).",
		arguments: [
			{
				name: "number",
				description: "is the number by which p is multiplied"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Returns a normalized value from a distribution characterized by a mean and standard deviation.",
		arguments: [
			{
				name: "x",
				description: "is the value you want to normalize"
			},
			{
				name: "mean",
				description: "is the arithmetic mean of the distribution"
			},
			{
				name: "standard_dev",
				description: "is the standard deviation of the distribution, a positive number"
			}
		]
	},
	{
		name: "STDEV",
		description: "Estimates standard deviation based on a sample (ignores logical values and text in the sample).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers corresponding to a sample of a population and can be numbers or references that contain numbers"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers corresponding to a sample of a population and can be numbers or references that contain numbers"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Calculates standard deviation based on the entire population given as arguments (ignores logical values and text).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers corresponding to a population and can be numbers or references that contain numbers"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers corresponding to a population and can be numbers or references that contain numbers"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Estimates standard deviation based on a sample (ignores logical values and text in the sample).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers corresponding to a sample of a population and can be numbers or references that contain numbers"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers corresponding to a sample of a population and can be numbers or references that contain numbers"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Estimates standard deviation based on a sample, including logical values and text. Text and the logical value FALSE have the value 0; the logical value TRUE has the value 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "are 1 to 255 values corresponding to a sample of a population and can be values or names or references to values"
			},
			{
				name: "value2",
				description: "are 1 to 255 values corresponding to a sample of a population and can be values or names or references to values"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Calculates standard deviation based on the entire population given as arguments (ignores logical values and text).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers corresponding to a population and can be numbers or references that contain numbers"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers corresponding to a population and can be numbers or references that contain numbers"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Calculates standard deviation based on an entire population, including logical values and text. Text and the logical value FALSE have the value 0; the logical value TRUE has the value 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "are 1 to 255 values corresponding to a population and can be values, names, arrays, or references that contain values"
			},
			{
				name: "value2",
				description: "are 1 to 255 values corresponding to a population and can be values, names, arrays, or references that contain values"
			}
		]
	},
	{
		name: "STEYX",
		description: "Returns the standard error of the predicted y-value for each x in a regression.",
		arguments: [
			{
				name: "known_y's",
				description: "is an array or range of dependent data points and can be numbers or names, arrays, or references that contain numbers"
			},
			{
				name: "known_x's",
				description: "is an array or range of independent data points and can be numbers or names, arrays, or references that contain numbers"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "Replaces existing text with new text in a text string.",
		arguments: [
			{
				name: "text",
				description: "is the text or the reference to a cell containing text in which you want to substitute characters"
			},
			{
				name: "old_text",
				description: "is the existing text you want to replace. If the case of Old_text does not match the case of text, SUBSTITUTE will not replace the text"
			},
			{
				name: "new_text",
				description: "is the text you want to replace Old_text with"
			},
			{
				name: "instance_num",
				description: "specifies which occurrence of Old_text you want to replace. If omitted, every instance of Old_text is replaced"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Returns a subtotal in a list or database.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "function_num",
				description: "is the number 1 to 11 that specifies the summary function for the subtotal."
			},
			{
				name: "ref1",
				description: "are 1 to 254 ranges or references for which you want the subtotal"
			}
		]
	},
	{
		name: "SUM",
		description: "Adds all the numbers in a range of cells.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers to sum. Logical values and text are ignored in cells, included if typed as arguments"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers to sum. Logical values and text are ignored in cells, included if typed as arguments"
			}
		]
	},
	{
		name: "SUMIF",
		description: "Adds the cells specified by a given condition or criteria.",
		arguments: [
			{
				name: "range",
				description: "is the range of cells you want evaluated"
			},
			{
				name: "criteria",
				description: "is the condition or criteria in the form of a number, expression, or text that defines which cells will be added"
			},
			{
				name: "sum_range",
				description: "are the actual cells to sum. If omitted, the cells in range are used"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Adds the cells specified by a given set of conditions or criteria.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sum_range",
				description: "are the actual cells to sum."
			},
			{
				name: "criteria_range",
				description: "is the range of cells you want evaluated for the particular condition"
			},
			{
				name: "criteria",
				description: "is the condition or criteria in the form of a number, expression, or text that defines which cells will be added"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "Returns the sum of the products of corresponding ranges or arrays.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "array1",
				description: "are 2 to 255 arrays for which you want to multiply and then add components. All arrays must have the same dimensions"
			},
			{
				name: "array2",
				description: "are 2 to 255 arrays for which you want to multiply and then add components. All arrays must have the same dimensions"
			},
			{
				name: "array3",
				description: "are 2 to 255 arrays for which you want to multiply and then add components. All arrays must have the same dimensions"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "Returns the sum of the squares of the arguments. The arguments can be numbers, arrays, names, or references to cells that contain numbers.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numbers, arrays, names, or references to arrays for which you want the sum of the squares"
			},
			{
				name: "number2",
				description: "are 1 to 255 numbers, arrays, names, or references to arrays for which you want the sum of the squares"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Sums the differences between the squares of two corresponding ranges or arrays.",
		arguments: [
			{
				name: "array_x",
				description: "is the first range or array of numbers and can be a number or name, array, or reference that contains numbers"
			},
			{
				name: "array_y",
				description: "is the second range or array of numbers and can be a number or name, array, or reference that contains numbers"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Returns the sum total of the sums of squares of numbers in two corresponding ranges or arrays.",
		arguments: [
			{
				name: "array_x",
				description: "is the first range or array of numbers and can be a number or name, array, or reference that contains numbers"
			},
			{
				name: "array_y",
				description: "is the second range or array of numbers and can be a number or name, array, or reference that contains numbers"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Sums the squares of the differences in two corresponding ranges or arrays.",
		arguments: [
			{
				name: "array_x",
				description: "is the first range or array of values and can be a number or name, array, or reference that contains numbers"
			},
			{
				name: "array_y",
				description: "is the second range or array of values and can be a number or name, array, or reference that contains numbers"
			}
		]
	},
	{
		name: "SYD",
		description: "Returns the sum-of-years' digits depreciation of an asset for a specified period.",
		arguments: [
			{
				name: "cost",
				description: "is the initial cost of the asset"
			},
			{
				name: "salvage",
				description: "is the salvage value at the end of the life of the asset"
			},
			{
				name: "life",
				description: "is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset)"
			},
			{
				name: "per",
				description: "is the period and must use the same units as Life"
			}
		]
	},
	{
		name: "T",
		description: "Checks whether a value is text, and returns the text if it is, or returns double quotes (empty text) if it is not.",
		arguments: [
			{
				name: "value",
				description: "is the value to test"
			}
		]
	},
	{
		name: "T.DIST",
		description: "Returns the left-tailed Student's t-distribution.",
		arguments: [
			{
				name: "x",
				description: "is the numeric value at which to evaluate the distribution"
			},
			{
				name: "deg_freedom",
				description: "is an integer indicating the number of degrees of freedom that characterize the distribution"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Returns the two-tailed Student's t-distribution.",
		arguments: [
			{
				name: "x",
				description: "is the numeric value at which to evaluate the distribution"
			},
			{
				name: "deg_freedom",
				description: "is an integer indicating the number of degrees of freedom that characterize the distribution"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Returns the right-tailed Student's t-distribution.",
		arguments: [
			{
				name: "x",
				description: "is the numeric value at which to evaluate the distribution"
			},
			{
				name: "deg_freedom",
				description: "is an integer indicating the number of degrees of freedom that characterize the distribution"
			}
		]
	},
	{
		name: "T.INV",
		description: "Returns the left-tailed inverse of the Student's t-distribution.",
		arguments: [
			{
				name: "probability",
				description: "is the probability associated with the two-tailed Student's t-distribution, a number between 0 and 1 inclusive"
			},
			{
				name: "deg_freedom",
				description: "is a positive integer indicating the number of degrees of freedom to characterize the distribution"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Returns the two-tailed inverse of the Student's t-distribution.",
		arguments: [
			{
				name: "probability",
				description: "is the probability associated with the two-tailed Student's t-distribution, a number between 0 and 1 inclusive"
			},
			{
				name: "deg_freedom",
				description: "is a positive integer indicating the number of degrees of freedom to characterize the distribution"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Returns the probability associated with a Student's t-Test.",
		arguments: [
			{
				name: "array1",
				description: "is the first data set"
			},
			{
				name: "array2",
				description: "is the second data set"
			},
			{
				name: "tails",
				description: "specifies the number of distribution tails to return: one-tailed distribution = 1; two-tailed distribution = 2"
			},
			{
				name: "type",
				description: "is the kind of t-test: paired = 1, two-sample equal variance (homoscedastic) = 2, two-sample unequal variance = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Returns the tangent of an angle.",
		arguments: [
			{
				name: "number",
				description: "is the angle in radians for which you want the tangent. Degrees * PI()/180 = radians"
			}
		]
	},
	{
		name: "TANH",
		description: "Returns the hyperbolic tangent of a number.",
		arguments: [
			{
				name: "number",
				description: "is any real number"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Returns the bond-equivalent yield for a treasury bill.",
		arguments: [
			{
				name: "settlement",
				description: "is the Treasury bill's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the Treasury bill's maturity date, expressed as a serial date number"
			},
			{
				name: "discount",
				description: "is the Treasury bill's discount rate"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Returns the price per $100 face value for a treasury bill.",
		arguments: [
			{
				name: "settlement",
				description: "is the Treasury bill's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the Treasury bill's maturity date, expressed as a serial date number"
			},
			{
				name: "discount",
				description: "is the Treasury bill's discount rate"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Returns the yield for a treasury bill.",
		arguments: [
			{
				name: "settlement",
				description: "is the Treasury bill's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the Treasury bill's maturity date, expressed as a serial date number"
			},
			{
				name: "pr",
				description: "is the Treasury Bill's price per $100 face value"
			}
		]
	},
	{
		name: "TDIST",
		description: "Returns the Student's t-distribution.",
		arguments: [
			{
				name: "x",
				description: "is the numeric value at which to evaluate the distribution"
			},
			{
				name: "deg_freedom",
				description: "is an integer indicating the number of degrees of freedom that characterize the distribution"
			},
			{
				name: "tails",
				description: "specifies the number of distribution tails to return: one-tailed distribution = 1; two-tailed distribution = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "Converts a value to text in a specific number format.",
		arguments: [
			{
				name: "value",
				description: "is a number, a formula that evaluates to a numeric value, or a reference to a cell containing a numeric value"
			},
			{
				name: "format_text",
				description: "is a number format in text form from the Category box on the Number tab in the Format Cells dialog box (not General)"
			}
		]
	},
	{
		name: "TIME",
		description: "Converts hours, minutes, and seconds given as numbers to an Spreadsheet serial number, formatted with a time format.",
		arguments: [
			{
				name: "hour",
				description: "is a number from 0 to 23 representing the hour"
			},
			{
				name: "minute",
				description: "is a number from 0 to 59 representing the minute"
			},
			{
				name: "second",
				description: "is a number from 0 to 59 representing the second"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "Converts a text time to an Spreadsheet serial number for a time, a number from 0 (12:00:00 AM) to 0.999988426 (11:59:59 PM). Format the number with a time format after entering the formula.",
		arguments: [
			{
				name: "time_text",
				description: "is a text string that gives a time in any one of the Spreadsheet time formats (date information in the string is ignored)"
			}
		]
	},
	{
		name: "TINV",
		description: "Returns the two-tailed inverse of the Student's t-distribution.",
		arguments: [
			{
				name: "probability",
				description: "is the probability associated with the two-tailed Student's t-distribution, a number between 0 and 1 inclusive"
			},
			{
				name: "deg_freedom",
				description: "is a positive integer indicating the number of degrees of freedom to characterize the distribution"
			}
		]
	},
	{
		name: "TODAY",
		description: "Returns the current date formatted as a date.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "Converts a vertical range of cells to a horizontal range, or vice versa.",
		arguments: [
			{
				name: "array",
				description: "is a range of cells on a worksheet or an array of values that you want to transpose"
			}
		]
	},
	{
		name: "TREND",
		description: "Returns numbers in a linear trend matching known data points, using the least squares method.",
		arguments: [
			{
				name: "known_y's",
				description: "is a range or array of y-values you already know in the relationship y = mx + b"
			},
			{
				name: "known_x's",
				description: "is an optional range or array of x-values that you know in the relationship y = mx + b, an array the same size as Known_y's"
			},
			{
				name: "new_x's",
				description: "is a range or array of new x-values for which you want TREND to return corresponding y-values"
			},
			{
				name: "const",
				description: "is a logical value: the constant b is calculated normally if Const = TRUE or omitted; b is set equal to 0 if Const = FALSE"
			}
		]
	},
	{
		name: "TRIM",
		description: "Removes all spaces from a text string except for single spaces between words.",
		arguments: [
			{
				name: "text",
				description: "is the text from which you want spaces removed"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Returns the mean of the interior portion of a set of data values.",
		arguments: [
			{
				name: "array",
				description: "is the range or array of values to trim and average"
			},
			{
				name: "percent",
				description: "is the fractional number of data points to exclude from the top and bottom of the data set"
			}
		]
	},
	{
		name: "TRUE",
		description: "Returns the logical value TRUE.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "Truncates a number to an integer by removing the decimal, or fractional, part of the number.",
		arguments: [
			{
				name: "number",
				description: "is the number you want to truncate"
			},
			{
				name: "num_digits",
				description: "is a number specifying the precision of the truncation, 0 (zero) if omitted"
			}
		]
	},
	{
		name: "TTEST",
		description: "Returns the probability associated with a Student's t-Test.",
		arguments: [
			{
				name: "array1",
				description: "is the first data set"
			},
			{
				name: "array2",
				description: "is the second data set"
			},
			{
				name: "tails",
				description: "specifies the number of distribution tails to return: one-tailed distribution = 1; two-tailed distribution = 2"
			},
			{
				name: "type",
				description: "is the kind of t-test: paired = 1, two-sample equal variance (homoscedastic) = 2, two-sample unequal variance = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "Returns an integer representing the data type of a value: number = 1; text = 2; logical value = 4; error value = 16; array = 64.",
		arguments: [
			{
				name: "value",
				description: "can be any value"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Returns the number (code point) corresponding to the first character of the text.",
		arguments: [
			{
				name: "text",
				description: "is the character that you want the Unicode value of"
			}
		]
	},
	{
		name: "UPPER",
		description: "Converts a text string to all uppercase letters.",
		arguments: [
			{
				name: "text",
				description: "is the text you want converted to uppercase, a reference or a text string"
			}
		]
	},
	{
		name: "VALUE",
		description: "Converts a text string that represents a number to a number.",
		arguments: [
			{
				name: "text",
				description: "is the text enclosed in quotation marks or a reference to a cell containing the text you want to convert"
			}
		]
	},
	{
		name: "VAR",
		description: "Estimates variance based on a sample (ignores logical values and text in the sample).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numeric arguments corresponding to a sample of a population"
			},
			{
				name: "number2",
				description: "are 1 to 255 numeric arguments corresponding to a sample of a population"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Calculates variance based on the entire population (ignores logical values and text in the population).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numeric arguments corresponding to a population"
			},
			{
				name: "number2",
				description: "are 1 to 255 numeric arguments corresponding to a population"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Estimates variance based on a sample (ignores logical values and text in the sample).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numeric arguments corresponding to a sample of a population"
			},
			{
				name: "number2",
				description: "are 1 to 255 numeric arguments corresponding to a sample of a population"
			}
		]
	},
	{
		name: "VARA",
		description: "Estimates variance based on a sample, including logical values and text. Text and the logical value FALSE have the value 0; the logical value TRUE has the value 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "are 1 to 255 value arguments corresponding to a sample of a population"
			},
			{
				name: "value2",
				description: "are 1 to 255 value arguments corresponding to a sample of a population"
			}
		]
	},
	{
		name: "VARP",
		description: "Calculates variance based on the entire population (ignores logical values and text in the population).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "are 1 to 255 numeric arguments corresponding to a population"
			},
			{
				name: "number2",
				description: "are 1 to 255 numeric arguments corresponding to a population"
			}
		]
	},
	{
		name: "VARPA",
		description: "Calculates variance based on the entire population, including logical values and text. Text and the logical value FALSE have the value 0; the logical value TRUE has the value 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "are 1 to 255 value arguments corresponding to a population"
			},
			{
				name: "value2",
				description: "are 1 to 255 value arguments corresponding to a population"
			}
		]
	},
	{
		name: "VDB",
		description: "Returns the depreciation of an asset for any period you specify, including partial periods, using the double-declining balance method or some other method you specify.",
		arguments: [
			{
				name: "cost",
				description: "is the initial cost of the asset"
			},
			{
				name: "salvage",
				description: "is the salvage value at the end of the life of the asset"
			},
			{
				name: "life",
				description: "is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset)"
			},
			{
				name: "start_period",
				description: "is the starting period for which you want to calculate the depreciation, in the same units as Life"
			},
			{
				name: "end_period",
				description: "is the ending period for which you want to calculate the depreciation, in the same units as Life"
			},
			{
				name: "factor",
				description: "is the rate at which the balance declines, 2 (double-declining balance) if omitted"
			},
			{
				name: "no_switch",
				description: "switch to straight-line depreciation when depreciation is greater than the declining balance = FALSE or omitted; do not switch = TRUE"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "Looks for a value in the leftmost column of a table, and then returns a value in the same row from a column you specify. By default, the table must be sorted in an ascending order.",
		arguments: [
			{
				name: "lookup_value",
				description: "is the value to be found in the first column of the table, and can be a value, a reference, or a text string"
			},
			{
				name: "table_array",
				description: "is a table of text, numbers, or logical values, in which data is retrieved. Table_array can be a reference to a range or a range name"
			},
			{
				name: "col_index_num",
				description: "is the column number in table_array from which the matching value should be returned. The first column of values in the table is column 1"
			},
			{
				name: "range_lookup",
				description: "is a logical value: to find the closest match in the first column (sorted in ascending order) = TRUE or omitted; find an exact match = FALSE"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "Returns a number from 1 to 7 identifying the day of the week of a date.",
		arguments: [
			{
				name: "serial_number",
				description: "is a number that represents a date"
			},
			{
				name: "return_type",
				description: "is a number: for Sunday=1 through Saturday=7, use 1; for Monday=1 through Sunday=7, use 2; for Monday=0 through Sunday=6, use 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Returns the week number in the year.",
		arguments: [
			{
				name: "serial_number",
				description: "is the date-time code used by Spreadsheet for date and time calculation"
			},
			{
				name: "return_type",
				description: "is a number (1 or 2) that determines the type of the return value"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Returns the Weibull distribution.",
		arguments: [
			{
				name: "x",
				description: "is the value at which to evaluate the function, a nonnegative number"
			},
			{
				name: "alpha",
				description: "is a parameter to the distribution, a positive number"
			},
			{
				name: "beta",
				description: "is a parameter to the distribution, a positive number"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative distribution function, use TRUE; for the probability mass function, use FALSE"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Returns the Weibull distribution.",
		arguments: [
			{
				name: "x",
				description: "is the value at which to evaluate the function, a nonnegative number"
			},
			{
				name: "alpha",
				description: "is a parameter to the distribution, a positive number"
			},
			{
				name: "beta",
				description: "is a parameter to the distribution, a positive number"
			},
			{
				name: "cumulative",
				description: "is a logical value: for the cumulative distribution function, use TRUE; for the probability mass function, use FALSE"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Returns the serial number of the date before or after a specified number of workdays.",
		arguments: [
			{
				name: "start_date",
				description: "is a serial date number that represents the start date"
			},
			{
				name: "days",
				description: "is the number of nonweekend and non-holiday days before or after start_date"
			},
			{
				name: "holidays",
				description: "is an optional array of one or more serial date numbers to exclude from the working calendar, such as state and federal holidays and floating holidays"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Returns the serial number of the date before or after a specified number of workdays with custom weekend parameters.",
		arguments: [
			{
				name: "start_date",
				description: "is a serial date number that represents the start date"
			},
			{
				name: "days",
				description: "is the number of nonweekend and non-holiday days before or after start_date"
			},
			{
				name: "weekend",
				description: "is a number or string specifying when weekends occur"
			},
			{
				name: "holidays",
				description: "is an optional array of one or more serial date numbers to exclude from the working calendar, such as state and federal holidays and floating holidays"
			}
		]
	},
	{
		name: "XIRR",
		description: "Returns the internal rate of return for a schedule of cash flows.",
		arguments: [
			{
				name: "values",
				description: "is a series of cash flows that correspond to a schedule of payments in dates"
			},
			{
				name: "dates",
				description: "is a schedule of payment dates that corresponds to the cash flow payments"
			},
			{
				name: "guess",
				description: "is a number that you guess is close to the result of XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "Returns the net present value for a schedule of cash flows.",
		arguments: [
			{
				name: "rate",
				description: "is the discount rate to apply to the cash flows"
			},
			{
				name: "values",
				description: "is a series of cash flows that correspond to a schedule of payments in dates"
			},
			{
				name: "dates",
				description: "is a schedule of payment dates that corresponds to the cash flow payments"
			}
		]
	},
	{
		name: "XOR",
		description: "Returns a logical 'Exclusive Or' of all arguments.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "are 1 to 254 conditions you want to test that can be either TRUE or FALSE and can be logical values, arrays, or references"
			},
			{
				name: "logical2",
				description: "are 1 to 254 conditions you want to test that can be either TRUE or FALSE and can be logical values, arrays, or references"
			}
		]
	},
	{
		name: "YEAR",
		description: "Returns the year of a date, an integer in the range 1900 - 9999.",
		arguments: [
			{
				name: "serial_number",
				description: "is a number in the date-time code used by Spreadsheet"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Returns the year fraction representing the number of whole days between start_date and end_date.",
		arguments: [
			{
				name: "start_date",
				description: "is a serial date number that represents the start date"
			},
			{
				name: "end_date",
				description: "is a serial date number that represents the end date"
			},
			{
				name: "basis",
				description: "is the type of day count basis to use"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Returns the annual yield for a discounted security. For example, a treasury bill.",
		arguments: [
			{
				name: "settlement",
				description: "is the security's settlement date, expressed as a serial date number"
			},
			{
				name: "maturity",
				description: "is the security's maturity date, expressed as a serial date number"
			},
			{
				name: "pr",
				description: "is the security's price per $100 face value"
			},
			{
				name: "redemption",
				description: "is the security's redemption value per $100 face value"
			},
			{
				name: "basis",
				description: "is the type of day count basis to use"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Returns the one-tailed P-value of a z-test.",
		arguments: [
			{
				name: "array",
				description: "is the array or range of data against which to test X"
			},
			{
				name: "x",
				description: "is the value to test"
			},
			{
				name: "sigma",
				description: "is the population (known) standard deviation. If omitted, the sample standard deviation is used"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Returns the one-tailed P-value of a z-test.",
		arguments: [
			{
				name: "array",
				description: "is the array or range of data against which to test X"
			},
			{
				name: "x",
				description: "is the value to test"
			},
			{
				name: "sigma",
				description: "is the population (known) standard deviation. If omitted, the sample standard deviation is used"
			}
		]
	}
];