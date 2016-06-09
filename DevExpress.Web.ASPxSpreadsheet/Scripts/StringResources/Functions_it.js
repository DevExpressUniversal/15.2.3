ASPxClientSpreadsheet.Functions = [
	{
		name: "ADESSO",
		description: "Restituisce la data e l'ora correnti nel formato data-ora.",
		arguments: [
		]
	},
	{
		name: "AMBIENTE.INFO",
		description: "Restituisce informazioni sull'ambiente operativo corrente.",
		arguments: [
			{
				name: "tipo",
				description: "è il testo che specifica il tipo di informazioni desiderato."
			}
		]
	},
	{
		name: "AMMORT",
		description: "Restituisce l'ammortamento di un bene per un periodo specificato utilizzando il metodo a doppie quote proporzionali ai valori residui o un altro metodo specificato.",
		arguments: [
			{
				name: "costo",
				description: "è il costo iniziale del bene"
			},
			{
				name: "val_residuo",
				description: "è il valore di recupero al termine della vita utile del bene"
			},
			{
				name: "vita_utile",
				description: "è il numero di periodi in cui il bene viene ammortizzato, denominato anche vita utile di un bene"
			},
			{
				name: "periodo",
				description: "è il periodo per il quale si calcola l'ammortamento. Periodo deve utilizzare la stessa unità di misura di Vita_utile."
			},
			{
				name: "fattore",
				description: "è il tasso di deprezzamento del valore residuo. Se Fattore non è specificato, verrà considerato uguale a 2 (metodo di ammortamento a doppie quote proporzionali ai valori residui)."
			}
		]
	},
	{
		name: "AMMORT.ANNUO",
		description: "Restituisce l'ammortamento americano di un bene per un determinato periodo.",
		arguments: [
			{
				name: "costo",
				description: "è il costo iniziale del bene"
			},
			{
				name: "val_residuo",
				description: "è il valore di recupero al termine della vita utile del bene"
			},
			{
				name: "vita_utile",
				description: "è il numero di periodi in cui il bene viene ammortizzato, denominato anche vita utile di un bene"
			},
			{
				name: "periodo",
				description: "definisce il periodo per il quale devono essere utilizzate le stesse unità di misura di Vita_utile"
			}
		]
	},
	{
		name: "AMMORT.COST",
		description: "Restituisce l'ammortamento costante di un bene per un periodo.",
		arguments: [
			{
				name: "costo",
				description: "è il costo iniziale del bene"
			},
			{
				name: "val_residuo",
				description: "è il valore di recupero al termine della vita utile del bene"
			},
			{
				name: "vita_utile",
				description: "è il numero di periodi in cui il bene viene ammortizzato, denominato anche vita utile di un bene"
			}
		]
	},
	{
		name: "AMMORT.FISSO",
		description: "Restituisce l'ammortamento di un bene per un periodo specificato utilizzando il metodo a quote fisse proporzionali ai valori residui.",
		arguments: [
			{
				name: "costo",
				description: "è il costo iniziale del bene"
			},
			{
				name: "val_residuo",
				description: "è il valore di recupero al termine della vita utile del bene"
			},
			{
				name: "vita_utile",
				description: "è il numero di periodi in cui il bene viene ammortizzato, denominato anche vita utile di un bene"
			},
			{
				name: "periodo",
				description: "è il periodo per il quale si calcola l'ammortamento. Periodo deve utilizzare la stessa unità di misura di Vita_utile."
			},
			{
				name: "mese",
				description: "è il numero di mesi nel primo anno. Se non è specificato, verrà considerato uguale a 12."
			}
		]
	},
	{
		name: "AMMORT.VAR",
		description: "Restituisce l'ammortamento di un bene per un periodo specificato, anche parziale, utilizzando il metodo a quote proporzionali ai valori residui o un altro metodo specificato.",
		arguments: [
			{
				name: "costo",
				description: "è il costo iniziale del bene"
			},
			{
				name: "val_residuo",
				description: "è il valore di recupero al termine della vita utile del bene"
			},
			{
				name: "vita_utile",
				description: "è il numero di periodi in cui il bene viene ammortizzato, denominato anche vita utile di un bene"
			},
			{
				name: "inizio",
				description: "è il periodo iniziale per il quale si calcola l'ammortamento, espresso nella stessa unità di Vita"
			},
			{
				name: "fine",
				description: "è il periodo finale per il quale si calcola l'ammortamento, espresso nella stessa unità di Vita"
			},
			{
				name: "fattore",
				description: "è il tasso di diminuzione della quota, 2 (a doppie quote proporzionali ai valori residui) se omesso"
			},
			{
				name: "nessuna_opzione",
				description: "passa all'ammortamento a quote costanti quando l'ammortamento è maggiore della quota decrescente = FALSO oppure omesso; non passare = VERO"
			}
		]
	},
	{
		name: "ANNO",
		description: "Restituisce l'anno di una data, un intero nell'intervallo compreso tra 1900 e 9999.",
		arguments: [
			{
				name: "num_seriale",
				description: "è un numero nel codice data-ora utilizzato da Spreadsheet"
			}
		]
	},
	{
		name: "ANNULLA.SPAZI",
		description: "Rimuove gli spazi da una stringa di testo eccetto gli spazi singoli tra le parole.",
		arguments: [
			{
				name: "testo",
				description: "è il testo da cui si desidera rimuovere gli spazi"
			}
		]
	},
	{
		name: "ARABO",
		description: "Converte un numero romano in arabo.",
		arguments: [
			{
				name: "num",
				description: "è il numero romano che si vuole convertire"
			}
		]
	},
	{
		name: "ARCCOS",
		description: "Restituisce l'arcocoseno di un numero, espresso in radianti da 0 a pi greco. L'arcocoseno è l'angolo il cui coseno è pari al numero.",
		arguments: [
			{
				name: "num",
				description: "è il coseno dell'angolo desiderato, un valore compreso tra -1 e 1"
			}
		]
	},
	{
		name: "ARCCOSH",
		description: "Restituisce l'inversa del coseno iperbolico di un numero.",
		arguments: [
			{
				name: "num",
				description: "è un numero reale maggiore o uguale ad 1"
			}
		]
	},
	{
		name: "ARCCOT",
		description: "Restituisce l'arcocotangente di un numero, espressa in radianti nell'intervallo tra 0 e pi greco.",
		arguments: [
			{
				name: "num",
				description: "è la cotangente dell'angolo"
			}
		]
	},
	{
		name: "ARCCOTH",
		description: "Restituisce la cotangente iperbolica inversa di un numero.",
		arguments: [
			{
				name: "num",
				description: "è la cotangente iperbolica dell'angolo"
			}
		]
	},
	{
		name: "ARCSEN",
		description: "Restituisce l'arcoseno di un numero, espresso in radianti nell'intervallo tra -pi greco/2 e pi greco/2.",
		arguments: [
			{
				name: "num",
				description: "è il seno dell'angolo desiderato, un valore compreso tra -1 e 1"
			}
		]
	},
	{
		name: "ARCSENH",
		description: "Restituisce l'inversa del seno iperbolico di un numero.",
		arguments: [
			{
				name: "num",
				description: "è un numero reale maggiore o uguale a 1"
			}
		]
	},
	{
		name: "ARCTAN",
		description: "Restituisce l'arcotangente di un numero, espressa in radianti nell'intervallo tra -pi greco/2 e pi greco/2.",
		arguments: [
			{
				name: "num",
				description: "è la tangente dell'angolo desiderato"
			}
		]
	},
	{
		name: "ARCTAN.2",
		description: "Restituisce l'arcotangente in radianti dalle coordinate x e y specificate, nell'intervallo tra -pi greco e pi greco, escluso -pi greco.",
		arguments: [
			{
				name: "x",
				description: "è l'ascissa del punto"
			},
			{
				name: "y",
				description: "è l'ordinata del punto"
			}
		]
	},
	{
		name: "ARCTANH",
		description: "Restituisce l'inversa della tangente iperbolica di un numero.",
		arguments: [
			{
				name: "num",
				description: "è un numero reale compreso tra -1 ed 1 esclusi"
			}
		]
	},
	{
		name: "AREE",
		description: "Restituisce il numero di aree in un riferimento. Un'area è un intervallo di celle contigue o una cella singola.",
		arguments: [
			{
				name: "rif",
				description: "è un riferimento ad una cella o ad un intervallo di celle e può riferirsi a più aree."
			}
		]
	},
	{
		name: "ARROTONDA",
		description: "Arrotonda un numero ad un numero specificato di cifre.",
		arguments: [
			{
				name: "num",
				description: "è il numero da arrotondare"
			},
			{
				name: "num_cifre",
				description: "è il numero di cifre a cui si desidera arrotondare. Negativo arrotonda a sinistra della virgola decimale; zero all'intero più prossimo."
			}
		]
	},
	{
		name: "ARROTONDA.DIFETTO",
		description: "Arrotonda un numero per difetto al multiplo più vicino a peso.",
		arguments: [
			{
				name: "num",
				description: "è il valore numerico da arrotondare"
			},
			{
				name: "peso",
				description: "è il multiplo per il quale si desidera arrotondare. Numero e Peso devono essere entrambi positivi o negativi."
			}
		]
	},
	{
		name: "ARROTONDA.DIFETTO.MAT",
		description: "Arrotonda un numero per difetto all'intero più vicino o al multiplo più vicino a peso.",
		arguments: [
			{
				name: "num",
				description: "è il valore da arrotondare"
			},
			{
				name: "peso",
				description: "è il multiplo in base al quale eseguire l'arrotondamento"
			},
			{
				name: "modalità",
				description: "se specificato e diverso da zero, la funzione eseguirà un arrotondamento avvicinandosi allo zero"
			}
		]
	},
	{
		name: "ARROTONDA.DIFETTO.PRECISA",
		description: "Arrotonda un numero per difetto all'intero più vicino o al multiplo più vicino a peso.",
		arguments: [
			{
				name: "num",
				description: "è il valore numerico da arrotondare"
			},
			{
				name: "peso",
				description: "è il multiplo per il quale si desidera arrotondare. "
			}
		]
	},
	{
		name: "ARROTONDA.ECCESSO",
		description: "Arrotonda un numero per eccesso al multiplo più vicino a peso.",
		arguments: [
			{
				name: "num",
				description: "è il valore da arrotondare"
			},
			{
				name: "peso",
				description: "è il multiplo per il quale si desidera arrotondare"
			}
		]
	},
	{
		name: "ARROTONDA.ECCESSO.MAT",
		description: "Arrotonda un numero per eccesso all'intero più vicino o al multiplo più vicino a peso.",
		arguments: [
			{
				name: "num",
				description: "è il valore da arrotondare"
			},
			{
				name: "peso",
				description: "è il multiplo in base al quale eseguire l'arrotondamento"
			},
			{
				name: "modalità",
				description: "se specificato e diverso da zero, la funzione eseguirà un arrotondamento allontanandosi dallo zero"
			}
		]
	},
	{
		name: "ARROTONDA.ECCESSO.PRECISA",
		description: "Arrotonda un numero per eccesso all'intero più vicino o al multiplo più vicino a peso.",
		arguments: [
			{
				name: "num",
				description: "è il valore da arrotondare"
			},
			{
				name: "peso",
				description: "è il multiplo per il quale si desidera arrotondare"
			}
		]
	},
	{
		name: "ARROTONDA.MULTIPLO",
		description: "Restituisce un numero arrotondato al multiplo desiderato.",
		arguments: [
			{
				name: "num",
				description: "è il valore da arrotondare"
			},
			{
				name: "multiplo",
				description: "è il multiplo a cui arrotondare il numero"
			}
		]
	},
	{
		name: "ARROTONDA.PER.DIF",
		description: "Arrotonda il valore assoluto di un numero per difetto.",
		arguments: [
			{
				name: "num",
				description: "è un qualsiasi numero reale da arrotondare per difetto"
			},
			{
				name: "num_cifre",
				description: "è il numero di cifre a cui si desidera arrotondare. Negativo arrotonda a sinistra della virgola decimale, zero oppure omesso all'intero più prossimo."
			}
		]
	},
	{
		name: "ARROTONDA.PER.ECC",
		description: "Arrotonda il valore assoluto di un numero per eccesso.",
		arguments: [
			{
				name: "num",
				description: "è un qualsiasi numero reale da arrotondare"
			},
			{
				name: "num_cifre",
				description: "è il numero di cifre a cui si desidera arrotondare. Negativo arrotonda a sinistra della virgola decimale, zero oppure omesso all'intero più prossimo."
			}
		]
	},
	{
		name: "ASIMMETRIA",
		description: "Restituisce il grado di asimmetria di una distribuzione, ovvero una caratterizzazione del grado di asimmetria di una distribuzione attorno alla media.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare l'asimmetria"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare l'asimmetria"
			}
		]
	},
	{
		name: "ASIMMETRIA.P",
		description: "Restituisce il grado di asimmetria di una distribuzione in base a una popolazione, ovvero una caratterizzazione del grado di asimmetria di una distribuzione attorno alla media.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare l'asimmetria della popolazione"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare l'asimmetria della popolazione"
			}
		]
	},
	{
		name: "ASS",
		description: "Restituisce il valore assoluto di un numero, il numero privo di segno.",
		arguments: [
			{
				name: "num",
				description: "è il numero reale di cui si calcola il valore assoluto"
			}
		]
	},
	{
		name: "BAHTTESTO",
		description: "Converte un numero in testo (baht).",
		arguments: [
			{
				name: "num",
				description: "è un numero che si desidera convertire"
			}
		]
	},
	{
		name: "BASE",
		description: "Converte un numero in una rappresentazione testuale con la radice (base) specificata.",
		arguments: [
			{
				name: "num",
				description: "è il numero che si vuole convertire"
			},
			{
				name: "radice",
				description: "è la radice di base per la conversione del numero"
			},
			{
				name: "lungh_min",
				description: "è la lunghezza minima della stringa restituita. Se omesso, non vengono aggiunti zeri iniziali"
			}
		]
	},
	{
		name: "BESSEL.I",
		description: "Restituisce la funzione di Bessel modificata In(x).",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui calcolare la funzione"
			},
			{
				name: "n",
				description: "è l'ordine della funzione di Bessel"
			}
		]
	},
	{
		name: "BESSEL.J",
		description: "Restituisce la funzione di Bessel Jn(x).",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui calcolare la funzione"
			},
			{
				name: "n",
				description: "è l'ordine della funzione di Bessel"
			}
		]
	},
	{
		name: "BESSEL.K",
		description: "Restituisce la funzione di Bessel modificata Kn(x).",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui calcolare la funzione"
			},
			{
				name: "n",
				description: "è l'ordine della funzione"
			}
		]
	},
	{
		name: "BESSEL.Y",
		description: "Restituisce la funzione di Bessel Yn(x).",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui calcolare la funzione"
			},
			{
				name: "n",
				description: "è l'ordine della funzione"
			}
		]
	},
	{
		name: "BINARIO.DECIMALE",
		description: "Converte un numero binario in decimale.",
		arguments: [
			{
				name: "num",
				description: "è il numero binario da convertire"
			}
		]
	},
	{
		name: "BINARIO.HEX",
		description: "Converte un numero binario in esadecimale.",
		arguments: [
			{
				name: "num",
				description: "è il numero binario da convertire"
			},
			{
				name: "cifre",
				description: "è il numero di caratteri da utilizzare"
			}
		]
	},
	{
		name: "BINARIO.OCT",
		description: "Converte un numero binario in ottale.",
		arguments: [
			{
				name: "num",
				description: "è il numero binario da convertire"
			},
			{
				name: "cifre",
				description: "è il numero di caratteri da utilizzare"
			}
		]
	},
	{
		name: "BIT.SPOSTA.DX",
		description: "Restituisce un numero spostato a destra dei bit indicati in bit_spostamento.",
		arguments: [
			{
				name: "num",
				description: "è la rappresentazione decimale del numero binario da calcolare"
			},
			{
				name: "bit_spostamento",
				description: "è il numero di bit di cui si vuole spostare num a destra"
			}
		]
	},
	{
		name: "BIT.SPOSTA.SX",
		description: "Restituisce un numero spostato a sinistra dei bit indicati in bit_spostamento.",
		arguments: [
			{
				name: "num",
				description: "è la rappresentazione decimale del numero binario da calcolare"
			},
			{
				name: "bit_spostamento",
				description: "è il numero di bit di cui si vuole spostare num a sinistra"
			}
		]
	},
	{
		name: "BITAND",
		description: "Restituisce un 'AND' bit per bit di due numeri.",
		arguments: [
			{
				name: "num1",
				description: "è la rappresentazione decimale del numero binario da calcolare"
			},
			{
				name: "num2",
				description: "è la rappresentazione decimale del numero binario da calcolare"
			}
		]
	},
	{
		name: "BITOR",
		description: "Restituisce un 'OR' bit per bit di due numeri.",
		arguments: [
			{
				name: "num1",
				description: "è la rappresentazione decimale del numero binario da calcolare"
			},
			{
				name: "num2",
				description: "è la rappresentazione decimale del numero binario da calcolare"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Restituisce un 'OR esclusivo' bit per bit di due numeri.",
		arguments: [
			{
				name: "num1",
				description: "è la rappresentazione decimale del numero binario da calcolare"
			},
			{
				name: "num2",
				description: "è la rappresentazione decimale del numero binario da calcolare"
			}
		]
	},
	{
		name: "BOT.EQUIV",
		description: "Calcola il rendimento equivalente a un'obbligazione per un buono del tesoro.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del buono del tesoro espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del buono del tesoro espressa come numero seriale"
			},
			{
				name: "sconto",
				description: "è il tasso di sconto del buono del tesoro"
			}
		]
	},
	{
		name: "BOT.PREZZO",
		description: "Calcola il prezzo di un buono del tesoro con valore nominale di 100 lire.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del buono del tesoro espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del buono del tesoro espressa come numero seriale"
			},
			{
				name: "sconto",
				description: "è il tasso di sconto del buono del tesoro"
			}
		]
	},
	{
		name: "BOT.REND",
		description: "Calcola il rendimento di un buono del tesoro.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del buono del tesoro espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del buono del tesoro espressa come numero seriale"
			},
			{
				name: "prezzo",
				description: "è il prezzo del buono del tesoro con valore nominale di 100 lire"
			}
		]
	},
	{
		name: "CAP.CUM",
		description: "Calcola il capitale cumulativo pagato per estinguere un debito tra due periodi.",
		arguments: [
			{
				name: "tasso_int",
				description: "è il tasso di interesse"
			},
			{
				name: "per",
				description: "è il numero totale dei periodi di pagamento"
			},
			{
				name: "val_attuale",
				description: "è il valore attuale"
			},
			{
				name: "iniz_per",
				description: "è il primo periodo nel calcolo"
			},
			{
				name: "fine_per",
				description: "è l'ultimo periodo nel calcolo"
			},
			{
				name: "tipo",
				description: "è la scadenza per il pagamento"
			}
		]
	},
	{
		name: "CASUALE",
		description: "Restituisce un numero casuale uniformemente distribuito, ossia cambia se viene ricalcolato, e maggiore o uguale a 0 e minore di 1.",
		arguments: [
		]
	},
	{
		name: "CASUALE.TRA",
		description: "Restituisce un numero casuale compreso tra i numeri specificati.",
		arguments: [
			{
				name: "minore",
				description: "è l'intero più piccolo restituito da CASUALE.TRA"
			},
			{
				name: "maggiore",
				description: "è l'intero più grande restituito da CASUALE.TRA"
			}
		]
	},
	{
		name: "CELLA",
		description: "Restituisce informazioni sulla formattazione, sulla posizione o sul contenuto della prima cella in un riferimento, in base all'ordine di lettura del foglio.",
		arguments: [
			{
				name: "info",
				description: "è un valore di testo che specifica il tipo di informazioni sulla cella desiderato"
			},
			{
				name: "rif",
				description: "è la cella sulla quale si desidera ottenere informazioni"
			}
		]
	},
	{
		name: "CERCA",
		description: "Ricerca un valore in un intervallo di una riga o di una colonna o da una matrice. Fornito per compatibilità con versioni precedenti.",
		arguments: [
			{
				name: "valore",
				description: "è un valore che CERCA in Vettore e può essere un numero, un testo, un valore logico, un nome o un riferimento a un valore"
			},
			{
				name: "vettore",
				description: "è un intervallo che contiene solo una riga o una colonna di testo, numeri o valori logici posti in ordine ascendente"
			},
			{
				name: "risultato",
				description: "è un intervallo che contiene solo una riga o una colonna delle stesse dimensioni di Vettore"
			}
		]
	},
	{
		name: "CERCA.ORIZZ",
		description: "Cerca un valore nella prima riga di una tabella o di una matrice e restituisce il valore nella stessa colonna da una riga specificata.",
		arguments: [
			{
				name: "valore",
				description: "è il valore da ricercare nella prima riga della tabella e può essere un valore, un riferimento o una stringa di testo"
			},
			{
				name: "matrice_tabella",
				description: "è la tabella di testo, numeri o valori logici nella quale vengono cercati i dati. Matrice_tabella può essere un riferimento a un intervallo o un nome di intervallo."
			},
			{
				name: "indice",
				description: "è il numero di riga in matrice_tabella da cui viene restituito il valore corrispondente. Riga 1 è la prima riga di valori nella tabella."
			},
			{
				name: "intervallo",
				description: "è un valore logico: per trovare la corrispondenza più simile nella prima riga, ordinata in ordine ascendente, = VERO oppure omesso; per trovare una corrispondenza esatta = FALSO"
			}
		]
	},
	{
		name: "CERCA.VERT",
		description: "Cerca un valore nella prima colonna sinistra di una tabella e restituisce un valore nella stessa riga da una colonna specificata. La tabella viene ordinata in ordine ascendente per impostazione predefinita.",
		arguments: [
			{
				name: "valore",
				description: "è il valore da ricercare nella prima colonna della tabella e può essere un valore, un riferimento o una stringa di testo"
			},
			{
				name: "matrice_tabella",
				description: "è una tabella di testo, numeri o valori logici nella quale vengono cercati i dati. Matrice_tabella può essere un riferimento a un intervallo o un nome di intervallo."
			},
			{
				name: "indice",
				description: "è il numero di colonna in matrice_tabella da cui viene restituito il valore corrispondente. La prima colonna di valori nella tabella è la colonna 1."
			},
			{
				name: "intervallo",
				description: "è un valore logico: per trovare la corrispondenza più simile nella prima colonna (in ordine ascendente) = VERO oppure omesso; trova una corrispondenza esatta = FALSO"
			}
		]
	},
	{
		name: "CODICE",
		description: "Restituisce il codice numerico del primo carattere di una stringa di testo, in base al set di caratteri installato nel sistema.",
		arguments: [
			{
				name: "testo",
				description: "è il testo da cui ottenere il codice del primo carattere"
			}
		]
	},
	{
		name: "CODICE.CARATT",
		description: "Restituisce il carattere specificato dal numero di codice del set di caratteri del computer.",
		arguments: [
			{
				name: "num",
				description: "è un numero compreso tra 1e 255 che specifica il carattere desiderato"
			}
		]
	},
	{
		name: "CODIFICA.URL",
		description: "Restituisce una stringa con codifica URL.",
		arguments: [
			{
				name: "text",
				description: "è una stringa cui applicare la codifica URL"
			}
		]
	},
	{
		name: "COLLEG.IPERTESTUALE",
		description: "Crea un collegamento ipertestuale che apre un documento memorizzato sul disco rigido, su un server di rete o su Internet.",
		arguments: [
			{
				name: "posizione_collegamento",
				description: "è il percorso completo e il nome del documento che verrà aperto, la posizione di un disco rigido, un indirizzo UNC o un percorso URL"
			},
			{
				name: "nome_collegamento",
				description: "è il testo o il numero visualizzato nella cella. Se viene omesso, nella cella verrà visualizzato il testo Posizione_collegamento"
			}
		]
	},
	{
		name: "COLONNE",
		description: "Restituisce il numero di colonne in una matrice o riferimento.",
		arguments: [
			{
				name: "matrice",
				description: "è una matrice o una formula in forma di matrice oppure un riferimento ad un intervallo di celle da cui ottenere il numero di colonne"
			}
		]
	},
	{
		name: "COMBINAZIONE",
		description: "Calcola il numero delle combinazioni per un numero assegnato di oggetti.",
		arguments: [
			{
				name: "num",
				description: "è il numero totale di oggetti"
			},
			{
				name: "classe",
				description: "è il numero degli oggetti in ciascuna combinazione"
			}
		]
	},
	{
		name: "COMBINAZIONE.VALORI",
		description: "Restituisce il numero delle combinazioni con ripetizioni per un numero specificato di elementi.",
		arguments: [
			{
				name: "num",
				description: "è il numero totale di elementi"
			},
			{
				name: "classe",
				description: "è il numero di elementi in ogni combinazione"
			}
		]
	},
	{
		name: "COMP.ARGOMENTO",
		description: "Restituisce l'argomento teta, un angolo espresso in radianti.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare l'argomento"
			}
		]
	},
	{
		name: "COMP.CONIUGATO",
		description: "Restituisce il complesso coniugato di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui ottenere il coniugato"
			}
		]
	},
	{
		name: "COMP.COS",
		description: "Restituisce il coseno di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare il coseno"
			}
		]
	},
	{
		name: "COMP.COSH",
		description: "Restituisce il coseno iperbolico di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare il coseno iperbolico"
			}
		]
	},
	{
		name: "COMP.COT",
		description: "Restituisce la cotangente di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare la cotangente"
			}
		]
	},
	{
		name: "COMP.CSC",
		description: "Restituisce la cosecante di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare la cosecante"
			}
		]
	},
	{
		name: "COMP.CSCH",
		description: "Restituisce la cosecante iperbolica di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare la cosecante iperbolica"
			}
		]
	},
	{
		name: "COMP.DIFF",
		description: "Restituisce la differenza di due numeri complessi.",
		arguments: [
			{
				name: "num_comp1",
				description: "è il numero complesso da cui si desidera sottrarre 'num_comp2'"
			},
			{
				name: "num_comp2",
				description: "è il numero complesso da sottrarre a 'num_comp1'"
			}
		]
	},
	{
		name: "COMP.DIV",
		description: "Restituisce il quoziente di due numeri complessi.",
		arguments: [
			{
				name: "num_comp1",
				description: "è il numeratore o dividendo complesso"
			},
			{
				name: "num_comp2",
				description: "è il denominatore o divisore complesso"
			}
		]
	},
	{
		name: "COMP.EXP",
		description: "Restituisce l'esponenziale di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare l'esponenziale"
			}
		]
	},
	{
		name: "COMP.IMMAGINARIO",
		description: "Restituisce il coefficiente dell'immaginario di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui ottenere il coefficiente dell'immaginario"
			}
		]
	},
	{
		name: "COMP.LN",
		description: "Restituisce il logaritmo naturale di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare il logaritmo naturale"
			}
		]
	},
	{
		name: "COMP.LOG10",
		description: "Restituisce il logaritmo in base 10 di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare il logaritmo"
			}
		]
	},
	{
		name: "COMP.LOG2",
		description: "Restituisce il logaritmo in base 2 di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare il logaritmo in base 2"
			}
		]
	},
	{
		name: "COMP.MODULO",
		description: "Restituisce il valore assoluto (modulo) di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare il valore assoluto"
			}
		]
	},
	{
		name: "COMP.PARTE.REALE",
		description: "Restituisce la parte reale di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui ottenere la parte reale"
			}
		]
	},
	{
		name: "COMP.POTENZA",
		description: "Restituisce un numero complesso elevato a un esponente intero.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso da elevare a potenza"
			},
			{
				name: "num",
				description: "è l'esponente a cui elevare il numero complesso"
			}
		]
	},
	{
		name: "COMP.PRODOTTO",
		description: "Restituisce il prodotto di numeri complessi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num_comp1",
				description: "num_comp1, num_comp2,... sono da 1 a 255 numeri complessi da moltiplicare"
			},
			{
				name: "num_comp2",
				description: "num_comp1, num_comp2,... sono da 1 a 255 numeri complessi da moltiplicare"
			}
		]
	},
	{
		name: "COMP.RADQ",
		description: "Restituisce la radice quadrata di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare la radice quadrata"
			}
		]
	},
	{
		name: "COMP.SEC",
		description: "Restituisce la secante di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare la secante"
			}
		]
	},
	{
		name: "COMP.SECH",
		description: "Restituisce la secante iperbolica di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare la secante iperbolica"
			}
		]
	},
	{
		name: "COMP.SEN",
		description: "Restituisce il seno di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare il seno"
			}
		]
	},
	{
		name: "COMP.SENH",
		description: "Restituisce il seno iperbolico di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare il seno iperbolico"
			}
		]
	},
	{
		name: "COMP.SOMMA",
		description: "Restituisce la somma di numeri complessi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num_comp1",
				description: "sono da 1 a 255 numeri complessi da sommare"
			},
			{
				name: "num_comp2",
				description: "sono da 1 a 255 numeri complessi da sommare"
			}
		]
	},
	{
		name: "COMP.TAN",
		description: "Restituisce la tangente di un numero complesso.",
		arguments: [
			{
				name: "num_comp",
				description: "è il numero complesso di cui calcolare la tangente"
			}
		]
	},
	{
		name: "COMPLESSO",
		description: "Converte la parte reale e il coefficiente dell'immaginario in un numero complesso.",
		arguments: [
			{
				name: "parte_reale",
				description: "è la parte reale del numero complesso"
			},
			{
				name: "coeff_imm",
				description: "è il coefficiente dell'immaginario del numero complesso"
			},
			{
				name: "suffisso",
				description: "è il suffisso per la parte immaginaria del numero complesso"
			}
		]
	},
	{
		name: "CONCATENA",
		description: "Unisce diverse stringhe di testo in una singola stringa.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "testo1",
				description: "sono da 1 a 255 stringhe di testo da unire in una singola stringa di testo e possono essere stringhe di testo, numeri o riferimenti a celle singole"
			},
			{
				name: "testo2",
				description: "sono da 1 a 255 stringhe di testo da unire in una singola stringa di testo e possono essere stringhe di testo, numeri o riferimenti a celle singole"
			}
		]
	},
	{
		name: "CONFIDENZA",
		description: "Restituisce l'intervallo di confidenza per una popolazione, utilizzando una distribuzione normale.",
		arguments: [
			{
				name: "alfa",
				description: "è il livello di significatività utilizzato per calcolare il livello di confidenza, un numero maggiore di 0 e minore di 1"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard della popolazione per l'intervallo di dati e si presuppone sia nota. Dev_standard deve essere maggiore di 0."
			},
			{
				name: "dimensioni",
				description: "è la dimensione del campione"
			}
		]
	},
	{
		name: "CONFIDENZA.NORM",
		description: "Restituisce l'intervallo di confidenza per una popolazione, utilizzando una distribuzione normale.",
		arguments: [
			{
				name: "alfa",
				description: "è il livello di significatività utilizzato per calcolare il livello di confidenza, un numero maggiore di 0 e minore di 1"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard della popolazione per l'intervallo di dati e si presuppone sia nota. Dev_standard deve essere maggiore di 0."
			},
			{
				name: "dimensioni",
				description: "è la dimensione del campione"
			}
		]
	},
	{
		name: "CONFIDENZA.T",
		description: "Restituisce l'intervallo di confidenza per una popolazione, utilizzando una distribuzione T di Student.",
		arguments: [
			{
				name: "alfa",
				description: "è il livello di significatività utilizzato per calcolare il livello di confidenza, un numero maggiore di 0 e minore di 1"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard della popolazione per l'intervallo di dati e si presuppone sia nota. Dev_standard deve essere maggiore di 0."
			},
			{
				name: "dimensioni",
				description: "è la dimensione del campione"
			}
		]
	},
	{
		name: "CONFRONTA",
		description: "Restituisce la posizione relativa di un elemento di matrice che corrisponde a un valore specificato in un ordine specificato.",
		arguments: [
			{
				name: "valore",
				description: "è il valore utilizzato per cercare il valore desiderato nella matrice, un numero, testo o un valore logico oppure un riferimento ad essi"
			},
			{
				name: "matrice",
				description: "è un intervallo contiguo di celle che contengono i possibili valori da cercare, una matrice di valori o un riferimento a una matrice"
			},
			{
				name: "corrisp",
				description: "è un numero (1, 0 o -1) che indica il valore da restituire."
			}
		]
	},
	{
		name: "CONTA.NUMERI",
		description: "Conta il numero di celle in un intervallo contenente numeri e i numeri presenti nell'elenco degli argomenti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "val1",
				description: "sono da 1 a 255 argomenti che possono contenere o riferirsi a più tipi di dati, di cui vengono contati soltanto i numeri"
			},
			{
				name: "val2",
				description: "sono da 1 a 255 argomenti che possono contenere o riferirsi a più tipi di dati, di cui vengono contati soltanto i numeri"
			}
		]
	},
	{
		name: "CONTA.PIÙ.SE",
		description: "Conta il numero di celle specificate da un determinato insieme di condizioni o criteri.",
		arguments: [
			{
				name: "intervallo_criteri",
				description: "è l'intervallo di celle da valutare per una particolare condizione"
			},
			{
				name: "criteri",
				description: "è la condizione, in forma di numero, espressione o testo, che definisce le celle da contare"
			}
		]
	},
	{
		name: "CONTA.SE",
		description: "Conta il numero di celle in un intervallo che corrispondono al criterio dato.",
		arguments: [
			{
				name: "intervallo",
				description: "è l'intervallo di celle di cui contare le celle non vuote"
			},
			{
				name: "criterio",
				description: "è la condizione in forma di numero, espressione o testo che definisce le celle da contare"
			}
		]
	},
	{
		name: "CONTA.VALORI",
		description: "Conta il numero delle celle non vuote e i valori presenti nell'elenco degli argomenti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "val1",
				description: "sono da 1 a 255 argomenti che rappresentano i valori e le celle da contare. Il valore può essere costituito da qualsiasi tipo di informazione."
			},
			{
				name: "val2",
				description: "sono da 1 a 255 argomenti che rappresentano i valori e le celle da contare. Il valore può essere costituito da qualsiasi tipo di informazione."
			}
		]
	},
	{
		name: "CONTA.VUOTE",
		description: "Conta il numero di celle vuote in uno specificato intervallo.",
		arguments: [
			{
				name: "intervallo",
				description: "è l'intervallo di cui contare le celle vuote"
			}
		]
	},
	{
		name: "CONVERTI",
		description: "Converte un numero da un sistema di unità di misura a un altro.",
		arguments: [
			{
				name: "num",
				description: "è il valore da convertire"
			},
			{
				name: "da_misura",
				description: "è l'unità di misura per num"
			},
			{
				name: "a_misura",
				description: "è l'unità di misura per il risultato"
			}
		]
	},
	{
		name: "CORRELAZIONE",
		description: "Restituisce il coefficiente di correlazione tra due set di dati.",
		arguments: [
			{
				name: "matrice1",
				description: "è un intervallo di celle di valori. I valori possono essere numeri, nomi, matrici o riferimenti contenenti numeri"
			},
			{
				name: "matrice2",
				description: "è il secondo intervallo di celle di valori. I valori possono essere numeri, nomi, matrici o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "COS",
		description: "Restituisce il coseno di un numero.",
		arguments: [
			{
				name: "num",
				description: "è l'angolo espresso in radianti di cui si calcola il coseno"
			}
		]
	},
	{
		name: "COSH",
		description: "Restituisce il coseno iperbolico di un numero.",
		arguments: [
			{
				name: "num",
				description: "è un numero reale qualsiasi"
			}
		]
	},
	{
		name: "COT",
		description: "Restituisce la cotangente di un angolo.",
		arguments: [
			{
				name: "num",
				description: "è l'angolo espresso in radianti di cui si calcola la cotangente"
			}
		]
	},
	{
		name: "COTH",
		description: "Restituisce la cotangente iperbolica di un numero.",
		arguments: [
			{
				name: "num",
				description: "è l'angolo espresso in radianti di cui si calcola la cotangente iperbolica"
			}
		]
	},
	{
		name: "COVARIANZA",
		description: "Calcola la covarianza, la media dei prodotti delle deviazioni di ciascuna coppia di coordinate in due set di dati.",
		arguments: [
			{
				name: "matrice1",
				description: "è il primo intervallo di celle di interi e deve essere costituito da numeri, matrici o riferimenti contenenti numeri"
			},
			{
				name: "matrice2",
				description: "è il secondo intervallo di celle di interi e deve essere costituito da numeri, matrici o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "COVARIANZA.C",
		description: "Calcola la covarianza del campione, la media dei prodotti delle deviazioni di ciascuna coppia di coordinate in due set di dati.",
		arguments: [
			{
				name: "matrice1",
				description: "è il primo intervallo di celle di interi e deve essere costituito da numeri, matrici o riferimenti contenenti numeri"
			},
			{
				name: "matrice2",
				description: "è il secondo intervallo di celle di interi e deve essere costituito da numeri, matrici o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "COVARIANZA.P",
		description: "Calcola la covarianza della popolazione, la media dei prodotti delle deviazioni di ciascuna coppia di coordinate in due set di dati.",
		arguments: [
			{
				name: "matrice1",
				description: "è il primo intervallo di celle di interi e deve essere costituito da numeri, matrici o riferimenti contenenti numeri"
			},
			{
				name: "matrice2",
				description: "è il secondo intervallo di celle di interi e deve essere costituito da numeri, matrici o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "CRESCITA",
		description: "Calcola la crescita esponenziale prevista utilizzando coordinate esistenti.",
		arguments: [
			{
				name: "y_nota",
				description: "è l'insieme dei valori y già noti dalla relazione y = b*m^x, da una matrice o da un intervallo di valori positivi"
			},
			{
				name: "x_nota",
				description: "è un insieme facoltativo di valori x che possono essere già noti dalla relazione y = b*m^x, da una matrice o da un intervallo della stessa dimensione di y_nota"
			},
			{
				name: "nuova_x",
				description: "sono i nuovi valori x per i quali CRESCITA restituirà i corrispondenti valori y"
			},
			{
				name: "cost",
				description: "è un valore logico: la costante b viene calcolata normalmente se Const = VERO; b è impostato a 1 se Const = FALSO oppure omesso"
			}
		]
	},
	{
		name: "CRIT.BINOM",
		description: "Restituisce il più piccolo valore per il quale la distribuzione cumulativa binomiale risulta maggiore o uguale ad un valore di criterio.",
		arguments: [
			{
				name: "prove",
				description: "è il numero delle prove di Bernoulli"
			},
			{
				name: "probabilità_s",
				description: "è la probabilità di un successo ad ogni prova, un numero tra 0 e 1 inclusi"
			},
			{
				name: "alfa",
				description: "è il valore di criterio, un numero tra 0 e 1 inclusi"
			}
		]
	},
	{
		name: "CSC",
		description: "Restituisce la cosecante di un angolo.",
		arguments: [
			{
				name: "num",
				description: "è l'angolo espresso in radianti di cui si calcola la cosecante"
			}
		]
	},
	{
		name: "CSCH",
		description: "Restituisce la cosecante iperbolica di un angolo.",
		arguments: [
			{
				name: "num",
				description: "è l'angolo espresso in radianti di cui si calcola la cosecante iperbolica"
			}
		]
	},
	{
		name: "CURTOSI",
		description: "Restituisce la curtosi di un set di dati.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la curtosi"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la curtosi"
			}
		]
	},
	{
		name: "DATA",
		description: "Restituisce il numero che rappresenta la data in codice data-ora di Spreadsheet.",
		arguments: [
			{
				name: "anno",
				description: "è un numero compreso tra 1900 e 9999 in Spreadsheet per Windows o tra 1904 e 9999 in Spreadsheet per Macintosh"
			},
			{
				name: "mese",
				description: "è un numero compreso tra 1 e 12 che rappresenta il mese dell'anno"
			},
			{
				name: "giorno",
				description: "è un numero compreso tra 1 e 31 che rappresenta il giorno del mese"
			}
		]
	},
	{
		name: "DATA.CED.PREC",
		description: "Restituisce la data della cedola precedente alla data di liquidazione.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del titolo espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del titolo espressa come numero seriale"
			},
			{
				name: "num_rate",
				description: "è il numero di pagamenti per anno"
			},
			{
				name: "base",
				description: "è il tipo di base da utilizzare per il conteggio dei giorni"
			}
		]
	},
	{
		name: "DATA.CED.SUCC",
		description: "Restituisce la data della cedola successiva alla data di liquidazione.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del titolo espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del titolo espressa come numero seriale"
			},
			{
				name: "num_rate",
				description: "è il numero di pagamenti per anno"
			},
			{
				name: "base",
				description: "è il tipo di base da utilizzare per il conteggio dei giorni"
			}
		]
	},
	{
		name: "DATA.DIFF",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATA.MESE",
		description: "Restituisce il numero seriale della data il cui mese è precedente o successivo a quello della data iniziale, a seconda del numero indicato dall'argomento mesi.",
		arguments: [
			{
				name: "data_iniziale",
				description: "è la data iniziale espressa come numero seriale"
			},
			{
				name: "mesi",
				description: "è il numero di mesi precedenti o successivi alla data iniziale"
			}
		]
	},
	{
		name: "DATA.VALORE",
		description: "Converte una data in formato testo in un numero che rappresenta la data nel codice data-ora di Spreadsheet.",
		arguments: [
			{
				name: "data",
				description: "è il testo che rappresenta una data in formato data di Spreadsheet, tra 01/01/1900 (Windows) o 01/01/1904 (Macintosh) e 31/12/9999"
			}
		]
	},
	{
		name: "DATITEMPOREALE",
		description: "Scarica dati in tempo reale da un programma che supporta l'automazione COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "IDprog",
				description: "è il nome dell'IDprog di un componente aggiuntivo registrato dell'automazione COM. Racchiudere il nome fra virgolette"
			},
			{
				name: "server",
				description: "è il nome del server sul quale deve essere eseguito il componente aggiuntivo. Racchiudere il nome fra virgolette. Se il componente aggiuntivo viene eseguito in locale, usare una stringa vuota."
			},
			{
				name: "arg1",
				description: "sono da 1 a 38 parametri che specificano una porzione di dati"
			},
			{
				name: "arg2",
				description: "sono da 1 a 38 parametri che specificano una porzione di dati"
			}
		]
	},
	{
		name: "DB.CONTA.NUMERI",
		description: "Conta le celle nel campo (colonna) dei record del database che soddisfano le condizioni specificate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta di colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco"
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sotto l'etichetta."
			}
		]
	},
	{
		name: "DB.CONTA.VALORI",
		description: "Conta le celle non vuote nel campo (colonna) dei record del database che soddisfano le condizioni specificate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta di colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco"
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sotto l'etichetta."
			}
		]
	},
	{
		name: "DB.DEV.ST",
		description: "Stima la deviazione standard sulla base di un campione di voci del database selezionate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta della colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco"
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sottostante in cui immettere una condizione."
			}
		]
	},
	{
		name: "DB.DEV.ST.POP",
		description: "Calcola la deviazione standard sulla base dell'intera popolazione di voci del database selezionate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta della colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco"
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sottostante in cui immettere una condizione."
			}
		]
	},
	{
		name: "DB.MAX",
		description: "Restituisce il valore massimo nel campo (colonna) di record del database che soddisfa le condizioni specificate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta della colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco"
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sottostante in cui immettere una condizione."
			}
		]
	},
	{
		name: "DB.MEDIA",
		description: "Restituisce la media dei valori di una colonna di un elenco o di un database che soddisfano le condizioni specificate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta di colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco."
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sottostante in cui immettere una condizione"
			}
		]
	},
	{
		name: "DB.MIN",
		description: "Restituisce il valore minimo nel campo (colonna) di record del database che soddisfa le condizioni specificate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta della colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco"
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sottostante in cui immettere una condizione."
			}
		]
	},
	{
		name: "DB.PRODOTTO",
		description: "Moltiplica i valori nel campo (colonna) di record del database che soddisfano le condizioni specificate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta della colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco"
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sottostante in cui immettere una condizione."
			}
		]
	},
	{
		name: "DB.SOMMA",
		description: "Aggiunge i numeri nel campo (colonna) di record del database che soddisfano le condizioni specificate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta della colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco"
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sottostante in cui immettere una condizione."
			}
		]
	},
	{
		name: "DB.VALORI",
		description: "Estrae da un database un singolo record che soddisfa le condizioni specificate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta della colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco"
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sottostante in cui immettere una condizione."
			}
		]
	},
	{
		name: "DB.VAR",
		description: "Stima la varianza sulla base di un campione di voci del database selezionate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta della colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco"
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sottostante in cui immettere una condizione."
			}
		]
	},
	{
		name: "DB.VAR.POP",
		description: "Calcola la varianza sulla base dell'intera popolazione di voci del database selezionate.",
		arguments: [
			{
				name: "database",
				description: "è l'intervallo di celle che costituisce l'elenco o il database. Un database è un elenco di dati correlati."
			},
			{
				name: "campo",
				description: "è l'etichetta della colonna tra virgolette o un numero che rappresenta la posizione della colonna nell'elenco"
			},
			{
				name: "criteri",
				description: "è l'intervallo di celle che contiene le condizioni specificate. L'intervallo include un'etichetta di colonna e una cella sottostante in cui immettere una condizione."
			}
		]
	},
	{
		name: "DECIMALE",
		description: "Converte una rappresentazione testuale di un numero con una base specificata in un numero decimale.",
		arguments: [
			{
				name: "num",
				description: "è il numero che si vuole convertire"
			},
			{
				name: "radice",
				description: "è la radice di base del numero da convertire"
			}
		]
	},
	{
		name: "DECIMALE.BINARIO",
		description: "Converte un numero decimale in binario.",
		arguments: [
			{
				name: "num",
				description: "è l'intero decimale da convertire"
			},
			{
				name: "cifre",
				description: "è il numero di caratteri da utilizzare"
			}
		]
	},
	{
		name: "DECIMALE.HEX",
		description: "Converte un numero decimale in esadecimale.",
		arguments: [
			{
				name: "num",
				description: "è l'intero decimale da convertire"
			},
			{
				name: "cifre",
				description: "è il numero di caratteri da utilizzare"
			}
		]
	},
	{
		name: "DECIMALE.OCT",
		description: "Converte un numero decimale in ottale.",
		arguments: [
			{
				name: "num",
				description: "è l'intero decimale da convertire"
			},
			{
				name: "cifre",
				description: "è il numero di caratteri da utilizzare"
			}
		]
	},
	{
		name: "DELTA",
		description: "Verifica se due numeri sono uguali.",
		arguments: [
			{
				name: "num1",
				description: "è il primo numero."
			},
			{
				name: "num2",
				description: "è il secondo numero"
			}
		]
	},
	{
		name: "DESTRA",
		description: "Restituisce il carattere o i caratteri più a destra di una stringa di testo.",
		arguments: [
			{
				name: "testo",
				description: "è la stringa di testo che contiene i caratteri da estrarre"
			},
			{
				name: "num_caratt",
				description: "specifica il numero dei caratteri da estrarre, 1 se omesso"
			}
		]
	},
	{
		name: "DEV.Q",
		description: "Restituisce la somma dei quadrati delle deviazioni delle coordinate dalla media di queste ultime sul campione.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti, una matrice o un riferimento a una matrice in base ai quali calcolare DEV.Q"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti, una matrice o un riferimento a una matrice in base ai quali calcolare DEV.Q"
			}
		]
	},
	{
		name: "DEV.ST",
		description: "Restituisce una stima della deviazione standard sulla base di un campione. Ignora i valori logici e il testo nel campione.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti corrispondenti a un campione della popolazione e possono essere numeri o riferimenti contenenti numeri"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti corrispondenti a un campione della popolazione e possono essere numeri o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "DEV.ST.C",
		description: "Restituisce una stima della deviazione standard sulla base di un campione. Ignora i valori logici e il testo nel campione.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti corrispondenti a un campione della popolazione e possono essere numeri o riferimenti contenenti numeri"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti corrispondenti a un campione della popolazione e possono essere numeri o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "DEV.ST.P",
		description: "Calcola la deviazione standard sulla base dell'intera popolazione, passata come argomenti (ignora i valori logici e il testo).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti corrispondenti a una popolazione e possono essere numeri o riferimenti contenenti numeri"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti corrispondenti a una popolazione e possono essere numeri o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "DEV.ST.POP",
		description: "Calcola la deviazione standard sulla base dell'intera popolazione, passata come argomenti (ignora i valori logici e il testo).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti corrispondenti a una popolazione e possono essere numeri o riferimenti contenenti numeri"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti corrispondenti a una popolazione e possono essere numeri o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "DEV.ST.POP.VALORI",
		description: "Calcola la deviazione standard sulla base dell'intera popolazione, inclusi valori logici e testo. Il testo e il valore FALSO vengono valutati come 0, il valore VERO come 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "val1",
				description: "sono da 1 a 255 valori corrispondenti a una popolazione e possono essere valori, nomi, matrici o riferimenti contenenti valori"
			},
			{
				name: "val2",
				description: "sono da 1 a 255 valori corrispondenti a una popolazione e possono essere valori, nomi, matrici o riferimenti contenenti valori"
			}
		]
	},
	{
		name: "DEV.ST.VALORI",
		description: "Restituisce una stima della deviazione standard sulla base di un campione, inclusi valori logici e testo. Il testo e il valore FALSO vengono valutati come 0, il valore VERO come 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "val1",
				description: "sono da 1 a 255 valori corrispondenti a un campione di popolazione e possono essere valori, nomi o riferimenti a valori"
			},
			{
				name: "val2",
				description: "sono da 1 a 255 valori corrispondenti a un campione di popolazione e possono essere valori, nomi o riferimenti a valori"
			}
		]
	},
	{
		name: "DISPARI",
		description: "Arrotonda un numero positivo per eccesso al numero intero più vicino e uno negativo per difetto al numero dispari più vicino.",
		arguments: [
			{
				name: "num",
				description: "è il valore da arrotondare"
			}
		]
	},
	{
		name: "DISTRIB.BETA",
		description: "Calcola la funzione densità di probabilità cumulativa beta.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la funzione nell'intervallo A  x  B"
			},
			{
				name: "alfa",
				description: "è un parametro per la distribuzione e deve essere maggiore di 0"
			},
			{
				name: "beta",
				description: "è un parametro per la distribuzione e deve essere maggiore di 0"
			},
			{
				name: "A",
				description: "è un valore facoltativo per l'estremo inferiore dell'intervallo di x. Se omesso, A = 0."
			},
			{
				name: "B",
				description: "è un valore facoltativo per l'estremo superiore dell'intervallo di x. Se omesso, B = 1."
			}
		]
	},
	{
		name: "DISTRIB.BETA.N",
		description: "Calcola la funzione di distribuzione probabilità beta.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la funzione nell'intervallo compreso tra A e B"
			},
			{
				name: "alfa",
				description: "è un parametro per la distribuzione e deve essere maggiore di 0"
			},
			{
				name: "beta",
				description: "è un parametro per la distribuzione e deve essere maggiore di 0"
			},
			{
				name: "cumulativa",
				description: "è un valore logico: per la funzione di distribuzione cumulativa, utilizzare VERO, per la funzione densità di probabilità, utilizzare FALSO"
			},
			{
				name: "A",
				description: "è un valore facoltativo per l'estremo inferiore dell'intervallo di x. Se omesso, A = 0."
			},
			{
				name: "B",
				description: "è un valore facoltativo per l'estremo superiore dell'intervallo di x. Se omesso, B = 1."
			}
		]
	},
	{
		name: "DISTRIB.BINOM",
		description: "Restituisce la distribuzione binomiale per il termine individuale.",
		arguments: [
			{
				name: "num_successi",
				description: "è il numero dei successi nelle prove"
			},
			{
				name: "prove",
				description: "è il numero di prove indipendenti"
			},
			{
				name: "probabilità_s",
				description: "è la probabilità di successo per ciascuna prova"
			},
			{
				name: "cumulativo",
				description: "è un valore logico: utilizzare VERO per la funzione distribuzione cumulativa; utilizzare FALSO per la funzione probabilità di massa"
			}
		]
	},
	{
		name: "DISTRIB.BINOM.N",
		description: "Restituisce la distribuzione binomiale per il termine individuale.",
		arguments: [
			{
				name: "num_successi",
				description: "è il numero dei successi nelle prove"
			},
			{
				name: "prove",
				description: "è il numero di prove indipendenti"
			},
			{
				name: "probabilità_s",
				description: "è la probabilità di successo per ciascuna prova"
			},
			{
				name: "cumulativo",
				description: "è un valore logico: utilizzare VERO per la funzione distribuzione cumulativa; utilizzare FALSO per la funzione probabilità di massa"
			}
		]
	},
	{
		name: "DISTRIB.BINOM.NEG",
		description: "Restituisce la distribuzione binomiale negativa, la probabilità che un numero di insuccessi pari a Num_insuccessi si verifichi prima del successo Num_successi, data la probabilità di successo Probabilità_s.",
		arguments: [
			{
				name: "num_insuccessi",
				description: "è il numero degli insuccessi"
			},
			{
				name: "num_successi",
				description: "è il numero di soglia per i successi"
			},
			{
				name: "probabilità_s",
				description: "è la probabilità di avere un successo, un numero compreso tra 0 e 1"
			}
		]
	},
	{
		name: "DISTRIB.BINOM.NEG.N",
		description: "Restituisce la distribuzione binomiale negativa, la probabilità che un numero di insuccessi pari a Num_insuccessi si verifichi prima del successo Num_successi, data la probabilità di successo Probabilità_s.",
		arguments: [
			{
				name: "num_insuccessi",
				description: "è il numero degli insuccessi"
			},
			{
				name: "num_successi",
				description: "è il numero di soglia per i successi"
			},
			{
				name: "probabilità_s",
				description: "è la probabilità di avere un successo, un numero compreso tra 0 e 1"
			},
			{
				name: "cumulativa",
				description: "è un valore logico: per la funzione di distribuzione cumulativa, utilizzare VERO, per la funzione probabilità di massa, utilizzare FALSO"
			}
		]
	},
	{
		name: "DISTRIB.CHI",
		description: "Restituisce la probabilità a una coda destra per la distribuzione del chi quadrato.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la distribuzione, un numero non negativo"
			},
			{
				name: "grad_libertà",
				description: "è il numero di gradi di libertà, un numero compreso tra 1 e 10^10, escluso 10^10"
			}
		]
	},
	{
		name: "DISTRIB.CHI.QUAD",
		description: "Restituisce la probabilità a una coda sinistra per la distribuzione del chi quadrato.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la distribuzione, un numero non negativo"
			},
			{
				name: "grad_libertà",
				description: "è il numero di gradi di libertà, un numero compreso tra 1 e 10^10, escluso 10^10"
			},
			{
				name: "cumulativa",
				description: "è un valore logico per la funzione da restituire: la funzione di distribuzione cumulativa = VERO; la funzione densità di probabilità = FALSO"
			}
		]
	},
	{
		name: "DISTRIB.CHI.QUAD.DS",
		description: "Restituisce la probabilità ad una coda destra per la distribuzione del chi quadrato.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la distribuzione, un numero non negativo"
			},
			{
				name: "grad_libertà",
				description: "è il numero di gradi di libertà, un numero compreso tra 1 e 10^10, escluso 10^10"
			}
		]
	},
	{
		name: "DISTRIB.EXP",
		description: "Restituisce la distribuzione esponenziale.",
		arguments: [
			{
				name: "x",
				description: "è il valore della funzione, un numero non negativo"
			},
			{
				name: "lambda",
				description: "è il valore del parametro, un numero positivo"
			},
			{
				name: "cumulativo",
				description: "è un valore logico che la funzione deve restituire: la funzione distribuzione cumulativa = VERO; la funzione densità di probabilità = FALSO"
			}
		]
	},
	{
		name: "DISTRIB.EXP.N",
		description: "Restituisce la distribuzione esponenziale.",
		arguments: [
			{
				name: "x",
				description: "è il valore della funzione, un numero non negativo"
			},
			{
				name: "lambda",
				description: "è il valore del parametro, un numero positivo"
			},
			{
				name: "cumulativo",
				description: "è un valore logico che la funzione deve restituire: la funzione distribuzione cumulativa = VERO; la funzione densità di probabilità = FALSO"
			}
		]
	},
	{
		name: "DISTRIB.F",
		description: "Restituisce la distribuzione di probabilità F (coda destra) (grado di diversità) per due set di dati.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la funzione, un numero non negativo"
			},
			{
				name: "grad_libertà1",
				description: "sono i gradi di libertà al numeratore, un numero compreso tra 1 e 10^10, escluso 10^10"
			},
			{
				name: "grad_libertà2",
				description: "sono i gradi di libertà al denominatore, un numero compreso tra 1 e 10^10, escluso 10^10"
			}
		]
	},
	{
		name: "DISTRIB.F.DS",
		description: "Restituisce la distribuzione di probabilità F (coda destra) (grado di diversità) per due set di dati.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la funzione, un numero non negativo"
			},
			{
				name: "grad_libertà1",
				description: "sono i gradi di libertà al numeratore, un numero compreso tra 1 e 10^10, escluso 10^10"
			},
			{
				name: "grad_libertà2",
				description: "sono i gradi di libertà al denominatore, un numero compreso tra 1 e 10^10, escluso 10^10"
			}
		]
	},
	{
		name: "DISTRIB.GAMMA",
		description: "Restituisce la distribuzione gamma.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la distribuzione, un numero non negativo"
			},
			{
				name: "alfa",
				description: "è un parametro per la distribuzione, un numero positivo"
			},
			{
				name: "beta",
				description: "è un parametro per la distribuzione, un numero positivo. Se beta = 1, DISTRIB.GAMMA restituisce la distribuzione gamma standard."
			},
			{
				name: "cumulativo",
				description: "è un valore logico: restituisce la funzione distribuzione cumulativa = VERO; restituisce la funzione probabilità di massa = FALSO oppure omesso"
			}
		]
	},
	{
		name: "DISTRIB.GAMMA.N",
		description: "Restituisce la distribuzione gamma.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la distribuzione, un numero non negativo"
			},
			{
				name: "alfa",
				description: "è un parametro per la distribuzione, un numero positivo"
			},
			{
				name: "beta",
				description: "è un parametro per la distribuzione, un numero positivo. Se beta = 1, DISTRIB.GAMMA.N restituisce la distribuzione gamma standard."
			},
			{
				name: "cumulativo",
				description: "è un valore logico: restituisce la funzione distribuzione cumulativa = VERO; restituisce la funzione probabilità di massa = FALSO oppure omesso"
			}
		]
	},
	{
		name: "DISTRIB.IPERGEOM",
		description: "Restituisce la distribuzione ipergeometrica.",
		arguments: [
			{
				name: "s_esempio",
				description: "è il numero di successi nel campione"
			},
			{
				name: "num_esempio",
				description: "è la dimensione del campione"
			},
			{
				name: "s_pop",
				description: "è il numero di successi nella popolazione"
			},
			{
				name: "num_pop",
				description: "è la dimensione della popolazione"
			}
		]
	},
	{
		name: "DISTRIB.IPERGEOM.N",
		description: "Restituisce la distribuzione ipergeometrica.",
		arguments: [
			{
				name: "s_campione",
				description: "è il numero di successi nel campione"
			},
			{
				name: "num_campione",
				description: "è la dimensione del campione"
			},
			{
				name: "s_pop",
				description: "è il numero di successi nella popolazione"
			},
			{
				name: "num_pop",
				description: "è la dimensione della popolazione"
			},
			{
				name: "cumulativa",
				description: "è un valore logico: per la funzione di distribuzione cumulativa, utilizzare VERO, per la funzione densità di probabilità, utilizzare FALSO"
			}
		]
	},
	{
		name: "DISTRIB.LOGNORM",
		description: "Restituisce la distribuzione lognormale di x, in cui ln(x) è distribuito normalmente con i parametri Media e Dev_standard.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la funzione, un numero positivo"
			},
			{
				name: "media",
				description: "è la media di ln(x)"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard di ln(x), un numero positivo"
			}
		]
	},
	{
		name: "DISTRIB.LOGNORM.N",
		description: "Restituisce la distribuzione lognormale di x, in cui ln(x) è distribuito normalmente con i parametri Media e Dev_standard.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la funzione, un numero positivo"
			},
			{
				name: "media",
				description: "è la media di ln(x)"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard di ln(x), un numero positivo"
			},
			{
				name: "cumulativa",
				description: "è un valore logico: per la funzione di distribuzione cumulativa, utilizzare VERO, per la funzione densità di probabilità, utilizzare FALSO"
			}
		]
	},
	{
		name: "DISTRIB.NORM",
		description: "Restituisce la distribuzione normale cumulativa per la media e la deviazione standard specificate.",
		arguments: [
			{
				name: "x",
				description: "è il valore per il quale si desidera la distribuzione"
			},
			{
				name: "media",
				description: "è la media aritmetica della distribuzione"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard della distribuzione, un numero positivo"
			},
			{
				name: "cumulativo",
				description: "è un valore logico: utilizzare VERO per la funzione distribuzione cumulativa; utilizzare FALSO per la funzione probabilità di densità"
			}
		]
	},
	{
		name: "DISTRIB.NORM.N",
		description: "Restituisce la distribuzione normale per la media e la deviazione standard specificate.",
		arguments: [
			{
				name: "x",
				description: "è il valore per il quale si desidera la distribuzione"
			},
			{
				name: "media",
				description: "è la media aritmetica della distribuzione"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard della distribuzione, un numero positivo"
			},
			{
				name: "cumulativo",
				description: "è un valore logico: utilizzare VERO per la funzione distribuzione cumulativa; utilizzare FALSO per la funzione probabilità di densità"
			}
		]
	},
	{
		name: "DISTRIB.NORM.ST",
		description: "Restituisce la distribuzione normale standard cumulativa (ha media pari a zero e deviazione standard pari a 1).",
		arguments: [
			{
				name: "z",
				description: "è il valore per il quale si desidera la distribuzione"
			}
		]
	},
	{
		name: "DISTRIB.NORM.ST.N",
		description: "Restituisce la distribuzione normale standard cumulativa (ha media pari a zero e deviazione standard pari a 1).",
		arguments: [
			{
				name: "z",
				description: "è il valore per il quale si desidera la distribuzione"
			},
			{
				name: "cumulativa",
				description: "è un valore logico per la funzione da restituire: la funzione di distribuzione cumulativa = VERO; la funzione densità di probabilità = FALSO"
			}
		]
	},
	{
		name: "DISTRIB.POISSON",
		description: "Calcola la distribuzione di probabilità di Poisson.",
		arguments: [
			{
				name: "x",
				description: "è il numero degli eventi"
			},
			{
				name: "media",
				description: "è il valore numerico previsto, un numero positivo"
			},
			{
				name: "cumulativo",
				description: "è un valore logico: utilizzare VERO per la probabilità cumulativa di Poisson; utilizzare FALSO per la funzione probabilità di massa"
			}
		]
	},
	{
		name: "DISTRIB.T",
		description: "Restituisce la distribuzione t di Student.",
		arguments: [
			{
				name: "x",
				description: "è il valore numerico in cui si calcola la distribuzione"
			},
			{
				name: "grad_libertà",
				description: "è un intero che indica il numero di gradi di libertà che caratterizza la distribuzione"
			},
			{
				name: "code",
				description: "specifica il numero di code di distribuzione da restituire: distribuzione a una coda = 1; distribuzione a due code = 2"
			}
		]
	},
	{
		name: "DISTRIB.T.2T",
		description: "Restituisce la distribuzione t di Student a due code.",
		arguments: [
			{
				name: "x",
				description: "è il valore numerico in cui si calcola la distribuzione"
			},
			{
				name: "grad_libertà",
				description: "è un intero che indica il numero di gradi di libertà che caratterizza la distribuzione"
			}
		]
	},
	{
		name: "DISTRIB.T.DS",
		description: "Restituisce la distribuzione t di Student a una coda destra.",
		arguments: [
			{
				name: "x",
				description: "è il valore numerico in cui si calcola la distribuzione"
			},
			{
				name: "grad_libertà",
				description: "è un intero che indica il numero di gradi di libertà che caratterizza la distribuzione"
			}
		]
	},
	{
		name: "DISTRIB.T.N",
		description: "Restituisce la distribuzione t di Student a una coda sinistra.",
		arguments: [
			{
				name: "x",
				description: "è il valore numerico in cui si calcola la distribuzione"
			},
			{
				name: "grad_libertà",
				description: "è un intero che indica il numero di gradi di libertà che caratterizza la distribuzione"
			},
			{
				name: "code",
				description: "è un valore logico: per la funzione di distribuzione cumulativa, utilizzare VERO, per la funzione densità di probabilità utilizzare FALSO"
			}
		]
	},
	{
		name: "DISTRIB.WEIBULL",
		description: "Restituisce la distribuzione di Weibull.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la funzione, un numero non negativo"
			},
			{
				name: "alfa",
				description: "è un parametro per la distribuzione, un numero positivo"
			},
			{
				name: "beta",
				description: "è un parametro per la distribuzione, un numero positivo"
			},
			{
				name: "cumulativo",
				description: "è un valore logico: utilizzare VERO per la funzione distribuzione cumulativa; utilizzare FALSO per la funzione probabilità di massa"
			}
		]
	},
	{
		name: "DISTRIBF",
		description: "Restituisce la distribuzione di probabilità F (coda sinistra ) (grado di diversità) per due set di dati.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la funzione, un numero non negativo"
			},
			{
				name: "grad_libertà1",
				description: "sono i gradi di libertà al numeratore, un numero compreso tra 1 e 10^10, escluso 10^10"
			},
			{
				name: "grad_libertà2",
				description: "sono i gradi di libertà al denominatore, un numero compreso tra 1 e 10^10, escluso 10^10"
			},
			{
				name: "cumulativa",
				description: "è un valore logico per la funzione da restituire: la funzione di distribuzione cumulativa = VERO; la funzione densità di probabilità = FALSO"
			}
		]
	},
	{
		name: "DURATA.P",
		description: "Restituisce il numero di periodi necessari affinché un investimento raggiunga un valore specificato.",
		arguments: [
			{
				name: "tasso_int",
				description: "è il tasso di interesse per periodo."
			},
			{
				name: "val_attuale",
				description: "è il valore attuale dell'investimento"
			},
			{
				name: "val_futuro",
				description: "è il valore futuro desiderato dell'investimento"
			}
		]
	},
	{
		name: "E",
		description: "Restituisce VERO se tutti gli argomenti hanno valore VERO.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logico1",
				description: "sono da 1 a 255 condizioni (valori logici, matrici o riferimenti) da verificare, che possono avere valore VERO o FALSO"
			},
			{
				name: "logico2",
				description: "sono da 1 a 255 condizioni (valori logici, matrici o riferimenti) da verificare, che possono avere valore VERO o FALSO"
			}
		]
	},
	{
		name: "EFFETTIVO",
		description: "Restituisce il tasso di interesse effettivo annuo.",
		arguments: [
			{
				name: "tasso_nominale",
				description: "è il tasso di interesse nominale"
			},
			{
				name: "periodi",
				description: "è il numero dei periodi di capitalizzazione per anno"
			}
		]
	},
	{
		name: "ERR.STD.YX",
		description: "Restituisce l'errore standard del valore previsto per y per ciascun valore di x nella regressione.",
		arguments: [
			{
				name: "y_nota",
				description: "è una matrice o un intervallo di dati dipendenti e può  essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri"
			},
			{
				name: "x_nota",
				description: "è una matrice o un intervallo di dati indipendenti e può  essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "ERRORE.TIPO",
		description: "Restituisce un numero corrispondente a uno dei valori di errore.",
		arguments: [
			{
				name: "errore",
				description: "è il valore di errore di cui trovare il numero di identificazione. Può essere un valore di errore o un riferimento a una cella contenente un valore di errore"
			}
		]
	},
	{
		name: "ESC.PERCENT.RANGO",
		description: "Restituisce il rango di un valore in un set di dati come percentuale del set di dati come percentuale (0..1, estremi esclusi) del set di dati.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o l'intervallo di dati con valori numerici che definiscono la condizione relativa"
			},
			{
				name: "x",
				description: "è il valore di cui conoscere il rango"
			},
			{
				name: "cifre_signific",
				description: "è un valore facoltativo che identifica il numero di cifre significative per la percentuale restituita, tre cifre se omesso (0,xxx %)"
			}
		]
	},
	{
		name: "ESC.PERCENTILE",
		description: "Restituisce il k-esimo dato percentile di valori in un intervallo, dove k è compreso nell'intervallo 0..1, estremi esclusi.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o l'intervallo di dati che definisce la condizione relativa"
			},
			{
				name: "k",
				description: "è il valore percentile, compreso nell'intervallo tra 0 e 1 inclusi"
			}
		]
	},
	{
		name: "ESC.QUARTILE",
		description: "Restituisce il quartile di un set di dati, in base a valori di percentile compresi nell'intervallo 0..1, estremi esclusi.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o intervallo di celle a valori numerici per cui si calcola il valore quartile"
			},
			{
				name: "quarto",
				description: "è un numero: valore minimo = 0; primo quartile = 1; mediana = 2; terzo quartile = 3; valore massimo = 4"
			}
		]
	},
	{
		name: "EXP",
		description: "Restituisce il numero e elevato alla potenza di un dato numero.",
		arguments: [
			{
				name: "num",
				description: "è l'esponente applicato alla base e. La costante e, base dei logaritmi naturali, è uguale a 2,71828182845904."
			}
		]
	},
	{
		name: "FALSO",
		description: "Restituisce il valore logico FALSO.",
		arguments: [
		]
	},
	{
		name: "FATT.DOPPIO",
		description: "Restituisce il fattoriale doppio di un numero.",
		arguments: [
			{
				name: "num",
				description: "è il valore di cui calcolare il fattoriale doppio"
			}
		]
	},
	{
		name: "FATTORIALE",
		description: "Restituisce il fattoriale di un numero, uguale a 1*2*3*...* numero.",
		arguments: [
			{
				name: "num",
				description: "è il numero non negativo di cui si calcola il fattoriale"
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
		name: "FINE.MESE",
		description: "Restituisce il numero seriale dell'ultimo giorno del mese precedente o successivo di un numero specificato di mesi.",
		arguments: [
			{
				name: "data_iniziale",
				description: "è un numero seriale che rappresenta la data iniziale"
			},
			{
				name: "mesi",
				description: "è il numero di mesi precedenti o successivi alla data iniziale"
			}
		]
	},
	{
		name: "FISHER",
		description: "Restituisce la trasformazione di Fisher.",
		arguments: [
			{
				name: "x",
				description: "è il valore per il quale si desidera eseguire la trasformazione, un numero tra -1 e 1 non compresi"
			}
		]
	},
	{
		name: "FISSO",
		description: "Arrotonda un numero al numero di cifre decimali specificato e restituisce il risultato come testo.",
		arguments: [
			{
				name: "num",
				description: "è il numero da arrotondare e convertire in testo"
			},
			{
				name: "decimali",
				description: "è il numero di cifre a destra della virgola decimale. Se viene omesso, Decimali = 2"
			},
			{
				name: "nessun_separatore",
				description: "è un valore logico: non visualizzare i separatori delle migliaia nel testo restituito con il valore VERO; visualizzarli nel testo restituito con il valore FALSO"
			}
		]
	},
	{
		name: "FOGLI",
		description: "Restituisce il numero di fogli in un riferimento.",
		arguments: [
			{
				name: "riferimento",
				description: "è un riferimento per il quale si vuole conoscere il numero di fogli contenuti. Se omesso, viene restituito il numero di fogli nella cartella di lavoro contenente la funzione"
			}
		]
	},
	{
		name: "FOGLIO",
		description: "Restituisce il numero del foglio del riferimento.",
		arguments: [
			{
				name: "valore",
				description: "è il nome di un foglio o un riferimento per il quale si vuole ottenere il numero di foglio. Se omesso, viene restituito il numero del foglio contenente la funzione"
			}
		]
	},
	{
		name: "FRAZIONE.ANNO",
		description: "Restituisce la frazione dell'anno corrispondente al numero dei giorni complessivi compresi tra 'data_iniziale' e 'data_finale'.",
		arguments: [
			{
				name: "data_iniziale",
				description: "è la data iniziale espressa in numero seriale"
			},
			{
				name: "data_finale",
				description: "è la data finale espressa in numero seriale"
			},
			{
				name: "base",
				description: "è il tipo di base da utilizzare per il conteggio dei giorni"
			}
		]
	},
	{
		name: "FREQUENZA",
		description: "Calcola la frequenza con cui si presentano valori compresi in un intervallo e restituisce una matrice verticale di numeri con un elemento in più rispetto a Matrice_classi.",
		arguments: [
			{
				name: "matrice_dati",
				description: "è una matrice o un riferimento ad un insieme di valori di cui si calcola la frequenza. Gli spazi e il testo vengono ignorati."
			},
			{
				name: "matrice_classi",
				description: "è una matrice o un riferimento agli intervalli in cui raggruppare i valori contenuti in matrice_dati"
			}
		]
	},
	{
		name: "FUNZ.ERRORE",
		description: "Restituisce la funzione di errore.",
		arguments: [
			{
				name: "limite_inf",
				description: "è il limite inferiore di integrazione per FUNZ.ERRORE"
			},
			{
				name: "limite_sup",
				description: "è il limite superiore di integrazione per FUNZ.ERRORE"
			}
		]
	},
	{
		name: "FUNZ.ERRORE.COMP",
		description: "Restituisce la funzione di errore complementare.",
		arguments: [
			{
				name: "x",
				description: "è il limite inferiore di integrazione per FUNZ.ERRORE"
			}
		]
	},
	{
		name: "FUNZ.ERRORE.COMP.PRECISA",
		description: "Restituisce la funzione di errore complementare.",
		arguments: [
			{
				name: "X",
				description: "è il limite inferiore di integrazione per FUNZ.ERRORE.COMP.PRECISA"
			}
		]
	},
	{
		name: "FUNZ.ERRORE.PRECISA",
		description: "Restituisce la funzione di errore.",
		arguments: [
			{
				name: "X",
				description: "è il limite inferiore di integrazione per FUNZ.ERRORE.PRECISA"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Restituisce il valore della funzione GAMMA.",
		arguments: [
			{
				name: "x",
				description: "è il valore per il quale si calcola GAMMA"
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
		name: "GIORNI",
		description: "Restituisce il numero di giorni che intercorre tra due date.",
		arguments: [
			{
				name: "data_fine",
				description: "data_inizio e data_fine sono le due date per le quali si vuole conoscere il numero di giorni che intercorre tra di esse"
			},
			{
				name: "data_inizio",
				description: "data_inizio e data_fine sono le due date per le quali si vuole conoscere il numero di giorni che intercorre tra di esse"
			}
		]
	},
	{
		name: "GIORNI.CED.INIZ.LIQ",
		description: "Calcola il numero dei giorni cha vanno dalla data di inizio del periodo della cedola alla liquidazione.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del titolo espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del titolo espressa come numero seriale"
			},
			{
				name: "num_rate",
				description: "è il numero di pagamenti per anno"
			},
			{
				name: "base",
				description: "è il tipo di base da utilizzare per il conteggio dei giorni"
			}
		]
	},
	{
		name: "GIORNI.LAVORATIVI.TOT",
		description: "Restituisce il numero dei giorni lavorativi compresi tra due date.",
		arguments: [
			{
				name: "data_iniziale",
				description: "è la data iniziale espressa come numero seriale."
			},
			{
				name: "data_finale",
				description: "è la data finale espressa come numero seriale"
			},
			{
				name: "vacanze",
				description: "è un insieme facoltativo di una o più date, espresse come numero seriale, che rappresentano i giorni da escludere dal calendario lavorativo, come le feste nazionali, locali e i permessi retribuiti"
			}
		]
	},
	{
		name: "GIORNI.LAVORATIVI.TOT.INTL",
		description: "Restituisce il numero dei giorni lavorativi compresi tra due date con parametri di giorni festivi personalizzati.",
		arguments: [
			{
				name: "data_iniziale",
				description: "è la data iniziale espressa come numero seriale."
			},
			{
				name: "data_finale",
				description: "è la data finale espressa come numero seriale"
			},
			{
				name: "festivi",
				description: "è un numero o una stringa che specifica i giorni festivi"
			},
			{
				name: "vacanze",
				description: "è un set facoltativo di una o più date, espresse come numero seriale, che rappresentano i giorni da escludere dal calendario lavorativo, come le feste nazionali, locali e i permessi retribuiti"
			}
		]
	},
	{
		name: "GIORNO",
		description: "Restituisce il giorno del mese, un numero compreso tra 1 e 31.",
		arguments: [
			{
				name: "num_seriale",
				description: "è un numero nel codice data-ora utilizzato da Spreadsheet"
			}
		]
	},
	{
		name: "GIORNO.LAVORATIVO",
		description: "Restituisce la data, espressa come numero seriale, del giorno precedente o successivo a un numero specificato di giorni lavorativi.",
		arguments: [
			{
				name: "data_iniziale",
				description: "è la data iniziale espressa come numero seriale"
			},
			{
				name: "giorni",
				description: "è il numero dei giorni lavorativi precedenti o successivi a 'data_iniziale'"
			},
			{
				name: "vacanze",
				description: "è una matrice facoltativa con una o più date, espresse come numero seriale, che rappresentano i giorni da escludere dal calendario lavorativo, come le feste nazionali, locali e i permessi retribuiti"
			}
		]
	},
	{
		name: "GIORNO.LAVORATIVO.INTL",
		description: "Restituisce la data, espressa come numero seriale, del giorno precedente o successivo a un numero specificato di giorni lavorativi con parametri di giorni festivi personalizzati.",
		arguments: [
			{
				name: "data_iniziale",
				description: "è la data iniziale espressa come numero seriale"
			},
			{
				name: "giorni",
				description: "è il numero dei giorni lavorativi precedenti o successivi a 'data_iniziale'"
			},
			{
				name: "festivi",
				description: "è un numero o una stringa che specifica i giorni festivi"
			},
			{
				name: "vacanze",
				description: "è una matrice facoltativa con una o più date, espresse come numero seriale, che rappresentano i giorni da escludere dal calendario lavorativo, come le feste nazionali, locali e i permessi retribuiti"
			}
		]
	},
	{
		name: "GIORNO.SETTIMANA",
		description: "Restituisce un numero compreso tra 1 e 7 che identifica il giorno della settimana di una data.",
		arguments: [
			{
				name: "num_seriale",
				description: "è un numero che rappresenta una data"
			},
			{
				name: "tipo_restituito",
				description: "è un numero: per domenica=1 fino a sabato=7 utilizzare 1; per lunedì=1 fino a domenica=7 utilizzare 2; per lunedì=0 fino a domenica=6 utilizzare 3"
			}
		]
	},
	{
		name: "GIORNO360",
		description: "Restituisce il numero di giorni compresi tra due date sulla base di un anno di 360 giorni (dodici mesi di 30 giorni).",
		arguments: [
			{
				name: "data_iniziale",
				description: "data_iniziale e data_finale sono le due date che delimitano il periodo di cui si desidera conoscere il numero di giorni"
			},
			{
				name: "data_finale",
				description: "data_iniziale e data_finale sono le due date che delimitano il periodo di cui si desidera conoscere il numero di giorni"
			},
			{
				name: "metodo",
				description: "è un valore logico che specifica il metodo di calcolo: U.S. (NASD) = FALSO o omesso; Europeo = VERO."
			}
		]
	},
	{
		name: "GRADI",
		description: "Converte i radianti in gradi.",
		arguments: [
			{
				name: "angolo",
				description: "è l'angolo da convertire, espresso in radianti"
			}
		]
	},
	{
		name: "GRANDE",
		description: "Restituisce il k-esimo valore più grande in un set di dati. Ad esempio, il quinto numero più grande.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o intervallo di dati di cui determinare il k-esimo valore più grande"
			},
			{
				name: "k",
				description: "è la posizione, partendo dal più grande, nella matrice o intervallo di celle del valore da restituire"
			}
		]
	},
	{
		name: "HEX.BINARIO",
		description: "Converte un numero esadecimale in binario.",
		arguments: [
			{
				name: "num",
				description: "è il numero esadecimale da convertire"
			},
			{
				name: "cifre",
				description: "è il numero di caratteri da utilizzare"
			}
		]
	},
	{
		name: "HEX.DECIMALE",
		description: "Converte un numero esadecimale in decimale.",
		arguments: [
			{
				name: "num",
				description: "è il numero esadecimale da convertire"
			}
		]
	},
	{
		name: "HEX.OCT",
		description: "Converte un numero esadecimale in ottale.",
		arguments: [
			{
				name: "num",
				description: "è il numero esadecimale da convertire"
			},
			{
				name: "cifre",
				description: "è il numero di caratteri da utilizzare"
			}
		]
	},
	{
		name: "IDENTICO",
		description: "Controlla due stringhe di testo e restituisce il valore VERO se sono identiche e FALSO in caso contrario. Distingue tra maiuscole e minuscole.",
		arguments: [
			{
				name: "testo1",
				description: "è la prima stringa di testo"
			},
			{
				name: "testo2",
				description: "è la seconda stringa di testo"
			}
		]
	},
	{
		name: "INC.PERCENT.RANGO",
		description: "Restituisce il rango di un valore in un set di dati come percentuale del set di dati come percentuale (0..1, estremi inclusi) del set di dati.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o l'intervallo di dati con valori numerici che definiscono la condizione relativa"
			},
			{
				name: "x",
				description: "è il valore di cui conoscere il rango"
			},
			{
				name: "cifre_signific",
				description: "è un valore facoltativo che identifica il numero di cifre significative per la percentuale restituita, tre cifre se omesso (0,xxx %)"
			}
		]
	},
	{
		name: "INC.PERCENTILE",
		description: "Restituisce il k-esimo dato percentile di valori in un intervallo, dove k è compreso nell'intervallo 0..1, estremi inclusi.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o l'intervallo di dati che definisce la condizione relativa"
			},
			{
				name: "k",
				description: "è il valore percentile, compreso nell'intervallo tra 0 e 1 inclusi"
			}
		]
	},
	{
		name: "INC.QUARTILE",
		description: "Restituisce il quartile di un set di dati, in base a valori di percentile compresi nell'intervallo 0..1, estremi inclusi.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o intervallo di celle a valori numerici per cui si calcola il valore quartile"
			},
			{
				name: "quarto",
				description: "è un numero: valore minimo = 0; primo quartile = 1; mediana = 2; terzo quartile = 3; valore massimo = 4"
			}
		]
	},
	{
		name: "INDICE",
		description: "Restituisce un valore o un riferimento della cella all'intersezione di una particolare riga e colonna in un dato intervallo.",
		arguments: [
			{
				name: "matrice",
				description: "è un intervallo di celle o una costante di matrice."
			},
			{
				name: "riga",
				description: "seleziona la riga nella Matrice o nel Riferimento dal quale restituire un valore. Se omesso, sarà necessario specificare Col."
			},
			{
				name: "col",
				description: "seleziona la colonna nella Matrice o nel Riferimento dal quale restituire un valore. Se omesso, sarà necessario specificare Riga."
			}
		]
	},
	{
		name: "INDIRETTO",
		description: "Restituisce un riferimento indicato da una stringa di testo.",
		arguments: [
			{
				name: "rif",
				description: "è un riferimento a una cella che contiene un riferimento di tipo A1 o di tipo R1C1, un nome definito come riferimento o un riferimento a una cella come stringa di testo"
			},
			{
				name: "a1",
				description: "è un valore logico che specifica il tipo di riferimento contenuto in rif: tipo R1C1 = FALSO; tipo A1 = VERO o omesso"
			}
		]
	},
	{
		name: "INDIRIZZO",
		description: "Dati il numero di riga e di colonna, crea un riferimento di cella in formato testo.",
		arguments: [
			{
				name: "riga",
				description: "è il numero di riga da utilizzare nel riferimento di cella; ad esempio, Riga = 1 per la riga 1"
			},
			{
				name: "col",
				description: "è il numero di colonna da utilizzare nel riferimento di cella; ad esempio, Col = 4 per la colonna D"
			},
			{
				name: "ass",
				description: "specifica il tipo di riferimento: assoluto = 1; riga assoluta/colonna relativa = 2;  riga relativa/colonna assoluta = 3; relativo = 4"
			},
			{
				name: "a1",
				description: "è un valore logico che specifica lo stile di riferimento: stile A1 = 1 o VERO; stile R1C1 = 0 o FALSO"
			},
			{
				name: "foglio",
				description: "è il testo indicante il nome del foglio di lavoro da utilizzare come riferimento esterno"
			}
		]
	},
	{
		name: "INFO.DATI.TAB.PIVOT",
		description: "Estrae i dati memorizzati in una tabella pivot.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "campo_dati",
				description: "è il nome del campo dati dal quale estrarre i dati"
			},
			{
				name: "tabella_pivot",
				description: "è un riferimento a una cella o intervallo di celle nella tabella pivot che contiene i dati da recuperare"
			},
			{
				name: "campo",
				description: "è il campo a cui fare riferimento"
			},
			{
				name: "elemento",
				description: "è l'elemento del campo a cui fare riferimento"
			}
		]
	},
	{
		name: "INT",
		description: "Arrotonda un numero per difetto all'intero più vicino.",
		arguments: [
			{
				name: "num",
				description: "è il numero reale da arrotondare per difetto a un intero"
			}
		]
	},
	{
		name: "INT.CUMUL",
		description: "Calcola l'interesse cumulativo pagato tra due periodi.",
		arguments: [
			{
				name: "tasso_int",
				description: "è il tasso di interesse"
			},
			{
				name: "periodi",
				description: "è il numero totale dei periodi di pagamento"
			},
			{
				name: "val_attuale",
				description: "è il valore attuale"
			},
			{
				name: "iniz_per",
				description: "è il primo periodo nel calcolo"
			},
			{
				name: "fine_per",
				description: "è l'ultimo periodo nel calcolo"
			},
			{
				name: "tipo",
				description: "è la scadenza per il pagamento"
			}
		]
	},
	{
		name: "INT.MATURATO.SCAD",
		description: "Restituisce l'interesse maturato per un titolo i cui interessi vengono pagati alla scadenza.",
		arguments: [
			{
				name: "emiss",
				description: "è la data di emissione del titolo espressa come numero seriale"
			},
			{
				name: "liquid",
				description: "è la data di scadenza del titolo espressa come numero seriale"
			},
			{
				name: "tasso_int",
				description: "è il tasso di interesse nominale annuo del titolo"
			},
			{
				name: "val_nom",
				description: "è il valore nominale del titolo"
			},
			{
				name: "base",
				description: "è il tipo di base da usare per il conteggio dei giorni"
			}
		]
	},
	{
		name: "INTERCETTA",
		description: "Calcola il punto di intersezione della retta con l'asse y tracciando una regressione lineare fra le coordinate note.",
		arguments: [
			{
				name: "y_nota",
				description: "è l'insieme dipendente di osservazioni o dati e può  essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri"
			},
			{
				name: "x_nota",
				description: "è l'insieme indipendente di osservazioni o dati e può  essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "INTERESSE.RATA",
		description: "Restituisce il tasso di interesse del prestito a tasso fisso.",
		arguments: [
			{
				name: "tasso_int",
				description: "tasso di interesse per periodo. Ad esempio, usare 6%/4 per pagamenti trimestrali al 6%."
			},
			{
				name: "periodo",
				description: "periodo per cui trovare l'interesse"
			},
			{
				name: "periodi",
				description: "numero dei periodi di pagamento in un'annualità"
			},
			{
				name: "val_attuale",
				description: "somma forfettaria pari al valore attuale di una serie di pagamenti futuri"
			}
		]
	},
	{
		name: "INTERESSI",
		description: "Restituisce l'ammontare degli interessi relativi ad un investimento di una certa durata di pagamenti periodici costanti e un tasso di interesse costante.",
		arguments: [
			{
				name: "tasso_int",
				description: "è il tasso di interesse per il periodo. Ad esempio, utilizzare 6%/4 per pagamenti trimestrali al tasso del 6%."
			},
			{
				name: "periodo",
				description: "è il periodo, compreso tra 1 e n periodi, per il quale si calcolano gli interessi."
			},
			{
				name: "periodi",
				description: "è il numero totale dei periodi di pagamento in un investimento"
			},
			{
				name: "val_attuale",
				description: "è il valore attuale o la somma forfettaria pari al valore attuale di una serie di pagamenti futuri"
			},
			{
				name: "val_futuro",
				description: "è il valore futuro o il saldo in contanti da conseguire dopo l'ultimo pagamento. Se omesso, Fv = 0"
			},
			{
				name: "tipo",
				description: "è un valore corrispondente al momento del pagamento: all'inizio del periodo = 1; alla fine del periodo = 0 oppure omesso"
			}
		]
	},
	{
		name: "INTERVALLO",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "INTERVALLO.DISTRIB.BINOM.N.",
		description: "Restituisce la probabilità di un risultato di prova usando una distribuzione binomiale.",
		arguments: [
			{
				name: "prove",
				description: "è il numero di prove indipendenti"
			},
			{
				name: "probabilità_s",
				description: "è la probabilità di successo di ogni prova"
			},
			{
				name: "num_s",
				description: "è il numero di successi nelle prove"
			},
			{
				name: "num_s2",
				description: "se specificato, questa funzione restituisce la probabilità che il numero di prove di successo sia compreso tra num_s e num_s2"
			}
		]
	},
	{
		name: "INV.BETA",
		description: "Restituisce l'inversa della funzione densità di probabilità cumulativa beta (DISTRIB.BETA).",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione beta"
			},
			{
				name: "alfa",
				description: "è un parametro per la distribuzione e deve essere maggiore di 0"
			},
			{
				name: "beta",
				description: "è un parametro per la distribuzione e deve essere maggiore di 0"
			},
			{
				name: "A",
				description: "è un valore facoltativo per l'estremo inferiore dell'intervallo di x. Se omesso, A = 0"
			},
			{
				name: "B",
				description: "è un valore facoltativo per l'estremo superiore dell'intervallo di x. Se omesso, B = 1"
			}
		]
	},
	{
		name: "INV.BETA.N",
		description: "Restituisce l'inversa della funzione densità di probabilità cumulativa beta (DISTRIB.BETA.N).",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione beta"
			},
			{
				name: "alfa",
				description: "è un parametro per la distribuzione e deve essere maggiore di 0"
			},
			{
				name: "beta",
				description: "è un parametro per la distribuzione e deve essere maggiore di 0"
			},
			{
				name: "A",
				description: "è un valore facoltativo per l'estremo inferiore dell'intervallo di x. Se omesso, A = 0"
			},
			{
				name: "B",
				description: "è un valore facoltativo per l'estremo superiore dell'intervallo di x. Se omesso, B = 1"
			}
		]
	},
	{
		name: "INV.BINOM",
		description: "Restituisce il più piccolo valore per il quale la distribuzione cumulativa binomiale risulta maggiore o uguale ad un valore di criterio.",
		arguments: [
			{
				name: "prove",
				description: "è il numero delle prove di Bernoulli"
			},
			{
				name: "probabilità_s",
				description: "è la probabilità di un successo ad ogni prova, un numero tra 0 e 1 inclusi"
			},
			{
				name: "alfa",
				description: "è il valore di criterio, un numero tra 0 e 1 inclusi"
			}
		]
	},
	{
		name: "INV.CHI",
		description: "Restituisce l'inversa della probabilità a una coda destra per la distribuzione del chi quadrato.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione del chi quadrato, un valore tra 0 e 1 inclusi"
			},
			{
				name: "grado_libertà",
				description: "è il numero di gradi di libertà, un numero compreso tra 1 e 10^10, escluso 10^10"
			}
		]
	},
	{
		name: "INV.CHI.QUAD",
		description: "Restituisce l'inversa della probabilità a una coda sinistra della distribuzione del chi quadrato.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione del chi quadrato, un valore tra 0 e 1 inclusi"
			},
			{
				name: "grado_libertà",
				description: "è il numero di gradi di libertà, un numero compreso tra 1 e 10^10, escluso 10^10"
			}
		]
	},
	{
		name: "INV.CHI.QUAD.DS",
		description: "Restituisce l'inversa della probabilità a una coda destra della distribuzione del chi quadrato.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione del chi quadrato, un valore tra 0 e 1 inclusi"
			},
			{
				name: "grado_libertà",
				description: "è il numero di gradi di libertà, un numero compreso tra 1 e 10^10, escluso 10^10"
			}
		]
	},
	{
		name: "INV.F",
		description: "Restituisce l'inversa della distribuzione di probabilità F (coda destra): se p = FDIST(x,...), allora FINV(p,...) = x.",
		arguments: [
			{
				name: "probabilità",
				description: "è una probabilità associata alla distribuzione cumulativa F, un numero tra 0 e 1 inclusi."
			},
			{
				name: "grado_libertà1",
				description: "sono i gradi di libertà al numeratore, un numero compreso tra 1 e 10^10, escluso 10^10"
			},
			{
				name: "grado_libertà2",
				description: "sono i gradi di libertà al denominatore, un numero compreso tra 1 e 10^10, escluso 10^10"
			}
		]
	},
	{
		name: "INV.F.DS",
		description: "Restituisce l'inversa della distribuzione di probabilità F (coda destra): se p = DISTRIB.F.DS(x,...), allora INV.F.DS(p,...) = x.",
		arguments: [
			{
				name: "probabilità",
				description: "è una probabilità associata alla distribuzione cumulativa F, un numero tra 0 e 1 inclusi."
			},
			{
				name: "grado_libertà1",
				description: "sono i gradi di libertà al numeratore, un numero compreso tra 1 e 10^10, escluso 10^10"
			},
			{
				name: "grado_libertà2",
				description: "sono i gradi di libertà al denominatore, un numero compreso tra 1 e 10^10, escluso 10^10"
			}
		]
	},
	{
		name: "INV.FISHER",
		description: "Restituisce l'inversa della trasformazione di Fisher: se y = FISHER(x), allora INV.FISHER(y) = x.",
		arguments: [
			{
				name: "y",
				description: "è il valore per il quale si desidera eseguire l'inversa della trasformazione"
			}
		]
	},
	{
		name: "INV.GAMMA",
		description: "Restituisce l'inversa della distribuzione cumulativa gamma: se p = DISTRIB.GAMMA(x,...), allora INV.GAMMA(p,...) = x.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione gamma, un numero tra 0 e 1 inclusi"
			},
			{
				name: "alfa",
				description: "è un parametro per la distribuzione, un numero positivo"
			},
			{
				name: "beta",
				description: "è un parametro per la distribuzione, un numero positivo. Se beta = 1, INV.GAMMA restituisce la distribuzione gamma standard."
			}
		]
	},
	{
		name: "INV.GAMMA.N",
		description: "Restituisce l'inversa della distribuzione cumulativa gamma: se p = DISTRIB.GAMMA.N(x,...), allora INV.GAMMA.N(p,...) = x.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione gamma, un numero tra 0 e 1 inclusi"
			},
			{
				name: "alfa",
				description: "è un parametro per la distribuzione, un numero positivo"
			},
			{
				name: "beta",
				description: "è un parametro per la distribuzione, un numero positivo. Se beta = 1, INV.GAMMA.N restituisce l'inversa della distribuzione gamma standard."
			}
		]
	},
	{
		name: "INV.LOGNORM",
		description: "Restituisce l'inversa della distribuzione lognormale di x, in cui ln(x) è distribuito normalmente con i parametri Media e Dev_standard.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione lognormale, un numero tra 0 e 1 inclusi"
			},
			{
				name: "media",
				description: "è la media di ln(x)"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard di ln(x), un numero positivo"
			}
		]
	},
	{
		name: "INV.LOGNORM.N",
		description: "Restituisce l'inversa della distribuzione lognormale di x, in cui ln(x) è distribuito normalmente con i parametri Media e Dev_standard.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione lognormale, un numero tra 0 e 1 inclusi"
			},
			{
				name: "media",
				description: "è la media di ln(x)"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard di ln(x), un numero positivo"
			}
		]
	},
	{
		name: "INV.NORM",
		description: "Restituisce l'inversa della distribuzione normale cumulativa per la media e la deviazione standard specificate.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità corrispondente alla distribuzione normale, un numero tra 0 e 1 inclusi"
			},
			{
				name: "media",
				description: "è la media aritmetica della distribuzione"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard della distribuzione, un numero positivo"
			}
		]
	},
	{
		name: "INV.NORM.N",
		description: "Restituisce l'inversa della distribuzione normale cumulativa per la media e la deviazione standard specificate.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità corrispondente alla distribuzione normale, un numero tra 0 e 1 inclusi"
			},
			{
				name: "media",
				description: "è la media aritmetica della distribuzione"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard della distribuzione, un numero positivo"
			}
		]
	},
	{
		name: "INV.NORM.S",
		description: "Restituisce l'inversa della distribuzione normale standard cumulativa (ha media pari a zero e deviazione standard pari a 1).",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità corrispondente alla distribuzione normale, un numero tra 0 e 1 inclusi"
			}
		]
	},
	{
		name: "INV.NORM.ST",
		description: "Restituisce l'inversa della distribuzione normale standard cumulativa (ha media pari a zero e deviazione standard pari a 1).",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità corrispondente alla distribuzione normale, un numero tra 0 e 1 inclusi"
			}
		]
	},
	{
		name: "INV.T",
		description: "Restituisce l'inversa della distribuzione t di Student.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione t di Student a due code, un numero tra 0 e 1 inclusi"
			},
			{
				name: "grado_libertà",
				description: "è l'intero positivo che indica il numero di gradi di libertà della distribuzione"
			}
		]
	},
	{
		name: "INV.T.2T",
		description: "Restituisce l'inversa della distribuzione t di Student a due code.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione t di Student a due code, un numero tra 0 e 1 inclusi"
			},
			{
				name: "grado_libertà",
				description: "è l'intero positivo che indica il numero di gradi di libertà della distribuzione"
			}
		]
	},
	{
		name: "INVF",
		description: "Restituisce l'inversa della distribuzione di probabilità F (coda sinistra): se p = DISTRIB.F(x,...), allora INVF(p,...) = x.",
		arguments: [
			{
				name: "probabilità",
				description: "è una probabilità associata alla distribuzione cumulativa F, un numero tra 0 e 1 inclusi."
			},
			{
				name: "grado_libertà1",
				description: "sono i gradi di libertà al numeratore, un numero compreso tra 1 e 10^10, escluso 10^10"
			},
			{
				name: "grado_libertà2",
				description: "sono i gradi di libertà al denominatore, un numero compreso tra 1 e 10^10, escluso 10^10"
			}
		]
	},
	{
		name: "INVT",
		description: "Restituisce l'inversa della distribuzione t di Student a una coda sinistra.",
		arguments: [
			{
				name: "probabilità",
				description: "è la probabilità associata alla distribuzione t di Student a due code, un numero tra 0 e 1 inclusi"
			},
			{
				name: "grado_libertà",
				description: "è l'intero positivo che indica il numero di gradi di libertà della distribuzione"
			}
		]
	},
	{
		name: "ISO.ARROTONDA.ECCESSO",
		description: "Arrotonda un numero per eccesso all'intero più vicino o al multiplo più vicino a peso.",
		arguments: [
			{
				name: "num",
				description: "è il valore da arrotondare"
			},
			{
				name: "peso",
				description: "è il multiplo facoltativo per il quale si desidera arrotondare"
			}
		]
	},
	{
		name: "LIBERA",
		description: "Rimuove dal testo tutti i caratteri che non possono essere stampati.",
		arguments: [
			{
				name: "testo",
				description: "è una qualsiasi informazione del foglio di lavoro dalla quale rimuovere i caratteri che non possono essere stampati"
			}
		]
	},
	{
		name: "LN",
		description: "Restituisce il logaritmo naturale di un numero.",
		arguments: [
			{
				name: "num",
				description: "è il numero reale positivo di cui si calcola il logaritmo naturale"
			}
		]
	},
	{
		name: "LN.GAMMA",
		description: "Restituisce il logaritmo naturale della funzione gamma.",
		arguments: [
			{
				name: "x",
				description: "è il valore per il quale si calcola INV.GAMMA, un numero positivo"
			}
		]
	},
	{
		name: "LN.GAMMA.PRECISA",
		description: "Restituisce il logaritmo naturale della funzione gamma.",
		arguments: [
			{
				name: "x",
				description: "è il valore per il quale si calcola LN.GAMMA.PRECISA, un numero positivo"
			}
		]
	},
	{
		name: "LOG",
		description: "Restituisce il logaritmo di un numero nella base specificata.",
		arguments: [
			{
				name: "num",
				description: "è il numero reale positivo di cui si calcola il logaritmo"
			},
			{
				name: "base",
				description: "è la base del logaritmo; 10 se omesso"
			}
		]
	},
	{
		name: "LOG10",
		description: "Restituisce il logaritmo in base 10 di un numero.",
		arguments: [
			{
				name: "num",
				description: "è il numero reale positivo di cui si calcola il logaritmo in base 10"
			}
		]
	},
	{
		name: "LUNGHEZZA",
		description: "Restituisce il numero di caratteri in una stringa di testo.",
		arguments: [
			{
				name: "testo",
				description: "è il testo di cui conoscere la lunghezza. Gli spazi sono contati come caratteri."
			}
		]
	},
	{
		name: "MAIUSC",
		description: "Converte una stringa di testo in maiuscolo.",
		arguments: [
			{
				name: "testo",
				description: "è il testo da convertire in maiuscolo, un riferimento o una stringa di testo"
			}
		]
	},
	{
		name: "MAIUSC.INIZ",
		description: "Converte in maiuscolo la prima lettera di ciascuna parola in una stringa di testo e converte le altre lettere in minuscolo.",
		arguments: [
			{
				name: "testo",
				description: "è del testo racchiuso tra virgolette, una formula che restituisce del testo o un riferimento ad una cella contenente del testo da convertire parzialmente in maiuscolo"
			}
		]
	},
	{
		name: "MATR.DETERM",
		description: "Restituisce il determinante di una matrice.",
		arguments: [
			{
				name: "matrice",
				description: "è una matrice numerica quadrata, un intervallo di celle o una costante di matrice"
			}
		]
	},
	{
		name: "MATR.INVERSA",
		description: "Restituisce l'inversa di una matrice.",
		arguments: [
			{
				name: "matrice",
				description: "è una matrice numerica quadrata, un intervallo di celle o una costante di matrice"
			}
		]
	},
	{
		name: "MATR.PRODOTTO",
		description: "Restituisce il prodotto di due matrici, una matrice avente un numero di righe pari a Matrice1 e un numero di colonne pari a Matrice2.",
		arguments: [
			{
				name: "matrice1",
				description: "è la prima matrice di numeri da moltiplicare e deve avere un numero di colonne pari alle righe di Matrice2"
			},
			{
				name: "matrice2",
				description: "è la prima matrice di numeri da moltiplicare e deve avere un numero di colonne pari alle righe di Matrice2"
			}
		]
	},
	{
		name: "MATR.SOMMA.PRODOTTO",
		description: "Moltiplica elementi numerici corrispondenti in matrici o intervalli di dati e restituisce la somma dei prodotti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matrice1",
				description: "sono da 2 a 255 matrici di cui moltiplicare e quindi sommare gli elementi. Le matrici devono avere le stesse dimensioni."
			},
			{
				name: "matrice2",
				description: "sono da 2 a 255 matrici di cui moltiplicare e quindi sommare gli elementi. Le matrici devono avere le stesse dimensioni."
			},
			{
				name: "matrice3",
				description: "sono da 2 a 255 matrici di cui moltiplicare e quindi sommare gli elementi. Le matrici devono avere le stesse dimensioni."
			}
		]
	},
	{
		name: "MATR.TRASPOSTA",
		description: "Converte un intervallo verticale in un un intervallo orizzontale o viceversa.",
		arguments: [
			{
				name: "matrice",
				description: "è un intervallo di celle in un foglio di lavoro o una matrice di valori da trasporre"
			}
		]
	},
	{
		name: "MATR.UNIT",
		description: "Restituisce la matrice unitaria per la dimensione specificata.",
		arguments: [
			{
				name: "dimensione",
				description: "è un intero che specifica la dimensione della matrice unitaria da restituire"
			}
		]
	},
	{
		name: "MAX",
		description: "Restituisce il valore massimo di un insieme di valori. Ignora i valori logici e il testo.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, celle vuote, valori logici o numeri in forma di testo di cui trovare il valore massimo"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, celle vuote, valori logici o numeri in forma di testo di cui trovare il valore massimo"
			}
		]
	},
	{
		name: "MAX.VALORI",
		description: "Restituisce il valore massimo di un insieme di valori. Non ignora i valori logici e il testo.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "val1",
				description: "sono da 1 a 255 numeri, celle vuote, valori logici o numeri in forma di testo per cui determinare il valore massimo"
			},
			{
				name: "val2",
				description: "sono da 1 a 255 numeri, celle vuote, valori logici o numeri in forma di testo per cui determinare il valore massimo"
			}
		]
	},
	{
		name: "MCD",
		description: "Restituisce il massimo comun divisore.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 valori"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 valori"
			}
		]
	},
	{
		name: "MCM",
		description: "Restituisce il minimo comune multiplo.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 valori per cui calcolare il minimo comune multiplo"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 valori per cui calcolare il minimo comune multiplo"
			}
		]
	},
	{
		name: "MEDIA",
		description: "Restituisce la media aritmetica degli argomenti (numeri, nomi o riferimenti contenenti numeri).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti numerici di cui calcolare la media"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti numerici di cui calcolare la media"
			}
		]
	},
	{
		name: "MEDIA.ARMONICA",
		description: "Calcola la media armonica (il reciproco della media aritmetica dei reciproci) di un set di dati costituiti da numeri positivi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la media armonica"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la media armonica"
			}
		]
	},
	{
		name: "MEDIA.DEV",
		description: "Restituisce la media delle deviazioni assolute delle coordinate rispetto alla media di queste ultime. Gli argomenti possono essere numeri o nomi, matrici o riferimenti contenenti numeri.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti di cui si calcola la media delle deviazioni assolute"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti di cui si calcola la media delle deviazioni assolute"
			}
		]
	},
	{
		name: "MEDIA.GEOMETRICA",
		description: "Restituisce la media geometrica di una matrice o di un intervallo di dati numerici positivi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la media"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la media"
			}
		]
	},
	{
		name: "MEDIA.PIÙ.SE",
		description: "Determina la media aritmetica per le celle specificate da un determinato insieme di condizioni o criteri.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "int_media",
				description: "sono le celle effettive da utilizzare per determinare la media"
			},
			{
				name: "int_criteri",
				description: "è l'intervallo di celle da valutare per la condizione specificata"
			},
			{
				name: "criterio",
				description: "è il criterio o la condizione, in forma di numero, espressione o testo, che definisce le celle da utilizzare per determinare la media"
			}
		]
	},
	{
		name: "MEDIA.SE",
		description: "Determina la media aritmetica per le celle specificate da una determinata condizione o criterio.",
		arguments: [
			{
				name: "intervallo",
				description: "è l'intervallo di celle da valutare"
			},
			{
				name: "criterio",
				description: "è il criterio o la condizione, in forma di numero, espressione o testo, che definisce le celle da utilizzare per determinare la media"
			},
			{
				name: "int_media",
				description: "sono le celle effettive da utilizzare per determinare la media; se viene omesso, verranno utilizzate le celle nell'intervallo"
			}
		]
	},
	{
		name: "MEDIA.TRONCATA",
		description: "Restituisce la media della parte interna di un set di valori di dati.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o intervallo di valori da troncare e di cui si calcola la media"
			},
			{
				name: "percento",
				description: "è il numero di dati frazionario da escludere dall'inizio e dalla fine del set di dati"
			}
		]
	},
	{
		name: "MEDIA.VALORI",
		description: "Restituisce la media aritmetica degli argomenti. Gli argomenti costituiti da testo o dal valore FALSO vengono valutati come 0, quelli costituiti dal valore VERO come 1. Gli argomenti possono essere numeri, nomi, matrici o riferimenti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "val1",
				description: "sono da 1 a 255 argomenti di cui calcolare la media"
			},
			{
				name: "val2",
				description: "sono da 1 a 255 argomenti di cui calcolare la media"
			}
		]
	},
	{
		name: "MEDIANA",
		description: "Restituisce la mediana, ovvero il valore centrale, di un insieme ordinato di numeri specificato.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui si calcola la mediana"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui si calcola la mediana"
			}
		]
	},
	{
		name: "MESE",
		description: "Restituisce il mese, un numero compreso tra 1 (gennaio) e 12 (dicembre).",
		arguments: [
			{
				name: "num_seriale",
				description: "è un numero nel codice data-ora utilizzato da Spreadsheet"
			}
		]
	},
	{
		name: "MIN",
		description: "Restituisce il valore minimo di un insieme di valori. Ignora i valori logici e il testo.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, celle vuote, valori logici o numeri in forma di testo di cui trovare il valore minimo"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, celle vuote, valori logici o numeri in forma di testo di cui trovare il valore minimo"
			}
		]
	},
	{
		name: "MIN.VALORI",
		description: "Restituisce il valore minimo di un insieme di valori. Non ignora i valori logici e il testo.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "val1",
				description: "sono da 1 a 255 numeri, celle vuote, valori logici o numeri in forma di testo per cui determinare il valore minimo"
			},
			{
				name: "val2",
				description: "sono da 1 a 255 numeri, celle vuote, valori logici o numeri in forma di testo per cui determinare il valore minimo"
			}
		]
	},
	{
		name: "MINUSC",
		description: "Converte le lettere maiuscole in una stringa di testo in lettere minuscole.",
		arguments: [
			{
				name: "testo",
				description: "è il testo da convertire in minuscolo. I caratteri che non sono lettere verranno lasciati invariati."
			}
		]
	},
	{
		name: "MINUTO",
		description: "Restituisce il minuto, un numero compreso tra 0 e 59.",
		arguments: [
			{
				name: "num_seriale",
				description: "è un numero nel codice data-ora utilizzato da Spreadsheet o testo in formato ora, quale 16.48.00"
			}
		]
	},
	{
		name: "MODA",
		description: "Restituisce il valore più ricorrente in una matrice o intervallo di dati.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la moda"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la moda"
			}
		]
	},
	{
		name: "MODA.MULT",
		description: "Restituisce il valore più ricorrente in una matrice o intervallo di dati. Per una matrice orizzontale, utilizzare MATR.TRASPOSTA(MODA.MULT(num1,num2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la moda"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la moda"
			}
		]
	},
	{
		name: "MODA.SNGL",
		description: "Restituisce il valore più ricorrente in una matrice o intervallo di dati.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la moda"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti contenenti numeri di cui calcolare la moda"
			}
		]
	},
	{
		name: "MULTINOMIALE",
		description: "Restituisce il multinomiale di un insieme di numeri.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 valori per cui calcolare il multinomiale"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 valori per cui calcolare il multinomiale"
			}
		]
	},
	{
		name: "NOMINALE",
		description: "Restituisce il tasso di interesse nominale annuo.",
		arguments: [
			{
				name: "tasso_effettivo",
				description: "è il tasso di interesse effettivo"
			},
			{
				name: "periodi",
				description: "è il numero dei periodi di capitalizzazione per anno"
			}
		]
	},
	{
		name: "NON",
		description: "Inverte il valore logico dell'argomento: restituisce FALSO per un argomento VERO e VERO per un argomento FALSO.",
		arguments: [
			{
				name: "logico",
				description: "è un valore o un'espressione che può dare come risultato VERO o FALSO"
			}
		]
	},
	{
		name: "NON.DISP",
		description: "Restituisce il valore di errore #N/D (valore non disponibile).",
		arguments: [
		]
	},
	{
		name: "NORMALIZZA",
		description: "Restituisce un valore normalizzato da una distribuzione caratterizzata da una media e da una deviazione standard.",
		arguments: [
			{
				name: "x",
				description: "è il valore da normalizzare"
			},
			{
				name: "media",
				description: "è la media aritmetica della distribuzione"
			},
			{
				name: "dev_standard",
				description: "è la deviazione standard della distribuzione, un numero positivo"
			}
		]
	},
	{
		name: "NUM",
		description: "Converte stringhe di valori in numeri, date in numeri seriali,  VERO in 1, ogni altro valore in 0 (zero).",
		arguments: [
			{
				name: "val",
				description: "è il valore da convertire"
			}
		]
	},
	{
		name: "NUM.CED",
		description: "Calcola il numero di cedole valide tra la data di liquidazione e la data di scadenza.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del titolo espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del titolo espressa come numero seriale"
			},
			{
				name: "num_rate",
				description: "è il numero di pagamenti per anno"
			},
			{
				name: "base",
				description: "è il tipo di base da utilizzare per il conteggio dei giorni"
			}
		]
	},
	{
		name: "NUM.RATE",
		description: "Restituisce il numero di periodi relativi ad un investimento, dati pagamenti periodici costanti e un tasso di interesse costante.",
		arguments: [
			{
				name: "tasso_int",
				description: "è il tasso di interesse per periodo. Ad esempio, usare 6%/4 per pagamenti trimestrali al 6%."
			},
			{
				name: "pagam",
				description: "è il pagamento effettuato in ciascun periodo e non può variare nel corso dell'investimento."
			},
			{
				name: "val_attuale",
				description: "è il valore attuale o la somma forfettaria pari al valore attuale di una serie di pagamenti futuri"
			},
			{
				name: "val_futuro",
				description: "è il valore futuro o il saldo in contanti da conseguire dopo l'ultimo pagamento. Se omesso, verrà considerato uguale a zero."
			},
			{
				name: "tipo",
				description: "è un valore logico: pagamento all'inizio del periodo = 1; pagamento alla fine del periodo = 0 oppure omesso"
			}
		]
	},
	{
		name: "NUM.SETTIMANA",
		description: "Restituisce il numero della settimana dell'anno.",
		arguments: [
			{
				name: "num_seriale",
				description: "è il codice data-ora utilizzato da Spreadsheet per il calcolo della data e dell'ora"
			},
			{
				name: "tipo_restituito",
				description: "è un numero (1 o 2) che determina il tipo del valore restituito"
			}
		]
	},
	{
		name: "NUM.SETTIMANA.ISO",
		description: "Restituisce il numero della settimana ISO dell'anno per una data specificata.",
		arguments: [
			{
				name: "data",
				description: "è il codice data-ora usato da Spreadsheet per il calcolo della data e dell'ora"
			}
		]
	},
	{
		name: "NUMERO.VALORE",
		description: "Converte il testo in numero in modo indipendente dalle impostazioni locali.",
		arguments: [
			{
				name: "testo",
				description: "è la stringa che rappresenta il numero da convertire"
			},
			{
				name: "separatore_decimale",
				description: "è il carattere usato come separatore decimale nella stringa"
			},
			{
				name: "separatore_gruppo",
				description: "è il carattere usato come separatore di gruppo nella stringa"
			}
		]
	},
	{
		name: "O",
		description: "Restituisce VERO se un argomento qualsiasi è VERO, FALSO se tutti gli argomenti sono FALSO.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logico1",
				description: "sono da 1 a 255 condizioni da verificare, che possono avere valore VERO o FALSO"
			},
			{
				name: "logico2",
				description: "sono da 1 a 255 condizioni da verificare, che possono avere valore VERO o FALSO"
			}
		]
	},
	{
		name: "OCT.BINARIO",
		description: "Converte un numero ottale in binario.",
		arguments: [
			{
				name: "num",
				description: "è il numero ottale da convertire"
			},
			{
				name: "cifre",
				description: "è il numero di caratteri da utilizzare"
			}
		]
	},
	{
		name: "OCT.DECIMALE",
		description: "Converte un numero ottale in decimale.",
		arguments: [
			{
				name: "num",
				description: "è il numero ottale da convertire"
			}
		]
	},
	{
		name: "OCT.HEX",
		description: "Converte un numero ottale in esadecimale.",
		arguments: [
			{
				name: "num",
				description: "è il numero ottale da convertire"
			},
			{
				name: "cifre",
				description: "è il numero di caratteri da utilizzare"
			}
		]
	},
	{
		name: "OGGI",
		description: "Restituisce la data corrente nel formato data.",
		arguments: [
		]
	},
	{
		name: "ORA",
		description: "Restituisce l'ora come numero compreso tra 0 e 23.",
		arguments: [
			{
				name: "num_seriale",
				description: "è un numero nel codice data-ora utilizzato da Spreadsheet o testo in formato ora, quale 16.48.00"
			}
		]
	},
	{
		name: "ORARIO",
		description: "Converte ore, minuti e secondi forniti come numeri in un un numero seriale di Spreadsheet, formattato in modo appropriato.",
		arguments: [
			{
				name: "ora",
				description: "è un numero compreso tra 0 e 23 che rappresenta l'ora"
			},
			{
				name: "minuto",
				description: "è un numero compreso tra 0 e 59 che rappresenta i minuti"
			},
			{
				name: "secondo",
				description: "è un numero compreso tra 0 e 59 che rappresenta i secondi"
			}
		]
	},
	{
		name: "ORARIO.VALORE",
		description: "Converte un orario in formato testo in un numero seriale di Spreadsheet che rappresenta un orario, ovvero un numero compreso tra 0 (00.00.00) e 0,999988426 (23.59.59). Formattare il numero in base a uno dei formati per l'ora dopo avere immesso la formula.",
		arguments: [
			{
				name: "ora",
				description: "è una stringa di testo che rappresenta un orario in uno dei formati specifici di Spreadsheet. Le informazioni di data nella stringa vengono ignorate"
			}
		]
	},
	{
		name: "P.RATA",
		description: "Restituisce il pagamento sul capitale di un investimento per un dato periodo, dati pagamenti periodici costanti e un tasso di interesse costante.",
		arguments: [
			{
				name: "tasso_int",
				description: "è il tasso di interesse per il periodo. Ad esempio, utilizzare 6%/4 per pagamenti trimestrali al tasso del 6%."
			},
			{
				name: "periodo",
				description: "specifica il periodo e deve essere compreso tra 1 e n periodi"
			},
			{
				name: "periodi",
				description: "è il numero totale di periodi di pagamento in un investimento"
			},
			{
				name: "val_attuale",
				description: "è il valore attuale: la somma totale pari al valore attuale di una serie di pagamenti futuri"
			},
			{
				name: "val_futuro",
				description: "è il valore futuro o il saldo in contanti da conseguire dopo l'ultimo pagamento"
			},
			{
				name: "tipo",
				description: "è un valore logico: pagamento all'inizio del periodo = 1; pagamento alla fine del periodo = 0 oppure omesso"
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
		name: "PARI",
		description: "Arrotonda il valore assoluto di un numero per eccesso all'intero pari più vicino. I numeri negativi sono arrotondati per difetto.",
		arguments: [
			{
				name: "num",
				description: "è il valore da arrotondare"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Restituisce il prodotto del coefficiente di momento di correlazione di Pearson, r.",
		arguments: [
			{
				name: "matrice1",
				description: "è un insieme di valori indipendenti"
			},
			{
				name: "matrice2",
				description: "è un insieme di valori dipendenti"
			}
		]
	},
	{
		name: "PENDENZA",
		description: "Restituisce la pendenza della retta di regressione lineare fra le coordinate note.",
		arguments: [
			{
				name: "y_nota",
				description: "è una matrice o un intervallo di celle di valori numerici dipendenti e può  essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri"
			},
			{
				name: "x_nota",
				description: "è l'insieme dei valori indipendenti e può  essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "PERCENT.RANGO",
		description: "Restituisce il rango di un valore in un set di dati come percentuale del set di dati.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o l'intervallo di dati con valori numerici che definiscono la condizione relativa"
			},
			{
				name: "x",
				description: "è il valore di cui conoscere il rango"
			},
			{
				name: "cifre_signific",
				description: "è un valore facoltativo che identifica il numero di cifre significative per la percentuale restituita, tre cifre se omesso (0,xxx %)"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Restituisce il k-esimo dato percentile di valori in un intervallo.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o l'intervallo di dati che definisce la condizione relativa"
			},
			{
				name: "k",
				description: "è il valore percentile, compreso nell'intervallo tra 0 e 1 inclusi"
			}
		]
	},
	{
		name: "PERMUTAZIONE",
		description: "Restituisce il numero delle permutazioni per un dato numero di oggetti che possono essere selezionati dagli oggetti totali.",
		arguments: [
			{
				name: "num",
				description: "è il numero totale di oggetti"
			},
			{
				name: "classe",
				description: "è il numero di oggetti per ogni permutazione"
			}
		]
	},
	{
		name: "PERMUTAZIONE.VALORI",
		description: "Restituisce il numero delle permutazioni per un dato numero di oggetti (con ripetizioni) che possono essere selezionati dagli oggetti totali.",
		arguments: [
			{
				name: "num",
				description: "è il numero totale di oggetti"
			},
			{
				name: "classe",
				description: "è il numero di oggetti per ogni permutazione"
			}
		]
	},
	{
		name: "PHI",
		description: "Restituisce il valore della funzione densità per una distribuzione normale standard.",
		arguments: [
			{
				name: "x",
				description: "è il valore per il quale si calcola la densità della distribuzione normale standard"
			}
		]
	},
	{
		name: "PI.GRECO",
		description: "Restituisce il valore di pi greco 3,14159265358979, approssimato a 15 cifre.",
		arguments: [
		]
	},
	{
		name: "PICCOLO",
		description: "Restituisce il k-esimo valore più piccolo di un set di dati. Ad esempio il quinto numero più piccolo.",
		arguments: [
			{
				name: "matrice",
				description: "è una matrice o un intervallo di dati numerici di cui determinare il k-esimo valore più piccolo"
			},
			{
				name: "k",
				description: "è la posizione del valore da restituire, partendo dal più piccolo, nella matrice o nell'intervallo"
			}
		]
	},
	{
		name: "POISSON",
		description: "Calcola la distribuzione di probabilità di Poisson.",
		arguments: [
			{
				name: "x",
				description: "è il numero degli eventi"
			},
			{
				name: "media",
				description: "è il valore numerico previsto, un numero positivo"
			},
			{
				name: "cumulativo",
				description: "è un valore logico: utilizzare VERO per la probabilità cumulativa di Poisson; utilizzare FALSO per la funzione probabilità di massa"
			}
		]
	},
	{
		name: "POTENZA",
		description: "Restituisce il risultato di un numero elevato a potenza.",
		arguments: [
			{
				name: "num",
				description: "è la base, un qualsiasi numero reale"
			},
			{
				name: "potenza",
				description: "è l'esponente a cui elevare la base"
			}
		]
	},
	{
		name: "PREVISIONE",
		description: "Calcola o prevede un valore futuro lungo una tendenza lineare utilizzando i valori esistenti.",
		arguments: [
			{
				name: "x",
				description: "è la coordinata di cui prevedere il valore. Deve essere un valore numerico."
			},
			{
				name: "y_nota",
				description: "è la matrice o intervallo di dati dipendente"
			},
			{
				name: "x_nota",
				description: "è la matrice o intervallo indipendente di valori numerici. La varianza di X_nota deve essere diversa da zero."
			}
		]
	},
	{
		name: "PREZZO.SCONT",
		description: "Restituisce il prezzo di un titolo scontato con valore nominale di 100 lire.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del titolo espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del titolo espressa come numero seriale"
			},
			{
				name: "sconto",
				description: "è il tasso di sconto del titolo"
			},
			{
				name: "prezzo_rimb",
				description: "è il valore di rimborso del titolo con valore nominale di 100 lire"
			},
			{
				name: "base",
				description: "è il tipo di base da utilizzare per il conteggio dei giorni"
			}
		]
	},
	{
		name: "PROBABILITÀ",
		description: "Calcola la probabilità che dei valori in un intervallo siano compresi tra due limiti o pari al limite inferiore.",
		arguments: [
			{
				name: "int_x",
				description: "è l'intervallo dei valori numerici per x a cui sono associate delle probabilità"
			},
			{
				name: "prob_int",
				description: "è l'insieme delle probabilità associate ai valori di Int_x, compresi tra 0 e 1 (0 escluso)"
			},
			{
				name: "limite_inf",
				description: "è il limite inferiore del valore per il quale si calcola la probabilità"
			},
			{
				name: "limite_sup",
				description: "è il limite superiore facoltativo del valore. Se viene omesso, PROBABILITÀ restituirà la probabilità che i valori Int_x siano uguali a Limite_inf."
			}
		]
	},
	{
		name: "PRODOTTO",
		description: "Moltiplica tutti i numeri passati come argomenti e restituisce il prodotto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, valori logici o rappresentazioni in formato testo dei numeri da moltiplicare"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, valori logici o rappresentazioni in formato testo dei numeri da moltiplicare"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Restituisce il quartile di un set di dati.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o intervallo di celle a valori numerici per cui si calcola il valore quartile"
			},
			{
				name: "quarto",
				description: "è un numero: valore minimo = 0; primo quartile = 1; mediana = 2; terzo quartile = 3; valore massimo = 4"
			}
		]
	},
	{
		name: "QUOZIENTE",
		description: "Restituisce il quoziente di una divisione.",
		arguments: [
			{
				name: "numeratore",
				description: "è il dividendo"
			},
			{
				name: "denominatore",
				description: "è il divisore"
			}
		]
	},
	{
		name: "RADIANTI",
		description: "Converte gradi in radianti.",
		arguments: [
			{
				name: "angolo",
				description: "è l'angolo da convertire, espresso in gradi"
			}
		]
	},
	{
		name: "RADQ",
		description: "Restituisce la radice quadrata di un numero.",
		arguments: [
			{
				name: "num",
				description: "è il numero del quale si desidera la radice quadrata"
			}
		]
	},
	{
		name: "RADQ.PI.GRECO",
		description: "Restituisce la radice quadrata di (num *pi greco).",
		arguments: [
			{
				name: "num",
				description: "è il numero per cui moltiplicare pi greco"
			}
		]
	},
	{
		name: "RANGO",
		description: "Restituisce il rango di un numero in un elenco di numeri: la sua grandezza relativa agli altri valori nell'elenco.",
		arguments: [
			{
				name: "num",
				description: "è il numero di cui ricercare il rango"
			},
			{
				name: "rif",
				description: "è una matrice di numeri o un riferimento ad un elenco di numeri. I valori in rif che non sono di tipo numerico vengono ignorati."
			},
			{
				name: "ordine",
				description: "è un numero: rango discendente nell'elenco ordinato = 0 oppure omesso; rango ascendente nell'elenco ordinato = qualsiasi valore diverso da zero"
			}
		]
	},
	{
		name: "RANGO.MEDIA",
		description: "Restituisce il rango di un numero in un elenco di numeri: la sua grandezza relativa agli altri valori nell'elenco; se più valori hanno lo stesso rango, viene restituito il rango medio.",
		arguments: [
			{
				name: "num",
				description: "è il numero di cui ricercare il rango"
			},
			{
				name: "rif",
				description: "è una matrice di numeri o un riferimento ad un elenco di numeri. I valori che non sono di tipo numerico vengono ignorati."
			},
			{
				name: "ordine",
				description: "è un numero: rango nell'elenco in ordine decrescente = 0 oppure omesso; rango nell'elenco in ordine crescente = qualsiasi valore diverso da zero"
			}
		]
	},
	{
		name: "RANGO.UG",
		description: "Restituisce il rango di un numero in un elenco di numeri: la sua grandezza relativa agli altri valori nell'elenco; se più valori hanno lo stesso rango, viene restituito il rango massimo del set di valori.",
		arguments: [
			{
				name: "num",
				description: "è il numero di cui ricercare il rango"
			},
			{
				name: "rif",
				description: "è una matrice di numeri o un riferimento ad un elenco di numeri. I valori che non sono di tipo numerico vengono ignorati."
			},
			{
				name: "ordine",
				description: "è un numero: rango nell'elenco in ordine decrescente = 0 oppure omesso; rango nell'elenco in ordine crescente = qualsiasi valore diverso da zero"
			}
		]
	},
	{
		name: "RATA",
		description: "Calcola il pagamento per un prestito in base a pagamenti costanti e a un tasso di interesse costante.",
		arguments: [
			{
				name: "tasso_int",
				description: "è il tasso d'interesse per periodo relativo al prestito. Ad esempio, usare 6%/4 per pagamenti trimestrali al 6%."
			},
			{
				name: "periodi",
				description: "è il numero totale dei pagamenti per il prestito"
			},
			{
				name: "pv",
				description: "è il valore attuale o la somma forfettaria pari al valore attuale di una serie di pagamenti futuri"
			},
			{
				name: "val_futuro",
				description: "è il valore futuro o il saldo in contanti da conseguire dopo l'ultimo pagamento. Se omesso, verrà considerato uguale a 0 (zero)."
			},
			{
				name: "tipo",
				description: "è un valore logico: pagamento all'inizio del periodo = 1; pagamento alla fine del periodo = 0 oppure omesso"
			}
		]
	},
	{
		name: "REGR.LIN",
		description: "Restituisce statistiche che descrivono una tendenza lineare corrispondente a punti di dati conosciuti  utilizzando il metodo dei minimi quadrati.",
		arguments: [
			{
				name: "y_nota",
				description: "è l'insieme dei valori y già noti dalla relazione y = mx + b"
			},
			{
				name: "x_nota",
				description: "è un insieme facoltativo di valori x che possono essere già noti dalla relazione y = mx + b"
			},
			{
				name: "cost",
				description: "è un valore logico: la costante b viene calcolata normalmente se Cost = VERO o omesso; b viene posta uguale a 0 se Cost = FALSO"
			},
			{
				name: "stat",
				description: "è un valore logico: restituisce statistiche aggiuntive per la regressione = VERO; restituisce coefficienti m e la costante b = FALSO oppure omesso"
			}
		]
	},
	{
		name: "REGR.LOG",
		description: "Restituisce statistiche che descrivono una curva esponenziale che corrisponde a punti di dati conosciuti.",
		arguments: [
			{
				name: "y_nota",
				description: "è l'insieme dei valori y già noti dalla relazione y = b*m^x"
			},
			{
				name: "x_nota",
				description: "è un insieme facoltativo di valori x che possono essere già noti dalla  relazione y = b*m^x"
			},
			{
				name: "cost",
				description: "è un valore logico: la costante b viene calcolata normalmente se Cost = VERO oppure omesso; b viene posto uguale a 1 se Cost = FALSO"
			},
			{
				name: "stat",
				description: "è un valore logico: restituisce statistiche aggiuntive per la regressione = VERO; restituisce coefficienti m e la costante b = FALSO oppure omesso"
			}
		]
	},
	{
		name: "REND.TITOLI.SCONT",
		description: "Calcola il rendimento annuale per un titolo scontato, ad esempio un buono del tesoro.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del titolo espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del titolo espressa come numero seriale"
			},
			{
				name: "prezzo",
				description: "è il prezzo del titolo con valore nominale di 100 lire"
			},
			{
				name: "prezzo_rimb",
				description: "è il valore di rimborso del titolo con valore nominale di 100 lire"
			},
			{
				name: "base",
				description: "è il tipo di base da utilizzare per il conteggio dei giorni"
			}
		]
	},
	{
		name: "RESTO",
		description: "Restituisce il resto della divisione di due numeri.",
		arguments: [
			{
				name: "dividendo",
				description: "è il numero di cui si calcola il resto dopo l'esecuzione della divisione"
			},
			{
				name: "divisore",
				description: "è il numero per il quale si desidera dividere il dividendo"
			}
		]
	},
	{
		name: "RICERCA",
		description: "Restituisce il numero corrispondente al carattere o alla stringa di testo trovata in una seconda stringa di testo (non distingue tra maiuscole e minuscole).",
		arguments: [
			{
				name: "testo",
				description: "è il testo da trovare. È possibile utilizzare i caratteri jolly ? e *. Utilizzare ~? e ~* per trovare i caratteri ? e *."
			},
			{
				name: "stringa",
				description: "è il testo all'interno del quale effettuare la ricerca di Testo"
			},
			{
				name: "inizio",
				description: "è il numero del carattere in Stringa, a partire da sinistra,  dal quale si desidera iniziare la ricerca. Se omesso, verrà utilizzato 1"
			}
		]
	},
	{
		name: "RICEV.SCAD",
		description: "Calcola l'importo ricevuto alla scadenza di un titolo.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del titolo espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del titolo espressa come numero seriale"
			},
			{
				name: "invest",
				description: "è l'importo investito nel titolo"
			},
			{
				name: "sconto",
				description: "è il tasso di sconto del titolo"
			},
			{
				name: "base",
				description: "è il tipo di base da utilizzare per il conteggio dei giorni"
			}
		]
	},
	{
		name: "RIF.COLONNA",
		description: "Restituisce il numero di colonna di un riferimento.",
		arguments: [
			{
				name: "rif",
				description: "è la cella o l'intervallo di celle contigue da cui ottenere il numero di colonna. Se viene omesso, verrà usata la cella contenente la funzione COLONNA."
			}
		]
	},
	{
		name: "RIF.RIGA",
		description: "Restituisce il numero di riga corrispondente a rif.",
		arguments: [
			{
				name: "rif",
				description: "è la cella o l'intervallo di celle di cui ottenere il numero di riga. Se viene omesso, verrà restituita la cella contenente la funzione RIGA."
			}
		]
	},
	{
		name: "RIGHE",
		description: "Restituisce il numero di righe in un riferimento o in una matrice.",
		arguments: [
			{
				name: "matrice",
				description: "è una matrice o una formula in forma di matrice oppure un riferimento ad un intervallo di celle di cui ottenere il numero di righe"
			}
		]
	},
	{
		name: "RIMPIAZZA",
		description: "Sostituisce parte di una stringa di testo con un'altra stringa di testo.",
		arguments: [
			{
				name: "testo_prec",
				description: "è il testo nel quale si desidera sostituire alcuni caratteri"
			},
			{
				name: "inizio",
				description: "è la posizione del carattere in Testo_prec da sostituire con Nuovo_testo"
			},
			{
				name: "num_caratt",
				description: "è il numero di caratteri in Testo_prec da sostituire con Nuovo_testo"
			},
			{
				name: "nuovo_testo",
				description: "è il testo che sostituirà i caratteri in Testo_prec"
			}
		]
	},
	{
		name: "RIPETI",
		description: "Ripete un testo per il numero di volte specificato. Utilizzare RIPETI per riempire una cella con il numero di occorrenze di una stringa di testo.",
		arguments: [
			{
				name: "testo",
				description: "è il testo da ripetere"
			},
			{
				name: "volte",
				description: "è un numero positivo che specifica di quante volte ripetere il testo"
			}
		]
	},
	{
		name: "RIT.INVEST.EFFETT",
		description: "Restituisce un tasso di interesse equivalente per la crescita di un investimento.",
		arguments: [
			{
				name: "periodi",
				description: "è il numero di periodi per l'investimento"
			},
			{
				name: "val_attuale",
				description: "è il valore attuale dell'investimento"
			},
			{
				name: "val_futuro",
				description: "è il valore futuro dell'investimento"
			}
		]
	},
	{
		name: "ROMANO",
		description: "Converte un numero arabo in un numero romano in forma di testo.",
		arguments: [
			{
				name: "num",
				description: "è il numero arabo da convertire"
			},
			{
				name: "forma",
				description: "è il numero che specifica il tipo di numero romano che si desidera."
			}
		]
	},
	{
		name: "RQ",
		description: "Restituisce la radice quadrata del coefficiente di  momento di correlazione di Pearson in corrispondenza delle coordinate date.",
		arguments: [
			{
				name: "y_nota",
				description: "è una matrice o un intervallo di valori e può  essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri"
			},
			{
				name: "x_nota",
				description: "è una matrice o un intervallo di valori e può  essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri"
			}
		]
	},
	{
		name: "SCARTO",
		description: "Restituisce un riferimento a un intervallo costituito da un numero specificato di righe e colonne da un riferimento dato.",
		arguments: [
			{
				name: "rif",
				description: "è il riferimento da cui si desidera iniziare lo scostamento, un riferimento a una cella o a un intervallo di celle adiacenti"
			},
			{
				name: "righe",
				description: "è il numero di righe, in alto o in basso, da utilizzare come riferimento per la cella superiore sinistra del risultato"
			},
			{
				name: "colonne",
				description: "è il numero di colonne, in alto o in basso, da utilizzare come riferimento per la cella superiore sinistra del risultato"
			},
			{
				name: "altezza",
				description: "è l'altezza del risultato espressa in numero di righe. Se viene omessa, sarà pari all'altezza in Riferimento."
			},
			{
				name: "largh",
				description: "è la larghezza del risultato espressa in numero di colonne. Se viene omessa, sarà pari alla larghezza in Riferimento"
			}
		]
	},
	{
		name: "SCEGLI",
		description: "Seleziona un valore o un'azione da eseguire da un elenco di valori in base a un indice.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "indice",
				description: "specifica quale argomento valore viene selezionato. L'indice deve essere un numero compreso tra 1 e 254, una formula o un riferimento a un numero compreso tra 1 e 254."
			},
			{
				name: "val1",
				description: "sono da 1 a 254 numeri, riferimenti di cella, nomi definiti, formule, funzioni o argomenti di testo tra i quali viene effettuata una selezione tramite SCEGLI"
			},
			{
				name: "val2",
				description: "sono da 1 a 254 numeri, riferimenti di cella, nomi definiti, formule, funzioni o argomenti di testo tra i quali viene effettuata una selezione tramite SCEGLI"
			}
		]
	},
	{
		name: "SE",
		description: "Restituisce un valore se una condizione specificata dà come risultato VERO e un altro valore se dà come risultato FALSO.",
		arguments: [
			{
				name: "test",
				description: "è un valore o un'espressione qualsiasi che può dare come risultato VERO o FALSO"
			},
			{
				name: "se_vero",
				description: "è il valore che viene restituito se test è VERO. Se viene omesso, verrà restituito VERO. È possibile annidare fino a sette funzioni SE."
			},
			{
				name: "se_falso",
				description: "è il valore che viene restituito se test è FALSO. Se viene omesso, verrà restituito FALSO."
			}
		]
	},
	{
		name: "SE.ERRORE",
		description: "Restituisce valore_se_errore se l'espressione genera un errore, in caso contrario restituisce il valore dell'espressione stessa.",
		arguments: [
			{
				name: "valore",
				description: "è un valore, un'espressione o un riferimento qualsiasi"
			},
			{
				name: "valore_se_errore",
				description: "è un valore, un'espressione o un riferimento qualsiasi"
			}
		]
	},
	{
		name: "SE.NON.DISP.",
		description: "Restituisce il valore specificato se l'espressione restituisce #N/D, in caso contrario restituisce il risultato dell'espressione.",
		arguments: [
			{
				name: "valore",
				description: "è un valore, un'espressione o un riferimento qualsiasi"
			},
			{
				name: "valore_se_nd",
				description: "è un valore, un'espressione o un riferimento qualsiasi"
			}
		]
	},
	{
		name: "SEC",
		description: "Restituisce la secante di un angolo.",
		arguments: [
			{
				name: "num",
				description: "è l'angolo espresso in radianti di cui si calcola la secante"
			}
		]
	},
	{
		name: "SECH",
		description: "Restituisce la secante iperbolica di un angolo.",
		arguments: [
			{
				name: "num",
				description: "è l'angolo espresso in radianti di cui si calcola la secante iperbolica"
			}
		]
	},
	{
		name: "SECONDO",
		description: "Restituisce il secondo, un numero compreso tra 0 e 59.",
		arguments: [
			{
				name: "num_seriale",
				description: "è un numero nel codice data-ora utilizzato da Spreadsheet o testo in formato ora, quale 16.48.23"
			}
		]
	},
	{
		name: "SEGNO",
		description: "Restituisce il segno di un numero: 1 se il numero è positivo, zero se il numero è zero o -1 se il numero è negativo.",
		arguments: [
			{
				name: "num",
				description: "è un qualsiasi numero reale"
			}
		]
	},
	{
		name: "SEN",
		description: "Restituisce il seno di un angolo.",
		arguments: [
			{
				name: "radianti",
				description: "è l'angolo espresso in radianti di cui si calcola il seno. Gradi * PI()/180 = radianti."
			}
		]
	},
	{
		name: "SENH",
		description: "Restituisce il seno iperbolico di un numero.",
		arguments: [
			{
				name: "num",
				description: "è un qualsiasi numero reale"
			}
		]
	},
	{
		name: "SINISTRA",
		description: "Restituisce il carattere o i caratteri più a sinistra di una stringa di testo.",
		arguments: [
			{
				name: "testo",
				description: "è la stringa di testo che contiene i caratteri da estrarre"
			},
			{
				name: "num_caratt",
				description: "specifica il numero di caratteri da estrarre da SINISTRA; 1 se omesso"
			}
		]
	},
	{
		name: "SOGLIA",
		description: "Verifica se un numero è maggiore di un valore soglia.",
		arguments: [
			{
				name: "num",
				description: "è il valore da confrontare con val_soglia"
			},
			{
				name: "val_soglia",
				description: "è il valore soglia"
			}
		]
	},
	{
		name: "SOMMA",
		description: "Somma i numeri presenti in un intervallo di celle.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti di cui ottenere la somma. I valori logici e il testo vengono ignorati, anche se digitati come argomenti."
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti di cui ottenere la somma. I valori logici e il testo vengono ignorati, anche se digitati come argomenti."
			}
		]
	},
	{
		name: "SOMMA.DIFF.Q",
		description: "Calcola la differenza tra i quadrati di numeri corrispondenti di due intervalli o matrici e restituisce la somma delle differenze.",
		arguments: [
			{
				name: "matrice_x",
				description: "è il primo intervallo o matrice di numeri e può essere un numero, un nome, una matrice o un riferimento contenente numeri"
			},
			{
				name: "matrice_y",
				description: "è il secondo intervallo o matrice di numeri e può essere un numero, un nome, una matrice o un riferimento contenente numeri"
			}
		]
	},
	{
		name: "SOMMA.PIÙ.SE",
		description: "Somma le celle specificate da un determinato insieme di condizioni o criteri.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "int_somma",
				description: "sono le celle effettive da sommare"
			},
			{
				name: "intervallo_criteri",
				description: "è l'intervallo di celle da valutare per la condizione specificata"
			},
			{
				name: "criteri",
				description: "è la condizione o il criterio, in forma di numero, espressione o testo, che definisce le celle da sommare"
			}
		]
	},
	{
		name: "SOMMA.Q",
		description: "Restituisce la somma dei quadrati degli argomenti. Gli argomenti possono essere numeri, nomi, matrici o riferimenti a celle contenenti numeri.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti a matrici di cui calcolare la somma dei quadrati"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 numeri, nomi, matrici o riferimenti a matrici di cui calcolare la somma dei quadrati"
			}
		]
	},
	{
		name: "SOMMA.Q.DIFF",
		description: "Calcola la differenza tra valori corrispondenti di due intervalli o matrici e restituisce la somma dei quadrati delle differenze.",
		arguments: [
			{
				name: "matrice_x",
				description: "è il primo intervallo o matrice di valori e può essere un numero, un nome, una matrice o un riferimento contenente numeri"
			},
			{
				name: "matrice_y",
				description: "è il secondo intervallo o matrice di valori e può essere un numero, un nome, una matrice o un riferimento contenente numeri"
			}
		]
	},
	{
		name: "SOMMA.SE",
		description: "Somma le celle specificate secondo una condizione o criterio assegnato.",
		arguments: [
			{
				name: "intervallo",
				description: "è l'intervallo di celle da analizzare"
			},
			{
				name: "criterio",
				description: "è il criterio o la condizione, in forma di numero, espressione o testo, che stabilisce quali celle verranno sommate"
			},
			{
				name: "int_somma",
				description: "sono le effettive celle da sommare. Se viene omesso, verranno utilizzate le celle nell'intervallo."
			}
		]
	},
	{
		name: "SOMMA.SERIE",
		description: "Restituisce la somma di una serie di potenze basata sulla formula.",
		arguments: [
			{
				name: "x",
				description: "è il valore di input della serie di potenze"
			},
			{
				name: "n",
				description: "è la potenza iniziale a cui elevare x"
			},
			{
				name: "m",
				description: "è l'incremento di 'n' per ogni termine della serie"
			},
			{
				name: "coefficienti",
				description: "è un insieme di coefficienti per cui moltiplicare ogni potenza successiva di x"
			}
		]
	},
	{
		name: "SOMMA.SOMMA.Q",
		description: "Calcola la somma dei quadrati di numeri corrispondenti di due intervalli o matrici e restituisce la somma delle somme.",
		arguments: [
			{
				name: "matrice_x",
				description: "è il primo intervallo o matrice di numeri e può essere un numero, un nome, una matrice o un riferimento contenente numeri"
			},
			{
				name: "matrice_y",
				description: "è il secondo intervallo o matrice di numeri e può essere un numero, un nome, una matrice o un riferimento contenente numeri"
			}
		]
	},
	{
		name: "SOSTITUISCI",
		description: "Sostituisce il nuovo testo a quello esistente in una stringa di testo.",
		arguments: [
			{
				name: "testo",
				description: "è il testo o un riferimento ad una cella contenente del testo in cui si desidera sostituire dei caratteri"
			},
			{
				name: "testo_prec",
				description: "è il testo esistente da sostituire. Se i caratteri maiuscoli/minuscoli di Testo_prec non corrispondono a quelli del testo, SOSTITUISCI non funzionerà."
			},
			{
				name: "nuovo_testo",
				description: "è il testo da sostituire a Testo_prec"
			},
			{
				name: "occorrenza",
				description: "specifica l'occorrenza di Testo_prec da sostituire. Se viene omesso, tutte le occorrenze di Testo_prec verranno sostituite."
			}
		]
	},
	{
		name: "STRINGA.ESTRAI",
		description: "Restituisce un numero specifico di caratteri da una stringa di testo iniziando dalla posizione specificata.",
		arguments: [
			{
				name: "testo",
				description: "è la stringa di testo da cui estrarre i caratteri"
			},
			{
				name: "inizio",
				description: "è la posizione del primo carattere da estrarre. Il primo carattere in Testo è 1"
			},
			{
				name: "num_caratt",
				description: "specifica il numero di caratteri che devono essere restituiti da Testo"
			}
		]
	},
	{
		name: "SUBTOTALE",
		description: "Restituisce un subtotale in un elenco o un database.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num_funzione",
				description: "è un numero compreso tra 1 e 11 che specifica la funzione di riepilogo per il subtotale"
			},
			{
				name: "rif1",
				description: "sono da 1 a 254 intervalli o riferimenti di cui calcolare il subtotale"
			}
		]
	},
	{
		name: "T",
		description: "Controlla se il valore è un testo e, in caso positivo, lo restituisce, altrimenti vengono restituite delle virgolette, ossia testo vuoto.",
		arguments: [
			{
				name: "val",
				description: "è il valore da verificare"
			}
		]
	},
	{
		name: "TAN",
		description: "Restituisce la tangente di un numero.",
		arguments: [
			{
				name: "radianti",
				description: "è l'angolo espresso in radianti di cui si calcola la tangente. Gradi * PI()/180 = radianti."
			}
		]
	},
	{
		name: "TANH",
		description: "Restituisce la tangente iperbolica di un numero.",
		arguments: [
			{
				name: "num",
				description: "è un qualsiasi numero reale"
			}
		]
	},
	{
		name: "TASSO",
		description: "Restituisce il tasso di interesse per periodo relativo a un prestito o a un investimento. Ad esempio, usare 6%/4 per pagamenti trimestrali al 6%.",
		arguments: [
			{
				name: "periodi",
				description: "è il numero totale dei periodi di pagamento per il prestito o per l'investimento"
			},
			{
				name: "pagam",
				description: "è il pagamento effettuato in ciascun periodo e non può variare nel corso del prestito o dell'investimento."
			},
			{
				name: "val_attuale",
				description: "è il valore attuale o la somma forfettaria pari al valore attuale di una serie di pagamenti futuri"
			},
			{
				name: "val_futuro",
				description: "è il valore futuro o il saldo in contanti da conseguire dopo l'ultimo pagamento. Se omesso, Fv = 0"
			},
			{
				name: "tipo",
				description: "è un valore logico: pagamento all'inizio del periodo = 1; pagamento alla fine del periodo = 0 oppure omesso"
			},
			{
				name: "ipotesi",
				description: "è la previsione del tasso di interesse futuro; se omesso, Ipotesi = 0,1 (10 %)"
			}
		]
	},
	{
		name: "TASSO.INT",
		description: "Restituisce il tasso di interesse per un titolo interamente investito.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del titolo espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del titolo espressa come numero seriale"
			},
			{
				name: "invest",
				description: "è l'importo investito nel titolo"
			},
			{
				name: "prezzo_rimb",
				description: "è l'importo da ricevere alla scadenza"
			},
			{
				name: "base",
				description: "è il tipo di base da utilizzare per il conteggio dei giorni"
			}
		]
	},
	{
		name: "TASSO.SCONTO",
		description: "Calcola il tasso di sconto di un titolo.",
		arguments: [
			{
				name: "liquid",
				description: "è la data di liquidazione del titolo espressa come numero seriale"
			},
			{
				name: "scad",
				description: "è la data di scadenza del titolo espressa come numero seriale"
			},
			{
				name: "prezzo",
				description: "è il prezzo del titolo con valore nominale di 100 lire"
			},
			{
				name: "prezzo_rimb",
				description: "è il valore di rimborso del titolo con valore nominale di 100 lire"
			},
			{
				name: "base",
				description: "è il tipo di base da utilizzare per il conteggio dei giorni"
			}
		]
	},
	{
		name: "TENDENZA",
		description: "Restituisce i numeri in una una tendenza lineare corrispondente a punti di dati conosciuti  utilizzando il metodo dei minimi quadrati.",
		arguments: [
			{
				name: "y_nota",
				description: "è l'insieme dei valori y già noti dalla relazione y = mx + b"
			},
			{
				name: "x_nota",
				description: "è una matrice o un intervallo facoltativo di valori x che possono essere già noti dalla relazione y = mx + b, una matrice delle stesse dimensioni di Y_nota"
			},
			{
				name: "nuova_x",
				description: "è un intervallo o una matrice di nuovi valori x per i quali TENDENZA restituirà i corrispondenti valori y"
			},
			{
				name: "cost",
				description: "è un valore logico: la costante b viene calcolata normalmente se Cost = VERO oppure omesso; b è posto uguale a 0 se Cost = FALSO"
			}
		]
	},
	{
		name: "TEST.CHI",
		description: "Restituisce il test per l'indipendenza: il valore dalla distribuzione del chi quadrato per la statistica e i gradi di libertà appropriati.",
		arguments: [
			{
				name: "int_effettivo",
				description: "è l'intervallo di dati contenente le osservazioni da confrontare con i valori attesi"
			},
			{
				name: "int_previsto",
				description: "è l'intervallo di dati contenente la proporzione del prodotto dei totali di riga e di colonna per il totale complessivo"
			}
		]
	},
	{
		name: "TEST.CHI.QUAD",
		description: "Restituisce il test per l'indipendenza: il valore dalla distribuzione del chi quadrato per la statistica e i gradi di libertà appropriati.",
		arguments: [
			{
				name: "int_effettivo",
				description: "è l'intervallo di dati contenente le osservazioni da confrontare con i valori attesi"
			},
			{
				name: "int_previsto",
				description: "è l'intervallo di dati contenente la proporzione  del prodotto dei totali di riga e di colonna per il totale complessivo"
			}
		]
	},
	{
		name: "TEST.F",
		description: "Restituisce il risultato di un test F: la probabilità a due code che le varianze in Matrice1 e Matrice2 non siano significativamente differenti.",
		arguments: [
			{
				name: "matrice1",
				description: "è il primo intervallo o matrice di dati e può essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri (gli spazi vengono ignorati)"
			},
			{
				name: "matrice2",
				description: "è il secondo intervallo o matrice di dati e può essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri (gli spazi vengono ignorati)"
			}
		]
	},
	{
		name: "TEST.T",
		description: "Restituisce la probabilità associata ad un test t di Student.",
		arguments: [
			{
				name: "matrice1",
				description: "è il primo set di dati"
			},
			{
				name: "matrice2",
				description: "è il secondo set di dati"
			},
			{
				name: "coda",
				description: "specifica il numero delle code della distribuzione da restituire: distribuzione a una coda = 1; distribuzione a due code = 2"
			},
			{
				name: "tipo",
				description: "è il tipo di test t da eseguire: accoppiato = 1, a due campioni a varianza identica a due campioni (omoscedastico) = 2, varianza dissimile a due campioni = 3"
			}
		]
	},
	{
		name: "TEST.Z",
		description: "Restituisce il livello di significatività a una coda per un test z.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o intervallo di dati con cui verificare x"
			},
			{
				name: "x",
				description: "è il valore da esaminare"
			},
			{
				name: "sigma",
				description: "è la deviazione standard della popolazione (nota). Se viene omessa, verrà utilizzata la deviazione standard del campione."
			}
		]
	},
	{
		name: "TESTF",
		description: "Restituisce il risultato di un test F: la probabilità a due code che le varianze in Matrice1 e Matrice2 non siano significativamente differenti.",
		arguments: [
			{
				name: "matrice1",
				description: "è il primo intervallo o matrice di dati e può essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri (gli spazi vengono ignorati)"
			},
			{
				name: "matrice2",
				description: "è il secondo intervallo o matrice di dati e può essere costituito da numeri, nomi, matrici o riferimenti contenenti numeri (gli spazi vengono ignorati)"
			}
		]
	},
	{
		name: "TESTO",
		description: "Converte un valore in testo secondo uno specifico formato numero.",
		arguments: [
			{
				name: "val",
				description: "è un valore numerico, una formula che calcola un valore numerico o un riferimento a una cella contenente un valore numerico"
			},
			{
				name: "formato",
				description: "è un formato numero sotto forma di testo nella casella Categoria della scheda Numero nella finestra di dialogo Formato celle"
			}
		]
	},
	{
		name: "TESTO.FORMULA",
		description: "Restituisce una formula come stringa.",
		arguments: [
			{
				name: "riferimento",
				description: "è un riferimento a una formula"
			}
		]
	},
	{
		name: "TESTT",
		description: "Restituisce la probabilità associata ad un test t di Student.",
		arguments: [
			{
				name: "matrice1",
				description: "è il primo set di dati"
			},
			{
				name: "matrice2",
				description: "è il secondo set di dati"
			},
			{
				name: "coda",
				description: "specifica il numero delle code della distribuzione da restituire: distribuzione a una coda = 1; distribuzione a due code = 2"
			},
			{
				name: "tipo",
				description: "è il tipo di test t da eseguire: accoppiato = 1, a due campioni a varianza identica a due campioni (omoscedastico) = 2, varianza dissimile a due campioni = 3"
			}
		]
	},
	{
		name: "TESTZ",
		description: "Restituisce il livello di significatività a una coda per un test z.",
		arguments: [
			{
				name: "matrice",
				description: "è la matrice o intervallo di dati con cui verificare x"
			},
			{
				name: "x",
				description: "è il valore da esaminare"
			},
			{
				name: "sigma",
				description: "è la deviazione standard della popolazione (nota). Se viene omessa, verrà utilizzata la deviazione standard del campione."
			}
		]
	},
	{
		name: "TIPO",
		description: "Restituisce un numero intero che indica il tipo di dati di un valore: numero = 1; testo = 2; valore logico = 4; formula = 8; valore di errore = 16; matrice = 64.",
		arguments: [
			{
				name: "val",
				description: "può essere un qualsiasi valore"
			}
		]
	},
	{
		name: "TIR.COST",
		description: "Restituisce il tasso di rendimento interno per una serie di flussi di cassa.",
		arguments: [
			{
				name: "val",
				description: "è una matrice o un riferimento a celle che contengono numeri di cui si calcola il tasso di rendimento"
			},
			{
				name: "ipotesi",
				description: "è un numero che si suppone vicino al risultato di TIR.COST; 0,1 (10 %) se omesso"
			}
		]
	},
	{
		name: "TIR.VAR",
		description: "Restituisce il tasso di rendimento interno per una serie di flussi di cassa periodici, considerando sia il costo di investimento sia gli interessi da reinvestimento della liquidità.",
		arguments: [
			{
				name: "val",
				description: "è una matrice o un riferimento a celle che contengono numeri che rappresentano una serie di uscite (negativi) ed entrate (positivi) a intervalli regolari"
			},
			{
				name: "costo",
				description: "è il tasso di interesse corrisposto sul contante utilizzato per i flussi di cassa"
			},
			{
				name: "ritorno",
				description: "è il tasso di interesse percepito sui flussi di cassa nel momento in cui il contante viene reinvestito"
			}
		]
	},
	{
		name: "TIR.X",
		description: "Restituisce il tasso di rendimento interno per un impiego di flussi di cassa.",
		arguments: [
			{
				name: "valori",
				description: "è una serie di flussi di cassa che corrispondono alle scadenze di pagamento"
			},
			{
				name: "date_pagam",
				description: "sono le scadenze di pagamento che corrispondono ai pagamenti dei flussi di cassa"
			},
			{
				name: "ipotesi",
				description: "è un numero che si suppone vicino al risultato di TIR"
			}
		]
	},
	{
		name: "TRONCA",
		description: "Elimina la parte decimale di un numero.",
		arguments: [
			{
				name: "num",
				description: "è il numero da troncare"
			},
			{
				name: "num_cifre",
				description: "è un numero che specifica la precisione del troncamento, 0 (zero) se omesso"
			}
		]
	},
	{
		name: "TROVA",
		description: "Trova una stringa di testo all'interno di un altra stringa e restituisce il numero corrispondente alla posizione iniziale della stringa trovata. La funzione distingue tra maiuscole e minuscole.",
		arguments: [
			{
				name: "testo",
				description: " Utilizzare le virgolette (testo vuoto) per trovare una corrispondenza con il primo carattere nella casella Stringa. Non sono ammessi caratteri jolly."
			},
			{
				name: "stringa",
				description: "è il testo contenente il testo da trovare"
			},
			{
				name: "inizio",
				description: "specifica il carattere in corrispondenza del quale iniziare la ricerca. Il primo carattere nella casella Stringa è il carattere numero 1. Se viene omesso, Inizio = 1"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Restituisce il numero (punto di codice) corrispondente al primo carattere del testo.",
		arguments: [
			{
				name: "testo",
				description: "è il carattere per il quale si calcola il valore Unicode"
			}
		]
	},
	{
		name: "VA",
		description: "Restituisce il valore attuale di un investimento: l'ammontare totale del valore attuale di una serie di pagamenti futuri.",
		arguments: [
			{
				name: "tasso_int",
				description: "è il tasso d'interesse per periodo. Ad esempio, usare 6%/4 per pagamenti trimestrali al 6%."
			},
			{
				name: "periodi",
				description: "è il numero totale dei periodi in un investimento"
			},
			{
				name: "pagam",
				description: "è il pagamento effettuato in ciascun periodo e non può variare nel corso dell'annualità"
			},
			{
				name: "val_futuro",
				description: "è il valore futuro o il saldo in contanti da conseguire dopo l'ultimo pagamento"
			},
			{
				name: "tipo",
				description: "è un valore logico: pagamento all'inizio del periodo = 1; pagamento alla fine del periodo = 0 oppure omesso"
			}
		]
	},
	{
		name: "VAL.DISPARI",
		description: "Restituisce VERO se il numero è dispari.",
		arguments: [
			{
				name: "num",
				description: "è il valore da esaminare"
			}
		]
	},
	{
		name: "VAL.ERR",
		description: "Controlla se il valore è un errore (#VALORE!, #RIF!, #DIV/0!, #NUM!, #NOME? o #NULLO!) ad eccezione di #N/D e restituisce VERO o FALSO.",
		arguments: [
			{
				name: "val",
				description: "è il valore da verificare. Il valore può riferirsi a una cella, a una formula o a un nome che fa riferimento a una cella, a una formula o a un valore"
			}
		]
	},
	{
		name: "VAL.ERRORE",
		description: "Controlla se il valore è un errore (#N/D, #VALORE!, #RIF!, #DIV/0!, #NUM!, #NOME? o #NULLO!), e restituisce VERO o FALSO.",
		arguments: [
			{
				name: "val",
				description: "è il valore da verificare. Il valore può riferirsi a una cella, a una formula o a un nome che fa riferimento a una cella, a una formula o a un valore"
			}
		]
	},
	{
		name: "VAL.FORMULA",
		description: "Controlla se il riferimento specificato è a una cella contenente una formula e restituisce VERO o FALSO.",
		arguments: [
			{
				name: "riferimento",
				description: "è un riferimento alla cella da verificare. Riferimento può essere un riferimento di cella, una formula o un nome che fa riferimento a una cella"
			}
		]
	},
	{
		name: "VAL.FUT",
		description: "Restituisce il valore futuro di un investimento dati pagamenti periodici costanti e un tasso di interesse costante.",
		arguments: [
			{
				name: "tasso_int",
				description: "è il tasso di interesse per periodo. Ad esempio, usare 6%/4 per pagamenti trimestrali al 6%."
			},
			{
				name: "periodi",
				description: "è il numero totale dei periodi di pagamento nell'investimento"
			},
			{
				name: "pagam",
				description: "è il pagamento effettuato in ciascun periodo e non può variare nel corso dell'investimento"
			},
			{
				name: "val_attuale",
				description: "è il valore attuale o la somma forfettaria pari al valore attuale di una serie di pagamenti futuri. Se viene omesso, Pv = 0"
			},
			{
				name: "tipo",
				description: "è un valore corrispondente al momento del pagamento: un pagamento all'inizio del periodo = 1; un pagamento alla fine del periodo = 0 oppure omesso"
			}
		]
	},
	{
		name: "VAL.FUT.CAPITALE",
		description: "Restituisce il valore futuro di un capitale iniziale dopo l'applicazione di una serie di tassi di interesse composto.",
		arguments: [
			{
				name: "capitale",
				description: "è il valore attuale"
			},
			{
				name: "piano_invest",
				description: "è una matrice di tassi di interesse da applicare"
			}
		]
	},
	{
		name: "VAL.LOGICO",
		description: "Restituisce VERO se Valore è un valore logico, VERO o FALSO.",
		arguments: [
			{
				name: "val",
				description: "è il valore da verificare. Valore può riferirsi a una cella, a una formula o a un nome che fa riferimento a una cella, a una formula o a un valore"
			}
		]
	},
	{
		name: "VAL.NON.DISP",
		description: "Controlla se un valore è #N/D e restituisce VERO o FALSO.",
		arguments: [
			{
				name: "val",
				description: "è il valore da verificare. Il valore può riferirsi a una cella, a una formula o a un nome che fa riferimento a una cella, a una formula o a un valore"
			}
		]
	},
	{
		name: "VAL.NON.TESTO",
		description: "Restituisce VERO se il valore non è del testo (le celle vuote non sono testo).",
		arguments: [
			{
				name: "val",
				description: "è il valore da verificare: una cella, una formula o un nome che fa riferimento a una cella, a una formula o a un valore"
			}
		]
	},
	{
		name: "VAL.NUMERO",
		description: "Restituisce VERO se il valore è un numero.",
		arguments: [
			{
				name: "val",
				description: "è il valore da verificare. Valore può riferirsi a una cella, a una formula o a un nome che fa riferimento a una cella, a una formula o a un valore"
			}
		]
	},
	{
		name: "VAL.PARI",
		description: "Restituisce VERO se il numero è pari.",
		arguments: [
			{
				name: "num",
				description: "è il valore da esaminare"
			}
		]
	},
	{
		name: "VAL.RIF",
		description: "Controlla se il valore è un riferimento e restituisce VERO o FALSO.",
		arguments: [
			{
				name: "val",
				description: "è il valore da verificare. Valore può riferirsi a una cella, a una formula o a un nome che fa riferimento a una cella, a una formula o a un valore"
			}
		]
	},
	{
		name: "VAL.TESTO",
		description: "Restituisce VERO se il valore è un testo.",
		arguments: [
			{
				name: "val",
				description: "è il valore da verificare. Valore può riferirsi a una cella, a una formula o a un nome che si riferisce a una cella, a una formula o a un valore."
			}
		]
	},
	{
		name: "VAL.VUOTO",
		description: "Restituisce VERO se il valore è una cella vuota.",
		arguments: [
			{
				name: "val",
				description: "è la cella o il nome che fa riferimento alla cella da verificare"
			}
		]
	},
	{
		name: "VALORE",
		description: "Converte una stringa di testo che rappresenta un numero in una stringa di testo.",
		arguments: [
			{
				name: "testo",
				description: "è il testo racchiuso tra virgolette o un riferimento a una cella contenente il testo da convertire"
			}
		]
	},
	{
		name: "VALUTA",
		description: "Converte un numero in testo utilizzando un formato valuta.",
		arguments: [
			{
				name: "num",
				description: "è un numero, un riferimento a una cella contenente un numero o una formula che calcola un numero"
			},
			{
				name: "decimali",
				description: "è il numero di cifre a destra della virgola decimale. Il numero verrà arrotondato come necessario; se viene omesso, Decimali = 2"
			}
		]
	},
	{
		name: "VALUTA.DEC",
		description: "Converte un prezzo espresso come frazione in un prezzo espresso come numero decimale.",
		arguments: [
			{
				name: "valuta_frazione",
				description: "è un numero espresso come frazione"
			},
			{
				name: "frazione",
				description: "è l'intero che costituisce il denominatore della frazione"
			}
		]
	},
	{
		name: "VALUTA.FRAZ",
		description: "Converte un prezzo espresso come numero decimale in un prezzo espresso come frazione.",
		arguments: [
			{
				name: "valuta_decimale",
				description: "è un numero decimale"
			},
			{
				name: "frazione",
				description: "è l'intero che costituisce il denominatore della frazione"
			}
		]
	},
	{
		name: "VAN",
		description: "Restituisce il valore attuale netto di un investimento basato su una serie di uscite (valori negativi) e di entrate (valori positivi) future.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tasso_int",
				description: "è il tasso di sconto per la durata di un periodo"
			},
			{
				name: "val1",
				description: "sono da 1 a 254 entrate e uscite periodiche al termine di ogni periodo"
			},
			{
				name: "val2",
				description: "sono da 1 a 254 entrate e uscite periodiche al termine di ogni periodo"
			}
		]
	},
	{
		name: "VAN.X",
		description: "Restituisce il valore attuale netto per un impiego di flussi di cassa.",
		arguments: [
			{
				name: "tasso_int",
				description: "è il tasso di sconto sui flussi di cassa"
			},
			{
				name: "valori",
				description: "è una serie di flussi di cassa che corrispondono alle scadenze di pagamento"
			},
			{
				name: "date_pagam",
				description: "sono le scadenze di pagamento che corrispondono ai pagamenti dei flussi di cassa"
			}
		]
	},
	{
		name: "VAR",
		description: "Stima la varianza sulla base di un campione. Ignora i valori logici e il testo nel campione.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti numerici corrispondenti a un campione della popolazione"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti numerici corrispondenti a un campione della popolazione"
			}
		]
	},
	{
		name: "VAR.C",
		description: "Stima la varianza sulla base di un campione. Ignora i valori logici e il testo nel campione.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti numerici corrispondenti a un campione della popolazione"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti numerici corrispondenti a un campione della popolazione"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Calcola la varianza sulla base dell'intera popolazione. Ignora i valori logici e il testo nella popolazione.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti numerici corrispondenti a una popolazione"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti numerici corrispondenti a una popolazione"
			}
		]
	},
	{
		name: "VAR.POP",
		description: "Calcola la varianza sulla base dell'intera popolazione. Ignora i valori logici e il testo nella popolazione.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "num1",
				description: "sono da 1 a 255 argomenti numerici corrispondenti a una popolazione"
			},
			{
				name: "num2",
				description: "sono da 1 a 255 argomenti numerici corrispondenti a una popolazione"
			}
		]
	},
	{
		name: "VAR.POP.VALORI",
		description: "Calcola la varianza sulla base dell'intera popolazione, inclusi valori logici e testo. Il testo e il valore FALSO vengono valutati come 0, il valore VERO come 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "val1",
				description: "sono da 1 a 255 argomenti di tipo valore corrispondenti a una popolazione"
			},
			{
				name: "val2",
				description: "sono da 1 a 255 argomenti di tipo valore corrispondenti a una popolazione"
			}
		]
	},
	{
		name: "VAR.VALORI",
		description: "Restituisce una stima della varianza sulla base di un campione, inclusi valori logici e testo. Il testo e il valore FALSO vengono valutati come 0, il valore VERO come 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "val1",
				description: "sono da 1 a 255 argomenti di valori corrispondenti a un campione di popolazione"
			},
			{
				name: "val2",
				description: "sono da 1 a 255 argomenti di valori corrispondenti a un campione di popolazione"
			}
		]
	},
	{
		name: "VERO",
		description: "Restituisce il valore logico VERO.",
		arguments: [
		]
	},
	{
		name: "WEIBULL",
		description: "Restituisce la distribuzione di Weibull.",
		arguments: [
			{
				name: "x",
				description: "è il valore in cui si calcola la funzione, un numero non negativo"
			},
			{
				name: "alfa",
				description: "è un parametro per la distribuzione, un numero positivo"
			},
			{
				name: "beta",
				description: "è un parametro per la distribuzione, un numero positivo"
			},
			{
				name: "cumulativo",
				description: "è un valore logico: utilizzare VERO per la funzione distribuzione cumulativa; utilizzare FALSO per la funzione probabilità di massa"
			}
		]
	},
	{
		name: "XOR",
		description: "Restituisce un 'OR esclusivo' logico di tutti gli argomenti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logico1",
				description: "sono le condizioni da 1 a 254 da verificare che possono essere VERO o FALSO e possono essere valori logici, matrici o riferimenti"
			},
			{
				name: "logico2",
				description: "sono le condizioni da 1 a 254 da verificare che possono essere VERO o FALSO e possono essere valori logici, matrici o riferimenti"
			}
		]
	}
];