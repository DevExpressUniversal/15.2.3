ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Devuelve el valor absoluto de un número, es decir, un número sin signo.",
		arguments: [
			{
				name: "número",
				description: "es el número real del que se desea obtener el valor absoluto"
			}
		]
	},
	{
		name: "ACOS",
		description: "Devuelve el arcoseno de un número, en radianes, dentro del intervalo de 0 a Pi. El arcoseno es el ángulo cuyo coseno es Número.",
		arguments: [
			{
				name: "número",
				description: "es el coseno del ángulo deseado y debe estar entre -1 y 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Devuelve el coseno hiperbólico inverso de un número.",
		arguments: [
			{
				name: "número",
				description: "es un número real y debe ser mayor o igual que 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Devuelve el arco tangente de un número en radianes dentro del rango de 0 a Pi.",
		arguments: [
			{
				name: "número",
				description: "es la cotangente del ángulo que quieres"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Devuelve la cotangente hiperbólica inversa de un número.",
		arguments: [
			{
				name: "número",
				description: "Es la cotangente hiperbólica del ángulo que quieres"
			}
		]
	},
	{
		name: "AHORA",
		description: "Devuelve la fecha y hora actuales con formato de fecha y hora.",
		arguments: [
		]
	},
	{
		name: "ALEATORIO",
		description: "Devuelve un número aleatorio mayor o igual que 0 y menor que 1, distribuido (cambia al actualizarse).",
		arguments: [
		]
	},
	{
		name: "ALEATORIO.ENTRE",
		description: "Devuelve el número aleatorio entre los números que especifique.",
		arguments: [
			{
				name: "inferior",
				description: "es el entero más pequeño que devolverá ALEATORIO.ENTRE"
			},
			{
				name: "superior",
				description: "es el entero más grande que devolverá ALEATORIO.ENTRE"
			}
		]
	},
	{
		name: "AÑO",
		description: "Devuelve el año, un número entero en el rango 1900-9999.",
		arguments: [
			{
				name: "núm_de_serie",
				description: "es un número en el código de fecha y hora usado por Spreadsheet"
			}
		]
	},
	{
		name: "AREAS",
		description: "Devuelve el número de áreas de una referencia. Un área es un rango de celdas contiguas o una única celda.",
		arguments: [
			{
				name: "ref",
				description: "es una referencia a una celda o rango de celdas y puede referirse a áreas múltiples"
			}
		]
	},
	{
		name: "ASENO",
		description: "Devuelve el arcoseno de un número en radianes, dentro del intervalo -Pi/2 a Pi/2.",
		arguments: [
			{
				name: "número",
				description: "es el seno del ángulo deseado y debe estar entre -1 y 1"
			}
		]
	},
	{
		name: "ASENOH",
		description: "Devuelve el seno hiperbólico inverso de un número.",
		arguments: [
			{
				name: "número",
				description: "es un número real y debe ser mayor o igual que 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Devuelve el arco tangente de un número en radianes, dentro del intervalo -Pi/2 a Pi/2.",
		arguments: [
			{
				name: "número",
				description: "es la tangente del ángulo deseado"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Devuelve el arco tangente de las coordenadas X e Y especificadas, en un valor en radianes comprendido entre -Pi y Pi, excluyendo -Pi.",
		arguments: [
			{
				name: "coord_x",
				description: "es la coordenada X del punto"
			},
			{
				name: "coord_y",
				description: "es la coordenada Y del punto"
			}
		]
	},
	{
		name: "ATANH",
		description: "Devuelve la tangente hiperbólica inversa de un número.",
		arguments: [
			{
				name: "número",
				description: "es un número real que debe ser mayor que -1 y menor que 1"
			}
		]
	},
	{
		name: "BASE",
		description: "Convierte un número en una representación de texto con la base dada.",
		arguments: [
			{
				name: "número",
				description: "Es el número que quieres convertir"
			},
			{
				name: "raíz",
				description: "Es la base en la que quieres que se convierta el número"
			},
			{
				name: "longitud_mín",
				description: "Es la longitud mínima de la cadena devuelta. Si se omite, no se agregan ceros iniciales"
			}
		]
	},
	{
		name: "BDCONTAR",
		description: "Cuenta las celdas que contienen números en el campo (columna) de registros de la base de datos que cumplen las condiciones especificadas.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BDCONTARA",
		description: "Cuenta el número de celdas que no están en blanco en el campo (columna) de los registros de la base de datos que cumplen las condiciones especificadas.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BDDESVEST",
		description: "Calcula la desviación estándar basándose en una muestra de las entradas seleccionadas de una base de datos.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BDDESVESTP",
		description: "Calcula la desviación estándar basándose en la población total de las entradas seleccionadas de una base de datos.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BDEXTRAER",
		description: "Extrae de una base de datos un único registro que coincide con las condiciones especificadas.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BDMAX",
		description: "Devuelve el número máximo en el campo (columna) de registros de la base de datos que coinciden con las condiciones especificadas.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BDMIN",
		description: "Devuelve el número menor del campo (columna) de registros de la base de datos que coincide con las condiciones especificadas.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BDPRODUCTO",
		description: "Multiplica los valores del campo (columna) de registros en la base de datos que coinciden con las condiciones especificadas.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BDPROMEDIO",
		description: "Obtiene el promedio de los valores de una columna, lista o base de datos que cumplen las condiciones especificadas.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BDSUMA",
		description: "Suma los números en el campo (columna) de los registros que coinciden con las condiciones especificadas.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BDVAR",
		description: "Calcula la varianza basándose en una muestra de las entradas seleccionadas de una base de datos.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BDVARP",
		description: "Calcula la varianza basándose en la población total de las entradas seleccionadas de una base de datos.",
		arguments: [
			{
				name: "base_de_datos",
				description: "es el rango de celdas que compone la lista o base de datos. Una base de datos es una lista de datos relacionados"
			},
			{
				name: "nombre_de_campo",
				description: "es el rótulo entre comillas dobles de la columna o un número que representa la posición de la columna en la lista"
			},
			{
				name: "criterios",
				description: "es el rango de celdas que contiene las condiciones especificadas. El rango incluye un rótulo de columna y una celda bajo el rótulo para una condición"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Devuelve la función Bessel In(x) modificada.",
		arguments: [
			{
				name: "x",
				description: "es el valor en el que se evalúa la función"
			},
			{
				name: "n",
				description: "es el orden de la función Bessel"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Devuelve la función Bessel Jn(x).",
		arguments: [
			{
				name: "x",
				description: "es el valor en el que se evalúa la función"
			},
			{
				name: "n",
				description: "es el orden de la función Bessel"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Devuelve la función Bessel Kn(x) modificada.",
		arguments: [
			{
				name: "x",
				description: "es el valor en el que se evalúa la función"
			},
			{
				name: "n",
				description: "es el orden de la función"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Devuelve la función Bessel Yn(x).",
		arguments: [
			{
				name: "x",
				description: "es el valor en el que se evalúa la función"
			},
			{
				name: "n",
				description: "es el orden de la función"
			}
		]
	},
	{
		name: "BIN.A.DEC",
		description: "Convierte un número binario en decimal.",
		arguments: [
			{
				name: "número",
				description: "es el número binario que desea convertir"
			}
		]
	},
	{
		name: "BIN.A.HEX",
		description: "Convierte un número binario en hexadecimal.",
		arguments: [
			{
				name: "número",
				description: "es el número binario que desea convertir"
			},
			{
				name: "posiciones",
				description: "es el número de caracteres que se deben usar"
			}
		]
	},
	{
		name: "BIN.A.OCT",
		description: "Convierte un número binario en octal.",
		arguments: [
			{
				name: "número",
				description: "es el número binario que desea convertir"
			},
			{
				name: "posiciones",
				description: "es el número de caracteres que se deben usar"
			}
		]
	},
	{
		name: "BINOM.CRIT",
		description: "Devuelve el menor valor cuya distribución binomial acumulativa es mayor o igual que un valor de criterio.",
		arguments: [
			{
				name: "ensayos",
				description: "es el número de ensayos de Bernoulli"
			},
			{
				name: "prob_éxito",
				description: "es la probabilidad de éxito en cada ensayo, un número entre 0 y 1 inclusive"
			},
			{
				name: "alfa",
				description: "es el valor de criterio, un número entre 0 y 1 inclusive"
			}
		]
	},
	{
		name: "BIT.DESPLDCHA",
		description: "Devuelve un número desplazado a la derecha por bits de desplazamiento.",
		arguments: [
			{
				name: "número",
				description: "Es la representación decimal del número binario que quiere evaluar"
			},
			{
				name: "cambio_cantidad",
				description: "Es la cantidad de bits que quiere desplazar el número hacia la derecha"
			}
		]
	},
	{
		name: "BIT.DESPLIZQDA",
		description: "Devuelve un número desplazado a la izquierda por bits de desplazamiento.",
		arguments: [
			{
				name: "número",
				description: "Es la representación decimal del número binario que quiere evaluar"
			},
			{
				name: "cambio_cantidad",
				description: "Es la cantidad de bits que quiere desplazar el número hacia la izquierda"
			}
		]
	},
	{
		name: "BIT.O",
		description: "Devuelve un bit a bit 'Or' de dos números.",
		arguments: [
			{
				name: "número1",
				description: "Es la representación decimal del número binario que quieres evaluar"
			},
			{
				name: "número2",
				description: "Es la representación decimal del número binario que quieres evaluar"
			}
		]
	},
	{
		name: "BIT.XO",
		description: "Devuelve un bit a bit 'Exclusive Or' de dos números.",
		arguments: [
			{
				name: "número1",
				description: "Es la representación decimal del número binario que quieres evaluar"
			},
			{
				name: "número2",
				description: "Es la representación decimal del número binario que quieres evaluar"
			}
		]
	},
	{
		name: "BIT.Y",
		description: "Devuelve un bit a bit 'And' de dos números.",
		arguments: [
			{
				name: "número1",
				description: "Es la representación decimal del número binario que quieres evaluar"
			},
			{
				name: "número2",
				description: "Es la representación decimal del número binario que quieres evaluar"
			}
		]
	},
	{
		name: "BUSCAR",
		description: "Busca valores de un rango de una columna o una fila o desde una matriz. Proporcionado para compatibilidad con versiones anteriores.",
		arguments: [
			{
				name: "valor_buscado",
				description: "es un valor que BUSCAR busca en vector_de_comparación y puede ser un número, texto, un valor lógico o un nombre o referencia a un valor"
			},
			{
				name: "vector_de_comparación",
				description: "es un rango que solo contiene una columna o una fila de texto, números o valores lógicos, en orden ascendente"
			},
			{
				name: "vector_resultado",
				description: "es un rango que solo contiene una columna o una fila, del mismo tamaño que vector_de_comparación"
			}
		]
	},
	{
		name: "BUSCARH",
		description: "Busca en la primera fila de una tabla o matriz de valores y devuelve el valor en la misma columna desde una fila especificada.",
		arguments: [
			{
				name: "valor_buscado",
				description: "es el valor que se busca en la primera fila de la tabla y puede ser un valor, una referencia o una cadena de texto"
			},
			{
				name: "matriz_buscar_en",
				description: "es una tabla de texto, números o valores lógicos en los que se buscan los datos. Tabla_matriz puede ser una referencia a un rango o un nombre de rango"
			},
			{
				name: "indicador_filas",
				description: "es el número de fila en tabla_matriz desde el cual se deberá devolver el valor coincidente. La primera fila de valores en la tabla es la fila 1"
			},
			{
				name: "ordenado",
				description: "es un valor lógico: para encontrar la coincidencia más cercana en la fila superior (ordenada de forma ascendente) = VERDADERO u omitido; para encontrar coincidencia exacta = FALSO"
			}
		]
	},
	{
		name: "BUSCARV",
		description: "Busca un valor en la primera columna de la izquierda de una tabla y luego devuelve un valor en la misma fila desde una columna especificada. De forma predeterminada, la tabla se ordena de forma ascendente.",
		arguments: [
			{
				name: "valor_buscado",
				description: "es el valor buscado en la primera columna de la tabla y puede ser un valor, referencia o una cadena de texto"
			},
			{
				name: "matriz_buscar_en",
				description: "es una tabla de texto, números o valores lógicos en los cuales se recuperan datos. Matriz_buscar_en puede ser una referencia a un rango o un nombre de rango"
			},
			{
				name: "indicador_columnas",
				description: "es el número de columna de matriz_buscar_en desde la cual debe devolverse el valor que coincida. La primera columna de valores en la tabla es la columna 1"
			},
			{
				name: "ordenado",
				description: "es un valor lógico: para encontrar la coincidencia más cercana en la primera columna (ordenada de forma ascendente) = VERDADERO u omitido; para encontrar la coincidencia exacta = FALSO"
			}
		]
	},
	{
		name: "CANTIDAD.RECIBIDA",
		description: "Devuelve la cantidad recibida al vencimiento para un valor bursátil completamente invertido.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de vencimiento del valor bursátil expresada como número de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de liquidación del valor bursátil, expresado como número de fecha de serie"
			},
			{
				name: "inversión",
				description: "es la cantidad invertida en el valor bursátil"
			},
			{
				name: "descuento",
				description: "es la tasa de descuento del valor bursátil"
			},
			{
				name: "base",
				description: "determina en qué tipo de base deben contarse los días"
			}
		]
	},
	{
		name: "CARACTER",
		description: "Devuelve el carácter especificado por el número de código a partir del juego de caracteres establecido en su PC.",
		arguments: [
			{
				name: "número",
				description: "es un número entre 1 y 255 que especifica el carácter deseado"
			}
		]
	},
	{
		name: "CELDA",
		description: "Devuelve información acerca del formato, ubicación o contenido de la primera celda, según el orden de lectura de la hoja, en una referencia.",
		arguments: [
			{
				name: "tipo_de_info",
				description: "es un valor de texto que especifica el tipo de información que desea obtener de la celda."
			},
			{
				name: "ref",
				description: "es la celda acerca de la cual se desea obtener información"
			}
		]
	},
	{
		name: "COCIENTE",
		description: "Devuelve la parte entera de una división.",
		arguments: [
			{
				name: "numerador",
				description: "es el dividendo"
			},
			{
				name: "denominador",
				description: "es el divisor"
			}
		]
	},
	{
		name: "CODIGO",
		description: "Devuelve el número de código del primer carácter del texto del juego de caracteres usados por su PC.",
		arguments: [
			{
				name: "texto",
				description: "es el texto del que se desea obtener el código del primer carácter"
			}
		]
	},
	{
		name: "COEF.DE.CORREL",
		description: "Devuelve el coeficiente de correlación de dos conjuntos de datos.",
		arguments: [
			{
				name: "matriz1",
				description: "es un rango de celdas de valores. Los valores deben ser números, nombres, matrices o referencias que contengan números"
			},
			{
				name: "matriz2",
				description: "es un segundo rango de celdas de valores. Los valores deben ser números, nombres, matrices o referencias que contengan números"
			}
		]
	},
	{
		name: "COEFICIENTE.ASIMETRIA",
		description: "Devuelve el sesgo de una distribución: una caracterización del grado de asimetría de una distribución alrededor de su media.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números o nombres, matrices o referencias que contienen números para los cuales desea conocer el sesgo"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números o nombres, matrices o referencias que contienen números para los cuales desea conocer el sesgo"
			}
		]
	},
	{
		name: "COEFICIENTE.ASIMETRIA.P",
		description: "Devuelve el sesgo de una distribución basado en una población: una caracterización del grado de asimetría de una distribución alrededor de su media.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "Son los números del 1 al 254 o nombres, matrices o referencias que contienen números para los que quieres saber el sesgo de población"
			},
			{
				name: "número2",
				description: "Son los números del 1 al 254 o nombres, matrices o referencias que contienen números para los que quieres saber el sesgo de población"
			}
		]
	},
	{
		name: "COEFICIENTE.R2",
		description: "Devuelve el cuadrado del coeficiente del momento de correlación del producto Pearson de los puntos dados.",
		arguments: [
			{
				name: "conocido_y",
				description: "es una matriz, o rango, de puntos de datos, formada por: números, nombres, matrices o referencias que contengan números"
			},
			{
				name: "conocido_x",
				description: "es una matriz, o rango, de puntos de datos, formada por: números, nombres, matrices o referencias que contengan números"
			}
		]
	},
	{
		name: "COINCIDIR",
		description: "Devuelve la posición relativa de un elemento en una matriz, que coincide con un valor dado en un orden especificado.",
		arguments: [
			{
				name: "valor_buscado",
				description: "es el valor que se usa para encontrar el valor deseado en la matriz y puede ser un número, texto, valor lógico o una referencia a uno de ellos"
			},
			{
				name: "matriz_buscada",
				description: "es un rango contiguo de celdas que contienen posibles valores de búsqueda, una matriz de valores o una referencia a una matriz"
			},
			{
				name: "tipo_de_coincidencia",
				description: "es un número 1, 0, -1 que indica el valor que se devolverá."
			}
		]
	},
	{
		name: "COLUMNA",
		description: "Devuelve el número de columna de una referencia.",
		arguments: [
			{
				name: "ref",
				description: "es la celda o rango de celdas contiguas de las cuales se desea obtener el número de columna. Si esta referencia se omite, se usará la celda con la función COLUMNA"
			}
		]
	},
	{
		name: "COLUMNAS",
		description: "Devuelve el número de columnas en una matriz o referencia.",
		arguments: [
			{
				name: "matriz",
				description: "es una matriz, fórmula matricial o una referencia a un rango de celdas de las que se desea obtener el número de columnas"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Devuelve la cantidad de combinaciones con repeticiones de una cantidad determinada de elementos.",
		arguments: [
			{
				name: "número",
				description: "Es la cantidad total de elementos"
			},
			{
				name: "número_elegido",
				description: "Es la cantidad de elementos en cada combinación"
			}
		]
	},
	{
		name: "COMBINAT",
		description: "Devuelve el número de combinaciones para un número determinado de elementos.",
		arguments: [
			{
				name: "número",
				description: "es el número total de elementos"
			},
			{
				name: "tamaño",
				description: "es el número de elementos en cada combinación"
			}
		]
	},
	{
		name: "COMPLEJO",
		description: "Convierte el coeficiente real e imaginario en un número complejo.",
		arguments: [
			{
				name: "núm_real",
				description: "es el coeficiente real de un número complejo"
			},
			{
				name: "i_núm",
				description: "es el coeficiente imaginario del número complejo"
			},
			{
				name: "sufijo",
				description: "es el sufijo para el componente imaginario del número complejo"
			}
		]
	},
	{
		name: "CONCATENAR",
		description: "Une varios elementos de texto en uno solo.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "texto1",
				description: "son entre 1 y 255 elementos de texto que se unirán en un solo elemento y que pueden ser texto, cadenas, números o referencias simples de celdas"
			},
			{
				name: "texto2",
				description: "son entre 1 y 255 elementos de texto que se unirán en un solo elemento y que pueden ser texto, cadenas, números o referencias simples de celdas"
			}
		]
	},
	{
		name: "CONTAR",
		description: "Cuenta el número de celdas de un rango que contienen números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "son de 1 a 255 argumentos que pueden contener o hacer referencia a distintos tipos de datos, pero solo se cuentan los números"
			},
			{
				name: "valor2",
				description: "son de 1 a 255 argumentos que pueden contener o hacer referencia a distintos tipos de datos, pero solo se cuentan los números"
			}
		]
	},
	{
		name: "CONTAR.BLANCO",
		description: "Cuenta el número de celdas en blanco dentro de un rango especificado.",
		arguments: [
			{
				name: "rango",
				description: "es el rango del que se desea contar el número de celdas en blanco"
			}
		]
	},
	{
		name: "CONTAR.SI",
		description: "Cuenta las celdas en el rango que coinciden con la condición dada.",
		arguments: [
			{
				name: "rango",
				description: "es el rango del que se desea contar el número de celdas que no están en blanco"
			},
			{
				name: "criterio",
				description: "es la condición en forma de número, expresión o texto que determina qué celdas deben contarse"
			}
		]
	},
	{
		name: "CONTAR.SI.CONJUNTO",
		description: "Cuenta el número de celdas que cumplen un determinado conjunto de condiciones o criterios.",
		arguments: [
			{
				name: "rango_criterios",
				description: "es el rango de celdas que desea evaluar para la condición determinada"
			},
			{
				name: "criterio",
				description: "es la condición en forma de número, expresión o texto que determina qué celdas deben contarse"
			}
		]
	},
	{
		name: "CONTARA",
		description: "Cuenta el número de celdas no vacías de un rango.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "son de 1 a 255 argumentos que representan los valores y las celdas que desea contar. Los valores pueden ser cualquier tipo de información"
			},
			{
				name: "valor2",
				description: "son de 1 a 255 argumentos que representan los valores y las celdas que desea contar. Los valores pueden ser cualquier tipo de información"
			}
		]
	},
	{
		name: "CONV.DECIMAL",
		description: "Convierte una representación de texto de un número en una base dada en un número decimal.",
		arguments: [
			{
				name: "número",
				description: "Es el número que quieres convertir"
			},
			{
				name: "raíz",
				description: "Es la base del número que estás convirtiendo"
			}
		]
	},
	{
		name: "CONVERTIR",
		description: "Convierte un número de un sistema decimal a otro.",
		arguments: [
			{
				name: "número",
				description: "es el valor desde_unidades para convertir"
			},
			{
				name: "desde_unidad",
				description: "son las unidades para el número"
			},
			{
				name: "a_unidad",
				description: "son las unidades para el resultado"
			}
		]
	},
	{
		name: "COS",
		description: "Devuelve el coseno de un ángulo.",
		arguments: [
			{
				name: "número",
				description: "es el ángulo en radianes del que se desea obtener el coseno"
			}
		]
	},
	{
		name: "COSH",
		description: "Devuelve el coseno hiperbólico de un número.",
		arguments: [
			{
				name: "número",
				description: "es cualquier número real"
			}
		]
	},
	{
		name: "COT",
		description: "Devuelve la cotangente de un ángulo.",
		arguments: [
			{
				name: "número",
				description: "Es el ángulo en radianes del que quieres saber la cotangente"
			}
		]
	},
	{
		name: "COTH",
		description: "Devuelve la cotangente hiperbólica de un número.",
		arguments: [
			{
				name: "número",
				description: "Es el ángulo en radianes del que quieres saber la cotangente hiperbólica"
			}
		]
	},
	{
		name: "COVAR",
		description: "Devuelve la covarianza, que es el promedio de los productos de las desviaciones de los pares de puntos de datos en dos conjuntos de datos.",
		arguments: [
			{
				name: "matriz1",
				description: "es el primer rango de celdas de números enteros formado por números, matrices o referencias que contengan números"
			},
			{
				name: "matriz2",
				description: "es el segundo rango de celdas de números enteros formado por números, matrices o referencias que contengan números"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Devuelve la covarianza de población, el promedio de los productos de las desviaciones para cada pareja de puntos de datos en dos conjuntos de datos.",
		arguments: [
			{
				name: "matriz1",
				description: "es el primer rango de celdas de números enteros y debe ser números, matrices o referencias que contengan números"
			},
			{
				name: "matriz2",
				description: "es el segundo rango de celdas de números enteros y debe ser números, matrices o referencias que contengan números"
			}
		]
	},
	{
		name: "COVARIANZA.M",
		description: "Devuelve la covarianza, el promedio de los productos de las desviaciones para cada pareja de puntos de datos en dos conjuntos de datos.",
		arguments: [
			{
				name: "matriz1",
				description: "es el primer rango de celdas de números enteros y debe ser números, matrices o referencias que contengan números"
			},
			{
				name: "matriz2",
				description: "es el segundo rango de celdas de números enteros y debe ser números, matrices o referencias que contengan números"
			}
		]
	},
	{
		name: "CRECIMIENTO",
		description: "Devuelve números en una tendencia de crecimiento exponencial coincidente con puntos de datos conocidos.",
		arguments: [
			{
				name: "conocido_y",
				description: "es el conjunto de valores de Y conocidos en la relación y = b*m^x, una matriz o rango de números positivos"
			},
			{
				name: "conocido_x",
				description: "es un conjunto de valores de X opcionales (puede que conocidos) de la relación y = b*m^x, una matriz o rango con el mismo tamaño que los valores y_ conocidos"
			},
			{
				name: "nueva_matriz_x",
				description: "son nuevos valores de X de los que se desea que la función CRECIMIENTO devuelva los valores de Y correspondientes"
			},
			{
				name: "constante",
				description: "es un valor lógico: la constante b se calcula normalmente si Const = VERDADERO; se establece b = 1 si Const= FALSO u omitida"
			}
		]
	},
	{
		name: "CSC",
		description: "Devuelve la cosecante de un ángulo.",
		arguments: [
			{
				name: "número",
				description: "Es el ángulo en radianes del que quieres saber la cosecante"
			}
		]
	},
	{
		name: "CSCH",
		description: "Devuelve la cosecante hiperbólica de un ángulo.",
		arguments: [
			{
				name: "número",
				description: "Es el ángulo en radianes del que quieres saber la cosecante hiperbólica"
			}
		]
	},
	{
		name: "CUARTIL",
		description: "Devuelve el cuartil de un conjunto de datos.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de celdas de valores numéricos cuyo cuartil desea obtener"
			},
			{
				name: "cuartil",
				description: "es un número: valor mínimo = 0; primer cuartil = 1; valor de la mediana = 2; tercer cuartil = 3; valor máximo = 4"
			}
		]
	},
	{
		name: "CUARTIL.EXC",
		description: "Devuelve el cuartil de un conjunto de datos en función de los valores del percentil de 0..1, exclusivo.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de celdas de valores numéricos para el que desea el valor del cuartil"
			},
			{
				name: "cuartil",
				description: "es un número: valor mínimo = 0; primer cuartil = 1; valor de la mediana = 2; tercer cuartil = 3; valor máximo = 4"
			}
		]
	},
	{
		name: "CUARTIL.INC",
		description: "Devuelve el cuartil de un conjunto de datos en función de los valores del percentil de 0..1, inclusive.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de celdas de los valores numéricos para el que desea el valor del cuartil"
			},
			{
				name: "cuartil",
				description: "es un número: valor mínimo = 0; primer cuartil = 1; valor de la mediana = 2; tercer cuartil = 3; valor máximo = 4"
			}
		]
	},
	{
		name: "CUPON.DIAS.L1",
		description: "Devuelve el número de días del inicio del período nominal hasta la fecha de liquidación.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de liquidación del valor bursátil, expresada como número de fecha de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de vencimiento del valor bursátil, expresada como número de fecha de serie"
			},
			{
				name: "frecuencia",
				description: "es el número de cupones pagaderos por año"
			},
			{
				name: "base",
				description: "determina en qué tipo de base deben ser contados los días"
			}
		]
	},
	{
		name: "CUPON.FECHA.L1",
		description: "Devuelve la fecha de cupón anterior antes de la fecha de liquidación.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de liquidación del valor bursátil, expresado como un número de fecha de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de vencimiento del valor bursátil, expresada como número de fecha de serie"
			},
			{
				name: "frecuencia",
				description: "es el número de pagos de cupón por año"
			},
			{
				name: "base",
				description: "determina en qué tipo de base deben ser contados los días"
			}
		]
	},
	{
		name: "CUPON.FECHA.L2",
		description: "Devuelve la próxima fecha nominal después de la fecha de liquidación.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de liquidación del valor bursátil, expresada como un número de fecha de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de vencimiento del valor bursátil, expresada como un número de fecha de serie"
			},
			{
				name: "frecuencia",
				description: "es el número de pagos nominales por año"
			},
			{
				name: "base",
				description: "determina en qué tipo de base deben ser contados los días"
			}
		]
	},
	{
		name: "CUPON.NUM",
		description: "Devuelve el número de cupones pagables entre la fecha de liquidación y la fecha de vencimiento.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de liquidación del valor bursátil, expresada como número de fecha de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de vencimiento del valor bursátil, expresada como un número de fecha de serie"
			},
			{
				name: "frecuencia",
				description: "es el número de cupones pagaderos por año"
			},
			{
				name: "base",
				description: "determina en qué tipo de base deben ser contados los días"
			}
		]
	},
	{
		name: "CURTOSIS",
		description: "Devuelve la curtosis de un conjunto de datos. Consulte la Ayuda para la ecuación usada.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son entre 1 y 255 números o nombres, matrices o referencias que contengan números cuya curtosis desea calcular"
			},
			{
				name: "número2",
				description: "son entre 1 y 255 números o nombres, matrices o referencias que contengan números cuya curtosis desea calcular"
			}
		]
	},
	{
		name: "DB",
		description: "Devuelve la depreciación de un activo durante un período específico usando el método de depreciación de saldo fijo.",
		arguments: [
			{
				name: "costo",
				description: "es el costo inicial del activo"
			},
			{
				name: "valor_residual",
				description: "es el valor residual al final de la vida de un activo"
			},
			{
				name: "vida",
				description: "es el número de períodos durante los que se produce la depreciación del activo (también conocido como vida útil del activo)"
			},
			{
				name: "período",
				description: "es el período del que se desea calcular la depreciación. El período debe usar las mismas unidades que las usadas en Vida"
			},
			{
				name: "mes",
				description: "es el número de meses del primer año; si se omite, se asume que es 12"
			}
		]
	},
	{
		name: "DDB",
		description: "Devuelve la depreciación de un activo en un período específico mediante el método de depreciación por doble disminución de saldo u otro método que se especifique.",
		arguments: [
			{
				name: "costo",
				description: "es el costo inicial del activo"
			},
			{
				name: "valor_residual",
				description: "es el valor residual al final de la vida de un bien"
			},
			{
				name: "vida",
				description: "es el número de períodos durante los que se produce la depreciación del activo (algunas veces se conoce como vida útil del activo)"
			},
			{
				name: "período",
				description: "es el período para el que se desea calcular la depreciación. El Período debe usar las mismas unidades que las utilizadas en Vida"
			},
			{
				name: "factor",
				description: "es la tasa a la que disminuye el saldo. Si se omite un factor, se asumirá el  valor 2 (método de disminución del saldo doble)"
			}
		]
	},
	{
		name: "DEC.A.BIN",
		description: "Convierte un número decimal en binario.",
		arguments: [
			{
				name: "número",
				description: "es el entero decimal que desea convertir"
			},
			{
				name: "posiciones",
				description: "es el número de caracteres que se deben usar"
			}
		]
	},
	{
		name: "DEC.A.HEX",
		description: "Convierte un número decimal en hexadecimal.",
		arguments: [
			{
				name: "número",
				description: "es el entero decimal que desea convertir"
			},
			{
				name: "posiciones",
				description: "es el número de caracteres que se deben usar"
			}
		]
	},
	{
		name: "DEC.A.OCT",
		description: "Convierte un número decimal en octal.",
		arguments: [
			{
				name: "número",
				description: "es el entero decimal que desea convertir"
			},
			{
				name: "posiciones",
				description: "es el número de caracteres que se deben usar"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Redondea un número al número especificado de decimales y devuelve el resultado como texto con o sin comas.",
		arguments: [
			{
				name: "número",
				description: "es el número que se desea redondear y convertir en texto"
			},
			{
				name: "decimales",
				description: "es el número de dígitos a la derecha del separador decimal. Si se omite se establecerá: Decimales = 2"
			},
			{
				name: "no_separar_millares",
				description: "es un valor lógico: para no presentar comas en el texto devuelto  = VERDADERO; para presentar comas en el texto devuelto = FALSO u omitido"
			}
		]
	},
	{
		name: "DELTA",
		description: "Prueba si los dos números son iguales.",
		arguments: [
			{
				name: "número1",
				description: "es el primer número"
			},
			{
				name: "número2",
				description: "es el segundo número"
			}
		]
	},
	{
		name: "DERECHA",
		description: "Devuelve el número especificado de caracteres del principio de una cadena de texto.",
		arguments: [
			{
				name: "texto",
				description: "es la cadena de texto que contiene los caracteres que se desea extraer"
			},
			{
				name: "núm_de_caracteres",
				description: "especifica el número de caracteres que se desea extraer; si se omite, se asume 1"
			}
		]
	},
	{
		name: "DESREF",
		description: "Devuelve una referencia a un rango que es un número especificado de filas y columnas de una referencia dada.",
		arguments: [
			{
				name: "ref",
				description: "es la referencia a partir de la cual se desea basar la desviación, una referencia a una celda o rango de celdas adyacentes"
			},
			{
				name: "filas",
				description: "es el número de filas, hacia arriba o hacia abajo, al que se desea que haga referencia el resultado de la celda superior izquierda"
			},
			{
				name: "columnas",
				description: "es el número de columnas, hacia la derecha o izquierda, al que se desea que haga referencia el resultado de la celda superior izquierda"
			},
			{
				name: "alto",
				description: "es el alto, en número de filas, que se desea que tenga el resultado y, que si se omite, tiene el mismo alto que Referencia"
			},
			{
				name: "ancho",
				description: "es el ancho, en número de columnas, que se desea que tenga el resultado y, que si se omite, tiene el mismo ancho que Referencia"
			}
		]
	},
	{
		name: "DESVEST",
		description: "Calcula la desviación estándar de una muestra (se omiten los valores lógicos y el texto de la muestra).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números que corresponden a una muestra de una población y pueden ser números o referencias que contienen números"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números que corresponden a una muestra de una población y pueden ser números o referencias que contienen números"
			}
		]
	},
	{
		name: "DESVEST.M",
		description: "Calcula la desviación estándar en función de una muestra (omite los valores lógicos y el texto).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 argumentos numéricos que se corresponden con una muestra de una población y que pueden ser números o referencias que contienen números"
			},
			{
				name: "número2",
				description: "son de 1 a 255 argumentos numéricos que se corresponden con una muestra de una población y que pueden ser números o referencias que contienen números"
			}
		]
	},
	{
		name: "DESVEST.P",
		description: "Calcula la desviación estándar en función de la población total proporcionada como argumentos (omite los valores lógicos y el texto).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 argumentos numéricos que se corresponden con una población y que pueden ser números o referencias que contienen números"
			},
			{
				name: "número2",
				description: "son de 1 a 255 argumentos numéricos que se corresponden con una población y que pueden ser números o referencias que contienen números"
			}
		]
	},
	{
		name: "DESVESTA",
		description: "Calcula la desviación estándar de una muestra, incluyendo valores lógicos y texto. Los valores lógicos y el texto con valor FALSO tiene valor asignado 0, los que presentan valor VERDADERO tienen valor 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "son de 1 a 255 argumentos de valores correspondientes a una muestra de una población y pueden ser valores, nombres o referencias a valores"
			},
			{
				name: "valor2",
				description: "son de 1 a 255 argumentos de valores correspondientes a una muestra de una población y pueden ser valores, nombres o referencias a valores"
			}
		]
	},
	{
		name: "DESVESTP",
		description: "Calcula la desviación estándar de la población total proporcionada como argumentos (se omiten los valores lógicos y el texto).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números que corresponden a una población y pueden ser números o referencias que contienen números"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números que corresponden a una población y pueden ser números o referencias que contienen números"
			}
		]
	},
	{
		name: "DESVESTPA",
		description: "Calcula la desviación estándar de la población total, incluyendo valores lógicos y el texto. Los valores lógicos y el texto con valor FALSO tienen el valor asignado 0, los que presentan un valor VERDADERO tienen valor 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "son de 1 a 255 argumentos de valores correspondientes a una población y pueden ser valores, nombres, matrices o referencias que contengan valores"
			},
			{
				name: "valor2",
				description: "son de 1 a 255 argumentos de valores correspondientes a una población y pueden ser valores, nombres, matrices o referencias que contengan valores"
			}
		]
	},
	{
		name: "DESVIA2",
		description: "Devuelve la suma de los cuadrados de las desviaciones de los puntos de datos con respecto al promedio de la muestra.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 argumentos, una matriz o una referencia matricial para los cuales se desea calcular la DESVIA2"
			},
			{
				name: "número2",
				description: "son de 1 a 255 argumentos, una matriz o una referencia matricial para los cuales se desea calcular la DESVIA2"
			}
		]
	},
	{
		name: "DESVPROM",
		description: "Devuelve el promedio de las desviaciones absolutas de la media de los puntos de datos. Los argumentos pueden ser números, nombres, matrices o referencias que contienen números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 argumentos cuyo promedio de las desviaciones absolutas desea calcular"
			},
			{
				name: "número2",
				description: "son de 1 a 255 argumentos cuyo promedio de las desviaciones absolutas desea calcular"
			}
		]
	},
	{
		name: "DIA",
		description: "Devuelve el día del mes (un número de 1 a 31).",
		arguments: [
			{
				name: "núm_de_serie",
				description: "es un número en el código de fecha y hora usado por Spreadsheet"
			}
		]
	},
	{
		name: "DIA.LAB",
		description: "Devuelve el número de serie de la fecha antes o después de un número especificado de días laborables.",
		arguments: [
			{
				name: "fecha_inicial",
				description: "es el número de fecha de serie que representa la fecha inicial"
			},
			{
				name: "días",
				description: "es el número de días laborables antes o después de la fecha_inicial"
			},
			{
				name: "vacaciones",
				description: "es una matriz opcional de uno o varios números de fecha de serie que se excluyen del calendario laboral, como las vacaciones estatales, federales y días libres"
			}
		]
	},
	{
		name: "DIA.LAB.INTL",
		description: "Devuelve el número de serie de la fecha anterior o posterior a un número especificado de días laborables con parámetros de fin de semana personalizados.",
		arguments: [
			{
				name: "fecha_inicial",
				description: "es un número de fecha de serie que representa la fecha de comienzo"
			},
			{
				name: "días",
				description: "es el número de días laborables antes o después de fecha_inicial"
			},
			{
				name: "fin_de_semana",
				description: "es un número o cadena que especifica cuándo tienen lugar los fines de semana"
			},
			{
				name: "días_no_laborables",
				description: "es una matriz opcional de uno o más números de fecha de serie que se deben excluir del calendario laboral, como los días no laborables estatales y federales y los días libres"
			}
		]
	},
	{
		name: "DIAS",
		description: "Devuelve la cantidad de días entre las dos fechas.",
		arguments: [
			{
				name: "fecha_inicial",
				description: "Fecha de inicio y fecha de finalización son las dos fechas entre las que quieres saber los días que hay"
			},
			{
				name: "fecha_final",
				description: "Fecha de inicio y fecha de finalización son las dos fechas entre las que quieres saber los días que hay"
			}
		]
	},
	{
		name: "DIAS.LAB",
		description: "Devuelve el número total de días laborables entre dos fechas.",
		arguments: [
			{
				name: "fecha_inicial",
				description: "es un número de fecha de serie que representa la fecha inicial"
			},
			{
				name: "fecha_final",
				description: "es un número de fecha de serie que representa la fecha final"
			},
			{
				name: "vacaciones",
				description: "es un conjunto opcional de uno o varios números de fecha de serie para excluir del calendario laboral, como las vacaciones estatales y federales y los días libres"
			}
		]
	},
	{
		name: "DIAS.LAB.INTL",
		description: "Devuelve el número de días laborables completos entre dos fechas con parámetros de fin de semana personalizados.",
		arguments: [
			{
				name: "fecha_inicial",
				description: "es un número de fecha de serie que representa la fecha inicial"
			},
			{
				name: "fecha_final",
				description: "es un número de fecha de serie que representa la fecha final"
			},
			{
				name: "fin_de_semana",
				description: "es un número o una cadena que especifica cuándo tienen lugar los fines de semana"
			},
			{
				name: "días_no_laborables",
				description: "es un conjunto opcional de uno o más números de fecha de serie que se deben excluir del calendario laboral, como los días no laborables estatales y federales, y los días libres"
			}
		]
	},
	{
		name: "DIAS360",
		description: "Calcula el número de días entre dos fechas basándose en un año de 360 días (doce meses de 30 días).",
		arguments: [
			{
				name: "fecha_inicial",
				description: "fecha_inicial y fecha_final son las dos fechas entre las que se desea saber el número de días"
			},
			{
				name: "fecha_final",
				description: "fecha_inicial y fecha_final son las dos fechas entre las que se desea saber el número de días"
			},
			{
				name: "método",
				description: "es un valor lógico que especifica el método de cálculo: para usar EE.UU. (NASD) = FALSO u omitido; para usar Europeo = VERDADERO."
			}
		]
	},
	{
		name: "DIASEM",
		description: "Devuelve un número de 1 a 7 que identifica el día de la semana.",
		arguments: [
			{
				name: "núm_de_serie",
				description: "es un número que representa una fecha"
			},
			{
				name: "tipo",
				description: "es un número: para domingo=1 a sábado=7, use 1; para lunes=1 a domingo=7, use 2; para lunes=0 a domingo=6, use 3"
			}
		]
	},
	{
		name: "DIRECCION",
		description: "Crea una referencia de celda en forma de texto una vez especificados los números de fila y columna.",
		arguments: [
			{
				name: "fila",
				description: "es el número de fila que se usa en la referencia de celda: Núm_fila = 1 para la fila 1"
			},
			{
				name: "columna",
				description: "es el número de columna que se usa en la referencia de celda. Por ejemplo, Núm_columna = 4 para la columna D"
			},
			{
				name: "abs",
				description: "especifica el tipo de referencia: absoluta = 1; fila absoluta y columna relativa = 2; fila relativa y columna absoluta = 3; relativa = 4"
			},
			{
				name: "a1",
				description: "es el valor lógico que especifica el estilo de referencia: para estilo A1 = 1 o VERDADERO; para estilo F1C1 = 0 o FALSO"
			},
			{
				name: "hoja",
				description: "es el nombre de la hoja de cálculo que se usará como referencia externa"
			}
		]
	},
	{
		name: "DIST.WEIBULL",
		description: "Devuelve la probabilidad de Weibull.",
		arguments: [
			{
				name: "x",
				description: "es el valor al que desea evaluar la función, un número no negativo"
			},
			{
				name: "alfa",
				description: "es un parámetro de la distribución, un número positivo"
			},
			{
				name: "beta",
				description: "es un parámetro de la distribución, un número positivo"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la función de distribución acumulativa = VERDADERO; para usar la función de probabilidad bruta = FALSO"
			}
		]
	},
	{
		name: "DISTR.BETA",
		description: "Devuelve la función de densidad de probabilidad beta acumulativa.",
		arguments: [
			{
				name: "x",
				description: "es el valor dentro del intervalo [A, B] con el que se evaluará la función"
			},
			{
				name: "alfa",
				description: "es un parámetro de la distribución y debe ser mayor que 0"
			},
			{
				name: "beta",
				description: "es un parámetro de la distribución y debe ser mayor que 0"
			},
			{
				name: "A",
				description: "es un límite inferior opcional del intervalo de x. Si se omite, se presupone A = 0"
			},
			{
				name: "B",
				description: "es un límite superior opcional del intervalo de x. Si se omite, se presupone B = 1"
			}
		]
	},
	{
		name: "DISTR.BETA.INV",
		description: "Devuelve el inverso de la función de densidad de probabilidad beta acumulativa (DISTR.BETA).",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada con la distribución beta"
			},
			{
				name: "alfa",
				description: "es un parámetro de la distribución y debe ser mayor que 0"
			},
			{
				name: "beta",
				description: "es un parámetro de la distribución y debe ser mayor que 0"
			},
			{
				name: "A",
				description: "es un límite inferior opcional del intervalo de x. Si se omite, se presupone A = 0"
			},
			{
				name: "B",
				description: "es un límite superior opcional del intervalo de x. Si se omite, B = 1"
			}
		]
	},
	{
		name: "DISTR.BETA.N",
		description: "Devuelve la función de distribución de probabilidad beta.",
		arguments: [
			{
				name: "x",
				description: "es el valor entre A y B para evaluar la función"
			},
			{
				name: "alfa",
				description: "es un parámetro de la distribución y debe ser mayor que 0"
			},
			{
				name: "beta",
				description: "es un parámetro de la distribución y debe ser mayor que 0"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la función de distribución acumulativa = VERDADERO; para usar la función de densidad de probabilidad = FALSO"
			},
			{
				name: "A",
				description: "es un límite inferior opcional del intervalo de x. Si se omite, A = 0"
			},
			{
				name: "B",
				description: "es un límite superior opcional del intervalo de x. Si se omite, B = 1"
			}
		]
	},
	{
		name: "DISTR.BINOM",
		description: "Devuelve la probabilidad de la distribución binomial del término individual.",
		arguments: [
			{
				name: "núm_éxito",
				description: "es el número de éxitos en los ensayos"
			},
			{
				name: "ensayos",
				description: "es el número de ensayos independientes"
			},
			{
				name: "prob_éxito",
				description: "es la probabilidad de éxito en cada ensayo"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la función de distribución acumulativa = VERDADERO; para usar la función de probabilidad bruta = FALSO"
			}
		]
	},
	{
		name: "DISTR.BINOM.N",
		description: "Devuelve la probabilidad de una variable aleatoria discreta siguiendo una distribución binomial.",
		arguments: [
			{
				name: "núm_éxito",
				description: "es el número de éxitos en los ensayos"
			},
			{
				name: "ensayos",
				description: "es el número de ensayos independientes"
			},
			{
				name: "prob_éxito",
				description: "es la probabilidad de éxito en cada ensayo"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la función de distribución acumulativa = VERDADERO; para usar la función de probabilidad bruta = FALSO"
			}
		]
	},
	{
		name: "DISTR.BINOM.SERIE",
		description: "Devuelve la probabilidad de un resultado de prueba que usa una distribución binomial.",
		arguments: [
			{
				name: "ensayos",
				description: "Es la cantidad de pruebas independientes"
			},
			{
				name: "probabilidad_s",
				description: "Es la probabilidad de éxito en cada prueba"
			},
			{
				name: "número_s",
				description: "Es la cantidad de resultados satisfactorios en las pruebas"
			},
			{
				name: "número_s2",
				description: "Si existe, esta función devuelve la probabilidad de que la cantidad de pruebas satisfactorias se encuentre entre number_s y number_s2"
			}
		]
	},
	{
		name: "DISTR.CHI",
		description: "Devuelve la probabilidad de cola derecha de la distribución chi cuadrado.",
		arguments: [
			{
				name: "x",
				description: "es el valor al que desea evaluar la distribución, un número no negativo"
			},
			{
				name: "grados_de_libertad",
				description: "es el número de grados de libertad, un número entre 1 y 10^10, excluido 10^10"
			}
		]
	},
	{
		name: "DISTR.CHICUAD",
		description: "Devuelve la probabilidad de cola izquierda de la distribución chi cuadrado.",
		arguments: [
			{
				name: "x",
				description: "es el valor en el que se desea evaluar la distribución, un número no negativo"
			},
			{
				name: "grados_de_libertad",
				description: "es el número de grados de libertad, un número entre 1 y 10^10, excluyendo 10^10"
			},
			{
				name: "acumulado",
				description: "es un valor lógico que devuelve la función: función de distribución acumulativa = VERDADERO; función de densidad de probabilidad = FALSO"
			}
		]
	},
	{
		name: "DISTR.CHICUAD.CD",
		description: "Devuelve la probabilidad de cola derecha de la distribución chi cuadrado.",
		arguments: [
			{
				name: "x",
				description: "es el valor en el que se desea evaluar la distribución, un número no negativo"
			},
			{
				name: "grados_de_probabilidad",
				description: "es el número de grados de libertad, un número entre 1 y 10^10, excluyendo 10^10"
			}
		]
	},
	{
		name: "DISTR.EXP",
		description: "Devuelve la distribución exponencial.",
		arguments: [
			{
				name: "x",
				description: "es el valor de la función, un número no negativo"
			},
			{
				name: "lambda",
				description: "es el valor del parámetro, un número positivo"
			},
			{
				name: "acum",
				description: "es un valor lógico que devuelve la función: función de distribución acumulativa = VERDADERO; función de densidad de probabilidad = FALSO"
			}
		]
	},
	{
		name: "DISTR.EXP.N",
		description: "Devuelve la distribución exponencial.",
		arguments: [
			{
				name: "x",
				description: "es el valor de la función, un número no negativo"
			},
			{
				name: "lambda",
				description: "es el valor del parámetro, un número positivo"
			},
			{
				name: "acum",
				description: "es un valor lógico que devuelve la función: función de distribución acumulativa = VERDADERO; función de densidad de probabilidad = FALSO"
			}
		]
	},
	{
		name: "DISTR.F",
		description: "Devuelve la distribución de probabilidad F (grado de diversidad) (de cola derecha) de dos conjuntos de datos.",
		arguments: [
			{
				name: "x",
				description: "es el valor al que desea evaluar la función, un número no negativo"
			},
			{
				name: "grados_de_libertad1",
				description: "es el número de grados de libertad del numerador, un número entre 1 y 10^10, excluido 10^10"
			},
			{
				name: "grados_de_libertad2",
				description: "es el número de grados de libertad del denominador, un número entre 1 y 10^10, excluido 10^10"
			}
		]
	},
	{
		name: "DISTR.F.CD",
		description: "Devuelve la distribución (de cola derecha) de probabilidad F (grado de diversidad) para dos conjuntos de datos.",
		arguments: [
			{
				name: "x",
				description: "es el valor para evaluar la función, un número no negativo"
			},
			{
				name: "grados_de_libertad1",
				description: "es el número de grados de libertad del numerador, un número entre 1 y 10^10, excluyendo 10^10"
			},
			{
				name: "grados_de_libertad2",
				description: "es el número de grados de libertad del denominador, un número entre 1 y 10^10, excluyendo 10^10"
			}
		]
	},
	{
		name: "DISTR.F.INV",
		description: "Devuelve el inverso de la distribución de probabilidad F (cola derecha): si p = DISTR.F (x,...), entonces INV.F(p,...) = x.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada con la función de distribución acumulativa F, un número entre 0 y 1 inclusive"
			},
			{
				name: "grados_de_libertad1",
				description: "es el número de grados de libertad del numerador, un número entre 1 y 10^10, excluido 10^10"
			},
			{
				name: "grados_de_libertad2",
				description: "es el número de grados de libertad del denominador, un número entre 1 y 10^10, excluido 10^10"
			}
		]
	},
	{
		name: "DISTR.F.N",
		description: "Devuelve la distribución (de cola izquierda) de probabilidad F (grado de diversidad) para dos conjuntos de datos.",
		arguments: [
			{
				name: "x",
				description: "es el valor para evaluar la función, un número no negativo"
			},
			{
				name: "grados_de_libertad1",
				description: "es el número de grados de libertad del numerador, un número entre 1 y 10^10, excluyendo 10^10"
			},
			{
				name: "grados_de_libertad2",
				description: "es el número de grados de libertad del denominador, un número entre 1 y 10^10, excluyendo 10^10"
			},
			{
				name: "acumulado",
				description: "es un valor lógico que devuelve la función: función de distribución acumulativa = VERDADERO; función de densidad de probabilidad = FALSO"
			}
		]
	},
	{
		name: "DISTR.GAMMA",
		description: "Devuelve la distribución gamma.",
		arguments: [
			{
				name: "x",
				description: "es el valor al que desea evaluar la distribución, un número no negativo"
			},
			{
				name: "alfa",
				description: "es un parámetro de la distribución, un número positivo"
			},
			{
				name: "beta",
				description: "es un parámetro de la distribución, un número positivo. Si beta = 1, DISTR.GAMMA devuelve la distribución gamma estándar"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para que devuelva la función de distribución acumulativa = VERDADERO; para que devuelva la función de probabilidad bruta = FALSO u omitido"
			}
		]
	},
	{
		name: "DISTR.GAMMA.INV",
		description: "Devuelve el inverso de la distribución gamma acumulativa: si p = DISTR.GAMMA(x,...), entonces INV.GAMMA(p,...) = x.",
		arguments: [
			{
				name: "prob",
				description: "es la probabilidad asociada con la distribución gamma, un número entre 0 y 1 inclusive"
			},
			{
				name: "alfa",
				description: "es un parámetro de la distribución, un número positivo"
			},
			{
				name: "beta",
				description: "es un parámetro de la distribución, un número positivo. Si beta = 1, INV.GAMMA devuelve el valor inverso de la distribución gamma estándar"
			}
		]
	},
	{
		name: "DISTR.GAMMA.N",
		description: "Devuelve la distribución gamma.",
		arguments: [
			{
				name: "x",
				description: "es el valor en el que desea evaluar la distribución, un número no negativo"
			},
			{
				name: "alfa",
				description: "es un parámetro de la distribución, un número positivo"
			},
			{
				name: "beta",
				description: "es un parámetro de la distribución, un número positivo. Si beta = 1, DISTR.GAMMA.N devuelve la distribución gamma estándar"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para que devuelva la función de distribución acumulativa = VERDADERO; para que devuelva la función de probabilidad bruta = FALSO u omitido"
			}
		]
	},
	{
		name: "DISTR.HIPERGEOM",
		description: "Devuelve la distribución hipergeométrica.",
		arguments: [
			{
				name: "muestra_éxito",
				description: "es el número de éxitos en la muestra"
			},
			{
				name: "núm_de_muestra",
				description: "es el tamaño de la muestra"
			},
			{
				name: "población_éxito",
				description: "es el número de éxitos en la población"
			},
			{
				name: "núm_de_población",
				description: "es el tamaño de la población"
			}
		]
	},
	{
		name: "DISTR.HIPERGEOM.N",
		description: "Devuelve la distribución hipergeométrica.",
		arguments: [
			{
				name: "muestra_éxito",
				description: "es el número de éxitos en la muestra"
			},
			{
				name: "núm_de_muestra",
				description: "es el tamaño de la muestra"
			},
			{
				name: "población_éxito",
				description: "es el número de éxitos en la población"
			},
			{
				name: "núm_de_población",
				description: "es el tamaño de la población"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la función de distribución acumulativa = VERDADERO; para usar la función de densidad de probabilidad = FALSO"
			}
		]
	},
	{
		name: "DISTR.LOG.INV",
		description: "Devuelve el inverso de la función de distribución logarítmico-normal acumulativa de x, donde ln(x) se distribuye de forma normal con los parámetros Media y desv_estándar.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada con la distribución logarítmico-normal, un número entre 0 y 1 inclusive"
			},
			{
				name: "media",
				description: "es la media de ln(x)"
			},
			{
				name: "desv_estándar",
				description: "es la desviación estándar de In(x), un número positivo"
			}
		]
	},
	{
		name: "DISTR.LOG.NORM",
		description: "Devuelve la distribución logarítmico-normal acumulativa de x, donde In(x) se distribuye de forma normal con los parámetros Media y desv_estándar.",
		arguments: [
			{
				name: "x",
				description: "es el valor al que desea evaluar la función, un número positivo"
			},
			{
				name: "media",
				description: "es la media de ln(x)"
			},
			{
				name: "desv_estándar",
				description: "es la desviación estándar de ln(x), un número positivo"
			}
		]
	},
	{
		name: "DISTR.LOGNORM",
		description: "Devuelve la distribución logarítmico-normal de x, donde ln(x) se distribuye normalmente con los parámetros media y desv_estándar.",
		arguments: [
			{
				name: "x",
				description: "es el valor para evaluar la función, un número positivo"
			},
			{
				name: "media",
				description: "es la media de ln(x)"
			},
			{
				name: "desv_estándar",
				description: "es la desviación estándar de ln(x), un número positivo"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la función de distribución acumulativa = VERDADERO; para usar la función de densidad de probabilidad = FALSO"
			}
		]
	},
	{
		name: "DISTR.NORM",
		description: "Devuelve la distribución acumulativa normal para la media y desviación estándar especificadas.",
		arguments: [
			{
				name: "x",
				description: "es el valor cuya distribución desea obtener"
			},
			{
				name: "media",
				description: "es la media aritmética de la distribución"
			},
			{
				name: "desv_estándar",
				description: "es la desviación estándar de la distribución, un número positivo"
			},
			{
				name: "acum",
				description: "es un valor lógico: para usar la función distribución acumulativa = VERDADERO; para usar la función de densidad de probabilidad  = FALSO"
			}
		]
	},
	{
		name: "DISTR.NORM.ESTAND",
		description: "Devuelve la distribución normal estándar acumulativa. Tiene una media de cero y una desviación estándar de uno.",
		arguments: [
			{
				name: "z",
				description: "es el valor cuya distribución desea obtener"
			}
		]
	},
	{
		name: "DISTR.NORM.ESTAND.INV",
		description: "Devuelve el inverso de la distribución normal estándar acumulativa. Tiene una media de cero y una desviación estándar de uno.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad que corresponde a la distribución normal, un número entre 0 y 1 inclusive"
			}
		]
	},
	{
		name: "DISTR.NORM.ESTAND.N",
		description: "Devuelve la distribución normal estándar (tiene una media de cero y una desviación estándar de uno).",
		arguments: [
			{
				name: "z",
				description: "es el valor para el que se desea la distribución"
			},
			{
				name: "acumulado",
				description: "es un valor lógico que devuelve la función: función de distribución acumulativa = VERDADERO; función de densidad de probabilidad = FALSO"
			}
		]
	},
	{
		name: "DISTR.NORM.INV",
		description: "Devuelve el inverso de la distribución acumulativa normal para la media y desviación estándar especificadas.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad que corresponde a la distribución normal, un número entre 0 y 1 inclusive"
			},
			{
				name: "media",
				description: "es la media aritmética de la distribución"
			},
			{
				name: "desv_estándar",
				description: "es la desviación estándar de la distribución, un número positivo"
			}
		]
	},
	{
		name: "DISTR.NORM.N",
		description: "Devuelve la distribución normal para la media  y la desviación estándar especificadas.",
		arguments: [
			{
				name: "x",
				description: "es el valor para el que desea la distribución"
			},
			{
				name: "media",
				description: "es la media aritmética de la distribución"
			},
			{
				name: "desv_estándar",
				description: "es la desviación estándar de la distribución, un número positivo"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la función de distribución acumulativa = VERDADERO; para usar la función de densidad de probabilidad = FALSO"
			}
		]
	},
	{
		name: "DISTR.T",
		description: "Devuelve la distribución t de Student .",
		arguments: [
			{
				name: "x",
				description: "es el valor numérico al que se va a evaluar la distribución"
			},
			{
				name: "grados_de_libertad",
				description: "es un entero que indica el número de grados de libertad que caracteriza la distribución"
			},
			{
				name: "colas",
				description: "especifica el número de colas de la distribución que se debe devolver: distribución de una cola = 1; distribución de dos colas = 2"
			}
		]
	},
	{
		name: "DISTR.T.2C",
		description: "Devuelve la distribución t de Student de dos colas.",
		arguments: [
			{
				name: "x",
				description: "es el valor numérico para evaluar la distribución"
			},
			{
				name: "grados_de_libertad",
				description: "es un entero que indica el número de grados de libertad que caracterizan la distribución"
			}
		]
	},
	{
		name: "DISTR.T.CD",
		description: "Devuelve la distribución t de Student de cola derecha.",
		arguments: [
			{
				name: "x",
				description: "es el valor numérico para evaluar la distribución"
			},
			{
				name: "grados_de_libertad",
				description: "es un entero que indica el número de grados de libertad que caracterizan la distribución"
			}
		]
	},
	{
		name: "DISTR.T.INV",
		description: "Devuelve el inverso de dos colas de la distribución t de Student.",
		arguments: [
			{
				name: "probabilidad",
				description: "es la probabilidad asociada con la distribución t de Student de dos colas, un número entre 0 y 1 inclusive"
			},
			{
				name: "grados_de_libertad",
				description: "es un número entero positivo que indica el número de grados de libertad que caracteriza la distribución"
			}
		]
	},
	{
		name: "DISTR.T.N",
		description: "Devuelve la distribución t de Student de cola izquierda.",
		arguments: [
			{
				name: "x",
				description: "es el valor numérico para evaluar la distribución"
			},
			{
				name: "grados_de_libertad",
				description: "es un entero que indica el número de grados de libertad que caracterizan la distribución"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la función de distribución acumulativa = VERDADERO; para usar la función de densidad de probabilidad = FALSO"
			}
		]
	},
	{
		name: "DISTR.WEIBULL",
		description: "Devuelve la probabilidad de una variable aleatoria siguiendo una distribución de Weibull.",
		arguments: [
			{
				name: "x",
				description: "es el valor al que desea evaluar la función, un número no negativo"
			},
			{
				name: "alfa",
				description: "es un parámetro de la distribución, un número positivo"
			},
			{
				name: "beta",
				description: "es un parámetro de la distribución, un número positivo"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la función de distribución acumulativa = VERDADERO; para usar la función de probabilidad bruta = FALSO"
			}
		]
	},
	{
		name: "DVS",
		description: "Devuelve la depreciación de un activo para cualquier período especificado, incluyendo períodos parciales, usando el método de depreciación por doble disminución del saldo u otro método que especifique.",
		arguments: [
			{
				name: "costo",
				description: "es el costo inicial del activo"
			},
			{
				name: "valor_residual",
				description: "es el valor remanente al final de la vida de un activo"
			},
			{
				name: "vida",
				description: "es el número de períodos durante los que se produce la depreciación del activo (algunas veces se conoce como vida útil del bien)"
			},
			{
				name: "período_inicial",
				description: "es el período inicial para el se desea calcular la depreciación, en las mismas unidades que Vida"
			},
			{
				name: "período_final",
				description: "es el período final para el se desea calcular la depreciación, en las mismas unidades que Vida"
			},
			{
				name: "factor",
				description: "es la tasa a la que disminuye el saldo; si se omite, se asume 2 (depreciación por doble disminución)"
			},
			{
				name: "sin_cambios",
				description: "cambia al método directo de depreciación cuando la depreciación es mayor que el saldo en disminución = FALSO o bien se omite; si no se desea que cambie = VERDADERO"
			}
		]
	},
	{
		name: "ELEGIR",
		description: "Elige un valor o una acción de una lista de valores a partir de un número de índice.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm_índice",
				description: "especifica el argumento de valor que se selecciona. El núm_índice debe estar entre 1 y 254 o ser una fórmula, o una referencia a un número, entre 1 y 254"
			},
			{
				name: "valor1",
				description: "son de 1 a 254 argumentos de valores, referencias de celda, nombres definidos, fórmulas, funciones o argumentos de texto entre los cuales ELEGIR selecciona un valor"
			},
			{
				name: "valor2",
				description: "son de 1 a 254 argumentos de valores, referencias de celda, nombres definidos, fórmulas, funciones o argumentos de texto entre los cuales ELEGIR selecciona un valor"
			}
		]
	},
	{
		name: "ENCONTRAR",
		description: "Devuelve la posición inicial de una cadena de texto dentro de otra cadena de texto. BUSCAR diferencia entre mayúsculas y minúsculas.",
		arguments: [
			{
				name: "texto_buscado",
				description: "es el texto que se desea encontrar. Use comillas dobles (sin texto) para que coincida con  el primer carácter en Dentro_texto. No se permiten caracteres comodín"
			},
			{
				name: "dentro_del_texto",
				description: "es el texto que a su vez contiene el texto que se desea encontrar"
			},
			{
				name: "núm_inicial",
				description: "especifica el carácter a partir del cual se iniciará la búsqueda. El primer carácter en Dentro_texto es 1. Si se omite, Núm_inicial = 1"
			}
		]
	},
	{
		name: "ENTERO",
		description: "Redondea un número hasta el entero inferior más próximo.",
		arguments: [
			{
				name: "número",
				description: "es el número real que se desea redondear a entero"
			}
		]
	},
	{
		name: "ERROR.TIPICO.XY",
		description: "Devuelve el error típico del valor de Y previsto para cada X de la regresión.",
		arguments: [
			{
				name: "conocido_y",
				description: "es una matriz, o rango de puntos de datos dependientes, formada por: números, nombres, matrices o referencias que contengan números"
			},
			{
				name: "conocido_x",
				description: "es una matriz, o rango de puntos de datos independientes, formada por: números, nombres, matrices o referencias que contengan números"
			}
		]
	},
	{
		name: "ES.IMPAR",
		description: "Devuelve VERDADERO si el número es impar.",
		arguments: [
			{
				name: "número",
				description: "es el valor que se va a comprobar"
			}
		]
	},
	{
		name: "ES.PAR",
		description: "Devuelve verdadero si el número es par.",
		arguments: [
			{
				name: "número",
				description: "es el valor a comprobar"
			}
		]
	},
	{
		name: "ESBLANCO",
		description: "Comprueba si se refiere a una celda vacía y devuelve VERDADERO o FALSO.",
		arguments: [
			{
				name: "valor",
				description: "es la celda o un nombre que se refiere a la celda que desea comprobar"
			}
		]
	},
	{
		name: "ESERR",
		description: "Comprueba si un valor es un error (#¡VALOR!, #¡REF!, #¡DIV/0!, #¡NUM!, #¿NOMBRE? o #NULO!) excepto #N/A (valor no aplicable), y devuelve VERDADERO o FALSO.",
		arguments: [
			{
				name: "valor",
				description: "es el valor que desea comprobar. Valor puede referirse a una celda, una fórmula o un nombre que se refiere a una celda, fórmula o valor"
			}
		]
	},
	{
		name: "ESERROR",
		description: "Comprueba si un valor es un error (#N/A, #¡VALOR!, #¡REF!, #¡DIV/0!, #¡NUM!, #¿NOMBRE? o #NULO!), y devuelve VERDADERO o FALSO.",
		arguments: [
			{
				name: "valor",
				description: "es el valor que desea probar. Valor puede referirse a una celda, una fórmula o un nombre que se refiere a una celda, fórmula o valor"
			}
		]
	},
	{
		name: "ESFORMULA",
		description: "Comprueba si la referencia es a una celda que contiene una fórmula y devuelve VERDADERO o FALSO.",
		arguments: [
			{
				name: "referencia",
				description: "Es una referencia a la celda que quieres probar. La referencia puede ser una referencia de celda, una fórmula o un nombre que hace referencia a una celda"
			}
		]
	},
	{
		name: "ESLOGICO",
		description: "Comprueba si un valor es un valor lógico (VERDADERO o FALSO), y devuelve VERDADERO o FALSO.",
		arguments: [
			{
				name: "valor",
				description: "es el valor que desea probar. Valor puede referirse a una celda, una fórmula o un nombre que se refiere a una celda, fórmula o valor"
			}
		]
	},
	{
		name: "ESNOD",
		description: "Comprueba si un valor de error es #N/A (valor no aplicable) y devuelve VERDADERO o FALSO.",
		arguments: [
			{
				name: "valor",
				description: "es el valor que desea probar. Valor puede referirse a una celda, una fórmula o un nombre que se refiere a una celda, fórmula o valor"
			}
		]
	},
	{
		name: "ESNOTEXTO",
		description: "Comprueba si un valor no es texto (las celdas en blanco no son texto), y devuelve VERDADERO o FALSO.",
		arguments: [
			{
				name: "valor",
				description: "es el valor que desea probar: una celda, una fórmula o un nombre que se refiere a una celda, fórmula o valor"
			}
		]
	},
	{
		name: "ESNUMERO",
		description: "Comprueba si un valor es un número y devuelve VERDADERO o FALSO.",
		arguments: [
			{
				name: "valor",
				description: "es el valor que desea probar. Valor puede referirse a una celda, una fórmula o un nombre que se refiere a una celda, fórmula o valor"
			}
		]
	},
	{
		name: "ESPACIOS",
		description: "Quita todos los espacios del texto excepto los espacios individuales entre palabras.",
		arguments: [
			{
				name: "texto",
				description: "es el texto del cual se desea quitar espacios"
			}
		]
	},
	{
		name: "ESREF",
		description: "Comprueba si valor es una referencia y devuelve VERDADERO o FALSO.",
		arguments: [
			{
				name: "valor",
				description: "es el valor que desea probar. Valor puede referirse a una celda, una fórmula o un nombre que se refiere a una celda, fórmula o valor"
			}
		]
	},
	{
		name: "ESTEXTO",
		description: "Comprueba si un valor es texto y devuelve VERDADERO o FALSO.",
		arguments: [
			{
				name: "valor",
				description: "es el valor de texto que se desea comprobar. Valor puede referirse a una celda, una fórmula o un nombre que se refiere a una celda, fórmula o valor"
			}
		]
	},
	{
		name: "ESTIMACION.LINEAL",
		description: "Devuelve estadísticas que describen una tendencia lineal que coincide con puntos de datos conocidos, mediante una línea recta usando el método de los mínimos cuadrados.",
		arguments: [
			{
				name: "conocido_y",
				description: "es el conjunto de valores de Y conocidos en  la relación y = mx + b"
			},
			{
				name: "conocido_x",
				description: "es un conjunto de valores de X opcionales (puede que conocidos) de la relación y = mx + b"
			},
			{
				name: "constante",
				description: "es un valor lógico: la constante b se calcula de forma normal si Const = VERDADERO u omitida; b será igual a 0 si Const = FALSO"
			},
			{
				name: "estadística",
				description: "es un valor lógico: para que devuelva estadísticas de regresión adicionales = VERDADERO; para que devuelva coeficientes m y la constante b = FALSO u omitida"
			}
		]
	},
	{
		name: "ESTIMACION.LOGARITMICA",
		description: "Devuelve estadísticas que describen una curva exponencial, coincidente con puntos de datos conocidos.",
		arguments: [
			{
				name: "conocido_y",
				description: "es el conjunto de valores de Y conocidos en la relación y = b*m^x"
			},
			{
				name: "conocido_x",
				description: "es un conjunto de valores de X opcionales (puede que conocidos) en la relación y = b*m^x"
			},
			{
				name: "constante",
				description: "es un valor lógico: la constante b se calcula normalmente si Const = VERDADERO u omitida;  la constante b se considera 1 si Const = FALSO"
			},
			{
				name: "estadística",
				description: "es un valor lógico: para que devuelva estadísticas de regresión adicionales = VERDADERO; para que devuelva coeficientes m y la constante b  = FALSO u omitida"
			}
		]
	},
	{
		name: "EXP",
		description: "Devuelve e elevado a la potencia de un número determinado.",
		arguments: [
			{
				name: "número",
				description: "es el exponente aplicado a la base e. La constante e es igual a 2.71828182845904, la base del logaritmo natural"
			}
		]
	},
	{
		name: "EXTRAE",
		description: "Devuelve los caracteres del centro de una cadena de texto, dada una posición y longitud iniciales.",
		arguments: [
			{
				name: "texto",
				description: "es la cadena de texto de la cual se desea extraer los caracteres"
			},
			{
				name: "posición_inicial",
				description: "es la posición del primer carácter que se desea extraer del argumento texto. El primer carácter en Texto es 1"
			},
			{
				name: "núm_de_caracteres",
				description: "especifica el número de caracteres de Texto que se debe devolver"
			}
		]
	},
	{
		name: "FACT",
		description: "Devuelve el factorial de un número, igual a 1*2*3*...*Número.",
		arguments: [
			{
				name: "número",
				description: "es el número no negativo del que desea obtener su factorial"
			}
		]
	},
	{
		name: "FACT.DOBLE",
		description: "Devuelve el factorial doble de un número.",
		arguments: [
			{
				name: "número",
				description: "es el número cuyo factorial doble desea calcular"
			}
		]
	},
	{
		name: "FALSO",
		description: "Devuelve el valor lógico FALSO.",
		arguments: [
		]
	},
	{
		name: "FECHA",
		description: "Devuelve el número que representa la fecha en código de fecha y hora de Spreadsheet.",
		arguments: [
			{
				name: "año",
				description: "es un número entre 1900 y 9999 en Spreadsheet para Windows o entre 1904 y 9999 en Spreadsheet para Macintosh"
			},
			{
				name: "mes",
				description: "es un número de 1 a 12 que representa el mes del año"
			},
			{
				name: "día",
				description: "es un número de 1 a 31 que representa el día del mes"
			}
		]
	},
	{
		name: "FECHA.MES",
		description: "Devuelve el número de serie de la fecha que es el número indicado de meses antes o después de la fecha inicial.",
		arguments: [
			{
				name: "fecha_inicial",
				description: "es un número de fecha de serie que representa la fecha inicial"
			},
			{
				name: "meses",
				description: "es el número de meses antes o después de la fecha_inicial"
			}
		]
	},
	{
		name: "FECHANUMERO",
		description: "Convierte una fecha en forma de texto en un número que representa la fecha en código de fecha y hora de Spreadsheet.",
		arguments: [
			{
				name: "texto_de_fecha",
				description: "es el texto que representa una fecha en formato de fecha de Spreadsheet, entre 1/1/1900 (Windows) o 1/1/1904 (Macintosh) y 31/12/9999"
			}
		]
	},
	{
		name: "FI",
		description: "Devuelve el valor de la función de densidad para una distribución normal estándar.",
		arguments: [
			{
				name: "x",
				description: "Es el número para el que quieres saber la densidad de la distribución normal estándar"
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
		name: "FILA",
		description: "Devuelve el número de fila de una referencia.",
		arguments: [
			{
				name: "ref",
				description: "es la celda, o rango único de celdas, de la que se desea obtener el número de fila; si se omite, devuelve la celda con la función FILA"
			}
		]
	},
	{
		name: "FILAS",
		description: "Devuelve el número de filas de una referencia o matriz.",
		arguments: [
			{
				name: "matriz",
				description: "es una matriz, fórmula matricial o referencia a un rango de celdas de las que se desea obtener el número de filas"
			}
		]
	},
	{
		name: "FIN.MES",
		description: "Devuelve el número de serie del último día del mes antes o después del número especificado de meses.",
		arguments: [
			{
				name: "fecha_inicial",
				description: "es el número de fecha de serie que representa la fecha inicial"
			},
			{
				name: "meses",
				description: "es el número de meses antes o después de la fecha_inicial"
			}
		]
	},
	{
		name: "FISHER",
		description: "Devuelve la transformación Fisher o coeficiente Z.",
		arguments: [
			{
				name: "x",
				description: "es un valor numérico para el que se desea calcular la transformación, un número entre -1 y 1, excluyendo -1 y 1"
			}
		]
	},
	{
		name: "FORMULATEXTO",
		description: "Devuelve una fórmula como una cadena.",
		arguments: [
			{
				name: "referencia",
				description: "Es una referencia a una fórmula"
			}
		]
	},
	{
		name: "FRAC.AÑO",
		description: "Devuelve la fracción del año que representa el número de días completos entre la fecha_inicial y la fecha_fin.",
		arguments: [
			{
				name: "fecha_inicial",
				description: "es el número de fecha de serie que representa la fecha inicial"
			},
			{
				name: "fecha_final",
				description: "es el número de fecha de serie que representa la fecha final"
			},
			{
				name: "base",
				description: "determina en qué tipo de base deben ser contados los días"
			}
		]
	},
	{
		name: "FRECUENCIA",
		description: "Calcula la frecuencia con la que ocurre un valor dentro de un rango de valores y devuelve una matriz vertical de números con más de un elemento que grupos.",
		arguments: [
			{
				name: "datos",
				description: "es una matriz, o una referencia, de un conjunto de valores de los cuales se desea contar frecuencias. Se omiten espacios en blanco y texto."
			},
			{
				name: "grupos",
				description: "es una matriz, o una referencia, a rangos dentro de los cuales se desea agrupar los valores de datos"
			}
		]
	},
	{
		name: "FUN.ERROR",
		description: "Devuelve la función de error.",
		arguments: [
			{
				name: "límite_inferior",
				description: "es el límite inferior para integrar FUN.ERR"
			},
			{
				name: "límite_superior",
				description: "es el límite superior para integrar FUN.ERR"
			}
		]
	},
	{
		name: "FUN.ERROR.COMPL",
		description: "Devuelve la función de error complementaria.",
		arguments: [
			{
				name: "x",
				description: "es el límite inferior para integrar FUN.ERR"
			}
		]
	},
	{
		name: "FUN.ERROR.COMPL.EXACTO",
		description: "Devuelve la función de error complementaria.",
		arguments: [
			{
				name: "X",
				description: "es el límite inferior para integrar FUN.ERROR.EXACTO"
			}
		]
	},
	{
		name: "FUN.ERROR.EXACTO",
		description: "Devuelve la función de error.",
		arguments: [
			{
				name: "X",
				description: "es el límite inferior para integrar FUN.ERROR.EXACTO"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Devuelve los valores de la función gamma.",
		arguments: [
			{
				name: "x",
				description: "Es el valor para el que quieres calcular gamma"
			}
		]
	},
	{
		name: "GAMMA.LN",
		description: "Devuelve el logaritmo natural de la función gamma.",
		arguments: [
			{
				name: "x",
				description: "es el valor cuya función GAMMA.LN desea calcular, un número positivo"
			}
		]
	},
	{
		name: "GAMMA.LN.EXACTO",
		description: "Devuelve el logaritmo natural de la función gamma.",
		arguments: [
			{
				name: "x",
				description: "es el valor cuya función GAMMA.LN.EXACTO desea calcular, un número positivo"
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
		name: "GRADOS",
		description: "Convierte radianes en grados.",
		arguments: [
			{
				name: "ángulo",
				description: "es el ángulo en radianes que se desea convertir"
			}
		]
	},
	{
		name: "HALLAR",
		description: "Devuelve el número de caracteres en el cual se encuentra un carácter en particular o cadena de texto, leyendo de izquierda a derecha (no diferencia entre mayúsculas ni minúsculas).",
		arguments: [
			{
				name: "texto_buscado",
				description: "es el texto que se desea encontrar. Puede usar ? y * como caracteres comodín; puede usar ~? y ~* para encontrar los caracteres ? y *"
			},
			{
				name: "dentro_del_texto",
				description: "es el texto en el que se desea encontrar texto_buscado"
			},
			{
				name: "núm_inicial",
				description: "es, contando desde la izquierda, el número del carácter en dentro_del_texto desde donde se desea iniciar la búsqueda. Si se omite, se usa 1"
			}
		]
	},
	{
		name: "HEX.A.BIN",
		description: "Convierte un número hexadecimal en binario.",
		arguments: [
			{
				name: "número",
				description: "es el número hexadecimal que desea convertir"
			},
			{
				name: "posiciones",
				description: "es el número de caracteres que se deben usar"
			}
		]
	},
	{
		name: "HEX.A.DEC",
		description: "Convierte un número hexadecimal en decimal.",
		arguments: [
			{
				name: "número",
				description: "es el número hexadecimal que desea convertir"
			}
		]
	},
	{
		name: "HEX.A.OCT",
		description: "Convierte un número hexadecimal en octal.",
		arguments: [
			{
				name: "número",
				description: "es el número hexadecimal que desea convertir"
			},
			{
				name: "posiciones",
				description: "es el número de caracteres que se deben usar"
			}
		]
	},
	{
		name: "HIPERVINCULO",
		description: "Crea un acceso directo o salto que abre un documento guardado en el disco duro, en un servidor de red o en Internet.",
		arguments: [
			{
				name: "ubicación_del_vínculo",
				description: "es el texto con la ruta de acceso y el nombre de archivo que se abrirá en el disco duro, en una dirección UNC o en una ruta URL"
			},
			{
				name: "nombre_descriptivo",
				description: "es el número, texto o función que aparece en la celda. Si se omite, la celda presentará el texto de ubicación_del_vínculo"
			}
		]
	},
	{
		name: "HOJA",
		description: "Devuelve el número de la hoja a la que se hace referencia.",
		arguments: [
			{
				name: "valor",
				description: "Es el nombre de una hoja o una referencia de la que quieres el número de hoja. Si se omite, se devuelve el número de la hoja que contiene la función"
			}
		]
	},
	{
		name: "HOJAS",
		description: "Devuelve la cantidad de hojas de una referencia.",
		arguments: [
			{
				name: "referencia",
				description: "Es una referencia de la que quieres saber la cantidad de hojas que contiene. Si se omite, se devuelve la cantidad de hojas del libro que contienen la función"
			}
		]
	},
	{
		name: "HORA",
		description: "Devuelve la hora como un número de 0 (12:00 a.m.) a 23 (11:00 p.m.).",
		arguments: [
			{
				name: "núm_de_serie",
				description: "es un número en el código de fecha y hora usado por Spreadsheet, o texto en formato de hora, como por ejemplo 16:48:00 o 4:48:00 p.m."
			}
		]
	},
	{
		name: "HORANUMERO",
		description: "Convierte una hora de texto en un número de serie de Spreadsheet para una hora, un número de 0 (12:00:00 a.m.) a 0.999988426 (11:59:59 p.m.). Da formato al número con un formato de hora después de introducir la fórmula.",
		arguments: [
			{
				name: "texto_de_hora",
				description: "es una cadena de texto que indica la hora en cualquiera de los formatos de hora de Spreadsheet (se omite la información de fecha en la cadena)"
			}
		]
	},
	{
		name: "HOY",
		description: "Devuelve la fecha actual con formato de fecha.",
		arguments: [
		]
	},
	{
		name: "IGUAL",
		description: "Comprueba si dos cadenas de texto son exactamente iguales y devuelve VERDADERO o FALSO. EXACTO diferencia entre mayúsculas y minúsculas.",
		arguments: [
			{
				name: "texto1",
				description: "es la primera cadena de texto"
			},
			{
				name: "texto2",
				description: "es la segunda cadena de texto"
			}
		]
	},
	{
		name: "IM.ABS",
		description: "Devuelve el valor absoluto (módulo) de un número complejo.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuyo valor absoluto desea calcular"
			}
		]
	},
	{
		name: "IM.ANGULO",
		description: "Devuelve el argumento q, un ángulo expresado en radianes.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuyo argumento desea calcular"
			}
		]
	},
	{
		name: "IM.CONJUGADA",
		description: "Devuelve el conjugado complejo de un número complejo.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuyo conjugado desea calcular"
			}
		]
	},
	{
		name: "IM.COS",
		description: "Devuelve el coseno de un número complejo.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuyo coseno desea calcular"
			}
		]
	},
	{
		name: "IM.COSH",
		description: "Devuelve el coseno hiperbólico de un número complejo.",
		arguments: [
			{
				name: "númeroi",
				description: "Es un número complejo del que quieres saber el coseno hiperbólico"
			}
		]
	},
	{
		name: "IM.COT",
		description: "Devuelve la cotangente de un número complejo.",
		arguments: [
			{
				name: "númeroi",
				description: "Es un número complejo del que quieres saber la cotangente"
			}
		]
	},
	{
		name: "IM.CSC",
		description: "Devuelve la cosecante de un número complejo.",
		arguments: [
			{
				name: "númeroi",
				description: "Es un número complejo del que quieres saber la cosecante"
			}
		]
	},
	{
		name: "IM.CSCH",
		description: "Devuelve la cosecante hiperbólica de un número complejo.",
		arguments: [
			{
				name: "númeroi",
				description: "Es un número complejo del que quieres saber la cosecante hiperbólica"
			}
		]
	},
	{
		name: "IM.DIV",
		description: "Devuelve el cociente de dos números complejos.",
		arguments: [
			{
				name: "inúmero1",
				description: "es el numerador o dividendo complejo"
			},
			{
				name: "inúmero2",
				description: "es el denominador o divisor complejo"
			}
		]
	},
	{
		name: "IM.EXP",
		description: "Devuelve el valor exponencial de un número complejo.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuyo valor exponencial desea calcular"
			}
		]
	},
	{
		name: "IM.LN",
		description: "Devuelve el logaritmo natural de un número complejo.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuyo logaritmo natural desea calcular"
			}
		]
	},
	{
		name: "IM.LOG10",
		description: "Devuelve el logaritmo de base 10 de un número complejo.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuyo logaritmo común desea calcular"
			}
		]
	},
	{
		name: "IM.LOG2",
		description: "Devuelve el logaritmo de base 2 de un número complejo.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuyo logaritmo de base 2 desea calcular"
			}
		]
	},
	{
		name: "IM.POT",
		description: "Devuelve un número complejo elevado a la potencia del entero.",
		arguments: [
			{
				name: "inúmero",
				description: "es un número complejo que desea elevar a una potencia"
			},
			{
				name: "número",
				description: "es la potencia a la cual desea elevar un número complejo"
			}
		]
	},
	{
		name: "IM.PRODUCT",
		description: "Devuelve el producto de 1 a 255 números complejos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inúmero1",
				description: "Inúmero1, Inúmero2,... son de 1 a 255 números que desea multiplicar."
			},
			{
				name: "inúmero2",
				description: "Inúmero1, Inúmero2,... son de 1 a 255 números que desea multiplicar."
			}
		]
	},
	{
		name: "IM.RAIZ2",
		description: "Devuelve la raíz cuadrada de un número complejo.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuya raíz cuadrada desea calcular"
			}
		]
	},
	{
		name: "IM.REAL",
		description: "Devuelve el coeficiente real de un número complejo.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuyo coeficiente real desea calcular"
			}
		]
	},
	{
		name: "IM.SEC",
		description: "Devuelve la secante de un número complejo.",
		arguments: [
			{
				name: "númeroi",
				description: "Es un número complejo del que quieres saber la secante"
			}
		]
	},
	{
		name: "IM.SECH",
		description: "Devuelve la secante hiperbólica de un número complejo .",
		arguments: [
			{
				name: "númeroi",
				description: "Es un número complejo del que quieres saber la secante hiperbólica"
			}
		]
	},
	{
		name: "IM.SENO",
		description: "Devuelve el seno de un número complejo.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuyo seno desea calcular"
			}
		]
	},
	{
		name: "IM.SENOH",
		description: "Devuelve el seno hiperbólico de un número complejo.",
		arguments: [
			{
				name: "númeroi",
				description: "Es un número complejo del que quieres saber el seno hiperbólico"
			}
		]
	},
	{
		name: "IM.SUM",
		description: "Devuelve la suma de números complejos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inúmero1",
				description: "son de 1 a 255 números que desea sumar"
			},
			{
				name: "inúmero2",
				description: "son de 1 a 255 números que desea sumar"
			}
		]
	},
	{
		name: "IM.SUSTR",
		description: "Devuelve la diferencia de dos números complejos.",
		arguments: [
			{
				name: "inúmero1",
				description: "es el número complejo del que debe restar inúmero2"
			},
			{
				name: "inúmero2",
				description: "es el número complejo para restar del inúmero1"
			}
		]
	},
	{
		name: "IM.TAN",
		description: "Devuelve la tangente de un número complejo.",
		arguments: [
			{
				name: "númeroi",
				description: "Es un número complejo del que quiere saber la tangente"
			}
		]
	},
	{
		name: "IMAGINARIO",
		description: "Devuelve el coeficiente imaginario de un número complejo.",
		arguments: [
			{
				name: "inúmero",
				description: "es el número complejo cuyo coeficiente imaginario desea calcular"
			}
		]
	},
	{
		name: "IMPORTARDATOSDINAMICOS",
		description: "Extrae datos almacenados en una tabla dinámica.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "camp_datos",
				description: "es el nombre del campo de datos del que extraer los datos"
			},
			{
				name: "tabla_dinámica",
				description: "es una referencia a una celda o rango de celdas en la tabla dinámica que contiene los datos que desea recuperar"
			},
			{
				name: "campo",
				description: "campo al que hacer referencia"
			},
			{
				name: "elemento",
				description: "elemento de campo al que hacer referencia"
			}
		]
	},
	{
		name: "INDICE",
		description: "Devuelve un valor o referencia de la celda en la intersección de una fila y columna en particular, en un rango especificado.",
		arguments: [
			{
				name: "matriz",
				description: "es un rango de celdas o una constante de matriz."
			},
			{
				name: "núm_fila",
				description: "selecciona, en Matriz o Referencia, la fila desde la cual se devolverá un valor. Si se omite, se requerirá núm_columna"
			},
			{
				name: "núm_columna",
				description: "selecciona, en Matriz o Referencia, la columna desde la cual se devolverá un valor. Si se omite, se requerirá núm_fila"
			}
		]
	},
	{
		name: "INDIRECTO",
		description: "Devuelve una referencia especificada por un valor de texto.",
		arguments: [
			{
				name: "ref",
				description: "es una referencia a una celda que contiene una referencia de tipo A1, de tipo F1C1, un nombre definido como referencia o una referencia a una celda como cadena de texto"
			},
			{
				name: "a1",
				description: "es un valor lógico que especifica el tipo de referencia en ref_texto: estilo F1C1 = FALSO; estilo A1 = VERDADERO u omitido"
			}
		]
	},
	{
		name: "INFO",
		description: "Devuelve información acerca del entorno operativo en uso.",
		arguments: [
			{
				name: "tipo",
				description: "es texto que especifica el tipo de información que se desea obtener."
			}
		]
	},
	{
		name: "INT.ACUM.V",
		description: "Devuelve el interés devengado para un valor bursátil que paga intereses al vencimiento.",
		arguments: [
			{
				name: "emisión",
				description: "es la fecha de emisión del valor bursátil, expresada como un número de fecha de serie"
			},
			{
				name: "liquidación",
				description: "es la fecha de vencimiento del valor bursátil, expresada como un número de fecha de serie"
			},
			{
				name: "tasa",
				description: "es la tasa del cupón anual del valor bursátil"
			},
			{
				name: "par",
				description: "es el valor de paridad del valor bursátil"
			},
			{
				name: "base",
				description: "determina en qué tipo de base deben ser contados los días"
			}
		]
	},
	{
		name: "INT.EFECTIVO",
		description: "Devuelve la tasa de interés anual efectiva.",
		arguments: [
			{
				name: "tasa_nominal",
				description: "es la tasa de interés nominal"
			},
			{
				name: "núm_per_año",
				description: "es el número de períodos por año"
			}
		]
	},
	{
		name: "INT.PAGO.DIR",
		description: "Devuelve el interés de un préstamo de pagos directos.",
		arguments: [
			{
				name: "tasa",
				description: "es la tasa de interés por período. Por ejemplo, use 6%/4 para pagos trimestrales al 6% TPA"
			},
			{
				name: "período",
				description: "es el período para el que desea averiguar el interés"
			},
			{
				name: "nper",
				description: "es el número total de períodos de pago en una anualidad"
			},
			{
				name: "va",
				description: "es la suma total del valor de una serie de pagos futuros"
			}
		]
	},
	{
		name: "INTERSECCION.EJE",
		description: "Calcula el punto en el cual una línea intersectará el eje Y usando una línea de regresión optimizada trazada a través de los valores conocidos de X e Y.",
		arguments: [
			{
				name: "conocido_y",
				description: "es el conjunto de observaciones o datos dependientes y puede ser: números, nombres, matrices o referencias que contengan números"
			},
			{
				name: "conocido_x",
				description: "es el conjunto de observaciones, o datos independientes, formado por: números, nombres, matrices o referencias que contengan números"
			}
		]
	},
	{
		name: "INTERVALO.CONFIANZA",
		description: "Devuelve el intervalo de confianza para la media de una población, con una distribución normal.",
		arguments: [
			{
				name: "alfa",
				description: "es el nivel de significancia empleado para calcular el nivel de confianza, un número mayor que 0 y menor que 1"
			},
			{
				name: "desv_estándar",
				description: "es la desviación estándar de la población para el rango de datos y se presupone que es conocida. Desv_estándar debe ser mayor que 0"
			},
			{
				name: "tamaño",
				description: "es el tamaño de la muestra"
			}
		]
	},
	{
		name: "INTERVALO.CONFIANZA.NORM",
		description: "Devuelve el intervalo de confianza para una media de población con una distribución normal.",
		arguments: [
			{
				name: "alfa",
				description: "es el nivel de significación utilizado para calcular el nivel de confianza, un número mayor que 0 y menor que 1"
			},
			{
				name: "desv_estándar",
				description: "es la desviación de estándar de población para el rango de datos y se presupone que se conoce el valor. Desv_estándar debe ser mayor que 0"
			},
			{
				name: "tamaño",
				description: "es el tamaño de la muestra"
			}
		]
	},
	{
		name: "INTERVALO.CONFIANZA.T",
		description: "Devuelve el intervalo de confianza para una media de población con distribución de T de Student.",
		arguments: [
			{
				name: "alfa",
				description: "es el nivel de significación utilizado para calcular el nivel de confianza, un número mayor que 0 y menor que 1"
			},
			{
				name: "desv_estándar",
				description: "es la desviación de estándar de población para el rango de datos y se presupone que se conoce el valor. Desv_estándar debe ser mayor que 0"
			},
			{
				name: "tamaño",
				description: "es el tamaño de la muestra"
			}
		]
	},
	{
		name: "INV.BETA.N",
		description: "Devuelve el inverso de la función de densidad de probabilidad beta acumulativa (DISTR.BETA.N).",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada a la distribución beta"
			},
			{
				name: "alfa",
				description: "es un parámetro para la distribución y debe ser mayor que 0"
			},
			{
				name: "beta",
				description: "es un parámetro para la distribución y debe ser mayor que 0"
			},
			{
				name: "A",
				description: "es un límite inferior opcional del intervalo de x. Si se omite, A = 0"
			},
			{
				name: "B",
				description: "es un límite superior opcional del intervalo de x. Si se omite, B = 1"
			}
		]
	},
	{
		name: "INV.BINOM",
		description: "Devuelve el menor valor cuya distribución binomial acumulativa es mayor o igual que un valor de criterio.",
		arguments: [
			{
				name: "ensayos",
				description: "es el número de ensayos de Bernoulli"
			},
			{
				name: "prob_éxito",
				description: "es la probabilidad de éxito en cada ensayo, un número entre 0 y 1 inclusive"
			},
			{
				name: "alfa",
				description: "es el valor del criterio, un número entre 0 y 1 inclusive"
			}
		]
	},
	{
		name: "INV.CHICUAD",
		description: "Devuelve el inverso de la probabilidad de cola izquierda de la distribución chi cuadrado.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada a la distribución chi cuadrado, un valor entre 0 y 1 inclusive"
			},
			{
				name: "grados_de_libertad",
				description: "es el número de grados de libertad, un número entre 1 y 10^10, excluyendo 10^10"
			}
		]
	},
	{
		name: "INV.CHICUAD.CD",
		description: "Devuelve el inverso de la probabilidad de cola derecha de la distribución chi cuadrado.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada a la distribución chi cuadrado, un valor entre 0 y 1 inclusive"
			},
			{
				name: "grados_de_libertad",
				description: "es el número de grados de libertad, un número entre 1 y 10^10, excluyendo 10^10"
			}
		]
	},
	{
		name: "INV.F",
		description: "Devuelve el inverso de la distribución de probabilidad F (de cola izquierda): si p = DISTR.F(x,...), entonces INV.F(p,...) = x.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada a la distribución acumulativa F, un número entre 0 y 1 inclusive"
			},
			{
				name: "grados_de_libertad1",
				description: "es el número de grados de libertad del numerador, un número entre 1 y 10^10, excluido 10^10"
			},
			{
				name: "grados_de_libertad2",
				description: "es el número de grados de libertad del denominador, un número entre 1 y 10^10, excluido 10^10"
			}
		]
	},
	{
		name: "INV.F.CD",
		description: "Devuelve el inverso de la distribución de probabilidad F (de cola derecha): si p = DISTR.F.CD(x,...), entonces INV.F.CD(p,...) = x.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada a la distribución acumulativa F, un número entre 0 y 1 inclusive"
			},
			{
				name: "grados_de_libertad1",
				description: "es el número de grados de libertad del numerador, un número entre 1 y 10^10, excluyendo 10^10"
			},
			{
				name: "grados_de_libertad2",
				description: "es el número de grados de libertad del denominador, un número entre 1 y 10^10, excluyendo 10^10"
			}
		]
	},
	{
		name: "INV.GAMMA",
		description: "Devuelve el inverso de la distribución gamma acumulativa: si p = DISTR.GAMMA.N(x,...), entonces INV.GAMMA(p,...) = x.",
		arguments: [
			{
				name: "probabilidad",
				description: "es la probabilidad asociada con la distribución gamma, un número entre 0 y 1 inclusive"
			},
			{
				name: "alfa",
				description: "es un parámetro de la distribución, un número positivo"
			},
			{
				name: "beta",
				description: "es un parámetro de la distribución, un número positivo. Si beta = 1, INV.GAMMA devuelve el inverso de la distribución gamma estándar"
			}
		]
	},
	{
		name: "INV.LOGNORM",
		description: "Devuelve el inverso de la distribución logarítmico-normal de x, donde ln(x) se distribuye de forma normal con los parámetros Media y desv_estándar.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada con la distribución logarítmico-normal, un número entre 0 y 1 inclusive"
			},
			{
				name: "media",
				description: "es la media de ln(x)"
			},
			{
				name: "desv_estándar",
				description: "es la desviación estándar de In(x), un número positivo"
			}
		]
	},
	{
		name: "INV.NORM",
		description: "Devuelve el inverso de la distribución acumulativa normal para la media y desviación estándar especificadas.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada a la distribución normal, un número entre 0 y 1 inclusive"
			},
			{
				name: "media",
				description: "es la media aritmética de la distribución"
			},
			{
				name: "desv_estándar",
				description: "es la desviación estándar de la distribución, un número positivo"
			}
		]
	},
	{
		name: "INV.NORM.ESTAND",
		description: "Devuelve el inverso de la distribución normal estándar acumulativa. Tiene una media de cero y una desviación estándar de uno.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada a la distribución normal, un número entre 0 y 1 inclusive"
			}
		]
	},
	{
		name: "INV.T",
		description: "Devuelve el inverso de cola izquierda de la distribución t de Student.",
		arguments: [
			{
				name: "probabilidad",
				description: "es la probabilidad asociada a la distribución t de Student de dos colas, un número entre 0 y 1 inclusive"
			},
			{
				name: "grados_de_libertad",
				description: "es un entero positivo que indica el número de grados de libertad que caracteriza la distribución"
			}
		]
	},
	{
		name: "INV.T.2C",
		description: "Devuelve el inverso de dos colas de la distribución t de Student.",
		arguments: [
			{
				name: "probabilidad",
				description: "es la probabilidad asociada a la distribución t Student de dos colas, un número entre 0 y 1 inclusive"
			},
			{
				name: "grados_de_libertad",
				description: "es un entero positivo que indica el número de grados de libertad que caracteriza la distribución"
			}
		]
	},
	{
		name: "ISO.NUM.DE.SEMANA",
		description: "Devuelve el número de semana ISO del año para una fecha determinada.",
		arguments: [
			{
				name: "fecha",
				description: "Es el código de fecha y hora que usa Spreadsheet para calcular la fecha y la hora"
			}
		]
	},
	{
		name: "IZQUIERDA",
		description: "Devuelve el número especificado de caracteres del principio de una cadena de texto.",
		arguments: [
			{
				name: "texto",
				description: "es la cadena de texto que contiene los caracteres que desea extraer"
			},
			{
				name: "núm_de_caracteres",
				description: "especifica el número de caracteres que se desea que IZQUIERDA extraiga. Si se omite, se asume 1"
			}
		]
	},
	{
		name: "JERARQUIA",
		description: "Devuelve la jerarquía de un número dentro de una lista: su tamaño depende de los otros valores de la lista.",
		arguments: [
			{
				name: "número",
				description: "es el número cuya jerarquía desea conocer"
			},
			{
				name: "referencia",
				description: "es una matriz de una lista de números o una referencia a la misma. Omite los valores no numéricos"
			},
			{
				name: "orden",
				description: "es un número: si la jerarquía en la lista se ordena de forma descendente = 0 u omitido; si la jerarquía en la lista se ordena de forma ascendente = cualquier valor distinto de cero"
			}
		]
	},
	{
		name: "JERARQUIA.EQV",
		description: "Devuelve la jerarquía de un número dentro de una lista de números: su tamaño en relación con otros valores de la lista; si más de un valor tiene la misma jerarquía, se devuelve la jerarquía superior de ese conjunto de valores.",
		arguments: [
			{
				name: "número",
				description: "es el número del que desea encontrar la jerarquía"
			},
			{
				name: "referencia",
				description: "es una matriz de, o una referencia a, una lista de números. Se omiten los valores no numéricos"
			},
			{
				name: "orden",
				description: "es un número: jerarquía en la lista en orden descendente = 0 o se omite; jerarquía en la lista en orden ascendente = cualquier valor distinto de cero"
			}
		]
	},
	{
		name: "JERARQUIA.MEDIA",
		description: "Devuelve la jerarquía de un número dentro de una lista de números: su tamaño en relación con otros valores de la lista; si más de un valor tiene la misma jerarquía, se devuelve el promedio de jerarquía.",
		arguments: [
			{
				name: "número",
				description: "es el número del que desea encontrar la jerarquía"
			},
			{
				name: "referencia",
				description: "es una matriz de, o una referencia a, una lista de números. Se omiten los valores no numéricos"
			},
			{
				name: "orden",
				description: "es un número: jerarquía en la lista en orden descendente = 0 o se omite; jerarquía en la lista en orden ascendente = cualquier valor distinto de cero"
			}
		]
	},
	{
		name: "K.ESIMO.MAYOR",
		description: "Devuelve el valor k-ésimo mayor de un conjunto de datos. Por ejemplo, el trigésimo número más grande.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de datos cuyo valor k-ésimo mayor desea determinar"
			},
			{
				name: "k",
				description: "representa dentro de la matriz o rango de datos la posición, a partir del valor más alto, del dato a devolver"
			}
		]
	},
	{
		name: "K.ESIMO.MENOR",
		description: "Devuelve el valor k-ésimo menor de un conjunto de datos. Por ejemplo, el trigésimo número menor.",
		arguments: [
			{
				name: "matriz",
				description: "es una matriz o rango de datos numéricos cuyo valor k-ésimo menor desea determinar"
			},
			{
				name: "k",
				description: "representa dentro de la matriz o rango de datos la posición, a partir de valor más bajo, del dato a devolver"
			}
		]
	},
	{
		name: "LARGO",
		description: "Devuelve el número de caracteres de una cadena de texto.",
		arguments: [
			{
				name: "texto",
				description: "es el texto cuya longitud desea conocer. Los espacios cuentan como caracteres"
			}
		]
	},
	{
		name: "LETRA.DE.TES.PRECIO",
		description: "Devuelve el precio de un valor nominal de 100 $ para una letra de tesorería.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de liquidación de la letra de tesorería, expresada como un número de fecha de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de vencimiento de la letra de tesorería, expresada con un número de fecha de serie"
			},
			{
				name: "descuento",
				description: "es la tasa de descuento de la letra de tesorería"
			}
		]
	},
	{
		name: "LETRA.DE.TES.RENDTO",
		description: "Devuelve el rendimiento de una letra de tesorería.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de liquidación de la letra de tesorería, expresada como un número de fecha de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de vencimiento de la letra de tesorería, expresada como un número de fecha de serie"
			},
			{
				name: "pr",
				description: "es el precio de la letra de tesorería de un valor nominal de 100 $"
			}
		]
	},
	{
		name: "LETRA.DE.TEST.EQV.A.BONO",
		description: "Devuelve el rendimiento para un bono equivalente a una letra de tesorería.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de liquidación de la letra de tesorería, expresada como un número de fecha de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de vencimiento de la letra de tesorería, expresada como un número de fecha de serie"
			},
			{
				name: "descuento",
				description: "es la tasa de descuento de la letra de tesorería"
			}
		]
	},
	{
		name: "LIMPIAR",
		description: "Quita todos los caracteres no imprimibles del texto.",
		arguments: [
			{
				name: "texto",
				description: "es cualquier información de hoja de cálculo de la cual se desea quitar los caracteres no imprimibles"
			}
		]
	},
	{
		name: "LN",
		description: "Devuelve el logaritmo natural de un número.",
		arguments: [
			{
				name: "número",
				description: "es el número real positivo para el que se desea obtener el logaritmo natural"
			}
		]
	},
	{
		name: "LOG",
		description: "Devuelve el logaritmo de un número en la base especificada.",
		arguments: [
			{
				name: "número",
				description: "es el número real positivo para el que se desea obtener el logaritmo"
			},
			{
				name: "base",
				description: "es la base del logaritmo. Si se omite, se asume 10"
			}
		]
	},
	{
		name: "LOG10",
		description: "Devuelve el logaritmo en base 10 de un número.",
		arguments: [
			{
				name: "número",
				description: "es el número real positivo para el cual se desea el logaritmo en base 10"
			}
		]
	},
	{
		name: "M.C.D",
		description: "Devuelve el máximo común divisor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son valores del 1 al 255"
			},
			{
				name: "número2",
				description: "son valores del 1 al 255"
			}
		]
	},
	{
		name: "M.C.M",
		description: "Devuelve el mínimo común múltiplo.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son valores del 1 al 255 de los que desea el múltiplo menos común"
			},
			{
				name: "número2",
				description: "son valores del 1 al 255 de los que desea el múltiplo menos común"
			}
		]
	},
	{
		name: "M.UNIDAD",
		description: "Devuelve la matriz de la unidad para la dimensión especificada.",
		arguments: [
			{
				name: "dimension",
				description: "Es un entero que especifica la dimensión de la matriz de la unidad que quieres devolver"
			}
		]
	},
	{
		name: "MAX",
		description: "Devuelve el valor máximo de una lista de valores. Omite los valores lógicos y el texto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números, celdas vacías, valores lógicos o números en forma de texto para los cuales desea encontrar el máximo"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números, celdas vacías, valores lógicos o números en forma de texto para los cuales desea encontrar el máximo"
			}
		]
	},
	{
		name: "MAXA",
		description: "Devuelve el valor máximo de un conjunto de valores. Incluye valores lógicos y texto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "son de 1 a 255 números, celdas vacías, valores lógicos o números en forma de texto cuyo máximo desea calcular"
			},
			{
				name: "valor2",
				description: "son de 1 a 255 números, celdas vacías, valores lógicos o números en forma de texto cuyo máximo desea calcular"
			}
		]
	},
	{
		name: "MAYOR.O.IGUAL",
		description: "Prueba si un número es mayor que el valor de referencia.",
		arguments: [
			{
				name: "número",
				description: "es el valor a comparar con el argumento valor_red"
			},
			{
				name: "paso",
				description: "es el valor de referencia"
			}
		]
	},
	{
		name: "MAYUSC",
		description: "Convierte una cadena de texto en letras mayúsculas.",
		arguments: [
			{
				name: "texto",
				description: "es el texto que se desea convertir en mayúsculas, una referencia o una cadena de texto"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Devuelve el determinante matricial de una matriz.",
		arguments: [
			{
				name: "matriz",
				description: "es una matriz numérica con el mismo número de filas y columnas y puede ser un rango de celdas o una constante matricial"
			}
		]
	},
	{
		name: "MEDIA.ACOTADA",
		description: "Devuelve la media de la porción interior de un conjunto de valores de datos.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de valores que desea acotar y calcular su media"
			},
			{
				name: "porcentaje",
				description: "es el número fraccionario de puntos de datos que se excluyen del extremo superior e inferior del conjunto de datos"
			}
		]
	},
	{
		name: "MEDIA.ARMO",
		description: "Devuelve la media armónica de un conjunto de números positivos: el recíproco de la media aritmética de los recíprocos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números, argumentos, matrices o referencias que contienen números cuya media armónica desea calcular"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números, argumentos, matrices o referencias que contienen números cuya media armónica desea calcular"
			}
		]
	},
	{
		name: "MEDIA.GEOM",
		description: "Devuelve la media geométrica de una matriz o rango de datos numéricos positivos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números, argumentos, matrices o referencias que contienen números cuya media se desea calcular"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números, argumentos, matrices o referencias que contienen números cuya media se desea calcular"
			}
		]
	},
	{
		name: "MEDIANA",
		description: "Devuelve la mediana o el número central de un conjunto de números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números, nombres, matrices o referencias que contienen números, para los cuales desea obtener la mediana"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números, nombres, matrices o referencias que contienen números, para los cuales desea obtener la mediana"
			}
		]
	},
	{
		name: "MES",
		description: "Devuelve el mes, un número entero de 1 (enero) a 12 (diciembre).",
		arguments: [
			{
				name: "núm_de_serie",
				description: "es un número en el código de fecha y hora usado por Spreadsheet"
			}
		]
	},
	{
		name: "MIN",
		description: "Devuelve el valor mínimo de una lista de valores. Omite los valores lógicos y el texto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números, celdas vacías, valores lógicos o números en forma de texto, para los cuales desea obtener el mínimo"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números, celdas vacías, valores lógicos o números en forma de texto, para los cuales desea obtener el mínimo"
			}
		]
	},
	{
		name: "MINA",
		description: "Devuelve el valor mínimo de un conjunto de valores. Incluye valores lógicos y texto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "son de 1 a 255 números, celdas vacías, valores lógicos o números en forma de texto cuyo mínimo desea calcular"
			},
			{
				name: "valor2",
				description: "son de 1 a 255 números, celdas vacías, valores lógicos o números en forma de texto cuyo mínimo desea calcular"
			}
		]
	},
	{
		name: "MINUSC",
		description: "Convierte todas las letras de una cadena de texto en minúsculas.",
		arguments: [
			{
				name: "texto",
				description: "es el texto que se desea convertir en minúsculas. Los caracteres en Texto que no sean letras no cambiarán"
			}
		]
	},
	{
		name: "MINUTO",
		description: "Devuelve el minuto, un número de 0 a 59.",
		arguments: [
			{
				name: "núm_de_serie",
				description: "es un número en el código de fecha y hora usado por Spreadsheet, o texto en formato de hora, como por ejemplo 16:48:00 o 4:48:00 p.m."
			}
		]
	},
	{
		name: "MINVERSA",
		description: "Devuelve la matriz inversa de una matriz dentro de una matriz.",
		arguments: [
			{
				name: "matriz",
				description: "es una matriz numérica con el mismo número de filas y columnas, y puede ser un rango de celdas o una constante matricial"
			}
		]
	},
	{
		name: "MMULT",
		description: "Devuelve el producto matricial de dos matrices, una matriz con el mismo número de filas que Matriz1 y columnas que Matriz2.",
		arguments: [
			{
				name: "matriz1",
				description: "son las matrices que se desea multiplicar y debe tener el mismo número de columnas que filas hay en Matriz2"
			},
			{
				name: "matriz2",
				description: "son las matrices que se desea multiplicar y debe tener el mismo número de columnas que filas hay en Matriz2"
			}
		]
	},
	{
		name: "MODA",
		description: "Devuelve el valor más frecuente o que más se repite en una matriz o rango de datos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números, nombres, matrices o referencias que contienen números cuya moda desea calcular"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números, nombres, matrices o referencias que contienen números cuya moda desea calcular"
			}
		]
	},
	{
		name: "MODA.UNO",
		description: "Devuelve el valor más frecuente o repetitivo de una matriz o rango de datos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números, nombres, matrices o referencias que contienen números para los que desea la moda"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números, nombres, matrices o referencias que contienen números para los que desea la moda"
			}
		]
	},
	{
		name: "MODA.VARIOS",
		description: "Devuelve una matriz vertical de los valores más frecuente o repetitivos de una matriz o rango de datos. Para una matriz horizontal, use =TRANSPONER(MODA.VARIOS(número1,número2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números, nombres, matrices o referencias que contienen números para los que desea la moda"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números, nombres, matrices o referencias que contienen números para los que desea la moda"
			}
		]
	},
	{
		name: "MONEDA",
		description: "Convierte un número en texto usando formato de moneda.",
		arguments: [
			{
				name: "número",
				description: "es un número, una referencia a una celda que contiene un número o una fórmula que evalúa un número"
			},
			{
				name: "núm_de_decimales",
				description: "es el número de dígitos a la derecha del separador decimal. El número se redondea si es necesario. Si se omite se establecerá: Decimales = 2"
			}
		]
	},
	{
		name: "MONEDA.DEC",
		description: "Convierte un precio en dólar, expresado como fracción, en un precio en dólares, expresado como número decimal.",
		arguments: [
			{
				name: "dólar_fraccional",
				description: "es un número expresado como fracción"
			},
			{
				name: "fracción",
				description: "es el entero para utilizar en el denominador de la fracción"
			}
		]
	},
	{
		name: "MONEDA.FRAC",
		description: "Convierte un precio en dólar, expresado como número decimal en un precio en dólares, expresado como una fracción.",
		arguments: [
			{
				name: "dólar_decimal",
				description: "es un número decimal"
			},
			{
				name: "fracción",
				description: "es el entero que debe utilizar en el denominador de una fracción"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Devuelve el polinomio de un conjunto de números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "valores de 1 a 255 para los que desea el polinomio"
			},
			{
				name: "número2",
				description: "valores de 1 a 255 para los que desea el polinomio"
			}
		]
	},
	{
		name: "MULTIPLO.INFERIOR",
		description: "Redondea un número hacia abajo, hasta el múltiplo significativo más cercano.",
		arguments: [
			{
				name: "número",
				description: "es el valor numérico que se desea redondear"
			},
			{
				name: "cifra_significativa",
				description: "es el múltiplo hacia el que se desea redondear. Número y Cifra significativa deben ser ambos positivos o negativos"
			}
		]
	},
	{
		name: "MULTIPLO.INFERIOR.EXACTO",
		description: "Redondea un número hacia abajo, hasta el múltiplo o el entero más cercano significativo.",
		arguments: [
			{
				name: "número",
				description: "es el valor numérico que se desea redondear"
			},
			{
				name: "cifra_significativa",
				description: "es el múltiplo hacia el que se desea redondear. "
			}
		]
	},
	{
		name: "MULTIPLO.INFERIOR.MAT",
		description: "Redondea un número para abajo, al entero más cercano o al múltiplo significativo más cercano.",
		arguments: [
			{
				name: "número",
				description: "Es el valor que quieres redondear"
			},
			{
				name: "cifra_significativa",
				description: "Es el múltiplo al que quieres redondear"
			},
			{
				name: "moda",
				description: "Cuando está dada y no es nula, esta función se redondea hacia cero"
			}
		]
	},
	{
		name: "MULTIPLO.SUPERIOR",
		description: "Redondea un número hacia arriba, hasta el múltiplo significativo más cercano.",
		arguments: [
			{
				name: "número",
				description: "es el valor que se desea redondear"
			},
			{
				name: "cifra_significativa",
				description: "es el múltiplo hacia el que se desea redondear"
			}
		]
	},
	{
		name: "MULTIPLO.SUPERIOR.EXACTO",
		description: "Redondea un número hacia arriba, al entero o múltiplo significativo más próximo.",
		arguments: [
			{
				name: "número",
				description: "es el valor que se desea redondear"
			},
			{
				name: "cifra_significativa",
				description: "es el múltiplo al que desea redondear"
			}
		]
	},
	{
		name: "MULTIPLO.SUPERIOR.ISO",
		description: "Redondea un número hacia arriba, al entero o múltiplo significativo más cercano.",
		arguments: [
			{
				name: "número",
				description: "es el valor que desea redondear"
			},
			{
				name: "cifra_significativa",
				description: "es el múltiplo opcional hacia el que desea redondear"
			}
		]
	},
	{
		name: "MULTIPLO.SUPERIOR.MAT",
		description: "Redondea un número para arriba, al entero más cercano o al múltiplo significativo más cercano.",
		arguments: [
			{
				name: "número",
				description: "Es el valor que quieres redondear"
			},
			{
				name: "cifra_significativa",
				description: "Es el múltiplo al que quieres redondear"
			},
			{
				name: "moda",
				description: "Cuando está dada y no es nula, esta función se redondea a un valor alejado del cero"
			}
		]
	},
	{
		name: "N",
		description: "Convierte valores no numéricos en números, fechas en números de serie, VERDADERO en 1 y cualquier otro en 0 (cero).",
		arguments: [
			{
				name: "valor",
				description: "es el valor que se desea convertir"
			}
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Devuelve la distribución binomial negativa, la probabilidad de encontrar núm_fracasos antes que núm_éxito, con probabilidad probabilidad_s de éxito.",
		arguments: [
			{
				name: "núm_fracasos",
				description: "es el número de fracasos"
			},
			{
				name: "núm_éxitos",
				description: "es el número de umbral de éxitos"
			},
			{
				name: "prob_éxito",
				description: "es la probabilidad de éxito; un número entre 0 y 1"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la función de distribución acumulativa = VERDADERO; para usar la función de densidad de probabilidad = FALSO"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Devuelve la distribución binomial negativa, la probabilidad de encontrar núm_fracasos antes que núm_éxito, con la probabilidad probabilidad_éxito de éxito.",
		arguments: [
			{
				name: "núm_fracasos",
				description: "es el número de fracasos"
			},
			{
				name: "núm_éxitos",
				description: "es el número de umbral de éxitos"
			},
			{
				name: "prob_éxito",
				description: "es la probabilidad de éxito; un número entre 0 y 1"
			}
		]
	},
	{
		name: "NO",
		description: "Cambia FALSO por VERDADERO y VERDADERO por FALSO.",
		arguments: [
			{
				name: "valor_lógico",
				description: "es un valor lógico o expresión que se puede evaluar como VERDADERO o FALSO"
			}
		]
	},
	{
		name: "NOD",
		description: "Devuelve el valor de error #N/A (valor no disponible).",
		arguments: [
		]
	},
	{
		name: "NOMPROPIO",
		description: "Convierte una cadena de texto en mayúsculas o minúsculas, según corresponda; la primera letra de cada palabra en mayúscula y las demás letras en minúscula.",
		arguments: [
			{
				name: "texto",
				description: "es el texto entre comillas, una fórmula que devuelve texto o una referencia a una celda que contiene el texto al que se desea agregar mayúsculas"
			}
		]
	},
	{
		name: "NORMALIZACION",
		description: "Devuelve un valor normalizado de una distribución caracterizada por una media y desviación estándar.",
		arguments: [
			{
				name: "x",
				description: "es el valor que desea normalizar"
			},
			{
				name: "media",
				description: "es la media aritmética de la distribución"
			},
			{
				name: "desv_estándar",
				description: "es la desviación estándar de la distribución, un número positivo"
			}
		]
	},
	{
		name: "NPER",
		description: "Devuelve el número de pagos de una inversión, basado en pagos constantes y periódicos y una tasa de interés constante.",
		arguments: [
			{
				name: "tasa",
				description: "es la tasa de interés por período. Por ejemplo, use 6%/4 para pagos trimestrales al 6% TPA"
			},
			{
				name: "pago",
				description: "es el pago efectuado en cada período; no puede cambiar durante la vigencia de la inversión"
			},
			{
				name: "va",
				description: "es el valor actual o el valor de la suma total de una serie de pagos futuros"
			},
			{
				name: "vf",
				description: "es el valor futuro o saldo en efectivo que se desea lograr después de efectuar el último pago. Si se omite, se usa cero"
			},
			{
				name: "tipo",
				description: "es un valor lógico: para pago al comienzo del período = 1; para pago al final del período = 0 u omitido"
			}
		]
	},
	{
		name: "NSHORA",
		description: "Convierte horas, minutos y segundos dados como números en un número de serie de Spreadsheet, con formato de hora.",
		arguments: [
			{
				name: "hora",
				description: "es un número entre 0 y 23 que representa la hora"
			},
			{
				name: "minuto",
				description: "es un número entre 0 y 59 que representa los minutos"
			},
			{
				name: "segundo",
				description: "es un número entre 0 y 59 que representa los segundos"
			}
		]
	},
	{
		name: "NUM.DE.SEMANA",
		description: "Devuelve el número de semanas en el año.",
		arguments: [
			{
				name: "número_serie",
				description: "es el código de fecha-hora utilizado por Spreadsheet para los cálculos de fecha y hora"
			},
			{
				name: "tipo_devuelto",
				description: "es un número (1 o 2) que determina el tipo de valor devuelto"
			}
		]
	},
	{
		name: "NUMERO.ARABE",
		description: "Convierte un número romano en arábigo.",
		arguments: [
			{
				name: "texto",
				description: "Es el número romano que quieres convertir"
			}
		]
	},
	{
		name: "NUMERO.ROMANO",
		description: "Convierte un número arábigo en romano, en formato de texto.",
		arguments: [
			{
				name: "número",
				description: "es el número arábigo que desea convertir"
			},
			{
				name: "forma",
				description: "es el número que especifica el tipo de número romano que desea."
			}
		]
	},
	{
		name: "O",
		description: "Comprueba si alguno de los argumentos es VERDADERO, y devuelve VERDADERO o FALSO. Devuelve FALSO si todos los argumentos son FALSOS.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor_lógico1",
				description: "son entre 1 y 255 condiciones que se desea comprobar y que pueden ser VERDADERO o FALSO"
			},
			{
				name: "valor_lógico2",
				description: "son entre 1 y 255 condiciones que se desea comprobar y que pueden ser VERDADERO o FALSO"
			}
		]
	},
	{
		name: "OCT.A.BIN",
		description: "Convierte un número octal en binario.",
		arguments: [
			{
				name: "número",
				description: "es el número octal que desea convertir"
			},
			{
				name: "posiciones",
				description: "es el número de caracteres que se deben usar"
			}
		]
	},
	{
		name: "OCT.A.DEC",
		description: "Convierte un número octal en decimal.",
		arguments: [
			{
				name: "número",
				description: "es el número octal que desea convertir"
			}
		]
	},
	{
		name: "OCT.A.HEX",
		description: "Convierte un número octal en hexadecimal.",
		arguments: [
			{
				name: "número",
				description: "es el número octal que desea convertir"
			},
			{
				name: "posiciones",
				description: "es el número de caracteres que se deben usar"
			}
		]
	},
	{
		name: "P.DURACION",
		description: "Devuelve la cantidad de períodos necesarios para que una inversión alcance un valor especificado.",
		arguments: [
			{
				name: "tasa",
				description: "Es la tasa de interés por período."
			},
			{
				name: "va",
				description: "Es el valor actual de la inversión"
			},
			{
				name: "vf",
				description: "Es el valor futuro deseado de la inversión"
			}
		]
	},
	{
		name: "PAGO",
		description: "Calcula el pago de un préstamo basado en pagos y tasa de interés constantes.",
		arguments: [
			{
				name: "tasa",
				description: "es la tasa de interés por período del préstamo. Por ejemplo, use 6%/4 para pagos trimestrales al 6% TPA"
			},
			{
				name: "nper",
				description: "es el número total de pagos del préstamo"
			},
			{
				name: "va",
				description: "es el valor actual: la cantidad total de una serie de pagos futuros"
			},
			{
				name: "vf",
				description: "es el valor futuro o saldo en efectivo que se desea lograr después de efectuar el último pago y que se asume 0 (cero) si se omite"
			},
			{
				name: "tipo",
				description: "es un valor lógico: para pago al comienzo del período = 1; para pago al final del período = 0 u omitido"
			}
		]
	},
	{
		name: "PAGO.INT.ENTRE",
		description: "Devuelve el pago de intereses acumulativo entre dos períodos.",
		arguments: [
			{
				name: "tasa",
				description: "es la tasa de interés"
			},
			{
				name: "nper",
				description: "es el número total de períodos de pago"
			},
			{
				name: "va",
				description: "es el valor actual"
			},
			{
				name: "período_inicial",
				description: "es el primer período del cálculo"
			},
			{
				name: "período_final",
				description: "es el último período del cálculo"
			},
			{
				name: "tipo",
				description: "es cuando vencen los pagos"
			}
		]
	},
	{
		name: "PAGO.PRINC.ENTRE",
		description: "Devuelve el paso principal acumulativo de un préstamo entre dos períodos.",
		arguments: [
			{
				name: "tasa",
				description: "es la tasa de interés"
			},
			{
				name: "nper",
				description: "es el número total de períodos de pago"
			},
			{
				name: "va",
				description: "es el valor actual"
			},
			{
				name: "período_inicial",
				description: "es el primer período del cálculo"
			},
			{
				name: "período_final",
				description: "es el último período del cálculo"
			},
			{
				name: "tipo",
				description: "es cuando vencen los pagos"
			}
		]
	},
	{
		name: "PAGOINT",
		description: "Devuelve el interés pagado por una inversión durante un período determinado, basado en pagos periódicos y constantes y una tasa de interés constante.",
		arguments: [
			{
				name: "tasa",
				description: "es la tasa de interés por período. Por ejemplo, use 6%/4 para pagos trimestrales al 6% de TPA"
			},
			{
				name: "período",
				description: "es el período para el que se desea encontrar el interés, que deberá estar en el rango de 1 a Nper"
			},
			{
				name: "nper",
				description: "es el número total de períodos de pago en una inversión"
			},
			{
				name: "va",
				description: "es el valor actual o la suma total de una serie de pagos futuros"
			},
			{
				name: "vf",
				description: "es el valor futuro o saldo en efectivo que se desea obtener después de efectuar el último pago. Si se omite, se asume VA = 0"
			},
			{
				name: "tipo",
				description: "es un valor lógico que representa cuándo vencen los pagos: si se omite o está al final del período = 0, al comienzo del período = 1"
			}
		]
	},
	{
		name: "PAGOPRIN",
		description: "Devuelve el pago del capital de una inversión determinada, basado en pagos constantes y periódicos, y una tasa de interés constante.",
		arguments: [
			{
				name: "tasa",
				description: "es la tasa de interés por período. Por ejemplo, use 6%/4 para pagos trimestrales al 6% de TPA"
			},
			{
				name: "período",
				description: "especifica el período y deberá encontrarse en el rango comprendido entre 1 y nper"
			},
			{
				name: "nper",
				description: "es el número total de períodos de pago en una inversión"
			},
			{
				name: "va",
				description: "es el valor actual: la cantidad total de una serie de pagos futuros"
			},
			{
				name: "vf",
				description: "es el valor futuro o saldo en efectivo que se desea lograr después de efectuar el último pago"
			},
			{
				name: "tipo",
				description: "es un valor lógico: para pago al comienzo del período = 1; para pago al final del período = 0 u omitido"
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
		name: "PEARSON",
		description: "Devuelve el coeficiente de correlación producto o momento r de Pearson, r.",
		arguments: [
			{
				name: "matriz1",
				description: "es un conjunto de valores independientes"
			},
			{
				name: "matriz2",
				description: "es un conjunto de valores dependientes"
			}
		]
	},
	{
		name: "PENDIENTE",
		description: "Devuelve la pendiente de una línea de regresión lineal de los puntos dados.",
		arguments: [
			{
				name: "conocido_y",
				description: "es una matriz, o rango de celdas de puntos de datos numéricos dependientes, formada por: números, nombres, matrices o referencias que contengan números"
			},
			{
				name: "conocido_x",
				description: "es el conjunto de datos independientes que puede estar compuesto por: números, nombres, matrices o referencias que contengan números"
			}
		]
	},
	{
		name: "PERCENTIL",
		description: "Devuelve el percentil k-ésimo de los valores de un rango.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de datos que define la posición relativa"
			},
			{
				name: "k",
				description: "es el valor del percentil entre 0 y 1 inclusive"
			}
		]
	},
	{
		name: "PERCENTIL.EXC",
		description: "Devuelve el percentil k-ésimo de los valores de un rango, donde k está en el rango 0..1, exclusivo.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de datos que define la posición relativa"
			},
			{
				name: "k",
				description: "es el valor del percentil entre 0 y 1, inclusive"
			}
		]
	},
	{
		name: "PERCENTIL.INC",
		description: "Devuelve el percentil k-ésimo de los valores de un rango, donde k está en el rango 0..1, inclusive.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de datos que define la posición relativa"
			},
			{
				name: "k",
				description: "es el valor del percentil entre 0 y 1, inclusive"
			}
		]
	},
	{
		name: "PERMUTACIONES",
		description: "Devuelve el número de permutaciones para un número determinado de objetos que pueden ser seleccionados de los objetos totales.",
		arguments: [
			{
				name: "número",
				description: "es un número total de objetos"
			},
			{
				name: "tamaño",
				description: "es un número de objetos en cada permutación"
			}
		]
	},
	{
		name: "PERMUTACIONES.A",
		description: "Devuelve la cantidad de permutaciones de una cantidad determinada de objetos (con repeticiones) que pueden seleccionarse del total de objetos.",
		arguments: [
			{
				name: "número",
				description: "Es la cantidad total de objetos"
			},
			{
				name: "número_elegido",
				description: "Es la cantidad de objetos en cada permutación"
			}
		]
	},
	{
		name: "PI",
		description: "Devuelve el valor Pi, 3,14159265358979, con precisión de 15 dígitos.",
		arguments: [
		]
	},
	{
		name: "POISSON",
		description: "Devuelve la distribución de Poisson.",
		arguments: [
			{
				name: "x",
				description: "es el número de eventos"
			},
			{
				name: "media",
				description: "es el valor numérico esperado, un número positivo"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la probabilidad acumulativa de Poisson = VERDADERO; para usar la función de probabilidad bruta de Poisson = FALSO"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Devuelve la distribución de Poisson.",
		arguments: [
			{
				name: "x",
				description: "es el número de eventos"
			},
			{
				name: "media",
				description: "es el valor numérico esperado, un número positivo"
			},
			{
				name: "acumulado",
				description: "es un valor lógico: para usar la probabilidad acumulativa de Poisson = VERDADERO; para usar la función de probabilidad bruta de Poisson = FALSO"
			}
		]
	},
	{
		name: "POTENCIA",
		description: "Devuelve el resultado de elevar el número a una potencia.",
		arguments: [
			{
				name: "número",
				description: "es el número base; cualquier número real"
			},
			{
				name: "potencia",
				description: "es el exponente al que desea elevar la base"
			}
		]
	},
	{
		name: "PRECIO.DESCUENTO",
		description: "Devuelve el precio por 100 $ de un valor nominal de un valor bursátil con descuento.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de liquidación del valor bursátil, expresada como número de fecha de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de vencimiento del valor bursátil, expresada como número de fecha de serie"
			},
			{
				name: "descuento",
				description: "es el tipo de descuento del valor bursátil"
			},
			{
				name: "amortización",
				description: " el valor de amortización por cada 100 $ de valor nominal"
			},
			{
				name: "base",
				description: "determina en qué tipo de base deben contarse los días"
			}
		]
	},
	{
		name: "PROBABILIDAD",
		description: "Devuelve la probabilidad de que los valores de un rango se encuentren entre dos límites o sean iguales a un límite inferior.",
		arguments: [
			{
				name: "rango_x",
				description: "es el rango de valores numéricos de X con que el que hay probabilidades asociadas"
			},
			{
				name: "rango_probabilidad",
				description: "es un conjunto de probabilidades asociado a valores del rango_x, valores entre 0 y 1, excluyendo 0"
			},
			{
				name: "límite_inf",
				description: "es el límite inferior del valor para el que desea una probabilidad"
			},
			{
				name: "límite_sup",
				description: "es el límite superior opcional del valor para el que desea una probabilidad. Si se omite, PROB devuelve la probabilidad de que los valores rango_x sean iguales a límite_inf"
			}
		]
	},
	{
		name: "PRODUCTO",
		description: "Multiplica todos los números especificados como argumentos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son entre 1 y 255 números, valores lógicos o texto que representa números que desea multiplicar"
			},
			{
				name: "número2",
				description: "son entre 1 y 255 números, valores lógicos o texto que representa números que desea multiplicar"
			}
		]
	},
	{
		name: "PROMEDIO",
		description: "Devuelve el promedio (media aritmética) de los argumentos, los cuales pueden ser números, nombres, matrices o referencias que contengan números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son entre 1 y 255 argumentos numéricos de los que se desea obtener el promedio"
			},
			{
				name: "número2",
				description: "son entre 1 y 255 argumentos numéricos de los que se desea obtener el promedio"
			}
		]
	},
	{
		name: "PROMEDIO.SI",
		description: "Busca el promedio (media aritmética) de las celdas que cumplen un determinado criterio o condición.",
		arguments: [
			{
				name: "rango",
				description: "es el rango de celdas que desea evaluar"
			},
			{
				name: "criterio",
				description: "es la condición o el criterio en forma de número, expresión o texto que determina qué celdas se utilizarán para buscar el promedio"
			},
			{
				name: "rango_promedio",
				description: "son las celdas que se van a utilizar para buscar el promedio. Si se omite, se usarán las celdas en el rango"
			}
		]
	},
	{
		name: "PROMEDIO.SI.CONJUNTO",
		description: "Busca el promedio (media aritmética) de las celdas que cumplen un determinado conjunto de condiciones o criterios.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rango_promedio",
				description: "son las celdas que se van a utilizar para buscar el promedio."
			},
			{
				name: "rango_criterios",
				description: "es el rango de celdas que desea evaluar para la condición determinada"
			},
			{
				name: "criterio",
				description: "es la condición o el criterio en forma de número, expresión o texto que determina qué celdas se utilizarán para buscar el promedio"
			}
		]
	},
	{
		name: "PROMEDIOA",
		description: "Devuelve el promedio (media aritmética) de los argumentos; 0 evalúa el texto como FALSO; 1 como VERDADERO. Los argumentos pueden ser números, nombres, matrices o referencias.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "son de 1 a 255 argumentos de los que desea obtener la media"
			},
			{
				name: "valor2",
				description: "son de 1 a 255 argumentos de los que desea obtener la media"
			}
		]
	},
	{
		name: "PRONOSTICO",
		description: "Calcula o predice un valor futuro en una tendencia lineal usando valores existentes.",
		arguments: [
			{
				name: "x",
				description: "es el punto de datos para el cual desea predecir un valor. Debe ser un valor numérico"
			},
			{
				name: "conocido_y",
				description: "es la matriz dependiente o rango de datos numéricos"
			},
			{
				name: "conocido_x",
				description: "es el rango de datos numéricos o matriz independiente. La varianza de conocido_x no debe ser cero"
			}
		]
	},
	{
		name: "PRUEBA.CHI",
		description: "Devuelve la prueba de independencia: el valor de distribución chi cuadrado para la estadística y los grados de libertad apropiados.",
		arguments: [
			{
				name: "rango_real",
				description: "es el rango de datos que contiene observaciones para contrastar frente a los valores esperados"
			},
			{
				name: "rango_esperado",
				description: "es el rango de datos que contiene el resultado del producto de los totales de filas y columnas con el total general"
			}
		]
	},
	{
		name: "PRUEBA.CHI.INV",
		description: "Devuelve el inverso de una probabilidad dada, de una cola derecha, en una distribución chi cuadrado.",
		arguments: [
			{
				name: "probabilidad",
				description: "es una probabilidad asociada con la distribución chi cuadrado, un valor entre 0 y 1 inclusive"
			},
			{
				name: "grados_de_libertad",
				description: "es el número de grados de libertad, un número entre 1 y 10^10, excluido 10^10"
			}
		]
	},
	{
		name: "PRUEBA.CHICUAD",
		description: "Devuelve la prueba de independencia: el valor de la distribución chi cuadrado para la estadística y los grados adecuados de libertad.",
		arguments: [
			{
				name: "rango_real",
				description: "es el rango de datos que contiene observaciones para contrastar frente a los valores esperados"
			},
			{
				name: "rango_esperado",
				description: "es el rango de datos que contiene el resultado del producto de los totales de filas y columnas con el total general"
			}
		]
	},
	{
		name: "PRUEBA.F",
		description: "Devuelve el resultado de una prueba F, la probabilidad de dos colas de que las varianzas en Matriz1 y Matriz2 no sean significativamente diferentes.",
		arguments: [
			{
				name: "matriz1",
				description: "es la primera matriz o rango de datos formado por números, nombres, matrices o referencias que contengan números (se omiten los que estén en blanco)"
			},
			{
				name: "matriz2",
				description: "es la segunda matriz o rango de datos formado por números, nombres, matrices o referencias que contengan números (se omiten los que estén en blanco)"
			}
		]
	},
	{
		name: "PRUEBA.F.N",
		description: "Devuelve el resultado de una prueba F, la probabilidad de dos colas de que las varianzas en Matriz 1 y Matriz 2 no sean significativamente diferentes.",
		arguments: [
			{
				name: "matriz1",
				description: "es la primera matriz o rango de datos y puede ser números o nombres, matrices o referencias que contienen números (los blancos no se tienen en cuenta)"
			},
			{
				name: "matriz2",
				description: "es la segunda matriz o rango de datos y puede ser números o nombres, matrices o referencias que contienen números (los blancos no se tienen en cuenta)"
			}
		]
	},
	{
		name: "PRUEBA.FISHER.INV",
		description: "Devuelve la función inversa de la transformación Fisher o coeficiente Z: si y = FISHER (x), entonces PRUEBA.FISHER.INV (y) = x.",
		arguments: [
			{
				name: "y",
				description: "es el valor al que se realizará la transformación inversa"
			}
		]
	},
	{
		name: "PRUEBA.T",
		description: "Devuelve la probabilidad asociada con la prueba t de Student.",
		arguments: [
			{
				name: "matriz1",
				description: "es el primer conjunto de datos"
			},
			{
				name: "matriz2",
				description: "es el segundo conjunto de datos"
			},
			{
				name: "colas",
				description: "especifica el número de colas de distribución que se va a devolver: distribución de una cola = 1; distribución de dos colas = 2"
			},
			{
				name: "tipo",
				description: "es el tipo de prueba t: pareada = 1, varianza igual de dos muestras (homoscedástica) = 2, varianza desigual de dos muestras = 3"
			}
		]
	},
	{
		name: "PRUEBA.T.N",
		description: "Devuelve la probabilidad asociada con la prueba t de Student.",
		arguments: [
			{
				name: "matriz1",
				description: "es el primer conjunto de datos"
			},
			{
				name: "matriz2",
				description: "es el segundo conjunto de datos"
			},
			{
				name: "colas",
				description: "especifica el número de colas de distribución para devolver: una cola de distribución = 1; dos colas de distribución = 2"
			},
			{
				name: "tipo",
				description: "es el tipo de prueba t: pareado = 1, dos muestras de igual varianza = 2, dos muestras de varianza distinta = 3"
			}
		]
	},
	{
		name: "PRUEBA.Z",
		description: "Devuelve el valor P de una cola de una prueba z.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de datos frente a los que se probará X"
			},
			{
				name: "x",
				description: "es el valor que se probará"
			},
			{
				name: "sigma",
				description: "es la desviación estándar (conocida) de la población. Si se omite, se usará la desviación estándar de muestra"
			}
		]
	},
	{
		name: "PRUEBA.Z.N",
		description: "Devuelve el valor P de una cola de una prueba z.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de datos con los que se ha de contrastar X"
			},
			{
				name: "x",
				description: "es el valor para comprobar"
			},
			{
				name: "sigma",
				description: "es la desviación estándar (conocida) de la población. Si se omite, se usará la desviación estándar de muestra"
			}
		]
	},
	{
		name: "RADIANES",
		description: "Convierte grados en radianes.",
		arguments: [
			{
				name: "ángulo",
				description: "es el ángulo en grados que se desea convertir"
			}
		]
	},
	{
		name: "RAIZ",
		description: "Devuelve la raíz cuadrada de un número.",
		arguments: [
			{
				name: "número",
				description: "es el número del que se desea obtener la raíz cuadrada"
			}
		]
	},
	{
		name: "RAIZ2PI",
		description: "Devuelve la raíz cuadrada de (número * Pi).",
		arguments: [
			{
				name: "número",
				description: "es el número por el que será multiplicado p"
			}
		]
	},
	{
		name: "RANGO",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "RANGO.PERCENTIL",
		description: "Devuelve el rango de un valor en un conjunto de datos como porcentaje del conjunto.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de datos con valores numéricos que define la posición relativa"
			},
			{
				name: "x",
				description: "es el valor cuyo rango desea conocer"
			},
			{
				name: "cifra_significativa",
				description: "es un valor opcional que identifica el número de dígitos significativos para el porcentaje devuelto. Si se omite, se usarán tres dígitos (0,xxx%)"
			}
		]
	},
	{
		name: "RANGO.PERCENTIL.EXC",
		description: "Devuelve la jerarquía de un valor en un conjunto de datos como un porcentaje del conjunto de datos como un porcentaje (0..1, exclusivo) del conjunto de datos.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de datos con valores numéricos que define la posición relativa"
			},
			{
				name: "x",
				description: "es el valor del que se desea conocer la jerarquía"
			},
			{
				name: "cifra_significativa",
				description: "es un valor opcional que identifica el número de decimales significativos para el porcentaje devuelto. Si se omite, se usarán tres decimales (0.xxx%)"
			}
		]
	},
	{
		name: "RANGO.PERCENTIL.INC",
		description: "Devuelve la jerarquía de un valor en un conjunto de datos como un porcentaje del conjunto de datos como un porcentaje (0..1, inclusive) del conjunto de datos.",
		arguments: [
			{
				name: "matriz",
				description: "es la matriz o rango de datos con valores numéricos que define la posición relativa"
			},
			{
				name: "x",
				description: "es el valor del que se desea conocer la jerarquía"
			},
			{
				name: "cifra_significativa",
				description: "es un valor opcional que identifica el número de decimales significativos para el porcentaje devuelto. Si se omite, se usarán tres decimales (0.xxx%)"
			}
		]
	},
	{
		name: "RDTR",
		description: "Recupera datos en tiempo real de un programa compatible con automatizaciones COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "es el nombre del ProgID de un complemento de automatización COM registrado. Encierre el texto entre comillas"
			},
			{
				name: "servidor",
				description: "es el nombre del servidor donde se debe ejecutar el complemento. Encierre el texto entre comillas. Si el complemento se ejecuta localmente, use una cadena vacía"
			},
			{
				name: "tema1",
				description: "son de 1 a 38 parámetros que especifican una parte de los datos"
			},
			{
				name: "tema2",
				description: "son de 1 a 38 parámetros que especifican una parte de los datos"
			}
		]
	},
	{
		name: "REDOND.MULT",
		description: "Devuelve un número redondeado al múltiplo deseado.",
		arguments: [
			{
				name: "número",
				description: "es el valor para redondear"
			},
			{
				name: "múltiplo",
				description: "es el múltiplo al cual desea redondear el argumento"
			}
		]
	},
	{
		name: "REDONDEA.IMPAR",
		description: "Redondea un número positivo hacia arriba y un número negativo hacia abajo hasta el próximo entero impar.",
		arguments: [
			{
				name: "número",
				description: "es el valor que se redondea"
			}
		]
	},
	{
		name: "REDONDEA.PAR",
		description: "Redondea un número positivo hacia arriba y un número negativo hacia abajo hasta el próximo entero par. Los números negativos se ajustan alejándolos de cero.",
		arguments: [
			{
				name: "número",
				description: "es el valor que se redondea"
			}
		]
	},
	{
		name: "REDONDEAR",
		description: "Redondea un número al número de decimales especificado.",
		arguments: [
			{
				name: "número",
				description: "es el número que se desea redondear"
			},
			{
				name: "núm_decimales",
				description: "especifica el número de decimales al que se desea redondear. Los números negativos se redondean a la izquierda de la coma decimal; cero se redondea al entero más cercano"
			}
		]
	},
	{
		name: "REDONDEAR.MAS",
		description: "Redondea un número hacia arriba, en dirección contraria a cero.",
		arguments: [
			{
				name: "número",
				description: "es cualquier número real que desee redondear"
			},
			{
				name: "núm_decimales",
				description: "es el número de decimales a los cuales desea redondear. Para números negativos se redondea a la izquierda de la coma decimal; si se omite o el valor es cero, se redondea al entero más cercano"
			}
		]
	},
	{
		name: "REDONDEAR.MENOS",
		description: "Redondea un número hacia abajo, hacia cero.",
		arguments: [
			{
				name: "número",
				description: "es cualquier número real que desee redondear hacia abajo"
			},
			{
				name: "núm_decimales",
				description: "es el número de decimales a los cuales desea redondear. Para números negativos se redondea a la izquierda de la coma decimal; si se omite o el valor es cero, se redondea al entero más cercano"
			}
		]
	},
	{
		name: "REEMPLAZAR",
		description: "Reemplaza parte de una cadena de texto por otra.",
		arguments: [
			{
				name: "texto_original",
				description: "es el texto en el que se desea reemplazar ciertos caracteres"
			},
			{
				name: "núm_inicial",
				description: "es la posición del carácter dentro de texto_original que se desea reemplazar con texto_nuevo"
			},
			{
				name: "núm_de_caracteres",
				description: "es el número de caracteres en texto_original que se desea reemplazar"
			},
			{
				name: "texto_nuevo",
				description: "es el texto que reemplaza caracteres en texto_original"
			}
		]
	},
	{
		name: "RENDTO.DESC",
		description: "Devuelve el rendimiento anual para el valor bursátil con descuento. Por ejemplo, una letra de tesorería.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de liquidación del valor bursátil, expresada como número de fecha de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de vencimiento del valor bursátil, expresada como número de fecha de serie"
			},
			{
				name: "pr",
				description: "es el precio del valor bursátil por un valor nominal de 100 $"
			},
			{
				name: "amortización",
				description: "es el rendimiento del valor bursátil por cada 100 $ de valor nominal"
			},
			{
				name: "base",
				description: "determina en qué tipo de base deben ser contados los días"
			}
		]
	},
	{
		name: "REPETIR",
		description: "Repite el texto un número determinado de veces. Use REPETIR para rellenar una celda con el número de ocurrencias del texto en la cadena.",
		arguments: [
			{
				name: "texto",
				description: "es el texto que se desea repetir"
			},
			{
				name: "núm_de_veces",
				description: "es un número positivo que especifica el número de veces que el argumento texto se repetirá"
			}
		]
	},
	{
		name: "RESIDUO",
		description: "Proporciona el residuo después de dividir un número por un divisor.",
		arguments: [
			{
				name: "número",
				description: "es el número para el que se desea encontrar el residuo después de realizar la división"
			},
			{
				name: "núm_divisor",
				description: "es el número por el cual se desea dividir Número"
			}
		]
	},
	{
		name: "RRI",
		description: "Devuelve una tasa de interés equivalente para el crecimiento de una inversión.",
		arguments: [
			{
				name: "nper",
				description: "Es la cantidad de períodos de la inversión"
			},
			{
				name: "va",
				description: "Es el valor actual de la inversión"
			},
			{
				name: "vf",
				description: "Es el valor futuro de la inversión"
			}
		]
	},
	{
		name: "SEC",
		description: "Devuelve la secante de un ángulo.",
		arguments: [
			{
				name: "número",
				description: "Es el ángulo en radianes del que quieres saber la secante"
			}
		]
	},
	{
		name: "SECH",
		description: "Devuelve la secante hiperbólica de un ángulo.",
		arguments: [
			{
				name: "número",
				description: "Es el ángulo en radianes del que quieres saber la secante hiperbólica"
			}
		]
	},
	{
		name: "SEGUNDO",
		description: "Devuelve el segundo, un número de 0 a 59.",
		arguments: [
			{
				name: "núm_de_serie",
				description: "es un número en el código de fecha y hora usado por Spreadsheet, o texto en formato de hora, como por ejemplo 16:48:23 o 4:48:47 p.m."
			}
		]
	},
	{
		name: "SENO",
		description: "Devuelve el seno de un ángulo determinado.",
		arguments: [
			{
				name: "número",
				description: "es el ángulo en radianes del que se desea obtener el seno. Grados * PI()/180 = radianes"
			}
		]
	},
	{
		name: "SENOH",
		description: "Devuelve el seno hiperbólico de un número.",
		arguments: [
			{
				name: "número",
				description: "es un número real"
			}
		]
	},
	{
		name: "SI",
		description: "Comprueba si se cumple una condición y devuelve una valor si se evalúa como VERDADERO y otro valor si se evalúa como FALSO.",
		arguments: [
			{
				name: "prueba_lógica",
				description: "es cualquier valor o expresión que pueda evaluarse como VERDADERO o FALSO"
			},
			{
				name: "valor_si_verdadero",
				description: "es el valor que se devolverá si prueba_lógica es VERDADERO. Si se omite, devolverá VERDADERO. Puede anidar hasta siete funciones SI"
			},
			{
				name: "valor_si_falso",
				description: "es el valor que se devolverá si prueba_lógica es FALSO. Si se omite, devolverá FALSO"
			}
		]
	},
	{
		name: "SI.ERROR",
		description: "Devuelve valor_si_error si la expresión es un error y el valor de la expresión no lo es.",
		arguments: [
			{
				name: "valor",
				description: "es cualquier valor, expresión o referencia"
			},
			{
				name: "valor_si_error",
				description: "es cualquier valor, expresión o referencia"
			}
		]
	},
	{
		name: "SI.ND",
		description: "Devuelve el valor que especificas, si la expresión se convierte en #N/A. De lo contrario, devuelve el resultado de la expresión.",
		arguments: [
			{
				name: "valor",
				description: "Es cualquier valor, expresión o referencia"
			},
			{
				name: "valor_si_nd",
				description: "Es cualquier valor, expresión o referencia"
			}
		]
	},
	{
		name: "SIFECHA",
		description: "",
		arguments: [
		]
	},
	{
		name: "SIGNO",
		description: "Devuelve el signo de un número: 1, si el número es positivo; cero, si el número es cero y -1, si el número es negativo.",
		arguments: [
			{
				name: "número",
				description: "es un número real"
			}
		]
	},
	{
		name: "SLN",
		description: "Devuelve la depreciación por método directo de un activo en un período dado.",
		arguments: [
			{
				name: "costo",
				description: "es el costo inicial del bien"
			},
			{
				name: "valor_residual",
				description: "es el valor remanente al final de la vida de un activo"
			},
			{
				name: "vida",
				description: "es el número de períodos durante los que se produce la depreciación del activo (algunas veces se conoce como vida útil del activo)"
			}
		]
	},
	{
		name: "SUBTOTALES",
		description: "Devuelve un subtotal dentro de una lista o una base de datos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm_función",
				description: "es un número del 1 al 11 que indica qué función debe ser usada en el cálculo de los subtotales dentro de una lista."
			},
			{
				name: "ref1",
				description: "son de 1 a 254 rangos o referencias, de los cuales desea calcular el subtotal"
			}
		]
	},
	{
		name: "SUMA",
		description: "Suma todos los números en un rango de celdas.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 números que se desea sumar. Los valores lógicos y el texto se omiten en las celdas, incluso si están escritos como argumentos"
			},
			{
				name: "número2",
				description: "son de 1 a 255 números que se desea sumar. Los valores lógicos y el texto se omiten en las celdas, incluso si están escritos como argumentos"
			}
		]
	},
	{
		name: "SUMA.CUADRADOS",
		description: "Devuelve la suma de los cuadrados de los argumentos. Los argumentos pueden ser números, matrices, nombres o referencias a celdas que contengan números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 225 números, matrices, nombres o referencias a matrices cuya suma de cuadrados desea calcular"
			},
			{
				name: "número2",
				description: "son de 1 a 225 números, matrices, nombres o referencias a matrices cuya suma de cuadrados desea calcular"
			}
		]
	},
	{
		name: "SUMA.SERIES",
		description: "Devuelve la suma de una serie de potencias basándose en la fórmula.",
		arguments: [
			{
				name: "x",
				description: "es el valor de entrada para la serie de potencias"
			},
			{
				name: "n",
				description: "es la potencia inicial a la que desea elevar x"
			},
			{
				name: "m",
				description: "es el paso por el que se incrementa n para cada término de la serie"
			},
			{
				name: "coeficientes",
				description: "es un conjunto de coeficientes por el que cada potencia sucesiva de x se multiplica"
			}
		]
	},
	{
		name: "SUMAPRODUCTO",
		description: "Devuelve la suma de los productos de rangos o matrices correspondientes.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matriz1",
				description: "son de 2 a 255 matrices cuyos componentes se desea multiplicar y después sumar. Todas las matrices deben tener las mismas dimensiones"
			},
			{
				name: "matriz2",
				description: "son de 2 a 255 matrices cuyos componentes se desea multiplicar y después sumar. Todas las matrices deben tener las mismas dimensiones"
			},
			{
				name: "matriz3",
				description: "son de 2 a 255 matrices cuyos componentes se desea multiplicar y después sumar. Todas las matrices deben tener las mismas dimensiones"
			}
		]
	},
	{
		name: "SUMAR.SI",
		description: "Suma las celdas que cumplen determinado criterio o condición.",
		arguments: [
			{
				name: "rango",
				description: "es el rango de celdas que desea evaluar"
			},
			{
				name: "criterio",
				description: "es el criterio o condición que determina qué celdas deben sumarse. Puede estar en forma de número, texto o expresión"
			},
			{
				name: "rango_suma",
				description: "son las celdas que se van a sumar. Si se omite, se usarán las celdas en el rango"
			}
		]
	},
	{
		name: "SUMAR.SI.CONJUNTO",
		description: "Suma las celdas que cumplen un determinado conjunto de condiciones o criterios.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rango_suma",
				description: "son las celdas que se van a sumar"
			},
			{
				name: "rango_criterios",
				description: "es el rango de celdas que desea evaluar para la condición determinada"
			},
			{
				name: "criterio",
				description: "es el criterio o condición que determina qué celdas deben sumarse. Puede estar en forma de número, texto o expresión"
			}
		]
	},
	{
		name: "SUMAX2MASY2",
		description: "Devuelve la suma de total de las sumas de cuadrados de números en dos rangos o matrices correspondientes.",
		arguments: [
			{
				name: "matriz_x",
				description: "es la primera matriz o rango de valores y puede ser un número, nombre, matriz o referencia que contenga números"
			},
			{
				name: "matriz_y",
				description: "es la segunda matriz o rango de valores y puede ser un número, nombre, matriz o referencia que contenga números"
			}
		]
	},
	{
		name: "SUMAX2MENOSY2",
		description: "Suma las diferencias entre cuadrados de dos rangos o matrices correspondientes.",
		arguments: [
			{
				name: "matriz_x",
				description: "es la primera matriz o rango de valores y puede ser un número, nombre, matriz o referencia que contenga números"
			},
			{
				name: "matriz_y",
				description: "es la segunda matriz o rango de valores y puede ser un número, nombre, matriz o referencia que contenga números"
			}
		]
	},
	{
		name: "SUMAXMENOSY2",
		description: "Suma los cuadrados de las diferencias en dos rangos correspondientes de matrices.",
		arguments: [
			{
				name: "matriz_x",
				description: "es la primera matriz o rango de valores y puede ser un número, nombre, matriz o referencia que contiene números"
			},
			{
				name: "matriz_y",
				description: "es el segundo rango o matriz de valores y puede ser un número, nombre, matriz o referencia que contenga números"
			}
		]
	},
	{
		name: "SUSTITUIR",
		description: "Reemplaza el texto existente con texto nuevo en una cadena.",
		arguments: [
			{
				name: "texto",
				description: "es el texto o la referencia a una celda que contiene texto en el que se desea cambiar caracteres"
			},
			{
				name: "texto_original",
				description: "es el texto que se desea sustituir. Si las mayúsculas y minúsculas del texto_original y el texto a sustituir no coinciden, SUSTITUIR no reemplazará el texto"
			},
			{
				name: "texto_nuevo",
				description: "es el texto con el que se desea reemplazar texto_original"
			},
			{
				name: "núm_de_ocurrencia",
				description: "especifica la aparición de texto_original que se desea reemplazar. Si se omite, se reemplazará texto_original en todos los sitios donde aparezca"
			}
		]
	},
	{
		name: "SYD",
		description: "Devuelve la depreciación por método de anualidades de un activo durante un período específico.",
		arguments: [
			{
				name: "costo",
				description: "es el costo inicial del bien"
			},
			{
				name: "valor_residual",
				description: "es el valor remanente al final de la vida de un activo"
			},
			{
				name: "vida",
				description: "es el número de períodos durante los que se produce la depreciación del activo (algunas veces se conoce como vida útil del activo)"
			},
			{
				name: "período",
				description: "es el período y se deben utilizar las mismas unidades que Vida"
			}
		]
	},
	{
		name: "T",
		description: "Comprueba si un valor es texto y devuelve texto si lo es o comillas dobles (sin texto) si no lo es.",
		arguments: [
			{
				name: "valor",
				description: "es el valor que desea comprobar"
			}
		]
	},
	{
		name: "TAN",
		description: "Devuelve la tangente de un ángulo.",
		arguments: [
			{
				name: "número",
				description: "es el ángulo en radianes del que se desea obtener la tangente. Grados * PI()/180 = radianes"
			}
		]
	},
	{
		name: "TANH",
		description: "Devuelve la tangente hiperbólica de un número.",
		arguments: [
			{
				name: "número",
				description: "es cualquier número real"
			}
		]
	},
	{
		name: "TASA",
		description: "Devuelve la tasa de interés por período de un préstamo o una inversión. Por ejemplo, use 6%/4 para pagos trimestrales al 6% TPA.",
		arguments: [
			{
				name: "nper",
				description: "es el número total de períodos de pago de un préstamo o una inversión"
			},
			{
				name: "pago",
				description: "es el pago efectuado en cada período y no puede cambiar durante la vigencia del préstamo o la inversión"
			},
			{
				name: "va",
				description: "es el valor actual: la cantidad total de una serie de pagos futuros"
			},
			{
				name: "vf",
				description: "es el valor futuro o saldo en efectivo que se desea lograr después de efectuar el último pago. Si se omite, se usa VA = 0"
			},
			{
				name: "tipo",
				description: "es un valor lógico: para pago al comienzo del período = 1; para pago al final del período = 0 u omitido"
			},
			{
				name: "estimar",
				description: "es su estimación de la tasa de interés; si se omite Estimar = 0,1 (10%)"
			}
		]
	},
	{
		name: "TASA.DESC",
		description: "Devuelve la tasa de descuento del valor bursátil.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de liquidación del valor bursátil, expresada como número de fecha de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de vencimiento del valor bursátil, expresada como número de fecha de serie"
			},
			{
				name: "pr",
				description: "es el precio del valor bursátil por un valor nominal de 100 $"
			},
			{
				name: "amortización",
				description: "es el rendimiento del valor bursátil por cada 100 $ de valor nominal"
			},
			{
				name: "base",
				description: "determina en qué tipo de base deben ser contados los días"
			}
		]
	},
	{
		name: "TASA.INT",
		description: "Devuelve la tasa de interés para la inversión total en un valor bursátil.",
		arguments: [
			{
				name: "liquidación",
				description: "es la fecha de liquidación del valor bursátil, expresado como un número de fecha de serie"
			},
			{
				name: "vencimiento",
				description: "es la fecha de vencimiento del valor bursátil, expresado como un número de fecha de serie"
			},
			{
				name: "inversión",
				description: "es la cantidad invertida en el valor bursátil"
			},
			{
				name: "amortización",
				description: "es la cantidad recibida al vencimiento"
			},
			{
				name: "base",
				description: "determina en qué tipo de base deben contarse los días"
			}
		]
	},
	{
		name: "TASA.NOMINAL",
		description: "Devuelve la tasa de interés nominal anual.",
		arguments: [
			{
				name: "tasa_efect",
				description: "es la tasa de interés efectiva"
			},
			{
				name: "núm_per_año",
				description: "es el número de períodos compuestos por año"
			}
		]
	},
	{
		name: "TENDENCIA",
		description: "Devuelve números en una tendencia lineal que coincide con puntos de datos conocidos, usando el método de los mínimos cuadrados.",
		arguments: [
			{
				name: "conocido_y",
				description: "es el conjunto de valores de Y conocidos en la relación y = mx + b"
			},
			{
				name: "conocido_x",
				description: "es un conjunto de valores de X opcionales (puede que conocidos) de la relación y = mx + b, una matriz del mismo tamaño que conocido_y"
			},
			{
				name: "nueva_matriz_x",
				description: "son nuevos valores de X para los cuales se desea que TENDENCIA devuelva los valores de Y correspondientes"
			},
			{
				name: "constante",
				description: "es un valor lógico: la constante b se calcula normalmente si Const = VERDADERO u omitido; se establece b igual a 0 si Const = FALSO"
			}
		]
	},
	{
		name: "TEXTO",
		description: "Convierte un valor en texto, con un formato de número específico.",
		arguments: [
			{
				name: "valor",
				description: "es un valor numérico, una fórmula que evalúa un valor numérico o una referencia a una celda que contiene un valor numérico"
			},
			{
				name: "formato",
				description: "es un número en forma de texto del cuadro Categoría, pestaña Número del cuadro de diálogo Formato de celdas, distinto de la categoría General"
			}
		]
	},
	{
		name: "TEXTOBAHT",
		description: "Convierte un número en texto (baht).",
		arguments: [
			{
				name: "número",
				description: "es un número que se desea convertir"
			}
		]
	},
	{
		name: "TIPO",
		description: "Devuelve un entero que representa el tipo de datos de un valor: número = 1; texto = 2; valor lógico = 4; valor de error = 16; matriz = 64.",
		arguments: [
			{
				name: "valor",
				description: "puede ser cualquier valor"
			}
		]
	},
	{
		name: "TIPO.DE.ERROR",
		description: "Devuelve un número que coincide con un valor de error.",
		arguments: [
			{
				name: "valor_de_error",
				description: "es el valor de error cuyo número identificador desea buscar y puede ser un valor de error actual o una referencia a una celda que contiene un valor de error"
			}
		]
	},
	{
		name: "TIR",
		description: "Devuelve la tasa interna de retorno de una inversión para una serie de valores en efectivo.",
		arguments: [
			{
				name: "valores",
				description: "es una matriz o referencia a celdas que contengan los números para los cuales se desea calcular la tasa interna de retorno"
			},
			{
				name: "estimar",
				description: "es un número que el usuario estima que se aproximará al resultado de TIR; se asume 0,1 (10%) si se omite"
			}
		]
	},
	{
		name: "TIR.NO.PER",
		description: "Devuelve la tasa interna de retorno para un flujo de caja que no es necesariamente periódico.",
		arguments: [
			{
				name: "valores",
				description: "es un flujo de caja, no necesariamente periódico, que corresponde al plan de fechas de pagos"
			},
			{
				name: "fechas",
				description: "son las flechas del plan de pagos que corresponde al flujo de caja, no necesariamente periódico"
			},
			{
				name: "estimar",
				description: "es un número que estima que es aproximado al resultado de TIR.NO.PER"
			}
		]
	},
	{
		name: "TIRM",
		description: "Devuelve la tasa interna de retorno para una serie de flujos de efectivo periódicos, considerando costo de la inversión e interés al volver a invertir el efectivo.",
		arguments: [
			{
				name: "valores",
				description: "es una matriz o referencia a celdas que contienen números que representan una serie de pagos (negativos) y entradas (positivas) realizados en períodos constantes"
			},
			{
				name: "tasa_financiamiento",
				description: "es la tasa de interés que se paga del dinero utilizado en flujos de efectivo"
			},
			{
				name: "tasa_reinversión",
				description: "es la tasa de interés que se recibe de los flujos de efectivo a medida que se vuelven a invertir"
			}
		]
	},
	{
		name: "TRANSPONER",
		description: "Devuelve un rango vertical de celdas como un rango horizontal, o viceversa.",
		arguments: [
			{
				name: "matriz",
				description: "es un rango de celdas en una hoja de cálculo o una matriz de valores que se desea transponer"
			}
		]
	},
	{
		name: "TRUNCAR",
		description: "Convierte un número decimal a uno entero al quitar la parte decimal o de fracción.",
		arguments: [
			{
				name: "número",
				description: "es el número que se desea truncar"
			},
			{
				name: "núm_decimales",
				description: "es un número que especifica la precisión de truncado; si se omite se asume 0 (cero)"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Devuelve el número (punto de código) que corresponde al primer carácter del texto.",
		arguments: [
			{
				name: "texto",
				description: "Es el carácter del que quieres saber el valor Unicode"
			}
		]
	},
	{
		name: "URLCODIF",
		description: "Devuelve una cadena codificada en URL.",
		arguments: [
			{
				name: "texto",
				description: " es una cadena que hay que codificar con URL"
			}
		]
	},
	{
		name: "VA",
		description: "Devuelve el valor presente de una inversión: la suma total del valor actual de una serie de pagos futuros.",
		arguments: [
			{
				name: "tasa",
				description: "es la tasa de interés por período. Por ejemplo, use 6%/4 para pagos trimestrales al 6% TPA"
			},
			{
				name: "nper",
				description: "es el número total de períodos de pago en una inversión"
			},
			{
				name: "pago",
				description: "es el pago efectuado en cada período y no puede cambiar durante la vigencia de la inversión"
			},
			{
				name: "vf",
				description: "es el valor futuro o saldo en efectivo que se desea lograr después de efectuar el último pago"
			},
			{
				name: "tipo",
				description: "es un valor lógico: para pago al comienzo del período = 1; para pago al final del período = 0 u omitido"
			}
		]
	},
	{
		name: "VALOR",
		description: "Convierte un argumento de texto que representa un número en un número.",
		arguments: [
			{
				name: "texto",
				description: "es el texto entre comillas o una referencia a una celda que contiene el texto que se desea convertir"
			}
		]
	},
	{
		name: "VALOR.NUMERO",
		description: "Convierte texto a número de manera independiente a la configuración regional.",
		arguments: [
			{
				name: "texto",
				description: "Es la cadena que representa el número que quieres convertir"
			},
			{
				name: "separador_decimal",
				description: "Es el carácter que se usa como separador decimal en la cadena"
			},
			{
				name: "separador_grupo",
				description: "Es el carácter que se usa como el separador de grupos en la cadena"
			}
		]
	},
	{
		name: "VAR",
		description: "Calcula la varianza de una muestra (se omiten los valores lógicos y el texto de la muestra).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 argumentos numéricos que corresponden a una muestra de una población"
			},
			{
				name: "número2",
				description: "son de 1 a 255 argumentos numéricos que corresponden a una muestra de una población"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Calcula la varianza en función de la población total (omite los valores lógicos y el texto).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 argumentos numéricos que se corresponden con una población"
			},
			{
				name: "número2",
				description: "son de 1 a 255 argumentos numéricos que se corresponden con una población"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Calcula la varianza en función de una muestra (omite los valores lógicos y el texto).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 argumentos numéricos que se corresponden con una muestra de una población"
			},
			{
				name: "número2",
				description: "son de 1 a 255 argumentos numéricos que se corresponden con una muestra de una población"
			}
		]
	},
	{
		name: "VARA",
		description: "Calcula la varianza de una muestra, incluyendo valores lógicos y texto. Los valores lógicos y el texto con valor FALSO tiene valor asignado 0, los de valor lógico VERDADERO tienen valor 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "son de 1 a 255 argumentos de valores correspondientes a una muestra de una población"
			},
			{
				name: "valor2",
				description: "son de 1 a 255 argumentos de valores correspondientes a una muestra de una población"
			}
		]
	},
	{
		name: "VARP",
		description: "Calcula la varianza de la población total (se omiten los valores lógicos y el texto de la población).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: "son de 1 a 255 argumentos numéricos que corresponden a una población"
			},
			{
				name: "número2",
				description: "son de 1 a 255 argumentos numéricos que corresponden a una población"
			}
		]
	},
	{
		name: "VARPA",
		description: "Calcula la varianza de la población total, incluyendo valores lógicos y texto. Los valores lógicos y el texto con valor FALSO tienen el valor asignado 0, los de valor lógico VERDADERO tienen valor 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "son de 1 a 255 argumentos de valores correspondientes a una población"
			},
			{
				name: "valor2",
				description: "son de 1 a 255 argumentos de valores correspondientes a una población"
			}
		]
	},
	{
		name: "VERDADERO",
		description: "Devuelve el valor lógico VERDADERO.",
		arguments: [
		]
	},
	{
		name: "VF",
		description: "Devuelve el valor futuro de una inversión basado en pagos periódicos y constantes, y una tasa de interés también constante.",
		arguments: [
			{
				name: "tasa",
				description: "es la tasa de interés por período. Por ejemplo, use 6%/4 para pagos trimestrales al 6% de TPA"
			},
			{
				name: "nper",
				description: "es el número total de pagos de una inversión"
			},
			{
				name: "pago",
				description: "es el pago efectuado cada período; no puede cambiar durante la vigencia de la inversión"
			},
			{
				name: "va",
				description: "es el valor actual o la suma total del valor de una serie de pagos futuros. Si se omite, VA = 0"
			},
			{
				name: "tipo",
				description: "es el número 0 o 1 e indica cuándo vencen los pagos: pago al comienzo del período =1; pago al final del período = 0 u omitido"
			}
		]
	},
	{
		name: "VF.PLAN",
		description: "Devuelve el valor futuro de uno principal inicial después de aplicar una serie de tasas de interés compuesto.",
		arguments: [
			{
				name: "principal",
				description: "es el valor presente"
			},
			{
				name: "programación",
				description: "es una matriz de tasas de interés para aplicar"
			}
		]
	},
	{
		name: "VNA",
		description: "Devuelve el valor neto presente de una inversión a partir de una tasa de descuento y una serie de pagos futuros (valores negativos) y entradas (valores positivos).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tasa",
				description: "es la tasa de descuento durante un período"
			},
			{
				name: "valor1",
				description: "son de 1 a 254 pagos e ingresos, igualmente espaciados y que tienen lugar al final de cada período"
			},
			{
				name: "valor2",
				description: "son de 1 a 254 pagos e ingresos, igualmente espaciados y que tienen lugar al final de cada período"
			}
		]
	},
	{
		name: "VNA.NO.PER",
		description: "Devuelve el valor neto actual para un flujo de caja que no es necesariamente periódico.",
		arguments: [
			{
				name: "tasa",
				description: "es la tasa de descuento para aplicar el efectivo"
			},
			{
				name: "valores",
				description: "es un flujo de caja, no necesariamente periódico, que corresponde al plan de fechas de pagos"
			},
			{
				name: "fechas",
				description: "son las fechas del plan de pago que corresponde al flujo de caja, no necesariamente periódico"
			}
		]
	},
	{
		name: "XO",
		description: "Devuelve una 'Exclusive Or' lógica de todos los argumentos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "lógico1",
				description: "Son las condiciones de 1 a 254 que quieres probar, que pueden ser VERDADERAS o FALSAS, así como valores lógicos, matrices o referencias"
			},
			{
				name: "lógico2",
				description: "Son las condiciones de 1 a 254 que quieres probar, que pueden ser VERDADERAS o FALSAS, así como valores lógicos, matrices o referencias"
			}
		]
	},
	{
		name: "Y",
		description: "Comprueba si todos los argumentos son VERDADEROS, y devuelve VERDADERO si todos los argumentos son VERDADEROS.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor_lógico1",
				description: "son entre 1 y 255 condiciones que desea comprobar, que pueden ser VERDADERO o FALSO y que pueden ser valores lógicos, matrices o referencias"
			},
			{
				name: "valor_lógico2",
				description: "son entre 1 y 255 condiciones que desea comprobar, que pueden ser VERDADERO o FALSO y que pueden ser valores lógicos, matrices o referencias"
			}
		]
	}
];