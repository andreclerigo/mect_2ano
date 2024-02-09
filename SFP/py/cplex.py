import pulp

#dados
lucro = [4,2,3]
consumo = [
    [7,3,6],
    [4,4,5]
]
disponibilidade =[150,200]
procura = [5,10,5]

#conjuntos

Produtos=range(0,len(lucro))
Recursos=range(0,len(disponibilidade))

#Variaveis de decisao

x=pulp.LpVariable.dicts('x',Produtos,cat=pulp.LpContinuous,lowBound=0)

#criar modelo
modelo= pulp.LpProblem('locoes',pulp.LpMaximize)

#Funcao Objetivo
modelo += sum(lucro[j]*x[j] for j in Produtos)

#restrições
for i in Recursos:
    modelo += sum(consumo[i][j]*x[j] for j in Produtos) <= disponibilidade[i]
for j in Produtos:
    modelo += x[j] <= procura[j]
for j in Produtos:
    modelo += x[j] >= 0.25 *sum(x[k] for k in Produtos)

    modelo.writeLP('mylp')

#Resolver
status = modelo.solve()
if status == pulp.LpStatusOptimal:
    print(modelo.objective.value())
    for j in Produtos:
        print(x[j].varValue)
