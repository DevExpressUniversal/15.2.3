ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "返回给定数值的绝对值，即不带符号的数值.",
		arguments: [
			{
				name: "number",
				description: "要对其求绝对值的实数"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "返回在到期日支付利息的债券的应计利息.",
		arguments: [
			{
				name: "issue",
				description: "是债券的发行日期，以一串日期表示"
			},
			{
				name: "settlement",
				description: "是债券的到期日，以一串日期表示"
			},
			{
				name: "rate",
				description: "是债券的年票息率"
			},
			{
				name: "par",
				description: "是债券的票面值"
			},
			{
				name: "basis",
				description: "是所采用的日算类型"
			}
		]
	},
	{
		name: "ACOS",
		description: "返回一个弧度的反余弦。弧度值在 0 到 Pi 之间。反余弦值是指余弦值为 Number 的角度.",
		arguments: [
			{
				name: "number",
				description: "余弦值，必须在 -1 和 1 之间"
			}
		]
	},
	{
		name: "ACOSH",
		description: "返回反双曲余弦值.",
		arguments: [
			{
				name: "number",
				description: "大于或等于 1 的任何实数"
			}
		]
	},
	{
		name: "ACOT",
		description: "返回一个数字的反余切值。弧度值在 0 到 Pi 之间.",
		arguments: [
			{
				name: "number",
				description: " 所需角度的的余切值"
			}
		]
	},
	{
		name: "ACOTH",
		description: "返回一个数字的反双曲余切值.",
		arguments: [
			{
				name: "number",
				description: " 所需角度的双曲余切值"
			}
		]
	},
	{
		name: "ASIN",
		description: "返回一个弧度的反正弦。弧度值在 -Pi/2 到 Pi/2 之间.",
		arguments: [
			{
				name: "number",
				description: "正弦值，必须在 -1 和 1 之间"
			}
		]
	},
	{
		name: "ASINH",
		description: "返回反双曲正弦值.",
		arguments: [
			{
				name: "number",
				description: "大于等于 1 的任何实数"
			}
		]
	},
	{
		name: "ATAN",
		description: "返回反正切值。以弧度表示，大小在 -Pi/2 到 Pi/2 之间.",
		arguments: [
			{
				name: "number",
				description: "角度的正切值"
			}
		]
	},
	{
		name: "ATAN2",
		description: "根据给定的 X 轴及 Y 轴坐标值，返回反正切值。返回值在 -Pi 到 Pi 之间(不包括 -Pi).",
		arguments: [
			{
				name: "x_num",
				description: "某点的 X 轴坐标值"
			},
			{
				name: "y_num",
				description: "某点的 Y 轴坐标值"
			}
		]
	},
	{
		name: "ATANH",
		description: "返回反双曲正切值.",
		arguments: [
			{
				name: "number",
				description: "介于 -1 和 1 之间(不包括 -1 和 1)的任何实数"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "返回一组数据点到其算术平均值的绝对偏差的平均值。参数可以是数字、名称、数组或包含数字的引用.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是要计算绝对偏差平均值的 1 到 255 个数值"
			},
			{
				name: "number2",
				description: "是要计算绝对偏差平均值的 1 到 255 个数值"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "返回所有参数的算术平均值。字符串和 FALSE 相当于 0；TRUE 相当于 1。参数可以是数值、名称、数组或引用.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "是要求平均值的 1 到 255 个参数"
			},
			{
				name: "value2",
				description: "是要求平均值的 1 到 255 个参数"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "查找给定条件指定的单元格的平均值(算术平均值).",
		arguments: [
			{
				name: "range",
				description: "是要进行计算的单元格区域"
			},
			{
				name: "criteria",
				description: "是数字、表达式或文本形式的条件，它定义了用于查找平均值的单元格范围"
			},
			{
				name: "average_range",
				description: "是用于查找平均值的实际单元格。如果省略，则使用区域中的单元格"
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "查找一组给定条件指定的单元格的平均值(算术平均值).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "average_range",
				description: "是用于查找平均值的实际单元格"
			},
			{
				name: "criteria_range",
				description: "是要为特定条件计算的单元格区域"
			},
			{
				name: "criteria",
				description: "是数字、表达式或文本形式的条件，它定义了用于查找平均值的单元格范围"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "将数字转换为泰语文本.",
		arguments: [
			{
				name: "number",
				description: "要转换的数字"
			}
		]
	},
	{
		name: "BASE",
		description: "将数字转换成具有给定基数的文本表示形式.",
		arguments: [
			{
				name: "number",
				description: " 是您要转换的数字"
			},
			{
				name: "radix",
				description: " 是您希望将数字转换成的基数"
			},
			{
				name: "min_length",
				description: " 是返回的字符串的最小长度。如果省略，则不添加前导零"
			}
		]
	},
	{
		name: "BESSELI",
		description: "返回修正的贝赛耳函数 In(x).",
		arguments: [
			{
				name: "x",
				description: "是函数计算点的值"
			},
			{
				name: "n",
				description: "是贝赛耳函数的次序"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "返回贝赛耳函数 Jn(x).",
		arguments: [
			{
				name: "x",
				description: "是函数计算点的值"
			},
			{
				name: "n",
				description: "是贝赛耳函数的次序"
			}
		]
	},
	{
		name: "BESSELK",
		description: "返回修正的贝赛耳函数 Kn(x).",
		arguments: [
			{
				name: "x",
				description: "是函数计算点的值"
			},
			{
				name: "n",
				description: "是函数的次序"
			}
		]
	},
	{
		name: "BESSELY",
		description: "返回贝赛耳函数 Yn(x).",
		arguments: [
			{
				name: "x",
				description: "是函数计算点的值"
			},
			{
				name: "n",
				description: "是函数的次序"
			}
		]
	},
	{
		name: "Beta 版。DIST",
		description: "返回 beta 概率分布函数.",
		arguments: [
			{
				name: "x",
				description: "用来进行概率分布计算的值，须居于可选性上下界(A 和 B)之间"
			},
			{
				name: "alpha",
				description: "此分布的一个参数，必须大于 0"
			},
			{
				name: "beta",
				description: "此分布的一个参数，必须大于 0"
			},
			{
				name: "cumulative",
				description: "逻辑值，决定函数的形式。累积分布函数，使用 TRUE；概率密度函数，使用 FALSE"
			},
			{
				name: "A",
				description: "数值 x 的可选下界。如果忽略，A=0"
			},
			{
				name: "B",
				description: "数值 x 的可选上界。如果忽略，B=1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "返回具有给定概率的累积 beta 分布的区间点.",
		arguments: [
			{
				name: "probability",
				description: "Beta 分布的概率"
			},
			{
				name: "alpha",
				description: "Beta 分布的参数，必须大于 0"
			},
			{
				name: "beta",
				description: "Beta 分布的参数，必须大于 0"
			},
			{
				name: "A",
				description: "数值 x 的可选下界。如果忽略，A=0"
			},
			{
				name: "B",
				description: "数值 x 的可选上界。如果忽略，B=1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "返回累积 beta 分布的概率密度函数.",
		arguments: [
			{
				name: "x",
				description: "是函数计算点 A 和 B 之间的值"
			},
			{
				name: "alpha",
				description: "是此分布的一个参数，必须大于 0"
			},
			{
				name: "beta",
				description: "是此分布的一个参数，必须大于 0"
			},
			{
				name: "A",
				description: "是间隔 x 的可选下界。如果忽略，A=0"
			},
			{
				name: "B",
				description: "是间隔 x 的可选上界。如果忽略，B=1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "返回累积 beta 分布的概率密度函数区间点 (BETADIST).",
		arguments: [
			{
				name: "probability",
				description: "是与 beta 分布相关的概率"
			},
			{
				name: "alpha",
				description: "是分布的参数，必须大于 0"
			},
			{
				name: "beta",
				description: "是分布的参数，必须大于 0"
			},
			{
				name: "A",
				description: "是间隔 x 的可选下界。如果忽略，A=0"
			},
			{
				name: "B",
				description: "是间隔 x 的可选上界，如果忽略，B=1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "将二进制数转换为十进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的二进制数"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "将二进制数转换为十六进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的二进制数"
			},
			{
				name: "places",
				description: "是要使用的字符数"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "将二进制数转换为八进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的二进制数"
			},
			{
				name: "places",
				description: "是要使用的字符数"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "返回一元二项式分布的概率.",
		arguments: [
			{
				name: "number_s",
				description: "实验成功次数"
			},
			{
				name: "trials",
				description: "独立实验次数"
			},
			{
				name: "probability_s",
				description: "一次实验中成功的概率"
			},
			{
				name: "cumulative",
				description: "逻辑值，决定函数的形式。累积分布函数，使用 TRUE；概率密度函数，使用 FALSE"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "返回一个数值，它是使得累积二项式分布的函数值大于或等于临界值 α 的最小整数.",
		arguments: [
			{
				name: "trials",
				description: "贝努利试验次数"
			},
			{
				name: "probability_s",
				description: "一次试验中成功的概率，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "alpha",
				description: "临界值，介于 0 与 1 之间，含 0 与 1"
			}
		]
	},
	{
		name: "BINOM。区范围",
		description: "使用二项式分布返回试验结果的概率.",
		arguments: [
			{
				name: "trials",
				description: " 是独立试验次数"
			},
			{
				name: "probability_s",
				description: " 是每次试验成功的概率"
			},
			{
				name: "number_s",
				description: " 是试验成功次数"
			},
			{
				name: "number_s2",
				description: " 如果提供此函数，则返回成功试验次数介于 number_s 和 number_s2 之间的概率"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "返回一元二项式分布的概率.",
		arguments: [
			{
				name: "number_s",
				description: "是实验成功次数"
			},
			{
				name: "trials",
				description: "是独立实验次数"
			},
			{
				name: "probability_s",
				description: "是一次实验中成功的概率"
			},
			{
				name: "cumulative",
				description: "是逻辑值，决定函数的形式。累积分布函数，使用 TRUE；概率密度函数，使用 FALSE"
			}
		]
	},
	{
		name: "BITAND",
		description: "返回两个数字的按位'与'值.",
		arguments: [
			{
				name: "number1",
				description: " 是您要计算的二进制数的十进制表示形式"
			},
			{
				name: "number2",
				description: " 是您要计算的二进制数的十进制表示形式"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "返回按 shift_amount 位左移的值数字.",
		arguments: [
			{
				name: "number",
				description: " 是您要计算的二进制数的十进制表示形式"
			},
			{
				name: "shift_amount",
				description: " 是您要将数字 1 左移的位数"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "返回按 shift_amount 位右移的值数字.",
		arguments: [
			{
				name: "number",
				description: " 是您要计算的二进制数的十进制表示形式"
			},
			{
				name: "shift_amount",
				description: " 是您要将数字 1 右移的位数"
			}
		]
	},
	{
		name: "BITXOR",
		description: "返回两个数字的按位“异或”值.",
		arguments: [
			{
				name: "number1",
				description: " 是您要计算的二进制数的十进制表示形式"
			},
			{
				name: "number2",
				description: " 是您要计算的二进制数的十进制表示形式"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "将参数向上舍入为最接近的整数，或最接近的指定基数的倍数.",
		arguments: [
			{
				name: "number",
				description: "需要进行舍入的参数"
			},
			{
				name: "significance",
				description: "用于向上舍入的基数"
			}
		]
	},
	{
		name: "CHAR",
		description: "根据本机中的字符集，返回由代码数字指定的字符.",
		arguments: [
			{
				name: "number",
				description: "介于 1 到 255 之间的任一数字，该数字对应着您要返回的字符"
			}
		]
	},
	{
		name: "chidist 将",
		description: "返回 χ2 分布的右尾概率.",
		arguments: [
			{
				name: "x",
				description: "是用来计算 χ2 分布右尾概率的数值，非负数"
			},
			{
				name: "deg_freedom",
				description: "是自由度的数值，介于 1 与 10^10 之间，不含 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "返回具有给定概率的右尾 χ2 分布的区间点.",
		arguments: [
			{
				name: "probability",
				description: "χ2 分布的概率，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "deg_freedom",
				description: "自由度，介于 1 与 10^10 之间，不含 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "返回 χ2 分布的左尾概率.",
		arguments: [
			{
				name: "x",
				description: "用来计算 χ2 分布左尾概率的数值，非负数值"
			},
			{
				name: "deg_freedom",
				description: "自由度，介于 1 与 10^10 之间，不含 10^10"
			},
			{
				name: "cumulative",
				description: "逻辑值，当函数为累积分布函数时，返回值为 TRUE；当为概率密度函数时，返回值为 FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "返回 χ2 分布的右尾概率.",
		arguments: [
			{
				name: "x",
				description: "用来计算 χ2 分布右尾概率的数值，非负数值"
			},
			{
				name: "deg_freedom",
				description: "自由度，介于 1 与 10^10 之间，不含 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "返回具有给定概率的左尾 χ2 分布的区间点.",
		arguments: [
			{
				name: "probability",
				description: "χ2 分布的概率，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "deg_freedom",
				description: "自由度，介于 1 与 10^10 之间，不含 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "返回具有给定概率的右尾 χ2 分布的区间点.",
		arguments: [
			{
				name: "probability",
				description: "χ2 分布的概率，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "deg_freedom",
				description: "自由度，介于 1 与 10^10 之间，不含 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "返回独立性检验的结果: 针对统计和相应的自由度返回卡方分布值.",
		arguments: [
			{
				name: "actual_range",
				description: "为包含观察值的数据范围，用于检验预期值"
			},
			{
				name: "expected_range",
				description: "为一数据范围，其内容为各行总和乘以各列总和后的值，再除以全部值总和的比率"
			}
		]
	},
	{
		name: "CHITEST",
		description: "返回独立性检验的结果: 针对统计和相应的自由度返回卡方分布值.",
		arguments: [
			{
				name: "actual_range",
				description: "为包含观察值的数据范围，用于检验预期值"
			},
			{
				name: "expected_range",
				description: "为一数据范围，其内容为各行总和乘以各列总和后的值，再除以全部值总和的比率"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "返回某一引用或数组的列数.",
		arguments: [
			{
				name: "array",
				description: "要计算列数的数组、数组公式或是对单元格区域的引用"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "使用学生 T 分布，返回总体平均值的置信区间.",
		arguments: [
			{
				name: "alpha",
				description: "用来计算置信区间的显著性水平，一个大于 0 小于 1 的数值"
			},
			{
				name: "standard_dev",
				description: "假设为已知的总体标准方差。Standard_dev 必须大于 0"
			},
			{
				name: "size",
				description: "样本容量"
			}
		]
	},
	{
		name: "CORREL",
		description: "返回两组数值的相关系数.",
		arguments: [
			{
				name: "array1",
				description: "第一组数值单元格区域。值应为数值、名称、数组或包含数值的引用"
			},
			{
				name: "array2",
				description: "第二组数值单元格区域。值应为数值、名称、数组或包含数值的引用"
			}
		]
	},
	{
		name: "COS",
		description: "返回给定角度的余弦值.",
		arguments: [
			{
				name: "number",
				description: "以弧度表示的，准备求其余弦值的角度"
			}
		]
	},
	{
		name: "COT",
		description: "返回一个角度的余切值.",
		arguments: [
			{
				name: "number",
				description: " 以弧度表示，准备求其余切值的角度"
			}
		]
	},
	{
		name: "COTH",
		description: "返回一个数字的双曲余切值.",
		arguments: [
			{
				name: "number",
				description: " 以弧度表示，准备求其双曲余切值的角度"
			}
		]
	},
	{
		name: "COUNTA",
		description: "计算区域中非空单元格的个数.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "是 1 到 255 个参数，代表要进行计数的值和单元格。值可以是任意类型的信息"
			},
			{
				name: "value2",
				description: "是 1 到 255 个参数，代表要进行计数的值和单元格。值可以是任意类型的信息"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "计算某个区域中空单元格的数目.",
		arguments: [
			{
				name: "range",
				description: "指要计算空单元格数目的区域"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "计算某个区域中满足给定条件的单元格数目.",
		arguments: [
			{
				name: "range",
				description: "要计算其中非空单元格数目的区域"
			},
			{
				name: "criteria",
				description: "以数字、表达式或文本形式定义的条件"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "统计一组给定条件所指定的单元格数.",
		arguments: [
			{
				name: "criteria_range",
				description: "是要为特定条件计算的单元格区域"
			},
			{
				name: "criteria",
				description: "是数字、表达式或文本形式的条件，它定义了单元格统计的范围"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "返回从票息期开始到结算日之间的天数.",
		arguments: [
			{
				name: "settlement",
				description: "是债券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是债券的到期日，以一串日期表示"
			},
			{
				name: "frequency",
				description: "是每年支付票息的次数"
			},
			{
				name: "basis",
				description: "是所采用的日算类型"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "返回结算日后的下一票息支付日.",
		arguments: [
			{
				name: "settlement",
				description: "是债券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是债券的到期日，以一串日期表示"
			},
			{
				name: "frequency",
				description: "是每年支付票息的次数"
			},
			{
				name: "basis",
				description: "是所采用的日算类型"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "返回结算日与到期日之间可支付的票息数.",
		arguments: [
			{
				name: "settlement",
				description: "是债券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是债券的到期日，以一串日期表示"
			},
			{
				name: "frequency",
				description: "是每年支付票息的次数"
			},
			{
				name: "basis",
				description: "是所采用的日算类型"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "返回结算日前的上一票息支付日.",
		arguments: [
			{
				name: "settlement",
				description: "是债券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是债券的到期日，以一串日期表示"
			},
			{
				name: "frequency",
				description: "是每年支付票息的次数"
			},
			{
				name: "basis",
				description: "是所采用的日算类型"
			}
		]
	},
	{
		name: "COVAR",
		description: "返回协方差，即每对变量的偏差乘积的均值.",
		arguments: [
			{
				name: "array1",
				description: "第一组整数单元格区域，必须为数值、数组或包含数值的引用"
			},
			{
				name: "array2",
				description: "第二组整数单元格区域，必须为数值、名称、数组或包含数值的引用"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "返回一个数值，它是使得累积二项式分布的函数值大于等于临界值 α 的最小整数.",
		arguments: [
			{
				name: "trials",
				description: "是贝努利试验次数"
			},
			{
				name: "probability_s",
				description: "是一次试验中成功的概率，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "alpha",
				description: "是临界值，介于 0 与 1 之间，含 0 与 1"
			}
		]
	},
	{
		name: "CSC",
		description: "返回一个角度的余切值.",
		arguments: [
			{
				name: "number",
				description: " 以弧度表示，准备求其余切值的角度"
			}
		]
	},
	{
		name: "CSCH",
		description: "返回一个角度的双曲余割值.",
		arguments: [
			{
				name: "number",
				description: " 以弧度表示，准备求其双曲余割值的角度"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "返回两个付款期之间为贷款累积支付的利息.",
		arguments: [
			{
				name: "rate",
				description: "是利率"
			},
			{
				name: "nper",
				description: "是总付款期数"
			},
			{
				name: "pv",
				description: "是现值"
			},
			{
				name: "start_period",
				description: "是计算的第一期"
			},
			{
				name: "end_period",
				description: "是计算的最后一期"
			},
			{
				name: "type",
				description: "是付款的计时"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "返回两个付款期之间为贷款累积支付的本金.",
		arguments: [
			{
				name: "rate",
				description: "是利率"
			},
			{
				name: "nper",
				description: "是总付款期数"
			},
			{
				name: "pv",
				description: "是现值"
			},
			{
				name: "start_period",
				description: "是计算的第一期"
			},
			{
				name: "end_period",
				description: "是计算的最后一期"
			},
			{
				name: "type",
				description: "是付款的计时"
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
		description: "将日期值从字符串转化为序列数，表示 Spreadsheet 日期-时间代码的日期.",
		arguments: [
			{
				name: "date_text",
				description: "按 Spreadsheet 日期格式表示的字符串，应在 1/1/1900 (Windows) 或 1/1/1904 (Macintosh) 到 12/31/9999 之间"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "计算满足给定条件的列表或数据库的列中数值的平均值。请查看“帮助”.",
		arguments: [
			{
				name: "database",
				description: "构成列表或数据库的单元格区域。数据库是一系列相关的数据的列表"
			},
			{
				name: "field",
				description: "用双引号括住的列标签，或是表示该列在列表中位置的数值"
			},
			{
				name: "criteria",
				description: "包含给定条件的单元格区域。该区域包括列标签及列标签下满足条件的单元格"
			}
		]
	},
	{
		name: "DAY",
		description: "返回一个月中的第几天的数值，介于 1 到 31 之间。.",
		arguments: [
			{
				name: "serial_number",
				description: "Spreadsheet 进行日期及时间计算时使用的日期-时间代码"
			}
		]
	},
	{
		name: "DAYS360",
		description: "按每年 360 天返回两个日期间相差的天数(每月 30 天).",
		arguments: [
			{
				name: "start_date",
				description: "start_date 和 end_date 是要计算天数差值的起止日期"
			},
			{
				name: "end_date",
				description: "start_date 和 end_date 是要计算天数差值的起止日期"
			},
			{
				name: "method",
				description: "是一个指定计算方法的逻辑值: FALSE 或忽略，使用美国(NASD)方法；TRUE，使用欧洲方法。"
			}
		]
	},
	{
		name: "DB",
		description: "用固定余额递减法，返回指定期间内某项固定资产的折旧值.",
		arguments: [
			{
				name: "cost",
				description: "固定资产原值"
			},
			{
				name: "salvage",
				description: "资产使用年限结束时的估计残值"
			},
			{
				name: "life",
				description: "进行折旧计算的周期总数，也称固定资产的生命周期"
			},
			{
				name: "period",
				description: "进行折旧计算的期次，它必须与前者使用相同的单位"
			},
			{
				name: "month",
				description: "第一年的月份数，默认值为 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "从满足给定条件的数据库记录的字段(列)中，计算数值单元格数目.",
		arguments: [
			{
				name: "database",
				description: "构成列表或数据库的单元格区域。数据库是一系列相关的数据"
			},
			{
				name: "field",
				description: "用双引号括住的列标签，或是表示该列在列表中位置的数值"
			},
			{
				name: "criteria",
				description: "包含指定条件的单元格区域。区域包括列标签及列标签下满足某个条件的单元格"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "对满足指定条件的数据库中记录字段(列)的非空单元格进行记数.",
		arguments: [
			{
				name: "database",
				description: "是构成列表或数据库的单元格区域。数据库是相关数据的列表"
			},
			{
				name: "field",
				description: "或是用双引号括住的列标签，或是表示该列在列表中位置的数字"
			},
			{
				name: "criteria",
				description: "是包含指定条件的单元格区域。区域包括列标签及列标签下满足某个条件的单元格"
			}
		]
	},
	{
		name: "DDB",
		description: "用双倍余额递减法或其他指定方法，返回指定期间内某项固定资产的折旧值.",
		arguments: [
			{
				name: "cost",
				description: "固定资产原值"
			},
			{
				name: "salvage",
				description: "固定资产使用年限终了时的估计残值"
			},
			{
				name: "life",
				description: "固定资产进行折旧计算的周期总数，也称固定资产的生命周期"
			},
			{
				name: "period",
				description: "进行折旧计算的期次，它必须与前者使用相同的单位"
			},
			{
				name: "factor",
				description: "余额递减速率，若省略，则采用默认值 2(双倍余额递减)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "将十进制数转换为二进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的十进制整数"
			},
			{
				name: "places",
				description: "是要使用的字符数"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "将十进制数转换为十六进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的十进制整数"
			},
			{
				name: "places",
				description: "是要使用的字符数"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "将十进制数转换为八进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的十进制整数"
			},
			{
				name: "places",
				description: "是要使用的字符数"
			}
		]
	},
	{
		name: "DELTA",
		description: "测试两个数字是否相等.",
		arguments: [
			{
				name: "number1",
				description: "是第一个数"
			},
			{
				name: "number2",
				description: "是第二个数"
			}
		]
	},
	{
		name: "DGET",
		description: "从数据库中提取符合指定条件且唯一存在的记录.",
		arguments: [
			{
				name: "database",
				description: "是构成列表或数据库的单元格区域。数据库是相关数据的列表"
			},
			{
				name: "field",
				description: "或是用双引号括住的列标签，或是表示该列在列表中位置的数字"
			},
			{
				name: "criteria",
				description: "是包含指定条件的单元格区域。区域包括列标签及列标签下满足某个条件的单元格"
			}
		]
	},
	{
		name: "DISC",
		description: "返回债券的贴现率.",
		arguments: [
			{
				name: "settlement",
				description: "是债券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是债券的到期日，以一串日期表示"
			},
			{
				name: "pr",
				description: "是每张票面为 100 元的债券的现价"
			},
			{
				name: "redemption",
				description: "是每张票面为 100 元的债券的赎回值"
			},
			{
				name: "basis",
				description: "是所采用的日算类型"
			}
		]
	},
	{
		name: "DMAX",
		description: "返回满足给定条件的数据库中记录的字段(列)中数据的最大值.",
		arguments: [
			{
				name: "database",
				description: "构成列表或数据库的单元格区域。数据库是一系列相关的数据列表"
			},
			{
				name: "field",
				description: "用双引号括住的列标签，或是表示该列在列表中位置的数值"
			},
			{
				name: "criteria",
				description: "包含数据库条件的单元格区域。该区域包括列标签及列标签下满足条件的单元格"
			}
		]
	},
	{
		name: "DMIN",
		description: "返回满足给定条件的数据库中记录的字段(列)中数据的最小值.",
		arguments: [
			{
				name: "database",
				description: "构成列表或数据库的单元格区域。数据库是一系列相关的数据列表"
			},
			{
				name: "field",
				description: "用双引号括住的列标签，或是表示该列在列表中位置的数值"
			},
			{
				name: "criteria",
				description: "包含数据库条件的单元格区域。该区域包括列标签及列标签下满足条件的单元格"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "将以分数表示的货币值转换为以小数表示的货币值.",
		arguments: [
			{
				name: "fractional_dollar",
				description: "是以分数表示的货币值"
			},
			{
				name: "fraction",
				description: "是分数的分母中使用的整数"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "将以小数表示的货币值转换为以分数表示的货币值.",
		arguments: [
			{
				name: "decimal_dollar",
				description: "是小数值"
			},
			{
				name: "fraction",
				description: "是分数的分母中使用的整数"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "与满足指定条件的数据库中记录字段(列)的值相乘.",
		arguments: [
			{
				name: "database",
				description: "是构成列表或数据库的单元格区域。数据库是相关数据的列表"
			},
			{
				name: "field",
				description: "或是用双引号括住的列标签，或是表示该列在列表中位置的数字"
			},
			{
				name: "criteria",
				description: "是包含指定条件的单元格区域。区域包括列标签及列标签下满足某个条件的单元格"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "根据所选数据库条目中的样本估算数据的标准偏差.",
		arguments: [
			{
				name: "database",
				description: "构成列表或数据库的单元格区域。数据库是一系列相关的数据列表"
			},
			{
				name: "field",
				description: "用双引号括住的列标签，或是表示该列在列表中位置的数值"
			},
			{
				name: "criteria",
				description: "包含数据库条件的单元格区域。该区域包括列标签及列标签下满足条件的单元格"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "以数据库选定项作为样本总体，计算数据的标准偏差.",
		arguments: [
			{
				name: "database",
				description: "是构成列表或数据库的单元格区域。数据库是相关数据的列表"
			},
			{
				name: "field",
				description: "或是用双引号括住的列标签，或是表示该列在列表中位置的数字"
			},
			{
				name: "criteria",
				description: "是包含指定条件的单元格区域。区域包括列标签及列标签下满足某个条件的单元格"
			}
		]
	},
	{
		name: "DSUM",
		description: "求满足给定条件的数据库中记录的字段(列)数据的和.",
		arguments: [
			{
				name: "database",
				description: "构成列表或数据库的单元格区域。数据库是一系列相关的数据"
			},
			{
				name: "field",
				description: "用双引号括住的列标签，或是表示该列在列表中位置的数值"
			},
			{
				name: "criteria",
				description: "包含指定条件的单元格区域。区域包括列标签及列标签下满足某个条件的单元格"
			}
		]
	},
	{
		name: "DVAR",
		description: "根据所选数据库条目中的样本估算数据的方差.",
		arguments: [
			{
				name: "database",
				description: "构成列表或数据库的单元格区域。数据库是一系列相关的数据"
			},
			{
				name: "field",
				description: "用双引号括住的列标签，或是表示该列在列表中位置的数值"
			},
			{
				name: "criteria",
				description: "包含数据库条件的单元格区域。该区域包括列标签及列标签下满足条件的单元格"
			}
		]
	},
	{
		name: "DVARP",
		description: "以数据库选定项作为样本总体，计算数据的总体方差.",
		arguments: [
			{
				name: "database",
				description: "是构成列表或数据库的单元格区域。数据库是相关数据的列表"
			},
			{
				name: "field",
				description: "或是用双引号括住的列标签，或是表示该列在列表中位置的数字"
			},
			{
				name: "criteria",
				description: "是包含指定条件的单元格区域。区域包括列标签及列标签下满足某个条件的单元格"
			}
		]
	},
	{
		name: "EDATE",
		description: "返回一串日期，指示起始日期之前/之后的月数.",
		arguments: [
			{
				name: "start_date",
				description: "是一串代表起始日期的日期"
			},
			{
				name: "months",
				description: "是 start_date 之前/之后的月数"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "返回 URL 编码的字符串.",
		arguments: [
			{
				name: "text",
				description: " 是 URL 编码的字符串"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "返回一串日期，表示指定月数之前或之后的月份的最后一天.",
		arguments: [
			{
				name: "start_date",
				description: "是一串代表起始日期的日期"
			},
			{
				name: "months",
				description: "是 start_date 之前/之后的月数"
			}
		]
	},
	{
		name: "ERF",
		description: "返回误差函数.",
		arguments: [
			{
				name: "lower_limit",
				description: "是整合 ERF 的下界"
			},
			{
				name: "upper_limit",
				description: "是整合 ERF 的上界"
			}
		]
	},
	{
		name: "ERF。精确",
		description: "返回误差函数.",
		arguments: [
			{
				name: "X",
				description: "是整合 ERF.PRECISE 的下界"
			}
		]
	},
	{
		name: "ERFC",
		description: "返回补余误差函数.",
		arguments: [
			{
				name: "x",
				description: "是整合 ERF 的下界"
			}
		]
	},
	{
		name: "ERFC。精确",
		description: "返回补余误差函数.",
		arguments: [
			{
				name: "X",
				description: "是整合 ERFC.PRECISE 的下界"
			}
		]
	},
	{
		name: "EXP",
		description: "返回 e 的 n 次方.",
		arguments: [
			{
				name: "number",
				description: "指数。常数 e 等于 2.71828182845904，是自然对数的底"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "返回指数分布.",
		arguments: [
			{
				name: "x",
				description: "用于指数分布函数计算的区间点，非负数值"
			},
			{
				name: "lambda",
				description: "指数分布函数的参数，正数"
			},
			{
				name: "cumulative",
				description: "逻辑值，当函数为累积分布函数时，返回值为 TRUE；当为概率密度函数时，返回值为 FALSE，指定使用何种形式的指数函数"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "返回指数分布.",
		arguments: [
			{
				name: "x",
				description: "是用于指数分布函数计算的区间点，非负数值"
			},
			{
				name: "lambda",
				description: "是指数分布函数的参数，正数"
			},
			{
				name: "cumulative",
				description: "是逻辑值，当函数为累积分布函数时，返回值为 TRUE；当为概率密度函数时，返回值为 FALSE"
			}
		]
	},
	{
		name: "F.DIST",
		description: "返回两组数据的(左尾) F 概率分布.",
		arguments: [
			{
				name: "x",
				description: "用来计算概率分布的区间点，非负数值"
			},
			{
				name: "deg_freedom1",
				description: "分子的自由度，大小介于 1 和 10^10 之间，不含 10^10"
			},
			{
				name: "deg_freedom2",
				description: "分母的自由度，大小介于 1 和 10^10 之间，不含 10^10"
			},
			{
				name: "cumulative",
				description: "逻辑值，当函数为累积分布函数时，返回值为 TRUE；当为概率密度函数时，返回值为 FALSE"
			}
		]
	},
	{
		name: "F.DIST。RT",
		description: "返回两组数据的(右尾) F 概率分布.",
		arguments: [
			{
				name: "x",
				description: "用来计算概率分布的区间点，非负数值"
			},
			{
				name: "deg_freedom1",
				description: "分子的自由度，大小介于 1 和 10^10 之间，不含 10^10"
			},
			{
				name: "deg_freedom2",
				description: "分母的自由度，大小介于 1 和 10^10 之间，不含 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "返回(左尾) F 概率分布的逆函数值，如果 p = F.DIST(x,...)，那么 FINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "F 累积分布的概率值，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "deg_freedom1",
				description: "分子的自由度，大小介于 1 和 10^10 之间，不包括 10^10"
			},
			{
				name: "deg_freedom2",
				description: "分母的自由度，大小介于 1 和 10^10 之间，不包括 10^10"
			}
		]
	},
	{
		name: "F.INV。RT",
		description: "返回(右尾) F 概率分布的逆函数值，如果 p = F.DIST.RT(x,...)，那么 F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "F 累积分布的概率值，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "deg_freedom1",
				description: "分子的自由度，大小介于 1 和 10^10 之间，不含 10^10"
			},
			{
				name: "deg_freedom2",
				description: "分母的自由度，大小介于 1 和 10^10 之间，不含 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "返回 F 检验的结果，F 检验返回的是当 Array1 和 Array2 的方差无明显差异时的双尾概率.",
		arguments: [
			{
				name: "array1",
				description: "是第一个数组或数据区域，可以是数值、名称、数组或者是包含数值的引用(将忽略空白对象)"
			},
			{
				name: "array2",
				description: "是第二个数组或数据区域，可以是数值、名称、数组或者是包含数值的引用(将忽略空白对象)"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "返回数字的双阶乘.",
		arguments: [
			{
				name: "number",
				description: "是求解双阶乘的值"
			}
		]
	},
	{
		name: "FDIST",
		description: "返回两组数据的(右尾) F 概率分布(自由度).",
		arguments: [
			{
				name: "x",
				description: "是用来计算函数的值，非负值"
			},
			{
				name: "deg_freedom1",
				description: "是分子的自由度，大小介于 1 和 10^10 之间，不包含 10^10"
			},
			{
				name: "deg_freedom2",
				description: "是分母的自由度，大小介于 1 和 10^10 之间，不包含 10^10"
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
		name: "FINV",
		description: "返回(右尾) F 概率分布的逆函数值，如果 p = FDIST(x,...)，那么 FINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "F 分布的概率值，在 0 与 1 之间的值"
			},
			{
				name: "deg_freedom1",
				description: "分子的自由度，介于 1 与 10^10 之间，不包含 10^10"
			},
			{
				name: "deg_freedom2",
				description: "分母的自由度，介于 1 与 10^10 之间，不包含 10^10"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "返回 Fisher 逆变换值，如果 y = FISHER(x)，那么 FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "需要进行逆变换的数值"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "将参数向下舍入为最接近的整数或最接近的指定基数的倍数.",
		arguments: [
			{
				name: "number",
				description: "需要进行舍入运算的数值"
			},
			{
				name: "significance",
				description: "用以进行舍入计算的倍数。"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "作为字符串返回公式.",
		arguments: [
			{
				name: "reference",
				description: " 是对公式的引用"
			}
		]
	},
	{
		name: "FTEST",
		description: "返回 F 检验的结果，F 检验返回的是当 Array1 和 Array2 的方差无明显差异时的双尾概率.",
		arguments: [
			{
				name: "array1",
				description: "是第一个数组或数据区域，可以是数值、名称、数组或者是包含数值的引用(将忽略空白对象)"
			},
			{
				name: "array2",
				description: "是第二个数组或数据区域，可以是数值、名称、数组或者是包含数值的引用(将忽略空白对象)"
			}
		]
	},
	{
		name: "FV",
		description: "基于固定利率和等额分期付款方式，返回某项投资的未来值.",
		arguments: [
			{
				name: "rate",
				description: "各期利率。例如，当利率为 6% 时，使用 6%/4 计算一个季度的还款额"
			},
			{
				name: "nper",
				description: "总投资期，即该项投资总的付款期数"
			},
			{
				name: "pmt",
				description: "各期支出金额，在整个投资期内不变"
			},
			{
				name: "pv",
				description: "从该项投资开始计算时已经入账的款项；或一系列未来付款当前值的累积和。如果忽略，Pv=0"
			},
			{
				name: "type",
				description: "数值 0 或 1,指定付款时间是期初还是期末。1 = 期初；0 或忽略 = 期末"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "返回在应用一系列复利后，初始本金的终值.",
		arguments: [
			{
				name: "principal",
				description: "是现值"
			},
			{
				name: "schedule",
				description: "是要应用的利率组合"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "返回具有给定概率的 γ 累积分布的区间点.",
		arguments: [
			{
				name: "probability",
				description: "γ 分布的概率值，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "alpha",
				description: "γ 分布的一个参数，正数"
			},
			{
				name: "beta",
				description: "γ 分布的一个参数，为一个正数。当 beta= 1 时，GAMMAINV 返回标准 γ 分布的逆"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "返回 γ 分布函数.",
		arguments: [
			{
				name: "x",
				description: "是用来计算 γ 分布的值，非负数值"
			},
			{
				name: "alpha",
				description: "是 γ 分布的一个参数，正数"
			},
			{
				name: "beta",
				description: "是 γ 分布的一个参数，正数。当 beta=1 时，GAMMADIST 返回标准 γ 分布函数"
			},
			{
				name: "cumulative",
				description: "决定函数形式的逻辑值: 返回累积分布函数时，等于 TRUE；返回概率密度函数时，等于 FALSE 或忽略"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "返回具有给定概率的 γ 累积分布的区间点.",
		arguments: [
			{
				name: "probability",
				description: "是 γ 分布的概率值，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "alpha",
				description: "是 γ 分布的一个参数，正数"
			},
			{
				name: "beta",
				description: "是 γ 分布的一个参数，为一个正数。当 beta= 1 时，GAMMAINV 返回标准 γ 分布的区间点"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "返回 γ 函数的自然对数.",
		arguments: [
			{
				name: "x",
				description: "一个要计算 GAMMALN 的正数"
			}
		]
	},
	{
		name: "GAMMALN。精确",
		description: "返回 γ 函数的自然对数.",
		arguments: [
			{
				name: "x",
				description: "一个要计算 GAMMALN.PRECISE 的正数"
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
		description: "返回最大公约数.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是 1 到 255 个值"
			},
			{
				name: "number2",
				description: "是 1 到 255 个值"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "返回一正数数组或数值区域的几何平均数.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是用于计算几何平均数的 1 到 255 个数值、名称、数组，或者是对数值的引用"
			},
			{
				name: "number2",
				description: "是用于计算几何平均数的 1 到 255 个数值、名称、数组，或者是对数值的引用"
			}
		]
	},
	{
		name: "GESTEP",
		description: "测试某个数字是否大于阈值.",
		arguments: [
			{
				name: "number",
				description: "是按步长测试的值"
			},
			{
				name: "step",
				description: "是阈值"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "提取存储在数据透视表中的数据.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "data_field",
				description: "是要从中提取数据的数据字段的名称"
			},
			{
				name: "pivot_table",
				description: "是到包含要检索数据的数据透视表中某单元格或单元格区域的引用"
			},
			{
				name: "field",
				description: "是要引用的字段"
			},
			{
				name: "item",
				description: "是要引用的字段项"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "返回一组正数的调和平均数: 所有参数倒数平均值的倒数.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是用于计算调和平均数的 1 到 255 个数值、名称、数组，或者是数值的引用"
			},
			{
				name: "number2",
				description: "是用于计算调和平均数的 1 到 255 个数值、名称、数组，或者是数值的引用"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "将十六进制数转换为二进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的十六进制数"
			},
			{
				name: "places",
				description: "是要使用的字符数"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "将十六进制数转换为十进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的十六进制数"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "将十六进制数转换为八进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的十六进制数"
			},
			{
				name: "places",
				description: "是要使用的字符数"
			}
		]
	},
	{
		name: "Hlookup 函数",
		description: "搜索数组区域首行满足条件的元素，确定待检索单元格在区域中的列序号，再进一步返回选定单元格的值.",
		arguments: [
			{
				name: "lookup_value",
				description: "需要在数据表首行进行搜索的值，可以是数值、引用或字符串"
			},
			{
				name: "table_array",
				description: "需要在其中搜索数据的文本、数据或逻辑值表。Table_array 可为区域或区域名的引用"
			},
			{
				name: "row_index_num",
				description: "满足条件的单元格在数组区域 table_array 中的行序号。表中第一行序号为 1"
			},
			{
				name: "range_lookup",
				description: "逻辑值: 如果为 TRUE 或忽略，在第一行中查找最近似的匹配；如果为 FALSE，查找时精确匹配"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "返回超几何分布.",
		arguments: [
			{
				name: "sample_s",
				description: "样本中成功的数目"
			},
			{
				name: "number_sample",
				description: "样本容量"
			},
			{
				name: "population_s",
				description: "样本总体中成功的数目"
			},
			{
				name: "number_pop",
				description: "样本总体的容量"
			},
			{
				name: "cumulative",
				description: "逻辑值，决定函数的形式。累积分布函数，使用 TRUE；概率密度函数，使用 FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "返回超几何分布.",
		arguments: [
			{
				name: "sample_s",
				description: "是样本中成功的数目"
			},
			{
				name: "number_sample",
				description: "是样本容量"
			},
			{
				name: "population_s",
				description: "是样本总体中成功的数目"
			},
			{
				name: "number_pop",
				description: "是样本总体的容量"
			}
		]
	},
	{
		name: "IF",
		description: "判断是否满足某个条件，如果满足返回一个值，如果不满足则返回另一个值。.",
		arguments: [
			{
				name: "logical_test",
				description: " 是任何可能被计算为 TRUE 或 FALSE 的数值或表达式。"
			},
			{
				name: "value_if_true",
				description: " 是 Logical_test 为 TRUE 时的返回值。如果忽略，则返回 TRUE。IF 函数最多可嵌套七层。"
			},
			{
				name: "value_if_false",
				description: " 是当 Logical_test 为 FALSE 时的返回值。如果忽略，则返回 FALSE"
			}
		]
	},
	{
		name: "IFERROR",
		description: "如果表达式是一个错误，则返回 value_if_error，否则返回表达式自身的值.",
		arguments: [
			{
				name: "value",
				description: "是任意值、表达式或引用"
			},
			{
				name: "value_if_error",
				description: "是任意值、表达式或引用"
			}
		]
	},
	{
		name: "IMABS",
		description: "返回复数的绝对值(模数).",
		arguments: [
			{
				name: "inumber",
				description: "是求解绝对值的复数"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "返回辐角 q (以弧度表示的角度).",
		arguments: [
			{
				name: "inumber",
				description: "是求解辐角的复数"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "返回复数的共轭复数.",
		arguments: [
			{
				name: "inumber",
				description: "是求解共轭复数的复数"
			}
		]
	},
	{
		name: "IMCOS",
		description: "返回复数的余弦值.",
		arguments: [
			{
				name: "inumber",
				description: "是求解余弦值的复数"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "返回复数的双曲余弦值.",
		arguments: [
			{
				name: "inumber",
				description: " 准备求其双曲余弦值的复数"
			}
		]
	},
	{
		name: "IMCOT",
		description: "返回复数的余切值.",
		arguments: [
			{
				name: "inumber",
				description: " 准备求其余切值的复数"
			}
		]
	},
	{
		name: "IMCSC",
		description: "返回复数的余割值.",
		arguments: [
			{
				name: "inumber",
				description: " 准备求其余割值的复数"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "返回复数的双曲余割值.",
		arguments: [
			{
				name: "inumber",
				description: " 准备求其双曲余割值的复数"
			}
		]
	},
	{
		name: "IMDIV",
		description: "返回两个复数之商.",
		arguments: [
			{
				name: "inumber1",
				description: "是复数分子(被除数)"
			},
			{
				name: "inumber2",
				description: "是复数分母(除数)"
			}
		]
	},
	{
		name: "IMEXP",
		description: "返回复数的指数值.",
		arguments: [
			{
				name: "inumber",
				description: "是求解指数值的复数"
			}
		]
	},
	{
		name: "IMLN",
		description: "返回复数的自然对数.",
		arguments: [
			{
				name: "inumber",
				description: "是求解自然对数的复数"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "返回以 10 为底的复数的对数.",
		arguments: [
			{
				name: "inumber",
				description: "是求解常用对数的复数"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "返回以 2 为底的复数的对数.",
		arguments: [
			{
				name: "inumber",
				description: "是求解以 2 为底的对数的复数"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "返回 1 到 255 个复数的乘积.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "Inumber1, Inumber2,... 是要相乘的 1 到 255 个复数"
			},
			{
				name: "inumber2",
				description: "Inumber1, Inumber2,... 是要相乘的 1 到 255 个复数"
			}
		]
	},
	{
		name: "IMREAL",
		description: "返回复数的实部系数.",
		arguments: [
			{
				name: "inumber",
				description: "是求解实部系数的复数"
			}
		]
	},
	{
		name: "IMSEC",
		description: "返回复数的正割值.",
		arguments: [
			{
				name: "inumber",
				description: " 准备求其正割值的复数"
			}
		]
	},
	{
		name: "IMSECH",
		description: "返回复数的双曲正割值.",
		arguments: [
			{
				name: "inumber",
				description: " 准备求其双曲正割值的复数"
			}
		]
	},
	{
		name: "IMSIN",
		description: "返回复数的正弦值.",
		arguments: [
			{
				name: "inumber",
				description: "是求解正弦值的复数"
			}
		]
	},
	{
		name: "IMSINH",
		description: " 返回复数的双曲正弦值.",
		arguments: [
			{
				name: "inumber",
				description: " 准备求其双曲正弦值的复数"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "返回复数的平方根.",
		arguments: [
			{
				name: "inumber",
				description: "是求解平方根的复数"
			}
		]
	},
	{
		name: "IMSUB",
		description: "返回两个复数的差值.",
		arguments: [
			{
				name: "inumber1",
				description: "是从中减去 inumber2 的复数"
			},
			{
				name: "inumber2",
				description: "是从 inumber1 上减去的复数"
			}
		]
	},
	{
		name: "IMSUM",
		description: "返回复数的和.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "是要相加的 1 到 255 个复数"
			},
			{
				name: "inumber2",
				description: "是要相加的 1 到 255 个复数"
			}
		]
	},
	{
		name: "INT",
		description: "将数值向下取整为最接近的整数.",
		arguments: [
			{
				name: "number",
				description: "要取整的实数"
			}
		]
	},
	{
		name: "INTRATE",
		description: "返回完全投资型债券的利率.",
		arguments: [
			{
				name: "settlement",
				description: "是债券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是债券的到期日，以一串日期表示"
			},
			{
				name: "investment",
				description: "是投资债券的金额"
			},
			{
				name: "redemption",
				description: "是在到期日收回的金额"
			},
			{
				name: "basis",
				description: "是所采用的日算类型"
			}
		]
	},
	{
		name: "IPMT",
		description: "返回在定期偿还、固定利率条件下给定期次内某项投资回报(或贷款偿还)的利息部分.",
		arguments: [
			{
				name: "rate",
				description: "各期利率。例如，当利率为 6% 时，使用 6%/4 计算一个季度的还款额"
			},
			{
				name: "per",
				description: "用于计算利息的期次，它必须介于 1 和付息总次数 Nper 之间"
			},
			{
				name: "nper",
				description: "总投资(或贷款)期，即该项投资(或贷款)的付款期总数"
			},
			{
				name: "pv",
				description: "从该项投资(或贷款)开始计算时已经入账的款项，或一系列未来付款当前值的累积和"
			},
			{
				name: "fv",
				description: "未来值，或在最后一次付款后获得的现金余额。如果忽略，Fv = 0"
			},
			{
				name: "type",
				description: "数值 0 或 1 ，用以指定付息方式在期初还是在期末。如果为 0 或忽略，在期末；如果为 1，在期初"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "检查是否引用了空单元格，返回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "要检查的单元格或单元格名称"
			}
		]
	},
	{
		name: "ISERR",
		description: " 检查一个值是否为 #N/A 以外的错误(#VALUE!、#REF!、#DIV/0!、#NUM!、#NAME? 或 #NULL!)，返回 TRUE 或 FALSE。.",
		arguments: [
			{
				name: "value",
				description: " 是要测试的值。该值可以是一个单元格、公式，也可以是引用单元格、公式或值的名称"
			}
		]
	},
	{
		name: "ISERROR",
		description: " 检查一个值是否为错误(#N/A、 #VALUE!、#REF!、#DIV/0!、#NUM!、#NAME? 或 #NULL!)，返回 TRUE 或 FALSE，.",
		arguments: [
			{
				name: "value",
				description: " 是要检测的值。检测值可以是一个单元格、公式，也可以是引用单元格、公式或值的名称"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "检查引用是否指向包含公式的单元格，并返回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "reference",
				description: " 是对要测试的单元格的引用。引用可以是单元格引用、公式或引用单元格的名称"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "检测一个值是否是逻辑值(TRUE 或 FALSE)，返回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "检测值。该值可以是一个单元格、公式，或者是一个单元格、公式，或数值的名称"
			}
		]
	},
	{
		name: "ISNA",
		description: "检测一个值是否为 #N/A，返回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "检测值。检测值可以是一个单元格、公式，或者是一个单元格、公式，或数值的名称"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "检测一个值是否不是文本(空单元格不是文本)，返回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "要检测的值，可以是单元格；公式；或者是单元格、公式或数值的引用"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "检测一个值是否是数值，返回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "检测值。可以是一个单元格、公式，或者是一个单元格、公式，或数值的名称"
			}
		]
	},
	{
		name: "ISO。天花板",
		description: "将参数向上舍入为最接近的整数，或最接近的指定基数的倍数.",
		arguments: [
			{
				name: "number",
				description: "是要舍入的值"
			},
			{
				name: "significance",
				description: "是要舍入到的可选倍数"
			}
		]
	},
	{
		name: "ISODD",
		description: "如果数字为奇数则返回 TRUE.",
		arguments: [
			{
				name: "number",
				description: "是要测试的值"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "返回给定日期所在年份的 ISO 周数目.",
		arguments: [
			{
				name: "date",
				description: " 是由 Spreadsheet 用于日期和时间计算的日期时间代码"
			}
		]
	},
	{
		name: "ISPMT",
		description: "返回普通(无担保)贷款的利息偿还.",
		arguments: [
			{
				name: "rate",
				description: "各期利率。例如，当利率为 6% 时，使用 6%/4 计算一个季度的还款额"
			},
			{
				name: "per",
				description: "用于计算利息的期次"
			},
			{
				name: "nper",
				description: "总贷款期，即该笔贷款的偿还总期数"
			},
			{
				name: "pv",
				description: "一系列未来付款当前值的累积和"
			}
		]
	},
	{
		name: "ISREF",
		description: "检测一个值是否为引用，返回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "检测值,可以是一个单元格、公式，或者是一个单元格、公式，或数值的名称"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "检测一个值是否为文本，返回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "value",
				description: "检测值，可以是一个单元格、公式，或者是一个单元格、公式，或数值的名称"
			}
		]
	},
	{
		name: "KURT",
		description: "返回一组数据的峰值.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是用于峰值计算的 1 到 255 个参数，可以是数值、名称、数组，或者是数值的引用"
			},
			{
				name: "number2",
				description: "是用于峰值计算的 1 到 255 个参数，可以是数值、名称、数组，或者是数值的引用"
			}
		]
	},
	{
		name: "LCM",
		description: "返回最小公倍数.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是 1 到 255 个求解最小其公倍数的值"
			},
			{
				name: "number2",
				description: "是 1 到 255 个求解最小其公倍数的值"
			}
		]
	},
	{
		name: "LEN",
		description: "返回文本字符串中的字符个数.",
		arguments: [
			{
				name: "text",
				description: "要计算长度的文本字符串；包括空格"
			}
		]
	},
	{
		name: "LN",
		description: "返回给定数值的自然对数.",
		arguments: [
			{
				name: "number",
				description: "要对其求自然对数的正实数"
			}
		]
	},
	{
		name: "LOG10",
		description: "返回给定数值以 10 为底的对数.",
		arguments: [
			{
				name: "number",
				description: "要对其求以 10 为底的对数的正实数"
			}
		]
	},
	{
		name: "LOGEST",
		description: "返回指数回归拟合曲线方程的参数.",
		arguments: [
			{
				name: "known_y's",
				description: "一组满足指数回归曲线 y=b*m^x 的 y 值"
			},
			{
				name: "known_x's",
				description: "一组满足指数回归曲线 y=b*m^x 的 x 值，为可选参数"
			},
			{
				name: "const",
				description: "逻辑值: 如果 Const=TRUE 或省略，常数 b 正常计算；如果 Const=FALSE，常数 b 为 1"
			},
			{
				name: "stats",
				description: "逻辑值，如果返回附加的回归统计值，返回 TRUE；如果返回系数 m 和常数 b,返回 FALSE"
			}
		]
	},
	{
		name: "LOGINV",
		description: "返回 x 的对数正态累积分布函数的区间点，其中 ln(x) 是平均数和标准方差参数的正态分布.",
		arguments: [
			{
				name: "probability",
				description: "是对数正态分布的概率，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "mean",
				description: "是 ln(x) 的平均数"
			},
			{
				name: "standard_dev",
				description: "是 ln(x) 的标准方差，正数"
			}
		]
	},
	{
		name: "LOGNORM。DIST",
		description: "返回对数正态分布.",
		arguments: [
			{
				name: "x",
				description: "用来进行函数计算的参数，正数"
			},
			{
				name: "mean",
				description: "ln(x) 的平均数"
			},
			{
				name: "standard_dev",
				description: "ln(x) 的标准方差，正数"
			},
			{
				name: "cumulative",
				description: "逻辑值，决定函数的形式。累积分布函数，使用 TRUE；概率密度函数，使用 FALSE"
			}
		]
	},
	{
		name: "LOGNORM。INV",
		description: "返回具有给定概率的对数正态分布函数的区间点.",
		arguments: [
			{
				name: "probability",
				description: "对数正态分布的概率，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "mean",
				description: "ln(x) 的平均数"
			},
			{
				name: "standard_dev",
				description: "ln(x) 的标准方差，正数"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "返回 x 的累积正态分布，其中 ln(x) 是平均数和标准方差参数的正态分布.",
		arguments: [
			{
				name: "x",
				description: "是用来进行函数计算的值，正数"
			},
			{
				name: "mean",
				description: "是 ln(x) 的平均数"
			},
			{
				name: "standard_dev",
				description: "是 ln(x) 的标准方差，正数"
			}
		]
	},
	{
		name: "LOWER",
		description: "将一个文本字符串的所有字母转换为小写形式.",
		arguments: [
			{
				name: "text",
				description: "要对其进行转换的字符串。其中不是英文字母的字符不变"
			}
		]
	},
	{
		name: "MAX",
		description: "返回一组数值中的最大值，忽略逻辑值及文本.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是准备从中求取最大值的 1 到 255 个数值、空单元格、逻辑值或文本数值"
			},
			{
				name: "number2",
				description: "是准备从中求取最大值的 1 到 255 个数值、空单元格、逻辑值或文本数值"
			}
		]
	},
	{
		name: "MAXA",
		description: "返回一组参数中的最大值(不忽略逻辑值和字符串).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "是要求最大值的 1 到 255 个参数，可以是数值、空单元格、逻辑值或文本型数值"
			},
			{
				name: "value2",
				description: "是要求最大值的 1 到 255 个参数，可以是数值、空单元格、逻辑值或文本型数值"
			}
		]
	},
	{
		name: "MDETERM",
		description: "返回一数组所代表的矩阵行列式的值.",
		arguments: [
			{
				name: "array",
				description: "行数和列数相等的数值数组，或是单元格区域，或是数组常量"
			}
		]
	},
	{
		name: "MIN",
		description: "返回一组数值中的最小值，忽略逻辑值及文本.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是准备从中求取最小值的 1 到 255 个数值、空单元格、逻辑值或文本数值"
			},
			{
				name: "number2",
				description: "是准备从中求取最小值的 1 到 255 个数值、空单元格、逻辑值或文本数值"
			}
		]
	},
	{
		name: "MIRR",
		description: "返回在考虑投资成本以及现金再投资利率下一系列分期现金流的内部报酬率.",
		arguments: [
			{
				name: "values",
				description: "一个数组，或对数字单元格区的引用。代表固定期间内一系列支出(负数)及收入(正数)值"
			},
			{
				name: "finance_rate",
				description: "现金流中投入资金的融资利率"
			},
			{
				name: "reinvest_rate",
				description: "将各期收入净额再投资的报酬率"
			}
		]
	},
	{
		name: "MMULT",
		description: "返回两数组的矩阵积，结果矩阵的行数与 array1 相等，列数与 array2 相等.",
		arguments: [
			{
				name: "array1",
				description: "是用于乘积计算的第一个数组数值，array1 的列数应该与 array2 的行数相等"
			},
			{
				name: "array2",
				description: "是用于乘积计算的第一个数组数值，array1 的列数应该与 array2 的行数相等"
			}
		]
	},
	{
		name: "MOD",
		description: "返回两数相除的余数.",
		arguments: [
			{
				name: "number",
				description: "被除数"
			},
			{
				name: "divisor",
				description: "除数"
			}
		]
	},
	{
		name: "MROUND",
		description: "返回一个舍入到所需倍数的数字.",
		arguments: [
			{
				name: "number",
				description: "是要舍入的值"
			},
			{
				name: "multiple",
				description: "是要舍入到的倍数"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "返回一组数字的多项式.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是 1 到 255 个用于计算多项式的值"
			},
			{
				name: "number2",
				description: "是 1 到 255 个用于计算多项式的值"
			}
		]
	},
	{
		name: "MUNIT",
		description: "返回指定维度的单位矩阵.",
		arguments: [
			{
				name: "dimension",
				description: " 是一个整数，指定要返回的单位矩阵的维度"
			}
		]
	},
	{
		name: "N",
		description: "将不是数值形式的值转换为数值形式。日期转换成序列值，TRUE 转换成 1，其他值转换成 0.",
		arguments: [
			{
				name: "value",
				description: "要进行转换的值"
			}
		]
	},
	{
		name: "NA",
		description: "返回错误值 #N/A (无法计算出数值).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "返回负二项式分布函数值.",
		arguments: [
			{
				name: "number_f",
				description: "失败的次数"
			},
			{
				name: "number_s",
				description: "成功次数阀值"
			},
			{
				name: "probability_s",
				description: "一次试验中成功的概率,介于 0 与 1 之间"
			},
			{
				name: "cumulative",
				description: "逻辑值，决定函数的形式。累积分布函数，使用 TRUE；概率密度函数，使用 FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "返回负二项式分布函数，第 Number_s 次成功之前将有 Number_f 次失败的概率，具有 Probability_s 成功概率.",
		arguments: [
			{
				name: "number_f",
				description: "是失败的次数"
			},
			{
				name: "number_s",
				description: "是成功次数阀值"
			},
			{
				name: "probability_s",
				description: "是一次试验中成功的概率，介于 0 与 1 之间"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "返回两个日期之间的完整工作日数.",
		arguments: [
			{
				name: "start_date",
				description: "是一串表示起始日期的数字"
			},
			{
				name: "end_date",
				description: "是一串表示结束日期的数字"
			},
			{
				name: "holidays",
				description: "是要从工作日历中去除的一个或多个日期(一串数字)的可选组合，如传统假日、国家法定假日及非固定假日"
			}
		]
	},
	{
		name: "NETWORKDAYS。国际机场",
		description: "使用自定义周末参数返回两个日期之间的完整工作日数.",
		arguments: [
			{
				name: "start_date",
				description: "是一串表示起始日期的数字"
			},
			{
				name: "end_date",
				description: "是一串表示结束日期的数字"
			},
			{
				name: "weekend",
				description: "是一个用于指定周末个数的数字或字符串"
			},
			{
				name: "holidays",
				description: "是要从工作日历中去除的一个或多个日期(一串数字)的可选组合，如传统假日、国家法定假日及非固定假日"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "返回年度的单利.",
		arguments: [
			{
				name: "effect_rate",
				description: "是有效利率"
			},
			{
				name: "npery",
				description: "是每年的复利计算期数"
			}
		]
	},
	{
		name: "NORMINV",
		description: "返回指定平均值和标准方差的正态累积分布函数的区间点.",
		arguments: [
			{
				name: "probability",
				description: "是正态分布对应的概率，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "mean",
				description: "是分布的算术平均"
			},
			{
				name: "standard_dev",
				description: "是分布的标准方差，正数"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "返回标准正态累积分布函数值(具有零平均值和一标准方差).",
		arguments: [
			{
				name: "z",
				description: "用于计算标准正态分布函数的值"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "返回标准正态累积分布的区间点(具有零平均值和一标准方差).",
		arguments: [
			{
				name: "probability",
				description: "是正态分布对应的概率，介于 0 与 1 之间，含 0 与 1"
			}
		]
	},
	{
		name: "NPER",
		description: "基于固定利率和等额分期付款方式，返回某项投资或贷款的期数.",
		arguments: [
			{
				name: "rate",
				description: "各期利率。例如，当利率为 6% 时，使用 6%/4 计算一个季度的还款额"
			},
			{
				name: "pmt",
				description: "各期还款额"
			},
			{
				name: "pv",
				description: "从该项投资或贷款开始计算时已经入账的款项，或一系列未来付款当前值的累积和"
			},
			{
				name: "fv",
				description: "未来值，或在最后一次付款后可以获得的现金余额"
			},
			{
				name: "type",
				description: "数值 0 或 1 ，用来指定付款时间是期初还是期末"
			}
		]
	},
	{
		name: "NPV",
		description: "基于一系列将来的收(正值)支(负值)现金流和一贴现率，返回一项投资的净现值.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rate",
				description: "是一期的整个阶段的贴现率"
			},
			{
				name: "value1",
				description: "代表支出和收入的 1 到 254 个参数，时间均匀分布并出现在每期末尾"
			},
			{
				name: "value2",
				description: "代表支出和收入的 1 到 254 个参数，时间均匀分布并出现在每期末尾"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "按独立于区域设置的方式将文本转换为数字.",
		arguments: [
			{
				name: "text",
				description: " 代表要转换的数字的字符串"
			},
			{
				name: "decimal_separator",
				description: " 用作字符串的小数点分隔符的字符"
			},
			{
				name: "group_separator",
				description: " 用作字符串的组分隔符的字符"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "将八进制数转换为二进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的八进制数"
			},
			{
				name: "places",
				description: "是要使用的字符数"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "将八进制数转换为十进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的八进制数"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "将八进制数转换为十六进制.",
		arguments: [
			{
				name: "number",
				description: "是要转换的八进制数"
			},
			{
				name: "places",
				description: "是要使用的字符数"
			}
		]
	},
	{
		name: "OR",
		description: "如果任一参数值为 TRUE，即返回 TRUE；只有当所有参数值均为 FALSE 时才返回 FALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "1 到 225 个结果是 TRUE 或 FALSE 的检测条件"
			},
			{
				name: "logical2",
				description: "1 到 225 个结果是 TRUE 或 FALSE 的检测条件"
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
		description: "返回投资达到指定的值所需的期数.",
		arguments: [
			{
				name: "rate",
				description: " 是每期利率"
			},
			{
				name: "pv",
				description: " 是投资的现值"
			},
			{
				name: "fv",
				description: " 是投资的所需未来值"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "返回特定数值在一组数中的百分比排名.",
		arguments: [
			{
				name: "array",
				description: "为元素间关系确定的数值数组或数值区域"
			},
			{
				name: "x",
				description: "为数组中需要得到其排名的某一个元素的数值"
			},
			{
				name: "significance",
				description: "待返回百分比值的有效位数。此参数是可选的。如果忽略，则默认为 3 位(0.XXX%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "返回特定数值在一组数中的百分比排名(介于 0 与 1 之间，不含 0 与 1).",
		arguments: [
			{
				name: "array",
				description: "为元素间关系确定的数值数组或数值区域"
			},
			{
				name: "x",
				description: "为数组中需要得到其排名的某一个元素的数值"
			},
			{
				name: "significance",
				description: "待返回百分比值的有效位数。此参数是可选的。如果忽略，则默认为 3 位(0.XXX%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "返回特定数值在一组数中的百分比排名(介于 0 与 1 之间，含 0 与 1).",
		arguments: [
			{
				name: "array",
				description: "为元素间关系确定的数值数组或数值区域"
			},
			{
				name: "x",
				description: "为数组中需要得到其排名的某一个元素的数值"
			},
			{
				name: "significance",
				description: "待返回百分比值的有效位数。此参数是可选的。如果忽略，则默认为 3 位(0.XXX%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "返回从给定元素数目的集合中选取若干元素的排列数.",
		arguments: [
			{
				name: "number",
				description: "元素总数"
			},
			{
				name: "number_chosen",
				description: "每个排列中的元素数目"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "返回可以从对象总数中选取的给定数目对象(包含重复项)的排列数.",
		arguments: [
			{
				name: "number",
				description: " 对象总数"
			},
			{
				name: "number_chosen",
				description: " 每个排列中的对象数目"
			}
		]
	},
	{
		name: "PHI",
		description: "返回标准正态分布的密度函数值.",
		arguments: [
			{
				name: "x",
				description: " 是计算其标准正态分布密度的数字"
			}
		]
	},
	{
		name: "PI",
		description: "返回圆周率 Pi 的值，3.14159265358979，精确到 15 位.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "计算在固定利率下，贷款的等额分期偿还额.",
		arguments: [
			{
				name: "rate",
				description: "各期利率。例如，当利率为 6% 时，使用 6%/4 计算一个季度的还款额"
			},
			{
				name: "nper",
				description: "总投资期或贷款期，即该项投资或贷款的付款期总数"
			},
			{
				name: "pv",
				description: "从该项投资(或贷款)开始计算时已经入账的款项，或一系列未来付款当前值的累积和"
			},
			{
				name: "fv",
				description: "未来值，或在最后一次付款后可以获得的现金余额。如果忽略，则认为此值为 0"
			},
			{
				name: "type",
				description: "逻辑值 0 或1，用以指定付款时间在期初还是在期末。如果为 1，付款在期初；如果为 0 或忽略，付款在期末"
			}
		]
	},
	{
		name: "POISSON",
		description: "返回泊松(POISSON)分布.",
		arguments: [
			{
				name: "x",
				description: "是事件出现的次数"
			},
			{
				name: "mean",
				description: "是期望值(正数)"
			},
			{
				name: "cumulative",
				description: "是逻辑值，指定概率分布的返回形式，累积泊松概率，使用 TRUE；泊松概率密度函数，使用 FALSE"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "返回泊松(POISSON)分布.",
		arguments: [
			{
				name: "x",
				description: "事件出现的次数"
			},
			{
				name: "mean",
				description: "期望值(正数)"
			},
			{
				name: "cumulative",
				description: "逻辑值，指定概率分布的返回形式"
			}
		]
	},
	{
		name: "POWER",
		description: "返回某数的乘幂.",
		arguments: [
			{
				name: "number",
				description: "底数，任何实数"
			},
			{
				name: "power",
				description: "幂值"
			}
		]
	},
	{
		name: "PPMT",
		description: "返回在定期偿还、固定利率条件下给定期次内某项投资回报(或贷款偿还)的本金部分.",
		arguments: [
			{
				name: "rate",
				description: "各期利率。例如，当利率为 6% 时，使用 6%/4 计算一个季度的还款额"
			},
			{
				name: "per",
				description: "用于计算其本金数额的期次，它必须介于 1 和付款总次数 nper 之间"
			},
			{
				name: "nper",
				description: "总投资(或贷款)期，即该项投资(或贷款)的付款期总数"
			},
			{
				name: "pv",
				description: "从该项投资(或贷款)开始计算时已经入账的款项，或一系列未来付款当前值的累积和"
			},
			{
				name: "fv",
				description: "未来值，或在最后一次付款后可以获得的现金余额"
			},
			{
				name: "type",
				description: "数值 0 或 1 ，用以指定付款方式在期末还是期初。如果为 0 或忽略，在期末；如果为 1，在期初"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "返回每张票面为 100 元的已贴现债券的现价.",
		arguments: [
			{
				name: "settlement",
				description: "是债券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是债券的到期日，以一串日期表示"
			},
			{
				name: "discount",
				description: "是债券的贴现率"
			},
			{
				name: "redemption",
				description: "是每张票面为 100 元的债券的赎回值"
			},
			{
				name: "basis",
				description: "是所采用的日算类型"
			}
		]
	},
	{
		name: "PROB",
		description: "返回一概率事件组中符合指定条件的事件集所对应的概率之和.",
		arguments: [
			{
				name: "x_range",
				description: "具有各自不同的概率值的一组数值 x"
			},
			{
				name: "prob_range",
				description: "与 x_range 中数值相对应的一组概率值，取值区间为 0(不含) 到 1"
			},
			{
				name: "lower_limit",
				description: "用于概率求和计算的数值的下界"
			},
			{
				name: "upper_limit",
				description: "用于概率求和计算的数值的可选上界。如果省略，PROB 返回当 X_range 等于 Lower_limit 时的概率"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "计算所有参数的乘积.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是要计算乘积的 1 到 255 个数值、逻辑值或者代表数值的字符串"
			},
			{
				name: "number2",
				description: "是要计算乘积的 1 到 255 个数值、逻辑值或者代表数值的字符串"
			}
		]
	},
	{
		name: "PROPER",
		description: "将一个文本字符串中各英文单词的第一个字母转换成大写，将其他字符转换成小写.",
		arguments: [
			{
				name: "text",
				description: "所要转换的字符串数据，可以是包含在一对双引号中的字符串、能够返回字符串的公式，或者是对文本单元格的引用"
			}
		]
	},
	{
		name: "PV",
		description: "返回某项投资的一系列将来偿还额的当前总值(或一次性偿还额的现值).",
		arguments: [
			{
				name: "rate",
				description: "各期利率。例如，当利率为 6% 时，使用 6%/4 计算一个季度的还款额"
			},
			{
				name: "nper",
				description: "总投资期，即该项投资的偿款期总数"
			},
			{
				name: "pmt",
				description: "是各期所获得的金额，在整个投资期内不变"
			},
			{
				name: "fv",
				description: "未来值，或在最后一次付款期后获得的一次性偿还额"
			},
			{
				name: "type",
				description: "逻辑值 0 或1，用以指定付款时间在期初还是在期末。如果为 1，付款在期初；如果为 0 或忽略，付款在期末"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "返回一组数据的四分位点.",
		arguments: [
			{
				name: "array",
				description: "用来计算其四分位点的数值数组或数值区域"
			},
			{
				name: "quart",
				description: "数字，按四分位从小到大依次为 0 到 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "基于从 0 到 1 之间(不含 0 与 1)的百分点值，返回一组数据的四分位点.",
		arguments: [
			{
				name: "array",
				description: "用来计算其四分位点的数值数组或数值区域"
			},
			{
				name: "quart",
				description: "数字，按四分位从小到大依次为 0 到 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "基于从 0 到 1 之间(含 0 与 1)的百分点值，返回一组数据的四分位点.",
		arguments: [
			{
				name: "array",
				description: "用来计算其四分位点的数值数组或数值区域"
			},
			{
				name: "quart",
				description: "数字，按四分位从小到大依次为 0 到 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "返回除法的整数部分.",
		arguments: [
			{
				name: "numerator",
				description: "是被除数"
			},
			{
				name: "denominator",
				description: "是除数"
			}
		]
	},
	{
		name: "RADIANS",
		description: "将角度转为弧度.",
		arguments: [
			{
				name: "angle",
				description: "要转成弧度的角度值"
			}
		]
	},
	{
		name: "RAND",
		description: "返回大于或等于 0 且小于 1 的平均分布随机数(依重新计算而变).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "返回一个介于指定的数字之间的随机数.",
		arguments: [
			{
				name: "bottom",
				description: "是 RANDBETWEEN 能返回的最小整数"
			},
			{
				name: "top",
				description: "是 RANDBETWEEN 能返回的最大整数"
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
		description: "返回某数字在一列数字中相对于其他数值的大小排名.",
		arguments: [
			{
				name: "number",
				description: "是要查找排名的数字"
			},
			{
				name: "ref",
				description: "是一组数或对一个数据列表的引用。非数字值将被忽略"
			},
			{
				name: "order",
				description: "是在列表中排名的数字。如果为 0 或忽略，降序；非零值，升序"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "返回某数字在一列数字中相对于其他数值的大小排名；如果多个数值排名相同，则返回平均值排名.",
		arguments: [
			{
				name: "number",
				description: "指定的数字"
			},
			{
				name: "ref",
				description: "一组数或对一个数据列表的引用。非数字值将被忽略"
			},
			{
				name: "order",
				description: "指定排名的方式。如果为 0 或忽略，降序；非零值，升序"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "返回某数字在一列数字中相对于其他数值的大小排名；如果多个数值排名相同，则返回该组数值的最佳排名.",
		arguments: [
			{
				name: "number",
				description: "指定的数字"
			},
			{
				name: "ref",
				description: "一组数或对一个数据列表的引用。非数字值将被忽略"
			},
			{
				name: "order",
				description: "指定排名的方式。如果为 0 或忽略，降序；非零值，升序"
			}
		]
	},
	{
		name: "RATE",
		description: "返回投资或贷款的每期实际利率。例如，当利率为 6% 时，使用 6%/4 计算一个季度的还款额.",
		arguments: [
			{
				name: "nper",
				description: "总投资期或贷款期，即该项投资或贷款的付款期总数"
			},
			{
				name: "pmt",
				description: "各期所应收取(或支付)的金额，在整个投资期或付款期不能改变"
			},
			{
				name: "pv",
				description: "一系列未来付款的现值总额"
			},
			{
				name: "fv",
				description: "未来值，或在最后一次付款后可以获得的现金余额。如果忽略，Fv=0"
			},
			{
				name: "type",
				description: "数值 0 或1，用以指定付款时间在期初还是在期末。如果为 1，付款在期初；如果为 0 或忽略，付款在期末"
			},
			{
				name: "guess",
				description: "给定的利率猜测值。如果忽略，猜测值为 0.1(10%)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "返回完全投资型债券在到期日收回的金额.",
		arguments: [
			{
				name: "settlement",
				description: "是债券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是债券的到期日，以一串日期表示"
			},
			{
				name: "investment",
				description: "是投资债券的金额"
			},
			{
				name: "discount",
				description: "是债券的贴现率"
			},
			{
				name: "basis",
				description: "是所采用的日算类型"
			}
		]
	},
	{
		name: "REPLACE",
		description: "将一个字符串中的部分字符用另一个字符串替换.",
		arguments: [
			{
				name: "old_text",
				description: "要进行字符替换的文本"
			},
			{
				name: "start_num",
				description: "要替换为 new_text 的字符在 old_text 中的位置"
			},
			{
				name: "num_chars",
				description: "要从 old_text 中替换的字符个数"
			},
			{
				name: "new_text",
				description: "用来对 old_text 中指定字符串进行替换的字符串"
			}
		]
	},
	{
		name: "REPT",
		description: "根据指定次数重复文本。可用 RPET 在一个单元格中重复填写一个文本字符串.",
		arguments: [
			{
				name: "text",
				description: "要重复文本"
			},
			{
				name: "number_times",
				description: "文本的重复次数(正数)"
			}
		]
	},
	{
		name: "RIGHT",
		description: "从一个文本字符串的最后一个字符开始返回指定个数的字符.",
		arguments: [
			{
				name: "text",
				description: "要提取字符的字符串"
			},
			{
				name: "num_chars",
				description: "要提取的字符数；如果忽略，为 1"
			}
		]
	},
	{
		name: "ROMAN",
		description: "将阿拉伯数字转换成文本式罗马数字.",
		arguments: [
			{
				name: "number",
				description: "要转换的阿拉伯数字"
			},
			{
				name: "form",
				description: "一个用于指定罗马数字类型的数字"
			}
		]
	},
	{
		name: "ROUND",
		description: "按指定的位数对数值进行四舍五入.",
		arguments: [
			{
				name: "number",
				description: "要四舍五入的数值"
			},
			{
				name: "num_digits",
				description: "执行四舍五入时采用的位数。如果此参数为负数，则圆整到小数点的左边；如果此参数为零，则圆整到最接近的整数"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "向下舍入数字.",
		arguments: [
			{
				name: "number",
				description: "需要向下舍入的任意实数"
			},
			{
				name: "num_digits",
				description: "舍入后的数字位数。如果此参数为负数，则将小数舍入到小数点左边一位；如果参数为零，则将小数转换为最接近的整数"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "向上舍入数字.",
		arguments: [
			{
				name: "number",
				description: "需要向上舍入的任意实数"
			},
			{
				name: "num_digits",
				description: "舍入后的数字位数。如果此参数为负数，则将小数舍入到小数点左边一位；如果参数为零，则将小数转换为最接近的整数"
			}
		]
	},
	{
		name: "ROW",
		description: "返回一个引用的行号.",
		arguments: [
			{
				name: "reference",
				description: "准备求取其行号的单元格或单元格区域；如果忽略，则返回包含 ROW 函数的单元格"
			}
		]
	},
	{
		name: "ROWS",
		description: "返回某一引用或数组的行数.",
		arguments: [
			{
				name: "array",
				description: "要计算行数的数组、数组公式或是对单元格区域的引用"
			}
		]
	},
	{
		name: "RRI",
		description: "返回某项投资增长的等效利率.",
		arguments: [
			{
				name: "nper",
				description: " 是投资的期数"
			},
			{
				name: "pv",
				description: " 是投资的现值"
			},
			{
				name: "fv",
				description: " 是投资的未来值"
			}
		]
	},
	{
		name: "RSQ",
		description: "返回给定数据点的 Pearson 积矩法相关系数的平方.",
		arguments: [
			{
				name: "known_y's",
				description: "一个数值数组或数值区域，可以是数值、名称、数组，或者是数值的引用"
			},
			{
				name: "known_x's",
				description: "一个数值数组或数值区域，可以是数值、名称、数组，或者是数值的引用"
			}
		]
	},
	{
		name: "RTD",
		description: "从一个支持 COM 自动化的程序中获取实时数据.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "一个注册的 COM 自动化加载项的 ProgID 名称，名称放在双引号"
			},
			{
				name: "server",
				description: "运行该加载项的服务器名称，服务器名称放在双引号中。如果加载项在本地运行，请使用空字符串"
			},
			{
				name: "topic1",
				description: "用于指定数据的 1 到 38 个参数"
			},
			{
				name: "topic2",
				description: "用于指定数据的 1 到 38 个参数"
			}
		]
	},
	{
		name: "SEARCH",
		description: "返回一个指定字符或文本字符串在字符串中第一次出现的位置，从左到右查找(忽略大小写).",
		arguments: [
			{
				name: "find_text",
				description: "要查找的字符串。可以使用 ? 和 * 作为通配符；如果要查找 ? 和 * 字符，可使用 ~? 和 ~*"
			},
			{
				name: "within_text",
				description: "用来搜索 Find_text 的父字符串"
			},
			{
				name: "start_num",
				description: "数字值，用以指定从被搜索字符串左侧第几个字符开始查找。如果忽略，则为 1"
			}
		]
	},
	{
		name: "SEC",
		description: "返回角度的正切值.",
		arguments: [
			{
				name: "number",
				description: " 以弧度表示，准备求其正切值的角度"
			}
		]
	},
	{
		name: "SECH",
		description: "返回角度的双曲正割值.",
		arguments: [
			{
				name: "number",
				description: " 以弧度表示，准备求其双曲正割值的角度"
			}
		]
	},
	{
		name: "SECOND",
		description: "返回秒数值，是一个 0 到 59 之间的整数。.",
		arguments: [
			{
				name: "serial_number",
				description: "Spreadsheet 进行日期及时间计算的日期-时间代码，或以时间格式表示的文本，如 16:48:23 或 4:48:47 PM"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "返回基于公式的幂级数的和.",
		arguments: [
			{
				name: "x",
				description: "是幂级数的输入值"
			},
			{
				name: "n",
				description: "是 x 的初始幂次"
			},
			{
				name: "m",
				description: "是每级对 n 增加的步长"
			},
			{
				name: "coefficients",
				description: "是 x 的每个继接幂次所乘的一组系数"
			}
		]
	},
	{
		name: "SHEET",
		description: "返回引用的工作表的工作表编号.",
		arguments: [
			{
				name: "value",
				description: " 是需要工作表编号的工作表或引用的名称。如果省略，则返回包含函数的工作表的编号"
			}
		]
	},
	{
		name: "SHEETS",
		description: "返回引用中的工作表数目.",
		arguments: [
			{
				name: "reference",
				description: " 是要知道它包含的工作表数目的引用。如果省略，则返回工作簿中包含该函数的工作表数目"
			}
		]
	},
	{
		name: "SIGN",
		description: "返回数字的正负号: 为正时，返回 1；为零时，返回 0；为负时，返回 -1.",
		arguments: [
			{
				name: "number",
				description: "任意实数"
			}
		]
	},
	{
		name: "SIN",
		description: "返回给定角度的正弦值.",
		arguments: [
			{
				name: "number",
				description: "以弧度表示的，准备求其正弦值的角度。Degrees * PI()/180 = radians"
			}
		]
	},
	{
		name: "SINH",
		description: "返回双曲正弦值.",
		arguments: [
			{
				name: "number",
				description: "任意实数"
			}
		]
	},
	{
		name: "SKEW",
		description: "返回一个分布的不对称度: 用来体现某一分布相对其平均值的不对称程度.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是要计算不对称度的 1 到 255 个参数，可以是数值、名称、数组，或者是数值的引用"
			},
			{
				name: "number2",
				description: "是要计算不对称度的 1 到 255 个参数，可以是数值、名称、数组，或者是数值的引用"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "基于总体返回分布的不对称度: 用来体现某一分布相对其平均值的不对称程度.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: " 是要计算总体不对称度的 1 到 255 个参数，可以是数值、名称、数组或者是数值的引用"
			},
			{
				name: "number2",
				description: " 是要计算总体不对称度的 1 到 255 个参数，可以是数值、名称、数组或者是数值的引用"
			}
		]
	},
	{
		name: "SLN",
		description: "返回固定资产的每期线性折旧费.",
		arguments: [
			{
				name: "cost",
				description: "固定资产原值"
			},
			{
				name: "salvage",
				description: "固定资产使用年限终了时的估计残值"
			},
			{
				name: "life",
				description: "固定资产进行折旧计算的周期总数，也称固定资产的生命周期"
			}
		]
	},
	{
		name: "SLOPE",
		description: "返回经过给定数据点的线性回归拟合线方程的斜率.",
		arguments: [
			{
				name: "known_y's",
				description: "为因变量数组或数值区域，可以是数值、名称、数组，或者是数值的引用"
			},
			{
				name: "known_x's",
				description: "自变量数组或数值区域，可以是数值、名称、数组，或者是数值的引用"
			}
		]
	},
	{
		name: "SMALL",
		description: "返回数据组中第 k 个最小值.",
		arguments: [
			{
				name: "array",
				description: "要求第 K 个最小值点的数值数组或数值区域"
			},
			{
				name: "k",
				description: "要返回的最小值点在数组或数据区中的位次"
			}
		]
	},
	{
		name: "SQRT",
		description: "返回数值的平方根.",
		arguments: [
			{
				name: "number",
				description: "要对其求平方根的数值"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "返回(数字 * Pi)的平方根.",
		arguments: [
			{
				name: "number",
				description: "是 pi 的乘数"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "通过平均值和标准方差返回正态分布概率值.",
		arguments: [
			{
				name: "x",
				description: "用于求解正态分布概率的区间点"
			},
			{
				name: "mean",
				description: "分布的算术平均值"
			},
			{
				name: "standard_dev",
				description: "分布的标准方差"
			}
		]
	},
	{
		name: "STDEV",
		description: "估算基于给定样本的标准偏差(忽略样本中的逻辑值及文本).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是与总体抽样样本相应的 1 到 255 个数值，可以是数值，也可以是包含数值的引用"
			},
			{
				name: "number2",
				description: "是与总体抽样样本相应的 1 到 255 个数值，可以是数值，也可以是包含数值的引用"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "计算基于给定的样本总体的标准偏差(忽略逻辑值及文本).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是与总体抽样样本相应的 1 到 255 个数值，可以是数值，也可以是包含数值的引用"
			},
			{
				name: "number2",
				description: "是与总体抽样样本相应的 1 到 255 个数值，可以是数值，也可以是包含数值的引用"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "估算基于给定样本的标准偏差(忽略样本中的逻辑值及文本).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是与总体抽样样本相应的 1 到 255 个数值，可以是数值，也可以是包含数值的引用"
			},
			{
				name: "number2",
				description: "是与总体抽样样本相应的 1 到 255 个数值，可以是数值，也可以是包含数值的引用"
			}
		]
	},
	{
		name: "STDEVA",
		description: "估算基于给定样本(包括逻辑值和字符串)的标准偏差。字符串和逻辑值 FALSE 数值为 0；逻辑值 TRUE 为 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "是构成总体抽样样本的 1 到 255 个数值参数，可以是数字、名称或对数字的引用"
			},
			{
				name: "value2",
				description: "是构成总体抽样样本的 1 到 255 个数值参数，可以是数字、名称或对数字的引用"
			}
		]
	},
	{
		name: "STDEVP",
		description: "计算基于给定的样本总体的标准偏差(忽略逻辑值及文本).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是与总体抽样样本相应的 1 到 255 个数值，可以是数值，也可以是包含数值的引用"
			},
			{
				name: "number2",
				description: "是与总体抽样样本相应的 1 到 255 个数值，可以是数值，也可以是包含数值的引用"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "计算样本(包括逻辑值和字符串)总体的标准偏差。字符串和逻辑值 FALSE 数值为 0；逻辑值 TRUE 为 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "是构成样本总体的 1 到 255 个数值参数，这些参数可以是数值、名称、数组或对一个值的引用"
			},
			{
				name: "value2",
				description: "是构成样本总体的 1 到 255 个数值参数，这些参数可以是数值、名称、数组或对一个值的引用"
			}
		]
	},
	{
		name: "STEYX",
		description: "返回通过线性回归法计算纵坐标预测值所产生的标准误差.",
		arguments: [
			{
				name: "known_y's",
				description: "因变量数组或数值区域，可以是数值、名称、数组，或者是数值的引用"
			},
			{
				name: "known_x's",
				description: "自变量数组或数值区域，可以是数值、名称、数组，或者是数值的引用"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "将字符串中的部分字符串以新字符串替换.",
		arguments: [
			{
				name: "text",
				description: "包含有要替换字符的字符串或文本单元格引用"
			},
			{
				name: "old_text",
				description: "要被替换的字符串。如果原有字符串中的大小写与新字符串中的大小写不匹配的话，将不进行替换"
			},
			{
				name: "new_text",
				description: "用于替换 old_text 的新字符串"
			},
			{
				name: "instance_num",
				description: "若指定的字符串 old_text 在父字符串中出现多次，则用本参数指定要替换第几个。如果省略，则全部替换"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "返回一个数据列表或数据库的分类汇总.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "function_num",
				description: "是从 1 到 11 的数字，用来指定分类汇总所采用的汇总函数"
			},
			{
				name: "ref1",
				description: "为 1 到 254 个要进行分类汇总的区域或引用"
			}
		]
	},
	{
		name: "SUM",
		description: "计算单元格区域中所有数值的和.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: " 1 到 255 个待求和的数值。单元格中的逻辑值和文本将被忽略。但当作为参数键入时，逻辑值和文本有效"
			},
			{
				name: "number2",
				description: " 1 到 255 个待求和的数值。单元格中的逻辑值和文本将被忽略。但当作为参数键入时，逻辑值和文本有效"
			}
		]
	},
	{
		name: "SUMIF",
		description: "对满足条件的单元格求和.",
		arguments: [
			{
				name: "range",
				description: "要进行计算的单元格区域"
			},
			{
				name: "criteria",
				description: "以数字、表达式或文本形式定义的条件"
			},
			{
				name: "sum_range",
				description: "用于求和计算的实际单元格。如果省略，将使用区域中的单元格"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "对一组给定条件指定的单元格求和.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sum_range",
				description: "是求和的实际单元格"
			},
			{
				name: "criteria_range",
				description: "是要为特定条件计算的单元格区域"
			},
			{
				name: "criteria",
				description: "是数字、表达式或文本形式的条件，它定义了单元格求和的范围"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "返回相应的数组或区域乘积的和.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "array1",
				description: "是 2 到 255 个数组。所有数组的维数必须一样"
			},
			{
				name: "array2",
				description: "是 2 到 255 个数组。所有数组的维数必须一样"
			},
			{
				name: "array3",
				description: "是 2 到 255 个数组。所有数组的维数必须一样"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "返回所有参数的平方和。参数可以是数值、数组、名称，或者是对数值单元格的引用.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是用于平方和计算的 1 至 255 个参数，可以是数值、数组、名称，或者是数组的引用"
			},
			{
				name: "number2",
				description: "是用于平方和计算的 1 至 255 个参数，可以是数值、数组、名称，或者是数组的引用"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "计算两数组中对应数值平方差的和.",
		arguments: [
			{
				name: "array_x",
				description: "第一个数组或数值区域，可以是数值、名称、数组，或者是对数值的引用"
			},
			{
				name: "array_y",
				description: "第二个数组或数值区域，可以是数值、名称、数组，或者是对数值的引用"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "计算两数组中对应数值平方和的和.",
		arguments: [
			{
				name: "array_x",
				description: "第一个数组或数值区域"
			},
			{
				name: "array_y",
				description: "第二个数组或数值区域"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "求两数组中对应数值差的平方和.",
		arguments: [
			{
				name: "array_x",
				description: "第一个数组或数值区域，可以是数值、名称、数组，或者是对数值的引用"
			},
			{
				name: "array_y",
				description: "第二个数组或数值区域，可以是数值、名称、数组，或者是对数值的引用"
			}
		]
	},
	{
		name: "SYD",
		description: "返回某项固定资产按年限总和折旧法计算的每期折旧金额.",
		arguments: [
			{
				name: "cost",
				description: "固定资产原值"
			},
			{
				name: "salvage",
				description: "固定资产使用年限终了时的估计残值"
			},
			{
				name: "life",
				description: "固定资产进行折旧计算的周期总数，也称固定资产的生命周期"
			},
			{
				name: "per",
				description: "进行折旧计算的期次，它必须与前者使用相同的单位"
			}
		]
	},
	{
		name: "T",
		description: "检测给定值是否为文本，如果是文本按原样返回，如果不是文本则返回双引号(空文本).",
		arguments: [
			{
				name: "value",
				description: "要检测的值"
			}
		]
	},
	{
		name: "T.DIST",
		description: "返回左尾学生 t-分布.",
		arguments: [
			{
				name: "x",
				description: "用来计算 t-分布的数值"
			},
			{
				name: "deg_freedom",
				description: "一个整数，用于定义分布的自由度"
			},
			{
				name: "cumulative",
				description: "逻辑值，决定函数的形式。累积分布函数，使用 TRUE；概率密度函数，使用 FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "返回双尾学生 t-分布.",
		arguments: [
			{
				name: "x",
				description: "用来计算 t-分布的数值"
			},
			{
				name: "deg_freedom",
				description: "一个整数，用于定义分布的自由度"
			}
		]
	},
	{
		name: "T.DIST。RT",
		description: "返回右尾学生 t-分布.",
		arguments: [
			{
				name: "x",
				description: "用来计算 t-分布的数值"
			},
			{
				name: "deg_freedom",
				description: "一个整数，用于定义分布的自由度"
			}
		]
	},
	{
		name: "T.INV",
		description: "返回学生 t-分布的左尾区间点.",
		arguments: [
			{
				name: "probability",
				description: "双尾学生 t-分布的概率值，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "deg_freedom",
				description: "为一个正整数，用于定义分布的自由度"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "返回学生 t-分布的双尾区间点.",
		arguments: [
			{
				name: "probability",
				description: "双尾学生 t-分布的概率值，介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "deg_freedom",
				description: "为一个正整数，用于定义分布的自由度"
			}
		]
	},
	{
		name: "T.TEST",
		description: "返回学生 t-检验的概率值.",
		arguments: [
			{
				name: "array1",
				description: "第一组数据集"
			},
			{
				name: "array2",
				description: "第二组数据集"
			},
			{
				name: "tails",
				description: "用于定义所返回的分布的尾数: 1 代表单尾；2 代表双尾"
			},
			{
				name: "type",
				description: "用于定义 t-检验的类型: 1 代表成对检验；2 代表双样本等方差假设；3 代表双样本异方差假设"
			}
		]
	},
	{
		name: "TAN",
		description: "返回给定角度的正切值.",
		arguments: [
			{
				name: "number",
				description: "以弧度表示的，准备求其正切值的角度。Degrees * PI()/180 = radians"
			}
		]
	},
	{
		name: "TANH",
		description: "返回双曲正切值.",
		arguments: [
			{
				name: "number",
				description: "任意实数。"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "返回短期国库券的等价债券收益.",
		arguments: [
			{
				name: "settlement",
				description: "是短期国库券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是短期国库券的到期日，以一串日期表示"
			},
			{
				name: "discount",
				description: "是短期国库券的贴现率"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "返回每张票面为 100 元的短期国库券的现价.",
		arguments: [
			{
				name: "settlement",
				description: "是短期国库券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是短期国库券的到期日，以一串日期表示"
			},
			{
				name: "discount",
				description: "是短期国库券的贴现率"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "返回短期国库券的收益.",
		arguments: [
			{
				name: "settlement",
				description: "是短期国库券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是短期国库券的到期日，以一串日期表示"
			},
			{
				name: "pr",
				description: "是每张票面为 100 元的短期国库券的现价"
			}
		]
	},
	{
		name: "TDIST",
		description: "返回学生 t-分布.",
		arguments: [
			{
				name: "x",
				description: "用来计算 t 分布的数值"
			},
			{
				name: "deg_freedom",
				description: "是一个表示自由度的整数值，用于定义分布"
			},
			{
				name: "tails",
				description: "指定要返回的分布尾数的个数: 1 表示单尾分布；2 表示双尾分布"
			}
		]
	},
	{
		name: "TEXT",
		description: "根据指定的数值格式将数字转成文本.",
		arguments: [
			{
				name: "value",
				description: "数值、能够返回数值的公式，或者对数值单元格的引用"
			},
			{
				name: "format_text",
				description: "文字形式的数字格式，文字形式来自于“单元格格式”对话框“数字”选项卡的“分类”框(不是“常规”选择项卡)"
			}
		]
	},
	{
		name: "TIME",
		description: "返回特定时间的序列数.",
		arguments: [
			{
				name: "hour",
				description: "介于 0 到 23 之间的数字，代表小时数"
			},
			{
				name: "minute",
				description: "介于 0 到 59 之间的数字，代表分钟数"
			},
			{
				name: "second",
				description: "介于 0 到 59 之间的数字，代表秒数"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "将文本形式表示的时间转换成 Spreadsheet 序列数，是一个从 0 (12:00:00 AM) 到 0.999988426 (11:59:59 PM) 的数。在输入公式后将数字设置为时间格式.",
		arguments: [
			{
				name: "time_text",
				description: "按任何一种 Spreadsheet 时间格式表示的时间(其中的日期信息将忽略)"
			}
		]
	},
	{
		name: "TINV",
		description: "返回给定自由度和双尾概率的学生 t-分布的区间点.",
		arguments: [
			{
				name: "probability",
				description: "双尾学生 t-分布的概率值，位于区间 0 到 1之间，含 0 与 1"
			},
			{
				name: "deg_freedom",
				description: "为一正数，用于定义分布的自由度"
			}
		]
	},
	{
		name: "TODAY",
		description: "返回日期格式的的当前日期。.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "转置单元格区域.",
		arguments: [
			{
				name: "array",
				description: "工作表中的单元格区域或数组"
			}
		]
	},
	{
		name: "TREND",
		description: "返回线性回归拟合线的一组纵坐标值(y 值).",
		arguments: [
			{
				name: "known_y's",
				description: "满足线性拟合直线 y=mx+b 的一组已知的 y 值"
			},
			{
				name: "known_x's",
				description: "满足线性拟合直线 y = mx + b 的一组已知的 x 值，为可选项"
			},
			{
				name: "new_x's",
				description: "一组新 x 值，希望通过 TREND 函数推出相应的 y 值"
			},
			{
				name: "const",
				description: "逻辑值，用以指定是否强制常数 b 为 0，如果 Const=TRUE 或忽略，b 按通常方式计算；如果 Const=FALSE，b 强制为 0"
			}
		]
	},
	{
		name: "TRIM",
		description: "删除字符串中多余的空格，但会在英文字符串中保留一个作为词与词之间分隔的空格.",
		arguments: [
			{
				name: "text",
				description: "要删除空格的字符串"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "返回一组数据的修剪平均值.",
		arguments: [
			{
				name: "array",
				description: "用于截去极值后求取均值的数值数组或数值区域"
			},
			{
				name: "percent",
				description: "为一分数，用于指定数据点集中所要消除的极值比例"
			}
		]
	},
	{
		name: "TRUE",
		description: "返回逻辑值 TRUE.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "将数字截为整数或保留指定位数的小数.",
		arguments: [
			{
				name: "number",
				description: "要进行截尾操作的数字"
			},
			{
				name: "num_digits",
				description: "用于指定截尾精度的数字。如果忽略，为 0"
			}
		]
	},
	{
		name: "TTEST",
		description: "返回学生 t-检验的概率值.",
		arguments: [
			{
				name: "array1",
				description: "第一组数据集"
			},
			{
				name: "array2",
				description: "第二组数据集"
			},
			{
				name: "tails",
				description: "用于定义所返回的分布的尾数: 1 代表单尾；2 代表双尾"
			},
			{
				name: "type",
				description: "用于定义 t-检验的类型: 1 代表成对检验；2 代表双样本等方差假设；3 代表双样本异方差假设"
			}
		]
	},
	{
		name: "TYPE",
		description: "以整数形式返回参数的数据类型: 数值 = 1；文字 = 2；逻辑值 = 4；错误值 = 16；数组 = 64.",
		arguments: [
			{
				name: "value",
				description: "任何值"
			}
		]
	},
	{
		name: "UNICODE",
		description: "返回对应于文本的第一个字符的数字(代码点).",
		arguments: [
			{
				name: "text",
				description: " 是需要其 Unicode 值的字符"
			}
		]
	},
	{
		name: "UPPER",
		description: "将文本字符串转换成字母全部大写形式.",
		arguments: [
			{
				name: "text",
				description: "要转换成大写的文本字符串"
			}
		]
	},
	{
		name: "VALUE",
		description: "将一个代表数值的文本字符串转换成数值.",
		arguments: [
			{
				name: "text",
				description: "带双引号的文本，或是一个单元格引用。该单元格中有要被转换的文本"
			}
		]
	},
	{
		name: "VAR",
		description: "估算基于给定样本的方差(忽略样本中的逻辑值及文本).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是与总体抽样样本相应的 1 到 255 个数值参数"
			},
			{
				name: "number2",
				description: "是与总体抽样样本相应的 1 到 255 个数值参数"
			}
		]
	},
	{
		name: "VAR.P",
		description: "计算基于给定的样本总体的方差(忽略样本中的逻辑值及文本).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是与总体抽样样本相应的 1 到 255 个数值参数"
			},
			{
				name: "number2",
				description: "是与总体抽样样本相应的 1 到 255 个数值参数"
			}
		]
	},
	{
		name: "VAR.S",
		description: "估算基于给定样本的方差(忽略样本中的逻辑值及文本).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是与总体抽样样本相应的 1 到 255 个数值参数"
			},
			{
				name: "number2",
				description: "是与总体抽样样本相应的 1 到 255 个数值参数"
			}
		]
	},
	{
		name: "VARA",
		description: "估算基于给定样本(包括逻辑值和字符串)的方差。字符串和逻辑值 FALSE 数值为 0；逻辑值 TRUE 为 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "是构成总体抽样样本的 1 到 255 个数值参数"
			},
			{
				name: "value2",
				description: "是构成总体抽样样本的 1 到 255 个数值参数"
			}
		]
	},
	{
		name: "VARP",
		description: "计算基于给定的样本总体的方差(忽略样本总体中的逻辑值及文本).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是与总体抽样样本相应的 1 到 255 个数值参数"
			},
			{
				name: "number2",
				description: "是与总体抽样样本相应的 1 到 255 个数值参数"
			}
		]
	},
	{
		name: "VARPA",
		description: "计算样本(包括逻辑值和字符串)总体的方差。字符串和逻辑值 FALSE 数值为 0；逻辑值 TRUE 为 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "是构成样本总体的 1 到 255 个数值参数"
			},
			{
				name: "value2",
				description: "是构成样本总体的 1 到 255 个数值参数"
			}
		]
	},
	{
		name: "VDB",
		description: "返回某项固定资产用余额递减法或其他指定方法计算的特定或部分时期的折旧额.",
		arguments: [
			{
				name: "cost",
				description: "固定资产原值"
			},
			{
				name: "salvage",
				description: "固定资产使用年限终了时的估计残值"
			},
			{
				name: "life",
				description: "固定资产进行折旧计算的周期总数，也称固定资产的生命周期"
			},
			{
				name: "start_period",
				description: "进行折旧计算的开始期次，与生命周期的单位相同"
			},
			{
				name: "end_period",
				description: "进行折旧计算的结束期次，上述三者必须保持同样的单位(年，月等)"
			},
			{
				name: "factor",
				description: "指余额递减速率"
			},
			{
				name: "no_switch",
				description: "逻辑值，指定当折旧额超出用余额递减法计算的水平时，是否转换成直线折旧法"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "搜索表区域首列满足条件的元素，确定待检索单元格在区域中的行序号，再进一步返回选定单元格的值。默认情况下，表是以升序排序的.",
		arguments: [
			{
				name: "lookup_value",
				description: "需要在数据表首列进行搜索的值，lookup_value 可以是数值、引用或字符串"
			},
			{
				name: "table_array",
				description: "需要在其中搜索数据的信息表。Table-array 可以是对区域或区域名称的引用"
			},
			{
				name: "col_index_num",
				description: "满足条件的单元格在数组区域 table_array 中的列序号。首列序号为 1"
			},
			{
				name: "range_lookup",
				description: "指定在查找时是要求精确匹配，还是大致匹配。如果为 FALSE，大致匹配。如果为 TRUE 或忽略，精确匹配"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "返回代表一周中的第几天的数值，是一个 1 到 7 之间的整数。.",
		arguments: [
			{
				name: "serial_number",
				description: "一个表示返回值类型的数字: "
			},
			{
				name: "return_type",
				description: "从 星期日=1 到 星期六=7，用 1；从 星期一=1 到 星期日=7，用 2；从 星期一=0 到 星期日=6 时，用 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "返回一年中的周数.",
		arguments: [
			{
				name: "serial_number",
				description: "是 Spreadsheet 用于日期和时间计算的日期时间代码"
			},
			{
				name: "return_type",
				description: "是一个确定返回值类型的数值(1 或 2)"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "返回 Weibull 分布(概率密度).",
		arguments: [
			{
				name: "x",
				description: "用来计算分布的数值，非负"
			},
			{
				name: "alpha",
				description: "是分布的参数值，为正"
			},
			{
				name: "beta",
				description: "是分布的参数值，为正"
			},
			{
				name: "cumulative",
				description: "逻辑值，决定函数的形式。累积分布函数，使用 TRUE；概率密度函数，使用 FALSE"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "返回 Weibull 分布(概率密度).",
		arguments: [
			{
				name: "x",
				description: "用来计算分布的数值，非负数值"
			},
			{
				name: "alpha",
				description: "是分布的参数值，正数"
			},
			{
				name: "beta",
				description: "是分布的参数值，正数"
			},
			{
				name: "cumulative",
				description: "逻辑值，决定函数的形式。累积分布函数，使用 TRUE；概率密度函数，使用 FALSE"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "返回在指定的若干个工作日之前/之后的日期(一串数字).",
		arguments: [
			{
				name: "start_date",
				description: "是一串表示起始日期的数字"
			},
			{
				name: "days",
				description: "是 start_date 之前/之后非周末和非假日的天数"
			},
			{
				name: "holidays",
				description: "是要从工作日历中去除的一个或多个日期(一串数字)的可选组合，如传统假日、国家法定假日及非固定假日"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "使用自定义周末参数返回在指定的若干个工作日之前/之后的日期(一串数字).",
		arguments: [
			{
				name: "start_date",
				description: "是一串表示起始日期的数字"
			},
			{
				name: "days",
				description: "start_date 之前/之后非周末和非假日的天数"
			},
			{
				name: "weekend",
				description: "是一个用于指定周末个数的数字或字符串"
			},
			{
				name: "holidays",
				description: "是要从工作日历中去除的一个或多个日期(一串数字)的可选组合，如传统假日、国家法定假日及非固定假日"
			}
		]
	},
	{
		name: "XIRR",
		description: "返回现金流计划的内部回报率.",
		arguments: [
			{
				name: "values",
				description: "是一系列按日期对应付款计划的现金流"
			},
			{
				name: "dates",
				description: "是对应现金流付款的付款日期计划"
			},
			{
				name: "guess",
				description: "是一个认为接近 XIRR 结果的数字"
			}
		]
	},
	{
		name: "XNPV",
		description: "返回现金流计划的净现值.",
		arguments: [
			{
				name: "rate",
				description: "是应用于现金流的贴现率"
			},
			{
				name: "values",
				description: "是一系列按日期对应付款计划的现金流"
			},
			{
				name: "dates",
				description: "是对应现金流付款的付款日期计划"
			}
		]
	},
	{
		name: "XOR",
		description: "返回所有参数的逻辑“异或”值.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: " 您想要测试的 1 到 254 个条件，可以为 TURE 或 FALSE，可以是逻辑值、数组或引用"
			},
			{
				name: "logical2",
				description: " 您想要测试的 1 到 254 个条件，可以为 TURE 或 FALSE，可以是逻辑值、数组或引用"
			}
		]
	},
	{
		name: "YEAR",
		description: "返回日期的年份值，一个 1900-9999 之间的数字。.",
		arguments: [
			{
				name: "serial_number",
				description: "Spreadsheet 进行日期及时间计算的日期-时间代码"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "返回一个年分数，表示 start_date 和 end_date 之间的整天天数.",
		arguments: [
			{
				name: "start_date",
				description: "是一串代表起始日期的日期"
			},
			{
				name: "end_date",
				description: "是一串代表结束日期的日期"
			},
			{
				name: "basis",
				description: "是所采用的日算类型"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "返回已贴现债券的年收益，如短期国库券.",
		arguments: [
			{
				name: "settlement",
				description: "是债券的结算日，以一串日期表示"
			},
			{
				name: "maturity",
				description: "是债券的到期日，以一串日期表示"
			},
			{
				name: "pr",
				description: "是每张票面为 100 元的债券的现价"
			},
			{
				name: "redemption",
				description: "是每张票面为 100 元的债券的赎回值"
			},
			{
				name: "basis",
				description: "是所采用的日算类型"
			}
		]
	},
	{
		name: "Z.TEST",
		description: " 返回 z 测试的单尾 P 值。.",
		arguments: [
			{
				name: "array",
				description: " 是用于 z 测试的数值数组或数值区域。X"
			},
			{
				name: "x",
				description: " 是要测试的值。"
			},
			{
				name: "sigma",
				description: " 是总体标准偏差(已知)。如果忽略，则使用样本标准偏差"
			}
		]
	},
	{
		name: "ZTEST",
		description: "返回 z 测试的单尾 P 值。.",
		arguments: [
			{
				name: "array",
				description: " 是用于 z 测试的数值数组或数值区域。X"
			},
			{
				name: "x",
				description: " 是要测试的值"
			},
			{
				name: "sigma",
				description: " 是总体标准偏差(已知)。如果忽略，则使用样本标准偏差"
			}
		]
	},
	{
		name: "不",
		description: "对参数的逻辑值求反: 参数为 TRUE 时返回 FALSE；参数为 FALSE 时返回TRUE.",
		arguments: [
			{
				name: "logical",
				description: "可以对其进行真(TRUE)假(FALSE) 判断的任何值或表达式"
			}
		]
	},
	{
		name: "个百分点",
		description: "返回数组的 K 百分点值.",
		arguments: [
			{
				name: "array",
				description: "为元素间关系确定的数值数组或数值区域"
			},
			{
				name: "k",
				description: "0 到 1 之间(含 0 与 1)的百分点值"
			}
		]
	},
	{
		name: "个百分点。EXC",
		description: "返回数组的 K 百分点值，K 介于 0 与 1 之间，不含 0 与 1.",
		arguments: [
			{
				name: "array",
				description: "为元素间关系确定的数值数组或数值区域"
			},
			{
				name: "k",
				description: "0 到 1 之间(含 0 与 1)的百分点值"
			}
		]
	},
	{
		name: "个百分点。公司",
		description: "返回数组的 K 百分点值，K 介于 0 与 1 之间，含 0 与 1.",
		arguments: [
			{
				name: "array",
				description: "为元素间关系确定的数值数组或数值区域"
			},
			{
				name: "k",
				description: "0 到 1 之间(含 0 与 1)的百分点值"
			}
		]
	},
	{
		name: "中位数",
		description: "返回一组数的中值.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是用于中值计算的 1 到 255 个数字、名称、数组，或者是数值引用"
			},
			{
				name: "number2",
				description: "是用于中值计算的 1 到 255 个数字、名称、数组，或者是数值引用"
			}
		]
	},
	{
		name: "中期",
		description: "从文本字符串中指定的起始位置起返回指定长度的字符.",
		arguments: [
			{
				name: "text",
				description: "准备从中提取字符串的文本字符串"
			},
			{
				name: "start_num",
				description: "准备提取的第一个字符的位置。Text 中第一个字符为 1"
			},
			{
				name: "num_chars",
				description: "指定所要提取的字符串长度"
			}
		]
	},
	{
		name: "串联",
		description: "将多个文本字符串合并成一个.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "是 1 到 255 个要合并的文本字符串。可以是字符串、数字或对单个单元格的引用"
			},
			{
				name: "text2",
				description: "是 1 到 255 个要合并的文本字符串。可以是字符串、数字或对单个单元格的引用"
			}
		]
	},
	{
		name: "事实",
		description: "返回某数的阶乘，等于 1*2*...*Number.",
		arguments: [
			{
				name: "number",
				description: "要进行阶乘计算的非负数"
			}
		]
	},
	{
		name: "代码",
		description: "返回文本字符串第一个字符在本机所用字符集中的数字代码.",
		arguments: [
			{
				name: "text",
				description: "要取第一个字符代码的字符串"
			}
		]
	},
	{
		name: "伊姆坦",
		description: "返回复数的正切值.",
		arguments: [
			{
				name: "inumber",
				description: " 准备求其正切值的复数"
			}
		]
	},
	{
		name: "伽玛",
		description: "返回伽玛函数值.",
		arguments: [
			{
				name: "x",
				description: " 是您要为其计算伽玛的值"
			}
		]
	},
	{
		name: "伽马。DIST",
		description: "返回 γ 分布函数.",
		arguments: [
			{
				name: "x",
				description: "用来计算 γ 分布的区间点，非负数值"
			},
			{
				name: "alpha",
				description: "γ 分布的一个参数，正数"
			},
			{
				name: "beta",
				description: "γ 分布的一个参数，正数。如果 beta=1，GAMMADIST 返回标准 γ 分布函数"
			},
			{
				name: "cumulative",
				description: "决定函数形式的逻辑值: 返回累积分布函数时，等于 TRUE；返回概率密度函数时，等于 FALSE 或忽略"
			}
		]
	},
	{
		name: "信心",
		description: "使用正态分布，返回总体平均值的置信区间.",
		arguments: [
			{
				name: "alpha",
				description: "是用来计算置信区间的显著性水平,一个大于 0 小于 1 的数值"
			},
			{
				name: "standard_dev",
				description: "是假设为已知的数据范围的总体标准方差。Standard_dev 必须大于 0"
			},
			{
				name: "size",
				description: "是样本容量"
			}
		]
	},
	{
		name: "信息",
		description: "返回当前操作环境的有关信息.",
		arguments: [
			{
				name: "type_text",
				description: "指定所要获得的信息类型。"
			}
		]
	},
	{
		name: "偏移量",
		description: "以指定的引用为参照系，通过给定偏移量返回新的引用.",
		arguments: [
			{
				name: "reference",
				description: "作为参照系的引用区域，其左上角单元格是偏移量的起始位置"
			},
			{
				name: "rows",
				description: "相对于引用参照系的左上角单元格，上(下)偏移的行数"
			},
			{
				name: "cols",
				description: "相对于引用参照系的左上角单元格，左(右)偏移的列数"
			},
			{
				name: "height",
				description: "新引用区域的行数"
			},
			{
				name: "width",
				description: "新引用区域的列数"
			}
		]
	},
	{
		name: "内部回报率",
		description: "返回一系列现金流的内部报酬率.",
		arguments: [
			{
				name: "values",
				description: "一个数组，或对包含用来计算返回内部报酬率的数字的单元格的引用"
			},
			{
				name: "guess",
				description: "内部报酬率的猜测值。如果忽略，则为 0.1(百分之十)"
			}
		]
	},
	{
		name: "分钟",
		description: "返回分钟数值，是一个 0 到 59 之间的整数。.",
		arguments: [
			{
				name: "serial_number",
				description: "Spreadsheet 进行日期及时间计算的日期-时间代码，或以时间格式表示的文本，如 16:48:00 或 4:48:00 PM"
			}
		]
	},
	{
		name: "列",
		description: "返回一引用的列号.",
		arguments: [
			{
				name: "reference",
				description: "准备求取其列号的单元格或连续的单元格区域；如果忽略，则使用包含 COLUMN 函数的单元格"
			}
		]
	},
	{
		name: "匹配",
		description: "返回符合特定值特定顺序的项在数组中的相对位置.",
		arguments: [
			{
				name: "lookup_value",
				description: "在数组中所要查找匹配的值，可以是数值、文本或逻辑值，或者对上述类型的引用"
			},
			{
				name: "lookup_array",
				description: "含有要查找的值的连续单元格区域，一个数组，或是对某数组的引用"
			},
			{
				name: "match_type",
				description: "数字 -1、 0 或 1。 Match_type 指定了 Spreadsheet 将 lookup_value 与 lookup_array 中数值进行匹配的方式"
			}
		]
	},
	{
		name: "十进制",
		description: "按给定基数将数字的文本表示形式转换成十进制数.",
		arguments: [
			{
				name: "number",
				description: " 是您要转换的数字"
			},
			{
				name: "radix",
				description: " 是正在转换的数字的基数"
			}
		]
	},
	{
		name: "协方差。P",
		description: "返回总体协方差，即两组数值中每对变量的偏差乘积的平均值.",
		arguments: [
			{
				name: "array1",
				description: "第一组整数单元格区域，必须为数值、数组或包含数值的引用"
			},
			{
				name: "array2",
				description: "第二组整数单元格区域，必须为数值、数组或包含数值的引用"
			}
		]
	},
	{
		name: "协方差。S",
		description: "返回样本协方差，即两组数值中每对变量的偏差乘积的平均值.",
		arguments: [
			{
				name: "array1",
				description: "第一组整数单元格区域，必须为数值、数组或包含数值的引用"
			},
			{
				name: "array2",
				description: "第二组整数单元格区域，必须为数值、数组或包含数值的引用"
			}
		]
	},
	{
		name: "单元格",
		description: " 返回引用中第一个单元格的格式、位置或内容的有关信息(取决于工作表的读取顺序)。.",
		arguments: [
			{
				name: "info_type",
				description: " 是一个文本值，用于指定所需的单元格信息类型。"
			},
			{
				name: "reference",
				description: " 是要了解其信息的单元格"
			}
		]
	},
	{
		name: "参考",
		description: "如果表达式解析为 #N/A，则返回您指定的值,否则返回表达式的结果.",
		arguments: [
			{
				name: "value",
				description: " 是任何值或表达式或引用"
			},
			{
				name: "value_if_na",
				description: " 是任何值或表达式或引用"
			}
		]
	},
	{
		name: "吸烟与健康委员会",
		description: "返回双曲余弦值.",
		arguments: [
			{
				name: "number",
				description: "任意实数"
			}
		]
	},
	{
		name: "和",
		description: "检查是否所有参数均为 TRUE，如果所有参数值均为 TRUE，则返回 TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "是 1 到 255 个结果为 TRUE 或 FALSE 的检测条件，检测内容可以是逻辑值、数组或引用"
			},
			{
				name: "logical2",
				description: "是 1 到 255 个结果为 TRUE 或 FALSE 的检测条件，检测内容可以是逻辑值、数组或引用"
			}
		]
	},
	{
		name: "固定",
		description: "用定点小数格式将数值舍入成特定位数并返回带或不带逗号的文本.",
		arguments: [
			{
				name: "number",
				description: "准备舍入并转化为文本字符的数值"
			},
			{
				name: "decimals",
				description: "指小数点右边的位数。如果忽略，Decimals = 2"
			},
			{
				name: "no_commas",
				description: "一个逻辑值，指定在返回文本中是否显示逗号。为 TRUE 时显示逗号；忽略或为 FALSE 时则不显示逗号"
			}
		]
	},
	{
		name: "地区",
		description: "返回引用中涉及的区域个数。区域指一块连续的单元格或单个单元格.",
		arguments: [
			{
				name: "reference",
				description: "对某个单元格或单元格区域的引用，可包含多个区域"
			}
		]
	},
	{
		name: "地址",
		description: "创建一个以文本方式对工作簿中某一单元格的引用.",
		arguments: [
			{
				name: "row_num",
				description: "指定引用单元格的行号。例如，Row_number = 1 代表第 1 行"
			},
			{
				name: "column_num",
				description: "指定引用单元格的列标。例如，Column_number = 4 代表 D 列"
			},
			{
				name: "abs_num",
				description: "指定引用类型: 绝对引用 = 1；绝对行/相对列 = 2；相对行/绝对列 = 3；相对引用 = 4"
			},
			{
				name: "a1",
				description: "用逻辑值指定引用样式: A1 样式 = 1 或 TRUE；R1C1 样式 = 0 或 FALSE"
			},
			{
				name: "sheet_text",
				description: "字符串，指定用作外部引用的工作表的名称"
			}
		]
	},
	{
		name: "地板",
		description: "将参数向下舍入为最接近的指定基数的倍数.",
		arguments: [
			{
				name: "number",
				description: "需要进行舍入运算的数值"
			},
			{
				name: "significance",
				description: "用以进行舍入计算的倍数。Number 和 Significance 两个参数必须同时为正或同时为负"
			}
		]
	},
	{
		name: "地板。数学",
		description: "将数字向下舍入到最接近的整数或最接近的指定基数的倍数.",
		arguments: [
			{
				name: "number",
				description: " 是要进行舍入的值"
			},
			{
				name: "significance",
				description: " 是要舍入到的倍数"
			},
			{
				name: "mode",
				description: " 当为给定和非零值时，此函数将向零方向舍入"
			}
		]
	},
	{
		name: "均值",
		description: "返回各数据点与数据均值点之差(数据偏差)的平方和.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是用于计算偏差平方和的 1 到 255 个参数，或者是一个数组或数组引用"
			},
			{
				name: "number2",
				description: "是用于计算偏差平方和的 1 到 255 个参数，或者是一个数组或数组引用"
			}
		]
	},
	{
		name: "增长",
		description: "返回指数回归拟合曲线的一组纵坐标值(y 值).",
		arguments: [
			{
				name: "known_y's",
				description: "满足指数回归拟合曲线 y=b*m^x 的一组已知的 y 值"
			},
			{
				name: "known_x's",
				description: "满足指数回归拟合曲线 y=b*m^x 的一组已知的 x 值，个数与 y 值相同。为可选参数"
			},
			{
				name: "new_x's",
				description: "一组新 x 值，通过 GROWTH 函数返回各自对应的 y 值"
			},
			{
				name: "const",
				description: "逻辑值，指定是否将系数 b 强制设为 1。如果为 FALSE 或忽略，b 等于 1；如果为 TRUE，b 值按普通方法计算"
			}
		]
	},
	{
		name: "复杂",
		description: "将实部系数和虚部系数转换为复数.",
		arguments: [
			{
				name: "real_num",
				description: "是复数的实部系数"
			},
			{
				name: "i_num",
				description: "是复数的虚部系数"
			},
			{
				name: "suffix",
				description: "是复数虚部的后缀"
			}
		]
	},
	{
		name: "大",
		description: "返回数据组中第 k 个最大值。例如，第五个最大值.",
		arguments: [
			{
				name: "array",
				description: "用来计算第 k 个最大值点的数值数组或数值区域"
			},
			{
				name: "k",
				description: "所要返回的最大值点在数组或数据区中的位置(从最大值开始)"
			}
		]
	},
	{
		name: "天",
		description: "返回两个日期之间的天数。.",
		arguments: [
			{
				name: "end_date",
				description: "start_date 和 end_date 是您想要知道期间天数的两个日期 "
			},
			{
				name: "start_date",
				description: "start_date 和 end_date 是您想要知道期间天数的两个日期"
			}
		]
	},
	{
		name: "奇数",
		description: "将正(负)数向上(下)舍入到最接近的奇数.",
		arguments: [
			{
				name: "number",
				description: "要舍入的数值"
			}
		]
	},
	{
		name: "字段",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "小时",
		description: "返回小时数值，是一个 0 (12:00 A.M.) 到 23 (11:00 P.M.) 之间的整数。.",
		arguments: [
			{
				name: "serial_number",
				description: "Spreadsheet 进行日期及时间计算的日期-时间代码，或以时间格式表示的文本，如 16:48:00 或 4:48:00 PM"
			}
		]
	},
	{
		name: "左",
		description: "从一个文本字符串的第一个字符开始返回指定个数的字符.",
		arguments: [
			{
				name: "text",
				description: "要提取字符的字符串"
			},
			{
				name: "num_chars",
				description: "要 LEFT 提取的字符数；如果忽略，为 1"
			}
		]
	},
	{
		name: "平均",
		description: "返回其参数的算术平均值；参数可以是数值或包含数值的名称、数组或引用.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是用于计算平均值的 1 到 255 个数值参数"
			},
			{
				name: "number2",
				description: "是用于计算平均值的 1 到 255 个数值参数"
			}
		]
	},
	{
		name: "度",
		description: "将弧度转换成角度.",
		arguments: [
			{
				name: "angle",
				description: "以弧度表示的角"
			}
		]
	},
	{
		name: "影响",
		description: "返回年有效利率.",
		arguments: [
			{
				name: "nominal_rate",
				description: "是单利"
			},
			{
				name: "npery",
				description: "是每年的复利计算期数"
			}
		]
	},
	{
		name: "执行 ISEVEN",
		description: "如果数字为偶数则返回 TRUE.",
		arguments: [
			{
				name: "number",
				description: "是要测试的值"
			}
		]
	},
	{
		name: "拦截",
		description: "求线性回归拟合线方程的截距.",
		arguments: [
			{
				name: "known_y's",
				description: "因变量数据点"
			},
			{
				name: "known_x's",
				description: "自变量数据点"
			}
		]
	},
	{
		name: "授权服务",
		description: "返回复数的整数幂.",
		arguments: [
			{
				name: "inumber",
				description: "是要进行幂运算的复数"
			},
			{
				name: "number",
				description: "是幂次"
			}
		]
	},
	{
		name: "日志",
		description: "根据给定底数返回数字的对数.",
		arguments: [
			{
				name: "number",
				description: "要取其对数的正实数"
			},
			{
				name: "base",
				description: "计算对数时所使用的底数，如果省略，则以 10 为底"
			}
		]
	},
	{
		name: "日期",
		description: "返回在 Spreadsheet 日期时间代码中代表日期的数字.",
		arguments: [
			{
				name: "year",
				description: "在 Windows Spreadsheet 中介于 1900 到 9999 之间的数字或在 Macintosh Spreadsheet 中介于 1904 到 9999 之间的数字"
			},
			{
				name: "month",
				description: "代表一年中月份的数字，其值在 1 到 12 之间"
			},
			{
				name: "day",
				description: "代表一个月中第几天的数字，其值在 1 到 31 之间"
			}
		]
	},
	{
		name: "最高限额",
		description: "将参数向上舍入为最接近的指定基数的倍数.",
		arguments: [
			{
				name: "number",
				description: "需要进行舍入的参数"
			},
			{
				name: "significance",
				description: "用于向上舍入的基数"
			}
		]
	},
	{
		name: "最高限额。数学",
		description: "将数字向上舍入到最接近的整数或最接近的指定基数的倍数.",
		arguments: [
			{
				name: "number",
				description: " 是要进行舍入的值"
			},
			{
				name: "significance",
				description: " 是要舍入到的倍数"
			},
			{
				name: "mode",
				description: " 当为给定和非零值时，此函数将远离零的方向舍入"
			}
		]
	},
	{
		name: "月",
		description: "返回月份值，是一个 1 (一月)到 12 (十二月)之间的数字。.",
		arguments: [
			{
				name: "serial_number",
				description: "Spreadsheet 进行日期及时间计算的日期-时间代码"
			}
		]
	},
	{
		name: "查找",
		description: "从单行或单列或从数组中查找一个值。条件是向后兼容性.",
		arguments: [
			{
				name: "lookup_value",
				description: "LOOKUP 要在 Lookup_vector 中查找的值，可以是数值、文本、逻辑值，也可以是数值的的名称或引用"
			},
			{
				name: "lookup_vector",
				description: "只包含单行或单列的单元格区域，其值为文本，数值或者逻辑值且以升序排序"
			},
			{
				name: "result_vector",
				description: "只包含单行或单列的单元格区域，与 Lookup_vector 大小相同"
			}
		]
	},
	{
		name: "查找 ",
		description: "返回一个字符串在另一个字符串中出现的起始位置(区分大小写).",
		arguments: [
			{
				name: "find_text",
				description: "要查找的字符串。用双引号(表示空串)可匹配 Within_text 中的第一个字符，不能使用通配符"
			},
			{
				name: "within_text",
				description: "要在其中进行搜索的字符串"
			},
			{
				name: "start_num",
				description: "起始搜索位置，Within_text 中第一个字符的位置为 1。如果忽略，Start_num = 1"
			}
		]
	},
	{
		name: "模式",
		description: "返回一组数据或数据区域中的众数(出现频率最高的数).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是 1 到 255 个数值、名称、数组或对数值的引用"
			},
			{
				name: "number2",
				description: "是 1 到 255 个数值、名称、数组或对数值的引用"
			}
		]
	},
	{
		name: "模式。SNGL",
		description: "返回一组数据或数据区域中出现频率最高或重复出现数值.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是用于模式的 1 到 255 个数值、名称、数组或包含数值的引用"
			},
			{
				name: "number2",
				description: "是用于模式的 1 到 255 个数值、名称、数组或包含数值的引用"
			}
		]
	},
	{
		name: "模式。多",
		description: "返回一组数据或数据区域中出现频率最高或重复出现的数值的垂直数组。对于水平数组，可使用 =TRANSPOSE(MODE.MULT(number1,number2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "是用于模式的 1 到 255 个数值、名称、数组或包含数值的引用"
			},
			{
				name: "number2",
				description: "是用于模式的 1 到 255 个数值、名称、数组或包含数值的引用"
			}
		]
	},
	{
		name: "正态分布",
		description: "返回指定平均值和标准方差的正态累积分布函数值.",
		arguments: [
			{
				name: "x",
				description: "是用于计算正态分布函数的值"
			},
			{
				name: "mean",
				description: "是分布的算术平均"
			},
			{
				name: "standard_dev",
				description: "分布的标准方差，正数"
			},
			{
				name: "cumulative",
				description: "是逻辑值，决定函数的形式。累积分布函数，使用 TRUE；概率密度函数，使用 FALSE"
			}
		]
	},
	{
		name: "比托尔",
		description: "返回两个数字的按位“或”值.",
		arguments: [
			{
				name: "number1",
				description: " 是您要计算的二进制数的十进制表示形式"
			},
			{
				name: "number2",
				description: " 是您要计算的二进制数的十进制表示形式"
			}
		]
	},
	{
		name: "清洁",
		description: "删除文本中的所有非打印字符.",
		arguments: [
			{
				name: "text",
				description: "任何想要从中删除非打印字符的工作表信息"
			}
		]
	},
	{
		name: "现在",
		description: "返回日期时间格式的当前日期和时间。.",
		arguments: [
		]
	},
	{
		name: "甚至",
		description: "将正数向上舍入到最近的偶数，负数向下舍入到最近的偶数.",
		arguments: [
			{
				name: "number",
				description: "需要取偶的数值"
			}
		]
	},
	{
		name: "的信心。规范",
		description: "使用正态分布，返回总体平均值的置信区间.",
		arguments: [
			{
				name: "alpha",
				description: "用来计算置信区间的显著性水平，一个大于 0 小于 1 的数值"
			},
			{
				name: "standard_dev",
				description: "假设为已知的总体标准方差。Standard_dev 必须大于 0"
			},
			{
				name: "size",
				description: "样本容量"
			}
		]
	},
	{
		name: "皮尔逊",
		description: "求皮尔生(Pearson)积矩法的相关系数 r.",
		arguments: [
			{
				name: "array1",
				description: "一组自变量"
			},
			{
				name: "array2",
				description: "一组因变量"
			}
		]
	},
	{
		name: "确切",
		description: "比较两个字符串是否完全相同(区分大小写)。返回 TRUE 或 FALSE.",
		arguments: [
			{
				name: "text1",
				description: "第一个字符串"
			},
			{
				name: "text2",
				description: "第二个字符串"
			}
		]
	},
	{
		name: "米娜",
		description: "返回一组参数中的最小值(不忽略逻辑值和字符串).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "是要求最小值的 1 到 255 个参数，可以是数值、空单元格、逻辑值或文本型数值"
			},
			{
				name: "value2",
				description: "是要求最小值的 1 到 255 个参数，可以是数值、空单元格、逻辑值或文本型数值"
			}
		]
	},
	{
		name: "索引",
		description: "在给定的单元格区域中，返回特定行列交叉处单元格的值或引用.",
		arguments: [
			{
				name: "array",
				description: "单元格区域或数组常量"
			},
			{
				name: "row_num",
				description: "数组或引用中要返回值的行序号。如果忽略，则必须有 Column_num 参数"
			},
			{
				name: "column_num",
				description: "数组或引用中要返回值的列序号。如果忽略，则必须有 Row_num 参数"
			}
		]
	},
	{
		name: "线性",
		description: "返回线性回归方程的参数.",
		arguments: [
			{
				name: "known_y's",
				description: "满足线性拟合直线 y=mx+b 的一组已知的 y 值"
			},
			{
				name: "known_x's",
				description: "满足线性拟合直线 y=mx+b  的一组已知的 x 值，为可选的参数"
			},
			{
				name: "const",
				description: "逻辑值，用以指定是否要强制常数 b 为 0。如果 Const=TRUE 或忽略，b 取正常值；如果 Const=FALSE，b 等于 0"
			},
			{
				name: "stats",
				description: "逻辑值，如果返回附加的回归统计值，返回 TRUE；如果返回系数 m 和常数 b,返回 FALSE"
			}
		]
	},
	{
		name: "组合仪表",
		description: "返回从给定元素数目的集合中提取若干元素的组合数.",
		arguments: [
			{
				name: "number",
				description: "元素总数"
			},
			{
				name: "number_chosen",
				description: "每个组合包含的元素数目"
			}
		]
	},
	{
		name: "结合",
		description: "返回给定数目的项目的组合数(包含重复项).",
		arguments: [
			{
				name: "number",
				description: " 项目总数"
			},
			{
				name: "number_chosen",
				description: " 每个组合包含的项目数目"
			}
		]
	},
	{
		name: "美元",
		description: "用货币格式将数值转换成文本字符.",
		arguments: [
			{
				name: "number",
				description: "一个数值、一个对含有数值的单元格的引用、或一个可导出数值的公式"
			},
			{
				name: "decimals",
				description: "指定小数点右边的位数。如果必要，数字将四舍五入；如果忽略，Decimals= 2"
			}
		]
	},
	{
		name: "虚假",
		description: "返回逻辑值 FALSE.",
		arguments: [
		]
	},
	{
		name: "虚数",
		description: "返回复数的虚部系数.",
		arguments: [
			{
				name: "inumber",
				description: "是求解虚部系数的复数"
			}
		]
	},
	{
		name: "行列式",
		description: "返回一数组所代表的矩阵的逆矩阵.",
		arguments: [
			{
				name: "array",
				description: "行数和列数相等的数值数组，或是单元格区域，或是数组常量"
			}
		]
	},
	{
		name: "规范。DIST",
		description: "返回正态分布函数值.",
		arguments: [
			{
				name: "x",
				description: "用于计算正态分布函数值的区间点"
			},
			{
				name: "mean",
				description: "分布的算术平均"
			},
			{
				name: "standard_dev",
				description: "分布的标准方差，正数"
			},
			{
				name: "cumulative",
				description: "逻辑值，决定函数的形式。累积分布函数，使用 TRUE；概率密度函数，使用 FALSE"
			}
		]
	},
	{
		name: "规范。INV",
		description: "返回具有给定概率正态分布的区间点.",
		arguments: [
			{
				name: "probability",
				description: "正态分布的概率,介于 0 与 1 之间，含 0 与 1"
			},
			{
				name: "mean",
				description: "分布的算术平均"
			},
			{
				name: "standard_dev",
				description: "分布的标准方差，正数"
			}
		]
	},
	{
		name: "规范。S.DIST",
		description: "返回标准正态分布函数值.",
		arguments: [
			{
				name: "z",
				description: "用于计算标准正态分布函数的区间点"
			},
			{
				name: "cumulative",
				description: "逻辑值，当函数为累积分布函数时，返回值为 TRUE；当为概率密度函数时，返回值为 FALSE"
			}
		]
	},
	{
		name: "规范。S.INV",
		description: "返回标准正态分布的区间点.",
		arguments: [
			{
				name: "probability",
				description: "正态分布概率，介于 0 与 1 之间，含 0 与 1"
			}
		]
	},
	{
		name: "计数",
		description: "计算区域中包含数字的单元格的个数.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "是 1 到 255 个参数，可以包含或引用各种不同类型的数据，但只对数字型数据进行计数"
			},
			{
				name: "value2",
				description: "是 1 到 255 个参数，可以包含或引用各种不同类型的数据，但只对数字型数据进行计数"
			}
		]
	},
	{
		name: "费希尔",
		description: "返回 Fisher 变换值.",
		arguments: [
			{
				name: "x",
				description: "需要变换的数值，介于 -1 与 1 之间，不含 -1 与 1"
			}
		]
	},
	{
		name: "超链接",
		description: "创建一个快捷方式或链接，以便打开一个存储在硬盘、网络服务器或  Internet 上的文档.",
		arguments: [
			{
				name: "link_location",
				description: "要打开的文件名称及完整路径。可以是本地硬盘、UNC 路径或 URL 路径"
			},
			{
				name: "friendly_name",
				description: "要显示在单元格中的数字或字符串。如果忽略此参数，单元格中显示 Link_location 的文本"
			}
		]
	},
	{
		name: "转换",
		description: "将数字从一种度量体系转换为另一种度量体系.",
		arguments: [
			{
				name: "number",
				description: "是 from_units 中要转换的值"
			},
			{
				name: "from_unit",
				description: "是数字的单位"
			},
			{
				name: "to_unit",
				description: "是结果的单位"
			}
		]
	},
	{
		name: "选择",
		description: "根据给定的索引值，从参数串中选出相应值或操作.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_num",
				description: "指出所选参数值在参数表中的位置。Index_num 必须是介于 1 到 254 之间的数值，或者是返回值介于 1 到 254 之间的引用或公式"
			},
			{
				name: "value1",
				description: "是 1 到 254 个数值参数、单元格引用、已定义名称、公式、函数，或者是 CHOOSE 从中选定的文本参数"
			},
			{
				name: "value2",
				description: "是 1 到 254 个数值参数、单元格引用、已定义名称、公式、函数，或者是 CHOOSE 从中选定的文本参数"
			}
		]
	},
	{
		name: "错误。类型",
		description: "返回与错误值对应的数字。.",
		arguments: [
			{
				name: "error_val",
				description: "需要辨认其类型的错误值，可为实际错误值或对包含错误值的单元格的引用"
			}
		]
	},
	{
		name: "间接",
		description: "返回文本字符串所指定的引用.",
		arguments: [
			{
				name: "ref_text",
				description: "单元格引用，该引用所指向的单元格中存放有对另一单元格的引用，引用的形式为 A1、 R1C1 或是名称"
			},
			{
				name: "a1",
				description: "逻辑值，用以指明 ref_text 单元格中包含的引用方式。R1C1 格式 = FALSE；A1 格式 = TRUE 或忽略"
			}
		]
	},
	{
		name: "阿拉伯语",
		description: "将罗马数字转换为阿拉伯数字.",
		arguments: [
			{
				name: "text",
				description: " 是要转换的罗马数字"
			}
		]
	},
	{
		name: "预测",
		description: "通过一条线性回归拟合线返回一个预测值.",
		arguments: [
			{
				name: "x",
				description: "需要进行预测的数据点的 x 坐标(自变量值)"
			},
			{
				name: "known_y's",
				description: "从满足线性拟合直线 y=mx+b 的点集中选出的一组已知的 y 值"
			},
			{
				name: "known_x's",
				description: "从满足线性拟合直线 y=mx+b 的点集中选出的一组已知的 x 值"
			}
		]
	},
	{
		name: "频率",
		description: "以一列垂直数组返回一组数据的频率分布.",
		arguments: [
			{
				name: "data_array",
				description: "用来计算频率的数组，或对数组单元区域的引用(空格及字符串忽略)"
			},
			{
				name: "bins_array",
				description: "数据接收区间，为一数组或对数组区域的引用，设定对 data_array 进行频率计算的分段点"
			}
		]
	}
];