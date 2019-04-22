#Escalonador

def findWaitingTime(processes, n, 
                    bt, wt): #tempo para todos os processos
      
    wt[0] = 0 #tempo do primeiro processo é 0
    
    for i in range(1, n ): 
        wt[i] = bt[i - 1] + wt[i - 1]  #calculando tempo de espera
  
def findTurnAroundTime(processes, n,  
                       bt, wt, tat): #função para troca
    
    for i in range(n): 
        tat[i] = bt[i] + wt[i] # calculando tempo de troca com bt[i] + wt[i] 

def findavgTime( processes, n, bt): #Função para calcular tempo médio
  
    wt = [0] * n 
    tat = [0] * n  
    total_wt = 0
    total_tat = 0
    
    findWaitingTime(processes, n, bt, wt) #Função apra calcular tempo de espera de todos os processos 
    
    findTurnAroundTime(processes, n,  
                       bt, wt, tat) #Função para localizar tempo de troca em todos os processos
    
    print( "Tempo de burst " + 
                  " Tempo de espera " + 
                " Tempo de troca") #Mostrar processos com todos os detalhes
    
    for i in range(n): #Calcula tempo total de espera e tempo total de troca
        total_wt = total_wt + wt[i] 
        total_tat = total_tat + tat[i] 
        print(" " + str(i + 1) + "\t\t" + 
                    str(bt[i]) + "\t " + 
                    str(wt[i]) + "\t\t " + 
                    str(tat[i]))  
  
    print( "Tempo médio de espera = "+
                   str(total_wt / n)) 
    print("Tempo médio de troca = "+
                     str(total_tat / n)) 
  
if __name__ =="__main__": 
    processes = [ 1, 2, 3, 4, 5, 6, 7, 8] #ID de processos
    n = len(processes)

    burst_time = [12, 5, 8, 5, 17, 4, 1, 15] #Tempo de burst de todos os processos
    findavgTime(processes, n, burst_time)