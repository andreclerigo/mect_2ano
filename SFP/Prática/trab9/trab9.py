import pymysql
from matplotlib import pyplot as plt

con = pymysql.connect(user='root', password='', host='localhost', database='Reservatorio')
cur=con.cursor()
cur.execute('SELECT * from 	supervisaoreservatorio')
rows=cur.fetchall()
x=[]
y=[]
for col in rows:
    x.append(col[0])
    y.append( col[9])
cur.close()
con.close()

plt.plot(x,y)
plt.title('Fluxo luminoso')
plt.xlabel('id')
plt.ylabel('Fluxo')
plt.legend(["Legenda Fluxo"])
plt.show()