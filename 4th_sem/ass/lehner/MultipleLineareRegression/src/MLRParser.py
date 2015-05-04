#!/usr/bin/python

from pyparsing import *
import CSVParser
import MLRCalc

keyword = Literal("lm").suppress()
leftBracket,rightBracket,semicolon,comma,equal,tild = map(Suppress, "();,=~")
# variable = Regex('[a-zA-Z_][a-zA-Z_0-9]*')
variable = Word(alphas)
integer = Regex('([+-]?(([1-9][0-9]*)|0+))')

delimiter = Literal("|").suppress()
operator = oneOf("+ - * / [ ] . ^ { } %")

alpha = variable("alpha")
beta = variable("beta")
data = oneOf("csv sql excel")
path = Regex(r'([A-Z][:][\\])?[a-zA-Z_0-9.\\]*')
dataTerm = data + equal + path
dataTerms = delimitedList(dataTerm("dataTerms*"))

termPart  = nestedExpr() | nestedExpr('{','}') | integer | operator
termExpr  = OneOrMore(termPart)
term = (Optional(beta) + Optional(termExpr) + Optional(beta))
variables = delimitedList(term("variables*"))


dsl = keyword + leftBracket + alpha + tild + variables + delimiter + dataTerms + rightBracket
inputString = delimitedList(dsl("inputString*"), delim=semicolon)

class DuplicateKeysError(Exception):
	def __init__(self, msg):
		self.msg = msg
	def __str__(self):
		return repr(self.msg)

class KeyNotFoundError(Exception):
	def __init__(self, msg):
		self.msg = msg
	def __str__(self):
		return repr(self.msg)

def isKeyDuplicate(variables, sources):
	for variable in variables:
		trueCount = 0
		for source in sources:
			keys = []
			if source[0] == 'csv':
				keys = CSVParser.getKeys(source[1])
			elif source[0] == 'sql':
				print "sql"
			elif source[0] == 'excel':
				print "excel"

			if variable.beta:
				if variable.beta in keys:
					trueCount += 1
			# has nested term
			else:
				for var in variable:
					if isinstance(var, basestring) is not True:
						for key in var:
							if key in keys:
								trueCount += 1
		#print trueCount
		if trueCount > 1:
			msg = 'Duplicated x-value found in sources: '
			for sourceString in sources:
				msg += ' ' + sourceString[1]
			raise DuplicateKeysError(msg)

def getDataSourceFromKey(key, dataSources):
	for dataSource in dataSources:
		keys = []
		if dataSource[0] == 'csv':
			keys = CSVParser.getKeys(dataSource[1])
		elif dataSource[0] == 'sql':
			keys = [] #TODO SQL Parser
		elif dataSource[0] == 'excel':	
			keys = [] #TODO excel Parser

		if isinstance(key, basestring) is not True:
			for keyValue in key:
				if keyValue in keys:
					return dataSource
		else:
			if key in keys:
				return dataSource
	return ''
		

#result = dsl.parseString("lm(y ~ x1+2, x2, x3 | csv=mydata,sql=asdf);")
# result = dsl.parseString(r"lm(Sales ~ TV * 2, 4 / Radio, 3 * (Newspaper ^ 2) | csv=advertising.csv);")
code = """
lm(Sales ~ TV * 2, 4 / Radio, 3 * (Newspaper ^ 2) | csv=advertising_sourceA.csv, csv=advertising_sourceB.csv);
lm(Sales ~ TV, Radio, Newspaper | csv=advertising.csv)
"""

result = inputString.parseString(code)
test = code.split(';')
# print result.inputString
# print result.alpha
# print result.variables
# print result.beta
idx = 0
for linearRegression in result.inputString:
	isKeyDuplicate(linearRegression.variables, linearRegression.dataTerms)
	print ''
	print test[idx].strip()
	dataArray = []
	for term in linearRegression.variables:
		for key in term:
			data = getDataSourceFromKey(key, linearRegression.dataTerms)
			if data:
				if data[0] == 'csv':
					dataArray.append(CSVParser.csvDataFromTerm(term, data[1]))
				elif data[0] == 'sql':
					print 'sql'
				elif data[0] == 'excel':
					print 'excel'

	xArray = []
	for i in range(len(dataArray)):
		xArray.append(dataArray[i])

	alphaDataSource = getDataSourceFromKey(linearRegression.alpha, linearRegression.dataTerms)
	if alphaDataSource:
		data = []
		if alphaDataSource[0] == 'csv':
			data = CSVParser.csvDataFromTerm([linearRegression.alpha], alphaDataSource[1])
		elif alphaDataSource[0] == 'sql':
			print 'sql'
		elif alphaDataSource[0] == 'excel':
			print 'excel'
		MLRCalc.calcCoeff(data, xArray)
	else:
		raise KeyNotFoundError('Key not found in sources: ' + linearRegression.alpha)
	idx += 1



