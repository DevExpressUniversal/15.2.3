ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Devolve o valor absoluto de um número, um número sem o respetivo sinal.",
		arguments: [
			{
				name: "núm",
				description: "é o número real para o qual deseja obter o valor absoluto"
			}
		]
	},
	{
		name: "ACOS",
		description: "Devolve o arco de cosseno de um número, em radianos, no intervalo de 0 a Pi. O arco de cosseno é o ângulo cujo cosseno é Núm.",
		arguments: [
			{
				name: "núm",
				description: "é o cosseno do ângulo desejado e deve estar entre -1 e 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Devolve o cosseno hiperbólico inverso de um número.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real igual ou maior que 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Devolve o arco tangente de um número em radianos no intervalo 0 a Pi.",
		arguments: [
			{
				name: "número",
				description: "é a cotangente do ângulo que quer obter"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Devolve a cotangente hiperbólica inverso de um número.",
		arguments: [
			{
				name: "número",
				description: "é a cotangente hiperbólica do ângulo que quer obter"
			}
		]
	},
	{
		name: "AGORA",
		description: "Devolve a data e hora atuais com o formato de data e hora.",
		arguments: [
		]
	},
	{
		name: "ALEATÓRIO",
		description: "Devolve um número aleatório maior ou igual a 0 e menor que 1, segundo uma distribuição uniforme (altera ao voltar a calcular).",
		arguments: [
		]
	},
	{
		name: "ALEATÓRIOENTRE",
		description: "Devolve um número aleatório de entre os números especificados.",
		arguments: [
			{
				name: "inferior",
				description: "é o menor inteiro a devolver por ALEATÓRIOENTRE"
			},
			{
				name: "superior",
				description: "é o maior inteiro que ALEATÓRIOENTRE devolverá"
			}
		]
	},
	{
		name: "AMORT",
		description: "Devolve a depreciação linear de um bem durante um período.",
		arguments: [
			{
				name: "custo",
				description: "é o custo inicial do bem"
			},
			{
				name: "val_residual",
				description: "é o valor residual no final da vida útil do bem"
			},
			{
				name: "vida_útil",
				description: "é o número de períodos em que o bem é depreciado (por vezes chamado vida útil do bem)"
			}
		]
	},
	{
		name: "AMORTD",
		description: "Devolve a depreciação por algarismos da soma dos anos de um bem para um período especificado.",
		arguments: [
			{
				name: "custo",
				description: "é o custo inicial do bem"
			},
			{
				name: "val_residual",
				description: "é o valor residual no final da vida útil do bem"
			},
			{
				name: "vida_útil",
				description: "é o número de períodos em que o bem é depreciado (por vezes designado vida útil do bem)"
			},
			{
				name: "período",
				description: "é o período e tem de utilizar a mesma unidade de Vida_útil"
			}
		]
	},
	{
		name: "ANO",
		description: "Devolve o ano de uma data, um número inteiro do intervalo 1900-9999.",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código de data e hora utilizado pelo Spreadsheet"
			}
		]
	},
	{
		name: "ÁRABE",
		description: "Converte um número Romano em Arábico.",
		arguments: [
			{
				name: "texto",
				description: "é o número Romano que deseja converter"
			}
		]
	},
	{
		name: "ÁREAS",
		description: "Devolve o número de áreas numa referência. Uma área é um intervalo de células contíguas ou uma única célula.",
		arguments: [
			{
				name: "referência",
				description: "é uma referência a uma célula ou intervalo de células e pode referir-se a várias áreas"
			}
		]
	},
	{
		name: "ARRED",
		description: "Arredonda um valor para um número de algarismos especificado.",
		arguments: [
			{
				name: "núm",
				description: "é o número que deseja arredondar"
			},
			{
				name: "núm_dígitos",
				description: "é o número de algarismos que deseja obter no arredondamento. Os números negativos arredondam para a esquerda da vírgula decimal; zero arredonda para o número inteiro mais próximo"
			}
		]
	},
	{
		name: "ARRED.DEFEITO",
		description: "Arredonda um número por defeito para o múltiplo significativo mais próximo.",
		arguments: [
			{
				name: "núm",
				description: "é o valor numérico que deseja arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual deseja arredondar. Número e Significância têm de ser ambos positivos ou ambos negativos"
			}
		]
	},
	{
		name: "ARRED.DEFEITO.MAT",
		description: "Arredonda um número por defeito para o número inteiro ou para o múltiplo significativo mais próximo.",
		arguments: [
			{
				name: "número",
				description: "é o valor que quer arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual quer arrendondar"
			},
			{
				name: "modo",
				description: "quando dado e diferente de zero, esta função irá arredondar para zero"
			}
		]
	},
	{
		name: "ARRED.DEFEITO.PRECISO",
		description: "Arredonda um número por defeito para o número inteiro ou para o múltiplo significativo mais próximo.",
		arguments: [
			{
				name: "núm",
				description: "é o valor numérico que deseja arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual deseja fazer o arredondamento"
			}
		]
	},
	{
		name: "ARRED.EXCESSO",
		description: "Arredonda um número por excesso para o múltiplo significativo mais próximo.",
		arguments: [
			{
				name: "núm",
				description: "é o valor que deseja arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual deseja fazer o arredondamento"
			}
		]
	},
	{
		name: "ARRED.EXCESSO.ISO",
		description: "Arredonda um número por excesso para o número inteiro ou para o múltiplo significativo mais próximo.",
		arguments: [
			{
				name: "núm",
				description: "é o valor que pretende arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo opcional para o qual pretende arredondar"
			}
		]
	},
	{
		name: "ARRED.EXCESSO.MAT",
		description: "Arredonda um número por excesso para o número inteiro ou para o múltiplo significativo mais próximo.",
		arguments: [
			{
				name: "número",
				description: "é o valor que quer arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual quer arredondar"
			},
			{
				name: "modo",
				description: "quando dado e diferente de zero, esta função irá arredondar acima de zero"
			}
		]
	},
	{
		name: "ARRED.EXCESSO.PRECISO",
		description: "Arredonda um número por excesso para o número inteiro ou para o múltiplo significativo mais próximos.",
		arguments: [
			{
				name: "núm",
				description: "é o valor que pretende arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual pretende arredondar"
			}
		]
	},
	{
		name: "ARRED.PARA.BAIXO",
		description: "Arredonda um número por defeito, em valor absoluto.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real que deseja arredondar por defeito"
			},
			{
				name: "núm_dígitos",
				description: "é o número de algarismos para o qual deseja fazer o arredondamento. Os valores negativos arredondam para a esquerda da vírgula decimal; zero ou omisso para o número inteiro mais próximo"
			}
		]
	},
	{
		name: "ARRED.PARA.CIMA",
		description: "Arredonda um número por excesso, em valor absoluto.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real que deseja arredondar por excesso"
			},
			{
				name: "núm_dígitos",
				description: "é o número de algarismos que deseja para o arredondamento. Os valores negativos arredondam para a esquerda da vírgula decimal; zero ou omisso para o número inteiro mais próximo"
			}
		]
	},
	{
		name: "ASEN",
		description: "Devolve o arco de seno de um número em radianos, no intervalo de -Pi/2 a Pi/2.",
		arguments: [
			{
				name: "núm",
				description: "é o seno do ângulo desejado e tem de situar-se entre -1 e 1"
			}
		]
	},
	{
		name: "ASENH",
		description: "Devolve o seno hiperbólico inverso de um número.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real igual ou maior que 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Devolve o arco de tangente de um número em radianos, num intervalo de -Pi/2 a Pi/2.",
		arguments: [
			{
				name: "núm",
				description: "é a tangente do ângulo desejado"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Devolve o arco de tangente das coordenadas x e y especificadas, em radianos, de -Pi a Pi, excluindo -Pi.",
		arguments: [
			{
				name: "núm_x",
				description: "é a coordenada-x do ponto"
			},
			{
				name: "núm_y",
				description: "é a coordenada-y do ponto"
			}
		]
	},
	{
		name: "ATANH",
		description: "Devolve a tangente hiperbólica inversa de um número.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real entre -1 e 1, à exceção dos próprios"
			}
		]
	},
	{
		name: "BASE",
		description: "Converte um número numa representação textual com a base dada.",
		arguments: [
			{
				name: "número",
				description: "é o número que quer converter"
			},
			{
				name: "base",
				description: "é a base para a qual quer converter o número"
			},
			{
				name: "comprimento_mín",
				description: "é o comprimento mínimo da cadeia devolvida. Se omitido, não serão adicionados zeros à esquerda"
			}
		]
	},
	{
		name: "BD",
		description: "Devolve a depreciação de um bem num determinado período, utilizando o método de redução fixa do saldo.",
		arguments: [
			{
				name: "custo",
				description: "é o custo inicial do bem"
			},
			{
				name: "val_residual",
				description: "é o valor residual no final da vida útil do bem"
			},
			{
				name: "vida_útil",
				description: "é o número de períodos durante os quais o bem é depreciado (por vezes chamado vida útil do bem)"
			},
			{
				name: "período",
				description: "é o período para o qual deseja calcular a depreciação. A unidade do período tem de ser a mesma de Vida_útil"
			},
			{
				name: "mês",
				description: "é o número de meses do primeiro ano. Se mês for omisso, o valor assumido é 12"
			}
		]
	},
	{
		name: "BDCONTAR",
		description: "Conta as células que contêm números no campo (coluna) de dados da base de dados que cumpre as condições especificadas.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "ou é o rótulo da coluna, entre aspas duplas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDCONTAR.VAL",
		description: "Conta as células não em branco de um campo (coluna) de dados da base de dados que cumpre as condições especificadas.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "ou é o rótulo da coluna, entre aspas duplas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDD",
		description: "Devolve a depreciação de um bem para um determinado período utilizando o método de redução dupla do saldo ou qualquer outro método especificado.",
		arguments: [
			{
				name: "custo",
				description: "é o custo inicial do bem"
			},
			{
				name: "val_residual",
				description: "é o valor residual no final da vida útil do bem"
			},
			{
				name: "vida_útil",
				description: "é o número de períodos durante os quais o bem é depreciado (por vezes chamado vida útil do bem)"
			},
			{
				name: "período",
				description: "é o período para o qual deseja calcular a depreciação. A unidade do período tem de ser a mesma de Vida_útil"
			},
			{
				name: "fator",
				description: "é o índice de redução do saldo. Se Factor for omisso, assume-se que é 2 (método de redução dupla do saldo)"
			}
		]
	},
	{
		name: "BDDESVPAD",
		description: "Calcula o desvio-padrão a partir de uma amostra de entradas selecionadas da base de dados.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "ou é o rótulo da coluna, entre aspas duplas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDDESVPADP",
		description: "Calcula o desvio-padrão com base na população total das entradas selecionadas da base de dados.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "ou é o rótulo da coluna, entre aspas duplas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDMÁX",
		description: "Devolve o maior número de um campo (coluna) de registos da base de dados que cumpre as condições especificadas.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "é ou um rótulo da coluna, entre aspas duplas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDMÉDIA",
		description: "Calcula a média dos valores de uma coluna numa lista ou base de dados que cumprem as condições especificadas.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "ou é o rótulo da coluna, entre plicas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDMÍN",
		description: "Devolve o número mais pequeno de um campo (coluna) de registos da base de dados que cumpre as condições especificadas.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "ou é o rótulo da coluna, entre aspas duplas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDMULTIPL",
		description: "Multiplica os valores de um campo (colunas) de registos da base de dados que cumpre as condições especificadas.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "ou é o rótulo da coluna, entre aspas duplas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDOBTER",
		description: "Extrai de uma base de dados um único registo que cumpre as condições especificadas.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "ou é o rótulo da coluna, entre aspas duplas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDSOMA",
		description: "Adiciona os números de um campo (coluna) de registos da base de dados que cumpre as condições especificadas.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "ou é o rótulo da coluna, entre aspas duplas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDV",
		description: "Devolve a depreciação de um bem em qualquer período dado, incluindo parciais, utilizando o método de redução dupla do saldo ou outro especificado.",
		arguments: [
			{
				name: "custo",
				description: "é o custo inicial do bem"
			},
			{
				name: "val_residual",
				description: "é o valor residual no final da vida útil do bem"
			},
			{
				name: "vida_útil",
				description: "é o número de períodos ao longo dos quais o bem é depreciado (por vezes chamado vida útil do bem)"
			},
			{
				name: "início_período",
				description: "é o período inicial em que deseja calcular a depreciação, na mesma unidade de Vida_útil"
			},
			{
				name: "final_período",
				description: "é o período final em que deseja calcular a depreciação, nas mesmas unidades de Vida_útil"
			},
			{
				name: "fator",
				description: "é a taxa de redução do saldo, 2 (redução dupla do saldo), se omisso"
			},
			{
				name: "sem_mudança",
				description: "mudar para depreciação linear quando a depreciação for maior que o cálculo da redução do saldo = FALSO ou omisso; não mudar = VERDADEIRO"
			}
		]
	},
	{
		name: "BDVAR",
		description: "Calcula a variância a partir de uma amostra de entradas selecionadas da base de dados.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "ou é o rótulo da coluna, entre aspas duplas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDVARP",
		description: "Calcula a variância com base na população total de entradas selecionadas da base de dados.",
		arguments: [
			{
				name: "base_dados",
				description: "é o intervalo de células que constitui a lista ou a base de dados. Uma base de dados é uma lista de dados relacionados entre si"
			},
			{
				name: "campo",
				description: "ou é o rótulo da coluna, entre aspas duplas, ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Devolve a função de Bessel modificada ln(x).",
		arguments: [
			{
				name: "x",
				description: "é o valor em que a função será avaliada"
			},
			{
				name: "n",
				description: "é a ordem da função de Bessel"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Devolve a função de Bessel Jn(x).",
		arguments: [
			{
				name: "x",
				description: "é o valor em que a função será avaliada"
			},
			{
				name: "n",
				description: "é a ordem da função de Bessel"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Devolve a função de Bessel modificada Kn(x).",
		arguments: [
			{
				name: "x",
				description: "é o valor em que a função será avaliada"
			},
			{
				name: "n",
				description: "é a ordem da função"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Devolve a função de Bessel Yn(x).",
		arguments: [
			{
				name: "x",
				description: "é o valor em que a função será avaliada"
			},
			{
				name: "n",
				description: "é a ordem da função"
			}
		]
	},
	{
		name: "BETA.ACUM.INV",
		description: "Devolve o inverso da função de densidade de probabilidade beta cumulativa (DISTBETA).",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição beta"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição e tem de ser maior que 0"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição e tem de ser maior que 0"
			},
			{
				name: "A",
				description: "é um limite inferior opcional para o intervalo de x. Se omisso, A = 0"
			},
			{
				name: "B",
				description: "é um limite superior opcional para o intervalo de x. Se omisso, B = 1"
			}
		]
	},
	{
		name: "BINADEC",
		description: "Converte um número binário em decimal.",
		arguments: [
			{
				name: "núm",
				description: "é o número binário a converter"
			}
		]
	},
	{
		name: "BINAHEX",
		description: "Converte um número binário em hexadecimal.",
		arguments: [
			{
				name: "núm",
				description: "é o número binário a converter"
			},
			{
				name: "casas",
				description: "é o número de carateres a utilizar"
			}
		]
	},
	{
		name: "BINAOCT",
		description: "Converte um número binário em octal.",
		arguments: [
			{
				name: "núm",
				description: "é o número binário a converter"
			},
			{
				name: "casas",
				description: "é o número de carateres a utilizar"
			}
		]
	},
	{
		name: "BIT.E",
		description: "Devolve um 'E' bit-a-bit de dois números.",
		arguments: [
			{
				name: "número1",
				description: "é a representação decimal do número binário que quer calcular"
			},
			{
				name: "número2",
				description: "é a representação decimal do número binário que quer calcular"
			}
		]
	},
	{
		name: "BIT.OU",
		description: "Devolve um 'Ou' bit-a-bit de dois números.",
		arguments: [
			{
				name: "número1",
				description: "é a representação decimal do número binário que quer calcular"
			},
			{
				name: "número2",
				description: "é a representação decimal do número binário que quer calcular"
			}
		]
	},
	{
		name: "BIT.XOU",
		description: "Devolve um 'Ou Exclusivo' bit-a-bit de dois números.",
		arguments: [
			{
				name: "número1",
				description: "é a representação decimal do número binário que quer calcular"
			},
			{
				name: "número2",
				description: "é a representação decimal do número binário que quer calcular"
			}
		]
	},
	{
		name: "BITDESL.DIR",
		description: "Devolve um número deslocado à direita por valor_da_deslocação bits.",
		arguments: [
			{
				name: "número",
				description: "é a representação decimal do número binário que deseja calcular"
			},
			{
				name: "valor_da_deslocação",
				description: "é o número de bits pelos quais quer deslocar o Número à direita"
			}
		]
	},
	{
		name: "BITDESL.ESQ",
		description: "Devolve um número deslocado à esquerda por valor_da_deslocação bits.",
		arguments: [
			{
				name: "número",
				description: "é a representação decimal do número binário que deseja calcular"
			},
			{
				name: "valor_da_deslocação",
				description: "é o número de bits pelos quais quer deslocar o Número à esquerda"
			}
		]
	},
	{
		name: "CARÁCT",
		description: "Devolve o caráter especificado pelo número de código, a partir do conjunto de carateres do computador.",
		arguments: [
			{
				name: "núm",
				description: "é um número entre 1 e 255 que especifica o caráter desejado"
			}
		]
	},
	{
		name: "CÉL",
		description: "Devolve informações sobre formatação, localização ou conteúdo da primeira célula, de acordo com o sentido de leitura da folha numa referência.",
		arguments: [
			{
				name: "tipo_info",
				description: "é um valor de texto que especifica o tipo de informações da célula que pretende."
			},
			{
				name: "referência",
				description: "é a célula acerca da qual pretende obter informações"
			}
		]
	},
	{
		name: "CODIFICAÇÃOURL",
		description: "Devolve uma cadeia com codificação URL.",
		arguments: [
			{
				name: "texto",
				description: "é uma cadeia para ser codificada com URL"
			}
		]
	},
	{
		name: "CÓDIGO",
		description: "Devolve um código numérico para o primeiro caráter de uma cadeia de texto, utilizando o conjunto de carateres utilizado pelo computador.",
		arguments: [
			{
				name: "texto",
				description: "é o texto para o qual deseja obter o código do primeiro caráter"
			}
		]
	},
	{
		name: "COL",
		description: "Devolve o número da coluna de uma referência.",
		arguments: [
			{
				name: "referência",
				description: "é a célula ou intervalo de células contíguas para o qual deseja obter o número da coluna. Se omisso, é utilizada a célula que contém a função COL"
			}
		]
	},
	{
		name: "COLS",
		description: "Devolve o número de colunas de uma matriz ou referência.",
		arguments: [
			{
				name: "matriz",
				description: "é uma matriz, fórmula matricial ou referência a um intervalo de células de que deseja saber o número de colunas"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Devolve o número de combinações para um determinado conjunto de itens.",
		arguments: [
			{
				name: "núm",
				description: "é o número total de itens"
			},
			{
				name: "núm_escolhido",
				description: "é o número de itens em cada combinação"
			}
		]
	},
	{
		name: "COMBIN.R",
		description: "Devolve o número de combinações com repetições para um determinado número de itens.",
		arguments: [
			{
				name: "número",
				description: "é o número total de itens"
			},
			{
				name: "número_escolhido",
				description: "é o número total de itens em cada combinação"
			}
		]
	},
	{
		name: "COMPACTAR",
		description: "Remove todos os espaços de uma cadeia de texto, à exceção de espaços simples entre palavras.",
		arguments: [
			{
				name: "texto",
				description: "é o texto cujos espaços deseja que sejam removidos"
			}
		]
	},
	{
		name: "COMPLEXO",
		description: "Converte coeficientes reais e imaginários num número complexo.",
		arguments: [
			{
				name: "núm_real",
				description: "é o coeficiente real do número complexo"
			},
			{
				name: "i_núm",
				description: "é o coeficiente imaginário do número complexo"
			},
			{
				name: "sufixo",
				description: "é o sufixo para o componente imaginário do número complexo"
			}
		]
	},
	{
		name: "CONCATENAR",
		description: "Junta várias cadeias de texto numa só.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "texto1",
				description: "são de 1 a 255 cadeias de texto a serem agrupadas numa única cadeia de texto e podem ser cadeias de texto, números ou referências de uma só célula"
			},
			{
				name: "texto2",
				description: "são de 1 a 255 cadeias de texto a serem agrupadas numa única cadeia de texto e podem ser cadeias de texto, números ou referências de uma só célula"
			}
		]
	},
	{
		name: "CONTAR",
		description: "Conta o número de células num intervalo que contém números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "são de 1 a 255 argumentos que podem conter ou referir-se a diversos tipos de dados, mas em que só são contados os números"
			},
			{
				name: "valor2",
				description: "são de 1 a 255 argumentos que podem conter ou referir-se a diversos tipos de dados, mas em que só são contados os números"
			}
		]
	},
	{
		name: "CONTAR.SE",
		description: "Conta o número de células de um intervalo que respeitam uma dada condição.",
		arguments: [
			{
				name: "intervalo",
				description: "é o intervalo de células no qual deseja contar células não em branco"
			},
			{
				name: "critérios",
				description: "é a condição, sob a forma de um número, expressão ou texto, que define quais as células que serão contadas"
			}
		]
	},
	{
		name: "CONTAR.SE.S",
		description: "Conta o número de células especificado por um determinado conjunto de condições ou critérios.",
		arguments: [
			{
				name: "intervalo_critérios",
				description: "é o intervalo de células que pretende avaliar para a condição específica"
			},
			{
				name: "critérios",
				description: "é a condição, sob a forma de um número, expressão ou texto, que define quais as células que serão contadas"
			}
		]
	},
	{
		name: "CONTAR.VAL",
		description: "Conta o número de células não em branco num intervalo.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "são de 1 a 255 argumentos que representam os valores e células que pretende contar. Os valores podem conter qualquer tipo de informação"
			},
			{
				name: "valor2",
				description: "são de 1 a 255 argumentos que representam os valores e células que pretende contar. Os valores podem conter qualquer tipo de informação"
			}
		]
	},
	{
		name: "CONTAR.VAZIO",
		description: "Conta as células em branco dentro de um intervalo de células especificado.",
		arguments: [
			{
				name: "intervalo",
				description: "é o intervalo no qual deseja contar as células em branco"
			}
		]
	},
	{
		name: "CONVERTER",
		description: "Converte um número de um sistema de medida para outro.",
		arguments: [
			{
				name: "núm",
				description: "é o valor de de_unidades a converter"
			},
			{
				name: "de_unidade",
				description: "é a unidade do número"
			},
			{
				name: "para_unidade",
				description: "é a unidade do resultado"
			}
		]
	},
	{
		name: "CORREL",
		description: "Devolve o coeficiente de correlação entre dois conjuntos de dados.",
		arguments: [
			{
				name: "matriz1",
				description: "é um intervalo de células de valores. Os valores podem ser números, nomes, matrizes ou referências que contenham números"
			},
			{
				name: "matriz2",
				description: "é o segundo intervalo de células de valores. Os valores devem ser números, nomes, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "CORRESP",
		description: "Devolve a posição relativa de um item numa matriz que corresponde a um valor especificado por uma ordem especificada.",
		arguments: [
			{
				name: "valor_proc",
				description: "é o valor utilizado para encontrar o valor que pretende numa matriz, um número, texto ou valor lógico ou uma referência a um destes"
			},
			{
				name: "matriz_proc",
				description: "é um intervalo contíguo de células contendo valores possíveis de procura, uma matriz de valores ou uma referência a uma matriz"
			},
			{
				name: "tipo_corresp",
				description: "é um número 1, 0 ou -1 que indica o valor a devolver."
			}
		]
	},
	{
		name: "COS",
		description: "Devolve o cosseno de um ângulo.",
		arguments: [
			{
				name: "núm",
				description: "é o ângulo em radianos para o qual deseja obter o cosseno"
			}
		]
	},
	{
		name: "COSH",
		description: "Devolve o cosseno hiperbólico de um número.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real"
			}
		]
	},
	{
		name: "COT",
		description: "Devolve a cotangente de um ângulo.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual quer obter a cotangente"
			}
		]
	},
	{
		name: "COTH",
		description: "Devolve a cotangente hiperbólica de um número.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual quer obter a cotangente hiperbólica"
			}
		]
	},
	{
		name: "COVAR",
		description: "Devolve a covariância, a média dos produtos de desvios para cada par de ponto de dados em dois conjuntos de dados.",
		arguments: [
			{
				name: "matriz1",
				description: "é o primeiro intervalo de células de números inteiros e têm de ser números, matrizes ou referências que contenham números"
			},
			{
				name: "matriz2",
				description: "é o segundo intervalo de célula de números inteiros e têm de ser números, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "COVARIÂNCIA.P",
		description: "Devolve a covariância da população, a média dos produtos dos desvios para cada par de pontos de dados em dois conjuntos de dados.",
		arguments: [
			{
				name: "matriz1",
				description: "é o primeiro intervalo de células de números inteiros e têm de ser números, matrizes ou referências que contenham números"
			},
			{
				name: "matriz2",
				description: "é o segundo intervalo de células de números inteiros e têm de ser números, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "COVARIÂNCIA.S",
		description: "Devolve a covariância da amostra, a média dos produtos dos desvios para cada par de pontos de dados em dois conjuntos de dados.",
		arguments: [
			{
				name: "matriz1",
				description: "é o primeiro intervalo de células de números inteiros e têm de ser números, matrizes ou referências que contenham números"
			},
			{
				name: "matriz2",
				description: "é o segundo intervalo de células de números inteiros e têm de ser números, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "CRESCIMENTO",
		description: "Devolve números com base na tendência de crescimento exponencial de acordo com os dados existentes conhecidos.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é o conjunto de valores de y já conhecidos na relação y = b*m^x, uma matriz ou intervalo de números positivos"
			},
			{
				name: "val_conhecidos_x",
				description: "é um conjunto opcional de valores de x que já deve ser conhecido na relação y = b*m^x, uma matriz ou intervalo do mesmo tamanho de val_conhecidos_y"
			},
			{
				name: "novos_valores_x",
				description: "são novos valores de x para os quais deseja que CRESCIMENTO devolva os valores de y correspondentes"
			},
			{
				name: "constante",
				description: "é um valor lógico: a constante b é calculada normalmente se Constante = VERDADEIRO; b é definido como igual a 1, se Constante = FALSO ou omisso"
			}
		]
	},
	{
		name: "CRIT.BINOM",
		description: "Devolve o menor valor para o qual a distribuição binomial cumulativa é maior ou igual a um valor de critério.",
		arguments: [
			{
				name: "tentativas",
				description: "é o número de tentativas de Bernoulli"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso em cada tentativa, um número entre 0 e 1 inclusive"
			},
			{
				name: "alfa",
				description: "é o valor do critério, um número entre 0 e 1 inclusive"
			}
		]
	},
	{
		name: "CSC",
		description: "Devolve a cossecante de um ângulo.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual quer obter a cossecante"
			}
		]
	},
	{
		name: "CSCH",
		description: "Devolve a cossecante hiperbólica de um ângulo.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual quer obter a cossecante hiperbólica"
			}
		]
	},
	{
		name: "CUPDATAANT",
		description: "Devolve a última data do cupão antes da data de liquidação.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número de série de data"
			},
			{
				name: "frequência",
				description: "é o número de pagamentos de cupão por ano"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a utilizar"
			}
		]
	},
	{
		name: "CUPDATAPRÓX",
		description: "Devolve a data seguinte do cupão depois da data de liquidação.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número de série de data"
			},
			{
				name: "frequência",
				description: "é o número de pagamentos de cupão por ano"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a utilizar"
			}
		]
	},
	{
		name: "CUPDIASINLIQ",
		description: "Devolve o número de dias entre o início do período do cupão e a data de liquidação.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número de série de data"
			},
			{
				name: "frequência",
				description: "é o número de pagamentos de cupão por ano"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a utilizar"
			}
		]
	},
	{
		name: "CUPNÚM",
		description: "Devolve o número de cupões a pagar entre a data de liquidação e a data do vencimento.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número de série de data"
			},
			{
				name: "frequência",
				description: "é o número de pagamentos de cupão por ano"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a utilizar"
			}
		]
	},
	{
		name: "CURT",
		description: "Devolve a curtose de um conjunto de dados.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências contendo números cuja curtose deseja obter"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências contendo números cuja curtose deseja obter"
			}
		]
	},
	{
		name: "DATA",
		description: "Devolve o número que representa a data, no código de data e hora do Spreadsheet.",
		arguments: [
			{
				name: "ano",
				description: "é um número entre 1900 e 9999 no Spreadsheet para Windows ou entre 1904 e 9999 no Spreadsheet para Macintosh"
			},
			{
				name: "mês",
				description: "é um número de 1 a 12 que representa o mês do ano"
			},
			{
				name: "dia",
				description: "é um número de 1 a 31 que representa o dia do mês"
			}
		]
	},
	{
		name: "DATA.VALOR",
		description: "Converte uma data em forma de texto para um número que representa a data no código de data e hora do Spreadsheet.",
		arguments: [
			{
				name: "texto_data",
				description: "é o texto que representa uma data num formato de datas do Spreadsheet, entre 1/1/1900 (Windows), ou 1/1/1904 (Macintosh), e 31/12/9999"
			}
		]
	},
	{
		name: "DATADIF",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATAM",
		description: "Devolve o número de série da data que é o número indicador dos meses antes ou depois da data inicial.",
		arguments: [
			{
				name: "data_inicial",
				description: "é o número de série de data que representa a data inicial"
			},
			{
				name: "meses",
				description: "é o número de meses antes ou depois de data_inicial"
			}
		]
	},
	{
		name: "DECABIN",
		description: "Converte um número decimal em binário.",
		arguments: [
			{
				name: "núm",
				description: "é o decimal inteiro a converter"
			},
			{
				name: "casas",
				description: "é o número de carateres a utilizar"
			}
		]
	},
	{
		name: "DECAHEX",
		description: "Converte um número decimal em hexadecimal.",
		arguments: [
			{
				name: "núm",
				description: "é o decimal inteiro a converter"
			},
			{
				name: "casas",
				description: "é o número de carateres a utilizar"
			}
		]
	},
	{
		name: "DECAOCT",
		description: "Converte um número decimal em octal.",
		arguments: [
			{
				name: "núm",
				description: "é o decimal inteiro a converter"
			},
			{
				name: "casas",
				description: "é o número de carateres a utilizar"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Converte uma representação textual de um número numa determinada base em um número decimal.",
		arguments: [
			{
				name: "número",
				description: "é o número que quer converter"
			},
			{
				name: "base",
				description: "é a base do número que vai a converter"
			}
		]
	},
	{
		name: "DECLIVE",
		description: "Devolve o declive da reta de regressão linear através dos pontos dados.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é uma matriz ou intervalo de célula de pontos de dados dependentes e numéricos e podem ser números ou nomes, matrizes ou referências que contenham números"
			},
			{
				name: "val_conhecidos_x",
				description: "é o conjunto de pontos de dados independentes e podem ser números ou nomes, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "DEGRAU",
		description: "Testa se um número é maior do que um valor limite.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a testar passo a passo"
			},
			{
				name: "passo",
				description: "é o valor limite"
			}
		]
	},
	{
		name: "DELTA",
		description: "Testa se dois números são iguais.",
		arguments: [
			{
				name: "núm1",
				description: "é o primeiro número"
			},
			{
				name: "núm2",
				description: "é o segundo número"
			}
		]
	},
	{
		name: "DESC",
		description: "Devolve a taxa de desconto de um título.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número de série de data"
			},
			{
				name: "pr",
				description: "é o preço do título por cada 100 € do valor nominal"
			},
			{
				name: "resgate",
				description: "é o valor de resgate do título por cada 100 € do valor nominal"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a utilizar"
			}
		]
	},
	{
		name: "DESLOCAMENTO",
		description: "Devolve, a partir de uma célula ou intervalo de células, uma referência a um intervalo.",
		arguments: [
			{
				name: "referência",
				description: "é a referência em que deseja basear o deslocamento, uma referência a uma célula ou intervalo de células adjacentes"
			},
			{
				name: "linhas",
				description: "é o número de linhas, para cima ou para baixo, a que deseja que a célula superior esquerda do resultado se refira"
			},
			{
				name: "colunas",
				description: "é o número de colunas, à esquerda ou à direita, a que deseja que a célula superior esquerda do resultado se refira"
			},
			{
				name: "altura",
				description: "é a altura, em número de linhas, que deseja que a referência fornecida apresente; se omissa, é a mesma altura de Referência"
			},
			{
				name: "largura",
				description: "é a largura, em número de colunas, que deseja como resultado; se omissa, é a mesma largura de Referência"
			}
		]
	},
	{
		name: "DESV.MÉDIO",
		description: "Devolve a média aritmética dos desvios absolutos à média dos pontos de dados. Os argumentos são números ou nomes, matrizes ou referências que contêm números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 argumentos de cujos desvios absolutos deseja obter a média"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 argumentos de cujos desvios absolutos deseja obter a média"
			}
		]
	},
	{
		name: "DESVPAD",
		description: "Calcula o desvio-padrão a partir de uma amostra (ignora valores lógicos e texto na amostra).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números correspondentes a uma amostra da população e podem ser números ou referências que contêm números"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números correspondentes a uma amostra da população e podem ser números ou referências que contêm números"
			}
		]
	},
	{
		name: "DESVPAD.P",
		description: "Calcula o desvio-padrão com base na população total fornecida como argumento (ignora valores lógicos e texto).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números correspondentes a uma população e podem ser números ou referências que contêm números"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números correspondentes a uma população e podem ser números ou referências que contêm números"
			}
		]
	},
	{
		name: "DESVPAD.S",
		description: "Calcula o desvio-padrão a partir de uma amostra (ignora valores lógicos e texto na amostra).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números correspondentes a uma amostra da população e podem ser números ou referências que contêm números"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números correspondentes a uma amostra da população e podem ser números ou referências que contêm números"
			}
		]
	},
	{
		name: "DESVPADA",
		description: "Calcula o desvio-padrão a partir de uma amostra, incluindo valores lógicos e texto. Texto e FALSO têm valor 0; VERDADEIRO tem o valor 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "são de 1 a 255 argumentos de valores correspondentes a uma amostra de uma população e podem ser valores ou nomes ou referências a valores"
			},
			{
				name: "valor2",
				description: "são de 1 a 255 argumentos de valores correspondentes a uma amostra de uma população e podem ser valores ou nomes ou referências a valores"
			}
		]
	},
	{
		name: "DESVPADP",
		description: "Calcula o desvio-padrão com base na população total fornecida como argumento (ignora valores lógicos e texto).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números correspondentes a uma população e podem ser números ou referências que contêm números"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números correspondentes a uma população e podem ser números ou referências que contêm números"
			}
		]
	},
	{
		name: "DESVPADPA",
		description: "Calcula o desvio-padrão com base na população total, incluindo valores lógicos e texto. Texto e FALSO têm valor 0; VERDADEIRO tem o valor 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "são de 1 a 255 valores correspondentes a uma população e podem ser valores, nomes, matrizes ou referências que contêm valores"
			},
			{
				name: "valor2",
				description: "são de 1 a 255 valores correspondentes a uma população e podem ser valores, nomes, matrizes ou referências que contêm valores"
			}
		]
	},
	{
		name: "DESVQ",
		description: "Devolve a soma dos quadrados dos desvios de pontos de dados em relação à média da sua amostra.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 argumentos ou uma matriz ou uma referência de matriz, para os quais pretende calcular o DESVQ"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 argumentos ou uma matriz ou uma referência de matriz, para os quais pretende calcular o DESVQ"
			}
		]
	},
	{
		name: "DEVOLVERTAXAJUROS",
		description: "Devolve uma taxa de juro equivalente para o crescimento de um investimento.",
		arguments: [
			{
				name: "nper",
				description: "é o número de períodos para o investimento"
			},
			{
				name: "va",
				description: "é o valor atual do investimento"
			},
			{
				name: "vf",
				description: "é o valor futuro do investimento"
			}
		]
	},
	{
		name: "DIA",
		description: "Devolve o dia do mês, um número entre 1 e 31.",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código de data e hora utilizado pelo Spreadsheet"
			}
		]
	},
	{
		name: "DIA.SEMANA",
		description: "Devolve um número de 1 a 7 identificando o dia da semana.",
		arguments: [
			{
				name: "núm_série",
				description: "é um número que representa uma data"
			},
			{
				name: "tipo_devolvido",
				description: "é um número: para Domingo=1 a Sábado=7, utilize 1; para Segunda=1 a Domingo=7, utilize 2; para Segunda=0 a Domingo=6, utilize 3"
			}
		]
	},
	{
		name: "DIAS",
		description: "Devolve o número de dias entre duas datas.",
		arguments: [
			{
				name: "data_de_fim",
				description: "data_de_início e data_de_final são as duas datas entre as quais quer saber o número de dias"
			},
			{
				name: "data_de_início",
				description: "data_de_início e data_de_fim são as duas datas entre as quais quer saber o número de dias"
			}
		]
	},
	{
		name: "DIAS360",
		description: "Devolve o número de dias decorridos entre duas datas, com base num ano de 360 dias (doze meses de 30 dias).",
		arguments: [
			{
				name: "data_inicial",
				description: "data_início e data_fim são as duas datas cujo intervalo em dias deseja saber"
			},
			{
				name: "data_final",
				description: "Data_início e data_fim são as duas datas cujo intervalo em dias deseja saber"
			},
			{
				name: "método",
				description: "é um valor lógico que especifica o método de cálculo: E.U.A. (NASD) = FALSE ou omisso; europeu = TRUE."
			}
		]
	},
	{
		name: "DIATRABALHO",
		description: "Devolve o número de série da data antes ou depois de um dado número de dias úteis.",
		arguments: [
			{
				name: "data_inicial",
				description: "é o número de série de data que representa a data inicial"
			},
			{
				name: "dias",
				description: "é o número de dias úteis antes ou depois de data_inicial"
			},
			{
				name: "feriados",
				description: "é uma matriz opcional de um ou mais números de série de datas a eliminar do calendário de trabalho, como feriados municipais, nacionais e móveis"
			}
		]
	},
	{
		name: "DIATRABALHO.INTL",
		description: "Devolve o número de série da data anterior ou posterior a um número específico de dias úteis com parâmetros de fim de semana personalizados.",
		arguments: [
			{
				name: "data_inicial",
				description: "é um número de série de data que representa a data inicial"
			},
			{
				name: "dias",
				description: "é o número de dias que não correspondem a fins de semana ou feriados antes ou depois da data_inicial"
			},
			{
				name: "fim_de_semana",
				description: "é um número ou cadeia que especifica quando ocorrem os fins de semana"
			},
			{
				name: "feriados",
				description: "é uma matriz opcional de um ou mais números de série de data a excluir do calendário de trabalho, como feriados municipais, nacionais e móveis"
			}
		]
	},
	{
		name: "DIATRABALHOTOTAL",
		description: "Devolve o número de dias úteis entre duas datas.",
		arguments: [
			{
				name: "data_inicial",
				description: "é o número de série de data que representa a data inicial"
			},
			{
				name: "data_final",
				description: "é o número de série de data que representa a data final"
			},
			{
				name: "feriados",
				description: "é uma matriz opcional de um ou mais números de série de datas a eliminar do calendário de trabalho, como feriados municipais, nacionais e móveis"
			}
		]
	},
	{
		name: "DIATRABALHOTOTAL.INTL",
		description: "Devolve o número de dias úteis completos entre duas datas com parâmetros de fim de semana personalizados.",
		arguments: [
			{
				name: "data_inicial",
				description: "é um número de série de data que representa a data inicial"
			},
			{
				name: "data_final",
				description: "é um número de série de data que representa a data final"
			},
			{
				name: "fim_de_semana",
				description: "é um número ou cadeia que especifica quando ocorrem os fins de semana"
			},
			{
				name: "feriados",
				description: "é um conjunto opcional de um ou mais números de série de data a excluir do calendário de trabalho, como feriados municipais, nacionais e móveis"
			}
		]
	},
	{
		name: "DIREITA",
		description: "Devolve o número especificado de carateres do fim de uma cadeia de texto.",
		arguments: [
			{
				name: "texto",
				description: "é a cadeia de texto que contém os carateres que deseja extrair"
			},
			{
				name: "núm_caract",
				description: "especifica o número de carateres que deseja extrair, utilizando 1 se for omisso"
			}
		]
	},
	{
		name: "DIST.BETA",
		description: "Devolve a função de distribuição de probabilidade beta.",
		arguments: [
			{
				name: "x",
				description: "é o valor entre A e B sobre o qual a função deve ser avaliada"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição e tem de ser maior que 0"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição e tem de ser maior que 0"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade, utilize FALSO"
			},
			{
				name: "A",
				description: "é um limite inferior opcional para o intervalo de x. Se omisso, A = 0"
			},
			{
				name: "B",
				description: "é um limite superior opcional para o intervalo de x. Se omisso, B = 1"
			}
		]
	},
	{
		name: "DIST.BIN.NEG",
		description: "Devolve a distribuição binomial negativa, a probabilidade de Núm_i insucessos antes de Núm_s sucessos, com Probabilidade_s probabilidades de um sucesso.",
		arguments: [
			{
				name: "núm_i",
				description: "é o número de insucessos"
			},
			{
				name: "núm_s",
				description: "é o número a partir do qual se considera haver sucesso"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de um sucesso; um número entre 0 e 1"
			}
		]
	},
	{
		name: "DIST.BINOM.INTERVALO",
		description: "Devolve a probabilidade de um resultado tentado utilizando uma distribuição binomial.",
		arguments: [
			{
				name: "tentativas",
				description: "é o número de tentativas independentes"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso de cada tentativa"
			},
			{
				name: "número_s",
				description: "é o número de sucessos nas tentativas"
			},
			{
				name: "número_s2",
				description: "se fornecido, esta função devolve a probabilidade de o número de tentativas com sucesso estar entre número_s e número_s2"
			}
		]
	},
	{
		name: "DIST.BINOM.NEG",
		description: "Devolve a distribuição binomial negativa, a probabilidade de Núm_i insucessos antes do Núm_s.º sucesso, com Probabilidade_s de sucesso.",
		arguments: [
			{
				name: "núm_i",
				description: "é o número de falhas"
			},
			{
				name: "núm_s",
				description: "é o número limite de sucessos"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso; um número entre 0 e 1"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade, utilize FALSO"
			}
		]
	},
	{
		name: "DIST.CHI",
		description: "Devolve a probabilidade unilateral à direita da distribuição chi-quadrado.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual pretende avaliar a distribuição, um número não negativo"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "DIST.CHIQ",
		description: "Devolve a probabilidade unilateral à esquerda da distribuição chi-quadrado.",
		arguments: [
			{
				name: "x",
				description: "é o valor sobre o qual pretende avaliar a distribuição, um número não negativo"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico que a função deve devolver: a função de distribuição cumulativa = VERDADEIRO; a função de densidade de probabilidade = FALSO"
			}
		]
	},
	{
		name: "DIST.CHIQ.DIR",
		description: "Devolve a probabilidade unilateral à direita da distribuição chi-quadrado.",
		arguments: [
			{
				name: "x",
				description: "é o valor sobre o qual pretende avaliar a distribuição, um número não negativo"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "DIST.EXPON",
		description: "Devolve a distribuição exponencial.",
		arguments: [
			{
				name: "x",
				description: "é o valor da função, um número não negativo"
			},
			{
				name: "lambda",
				description: "é o valor do parâmetro, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico que a função deve devolver: a função de distribuição cumulativa = VERDADEIRO; a função de densidade da probabilidade = FALSO"
			}
		]
	},
	{
		name: "DIST.F",
		description: "Devolve a distribuição (unilateral à esquerda) de probabilidade F (grau de diversidade) de dois conjuntos de dados.",
		arguments: [
			{
				name: "x",
				description: "é o valor sobre o qual pretende avaliar a função, um número não negativo"
			},
			{
				name: "grau_liberdade1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "grau_liberdade2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico que a função deve devolver: a função de distribuição cumulativa = VERDADEIRO; a função de densidade de probabilidade = FALSO"
			}
		]
	},
	{
		name: "DIST.F.DIR",
		description: "Devolve a distribuição (unilateral à direita) de probabilidade F (grau de diversidade) de dois conjuntos de dados.",
		arguments: [
			{
				name: "x",
				description: "é o valor sobre o qual pretende avaliar a função, um número não negativo"
			},
			{
				name: "grau_liberdade1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "grau_liberdade2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "DIST.GAMA",
		description: "Devolve a distribuição gama.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual pretende avaliar a distribuição, um número não negativo"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição, um número positivo. Se beta = 1; DIST.GAMA devolve a distribuição gama padrão"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: devolver a função de distribuição cumulativa = VERDADEIRO; devolver a função de densidade de probabilidade = FALSO ou omisso"
			}
		]
	},
	{
		name: "DIST.HIPERGEOM",
		description: "Devolve a distribuição hipergeométrica.",
		arguments: [
			{
				name: "exemplo_s",
				description: "é o número de sucessos na amostra"
			},
			{
				name: "exemplo_núm",
				description: "é o tamanho da amostra"
			},
			{
				name: "população_s",
				description: "é o número de sucessos na população"
			},
			{
				name: "núm_população",
				description: "é o tamanho da população"
			}
		]
	},
	{
		name: "DIST.HIPGEOM",
		description: "Devolve a distribuição hipergeométrica.",
		arguments: [
			{
				name: "tam_amostra",
				description: "é o número de sucessos na amostra"
			},
			{
				name: "número_amostra",
				description: "é o tamanho da amostra"
			},
			{
				name: "tam_população",
				description: "é o número de sucessos na população"
			},
			{
				name: "número_pop",
				description: "é o tamanho da população"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade, utilize FALSO"
			}
		]
	},
	{
		name: "DIST.NORM",
		description: "Devolve a distribuição cumulativa normal para a média e o desvio-padrão especificados.",
		arguments: [
			{
				name: "x",
				description: "é o valor para o qual pretende obter a distribuição"
			},
			{
				name: "média",
				description: "é a média aritmética da distribuição"
			},
			{
				name: "desv_padrão",
				description: "é o desvio-padrão da distribuição, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade, utilize FALSO"
			}
		]
	},
	{
		name: "DIST.NORMAL",
		description: "Devolve a distribuição cumulativa normal para a média e o desvio-padrão especificados.",
		arguments: [
			{
				name: "x",
				description: "é o valor para o qual pretende obter a distribuição"
			},
			{
				name: "média",
				description: "é a média aritmética da distribuição"
			},
			{
				name: "desv_padrão",
				description: "é o desvio-padrão da distribuição, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade, utilize FALSO"
			}
		]
	},
	{
		name: "DIST.NORMALLOG",
		description: "Devolve a distribuição normal logarítmica cumulativa de x, em que ln(x) tem uma distribuição normal com os parâmetros Média e Desv_padrão.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se avalia a função, um número positivo"
			},
			{
				name: "média",
				description: "é a média de ln(x)"
			},
			{
				name: "desv_padrão",
				description: "é o desvio-padrão de ln(x), um número positivo"
			}
		]
	},
	{
		name: "DIST.NORMLOG",
		description: "Devolve a distribuição normal logarítmica de x, em que ln(x) tem uma distribuição normal com os parâmetros Média e Desv_Padrão.",
		arguments: [
			{
				name: "x",
				description: "é o valor sobre o qual a função deve ser avaliada, um número positivo"
			},
			{
				name: "média",
				description: "é a média de ln(x)"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão de ln(x), um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade, utilize FALSO"
			}
		]
	},
	{
		name: "DIST.NORMP",
		description: "Devolve a distribuição cumulativa normal padrão (tem uma média de 0 e um desvio-padrão de 1).",
		arguments: [
			{
				name: "z",
				description: "é o valor para o qual pretende obter a distribuição"
			}
		]
	},
	{
		name: "DIST.POISSON",
		description: "Devolve a distribuição de Poisson.",
		arguments: [
			{
				name: "x",
				description: "é o número de eventos"
			},
			{
				name: "média",
				description: "é o valor numérico esperado, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a probabilidade de Poisson cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade de Poisson, utilize FALSO"
			}
		]
	},
	{
		name: "DIST.S.NORM",
		description: "Devolve a distribuição normal padrão (tem uma média de zero e um desvio padrão de um).",
		arguments: [
			{
				name: "z",
				description: "é o valor para o qual pretende obter a distribuição"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico que a função deve devolver: a função de distribuição cumulativa = VERDADEIRO; a função de densidade de probabilidade = FALSO"
			}
		]
	},
	{
		name: "DIST.T",
		description: "Devolve a distribuição t de Student unilateral à esquerda.",
		arguments: [
			{
				name: "x",
				description: "é o valor numérico sobre o qual a distribuição deve ser avaliada"
			},
			{
				name: "graus_liberdade",
				description: "é um número inteiro que indica o número de graus de liberdade que caracteriza a distribuição"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade, utilize FALSO"
			}
		]
	},
	{
		name: "DIST.T.2C",
		description: "Devolve a distribuição t de Student bicaudal.",
		arguments: [
			{
				name: "x",
				description: "é o valor numérico sobre o qual a distribuição deve ser avaliada"
			},
			{
				name: "graus_liberdade",
				description: "é um número inteiro que indica o número de graus de liberdade que caracteriza a distribuição"
			}
		]
	},
	{
		name: "DIST.T.DIR",
		description: "Devolve a distribuição t de Student unilateral à direita.",
		arguments: [
			{
				name: "x",
				description: "é o valor numérico sobre o qual a distribuição deve ser avaliada"
			},
			{
				name: "graus_liberdade",
				description: "é um número inteiro que indica o número de graus de liberdade que caracteriza a distribuição"
			}
		]
	},
	{
		name: "DIST.WEIBULL",
		description: "Devolve a distribuição Weibull.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se avalia a função, um número não negativo"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade, utilize FALSO"
			}
		]
	},
	{
		name: "DISTBETA",
		description: "Devolve a função de densidade de probabilidade beta cumulativa.",
		arguments: [
			{
				name: "x",
				description: "é o valor entre A e B, no qual se avalia a função"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição e tem de ser maior que 0."
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição e tem de ser maior que 0"
			},
			{
				name: "A",
				description: "é um limite inferior opcional para o intervalo de x. Se omisso, A = 0"
			},
			{
				name: "B",
				description: "é um limite superior opcional para o intervalo de x. Se omisso, B = 1"
			}
		]
	},
	{
		name: "DISTEXPON",
		description: "Devolve a distribuição exponencial.",
		arguments: [
			{
				name: "x",
				description: "é o valor da função, um número não negativo"
			},
			{
				name: "lambda",
				description: "é o valor do parâmetro, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico que a função deve devolver: a função de distribuição cumulativa = VERDADEIRO; a função de densidade da probabilidade = FALSO"
			}
		]
	},
	{
		name: "DISTF",
		description: "Devolve a distribuição (unilateral à direita) de probabilidade F (grau de diversidade) para dois conjuntos de dados.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se avalia a função, um número não negativo"
			},
			{
				name: "graus_liberdade1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "graus_liberdade2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "DISTGAMA",
		description: "Devolve a distribuição gama.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual pretende avaliar a distribuição, um número não negativo"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição, um número positivo. Se beta = 1, DISTGAMA devolve a distribuição gama padrão"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: devolver a função de distribuição cumulativa = VERDADEIRO; devolver a função de densidade de probabilidade = FALSO ou omisso"
			}
		]
	},
	{
		name: "DISTORÇÃO",
		description: "Devolve a distorção de uma distribuição: uma caracterização do grau de assimetria da distribuição em torno da média.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências contendo números cuja distorção deseja obter"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências contendo números cuja distorção deseja obter"
			}
		]
	},
	{
		name: "DISTORÇÃO.P",
		description: "Devolve a distorção de uma distribuição baseada numa população: uma caracterização do grau de assimetria de uma distribuição em torno da média.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "são de 1 a 254 números ou nomes, matrizes ou referências contendo números cuja distorção quer obter"
			},
			{
				name: "número2",
				description: "são de 1 a 254 números ou nomes, matrizes ou referências contendo números cuja distorção quer obter"
			}
		]
	},
	{
		name: "DISTR.BINOM",
		description: "Devolve a probabilidade da distribuição binomial do termo individual.",
		arguments: [
			{
				name: "núm_s",
				description: "é o número de tentativas bem sucedidas"
			},
			{
				name: "tentativas",
				description: "é o número de tentativas independentes"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso em cada tentativa"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade, utilize FALSO"
			}
		]
	},
	{
		name: "DISTRBINOM",
		description: "Devolve a probabilidade da distribuição binomial do termo individual.",
		arguments: [
			{
				name: "núm_s",
				description: "é o número de tentativas bem sucedidas"
			},
			{
				name: "tentativas",
				description: "é o número de tentativas independentes"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso em cada tentativa"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade, utilize FALSO"
			}
		]
	},
	{
		name: "DISTT",
		description: "Devolve a distribuição t de Student.",
		arguments: [
			{
				name: "x",
				description: "é o valor numérico em que se avalia a distribuição"
			},
			{
				name: "graus_liberdade",
				description: "é um número inteiro que indica o número de graus de liberdade que caracteriza a distribuição"
			},
			{
				name: "caudas",
				description: "especifica o número de caudas de distribuição a ser devolvido: distribuição unicaudal = 1; distribuição bicaudal = 2"
			}
		]
	},
	{
		name: "E",
		description: "Devolve VERDADEIRO, se todos os argumentos forem VERDADEIRO;.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor_lógico1",
				description: "são de 1 a 255 condições a ser testadas, que podem ser VERDADEIRO ou FALSO e que podem ser valores lógicos, matrizes ou referências"
			},
			{
				name: "valor_lógico2",
				description: "são de 1 a 255 condições a ser testadas, que podem ser VERDADEIRO ou FALSO e que podem ser valores lógicos, matrizes ou referências"
			}
		]
	},
	{
		name: "É.CÉL.VAZIA",
		description: "Devolve VERDADEIRO ou FALSO se valor se referir a uma célula vazia.",
		arguments: [
			{
				name: "valor",
				description: "é a célula ou um nome que se refere à célula que deseja testar"
			}
		]
	},
	{
		name: "É.ERRO",
		description: "Verifica se um valor é um erro (#N/D, #VALOR!, #REF!, #DIV/0!, #NUM!, #NOME? ou #NULO!) e devolve VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que pretende testar. Valor pode referir-se a uma célula, uma fórmula ou um nome que se refere a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "É.ERROS",
		description: "Verifica se o valor é um erro (#VALOR!, #REF!, #DIV/0!, #NUM!, #NOME? ou #NULO!), à exceção de #N/D (valor não disponível) e devolve VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: " o valor que deseja testar. Valor pode referir-se a uma célula, uma fórmula ou um nome que se refere a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "É.FORMULA",
		description: "Verifica se uma referência se relaciona com uma célula que contém uma fórmula e devolve VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "referência",
				description: "é uma referência à célula que deseja testar. A referência pode ser uma referência de célula, uma fórmula ou um nome que faça referência a uma célula"
			}
		]
	},
	{
		name: "É.LÓGICO",
		description: "Verifica se Valor é um valor lógico (VERDADEIRO ou FALSO) e devolve VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que deseja testar. O valor pode referir-se a uma célula, uma fórmula ou um nome referente a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "É.NÃO.DISP",
		description: "Devolve VERDADEIRO ou FALSO se for um valor de erro #N/D (valor não disponível).",
		arguments: [
			{
				name: "valor",
				description: "é o valor que deseja testar. Valor pode referir-se a uma célula, uma fórmula ou um nome que se refere a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "É.NÃO.TEXTO",
		description: "Devolve VERDADEIRO ou FALSO se um valor não for texto (células em branco não são texto).",
		arguments: [
			{
				name: "valor",
				description: "é o valor que deseja testar: uma célula; uma fórmula; um nome com referência a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "É.NÚM",
		description: "Devolve VERDADEIRO ou FALSO se valor for um número.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que deseja testar. Valor pode referir-se a uma célula, uma fórmula ou um nome que se refere a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "É.PGTO",
		description: "Devolve os juros dos pagamentos de um empréstimo simples durante um período específico.",
		arguments: [
			{
				name: "taxa",
				description: "taxa de juro por período. Por exemplo, utilize 6%/4 para pagamentos trimestrais a 6% APR"
			},
			{
				name: "período",
				description: "período cujos juros deseja obter"
			},
			{
				name: "nper",
				description: "número de períodos de pagamento de um investimento"
			},
			{
				name: "va",
				description: "valor atual da soma global de uma série de pagamentos"
			}
		]
	},
	{
		name: "É.REF",
		description: "Devolve VERDADEIRO ou FALSO se um valor for uma referência.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que deseja testar. Valor pode referir-se a uma célula, uma fórmula ou um nome que se refere a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "É.TEXTO",
		description: "Devolve VERDADEIRO ou FALSO se valor for texto.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que deseja testar. O valor pode referir-se a uma célula, uma fórmula ou um nome que se refere a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "EFECTIVA",
		description: "Devolve a taxa de juros anual efetiva.",
		arguments: [
			{
				name: "taxa_nominal",
				description: "é a taxa de juros nominal"
			},
			{
				name: "núm_por_ano",
				description: "é o número de períodos compostos por ano"
			}
		]
	},
	{
		name: "ÉÍMPAR",
		description: "Devolve VERDADEIRO se o número for ímpar.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a testar"
			}
		]
	},
	{
		name: "ENDEREÇO",
		description: "Cria uma referência de célula como texto, a partir de números de linhas e colunas especificados.",
		arguments: [
			{
				name: "núm_linha",
				description: "é o número de linha a ser utilizado na referência da célula: núm_linha = 1 para a linha 1"
			},
			{
				name: "núm_coluna",
				description: "é o número da coluna a ser utilizado na referência da célula. Por exemplo, núm_coluna = 4 para a coluna D"
			},
			{
				name: "núm_abs",
				description: "especifica o tipo de referência: absoluta = 1; linha absoluta/coluna relativa = 2; linha relativa/coluna absoluta = 3; relativa = 4"
			},
			{
				name: "a1",
				description: "é um valor lógico que especifica o estilo de referência: estilo A1 = 1 ou VERDADEIRO; estilo L1C1 = 0 ou FALSO"
			},
			{
				name: "texto_folha",
				description: "é o texto que especifica o nome da folha de cálculo a ser utilizada como referência externa"
			}
		]
	},
	{
		name: "EPADYX",
		description: "Devolve o erro-padrão do Valor-y previsto para cada x da regressão.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é uma matriz ou intervalo de pontos de dados dependentes e podem ser números ou nomes, matrizes ou referências que contenham números"
			},
			{
				name: "val_conhecidos_x",
				description: "é uma matriz ou intervalo de pontos de dados independentes e podem ser números ou nomes, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "ÉPAR",
		description: "Devolve VERDADEIRO se o número for par.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a testar"
			}
		]
	},
	{
		name: "ESQUERDA",
		description: "Devolve o número especificado de carateres do início de uma cadeia de texto.",
		arguments: [
			{
				name: "texto",
				description: "é a cadeia de texto que contém os carateres que deseja extrair"
			},
			{
				name: "núm_caract",
				description: "especifica quantos carateres ESQUERDA deve extrair; 1, se omisso"
			}
		]
	},
	{
		name: "EXACTO",
		description: "Compara se duas cadeias de texto são iguais e devolve VERDADEIRO ou FALSO. EXATO se for sensível às maiúsculas e minúsculas.",
		arguments: [
			{
				name: "texto1",
				description: "é a primeira cadeia de texto"
			},
			{
				name: "texto2",
				description: "é a segunda cadeia de texto"
			}
		]
	},
	{
		name: "EXP",
		description: "Devolve 'e' elevado à potência de um número dado.",
		arguments: [
			{
				name: "núm",
				description: "é o expoente aplicado à base 'e'. A constante 'e' é igual a 2,71828182845904, a base do logaritmo natural"
			}
		]
	},
	{
		name: "FACTDUPLO",
		description: "Devolve o fatorial duplo de um número.",
		arguments: [
			{
				name: "núm",
				description: "é o valor cujo fatorial duplo se pretende"
			}
		]
	},
	{
		name: "FACTORIAL",
		description: "Devolve o fatorial de um número, igual a 1*2*3*...*núm.",
		arguments: [
			{
				name: "núm",
				description: "é um número não negativo cujo fatorial deseja obter"
			}
		]
	},
	{
		name: "FALSO",
		description: "Devolve o valor lógico FALSO.",
		arguments: [
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
		name: "FIMMÊS",
		description: "Devolve o número de série do último dia do mês antes ou depois de um dado número de meses.",
		arguments: [
			{
				name: "data_inicial",
				description: "é o número de série de data que representa a data inicial"
			},
			{
				name: "meses",
				description: "é o número de meses antes ou depois de data_inicial"
			}
		]
	},
	{
		name: "FISHER",
		description: "Devolve a transformação Fisher.",
		arguments: [
			{
				name: "x",
				description: "é um valor numérico para o qual deseja a transformação, um número entre -1 e 1, excluindo -1 e 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Devolve o inverso da transformação Fisher: se y = FISHER(x), então FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "é o valor para o qual deseja executar o inverso da transformação"
			}
		]
	},
	{
		name: "FIXA",
		description: "Arredonda um número para as casas decimais especificadas e devolve o resultado como texto, com ou sem separadores de milhares.",
		arguments: [
			{
				name: "núm",
				description: "é o número que deseja arredondar e converter para texto"
			},
			{
				name: "decimais",
				description: "é o número de algarismos à direita da vírgula decimal. Se omisso, Decimais = 2"
			},
			{
				name: "sem_sep_milhar",
				description: "é um valor lógico: não mostrar separadores de milhares no texto devolvido = VERDADEIRO; mostrar separadores de milhares no texto devolvido = FALSO ou omisso"
			}
		]
	},
	{
		name: "FOLHA",
		description: "Devolve o número de folha da folha referenciada.",
		arguments: [
			{
				name: "valor",
				description: "é o nome de uma folha ou referência para a qual quer o número de folha. Se omitido, será devolvido o número da folha que contém a função"
			}
		]
	},
	{
		name: "FOLHAS",
		description: "Devolve o número de folhas numa referência.",
		arguments: [
			{
				name: "referência",
				description: "é uma referência para a qual quer saber o número de folhas que contém. Se omitido, é devolvido o número de folhas no livro que tenham a função"
			}
		]
	},
	{
		name: "FÓRMULA.TEXTO",
		description: "Devolve uma fórmula como uma cadeia.",
		arguments: [
			{
				name: "referência",
				description: "é uma referência a uma fórmula"
			}
		]
	},
	{
		name: "FRACÇÃOANO",
		description: "Devolve a fração do ano que representa o número de dias entre data_inicial e data_final.",
		arguments: [
			{
				name: "data_inicial",
				description: "é o número de série de data que representa a data inicial"
			},
			{
				name: "data_final",
				description: "é o número de série de data que representa a data final"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a utilizar"
			}
		]
	},
	{
		name: "FREQUÊNCIA",
		description: "Calcula a frequência com que ocorrem valores num intervalo e devolve a matriz vertical de números com mais um elemento que Matriz_bin.",
		arguments: [
			{
				name: "matriz_dados",
				description: "é uma matriz ou uma referência a um conjunto de valores cuja frequência deseja contar (valores em branco ou texto são ignorados)"
			},
			{
				name: "matriz_bin",
				description: "é uma matriz ou referência a intervalos nos quais deseja agrupar os valores contidos em Matriz_dados"
			}
		]
	},
	{
		name: "FUNCERRO",
		description: "Devolve a função de erro.",
		arguments: [
			{
				name: "limite_inferior",
				description: "é o limite inferior da integração de FUNCERRO"
			},
			{
				name: "limite_superior",
				description: "é o limite superior na integração de FUNERRO"
			}
		]
	},
	{
		name: "FUNCERRO.PRECISO",
		description: "Devolve a função de erro.",
		arguments: [
			{
				name: "X",
				description: "é o limite inferior da integração de FUNCERRO.PRECISO"
			}
		]
	},
	{
		name: "FUNCERROCOMPL",
		description: "Devolve a função complementar de erro.",
		arguments: [
			{
				name: "x",
				description: "é o limite inferior da integração de FUNCERRO"
			}
		]
	},
	{
		name: "FUNCERROCOMPL.PRECISO",
		description: "Devolve a função complementar de erro.",
		arguments: [
			{
				name: "X",
				description: "é o limite inferior da integração de FUNCERROCOMPL.PRECISO"
			}
		]
	},
	{
		name: "GAMA",
		description: "Devolve o valor da função Gama.",
		arguments: [
			{
				name: "x",
				description: "é o valor para o qual quer calcular Gama"
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
		name: "GRAUS",
		description: "Converte radianos em graus.",
		arguments: [
			{
				name: "ângulo",
				description: "é o ângulo em radianos que deseja converter"
			}
		]
	},
	{
		name: "HEXABIN",
		description: "Converte um número hexadecimal em binário.",
		arguments: [
			{
				name: "núm",
				description: "é o número hexadecimal a converter"
			},
			{
				name: "casas",
				description: "é o número de carateres a utilizar"
			}
		]
	},
	{
		name: "HEXADEC",
		description: "Converte um número hexadecimal em decimal.",
		arguments: [
			{
				name: "núm",
				description: "é o número hexadecimal a converter"
			}
		]
	},
	{
		name: "HEXAOCT",
		description: "Converte um número hexadecimal em octal.",
		arguments: [
			{
				name: "núm",
				description: "é o número hexadecimal a converter"
			},
			{
				name: "casas",
				description: "é o número de carateres a utilizar"
			}
		]
	},
	{
		name: "HIPERLIGAÇÃO",
		description: "Cria um atalho ou salto que abre um documento armazenado no disco rígido, num servidor da rede ou na Internet.",
		arguments: [
			{
				name: "local_vínculo",
				description: "é o texto que dá o caminho e o nome do ficheiro a ser aberto, uma localização de unidade de disco rígido, endereço de UNC ou caminho de URL"
			},
			{
				name: "nome_abrev",
				description: "é texto ou um número que aparece na célula. Se omisso, a célula mostra o texto de Local_vínculo"
			}
		]
	},
	{
		name: "HOJE",
		description: "Devolve a data atual formatada como uma data.",
		arguments: [
		]
	},
	{
		name: "HORA",
		description: "Devolve a hora como um número de 0 (00:00) a 23 (23:00).",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código de data e hora utilizado pelo Spreadsheet ou texto em formato de hora, tal como 16:48:00 ou 4:48:00 PM"
			}
		]
	},
	{
		name: "IMABS",
		description: "Devolve o valor absoluto (módulo) de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cujo valor absoluto se deseja"
			}
		]
	},
	{
		name: "IMAGINÁRIO",
		description: "Devolve o coeficiente imaginário de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cujo coeficiente imaginário se deseja"
			}
		]
	},
	{
		name: "IMARG",
		description: "Devolve o argumento q, um ângulo expresso em radianos.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cujo argumento se deseja"
			}
		]
	},
	{
		name: "IMCONJ",
		description: "Devolve o conjugado complexo de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cujo conjugado se deseja"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Devolve o cosseno de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cujo cosseno se deseja"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Devolve o cosseno de um número complexo.",
		arguments: [
			{
				name: "número",
				description: "é o número complexo para o qual quer obter o cosseno"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Devolve a cotangente de um número complexo.",
		arguments: [
			{
				name: "número",
				description: "é um número complexo para o qual quer obter a cotangente"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Devolve a cossecante de um número complexo.",
		arguments: [
			{
				name: "número",
				description: "é um número complexo para o qual quer obter a cossecante"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Devolve a cossecante hiperbólica de um número complexo.",
		arguments: [
			{
				name: "número",
				description: "é um número complexo para o qual quer obter a cossecante hiperbólica"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Devolve o quociente de dois números complexos.",
		arguments: [
			{
				name: "inúm1",
				description: "é o numerador ou dividendo complexo"
			},
			{
				name: "inúm2",
				description: "é o denominador ou divisor complexo"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Devolve o exponencial de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cujo exponencial se deseja"
			}
		]
	},
	{
		name: "IMLN",
		description: "Devolve o logaritmo natural de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cujo logaritmo natural se deseja"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Devolve o logaritmo de base 10 de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cujo logaritmo de base 10 se deseja"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Devolve o logaritmo de base 2 de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cujo logaritmo de base 2 se deseja"
			}
		]
	},
	{
		name: "ÍMPAR",
		description: "Arredonda um número positivo para cima e um número negativo para baixo até ao número ímpar inteiro mais próximo.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a arredondar"
			}
		]
	},
	{
		name: "IMPOT",
		description: "Devolve um número complexo elevado a uma potência inteira.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo que se deseja elevar a uma potência"
			},
			{
				name: "núm",
				description: "é a potência para a qual se deseja elevar o número complexo"
			}
		]
	},
	{
		name: "IMPROD",
		description: "Devolve o produto de 1 a 255 números complexos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inúm1",
				description: "Inúm1, Inúm2,... de 1 a 255 números complexos para multiplicar."
			},
			{
				name: "inúm2",
				description: "Inúm1, Inúm2,... de 1 a 255 números complexos para multiplicar."
			}
		]
	},
	{
		name: "IMRAIZ",
		description: "Devolve a raiz quadrada de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cuja raiz quadrada se deseja"
			}
		]
	},
	{
		name: "IMREAL",
		description: "Devolve o coeficiente real de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cujo coeficiente real se deseja"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Devolve a secante de um número complexo.",
		arguments: [
			{
				name: "número",
				description: "é um número complexo para o qual quer obter a secante"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Devolve a secante hiperbólica de um número complexo.",
		arguments: [
			{
				name: "número",
				description: "é um número complexo para o qual quer obter a secante hiperbólica"
			}
		]
	},
	{
		name: "IMSENO",
		description: "Devolve o seno de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é o número complexo cujo seno se deseja"
			}
		]
	},
	{
		name: "IMSENOH",
		description: "Devolve o seno hiperbólico de um número complexo.",
		arguments: [
			{
				name: "número",
				description: "é o número complexo para o qual quer obter o seno hiperbólico"
			}
		]
	},
	{
		name: "IMSOMA",
		description: "Devolve a soma de números complexos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inúm1",
				description: "de 1 a 255 números complexos para adicionar"
			},
			{
				name: "inúm2",
				description: "de 1 a 255 números complexos para adicionar"
			}
		]
	},
	{
		name: "IMSUBTR",
		description: "Devolve a diferença de dois números complexos.",
		arguments: [
			{
				name: "inúm1",
				description: "é o número complexo do qual inúm2 será subtraído"
			},
			{
				name: "inúm2",
				description: "é o número complexo a ser subtraído de inúm1"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Devolve a tangente de um número complexo.",
		arguments: [
			{
				name: "número",
				description: "é um número complexo para o qual quer obter a tangente"
			}
		]
	},
	{
		name: "ÍNDICE",
		description: "Devolve um valor ou a referência da célula na interseção de uma linha e coluna em particular, num determinado intervalo.",
		arguments: [
			{
				name: "matriz",
				description: "é um intervalo de células ou uma constante de matriz"
			},
			{
				name: "núm_linha",
				description: "seleciona a linha na matriz ou referência de onde será devolvido um valor. Se for omitido, é necessário Núm_coluna"
			},
			{
				name: "núm_coluna",
				description: "seleciona a coluna na matriz ou referência de onde será devolvido um valor. Se for omisso, é necessário Núm_linha"
			}
		]
	},
	{
		name: "INDIRECTO",
		description: "Devolve uma referência especificada por um valor de texto.",
		arguments: [
			{
				name: "ref_texto",
				description: "é uma referência a uma célula que contém uma referência A1- ou do estilo L1C1, um nome definido como uma referência ou uma referência a uma célula como cadeia de texto"
			},
			{
				name: "a1",
				description: "é um valor lógico que especifica o tipo de referência em ref_texto: estilo-L1C1 = FALSO; estilo-A1 = VERDADEIRO ou omisso"
			}
		]
	},
	{
		name: "INFORMAÇÃO",
		description: "Devolve informações acerca do ambiente de funcionamento atual.",
		arguments: [
			{
				name: "tipo_texto",
				description: "é o texto que especifica o tipo de informação a ser devolvida."
			}
		]
	},
	{
		name: "INICIAL.MAIÚSCULA",
		description: "Converte uma cadeia de texto em maiúsculas/minúsculas adequadas; a primeira letra de cada palavra em maiúsculas e todas as outras letras em minúsculas.",
		arguments: [
			{
				name: "texto",
				description: "é o texto entre aspas, uma fórmula que devolve texto ou uma referência a uma célula que contém o texto que deseja colocar parcialmente em maiúsculas"
			}
		]
	},
	{
		name: "INT",
		description: "Arredonda um número por defeito até ao número inteiro mais próximo.",
		arguments: [
			{
				name: "núm",
				description: "é o número real que deseja arredondar por defeito até um inteiro"
			}
		]
	},
	{
		name: "INT.CONFIANÇA",
		description: "Devolve o intervalo de confiança para uma média da população utilizando uma distribuição normal.",
		arguments: [
			{
				name: "alfa",
				description: "é o nível de significância utilizado para calcular o nível de confiança, um número maior que 0 e menor que 1"
			},
			{
				name: "desv_padrão",
				description: "é o desvio-padrão da população para o intervalo de dados e assume-se que é conhecido. O Desv_padrão tem de ser maior que 0"
			},
			{
				name: "tamanho",
				description: "é o tamanho da amostra"
			}
		]
	},
	{
		name: "INT.CONFIANÇA.NORM",
		description: "Devolve o intervalo de confiança para uma média da população.",
		arguments: [
			{
				name: "alfa",
				description: "é o nível de significância utilizado para calcular o nível de confiança, um número maior que 0 e menor que 1"
			},
			{
				name: "desv_padrão",
				description: "é o desvio-padrão da população para o intervalo de dados e presume-se que é conhecido. Desv_padrão tem de ser maior que 0"
			},
			{
				name: "tamanho",
				description: "é o tamanho da amostra"
			}
		]
	},
	{
		name: "INT.CONFIANÇA.T",
		description: "Devolve o intervalo de confiança para uma média da população, utilizando uma distribuição T de Student.",
		arguments: [
			{
				name: "alfa",
				description: "é o nível de significância utilizado para calcular o nível de confiança, um número maior que 0 e menor que 1"
			},
			{
				name: "desv_padrão",
				description: "é o desvio-padrão da população para o intervalo de dados e presume-se que é conhecido. Desv_padrão tem de ser maior que 0"
			},
			{
				name: "tamanho",
				description: "é o tamanho da amostra"
			}
		]
	},
	{
		name: "INTERCEPTAR",
		description: "Calcula o ponto onde uma linha intercetará o eixo dos YY, utilizando a melhor linha de regressão, desenhada com os valores conhecidos de X e Y.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é o conjunto dependente de observações ou dados e podem ser números ou nomes, matrizes ou referências que contenham números"
			},
			{
				name: "val_conhecidos_x",
				description: "é o conjunto independente de observações ou dados e podem ser números ou nomes, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "INV.BETA",
		description: "Devolve o inverso da função de densidade de probabilidade beta cumulativa (DIST.BETA).",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade associada à distribuição beta"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição e tem de ser maior que 0"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição e tem de ser maior que 0"
			},
			{
				name: "A",
				description: "é um limite inferior opcional para o intervalo de x. Se omisso, A = 0"
			},
			{
				name: "B",
				description: "é um limite superior opcional para o intervalo de x. Se omisso, B = 1"
			}
		]
	},
	{
		name: "INV.BINOM",
		description: "Devolve o menor valor para o qual a distribuição binomial cumulativa é maior ou igual a um valor de critério.",
		arguments: [
			{
				name: "tentativas",
				description: "é o número de tentativas de Bernoulli"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso em cada tentativa, um número entre 0 e 1 inclusive"
			},
			{
				name: "alfa",
				description: "é o valor do critério, um número entre 0 e 1 inclusive"
			}
		]
	},
	{
		name: "INV.CHI",
		description: "Devolve o inverso da probabilidade unilateral à direita da distribuição chi-quadrado.",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade associada à distribuição chi-quadrado, um valor entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INV.CHIQ",
		description: "Devolve o inverso da probabilidade unilateral à esquerda da distribuição chi-quadrado.",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade associada à distribuição chi-quadrado, um valor entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INV.CHIQ.DIR",
		description: "Devolve o inverso da probabilidade unilateral à direita da distribuição chi-quadrado.",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade associada à distribuição chi-quadrado, um valor entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INV.F",
		description: "Devolve o inverso da distribuição (unilateral à esquerda) de probabilidade F: se p = DIST.F(x,...), então INV.F(p,...) = x.",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade associada à distribuição cumulativa F, um número entre 0 e 1 inclusive"
			},
			{
				name: "grau_liberdade1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "grau_liberdade2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INV.F.DIR",
		description: "Devolve o inverso da distribuição (unilateral à direita) de probabilidade F: se p = DIST.F.DIR(x,...), então INV.F.DIR(p,...) = x.",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade associada à distribuição cumulativa F, um número entre 0 e 1 inclusive"
			},
			{
				name: "grau_liberdade1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "grau_liberdade2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INV.GAMA",
		description: "Devolve o inverso da distribuição cumulativa gama: se p = DIST.GAMA(x,...), então INV.GAMA(p,...) = x.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição gama, um número entre 0 e 1, inclusive"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição, um número positivo. Se beta = 1; INV.GAMA devolve o inverso da distribuição gama padrão"
			}
		]
	},
	{
		name: "INV.NORM",
		description: "Devolve o inverso da distribuição cumulativa normal para a média e o desvio-padrão especificados.",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade correspondente à distribuição normal, um número entre 0 e 1 inclusive"
			},
			{
				name: "média",
				description: "é a média aritmética da distribuição"
			},
			{
				name: "desv_padrão",
				description: "é o desvio-padrão da distribuição, um número positivo"
			}
		]
	},
	{
		name: "INV.NORMAL",
		description: "Devolve o inverso da distribuição cumulativa normal para a média e o desvio-padrão especificados.",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade correspondente à distribuição normal, um número entre 0 e 1 inclusive"
			},
			{
				name: "média",
				description: "é a média aritmética da distribuição"
			},
			{
				name: "desv_padrão",
				description: "é o desvio-padrão da distribuição, um número positivo"
			}
		]
	},
	{
		name: "INV.NORMALLOG",
		description: "Devolve o inverso da função de distribuição normal logarítmica cumulativa de x, onde ln(x) tem uma distribuição normal com Média e Desv_padrão.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição normal logarítmica, um número entre 0 e 1, inclusive"
			},
			{
				name: "média",
				description: "é a média de ln(x)"
			},
			{
				name: "desv_padrão",
				description: "é o desvio-padrão de ln(x), um número positivo"
			}
		]
	},
	{
		name: "INV.NORMP",
		description: "Devolve o inverso da distribuição cumulativa normal padrão (tem uma média de 0 e um desvio-padrão de 1).",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade correspondente à distribuição normal, um número entre 0 e 1 inclusive"
			}
		]
	},
	{
		name: "INV.S.NORM",
		description: "Devolve o inverso da distribuição cumulativa normal padrão (tem uma média de 0 e um desvio-padrão de 1).",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade correspondente à distribuição normal, um número entre 0 e 1 inclusive"
			}
		]
	},
	{
		name: "INV.T",
		description: "Devolve o inverso unilateral à esquerda da distribuição t de Student.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição t de Student bicaudal, um número entre 0 e 1, inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é um número inteiro positivo que indica o número de graus de liberdade que caracteriza a distribuição"
			}
		]
	},
	{
		name: "INV.T.2C",
		description: "Devolve o inverso bicaudal da distribuição t de Student.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição t de Student bicaudal, um número entre 0 e 1, inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é um número inteiro positivo que indica o número de graus de liberdade que caracteriza a distribuição"
			}
		]
	},
	{
		name: "INVF",
		description: "Devolve o inverso da distribuição (unilateral à direita) de probabilidade F: se p = DISTF(x,...), então INVF(p,....) = x.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição cumulativa F, um número entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "graus_liberdade2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INVGAMA",
		description: "Devolve o inverso da distribuição cumulativa gama: se p = DISTGAMA(x,...), então INVGAMA(p,...) = x.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição gama, um número entre 0 e 1, inclusive"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é um parâmetro para a distribuição, um número positivo. Se Beta = 1, INVGAMA devolve o inverso da distribuição gama padrão"
			}
		]
	},
	{
		name: "INVLOG",
		description: "Devolve o inverso da função de distribuição normal logarítmica cumulativa de x, onde ln(x) tem uma distribuição normal com parâmetros Média e Desv_padrão.",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade associada à distribuição normal logarítmica, um número entre 0 e 1, inclusive"
			},
			{
				name: "média",
				description: "é a média de ln(x)"
			},
			{
				name: "desv_padrão",
				description: "é o desvio-padrão de ln(x), um número positivo"
			}
		]
	},
	{
		name: "INVT",
		description: "Devolve o inverso bicaudal da distribuição t de Student.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição t de Student bicaudal, um número entre 0 e 1, inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é um número inteiro positivo que indica o número de graus de liberdade para caracterizar a distribuição"
			}
		]
	},
	{
		name: "IPGTO",
		description: "Devolve o pagamento dos juros de um investimento durante um dado período, a partir de pagamentos periódicos e uma taxa de juro constantes.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juro por período. Por exemplo, utilize 6%/4 para pagamentos trimestrais a 6% APR"
			},
			{
				name: "período",
				description: "é o período cujos juros deseja obter e tem de estar situado no intervalo entre 1 e Nper"
			},
			{
				name: "nper",
				description: "é o número total de períodos de pagamento de um investimento"
			},
			{
				name: "va",
				description: "é o valor atual da quantia correspondente aos pagamentos futuros"
			},
			{
				name: "vf",
				description: "é o valor futuro ou o saldo em dinheiro que deseja atingir após ter sido efetuado o último pagamento. Se omisso, Fv = 0"
			},
			{
				name: "tipo",
				description: "é o valor lógico que representa o escalonamento do pagamento: no final do período = 0 ou omisso; no início do período = 1"
			}
		]
	},
	{
		name: "JUROSACUMV",
		description: "Devolve os juros decorridos de um título que paga juros no vencimento.",
		arguments: [
			{
				name: "emissão",
				description: "é a data de emissão do título, expressa como um número de série de data"
			},
			{
				name: "liquidação",
				description: "é a data de vencimento do título, expressa como um número de série de data"
			},
			{
				name: "taxa",
				description: "é a taxa de cupão anual do título"
			},
			{
				name: "valor_nominal",
				description: "é o valor de paridade do título"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a utilizar"
			}
		]
	},
	{
		name: "LIMPARB",
		description: "Remove todos os carateres do texto que não é possível imprimir.",
		arguments: [
			{
				name: "texto",
				description: "é qualquer informação da folha de cálculo de onde deseja remover os carateres que não é possível imprimir"
			}
		]
	},
	{
		name: "LIN",
		description: "Devolve o número da linha de uma referência.",
		arguments: [
			{
				name: "referência",
				description: "é a célula ou um único intervalo de células cujo número da linha deseja obter; se omisso, devolve a célula contendo a função LIN"
			}
		]
	},
	{
		name: "LINS",
		description: "Devolve o número de linhas numa referência ou matriz.",
		arguments: [
			{
				name: "matriz",
				description: "é uma matriz, uma fórmula matricial ou uma referência a um intervalo de células cujo número de linhas deseja obter"
			}
		]
	},
	{
		name: "LN",
		description: "Devolve o logaritmo natural de um número.",
		arguments: [
			{
				name: "núm",
				description: "é o número real positivo cujo logaritmo natural deseja obter"
			}
		]
	},
	{
		name: "LNGAMA",
		description: "Devolve o logaritmo natural da função gama.",
		arguments: [
			{
				name: "x",
				description: "é o valor para o qual deseja calcular LNGAMA, um número positivo"
			}
		]
	},
	{
		name: "LNGAMA.PRECISO",
		description: "Devolve o logaritmo natural da função gama.",
		arguments: [
			{
				name: "x",
				description: "é o valor para o qual deseja calcular LNGAMA.PRECISO, um número positivo"
			}
		]
	},
	{
		name: "LOCALIZAR",
		description: "Devolve a posição de partida de uma cadeia de texto dentro de outra. LOCALIZAR é sensível a maiúsculas e minúsculas.",
		arguments: [
			{
				name: "texto_a_localizar",
				description: "é o texto que deseja localizar. Utilize aspas (sem texto) para fazer a comparação com o primeiro caráter de No_texto; não são permitidos carateres universais"
			},
			{
				name: "no_texto",
				description: "é o texto que contém o que deseja localizar"
			},
			{
				name: "núm_inicial",
				description: "especifica o caráter a partir do qual será iniciada a pesquisa. O primeiro caráter em No_texto é o número 1. Se for omisso, Núm_inicial = 1"
			}
		]
	},
	{
		name: "LOG",
		description: "Devolve o logaritmo de um número na base especificada.",
		arguments: [
			{
				name: "núm",
				description: "é o número real positivo cujo logaritmo deseja obter"
			},
			{
				name: "base",
				description: "é a base do logaritmo; 10, se omisso"
			}
		]
	},
	{
		name: "LOG10",
		description: "Devolve o logaritmo de base 10 de um número.",
		arguments: [
			{
				name: "núm",
				description: "é o número real positivo cujo logaritmo de base 10 deseja obter"
			}
		]
	},
	{
		name: "LUCRODESC",
		description: "Devolve o rendimento anual de um título descontável, como, por exemplo, os títulos do Tesouro.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número de série de data"
			},
			{
				name: "pr",
				description: "é o preço do título por cada 100 € do valor nominal"
			},
			{
				name: "resgate",
				description: "é o valor de resgate do título por cada 100 € do valor nominal"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a utilizar"
			}
		]
	},
	{
		name: "MAIOR",
		description: "Devolve o n-ésimo maior valor de um conjunto de dados. Por exemplo, o quinto número maior.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados para que deseja determinar o n-ésimo maior valor"
			},
			{
				name: "k",
				description: "é a posição (a partir do maior) na matriz ou intervalo de células do valor a devolver"
			}
		]
	},
	{
		name: "MAIÚSCULAS",
		description: "Converte uma cadeia de texto em maiúsculas.",
		arguments: [
			{
				name: "texto",
				description: "é o texto que deseja converter para maiúsculas, uma referência ou uma cadeia de texto"
			}
		]
	},
	{
		name: "MARRED",
		description: "Devolve um número arredondado para o múltiplo desejado.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a arredondar"
			},
			{
				name: "múltiplo",
				description: "é o múltiplo para o qual se deseja arredondar núm"
			}
		]
	},
	{
		name: "MATRIZ.DETERM",
		description: "Devolve o determinante de uma matriz.",
		arguments: [
			{
				name: "matriz",
				description: "é uma matriz numérica com um número igual de linhas e colunas, seja um intervalo de células ou uma constante de matriz"
			}
		]
	},
	{
		name: "MATRIZ.INVERSA",
		description: "Devolve a inversa de uma matriz armazenada numa matriz.",
		arguments: [
			{
				name: "matriz",
				description: "é uma matriz numérica com o mesmo número de linhas e colunas, seja um intervalo de células seja uma constante de matriz"
			}
		]
	},
	{
		name: "MATRIZ.MULT",
		description: "Devolve a matriz produto de duas matrizes, uma matriz com o mesmo número de linhas que matriz1 e de colunas que matriz2.",
		arguments: [
			{
				name: "matriz1",
				description: "é a primeira matriz de números a multiplicar e tem de ter o mesmo número de colunas que Matriz2 tem de linhas"
			},
			{
				name: "matriz2",
				description: "é a primeira matriz de números a multiplicar e tem de ter o mesmo número de colunas que Matriz2 tem de linhas"
			}
		]
	},
	{
		name: "MÁXIMO",
		description: "Devolve o valor máximo de uma lista de argumentos. Ignora os valores lógicos e texto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números, células vazias, valores lógicos ou números de texto para os quais deseja encontrar o valor máximo"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números, células vazias, valores lógicos ou números de texto para os quais deseja encontrar o valor máximo"
			}
		]
	},
	{
		name: "MÁXIMOA",
		description: "Devolve o valor máximo de um conjunto de valores. Não ignora valores lógicos e texto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "são de 1 a 255 números, células vazias, valores lógicos ou números de texto cujo máximo deseja determinar"
			},
			{
				name: "valor2",
				description: "são de 1 a 255 números, células vazias, valores lógicos ou números de texto cujo máximo deseja determinar"
			}
		]
	},
	{
		name: "MDC",
		description: "devolve o maior divisor comum.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 valores"
			},
			{
				name: "núm2",
				description: "de 1 a 255 valores"
			}
		]
	},
	{
		name: "MED",
		description: "Devolve a mediana ou o número no meio de um conjunto de números indicados.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências que contêm números para os quais deseja obter a mediana"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências que contêm números para os quais deseja obter a mediana"
			}
		]
	},
	{
		name: "MÉDIA",
		description: "Devolve a média aritmética dos argumentos, que podem ser números ou nomes, matrizes ou referências que contêm números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 argumentos numéricos para os quais pretende obter a média"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 argumentos numéricos para os quais pretende obter a média"
			}
		]
	},
	{
		name: "MÉDIA.GEOMÉTRICA",
		description: "Devolve a média geométrica de uma matriz ou intervalo de dados numéricos positivos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências contendo números, cujas médias pretende calcular"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências contendo números, cujas médias pretende calcular"
			}
		]
	},
	{
		name: "MÉDIA.HARMÓNICA",
		description: "Devolve a média harmónica de um conjunto de dados de números positivos: o recíproco da média aritmética dos recíprocos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências que contêm números para os quais deseja determinar a média harmónica"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências que contêm números para os quais deseja determinar a média harmónica"
			}
		]
	},
	{
		name: "MÉDIA.INTERNA",
		description: "Devolve a média da porção interior de um conjunto de valores de dados.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de valores que deseja compactar e calcular a média"
			},
			{
				name: "percentagem",
				description: "é o número fracionário de ponto de dados a ser excluído do topo e da base do conjunto de dados"
			}
		]
	},
	{
		name: "MÉDIA.SE",
		description: "Calcula a média (média aritmética) das células especificadas por uma determinada condição ou critério.",
		arguments: [
			{
				name: "intervalo",
				description: "é o intervalo de células que pretende avaliar"
			},
			{
				name: "critérios",
				description: "é a condição ou critério, sob a forma de um número, expressão ou texto, que define as células que serão utilizadas para calcular a média"
			},
			{
				name: "intervalo_médio",
				description: "são as células a utilizar para calcular a média. Se omissas, utilizam-se as células do intervalo"
			}
		]
	},
	{
		name: "MÉDIA.SE.S",
		description: "Calcula a média (média aritmética) das células especificadas por um determinado conjunto de condições ou critérios.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "intervalo_médio",
				description: "são as células a utilizar para calcular a média."
			},
			{
				name: "intervalo_critérios",
				description: "é o intervalo de células que pretende avaliar para a condição específica"
			},
			{
				name: "critérios",
				description: "é a condição ou critério, sob a forma de um número, expressão ou texto, que define as células que serão utilizadas para calcular a média"
			}
		]
	},
	{
		name: "MÉDIAA",
		description: "Devolve a média aritmética dos argumentos, que podem ser números, nomes, matrizes ou referências, fazendo texto e FALSO = 0; VERDADEIRO = 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "são de 1 a 255 argumentos cuja média pretende obter"
			},
			{
				name: "valor2",
				description: "são de 1 a 255 argumentos cuja média pretende obter"
			}
		]
	},
	{
		name: "MENOR",
		description: "Devolve o n-ésimo menor valor de um conjunto de dados. Por exemplo, o quinto número menor.",
		arguments: [
			{
				name: "matriz",
				description: "é uma matriz ou intervalo de dados numéricos em que deseja determinar o n-ésimo menor valor"
			},
			{
				name: "k",
				description: "é a posição (a partir da menor), na matriz ou intervalo de dados, do valor a ser fornecido"
			}
		]
	},
	{
		name: "MÊS",
		description: "Devolve o mês, um número de 1 (janeiro) a 12 (dezembro).",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código de data e hora utilizado pelo Spreadsheet"
			}
		]
	},
	{
		name: "MÍNIMO",
		description: "Devolve o valor mais pequeno de um conjunto de valores. Ignora valores lógicos e texto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números, células vazias, valores lógicos ou números de texto para os quais deseja encontrar o valor mínimo"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números, células vazias, valores lógicos ou números de texto para os quais deseja encontrar o valor mínimo"
			}
		]
	},
	{
		name: "MÍNIMOA",
		description: "Devolve o valor mais pequeno contido num conjunto de valores. Não ignora valores lógicos e texto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "são de 1 a 255 números, células vazias, valores lógicos ou números de texto cujo mínimo pretende determinar"
			},
			{
				name: "valor2",
				description: "são de 1 a 255 números, células vazias, valores lógicos ou números de texto cujo mínimo pretende determinar"
			}
		]
	},
	{
		name: "MINÚSCULAS",
		description: "Converte todas as letras de uma cadeia de texto em minúsculas.",
		arguments: [
			{
				name: "texto",
				description: "é o texto que deseja converter em minúsculas Os carateres em Texto que não forem letras não são alterados"
			}
		]
	},
	{
		name: "MINUTO",
		description: "Devolve os minutos, um número de 0 a 59.",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código de data e hora utilizado pelo Spreadsheet ou texto em formato de hora, tal como 16:48:00 ou 4:48:00 PM"
			}
		]
	},
	{
		name: "MMC",
		description: "Devolve o mínimo múltiplo comum.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 valores para os quais pretende o mínimo múltiplo comum"
			},
			{
				name: "núm2",
				description: "de 1 a 255 valores para os quais pretende o mínimo múltiplo comum"
			}
		]
	},
	{
		name: "MODA",
		description: "Devolve o valor mais frequente ou repetitivo numa matriz ou intervalo de dados.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências contendo números para os quais pretende calcular a moda"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências contendo números para os quais pretende calcular a moda"
			}
		]
	},
	{
		name: "MODO.MÚLT",
		description: "Devolve uma matriz vertical dos valores que ocorrem mais frequentemente, ou repetitivos, numa matriz ou intervalo de dados. Para uma matriz horizontal, utilize =TRANSPOR(MODO.MÚLT(número1,número2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "são 1 a 255 números, ou nomes, matrizes ou referências, que contêm determinados números cujo modo pretende obter"
			},
			{
				name: "número2",
				description: "são 1 a 255 números, ou nomes, matrizes ou referências, que contêm determinados números cujo modo pretende obter"
			}
		]
	},
	{
		name: "MODO.SIMPLES",
		description: "Devolve o valor mais frequente numa matriz ou intervalo de dados.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências contendo números para os quais deseja calcular a moda"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números ou nomes, matrizes ou referências contendo números para os quais deseja calcular a moda"
			}
		]
	},
	{
		name: "MOEDA",
		description: "Converte um número em texto, utilizando o formato monetário.",
		arguments: [
			{
				name: "núm",
				description: "é um número, uma referência a uma célula que contém um número, ou uma fórmula que gera um número"
			},
			{
				name: "decimais",
				description: "é o número de algarismos à direita da vírgula decimal. O número é arredondado de acordo com as necessidades; se omisso, Decimais = 2"
			}
		]
	},
	{
		name: "MOEDADEC",
		description: "Converte a expressão de um preço em moeda de uma fração para um número decimal.",
		arguments: [
			{
				name: "moeda_fraccionária",
				description: "é um número expresso como uma fração"
			},
			{
				name: "fração",
				description: "é o inteiro a utilizar no denominador da fração"
			}
		]
	},
	{
		name: "MOEDAFRA",
		description: "Converte a expressão de um preço em moeda de um número decimal para uma fração.",
		arguments: [
			{
				name: "moeda_decimal",
				description: "é um número decimal"
			},
			{
				name: "fração",
				description: "é o inteiro a utilizar no denominador da fração"
			}
		]
	},
	{
		name: "MTIR",
		description: "Devolve a taxa interna de rentabilidade de fluxos monetários periódicos, avaliando custos de investimento e juros de reinvestimento dos valores líquidos.",
		arguments: [
			{
				name: "valores",
				description: "é uma matriz ou uma referência a células que contêm números que representam uma série de pagamentos (negativos) e recebimentos (positivos) em períodos regulares"
			},
			{
				name: "taxa_financ",
				description: "é a taxa de juro paga sobre o dinheiro utilizado em fluxos monetários"
			},
			{
				name: "taxa_reinvest",
				description: "é a taxa de juros recebida sobre os fluxos monetários à medida que estes foram sendo reinvestidos"
			}
		]
	},
	{
		name: "N",
		description: "Converte um valor não numérico para um número, datas para números de série, VERDADEIRO para 1, o resto para 0 (zero).",
		arguments: [
			{
				name: "valor",
				description: "é o valor que deseja converter"
			}
		]
	},
	{
		name: "NÃO",
		description: "Altera FALSO para VERDADEIRO e VERDADEIRO para FALSO.",
		arguments: [
			{
				name: "valor_lógico",
				description: "é o valor ou expressão que pode ser avaliado como VERDADEIRO ou FALSO"
			}
		]
	},
	{
		name: "NÃO.DISP",
		description: "Devolve o valor de erro #N/D (valor não disponível).",
		arguments: [
		]
	},
	{
		name: "NOMINAL",
		description: "Devolve a taxa de juros nominal anual.",
		arguments: [
			{
				name: "taxa_efetiva",
				description: "é a taxa de juros efetiva"
			},
			{
				name: "núm_por_ano",
				description: "é o número de períodos compostos por ano"
			}
		]
	},
	{
		name: "NORMALIZAR",
		description: "Devolve um valor normalizado de uma distribuição caracterizada por uma média e um desvio-padrão.",
		arguments: [
			{
				name: "x",
				description: "é o valor que deseja normalizar"
			},
			{
				name: "média",
				description: "é a média aritmética da distribuição"
			},
			{
				name: "desv_padrão",
				description: "é o desvio-padrão da distribuição, um número positivo"
			}
		]
	},
	{
		name: "NPER",
		description: "Devolve o número de períodos de um investimento, com base em pagamentos periódicos constantes e uma taxa de juros constante.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juro por período. Por exemplo, utilize 6%/4 para pagamentos trimestrais a 6% APR"
			},
			{
				name: "pgto",
				description: "é o pagamento efetuado em cada período; não é possível alterá-lo enquanto durar o investimento"
			},
			{
				name: "va",
				description: "é o valor atual da quantia correspondente aos pagamentos futuros"
			},
			{
				name: "vf",
				description: "é o valor futuro ou o saldo em dinheiro que deseja atingir após o último pagamento ter sido efetuado. Se omisso, é utilizado zero"
			},
			{
				name: "tipo",
				description: "é um valor lógico: pagamento no início do período = 1; no final do período = 0 ou omisso"
			}
		]
	},
	{
		name: "NÚM.CARACT",
		description: "Devolve o número de carateres de uma cadeia de texto.",
		arguments: [
			{
				name: "texto",
				description: "é o texto cujo comprimento deseja determinar. Os espaços contam como carateres"
			}
		]
	},
	{
		name: "NÚMSEMANA",
		description: "Devolve o número da semana no ano.",
		arguments: [
			{
				name: "núm_série",
				description: "é o código de data e hora utilizado pelo Spreadsheet para cálculo de data e hora"
			},
			{
				name: "tipo_retorno",
				description: "é um número (1 ou 2) que determina o tipo do valor devolvido"
			}
		]
	},
	{
		name: "NUMSEMANAISO",
		description: "Devolve o número do número de semana ISO do ano de uma determinada data.",
		arguments: [
			{
				name: "data",
				description: "é o código data-hora utilizado pelo Spreadsheet para calcular a data e hora"
			}
		]
	},
	{
		name: "OBTERDADOSDIN",
		description: "Extrai os dados guardados numa Tabela Dinâmica.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "campo_dados",
				description: "é o nome do campo de dados a partir do qual os dados serão extraídos"
			},
			{
				name: "tabela_dinâmica",
				description: "é uma referência a uma célula ou intervalo de células na Tabela Dinâmica que contém os dados que pretende obter"
			},
			{
				name: "campo",
				description: "campo para referir"
			},
			{
				name: "item",
				description: "item do campo para referir"
			}
		]
	},
	{
		name: "OCTABIN",
		description: "Converte um número octal em binário.",
		arguments: [
			{
				name: "núm",
				description: "é o número octal a converter"
			},
			{
				name: "casas",
				description: "é o número de carateres a utilizar"
			}
		]
	},
	{
		name: "OCTADEC",
		description: "Converte um número octal em decimal.",
		arguments: [
			{
				name: "núm",
				description: "é o número octal a converter"
			}
		]
	},
	{
		name: "OCTAHEX",
		description: "Converte um número octal em hexadecimal.",
		arguments: [
			{
				name: "núm",
				description: "é o número octal a converter"
			},
			{
				name: "casas",
				description: "é o número de carateres a utilizar"
			}
		]
	},
	{
		name: "ORDEM",
		description: "Devolve a classificação de um número numa lista de números: o tamanho do mesmo em relação a outros valores na lista.",
		arguments: [
			{
				name: "núm",
				description: "é o número cuja classificação pretende encontrar"
			},
			{
				name: "ref",
				description: "é uma matriz (ou uma referência) de uma lista de números. Valores não numéricos são ignorados"
			},
			{
				name: "ordem",
				description: "é um número: classificação na lista por ordem descendente = 0 ou omisso; classificação na lista por ordem ascendente = qualquer valor diferente de zero"
			}
		]
	},
	{
		name: "ORDEM.EQ",
		description: "Devolve a classificação de um número numa lista de números: o seu tamanho em relação a outros valores na lista; se mais de um valor tiver a mesma classificação, é devolvida a classificação superior desse conjunto de valores.",
		arguments: [
			{
				name: "núm",
				description: "é o número cuja classificação pretende encontrar"
			},
			{
				name: "ref",
				description: "é uma matriz (ou uma referência) de uma lista de números. Os valores não numéricos são ignorados"
			},
			{
				name: "ordem",
				description: "é um número: classificação na lista por ordem descendente = 0 ou omisso; classificação na lista por ordem ascendente = qualquer valor diferente de zero"
			}
		]
	},
	{
		name: "ORDEM.MÉD",
		description: "Devolve a classificação de um número numa lista de números: o seu tamanho em relação a outros valores na lista; se mais de um valor tiver a mesma classificação, é devolvida a classificação média.",
		arguments: [
			{
				name: "núm",
				description: "é o número cuja classificação pretende encontrar"
			},
			{
				name: "ref",
				description: "é uma matriz (ou uma referência) de uma lista de números. Os valores não numéricos são ignorados"
			},
			{
				name: "ordem",
				description: "é um número: classificação na lista por ordem descendente = 0 ou omisso; classificação na lista por ordem ascendente = qualquer valor diferente de zero"
			}
		]
	},
	{
		name: "ORDEM.PERCENTUAL",
		description: "Devolve a classificação de um valor num conjunto de dados como percentagem desse conjunto.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados com valores numéricos que define a posição relativa"
			},
			{
				name: "x",
				description: "é o valor para o qual pretende saber a classificação"
			},
			{
				name: "significância",
				description: "é um valor opcional que identifica o número de algarismos significativos para a percentagem devolvida, três algarismos se omisso (0,xxx%)"
			}
		]
	},
	{
		name: "ORDEM.PERCENTUAL.EXC",
		description: "Devolve a classificação de um valor num conjunto de dados como percentagem (0..1, exclusive) desse conjunto.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados com valores numéricos que define uma classificação relativa"
			},
			{
				name: "x",
				description: "é o valor para o qual pretende saber a posição"
			},
			{
				name: "significância",
				description: "é um valor opcional que identifica o número de dígitos significativos para a percentagem devolvida, três dígitos se omisso (0,xxx%)"
			}
		]
	},
	{
		name: "ORDEM.PERCENTUAL.INC",
		description: "Devolve a classificação de um valor num conjunto de dados como percentagem (0..1, inclusive) desse conjunto.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados com valores numéricos que define uma classificação relativa"
			},
			{
				name: "x",
				description: "é o valor para o qual deseja saber a ordem"
			},
			{
				name: "significância",
				description: "é um valor opcional que identifica o número de dígitos significativos para a percentagem devolvida, três dígitos se omisso (0,xxx%)"
			}
		]
	},
	{
		name: "OTN",
		description: "Devolve o rendimento de um título do Tesouro equivalente a um novo título.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título do Tesouro, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título do Tesouro, expressa como um número de série de data"
			},
			{
				name: "desconto",
				description: "é a taxa de desconto do título do Tesouro"
			}
		]
	},
	{
		name: "OTNLUCRO",
		description: "Devolve o rendimento de um título do Tesouro.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título do Tesouro, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título do Tesouro, expressa como um número de série de data"
			},
			{
				name: "pr",
				description: "é o preço por cada 100 € do valor nominal do título do Tesouro"
			}
		]
	},
	{
		name: "OTNVALOR",
		description: "Devolve o preço por cada 100 € do valor nominal de um título do Tesouro.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título do Tesouro, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título do Tesouro, expressa como um número de série de data"
			},
			{
				name: "desconto",
				description: "é a taxa de desconto do título do Tesouro"
			}
		]
	},
	{
		name: "OU",
		description: "Verifica se algum dos argumentos é VERDADEIRO e devolve VERDADEIRO ou FALSO. Devolve FALSO se todos os argumentos forem FALSO.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor_lógico1",
				description: "são de 1 a 255 condições a serem testadas, que podem ser VERDADEIRO ou FALSO"
			},
			{
				name: "valor_lógico2",
				description: "são de 1 a 255 condições a serem testadas, que podem ser VERDADEIRO ou FALSO"
			}
		]
	},
	{
		name: "PAR",
		description: "Arredonda um número positivo para cima e um número negativo para baixo para o valor inteiro mais próximo.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a arredondar"
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
		name: "PDURAÇÃO",
		description: "Devolve o número de períodos necessários a um investimento para atingir um valor específico.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juro por período"
			},
			{
				name: "va",
				description: "é o valor atual do investimento"
			},
			{
				name: "vf",
				description: "é o valor futuro pretendido do investimento"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Devolve o coeficiente de correlação do momento do produto Pearson, r.",
		arguments: [
			{
				name: "matriz1",
				description: "é um conjunto de valores independentes"
			},
			{
				name: "matriz2",
				description: "é um conjunto de valores dependentes"
			}
		]
	},
	{
		name: "PERCENTIL",
		description: "Devolve o enésimo percentil de valores num intervalo.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados que define a posição relativa"
			},
			{
				name: "k",
				description: "é o valor do percentil no intervalo de 0 a 1, inclusive"
			}
		]
	},
	{
		name: "PERCENTIL.EXC",
		description: "Devolve o enésimo percentil de valores num intervalo, em que k está no intervalo 0..1, exclusive.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados que define a posição relativa"
			},
			{
				name: "k",
				description: "é o valor do percentil no intervalo de 0 a 1, inclusive"
			}
		]
	},
	{
		name: "PERCENTIL.INC",
		description: "Devolve o enésimo percentil de valores num intervalo, em que k está no intervalo 0..1, inclusive.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados que define a posição relativa"
			},
			{
				name: "k",
				description: "é o valor do percentil no intervalo de 0 a 1, inclusive"
			}
		]
	},
	{
		name: "PERMUTAR",
		description: "Devolve o número de permutações para um dado número de objetos que pode ser selecionado de entre a totalidade dos objetos.",
		arguments: [
			{
				name: "núm",
				description: "é o número total de objetos"
			},
			{
				name: "núm_escolhido",
				description: "é o número de objetos em cada permutação"
			}
		]
	},
	{
		name: "PERMUTAR.R",
		description: "Devolve o número de permutações para um determinado número de objetos (com repetições) que pode ser selecionado do total de objetos.",
		arguments: [
			{
				name: "número",
				description: "é o número total de objetos"
			},
			{
				name: "número_escolhido",
				description: "é o número de objetos em cada permutação"
			}
		]
	},
	{
		name: "PGTO",
		description: "Calcula o pagamento de um empréstimo, a partir de pagamentos constantes e uma taxa de juros constante.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juro por período para o empréstimo. Por exemplo, utilize 6%/4 para pagamentos trimestrais a 6% APR"
			},
			{
				name: "nper",
				description: "é o número total de pagamentos do empréstimo"
			},
			{
				name: "va",
				description: "é o valor atual: o montante total que representa agora uma série de pagamentos futuros"
			},
			{
				name: "vf",
				description: "é o valor futuro ou o saldo em dinheiro que deseja obter após o último pagamento ter sido efetuado, 0 (zero), se omisso"
			},
			{
				name: "tipo",
				description: "é um valor lógico: pagamento no início do período = 1; pagamento no final do período = 0 ou omisso"
			}
		]
	},
	{
		name: "PGTOCAPACUM",
		description: "Devolve o capital acumulado pago por um empréstimo entre dois períodos.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juros"
			},
			{
				name: "nper",
				description: "é o número total de períodos de pagamento"
			},
			{
				name: "va",
				description: "é o valor atual"
			},
			{
				name: "início_período",
				description: "é o primeiro período do cálculo"
			},
			{
				name: "final_período",
				description: "é o último período do cálculo"
			},
			{
				name: "tipo_pgto",
				description: "indica quando o pagamento deve ser efetuado"
			}
		]
	},
	{
		name: "PGTOJURACUM",
		description: "Devolve os juros acumulados pagos entre dois períodos.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juros"
			},
			{
				name: "nper",
				description: "é o número total de períodos de pagamento"
			},
			{
				name: "va",
				description: "é o valor atual"
			},
			{
				name: "início_período",
				description: "é o primeiro período do cálculo"
			},
			{
				name: "final_período",
				description: "é o último período do cálculo"
			},
			{
				name: "tipo_pgto",
				description: "indica quando o pagamento deve ser efetuado"
			}
		]
	},
	{
		name: "PHI",
		description: "Devolve o valor da função de densidade para uma distribuição normal padrão.",
		arguments: [
			{
				name: "x",
				description: "é o número para o qual quer obter a densidade da distribuição normal padrão"
			}
		]
	},
	{
		name: "PI",
		description: "Devolve o valor de Pi, 3,14159265358979, com uma precisão de 13 casas decimais.",
		arguments: [
		]
	},
	{
		name: "POISSON",
		description: "Devolve a distribuição de Poisson.",
		arguments: [
			{
				name: "x",
				description: "é o número de eventos"
			},
			{
				name: "média",
				description: "é o valor numérico esperado, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a probabilidade de Poisson cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade de Poisson, utilize FALSO"
			}
		]
	},
	{
		name: "POLINOMIAL",
		description: "Devolve o polinomial de um conjunto de números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 valores para os quais pretende o polinomial"
			},
			{
				name: "núm2",
				description: "de 1 a 255 valores para os quais pretende o polinomial"
			}
		]
	},
	{
		name: "POTÊNCIA",
		description: "Devolve o resultado de um número elevado a uma potência.",
		arguments: [
			{
				name: "núm",
				description: "é o número da base, qualquer número real"
			},
			{
				name: "potência",
				description: "é o expoente pelo qual a base do número é elevada"
			}
		]
	},
	{
		name: "PPGTO",
		description: "Devolve o pagamento sobre o montante de um investimento, a partir de pagamentos constantes e periódicos e uma taxa de juros constante.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juro por período. Por exemplo, utilize 6%/4 para pagamentos trimestrais a 6% APR"
			},
			{
				name: "período",
				description: "especifica o período e tem de estar no intervalo entre 1 e nper"
			},
			{
				name: "nper",
				description: "é o número total de períodos de pagamento de um investimento"
			},
			{
				name: "va",
				description: "é o valor atual: o montante total atual de uma série de pagamentos futuros"
			},
			{
				name: "vf",
				description: "é o valor futuro ou o saldo em dinheiro que deseja atingir após o último pagamento ter sido efetuado"
			},
			{
				name: "tipo",
				description: "é um valor lógico: pagamento no início do período = 1; pagamento no final do período = 0 ou omisso"
			}
		]
	},
	{
		name: "PREÇODESC",
		description: "Devolve o preço por cada 100 € do valor nominal de um título descontável.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número de série de data"
			},
			{
				name: "desconto",
				description: "é a taxa de desconto do título"
			},
			{
				name: "resgate",
				description: "é o valor de resgate do título por cada 100 € do valor nominal"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a utilizar"
			}
		]
	},
	{
		name: "PREVISÃO",
		description: "Calcula ou prevê um valor futuro ao longo de uma tendência linear, utilizando os valores existentes.",
		arguments: [
			{
				name: "x",
				description: "é o ponto de dados cujo valor deseja prever e deve ser um valor numérico"
			},
			{
				name: "val_conhecidos_y",
				description: "é o intervalo de dados numéricos ou matriz dependente"
			},
			{
				name: "val_conhecidos_x",
				description: "é o intervalo de dados numéricos ou matriz independente. A variância de Val_conhecidos_x não deve ser zero"
			}
		]
	},
	{
		name: "PROB",
		description: "Devolve a probabilidade de valores de um intervalo estarem entre dois limites ou serem iguais ao limite inferior.",
		arguments: [
			{
				name: "intervalo_x",
				description: "é o intervalo de valores numéricos de x com os quais estão associadas probabilidades"
			},
			{
				name: "intervalo_prob",
				description: "é um conjunto de probabilidades associadas com valores no Intervalo_x, valores entre 0 e 1 à exceção do 0"
			},
			{
				name: "limite_inferior",
				description: "é o limite inferior do valor cuja probabilidade deseja obter"
			},
			{
				name: "limite_superior",
				description: "é o limite superior opcional do valor. Se omisso, PROB devolve a probabilidade de os valores de Intervalo_x serem iguais a Limite_inferior"
			}
		]
	},
	{
		name: "PROC",
		description: "Devolve um valor, quer de um intervalo de uma linha ou de uma coluna, quer de uma matriz. Fornecido para compatibilidade com conversões anteriores.",
		arguments: [
			{
				name: "valor_proc",
				description: "é um valor que PROC procura em Vector _proc e pode ser um número, texto, um valor lógico ou um nome ou referência a um valor"
			},
			{
				name: "vetor_proc",
				description: "é um intervalo que contém apenas uma linha ou uma coluna de texto, números ou valores lógicos, colocados por ordem ascendente"
			},
			{
				name: "vetor_result",
				description: "é um intervalo que contém apenas uma linha ou coluna, do mesmo tamanho de Vector_proc"
			}
		]
	},
	{
		name: "PROCH",
		description: "Procura um valor na linha superior de uma tabela ou matriz de valores e devolve o valor na mesma coluna de uma linha especificada.",
		arguments: [
			{
				name: "valor_proc",
				description: "é o valor a ser encontrado na primeira linha da tabela; pode ser um valor, uma referência ou uma cadeia de texto"
			},
			{
				name: "matriz_tabela",
				description: "é uma tabela de texto, números ou valores lógicos onde os dados devem ser procurados. Matriz_tabela pode ser uma referência a um intervalo ou um nome de intervalo"
			},
			{
				name: "núm_índice_linha",
				description: "é o número da linha na matriz_tabela de onde o valor correspondente deve ser retirado. A primeira linha de valores na tabela é a linha 1"
			},
			{
				name: "procurar_intervalo",
				description: "é um valor lógico: localizar a correspondência mais próxima na linha superior (ordenada por ordem ascendente) = VERDADEIRO ou omisso; localizar uma correspondência exata = FALSO"
			}
		]
	},
	{
		name: "PROCURAR",
		description: "Devolve o número do caráter no qual é localizado pela primeira vez uma cadeia de texto específica, lida da esquerda para a direita (não distingue maiúsculas e minúsculas).",
		arguments: [
			{
				name: "texto_a_localizar",
				description: "é o texto que deseja localizar. Pode utilizar os carateres universais ? e *; utilize ~? e ~* para localizar os carateres ? e *"
			},
			{
				name: "no_texto",
				description: "é o texto em que deseja procurar o Texto_proc"
			},
			{
				name: "núm_inicial",
				description: "é o número do caráter em No_texto, a contar da esquerda, onde deseja iniciar a pesquisa. Se omisso, será utilizado 1"
			}
		]
	},
	{
		name: "PROCV",
		description: "Procura um valor na coluna mais à esquerda de uma tabela e devolve um valor na mesma linha de uma dada coluna. Por predefinição, a tabela tem de ser ordenada por ordem ascendente.",
		arguments: [
			{
				name: "valor_proc",
				description: "é o valor a ser encontrado na primeira coluna da tabela, e pode ser um valor, uma referência ou uma cadeia de texto"
			},
			{
				name: "matriz_tabela",
				description: "é uma tabela de texto, números ou valores lógicos, de onde os dados são obtidos. Matriz_tabela pode ser uma referência a um intervalo ou um nome de intervalo"
			},
			{
				name: "núm_índice_coluna",
				description: "é o número da coluna em matriz_tabela a partir do qual o valor correspondente deve ser devolvido. A primeira coluna de valores da tabela é a coluna 1"
			},
			{
				name: "procurar_intervalo",
				description: "é um valor lógico: localizar o correspondente mais próximo na primeira coluna (ordenada por ordem ascendente) = VERDADEIRO ou omisso; localizar um correspondente exato = FALSO"
			}
		]
	},
	{
		name: "PRODUTO",
		description: "Multiplica todos os números apresentados como argumentos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números, valores lógicos ou representações de números em texto que pretende multiplicar"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números, valores lógicos ou representações de números em texto que pretende multiplicar"
			}
		]
	},
	{
		name: "PROJ.LIN",
		description: "Devolve estatísticas que descrevem uma tendência linear que coincide com os dados conhecidos, baseada numa reta obtida por aplicação do método dos quadrados mínimos aos valores conhecidos.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é o conjunto de valores de y já conhecidos na relação y = mx + b"
			},
			{
				name: "val_conhecidos_x",
				description: "é um conjunto opcional de valores de x que já deve conhecer da relação y = mx + b"
			},
			{
				name: "constante",
				description: "é um valor lógico: a constante b é calculada normalmente se constante = VERDADEIRO ou omisso; b é definido como igual a 0, se constante = FALSO"
			},
			{
				name: "estatística",
				description: "é um valor lógico: devolve estatísticas adicionais de regressão = VERDADEIRO; devolve m-coeficientes e a constante b = FALSO ou omisso"
			}
		]
	},
	{
		name: "PROJ.LOG",
		description: "Devolve estatísticas que descrevem uma curva exponencial, de modo a ajustar-se aos dados.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é o conjunto de valores de y já conhecidos na relação y = b*m^x"
			},
			{
				name: "val_conhecidos_x",
				description: "é um conjunto opcional de valores de x já conhecidos na relação y = b*m^x"
			},
			{
				name: "constante",
				description: "é um valor lógico: a constante b é calculada normalmente se constante = VERDADEIRO ou omisso; b é definido como igual a 1, se constante = FALSO"
			},
			{
				name: "estatística",
				description: "é um valor lógico: devolve a estatística de regressão adicional = VERDADEIRO; devolve m-coeficientes e a constante b = FALSO ou omisso"
			}
		]
	},
	{
		name: "QUARTIL",
		description: "Devolve o quartil de um conjunto de dados.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou o intervalo de células de valores numéricos cujo valor quartil pretende obter"
			},
			{
				name: "quarto",
				description: "é um número: valor mínimo = 0; primeiro quartil = 1; valor da mediana = 2; terceiro quartil = 3; valor máximo = 4"
			}
		]
	},
	{
		name: "QUARTIL.EXC",
		description: "Devolve o quartil de um conjunto de dados, baseado em valores de percentil de 0..1, exclusive.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou o intervalo de células de valores numéricos cujo valor quartil pretende obter"
			},
			{
				name: "quarto",
				description: "é um número: valor mínimo = 0; primeiro quartil = 1; valor da mediana = 2; terceiro quartil = 3; valor máximo = 4"
			}
		]
	},
	{
		name: "QUARTIL.INC",
		description: "Devolve o quartil de um conjunto de dados, baseado em valores de percentil de 0..1, inclusive.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou o intervalo de células de valores numéricos cujo valor quartil pretende obter"
			},
			{
				name: "quarto",
				description: "é um número: valor mínimo = 0; primeiro quartil = 1; valor da mediana = 2; terceiro quartil = 3; valor máximo = 4"
			}
		]
	},
	{
		name: "QUOCIENTE",
		description: "Devolve a parte inteira de uma divisão.",
		arguments: [
			{
				name: "numerador",
				description: "é o dividendo"
			},
			{
				name: "denominador",
				description: "é o divisor"
			}
		]
	},
	{
		name: "RADIANOS",
		description: "Converte graus em radianos.",
		arguments: [
			{
				name: "ângulo",
				description: "é um ângulo em graus que deseja converter"
			}
		]
	},
	{
		name: "RAIZPI",
		description: "Devolve a raiz quadrada de um número a multiplicar por Pi.",
		arguments: [
			{
				name: "núm",
				description: "é o número pelo qual p é multiplicado"
			}
		]
	},
	{
		name: "RAIZQ",
		description: "Devolve a raiz quadrada de um número.",
		arguments: [
			{
				name: "núm",
				description: "é o número para o qual deseja calcular a raiz quadrada"
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
		name: "RECEBER",
		description: "Devolve a quantia recebida no vencimento de um título totalmente investido.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número de série de data"
			},
			{
				name: "investimento",
				description: "é a quantia investida no título"
			},
			{
				name: "desconto",
				description: "é a taxa de desconto do título"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a utilizar"
			}
		]
	},
	{
		name: "REPETIR",
		description: "Repete o texto um dado número de vezes. Utilize REPETIR para preencher uma célula com um número de ocorrências da cadeia de texto.",
		arguments: [
			{
				name: "texto",
				description: "é o texto que deseja repetir"
			},
			{
				name: "núm_vezes",
				description: "é um número positivo que especifica o número de vezes que texto deve ser repetido"
			}
		]
	},
	{
		name: "RESTO",
		description: "Devolve o resto depois de um número ser dividido por Divisor.",
		arguments: [
			{
				name: "núm",
				description: "é o número para o qual deseja determinar o resto depois de executar a divisão"
			},
			{
				name: "divisor",
				description: "é o número pelo qual deseja dividir Núm"
			}
		]
	},
	{
		name: "ROMANO",
		description: "Converte um número árabe em romano, como texto.",
		arguments: [
			{
				name: "núm",
				description: "é o número árabe que pretende converter"
			},
			{
				name: "forma",
				description: "é o número que especifica o tipo de número romano pretendido."
			}
		]
	},
	{
		name: "RQUAD",
		description: "Devolve o quadrado do coeficiente de correlação do momento do produto de Pearson através dos pontos dados.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é uma matriz ou intervalo de pontos de dados e podem ser números ou nomes, matrizes ou referências que contenham números"
			},
			{
				name: "val_conhecidos_x",
				description: "é uma matriz ou intervalo de pontos de dados e podem ser números ou nomes, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "RTD",
		description: "Obtém dados em tempo real a partir de um programa que suporta automatização COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "IDprog",
				description: "é o nome de IDProg de um suplemento de automatização COM registado. Inclua o nome entre aspas"
			},
			{
				name: "servidor",
				description: "é o nome do servidor onde o suplemento deve ser executado. Inclua o nome entre aspas. Se o suplemento for executado localmente, utilize uma cadeia vazia"
			},
			{
				name: "tópico1",
				description: "são parâmetros de 1 a 38 que especificam dados"
			},
			{
				name: "tópico2",
				description: "são parâmetros de 1 a 38 que especificam dados"
			}
		]
	},
	{
		name: "SE",
		description: "Devolve um valor se a condição especificada equivaler a VERDADEIRO e outro valor se equivaler a FALSO.",
		arguments: [
			{
				name: "teste_lógico",
				description: "é qualquer valor ou expressão que pode ser avaliada como VERDADEIRO ou FALSO"
			},
			{
				name: "valor_se_verdadeiro",
				description: "é o valor devolvido se Teste_lógico for VERDADEIRO. Se for omisso, devolve VERDADEIRO. É possível aninhar até sete funções SE"
			},
			{
				name: "valor_se_falso",
				description: "é o valor devolvido se Teste_lógico for FALSO. Se omisso, é devolvido FALSO"
			}
		]
	},
	{
		name: "SE.ERRO",
		description: "Devolve valor_se_erro se a expressão for um erro e o valor da própria expressão não o for.",
		arguments: [
			{
				name: "valor",
				description: "é qualquer valor, expressão ou referência"
			},
			{
				name: "valor_se_erro",
				description: "é qualquer valor, expressão ou referência"
			}
		]
	},
	{
		name: "SEC",
		description: "Devolve a secante de um ângulo.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual quer obter a secante"
			}
		]
	},
	{
		name: "SECH",
		description: "Devolve a secante hiperbólica de um ângulo.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual quer obter a secante hiperbólica"
			}
		]
	},
	{
		name: "SEG.TEXTO",
		description: "Devolve um número específico de carateres de uma cadeia de texto, com início na posição especificada.",
		arguments: [
			{
				name: "texto",
				description: "é a cadeia de texto que contém os carateres que deseja extrair"
			},
			{
				name: "núm_inicial",
				description: "é a posição do primeiro caráter que deseja extrair. O primeiro caráter em Texto é 1"
			},
			{
				name: "núm_caract",
				description: "especifica quantos carateres devem ser devolvidos de Texto"
			}
		]
	},
	{
		name: "SEGUNDO",
		description: "Devolve os segundos, um número de 0 a 59.",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código de data e hora utilizado pelo Spreadsheet ou texto em formato de hora, tal como 16:48:23 ou 4:48:47 PM"
			}
		]
	},
	{
		name: "SELECCIONAR",
		description: "Seleciona um valor ou ação a executar a partir de uma lista de valores, baseada num número de índice.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm_índice",
				description: "especifica qual o argumento de valor selecionado; Núm_índice deve estar compreendido entre 1 e 254, ser uma fórmula ou uma referência a um número compreendido entre 1 e 254"
			},
			{
				name: "valor1",
				description: "são de 1 a 254 números, referências de células, nomes definidos, fórmulas, funções ou argumentos de texto a partir dos quais SELECIONAR faz a seleção"
			},
			{
				name: "valor2",
				description: "são de 1 a 254 números, referências de células, nomes definidos, fórmulas, funções ou argumentos de texto a partir dos quais SELECIONAR faz a seleção"
			}
		]
	},
	{
		name: "SEN",
		description: "Devolve o seno de um ângulo.",
		arguments: [
			{
				name: "núm",
				description: "é o ângulo em radianos para o qual deseja obter o seno. Graus * PI()/180 = radianos"
			}
		]
	},
	{
		name: "SEND",
		description: "Devolve o valor especificado se a expressão devolver #N/D, caso contrário devolve o resultado da expressão.",
		arguments: [
			{
				name: "valor",
				description: "é um valor, expressão ou referência"
			},
			{
				name: "valor_se_nd",
				description: "é valor, expressão ou referência"
			}
		]
	},
	{
		name: "SENH",
		description: "Devolve o seno hiperbólico de um número.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real"
			}
		]
	},
	{
		name: "SINAL",
		description: "Devolve o sinal de um número: 1, se o número for positivo, zero se o número for zero ou -1 se o número for negativo.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real"
			}
		]
	},
	{
		name: "SOMA",
		description: "Adiciona todos os números de um intervalo de células.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números a somar. Valores lógicos e texto são ignorados nas células, incluindo os introduzidos como argumentos"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números a somar. Valores lógicos e texto são ignorados nas células, incluindo os introduzidos como argumentos"
			}
		]
	},
	{
		name: "SOMA.SE",
		description: "Adiciona as células especificadas por uma determinada condição ou critério.",
		arguments: [
			{
				name: "intervalo",
				description: "é o intervalo de células que deseja avaliar"
			},
			{
				name: "critérios",
				description: "é a condição ou critério na forma de um número, expressão ou texto, que define quais as células a serem adicionadas"
			},
			{
				name: "intervalo_soma",
				description: "são as células a somar. Se omissas, utilizam-se as células do intervalo"
			}
		]
	},
	{
		name: "SOMA.SE.S",
		description: "Adiciona as células especificadas por um determinado conjunto de condições ou critérios.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "intervalo_soma",
				description: "são as células a somar."
			},
			{
				name: "intervalo_critérios",
				description: "é o intervalo de células que pretende avaliar para a condição específica"
			},
			{
				name: "critérios",
				description: "é a condição ou critério, sob a forma de um número, expressão ou texto, que define quais as células que serão adicionadas"
			}
		]
	},
	{
		name: "SOMARPRODUTO",
		description: "Devolve a soma dos produtos dos intervalos ou matrizes correspondentes.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matriz1",
				description: "são de 2 a 255 matrizes, cujos componentes deseja multiplicar e em seguida adicionar. As matrizes têm de ter todas as mesmas dimensões"
			},
			{
				name: "matriz2",
				description: "são de 2 a 255 matrizes, cujos componentes deseja multiplicar e em seguida adicionar. As matrizes têm de ter todas as mesmas dimensões"
			},
			{
				name: "matriz3",
				description: "são de 2 a 255 matrizes, cujos componentes deseja multiplicar e em seguida adicionar. As matrizes têm de ter todas as mesmas dimensões"
			}
		]
	},
	{
		name: "SOMARQUAD",
		description: "Devolve a soma dos quadrados dos argumentos. Os argumentos podem ser números, nomes, matrizes ou referências a células que contenham números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 números, matrizes, nomes ou referências cuja soma dos quadrados deseja obter"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 números, matrizes, nomes ou referências cuja soma dos quadrados deseja obter"
			}
		]
	},
	{
		name: "SOMASÉRIE",
		description: "Devolve a soma de uma série de potência baseada na fórmula.",
		arguments: [
			{
				name: "x",
				description: "é o valor de entrada para a série de potência"
			},
			{
				name: "n",
				description: "é a potência inicial para a qual se deseja elevar x"
			},
			{
				name: "m",
				description: "é o passo em que se acrescenta n a cada termo da série"
			},
			{
				name: "coeficientes",
				description: "é um conjunto de coeficientes pelo qual cada potência sucessiva de x será multiplicada"
			}
		]
	},
	{
		name: "SOMAX2DY2",
		description: "Soma as diferenças dos quadrados de números correspondentes em dois intervalos ou matrizes.",
		arguments: [
			{
				name: "matriz_x",
				description: "é o primeiro intervalo ou matriz de números e pode ser um número ou nome, matriz ou referência que contém números"
			},
			{
				name: "matriz_y",
				description: "é o segundo intervalo ou matriz de números e pode ser um número ou nome, matriz ou referência que contém números"
			}
		]
	},
	{
		name: "SOMAX2SY2",
		description: "Devolve o total das somas dos quadrados de números correspondentes em dois intervalos ou matrizes.",
		arguments: [
			{
				name: "matriz_x",
				description: "é o primeiro intervalo ou matriz de números e pode ser um número ou um nome, matriz ou referência que contém números"
			},
			{
				name: "matriz_y",
				description: "é o segundo intervalo ou matriz de números e pode ser um número ou nome, matriz ou referência que contenha números"
			}
		]
	},
	{
		name: "SOMAXMY2",
		description: "Soma os quadrados das diferenças de valores correspondentes em dois intervalos ou matrizes.",
		arguments: [
			{
				name: "matriz_x",
				description: "é o primeiro intervalo ou matriz de valores e pode ser um número ou nome, matriz ou referência que contenha números"
			},
			{
				name: "matriz_y",
				description: "é o segundo intervalo ou matriz de valores e pode ser um número ou nome, matriz ou referência que contenha números"
			}
		]
	},
	{
		name: "SUBST",
		description: "Substitui um texto existente por um novo numa cadeia de texto.",
		arguments: [
			{
				name: "texto",
				description: "é o texto ou a referência a uma célula que contém o texto cujos carateres deseja substituir"
			},
			{
				name: "texto_antigo",
				description: "é o texto existente que deseja substituir. Se as maiúsculas e minúsculas de Texto_antigo não correspondem às do texto, SUBSTITUIR não substituirá o texto"
			},
			{
				name: "texto_novo",
				description: "é o texto pelo qual deseja substituir Texto_antigo"
			},
			{
				name: "núm_ocorrência",
				description: "especifica qual a ocorrência de Texto_antigo que deseja substituir. Se omissa, substitui todas as ocorrências de Texto_antigo"
			}
		]
	},
	{
		name: "SUBSTITUIR",
		description: "Substitui parte de uma cadeia de texto por outra diferente.",
		arguments: [
			{
				name: "texto_antigo",
				description: "é o texto em que deseja substituir alguns carateres"
			},
			{
				name: "núm_inicial",
				description: "é a posição do caráter em Texto_antigo que deseja substituir por Texto_novo"
			},
			{
				name: "núm_caract",
				description: "é o número de carateres em Texto_antigo que deseja substituir"
			},
			{
				name: "texto_novo",
				description: "é o texto que substituirá carateres em Texto_antigo"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Devolve um subtotal numa lista ou base de dados.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm_função",
				description: "é o número de 1 a 11 que especifica a funcionalidade de resumo para o subtotal."
			},
			{
				name: "ref1",
				description: "são 1 a 254 intervalos ou referências para os quais pretende obter o subtotal"
			}
		]
	},
	{
		name: "T",
		description: "Verifica se o valor é texto e devolve o texto se referir a texto ou devolve aspas (texto em branco) se não for.",
		arguments: [
			{
				name: "valor",
				description: "é o valor a testar"
			}
		]
	},
	{
		name: "TAN",
		description: "Devolve a tangente de um ângulo.",
		arguments: [
			{
				name: "núm",
				description: "é o ângulo em radianos cuja tangente deseja obter. Graus * PI()/180 = radianos"
			}
		]
	},
	{
		name: "TANH",
		description: "Devolve a tangente hiperbólica de um número.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real"
			}
		]
	},
	{
		name: "TAXA",
		description: "Devolve a taxa de juros por período de um empréstimo ou um investimento. Por exemplo, utilize 6%/4 para pagamentos trimestrais a 6% APR.",
		arguments: [
			{
				name: "nper",
				description: "é o número total de períodos de pagamento do empréstimo ou investimento"
			},
			{
				name: "pgto",
				description: "é o pagamento efetuado em cada período; não é possível alterá-lo enquanto durar o empréstimo ou investimento"
			},
			{
				name: "va",
				description: "é o valor atual: o que o montante total de uma série de pagamentos futuros vale no presente"
			},
			{
				name: "vf",
				description: "é o valor futuro ou um saldo líquido que deseja atingir após o último pagamento ter sido efetuado. Se omisso, utiliza Fv = 0"
			},
			{
				name: "tipo",
				description: "é um valor lógico: pagamento no início do período = 1; pagamento no final do período = 0 ou omisso "
			},
			{
				name: "estimativa",
				description: "é a estimativa do valor da taxa; se omisso, Estimativa = 0,1 (10 por cento)"
			}
		]
	},
	{
		name: "TAXAJUROS",
		description: "Devolve a taxa de juros de um título totalmente investido.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número de série de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número de série de data"
			},
			{
				name: "investimento",
				description: "é a quantia investida no título"
			},
			{
				name: "resgate",
				description: "é a quantia a receber na data de vencimento"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a utilizar"
			}
		]
	},
	{
		name: "TEMPO",
		description: "Converte horas, minutos e segundos, correspondentes a números, num número de série do Spreadsheet, com um formato de hora.",
		arguments: [
			{
				name: "horas",
				description: "é um número de 0 a 23, que representa as horas"
			},
			{
				name: "minutos",
				description: "é um número de 0 a 59, que representa os minutos"
			},
			{
				name: "segundos",
				description: "é um número de 0 a 59, que representa os segundos"
			}
		]
	},
	{
		name: "TENDÊNCIA",
		description: "Devolve valores de uma tendência linear, baseada numa reta obtida por aplicação do método dos quadrados mínimos aos valores conhecidos.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é um intervalo ou matriz de valores de y já conhecidos na relação y = mx + b"
			},
			{
				name: "val_conhecidos_x",
				description: "é um intervalo opcional ou matriz de valores de x que já deve ser conhecido na relação y = mx + b, uma matriz com o mesmo tamanho de val_conhecidos_y"
			},
			{
				name: "novos_valores_x",
				description: "é um intervalo ou matriz de novos valores de x para os quais deseja que TENDÊNCIA devolva valores de y correspondentes"
			},
			{
				name: "constante",
				description: "é um valor lógico: a constante b é calculada normalmente se constante = VERDADEIRO ou omisso; b é definido igual a 0 se constante = FALSO"
			}
		]
	},
	{
		name: "TESTE.CHI",
		description: "Devolve o teste de independência: o valor da distribuição chi-quadrado para a estatística, com os graus de liberdade adequados.",
		arguments: [
			{
				name: "intervalo_real",
				description: "é o intervalo de dados que contém observações a testar perante os valores esperados"
			},
			{
				name: "intervalo_esperado",
				description: "é o intervalo de dados que contém a proporção entre o produto do total de linhas e do total de colunas e o total geral"
			}
		]
	},
	{
		name: "TESTE.CHIQ",
		description: "Devolve o teste de independência: o valor da distribuição chi-quadrado para a estatística, com os graus de liberdade adequados.",
		arguments: [
			{
				name: "intervalo_real",
				description: "é o intervalo de dados que contém observações a testar face aos valores esperados"
			},
			{
				name: "intervalo_esperado",
				description: "é o intervalo de dados que contém a proporção entre o produto do total de linhas e do total de colunas e o total geral"
			}
		]
	},
	{
		name: "TESTE.F",
		description: "Devolve o resultado de um teste F, a probabilidade bicaudal de as variações de Matriz1 e Matriz2 não serem significativamente diferentes.",
		arguments: [
			{
				name: "matriz1",
				description: "é a primeira matriz ou intervalo de dados e podem ser números ou nomes, matrizes ou referências que contenham números (valores em branco são ignorados)"
			},
			{
				name: "matriz2",
				description: "é a segunda matriz ou intervalo de dados e podem ser números ou nomes, matrizes ou referências que contenham números (valores em branco são ignorados)"
			}
		]
	},
	{
		name: "TESTE.T",
		description: "Devolve a probabilidade associada ao teste t de Student.",
		arguments: [
			{
				name: "matriz1",
				description: "é o primeiro conjunto de dados"
			},
			{
				name: "matriz2",
				description: "é o segundo conjunto de dados"
			},
			{
				name: "caudas",
				description: "especifica o número de caudas da distribuição a devolver: distribuição unicaudal = 1; distribuição bicaudal = 2"
			},
			{
				name: "tipo",
				description: "é o tipo de teste t: emparelhada = 1, duas amostras de variância igual (homocedásticas) = 2, duas amostras de variância desigual = 3"
			}
		]
	},
	{
		name: "TESTE.Z",
		description: "Devolve o valor P unicaudal de um teste z.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados com que X será testado"
			},
			{
				name: "x",
				description: "é o valor a testar"
			},
			{
				name: "sigma",
				description: "é o desvio-padrão (conhecido) da população. Se omisso, é utilizado o desvio-padrão da amostra"
			}
		]
	},
	{
		name: "TESTEF",
		description: "Devolve o resultado de um teste F, a probabilidade bicaudal de as variações de Matriz1 e Matriz2 não serem significativamente diferentes.",
		arguments: [
			{
				name: "matriz1",
				description: "é a primeira matriz ou intervalo de dados e podem ser números ou nomes, matrizes ou referências que contenham números (valores em branco são ignorados)"
			},
			{
				name: "matriz2",
				description: "é a segunda matriz ou intervalo de dados e podem ser números ou nomes, matrizes ou referências que contenham números (valores em branco são ignorados)"
			}
		]
	},
	{
		name: "TESTET",
		description: "Devolve a probabilidade associada ao teste t de Student.",
		arguments: [
			{
				name: "matriz1",
				description: "é o primeiro conjunto de dados"
			},
			{
				name: "matriz2",
				description: "é o segundo conjunto de dados"
			},
			{
				name: "caudas",
				description: "especifica o número de caudas de distribuição a devolver: distribuição unicaudal = 1; distribuição bicaudal = 2"
			},
			{
				name: "tipo",
				description: "é o tipo de teste t: emparelhada = 1, duas amostras de variância igual (homocedásticas) = 2, duas amostras de variância desigual = 3"
			}
		]
	},
	{
		name: "TESTEZ",
		description: "Devolve o valor P unicaudal de um teste z.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados com que X será testado"
			},
			{
				name: "x",
				description: "é o valor a testar"
			},
			{
				name: "sigma",
				description: "é o desvio-padrão (conhecido) da população. Se omisso, é utilizado o desvio-padrão da amostra"
			}
		]
	},
	{
		name: "TEXTO",
		description: "Converte um valor para texto num determinado formato numérico.",
		arguments: [
			{
				name: "valor",
				description: "é um número, uma fórmula que representa um valor numérico ou uma referência a uma célula que contém um valor numérico"
			},
			{
				name: "formato_texto",
				description: "é um formato numérico em forma de texto da caixa 'Categoria', no separador 'Número' da caixa de diálogo 'Formatar células' (outro que não 'Geral')"
			}
		]
	},
	{
		name: "TEXTO.BAHT",
		description: "Converte um número em texto (baht).",
		arguments: [
			{
				name: "número",
				description: "é um número que deseja converter"
			}
		]
	},
	{
		name: "TIPO",
		description: "Devolve um número que indica o tipo de dados de um valor: número = 1; texto = 2; valor lógico = 4; valor de erro = 16; matriz = 64.",
		arguments: [
			{
				name: "valor",
				description: "pode ser qualquer valor"
			}
		]
	},
	{
		name: "TIPO.ERRO",
		description: "Devolve um número que corresponde a um valor de erro.",
		arguments: [
			{
				name: "val_erro",
				description: "é o valor de erro cujo número de identificação pretende e pode ser um valor de erro real ou uma referência a uma célula que contém um valor de erro"
			}
		]
	},
	{
		name: "TIR",
		description: "Devolve a taxa interna de rentabilidade de uma série de fluxos monetários.",
		arguments: [
			{
				name: "valores",
				description: "é uma matriz ou uma referência a células que contêm números cuja taxa interna de rentabilidade deseja calcular"
			},
			{
				name: "estimativa",
				description: "é um número que se estima ser próximo do resultado de TIR; 0,1 (10 por cento), se omisso"
			}
		]
	},
	{
		name: "TRANSPOR",
		description: "Converte um intervalo vertical de células para um intervalo horizontal, ou vice-versa.",
		arguments: [
			{
				name: "matriz",
				description: "é um intervalo de células na folha de cálculo ou uma matriz de valores que deseja transpor"
			}
		]
	},
	{
		name: "TRUNCAR",
		description: "Trunca um número, tornando-o um número inteiro, ao remover a sua parte decimal ou fraccional.",
		arguments: [
			{
				name: "núm",
				description: "é o número que deseja truncar"
			},
			{
				name: "núm_dígitos",
				description: "é um número que especifica a precisão da truncagem; se omisso, é 0 (zero)"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Devolve o número (ponto de código) correspondente ao primeiro caráter do texto.",
		arguments: [
			{
				name: "texto",
				description: "é o caráter para o qual quer obter o valor Unicode"
			}
		]
	},
	{
		name: "UNIDM",
		description: "Devolve a matriz identidade para a dimensão especificada.",
		arguments: [
			{
				name: "dimensão",
				description: "é um número inteiro que especifica a dimensão da matriz identidade que quer devolver"
			}
		]
	},
	{
		name: "VA",
		description: "Devolve o valor atual de um investimento: o montante total que vale agora uma série de pagamentos futuros.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juro por período. Por exemplo, utilize 6%/4 para pagamentos trimestrais a 6% APR"
			},
			{
				name: "nper",
				description: "é o número total de períodos de pagamento de um investimento"
			},
			{
				name: "pgto",
				description: "é o pagamento efetuado em cada período; não é possível alterá-lo enquanto durar o investimento"
			},
			{
				name: "vf",
				description: "é o valor futuro ou o saldo em dinheiro que deseja atingir após o último pagamento ter sido efetuado"
			},
			{
				name: "tipo",
				description: "é um valor lógico: pagamento no início do período = 1; pagamento no final do período = 0 ou omisso"
			}
		]
	},
	{
		name: "VAL",
		description: "Devolve o valor atual líquido de um investimento, com uma taxa de desconto e uma série de pagamentos futuros (valor negativo) e rendimentos (valor positivo).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de desconto ao longo de um período"
			},
			{
				name: "valor1",
				description: "são de 1 a 254 pagamentos e rendimentos, igualmente espaçados no tempo e ocorrendo no final de cada período"
			},
			{
				name: "valor2",
				description: "são de 1 a 254 pagamentos e rendimentos, igualmente espaçados no tempo e ocorrendo no final de cada período"
			}
		]
	},
	{
		name: "VALOR",
		description: "Converte num número uma cadeia de texto que representa um número.",
		arguments: [
			{
				name: "texto",
				description: "é o texto entre aspas ou a referência a uma célula que contém o texto que deseja converter"
			}
		]
	},
	{
		name: "VALOR.NÚMERO",
		description: "Converte o texto em números independentemente da região.",
		arguments: [
			{
				name: "texto",
				description: "é a cadeia representante do número que quer converter"
			},
			{
				name: "separador_decimal",
				description: "é o caráter usado como separador decimal na cadeia"
			},
			{
				name: "separador_grupo",
				description: "é o caráter usado como separador de grupo na cadeia"
			}
		]
	},
	{
		name: "VALOR.TEMPO",
		description: "Converte uma hora de texto num número de série do Spreadsheet para uma hora, um número de 0 (00:00:00) a 0,999988426 (23:59:59). Formate o número com um formato de hora depois de introduzir a fórmula.",
		arguments: [
			{
				name: "texto_hora",
				description: "é uma cadeia de texto que dá as horas em qualquer dos formatos de hora do Spreadsheet (as informações de data na cadeia é ignorada)"
			}
		]
	},
	{
		name: "VAR",
		description: "Calcula a variância a partir de uma amostra (ignora valores lógicos e texto da amostra).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 argumentos numéricos correspondentes a uma amostra de uma população"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 argumentos numéricos correspondentes a uma amostra de uma população"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Calcula a variância a partir da população total (ignora valores lógicos e texto da população).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 argumentos numéricos correspondentes a uma população"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 argumentos numéricos correspondentes a uma população"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Calcula a variância a partir de uma amostra (ignora valores lógicos e texto da amostra).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 argumentos numéricos correspondentes a uma amostra de uma população"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 argumentos numéricos correspondentes a uma amostra de uma população"
			}
		]
	},
	{
		name: "VARA",
		description: "Calcula a variância a partir de uma amostra, incluindo valores lógicos e texto. Texto e o valor lógico FALSO = 0; o valor lógico VERDADEIRO = 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "são de 1 a 255 argumentos de valores correspondentes a uma amostra de uma população"
			},
			{
				name: "valor2",
				description: "são de 1 a 255 argumentos de valores correspondentes a uma amostra de uma população"
			}
		]
	},
	{
		name: "VARP",
		description: "Calcula a variância a partir da população total (ignora valores lógicos e texto da população).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 argumentos numéricos correspondentes a uma população"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 argumentos numéricos correspondentes a uma população"
			}
		]
	},
	{
		name: "VARPA",
		description: "Calcula a variância com base na população total, incluindo valores lógicos e texto. Texto e o valor lógico FALSO = 0; o valor lógico VERDADEIRO = 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "são de 1 a 255 argumentos correspondentes a uma população"
			},
			{
				name: "valor2",
				description: "são de 1 a 255 argumentos correspondentes a uma população"
			}
		]
	},
	{
		name: "VERDADEIRO",
		description: "Devolve o valor lógico VERDADEIRO.",
		arguments: [
		]
	},
	{
		name: "VF",
		description: "Devolve o valor futuro de um investimento a partir de pagamentos periódicos constantes e de uma taxa de juros constante.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juro por período. Por exemplo, utilize 6%/4 para pagamentos trimestrais a 6% APR"
			},
			{
				name: "nper",
				description: "é o número total de períodos de pagamento de um investimento"
			},
			{
				name: "pgto",
				description: "é o pagamento efetuado em cada período; não é possível alterá-lo enquanto durar o investimento"
			},
			{
				name: "va",
				description: "é o valor atual ou a quantia global correspondente a uma série de pagamentos futuros. Se omisso, Va = 0"
			},
			{
				name: "tipo",
				description: "é um valor que representa o escalonamento de pagamentos: pagamento no início do período = 1; pagamento no final do período = 0 ou omisso"
			}
		]
	},
	{
		name: "VFPLANO",
		description: "Devolve o valor futuro de um capital inicial depois de ter sido aplicada uma série de taxas de juros compostos.",
		arguments: [
			{
				name: "capital",
				description: "é o valor presente"
			},
			{
				name: "plano",
				description: "é uma matriz de taxas de juros a aplicar"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Devolve a distribuição Weibull.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se avalia a função, um número não negativo"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, utilize VERDADEIRO; para a função de densidade de probabilidade, utilize FALSO"
			}
		]
	},
	{
		name: "XOU",
		description: "Devolve um 'Ou Exclusivo' lógico de todos os argumentos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "lógica1",
				description: "são condições de 1 a 254 que quer testar que podem ser VERDADEIRO ou FALSO e que podem ser matrizes, referências ou valores lógicos"
			},
			{
				name: "lógica2",
				description: "são condições de 1 a 254 que quer testar que podem ser VERDADEIRO ou FALSO e que podem ser matrizes, referências ou valores lógicos"
			}
		]
	},
	{
		name: "XTIR",
		description: "Devolve a taxa de rentabilidade interna de uma sucessão de fluxos monetários.",
		arguments: [
			{
				name: "valores",
				description: "é uma série de fluxos monetários que corresponde a um programa de pagamentos em datas"
			},
			{
				name: "datas",
				description: "é a sucessão de datas que corresponde aos pagamentos de fluxos monetários"
			},
			{
				name: "estimativa",
				description: "é um número que se estima próximo do resultado de XTIR"
			}
		]
	},
	{
		name: "XVAL",
		description: "Devolve o valor atual líquido de uma sucessão de fluxos monetários.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de desconto a aplicar nos fluxos monetários"
			},
			{
				name: "valores",
				description: "é uma série de fluxos monetários que corresponde a um programa de pagamentos em datas"
			},
			{
				name: "datas",
				description: "é o programa de datas de pagamento que corresponde aos pagamentos de fluxos monetários"
			}
		]
	}
];