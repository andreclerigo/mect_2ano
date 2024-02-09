import pandas as pd

df = pd.DataFrame( # Cria um objecto do tipo DataFrame
    [
        ['Jan',58,42], # Cria uma linha da DataFrame
        ['Fev',68,42],
        ['Dec',48,42],
    ],
    index = [0,1,2],
    columns = ['month','avg_high','avg_low'] # Cria o cabeçalho da DataFrame
)
print(df) # imprime a tabela criada, com cabeçalho e número da linha (index)
 # month avg_high avg_low
 # 0 Jan 58 42
 # 1 Fev 68 42
 # 2 Dec 48 42
df.to_csv('modified.csv',index=False) # guarda os dados do objecto panda “df”
 # no ficheiro de texto ‘modified.csv’
dd=pd.read_csv('modified.csv') # lê o ficheiro de texto ‘modified.csv’ ,
 # cria e inicializa o objecto do tipo panda ,“dd”
print(dd)
df.to_excel('modified.xlsx') # guarda o dados no ficheiro ‘modified.xlsx’
dg=pd.read_excel('modified.xlsx',index_col=0) # Lê o ficheiro , cria o objecto “dg” e inicializa-o
print(dg) 